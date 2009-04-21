// Copyright (c) 2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 2 as published by
// the Free Software Foundation
//
// There are special exceptions to the terms and conditions of the GPL 
// as it is applied to this software. View the full text of the 
// exception in file EXCEPTIONS in the directory of this software 
// distribution.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace MySql.Data.MySqlClient
{
    internal class MySqlTokenizer
    {
        private string sql;

        private int startLine;
        private int stopLine;
        private int startIndex;
        private int stopIndex;

        private bool ansiQuotes;
        private bool backslashEscapes;
        private bool returnComments;
        private bool multiLine;

        private bool quoted;
        private bool isComment;

        private int pos;

        public MySqlTokenizer()
        {
            backslashEscapes = true;
            multiLine = true;
            pos = 0;
        }

        public MySqlTokenizer(string input) : this()
        {
            sql = input;
        }

        #region Properties

        public string Text
        {
            get { return sql; }
            set { sql = value; pos = 0; }
        }

        public bool AnsiQuotes
        {
            get { return ansiQuotes; }
            set { ansiQuotes = value; }
        }

        public bool BackslashEscapes
        {
            get { return backslashEscapes; }
            set { backslashEscapes = value; }
        }

        public bool MultiLine
        {
            get { return multiLine; }
            set { multiLine = value; }
        }

        public bool Quoted
        {
            get { return quoted; }
            private set { quoted = value; }
        }

        public bool IsComment
        {
            get { return isComment; }
        }

        public int StartIndex
        {
            get { return startIndex; }
        }

        public int StopIndex
        {
            get { return stopIndex; }
        }

        public int StartLine
        {
            get { return startLine; }
        }

        public int StopLine
        {
            get { return stopLine; }
        }

        public bool ReturnComments
        {
            get { return returnComments; }
            set { returnComments = value; }
        }

        #endregion

        public List<string> GetAllTokens()
        {
            List<string> tokens = new List<string>();
            string token = NextToken();
            while (token != null)
            {
                tokens.Add(token);
                token = NextToken();
            }
            return tokens;
        }

        public string NextToken()
        {
            while (FindToken())
            {
                string token = sql.Substring(startIndex, stopIndex - startIndex).Trim();
                return token;
            }
            return null;
        }

        public string NextParameter()
        {
            while (FindToken())
            {
                if ((stopIndex - startIndex) < 2) continue;
         string token = sql.Substring(startIndex, stopIndex - startIndex).Trim();
                char c1 = sql[startIndex];
                char c2 = sql[startIndex+1];
                if (c1 != '@' && c1 != '?') continue;
                if (c1 == '@' && c2 == '@') continue;
                return sql.Substring(startIndex, stopIndex - startIndex);
            }
            return null;
        }

        public bool FindToken()
        {
            isComment = quoted = false;  // reset our flags
            startIndex = stopIndex = -1;

            while (pos < sql.Length)
            {
                char c = sql[pos++];
                if (Char.IsWhiteSpace(c)) continue;
                
                if (c == '`' || c == '\'' || (c == '"' && AnsiQuotes))
                    ReadQuotedToken(c);
                else if (c == '#' || c == '-' || c == '/')
                    AttemptToReadComment(c);
                else
                    ReadUnquotedToken();
                if (startIndex != -1) return true;
            }
            return false;
        }

        public string ReadParenthesis()
        {
            StringBuilder sb = new StringBuilder("(");
            int start = StartIndex;
            string token = NextToken();
            while (true)
            {
                if (token == null)
                    throw new InvalidOperationException("Unable to parse SQL");
                sb.Append(token);
                if (token == ")" && !Quoted) break;
                token = NextToken();
            }
            return sb.ToString();
        }

        private void AttemptToReadComment(char c)
        {
            // make sure the comment starts correctly
            if (c == '/' && (pos >= sql.Length || sql[pos] != '*')) return;
            if (c == '-' && ((pos + 1) >= sql.Length || sql[pos] != '-' || sql[pos + 1] != ' ')) return;

            string endingPattern = "\n";
            if (sql[pos] == '*')
                endingPattern = "*/";

            int startingIndex = pos-1;

            int index = sql.IndexOf(endingPattern, pos);
            if (index == -1) 
                index = sql.Length - 1;
            else 
                index += endingPattern.Length;

            pos = index;
            if (ReturnComments)
            {
                startIndex = startingIndex;
                stopIndex = index;
                isComment = true;
            }
        }

        private void CalculatePosition(int start, int stop)
        {
            startIndex = start;
            stopIndex = stop;
            if (!MultiLine) return;
        }

        private void ReadUnquotedToken()
        {
            startIndex = pos-1;

            if (!IsSpecialCharacter(sql[startIndex]))
            {
                while (pos < sql.Length)
                {
                    char c = sql[pos];
                    if (Char.IsWhiteSpace(c)) break;
                    if (IsSpecialCharacter(c)) break;
                    if (IsParameterMarker(c))
                    {
                        if (c != '@' || pos > startIndex + 1 || sql[startIndex] != '@') break;
                    }
                    pos++;
                }
            }

            Quoted = false;
            stopIndex = pos;
        }

        /// <summary>
        ///  Read a single quoted identifier from the stream
        /// </summary>
        /// <param name="quoteChar"></param>
        /// <returns></returns>
        private void ReadQuotedToken(char quoteChar)
        {
            startIndex = pos-1;
            bool escaped = false;

            while (pos < sql.Length)
            {
                char c = sql[pos];

                if (c == quoteChar && !escaped)
                    break;

                if (escaped)
                    escaped = false;
                if (c == '\\' && BackslashEscapes)
                    escaped = true;
                pos++;
            }
            pos++;
            Quoted = true;
            stopIndex = pos;
        }

        private bool IsQuoteChar(char c)
        {
            return c == '`' || c == '\'' || (c == '\"' && AnsiQuotes);
        }

        private bool IsParameterMarker(char c)
        {
            return c == '@' || c == '?'; 
        }

        private bool IsSpecialCharacter(char c)
        {
            return c == '=' || 
                   c == '(' || 
                   c == ')' || 
                   c == ',' || 
                   c == ';' || 
                   c == '-' || 
                   c == '/' || 
                   c == '#';
        }
    }
}
