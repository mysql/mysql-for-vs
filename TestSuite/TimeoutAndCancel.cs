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
using System.Data;
using System.IO;
using NUnit.Framework;
using System.Threading;

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

        [Category("5.0")]
        [Test]
        [Category("NotWorking")]
        public void CancelSingleQuery()
        {
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

        [Category("5.0")]
        [Test]
        [Category("NotWorking")]
        public void TimeoutExpiring()
        {
            // first we need a routine that will run for a bit
            execSQL("CREATE PROCEDURE spTest() BEGIN SET @start=NOW()+0; REPEAT SET @end=NOW()-@start; " +
                "UNTIL @end >= 600 END REPEAT; SELECT @start, @end; END");

            DateTime start = DateTime.Now;
            try
            {
                MySqlCommand cmd = new MySqlCommand("spTest", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 15;
                cmd.ExecuteNonQuery();
                Assert.Fail("Should not get to this point");
            }
            catch (MySqlException ex)
            {
                TimeSpan ts = DateTime.Now.Subtract(start);
                Assert.IsTrue(ex.Message.StartsWith("Timeout expired"));
                Assert.IsTrue(ts.TotalSeconds < 20);
            }
        }

        [Category("5.0")]
        [Test]
        public void TimeoutNotExpiring()
        {
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
