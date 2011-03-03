// Copyright (c) 2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
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
using MySql.Data.Entity.Tests.Properties;

namespace MySql.Data.Entity.Tests
{
    [TestFixture]
    public class OrderingAndGrouping : BaseEdmTest
    {
        [Test]
        public void OrderBySimple()
        {
            MySqlDataAdapter da = new MySqlDataAdapter(
                "SELECT id FROM Companies c ORDER BY c.Name", conn);
            DataTable dt = new DataTable();
            da.Fill(dt);

            using (testEntities context = new testEntities())
            {
                string eSql = "SELECT VALUE c FROM Companies AS c ORDER BY c.Name";
                ObjectQuery<Company> query = context.CreateQuery<Company>(eSql);

                string sql = query.ToTraceString();
                CheckSql(sql, SQLSyntax.OrderBySimple);

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

                    string eSql = "SELECT VALUE c FROM Companies AS c WHERE c.NumEmployees > 100 ORDER BY c.Name";
                    ObjectQuery<Company> query = context.CreateQuery<Company>(eSql);

                    string sql = query.ToTraceString();
                    CheckSql(sql, SQLSyntax.OrderByWithPredicate);

                    int i = 0;
                    foreach (Company c in query)
                        Assert.AreEqual(dt.Rows[i++][0], c.Id);
                }
            }
        }
    }
}