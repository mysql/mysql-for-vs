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

        public StoredProcedure(MySqlCommand cmd, string text)
            : base(cmd, text)
        {
            // set our parameter hash to something very unique
            uint code = (uint) DateTime.Now.GetHashCode();
            cmd.parameterHash = code.ToString();
        }

        private string GetReturnParameter()
        {
            if (Parameters != null)
                foreach (MySqlParameter p in Parameters)
                    if (p.Direction == ParameterDirection.ReturnValue)
                    {
                        string pName = p.ParameterName.Substring(1);
                        return command.parameterHash + pName;
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
            //if (Connection.Settings.UseProcedureBodies)
            DataSet ds = Connection.ProcedureCache.GetProcedure(Connection, procName);

            // if we got both proc and parameter data then just return
            if (ds.Tables.Count == 2) return ds;

            // we were not able to retrieve parameter data so we have to make do by
            // adding the parameters from the command object to our table
            // we use an internal method to create our procedure parameters table.  
            ISSchemaProvider sp = new ISSchemaProvider(Connection);
            DataTable pTable = sp.CreateParametersTable(); 
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

        private MySqlParameter GetAndFixParameter(DataRow param, bool realAsFloat)
        {
            if (param["ORDINAL_POSITION"].Equals(0)) return null;

            string mode = (string)param["PARAMETER_MODE"];
            string pName = (string)param["PARAMETER_NAME"];

            // make sure the parameters given to us have an appropriate
            // type set if it's not already
            MySqlParameter p = command.Parameters.GetParameterFlexible(pName, true);
            if (!p.TypeHasBeenSet)
            {
                string datatype = (string)param["DATA_TYPE"];
                bool unsigned = GetFlags(param["DTD_IDENTIFIER"].ToString()).IndexOf("UNSIGNED") != -1;
                p.MySqlDbType = MetaData.NameToType(datatype, unsigned, realAsFloat, Connection);
            }
            return p;
        }

        public override void Resolve(bool preparing)
        {
            // check to see if we are already resolved
            if (resolvedCommandText != null) return;

            // first retrieve the procedure definition from our
            // procedure cache
            string spName = commandText;
            string parameterHash = command.parameterHash;
            if (spName.IndexOf(".") == -1 && !String.IsNullOrEmpty(Connection.Database))
                spName = Connection.Database + "." + spName;
            spName = FixProcedureName(spName);

            DataSet ds = GetParameters(spName);

            DataTable procTable = ds.Tables["procedures"];
            parametersTable = ds.Tables["procedure parameters"];

            if (procTable.Rows.Count == 0)
                throw new InvalidOperationException(String.Format(Resources.RoutineNotFound, spName));

            bool realAsFloat = procTable.Rows[0]["SQL_MODE"].ToString().IndexOf("REAL_AS_FLOAT") != -1;
            StringBuilder sqlStr = new StringBuilder();
            StringBuilder outSql = new StringBuilder();
            string sqlDelimiter = "";
            string outDelimiter = "";

            string retParm = GetReturnParameter();
            foreach (DataRow param in parametersTable.Rows)
            {
                MySqlParameter p = GetAndFixParameter(param, realAsFloat);

                string baseName = p.ParameterName;
                if (baseName.StartsWith("@") || baseName.StartsWith("?"))
                    baseName = baseName.Substring(1);

                // if input then we just send the parameter normally
                if (p.Direction == ParameterDirection.Input || 
                    (Connection.driver.SupportsOutputParameters && preparing))
                {
                    sqlStr.AppendFormat(CultureInfo.InvariantCulture, "{0}@{1}", sqlDelimiter, baseName);
                    sqlDelimiter = ", ";
                }
                else
                {
                    Connection.driver.ExecuteDirect(String.Format(
                        "SET @{0}{1}={2}", parameterHash, baseName,
                        p.Direction == ParameterDirection.Output ? "NULL" : p.Value.ToString()));
                    outSql.AppendFormat(CultureInfo.InvariantCulture, "{0}@{1}{2}", outDelimiter, parameterHash, baseName);
                    outDelimiter = ", ";
                }
            }

            string sqlCmd = sqlStr.ToString().TrimEnd(' ', ',');
            outSelect = outSql.ToString().TrimEnd(' ', ',');

            if (procTable.Rows[0]["ROUTINE_TYPE"].Equals("PROCEDURE"))
                sqlCmd = String.Format("call {0} ({1})", spName, sqlCmd);
            else
            {
                if (retParm == null)
                    retParm = parameterHash + "dummy";
                else
                    outSelect = String.Format("@{0}", retParm);
                sqlCmd = String.Format("SET @{0}={1}({2})", retParm, spName, sqlCmd);
            }

            resolvedCommandText = sqlCmd;
        }

        private MySqlDataReader GetHackedOuputParameters()
        {
            if (outSelect.Length == 0) return null;

            MySqlCommand cmd = new MySqlCommand("SELECT " + outSelect, Connection);

            // set the parameter hash for this new command to our current parameter hash
            // so the inout and out parameters won't cause a problem
            string parameterHash = command.parameterHash;
            cmd.parameterHash = parameterHash;

            MySqlDataReader reader = cmd.ExecuteReader();
            // since MySQL likes to return user variables as strings
            // we reset the types of the readers internal value objects
            // this will allow those value objects to parse the string based
            // return values
            ResultSet results = reader.ResultSet;
            for (int i = 0; i < reader.FieldCount; i++)
            {
                string fieldName = reader.GetName(i);
                fieldName = fieldName.Remove(0, parameterHash.Length + 1);
                MySqlParameter parameter = Parameters.GetParameterFlexible(fieldName, true);
                results.SetValueObject(i, MySqlField.GetIMySqlValue(parameter.MySqlDbType));
            }
            if (!reader.Read()) return null;
            return reader;
        }

		public override void Close(MySqlDataReader reader)
		{
            base.Close(reader);

            // if our closing reader doesn't have output parameters then we may have to
            // use the user variable hack
            if (!reader.ResultSet.HasOutputParameters)
            {
                MySqlDataReader rdr = GetHackedOuputParameters();
                if (rdr == null) return;
                reader = rdr;
            }

            for (int i = 0; i < reader.FieldCount; i++)
            {
                string fieldName = reader.GetName(i);
                if (fieldName.StartsWith(command.parameterHash))
                    fieldName = fieldName.Remove(0, command.parameterHash.Length + 1);
                MySqlParameter parameter = Parameters.GetParameterFlexible(fieldName, true);
                parameter.Value = reader.GetValue(i);
            }
		}
	}
}
