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
using System.Data.Common;
using System.Threading;
using MySql.Data.MySqlClient;
using MySql.Data.Types;
using System.Globalization;
using NUnit.Framework;
using System.Text;

namespace MySql.Data.MySqlClient.Tests
{
	[NUnit.Framework.TestFixture]
	public class DateTimeTests : BaseTest
	{
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			Open();
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown() 
		{
			Close();
		}

        protected override void Setup()
        {
            base.Setup();
            execSQL("DROP TABLE IF EXISTS Test");
            execSQL("CREATE TABLE Test (id INT NOT NULL, dt DATETIME, d DATE, " +
                "t TIME, ts TIMESTAMP, PRIMARY KEY(id))");
        }

		[Test]
		public void ConvertZeroDateTime()
		{
			execSQL("INSERT INTO Test VALUES(1, '0000-00-00', '0000-00-00', " +
                "'00:00:00', NULL)");

			MySqlConnection c;
			MySqlDataReader reader = null;

			string connStr = this.GetConnectionString(true);
			connStr += ";convert zero datetime=yes";
			c = new MySqlConnection(connStr);

			try 
			{
				c.Open();

				MySqlCommand cmd = new MySqlCommand("SELECT * FROM test", c);
				reader = cmd.ExecuteReader();
				Assert.IsTrue(reader.Read());
				Assert.AreEqual(DateTime.MinValue.Date, reader.GetDateTime(1).Date);
				Assert.AreEqual(DateTime.MinValue.Date, reader.GetDateTime(2).Date);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}
			finally 
			{
				if (reader != null) reader.Close();
				c.Close();
			}
		}

		[Test]
		public void TestNotAllowZerDateAndTime() 
		{
            execSQL("SET SQL_MODE=''");
			execSQL("INSERT INTO Test VALUES(1, 'Test', '0000-00-00', '0000-00-00', '00:00:00')");
			execSQL("INSERT INTO Test VALUES(2, 'Test', '2004-11-11', '2004-11-11', '06:06:06')");

			MySqlDataReader reader = null;
			try 
			{
				MySqlCommand cmd = new MySqlCommand("SELECT * FROM Test", conn);
				reader = cmd.ExecuteReader();
				Assert.IsTrue(reader.Read());

				MySqlDateTime testDate = reader.GetMySqlDateTime(2);
                Assert.IsFalse(testDate.IsValidDateTime, "IsZero is false");

				try 
				{
                    reader.GetValue(2);
                    Assert.Fail("This should not work");
				}
				catch (MySqlConversionException) { }

				Assert.IsTrue( reader.Read() );

				DateTime dt2 = (DateTime)reader.GetValue(2);
				Assert.AreEqual( new DateTime(2004,11,11).Date, dt2.Date );
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
		public void DateAdd() 
		{
			MySqlCommand cmd = new MySqlCommand( "select date_add(?someday, interval 1 hour)", 
                conn);
			DateTime now = DateTime.Now;
			DateTime later = now.AddHours(1);
			later = later.AddMilliseconds(later.Millisecond * -1);
			cmd.Parameters.Add("?someday", now );
			MySqlDataReader reader = null;
			try 
			{
				reader = cmd.ExecuteReader();
				Assert.IsTrue(reader.Read());
				DateTime dt = reader.GetDateTime(0);
				Assert.AreEqual(later.Date, dt.Date);
				Assert.AreEqual(later.Hour, dt.Hour);
				Assert.AreEqual(later.Minute, dt.Minute);
				Assert.AreEqual(later.Second, dt.Second);
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

        /// <summary>
        /// Bug #9619 Cannot update row using DbDataAdapter when row contains an invalid date 
        /// Bug #15112 MySqlDateTime Constructor 
        /// </summary>
		[Test]
		public void TestAllowZeroDateTime()
		{
			execSQL("INSERT INTO Test (id, d, dt) VALUES (1, '0000-00-00', '0000-00-00 00:00:00')");

			MySqlConnection c = new MySqlConnection(
				conn.ConnectionString + ";pooling=false;AllowZeroDatetime=true");
			c.Open();
			MySqlDataReader reader = null;
			try 
			{
				MySqlCommand cmd = new MySqlCommand("SELECT * FROM Test", c);
				reader = cmd.ExecuteReader();
				reader.Read();

				Assert.IsTrue(reader.GetValue(1) is MySqlDateTime);
				Assert.IsTrue(reader.GetValue(2) is MySqlDateTime);

				Assert.IsFalse(reader.GetMySqlDateTime(1).IsValidDateTime);
				Assert.IsFalse(reader.GetMySqlDateTime(2).IsValidDateTime);

				try 
				{
					reader.GetDateTime(1);
					Assert.Fail("This should not succeed");
				}
				catch (MySqlConversionException) {}
                reader.Close();
                reader = null;

                DataTable dt = new DataTable();
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM test", c);
                MySqlCommandBuilder cb = new MySqlCommandBuilder(da);
                da.Fill(dt);
                dt.Rows[0]["id"] = 2;
                DataRow row = dt.NewRow();
                row["id"] = 3;
                row["d"] = new MySqlDateTime("2003-9-24");
                row["dt"] = new MySqlDateTime("0000/0/00 00:00:00");
                dt.Rows.Add(row);

                da.Update(dt);

                dt.Clear();
                da.Fill(dt);
                Assert.AreEqual(2, dt.Rows.Count);
                MySqlDateTime date = (MySqlDateTime)dt.Rows[1]["d"];
                Assert.AreEqual(2003, date.Year);
                Assert.AreEqual(9, date.Month);
                Assert.AreEqual(24, date.Day);
			}
			catch (Exception ex) 
			{
				Assert.Fail(ex.Message);
			}
			finally 
			{
				if (reader != null) reader.Close();
				c.Close();
			}
		}

		[Test]
		public void InsertDateTimeValue()
		{
			MySqlConnection c = new MySqlConnection( conn.ConnectionString + 
                ";allow zero datetime=yes");
			try 
			{
				c.Open();
				MySqlDataAdapter da = new MySqlDataAdapter("SELECT id, dt FROM Test", c);
				MySqlCommandBuilder cb = new MySqlCommandBuilder(da);

				DataTable dt = new DataTable();
				dt.Columns.Add(new DataColumn("id", typeof(int)));
				dt.Columns.Add(new DataColumn("dt", typeof(DateTime)));

				da.Fill(dt);

				DateTime now = DateTime.Now;
				DataRow row = dt.NewRow();
				row["id"] = 1;
				row["dt"] = now;
				dt.Rows.Add(row);
				da.Update(dt);

				dt.Clear();
				da.Fill(dt);

				Assert.AreEqual(1, dt.Rows.Count);
				Assert.AreEqual(now.Date, ((DateTime)dt.Rows[0]["dt"]).Date );
			}
			catch (Exception ex) 
			{
				Assert.Fail( ex.Message );
			}
			finally 
			{
				c.Close();
			}
		}


		[Test]
		public void SortingMySqlDateTimes()
		{
			execSQL("INSERT INTO Test (id, dt) VALUES (1, '2004-10-01')");
			execSQL("INSERT INTO Test (id, dt) VALUES (2, '2004-10-02')");
			execSQL("INSERT INTO Test (id, dt) VALUES (3, '2004-11-01')");
			execSQL("INSERT INTO Test (id, dt) VALUES (4, '2004-11-02')");

			CultureInfo curCulture = Thread.CurrentThread.CurrentCulture;
			CultureInfo curUICulture = Thread.CurrentThread.CurrentUICulture;
			CultureInfo cul = new CultureInfo("en-GB");
			Thread.CurrentThread.CurrentCulture = cul;
			Thread.CurrentThread.CurrentUICulture = cul;

			using( MySqlConnection c = new MySqlConnection( conn.ConnectionString + ";allow zero datetime=yes" ))
			{
				MySqlDataAdapter da = new MySqlDataAdapter("SELECT dt FROM Test", c);
				DataTable dt = new DataTable();
				da.Fill(dt);
				
				DataView dv = dt.DefaultView;
				dv.Sort = "dt ASC";

				Assert.AreEqual( new DateTime(2004, 10, 1).Date, Convert.ToDateTime(dv[0]["dt"]).Date );
				Assert.AreEqual( new DateTime(2004, 10, 2).Date, Convert.ToDateTime(dv[1]["dt"]).Date );
				Assert.AreEqual( new DateTime(2004, 11, 1).Date, Convert.ToDateTime(dv[2]["dt"]).Date );
				Assert.AreEqual( new DateTime(2004, 11, 2).Date, Convert.ToDateTime(dv[3]["dt"]).Date );

				Thread.CurrentThread.CurrentCulture = curCulture;
				Thread.CurrentThread.CurrentUICulture = curUICulture;
			}
		}

		[Test]
		public void TestZeroDateTimeException() 
		{
			execSQL("INSERT INTO Test (id, d, dt) VALUES (1, '0000-00-00', '0000-00-00 00:00:00')");

			MySqlDataReader reader = null;
			try 
			{
				MySqlCommand cmd = new MySqlCommand("SELECT * FROM Test", conn);
				reader = cmd.ExecuteReader();
				reader.Read();
				reader.GetDateTime(2);
				Assert.Fail("Should throw an exception");
			}
			catch (MySqlConversionException) 
			{
			}
			catch (MySqlException ex) 
			{
				Assert.Fail( ex.Message );
			}
			finally 
			{
				if (reader != null) reader.Close();
			}
		}

        /// <summary>
		/// Bug #8929  	Timestamp values with a date > 10/29/9997 cause problems
		/// </summary>
		[Test]
		public void LargeDateTime() 
		{
			MySqlCommand cmd = new MySqlCommand("INSERT INTO Test (id, dt) VALUES(?id,?dt)", conn);
			cmd.Parameters.Add(new MySqlParameter("?id", 1));
			cmd.Parameters.Add(new MySqlParameter("?dt", DateTime.Parse("9997-10-29")));
			cmd.ExecuteNonQuery();
			cmd.Parameters[0].Value = 2;
			cmd.Parameters[1].Value = DateTime.Parse("9997-10-30");
			cmd.ExecuteNonQuery();
			cmd.Parameters[0].Value = 3;
			cmd.Parameters[1].Value = DateTime.Parse("9999-12-31");
			cmd.ExecuteNonQuery();

			cmd.CommandText = "SELECT id,dt FROM Test";
			using (MySqlDataReader reader = cmd.ExecuteReader()) 
			{
				Assert.IsTrue(reader.Read());
				Assert.AreEqual(DateTime.Parse("9997-10-29").Date, reader.GetDateTime(1).Date);
				Assert.IsTrue(reader.Read());
				Assert.AreEqual(DateTime.Parse("9997-10-30").Date, reader.GetDateTime(1).Date);
				Assert.IsTrue(reader.Read());
				Assert.AreEqual(DateTime.Parse("9999-12-31").Date, reader.GetDateTime(1).Date);
			}
		}

		[Test]
		public void UsingDatesAsStrings()
		{
			MySqlCommand cmd = new MySqlCommand("INSERT INTO test (id, dt) VALUES (1, ?dt)", conn);
			cmd.Parameters.Add("?dt", MySqlDbType.Date);
			cmd.Parameters[0].Value = "2005-03-04";
			cmd.ExecuteNonQuery();

			MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM test", conn);
			DataTable dt = new DataTable();
			da.Fill(dt);
			Assert.AreEqual(1, dt.Rows.Count);
			DateTime date = (DateTime)dt.Rows[0]["dt"];
			Assert.AreEqual(2005, date.Year);
			Assert.AreEqual(3, date.Month);
			Assert.AreEqual(4, date.Day);
		}

        /// <summary>
        /// Bug #19481 Where clause with datetime throws exception [any warning causes the exception]
        /// </summary>
        [Test]
        public void Bug19481()
        {
            execSQL("DROP TABLE IF EXISTS test");
            execSQL("CREATE TABLE test(ID INT NOT NULL AUTO_INCREMENT, " +
                "SATELLITEID VARCHAR(3) NOT NULL, ANTENNAID INT, AOS_TIMESTAMP DATETIME NOT NULL, " +
                "TEL_TIMESTAMP DATETIME, LOS_TIMESTAMP DATETIME, PRIMARY KEY (ID))");
            execSQL("INSERT INTO test VALUES (NULL,'224','0','2005-07-24 00:00:00'," +
                "'2005-07-24 00:02:00','2005-07-24 00:22:00')");
            execSQL("INSERT INTO test VALUES (NULL,'155','24','2005-07-24 03:00:00'," +
                "'2005-07-24 03:02:30','2005-07-24 03:20:00')");
            execSQL("INSERT INTO test VALUES (NULL,'094','34','2005-07-24 09:00:00'," +
                "'2005-07-24 09:00:30','2005-07-24 09:15:00')");
            execSQL("INSERT INTO test VALUES (NULL,'224','54','2005-07-24 12:00:00'," +
                "'2005-07-24 12:01:00','2005-07-24 12:33:00')");
            execSQL("INSERT INTO test VALUES (NULL,'155','25','2005-07-24 15:00:00'," +
                "'2005-07-24 15:02:00','2005-07-24 15:22:00')");
            execSQL("INSERT INTO test VALUES (NULL,'094','0','2005-07-24 17:00:00'," +
                "'2005-07-24 17:02:12','2005-07-24 17:20:00')");
            execSQL("INSERT INTO test VALUES (NULL,'224','24','2005-07-24 19:00:00'," +
                "'2005-07-24 19:02:00','2005-07-24 19:27:00')");
            execSQL("INSERT INTO test VALUES (NULL,'155','34','2005-07-24 21:00:00'," +
                "'2005-07-24 21:02:33','2005-07-24 21:22:55')");
            execSQL("INSERT INTO test VALUES (NULL,'094','55','2005-07-24 23:00:00'," +
                "'2005-07-24 23:00:45','2005-07-24 23:22:23')");

            DateTime date = DateTime.Parse("7/24/2005");
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("SELECT ID, ANTENNAID, TEL_TIMESTAMP, LOS_TIMESTAMP FROM test " +
                "WHERE TEL_TIMESTAMP >= '{0}'", date.ToString("u"));
            MySqlDataAdapter da = new MySqlDataAdapter(sql.ToString(), conn);
            DataSet dataSet = new DataSet();
            da.Fill(dataSet);
        }

        /// <summary>
        /// Bug #17736 Selecting a row with with empty date '0000-00-00' results in Read() hanging. 
        /// </summary>
        [Category("4.1")]
        [Test]
        public void PreparedZeroDateTime()
        {
            execSQL("INSERT INTO test VALUES(1, Now(), '0000-00-00', NULL, NULL)");
            MySqlCommand cmd = new MySqlCommand("SELECT d FROM test WHERE id=?id", conn);
            cmd.Parameters.Add("?id", 1);
            cmd.Prepare();
            MySqlDataReader reader = null;
            try
            {
                reader = cmd.ExecuteReader();
                reader.Read();
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
        public void DateTimeInDataTable()
        {
            execSQL("INSERT INTO test VALUES(1, Now(), '0000-00-00', NULL, NULL)");

            MySqlConnection c = new MySqlConnection(
                conn.ConnectionString + ";pooling=false;AllowZeroDatetime=true");
            c.Open();

            MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM test", c);
            DataTable dt = new DataTable();
            try
            {
                da.Fill(dt);
                DataRow row = dt.NewRow();
                row["id"] = 2;
                row["dt"] = DateTime.Now;
                row["d"] = DateTime.Now;
                row["t"] = DateTime.Now;
                row["ts"] = DBNull.Value;
                dt.Rows.Add(row);
                da.Update(dt);

                dt.Rows.Clear();
                da.Fill(dt);
                Assert.AreEqual(2, dt.Rows.Count);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                if (c != null)
                    c.Close();
            }
        }        
	}

}
