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

using System;
using System.Data.Common;

namespace MySql.Data.MySqlClient
{
    /// <summary>
    /// DBProviderFactory implementation for MysqlClient.
    /// </summary>
    public sealed class MySqlClientFactory : DbProviderFactory
    {
        /// <summary>
        /// Gets an instance of the <see cref="MySqlClientFactory"/>. 
        /// This can be used to retrieve strongly typed data objects. 
        /// </summary>
        public static readonly MySqlClientFactory Instance;

        static MySqlClientFactory()
        {
            Instance = new MySqlClientFactory();
        }

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
    }
}

#endif
