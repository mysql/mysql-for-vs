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
using MySql.Data.MySqlClient;
using NUnit.Framework;

namespace MySql.Data.MySqlClient.Tests
{
	[TestFixture]
	public class ProcedureParameterTests : BaseTest
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
        public void ProcedureParameters()
        {
            if (version < new Version(5, 0)) return;

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

        /// <summary>
        /// Bug #6902 Errors in parsing stored procedure parameters 
        /// </summary>
        [Test]
        public void ProcedureParameters2()
        {
            execSQL(@"CREATE PROCEDURE spTest(`/*id*/` /* before type 1 */ varchar(20), 
                     /* after type 1 */ OUT result2 DECIMAL(/*size1*/10,/*size2*/2) /* p2 */) 
                     BEGIN SELECT action, result; END");

            string[] restrictions = new string[5];
            restrictions[1] = databases[0];
            restrictions[2] = "spTest";
            DataTable dt = conn.GetSchema("Procedure Parameters", restrictions);

            Assert.IsTrue(dt.Rows.Count == 2);
            Assert.AreEqual(databases[0].ToLower(), dt.Rows[0]["ROUTINE_SCHEMA"].ToString().ToLower());
            Assert.AreEqual("sptest", dt.Rows[0]["ROUTINE_NAME"].ToString().ToLower());
            Assert.AreEqual("?/*id*/", dt.Rows[0]["PARAMETER_NAME"].ToString().ToLower());
            Assert.AreEqual(1, dt.Rows[0]["ORDINAL_POSITION"]);
            Assert.AreEqual("IN", dt.Rows[0]["PARAMETER_MODE"]);
            Assert.AreEqual("VARCHAR", dt.Rows[0]["DATA_TYPE"]);
            Assert.AreEqual(20, dt.Rows[0]["CHARACTER_OCTET_LENGTH"]);
            Assert.AreEqual("NO", dt.Rows[0]["IS_RESULT"]);

            Assert.AreEqual(databases[0].ToLower(), dt.Rows[1]["ROUTINE_SCHEMA"].ToString().ToLower());
            Assert.AreEqual("sptest", dt.Rows[1]["ROUTINE_NAME"].ToString().ToLower());
            Assert.AreEqual("?result2", dt.Rows[1]["PARAMETER_NAME"].ToString().ToLower());
            Assert.AreEqual(2, dt.Rows[1]["ORDINAL_POSITION"]);
            Assert.AreEqual("OUT", dt.Rows[1]["PARAMETER_MODE"]);
            Assert.AreEqual("DECIMAL", dt.Rows[1]["DATA_TYPE"]);
            Assert.AreEqual(10, dt.Rows[1]["NUMERIC_PRECISION"]);
            Assert.AreEqual(2, dt.Rows[1]["NUMERIC_SCALE"]);
            Assert.AreEqual("NO", dt.Rows[1]["IS_RESULT"]);
        }

        [Test]
        public void ProcedureParameters3()
        {
            execSQL(@"CREATE  PROCEDURE spTest (_ACTION varchar(20),
                    `/*dumb-identifier-1*/` int, `#dumb-identifier-2` int,
                    `--dumb-identifier-3` int, 
                    _CLIENT_ID int, -- ABC
                    _LOGIN_ID  int, # DEF
                    _WHERE varchar(2000), 
                    _SORT varchar(2000),
                    out _SQL varchar(/* inline right here - oh my gosh! */ 8000),
                    _SONG_ID int,
                    _NOTES varchar(2000),
                    out _RESULT varchar(10)
                    /*
                    ,    -- Generic result parameter
                    out _PERIOD_ID int,         -- Returns the period_id. Useful when using @PREDEFLINK to return which is the last period
                    _SONGS_LIST varchar(8000),
                    _COMPOSERID int,
                    _PUBLISHERID int,
                    _PREDEFLINK int        -- If the user is accessing through a predefined link: 0=none  1=last period
                    */) BEGIN SELECT 1; END");

            string[] restrictions = new string[5];
            restrictions[1] = databases[0];
            restrictions[2] = "spTest";
            DataTable dt = conn.GetSchema("Procedure Parameters", restrictions);

            Assert.IsTrue(dt.Rows.Count == 12);
            Assert.AreEqual(databases[0].ToLower(), dt.Rows[0]["ROUTINE_SCHEMA"].ToString().ToLower());
            Assert.AreEqual("sptest", dt.Rows[0]["ROUTINE_NAME"].ToString().ToLower());
            Assert.AreEqual("?_action", dt.Rows[0]["PARAMETER_NAME"].ToString().ToLower());
            Assert.AreEqual(1, dt.Rows[0]["ORDINAL_POSITION"]);
            Assert.AreEqual("IN", dt.Rows[0]["PARAMETER_MODE"]);
            Assert.AreEqual("VARCHAR", dt.Rows[0]["DATA_TYPE"]);
            Assert.AreEqual(20, dt.Rows[0]["CHARACTER_OCTET_LENGTH"]);
            Assert.AreEqual("NO", dt.Rows[0]["IS_RESULT"]);

            Assert.AreEqual(databases[0].ToLower(), dt.Rows[1]["ROUTINE_SCHEMA"].ToString().ToLower());
            Assert.AreEqual("sptest", dt.Rows[1]["ROUTINE_NAME"].ToString().ToLower());
            Assert.AreEqual("?/*dumb-identifier-1*/", dt.Rows[1]["PARAMETER_NAME"].ToString().ToLower());
            Assert.AreEqual(2, dt.Rows[1]["ORDINAL_POSITION"]);
            Assert.AreEqual("IN", dt.Rows[1]["PARAMETER_MODE"]);
            Assert.AreEqual("INT", dt.Rows[1]["DATA_TYPE"]);
            Assert.AreEqual("NO", dt.Rows[1]["IS_RESULT"]);

            Assert.AreEqual(databases[0].ToLower(), dt.Rows[2]["ROUTINE_SCHEMA"].ToString().ToLower());
            Assert.AreEqual("sptest", dt.Rows[2]["ROUTINE_NAME"].ToString().ToLower());
            Assert.AreEqual("?#dumb-identifier-2", dt.Rows[2]["PARAMETER_NAME"].ToString().ToLower());
            Assert.AreEqual(3, dt.Rows[2]["ORDINAL_POSITION"]);
            Assert.AreEqual("IN", dt.Rows[2]["PARAMETER_MODE"]);
            Assert.AreEqual("INT", dt.Rows[2]["DATA_TYPE"]);
            Assert.AreEqual("NO", dt.Rows[2]["IS_RESULT"]);

            Assert.AreEqual(databases[0].ToLower(), dt.Rows[3]["ROUTINE_SCHEMA"].ToString().ToLower());
            Assert.AreEqual("sptest", dt.Rows[3]["ROUTINE_NAME"].ToString().ToLower());
            Assert.AreEqual("?--dumb-identifier-3", dt.Rows[3]["PARAMETER_NAME"].ToString().ToLower());
            Assert.AreEqual(4, dt.Rows[3]["ORDINAL_POSITION"]);
            Assert.AreEqual("IN", dt.Rows[3]["PARAMETER_MODE"]);
            Assert.AreEqual("INT", dt.Rows[3]["DATA_TYPE"]);
            Assert.AreEqual("NO", dt.Rows[3]["IS_RESULT"]);

            Assert.AreEqual(databases[0].ToLower(), dt.Rows[4]["ROUTINE_SCHEMA"].ToString().ToLower());
            Assert.AreEqual("sptest", dt.Rows[4]["ROUTINE_NAME"].ToString().ToLower());
            Assert.AreEqual("?_client_id", dt.Rows[4]["PARAMETER_NAME"].ToString().ToLower());
            Assert.AreEqual(5, dt.Rows[4]["ORDINAL_POSITION"]);
            Assert.AreEqual("IN", dt.Rows[4]["PARAMETER_MODE"]);
            Assert.AreEqual("INT", dt.Rows[4]["DATA_TYPE"]);
            Assert.AreEqual("NO", dt.Rows[4]["IS_RESULT"]);

            Assert.AreEqual(databases[0].ToLower(), dt.Rows[5]["ROUTINE_SCHEMA"].ToString().ToLower());
            Assert.AreEqual("sptest", dt.Rows[5]["ROUTINE_NAME"].ToString().ToLower());
            Assert.AreEqual("?_login_id", dt.Rows[5]["PARAMETER_NAME"].ToString().ToLower());
            Assert.AreEqual(6, dt.Rows[5]["ORDINAL_POSITION"]);
            Assert.AreEqual("IN", dt.Rows[5]["PARAMETER_MODE"]);
            Assert.AreEqual("INT", dt.Rows[5]["DATA_TYPE"]);
            Assert.AreEqual("NO", dt.Rows[5]["IS_RESULT"]);

            Assert.AreEqual(databases[0].ToLower(), dt.Rows[6]["ROUTINE_SCHEMA"].ToString().ToLower());
            Assert.AreEqual("sptest", dt.Rows[6]["ROUTINE_NAME"].ToString().ToLower());
            Assert.AreEqual("?_where", dt.Rows[6]["PARAMETER_NAME"].ToString().ToLower());
            Assert.AreEqual(7, dt.Rows[6]["ORDINAL_POSITION"]);
            Assert.AreEqual("IN", dt.Rows[6]["PARAMETER_MODE"]);
            Assert.AreEqual("VARCHAR", dt.Rows[6]["DATA_TYPE"]);
            Assert.AreEqual("NO", dt.Rows[6]["IS_RESULT"]);
            Assert.AreEqual(2000, dt.Rows[6]["CHARACTER_OCTET_LENGTH"]);

            Assert.AreEqual(databases[0].ToLower(), dt.Rows[7]["ROUTINE_SCHEMA"].ToString().ToLower());
            Assert.AreEqual("sptest", dt.Rows[7]["ROUTINE_NAME"].ToString().ToLower());
            Assert.AreEqual("?_sort", dt.Rows[7]["PARAMETER_NAME"].ToString().ToLower());
            Assert.AreEqual(8, dt.Rows[7]["ORDINAL_POSITION"]);
            Assert.AreEqual("IN", dt.Rows[7]["PARAMETER_MODE"]);
            Assert.AreEqual("VARCHAR", dt.Rows[7]["DATA_TYPE"]);
            Assert.AreEqual("NO", dt.Rows[7]["IS_RESULT"]);
            Assert.AreEqual(2000, dt.Rows[7]["CHARACTER_OCTET_LENGTH"]);

            Assert.AreEqual(databases[0].ToLower(), dt.Rows[8]["ROUTINE_SCHEMA"].ToString().ToLower());
            Assert.AreEqual("sptest", dt.Rows[8]["ROUTINE_NAME"].ToString().ToLower());
            Assert.AreEqual("?_sql", dt.Rows[8]["PARAMETER_NAME"].ToString().ToLower());
            Assert.AreEqual(9, dt.Rows[8]["ORDINAL_POSITION"]);
            Assert.AreEqual("OUT", dt.Rows[8]["PARAMETER_MODE"]);
            Assert.AreEqual("VARCHAR", dt.Rows[8]["DATA_TYPE"]);
            Assert.AreEqual("NO", dt.Rows[8]["IS_RESULT"]);
            Assert.AreEqual(8000, dt.Rows[8]["CHARACTER_OCTET_LENGTH"]);

            Assert.AreEqual(databases[0].ToLower(), dt.Rows[9]["ROUTINE_SCHEMA"].ToString().ToLower());
            Assert.AreEqual("sptest", dt.Rows[9]["ROUTINE_NAME"].ToString().ToLower());
            Assert.AreEqual("?_song_id", dt.Rows[9]["PARAMETER_NAME"].ToString().ToLower());
            Assert.AreEqual(10, dt.Rows[9]["ORDINAL_POSITION"]);
            Assert.AreEqual("IN", dt.Rows[9]["PARAMETER_MODE"]);
            Assert.AreEqual("INT", dt.Rows[9]["DATA_TYPE"]);
            Assert.AreEqual("NO", dt.Rows[9]["IS_RESULT"]);

            Assert.AreEqual(databases[0].ToLower(), dt.Rows[10]["ROUTINE_SCHEMA"].ToString().ToLower());
            Assert.AreEqual("sptest", dt.Rows[10]["ROUTINE_NAME"].ToString().ToLower());
            Assert.AreEqual("?_notes", dt.Rows[10]["PARAMETER_NAME"].ToString().ToLower());
            Assert.AreEqual(11, dt.Rows[10]["ORDINAL_POSITION"]);
            Assert.AreEqual("IN", dt.Rows[10]["PARAMETER_MODE"]);
            Assert.AreEqual("VARCHAR", dt.Rows[10]["DATA_TYPE"]);
            Assert.AreEqual("NO", dt.Rows[10]["IS_RESULT"]);
            Assert.AreEqual(2000, dt.Rows[10]["CHARACTER_OCTET_LENGTH"]);

            Assert.AreEqual(databases[0].ToLower(), dt.Rows[11]["ROUTINE_SCHEMA"].ToString().ToLower());
            Assert.AreEqual("sptest", dt.Rows[11]["ROUTINE_NAME"].ToString().ToLower());
            Assert.AreEqual("?_result", dt.Rows[11]["PARAMETER_NAME"].ToString().ToLower());
            Assert.AreEqual(12, dt.Rows[11]["ORDINAL_POSITION"]);
            Assert.AreEqual("OUT", dt.Rows[11]["PARAMETER_MODE"]);
            Assert.AreEqual("VARCHAR", dt.Rows[11]["DATA_TYPE"]);
            Assert.AreEqual("NO", dt.Rows[11]["IS_RESULT"]);
            Assert.AreEqual(10, dt.Rows[11]["CHARACTER_OCTET_LENGTH"]);
        }

        [Test]
        public void ProcedureParameters4()
        {
            execSQL(@"CREATE  PROCEDURE spTest (name VARCHAR(1200) 
                    CHARACTER /* hello*/ SET utf8) BEGIN SELECT name; END");

            string[] restrictions = new string[5];
            restrictions[1] = databases[0];
            restrictions[2] = "spTest";
            DataTable dt = conn.GetSchema("Procedure Parameters", restrictions);

            Assert.IsTrue(dt.Rows.Count == 1);
            Assert.AreEqual(databases[0].ToLower(), dt.Rows[0]["ROUTINE_SCHEMA"].ToString().ToLower());
            Assert.AreEqual("sptest", dt.Rows[0]["ROUTINE_NAME"].ToString().ToLower());
            Assert.AreEqual("?name", dt.Rows[0]["PARAMETER_NAME"].ToString().ToLower());
            Assert.AreEqual(1, dt.Rows[0]["ORDINAL_POSITION"]);
            Assert.AreEqual("IN", dt.Rows[0]["PARAMETER_MODE"]);
            Assert.AreEqual("VARCHAR", dt.Rows[0]["DATA_TYPE"]);
            Assert.AreEqual("NO", dt.Rows[0]["IS_RESULT"]);
            Assert.AreEqual(1200, dt.Rows[0]["CHARACTER_OCTET_LENGTH"]);
            Assert.AreEqual("utf8", dt.Rows[0]["CHARACTER_SET"]);
        }

        [Test]
        public void ProcedureParameters5()
        {
            execSQL(@"CREATE  PROCEDURE spTest (name VARCHAR(1200) ASCII BINARY, 
                    name2 TEXT UNICODE) BEGIN SELECT name; END");

            string[] restrictions = new string[5];
            restrictions[1] = databases[0];
            restrictions[2] = "spTest";
            DataTable dt = conn.GetSchema("Procedure Parameters", restrictions);

            Assert.IsTrue(dt.Rows.Count == 2);
            Assert.AreEqual(databases[0].ToLower(), dt.Rows[0]["ROUTINE_SCHEMA"].ToString().ToLower());
            Assert.AreEqual("sptest", dt.Rows[0]["ROUTINE_NAME"].ToString().ToLower());
            Assert.AreEqual("?name", dt.Rows[0]["PARAMETER_NAME"].ToString().ToLower());
            Assert.AreEqual(1, dt.Rows[0]["ORDINAL_POSITION"]);
            Assert.AreEqual("IN", dt.Rows[0]["PARAMETER_MODE"]);
            Assert.AreEqual("VARCHAR", dt.Rows[0]["DATA_TYPE"]);
            Assert.AreEqual("NO", dt.Rows[0]["IS_RESULT"]);
            Assert.AreEqual("latin1", dt.Rows[0]["CHARACTER_SET"]);
            Assert.AreEqual(1200, dt.Rows[0]["CHARACTER_OCTET_LENGTH"]);

            Assert.AreEqual(databases[0].ToLower(), dt.Rows[1]["ROUTINE_SCHEMA"].ToString().ToLower());
            Assert.AreEqual("sptest", dt.Rows[1]["ROUTINE_NAME"].ToString().ToLower());
            Assert.AreEqual("?name2", dt.Rows[1]["PARAMETER_NAME"].ToString().ToLower());
            Assert.AreEqual(2, dt.Rows[1]["ORDINAL_POSITION"]);
            Assert.AreEqual("IN", dt.Rows[1]["PARAMETER_MODE"]);
            Assert.AreEqual("TEXT", dt.Rows[1]["DATA_TYPE"]);
            Assert.AreEqual("NO", dt.Rows[1]["IS_RESULT"]);
            Assert.AreEqual("ucs2", dt.Rows[1]["CHARACTER_SET"]);
        }
    }
}
