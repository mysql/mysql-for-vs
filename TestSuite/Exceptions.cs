// Copyright (C) 2004 MySQL AB
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
	[TestFixture()]
	public class Exceptions : BaseTest
	{
		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			Open();

			execSQL("DROP TABLE IF EXISTS Test");
			execSQL("CREATE TABLE Test (id INT NOT NULL, name VARCHAR(100))");
		}

		[TestFixtureTearDown]
		public void FixtureTeardown()
		{
			Close();
		}


		[Test()]
		[Explicit]
		public void TimeoutDuringRead() 
		{
			execSQL( "SET @@global.wait_timeout=15" );
			execSQL( "SET @@local.wait_timeout=28800" );

			for (int i=1; i < 2000; i++)
				execSQL( "INSERT INTO Test VALUES (" + i + ", 'This is a long text string that I am inserting')" );

			MySqlConnection c2 = new MySqlConnection( conn.ConnectionString );
			c2.Open();

			MySqlCommand cmd = new MySqlCommand( "SELECT * FROM Test", c2 );
			MySqlDataReader reader = null;

			try 
			{
				reader = cmd.ExecuteReader();
				Thread.Sleep( 20000 );
				while (reader.Read()) 
				{
				}
				reader.Close();
				Assert.Fail("We should not reach this code");
			}
			catch (MySqlException ex)
			{
				Assert.IsTrue( ex.IsFatal );
				Assert.AreEqual( ConnectionState.Closed, c2.State );
			}
			finally 
			{
				if (reader != null) reader.Close();
			}

			execSQL( "SET @@global.wait_timeout=28800" );
		}
	}
}
