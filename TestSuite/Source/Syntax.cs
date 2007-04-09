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
using System.Data;
using System.IO;
using NUnit.Framework;

namespace MySql.Data.MySqlClient.Tests
{
	[TestFixture]
	public class Syntax : BaseTest
	{
		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			Open();
		}

		[TestFixtureTearDown]
		public void FixtureTeardown()
		{
			Close();
		}

		[SetUp]
		protected override void Setup()
		{
			base.Setup ();
			execSQL("DROP TABLE IF EXISTS Test");
			execSQL("CREATE TABLE Test (id INT NOT NULL, name VARCHAR(250), PRIMARY KEY(id))");
		}


		[Test]
		public void ShowCreateTable()
		{
			MySqlDataAdapter da = new MySqlDataAdapter("SHOW CREATE TABLE test", conn);
			DataTable dt = new DataTable();
			da.Fill(dt);

			Assert.AreEqual(1, dt.Rows.Count);
			Assert.AreEqual(2, dt.Columns.Count);
		}

		[Test]
		[Category("4.1")]
		public void ProblemCharsInSQLUTF8()
		{
			execSQL("DROP TABLE IF EXISTS Test");
			execSQL("CREATE TABLE Test (id INT NOT NULL, name VARCHAR(250), mt MEDIUMTEXT, " +
					  "PRIMARY KEY(id)) CHAR SET utf8");

			MySqlConnection c = new MySqlConnection(GetConnectionString(true) + ";charset=utf8");
			c.Open();

			MySqlCommand cmd = new MySqlCommand("INSERT INTO Test VALUES (?id, ?text, ?mt)", c);
			cmd.Parameters.AddWithValue("?id", 1);
			cmd.Parameters.AddWithValue("?text", "This is my;test ? string–’‘’“”…");
			cmd.Parameters.AddWithValue("?mt", "My MT string: ?");
			cmd.ExecuteNonQuery();

			cmd.CommandText = "SELECT * FROM Test";
			MySqlDataReader reader = null;
			try
			{
				reader = cmd.ExecuteReader();
				Assert.IsTrue(reader.Read());
				Assert.AreEqual(1, reader.GetInt32(0));
				Assert.AreEqual("This is my;test ? string–’‘’“”…", reader.GetString(1));
				Assert.AreEqual("My MT string: ?", reader.GetString(2));
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}
			finally
			{
				if (reader != null) reader.Close();
				if (c != null) c.Close();
			}
		}


		[Test]
		public void ProblemCharsInSQL()
		{
			execSQL("DROP TABLE IF EXISTS Test");
			execSQL("CREATE TABLE Test (id INT NOT NULL, name VARCHAR(250), mt MEDIUMTEXT, " +
					  "PRIMARY KEY(id))");

			MySqlCommand cmd = new MySqlCommand("INSERT INTO Test VALUES (?id, ?text, ?mt)", conn);
			cmd.Parameters.AddWithValue("?id", 1);
			cmd.Parameters.AddWithValue("?text", "This is my;test ? string-'''\"\".");
			cmd.Parameters.AddWithValue("?mt", "My MT string: ?");
			cmd.ExecuteNonQuery();

			cmd.CommandText = "SELECT * FROM Test";
			MySqlDataReader reader = null;
			try
			{
				reader = cmd.ExecuteReader();
				Assert.IsTrue(reader.Read());
				Assert.AreEqual(1, reader.GetInt32(0));
				Assert.AreEqual("This is my;test ? string-'''\"\".", reader.GetString(1));
				Assert.AreEqual("My MT string: ?", reader.GetString(2));
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
		public void LoadDataLocalInfile() 
		{
			string connString = conn.ConnectionString + ";pooling=false";
			MySqlConnection c = new MySqlConnection(connString);
			c.Open();

            MySqlCommand cmd = new MySqlCommand("SET max_allowed_packet=250000000", c);
            cmd.ExecuteNonQuery();

			string path = Path.GetTempFileName();
			StreamWriter sw = new StreamWriter(path);
			for (int i = 0; i < 2000000; i++) 
				sw.WriteLine(i + ",'Test'");
			sw.Flush();
			sw.Close();

			path = path.Replace(@"\", @"\\");
			cmd.CommandText = "LOAD DATA LOCAL INFILE '" + path + "' INTO TABLE Test FIELDS TERMINATED BY ','";

			object cnt = 0;
			try 
			{
				cnt = cmd.ExecuteNonQuery();
			}
			catch (Exception ex) 
			{
				Assert.Fail(ex.Message);
			}
			Assert.AreEqual(2000000, cnt);

			cmd.CommandText = "SELECT COUNT(*) FROM Test";
			cnt = cmd.ExecuteScalar();
			Assert.AreEqual(2000000, cnt);

			c.Close();
		}

		[Test]
		public void ShowTablesInNonExistentDb() 
		{
			MySqlCommand cmd = new MySqlCommand("SHOW TABLES FROM dummy", conn);
			MySqlDataReader reader =null;
			try 
			{
				reader = cmd.ExecuteReader();
				Assert.Fail("ExecuteReader should not succeed");
			}
			catch (MySqlException) 
			{
				Assert.AreEqual(ConnectionState.Open, conn.State);
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
		public void Bug6135() 
		{
			execSQL("DROP TABLE IF EXISTS KLANT");
			string sql = "CREATE TABLE `KLANT` (`KlantNummer` int(11) NOT NULL auto_increment, " +
				"`Username` varchar(50) NOT NULL default '', `Password` varchar(100) NOT NULL default '', " + 
				"`Naam` varchar(100) NOT NULL default '', `Voornaam` varchar(100) NOT NULL default '', " +
				"`Straat` varchar(100) NOT NULL default '', `StraatNr` varchar(10) NOT NULL default '', " +
				"`Gemeente` varchar(100) NOT NULL default '', `Postcode` varchar(10) NOT NULL default '', " +
				"`DefaultMail` varchar(255) default '', 	`BtwNr` varchar(50) default '', " + 
				"`ReceiveMail` tinyint(1) NOT NULL default '0',	`Online` tinyint(1) NOT NULL default '0', " +
				"`LastVisit` timestamp(14) NOT NULL, `Categorie` int(11) NOT NULL default '0', " +
				"PRIMARY KEY  (`KlantNummer`),	UNIQUE KEY `UniqueUsername` (`Username`), " +
				"UNIQUE KEY `UniqueDefaultMail` (`DefaultMail`)	)";
			createTable(sql, "MyISAM");

			MySqlCommand cmd = new MySqlCommand("SELECT * FROM KLANT", conn);
			MySqlDataReader reader = null;
			try 
			{
				reader = cmd.ExecuteReader();
				while (reader.Read()) { }
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
		public void CharFunction() 
		{
			execSQL("DROP TABLE IF EXISTS Test");
			execSQL("CREATE TABLE Test (id tinyint,val1	tinyint,val2 tinyint)");
			execSQL("INSERT INTO Test VALUES (65,1,1),(65,1,1)");

			MySqlDataAdapter da = new MySqlDataAdapter("SELECT CHAR(id) FROM Test GROUP BY val1,val2", conn);
			DataTable dt = new DataTable();
			da.Fill(dt);
            Assert.IsTrue(dt.Rows[0][0].GetType() == typeof(string));
			Assert.AreEqual("A", dt.Rows[0][0]);
		}

		[Test]
		public void Sum()
		{
			execSQL("DROP TABLE IF EXISTS test");

			execSQL("CREATE TABLE test (field1 mediumint(9) default '0', field2 float(9,3) " +
				"default '0.000', field3 double(15,3) default '0.000') engine=innodb ");
			execSQL("INSERT INTO test values (1,1,1)");

			MySqlDataReader reader = null;

			MySqlCommand cmd2 = new MySqlCommand("SELECT sum(field2) FROM test", conn);
			try 
			{
				reader = cmd2.ExecuteReader();
				reader.Read();
				object o = reader[0];
				Assert.AreEqual(1, o);
			}
			catch (Exception ex) 
			{
				Assert.Fail(ex.Message);
			}
			finally 
			{
				if (reader != null) reader.Close();
				reader = null;
			}

			execSQL("DROP TABLE IF EXISTS test");
			execSQL("CREATE TABLE Test (id int, count int)");
			execSQL("INSERT INTO Test VALUES (1, 21)");
			execSQL("INSERT INTO Test VALUES (1, 33)");
			execSQL("INSERT INTO Test VALUES (1, 16)");
			execSQL("INSERT INTO Test VALUES (1, 40)");

			MySqlCommand cmd = new MySqlCommand("SELECT id, SUM(count) FROM Test GROUP BY id", conn);
			try 
			{
				reader = cmd.ExecuteReader();
				reader.Read();
				Assert.AreEqual( 1, reader.GetInt32(0) );
				Assert.AreEqual( 110, reader.GetDouble(1) );
			}
			catch (Exception ex) 
			{
				Assert.Fail( ex.Message );
			}
			finally 
			{
				if (reader != null)
					reader.Close();
			}
		}

		[Category("4.1")]
		[Test]
		public void ForceWarnings() 
		{
			MySqlCommand cmd = new MySqlCommand("SELECT * FROM test; DROP TABLE IF EXISTS test2; SELECT * FROM test", conn);
			MySqlDataReader reader = null; 
			try 
			{
				reader = cmd.ExecuteReader();
				while (reader.NextResult()) { }
			}
			catch( Exception ex) 
			{
				Assert.Fail( ex.Message );
			}
			finally 
			{
				if (reader != null) reader.Close();
			}
		}

		[Test]
		public void SettingAutoIncrementColumns() 
		{
			execSQL("DROP TABLE IF EXISTS Test");
			execSQL("CREATE TABLE Test (id int auto_increment, name varchar(100), primary key(id))");
			execSQL("INSERT INTO Test VALUES (1, 'One')");
			execSQL("INSERT INTO Test VALUES (3, 'Two')");

			MySqlCommand cmd = new MySqlCommand("SELECT name FROM Test WHERE id=1", conn);
			object name = cmd.ExecuteScalar();
			Assert.AreEqual( "One", name );

			cmd.CommandText = "SELECT name FROM Test WHERE id=3";
			name = cmd.ExecuteScalar();
			Assert.AreEqual( "Two", name );

			try 
			{
				execSQL("INSERT INTO Test (id, name2) values (5, 'Three')");
				Assert.Fail( "This should have failed" );
			}
			catch (MySqlException) 	{}
			catch (Exception ex) { Assert.Fail( ex.Message); }
		}

		/// <summary>
		/// Bug #16645 FOUND_ROWS() Bug 
		/// </summary>
		[Test]
		public void FoundRows()
		{
			execSQL("DROP TABLE IF EXISTS test");
			execSQL("CREATE TABLE test (testID int(11) NOT NULL auto_increment, testName varchar(100) default '', " +
				    "PRIMARY KEY  (testID)) ENGINE=InnoDB DEFAULT CHARSET=latin1");
			MySqlCommand cmd = new MySqlCommand("INSERT INTO test VALUES (NULL, 'test')", conn);
			for (int i=0; i < 1000; i++)
				cmd.ExecuteNonQuery();
			cmd.CommandText = "SELECT SQL_CALC_FOUND_ROWS * FROM test LIMIT 0, 10";
			cmd.ExecuteNonQuery();
			cmd.CommandText = "SELECT FOUND_ROWS()";
			object cnt = cmd.ExecuteScalar();
			Assert.AreEqual(1000, cnt);
		}

        [Test]
        public void AutoIncrement()
        {
            execSQL("DROP TABLE IF EXISTS test");
            execSQL("CREATE TABLE test (testID int(11) NOT NULL auto_increment, testName varchar(100) default '', " +
                    "PRIMARY KEY  (testID)) ENGINE=InnoDB DEFAULT CHARSET=latin1");
            MySqlCommand cmd = new MySqlCommand("INSERT INTO test VALUES (NULL, 'test')", conn);
            cmd.ExecuteNonQuery();
            cmd.CommandText = "SELECT @@IDENTITY as 'Identity'";
            MySqlDataReader reader = null;
            try
            {
                reader = cmd.ExecuteReader();
                reader.Read();
                int ident = Int32.Parse(reader.GetValue(0).ToString());
                Assert.AreEqual(1, ident);
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
        /// Bug #21521 # Symbols not allowed in column/table names. 
        /// </summary>
        [Test]
        public void CommentSymbolInTableName()
        {
            try
            {
                execSQL("DROP TABLE IF EXISTS test");
                execSQL("CREATE TABLE test (`PO#` int(11) NOT NULL auto_increment, " +
                    "`PODate` date default NULL, PRIMARY KEY  (`PO#`))");
                execSQL("INSERT INTO test ( `PO#`, `PODate` ) " +
                    "VALUES ( NULL, '2006-01-01' )");

                string sql = "SELECT `PO#` AS PurchaseOrderNumber, " +
                    "`PODate` AS OrderDate FROM  test";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                Assert.AreEqual(1, dt.Rows.Count);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        /// <summary>
        /// Bug #25178 Addition message in error 
        /// </summary>
        [Test]
        public void ErrorMessage()
        {
            MySqlCommand cmd = new MySqlCommand("SELEKT NOW() as theTime", conn);
            try
            {
                object o = cmd.ExecuteScalar();
            }
            catch (MySqlException ex)
            {
                string s = ex.Message;
                Assert.IsFalse(s.StartsWith("#"));
            }
        }

        /// <summary>
        /// Bug #27221 describe SQL command returns all byte array on MySQL versions older than 4.1.15 
        /// </summary>
        [Test]
        public void Describe()
        {
            MySqlDataAdapter da = new MySqlDataAdapter("DESCRIBE test", conn);
            DataTable dt = new DataTable();
            da.Fill(dt);

            Assert.IsTrue(dt.Rows[0][0].GetType() == typeof(string));
            Assert.IsTrue(dt.Rows[0][1].GetType() == typeof(string));
            Assert.IsTrue(dt.Rows[0][2].GetType() == typeof(string));
            Assert.IsTrue(dt.Rows[0][3].GetType() == typeof(string));
            Assert.IsTrue(dt.Rows[0][4].GetType() == typeof(string));
            Assert.IsTrue(dt.Rows[0][5].GetType() == typeof(string));
        }

        [Test]
        public void ShowTableStatus()
        {
            MySqlDataAdapter da = new MySqlDataAdapter(
                String.Format("SHOW TABLE STATUS FROM {0} LIKE 'test'",
                databases[0]), conn);
            DataTable dt = new DataTable();
            da.Fill(dt);

            Assert.IsTrue(dt.Rows[0][0].GetType() == typeof(string));
        }

        /// <summary>
        /// Bug #26960 Connector .NET 5.0.5 / Visual Studio Plugin 1.1.2 
        /// </summary>
        [Test]
        public void NullAsAType()
        {
            MySqlDataAdapter da = new MySqlDataAdapter(
                @"SELECT 'localhost' as SERVER_NAME, 
                null as CATALOG_NAME, database() as SCHEMA_NAME", conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            Assert.IsTrue(dt.Rows[0][0].GetType() == typeof(string));
            Assert.AreEqual(DBNull.Value, dt.Rows[0][1]);
            Assert.IsTrue(dt.Rows[0][2].GetType() == typeof(string));
        }
    }
}
