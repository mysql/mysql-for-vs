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
	public class DataAdapterTests : BaseTest
	{

		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			Open();
		}

		[SetUp]
		protected override void Setup()
		{
			base.Setup();

			execSQL("DROP TABLE IF EXISTS Test");
			execSQL("CREATE TABLE Test (id INT NOT NULL AUTO_INCREMENT, id2 INT NOT NULL, name VARCHAR(100), dt DATETIME, tm TIME, ts TIMESTAMP, OriginalId INT, PRIMARY KEY(id, id2))");
		}


		[Test()]
		public void TestFill()
		{
			FillImpl( false );
			if (Is41 || Is50) FillImpl(true);
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
			da.Fill( ds, "Test" );

			Assert.AreEqual( 1, ds.Tables.Count );
			Assert.AreEqual( 3, ds.Tables[0].Rows.Count );

			Assert.AreEqual( 1, ds.Tables[0].Rows[0]["id"] );
			Assert.AreEqual( 2, ds.Tables[0].Rows[1]["id"] );
			Assert.AreEqual( 3, ds.Tables[0].Rows[2]["id"] );

			Assert.AreEqual( 1, ds.Tables[0].Rows[0]["id2"] );
			Assert.AreEqual( 2, ds.Tables[0].Rows[1]["id2"] );
			Assert.AreEqual( 3, ds.Tables[0].Rows[2]["id2"] );

			Assert.AreEqual( "Name 1", ds.Tables[0].Rows[0]["name"] );
			Assert.AreEqual( DBNull.Value, ds.Tables[0].Rows[1]["name"] );
			Assert.AreEqual( String.Empty, ds.Tables[0].Rows[2]["name"] );
		}

		[Test()]
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
			dt.Rows.Add( dr );

			int count = da.Update(dt);

			// make sure our refresh of auto increment values worked
			Assert.AreEqual( 1, count, "checking insert count" );
			Assert.IsNotNull( dt.Rows[ dt.Rows.Count-1 ]["id"], "Checking auto increment column" );

			dt.Rows[0]["id2"] = 3;
			dt.Rows[0]["name"] = "TestName2";
			dt.Rows[0]["ts"] = DBNull.Value;
			DateTime day1 = new DateTime(2003, 1, 16, 12, 24, 0);
			dt.Rows[0]["dt"] = day1;
			dt.Rows[0]["tm"] = day1.TimeOfDay;
			count = da.Update( dt );

			Assert.IsNotNull( dt.Rows[0]["ts"], "checking refresh of record" );
			Assert.AreEqual( 3, dt.Rows[0]["id2"], "checking refresh of primary column" );

			dt.Rows.Clear();
			da.Fill( dt );

			Assert.AreEqual( 1, count, "checking update count" );
			DateTime dateTime = (DateTime)dt.Rows[0]["dt"];
			Assert.AreEqual( day1, dateTime, "checking date" );
			Assert.AreEqual( day1.TimeOfDay, dt.Rows[0]["tm"], "checking time" );

			dt.Rows[0].Delete();
			count = da.Update( dt );

			Assert.AreEqual( 1, count, "checking insert count" );

			dt.Rows.Clear();
			da.Fill(dt);
			Assert.AreEqual( 0, dt.Rows.Count, "checking row count" );
		}

		[Test()]
		public void OriginalInName()
		{
			MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Test", conn);
			MySqlCommandBuilder cb = new MySqlCommandBuilder(da);
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

		[Test()]
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

		[Test()]
		public void UpdateNullTextFieldToEmptyString() 
		{
			execSQL("INSERT INTO Test (id, id2, name) VALUES (1, 1, NULL)");

			try 
			{
				MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Test", conn);
				MySqlCommandBuilder cb = new MySqlCommandBuilder(da);

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

		[Test()]
		public void UpdateExtendedTextFields() 
		{
			execSQL("DROP TABLE IF EXISTS Test");
			execSQL( "CREATE TABLE Test (id int, notes MEDIUMTEXT, PRIMARY KEY(id))" );
			execSQL("INSERT INTO Test VALUES(1, 'This is my note')" );

			MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Test", conn);
			MySqlCommandBuilder cb = new MySqlCommandBuilder(da);
			DataTable dt = new DataTable();
			da.Fill(dt);

			dt.Rows[0]["notes"] = "This is my new note";
			da.Update( dt );

			dt.Clear();
			da.Fill(dt);
			Assert.AreEqual( "This is my new note", dt.Rows[0]["notes"]);
		}

		[Test()]
		public void SelectMoreThan252Rows() 
		{
			for (int i=0; i < 500; i++) 
				execSQL("INSERT INTO Test(id, id2) VALUES(NULL, " + i + ")");

			MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Test", conn);
			DataTable dt = new DataTable();
			da.Fill(dt);

			Assert.AreEqual( 500, dt.Rows.Count );
		}

		[Test()]
		[NUnit.Framework.Explicit()]
		public void UpdateManyRows() 
		{
			MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM test", conn);
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
			Assert.AreEqual( 100000, dt.Rows.Count );
		}

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
			DataTable dt = new DataTable();
			da.Fill( dt );

			Assert.AreEqual( String.Empty, dt.Rows[0]["name"] );

			dt.Rows[0]["name"] = "Test";
			da.Update( dt );

			dt.Clear();
			da.Fill(dt);
			Assert.AreEqual( "Test", dt.Rows[0]["name"] );
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

			MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Test", conn);
			MySqlCommandBuilder cb = new MySqlCommandBuilder(da);
			DataTable dt = new DataTable();
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
	}
}
