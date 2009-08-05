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
using NUnit.Framework;
using MySql.Data.MySqlClient.Tests;
using System.Data.EntityClient;
using System.Data.Common;
using System.Data.Objects;

namespace MySql.Data.Entity.Tests
{
	[TestFixture]
	public class DataTypeTests : BaseEdmTest
    {
        /// <summary>
        /// Bug #45457 DbType Time is not supported in entity framework
        /// </summary>
        [Test]
        public void TimeType()
        {
            using (testEntities context = new testEntities())
            {
                TimeSpan birth = new TimeSpan(11,3,2);

                Child c = new Child();
                c.Id = 1;
                c.EmployeeID = 1;
                c.FirstName = "first";
                c.LastName = "last";
                c.BirthTime = birth;
                context.AddToChildren(c);
                context.SaveChanges();

                MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM EmployeeChildren", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                Assert.AreEqual(birth, dt.Rows[0]["birthtime"]);
            }
        }
    }
}