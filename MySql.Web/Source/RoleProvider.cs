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
using Microsoft.VisualBasic;
using MySql.Data.MySqlClient;
using MySql.Data.Types;

namespace MySql.Web.Security
{
    public sealed class MySqlRoleProvider : RoleProvider
    {
        private MySqlConnection conn;
        private string eventSource = "MySQLRoleProvider";
        private string eventLog = "Application";
        private string exceptionMessage = "An exception occurred. Please check the Event Log.";
        private ConnectionStringSettings pConnectionStringSettings;
        private string connectionString;
        private bool pWriteExceptionsToEventLog = false;
        private string pApplicationName;

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
                pApplicationName = System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath;
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
            if (pConnectionStringSettings == null || pConnectionStringSettings.ConnectionString.Trim() == "")
            {
                throw new ProviderException("Connection string cannot be blank.");
            }
            connectionString = pConnectionStringSettings.ConnectionString;
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

        #endregion

        public override void AddUsersToRoles(string[] usernames, string[] rolenames)
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
                if (username.Contains(","))
                {
                    throw new ArgumentException("User names cannot contain commas.");
                }
                foreach (string rolename in rolenames)
                {
                    if (IsUserInRole(username, rolename))
                    {
                        throw new ProviderException("User is already in role.");
                    }
                }
            }
            MySqlConnection conn = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand("INSERT INTO MySqlWebUsersInRoles " + " (Username, Rolename, ApplicationName) " + " Values(?Username, ?Rolename, ?ApplicationName)", conn);
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
                try
                {
                    tran.Rollback();
                }
                catch
                {
                }
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "AddUsersToRoles");
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

        public override void CreateRole(string rolename)
        {
            if (rolename.Contains(","))
            {
                throw new ArgumentException("Role names cannot contain commas.");
            }
            if (RoleExists(rolename))
            {
                throw new ProviderException("Role name already exists.");
            }
            MySqlConnection conn = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand("INSERT INTO MySqlWebRoles " + " (Rolename, ApplicationName) " + " Values(?Rolename, ?ApplicationName)", conn);
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
                {
                    WriteToEventLog(e, "CreateRole");
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

        public override bool DeleteRole(string rolename, bool throwOnPopulatedRole)
        {
            if (!(RoleExists(rolename)))
            {
                throw new ProviderException("Role does not exist.");
            }
            if (throwOnPopulatedRole && GetUsersInRole(rolename).Length > 0)
            {
                throw new ProviderException("Cannot delete a populated role.");
            }
            MySqlConnection conn = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand("DELETE FROM MySqlWebRoles " + " WHERE Rolename = ?Rolename AND ApplicationName = ?ApplicationName", conn);
            cmd.Parameters.Add("?Rolename", MySqlDbType.VarChar, 255).Value = rolename;
            cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;
            MySqlCommand cmd2 = new MySqlCommand("DELETE FROM MySqlWebUsersInRoles " + " WHERE Rolename = ?Rolename AND ApplicationName = ?ApplicationName", conn);
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
            }
            catch (MySqlException e)
            {
                try
                {
                    tran.Rollback();
                }
                catch
                {
                }
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "DeleteRole");
                    return false;
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
            return true;
        }

        public override string[] GetAllRoles()
        {
            string tmpRoleNames = "";
            MySqlConnection conn = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand("SELECT Rolename FROM MySqlWebRoles " + " WHERE ApplicationName = ?ApplicationName", conn);
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
                {
                    WriteToEventLog(e, "GetAllRoles");
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
            if (tmpRoleNames.Length > 0)
            {
                tmpRoleNames = tmpRoleNames.Substring(0, tmpRoleNames.Length - 1);
                return tmpRoleNames.Split(System.Convert.ToChar(","));
            }
            return new string[0];
        }

        public override string[] GetRolesForUser(string username) 
   { 
     string tmpRoleNames = ""; 
     MySqlConnection conn = new MySqlConnection(connectionString); 
     MySqlCommand cmd = new MySqlCommand("SELECT Rolename FROM MySqlWebUsersInRoles " + " WHERE Username = ?Username AND ApplicationName = ?ApplicationName", conn); 
     cmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255).Value = username; 
     cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName; 
     MySqlDataReader reader = null; 
     try { 
       conn.Open(); 
       reader = cmd.ExecuteReader(); 
       while (reader.Read()) { 
         tmpRoleNames += reader.GetString(0) + ","; 
       } 
     } catch (MySqlException e) { 
       if (WriteExceptionsToEventLog) { 
         WriteToEventLog(e, "GetRolesForUser"); 
       } else { 
         throw e; 
       } 
     } finally { 
       if (!(reader == null)) { 
         reader.Close(); 
       } 
       conn.Close(); 
     } 
     if (tmpRoleNames.Length > 0) { 
       tmpRoleNames = tmpRoleNames.Substring(0, tmpRoleNames.Length - 1); 
       return tmpRoleNames.Split(System.Convert.ToChar(",")); 
     } 
     return new string[0]; 
   }

        public override string[] GetUsersInRole(string rolename) 
   { 
     string tmpUserNames = ""; 
     MySqlConnection conn = new MySqlConnection(connectionString); 
     MySqlCommand cmd = new MySqlCommand("SELECT Username FROM MySqlWebUsersInRoles " + " WHERE Rolename = ?Rolename AND ApplicationName = ?ApplicationName", conn); 
     cmd.Parameters.Add("?Rolename", MySqlDbType.VarChar, 255).Value = rolename; 
     cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName; 
     MySqlDataReader reader = null; 
     try { 
       conn.Open(); 
       reader = cmd.ExecuteReader(); 
       while (reader.Read()) { 
         tmpUserNames += reader.GetString(0) + ","; 
       } 
     } catch (MySqlException e) { 
       if (WriteExceptionsToEventLog) { 
         WriteToEventLog(e, "GetUsersInRole"); 
       } else { 
         throw e; 
       } 
     } finally { 
       if (!(reader == null)) { 
         reader.Close(); 
       } 
       conn.Close(); 
     } 
     if (tmpUserNames.Length > 0) { 
       tmpUserNames = tmpUserNames.Substring(0, tmpUserNames.Length - 1); 
       return tmpUserNames.Split(System.Convert.ToChar(",")); 
     } 
     return new string[0]; 
   }

        public override bool IsUserInRole(string username, string rolename)
        {
            bool userIsInRole = false;
            MySqlConnection conn = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) FROM MySqlWebUsersInRoles " + " WHERE Username = ?Username AND Rolename = ?Rolename AND ApplicationName = ?ApplicationName", conn);
            cmd.Parameters.Add("?Username", MySqlDbType.VarChar, 255).Value = username;
            cmd.Parameters.Add("?Rolename", MySqlDbType.VarChar, 255).Value = rolename;
            cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;
            try
            {
                conn.Open();
                int numRecs = ((int)(cmd.ExecuteScalar()));
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
                    throw e;
                }
            }
            finally
            {
                conn.Close();
            }
            return userIsInRole;
        }

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
            MySqlConnection conn = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand("DELETE FROM MySqlWebUsersInRoles " + " WHERE Username = ?Username AND Rolename = ?Rolename AND ApplicationName = ?ApplicationName", conn);
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
                try
                {
                    tran.Rollback();
                }
                catch
                {
                }
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "RemoveUsersFromRoles");
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

        public override bool RoleExists(string rolename)
        {
            bool exists = false;
            MySqlConnection conn = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) FROM MySqlWebRoles " + " WHERE Rolename = ?Rolename AND ApplicationName = ?ApplicationName", conn);
            cmd.Parameters.Add("?Rolename", MySqlDbType.VarChar, 255).Value = rolename;
            cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName;
            try
            {
                conn.Open();
                int numRecs = ((int)(cmd.ExecuteScalar()));
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
                    throw e;
                }
            }
            finally
            {
                conn.Close();
            }
            return exists;
        }

        public override string[] FindUsersInRole(string rolename, string usernameToMatch) 
   { 
     MySqlConnection conn = new MySqlConnection(connectionString); 
     MySqlCommand cmd = new MySqlCommand("SELECT Username FROM MySqlWebUsersInRoles " + "WHERE Username LIKE ?UsernameSearch AND RoleName = ?RoleName AND ApplicationName = ?ApplicationName", conn); 
     cmd.Parameters.Add("?UsernameSearch", MySqlDbType.VarChar, 255).Value = usernameToMatch; 
     cmd.Parameters.Add("?RoleName", MySqlDbType.VarChar, 255).Value = rolename; 
     cmd.Parameters.Add("?ApplicationName", MySqlDbType.VarChar, 255).Value = pApplicationName; 
     string tmpUserNames = ""; 
     MySqlDataReader reader = null; 
     try { 
       conn.Open(); 
       reader = cmd.ExecuteReader(); 
       while (reader.Read()) { 
         tmpUserNames += reader.GetString(0) + ","; 
       } 
     } catch (MySqlException e) { 
       if (WriteExceptionsToEventLog) { 
         WriteToEventLog(e, "FindUsersInRole"); 
       } else { 
         throw e; 
       } 
     } finally { 
       if (!(reader == null)) { 
         reader.Close(); 
       } 
       conn.Close(); 
     } 
     if (tmpUserNames.Length > 0) { 
       tmpUserNames = tmpUserNames.Substring(0, tmpUserNames.Length - 1); 
       return tmpUserNames.Split(System.Convert.ToChar(",")); 
     } 
     return new string[0]; 
   }

        private void WriteToEventLog(MySqlException e, string action)
        {
            EventLog log = new EventLog();
            log.Source = eventSource;
            log.Log = eventLog;
            string message = exceptionMessage + Environment.NewLine + Environment.NewLine;
            message += "Action: " + action + Environment.NewLine + Environment.NewLine;
            message += "Exception: " + e.ToString();
            log.WriteEntry(message);
        }
    }
}