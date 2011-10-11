using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Antlr.Runtime;
using Antlr.Runtime.Tree;


namespace MySqlParser.Tests
{
	[TestFixture]
	public class Delete
	{
		[Test]
		public void MissingTableDeleteTest()
		{
			MySQL51Parser.statement_list_return r = Utility.ParseSql("delete from", true);			
		}

		[Test]
		public void DangerousDeleteTest()
		{
			MySQL51Parser.statement_list_return r = Utility.ParseSql("delete quick from a");
			/*
			Assert.AreEqual(1, statements.Count);
			Assert.IsTrue(statements[0] is DeleteStatement);
			DeleteStatement ds = statements[0] as DeleteStatement;
			Assert.AreEqual("a", ds.Table.Name.Text);
			Assert.AreEqual(false, ds.Ignore);
			Assert.AreEqual(true, ds.Quick);
			Assert.AreEqual(false, ds.LowPriority);
			 * */
		}

		[Test]
		public void DeleteSimpleTest()
		{
			MySQL51Parser.statement_list_return r = Utility.ParseSql("delete ignore from Table1 where ( Flag is null );");
			/*
			Assert.AreEqual(1, statements.Count);
			Assert.IsTrue(statements[0] is DeleteStatement);
			DeleteStatement ds = statements[0] as DeleteStatement;
			Assert.AreEqual("Table1", ds.Table.Name.Text);
			// Where
			Assert.AreEqual("Flag",
				ds.Where.Term.Reference.Predicate.Expression.Term.Reference.Predicate.BitExprLeft.Column.Name.Text);
			Assert.AreEqual(BooleanExpressionPrimaryOperator.IsNull,
				ds.Where.Term.Reference.Predicate.Expression.Term.Reference.Rest.Operator);
			Assert.AreEqual(true, ds.Ignore);
			Assert.AreEqual(false, ds.Quick);
			Assert.AreEqual(false, ds.LowPriority);
			 * */
		}

		[Test]
		public void DeleteWithClausules()
		{
			MySQL51Parser.statement_list_return r = Utility.ParseSql(
				"delete ignore quick low_priority from Table2 where ( Id <> 1 ) order by Id desc limit 100");
			/*
			Assert.AreEqual(1, statements.Count);
			Assert.IsTrue(statements[0] is DeleteStatement);
			DeleteStatement ds = statements[0] as DeleteStatement;
			Assert.AreEqual("Table2", ds.Table.Name.Text);
			// Where
			Assert.AreEqual("Id",
				ds.Where.Term.Reference.Predicate.Expression.Term.Reference.Predicate.BitExprLeft.Column.Name.Text);
			Assert.AreEqual(BooleanExpressionPrimaryOperator.DistinctThan,
				ds.Where.Term.Reference.Predicate.Expression.Term.Reference.Rest.Operator);
			Assert.AreEqual("1",
				ds.Where.Term.Reference.Predicate.Expression.Term.Reference.Rest.Rest.Predicate.Literal.Value);
			// Order By
			Assert.AreEqual("Id", ds.Order.ExprList[0].Term.Reference.Predicate.BitExprLeft.Column.Name.Text);
			Assert.AreEqual(OrderByDirection.Desc, ds.Order.DirList[0]);
			// Limit
			Assert.AreEqual(ds.Limit.RowCount, 100);
			// keywords
			Assert.AreEqual(true, ds.Ignore);
			Assert.AreEqual(true, ds.Quick);
			Assert.AreEqual(true, ds.LowPriority);
			 * */
		}

		[Test]
		public void DeleteMultiTableTest()
		{
			MySQL51Parser.statement_list_return r = Utility.ParseSql(
				@"delete from Table1, Table2.*, Table3.* using Table1, Table4 inner join Table5
				on Table4.KeyGuid = Table5.ForeignKeyGuid where ( IdKey <> 1 )");
			/*
			Assert.AreEqual(1, statements.Count);
			Assert.IsTrue(statements[0] is DeleteStatement);
			DeleteStatement ds = statements[0] as DeleteStatement;
			// Where
			Assert.AreEqual("IdKey",
				ds.Where.Term.Reference.Predicate.Expression.Term.Reference.Predicate.BitExprLeft.Column.Name.Text);
			Assert.AreEqual(BooleanExpressionPrimaryOperator.DistinctThan,
				ds.Where.Term.Reference.Predicate.Expression.Term.Reference.Rest.Operator);
			Assert.AreEqual("1",
				ds.Where.Term.Reference.Predicate.Expression.Term.Reference.Rest.Rest.Predicate.Literal.Value);
			// MultiTable syntax
			Assert.AreEqual(3, ds.Tables.Count);
			Assert.AreEqual("Table1", ds.Tables[0].Name.Text);
			Assert.AreEqual("Table2", ds.Tables[1].Name.Text);
			Assert.AreEqual("Table3", ds.Tables[2].Name.Text);
			Assert.AreEqual(2, ds.TableReferences.TablesReferences.Count);
			Assert.AreEqual("Table1", ds.TableReferences.TablesReferences[0].Factor.TableName.Name.Text);
			Assert.AreEqual("Table4", ds.TableReferences.TablesReferences[1].Factor.TableName.Name.Text);
			Assert.AreEqual("Table5", ds.TableReferences.TablesReferences[1].Join.Reference.TableName.Name.Text);
			// on clause
			ColumnReference c =
				ds.TableReferences.TablesReferences[1].Join.Conditional.Term.Reference.
				Predicate.BitExprLeft.Column;
			Assert.AreEqual("Table4.KeyGuid", string.Format("{0}.{1}", c.Table.Text, c.Name.Text));
			c = ds.TableReferences.TablesReferences[1].Join.Conditional.Term.Reference.Rest.Rest.
				Predicate.BitExprLeft.Column;
			Assert.AreEqual("Table5.ForeignKeyGuid", string.Format("{0}.{1}", c.Table.Text, c.Name.Text));
			Assert.AreEqual(BooleanExpressionPrimaryOperator.Equal, ds.TableReferences.TablesReferences[1].Join.Conditional.Term.Reference.Rest.Operator);
			 * */
		}

		[Test]
		public void DeleteMultiTableTest2()
		{
			MySQL51Parser.statement_list_return r = Utility.ParseSql(
				@"delete Table1.*, Table2, Table3 from Table1, Table2 inner join Table3 
				on Table2.KeyId = Table3.ForeignKeyId where ( IdKey = 2 )");
			/*
			Assert.AreEqual(1, statements.Count);
			Assert.IsTrue(statements[0] is DeleteStatement);
			DeleteStatement ds = statements[0] as DeleteStatement;
			// Where
			Assert.AreEqual("IdKey",
				ds.Where.Term.Reference.Predicate.Expression.Term.Reference.Predicate.BitExprLeft.Column.Name.Text);
			Assert.AreEqual(BooleanExpressionPrimaryOperator.Equal,
				ds.Where.Term.Reference.Predicate.Expression.Term.Reference.Rest.Operator);
			Assert.AreEqual("2",
				ds.Where.Term.Reference.Predicate.Expression.Term.Reference.Rest.Rest.Predicate.Literal.Value);
			// MultiTable syntax
			Assert.AreEqual(3, ds.Tables.Count);
			Assert.AreEqual("Table1", ds.Tables[0].Name.Text);
			Assert.AreEqual("Table2", ds.Tables[1].Name.Text);
			Assert.AreEqual("Table3", ds.Tables[2].Name.Text);
			Assert.AreEqual(2, ds.TableReferences.TablesReferences.Count);
			Assert.AreEqual("Table1", ds.TableReferences.TablesReferences[0].Factor.TableName.Name.Text);
			Assert.AreEqual("Table2", ds.TableReferences.TablesReferences[1].Factor.TableName.Name.Text);
			Assert.AreEqual("Table3", ds.TableReferences.TablesReferences[1].Join.Reference.TableName.Name.Text);
			// on clause
			ColumnReference c =
				ds.TableReferences.TablesReferences[1].Join.Conditional.Term.Reference.
				Predicate.BitExprLeft.Column;
			Assert.AreEqual("Table2.KeyId", string.Format("{0}.{1}", c.Table.Text, c.Name.Text));
			c = ds.TableReferences.TablesReferences[1].Join.Conditional.Term.Reference.Rest.Rest.
				Predicate.BitExprLeft.Column;
			Assert.AreEqual("Table3.ForeignKeyId", string.Format("{0}.{1}", c.Table.Text, c.Name.Text));
			Assert.AreEqual(BooleanExpressionPrimaryOperator.Equal,
				ds.TableReferences.TablesReferences[1].Join.Conditional.Term.Reference.Rest.Operator);
			 * */
		}

		[Test]
		public void DeleteMultiTableWrongTest()
		{
			// TODO: Check if effectively is the multitable syntax disallowed in combination with order by.
			MySQL51Parser.statement_list_return r = Utility.ParseSql(
				@"delete Table1.*, Table2, Table3 from Table1, Table2 inner join Table3 
				on Table2.KeyId = Table3.ForeignKeyId where ( Id <> 1 ) order by Id desc", true);
			/*
			Assert.AreEqual(1, statements.Count);
			Assert.IsTrue(statements[0] is DeleteStatement);
			DeleteStatement ds = statements[0] as DeleteStatement;
			// Where
			Assert.AreEqual("Id",
				ds.Where.Term.Reference.Predicate.Expression.Term.Reference.Predicate.BitExprLeft.Column.Name.Text);
			Assert.AreEqual(BooleanExpressionPrimaryOperator.DistinctThan,
				ds.Where.Term.Reference.Predicate.Expression.Term.Reference.Rest.Operator);
			Assert.AreEqual("1",
				ds.Where.Term.Reference.Predicate.Expression.Term.Reference.Rest.Rest.Predicate.Literal.Value);
			// MultiTable syntax
			Assert.AreEqual(3, ds.Tables.Count);
			Assert.AreEqual("Table1", ds.Tables[0].Name.Text);
			Assert.AreEqual("Table2", ds.Tables[1].Name.Text);
			Assert.AreEqual("Table3", ds.Tables[2].Name.Text);
			Assert.AreEqual(2, ds.TableReferences.TablesReferences.Count);
			Assert.AreEqual("Table1", ds.TableReferences.TablesReferences[0].Factor.TableName.Name.Text);
			Assert.AreEqual("Table2", ds.TableReferences.TablesReferences[1].Factor.TableName.Name.Text);
			Assert.AreEqual("Table3", ds.TableReferences.TablesReferences[1].Join.Reference.TableName.Name.Text);
			// on clause
			ColumnReference c =
				ds.TableReferences.TablesReferences[1].Join.Conditional.Term.Reference.Predicate.BitExprLeft.Column;
			Assert.AreEqual("Table2.KeyId", string.Format("{0}.{1}", c.Table.Text, c.Name.Text));
			c = ds.TableReferences.TablesReferences[1].Join.Conditional.Term.Reference.Predicate.BitExprRight.Column;
			Assert.AreEqual("Table3.ForeignKeyId", string.Format("{0}.{1}", c.Table.Text, c.Name.Text));
			Assert.AreEqual(BooleanExpressionPrimaryOperator.Equal,
				ds.TableReferences.TablesReferences[1].Join.Conditional.Term.Reference.Rest.Operator);			
			*/
		}

		[Test]
		public void DeleteMultiTableWrongTest2()
		{
			MySQL51Parser.statement_list_return r = Utility.ParseSql(
				@"delete Table1.*, Table2, Table3 from Table1, Table2 inner join Table3 
				on Table2.KeyId = Table3.ForeignKeyId where ( Id <> 1 ) limit 1000", true);
			/*
			Assert.AreEqual(1, statements.Count);
			Assert.IsTrue(statements[0] is DeleteStatement);
			DeleteStatement ds = statements[0] as DeleteStatement;
			// Where
			Assert.AreEqual("IdKey",
				ds.Where.Term.Reference.Predicate.Expression.Term.Reference.Predicate.BitExprLeft.Column.Name.Text);
			Assert.AreEqual(BooleanExpressionPrimaryOperator.Equal,
				ds.Where.Term.Reference.Predicate.Expression.Term.Reference.Rest.Operator);
			Assert.AreEqual("2",
				ds.Where.Term.Reference.Predicate.Expression.Term.Reference.Rest.Rest.Predicate.Literal.Value);
			// MultiTable syntax
			Assert.AreEqual(3, ds.Tables.Count);
			Assert.AreEqual("Table1", ds.Tables[0].Name.Text);
			Assert.AreEqual("Table2", ds.Tables[1].Name.Text);
			Assert.AreEqual("Table3", ds.Tables[2].Name.Text);
			Assert.AreEqual(2, ds.TableReferences.TablesReferences.Count);
			Assert.AreEqual("Table1", ds.TableReferences.TablesReferences[0].Factor.TableName.Name.Text);
			Assert.AreEqual("Table2", ds.TableReferences.TablesReferences[1].Factor.TableName.Name.Text);
			Assert.AreEqual("Table3", ds.TableReferences.TablesReferences[1].Join.Reference.TableName.Name.Text);
			// on clause
			ColumnReference c =
				ds.TableReferences.TablesReferences[1].Join.Conditional.Term.Reference.Predicate.BitExprLeft.Column;
			Assert.AreEqual("Table2.KeyId", string.Format("{0}.{1}", c.Table.Text, c.Name.Text));
			c = ds.TableReferences.TablesReferences[1].Join.Conditional.Term.Reference.Predicate.BitExprRight.Column;
			Assert.AreEqual("Table3.ForeignKeyId", string.Format("{0}.{1}", c.Table.Text, c.Name.Text));
			Assert.AreEqual(BooleanExpressionPrimaryOperator.Equal,
				ds.TableReferences.TablesReferences[1].Join.Conditional.Term.Reference.Rest.Operator);
			 * */
		}
	}
}
