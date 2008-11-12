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
using MySql.Web.Security.Tests;
using System.Data.EntityClient;
using System.Data.Common;
using NUnit.Framework;

namespace MySql.Data.MySqlClient.Edm.Tests
{
	/// <summary>
	/// Summary description for BlobTests.
	/// </summary>
	[TestFixture]
	public class AggregateTests : BaseEdmTest
	{
        public AggregateTests()
            : base()
        {
            //csAdditions += ";logging=true;";
        }

        private EntityConnection GetEntityConnection()
        {
            string connectionString = String.Format(
                "metadata=TestDB.csdl|TestDB.msl|TestDB.ssdl;provider=MySql.Data.MySqlClient; provider connection string=\"{0}\"", GetConnectionString(true));
            EntityConnection connection = new EntityConnection(connectionString);
            return connection;
        }

        [Test]
        public void Count()
        {
            MySqlCommand trueCmd = new MySqlCommand("SELECT COUNT(*) FROM Employees", conn);
            object trueCount = trueCmd.ExecuteScalar();

            using (EntityConnection connection = GetEntityConnection())
            {
                connection.Open();

                using (EntityCommand cmd = new EntityCommand(
                    "SELECT VALUE Count(e.EmployeeId) FROM TestDB.Employees AS e", connection))
                {
                    object count = cmd.ExecuteScalar();
                    Assert.AreEqual(trueCount, count);
                }
            }
        }

        [Test]
        public void CountWithPredicate()
        {
            MySqlCommand trueCmd = new MySqlCommand("SELECT COUNT(*) FROM Employees WHERE age > 20", conn);
            object trueCount = trueCmd.ExecuteScalar();

            using (EntityConnection connection = GetEntityConnection())
            {
                connection.Open();

                using (EntityCommand cmd = new EntityCommand(
                    "SELECT VALUE Count(e.EmployeeId) FROM TestDB.Employees AS e WHERE e.Age > 20", connection))
                {
                    object count = cmd.ExecuteScalar();
                    Assert.AreEqual(trueCount, count);
                }
            }
        }

        [Test]
        public void MinSimple()
        {
            MySqlCommand trueCmd = new MySqlCommand("SELECT MIN(employeeId) FROM Employees", conn);
            int trueMin = (int)trueCmd.ExecuteScalar();

            using (EntityConnection connection = GetEntityConnection())
            {
                connection.Open();

                using (EntityCommand cmd = new EntityCommand(
                    "SELECT MIN(e.EmployeeId) FROM TestDB.Employees AS e", connection))
                {
                    object minId = cmd.ExecuteScalar();
                    Assert.AreEqual(trueMin, minId);
                }
            }
        }

        [Test]
        public void MaxSimple()
        {
            MySqlCommand trueCmd = new MySqlCommand("SELECT MAX(employeeId) FROM Employees", conn);
            int trueMax = (int)trueCmd.ExecuteScalar();

            using (EntityConnection connection = GetEntityConnection())
            {
                connection.Open();

                using (EntityCommand cmd = new EntityCommand(
                    "SELECT MAX(e.EmployeeId) FROM TestDB.Employees AS e", connection))
                {
                    object maxId = cmd.ExecuteScalar();
                    Assert.AreEqual(trueMax, maxId);
                }
            }
        }

        [Test]
        public void AvgSimple()
        {
            MySqlCommand trueCmd = new MySqlCommand("SELECT AVG(age) FROM Employees", conn);
            object avgAge = trueCmd.ExecuteScalar();

            using (EntityConnection connection = GetEntityConnection())
            {
                connection.Open();

                using (EntityCommand cmd = new EntityCommand(
                    "SELECT VALUE Avg(e.Age) FROM TestDB.Employees AS e", connection))
                {
                    object avg = cmd.ExecuteScalar();
                    Assert.AreEqual(avgAge, avg);
                }
            }
        }

        [Test]
        public void SumSimple()
        {
            MySqlCommand trueCmd = new MySqlCommand("SELECT SUM(age) FROM Employees", conn);
            object sumAge = trueCmd.ExecuteScalar();

            using (EntityConnection connection = GetEntityConnection())
            {
                connection.Open();

                using (EntityCommand cmd = new EntityCommand(
                    "SELECT VALUE Sum(e.Age) FROM TestDB.Employees AS e", connection))
                {
                    object sum = cmd.ExecuteScalar();
                    Assert.AreEqual(sumAge, sum);
                }
            }
        }
    }
}