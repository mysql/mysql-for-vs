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
using MySql.Data.MySqlClient;
using NUnit.Framework;

namespace MySql.Data.MySqlClient.Tests
{
	[TestFixture()]
	public class CommandTests : BaseTest
	{

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			Open();
			execSQL("DROP TABLE IF EXISTS Test");
			execSQL("CREATE TABLE Test (id int NOT NULL, name VARCHAR(100))");
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown() 
		{
			Close();
		}

		[Test()]
		public void InsertTest()
		{
			try 
			{
				// do the insert
				MySqlCommand cmd = new MySqlCommand("INSERT INTO Test (id,name) VALUES(10,'Test')", conn);
				int cnt = cmd.ExecuteNonQuery();
				Assert.AreEqual( 1, cnt, "Insert Count" );

				// make sure we get the right value back out
				cmd.CommandText = "SELECT name FROM Test WHERE id=10";
				string name = (string)cmd.ExecuteScalar();
				Assert.AreEqual( "Test", name, "Insert result" );

				// now do the insert with parameters
				cmd.CommandText = "INSERT INTO Test (id,name) VALUES(?id, ?name)";
				cmd.Parameters.Add( new MySqlParameter("?id", 11));
				cmd.Parameters.Add( new MySqlParameter("?name", "Test2"));
				cnt = cmd.ExecuteNonQuery();
				Assert.AreEqual( 1, cnt, "Insert with Parameters Count" );

				// make sure we get the right value back out
				cmd.Parameters.Clear();
				cmd.CommandText = "SELECT name FROM Test WHERE id=11";
				name = (string)cmd.ExecuteScalar();
				Assert.AreEqual( "Test2", name, "Insert with parameters result" );
			}
			catch (MySqlException ex)
			{
				Assert.Fail( ex.Message );
			}
		}

		[Test()]
		public void UpdateTest()
		{
			try 
			{
				execSQL("INSERT INTO Test (id,name) VALUES(10, 'Test')");
				execSQL("INSERT INTO Test (id,name) VALUES(11, 'Test2')");

				// do the update
				MySqlCommand cmd = new MySqlCommand("UPDATE Test SET name='Test3' WHERE id=10 OR id=11", conn);
				MySqlConnection c = cmd.Connection;
				Assert.AreEqual( conn, c );
				int cnt = cmd.ExecuteNonQuery();
				Assert.AreEqual( 2, cnt );

				// make sure we get the right value back out
				cmd.CommandText = "SELECT name FROM Test WHERE id=10";
				string name = (string)cmd.ExecuteScalar();
				Assert.AreEqual( "Test3", name );
			
				cmd.CommandText = "SELECT name FROM Test WHERE id=11";
				name = (string)cmd.ExecuteScalar();
				Assert.AreEqual( "Test3", name );

				// now do the update with parameters
				cmd.CommandText = "UPDATE Test SET name=?name WHERE id=?id";
				cmd.Parameters.Add( new MySqlParameter("?id", 11));
				cmd.Parameters.Add( new MySqlParameter("?name", "Test5"));
				cnt = cmd.ExecuteNonQuery();
				Assert.AreEqual( 1, cnt, "Update with Parameters Count" );

				// make sure we get the right value back out
				cmd.Parameters.Clear();
				cmd.CommandText = "SELECT name FROM Test WHERE id=11";
				name = (string)cmd.ExecuteScalar();
				Assert.AreEqual( "Test5", name );
			}
			catch (Exception ex)
			{
				Assert.Fail( ex.Message );
			}
		}

		[Test()]
		public void DeleteTest()
		{
			try 
			{
				execSQL("INSERT INTO Test (id, name) VALUES(1, 'Test')");
				execSQL("INSERT INTO Test (id, name) VALUES(2, 'Test2')");

				// make sure we get the right value back out
				MySqlCommand cmd = new MySqlCommand("DELETE FROM Test WHERE id=1 or id=2", conn);
				int delcnt = cmd.ExecuteNonQuery();
				Assert.AreEqual( 2, delcnt );
			
				// find out how many rows we have now
				cmd.CommandText = "SELECT COUNT(*) FROM Test";
				object after_cnt = cmd.ExecuteScalar();
				Assert.AreEqual( 0, after_cnt );
			}
			catch (Exception ex)
			{
				Assert.Fail( ex.Message );
			}
		}

		[Test]
		public void CtorTest() 
		{
			MySqlTransaction txn = conn.BeginTransaction();
			MySqlCommand cmd = new MySqlCommand("SELECT * FROM Test", conn);

			MySqlCommand clone = new MySqlCommand( cmd.CommandText, (MySqlConnection)cmd.Connection, 
				(MySqlTransaction)cmd.Transaction );
			clone.Parameters.Add( "?test", 1 );
			txn.Rollback();
		}

		[Test]
		public void CloneCommand() 
		{
			IDbCommand cmd = new MySqlCommand();
			IDbCommand cmd2 = ((ICloneable)cmd).Clone() as IDbCommand;
		}
	}
}
