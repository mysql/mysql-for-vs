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
#if NET20
using System.Data.Common;
#endif

namespace MySql.Data.MySqlClient.Tests
{
	[TestFixture]
	public class SimpleTransactions : BaseTest
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

		[Test]
		public void TestReader() 
		{
			execSQL("INSERT INTO Test VALUES('P', 'Test1', 'Test2')");

			MySqlTransaction txn = conn.BeginTransaction();
			MySqlConnection c = txn.Connection;
			Assert.AreEqual( conn, c );
			MySqlCommand cmd = new MySqlCommand("SELECT name, name2 FROM Test WHERE key2='P'", 
				conn, txn);
			MySqlTransaction t2 = cmd.Transaction;
			Assert.AreEqual( txn, t2 );
			MySqlDataReader reader = null;
			try 
			{
				reader = cmd.ExecuteReader();
				reader.Close();
				txn.Commit();
			}
			catch (Exception ex) 
			{
				Assert.Fail( ex.Message );
				txn.Rollback();
			}
			finally 
			{
				if (reader != null) reader.Close();
			}
		}

        /// <summary>
        /// Bug #22400 Nested transactions 
        /// </summary>
        [Test]
        public void NestedTransactions()
        {
            MySqlTransaction t1 = conn.BeginTransaction();
            try
            {
                MySqlTransaction t2 = conn.BeginTransaction();
                Assert.Fail("Exception should have been thrown");
                t2.Rollback();
            }
            catch (InvalidOperationException)
            {
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally 
            {
                t1.Rollback();
            }
        }

        [Test]
        public void BeginTransactionOnPreviouslyOpenConnection()
        {
            string connStr = GetConnectionString(true);
            MySqlConnection c = new MySqlConnection(connStr);
            c.Open();
            c.Close();
            try
            {
                MySqlTransaction t = c.BeginTransaction();
            }
            catch (Exception ex)
            {
                Assert.AreEqual("The connection is not open.", ex.Message);
            }
        }
	}
}
