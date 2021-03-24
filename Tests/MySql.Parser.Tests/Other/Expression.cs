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

using System.Text;
using Antlr.Runtime;
using Xunit;


namespace MySql.Parser.Tests
{
  public class Expression
  {
    [Fact]
    public void Sum()
    {
      Utility.ParseSql("select ( a + b )");
    }

    [Fact]
    public void CaseSimple()
    {
      Utility.ParseSql("select CASE WHEN 1 THEN 'one' WHEN 2 THEN 'two' END;");
    }

    [Fact]
    public void CaseSimpleWithElse()
    {
      Utility.ParseSql("select CASE WHEN 1 THEN 'one' WHEN 2 THEN 'two' ELSE 'more' END;");
    }

    [Fact]
    public void IfSimple()
    {
      Utility.ParseSql("SELECT IF(1<2,'yes','no');", false);
    }

    [Fact]
    public void IfNull()
    {
      Utility.ParseSql("SELECT IFNULL(1/0,10);");
    }

    [Fact]
    public void NullIf()
    {
      Utility.ParseSql("SELECT NULLIF(1,2);");
    }

    [Fact]
    public void ParameterMarker()
    {
      Utility.ParseSql(@"SELECT @myvar, id FROM MyTable WHERE id >= @maxId");
    }
  }
}
