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
using System.Data;
using System.Globalization;
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

        public StoredProcedure(MySqlCommand cmd, string text)
            : base(cmd, text)
        {
            uint code = (uint) DateTime.Now.GetHashCode();
            hash = code.ToString();
        }

        private string GetReturnParameter()
        {
            if (Parameters != null)
                foreach (MySqlParameter p in Parameters)
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

        private DataSet GetParameters(string procName)
        {
            // if we can use mysql.proc, then do so
            if (Connection.Settings.UseProcedureBodies)
                return Connection.ProcedureCache.GetProcedure(Connection, procName);

            // we can't use mysql.proc so we attempt to "make do"
            DataSet ds = new DataSet();
            string[] restrictions = new string[4];
            int dotIndex = procName.IndexOf('.');
            restrictions[1] = procName.Substring(0, dotIndex++);
            restrictions[2] = procName.Substring(dotIndex, procName.Length - dotIndex);
            ds.Tables.Add(Connection.GetSchema("procedures", restrictions));

            // we use an internal method to create our procedure parameters table.  We pass
            // in a non-null routines table and this will prevent the code from attempting
            // a show create. It will process zero routine records but will return an empty
            // parameters table we can then fill.
            DataTable zeroRoutines = new DataTable();
            ISSchemaProvider sp = new ISSchemaProvider(Connection);
            DataTable pTable = sp.GetProcedureParameters(null, zeroRoutines);
            pTable.TableName = "procedure parameters";
            ds.Tables.Add(pTable);

            // now we run through the parameters that were set and fill in the parameters table
            // the best we can
            int pos = 1;
            foreach (MySqlParameter p in command.Parameters)
            {
                // in this mode, all parameters must have their type set
                if (!p.TypeHasBeenSet)
                    throw new InvalidOperationException(Resources.NoBodiesAndTypeNotSet);

                DataRow row = pTable.NewRow();
                row["PARAMETER_NAME"] = p.ParameterName;
                row["PARAMETER_MODE"] = "IN";
                if (p.Direction == ParameterDirection.InputOutput)
                    row["PARAMETER_MODE"] = "INOUT";
                else if (p.Direction == ParameterDirection.Output)
                    row["PARAMETER_MODE"] = "OUT";
                else if (p.Direction == ParameterDirection.ReturnValue)
                {
                    row["PARAMETER_MODE"] = "OUT";
                    row["ORDINAL_POSITION"] = 0;
                }
                else
                    row["ORDINAL_POSITION"] = pos++;
                pTable.Rows.Add(row);
            }
            return ds;
        }

        public override void Resolve()
        {
            // first retrieve the procedure definition from our
            // procedure cache
            string spName = commandText;
            if (spName.IndexOf(".") == -1)
                spName = Connection.Database + "." + spName;

            DataSet ds = GetParameters(spName);

            DataTable procTable = ds.Tables["procedures"];
            parametersTable = ds.Tables["procedure parameters"];

            StringBuilder sqlStr = new StringBuilder();
            StringBuilder setStr = new StringBuilder();
            outSelect = String.Empty;

            string retParm = GetReturnParameter();
            foreach (DataRow param in parametersTable.Rows)
            {
                if (param["ORDINAL_POSITION"].Equals(0)) continue;
                string mode = (string) param["PARAMETER_MODE"];
                string pName = (string) param["PARAMETER_NAME"];

                // make sure the parameters given to us have an appropriate
                // type set if it's not already
                MySqlParameter p = command.Parameters[pName];
                if (!p.TypeHasBeenSet)
                {
                    string datatype = (string) param["DATA_TYPE"];
                    bool unsigned = param["FLAGS"].ToString().IndexOf("UNSIGNED") != -1;
                    bool real_as_float = procTable.Rows[0]["SQL_MODE"].ToString().IndexOf("REAL_AS_FLOAT") != -1;
                    p.MySqlDbType = MetaData.NameToType(datatype, unsigned, real_as_float, Connection);
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
                    setStr.AppendFormat(CultureInfo.InvariantCulture, "SET {0}={1};", vName, pName);
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
                sqlCmd = setStr + sqlCmd;

            resolvedCommandText = sqlCmd;
        }

        public override void Close()
        {
            if (outSelect.Length == 0) return;

            char marker = Connection.ParameterMarker;

            MySqlCommand cmd = new MySqlCommand("SELECT " + outSelect, Connection);
            MySqlDataReader reader = cmd.ExecuteReader();

            // since MySQL likes to return user variables as strings
            // we reset the types of the readers internal value objects
            // this will allow those value objects to parse the string based
            // return values
            for (int i = 0; i < reader.FieldCount; i++)
            {
                string fieldName = reader.GetName(i);
                fieldName = marker + fieldName.Remove(0, hash.Length + 1);
                reader.values[i] = MySqlField.GetIMySqlValue(Parameters[fieldName].MySqlDbType);
            }

            reader.Read();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                string fieldName = reader.GetName(i);
                fieldName = marker + fieldName.Remove(0, hash.Length + 1);
                Parameters[fieldName].Value = reader.GetValue(i);
            }
            reader.Close();
        }
    }
}