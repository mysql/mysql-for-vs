using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Antlr.Runtime;
using Antlr.Runtime.Tree;


namespace MySqlParser.Tests.DDL.Other
{
	[TestFixture]
	public class TruncateTable
	{
		[Test]
		public void Simple()
		{			
			MySQL51Parser.statement_list_return r = Utility.ParseSql("TRUNCATE TABLE table1");
			/*
			Assert.AreEqual(1, statements.Count);
			Assert.IsTrue(statements[0] is TruncateTableStatement);
			TruncateTableStatement ts = statements[0] as TruncateTableStatement;

			Assert.IsNull(ts.Table.Database);
			Assert.AreEqual("table1", ts.Table.Name.Text);
			 * */
		}

		[Test]
		public void SimpleWithShortSyntax()
		{			
			MySQL51Parser.statement_list_return r = Utility.ParseSql("TRUNCATE `schema1`.`table1`");
			/*
			Assert.AreEqual(1, statements.Count);
			Assert.IsTrue(statements[0] is TruncateTableStatement);
			TruncateTableStatement ts = statements[0] as TruncateTableStatement;

			Assert.AreEqual("`schema1`", ts.Table.Database.Text);
			Assert.AreEqual("`table1`", ts.Table.Name.Text);
			 * */
		}

		[Test]
		public void MissingTableName()
		{
			MySQL51Parser.statement_list_return r = Utility.ParseSql("TRUNCATE TABLE ", true);
		}
	}
}
