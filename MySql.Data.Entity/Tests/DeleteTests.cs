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
using NUnit.Framework;
using MySql.Data.MySqlClient.Tests;
using System.Data.EntityClient;
using System.Data.Common;
using System.Data.Objects;

namespace MySql.Data.Entity.Tests
{
	[TestFixture]
	public class DeleteTests : BaseEdmTest
    {
        [Test]
        public void SimpleDeleteAllRows()
        {
            using (testEntities context = new testEntities())
            {
                foreach (Toys t in context.Toys)
                    context.DeleteObject(t);
                context.SaveChanges();

                EntityConnection ec = context.Connection as EntityConnection;
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM toys",
                    (MySqlConnection)ec.StoreConnection);
                DataTable dt = new DataTable();
                da.Fill(dt);
                Assert.AreEqual(0, dt.Rows.Count);
            }
        }

        [Test]
        public void SimpleDeleteRowByParameter()
        {
            using (testEntities context = new testEntities())
            {
                EntityConnection ec = context.Connection as EntityConnection;
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM toys WHERE minage=3", (MySqlConnection)ec.StoreConnection);
                DataTable dt = new DataTable();
                da.Fill(dt);
                Assert.IsTrue(dt.Rows.Count > 0);

                ObjectQuery<Toys> toys = context.Toys.Where("it.MinAge = @age", new ObjectParameter("age", 3));
                foreach (Toys t in toys)
                    context.DeleteObject(t);
                context.SaveChanges();

                dt.Clear();
                da.Fill(dt);
                Assert.AreEqual(0, dt.Rows.Count);
            }
        }

       [Test]
       public void DeleteAllRowsParameter()
       {
           using (testEntities context = new testEntities())
           {
               foreach (Employees e in context.Employees)
                   context.DeleteObject(e);
               context.SaveChanges();

               EntityConnection ec = context.Connection as EntityConnection;
               MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM employees",
                   (MySqlConnection)ec.StoreConnection);
               DataTable dt = new DataTable();
               da.Fill(dt);
               Assert.AreEqual(0, dt.Rows.Count);
           }
       }

       [Test]
       public void DeleteRowByParameter()
       {
           using (testEntities context = new testEntities())
           {
               ObjectQuery<Employees> emp = context.Employees.Where("it.ID = @id", new ObjectParameter("id", 1));
               foreach (Employees e in emp)
                   context.DeleteObject(e);
               context.SaveChanges();

               EntityConnection ec = context.Connection as EntityConnection;
               MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM employees", (MySqlConnection)ec.StoreConnection);
               DataTable dt = new DataTable();
               da.Fill(dt);
               Assert.AreEqual(1, dt.Rows.Count);
               DataRow row = dt.Rows[0];
               Assert.AreEqual("Rubble", row["LastName"]);
           }
       }
    }
}