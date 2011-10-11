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
	public class DropRoutine
	{		
		[Test]
		public void SimpleNoSchema()
		{			
			MySQL51Parser.statement_list_return r = Utility.ParseSql("DROP PROCEDURE procname");
			/*
			Assert.AreEqual(1, statements.Count);
			Assert.IsTrue(statements[0] is DropRoutineStatement);
			DropRoutineStatement ds = statements[0] as DropRoutineStatement;
			Assert.IsNull(ds.RoutineToDrop.Database);
			Assert.AreEqual("procname", ds.RoutineToDrop.Name.Text);
			 * */

			r = Utility.ParseSql("DROP FUNCTION `funcname`");
			/*
			Assert.AreEqual(1, statements.Count);
			Assert.IsTrue(statements[0] is DropRoutineStatement);
			ds = statements[0] as DropRoutineStatement;
			Assert.IsNull(ds.RoutineToDrop.Database);
			Assert.AreEqual("`funcname`", ds.RoutineToDrop.Name.Text);
			 * */
		}

		[Test]
		public void Simple()
		{			
			MySQL51Parser.statement_list_return r = Utility.ParseSql("DROP PROCEDURE schema1.procname");
			/*
			Assert.AreEqual(1, statements.Count);
			Assert.IsTrue(statements[0] is DropRoutineStatement);
			DropRoutineStatement ds = statements[0] as DropRoutineStatement;
			Assert.AreEqual("schema1", ds.RoutineToDrop.Database.Text);
			Assert.AreEqual("procname", ds.RoutineToDrop.Name.Text);
			 * */

			r = Utility.ParseSql("DROP FUNCTION `schema1`.`funcname`");
			/*
			Assert.AreEqual(1, statements.Count);
			Assert.IsTrue(statements[0] is DropRoutineStatement);
			ds = statements[0] as DropRoutineStatement;
			Assert.AreEqual("`schema1`", ds.RoutineToDrop.Database.Text);
			Assert.AreEqual("`funcname`", ds.RoutineToDrop.Name.Text);
			 * */
		}

		[Test]
		public void MissingRoutineName()
		{
			MySQL51Parser.statement_list_return r = Utility.ParseSql("DROP PROCEDURE", true);
		}

		[Test]
		public void IfExists()
		{			
			MySQL51Parser.statement_list_return r = Utility.ParseSql("DROP PROCEDURE IF EXISTS `procname`");
			/*
			Assert.AreEqual(1, statements.Count);
			Assert.IsTrue(statements[0] is DropRoutineStatement);
			DropRoutineStatement ds = statements[0] as DropRoutineStatement;
			Assert.AreEqual("`procname`", ds.RoutineToDrop.Name.Text);
			Assert.IsTrue(ds.IfExists);*/
		}
	}
}
