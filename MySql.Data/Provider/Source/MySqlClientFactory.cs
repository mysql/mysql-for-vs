// Copyright (C) 2004-2007 MySQL AB
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

#if !PocketPC

using System.Data.Common;
using System;
using System.Reflection;

namespace MySql.Data.MySqlClient
{
    /// <summary>
    /// DBProviderFactory implementation for MysqlClient.
    /// </summary>
    public sealed class MySqlClientFactory : DbProviderFactory, IServiceProvider
    {
        /// <summary>
        /// Gets an instance of the <see cref="MySqlClientFactory"/>. 
        /// This can be used to retrieve strongly typed data objects. 
        /// </summary>
        public static MySqlClientFactory Instance = new MySqlClientFactory();
        private Type dbServicesType;
        private FieldInfo mySqlDbProviderServicesInstance;

        /// <summary>
        /// Returns a strongly typed <see cref="DbCommandBuilder"/> instance. 
        /// </summary>
        /// <returns>A new strongly typed instance of <b>DbCommandBuilder</b>.</returns>
        public override DbCommandBuilder CreateCommandBuilder()
        {
            return new MySqlCommandBuilder();
        }

        /// <summary>
        /// Returns a strongly typed <see cref="DbCommand"/> instance. 
        /// </summary>
        /// <returns>A new strongly typed instance of <b>DbCommand</b>.</returns>
        public override DbCommand CreateCommand()
        {
            return new MySqlCommand();
        }

        /// <summary>
        /// Returns a strongly typed <see cref="DbConnection"/> instance. 
        /// </summary>
        /// <returns>A new strongly typed instance of <b>DbConnection</b>.</returns>
        public override DbConnection CreateConnection()
        {
            return new MySqlConnection();
        }

        /// <summary>
        /// Returns a strongly typed <see cref="DbDataAdapter"/> instance. 
        /// </summary>
        /// <returns>A new strongly typed instance of <b>DbDataAdapter</b>. </returns>
        public override DbDataAdapter CreateDataAdapter()
        {
            return new MySqlDataAdapter();
        }

        /// <summary>
        /// Returns a strongly typed <see cref="DbParameter"/> instance. 
        /// </summary>
        /// <returns>A new strongly typed instance of <b>DbParameter</b>.</returns>
        public override DbParameter CreateParameter()
        {
            return new MySqlParameter();
        }

        /// <summary>
        /// Returns a strongly typed <see cref="DbConnectionStringBuilder"/> instance. 
        /// </summary>
        /// <returns>A new strongly typed instance of <b>DbConnectionStringBuilder</b>.</returns>
        public override DbConnectionStringBuilder CreateConnectionStringBuilder()
        {
            return new MySqlConnectionStringBuilder();
        }

        /// <summary>
        /// Returns true if a <b>MySqlDataSourceEnumerator</b> can be created; 
        /// otherwise false. 
        /// </summary>
        public override bool CanCreateDataSourceEnumerator
        {
            get { return false; }
        }

        #region IServiceProvider Members

        /// <summary>
        /// Provide a simple caching layer
        /// </summary>
        private Type DbServicesType
        {
            get 
            {
                if (dbServicesType == null)
                {
                    // Get the type this way so we don't have to reference System.Data.Entity
                    // from our core provider
                    dbServicesType = Type.GetType(
                        @"System.Data.Common.DbProviderServices, System.Data.Entity, 
                        Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", 
                                                                                          false);
                }
                return dbServicesType;
            }
        }

        private FieldInfo MySqlDbProviderServicesInstance
        {
            get
            {
                if (mySqlDbProviderServicesInstance == null)
                {
                    string fullName = Assembly.GetExecutingAssembly().FullName;
                    fullName = fullName.Replace("MySql.Data", "MySql.Data.Entity");
                    fullName = String.Format("MySql.Data.MySqlClient.MySqlProviderServices, {0}", fullName);

                    Type providerServicesType = Type.GetType(fullName, false);
                    mySqlDbProviderServicesInstance = providerServicesType.GetField("Instance",
                        BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                }
                return mySqlDbProviderServicesInstance;
            }
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            // DbProviderServices is the only service we offer up right now
            if (serviceType != DbServicesType) return null;

            if (MySqlDbProviderServicesInstance == null) return null;

            return MySqlDbProviderServicesInstance.GetValue(null);
        }

        #endregion
    }
}

#endif
