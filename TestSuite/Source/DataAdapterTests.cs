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
	public class DataAdapterTests : BaseTest
	{
		protected override void Setup()
		{
			base.Setup();

			execSQL("DROP TABLE IF EXISTS Test");
			execSQL("CREATE TABLE Test (id INT NOT NULL AUTO_INCREMENT, " +
                "id2 INT NOT NULL, name VARCHAR(100), dt DATETIME, tm TIME, " +
                "ts TIMESTAMP, OriginalId INT, PRIMARY KEY(id, id2))");
		}


		[Test]
		public void TestFill()
		{
			FillImpl(false);
			FillImpl(true);
		}

		private void FillImpl(bool prepare)
		{
			execSQL("TRUNCATE TABLE Test");
			execSQL("INSERT INTO Test (id, id2, name, dt) VALUES (NULL, 1, 'Name 1', Now())");
			execSQL("INSERT INTO Test (id, id2, name, dt) VALUES (NULL, 2, NULL, Now())");
			execSQL("INSERT INTO Test (id, id2, name, dt) VALUES (NULL, 3, '', Now())");

			MySqlDataAdapter da = new MySqlDataAdapter("select * from Test", conn);
			if (prepare) da.SelectCommand.Prepare();
			DataSet ds = new DataSet();
			da.Fill(ds, "Test");

			Assert.AreEqual(1, ds.Tables.Count);
			Assert.AreEqual(3, ds.Tables[0].Rows.Count);

			Assert.AreEqual(1, ds.Tables[0].Rows[0]["id2"]);
			Assert.AreEqual(2, ds.Tables[0].Rows[1]["id2"]);
			Assert.AreEqual(3, ds.Tables[0].Rows[2]["id2"]);

			Assert.AreEqual("Name 1", ds.Tables[0].Rows[0]["name"]);
			Assert.AreEqual(DBNull.Value, ds.Tables[0].Rows[1]["name"]);
			Assert.AreEqual(String.Empty, ds.Tables[0].Rows[2]["name"]);
		}

		[Test]
		public void TestUpdate()
		{
			MySqlCommand cmd = new MySqlCommand("TRUNCATE TABLE Test", conn);
			cmd.ExecuteNonQuery();

			MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Test", conn);
			MySqlCommandBuilder cb = new MySqlCommandBuilder(da);
			DataTable dt = new DataTable();
			da.Fill(dt);

			DataRow dr = dt.NewRow();
			dr["id2"] = 2;
			dr["name"] = "TestName1";
			dt.Rows.Add(dr);
			int count = da.Update(dt);

			// make sure our refresh of auto increment values worked
			Assert.AreEqual(1, count, "checking insert count");
			Assert.IsNotNull(dt.Rows[ dt.Rows.Count-1 ]["id"], 
                "Checking auto increment column");

            dt.Rows.Clear();
            da.Fill(dt);
			dt.Rows[0]["id2"] = 3;
			dt.Rows[0]["name"] = "TestName2";
			dt.Rows[0]["ts"] = DBNull.Value;
			DateTime day1 = new DateTime(2003, 1, 16, 12, 24, 0);
			dt.Rows[0]["dt"] = day1;
			dt.Rows[0]["tm"] = day1.TimeOfDay;
			count = da.Update(dt);

			Assert.IsNotNull(dt.Rows[0]["ts"], "checking refresh of record");
			Assert.AreEqual(3, dt.Rows[0]["id2"], "checking refresh of primary column");

			dt.Rows.Clear();
			da.Fill(dt);

			Assert.AreEqual(1, count, "checking update count");
			DateTime dateTime = (DateTime)dt.Rows[0]["dt"];
			Assert.AreEqual(day1.Date, dateTime.Date, "checking date");
			Assert.AreEqual(day1.TimeOfDay, dt.Rows[0]["tm"], "checking time");

			dt.Rows[0].Delete();
			count = da.Update(dt);

			Assert.AreEqual(1, count, "checking insert count");

			dt.Rows.Clear();
			da.Fill(dt);
			Assert.AreEqual(0, dt.Rows.Count, "checking row count");
		}

		[Test]
		public void OriginalInName()
		{
			MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Test", conn);
			MySqlCommandBuilder cb = new MySqlCommandBuilder(da);
			cb.ToString();  // keep the compiler happy
			DataTable dt = new DataTable();
			da.Fill(dt);

			DataRow row = dt.NewRow();
			row["id"] = DBNull.Value;
			row["id2"] = 1;
			row["name"] = "Test";
			row["dt"] = DBNull.Value;
			row["tm"] = DBNull.Value;
			row["ts"] = DBNull.Value;
			row["OriginalId"] = 2;
			dt.Rows.Add( row );
			da.Update(dt);

			Assert.AreEqual( 1, dt.Rows.Count );
			Assert.AreEqual( 2, dt.Rows[0]["OriginalId"] );
		}

		[Test]
		public void UseAdapterPropertyOfCommandBuilder() 
		{
			execSQL("INSERT INTO Test (id, id2, name) VALUES (NULL, 1, 'Test')");

			MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Test", conn);
			MySqlCommandBuilder cb = new MySqlCommandBuilder();
			cb.DataAdapter = da;

			DataTable dt = new DataTable();
			da.Fill(dt);

			dt.Rows[0]["name"] = "Test Update";
			int updateCnt = da.Update(dt);

			Assert.AreEqual( 1, updateCnt );

			dt.Rows.Clear();
			da.Fill(dt);

			Assert.AreEqual( 1, dt.Rows.Count );
			Assert.AreEqual( "Test Update", dt.Rows[0]["name"] );
		}

		[Test]
		public void UpdateNullTextFieldToEmptyString() 
		{
			execSQL("INSERT INTO Test (id, id2, name) VALUES (1, 1, NULL)");

			try 
			{
				MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Test", conn);
				MySqlCommandBuilder cb = new MySqlCommandBuilder(da);
				cb.ToString();  // keep the compiler happy

				DataTable dt = new DataTable();
				da.Fill(dt);

				dt.Rows[0]["name"] = "";
				int updateCnt = da.Update(dt);

				Assert.AreEqual( 1, updateCnt );

				dt.Rows.Clear();
				da.Fill(dt);

				Assert.AreEqual( 1, dt.Rows.Count );
				Assert.AreEqual( "", dt.Rows[0]["name"] );
			}
			catch (Exception ex) 
			{
				Assert.Fail( ex.Message );
			}
		}

		[Test]
		public void UpdateExtendedTextFields() 
		{
			execSQL("DROP TABLE IF EXISTS Test");
			execSQL( "CREATE TABLE Test (id int, notes MEDIUMTEXT, PRIMARY KEY(id))" );
			execSQL("INSERT INTO Test VALUES(1, 'This is my note')" );

			MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Test", conn);
			MySqlCommandBuilder cb = new MySqlCommandBuilder(da);
			cb.ToString();  // keep the compiler happy
			DataTable dt = new DataTable();
			da.Fill(dt);

			dt.Rows[0]["notes"] = "This is my new note";
			da.Update( dt );

			dt.Clear();
			da.Fill(dt);
			Assert.AreEqual( "This is my new note", dt.Rows[0]["notes"]);
		}

		[Test]
		public void SelectMoreThan252Rows() 
		{
			for (int i=0; i < 500; i++) 
				execSQL("INSERT INTO Test(id, id2) VALUES(NULL, " + i + ")");

			MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Test", conn);
			DataTable dt = new DataTable();
			da.Fill(dt);

			Assert.AreEqual( 500, dt.Rows.Count );
		}

/*		[Test]
		[Explicit]
		public void UpdateManyRows() 
		{
			MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Test", conn);
			MySqlCommandBuilder cb = new MySqlCommandBuilder(da);
			DataTable dt = new DataTable();
			da.Fill(dt);

			for (int i=0; i < 100000; i++)
			{
				DataRow dr = dt.NewRow();
				dr["id"] = DBNull.Value;
				dr["id2"] = i;
				dt.Rows.Add( dr );
				DataTable changes = dt.GetChanges();
				da.Update( changes );
				dt.AcceptChanges();
			}

			dt.Clear();
			da.Fill(dt);
			Assert.AreEqual(100000, dt.Rows.Count);
		}
*/
		[Test]
		public void DiscreteValues() 
		{
			execSQL("DROP TABLE IF EXISTS Test");
			execSQL("CREATE TABLE Test (id int, name varchar(200), dt DATETIME, b1 TEXT)");
			execSQL("INSERT INTO Test VALUES (1, 'Test', '2004-08-01', 'Text 1')");
			execSQL("INSERT INTO Test VALUES (2, 'Test 1', '2004-07-02', 'Text 2')");

			MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Test", conn);
			DataTable dt = new DataTable();
			da.Fill(dt);

			Assert.AreEqual( "Test", dt.Rows[0]["name"] );
			Assert.AreEqual( "Test 1", dt.Rows[1]["name"] );

			Assert.AreEqual( "Text 1", dt.Rows[0]["b1"] );
			Assert.AreEqual( "Text 2", dt.Rows[1]["b1"] );

			Assert.AreEqual( new DateTime(2004, 8, 1, 0, 0, 0).ToString(),  dt.Rows[0]["dt"].ToString() );
			Assert.AreEqual( new DateTime(2004, 7, 2, 0, 0, 0).ToString(), dt.Rows[1]["dt"].ToString() );
		}

		[Test]
		public void Bug5798() 
		{
			execSQL("INSERT INTO Test (id, id2, name) VALUES (1, 1, '')");

			MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Test", conn);
			MySqlCommandBuilder cb = new MySqlCommandBuilder(da);
			cb.ToString();  // keep the compiler happy
			DataTable dt = new DataTable();
			da.Fill(dt);

			Assert.AreEqual(String.Empty, dt.Rows[0]["name"]);

			dt.Rows[0]["name"] = "Test";
			da.Update(dt);

			dt.Clear();
			da.Fill(dt);
			Assert.AreEqual("Test", dt.Rows[0]["name"]);
		}

		[Test]
		public void ColumnMapping() 
		{
			execSQL("DROP TABLE IF EXISTS Test");
			execSQL("CREATE TABLE Test (id int, dcname varchar(100), primary key(id))");
			execSQL("INSERT INTO Test VALUES (1, 'Test1')");
			execSQL("INSERT INTO Test VALUES (2, 'Test2')");
			execSQL("INSERT INTO Test VALUES (3, 'Test3')");
			execSQL("INSERT INTO Test VALUES (4, 'Test4')");

//			MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Test", conn);
//			MySqlCommandBuilder cb = new MySqlCommandBuilder(da);
//			DataTable dt = new DataTable();
		}

		[Test]
		public void TestFillWithHelper() 
		{
			execSQL("DROP TABLE IF EXISTS table1");
			execSQL("DROP TABLE IF EXISTS table2");
			execSQL( "CREATE TABLE table1 (`key` INT, PRIMARY KEY(`key`))" );
			execSQL( "CREATE TABLE table2 (`key` INT, PRIMARY KEY(`key`))" );
			execSQL("INSERT INTO table1 VALUES (1)");
			execSQL("INSERT INTO table2 VALUES (1)");

			string sql = "SELECT table1.key FROM table1 WHERE table1.key=1; " +
				"SELECT table2.key FROM table2 WHERE table2.key=1";
			DataSet ds = MySqlHelper.ExecuteDataset( conn, sql, null );
			Assert.AreEqual( 2, ds.Tables.Count );
			Assert.AreEqual( 1, ds.Tables[0].Rows.Count );
			Assert.AreEqual( 1, ds.Tables[1].Rows.Count );
			Assert.AreEqual( 1, ds.Tables[0].Rows[0]["key"] );
			Assert.AreEqual( 1, ds.Tables[1].Rows[0]["key"] );
		}

		/// <summary>
		/// Bug #8509 - MySqlDataAdapter.FillSchema does not interpret unsigned integer
		/// </summary>
		[Test]
		public void AutoIncrementColumns()
		{
			execSQL("DROP TABLE IF EXISTS Test");
			execSQL("CREATE TABLE Test (id int(10) unsigned NOT NULL auto_increment primary key)");
			execSQL("INSERT INTO Test VALUES(NULL)");

			MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Test", conn);
			MySqlCommandBuilder cb = new MySqlCommandBuilder(da);
			DataSet ds = new DataSet();
			da.Fill(ds);
			Assert.AreEqual(1, ds.Tables[0].Rows[0]["id"]);
			DataRow row = ds.Tables[0].NewRow();
			ds.Tables[0].Rows.Add(row);

			try
			{
				da.Update(ds);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}

			ds.Clear();
			da.Fill(ds);
			Assert.AreEqual(1, ds.Tables[0].Rows[0]["id"]);
			Assert.AreEqual(2, ds.Tables[0].Rows[1]["id"]);
		}

		/// <summary>
		/// Bug #8292  	GROUP BY / WITH ROLLUP with DataSet causes System.Data.ConstraintException
		/// </summary>
		[Test]
		public void Rollup() 
		{
            if (Version < new Version(4, 1))
                return;

			execSQL("DROP TABLE IF EXISTS Test");
			execSQL("CREATE TABLE Test ( id INT NOT NULL, amount INT )");
			execSQL("INSERT INTO Test VALUES (1, 44)");
			execSQL("INSERT INTO Test VALUES (2, 88)");

			MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Test GROUP BY id WITH ROLLUP", conn);
			DataSet ds = new DataSet();
			da.Fill(ds);

			Assert.AreEqual(1, ds.Tables.Count);
			Assert.AreEqual(3, ds.Tables[0].Rows.Count);
			Assert.AreEqual(88, ds.Tables[0].Rows[2]["amount"]);
			Assert.AreEqual(DBNull.Value, ds.Tables[0].Rows[2]["id"]);
		}

		/// <summary>
		/// Bug #8514  	CURRENT_TIMESTAMP default not respected
		/// </summary>
/*		[Test]
		public void DefaultValues() 
		{
			execSQL("DROP TABLE IF EXISTS Test");
			execSQL("CREATE TABLE Test (id int, name VARCHAR(20) NOT NULL DEFAULT 'abc', dt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP)");
			
			MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Test", conn);
			MySqlCommand insCmd = new MySqlCommand("INSERT INTO Test VALUES (?id, ?name, ?dt)", conn);
			insCmd.Parameters.Add("?id", MySqlDbType.Int32, 0, "id");
			insCmd.Parameters.Add("?name", MySqlDbType.VarChar, 20, "name");
			insCmd.Parameters.Add("?dt", MySqlDbType.Datetime, 0, "dt");
			da.InsertCommand = insCmd;

			DataTable dt = new DataTable();

			//da.FillSchema(ds, SchemaType.Source);//, "Test");
			da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
			try 
			{
				da.Fill(dt);
			}
			catch (Exception ex) 
			{
				Console.WriteLine(ex.Message);
			}


			DataRow row = dt.NewRow();
			row["id"] = 1;
			row["name"] = "xyz";
			dt.Rows.Add(row);

			DataRow row2 = dt.NewRow();
			row2["id"] = 2;
			row2["name"] = DBNull.Value;
			dt.Rows.Add(row2);

			da.Update(dt);

			MySqlCommand cmd = new MySqlCommand("SELECT * FROM Test", conn);
			try 
			{
				using (MySqlDataReader reader = cmd.ExecuteReader()) 
				{
					Assert.IsTrue(reader.Read());
					Assert.AreEqual(1, reader["id"]);
					Assert.AreEqual("xyz", reader["name"]);
					Assert.AreEqual(DateTime.Now.Year, reader.GetDateTime(2).Year);
					Assert.IsTrue(reader.Read());
					Assert.AreEqual(2, reader["id"]);
					Assert.AreEqual("abc", reader["name"]);
					Assert.AreEqual(DateTime.Now.Year, reader.GetDateTime(2).Year);
					Assert.IsFalse(reader.Read());
				}
			}
			catch (Exception ex) 
			{
				Assert.Fail(ex.Message);
			}
		}
*/
        /// <summary>
        /// Bug #16307 @@Identity returning incorrect value 
        /// </summary>
        [Test]
        public void Bug16307()
        {
            execSQL("DROP TABLE IF EXISTS Test");
            execSQL("CREATE TABLE Test (OrgNum int auto_increment, CallReportNum int, Stamp varchar(50), " +
                "WasRealCall varchar(50), WasHangup varchar(50), primary key(orgnum))");

            string strSQL = "INSERT INTO Test(OrgNum, CallReportNum, Stamp, WasRealCall, WasHangup) " +
                "VALUES (?OrgNum, ?CallReportNum, ?Stamp, ?WasRealCall, ?WasHangup)";

            MySqlCommand cmd = new MySqlCommand(strSQL, conn);
            MySqlParameterCollection pc = cmd.Parameters;
        		
            pc.Add("?OrgNum", MySqlDbType.Int32, 0, "OrgNum");
            pc.Add("?CallReportNum", MySqlDbType.Int32, 0, "CallReportNum");
            pc.Add("?Stamp", MySqlDbType.VarChar, 0, "Stamp");
            pc.Add("?WasRealCall", MySqlDbType.VarChar, 0, "WasRealCall");
            pc.Add("?WasHangup", MySqlDbType.VarChar, 0, "WasHangup");

            MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Test", conn);
            da.InsertCommand = cmd;

            DataSet ds = new DataSet();
            da.Fill(ds);

            DataRow row = ds.Tables[0].NewRow();
            row["CallReportNum"] = 1;
            row["Stamp"] = "stamp";
            row["WasRealCall"] = "yes";
            row["WasHangup"] = "no";
            ds.Tables[0].Rows.Add(row);

            da.Update(ds.Tables[0]);

            strSQL = "SELECT @@IDENTITY AS 'Identity';";
            MySqlCommand cmd2 = new MySqlCommand(strSQL, conn);
            MySqlDataReader reader = null;
            try
            {
                reader = cmd2.ExecuteReader();
                reader.Read();
                int intCallNum = Int32.Parse(reader.GetValue(0).ToString());
                Assert.AreEqual(1, intCallNum);
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
        
        /// <summary>
        /// Bug #8131 Data Adapter doesn't close connection 
        /// </summary>
        [Test]
        public void QuietOpenAndClose()
        {
            execSQL("DROP TABLE IF EXISTS Test");
            execSQL("CREATE TABLE Test (id INT, PRIMARY KEY(id))");
            execSQL("INSERT INTO Test VALUES(1)");

            try
            {
                MySqlConnection c = new MySqlConnection(GetConnectionString(true));
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Test", c);
                MySqlCommandBuilder cb = new MySqlCommandBuilder(da);
                Assert.IsTrue(c.State == ConnectionState.Closed);
                DataTable dt = new DataTable();
                da.Fill(dt);
                Assert.IsTrue(c.State == ConnectionState.Closed);
                Assert.AreEqual(1, dt.Rows.Count);

                dt.Rows[0][0] = 2;
                DataRow[] rows = new DataRow[1];
                rows[0] = dt.Rows[0];
                da.Update(dt);
                Assert.IsTrue(c.State == ConnectionState.Closed);

                dt.Clear();
                c.Open();
                Assert.IsTrue(c.State == ConnectionState.Open);
                da.Fill(dt);
                Assert.IsTrue(c.State == ConnectionState.Open);
                Assert.AreEqual(1, dt.Rows.Count);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test]
        public void RangeFill()
        {
            execSQL("DROP TABLE IF EXISTS Test");
            execSQL("CREATE TABLE Test (id INT)");
            execSQL("INSERT INTO Test VALUES (1)");
            execSQL("INSERT INTO Test VALUES (2)");
            execSQL("INSERT INTO Test VALUES (3)");
            execSQL("INSERT INTO Test VALUES (4)");

            MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Test", conn);
            DataSet ds = new DataSet();
            da.Fill(ds, 1, 2, "Test");
        }

        [Test]
        public void FillWithNulls()
        {
            execSQL("DROP TABLE IF EXISTS Test");
            execSQL(@"CREATE TABLE Test (id INT UNSIGNED NOT NULL AUTO_INCREMENT, 
                      name VARCHAR(100), PRIMARY KEY(id))");

            MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Test", conn);
            MySqlCommandBuilder cb = new MySqlCommandBuilder(da);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dt.Columns[0].AutoIncrement = true;
            dt.Columns[0].AutoIncrementSeed = -1;
            dt.Columns[0].AutoIncrementStep = -1;
            DataRow row = dt.NewRow();
            row["name"] = "Test1";
            try
            {
                dt.Rows.Add(row);
                da.Update(dt);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            dt.Clear();
            da.Fill(dt);
            Assert.AreEqual(1, dt.Rows.Count);
            Assert.AreEqual(1, dt.Rows[0]["id"]);
            Assert.AreEqual("Test1", dt.Rows[0]["name"]);

            row = dt.NewRow();
            row["name"] = System.DBNull.Value;
            try
            {
                dt.Rows.Add(row);
                da.Update(dt);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            dt.Clear();
            da.Fill(dt);
            Assert.AreEqual(2, dt.Rows.Count);
            Assert.AreEqual(2, dt.Rows[1]["id"]);
            Assert.AreEqual(DBNull.Value, dt.Rows[1]["name"]);

            row = dt.NewRow();
            row["name"] = "Test3";
            try
            {
                dt.Rows.Add(row);
                da.Update(dt);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            dt.Clear();
            da.Fill(dt);
            Assert.AreEqual(3, dt.Rows.Count);
            Assert.AreEqual(3, dt.Rows[2]["id"]);
            Assert.AreEqual("Test3", dt.Rows[2]["name"]);
        }

        [Test]
        public void PagingFill()
        {
            execSQL("INSERT INTO Test (id, id2, name) VALUES (NULL, 1, 'Name 1')");
            execSQL("INSERT INTO Test (id, id2, name) VALUES (NULL, 2, 'Name 2')");
            execSQL("INSERT INTO Test (id, id2, name) VALUES (NULL, 3, 'Name 3')");
            execSQL("INSERT INTO Test (id, id2, name) VALUES (NULL, 4, 'Name 4')");
            execSQL("INSERT INTO Test (id, id2, name) VALUES (NULL, 5, 'Name 5')");
            execSQL("INSERT INTO Test (id, id2, name) VALUES (NULL, 6, 'Name 6')");
            execSQL("INSERT INTO Test (id, id2, name) VALUES (NULL, 7, 'Name 7')");
            execSQL("INSERT INTO Test (id, id2, name) VALUES (NULL, 8, 'Name 8')");
            execSQL("INSERT INTO Test (id, id2, name) VALUES (NULL, 9, 'Name 9')");
            execSQL("INSERT INTO Test (id, id2, name) VALUES (NULL, 10, 'Name 10')");
            execSQL("INSERT INTO Test (id, id2, name) VALUES (NULL, 11, 'Name 11')");

            MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Test", conn);
            DataTable dt = new DataTable();
            da.Fill(0, 10, dt);
            Assert.AreEqual(10, dt.Rows.Count);
        }

        private string MakeLargeString(int len)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(len);
            while (len-- > 0)
                sb.Append('a');
            return sb.ToString();
        }

        [Test]
        public void SkippingRowsLargerThan1024()
        {
            execSQL("DROP TABLE IF EXISTS Test");
            execSQL("CREATE TABLE Test (id INT, name TEXT)");

            MySqlCommand cmd = new MySqlCommand("INSERT INTO Test VALUES (?id, ?name)", conn);
            cmd.Parameters.Add("?id", MySqlDbType.Int32);
            cmd.Parameters.Add("?name", MySqlDbType.Text);
            for (int i = 0; i < 5; i++)
            {
                cmd.Parameters[0].Value = i;
                cmd.Parameters[1].Value = MakeLargeString(2000);
                cmd.ExecuteNonQuery();
            }

            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Test", conn);
                DataTable dt = new DataTable();
                da.Fill(0, 2, dt);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
