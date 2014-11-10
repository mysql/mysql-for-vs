// Copyright © 2014, Oracle and/or its affiliates. All rights reserved.
//
// MySQL for Visual Studio is licensed under the terms of the GPLv2
// <http://www.gnu.org/licenses/old-licenses/gpl-2.0.html>, like most 
// MySQL Connectors. There are special exceptions to the terms and 
// conditions of the GPLv2 as it is applied to this software, see the 
// FLOSS License Exception
// <http://www.mysql.com/about/legal/licensing/foss-exception.html>.
//
// This program is free software; you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published 
// by the Free Software Foundation; version 2 of the License.
//
// This program is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
//
// You should have received a copy of the GNU General Public License along 
// with this program; if not, write to the Free Software Foundation, Inc., 
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using Microsoft.VisualStudio.Package;
using System.Collections.Generic;

namespace MySql.Data.VisualStudio
{
    internal class Configuration
    {
        private static List<string> keywords;

        static Configuration()
        {
            if (keywords != null) return;
            keywords = new List<string>();

            // procedures and functions
            keywords.Add("CREATE");
            keywords.Add("ALTER");
            keywords.Add("PROCEDURE");
            keywords.Add("CALL");
            keywords.Add("RETURN");
            keywords.Add("FUNCTION");
            keywords.Add("RETURNS");
            keywords.Add("DECLARE");
            keywords.Add("DEFINER");
            keywords.Add("CURRENT_USER");
            keywords.Add("OUT");
            keywords.Add("INOUT");
            keywords.Add("IN");
            keywords.Add("BEGIN");
            keywords.Add("END");

            // update
            keywords.Add("UPDATE");
            keywords.Add("TABLE");

            // delete
            keywords.Add("DELETE");

            // select 
            keywords.Add("SELECT");
            keywords.Add("FROM");
            keywords.Add("WHERE");
            keywords.Add("GROUP");
            keywords.Add("BY");
            keywords.Add("ASC");
            keywords.Add("DESC");
            keywords.Add("WITH");
            keywords.Add("ROLLUP");
            keywords.Add("HAVING");
            keywords.Add("ORDER");
            keywords.Add("LIMIT");
            keywords.Add("OFFSET");
            keywords.Add("INTO");
            keywords.Add("OUTFILE");
            keywords.Add("DUMPFILE");
            keywords.Add("FOR");
            keywords.Add("LOCK");
            keywords.Add("SHARE");
            keywords.Add("MODE");
            keywords.Add("ALL");
            keywords.Add("DISTINCT");
            keywords.Add("DISTINCTROW");
            keywords.Add("HIGH_PRIORITY");
            keywords.Add("STRAIGHT_JOIN");
            keywords.Add("SQL_SMALL_RESULT");
            keywords.Add("SQL_BIG_RESULT");
            keywords.Add("SQL_BUFFER_RESULT");
            keywords.Add("SQL_CACHE");
            keywords.Add("SQL_NO_CACHE");
            keywords.Add("SQL_CALC_FOUND_ROWS");

            // misc
            keywords.Add("SHOW");
            keywords.Add("PROCESSLIST");
            keywords.Add("KILL");
            keywords.Add("STATUS");

            // data types
            keywords.Add("INT");
            keywords.Add("CHAR");
            keywords.Add("VARCHAR");

            // functions
            keywords.Add("COUNT");
            keywords.Add("REPLACE");
        }

        public static TokenInfo GetTokenInfo(string token, MySqlTokenizer tokenizer)
        {
            TokenInfo ti = new TokenInfo();
            if (tokenizer.Quoted)
            {
                if (token.StartsWith("\'"))
                    ti.Type = TokenType.Literal;
                else
                {
                    if (token.StartsWith("\"") && !tokenizer.AnsiQuotes)
                        ti.Type = TokenType.String;
                    else
                        ti.Type = TokenType.Identifier;
                }
            }
            else if (IsKeyword(token.ToUpperInvariant()))
                ti.Type = TokenType.Keyword;
            else if (tokenizer.IsComment)
                ti.Type = TokenType.Comment;
            else
                ti.Type = TokenType.Text;
            ti.Color = GetTokenColor(ti);
            return ti;
        }

        private static TokenColor GetTokenColor(TokenInfo ti)
        {
            switch (ti.Type)
            {
                case TokenType.String:
                case TokenType.Literal:
                    return TokenColor.String;
                case TokenType.Identifier:
                    return TokenColor.Identifier;
                case TokenType.Keyword:
                    return TokenColor.Keyword;
                case TokenType.Comment:
                    return TokenColor.Comment;
                default:
                    return TokenColor.Text;
            }
        }

        private static bool IsKeyword(string token)
        {
            return keywords.Contains(token);
        }
    }
}
