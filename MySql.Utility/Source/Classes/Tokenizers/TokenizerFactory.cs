// Copyright (c) 2016, Oracle and/or its affiliates. All rights reserved.
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License as
// published by the Free Software Foundation; version 2 of the
// License.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301  USA

using MySql.Utility.Enums;
using MySql.Utility.Interfaces;

namespace MySql.Utility.Classes.Tokenizers
{
  /// <summary>
  /// Defines a factory for <see cref="ITokenizer"/> based classes.
  /// </summary>
  public static class TokenizerFactory
  {
    /// <summary>
    /// Gets a <see cref="ITokenizer"/> class instance corresponding to the given language.
    /// </summary>
    /// <param name="language">A <see cref="ScriptLanguageType"/> value.</param>
    /// <param name="script">A script to initialize the tokenizer.</param>
    /// <returns>A <see cref="ITokenizer"/> class instance corresponding to the given language.</returns>
    public static ITokenizer GetTokenizer(ScriptLanguageType language, string script)
    {
      switch (language)
      {
        case ScriptLanguageType.JavaScript:
          return new MyJsTokenizer(script);

        case ScriptLanguageType.Python:
          return new MyPythonTokenizer(script);

        case ScriptLanguageType.Sql:
          return new MySqlTokenizer(script);

        default:
          return null;
      }
    }
  }
}
