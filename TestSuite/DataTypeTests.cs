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
using MySql.Data.MySqlClient;
using MySql.Data.Types;
using System.Data;
using NUnit.Framework;

namespace MySql.Data.MySqlClient.Tests
{
	/// <summary>
	/// Summary description for ConnectionTests.
	/// </summary>
	[TestFixture] 
	public class DataTypeTests : BaseTest
	{
		[TestFixtureSetUp]
		public void TestFixtureSetup()
		{
			Open();
		}

		[TestFixtureTearDown()]
		public void TestFixtureTearDown() 
		{
			Close();
		}

		[SetUp()]
		protected override void Setup() 
		{
			base.Setup();

			execSQL("DROP TABLE IF EXISTS Test");
			execSQL("CREATE TABLE Test (id INT NOT NULL, name VARCHAR(100), d DATE, dt DATETIME, tm TIME,  PRIMARY KEY(id))");
		}

		[Test]
		public void BytesAndBooleans() 
		{
			InternalBytesAndBooleans( false );
			if (! Is41 && ! Is50) return;
			InternalBytesAndBooleans( true );
		}

		private void InternalBytesAndBooleans( bool prepare ) 
		{
			execSQL( "DROP TABLE IF EXISTS Test" );
			execSQL( "CREATE TABLE Test (id TINYINT, idu TINYINT UNSIGNED, i INT UNSIGNED)" );
			execSQL( "INSERT INTO Test VALUES (-98, 140, 20)" );
			execSQL( "INSERT INTO Test VALUES (0, 0, 0)" );

			MySqlCommand cmd = new MySqlCommand("SELECT * FROM Test", conn);
			if (prepare) cmd.Prepare();
			MySqlDataReader reader = null;
			try 
			{
				reader = cmd.ExecuteReader();
				Assert.IsTrue( reader.Read() );
				Assert.AreEqual( -98, (sbyte)reader.GetByte(0) );
				Assert.AreEqual( 140, reader.GetByte(1) );
				Assert.IsTrue( reader.GetBoolean(1) );
				Assert.AreEqual( 20, reader.GetUInt32(2) );
				Assert.AreEqual( 20, reader.GetInt32(2) );

				Assert.IsTrue( reader.Read() );
				Assert.AreEqual( 0, reader.GetByte(0) );
				Assert.AreEqual( 0, reader.GetByte(1) );
				Assert.IsFalse( reader.GetBoolean(1) );

				Assert.IsFalse( reader.Read() );
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
		public void TestFloats() 
		{
			InternalTestFloats(false);
			if (! Is41 && ! Is50) return;
			InternalTestFloats(true);
		}

		private void InternalTestFloats(bool prepared)
		{
			execSQL( "DROP TABLE IF EXISTS Test" );
			execSQL( "CREATE TABLE Test (fl FLOAT, db DOUBLE, dec1 DECIMAL(5,2))" );

			MySqlCommand cmd = new MySqlCommand("INSERT INTO Test VALUES (?fl, ?db, ?dec)", conn);
			cmd.Parameters.Add( "?fl", MySqlDbType.Float );
			cmd.Parameters.Add( "?db", MySqlDbType.Double );
			cmd.Parameters.Add( "?dec", MySqlDbType.Decimal );
			cmd.Parameters[0].Value = 2.3;
			cmd.Parameters[1].Value = 4.6;
			cmd.Parameters[2].Value = 23.82;
			if (prepared)
				cmd.Prepare();
			int count = cmd.ExecuteNonQuery();
			Assert.AreEqual( 1, count );

			cmd.Parameters[0].Value = 1.5;
			cmd.Parameters[1].Value = 47.85;
			cmd.Parameters[2].Value = 123.85;
			count = cmd.ExecuteNonQuery();
			Assert.AreEqual( 1, count );

			MySqlDataReader reader = null;
			try 
			{
				cmd.CommandText = "SELECT * FROM Test";
				if (prepared) cmd.Prepare();
				reader = cmd.ExecuteReader();
				Assert.IsTrue( reader.Read() );
				Assert.AreEqual( 2.3, reader.GetFloat(0) );
				Assert.AreEqual( 4.6, reader.GetDouble(1) );
				Assert.AreEqual( 23.82, reader.GetDecimal(2) );

				Assert.IsTrue( reader.Read() );
				Assert.AreEqual( 1.5, reader.GetFloat(0) );
				Assert.AreEqual( 47.85, reader.GetDouble(1) );
				Assert.AreEqual( 123.85, reader.GetDecimal(2) );
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
		public void TestGuid()
		{
			MySqlDataReader reader =null;

			try 
			{
				MySqlCommand cmd = new MySqlCommand("TRUNCATE TABLE Test", conn);
				cmd.ExecuteNonQuery();

				Guid g = Guid.NewGuid();
				cmd.CommandText = "INSERT INTO Test VALUES (?id, ?guid, NULL, NULL, NULL)";
				cmd.Parameters.Add( new MySqlParameter("?id", 1));
				cmd.Parameters.Add( new MySqlParameter( "?guid", g ));
				cmd.ExecuteNonQuery();

				cmd.Parameters[0].Value = 2;
				cmd.Parameters[1].Value = g.ToString("N");
				cmd.ExecuteNonQuery();

				cmd.Parameters[0].Value = 3;
				cmd.Parameters[1].Value = g.ToString("D");
				cmd.ExecuteNonQuery();

				cmd.Parameters[0].Value = 4;
				cmd.Parameters[1].Value = g.ToString("B");
				cmd.ExecuteNonQuery();

				cmd.Parameters[0].Value = 5;
				cmd.Parameters[1].Value = g.ToString("P");
				cmd.ExecuteNonQuery();

				cmd.CommandText = "SELECT * FROM Test";
				reader = cmd.ExecuteReader();

				Assert.AreEqual( true, reader.Read() );
				Guid newG = reader.GetGuid(1);
				Assert.AreEqual( g, newG );

				Assert.AreEqual( true, reader.Read() );
				newG = reader.GetGuid(1);
				Assert.AreEqual( g, newG );

				Assert.AreEqual( true, reader.Read() );
				newG = reader.GetGuid(1);
				Assert.AreEqual( g, newG );

				Assert.AreEqual( true, reader.Read() );
				newG = reader.GetGuid(1);
				Assert.AreEqual( g, newG );
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
		public void TestTime()
		{
			MySqlDataReader reader = null;

			try 
			{
				MySqlCommand cmd = new MySqlCommand("INSERT INTO Test (id, tm) VALUES (1, '00:00')", conn);
				cmd.ExecuteNonQuery();
				cmd.CommandText = "INSERT INTO Test (id, tm) VALUES (2, '512:45:17')";
				cmd.ExecuteNonQuery();

				cmd.CommandText = "SELECT * FROM Test";
				reader = cmd.ExecuteReader();
				reader.Read();

				object value = reader["tm"];
				Assert.AreEqual( value.GetType(), typeof(TimeSpan));
				TimeSpan ts = (TimeSpan)reader["tm"];
				Assert.AreEqual( 0, ts.Hours );
				Assert.AreEqual( 0, ts.Minutes );
				Assert.AreEqual( 0, ts.Seconds );

				reader.Read();
				value = reader["tm"];
				Assert.AreEqual( value.GetType(), typeof(TimeSpan));
				ts = (TimeSpan)reader["tm"];
				Assert.AreEqual( 21, ts.Days );
				Assert.AreEqual( 8, ts.Hours );
				Assert.AreEqual( 45, ts.Minutes );
				Assert.AreEqual( 17, ts.Seconds );
			
				reader.Close();
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}
			finally 
			{
				if (reader != null) reader.Close();
			}
		}

		[Test()]
		public void YearType() 
		{
			execSQL("DROP TABLE IF EXISTS Test");
			execSQL("CREATE TABLE Test (yr YEAR)");
			execSQL("INSERT INTO Test VALUES (98)");
			execSQL("INSERT INTO Test VALUES (1990)");
			execSQL("INSERT INTO Test VALUES (2004)");
			execSQL("INSERT INTO Test VALUES (111111111111111111111)");

			MySqlCommand cmd = new MySqlCommand("SELECT * FROM Test", conn);
			MySqlDataReader reader = null;
			try 
			{
				reader = cmd.ExecuteReader();
				reader.Read();
				Assert.AreEqual( 1998, reader.GetUInt32(0) );
				reader.Read();
				Assert.AreEqual( 1990, reader.GetUInt32(0) );
				reader.Read();
				Assert.AreEqual( 2004, reader.GetUInt32(0) );
				reader.Read();
				Assert.AreEqual( 0, reader.GetUInt32(0) );
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
		public void TypeCoercion()
		{
			MySqlParameter p = new MySqlParameter("?test", 1);
			Assert.AreEqual( DbType.Int32, p.DbType );
			Assert.AreEqual( MySqlDbType.Int32, p.MySqlDbType );

			p.DbType = DbType.Int64;
			Assert.AreEqual( DbType.Int64, p.DbType );
			Assert.AreEqual( MySqlDbType.Int64, p.MySqlDbType );

			p.MySqlDbType = MySqlDbType.Int16;
			Assert.AreEqual( DbType.Int16, p.DbType );
			Assert.AreEqual( MySqlDbType.Int16, p.MySqlDbType );
		}

		[Test]
		public void Timestamp() 
		{
			execSQL("DROP TABLE IF EXISTS test");
			execSQL("CREATE TABLE test (id int, ts TIMESTAMP)");
			execSQL("INSERT INTO test (id) VALUES (1)");

			DateTime now = DateTime.Now;
			MySqlDataAdapter da = new MySqlDataAdapter( "SELECT * FROM test", conn);
			DataTable dt = new DataTable();
			da.Fill(dt);

			Assert.AreEqual( 1, dt.Rows[0]["id"] );
			Assert.AreEqual( now.Date, ((DateTime)dt.Rows[0]["ts"]).Date );
		}


		[Test]
		public void AggregateTypesTest()
		{
			execSQL( "DROP TABLE IF EXISTS foo" );
			execSQL( "CREATE TABLE foo (abigint bigint, aint int)");
			execSQL( "INSERT INTO foo VALUES (1, 2)");
			execSQL( "INSERT INTO foo VALUES (2, 3)");
			execSQL( "INSERT INTO foo VALUES (3, 4)");
			execSQL( "INSERT INTO foo VALUES (3, 5)");
						
			// Try a normal query
			string NORMAL_QRY = "SELECT abigint, aint FROM foo WHERE abigint = {0}";
			string qry = String.Format(NORMAL_QRY, 3);
			MySqlCommand cmd = new MySqlCommand(qry, conn);
			MySqlDataReader reader = null;

			try 
			{
				reader = cmd.ExecuteReader();
				while (reader.Read())
				{
					long field1 = reader.GetInt64(0);
					int field2 = reader.GetInt32(1); // <--- aint... this succeeds
				}
			}
			catch (Exception ex) 
			{
				Assert.Fail( ex.Message );
			}
			finally 
			{
				if (reader != null) reader.Close();
			}

			cmd.CommandText = "SELECT abigint, max(aint) FROM foo GROUP BY abigint";
			try 
			{
				reader = cmd.ExecuteReader();
				while (reader.Read())
				{
					long field1 = reader.GetInt64(0);
					long field2 = reader.GetInt64(1); // <--- max(aint)... this fails
				}
			}
			catch (Exception ex) 
			{
				Assert.Fail( ex.Message );
			}
			finally 
			{
				if (reader != null) reader.Close();
			}
			execSQL( "DROP TABLE IF EXISTS foo");
		}


	}
}
