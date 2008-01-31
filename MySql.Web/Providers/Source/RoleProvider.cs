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
using System.Diagnostics;
using System.Web.Hosting;
using System.Web.Security;
using MySql.Data.MySqlClient;
using System.Transactions;
using System.Collections.Generic;
using MySql.Web.Common;

namespace MySql.Web.Security
{
    /// <summary>
    /// Manages storage of role membership information for an ASP.NET application in a MySQL database. 
    /// </summary>
    public sealed class MySQLRoleProvider : RoleProvider
    {
        private string eventSource = "MySQLRoleProvider";
        private string eventLog = "Application";
        private string exceptionMessage = "An exception occurred. Please check the Event Log.";
        private ConnectionStringSettings pConnectionStringSettings;
        private string connectionString;
        private bool pWriteExceptionsToEventLog = false;
        private string applicationName;
        private int applicationId;

        /// <summary>
        /// Initializes the provider.
        /// </summary>
        /// <param name="name">The friendly name of the provider.</param>
        /// <param name="config">A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider.</param>
        /// <exception cref="T:System.ArgumentNullException">The name of the provider is null.</exception>
        /// <exception cref="T:System.ArgumentException">The name of the provider has a length of zero.</exception>
        /// <exception cref="T:System.InvalidOperationException">An attempt is made to call <see cref="M:System.Configuration.Provider.ProviderBase.Initialize(System.String,System.Collections.Specialized.NameValueCollection)"/> on a provider after the provider has already been initialized.</exception>
        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }
            if (name == null || name.Length == 0)
            {
                name = "MySQLRoleProvider";
            }
            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "MySQL Role provider");
            }
            base.Initialize(name, config);

            if (config["applicationName"] == null || config["applicationName"].Trim() == "")
                applicationName = HostingEnvironment.ApplicationVirtualPath;
            else
                applicationName = config["applicationName"];

            if (!(config["writeExceptionsToEventLog"] == null))
            {
                if (config["writeExceptionsToEventLog"].ToUpper() == "TRUE")
                {
                    pWriteExceptionsToEventLog = true;
                }
            }
            pConnectionStringSettings = ConfigurationManager.ConnectionStrings[config["connectionStringName"]];
            if (pConnectionStringSettings != null)
                connectionString = pConnectionStringSettings.ConnectionString.Trim();
            else
                connectionString = "";

            // make sure our schema is up to date
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

        #region Properties

        /// <summary>
        /// Gets or sets the name of the application to store and retrieve role information for.
        /// </summary>
        /// <value>The name of the application to store and retrieve role information for.</value>
        /// <example>
        /// <code lang="" source="CodeExamples\RoleCodeExample1.xml"/>
        /// </example>
        public override string ApplicationName
        {
            get { return applicationName; }
            set { applicationName = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [write exceptions to event log].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if exceptions should be written to the event log; otherwise, <c>false</c>.
        /// </value>
        /// <example>
        /// <code lang="" source="CodeExamples\RoleCodeExample1.xml"/>
        /// </example>
        public bool WriteExceptionsToEventLog
        {
            get { return pWriteExceptionsToEventLog; }
            set { pWriteExceptionsToEventLog = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds the users to roles.
        /// </summary>
        /// <param name="usernames">The usernames.</param>
        /// <param name="rolenames">The rolenames.</param>
        public override void AddUsersToRoles(string[] usernames, string[] rolenames)
        {
            foreach (string rolename in rolenames)
            {
                if (!(RoleExists(rolename)))
                    throw new ProviderException("Role name not found.");
            }

            foreach (string username in usernames)
            {
                if (username.IndexOf(',') != -1)
                    throw new ArgumentException("User names cannot contain commas.");

                foreach (string rolename in rolenames)
                {
                    if (IsUserInRole(username, rolename))
                        throw new ProviderException("User is already in role.");
                }
            }

            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        MySqlCommand cmd = new MySqlCommand(
                            "INSERT INTO my_aspnet_UsersInRoles VALUES(@userId, @roleId)", conn);
                        cmd.Parameters.Add("@userId", MySqlDbType.Int32);
                        cmd.Parameters.Add("@roleId", MySqlDbType.Int32);
                        foreach (string username in usernames)
                        {
                            int userId = GetUserId(conn, username);
                            foreach (string rolename in rolenames)
                            {
                                int roleId = GetRoleId(conn, rolename);
                                cmd.Parameters[0].Value = userId;
                                cmd.Parameters[1].Value = roleId;
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(ex, "AddUsersToRoles");
                throw;
            }
        }

        /// <summary>
        /// Creates the role.
        /// </summary>
        /// <param name="rolename">The rolename.</param>
        public override void CreateRole(string rolename)
        {
            if (rolename.IndexOf(',') != -1)
                throw new ArgumentException("Role names cannot contain commas.");
            if (RoleExists(rolename))
                throw new ProviderException("Role name already exists.");

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand cmd =
                    new MySqlCommand(
                        @"INSERT INTO my_aspnet_Roles Values(NULL, @name)", conn);
                cmd.Parameters.AddWithValue("@name", rolename);
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (MySqlException e)
                {
                    if (WriteExceptionsToEventLog)
                        WriteToEventLog(e, "CreateRole");
                    throw;
                }
            }
        }

        /// <summary>
        /// Deletes the role.
        /// </summary>
        /// <param name="rolename">The rolename.</param>
        /// <param name="throwOnPopulatedRole">if set to <c>true</c> [throw on populated role].</param>
        /// <returns>true if the role was successfully deleted; otherwise, false. </returns>
        public override bool DeleteRole(string rolename, bool throwOnPopulatedRole)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        if (!(RoleExists(rolename)))
                            throw new ProviderException("Role does not exist.");
                        if (throwOnPopulatedRole && GetUsersInRole(rolename).Length > 0)
                            throw new ProviderException("Cannot delete a populated role.");

                        // first delete all the user/role mappings with that roleid
                        MySqlCommand cmd = new MySqlCommand(
                            @"DELETE uir FROM my_aspnet_UsersInRoles uir JOIN 
                            my_aspnet_Roles r ON uir.roleId=r.id 
                            WHERE r.name LIKE @rolename AND r.applicationId=@appId", conn);
                        cmd.Parameters.AddWithValue("@rolename", rolename);
                        cmd.Parameters.AddWithValue("@appId", applicationId);
                        cmd.ExecuteNonQuery();

                        // now delete the role itself
                        cmd.CommandText = @"DELETE FROM my_aspnet_Roles WHERE name=@rolename 
                            AND applicationId=@appId";
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(ex, "DeleteRole");
                throw;
            }
            return true;
        }

        /// <summary>
        /// Gets a list of all the roles for the configured applicationName.
        /// </summary>
        /// <returns>
        /// A string array containing the names of all the roles stored in the data source for the configured applicationName.
        /// </returns>
        public override string[] GetAllRoles()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                return GetRolesByUserName(conn, null);
            }
        }

        /// <summary>
        /// Gets a list of the roles that a specified user is in for the configured applicationName.
        /// </summary>
        /// <param name="username">The user to return a list of roles for.</param>
        /// <returns>
        /// A string array containing the names of all the roles that the specified user is in for the configured applicationName.
        /// </returns>
        public override string[] GetRolesForUser(string username)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                return GetRolesByUserName(conn, username);
            }
        }

        /// <summary>
        /// Gets the users in role.
        /// </summary>
        /// <param name="rolename">The rolename.</param>
        /// <returns>A string array containing the names of all the users 
        /// who are members of the specified role. </returns>
        public override string[] GetUsersInRole(string rolename)
        {
            List<string> users = new List<string>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    int roleId = GetRoleId(conn, rolename);

                    string sql = @"SELECT u.name FROM my_aspnet_Users u JOIN
                    my_aspnet_UsersInRoles uir ON uir.userid=u.id AND uir.roleid=@roleId
                    WHERE u.applicationId=@appId";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            users.Add(reader.GetString(0));
                    }
                }
                return users.ToArray();
            }
            catch (Exception ex)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(ex, "GetUsersInRole");
                throw;
            }
            return new string[0];
        }

        /// <summary>
        /// Determines whether [is user in role] [the specified username].
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="rolename">The rolename.</param>
        /// <returns>
        /// 	<c>true</c> if [is user in role] [the specified username]; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsUserInRole(string username, string rolename)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = @"SELECT COUNT(uir.*) FROM my_aspnet_UsersInRoles uir 
                        JOIN my_aspnet_Users u ON uir.userId=u.id
                        JOIN my_aspnet_Roles r ON uir.roleId=r.id
                        WHERE u.name LIKE @userName AND r.name LIKE @roleName";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@userName", username);
                    cmd.Parameters.AddWithValue("@roleName", rolename);
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
            catch (Exception ex)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(ex, "IsUserInRole");
                throw;
            }
        }

        /// <summary>
        /// Removes the users from roles.
        /// </summary>
        /// <param name="usernames">The usernames.</param>
        /// <param name="rolenames">The rolenames.</param>
        public override void RemoveUsersFromRoles(string[] usernames, string[] rolenames)
        {
            foreach (string rolename in rolenames)
            {
                if (!(RoleExists(rolename)))
                    throw new ProviderException("Role name not found.");
            }

            foreach (string username in usernames)
            {
                foreach (string rolename in rolenames)
                {
                    if (!(IsUserInRole(username, rolename)))
                        throw new ProviderException("User is not in role.");
                }
            }

            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();
                        string sql = @"DELETE uir FROM my_aspnet_UsersInRoles uir
                                JOIN my_aspnet_Users u ON uir.userId=u.id 
                                JOIN my_aspnet_Roles r ON uir.roleId=r.id
                                WHERE u.name LIKE @username AND r.name LIKE @rolename 
                                AND u.applicationId=@appId AND r.applicationId=@appId";

                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        cmd.Parameters.Add("@username", MySqlDbType.VarChar, 255);
                        cmd.Parameters.Add("@rolename", MySqlDbType.VarChar, 255);
                        cmd.Parameters.AddWithValue("@appId", applicationId);

                        foreach (string username in usernames)
                        {
                            foreach (string rolename in rolenames)
                            {
                                cmd.Parameters[0].Value = username;
                                cmd.Parameters[1].Value = rolename;
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (MySqlException e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "RemoveUsersFromRoles");
                throw;
            }
        }

        /// <summary>
        /// Roles the exists.
        /// </summary>
        /// <param name="rolename">The rolename.</param>
        /// <returns>true if the role name already exists in the database; otherwise, false. </returns>
        public override bool RoleExists(string rolename)
        {
            try 
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    MySqlCommand cmd = new MySqlCommand(
                        @"SELECT COUNT(*) FROM my_aspnet_Roles WHERE applicationId=@appId 
                        AND name LIKE @name", conn);
                    cmd.Parameters.AddWithValue("@appId", applicationId);
                    cmd.Parameters.AddWithValue("@name", rolename);
                    return (int)cmd.ExecuteScalar() != 0;
                }
            }
            catch (Exception ex) 
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(ex, "RoleExists");
                throw;
            }
        }

        /// <summary>
        /// Finds the users in role.
        /// </summary>
        /// <param name="rolename">The rolename.</param>
        /// <param name="usernameToMatch">The username to match.</param>
        /// <returns>A string array containing the names of all the users where the 
        /// user name matches usernameToMatch and the user is a member of the specified role. </returns>
        public override string[] FindUsersInRole(string rolename, string usernameToMatch)
        {
            List<string> users =new List<string>();

            try {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    string sql = @"SELECT u.name FROM my_aspnet_UsersInRole uir
                        JOIN my_aspnet_Users u ON uir.userId=u.id
                        JOIN my_aspnet_Roles r ON uir.roleId=r.id
                        WHERE r.name LIKE @rolename AND
                        u.name LIKE @username AND
                        u.applicationId=@appId";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@username", usernameToMatch);
                    cmd.Parameters.AddWithValue("@rolename", rolename);
                    cmd.Parameters.AddWithValue("@appId", applicationId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            users.Add(reader.GetString(0));
                    }
                }
                return users.ToArray();
            }
            catch (MySqlException e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "FindUsersInRole");
                throw;
            }
        }

        #endregion

        internal static void DeleteUserData(MySqlConnection connection, int userId)
        {
            MySqlCommand cmd = new MySqlCommand(
                "DELETE FROM my_aspnet_UsersInRoles WHERE userId=@userId", connection);
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.ExecuteNonQuery();
        }

        #region Private Methods

        private string[] GetRolesByUserName(MySqlConnection connection, string username)
        {
            List<string> roleList = new List<string>();

            try
            {
                string sql = "SELECT r.name FROM my_aspnet_roles r ";
                if (username != null)
                    sql += "JOIN my_aspnet_usersinroles uir ON uir.roleId=r.id AND uir.userid=" + 
                        GetUserId(connection, username);
                sql += " WHERE r.applicationId=@appId";
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@appId", applicationId);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        roleList.Add(reader.GetString(0));
                }
                return roleList.ToArray();
            }
            catch (Exception ex)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(ex, "GetRolesByUserName");
                throw;
            }
        }

        private int GetUserId(MySqlConnection connection, string username)
        {
            MySqlCommand cmd = new MySqlCommand(
                "SELECT id FROM my_aspnet_Users WHERE name=@name AND applicationId=@appId",
                connection);
            cmd.Parameters.AddWithValue("@name", username);
            cmd.Parameters.AddWithValue("@appId", applicationId);
            return (int)cmd.ExecuteScalar();
        }

        private int GetRoleId(MySqlConnection connection, string rolename)
        {
            MySqlCommand cmd = new MySqlCommand(
                "SELECT id FROM my_aspnet_Roles WHERE name=@name AND applicationId=@appId",
                connection);
            cmd.Parameters.AddWithValue("@name", rolename);
            cmd.Parameters.AddWithValue("@appId", applicationId);
            return (int)cmd.ExecuteScalar();
        }

        private void WriteToEventLog(Exception e, string action)
        {
            EventLog log = new EventLog();
            log.Source = eventSource;
            log.Log = eventLog;
            string message = exceptionMessage + Environment.NewLine + Environment.NewLine;
            message += "Action: " + action + Environment.NewLine + Environment.NewLine;
            message += "Exception: " + e;
            log.WriteEntry(message);
        }

        #endregion

    }
}