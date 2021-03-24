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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using Xunit;

namespace MySql.Parser.Tests
{
  /// <summary>
  /// Tests to verify the error reporting interface for Intellisense client.
  /// </summary>
  
  public class Intellisense
  {
    [Fact]
    public void SelectSimpleTableCompletion()
    {
      StringBuilder sb;
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql("select * from ", true, out sb);
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.True(
        expectedToken == "table_factor" ||
        expectedToken == "simple_table_ref_no_alias_existing");
    }

    [Fact]
    public void SelectSimpleTableCompletionWitBeginEnd()
    {
      StringBuilder sb;
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql("begin select * from;  end", true, out sb);
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.True(
        expectedToken == "table_factor" ||
        expectedToken == "simple_table_ref_no_alias_existing");
    }

    [Fact]
    public void SelectSimpleTableCompletionWitBeginEnd2()
    {
      StringBuilder sb;
      Utility.ParseSql("begin select * from  ", true, out sb);
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.True(
        expectedToken == "table_factor" ||
        expectedToken == "simple_table_ref_no_alias_existing");
    }

    [Fact]
    public void SelectSimpleTableCompletionWithBeginEnd3()
    {
      StringBuilder sb;
      Utility.ParseSql("begin select * from end", true, out sb);
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.True(
        expectedToken == "table_factor" ||
        expectedToken == "simple_table_ref_no_alias_existing");
    }

    [Fact]
    public void SelectSimpleTableCompletionWithoutBeginEnd()
    {
      Utility.ParseSql("select * from end", false);
    }

    [Fact]
    public void SelectJoinTableCompletion()
    {
      StringBuilder sb;
      Utility.ParseSql("select facility.Id from facility inner join t2 on true left join ",
        true, out sb);
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.True(
        expectedToken == "table_factor" ||
        expectedToken == "simple_table_ref_no_alias_existing");
    }

    [Fact]
    public void TruncateTableCompletion()
    {
      TestTableExpected("truncate table ");
    }

    [Fact]
    public void TruncateTableCompletion2()
    {
      //TestTableExpected("truncate table db.t");
    }

    [Fact]
    public void ShowCreateTableCompletion()
    {
      TestTableExpected("show create table ");
    }

    [Fact]
    public void DropTableCompletion()
    {
      TestTableExpected("drop table ");
    }

    [Fact]
    public void UpdateTableCompletion()
    {
      TestTableExpected("update ");
    }

    [Fact]
    public void DeleteFromTableCompletion()
    {
      TestTableExpected("delete from ");
    }

    [Fact]
    public void InsertIntoTableCompletion()
    {
      TestTableExpected("insert into ");
    }

    [Fact]
    public void RenameTableCompletion()
    {
      TestTableExpected("rename table ");
    }

    private void TestTableExpected(string sql)
    {
      StringBuilder sb;
      Utility.ParseSql(sql, true, out sb);
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.True(
        expectedToken == "table_factor" ||
        expectedToken == "simple_table_ref_no_alias_existing");
    }

    [Fact]
    public void CallProcedureNameCompletion()
    {
      StringBuilder sb;
      Utility.ParseSql( "call", true, out sb );
      Assert.True(sb.ToString().EndsWith("no viable alternative at input '<EOF>'\r\n"));
      //string expectedToken =
      //    new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      //Assert.True(expectedToken == "proc_name");
      //Assert.True(((CommonErrorNode)r.Tree).Text == "call");
    }

    [Fact]
    public void SelectColumnCompletionWithTables()
    {
      StringBuilder sb;
      string sql = "select *, fromtable.name, from fromtable inner join computer";
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql( sql, true, out sb);
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.True(expectedToken == "column_name");
      List<TableWithAlias> tables = new List<TableWithAlias>();
      ParserUtils.GetTables((ITree)r.Tree, tables);
      List<string> tablesUsed = new string[] { "fromtable", "computer" }.ToList();
      Assert.Equal(tablesUsed.Count, tables.Count);
      foreach (TableWithAlias ta in tables)
      {
        Assert.True(tablesUsed.Contains(ta.TableName.ToLower()));
      }
      //Assert.True(sb.ToString().EndsWith("no viable alternative at input 'FROM'\r\n",
      //  StringComparison.CurrentCultureIgnoreCase));
      //Match m = new Regex(@"select (?<columns>.*) (?<from>from .*$)").Match(sql);
      //if (m.Success)
      //{
      //  sql = string.Format("select c {0}", m.Groups["from"].Value);
      //  r = Utility.ParseSql(sql, false, out sb);
      //  List<TableWithAlias> tables = new List<TableWithAlias>();
      //  ParserUtils.GetTables((ITree)r.Tree, tables);
      //  List<string> tablesUsed = new string[] { "fromtable", "computer" }.ToList();
      //  Assert.Equal(tablesUsed.Count, tables.Count);        
      //  foreach (TableWithAlias ta in tables)
      //  {
      //    Assert.True(tablesUsed.Contains(ta.TableName.ToLower() ));
      //  }
      //}
    }    

    [Fact]
    public void SelectColumnCompletionWithTableWithAlias()
    {
      StringBuilder sb;
      string sql = "select *, fromtable.name, from fromtable as a inner join computer as B";
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql(sql, true, out sb);
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.True(expectedToken == "column_name");
      List<TableWithAlias> tables = new List<TableWithAlias>();
      ParserUtils.GetTables((ITree)r.Tree, tables);
      List<TableWithAlias> tablesUsed = new List<TableWithAlias>();
      tablesUsed.Add(new TableWithAlias("fromtable", "a"));
      tablesUsed.Add(new TableWithAlias("computer", "B"));
      Assert.Equal(tablesUsed.Count, tables.Count);
      foreach (TableWithAlias ta in tablesUsed)
      {
        Assert.True(tables.Contains(ta));
      }
      Assert.Equal(tablesUsed.Count, tables.Count);      
      //Assert.True(sb.ToString().EndsWith("no viable alternative at input 'FROM'\r\n",
      //  StringComparison.CurrentCultureIgnoreCase));
      //Match m = new Regex(@"select (?<columns>.*) (?<from>from .*$)").Match(sql);
      //if (m.Success)
      //{
      //  sql = string.Format("select c {0}", m.Groups["from"].Value);
      //  r = Utility.ParseSql(sql, false, out sb);
      //  List<TableWithAlias> tables = new List<TableWithAlias>();
      //  ParserUtils.GetTables((ITree)r.Tree, tables);
      //  List<TableWithAlias> tablesUsed = new List<TableWithAlias>();
      //  tablesUsed.Add( new TableWithAlias( "fromtable", "a" ) );
      //  tablesUsed.Add( new TableWithAlias( "computer", "B" ) );
      //  Assert.Equal(tablesUsed.Count, tables.Count);
      //  foreach (TableWithAlias ta in tablesUsed)
      //  {          
      //    Assert.True(tables.Contains(ta));
      //  }
      //}
    }

    [Fact]
    public void SelectColumnCompletionWithTableWithDatabase()
    {
      StringBuilder sb;
      string sql = "select *, fromtable.name, from test2.fromtable inner join test1.computer as B";
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql(sql, true, out sb);
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.True(expectedToken == "column_name");
      List<TableWithAlias> tables = new List<TableWithAlias>();
      ParserUtils.GetTables((ITree)r.Tree, tables);
      List<TableWithAlias> tablesUsed = new List<TableWithAlias>();
      tablesUsed.Add(new TableWithAlias("test2", "fromtable", ""));
      tablesUsed.Add(new TableWithAlias("test1", "computer", "B"));
      Assert.Equal(tablesUsed.Count, tables.Count);
      foreach (TableWithAlias ta in tablesUsed)
      {
        Assert.True(tables.Contains(ta));
      }
      Assert.Equal(tablesUsed.Count, tables.Count);      
      //Assert.True(sb.ToString().EndsWith("no viable alternative at input 'FROM'\r\n",
      //  StringComparison.CurrentCultureIgnoreCase));
      //Match m = new Regex(@"select (?<columns>.*) (?<from>from .*$)").Match(sql);
      //if (m.Success)
      //{
      //  sql = string.Format("select c {0}", m.Groups["from"].Value);
      //  r = Utility.ParseSql(sql, false, out sb);
      //  List<TableWithAlias> tables = new List<TableWithAlias>();
      //  ParserUtils.GetTables((ITree)r.Tree, tables);
      //  List<TableWithAlias> tablesUsed = new List<TableWithAlias>();
      //  tablesUsed.Add(new TableWithAlias( "test2", "fromtable", ""));
      //  tablesUsed.Add(new TableWithAlias( "test1", "computer", "B"));
      //  Assert.Equal(tablesUsed.Count, tables.Count);
      //  foreach (TableWithAlias ta in tablesUsed)
      //  {
      //    Assert.True(tables.Contains(ta));
      //  }
      //}
    }

    [Fact]
    public void SelectColumnCompletionWithoutFrom()
    {
      // "select" 
      StringBuilder sb;
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql("select", true, out sb);
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.True(expectedToken == "column_name" );
      //Assert.True(r.Tree is CommonErrorNode);
    }

    [Fact]
    public void SelectColumnCompletionWithoutFrom2()
    {
      // "select" 
      StringBuilder sb;
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql("select a, ", true, out sb);
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.True(expectedToken == "column_name");
      Assert.True((r.Tree as ITree).GetChild( 0 ).Text.Equals( 
        "select", StringComparison.CurrentCultureIgnoreCase ));
    }

    [Fact]
    public void SelectTableCompletionIncorrect()
    {
      // "select" 
      StringBuilder sb;
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql("select * from `fromtable` as inner join `fromtable`", true, out sb);
      Assert.True( sb.ToString().EndsWith( 
        "no viable alternative at input 'inner'\r\n", StringComparison.CurrentCultureIgnoreCase) );
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.False(expectedToken == "column_name");
    }

    [Fact]
    public void ColumnCompletionAtSelectWhere()
    {
      StringBuilder sb;
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql("select a from t where ", true, out sb);
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.True(expectedToken == "column_name");
      List<TableWithAlias> tables = new List<TableWithAlias>();
      ParserUtils.GetTables((ITree)r.Tree, tables);
      Assert.True(tables.Count != 0);
    }

    [Fact]
    public void ColumnCompletionAtUpdateWhere()
    {
      StringBuilder sb;
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql("update t set c = 5 where  ", true, out sb);
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.True(expectedToken == "column_name");
      List<TableWithAlias> tables = new List<TableWithAlias>();
      ParserUtils.GetTables((ITree)r.Tree, tables);
      Assert.True(tables.Count != 0);
    }

    [Fact]
    public void ColumnCompletionAtUpdateWhereWithMinus()
    {
      StringBuilder sb;
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql("update t set c = 5 where - ", true, out sb);
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.True(expectedToken == "column_name");
      List<TableWithAlias> tables = new List<TableWithAlias>();
      ParserUtils.GetTables((ITree)r.Tree, tables);
      Assert.True(tables.Count != 0);
    }

    [Fact]
    public void ColumnCompletionAtUpdateWhereWithMinus2()
    {      
      StringBuilder sb;
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql("update t set c = 5 where a = ", true, out sb);
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.True(expectedToken == "column_name");
      List<TableWithAlias> tables = new List<TableWithAlias>();
      ParserUtils.GetTables((ITree)r.Tree, tables);
      Assert.True(tables.Count != 0);
    }    
    
    [Fact]
    public void ColumnCompletionOnExpression()
    {      
      StringBuilder sb;
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql("select * from `fromtable` where `fromtable`.`Id` = ", true, out sb);
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.True(expectedToken == "column_name");
      List<TableWithAlias> tables = new List<TableWithAlias>();
      ParserUtils.GetTables((ITree)r.Tree, tables);
      Assert.True(tables.Count != 0);
    }

    [Fact]
    public void ColumnCompletionOnExpression2()
    {      
      StringBuilder sb;
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql("select * from `fromtable` where `fromtable`.`Id` between ", true, out sb);
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.True(expectedToken == "column_name");
      List<TableWithAlias> tables = new List<TableWithAlias>();
      ParserUtils.GetTables((ITree)r.Tree, tables);
      Assert.True(tables.Count != 0);
    }

    [Fact]
    public void ColumnCompletionOnExpression3()
    {      
      StringBuilder sb;
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql("select * from `fromtable` where `fromtable`.`Id` between c and ", true, out sb);
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.True(expectedToken == "column_name");
      List<TableWithAlias> tables = new List<TableWithAlias>();
      ParserUtils.GetTables((ITree)r.Tree, tables);
      Assert.True(tables.Count != 0);
    }

    [Fact]
    public void ColumnCompletionOnExpression4()
    {      
      StringBuilder sb;
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql("select * from `fromtable` where `fromtable`.`Id` & ", true, out sb);
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.True(expectedToken == "column_name");
      List<TableWithAlias> tables = new List<TableWithAlias>();
      ParserUtils.GetTables((ITree)r.Tree, tables);
      Assert.True(tables.Count != 0);
    }

    [Fact]
    public void ColumnCompletionOnExpression5()
    {      
      StringBuilder sb;
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql("select * from `fromtable` where `fromtable`.`Id` >> ", true, out sb);
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.True(expectedToken == "column_name");
      List<TableWithAlias> tables = new List<TableWithAlias>();
      ParserUtils.GetTables((ITree)r.Tree, tables);
      Assert.True(tables.Count != 0);
    }

    [Fact]
    public void ColumnCompletionOnExpression6()
    {      
      StringBuilder sb;
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql("select * from `fromtable` where `fromtable`.`Id` * ", true, out sb);
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.True(expectedToken == "column_name");
      List<TableWithAlias> tables = new List<TableWithAlias>();
      ParserUtils.GetTables((ITree)r.Tree, tables);
      Assert.True(tables.Count != 0);
    }

    [Fact]
    public void ColumnCompletionOnExpression7()
    {      
      StringBuilder sb;
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql("select * from `fromtable` where `fromtable`.`Id` ^ ", true, out sb);
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.True(expectedToken == "column_name");
      List<TableWithAlias> tables = new List<TableWithAlias>();
      ParserUtils.GetTables((ITree)r.Tree, tables);
      Assert.True(tables.Count != 0);
    }

    [Fact]
    public void ColumnCompletionOnExpression8()
    {      
      StringBuilder sb;
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql("select * from `fromtable` where `fromtable`.`Id` = 1 and binary ", true, out sb);
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.True(expectedToken == "column_name");
      List<TableWithAlias> tables = new List<TableWithAlias>();
      ParserUtils.GetTables((ITree)r.Tree, tables);
      Assert.True(tables.Count != 0);
    }

    [Fact]
    public void ColumnCompletionOnExpression9()
    {      
      StringBuilder sb;
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql("select * from `fromtable` where `fromtable`.`Id` = 1 and interval ", true, out sb);
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.True(expectedToken == "column_name");
      List<TableWithAlias> tables = new List<TableWithAlias>();
      ParserUtils.GetTables((ITree)r.Tree, tables);
      Assert.True(tables.Count != 0);
    }

    [Fact]
    public void ColumnCompletionOnExpression10()
    {      
      StringBuilder sb;
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql("select * from `fromtable` where `fromtable`.`Id` = 1 and ( ", true, out sb);
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.True(expectedToken == "column_name");
      List<TableWithAlias> tables = new List<TableWithAlias>();
      ParserUtils.GetTables((ITree)r.Tree, tables);
      Assert.True(tables.Count != 0);
    }

    [Fact]
    public void ColumnCompletionOnExpression11()
    {      
      StringBuilder sb;
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql("select * from `fromtable` where `fromtable`.`Id` = 1 and { id ", true, out sb);
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.True(expectedToken == "column_name");
      List<TableWithAlias> tables = new List<TableWithAlias>();
      ParserUtils.GetTables((ITree)r.Tree, tables);
      Assert.True(tables.Count != 0);
    }

    [Fact]
    public void ColumnCompletionOnExpression12()
    {      
      StringBuilder sb;
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql("select * from `fromtable` where `fromtable`.`Id` = 1 and match ( ", true, out sb);
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.True(expectedToken == "column_name");
      List<TableWithAlias> tables = new List<TableWithAlias>();
      ParserUtils.GetTables((ITree)r.Tree, tables);
      Assert.True(tables.Count != 0);
    }

    [Fact]
    public void NonColumnCompletionOnExpression12()
    {      
      StringBuilder sb;
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql("select * from `fromtable` where `fromtable`.`Id` = 1 and match ", true, out sb);
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.True(expectedToken != "column_name");
      List<TableWithAlias> tables = new List<TableWithAlias>();
      ParserUtils.GetTables((ITree)r.Tree, tables);
      Assert.True(tables.Count == 1);
    }

    [Fact]
    public void ColumnCompletionOnExpression13()
    {      
      StringBuilder sb;
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql("select * from `fromtable` where `fromtable`.`Id` = 1 and match ( a, b, c ) against ( ", true, out sb);
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.True(expectedToken == "column_name");
      List<TableWithAlias> tables = new List<TableWithAlias>();
      ParserUtils.GetTables((ITree)r.Tree, tables);
      Assert.True(tables.Count != 0);
    }

    [Fact]
    public void NonColumnCompletionOnExpression13()
    {      
      StringBuilder sb;
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql("select * from `fromtable` where `fromtable`.`Id` = 1 and match ( a, b, c ) against ", true, out sb);
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.True(expectedToken != "column_name");
      List<TableWithAlias> tables = new List<TableWithAlias>();
      ParserUtils.GetTables((ITree)r.Tree, tables);
      Assert.True(tables.Count != 0);
    }

    [Fact]
    public void ColumnCompletionOnExpression14()
    {
      StringBuilder sb;
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql("select * from `fromtable` where `fromtable`.`Id` = 1 and match ( a, b, ", true, out sb);
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.True(expectedToken == "column_name");
      List<TableWithAlias> tables = new List<TableWithAlias>();
      ParserUtils.GetTables((ITree)r.Tree, tables);
      Assert.True(tables.Count != 0);
    }

    [Fact]
    public void ColumnCompletionOnExpression15()
    {
      StringBuilder sb;
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql("select * from `fromtable` where `fromtable`.`Id` = 1 and case when ", true, out sb);
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.True(expectedToken == "column_name");
      List<TableWithAlias> tables = new List<TableWithAlias>();
      ParserUtils.GetTables((ITree)r.Tree, tables);
      Assert.True(tables.Count != 0);
    }

    [Fact]
    public void ColumnCompletionOnExpression16()
    {
      StringBuilder sb;
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql("select * from `fromtable` where `fromtable`.`Id` = 1 and case when true then ", true, out sb);
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.True(expectedToken == "column_name");
      List<TableWithAlias> tables = new List<TableWithAlias>();
      ParserUtils.GetTables((ITree)r.Tree, tables);
      Assert.True(tables.Count != 0);
    }

    [Fact]
    public void ColumnCompletionOnExpression17()
    {
      StringBuilder sb;
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql("select * from `fromtable` where `fromtable`.`Id` = 1 and case when ( a = b ) then x + 1 else ", 
        true, out sb);
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.True(expectedToken == "column_name");
      List<TableWithAlias> tables = new List<TableWithAlias>();
      ParserUtils.GetTables((ITree)r.Tree, tables);
      Assert.True(tables.Count != 0);
    }

    [Fact]
    public void ColumnCompletionOnExpression18()
    {
      StringBuilder sb;
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql("case ",
        true, out sb);
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.True(expectedToken == "column_name");
      List<TableWithAlias> tables = new List<TableWithAlias>();
      ParserUtils.GetTables((ITree)r.Tree, tables);
      Assert.True(tables.Count == 0);
    }

    [Fact]
    public void ColumnCompletionOnExpression19()
    {
      StringBuilder sb;
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql("case when ",
        true, out sb);
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.True(expectedToken == "column_name");
      List<TableWithAlias> tables = new List<TableWithAlias>();
      ParserUtils.GetTables((ITree)r.Tree, tables);
      Assert.True(tables.Count == 0);
    }
    
    [Fact]
    public void RegressionTest2()
    {      
      StringBuilder sb;
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql("-", true, out sb);
      Assert.True( sb.ToString().EndsWith( "no viable alternative at input '-'\r\n" ));
    }

    [Fact]
    public void ColumnCompletionAtDeleteWhere()
    {
      // "delete from t where "
      StringBuilder sb;
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql("delete from t where ", true, out sb);
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.True(expectedToken == "column_name");
      List<TableWithAlias> tables = new List<TableWithAlias>();
      ParserUtils.GetTables((ITree)r.Tree, tables);
      Assert.True(tables.Count != 0);
    }

    [Fact]
    public void ColumnCompletionAtInsert()
    {
      // "insert into ta("
      // "insert into ta() values"
      // "insert into ta select "
      StringBuilder sb;
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql("insert into ta(", true, out sb);
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.True(expectedToken == "column_name");
      List<TableWithAlias> tables = new List<TableWithAlias>();
      ParserUtils.GetTables((ITree)r.Tree, tables);
      Assert.True(tables.Count != 0);
    }

    [Fact]
    public void ColumnCompletionAtInsertPartialList()
    {
      // "insert into ta( a,  "
      // "insert into ta( a, b "
      // "insert into ta( a, b, "
      StringBuilder sb;
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql("insert into ta( a, ", true, out sb);
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.True(expectedToken == "column_name");
      List<TableWithAlias> tables = new List<TableWithAlias>();
      ParserUtils.GetTables((ITree)r.Tree, tables);
      Assert.True(tables.Count != 0);
    }

    [Fact]
    public void ColumnCompletionAtInsertSelect()
    {      
      StringBuilder sb;
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql("insert into ta select ", true, out sb);
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.True(expectedToken == "column_name");
      List<TableWithAlias> tables = new List<TableWithAlias>();
      ParserUtils.GetTables((ITree)r.Tree, tables);
      Assert.True(tables.Count != 0);
    }

    [Fact]
    public void ColumnCompletionAtInsertSelectWithFrom()
    {
      StringBuilder sb;
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql("insert into ta select *, from a inner join b on true", true, out sb);
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.True(expectedToken == "column_name");
      List<TableWithAlias> tables = new List<TableWithAlias>();
      ParserUtils.GetTables((ITree)r.Tree, tables);
      Assert.True(tables.Count != 0);
    }

    [Fact]
    public void ColumnCompletionAtUpdate()
    {
      // "update t set "
      StringBuilder sb;
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql("update t set ", true, out sb);
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.True(expectedToken == "column_name");
      List<TableWithAlias> tables = new List<TableWithAlias>();
      ParserUtils.GetTables((ITree)r.Tree, tables);
      Assert.True(tables.Count != 0);
    }

    [Fact]
    public void ColumnCompletionBeginEnd()
    {
      StringBuilder sb;
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql("CREATE PROCEDURE doiterate(p1 INT) begin select * from t where;  end ", true, out sb);
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.True(expectedToken == "column_name");
      List<TableWithAlias> tables = new List<TableWithAlias>();
      ParserUtils.GetTables((ITree)r.Tree, tables);
      Assert.True(tables.Count != 0);
    }

    [Fact]
    public void ColumnCompletionBeginEnd2()
    {
      StringBuilder sb;
      AstParserRuleReturnScope<object, IToken> r =
        Utility.ParseSql("CREATE PROCEDURE doiterate(p1 INT) begin select ;  end ", true, out sb);
      string expectedToken =
          new Regex(@"Expected (?<item>.*)\.").Match(sb.ToString()).Groups["item"].Value;
      Assert.True(expectedToken == "column_name");
      List<TableWithAlias> tables = new List<TableWithAlias>();
      ParserUtils.GetTables((ITree)r.Tree, tables);
      Assert.True(tables.Count == 0);
    }
  }
}
