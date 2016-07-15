// Copyright © 2014, 2016, Oracle and/or its affiliates. All rights reserved.
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
using System.Text;
using Xunit;

namespace MySql.Parser.Tests.Other
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
      Utility.ParseSql("explain extended select * from tbl", false, new Version(5, 1));
    }

    [Fact]
    public void Explain2WithDescribe()
    {
      Utility.ParseSql("describe extended select * from tbl", false, new Version(5, 1));
    }

    [Fact]
    public void Explain2WithDesc()
    {
      Utility.ParseSql("desc extended select * from tbl", false, new Version(5, 1));
    }

    [Fact]
    public void Explain3()
    {
      string result = Utility.ParseSql("explain partitions select * from tbl", true, new Version(5, 0, 0));
      Assert.True(result.IndexOf("'partitions'", StringComparison.OrdinalIgnoreCase) != -1);
    }

    [Fact]
    public void Explain4()
    {
      Utility.ParseSql("explain partitions select * from tbl", false);
    }

    [Fact]
    public void ExplainDelete_55()
    {
      string result = Utility.ParseSql("explain DELETE from t1;", true, new Version(5, 5, 50));
      Assert.True(result.IndexOf("delete", StringComparison.OrdinalIgnoreCase) != -1);
    }

    [Fact]
    public void ExplainDelete_56()
    {
      Utility.ParseSql("explain DELETE from t1;", false, new Version(5, 6, 31));
    }

    [Fact]
    public void ExplainInsert_55()
    {
      string result = Utility.ParseSql("explain INSERT into t1 ( col1, col2 ) values ( '', 1 );", true, new Version(5, 5, 50));
      Assert.True(result.IndexOf("insert", StringComparison.OrdinalIgnoreCase) != -1);
    }

    [Fact]
    public void ExplainInsert_56()
    {
      Utility.ParseSql("explain INSERT into t1 ( col1, col2 ) values ( '', 1 );", false, new Version(5, 6, 31));
    }

    [Fact]
    public void ExplainReplace_55()
    {
      string result = Utility.ParseSql("explain format = json REPLACE into t1 ( col1, col2 ) values ( '', 1 );", true, new Version(5, 5, 50));
      Assert.True(result.IndexOf("'=' (equal operator) is not valid input at this position", StringComparison.OrdinalIgnoreCase) != -1);
    }

    [Fact]
    public void ExplainReplace_56()
    {
      Utility.ParseSql("explain format = json REPLACE into t1 ( col1, col2 ) values ( '', 1 );", false, new Version(5, 6, 31));
    }

    [Fact]
    public void ExplainReplace_56WithDescribe()
    {
      Utility.ParseSql("describe format = json REPLACE into t1 ( col1, col2 ) values ( '', 1 );", false, new Version(5, 6, 31));
    }

    [Fact]
    public void ExplainReplace_56WithDesc()
    {
      Utility.ParseSql("desc format = json REPLACE into t1 ( col1, col2 ) values ( '', 1 );", false, new Version(5, 6, 31));
    }

    [Fact]
    public void ExplainUpdate_55()
    {
      string result = Utility.ParseSql("explain format = traditional UPDATE t1 set col1 = val1, col2 = val2;", true, new Version(5, 5, 50));
      Assert.True(result.IndexOf("'=' (equal operator) is not valid input at this position", StringComparison.OrdinalIgnoreCase) != -1);
    }

    [Fact]
    public void ExplainUpdate_56()
    {
      Utility.ParseSql("explain format = traditional UPDATE t1 set col1 = val1, col2 = val2;", false, new Version(5, 6, 31));
    }

    [Fact]
    public void ExplainForConnection_56()
    {
      Utility.ParseSql("explain format = traditional for connection 1;", true, new Version(5, 6, 31));
    }

    [Fact]
    public void ExplainForConnection_57()
    {
      Utility.ParseSql("explain format = traditional for connection 1;", false, new Version(5, 7, 12));
    }
  }
}
