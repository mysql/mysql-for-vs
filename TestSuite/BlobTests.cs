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
using System.Threading;
using MySql.Data.MySqlClient;
using NUnit.Framework;

namespace MySql.Data.MySqlClient.Tests
{
	/// <summary>
	/// Summary description for BlobTests.
	/// </summary>
	[NUnit.Framework.TestFixture]
	public class BlobTests : BaseTest
	{
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			Open();

			execSQL("DROP TABLE IF EXISTS Test");
			execSQL("CREATE TABLE Test (id INT NOT NULL, blob1 LONGBLOB, text1 LONGTEXT, PRIMARY KEY(id))");
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown() 
		{
			Close();
		}

		[Test]
		public void InsertBinary() 
		{
			int lenIn = 400000;
			byte[] dataIn = Utils.CreateBlob(lenIn);

			MySqlCommand cmd = new MySqlCommand("TRUNCATE TABLE Test", conn);
			cmd.ExecuteNonQuery();

			cmd.CommandText = "INSERT INTO Test VALUES (?id, ?b1, NULL)";
			cmd.Parameters.Add( new MySqlParameter("?id", 1));
			cmd.Parameters.Add( new MySqlParameter("?b1", dataIn));
			int rows = cmd.ExecuteNonQuery();

			byte[] dataIn2 = Utils.CreateBlob(lenIn);
			cmd.Parameters[0].Value = 2;
			cmd.Parameters[1].Value = dataIn2;
			rows += cmd.ExecuteNonQuery();

			Assert.AreEqual( 2, rows, "Checking insert rowcount" );

			MySqlDataReader reader = null;
			try 
			{
				cmd.CommandText = "SELECT * FROM Test";
				reader = cmd.ExecuteReader();
				Assert.AreEqual( true, reader.HasRows, "Checking HasRows" );
			
				reader.Read();

				byte[] dataOut = new byte[ lenIn ];
				long lenOut = reader.GetBytes( 1, 0, dataOut, 0, lenIn );

				Assert.AreEqual( lenIn, lenOut, "Checking length of binary data (row 1)" );

				// now see if the buffer is intact
				for (int x=0; x < dataIn.Length; x++) 
					Assert.AreEqual( dataIn[x], dataOut[x], "Checking first binary array at " + x );

				// now we test chunking
				int pos = 0;
				int lenToRead = dataIn.Length;
				while (lenToRead > 0) 
				{
					int size = Math.Min( lenToRead, 1024 );
					int read = (int)reader.GetBytes( 1, pos, dataOut, pos, size );
					lenToRead -= read;
					pos += read;
				}
				// now see if the buffer is intact
				for (int x=0; x < dataIn.Length; x++) 
					Assert.AreEqual( dataIn[x], dataOut[x], "Checking first binary array at " + x );

				reader.Read();
				lenOut = reader.GetBytes( 1, 0, dataOut, 0, lenIn );
				Assert.AreEqual( lenIn, lenOut, "Checking length of binary data (row 2)" );

				// now see if the buffer is intact
				for (int x=0; x < dataIn2.Length; x++) 
					Assert.AreEqual( dataIn2[x], dataOut[x], "Checking second binary array at " + x );
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
		public void GetChars() 
		{
			InternalGetChars(false);
			if (!Is41 && !Is50) return;
			InternalGetChars(true);
		}

		private void InternalGetChars( bool prepare ) 
		{
			execSQL("TRUNCATE TABLE Test");

			char[] data = new char[20000];
			for (int x=0; x < data.Length; x++) 
				data[x] = (char)(65 + (x%20));

			MySqlCommand cmd = new MySqlCommand("INSERT INTO Test VALUES (1, NULL, ?text1)", conn);
			cmd.Parameters.Add( "?text1", data );
			if (prepare)
				cmd.Prepare();
			cmd.ExecuteNonQuery();

			cmd.CommandText = "SELECT * FROM Test";
			if (prepare)
				cmd.Prepare();
			MySqlDataReader reader = null;
			try 
			{
				reader = cmd.ExecuteReader();
				reader.Read();

				// now we test chunking
				char[] dataOut = new char[data.Length];
				int pos = 0;
				int lenToRead = data.Length;
				while (lenToRead > 0) 
				{
					int size = Math.Min( lenToRead, 1024 );
					int read = (int)reader.GetChars( 2, pos, dataOut, pos, size );
					lenToRead -= read;
					pos += read;
				}
				// now see if the buffer is intact
				for (int x=0; x < data.Length; x++) 
					Assert.AreEqual( data[x], dataOut[x], "Checking first text array at " + x );

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
		public void InsertText() 
		{
			InternalInsertText(false);
			if (!Is41 && !Is50) return;
			InternalInsertText(true);
		}

		private void InternalInsertText(bool prepare) 
		{
			byte[] data = new byte[1024];
			for (int x=0; x < 1024; x++) 
				data[x] = (byte)(65 + (x%20));

			// Create sample table
			execSQL("TRUNCATE TABLE Test");
			MySqlCommand cmd = new MySqlCommand("INSERT INTO Test VALUES (1, ?b1, ?t1)", conn);
			cmd.Parameters.Add( new MySqlParameter("?t1", data));
			cmd.Parameters.Add( new MySqlParameter("?b1", "This is my blob data"));
			if (prepare) cmd.Prepare();
			int rows = cmd.ExecuteNonQuery();
			Assert.AreEqual( 1, rows, "Checking insert rowcount" );

			cmd.CommandText = "INSERT INTO Test VALUES(2, ?b1, ?t1)";
			cmd.Parameters.Clear();
			cmd.Parameters.Add( "?t1", DBNull.Value );
			string str = "This is my text value";
			cmd.Parameters.Add( new MySqlParameter( "?b1", MySqlDbType.LongBlob, str.Length,
				ParameterDirection.Input, true, 0, 0, "b1", DataRowVersion.Current, str ) );
			rows = cmd.ExecuteNonQuery();
			Assert.AreEqual( 1, rows, "Checking insert rowcount" );

			MySqlDataReader reader = null;
			try 
			{
				cmd.CommandText = "SELECT * FROM Test";
				if (prepare) cmd.Prepare();
				reader = cmd.ExecuteReader();
				Assert.AreEqual( true, reader.HasRows, "Checking HasRows" );
			
				Assert.IsTrue( reader.Read() );

				Assert.AreEqual( "This is my blob data", reader.GetString(1));
				string s = reader.GetString(2);
				Assert.AreEqual( 1024, s.Length, "Checking length returned " );
				Assert.AreEqual( "ABCDEFGHI", s.Substring(0, 9), "Checking first few chars of string" );

				Assert.IsTrue( reader.Read() );
				Assert.AreEqual( DBNull.Value, reader.GetValue(2) );
				Assert.AreEqual( "This is my text value", reader.GetString(1) );
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
		public void UpdateDataSet() 
		{
			execSQL("TRUNCATE TABLE Test");
			execSQL("INSERT INTO Test VALUES( 1, NULL, 'Text field' )");

			MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Test", conn);
			MySqlCommandBuilder cb = new MySqlCommandBuilder(da);
			DataTable dt = new DataTable();
			da.Fill(dt);

			string s = (string)dt.Rows[0][2];
			Assert.AreEqual( "Text field", s );

			byte[] inBuf = Utils.CreateBlob(512);
			dt.Rows[0]["blob1"] = inBuf;
			DataTable changes = dt.GetChanges();
			da.Update( changes );
			dt.AcceptChanges();

			dt.Clear();
			da.Fill(dt);

			byte[] outBuf = (byte[])dt.Rows[0]["blob1"];
			Assert.AreEqual( inBuf.Length, outBuf.Length, "checking length of updated buffer" );
			for (int y=0; y < inBuf.Length; y++)
				Assert.AreEqual( inBuf[y], outBuf[y], "checking array data" );
		}

		[Test]
		public void GetCharsOnLongTextColumn() 
		{
			execSQL("INSERT INTO Test (id, text1) VALUES(1, 'Test')");

			MySqlCommand cmd = new MySqlCommand("SELECT id, text1 FROM Test", conn);
			MySqlDataReader reader = null;
			try 
			{
				char[] buf = new char[2];

				reader = cmd.ExecuteReader();
				reader.Read();
				reader.GetChars( 1, 0, buf, 0, 2 );
				Assert.AreEqual( 'T', buf[0] );
				Assert.AreEqual( 'e', buf[1] );
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
	}
}
