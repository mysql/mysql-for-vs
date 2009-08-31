// Copyright (c) 2004-2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
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
using System.Threading;
using NUnit.Framework;
using System.Globalization;

namespace MySql.Data.MySqlClient.Tests
{
	[TestFixture]
	public class TimeoutAndCancel : BaseTest
	{
        private delegate void CommandInvokerDelegate(MySqlCommand cmdToRun);

        private void CommandRunner(MySqlCommand cmdToRun)
        {
            object o = cmdToRun.ExecuteScalar();
            Assert.IsNull(o);
        }

        [Test]
        public void CancelSingleQuery()
        {
            if (Version < new Version(5, 0)) return;

            // first we need a routine that will run for a bit
            execSQL(@"CREATE PROCEDURE spTest(duration INT) 
                BEGIN 
                    SELECT SLEEP(duration);
                END");

            MySqlCommand cmd = new MySqlCommand("spTest", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("duration", 60);

            // now we start execution of the command
            CommandInvokerDelegate d = new CommandInvokerDelegate(CommandRunner);
            d.BeginInvoke(cmd, null, null);

            // sleep 5 seconds
            Thread.Sleep(5000);

            // now cancel the command
            cmd.Cancel();
        }

        int stateChangeCount;
        [Test]
        public void WaitTimeoutExpiring()
        {
            using (MySqlConnection c = new MySqlConnection(GetConnectionString(true)))
            {
                c.Open();
                c.StateChange += new StateChangeEventHandler(c_StateChange);

                // set the session wait timeout on this new connection
                MySqlCommand cmd = new MySqlCommand("SET SESSION interactive_timeout=10", c);
                cmd.ExecuteNonQuery();
                cmd.CommandText = "SET SESSION wait_timeout=10";
                cmd.ExecuteNonQuery();

                stateChangeCount = 0;
                // now wait 10 seconds
                Thread.Sleep(15000);

                try
                {
                    cmd.CommandText = "SELECT now()";
                    cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    Assert.IsTrue(ex.Message.StartsWith("Fatal"));
                }

                Assert.AreEqual(1, stateChangeCount);
                Assert.AreEqual(ConnectionState.Closed, c.State);
            }

            using (MySqlConnection c = new MySqlConnection(GetConnectionString(true)))
            {
                c.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT now() as thetime, database() as db", c);
                using (MySqlDataReader r = cmd.ExecuteReader())
                {
                    Assert.IsTrue(r.Read());
                }
            }
        }

        void c_StateChange(object sender, StateChangeEventArgs e)
        {
            stateChangeCount++;
        }

        [Test]
        public void TimeoutExpiring()
        {
            if (version < new Version(5, 0)) return;

            // first we need a routine that will run for a bit
            execSQL(@"CREATE PROCEDURE spTest(duration INT) 
                BEGIN 
                    SELECT SLEEP(duration);
                END");

            DateTime start = DateTime.Now;
            try
            {
                MySqlCommand cmd = new MySqlCommand("spTest", conn);
                cmd.Parameters.AddWithValue("duration", 60);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 5;
                cmd.ExecuteNonQuery();
                Assert.Fail("Should not get to this point");
            }
            catch (MySqlException ex)
            {
                TimeSpan ts = DateTime.Now.Subtract(start);
                Assert.IsTrue(ts.TotalSeconds <= 10);
                Assert.IsTrue(ex.Message.StartsWith("Timeout expired"), "Message is wrong");
            }
        }

        [Test]
        public void TimeoutNotExpiring()
        {
            if (Version < new Version(5, 0)) return;

            // first we need a routine that will run for a bit
            execSQL(@"CREATE PROCEDURE spTest(duration INT) 
                BEGIN 
                    SELECT SLEEP(duration);
                END");

            MySqlCommand cmd = new MySqlCommand("spTest", conn);
            cmd.Parameters.AddWithValue("duration", 10);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 15;
            cmd.ExecuteNonQuery();
        }

        [Test]
        public void TimeoutDuringBatch()
        {
            if (Version < new Version(5, 0)) return;

            execSQL(@"CREATE PROCEDURE spTest(duration INT) 
                BEGIN 
                    SELECT SLEEP(duration);
                END");

            execSQL("CREATE TABLE test (id INT)");

            MySqlCommand cmd = new MySqlCommand(
                "call spTest(60);INSERT INTO test VALUES(4)", conn);
            cmd.CommandTimeout = 5;
            try
            {
                cmd.ExecuteNonQuery();
                Assert.Fail("This should have timed out");
            }
            catch (MySqlException ex)
            {
                Assert.IsTrue(ex.Message.StartsWith("Timeout expired"), "Message is wrong");
            }

            cmd.CommandText = "SELECT COUNT(*) FROM test";
            Assert.AreEqual(0, cmd.ExecuteScalar());
        }
        
        [Test]
        public void CancelSelect()
        {
            if (Version < new Version(5, 0)) return;

            execSQL("CREATE TABLE Test (id INT AUTO_INCREMENT PRIMARY KEY, name VARCHAR(20))");
            for (int i=0; i < 10000; i++)
                execSQL("INSERT INTO Test VALUES (NULL, 'my string')");

            MySqlCommand cmd = new MySqlCommand("SELECT * FROM Test", conn);
            cmd.CommandTimeout = 0;
            int rows = 0;
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                reader.Read();

                cmd.Cancel();

                while (reader.Read())
                {
                    rows++;
                }
            }
            Assert.IsTrue(rows < 10000);
        }        

        /// <summary>
        /// Bug #40091	mysql driver 5.2.3.0 connection pooling issue
        /// </summary>
        [Test]
        public void ConnectionStringModifiedAfterCancel()
        {
            if (Version.Major < 5) return;

            string connStr = GetPoolingConnectionString();
            connStr = connStr.Replace("persist security info=true", "persist security info=false");

            using (MySqlConnection c = new MySqlConnection(connStr))
            {
                c.Open();
                string connStr1 = c.ConnectionString;

                MySqlCommand cmd = new MySqlCommand("SELECT SLEEP(10)", c);
                cmd.CommandTimeout = 5;

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    string connStr2 = c.ConnectionString.ToLower(CultureInfo.InvariantCulture);
                    Assert.AreEqual(connStr1, connStr2);
                    reader.Read();
                }
            }
            MySqlConnection c1 = new MySqlConnection(connStr);
            c1.Open();
            KillConnection(c1);
        }
    }
}
