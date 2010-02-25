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