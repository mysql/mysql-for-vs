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

            string sql = "SELECT * FROM Test; SELECT * FROM Test WHERE id > 2";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                reader.Read();
                reader.GetInt32(0);  // access  the first field
                reader.Read();
                Assert.IsTrue(reader.NextResult());
                reader.Read();
                Assert.AreEqual("Test3", reader.GetString(1));
                Assert.IsFalse(reader.NextResult());
            }
            string log = listener.Strings[0];
            Assert.IsTrue(log.Contains(sql));
            Assert.IsTrue(log.Contains("Rows returned: 4"));
            Assert.IsTrue(log.Contains("Rows returned: 2"));
            Assert.IsTrue(log.Contains("some fields not accessed (name)"));
            Assert.IsTrue(log.Contains("some fields not accessed (id)"));
            Assert.IsTrue(log.Contains("UA Warning: not all rows were read.  Skipped 2 rows"));
            Assert.IsTrue(log.Contains("UA Warning: not all rows were read.  Skipped 1 rows"));
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
                Assert.IsTrue(reader.NextResult());
                reader.Read();
                reader.Read();
                Assert.IsFalse(reader.NextResult());
            }
            Assert.IsTrue(listener.Find("UA Warning: not all rows were read") > 0);
        }

    }
}
