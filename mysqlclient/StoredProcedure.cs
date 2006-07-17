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

namespace MySql.Data.MySqlClient
{
	/// <summary>
	/// Summary description for StoredProcedure.
	/// </summary>
	internal class StoredProcedure : Statement
	{
		private string			hash;
		private string			outSelect;

		public StoredProcedure(MySqlConnection connection, string text) : 
            base(connection, text)
		{
			uint code = (uint)DateTime.Now.GetHashCode();
			hash = code.ToString();
            this.connection = connection;
		}

		private string GetReturnParameter()
		{
			foreach (MySqlParameter p in parameters)
				if (p.Direction == ParameterDirection.ReturnValue)
					return hash + p.ParameterName;
			return null;
		}

		private string PrepareAsFunction(MySqlCommand cmd)
		{
			return null;
		}

        public override bool ExecuteNext()
        {
            bool returnVal = base.ExecuteNext();
            if (!returnVal)
                UpdateParameters();
            return returnVal;
        }

        protected override void BindParameters()
        {
            // first retrieve the procedure definition from our
            // procedure cache
            string spName = commandText;
            if (spName.IndexOf(".") == -1)
                spName = connection.Database + "." + spName;
            DataSet ds = connection.ProcedureCache.GetProcedure(connection, spName);

            DataTable procTable = ds.Tables["procedures"];
            DataTable paramTable = ds.Tables["procedure parameters"];

            string sqlStr = String.Empty;
            string setStr = String.Empty;

            string retParm = GetReturnParameter();
            foreach (DataRow param in paramTable.Rows)
            {
                if (param["ordinal_position"].Equals(0)) continue;
                string mode = (string)param["parameter_mode"];
                string name = (string)param["parameter_name"];
                string pName = connection.ParameterMarker + name;
                string vName = "@" + hash + name;

                sqlStr += pName + ", ";
                if (mode == "OUT")
				    outSelect += vName + ", ";
                else
                {
                    setStr += "SET " + vName + "=" + pName + ";";
                    outSelect += vName + ", ";
                }

            }

			sqlStr = sqlStr.TrimEnd(' ', ',');
			outSelect = outSelect.TrimEnd(' ', ',');
			if (procTable.Rows[0]["ROUTINE_TYPE"].Equals("PROCEDURE"))
				sqlStr = "call " + commandText + "(" + sqlStr + ")";
			else
			{
				sqlStr = "set @" + retParm + "=" + commandText + "(" + sqlStr + ")";
				outSelect = "@" + retParm;
			}
			if (setStr.Length > 0)
				sqlStr = setStr + sqlStr;
            commandText = sqlStr;

            // now call our base version
            base.BindParameters();
		}

		public void UpdateParameters()
		{
			if (outSelect.Length == 0) return;

			char marker = connection.ParameterMarker;

			MySqlCommand cmd = new MySqlCommand("SELECT " + outSelect, connection);
			MySqlDataReader reader = cmd.ExecuteReader();

/*			for (int i=0; i < reader.FieldCount; i++) 
			{
				string fieldName = reader.GetName(i);
				fieldName = marker + fieldName.Remove(0, hash.Length+1);
				reader.GetField(i) = MySqlField.GetIMySqlValue(parameters[fieldName].MySqlDbType, true);
                reader.fi
			}
*/
			reader.Read();
			for (int i=0; i < reader.FieldCount; i++)
			{
				string fieldName = reader.GetName(i);
				fieldName = marker + fieldName.Remove(0, hash.Length+1);
				parameters[fieldName].Value = reader.GetValue(i);
			}
			reader.Close();
		}
	}
}
