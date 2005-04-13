// Copyright (C) 2004 MySQL AB
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

namespace MySql.Data.MySqlClient
{
	/// <include file='docs/MySqlDataAdapter.xml' path='docs/class/*'/>
#if DESIGN
	[System.Drawing.ToolboxBitmap( typeof(MySqlDataAdapter), "MySqlClient.resources.dataadapter.bmp")]
	[System.ComponentModel.DesignerCategory("Code")]
	[Designer("MySql.Data.MySqlClient.Design.MySqlDataAdapterDesigner,MySqlClient.Design")]
#endif
	public sealed class MySqlDataAdapter : DbDataAdapter, IDbDataAdapter, IDataAdapter, ICloneable
	{
		private MySqlCommand m_selectCommand;
		private MySqlCommand m_insertCommand;
		private MySqlCommand m_updateCommand;
		private MySqlCommand m_deleteCommand;

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
		}

		/// <include file='docs/MySqlDataAdapter.xml' path='docs/Ctor1/*'/>
		public MySqlDataAdapter( MySqlCommand selectCommand ) 
		{
			SelectCommand = selectCommand;
		}

		/// <include file='docs/MySqlDataAdapter.xml' path='docs/Ctor2/*'/>
		public MySqlDataAdapter( string selectCommandText, MySqlConnection conn) 
		{
			SelectCommand = new MySqlCommand( selectCommandText, conn );
		}

		/// <include file='docs/MySqlDataAdapter.xml' path='docs/Ctor3/*'/>
		public MySqlDataAdapter( string selectCommandText, string selectConnString) 
		{
			SelectCommand = new MySqlCommand( selectCommandText, 
				new MySqlConnection(selectConnString) );
		}

		#region Properties

		/// <include file='docs/MySqlDataAdapter.xml' path='docs/DeleteCommand/*'/>
#if DESIGN
		[Description("Used during Update for deleted rows in Dataset.")]
#endif
		public MySqlCommand DeleteCommand 
		{
			get { return m_deleteCommand; }
			set { m_deleteCommand = value; }
		}

		IDbCommand IDbDataAdapter.DeleteCommand 
		{
			get { return m_deleteCommand; }
			set { m_deleteCommand = (MySqlCommand)value; }
		}

		/// <include file='docs/MySqlDataAdapter.xml' path='docs/InsertCommand/*'/>
#if DESIGN
		[Description("Used during Update for new rows in Dataset.")]
#endif
		public MySqlCommand InsertCommand 
		{
			get { return m_insertCommand; }
			set { m_insertCommand = value; }
		}

		IDbCommand IDbDataAdapter.InsertCommand 
		{
			get { return m_insertCommand; }
			set { m_insertCommand = (MySqlCommand)value; }
		}

		/// <include file='docs/MySqlDataAdapter.xml' path='docs/SelectCommand/*'/>
#if DESIGN
		[Description("Used during Fill/FillSchema")]
		[Category("Fill")]
#endif
		public MySqlCommand SelectCommand 
		{
			get { return m_selectCommand; }
			set { m_selectCommand = value; }
		}

		IDbCommand IDbDataAdapter.SelectCommand 
		{
			get { return m_selectCommand; }
			set { m_selectCommand = (MySqlCommand)value; }
		}

		/// <include file='docs/MySqlDataAdapter.xml' path='docs/UpdateCommand/*'/>
#if DESIGN
		[Description("Used during Update for modified rows in Dataset.")]
#endif
		public MySqlCommand UpdateCommand 
		{
			get { return m_updateCommand; }
			set { m_updateCommand = value; }
		}

		IDbCommand IDbDataAdapter.UpdateCommand 
		{
			get { return m_updateCommand; }
			set { m_updateCommand = (MySqlCommand)value; }
		}

		#endregion

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
			if (RowUpdating != null)
				RowUpdating(this, (MySqlRowUpdatingEventArgs)value);
		}

		/// <summary>
		/// Overridden. Raises the RowUpdated event.
		/// </summary>
		/// <param name="value">A MySqlRowUpdatedEventArgs that contains the event data. </param>
		override protected void OnRowUpdated(RowUpdatedEventArgs value)
		{
			if (RowUpdated != null)
				RowUpdated(this, (MySqlRowUpdatedEventArgs)value);
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
		/// <param name="row">The <see cref="DataRow"/> to <see cref="DbDataAdapter.Update"/>.</param>
		/// <param name="command">The <see cref="IDbCommand"/> to execute during <see cref="DbDataAdapter.Update"/>.</param>
		/// <param name="statementType">One of the <see cref="StatementType"/> values that specifies the type of query executed.</param>
		/// <param name="tableMapping">The <see cref="DataTableMapping"/> sent through an <see cref="DbDataAdapter.Update"/>.</param>
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
		/// <param name="row">The <see cref="DataRow"/> sent through an <see cref="DbDataAdapter.Update"/>.</param>
		/// <param name="command">The <see cref="IDbCommand"/> executed when <see cref="DbDataAdapter.Update"/> is called.</param>
		/// <param name="statementType">One of the <see cref="StatementType"/> values that specifies the type of query executed.</param>
		/// <param name="tableMapping">The <see cref="DataTableMapping"/> sent through an <see cref="DbDataAdapter.Update"/>.</param>
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
