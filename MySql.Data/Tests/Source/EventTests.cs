// Copyright (c) 2004-2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
//
// MySQL Connector/NET is licensed under the terms of the GPLv2
// <http://www.gnu.org/licenses/old-licenses/gpl-2.0.html>, like most 
// MySQL Connectors. There are special exceptions to the terms and 
// conditions of the GPLv2 as it is applied to this software, see the 
// FLOSS License Exception
// <http://www.mysql.com/about/legal/licensing/foss-exception.html>.
//
// This program is free software; you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published 
// by the Free Software Foundation; version 2 of the License.
//
// This program is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
//
// You should have received a copy of the GNU General Public License along 
// with this program; if not, write to the Free Software Foundation, Inc., 
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

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

            execSQL("CREATE TABLE Test (name VARCHAR(10))");

            string connStr = GetConnectionString(true);
            using (MySqlConnection c = new MySqlConnection(connStr))
            {
                c.Open();

                MySqlCommand cmd = new MySqlCommand("SET SQL_MODE=''", c);
                cmd.ExecuteNonQuery();

			    c.InfoMessage += new MySqlInfoMessageEventHandler(WarningsInfoMessage);

			    cmd.CommandText = "INSERT INTO Test VALUES ('12345678901')";
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                }
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
