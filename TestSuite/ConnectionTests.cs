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
using System.Configuration;

namespace MySql.Data.MySqlClient.Tests
{
	/// <summary>
	/// Summary description for ConnectionTests.
	/// </summary>
	[TestFixture] 
	public class ConnectionTests : BaseTest
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

		[Test()]
		public void TestConnectionStrings()
		{
			MySqlConnection c = new MySqlConnection();

			// public properties
			Assert.AreEqual( 15, c.ConnectionTimeout, "ConnectionTimeout" );
			Assert.AreEqual( "", c.Database, "Database" );
			Assert.AreEqual( String.Empty, c.DataSource, "DataSource" );
			Assert.AreEqual( false, c.UseCompression, "Use Compression" );
			Assert.AreEqual( System.Data.ConnectionState.Closed, c.State, "State" );

			c = new MySqlConnection("connection timeout=25; user id=myuser; " +
				"password=mypass; database=Test;server=myserver; use compression=true; " +
				"pooling=false;min pool size=5; max pool size=101");
			// public properties
			Assert.AreEqual( 25, c.ConnectionTimeout, "ConnectionTimeout" );
			Assert.AreEqual( "Test", c.Database, "Database" );
			Assert.AreEqual( "myserver", c.DataSource, "DataSource" );
			Assert.AreEqual( true, c.UseCompression, "Use Compression" );
			Assert.AreEqual( System.Data.ConnectionState.Closed, c.State, "State" );

			c.ConnectionString = "connection timeout=15; user id=newuser; " +
				"password=newpass; port=3308; database=mydb; data source=myserver2; " + 
				"use compression=true; pooling=true; min pool size=3; max pool size=76";

			// public properties
			Assert.AreEqual( 15, c.ConnectionTimeout, "ConnectionTimeout" );
			Assert.AreEqual( "mydb", c.Database, "Database" );
			Assert.AreEqual( "myserver2", c.DataSource, "DataSource" );
			Assert.AreEqual( true, c.UseCompression, "Use Compression" );
			Assert.AreEqual( System.Data.ConnectionState.Closed, c.State, "State" );
		}

		[Test()]
		[ExpectedException(typeof(MySqlException))]
		[Explicit]
		public void TestConnectingSocketBadUserName()
		{
			string host = ConfigurationSettings.AppSettings["host"];

			string connStr = "server={0};user id=dummy;password=;database=Test";
			MySqlConnection c = new MySqlConnection(
				String.Format(connStr, host));
			c.Open();
			c.Close();
		}

		[Test()]
		[ExpectedException(typeof(MySqlException))]
		public void TestConnectingSocketBadDbName()
		{
			string host = ConfigurationSettings.AppSettings["host"];
			string userid = ConfigurationSettings.AppSettings["userid"];
			string password = ConfigurationSettings.AppSettings["password"];

			string connStr = "server={0};user id={1};password={2};database=dummy; pooling=false";
			MySqlConnection c = new MySqlConnection(
				String.Format(connStr, host, userid, password));
			c.Open();
			c.Close();
		}

		[Test()]
		public void TestPersistSecurityInfoCachingPasswords() 
		{
			string host = ConfigurationSettings.AppSettings["host"];
			string uid = ConfigurationSettings.AppSettings["user"];
			string pwd = ConfigurationSettings.AppSettings["password"];

			string connStr = String.Format("database=test;server={0};user id={1};Password={2}; pooling=false",
				host, uid, pwd );
			MySqlConnection c = new MySqlConnection( connStr );
			c.Open();
			c.Close();

			// this shouldn't work
			connStr = String.Format("database=test;server={0};user id={1};Password={2}; pooling=false",
				host, uid, "bad_password" );
			c = new MySqlConnection( connStr );
			try 
			{
				c.Open();
				Assert.Fail("Thn is should not work");
				c.Close();
				return;
			}
			catch (MySqlException)
			{
			}

			// this should work
			connStr = String.Format("database=test;server={0};user id={1};Password={2}; pooling=false",
				host, uid, pwd );
			c = new MySqlConnection( connStr );
			c.Open();
			c.Close();
		}

		[Test()]
		public void ChangeDatabase() 
		{
			MySqlConnection c = new MySqlConnection( conn.ConnectionString + ";pooling=false" );
			c.Open();
			Assert.IsTrue(c.State == ConnectionState.Open);

			Assert.AreEqual( "test", c.Database.ToLower() );

			c.ChangeDatabase( "mysql" );

			Assert.AreEqual( "mysql", c.Database.ToLower() );

			c.Close();
		}

		[Test()]
		public void ConnectionTimeout() 
		{

			MySqlConnection c = new MySqlConnection( 
				"server=1.1.1.1;user id=bogus;pwd=bogus;Connection timeout=5;pooling=false" );
			DateTime start = DateTime.Now;
			try 
			{
				c.Open();
			}
			catch (Exception) 
			{
				TimeSpan diff = DateTime.Now.Subtract( start );
				Assert.IsTrue( diff.TotalSeconds < 6, "Timeout exceeded" );
			}
		}

		[Test]
		public void ConnectInVariousWays()
		{
			try 
			{
				string connStr = conn.ConnectionString;

				// connect with no db
				string connStr2 = connStr.Replace("database=test;","");
				MySqlConnection c = new MySqlConnection( connStr2 );
				c.Open();
				c.Close();

				// connect with all defaults
				if (connStr.IndexOf("localhost") != -1) 
				{
					c = new MySqlConnection( String.Empty );
					c.Open();
					c.Close();
				}

				// connect with no password
				string host = System.Configuration.ConfigurationSettings.AppSettings["host"];
				string user = System.Configuration.ConfigurationSettings.AppSettings["nopassuser"];
				connStr2 = "server=" + host + ";user id=" + user;
				c = new MySqlConnection( connStr2 );
				c.Open();
				c.Close();

				connStr2 += ";password=;";
				c = new MySqlConnection( connStr2 );
				c.Open();
				c.Close();
			}
			catch (Exception ex)
			{
				Assert.Fail( ex.Message );
			}
		}
		/// <summary>
		/// Bug #10281 Clone issue with MySqlConnection 
		/// </summary>
		[Test()]
		public void TestConnectionClone()
		{
			MySqlConnection c = new MySqlConnection();
			MySqlConnection clone = (MySqlConnection) ((ICloneable)c).Clone();
		}
	}
}
