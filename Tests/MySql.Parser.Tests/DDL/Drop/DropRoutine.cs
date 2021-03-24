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
	
	public class DropRoutine
	{		
		[Fact]
		public void SimpleNoSchema()
		{
      AstParserRuleReturnScope<object, IToken> r = Utility.ParseSql("DROP PROCEDURE procname");
      /*
			Assert.Equal(1, statements.Count);
			Assert.True(statements[0] is DropRoutineStatement);
			DropRoutineStatement ds = statements[0] as DropRoutineStatement;
			Assert.Null(ds.RoutineToDrop.Database);
			Assert.Equal("procname", ds.RoutineToDrop.Name.Text);
			 * */

      r = Utility.ParseSql("DROP FUNCTION `funcname`");
			/*
			Assert.Equal(1, statements.Count);
			Assert.True(statements[0] is DropRoutineStatement);
			ds = statements[0] as DropRoutineStatement;
			Assert.Null(ds.RoutineToDrop.Database);
			Assert.Equal("`funcname`", ds.RoutineToDrop.Name.Text);
			 * */
		}

		[Fact]
		public void Simple()
		{
      AstParserRuleReturnScope<object, IToken> r = Utility.ParseSql("DROP PROCEDURE schema1.procname");
      /*
			Assert.Equal(1, statements.Count);
			Assert.True(statements[0] is DropRoutineStatement);
			DropRoutineStatement ds = statements[0] as DropRoutineStatement;
			Assert.Equal("schema1", ds.RoutineToDrop.Database.Text);
			Assert.Equal("procname", ds.RoutineToDrop.Name.Text);
			 * */

      r = Utility.ParseSql("DROP FUNCTION `schema1`.`funcname`");
			/*
			Assert.Equal(1, statements.Count);
			Assert.True(statements[0] is DropRoutineStatement);
			ds = statements[0] as DropRoutineStatement;
			Assert.Equal("`schema1`", ds.RoutineToDrop.Database.Text);
			Assert.Equal("`funcname`", ds.RoutineToDrop.Name.Text);
			 * */
		}

		[Fact]
		public void MissingRoutineName()
		{
      AstParserRuleReturnScope<object, IToken> r = Utility.ParseSql("DROP PROCEDURE", true);
		}

		[Fact]
		public void IfExists()
		{
      AstParserRuleReturnScope<object, IToken> r = Utility.ParseSql("DROP PROCEDURE IF EXISTS `procname`");
			/*
			Assert.Equal(1, statements.Count);
			Assert.True(statements[0] is DropRoutineStatement);
			DropRoutineStatement ds = statements[0] as DropRoutineStatement;
			Assert.Equal("`procname`", ds.RoutineToDrop.Name.Text);
			Assert.True(ds.IfExists);*/
		}
	}
}
