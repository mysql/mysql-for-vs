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

using Antlr.Runtime;
using Xunit;

namespace MySql.Parser.Tests.DDL.Drop
{

  public class DropTable
  {
    [Fact]
    public void SimpleNoSchema()
    {
      AstParserRuleReturnScope<object, IToken> r = Utility.ParseSql("DROP TABLE `tablename`");
      /*
			Assert.Equal(1, statements.Count);
			Assert.True(statements[0] is DropTableStatement);
			DropTableStatement ds = statements[0] as DropTableStatement;
			Assert.Equal(1, ds.ToDrop.Count);
			Assert.Null(ds.ToDrop[0].Database);
			Assert.Equal("`tablename`", ds.ToDrop[0].Name.Text);*/
    }

    [Fact]
    public void SimpleWithSchema()
    {
      AstParserRuleReturnScope<object, IToken> r = Utility.ParseSql("DROP TABLE `schema1`.`tablename`");
      /*
			Assert.Equal(1, statements.Count);
			Assert.True(statements[0] is DropTableStatement);
			DropTableStatement ds = statements[0] as DropTableStatement;
			Assert.Equal(1, ds.ToDrop.Count);
			Assert.Equal("`schema1`", ds.ToDrop[0].Database.Text);
			Assert.Equal("`tablename`", ds.ToDrop[0].Name.Text);
			 * */
    }

    [Fact]
    public void MissingTableName()
    {
      AstParserRuleReturnScope<object, IToken> r = Utility.ParseSql("DROP TABLE", true);
    }

    [Fact]
    public void MultipleTables()
    {
      AstParserRuleReturnScope<object, IToken> r = Utility.ParseSql("DROP TABLE `table1`, schema2.table2, `schema3`.`table3`, table4");
      /*
			Assert.Equal(1, statements.Count);
			Assert.True(statements[0] is DropTableStatement);
			DropTableStatement ds = statements[0] as DropTableStatement;
			Assert.Equal(4, ds.ToDrop.Count);

			Assert.Null(ds.ToDrop[0].Database);
			Assert.Equal("`table1`", ds.ToDrop[0].Name.Text);

			Assert.Equal("schema2", ds.ToDrop[1].Database.Text);
			Assert.Equal("table2", ds.ToDrop[1].Name.Text);

			Assert.Equal("`schema3`", ds.ToDrop[2].Database.Text);
			Assert.Equal("`table3`", ds.ToDrop[2].Name.Text);

			Assert.Null(ds.ToDrop[3].Database);
			Assert.Equal("table4", ds.ToDrop[3].Name.Text);
			 * */
    }

    [Fact]
    public void IfExists()
    {
      AstParserRuleReturnScope<object, IToken> r = Utility.ParseSql("DROP TABLE IF EXISTS `tablename`");
      /*
			Assert.Equal(1, statements.Count);
			Assert.True(statements[0] is DropTableStatement);
			DropTableStatement ds = statements[0] as DropTableStatement;
			Assert.Equal(1, ds.ToDrop.Count);
			Assert.Equal("`tablename`", ds.ToDrop[0].Name.Text);
			Assert.True(ds.IfExists);
			 * */
    }

    [Fact]
    public void Temporary()
    {
      AstParserRuleReturnScope<object, IToken> r = Utility.ParseSql("DROP TEMPORARY TABLE IF EXISTS `tablename`");
      /*
			Assert.Equal(1, statements.Count);
			Assert.True(statements[0] is DropTableStatement);
			DropTableStatement ds = statements[0] as DropTableStatement;
			Assert.Equal(1, ds.ToDrop.Count);
			Assert.Equal("`tablename`", ds.ToDrop[0].Name.Text);
			Assert.True(ds.Temporary);
			 * */
    }

    [Fact]
    public void CascadeOrRestrict()
    {
      AstParserRuleReturnScope<object, IToken> r = Utility.ParseSql("DROP TABLE IF EXISTS `tablename` CASCADE");
      /*
			DropTableStatement ds = statements[0] as DropTableStatement;
			Assert.True(ds.Cascade);
			Assert.False(ds.Restrict);
			*/
      r = Utility.ParseSql("DROP TABLE IF EXISTS `tablename` RESTRICT");
      /*
			ds = statements[0] as DropTableStatement;
			Assert.False(ds.Cascade);
			Assert.True(ds.Restrict);
			*/

    }

    [Fact]
    public void CascadeAndRestrict()
    {
      Utility.ParseSql("DROP TABLE IF EXISTS `tablename` RESTRICT CASCADE", true);
    }
  }
}
