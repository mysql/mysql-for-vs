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
	/// <summary>
	/// Summary description for BlobTests.
	/// </summary>
    [TestFixture]
    public class OrderingAndGrouping : BaseEdmTest
    {
        public OrderingAndGrouping()
            : base()
        {
            csAdditions += ";logging=true;";
        }

        private EntityConnection GetEntityConnection()
        {
            string connectionString = String.Format(
                "metadata=TestDB.csdl|TestDB.msl|TestDB.ssdl;provider=MySql.Data.MySqlClient; provider connection string=\"{0}\"", GetConnectionString(true));
            EntityConnection connection = new EntityConnection(connectionString);
            return connection;
        }

        [Test]
        public void OrderBySimple()
        {
            MySqlDataAdapter da = new MySqlDataAdapter(
                "SELECT id FROM Companies c ORDER BY c.Name", conn);
            DataTable dt = new DataTable();
            da.Fill(dt);

            using (testEntities context = new testEntities())
            {
                string sql = "SELECT VALUE c FROM Companies AS c ORDER BY c.Name";
                ObjectQuery<Company> query = context.CreateQuery<Company>(sql);

                int i = 0;
                foreach (Company c in query)
                    Assert.AreEqual(dt.Rows[i++][0], c.Id);
            }
        }

        [Test]
        public void OrderByWithPredicate()
        {
            using (testEntities context = new testEntities())
            {
                using (EntityConnection ec = context.Connection as EntityConnection)
                {
                    ec.Open();
                    MySqlDataAdapter da = new MySqlDataAdapter(
                        "SELECT id FROM Companies c WHERE c.NumEmployees > 100 ORDER BY c.Name", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    string sql = "SELECT VALUE c FROM Companies AS c WHERE c.NumEmployees > 100 ORDER BY c.Name";
                    ObjectQuery<Company> query = context.CreateQuery<Company>(sql);

                    int i = 0;
                    foreach (Company c in query)
                        Assert.AreEqual(dt.Rows[i++][0], c.Id);
                }
            }
        }
    }
}