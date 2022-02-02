// Copyright (c) 2015, 2018, Oracle and/or its affiliates. All rights reserved.
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
using System.Text;
using MySql.Utility.Enums;
using MySql.Utility.Interfaces;

namespace MySql.Utility.Classes.Tokenizers
{
  /// <summary>
  /// Defines base methods to split a given script into individual tokens.
  /// </summary>
  public class BaseTokenizer : ITokenizer
  {
    #region Fields

    /// <summary>
    /// The <see cref="StringBuilder"/> used to build a single statement.
    /// </summary>
    private StringBuilder _currentStatementStringBuilder;

    /// <summary>
    /// Text that contains the data to tokenize.
    /// </summary>
    private string _dataToTokenize;

    /// <summary>
    /// Gets or sets a list of statements in which a script can be broken into.
    /// </summary>
    private List<string> _statements;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of BaseTokenizer class.
    /// </summary>
    public BaseTokenizer()
    {
      _statements = null;
      _currentStatementStringBuilder = null;
      BackslashEscapes = true;
      BlockEndToken = null;
      BlockStartToken = null;
      BracketBlocksCharacters = new List<Tuple<char, TokenType>>
      {
        new Tuple<char, TokenType>('{', TokenType.OpeningCurlyBrace),
        new Tuple<char, TokenType>('[', TokenType.OpeningSquareBracket),
        new Tuple<char, TokenType>('(', TokenType.OpeningParenthesis),
        new Tuple<char, TokenType>('}', TokenType.ClosingCurlyBrace),
        new Tuple<char, TokenType>(']', TokenType.ClosingSquareBracket),
        new Tuple<char, TokenType>(')', TokenType.ClosingParenthesis)
      };
      CommentStartEndTokens = null;
      CurrentStatementFirstToken = null;
      DelimiterToken = ";";
      DataToTokenize = null;
      FoundToken = null;
      FoundTokenType = TokenType.None;
      IgnoreComments = true;
      IgnoreWhiteSpace = false;
      IsCaseSensitiveLanguage = false;
      NestedBlocksCount = 0;
    }

    /// <summary>
    /// Initializes a new instance of BaseTokenizer class.
    /// </summary>
    /// <param name="dataToTokenize">Text that contains the data to tokenize.</param>
    public BaseTokenizer(string dataToTokenize)
      : this()
    {
      DataToTokenize = dataToTokenize;
    }

    #region Properties

    /// <summary>
    /// Gets or sets a value indicating whether a back-slash (\) is treated as an escape character within strings.
    /// </summary>
    public bool BackslashEscapes { get; set; }

    /// <summary>
    /// Gets or sets the token used to end a block of code.
    /// </summary>
    public string BlockEndToken { get; set; }

    /// <summary>
    /// Gets or sets the token used to start a block of code.
    /// </summary>
    public string BlockStartToken { get; set; }

    /// <summary>
    /// Gets a list of tuples containing bracket characters and their corresponding token type.
    /// </summary>
    public List<Tuple<char, TokenType>> BracketBlocksCharacters { get; protected set; }

    /// <summary>
    /// Gets or sets a list of tuples containing start and end comment tokens.
    /// </summary>
    public List<Tuple<string,string>> CommentStartEndTokens { get; set; }

    /// <summary>
    /// Gets or sets the data to tokenize.
    /// </summary>
    public string DataToTokenize
    {
      get
      {
        return _dataToTokenize;
      }

      set
      {
        _dataToTokenize = value;
        Position = 0;
      }
    }

    /// <summary>
    /// Gets or sets the token used as a delimiter for the data to tokenize.
    /// </summary>
    public string DelimiterToken { get; set; }

    /// <summary>
    /// Gets the last token found by <see cref="FindToken"/>.
    /// </summary>
    public string FoundToken { get; protected set; }

    /// <summary>
    /// Gets the type of the last token found by <see cref="FindToken"/>.
    /// </summary>
    public TokenType FoundTokenType { get; protected set; }

    /// <summary>
    /// Gets or sets a value indicating whether comment tokens are ignored.
    /// </summary>
    public bool IgnoreComments { get; set; }

    /// <summary>
    /// Gets a value indicating whether any whitespace (defined by <seealso cref="char.IsWhiteSpace(char)"/> is ignored.
    /// </summary>
    public bool IgnoreWhiteSpace { get; protected set; }

    /// <summary>
    /// Gets a value indicating whether the language used in this tokenizer is case sensitive.
    /// </summary>
    public bool IsCaseSensitiveLanguage { get; protected set; }

    /// <summary>
    /// Gets the position where the tokenizer starts or continues processing.
    /// </summary>
    public int Position { get; protected set;}

    /// <summary>
    /// Gets the starting position within the data given from which a token is read.
    /// </summary>
    public int StartIndex { get; protected set; }

    /// <summary>
    /// Gets the ending position within the <see cref="DataToTokenize"/> up to which a token is read.
    /// </summary>
    public int StopIndex { get; protected set; }

    /// <summary>
    /// Gets the corresponding <see cref="StringComparison"/> for the tokenizer's language.
    /// </summary>
    public StringComparison StringComparison
    {
      get
      {
        return IsCaseSensitiveLanguage ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
      }
    }

    /// <summary>
    /// Gets or sets the first token of the current statement, normally the one which names the statement itself.
    /// </summary>
    protected string CurrentStatementFirstToken { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="StringBuilder"/> used to build a single statement.
    /// </summary>
    protected StringBuilder CurrentStatementStringBuilder
    {
      get
      {
        if (_currentStatementStringBuilder == null)
        {
          _currentStatementStringBuilder = new StringBuilder(_dataToTokenize.Length);
        }

        return _currentStatementStringBuilder;
      }
    }

    /// <summary>
    /// Gets or sets the nested blocks founds so far by processing starting and ending block tokens.
    /// </summary>
    protected uint NestedBlocksCount { get; set; }

    /// <summary>
    /// Gets a list of statements in which a script can be broken into.
    /// </summary>
    protected List<string> Statements
    {
      get
      {
        if (_statements == null)
        {
          _statements = new List<string>();
        }

        return _statements;
      }
    }

    #endregion

    /// <summary>
    /// Base method that breaks the given script into a list of statements using the delimiter configured.
    /// </summary>
    /// <returns>List of individual statements.</returns>
    public virtual List<string> BreakIntoStatements()
    {
      InitializeTokenizer();
      GetNextToken();
      while (FoundToken != null)
      {
        switch (FoundTokenType)
        {
          case TokenType.WhiteSpace:
            ProcessWhiteSpace();
            break;

          case TokenType.Unknown:
          case TokenType.UnQuoted:
            ProcessUnQuoted();
            break;

          case TokenType.Quoted:
            ProcessQuoted();
            break;

          case TokenType.ClosingCurlyBrace:
          case TokenType.ClosingParenthesis:
          case TokenType.ClosingSquareBracket:
            ProcessClosingBracket();
            break;

          case TokenType.OpeningCurlyBrace:
          case TokenType.OpeningParenthesis:
          case TokenType.OpeningSquareBracket:
            ProcessOpeningBracket();
            break;

          case TokenType.BlockStart:
            ProcessBlockStart();
            break;

          case TokenType.BlockEnd:
            ProcessBlockEnd();
            break;

          case TokenType.Delimiter:
            ProcessDelimiter();
            break;

          case TokenType.Indent:
            ProcessIndent();
            break;

          case TokenType.Dedent:
            ProcessDedent();
            break;

          case TokenType.Comment:
            ProcessComment();
            break;

          case TokenType.DelimiterOverride:
          case TokenType.None:
            // Ignored tokens
            break;
        }

        GetNextToken();
      }

      // Process any leftovers after the last found delimiter
      var lastStatement = CurrentStatementStringBuilder.ToString().Trim();
      if (!string.IsNullOrEmpty(lastStatement))
      {
        Statements.Add(lastStatement);
      }

      return Statements;
    }

    /// <summary>
    /// Finds the next token in the data given.
    /// </summary>
    /// <returns>A <see cref="TokenType"/> value.</returns>
    public virtual TokenType FindToken()
    {
      FoundTokenType = TokenType.None;
      StartIndex = StopIndex = -1;
      while (Position < _dataToTokenize.Length)
      {
        char c = _dataToTokenize[Position++];
        if (IsDelimiterMarkerAndFoundInPosition(c, Position - 1, true))
        {
          break;
        }

        if (ReadWhiteSpace(c))
        {
          if (IgnoreWhiteSpace)
          {
            continue;
          }

          break;
        }

        if (ReadQuotedToken(c))
        {
          break;
        }

        if (ReadComment(c))
        {
          break;
        }

        if (ReadBlockStartOrEnd(c))
        {
          break;
        }

        if (ReadOpeningOrClosingBracket(c))
        {
          break;
        }

        if (ReadDelimiterTokenOverride(c))
        {
          break;
        }

        ReadUnquotedToken();

        if (StartIndex != -1)
        {
          break;
        }
      }

      return FoundTokenType;
    }

    /// <summary>
    /// Fetches the next token in the data given and stores it in <see cref="FoundToken"/>.
    /// </summary>
    public virtual void GetNextToken()
    {
      FoundToken = FindToken() != TokenType.None
        ? _dataToTokenize.Substring(StartIndex, StopIndex - StartIndex)
        : null;
    }

    /// <summary>
    /// Gets a list of tokens along with their types from the data given.
    /// </summary>
    /// <returns>A list of tokens along with their types from the data given.</returns>
    public virtual List<Tuple<string, TokenType>> GetAllTokens()
    {
      var tokens = new List<Tuple<string, TokenType>>();
      GetNextToken();
      while (FoundToken != null && FoundTokenType != TokenType.None)
      {
        tokens.Add(new Tuple<string, TokenType>(FoundToken, FoundTokenType));
        GetNextToken();
      }

      return tokens;
    }

    /// <summary>
    /// Adjusts the ending position of the tokenizer in case the end of the text given is reached.
    /// </summary>
    /// <param name="whileWhiteSpace">Flag indicating whether the stop position is set until a non-white space is found, or until a white space is found.</param>
    protected virtual void AdjustPositionsDependingOnWhiteSpace(bool whileWhiteSpace)
    {
      if (string.IsNullOrEmpty(_dataToTokenize) || StopIndex >= _dataToTokenize.Length)
      {
        return;
      }

      int pos = StopIndex;
      char c = _dataToTokenize[pos];
      while (pos < (_dataToTokenize.Length - 1) && (whileWhiteSpace && char.IsWhiteSpace(c) || (!whileWhiteSpace && !char.IsWhiteSpace(c))))
      {
        c = _dataToTokenize[++pos];
      }

      StopIndex = pos;
      Position = pos;
    }

    /// <summary>
    /// Gets the closing bracket character that corresponds to the given opening bracket <see cref="TokenType"/>.
    /// </summary>
    /// <param name="openingBracketTokenType">The <see cref="TokenType"/> that corresponds to the opening bracket.</param>
    /// <returns>The closing bracket character that corresponds to the given opening bracket <see cref="TokenType"/>.</returns>
    protected char GetClosingBracketCharacter(TokenType openingBracketTokenType)
    {
      switch (openingBracketTokenType)
      {
        case TokenType.OpeningCurlyBrace:
          return BracketBlocksCharacters.Find(t => t.Item2 == TokenType.ClosingCurlyBrace).Item1;

        case TokenType.OpeningSquareBracket:
          return BracketBlocksCharacters.Find(t => t.Item2 == TokenType.ClosingSquareBracket).Item1;

        case TokenType.OpeningParenthesis:
          return BracketBlocksCharacters.Find(t => t.Item2 == TokenType.ClosingParenthesis).Item1;

        default:
          return char.MinValue;
      }
    }

    /// <summary>
    /// Resets the values used by the tokenizer.
    /// </summary>
    protected virtual void InitializeTokenizer()
    {
      FoundToken = null;
      FoundTokenType = TokenType.None;
      NestedBlocksCount = 0;
      Statements.Clear();
      CurrentStatementStringBuilder.Clear();
    }

    /// <summary>
    /// Checks whether the <see cref="DelimiterToken"/> starts with the given character and is found in the current <see cref="Position"/>.
    /// </summary>
    /// <param name="c">The character to validate.</param>
    /// <param name="cPosition">The position of the given character relative to the data being tokenized.</param>
    /// <param name="adjustPositions">Flag indicating whether positions are adjusted.</param>
    /// <returns><c>true</c> if the <see cref="DelimiterToken"/> starts with the given character and is found in the current <see cref="Position"/>, <c>false</c> otherwise.</returns>
    protected virtual bool IsDelimiterMarkerAndFoundInPosition(char c, int cPosition, bool adjustPositions = false)
    {
      var stopIndex = cPosition + DelimiterToken.Length;
      var found = !string.IsNullOrEmpty(DelimiterToken)
                  && DelimiterToken[0] == c && stopIndex <= _dataToTokenize.Length
                  && _dataToTokenize.IndexOf(DelimiterToken, cPosition, StringComparison) == cPosition;

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
    /// Checks whether a given character is a special character.
    /// </summary>
    /// <param name="c">The character to validate.</param>
    /// <returns><c>true</c> if the given character is a special character, <c>false</c> otherwise.</returns>
    protected virtual bool IsSpecialCharacter(char c)
    {
      return !char.IsLetterOrDigit(c) && c != '$' && c != '_' && c != '.';
    }

    /// <summary>
    /// Checks whether a given character is a text delimiter marker.
    /// </summary>
    /// <param name="c">The character to validate.</param>
    /// <returns><c>true</c> if the given character is a text delimiter marker, <c>false</c> otherwise.</returns>
    protected virtual bool IsTextDelimiterMarker(char c)
    {
      return c == '\'' || c == '"';
    }

    /// <summary>
    /// Processes the end of a block of statements.
    /// </summary>
    protected virtual void ProcessBlockEnd()
    {
      NestedBlocksCount--;
      CurrentStatementStringBuilder.Append(FoundToken);
    }

    /// <summary>
    /// Processes the start of a block of statements.
    /// </summary>
    protected virtual void ProcessBlockStart()
    {
      NestedBlocksCount++;
      CurrentStatementStringBuilder.Append(FoundToken);
    }

    /// <summary>
    /// Processes the end of a bracket block.
    /// </summary>
    protected virtual void ProcessClosingBracket()
    {
      CurrentStatementStringBuilder.Append(FoundToken);
    }

    /// <summary>
    /// Processes a comment.
    /// </summary>
    protected virtual void ProcessComment()
    {
      if (IgnoreComments)
      {
        return;
      }

      CurrentStatementStringBuilder.Append(FoundToken);
    }

    /// <summary>
    /// Processes an added indentation level.
    /// </summary>
    protected virtual void ProcessDedent()
    {
    }

    /// <summary>
    /// Processes the delimiter used to separate statements.
    /// </summary>
    protected virtual void ProcessDelimiter()
    {
      if (NestedBlocksCount > 0)
      {
        CurrentStatementStringBuilder.Append(FoundToken);
        return;
      }

      StoreStatement();
    }

    /// <summary>
    /// Processes a removed indentation level.
    /// </summary>
    protected virtual void ProcessIndent()
    {
    }

    /// <summary>
    /// Processes the start of a bracket block.
    /// </summary>
    protected virtual void ProcessOpeningBracket()
    {
      CurrentStatementStringBuilder.Append(FoundToken);
    }

    /// <summary>
    /// Processes a quoted string token.
    /// </summary>
    protected virtual void ProcessQuoted()
    {
      CurrentStatementStringBuilder.Append(FoundToken);
    }

    /// <summary>
    /// Processes an unquoted token.
    /// </summary>
    protected virtual void ProcessUnQuoted()
    {
      CurrentStatementStringBuilder.Append(FoundToken);
      if (CurrentStatementFirstToken == null)
      {
        CurrentStatementFirstToken = FoundToken;
      }
    }

    /// <summary>
    /// Processes a series of contiguous whitespace.
    /// </summary>
    protected virtual void ProcessWhiteSpace()
    {
      CurrentStatementStringBuilder.Append(" ");
    }

    /// <summary>
    /// Reads a block start or end token.
    /// </summary>
    /// <param name="character">The character being processed.</param>
    /// <returns><c>true</c> if a block start ore end token was successfully read, <c>false</c> otherwise.</returns>
    protected virtual bool ReadBlockStartOrEnd(char character)
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
    /// Reads an opening or closing bracket.
    /// </summary>
    /// <param name="character">The character being processed.</param>
    /// <returns><c>true</c> if a bracket (opening or closing) was read successfully, <c>false</c> otherwise.</returns>
    protected virtual bool ReadOpeningOrClosingBracket(char character)
    {
      var bracketAndType = BracketBlocksCharacters.FirstOrDefault(t => t.Item1 == character);
      if (bracketAndType == null)
      {
        FoundTokenType = TokenType.None;
        return false;
      }

      FoundTokenType = bracketAndType.Item2;
      ResetStartStopPositions();
      return true;
    }

    /// <summary>
    /// Reads a single commented text token from the data given
    /// </summary>
    /// <param name="character">The currently processed character.</param>
    /// <returns><c>true</c> if a comment was read successfully, <c>false</c> otherwise.</returns>
    protected virtual bool ReadComment(char character)
    {
      // Attempt to find a comment tuple that matches the character and Position
      var startAndEndCommentTokens = CommentStartEndTokens == null
        ? null
        : CommentStartEndTokens.FirstOrDefault(tuple => ValidateTokenInCurrentPosition(character, tuple.Item1));
      if (startAndEndCommentTokens == null)
      {
        FoundTokenType = TokenType.None;
        return false;
      }

      // Adjusts positions depending on the comment end token
      int startingIndex = Position - 1;
      int index = _dataToTokenize.IndexOf(startAndEndCommentTokens.Item2, Position, StringComparison);
      Position = index == -1 ? _dataToTokenize.Length - 1 : index + startAndEndCommentTokens.Item2.Length;
      StartIndex = startingIndex;
      StopIndex = Position;
      FoundTokenType = TokenType.Comment;
      return true;
    }

    /// <summary>
    /// Handles the case where the delimiter is dynamically overriden within the script session.
    /// </summary>
    /// <param name="character">The currently processed character.</param>
    /// <returns><c>true</c> if the delimiter token was overriden, <c>false</c> otherwise.</returns>
    /// <remarks>
    /// The implementation of this method must take care of assigning the new delimiter to the <see cref="DelimiterToken"/>
    /// property and of updating the <see cref="FoundTokenType"/>, <see cref="StartIndex"/>, <see cref="StopIndex"/>
    /// and <see cref="Position"/> propreties so that the whole delimiter overriding statement is retunred as a single token.
    /// </remarks>
    protected virtual bool ReadDelimiterTokenOverride(char character)
    {
      FoundTokenType = TokenType.None;
      return false;
    }

    /// <summary>
    /// Reads a single quoted token from the data given
    /// </summary>
    /// <param name="character">The currently processed character.</param>
    /// <returns><c>true</c> if a quoted token was successfully read, <c>false</c> otherwise.</returns>
    protected virtual bool ReadQuotedToken(char character)
    {
      if (!IsTextDelimiterMarker(character))
      {
        return false;
      }

      bool escaped = false;
      FoundTokenType = TokenType.Unknown;
      StartIndex = Position - 1;
      while (Position < _dataToTokenize.Length)
      {
        char c = _dataToTokenize[Position++];
        if (c == character && !escaped)
        {
          FoundTokenType = TokenType.Quoted;
          break;
        }

        escaped = !escaped && (c == '\\' && BackslashEscapes);
      }

      StopIndex = Position;
      return true;
    }

    /// <summary>
    /// Reads a token that is not a comment and not a quoted text.
    /// </summary>
    protected virtual void ReadUnquotedToken()
    {
      FoundTokenType = TokenType.Unknown;
      StartIndex = Position - 1;
      if (!IsSpecialCharacter(_dataToTokenize[StartIndex]))
      {
        while (Position < _dataToTokenize.Length)
        {
          char c = _dataToTokenize[Position];
          if (char.IsWhiteSpace(c) || IsSpecialCharacter(c) || IsDelimiterMarkerAndFoundInPosition(c, Position))
          {
            FoundTokenType = TokenType.UnQuoted;
            break;
          }

          Position++;
        }
      }

      StopIndex = Position;
    }

    /// <summary>
    /// Reads a token that is a <see cref="TokenType.WhiteSpace"/>.
    /// </summary>
    /// <param name="character">The currently processed character.</param>
    /// <returns><c>true</c> if whitespace was successfully read, <c>false</c> otherwise.</returns>
    protected virtual bool ReadWhiteSpace(char character)
    {
      if (!char.IsWhiteSpace(character))
      {
        FoundTokenType = TokenType.None;
        return false;
      }

      StartIndex = Position - 1;
      while (Position < _dataToTokenize.Length)
      {
        char c = _dataToTokenize[Position];
        if (!char.IsWhiteSpace(c))
        {
          break;
        }

        Position++;
      }

      StopIndex = Position;
      FoundTokenType = TokenType.WhiteSpace;
      return true;
    }

    /// <summary>
    /// Resets the start and stop positions to the data given.
    /// </summary>
    protected virtual void ResetStartStopPositions()
    {
      StartIndex = Position - 1;
      StopIndex = Position;
    }

    /// <summary>
    /// Stores the current statement in the list of statements.
    /// </summary>
    protected virtual void StoreStatement()
    {
      CurrentStatementFirstToken = null;
      var currentStatement = CurrentStatementStringBuilder.ToString().Trim();
      if (!string.IsNullOrEmpty(currentStatement))
      {
        Statements.Add(currentStatement);
      }

      CurrentStatementStringBuilder.Clear();
    }

    /// <summary>
    /// Validates if a token starts with the given character and is present in the current position.
    /// </summary>
    /// <param name="character">The character being processed.</param>
    /// <param name="token">The token to validate against the character.</param>
    /// <returns><c>true</c> if the token starts with the given character and is present in the current position, <c>false</c> otherwise.</returns>
    protected virtual bool ValidateTokenInCurrentPosition(char character, string token)
    {
      var startPos = Position - 1;
      return !string.IsNullOrEmpty(token)
              && token[0] == character
              && startPos + token.Length <= _dataToTokenize.Length
              && _dataToTokenize.IndexOf(token, startPos, StringComparison) == startPos;
    }
  }
}
