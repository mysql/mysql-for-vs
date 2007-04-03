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
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Data;

namespace MySql.Web.Security
{
    internal static class MembershipSchema
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
                    int currentVersion = GetSchemaVersion(conn);

                    while (currentVersion < schemaVersion)
                    {
                        if (0 == currentVersion)
                            GenerateFirstSchema(conn);
                        currentVersion++;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to check current provider schema", ex);
                }
            }
        }

        /// <summary>
        /// Returns the current version of the membership schema
        /// </summary>
        private static int GetSchemaVersion(MySqlConnection connection)
        {
            string[] restrictions = new string[4];
            restrictions[2] = "mysql_Membership";
            DataTable dt = connection.GetSchema("Tables", restrictions);
            if (dt.Rows.Count == 0)
                return 0;

            return Convert.ToInt32(dt.Rows[0]["TABLE_COMMENT"]);
        }

        private static void GenerateFirstSchema(MySqlConnection connection)
        {
            string sql = @"CREATE TABLE  mysql_Membership(`PKID` varchar(36) NOT NULL,
                `Username` varchar(255) NOT NULL, `ApplicationName` varchar(255) NOT NULL,
                `Email` varchar(128) NOT NULL, `Comment` varchar(255) default NULL,
                `Password` varchar(128) NOT NULL, `PasswordQuestion` varchar(255) default NULL,
                `PasswordAnswer` varchar(255) default NULL, `IsApproved` tinyint(1) default NULL,
                `LastActivityDate` datetime default NULL, `LastLoginDate` datetime default NULL,
                `LastPasswordChangedDate` datetime default NULL, `CreationDate` datetime default NULL,
                `IsOnline` tinyint(1) default NULL, `IsLockedOut` tinyint(1) default NULL,
                `LastLockedOutDate` datetime default NULL, 
                `FailedPasswordAttemptCount` int(10) unsigned default NULL,
                `FailedPasswordAttemptWindowStart` datetime default NULL,
                `FailedPasswordAnswerAttemptCount` int(10) unsigned default NULL,
                `FailedPasswordAnswerAttemptWindowStart` datetime default NULL,
                PRIMARY KEY  (`PKID`)) ENGINE=MyISAM DEFAULT CHARSET=latin1 COMMENT='1'";

            MySqlCommand cmd = new MySqlCommand(sql, connection);
            cmd.ExecuteNonQuery();
        }
    }
}
