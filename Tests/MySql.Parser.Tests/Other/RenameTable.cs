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

namespace MySql.Parser.Tests
{
	
	public class RenameTable
	{
		[Fact]
		public void SimpleNoSchema()
		{
			AstParserRuleReturnScope<object, IToken> r = Utility.ParseSql("RENAME TABLE `table1` TO `table2`");
			/*
			Assert.Equal(1, statements.Count);
			Assert.True(statements[0] is RenameTableStatement);
			RenameTableStatement ds = statements[0] as RenameTableStatement;
			Assert.Equal(1, ds.OldNames.Count);
			Assert.Equal(1, ds.NewNames.Count);

			Assert.Null(ds.OldNames[0].Database);
			Assert.Equal("`table1`", ds.OldNames[0].Name.Text);
			Assert.Null(ds.NewNames[0].Database);
			Assert.Equal("`table2`", ds.NewNames[0].Name.Text);
			 * */
		}

		[Fact]
		public void SimpleWithSchema()
		{
			AstParserRuleReturnScope<object, IToken> r = Utility.ParseSql(
				"RENAME TABLE `schema1`.`table1` TO `schema2`.`table2`");
			/*
			Assert.Equal(1, statements.Count);
			Assert.True(statements[0] is RenameTableStatement);
			RenameTableStatement ds = statements[0] as RenameTableStatement;
			Assert.Equal(1, ds.OldNames.Count);
			Assert.Equal(1, ds.NewNames.Count);

			Assert.Equal("`schema1`", ds.OldNames[0].Database.Text);
			Assert.Equal("`table1`", ds.OldNames[0].Name.Text);
			Assert.Equal("`schema2`", ds.NewNames[0].Database.Text);
			Assert.Equal("`table2`", ds.NewNames[0].Name.Text);
			 * */
		}

		[Fact]
		public void MissingFromTableName()
		{
			Utility.ParseSql("RENAME TABLE", true);
		}

		[Fact]
		public void MissingToTableName()
		{
			Utility.ParseSql("RENAME TABLE table1 TO", true);
		}

		[Fact]
		public void MultipleRenames()
		{
			AstParserRuleReturnScope<object, IToken> r = Utility.ParseSql(
				@"RENAME TABLE table1 TO table2, schema1.table4 TO table5, 
				`schema3`.table6 TO `schema7`.table8");
			/*
			Assert.Equal(1, statements.Count);
			Assert.True(statements[0] is RenameTableStatement);
			RenameTableStatement ds = statements[0] as RenameTableStatement;

			Assert.Equal(3, ds.OldNames.Count);
			Assert.Equal(3, ds.NewNames.Count);

			Assert.Null(ds.OldNames[0].Database);
			Assert.Equal("table1", ds.OldNames[0].Name.Text);
			Assert.Null(ds.NewNames[0].Database);
			Assert.Equal("table2", ds.NewNames[0].Name.Text);

			Assert.Equal("schema1", ds.OldNames[1].Database.Text);
			Assert.Equal("table4", ds.OldNames[1].Name.Text);
			Assert.Null(ds.NewNames[1].Database);
			Assert.Equal("table5", ds.NewNames[1].Name.Text);

			Assert.Equal("`schema3`", ds.OldNames[2].Database.Text);
			Assert.Equal("table6", ds.OldNames[2].Name.Text);
			Assert.Equal("`schema7`", ds.NewNames[2].Database.Text);
			Assert.Equal("table8", ds.NewNames[2].Name.Text);
			 * */
		}
	}
}
