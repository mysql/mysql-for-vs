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
using System.Diagnostics;
using System.Text;

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
            string connStr = GetConnectionString(true) + ";auto enlist=false";

            using (TransactionScope ts = new TransactionScope())
            {
                MySqlConnection c = new MySqlConnection(connStr);
                c.Open();

                MySqlCommand cmd = new MySqlCommand("INSERT INTO Test VALUES ('a', 'name', 'name2')", c);
                cmd.ExecuteNonQuery();
            }
            MySqlCommand cmd2 = new MySqlCommand("SELECT COUNT(*) FROM Test", conn);
            Assert.AreEqual(1, cmd2.ExecuteScalar());

            KillPooledConnection(connStr);
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

        /// <summary>
        /// Ensures that a commit after heavy ammount of inserts does not timeout.
        /// </summary>
        [Test]
        public void CommitDoesNotTimeout()
        {
            const int requiredNumberOfRuns = 1;
            const int binarySize = 5000000;
            const int requiredNumberOfRowsPerRun = 100;

            Debug.WriteLine("Required Number Of Runs :" + requiredNumberOfRuns);
            Debug.WriteLine("Required Number Of Rows Per Run :" + requiredNumberOfRowsPerRun);

            suExecSQL("SET GLOBAL max_allowed_packet=64000000");

            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();

            using (MySqlConnection connection = new MySqlConnection(GetConnectionString(true)))
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "DROP TABLE IF EXISTS test_timeout;";
                    command.ExecuteNonQuery();

                    StringBuilder sqlCommand = new StringBuilder(512);

                    sqlCommand.Append("CREATE TABLE test_timeout (");
                    sqlCommand.Append("identity INT NOT NULL auto_increment, ");
                    sqlCommand.Append("a INT NOT NULL, ");
                    sqlCommand.Append("b INT NOT NULL, ");
                    sqlCommand.Append("c INT NOT NULL, ");
                    sqlCommand.Append("binary_data LONGBLOB NOT NULL, ");
                    sqlCommand.Append("PRIMARY KEY(identity), ");
                    sqlCommand.Append("KEY `abc` (`a`,`b`, `c`) ");
                    sqlCommand.Append(") TYPE = INNODB");

                    command.CommandText = sqlCommand.ToString();
                    command.ExecuteNonQuery();
                }

                for (int numberOfRuns = 0; numberOfRuns < requiredNumberOfRuns; ++numberOfRuns)
                {
                    using (MySqlTransaction transaction = connection.BeginTransaction())
                    {
                        Stopwatch stopwatch = Stopwatch.StartNew();

                        using (MySqlCommand command = new MySqlCommand())
                        {
                            command.Connection = connection;
                            command.CommandText = "INSERT INTO test_timeout VALUES (?f1, ?f2, ?f3, ?f4, ?f5)";
                            command.Parameters.Add("?f1", MySqlDbType.Int32);
                            command.Parameters.Add("?f2", MySqlDbType.Int32);
                            command.Parameters.Add("?f3", MySqlDbType.Int32);
                            command.Parameters.Add("?f4", MySqlDbType.Int32);
                            command.Parameters.Add("?f5", MySqlDbType.LongBlob);
                            command.CommandTimeout = 0;
                            command.Prepare();


                            byte[] buffer;

                            using (MemoryStream stream = new MemoryStream())
                            {
                                using (BinaryWriter binary = new BinaryWriter(stream))
                                {
                                    int count = 0;

                                    while (stream.Position < binarySize)
                                    {
                                        binary.Write(++count);
                                    }
                                }

                                buffer = stream.ToArray();
                            }

                            for (int i = 0; i < requiredNumberOfRowsPerRun; ++i)
                            {
                                command.Parameters[1].Value = i;
                                command.Parameters[2].Value = i;
                                command.Parameters[3].Value = i;
                                command.Parameters[4].Value = buffer;
                                command.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();

                        Assert.IsNotNull(transaction);

                        stopwatch.Stop();

                        double seconds = stopwatch.Elapsed.TotalSeconds;
                        double recordsPerSecond = requiredNumberOfRowsPerRun / seconds;

                        StringBuilder sb = new StringBuilder();
                        sb.AppendFormat("Truncate Result : Insert {0} Took {1:F4}; Per Second {2:F1} ",
                        requiredNumberOfRowsPerRun, seconds, recordsPerSecond);

                        Debug.WriteLine(sb.ToString());
                    }

                    using (MySqlCommand command = new MySqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "SELECT * FROM test_timeout";

                        Stopwatch stopwatch = Stopwatch.StartNew();
                        int count = 0;

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            int previous = -1;

                            while (reader.Read())
                            {
                                int current = reader.GetInt32(0);
                                Assert.Greater(current, previous);
                                previous = current;

                                ++count;
                            }
                        }

                        stopwatch.Stop();

                        double seconds = stopwatch.Elapsed.TotalSeconds;
                        double recordsPerSecond = count / seconds;

                        StringBuilder sb = new StringBuilder();
                        sb.AppendFormat("Test Result : Select {0} Took {1:F4}; Per Second {2:F1} ",
                        count, seconds, recordsPerSecond);

                        Debug.WriteLine(sb.ToString());
                    }

                    using (MySqlCommand command = new MySqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "TRUNCATE TABLE test_timeout";
                        command.ExecuteNonQuery();
                    }
                }

                MySqlConnection.ClearPool(connection);
            }
        }
    }
}
