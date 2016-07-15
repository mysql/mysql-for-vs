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
  public class Insert
  {
    [Fact]
    public void Simple()
    {
      Utility.ParseSql("insert into tableA ( col1, col2, col3 ) values ( 'a', tableB.colx, 4.55 )");
    }

    [Fact]
    public void WithSelect()
    {
      Utility.ParseSql("insert into tableA ( col1, col2, col3 ) select 'a', tableB.colx, 4.55 from tableB");
    }

    [Fact]
    public void WithSelect2()
    {
      Utility.ParseSql(
                @"INSERT INTO t2
SELECT
a.X +
a.AUTO_INCR_PK -
b.FIRST_KEY_IN_SERIES AS ID
FROM
t1
INNER JOIN
(
SELECT
X,
MIN(AUTO_INCR_PK) AS FIRST_KEY_IN_SERIES
FROM
t1
GROUP BY
X
) AS b
USING
(X)");
    }

    [Fact]
    public void WithSelect3()
    {
      Utility.ParseSql("INSERT INTO table2 (field1, field2, field3, field4) (SELECT 'value1 from user input', field1, field2, field3 from table1)");
    }

    [Fact]
    public void WithoutColumns()
    {
      Utility.ParseSql("insert into test3 values (1), (2), (3)");
    }

    [Fact]
    public void WithPartition_55()
    {
      string result = Utility.ParseSql(@"INSERT INTO employees_copy SELECT * FROM employees PARTITION (p2);	", true, new Version(5, 5, 0));
      Assert.True(result.IndexOf("'(' (opening parenthesis) is not valid input at this position", StringComparison.OrdinalIgnoreCase) != -1);
    }

    [Fact]
    public void WithPartition_56()
    {
      Utility.ParseSql(@"INSERT INTO employees_copy SELECT * FROM employees PARTITION (p2);	", false, new Version(5, 6, 31));
    }

    [Fact]
    public void WithPartition_2_55()
    {
      string result = Utility.ParseSql(@"INSERT INTO employees PARTITION (p3) VALUES (20, 'Jan', 'Jones', 1, 3);	", true, new Version(5, 5, 50));
      Assert.True(result.IndexOf("'partition'", StringComparison.OrdinalIgnoreCase) != -1);
    }

    [Fact]
    public void WithPartition_2_56()
    {
      Utility.ParseSql(@"INSERT INTO employees PARTITION (p3) VALUES (20, 'Jan', 'Jones', 1, 3);	", false, new Version(5, 6, 31));
    }
  }
}
