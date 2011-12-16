using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using NUnit.Framework;
using System.IO;

namespace MySql.Parser.Tests
{
  public static class Utility
  {
    public static MySQL51Parser.program_return ParseSql(string sql, bool expectErrors, out StringBuilder sb)
    {
      // The grammar supports upper case only
      MemoryStream ms = new MemoryStream(ASCIIEncoding.ASCII.GetBytes(sql/*.ToUpper() */));
      CaseInsensitiveInputStream input = new CaseInsensitiveInputStream(ms);
      //ANTLRInputStream input = new ANTLRInputStream(ms);
      MySQL51Lexer lexer = new MySQL51Lexer(input);
      CommonTokenStream tokens = new CommonTokenStream(lexer);
      MySQL51Parser parser = new MySQL51Parser(tokens);
      sb = new StringBuilder();
      TextWriter tw = new StringWriter(sb);
      parser.TraceDestination = tw;
      MySQL51Parser.program_return r = parser.program();
      if (!expectErrors)
      {
        if (0 != parser.NumberOfSyntaxErrors)
          Assert.AreEqual("", sb.ToString());
        //Assert.AreEqual( 0, parser.NumberOfSyntaxErrors);
      }
      else
      {
        Assert.AreNotEqual(0, parser.NumberOfSyntaxErrors);
      }
      return r;
    }

    public static MySQL51Parser.program_return ParseSql(string sql, bool expectErrors)
    {
      StringBuilder sb;
      return ParseSql(sql, expectErrors, out sb);
    }

    public static MySQL51Parser.program_return ParseSql(string sql)
	{
		return ParseSql(sql, false);
	}
  }
}
