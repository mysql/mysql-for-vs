﻿// Copyright © 2013 Oracle and/or its affiliates. All rights reserved.
//
// MySQL Connector/NET is licensed under the terms of the GPLv2
// <http://www.gnu.org/licenses/old-licenses/gpl-2.0.html>, like most 
// MySQL Connectors. There are special exceptions to the terms and 
// conditions of the GPLv2 as it is applied to this software, see the 
// FLOSS License Exception
// <http://www.mysql.com/about/legal/licensing/foss-exception.html>.
//
// This program is free software; you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published 
// by the Free Software Foundation; version 2 of the License.
//
// This program is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
//
// You should have received a copy of the GNU General Public License along 
// with this program; if not, write to the Free Software Foundation, Inc., 
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using MySql.Data.Entity.CodeFirst.Tests.Properties;
using MySql.Data.MySqlClient;
using MySql.Data.MySqlClient.Tests;
using System.Xml.Linq;
using System.Collections.Generic;
using Xunit;


namespace MySql.Data.Entity.CodeFirst.Tests
{
  public class CodeFirstTests : IUseFixture<SetUpCodeFirstTests>, IDisposable
  {
    private SetUpCodeFirstTests st;

    public void SetFixture(SetUpCodeFirstTests data)
    {
      st = data;
    }

    public void Dispose()
    {
      ReInitDb();
    }

    private void ReInitDb()
    {
      st.suExecSQL(string.Format("drop database if exists `{0}`", st.conn.Database));
      st.suExecSQL(string.Format("create database `{0}`", st.conn.Database));
    }

    /// <summary>
    /// Tests for fix of http://bugs.mysql.com/bug.php?id=61230
    /// ("The provider did not return a ProviderManifestToken string.").
    /// </summary>
    [Fact]
    public void SimpleCodeFirstSelect()
    {
      MovieDBContext db = new MovieDBContext();
      db.Database.Initialize(true);
      var l = db.Movies.ToList();
      foreach (var i in l)
      {
      }
    }

    /// <summary>
    /// Tests for fix of http://bugs.mysql.com/bug.php?id=62150
    /// ("EF4.1, Code First, CreateDatabaseScript() generates an invalid MySQL script.").
    /// </summary>
    [Fact]
    public void AlterTableTest()
    {
      ReInitDb();
      MovieDBContext db = new MovieDBContext();      
      db.Database.Initialize(true);
      var l = db.MovieFormats.ToList();
      foreach (var i in l)
      {
      }
      MovieFormat m = new MovieFormat();
      m.Format = 8.0f;
      db.MovieFormats.Add(m);
      db.SaveChanges();
    }

    /// <summary>
    /// Fix for "Connector/Net Generates Incorrect SELECT Clause after UPDATE" (MySql bug #62134, Oracle bug #13491689).
    /// </summary>
    [Fact]
    public void ConcurrencyCheck()
    {
      using (MovieDBContext db = new MovieDBContext())
      {
        db.Database.Delete();
        db.Database.CreateIfNotExists();
        
        db.Database.ExecuteSqlCommand(
@"DROP TABLE IF EXISTS `MovieReleases`");

        db.Database.ExecuteSqlCommand(
@"CREATE TABLE IF NOT EXISTS `MovieReleases` (
  `Id` int(11) NOT NULL,
  `Name` varbinary(45) NOT NULL,
  `Timestamp` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=binary");
        MySqlTrace.Listeners.Clear();
        MySqlTrace.Switch.Level = SourceLevels.All;
        GenericListener listener = new GenericListener();
        MySqlTrace.Listeners.Add(listener);
        try
        {
          MovieRelease mr = db.MovieReleases.Create();
          mr.Id = 1;
          mr.Name = "Commercial";
          db.MovieReleases.Add(mr);
          db.SaveChanges();
          mr.Name = "Director's Cut";
          db.SaveChanges();
        }
        finally
        {
          db.Database.ExecuteSqlCommand(@"DROP TABLE IF EXISTS `MovieReleases`");
        }
        // Check sql        
        Regex rx = new Regex(@"Query Opened: (?<item>UPDATE .*)", RegexOptions.Compiled | RegexOptions.Singleline);
        foreach (string s in listener.Strings)
        {
          Match m = rx.Match(s);
          if (m.Success)
          {
            st.CheckSql(m.Groups["item"].Value, SQLSyntax.UpdateWithSelect);
            //Assert.Pass();
          }
        }
        //Assert.Fail();
      }
    }

    /// <summary>
    /// This tests fix for http://bugs.mysql.com/bug.php?id=64216.
    /// </summary>
    [Fact]
    public void CheckByteArray()
    {
      MovieDBContext db = new MovieDBContext();
      db.Database.Initialize(true);
      string dbCreationScript =
        ((IObjectContextAdapter)db).ObjectContext.CreateDatabaseScript();
      Regex rx = new Regex(@"`Data` (?<type>[^\),]*)", RegexOptions.Compiled | RegexOptions.Singleline);
      Match m = rx.Match(dbCreationScript);
      Assert.Equal("longblob", m.Groups["type"].Value);
    }

/// <summary>
    /// Validates a stored procedure call using Code First
    /// Bug #14008699
    [Fact]
    public void CallStoredProcedure()
    {
      using (MovieDBContext context = new MovieDBContext())
      {
        context.Database.Initialize(true);
        int count = context.Database.SqlQuery<int>("GetCount").First();

        Assert.Equal(5, count);
      }
    }

    /// <summary>
    /// Tests for fix of http://bugs.mysql.com/bug.php?id=63920
    /// Maxlength error when it's used code-first and inheritance (discriminator generated column)
    /// </summary>
    [Fact]
    public void Bug63920_Test1()
    {
      ReInitDb();
      using (VehicleDbContext context = new VehicleDbContext())
      {
        context.Database.Delete();
        context.Database.Initialize(true);
        
        context.Vehicles.Add(new Car { Id = 1, Name = "Mustang", Year = 2012, CarProperty = "Car" });
        context.Vehicles.Add(new Bike { Id = 101, Name = "Mountain", Year = 2011, BikeProperty = "Bike" });
        context.SaveChanges();

        var list = context.Vehicles.ToList();

        int records = -1;
        using (MySqlConnection conn = new MySqlConnection(context.Database.Connection.ConnectionString))
        {
          conn.Open();
          MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) FROM Vehicles", conn);
          records = Convert.ToInt32(cmd.ExecuteScalar());
        }

        Assert.Equal(context.Vehicles.Count(), records);
      }
    }

    /// <summary>
    /// Tests for fix of http://bugs.mysql.com/bug.php?id=63920
    /// Key reference generation script error when it's used code-first and a single table for the inherited models
    /// </summary>
    [Fact]
    public void Bug63920_Test2()
    {
      ReInitDb();
      using (VehicleDbContext2 context = new VehicleDbContext2())
      {
        context.Database.Delete();
        context.Database.Initialize(true);

        context.Vehicles.Add(new Car2 { Id = 1, Name = "Mustang", Year = 2012, CarProperty = "Car" });
        context.Vehicles.Add(new Bike2 { Id = 101, Name = "Mountain", Year = 2011, BikeProperty = "Bike" });
        context.SaveChanges();

        var list = context.Vehicles.ToList();

        int records = -1;
        using (MySqlConnection conn = new MySqlConnection(context.Database.Connection.ConnectionString))
        {
          conn.Open();
          MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) FROM Vehicle2", conn);
          records = Convert.ToInt32(cmd.ExecuteScalar());
        }

        Assert.Equal(context.Vehicles.Count(), records);
      }     
    }

    /// <summary>
    /// This test fix for precision customization for columns bug (http://bugs.mysql.com/bug.php?id=65001), 
    /// Trying to customize column precision in Code First does not work).
    /// </summary>
    [Fact]
    public void TestPrecisionNscale()
    {
      MovieDBContext db = new MovieDBContext();
      db.Database.Initialize(true);
      var l = db.Movies.ToList();
      IDataReader r = st.execReader( string.Format( 
@"select numeric_precision, numeric_scale from information_schema.columns 
where table_schema = '{0}' and table_name = 'movies' and column_name = 'Price'", st.conn.Database ));
      r.Read();
      Assert.Equal( 16, r.GetInt32( 0 ) );
      Assert.Equal( 2, r.GetInt32( 1 ) );
    }       

    /// <summary>
    /// Test String types to StoreType for String
    /// A string with FixedLength=true will become a char 
    /// Max Length left empty will be char(max)
    /// Max Length(100) will be char(100) 
    /// while FixedLength=false will result in nvarchar. 
    /// Max Length left empty will be nvarchar(max)
    /// Max Length(100) will be nvarchar(100)                
    /// </summary>
    [Fact]
    public void TestStringTypeToStoreType()
    {
      using (VehicleDbContext3 context = new VehicleDbContext3())
      {
        if (context.Database.Exists()) context.Database.Delete();
        context.Database.CreateIfNotExists();
        context.Accessories.Add(new Accessory { Name = "Accesory One", Description = "Accesories descriptions", LongDescription = "Some long description" });
        context.SaveChanges();

        using (MySqlConnection conn = new MySqlConnection(context.Database.Connection.ConnectionString))
        {
          conn.Open();
          MySqlCommand query = new MySqlCommand("Select Column_name, Is_Nullable, Data_Type from information_schema.Columns where table_schema ='" + conn.Database + "' and table_name = 'Accessories' and column_name ='Description'", conn);
          query.Connection = conn;
          MySqlDataReader reader = query.ExecuteReader();
          while (reader.Read())
          {
            Assert.Equal("Description", reader[0].ToString());
            Assert.Equal("NO", reader[1].ToString());
            Assert.Equal("mediumtext", reader[2].ToString());
          }
          reader.Close();

          query = new MySqlCommand("Select Column_name, Is_Nullable, Data_Type, character_maximum_length from information_schema.Columns where table_schema ='" + conn.Database + "' and table_name = 'Accessories' and column_name ='Name'", conn);
          reader = query.ExecuteReader();
          while (reader.Read())
          {
            Assert.Equal("Name", reader[0].ToString());
            Assert.Equal("NO", reader[1].ToString());
            Assert.Equal("varchar", reader[2].ToString());
            Assert.Equal("255", reader[3].ToString());
          }
          reader.Close();

          query = new MySqlCommand("Select Column_name, Is_Nullable, Data_Type, character_maximum_length from information_schema.Columns where table_schema ='" + conn.Database + "' and table_name = 'Accessories' and column_name ='LongDescription'", conn);
          reader = query.ExecuteReader();
          while (reader.Read())
          {
            Assert.Equal("LongDescription", reader[0].ToString());
            Assert.Equal("NO", reader[1].ToString());
            Assert.Equal("longtext", reader[2].ToString());
            Assert.Equal("4294967295", reader[3].ToString());
          }
        }
      }
    }

    /// <summary>
    /// Test fix for http://bugs.mysql.com/bug.php?id=66066 / http://clustra.no.oracle.com/orabugs/bug.php?id=14479715
    /// (Using EF, crash when generating insert with no values.).
    /// </summary>
    [Fact]
    public void AddingEmptyRow()
    {
      using (MovieDBContext ctx = new MovieDBContext())
      {
        ctx.Database.Initialize(true);
        ctx.EntitySingleColumns.Add(new EntitySingleColumn());
        ctx.SaveChanges();
      }

      using (MovieDBContext ctx2 = new MovieDBContext())
      {
        var q = from esc in ctx2.EntitySingleColumns where esc.Id == 1 select esc;
        Assert.Equal(1, q.Count());
      }
    }

/// <summary>
    /// Test for identity columns when type is Integer or Guid (auto-generate
    /// values)
    /// </summary>
    [Fact]
    public void IdentityTest()
    {
      using (VehicleDbContext context = new VehicleDbContext())
      {
        context.Database.ExecuteSqlCommand("SET GLOBAL sql_mode='STRICT_ALL_TABLES'");
        if (context.Database.Exists()) context.Database.Delete();
        context.Database.CreateIfNotExists();

        // Identity as Guid
        Manufacturer nissan = new Manufacturer
        {
          Name = "Nissan"
        };
        Manufacturer ford = new Manufacturer
        {
          Name = "Ford"
        };
        context.Manufacturers.Add(nissan);
        context.Manufacturers.Add(ford);

        // Identity as Integer
        Distributor dis1 = new Distributor
        {
          Name = "Distributor1"
        };
        Distributor dis2 = new Distributor
        {
          Name = "Distributor2"
        };
        context.Distributors.Add(dis1);
        context.Distributors.Add(dis2);

        context.SaveChanges();

        using (MySqlConnection conn = new MySqlConnection(context.Database.Connection.ConnectionString))
        {
          conn.Open();

          // Validates Guid
          MySqlCommand cmd = new MySqlCommand("SELECT * FROM Manufacturers", conn);
          MySqlDataReader dr = cmd.ExecuteReader();
          Assert.False(!dr.HasRows, "No records found");

          while (dr.Read())
          {
            string name = dr.GetString(1);
            switch (name)
            {
              case "Nissan":
                Assert.Equal(dr.GetGuid(0), nissan.ManufacturerId);
                Assert.Equal(dr.GetGuid(2), nissan.GroupIdentifier);
                break;
              case "Ford":
                Assert.Equal(dr.GetGuid(0), ford.ManufacturerId);
                Assert.Equal(dr.GetGuid(2), ford.GroupIdentifier);
                break;
              default:
                //Assert.Fail();
                break;
            }
          }
          dr.Close();

          // Validates Integer
          cmd = new MySqlCommand("SELECT * FROM Distributors", conn);
          dr = cmd.ExecuteReader();
          if (!dr.HasRows)
            //Assert.Fail("No records found");
          while (dr.Read())
          {
            string name = dr.GetString(1);
            switch (name)
            {
              case "Distributor1":
                Assert.Equal(dr.GetInt32(0), dis1.DistributorId);
                break;
              case "Distributor2":
                Assert.Equal(dr.GetInt32(0), dis2.DistributorId);
                break;
              default:
                //Assert.Fail();
                break;
            }
          }
          dr.Close();
        }
      }
    }

    /// <summary>
    /// This test the fix for bug 67377.
    /// </summary>
    [Fact]
    public void FirstOrDefaultNested()
    {
      using (MovieDBContext ctx = new MovieDBContext())
      {
        ctx.Database.Initialize(true);
        int DirectorId = 1;
        var q = ctx.Movies.Where(p => p.Director.ID == DirectorId).Select(p => 
          new
          {
            Id = p.ID,
            FirstMovieFormat = p.Formats.Count == 0 ? 0.0 : p.Formats.FirstOrDefault().Format
          });
        string sql = q.ToString();
        foreach (var r in q)
        {
        }
      }
    }
  
     /// <summary>
    /// SUPPORT FOR DATE TYPES WITH PRECISION
    /// </summary>
    [Fact]
    public void CanDefineDatesWithPrecisionFor56()
    {
      if (st.Version < new Version(5, 6)) return;

      using (var db = new ProductsDbContext())
      {
        db.Database.CreateIfNotExists();
        using (MySqlConnection conn = new MySqlConnection(db.Database.Connection.ConnectionString))
        {
          conn.Open();
          MySqlCommand query = new MySqlCommand("Select Column_name, Is_Nullable, Data_Type, DateTime_Precision from information_schema.Columns where table_schema ='" + conn.Database + "' and table_name = 'Products' and column_name ='DateTimeWithPrecision'", conn);
          query.Connection = conn;
          MySqlDataReader reader = query.ExecuteReader();
          while (reader.Read())
          {
            Assert.Equal("DateTimeWithPrecision", reader[0].ToString());
            Assert.Equal("NO", reader[1].ToString());
            Assert.Equal("datetime", reader[2].ToString());
            Assert.Equal("3", reader[3].ToString());
          }
          reader.Close();

          query = new MySqlCommand("Select Column_name, Is_Nullable, Data_Type, DateTime_Precision from information_schema.Columns where table_schema ='" + conn.Database + "' and table_name = 'Products' and column_name ='TimeStampWithPrecision'", conn);
          query.Connection = conn;
          reader = query.ExecuteReader();
          while (reader.Read())
          {
            Assert.Equal("TimeStampWithPrecision", reader[0].ToString());
            Assert.Equal("NO", reader[1].ToString());
            Assert.Equal("timestamp", reader[2].ToString());
            Assert.Equal("3", reader[3].ToString());
          }
          reader.Close();
        }
        db.Database.Delete();
      }
    }

    /// <summary>
    /// Orabug #15935094 SUPPORT FOR CURRENT_TIMESTAMP AS DEFAULT FOR DATETIME WITH EF
    /// </summary>
    [Fact]
    public void CanDefineDateTimeAndTimestampWithIdentity()
    {

      if (st.Version < new Version(5, 6)) return;

      using (var db = new ProductsDbContext())
      {
        db.Database.CreateIfNotExists();
        Product product = new Product
        {
          //Omitting Identity Columns
          DateTimeWithPrecision = DateTime.Now,
          TimeStampWithPrecision = DateTime.Now
        };

        db.Products.Add(product);
        db.SaveChanges();

        var updateProduct = db.Products.First();
        updateProduct.DateTimeWithPrecision = new DateTime(2012, 3, 18, 23, 9, 7, 6);
        db.SaveChanges();

        Assert.NotNull(db.Products.First().Timestamp);
        Assert.NotNull(db.Products.First().DateCreated);
        Assert.Equal(new DateTime(2012, 3, 18, 23, 9, 7, 6), db.Products.First().DateTimeWithPrecision);
        Assert.Equal(1, db.Products.Count());

        db.Database.Delete();
      }
    }

    /// <summary>
    /// Test fix for http://bugs.mysql.com/bug.php?id=67183
    /// (Malformed Query while eager loading with EF 4 due to multiple projections).
    /// </summary>
    [Fact]
    public void ShipTest()
    {
      using (var context = new ShipContext())
      {
        context.Database.Initialize(true);

        var harbor = new Harbor
        {
          Ships = new HashSet<Ship>
            {
                new Ship
                {
                    CrewMembers = new HashSet<CrewMember>
                    {
                        new CrewMember
                        {
                            Rank = new Rank { Description = "Rank A" },
                            Clearance = new Clearance { Description = "Clearance A" },
                            Description = "CrewMember A"
                        },
                        new CrewMember
                        {
                            Rank = new Rank { Description = "Rank B" },
                            Clearance = new Clearance { Description = "Clearance B" },
                            Description = "CrewMember B"
                        }
                    },
                    Description = "Ship AB"
                },
                new Ship
                {
                    CrewMembers = new HashSet<CrewMember>
                    {
                        new CrewMember
                        {
                            Rank = new Rank { Description = "Rank C" },
                            Clearance = new Clearance { Description = "Clearance C" },
                            Description = "CrewMember C"
                        },
                        new CrewMember
                        {
                            Rank = new Rank { Description = "Rank D" },
                            Clearance = new Clearance { Description = "Clearance D" },
                            Description = "CrewMember D"
                        }
                    },
                    Description = "Ship CD"
                }
            },
          Description = "Harbor ABCD"
        };

        context.Harbors.Add(harbor);
        context.SaveChanges();
      }

      using (var context = new ShipContext())
      {
        DbSet<Harbor> dbSet = context.Set<Harbor>();
        IQueryable<Harbor> query = dbSet;
        query = query.Include(entity => entity.Ships);
        query = query.Include(entity => entity.Ships.Select(s => s.CrewMembers));
        query = query.Include(entity => entity.Ships.Select(s => s.CrewMembers.Select(cm => cm.Rank)));
        query = query.Include(entity => entity.Ships.Select(s => s.CrewMembers.Select(cm => cm.Clearance)));

        string[] data = new string[] { 
          "1,Harbor ABCD,1,1,1,Ship AB,1,1,1,1,1,CrewMember A,1,Rank A,1,Clearance A",
          "1,Harbor ABCD,1,1,1,Ship AB,1,2,1,2,2,CrewMember B,2,Rank B,2,Clearance B",
          "1,Harbor ABCD,1,2,1,Ship CD,1,3,2,3,3,CrewMember C,3,Rank C,3,Clearance C",
          "1,Harbor ABCD,1,2,1,Ship CD,1,4,2,4,4,CrewMember D,4,Rank D,4,Clearance D"
        };
        Dictionary<string, string> outData = new Dictionary<string, string>();

        var sqlString = query.ToString();
        st.CheckSql(sqlString, SQLSyntax.ShipQueryMalformedDueMultipleProjecttionsCorrected);
        // see below for the generated SQL query

        var harbor = query.Single();
        
        foreach (var ship in harbor.Ships)
        {
          foreach (var crewMember in ship.CrewMembers)
          {
            outData.Add(string.Format( 
              "{0},{1},1,{2},{3},{4},1,{5},{6},{7},{8},{9},{10},{11},{12},{13}",
              harbor.HarborId, harbor.Description, ship.ShipId, harbor.HarborId,
              ship.Description, crewMember.CrewMemberId, crewMember.ShipId, crewMember.RankId,
              crewMember.ClearanceId, crewMember.Description, crewMember.Rank.RankId,
              crewMember.Rank.Description, crewMember.Clearance.ClearanceId,
              crewMember.Clearance.Description), null);
          }
        }
        // check data integrity
        Assert.Equal(outData.Count, data.Length);
        for (int i = 0; i < data.Length; i++)
        {
          Assert.True(outData.ContainsKey(data[i]));
        }
      }
    }

    /// <summary>
    /// Tests fix for bug http://bugs.mysql.com/bug.php?id=68513, Error in LINQ to Entities query when using Distinct().Count().
    /// </summary>
    [Fact]
    public void DistinctCount()
    {
      ReInitDb();
      using (SiteDbContext ctx = new SiteDbContext())
      {
        ctx.Database.Initialize(true);
        visitante v1 = new visitante() { nCdSite = 1, nCdVisitante = 1, sDsIp = "x1" };
        visitante v2 = new visitante() { nCdSite = 1, nCdVisitante = 2, sDsIp = "x2" };
        site s1 = new site() { nCdSite = 1, sDsTitulo = "MyNewsPage" };
        site s2 = new site() { nCdSite = 2, sDsTitulo = "MySearchPage" };
        ctx.Visitante.Add(v1);
        ctx.Visitante.Add(v2);
        ctx.Site.Add(s1);
        ctx.Site.Add(s2);
        ctx.SaveChanges();

        var q = (from vis in ctx.Visitante.Include("site")
                  group vis by vis.nCdSite into g
                  select new retorno
                  {
                    Key = g.Key,
                    Online = g.Select(e => e.sDsIp).Distinct().Count()
                  });
        string sql = q.ToString();
        st.CheckSql(sql, SQLSyntax.CountGroupBy);
        var q2 = q.ToList<retorno>();
        foreach( var row in q2 )
        {
        }
      }
    }

    /// <summary>
    /// Tests fix for bug http://bugs.mysql.com/bug.php?id=68513, Error in LINQ to Entities query when using Distinct().Count().
    /// </summary>
    [Fact]
    public void DistinctCount2()
    {
      ReInitDb();
      using (SiteDbContext ctx = new SiteDbContext())
      {
        ctx.Database.Initialize(true);
        visitante v1 = new visitante() { nCdSite = 1, nCdVisitante = 1, sDsIp = "x1" };
        visitante v2 = new visitante() { nCdSite = 1, nCdVisitante = 2, sDsIp = "x2" };
        site s1 = new site() { nCdSite = 1, sDsTitulo = "MyNewsPage" };
        site s2 = new site() { nCdSite = 2, sDsTitulo = "MySearchPage" };
        pagina p1 = new pagina() { nCdPagina = 1, nCdVisitante = 1, sDsTitulo = "index.html" };
        ctx.Visitante.Add(v1);
        ctx.Visitante.Add(v2);
        ctx.Site.Add(s1);
        ctx.Site.Add(s2);
        ctx.Pagina.Add(p1);
        ctx.SaveChanges();
        
        var q = (from pag in ctx.Pagina.Include("visitante").Include("site")
                   group pag by pag.visitante.nCdSite into g
                   select new retorno
                   {
                       Key = g.Key,
                       Online = g.Select(e => e.visitante.sDsIp).Distinct().Count()
                   });        
        string sql = q.ToString();
        st.CheckSql(sql, SQLSyntax.CountGroupBy2);
        var q2 = q.ToList<retorno>();
        foreach (var row in q2)
        {
        }
      }
    }

    /// <summary>
    /// Tests fix for bug http://bugs.mysql.com/bug.php?id=65723, MySql Provider for EntityFramework produces "bad" SQL for OrderBy.
    /// </summary>
    [Fact]
    public void BadOrderBy()
    {
      ReInitDb();
      using (MovieDBContext db = new MovieDBContext())
      {
        db.Database.Initialize(true);
        Movie m1 = new Movie() { Title = "Terminator 1", ReleaseDate = new DateTime(1984, 10, 26) };
        Movie m2 = new Movie() { Title = "The Matrix", ReleaseDate = new DateTime(1999, 3, 31) };
        Movie m3 = new Movie() { Title = "Predator", ReleaseDate = new DateTime(1987, 6, 12) };
        Movie m4 = new Movie() { Title = "Star Wars, The Sith Revenge", ReleaseDate = new DateTime(2005, 5, 19) };
        db.Movies.Add(m1);
        db.Movies.Add(m2);
        db.Movies.Add(m3);
        db.Movies.Add(m4);
        db.SaveChanges();
        DateTime filterDate = new DateTime(1986, 1, 1);
        var q = db.Movies.Where(p => p.ReleaseDate >= filterDate).
          OrderByDescending(p => p.ReleaseDate).Take(2);
        string sql = q.ToString();
        st.CheckSql(SQLSyntax.NestedOrderBy, sql);
        // Data integrity testing
        Movie[] data = new Movie[] {
          new Movie() { ID = 4, Title = "Star Wars, The Sith Revenge", ReleaseDate = new DateTime( 2005, 5, 19 ) },
          new Movie() { ID = 2, Title = "The Matrix", ReleaseDate = new DateTime( 1999, 3, 31 ) }
        };
        int i = 0;
        foreach (Movie m in q)
        {
          Assert.Equal(data[i].ID, m.ID);
          Assert.Equal(data[i].Title, m.Title);
          Assert.Equal(data[i].ReleaseDate, m.ReleaseDate);
          i++;
        }
      }
    }
  }
}

