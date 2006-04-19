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


        private DataTable Query(string table_name, string[] keys, string[] values)
        {
            StringBuilder where = new StringBuilder();
            StringBuilder query = new StringBuilder("SELECT * FROM INFORMATION_SCHEMA.");
            query.Append(table_name);

            for (int i=0; i < values.Length; i++)
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

        public override DataTable HelpCollection()
        {
            return null;
        }

        public override DataTable GetDatabases()
        {
            try
            {
                DataTable table = new DataTable();
                MySqlDataAdapter da = new MySqlDataAdapter("SHOW DATABASES", connection);
                da.Fill(table);
                table.Columns[0].ColumnName = "database_name";
                return table;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override DataTable GetTables(string[] restrictions)
        {
            string[] keys = new string[4];
            keys[0] = "TABLE_CATALOG";
            keys[1] = "TABLE_sCHEMA";
            keys[2] = "TABLE_NAME";
            keys[3] = "TABLE_TYPE";
            return Query("TABLES", keys, restrictions);
        }

        public override DataTable GetColumns(string[] restrictions)
        {
            string[] keys = new string[4];
            keys[0] = "TABLE_CATALOG";
            keys[1] = "TABLE_sCHEMA";
            keys[2] = "TABLE_NAME";
            keys[3] = "COLUMN_NAME";
            return Query("COLUMNS", keys, restrictions);
        }

        public override DataTable GetViews(string[] restrictions)
        {
            return Query("VIEWS", null, restrictions);
        }

        public override DataTable GetTriggers(string[] restrictions)
        {
            string[] keys = new string[4];
            keys[0] = "TRIGGER_CATALOG";
            keys[1] = "TRIGGER_sCHEMA";
            keys[2] = "TRIGGER_NAME";
            keys[3] = "EVENT_OBJECT_TABLE";
            return Query("TRIGGERS", keys, restrictions);
        }

        /// <summary>
        /// Return schema information about procedures and functions
        /// Restrictions supported are:
        /// schema, name, type
        /// </summary>
        /// <param name="restrictions"></param>
        /// <returns></returns>
        public override DataTable GetProcedures(string[] restrictions)
        {
            StringBuilder where = new StringBuilder("");
            StringBuilder query = new StringBuilder("SELECT * FROM INFORMATION_SCHEMA.ROUTINES");

            if (restrictions[1] != null && restrictions[1].Length != 0)
                where.AppendFormat("ROUTINE_SCHEMA='{0}'", restrictions[1]);
            if (restrictions[2] != null && restrictions[2].Length != 0)
            {
                if (where.Length > 0)
                    where.Append(" AND ");
                where.AppendFormat("ROUTINE_NAME='{0}'", restrictions[2]);
            }
            if (restrictions[3] != null && restrictions[3].Length != 0)
            {
                if (where.Length > 0)
                    where.AppendLine(" AND ");
                where.AppendFormat("ROUTINE_TYPE='{0}'", restrictions[3]);
            }
            if (where.Length > 0)
            {
                query.Append(" WHERE ");
                query.Append(where);
            }
            DataTable table = GetTable(query.ToString());
            table.TableName = "Procedures";
            return table;
        }

        /// <summary>
        /// Return schema information about parameters for procedures and functions
        /// Restrictions supported are:
        /// schema, name, type, parameter name
        /// </summary>
        /// <param name="restrictions"></param>
        /// <returns></returns>
        public override DataTable GetProcedureParameters(string[] restrictions)
        {
            DataTable parametersTable = CreateParametersTable();

            // first try and get parameter information from mysql.proc
            // since that will be faster.
            // Fall back to show create since that requires lesser privs
            try
            {
                GetParametersFromMySqlProc(parametersTable, restrictions);
            }
            catch (MySqlException)
            {
                GetParametersFromShowCreate(parametersTable, restrictions);
            }
            catch (Exception)
            {
                throw;
            }

            return parametersTable;
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

        private DataTable CreateParametersTable()
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
            return dt;
        }

        private void ParseProcedureBody(DataTable parametersTable, string body,
            DataRow row)
        {

        }

        private void ProcessParameterList(DataTable parametersTable, string db, 
            string procName, string paramList, string returns)
        {
            string[] paramDefs = Utility.ContextSplit(paramList, ",", "()" );
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
