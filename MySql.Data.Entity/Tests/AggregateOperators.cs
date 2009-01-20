// Copyright (C) 2004-2007 MySQL AB
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
                string sql = "SELECT VALUE Count(t.Id) FROM TestDB.Toys AS t";
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

            using (EntityConnection connection = GetEntityConnection())
            {
                connection.Open();

                using (EntityCommand cmd = new EntityCommand(
                    "SELECT VALUE Count(t.Id) FROM TestDB.Toys AS t WHERE t.MinAge > 3", connection))
                {
                    object count = cmd.ExecuteScalar();
                    Assert.AreEqual(trueCount, count);
                }
            }
        }

        [Test]
        public void MinSimple()
        {
            MySqlCommand trueCmd = new MySqlCommand("SELECT MIN(minage) FROM Toys", conn);
            int trueMin = (int)trueCmd.ExecuteScalar();

            using (EntityConnection connection = GetEntityConnection())
            {
                connection.Open();

                using (EntityCommand cmd = new EntityCommand(
                    "SELECT MIN(t.MinAge) FROM TestDB.Toys AS t", connection))
                {
                    object minId = cmd.ExecuteScalar();
                    Assert.AreEqual(trueMin, minId);
                }
            }
        }

        [Test]
        public void MinWithPredicate()
        {
            MySqlCommand trueCmd = new MySqlCommand("SELECT MIN(Freight) FROM Orders WHERE storeId=2", conn);
            object freight = trueCmd.ExecuteScalar();

            using (testEntities context = new testEntities())
            {
                string sql = "SELECT VALUE Min(o.Freight) FROM Orders AS o WHERE o.StoreId = 2";
                ObjectQuery<Double> q = context.CreateQuery<Double>(sql);

                Assert.AreEqual(freight, q);
            }
        }

        [Test]
        public void MinWithGrouping()
        {
            MySqlCommand trueCmd = new MySqlCommand("SELECT MIN(Freight) FROM Orders WHERE storeId=2", conn);

            using (testEntities context = new testEntities())
            {
                string sql = "SELECT Min(o.Freight) FROM Orders AS o GROUP BY o.StoreId";
                ObjectQuery<Double> q = context.CreateQuery<Double>(sql);

                foreach (double freight in q)
                    Assert.AreEqual(2, freight);
            }
        }

        [Test]
        public void MaxSimple()
        {
            MySqlCommand trueCmd = new MySqlCommand("SELECT MAX(minage) FROM Toys", conn);
            int trueMax = (int)trueCmd.ExecuteScalar();

            using (EntityConnection connection = GetEntityConnection())
            {
                connection.Open();

                using (EntityCommand cmd = new EntityCommand(
                    "SELECT MAX(t.MinAge) FROM TestDB.Toys AS t", connection))
                {
                    object maxId = cmd.ExecuteScalar();
                    Assert.AreEqual(trueMax, maxId);
                }
            }
        }

        [Test]
        public void MaxWithPredicate()
        {
            MySqlCommand trueCmd = new MySqlCommand("SELECT MAX(Freight) FROM Orders WHERE storeId=1", conn);
            object freight = trueCmd.ExecuteScalar();

            using (testEntities context = new testEntities())
            {
                string sql = "SELECT VALUE MAX(o.Freight) FROM Orders AS o WHERE o.StoreId = 1";
                ObjectQuery<Double> q = context.CreateQuery<Double>(sql);

                Assert.AreEqual(freight, q);
            }
        }

        [Test]
        public void MaxWithGrouping()
        {
            MySqlCommand trueCmd = new MySqlCommand("SELECT MAX(Freight) FROM Orders GROUP BY StoreId", conn);

            using (testEntities context = new testEntities())
            {
                string sql = "SELECT MAX(o.Freight) FROM Orders AS o GROUP BY o.StoreId";
                ObjectQuery<Double> q = context.CreateQuery<Double>(sql);

                foreach (double freight in q)
                    Assert.AreEqual(2, freight);
            }
        }

        [Test]
        public void AverageSimple()
        {
            MySqlCommand trueCmd = new MySqlCommand("SELECT AVG(minAge) FROM Employees", conn);
            object avgAge = trueCmd.ExecuteScalar();

            using (EntityConnection connection = GetEntityConnection())
            {
                connection.Open();

                using (EntityCommand cmd = new EntityCommand(
                    "SELECT VALUE Avg(t.MinAge) FROM TestDB.Toys AS t", connection))
                {
                    object avg = cmd.ExecuteScalar();
                    Assert.AreEqual(avgAge, avg);
                }
            }
        }

        [Test]
        public void AverageWithPredicate()
        {
            MySqlCommand trueCmd = new MySqlCommand("SELECT AVG(Freight) FROM Orders WHERE storeId=3", conn);
            object freight = trueCmd.ExecuteScalar();

            using (testEntities context = new testEntities())
            {
                string sql = "SELECT VALUE AVG(o.Freight) FROM Orders AS o WHERE o.StoreId = 3";
                ObjectQuery<Double> q = context.CreateQuery<Double>(sql);

                Assert.AreEqual(freight, q);
            }
        }

        [Test]
        public void AverageWithGrouping()
        {
            MySqlCommand trueCmd = new MySqlCommand("SELECT AVG(Freight) FROM Orders GROUP BY StoreId", conn);

            using (testEntities context = new testEntities())
            {
                string sql = "SELECT AVG(o.Freight) FROM Orders AS o GROUP BY o.StoreId";
                ObjectQuery<Double> q = context.CreateQuery<Double>(sql);

                foreach (double freight in q)
                    Assert.AreEqual(2, freight);
            }
        }

        [Test]
        public void SumSimple()
        {
            MySqlCommand trueCmd = new MySqlCommand("SELECT SUM(minage) FROM Toys", conn);
            object sumAge = trueCmd.ExecuteScalar();

            using (EntityConnection connection = GetEntityConnection())
            {
                connection.Open();

                using (EntityCommand cmd = new EntityCommand(
                    "SELECT VALUE Sum(t.MinAge) FROM TestDB.Toys AS t", connection))
                {
                    object sum = cmd.ExecuteScalar();
                    Assert.AreEqual(sumAge, sum);
                }
            }
        }

        [Test]
        public void SumWithPredicate()
        {
            MySqlCommand trueCmd = new MySqlCommand("SELECT SUM(Freight) FROM Orders WHERE storeId=2", conn);
            object freight = trueCmd.ExecuteScalar();

            using (testEntities context = new testEntities())
            {
                string sql = "SELECT VALUE SUM(o.Freight) FROM Orders AS o WHERE o.StoreId = 2";
                ObjectQuery<Double> q = context.CreateQuery<Double>(sql);

                Assert.AreEqual(freight, q);
            }
        }

        [Test]
        public void SumWithGrouping()
        {
            MySqlCommand trueCmd = new MySqlCommand("SELECT SUM(Freight) FROM Orders GROUP BY StoreId", conn);

            using (testEntities context = new testEntities())
            {
                string sql = "SELECT SUM(o.Freight) FROM Orders AS o GROUP BY o.StoreId";
                ObjectQuery<Double> q = context.CreateQuery<Double>(sql);

                foreach (double freight in q)
                    Assert.AreEqual(2, freight);
            }
        }

        [Test]
        public void MaxInSubQuery1()
        {
            MySqlCommand trueCmd = new MySqlCommand(
                "SELECT s.* FROM Stores AS s WHERE s.id=SELECT MAX(o.storeId) FROM Orders AS o", conn);

            using (testEntities context = new testEntities())
            {
                string sql = @"SELECT VALUE s FROM Stores AS s WHERE s.Id = MAX(SELECT VALUE o.StoreId FROM Orders As o)";
                ObjectQuery<Stores> q = context.CreateQuery<Stores>(sql);

//                foreach (double freight in q)
  //                  Assert.AreEqual(2, freight);
            }
        }

        [Test]
        public void MaxInSubQuery2()
        {
            MySqlCommand trueCmd = new MySqlCommand(
                "SELECT s.* FROM Stores AS s WHERE s.id=SELECT MAX(o.storeId) FROM Orders AS o", conn);

            using (testEntities context = new testEntities())
            {
                string sql = @"SELECT VALUE s FROM Stores AS s WHERE s.Id = ANYELEMENT(SELECT VALUE MAX(o.StoreId) FROM Orders As o)";
                ObjectQuery<Stores> q = context.CreateQuery<Stores>(sql);

     //           foreach (double freight in q)
    // /               Assert.AreEqual(2, freight);
            }
        }
    }
}