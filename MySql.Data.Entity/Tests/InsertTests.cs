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
    public class InsertTests : BaseEdmTest
    {
        public InsertTests()
            : base()
        {
            csAdditions += ";logging=true;";
        }

        [Test]
        public void InsertSingleRow()
        {
            MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM companies", conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            DataRow lastRow = dt.Rows[dt.Rows.Count - 1];
            int lastId = (int)lastRow["id"];
            DateTime dateBegan = DateTime.Now;

            using (testEntities context = new testEntities())
            {
                Company c = new Company();
                //c.Id = lastId + 1;
                c.Name = "Yoyo";
                c.NumEmployees = 486;
                c.DateBegan = dateBegan;
                c.Address.Address = "212 My Street.";
                c.Address.City = "Helena";
                c.Address.State = "MT";
                c.Address.ZipCode = "44558";

                context.AddToCompanies(c);
                try
                {
                    int result = context.SaveChanges();
                }
                catch (OptimisticConcurrencyException ex)
                {
                    context.Refresh(RefreshMode.StoreWins, c);
                    context.SaveChanges();
                }


                DataTable afterInsert = new DataTable();
                da.Fill(afterInsert);
                lastRow = afterInsert.Rows[afterInsert.Rows.Count - 1];

                Assert.AreEqual(dt.Rows.Count + 1, afterInsert.Rows.Count);
                Assert.AreEqual(lastId+1, lastRow["id"]);
                Assert.AreEqual("Yoyo", lastRow["name"]);
                Assert.AreEqual(486, lastRow["numemployees"]);
                DateTime insertedDT = (DateTime)lastRow["dateBegan"];
                Assert.AreEqual(dateBegan.Date, insertedDT.Date);
                Assert.AreEqual("212 My Street.", lastRow["address"]);
                Assert.AreEqual("Helena", lastRow["city"]);
                Assert.AreEqual("MT", lastRow["state"]);
                Assert.AreEqual("44558", lastRow["zipcode"]);
            }
        }
    }
}