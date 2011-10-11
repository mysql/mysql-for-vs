using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Antlr.Runtime;
using Antlr.Runtime.Tree;

namespace MySqlParser.Tests.DDL.Drop
{
	[TestFixture]
	public class DropTable
	{
		[Test]
		public void SimpleNoSchema()
		{			
			MySQL51Parser.statement_list_return r = Utility.ParseSql("DROP TABLE `tablename`");
			/*
			Assert.AreEqual(1, statements.Count);
			Assert.IsTrue(statements[0] is DropTableStatement);
			DropTableStatement ds = statements[0] as DropTableStatement;
			Assert.AreEqual(1, ds.ToDrop.Count);
			Assert.IsNull(ds.ToDrop[0].Database);
			Assert.AreEqual("`tablename`", ds.ToDrop[0].Name.Text);*/
		}

		[Test]
		public void SimpleWithSchema()
		{			
			MySQL51Parser.statement_list_return r = Utility.ParseSql("DROP TABLE `schema1`.`tablename`");
			/*
			Assert.AreEqual(1, statements.Count);
			Assert.IsTrue(statements[0] is DropTableStatement);
			DropTableStatement ds = statements[0] as DropTableStatement;
			Assert.AreEqual(1, ds.ToDrop.Count);
			Assert.AreEqual("`schema1`", ds.ToDrop[0].Database.Text);
			Assert.AreEqual("`tablename`", ds.ToDrop[0].Name.Text);
			 * */
		}

		[Test]
		public void MissingTableName()
		{
			MySQL51Parser.statement_list_return r = Utility.ParseSql("DROP TABLE", true);			
		}

		[Test]
		public void MultipleTables()
		{			
			MySQL51Parser.statement_list_return r = Utility.ParseSql("DROP TABLE `table1`, schema2.table2, `schema3`.`table3`, table4");
			/*
			Assert.AreEqual(1, statements.Count);
			Assert.IsTrue(statements[0] is DropTableStatement);
			DropTableStatement ds = statements[0] as DropTableStatement;
			Assert.AreEqual(4, ds.ToDrop.Count);

			Assert.IsNull(ds.ToDrop[0].Database);
			Assert.AreEqual("`table1`", ds.ToDrop[0].Name.Text);

			Assert.AreEqual("schema2", ds.ToDrop[1].Database.Text);
			Assert.AreEqual("table2", ds.ToDrop[1].Name.Text);

			Assert.AreEqual("`schema3`", ds.ToDrop[2].Database.Text);
			Assert.AreEqual("`table3`", ds.ToDrop[2].Name.Text);

			Assert.IsNull(ds.ToDrop[3].Database);
			Assert.AreEqual("table4", ds.ToDrop[3].Name.Text);
			 * */
		}

		[Test]
		public void IfExists()
		{			
			MySQL51Parser.statement_list_return r = Utility.ParseSql("DROP TABLE IF EXISTS `tablename`");
			/*
			Assert.AreEqual(1, statements.Count);
			Assert.IsTrue(statements[0] is DropTableStatement);
			DropTableStatement ds = statements[0] as DropTableStatement;
			Assert.AreEqual(1, ds.ToDrop.Count);
			Assert.AreEqual("`tablename`", ds.ToDrop[0].Name.Text);
			Assert.IsTrue(ds.IfExists);
			 * */
		}

		[Test]
		public void Temporary()
		{
			MySQL51Parser.statement_list_return r = Utility.ParseSql("DROP TEMPORARY TABLE IF EXISTS `tablename`");
			/*
			Assert.AreEqual(1, statements.Count);
			Assert.IsTrue(statements[0] is DropTableStatement);
			DropTableStatement ds = statements[0] as DropTableStatement;
			Assert.AreEqual(1, ds.ToDrop.Count);
			Assert.AreEqual("`tablename`", ds.ToDrop[0].Name.Text);
			Assert.IsTrue(ds.Temporary);
			 * */
		}

		[Test]
		public void CascadeOrRestrict()
		{			
			MySQL51Parser.statement_list_return r = Utility.ParseSql("DROP TABLE IF EXISTS `tablename` CASCADE");
			/*
			DropTableStatement ds = statements[0] as DropTableStatement;
			Assert.IsTrue(ds.Cascade);
			Assert.IsFalse(ds.Restrict);
			*/
			r = Utility.ParseSql("DROP TABLE IF EXISTS `tablename` RESTRICT");
			/*
			ds = statements[0] as DropTableStatement;
			Assert.IsFalse(ds.Cascade);
			Assert.IsTrue(ds.Restrict);
			*/
			
		}

		[Test]
		public void CascadeAndRestrict()
		{
			MySQL51Parser.statement_list_return r = 
				Utility.ParseSql("DROP TABLE IF EXISTS `tablename` RESTRICT CASCADE", true);
		}
	}
}
