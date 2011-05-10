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
using System.Threading;
using MySql.Data.MySqlClient;
using MySql.Data.MySqlClient.Tests;
using System.Data.EntityClient;
using System.Data.Common;
using NUnit.Framework;
using System.Data.Objects;
using System.Data.Entity.Design;
using System.Linq;
using Store;
using System.Configuration;


namespace MySql.Data.Entity.Tests
{
    // This test unit covers the tests that the wizard runs when generating a model
    // from an existing database
    [TestFixture]
	public class WizardTests : BaseEdmTest
	{
        private EntityConnection GetConnection()
        {
            return EntityStoreSchemaGenerator.CreateStoreSchemaConnection(
                "MySql.Data.MySqlClient", @"server=localhost;uid=root;database=test;pooling=false");
        }

        [Test]
        public void SelectAllTables()
        {
            execSQL("CREATE TABLE test (id int)");

            System.Data.DataTable dt = conn.GetSchema("Tables");

            using (EntityConnection ec = GetConnection())
            {
                using (SchemaInformation si = new SchemaInformation(ec))
                {
                    int i = 0;
                    var q = si.Tables.Select("it.CatalogName, it.SchemaName, it.Name").OrderBy("it.Name, it.SchemaName");
                    foreach (DbDataRecord t in q)
                        Assert.AreEqual(dt.Rows[i++]["TABLE_NAME"], t.GetString(2));
                }
            }
        }

        [Test]
        public void SelectAllViews()
        {
            execSQL("CREATE TABLE test (id int)");
            execSQL("CREATE VIEW view1 as SELECT * FROM test");

            System.Data.DataTable dt = conn.GetSchema("Views");

            using (EntityConnection ec = GetConnection())
            {
                using (SchemaInformation si = new SchemaInformation(ec))
                {
                    int i = 0;
                    var q = si.Views.Select("it.CatalogName, it.SchemaName, it.Name").OrderBy("it.Name, it.SchemaName");
                    foreach (DbDataRecord t in q)
                        Assert.AreEqual(dt.Rows[i++]["TABLE_NAME"], t.GetString(2));
                }
            }
        }

        [Test]
        public void GetDbProviderManifestTokenReturnsCorrectSchemaVersion()
        {
            if (Version < new Version(5, 0)) return;

            MySqlProviderServices services = new MySqlProviderServices();
            string token = services.GetProviderManifestToken(conn);

            if (Version < new Version(5, 1))
                Assert.AreEqual("5.0", token);
            else if (Version < new Version(5, 5))
                Assert.AreEqual("5.1", token);
            else 
                Assert.AreEqual("5.5", token);
        }
    }

    public static class ExtensionMethods
    {
        public static string ToTraceString<T>(this IQueryable<T> t)
        {
            // try to cast to ObjectQuery<T>
            ObjectQuery<T> oqt = t as ObjectQuery<T>;
            if (oqt != null)
                return oqt.ToTraceString();
            return "";
        }
    }
}