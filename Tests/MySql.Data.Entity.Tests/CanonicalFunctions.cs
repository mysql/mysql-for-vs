// Copyright © 2008, 2011, Oracle and/or its affiliates. All rights reserved.
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
using System.Data;
using System.Threading;
using MySql.Data.MySqlClient;
using MySql.Data.MySqlClient.Tests;
using System.Data.EntityClient;
using System.Data.Common;
using NUnit.Framework;
using System.Data.Objects;
using System.Linq;

namespace MySql.Data.Entity.Tests
{
  [TestFixture]
  public class CanonicalFunctions : BaseEdmTest
  {
    private EntityConnection GetEntityConnection()
    {
      string connectionString = String.Format(
          "metadata=TestDB.csdl|TestDB.msl|TestDB.ssdl;provider=MySql.Data.MySqlClient; provider connection string=\"{0}\"", GetConnectionString(true));
      EntityConnection connection = new EntityConnection(connectionString);
      return connection;
    }

    [Test]
    public void Bitwise()
    {
      using (testEntities context = new testEntities())
      {
        ObjectQuery<Int32> q = context.CreateQuery<Int32>("BitwiseAnd(255,15)");
        foreach (int i in q)
          Assert.AreEqual(15, i);
        q = context.CreateQuery<Int32>("BitwiseOr(240,31)");
        foreach (int i in q)
          Assert.AreEqual(255, i);
        q = context.CreateQuery<Int32>("BitwiseXor(255,15)");
        foreach (int i in q)
          Assert.AreEqual(240, i);
      }
    }

    [Test]
    public void CurrentDateTime()
    {
      DateTime current = DateTime.Now;

      using (testEntities context = new testEntities())
      {
        ObjectQuery<DateTime> q = context.CreateQuery<DateTime>("CurrentDateTime()");
        foreach (DateTime dt in q)
        {
          Assert.AreEqual(current.Year, dt.Year);
          Assert.AreEqual(current.Month, dt.Month);
          Assert.AreEqual(current.Day, dt.Day);
          // we don't check time as that will be always be different
        }
      }
    }

    [Test]
    public void YearMonthDay()
    {
      using (testEntities context = new testEntities())
      {
        ObjectQuery<DbDataRecord> q = context.CreateQuery<DbDataRecord>(
            @"SELECT c.DateBegan, Year(c.DateBegan), Month(c.DateBegan), Day(c.DateBegan)
                        FROM Companies AS c WHERE c.Id=1");
        foreach (DbDataRecord record in q)
        {
          Assert.AreEqual(1996, record[1]);
          Assert.AreEqual(11, record[2]);
          Assert.AreEqual(15, record[3]);
        }
      }
    }

    [Test]
    public void HourMinuteSecond()
    {
      using (testEntities context = new testEntities())
      {
        ObjectQuery<DbDataRecord> q = context.CreateQuery<DbDataRecord>(
            @"SELECT c.DateBegan, Hour(c.DateBegan), Minute(c.DateBegan), Second(c.DateBegan)
                        FROM Companies AS c WHERE c.Id=1");
        foreach (DbDataRecord record in q)
        {
          Assert.AreEqual(5, record[1]);
          Assert.AreEqual(18, record[2]);
          Assert.AreEqual(23, record[3]);
        }
      }
    }

    [Test]
    public void IndexOf()
    {
      using (testEntities context = new testEntities())
      {
        ObjectQuery<Int32> q = context.CreateQuery<Int32>(@"IndexOf('needle', 'haystackneedle')");
        foreach (int index in q)
          Assert.AreEqual(9, index);

        q = context.CreateQuery<Int32>(@"IndexOf('haystack', 'needle')");
        foreach (int index in q)
          Assert.AreEqual(0, index);
      }
    }

    [Test]
    public void LeftRight()
    {
      using (testEntities context = new testEntities())
      {
        string entitySQL = "CONCAT(LEFT('foo',3),RIGHT('bar',3))";
        ObjectQuery<String> query = context.CreateQuery<String>(entitySQL);
        foreach (string s in query)
          Assert.AreEqual("foobar", s);

        entitySQL = "CONCAT(LEFT('foobar',3),RIGHT('barfoo',3))";
        query = context.CreateQuery<String>(entitySQL);
        foreach (string s in query)
          Assert.AreEqual("foofoo", s);

        entitySQL = "CONCAT(LEFT('foobar',8),RIGHT('barfoo',8))";
        query = context.CreateQuery<String>(entitySQL);
        foreach (string s in query)
          Assert.AreEqual("foobarbarfoo", s);
      }
    }

    [Test]
    public void Length()
    {
      using (testEntities context = new testEntities())
      {
        string entitySQL = "Length('abc')";
        ObjectQuery<Int32> query = context.CreateQuery<Int32>(entitySQL);
        foreach (int len in query)
          Assert.AreEqual(3, len);
      }
    }

    [Test]
    public void Trims()
    {
      using (testEntities context = new testEntities())
      {
        ObjectQuery<string> query = context.CreateQuery<string>("LTrim('   text   ')");
        foreach (string s in query)
          Assert.AreEqual("text   ", s);
        query = context.CreateQuery<string>("RTrim('   text   ')");
        foreach (string s in query)
          Assert.AreEqual("   text", s);
        query = context.CreateQuery<string>("Trim('   text   ')");
        foreach (string s in query)
          Assert.AreEqual("text", s);
      }
    }

    [Test]
    public void Round()
    {
      using (testEntities context = new testEntities())
      {
        ObjectQuery<DbDataRecord> q = context.CreateQuery<DbDataRecord>(@"
                    SELECT o.Id, o.Freight, 
                    Round(o.Freight) AS [Rounded Freight],
                    Floor(o.Freight) AS [Floor of Freight], 
                    Ceiling(o.Freight) AS [Ceiling of Freight] 
                    FROM Orders AS o WHERE o.Id=1");
        foreach (DbDataRecord r in q)
        {
          Assert.AreEqual(1, r[0]);
          Assert.AreEqual(65.3, r[1]);
          Assert.AreEqual(65, r[2]);
          Assert.AreEqual(65, r[3]);
          Assert.AreEqual(66, r[4]);
        }
      }
    }

    [Test]
    public void Substring()
    {
      using (testEntities context = new testEntities())
      {
        ObjectQuery<string> query = context.CreateQuery<string>("SUBSTRING('foobarfoo',4,3)");
        query = context.CreateQuery<string>("SUBSTRING('foobarfoo',4,30)");
        foreach (string s in query)
          Assert.AreEqual("barfoo", s);
      }
    }

    [Test]
    public void ToUpperToLowerReverse()
    {
      using (testEntities context = new testEntities())
      {
        ObjectQuery<DbDataRecord> q = context.CreateQuery<DbDataRecord>(
            @"SELECT ToUpper(c.Name),ToLower(c.Name),
                    Reverse(c.Name) FROM Companies AS c WHERE c.Id=1");
        foreach (DbDataRecord r in q)
        {
          Assert.AreEqual("HASBRO", r[0]);
          Assert.AreEqual("hasbro", r[1]);
          Assert.AreEqual("orbsaH", r[2]);
        }
      }
    }

    [Test]
    public void Replace()
    {
      using (testEntities context = new testEntities())
      {
        ObjectQuery<string> q = context.CreateQuery<string>(
            @"Replace('abcdefghi', 'def', 'zzz')");
        foreach (string s in q)
          Assert.AreEqual("abczzzghi", s);
      }
    }

#if CLR4
    [Test]
    public void CanRoundToNonZeroDigits()
    {

      using (testEntities context = new testEntities())
      {
        DbDataRecord order = context.CreateQuery<DbDataRecord>(@"
                                        SELECT o.Id, o.Freight, 
                                        Round(o.Freight, 2) AS [Rounded Freight]
                                        FROM Orders AS o WHERE o.Id=10").First();

        Assert.AreEqual(350.54721, order[1]);
        Assert.AreEqual(350.55, order[2]);
      }
    }
#endif
  }
}