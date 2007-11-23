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
            InternalBindParameters(ResolvedCommandText, command.Parameters, null);

            // now tack on the batched commands
            if (command.Batch == null) return;

            foreach (MySqlCommand cmd in command.Batch)
            {
                MySqlStream stream = (MySqlStream)buffers[buffers.Count - 1];
                buffers.RemoveAt(buffers.Count - 1);
                string text = cmd.BatchableCommandText;
                if (text.StartsWith("("))
                    stream.WriteStringNoNull(", ");
                else
                    stream.WriteStringNoNull("; ");
                InternalBindParameters(text, cmd.Parameters, stream);
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
                if (token[0] == '@' || token[0] == '?')
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
        private MySqlParameter GetParameter(MySqlParameterCollection parameters, string name)
        {
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
            MySqlParameter parameter = GetParameter(parameters, parmName);
            if (parameter == null)
            {
                // if we are using old syntax, we can't throw exceptions for parameters
                // not defined.
                if (Connection.Settings.UseOldSyntax)
                    return false;
                throw new MySqlException(
                    String.Format(Resources.ParameterMustBeDefined, parmName));
            }

            parameter.Serialize(stream, false);
            return true;
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
                else if ((c == '\'' || c == '\"' || c == '`') & !escaped & delim == Char.MinValue)
                    delim = c;
                else if (c == '\\')
                    escaped = !escaped;
                else if ((c == '@' || c == '?') && delim == Char.MinValue && !escaped && 
                    "(,= ".IndexOf(lastChar) != -1)
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