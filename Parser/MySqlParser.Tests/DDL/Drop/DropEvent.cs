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
	public class DropEvent
	{
		[Test]
		public void Simple()
		{
			
			MySQL51Parser.statement_list_return r = Utility.ParseSql("DROP EVENT eventName");
			/*
			Assert.AreEqual(1, statements.Count);
			Assert.IsTrue(statements[0] is DropEventStatement);
			DropEventStatement ds = statements[0] as DropEventStatement;
			Assert.AreEqual("eventName", ds.EventToDrop.Text);
			*/
			r = Utility.ParseSql("DROP EVENT `eventName`");
			/*
			Assert.AreEqual(1, statements.Count);
			Assert.IsTrue(statements[0] is DropEventStatement);
			ds = statements[0] as DropEventStatement;
			Assert.AreEqual("`eventName`", ds.EventToDrop.Text);
			 * */
		}

		[Test]
		public void MissingEventName()
		{
			MySQL51Parser.statement_list_return r = Utility.ParseSql("DROP EVENT", true);
			/*
			try
			{
				MySqlParser p = new MySqlParser();
				p.Parse("DROP EVENT");
				Assert.Fail("This should have thrown an exception");
			}
			catch (MySqlParserException mpe)
			{
				Assert.AreEqual(MySqlParserExceptionCode.IdentifierExpected, mpe.Code);
			}
			 * */
		}

		[Test]
		public void IfExists()
		{
			MySQL51Parser.statement_list_return r = Utility.ParseSql("DROP EVENT IF EXISTS `eventName`");
			/*
			Assert.AreEqual(1, statements.Count);
			Assert.IsTrue(statements[0] is DropEventStatement);
			DropEventStatement ds = statements[0] as DropEventStatement;
			Assert.AreEqual("`eventName`", ds.EventToDrop.Text);
			Assert.IsTrue(ds.IfExists); */
		}
	}
}
