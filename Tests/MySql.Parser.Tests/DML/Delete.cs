// Copyright © 2013, 2016, Oracle and/or its affiliates. All rights reserved.
//
// MySQL for Visual Studio is licensed under the terms of the GPLv2
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
using Xunit;

namespace MySql.Parser.Tests.DML
{
  public class Delete
  {
    [Fact]
    public void MissingTableDeleteTest()
    {
      Utility.ParseSql("delete from ", true);
    }

    [Fact]
    public void DangerousDeleteTest()
    {
      Utility.ParseSql("delete quick from a");
    }

    [Fact]
    public void DeleteSimpleTest()
    {
      Utility.ParseSql("delete ignore from Table1 where ( Flag is null );");
    }

    [Fact]
    public void DeleteWithClausules()
    {
      Utility.ParseSql("delete ignore quick low_priority from Table2 where ( Id <> 1 ) order by Id desc limit 100");
    }

    [Fact]
    public void DeleteMultiTableTest()
    {
      Utility.ParseSql(
        @"delete from Table1, Table2.*, Table3.* using Table1, Table4 inner join Table5
        on Table4.KeyGuid = Table5.ForeignKeyGuid where ( IdKey <> 1 )");
    }

    [Fact]
    public void DeleteMultiTableTest2()
    {
      Utility.ParseSql(
        @"delete Table1.*, Table2, Table3 from Table1, Table2 inner join Table3
        on Table2.KeyId = Table3.ForeignKeyId where ( IdKey = 2 )");
    }

    [Fact]
    public void DeleteMultiTableWrongTest()
    {
      // TODO: Check if effectively is the multitable syntax disallowed in combination with order by.
      Utility.ParseSql(
        @"delete Table1.*, Table2, Table3 from Table1, Table2 inner join Table3
        on Table2.KeyId = Table3.ForeignKeyId where ( Id <> 1 ) order by Id desc", true);
    }

    [Fact]
    public void DeleteMultiTableWrongTest2()
    {
      Utility.ParseSql(
        @"delete Table1.*, Table2, Table3 from Table1, Table2 inner join Table3
        on Table2.KeyId = Table3.ForeignKeyId where ( Id <> 1 ) limit 1000", true);
    }

    [Fact]
    public void Subquery()
    {
      Utility.ParseSql(
                @"DELETE FROM t1
WHERE s11 > ANY
 (SELECT COUNT(*) /* no hint */ FROM t2
  WHERE NOT EXISTS
   (SELECT * FROM t3
    WHERE ROW(5*t2.s1,77)=
     (SELECT 50,11*s1 FROM t4 UNION SELECT 50,77 FROM
      (SELECT * FROM t5) AS t5)));", true);
    }

    [Fact]
    public void WithPartition_55()
    {
      string result = Utility.ParseSql(@"DELETE FROM employees PARTITION (p0, p1) WHERE fname LIKE 'j%';", true, new Version(5, 5, 50));
      Assert.True(result.IndexOf("unexpected 'employees'", StringComparison.OrdinalIgnoreCase) != -1);
    }

    [Fact]
    public void WithPartition_56()
    {
      Utility.ParseSql(@"DELETE FROM employees PARTITION (p0, p1) WHERE fname LIKE 'j%';", false, new Version(5, 6, 31));
    }
  }
}
