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
            Assert.AreEqual(17, dt.Rows.Count);
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
            Assert.AreEqual("Views", dt.Rows[12][0]);
            Assert.AreEqual("ViewColumns", dt.Rows[13][0]);
            Assert.AreEqual("ProcedureParameters", dt.Rows[14][0]);
            Assert.AreEqual("Procedures", dt.Rows[15][0]);
            Assert.AreEqual("Triggers", dt.Rows[16][0]);
        }

        [Test]
        public void Databases()
        {
            DataTable dt = conn.GetSchema("Databases");
            Assert.IsTrue(dt.Rows.Count >= 3);
            Assert.AreEqual("Databases", dt.TableName);

            dt = conn.GetSchema("Databases", new string[1] { "mysql" });
            Assert.AreEqual(1, dt.Rows.Count);
            Assert.AreEqual("mysql", dt.Rows[0][1].ToString().ToLower());
        }

        [Test]
        public void Tables()
        {
            execSQL("DROP TABLE IF EXISTS test1");
            execSQL("CREATE TABLE test1 (id int)");

            string[] restrictions = new string[4];
            restrictions[1] = "test";
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
            restrictions[1] = "test";
            restrictions[2] = "test";
            DataTable dt = conn.GetSchema("Columns", restrictions);
            Assert.IsTrue(dt.Rows.Count == 4);
            Assert.AreEqual("Columns", dt.TableName);
            
            // first column
            Assert.AreEqual("TEST", dt.Rows[0]["TABLE_SCHEMA"].ToString().ToUpper());
            Assert.AreEqual("COL1", dt.Rows[0]["COLUMN_NAME"].ToString().ToUpper());
            Assert.AreEqual(1, dt.Rows[0]["ORDINAL_POSITION"]);
            Assert.AreEqual("YES", dt.Rows[0]["IS_NULLABLE"]);
            Assert.AreEqual("INT", dt.Rows[0]["DATA_TYPE"].ToString().ToUpper());

            // second column
            Assert.AreEqual("TEST", dt.Rows[1]["TABLE_SCHEMA"].ToString().ToUpper());
            Assert.AreEqual("COL2", dt.Rows[1]["COLUMN_NAME"].ToString().ToUpper());
            Assert.AreEqual(2, dt.Rows[1]["ORDINAL_POSITION"]);
            Assert.AreEqual("YES", dt.Rows[1]["IS_NULLABLE"]);
            Assert.AreEqual("DECIMAL", dt.Rows[1]["DATA_TYPE"].ToString().ToUpper());
            Assert.AreEqual("DECIMAL(20,5)", dt.Rows[1]["COLUMN_TYPE"].ToString().ToUpper());
            Assert.AreEqual(20, dt.Rows[1]["NUMERIC_PRECISION"]);
            Assert.AreEqual(5, dt.Rows[1]["NUMERIC_SCALE"]);

            // third column
            Assert.AreEqual("TEST", dt.Rows[2]["TABLE_SCHEMA"].ToString().ToUpper());
            Assert.AreEqual("COL3", dt.Rows[2]["COLUMN_NAME"].ToString().ToUpper());
            Assert.AreEqual(3, dt.Rows[2]["ORDINAL_POSITION"]);
            Assert.AreEqual("YES", dt.Rows[2]["IS_NULLABLE"]);
            Assert.AreEqual("VARCHAR", dt.Rows[2]["DATA_TYPE"].ToString().ToUpper());
            Assert.AreEqual("VARCHAR(50)", dt.Rows[2]["COLUMN_TYPE"].ToString().ToUpper());
            Assert.AreEqual(50, dt.Rows[2]["CHARACTER_MAXIMUM_LENGTH"]);

            // fourth column
            Assert.AreEqual("TEST", dt.Rows[3]["TABLE_SCHEMA"].ToString().ToUpper());
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
            restrictions[1] = "test";
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
            restrictions[1] = "test";
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
            execSQL("CREATE PROCEDURE spTest (id int) BEGIN SELECT 1; END");

            string[] restrictions = new string[4];
            restrictions[1] = "test";
            restrictions[2] = "spTest";
            DataTable dt = conn.GetSchema("Procedure Parameters", restrictions);
            Assert.IsTrue(dt.Rows.Count == 1);
            Assert.AreEqual("Procedure Parameters", dt.TableName);
            Assert.AreEqual("test", dt.Rows[0]["ROUTINE_SCHEMA"].ToString().ToLower());
            Assert.AreEqual("sptest", dt.Rows[0]["ROUTINE_NAME"].ToString().ToLower());
            Assert.AreEqual("id", dt.Rows[0]["PARAMETER_NAME"].ToString().ToLower());
            Assert.AreEqual(1, dt.Rows[0]["ORDINAL_POSITION"]);
            Assert.AreEqual("IN", dt.Rows[0]["PARAMETER_MODE"]);
            Assert.AreEqual("NO", dt.Rows[0]["IS_RESULT"]);

            execSQL("DROP FUNCTION IF EXISTS spFunc");
            execSQL("CREATE FUNCTION spFunc (id int) RETURNS INT BEGIN RETURN 1; END");

            restrictions[1] = "test";
            restrictions[2] = "spFunc";
            dt = conn.GetSchema("Procedure Parameters", restrictions);
            Assert.IsTrue(dt.Rows.Count == 2);
            Assert.AreEqual("Procedure Parameters", dt.TableName);
            Assert.AreEqual("test", dt.Rows[0]["ROUTINE_SCHEMA"].ToString().ToLower());
            Assert.AreEqual("spfunc", dt.Rows[0]["ROUTINE_NAME"].ToString().ToLower());
            Assert.AreEqual("id", dt.Rows[0]["PARAMETER_NAME"].ToString().ToLower());
            Assert.AreEqual(1, dt.Rows[0]["ORDINAL_POSITION"]);
            Assert.AreEqual("IN", dt.Rows[0]["PARAMETER_MODE"]);
            Assert.AreEqual("NO", dt.Rows[0]["IS_RESULT"]);

            Assert.AreEqual("test", dt.Rows[1]["ROUTINE_SCHEMA"].ToString().ToLower());
            Assert.AreEqual("spfunc", dt.Rows[1]["ROUTINE_NAME"].ToString().ToLower());
            Assert.AreEqual(0, dt.Rows[1]["ORDINAL_POSITION"]);
            Assert.AreEqual("YES", dt.Rows[1]["IS_RESULT"]);
        }

        [Category("5.0")]
        [Test]
        public void Views()
        {
            execSQL("CREATE VIEW vw AS SELECT Now() as theTime");

            string[] restrictions = new string[4];
            restrictions[1] = "test";
            restrictions[2] = "vw";
            DataTable dt = conn.GetSchema("Views", restrictions);
            Assert.IsTrue(dt.Rows.Count == 1);
            Assert.AreEqual("Views", dt.TableName);
            Assert.AreEqual("vw", dt.Rows[0][3]);
        }
	}
}
