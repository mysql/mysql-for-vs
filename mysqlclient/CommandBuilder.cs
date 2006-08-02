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
		private char				marker = '?';

		#region Constructors

		/// <include file='docs/MySqlCommandBuilder.xml' path='docs/Ctor/*'/>
		public MySqlCommandBuilder()
		{
			QuotePrefix = QuoteSuffix = "`";
		}

		/// <include file='docs/MySqlCommandBuilder.xml' path='docs/Ctor2/*'/>
		public MySqlCommandBuilder(MySqlDataAdapter adapter) : this()
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

		/// <include file='docs/MySqlCommandBuilder.xml' path='docs/RefreshSchema/*'/>
		public override void RefreshSchema()
		{
		}
		#endregion


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
