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
	/// <summary>
	/// Summary description for BlobTests.
	/// </summary>
    [TestFixture]
    public class RestrictionOperators : BaseEdmTest
    {
        public RestrictionOperators()
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
        public void SimpleSelect()
        {
            using (EntityConnection connection = GetEntityConnection())
            {
                connection.Open();

                using (EntityCommand cmd = new EntityCommand(
                    "SELECT C.Name FROM TestDB.Toys AS C", connection))
                {
                    using (DbDataReader reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                    {
                        Assert.IsTrue(reader.HasRows);
                        reader.Read();
                        Assert.AreEqual("Slinky", reader.GetString(0));
                        reader.Read();
                        Assert.AreEqual("Rubiks Cube", reader.GetString(0));
                        reader.Read();
                        Assert.AreEqual("Lincoln Logs", reader.GetString(0));
                        reader.Read();
                        Assert.AreEqual("Legos", reader.GetString(0));
                        Assert.IsFalse(reader.Read());
                    }
                }
            }
        }

        [Test]
        public void SimpleSelectWithFilter()
        {
            using (EntityConnection connection = GetEntityConnection())
            {
                connection.Open();

                using (EntityCommand cmd = new EntityCommand(
                    "SELECT T.Name FROM TestDB.Toys AS T WHERE T.MinAge = 4", connection))
                {
                    using (DbDataReader reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                    {
                        Assert.IsTrue(reader.HasRows);
                        reader.Read();
                        Assert.AreEqual("Legos", reader.GetString(0));
                        Assert.IsFalse(reader.Read());
                    }
                }
            }
        }

        [Test]
        public void SimpleSelectWithParam()
        {
            using (EntityConnection connection = GetEntityConnection())
            {
                connection.Open();

                using (EntityCommand cmd = new EntityCommand(
                    "SELECT T.Name FROM TestDB.Toys AS T WHERE T.MinAge = @age", connection))
                {
                    cmd.Parameters.Add("age", DbType.Int32);
                    cmd.Parameters[0].Value = 3;
                    using (DbDataReader reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                    {
                        Assert.IsTrue(reader.HasRows);
                        reader.Read();
                        Assert.AreEqual("Lincoln Logs", reader.GetString(0));
                        Assert.IsFalse(reader.Read());
                    }
                }
            }
        }

        [Test]
        public void SelectWithComplexType()
        {
            using (EntityConnection connection = GetEntityConnection())
            {
                connection.Open();

                using (EntityCommand cmd = new EntityCommand(
                    "SELECT C.LastName FROM TestDB.Employees AS C WHERE C.Age>20", connection))
                {
                    using (DbDataReader reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                    {
                        Assert.IsTrue(reader.HasRows);
                        reader.Read();
                        Assert.AreEqual("Fintstone", reader.GetString(0));
                        reader.Read();
                        Assert.AreEqual("Rubiks Cube", reader.GetString(0));
                        reader.Read();
                        Assert.AreEqual("Lincoln Logs", reader.GetString(0));
                        reader.Read();
                        Assert.AreEqual("Legos", reader.GetString(0));
                        Assert.IsFalse(reader.Read());
                    }
                }
            }
        }

        [Test]
        public void WhereLiteralOnRelation()
        {
            using (testEntities context = new testEntities())
            {
                using (EntityConnection ec = context.Connection as EntityConnection)
                {
                    ec.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT id FROM Companies c WHERE c.City = 'Dallas'", 
                        (MySqlConnection)ec.StoreConnection);
                    object id = cmd.ExecuteScalar();

                    string sql = "SELECT VALUE c FROM Companies AS c WHERE c.Address.City = 'Dallas'";
                    ObjectQuery<Companies> query = context.CreateQuery<Companies>(sql);

                    // should just be one
                    foreach (Companies c in query)
                        Assert.AreEqual(id, c.Id);
                }
            }
        }

        [Test]
        public void Exists()
        {
            using (testEntities context = new testEntities())
            {
                using (EntityConnection ec = context.Connection as EntityConnection)
                {
                    ec.Open();

                    using (EntityCommand cmd = new EntityCommand(
                        @"SELECT VALUE c FROM TestDB.Companies AS c WHERE EXISTS(
                    SELECT p FROM c.Toys AS p WHERE p.MinAge < 4)", ec))
                    {
                        using (DbDataReader reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                        {
                            Assert.IsTrue(reader.HasRows);
                            reader.Read();
                            Assert.AreEqual(1, reader.GetInt32(0));
                            reader.Read();
                            Assert.AreEqual(3, reader.GetInt32(0));
                            Assert.IsFalse(reader.Read());
                        }
                    }
                }
            }
        }
    }
}