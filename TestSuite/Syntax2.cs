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
using System.IO;
using NUnit.Framework;

namespace MySql.Data.MySqlClient.Tests
{
	[TestFixture]
	public class Syntax2 : BaseTest
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
        public void CommentsInSQL()
        {
            string sql = "INSERT INTO Test /* my table */ VALUES (1 /* this is the id */, 'Test' );" +
                "/* These next inserts are just for testing \r\n" +
                "   comments */\r\n" +
                "INSERT INTO \r\n" +
                "  # This table is bogus\r\n" +
                "test VALUES (2, 'Test2')";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.ExecuteNonQuery();

            MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Test", conn);
            DataTable table = new DataTable();
            da.Fill(table);
            Assert.AreEqual(1, table.Rows[0]["id"]);
            Assert.AreEqual("Test", table.Rows[0]["name"]);
            Assert.AreEqual(2, table.Rows.Count);
            Assert.AreEqual(2, table.Rows[1]["id"]);
            Assert.AreEqual("Test2", table.Rows[1]["name"]);
        }

        [Test]
        public void LastInsertid()
        {
            execSQL("DROP TABLE test");
            execSQL("CREATE TABLE test(id int auto_increment, name varchar(20), primary key(id))");
            MySqlCommand cmd = new MySqlCommand("INSERT INTO test VALUES(NULL, 'test')", conn);
            cmd.ExecuteNonQuery();
            Assert.AreEqual(1, cmd.LastInsertedId);

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
            Assert.AreEqual(2, cmd.LastInsertedId);

            cmd.CommandText = "SELECT id FROM test";
            cmd.ExecuteScalar();
            Assert.AreEqual(-1, cmd.LastInsertedId);
        }
    }
}
