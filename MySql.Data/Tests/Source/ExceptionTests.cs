// Copyright (c) 2004-2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
//
// MySQL Connector/NET is licensed under the terms of the GPLv2
// <http://www.gnu.org/licenses/old-licenses/gpl-2.0.html>, like most 
// MySQL Connectors. There are special exceptions to the terms and 
// conditions of the GPLv2 as it is applied to this software, see the 
// FLOSS License Exception
// <http://www.mysql.com/about/legal/licensing/foss-exception.html>.
//
// This program is free software; you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published 
// by the Free Software Foundation; version 2 of the License.
//
// This program is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
//
// You should have received a copy of the GNU General Public License along 
// with this program; if not, write to the Free Software Foundation, Inc., 
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using System;
using System.Data;
using System.IO;
using System.Threading;
using NUnit.Framework;

namespace MySql.Data.MySqlClient.Tests
{
	[TestFixture]
	public class ExceptionTests : BaseTest
	{
        public override void Setup()
        {
            base.Setup();
            execSQL("CREATE TABLE Test (id INT NOT NULL, name VARCHAR(100))");
        }

		[Test]
		public void Timeout() 
		{
			for (int i=1; i < 10; i++)
				execSQL("INSERT INTO Test VALUES (" + i + ", 'This is a long text string that I am inserting')");

			// we create a new connection so our base one is not closed
			MySqlConnection c2 = new MySqlConnection(conn.ConnectionString);
			c2.Open();

			KillConnection(c2);
			MySqlCommand cmd = new MySqlCommand("SELECT * FROM Test", c2);
			MySqlDataReader reader = null;

			try 
			{
				reader = cmd.ExecuteReader();
				reader.Read();
				reader.Read();
				reader.Close();
				Assert.Fail("We should not reach this code");
			}
			catch (Exception)
			{
				Assert.AreEqual(ConnectionState.Closed, c2.State);
			}
			finally 
			{
				if (reader != null) reader.Close();
				c2.Close();
			}
		}
#if !CF
        /// <summary>
        /// Bug #27436 Add the MySqlException.Number property value to the Exception.Data Dictionary  
        /// </summary>
        [Test]
        public void ErrorData()
        {
            MySqlCommand cmd = new MySqlCommand("SELEDT 1", conn);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(1064, ex.Data["Server Error Code"]);
            }
        }
#endif
    }
}
