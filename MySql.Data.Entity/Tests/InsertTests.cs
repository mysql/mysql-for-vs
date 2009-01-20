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
using NUnit.Framework;
using MySql.Data.MySqlClient.Tests;
using System.Data.EntityClient;
using System.Data.Common;
using System.Data.Objects;

namespace MySql.Data.Entity.Tests
{
    [TestFixture]
    public class InsertTests : BaseEdmTest
    {
        [Test]
        public void InsertSingleRow()
        {
            using (testEntities context = new testEntities())
            {
                EntityConnection ec = context.Connection as EntityConnection;
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM toys", (MySqlConnection)ec.StoreConnection);
                DataTable dt = new DataTable();
                da.Fill(dt);

                Toys t = new Toys();
                t.Id = 5;
                t.Name = "Yoyo";
                t.MinAge = 5;
                context.AddToToys(t);
                int result = context.SaveChanges();

                DataTable afterInsert = new DataTable();
                da.Fill(afterInsert);
                Assert.AreEqual(dt.Rows.Count + 1, afterInsert.Rows.Count);
                Assert.AreEqual(5, dt.Rows[4]["id"]);
                Assert.AreEqual("Yoyo", dt.Rows[4]["name"]);
                Assert.AreEqual(5, dt.Rows[4]["minage"]);
            }
        }
    }
}