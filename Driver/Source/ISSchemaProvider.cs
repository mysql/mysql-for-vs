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
using System.Text;
using MySql.Data.Common;
using System.Globalization;
using System.Diagnostics;
using System.Collections;
using System.Data.SqlTypes;
using MySql.Data.Types;

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
            dt.Columns.Add("CHARACTER_OCTET_LENGTH", typeof(Int32));
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

        private void ParseProcedureBody(DataTable parametersTable, string body,
            DataRow row, string nameToRestrict)
        {
            string sqlMode = row["SQL_MODE"].ToString();

            int pos = 1;
            SqlTokenizer tokenizer = new SqlTokenizer(body);
            tokenizer.AnsiQuotes = sqlMode.IndexOf("ANSI_QUOTES") != -1;
            tokenizer.BackslashEscapes = sqlMode.IndexOf("NO_BACKSLASH_ESCAPES") == -1;
            string token = tokenizer.NextToken();

            // look for the opening paren
            while (token != "(")
                token = tokenizer.NextToken();

            while (token != ")")
            {
                token = tokenizer.NextToken();
                DataRow parmRow = parametersTable.NewRow();
                InitParameterRow(row, parmRow);
                parmRow["ORDINAL_POSITION"] = pos++;
                string parameterName = token;
                if (!tokenizer.Quoted)
                {
                    string mode = null;
                    string lowerToken = token.ToLower(CultureInfo.InvariantCulture);
                    if (lowerToken == "in")
                        mode = "IN";
                    else if (lowerToken == "inout")
                        mode = "INOUT";
                    else if (lowerToken == "out")
                        mode = "OUT";
                    if (mode != null)
                    {
                        parmRow["PARAMETER_MODE"] = mode;
                        parameterName = tokenizer.NextToken();
                    }
                }
                parmRow["PARAMETER_NAME"] = String.Format("{0}{1}",
                    connection.ParameterMarker, parameterName);
                token = ParseDataType(parmRow, tokenizer);
                if (nameToRestrict == null ||
                  parmRow["PARAMETER_NAME"].ToString().ToLower() ==
                nameToRestrict)
                    parametersTable.Rows.Add(parmRow);
            }

            // now parse out the return parameter if there is one.
            token = tokenizer.NextToken().ToLower(CultureInfo.InvariantCulture);
            if (token == "returns")
            {
                DataRow parameterRow = parametersTable.NewRow();
                InitParameterRow(row, parameterRow);
                parameterRow["PARAMETER_NAME"] = String.Format("{0}RETURN_VALUE",
                    connection.ParameterMarker);
                parameterRow["IS_RESULT"] = "YES";
                ParseDataType(parameterRow, tokenizer);
                parametersTable.Rows.Add(parameterRow);
            }
        }

        /// <summary>
        /// Initializes a new row for the procedure parameters table.
        /// </summary>
        private void InitParameterRow(DataRow procedure, DataRow parameter)
        {
            parameter["ROUTINE_CATALOG"] = null;
            parameter["ROUTINE_SCHEMA"] = procedure["ROUTINE_SCHEMA"];
            parameter["ROUTINE_NAME"] = procedure["ROUTINE_NAME"];
            parameter["ROUTINE_TYPE"] = procedure["ROUTINE_TYPE"];
            parameter["PARAMETER_MODE"] = "IN";
            parameter["ORDINAL_POSITION"] = 0;
            parameter["IS_RESULT"] = "NO";
        }

        /// <summary>
        ///  Parses out the elements of a procedure parameter data type.
        /// </summary>
        private string ParseDataType(DataRow row, SqlTokenizer tokenizer)
        {
            row["DATA_TYPE"] = tokenizer.NextToken().ToUpper(CultureInfo.InvariantCulture);
            string token = tokenizer.NextToken();
            while (SetParameterAttribute(row, token, tokenizer.IsSize, tokenizer))
                token = tokenizer.NextToken();
            return token;
        }

        /// <summary>
        /// Attempts to parse a given token as a type attribute.
        /// </summary>
        /// <returns>True if the token was recognized as a type attribute,
        /// false otherwise.</returns>
        private bool SetParameterAttribute(DataRow row, string token, bool isSize,
            SqlTokenizer tokenizer)
        {
            if (isSize)
            {
                string[] sizeParts = token.Split(new char[] { ',' });
                if (MetaData.IsNumericType(row["DATA_TYPE"].ToString()))
                    row["NUMERIC_PRECISION"] = Int32.Parse(sizeParts[0]);
                else
                    row["CHARACTER_OCTET_LENGTH"] = Int32.Parse(sizeParts[0]);
                if (sizeParts.Length == 2)
                    row["NUMERIC_SCALE"] = Int32.Parse(sizeParts[1]);
                return true;
            }
            else
            {
                string lowerToken = token.ToLower(CultureInfo.InvariantCulture);
                switch (lowerToken)
                {
                    case "unsigned":
                    case "zerofill":
                        row["FLAGS"] = String.Format("{0} {1}", row["FLAGS"], token);
                        return true;
                    case "character":
                        string set = tokenizer.NextToken().ToLower(CultureInfo.InvariantCulture);
                        Debug.Assert(set == "set");
                        row["CHARACTER_SET"] = tokenizer.NextToken();
                        return true;
                    case "ascii":
                        row["CHARACTER_SET"] = "latin1";
                        return true;
                    case "unicode":
                        row["CHARACTER_SET"] = "ucs2";
                        return true;
                    case "binary":
                        return true;
                }
            }
            return false;
        }

        #endregion

    }
}
