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
using System.Text;
using MySql.Utility.Enums;

namespace MySql.Utility.Classes.Tokenizers
{
  public class MyPythonTokenizer : BaseTokenizer
  {
    #region Constants

    /// <summary>
    /// The line continuation character in Python.
    /// </summary>
    public const string LINE_CONTINUATION_CHARACTER = "\\";

    /// <summary>
    /// The number of spaces that are replaced by a tab.
    /// </summary>
    public const int SPACES_COUNT_PER_TAB = 4;

    #endregion Constants

    #region Fields

    /// <summary>
    /// Flag indicating if a block start was found and has not been processed.
    /// </summary>
    private bool _unprocessedBlockStart;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of MyPythonTokenizer class.
    /// </summary>
    public MyPythonTokenizer()
      : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of MyPythonTokenizer class.
    /// </summary>
    /// <param name="script">The script to tokenize.</param>
    public MyPythonTokenizer(string script)
      : base(script)
    {
      _unprocessedBlockStart = false;

      BlockStartSpacesCounts = new Stack<int>();
      BracketBlockInProcess = false;
      CurrentClosingBracketCharacter = char.MinValue;
      CurrentBlockStartSpacesCount = 0;
      SingleStatementBuilder = new StringBuilder();
      SpacesCountInReadWhiteSpace = 0;

      // To recognize the end of a block in Python is tricky, since a block is determined by
      //  indentation, we need to keep track of it instead of finding a specific end block token.
      BlockStartToken = ":";
      BlockEndToken = null;

      // In Python ";" is a valid delimiter for writing several statements in a single line
      //  but also the new line is a valid delimiter as well when one statement per line is used.
      AlternateDelimiterToken = Environment.NewLine;

      // Valid comments are like:
      //  # This is a one-line comment
      CommentStartEndTokens = new List<Tuple<string, string>>
      {
        new Tuple<string, string>("#", Environment.NewLine)
      };

      IsCaseSensitiveLanguage = true;
    }

    #region Properties

    /// <summary>
    /// Gets an alternate token used as a delimiter in Python.
    /// </summary>
    public string AlternateDelimiterToken { get; private set; }

    /// <summary>
    /// Gets or sets a stack of space counts that represent indentations for Python blocks.
    /// </summary>
    protected Stack<int> BlockStartSpacesCounts { get; set; }

    /// <summary>
    /// Gets or sets a <see cref="StringBuilder"/> to assemble a single statement until a delimiter is found.
    /// </summary>
    protected StringBuilder SingleStatementBuilder { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a bracket block is currently being processed, which means anything inside it can span several lines.
    /// </summary>
    protected bool BracketBlockInProcess { get; set; }

    /// <summary>
    /// Gets or sets the character that closes the current bracket block being processed.
    /// </summary>
    protected char CurrentClosingBracketCharacter { get; set; }

    /// <summary>
    /// Gets or sets the space count for the current Python block.
    /// </summary>
    protected int CurrentBlockStartSpacesCount { get; set; }

    /// <summary>
    /// Gets or sets the previous token found by the last one by <see cref="BaseTokenizer.FindToken"/>.
    /// </summary>
    protected string PreviousFoundToken { get; set; }

    /// <summary>
    /// Gets or sets the type of the previous token found by the last one by <see cref="BaseTokenizer.FindToken"/>.
    /// </summary>
    protected TokenType PreviousFoundTokenType { get; set; }

    /// <summary>
    /// Gets or sets the number of spaces read by <see cref="ReadWhiteSpace"/> which processes not only spaces but all characters detected by <seealso cref="char.IsWhiteSpace(char)"/>
    /// </summary>
    /// <remarks>Each tab counts as 4 spaces.</remarks>
    protected int SpacesCountInReadWhiteSpace { get; set; }

    #endregion Properties

    /// <summary>
    /// Finds the next token in the data given.
    /// </summary>
    /// <returns>A <see cref="TokenType"/> value.</returns>
    public override TokenType FindToken()
    {
      PreviousFoundTokenType = FoundTokenType;
      return base.FindToken();
    }

    /// <summary>
    /// Fetches the next token in the data given and stores it in <see cref="BaseTokenizer.FoundToken"/>.
    /// </summary>
    public override void GetNextToken()
    {
      var positionCopy = Position;
      PreviousFoundToken = FoundToken;
      if (FindToken() == TokenType.None)
      {
        FoundToken = null;
        AppendSingleToCurrentStatement();
        return;
      }

      // If the found token is whitespace, the FoundToken was already assigned in the ReadWhiteSpace method.
      if (FoundTokenType != TokenType.WhiteSpace)
      {
        FoundToken = DataToTokenize.Substring(StartIndex, StopIndex - StartIndex);
      }

      if (PreviousFoundToken == LINE_CONTINUATION_CHARACTER
          || IsFoundTokenIndentOrDedent(positionCopy)
          || FoundToken != LINE_CONTINUATION_CHARACTER)
      {
        return;
      }

      // Skip the combination of LINE_CONTINUATION_CHARACTER + AlternateDelimiterToken since that is a line continuation and not a statement delimiter
      GetNextToken();
      if (FoundToken != AlternateDelimiterToken)
      {
        throw new TokenizerException(Resources.TokenizerUnexpectedCharAfterLineContinuatonError, Position);
      }

      FoundToken = LINE_CONTINUATION_CHARACTER;
      GetNextToken();
    }

    /// <summary>
    /// Resets the values used by the tokenizer.
    /// </summary>
    protected override void InitializeTokenizer()
    {
      base.InitializeTokenizer();
      SingleStatementBuilder.Clear();
    }

    /// <summary>
    /// Checks whether the statements delimiter starts with the given character and is found in the current <see cref="BaseTokenizer.Position"/>.
    /// </summary>
    /// <param name="c">The character to validate.</param>
    /// <param name="cPosition">The position of the given character relative to the data being tokenized.</param>
    /// <param name="adjustPositions">Flag indicating whether positions are adjusted.</param>
    /// <returns><c>true</c> if the statements delimiter starts with the given character and is found in the current <see cref="BaseTokenizer.Position"/>, <c>false</c> otherwise.</returns>
    protected override bool IsDelimiterMarkerAndFoundInPosition(char c, int cPosition, bool adjustPositions = false)
    {
      var totalLength = DataToTokenize.Length;
      var stringComparison = StringComparison;

      // Check first delimiter
      var stopIndex = cPosition + DelimiterToken.Length;
      var found = !string.IsNullOrEmpty(DelimiterToken)
                  && DelimiterToken[0] == c && stopIndex <= totalLength
                  && DataToTokenize.IndexOf(DelimiterToken, cPosition, stringComparison) == cPosition;

      // Check for alternate delimiter
      if (!found)
      {
        stopIndex = cPosition + AlternateDelimiterToken.Length;
        found = !string.IsNullOrEmpty(AlternateDelimiterToken)
                 && AlternateDelimiterToken[0] == c && stopIndex <= totalLength
                 && DataToTokenize.IndexOf(AlternateDelimiterToken, cPosition, stringComparison) == cPosition;
      }

      if (found)
      {
        if (adjustPositions)
        {
          StartIndex = cPosition;
          StopIndex = stopIndex;
          Position = StopIndex;
        }

        FoundTokenType = TokenType.Delimiter;
      }

      return found;
    }

    /// <summary>
    /// Processes the end of a block of statements.
    /// </summary>
    protected override void ProcessBlockEnd()
    {
      if (NestedBlocksCount == 0)
      {
        StoreStatement();
      }
    }

    /// <summary>
    /// Processes the start of a block of statements.
    /// </summary>
    protected override void ProcessBlockStart()
    {
      SingleStatementBuilder.Append(FoundToken);
      NestedBlocksCount++;
    }

    /// <summary>
    /// Processes the end of a bracket block.
    /// </summary>
    protected override void ProcessClosingBracket()
    {
      SingleStatementBuilder.Append(FoundToken);
      if (!BracketBlockInProcess
          || FoundToken[0] != CurrentClosingBracketCharacter)
      {
        return;
      }

      BracketBlockInProcess = false;
      CurrentClosingBracketCharacter = char.MinValue;
    }

    /// <summary>
    /// Processes a comment.
    /// </summary>
    protected override void ProcessComment()
    {
      if (!IgnoreComments)
      {
        SingleStatementBuilder.Append(FoundToken);
      }

      ProcessDelimiter();
    }

    /// <summary>
    /// Processes an added indentation level.
    /// </summary>
    protected override void ProcessDedent()
    {
      if (_unprocessedBlockStart
          && SpacesCountInReadWhiteSpace == 0)
      {
        throw new TokenizerException(Resources.TokenizerIndentedBlockExpectedError, Position);
      }

      do
      {
        CurrentBlockStartSpacesCount = BlockStartSpacesCounts.Pop();
        if (SpacesCountInReadWhiteSpace > CurrentBlockStartSpacesCount)
        {
          throw new TokenizerException(Resources.TokenizerUnexpectedDedentError, Position);
        }

        NestedBlocksCount--;
      } while (SpacesCountInReadWhiteSpace != CurrentBlockStartSpacesCount
               && BlockStartSpacesCounts.Count > 0);

      ProcessBlockEnd();
    }

    /// <summary>
    /// Processes the delimiter used to separate statements.
    /// </summary>
    protected override void ProcessDelimiter()
    {
      if (BracketBlockInProcess)
      {
        return;
      }

      if (FoundToken == DelimiterToken
          || (NestedBlocksCount > 0
              && (FoundTokenType != TokenType.Comment
                  || FoundTokenType == TokenType.Comment && !IgnoreComments)))
      {
        SingleStatementBuilder.Append(FoundToken);
      }

      AppendSingleToCurrentStatement();
      if (FoundToken == AlternateDelimiterToken)
      {
        ProcessBlockEnd();
      }
    }

    /// <summary>
    /// Processes a removed indentation level.
    /// </summary>
    protected override void ProcessIndent()
    {
      if (_unprocessedBlockStart
          && SpacesCountInReadWhiteSpace == CurrentBlockStartSpacesCount)
      {
        throw new TokenizerException(Resources.TokenizerIndentedBlockExpectedError, Position);
      }

      if (!_unprocessedBlockStart
          && SpacesCountInReadWhiteSpace > CurrentBlockStartSpacesCount)
      {
        throw new TokenizerException(Resources.TokenizerUnexpectedIndentError, Position);
      }

      SingleStatementBuilder.Append(FoundToken);
      if (SpacesCountInReadWhiteSpace == CurrentBlockStartSpacesCount)
      {
        return;
      }

      // A new level of indentation should be recorded
      _unprocessedBlockStart = false;
      CurrentBlockStartSpacesCount = SpacesCountInReadWhiteSpace;
    }

    /// <summary>
    /// Processes the start of a bracket block.
    /// </summary>
    protected override void ProcessOpeningBracket()
    {
      SingleStatementBuilder.Append(FoundToken);
      if (BracketBlockInProcess)
      {
        return;
      }

      BracketBlockInProcess = true;
      CurrentClosingBracketCharacter = GetClosingBracketCharacter(FoundTokenType);
    }

    /// <summary>
    /// Processes a quoted string token.
    /// </summary>
    protected override void ProcessQuoted()
    {
      SingleStatementBuilder.Append(FoundToken);
    }

    /// <summary>
    /// Processes an unquoted token.
    /// </summary>
    protected override void ProcessUnQuoted()
    {
      SingleStatementBuilder.Append(FoundToken);
      if (CurrentStatementFirstToken == null)
      {
        CurrentStatementFirstToken = FoundToken;
      }
    }

    /// <summary>
    /// Processes a series of contiguous whitespace.
    /// </summary>
    protected override void ProcessWhiteSpace()
    {
      SingleStatementBuilder.Append(BracketBlockInProcess || PreviousFoundToken == LINE_CONTINUATION_CHARACTER ? " " : FoundToken);
    }

    /// <summary>
    /// Reads a block start or end token.
    /// </summary>
    /// <param name="character">The character being processed.</param>
    /// <returns><c>true</c> if a block start ore end token was successfully read, <c>false</c> otherwise.</returns>
    protected override bool ReadBlockStartOrEnd(char character)
    {
      if (BracketBlockInProcess)
      {
        FoundTokenType = TokenType.None;
        return false;
      }

      int startingIndex = Position - 1;
      FoundTokenType = ValidateTokenInCurrentPosition(character, BlockStartToken)
        ? TokenType.BlockStart
        : TokenType.None;
      if (FoundTokenType == TokenType.None)
      {
        return false;
      }

      // Note we just process a block start, since a block is determined by indentation,
      //  there is no block end token and we do the finding of the block end in ProcessDedent.

      _unprocessedBlockStart = true;
      BlockStartSpacesCounts.Push(CurrentBlockStartSpacesCount);
      Position = startingIndex + BlockStartToken.Length;
      StartIndex = startingIndex;
      StopIndex = Position;
      return true;
    }

    /// <summary>
    /// Reads a single quoted token from the data given
    /// </summary>
    /// <param name="character">The currently processed character.</param>
    /// <returns><c>true</c> if a quoted token was successfully read, <c>false</c> otherwise.</returns>
    protected override bool ReadQuotedToken(char character)
    {
      if (!IsTextDelimiterMarker(character))
      {
        return false;
      }

      StartIndex = Position - 1;
      bool isTripleQuoteDelimiter = CheckIfTripleQuoteDelimiter(character);
      bool escaped = false;
      FoundTokenType = TokenType.Unknown;
      while (Position < DataToTokenize.Length)
      {
        char c = DataToTokenize[Position++];
        if (c == character && !escaped)
        {
          var found = true;

          // Check if the end delimiter is triple quoted
          if (isTripleQuoteDelimiter)
          {
            if (!CheckIfTripleQuoteDelimiter(character))
            {
              found = false;
            }
          }

          if (found)
          {
            FoundTokenType = TokenType.Quoted;
            break;
          }
        }

        escaped = !escaped && (c == '\\' && BackslashEscapes);
      }

      StopIndex = Position;
      return true;
    }

    /// <summary>
    /// Reads a token that is a <see cref="TokenType.WhiteSpace"/>.
    /// </summary>
    /// <param name="character">The currently processed character.</param>
    /// <returns><c>true</c> if whitespace was successfully read, <c>false</c> otherwise.</returns>
    protected override bool ReadWhiteSpace(char character)
    {
      var startIndex = Position - 1;
      if (!char.IsWhiteSpace(character)
          || AlternateDelimiterToken[0] == character
             && DataToTokenize.IndexOf(AlternateDelimiterToken, startIndex, StringComparison) == startIndex)
      {
        FoundTokenType = TokenType.None;
        return false;
      }

      Position = StartIndex = startIndex;
      char c = character;
      SpacesCountInReadWhiteSpace = 0;
      var tokenStringBuilder = new StringBuilder();
      do
      {
        Position++;
        switch (c)
        {
          case '\u0009': // Tab character
            SpacesCountInReadWhiteSpace += SPACES_COUNT_PER_TAB;
            tokenStringBuilder.Append(new string(' ', SPACES_COUNT_PER_TAB));
            break;

          default:
            SpacesCountInReadWhiteSpace++;
            tokenStringBuilder.Append(c);
            break;
        }

        c = DataToTokenize[Position];
      } while (Position < DataToTokenize.Length
               && char.IsWhiteSpace(c)
               && (AlternateDelimiterToken[0] != c
                   || DataToTokenize.IndexOf(AlternateDelimiterToken, Position, StringComparison) != Position));

      StopIndex = Position;
      FoundToken = tokenStringBuilder.ToString();
      FoundTokenType = TokenType.WhiteSpace;
      return true;
    }

    /// <summary>
    /// Appends the contents of the <see cref="SingleStatementBuilder"/> to the <see cref="BaseTokenizer.CurrentStatementStringBuilder"/>.
    /// </summary>
    private void AppendSingleToCurrentStatement()
    {
      var statement = SingleStatementBuilder.ToString();
      if (!string.IsNullOrEmpty(statement.Trim()))
      {
        CurrentStatementStringBuilder.Append(statement);
      }

      SingleStatementBuilder.Clear();
    }

    /// <summary>
    /// Checks if the next 3 characters correspond to a triple quote delimiter.
    /// </summary>
    /// <param name="quotedCharacter">The single quote delimiter character.</param>
    /// <returns><c>true</c> if the next 3 characters correspond to a triple quote delimiter, <c>false</c> otherwise.</returns>
    private bool CheckIfTripleQuoteDelimiter(char quotedCharacter)
    {
      var startPos = Position - 1;
      bool isTripleQuote = startPos + 3 < DataToTokenize.Length;
      if (isTripleQuote)
      {
        for (int i = 0; i < 3; i++)
        {
          char c = DataToTokenize[startPos + i];
          if (c != quotedCharacter)
          {
            isTripleQuote = false;
          }
        }
      }

      if (isTripleQuote)
      {
        Position += 2;
      }

      return isTripleQuote;
    }

    /// <summary>
    /// Checks if the token found needs to be processed as indentation and if so changes the <see cref="BaseTokenizer.FoundTokenType"/>.
    /// </summary>
    /// <param name="previousPosition">The position before the current token was found.</param>
    /// <returns><c>true</c> if the token found needs to be processed as indentation, <c>false</c> otherwise.</returns>
    private bool IsFoundTokenIndentOrDedent(int previousPosition)
    {
      if (((PreviousFoundTokenType != TokenType.Delimiter || PreviousFoundToken != AlternateDelimiterToken)
           && PreviousFoundTokenType != TokenType.Dedent)
          || BracketBlockInProcess
          || (FoundTokenType != TokenType.WhiteSpace && CurrentBlockStartSpacesCount <= 0))
      {
        return false;
      }

      if (FoundTokenType == TokenType.WhiteSpace)
      {
        if (SpacesCountInReadWhiteSpace >= CurrentBlockStartSpacesCount)
        {
          FoundTokenType = TokenType.Indent;
          return true;
        }
      }
      else
      {
        SpacesCountInReadWhiteSpace = 0;
      }

      FoundToken = string.Empty;
      FoundTokenType = TokenType.Dedent;
      Position = previousPosition;
      return true;
    }
  }
}
