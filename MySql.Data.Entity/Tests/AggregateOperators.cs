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

namespace MySql.Data.Entity.Tests
{
	[TestFixture]
	public class AggregateOperators : BaseEdmTest
	{
        public AggregateOperators()
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
        public void CountSimple()
        {
            MySqlCommand trueCmd = new MySqlCommand("SELECT COUNT(*) FROM Toys", conn);
            object trueCount = trueCmd.ExecuteScalar();

            using (testEntities context = new testEntities())
            {
                string sql = "SELECT VALUE Count(t.Id) FROM Toys AS t";
                ObjectQuery<Int32> q = context.CreateQuery<Int32>(sql);

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
                string sql = "SELECT VALUE BigCount(t.Id) FROM Toys AS t";
                ObjectQuery<Int32> q = context.CreateQuery<Int32>(sql);

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
                string sql = "SELECT VALUE Count(t.Id) FROM Toys AS t WHERE t.MinAge > 3";
                ObjectQuery<Int32> q = context.CreateQuery<Int32>(sql);

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
                string sql = "SELECT VALUE MIN(t.MinAge) FROM Toys AS t";
                ObjectQuery<Int32> q = context.CreateQuery<Int32>(sql);

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
                string sql = "SELECT Min(o.Freight) FROM Orders AS o WHERE o.Store.Id = 2";
                ObjectQuery<DbDataRecord> q = context.CreateQuery<DbDataRecord>(sql);

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
                string sql = "SELECT VALUE Min(o.Freight) FROM Orders AS o GROUP BY o.Store.Id";
                ObjectQuery<Double> q = context.CreateQuery<Double>(sql);

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
                string sql = "SELECT VALUE MAX(t.MinAge) FROM Toys AS t";
                ObjectQuery<Int32> q = context.CreateQuery<Int32>(sql);

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
                string sql = "SELECT MAX(o.Freight) FROM Orders AS o WHERE o.Store.Id = 1";
                ObjectQuery<DbDataRecord> q = context.CreateQuery<DbDataRecord>(sql);

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
                string sql = "SELECT VALUE MAX(o.Freight) FROM Orders AS o GROUP BY o.Store.Id";
                ObjectQuery<Double> q = context.CreateQuery<Double>(sql);

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
                string sql = "SELECT VALUE Avg(t.MinAge) FROM Toys AS t";
                ObjectQuery<Decimal> q = context.CreateQuery<Decimal>(sql);

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
                string sql = "SELECT VALUE AVG(o.Freight) FROM Orders AS o WHERE o.Store.Id = 3";
                ObjectQuery<Double> q = context.CreateQuery<Double>(sql);

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
                string sql = "SELECT VALUE AVG(o.Freight) FROM Orders AS o GROUP BY o.Store.Id";
                ObjectQuery<Double> q = context.CreateQuery<Double>(sql);

                int i = 0;
                foreach (double freight in q)
                    Assert.AreEqual(Convert.ToInt32(dt.Rows[i++][0]), Convert.ToInt32(freight));
            }
        }

        [Test]
        public void SumSimple()
        {
            MySqlCommand trueCmd = new MySqlCommand("SELECT SUM(minage) FROM Toys", conn);
            object sumAge = trueCmd.ExecuteScalar();

            using (testEntities context = new testEntities())
            {
                string sql = "SELECT VALUE Sum(t.MinAge) FROM Toys AS t";
                ObjectQuery<Int32> q = context.CreateQuery<Int32>(sql);

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
                string sql = "SELECT VALUE SUM(o.Freight) FROM Orders AS o WHERE o.Store.Id = 2";
                ObjectQuery<Double> q = context.CreateQuery<Double>(sql);

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
                string sql = "SELECT VALUE SUM(o.Freight) FROM Orders AS o GROUP BY o.Store.Id";
                ObjectQuery<Double> q = context.CreateQuery<Double>(sql);

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
                string sql = @"SELECT VALUE s FROM Stores AS s WHERE s.Id = 
                                MAX(SELECT VALUE o.Store.Id FROM Orders As o)";
                ObjectQuery<Store> q = context.CreateQuery<Store>(sql);

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
                string sql = @"SELECT VALUE s FROM Stores AS s WHERE s.Id = 
                                ANYELEMENT(SELECT VALUE MAX(o.Store.Id) FROM Orders As o)";
                ObjectQuery<Store> q = context.CreateQuery<Store>(sql);

                int i = 0;
                foreach (Store s in q)
                    Assert.AreEqual(dt.Rows[i++]["id"], s.Id);
            }
        }
    }
}