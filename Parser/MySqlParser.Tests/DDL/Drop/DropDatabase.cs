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
	public class DropDatabase
	{
		[Test]
		public void Simple()
		{
			MySQL51Parser.program_return r = Utility.ParseSql("DROP DATABASE dbname");
			/*
			Assert.AreEqual(1, statements.Count);
			Assert.IsTrue(statements[0] is DropDatabaseStatement);
			DropDatabaseStatement ds = statements[0] as DropDatabaseStatement;
			Assert.AreEqual("dbname", ds.DatabaseToDrop.Text);
			*/
			r = Utility.ParseSql("DROP SCHEMA `schemaname`");
			/*
			Assert.AreEqual(1, statements.Count);
			Assert.IsTrue(statements[0] is DropDatabaseStatement);
			ds = statements[0] as DropDatabaseStatement;
			Assert.AreEqual("`schemaname`", ds.DatabaseToDrop.Text);
			 * */
		}

		[Test]
		public void MissingDbName()
		{
			MySQL51Parser.program_return r = Utility.ParseSql("DROP DATABASE", true);
			/*
			try
			{
				MySqlParser p = new MySqlParser();
				p.Parse("DROP DATABASE");
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
			MySQL51Parser.program_return r = Utility.ParseSql("DROP DATABASE IF EXISTS `dbname`");
			/*
			Assert.AreEqual(1, statements.Count);
			Assert.IsTrue(statements[0] is DropDatabaseStatement);
			DropDatabaseStatement ds = statements[0] as DropDatabaseStatement;
			Assert.AreEqual("`dbname`", ds.DatabaseToDrop.Text);
			Assert.IsTrue(ds.IfExists);
			 */
		}
	}
}
