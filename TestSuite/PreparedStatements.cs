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
using System.IO;
using NUnit.Framework;

namespace MySql.Data.MySqlClient.Tests
{
	[TestFixture()]
	public class PreparedStatements : BaseTest
	{
		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			Open();

			execSQL("DROP TABLE IF EXISTS Test");
		}

		[Test()]
		public void Simple() 
		{
			execSQL("DROP TABLE IF EXISTS Test");
			execSQL("CREATE TABLE Test (id INT, dec1 DECIMAL(5,2), name VARCHAR(100))");
			execSQL("INSERT INTO Test VALUES (1, 345.12, 'abcd')");

			MySqlCommand cmd = new MySqlCommand("INSERT INTO Test VALUES(1,345.12,'abcd')", conn);
			cmd.Prepare();
			cmd.ExecuteNonQuery();

			cmd.CommandText = "SELECT * FROM Test";
			cmd.Prepare();
			MySqlDataReader reader = null;
			try 
			{
				reader = cmd.ExecuteReader();
				Assert.IsTrue( reader.Read() );
				Assert.AreEqual( 1, reader.GetInt32(0) );
				Assert.AreEqual( 345.12, reader.GetDecimal(1) );
				Assert.AreEqual( "abcd", reader.GetString(2) );
			}
			catch (Exception ex) 
			{
				Assert.Fail( ex.Message );
			}
			finally 
			{
				if (reader != null) reader.Close();
			}
		}


		[Test()]
		public void SimplePrepareBeforeParms() 
		{
			execSQL("DROP TABLE IF EXISTS Test");
			execSQL("CREATE TABLE Test (one INTEGER, two INTEGER)");
			execSQL("INSERT INTO Test VALUES (1, 2)");

			// create the command and prepare the statement
			IDbCommand cmd = conn.CreateCommand();
			cmd.CommandText = "SELECT * FROM Test WHERE one = ?p1";
			cmd.Prepare();

			// create the parameter
			IDbDataParameter p1 = cmd.CreateParameter();
			p1.ParameterName = "?p1";
			p1.DbType = DbType.Int32;
			p1.Precision = (byte)10;
			p1.Scale = (byte)0;
			p1.Size = 4;
			cmd.Parameters.Add(p1);
			p1.Value = 1;

			// Execute the reader
			IDataReader reader = null;
			try 
			{
				reader = cmd.ExecuteReader();

				// Fetch the first record
				reader.Read();

				Assert.AreEqual(1, reader.GetInt32(0));
				Assert.AreEqual(2, reader.GetInt32(1));
			}
			catch (Exception ex) 
			{
				Assert.Fail( ex.Message );
			}
			finally 
			{
				if (reader != null) reader.Close();
			}
		}

		[Test()]
		public void DateAndTimes() 
		{
			execSQL("DROP TABLE IF EXISTS Test");
			execSQL("CREATE TABLE Test (id INT NOT NULL, d DATE, dt DATETIME, tm TIME, ts TIMESTAMP, PRIMARY KEY(id))");

			string sql = "INSERT INTO Test VALUES(?id, ?d, ?dt, ?tm, NULL)";
			MySqlCommand cmd = new MySqlCommand(sql, conn);
			cmd.Prepare();

			DateTime dt = DateTime.Now;
			dt = dt.AddMilliseconds( dt.Millisecond * -1 );
			TimeSpan ts = new TimeSpan( 8, 11, 44, 56, 501 );

			cmd.Parameters.Add( "?id", 1 );
			cmd.Parameters.Add( "?d", dt );
			cmd.Parameters.Add( "?dt", dt );
			cmd.Parameters.Add( "?tm", ts );
			int count = cmd.ExecuteNonQuery();
			Assert.AreEqual( 1, count, "Records affected by insert" );

			cmd.CommandText = "SELECT * FROM Test";
			cmd.Prepare();
			
			MySqlDataReader reader = null;
			try 
			{
				reader = cmd.ExecuteReader();
				reader.Read();
				Assert.AreEqual( 1, reader.GetInt32(0), "Id column" );
				Assert.AreEqual( dt.Date, reader.GetDateTime(1).Date, "Date column" );

				DateTime dt2 = reader.GetDateTime(2);
				Assert.AreEqual( dt.Date, dt2.Date );
				Assert.AreEqual( dt.Hour, dt2.Hour );
				Assert.AreEqual( dt.Minute, dt2.Minute );
				Assert.AreEqual( dt.Second, dt2.Second );

				TimeSpan ts2 = reader.GetTimeSpan(3);
				Assert.AreEqual( ts.Days, ts2.Days );
				Assert.AreEqual( ts.Hours, ts2.Hours );
				Assert.AreEqual( ts.Minutes, ts2.Minutes );
				Assert.AreEqual( ts.Seconds, ts2.Seconds );

				Assert.AreEqual( dt.Date, reader.GetDateTime(4).Date, "Timestamp column" );
			}
			catch (Exception ex) 
			{
				Assert.Fail( ex.Message );
			}
			finally 
			{
				if (reader != null) reader.Close();
			}
		}

		[Test()]
		public void ResetCommandText() 
		{
			execSQL("DROP TABLE IF EXISTS Test"); 
			execSQL("CREATE TABLE Test (id int, name varchar(100))");
			execSQL("INSERT INTO Test VALUES (1, 'Test')");

			MySqlCommand cmd = new MySqlCommand("SELECT id FROM Test", conn);
			cmd.Prepare();
			object o = cmd.ExecuteScalar();
			Assert.AreEqual( 1, o );

			cmd.CommandText = "SELECT name FROM Test";
			cmd.Prepare();
			o = cmd.ExecuteScalar();
			Assert.AreEqual( "Test", o );
			
		}

		[Test()]
		public void DifferentParameterOrder() 
		{
			execSQL("DROP TABLE IF EXISTS Test"); 
			execSQL("CREATE TABLE Test (id int, name varchar(100))");

			MySqlCommand cmd = new MySqlCommand("INSERT INTO Test (name,id) VALUES(?name,?id)", conn);
			cmd.Prepare();

			cmd.Parameters.Add( "?name", "Name" );
			cmd.Parameters.Add( "?id", 1 );
			Assert.AreEqual( 1, cmd.ExecuteNonQuery() );

			cmd.Parameters[0].Value = "Name 2";
			cmd.Parameters[1].Value = 2;
			Assert.AreEqual( 1, cmd.ExecuteNonQuery() );

			cmd.CommandText = "SELECT id FROM Test";
			Assert.AreEqual( 1, cmd.ExecuteScalar() );

			cmd.CommandText = "SELECT name FROM Test";
			Assert.AreEqual( "Name", cmd.ExecuteScalar() );
		}

		[Test()]
		public void Blobs() 
		{
			execSQL("DROP TABLE IF EXISTS Test");
			execSQL("CREATE TABLE Test (id INT, blob1 LONGBLOB, text1 LONGTEXT)");

			MySqlCommand cmd = new MySqlCommand("INSERT INTO Test VALUES (?id, ?blob1, ?text1)", conn);
			cmd.Prepare();

			byte[] bytes = Utils.CreateBlob( 400000 );
			string inStr = "This is my text";

			cmd.Parameters.Add( "?id", 1 );
			cmd.Parameters.Add( "?blob1", bytes );
			cmd.Parameters.Add( "?text1", inStr );
			int count = cmd.ExecuteNonQuery();
			Assert.AreEqual( 1, count );

			cmd.CommandText = "SELECT * FROM Test";
			cmd.Prepare();
			MySqlDataReader reader = null;

			try 
			{
				reader = cmd.ExecuteReader();
				Assert.IsTrue( reader.Read() );
				Assert.AreEqual( 1, reader.GetInt32(0) );
				Assert.AreEqual( bytes.Length, reader.GetBytes( 1, 0, null, 0, 0 ));	
				byte[] outBytes = new byte[ bytes.Length ];
				reader.GetBytes( 1, 0, outBytes, 0, bytes.Length );
				for (int x=0; x < bytes.Length; x++)
					Assert.AreEqual( bytes[x], outBytes[x] );
				Assert.AreEqual( inStr, reader.GetString( 2 ) );
			}
			catch (Exception ex) 
			{
				Assert.Fail( ex.Message );
			}
			finally 
			{
				if (reader != null) reader.Close();
			}
		}

		[Test]
		public void SimpleTest2() 
		{
			execSQL("DROP TABLE IF EXISTS Test");
			execSQL("CREATE TABLE Test (one integer, two integer, three integer, four integer, five integer, six integer, seven integer)");
			execSQL("INSERT INTO Test VALUES (1, 2, 3, 4, 5, 6, 7)");

			// create the command and prepare the statement
			IDbCommand cmd = conn.CreateCommand();
			cmd.CommandText = "SELECT one, two, three, four, five, six, seven FROM Test";
			cmd.Prepare();
			// Execute the reader
			IDataReader reader = null;
			try 
			{
				reader = cmd.ExecuteReader();
				// Fetch the first record
				reader.Read();

				Assert.AreEqual( 1, reader.GetInt32(0) );
				Assert.AreEqual( 2, reader.GetInt32(1) );
				Assert.AreEqual( 3, reader.GetInt32(2) );
				Assert.AreEqual( 4, reader.GetInt32(3) );
				Assert.AreEqual( 5, reader.GetInt32(4) );
				Assert.AreEqual( 6, reader.GetInt32(5) );
				Assert.AreEqual( 7, reader.GetInt32(6) );
			}
			catch (Exception ex) 
			{
				Assert.Fail( ex.Message );
			}
			finally 
			{
				if (reader != null) reader.Close();
			}
		}

		[Test]
		[Category("4.1")]
		public void Bug6271() 
		{
			execSQL("DROP TABLE IF EXISTS Test2");

			// Create the table again
			execSQL("CREATE TABLE `Test2` (id INT unsigned NOT NULL auto_increment, " +
				"`xpDOSG_Name` text,`xpDOSG_Desc` text, `Avatar` MEDIUMBLOB, `dtAdded` DATETIME, `dtTime` TIMESTAMP, " +
				"PRIMARY KEY(id)) ENGINE=InnoDB DEFAULT CHARSET=latin1" );

			string sql = "INSERT INTO `Test2` (`xpDOSG_Name`,`dtAdded`, `xpDOSG_Desc`,`Avatar`, `dtTime`) " +
				"VALUES(?name, ?dt, ?desc, ?Avatar, NULL)";

			MySqlCommand cmd = new MySqlCommand(sql, conn);
			cmd.Prepare();

			DateTime dt = DateTime.Now;
			dt = dt.AddMilliseconds( dt.Millisecond * -1 );

			byte[] xpDOSG_Avatar = Utils.CreateBlob( 13000 );
			cmd.Parameters.Add( "?name", "Ceci est un nom");

			cmd.Parameters.Add( "?desc", "Ceci est une description facile à plantouiller");
			cmd.Parameters.Add( "?avatar",xpDOSG_Avatar); 
			cmd.Parameters.Add( "?dt", dt);
			int count = cmd.ExecuteNonQuery();
			Assert.AreEqual( 1, count );

			MySqlDataReader reader = null;
			try 
			{
				cmd.CommandText = "SELECT * FROM Test2";
				reader = cmd.ExecuteReader();
				Assert.IsTrue( reader.Read() );
				Assert.AreEqual( "Ceci est un nom", reader.GetString(1) );
				Assert.AreEqual( dt.ToString("G"), reader.GetDateTime(4).ToString("G") );
				Assert.AreEqual( "Ceci est une description facile à plantouiller", reader.GetString(2) );
				
				long len = reader.GetBytes( 3, 0, null, 0, 0 );
				Assert.AreEqual( xpDOSG_Avatar.Length, len );
				byte[] outBytes = new byte[len];
				reader.GetBytes( 3, 0, outBytes, 0, (int)len );

				for (int x=0; x < xpDOSG_Avatar.Length; x++)
					Assert.AreEqual( xpDOSG_Avatar[x], outBytes[x] );
				
			}
			catch (Exception ex) 
			{
				Assert.Fail( ex.Message );
			}
			finally 
			{
				if (reader != null) reader.Close();
			}
		}

		[Test]
		public void SimpleTest() 
		{
			execSQL("DROP TABLE IF EXISTS Test");
			execSQL("CREATE TABLE Test (one integer, two integer )");
			execSQL("INSERT INTO Test VALUES( 1, 2)");
			// create the command and prepare the statement
			IDbCommand cmd = conn.CreateCommand();
			cmd.CommandText = "SELECT * FROM test where one = ?p1";
			// create the parameter
			IDbDataParameter p1 = cmd.CreateParameter();
			p1.ParameterName = "p1";
			p1.DbType = DbType.Int32;
			p1.Precision = (byte)10;
			p1.Scale = (byte)0;
			p1.Size = 4;
			cmd.Parameters.Add(p1);
			// prepare the command
			cmd.Prepare();
			// set the parameter value
			p1.Value = 1;
			// Execute the reader
			IDataReader reader = null;
			try 
			{
				reader = cmd.ExecuteReader();
				// Fetch the first record
				reader.Read();
			}
			catch (Exception ex) 
			{
				Assert.Fail( ex.Message );
			}
			finally 
			{
				if (reader != null) reader.Close();
			}
		}
	}
}
