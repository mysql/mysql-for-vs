// Copyright (c) 2014, 2016, Oracle and/or its affiliates. All rights reserved.
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

namespace MySql.Utility.Classes.Tokenizers
{
  /// <summary>
  /// Defines methods to split a SQL text into individual tokens.
  /// </summary>
  public class MySqlTokenizer : BaseTokenizer
  {
    #region Constants

    /// <summary>
    /// The MySQL keyword to override the current session delimiter.
    /// </summary>
    public const string DELIMITER_KEYWORD = "DELIMITER";

    #endregion Constants

    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlTokenizer"/> class.
    /// </summary>
    public MySqlTokenizer()
      : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlTokenizer"/> class.
    /// </summary>
    /// <param name="sqlText">The text containing a SQL script.</param>
    public MySqlTokenizer(string sqlText)
      : base(sqlText)
    {
      AnsiQuotes = false;

      // To recognize the end of a block in MySQL is tricky, since many flow control statements
      //  end their blocks using END as well, so we may encounter false positives for closing blocks.
      BlockStartToken = "BEGIN";
      BlockEndToken = "END";

      // Valid comments are like:
      //  # This is a one-line comment
      //  -- This too is a one-line comment (note that -- must be followed by at least one space)
      //  /* This is a multi-line or in-line comment */
      CommentStartEndTokens = new List<Tuple<string, string>>
      {
        new Tuple<string, string>("#", Environment.NewLine),
        new Tuple<string, string>("-- ", Environment.NewLine),
        new Tuple<string, string>("/*", "*/")
      };

      IsCaseSensitiveLanguage = false;
    }

    #region Properties

    /// <summary>
    /// Gets or sets a value indicating whether a double quote (") is treated as an identifier quote character (like the back-tick (`) character) and not as a string quote character.
    /// </summary>
    public bool AnsiQuotes { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether square brackets ([,]) are used as delimiters for quoted text.
    /// </summary>
    public bool SquareBracketsAsQuotes { get; set; }

    #endregion Properties

    /// <summary>
    /// Fetches the next parameter as a token within <see cref="BaseTokenizer.DataToTokenize"/>.
    /// </summary>
    /// <returns>A token representing the next parameter, or <c>null</c> if there is no next parameter.</returns>
    public string NextParameter()
    {
      while (FindToken() != TokenType.None)
      {
        if ((StopIndex - StartIndex) < 2)
        {
          continue;
        }

        char c1 = DataToTokenize[StartIndex];
        char c2 = DataToTokenize[StartIndex + 1];
        if (c1 == '?' || (c1 == '@' && c2 != '@'))
        {
          return DataToTokenize.Substring(StartIndex, StopIndex - StartIndex);
        }
      }

      return null;
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
        ? TokenType.BlockStart
        : TokenType.None;
      if (FoundTokenType == TokenType.None)
      {
        FoundTokenType = ValidateTokenInCurrentPosition(character, BlockEndToken)
          ? TokenType.BlockEnd
          : TokenType.None;
      }

      // If the block end token was found, then check the next token to differentiate
      //  between an end token for a flow control statement and a block end token
      switch (FoundTokenType)
      {
        case TokenType.None:
          return false;

        case TokenType.BlockEnd:
          var positionCopy = Position = startingIndex + BlockEndToken.Length;
          IgnoreWhiteSpace = true;
          GetNextToken();
          IgnoreWhiteSpace = false;
          Position = positionCopy;
          FoundTokenType = FoundTokenType == TokenType.Delimiter
            ? TokenType.BlockEnd
            : TokenType.UnQuoted;
          break;

        case TokenType.BlockStart:
          Position = startingIndex + BlockStartToken.Length;
          break;
      }

      StartIndex = startingIndex;
      StopIndex = Position;
      return true;
    }

    /// <summary>
    /// Handles the case where the delimiter is dynamically overriden within the script session.
    /// </summary>
    /// <param name="c">The currently processed character.</param>
    /// <returns><c>true</c> if the delimiter token was overriden, <c>false</c> otherwise.</returns>
    /// <remarks>
    /// The implementation of this method must take care of assigning the new delimiter to the <see cref="BaseTokenizer.DelimiterToken"/>
    /// property and of updating the <see cref="BaseTokenizer.FoundTokenType"/>, <see cref="BaseTokenizer.StartIndex"/>,
    /// <see cref="BaseTokenizer.StopIndex"/> and <see cref="BaseTokenizer.Position"/> propreties so that the whole delimiter
    /// overriding statement is retunred as a single token.
    /// </remarks>
    protected override bool ReadDelimiterTokenOverride(char c)
    {
      if (!IsDelimiterOverrideKeywordAndFoundInPosition(c))
      {
        FoundTokenType = TokenType.None;
        return false;
      }

      // The StartIndex and StopIndex have been already adjusted by the IsDelimiterOverrideKeywordAndFoundInPosition call above
      // Adjust the StopIndex and Position skipping all whitespace found after the delimiter override keyword
      AdjustPositionsDependingOnWhiteSpace(true);

      // Adjust the StopIndex and Position skipping all non-whitespace found within the new delimiter
      var delimiterStartIndex = StopIndex;
      AdjustPositionsDependingOnWhiteSpace(false);

      // Assign new delimiter
      DelimiterToken = DataToTokenize.Substring(delimiterStartIndex, StopIndex - delimiterStartIndex).Trim();
      FoundTokenType = TokenType.DelimiterOverride;
      return true;
    }

    /// <summary>
    /// Checks whether a given character is a special character or a MysQL parameter marker.
    /// </summary>
    /// <param name="c">A character.</param>
    /// <returns><c>true</c> if the given character is a special character or a MySQL parameter marker, <c>false</c> otherwise.</returns>
    protected override bool IsSpecialCharacter(char c)
    {
      if (char.IsLetterOrDigit(c) || c == '$' || c == '_' || c == '.')
      {
        return false;
      }

      return !IsParameterMarker(c);
    }

    /// <summary>
    /// Checks whether a given character is a MySQL text delimiter marker.
    /// </summary>
    /// <param name="c">A character.</param>
    /// <returns><c>true</c> if the given character is a MySQL parameter marker, <c>false</c> otherwise.</returns>
    protected override bool IsTextDelimiterMarker(char c)
    {
      return c == '`' || c == '\'' || (c == '"' && !AnsiQuotes) || (c == '[' && SquareBracketsAsQuotes);
    }

    /// <summary>
    ///  Reads a single quoted token from the <see cref="BaseTokenizer.DataToTokenize"/>.
    /// </summary>
    /// <param name="character">The currently processed character.</param>
    /// <returns><c>true</c> if a quoted token was successfully read, <c>false</c> otherwise.</returns>
    protected override bool ReadQuotedToken(char character)
    {
      if (!IsTextDelimiterMarker(character))
      {
        return false;
      }

      if (character == '[')
      {
        character = ']';
      }

      StartIndex = Position - 1;
      bool escaped = false;
      FoundTokenType = TokenType.Unknown;
      while (Position < DataToTokenize.Length)
      {
        char c = DataToTokenize[Position];
        if (c == character && !escaped)
        {
          Position++;
          FoundTokenType = TokenType.Quoted;
          break;
        }

        if (escaped)
        {
          escaped = false;
        }
        else if (c == '\\' && BackslashEscapes)
        {
          escaped = true;
        }

        Position++;
      }

      StopIndex = Position;
      return true;
    }

    /// <summary>
    /// Checks whether the delimiter override keyword starts with the given character and is found in the current <see cref="BaseTokenizer.Position"/>.
    /// </summary>
    /// <param name="c">The character to validate.</param>
    /// <returns><c>true</c> if the delimiter override keyword starts with the given character and is found in the current <see cref="BaseTokenizer.Position"/>, <c>false</c> otherwise.</returns>
    private bool IsDelimiterOverrideKeywordAndFoundInPosition(char c)
    {
      StartIndex = Position - 1;
      StopIndex = StartIndex + DELIMITER_KEYWORD.Length;
      return DELIMITER_KEYWORD[0] == c
              && StopIndex <= DataToTokenize.Length
              && DataToTokenize.IndexOf(DELIMITER_KEYWORD, StartIndex, StringComparison) == StartIndex;
    }

    /// <summary>
    /// Checks whether a given character is a MySQL parameter marker.
    /// </summary>
    /// <param name="c">A character.</param>
    /// <returns><c>true</c> if the given character is a MySQL parameter marker, <c>false</c> otherwise.</returns>
    private bool IsParameterMarker(char c)
    {
      return c == '@' || c == '?';
    }
  }
}