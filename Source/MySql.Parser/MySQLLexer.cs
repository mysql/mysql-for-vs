// Copyright (c) 2013, 2021, Oracle and/or its affiliates.
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
using Antlr.Runtime;

namespace MySql.Parser
{
  /// <summary>
  /// Abstract superclass for MySQL Lexers, for now containing some common code, so it's not in the grammar.
  /// Author: kroepke
  /// </summary>
  public abstract class MySQLLexerBase : Lexer
  {
    bool nextTokenIsID = false;

    //private ErrorListener errorListener;

    /**
     * Check ahead in the input stream for a left paren to distinguish between built-in functions
     * and identifiers.
     * TODO: This is the place to support certain SQL modes.
     * @param proposedType the original token type for this input.
     * @return the new token type to emit a token with
     */
    public virtual int checkFunctionAsID(int proposedType)
    {
      return proposedType;
    }

    public virtual int checkFunctionAsNotId(int proposedType)
    {
      return proposedType;
    }

    public int checkFunctionAsID(int proposedType, int alternativeProposedType)
    {
      return (input.LA(1) != '(') ? alternativeProposedType : proposedType;
    }

    /// <summary>
    /// This functions allows certain keywords to be used as identifiers in a version before they were recognized as keywords.
    /// </summary>
    /// <param name="versionString">A string representing the version to check.</param>
    /// <param name="proposedType">The proposed token type.</param>
    /// <param name="alternativeProposedType">The alternative proposed token type.</param>
    /// <returns></returns>
    public int checkIDperVersion(string versionString, int proposedType, int alternativeProposedType)
    {
      var version = new Version(versionString);
      return (MySqlVersion >= version) ? proposedType : alternativeProposedType;
    }

    public int checkFunctionasIDperVersion(string versionString, int proposedType, int alternativeProposedType)
    {
      var version = new Version(versionString);
      if (MySqlVersion < version) return alternativeProposedType;
      else return (input.LA(1) != '(') ? proposedType : alternativeProposedType;
    }

    // Holds values like 5.6, 5.7, 8.0, etc.
    protected string _mysqlVersion;

    public Version MySqlVersion
    {
      get { return new Version(_mysqlVersion); }
      set { _mysqlVersion = value.ToString(); }
    }

    public MySQLLexerBase() { }

    public MySQLLexerBase(ICharStream input, RecognizerSharedState state)
      : base(input, state)
    { }
  }

  public class MySQLLexer : MySQL51Lexer
  {
    public MySQLLexer() { }

    public MySQLLexer(ICharStream input)
      : base(input)
    { }

    public MySQLLexer(ICharStream input, RecognizerSharedState state)
      : base(input, state)
    { }

    public override int checkFunctionAsID(int proposedType)
    {
      return (input.LA(1) == '(') ? ID : proposedType;
    }

    public override int checkFunctionAsNotId(int proposedType)
    {
      return (input.LA(1) != '(') ? ID : proposedType;
    }
  }
}
