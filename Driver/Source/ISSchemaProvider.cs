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
using System.Text;
using MySql.Data.Common;
using System.Globalization;
using System.Diagnostics;
using System.Collections;
using System.Data.SqlTypes;

namespace MySql.Data.MySqlClient
{
    internal class ISSchemaProvider : SchemaProvider
    {
        public ISSchemaProvider(MySqlConnection connection) : base(connection)
        {
        }

        protected override DataTable GetCollections()
        {
           DataTable dt = base.GetCollections();

           object[][] collections = new object[][] 
            {
                new object[] {"Views", 2, 3},
                new object[] {"ViewColumns", 3, 4},
                new object[] {"Procedure Parameters", 5, 1},
                new object[] {"Procedures", 4, 3},
                new object[] {"Triggers", 2, 4}
            };

            FillTable(dt, collections);
            return dt;
        }

        protected override DataTable GetRestrictions()
        {
            DataTable dt = base.GetRestrictions();

            object[][] restrictions = new object[][] 
            {
                new object[] {"Procedure Parameters", "Catalog", "", 0},
                new object[] {"Procedure Parameters", "Owner", "", 1},
                new object[] {"Procedure Parameters", "Name", "", 2},
                new object[] {"Procedure Parameters", "Type", "", 3},
                new object[] {"Procedure Parameters", "Parameter", "", 4},
                new object[] {"Procedures", "Catalog", "", 0},
                new object[] {"Procedures", "Owner", "", 1},
                new object[] {"Procedures", "Name", "", 2},
                new object[] {"Procedures", "Type", "", 3},
                new object[] {"Views", "Catalog", "", 0},
                new object[] {"Views", "Owner", "", 1},
                new object[] {"Views", "Table", "", 2},
                new object[] {"ViewColumns", "Catalog", "", 0},
                new object[] {"ViewColumns", "Owner", "", 1},
                new object[] {"ViewColumns", "Table", "", 2},
                new object[] {"ViewColumns", "Column", "", 3},
                new object[] {"Triggers", "Catalog", "", 0},
                new object[] {"Triggers", "Schema", "", 1},
                new object[] {"Triggers", "Name", "", 2},
                new object[] {"Triggers", "EventObjectTable", "", 3},
            };
            FillTable(dt, restrictions);
            return dt;
        }

        public override DataTable GetDatabases(string[] restrictions)
        {
            string[] keys = new string[1];
            keys[0] = "SCHEMA_NAME";
            DataTable dt = Query("SCHEMATA", "", keys, restrictions);
            dt.Columns[1].ColumnName = "database_name";
            dt.TableName = "Databases";
            return dt;
        }

        public override DataTable GetTables(string[] restrictions)
        {
            string[] keys = new string[4];
            keys[0] = "TABLE_CATALOG";
            keys[1] = "TABLE_SCHEMA";
            keys[2] = "TABLE_NAME";
            keys[3] = "TABLE_TYPE";
            DataTable dt = Query("TABLES", "TABLE_TYPE != 'VIEW'", keys, restrictions);
            dt.TableName = "Tables";
            return dt;
        }

        public override DataTable GetColumns(string[] restrictions)
        {
            string[] keys = new string[4];
            keys[0] = "TABLE_CATALOG";
            keys[1] = "TABLE_SCHEMA";
            keys[2] = "TABLE_NAME";
            keys[3] = "COLUMN_NAME";
            DataTable dt = Query("COLUMNS", null, keys, restrictions);
            dt.Columns.Remove("CHARACTER_OCTET_LENGTH");
            dt.TableName = "Columns";
            return dt;
        }

        private DataTable GetViews(string[] restrictions)
        {
            string[] keys = new string[3];
            keys[0] = "TABLE_CATALOG";
            keys[1] = "TABLE_SCHEMA";
            keys[2] = "TABLE_NAME";
            DataTable dt = Query("VIEWS", null, keys, restrictions);
            dt.TableName = "Views";
            return dt;
        }

        private DataTable GetViewColumns(string[] restrictions)
        {
            StringBuilder where = new StringBuilder();
            StringBuilder sql = new StringBuilder(
                "SELECT C.* FROM information_schema.columns C");
            sql.Append(" JOIN information_schema.views V ");
            sql.Append("ON C.table_schema=V.table_schema AND C.table_name=V.table_name ");
            if (restrictions != null && restrictions.Length >= 2 &&
                restrictions[1] != null)
                where.AppendFormat(CultureInfo.InvariantCulture, "C.table_schema='{0}' ", restrictions[1]);
            if (restrictions != null && restrictions.Length >= 3 &&
                restrictions[2] != null)
            {
                if (where.Length > 0)
                    where.Append("AND ");
                where.AppendFormat(CultureInfo.InvariantCulture, "C.table_name='{0}' ", restrictions[2]);
            }
            if (restrictions != null && restrictions.Length == 4 &&
                restrictions[3] != null)
            {
                if (where.Length > 0)
                    where.Append("AND ");
                where.AppendFormat(CultureInfo.InvariantCulture, "C.column_name='{0}' ", restrictions[3]);
            }
            if (where.Length > 0)
                sql.AppendFormat(CultureInfo.InvariantCulture, " WHERE {0}", where.ToString());
            DataTable dt = GetTable(sql.ToString());
            dt.TableName = "ViewColumns";
            dt.Columns[0].ColumnName = "VIEW_CATALOG";
            dt.Columns[1].ColumnName = "VIEW_SCHEMA";
            dt.Columns[2].ColumnName  = "VIEW_NAME";
            return dt;
        }

        private DataTable GetTriggers(string[] restrictions)
        {
            string[] keys = new string[4];
            keys[0] = "TRIGGER_CATALOG";
            keys[1] = "TRIGGER_SCHEMA";
            keys[2] = "EVENT_OBJECT_TABLE";
            keys[3] = "TRIGGER_NAME";
            DataTable dt = Query("TRIGGERS", null, keys, restrictions);
            dt.TableName = "Triggers";
            return dt;
        }

        /// <summary>
        /// Return schema information about procedures and functions
        /// Restrictions supported are:
        /// schema, name, type
        /// </summary>
        /// <param name="restrictions"></param>
        /// <returns></returns>
        private DataTable GetProcedures(string[] restrictions)
        {
            string[] keys = new string[4];
            keys[0] = "ROUTINE_CATALOG";
            keys[1] = "ROUTINE_SCHEMA";
            keys[2] = "ROUTINE_NAME";
            keys[3] = "ROUTINE_TYPE";
            DataTable dt = Query("ROUTINES", null, keys, restrictions);
            dt.TableName = "Procedures";
            return dt;
        }

        /// <summary>
        /// Return schema information about parameters for procedures and functions
        /// Restrictions supported are:
        /// schema, name, type, parameter name
        /// </summary>
        public virtual DataTable GetProcedureParameters(string[] restrictions,
            DataTable routines)
        {
            DataTable dt = new DataTable("Procedure Parameters");
            dt.Columns.Add("ROUTINE_CATALOG", typeof(string));
            dt.Columns.Add("ROUTINE_SCHEMA", typeof(string));
            dt.Columns.Add("ROUTINE_NAME", typeof(string));
            dt.Columns.Add("ROUTINE_TYPE", typeof(string));
            dt.Columns.Add("PARAMETER_NAME", typeof(string));
            dt.Columns.Add("ORDINAL_POSITION", typeof(Int32));
            dt.Columns.Add("PARAMETER_MODE", typeof(string));
            dt.Columns.Add("IS_RESULT", typeof(string));
            dt.Columns.Add("DATA_TYPE", typeof(string));
            dt.Columns.Add("FLAGS", typeof(string));
            dt.Columns.Add("CHARACTER_SET", typeof(string));
            dt.Columns.Add("CHARACTER_MAXIMUM_LENGTH", typeof(Int32));
            dt.Columns.Add("NUMERIC_PRECISION", typeof(byte));
            dt.Columns.Add("NUMERIC_SCALE", typeof(Int32));

            try
            {
                GetParametersFromShowCreate(dt, restrictions, routines);
            }
            catch (Exception)
            {
                throw;
            }

            return dt;
        }

        protected override DataTable GetSchemaInternal(string collection, string[] restrictions)
        {
            DataTable dt = base.GetSchemaInternal(collection, restrictions);
            if (dt != null)
                return dt;

            switch (collection)
            {
                case "views":
                    return GetViews(restrictions);
                case "procedures":
                    return GetProcedures(restrictions);
                case "procedure parameters":
                    return GetProcedureParameters(restrictions, null);
                case "triggers":
                    return GetTriggers(restrictions);
                case "viewcolumns":
                    return GetViewColumns(restrictions);
            }
            return null;
        }

        private DataTable Query(string table_name, string initial_where,
            string[] keys, string[] values)
        {
            StringBuilder where = new StringBuilder(initial_where);
            StringBuilder query = new StringBuilder("SELECT * FROM INFORMATION_SCHEMA.");
            query.Append(table_name);

            if (values != null)
                for (int i = 0; i < keys.Length; i++)
                {
                    if (i >= values.Length) break;
                    if (values[i] == null || values[i] == String.Empty) continue;
                    if (where.Length > 0)
                        where.Append(" AND ");
                    where.AppendFormat(CultureInfo.InvariantCulture, "{0}='{1}'", keys[i], values[i]);
                }

            if (where.Length > 0)
                query.AppendFormat(CultureInfo.InvariantCulture, " WHERE {0}", where.ToString());

            return GetTable(query.ToString());
        }

        private DataTable GetTable(string sql)
        {
            try
            {
                DataTable table = new DataTable();
                MySqlDataAdapter da = new MySqlDataAdapter(sql, connection);
                da.Fill(table);
                return table;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region Procedures Support Rouines

/*        private void GetParametersFromMySqlProc(DataTable parametersTable,
            string[] restrictions)
        {
            StringBuilder baseQuery = new StringBuilder(
                "SELECT * FROM mysql.proc");
            StringBuilder where = new StringBuilder();

            if (restrictions[1] != null && restrictions[1].Length != 0)
                where.AppendFormat("db = _latin1 '{0}'", restrictions[1]);
            if (restrictions[2] != null && restrictions[2].Length != 0)
            {
                if (where.Length > 0)
                    where.Append(" AND ");
                where.AppendFormat("name = _latin1 '{0}'", restrictions[2]);
            }

            if (where.Length > 0)
            {
                baseQuery.Append(" WHERE ");
                baseQuery.Append(where);
            }

            MySqlCommand cmd = new MySqlCommand(baseQuery.ToString(), connection);
            MySqlDataReader reader = null;
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ProcessParameterList(parametersTable, reader.GetString(0),
                        reader.GetString(1),
                        reader.GetString(reader.GetOrdinal("param_list")),
                        reader.GetString(reader.GetOrdinal("returns")));
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }
        */
        internal void GetParametersFromShowCreate(DataTable parametersTable,
            string[] restrictions, DataTable routines)
        {
            // this allows us to pass in a pre-populated routines table
            // and avoid the querying for them again.
            // we use this when calling a procedure or function
            if (routines == null)
                routines = GetSchema("procedures", restrictions);

            MySqlCommand cmd = connection.CreateCommand();
            MySqlDataReader reader = null;

            foreach (DataRow routine in routines.Rows)
            {
                string showCreateSql = String.Format("SHOW CREATE {0} {1}.{2}",
                    routine["ROUTINE_TYPE"], routine["ROUTINE_SCHEMA"],
                    routine["ROUTINE_NAME"]);
                cmd.CommandText = showCreateSql;
                try
                {
                    string nameToRestrict = null;
                    if (restrictions != null && restrictions.Length == 5 &&
                        restrictions[4] != null)
                        nameToRestrict = restrictions[4];
                    reader = cmd.ExecuteReader();
                    reader.Read();
                    ParseProcedureBody(parametersTable, reader.GetString(2),
                        routine, nameToRestrict);
                }
                catch (SqlNullValueException snex)
                {
                    throw new InvalidOperationException(
                        Resources.UnableToRetrieveSProcData, snex);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                }
            }
        }

        private int FindRightParen(string body, string quotePattern)
        {
            int pos = 0;
            bool left = false;
            char quote = Char.MinValue;

            foreach (char c in body)
            {
                if (c == ')')
                {
                    if (left)
                        left = false;
                    else if (quote == Char.MinValue)
                        break;
                }
                else if (c == '(' && quote == Char.MinValue)
                    left = true;
                else
                {
                    int quoteIndex = quotePattern.IndexOf(c);
                    if (quoteIndex > -1)
                        if (quote == Char.MinValue)
                            quote = c;
                        else if (quote == c)
                            quote = Char.MinValue;
                }

                pos++;
            }
            return pos;
        }

        private void ParseProcedureBody(DataTable parametersTable, string body,
            DataRow row, string nameToRestrict)
        {
            string sqlMode = row["SQL_MODE"].ToString();
            bool ansiQuotes = sqlMode.IndexOf("ANSI_QUOTES") != -1;
            bool noBackslash = sqlMode.IndexOf("NO_BACKSLASH_ESCAPES") != -1;
            string quotePattern = ansiQuotes ? "``\"\"" : "``";

            ContextString cs = new ContextString(quotePattern, !noBackslash);

            int leftParen = cs.IndexOf(body, '(');
            Debug.Assert(leftParen != -1);

            // trim off the first part
            body = body.Substring(leftParen + 1);

            int rightParen = FindRightParen(body, quotePattern);
            Debug.Assert(rightParen != -1);
            string parms = body.Substring(0, rightParen).Trim();

            quotePattern += "()";
            cs.ContextMarkers = quotePattern;
            string[] paramDefs = cs.Split(parms, ",");
            ArrayList parmArray = new ArrayList(paramDefs);
            body = body.Substring(rightParen + 1).Trim().ToLower(CultureInfo.InvariantCulture);
            if (body.StartsWith("returns"))
                parmArray.Add(body);
            int pos = 1;
            foreach (string def in parmArray)
            {
                DataRow parmRow = parametersTable.NewRow();
                parmRow["ROUTINE_CATALOG"] = null;
                parmRow["ROUTINE_SCHEMA"] = row["ROUTINE_SCHEMA"];
                parmRow["ROUTINE_NAME"] = row["ROUTINE_NAME"];
                parmRow["ROUTINE_TYPE"] = row["ROUTINE_TYPE"];
                ParseParameter(def, cs, sqlMode, parmRow);
                if (parmRow["IS_RESULT"].Equals("YES"))
                    parmRow["ORDINAL_POSITION"] = 0;
                else
                    parmRow["ORDINAL_POSITION"] = pos++;
                if (nameToRestrict == null ||
                    parmRow["PARAMETER_NAME"].ToString().ToLower() ==
                    nameToRestrict)
                    parametersTable.Rows.Add(parmRow);
            }
        }

        private void ParseParameter(string parmDef, ContextString cs,
            string sqlMode, DataRow parmRow)
        {
            parmDef = parmDef.Trim();
            string lowerDef = parmDef.ToLower(CultureInfo.InvariantCulture);

            parmRow["IS_RESULT"] = "NO";
            parmRow["PARAMETER_MODE"] = "IN";
            if (lowerDef.StartsWith("inout "))
            {
                parmRow["PARAMETER_MODE"] = "INOUT";
                parmDef = parmDef.Substring(6);
            }
            else if (lowerDef.StartsWith("out "))
            {
                parmRow["PARAMETER_MODE"] = "OUT";
                parmDef = parmDef.Substring(4);
            }
            else if (lowerDef.StartsWith("returns "))
            {
                parmRow["PARAMETER_MODE"] = "OUT";
                parmRow["IS_RESULT"] = "YES";
                parmDef = parmDef.Substring(8);
            }
            else if (lowerDef.StartsWith("in "))
                parmDef = parmDef.Substring(3);
            parmDef = parmDef.Trim();

            string[] split = cs.Split(parmDef, " \t\r\n");
            if (parmRow["IS_RESULT"].Equals("NO"))
            {
                parmRow["PARAMETER_NAME"] = String.Format("{0}{1}",
                    connection.ParameterMarker, CleanParameterName(split[0]));
                parmDef = parmDef.Substring(split[0].Length);
            }
            else
                parmRow["PARAMETER_NAME"] = String.Format("{0}RETURN_VALUE",
                    connection.ParameterMarker);

            ParseType(parmDef, sqlMode, parmRow);
        }

        private string CleanParameterName(string parameter)
        {
            char c = parameter[0];
            if (c == '`' || c == '\'' || c == '"')
                return parameter.Substring(1, parameter.Length - 2);
            return parameter;
        }

        private void ParseType(string type, string sql_mode, DataRow parmRow)
        {
            string typeName, flags = String.Empty, size;
            int endExtra;
            string rest = null;

            type = type.ToLower(CultureInfo.InvariantCulture).Trim();
            int startExtra = type.IndexOf("(");
            if (startExtra != -1)
                endExtra = type.IndexOf(')', startExtra + 1);
            else
                endExtra = startExtra = type.IndexOf(' ');
            if (startExtra == -1)
                startExtra = type.Length;
            if (endExtra == -1)
                endExtra = type.Length;

            typeName = type.Substring(0, startExtra);
            rest = type.Substring(endExtra).Trim();

            if (type.Length > endExtra)
            {
                flags = type.Substring(endExtra + 1);
                if (flags.IndexOf("unsigned") != -1)
                    parmRow["FLAGS"] = "UNSIGNED";
            }
            bool real_as_float = sql_mode.IndexOf("REAL_AS_FLOAT") != -1;

            parmRow["DATA_TYPE"] = typeName;

            if (endExtra > startExtra && typeName != "set" && typeName != "enum")
            {
                size = type.Substring(startExtra + 1, endExtra - (startExtra + 1));
                string[] parts = size.Split(new char[] { ',' });
                if (typeName == "varchar" || typeName == "char")
                    parmRow["CHARACTER_MAXIMUM_LENGTH"] = Int32.Parse(parts[0]);
                else
                {
                    parmRow["NUMERIC_PRECISION"] = Byte.Parse(parts[0]);
                    if (parts.Length > 1)
                        parmRow["NUMERIC_SCALE"] = Int32.Parse(parts[1]);
                }
            }
            if (rest.StartsWith("character set"))
            {
                rest = rest.Substring(14);
                startExtra = rest.IndexOf(' ');
                if (startExtra == -1)
                    startExtra = rest.Length;
                parmRow["CHARACTER_SET"] = rest.Substring(0, startExtra);
            }
        }

/*        private void ProcessParameterList(DataTable parametersTable, string db,
            string procName, string paramList, string returns)
        {
            string[] paramDefs = Utility.ContextSplit(paramList, ",", "()");
            int pos = 1;
            while (returns.Length > 0 || pos <= paramDefs.Length)
            {
                string type;
                DataRow row = parametersTable.NewRow();
                row["specific_schema"] = db;
                row["specific_name"] = procName;
                if (returns.Length == 0)
                {
                    string[] parts = Utility.ContextSplit(paramDefs[pos - 1].ToLower(CultureInfo.InvariantCulture),
                        " \t\r\n", "");
                    row["ordinal_position"] = pos++;
                    if (parts.Length == 0) continue;
                    row["parameter_name"] = parts.Length == 3 ? parts[1] : parts[0];
                    row["parameter_mode"] = parts.Length == 3 ?
                        parts[0].ToUpper(CultureInfo.InvariantCulture) : "IN";
                    row["is_result"] = "NO";
                    type = parts.Length == 3 ? parts[2] : parts[1];
                }
                else
                {
                    row["is_result"] = "YES";
                    type = returns;
                    row["ordinal_position"] = 0;
                }
                row["data_type"] = type.ToUpper(CultureInfo.InvariantCulture);
                returns = String.Empty;
                parametersTable.Rows.Add(row);
            }
        }
        */
        #endregion

    }
}
