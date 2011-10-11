using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Antlr.Runtime;
using Antlr.Runtime.Tree;

namespace MySqlParser.Tests
{
	[TestFixture]
	public class Select
	{
		[Test]
		public void SelectSimple()
		{
			MySQL51Parser.statement_list_return r = Utility.ParseSql("select * from `t`");
			CommonTree ct = r.Tree as CommonTree;
			Assert.AreEqual( "SELECT", ct.Text );
			Assert.AreEqual( 2, ct.ChildCount );
			Assert.AreEqual( "FROM", ct.Children[ 1 ].Text );
			Assert.AreEqual( 1, ct.Children[ 1 ].ChildCount );
			Assert.AreEqual( "TABLE", ct.Children[ 1 ].GetChild( 0 ).Text );
			Assert.AreEqual( 1, ct.Children[ 1 ].GetChild( 0 ).ChildCount );
			Assert.AreEqual( "`T`", ct.Children[ 1 ].GetChild( 0 ).GetChild( 0 ).Text );			
			Assert.AreEqual( "COLUMNS", ct.Children[ 0 ].Text );
			Assert.AreEqual( 1, ct.Children[ 0 ].ChildCount );
			Assert.AreEqual( "SELECT_EXPR", ct.Children[ 0 ].GetChild( 0 ).Text );
			Assert.AreEqual( 1, ct.Children[ 0 ].GetChild( 0 ).ChildCount );
			Assert.AreEqual( "*", ct.Children[ 0 ].GetChild( 0 ).GetChild( 0 ).Text );
		}

		[Test]
		public void UnionTest()
		{
			Utility.ParseSql(@"(SELECT a FROM t1 WHERE a=10 AND B=1)
				UNION
				(SELECT a FROM t2 WHERE a=11 AND B=2)
				union all ( select 1, 2, 3, t.* from t1 straight_join t2 )
				");
		}

		[Test]
		public void CompoundOperatorsTest()
		{
			Utility.ParseSql("select a from t where ( a <= b )");
		}

		[Test]
		public void LimitTest()
		{
			Utility.ParseSql("select a, b, c from Table1 limit 100");
			Utility.ParseSql("select a, b, c from Table1 limit 5000, 6000");
			Utility.ParseSql("select a, b, c from Table1 where ( a = 1 ) limit 200 offset 100");
		}

		[Test]
		public void OrderByTest()
		{
			Utility.ParseSql( "select a, b, c from Table1 order by c asc, b desc, 2 asc" );
			Utility.ParseSql("select a, b, c from Table1 order by null");
		}

		[Test]
		public void WhereTestBroken()
		{
			Utility.ParseSql("select a from", true);
		}

		[Test]
		public void WhereBetweenTest()
		{
			Utility.ParseSql("select a from `table` where `a` between 1 and 2");
		}

		[Test]
		public void WhereBetweenTest2()
		{
			Utility.ParseSql("select `a` from `table` where ( `a` between 1 and 2 )");
		}

		[Test]
		public void JoinTest()
		{
			Utility.ParseSql("select `a`, `b`, `c` from `tabA` inner join `tabB` on `tabA`.`KeyId` = `tabB`.`ForeignKeyId`;");
		}

		[Test]
		public void StarSimpleTest()
		{
			Utility.ParseSql("select * from TabA");
		}

		[Test]
		public void StarComplexTest()
		{
			Utility.ParseSql(@"select t1.*, t2.*, t3.*, * from t1 inner join t2 on t1.Id = t2.ParentId 
				  inner join t3 on t2.Id = t3.ParentId ");
		}

		[Test]
		public void SimpleCrossJoinWithoutOnTest()
		{
			Utility.ParseSql("select * from t1 cross join t2");
		}

		[Test]
		public void SimpleCrossJoinTest()
		{
			Utility.ParseSql("select * from t1 cross join t2 on t1.col1 = t2.col2");
		}

		[Test]
		public void SimpleInnerJoinWithoutOnTest()
		{
			Utility.ParseSql("select * from t1 inner join t2");
		}

		[Test]
		public void SimpleInnerJoinTest()
		{
			Utility.ParseSql("select * from t1 inner join t2 on t1.col1 = t2.col2");
		}

		[Test]
		public void SimpleStraightJoinWithoutOnTest()
		{
			Utility.ParseSql("select * from t1 straight_join t2");
		}

		[Test]
		public void SimpleStraightJoinTest()
		{
			Utility.ParseSql("select * from t1 straight_join t2 on t1.col1 = t2.col2");
		}

		[Test]
		public void SimpleLeftJoinTest()
		{
			Utility.ParseSql("select * from t1 left join t2 on t1.col1 = t2.col2");
		}

		[Test]
		public void SimpleRightJoinTest()
		{
			Utility.ParseSql("select * from t1 right join t2 on t1.col1 = t2.col2");
		}

		[Test]
		public void SimpleLeftOuterJoinTest()
		{
			Utility.ParseSql("select * from t1 left outer join t2 on t1.col1 = t2.col2");
		}

		[Test]
		public void SimpleRightOuterJoinTest()
		{
			Utility.ParseSql("select * from t1 right outer join t2 on t1.col1 = t2.col2");
		}

		[Test]
		public void SimpleNaturalJoinTest()
		{
			Utility.ParseSql("select * from t1 natural join t2");
		}

		[Test]
		public void SimpleNaturalLeftJoinTest()
		{
			Utility.ParseSql("select * from t1 natural left join t2");
		}

		[Test]
		public void SimpleNaturalLeftOuterJoinTest()
		{
			Utility.ParseSql("select * from t1 natural left outer join t2");
		}

		[Test]
		public void SimpleNaturalRightJoinTest()
		{
			Utility.ParseSql("select * from t1 natural right join t2");
		}

		[Test]
		public void SimpleNaturalRightOuterJoinTest()
		{
			Utility.ParseSql("select * from t1 natural right outer join t2");
		}

		[Test]
		public void SimpleInnerJoinSimplestOnConditionalTest()
		{
			Utility.ParseSql("select * from t1 inner join t2 on true");
		}

		[Test]
		public void MissingOnClausuleForJoinsTest()
		{
			Utility.ParseSql("select * from t1 left join t2", true);
			Utility.ParseSql("select * from t1 left join t2 ", true);
			Utility.ParseSql("select * from t1 right join t2 ", true);
			Utility.ParseSql("select * from t1 left outer join t2", true);
			Utility.ParseSql("select * from t1 right outer join t2", true);
		}

		[Test]
		public void WithoutFromTestTest()
		{
			Utility.ParseSql("select 1, 2, 3");
		}

		[Test]
		public void OdbcJoinTest()
		{
			Utility.ParseSql("select * from { oj TabA left outer join TabB on TabA.ID = TabB.ID }");
		}

		[Test]
		public void LikeConditionTest()
		{
			Utility.ParseSql("select * from t where a like 'ab'");
		}

		[Test]
		public void T()
		{
			Utility.ParseSql("select * from mysql.user limit 2;");
			//Utility.ParseSql("select * from t where a between b");
		}


		/*
		 * TODO:
		 * Since it is legal ( select * from t ) limit 10, but illegal
		 * ((select * from t ) limit 20 ) limit 30
		 * This might need a semantic predicate to be resolved.
		 * */
		/*
		[Test]
		public void NestedLimitTest()
		{
			Utility.ParseSql("(SELECT * from T LIMIT 1) LIMIT 2;");
		}
		 * */
	}
}
