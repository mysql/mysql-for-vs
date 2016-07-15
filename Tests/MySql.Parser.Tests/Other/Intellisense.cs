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
using System.Text.RegularExpressions;
using Xunit;

namespace MySql.Parser.Tests.Other
{
  /// <summary>
  /// Tests to verify the error reporting interface for Intellisense client.
  /// </summary>

  public class Intellisense
  {
    // ToDo: MYSQLFORVS-612 - This should be working, once autoComplete is available in WB's parser.
    //[Fact]
    //public void SelectSimpleTableCompletion()
    //{
    //  string result = Utility.ParseSql("select * from ", true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken == "table_factor" || expectedToken == "simple_table_ref_no_alias_existing");
    //}

    //[Fact]
    //public void SelectSimpleTableCompletionWitBeginEnd()
    //{
    //  string result = Utility.ParseSql("begin select * from;  end", true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken == "table_factor" || expectedToken == "simple_table_ref_no_alias_existing");
    //}

    //[Fact]
    //public void SelectSimpleTableCompletionWitBeginEnd2()
    //{
    //  string result = Utility.ParseSql("begin select * from  ", true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken == "table_factor" || expectedToken == "simple_table_ref_no_alias_existing");
    //}

    //[Fact]
    //public void SelectSimpleTableCompletionWithBeginEnd3()
    //{
    //  string result = Utility.ParseSql("begin select * from end", true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken == "table_factor" || expectedToken == "simple_table_ref_no_alias_existing");
    //}

    //[Fact]
    //public void SelectSimpleTableCompletionWithoutBeginEnd()
    //{
    //  Utility.ParseSql("select * from end", false);
    //}

    //[Fact]
    //public void SelectJoinTableCompletion()
    //{
    //  string result = Utility.ParseSql("select facility.Id from facility inner join t2 on true left join ", true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken == "table_factor" || expectedToken == "simple_table_ref_no_alias_existing");
    //}

    //[Fact]
    //public void TruncateTableCompletion()
    //{
    //  TestTableExpected("truncate table ");
    //}

    //[Fact]
    //public void ShowCreateTableCompletion()
    //{
    //  TestTableExpected("show create table ");
    //}

    //[Fact]
    //public void DropTableCompletion()
    //{
    //  TestTableExpected("drop table ");
    //}

    //[Fact]
    //public void UpdateTableCompletion()
    //{
    //  TestTableExpected("update ");
    //}

    //[Fact]
    //public void DeleteFromTableCompletion()
    //{
    //  TestTableExpected("delete from ");
    //}

    //[Fact]
    //public void InsertIntoTableCompletion()
    //{
    //  TestTableExpected("insert into ");
    //}

    //[Fact]
    //public void RenameTableCompletion()
    //{
    //  TestTableExpected("rename table ");
    //}

    //private void TestTableExpected(string sql)
    //{
    //  string result = Utility.ParseSql(sql, true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken == "table_factor" || expectedToken == "simple_table_ref_no_alias_existing");
    //}

    //[Fact]
    //public void CallProcedureNameCompletion()
    //{
    //  string result = Utility.ParseSql("call", true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken == "proc_name");
    //}

    //[Fact]
    //public void SelectColumnCompletionWithTables()
    //{
    //  string sql = "select *, fromtable.name, from fromtable inner join computer";
    //  string result = Utility.ParseSql(sql, true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken == "column_name");
    //}

    //[Fact]
    //public void SelectColumnCompletionWithTableWithAlias()
    //{
    //  string sql = "select *, fromtable.name, from fromtable as a inner join computer as B";
    //  string result = Utility.ParseSql(sql, true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken == "column_name");
    //}

    //[Fact]
    //public void SelectColumnCompletionWithTableWithDatabase()
    //{
    //  string sql = "select *, fromtable.name, from test2.fromtable inner join test1.computer as B";
    //  string result = Utility.ParseSql(sql, true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken == "column_name");
    //}

    //[Fact]
    //public void SelectColumnCompletionWithoutFrom()
    //{
    //  string result = Utility.ParseSql("select", true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken == "column_name");
    //}

    //[Fact]
    //public void SelectColumnCompletionWithoutFrom2()
    //{
    //  string result = Utility.ParseSql("select a, ", true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken == "column_name");
    //}

    //[Fact]
    //public void SelectTableCompletionIncorrect()
    //{
    //  string result = Utility.ParseSql("select * from `fromtable` as inner join `fromtable`", true);
    //  Assert.True(result.EndsWith("no viable alternative at input 'inner'\r\n", StringComparison.CurrentCultureIgnoreCase));
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.False(expectedToken == "column_name");
    //}

    //[Fact]
    //public void ColumnCompletionAtSelectWhere()
    //{
    //  string result = Utility.ParseSql("select a from t where ", true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken == "column_name");
    //}

    //[Fact]
    //public void ColumnCompletionAtUpdateWhere()
    //{
    //  string result = Utility.ParseSql("update t set c = 5 where  ", true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken == "column_name");
    //}

    //[Fact]
    //public void ColumnCompletionAtUpdateWhereWithMinus()
    //{
    //  string result = Utility.ParseSql("update t set c = 5 where - ", true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken == "column_name");
    //}

    //[Fact]
    //public void ColumnCompletionAtUpdateWhereWithMinus2()
    //{
    //  string result = Utility.ParseSql("update t set c = 5 where a = ", true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken == "column_name");
    //}

    //[Fact]
    //public void ColumnCompletionOnExpression()
    //{
    //  string result = Utility.ParseSql("select * from `fromtable` where `fromtable`.`Id` = ", true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken == "column_name");
    //}

    //[Fact]
    //public void ColumnCompletionOnExpression2()
    //{
    //  string result = Utility.ParseSql("select * from `fromtable` where `fromtable`.`Id` between ", true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken == "column_name");
    //}

    //[Fact]
    //public void ColumnCompletionOnExpression3()
    //{
    //  string result = Utility.ParseSql("select * from `fromtable` where `fromtable`.`Id` between c and ", true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken == "column_name");
    //}

    //[Fact]
    //public void ColumnCompletionOnExpression4()
    //{
    //  string result = Utility.ParseSql("select * from `fromtable` where `fromtable`.`Id` & ", true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken == "column_name");
    //}

    //[Fact]
    //public void ColumnCompletionOnExpression5()
    //{
    //  string result = Utility.ParseSql("select * from `fromtable` where `fromtable`.`Id` >> ", true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken == "column_name");
    //}

    //[Fact]
    //public void ColumnCompletionOnExpression6()
    //{
    //  string result = Utility.ParseSql("select * from `fromtable` where `fromtable`.`Id` * ", true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken == "column_name");
    //}

    //[Fact]
    //public void ColumnCompletionOnExpression7()
    //{
    //  string result = Utility.ParseSql("select * from `fromtable` where `fromtable`.`Id` ^ ", true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken == "column_name");
    //}

    //[Fact]
    //public void ColumnCompletionOnExpression8()
    //{
    //  string result = Utility.ParseSql("select * from `fromtable` where `fromtable`.`Id` = 1 and binary ", true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken == "column_name");
    //}

    //[Fact]
    //public void ColumnCompletionOnExpression9()
    //{
    //  string result = Utility.ParseSql("select * from `fromtable` where `fromtable`.`Id` = 1 and interval ", true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken == "column_name");
    //}

    //[Fact]
    //public void ColumnCompletionOnExpression10()
    //{
    //  string result = Utility.ParseSql("select * from `fromtable` where `fromtable`.`Id` = 1 and ( ", true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken == "column_name");
    //}

    //[Fact]
    //public void ColumnCompletionOnExpression11()
    //{
    //  string result = Utility.ParseSql("select * from `fromtable` where `fromtable`.`Id` = 1 and { id ", true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken == "column_name");
    //}

    //[Fact]
    //public void ColumnCompletionOnExpression12()
    //{
    //  string result = Utility.ParseSql("select * from `fromtable` where `fromtable`.`Id` = 1 and match ( ", true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken == "column_name");
    //}

    //[Fact]
    //public void NonColumnCompletionOnExpression12()
    //{
    //  string result = Utility.ParseSql("select * from `fromtable` where `fromtable`.`Id` = 1 and match ", true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken != "column_name");
    //}

    //[Fact]
    //public void ColumnCompletionOnExpression13()
    //{
    //  string result = Utility.ParseSql("select * from `fromtable` where `fromtable`.`Id` = 1 and match ( a, b, c ) against ( ", true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken == "column_name");
    //}

    //[Fact]
    //public void NonColumnCompletionOnExpression13()
    //{
    //  string result = Utility.ParseSql("select * from `fromtable` where `fromtable`.`Id` = 1 and match ( a, b, c ) against ", true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken != "column_name");
    //}

    //[Fact]
    //public void ColumnCompletionOnExpression14()
    //{
    //  string result = Utility.ParseSql("select * from `fromtable` where `fromtable`.`Id` = 1 and match ( a, b, ", true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken == "column_name");
    //}

    //[Fact]
    //public void ColumnCompletionOnExpression15()
    //{
    //  string result = Utility.ParseSql("select * from `fromtable` where `fromtable`.`Id` = 1 and case when ", true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken == "column_name");
    //}

    //[Fact]
    //public void ColumnCompletionOnExpression16()
    //{
    //  string result = Utility.ParseSql("select * from `fromtable` where `fromtable`.`Id` = 1 and case when true then ", true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken == "column_name");
    //}

    //[Fact]
    //public void ColumnCompletionOnExpression17()
    //{
    //  string result = Utility.ParseSql("select * from `fromtable` where `fromtable`.`Id` = 1 and case when ( a = b ) then x + 1 else ",
    //    true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken == "column_name");
    //}

    //[Fact]
    //public void ColumnCompletionOnExpression18()
    //{
    //  string result = Utility.ParseSql("case ", true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken == "column_name");
    //}

    //[Fact]
    //public void ColumnCompletionOnExpression19()
    //{
    //  string result = Utility.ParseSql("case when ", true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken == "column_name");
    //}

    //[Fact]
    //public void RegressionTest2()
    //{
    //  string result = Utility.ParseSql("-", true);
    //  Assert.True(result.EndsWith("no viable alternative at input '-'\r\n"));
    //}

    //[Fact]
    //public void ColumnCompletionAtDeleteWhere()
    //{
    //  string result = Utility.ParseSql("delete from t where ", true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken == "column_name");
    //}

    //[Fact]
    //public void ColumnCompletionAtInsert()
    //{
    //  string result = Utility.ParseSql("insert into ta(", true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken == "column_name");
    //}

    //[Fact]
    //public void ColumnCompletionAtInsertPartialList()
    //{
    //  string result = Utility.ParseSql("insert into ta( a, ", true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken == "column_name");
    //}

    //[Fact]
    //public void ColumnCompletionAtInsertSelect()
    //{
    //  string result = Utility.ParseSql("insert into ta select ", true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken == "column_name");
    //}

    //[Fact]
    //public void ColumnCompletionAtInsertSelectWithFrom()
    //{
    //  string result = Utility.ParseSql("insert into ta select *, from a inner join b on true", true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken == "column_name");
    //}

    //[Fact]
    //public void ColumnCompletionAtUpdate()
    //{
    //  string result = Utility.ParseSql("update t set ", true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken == "column_name");
    //}

    //[Fact]
    //public void ColumnCompletionBeginEnd()
    //{
    //  string result = Utility.ParseSql("CREATE PROCEDURE doiterate(p1 INT) begin select * from t where;  end ", true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken == "column_name");
    //}

    //[Fact]
    //public void ColumnCompletionBeginEnd2()
    //{
    //  string result = Utility.ParseSql("CREATE PROCEDURE doiterate(p1 INT) begin select ;  end ", true);
    //  string expectedToken = new Regex(@"Expected (?<item>.*)\.").Match(result).Groups["item"].Value;
    //  Assert.True(expectedToken == "column_name");
    //}
  }
}
