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
	/// Summary description for CompressionTests.
	/// </summary>
	[NUnit.Framework.TestFixture]
	public class ProtocolTests : BaseTest
	{
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			Open();
			execSQL("DROP TABLE IF EXISTS Test");
			execSQL("CREATE TABLE Test (id INT NOT NULL, name VARCHAR(200), dt DATETIME, f FLOAT, blob1 LONGBLOB, text1 LONGTEXT, PRIMARY KEY(id))");
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown() 
		{
		}


		[Test]
		public void Compression() 
		{
			conn.Close();
			conn = new MySqlConnection( conn.ConnectionString + ";compress=true" );
			conn.Open();

			DataReaderTest();
			DataAdapterTest();
			BlobTest();

			conn.Close();
		}

		[Test]
		public void PipeNoCompression() 
		{
			Close();
			csAdditions = ";protocol=pipe;compress=false";
			Open();
			DataReaderTest();
			DataAdapterTest();
			BlobTest();
			Close();
		}

		[Test]
		public void PipeCompression() 
		{
			Close();
			csAdditions = ";protocol=pipe;compress=true";
			Open();
			DataReaderTest();
			DataAdapterTest();
			BlobTest();
			Close();
		}

		[Test]
		public void MemoryNoCompression() 
		{
			if (! Is41 && ! Is50) return;
			Close();
			csAdditions = ";protocol=memory;compress=false";
			Open();
			DataReaderTest();
			DataAdapterTest();
			BlobTest();
			Close();
		}

		[Test]
		public void MemoryCompression() 
		{
			if (! Is41 && ! Is50) return;
			Close();
			csAdditions = ";protocol=memory;compress=true";
			Open();
			DataReaderTest();
			DataAdapterTest();
			BlobTest();
			Close();
		}

		private void LoadTable( int count ) 
		{
			MySqlCommand cmd = new MySqlCommand(
				"INSERT INTO Test VALUES (?id, ?name, ?date, ?float, ?blob, ?text)", conn);
			cmd.Parameters.Add( "?id", MySqlDbType.Int32 );
			cmd.Parameters.Add( "?name", MySqlDbType.VarChar );
			cmd.Parameters.Add( "?date", MySqlDbType.Datetime );
			cmd.Parameters.Add( "?float", MySqlDbType.Float );
			cmd.Parameters.Add( "?blob", MySqlDbType.LongBlob );
			cmd.Parameters.Add( "?text", MySqlDbType.LongBlob );

			byte[] myblob = Utils.CreateBlob(1000);
			for (int i=0; i < count; i++)
			{
				cmd.Parameters[0].Value = i;
				cmd.Parameters[1].Value = "Name " + i;
				cmd.Parameters[2].Value = DateTime.Now;
				cmd.Parameters[3].Value = Convert.ToSingle(i);
				cmd.Parameters[4].Value = myblob;
				cmd.Parameters[5].Value = "my long text " + i;
				cmd.ExecuteNonQuery();
			}
		}

		public void DataReaderTest()
		{
			LoadTable(100);

			MySqlCommand cmd = new MySqlCommand("SELECT * FROM Test WHERE id < 50; SELECT * FROM Test WHERE id >= 50", conn);
			MySqlDataReader reader = null;
			try 
			{
				reader = cmd.ExecuteReader();
				for (int i=0; i < 50; i++) 
					Assert.IsTrue( reader.Read() );
				Assert.AreEqual( 49, reader.GetInt32(0) );
				Assert.AreEqual( "Name 49", reader.GetString(1) );
				Assert.IsFalse( reader.Read() );
			
				Assert.IsTrue( reader.NextResult() );
				for (int i=0; i < 50; i++) 
					Assert.IsTrue( reader.Read() );
				Assert.AreEqual( 99, reader.GetInt32(0) );
				Assert.AreEqual( "Name 99", reader.GetString(1) );
				Assert.IsFalse( reader.Read() );

				Assert.IsFalse( reader.NextResult() );
				reader.Close();

				// now test singlerow
				cmd.CommandText = "SELECT * FROM Test";
				reader = cmd.ExecuteReader( CommandBehavior.SingleRow );
				Assert.IsTrue( reader.Read() );
				Assert.AreEqual( 0, reader.GetInt32( 0 ) );
				Assert.AreEqual( "Name 0", reader.GetString(1) );
				Assert.IsFalse( reader.Read() );
				Assert.IsFalse( reader.NextResult() );
				reader.Close();

				//now test singleresult
				cmd.CommandText = "SELECT * FROM Test WHERE id <= 10; SELECT * FROM Test WHERE id >= 10";
				reader = cmd.ExecuteReader( CommandBehavior.SingleResult );
				Assert.IsTrue( reader.Read() );
				Assert.AreEqual( 0, reader.GetInt32( 0 ) );
				Assert.AreEqual( "Name 0", reader.GetString(1) );
				Assert.IsTrue( reader.Read() );
				Assert.IsFalse( reader.NextResult() );
				reader.Close();
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

		public void DataAdapterTest()
		{
			DataAdapterFillImpl( false );
			execSQL("TRUNCATE TABLE Test");
			if (Is41 || Is50) DataAdapterFillImpl(true);
		}

		private void DataAdapterFillImpl(bool prepare)
		{
			LoadTable(100);

			MySqlDataAdapter da = new MySqlDataAdapter("select * from Test", conn);
			if (prepare) da.SelectCommand.Prepare();
			DataSet ds = new DataSet();
			da.Fill( ds, "Test" );

			Assert.AreEqual( 1, ds.Tables.Count );
			Assert.AreEqual( 100, ds.Tables[0].Rows.Count );

			for (int i=0; i < 100; i++) 
			{
				Assert.AreEqual( i, ds.Tables[0].Rows[i]["id"] );
				Assert.AreEqual( "Name " + i, ds.Tables[0].Rows[i]["name"] );
				Assert.AreEqual( "my long text " + i, ds.Tables[0].Rows[i]["text1"] );
				Assert.AreEqual( i, ds.Tables[0].Rows[i]["f"] );
			}
		}


		public void BlobTest() 
		{
			InsertBinary();
		}

		public void InsertBinary() 
		{
			int lenIn = 400000;
			byte[] dataIn = Utils.CreateBlob(lenIn);

			MySqlCommand cmd = new MySqlCommand( "INSERT INTO Test (id, blob1) VALUES (?id, ?b1)", conn);
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
				cmd.CommandText = "SELECT id, blob1, text1 FROM Test";
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

	}
}
