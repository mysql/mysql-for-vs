// Copyright © 2004,2010, Oracle and/or its affiliates.  All rights reserved.
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
using MySql.Data.MySqlClient.Properties;

namespace MySql.Data.MySqlClient
{
    /// <summary>
    /// Summary description for StoredProcedure.
    /// </summary>
    internal class StoredProcedure : PreparableStatement
    {
        private string outSelect;
        private DataTable parametersTable;
        private string resolvedCommandText;

        // Prefix used for to generate inout or output parameters names
        internal const string ParameterPrefix = "_cnet_param_";

        public StoredProcedure(MySqlCommand cmd, string text)
            : base(cmd, text)
        {
        }

        private string GetReturnParameter()
        {
            if (Parameters != null)
                foreach (MySqlParameter p in Parameters)
                    if (p.Direction == ParameterDirection.ReturnValue)
                        return p.ParameterName.Substring(1);
            return null;
        }

        public override string ResolvedCommandText
        {
            get { return resolvedCommandText; }
        }

        internal string GetCacheKey(string spName)
        {
            string retValue = String.Empty;
            StringBuilder key = new StringBuilder(spName);
            key.Append("(");
            string delimiter = "";
            foreach (MySqlParameter p in command.Parameters)
            {
                if (p.Direction == ParameterDirection.ReturnValue)
                    retValue = "?=";
                else
                {
                    key.AppendFormat(CultureInfo.InvariantCulture, "{0}?", delimiter);
                    delimiter = ",";
                }
            }
            key.Append(")");
            return retValue + key.ToString();
        }

        private void GetParameters(string procName, out DataTable proceduresTable,
            out DataTable parametersTable)
        {
            string procCacheKey = GetCacheKey(procName);
            DataSet ds = Connection.ProcedureCache.GetProcedure(Connection, procName, procCacheKey);

            if(ds.Tables.Count == 2)
            {
                // if we got our parameters and our user says it is ok to use proc bodies
                // then just return them
                if (Connection.Settings.UseProcedureBodies)
                {
                    lock(ds)
                    {
                        proceduresTable = ds.Tables["procedures"];
                        parametersTable = ds.Tables["procedure parameters"];
                        return;
                    }
                }
            }

             lock(ds)
             {
                proceduresTable = ds.Tables["procedures"];
             }
            // we were not able to retrieve parameter data so we have to make do by
            // adding the parameters from the command object to our table
            // we use an internal method to create our procedure parameters table.  
            ISSchemaProvider sp = new ISSchemaProvider(Connection);
            parametersTable = sp.CreateParametersTable(); 

            // now we run through the parameters that were set and fill in the parameters table
            // the best we can
            int pos = 1;
            foreach (MySqlParameter p in command.Parameters)
            {
                // in this mode, all parameters must have their type set
                if (!p.TypeHasBeenSet)
                    throw new InvalidOperationException(Resources.NoBodiesAndTypeNotSet);

                DataRow row = parametersTable.NewRow();
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
                parametersTable.Rows.Add(row);
            }
            if (Connection.Settings.UseProcedureBodies)
            {
                lock (ds)
                {
                    // we got the parameters, but ignore them.
                    if (ds.Tables.Contains("Procedure Parameters"))
                        ds.Tables.Remove("Procedure Parameters");

                    ds.Tables.Add(parametersTable);
                }
            }
        }

        public static string GetFlags(string dtd)
        {
            int x = dtd.Length - 1;
            while (x > 0 && (Char.IsLetterOrDigit(dtd[x]) || dtd[x] == ' '))
                x--;
            return dtd.Substring(x).ToUpper(CultureInfo.InvariantCulture);
        }

        private string FixProcedureName(string name)
        {
            string[] parts = name.Split('.');
            for (int i = 0; i < parts.Length; i++)
                if (!parts[i].StartsWith("`"))
                    parts[i] = String.Format("`{0}`", parts[i]);
            if (parts.Length == 1) return parts[0];
            return String.Format("{0}.{1}", parts[0], parts[1]);
        }

        public override void Resolve()
        {
            // first retrieve the procedure definition from our
            // procedure cache
            string spName = commandText;
            if (spName.IndexOf(".") == -1 && !String.IsNullOrEmpty(Connection.Database))
                spName = Connection.Database + "." + spName;
            spName = FixProcedureName(spName);

            DataTable procTable;
            GetParameters(spName,out procTable, out parametersTable);

            if (procTable.Rows.Count == 0)
                throw new InvalidOperationException(String.Format(Resources.RoutineNotFound, spName));

            StringBuilder sqlStr = new StringBuilder();
            StringBuilder setStr = new StringBuilder();
            outSelect = String.Empty;

            string retParm = GetReturnParameter();
            foreach (DataRow param in parametersTable.Rows)
            {
                string mode = (string) param["PARAMETER_MODE"];
                string pName = (string) param["PARAMETER_NAME"];

                if (param["ORDINAL_POSITION"].Equals(0))
                    pName = retParm;

                if (pName != null)
                {
                    // make sure the parameters given to us have an appropriate
                    // type set if it's not already
                    MySqlParameter p = command.Parameters.GetParameterFlexible(pName, true);
                    if (!p.TypeHasBeenSet)
                    {
                        string datatype = (string)param["DATA_TYPE"];
                        bool unsigned = GetFlags(param["DTD_IDENTIFIER"].ToString()).IndexOf("UNSIGNED") != -1;
                        bool real_as_float = procTable.Rows[0]["SQL_MODE"].ToString().IndexOf("REAL_AS_FLOAT") != -1;
                        p.MySqlDbType = MetaData.NameToType(datatype, unsigned, real_as_float, Connection);
                    }

                    if (param["ORDINAL_POSITION"].Equals(0)) continue;

                    string basePName = pName;
                    if (pName.StartsWith("@") || pName.StartsWith("?"))
                        basePName = pName.Substring(1);
                    string vName = string.Format("@{0}{1}", ParameterPrefix, basePName);

                    // if our parameter doesn't have a leading marker then we need to give it one
                    pName = p.ParameterName;
                    if (!pName.StartsWith("@") && !pName.StartsWith("?"))
                        pName = "@" + pName;

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
            }

            string sqlCmd = sqlStr.ToString().TrimEnd(' ', ',');
            outSelect = outSelect.TrimEnd(' ', ',');
            if (procTable.Rows[0]["ROUTINE_TYPE"].Equals("PROCEDURE"))
                sqlCmd = String.Format("call {0} ({1})", spName, sqlCmd);
            else
            {
                if (retParm == null)
                    retParm = ParameterPrefix + "dummy";
                else
                    outSelect = String.Format("@{0}{1}", ParameterPrefix, retParm);
                sqlCmd = String.Format("SET @{0}{1}={2}({3})", ParameterPrefix, retParm, spName, sqlCmd);
            }

            if (setStr.Length > 0)
                sqlCmd = setStr + sqlCmd;

            resolvedCommandText = sqlCmd;
        }

		public override void Close()
		{
            base.Close();

			if (outSelect.Length == 0) return;

            MySqlCommand cmd = new MySqlCommand("SELECT " + outSelect, Connection);

            // Read output parameters
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                // since MySQL likes to return user variables as strings
                // we reset the types of the readers internal value objects
                // this will allow those value objects to parse the string based
                // return values
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string fieldName = reader.GetName(i);
                    fieldName = fieldName.Remove(0, ParameterPrefix.Length + 1);
                    MySqlParameter parameter = Parameters.GetParameterFlexible(fieldName, true);
                    IMySqlValue v = MySqlField.GetIMySqlValue(parameter.MySqlDbType);
                    reader.values[i] = v;
                    if (v is MySqlBit)
                    {
                        MySqlBit bit = (MySqlBit)v;
                        bit.ReadAsString = true;
                        reader.values[i] = bit;
                    }
                }

                if (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string fieldName = reader.GetName(i);
                        fieldName = fieldName.Remove(0, ParameterPrefix.Length + 1);
                        MySqlParameter parameter = Parameters.GetParameterFlexible(fieldName, true);
                        parameter.Value = reader.GetValue(i);
                    }
                }
            }
		}
	}
}
