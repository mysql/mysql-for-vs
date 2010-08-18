// Copyright (c) 2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
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
using MySql.Data.MySqlClient.Tests;
using System.Data.EntityClient;
using System.Data.Common;
using NUnit.Framework;
using System.Data.Objects;

namespace MySql.Data.Entity.Tests
{
	[TestFixture]
	public class ModelFirst : BaseEdmTest
	{
        public ModelFirst()
            : base()
        {
        }

        [Test]
        public void CreateDatabase()
        {
            using (testEntities ctx = new testEntities())
            {
                ctx.DeleteDatabase();
                ctx.CreateDatabase();
                Assert.IsTrue(ctx.DatabaseExists());
            }
        }

        [Test]
        public void CreateDatabaseScript()
        {
            using (testEntities ctx = new testEntities())
            {
                string s = ctx.CreateDatabaseScript();
            }
        }

        [Test]
        public void DeleteDatabase()
        {
            using (testEntities ctx = new testEntities())
            {
                ctx.DeleteDatabase();
            }

            using (MySqlConnection c = new MySqlConnection("database=mysql;uid=root;pooling=false"))
            {
                c.Open();
                MySqlDataAdapter da =new MySqlDataAdapter("SHOW DATABASES LIKE 'test'", c);
                DataTable dt =new DataTable();
                da.Fill(dt);
                Assert.AreEqual(0, dt.Rows.Count);
            }
        }

        [Test]
        public void DatabaseExists()
        {
            using (testEntities ctx = new testEntities())
            {
                Assert.IsTrue(ctx.DatabaseExists());
                ctx.DeleteDatabase();
                Assert.IsFalse(ctx.DatabaseExists());
            }
        }
    }
}