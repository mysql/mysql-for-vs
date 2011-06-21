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
using MySql.Data.Entity.Tests.Properties;

namespace MySql.Data.Entity.Tests
{
    [TestFixture]
    public class AggregateOperators : BaseEdmTest
    {
        [Test]
        public void CountSimple()
        {
            MySqlCommand trueCmd = new MySqlCommand("SELECT COUNT(*) FROM Toys", conn);
            object trueCount = trueCmd.ExecuteScalar();

            using (testEntities context = new testEntities())
            {
                string eSql = "SELECT VALUE Count(t.Id) FROM Toys AS t";
                ObjectQuery<Int32> q = context.CreateQuery<Int32>(eSql);

                string sql = q.ToTraceString();
                CheckSql(sql, SQLSyntax.CountSimple);

                foreach (int count in q)
                    Assert.AreEqual(trueCount, count);
            }
        }

        [Test]
        public void BigCountSimple()
        {
            MySqlCommand trueCmd = new MySqlCommand("SELECT COUNT(*) FROM Toys", conn);
            object trueCount = trueCmd.ExecuteScalar();

            using (testEntities context = new testEntities())
            {
                string eSql = "SELECT VALUE BigCount(t.Id) FROM Toys AS t";
                ObjectQuery<Int32> q = context.CreateQuery<Int32>(eSql);

                string sql = q.ToTraceString();
                CheckSql(sql, SQLSyntax.BigCountSimple);

                foreach (int count in q)
                    Assert.AreEqual(trueCount, count);
            }
        }

        [Test]
        public void CountWithPredicate()
        {
            MySqlCommand trueCmd = new MySqlCommand("SELECT COUNT(*) FROM Toys AS t WHERE t.MinAge > 3", conn);
            object trueCount = trueCmd.ExecuteScalar();

            using (testEntities context = new testEntities())
            {
                string eSql = "SELECT VALUE Count(t.Id) FROM Toys AS t WHERE t.MinAge > 3";
                ObjectQuery<Int32> q = context.CreateQuery<Int32>(eSql);

                string sql = q.ToTraceString();
                CheckSql(sql, SQLSyntax.CountWithPredicate);

                foreach (int count in q)
                    Assert.AreEqual(trueCount, count);
            }
        }

        [Test]
        public void MinSimple()
        {
            MySqlCommand trueCmd = new MySqlCommand("SELECT MIN(minage) FROM Toys", conn);
            int trueMin = (int)trueCmd.ExecuteScalar();

            using (testEntities context = new testEntities())
            {
                string eSql = "SELECT VALUE MIN(t.MinAge) FROM Toys AS t";
                ObjectQuery<Int32> q = context.CreateQuery<Int32>(eSql);

                string sql = q.ToTraceString();
                CheckSql(sql, SQLSyntax.MinSimple);

                foreach (int age in q)
                    Assert.AreEqual(trueMin, age);
            }
        }

        [Test]
        public void MinWithPredicate()
        {
            MySqlCommand trueCmd = new MySqlCommand("SELECT MIN(Freight) FROM Orders WHERE storeId=2", conn);
            object freight = trueCmd.ExecuteScalar();

            using (testEntities context = new testEntities())
            {
                string eSql = "SELECT Min(o.Freight) FROM Orders AS o WHERE o.Store.Id = 2";
                ObjectQuery<DbDataRecord> q = context.CreateQuery<DbDataRecord>(eSql);

                string sql = q.ToTraceString();
                CheckSql(sql, SQLSyntax.MinWithPredicate);

                foreach (DbDataRecord r in q)
                {
                    Assert.AreEqual(freight, r.GetDouble(0));
                }
            }
        }

        [Test]
        public void MinWithGrouping()
        {
            MySqlDataAdapter da = new MySqlDataAdapter(
                "SELECT MIN(Freight) FROM Orders GROUP BY storeId", conn);
            DataTable dt = new DataTable();
            da.Fill(dt);

            using (testEntities context = new testEntities())
            {
                string eSql = "SELECT VALUE Min(o.Freight) FROM Orders AS o GROUP BY o.Store.Id";
                ObjectQuery<Double> q = context.CreateQuery<Double>(eSql);

                string sql = q.ToTraceString();
                CheckSql(sql, SQLSyntax.MinWithGrouping);

                int i = 0;
                foreach (double freight in q)
                    Assert.AreEqual(Convert.ToInt32(dt.Rows[i++][0]), Convert.ToInt32(freight));
            }
        }

        [Test]
        public void MaxSimple()
        {
            MySqlCommand trueCmd = new MySqlCommand("SELECT MAX(minage) FROM Toys", conn);
            int trueMax = (int)trueCmd.ExecuteScalar();

            using (testEntities context = new testEntities())
            {
                string eSql = "SELECT VALUE MAX(t.MinAge) FROM Toys AS t";
                ObjectQuery<Int32> q = context.CreateQuery<Int32>(eSql);

                string sql = q.ToTraceString();
                CheckSql(sql, SQLSyntax.MaxSimple);

                foreach (int max in q)
                    Assert.AreEqual(trueMax, max);
            }
        }

        [Test]
        public void MaxWithPredicate()
        {
            MySqlCommand trueCmd = new MySqlCommand("SELECT MAX(Freight) FROM Orders WHERE storeId=1", conn);
            object freight = trueCmd.ExecuteScalar();

            using (testEntities context = new testEntities())
            {
                string eSql = "SELECT MAX(o.Freight) FROM Orders AS o WHERE o.Store.Id = 1";
                ObjectQuery<DbDataRecord> q = context.CreateQuery<DbDataRecord>(eSql);

                string sql = q.ToTraceString();
                CheckSql(sql, SQLSyntax.MaxWithPredicate);

                foreach (DbDataRecord r in q)
                    Assert.AreEqual(freight, r.GetDouble(0));
            }
        }

        [Test]
        public void MaxWithGrouping()
        {
            MySqlDataAdapter da = new MySqlDataAdapter(
                "SELECT MAX(Freight) FROM Orders GROUP BY StoreId", conn);
            DataTable dt = new DataTable();
            da.Fill(dt);

            using (testEntities context = new testEntities())
            {
                string eSql = "SELECT VALUE MAX(o.Freight) FROM Orders AS o GROUP BY o.Store.Id";
                ObjectQuery<Double> q = context.CreateQuery<Double>(eSql);

                string sql = q.ToTraceString();
                CheckSql(sql, SQLSyntax.MaxWithGrouping);

                int i = 0;
                foreach (double freight in q)
                    Assert.AreEqual(Convert.ToInt32(dt.Rows[i++][0]), Convert.ToInt32(freight));
            }
        }

        [Test]
        public void AverageSimple()
        {
            MySqlCommand trueCmd = new MySqlCommand("SELECT AVG(minAge) FROM Toys", conn);
            object avgAge = trueCmd.ExecuteScalar();

            using (testEntities context = new testEntities())
            {
                string eSql = "SELECT VALUE Avg(t.MinAge) FROM Toys AS t";
                ObjectQuery<Decimal> q = context.CreateQuery<Decimal>(eSql);

                string sql = q.ToTraceString();
                CheckSql(sql, SQLSyntax.AverageSimple);

                foreach (Decimal r in q)
                    Assert.AreEqual(avgAge, r);
            }
        }

        [Test]
        public void AverageWithPredicate()
        {
            MySqlCommand trueCmd = new MySqlCommand("SELECT AVG(Freight) FROM Orders WHERE storeId=3", conn);
            Double freight = (Double)trueCmd.ExecuteScalar();

            using (testEntities context = new testEntities())
            {
                string eSql = "SELECT VALUE AVG(o.Freight) FROM Orders AS o WHERE o.Store.Id = 3";
                ObjectQuery<Double> q = context.CreateQuery<Double>(eSql);

                string sql = q.ToTraceString();
                CheckSql(sql, SQLSyntax.AverageWithPredicate);

                foreach (Double r in q)
                    Assert.AreEqual(Convert.ToInt32(freight), Convert.ToInt32(r));
            }
        }

        [Test]
        public void AverageWithGrouping()
        {
            MySqlDataAdapter da = new MySqlDataAdapter(
                "SELECT AVG(Freight) FROM Orders GROUP BY StoreId", conn);
            DataTable dt = new DataTable();
            da.Fill(dt);

            using (testEntities context = new testEntities())
            {
                string eSql = "SELECT AVG(o.Freight) FROM Orders AS o GROUP BY o.Store.Id";
                ObjectQuery<DbDataRecord> q = context.CreateQuery<DbDataRecord>(eSql);

                string sql = q.ToTraceString();
                CheckSql(sql, SQLSyntax.AverageWithGrouping);

                foreach (object x in q)
                {
                    string s = x.GetType().ToString();
                }
                int i = 0;
                foreach (var freight in q)
                {
                 //   Assert.AreEqual(Convert.ToInt32(dt.Rows[i++][0]), Convert.ToInt32(freight));
                }
            }
        }

        [Test]
        public void SumSimple()
        {
            MySqlCommand trueCmd = new MySqlCommand("SELECT SUM(minage) FROM Toys", conn);
            object sumAge = trueCmd.ExecuteScalar();

            using (testEntities context = new testEntities())
            {
                string eSql = "SELECT VALUE Sum(t.MinAge) FROM Toys AS t";
                ObjectQuery<Int32> q = context.CreateQuery<Int32>(eSql);

                string sql = q.ToTraceString();
                CheckSql(sql, SQLSyntax.SumSimple);

                foreach (int r in q)
                    Assert.AreEqual(sumAge, r);
            }
        }

        [Test]
        public void SumWithPredicate()
        {
            MySqlCommand trueCmd = new MySqlCommand("SELECT SUM(Freight) FROM Orders WHERE storeId=2", conn);
            object freight = trueCmd.ExecuteScalar();

            using (testEntities context = new testEntities())
            {
                string eSql = "SELECT VALUE SUM(o.Freight) FROM Orders AS o WHERE o.Store.Id = 2";
                ObjectQuery<Double> q = context.CreateQuery<Double>(eSql);

                string sql = q.ToTraceString();
                CheckSql(sql, SQLSyntax.SumWithPredicate);

                foreach (Double r in q)
                    Assert.AreEqual(freight, r);
            }
        }

        [Test]
        public void SumWithGrouping()
        {
            MySqlDataAdapter da = new MySqlDataAdapter(
                "SELECT SUM(Freight) FROM Orders GROUP BY StoreId", conn);
            DataTable dt = new DataTable();
            da.Fill(dt);

            using (testEntities context = new testEntities())
            {
                string eSql = "SELECT VALUE SUM(o.Freight) FROM Orders AS o GROUP BY o.Store.Id";
                ObjectQuery<Double> q = context.CreateQuery<Double>(eSql);

                string sql = q.ToTraceString();
                CheckSql(sql, SQLSyntax.SumWithGrouping);

                int i = 0;
                foreach (double freight in q)
                    Assert.AreEqual(Convert.ToInt32(dt.Rows[i++][0]), Convert.ToInt32(freight));
            }
        }

        [Test]
        public void MaxInSubQuery1()
        {
            MySqlDataAdapter da= new MySqlDataAdapter(
                "SELECT s.* FROM Stores AS s WHERE s.id=(SELECT MAX(o.storeId) FROM Orders AS o)", conn);
            DataTable dt = new DataTable();
            da.Fill(dt);

            using (testEntities context = new testEntities())
            {
                string eSql = @"SELECT VALUE s FROM Stores AS s WHERE s.Id = 
                                MAX(SELECT VALUE o.Store.Id FROM Orders As o)";
                ObjectQuery<Store> q = context.CreateQuery<Store>(eSql);

                string sql = q.ToTraceString();
                CheckSql(sql, SQLSyntax.MaxInSubQuery1);

                int i = 0;
                foreach (Store s in q)
                    Assert.AreEqual(dt.Rows[i++]["id"], s.Id);
            }
        }

        [Test]
        public void MaxInSubQuery2()
        {
            MySqlDataAdapter da = new MySqlDataAdapter(
                "SELECT s.* FROM Stores AS s WHERE s.id=(SELECT MAX(o.storeId) FROM Orders AS o)", conn);
            DataTable dt = new DataTable();
            da.Fill(dt);

            using (testEntities context = new testEntities())
            {
                string eSql = @"SELECT VALUE s FROM Stores AS s WHERE s.Id = 
                                ANYELEMENT(SELECT VALUE MAX(o.Store.Id) FROM Orders As o)";
                ObjectQuery<Store> q = context.CreateQuery<Store>(eSql);

                string sql = q.ToTraceString();
                CheckSql(sql, SQLSyntax.MaxInSubQuery2);

                int i = 0;
                foreach (Store s in q)
                    Assert.AreEqual(dt.Rows[i++]["id"], s.Id);
            }
        }
    }
}