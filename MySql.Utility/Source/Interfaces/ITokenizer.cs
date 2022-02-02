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

using System;
using System.Collections.Generic;
using MySql.Utility.Enums;

namespace MySql.Utility.Interfaces
{
  /// <summary>
  /// Defines methods to tokenize a script.
  /// </summary>
  public interface ITokenizer
  {
    #region Properties

    /// <summary>
    /// Gets or sets the data to tokenize.
    /// </summary>
    string DataToTokenize { get; set; }

    /// <summary>
    /// Gets or sets the token used as a delimiter for the data to tokenize.
    /// </summary>
    string DelimiterToken { get; set; }

    /// <summary>
    /// Gets or sets the last token found by <see cref="FindToken"/>.
    /// </summary>
    string FoundToken { get; }

    /// <summary>
    /// Gets or sets the type of the last token found by <see cref="FindToken"/>.
    /// </summary>
    TokenType FoundTokenType { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the language used in this tokenizer is case sensitive.
    /// </summary>
    bool IsCaseSensitiveLanguage { get; }

    /// <summary>
    /// Gets or sets the position where the tokenizer starts or continues processing.
    /// </summary>
    int Position { get; }

    /// <summary>
    /// Gets or sets the starting position within the data given from which a token is read.
    /// </summary>
    int StartIndex { get; }

    /// <summary>
    /// Gets or sets the ending position within the <see cref="DataToTokenize"/> up to which a token is read.
    /// </summary>
    int StopIndex { get; }

    /// <summary>
    /// Gets the corresponding <see cref="StringComparison"/> for the tokenizer's language.
    /// </summary>
    StringComparison StringComparison { get; }

    #endregion

    /// <summary>
    /// Base method that breaks the given script into a list of statements using the delimiter configured.
    /// </summary>
    /// <returns>List of individual statements.</returns>
    List<string> BreakIntoStatements();

    /// <summary>
    /// Finds the next token in the data given.
    /// </summary>
    /// <returns>A <see cref="TokenType"/> value.</returns>
    TokenType FindToken();

    /// <summary>
    /// Returns the next token in the data given and stores it in <see cref="FoundToken"/>.
    /// </summary>
    void GetNextToken();

    /// <summary>
    /// Gets a list of tokens along with their types from the data given.
    /// </summary>
    /// <returns>A list of tokens along with their types from the data given.</returns>
    List<Tuple<string, TokenType>> GetAllTokens();
  }
}
