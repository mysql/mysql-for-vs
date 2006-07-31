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
using MySql.Data.MySqlClient;
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
			InternalBytesAndBooleans(false);
        }

        [Category("4.1")]
        [Test]
        public void BytesAndBooleansPrepared()
        {
			InternalBytesAndBooleans(true);
		}

		private void InternalBytesAndBooleans( bool prepare ) 
		{
			execSQL("DROP TABLE IF EXISTS Test");
			execSQL("CREATE TABLE Test (id TINYINT, idu TINYINT UNSIGNED, i INT UNSIGNED)");
			execSQL("INSERT INTO Test VALUES (-98, 140, 20)");
			execSQL("INSERT INTO Test VALUES (0, 0, 0)");

			MySqlCommand cmd = new MySqlCommand("SELECT * FROM Test", conn);
			if (prepare) cmd.Prepare();
			MySqlDataReader reader = null;
			try 
			{
				reader = cmd.ExecuteReader();
				Assert.IsTrue(reader.Read());
				Assert.AreEqual(-98, (sbyte)reader.GetByte(0));
				Assert.AreEqual(140, reader.GetByte(1));
				Assert.IsTrue(reader.GetBoolean(1));
				Assert.AreEqual(20, reader.GetUInt32(2));
				Assert.AreEqual(20, reader.GetInt32(2));

				Assert.IsTrue(reader.Read());
				Assert.AreEqual(0, reader.GetByte(0));
				Assert.AreEqual(0, reader.GetByte(1));
				Assert.IsFalse(reader.GetBoolean(1));

				Assert.IsFalse(reader.Read());
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

		[Test]
		public void TestFloat() 
		{
			InternalTestFloats(false);
		}

        [Test]
        [Category("4.1")]
        public void TestFloatPrepared()
        {
            InternalTestFloats(true);
        }

		private void InternalTestFloats(bool prepared)
		{
			execSQL( "DROP TABLE IF EXISTS Test" );
			execSQL( "CREATE TABLE Test (fl FLOAT, db DOUBLE, dec1 DECIMAL(5,2))" );

			MySqlCommand cmd = new MySqlCommand("INSERT INTO Test VALUES (?fl, ?db, ?dec)", conn);
			cmd.Parameters.Add("?fl", MySqlDbType.Float);
			cmd.Parameters.Add("?db", MySqlDbType.Double);
			cmd.Parameters.Add("?dec", MySqlDbType.Decimal);
			cmd.Parameters[0].Value = 2.3;
			cmd.Parameters[1].Value = 4.6;
			cmd.Parameters[2].Value = 23.82;
			if (prepared)
				cmd.Prepare();
			int count = cmd.ExecuteNonQuery();
			Assert.AreEqual(1, count);

			cmd.Parameters[0].Value = 1.5;
			cmd.Parameters[1].Value = 47.85;
			cmd.Parameters[2].Value = 123.85;
			count = cmd.ExecuteNonQuery();
			Assert.AreEqual(1, count);

			MySqlDataReader reader = null;
			try 
			{
				cmd.CommandText = "SELECT * FROM Test";
				if (prepared) 
                    cmd.Prepare();
				reader = cmd.ExecuteReader();
				Assert.IsTrue(reader.Read());
				Assert.AreEqual(2.3, reader.GetFloat(0));
				Assert.AreEqual(4.6, reader.GetDouble(1));
				Assert.AreEqual(23.82, reader.GetDecimal(2));

				Assert.IsTrue(reader.Read());
				Assert.AreEqual(1.5, reader.GetFloat(0));
				Assert.AreEqual(47.85, reader.GetDouble(1));
				Assert.AreEqual(123.85, reader.GetDecimal(2));
			}
			catch (Exception ex) 
			{
				Assert.Fail(ex.Message);
			}
			finally 
			{
				if (reader != null) 
                    reader.Close();
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

		[Test]
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

		[Test]
		public void YearType() 
		{
			execSQL("DROP TABLE IF EXISTS Test");
			execSQL("CREATE TABLE Test (yr YEAR)");
			execSQL("INSERT INTO Test VALUES (98)");
			execSQL("INSERT INTO Test VALUES (1990)");
			execSQL("INSERT INTO Test VALUES (2004)");
            execSQL("SET SQL_MODE=''");
			execSQL("INSERT INTO Test VALUES (111111111111111111111)");

			MySqlCommand cmd = new MySqlCommand("SELECT * FROM Test", conn);
			MySqlDataReader reader = null;
			try 
			{
				reader = cmd.ExecuteReader();
				reader.Read();
				Assert.AreEqual(1998, reader.GetUInt32(0));
				reader.Read();
				Assert.AreEqual(1990, reader.GetUInt32(0));
				reader.Read();
				Assert.AreEqual(2004, reader.GetUInt32(0));
				reader.Read();
				Assert.AreEqual(0, reader.GetUInt32(0));
			}
			catch (Exception ex) 
			{
				Assert.Fail(ex.Message);
			}
			finally 
			{
				if (reader != null) 
                    reader.Close();
			}
		}

		[Test]
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

		/// <summary>
		/// Bug #7951 - Error reading timestamp column
		/// </summary>
		[Test]
		public void Timestamp() 
		{
			execSQL("DROP TABLE IF EXISTS test");
			execSQL("CREATE TABLE test (id int, dt DATETIME, ts2 TIMESTAMP(2), ts4 TIMESTAMP(4), " + 
				"ts6 TIMESTAMP(6), ts8 TIMESTAMP(8), ts10 TIMESTAMP(10), ts12 TIMESTAMP(12), " +
				"ts14 TIMESTAMP(14))");
			execSQL("INSERT INTO test (id, dt, ts2, ts4, ts6, ts8, ts10, ts12, ts14) " +
				"VALUES (1, Now(), Now(), Now(), Now(), Now(), Now(), Now(), Now())");

			MySqlDataAdapter da = new MySqlDataAdapter( "SELECT * FROM test", conn);
			DataTable dt = new DataTable();
			da.Fill(dt);

			DateTime now = (DateTime)dt.Rows[0]["dt"];
			Assert.AreEqual( 1, dt.Rows[0]["id"] );

			DateTime ts2 = (DateTime)dt.Rows[0]["ts2"];
			Assert.AreEqual( now.Year, ts2.Year );

			DateTime ts4 = (DateTime)dt.Rows[0]["ts4"];
			Assert.AreEqual( now.Year, ts4.Year );
			Assert.AreEqual( now.Month, ts4.Month );

			DateTime ts6 = (DateTime)dt.Rows[0]["ts6"];
			Assert.AreEqual( now.Year, ts6.Year );
			Assert.AreEqual( now.Month, ts6.Month );
			Assert.AreEqual( now.Day, ts6.Day );

			DateTime ts8 = (DateTime)dt.Rows[0]["ts8"];
			Assert.AreEqual( now.Year, ts8.Year );
			Assert.AreEqual( now.Month, ts8.Month );
			Assert.AreEqual( now.Day, ts8.Day );

			DateTime ts10 = (DateTime)dt.Rows[0]["ts10"];
			Assert.AreEqual( now.Year, ts10.Year );
			Assert.AreEqual( now.Month, ts10.Month );
			Assert.AreEqual( now.Day, ts10.Day );
			Assert.AreEqual( now.Hour, ts10.Hour );
			Assert.AreEqual( now.Minute, ts10.Minute );

			DateTime ts12 = (DateTime)dt.Rows[0]["ts12"];
			Assert.AreEqual( now.Year, ts12.Year );
			Assert.AreEqual( now.Month, ts12.Month );
			Assert.AreEqual( now.Day, ts12.Day );
			Assert.AreEqual( now.Hour, ts12.Hour );
			Assert.AreEqual( now.Minute, ts12.Minute );
			Assert.AreEqual( now.Second, ts12.Second );

			DateTime ts14 = (DateTime)dt.Rows[0]["ts14"];
			Assert.AreEqual( now.Year, ts14.Year );
			Assert.AreEqual( now.Month, ts14.Month );
			Assert.AreEqual( now.Day, ts14.Day );
			Assert.AreEqual( now.Hour, ts14.Hour );
			Assert.AreEqual( now.Minute, ts14.Minute );
			Assert.AreEqual( now.Second, ts14.Second );
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
					reader.GetInt64(0);
					reader.GetInt32(1); // <--- aint... this succeeds
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
					reader.GetInt64(0);
					reader.GetInt64(1); // <--- max(aint)... this fails
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

/*		[Test]
		public void TypeBoundaries() 
		{
			execSQL("DROP TABLE IF EXISTS Test");
			execSQL("CREATE TABLE Test ( MaxDouble DOUBLE, MinDouble DOUBLE, MaxFloat FLOAT, MinFloat FLOAT )");

			MySqlCommand cmd = new MySqlCommand(
				"INSERT Test (MaxDouble, MinDouble, MaxFloat, MinFloat) VALUES " +
				"(?maxDouble, ?minDouble, ?maxFloat, ?minFloat)", conn);
			cmd.Parameters.Add("?maxDouble", MySqlDouble.MaxValue);
			cmd.Parameters.Add("?minDouble", MySqlDouble.MinValue);
			cmd.Parameters.Add("?maxFloat", MySqlFloat.MaxValue);
			cmd.Parameters.Add("?minFloat", MySqlFloat.MinValue);
			cmd.ExecuteNonQuery();

			cmd.CommandText = "SELECT * FROM Test";
			try 
			{
				using (MySqlDataReader reader = cmd.ExecuteReader()) 
				{
					reader.Read();
					Assert.AreEqual(MySqlDouble.MaxValue, reader.GetDouble(0));
					Assert.AreEqual(MySqlDouble.MinValue, reader.GetDouble(1));
					Assert.AreEqual(MySqlFloat.MaxValue, reader.GetFloat(2));
					Assert.AreEqual(MySqlFloat.MinValue, reader.GetFloat(3));
				}
			}
			catch (Exception ex) 
			{
				Assert.Fail(ex.Message);
			}
		}*/

        [Test]
        public void BitAndDecimal()
        {
            execSQL("DROP TABLE IF EXISTS test");
            execSQL("CREATE TABLE test (bt1 BIT, bt4 BIT(4), bt11 BIT(11), bt23 BIT(23), bt32 BIT(32)) engine=myisam");
            execSQL("INSERT INTO test VALUES (1, 2, 120, 240, 1000)");

            MySqlCommand cmd = new MySqlCommand("SELECT * FROM test", conn);
            MySqlDataReader reader = null;
            try
            {
                reader = cmd.ExecuteReader();
                Assert.IsTrue(reader.Read());
                Assert.AreEqual(1, reader.GetInt32(0));
                Assert.AreEqual(2, reader.GetInt32(1));
                Assert.AreEqual(120, reader.GetInt32(2));
                Assert.AreEqual(240, reader.GetInt32(3));
                Assert.AreEqual(1000, reader.GetInt32(4));
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

        /// <summary>
        /// Bug #10486 MySqlDataAdapter.Update error for decimal column 
        /// </summary>
        [Test]
        public void UpdateDecimalColumns()
        {
            execSQL("DROP TABLE IF EXISTS test");
            execSQL("CREATE TABLE test (id int not null auto_increment primary key, " +
                "dec1 decimal(10,1))");

            MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM test", conn);
            MySqlCommandBuilder cb = new MySqlCommandBuilder(da);
            DataTable dt = new DataTable();
            da.Fill(dt);
            DataRow row = dt.NewRow();
            row["id"] = DBNull.Value;
            row["dec1"] = 23.4;
            dt.Rows.Add(row);
            da.Update(dt);

            dt.Clear();
            da.Fill(dt);
            Assert.AreEqual(1, dt.Rows.Count);
            Assert.AreEqual(1, dt.Rows[0]["id"]);
            Assert.AreEqual(23.4, dt.Rows[0]["dec1"]);
        }

        [Test]
        public void DecimalTests()
        {
            execSQL("DROP TABLE IF EXISTS test");
            execSQL("CREATE TABLE test (val decimal(10,1))");

            MySqlCommand cmd = new MySqlCommand("INSERT INTO test VALUES(?dec)", conn);
            cmd.Parameters.Add("?dec", (decimal)2.4);
            Assert.AreEqual(1, cmd.ExecuteNonQuery());

            cmd.Prepare();
            Assert.AreEqual(1, cmd.ExecuteNonQuery());

            cmd.CommandText = "SELECT * FROM test";
            MySqlDataReader reader = null;
            try
            {
                reader = cmd.ExecuteReader();
                Assert.IsTrue(reader.Read());
                Assert.IsTrue(reader[0] is Decimal);
                Assert.AreEqual(2.4, reader[0]);

                Assert.IsTrue(reader.Read());
                Assert.IsTrue(reader[0] is Decimal);
                Assert.AreEqual(2.4, reader[0]);

                Assert.IsFalse(reader.Read());
                Assert.IsFalse(reader.NextResult());
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

        [Test]
        public void DecimalTests2()
        {
            execSQL("DROP TABLE IF EXISTS test");
            execSQL("CREATE TABLE test (val decimal(10,1))");

            MySqlCommand cmd = new MySqlCommand("INSERT INTO test VALUES(?dec)", conn);
            cmd.Parameters.Add("?dec", (decimal)2.4);
            Assert.AreEqual(1, cmd.ExecuteNonQuery());

            cmd.Prepare();
            Assert.AreEqual(1, cmd.ExecuteNonQuery());

            cmd.CommandText = "SELECT * FROM test";
            MySqlDataReader reader = null;
            try
            {
                reader = cmd.ExecuteReader();
                Assert.IsTrue(reader.Read());
                Assert.IsTrue(reader[0] is Decimal);
                Assert.AreEqual(2.4, reader[0]);

                Assert.IsTrue(reader.Read());
                Assert.IsTrue(reader[0] is Decimal);
                Assert.AreEqual(2.4, reader[0]);

                Assert.IsFalse(reader.Read());
                Assert.IsFalse(reader.NextResult());
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
    
		[Test]
		[Category("5.0")]
		public void Bit()
		{
			execSQL("DROP TABLE IF EXISTS test");
			execSQL("CREATE TABLE test (bit1 BIT, bit2 BIT(5), bit3 BIT(10))");

			MySqlCommand cmd = new MySqlCommand("INSERT INTO test VALUES (?b1, ?b2, ?b3)", conn);
			cmd.Parameters.Add(new MySqlParameter("?b1", MySqlDbType.Bit));
			cmd.Parameters.Add(new MySqlParameter("?b2", MySqlDbType.Bit));
			cmd.Parameters.Add(new MySqlParameter("?b3", MySqlDbType.Bit));
			cmd.Prepare();
			cmd.Parameters[0].Value = 1;
			cmd.Parameters[1].Value = 2;
			cmd.Parameters[2].Value = 3;
			cmd.ExecuteNonQuery();

			MySqlDataReader reader = null;
			try 
			{
				cmd.CommandText = "SELECT * FROM test";
				cmd.Prepare();
				reader = cmd.ExecuteReader();
				Assert.IsTrue(reader.Read());
				Assert.AreEqual(1, reader[0]);
				Assert.AreEqual(2, reader[1]);
				Assert.AreEqual(3, reader[2]);
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

		/// <summary>
		/// Bug #17375 CommandBuilder ignores Unsigned flag at Parameter creation 
        /// Bug #15274 Use MySqlDbType.UInt32, throwed exception 'Only byte arrays can be serialize' 
		/// </summary>
		[Test]
		public void UnsignedTypes()
		{
			execSQL("DROP TABLE IF EXISTS Test");
			execSQL("CREATE TABLE Test (b TINYINT UNSIGNED PRIMARY KEY)");
			
			MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Test", conn);
			MySqlCommandBuilder cb = new MySqlCommandBuilder(da);

			DataTable dt = new DataTable();
			da.Fill(dt);

			DataView dv = new DataView(dt);
			DataRowView row;

			row = dv.AddNew();
			row["b"] = 120;
			row.EndEdit();
			da.Update(dv.Table);

			row = dv.AddNew();
			row["b"] = 135;
			row.EndEdit();
			da.Update(dv.Table);

			execSQL("DROP TABLE IF EXISTS Test");
			execSQL("CREATE TABLE Test (b MEDIUMINT UNSIGNED PRIMARY KEY)");
            execSQL("INSERT INTO test VALUES(20)");
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM test WHERE (b > ?id)", conn);
            cmd.Parameters.Add("?id", MySqlDbType.UInt16).Value = 10;
            MySqlDataReader dr = null;
            try
            {
                dr = cmd.ExecuteReader();
                dr.Read();
                Assert.AreEqual(20, dr.GetUInt16(0));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                if (dr != null)
                    dr.Close();
            }
		}
	}
}
