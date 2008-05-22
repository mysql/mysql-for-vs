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
	public class AsyncTests : BaseTest
	{
		[Test]
		public void ExecuteNonQuery()
		{
			if (Version < new Version(5, 0)) return;

			execSQL("CREATE TABLE test (id int)");
			execSQL("CREATE PROCEDURE spTest() BEGIN SET @x=0; REPEAT INSERT INTO test VALUES(@x); " +
				"SET @x=@x+1; UNTIL @x = 300 END REPEAT; END");

			MySqlCommand proc = new MySqlCommand("spTest", conn);
			proc.CommandType = CommandType.StoredProcedure;
			IAsyncResult iar = proc.BeginExecuteNonQuery();
			int count = 0;
			while (!iar.IsCompleted)
			{
				count++;
				System.Threading.Thread.Sleep(20);
			}
			proc.EndExecuteNonQuery(iar);
			Assert.IsTrue(count > 0);

			proc.CommandType = CommandType.Text;
			proc.CommandText = "SELECT COUNT(*) FROM test";
			object cnt = proc.ExecuteScalar();
			Assert.AreEqual(300, cnt);
		}

		[Test]
		public void ExecuteReader()
		{
            if (Version < new Version(5, 0)) return;

			execSQL("CREATE TABLE test (id int)");
			execSQL("CREATE PROCEDURE spTest() BEGIN INSERT INTO test VALUES(1); " +
				"SELECT SLEEP(2); SELECT 'done'; END");

			MySqlCommand proc = new MySqlCommand("spTest", conn);
			proc.CommandType = CommandType.StoredProcedure;
			IAsyncResult iar = proc.BeginExecuteReader();
			int count = 0;
			while (!iar.IsCompleted)
			{
				count++;
				System.Threading.Thread.Sleep(20);
			}

			using (MySqlDataReader reader = proc.EndExecuteReader(iar))
            {
				Assert.IsNotNull(reader);
				Assert.IsTrue(count > 0, "count > 0");
				Assert.IsTrue(reader.Read(), "can read");
				Assert.IsTrue(reader.NextResult());
				Assert.IsTrue(reader.Read());
				Assert.AreEqual("done", reader.GetString(0));
				reader.Close();

				proc.CommandType = CommandType.Text;
				proc.CommandText = "SELECT COUNT(*) FROM test";
				object cnt = proc.ExecuteScalar();
				Assert.AreEqual(1, cnt);
			}
		}

		[Test]
		public void ThrowingExceptions()
		{
			MySqlCommand cmd = new MySqlCommand("SELECT xxx", conn);
			IAsyncResult r = cmd.BeginExecuteReader();
			try
			{
                using (MySqlDataReader reader = cmd.EndExecuteReader(r))
                {
                    Assert.Fail("EndExecuteReader should have thrown an exception");
                }
			}
			catch (MySqlException)
			{
			}
		}
	}
}
