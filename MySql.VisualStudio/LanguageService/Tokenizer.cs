// Copyright © 2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
//
// This file is part of MySQL Tools for Visual Studio.
// MySQL Tools for Visual Studio is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public 
// License version 2.1 as published by the Free Software Foundation
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.Collections.Generic;
using System.Text;

namespace MySql.Data.VisualStudio
{
    class Tokenizer
    {
        private string text;

        #region Properties

        public string Text
        {
            get { return text; }
            set { text = value; Pos = 0; }
        }

        public bool AnsiQuotes { get; set; }
        public bool BackslashEscapes { get; set; }
        public int StartIndex { get; private set; }
        public int StopIndex { get; private set; }
        public bool ReturnComments { get; set; }
        public bool LineComment { get; private set; }
        public bool BlockComment { get; set; }
        public bool Quoted { get; private set; }
        private int Pos { get; set; }
        private string LastToken { get; set; }

        #endregion


        public string NextToken()
        {
            if (LastToken == "*/" && BlockComment)
                BlockComment = false;

            LastToken = GetNextToken();
            return LastToken;
        }

        private string GetNextToken()
        {
            if (Text == null) return null;

            StartIndex = StopIndex = 0;
            Quoted = false;
            LineComment = false;

            while (Pos < Text.Length)
            {
                if (BlockComment)
                    return ExtractComment();

                char c = Text[Pos++];
                if (Char.IsWhiteSpace(c) && StartIndex == 0) continue;

                StartIndex = Pos - 1;

                if (IsQuoteCharacter(c) && !BlockComment)
                    return ExtractQuotedToken(c);

                string comment = ReadComment(c);
                if (comment != null && ReturnComments) return comment;

                return ExtractUnquotedToken();
            }
            return null;
        }

        #region Private methods

        private string ReadComment(char startingChar)
        {
            if (startingChar != '/' && startingChar != '#' && startingChar != '-') return null;
            if (startingChar == '-' && Text.Length == (Pos + 1)) return null;
            if (startingChar == '/' && Text.Length == Pos) return null;

            if ((startingChar == '-' && Text[Pos] == '-' && Text[Pos+1] == ' ') ||
                startingChar == '#')
            {
                LineComment = true;
                return ExtractComment();
            }

            if (startingChar == '/' && Text[Pos] == '*')
            {
                BlockComment = true;
                return ExtractComment();
            }

            return null;
        }

        private string ExtractComment()
        {
            char lastChar = Char.MinValue;

            while (Pos < Text.Length)
            {
                char c = Text[Pos++];
                if ((c == '\n' && LineComment) ||
                    (c == '/' && lastChar == '*' && BlockComment))
                    return ExtractToken(true);
                lastChar = c;
            }
            return ExtractToken(false);
        }

        private string ExtractUnquotedToken()
        {
            while (Pos < Text.Length)
            {
                char c = Text[Pos++];
                if (IsTokenTerminator(c))
                    return ExtractToken(true);
            }
            return ExtractToken(false);
        }

        private string ExtractQuotedToken(char quoteChar)
        {
            bool escaped = false;
            while (Pos < Text.Length)
            {
                char c = Text[Pos++];
                if (c == quoteChar && !escaped)
                {
                    Quoted = true;
                    return ExtractToken(true);
                }
                if (escaped) escaped = false;
                if (c == '\\' && BackslashEscapes)
                    escaped = true;
            }
            return ExtractToken(false);
        }

        private bool IsQuoteCharacter(char c)
        {
            return c == '\'' || c == '`' || (c == '\"' && AnsiQuotes);
        }

        private bool IsTokenTerminator(char c)
        {
            if (c == '\n') return true;
            if (c == '#' || c == '-' || c == '/' || c == '\\') return true;
            if (c == '(' || c == ',') return true;
            if (Char.IsWhiteSpace(c)) return true;
            return false;
        }

        private string ExtractToken(bool preserveLast)
        {
            if (preserveLast) Pos--;

            StopIndex = Pos - 1;
            if (Pos < Text.Length)
                return Text.Substring(StartIndex, StopIndex - StartIndex + 1);
            StopIndex = Pos;
            return Text.Substring(StartIndex, StopIndex - StartIndex);
        }

        #endregion
    }
}
