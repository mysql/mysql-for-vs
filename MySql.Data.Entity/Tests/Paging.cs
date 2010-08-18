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
using System.Linq;
using System.Data.EntityClient;
using System.Data.Common;
using NUnit.Framework;
using System.Data.Objects;

namespace MySql.Data.Entity.Tests
{
	[TestFixture]
	public class Paging : BaseEdmTest
	{
        public Paging()
            : base()
        {
        }

        private EntityConnection GetEntityConnection()
        {
            string connectionString = String.Format(
                "metadata=TestDB.csdl|TestDB.msl|TestDB.ssdl;provider=MySql.Data.MySqlClient; provider connection string=\"{0}\"", GetConnectionString(true));
            EntityConnection connection = new EntityConnection(connectionString);
            return connection;
        }

        [Test]
        public void Top()
        {
            using (testEntities context = new testEntities())
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Companies LIMIT 2", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                int i = 0;
                var query = context.Companies.Top("2");
                foreach (Company c in query)
                {
                    Assert.AreEqual(dt.Rows[i++]["id"], c.Id);
                }
            }
        }

        [Test]
        public void Skip()
        {
            using (testEntities context = new testEntities())
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Companies LIMIT 3,20", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                int i = 0;
                var query = context.Companies.Skip("it.Id", "3");
                foreach (Company c in query)
                {
                    Assert.AreEqual(dt.Rows[i++]["id"], c.Id);
                }
            }
        }

        [Test]
        public void SkipAndTakeSimple()
        {
            using (testEntities context = new testEntities())
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Companies LIMIT 2,2", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                int i = 0;
                var query = context.Companies.Skip("it.Id", "2").Top("2");
                foreach (Company c in query)
                {
                    Assert.AreEqual(dt.Rows[i++]["id"], c.Id);
                }
                Assert.AreEqual(2, i);
            }
        }

        /// <summary>
        /// Bug #45723 Entity Framework DbSortExpression not processed when using Skip & Take  
        /// </summary>
        [Test]
        public void SkipAndTakeWithOrdering()
        {
            using (testEntities context = new testEntities())
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Companies ORDER BY Name DESC LIMIT 2,2", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                int i = 0;
                var query = context.Companies.OrderByDescending(q => q.Name).Skip(2).Take(2).ToList();
                foreach (Company c in query)
                    Assert.AreEqual(dt.Rows[i++]["Name"], c.Name);
            }
        }
    }
}