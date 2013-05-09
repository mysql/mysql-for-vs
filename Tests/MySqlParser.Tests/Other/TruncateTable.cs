﻿// Copyright © 2013, Oracle and/or its affiliates. All rights reserved.
//
// MySQL Connector/NET is licensed under the terms of the GPLv2
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
