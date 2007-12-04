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
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Resources;
using System.IO;

namespace MySql.Web.Security
{
    /// <summary>
    /// 
    /// </summary>
    public static class SchemaManager
    {
        private const int schemaVersion = 3;

        /// <summary>
        /// Gets the most recent version of the schema.
        /// </summary>
        /// <value>The most recent version number of the schema.</value>
        public static int Version
        {
            get { return schemaVersion; }
        }

        internal static void CheckSchema(string connectionString, NameValueCollection config)
        {
            // make sure the user doesn't use autogenerateschema any longer
            foreach (string key in config.AllKeys)
                if (key.ToLowerInvariant() == "autogenerateschema")
                    throw new ProviderException(
                        "AutoGenerateSchema is not a valid configuration option.");

            try
            {
                int ver = GetSchemaVersion(connectionString);
                if (ver == Version) return;
            }
            catch (Exception) { }

            throw new ProviderException(
                "Unable to initialize provider.  Possibly missing or incorrect schema version.");
        }

        private static int GetSchemaVersion(string connectionString)
        {
            // retrieve the current schema version
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string[] restrictions = new string[4];
                restrictions[2] = "mysql_Membership";
                DataTable dt = conn.GetSchema("Tables", restrictions);
                if (dt.Rows.Count == 1)
                    return Convert.ToInt32(dt.Rows[0]["TABLE_COMMENT"]);

                MySqlCommand cmd = new MySqlCommand("SELECT * FROM my_aspnet_SchemaVersion", conn);
                object ver = cmd.ExecuteScalar();
                if (ver == null)
                    return 0;
                return (int)ver;
            }
        }
    }
}