// Copyright (C) 2006-2007 MySQL AB
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
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA using System;

/*
 * This file contains implementation of customized connection properties. 
 */
using System;
using Microsoft.VisualStudio.Data.AdoDotNet;

namespace MySql.Data.VisualStudio
{
    /// <summary>
    /// This class customize standard connection properties for 
    /// MySql data base connection.
    /// </summary>
    public class MySqlConnectionProperties : AdoDotNetConnectionProperties
    {
        #region Option names

        /// <summary>
        /// This class contains names for all connection options
        /// </summary>
        public static class Names
        {
            public const string Server = "Server";
            public const string Database = "Database";
            public const string ConnectionProtocol = "Protocol";
            public const string PipeName = "Pipe Name";
            public const string UseCompression = "Compress";
            public const string AllowBatch = "Allow Batch";
            public const string Logging = "Logging";
            public const string SharedMemoryName = "Shared Memory Name";
            public const string UseOldSyntax = "Old Syntax";
            public const string DriverType = "Driver Type";
            public const string OptionFile = "Option File";
            public const string Port = "Port";
            public const string ConnectionTimeout = "Connection Timeout";
            public const string UserID = "User ID";
            public const string Password = "Password";
            public const string PersistSecurityInfo = "Persist Security Info";
            public const string UseSSL = "UseSSL";
            public const string AllowZeroDateTime = "Allow Zero DateTime";
            public const string ConvertZeroDateTime = "Convert Zero DateTime";
            public const string CharacterSet = "Character Set";
            public const string UseUsageAdvisor = "Use Usage Advisor";
            public const string ProcedureCacheSize = "Procedure Cache Size";
            public const string UsePerformanceMonitor = "Use Performance Monitor";
            public const string ConnectionLifeTime = "Connection LifeTime";
            public const string Pooling = "Pooling";
            public const string MinimumPoolSize = "Minimum Pool Size";
            public const string MaximumPoolSize = "Maximum Pool Size";
            public const string ConnectionReset = "Connection Reset";
            public const string InvariantProviderName = "MySql.Data.MySqlClient";
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor fills base object with list of custom options and their description.
        /// </summary>
        public MySqlConnectionProperties()
            : base(Names.InvariantProviderName)
        {
        }

        #endregion

        #region Overridings

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
                return !String.IsNullOrEmpty(this[Names.Server] as string)
                       && !String.IsNullOrEmpty(this[Names.Database] as string);
            }
        }

        #endregion
    }
}