// Copyright (C) 2004-2007 MySQL AB
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
using System.Diagnostics;
using NUnit.Framework;

namespace MySql.Data.MySqlClient.Tests
{
    [TestFixture]
    public class UsageAdvisorTests : BaseTest
    {
        public UsageAdvisorTests()
        {
            csAdditions = ";Usage Advisor=true;";
        }

        public override void Setup()
        {
            base.Setup();
            createTable("CREATE TABLE Test (id int, name VARCHAR(200))", "INNODB");
        }

        [Test]
        public void NotReadingEveryField()
        {
            execSQL("INSERT INTO Test VALUES (1, 'Test1')");
            execSQL("INSERT INTO Test VALUES (2, 'Test2')");
            execSQL("INSERT INTO Test VALUES (3, 'Test3')");
            execSQL("INSERT INTO Test VALUES (4, 'Test4')");

            Trace.Listeners.Clear();
            GenericListener listener = new GenericListener();
            Trace.Listeners.Add(listener);

            MySqlCommand cmd = new MySqlCommand("SELECT * FROM Test; SELECT * FROM Test WHERE id > 2", conn);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                reader.Read();
                reader.GetInt32(0);  // access  the first field
                reader.Read();

                Assert.IsTrue(reader.NextResult());
                Assert.IsTrue(listener.Find("Fields not accessed:  name") != 0);

                reader.Read();
                listener.Clear();

                Assert.AreEqual("Test3", reader.GetString(1));
                Assert.IsFalse(reader.NextResult());
                Assert.IsTrue(listener.Find("Fields not accessed:  id") > 0);
            }
        }

        [Test]
        public void NotReadingEveryRow()
        {
            execSQL("INSERT INTO Test VALUES (1, 'Test1')");
            execSQL("INSERT INTO Test VALUES (2, 'Test2')");
            execSQL("INSERT INTO Test VALUES (3, 'Test3')");
            execSQL("INSERT INTO Test VALUES (4, 'Test4')");

            Trace.Listeners.Clear();
            GenericListener listener = new GenericListener();
            Trace.Listeners.Add(listener);

            MySqlCommand cmd = new MySqlCommand("SELECT * FROM Test; SELECT * FROM Test WHERE id > 2", conn);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                reader.Read();
                reader.Read();

                listener.Clear();
                Assert.IsTrue(reader.NextResult());
                Assert.IsTrue(listener.Find("Reason: Not all rows in resultset were read.") > 0);

                reader.Read();
                reader.Read();
                listener.Clear();

                Assert.IsFalse(reader.NextResult());
                Assert.IsTrue(listener.Find("Reason: Not all rows in resultset were read.") > 0);
            }
        }

    }
}
