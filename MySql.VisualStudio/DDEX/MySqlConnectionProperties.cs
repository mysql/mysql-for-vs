// Copyright © 2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
//
// This file is part of MySQL Tools for Visual Studio.
// MySQL Tools for Visual Studio is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public 
// License version 2.1 as published by the Free Software Foundation
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

/*
 * This file contains implementation of customized connection properties. 
 */
using System;
using Microsoft.VisualStudio.Data.AdoDotNet;
using System.Data.Common;

namespace MySql.Data.VisualStudio
{
    /// <summary>
    /// This class customize standard connection properties for 
    /// MySql data base connection.
    /// </summary>
    public class MySqlConnectionProperties : AdoDotNetConnectionProperties
    {
		public static string InvariantName = "MySql.Data.MySqlClient";

        /// <summary>
        /// Constructor fills base object with list of custom options and their description.
        /// </summary>
        public MySqlConnectionProperties() : base(InvariantName)
        {
        }

        /// <summary>
        /// Test connection for these properties. Uses MySqlConnection support for version validation.
        /// </summary>
        public override void Test()
        {
            // Create connection support
            MySqlConnectionSupport conn = new MySqlConnectionSupport();
            try
            {
                // Initializes it with empty provider
                conn.Initialize(null);
                // Set connection string
                conn.ConnectionString = ToTestString();
                // Try to open
                conn.Open(false);
                // Close after open
                conn.Close();
            }
            finally
            {
                // In any case dispose connection support
                if (conn != null)
                    conn.Dispose();
            }
        }

        /// <summary>
        /// Connection properties are complete if server and database specified.
        /// </summary>
        public override bool IsComplete
        {
            get
            {
                DbConnectionStringBuilder cb = this.ConnectionStringBuilder;
                return !String.IsNullOrEmpty((string)cb["Server"])
                       && !String.IsNullOrEmpty((string)cb["Database"]);
            }
        }

    }
}