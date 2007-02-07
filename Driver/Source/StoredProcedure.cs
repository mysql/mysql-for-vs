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
using System.Data;
using MySql.Data.Common;
using System.Text;
using MySql.Data.Types;

namespace MySql.Data.MySqlClient
{
	/// <summary>
	/// Summary description for StoredProcedure.
	/// </summary>
	internal class StoredProcedure : PreparableStatement
	{
		private string hash;
		private string outSelect;
		private DataTable parametersTable;
		private string resolvedCommandText;

		public StoredProcedure(MySqlConnection connection, string text)
			:
				base(connection, text)
		{
			uint code = (uint)DateTime.Now.GetHashCode();
			hash = code.ToString();
			this.connection = connection;
		}

		private string GetReturnParameter()
		{
			if (parameters != null)
				foreach (MySqlParameter p in parameters)
					if (p.Direction == ParameterDirection.ReturnValue)
					{
						string pName = p.ParameterName.Substring(1);
						return hash + pName;
					}
			return null;
		}

		public override string ResolvedCommandText
		{
            get { return resolvedCommandText; }
		}

		public override void Resolve()
		{
			// first retrieve the procedure definition from our
			// procedure cache
			string spName = commandText;
			if (spName.IndexOf(".") == -1)
				spName = connection.Database + "." + spName;
			DataSet ds = connection.ProcedureCache.GetProcedure(connection, spName);

			DataTable procTable = ds.Tables["procedures"];
			parametersTable = ds.Tables["procedure parameters"];

			StringBuilder sqlStr = new StringBuilder();
			StringBuilder setStr = new StringBuilder();
			outSelect = String.Empty;

			string retParm = GetReturnParameter();
			foreach (DataRow param in parametersTable.Rows)
			{
				if (param["ORDINAL_POSITION"].Equals(0)) continue;
				string mode = (string)param["PARAMETER_MODE"];
				string pName = (string)param["PARAMETER_NAME"];
				string datatype = (string)param["DATA_TYPE"];

				// make sure the parameters given to us have an appropriate
				// type set if it's not already
				MySqlParameter p = parameters[pName];
				if (!p.TypeHasBeenSet)
				{
					bool unsigned = param["FLAGS"].ToString().IndexOf("UNSIGNED") != -1;
					bool real_as_float = procTable.Rows[0]["SQL_MODE"].ToString().IndexOf("REAL_AS_FLOAT") != -1;
					p.MySqlDbType = MetaData.NameToType(datatype, unsigned, real_as_float, connection);
				}

				string basePName = pName.Substring(1);
				string vName = string.Format("@{0}{1}", hash, basePName);

				if (mode == "OUT" || mode == "INOUT")
				{
					outSelect += vName + ", ";
					sqlStr.Append(vName);
					sqlStr.Append(", ");
				}
				else
				{
					sqlStr.Append(pName);
					sqlStr.Append(", ");
				}

				if (mode == "INOUT")
				{
					setStr.AppendFormat("SET {0}={1};", vName, pName);
					outSelect += vName + ", ";
				}
			}

			string sqlCmd = sqlStr.ToString().TrimEnd(' ', ',');
			outSelect = outSelect.TrimEnd(' ', ',');
			if (procTable.Rows[0]["ROUTINE_TYPE"].Equals("PROCEDURE"))
				sqlCmd = String.Format("call {0} ({1})", commandText, sqlCmd);
			else
			{
                if (retParm == null)
                    retParm = hash + "dummy";
                else
                    outSelect = String.Format("@{0}", retParm);
                sqlCmd = String.Format("set @{0}={1}({2})", retParm, commandText, sqlCmd);
			}

			if (setStr.Length > 0)
				sqlCmd = setStr.ToString() + sqlCmd;
			string oldCmdText = commandText;

			resolvedCommandText = sqlCmd;
		}

		public override void Close()
		{
			if (outSelect.Length == 0) return;

			char marker = connection.ParameterMarker;

			MySqlCommand cmd = new MySqlCommand("SELECT " + outSelect, connection);
			MySqlDataReader reader = cmd.ExecuteReader();

			// since MySQL likes to return user variables as strings
			// we reset the types of the readers internal value objects
			// this will allow those value objects to parse the string based
			// return values
			for (int i = 0; i < reader.FieldCount; i++)
			{
				string fieldName = reader.GetName(i);
				fieldName = marker + fieldName.Remove(0, hash.Length + 1);
				reader.values[i] = MySqlField.GetIMySqlValue(parameters[fieldName].MySqlDbType, true);
			}

			reader.Read();
			for (int i = 0; i < reader.FieldCount; i++)
			{
				string fieldName = reader.GetName(i);
				fieldName = marker + fieldName.Remove(0, hash.Length + 1);
				parameters[fieldName].Value = reader.GetValue(i);
			}
			reader.Close();
		}
	}
}
