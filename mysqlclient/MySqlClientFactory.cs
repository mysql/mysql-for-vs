// Copyright (C) 2004-2006 MySQL AB
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

using System;
using System.Data.Common;

namespace MySql.Data.MySqlClient
{
    /// <summary>
    /// DBProviderFactory implementation for MysqlClient.
    /// </summary>
    public sealed class MySqlClientFactory : DbProviderFactory
    {
        public static readonly MySqlClientFactory Instance;

        static MySqlClientFactory()
        {
            Logger.WriteLine("MySqlClientFactory::ctor");
            Instance = new MySqlClientFactory();
        }

        public override DbCommandBuilder CreateCommandBuilder()
        {
            Logger.WriteLine("MySqlClientFactory::CreateCommandBuilder");
            return new MySqlCommandBuilder();
        }

        public override DbCommand CreateCommand()
        {
            Logger.WriteLine("MySqlClientFactory::CreateCommand");
            return new MySqlCommand();
        }

        public override DbConnection CreateConnection()
        {
            Logger.WriteLine("MySqlClientFactory::CreateConnection");
            return new MySqlConnection();
        }

        public override DbDataAdapter CreateDataAdapter()
        {
            Logger.WriteLine("MySqlClientFactory::CreateDataAdapter");
            return new MySqlDataAdapter();
        }

        public override DbParameter CreateParameter()
        {
            Logger.WriteLine("MySqlClientFactory::CreateParameter");
            return new MySqlParameter();
        }

        public override DbConnectionStringBuilder CreateConnectionStringBuilder()
        {
            Logger.WriteLine("MySqlClientFactory::CreateConnectionString");
            return new MySqlConnectionStringBuilder();
        }

        public override bool CanCreateDataSourceEnumerator
        {
            get { return false; }
        }
    }
}
