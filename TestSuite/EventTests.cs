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
using System.Globalization;
using System.Threading;
using NUnit.Framework;

namespace MySql.Data.MySqlClient.Tests
{
	[TestFixture]
	public class EventTests : BaseTest
	{
		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			Open();

			execSQL("DROP TABLE IF EXISTS Test");
			execSQL("CREATE TABLE Test (id INT NOT NULL, name VARCHAR(250), PRIMARY KEY(id))");
		}

		[TestFixtureTearDown]
		public void FixtureTeardown()
		{
			Close();
		}

		[Test]
		[Category("4.1")]
		public void Warnings()
		{
			conn.InfoMessage += new MySqlInfoMessageEventHandler(WarningsInfoMessage);

			MySqlCommand cmd = new MySqlCommand("SELECT * FROM mysql.host WHERE TIME('2005/6/21')", conn);
			MySqlDataReader reader = null;

			try 
			{
				reader = cmd.ExecuteReader();
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

		private void WarningsInfoMessage(object sender, MySqlInfoMessageEventArgs args)
		{
			Assert.AreEqual(1, args.errors.Length);
		}
		
		[Test]
		public void StateChange() 
		{
			MySqlConnection c = new MySqlConnection(GetConnectionString(true));
			c.StateChange += new StateChangeEventHandler(StateChangeHandler);
			c.Open();
			c.Close();
		}

		private void StateChangeHandler(object sender, StateChangeEventArgs e)
		{
		}		
	}
}
