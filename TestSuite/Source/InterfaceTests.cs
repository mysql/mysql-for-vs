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

using System;
using System.Data;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using System.Data.Common;

namespace MySql.Data.MySqlClient.Tests
{
	[NUnit.Framework.TestFixture]
	public class InterfaceTests : BaseTest
	{
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			Open();
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown() 
		{
			Close();
		}

#if !NET_CF
        [Test]
        public void ClientFactory()
        {
            DbProviderFactory f = new MySqlClientFactory();
            DbConnection c = f.CreateConnection();
            DbConnectionStringBuilder cb = f.CreateConnectionStringBuilder();
            cb.ConnectionString = GetConnectionString(true);
            c.ConnectionString = cb.ConnectionString;
            c.Open();

            DbCommand cmd = f.CreateCommand();
            cmd.Connection = c;
            cmd.CommandText = "SHOW TABLES FROM test";
            cmd.CommandType = CommandType.Text;
            DbDataReader reader = null;
            try
            {
                reader = cmd.ExecuteReader();
                reader.Read();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

        }
#endif
	}
}
