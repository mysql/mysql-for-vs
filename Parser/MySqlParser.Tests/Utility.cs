using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using NUnit.Framework;
using System.IO;

namespace MySqlParser.Tests
{
	public static class Utility
	{
		public static MySQL51Parser.statement_list_return ParseSql(string sql, bool expectErrors)
		{
			// The grammar supports upper case only
			MemoryStream ms = new MemoryStream(ASCIIEncoding.ASCII.GetBytes(sql.ToUpper()));
			ANTLRInputStream input = new ANTLRInputStream(ms);
			MySQL51Lexer lexer = new MySQL51Lexer(input);
			CommonTokenStream tokens = new CommonTokenStream(lexer);
			MySQL51Parser parser = new MySQL51Parser(tokens);
			StringBuilder sb = new StringBuilder();
			TextWriter tw = new StringWriter(sb);
			parser.TraceDestination = tw;
			MySQL51Parser.statement_list_return r = parser.statement_list();
			if (!expectErrors)
			{
				if( 0 != parser.NumberOfSyntaxErrors )
					Assert.AreEqual("", sb.ToString());
				//Assert.AreEqual( 0, parser.NumberOfSyntaxErrors);
			}
			else
			{
				Assert.AreNotEqual(0, parser.NumberOfSyntaxErrors);
			}
			return r;
		}

		public static MySQL51Parser.statement_list_return ParseSql(string sql)
		{
			return ParseSql(sql, false);
		}
	}
}
