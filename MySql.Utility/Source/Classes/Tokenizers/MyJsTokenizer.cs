// Copyright (c) 2015, 2016, Oracle and/or its affiliates. All rights reserved.
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
using System.Linq;
using MySql.Utility.Enums;

namespace MySql.Utility.Classes.Tokenizers
{
  public class MyJsTokenizer : BaseTokenizer
  {
    /// <summary>
    /// Initializes a new instance of MyJsTokenizer class.
    /// </summary>
    public MyJsTokenizer()
      : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of MyJsTokenizer class.
    /// </summary>
    /// <param name="script">The script to tokenize.</param>
    public MyJsTokenizer(string script)
      : base(script)
    {
      BlockStartToken = "{";
      BlockEndToken = "}";

      BlockStartValidFirstTokens = new List<string>
      {
        "class",
        "do",
        "for",
        "function",
        "if",
        "switch",
        "try",
        "while",
        "with"
      };

      BlockStartValidPreviousTokens = new List<string>
      {
        "catch",
        "else",
        "finally"
      };

      // Valid comments are like:
      //  // This is a one-line comment
      //  /* This is a multi-line or in-line comment */
      CommentStartEndTokens = new List<Tuple<string, string>>
      {
        new Tuple<string, string>("//", Environment.NewLine),
        new Tuple<string, string>("/*", "*/")
      };

      IsCaseSensitiveLanguage = true;
      PreviousFoundToken = null;
    }

    #region Properties

    /// <summary>
    /// Gets or sets a list of valid tokens that start a statement and can have a block.
    /// </summary>
    protected List<string> BlockStartValidFirstTokens { get; set; }

    /// <summary>
    /// Gets or sets a list of valid tokens that can be followed by a block start.
    /// </summary>
    protected List<string> BlockStartValidPreviousTokens { get; set; }

    /// <summary>
    /// Gets or sets the previously found token.
    /// </summary>
    protected string PreviousFoundToken { get; set; }

    #endregion Properties

    /// <summary>
    /// Returns the next token in the data given and stores it in <see cref="BaseTokenizer.FoundToken"/>.
    /// </summary>
    public override void GetNextToken()
    {
      if (FoundTokenType != TokenType.WhiteSpace)
      {
        PreviousFoundToken = FoundToken;
      }

      base.GetNextToken();
    }

    /// <summary>
    /// Processes the end of a block of statements.
    /// </summary>
    protected override void ProcessBlockEnd()
    {
      base.ProcessBlockEnd();
      if (NestedBlocksCount == 0)
      {
        StoreStatement();
      }
    }

    /// <summary>
    /// Reads a block start or end token.
    /// </summary>
    /// <param name="character">The character being processed.</param>
    /// <returns><c>true</c> if a block start ore end token was successfully read, <c>false</c> otherwise.</returns>
    protected override bool ReadBlockStartOrEnd(char character)
    {
      int startingIndex = Position - 1;
      FoundTokenType = ValidateTokenInCurrentPosition(character, BlockStartToken)
                        && ValidateOpeningCurlyBraceIsBlockStart()
        ? TokenType.BlockStart
        : TokenType.None;
      if (FoundTokenType == TokenType.None)
      {
        FoundTokenType = ValidateTokenInCurrentPosition(character, BlockEndToken)
                         && NestedBlocksCount > 0
          ? TokenType.BlockEnd
          : TokenType.None;
      }

      switch (FoundTokenType)
      {
        case TokenType.None:
          return false;

        case TokenType.BlockStart:
          Position = startingIndex + BlockStartToken.Length;
          break;

        case TokenType.BlockEnd:
          Position = startingIndex + BlockEndToken.Length;
          break;
      }

      StartIndex = startingIndex;
      StopIndex = Position;
      return true;
    }

    /// <summary>
    /// Validates that a found opening curly brace '{' is a valid block start.
    /// </summary>
    /// <returns><c>true</c> if the found opening curly brace '{' is a valid block start, <c>false</c> otherwise.</returns>
    private bool ValidateOpeningCurlyBraceIsBlockStart()
    {
      return !string.IsNullOrEmpty(CurrentStatementFirstToken)
             && (BlockStartValidFirstTokens.Any(t => CurrentStatementFirstToken.Equals(t, StringComparison))
                 || BlockStartValidPreviousTokens.Any(t => PreviousFoundToken.Equals(t, StringComparison)));
    }
  }
}
