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
using System.Linq;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using MySql.Data.MySqlClient.Tests;
using System.Data.EntityClient;
using System.Data.Common;
using System.Data.Objects;
using System.Globalization;

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
                c.Id = 20;
                c.EmployeeID = 1;
                c.FirstName = "first";
                c.LastName = "last";
                c.BirthTime = birth;
                context.AddToChildren(c);
                context.SaveChanges();

                MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM EmployeeChildren WHERE id=20", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                Assert.AreEqual(birth, dt.Rows[0]["birthtime"]);
            }
        }

        /// <summary>
        /// Bug #45077	Insert problem with Connector/NET 6.0.3 and entity framework
        /// Bug #45175	Wrong SqlType for unsigned smallint when generating Entity Framework Model
        /// </summary>
        [Test]
        public void UnsignedValues()
        {
            using (testEntities context = new testEntities())
            {
                var row = context.Children.First();
                context.Detach(row);
                context.Attach(row);
            }
        }

        /// <summary>
        /// Bug #44455	insert and update error with entity framework
        /// </summary>
        [Test]
        public void DoubleValuesNonEnglish()
        {
            CultureInfo curCulture = Thread.CurrentThread.CurrentCulture;
            CultureInfo curUICulture = Thread.CurrentThread.CurrentUICulture;
            CultureInfo newCulture = new CultureInfo("da-DK");
            Thread.CurrentThread.CurrentCulture = newCulture;
            Thread.CurrentThread.CurrentUICulture = newCulture;

            try
            {
                using (testEntities context = new testEntities())
                {
                    Child c = new Child();
                    c.Id = 20;
                    c.EmployeeID = 1;
                    c.FirstName = "Bam bam";
                    c.LastName = "Rubble";
                    c.BirthWeight = 8.65;
                    context.AddToChildren(c);
                    context.SaveChanges();
                }
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = curCulture;
                Thread.CurrentThread.CurrentUICulture = curUICulture;
            }
        }

        /// <summary>
        /// Bug #46311	TimeStamp table column Entity Framework issue.
        /// </summary>
        [Test]
        public void TimestampColumn()
        {
            DateTime now = DateTime.Now;

            using (testEntities context = new testEntities())
            {
                Child c = context.Children.First();
                DateTime dt = c.Modified.DateTime;
                c.Modified = now;
                context.SaveChanges();

                c = context.Children.First();
                dt = c.Modified.DateTime;
                Assert.AreEqual(now, dt);
            }
        }
    }
}