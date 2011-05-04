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
        private bool serverProvidingOutputParameters;

        // Prefix used for to generate inout or output parameters names
        internal const string ParameterPrefix = "_cnet_param_";

        public StoredProcedure(MySqlCommand cmd, string text)
            : base(cmd, text)
        {
        }

        private MySqlParameter GetReturnParameter()
        {
            if (Parameters != null)
                foreach (MySqlParameter p in Parameters)
                    if (p.Direction == ParameterDirection.ReturnValue)
                        return p;
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
            lock (ds)
            {
                proceduresTable = ds.Tables["procedures"];
                parametersTable = ds.Tables["procedure parameters"];
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

        private MySqlParameter GetAndFixParameter(string spName, DataRow param, bool realAsFloat, MySqlParameter returnParameter)
        {
            string mode = (string)param["PARAMETER_MODE"];
            string pName = (string)param["PARAMETER_NAME"];

            if (param["ORDINAL_POSITION"].Equals(0))
            {
                if (returnParameter == null)
                    throw new InvalidOperationException(
                        String.Format(Resources.RoutineRequiresReturnParameter, spName));
                pName = returnParameter.ParameterName;
            }

            // make sure the parameters given to us have an appropriate type set if it's not already
            MySqlParameter p = command.Parameters.GetParameterFlexible(pName, true);
            if (!p.TypeHasBeenSet)
            {
                string datatype = (string)param["DATA_TYPE"];
                bool unsigned = GetFlags(param["DTD_IDENTIFIER"].ToString()).IndexOf("UNSIGNED") != -1;
                p.MySqlDbType = MetaData.NameToType(datatype, unsigned, realAsFloat, Connection);
            }
            return p;
        }

        private MySqlParameterCollection CheckParameters(string spName)
        {
            MySqlParameterCollection newParms = new MySqlParameterCollection(command);
            MySqlParameter returnParameter = GetReturnParameter();

            DataTable procTable;
            GetParameters(spName, out procTable, out parametersTable);
            if (procTable.Rows.Count == 0)
                throw new InvalidOperationException(String.Format(Resources.RoutineNotFound, spName));

            bool realAsFloat = procTable.Rows[0]["SQL_MODE"].ToString().IndexOf("REAL_AS_FLOAT") != -1;

            foreach (DataRow param in parametersTable.Rows)
                newParms.Add(GetAndFixParameter(spName, param, realAsFloat, returnParameter));
            return newParms;
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

            MySqlParameter returnParameter = GetReturnParameter();

            MySqlParameterCollection parms = command.Connection.Settings.CheckParameters ?
                CheckParameters(spName) : Parameters;

            StringBuilder setSql = new StringBuilder();
            StringBuilder callSql = new StringBuilder();
            StringBuilder selectSql = new StringBuilder();
            string callDelimiter = String.Empty;
            string selectDelimiter = String.Empty;
            serverProvidingOutputParameters = Driver.SupportsOutputParameters && preparing;

            foreach (MySqlParameter p in parms)
            {
                if (p.Direction == ParameterDirection.ReturnValue) continue;

                string pName = p.ParameterName;
                if (!pName.StartsWith("@") && !pName.StartsWith("?"))
                    pName = "@" + pName;
                string parameterName = pName;
                if (p.Direction != ParameterDirection.Input && !serverProvidingOutputParameters)
                {
                    pName = String.Format("@{0}{1}", ParameterPrefix, p.BaseName);
                    if (p.Direction == ParameterDirection.InputOutput)
                        setSql.AppendFormat(CultureInfo.InvariantCulture, "SET {0}={1};", pName, parameterName);
                    selectSql.AppendFormat(CultureInfo.InvariantCulture, "{0}{1}", selectDelimiter, pName);
                    selectDelimiter = ", ";
                }
                callSql.AppendFormat(CultureInfo.InvariantCulture, "{0}{1}", callDelimiter, pName);
                callDelimiter = ", ";            
            }

            string sqlCmd = String.Empty;

            if (returnParameter == null)
                sqlCmd = String.Format("CALL {0} ({1})", spName, callSql.ToString());
            else
            {
                string returnParameterName = returnParameter.BaseName;
                if (String.IsNullOrEmpty(returnParameterName))
                    returnParameterName = "dummy";

                sqlCmd = String.Format("SET @{0}{1}={2}({3})", ParameterPrefix, returnParameterName, spName, callSql.ToString());
                selectSql.AppendFormat(CultureInfo.InvariantCulture, 
                    "{0}@{1}{2}", selectSql.ToString(), ParameterPrefix, returnParameterName);
            }
            if (setSql.Length > 0)
                sqlCmd = String.Format("{0}{1}", setSql.ToString(), sqlCmd);
            if (selectSql.Length > 0)
                sqlCmd = String.Format("{0}; SELECT {1}", sqlCmd, selectSql.ToString());

            resolvedCommandText = sqlCmd;
        }
	}
}
