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
using System.IO;
using NUnit.Framework;

namespace MySql.Data.MySqlClient.Tests
{
	[TestFixture]
	public class ConnectionStringBuilder : BaseTest
	{
        [Test]
        public void Simple()
        {
            MySqlConnectionStringBuilder sb = null;
            sb = new MySqlConnectionStringBuilder();
            sb.ConnectionString = "server=localhost;uid=reggie;pwd=pass;port=1111;" +
                "connection timeout=23; pooling=true; min pool size=33; " +
                "max pool size=66;keepalive=1";
            Assert.AreEqual("localhost", sb.Server);
            Assert.AreEqual("reggie", sb.UserID);
            Assert.AreEqual("pass", sb.Password);
            Assert.AreEqual(1111, sb.Port);
            Assert.AreEqual(23, sb.ConnectionTimeout);
            Assert.IsTrue(sb.Pooling);
            Assert.AreEqual(33, sb.MinimumPoolSize);
            Assert.AreEqual(66, sb.MaximumPoolSize);
            Assert.AreEqual(sb.Keepalive, 1);

            try
            {
                sb.ConnectionString = "server=localhost;badkey=badvalue";
                Assert.Fail("This should not work");
            }
            catch (ArgumentException)
            {
            }

            sb.Clear();
            Assert.AreEqual(15, sb.ConnectionTimeout);
            Assert.AreEqual(true, sb.Pooling);
            Assert.AreEqual(3306, sb.Port);
            Assert.AreEqual(String.Empty, sb.Server);
            Assert.AreEqual(false, sb.PersistSecurityInfo);
            Assert.AreEqual(0, sb.ConnectionLifeTime);
            Assert.IsFalse(sb.ConnectionReset);
            Assert.AreEqual(0, sb.MinimumPoolSize);
            Assert.AreEqual(100, sb.MaximumPoolSize);
            Assert.AreEqual(String.Empty, sb.UserID);
            Assert.AreEqual(String.Empty, sb.Password);
            Assert.AreEqual(false, sb.UseUsageAdvisor);
            Assert.AreEqual(String.Empty, sb.CharacterSet);
            Assert.AreEqual(false, sb.UseCompression);
            Assert.AreEqual("MYSQL", sb.PipeName);
            Assert.IsFalse(sb.Logging);
            Assert.IsFalse(sb.UseOldSyntax);
            Assert.IsTrue(sb.AllowBatch);
            Assert.IsFalse(sb.ConvertZeroDateTime);
            Assert.AreEqual("MYSQL", sb.SharedMemoryName);
            Assert.AreEqual(String.Empty, sb.Database);
            Assert.AreEqual(MySqlConnectionProtocol.Sockets, sb.ConnectionProtocol);
            Assert.IsFalse(sb.AllowZeroDateTime);
            Assert.IsFalse(sb.UsePerformanceMonitor);
            Assert.AreEqual(25, sb.ProcedureCacheSize);
            Assert.AreEqual(0, sb.Keepalive);
        }

        /// <summary>
        /// Bug #37955 Connector/NET keeps adding the same option to the connection string
        /// </summary>
        [Test]
        public void SettingValueMultipeTimes()
        {
            MySqlConnectionStringBuilder s = new MySqlConnectionStringBuilder();
            s["database"] = "test";
            s["database"] = "test2";
            Assert.AreEqual("database=test2", s.ConnectionString);
        }
    }
}
