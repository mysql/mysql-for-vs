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
using MySql.Data.MySqlClient;
using NUnit.Framework;

namespace MySql.Data.MySqlClient.Tests
{
	/// <summary>
	/// Summary description for StoredProcedure.
	/// </summary>
	[TestFixture]
	public class SchemaTests : BaseTest
	{

		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			csAdditions = ";pooling=false";
			Open();
			execSQL("DROP TABLE IF EXISTS Test; CREATE TABLE Test (id INT, name VARCHAR(100))");
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown() 
		{
			Close();
		}

        [Test]
        [Category("5.0")]
        public void SingleProcedureParameters()
        {
            execSQL("DROP PROCEDURE IF EXISTS spTest");
            execSQL("CREATE PROCEDURE spTest(id int, IN id2 INT(11), " +
                "INOUT io1 VARCHAR(20), OUT out1 FLOAT) BEGIN END");
            string[] restrictions = new string[4];
            restrictions[1] = "test";
            restrictions[2] = "spTest";
            DataTable procs = conn.GetSchema("PROCEDURES", restrictions);
            Assert.AreEqual(1, procs.Rows.Count);
            Assert.AreEqual("spTest", procs.Rows[0][0]);
            Assert.AreEqual("test", procs.Rows[0][2]);
            Assert.AreEqual("spTest", procs.Rows[0][3]);

            DataTable parameters = conn.GetSchema("PROCEDURE PARAMETERS", restrictions);
            Assert.AreEqual(4, parameters.Rows.Count);
            Assert.AreEqual("test", parameters.Rows[0][0]);
            Assert.AreEqual("test", parameters.Rows[1][0]);
            Assert.AreEqual("test", parameters.Rows[2][0]);
            Assert.AreEqual("test", parameters.Rows[3][0]);
            
            Assert.AreEqual("spTest", parameters.Rows[0][1]);
            Assert.AreEqual("spTest", parameters.Rows[1][1]);
            Assert.AreEqual("spTest", parameters.Rows[2][1]);
            Assert.AreEqual("spTest", parameters.Rows[3][1]);

            Assert.AreEqual("id", parameters.Rows[0][2]);
            Assert.AreEqual(1, parameters.Rows[0][3]);
            Assert.AreEqual("IN", parameters.Rows[0][4]);
            Assert.AreEqual("NO", parameters.Rows[0][5]);
            Assert.AreEqual("INT", parameters.Rows[0][6]);

            Assert.AreEqual("id2", parameters.Rows[1][2]);
            Assert.AreEqual(2, parameters.Rows[1][3]);
            Assert.AreEqual("IN", parameters.Rows[1][4]);
            Assert.AreEqual("NO", parameters.Rows[1][5]);
            Assert.AreEqual("INT(11)", parameters.Rows[1][6]);

            Assert.AreEqual("io1", parameters.Rows[2][2]);
            Assert.AreEqual(3, parameters.Rows[2][3]);
            Assert.AreEqual("INOUT", parameters.Rows[2][4]);
            Assert.AreEqual("NO", parameters.Rows[2][5]);
            Assert.AreEqual("VARCHAR(20)", parameters.Rows[2][6]);
            
            Assert.AreEqual("out1", parameters.Rows[3][2]);
            Assert.AreEqual(4, parameters.Rows[3][3]);
            Assert.AreEqual("OUT", parameters.Rows[3][4]);
            Assert.AreEqual("NO", parameters.Rows[3][5]);
            Assert.AreEqual("FLOAT", parameters.Rows[3][6]);
        }

	}
}
