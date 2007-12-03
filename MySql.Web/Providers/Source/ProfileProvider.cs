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
using System.Data;
using System.IO;
using System.Globalization;

namespace MySql.Web.Security
{
    class MySQLProfileProvider : ProfileProvider
    {
        private string applicationName;
        private string connectionString;
        private int applicationId;

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

            try
            {
                applicationName = GetConfigValue(config["applicationName"], HostingEnvironment.ApplicationVirtualPath);

                ConnectionStringSettings ConnectionStringSettings = ConfigurationManager.ConnectionStrings[
                    config["connectionStringName"]];
                if (ConnectionStringSettings != null)
                    connectionString = ConnectionStringSettings.ConnectionString.Trim();
                else
                    connectionString = "";

                // make sure our schema is up to date
                SchemaManager.CheckSchema(connectionString, config);

                // now pre-cache the applicationId
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    MySqlCommand cmd = new MySqlCommand("SELECT id FROM my_aspnet_Applications WHERE name=@name", conn);
                    applicationId = (int)cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                throw new ProviderException("There was an error during provider initilization.", ex);
            }
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
                    @"SELECT * FROM my_aspnet_Users 
                    WHERE applicationId=@appId AND 
                    LastActivityDate < @lastActivityDate",
                    c);
                queryCmd.Parameters.AddWithValue("@appId", applicationId);
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
                    @"SELECT * FROM my_aspnet_Users  
                    WHERE applicationId=@appId AND name = @name", c);
                queryCmd.Parameters.AddWithValue("@appId", applicationId);
                queryCmd.Parameters.Add("@name", MySqlDbType.VarChar);

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
                    @"SELECT COUNT(*) FROM my_aspnet_Users
                    WHERE applicationId = @appId AND 
                    LastActivityDate < @lastActivityDate",
                    c);
                queryCmd.Parameters.AddWithValue("@appId", applicationId);
                queryCmd.Parameters.AddWithValue("@lastActivityDate", userInactiveSinceDate);
                if (authenticationOption == ProfileAuthenticationOption.Anonymous)
                    queryCmd.CommandText += " AND IsAnonymous = 1";
                else if (authenticationOption == ProfileAuthenticationOption.Authenticated)
                    queryCmd.CommandText += " AND IsAnonymous = 0";
                return (int)queryCmd.ExecuteScalar();
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
            SettingsPropertyValueCollection values = new SettingsPropertyValueCollection();

            if (collection.Count < 1) return values;

            string username = (string)context["UserName"];

            foreach (SettingsProperty property in collection)
            {
                values.Add(new SettingsPropertyValue(property));
            }

            if (String.IsNullOrEmpty(username))
                return values;

            // retrieve encoded profile data from the database
            try
            {
                MySqlConnection c = new MySqlConnection(connectionString);
                MySqlCommand cmd = new MySqlCommand(@"SELECT * FROM my_aspnet_Profiles p
                JOIN my_aspnet_Users u ON u.userId = p.userId
                WHERE u.applicationId = @appId AND u.name = @name", c);
                cmd.Parameters.AddWithValue("@appId", applicationId);
                cmd.Parameters.AddWithValue("@name", username);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                DecodeProfileData(dt.Rows[0], values);
                return values;
            }
            catch (Exception ex)
            {
                throw new ProviderException("Unable to retrieve profile data from database.", ex);
            }
        }

        public override void SetPropertyValues(
            SettingsContext context, SettingsPropertyValueCollection collection)
        {
            bool isAuthenticated = (bool)context["IsAuthenticated"];
            string username = (string)context["UserName"];

            if (String.IsNullOrEmpty(username)) return;
            if (collection.Count < 1) return;

            string index = String.Empty;
            string stringData = String.Empty;
            byte[] binaryData = null;
            int count = EncodeProfileData(collection, isAuthenticated, ref index, ref stringData, ref binaryData);
            if (count < 1) return;

            // save the encoded profile data to the database
            try
            {
                MySqlConnection c = new MySqlConnection(connectionString);
                MySqlCommand cmd = new MySqlCommand(@"SELECT id FROM my_aspnet_Users
                WHERE applicationId = @appId AND name = @name", c);
                cmd.Parameters.AddWithValue("@appId", applicationId);
                cmd.Parameters.AddWithValue("@name", username);
                int userId = (int)cmd.ExecuteScalar();

                cmd.CommandText = @"INSERT INTO my_aspnet_Profiles (userId, index, stringData, binaryData) 
                    VALUES (@userId, @index, @stringData, @binaryData) ON DUPLICATE KEY UPDATE";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@index", index);
                cmd.Parameters.AddWithValue("@stringData", stringData);
                cmd.Parameters.AddWithValue("@binaryData", binaryData);
                count = cmd.ExecuteNonQuery();
                if (count != 1)
                    throw new Exception("Profile update operation affected zero rows.");
            }
            catch (Exception ex)
            {
                throw new ProviderException("Unable to save profile data to database.", ex);
            }
        }

        #endregion

        #region Private Methods

        private void DecodeProfileData(DataRow profileRow, SettingsPropertyValueCollection values)
        {
            string indexData = (string)profileRow["index"];
            string stringData = (string)profileRow["stringData"];
            byte[] binaryData = (byte[])profileRow["binaryData"];

            if (indexData == null) return;

            string[] indexes = indexData.Split(':');

            foreach (string index in indexes)
            {
                string[] parts = index.Split('/');
                SettingsPropertyValue value = values[parts[0]];
                if (value == null) continue;

                int pos = Int32.Parse(parts[2], CultureInfo.InvariantCulture);
                int len = Int32.Parse(parts[3], CultureInfo.InvariantCulture);
                if (len == -1)
                {
                    value.PropertyValue = null;
                    value.IsDirty = false;
                    value.Deserialized = true;
                }
                else if (parts[1].Equals("0"))
                    value.SerializedValue = stringData.Substring(pos, len);
                else
                {
                    byte[] buf = new byte[len];
                    Buffer.BlockCopy(binaryData, pos, buf, 0, len);
                    value.SerializedValue = buf;
                }
            }
        }

        private int EncodeProfileData(SettingsPropertyValueCollection collection, bool isAuthenticated,
            ref string index, ref string stringData, ref byte[] binaryData)
        {
            bool itemsToSave = false;

            // first we need to determine if there are any items that need saving
            // this is an optimization
            foreach (SettingsPropertyValue value in collection)
            {
                if (!value.IsDirty) continue;
                if (value.Property.Attributes["AllowAnonymous"].Equals(false) &&
                    !isAuthenticated) continue;
                itemsToSave = true;
                break;
            }
            if (!itemsToSave) return 0;

            StringBuilder indexBuilder = new StringBuilder();
            StringBuilder stringDataBuilder = new StringBuilder();
            MemoryStream binaryBuilder = new MemoryStream();
            int count = 0;

            // ok, we have some values that need to be saved so we go back through
            foreach (SettingsPropertyValue value in collection)
            {
                // if the value has not been written to and is still using the default value
                // no need to save it
                if (value.UsingDefaultValue && !value.IsDirty) continue;

                // we don't save properties that require the user to be authenticated when the
                // current user is not authenticated.
                if (value.Property.Attributes["AllowAnonymous"].Equals(false) &&
                    !isAuthenticated) continue;

                count++;
                if ((value.Deserialized && value.PropertyValue == null) ||
                    value.SerializedValue == null)
                    indexBuilder.AppendFormat("{0}//0/-1:", value.Name);
                else if (value.PropertyValue is string)
                {
                    indexBuilder.AppendFormat("{0}/0/{1}/{2}:", value.Name,
                        stringDataBuilder.Length, (value.PropertyValue as string).Length);
                    stringDataBuilder.Append(value.PropertyValue);
                }
                else
                {
                    byte[] binaryValue = (byte[])value.PropertyValue;
                    indexBuilder.AppendFormat("{0}/1/{1}/{2}:", value.Name,
                        binaryBuilder.Position, binaryValue.Length);
                    binaryBuilder.Write(binaryValue, 0, binaryValue.Length);
                }
            }
            index = indexBuilder.ToString();
            stringData = stringDataBuilder.ToString();
            binaryData = binaryBuilder.ToArray();
            return count;
        }


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
                @"SELECT p.*, LENGTH(p.stringdata) + LENGTH(p.binarydata) AS profilesize, 
                u.UserName FROM my_aspnet_Profiles p 
                JOIN my_aspnet_Users u ON u.UserId = p.UserId 
                WHERE u.applicationId = @appId", c);
                cmd.Parameters.AddWithValue("@appId", applicationId);

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
                            reader.GetInt32("profilesize"));
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
