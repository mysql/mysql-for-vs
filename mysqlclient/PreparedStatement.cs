// Copyright (C) 2004-2006 MySQL AB
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
using System.Collections;

namespace MySql.Data.MySqlClient
{

	/// <summary>
	/// Summary description for PreparedStatement.
	/// </summary>
	internal class PreparedStatement : Statement
	{
		private MySqlField[] paramList;
		private int executionCount;
        private int pageSize;

        public PreparedStatement(MySqlConnection connection, string text, int pageSize)
            : base(connection, text)
        {
            this.pageSize = pageSize;
        }

		#region Properties

		public int NumParameters 
		{
			get { return paramList.Length; }
		}

		public int ExecutionCount 
		{
			get { return executionCount; }
			set { executionCount = value; }
		}

		#endregion

        public void Prepare()
        {
            // strip out names from parameter markers
            string text;
            ArrayList parameter_names = PrepareCommandText(out text);

            // ask our connection to send the prepare command
            statementId = driver.PrepareStatement(text, ref paramList);

            // now we need to assign our field names since we stripped them out
            // for the prepare
            for (int i=0; i < parameter_names.Count; i++)
                paramList[i].ColumnName = (string)parameter_names[i];
        }

		public override void Execute(MySqlParameterCollection parameters)
		{
			MySqlStream stream = new MySqlStream(driver.Encoding);

			//TODO: support long data here
			// create our null bitmap
			BitArray nullMap = new BitArray(parameters.Count);

            // now we run through the parameters that PREPARE sent back and use
            // those names to index into the parameters the user gave us.
            // if the user set that parameter to NULL, then we set the null map
            // accordingly
            if (paramList != null)
                for (int x=0; x < paramList.Length; x++)
			    {
                    MySqlParameter p = parameters[paramList[x].ColumnName];
				    if (p.Value == DBNull.Value || p.Value == null)
					    nullMap[x] = true;
			    }
			byte[] nullMapBytes = new byte[(parameters.Count + 7)/8];
			nullMap.CopyTo(nullMapBytes, 0);

			// start constructing our packet
			stream.WriteInteger(Id, 4);
            stream.WriteByte((byte)pageSize);          // flags; always 0 for 4.1
            stream.WriteInteger(1, 4);    // interation count; 1 for 4.1
            stream.Write(nullMapBytes);
			//if (parameters != null && parameters.Count > 0)
            stream.WriteByte(1);			// rebound flag
			//else
			//	packet.WriteByte( 0 );
			//TODO:  only send rebound if parms change

			// write out the parameter types
            if (paramList != null)
            {
                foreach (MySqlField param in paramList)
                {
                    MySqlParameter parm = parameters[param.ColumnName];
                    stream.WriteInteger((long)parm.GetPSType(), 2);
                }

                // now write out all non-null values
                foreach (MySqlField param in paramList)
                {
                    int index = parameters.IndexOf(param.ColumnName);
                    if (index == -1)
                        throw new MySqlException("Parameter '" + param.ColumnName +
                            "' is not defined.");
                    MySqlParameter parm = parameters[index];
                    if (parm.Value == DBNull.Value || parm.Value == null)
                        continue;

                    stream.Encoding = param.Encoding;
                    parm.Serialize(stream, true);
                }
            }

			executionCount ++;

            driver.ExecuteStatement(stream.InternalBuffer.ToArray());
		}

        public override bool ExecuteNext()
        {
            return false;
        }

        /// <summary>
        /// Prepares CommandText for use with the Prepare method
        /// </summary>
        /// <returns>Command text stripped of all paramter names</returns>
        /// <remarks>
        /// Takes the output of TokenizeSql and creates a single string of SQL
        /// that only contains '?' markers for each parameter.  It also creates
        /// the parameterMap array list that includes all the paramter names in the
        /// order they appeared in the SQL
        /// </remarks>
        private ArrayList PrepareCommandText(out string stripped_sql)
        {
            StringBuilder	newSQL = new StringBuilder();
            ArrayList parameterMap = new ArrayList();

            // tokenize the sql first
            ArrayList tokens = TokenizeSql(commandText);
            parameterMap.Clear();

            foreach (string token in tokens)
            {
                if ( token[0] != connection.ParameterMarker)
                    newSQL.Append(token);
                else
                {
                    parameterMap.Add( token );
                    newSQL.Append(connection.ParameterMarker);
                }
            }

            stripped_sql = newSQL.ToString();
            return parameterMap;
        }
	}
}
