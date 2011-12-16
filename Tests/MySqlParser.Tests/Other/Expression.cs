using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Antlr.Runtime;
using Antlr.Runtime.Tree;


namespace MySql.Parser.Tests.Other
{
	[TestFixture]
	public class Expression
	{
		[Test]
		public void Sum()
		{
			MySQL51Parser.program_return r = Utility.ParseSql("select ( a + b )");
		}

		[Test]
		public void CaseSimple()
		{
			MySQL51Parser.program_return r = Utility.ParseSql(
				"select CASE WHEN 1 THEN 'one' WHEN 2 THEN 'two' END;");
		}

		[Test]
		public void CaseSimpleWithElse()
		{
			MySQL51Parser.program_return r = Utility.ParseSql(
				"select CASE WHEN 1 THEN 'one' WHEN 2 THEN 'two' ELSE 'more' END;");			
		}

		[Test]
		public void IfSimple()
		{
			MySQL51Parser.program_return r = Utility.ParseSql(
				"SELECT IF(1<2,'yes','no');");
		}

		[Test]
		public void IfNull()
		{
			MySQL51Parser.program_return r = Utility.ParseSql(
				"SELECT IFNULL(1/0,10);");
		}

		[Test]
		public void NullIf()
		{
			MySQL51Parser.program_return r = Utility.ParseSql(
				"SELECT NULLIF(1,2);");
		}

		[Test]
		public void ParameterMarker()
		{
			// SET @myvar = 5;
			MySQL51Parser.program_return r = Utility.ParseSql(@"				
				SELECT @myvar, id FROM MyTable WHERE id >= @maxId");
          //SELECT @myvar, id FROM MyTable WHERE id >= ?maxId");
		}
	}
}
