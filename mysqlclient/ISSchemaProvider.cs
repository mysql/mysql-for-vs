using System;
using System.Data;
using System.Text;
using MySql.Data.Common;
using System.Globalization;

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

           object[,] collections = new object[,] 
            {
                {"Views", 2, 3},
                {"ViewColumns", 3, 4},
                {"ProcedureParameters", 4, 1},
                {"Procedures", 4, 3},
                {"Triggers", 2, 4}
            };

            FillTable(dt, collections);
            return dt;
        }

        protected override DataTable GetRestrictions()
        {
            DataTable dt = base.GetRestrictions();

            object[,] restrictions = new object[,] 
            {
                {"ProcedureParameters", "Catalog", "", 0},
                {"ProcedureParameters", "Owner", "", 1},
                {"ProcedureParameters", "Name", "", 2},
                {"ProcedureParameters", "Parameter", "", 3},
                {"Procedures", "Catalog", "", 0},
                {"Procedures", "Owner", "", 1},
                {"Procedures", "Name", "", 2},
                {"Procedures", "Type", "", 3},
                {"Views", "Catalog", "", 0},
                {"Views", "Owner", "", 1},
                {"Views", "Table", "", 2},
                {"ViewColumns", "Catalog", "", 0},
                {"ViewColumns", "Owner", "", 1},
                {"ViewColumns", "Table", "", 2},
                {"ViewColumns", "Column", "", 3},
                {"Triggers", "Catalog", "", 0},
                {"Triggers", "Schema", "", 1},
                {"Triggers", "Name", "", 2},
                {"Triggers", "EventObjectTable", "", 3},
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
            return dt;
        }

        public override DataTable GetTables(string[] restrictions)
        {
            string[] keys = new string[4];
            keys[0] = "TABLE_CATALOG";
            keys[1] = "TABLE_SCHEMA";
            keys[2] = "TABLE_NAME";
            keys[3] = "TABLE_TYPE";
            return Query("TABLES", "TABLE_TYPE != 'VIEW'", keys, restrictions);
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
            return dt;
        }

        private DataTable GetViews(string[] restrictions)
        {
            string[] keys = new string[3];
            keys[0] = "TABLE_CATALOG";
            keys[1] = "TABLE_SCHEMA";
            keys[2] = "TABLE_NAME";
            return Query("VIEWS", null, keys, restrictions);
        }

        private DataTable GetViewColumns(string[] restrictions)
        {
            DataTable dt = new DataTable("ViewColumns");
            dt.Columns.Add("VIEW_CATALOG", typeof(string));
            dt.Columns.Add("VIEW_SCHEMA", typeof(string));
            dt.Columns.Add("VIEW_NAME", typeof(string));
            dt.Columns.Add("COLUMN_NAME", typeof(string));

            StringBuilder where = new StringBuilder();
            StringBuilder sql = new StringBuilder(
                "SELECT NULL as VIEW_CATALOG, C.table_schema AS VIEW_SCHEMA, ");
            sql.Append("C.table_name AS VIEW_NAME, C.column_name ");
            sql.Append("FROM information_schema.columns C JOIN information_schema.views V");
            sql.Append("ON C.table_schema=V.table_schema AND C.table_name=V.table_name ");
            if (restrictions != null && restrictions.Length >= 2 &&
                restrictions[1] != null)
                where.AppendFormat("C.table_schema='{0}'", restrictions[1]);
            if (restrictions != null && restrictions.Length >= 3 &&
                restrictions[2] != null)
                where.AppendFormat("C.table_name='{0}'", restrictions[2]);
            if (restrictions != null && restrictions.Length == 4 &&
                restrictions[3] != null)
                where.AppendFormat("C.column_name='{0}'", restrictions[3]);
            if (where.Length > 0)
                sql.AppendFormat(" WHERE {0}", where.ToString());
            return GetTable(sql.ToString());
        }

        private DataTable GetTriggers(string[] restrictions)
        {
            string[] keys = new string[4];
            keys[0] = "TRIGGER_CATALOG";
            keys[1] = "TRIGGER_SCHEMA";
            keys[2] = "TRIGGER_NAME";
            keys[3] = "EVENT_OBJECT_TABLE";
            return Query("TRIGGERS", null, keys, restrictions);
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
            return Query("ROUTINES", null, keys, restrictions);
        }

        /// <summary>
        /// Return schema information about parameters for procedures and functions
        /// Restrictions supported are:
        /// schema, name, type, parameter name
        /// </summary>
        /// <param name="restrictions"></param>
        /// <returns></returns>
        public virtual DataTable GetProcedureParameters(string[] restrictions)
        {
            DataTable dt = new DataTable("Procedure Parameters");
            dt.Columns.Add("specific_schema", typeof(string));
            dt.Columns.Add("specific_name", typeof(string));
            dt.Columns.Add("parameter_name", typeof(string));
            dt.Columns.Add("ordinal_position", typeof(Int32));
            dt.Columns.Add("parameter_mode", typeof(string));
            dt.Columns.Add("is_result", typeof(string));
            dt.Columns.Add("data_type", typeof(string));
            dt.Columns.Add("length", typeof(string));

            // first try and get parameter information from mysql.proc
            // since that will be faster.
            // Fall back to show create since that requires lesser privs
            try
            {
                GetParametersFromMySqlProc(dt, restrictions);
            }
            catch (MySqlException)
            {
                GetParametersFromShowCreate(dt, restrictions);
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
                    return GetProcedureParameters(restrictions);
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

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] == null || values[i] == String.Empty) continue;
                if (where.Length > 0)
                    where.Append(" AND ");
                where.AppendFormat("{0}='{1}'", keys[i], values[i]);
            }

            if (where.Length > 0)
                query.AppendFormat(" WHERE {0}", where.ToString());

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

        private void GetParametersFromMySqlProc(DataTable parametersTable,
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

        private void GetParametersFromShowCreate(DataTable parametersTable,
            string[] restrictions)
        {
            DataTable routines = GetSchema("procedures", restrictions);

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
                    reader = cmd.ExecuteReader();
                    reader.Read();
                    ParseProcedureBody(parametersTable, reader.GetString(2),
                        routine);
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
            DataRow row)
        {

        }

        private void ProcessParameterList(DataTable parametersTable, string db,
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

        #endregion

    }
}
