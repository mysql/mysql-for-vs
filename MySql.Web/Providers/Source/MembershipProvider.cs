// Copyright (C) 2007 MySQL AB
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 2 as published by
// the Free Software Foundation
//
// There are special exceptions to the terms and conditions of the GPL 
// as it is applied to this software. View the full text of the 
// exception in file EXCEPTIONS in the directory of this software 
// distribution.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

//  This code was contributed by Sean Wright (srwright@alcor.concordia.ca) on 2007-01-12
//  The copyright was assigned and transferred under the terms of
//  the MySQL Contributor License Agreement (CLA)

using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Security;
using MySql.Data.MySqlClient;
using MySql.Web.Properties;

namespace MySql.Web.Security
{
    /// <summary>
    /// Manages storage of membership information for an ASP.NET application in a MySQL database. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class is used by the <see cref="Membership"/> and <see cref="MembershipUser"/> classes
    /// to provide membership services for ASP.NET applications using a MySQL database.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code lang="" source="CodeExamples\MembershipCodeExample2.xml"/>
    /// </example>
    public sealed class MySQLMembershipProvider : MembershipProvider
    {
        private int newPasswordLength = 8;
        private string eventSource = "MySQLMembershipProvider";
        private string eventLog = "Application";
        private string exceptionMessage = "An exception occurred. Please check the Event Log.";
        private string connectionString;
        private int minRequiredPasswordLength;
        private bool writeExceptionsToEventLog;
        private string applicationName;
        private bool enablePasswordReset;
        private bool enablePasswordRetrieval;
        private bool requiresQuestionAndAnswer;
        private bool requiresUniqueEmail;
        private int maxInvalidPasswordAttempts;
        private int passwordAttemptWindow;
        private MembershipPasswordFormat passwordFormat;
        private int minRequiredNonAlphanumericCharacters;
        private string passwordStrengthRegularExpression;
        private int applicationId;

        /// <summary>
        /// Initializes the MySQL membership provider with the property values specified in the 
        /// ASP.NET application's configuration file. This method is not intended to be used directly 
        /// from your code. 
        /// </summary>
        /// <param name="name">The name of the <see cref="MySQLMembershipProvider"/> instance to initialize.</param>
        /// <param name="config">A collection of the name/value pairs representing the 
        /// provider-specific attributes specified in the configuration for this provider.</param>
        /// <exception cref="T:System.ArgumentNullException">config is a null reference.</exception>
        /// <exception cref="T:System.InvalidOperationException">An attempt is made to call <see cref="M:System.Configuration.Provider.ProviderBase.Initialize(System.String,System.Collections.Specialized.NameValueCollection)"/> on a provider after the provider has already been initialized.</exception>
        /// <exception cref="T:System.Configuration.Provider.ProviderException"></exception>
        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }
            if (name == null || name.Length == 0)
            {
                name = "MySQLMembershipProvider";
            }
            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "MySQL Membership provider");
            }
            base.Initialize(name, config);

            applicationName = GetConfigValue(config["applicationName"], 
                HostingEnvironment.ApplicationVirtualPath);
            maxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(config["maxInvalidPasswordAttempts"], "5"));
            passwordAttemptWindow = Convert.ToInt32(GetConfigValue(config["passwordAttemptWindow"], "10"));
            minRequiredNonAlphanumericCharacters =
                Convert.ToInt32(GetConfigValue(config["minRequiredAlphaNumericCharacters"], "1"));
            minRequiredPasswordLength = Convert.ToInt32(GetConfigValue(config["minRequiredPasswordLength"], "7"));
            passwordStrengthRegularExpression =
                Convert.ToString(GetConfigValue(config["passwordStrengthRegularExpression"], ""));
            enablePasswordReset = Convert.ToBoolean(GetConfigValue(config["enablePasswordReset"], "True"));
            enablePasswordRetrieval = Convert.ToBoolean(
                GetConfigValue(config["enablePasswordRetrieval"], "False"));
            requiresQuestionAndAnswer = Convert.ToBoolean(GetConfigValue(config["requiresQuestionAndAnswer"], "False"));
            requiresUniqueEmail = Convert.ToBoolean(GetConfigValue(config["requiresUniqueEmail"], "True"));
            writeExceptionsToEventLog = Convert.ToBoolean(GetConfigValue(config["writeExceptionsToEventLog"], "True"));
            string temp_format = config["passwordFormat"];
            if (temp_format == null)
                temp_format = "Hashed";

            if (temp_format == "Hashed")
                passwordFormat = MembershipPasswordFormat.Hashed;
            else if (temp_format == "Encrypted")
                passwordFormat = MembershipPasswordFormat.Encrypted;
            else if (temp_format == "Clear")
                passwordFormat = MembershipPasswordFormat.Clear;
            else
                throw new ProviderException("Password format not supported.");

            // if the user is asking for the ability to retrieve hashed passwords, then let
            // them know we can't
            if (PasswordFormat == MembershipPasswordFormat.Hashed && EnablePasswordRetrieval)
                throw new ProviderException(Resources.CannotRetrieveHashedPasswords);

            ConnectionStringSettings ConnectionStringSettings = ConfigurationManager.ConnectionStrings[
                config["connectionStringName"]];
            if (ConnectionStringSettings != null)
                connectionString = ConnectionStringSettings.ConnectionString.Trim();
            else
                connectionString = "";

            // make sure we have the correct schema
            SchemaManager.CheckSchema(connectionString, config);

            try
            {
                // now pre-cache the applicationId
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    MySqlCommand cmd = new MySqlCommand("SELECT id FROM my_aspnet_Applications WHERE name=@name", conn);
                    applicationId = (int)cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                throw new ProviderException("There was an error during role provider initilization.", ex);
            }
        }

        private static string GetConfigValue(string configValue, string defaultValue)
        {
            if (string.IsNullOrEmpty(configValue))
            {
                return defaultValue;
            }
            return configValue;
        }

        #region Properties

        /// <summary>
        /// The name of the application using the MySQL membership provider.
        /// </summary>
        /// <value>The name of the application using the MySQL membership provider.  The default is the 
        /// application virtual path.</value>
        /// <remarks>The ApplicationName is used by the MySqlMembershipProvider to separate 
        /// membership information for multiple applications.  Using different application names, 
        /// applications can use the same membership database.
        /// Likewise, multiple applications can make use of the same membership data by simply using
        /// the same application name.
        /// Caution should be taken with multiple applications as the ApplicationName property is not
        /// thread safe during writes.
        /// </remarks>
        /// <example>
        /// The following example shows the membership element being used in an applications web.config file.
        /// The application name setting is being used.
        /// <code lang="" source="CodeExamples\MembershipCodeExample1.xml"/>
        /// </example>
        public override string ApplicationName
        {
            get { return applicationName; }
            set { applicationName = value; }
        }

        /// <summary>
        /// Indicates whether the membership provider is configured to allow users to reset their passwords.
        /// </summary>
        /// <value>true if the membership provider supports password reset; otherwise, false. The default is true.</value>
        /// <remarks>Allows the user to replace their password with a new, randomly generated password.  
        /// This can be especially handy when using hashed passwords since hashed passwords cannot be
        /// retrieved.</remarks>
        /// <example>
        /// The following example shows the membership element being used in an applications web.config file.
        /// <code source="CodeExamples\MembershipCodeExample1.xml"/>
        /// </example>
        public override bool EnablePasswordReset
        {
            get { return enablePasswordReset; }
        }

        /// <summary>
        /// Indicates whether the membership provider is configured to allow users to retrieve 
        /// their passwords.
        /// </summary>
        /// <value>true if the membership provider is configured to support password retrieval; 
        /// otherwise, false. The default is false.</value>
        /// <remarks>If the system is configured to use hashed passwords, then retrieval is not possible.  
        /// If the user attempts to initialize the provider with hashed passwords and enable password retrieval
        /// set to true then a <see cref="ProviderException"/> is thrown.</remarks>
        /// <example>
        /// The following example shows the membership element being used in an applications web.config file.
        /// <code source="CodeExamples\MembershipCodeExample1.xml"/>
        /// </example>
        public override bool EnablePasswordRetrieval
        {
            get { return enablePasswordRetrieval; }
        }

        /// <summary>
        /// Gets a value indicating whether the membership provider is 
        /// configured to require the user to answer a password question 
        /// for password reset and retrieval.
        /// </summary>
        /// <value>true if a password answer is required for password 
        /// reset and retrieval; otherwise, false. The default is false.</value>
        /// <example>
        /// The following example shows the membership element being used in an applications web.config file.
        /// <code source="CodeExamples\MembershipCodeExample1.xml"/>
        /// </example>
        public override bool RequiresQuestionAndAnswer
        {
            get { return requiresQuestionAndAnswer; }
        }

        /// <summary>
        /// Gets a value indicating whether the membership provider is configured 
        /// to require a unique e-mail address for each user name.
        /// </summary>
        /// <value>true if the membership provider requires a unique e-mail address; 
        /// otherwise, false. The default is true.</value>
        /// <example>
        /// The following example shows the membership element being used in an applications web.config file.
        /// <code source="CodeExamples\MembershipCodeExample1.xml"/>
        /// </example>
        public override bool RequiresUniqueEmail
        {
            get { return requiresUniqueEmail; }
        }

        /// <summary>
        /// Gets the number of invalid password or password-answer attempts allowed 
        /// before the membership user is locked out.
        /// </summary>
        /// <value>The number of invalid password or password-answer attempts allowed 
        /// before the membership user is locked out.</value>
        /// <example>
        /// The following example shows the membership element being used in an applications web.config file.
        /// <code source="CodeExamples\MembershipCodeExample1.xml"/>
        /// </example>
        public override int MaxInvalidPasswordAttempts
        {
            get { return maxInvalidPasswordAttempts; }
        }

        /// <summary>
        /// Gets the number of minutes in which a maximum number of invalid password or 
        /// password-answer attempts are allowed before the membership user is locked out.
        /// </summary>
        /// <value>The number of minutes in which a maximum number of invalid password or 
        /// password-answer attempts are allowed before the membership user is locked out.</value>
        /// <example>
        /// The following example shows the membership element being used in an applications web.config file.
        /// <code source="CodeExamples\MembershipCodeExample1.xml"/>
        /// </example>
        public override int PasswordAttemptWindow
        {
            get { return passwordAttemptWindow; }
        }

        /// <summary>
        /// Gets a value indicating the format for storing passwords in the membership data store.
        /// </summary>
        /// <value>One of the <see cref="T:System.Web.Security.MembershipPasswordFormat"/> 
        /// values indicating the format for storing passwords in the data store.</value>
        /// <example>
        /// The following example shows the membership element being used in an applications web.config file.
        /// <code source="CodeExamples\MembershipCodeExample1.xml"/>
        /// </example>
        public override MembershipPasswordFormat PasswordFormat
        {
            get { return passwordFormat; }
        }

        /// <summary>
        /// Gets the minimum number of special characters that must be present in a valid password.
        /// </summary>
        /// <value>The minimum number of special characters that must be present 
        /// in a valid password.</value>
        /// <example>
        /// The following example shows the membership element being used in an applications web.config file.
        /// <code source="CodeExamples\MembershipCodeExample1.xml"/>
        /// </example>
        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return minRequiredNonAlphanumericCharacters; }
        }

        /// <summary>
        /// Gets the minimum length required for a password.
        /// </summary>
        /// <value>The minimum length required for a password. </value>
        /// <example>
        /// The following example shows the membership element being used in an applications web.config file.
        /// <code source="CodeExamples\MembershipCodeExample1.xml"/>
        /// </example>
        public override int MinRequiredPasswordLength
        {
            get { return minRequiredPasswordLength; }
        }

        /// <summary>
        /// Gets the regular expression used to evaluate a password.
        /// </summary>
        /// <value>A regular expression used to evaluate a password.</value>
        /// <example>
        /// The following example shows the membership element being used in an applications web.config file.
        /// In this example, the regular expression specifies that the password must meet the following
        /// criteria:
        /// <ul>
        /// <list>Is at least seven characters.</list>
        /// <list>Contains at least one digit.</list>
        /// <list>Contains at least one special (non-alphanumeric) character.</list>
        /// </ul>
        /// <code source="CodeExamples\MembershipCodeExample1.xml"/>
        /// </example>
        public override string PasswordStrengthRegularExpression
        {
            get { return passwordStrengthRegularExpression; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether exceptions are written to the event log.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if exceptions should be written to the log; otherwise, <c>false</c>.
        /// </value>
        public bool WriteExceptionsToEventLog
        {
            get { return writeExceptionsToEventLog; }
            set { writeExceptionsToEventLog = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Changes the password.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="oldPwd">The old password.</param>
        /// <param name="newPwd">The new password.</param>
        /// <returns>true if the password was updated successfully, false if the supplied old password
        /// is invalid, the user is locked out, or the user does not exist in the database.</returns>
        public override bool ChangePassword(string username, string oldPwd, string newPwd)
        {
            // this will return false if the username doesn't exist
            if (!(ValidateUser(username, oldPwd)))
                return false;

            ValidatePasswordEventArgs args = new ValidatePasswordEventArgs(username, newPwd, true);
            OnValidatingPassword(args);
            if (args.Cancel)
            {
                if (!(args.FailureInformation == null))
                    throw args.FailureInformation;
                else
                    throw new ProviderException("Change password canceled due to New password validation failure.");
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // retrieve the existing key and format for this user
                    string passwordKey;
                    MembershipPasswordFormat passwordFormat;
                    GetPasswordInfo(conn, username, out passwordKey, out passwordFormat);
                    int userId = GetUserId(conn, username);

                    MySqlCommand cmd = new MySqlCommand(
                        @"UPDATE my_aspnet_Membership
                        SET Password = @pass, LastPasswordChangedDate = @lastPasswordChangedDate 
                        WHERE userId=@userId", conn);
                    cmd.Parameters.AddWithValue("@pass", 
                        EncodePassword(newPwd, passwordKey, passwordFormat));
                    cmd.Parameters.AddWithValue("@lastPasswordChangedDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (MySqlException e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "ChangePassword");
                throw new ProviderException(exceptionMessage, e);
            }
        }

        /// <summary>
        /// Changes the password question and answer.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="newPwdQuestion">The new password question.</param>
        /// <param name="newPwdAnswer">The new password answer.</param>
        /// <returns>true if the update was successful; otherwise, false. A value of false is 
        /// also returned if the password is incorrect, the user is locked out, or the user 
        /// does not exist in the database.</returns>
        public override bool ChangePasswordQuestionAndAnswer(string username,
            string password, string newPwdQuestion, string newPwdAnswer)
        {
            // this handles the case where the username doesn't exist
            if (!(ValidateUser(username, password)))
                return false;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string passwordKey;
                    MembershipPasswordFormat passwordFormat;
                    GetPasswordInfo(conn, username, out passwordKey, out passwordFormat);

                    int userId = GetUserId(conn, username);

                    MySqlCommand cmd = new MySqlCommand(
                        @"UPDATE my_aspnet_Membership 
                        SET PasswordQuestion = @passQuestion, PasswordAnswer = @passAnswer
                        WHERE userId=@userId", conn);
                    cmd.Parameters.AddWithValue("@passQuestion", newPwdQuestion);
                    cmd.Parameters.AddWithValue("@passwordAnswer",
                        EncodePassword(newPwdAnswer, passwordKey, passwordFormat));
                    cmd.Parameters.AddWithValue("@userId", userId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (MySqlException e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "ChangePasswordQuestionAndAnswer");
                throw new ProviderException(exceptionMessage, e);
            }
        }

        /// <summary>
        /// Adds a new membership user to the data source.
        /// </summary>
        /// <param name="username">The user name for the new user.</param>
        /// <param name="password">The password for the new user.</param>
        /// <param name="email">The e-mail address for the new user.</param>
        /// <param name="passwordQuestion">The password question for the new user.</param>
        /// <param name="passwordAnswer">The password answer for the new user</param>
        /// <param name="isApproved">Whether or not the new user is approved to be validated.</param>
        /// <param name="providerUserKey">The unique identifier from the membership data source for the user.</param>
        /// <param name="status">A <see cref="T:System.Web.Security.MembershipCreateStatus"/> enumeration value indicating whether the user was created successfully.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUser"/> object populated with the information for the newly created user.
        /// </returns>
        public override MembershipUser CreateUser(string username, string password,
            string email, string passwordQuestion, string passwordAnswer,
            bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            ValidatePasswordEventArgs Args = new ValidatePasswordEventArgs(username, password, true);
            OnValidatingPassword(Args);
            if (Args.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }
            if (RequiresUniqueEmail && GetUserNameByEmail(email) != "")
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            // now check to see if that username is already in use.
            MembershipUser u = GetUser(username, false);
            if (u != null)
            {
                status = MembershipCreateStatus.DuplicateUserName;
                return null;
            }

            string passwordKey = GetPasswordKey();
            DateTime createDate = DateTime.Now;
            if (providerUserKey == null)
            {
                providerUserKey = Guid.NewGuid();
            }
            else
            {
                if (!(providerUserKey is Guid))
                {
                    status = MembershipCreateStatus.InvalidProviderUserKey;
                    return null;
                }
            }
            MySqlConnection conn = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand(
                    @"INSERT INTO mysql_Membership (
PKID, Username, ApplicationName,
                    Email, Comment, Password, PasswordKey, PasswordFormat, 
                    PasswordQuestion, PasswordAnswer, IsApproved, LastActivityDate,
                    LastPasswordChangedDate, CreationDate, 
                    IsLockedOut, LastLockedOutDate,  
                    FailedPasswordAttemptCount, FailedPasswordAttemptWindowStart, 
                    FailedPasswordAnswerAttemptCount, 
                    FailedPasswordAnswerAttemptWindowStart)
                    Values(?PKID, ?Username, ?ApplicationName, ?Email, 
                     ?Comment, ?Password, ?PasswordKey, ?PasswordFormat, 
                    ?PasswordQuestion, ?PasswordAnswer, ?IsApproved, ?LastActivityDate,  
                    ?LastPasswordChangedDate, ?CreationDate, 
                    ?IsLockedOut, ?LastLockedOutDate, 
                    ?FailedPasswordAttemptCount,
                    ?FailedPasswordAttemptWindowStart, 
                    ?FailedPasswordAnswerAttemptCount, 
                    ?FailedPasswordAnswerAttemptWindowStart)",
                    conn);
            cmd.Parameters.AddWithValue("?PKID", providerUserKey.ToString());
            cmd.Parameters.AddWithValue("?Username", username);
            cmd.Parameters.AddWithValue("?ApplicationName", applicationName);
            cmd.Parameters.AddWithValue("?Email", email);
            cmd.Parameters.AddWithValue("?Comment", "");
            cmd.Parameters.AddWithValue("?Password",
                EncodePassword(password, passwordKey, PasswordFormat));
            cmd.Parameters.AddWithValue("?PasswordKey", passwordKey);
            cmd.Parameters.AddWithValue("?PasswordFormat", PasswordFormat);
            cmd.Parameters.AddWithValue("?PasswordQuestion", passwordQuestion);
            cmd.Parameters.AddWithValue("?PasswordAnswer",
                EncodePassword(passwordAnswer, passwordKey, PasswordFormat));
            cmd.Parameters.AddWithValue("?IsApproved", isApproved);
            cmd.Parameters.AddWithValue("?LastActivityDate", createDate);
            cmd.Parameters.AddWithValue("?LastPasswordChangedDate", createDate);
            cmd.Parameters.AddWithValue("?CreationDate", createDate);
            cmd.Parameters.AddWithValue("?IsLockedOut", false);
            cmd.Parameters.AddWithValue("?LastLockedOutDate", createDate);
            cmd.Parameters.AddWithValue("?FailedPasswordAttemptCount", 0);
            cmd.Parameters.AddWithValue("?FailedPasswordAttemptWindowStart", createDate);
            cmd.Parameters.AddWithValue("?FailedPasswordAnswerAttemptCount", 0);
            cmd.Parameters.AddWithValue("?FailedPasswordAnswerAttemptWindowStart", createDate);
            try
            {
                conn.Open();
                int recAdded = cmd.ExecuteNonQuery();
                if (recAdded > 0)
                {
                    status = MembershipCreateStatus.Success;
                }
                else
                {
                    status = MembershipCreateStatus.UserRejected;
                }
            }
            catch (MySqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "CreateUser");
                }
                status = MembershipCreateStatus.ProviderError;
            }
            finally
            {
                conn.Close();
            }
            return GetUser(username, false);
        }

        /// <summary>
        /// Removes a user from the membership data source.
        /// </summary>
        /// <param name="username">The name of the user to delete.</param>
        /// <param name="deleteAllRelatedData">true to delete data related to the user from the database; false to leave data related to the user in the database.</param>
        /// <returns>
        /// true if the user was successfully deleted; otherwise, false.
        /// </returns>
        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    int userId = GetUserId(conn, username);
                    if (-1 == userId) return false;

                    // if we are supposed to delete all related data, then delegate that to those providers
                    if (deleteAllRelatedData)
                    {
                        MySQLRoleProvider.DeleteUserData(conn, userId);
                        MySQLProfileProvider.DeleteUserData(conn, userId);
                    }

                    string sql = @"DELETE {0}m 
                        FROM my_aspnet_Users u, my_aspnet_Membership m
                        WHERE u.id=m.userId AND u.id=@userId";

                    MySqlCommand cmd = new MySqlCommand(
                        String.Format(sql, deleteAllRelatedData ? "u," : ""), conn);
                    cmd.Parameters.AddWithValue("@appId", applicationId);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (MySqlException e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "DeleteUser");
                throw new ProviderException(exceptionMessage, e);
            }
        }

        /// <summary>
        /// Gets a collection of all the users in the data source in pages of data.
        /// </summary>
        /// <param name="pageIndex">The index of the page of results to return. <paramref name="pageIndex"/> is zero-based.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">The total number of matched users.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUserCollection"/> collection that contains a page of <paramref name="pageSize"/><see cref="T:System.Web.Security.MembershipUser"/> objects beginning at the page specified by <paramref name="pageIndex"/>.
        /// </returns>
        public override MembershipUserCollection GetAllUsers(int pageIndex, 
            int pageSize, out int totalRecords)
        {
            return GetUsers(null, null, pageIndex, pageSize, out totalRecords);
        }

        /// <summary>
        /// Gets the number of users currently accessing the application.
        /// </summary>
        /// <returns>
        /// The number of users currently accessing the application.
        /// </returns>
        public override int GetNumberOfUsersOnline()
        {
            TimeSpan onlineSpan = new TimeSpan(0, Membership.UserIsOnlineTimeWindow, 0);
            DateTime compareTime = DateTime.Now.Subtract(onlineSpan);

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(
                            @"SELECT Count(*) FROM my_aspnet_Users
                        WHERE LastActivityDate > @date AND applicationId=@appId", conn);
                    cmd.Parameters.AddWithValue("@date", compareTime);
                    cmd.Parameters.AddWithValue("@appId", applicationId);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (MySqlException e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "GetNumberOfUsersOnline");
                throw new ProviderException(exceptionMessage, e);
            }
        }

        /// <summary>
        /// Gets the password for the specified user name from the data source.
        /// </summary>
        /// <param name="username">The user to retrieve the password for.</param>
        /// <param name="answer">The password answer for the user.</param>
        /// <returns>
        /// The password for the specified user name.
        /// </returns>
        public override string GetPassword(string username, string answer)
        {
            if (!(EnablePasswordRetrieval))
                throw new ProviderException("Password Retrieval Not Enabled.");

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    int userId = GetUserId(conn, username);
                    if (-1 == userId)
                        throw new ProviderException("Username not found.");

                    string sql = @"SELECT password, passwordAnswer, passwordKey, passwordFormat, 
                    isLockedOut FROM my_aspnet_Membership WHERE userId=@userId";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@userId", userId);

                    using (MySqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        reader.Read();
                        if (reader.GetBoolean("IsLockedOut"))
                            throw new MembershipPasswordException("The supplied user is locked out.");

                        string password = reader.GetString("password");
                        string passwordAnswer = reader.GetString("passwordAnswer");
                        string passwordKey = reader.GetString("passwordKey");
                        MembershipPasswordFormat format = (MembershipPasswordFormat)
                            reader.GetInt32(3);

                        if (RequiresQuestionAndAnswer &&
                            !(CheckPassword(answer, passwordAnswer, passwordKey, format)))
                        {
                            UpdateFailureCount(username, "passwordAnswer");
                            throw new MembershipPasswordException("Incorrect password answer.");
                        }
                        if (PasswordFormat == MembershipPasswordFormat.Encrypted)
                        {
                            password = UnEncodePassword(password, format);
                        }
                        return password;
                    }
                }
            }
            catch (MySqlException e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "GetPassword");
                throw new ProviderException(exceptionMessage, e);
            }
        }

        /// <summary>
        /// Gets information from the data source for a user. Provides an option to update the last-activity date/time stamp for the user.
        /// </summary>
        /// <param name="username">The name of the user to get information for.</param>
        /// <param name="userIsOnline">true to update the last-activity date/time stamp for the user; false to return user information without updating the last-activity date/time stamp for the user.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUser"/> object populated with the specified user's information from the data source.
        /// </returns>
        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    int userId = GetUserId(conn, username);
                    if (-1 == userId) return null;

                    string sql = @"SELECT * FROM my_aspnet_Membership WHERE userId=@userId";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    MembershipUser user;
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        user = GetUserFromReader(reader);
                    }
                    if (userIsOnline)
                    {
                        cmd.CommandText =
                            @"UPDATE my_aspnet_Users SET LastActivityDate = @date WHERE id=@userId";
                        cmd.Parameters.AddWithValue("@date", DateTime.Now);
                        cmd.ExecuteNonQuery();
                    }
                    return user;
                }
            }
            catch (MySqlException e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "GetUser(String, Boolean)");
                throw new ProviderException(exceptionMessage);
            }
        }

        /// <summary>
        /// Gets user information from the data source based on the unique identifier for the membership user. Provides an option to update the last-activity date/time stamp for the user.
        /// </summary>
        /// <param name="providerUserKey">The unique identifier for the membership user to get information for.</param>
        /// <param name="userIsOnline">true to update the last-activity date/time stamp for the user; false to return user information without updating the last-activity date/time stamp for the user.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUser"/> object populated with the specified user's information from the data source.
        /// </returns>
        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    MySqlCommand cmd = new MySqlCommand(
                        @"SELECT * FROM my_aspnet_Membership WHERE userId=@userId", conn);
                    cmd.Parameters.AddWithValue("@userId", providerUserKey);

                    MembershipUser user;
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read()) return null;
                        user = GetUserFromReader(reader);
                    }

                    if (userIsOnline)
                    {
                        cmd.CommandText =
                            @"UPDATE my_aspnet_Users SET LastActivityDate = @date WHERE id=@userId";
                        cmd.Parameters.AddWithValue("@date", DateTime.Now);
                        cmd.ExecuteNonQuery();
                    }
                    return user;
                }
            }
            catch (MySqlException e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "GetUser(Object, Boolean)");
                throw new ProviderException(exceptionMessage);
            }
        }

        /// <summary>
        /// Unlocks the user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>true if the membership user was successfully unlocked; 
        /// otherwise, false. A value of false is also returned if the user 
        /// does not exist in the database. </returns>
        public override bool UnlockUser(string username)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    int userId = GetUserId(conn, username);
                    if (-1 == userId) return false;

                    string sql = @"UPDATE my_aspnet_Membership  
                        SET IsLockedOut = false, LastLockedOutDate = @lastDate 
                        WHERE userId=@userId";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@LastLockedOutDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (MySqlException e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "UnlockUser");
                throw new ProviderException(exceptionMessage, e);
            }
        }

        /// <summary>
        /// Gets the user name associated with the specified e-mail address.
        /// </summary>
        /// <param name="email">The e-mail address to search for.</param>
        /// <returns>
        /// The user name associated with the specified e-mail address. If no match is found, return null.
        /// </returns>
        public override string GetUserNameByEmail(string email)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string sql = @"SELECT u.name FROM my_aspnet_Users u
                        JOIN my_aspnet_Membership m ON m.userid=u.id
                        WHERE m.email like @email AND u.applicationId=@appId";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@appId", applicationId);
                    return (string)cmd.ExecuteScalar();
                }
            }
            catch (MySqlException e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "GetUserNameByEmail");
                throw new ProviderException(exceptionMessage);
            }
        }

        /// <summary>
        /// Resets a user's password to a new, automatically generated password.
        /// </summary>
        /// <param name="username">The user to reset the password for.</param>
        /// <param name="answer">The password answer for the specified user.</param>
        /// <returns>The new password for the specified user.</returns>
        public override string ResetPassword(string username, string answer)
        {
            if (!(EnablePasswordReset))
                throw new NotSupportedException("Password Reset is not enabled.");
            if (answer == null && RequiresQuestionAndAnswer)
            {
                UpdateFailureCount(username, "passwordAnswer");
                throw new ProviderException("Password answer required for password Reset.");
            }

            string newPassword = Membership.GeneratePassword(newPasswordLength, MinRequiredNonAlphanumericCharacters);
            ValidatePasswordEventArgs Args = new ValidatePasswordEventArgs(username, newPassword, true);
            OnValidatingPassword(Args);
            if (Args.Cancel)
            {
                if (!(Args.FailureInformation == null))
                    throw Args.FailureInformation;
                else
                    throw new MembershipPasswordException("Reset password canceled due to password validation failure.");
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    int userId = GetUserId(conn, username);
                    if (-1 == userId)
                        throw new ProviderException("Username not found.");

                    MySqlCommand cmd = new MySqlCommand(@"SELECT passwordAnswer, 
                    passwordKey, passwordFormat, IsLockedOut 
                    FROM my_aspnet_Membership WHERE userId=@userId", conn);
                    cmd.Parameters.AddWithValue("@userId", userId);

                    string passwordKey = String.Empty;
                    MembershipPasswordFormat format;
                    using (MySqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        reader.Read();
                        if (reader.GetBoolean("IsLockedOut"))
                            throw new MembershipPasswordException("The supplied user is locked out.");

                        string passwordAnswer = reader.GetString("passwordAnswer");
                        passwordKey = reader.GetString("passwordKey");
                        format = (MembershipPasswordFormat)reader.GetByte("passwordFormat");
                        reader.Close();

                        if (RequiresQuestionAndAnswer &&
                            !CheckPassword(answer, passwordAnswer, passwordKey, format))
                        {
                            UpdateFailureCount(username, "passwordAnswer");
                            throw new MembershipPasswordException("Incorrect password answer.");
                        }
                    }

                    cmd.CommandText = @"UPDATE my_aspnet_Membership 
                        SET Password = @pass, LastPasswordChangedDate = @lastPassChange
                        WHERE userId=@userId";

                    cmd.Parameters.AddWithValue("@pass",
                        EncodePassword(newPassword, passwordKey, format));
                    cmd.Parameters.AddWithValue("@lastPassChange", DateTime.Now);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected != 1)
                        throw new MembershipPasswordException("There was an error resetting the password.");
                    return newPassword;
                }
            }
            catch (MySqlException e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "ResetPassword");
                throw new ProviderException(exceptionMessage, e);
            }
        }

        /// <summary>
        /// Updates information about a user in the data source.
        /// </summary>
        /// <param name="user">A <see cref="T:System.Web.Security.MembershipUser"/> object 
        /// that represents the user to update and the updated information for the user.</param>
        public override void UpdateUser(MembershipUser user)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    int userId = GetUserId(conn, user.UserName);
                    if (-1 == userId)
                        throw new ProviderException("Username not found.");

                    string sql = @"UPDATE my_aspnet_Membership m, my_aspnet_Users u 
                        SET m.email=@email, m.comment=@comment, m.isApproved=@isApproved,
                        m.lastLoginDate=@lastLoginDate, u.lastActivityDate=@lastActivityDate
                        WHERE m.userId=u.id AND u.name LIKE @name AND u.applicationId=@appId";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Comment", user.Comment);
                    cmd.Parameters.AddWithValue("@isApproved", user.IsApproved);
                    cmd.Parameters.AddWithValue("@lastLoginDate", user.LastLoginDate);
                    cmd.Parameters.AddWithValue("@lastActivityDate", user.LastActivityDate);
                    cmd.Parameters.AddWithValue("@name", user.UserName);
                    cmd.Parameters.AddWithValue("@appId", applicationId);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (MySqlException e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "UpdateUser");
                throw new ProviderException(exceptionMessage);
            }
        }

        /// <summary>
        /// Verifies that the specified user name and password exist in the data source.
        /// </summary>
        /// <param name="username">The name of the user to validate.</param>
        /// <param name="password">The password for the specified user.</param>
        /// <returns>
        /// true if the specified username and password are valid; otherwise, false.
        /// </returns>
        public override bool ValidateUser(string username, string password)
        {
            bool isValid = false;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // first get the user id.  If that is -1, then the user doesn't exist
                    // so we just return false since we can't bump any counters
                    int userId = GetUserId(conn, username);
                    if (-1 == userId) return false;

                    string sql = @"SELECT password, passwordKey, passwordFormat, isApproved 
                            FROM my_aspnet_Membership WHERE userId=@userId";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@appId", applicationId);

                    using (MySqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (!reader.HasRows) return false;
                        reader.Read();
                        if (reader.GetBoolean("IsLockedOut")) return false;

                        string pwd = reader.GetString(0);
                        string passwordKey = reader.GetString(1);
                        MembershipPasswordFormat format = (MembershipPasswordFormat)
                            reader.GetInt32(2);
                        bool isApproved = reader.GetBoolean(3);
                        reader.Close();

                        if (!CheckPassword(password, pwd, passwordKey, format))
                            UpdateFailureCount(username, "password");
                        else if (isApproved)
                        {
                            isValid = true;
                            MySqlCommand updateCmd = new MySqlCommand(
                                @"UPDATE my_aspnet_Membership m, my_aspnet_Users u 
                                SET m.LastLoginDate = @lastLoginDate, u.lastActivityDate = @date 
                                WHERE m.userid=@userid AND u.id=@userid", conn);
                            updateCmd.Parameters.AddWithValue("@lastLoginDate", DateTime.Now);
                            updateCmd.Parameters.AddWithValue("@userid", userId);
                            updateCmd.Parameters.AddWithValue("@appId", applicationId);
                            updateCmd.ExecuteNonQuery();
                        }
                    }
                    return isValid;
                }
            }
            catch (MySqlException e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "ValidateUser");
                throw new ProviderException(exceptionMessage, e);
            }
        }

        /// <summary>
        /// Gets a collection of membership users where the user name contains the specified user name to match.
        /// </summary>
        /// <param name="usernameToMatch">The user name to search for.</param>
        /// <param name="pageIndex">The index of the page of results to return. <paramref name="pageIndex"/> is zero-based.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">The total number of matched users.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUserCollection"/> collection that contains a page of <paramref name="pageSize"/><see cref="T:System.Web.Security.MembershipUser"/> objects beginning at the page specified by <paramref name="pageIndex"/>.
        /// </returns>
        public override MembershipUserCollection FindUsersByName(string usernameToMatch,
                                         int pageIndex, int pageSize, out int totalRecords)
        {
            return GetUsers(usernameToMatch, null, pageIndex, pageSize, out totalRecords);
        }

        /// <summary>
        /// Gets a collection of membership users where the e-mail address contains the specified e-mail address to match.
        /// </summary>
        /// <param name="emailToMatch">The e-mail address to search for.</param>
        /// <param name="pageIndex">The index of the page of results to return. <paramref name="pageIndex"/> is zero-based.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">The total number of matched users.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUserCollection"/> collection that contains a page of <paramref name="pageSize"/><see cref="T:System.Web.Security.MembershipUser"/> objects beginning at the page specified by <paramref name="pageIndex"/>.
        /// </returns>
        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex,
                                                                  int pageSize, out int totalRecords)
        {
            return GetUsers(null, emailToMatch, pageIndex, pageSize, out totalRecords);
        }

        #endregion

        #region Private Methods

        private int GetUserId(MySqlConnection connection, string username)
        {
            MySqlCommand cmd = new MySqlCommand("SELECT id FROM my_aspnet_Users WHERE name LIKE @name", connection);
            object id = cmd.ExecuteScalar();
            if (id == null) return -1;
            return (int)id;
        }

        private void WriteToEventLog(Exception e, string action)
        {
            EventLog log = new EventLog();
            log.Source = eventSource;
            log.Log = eventLog;
            string message = "An exception occurred communicating with the data source." +
                             Environment.NewLine + Environment.NewLine;
            message += "Action: " + action + Environment.NewLine + Environment.NewLine;
            message += "Exception: " + e;
            log.WriteEntry(message);
        }

        private MembershipUser GetUserFromReader(MySqlDataReader reader)
        {
            object providerUserKey = reader.GetValue(0);
            string username = reader.GetString(1);

            string email = null;
            if (!reader.IsDBNull(2))
                email = reader.GetString(2);

            string passwordQuestion = "";
            if (!(reader.GetValue(3) == DBNull.Value))
                passwordQuestion = reader.GetString(3);

            string comment = "";
            if (!(reader.GetValue(4) == DBNull.Value))
                comment = reader.GetString(4);

            bool isApproved = reader.GetBoolean(5);
            bool isLockedOut = reader.GetBoolean(6);
            DateTime creationDate = reader.GetDateTime(7);
            DateTime lastLoginDate = new DateTime();
            if (!(reader.GetValue(8) == DBNull.Value))
            {
                lastLoginDate = reader.GetDateTime(8);
            }
            DateTime lastActivityDate = reader.GetDateTime(9);
            DateTime lastPasswordChangedDate = reader.GetDateTime(10);
            DateTime lastLockedOutDate = new DateTime();
            if (!(reader.GetValue(11) == DBNull.Value))
            {
                lastLockedOutDate = reader.GetDateTime(11);
            }
            MembershipUser u =
                new MembershipUser(Name, username, providerUserKey, email, passwordQuestion, comment, isApproved,
                                   isLockedOut, creationDate, lastLoginDate, lastActivityDate, lastPasswordChangedDate,
                                   lastLockedOutDate);
            return u;
        }

        private string UnEncodePassword(string encodedPassword, MembershipPasswordFormat format)
        {
            string password = encodedPassword;
            if (format == MembershipPasswordFormat.Clear)
                return encodedPassword;
            else if (format == MembershipPasswordFormat.Encrypted)
                return Encoding.Unicode.GetString(
                    DecryptPassword(Convert.FromBase64String(password)));
            else if (format == MembershipPasswordFormat.Hashed)
                throw new ProviderException("Cannot unencode a hashed password.");
            else
                throw new ProviderException("Unsupported password format.");
        }

        private string GetPasswordKey()
        {
            RNGCryptoServiceProvider cryptoProvider =
                new RNGCryptoServiceProvider();
            byte[] key = new byte[16];
            cryptoProvider.GetBytes(key);
            return Convert.ToBase64String(key);
        }

        private string EncodePassword(string password, string passwordKey,
            MembershipPasswordFormat format)
        {
            if (password == null)
                return null;
            if (format == MembershipPasswordFormat.Clear)
                return password;

            byte[] passwordBytes = Encoding.Unicode.GetBytes(password);
            byte[] keyBytes = Convert.FromBase64String(passwordKey);
            byte[] keyedBytes = new byte[passwordBytes.Length + keyBytes.Length];
            Array.Copy(keyBytes, keyedBytes, keyBytes.Length);
            Array.Copy(passwordBytes, 0, keyedBytes, keyBytes.Length, passwordBytes.Length);

            if (format == MembershipPasswordFormat.Encrypted)
            {
                byte[] encryptedBytes = EncryptPassword(keyedBytes);
                return Convert.ToBase64String(encryptedBytes);
            }
            else if (format == MembershipPasswordFormat.Hashed)
            {
                HashAlgorithm hash = HashAlgorithm.Create(Membership.HashAlgorithmType);
                return Convert.ToBase64String(hash.ComputeHash(keyedBytes));
            }
            else
            {
                throw new ProviderException("Unsupported password format.");
            }
        }

        private void UpdateFailureCount(string username, string failureType)
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            MySqlCommand cmd =
                new MySqlCommand(
                    @"SELECT FailedPasswordAttemptCount, 
                FailedPasswordAttemptWindowStart, FailedPasswordAnswerAttemptCount, 
                FailedPasswordAnswerAttemptWindowStart FROM mysql_Membership 
                WHERE Username = ?Username AND ApplicationName = ?ApplicationName",
                    conn);
            cmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255).Value = username;
            cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = applicationName;
            MySqlDataReader reader = null;
            DateTime windowStart = new DateTime();
            int failureCount = 0;
            try
            {
                conn.Open();
                reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                if (reader.HasRows)
                {
                    reader.Read();
                    if (failureType == "password")
                    {
                        failureCount = reader.GetInt32(0);
                        windowStart = reader.GetDateTime(1);
                    }
                    if (failureType == "passwordAnswer")
                    {
                        failureCount = reader.GetInt32(2);
                        windowStart = reader.GetDateTime(3);
                    }
                }
                reader.Close();
                DateTime windowEnd = windowStart.AddMinutes(PasswordAttemptWindow);
                if (failureCount == 0 || DateTime.Now > windowEnd)
                {
                    if (failureType == "password")
                    {
                        cmd.CommandText =
                            @"UPDATE mysql_Membership 
                            SET FailedPasswordAttemptCount = ?Count, 
                            FailedPasswordAttemptWindowStart = ?WindowStart 
                            WHERE Username = ?Username AND ApplicationName = ?ApplicationName";
                    }
                    if (failureType == "passwordAnswer")
                    {
                        cmd.CommandText =
                            @"UPDATE mysql_Membership 
                            SET FailedPasswordAnswerAttemptCount = ?Count, 
                            FailedPasswordAnswerAttemptWindowStart = ?WindowStart 
                            WHERE Username = ?Username AND ApplicationName = ?ApplicationName";
                    }
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("?Count", MySqlDbType.Int32).Value = 1;
                    cmd.Parameters.Add("?WindowStart", MySqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255).Value = username;
                    cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = applicationName;
                    if (cmd.ExecuteNonQuery() < 0)
                    {
                        throw new ProviderException("Unable to update failure count and window start.");
                    }
                }
                else
                {
                    failureCount += 1;
                    if (failureCount >= MaxInvalidPasswordAttempts)
                    {
                        cmd.CommandText =
                            @"UPDATE mysql_Membership SET IsLockedOut = ?IsLockedOut, 
                            LastLockedOutDate = ?LastLockedOutDate WHERE Username = ?Username AND 
                            ApplicationName = ?ApplicationName";
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("?IsLockedOut", MySqlDbType.Bit).Value = true;
                        cmd.Parameters.Add("?LastLockedOutDate", MySqlDbType.DateTime).Value = DateTime.Now;
                        cmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255).Value = username;
                        cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = applicationName;
                        if (cmd.ExecuteNonQuery() < 0)
                        {
                            throw new ProviderException("Unable to lock out user.");
                        }
                    }
                    else
                    {
                        if (failureType == "password")
                        {
                            cmd.CommandText =
                                @"UPDATE mysql_Membership 
                                SET FailedPasswordAttemptCount = ?Count WHERE Username = ?Username 
                                AND ApplicationName = ?ApplicationName";
                        }
                        if (failureType == "passwordAnswer")
                        {
                            cmd.CommandText =
                                @"UPDATE mysql_Membership 
                                SET FailedPasswordAnswerAttemptCount = ?Count 
                                WHERE Username = ?Username AND ApplicationName = ?ApplicationName";
                        }
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("?Count", MySqlDbType.Int32).Value = failureCount;
                        cmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255).Value = username;
                        cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = applicationName;
                        if (cmd.ExecuteNonQuery() < 0)
                        {
                            throw new ProviderException("Unable to update failure count.");
                        }
                    }
                }
            }
            catch (MySqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "UpdateFailureCount");
                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                if (!(reader == null))
                {
                    reader.Close();
                }
                conn.Close();
            }
        }

        private bool CheckPassword(string password, string dbpassword,
            string passwordKey, MembershipPasswordFormat format)
        {
            password = EncodePassword(password, passwordKey, format);
            return password == dbpassword;
        }

        private void GetPasswordInfo(MySqlConnection connection, string username,
            out string passwordKey, out MembershipPasswordFormat passwordFormat)
        {
            MySqlCommand cmd = new MySqlCommand(
                @"SELECT PasswordKey, PasswordFormat FROM mysql_Membership WHERE
                  Username = ?Username AND ApplicationName = ?ApplicationName", connection);
            cmd.Parameters.AddWithValue("?Username", username);
            cmd.Parameters.AddWithValue("?ApplicationName", applicationName);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                reader.Read();
                passwordKey = reader.GetString(reader.GetOrdinal("PasswordKey"));
                passwordFormat = (MembershipPasswordFormat)reader.GetByte(
                    reader.GetOrdinal("PasswordFormat"));
            }
        }

        private MembershipUserCollection GetUsers(string username, string email,
            int pageIndex, int pageSize, out int totalRecords)
        {
            MembershipUserCollection users = new MembershipUserCollection();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conn;

                    string sql = @"SELECT u.*,m.* FROM my_aspnet_Users u
                        JOIN my_aspnet_Membership m ON m.usrId=u.id 
                        WHERE u.applicationId=@appId";

                    if (username != null)
                    {
                        sql += " AND u.name LIKE @name";
                        cmd.Parameters.AddWithValue("@name", username);
                    }
                    else if (email != null)
                    {
                        sql += " AND m.email LIKE @email";
                        cmd.Parameters.AddWithValue("@email", email);
                    }
                    sql += "ORDER BY u.id ASC LIMIT {0},{1}";
                    cmd.CommandText = String.Format(sql, pageIndex * pageSize, pageSize);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            users.Add(GetUserFromReader(reader));
                    }
                    cmd.CommandText = "SELECT FOUND_ROWS()";
                    cmd.Parameters.Clear();
                    totalRecords = Convert.ToInt32(cmd.ExecuteScalar());
                }
                return users;
            }
            catch (MySqlException e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "GetUsers");
                throw new ProviderException(exceptionMessage);
            }
        }

        #endregion
    }
}