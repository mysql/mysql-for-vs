// Copyright (c) 2004-2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
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
using System.Globalization;
using System.Threading;
using MySql.Data.Types;
using System.Data.Common;

namespace MySql.Data.MySqlClient.Tests
{
    [TestFixture]
    public class StoredProcedureAccess : StoredProcedure
    {
        public override void Setup()
        {
            base.Setup();
            SetAccountPerms(true);
        }

        protected override string GetConnectionInfo()
        {
            string info = base.GetConnectionInfo();
            info = info.Replace("use procedure bodies=false", "");
            return info;
        }

        /// <summary>
        /// Bug #40139	ExecuteNonQuery hangs
        /// </summary>
        [Test]
        public void CallingStoredProcWithOnlyExecPrivs()
        {
            if (Version < new Version(5, 0)) return;
            
            execSQL("CREATE PROCEDURE spTest() BEGIN SELECT 1; END");
            execSQL("CREATE PROCEDURE spTest2() BEGIN SELECT 1; END");
            suExecSQL("CREATE USER abc IDENTIFIED BY 'abc'");
            try
            {
                suExecSQL(String.Format("GRANT SELECT ON `{0}`.* TO 'abc'@'%'", database0));
                suExecSQL(String.Format("GRANT EXECUTE ON PROCEDURE `{0}`.spTest TO abc", database0));

                string connStr = GetConnectionStringEx("abc", "abc", true);
                using (MySqlConnection c = new MySqlConnection(connStr))
                {
                    c.Open();
                    MySqlCommand cmd = new MySqlCommand("spTest", c);
                    cmd.CommandType = CommandType.StoredProcedure;
                    try
                    {
                        object o = cmd.ExecuteScalar();
                        Assert.Fail("This should fail");
                    }
                    catch (InvalidOperationException)
                    {
                    }
                    catch (MySqlException)
                    {
                    }
                    catch (Exception)
                    {
                        Assert.Fail("A more specific exception should have been thrown");
                    }

                    try 
                    {
                        cmd.CommandText = "spTest2";
                        cmd.ExecuteScalar();
                    }
                    catch (InvalidOperationException)
                    {
                    }
                    catch (MySqlException ex)
                    {
                        string s = ex.Message;
                    }
                    catch (Exception)
                    {
                        Assert.Fail("This should have thrown a more specific exception");
                    }
                }
            }
            finally
            {
                suExecSQL("DROP USER abc");
            }
        }

        [Test]
        public void ProcedureParameters()
        {
            if (Version < new Version(5, 0)) return;

            execSQL("DROP PROCEDURE IF EXISTS spTest");
            execSQL("CREATE PROCEDURE spTest (id int, name varchar(50)) BEGIN SELECT 1; END");

            string[] restrictions = new string[5];
            restrictions[1] = database0;
            restrictions[2] = "spTest";
            DataTable dt = conn.GetSchema("Procedure Parameters", restrictions);
            Assert.IsTrue(dt.Rows.Count == 2);
            Assert.AreEqual("Procedure Parameters", dt.TableName);
            Assert.AreEqual(database0.ToLower(), dt.Rows[0]["SPECIFIC_SCHEMA"].ToString().ToLower());
            Assert.AreEqual("sptest", dt.Rows[0]["SPECIFIC_NAME"].ToString().ToLower());
            Assert.AreEqual("id", dt.Rows[0]["PARAMETER_NAME"].ToString().ToLower());
            Assert.AreEqual(1, dt.Rows[0]["ORDINAL_POSITION"]);
            Assert.AreEqual("IN", dt.Rows[0]["PARAMETER_MODE"]);

            restrictions[4] = "name";
            dt.Clear();
            dt = conn.GetSchema("Procedure Parameters", restrictions);
            Assert.AreEqual(1, dt.Rows.Count);
            Assert.AreEqual(database0.ToLower(), dt.Rows[0]["SPECIFIC_SCHEMA"].ToString().ToLower());
            Assert.AreEqual("sptest", dt.Rows[0]["SPECIFIC_NAME"].ToString().ToLower());
            Assert.AreEqual("name", dt.Rows[0]["PARAMETER_NAME"].ToString().ToLower());
            Assert.AreEqual(2, dt.Rows[0]["ORDINAL_POSITION"]);
            Assert.AreEqual("IN", dt.Rows[0]["PARAMETER_MODE"]);

            execSQL("DROP FUNCTION IF EXISTS spFunc");
            execSQL("CREATE FUNCTION spFunc (id int) RETURNS INT BEGIN RETURN 1; END");

            restrictions[4] = null;
            restrictions[1] = database0;
            restrictions[2] = "spFunc";
            dt = conn.GetSchema("Procedure Parameters", restrictions);
            Assert.IsTrue(dt.Rows.Count == 2);
            Assert.AreEqual("Procedure Parameters", dt.TableName);
            Assert.AreEqual(database0.ToLower(), dt.Rows[0]["SPECIFIC_SCHEMA"].ToString().ToLower());
            Assert.AreEqual("spfunc", dt.Rows[0]["SPECIFIC_NAME"].ToString().ToLower());
            Assert.AreEqual(0, dt.Rows[0]["ORDINAL_POSITION"]);

            Assert.AreEqual(database0.ToLower(), dt.Rows[1]["SPECIFIC_SCHEMA"].ToString().ToLower());
            Assert.AreEqual("spfunc", dt.Rows[1]["SPECIFIC_NAME"].ToString().ToLower());
            Assert.AreEqual("id", dt.Rows[1]["PARAMETER_NAME"].ToString().ToLower());
            Assert.AreEqual(1, dt.Rows[1]["ORDINAL_POSITION"]);
            Assert.AreEqual("IN", dt.Rows[1]["PARAMETER_MODE"]);
        }

        [Test]
        public void SingleProcedureParameters()
        {
            if (Version < new Version(5, 0)) return;

            execSQL("DROP PROCEDURE IF EXISTS spTest");
            execSQL("CREATE PROCEDURE spTest(id int, IN id2 INT(11), " +
                "INOUT io1 VARCHAR(20), OUT out1 FLOAT) BEGIN END");
            string[] restrictions = new string[4];
            restrictions[1] = database0;
            restrictions[2] = "spTest";
            DataTable procs = conn.GetSchema("PROCEDURES", restrictions);
            Assert.AreEqual(1, procs.Rows.Count);
            Assert.AreEqual("spTest", procs.Rows[0][0]);
            Assert.AreEqual(database0.ToLower(), procs.Rows[0][2].ToString().ToLower(CultureInfo.InvariantCulture));
            Assert.AreEqual("spTest", procs.Rows[0][3]);

            DataTable parameters = conn.GetSchema("PROCEDURE PARAMETERS", restrictions);
            Assert.AreEqual(4, parameters.Rows.Count);

            DataRow row = parameters.Rows[0];
            Assert.AreEqual(database0.ToLower(CultureInfo.InvariantCulture),
                row["SPECIFIC_SCHEMA"].ToString().ToLower(CultureInfo.InvariantCulture));
            Assert.AreEqual("spTest", row["SPECIFIC_NAME"]);
            Assert.AreEqual(1, row["ORDINAL_POSITION"]);
            Assert.AreEqual("IN", row["PARAMETER_MODE"]);
            Assert.AreEqual("id", row["PARAMETER_NAME"]);
            Assert.AreEqual("INT", row["DATA_TYPE"].ToString().ToUpper(CultureInfo.InvariantCulture));

            row = parameters.Rows[1];
            Assert.AreEqual(database0.ToLower(CultureInfo.InvariantCulture), row["SPECIFIC_SCHEMA"].ToString().ToLower(CultureInfo.InvariantCulture));
            Assert.AreEqual("spTest", row["SPECIFIC_NAME"]);
            Assert.AreEqual(2, row["ORDINAL_POSITION"]);
            Assert.AreEqual("IN", row["PARAMETER_MODE"]);
            Assert.AreEqual("id2", row["PARAMETER_NAME"]);
            Assert.AreEqual("INT", row["DATA_TYPE"].ToString().ToUpper(CultureInfo.InvariantCulture));

            row = parameters.Rows[2];
            Assert.AreEqual(database0.ToLower(CultureInfo.InvariantCulture), row["SPECIFIC_SCHEMA"].ToString().ToLower(CultureInfo.InvariantCulture));
            Assert.AreEqual("spTest", row["SPECIFIC_NAME"]);
            Assert.AreEqual(3, row["ORDINAL_POSITION"]);
            Assert.AreEqual("INOUT", row["PARAMETER_MODE"]);
            Assert.AreEqual("io1", row["PARAMETER_NAME"]);
            Assert.AreEqual("VARCHAR", row["DATA_TYPE"].ToString().ToUpper(CultureInfo.InvariantCulture));

            row = parameters.Rows[3];
            Assert.AreEqual(database0.ToLower(CultureInfo.InvariantCulture), row["SPECIFIC_SCHEMA"].ToString().ToLower(CultureInfo.InvariantCulture));
            Assert.AreEqual("spTest", row["SPECIFIC_NAME"]);
            Assert.AreEqual(4, row["ORDINAL_POSITION"]);
            Assert.AreEqual("OUT", row["PARAMETER_MODE"]);
            Assert.AreEqual("out1", row["PARAMETER_NAME"]);
            Assert.AreEqual("FLOAT", row["DATA_TYPE"].ToString().ToUpper(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Bug #27679  	MySqlCommandBuilder.DeriveParameters ignores UNSIGNED flag
        /// </summary>
        [Test]
        public void UnsignedParametersInSP()
        {
            if (Version < new Version(5, 0)) return;

            execSQL("CREATE PROCEDURE spTest(testid TINYINT UNSIGNED) BEGIN SELECT testid; END");

            MySqlCommand cmd = new MySqlCommand("spTest", conn);
            MySqlCommandBuilder.DeriveParameters(cmd);
            Assert.AreEqual(MySqlDbType.UByte, cmd.Parameters[0].MySqlDbType);
            Assert.AreEqual(DbType.Byte, cmd.Parameters[0].DbType);
        }

        [Test]
        public void CheckNameOfReturnParameter()
        {
            if (Version < new Version(5, 0)) return;

            execSQL("CREATE FUNCTION fnTest() RETURNS CHAR(50)" +
                " LANGUAGE SQL DETERMINISTIC BEGIN  RETURN \"Test\"; END");

            MySqlCommand cmd = new MySqlCommand("fnTest", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            MySqlCommandBuilder.DeriveParameters(cmd);
            Assert.AreEqual(1, cmd.Parameters.Count);
            Assert.AreEqual("@RETURN_VALUE", cmd.Parameters[0].ParameterName);
        }

        /// <summary>
        /// Bug #13632  	the MySQLCommandBuilder.deriveparameters has not been updated for MySQL 5
        /// Bug #15077  	Error MySqlCommandBuilder.DeriveParameters for sp without parameters.
        /// Bug #19515  	DiscoverParameters fails on numeric datatype
        /// </summary>
        [Test]
        public void DeriveParameters()
        {
            if (Version < new Version(5, 0)) return;

            execSQL("CREATE TABLE test2 (c CHAR(20))");
            execSQL("INSERT INTO test2 values ( 'xxxx')");
            MySqlCommand cmd2 = new MySqlCommand("SELECT * FROM test2", conn);
            using (MySqlDataReader reader = cmd2.ExecuteReader())
            {
            }

            execSQL("CREATE PROCEDURE spTest(IN \r\nvalin DECIMAL(10,2), " +
                "\nIN val2 INT, INOUT val3 FLOAT, OUT val4 DOUBLE, INOUT val5 BIT, " +
                "val6 VARCHAR(155), val7 SET('a','b'), val8 CHAR, val9 NUMERIC(10,2)) " +
                     "BEGIN SELECT 1; END");

            MySqlCommand cmd = new MySqlCommand("spTest", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            MySqlCommandBuilder.DeriveParameters(cmd);

            Assert.AreEqual(9, cmd.Parameters.Count);
            Assert.AreEqual("@valin", cmd.Parameters[0].ParameterName);
            Assert.AreEqual(ParameterDirection.Input, cmd.Parameters[0].Direction);
            Assert.AreEqual(MySqlDbType.NewDecimal, cmd.Parameters[0].MySqlDbType);

            Assert.AreEqual("@val2", cmd.Parameters[1].ParameterName);
            Assert.AreEqual(ParameterDirection.Input, cmd.Parameters[1].Direction);
            Assert.AreEqual(MySqlDbType.Int32, cmd.Parameters[1].MySqlDbType);

            Assert.AreEqual("@val3", cmd.Parameters[2].ParameterName);
            Assert.AreEqual(ParameterDirection.InputOutput, cmd.Parameters[2].Direction);
            Assert.AreEqual(MySqlDbType.Float, cmd.Parameters[2].MySqlDbType);

            Assert.AreEqual("@val4", cmd.Parameters[3].ParameterName);
            Assert.AreEqual(ParameterDirection.Output, cmd.Parameters[3].Direction);
            Assert.AreEqual(MySqlDbType.Double, cmd.Parameters[3].MySqlDbType);

            Assert.AreEqual("@val5", cmd.Parameters[4].ParameterName);
            Assert.AreEqual(ParameterDirection.InputOutput, cmd.Parameters[4].Direction);
            Assert.AreEqual(MySqlDbType.Bit, cmd.Parameters[4].MySqlDbType);

            Assert.AreEqual("@val6", cmd.Parameters[5].ParameterName);
            Assert.AreEqual(ParameterDirection.Input, cmd.Parameters[5].Direction);
            Assert.AreEqual(MySqlDbType.VarChar, cmd.Parameters[5].MySqlDbType);

            Assert.AreEqual("@val7", cmd.Parameters[6].ParameterName);
            Assert.AreEqual(ParameterDirection.Input, cmd.Parameters[6].Direction);
            Assert.AreEqual(MySqlDbType.Set, cmd.Parameters[6].MySqlDbType);

            Assert.AreEqual("@val8", cmd.Parameters[7].ParameterName);
            Assert.AreEqual(ParameterDirection.Input, cmd.Parameters[7].Direction);
            Assert.AreEqual(MySqlDbType.String, cmd.Parameters[7].MySqlDbType);

            Assert.AreEqual("@val9", cmd.Parameters[8].ParameterName);
            Assert.AreEqual(ParameterDirection.Input, cmd.Parameters[8].Direction);
            Assert.AreEqual(MySqlDbType.NewDecimal, cmd.Parameters[8].MySqlDbType);

            execSQL("DROP PROCEDURE spTest");
            execSQL("CREATE PROCEDURE spTest() BEGIN END");
            cmd.CommandText = "spTest";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            MySqlCommandBuilder.DeriveParameters(cmd);
            Assert.AreEqual(0, cmd.Parameters.Count);
        }

        /// <summary>
        /// Bug #13632  	the MySQLCommandBuilder.deriveparameters has not been updated for MySQL 5
        /// </summary>
        [Test]
        public void DeriveParametersForFunction()
        {
            if (Version < new Version(5, 0)) return;

            execSQL("CREATE FUNCTION fnTest(v1 DATETIME) RETURNS INT " +
                "  LANGUAGE SQL DETERMINISTIC BEGIN RETURN 1; END");

            MySqlCommand cmd = new MySqlCommand("fnTest", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            MySqlCommandBuilder.DeriveParameters(cmd);

            Assert.AreEqual(2, cmd.Parameters.Count);
            Assert.AreEqual("@v1", cmd.Parameters[1].ParameterName);
            Assert.AreEqual(ParameterDirection.Input, cmd.Parameters[1].Direction);
            Assert.AreEqual(MySqlDbType.DateTime, cmd.Parameters[1].MySqlDbType);

            Assert.AreEqual(ParameterDirection.ReturnValue, cmd.Parameters[0].Direction);
            Assert.AreEqual(MySqlDbType.Int32, cmd.Parameters[0].MySqlDbType);
        }
    }
}
