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

using System;
using System.Collections;
using System.IO;
using System.Text;
using MySql.Data.Common;
using System.Data;

namespace MySql.Data.MySqlClient
{
    internal abstract class Statement
    {
        protected MySqlCommand command;
        protected string commandText;
        private ArrayList buffers;

        private Statement(MySqlCommand cmd)
        {
            command = cmd;
            buffers = new ArrayList();
        }

        public Statement(MySqlCommand cmd, string text) : this(cmd)
        {
            commandText = text;
        }

        #region Properties

        public virtual string ResolvedCommandText
        {
            get { return commandText; }
        }

        protected Driver Driver
        {
            get { return command.Connection.driver; }
        }

        protected MySqlConnection Connection
        {
            get { return command.Connection; }
        }

        protected MySqlParameterCollection Parameters
        {
            get { return command.Parameters; }
        }

        #endregion

        public virtual void Close()
        {
        }

        public virtual void Resolve()
        {
        }

        public virtual void Execute()
        {
            // we keep a reference to this until we are done
            BindParameters();
            ExecuteNext();
        }

        public virtual bool ExecuteNext()
        {
            if (buffers.Count == 0)
                return false;

            MySqlStream stream = (MySqlStream)buffers[0];
            MemoryStream ms = stream.InternalBuffer;
            Driver.Query(ms.GetBuffer(), (int) ms.Length);
            buffers.RemoveAt(0);
            return true;
        }

        protected virtual void BindParameters()
        {
            MySqlParameterCollection parameters = command.Parameters;
            int index = 0;

            while (true)
            {
                InternalBindParameters(ResolvedCommandText, parameters, null);

                // if we are not batching, then we are done.  This is only really relevant the
                // first time through
                if (command.Batch == null) return;
                while (index < command.Batch.Count)
                {
                    MySqlCommand batchedCmd = command.Batch[index++];
                    MySqlStream stream = (MySqlStream)buffers[buffers.Count - 1];

                    // now we make a guess if this statement will fit in our current stream
                    long estimatedCmdSize = batchedCmd.EstimatedSize();
                    if ((stream.InternalBuffer.Length + estimatedCmdSize) > Connection.driver.MaxPacketSize)
                    {
                        // it won't, so we setup to start a new run from here
                        parameters = batchedCmd.Parameters;
                        break;
                    }

                    // looks like we might have room for it so we remember the current end of the stream
                    buffers.RemoveAt(buffers.Count - 1);
                    long originalLength = stream.InternalBuffer.Length;

                    // and attempt to stream the next command
                    string text = batchedCmd.BatchableCommandText;
                    if (text.StartsWith("("))
                        stream.WriteStringNoNull(", ");
                    else
                        stream.WriteStringNoNull("; ");
                    InternalBindParameters(text, batchedCmd.Parameters, stream);
                    if (stream.InternalBuffer.Length > Connection.driver.MaxPacketSize)
                    {
                        stream.InternalBuffer.SetLength(originalLength);
                        parameters = batchedCmd.Parameters;
                        break;
                    }
                }
                if (index == command.Batch.Count)
                    return;
            }
        }

        private void InternalBindParameters(string sql, MySqlParameterCollection parameters, MySqlStream stream)
        {
            // tokenize the sql
            ArrayList tokenArray = TokenizeSql(sql);

            if (stream == null)
            {
                stream = new MySqlStream(Driver.Encoding);
                stream.Version = Driver.Version;
            }

            // make sure our token array ends with a ;
            string lastToken = (string) tokenArray[tokenArray.Count - 1];
            if (lastToken != ";")
                tokenArray.Add(";");

            foreach (String token in tokenArray)
            {
                if (token.Trim().Length == 0)
                    continue;
                if (token == ";")
                {
                    buffers.Add(stream);
                    stream = new MySqlStream(Driver.Encoding);
                    continue;
                }
                if (token.Length >= 2 && 
                    ((token[0] == '@' && token[1] != '@') || 
                    token[0] == '?'))
                {
                    if (SerializeParameter(parameters, stream, token))
                        continue;
                }

                // our fall through case is to write the token to the byte stream
                stream.WriteStringNoNull(token);
            }
        }

        /// <summary>
        /// We use a separate method here because we want to support using parameter
        /// names with and without a leading marker but we don't want the indexing
        /// methods of MySqlParameterCollection to support that.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="name"></param>
        /// <returns></returns>
/*        private MySqlParameter GetParameter(MySqlParameterCollection parameters, string name)
        {
            MySqlParameter parameter = parameters.GetParameterFlexible(name, false);
            int index = parameters.IndexOf(name);
            if (index == -1)
            {
                name = name.Substring(1);
                index = Parameters.IndexOf(name);
                if (index == -1)
                    return null;
            }
            return parameters[index];
        }
        */

        protected virtual bool ShouldIgnoreMissingParameter(string parameterName)
        {
            return Connection.Settings.AllowUserVariables ||
                (parameterName.Length > 1 &&
                (parameterName[1] == '`' || parameterName[1] == '\''));
        }

        /// <summary>
        /// Serializes the given parameter to the given memory stream
        /// </summary>
        /// <remarks>
        /// <para>This method is called by PrepareSqlBuffers to convert the given
        /// parameter to bytes and write those bytes to the given memory stream.
        /// </para>
        /// </remarks>
        /// <returns>True if the parameter was successfully serialized, false otherwise.</returns>
        private bool SerializeParameter(MySqlParameterCollection parameters,
                                        MySqlStream stream, string parmName)
        {
            MySqlParameter parameter = parameters.GetParameterFlexible(parmName, false);
            if (parameter == null)
            {
                // if we are allowing user variables and the parameter name starts with @
                // then we can't throw an exception
                if (parmName.StartsWith("@") && ShouldIgnoreMissingParameter(parmName))
                    return false;
                throw new MySqlException(
                    String.Format(Resources.ParameterMustBeDefined, parmName));
            }
            parameter.Serialize(stream, false);
            return true;
        }

        /// <summary>
        /// THis code is not used yet but will likely be used in the future.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private ArrayList TokenizeSql2(string sql) 
        {
            ArrayList sqlChunks = new ArrayList();
            StringBuilder currentChunk = new StringBuilder();
            bool batch = Connection.Settings.AllowBatch & Driver.SupportsBatch;

            int lastPos = 0;
            SqlTokenizer tokenizer = new SqlTokenizer(sql);
            string sql_mode = Connection.driver.Property("sql_mode");
            if (sql_mode != null)
            {
                sql_mode = sql_mode.ToString().ToLower();
                tokenizer.AnsiQuotes = sql_mode.IndexOf("ansi_quotes") != -1;
                tokenizer.BackslashEscapes = sql_mode.IndexOf("no_backslash_escapes") == -1;
            }

            string token = tokenizer.NextToken();
            while (token != null)
            {
                if (token == ";" && !batch)
                {
                    sqlChunks.Add(currentChunk.ToString());
                    currentChunk.Remove(0, currentChunk.Length);
                }

                else if (token.Length >= 2 &&
                    ((token[0] == '@' && token[1] != '@') ||
                    token[0] == '?'))
                {
                    sqlChunks.Add(currentChunk.ToString());
                    currentChunk.Remove(0, currentChunk.Length);
                }
                else
                {
                    currentChunk.Append(sql.Substring(lastPos, tokenizer.Index - lastPos+1));
                    lastPos = tokenizer.Index;
                }
                token = tokenizer.NextToken();
            }
            if (currentChunk.Length > 0)
                sqlChunks.Add(currentChunk.ToString());
            return sqlChunks;
        }

        /// <summary>
        /// Breaks the given SQL up into 'tokens' that are easier to output
        /// into another form (bytes, preparedText, etc).
        /// </summary>
        /// <param name="sql">SQL to be tokenized</param>
        /// <returns>Array of tokens</returns>
        /// <remarks>The SQL is tokenized at parameter markers ('?') and at 
        /// (';') sql end markers if the server doesn't support batching.
        /// </remarks>
        public ArrayList TokenizeSql(string sql)
        {
            bool batch = Connection.Settings.AllowBatch & Driver.SupportsBatch;
            char delim = Char.MinValue;
            StringBuilder sqlPart = new StringBuilder();
            bool escaped = false;
            ArrayList tokens = new ArrayList();

            sql = sql.TrimStart(';').TrimEnd(';');
            char c = Char.MinValue;
            char lastChar;
            for (int i = 0; i < sql.Length; i++)
            {
                lastChar = c;
                c = sql[i];
                if (escaped)
                    escaped = !escaped;
                else if (c == delim)
                    delim = Char.MinValue;
                else if (c == ';' && !escaped && delim == Char.MinValue && !batch)
                {
                    tokens.Add(sqlPart.ToString());
                    tokens.Add(";");
                    sqlPart.Remove(0, sqlPart.Length);
                    continue;
                }
                else if ((c == '\'' || c == '\"' || c == '`') && !escaped && delim == Char.MinValue)
                    delim = c;
                else if (c == '\\')
                    escaped = !escaped;
                else if (sqlPart.Length == 1 && sqlPart[0] == '@' && c == '@') { }
                else if ((c == '@' || c == '?') && delim == Char.MinValue && !escaped)
                {
                    tokens.Add(sqlPart.ToString());
                    sqlPart.Remove(0, sqlPart.Length);
                }
                else if (sqlPart.Length > 0 && (sqlPart[0] == '@' || sqlPart[0] == '?') &&
                         !Char.IsLetterOrDigit(c) && c != '_' && c != '.' && c != '$')
                {
                    tokens.Add(sqlPart.ToString());
                    sqlPart.Remove(0, sqlPart.Length);
                }

                sqlPart.Append(c);
            }
            tokens.Add(sqlPart.ToString());
            return tokens;
        }
    }
}
