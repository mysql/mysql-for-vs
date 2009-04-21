// Copyright � 2004-2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
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
using System.Data;
using System.Data.Common;
#if !CF
using System.Drawing;
using System.Drawing.Design;
using System.Transactions;
#endif
using System.Text;
using IsolationLevel=System.Data.IsolationLevel;
using MySql.Data.Common;
using System.Diagnostics;
using MySql.Data.MySqlClient.Properties;

namespace MySql.Data.MySqlClient
{
    /// <include file='docs/MySqlConnection.xml' path='docs/ClassSummary/*'/>
#if !CF
    [ToolboxBitmap(typeof (MySqlConnection), "MySqlClient.resources.connection.bmp")]
    [DesignerCategory("Code")]
    [ToolboxItem(true)]
#endif
    public sealed class MySqlConnection : DbConnection, ICloneable
    {
        internal ConnectionState connectionState;
        internal Driver driver;
        private MySqlDataReader dataReader;
        private MySqlConnectionStringBuilder settings;
        private UsageAdvisor advisor;
        private bool hasBeenOpen;
        private SchemaProvider schemaProvider;
        private ProcedureCache procedureCache;
#if !CF
        private PerformanceMonitor perfMonitor;
#endif
        private bool isExecutingBuggyQuery;
        private string database;

        /// <include file='docs/MySqlConnection.xml' path='docs/InfoMessage/*'/>
        public event MySqlInfoMessageEventHandler InfoMessage;

        private static Cache<string, MySqlConnectionStringBuilder> connectionStringCache =
            new Cache<string, MySqlConnectionStringBuilder>(0, 25);

        /// <include file='docs/MySqlConnection.xml' path='docs/DefaultCtor/*'/>
        public MySqlConnection()
        {
            //TODO: add event data to StateChange docs
            settings = new MySqlConnectionStringBuilder();
            advisor = new UsageAdvisor(this);
            database = String.Empty;
        }

        /// <include file='docs/MySqlConnection.xml' path='docs/Ctor1/*'/>
        public MySqlConnection(string connectionString)
            : this()
        {
            ConnectionString = connectionString;
        }

        #region Interal Methods & Properties

#if !CF
        internal PerformanceMonitor PerfMonitor
        {
            get { return perfMonitor; }
        }

#endif

        internal ProcedureCache ProcedureCache
        {
            get { return procedureCache; }
        }

        internal MySqlConnectionStringBuilder Settings
        {
            get { return settings; }
        }

        internal MySqlDataReader Reader
        {
            get { return dataReader; }
            set { dataReader = value; }
        }

        internal void OnInfoMessage(MySqlInfoMessageEventArgs args)
        {
            if (InfoMessage != null)
            {
                InfoMessage(this, args);
            }
        }

        internal bool IsExecutingBuggyQuery
        {
            get { return isExecutingBuggyQuery; }
            set { isExecutingBuggyQuery = value; }
        }

        internal bool SoftClosed
        {
            get 
            {
#if !CF
                return (State == ConnectionState.Closed) && 
                    driver != null && 
                    driver.CurrentTransaction != null;
#else
                return false;            
#endif
            }
        }

        #endregion

        #region Properties

#if !CF
        [Browsable(false)]
#endif
            internal UsageAdvisor UsageAdvisor
        {
            get { return advisor; }
        }

        /// <summary>
        /// Returns the id of the server thread this connection is executing on
        /// </summary>
#if !CF
        [Browsable(false)]
#endif
            public int ServerThread
        {
            get { return driver.ThreadID; }
        }

        /// <summary>
        /// Gets the name of the MySQL server to which to connect.
        /// </summary>
#if !CF
        [Browsable(true)]
#endif
            public override string DataSource
        {
            get { return settings.Server; }
        }

        /// <include file='docs/MySqlConnection.xml' path='docs/ConnectionTimeout/*'/>
#if !CF
        [Browsable(true)]
#endif
            public override int ConnectionTimeout
        {
            get { return (int) settings.ConnectionTimeout; }
        }

        /// <include file='docs/MySqlConnection.xml' path='docs/Database/*'/>
#if !CF
        [Browsable(true)]
#endif
            public override string Database
        {
            get { return database; }
        }

        /// <summary>
        /// Indicates if this connection should use compression when communicating with the server.
        /// </summary>
#if !CF
        [Browsable(false)]
#endif
            public bool UseCompression
        {
            get { return settings.UseCompression; }
        }

        /// <include file='docs/MySqlConnection.xml' path='docs/State/*'/>
#if !CF
        [Browsable(false)]
#endif
            public override ConnectionState State
        {
            get { return connectionState; }
        }

        /// <include file='docs/MySqlConnection.xml' path='docs/ServerVersion/*'/>
#if !CF
        [Browsable(false)]
#endif
            public override string ServerVersion
        {
            get { return driver.Version.ToString(); }
        }

        internal Encoding Encoding
        {
            get
            {
                if (driver == null)
                    return Encoding.Default;
                else
                    return driver.Encoding;
            }
        }


        /// <include file='docs/MySqlConnection.xml' path='docs/ConnectionString/*'/>
#if !CF
        [Editor("MySql.Data.MySqlClient.Design.ConnectionStringTypeEditor,MySqlClient.Design", typeof (UITypeEditor))]
        [Browsable(true)]
        [Category("Data")]
        [Description(
            "Information used to connect to a DataSource, such as 'Server=xxx;UserId=yyy;Password=zzz;Database=dbdb'.")]
#endif
            public override string ConnectionString
        {
            get
            {
                // Always return exactly what the user set.
                // Security-sensitive information may be removed.
                return settings.GetConnectionString(!hasBeenOpen || settings.PersistSecurityInfo);
            }
            set
            {
                if (State != ConnectionState.Closed)
                    throw new MySqlException(
                        "Not allowed to change the 'ConnectionString' property while the connection (state=" + State +
                        ").");

                MySqlConnectionStringBuilder newSettings;
                lock (connectionStringCache)
                {
                    if (value == null)
                        newSettings = new MySqlConnectionStringBuilder();
                    else
                    {
                        newSettings = (MySqlConnectionStringBuilder)connectionStringCache[value];
                        if (null == newSettings)
                        {
                            newSettings = new MySqlConnectionStringBuilder(value);
                            connectionStringCache.Add(value, newSettings);
                        }
                    }
                }

                settings = newSettings;

                if (settings.Database != null && settings.Database.Length > 0)
                    this.database = settings.Database;

                if (driver != null)
                    driver.Settings = newSettings;
            }
        }

#if !CF

        protected override DbProviderFactory DbProviderFactory
        {
            get
            {
                return MySqlClientFactory.Instance;
            }
        }
#endif

        #endregion

        #region Transactions

#if !MONO && !CF
        /// <summary>
        /// Enlists in the specified transaction. 
        /// </summary>
        /// <param name="transaction">
        /// A reference to an existing <see cref="System.Transactions.Transaction"/> in which to enlist.
        /// </param>
        public override void EnlistTransaction(Transaction transaction)
        {
            // enlisting in the null transaction is a noop
            if (transaction == null)
                return;

            // guard against trying to enlist in more than one transaction
            if (driver.CurrentTransaction != null)
            {
                if (driver.CurrentTransaction.BaseTransaction == transaction)
                    return;

                throw new MySqlException("Already enlisted");
            }

            // now see if we need to swap out drivers.  We would need to do this since
            // we have to make sure all ops for a given transaction are done on the
            // same physical connection.
            Driver existingDriver = DriverTransactionManager.GetDriverInTransaction(transaction);
            if (existingDriver != null)
            {
                // we can't allow more than one driver to contribute to the same connection
                if (existingDriver.IsInActiveUse)
                    throw new NotSupportedException(Resources.MultipleConnectionsInTransactionNotSupported);

                // there is an existing driver and it's not being currently used.
                // now we need to see if it is using the same connection string
                string text1 = existingDriver.Settings.GetConnectionString(true);
                string text2 = Settings.GetConnectionString(true);
                if (String.Compare(text1, text2, true) != 0)
                    throw new NotSupportedException(Resources.MultipleConnectionsInTransactionNotSupported);

                // close existing driver
                // set this new driver as our existing driver
                CloseFully();
                driver = existingDriver;
            }

            if (driver.CurrentTransaction == null)
            {
                MySqlPromotableTransaction t = new MySqlPromotableTransaction(this, transaction);
                if (!transaction.EnlistPromotableSinglePhase(t))
                    throw new NotSupportedException(Resources.DistributedTxnNotSupported);

                driver.CurrentTransaction = t;
                DriverTransactionManager.SetDriverInTransaction(driver);
                driver.IsInActiveUse = true;
            }
        }
#endif

        /// <include file='docs/MySqlConnection.xml' path='docs/BeginTransaction/*'/>
        public new MySqlTransaction BeginTransaction()
        {
            return BeginTransaction(IsolationLevel.RepeatableRead);
        }

        /// <include file='docs/MySqlConnection.xml' path='docs/BeginTransaction1/*'/>
        public new MySqlTransaction BeginTransaction(IsolationLevel iso)
        {
            //TODO: check note in help
            if (State != ConnectionState.Open)
                throw new InvalidOperationException(Resources.ConnectionNotOpen);

            // First check to see if we are in a current transaction
            if ((driver.ServerStatus & ServerStatusFlags.InTransaction) != 0)
                throw new InvalidOperationException(Resources.NoNestedTransactions);

            MySqlTransaction t = new MySqlTransaction(this, iso);

            MySqlCommand cmd = new MySqlCommand("", this);

            cmd.CommandText = "SET SESSION TRANSACTION ISOLATION LEVEL ";
            switch (iso)
            {
                case IsolationLevel.ReadCommitted:
                    cmd.CommandText += "READ COMMITTED";
                    break;
                case IsolationLevel.ReadUncommitted:
                    cmd.CommandText += "READ UNCOMMITTED";
                    break;
                case IsolationLevel.RepeatableRead:
                    cmd.CommandText += "REPEATABLE READ";
                    break;
                case IsolationLevel.Serializable:
                    cmd.CommandText += "SERIALIZABLE";
                    break;
                case IsolationLevel.Chaos:
                    throw new NotSupportedException(Resources.ChaosNotSupported);
            }

            cmd.ExecuteNonQuery();

            cmd.CommandText = "BEGIN";
            cmd.ExecuteNonQuery();

            return t;
        }

        #endregion

        /// <include file='docs/MySqlConnection.xml' path='docs/ChangeDatabase/*'/>
        public override void ChangeDatabase(string databaseName)
        {
            if (databaseName == null || databaseName.Trim().Length == 0)
                throw new ArgumentException(Resources.ParameterIsInvalid, "databaseName");

            if (State != ConnectionState.Open)
                throw new InvalidOperationException(Resources.ConnectionNotOpen);

            driver.SetDatabase(databaseName);
            this.database = databaseName;
        }

        internal void SetState(ConnectionState newConnectionState, bool broadcast)
        {
            if (newConnectionState == connectionState && !broadcast)
                return;
            ConnectionState oldConnectionState = connectionState;
            connectionState = newConnectionState;
			if (broadcast)
				OnStateChange(new StateChangeEventArgs(oldConnectionState, connectionState));
        }

        /// <summary>
        /// Ping
        /// </summary>
        /// <returns></returns>
        public bool Ping()
        {
            if (driver != null && driver.Ping())
                return true;
            driver = null;
            SetState(ConnectionState.Closed, true);
            return false;
        }

        /// <include file='docs/MySqlConnection.xml' path='docs/Open/*'/>
        public override void Open()
        {
            if (State == ConnectionState.Open)
                throw new InvalidOperationException(Resources.ConnectionAlreadyOpen);

            SetState(ConnectionState.Connecting, true);

#if !CF
                // if we are auto enlisting in a current transaction, then we will be
                // treating the connection as pooled
                if (settings.AutoEnlist && Transaction.Current != null)
                {
                    driver = DriverTransactionManager.GetDriverInTransaction(Transaction.Current);
                    if (driver != null &&
                        (driver.IsInActiveUse ||
                        !driver.Settings.EquivalentTo(this.Settings)))
                        throw new NotSupportedException(Resources.MultipleConnectionsInTransactionNotSupported);
                }
#endif

            try
            {
                if (settings.Pooling)
                {
                    MySqlPool pool = MySqlPoolManager.GetPool(settings);
                    if (driver == null)
                        driver = pool.GetConnection();
                    procedureCache = pool.ProcedureCache;
                }
                else
                {
                    if (driver == null)
                        driver = Driver.Create(settings);
                    procedureCache = new ProcedureCache((int) settings.ProcedureCacheSize);
                }
            }
            catch (Exception)
            {
                SetState(ConnectionState.Closed, true);
                throw;
            }

            // if the user is using old syntax, let them know
            if (driver.Settings.UseOldSyntax)
                Logger.LogWarning("You are using old syntax that will be removed in future versions");

            SetState(ConnectionState.Open, false);
            driver.Configure(this);
            if (settings.Database != null && settings.Database != String.Empty)
                ChangeDatabase(settings.Database);

            // setup our schema provider
            if (driver.Version.isAtLeast(5, 0, 0))
                schemaProvider = new ISSchemaProvider(this);
            else
                schemaProvider = new SchemaProvider(this);
#if !CF
            perfMonitor = new PerformanceMonitor(this);
#endif

            // if we are opening up inside a current transaction, then autoenlist
            // TODO: control this with a connection string option
#if !MONO && !CF
            if (Transaction.Current != null && settings.AutoEnlist)
                EnlistTransaction(Transaction.Current);
#endif

            hasBeenOpen = true;
			SetState(ConnectionState.Open, true);
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
            string connectionString = settings.GetConnectionString(true);
            if (connectionString != null)
                clone.ConnectionString = connectionString;
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
            if (isolationLevel == IsolationLevel.Unspecified)
                return BeginTransaction();
            return BeginTransaction(isolationLevel);
        }

        protected override DbCommand CreateDbCommand()
        {
            return CreateCommand();
        }

        internal void Abort()
        {
            try
            {
                if (settings.Pooling)
                    MySqlPoolManager.ReleaseConnection(driver);
                else
                    driver.Close();
            }
            catch (Exception)
            {
            }
            SetState(ConnectionState.Closed, true);
        }

        internal void CloseFully()
        {
            if (settings.Pooling && driver.IsOpen)
            {
                // if we are in a transaction, roll it back
                if ((driver.ServerStatus & ServerStatusFlags.InTransaction) != 0)
                {
                    MySqlTransaction t = new MySqlTransaction(this, IsolationLevel.Unspecified);
                    t.Rollback();
                }

                MySqlPoolManager.ReleaseConnection(driver);
            }
            else
                driver.Close();
            driver = null;
        }

        /// <include file='docs/MySqlConnection.xml' path='docs/Close/*'/>
        public override void Close()
        {
            if (State == ConnectionState.Closed) return;

            if (dataReader != null)
                dataReader.Close();

			// if the reader was opened with CloseConnection then driver
			// will be null on the second time through
			if (driver != null)
			{
#if !CF
				if (driver.CurrentTransaction == null)
#endif	
			        CloseFully();
#if !CF
				else
					driver.IsInActiveUse = false;
#endif
			}

            SetState(ConnectionState.Closed, true);
        }

		internal string CurrentDatabase()
		{
			if (Database != null && Database.Length > 0)
				return Database;
			MySqlCommand cmd = new MySqlCommand("SELECT database()", this);
			return cmd.ExecuteScalar().ToString();
		}

        #region GetSchema Support

        /// <summary>
        /// Returns schema information for the data source of this <see cref="DbConnection"/>. 
        /// </summary>
        /// <returns>A <see cref="DataTable"/> that contains schema information. </returns>
        public override DataTable GetSchema()
        {
            return GetSchema(null);
        }

        /// <summary>
        /// Returns schema information for the data source of this 
        /// <see cref="DbConnection"/> using the specified string for the schema name. 
        /// </summary>
        /// <param name="collectionName">Specifies the name of the schema to return. </param>
        /// <returns>A <see cref="DataTable"/> that contains schema information. </returns>
        public override DataTable GetSchema(string collectionName)
        {
            if (collectionName == null)
                collectionName = SchemaProvider.MetaCollection;

            return GetSchema(collectionName, null);
        }

        /// <summary>
        /// Returns schema information for the data source of this <see cref="DbConnection"/>
        /// using the specified string for the schema name and the specified string array 
        /// for the restriction values. 
        /// </summary>
        /// <param name="collectionName">Specifies the name of the schema to return.</param>
        /// <param name="restrictionValues">Specifies a set of restriction values for the requested schema.</param>
        /// <returns>A <see cref="DataTable"/> that contains schema information.</returns>
        public override DataTable GetSchema(string collectionName, string[] restrictionValues)
        {
            if (collectionName == null)
                collectionName = SchemaProvider.MetaCollection;

            string[] restrictions = schemaProvider.CleanRestrictions(restrictionValues);
            DataTable dt = schemaProvider.GetSchema(collectionName, restrictions);
            return dt;
        }

        #endregion

        #region Pool Routines

        /// <include file='docs/MySqlConnection.xml' path='docs/ClearPool/*'/>
        public static void ClearPool(MySqlConnection connection)
        {
            MySqlPoolManager.ClearPool(connection.Settings);
        }

        /// <include file='docs/MySqlConnection.xml' path='docs/ClearAllPools/*'/>
        public static void ClearAllPools()
        {
            MySqlPoolManager.ClearAllPools();
        }

        #endregion
    }

    /// <summary>
    /// Represents the method that will handle the <see cref="MySqlConnection.InfoMessage"/> event of a 
    /// <see cref="MySqlConnection"/>.
    /// </summary>
    public delegate void MySqlInfoMessageEventHandler(object sender, MySqlInfoMessageEventArgs args);

    /// <summary>
    /// Provides data for the InfoMessage event. This class cannot be inherited.
    /// </summary>
    public class MySqlInfoMessageEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public MySqlError[] errors;
    }
}
