// Copyright (C) 2004-2005 MySQL AB
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
using System.Threading;
using MySql.Data.MySqlClient;
using NUnit.Framework;

namespace MySql.Data.MySqlClient.Tests
{
	/// <summary>
	/// Summary description for PoolingTests.
	/// </summary>
	[TestFixture]
	public class PoolingTests : BaseTest
	{
		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			Open();
		}

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			Close();
		}

		[Test]
		public void Connection() 
		{
			string connStr = conn.ConnectionString + ";pooling=true";

			MySqlConnection c = new MySqlConnection( connStr );
			c.Open();
			int serverThread = c.ServerThread;
			c.Close();
			
			// first test that only a single connection get's used
			for (int i=0; i < 10; i++) 
			{
				c = new MySqlConnection( connStr );
				c.Open();
				Assert.AreEqual( serverThread, c.ServerThread );
				c.Close();
			}

			c.Open();
			KillConnection(c);
			c.Close();

			connStr += ";Min Pool Size=10";
			MySqlConnection[] connArray = new MySqlConnection[10];
			for (int i=0; i < connArray.Length; i++) 
			{
				connArray[i] = new MySqlConnection( connStr );
				connArray[i].Open();
			}

			// now make sure all the server ids are different
			for (int i=0; i < connArray.Length; i++) 
			{
				for (int j=0; j < connArray.Length; j++)
				{
					if (i != j)
						Assert.IsTrue( connArray[i].ServerThread != connArray[j].ServerThread );
				}
			}

			for (int i=0; i < connArray.Length; i++)
			{
				int id = connArray[i].ServerThread;
				KillConnection(connArray[i]);
				connArray[i].Close();
			}
		}

		[Test]
		public void OpenKilled()
		{
			try 
			{
				string connStr = conn.ConnectionString + ";pooling=true";
				MySqlConnection c = new MySqlConnection(connStr);
				c.Open();
				int threadId = c.ServerThread;
				// thread gets killed right here
				KillConnection(c);
				c.Close();

				c.Dispose();

				c = new MySqlConnection(connStr);
				c.Open();
				int secondThreadId = c.ServerThread;
				KillConnection(c);
				c.Close();
				Assert.IsFalse(threadId == secondThreadId);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}
		}


		[Test]
		public void ReclaimBrokenConnection() 
		{
			// now create a new connection string only allowing 1 connection in the pool
			string connStr = conn.ConnectionString + ";max pool size=1";

			// now use up that connection
			MySqlConnection c = new MySqlConnection( connStr );
			c.Open();

			// now attempting to open a connection should fail
			try 
			{
				MySqlConnection c2 = new MySqlConnection( connStr );
				c2.Open();
				Assert.Fail( "Open after using up pool should fail" );
			}
			catch (Exception) { }

			// we now kill the first connection
			execSQL("KILL " + c.ServerThread);

			// Opening a connection now should work
			try 
			{
				MySqlConnection c2 = new MySqlConnection( connStr );
				c2.Open();
				c2.Close();
			}
			catch (Exception ex) 
			{ 
				Assert.Fail( ex.Message);
			}
		}

		[Test()]
		public void TestUserReset()
		{
			execSQL("SET @testvar='5'");
			MySqlCommand cmd = new MySqlCommand("SELECT @testvar", conn);
			object var = cmd.ExecuteScalar();
			Assert.AreEqual("5", var );
			conn.Close();

			conn.Open();
			object var2 = cmd.ExecuteScalar();
			Assert.AreEqual( DBNull.Value, var2 );
			conn.Close();
		}

	}
}
