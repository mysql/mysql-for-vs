// Copyright © 2011, Oracle and/or its affiliates. All rights reserved.
//
// MySQL Connector/NET is licensed under the terms of the GPLv2
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

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace MySql.Data.VisualStudio
{
  /// <summary>
  /// Represents a provider of MySQL tags.
  /// </summary>
  internal sealed class MySqlTokenTagger : ITagger<MySqlTokenTag>
  {
    private List<string> keywords;
    private List<string> operators;
    private Tokenizer tokenizer = new Tokenizer(true);

    /// <summary>
    /// Initializes a new instance of MySqlTokenTagger.
    /// </summary>
    /// <param name="buffer">The <see cref="ITextBuffer"/>.</param>
    internal MySqlTokenTagger(ITextBuffer buffer)
    {
      this.tokenizer.ReturnComments = true;
      this.Initialize();
    }

    /// <summary>
    /// Occurs when tags change in response to a change in the text buffer.
    /// </summary>
    public event EventHandler<SnapshotSpanEventArgs> TagsChanged
    {
      add { }
      remove { }
    }

    /// <summary>
    /// Gets the tags found in the specified spans.
    /// </summary>
    /// <param name="spans">Spans to check for supported tags.</param>
    /// <returns>A <see cref="IEnumerable"/> containing the list of tags.</returns>
    public IEnumerable<ITagSpan<MySqlTokenTag>> GetTags(NormalizedSnapshotSpanCollection spans)
    {
      string token = null;
      int startIndex;
      ITextSnapshotLine containingLine = null;
      SnapshotSpan snapshotSpan;

      // Not efficient, but we need to make sure every single line is re-tagged
      // specially because of scrolling in the VS editor.
      foreach (SnapshotSpan span in spans)
      {
        foreach (var line in span.Snapshot.Lines)
        {
          containingLine = line.Start.GetContainingLine();

          tokenizer.Text = containingLine.GetText();
          token = tokenizer.NextToken();

          while (!string.IsNullOrWhiteSpace(token))
          {
            token = token.Trim();
            startIndex = containingLine.Start + tokenizer.StartIndex;

            snapshotSpan = new SnapshotSpan(span.Snapshot, new Span(startIndex, token.Length));
            if (snapshotSpan.IntersectsWith(span))
            {
              yield return new TagSpan<MySqlTokenTag>(snapshotSpan, new MySqlTokenTag(GetTokenType(token)));
            }

            tokenizer.BlockComment = (tokenizer.BlockComment && !token.EndsWith("*/")) ? true : false;

            token = tokenizer.NextToken();
          }
        }
      }
    }

    private void Initialize()
    {
      InitializeKeywords();
      InitializeOperators();
    }

    private void InitializeKeywords()
    {
      if (keywords != null) return;
      keywords = new List<string>();

      keywords.Add("ACCESSIBLE");
      keywords.Add("ADD");
      keywords.Add("ALL");
      keywords.Add("ALTER");
      keywords.Add("ANALYZE");
      keywords.Add("AND");
      keywords.Add("AS");
      keywords.Add("ASC");
      keywords.Add("ASENSITIVE");
      keywords.Add("BEFORE");
      keywords.Add("BETWEEN");
      keywords.Add("BINARY");
      keywords.Add("BOTH");
      keywords.Add("BY");
      keywords.Add("CALL");
      keywords.Add("CASCADE");
      keywords.Add("CASE");
      keywords.Add("CHANGE");
      keywords.Add("CHARACTER");
      keywords.Add("CHECK");
      keywords.Add("COLLATE");
      keywords.Add(":");
      keywords.Add("COLUMN");
      keywords.Add("CONDITION");
      keywords.Add("CONSTRAINT");
      keywords.Add("CONTINUE");
      keywords.Add("CONVERT");
      keywords.Add("CREATE");
      keywords.Add("CROSS");
      keywords.Add("CURRENT_DATE");
      keywords.Add("CURRENT_TIME");
      keywords.Add("CURRENT_TIMESTAMP");
      keywords.Add("CURSOR");
      keywords.Add("");
      
      /*
      DATABASE	:	'DATABASE';
DATABASES	:	'DATABASES';
DAY_HOUR	:	'DAY_HOUR';
DAY_MICROSECOND	:	'DAY_MICROSECOND';
DAY_MINUTE	:	'DAY_MINUTE';
DAY_SECOND	:	'DAY_SECOND';
DEC	:	'DEC';
//DECIMAL	:	'DECIMAL';		// datatype defined below 
DECLARE	:	'DECLARE';
DEFAULT	:	'DEFAULT';
DELAYED	:	'DELAYED';
DELETE	:	'DELETE';
DESC	:	'DESC';
DESCRIBE	:	'DESCRIBE';
DETERMINISTIC	:	'DETERMINISTIC';
DISTINCT	:	'DISTINCT';
DISTINCTROW	:	'DISTINCTROW';
DIV	:	'DIV';
//DOUBLE	:	'DOUBLE';		// datatype defined below 
DROP	:	'DROP';
DUAL	:	'DUAL';
EACH	:	'EACH';
ELSE	:	'ELSE';
ELSEIF	:	'ELSEIF';
ENCLOSED	:	'ENCLOSED';
ESCAPED	:	'ESCAPED';
EXISTS	:	'EXISTS';
EXIT	:	'EXIT';
EXPLAIN	:	'EXPLAIN';
FALSE	:	'FALSE';
FETCH	:	'FETCH';
//FLOAT	:	'FLOAT';		// datatype defined below 
FLOAT4	:	'FLOAT4';
FLOAT8	:	'FLOAT8';
FOR	:	'FOR';
FORCE	:	'FORCE';
FOREIGN	:	'FOREIGN';
FROM	:	'FROM';
FULLTEXT	:	'FULLTEXT';
GOTO	:	'GOTO';
GRANT	:	'GRANT';
GROUP	:	'GROUP';
HAVING	:	'HAVING';
HIGH_PRIORITY	:	'HIGH_PRIORITY';
HOUR_MICROSECOND	:	'HOUR_MICROSECOND';
HOUR_MINUTE	:	'HOUR_MINUTE';
HOUR_SECOND	:	'HOUR_SECOND';
IF	:	'IF';
IFNULL	:	'IFNULL';
IGNORE	:	'IGNORE';
IN	:	'IN';
INDEX	:	'INDEX';
INFILE	:	'INFILE';
INNER	:	'INNER';
INNODB  : 'INNODB';
INOUT	:	'INOUT';
INSENSITIVE	:	'INSENSITIVE';
//INSERT	:	'INSERT';	// reserved keyword and function below
//INT	:	'INT';		// datatype defined below 
INT1	:	'INT1';
INT2	:	'INT2';
INT3	:	'INT3';
INT4	:	'INT4';
INT8	:	'INT8';
//INTEGER	:	'INTEGER';		// datatype defined below 
//INTERVAL	:	'INTERVAL';		// reserved keyword and function below
INTO	:	'INTO';
IS	:	'IS';
ITERATE	:	'ITERATE';
JOIN	:	'JOIN';
KEY	:	'KEY';
KEYS	:	'KEYS';
KILL	:	'KILL';
LABEL	:	'LABEL';
LEADING	:	'LEADING';
LEAVE	:	'LEAVE';
//LEFT	:	'LEFT';	// reserved keyword and function below
LIKE	:	'LIKE';
LIMIT	:	'LIMIT';
LINEAR	:	'LINEAR';
LINES	:	'LINES';
LOAD	:	'LOAD';
LOCALTIME	:	'LOCALTIME';
LOCALTIMESTAMP	:	'LOCALTIMESTAMP';
LOCK	:	'LOCK';
LONG	:	'LONG';
//LONGBLOB	:	'LONGBLOB';		// datatype defined below 
//LONGTEXT	:	'LONGTEXT';		// datatype defined below 
LOOP	:	'LOOP';
LOW_PRIORITY	:	'LOW_PRIORITY';
MASTER_SSL_VERIFY_SERVER_CERT	:	'MASTER_SSL_VERIFY_SERVER_CERT';
MATCH	:	'MATCH';
//MEDIUMBLOB	:	'MEDIUMBLOB';		// datatype defined below 
//MEDIUMINT	:	'MEDIUMINT';		// datatype defined below 
//MEDIUMTEXT	:	'MEDIUMTEXT';		// datatype defined below 
MIDDLEINT	:	'MIDDLEINT';		// datatype defined below 
MINUTE_MICROSECOND	:	'MINUTE_MICROSECOND';
MINUTE_SECOND	:	'MINUTE_SECOND';
MOD	:	'MOD';
MYISAM : 'MYISAM';
MODIFIES	:	'MODIFIES';
NATURAL	:	'NATURAL';
NOT	:	'NOT';
NO_WRITE_TO_BINLOG	:	'NO_WRITE_TO_BINLOG';
NULL	:	'NULL';
NULLIF	:	'NULLIF';
//NUMERIC	:	'NUMERIC';		// datatype defined below 
ON	:	'ON';
OPTIMIZE	:	'OPTIMIZE';
OPTION	:	'OPTION';
OPTIONALLY	:	'OPTIONALLY';
OR	:	'OR';
ORDER	:	'ORDER';
OUT	:	'OUT';
OUTER	:	'OUTER';
OUTFILE	:	'OUTFILE';
PRECISION	:	'PRECISION';
PRIMARY	:	'PRIMARY';
PROCEDURE	:	'PROCEDURE';
PURGE	:	'PURGE';
RANGE	:	'RANGE';
READ	:	'READ';
READS	:	'READS';
READ_ONLY	:	'READ_ONLY';
READ_WRITE	:	'READ_WRITE';
//REAL	:	'REAL';		// datatype defined below 
REFERENCES	:	'REFERENCES';
REGEXP	:	'REGEXP';
RELEASE	:	'RELEASE';
RENAME	:	'RENAME';
REPEAT	:	'REPEAT';
REPLACE	:	'REPLACE';
REQUIRE	:	'REQUIRE';
RESTRICT	:	'RESTRICT';
RETURN	:	'RETURN';
REVOKE	:	'REVOKE';
//RIGHT	:	'RIGHT';	// reserved keyword and function below
RLIKE	:	'RLIKE';
SCHEDULER : 'SCHEDULER';
SCHEMA	:	'SCHEMA';
SCHEMAS	:	'SCHEMAS';
SECOND_MICROSECOND	:	'SECOND_MICROSECOND';
SELECT	:	'SELECT';
SENSITIVE	:	'SENSITIVE';
SEPARATOR	:	'SEPARATOR';
SET	:	'SET';
SHOW	:	'SHOW';
//SMALLINT	:	'SMALLINT';		// datatype defined below 
SPATIAL	:	'SPATIAL';
SPECIFIC	:	'SPECIFIC';
SQL	:	'SQL';
SQLEXCEPTION	:	'SQLEXCEPTION';
SQLSTATE	:	'SQLSTATE';
SQLWARNING	:	'SQLWARNING';
SQL_BIG_RESULT	:	'SQL_BIG_RESULT';
SQL_CALC_FOUND_ROWS	:	'SQL_CALC_FOUND_ROWS';
SQL_SMALL_RESULT	:	'SQL_SMALL_RESULT';
SSL	:	'SSL';
STARTING	:	'STARTING';
STRAIGHT_JOIN	:	'STRAIGHT_JOIN';
TABLE	:	'TABLE';
TERMINATED	:	'TERMINATED';
THEN	:	'THEN';
//TINYBLOB	:	'TINYBLOB';		// datatype defined below 
//TINYINT	:	'TINYINT';		// datatype defined below 
//TINYTEXT	:	'TINYTEXT';		// datatype defined below 
TO	:	'TO';
TRAILING	:	'TRAILING';
TRIGGER	:	'TRIGGER';
TRUE	:	'TRUE';
UNDO	:	'UNDO';
UNION	:	'UNION';
UNIQUE	:	'UNIQUE';
UNLOCK	:	'UNLOCK';
UNSIGNED	:	'UNSIGNED';
UPDATE	:	'UPDATE';
USAGE	:	'USAGE';
USE	:	'USE';
USING	:	'USING';
//UTC_DATE	:	'UTC_DATE';		// next three are functions defined below
//UTC_TIME	:	'UTC_TIME';
//UTC_TIMESTAMP	:	'UTC_TIMESTAMP';
VALUES	:	'VALUES';
//VARBINARY	:	'VARBINARY';		// datatype defined below 
//VARCHAR	:	'VARCHAR';		// datatype defined below 
VARCHARACTER	:	'VARCHARACTER';
VARYING	:	'VARYING';
WHEN	:	'WHEN';
WHERE	:	'WHERE';
WHILE	:	'WHILE';
WITH	:	'WITH';
WRITE	:	'WRITE';
XOR	:	'XOR';
YEAR_MONTH	:	'YEAR_MONTH';
ZEROFILL	:	'ZEROFILL';
      */

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
      keywords.Add("VIEW");
      keywords.Add("AS");
      keywords.Add("TRIGGER");

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
      keywords.Add("LEFT");
      keywords.Add("LIMIT");
      keywords.Add("OFFSET");
      keywords.Add("INNER");
      keywords.Add("INTO");
      keywords.Add("JOIN");
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
      keywords.Add("RIGHT");
      keywords.Add("STRAIGHT_JOIN");
      keywords.Add("SQL_SMALL_RESULT");
      keywords.Add("SQL_BIG_RESULT");
      keywords.Add("SQL_BUFFER_RESULT");
      keywords.Add("SQL_CACHE");
      keywords.Add("SQL_NO_CACHE");
      keywords.Add("SQL_CALC_FOUND_ROWS");

      // misc
      keywords.Add("DROP");
      keywords.Add("SHOW");
      keywords.Add("PROCESSLIST");
      keywords.Add("KILL");
      keywords.Add("STATUS");
      keywords.Add("RENAME");
      keywords.Add("TRUNCATE");

      // data types
      keywords.Add("BIGINT");
      keywords.Add("BIT");
      keywords.Add("BLOB");
      keywords.Add("CHAR");
      keywords.Add("DATE");
      keywords.Add("DATETIME");
      keywords.Add("DECIMAL");
      keywords.Add("DOUBLE");
      keywords.Add("ENUM");
      keywords.Add("FLOAT");
      keywords.Add("INT");
      keywords.Add("INTEGER");
      keywords.Add("LONGBLOB");
      keywords.Add("LONGTEXT");
      keywords.Add("MEDIUMBLOB");
      keywords.Add("MEDIUMINT");
      keywords.Add("MEDIUMTEXT");
      keywords.Add("NUMERIC");
      keywords.Add("REAL");
      keywords.Add("SMALLINT");
      keywords.Add("TEXT");
      keywords.Add("TIME");
      keywords.Add("TIMESTAMP");
      keywords.Add("TINYBLOB");
      keywords.Add("TINYINT");
      keywords.Add("TINYTEXT");
      keywords.Add("VARBINARY");
      keywords.Add("VARCHAR");

      // functions
      keywords.Add("COUNT");
      keywords.Add("REPLACE");

      // trigger keywords
      keywords.Add("BEFORE");
      keywords.Add("AFTER");
      keywords.Add("INSERT");
      keywords.Add("ON");
      keywords.Add("EACH");
      keywords.Add("ROW");
      keywords.Add("SET");

      // Other
      keywords.Add("IF");
      keywords.Add("CASE");
      keywords.Add("WHEN");
      keywords.Add("THEN");
      keywords.Add("ELSE");
    }

    private void InitializeOperators()
    {
      if (operators != null) return;
      operators = new List<string>();

      // Logical
      operators.Add("AND");
      operators.Add("&&");
      operators.Add("&");
      operators.Add("NOT");
      operators.Add("!");
      operators.Add("||");
      operators.Add("|");
      operators.Add("OR");
      operators.Add("XOR");
      operators.Add("^");

      // Assignment
      operators.Add("=");
      operators.Add(":=");

      // Arithmetic
      operators.Add("+");
      operators.Add("-");
      operators.Add("*");
      operators.Add("/");
      operators.Add("DIV");
      operators.Add("%");

      // Comparison
      operators.Add("BETWEEN");
      operators.Add("<=>");
      operators.Add(">=");
      operators.Add(">");
      operators.Add("IS");
      operators.Add("IS NOT NULL");
      operators.Add("IS NULL");
      operators.Add("<=");
      operators.Add("<");
      operators.Add("LIKE");
      operators.Add("!=");
      operators.Add("<>");
      operators.Add("%");
      operators.Add("SOUNDS LIKE");

      // Others
      operators.Add("BINARY");
      operators.Add("~");
      operators.Add("<<");
      operators.Add(">>");
      operators.Add("REGEXP");
      operators.Add("RLIKE");
    }

    private MySqlTokenType GetTokenType(string token)
    {
      if (tokenizer.LineComment) return MySqlTokenType.Comment;
      else if (tokenizer.BlockComment) return MySqlTokenType.Comment;
      else if (IsKeyword(token)) return MySqlTokenType.Keyword;
      else if (IsOperator(token)) return MySqlTokenType.Operator;
      else if (tokenizer.Quoted) return MySqlTokenType.Literal;
      return MySqlTokenType.Text;
    }

    private bool IsKeyword(string token)
    {
      return keywords.Contains(token.ToUpperInvariant());
    }

    private bool IsOperator(string token)
    {
      return operators.Contains(token.ToUpper());
    }
  }
}
