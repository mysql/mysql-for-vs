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
        void TransactionScopeInternal(bool commit)
        {
            createTable("CREATE TABLE Test (key2 VARCHAR(1), name VARCHAR(100), name2 VARCHAR(100))", "INNODB");
            using (MySqlConnection c = new MySqlConnection(GetConnectionString(true)))
            {
                MySqlCommand cmd = new MySqlCommand("INSERT INTO Test VALUES ('a', 'name', 'name2')", c);

                using (TransactionScope ts = new TransactionScope())
                {
                    c.Open();

                    cmd.ExecuteNonQuery();

                    if (commit)
                        ts.Complete();
                }

                cmd.CommandText = "SELECT COUNT(*) FROM Test";
                object count = cmd.ExecuteScalar();
                Assert.AreEqual(commit ? 1 : 0, count);
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

        // The following block is not currently supported
/*        void TransactionScopeMultipleInternal(bool commit)
        {
            MySqlConnection c1 = new MySqlConnection(GetConnectionString(true));
            MySqlConnection c2 = new MySqlConnection(GetConnectionString(true));
            MySqlCommand cmd1 = new MySqlCommand("INSERT INTO Test VALUES ('a', 'name', 'name2')", c1);
            MySqlCommand cmd2 = new MySqlCommand("INSERT INTO Test VALUES ('b', 'name', 'name2')", c1);

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

                cmd1.CommandText = "SELECT COUNT(*) FROM Test";
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
*/
        /// <summary>
        /// Bug #34448 Connector .Net 5.2.0 with Transactionscope doesn´t use specified IsolationLevel 
        /// </summary>
        [Test]
        public void TransactionScopeWithIsolationLevel()
        {
            TransactionOptions opts = new TransactionOptions();
            opts.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, opts))
            {
                string connStr = GetConnectionString(true);
                using (MySqlConnection myconn = new MySqlConnection(connStr))
                {
                    myconn.Open();
                    MySqlCommand cmd = new MySqlCommand("SHOW VARIABLES LIKE 'tx_isolation'", myconn);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        string level = reader.GetString(1);
                        Assert.AreEqual("READ-COMMITTED", level);
                    }
                }
            }
        }

        /// <summary>
        /// Bug #27289 Transaction is not rolledback when connection close 
        /// </summary>
        [Test]
        public void RollingBackOnClose()
        {
            execSQL("CREATE TABLE Test (id INT) ENGINE=InnoDB");

            string connStr = GetPoolingConnectionString();
            using (MySqlConnection c = new MySqlConnection(connStr))
            {
                c.Open();
                MySqlCommand cmd = new MySqlCommand("INSERT INTO Test VALUES (1)", c);
                c.BeginTransaction();
                cmd.ExecuteNonQuery();
            }

            using (MySqlConnection c2 = new MySqlConnection(connStr))
            {
                c2.Open();
                MySqlCommand cmd2 = new MySqlCommand("SELECT COUNT(*) from Test", c2);
                c2.BeginTransaction();
                object count = cmd2.ExecuteScalar();
                Assert.AreEqual(0, count);
            }

            MySqlConnection connection = new MySqlConnection(connStr);
            connection.Open();
            KillConnection(connection);
        }


        /// <summary>
        /// Bug #22042 mysql-connector-net-5.0.0-alpha BeginTransaction 
        /// </summary>
        [Test]
        public void Bug22042()
        {
            DbProviderFactory factory =
                new MySql.Data.MySqlClient.MySqlClientFactory();
            using (DbConnection conexion = factory.CreateConnection())
            {
                conexion.ConnectionString = GetConnectionString(true);
                conexion.Open();
                DbTransaction trans = conexion.BeginTransaction();
                trans.Rollback();
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
                cmd.Connection.EnlistTransaction(Transaction.Current);
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
                cmd.Connection.EnlistTransaction(Transaction.Current);
            }
        }

        [Test]
        public void ManualEnlistment()
        {
            createTable("CREATE TABLE Test (key2 VARCHAR(1), name VARCHAR(100), name2 VARCHAR(100))", "INNODB");

            using (TransactionScope ts = new TransactionScope())
            {
                string connStr = GetConnectionString(true) + ";auto enlist=false";
                using (MySqlConnection c = new MySqlConnection(connStr))
                {
                    c.Open();
                    MySqlCommand cmd = new MySqlCommand("INSERT INTO Test VALUES ('a', 'name', 'name2')", c);
                    cmd.ExecuteNonQuery();
                }
            }
            MySqlCommand cmd2 = new MySqlCommand("SELECT COUNT(*) FROM Test", conn);
            Assert.AreEqual(1, cmd2.ExecuteScalar());

        }

        private void ManuallyEnlistingInitialConnection(bool complete)
        {
            createTable("CREATE TABLE Test (key2 VARCHAR(1), name VARCHAR(100), name2 VARCHAR(100))", "INNODB");
            string connStr = GetConnectionString(true) + ";auto enlist=false";

            using (TransactionScope ts = new TransactionScope())
            {
                using (MySqlConnection c1 = new MySqlConnection(connStr))
                {
                    c1.Open();
                    c1.EnlistTransaction(Transaction.Current);
                    MySqlCommand cmd1 = new MySqlCommand("INSERT INTO Test (key2) VALUES ('a')", c1);
                    cmd1.ExecuteNonQuery();
                }

                using (MySqlConnection c2 = new MySqlConnection(connStr))
                {
                    c2.Open();
                    c2.EnlistTransaction(Transaction.Current);
                    MySqlCommand cmd2 = new MySqlCommand("INSERT INTO Test (key2) VALUES ('b')", c2);
                    cmd2.ExecuteNonQuery();
                }
                if (complete)
                    ts.Complete();
            }

            KillPooledConnection(connStr);
        }

        [Test]
        public void ManuallyEnlistingInitialConnection()
        {
            ManuallyEnlistingInitialConnection(true);
        }

        [Test]
        public void ManuallyEnlistingInitialConnectionNoComplete()
        {
            ManuallyEnlistingInitialConnection(false);
        }

        [Test]
        public void ManualEnlistmentWithActiveConnection()
        {
            using (TransactionScope ts = new TransactionScope())
            {
                string connStr = GetConnectionString(true);

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
                string connStr = GetConnectionString(true);

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


        private void NestedScopeInternalTest(
            TransactionScopeOption nestedOption,
            bool innerComplete,
            bool outerComplete,
            bool expectInnerChangesVisible,
            bool expectOuterChangesVisible)
        {
            createTable("CREATE TABLE T(str varchar(10))", "INNODB");
            try
            {
                using (TransactionScope outer = new TransactionScope())
                {
                    string connStr = GetConnectionString(true);
                    using (MySqlConnection c1 = new MySqlConnection(connStr))
                    {
                        c1.Open();
                        MySqlCommand cmd1 = new MySqlCommand("INSERT INTO T VALUES ('outer')", c1);
                        cmd1.ExecuteNonQuery();
                        using (TransactionScope inner = new TransactionScope(nestedOption))
                        {
                          
                            MySqlConnection c2;
                            if (nestedOption == TransactionScopeOption.Required)
                            {
                                // inner scope joins already running ambient
                                // transaction, we cannot use new connection here
                                c2 = c1;
                            }
                            else
                            {
                                // when TransactionScopeOption.RequiresNew or 
                                // new TransactionScopeOption.Suppress is used,
                                // we have to use a new transaction. We create a
                                // new connection for it.
                                c2 = new MySqlConnection(connStr);
                                c2.Open();
                            }

                            MySqlCommand cmd2 =
                                    new MySqlCommand("INSERT INTO T VALUES ('inner')", c2);
                            cmd2.ExecuteNonQuery();

                            if (innerComplete)
                                inner.Complete();

                            // Dispose connection if it was created.
                            if (c2 != c1)
                                c2.Dispose();
                        }
                    }
                    if (outerComplete)
                        outer.Complete();

                }
                bool innerChangesVisible =
                   ((long)MySqlHelper.ExecuteScalar(conn, "select count(*) from T where str = 'inner'") == 1);
                bool outerChangesVisible =
                    ((long)MySqlHelper.ExecuteScalar(conn, "select count(*) from T where str = 'outer'") == 1);
                Assert.AreEqual(innerChangesVisible, expectInnerChangesVisible);
                Assert.AreEqual(outerChangesVisible, expectOuterChangesVisible);
            }
            finally
            {
                MySqlHelper.ExecuteNonQuery(conn, "DROP TABLE T");
            }
        }

        /// <summary>
        /// Test inner/outer scope behavior, with different scope options, 
        /// completing either inner or outer scope, or both.
        /// </summary>
        [Test]
        public void NestedScope()
        {

            // inner scope joins the ambient scope, neither inner not outer  scope completes
            // Expect empty table.
            NestedScopeInternalTest(TransactionScopeOption.Required, false, false, false, false);

            // inner scope joins the ambient scope, inner does not complete, outer completes
            // Expect exception while disposing outer transaction
            try
            {
                NestedScopeInternalTest(TransactionScopeOption.Required, false, true, false, false);
                Assert.Fail("expected TransactionAborted exception");
            }
            catch (TransactionAbortedException)
            {
            }

            // inner scope joins the ambient scope, inner completes, outer does not
            // Expect empty table.
            NestedScopeInternalTest(TransactionScopeOption.Required, true, false, false, false);

            // inner scope joins the ambient scope, both complete.
            // Expect table with entries for inner and outer scope
            NestedScopeInternalTest(TransactionScopeOption.Required, true, true, true, true);



            // inner scope creates new transaction, neither inner not outer  scope completes
            // Expect empty table.
            NestedScopeInternalTest(TransactionScopeOption.RequiresNew, false, false, false, false);

            // inner scope creates new transaction, inner does not complete, outer completes
            // Expect changes by outer transaction visible ??
            NestedScopeInternalTest(TransactionScopeOption.RequiresNew, false, true, false, true);

            // inner scope creates new transactiion, inner completes, outer does not
            // Expect changes by inner transaction visible
            NestedScopeInternalTest(TransactionScopeOption.RequiresNew, true, false, true, false);

            // inner scope creates new transaction, both complete
            NestedScopeInternalTest(TransactionScopeOption.RequiresNew, true, true, true, true);


            // inner scope suppresses transaction, neither inner not outer  scope completes
            // Expect changes made by inner scope to be visible
            NestedScopeInternalTest(TransactionScopeOption.Suppress, false, false, true, false);

            // inner scope supresses transaction, inner does not complete, outer completes
            // Expect changes by inner scope to be visible ??
            NestedScopeInternalTest(TransactionScopeOption.Suppress,  true, false, true, false);

            // inner scope supresses transaction, inner completes, outer does not
            // Expect changes by inner transaction visible
            NestedScopeInternalTest(TransactionScopeOption.Suppress, true, false, true, false);

            // inner scope supresses transaction, both complete
            NestedScopeInternalTest(TransactionScopeOption.Suppress, true, true, true, true);
        }



        private void ReusingSameConnection(bool pooling, bool complete)
        {
            int c1Thread;
            execSQL("TRUNCATE TABLE Test");

            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.MaxValue))
            {
                string connStr = GetConnectionString(true);
                if (!pooling)
                    connStr += ";pooling=false";

                using (MySqlConnection c1 = new MySqlConnection(connStr))
                {
                    c1.Open();
                    MySqlCommand cmd1 = new MySqlCommand("INSERT INTO Test (key2) VALUES ('a')", c1);
                    cmd1.ExecuteNonQuery();
                    c1Thread = c1.ServerThread;
                }

                using (MySqlConnection c2 = new MySqlConnection(connStr))
                {
                    c2.Open();
                    MySqlCommand cmd2 = new MySqlCommand("INSERT INTO Test (key2) VALUES ('b')", c2);
                    cmd2.ExecuteNonQuery();
                    Assert.AreEqual(c1Thread, c2.ServerThread);
                }

                if (complete)
                    ts.Complete();
            }

            MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Test", conn);
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
            createTable("CREATE TABLE Test (key2 VARCHAR(1), name VARCHAR(100), name2 VARCHAR(100))", "INNODB");
            ReusingSameConnection(true, true);
//            Assert.AreEqual(processes + 1, CountProcesses());

            ReusingSameConnection(true, false);
  //          Assert.AreEqual(processes + 1, CountProcesses());

            ReusingSameConnection(false, true);
    //        Assert.AreEqual(processes + 1, CountProcesses());

            ReusingSameConnection(false, false);
      //      Assert.AreEqual(processes + 1, CountProcesses());
        }
    }
}
