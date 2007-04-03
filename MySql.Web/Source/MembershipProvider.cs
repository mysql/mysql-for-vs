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

using System.Web.Security;
using System.Configuration.Provider;
using System.Collections.Specialized;
using System;
using System.Data;
using System.Configuration;
using System.Diagnostics;
using System.Web;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Web.Configuration;
using MySql.Data.MySqlClient;
using MySql.Data.Types;

namespace MySql.Web.Security
{
    public sealed class MySqlMembershipProvider : MembershipProvider
    {
        private int newPasswordLength = 8;
        private string eventSource = "MySQLMembershipProvider";
        private string eventLog = "Application";
        private string exceptionMessage = "An exception occurred. Please check the Event Log.";
        private string connectionString;
        private int pMinRequiredPasswordLength;
        private MachineKeySection machineKey;
        private bool pWriteExceptionsToEventLog;
        private string pApplicationName;
        private bool pEnablePasswordReset;
        private bool pEnablePasswordRetrieval;
        private bool pRequiresQuestionAndAnswer;
        private bool pRequiresUniqueEmail;
        private int pMaxInvalidPasswordAttempts;
        private int pPasswordAttemptWindow;
        private MembershipPasswordFormat pPasswordFormat;
        private int pMinRequiredNonAlphanumericCharacters;
        private string pPasswordStrengthRegularExpression;

        public bool WriteExceptionsToEventLog
        {
            get
            {
                return pWriteExceptionsToEventLog;
            }
            set
            {
                pWriteExceptionsToEventLog = value;
            }
        }

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
            pApplicationName = GetConfigValue(config["applicationName"], System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            pMaxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(config["maxInvalidPasswordAttempts"], "5"));
            pPasswordAttemptWindow = Convert.ToInt32(GetConfigValue(config["passwordAttemptWindow"], "10"));
            pMinRequiredNonAlphanumericCharacters = Convert.ToInt32(GetConfigValue(config["minRequiredAlphaNumericCharacters"], "1"));
            pMinRequiredPasswordLength = Convert.ToInt32(GetConfigValue(config["minRequiredPasswordLength"], "7"));
            pPasswordStrengthRegularExpression = Convert.ToString(GetConfigValue(config["passwordStrengthRegularExpression"], ""));
            pEnablePasswordReset = Convert.ToBoolean(GetConfigValue(config["enablePasswordReset"], "True"));
            pEnablePasswordRetrieval = Convert.ToBoolean(GetConfigValue(config["enablePasswordRetrieval"], "True"));
            pRequiresQuestionAndAnswer = Convert.ToBoolean(GetConfigValue(config["requiresQuestionAndAnswer"], "False"));
            pRequiresUniqueEmail = Convert.ToBoolean(GetConfigValue(config["requiresUniqueEmail"], "True"));
            pWriteExceptionsToEventLog = Convert.ToBoolean(GetConfigValue(config["writeExceptionsToEventLog"], "True"));
            string temp_format = config["passwordFormat"];
            if (temp_format == null)
            {
                temp_format = "Hashed";
            }
            if (temp_format == "Hashed")
            {
                pPasswordFormat = MembershipPasswordFormat.Hashed;
            }
            else if (temp_format == "Encrypted")
            {
                pPasswordFormat = MembershipPasswordFormat.Encrypted;
            }
            else if (temp_format == "Clear")
            {
                pPasswordFormat = MembershipPasswordFormat.Clear;
            }
            else
            {
                throw new ProviderException("Password format not supported.");
            }
            ConnectionStringSettings ConnectionStringSettings = ConfigurationManager.ConnectionStrings[
                config["connectionStringName"]];
            if (ConnectionStringSettings == null || ConnectionStringSettings.ConnectionString.Trim() == "")
            {
                throw new ProviderException("Connection string cannot be blank.");
            }
            connectionString = ConnectionStringSettings.ConnectionString;
            System.Configuration.Configuration cfg = WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            machineKey = ((MachineKeySection)(cfg.GetSection("system.web/machineKey")));
            if (machineKey.ValidationKey.Contains("AutoGenerate"))
            {
                if (PasswordFormat != MembershipPasswordFormat.Clear)
                {
                    throw new ProviderException("Hashed or Encrypted passwords " + "are not supported with auto-generated keys.");
                }
            }

            // make sure our schema is up to date
            string autoGenSchema = config["AutoGenerateSchema"];
            if (String.IsNullOrEmpty(autoGenSchema) || Convert.ToBoolean(autoGenSchema))
                MembershipSchema.CheckSchema(connectionString);
        }



        private string GetConfigValue(string configValue, string defaultValue)
        {
            if (string.IsNullOrEmpty(configValue))
            {
                return defaultValue;
            }
            return configValue;
        }

        #region Properties

        public override string ApplicationName
        {
            get
            {
                return pApplicationName;
            }
            set
            {
                pApplicationName = value;
            }
        }

        public override bool EnablePasswordReset
        {
            get
            {
                return pEnablePasswordReset;
            }
        }

        public override bool EnablePasswordRetrieval
        {
            get
            {
                return pEnablePasswordRetrieval;
            }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get
            {
                return pRequiresQuestionAndAnswer;
            }
        }

        public override bool RequiresUniqueEmail
        {
            get
            {
                return pRequiresUniqueEmail;
            }
        }

        public override int MaxInvalidPasswordAttempts
        {
            get
            {
                return pMaxInvalidPasswordAttempts;
            }
        }

        public override int PasswordAttemptWindow
        {
            get
            {
                return pPasswordAttemptWindow;
            }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get
            {
                return pPasswordFormat;
            }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get
            {
                return pMinRequiredNonAlphanumericCharacters;
            }
        }

        public override int MinRequiredPasswordLength
        {
            get
            {
                return pMinRequiredPasswordLength;
            }
        }

        public override string PasswordStrengthRegularExpression
        {
            get
            {
                return pPasswordStrengthRegularExpression;
            }
        }

        #endregion

        public override bool ChangePassword(string username, string oldPwd, string newPwd)
        {
            if (!(ValidateUser(username, oldPwd)))
            {
                return false;
            }
            ValidatePasswordEventArgs args = new ValidatePasswordEventArgs(username, newPwd, true);
            OnValidatingPassword(args);
            if (args.Cancel)
            {
                if (!(args.FailureInformation == null))
                {
                    throw args.FailureInformation;
                }
                else
                {
                    throw new ProviderException("Change password canceled due to New password validation failure.");
                }
            }
            MySqlConnection conn = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand("UPDATE mysql_Membership " + " SET Password = ?Password, LastPasswordChangedDate = ?LastPasswordChangedDate " + " WHERE Username = ?Username AND ApplicationName = ?ApplicationName", conn);
            cmd.Parameters.Add("?Password", MySqlDbType.VarChar, 255).Value = EncodePassword(newPwd);
            cmd.Parameters.Add("?LastPasswordChangedDate", MySqlDbType.Datetime).Value = DateTime.Now;
            cmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255).Value = username;
            cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;
            int rowsAffected = 0;
            try
            {
                conn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (MySqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "ChangePassword");
                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                conn.Close();
            }
            if (rowsAffected > 0)
            {
                return true;
            }
            return false;
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPwdQuestion, string newPwdAnswer)
        {
            if (!(ValidateUser(username, password)))
            {
                return false;
            }
            MySqlConnection conn = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand("UPDATE mysql_Membership " + " SET PasswordQuestion = ?PasswordQuestion, PasswordAnswer = ?PasswordAnswer" + " WHERE Username = ?Username AND ApplicationName = ?ApplicationName", conn);
            cmd.Parameters.Add("?PasswordQuestion", MySqlDbType.VarChar, 255).Value = newPwdQuestion;
            cmd.Parameters.Add("?PasswordAnswer", MySqlDbType.VarChar, 255).Value = EncodePassword(newPwdAnswer);
            cmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255).Value = username;
            cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;
            int rowsAffected = 0;
            try
            {
                conn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (MySqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "ChangePasswordQuestionAndAnswer");
                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                conn.Close();
            }
            if (rowsAffected > 0)
            {
                return true;
            }
            return false;
        }

        public override MembershipUser CreateUser(string username, string password, string email, 
            string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, 
            out MembershipCreateStatus status)
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
            MembershipUser u = GetUser(username, false);
            if (u == null)
            {
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
                MySqlCommand cmd = new MySqlCommand("INSERT INTO mysql_Membership " + " (PKID, Username, Password, Email, PasswordQuestion, " + " PasswordAnswer, IsApproved," + " Comment, CreationDate, LastPasswordChangedDate, LastActivityDate," + " ApplicationName, IsLockedOut, LastLockedOutDate," + " FailedPasswordAttemptCount, FailedPasswordAttemptWindowStart, " + " FailedPasswordAnswerAttemptCount, FailedPasswordAnswerAttemptWindowStart)" + " Values(?PKID, ?Username, ?Password, ?Email, ?PasswordQuestion, ?PasswordAnswer," + " ?IsApproved, ?Comment, ?CreationDate, ?LastPasswordChangedDate, ?LastActivityDate," + " ?ApplicationName, ?IsLockedOut, ?LastLockedOutDate, ?FailedPasswordAttemptCount," + " ?FailedPasswordAttemptWindowStart, ?FailedPasswordAnswerAttemptCount, ?FailedPasswordAnswerAttemptWindowStart)", conn);
                cmd.Parameters.Add("?PKID", MySqlDbType.VarChar).Value = providerUserKey.ToString();
                cmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255).Value = username;
                cmd.Parameters.Add("?Password", MySqlDbType.VarChar, 255).Value = EncodePassword(password);
                cmd.Parameters.Add("?Email", MySqlDbType.VarChar, 128).Value = email;
                cmd.Parameters.Add("?PasswordQuestion", MySqlDbType.VarChar, 255).Value = passwordQuestion;
                cmd.Parameters.Add("?PasswordAnswer", MySqlDbType.VarChar, 255).Value = EncodePassword(passwordAnswer);
                cmd.Parameters.Add("?IsApproved", MySqlDbType.Bit).Value = isApproved;
                cmd.Parameters.Add("?Comment", MySqlDbType.VarChar, 255).Value = "";
                cmd.Parameters.Add("?CreationDate", MySqlDbType.Datetime).Value = createDate;
                cmd.Parameters.Add("?LastPasswordChangedDate", MySqlDbType.Datetime).Value = createDate;
                cmd.Parameters.Add("?LastActivityDate", MySqlDbType.Datetime).Value = createDate;
                cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;
                cmd.Parameters.Add("?IsLockedOut", MySqlDbType.Bit).Value = false;
                cmd.Parameters.Add("?LastLockedOutDate", MySqlDbType.Datetime).Value = createDate;
                cmd.Parameters.Add("?FailedPasswordAttemptCount", MySqlDbType.Int32).Value = 0;
                cmd.Parameters.Add("?FailedPasswordAttemptWindowStart", MySqlDbType.Datetime).Value = createDate;
                cmd.Parameters.Add("?FailedPasswordAnswerAttemptCount", MySqlDbType.Int32).Value = 0;
                cmd.Parameters.Add("?FailedPasswordAnswerAttemptWindowStart", MySqlDbType.Datetime).Value = createDate;
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
            else
            {
                status = MembershipCreateStatus.DuplicateUserName;
            }
            return null;
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand("DELETE FROM mysql_Membership " + " WHERE Username = ?Username AND Applicationname = ?Applicationname", conn);
            cmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255).Value = username;
            cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName.ToString();
            int rowsAffected = 0;
            try
            {
                conn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
/*                if (deleteAllRelatedData)
                {
                    cmd = new MySqlCommand("DELETE FROM mysql_MembershipRoles WHERE Username = ?Username", conn);
                    cmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255).Value = username;
                    rowsAffected = cmd.ExecuteNonQuery();
                }*/
            }
            catch (MySqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "DeleteUser");
                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                conn.Close();
            }
            if (rowsAffected > 0)
            {
                return true;
            }
            return false;
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand("SELECT Count(*) FROM mysql_Membership " + "WHERE ApplicationName = ?ApplicationName", conn);
            cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;
            MembershipUserCollection users = new MembershipUserCollection();
            MySqlDataReader reader = null;
            totalRecords = 0;
            try
            {
                conn.Open();
                totalRecords = System.Convert.ToInt32(cmd.ExecuteScalar());
                if (totalRecords <= 0)
                {
                    return users;
                }
                cmd.CommandText = "SELECT PKID, Username, Email, PasswordQuestion," + " Comment, IsApproved, IsLockedOut, CreationDate, LastLoginDate," + " LastActivityDate, LastPasswordChangedDate, LastLockedOutDate " + " FROM mysql_Membership " + " WHERE ApplicationName = ?ApplicationName " + " ORDER BY Username Asc";
                cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName.ToString();
                reader = cmd.ExecuteReader();
                int counter = 0;
                int startIndex = pageSize * pageIndex;
                int endIndex = startIndex + pageSize - 1;
                while (reader.Read())
                {
                    if (counter >= startIndex)
                    {
                        MembershipUser u = GetUserFromReader(reader);
                        users.Add(u);
                    }
                    if (counter >= endIndex)
                    {
                        cmd.Cancel();
                    }
                    counter += 1;
                }
            }
            catch (MySqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetAllUsers");
                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
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
            return users;
        }

        public override int GetNumberOfUsersOnline()
        {
            TimeSpan onlineSpan = new TimeSpan(0, System.Web.Security.Membership.UserIsOnlineTimeWindow, 0);
            DateTime compareTime = DateTime.Now.Subtract(onlineSpan);
            MySqlConnection conn = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand("SELECT Count(*) FROM mysql_Membership " + " WHERE LastActivityDate > ?LastActivityUpdate AND ApplicationName = ?ApplicationName", conn);
            cmd.Parameters.Add("?CompareDate", MySqlDbType.Datetime).Value = compareTime;
            cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;
            int numOnline = 0;
            try
            {
                conn.Open();
                numOnline = System.Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (MySqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetNumberOfUsersOnline");
                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                conn.Close();
            }
            return numOnline;
        }

        public override string GetPassword(string username, string answer)
        {
            if (!(EnablePasswordRetrieval))
            {
                throw new ProviderException("Password Retrieval Not Enabled.");
            }
            if (PasswordFormat == MembershipPasswordFormat.Hashed)
            {
                throw new ProviderException("Cannot retrieve Hashed passwords.");
            }
            MySqlConnection conn = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand("SELECT Password, PasswordAnswer, IsLockedOut FROM mysql_Membership " + " WHERE Username = ?Username AND ApplicationName = ?ApplicationName", conn);
            cmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255).Value = username;
            cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;
            string password = "";
            string passwordAnswer = "";
            MySqlDataReader reader = null;
            try
            {
                conn.Open();
                reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                if (reader.HasRows)
                {
                    reader.Read();
                    if (reader.GetBoolean(2))
                    {
                        throw new MembershipPasswordException("The supplied user is locked out.");
                    }
                    password = reader.GetString(0);
                    passwordAnswer = reader.GetString(1);
                }
                else
                {
                    throw new MembershipPasswordException("The supplied user name is not found.");
                }
            }
            catch (MySqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetPassword");
                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
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
            if (RequiresQuestionAndAnswer && !(CheckPassword(answer, passwordAnswer)))
            {
                UpdateFailureCount(username, "passwordAnswer");
                throw new MembershipPasswordException("Incorrect password answer.");
            }
            if (PasswordFormat == MembershipPasswordFormat.Encrypted)
            {
                password = UnEncodePassword(password);
            }
            return password;
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand(@"SELECT PKID, Username, Email, PasswordQuestion,
                Comment, IsApproved, IsLockedOut, CreationDate, LastLoginDate,
                LastActivityDate, LastPasswordChangedDate, LastLockedOutDate
                FROM mysql_Membership WHERE Username = ?Username AND 
                ApplicationName = ?ApplicationName", conn);
            cmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255).Value = username;
            cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;
            MembershipUser u = null;
            MySqlDataReader reader = null;
            try
            {
                conn.Open();
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    u = GetUserFromReader(reader);
                    if (userIsOnline)
                    {
                        MySqlCommand updateCmd = new MySqlCommand(
                            @"UPDATE mysql_Membership SET LastActivityDate = ?LastActivityDate 
                            WHERE Username = ?Username AND Applicationname = ?Applicationname", conn);
                        updateCmd.Parameters.Add("?LastActivityDate", MySqlDbType.Datetime).Value = DateTime.Now;
                        updateCmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255).Value = username;
                        updateCmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;
                        updateCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (MySqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetUser(String, Boolean)");
                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
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
            return u;
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand(@"SELECT PKID, Username, Email, PasswordQuestion,
                Comment, IsApproved, IsLockedOut, CreationDate, LastLoginDate,
                LastActivityDate, LastPasswordChangedDate, LastLockedOutDate
                FROM mysql_Membership WHERE PKID = ?PKID", conn);
            cmd.Parameters.Add("?PKID", MySqlDbType.VarChar).Value = providerUserKey.ToString();
            MembershipUser u = null;
            MySqlDataReader reader = null;
            try
            {
                conn.Open();
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    u = GetUserFromReader(reader);
                    if (userIsOnline)
                    {
                        MySqlCommand updateCmd = new MySqlCommand(
                            @"UPDATE mysql_Membership SET LastActivityDate = ?LastActivityDate
                            WHERE PKID = ?PKID", conn);
                        updateCmd.Parameters.Add("?LastActivityDate", MySqlDbType.Datetime).Value = DateTime.Now;
                        updateCmd.Parameters.Add("?PKID", MySqlDbType.VarChar).Value = providerUserKey.ToString();
                        updateCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (MySqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetUser(Object, Boolean)");
                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
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
            return u;
        }

        private MembershipUser GetUserFromReader(MySqlDataReader reader)
        {
            object providerUserKey = reader.GetValue(0);
            string username = reader.GetString(1);
            string email = reader.GetString(2);
            string passwordQuestion = "";
            if (!(reader.GetValue(3) == DBNull.Value))
            {
                passwordQuestion = reader.GetString(3);
            }
            string comment = "";
            if (!(reader.GetValue(4) == DBNull.Value))
            {
                comment = reader.GetString(4);
            }
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
            MembershipUser u = new MembershipUser(this.Name, username, providerUserKey, email, passwordQuestion, comment, isApproved, isLockedOut, creationDate, lastLoginDate, lastActivityDate, lastPasswordChangedDate, lastLockedOutDate);
            return u;
        }

        public override bool UnlockUser(string username)
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand(@"UPDATE mysql_Membership SET IsLockedOut = False, 
                LastLockedOutDate = ?LastLockedOutDate WHERE Username = ?Username AND 
                ApplicationName = ?ApplicationName", conn);
            cmd.Parameters.Add("?LastLockedOutDate", MySqlDbType.Datetime).Value = DateTime.Now;
            cmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255).Value = username;
            cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;
            int rowsAffected = 0;
            try
            {
                conn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (MySqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "UnlockUser");
                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                conn.Close();
            }
            if (rowsAffected > 0)
            {
                return true;
            }
            return false;
        }

        public override string GetUserNameByEmail(string email)
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand(@"SELECT Username FROM mysql_Membership 
                WHERE Email = ?Email AND ApplicationName = ?ApplicationName", conn);
            cmd.Parameters.Add("?Email", MySqlDbType.VarChar, 128).Value = email;
            cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;
            string username = "";
            try
            {
                conn.Open();
                object name = cmd.ExecuteScalar();
                if (name != null)
                    username = name.ToString();
            }
            catch (MySqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetUserNameByEmail");
                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                conn.Close();
            }
            if (username == null)
            {
                username = "";
            }
            return username;
        }

        public override string ResetPassword(string username, string answer)
        {
            if (!(EnablePasswordReset))
            {
                throw new NotSupportedException("Password Reset is not enabled.");
            }
            if (answer == null && RequiresQuestionAndAnswer)
            {
                UpdateFailureCount(username, "passwordAnswer");
                throw new ProviderException("Password answer required for password Reset.");
            }
            string newPassword = System.Web.Security.Membership.GeneratePassword(newPasswordLength, MinRequiredNonAlphanumericCharacters);
            ValidatePasswordEventArgs Args = new ValidatePasswordEventArgs(username, newPassword, true);
            OnValidatingPassword(Args);
            if (Args.Cancel)
            {
                if (!(Args.FailureInformation == null))
                {
                    throw Args.FailureInformation;
                }
                else
                {
                    throw new MembershipPasswordException("Reset password canceled due to password validation failure.");
                }
            }
            MySqlConnection conn = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand(@"SELECT PasswordAnswer, IsLockedOut FROM mysql_Membership 
                WHERE Username = ?Username AND ApplicationName = ?ApplicationName", conn);
            cmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255).Value = username;
            cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;
            int rowsAffected = 0;
            string passwordAnswer = "";
            MySqlDataReader reader = null;
            try
            {
                conn.Open();
                reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                if (reader.HasRows)
                {
                    reader.Read();
                    if (reader.GetBoolean(1))
                    {
                        throw new MembershipPasswordException("The supplied user is locked out.");
                    }
                    passwordAnswer = reader.GetString(0);
                }
                else
                {
                    throw new MembershipPasswordException("The supplied user name is not found.");
                }
                if (RequiresQuestionAndAnswer && !(CheckPassword(answer, passwordAnswer)))
                {
                    UpdateFailureCount(username, "passwordAnswer");
                    throw new MembershipPasswordException("Incorrect password answer.");
                }
                MySqlCommand updateCmd = new MySqlCommand(@"UPDATE mysql_Membership 
                    SET Password = ?Password, LastPasswordChangedDate = ?LastPasswordChangedDate
                    WHERE Username = ?Username AND ApplicationName = ?ApplicationName AND 
                    IsLockedOut = False", conn);
                updateCmd.Parameters.Add("?Password", MySqlDbType.VarChar, 255).Value = EncodePassword(newPassword);
                updateCmd.Parameters.Add("?LastPasswordChangedDate", MySqlDbType.Datetime).Value = DateTime.Now;
                updateCmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255).Value = username;
                updateCmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;
                rowsAffected = updateCmd.ExecuteNonQuery();
            }
            catch (MySqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "ResetPassword");
                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
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
            if (rowsAffected > 0)
            {
                return newPassword;
            }
            else
            {
                throw new MembershipPasswordException("User not found, or user is locked out. Password not Reset.");
            }
        }

        public override void UpdateUser(MembershipUser user)
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand(@"UPDATE mysql_Membership SET Email = ?Email, 
                Comment = ?Comment, IsApproved = ?IsApproved WHERE Username = ?Username AND 
                ApplicationName = ?ApplicationName", conn);
            cmd.Parameters.Add("?Email", MySqlDbType.VarChar, 128).Value = user.Email;
            cmd.Parameters.Add("?Comment", MySqlDbType.VarChar, 255).Value = user.Comment;
            cmd.Parameters.Add("?IsApproved", MySqlDbType.Bit).Value = user.IsApproved;
            cmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255).Value = user.UserName;
            cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;
            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "UpdateUser");
                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                conn.Close();
            }
        }

        public override bool ValidateUser(string username, string password)
        {
            bool isValid = false;
            MySqlConnection conn = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand(@"SELECT Password, IsApproved FROM mysql_Membership 
                WHERE Username = ?Username AND ApplicationName = ?ApplicationName AND 
                IsLockedOut = False", conn);
            cmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255).Value = username;
            cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;
            MySqlDataReader reader = null;
            bool isApproved = false;
            string pwd = "";
            try
            {
                conn.Open();
                reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                if (reader.HasRows)
                {
                    reader.Read();
                    pwd = reader.GetString(0);
                    isApproved = reader.GetBoolean(1);
                }
                else
                {
                    return false;
                }
                reader.Close();
                if (CheckPassword(password, pwd))
                {
                    if (isApproved)
                    {
                        isValid = true;
                        MySqlCommand updateCmd = new MySqlCommand(@"UPDATE mysql_Membership 
                            SET LastLoginDate = ?LastLoginDate WHERE Username = ?Username AND 
                            ApplicationName = ?ApplicationName", conn);
                        updateCmd.Parameters.Add("?LastLoginDate", MySqlDbType.Datetime).Value = DateTime.Now;
                        updateCmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255).Value = username;
                        updateCmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;
                        updateCmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    conn.Close();
                    UpdateFailureCount(username, "password");
                }
            }
            catch (MySqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "ValidateUser");
                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
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
            return isValid;
        }

        private void UpdateFailureCount(string username, string failureType)
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand(@"SELECT FailedPasswordAttemptCount, 
                FailedPasswordAttemptWindowStart, FailedPasswordAnswerAttemptCount, 
                FailedPasswordAnswerAttemptWindowStart FROM mysql_Membership 
                WHERE Username = ?Username AND ApplicationName = ?ApplicationName", conn);
            cmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255).Value = username;
            cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;
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
                        cmd.CommandText = @"UPDATE mysql_Membership 
                            SET FailedPasswordAttemptCount = ?Count, 
                            FailedPasswordAttemptWindowStart = ?WindowStart 
                            WHERE Username = ?Username AND ApplicationName = ?ApplicationName";
                    }
                    if (failureType == "passwordAnswer")
                    {
                        cmd.CommandText = @"UPDATE mysql_Membership 
                            SET FailedPasswordAnswerAttemptCount = ?Count, 
                            FailedPasswordAnswerAttemptWindowStart = ?WindowStart 
                            WHERE Username = ?Username AND ApplicationName = ?ApplicationName";
                    }
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("?Count", MySqlDbType.Int32).Value = 1;
                    cmd.Parameters.Add("?WindowStart", MySqlDbType.Datetime).Value = DateTime.Now;
                    cmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255).Value = username;
                    cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;
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
                        cmd.CommandText = @"UPDATE mysql_Membership SET IsLockedOut = ?IsLockedOut, 
                            LastLockedOutDate = ?LastLockedOutDate WHERE Username = ?Username AND 
                            ApplicationName = ?ApplicationName";
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("?IsLockedOut", MySqlDbType.Bit).Value = true;
                        cmd.Parameters.Add("?LastLockedOutDate", MySqlDbType.Datetime).Value = DateTime.Now;
                        cmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255).Value = username;
                        cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;
                        if (cmd.ExecuteNonQuery() < 0)
                        {
                            throw new ProviderException("Unable to lock out user.");
                        }
                    }
                    else
                    {
                        if (failureType == "password")
                        {
                            cmd.CommandText = @"UPDATE mysql_Membership 
                                SET FailedPasswordAttemptCount = ?Count WHERE Username = ?Username 
                                AND ApplicationName = ?ApplicationName";
                        }
                        if (failureType == "passwordAnswer")
                        {
                            cmd.CommandText = @"UPDATE mysql_Membership 
                                SET FailedPasswordAnswerAttemptCount = ?Count 
                                WHERE Username = ?Username AND ApplicationName = ?ApplicationName";
                        }
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("?Count", MySqlDbType.Int32).Value = failureCount;
                        cmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255).Value = username;
                        cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;
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
                    throw e;
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

        private bool CheckPassword(string password, string dbpassword)
        {
            string pass1 = password;
            string pass2 = dbpassword;
            if (PasswordFormat == MembershipPasswordFormat.Encrypted)
            {
                pass2 = UnEncodePassword(dbpassword);
            }
            else if (PasswordFormat == MembershipPasswordFormat.Hashed)
            {
                pass1 = EncodePassword(password);
            }
            else
            {
            }
            if (pass1 == pass2)
            {
                return true;
            }
            return false;
        }

        private string EncodePassword(string password)
        {
            string encodedPassword = password;
            if (PasswordFormat == MembershipPasswordFormat.Clear)
            {
            }
            else if (PasswordFormat == MembershipPasswordFormat.Encrypted)
            {
                encodedPassword = Convert.ToBase64String(EncryptPassword(Encoding.Unicode.GetBytes(password)));
            }
            else if (PasswordFormat == MembershipPasswordFormat.Hashed)
            {
                HMACSHA1 hash = new HMACSHA1();
                hash.Key = HexToByte(machineKey.ValidationKey);
                encodedPassword = Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(password)));
            }
            else
            {
                throw new ProviderException("Unsupported password format.");
            }
            return encodedPassword;
        }

        private string UnEncodePassword(string encodedPassword)
        {
            string password = encodedPassword;
            if (PasswordFormat == MembershipPasswordFormat.Clear)
            {
            }
            else if (PasswordFormat == MembershipPasswordFormat.Encrypted)
            {
                password = Encoding.Unicode.GetString(DecryptPassword(Convert.FromBase64String(password)));
            }
            else if (PasswordFormat == MembershipPasswordFormat.Hashed)
            {
                throw new ProviderException("Cannot unencode a hashed password.");
            }
            else
            {
                throw new ProviderException("Unsupported password format.");
            }
            return password;
        }

        private byte[] HexToByte(string hexString)
        {
            byte[] ReturnBytes = new byte[(hexString.Length / 2) - 1];
            for (int i = 0; i <= ReturnBytes.Length - 1; i++)
            {
                ReturnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return ReturnBytes;
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, 
            int pageIndex, int pageSize, out int totalRecords)
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand(@"SELECT Count(*) FROM mysql_Membership 
                WHERE Username LIKE ?UsernameSearch AND ApplicationName = ?ApplicationName", conn);
            cmd.Parameters.Add("?UsernameSearch", MySqlDbType.VarChar, 255).Value = usernameToMatch;
            cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;
            MembershipUserCollection users = new MembershipUserCollection();
            MySqlDataReader reader = null;
            try
            {
                conn.Open();
                totalRecords = System.Convert.ToInt32(cmd.ExecuteScalar());
                if (totalRecords <= 0)
                {
                    return users;
                }
                cmd.CommandText = @"SELECT PKID, Username, Email, PasswordQuestion, Comment, 
                    IsApproved, IsLockedOut, CreationDate, LastLoginDate, LastActivityDate, 
                    LastPasswordChangedDate, LastLockedOutDate FROM mysql_Membership 
                    WHERE Username LIKE ?UsernameSearch AND ApplicationName = ?ApplicationName 
                    ORDER BY Username Asc";
                cmd.Parameters.Add("?UsernameSearch", MySqlDbType.VarChar, 255).Value = usernameToMatch;
                cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;
                reader = cmd.ExecuteReader();
                int counter = 0;
                int startIndex = pageSize * pageIndex;
                int endIndex = startIndex + pageSize - 1;
                while (reader.Read())
                {
                    if (counter >= startIndex)
                    {
                        MembershipUser u = GetUserFromReader(reader);
                        users.Add(u);
                    }
                    if (counter >= endIndex)
                    {
                        cmd.Cancel();
                    }
                    counter += 1;
                }
            }
            catch (MySqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "FindUsersByName");
                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
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
            return users;
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, 
            int pageSize, out int totalRecords)
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand(@"SELECT Count(*) FROM mysql_Membership 
                WHERE Email LIKE ?EmailSearch AND ApplicationName = ?ApplicationName", conn);
            cmd.Parameters.Add("?EmailSearch", MySqlDbType.VarChar, 255).Value = emailToMatch;
            cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName.ToString();
            MembershipUserCollection users = new MembershipUserCollection();
            MySqlDataReader reader = null;
            totalRecords = 0;
            try
            {
                conn.Open();
                totalRecords = System.Convert.ToInt32(cmd.ExecuteScalar());
                if (totalRecords <= 0)
                {
                    return users;
                }
                cmd.CommandText = @"SELECT PKID, Username, Email, PasswordQuestion, Comment, 
                    IsApproved, IsLockedOut, CreationDate, LastLoginDate, 
                    LastActivityDate, LastPasswordChangedDate, LastLockedOutDate 
                    FROM mysql_Membership WHERE Email LIKE ?EmailSearch AND 
                    ApplicationName = ?ApplicationName " + " ORDER BY Username Asc";
                cmd.Parameters.Add("?EmailSearch", MySqlDbType.VarChar, 255).Value = emailToMatch;
                cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;
                reader = cmd.ExecuteReader();
                int counter = 0;
                int startIndex = pageSize * pageIndex;
                int endIndex = startIndex + pageSize - 1;
                while (reader.Read())
                {
                    if (counter >= startIndex)
                    {
                        MembershipUser u = GetUserFromReader(reader);
                        users.Add(u);
                    }
                    if (counter >= endIndex)
                    {
                        cmd.Cancel();
                    }
                    counter += 1;
                }
            }
            catch (MySqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "FindUsersByEmail");
                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
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
            return users;
        }

        private void WriteToEventLog(Exception e, string action)
        {
            EventLog log = new EventLog();
            log.Source = eventSource;
            log.Log = eventLog;
            string message = "An exception occurred communicating with the data source." +
                Environment.NewLine + Environment.NewLine;
            message += "Action: " + action + Environment.NewLine + Environment.NewLine;
            message += "Exception: " + e.ToString();
            log.WriteEntry(message);
        }
    }
}