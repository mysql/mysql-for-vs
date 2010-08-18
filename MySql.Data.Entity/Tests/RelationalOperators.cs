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
	public class RelationalOperators : BaseEdmTest
	{
        public RelationalOperators()
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
        public void Except()
        {
/*            using (TestDB.TestDB db = new TestDB.TestDB())
            {
                var q = from c in db.Companies where 
                var query = from o in db.Orders
                            where o.StoreId = 3
                            select o;

                var result = query.First();
            }*/
        }

        [Test]
        public void Intersect()
        {
        }

        [Test]
        public void CrossJoin()
        {
        }

        [Test]
        public void Union()
        {
        }

        [Test]
        public void UnionAll()
        {
            using (testEntities context = new testEntities())
            {
                MySqlDataAdapter da = new MySqlDataAdapter(
                    "SELECT t.Id FROM Toys t UNION ALL SELECT c.Id FROM Companies c", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                string entitySQL = @"(SELECT t.Id, t.Name FROM Toys AS t) 
                UNION ALL (SELECT c.Id, c.Name FROM Companies AS c)";
                ObjectQuery<DbDataRecord> query = context.CreateQuery<DbDataRecord>(entitySQL);
                int i = 0;
                foreach (DbDataRecord r in query)
                {
                    i++;
                }
                Assert.AreEqual(dt.Rows.Count, i);
            }
        }
    }
}