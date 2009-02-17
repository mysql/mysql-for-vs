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
using MySql.Data.MySqlClient.Tests;
using System.Data.EntityClient;
using System.Data.Common;
using NUnit.Framework;
using System.Data.Objects;
using Store;
using System.Data.Entity.Design;
using System.Linq;

namespace MySql.Data.Entity.Tests
{
	[TestFixture]
	public class WizardTests : BaseEdmTest
	{
        // This test unit covers the tests that the wizard runs when generating a model
        // from an existing database
        public WizardTests()
            : base()
        {
        }

        private EntityConnection GetConnection()
        {
            return EntityStoreSchemaGenerator.CreateStoreSchemaConnection(
                "MySql.Data.MySqlClient", @"server=localhost;uid=root;database=test3");
        }

        [Test]
        public void SelectAllTables()
        {
            execSQL("CREATE TABLE test (id int)");

            DataTable dt = conn.GetSchema("Tables");

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

            DataTable dt = conn.GetSchema("Views");

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
        public void FunctionsAndProcedures()
        {
            execSQL("CREATE PROCEDURE spTest (id int) BEGIN END");
            execSQL("CREATE FUNCTION spTestFunc (id int) RETURNS INT BEGIN RETURN 0; END");

            DataTable dt = conn.GetSchema("Procedures");

            using (EntityConnection ec = GetConnection())
            {
                using (SchemaInformation si = new SchemaInformation(ec))
                {
                    int i = 0;
                    var funcs = si.Functions.Select("it.CatalogName, it.SchemaName, it.Name");
                    var procs = si.Procedures.Select("it.CatalogName, it.SchemaName, it.Name");
                    var q = funcs.UnionAll(procs).Select("it.CatalogName, it.SchemaName, it.Name").OrderBy("it.Name, it.SchemaName");
                    // the test won't work until we schema mapping actually returns procs
//                    foreach (DbDataRecord t in q)
  //                      Assert.AreEqual(dt.Rows[i++]["ROUTINE_NAME"], t.GetString(2));
                }
            }
        }

        [Test]
        public void TableAndViewColumns()
        {
            using (EntityConnection ec = GetConnection())
            {
                using (SchemaInformation si = new SchemaInformation(ec))
                {
//                    string selectString = @"it.Id, it.Name, it.Ordinal, it.IsNullable, 
//                        it.ColumnType.TypeName, it.ColumnType.MaxLength, it.ColumnType.Precision,
//                        it.ColumnType.DateTimePrecision, it.ColumnType.Scale, it.IsIdentity, 
//                        it.IsStoreGenerated";
//                    var tc = si.TableColumns.Select(selectString);
//                    var vc = si.ViewColumns.Select(selectString);
//                    var TcAndVc = tc.UnionAll(vc);
//                    var q = si.Tables.Join(TcAndVc,
  //                                         table => table.Id,
    //                                       inner => inner.GetString(1),
      //                                     (table, inner) => new { Id = 0 }).Select(
                    var q3 = from table in si.Tables
                             select table;
                    //var q2 = from con in si.Tables
                    //         where con.Constraints.
                    //         select con.Constraints;
                    string s = q3.ToTraceString();


                    var tAndv = from tableCol in si.TableColumns
                                select new
                                {
                                    Id = tableCol.Id,
                                    Name = tableCol.Name,
                                    ParentId = tableCol.Parent.Id
                                };
                    string sql2 = tAndv.ToTraceString();

                    var vCols = from viewCol in si.ViewColumns
                                select new
                                {
                                    Id = viewCol.Id,
                                    Name = viewCol.Name,
                                    ParentId = viewCol.Parent.Id
                                };
                    var cols = tAndv.Union(vCols);

                    var q = from table in si.Tables
                            join col in cols on table.Id equals col.ParentId
                            select new
                            {
                                Id = col.Id,
                                Name = col.Name
                            };

                    string sql = q.ToTraceString();

                    //string sql = q.Select(p => new { Id = p.Id }).ToTr

                }
            }
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