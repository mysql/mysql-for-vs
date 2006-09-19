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
        private DataTable parametersTable;

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

        protected override void BindParameters()
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
                string name = (string)param["PARAMETER_NAME"];
                string datatype = (string)param["DATA_TYPE"];

                // make sure the parameters given to us have an appropriate
                // type set if it's not already
                MySqlParameter p = parameters[name];
                if (!p.TypeHasBeenSet)
                    p.MySqlDbType = GetTypeFromName(datatype,
                        param["FLAGS"].ToString().IndexOf("UNSIGNED") != -1,
                        procTable.Rows[0]["SQL_MODE"].ToString().IndexOf("REAL_AS_FLOAT") != -1);

                string pName = String.Format("{0}{1}",
                    connection.ParameterMarker, name);
                string vName = string.Format("@{0}{1}", hash, name);

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
                sqlCmd = String.Format("set @{0}={1} ({2})", retParm,
                    commandText, sqlCmd);
                outSelect = String.Format("@{0}", retParm);
            }

			if (setStr.Length > 0)
				sqlCmd = setStr.ToString() + sqlCmd;
            string oldCmdText = commandText;

            // now call our base version
            // we save our original command text because we are about to
            // overwrite it.  We restore it once our base class gets done
            // binding the parameters
            commandText = sqlCmd;
            base.BindParameters();
            commandText = oldCmdText;
		}

        private MySqlDbType GetTypeFromName(string typeName, bool unsigned, 
            bool realAsFloat)
        {
            switch (typeName)
            {
                case "char": return MySqlDbType.String;
                case "varchar": return MySqlDbType.VarChar;
                case "date": return MySqlDbType.Date;
                case "datetime": return MySqlDbType.Datetime;
                case "numeric":
                case "decimal":
                case "dec":
                case "fixed":
                    if (connection.driver.Version.isAtLeast(5, 0, 3))
                        return MySqlDbType.NewDecimal;
                    else
                        return MySqlDbType.Decimal;
                case "year":
                    return MySqlDbType.Year;
                case "time":
                    return MySqlDbType.Time;
                case "timestamp":
                    return MySqlDbType.Timestamp;
                case "set": return MySqlDbType.Set;
                case "enum": return MySqlDbType.Enum;
                case "bit": return MySqlDbType.Bit;

                case "tinyint":
                case "bool":
                case "boolean":
                    return MySqlDbType.Byte;
                case "smallint":
                    return unsigned ? MySqlDbType.UInt16 : MySqlDbType.Int16;
                case "mediumint":
                    return unsigned ? MySqlDbType.UInt24 : MySqlDbType.Int24;
                case "int":
                case "integer":
                    return unsigned ? MySqlDbType.UInt32 : MySqlDbType.Int32;
                case "serial":
                    return MySqlDbType.UInt64;
                case "bigint":
                    return unsigned ? MySqlDbType.UInt64 : MySqlDbType.Int64;
                case "float": return MySqlDbType.Float;
                case "double": return MySqlDbType.Double;
                case "real": return
                     realAsFloat ? MySqlDbType.Float : MySqlDbType.Double;
                case "blob":
                case "text":
                    return MySqlDbType.Blob;
                case "longblob":
                case "longtext":
                    return MySqlDbType.LongBlob;
                case "mediumblob":
                case "mediumtext":
                    return MySqlDbType.MediumBlob;
                case "tinyblob":
                case "tinytext":
                    return MySqlDbType.TinyBlob;
            }
            throw new MySqlException("Unhandled type encountered");
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
			for (int i=0; i < reader.FieldCount; i++) 
			{
				string fieldName = reader.GetName(i);
				fieldName = marker + fieldName.Remove(0, hash.Length+1);
                reader.values[i] = MySqlField.GetIMySqlValue(parameters[fieldName].MySqlDbType, true);
			}

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
