// Copyright (C) 2004-2005 MySQL AB
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
using System.Collections.Specialized;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using MySql.Data.Common;

namespace MySql.Data.MySqlClient
{
	/// <include file='docs/MySqlConnection.xml' path='docs/ClassSummary/*'/>
#if DESIGN
	[System.Drawing.ToolboxBitmap( typeof(MySqlConnection), "MySqlClient.resources.connection.bmp")]
	[System.ComponentModel.DesignerCategory("Code")]
	[ToolboxItem(true)]
#endif
	public sealed class MySqlConnection : DbConnection, ICloneable
	{
		internal ConnectionState connectionState;
		internal Driver driver;
		private  MySqlDataReader dataReader;
		private  MySqlConnectionString settings;
		private  UsageAdvisor advisor;
		private  bool hasBeenOpen;
        private SchemaProvider schemaProvider;
        private ProcedureCache procedureCache;
        private PerformanceMonitor perfMonitor;

		/// <include file='docs/MySqlConnection.xml' path='docs/InfoMessage/*'/>
		public event MySqlInfoMessageEventHandler	InfoMessage;


		/// <include file='docs/MySqlConnection.xml' path='docs/DefaultCtor/*'/>
		public MySqlConnection()
		{
			//TODO: add event data to StateChange docs
			settings = new MySqlConnectionString();
			advisor = new UsageAdvisor( this );
		}

		/// <include file='docs/MySqlConnection.xml' path='docs/Ctor1/*'/>
		public MySqlConnection(string connectionString) : this()
		{
            ConnectionString = connectionString;
		}

		#region Interal Methods & Properties

        internal ProcedureCache ProcedureCache
        {
            get { return procedureCache; }
        }

		internal MySqlConnectionString Settings 
		{
			get { return settings; }
		}

		internal MySqlDataReader Reader
		{
			get { return dataReader; }
			set { dataReader = value; }
		}

		internal char ParameterMarker 
		{
			get { if (settings.UseOldSyntax) return '@'; return '?'; }
		}

		internal void OnInfoMessage( MySqlInfoMessageEventArgs args ) 
		{
			if (InfoMessage != null) 
			{
				InfoMessage( this, args );
			}
		}

        internal PerformanceMonitor PerfMonitor
        {
            get { return perfMonitor; }
        }

		#endregion

		#region Properties

#if DESIGN
		[Browsable(false)]
#endif
		internal UsageAdvisor UsageAdvisor 
		{
			get { return advisor; }
		}

		/// <summary>
		/// Returns the id of the server thread this connection is executing on
		/// </summary>
#if DESIGN
		[Browsable(false)] 
#endif
		public int ServerThread 
		{
			get { return driver.ThreadID; }
		}

		/// <summary>
		/// Gets the name of the MySQL server to which to connect.
		/// </summary>
#if DESIGN
		[Browsable(true)]
#endif
		public override string DataSource
		{
			get { return settings.Server; }
		}

		/// <include file='docs/MySqlConnection.xml' path='docs/ConnectionTimeout/*'/>
#if DESIGN
		[Browsable(true)]
#endif
		public override int ConnectionTimeout
		{
			get { return settings.ConnectionTimeout; }
		}
		
		/// <include file='docs/MySqlConnection.xml' path='docs/Database/*'/>
#if DESIGN
		[Browsable(true)]
#endif
		public override string Database
		{
			get	{ return settings.Database; }
		}

		/// <summary>
		/// Indicates if this connection should use compression when communicating with the server.
		/// </summary>
#if DESIGN
		[Browsable(false)]
#endif
		public bool UseCompression
		{
			get { return settings.UseCompression; }
		}
		
		/// <include file='docs/MySqlConnection.xml' path='docs/State/*'/>
#if DESIGN
		[Browsable(false)]
#endif
		public override ConnectionState State
		{
			get { return connectionState; }
		}

		/// <include file='docs/MySqlConnection.xml' path='docs/ServerVersion/*'/>
#if DESIGN
		[Browsable(false)]
#endif
		public override string ServerVersion 
		{
			get { return  driver.Version.ToString(); }
		}

		internal Encoding Encoding 
		{
			get 
			{
				if (driver == null)
					return System.Text.Encoding.Default;
				else 
					return driver.Encoding;
			}
		}


		/// <include file='docs/MySqlConnection.xml' path='docs/ConnectionString/*'/>
#if DESIGN
		[Editor("MySql.Data.MySqlClient.Design.ConnectionStringTypeEditor,MySqlClient.Design", typeof(System.Drawing.Design.UITypeEditor))]
		[Browsable(true)]
		[Category("Data")]
		[Description("Information used to connect to a DataSource, such as 'Server=xxx;UserId=yyy;Password=zzz;Database=dbdb'.")]
#endif
		public override string ConnectionString
		{
			get
			{
				// Always return exactly what the user set.
				// Security-sensitive information may be removed.
				return settings.GetConnectionString(!hasBeenOpen);
			}
			set
			{
				if (this.State != ConnectionState.Closed)
					throw new MySqlException("Not allowed to change the 'ConnectionString' property while the connection (state=" + State + ").");

                try
                {
                    MySqlConnectionString newSettings = new MySqlConnectionString(value);
                    settings = newSettings;
                    if (driver != null)
                        driver.Settings = newSettings;
                }
                catch (Exception)
                {
                    throw;
                }
			}
		}

		#endregion

		#region Transactions

		/// <include file='docs/MySqlConnection.xml' path='docs/BeginTransaction/*'/>
		public new MySqlTransaction BeginTransaction()
		{
			return this.BeginTransaction(IsolationLevel.RepeatableRead);
		}

		/// <include file='docs/MySqlConnection.xml' path='docs/BeginTransaction1/*'/>
		public new MySqlTransaction BeginTransaction(IsolationLevel iso)
		{
			//TODO: check note in help
			if (State != ConnectionState.Open)
				throw new InvalidOperationException(Resources.GetString("ConnectionNotOpen"));

			MySqlTransaction t = new MySqlTransaction(this, iso);

			MySqlCommand cmd = new MySqlCommand( "", this);

			cmd.CommandText = "SET SESSION TRANSACTION ISOLATION LEVEL ";
			switch (iso) 
			{
				case IsolationLevel.ReadCommitted:
					cmd.CommandText += "READ COMMITTED"; break;
				case IsolationLevel.ReadUncommitted:
					cmd.CommandText += "READ UNCOMMITTED"; break;
				case IsolationLevel.RepeatableRead:
					cmd.CommandText += "REPEATABLE READ"; break;
				case IsolationLevel.Serializable:
					cmd.CommandText += "SERIALIZABLE"; break;
				case IsolationLevel.Chaos:
					throw new NotSupportedException(Resources.GetString("ChaosNotSupported"));
			}

			cmd.ExecuteNonQuery();

			cmd.CommandText = "BEGIN";
			cmd.ExecuteNonQuery();

			return t;
		}

		#endregion

		/// <include file='docs/MySqlConnection.xml' path='docs/ChangeDatabase/*'/>
		public override void ChangeDatabase(string database)
		{
			if (database == null || database.Trim().Length == 0)
				throw new ArgumentException(
					Resources.GetString("ParameterIsInvalid"), "database");

			if (State != ConnectionState.Open)
				throw new InvalidOperationException(
					Resources.GetString("ConnectionNotOpen"));

			driver.SetDatabase( database );
			settings.Database = database;
		}

		internal void SetState(ConnectionState newConnectionState) 
		{
            if (newConnectionState == this.connectionState) 
                return;
            ConnectionState oldConnectionState = this.connectionState;
            this.connectionState = newConnectionState;
            this.OnStateChange(new StateChangeEventArgs(oldConnectionState, this.connectionState));
		}

		/// <summary>
		/// Ping
		/// </summary>
		/// <returns></returns>
		public bool Ping() 
		{
			return driver.Ping();
		}

		/// <include file='docs/MySqlConnection.xml' path='docs/Open/*'/>
		public override void Open()
		{
			if (State == ConnectionState.Open)
				throw new InvalidOperationException(
					Resources.GetString("ConnectionAlreadyOpen"));

			SetState(ConnectionState.Connecting);

			try 
			{
				if (settings.Pooling) 
				{
                    MySqlPool pool = MySqlPoolManager.GetPool(settings);
                    driver = pool.GetConnection();
                    procedureCache = pool.ProcedureCache;
				}
				else
				{
					driver = Driver.Create(settings);
                    procedureCache = new ProcedureCache(settings.ProcedureCacheSize);
				}
			}
			catch (Exception)
			{
				SetState(ConnectionState.Closed);
				throw;
			}

			// if the user is using old syntax, let them know
			if ( driver.Settings.UseOldSyntax)
				Logger.LogWarning("You are using old syntax that will be removed in future versions");

			SetState(ConnectionState.Open);
			driver.Configure(this);
			if (settings.Database != null && settings.Database != String.Empty)
				ChangeDatabase(settings.Database);

            // setup our schema provider
            if (driver.Version.isAtLeast(5, 0, 0))
                schemaProvider = new ISSchemaProvider(this);
            else
                schemaProvider = new NonISSchemaProvider(this);
            perfMonitor = new PerformanceMonitor(this);

            hasBeenOpen = true;
		}

		/// <include file='docs/MySqlConnection.xml' path='docs/CreateCommand/*'/>
		public new MySqlCommand CreateCommand()
		{
			// Return a new instance of a command object.
			MySqlCommand c = new MySqlCommand();
			c.Connection = this;
			return c;
		}

		#region ICloneable
		/// <summary>
		/// Creates a new MySqlConnection object with the exact same ConnectionString value
		/// </summary>
		/// <returns>A cloned MySqlConnection object</returns>
		object ICloneable.Clone()
		{
			MySqlConnection clone = new MySqlConnection();
			clone.ConnectionString = this.ConnectionString;
			//TODO:  how deep should this go?
			return clone;
		}
		#endregion

		#region IDisposeable

		protected override void Dispose(bool disposing)
		{
			if (disposing && State == ConnectionState.Open)
				Close();
			base.Dispose(disposing);
		}

		#endregion

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return BeginTransaction(isolationLevel);
        }

        protected override DbCommand CreateDbCommand()
        {
            return CreateCommand();
        }

        internal void Abort()
        {
            //TODO: implement me
        }

        /// <include file='docs/MySqlConnection.xml' path='docs/Close/*'/>
        public override void Close()
        {
            //TODO: rollback any pending transaction
            if (State == ConnectionState.Closed) return;

            if (dataReader != null)
                dataReader.Close();

            if (settings.Pooling)
                MySqlPoolManager.ReleaseConnection(driver);
            else
                driver.Close();

            SetState(ConnectionState.Closed);
        }

        #region GetSchema Support

        public override DataTable GetSchema()
        {
            return null;
        }

        public override DataTable GetSchema(string collectionName)
        {
            return schemaProvider.GetSchema(collectionName, null);
        }

        public override DataTable GetSchema(string collectionName, string[] restrictionValues)
        {
            return schemaProvider.GetSchema(collectionName, restrictionValues);
        }

        #endregion

    }

	/// <summary>
	/// Represents the method that will handle the <see cref="MySqlConnection.InfoMessage"/> event of a 
	/// <see cref="MySqlConnection"/>.
	/// </summary>
	public delegate void MySqlInfoMessageEventHandler( object sender, MySqlInfoMessageEventArgs args );

	/// <summary>
	/// Provides data for the InfoMessage event. This class cannot be inherited.
	/// </summary>
	public class MySqlInfoMessageEventArgs : EventArgs
	{
		/// <summary>
		/// 
		/// </summary>
		public MySqlError[]	errors;
	}
}
