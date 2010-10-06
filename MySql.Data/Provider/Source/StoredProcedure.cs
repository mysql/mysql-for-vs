// Copyright (c) 2004-2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
//
// MySQL Connector/NET is licensed under the terms of the GPLv2
// <http://www.gnu.org/licenses/old-licenses/gpl-2.0.html>, like most 
// MySQL Connectors. There are special exceptions to the terms and 
// conditions of the GPLv2 as it is applied to this software, see the 
// FLOSS License Exception
// <http://www.mysql.com/about/legal/licensing/foss-exception.html>.
//
// This program is free software; you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published 
// by the Free Software Foundation; version 2 of the License.
//
// This program is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
//
// You should have received a copy of the GNU General Public License along 
// with this program; if not, write to the Free Software Foundation, Inc., 
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

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

        private MySqlParameter GetAndFixParameter(DataRow param, bool realAsFloat, string returnParameter)
        {
            string mode = (string)param["PARAMETER_MODE"];
            string pName = (string)param["PARAMETER_NAME"];

            if (param["ORDINAL_POSITION"].Equals(0))
                pName = returnParameter;

            if (pName == null) return null;

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
            if (spName.IndexOf(".") == -1 && !String.IsNullOrEmpty(Connection.Database))
                spName = Connection.Database + "." + spName;
            spName = FixProcedureName(spName);

            DataTable procTable;
            GetParameters(spName,out procTable, out parametersTable);

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
                MySqlParameter p = GetAndFixParameter(param, realAsFloat, retParm);
                if (p == null) continue;

                if (param["ORDINAL_POSITION"].Equals(0))
                    continue;

                string baseName = p.ParameterName;
                string pName = baseName;
                if (baseName.StartsWith("@") || baseName.StartsWith("?"))
                    baseName = baseName.Substring(1);
                else
                    pName = "@" + pName;

                string inputVar = pName;
                if (p.Direction != ParameterDirection.Input &&
                    !(Connection.driver.SupportsOutputParameters || preparing))
                {
                    // set a user variable to our current value
                    string sql = String.Format("SET @{0}{1}={2}", ParameterPrefix, baseName, pName);
                    MySqlCommand cmd = new MySqlCommand(sql, Connection);
                   
                    cmd.Parameters.Add(p);
                    cmd.ExecuteNonQuery();

                    inputVar = String.Format("@{0}{1}", ParameterPrefix, baseName);

                    outSql.AppendFormat(CultureInfo.InvariantCulture, "{0}{1}", outDelimiter, inputVar);
                    outDelimiter = ", ";
                }
                sqlStr.AppendFormat(CultureInfo.InvariantCulture, "{0}{1}", sqlDelimiter, inputVar);
                sqlDelimiter = ", ";
            }

            string sqlCmd = sqlStr.ToString().TrimEnd(' ', ',');
            outSelect = outSql.ToString().TrimEnd(' ', ',');

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

            resolvedCommandText = sqlCmd;
        }

        private MySqlDataReader GetHackedOuputParameters()
        {
            if (outSelect.Length == 0) return null;

            MySqlCommand cmd = new MySqlCommand("SELECT " + outSelect, Connection);

            MySqlDataReader reader = cmd.ExecuteReader();
            // since MySQL likes to return user variables as strings
            // we reset the types of the readers internal value objects
            // this will allow those value objects to parse the string based
            // return values
            ResultSet results = reader.ResultSet;
            for (int i = 0; i < reader.FieldCount; i++)
            {
                string fieldName = reader.GetName(i);
                fieldName = fieldName.Remove(0, ParameterPrefix.Length + 1);
                MySqlParameter parameter = Parameters.GetParameterFlexible(fieldName, true);

                IMySqlValue v = MySqlField.GetIMySqlValue(parameter.MySqlDbType);
                if (v is MySqlBit)
                {
                    MySqlBit bit = (MySqlBit)v;
                    bit.ReadAsString = true;
                    results.SetValueObject(i, bit);
                }
                else
                    results.SetValueObject(i, v);
            }
            if (!reader.Read())
            {
                reader.Close();
                return null;
            }
            return reader;
        }

		public override void Close(MySqlDataReader reader)
		{
            base.Close(reader);

            ResultSet rs = reader.ResultSet;
            // if our closing reader doesn't have output parameters then we may have to
            // use the user variable hack
            if (rs == null || !rs.IsOutputParameters)
            {
                MySqlDataReader rdr = GetHackedOuputParameters();
                if (rdr == null) return;
                reader = rdr;
            }

            using (reader)
            {
                string prefix = "@" + ParameterPrefix;

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string fieldName = reader.GetName(i);
                    if (fieldName.StartsWith(prefix))
                        fieldName = fieldName.Remove(0, prefix.Length);
                    MySqlParameter parameter = Parameters.GetParameterFlexible(fieldName, true);
                    parameter.Value = reader.GetValue(i);
                }
                reader.Close();
            }
		}
	}
}
