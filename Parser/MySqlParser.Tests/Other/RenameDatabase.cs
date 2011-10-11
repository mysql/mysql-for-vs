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
	public class RenameDatabase
	{
		[Test]
		public void Simple()
		{
			MySQL51Parser.statement_list_return r = Utility.ParseSql("RENAME DATABASE `db1` TO `db2`");
			/*
			Assert.AreEqual(1, statements.Count);
			Assert.IsTrue(statements[0] is RenameDatabaseStatement);
			RenameDatabaseStatement ds = statements[0] as RenameDatabaseStatement;

			Assert.AreEqual("`db1`", ds.OldName.Text);
			Assert.AreEqual("`db2`", ds.NewName.Text);
			 * */
		}

		[Test]
		public void MissingFromDbName()
		{
			MySQL51Parser.statement_list_return r = Utility.ParseSql("RENAME DATABASE ", true);
		}

		[Test]
		public void MissingToDbName()
		{
			MySQL51Parser.statement_list_return r = Utility.ParseSql("RENAME DATABASE db1 TO", true);
		}
	}
}
