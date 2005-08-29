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


		[Test()]
		public void ShowCreateTable()
		{
			MySqlDataAdapter da = new MySqlDataAdapter("SHOW CREATE TABLE test", conn);
			DataTable dt = new DataTable();
			da.Fill(dt);

			Assert.AreEqual( 1, dt.Rows.Count );
			Assert.AreEqual( 2, dt.Columns.Count );
		}

		[Test()]
		[Category("4.1")]
		public void ProblemCharsInSQL()
		{
			execSQL("DROP TABLE IF EXISTS Test");
			execSQL("CREATE TABLE Test (id INT NOT NULL, name VARCHAR(250), mt MEDIUMTEXT, PRIMARY KEY(id)) CHAR SET utf8");

			MySqlCommand cmd = new MySqlCommand( "INSERT INTO Test VALUES (?id, ?text, ?mt)", conn);
			cmd.Parameters.Add( "?id", 1 );
			cmd.Parameters.Add( "?text", "This is my;test ? string–’‘’“”…" );
			cmd.Parameters.Add( "?mt", "My MT string: £" );
			cmd.ExecuteNonQuery();

			cmd.CommandText = "SELECT * FROM Test";
			MySqlDataReader reader = null;
			try 
			{
				reader = cmd.ExecuteReader();
				Assert.IsTrue( reader.Read() );
				Assert.AreEqual( 1, reader.GetInt32(0));
				if (Is40)
					Assert.AreEqual( "This is my;test ? string-'''\"\".", reader.GetString(1));
				else
					Assert.AreEqual( "This is my;test ? string–’‘’“”…", reader.GetString(1));
				Assert.AreEqual( "My MT string: £", reader.GetString(2) );
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
		public void LoadDataLocalInfile() 
		{
			execSQL("set @@global.max_allowed_packet=250000000");

			string connString = conn.ConnectionString + ";pooling=false";
			MySqlConnection c = new MySqlConnection( connString );
			c.Open();

			string path = Path.GetTempFileName();
			StreamWriter sw = new StreamWriter( path );
			for (int i=0; i < 2000000; i++) 
				sw.WriteLine(i + ",'Test'");
			sw.Flush();
			sw.Close();

			path = path.Replace(@"\", @"\\");
			MySqlCommand cmd = new MySqlCommand("LOAD DATA LOCAL INFILE '" + path + "' INTO TABLE Test FIELDS TERMINATED BY ','", c);

			object cnt = 0;
			try 
			{
				cnt = cmd.ExecuteNonQuery();
			}
			catch (Exception ex) 
			{
				Assert.Fail( ex.Message );
			}
			Assert.AreEqual( 2000000, cnt );

			cmd.CommandText = "SELECT COUNT(*) FROM Test";
			cnt = cmd.ExecuteScalar();
			Assert.AreEqual( 2000000, cnt );

			c.Close();
			execSQL("set @@global.max_allowed_packet=1047256");
		}

		[Test()]
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
				Assert.AreEqual( ConnectionState.Open, conn.State );
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
			createTable( sql, "MyISAM" );

			MySqlCommand cmd = new MySqlCommand("SELECT * FROM KLANT", conn);
			MySqlDataReader reader = null;
			try 
			{
				reader = cmd.ExecuteReader();
				while (reader.Read()) { }
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
		public void CharFunction() 
		{
			execSQL("DROP TABLE IF EXISTS Test");
			execSQL("CREATE TABLE Test (id tinyint,val1	tinyint,val2 tinyint)");
			execSQL("INSERT INTO Test VALUES (65,1,1),(65,1,1)");

			MySqlDataAdapter da = new MySqlDataAdapter("SELECT CHAR(id) FROM Test GROUP BY val1,val2", conn);
			DataTable dt = new DataTable();
			da.Fill(dt);
			Assert.AreEqual( "A", dt.Rows[0][0] );
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

		[Test]
		public void ForceWarnings() 
		{
			if (! Is41 && ! Is50) return;

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

	}
}
