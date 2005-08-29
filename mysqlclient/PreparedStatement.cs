// Copyright (C) 2004 MySQL AB
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
using System.IO;
using System.Collections;

namespace MySql.Data.MySqlClient
{

	/// <summary>
	/// Summary description for PreparedStatement.
	/// </summary>
	internal class PreparedStatement
	{
		private Driver				driver;
		private int					statementId;
		private MySqlField[]		paramList;
		private int					executionCount;

		public PreparedStatement(Driver driver, int statementId, MySqlField[] paramList )
		{
			this.paramList = new MySqlField[0];
			this.driver = driver;
			this.statementId = statementId;
			this.paramList = paramList;
			executionCount = 0;
		}

		#region Properties

		public int StatementId 
		{
			get { return statementId; }
		}

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

		public CommandResult Execute( MySqlParameterCollection parameters, int cursorPageSize )
		{
			if (parameters.Count < paramList.Length)
				throw new MySqlException( "Invalid number of parameters for statement execute" );

			MySqlStreamWriter writer = new MySqlStreamWriter(new MemoryStream(), driver.Encoding);

			//TODO: support long data here
			// create our null bitmap
			BitArray nullMap = new BitArray( parameters.Count ); //metaData.Length );
			for (int x=0; x < parameters.Count; x++)
			{
				if (parameters[x].Value == DBNull.Value ||
					parameters[x].Value == null)
					nullMap[x] = true;
			}
			byte[] nullMapBytes = new byte[ (parameters.Count + 7)/8 ];
			nullMap.CopyTo( nullMapBytes, 0 );

			// start constructing our packet
			writer.WriteInteger( StatementId, 4 );
			writer.WriteByte( (byte)cursorPageSize );          // flags; always 0 for 4.1
			writer.WriteInteger( 1, 4 );    // interation count; 1 for 4.1
			writer.Write( nullMapBytes );
			//if (parameters != null && parameters.Count > 0)
				writer.WriteByte( 1 );			// rebound flag
			//else
			//	packet.WriteByte( 0 );
			//TODO:  only send rebound if parms change

			// write out the parameter types
			foreach ( MySqlField param in paramList )
			{
				MySqlParameter parm = parameters[ param.ColumnName ];
				writer.WriteInteger( (long)parm.MySqlDbType, 2 ); 
			}

			// now write out all non-null values
			foreach ( MySqlField param in paramList )
			{
				MySqlParameter parm = parameters[ param.ColumnName ];
				if (parm.Value == DBNull.Value || parm.Value == null) continue;

				writer.Encoding = param.Encoding;
				parm.Serialize(writer, true);
			}

			executionCount ++;
			// send the data packet and return the CommandResult
			CommandResult result = driver.ExecuteStatement(
				((System.IO.MemoryStream)writer.Stream).ToArray(), StatementId, cursorPageSize );
			return result;
		}

	}
}
