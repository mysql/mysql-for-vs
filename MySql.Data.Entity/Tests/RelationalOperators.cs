// Copyright (C) 2008-2009 Sun Microsystems, Inc.
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