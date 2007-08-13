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
	public class ExceptionTests : BaseTest
	{
        protected override void Setup()
        {
            base.Setup();
            execSQL("DROP TABLE IF EXISTS Test");
            execSQL("CREATE TABLE Test (id INT NOT NULL, name VARCHAR(100))");
        }

		[Test]
		public void Timeout() 
		{
			for (int i=1; i < 10; i++)
				execSQL("INSERT INTO Test VALUES (" + i + ", 'This is a long text string that I am inserting')");

			// we create a new connection so our base one is not closed
			MySqlConnection c2 = new MySqlConnection(conn.ConnectionString);
			c2.Open();

			KillConnection(c2);
			MySqlCommand cmd = new MySqlCommand("SELECT * FROM Test", c2);
			MySqlDataReader reader = null;

			try 
			{
				reader = cmd.ExecuteReader();
				reader.Read();
				reader.Read();
				reader.Close();
				Assert.Fail("We should not reach this code");
			}
			catch (Exception)
			{
				Assert.AreEqual(ConnectionState.Closed, c2.State);
			}
			finally 
			{
				if (reader != null) reader.Close();
				c2.Close();
			}
		}
	}
}
