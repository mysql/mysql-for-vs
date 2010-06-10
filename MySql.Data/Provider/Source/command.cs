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
using System.IO;
using System.Collections;
using System.Text;
using MySql.Data.Common;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using MySql.Data.MySqlClient.Properties;

namespace MySql.Data.MySqlClient
{
	/// <include file='docs/mysqlcommand.xml' path='docs/ClassSummary/*'/> 
#if !CF
	[System.Drawing.ToolboxBitmap(typeof(MySqlCommand), "MySqlClient.resources.command.bmp")]
	[System.ComponentModel.DesignerCategory("Code")]
#endif
	public sealed class MySqlCommand : DbCommand, ICloneable
	{
		MySqlConnection connection;
		MySqlTransaction curTransaction;
		string cmdText;
		CommandType cmdType;
		long updatedRowCount;
		UpdateRowSource updatedRowSource;
		MySqlParameterCollection parameters;
		private IAsyncResult asyncResult;
		private bool designTimeVisible;
		internal Int64 lastInsertedId;
		private PreparableStatement statement;
		private int commandTimeout;
		private bool timedOut, canceled;
        private bool resetSqlSelect;
        List<MySqlCommand> batch;
        private string batchableCommandText;

		/// <include file='docs/mysqlcommand.xml' path='docs/ctor1/*'/>
		public MySqlCommand()
		{
			designTimeVisible = true;
			cmdType = CommandType.Text;
			parameters = new MySqlParameterCollection(this);
			updatedRowSource = UpdateRowSource.Both;
			cmdText = String.Empty;
		}

		/// <include file='docs/mysqlcommand.xml' path='docs/ctor2/*'/>
		public MySqlCommand(string cmdText)
			: this()
		{
			CommandText = cmdText;
		}

		/// <include file='docs/mysqlcommand.xml' path='docs/ctor3/*'/>
		public MySqlCommand(string cmdText, MySqlConnection connection)
			: this(cmdText)
		{
			Connection = connection;
		}

		/// <include file='docs/mysqlcommand.xml' path='docs/ctor4/*'/>
		public MySqlCommand(string cmdText, MySqlConnection connection,
				MySqlTransaction transaction)
			:
			this(cmdText, connection)
		{
			curTransaction = transaction;
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
				statement = null;
                batchableCommandText = null;
				if (cmdText != null && cmdText.EndsWith("DEFAULT VALUES"))
				{
					cmdText = cmdText.Substring(0, cmdText.Length - 14);
					cmdText = cmdText + "() VALUES ()";
				}
			}
		}

		/// <include file='docs/mysqlcommand.xml' path='docs/CommandTimeout/*'/>
#if !CF
		[Category("Misc")]
		[Description("Time to wait for command to execute")]
		[DefaultValue(30)]
#endif
		public override int CommandTimeout
		{
			get { return commandTimeout == 0 ? 30 : commandTimeout; }
			set { commandTimeout = value; }
		}

		/// <include file='docs/mysqlcommand.xml' path='docs/CommandType/*'/>
#if !CF
		[Category("Data")]
#endif
		public override CommandType CommandType
		{
			get { return cmdType; }
			set { cmdType = value; }
		}

		/// <include file='docs/mysqlcommand.xml' path='docs/IsPrepared/*'/>
#if !CF
		[Browsable(false)]
#endif
		public bool IsPrepared
		{
			get { return statement != null && statement.IsPrepared; }
		}

		/// <include file='docs/mysqlcommand.xml' path='docs/Connection/*'/>
#if !CF
		[Category("Behavior")]
		[Description("Connection used by the command")]
#endif
		public new MySqlConnection Connection
		{
			get { return connection; }
			set
			{
				/*
				* The connection is associated with the transaction
				* so set the transaction object to return a null reference if the connection 
				* is reset.
				*/
				if (connection != value)
					Transaction = null;

				connection = value;

                // if the user has not already set the command timeout, then
                // take the default from the connection
                if (connection != null && commandTimeout == 0)
                    commandTimeout = (int)connection.Settings.DefaultCommandTimeout;
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
			get { return parameters; }
		}


		/// <include file='docs/mysqlcommand.xml' path='docs/Transaction/*'/>
#if !CF
		[Browsable(false)]
#endif
		public new MySqlTransaction Transaction
		{
			get { return curTransaction; }
			set { curTransaction = value; }
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

        internal List<MySqlCommand> Batch
        {
            get { return batch; }
        }

        internal bool TimedOut
        {
            get { return timedOut; }
        }

        internal string BatchableCommandText
        {
            get { return batchableCommandText; }
        }

		#endregion

		#region Methods

		/// <summary>
		/// Attempts to cancel the execution of a MySqlCommand.
		/// </summary>
		/// <remarks>
		/// Cancelling a currently active query only works with MySQL versions 5.0.0 and higher.
		/// </remarks>
		public override void Cancel()
		{
            if (canceled) return;

			if (!connection.driver.Version.isAtLeast(5, 0, 0))
				throw new NotSupportedException(Resources.CancelNotSupported);

            MySqlConnectionStringBuilder cb = new MySqlConnectionStringBuilder(
                connection.Settings.GetConnectionString(true));
            cb.Pooling = false;
			using(MySqlConnection c = new MySqlConnection(cb.ConnectionString))
			{
                c.Open();
                MySqlCommand cmd = new MySqlCommand(String.Format("KILL QUERY {0}",
                     connection.ServerThread), c);
                cmd.ExecuteNonQuery();
                canceled = true;
			}
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
            if (connection == null)
                throw new InvalidOperationException("Connection must be valid and open.");

            if (connection.State != ConnectionState.Open && !connection.SoftClosed)
                throw new InvalidOperationException("Connection must be valid and open.");

			// Data readers have to be closed first
			if (connection.Reader != null)
				throw new MySqlException("There is already an open DataReader associated with this Connection which must be closed first.");

            if (CommandType == CommandType.TableDirect)
                throw new InvalidOperationException(Resources.TableDirectNotSupported);

			if (CommandType == CommandType.StoredProcedure && !connection.driver.Version.isAtLeast(5, 0, 0))
				throw new MySqlException("Stored procedures are not supported on this version of MySQL");
		}

		/// <include file='docs/mysqlcommand.xml' path='docs/ExecuteNonQuery/*'/>
		public override int ExecuteNonQuery()
		{
            MySqlDataReader reader = null;
            using (reader = ExecuteReader())
            { 
            }

            return reader.RecordsAffected;
		}

		internal void Close(MySqlDataReader reader)
		{
			if (statement != null)
				statement.Close(reader);
            ResetSqlSelectLimit();
        }

        /// <summary>
        /// Reset SQL_SELECT_LIMIT that could have been modified by CommandBehavior.
        /// </summary>
        internal void ResetSqlSelectLimit()
        {
            // if we are supposed to reset the sql select limit, do that here
            if (resetSqlSelect)
                new MySqlCommand("SET SQL_SELECT_LIMIT=DEFAULT", connection).ExecuteNonQuery();
            resetSqlSelect = false;
        }

		/// <include file='docs/mysqlcommand.xml' path='docs/ExecuteReader/*'/>
		public new MySqlDataReader ExecuteReader()
		{
			return ExecuteReader(CommandBehavior.Default);
		}

		private void TimeoutExpired(object commandObject)
		{
			MySqlCommand cmd = (commandObject as MySqlCommand);
            if (cmd == null)
            {
                Logger.LogWarning(Resources.TimeoutExpiredNullObject);
                return;
            }

            cmd.timedOut = true;
            try
            {
                cmd.Cancel();
            }
            catch (Exception ex)
            {
                // if something goes wrong, we log it and try to close the connection. 
                // There's really nothing else we can do.
                if (connection.Settings.Logging)
                    Logger.LogException(ex);

                try
                {
                    cmd.Connection.Close();
                }
                catch (Exception ex1)
                {
                    if (connection.Settings.Logging)
                        Logger.LogException(ex1);
                }
            }
		}

		/// <include file='docs/mysqlcommand.xml' path='docs/ExecuteReader1/*'/>
		public new MySqlDataReader ExecuteReader(CommandBehavior behavior)
		{
			lastInsertedId = -1;
			CheckState();

			if (cmdText == null ||
				 cmdText.Trim().Length == 0)
				throw new InvalidOperationException(Resources.CommandTextNotInitialized);

			string sql = TrimSemicolons(cmdText);

            // now we check to see if we are executing a query that is buggy
            // in 4.1
            connection.IsExecutingBuggyQuery = false;
            if (!connection.driver.Version.isAtLeast(5, 0, 0) &&
                connection.driver.Version.isAtLeast(4, 1, 0))
            {
                string snippet = sql;
                if (snippet.Length > 17)
                    snippet = sql.Substring(0, 17);
                snippet = snippet.ToUpper(CultureInfo.InvariantCulture);
                connection.IsExecutingBuggyQuery = 
                    snippet.StartsWith("DESCRIBE") ||
                    snippet.StartsWith("SHOW TABLE STATUS");
            }

			if (statement == null || !statement.IsPrepared)
			{
				if (CommandType == CommandType.StoredProcedure)
					statement = new StoredProcedure(this, sql);
				else
					statement = new PreparableStatement(this, sql);
			}

            // stored procs are the only statement type that need do anything during resolve
            statement.Resolve(false);

            // Now that we have completed our resolve step, we can handle our
            // command behaviors
            HandleCommandBehaviors(behavior);

			updatedRowCount = -1;

            Timer timer = null;
            try
            {
                MySqlDataReader reader = new MySqlDataReader(this, statement, behavior);

                // start a threading timer on our command timeout 
                timedOut = false;
                canceled = false;

                // execute the statement
                statement.Execute();

                // start a timeout timer
                if (connection.driver.Version.isAtLeast(5, 0, 0) &&
                     commandTimeout > 0)
                {
                    TimerCallback timerDelegate =
                         new TimerCallback(TimeoutExpired);
                    timer = new Timer(timerDelegate, this, this.CommandTimeout * 1000, Timeout.Infinite);
                }

                // wait for data to return
                reader.NextResult();
                
                connection.Reader = reader;
                return reader;
            }
            catch (MySqlException ex)
            {
                try
                {
                    ResetSqlSelectLimit();
                }
                catch (Exception) { }

                // if we caught an exception because of a cancel, then just return null
                if (ex.IsQueryAborted)
                {
                    if (TimedOut)
                        throw new MySqlException(Resources.Timeout);
                    return null;
                }
                if (ex.IsFatal)
                    Connection.Close();
                if (ex.Number == 0)
                    throw new MySqlException(Resources.FatalErrorDuringExecute, ex);
                throw;
            }
            finally
            {
                if (timer != null)
                    timer.Dispose();
            }
		}

		/// <include file='docs/mysqlcommand.xml' path='docs/ExecuteScalar/*'/>
		public override object ExecuteScalar()
		{
            lastInsertedId = -1;
            object val = null;

            using (MySqlDataReader reader = ExecuteReader())
            {
                if (reader == null) return null;

                if (reader.Read())
                    val = reader.GetValue(0);
            }

            return val;
		}

        private void HandleCommandBehaviors(CommandBehavior behavior)
        {
            if ((behavior & CommandBehavior.SchemaOnly) != 0)
            {
                new MySqlCommand("SET SQL_SELECT_LIMIT=0", connection).ExecuteNonQuery();
                resetSqlSelect = true;
            }
            else if ((behavior & CommandBehavior.SingleRow) != 0)
            {
                new MySqlCommand("SET SQL_SELECT_LIMIT=1", connection).ExecuteNonQuery();
                resetSqlSelect = true;
            }
        }

		/// <include file='docs/mysqlcommand.xml' path='docs/Prepare2/*'/>
		private void Prepare(int cursorPageSize)
		{
			if (!connection.driver.Version.isAtLeast(5, 0, 0) && cursorPageSize > 0)
				throw new InvalidOperationException("Nested commands are only supported on MySQL 5.0 and later");

			// if the length of the command text is zero, then just return
			string psSQL = CommandText;
			if (psSQL == null ||
				 psSQL.Trim().Length == 0)
				return;

			if (CommandType == CommandType.StoredProcedure)
				statement = new StoredProcedure(this, CommandText);
			else
				statement = new PreparableStatement(this, CommandText);

            statement.Resolve(true);
			statement.Prepare();
		}

		/// <include file='docs/mysqlcommand.xml' path='docs/Prepare/*'/>
		public override void Prepare()
		{
			if (connection == null)
				throw new InvalidOperationException("The connection property has not been set.");
			if (connection.State != ConnectionState.Open)
				throw new InvalidOperationException("The connection is not open.");
			if (!connection.driver.Version.isAtLeast(4, 1, 0) ||
				 connection.Settings.IgnorePrepare)
				return;

			Prepare(0);
		}
		#endregion

		#region Async Methods

		internal delegate object AsyncDelegate(int type, CommandBehavior behavior);
        internal AsyncDelegate caller = null;
		internal Exception thrownException;

		private static string TrimSemicolons(string sql)
		{
			int start = 0;
			while (sql[start] == ';')
				start++;

			int end = sql.Length - 1;
			while (sql[end] == ';')
				end--;
			return sql.Substring(start, end-start+1);
		}

		internal object AsyncExecuteWrapper(int type, CommandBehavior behavior)
		{
			thrownException = null;
			try
			{
                if (type == 1)
                    return ExecuteReader(behavior);
                return ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				thrownException = ex;
			}
            return null;
		}

		/// <summary>
		/// Initiates the asynchronous execution of the SQL statement or stored procedure 
		/// that is described by this <see cref="MySqlCommand"/>, and retrieves one or more 
		/// result sets from the server. 
		/// </summary>
		/// <returns>An <see cref="IAsyncResult"/> that can be used to poll, wait for results, 
		/// or both; this value is also needed when invoking EndExecuteReader, 
		/// which returns a <see cref="MySqlDataReader"/> instance that can be used to retrieve 
		/// the returned rows. </returns>
		public IAsyncResult BeginExecuteReader()
		{
			return BeginExecuteReader(CommandBehavior.Default);
		}

		/// <summary>
		/// Initiates the asynchronous execution of the SQL statement or stored procedure 
		/// that is described by this <see cref="MySqlCommand"/> using one of the 
		/// <b>CommandBehavior</b> values. 
		/// </summary>
		/// <param name="behavior">One of the <see cref="CommandBehavior"/> values, indicating 
		/// options for statement execution and data retrieval.</param>
		/// <returns>An <see cref="IAsyncResult"/> that can be used to poll, wait for results, 
		/// or both; this value is also needed when invoking EndExecuteReader, 
		/// which returns a <see cref="MySqlDataReader"/> instance that can be used to retrieve 
		/// the returned rows. </returns>
		public IAsyncResult BeginExecuteReader(CommandBehavior behavior)
		{
            if (caller != null)
                throw new MySqlException(Resources.UnableToStartSecondAsyncOp);

			caller = new AsyncDelegate(AsyncExecuteWrapper);
			asyncResult = caller.BeginInvoke(1, behavior, null, null);
			return asyncResult;
		}

		/// <summary>
		/// Finishes asynchronous execution of a SQL statement, returning the requested 
		/// <see cref="MySqlDataReader"/>.
		/// </summary>
		/// <param name="result">The <see cref="IAsyncResult"/> returned by the call to 
		/// <see cref="BeginExecuteReader()"/>.</param>
		/// <returns>A <b>MySqlDataReader</b> object that can be used to retrieve the requested rows. </returns>
		public MySqlDataReader EndExecuteReader(IAsyncResult result)
		{
			result.AsyncWaitHandle.WaitOne();
            AsyncDelegate c = caller;
            caller = null;
            if (thrownException != null)
                throw thrownException;
            return (MySqlDataReader)c.EndInvoke(result);
		}

		/// <summary>
		/// Initiates the asynchronous execution of the SQL statement or stored procedure 
		/// that is described by this <see cref="MySqlCommand"/>. 
		/// </summary>
		/// <param name="callback">
		/// An <see cref="AsyncCallback"/> delegate that is invoked when the command's 
		/// execution has completed. Pass a null reference (<b>Nothing</b> in Visual Basic) 
		/// to indicate that no callback is required.</param>
		/// <param name="stateObject">A user-defined state object that is passed to the 
		/// callback procedure. Retrieve this object from within the callback procedure 
		/// using the <see cref="IAsyncResult.AsyncState"/> property.</param>
		/// <returns>An <see cref="IAsyncResult"/> that can be used to poll or wait for results, 
		/// or both; this value is also needed when invoking <see cref="EndExecuteNonQuery"/>, 
		/// which returns the number of affected rows. </returns>
		public IAsyncResult BeginExecuteNonQuery(AsyncCallback callback, object stateObject)
		{
            if (caller != null)
                throw new MySqlException(Resources.UnableToStartSecondAsyncOp);

            caller = new AsyncDelegate(AsyncExecuteWrapper);
			asyncResult = caller.BeginInvoke(2, CommandBehavior.Default, 
				callback, stateObject);
			return asyncResult;
		}

		/// <summary>
		/// Initiates the asynchronous execution of the SQL statement or stored procedure 
		/// that is described by this <see cref="MySqlCommand"/>. 
		/// </summary>
		/// <returns>An <see cref="IAsyncResult"/> that can be used to poll or wait for results, 
		/// or both; this value is also needed when invoking <see cref="EndExecuteNonQuery"/>, 
		/// which returns the number of affected rows. </returns>
		public IAsyncResult BeginExecuteNonQuery()
		{
            if (caller != null)
                throw new MySqlException(Resources.UnableToStartSecondAsyncOp);

            caller = new AsyncDelegate(AsyncExecuteWrapper);
			asyncResult = caller.BeginInvoke(2, CommandBehavior.Default, null, null);
			return asyncResult;
		}

		/// <summary>
		/// Finishes asynchronous execution of a SQL statement. 
		/// </summary>
		/// <param name="asyncResult">The <see cref="IAsyncResult"/> returned by the call 
		/// to <see cref="BeginExecuteNonQuery()"/>.</param>
		/// <returns></returns>
		public int EndExecuteNonQuery(IAsyncResult asyncResult)
		{
			asyncResult.AsyncWaitHandle.WaitOne();
            AsyncDelegate c = caller;
            caller = null;
            if (thrownException != null)
                throw thrownException;
            return (int)c.EndInvoke(asyncResult);
		}

		#endregion

		#region Private Methods

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

        internal long EstimatedSize()
        {
            long size = CommandText.Length;
            foreach (MySqlParameter parameter in Parameters)
                size += parameter.EstimatedSize();
            return size;
        }

		#endregion

		#region ICloneable

		/// <summary>
		/// Creates a clone of this MySqlCommand object.  CommandText, Connection, and Transaction properties
		/// are included as well as the entire parameter list.
		/// </summary>
		/// <returns>The cloned MySqlCommand object</returns>
		public MySqlCommand Clone()
		{
			MySqlCommand clone = new MySqlCommand(cmdText, connection, curTransaction);
            clone.CommandType = CommandType;
            clone.CommandTimeout = CommandTimeout;
            clone.batchableCommandText = batchableCommandText;
            clone.UpdatedRowSource = UpdatedRowSource;

			foreach (MySqlParameter p in parameters)
			{
				clone.Parameters.Add(p.Clone());
			}
			return clone;
		}

        object ICloneable.Clone()
        {
            return this.Clone();
        }

		#endregion

        #region Batching support

        internal void AddToBatch(MySqlCommand command)
        {
            if (batch == null)
                batch = new List<MySqlCommand>();
            batch.Add(command);
        }

        internal string GetCommandTextForBatching()
        {
            if (batchableCommandText == null)
            {
                // if the command starts with insert and is "simple" enough, then
                // we can use the multi-value form of insert
                if (String.Compare(CommandText.Substring(0, 6), "INSERT", true) == 0)
                {
                    MySqlCommand cmd = new MySqlCommand("SELECT @@sql_mode", Connection);
                    string sql_mode = cmd.ExecuteScalar().ToString().ToUpper(CultureInfo.InvariantCulture);
                    MySqlTokenizer tokenizer = new MySqlTokenizer(CommandText);
                    tokenizer.AnsiQuotes = sql_mode.IndexOf("ANSI_QUOTES") != -1;
                    tokenizer.BackslashEscapes = sql_mode.IndexOf("NO_BACKSLASH_ESCAPES") == -1;
                    string token = tokenizer.NextToken().ToLower(CultureInfo.InvariantCulture);
                    while (token != null)
                    {
                        if (token.ToUpper(CultureInfo.InvariantCulture) == "VALUES" && 
                            !tokenizer.Quoted)
                        {
                            token = tokenizer.NextToken();
                            Debug.Assert(token == "(");
                            while (token != null && token != ")")
                            {
                                batchableCommandText += token;
                                token = tokenizer.NextToken();
                            }
                            if (token != null)
                                batchableCommandText += token;
                            token = tokenizer.NextToken();
                            if (token != null && (token == "," || 
                                token.ToUpper(CultureInfo.InvariantCulture) == "ON"))
                            {
                                batchableCommandText = null;
                                break;
                            }
                        }
                        token = tokenizer.NextToken();
                    }
                }
            }

            return batchableCommandText;
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (statement != null && statement.IsPrepared)
                    statement.CloseStatement();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the command object should be visible in a Windows Form Designer control. 
        /// </summary>
#if !CF
        [Browsable(false)]
#endif
		public override bool DesignTimeVisible
		{
			get
			{
				return designTimeVisible;
			}
			set
			{
				designTimeVisible = value;
			}
		}

        /// <summary>
        /// Gets or sets how command results are applied to the DataRow when used by the 
        /// Update method of the DbDataAdapter. 
        /// </summary>
		public override UpdateRowSource UpdatedRowSource
		{
			get
			{
				return updatedRowSource;
			}
			set
			{
				updatedRowSource = value;
			}
		}

		protected override DbParameter CreateDbParameter()
		{
			return new MySqlParameter();
		}

		protected override DbConnection DbConnection
		{
			get { return Connection; }
			set { Connection = (MySqlConnection)value; }
		}

		protected override DbParameterCollection DbParameterCollection
		{
			get { return Parameters; }
		}

		protected override DbTransaction DbTransaction
		{
			get { return Transaction; }
			set { Transaction = (MySqlTransaction)value; }
		}

		protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
		{
			return ExecuteReader(behavior);
		}
	}
}

