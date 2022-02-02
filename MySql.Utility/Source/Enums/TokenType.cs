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

namespace MySql.Utility.Enums
{
  /// <summary>
  /// Specifies identifiers to indicate the type of a token.
  /// </summary>
  public enum TokenType
  {
    /// <summary>
    /// A block start marker.
    /// </summary>
    BlockEnd,

    /// <summary>
    /// A block end marker.
    /// </summary>
    BlockStart,

    /// <summary>
    /// Closing curly brace '}' indicating the end of a bracket block.
    /// </summary>
    ClosingCurlyBrace,

    /// <summary>
    /// Closing  parenthesis ')' indicating the end of a bracket block.
    /// </summary>
    ClosingParenthesis,

    /// <summary>
    /// Closing square bracket ']' indicating the end of a bracket block.
    /// </summary>
    ClosingSquareBracket,

    /// <summary>
    /// A comment.
    /// </summary>
    Comment,

    /// <summary>
    /// Lack of whitespace considered a level of indentation has been removed (Python-only).
    /// </summary>
    Dedent,

    /// <summary>
    /// A statement delimiter.
    /// </summary>
    Delimiter,

    /// <summary>
    /// A whole statement that overrides the current delimiter.
    /// </summary>
    DelimiterOverride,

    /// <summary>
    /// Whitespace considered a level of indentation has been added (Python-only).
    /// </summary>
    Indent,

    /// <summary>
    /// No token.
    /// </summary>
    None,

    /// <summary>
    /// Opening curly brace '{' indicating the start of a bracket block.
    /// </summary>
    OpeningCurlyBrace,

    /// <summary>
    /// Opening  parenthesis '(' indicating the start of a bracket block.
    /// </summary>
    OpeningParenthesis,

    /// <summary>
    /// Opening square bracket '[' indicating the start of a bracket block.
    /// </summary>
    OpeningSquareBracket,

    /// <summary>
    /// A quoted token (string literal).
    /// </summary>
    Quoted,

    /// <summary>
    /// An unrecognized token.
    /// </summary>
    Unknown,

    /// <summary>
    /// An unquoted token.
    /// </summary>
    UnQuoted,

    /// <summary>
    /// Any member of the <see cref="System.Globalization.UnicodeCategory.SpaceSeparator"/>,
    /// <see cref="System.Globalization.UnicodeCategory.LineSeparator"/>,
    /// <see cref="System.Globalization.UnicodeCategory.ParagraphSeparator"/>
    /// or the characters CHARACTER TABULATION, LINE FEED, LINE TABULATION, FORM FEED, CARRIAGE RETURN, NEXT LINE, and NO-BREAK SPACE.
    /// </summary>
    /// <remarks>For more information see <seealso cref="char.IsWhiteSpace(char)"/>.</remarks>
    WhiteSpace
  }
}
