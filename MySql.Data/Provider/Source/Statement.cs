// Copyright (c) 2004-2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
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
using MySql.Data.MySqlClient.Properties;
using System.Collections.Generic;

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

        public virtual void Close(MySqlDataReader reader)
        {
        }

        public virtual void Resolve(bool preparing)
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

            MySqlPacket packet = (MySqlPacket)buffers[0];
            //MemoryStream ms = stream.InternalBuffer;
            Driver.SendQuery(packet);
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
                    MySqlPacket packet = (MySqlPacket)buffers[buffers.Count - 1];

                    // now we make a guess if this statement will fit in our current stream
                    long estimatedCmdSize = batchedCmd.EstimatedSize();
                    if (((packet.Length-4) + estimatedCmdSize) > Connection.driver.MaxPacketSize)
                    {
                        // it won't, so we setup to start a new run from here
                        parameters = batchedCmd.Parameters;
                        break;
                    }

                    // looks like we might have room for it so we remember the current end of the stream
                    buffers.RemoveAt(buffers.Count - 1);
                    //long originalLength = packet.Length - 4;

                    // and attempt to stream the next command
                    string text = batchedCmd.BatchableCommandText;
                    if (text.StartsWith("("))
                        packet.WriteStringNoNull(", ");
                    else
                        packet.WriteStringNoNull("; ");
                    InternalBindParameters(text, batchedCmd.Parameters, packet);
                    if ((packet.Length-4) > Connection.driver.MaxPacketSize)
                    {
                        //TODO
                        //stream.InternalBuffer.SetLength(originalLength);
                        parameters = batchedCmd.Parameters;
                        break;
                    }
                }
                if (index == command.Batch.Count)
                    return;
            }
        }

        private void InternalBindParameters(string sql, MySqlParameterCollection parameters, 
            MySqlPacket packet)
        {
            if (packet == null)
            {
                packet = new MySqlPacket(Driver.Encoding);
                packet.Version = Driver.Version;
                packet.WriteByte(0);
            }

            int startPos = 0;
            MySqlTokenizer tokenizer = new MySqlTokenizer(sql);
            tokenizer.ReturnComments = true;
            string parameter = tokenizer.NextParameter();
            while (parameter != null)
            {
                packet.WriteStringNoNull(sql.Substring(startPos, tokenizer.StartIndex - startPos));
                bool serialized = SerializeParameter(parameters, packet, parameter);
                startPos = tokenizer.StopIndex;
                if (!serialized)
                    startPos = tokenizer.StartIndex;
                parameter = tokenizer.NextParameter();
            }
            packet.WriteStringNoNull(sql.Substring(startPos));
            buffers.Add(packet);
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
            if (Connection.Settings.AllowUserVariables)
                return true;
            if (command.parameterHash != null && parameterName.StartsWith("@" + command.parameterHash))
                return true;
            if (parameterName.Length > 1 &&
                (parameterName[1] == '`' || parameterName[1] == '\''))
                return true;
            return false;
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
                                        MySqlPacket packet, string parmName)
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
            parameter.Serialize(packet, false, Connection.Settings);
            return true;
        }
    }
}
