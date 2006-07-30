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
        }

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

        [Test]
        public void Functions()
        {
        }

        [Test]
        public void ProcedureParameters()
        {
        }

        [Test]
        public void Views()
        {
        }
	}
}
