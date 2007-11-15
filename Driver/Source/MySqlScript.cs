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

        public event MySqlQueryExecutedEventHandler QueryExecuted;
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
                mode = mode.ToLower(CultureInfo.InvariantCulture);
                bool ansiQuotes = mode.IndexOf("ansi_quotes") != -1;
                bool noBackslashEscapes = mode.IndexOf("no_backslash_escpaes") != -1;

                // first we break the query up into smaller queries
                List<string> queries = BreakIntoQueries(ansiQuotes, noBackslashEscapes);

                int count = 0;
                MySqlCommand cmd = new MySqlCommand(null, connection);
                foreach (string singleQuery in queries)
                {
                    cmd.CommandText = singleQuery;
                    cmd.ExecuteNonQuery();
                    count++;
                    OnQueryExecuted(singleQuery);
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

        private void OnQueryExecuted(string singleQuery)
        {
            if (QueryExecuted != null)
                QueryExecuted(this, new QueryExecutedEventArgs(singleQuery));
        }

        private void OnScriptCompleted()
        {
            if (ScriptCompleted != null)
                ScriptCompleted(this, null);
        }

        private List<string> BreakIntoQueries(bool ansiQuotes, bool noBackslashEscapes)
        {
            int startPos = 0;
            List<string> queries = new List<string>();
            SqlTokenizer tokenizer = new SqlTokenizer(query);

            tokenizer.AnsiQuotes = ansiQuotes;
            tokenizer.BackslashEscapes = !noBackslashEscapes;

            string token = tokenizer.NextToken();
            while (token != null)
            {
                if (!tokenizer.Quoted &&
                    !tokenizer.IsSize)
                {
                    int delimiterPos = token.IndexOf(Delimiter);
                    if (delimiterPos != -1)
                    {
                        int endPos = tokenizer.CurrentPos - token.Length + delimiterPos;
                        if (tokenizer.CurrentPos == query.Length-1)
                            endPos++;
                        string currentQuery = query.Substring(startPos, endPos-startPos);
                        queries.Add(currentQuery.Trim());
                        startPos = endPos + delimiter.Length;
                    }
                }
                token = tokenizer.NextToken();
            }

            // now clean up the last statement
            if (tokenizer.CurrentPos > startPos)
            {
                queries.Add(query.Substring(startPos).Trim());
            }
            return queries;
        }
    }

    public delegate void MySqlQueryExecutedEventHandler(object sender, QueryExecutedEventArgs args);

    /// <summary>
    /// 
    /// </summary>
    public class QueryExecutedEventArgs : EventArgs
    {
        private string query;

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="QueryExecutedEventArgs"/> class.
        /// </summary>
        /// <param name="query">The query.</param>
        public QueryExecutedEventArgs(string query)
        {
            this.query = query;
        }

        /// <summary>
        /// Gets the query.
        /// </summary>
        /// <value>The query.</value>
        public string Query
        {
            get { return query; }
        }
    }
}
