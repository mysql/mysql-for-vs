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

using System.Collections.Generic;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using Xunit;

namespace MySql.Parser.Tests
{
  
  public class GatheringTables
  {
    [Fact]
    public void InsertWithTableExtraction()
    {
      AstParserRuleReturnScope<object, IToken> r = Utility.ParseSql(
          "INSERT INTO test6.d_table VALUES (1);");
      List<TableWithAlias> twa = new List<TableWithAlias>();
      ParserUtils.GetTables((ITree)r.Tree, twa);
      Assert.Equal(1, twa.Count);
      Assert.Equal("test6", twa[0].Database);
      Assert.Equal("d_table", twa[0].TableName);
    }

    [Fact]
    public void InsertWithTableExtraction2()
    {
      AstParserRuleReturnScope<object, IToken> r = Utility.ParseSql(
          "INSERT INTO `test6`.`d_table` VALUES (1);");
      List<TableWithAlias> twa = new List<TableWithAlias>();
      ParserUtils.GetTables((ITree)r.Tree, twa);
      Assert.Equal(1, twa.Count);
      Assert.Equal("test6", twa[0].Database);
      Assert.Equal("d_table", twa[0].TableName);
      Assert.True(string.IsNullOrEmpty(twa[0].Alias));
    }

    [Fact]
    public void InsertWithTableExtraction3()
    {
      AstParserRuleReturnScope<object, IToken> r = Utility.ParseSql(
          "INSERT INTO test6.d_table ( col ) VALUES (1);");
      List<TableWithAlias> twa = new List<TableWithAlias>();
      ParserUtils.GetTables((ITree)r.Tree, twa);
      Assert.Equal(1, twa.Count);
      Assert.Equal("test6", twa[0].Database);
      Assert.Equal("d_table", twa[0].TableName);
    }

    [Fact]
    public void SelectWithTableExtraction()
    {
      AstParserRuleReturnScope<object, IToken> r = Utility.ParseSql(
          "select * from test6.d_table;");
      List<TableWithAlias> twa = new List<TableWithAlias>();
      ParserUtils.GetTables((ITree)r.Tree, twa);
      Assert.Equal(1, twa.Count);
      Assert.Equal("test6", twa[0].Database);
      Assert.Equal("d_table", twa[0].TableName);
      Assert.True(string.IsNullOrEmpty(twa[0].Alias));
    }

    [Fact]
    public void SelectWithTableExtraction2()
    {
      AstParserRuleReturnScope<object, IToken> r = Utility.ParseSql(
          "select * from test6.d_table as T;");
      List<TableWithAlias> twa = new List<TableWithAlias>();
      ParserUtils.GetTables((ITree)r.Tree, twa);
      Assert.Equal(1, twa.Count);
      Assert.Equal("test6", twa[0].Database);
      Assert.Equal("d_table", twa[0].TableName);
      Assert.Equal("T", twa[0].Alias);
    }

    [Fact]
    public void SelectWithTableExtraction3()
    {
      AstParserRuleReturnScope<object, IToken> r = Utility.ParseSql(
          "select * from d_table as T;");
      List<TableWithAlias> twa = new List<TableWithAlias>();
      ParserUtils.GetTables((ITree)r.Tree, twa);
      Assert.Equal(1, twa.Count);
      Assert.True(string.IsNullOrEmpty(twa[0].Database));
      Assert.Equal("d_table", twa[0].TableName);
      Assert.Equal("T", twa[0].Alias);
    }

    [Fact]
    public void SelectWithTableExtraction4()
    {
      AstParserRuleReturnScope<object, IToken> r = Utility.ParseSql(
          "select * from d_table;");
      List<TableWithAlias> twa = new List<TableWithAlias>();
      ParserUtils.GetTables((ITree)r.Tree, twa);
      Assert.Equal(1, twa.Count);
      Assert.True(string.IsNullOrEmpty(twa[0].Database));
      Assert.Equal("d_table", twa[0].TableName);
      Assert.True(string.IsNullOrEmpty(twa[0].Alias));
    }
  }
}
