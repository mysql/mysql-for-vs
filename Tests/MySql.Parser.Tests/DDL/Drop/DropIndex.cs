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
	
	public class DropIndex
	{
		[Fact]
		public void Simple()
		{
      AstParserRuleReturnScope<object, IToken> r = Utility.ParseSql("DROP INDEX indexName ON table1");
			/*
			Assert.Equal(1, statements.Count);
			Assert.True(statements[0] is DropIndexStatement);
			DropIndexStatement ds = statements[0] as DropIndexStatement;
			Assert.Equal("indexName", ds.Index.Text);
			Assert.Null(ds.Table.Database);
			Assert.Equal("table1", ds.Table.Name.Text);
			*/
			r = Utility.ParseSql("DROP INDEX indexName ON schema2.`table2`");
			/*
			Assert.Equal(1, statements.Count);
			Assert.True(statements[0] is DropIndexStatement);
			ds = statements[0] as DropIndexStatement;
			Assert.Equal("indexName", ds.Index.Text);
			Assert.Equal("schema2", ds.Table.Database.Text);
			Assert.Equal("`table2`", ds.Table.Name.Text);
			 * */
		}

		[Fact]
		public void MissingIndexName()
		{
      AstParserRuleReturnScope<object, IToken> r = Utility.ParseSql("DROP INDEX", true);
		}

		[Fact]
		public void MissingTableName()
		{
      AstParserRuleReturnScope<object, IToken> r = Utility.ParseSql("DROP INDEX indexName ON", true);
		}
	}
}
