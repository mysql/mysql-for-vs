using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Antlr.Runtime;
using Antlr.Runtime.Tree;


namespace MySql.Parser.Tests.DDL.Drop
{
	[TestFixture]
	public class DropIndex
	{
		[Test]
		public void Simple()
		{			
			MySQL51Parser.program_return r = Utility.ParseSql("DROP INDEX indexName ON table1");
			/*
			Assert.AreEqual(1, statements.Count);
			Assert.IsTrue(statements[0] is DropIndexStatement);
			DropIndexStatement ds = statements[0] as DropIndexStatement;
			Assert.AreEqual("indexName", ds.Index.Text);
			Assert.IsNull(ds.Table.Database);
			Assert.AreEqual("table1", ds.Table.Name.Text);
			*/
			r = Utility.ParseSql("DROP INDEX indexName ON schema2.`table2`");
			/*
			Assert.AreEqual(1, statements.Count);
			Assert.IsTrue(statements[0] is DropIndexStatement);
			ds = statements[0] as DropIndexStatement;
			Assert.AreEqual("indexName", ds.Index.Text);
			Assert.AreEqual("schema2", ds.Table.Database.Text);
			Assert.AreEqual("`table2`", ds.Table.Name.Text);
			 * */
		}

		[Test]
		public void MissingIndexName()
		{
			MySQL51Parser.program_return r = Utility.ParseSql("DROP INDEX", true);
		}

		[Test]
		public void MissingTableName()
		{
			MySQL51Parser.program_return r = Utility.ParseSql("DROP INDEX indexName ON", true);
		}
	}
}
