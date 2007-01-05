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

namespace MySql.Data.MySqlClient.Tests
{
	[NUnit.Framework.TestFixture]
	public class GetSchemaTests : BaseTest
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

        [Test]
        public void Collections()
        {
            DataTable dt = conn.GetSchema();

            Assert.AreEqual("MetaDataCollections", dt.Rows[0][0]);
            Assert.AreEqual("DataSourceInformation", dt.Rows[1][0]);
            Assert.AreEqual("DataTypes", dt.Rows[2][0]);
            Assert.AreEqual("Restrictions", dt.Rows[3][0]);
            Assert.AreEqual("ReservedWords", dt.Rows[4][0]);
            Assert.AreEqual("Databases", dt.Rows[5][0]);
            Assert.AreEqual("Tables", dt.Rows[6][0]);
            Assert.AreEqual("Columns", dt.Rows[7][0]);
            Assert.AreEqual("Users", dt.Rows[8][0]);
            Assert.AreEqual("Foreign Keys", dt.Rows[9][0]);
            Assert.AreEqual("IndexColumns", dt.Rows[10][0]);
            Assert.AreEqual("Indexes", dt.Rows[11][0]);

            if (Is50)
            {
                Assert.AreEqual("Views", dt.Rows[12][0]);
                Assert.AreEqual("ViewColumns", dt.Rows[13][0]);
                Assert.AreEqual("Procedure Parameters", dt.Rows[14][0]);
                Assert.AreEqual("Procedures", dt.Rows[15][0]);
                Assert.AreEqual("Triggers", dt.Rows[16][0]);
            }
        }

        [Test]
        public void Databases()
        {
            DataTable dt = conn.GetSchema("Databases");
            Assert.AreEqual("Databases", dt.TableName);

            bool foundZero = false;
            bool foundOne = false;
            foreach (DataRow row in dt.Rows)
            {
                string dbName = row[1].ToString().ToLower();
                if (dbName == databases[0])
                    foundZero = true;
                else if (dbName == databases[1])
                    foundOne = true;
            }
            Assert.IsTrue(foundZero);
            Assert.IsTrue(foundOne);

            dt = conn.GetSchema("Databases", new string[1] { databases[0] });
            Assert.AreEqual(1, dt.Rows.Count);
            Assert.AreEqual(databases[0], dt.Rows[0][1].ToString().ToLower());
        }

        [Test]
        public void Tables()
        {
            execSQL("DROP TABLE IF EXISTS test1");
            execSQL("CREATE TABLE test1 (id int)");

            string[] restrictions = new string[4];
            restrictions[1] = databases[0];
            restrictions[2] = "test1";
            DataTable dt = conn.GetSchema("Tables", restrictions);
            Assert.IsTrue(dt.Rows.Count == 1);
            Assert.AreEqual("Tables", dt.TableName);
            Assert.AreEqual("test1", dt.Rows[0][2]);
        }

        [Test]
        public void Columns()
        {
            execSQL("DROP TABLE IF EXISTS test");
            execSQL("CREATE TABLE test (col1 int, col2 decimal(20,5), " +
                "col3 varchar(50) character set utf8, col4 tinyint unsigned)");

            string[] restrictions = new string[4];
            restrictions[1] = databases[0];
            restrictions[2] = "test";
            DataTable dt = conn.GetSchema("Columns", restrictions);
            Assert.IsTrue(dt.Rows.Count == 4);
            Assert.AreEqual("Columns", dt.TableName);
            
            // first column
            Assert.AreEqual(databases[0].ToUpper(), dt.Rows[0]["TABLE_SCHEMA"].ToString().ToUpper());
            Assert.AreEqual("COL1", dt.Rows[0]["COLUMN_NAME"].ToString().ToUpper());
            Assert.AreEqual(1, dt.Rows[0]["ORDINAL_POSITION"]);
            Assert.AreEqual("YES", dt.Rows[0]["IS_NULLABLE"]);
            Assert.AreEqual("INT", dt.Rows[0]["DATA_TYPE"].ToString().ToUpper());

            // second column
            Assert.AreEqual(databases[0].ToUpper(), dt.Rows[1]["TABLE_SCHEMA"].ToString().ToUpper());
            Assert.AreEqual("COL2", dt.Rows[1]["COLUMN_NAME"].ToString().ToUpper());
            Assert.AreEqual(2, dt.Rows[1]["ORDINAL_POSITION"]);
            Assert.AreEqual("YES", dt.Rows[1]["IS_NULLABLE"]);
            Assert.AreEqual("DECIMAL", dt.Rows[1]["DATA_TYPE"].ToString().ToUpper());
            Assert.AreEqual("DECIMAL(20,5)", dt.Rows[1]["COLUMN_TYPE"].ToString().ToUpper());
            Assert.AreEqual(20, dt.Rows[1]["NUMERIC_PRECISION"]);
            Assert.AreEqual(5, dt.Rows[1]["NUMERIC_SCALE"]);

            // third column
            Assert.AreEqual(databases[0].ToUpper(), dt.Rows[2]["TABLE_SCHEMA"].ToString().ToUpper());
            Assert.AreEqual("COL3", dt.Rows[2]["COLUMN_NAME"].ToString().ToUpper());
            Assert.AreEqual(3, dt.Rows[2]["ORDINAL_POSITION"]);
            Assert.AreEqual("YES", dt.Rows[2]["IS_NULLABLE"]);
            Assert.AreEqual("VARCHAR", dt.Rows[2]["DATA_TYPE"].ToString().ToUpper());
            Assert.AreEqual("VARCHAR(50)", dt.Rows[2]["COLUMN_TYPE"].ToString().ToUpper());

            // fourth column
            Assert.AreEqual(databases[0].ToUpper(), dt.Rows[3]["TABLE_SCHEMA"].ToString().ToUpper());
            Assert.AreEqual("COL4", dt.Rows[3]["COLUMN_NAME"].ToString().ToUpper());
            Assert.AreEqual(4, dt.Rows[3]["ORDINAL_POSITION"]);
            Assert.AreEqual("YES", dt.Rows[3]["IS_NULLABLE"]);
            Assert.AreEqual("TINYINT", dt.Rows[3]["DATA_TYPE"].ToString().ToUpper());
        }

        [Category("5.0")]
        [Test]
        public void Procedures()
        {
            execSQL("DROP PROCEDURE IF EXISTS spTest");
            execSQL("CREATE PROCEDURE spTest (id int) BEGIN SELECT 1; END");

            string[] restrictions = new string[4];
            restrictions[1] = databases[0];
            restrictions[2] = "spTest";
            DataTable dt = conn.GetSchema("Procedures", restrictions);
            Assert.IsTrue(dt.Rows.Count == 1);
            Assert.AreEqual("Procedures", dt.TableName);
            Assert.AreEqual("spTest", dt.Rows[0][3]);
        }

        [Category("5.0")]
        [Test]
        public void Functions()
        {
            execSQL("DROP FUNCTION IF EXISTS spFunc");
            execSQL("CREATE FUNCTION spFunc (id int) RETURNS INT BEGIN RETURN 1; END");

            string[] restrictions = new string[4];
            restrictions[1] = databases[0];
            restrictions[2] = "spFunc";
            DataTable dt = conn.GetSchema("Procedures", restrictions);
            Assert.IsTrue(dt.Rows.Count == 1);
            Assert.AreEqual("Procedures", dt.TableName);
            Assert.AreEqual("spFunc", dt.Rows[0][3]);
        }

        [Category("5.0")]
        [Test]
        public void ProcedureParameters()
        {
            execSQL("DROP PROCEDURE IF EXISTS spTest");
            execSQL("CREATE PROCEDURE spTest (id int, name varchar(50)) BEGIN SELECT 1; END");

            string[] restrictions = new string[5];
            restrictions[1] = databases[0];
            restrictions[2] = "spTest";
            DataTable dt = conn.GetSchema("Procedure Parameters", restrictions);
            Assert.IsTrue(dt.Rows.Count == 2);
            Assert.AreEqual("Procedure Parameters", dt.TableName);
            Assert.AreEqual(databases[0].ToLower(), dt.Rows[0]["ROUTINE_SCHEMA"].ToString().ToLower());
            Assert.AreEqual("sptest", dt.Rows[0]["ROUTINE_NAME"].ToString().ToLower());
            Assert.AreEqual("?id", dt.Rows[0]["PARAMETER_NAME"].ToString().ToLower());
            Assert.AreEqual(1, dt.Rows[0]["ORDINAL_POSITION"]);
            Assert.AreEqual("IN", dt.Rows[0]["PARAMETER_MODE"]);
            Assert.AreEqual("NO", dt.Rows[0]["IS_RESULT"]);

            restrictions[4] = "?name";
            dt.Clear();
            dt = conn.GetSchema("Procedure Parameters", restrictions);
            Assert.AreEqual(1, dt.Rows.Count);
            Assert.AreEqual(databases[0].ToLower(), dt.Rows[0]["ROUTINE_SCHEMA"].ToString().ToLower());
            Assert.AreEqual("sptest", dt.Rows[0]["ROUTINE_NAME"].ToString().ToLower());
            Assert.AreEqual("?name", dt.Rows[0]["PARAMETER_NAME"].ToString().ToLower());
            Assert.AreEqual(2, dt.Rows[0]["ORDINAL_POSITION"]);
            Assert.AreEqual("IN", dt.Rows[0]["PARAMETER_MODE"]);
            Assert.AreEqual("NO", dt.Rows[0]["IS_RESULT"]);

            execSQL("DROP FUNCTION IF EXISTS spFunc");
            execSQL("CREATE FUNCTION spFunc (id int) RETURNS INT BEGIN RETURN 1; END");

            restrictions[4] = null;
            restrictions[1] = databases[0];
            restrictions[2] = "spFunc";
            dt = conn.GetSchema("Procedure Parameters", restrictions);
            Assert.IsTrue(dt.Rows.Count == 2);
            Assert.AreEqual("Procedure Parameters", dt.TableName);
            Assert.AreEqual(databases[0].ToLower(), dt.Rows[0]["ROUTINE_SCHEMA"].ToString().ToLower());
            Assert.AreEqual("spfunc", dt.Rows[0]["ROUTINE_NAME"].ToString().ToLower());
            Assert.AreEqual("?id", dt.Rows[0]["PARAMETER_NAME"].ToString().ToLower());
            Assert.AreEqual(1, dt.Rows[0]["ORDINAL_POSITION"]);
            Assert.AreEqual("IN", dt.Rows[0]["PARAMETER_MODE"]);
            Assert.AreEqual("NO", dt.Rows[0]["IS_RESULT"]);

            Assert.AreEqual(databases[0].ToLower(), dt.Rows[1]["ROUTINE_SCHEMA"].ToString().ToLower());
            Assert.AreEqual("spfunc", dt.Rows[1]["ROUTINE_NAME"].ToString().ToLower());
            Assert.AreEqual(0, dt.Rows[1]["ORDINAL_POSITION"]);
            Assert.AreEqual("YES", dt.Rows[1]["IS_RESULT"]);
        }

        [Category("5.0")]
        [Test]
        public void Indexes()
        {
            execSQL("DROP TABLE IF EXISTS test");
            execSQL("CREATE TABLE test (id int, PRIMARY KEY(id))");
            string[] restrictions = new string[4];
            restrictions[2] = "test";
            restrictions[1] = databases[0];
            DataTable dt = conn.GetSchema("Indexes", restrictions);
            Assert.AreEqual(1, dt.Rows.Count);
            Assert.AreEqual("test", dt.Rows[0]["TABLE_NAME"]);
            Assert.AreEqual(true, dt.Rows[0]["PRIMARY"]);
            Assert.AreEqual(true, dt.Rows[0]["UNIQUE"]);

            execSQL("DROP TABLE IF EXISTS test");
            execSQL("CREATE TABLE test (id int, name varchar(50), " +
                "UNIQUE KEY key2 (name))");

            dt = conn.GetSchema("Indexes", restrictions);
            Assert.AreEqual(1, dt.Rows.Count);
            Assert.AreEqual("test", dt.Rows[0]["TABLE_NAME"]);
            Assert.AreEqual("key2", dt.Rows[0]["INDEX_NAME"]);
            Assert.AreEqual(false, dt.Rows[0]["PRIMARY"]);
            Assert.AreEqual(true, dt.Rows[0]["UNIQUE"]);

            execSQL("DROP TABLE IF EXISTS test");
            execSQL("CREATE TABLE test (id int, name varchar(50), " +
                "KEY key2 (name))");

            dt = conn.GetSchema("Indexes", restrictions);
            Assert.AreEqual(1, dt.Rows.Count);
            Assert.AreEqual("test", dt.Rows[0]["TABLE_NAME"]);
            Assert.AreEqual("key2", dt.Rows[0]["INDEX_NAME"]);
            Assert.AreEqual(false, dt.Rows[0]["PRIMARY"]);
            Assert.AreEqual(false, dt.Rows[0]["UNIQUE"]);
        }

        [Test]
        public void IndexColumns()
        {
            execSQL("DROP TABLE IF EXISTS test");
            execSQL("CREATE TABLE test (id int, PRIMARY KEY(id))");
            string[] restrictions = new string[5];
            restrictions[2] = "test";
            restrictions[1] = databases[0];
            DataTable dt = conn.GetSchema("IndexColumns", restrictions);
            Assert.AreEqual(1, dt.Rows.Count);
            Assert.AreEqual("test", dt.Rows[0]["TABLE_NAME"]);
            Assert.AreEqual("id", dt.Rows[0]["COLUMN_NAME"]);

            execSQL("DROP TABLE IF EXISTS test");
            execSQL("CREATE TABLE test (id int, id1 int, id2 int, " +
                "INDEX key1 (id1, id2))");
            restrictions[2] = "test";
            restrictions[1] = databases[0];
            restrictions[4] = "id2";
            dt = conn.GetSchema("IndexColumns", restrictions);
            Assert.AreEqual(1, dt.Rows.Count);
            Assert.AreEqual("test", dt.Rows[0]["TABLE_NAME"]);
            Assert.AreEqual("id2", dt.Rows[0]["COLUMN_NAME"]);
            Assert.AreEqual(2, dt.Rows[0]["ORDINAL_POSITION"]);
        }

        [Category("5.0")]
        [Test]
        public void Views()
        {
            execSQL("DROP VIEW IF EXISTS vw");
            execSQL("CREATE VIEW vw AS SELECT Now() as theTime");

            string[] restrictions = new string[4];
            restrictions[1] = databases[0];
            restrictions[2] = "vw";
            DataTable dt = conn.GetSchema("Views", restrictions);
            Assert.IsTrue(dt.Rows.Count == 1);
            Assert.AreEqual("Views", dt.TableName);
            Assert.AreEqual("vw", dt.Rows[0]["TABLE_NAME"]);
        }

        [Category("5.0")]
        [Test]
        public void ViewColumns()
        {
            execSQL("DROP VIEW IF EXISTS vw");
            execSQL("CREATE VIEW vw AS SELECT Now() as theTime");

            string[] restrictions = new string[4];
            restrictions[1] = databases[0];
            restrictions[2] = "vw";
            DataTable dt = conn.GetSchema("ViewColumns", restrictions);
            Assert.IsTrue(dt.Rows.Count == 1);
            Assert.AreEqual("ViewColumns", dt.TableName);
            Assert.AreEqual(databases[0].ToLower(), dt.Rows[0]["VIEW_SCHEMA"].ToString().ToLower());
            Assert.AreEqual("vw", dt.Rows[0]["VIEW_NAME"]);
            Assert.AreEqual("theTime", dt.Rows[0]["COLUMN_NAME"]);
        }

        [Test]
        [Category("5.0")]
        public void SingleProcedureParameters()
        {
            execSQL("DROP PROCEDURE IF EXISTS spTest");
            execSQL("CREATE PROCEDURE spTest(id int, IN id2 INT(11), " +
                "INOUT io1 VARCHAR(20), OUT out1 FLOAT) BEGIN END");
            string[] restrictions = new string[4];
            restrictions[1] = databases[0];
            restrictions[2] = "spTest";
            DataTable procs = conn.GetSchema("PROCEDURES", restrictions);
            Assert.AreEqual(1, procs.Rows.Count);
            Assert.AreEqual("spTest", procs.Rows[0][0]);
            Assert.AreEqual(databases[0].ToLower(), procs.Rows[0][2].ToString().ToLower());
            Assert.AreEqual("spTest", procs.Rows[0][3]);

            DataTable parameters = conn.GetSchema("PROCEDURE PARAMETERS", restrictions);
            Assert.AreEqual(4, parameters.Rows.Count);
            Assert.AreEqual(DBNull.Value, parameters.Rows[0][0]);
            Assert.AreEqual(DBNull.Value, parameters.Rows[1][0]);
            Assert.AreEqual(DBNull.Value, parameters.Rows[2][0]);
            Assert.AreEqual(DBNull.Value, parameters.Rows[3][0]);

            Assert.AreEqual(databases[0].ToLower(), parameters.Rows[0][1].ToString().ToLower());
            Assert.AreEqual(databases[0].ToLower(), parameters.Rows[1][1].ToString().ToLower());
            Assert.AreEqual(databases[0].ToLower(), parameters.Rows[2][1].ToString().ToLower());
            Assert.AreEqual(databases[0].ToLower(), parameters.Rows[3][1].ToString().ToLower());

            Assert.AreEqual("spTest", parameters.Rows[0][2]);
            Assert.AreEqual("spTest", parameters.Rows[1][2]);
            Assert.AreEqual("spTest", parameters.Rows[2][2]);
            Assert.AreEqual("spTest", parameters.Rows[3][2]);

            Assert.AreEqual("PROCEDURE", parameters.Rows[0][3]);
            Assert.AreEqual("?id", parameters.Rows[0][4]);
            Assert.AreEqual(1, parameters.Rows[0][5]);
            Assert.AreEqual("IN", parameters.Rows[0][6]);
            Assert.AreEqual("NO", parameters.Rows[0][7]);
            Assert.AreEqual("INT", parameters.Rows[0][8].ToString().ToUpper());

            Assert.AreEqual("PROCEDURE", parameters.Rows[1][3]);
            Assert.AreEqual("?id2", parameters.Rows[1][4]);
            Assert.AreEqual(2, parameters.Rows[1][5]);
            Assert.AreEqual("IN", parameters.Rows[1][6]);
            Assert.AreEqual("NO", parameters.Rows[1][7]);
            Assert.AreEqual("INT", parameters.Rows[1][8].ToString().ToUpper());

            Assert.AreEqual("PROCEDURE", parameters.Rows[2][3]);
            Assert.AreEqual("?io1", parameters.Rows[2][4]);
            Assert.AreEqual(3, parameters.Rows[2][5]);
            Assert.AreEqual("INOUT", parameters.Rows[2][6]);
            Assert.AreEqual("NO", parameters.Rows[2][7]);
            Assert.AreEqual("VARCHAR", parameters.Rows[2][8].ToString().ToUpper());

            Assert.AreEqual("PROCEDURE", parameters.Rows[3][3]);
            Assert.AreEqual("?out1", parameters.Rows[3][4]);
            Assert.AreEqual(4, parameters.Rows[3][5]);
            Assert.AreEqual("OUT", parameters.Rows[3][6]);
            Assert.AreEqual("NO", parameters.Rows[3][7]);
            Assert.AreEqual("FLOAT", parameters.Rows[3][8].ToString().ToUpper());
        }

        [Test]
        public void SingleForeignKey()
        {
            execSQL("DROP TABLE IF EXISTS child");
            execSQL("DROP TABLE IF EXISTS parent");
            execSQL("CREATE TABLE parent (id INT NOT NULL, PRIMARY KEY (id)) TYPE=INNODB");
            execSQL("CREATE TABLE child (id INT, parent_id INT, INDEX par_ind (parent_id), " +
                "CONSTRAINT c1 FOREIGN KEY (parent_id) REFERENCES parent(id) ON DELETE CASCADE) TYPE=INNODB");
            string[] restrictions = new string[4];
            restrictions[0] = null;
            restrictions[1] = databases[0];
            restrictions[2] = "child";
            DataTable dt = conn.GetSchema("Foreign Keys", restrictions);
            Assert.AreEqual(1, dt.Rows.Count);
            DataRow row = dt.Rows[0];
            Assert.AreEqual(DBNull.Value, row["CONSTRAINT_CATALOG"]);
            Assert.AreEqual(databases[0].ToLower(), row["CONSTRAINT_SCHEMA"].ToString().ToLower());
            Assert.AreEqual("c1", row["CONSTRAINT_NAME"]);
            Assert.AreEqual(DBNull.Value, row["TABLE_CATALOG"]);
            Assert.AreEqual(databases[0].ToLower(), row["TABLE_SCHEMA"].ToString().ToLower());
            Assert.AreEqual("child", row["TABLE_NAME"]);
            Assert.AreEqual("parent_id", row["COLUMN_NAME"]);
            Assert.AreEqual(0, row["ORDINAL_POSITION"]);
            Assert.AreEqual(databases[0].ToLower(), row["REFERENCED_TABLE_SCHEMA"].ToString().ToLower());
            Assert.AreEqual("parent", row["REFERENCED_TABLE_NAME"]);
            Assert.AreEqual("id", row["REFERENCED_COLUMN_NAME"]);
        }

        [Test]
        public void MultiSingleForeignKey()
        {
            execSQL("DROP TABLE IF EXISTS product_order");
            execSQL("DROP TABLE IF EXISTS product");
            execSQL("DROP TABLE IF EXISTS customer");
            execSQL("CREATE TABLE product (category INT NOT NULL, id INT NOT NULL, " +
                      "price DECIMAL, PRIMARY KEY(category, id)) TYPE=INNODB");
            execSQL("CREATE TABLE customer (id INT NOT NULL, PRIMARY KEY (id)) TYPE=INNODB");
            execSQL("CREATE TABLE product_order (no INT NOT NULL AUTO_INCREMENT, " +
                "product_category INT NOT NULL, product_id INT NOT NULL, customer_id INT NOT NULL, " +
                "PRIMARY KEY(no), INDEX (product_category, product_id), " +
                "FOREIGN KEY (product_category, product_id) REFERENCES product(category, id) " +
                "ON UPDATE CASCADE ON DELETE RESTRICT, INDEX (customer_id), " +
                "FOREIGN KEY (customer_id) REFERENCES customer(id)) TYPE=INNODB");

            string[] restrictions = new string[4];
            restrictions[0] = null;
            restrictions[1] = databases[0];
            restrictions[2] = "product_order";
            DataTable dt = conn.GetSchema("Foreign Keys", restrictions);
            Assert.AreEqual(3, dt.Rows.Count);
            DataRow row = dt.Rows[0];
            Assert.AreEqual(DBNull.Value, row["CONSTRAINT_CATALOG"]);
            Assert.AreEqual(databases[0].ToLower(), row["CONSTRAINT_SCHEMA"].ToString().ToLower());
            Assert.AreEqual("product_order_ibfk_1", row["CONSTRAINT_NAME"]);
            Assert.AreEqual(DBNull.Value, row["TABLE_CATALOG"]);
            Assert.AreEqual(databases[0].ToLower(), row["TABLE_SCHEMA"].ToString().ToLower());
            Assert.AreEqual("product_order", row["TABLE_NAME"]);
            Assert.AreEqual("product_category", row["COLUMN_NAME"]);
            Assert.AreEqual(0, row["ORDINAL_POSITION"]);
            Assert.AreEqual(databases[0].ToLower(), row["REFERENCED_TABLE_SCHEMA"].ToString().ToLower());
            Assert.AreEqual("product", row["REFERENCED_TABLE_NAME"]);
            Assert.AreEqual("category", row["REFERENCED_COLUMN_NAME"]);

            row = dt.Rows[1];
            Assert.AreEqual(DBNull.Value, row["CONSTRAINT_CATALOG"]);
            Assert.AreEqual(databases[0].ToLower(), row["CONSTRAINT_SCHEMA"].ToString().ToLower());
            Assert.AreEqual("product_order_ibfk_1", row["CONSTRAINT_NAME"]);
            Assert.AreEqual(DBNull.Value, row["TABLE_CATALOG"]);
            Assert.AreEqual(databases[0].ToLower(), row["TABLE_SCHEMA"].ToString().ToLower());
            Assert.AreEqual("product_order", row["TABLE_NAME"]);
            Assert.AreEqual("product_id", row["COLUMN_NAME"]);
            Assert.AreEqual(1, row["ORDINAL_POSITION"]);
            Assert.AreEqual(databases[0].ToLower(), row["REFERENCED_TABLE_SCHEMA"].ToString().ToLower());
            Assert.AreEqual("product", row["REFERENCED_TABLE_NAME"]);
            Assert.AreEqual("id", row["REFERENCED_COLUMN_NAME"]);

            row = dt.Rows[2];
            Assert.AreEqual(DBNull.Value, row["CONSTRAINT_CATALOG"]);
            Assert.AreEqual(databases[0].ToLower(), row["CONSTRAINT_SCHEMA"].ToString().ToLower());
            Assert.AreEqual("product_order_ibfk_2", row["CONSTRAINT_NAME"]);
            Assert.AreEqual(DBNull.Value, row["TABLE_CATALOG"]);
            Assert.AreEqual(databases[0].ToLower(), row["TABLE_SCHEMA"].ToString().ToLower());
            Assert.AreEqual("product_order", row["TABLE_NAME"]);
            Assert.AreEqual("customer_id", row["COLUMN_NAME"]);
            Assert.AreEqual(0, row["ORDINAL_POSITION"]);
            Assert.AreEqual(databases[0].ToLower(), row["REFERENCED_TABLE_SCHEMA"].ToString().ToLower());
            Assert.AreEqual("customer", row["REFERENCED_TABLE_NAME"]);
            Assert.AreEqual("id", row["REFERENCED_COLUMN_NAME"]);
        }

        [Category("5.0")]
        [Test]
        public void Triggers()
        {
            // CREATE TRIGGER AND DROP TRIGGER required SUPER privs in 5.0 so we 
            // test them only in 5.1
            if (!Is51) return;

            try
            {
                execSQL("DROP TRIGGER trigger1");
            }
            catch (Exception) { }

            execSQL("DROP TABLE IF EXISTS test2");
            execSQL("DROP TABLE IF EXISTS test1");
            execSQL("CREATE TABLE test1 (id int)");
            execSQL("CREATE TABLE test2 (count int)");
            execSQL("INSERT INTO test2 VALUES (0)");
            execSQL("CREATE TRIGGER trigger1 AFTER INSERT ON test1 FOR EACH ROW BEGIN " +
                "UPDATE test2 SET count = count+1; END");

            try
            {
                string[] restrictions = new string[4];
                restrictions[1] = "test";
                restrictions[2] = "test1";
                DataTable dt = conn.GetSchema("Triggers", restrictions);
                Assert.IsTrue(dt.Rows.Count == 1);
                Assert.AreEqual("Triggers", dt.TableName);
                Assert.AreEqual("trigger1", dt.Rows[0]["TRIGGER_NAME"]);
                Assert.AreEqual("INSERT", dt.Rows[0]["EVENT_MANIPULATION"]);
                Assert.AreEqual("test1", dt.Rows[0]["EVENT_OBJECT_TABLE"]);
                Assert.AreEqual("ROW", dt.Rows[0]["ACTION_ORIENTATION"]);
                Assert.AreEqual("AFTER", dt.Rows[0]["ACTION_TIMING"]);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
