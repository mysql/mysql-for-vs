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
using MySql.Data.MySqlClient;
using System.Data;
using NUnit.Framework;
using System.Threading;
using System.Collections;

namespace MySql.Data.MySqlClient.Tests
{
	/// <summary>
	/// Summary description for ConnectionTests.
	/// </summary>
	[TestFixture] 
	public class StressTests : BaseTest
	{
		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			Open();
			execSQL("DROP TABLE IF EXISTS Test");
			execSQL("CREATE TABLE Test (id INT NOT NULL, name varchar(100), blob1 LONGBLOB, text1 TEXT, " +
				"PRIMARY KEY(id))");
		}

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			Close();
		}

		[Test]
		public void TestMultiPacket()
		{
			int len = 20000000;

			// currently do not test this with compression
			if (conn.UseCompression) return;

			execSQL("set @@global.max_allowed_packet=35000000");

			MySqlConnection c = new MySqlConnection(conn.ConnectionString + ";pooling=false");
			c.Open();

			byte[] dataIn = Utils.CreateBlob(len);
			byte[] dataIn2 = Utils.CreateBlob(len);

			MySqlCommand cmd = new MySqlCommand("INSERT INTO Test VALUES (?id, NULL, ?blob, NULL )", c);
			cmd.Parameters.Add(new MySqlParameter("?id", 1));
			cmd.Parameters.Add(new MySqlParameter("?blob", dataIn));
			try 
			{
				cmd.ExecuteNonQuery();
			}
			catch (Exception ex) 
			{
				Assert.Fail(ex.Message);
			}

			cmd.Parameters[0].Value = 2;
			cmd.Parameters[1].Value = dataIn2;
			cmd.ExecuteNonQuery();


			cmd.CommandText = "SELECT * FROM Test";
			MySqlDataReader reader = null;
			
			try 
			{
				reader = cmd.ExecuteReader();
				reader.Read();
				byte[] dataOut = new byte[ len ];
				long count = reader.GetBytes(2, 0, dataOut, 0, len);
				Assert.AreEqual(len, count);

				for (int i=0; i < len; i++)
					Assert.AreEqual(dataIn[i], dataOut[i]);

				reader.Read();
				count = reader.GetBytes(2, 0, dataOut, 0, len);
				Assert.AreEqual(len, count);

				for (int i=0; i < len; i++)
					Assert.AreEqual(dataIn2[i], dataOut[i]);
			}
			catch (Exception ex) 
			{
				Assert.Fail(ex.Message);
			}
			finally 
			{
				if (reader != null) reader.Close();
				c.Close();
			}
			execSQL("set @@global.max_allowed_packet=1047552");
		}

		[Test]
		public void TestSequence()
		{
            MySqlCommand cmd = new MySqlCommand("insert into Test (id, name) values (?id, 'test')", conn);
			cmd.Parameters.Add( new MySqlParameter("?id", 1));

			for (int i=1; i <= 8000; i++)
			{
				cmd.Parameters[0].Value = i;
				cmd.ExecuteNonQuery();
			}
				
			int i2 = 0;
			cmd = new MySqlCommand("select * from Test", conn);
			MySqlDataReader reader = null;
			try 
			{
				reader = cmd.ExecuteReader();
				while (reader.Read())
				{
					Assert.AreEqual( i2+1, reader.GetInt32(0), "Sequence out of order" );
					i2++;
				}
				reader.Close();

				Assert.AreEqual( 8000, i2 );
				cmd = new MySqlCommand("delete from Test where id >= 100", conn);
				cmd.ExecuteNonQuery();
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

    #region Configs

	[Category("Compressed")]
    public class StressTestsSocketCompressed : StressTests
    {
        protected override string GetConnectionInfo()
        {
            return String.Format("port={0};compress=true", port);
        }
    }

	[Category("Pipe")]
    public class StressTestsPipe : StressTests
    {
        protected override string GetConnectionInfo()
        {
            return String.Format("protocol=pipe;pipe name={0}", pipeName);
        }
    }

    [Category("Compressed")]
    [Category("Pipe")]
    public class StressTestsPipeCompressed : StressTests
    {
        protected override string GetConnectionInfo()
        {
            return String.Format("protocol=pipe;pipe name={0};compress=true", pipeName);
        }
    }

    [Category("SharedMemory")]
    public class StressTestsSharedMemory : StressTests
    {
        protected override string GetConnectionInfo()
        {
            return String.Format("protocol=memory; shared memory name={0}", memoryName);
        }
    }

    [Category("Compressed")]
    [Category("SharedMemory")]
    public class StressTestsSharedMemoryCompressed : StressTests
    {
        protected override string GetConnectionInfo()
        {
            return String.Format("protocol=memory; shared memory name={0};compress=true", memoryName);
        }
    }

    #endregion

}
