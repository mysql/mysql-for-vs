// Copyright (c) 2013, 2021, Oracle and/or its affiliates.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License, version 2.0, as
// published by the Free Software Foundation.
//
// This program is also distributed with certain software (including
// but not limited to OpenSSL) that is licensed under separate terms,
// as designated in a particular file or component or in included license
// documentation.  The authors of MySQL hereby grant you an
// additional permission to link the program and your derivative works
// with the separately licensed software that they have included with
// MySQL.
//
// Without limiting anything contained in the foregoing, this file,
// which is part of MySQL for Visual Studio, is also subject to the
// Universal FOSS Exception, version 1.0, a copy of which can be found at
// http://oss.oracle.com/licenses/universal-foss-exception.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU General Public License, version 2.0, for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using System;
using System.Text;
using Antlr.Runtime;
using Xunit;
//using MySQLParser;

namespace MySql.Parser.Tests
{  
  public class Insert
  {
    [Fact]
    public void Simple()
    {
      Utility.ParseSql(
        "insert into tableA ( col1, col2, col3 ) values ( 'a', tableB.colx, 4.55 )");
    }

    [Fact]
    public void WithSelect()
    {
      Utility.ParseSql(
        "insert into tableA ( col1, col2, col3 ) select 'a', tableB.colx, 4.55 from tableB");
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
      StringBuilder sb;
      Utility.ParseSql(@"INSERT INTO employees_copy SELECT * FROM employees PARTITION (p2);	", true, out sb, new Version(5, 5));
      Assert.True(sb.ToString().IndexOf("no viable alternative at input 'PARTITION'", StringComparison.OrdinalIgnoreCase) != -1);
    }

    [Fact]
    public void WithPartition_56()
    {
      StringBuilder sb;
      Utility.ParseSql(@"INSERT INTO employees_copy SELECT * FROM employees PARTITION (p2);	", false, out sb, new Version(5, 6));
    }

    [Fact]
    public void WithPartition_2_55()
    {
      StringBuilder sb;
      Utility.ParseSql(@"INSERT INTO employees PARTITION (p3) VALUES (20, 'Jan', 'Jones', 1, 3);	", true, out sb, new Version(5, 5));
      Assert.True(sb.ToString().IndexOf("no viable alternative at input 'PARTITION'", StringComparison.OrdinalIgnoreCase) != -1);
    }

    [Fact]
    public void WithPartition_2_56()
    {
      StringBuilder sb;
      Utility.ParseSql(@"INSERT INTO employees PARTITION (p3) VALUES (20, 'Jan', 'Jones', 1, 3);	", false, out sb, new Version(5, 6));
    }
  }
}
