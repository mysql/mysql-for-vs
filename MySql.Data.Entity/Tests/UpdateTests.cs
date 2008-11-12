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
using MySql.Web.Security.Tests;
using TestDB;

namespace MySql.Data.Entity.Tests
{
	[TestFixture]
	public class UpdateTests : BaseEdmTest
	{
       [Test]
       public void UpdateAllRows()
       {
           using (TestDB.TestDB db = new TestDB.TestDB())
           {
               foreach (Employee e in db.Employees)
                   e.LastName = "Doe";
               db.SaveChanges();

               EntityConnection ec = db.Connection as EntityConnection;
               MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM employees", (MySqlConnection)ec.StoreConnection);
               DataTable dt = new DataTable();
               da.Fill(dt);
               Assert.AreEqual(4, dt.Rows.Count);
               foreach (DataRow row in dt.Rows)
                   Assert.AreEqual("Doe", row["LastName"]);
           }
       }
    }
}