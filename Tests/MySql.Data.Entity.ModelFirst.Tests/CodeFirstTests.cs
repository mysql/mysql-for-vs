// Copyright © 2011, Oracle and/or its affiliates. All rights reserved.
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
using System.Data.Entity;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using MySql.Data.MySqlClient.Tests;
using System.Data.EntityClient;
using System.Data.Common;
using System.Data.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using MySql.Data.Entity.ModelFirst.Tests.Properties;

namespace MySql.Data.Entity.ModelFirst.Tests
{
  [TestFixture]
  public class CodeFirstTests : BaseModelFirstTest
  {
    /// <summary>
    /// Tests for fix of http://bugs.mysql.com/bug.php?id=61230
    /// ("The provider did not return a ProviderManifestToken string.").
    /// </summary>
    [Test]
    public void SimpleCodeFirstSelect()
    {
      MovieDBContext db = new MovieDBContext();
      var l = db.Movies.ToList();
      foreach (var i in l)
      {
      }
    }

    /// <summary>
    /// Tests for fix of http://bugs.mysql.com/bug.php?id=62150
    /// ("EF4.1, Code First, CreateDatabaseScript() generates an invalid MySQL script.").
    /// </summary>
    [Test]
    public void AlterTableTest()
    {
      MovieDBContext db = new MovieDBContext();
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
    [Test]
    public void ConcurrencyCheck()
    {
      using (MovieDBContext db = new MovieDBContext())
      {
        db.Database.ExecuteSqlCommand(
@"DROP TABLE IF EXISTS `test3`.`MovieReleases`");

        db.Database.ExecuteSqlCommand(
@"CREATE TABLE `MovieReleases` (
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
            CheckSql(m.Groups["item"].Value, SQLSyntax.UpdateWithSelect);
            Assert.Pass();
          }
        }
        Assert.Fail();
      }
    }

    /// <summary>
    /// This tests fix for http://bugs.mysql.com/bug.php?id=64216.
    /// </summary>
    [Test]
    public void CheckByteArray()
    {
      MovieDBContext db = new MovieDBContext();
      
      string dbCreationScript =
        ((IObjectContextAdapter)db).ObjectContext.CreateDatabaseScript();
      Regex rx = new Regex(@"`Data` (?<type>[^\)]*)", RegexOptions.Compiled | RegexOptions.Singleline);
      Match m = rx.Match(dbCreationScript);
      Assert.AreEqual(m.Groups["type"].Value, "longblob");
    }

/// <summary>
    /// Validates a stored procedure call using Code First
    /// Bug #14008699
    [Test]
    public void CallStoredProcedure()
    {
      using (MovieDBContext context = new MovieDBContext())
      {
        int count = context.Database.SqlQuery<int>("GetCount").First();

        Assert.AreEqual(5, count);
      }
    }

    /// <summary>
    /// Tests for fix of http://bugs.mysql.com/bug.php?id=63920
    /// Maxlength error when it's used code-first and inheritance (discriminator generated column)
    /// </summary>
    [Test]
    public void Bug63920_Test1()
    {
      using (VehicleDbContext context = new VehicleDbContext())
      {
        context.Database.Delete();
        
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

        Assert.AreEqual(context.Vehicles.Count(), records);
      }
    }

    /// <summary>
    /// Tests for fix of http://bugs.mysql.com/bug.php?id=63920
    /// Key reference generation script error when it's used code-first and a single table for the inherited models
    /// </summary>
    [Test]
    public void Bug63920_Test2()
    {
      using (VehicleDbContext2 context = new VehicleDbContext2())
      {
        context.Database.Delete();

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

        Assert.AreEqual(context.Vehicles.Count(), records);
      }
    }
  }
}

