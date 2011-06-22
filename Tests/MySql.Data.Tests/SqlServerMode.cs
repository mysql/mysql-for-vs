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
using System.Threading;
using MySql.Data.MySqlClient;
using NUnit.Framework;

namespace MySql.Data.MySqlClient.Tests
{
	/// <summary>
	/// Summary description for BlobTests.
	/// </summary>
	[TestFixture]
	public class SqlServerMode : BaseTest
	{
        public SqlServerMode()
        {
            csAdditions += ";sqlservermode=yes;";
        }

        [Test]
        public void Simple()
        {
            execSQL("CREATE TABLE Test (id INT, name VARCHAR(20))");
            execSQL("INSERT INTO Test VALUES (1, 'A')");

            MySqlCommand cmd = new MySqlCommand("SELECT [id], [name] FROM [Test]", conn);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                reader.Read();
                Assert.AreEqual(1, reader.GetInt32(0));
                Assert.AreEqual("A", reader.GetString(1));
            }
        }
	}
}