using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
//using MySQLParser;

namespace MySqlParser.Tests
{
	[TestFixture]
	public class Insert
	{
		[Test]
		public void Simple()
		{			
			
			MySQL51Parser.statement_list_return r = Utility.ParseSql(
				"insert into tableA ( col1, col2, col3 ) values ( 'a', tableB.colx, 4.55 )");
		}

		[Test]
		public void WithSelect()
		{
			MySQL51Parser.statement_list_return r = Utility.ParseSql(
				"insert into tableA ( col1, col2, col3 ) select 'a', tableB.colx, 4.55 from tableB");
		}
	}
}
