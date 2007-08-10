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
        protected override void Setup()
        {
            base.Setup();

            execSQL("DROP TABLE IF EXISTS Test");
            createTable("CREATE TABLE Test (key2 VARCHAR(1), name VARCHAR(100), name2 VARCHAR(100))", "INNODB");
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

#endif

        /// <summary>
        /// Bug #27289 Transaction is not rolledback when connection close 
        /// </summary>
        [Test]
        public void RollingBackOnClose()
        {
            execSQL("DROP TABLE IF EXISTS test");
            execSQL("CREATE TABLE test (id INT) TYPE=InnoDB");

            string connStr = GetConnectionString(true) + ";pooling=true;";
            MySqlConnection c = new MySqlConnection(connStr);
            c.Open();
            MySqlCommand cmd = new MySqlCommand("INSERT INTO test VALUES (1)", c);
            MySqlTransaction tx = c.BeginTransaction();
            cmd.ExecuteNonQuery();
            c.Close();

            MySqlConnection c2 = new MySqlConnection(connStr);
            c2.Open();
            MySqlCommand cmd2 = new MySqlCommand("SELECT COUNT(*) from test", c2);
            MySqlTransaction tx2 = c2.BeginTransaction();
            object count = cmd2.ExecuteScalar();
            c2.Close();
            Assert.AreEqual(0, count);
        }

#if NET20

        /// <summary>
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

        /// <summary>
        /// Bug #26754  	EnlistTransaction throws false MySqlExeption "Already enlisted"
        /// </summary>
        [Test]
        public void EnlistTransactionNullTest()
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.Connection.EnlistTransaction(null);
            }
            catch { }

            using (TransactionScope ts = new TransactionScope())
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                try
                {
                    cmd.Connection.EnlistTransaction(Transaction.Current);
                }
                catch (MySqlException)
                {
                    Assert.Fail("No exception should have been thrown");
                }
            }
        }

        /// <summary>
        /// Bug #26754  	EnlistTransaction throws false MySqlExeption "Already enlisted"
        /// </summary>
        [Test]
        public void EnlistTransactionWNestedTrxTest()
        {
            MySqlTransaction t = conn.BeginTransaction();

            using (TransactionScope ts = new TransactionScope())
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                try
                {
                    cmd.Connection.EnlistTransaction(Transaction.Current);
                }
                catch (InvalidOperationException) 
                { 
                    /* caught NoNestedTransactions */  
                }
            }

            t.Rollback();

            using (TransactionScope ts = new TransactionScope())
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                try
                {
                    cmd.Connection.EnlistTransaction(Transaction.Current);
                }
                catch (MySqlException)
                {
                    Assert.Fail("No exception should have been thrown");
                }
            }
        }

        [Test]
        public void ManualEnlistment()
        {
            using (TransactionScope ts = new TransactionScope())
            {
                string connStr = GetConnectionString(true) + ";auto enlist=false";
                MySqlConnection c = new MySqlConnection(connStr);
                c.Open();

                MySqlCommand cmd = new MySqlCommand("INSERT INTO test VALUES ('a', 'name', 'name2')", c);
                cmd.ExecuteNonQuery();
            }
            MySqlCommand cmd2 = new MySqlCommand("SELECT COUNT(*) FROM test", conn);
            Assert.AreEqual(1, cmd2.ExecuteScalar());
        }

        private void ManuallyEnlistingInitialConnection(bool complete)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                string connStr = GetConnectionStringBasic(true) + ";auto enlist=false";

                using (MySqlConnection c1 = new MySqlConnection(connStr))
                {
                    c1.Open();
                    c1.EnlistTransaction(Transaction.Current);
                    MySqlCommand cmd1 = new MySqlCommand("INSERT INTO test (key2) VALUES ('a')", c1);
                    cmd1.ExecuteNonQuery();
                }

                using (MySqlConnection c2 = new MySqlConnection(connStr))
                {
                    c2.Open();
                    c2.EnlistTransaction(Transaction.Current);
                    MySqlCommand cmd2 = new MySqlCommand("INSERT INTO test (key2) VALUES ('b')", c2);
                    cmd2.ExecuteNonQuery();
                }
                if (complete)
                    ts.Complete();
            }
        }

        [Test]
        public void ManuallyEnlistingInitialConnection()
        {
            ManuallyEnlistingInitialConnection(true);
            ManuallyEnlistingInitialConnection(false);
        }

        [Test]
        public void ManualEnlistmentWithActiveConnection()
        {
            using (TransactionScope ts = new TransactionScope())
            {
                string connStr = GetConnectionStringBasic(true);

                using (MySqlConnection c1 = new MySqlConnection(connStr))
                {
                    c1.Open();

                    connStr += "; auto enlist=false";
                    using (MySqlConnection c2 = new MySqlConnection(connStr))
                    {
                        c2.Open();
                        try
                        {
                            c2.EnlistTransaction(Transaction.Current);
                        }
                        catch (NotSupportedException)
                        {
                        }
                    }
                }
            }
        }

        [Test]
        public void AttemptToEnlistTwoConnections()
        {
            using (TransactionScope ts = new TransactionScope())
            {
                string connStr = GetConnectionStringBasic(true);

                using (MySqlConnection c1 = new MySqlConnection(connStr))
                {
                    c1.Open();

                    using (MySqlConnection c2 = new MySqlConnection(connStr))
                    {
                        try
                        {
                            c2.Open();
                        }
                        catch (NotSupportedException)
                        {
                        }
                    }
                }
            }
        }

        private void ReusingSameConnection(bool pooling, bool complete)
        {
            execSQL("TRUNCATE TABLE test");
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.MaxValue))
            {
                string connStr = GetConnectionStringBasic(true);
                if (!pooling)
                    connStr += ";pooling=false";

                using (MySqlConnection c1 = new MySqlConnection(connStr))
                {
                    c1.Open();
                    MySqlCommand cmd1 = new MySqlCommand("INSERT INTO test (key2) VALUES ('a')", c1);
                    cmd1.ExecuteNonQuery();
                }

                using (MySqlConnection c2 = new MySqlConnection(connStr))
                {
                    c2.Open();
                    MySqlCommand cmd2 = new MySqlCommand("INSERT INTO test (key2) VALUES ('b')", c2);
                    cmd2.ExecuteNonQuery();
                }

                try
                {
                    if (complete)
                        ts.Complete();
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
            }

            MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM test", conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (complete)
            {
                Assert.AreEqual(2, dt.Rows.Count);
                Assert.AreEqual("a", dt.Rows[0][0]);
                Assert.AreEqual("b", dt.Rows[1][0]);
            }
            else
            {
                Assert.AreEqual(0, dt.Rows.Count);
            }
        }

        [Test]
        public void ReusingSameConnection()
        {
            int processes = CountProcesses();

            ReusingSameConnection(true, true);
            Assert.AreEqual(processes + 1, CountProcesses());

            ReusingSameConnection(true, false);
            Assert.AreEqual(processes + 1, CountProcesses());

            ReusingSameConnection(false, true);
            Assert.AreEqual(processes + 1, CountProcesses());

            ReusingSameConnection(false, false);
            Assert.AreEqual(processes + 1, CountProcesses());
        }

#endif

    }
}
