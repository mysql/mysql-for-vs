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
using System.Data.Common;
using System.ComponentModel;
using System.Collections.Generic;

namespace MySql.Data.MySqlClient
{
	/// <include file='docs/MySqlDataAdapter.xml' path='docs/class/*'/>
#if !CF
	[System.Drawing.ToolboxBitmap( typeof(MySqlDataAdapter), "MySqlClient.resources.dataadapter.bmp")]
	[System.ComponentModel.DesignerCategory("Code")]
	[Designer("MySql.Data.MySqlClient.Design.MySqlDataAdapterDesigner,MySqlClient.Design")]
#endif
	public sealed class MySqlDataAdapter : DbDataAdapter, IDbDataAdapter, IDataAdapter, ICloneable
	{
		private bool loadingDefaults;
        private int updateBatchSize;
        List<IDbCommand> commandBatch;

		/// <summary>
		/// Occurs during Update before a command is executed against the data source. The attempt to update is made, so the event fires.
		/// </summary>
		public event MySqlRowUpdatingEventHandler RowUpdating;

		/// <summary>
		/// Occurs during Update after a command is executed against the data source. The attempt to update is made, so the event fires.
		/// </summary>
		public event MySqlRowUpdatedEventHandler RowUpdated;

		/// <include file='docs/MySqlDataAdapter.xml' path='docs/Ctor/*'/>
		public MySqlDataAdapter()
		{
			loadingDefaults = true;
            updateBatchSize = 1;
		}

		/// <include file='docs/MySqlDataAdapter.xml' path='docs/Ctor1/*'/>
		public MySqlDataAdapter(MySqlCommand selectCommand) : this()
		{
			SelectCommand = selectCommand;
		}

		/// <include file='docs/MySqlDataAdapter.xml' path='docs/Ctor2/*'/>
		public MySqlDataAdapter(string selectCommandText, MySqlConnection connection) : this()
		{
			SelectCommand = new MySqlCommand(selectCommandText, connection);
		}

		/// <include file='docs/MySqlDataAdapter.xml' path='docs/Ctor3/*'/>
		public MySqlDataAdapter(string selectCommandText, string selectConnString) : this()
		{
			SelectCommand = new MySqlCommand(selectCommandText, 
				new MySqlConnection(selectConnString) );
		}

		#region Properties

		/// <include file='docs/MySqlDataAdapter.xml' path='docs/DeleteCommand/*'/>
#if !CF
		[Description("Used during Update for deleted rows in Dataset.")]
#endif
		public new MySqlCommand DeleteCommand 
		{
			get { return (MySqlCommand)base.DeleteCommand; }
			set { base.DeleteCommand = value; }
		}

		/// <include file='docs/MySqlDataAdapter.xml' path='docs/InsertCommand/*'/>
#if !CF
		[Description("Used during Update for new rows in Dataset.")]
#endif
		public new MySqlCommand InsertCommand 
		{
			get { return (MySqlCommand)base.InsertCommand; }
			set { base.InsertCommand = value; }
		}

		/// <include file='docs/MySqlDataAdapter.xml' path='docs/SelectCommand/*'/>
#if !CF
		[Description("Used during Fill/FillSchema")]
		[Category("Fill")]
#endif
		public new MySqlCommand SelectCommand 
		{
			get { return (MySqlCommand)base.SelectCommand; }
            set { base.SelectCommand = value; }
		}

		/// <include file='docs/MySqlDataAdapter.xml' path='docs/UpdateCommand/*'/>
#if !CF
		[Description("Used during Update for modified rows in Dataset.")]
#endif
		public new MySqlCommand UpdateCommand 
		{
			get { return (MySqlCommand)base.UpdateCommand; }
			set { base.UpdateCommand = value; }
		}

		internal bool LoadDefaults 
		{
			get { return loadingDefaults; }
			set { loadingDefaults = value; }
		}

		#endregion

        #region Batching Support

        public override int UpdateBatchSize
        {
            get { return updateBatchSize; }
            set { updateBatchSize = value; }
        }

        protected override void InitializeBatching()
        {
            commandBatch = new List<IDbCommand>();
        }

        protected override int AddToBatch(IDbCommand command)
        {
            // the first time each command is asked to be batched, we ask
            // that command to prepare its batchable command text.  We only want
            // to do this one time for each command
            MySqlCommand commandToBatch = (MySqlCommand)command;
            if (commandToBatch.BatchableCommandText == null)
                commandToBatch.GetCommandTextForBatching();

            IDbCommand cloneCommand = (IDbCommand)((ICloneable)command).Clone();
            commandBatch.Add(cloneCommand);

            return commandBatch.Count - 1;
        }

        protected override int ExecuteBatch()
        {
            int recordsAffected = 0;
            int index = 0;
            while (index < commandBatch.Count)
            {
                MySqlCommand cmd = (MySqlCommand)commandBatch[index++];
                for (int index2 = index; index2 < commandBatch.Count; index2++,index++)
                {
                    MySqlCommand cmd2 = (MySqlCommand)commandBatch[index2];
                    if (cmd2.CommandText != cmd.CommandText) break;
                    cmd.AddToBatch(cmd2);
                }
                recordsAffected += cmd.ExecuteNonQuery();
            }
            return recordsAffected;
        }

        protected override void ClearBatch()
        {
            if (commandBatch.Count > 0)
            {
                MySqlCommand cmd = (MySqlCommand)commandBatch[0];
                if (cmd.Batch != null)
                    cmd.Batch.Clear();
            }
            commandBatch.Clear();
        }

        protected override void TerminateBatching()
        {
            ClearBatch();
            commandBatch = null;
        }

        protected override IDataParameter GetBatchedParameter(int commandIdentifier, int parameterIndex)
        {
            return (IDataParameter)commandBatch[commandIdentifier].Parameters[parameterIndex];
        }

        #endregion

        /*		protected override int Fill(DataTable dataTable, IDataReader dataReader)
		{
			int result = base.Fill (dataTable, dataReader);
			//LoadDefaultValues(dataTable, dataReader);
			return result;
		}

		protected override int Fill(DataSet dataSet, string srcTable, IDataReader dataReader, int startRecord, int maxRecords)
		{
			int result = base.Fill (dataSet, srcTable, dataReader, startRecord, maxRecords);
			//LoadDefaultValues(dataSet.Tables[srcTable], dataReader);
			return result;
		}

*/
/*		private void LoadDefaultValues(DataTable dataTable, IDataReader reader)
		{
			if (! loadingDefaults) return;
			if (dataTable.ExtendedProperties["DefaultsChecked"] != null) return;
			if (this.MissingSchemaAction != MissingSchemaAction.Add &&
				this.MissingSchemaAction != MissingSchemaAction.AddWithKey) return;
			
			DataTable schemaTable = reader.GetSchemaTable();
			reader.Close();

			DatabaseMetaData dmd = new DatabaseMetaData(this.SelectCommand.Connection);

			foreach (DataRow row in schemaTable.Rows)
			{
				DataRow dmdRow = dmd.GetColumn(row["BaseCatalogName"].ToString(), 
					null, row["BaseTableName"].ToString(), row["BaseColumnName"].ToString() );
				object defaultVal = dmdRow["COLUMN_DEFAULT"];
				DataColumn col = dataTable.Columns[row["ColumnName"].ToString()];
				if (defaultVal != System.DBNull.Value)
				{
					if (! col.AllowDBNull) col.ExtendedProperties.Add("UseDefault", true);
					col.AllowDBNull = true;
					mayUseDefault = true;
				}
			}

			dataTable.ExtendedProperties.Add("DefaultsChecked", true);
		}

*/

/*		private void FixupStatementDefaults(RowUpdatingEventArgs args)
		{
			DataTable table = args.Row.Table;
			DataRow row = args.Row;

			savedSql = args.Command.CommandText;
			string newSql = savedSql;

			if (mayUseDefault)
				this.InsertCommand.Unprepare();

			foreach (IDataParameter p in args.Command.Parameters)
			{
				if (row[p.SourceColumn] != DBNull.Value) continue;
				DataColumn col = table.Columns[p.SourceColumn];
				if (! col.ExtendedProperties.ContainsKey("UseDefault")) continue;
				newSql = newSql.Replace("?"+p.ParameterName, "DEFAULT");
			}
			args.Command.CommandText = newSql;
		}
*/		
		/*
		* Implement abstract methods inherited from DbDataAdapter.
		*/
		/// <summary>
		/// Overridden. See <see cref="DbDataAdapter.CreateRowUpdatedEvent"/>.
		/// </summary>
		/// <param name="dataRow"></param>
		/// <param name="command"></param>
		/// <param name="statementType"></param>
		/// <param name="tableMapping"></param>
		/// <returns></returns>
		override protected RowUpdatedEventArgs CreateRowUpdatedEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
		{
			return new MySqlRowUpdatedEventArgs(dataRow, command, statementType, tableMapping);
		}

		/// <summary>
		/// Overridden. See <see cref="DbDataAdapter.CreateRowUpdatingEvent"/>.
		/// </summary>
		/// <param name="dataRow"></param>
		/// <param name="command"></param>
		/// <param name="statementType"></param>
		/// <param name="tableMapping"></param>
		/// <returns></returns>
		override protected RowUpdatingEventArgs CreateRowUpdatingEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
		{
			return new MySqlRowUpdatingEventArgs(dataRow, command, statementType, tableMapping);
		}

		/// <summary>
		/// Overridden. Raises the RowUpdating event.
		/// </summary>
		/// <param name="value">A MySqlRowUpdatingEventArgs that contains the event data.</param>
		override protected void OnRowUpdating(RowUpdatingEventArgs value)
		{
	//		MySqlRowUpdatingEventArgs args = (value as MySqlRowUpdatingEventArgs);
			//			if (args.StatementType == StatementType.Insert)
//				FixupStatementDefaults(args);

            if (RowUpdating != null)
                RowUpdating(this, (value as MySqlRowUpdatingEventArgs));

//			MySqlRowUpdatingEventHandler handler = (MySqlRowUpdatingEventHandler) Events[EventRowUpdating];
//			if ((null != handler) && (margs != null)) 
//			{
//				handler(this, margs);
//			}
		}

		/// <summary>
		/// Overridden. Raises the RowUpdated event.
		/// </summary>
		/// <param name="value">A MySqlRowUpdatedEventArgs that contains the event data. </param>
		override protected void OnRowUpdated(RowUpdatedEventArgs value)
		{
//			MySqlRowUpdatedEventArgs margs = (value as MySqlRowUpdatedEventArgs);
			//args.Command.CommandText = savedSql;

            if (RowUpdated != null)
                RowUpdated(this, (value as MySqlRowUpdatedEventArgs));

  //          MySqlRowUpdatedEventHandler handler = (MySqlRowUpdatedEventHandler)Events[EventRowUpdated];
	//		if ((null != handler) && (margs != null))
	//		{
	//			handler(this, margs);
	//		}
		}
	}

	/// <summary>
	/// Represents the method that will handle the <see cref="MySqlDataAdapter.RowUpdating"/> event of a <see cref="MySqlDataAdapter"/>.
	/// </summary>
	public delegate void MySqlRowUpdatingEventHandler(object sender, MySqlRowUpdatingEventArgs e);

	/// <summary>
	/// Represents the method that will handle the <see cref="MySqlDataAdapter.RowUpdated"/> event of a <see cref="MySqlDataAdapter"/>.
	/// </summary>
	public delegate void MySqlRowUpdatedEventHandler(object sender, MySqlRowUpdatedEventArgs e);

	/// <summary>
	/// Provides data for the RowUpdating event. This class cannot be inherited.
	/// </summary>
	public sealed class MySqlRowUpdatingEventArgs : RowUpdatingEventArgs
	{
		/// <summary>
		/// Initializes a new instance of the MySqlRowUpdatingEventArgs class.
		/// </summary>
		/// <param name="row">The <see cref="DataRow"/> to 
        /// <see cref="DbDataAdapter.Update(DataSet)"/>.</param>
		/// <param name="command">The <see cref="IDbCommand"/> to execute during <see cref="DbDataAdapter.Update(DataSet)"/>.</param>
		/// <param name="statementType">One of the <see cref="StatementType"/> values that specifies the type of query executed.</param>
		/// <param name="tableMapping">The <see cref="DataTableMapping"/> sent through an <see cref="DbDataAdapter.Update(DataSet)"/>.</param>
		public MySqlRowUpdatingEventArgs(DataRow row, IDbCommand command, StatementType statementType, DataTableMapping tableMapping) 
			: base(row, command, statementType, tableMapping) 
		{
		}

		/// <summary>
		/// Gets or sets the MySqlCommand to execute when performing the Update.
		/// </summary>
		new public MySqlCommand Command
		{
			get  { return (MySqlCommand)base.Command; }
			set  { base.Command = value; }
		}
	}

	/// <summary>
	/// Provides data for the RowUpdated event. This class cannot be inherited.
	/// </summary>
	public sealed class MySqlRowUpdatedEventArgs : RowUpdatedEventArgs
	{
		/// <summary>
		/// Initializes a new instance of the MySqlRowUpdatedEventArgs class.
		/// </summary>
		/// <param name="row">The <see cref="DataRow"/> sent through an <see cref="DbDataAdapter.Update(DataSet)"/>.</param>
		/// <param name="command">The <see cref="IDbCommand"/> executed when <see cref="DbDataAdapter.Update(DataSet)"/> is called.</param>
		/// <param name="statementType">One of the <see cref="StatementType"/> values that specifies the type of query executed.</param>
		/// <param name="tableMapping">The <see cref="DataTableMapping"/> sent through an <see cref="DbDataAdapter.Update(DataSet)"/>.</param>
		public MySqlRowUpdatedEventArgs(DataRow row, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
			: base(row, command, statementType, tableMapping) 
		{
		}

		/// <summary>
		/// Gets or sets the MySqlCommand executed when Update is called.
		/// </summary>
		new public MySqlCommand Command
		{
			get  { return (MySqlCommand)base.Command; }
		}
	}
}
