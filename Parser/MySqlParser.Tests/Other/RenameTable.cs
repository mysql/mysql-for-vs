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
	public class RenameTable
	{
		[Test]
		public void SimpleNoSchema()
		{
			MySQL51Parser.statement_list_return r = Utility.ParseSql("RENAME TABLE `table1` TO `table2`");
			/*
			Assert.AreEqual(1, statements.Count);
			Assert.IsTrue(statements[0] is RenameTableStatement);
			RenameTableStatement ds = statements[0] as RenameTableStatement;
			Assert.AreEqual(1, ds.OldNames.Count);
			Assert.AreEqual(1, ds.NewNames.Count);

			Assert.IsNull(ds.OldNames[0].Database);
			Assert.AreEqual("`table1`", ds.OldNames[0].Name.Text);
			Assert.IsNull(ds.NewNames[0].Database);
			Assert.AreEqual("`table2`", ds.NewNames[0].Name.Text);
			 * */
		}

		[Test]
		public void SimpleWithSchema()
		{
			MySQL51Parser.statement_list_return r = Utility.ParseSql(
				"RENAME TABLE `schema1`.`table1` TO `schema2`.`table2`");
			/*
			Assert.AreEqual(1, statements.Count);
			Assert.IsTrue(statements[0] is RenameTableStatement);
			RenameTableStatement ds = statements[0] as RenameTableStatement;
			Assert.AreEqual(1, ds.OldNames.Count);
			Assert.AreEqual(1, ds.NewNames.Count);

			Assert.AreEqual("`schema1`", ds.OldNames[0].Database.Text);
			Assert.AreEqual("`table1`", ds.OldNames[0].Name.Text);
			Assert.AreEqual("`schema2`", ds.NewNames[0].Database.Text);
			Assert.AreEqual("`table2`", ds.NewNames[0].Name.Text);
			 * */
		}

		[Test]
		public void MissingFromTableName()
		{
			MySQL51Parser.statement_list_return r = Utility.ParseSql("RENAME TABLE", true);
		}

		[Test]
		public void MissingToTableName()
		{
			MySQL51Parser.statement_list_return r = Utility.ParseSql("RENAME TABLE table1 TO", true);
		}

		[Test]
		public void MultipleRenames()
		{
			MySQL51Parser.statement_list_return r = Utility.ParseSql(
				@"RENAME TABLE table1 TO table2, schema1.table4 TO table5, 
				`schema3`.table6 TO `schema7`.table8");
			/*
			Assert.AreEqual(1, statements.Count);
			Assert.IsTrue(statements[0] is RenameTableStatement);
			RenameTableStatement ds = statements[0] as RenameTableStatement;

			Assert.AreEqual(3, ds.OldNames.Count);
			Assert.AreEqual(3, ds.NewNames.Count);

			Assert.IsNull(ds.OldNames[0].Database);
			Assert.AreEqual("table1", ds.OldNames[0].Name.Text);
			Assert.IsNull(ds.NewNames[0].Database);
			Assert.AreEqual("table2", ds.NewNames[0].Name.Text);

			Assert.AreEqual("schema1", ds.OldNames[1].Database.Text);
			Assert.AreEqual("table4", ds.OldNames[1].Name.Text);
			Assert.IsNull(ds.NewNames[1].Database);
			Assert.AreEqual("table5", ds.NewNames[1].Name.Text);

			Assert.AreEqual("`schema3`", ds.OldNames[2].Database.Text);
			Assert.AreEqual("table6", ds.OldNames[2].Name.Text);
			Assert.AreEqual("`schema7`", ds.NewNames[2].Database.Text);
			Assert.AreEqual("table8", ds.NewNames[2].Name.Text);
			 * */
		}
	}
}
