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
using System.Data.Common;
using System.IO;
using System.Collections;
using System.Text;
using MySql.Data.Common;
using System.ComponentModel;

namespace MySql.Data.MySqlClient
{
	/// <include file='docs/mysqlcommand.xml' path='docs/ClassSummary/*'/>
#if !CF
	[System.Drawing.ToolboxBitmap( typeof(MySqlCommand), "MySqlClient.resources.command.bmp")]
	[System.ComponentModel.DesignerCategory("Code")]
#endif
	public sealed class MySqlCommand : DbCommand, ICloneable
	{
		MySqlConnection				connection;
		MySqlTransaction			curTransaction;
		string						cmdText;
		CommandType					cmdType;
		long						updatedRowCount;
		UpdateRowSource				updatedRowSource;
		MySqlParameterCollection	parameters;
		private ArrayList			parameterMap;
		private int					cursorPageSize;
		private IAsyncResult		asyncResult;
        private bool                designTimeVisible;
        internal Int64 lastInsertedId;
        private Statement statement;

		/// <include file='docs/mysqlcommand.xml' path='docs/ctor1/*'/>
		public MySqlCommand()
		{
            designTimeVisible = true;
			cmdType = CommandType.Text;
			parameterMap = new ArrayList();
			parameters = new MySqlParameterCollection();
			updatedRowSource = UpdateRowSource.Both;
			cursorPageSize = 0;
            cmdText = String.Empty;
		}

		/// <include file='docs/mysqlcommand.xml' path='docs/ctor2/*'/>
		public MySqlCommand(string cmdText) : this()
		{
			CommandText = cmdText;
		}

		/// <include file='docs/mysqlcommand.xml' path='docs/ctor3/*'/>
		public MySqlCommand(string cmdText, MySqlConnection connection) : this(cmdText)
		{
			Connection = connection;
			if (connection != null)
				parameters.ParameterMarker = connection.ParameterMarker;
		}

		/// <include file='docs/mysqlcommand.xml' path='docs/ctor4/*'/>
		public MySqlCommand(string cmdText, MySqlConnection connection, 
            MySqlTransaction transaction) : 
			this(cmdText, connection)
		{
			curTransaction	= transaction;
		} 

		#region Properties


        /// <include file='docs/mysqlcommand.xml' path='docs/LastInseredId/*'/>
#if !CF
		[Browsable(false)]
#endif
        public Int64 LastInsertedId
        {
            get { return lastInsertedId; }
        }

		/// <include file='docs/mysqlcommand.xml' path='docs/CommandText/*'/>
#if !CF
		[Category("Data")]
		[Description("Command text to execute")]
		[Editor("MySql.Data.Common.Design.SqlCommandTextEditor,MySqlClient.Design", typeof(System.Drawing.Design.UITypeEditor))]
#endif
		public override string CommandText
		{
			get { return cmdText; }
			set 
            { 
                cmdText = value;  
                statement=null;
                if (cmdText.EndsWith("DEFAULT VALUES"))
                {
                    cmdText = cmdText.Substring(0, cmdText.Length - 14);
                    cmdText = cmdText + "() VALUES ()";
                }

            }
		}

		internal int UpdateCount 
		{
			get { return (int)updatedRowCount; }
		}

/*        internal int StatementId
        {
            get
            {
                if (this.preparedStatement == null)
                    return -1;
                return (preparedStatement.StatementId);
            }
        }*/

		/// <include file='docs/mysqlcommand.xml' path='docs/CommandTimeout/*'/>
#if !CF
		[Category("Misc")]
		[Description("Time to wait for command to execute")]
#endif
		public override int CommandTimeout
		{
			// TODO: support this
			get  { return 0; }
			set  { if (value != 0) throw new NotSupportedException(); }
		}

		/// <include file='docs/mysqlcommand.xml' path='docs/CommandType/*'/>
#if !CF
		[Category("Data")]
#endif
		public override CommandType CommandType
		{
			get { return cmdType; }
            set { cmdType = value; SyncCommandType(true);  }
		}

		/// <include file='docs/mysqlcommand.xml' path='docs/IsPrepared/*'/>
#if !CF
		[Browsable(false)]
#endif
		public bool IsPrepared 
		{
            get { return statement != null && statement is PreparedStatement; }
		}

			/// <include file='docs/mysqlcommand.xml' path='docs/Connection/*'/>
#if !CF
		[Category("Behavior")]
		[Description("Connection used by the command")]
#endif
		public new MySqlConnection Connection
		{
			get { return connection;  }
			set
			{
				/*
				* The connection is associated with the transaction
				* so set the transaction object to return a null reference if the connection 
				* is reset.
				*/
				if (connection != value)
					this.Transaction = null;

				connection = (MySqlConnection)value;
				if (connection != null)
					parameters.ParameterMarker = connection.ParameterMarker;
			}
		}

		/// <include file='docs/mysqlcommand.xml' path='docs/Parameters/*'/>
#if !CF
		[Category("Data")]
		[Description("The parameters collection")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
#endif
		public new MySqlParameterCollection Parameters
		{
			get  { return parameters; }
		}


		/// <include file='docs/mysqlcommand.xml' path='docs/Transaction/*'/>
#if !CF
		[Browsable(false)]
#endif
		public new MySqlTransaction Transaction
		{
			get { return curTransaction; }
			set { curTransaction = (MySqlTransaction)value; }
		}

/*		/// <include file='docs/mysqlcommand.xml' path='docs/UpdatedRowSource/*'/>
#if !CF
		[Category("Behavior")]
#endif
		public override UpdateRowSource UpdatedRowSource
		{
			get { return updatedRowSource;  }
			set { updatedRowSource = value; }
		}*/
		#endregion

		#region Methods

		/// <summary>
		/// Attempts to cancel the execution of a MySqlCommand.  This operation is not supported.
		/// </summary>
		/// <remarks>
		/// Cancelling an executing command is currently not supported on any version of MySQL.
		/// </remarks>
		/// <exception cref="NotSupportedException">This operation is not supported.</exception>
		public override void Cancel()
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Creates a new instance of a <see cref="MySqlParameter"/> object.
		/// </summary>
		/// <remarks>
		/// This method is a strongly-typed version of <see cref="IDbCommand.CreateParameter"/>.
		/// </remarks>
		/// <returns>A <see cref="MySqlParameter"/> object.</returns>
		/// 
		public new MySqlParameter CreateParameter()
		{
            return (MySqlParameter)CreateDbParameter();
		}

		/// <summary>
		/// Check the connection to make sure
		///		- it is open
		///		- it is not currently being used by a reader
		///		- and we have the right version of MySQL for the requested command type
		/// </summary>
		private void CheckState()
		{
			// There must be a valid and open connection.
			if (connection == null || connection.State != ConnectionState.Open)
				throw new InvalidOperationException("Connection must be valid and open");

			// Data readers have to be closed first
			if (connection.Reader != null && cursorPageSize == 0)
				throw new MySqlException("There is already an open DataReader associated with this Connection which must be closed first.");

			if (CommandType == CommandType.StoredProcedure && ! connection.driver.Version.isAtLeast(5,0,0))
				throw new MySqlException( "Stored procedures are not supported on this version of MySQL" );
		}

		/// <include file='docs/mysqlcommand.xml' path='docs/ExecuteNonQuery/*'/>
		public override int ExecuteNonQuery()
		{
            lastInsertedId = -1;
            MySqlDataReader reader = ExecuteReader();
            reader.Close();
            lastInsertedId = reader.InsertedId;
            this.updatedRowCount = reader.RecordsAffected;
            return reader.RecordsAffected;
        }

        internal void Close()
        {
            if (statement != null)
                statement.Close();
        }

		/// <include file='docs/mysqlcommand.xml' path='docs/ExecuteReader/*'/>
		public new MySqlDataReader ExecuteReader()
		{
			return ExecuteReader(CommandBehavior.Default);
		}


		/// <include file='docs/mysqlcommand.xml' path='docs/ExecuteReader1/*'/>
		public new MySqlDataReader ExecuteReader(CommandBehavior behavior)
		{
            lastInsertedId = -1;
			CheckState();

			string sql = TrimSemicolons(cmdText);

            //TODO: make these work with prepared statements and stored procedures
			if (0 != (behavior & CommandBehavior.SchemaOnly))
			{
                sql = String.Format("SET SQL_SELECT_LIMIT=0;{0};SET sql_select_limit=-1;", cmdText);
			}

			if (0 != (behavior & CommandBehavior.SingleRow))
			{
				sql = String.Format("SET SQL_SELECT_LIMIT=1;{0};SET sql_select_limit=-1;", cmdText);
			}
            
            if (statement == null)
            {
                if (CommandType == CommandType.StoredProcedure)
                    statement = new StoredProcedure(this.Connection, sql);
                else
                    statement = new Statement(this.Connection, sql);
            }

			updatedRowCount = -1;

			MySqlDataReader reader = new MySqlDataReader(this, statement, behavior);

            // execute the statement
            statement.Execute(Parameters);

			reader.NextResult();
			connection.Reader = reader;
			return reader;
		}

		/// <include file='docs/mysqlcommand.xml' path='docs/ExecuteScalar/*'/>
		public override object ExecuteScalar()
		{
            lastInsertedId = -1;
			object val = null;
			MySqlDataReader reader = ExecuteReader();
			if (reader.Read())
				val = reader.GetValue(0);
			reader.Close();
            lastInsertedId = reader.InsertedId;

			return val;
		}

		/// <include file='docs/mysqlcommand.xml' path='docs/Prepare2/*'/>
		private void Prepare(int cursorPageSize) 
		{
			if (! connection.driver.Version.isAtLeast(5,0,0) && cursorPageSize > 0)
				throw new InvalidOperationException("Nested commands are only supported on MySQL 5.0 and later");

            PreparedStatement ps = new PreparedStatement(connection, CommandText, cursorPageSize);
            ps.Prepare();
            statement = ps;
		}

		/// <include file='docs/mysqlcommand.xml' path='docs/Prepare/*'/>
		public override void Prepare()
		{
			if (connection == null)
				throw new InvalidOperationException("The connection property has not been set.");
			if (connection.State != ConnectionState.Open)
				throw new InvalidOperationException("The connection is not open.");
			if (! connection.driver.Version.isAtLeast( 4,1,0)) 
                return;

            Prepare(0);
		}
		#endregion

		#region Async Methods

		internal delegate int AsyncExecuteNonQueryDelegate();
        internal delegate MySqlDataReader AsyncExecuteReaderDelegate(CommandBehavior behavior);

		private string TrimSemicolons(string sql)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder(sql);
			int start = 0;
			while (sb[start] == ';')
				start++;

			int end = sb.Length-1;
			while (sb[end] == ';')
				end--;
			return sb.ToString(start, end-start+1);
		}

        public IAsyncResult BeginExecuteReader()
        {
            return BeginExecuteReader(CommandBehavior.Default);
        }

        public IAsyncResult BeginExecuteReader(CommandBehavior behavior)
        {
            AsyncExecuteReaderDelegate del = new AsyncExecuteReaderDelegate(ExecuteReader);
            asyncResult = del.BeginInvoke(behavior, null, null);
            return asyncResult;
        }

        public MySqlDataReader EndExecuteReader(IAsyncResult result)
        {
            result.AsyncWaitHandle.WaitOne();
            return connection.Reader;
        }

        public IAsyncResult BeginExecuteNonQuery(AsyncCallback callback, object stateObject)
        {
            AsyncExecuteNonQueryDelegate del =
                new AsyncExecuteNonQueryDelegate(ExecuteNonQuery);
            asyncResult = del.BeginInvoke(callback, stateObject);
            return asyncResult;
        }

		public IAsyncResult BeginExecuteNonQuery() 
		{
			AsyncExecuteNonQueryDelegate del = 
				new AsyncExecuteNonQueryDelegate(ExecuteNonQuery);
			asyncResult = del.BeginInvoke(null, null);
			return asyncResult;
		}

		public int EndExecuteNonQuery(IAsyncResult result)
		{
            result.AsyncWaitHandle.WaitOne();
			return (int)updatedRowCount;
		}

		#endregion

		#region Private Methods

        private void SyncCommandType(bool cmdTypeSet)
        {
            int i = (int)CommandType;
        }

/*		private ArrayList PrepareSqlBuffers(string sql)
		{
			ArrayList buffers = new ArrayList();
			MySqlStreamWriter writer = new MySqlStreamWriter(new MemoryStream(), connection.Encoding);
			writer.Version = connection.driver.Version;

			// if we are executing as a stored procedure, then we need to add the call
			// keyword.
			if (CommandType == CommandType.StoredProcedure)
			{
				if (storedProcedure == null)
					storedProcedure = new StoredProcedure(this);
				sql = storedProcedure.Prepare( CommandText );
			}

			// tokenize the SQL
			sql = sql.TrimStart(';').TrimEnd(';');
			ArrayList tokens = TokenizeSql( sql );

			foreach (string token in tokens)
			{
				if (token.Trim().Length == 0) continue;
				if (token == ";" && ! connection.driver.SupportsBatch)
				{
					MemoryStream ms = (MemoryStream)writer.Stream;
					if (ms.Length > 0)
						buffers.Add( ms );

					writer = new MySqlStreamWriter(new MemoryStream(), connection.Encoding);
					writer.Version = connection.driver.Version;
					continue;
				}
				else if (token[0] == parameters.ParameterMarker) 
				{
					if (SerializeParameter(writer, token)) continue;
				}

				// our fall through case is to write the token to the byte stream
				writer.WriteStringNoNull(token);
			}

			// capture any buffer that is left over
			MemoryStream mStream = (MemoryStream)writer.Stream;
			if (mStream.Length > 0)
				buffers.Add( mStream );

			return buffers;
		}*/


		#endregion

		#region ICloneable
		/// <summary>
		/// Creates a clone of this MySqlCommand object.  CommandText, Connection, and Transaction properties
		/// are included as well as the entire parameter list.
		/// </summary>
		/// <returns>The cloned MySqlCommand object</returns>
		object ICloneable.Clone() 
		{
			MySqlCommand clone = new MySqlCommand(cmdText, connection, curTransaction);
			foreach (MySqlParameter p in parameters) 
			{
				clone.Parameters.Add((p as ICloneable).Clone());
			}
			return clone;
		}
		#endregion

		#region IDisposable Members

		public new void Dispose()
		{
			base.Dispose(true);
		}

		#endregion

        [Browsable(false)]
        public override bool DesignTimeVisible
        {
            get
            {
                return this.designTimeVisible; 
            }
            set
            {
                this.designTimeVisible = value;
            }
        }

        public override UpdateRowSource UpdatedRowSource
        {
            get
            {
                return this.updatedRowSource;
            }
            set
            {
                this.updatedRowSource = value;
            }
        }

        protected override DbParameter CreateDbParameter()
        {
            return new MySqlParameter();
        }

        protected override DbConnection DbConnection
        {
            get { return this.Connection; }
            set { this.Connection = (MySqlConnection)value; }
        }

        protected override DbParameterCollection DbParameterCollection
        {
            get { return this.Parameters; }
        }

        protected override DbTransaction DbTransaction
        {
            get { return this.Transaction; }
            set { this.Transaction = (MySqlTransaction)value; }
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            return this.ExecuteReader(behavior);
        }
    }
}

