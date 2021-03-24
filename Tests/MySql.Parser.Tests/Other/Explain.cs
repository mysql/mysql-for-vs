// Copyright (c) 2014, 2021, Oracle and/or its affiliates.
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


namespace MySql.Parser.Tests
{
  
  public class Explain
  {
    [Fact]
    public void Explain1()
    {
      Utility.ParseSql("explain tbl", false);
    }

    [Fact]
    public void Explain1WithDescribe()
    {
      Utility.ParseSql("describe tbl", false);
    }

    [Fact]
    public void Explain1WithDesc()
    {
      Utility.ParseSql("desc tbl", false);
    }

    [Fact]
    public void Explain2()
    {
      Utility.ParseSql(
        "explain extended select * from tbl", false, new Version(5, 1));
    }

    [Fact]
    public void Explain2WithDescribe()
    {
      Utility.ParseSql(
        "describe extended select * from tbl", false, new Version(5, 1));
    }

    [Fact]
    public void Explain2WithDesc()
    {
      Utility.ParseSql(
        "desc extended select * from tbl", false, new Version(5, 1));
    }

    [Fact]
    public void Explain3()
    {
      StringBuilder sb;
      Utility.ParseSql(
        "explain partitions select * from tbl", true, out sb, new Version(5, 0));
      Assert.True(sb.ToString().IndexOf("no viable alternative at input 'partitions'", StringComparison.OrdinalIgnoreCase ) != -1);
    }

    [Fact]
    public void Explain4()
    {
      Utility.ParseSql("explain partitions select * from tbl", false);
    }

    [Fact]
    public void ExplainDelete_55()
    {
      StringBuilder sb;
      Utility.ParseSql(
        "explain DELETE from t1;", true, out sb, new Version(5, 5));
      Assert.True(sb.ToString().IndexOf("delete", StringComparison.OrdinalIgnoreCase) != -1);
    }

    [Fact]
    public void ExplainDelete_56()
    {
      StringBuilder sb;
      Utility.ParseSql(
        "explain DELETE from t1;", false, out sb, new Version(5, 6));
    }

    [Fact]
    public void ExplainInsert_55()
    {
      StringBuilder sb;
      Utility.ParseSql(
        "explain INSERT into t1 ( col1, col2 ) values ( '', 1 );", true, out sb, new Version(5, 5));
      Assert.True(sb.ToString().IndexOf("insert", StringComparison.OrdinalIgnoreCase) != -1);
    }

    [Fact]
    public void ExplainInsert_56()
    {
      Utility.ParseSql(
        "explain INSERT into t1 ( col1, col2 ) values ( '', 1 );", false, new Version(5, 6));
    }

    [Fact]
    public void ExplainReplace_55()
    {
      StringBuilder sb;
      Utility.ParseSql(
        "explain format = json REPLACE into t1 ( col1, col2 ) values ( '', 1 );", true, out sb, new Version(5, 5));
      Assert.True(sb.ToString().IndexOf("no viable alternative at input '='", StringComparison.OrdinalIgnoreCase) != -1);
    }

    [Fact]
    public void ExplainReplace_56()
    {
      Utility.ParseSql(
        "explain format = json REPLACE into t1 ( col1, col2 ) values ( '', 1 );", false, new Version(5, 6));
    }

    [Fact]
    public void ExplainReplace_56WithDescribe()
    {
      Utility.ParseSql(
        "describe format = json REPLACE into t1 ( col1, col2 ) values ( '', 1 );", false, new Version(5, 6));
    }

    [Fact]
    public void ExplainReplace_56WithDesc()
    {
      Utility.ParseSql(
        "desc format = json REPLACE into t1 ( col1, col2 ) values ( '', 1 );", false, new Version(5, 6));
    }

    [Fact]
    public void ExplainUpdate_55()
    {
      StringBuilder sb;
      Utility.ParseSql(
        "explain format = traditional UPDATE t1 set col1 = val1, col2 = val2;", true, out sb, new Version(5, 5));
      //Assert.True(sb.ToString().IndexOf("missing EndOfFile at '='", StringComparison.OrdinalIgnoreCase) != -1);
      Assert.True(sb.ToString().IndexOf("no viable alternative at input '='", StringComparison.OrdinalIgnoreCase) != -1);
    }

    [Fact]
    public void ExplainUpdate_56()
    {
      Utility.ParseSql("explain format = traditional UPDATE t1 set col1 = val1, col2 = val2;", false, new Version(5, 6));
    }

    [Fact]
    public void ExplainForConnection_56()
    {
      Utility.ParseSql("explain format = traditional for connection 1;", true, new Version(5, 6));
    }

    [Fact]
    public void ExplainForConnection_57()
    {
      Utility.ParseSql("explain format = traditional for connection 1;", false, new Version(5, 7));
    }
  }
}
