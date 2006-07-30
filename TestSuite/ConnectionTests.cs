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

        [Test]
        public void ConnectionStringBuilder()
        {
            MySqlConnectionStringBuilder sb = null;
            try
            {
                sb = new MySqlConnectionStringBuilder();
                sb.ConnectionString = "server=localhost;uid=reggie;pwd=pass;port=1111;" +
                    "connection timeout=23; pooling=true; min pool size=33; " +
                    "max pool size=66";
            }
            catch (ArgumentException ex)
            {
                Assert.Fail(ex.Message);
            }
            Assert.AreEqual("localhost", sb.Server);
            Assert.AreEqual("reggie", sb.UserID);
            Assert.AreEqual("pass", sb.Password);
            Assert.AreEqual(1111, sb.Port);
            Assert.AreEqual(23, sb.ConnectionTimeout);
            Assert.IsTrue(sb.Pooling);
            Assert.AreEqual(33, sb.MinimumPoolSize);
            Assert.AreEqual(66, sb.MaximumPoolSize);
            string s = sb.ConnectionString;

            try
            {
                sb.ConnectionString = "server=localhost;badkey=badvalue";
                Assert.Fail("This should not work");
            }
            catch (ArgumentException)
            {
            }
            catch (Exception)
            {
                Assert.Fail("Wrong exception type");
            }

            sb.Clear();
            Assert.AreEqual(15, sb.ConnectionTimeout);
            Assert.AreEqual(true, sb.Pooling);
            Assert.AreEqual(3306, sb.Port);
            Assert.AreEqual(String.Empty, sb.Server);
            Assert.AreEqual(false, sb.PersistSecurityInfo);
            Assert.AreEqual(0, sb.ConnectionLifeTime);
            Assert.IsFalse(sb.ConnectionReset);
            Assert.AreEqual(0, sb.MinimumPoolSize);
            Assert.AreEqual(100, sb.MaximumPoolSize);
            Assert.AreEqual(String.Empty, sb.UserID);
            Assert.AreEqual(String.Empty, sb.Password);
            Assert.AreEqual(false, sb.UseUsageAdvisor);
            Assert.AreEqual(String.Empty, sb.CharacterSet);
            Assert.AreEqual(false, sb.UseCompression);
            Assert.AreEqual("MYSQL", sb.PipeName);
            Assert.IsFalse(sb.Logging);
            Assert.IsFalse(sb.UseOldSyntax);
            Assert.IsTrue(sb.AllowBatch);
            Assert.IsFalse(sb.ConvertZeroDateTime);
            Assert.AreEqual("MYSQL", sb.SharedMemoryName);
            Assert.AreEqual(String.Empty, sb.Database);
            Assert.AreEqual(MySqlDriverType.Native, sb.DriverType);
            Assert.AreEqual(MySqlConnectionProtocol.Sockets, sb.ConnectionProtocol);
            Assert.IsFalse(sb.AllowZeroDateTime);
            Assert.IsFalse(sb.UsePerformanceMonitor);
            Assert.AreEqual(25, sb.ProcedureCacheSize);
        }

		[Test]
		public void TestConnectionStrings()
		{
			MySqlConnection c = new MySqlConnection();

			// public properties
			Assert.AreEqual(15, c.ConnectionTimeout, "ConnectionTimeout");
			Assert.AreEqual("", c.Database, "Database");
			Assert.AreEqual(String.Empty, c.DataSource, "DataSource");
			Assert.AreEqual(false, c.UseCompression, "Use Compression");
			Assert.AreEqual(System.Data.ConnectionState.Closed, c.State, "State");

			c = new MySqlConnection("connection timeout=25; user id=myuser; " +
				"password=mypass; database=Test;server=myserver; use compression=true; " +
				"pooling=false;min pool size=5; max pool size=101");
			// public properties
			Assert.AreEqual(25, c.ConnectionTimeout, "ConnectionTimeout");
			Assert.AreEqual("Test", c.Database, "Database");
			Assert.AreEqual("myserver", c.DataSource, "DataSource");
			Assert.AreEqual(true, c.UseCompression, "Use Compression");
			Assert.AreEqual(System.Data.ConnectionState.Closed, c.State, "State");

			c.ConnectionString = "connection timeout=15; user id=newuser; " +
				"password=newpass; port=3308; database=mydb; data source=myserver2; " + 
				"use compression=true; pooling=true; min pool size=3; max pool size=76";

			// public properties
			Assert.AreEqual(15, c.ConnectionTimeout, "ConnectionTimeout");
			Assert.AreEqual("mydb", c.Database, "Database");
			Assert.AreEqual("myserver2", c.DataSource, "DataSource");
			Assert.AreEqual(true, c.UseCompression, "Use Compression");
			Assert.AreEqual(System.Data.ConnectionState.Closed, c.State, "State");
		}

		[Test]
		[ExpectedException(typeof(MySqlException))]
		public void TestConnectingSocketBadUserName()
		{
			execSQL("DELETE FROM mysql.user WHERE length(user) = 0");
			execSQL("FLUSH PRIVILEGES");

			string connStr = "server={0};user id=dummy;password=;database=Test;pooling=false";
			MySqlConnection c = new MySqlConnection(
				String.Format(connStr, host));
			c.Open();
			c.Close();
		}

		[Test]
		[ExpectedException(typeof(MySqlException))]
		public void TestConnectingSocketBadDbName()
		{
			string connStr = "server={0};user id={1};password={2};database=dummy; " +
				"pooling=false";
			MySqlConnection c = new MySqlConnection(
				String.Format(connStr, host, this.user, this.password));
			c.Open();
			c.Close();
		}

		[Test]
		public void TestPersistSecurityInfoCachingPasswords() 
		{
			string connStr = String.Format("database=test;server={0};user id={1};Password={2}; pooling=false",
				host, this.user, this.password );
			MySqlConnection c = new MySqlConnection( connStr );
			c.Open();
			c.Close();

			// this shouldn't work
			connStr = String.Format("database=test;server={0};user id={1};Password={2}; pooling=false",
				host, this.user, "bad_password" );
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
				host, this.user, this.password);
			c = new MySqlConnection( connStr );
			c.Open();
			c.Close();
		}

		[Test]
		public void ChangeDatabase() 
		{
            string connStr = GetConnectionString(true);
			MySqlConnection c = new MySqlConnection(connStr + ";pooling=false");
			c.Open();
			Assert.IsTrue(c.State == ConnectionState.Open);

			Assert.AreEqual("test", c.Database.ToLower());

			c.ChangeDatabase("mysql");

			Assert.AreEqual("mysql", c.Database.ToLower());

			c.Close();
		}

		[Test]
		public void ConnectionTimeout() 
		{
			MySqlConnection c = new MySqlConnection( 
				"server=1.1.1.1;user id=bogus;pwd=bogus;Connection timeout=5;" +
                "pooling=false");
			DateTime start = DateTime.Now;
			try 
			{
				c.Open();
			}
			catch (Exception) 
			{
				TimeSpan diff = DateTime.Now.Subtract(start);
				Assert.IsTrue(diff.TotalSeconds < 30, "Timeout exceeded");
			}
		}

		[Test]
		public void ConnectInVariousWays()
		{
			try 
			{
				string connStr = conn.ConnectionString;

				// connect with no db
				string connStr2 = GetConnectionString(false);
				MySqlConnection c = new MySqlConnection(connStr2);
				c.Open();
				c.Close();

                // TODO: make anonymous login work
                execSQL("GRANT ALL ON *.* to '' IDENTIFIED BY ''");

				// connect with all defaults
				if (connStr.IndexOf("localhost") != -1) 
				{
					c = new MySqlConnection(String.Empty);
					c.Open();
					c.Close();
				}

				execSQL("GRANT ALL ON *.* to 'nopass'@'localhost'");
				execSQL("FLUSH PRIVILEGES");

				// connect with no password
				connStr2 = "server=" + host + ";user id=nopass";
				c = new MySqlConnection( connStr2 );
				c.Open();
				c.Close();

				connStr2 += ";password=;";
				c = new MySqlConnection(connStr2);
				c.Open();
				c.Close();
			}
			catch (Exception ex)
			{
				Assert.Fail( ex.Message );
			}
			finally 
			{
				execSQL("DELETE FROM mysql.user WHERE length(user) = 0");
				execSQL("DELETE FROM mysql.user WHERE user='nopass'");
				execSQL("FLUSH PRIVILEGES");
			}
		}

		[Test]
		[Category("4.1")]
		public void ConnectingAsUTF8()
		{
			execSQL("CREATE Database IF NOT EXISTS test2 DEFAULT CHARACTER SET utf8");

			string connStr = String.Format("server={0};user id={1}; password={2}; database=test2;pooling=false;charset=utf8",
				host, user, password);
			MySqlConnection c = new MySqlConnection(connStr);
			c.Open();

			MySqlCommand cmd = new MySqlCommand("DROP TABLE IF EXISTS test;CREATE TABLE test (id varbinary(16), active bit)", c);
			cmd.ExecuteNonQuery();
			cmd.CommandText = "INSERT INTO test (id, active) VALUES (CAST(0x1234567890 AS Binary), true)";
			cmd.ExecuteNonQuery();
			cmd.CommandText = "INSERT INTO test (id, active) VALUES (CAST(0x123456789a AS Binary), true)";
			cmd.ExecuteNonQuery();
			cmd.CommandText = "INSERT INTO test (id, active) VALUES (CAST(0x123456789b AS Binary), true)";
			cmd.ExecuteNonQuery();
			c.Close();

			MySqlConnection d = new MySqlConnection(connStr);
			d.Open();

			MySqlCommand cmd2 = new MySqlCommand("SELECT id, active FROM test", d);
			MySqlDataReader reader = null;
			try 
			{
				reader = cmd2.ExecuteReader();
				Assert.IsTrue(reader.Read());
				Assert.IsTrue(reader.GetBoolean(1));
			}
			catch (Exception ex) 
			{
				Assert.Fail(ex.Message);
			}
			finally 
			{
				if (reader != null) reader.Close();
			}
			
			d.Close();

			execSQL("DROP DATABASE IF EXISTS test2");
		}

		/// <summary>
		/// Bug #10281 Clone issue with MySqlConnection 
		/// </summary>
		[Test]
		public void TestConnectionClone()
		{
			MySqlConnection c = new MySqlConnection();
			MySqlConnection clone = (MySqlConnection) ((ICloneable)c).Clone();
			clone.ToString();
		}

		/// <summary>
		/// Bug #13321  	Persist security info does not woek
		/// </summary>
		[Test]
		public void PersistSecurityInfo()
		{
			string s = GetConnectionString(true).ToLower();
			int start = s.IndexOf("persist security info");
			int end = s.IndexOf(";", start);
			string newConnStr = s.Substring(0, start);
			newConnStr += s.Substring(end, s.Length - (end));
			newConnStr += ";persist security info=false";

			MySqlConnection conn2 = new MySqlConnection(newConnStr);
			string p = "password";
			if (conn2.ConnectionString.IndexOf("pwd") != -1)
				p = "pwd";
			else if (conn2.ConnectionString.IndexOf("passwd") != -1)
				p = "passwd";

			Assert.IsTrue(conn2.ConnectionString.IndexOf(p) != -1);
			conn2.Open();
			conn2.Close();
			Assert.IsTrue(conn2.ConnectionString.IndexOf(p) == -1);
		}

		/// <summary>
		/// Bug #13658  	connection.state does not update on Ping()
		/// </summary>
		[Test]
		public void PingUpdatesState()
		{
			MySqlConnection conn2 = new MySqlConnection(GetConnectionString(true));
			conn2.Open();
			KillConnection(conn2);
			Assert.IsFalse(conn2.Ping());
			Assert.IsTrue(conn2.State == ConnectionState.Closed);
		}

		/// <summary>
		/// Bug #16659  	Can't use double quotation marks(") as password access server by Connector/NET
		/// </summary>
		[Test]
		public void ConnectWithQuotePassword()
		{
			execSQL("GRANT ALL ON *.* to 'test'@'localhost' IDENTIFIED BY '\"'");
			MySqlConnection c = new MySqlConnection("server=" + host + ";uid=test;pwd='\"';pooling=false");
			try 
			{
				c.Open();
				c.Close();
			}
			catch (Exception ex) 
			{
				Assert.Fail(ex.Message);
			}
			execSQL("DELETE FROM mysql.user WHERE user='test'");
		}
	}
}
