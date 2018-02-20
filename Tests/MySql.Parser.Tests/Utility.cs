// Copyright (c) 2014, Oracle and/or its affiliates. All rights reserved.
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
using System.Linq;
using System.Text;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using System.IO;
using MySql.Parser;
using Xunit;

namespace MySql.Parser.Tests
{
  public static class Utility
  {
    public static MySQL51Parser.program_return ParseSql(string sql, bool expectErrors, out StringBuilder sb)
    {
      return ParseSql(sql, expectErrors, out sb, new Version(5, 1));
    }

    public static MySQL51Parser.query_return ParseSqlQuery(string sql, bool expectErrors, out StringBuilder sb)
    {
      return ParseSqlQuery(sql, expectErrors, out sb, new Version(5, 1));
    }

    public static MySQL51Parser.query_return ParseSqlQuery(string sql, bool expectErrors, out StringBuilder sb, Version version)
    {
      MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(sql));//ASCIIEncoding.ASCII.GetBytes(sql/*.ToUpper() */));
      CaseInsensitiveInputStream input = new CaseInsensitiveInputStream(ms);
      MySQLLexer lexer = new MySQLLexer(input);
      lexer.MySqlVersion = version;
      StringBuilder sbLex = new StringBuilder();
      lexer.TraceDestination = new StringWriter(sbLex);
      CommonTokenStream tokens = new CommonTokenStream(lexer);
      MySQLParser parser = new MySQLParser(tokens);
      parser.MySqlVersion = version;
      sb = new StringBuilder();
      TextWriter tw = new StringWriter(sb);
      parser.TraceDestination = tw;
      MySQL51Parser.query_return r = parser.query();
      if (!expectErrors)
      {
        if (0 != parser.NumberOfSyntaxErrors)
          Assert.Equal("", sb.ToString());
        Assert.Equal("", sbLex.ToString());
      }
      else
      {
        Assert.NotEqual(0, parser.NumberOfSyntaxErrors);
      }
      return r;
    }

    public static MySQL51Parser.program_return ParseSql(string sql, bool expectErrors, out StringBuilder sb, Version version )
    {
      MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(sql));//ASCIIEncoding.ASCII.GetBytes(sql/*.ToUpper() */));
      CaseInsensitiveInputStream input = new CaseInsensitiveInputStream(ms);
      MySQLLexer lexer = new MySQLLexer(input);
      lexer.MySqlVersion = version;
      StringBuilder sbLex = new StringBuilder();
      lexer.TraceDestination = new StringWriter(sbLex);
      CommonTokenStream tokens = new CommonTokenStream(lexer);
      MySQLParser parser = new MySQLParser(tokens);
      parser.MySqlVersion = version;
      sb = new StringBuilder();
      TextWriter tw = new StringWriter(sb);
      parser.TraceDestination = tw;
      MySQL51Parser.program_return r = parser.program();
      if (!expectErrors)
      {
        if (0 != parser.NumberOfSyntaxErrors)
          Assert.Equal("", sb.ToString());
        Assert.Equal("", sbLex.ToString());
      }
      else
      {
        Assert.NotEqual(0, parser.NumberOfSyntaxErrors);
      }
      return r;
    }

    public static MySQL51Parser.program_return ParseSql(string sql, bool expectErrors, Version version)
    {
      StringBuilder sb;
      return ParseSql(sql, expectErrors, out sb, version);
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
