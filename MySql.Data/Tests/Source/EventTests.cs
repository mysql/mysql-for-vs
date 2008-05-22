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
using System.Globalization;
using System.Threading;
using NUnit.Framework;

namespace MySql.Data.MySqlClient.Tests
{
	[TestFixture]
	public class EventTests : BaseTest
	{
		[Test]
		public void Warnings()
		{
            if (Version < new Version(4, 1)) return;

			conn.InfoMessage += new MySqlInfoMessageEventHandler(WarningsInfoMessage);

            execSQL("CREATE TABLE Test (name VARCHAR(10))");

			MySqlCommand cmd = new MySqlCommand("INSERT INTO Test VALUES ('12345678901')", conn);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
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
