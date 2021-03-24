// Copyright (c) 2014, 2021, Oracle and/or its affiliates.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License, version 2.0, as
// published by the Free Software Foundation.
//
// This program is also distributed with certain software (including
// but not limited to OpenSSL) that is licensed under separate terms,
// as designated in a particular file or component or in included license
// documentation.  The authors of MySQL hereby grant you an
// additional permission to link the program and your derivative works
// with the separately licensed software that they have included with
// MySQL.
//
// Without limiting anything contained in the foregoing, this file,
// which is part of MySQL for Visual Studio, is also subject to the
// Universal FOSS Exception, version 1.0, a copy of which can be found at
// http://oss.oracle.com/licenses/universal-foss-exception.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU General Public License, version 2.0, for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using System;
using System.Collections.Generic;
using System.Data.Common;
#if CLR4
using System.Linq;
#endif
using System.IO;
using System.Text;
using MySql.Data.MySqlClient;
using MySql.Parser;
using Antlr.Runtime;
using Antlr.Runtime.Tree;

namespace MySql.Data.VisualStudio
{
  internal static class LanguageServiceUtil
  {
    // Cache of parsed text.
    // This cache is organized by input text then by server version (for each combination of the previous two,
    // returns a CommonTokenStream).
    private static Dictionary<string,  Dictionary<Version, TokenStreamRemovable>> _parserCache =
      new Dictionary<string, Dictionary<Version, TokenStreamRemovable>>();

    private static Dictionary<string, string> _keywords4ResultSets;

    static LanguageServiceUtil()
    {
      _keywords4ResultSets = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase );
      _keywords4ResultSets.Add("analyze", "");
      _keywords4ResultSets.Add("check", "");
      _keywords4ResultSets.Add("checksum", "");
      _keywords4ResultSets.Add("describe", "");
      _keywords4ResultSets.Add("explain", "");
      _keywords4ResultSets.Add("explain_stmt", "");
      _keywords4ResultSets.Add("optimize", "");
      _keywords4ResultSets.Add("repair", "");
      _keywords4ResultSets.Add("select", "");
      _keywords4ResultSets.Add("show", "");
    }

    public static TokenStreamRemovable GetTokenStream(string sql, Version version)
    {
      Dictionary<Version, TokenStreamRemovable> lines = null;
      if (_parserCache.TryGetValue(sql, out lines))
      {
        TokenStreamRemovable tsr = null;
        if( lines.TryGetValue( version, out tsr ) )
          return tsr;
      }
      // no cache entry, then parse
      MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(sql));//ASCIIEncoding.ASCII.GetBytes(sql));
      CaseInsensitiveInputStream input = new CaseInsensitiveInputStream(ms);
      MySQLLexer lexer = new MySQLLexer(input);
      lexer.MySqlVersion = version;
      TokenStreamRemovable tokens = new TokenStreamRemovable(lexer);

      if (lines == null)
      {
        lines = new Dictionary<Version, TokenStreamRemovable>();        
        _parserCache.Add(sql, lines);
      }
      lines.Add(version, tokens);
      return tokens;
    }

    internal static AstParserRuleReturnScope<object, IToken> ParseSql(string sql)
    {
      return ParseSql(sql, false);
    }

    internal static AstParserRuleReturnScope<object, IToken> ParseSql(string sql, bool expectErrors, out StringBuilder sb)
    {
      CommonTokenStream ts;
      return ParseSql(sql, expectErrors, out sb, out ts);
    }

    internal static AstParserRuleReturnScope<object, IToken> ParseSql(string sql, bool expectErrors, out StringBuilder sb, string version)
    {
      Version ver = GetVersion(version);
      CommonTokenStream ts;
      return ParseSql(sql, expectErrors, out sb, out ts, ver);
    }

    internal static AstParserRuleReturnScope<object, IToken> ParseSql(
      string sql, bool expectErrors, out StringBuilder sb, out CommonTokenStream tokensOutput, Version version)
    {
      sql = sql.TrimStart();
      MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(sql));//ASCIIEncoding.ASCII.GetBytes(sql));
      CaseInsensitiveInputStream input = new CaseInsensitiveInputStream(ms);
      //ANTLRInputStream input = new ANTLRInputStream(ms);
      MySQLLexer lexer = new MySQLLexer(input);
      lexer.MySqlVersion = version;
      CommonTokenStream tokens = new CommonTokenStream(lexer);
      tokensOutput = tokens;
      return DoParse(tokens, expectErrors, out sb, lexer.MySqlVersion);
    }

    internal static AstParserRuleReturnScope<object, IToken> ParseSql(
      string sql, bool expectErrors, out StringBuilder sb, CommonTokenStream tokens)
    {
      DbConnection con = GetConnection();
      return DoParse( tokens, expectErrors, out sb, GetVersion( con.ServerVersion ));
    }

    internal static AstParserRuleReturnScope<object, IToken> ParseSql(
      string sql, bool expectErrors, out StringBuilder sb, out CommonTokenStream tokensOutput)
    {
      sql = sql.TrimStart();
      DbConnection con = GetConnection();
      MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(sql));//ASCIIEncoding.ASCII.GetBytes(sql));
      CaseInsensitiveInputStream input = new CaseInsensitiveInputStream(ms);
      //ANTLRInputStream input = new ANTLRInputStream(ms);
      MySQLLexer lexer = new MySQLLexer(input);
      lexer.MySqlVersion = GetVersion( con.ServerVersion );
      CommonTokenStream tokens = new CommonTokenStream(lexer);
      tokensOutput = tokens;
      return DoParse(tokens, expectErrors, out sb, lexer.MySqlVersion);
    }

    private static AstParserRuleReturnScope<object, IToken> DoParse( 
      CommonTokenStream tokens, bool expectErrors, out StringBuilder sb, Version version )
    {
      MySQLParser parser = new MySQLParser(tokens);
      parser.MySqlVersion = version;
      sb = new StringBuilder();
      TextWriter tw = new StringWriter(sb);
      parser.TraceDestination = tw;
      AstParserRuleReturnScope<object, IToken> r = null;
      int tokCount = tokens.Count;
      try
      {
        r = parser.program();
      }
      catch (RewriteEmptyStreamException e)
      {
        if (!expectErrors)
        {
          sb.AppendLine();
          sb.Append(e.Message);
        }
      }
      /*
       * The parser inserts a new <EOF> to the token stream, this is probably an ANTLR bug, the code here takes care of it.
       * */
      if (( tokens.Count - 1 ) == tokCount)
      {
        TokenStreamRemovable tsr = (TokenStreamRemovable)tokens;
        tsr.Remove(tokens.Get(tokens.Count - 1));
      }
      tokens.Reset();
      return r;
    }

    private static AstParserRuleReturnScope<object, IToken> ParseSql(string sql, bool expectErrors)
    {
      StringBuilder sb;
      CommonTokenStream ts;
      return ParseSql(sql, expectErrors, out sb, out ts);
    }

    internal static Version GetVersion( string versionString )
    {
      Version version;
      int i = 0;
      while (i < versionString.Length &&
          (Char.IsDigit(versionString[i]) || versionString[i] == '.'))
        i++;
      version = new Version(versionString.Substring(0, i));
      return version;
    }

    /// <summary>
    /// Gets MySql server version from the connection string (useful for recognize the exact syntax for that version in
    /// the parser's grammar and scanner).
    /// </summary>
    /// <returns></returns>
    internal static Version GetVersion()
    {
      DbConnection con = GetConnection();
      if (con != null)
      {
        return GetVersion(con.ServerVersion);
      }
      else
      {
        // default to server 5.1
        return new Version(5, 1);
      }
    }

    public static DbConnection GetConnection()
    {
      DbConnection connection = DocumentNode.GetCurrentConnection();
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

    public static string GetCurrentDatabase()
    {
      Editors.EditorBroker broker = MySql.Data.VisualStudio.Editors.EditorBroker.Broker;
      if (broker != null)
      {
        return broker.GetCurrentDatabase();
      }
      return null;
    }

    public static string GetRoutineName(string sql)
    {
      StringBuilder sb;
      AstParserRuleReturnScope<object, IToken> pr = LanguageServiceUtil.ParseSql(sql, false, out sb);
      if (sb.Length != 0)
      {
        throw new ApplicationException(string.Format("Syntactic error in stored routine: {0}", sb.ToString()));
      }
      else
      {
        CommonTree t = (CommonTree)pr.Tree;
        if (t.IsNil)
          t = (CommonTree)t.GetChild(0);
        string name;
        if (string.Equals(t.GetChild(1).Text, "definer", StringComparison.OrdinalIgnoreCase))
          name = t.GetChild(3).Text;
        else
          name = t.GetChild(1).Text;
        return name.Replace("`", "");
      }
    }

    /// <summary>
    /// Checks if the first SQL statement returns a result set. 
    /// </summary>
    /// <param name="sql">The SQL statements to parse.</param>
    /// <param name="connection">The connection used to get the server version.</param>
    /// <returns><c>null</c> if the an error ocurred parsing the SQL statements.
    /// <c>true</c> if the first SQL statement in the string returns a result set; otherwise, <c>false</c>.</returns>
    public static bool? DoesStmtReturnResults(string sql, MySqlConnection connection, out StringBuilder builder)
    {
      AstParserRuleReturnScope<object, IToken> programReturn = ParseSql(sql, false, out builder, connection.ServerVersion);
      if (programReturn == null)
      {
        return null;
      }

      ITree tree = programReturn.Tree as ITree;
      if (tree.IsNil)
      {
        tree = tree.GetChild(0);
      }

      return _keywords4ResultSets.ContainsKey(tree.Text);
    }
  }
}
