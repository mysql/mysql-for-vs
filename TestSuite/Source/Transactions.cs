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
using System.IO;
using NUnit.Framework;
using System.Transactions;
using System.Data.Common;

namespace MySql.Data.MySqlClient.Tests
{
	[TestFixture]
	public class Transactions : BaseTest
	{
		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			Open();

			execSQL("DROP TABLE IF EXISTS Test");
			createTable("CREATE TABLE Test (key2 VARCHAR(1), name VARCHAR(100), name2 VARCHAR(100))", "INNODB");
		}

		[TestFixtureTearDown]
		public void FixtureTeardown()
		{
			Close();
		}

#if NET20

        void TransactionScopeInternal(bool commit) 
        {
            MySqlConnection c = new MySqlConnection(GetConnectionString(true));
            MySqlCommand cmd = new MySqlCommand("INSERT INTO test VALUES ('a', 'name', 'name2')", c);

            try 
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    c.Open();

                    cmd.ExecuteNonQuery();

                    if (commit)
                        ts.Complete();
                }

                cmd.CommandText = "SELECT COUNT(*) FROM test";
                object count = cmd.ExecuteScalar();
                Assert.AreEqual(commit ? 1 : 0, count);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                if (c != null)
                    c.Close();
            }
        }

        [Test]
        public void TransactionScopeRollback()
        {
            TransactionScopeInternal(false);
        }

        [Test]
        public void TransactionScopeCommit()
        {
            TransactionScopeInternal(true);
        }

        void TransactionScopeMultipleInternal(bool commit)
        {
            MySqlConnection c1 = new MySqlConnection(GetConnectionString(true));
            MySqlConnection c2 = new MySqlConnection(GetConnectionString(true));
            MySqlCommand cmd1 = new MySqlCommand("INSERT INTO test VALUES ('a', 'name', 'name2')", c1);
            MySqlCommand cmd2 = new MySqlCommand("INSERT INTO test VALUES ('b', 'name', 'name2')", c1);

            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    c1.Open();
                    cmd1.ExecuteNonQuery();

                    c2.Open();
                    cmd2.ExecuteNonQuery();

                    if (commit)
                        ts.Complete();
                }

                cmd1.CommandText = "SELECT COUNT(*) FROM test";
                object count = cmd1.ExecuteScalar();
                Assert.AreEqual(commit ? 2 : 0, count);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                if (c1 != null)
                    c1.Close();
                if (c2 != null)
                    c2.Close();
            }
        }

        [Test]
        public void TransactionScopeMultipleRollback()
        {
            TransactionScopeMultipleInternal(false);
        }

        [Test]
        public void TransactionScopeMultipleCommit()
        {
            TransactionScopeMultipleInternal(true);
        }


        /// <summary
        /// Bug #22042 mysql-connector-net-5.0.0-alpha BeginTransaction 
        /// </summary>
        void Bug22042()
        {
            DbProviderFactory factory =
                new MySql.Data.MySqlClient.MySqlClientFactory();
            DbConnection conexion = factory.CreateConnection();

            try
            {
                conexion.ConnectionString = GetConnectionString(true);
                conexion.Open();
                DbTransaction trans = conexion.BeginTransaction();
                trans.Rollback();
                conexion.Close();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

#endif

    }
}
