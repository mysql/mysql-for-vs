// Copyright (c) 2009 Sun Microsystems, Inc.
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
    public class LoggingTests : BaseTest
    {
        public LoggingTests()
        {
            csAdditions = ";logging=true;";
        }

        public override void Setup()
        {
            base.Setup();
            createTable("CREATE TABLE Test (id int, name VARCHAR(200))", "INNODB");
        }

        [Test]
        public void SimpleLogging()
        {
            execSQL("INSERT INTO Test VALUES (1, 'Test1')");
            execSQL("INSERT INTO Test VALUES (2, 'Test2')");
            execSQL("INSERT INTO Test VALUES (3, 'Test3')");
            execSQL("INSERT INTO Test VALUES (4, 'Test4')");

            MySqlTrace.Listeners.Clear();
            MySqlTrace.Switch.Level = SourceLevels.All;
            GenericListener listener = new GenericListener();
            MySqlTrace.Listeners.Add(listener);

            MySqlCommand cmd = new MySqlCommand("SELECT * FROM Test", conn);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
            }
            Assert.AreEqual(4, listener.Strings.Count);
            Assert.IsTrue(listener.Strings[0].Contains("Query Opened: SELECT * FROM Test"));
            Assert.IsTrue(listener.Strings[1].Contains("Resultset Opened: field(s) = 2, affected rows = -1, inserted id = -1"));
            Assert.IsTrue(listener.Strings[2].Contains("Resultset Closed. Total rows=4, skipped rows=4, size (bytes)=32"));
            Assert.IsTrue(listener.Strings[3].Contains("Query Closed"));
        }

        [Test]
        public void Warnings()
        {
            execSQL("DROP TABLE IF EXISTS Test");
            execSQL("CREATE TABLE Test(id INT, name VARCHAR(5))");

            MySqlTrace.Listeners.Clear();
            MySqlTrace.Switch.Level = SourceLevels.All;
            GenericListener listener = new GenericListener();
            MySqlTrace.Listeners.Add(listener);

            MySqlCommand cmd = new MySqlCommand("INSERT IGNORE INTO Test VALUES (1, 'abcdef')", conn);
            cmd.ExecuteNonQuery();

            Assert.AreEqual(9, listener.Strings.Count);
            Assert.IsTrue(listener.Strings[0].Contains("Query Opened: INSERT IGNORE INTO Test VALUES (1, 'abcdef')"));
            Assert.IsTrue(listener.Strings[1].Contains("Resultset Opened: field(s) = 0, affected rows = 1, inserted id = 0"));
            Assert.IsTrue(listener.Strings[2].Contains("Resultset Closed. Total rows=0, skipped rows=0, size (bytes)=0"));
            Assert.IsTrue(listener.Strings[3].Contains("Query Opened: SHOW WARNINGS"));
            Assert.IsTrue(listener.Strings[4].Contains("Resultset Opened: field(s) = 3, affected rows = -1, inserted id = -1"));
            Assert.IsTrue(listener.Strings[5].Contains("Resultset Closed. Total rows=1, skipped rows=0, size (bytes)=55"));
            Assert.IsTrue(listener.Strings[6].Contains("Query Closed"));
            Assert.IsTrue(listener.Strings[7].Contains("MySql Warning: Level=Warning, Code=1265, Message=Data truncated for column 'name' at row 1"));
            Assert.IsTrue(listener.Strings[8].Contains("Query Closed"));
        }
    }
}
