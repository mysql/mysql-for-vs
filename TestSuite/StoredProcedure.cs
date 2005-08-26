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
using System.Data;
using MySql.Data.MySqlClient;
using NUnit.Framework;

namespace MySql.Data.MySqlClient.Tests
{
	/// <summary>
	/// Summary description for StoredProcedure.
	/// </summary>
	[TestFixture()]
	public class StoredProcedure : BaseTest
	{
		private static string fillError = null;

		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			Open();
			execSQL("DROP TABLE IF EXISTS Test; CREATE TABLE Test (id INT, name VARCHAR(100))");
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown() 
		{
			Close();
		}

		[Test()]
		public void NonQuery()
		{
			if (! Is50) return;

			execSQL("CREATE PROCEDURE spTest(IN value INT) BEGIN INSERT INTO Test VALUES(value, 'Test'); END" );

			//setup testing data
			MySqlCommand cmd = new MySqlCommand("spTest", conn);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.Add( "?value", 2 );
			int rowsAffected = cmd.ExecuteNonQuery();
			Assert.AreEqual( 1, rowsAffected );

			cmd.CommandText = "SELECT * FROM Test";
			cmd.CommandType = CommandType.Text;
			MySqlDataReader reader = null;
			
			try 
			{
				reader = cmd.ExecuteReader();
				Assert.IsTrue( reader.Read() );
				Assert.AreEqual( 2, reader.GetInt32(0) );
				Assert.AreEqual( "Test", reader.GetString(1) );
				Assert.IsFalse( reader.Read() );
				Assert.IsFalse( reader.NextResult() );
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
		public void OutputParameters()
		{
			if (! Is50) return;

			// create our procedure
			execSQL( "DROP PROCEDURE IF EXISTS spCount" );
			execSQL( "CREATE PROCEDURE spCount( out value VARCHAR(50), OUT intVal INT, OUT dateVal TIMESTAMP, OUT floatVal FLOAT ) " + 
				"BEGIN  SET value='42';  SET intVal=33; SET dateVal='2004-06-05 07:58:09'; SET floatVal = 1.2; END" );

			MySqlCommand cmd = new MySqlCommand("spCount", conn);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.Add( new MySqlParameter("?value", MySqlDbType.VarChar));
			cmd.Parameters.Add( new MySqlParameter( "?intVal", MySqlDbType.Int32 ) );
			cmd.Parameters.Add( new MySqlParameter( "?dateVal", MySqlDbType.Datetime ) );
			cmd.Parameters.Add( new MySqlParameter( "?floatVal", MySqlDbType.Float ) );
			cmd.Parameters[0].Direction = ParameterDirection.Output;
			cmd.Parameters[1].Direction = ParameterDirection.Output;
			cmd.Parameters[2].Direction = ParameterDirection.Output;
			cmd.Parameters[3].Direction = ParameterDirection.Output;
			int rowsAffected = cmd.ExecuteNonQuery();

			Assert.AreEqual( 0, rowsAffected );
			Assert.AreEqual( "42", cmd.Parameters[0].Value );
			Assert.AreEqual( 33, cmd.Parameters[1].Value );
			Assert.AreEqual( new DateTime(2004, 6, 5, 7, 58, 9), Convert.ToDateTime(cmd.Parameters[2].Value) );
			Assert.AreEqual( 1.2, cmd.Parameters[3].Value );

			execSQL("DROP PROCEDURE spCount");
		}

		[Test()]
		public void NoBatch()
		{
			if (! Is50) return;

			try 
			{
				MySqlCommand cmd = new MySqlCommand("spTest;select * from test", conn);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.ExecuteNonQuery();
				Assert.Fail("Should have thrown an exception");
			}
			catch (MySqlException) 
			{
			}
		}

		[Test()]
		public void WrongParameters()
		{
			if (! Is50) return;

			try 
			{
			MySqlCommand cmd = new MySqlCommand("spTest", conn);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.Add( "?p2", 1 );
			int rowsAffected = cmd.ExecuteNonQuery();
			Assert.Fail("Should have thrown an exception");
			}
			catch (MySqlException) 
			{
			}
		}

		[Test]
		public void NoInOutMarker() 
		{
			if (! Is50) return;

			// create our procedure
			execSQL( "CREATE PROCEDURE spTest( valin varchar(50) ) BEGIN  SELECT valin;  END" );

			MySqlCommand cmd = new MySqlCommand("spTest", conn);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.Add( "?valin", "myvalue" );
			object val = cmd.ExecuteScalar();
			Assert.AreEqual( "myvalue", val );

			execSQL("DROP PROCEDURE spTest");
		}

		[Test()]
		public void InputOutputParameters()
		{
			if (! Is50) return;

			// create our procedure
			execSQL( "CREATE PROCEDURE spTest( INOUT strVal VARCHAR(50), INOUT numVal INT ) " +
				"BEGIN  SET strVal = CONCAT(strVal,'ending'); SET numVal=numVal * 2;  END" );

			MySqlCommand cmd = new MySqlCommand("spTest", conn);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.Add( "?strVal", "beginning" );
			cmd.Parameters.Add( "?numVal", 33 );
			cmd.Parameters[0].Direction = ParameterDirection.InputOutput;
			cmd.Parameters[1].Direction = ParameterDirection.InputOutput;
			int rowsAffected = cmd.ExecuteNonQuery();
			Assert.AreEqual( 0, rowsAffected );
			Assert.AreEqual( "beginningending", cmd.Parameters[0].Value );
			Assert.AreEqual( 66, cmd.Parameters[1].Value );

			execSQL("DROP PROCEDURE spTest");
		}

		[Test()]
		public void NoSPOnPre50() 
		{
			if (Is50) return;

			try 
			{
				MySqlCommand cmd = new MySqlCommand("spTest", conn);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.ExecuteNonQuery();
				Assert.Fail( "This should not have worked" );
			}
			catch (Exception) 
			{
			}
		}

		[Test()]
		public void ExecuteScalar() 
		{
			if (! Is50) return;

			// create our procedure
			execSQL( "CREATE PROCEDURE spTest( IN valin VARCHAR(50), OUT valout VARCHAR(50) ) " +
				"BEGIN  SET valout=valin;  SELECT 'Test'; END" );

			MySqlCommand cmd = new MySqlCommand("spTest", conn);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.Add( "?valin", "valuein" );
			cmd.Parameters.Add( new MySqlParameter("?valout", MySqlDbType.VarChar));
			cmd.Parameters[1].Direction = ParameterDirection.Output;
			object result = cmd.ExecuteScalar();
			Assert.AreEqual( "Test", result );
			Assert.AreEqual( "valuein", cmd.Parameters[1].Value );

			execSQL("DROP PROCEDURE spTest");
		}

		[Test()]
		public void ExecuteReader()
		{
			if (! Is50) return;

			// create our procedure
			execSQL( "CREATE PROCEDURE spTest() " +
				"BEGIN  SELECT * FROM mysql.db; END" );

			MySqlCommand cmd = new MySqlCommand("spTest", conn);
			cmd.Parameters.Add("?a", 3);
			cmd.CommandType = CommandType.StoredProcedure;
			MySqlDataReader reader = cmd.ExecuteReader();
			Assert.AreEqual( true, reader.Read() );
			Assert.AreEqual( false, reader.NextResult() );
			Assert.AreEqual( false, reader.Read() );
			reader.Close();

			execSQL("DROP PROCEDURE spTest");
		}

		[Test()]
		public void MultipleResultsets() 
		{
			if (! Is50) return;
			MultipleResultsetsImpl(false);
//			MultipleResultsetsImpl(true);
		}

		private void MultipleResultsetsImpl(bool prepare)
		{
			// create our procedure
			execSQL( "CREATE PROCEDURE spTest() " +
				"BEGIN  SELECT 1; SELECT 2; END" );

			MySqlCommand cmd = new MySqlCommand("spTest", conn);
			if (prepare) cmd.Prepare();
			cmd.CommandType = CommandType.StoredProcedure;
			MySqlDataReader reader = cmd.ExecuteReader();
			Assert.AreEqual( true, reader.Read() );
			Assert.AreEqual( true, reader.NextResult() );
			Assert.AreEqual( true, reader.Read() );
			Assert.AreEqual( false, reader.NextResult() );
			Assert.AreEqual( false, reader.Read() );
			reader.Close();

			DataSet ds = new DataSet();
			MySqlCommand cmd2 = new MySqlCommand("spTest", conn);
			cmd2.CommandType = CommandType.StoredProcedure;
			MySqlDataAdapter da = new MySqlDataAdapter( cmd2 );
			da.FillError += new FillErrorEventHandler(da_FillError);
			fillError = null;
			da.Fill(ds);
			Assert.AreEqual( 2, ds.Tables.Count );
			Assert.AreEqual( 1, ds.Tables[0].Rows.Count );
			Assert.AreEqual( 1, ds.Tables[1].Rows.Count );
			Assert.AreEqual( 1, ds.Tables[0].Rows[0][0] );
			Assert.AreEqual( 2, ds.Tables[1].Rows[0][0] );
			Assert.IsNull( fillError );

			execSQL("DROP PROCEDURE spTest");
		}

		private void da_FillError(object sender, FillErrorEventArgs e)
		{
			fillError = e.Errors.Message;
			e.Continue = true;
		}

		[Test()]
		public void FunctionNoParams() 
		{
			if (! Is50) return;

			execSQL( "CREATE FUNCTION fnTest() RETURNS CHAR(50)" +
				"BEGIN  RETURN \"Test\"; END" );

			MySqlCommand cmd = new MySqlCommand("SELECT fnTest()", conn);
			cmd.CommandType = CommandType.Text;
			object result = cmd.ExecuteScalar();
			Assert.AreEqual( "Test", result );

			execSQL("DROP FUNCTION fnTest");
		}

		[Test()]
		public void FunctionParams() 
		{
			if (! Is50) return;

			execSQL( "CREATE FUNCTION fnTest( val1 INT, val2 CHAR(40) ) RETURNS INT " +
				"BEGIN  RETURN val1 + LENGTH(val2);  END" );

			MySqlCommand cmd = new MySqlCommand("SELECT fnTest(22, 'Test')", conn);
			cmd.CommandType = CommandType.Text;
			object result = cmd.ExecuteScalar();
			Assert.AreEqual( 26, result);

			execSQL("DROP FUNCTION fnTest");
		}

		[Test()]
		public void UseOldSyntax() 
		{
			if (! Is50) return;
			
			// create our procedure
			execSQL( "CREATE PROCEDURE spTest( IN valin VARCHAR(50), OUT valout VARCHAR(50) ) " +
				"BEGIN  SET valout=valin;  SELECT 'Test'; END" );

			MySqlConnection c2 = new MySqlConnection( conn.ConnectionString + ";old syntax=yes" );
			c2.Open();

			MySqlCommand cmd = new MySqlCommand("spTest", c2);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.Add( "@valin", "value" );
			cmd.Parameters.Add( new MySqlParameter("@valout", MySqlDbType.VarChar));
			cmd.Parameters[1].Direction = ParameterDirection.Output;
			object result = cmd.ExecuteScalar();
			Assert.AreEqual( "Test", result );
			Assert.AreEqual( "value", cmd.Parameters[1].Value );
			c2.Close();

			execSQL("DROP PROCEDURE spTest");
		}

		[Test]
		public void ExecuteWithCreate() 
		{
			if (! Is50) return;
			
			// create our procedure
			string sql = "CREATE PROCEDURE spTest(IN var INT) BEGIN  SELECT var; END; call spTest(?v)";

			MySqlCommand cmd = new MySqlCommand(sql, conn);
			cmd.Parameters.Add(new MySqlParameter("?v", 33));
			object val = cmd.ExecuteScalar();
			Assert.AreEqual( 33, val );
		}

		/// <summary>
		/// Bug #9722 Connector does not recognize parameters separated by a linefeed 
		/// </summary>
		[Test]
		public void OtherProcSigs() 
		{
			if (! Is50) return;
			
			// create our procedure
			execSQL( "CREATE PROCEDURE spTest(IN \r\nvalin DECIMAL(10,2),\nIN val2 INT) BEGIN  SELECT valin; END" );

			MySqlCommand cmd = new MySqlCommand("spTest", conn);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.Add( "?valin", 20.4 );
			cmd.Parameters.Add( "?val2", 4 );
			object val = cmd.ExecuteScalar();
			Assert.AreEqual( 20.4, val );

			// create our second procedure
			execSQL("DROP PROCEDURE spTest");
			execSQL("CREATE PROCEDURE spTest( \r\n) BEGIN  SELECT 4; END" );
			cmd.Parameters.Clear();
			val = cmd.ExecuteScalar();
			Assert.AreEqual(4, val);
		}


		/// <summary>
		/// Bug #10644 Cannot call a stored function directly from Connector/Net 
		/// </summary>
		[Test]
		public void CallingStoredFunctionasProcedure()
		{
			if (! Is50) return;

			execSQL("DROP FUNCTION IF EXISTS spFunc");
			execSQL("CREATE FUNCTION spFunc(valin int) RETURNS INT BEGIN return valin * 2; END");
			MySqlCommand cmd = new MySqlCommand("spFunc", conn);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.Add("?valin", 22);
			cmd.Parameters.Add("retval", MySqlDbType.Int32);
			cmd.Parameters[1].Direction = ParameterDirection.ReturnValue;
			cmd.ExecuteNonQuery();
			Assert.AreEqual(44, cmd.Parameters[1].Value);
		}

		/// <summary>
		/// Bug #11450  	Connector/Net, current database and stored procedures
		/// </summary>
		[Test]
		public void NoDefaultDatabase()
		{
			if (! Is50) return;
			
			// create our procedure
			execSQL("DROP PROCEDURE IF EXISTS spTest");
			execSQL("CREATE PROCEDURE spTest() BEGIN  SELECT 4; END" );

			string newConnStr = GetConnectionString(false);
			MySqlConnection c = new MySqlConnection(newConnStr);
			try 
			{
				c.Open();
				MySqlCommand cmd2 = new MySqlCommand("use test", c);
				cmd2.ExecuteNonQuery();

				MySqlCommand cmd = new MySqlCommand("spTest", c);
				cmd.CommandType = CommandType.StoredProcedure;
				object val = cmd.ExecuteScalar();
				Assert.AreEqual(4, val);

				cmd2.CommandText = "use mysql";
				cmd2.ExecuteNonQuery();

				cmd.CommandText = "test.spTest";
				val = cmd.ExecuteScalar();
				Assert.AreEqual(4, val);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}
			finally 
			{
				c.Close();
			}
				
		}
	}
}
