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
using System.Data;
using MySql.Data.MySqlClient;

namespace MySql.Web.Security
{
    internal static class RoleSchema
    {
        private const int schemaVersion = 1;

        public static void CheckSchema(string connectionString)
        {
            // retrieve the current membership schema version
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    UpdateSchema("mysql_Roles", conn);
                    UpdateSchema("mysql_UsersInRoles", conn);
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to check current provider schema", ex);
                }
            }
        }

        private static void UpdateSchema(string tablename, MySqlConnection connection)
        {
            int currentVersion = GetSchemaVersion(tablename, connection);

            while (currentVersion < schemaVersion)
            {
                if (0 == currentVersion)
                    GenerateFirstSchema(tablename, connection);
                currentVersion++;
            }
        }

        /// <summary>
        /// Returns the current version of the membership schema
        /// </summary>
        private static int GetSchemaVersion(string tablename, MySqlConnection connection)
        {
            string[] restrictions = new string[4];
            restrictions[2] = tablename;
            DataTable dt = connection.GetSchema("Tables", restrictions);
            if (dt.Rows.Count == 0)
                return 0;

            return Convert.ToInt32(dt.Rows[0]["TABLE_COMMENT"]);
        }

        private static void GenerateFirstSchema(string tablename, MySqlConnection connection)
        {
            string sql =
                @"CREATE TABLE  mysql_UsersInRoles(`Username` varchar(255) NOT NULL,
                `Rolename` varchar(255) NOT NULL, `ApplicationName` varchar(255) NOT NULL,
                KEY `Username` (`Username`,`Rolename`,`ApplicationName`)
                ) ENGINE=MyISAM DEFAULT CHARSET=latin1 ROW_FORMAT=DYNAMIC COMMENT='1'";

            if (tablename == "mysql_Roles")
                sql =
                    @"CREATE TABLE mysql_Roles(`Rolename` varchar(255) NOT NULL,
                `ApplicationName` varchar(255) NOT NULL, 
                KEY `Rolename` (`Rolename`,`ApplicationName`)
                ) ENGINE=MyISAM DEFAULT CHARSET=latin1 ROW_FORMAT=DYNAMIC COMMENT='1'";

            MySqlCommand cmd = new MySqlCommand(sql, connection);
            cmd.ExecuteNonQuery();
        }
    }
}