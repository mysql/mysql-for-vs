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
using System.Web.Profile;
using System.Configuration;
using System.Collections.Specialized;
using System.Web.Hosting;
using MySql.Data.MySqlClient;
using System.Configuration.Provider;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MySql.Web.Security
{
    class MySQLProfileProvider : ProfileProvider
    {
        private string applicationName;
        private string connectionString;

        #region Abstract Members

        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (name == null || name.Length == 0)
                name = "MySQLProfileProvider";

            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "MySQL Profile provider");
            }
            base.Initialize(name, config);

            applicationName = GetConfigValue(config["applicationName"], HostingEnvironment.ApplicationVirtualPath);

            ConnectionStringSettings ConnectionStringSettings = ConfigurationManager.ConnectionStrings[
                config["connectionStringName"]];
            if (ConnectionStringSettings != null)
                connectionString = ConnectionStringSettings.ConnectionString.Trim();
            else
                connectionString = "";

            // make sure our schema is up to date
            string autoGenSchema = config["AutoGenerateSchema"];
            if ((String.IsNullOrEmpty(autoGenSchema) || Convert.ToBoolean(autoGenSchema)) &&
                connectionString != String.Empty)
                ProfileSchema.CheckSchema(connectionString);
        }

        /// <summary>
        /// When overridden in a derived class, deletes all user-profile data 
        /// for profiles in which the last activity date occurred before the 
        /// specified date.
        /// </summary>
        /// <param name="authenticationOption">One of the 
        /// <see cref="T:System.Web.Profile.ProfileAuthenticationOption"/> 
        /// values, specifying whether anonymous, authenticated, or both 
        /// types of profiles are deleted.</param>
        /// <param name="userInactiveSinceDate">A <see cref="T:System.DateTime"/> 
        /// that identifies which user profiles are considered inactive. If the 
        /// <see cref="P:System.Web.Profile.ProfileInfo.LastActivityDate"/>  
        /// value of a user profile occurs on or before this date and time, the 
        /// profile is considered inactive.</param>
        /// <returns>
        /// The number of profiles deleted from the data source.
        /// </returns>
        public override int DeleteInactiveProfiles(
            ProfileAuthenticationOption authenticationOption, 
            DateTime userInactiveSinceDate)
        {
            using (MySqlConnection c = new MySqlConnection(connectionString))
            {
                c.Open();

                MySqlCommand queryCmd = new MySqlCommand(
                    @"SELECT u.UserId FROM my_aspnet_Users u
                    JOIN my_aspnet_Applications a ON a.applicationId = u.applicationId
                    WHERE a.ApplicationName = @appName AND 
                    LastActivityDate < @lastActivityDate",
                    c);
                queryCmd.Parameters.AddWithValue("@appName", applicationName);
                queryCmd.Parameters.AddWithValue("@lastActivityDate", userInactiveSinceDate);
                if (authenticationOption == ProfileAuthenticationOption.Anonymous)
                    queryCmd.CommandText += " AND IsAnonymous = 1";
                else if (authenticationOption == ProfileAuthenticationOption.Authenticated)
                    queryCmd.CommandText += " AND IsAnonymous = 0";

                MySqlCommand deleteCmd = new MySqlCommand(
                    "DELETE FROM my_aspnet_Profiles WHERE UserId = @userId", c);
                deleteCmd.Parameters.Add("@userId", MySqlDbType.UInt64);

                List<ulong> uidList = new List<ulong>();
                using (MySqlDataReader reader = queryCmd.ExecuteReader())
                {
                    while (reader.Read())
                        uidList.Add(reader.GetUInt64("UserId"));
                }

                int count = 0;
                foreach (ulong uid in uidList)
                {
                    deleteCmd.Parameters[0].Value = uid;
                    count += deleteCmd.ExecuteNonQuery();
                }
                return count;
            }
        }

        /// <summary>
        /// When overridden in a derived class, deletes profile properties 
        /// and information for profiles that match the supplied list of user names.
        /// </summary>
        /// <param name="usernames">A string array of user names for 
        /// profiles to be deleted.</param>
        /// <returns>
        /// The number of profiles deleted from the data source.
        /// </returns>
        public override int DeleteProfiles(string[] usernames)
        {
            using (MySqlConnection c = new MySqlConnection(connectionString))
            {
                c.Open();

                MySqlCommand queryCmd = new MySqlCommand(
                    @"SELECT u.UserId FROM my_aspnet_Users u JOIN
                    my_aspnet_Applications a ON a.applicationId = u.applicationId 
                    WHERE a.ApplicationName = @appName AND u.UserName = @username",
                    c);
                queryCmd.Parameters.AddWithValue("@appName", applicationName);
                queryCmd.Parameters.Add("@username", MySqlDbType.VarChar);

                MySqlCommand deleteCmd = new MySqlCommand(
                    "DELETE FROM my_aspnet_Profiles WHERE UserId = @userId", c);
                deleteCmd.Parameters.Add("@userId", MySqlDbType.UInt64);

                int count = 0;
                foreach (string name in usernames)
                {
                    queryCmd.Parameters[1].Value = name;
                    ulong uid = (ulong)queryCmd.ExecuteScalar();

                    deleteCmd.Parameters[0].Value = uid;
                    count += deleteCmd.ExecuteNonQuery();
                }
                return count;
            }
        }

        /// <summary>
        /// When overridden in a derived class, deletes profile properties 
        /// and information for the supplied list of profiles.
        /// </summary>
        /// <param name="profiles">A 
        /// <see cref="T:System.Web.Profile.ProfileInfoCollection"/>  of 
        /// information about profiles that are to be deleted.</param>
        /// <returns>
        /// The number of profiles deleted from the data source.
        /// </returns>
        public override int DeleteProfiles(ProfileInfoCollection profiles)
        {
            string[] s = new string[profiles.Count];

            int i = 0;
            foreach (ProfileInfo p in profiles)
                s[i++] = p.UserName;
            return DeleteProfiles(s);
        }

        /// <summary>
        /// When overridden in a derived class, retrieves profile information 
        /// for profiles in which the last activity date occurred on or before 
        /// the specified date and the user name matches the specified user name.
        /// </summary>
        /// <param name="authenticationOption">One of the 
        /// <see cref="T:System.Web.Profile.ProfileAuthenticationOption"/> values, 
        /// specifying whether anonymous, authenticated, or both types of profiles 
        /// are returned.</param>
        /// <param name="usernameToMatch">The user name to search for.</param>
        /// <param name="userInactiveSinceDate">A <see cref="T:System.DateTime"/> 
        /// that identifies which user profiles are considered inactive. If the 
        /// <see cref="P:System.Web.Profile.ProfileInfo.LastActivityDate"/> value 
        /// of a user profile occurs on or before this date and time, the profile 
        /// is considered inactive.</param>
        /// <param name="pageIndex">The index of the page of results to return.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">When this method returns, contains the total 
        /// number of profiles.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Profile.ProfileInfoCollection"/> containing 
        /// user profile information for inactive profiles where the user name 
        /// matches the supplied <paramref name="usernameToMatch"/> parameter.
        /// </returns>
        public override ProfileInfoCollection FindInactiveProfilesByUserName(
            ProfileAuthenticationOption authenticationOption, 
            string usernameToMatch, DateTime userInactiveSinceDate, 
            int pageIndex, int pageSize, out int totalRecords)
        {
            return GetProfiles(authenticationOption, usernameToMatch,
                userInactiveSinceDate, pageIndex, pageSize, out totalRecords);
        }

        /// <summary>
        /// When overridden in a derived class, retrieves profile information 
        /// for profiles in which the user name matches the specified user names.
        /// </summary>
        /// <param name="authenticationOption">One of the 
        /// <see cref="T:System.Web.Profile.ProfileAuthenticationOption"/> values, 
        /// specifying whether anonymous, authenticated, or both types of profiles 
        /// are returned.</param>
        /// <param name="usernameToMatch">The user name to search for.</param>
        /// <param name="pageIndex">The index of the page of results to return.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">When this method returns, contains the total 
        /// number of profiles.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Profile.ProfileInfoCollection"/> containing 
        /// user-profile information for profiles where the user name matches the 
        /// supplied <paramref name="usernameToMatch"/> parameter.
        /// </returns>
        public override ProfileInfoCollection FindProfilesByUserName(
            ProfileAuthenticationOption authenticationOption, 
            string usernameToMatch, int pageIndex, int pageSize, 
            out int totalRecords)
        {
            return GetProfiles(authenticationOption, usernameToMatch,
                DateTime.MinValue, pageIndex, pageSize, out totalRecords);
        }

        /// <summary>
        /// When overridden in a derived class, retrieves user-profile data 
        /// from the data source for profiles in which the last activity date 
        /// occurred on or before the specified date.
        /// </summary>
        /// <param name="authenticationOption">One of the 
        /// <see cref="T:System.Web.Profile.ProfileAuthenticationOption"/> values, 
        /// specifying whether anonymous, authenticated, or both types of profiles 
        /// are returned.</param>
        /// <param name="userInactiveSinceDate">A <see cref="T:System.DateTime"/> 
        /// that identifies which user profiles are considered inactive. If the 
        /// <see cref="P:System.Web.Profile.ProfileInfo.LastActivityDate"/>  of 
        /// a user profile occurs on or before this date and time, the profile is 
        /// considered inactive.</param>
        /// <param name="pageIndex">The index of the page of results to return.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">When this method returns, contains the 
        /// total number of profiles.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Profile.ProfileInfoCollection"/> containing user-profile information about the inactive profiles.
        /// </returns>
        public override ProfileInfoCollection GetAllInactiveProfiles(
            ProfileAuthenticationOption authenticationOption, 
            DateTime userInactiveSinceDate, int pageIndex, int pageSize, 
            out int totalRecords)
        {
            return GetProfiles(authenticationOption, null,
                userInactiveSinceDate, pageIndex, pageSize, out totalRecords);
        }

        /// <summary>
        /// When overridden in a derived class, retrieves user profile data for 
        /// all profiles in the data source.
        /// </summary>
        /// <param name="authenticationOption">One of the 
        /// <see cref="T:System.Web.Profile.ProfileAuthenticationOption"/> values, 
        /// specifying whether anonymous, authenticated, or both types of profiles 
        /// are returned.</param>
        /// <param name="pageIndex">The index of the page of results to return.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">When this method returns, contains the 
        /// total number of profiles.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Profile.ProfileInfoCollection"/> containing 
        /// user-profile information for all profiles in the data source.
        /// </returns>
        public override ProfileInfoCollection GetAllProfiles(
            ProfileAuthenticationOption authenticationOption, int pageIndex, 
            int pageSize, out int totalRecords)
        {
            return GetProfiles(authenticationOption, null,
                DateTime.MinValue, pageIndex, pageSize, out totalRecords);
        }

        /// <summary>
        /// When overridden in a derived class, returns the number of profiles 
        /// in which the last activity date occurred on or before the specified 
        /// date.
        /// </summary>
        /// <param name="authenticationOption">One of the 
        /// <see cref="T:System.Web.Profile.ProfileAuthenticationOption"/> values, 
        /// specifying whether anonymous, authenticated, or both types of profiles 
        /// are returned.</param>
        /// <param name="userInactiveSinceDate">A <see cref="T:System.DateTime"/> 
        /// that identifies which user profiles are considered inactive. If the 
        /// <see cref="P:System.Web.Profile.ProfileInfo.LastActivityDate"/>  of 
        /// a user profile occurs on or before this date and time, the profile 
        /// is considered inactive.</param>
        /// <returns>
        /// The number of profiles in which the last activity date occurred on 
        /// or before the specified date.
        /// </returns>
        public override int GetNumberOfInactiveProfiles(
            ProfileAuthenticationOption authenticationOption, 
            DateTime userInactiveSinceDate)
        {
            using (MySqlConnection c = new MySqlConnection(connectionString))
            {
                c.Open();

                MySqlCommand queryCmd = new MySqlCommand(
                    @"SELECT COUNT(u.*) FROM my_aspnet_Users u
                    JOIN my_aspnet_Applications a ON a.applicationId = u.applicationId
                    WHERE a.ApplicationName = @appName AND 
                    LastActivityDate < @lastActivityDate",
                    c);
                queryCmd.Parameters.AddWithValue("@appName", applicationName);
                queryCmd.Parameters.AddWithValue("@lastActivityDate", userInactiveSinceDate);
                if (authenticationOption == ProfileAuthenticationOption.Anonymous)
                    queryCmd.CommandText += " AND IsAnonymous = 1";
                else if (authenticationOption == ProfileAuthenticationOption.Authenticated)
                    queryCmd.CommandText += " AND IsAnonymous = 0";
                return queryCmd.ExecuteScalar();
            }
        }

        public override string ApplicationName
        {
            get { return applicationName; }
            set { applicationName = value; }
        }

        public override string Name
        {
            get { return "MySQLProfileProvider"; }
        }

        public override string Description
        {
            get { return "MySQL Profile provider"; }
        }

        public override SettingsPropertyValueCollection GetPropertyValues(
            SettingsContext context, SettingsPropertyCollection collection)
        {
            throw new System.Exception("The method or operation is not implemented.");
        }

        public override void SetPropertyValues(
            SettingsContext context, SettingsPropertyValueCollection collection)
        {
            throw new System.Exception("The method or operation is not implemented.");
        }

        #endregion

        #region Private Methods

        private ProfileInfoCollection GetProfiles(
            ProfileAuthenticationOption authenticationOption,
            string usernameToMatch, DateTime userInactiveSinceDate,
            int pageIndex, int pageSize, out int totalRecords)
        {
            List<string> whereClauses = new List<string>();

            using (MySqlConnection c = new MySqlConnection(connectionString))
            {
                c.Open();

                MySqlCommand cmd = new MySqlCommand(
                @"SELECT p.*, u.UserName FROM my_aspnet_Profiles p 
                JOIN my_aspnet_Users u ON u.UserId = p.UserId 
                JOIN my_aspnet_Applications a on a.ApplicationId = p.ApplicationId
                WHERE a.ApplicationId = @appName", c);
                cmd.Parameters.AddWithValue("@appName", applicationName);

                if (usernameToMatch != null)
                {
                    cmd.CommandText += " AND u.UserName LIKE @userName";
                    cmd.Parameters.AddWithValue("@userName", usernameToMatch);
                }
                if (userInactiveSinceDate != DateTime.MinValue)
                {
                    cmd.CommandText += " AND u.LastActivityDate < @lastActivityDate";
                    cmd.Parameters.AddWithValue("@lastActivityDate", userInactiveSinceDate);
                }
                if (authenticationOption == ProfileAuthenticationOption.Anonymous)
                     cmd.CommandText += " AND u.IsAnonymous = 1";
                else if (authenticationOption == ProfileAuthenticationOption.Authenticated)
                    cmd.CommandText += " AND u.IsAnonymous = 0";

                cmd.CommandText += String.Format(" LIMIT {0},{1}", pageIndex * pageSize, pageSize);

                ProfileInfoCollection pic = new ProfileInfoCollection();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ProfileInfo pi = new ProfileInfo(
                            reader.GetString("UserName"),
                            reader.GetBoolean("IsAnonymous"),
                            reader.GetDateTime("LastActivityDate"),
                            reader.GetDateTime("LastUpdatdDate"),
                            0 // TODO: fix this
                            );
                        pic.Add(pi);
                    }
                }
                cmd.CommandText = "SELECT FOUND_ROWS()";
                totalRecords = (int)cmd.ExecuteScalar();
                return pic;
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

        #endregion

    }
}
