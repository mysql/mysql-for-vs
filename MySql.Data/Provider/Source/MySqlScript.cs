// Copyright (C) 2004-2007 MySQL AB
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

using MySql.Data.Common;
using System.Collections.Generic;
using System.Text;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using MySql.Data.MySqlClient.Properties;
namespace MySql.Data.MySqlClient
{
    /// <summary>
    /// Provides a class capable of executing a SQL script containing
    /// multiple SQL statements including CREATE PROCEDURE statements
    /// that require changing the delimiter
    /// </summary>
    public class MySqlScript
    {
        private MySqlConnection connection;
        private string query;
        private string delimiter;

        public event MySqlStatementExecutedEventHandler StatementExecuted;
        public event MySqlScriptErrorEventHandler Error;
        public event EventHandler ScriptCompleted;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="MySqlScript"/> class.
        /// </summary>
        public MySqlScript()
        {
            delimiter = ";";
        }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="MySqlScript"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public MySqlScript(MySqlConnection connection) : this()
        {
            this.connection = connection;
        }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="MySqlScript"/> class.
        /// </summary>
        /// <param name="query">The query.</param>
        public MySqlScript(string query) : this()
        {
            this.query = query;
        }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="MySqlScript"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="query">The query.</param>
        public MySqlScript(MySqlConnection connection, string query)
            :this()
        {
            this.connection = connection;
            this.query = query;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the connection.
        /// </summary>
        /// <value>The connection.</value>
        public MySqlConnection Connection
        {
            get { return connection; }
            set { connection = value; }
        }

        /// <summary>
        /// Gets or sets the query.
        /// </summary>
        /// <value>The query.</value>
        public string Query
        {
            get { return query; }
            set { query = value; }
        }

        /// <summary>
        /// Gets or sets the delimiter.
        /// </summary>
        /// <value>The delimiter.</value>
        public string Delimiter
        {
            get { return delimiter; }
            set { delimiter = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Executes this instance.
        /// </summary>
        /// <returns>The number of statements executed as part of the script.</returns>
        public int Execute()
        {
            bool openedConnection = false;

            if (connection == null)
                throw new InvalidOperationException(Resources.ConnectionNotSet);
            if (query == null || query.Length == 0)
                return 0;

            // next we open up the connetion if it is not already open
            if (connection.State != ConnectionState.Open)
            {
                openedConnection = true;
                connection.Open();
            }

            try
            {
                string mode = connection.driver.Property("sql_mode");
                mode = mode.ToUpper(CultureInfo.InvariantCulture);
                bool ansiQuotes = mode.IndexOf("ANSI_QUOTES") != -1;
                bool noBackslashEscapes = mode.IndexOf("NO_BACKSLASH_ESCAPES") != -1;

                // first we break the query up into smaller queries
                List<ScriptStatement> statements = BreakIntoStatements(ansiQuotes, noBackslashEscapes);

                int count = 0;
                MySqlCommand cmd = new MySqlCommand(null, connection);
                foreach (ScriptStatement statement in statements)
                {
                    cmd.CommandText = statement.text;
                    try
                    {
                        cmd.ExecuteNonQuery();
                        count++;
                        OnQueryExecuted(statement);
                    }
                    catch (Exception ex)
                    {
                        if (Error == null)
                            throw;
                        if (!OnScriptError(ex))
                            break;
                    }
                }
                OnScriptCompleted();
                return count;
            }
            finally
            {
                if (openedConnection)
                {
                    connection.Close();
                }
            }
        }

        #endregion

        private void OnQueryExecuted(ScriptStatement statement)
        {
            if (StatementExecuted != null)
            {
                MySqlScriptEventArgs args = new MySqlScriptEventArgs();
                args.Statement = statement;
                StatementExecuted(this, args);
            }
        }

        private void OnScriptCompleted()
        {
            if (ScriptCompleted != null)
                ScriptCompleted(this, null);
        }

        private bool OnScriptError(Exception ex)
        {
            if (Error != null)
            {
                MySqlScriptErrorEventArgs args = new MySqlScriptErrorEventArgs(ex);
                Error(this, args);
                return args.Ignore;
            }
            return false;
        }

        private List<int> BreakScriptIntoLines()
        {
            List<int> lineNumbers = new List<int>();

            StringReader sr = new StringReader(query);
            string line = sr.ReadLine();
            int pos = 0;
            while (line != null)
            {
                lineNumbers.Add(pos);
                pos += line.Length;
                line = sr.ReadLine();
            }
            return lineNumbers;
        }

        private static int FindLineNumber(int position, List<int> lineNumbers)
        {
            int i = 0;
            while (i < lineNumbers.Count && position < lineNumbers[i])
                i++;
            return i;
        }

        private List<ScriptStatement> BreakIntoStatements(bool ansiQuotes, bool noBackslashEscapes)
        {
            int startPos = 0;
            List<ScriptStatement> statements = new List<ScriptStatement>();
            List<int> lineNumbers = BreakScriptIntoLines();
            MySqlTokenizer tokenizer = new MySqlTokenizer(query);

            tokenizer.AnsiQuotes = ansiQuotes;
            tokenizer.BackslashEscapes = !noBackslashEscapes;

            string token = tokenizer.NextToken();
            while (token != null)
            {
                if (!tokenizer.Quoted) // &&
                    //!tokenizer.IsSize)
                {
                    int delimiterPos = token.IndexOf(Delimiter);
                    if (delimiterPos != -1)
                    {
                        int endPos = tokenizer.StopIndex - token.Length + delimiterPos;
                        if (tokenizer.StopIndex == query.Length-1)
                            endPos++;
                        string currentQuery = query.Substring(startPos, endPos-startPos);
                        ScriptStatement statement = new ScriptStatement();
                        statement.text = currentQuery.Trim();
                        statement.line = FindLineNumber(startPos, lineNumbers);
                        statement.position = startPos - lineNumbers[statement.line];
                        statements.Add(statement);
                        startPos = endPos + delimiter.Length;
                    }
                }
                token = tokenizer.NextToken();
            }

            // now clean up the last statement
            if (tokenizer.StartIndex > startPos)
            {
                string sqlLeftOver = query.Substring(startPos).Trim();
                if (!String.IsNullOrEmpty(sqlLeftOver))
                {
                    ScriptStatement statement = new ScriptStatement();
                    statement.text = sqlLeftOver;
                    statement.line = FindLineNumber(startPos, lineNumbers);
                    statement.position = startPos - lineNumbers[statement.line];
                    statements.Add(statement);
                }
            }
            return statements;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public delegate void MySqlStatementExecutedEventHandler(object sender, MySqlScriptEventArgs args);
    /// <summary>
    /// 
    /// </summary>
    public delegate void MySqlScriptErrorEventHandler(object sender, MySqlScriptErrorEventArgs args);

    /// <summary>
    /// 
    /// </summary>
    public class MySqlScriptEventArgs : EventArgs
    {
        private ScriptStatement statement;

        internal ScriptStatement Statement
        {
            set { this.statement = value; }
        }

        /// <summary>
        /// Gets the statement text.
        /// </summary>
        /// <value>The statement text.</value>
        public string StatementText
        {
            get { return statement.text; }
        }

        /// <summary>
        /// Gets the line.
        /// </summary>
        /// <value>The line.</value>
        public int Line
        {
            get { return statement.line; }
        }

        /// <summary>
        /// Gets the position.
        /// </summary>
        /// <value>The position.</value>
        public int Position
        {
            get { return statement.position; }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MySqlScriptErrorEventArgs : MySqlScriptEventArgs
    {
        private Exception exception;
        private bool ignore;

        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlScriptErrorEventArgs"/> class.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public MySqlScriptErrorEventArgs(Exception exception) : base()
        {
            this.exception = exception;
        }

        /// <summary>
        /// Gets the exception.
        /// </summary>
        /// <value>The exception.</value>
        public Exception Exception
        {
            get { return exception; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MySqlScriptErrorEventArgs"/> is ignore.
        /// </summary>
        /// <value><c>true</c> if ignore; otherwise, <c>false</c>.</value>
        public bool Ignore
        {
            get { return ignore; }
            set { ignore = value; }
        }
    }

    struct ScriptStatement
    {
        public string text;
        public int line;
        public int position;
    }
}
