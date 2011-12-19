using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.IO;
using System.Text;
using MySql.Parser;
using Antlr.Runtime;
using Antlr.Runtime.Tree;

namespace MySql.Data.VisualStudio
{
  internal static class LanguageServiceUtil
  {
    internal static MySQL51Parser.program_return ParseSql(string sql)
    {
      return ParseSql(sql, false);
    }

    internal static MySQL51Parser.program_return ParseSql(string sql, bool expectErrors, out StringBuilder sb)
    {
      CommonTokenStream ts;
      return ParseSql(sql, expectErrors, out sb, out ts);
    }

    internal static MySQL51Parser.program_return ParseSql(
      string sql, bool expectErrors, out StringBuilder sb, CommonTokenStream tokens)
    {
      return DoParse( tokens, expectErrors, out sb);
    }

    internal static MySQL51Parser.program_return ParseSql(
      string sql, bool expectErrors, out StringBuilder sb, out CommonTokenStream tokensOutput)
    {
      // The grammar supports upper case only
      MemoryStream ms = new MemoryStream(ASCIIEncoding.ASCII.GetBytes(sql));
      CaseInsensitiveInputStream input = new CaseInsensitiveInputStream(ms);
      //ANTLRInputStream input = new ANTLRInputStream(ms);
      MySQL51Lexer lexer = new MySQL51Lexer(input);
      CommonTokenStream tokens = new CommonTokenStream(lexer);
      tokensOutput = tokens;
      return DoParse(tokens, expectErrors, out sb);
    }

    private static MySQL51Parser.program_return DoParse( 
      CommonTokenStream tokens, bool expectErrors, out StringBuilder sb )
    {
      MySQL51Parser parser = new MySQL51Parser(tokens);
      sb = new StringBuilder();
      TextWriter tw = new StringWriter(sb);
      parser.TraceDestination = tw;
      MySQL51Parser.program_return r = parser.program();
      if (!expectErrors)
      {
        //if (0 != parser.NumberOfSyntaxErrors)
        //  Assert.AreEqual("", sb.ToString());
        //Assert.AreEqual( 0, parser.NumberOfSyntaxErrors);
      }
      else
      {
        //Assert.AreNotEqual(0, parser.NumberOfSyntaxErrors);
      }
      return r;
    }

    private static MySQL51Parser.program_return ParseSql(string sql, bool expectErrors)
    {
      StringBuilder sb;
      CommonTokenStream ts;
      return ParseSql(sql, expectErrors, out sb, out ts);
    }

    public static DbConnection GetConnection()
    {
      DbConnection connection = StoredProcedureNode.GetCurrentConnection();
      if (connection == null)
      {
        Editors.EditorBroker broker = MySql.Data.VisualStudio.Editors.EditorBroker.Broker;
        if (broker != null)
        {
          connection = broker.GetCurrentConnection();
        }
      }
      return connection;
    }
  }
}
