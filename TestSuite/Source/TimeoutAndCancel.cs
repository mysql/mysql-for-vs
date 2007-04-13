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
using System.Threading;
using NUnit.Framework;

namespace MySql.Data.MySqlClient.Tests
{
	[TestFixture]
	public class TimeoutAndCancel : BaseTest
	{
		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			Open();
		}

		[TestFixtureTearDown]
		public void FixtureTeardown()
		{
			Close();
		}

        private delegate void CommandInvokerDelegate(MySqlCommand cmdToRun);

        private void CommandRunner(MySqlCommand cmdToRun)
        {
            try
            {
                object o = cmdToRun.ExecuteScalar();
                Assert.IsNull(o);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test]
        public void CancelSingleQuery()
        {
            if (version < new Version(5, 0)) return;

            // first we need a routine that will run for a bit
            execSQL("CREATE PROCEDURE spTest() BEGIN SET @start=NOW()+0; REPEAT SET @end=NOW()-@start; " +
                "UNTIL @end >= 5000 END REPEAT; SELECT @start, @end; END");

            MySqlCommand cmd = new MySqlCommand("spTest", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // now we start execution of the command
            CommandInvokerDelegate d = new CommandInvokerDelegate(CommandRunner);
            IAsyncResult iar = d.BeginInvoke(cmd, null, null);

            // sleep 5 seconds
            Thread.Sleep(5000);

            // now cancel the command
            cmd.Cancel();
        }

        int stateChangeCount;
        [Test]
        public void WaitTimeoutExpiring()
        {
            MySqlConnection c = new MySqlConnection(GetConnectionString(true));
            c.Open();
            c.StateChange += new StateChangeEventHandler(c_StateChange);

            // set the session wait timeout on this new connection
            MySqlCommand cmd = new MySqlCommand("SET SESSION interactive_timeout=10", c);
            cmd.ExecuteNonQuery();
            cmd.CommandText = "SET SESSION wait_timeout=10";
            cmd.ExecuteNonQuery();

            stateChangeCount = 0;
            // now wait 10 seconds
            System.Threading.Thread.Sleep(15000);

            try
            {
                cmd.CommandText = "SELECT now()";
                object date = cmd.ExecuteScalar();
            }
            catch (Exception) { }
            Assert.AreEqual(1, stateChangeCount);
            Assert.AreEqual(ConnectionState.Closed, c.State);

            c = new MySqlConnection(GetConnectionString(true));
            c.Open();
            cmd = new MySqlCommand("SELECT now() as thetime, database() as db", c);
            using (MySqlDataReader r = cmd.ExecuteReader())
            {
                Assert.IsTrue(r.Read());
            }
        }

        void c_StateChange(object sender, StateChangeEventArgs e)
        {
            stateChangeCount++;
        }

/*        [Test]
        public void TimeoutExpiring()
        {
            if (version < new Version(5, 0)) return;

            // first we need a routine that will run for a bit
            execSQL("CREATE PROCEDURE spTest() BEGIN SET @start=UNIX_TIMESTAMP(NOW()); " +
                "REPEAT SET @end=UNIX_TIMESTAMP(NOW())-@start; " +
                "UNTIL @end >= 60 END REPEAT; SELECT @start, @end; END");

            DateTime start = DateTime.Now;
            try
            {
                MySqlCommand cmd = new MySqlCommand("spTest", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 10;
                cmd.ExecuteNonQuery();
                Assert.Fail("Should not get to this point");
            }
            catch (MySqlException ex)
            {
                TimeSpan ts = DateTime.Now.Subtract(start);
                Assert.IsTrue(ex.Message.StartsWith("Timeout expired"), "Message is wrong");
                Assert.IsTrue(ts.TotalSeconds < 60, "Took too much time");
            }
        }
        */

        [Test]
        public void TimeoutNotExpiring()
        {
            if (version < new Version(5, 0)) return;

            // first we need a routine that will run for a bit
            execSQL("CREATE PROCEDURE spTest() BEGIN SET @start=NOW()+0; REPEAT SET @end=NOW()-@start; " +
                "UNTIL @end >= 5 END REPEAT; SELECT @start, @end; END");

            try
            {
                MySqlCommand cmd = new MySqlCommand("spTest", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 15;
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
