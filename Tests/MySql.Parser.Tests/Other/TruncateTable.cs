// Copyright (c) 2013, Oracle and/or its affiliates. All rights reserved.
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

using Antlr.Runtime;
using Antlr.Runtime.Tree;
using Xunit;


namespace MySql.Parser.Tests
{
	
	public class TruncateTable
	{
		[Fact]
		public void Simple()
		{			
			MySQL51Parser.program_return r = Utility.ParseSql("TRUNCATE TABLE table1");
			/*
			Assert.Equal(1, statements.Count);
			Assert.True(statements[0] is TruncateTableStatement);
			TruncateTableStatement ts = statements[0] as TruncateTableStatement;

			Assert.Null(ts.Table.Database);
			Assert.Equal("table1", ts.Table.Name.Text);
			 * */
		}

		[Fact]
		public void SimpleWithShortSyntax()
		{			
			MySQL51Parser.program_return r = Utility.ParseSql("TRUNCATE `schema1`.`table1`");
			/*
			Assert.Equal(1, statements.Count);
			Assert.True(statements[0] is TruncateTableStatement);
			TruncateTableStatement ts = statements[0] as TruncateTableStatement;

			Assert.Equal("`schema1`", ts.Table.Database.Text);
			Assert.Equal("`table1`", ts.Table.Name.Text);
			 * */
		}

		[Fact]
		public void MissingTableName()
		{
			MySQL51Parser.program_return r = Utility.ParseSql("TRUNCATE TABLE ", true);
		}
	}
}
