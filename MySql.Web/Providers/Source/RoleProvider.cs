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

namespace MySql.Web.Security
{
    public sealed class MySQLRoleProvider : RoleProvider
    {
        private string eventSource = "MySQLRoleProvider";
        private string eventLog = "Application";
        private string exceptionMessage = "An exception occurred. Please check the Event Log.";
        private ConnectionStringSettings pConnectionStringSettings;
        private string connectionString;
        private bool pWriteExceptionsToEventLog = false;
        private string pApplicationName;

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
            {
                pApplicationName = HostingEnvironment.ApplicationVirtualPath;
            }
            else
            {
                pApplicationName = config["applicationName"];
            }
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
            string autoGenSchema = config["AutoGenerateSchema"];
            if ((String.IsNullOrEmpty(autoGenSchema) || Convert.ToBoolean(autoGenSchema)) &&
                connectionString != String.Empty)
                RoleSchema.CheckSchema(connectionString);
        }

        #region Properties

        /// <summary>
        /// Gets or sets the name of the application to store and retrieve role information for.
        /// </summary>
        /// <value></value>
        /// <returns>The name of the application to store and retrieve role information for.</returns>
        public override string ApplicationName
        {
            get { return pApplicationName; }
            set { pApplicationName = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [write exceptions to event log].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [write exceptions to event log]; otherwise, <c>false</c>.
        /// </value>
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
                    {
                        throw new ProviderException("User is already in role.");
                    }
                }
            }

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand cmd =
                    new MySqlCommand(
                        @"INSERT INTO mysql_UsersInRoles (Username, Rolename, ApplicationName) 
                Values(?Username, ?Rolename, ?ApplicationName)",
                        conn);
                MySqlParameter userParm = cmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255);
                MySqlParameter roleParm = cmd.Parameters.Add("?Rolename", MySqlDbType.VarChar, 255);
                cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;
                MySqlTransaction tran = null;
                try
                {
                    conn.Open();
                    tran = conn.BeginTransaction();
                    cmd.Transaction = tran;
                    foreach (string username in usernames)
                    {
                        foreach (string rolename in rolenames)
                        {
                            userParm.Value = username;
                            roleParm.Value = rolename;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    tran.Commit();
                }
                catch (MySqlException e)
                {
                    if (tran != null)
                        tran.Rollback();
                    if (WriteExceptionsToEventLog)
                        WriteToEventLog(e, "AddUsersToRoles");
                    else
                        throw;
                }
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
                        @"INSERT INTO mysql_Roles (Rolename, ApplicationName) 
                Values(?Rolename, ?ApplicationName)",
                        conn);
                cmd.Parameters.Add("?Rolename", MySqlDbType.VarChar, 255).Value = rolename;
                cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (MySqlException e)
                {
                    if (WriteExceptionsToEventLog)
                        WriteToEventLog(e, "CreateRole");
                    else
                        throw;
                }
            }
        }

        /// <summary>
        /// Deletes the role.
        /// </summary>
        /// <param name="rolename">The rolename.</param>
        /// <param name="throwOnPopulatedRole">if set to <c>true</c> [throw on populated role].</param>
        /// <returns></returns>
        public override bool DeleteRole(string rolename, bool throwOnPopulatedRole)
        {
            if (!(RoleExists(rolename)))
                throw new ProviderException("Role does not exist.");
            if (throwOnPopulatedRole && GetUsersInRole(rolename).Length > 0)
                throw new ProviderException("Cannot delete a populated role.");

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand cmd =
                    new MySqlCommand(
                        @"DELETE FROM mysql_Roles 
                WHERE Rolename = ?Rolename AND ApplicationName = ?ApplicationName",
                        conn);
                cmd.Parameters.Add("?Rolename", MySqlDbType.VarChar, 255).Value = rolename;
                cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;
                MySqlCommand cmd2 =
                    new MySqlCommand(
                        @"DELETE FROM mysql_UsersInRoles 
                WHERE Rolename = ?Rolename AND ApplicationName = ?ApplicationName",
                        conn);
                cmd2.Parameters.Add("?Rolename", MySqlDbType.VarChar, 255).Value = rolename;
                cmd2.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;

                MySqlTransaction tran = null;
                try
                {
                    conn.Open();
                    tran = conn.BeginTransaction();
                    cmd.Transaction = tran;
                    cmd2.Transaction = tran;
                    cmd2.ExecuteNonQuery();
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                    return true;
                }
                catch (MySqlException e)
                {
                    if (tran != null)
                        tran.Rollback();
                    if (WriteExceptionsToEventLog)
                        WriteToEventLog(e, "DeleteRole");
                    else
                        throw;
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets a list of all the roles for the configured applicationName.
        /// </summary>
        /// <returns>
        /// A string array containing the names of all the roles stored in the data source for the configured applicationName.
        /// </returns>
        public override string[] GetAllRoles()
        {
            string tmpRoleNames = "";
            MySqlConnection conn = new MySqlConnection(connectionString);
            MySqlCommand cmd =
                new MySqlCommand(
                    @"SELECT Rolename FROM mysql_Roles 
                WHERE ApplicationName = ?ApplicationName", conn);
            cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;
            MySqlDataReader reader = null;

            try
            {
                conn.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    tmpRoleNames += reader.GetString(0) + ",";
                }
            }
            catch (MySqlException e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "GetAllRoles");
                else
                    throw;
            }
            finally
            {
                if (!(reader == null))
                {
                    reader.Close();
                }
                conn.Close();
            }
            if (tmpRoleNames.Length > 0)
            {
                tmpRoleNames = tmpRoleNames.Substring(0, tmpRoleNames.Length - 1);
                return tmpRoleNames.Split(Convert.ToChar(","));
            }
            return new string[0];
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
            string tmpRoleNames = "";
            MySqlConnection conn = new MySqlConnection(connectionString);
            MySqlCommand cmd =
                new MySqlCommand(
                    @"SELECT Rolename FROM mysql_UsersInRoles 
                WHERE Username = ?Username AND ApplicationName = ?ApplicationName",
                    conn);
            cmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255).Value = username;
            cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;
            MySqlDataReader reader = null;
            try
            {
                conn.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    tmpRoleNames += reader.GetString(0) + ",";
                }
            }
            catch (MySqlException e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "GetRolesForUser");
                else
                    throw;
            }
            finally
            {
                if (!(reader == null))
                {
                    reader.Close();
                }
                conn.Close();
            }
            if (tmpRoleNames.Length > 0)
            {
                tmpRoleNames = tmpRoleNames.Substring(0, tmpRoleNames.Length - 1);
                return tmpRoleNames.Split(Convert.ToChar(","));
            }
            return new string[0];
        }

        /// <summary>
        /// Gets the users in role.
        /// </summary>
        /// <param name="rolename">The rolename.</param>
        /// <returns></returns>
        public override string[] GetUsersInRole(string rolename)
        {
            string tmpUserNames = "";
            MySqlConnection conn = new MySqlConnection(connectionString);
            MySqlCommand cmd =
                new MySqlCommand(
                    @"SELECT Username FROM mysql_UsersInRoles 
                WHERE Rolename = ?Rolename AND ApplicationName = ?ApplicationName",
                    conn);
            cmd.Parameters.Add("?Rolename", MySqlDbType.VarChar, 255).Value = rolename;
            cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;
            MySqlDataReader reader = null;
            try
            {
                conn.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    tmpUserNames += reader.GetString(0) + ",";
                }
            }
            catch (MySqlException e)
            {
                if (WriteExceptionsToEventLog)
                    WriteToEventLog(e, "GetUsersInRole");
                else
                    throw;
            }
            finally
            {
                if (!(reader == null))
                    reader.Close();
                conn.Close();
            }
            if (tmpUserNames.Length > 0)
            {
                tmpUserNames = tmpUserNames.Substring(0, tmpUserNames.Length - 1);
                return tmpUserNames.Split(Convert.ToChar(","));
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
            bool userIsInRole = false;
            MySqlConnection conn = new MySqlConnection(connectionString);
            MySqlCommand cmd =
                new MySqlCommand(
                    @"SELECT COUNT(*) FROM mysql_UsersInRoles 
                WHERE Username = ?Username AND Rolename = ?Rolename AND 
                ApplicationName = ?ApplicationName",
                    conn);
            cmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255).Value = username;
            cmd.Parameters.Add("?Rolename", MySqlDbType.VarChar, 255).Value = rolename;
            cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;
            try
            {
                conn.Open();
                int numRecs = Convert.ToInt32(cmd.ExecuteScalar());
                if (numRecs > 0)
                {
                    userIsInRole = true;
                }
            }
            catch (MySqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "IsUserInRole");
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                conn.Close();
            }
            return userIsInRole;
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
                {
                    throw new ProviderException("Role name not found.");
                }
            }
            foreach (string username in usernames)
            {
                foreach (string rolename in rolenames)
                {
                    if (!(IsUserInRole(username, rolename)))
                    {
                        throw new ProviderException("User is not in role.");
                    }
                }
            }

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand cmd =
                    new MySqlCommand(
                        @"DELETE FROM mysql_UsersInRoles 
                WHERE Username = ?Username AND Rolename = ?Rolename AND 
                ApplicationName = ?ApplicationName",
                        conn);
                MySqlParameter userParm = cmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255);
                MySqlParameter roleParm = cmd.Parameters.Add("?Rolename", MySqlDbType.VarChar, 255);
                cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;
                MySqlTransaction tran = null;
                try
                {
                    conn.Open();
                    tran = conn.BeginTransaction();
                    cmd.Transaction = tran;
                    foreach (string username in usernames)
                    {
                        foreach (string rolename in rolenames)
                        {
                            userParm.Value = username;
                            roleParm.Value = rolename;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    tran.Commit();
                }
                catch (MySqlException e)
                {
                    if (tran != null)
                        tran.Rollback();
                    if (WriteExceptionsToEventLog)
                        WriteToEventLog(e, "RemoveUsersFromRoles");
                    else
                        throw;
                }
            }
        }

        /// <summary>
        /// Roles the exists.
        /// </summary>
        /// <param name="rolename">The rolename.</param>
        /// <returns></returns>
        public override bool RoleExists(string rolename)
        {
            bool exists = false;
            MySqlConnection conn = new MySqlConnection(connectionString);
            MySqlCommand cmd =
                new MySqlCommand(
                    @"SELECT COUNT(*) FROM mysql_Roles 
                WHERE Rolename = ?Rolename AND ApplicationName = ?ApplicationName",
                    conn);
            cmd.Parameters.Add("?Rolename", MySqlDbType.VarChar, 255).Value = rolename;
            cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;
            try
            {
                conn.Open();
                int numRecs = Convert.ToInt32(cmd.ExecuteScalar());
                if (numRecs > 0)
                {
                    exists = true;
                }
            }
            catch (MySqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "RoleExists");
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                conn.Close();
            }
            return exists;
        }

        /// <summary>
        /// Finds the users in role.
        /// </summary>
        /// <param name="rolename">The rolename.</param>
        /// <param name="usernameToMatch">The username to match.</param>
        /// <returns></returns>
        public override string[] FindUsersInRole(string rolename, string usernameToMatch)
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            MySqlCommand cmd =
                new MySqlCommand(
                    @"SELECT Username FROM mysql_UsersInRoles 
                WHERE Username LIKE ?UsernameSearch AND RoleName = ?RoleName AND 
                ApplicationName = ?ApplicationName",
                    conn);
            cmd.Parameters.Add("?UsernameSearch", MySqlDbType.VarChar, 255).Value = usernameToMatch;
            cmd.Parameters.Add("?RoleName", MySqlDbType.VarChar, 255).Value = rolename;
            cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;
            string tmpUserNames = "";
            MySqlDataReader reader = null;
            try
            {
                conn.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    tmpUserNames += reader.GetString(0) + ",";
                }
            }
            catch (MySqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "FindUsersInRole");
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
            if (tmpUserNames.Length > 0)
            {
                tmpUserNames = tmpUserNames.Substring(0, tmpUserNames.Length - 1);
                return tmpUserNames.Split(Convert.ToChar(","));
            }
            return new string[0];
        }

        #endregion

        #region Private Methods

        private void WriteToEventLog(MySqlException e, string action)
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