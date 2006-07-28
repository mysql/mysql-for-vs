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
using System.ComponentModel;
using System.Data.Common;
using System.Data;
using System.Text;
using MySql.Data.Common;
using System.Collections;
using MySql.Data.Types;
using System.Globalization;

namespace MySql.Data.MySqlClient
{
	/// <include file='docs/MySqlCommandBuilder.xml' path='docs/class/*'/>
#if DESIGN
	[ToolboxItem(false)]
	[System.ComponentModel.DesignerCategory("Code")]
#endif
	public sealed class MySqlCommandBuilder : DbCommandBuilder
	{
		private DataTable			_schema;
		private string				tableName;
		private string				schemaName;

		private	MySqlCommand		_updateCmd;
		private MySqlCommand		_insertCmd;
		private MySqlCommand		_deleteCmd;

		private char				marker = '?';
		private bool				lastOneWins;

		#region Constructors

		/// <include file='docs/MySqlCommandBuilder.xml' path='docs/Ctor/*'/>
		public MySqlCommandBuilder()
		{
			QuotePrefix = QuoteSuffix = "`";
		}

		/// <include file='docs/MySqlCommandBuilder.xml' path='docs/Ctor1/*'/>
		public MySqlCommandBuilder(bool lastOneWins) : this()
		{
			this.lastOneWins = lastOneWins;
		}

		/// <include file='docs/MySqlCommandBuilder.xml' path='docs/Ctor2/*'/>
		public MySqlCommandBuilder(MySqlDataAdapter adapter) : this()
		{
			DataAdapter = adapter;
		}

		/// <include file='docs/MySqlCommandBuilder.xml' path='docs/Ctor3/*'/>
		public MySqlCommandBuilder(MySqlDataAdapter adapter, bool lastOneWins) : 
            this(lastOneWins)
		{
			DataAdapter = adapter;
		}

		#endregion

		#region Properties

		/// <include file='docs/mysqlcommandBuilder.xml' path='docs/DataAdapter/*'/>
        public new MySqlDataAdapter DataAdapter
        {
            get { return (MySqlDataAdapter)base.DataAdapter; }
            set { base.DataAdapter = value; }
        }

/*		/// <include file='docs/MySqlCommandBuilder.xml' path='docs/QuotePrefix/*'/>
		public override string QuotePrefix 
		{
			get { return _QuotePrefix; }
			set { _QuotePrefix = value; }
		}

		/// <include file='docs/MySqlCommandBuilder.xml' path='docs/QuoteSuffix/*'/>
		public string QuoteSuffix
		{
			get { return _QuoteSuffix; }
			set { _QuoteSuffix = value; }
		}
*/
		private string TableName 
		{
			get 
			{
				if (schemaName != null && schemaName.Length > 0)
					return Quote(schemaName) + "." + Quote(tableName);
				return Quote(tableName);
			}
		}

		#endregion

		#region Public Methods
		/// <summary>
		/// Retrieves parameter information from the stored procedure specified 
		/// in the MySqlCommand and populates the Parameters collection of the 
		/// specified MySqlCommand object.
		/// This method is not currently supported since stored procedures are 
		/// not available in MySql.
		/// </summary>
		/// <param name="command">The MySqlCommand referencing the stored 
		/// procedure from which the parameter information is to be derived. 
		/// The derived parameters are added to the Parameters collection of the 
		/// MySqlCommand.</param>
		/// <exception cref="InvalidOperationException">The command text is not 
		/// a valid stored procedure name.</exception>
		public static void DeriveParameters(MySqlCommand command)
		{
			if (!command.Connection.driver.Version.isAtLeast(5,0,0))
				throw new MySqlException("DeriveParameters is not supported on versions " +
					"prior to 5.0");
            StoredProcedure sp = new StoredProcedure(command.Connection, "");

            string schema = command.Connection.Database;
            string spName = command.CommandText;
            int dotIndex = spName.IndexOf('.');
            if (dotIndex != -1)
            {
                schema = spName.Substring(0, dotIndex);
                spName = spName.Substring(dotIndex + 1);
            }

            // now retrieve the paramters using GetSchema
            string[] restrictions = new string[5];
            restrictions[1] = schema;
            restrictions[2] = spName;
            DataTable parameters = command.Connection.GetSchema(
                "procedure parameters", restrictions);

            command.Parameters.Clear();
            foreach (DataRow row in parameters.Rows)
            {
                MySqlParameter p = new MySqlParameter();
                p.ParameterName = row["PARAMETER_NAME"].ToString();
                p.Direction = GetDirection(row["PARAMETER_MODE"].ToString(),
                    row["IS_RESULT"].ToString());
                p.MySqlDbType = MetaData.NameToType(row["DATA_TYPE"].ToString(),
                    false, false, command.Connection);
                if (!row["CHARACTER_MAXIMUM_LENGTH"].Equals(DBNull.Value))
                    p.Size = (int)row["CHARACTER_MAXIMUM_LENGTH"];
                if (!row["NUMERIC_PRECISION"].Equals(DBNull.Value))
                    p.Precision = (byte)row["NUMERIC_PRECISION"];
                if (!row["NUMERIC_SCALE"].Equals(DBNull.Value))
                    p.Scale = (byte)(int)row["NUMERIC_SCALE"];
                command.Parameters.Add(p);
            }
        }

        private static ParameterDirection GetDirection(string direction, string is_result)
        {
            if (is_result == "YES")
                return ParameterDirection.ReturnValue;
            else if (direction == "IN")
                return ParameterDirection.Input;
            else if (direction == "OUT")
                return ParameterDirection.Output;
            return ParameterDirection.InputOutput;
        }

        public new MySqlCommand GetDeleteCommand()
        {
            return (MySqlCommand)base.GetDeleteCommand();
        }

        public new MySqlCommand GetUpdateCommand()
        {
            return (MySqlCommand)base.GetUpdateCommand();
        }

        public new MySqlCommand GetInsertCommand()
        {
            return (MySqlCommand)GetInsertCommand(false);
        }

/*		/// <include file='docs/MySqlCommandBuilder.xml' path='docs/GetDeleteCommand/*'/>
		public new MySqlCommand GetDeleteCommand()
		{
			if (_schema == null)
				GenerateSchema();
			return CreateDeleteCommand();
		}

		/// <include file='docs/MySqlCommandBuilder.xml' path='docs/GetInsertCommand/*'/>
		public new MySqlCommand GetInsertCommand()
		{
			if (_schema == null)
				GenerateSchema();
			return CreateInsertCommand();
		}

		/// <include file='docs/MySqlCommandBuilder.xml' path='docs/GetUpdateCommand/*'/>
		public new MySqlCommand GetUpdateCommand() 
		{
			if (_schema == null)
				GenerateSchema();
			return CreateUpdateCommand();
		}
*/
		/// <include file='docs/MySqlCommandBuilder.xml' path='docs/RefreshSchema/*'/>
		public override void RefreshSchema()
		{
			_schema = null;
			_insertCmd = null;
			_deleteCmd = null;
			_updateCmd = null;
			tableName = null;
			schemaName = null;
		}
		#endregion

		#region Private Methods

		private void GenerateSchema()
		{
            if (DataAdapter == null)
				throw new MySqlException(Resources.AdapterIsNull);
            if (DataAdapter.SelectCommand == null)
				throw new MySqlException(Resources.AdapterSelectIsNull);

            // set the parameter marker
            MySqlConnection conn = (MySqlConnection)DataAdapter.SelectCommand.Connection;
            marker = conn.ParameterMarker;

            MySqlDataReader dr = DataAdapter.SelectCommand.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo);
			_schema = dr.GetSchemaTable();
			dr.Close();

			// make sure we got at least one unique or key field and count base table names
			bool   hasKeyOrUnique=false;

			foreach (DataRow row in _schema.Rows)
			{
				string rowTableName = row["BaseTableName"].ToString();
				string rowSchemaName = row["BaseSchemaName"].ToString();

				if (true == (bool)row["IsKey"] || true == (bool)row["IsUnique"])
					hasKeyOrUnique=true;

				if (tableName == null)
				{
					schemaName = rowSchemaName;
					tableName = rowTableName;
				}
				else if (tableName != rowTableName && rowTableName != null && rowTableName.Length > 0)
					throw new InvalidOperationException(Resources.CBMultiTableNotSupported);
				else if (schemaName != rowSchemaName && rowSchemaName != null && rowSchemaName.Length > 0)
					throw new InvalidOperationException(Resources.CBMultiTableNotSupported);
			}
			if (! hasKeyOrUnique)
				throw new InvalidOperationException(Resources.CBNoKeyColumn);
		}

		private string Quote(string table_or_column)
		{
			if (QuotePrefix == null || QuoteSuffix == null)
				return table_or_column;
			return QuotePrefix + table_or_column + QuoteSuffix;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
		protected override string GetParameterName(string columnName)
		{
			StringBuilder sb = new StringBuilder(columnName);
			sb.Replace(" ", "");
			sb.Replace("/", "_per_");
			sb.Replace("-", "_");
			sb.Replace(")", "_cb_");
			sb.Replace("(", "_ob_");
			sb.Replace("%", "_pct_");
			sb.Replace("<", "_lt_");
			sb.Replace(">", "_gt_");
			sb.Replace(".", "_pt_");
			return sb.ToString();
		}

		private MySqlParameter CreateParameter(DataRow row, bool Original)
		{
			MySqlParameter p;
			string colName = GetParameterName( row["ColumnName"].ToString() );
			MySqlDbType type = (MySqlDbType)row["ProviderType"];

			if (Original)
				p = new MySqlParameter( "Original_" + colName, type, ParameterDirection.Input, 
					(string)row["ColumnName"], DataRowVersion.Original, DBNull.Value );
			else
				p = new MySqlParameter( colName, type, ParameterDirection.Input, 
					(string)row["ColumnName"], DataRowVersion.Current, DBNull.Value );
			return p;
		}

		private MySqlCommand CreateBaseCommand()
		{
			MySqlCommand cmd = new MySqlCommand();
			cmd.Connection = DataAdapter.SelectCommand.Connection;
			cmd.CommandTimeout = DataAdapter.SelectCommand.CommandTimeout;
			cmd.Transaction = DataAdapter.SelectCommand.Transaction;
			return cmd;
		}

		private MySqlCommand CreateDeleteCommand()
		{
			if (_deleteCmd != null) return _deleteCmd;

			MySqlCommand cmd = CreateBaseCommand();

			cmd.CommandText = "DELETE FROM " + TableName + 
				" WHERE " + CreateOriginalWhere(cmd);

			_deleteCmd = cmd;
			return cmd;
		}

		private string CreateFinalSelect(bool forinsert)
		{
			StringBuilder sel = new StringBuilder();
			StringBuilder where = new StringBuilder();

			foreach (DataRow row in _schema.Rows)
			{
				// don't include functions in where clause
				string baseTableName = (string)row["BaseTableName"];
				if (baseTableName == null || baseTableName.Length == 0)
					continue;

				string colname = Quote(row["ColumnName"].ToString());
				string parmName = GetParameterName( row["ColumnName"].ToString() );

				if (sel.Length > 0)
					sel.Append(", ");
				sel.Append( colname );
				if ((bool)row["IsKey"] == false) continue;
				if (where.Length > 0)
					where.Append(" AND ");
				where.Append( "(" + colname + "=" );
				if (forinsert) 
				{
					if ((bool)row["IsAutoIncrement"])
						where.Append("last_insert_id()");
					else if ((bool)row["IsKey"])
						where.Append( marker + parmName);
				}
				else 
				{
					where.Append(marker + "Original_" + parmName);
				}
				where.Append(")");
			}
			return "SELECT " + sel.ToString() + " FROM " + TableName +
				" WHERE " + where.ToString();
		}

		private string CreateOriginalWhere(MySqlCommand cmd)
		{
			StringBuilder wherestr = new StringBuilder();

			foreach (DataRow row in _schema.Rows)
			{
				// don't include functions in where clause
				string baseTableName = (string)row["BaseTableName"];
				if (baseTableName == null || baseTableName.Length == 0)
					continue;

				// if we are doing last one wins and this column is not a key or is not
				// unique, then we don't care about it
				if (true != (bool)row["IsKey"] && true != (bool)row["IsUnique"] && lastOneWins)
					continue;

				if (! IncludedInWhereClause(row)) continue;

				// first update the where clause since it will contain all parameters
//				if (wherestr.Length > 0)
//					wherestr.Append(" AND ");
				string colname = Quote((string)row["ColumnName"]);

				MySqlParameter op = CreateParameter(row, true);
				cmd.Parameters.Add(op);

				wherestr.Append( colname + " <=> " + marker + op.ParameterName + " AND ");
//				if ((bool)row["AllowDBNull"] == true) 
//					wherestr.Append( " or (" + colname + " IS NULL and ?" + op.ParameterName + " IS NULL)");
				//wherestr.Append(")");
			}
			wherestr.Remove( wherestr.Length-5, 5 ); // remove the trailling " AND "
			return wherestr.ToString();
		}

		private MySqlCommand CreateUpdateCommand()
		{
			if (_updateCmd != null) return _updateCmd; 

			MySqlCommand cmd = CreateBaseCommand();

			StringBuilder setstr = new StringBuilder();
		
			foreach (DataRow schemaRow in _schema.Rows)
			{
				// don't include functions in where clause
				string baseTableName = (string)schemaRow["BaseTableName"];
				if (baseTableName == null || baseTableName.Length == 0)
					continue;

				string colname = Quote((string)schemaRow["ColumnName"]);

				if (! IncludedInUpdate(schemaRow)) continue;

				if (setstr.Length > 0) 
					setstr.Append(", ");

				MySqlParameter p = CreateParameter(schemaRow, false);
				cmd.Parameters.Add(p);

				setstr.Append( colname + "=" + marker + p.ParameterName );
			}

			cmd.CommandText = "UPDATE " + TableName + " SET " + setstr.ToString() + 
				" WHERE " + CreateOriginalWhere(cmd);
			cmd.CommandText += "; " + CreateFinalSelect(false);

			_updateCmd = cmd;
			return cmd;
		}

		private MySqlCommand CreateInsertCommand()
		{
			if (_insertCmd != null) return _insertCmd;

			MySqlCommand cmd = CreateBaseCommand();

			StringBuilder setstr = new StringBuilder();
			StringBuilder valstr = new StringBuilder();
			foreach (DataRow schemaRow in _schema.Rows)
			{
				// don't include functions in where clause
				string baseTableName = (string)schemaRow["BaseTableName"];
				if (baseTableName == null || baseTableName.Length == 0)
					continue;

				string colname = Quote((string)schemaRow["ColumnName"]);

				if (!IncludedInInsert(schemaRow)) continue;

				if (setstr.Length > 0) 
				{
					setstr.Append(", ");
					valstr.Append(", ");
				}

				MySqlParameter p = CreateParameter(schemaRow, false);
				cmd.Parameters.Add(p);

				setstr.Append( colname );
				valstr.Append( marker + p.ParameterName );
			}

			cmd.CommandText = "INSERT INTO " + TableName + " (" + setstr.ToString() + ") " +
				" VALUES (" + valstr.ToString() + ")";
			cmd.CommandText += "; " + CreateFinalSelect(true);

			_insertCmd = cmd;
			return cmd;
		}

		private static bool IncludedInInsert (DataRow schemaRow)
		{
			// If the parameter has one of these properties, then we don't include it in the insert:
			// AutoIncrement, Hidden, Expression, RowVersion, ReadOnly

//			if ((bool) schemaRow ["IsAutoIncrement"])
//				return false;
			/*			if ((bool) schemaRow ["IsHidden"])
							return false;
						if ((bool) schemaRow ["IsExpression"])
							return false;*/
			if ((bool) schemaRow ["IsRowVersion"])
				return false;
			if ((bool) schemaRow ["IsReadOnly"])
				return false;
			return true;
		}

		private static bool IncludedInUpdate (DataRow schemaRow)
		{
			// If the parameter has one of these properties, then we don't include it in the insert:
			// AutoIncrement, Hidden, RowVersion

			//if ((bool) schemaRow ["IsAutoIncrement"])
			//	return false;
			//			if ((bool) schemaRow ["IsHidden"])
			//				return false;
			if ((bool) schemaRow ["IsRowVersion"])
				return false;
			return true;
		}

		private static bool IncludedInWhereClause (DataRow schemaRow)
		{
			//			if ((bool) schemaRow ["IsLong"])
			//				return false;
			return true;
		}

		private static void SetParameterValues(MySqlCommand cmd, DataRow dataRow)
		{
			foreach (MySqlParameter p in cmd.Parameters)
			{
				if (p.SourceVersion == DataRowVersion.Original)
//				if (p.ParameterName.Length >= 8 && p.ParameterName.Substring(0, 8).Equals("Original"))
					p.Value = dataRow[p.SourceColumn, DataRowVersion.Original];
				else
					p.Value = dataRow[p.SourceColumn, DataRowVersion.Current];
			}
		}

		private void OnRowUpdating(object sender, MySqlRowUpdatingEventArgs args)
		{
			// make sure we are still to proceed
			if (args.Status != UpdateStatus.Continue) return;

			if (_schema == null)
				GenerateSchema();

			if (StatementType.Delete == args.StatementType)
				args.Command = CreateDeleteCommand();
			else if (StatementType.Update == args.StatementType)
				args.Command = CreateUpdateCommand();
			else if (StatementType.Insert == args.StatementType)
				args.Command = CreateInsertCommand();
			else if (StatementType.Select == args.StatementType)
				return;

			SetParameterValues(args.Command, args.Row);
		}
		#endregion

        protected override DbCommand InitializeCommand(DbCommand command)
        {
            return base.InitializeCommand(command);
        }

        protected override DataTable GetSchemaTable(DbCommand sourceCommand)
        {
            marker = (sourceCommand.Connection as MySqlConnection).ParameterMarker;

            DataTable schema;
            using (MySqlDataReader reader = (MySqlDataReader)sourceCommand.ExecuteReader(
                CommandBehavior.KeyInfo | CommandBehavior.SchemaOnly)) 
            {
                schema = reader.GetSchemaTable();
            }

            return schema;
            // make sure we got at least one unique or key field and count base table names
/*            bool hasKeyOrUnique = false;

            foreach (DataRow row in _schema.Rows)
            {
                string rowTableName = row["BaseTableName"].ToString();
                string rowSchemaName = row["BaseSchemaName"].ToString();

                if (true == (bool)row["IsKey"] || true == (bool)row["IsUnique"])
                    hasKeyOrUnique = true;

                if (tableName == null)
                {
                    schemaName = rowSchemaName;
                    tableName = rowTableName;
                }
                else if (tableName != rowTableName && rowTableName != null && rowTableName.Length > 0)
                    throw new InvalidOperationException(Resources.GetString("CBMultiTableNotSupported"));
                else if (schemaName != rowSchemaName && rowSchemaName != null && rowSchemaName.Length > 0)
                    throw new InvalidOperationException(Resources.GetString("CBMultiTableNotSupported"));
            }
            if (!hasKeyOrUnique)
                throw new InvalidOperationException(Resources.GetString("CBNoKeyColumn"));
 * */
        }

        protected override void ApplyParameterInfo(DbParameter parameter, DataRow row, 
            StatementType statementType, bool whereClause)
        {
            ((MySqlParameter)parameter).MySqlDbType = (MySqlDbType)row["ProviderType"];
        }

        protected override string GetParameterName(int parameterOrdinal)
        {
            return String.Format("{0}p{1}", marker,
                parameterOrdinal.ToString(CultureInfo.InvariantCulture));
        }

        protected override string GetParameterPlaceholder(int parameterOrdinal)
        {
            return String.Format("{0}p{1}", marker,
                parameterOrdinal.ToString(CultureInfo.InvariantCulture));
        }

        protected override void SetRowUpdatingHandler(DbDataAdapter adapter)
        {
            if (adapter != base.DataAdapter)
                ((MySqlDataAdapter)adapter).RowUpdating += new MySqlRowUpdatingEventHandler(RowUpdating);
            else
                ((MySqlDataAdapter)adapter).RowUpdating -= new MySqlRowUpdatingEventHandler(RowUpdating);
        }

        private void RowUpdating(object sender, MySqlRowUpdatingEventArgs args)
        {
            base.RowUpdatingHandler(args);
        }

    }
}
