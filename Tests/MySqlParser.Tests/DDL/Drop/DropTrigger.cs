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
	public class DropTrigger
	{
		[Test]
		public void SimpleNoSchema()
		{			
			MySQL51Parser.program_return r = Utility.ParseSql("DROP TRIGGER trigger1");
			/*
			Assert.AreEqual(1, statements.Count);
			Assert.IsTrue(statements[0] is DropTriggerStatement);
			DropTriggerStatement ds = statements[0] as DropTriggerStatement;
			Assert.IsNull(ds.TriggerToDrop.Database);
			Assert.AreEqual("trigger1", ds.TriggerToDrop.Name.Text);
			 * */
		}

		[Test]
		public void SimpleSchema()
		{			
			MySQL51Parser.program_return r = Utility.ParseSql("DROP TRIGGER schema1.trigger1");
			/*
			Assert.AreEqual(1, statements.Count);
			Assert.IsTrue(statements[0] is DropTriggerStatement);
			DropTriggerStatement ds = statements[0] as DropTriggerStatement;
			Assert.AreEqual("schema1", ds.TriggerToDrop.Database.Text);
			Assert.AreEqual("trigger1", ds.TriggerToDrop.Name.Text);
			*/

			r = Utility.ParseSql("DROP TRIGGER `schema2`.`trigger2`");
			/*
			Assert.AreEqual(1, statements.Count);
			Assert.IsTrue(statements[0] is DropTriggerStatement);
			ds = statements[0] as DropTriggerStatement;
			Assert.AreEqual("`schema2`", ds.TriggerToDrop.Database.Text);
			Assert.AreEqual("`trigger2`", ds.TriggerToDrop.Name.Text);
			 * */
		}

		[Test]
		public void MissingDbName()
		{
			MySQL51Parser.program_return r = Utility.ParseSql("DROP TRIGGER", true);
		}

		[Test]
		public void IfExists()
		{			
			MySQL51Parser.program_return r = Utility.ParseSql("DROP TRIGGER IF EXISTS `trigger1`");
			/*
			Assert.AreEqual(1, statements.Count);
			Assert.IsTrue(statements[0] is DropTriggerStatement);
			DropTriggerStatement ds = statements[0] as DropTriggerStatement;
			Assert.AreEqual("`trigger1`", ds.TriggerToDrop.Name.Text);
			Assert.IsTrue(ds.IfExists);
			 * */
		}
	}
}
