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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient.Properties;

namespace MySql.Data.MySqlClient
{
    /// <include file='docs/MySqlConnectionStringBuilder.xml' path='docs/Class/*'/>
    public sealed class MySqlConnectionStringBuilder : DbConnectionStringBuilder
    {
        private static Dictionary<Keyword, object> defaultValues = new Dictionary<Keyword, object>();

        string userId, password, server;
        string database, sharedMemName, pipeName, charSet;
        readonly string originalConnectionString;
        readonly StringBuilder persistConnString;
        uint port, connectionTimeout, minPoolSize, maxPoolSize;
        uint procCacheSize, connectionLifetime;
        MySqlConnectionProtocol protocol;
        MySqlDriverType driverType;
        bool compress, connectionReset, allowBatch, logging;
        bool oldSyntax, persistSI, usePerfMon, pooling;
        bool allowZeroDatetime, convertZeroDatetime;
        bool useUsageAdvisor, useSSL;
        bool ignorePrepare, useProcedureBodies;
        bool autoEnlist, respectBinaryFlags, treatBlobsAsUTF8;
        string blobAsUtf8IncludePattern, blobAsUtf8ExcludePattern;
        Regex blobAsUtf8ExcludeRegex, blobAsUtf8IncludeRegex;
        uint defaultCommandTimeout;
        bool treatTinyAsBoolean;
        bool allowUserVariables;
        bool clearing;
        bool interactiveSession;
        bool functionsReturnString;
        bool useAffectedRows;

        static MySqlConnectionStringBuilder()
        {
            defaultValues.Add(Keyword.ConnectionTimeout, 15);
            defaultValues.Add(Keyword.Pooling, true);
            defaultValues.Add(Keyword.Port, 3306);
            defaultValues.Add(Keyword.Server, "");
            defaultValues.Add(Keyword.PersistSecurityInfo, false);
            defaultValues.Add(Keyword.ConnectionLifetime, 0);
            defaultValues.Add(Keyword.ConnectionReset, false);
            defaultValues.Add(Keyword.MinimumPoolSize, 0);
            defaultValues.Add(Keyword.MaximumPoolSize, 100);
            defaultValues.Add(Keyword.UserID, "");
            defaultValues.Add(Keyword.Password, "");
            defaultValues.Add(Keyword.UseUsageAdvisor, false);
            defaultValues.Add(Keyword.CharacterSet, "");
            defaultValues.Add(Keyword.Compress, false);
            defaultValues.Add(Keyword.PipeName, "MYSQL");
            defaultValues.Add(Keyword.Logging, false);
            defaultValues.Add(Keyword.OldSyntax, false);
            defaultValues.Add(Keyword.SharedMemoryName, "MYSQL");
            defaultValues.Add(Keyword.AllowBatch, true);
            defaultValues.Add(Keyword.ConvertZeroDatetime, false);
            defaultValues.Add(Keyword.Database, "");
            defaultValues.Add(Keyword.DriverType, MySqlDriverType.Native);
            defaultValues.Add(Keyword.Protocol, MySqlConnectionProtocol.Sockets);
            defaultValues.Add(Keyword.AllowZeroDatetime, false);
            defaultValues.Add(Keyword.UsePerformanceMonitor, false);
            defaultValues.Add(Keyword.ProcedureCacheSize, 25);
            defaultValues.Add(Keyword.UseSSL, false);
            defaultValues.Add(Keyword.IgnorePrepare, true);
            defaultValues.Add(Keyword.UseProcedureBodies, true);
            defaultValues.Add(Keyword.AutoEnlist, true);
            defaultValues.Add(Keyword.RespectBinaryFlags, true);
            defaultValues.Add(Keyword.BlobAsUTF8ExcludePattern, null);
            defaultValues.Add(Keyword.BlobAsUTF8IncludePattern, null);
            defaultValues.Add(Keyword.TreatBlobsAsUTF8, false);
            defaultValues.Add(Keyword.DefaultCommandTimeout, 30);
            defaultValues.Add(Keyword.TreatTinyAsBoolean, true);
            defaultValues.Add(Keyword.AllowUserVariables, false);
            defaultValues.Add(Keyword.InteractiveSession, false);
            defaultValues.Add(Keyword.FunctionsReturnString, false);
            defaultValues.Add(Keyword.UseAffectedRows, false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlConnectionStringBuilder"/> class. 
        /// </summary>
        public MySqlConnectionStringBuilder()
        {
            persistConnString = new StringBuilder();
            Clear();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlConnectionStringBuilder"/> class. 
        /// The provided connection string provides the data for the instance's internal 
        /// connection information. 
        /// </summary>
        /// <param name="connectionString">The basis for the object's internal connection 
        /// information. Parsed into name/value pairs. Invalid key names raise 
        /// <see cref="KeyNotFoundException"/>.
        /// </param>
        public MySqlConnectionStringBuilder(string connectionString)
            : this()
        {
            originalConnectionString = connectionString;
            persistConnString = new StringBuilder();
            ConnectionString = connectionString;
        }

        #region Server Properties

        /// <summary>
        /// Gets or sets the name of the server.
        /// </summary>
        /// <value>The server.</value>
#if !CF && !MONO
        [Category("Connection")]
        [Description("Server to connect to")]
        [RefreshProperties(RefreshProperties.All)]
#endif
            public string Server
        {
            get { return server; }
            set
            {
                SetValue("Server", value); 
                server = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the database the connection should 
        /// initially connect to.
        /// </summary>
#if !CF && !MONO
        [Category("Connection")]
        [Description("Database to use initially")]
        [RefreshProperties(RefreshProperties.All)]
#endif
            public string Database
        {
            get { return database; }
            set
            {
                SetValue("Database", value); 
                database = value;
            }
        }

        /// <summary>
        /// Gets or sets the protocol that should be used for communicating
        /// with MySQL.
        /// </summary>
#if !CF && !MONO
        [Category("Connection")]
        [DisplayName("Connection Protocol")]
        [Description("Protocol to use for connection to MySQL")]
        [DefaultValue(MySqlConnectionProtocol.Sockets)]
        [RefreshProperties(RefreshProperties.All)]
#endif
            public MySqlConnectionProtocol ConnectionProtocol
        {
            get { return protocol; }
            set
            {
                SetValue("Connection Protocol", value); 
                protocol = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the named pipe that should be used
        /// for communicating with MySQL.
        /// </summary>
#if !CF && !MONO
        [Category("Connection")]
        [DisplayName("Pipe Name")]
        [Description("Name of pipe to use when connecting with named pipes (Win32 only)")]
        [DefaultValue("MYSQL")]
        [RefreshProperties(RefreshProperties.All)]
#endif
            public string PipeName
        {
            get { return pipeName; }
            set
            {
                SetValue("Pipe Name", value); 
                pipeName = value;
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether this connection
        /// should use compression.
        /// </summary>
#if !CF && !MONO
        [Category("Connection")]
        [DisplayName("Use Compression")]
        [Description("Should the connection ues compression")]
        [DefaultValue(false)]
        [RefreshProperties(RefreshProperties.All)]
#endif
            public bool UseCompression
        {
            get { return compress; }
            set
            {
                SetValue("Use Compression", value); 
                compress = value;
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether this connection will allow
        /// commands to send multiple SQL statements in one execution.
        /// </summary>
#if !CF && !MONO
        [Category("Connection")]
        [DisplayName("Allow Batch")]
        [Description("Allows execution of multiple SQL commands in a single statement")]
        [DefaultValue(true)]
        [RefreshProperties(RefreshProperties.All)]
#endif
            public bool AllowBatch
        {
            get { return allowBatch; }
            set
            {
                SetValue("Allow Batch", value); 
                allowBatch = value;
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether logging is enabled.
        /// </summary>
#if !CF && !MONO
        [Category("Connection")]
        [Description("Enables output of diagnostic messages")]
        [DefaultValue(false)]
        [RefreshProperties(RefreshProperties.All)]
#endif
            public bool Logging
        {
            get { return logging; }
            set
            {
                SetValue("Logging", value); 
                logging = value;
            }
        }

        /// <summary>
        /// Gets or sets the base name of the shared memory objects used to 
        /// communicate with MySQL when the shared memory protocol is being used.
        /// </summary>
#if !CF && !MONO
        [Category("Connection")]
        [DisplayName("Shared Memory Name")]
        [Description("Name of the shared memory object to use")]
        [DefaultValue("MYSQL")]
        [RefreshProperties(RefreshProperties.All)]
#endif
            public string SharedMemoryName
        {
            get { return sharedMemName; }
            set
            {
                SetValue("Shared Memory Name", value); 
                sharedMemName = value;
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether this connection uses
        /// the old style (@) parameter markers or the new (?) style.
        /// </summary>
#if !CF && !MONO
        [Category("Connection")]
        [DisplayName("Use Old Syntax")]
        [Description("Allows the use of old style @ syntax for parameters")]
        [DefaultValue(false)]
        [RefreshProperties(RefreshProperties.All)]
        [Obsolete("Use Old Syntax is no longer needed.  See documentation")]
#endif
            public bool UseOldSyntax
        {
            get { return oldSyntax; }
            set
            {
                SetValue("Use Old Syntax", value); 
                oldSyntax = value;
            }
        }

        /// <summary>
        /// Gets or sets the driver type that should be used for this connection.
        /// </summary>
        /// <remarks>
        /// There is only one valid value for this setting currently.
        /// </remarks>
#if !CF && !MONO
        [Category("Connection")]
        [DisplayName("Driver Type")]
        [Description("Specifies the type of driver to use for this connection")]
        [DefaultValue(MySqlDriverType.Native)]
        [RefreshProperties(RefreshProperties.All)]
        [Browsable(false)]
#endif
            public MySqlDriverType DriverType
        {
            get { return driverType; }
            set 
            { 
                SetValue("Driver Type", value); 
                driverType = value; 
            }
        }

        /// <summary>
        /// Gets or sets the port number that is used when the socket
        /// protocol is being used.
        /// </summary>
#if !CF && !MONO
        [Category("Connection")]
        [Description("Port to use for TCP/IP connections")]
        [DefaultValue(3306)]
        [RefreshProperties(RefreshProperties.All)]
#endif
            public uint Port
        {
            get { return port; }
            set 
            { 
                SetValue("Port", value); 
                port = value; 
            }
        }

        /// <summary>
        /// Gets or sets the connection timeout.
        /// </summary>
#if !CF && !MONO
        [Category("Connection")]
        [DisplayName("Connect Timeout")]
        [Description("The length of time (in seconds) to wait for a connection " +
                     "to the server before terminating the attempt and generating an error.")]
        [DefaultValue(15)]
        [RefreshProperties(RefreshProperties.All)]
#endif
            public uint ConnectionTimeout
        {
            get { return connectionTimeout; }
            set 
            {
                SetValue("Connect Timeout", value); 
                connectionTimeout = value; 
            }
        }

        /// <summary>
        /// Gets or sets the default command timeout.
        /// </summary>
#if !CF && !MONO
        [Category("Connection")]
        [DisplayName("Default Command Timeout")]
        [Description(@"The default timeout that MySqlCommand objects will use
                     unless changed.")]
        [DefaultValue(30)]
        [RefreshProperties(RefreshProperties.All)]
#endif
        public uint DefaultCommandTimeout
        {
            get { return defaultCommandTimeout; }
            set
            {
                SetValue("Default Command Timeout", value);
                defaultCommandTimeout = value;
            }
        }


        #endregion

        #region Authentication Properties

        /// <summary>
        /// Gets or sets the user id that should be used to connect with.
        /// </summary>
#if !CF && !MONO
        [Category("Security")]
        [DisplayName("User Id")]
        [Description("Indicates the user ID to be used when connecting to the data source.")]
        [RefreshProperties(RefreshProperties.All)]
#endif
            public string UserID
        {
            get { return userId; }
            set
            {
                SetValue("User Id", value); 
                userId = value;
            }
        }

        /// <summary>
        /// Gets or sets the password that should be used to connect with.
        /// </summary>
#if !CF && !MONO
        [Category("Security")]
        [Description("Indicates the password to be used when connecting to the data source.")]
        [PasswordPropertyText(true)]
        [RefreshProperties(RefreshProperties.All)]
#endif
            public string Password
        {
            get { return password; }
            set
            {
                SetValue("Password", value); 
                password = value;
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates if the password should be persisted
        /// in the connection string.
        /// </summary>
#if !CF && !MONO
        [Category("Security")]
        [DisplayName("Persist Security Info")]
        [Description("When false, security-sensitive information, such as the password, " +
                     "is not returned as part of the connection if the connection is open or " +
                     "has ever been in an open state.")]
        [RefreshProperties(RefreshProperties.All)]
#endif
            public bool PersistSecurityInfo
        {
            get { return persistSI; }
            set
            {
                SetValue("Persist Security Info", value); 
                persistSI = value;
            }
        }

#if !CF && !MONO
        [Category("Authentication")]
        [Description("Should the connection use SSL.  This currently has no effect.")]
        [DefaultValue(false)]
        [RefreshProperties(RefreshProperties.All)]
#endif
            internal bool UseSSL
        {
            get { return useSSL; }
            set
            {
                SetValue("UseSSL", value); 
                useSSL = value;
            }
        }

        #endregion

        #region Other Properties

        /// <summary>
        /// Gets or sets a boolean value that indicates if zero date time values are supported.
        /// </summary>
#if !CF && !MONO
        [Category("Advanced")]
        [DisplayName("Allow Zero Datetime")]
        [Description("Should zero datetimes be supported")]
        [DefaultValue(false)]
        [RefreshProperties(RefreshProperties.All)]
#endif
            public bool AllowZeroDateTime
        {
            get { return allowZeroDatetime; }
            set
            {
                SetValue("Allow Zero Datetime", value); 
                allowZeroDatetime = value;
            }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating if zero datetime values should be 
        /// converted to DateTime.MinValue.
        /// </summary>
#if !CF && !MONO
        [Category("Advanced")]
        [DisplayName("Convert Zero Datetime")]
        [Description("Should illegal datetime values be converted to DateTime.MinValue")]
        [DefaultValue(false)]
        [RefreshProperties(RefreshProperties.All)]
#endif
            public bool ConvertZeroDateTime
        {
            get { return convertZeroDatetime; }
            set
            {
                SetValue("Convert Zero Datetime", value); 
                convertZeroDatetime = value;
            }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating if the Usage Advisor should be enabled.
        /// </summary>
#if !CF && !MONO
        [Category("Advanced")]
        [DisplayName("Use Usage Advisor")]
        [Description("Logs inefficient database operations")]
        [DefaultValue(false)]
        [RefreshProperties(RefreshProperties.All)]
#endif
            public bool UseUsageAdvisor
        {
            get { return useUsageAdvisor; }
            set
            {
                SetValue("Use Usage Advisor", value); 
                useUsageAdvisor = value;
            }
        }

        /// <summary>
        /// Gets or sets the size of the stored procedure cache.
        /// </summary>
#if !CF && !MONO
        [Category("Advanced")]
        [DisplayName("Procedure Cache Size")]
        [Description("Indicates how many stored procedures can be cached at one time. " +
                     "A value of 0 effectively disables the procedure cache.")]
        [DefaultValue(25)]
        [RefreshProperties(RefreshProperties.All)]
#endif
            public uint ProcedureCacheSize
        {
            get { return procCacheSize; }
            set
            {
                SetValue("Procedure Cache Size", value); 
                procCacheSize = value;
            }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating if the permon hooks should be enabled.
        /// </summary>
#if !CF && !MONO
        [Category("Advanced")]
        [DisplayName("Use Performance Monitor")]
        [Description("Indicates that performance counters should be updated during execution.")]
        [DefaultValue(false)]
        [RefreshProperties(RefreshProperties.All)]
#endif
            public bool UsePerformanceMonitor
        {
            get { return usePerfMon; }
            set
            {
                SetValue("Use Performance Monitor", value); 
                usePerfMon = value;
            }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating if calls to Prepare() should be ignored.
        /// </summary>
#if !CF && !MONO
        [Category("Advanced")]
        [DisplayName("Ignore Prepare")]
        [Description("Instructs the provider to ignore any attempts to prepare a command.")]
        [DefaultValue(true)]
        [RefreshProperties(RefreshProperties.All)]
#endif
            public bool IgnorePrepare
        {
            get { return ignorePrepare; }
            set
            {
                SetValue("Ignore Prepare", value); 
                ignorePrepare = value;
            }
        }

#if !CF && !MONO
        [Category("Advanced")]
        [DisplayName("Use Procedure Bodies")]
        [Description("Indicates if stored procedure bodies will be available for parameter detection.")]
        [DefaultValue(true)]
        [RefreshProperties(RefreshProperties.All)]
#endif
        public bool UseProcedureBodies
        {
            get { return useProcedureBodies; }
            set
            {
                SetValue("Use Procedure Bodies", value); 
                useProcedureBodies = value;
            }
        }

#if !CF && !MONO
        [Category("Advanced")]
        [DisplayName("Auto Enlist")]
        [Description("Should the connetion automatically enlist in the active connection, if there are any.")]
        [DefaultValue(true)]
        [RefreshProperties(RefreshProperties.All)]
#endif
        public bool AutoEnlist
        {
            get { return autoEnlist; }
            set
            {
                SetValue("Auto Enlist", value);
                autoEnlist = value;
            }
        }

#if !CF && !MONO
        [Category("Advanced")]
        [DisplayName("Respect Binary Flags")]
        [Description("Should binary flags on column metadata be respected.")]
        [DefaultValue(true)]
        [RefreshProperties(RefreshProperties.All)]
#endif
        public bool RespectBinaryFlags
        {
            get { return respectBinaryFlags; }
            set
            {
                SetValue("Respect Binary Flags", value);
                respectBinaryFlags = value;
            }
        }

#if !CF && !MONO
        [Category("Advanced")]
        [DisplayName("Treat Tiny As Boolean")]
        [Description("Should the provider treat TINYINT(1) columns as boolean.")]
        [DefaultValue(true)]
        [RefreshProperties(RefreshProperties.All)]
#endif
        public bool TreatTinyAsBoolean
        {
            get { return treatTinyAsBoolean; }
            set
            {
                SetValue("Treat Tiny As Boolean", value);
                treatTinyAsBoolean = value;
            }
        }

#if !CF && !MONO
        [Category("Advanced")]
        [DisplayName("Allow User Variables")]
        [Description("Should the provider expect user variables to appear in the SQL.")]
        [DefaultValue(false)]
        [RefreshProperties(RefreshProperties.All)]
#endif
        public bool AllowUserVariables
        {
            get { return allowUserVariables; }
            set
            {
                SetValue("Allow User Variables", value);
                allowUserVariables = value;
            }
        }

#if !CF && !MONO
        [Category("Advanced")]
        [DisplayName("Interactive Session")]
        [Description("Should this session be considered interactive?")]
        [DefaultValue(false)]
        [RefreshProperties(RefreshProperties.All)]
#endif
        public bool InteractiveSession
        {
            get { return interactiveSession; }
            set
            {
                SetValue("Interactive Session", value);
                interactiveSession = value;
            }
        }

#if !CF && !MONO
        [Category("Advanced")]
        [DisplayName("Functions Return String")]
        [Description("Should all server functions be treated as returning string?")]
        [DefaultValue(false)]
#endif
        public bool FunctionsReturnString
        {
            get { return functionsReturnString; }
            set
            {
                SetValue("Functions Return String", value);
                functionsReturnString = value;
            }
        }

#if !CF && !MONO
        [Category("Advanced")]
        [DisplayName("Use Affected Rows")]
        [Description("Should the returned affected row count reflect affected rows instead of found rows?")]
        [DefaultValue(false)]
#endif
        public bool UseAffectedRows
        {
            get { return useAffectedRows; }
            set
            {
                SetValue("Use Affected Rows", value);
                useAffectedRows = value;
            }
        }

        #endregion

        #region Pooling Properties

        /// <summary>
        /// Gets or sets the lifetime of a pooled connection.
        /// </summary>
#if !CF && !MONO
        [Category("Pooling")]
        [DisplayName("Connection Lifetime")]
        [Description("The minimum amount of time (in seconds) for this connection to " +
                     "live in the pool before being destroyed.")]
        [DefaultValue(0)]
        [RefreshProperties(RefreshProperties.All)]
#endif
        public uint ConnectionLifeTime
        {
            get { return connectionLifetime; }
            set
            {
                SetValue("Connection Lifetime", value); 
                connectionLifetime = value;
            }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating if connection pooling is enabled.
        /// </summary>
#if !CF && !MONO
        [Category("Pooling")]
        [Description("When true, the connection object is drawn from the appropriate " +
                     "pool, or if necessary, is created and added to the appropriate pool.")]
        [DefaultValue(true)]
        [RefreshProperties(RefreshProperties.All)]
#endif
            public bool Pooling
        {
            get { return pooling; }
            set
            {
                SetValue("Pooling", value); 
                pooling = value;
            }
        }

        /// <summary>
        /// Gets the minimum connection pool size.
        /// </summary>
#if !CF && !MONO
        [Category("Pooling")]
        [DisplayName("Minimum Pool Size")]
        [Description("The minimum number of connections allowed in the pool.")]
        [DefaultValue(0)]
        [RefreshProperties(RefreshProperties.All)]
#endif
            public uint MinimumPoolSize
        {
            get { return minPoolSize; }
            set
            {
                SetValue("Minimum Pool Size", value); 
                minPoolSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum connection pool setting.
        /// </summary>
#if !CF && !MONO
        [Category("Pooling")]
        [DisplayName("Maximum Pool Size")]
        [Description("The maximum number of connections allowed in the pool.")]
        [DefaultValue(100)]
        [RefreshProperties(RefreshProperties.All)]
#endif
            public uint MaximumPoolSize
        {
            get { return maxPoolSize; }
            set
            {
                SetValue("Maximum Pool Size", value); 
                maxPoolSize = value;
            }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating if the connection should be reset when retrieved
        /// from the pool.
        /// </summary>
#if !CF && !MONO
        [Category("Pooling")]
        [DisplayName("Connection Reset")]
        [Description("When true, indicates the connection state is reset when " +
                     "removed from the pool.")]
        [DefaultValue(true)]
        [RefreshProperties(RefreshProperties.All)]
#endif
            public bool ConnectionReset
        {
            get { return connectionReset; }
            set
            {
                SetValue("Connection Reset", value); 
                connectionReset = value;
            }
        }

        #endregion

        #region Language and Character Set Properties

#if !CF && !MONO
        /// <summary>
        /// Gets or sets the character set that should be used for sending queries to the server.
        /// </summary>
        [DisplayName("Character Set")]
        [Category("Advanced")]
        [Description("Character set this connection should use")]
        [RefreshProperties(RefreshProperties.All)]
#endif
        public string CharacterSet
        {
            get { return charSet; }
            set
            {
                SetValue("Character Set", value);
                charSet = value;
            }
        }

#if !CF && !MONO
        /// <summary>
        /// Indicates whether the driver should treat binary blobs as UTF8
        /// </summary>
        [DisplayName("Treat Blobs As UTF8")]
        [Category("Advanced")]
        [Description("Should binary blobs be treated as UTF8")]
        [RefreshProperties(RefreshProperties.All)]
#endif
        public bool TreatBlobsAsUTF8
        {
            get { return treatBlobsAsUTF8; }
            set
            {
                SetValue("TreatBlobsAsUTF8", value);
                treatBlobsAsUTF8 = value;
            }
        }

#if !CF && !MONO
        /// <summary>
        /// Gets or sets the pattern that matches the columns that should be treated as UTF8
        /// </summary>
        [DisplayName("BlobAsUTF8IncludePattern")]
        [Category("Advanced")]
        [Description("Pattern that matches columns that should be treated as UTF8")]
        [RefreshProperties(RefreshProperties.All)]
#endif
        public string BlobAsUTF8IncludePattern
        {
            get { return blobAsUtf8IncludePattern; }
            set
            {
                SetValue("BlobAsUTF8IncludePattern", value);
                blobAsUtf8IncludePattern = value;
                blobAsUtf8IncludeRegex = null;
            }
        }

#if !CF && !MONO
        /// <summary>
        /// Gets or sets the pattern that matches the columns that should not be treated as UTF8
        /// </summary>
        [DisplayName("BlobAsUTF8ExcludePattern")]
        [Category("Advanced")]
        [Description("Pattern that matches columns that should not be treated as UTF8")]
        [RefreshProperties(RefreshProperties.All)]
#endif
        public string BlobAsUTF8ExcludePattern
        {
            get { return blobAsUtf8ExcludePattern; }
            set
            {
                SetValue("BlobAsUTF8ExcludePattern", value);
                blobAsUtf8ExcludePattern = value;
                blobAsUtf8ExcludeRegex = null;
            }
        }

        #endregion

        #region Conversion Routines

        private static uint ConvertToUInt(object value)
        {
            try
            {
                uint uValue = (value as IConvertible).ToUInt32(CultureInfo.InvariantCulture);
                return uValue;
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException(Resources.ImproperValueFormat, value.ToString());
            }
        }

        private static bool ConvertToBool(object value)
        {
            string valAsString = value as string;
            if (valAsString != null)
            {
                string s = valAsString.ToUpper(CultureInfo.InvariantCulture);
                if (s == "YES" || s == "TRUE") return true;
                if (s == "NO" || s == "FALSE") return false;
                throw new ArgumentException(Resources.ImproperValueFormat, valAsString);
            }
            try
            {
                return (value as IConvertible).ToBoolean(CultureInfo.InvariantCulture);
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException(Resources.ImproperValueFormat, value.ToString());
            }
        }

        private static MySqlConnectionProtocol ConvertToProtocol(object value)
        {
            try
            {
                if (value is MySqlConnectionProtocol) 
                    return (MySqlConnectionProtocol) value;
                return (MySqlConnectionProtocol) Enum.Parse(
                                                     typeof (MySqlConnectionProtocol), value.ToString(), true);
            }
            catch (Exception)
            {
                string valAsString = value as String;
                if (valAsString != null)
                {
                    string upperString = valAsString.ToUpper(CultureInfo.InvariantCulture);
                    if (upperString == "SOCKET" || upperString == "TCP")
                        return MySqlConnectionProtocol.Sockets;
                    if (upperString == "PIPE")
                        return MySqlConnectionProtocol.NamedPipe;
                    if (upperString == "UNIX")
                        return MySqlConnectionProtocol.UnixSocket;
                    if (upperString == "MEMORY")
                        return MySqlConnectionProtocol.SharedMemory;
                }
            }
            throw new ArgumentException(Resources.ImproperValueFormat, value.ToString());
        }

        private static MySqlDriverType ConvertToDriverType(object value)
        {
            if (value is MySqlDriverType) return (MySqlDriverType) value;
            return (MySqlDriverType) Enum.Parse(
                                         typeof (MySqlDriverType), value.ToString(), true);
        }

        #endregion

        #region Internal Properties

        internal Regex BlobAsUTF8IncludeRegex
        {
            get
            {
                if (blobAsUtf8IncludePattern == null) return null;
                if (blobAsUtf8IncludeRegex == null)
                    blobAsUtf8IncludeRegex = new Regex(blobAsUtf8IncludePattern);
                return blobAsUtf8IncludeRegex;
            }
        }

        internal Regex BlobAsUTF8ExcludeRegex
        {
            get 
            {
                if (blobAsUtf8ExcludePattern == null) return null;
                if (blobAsUtf8ExcludeRegex == null)
                    blobAsUtf8ExcludeRegex = new Regex(blobAsUtf8ExcludePattern);
                return blobAsUtf8ExcludeRegex;
            }
        }

        #endregion

        /// <summary>
        /// Takes a given connection string and returns it, possible
        /// stripping out the password info
        /// </summary>
        /// <returns></returns>
        internal string GetConnectionString(bool includePass)
        {
            if (includePass)
                return originalConnectionString;
            string connStr = persistConnString.ToString();
            return connStr.Remove(connStr.Length - 1, 1);
        }

        /// <summary>
        /// Clears the contents of the <see cref="MySqlConnectionStringBuilder"/> instance. 
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            persistConnString.Remove(0, persistConnString.Length);

            clearing = true;
            // set all the proper defaults
            foreach (KeyValuePair<Keyword, object> k in defaultValues)
                SetValue(k.Key, k.Value);
            clearing = false;
        }

        private static Keyword GetKey(string key)
        {
            string lowerKey = key.ToUpper(CultureInfo.InvariantCulture);
            switch (lowerKey)
            {
                case "UID":
                case "USERNAME":
                case "USER ID":
                case "USER NAME":
                case "USERID":
                case "USER":
                    return Keyword.UserID;
                case "HOST":
                case "SERVER":
                case "DATA SOURCE":
                case "DATASOURCE":
                case "ADDRESS":
                case "ADDR":
                case "NETWORK ADDRESS":
                    return Keyword.Server;
                case "PASSWORD":
                case "PWD":
                    return Keyword.Password;
                case "USEUSAGEADVISOR":
                case "USAGE ADVISOR":
                case "USE USAGE ADVISOR":
                    return Keyword.UseUsageAdvisor;
                case "CHARACTER SET":
                case "CHARSET":
                    return Keyword.CharacterSet;
                case "USE COMPRESSION":
                case "COMPRESS":
                    return Keyword.Compress;
                case "PIPE NAME":
                case "PIPE":
                    return Keyword.PipeName;
                case "LOGGING":
                    return Keyword.Logging;
                case "USE OLD SYNTAX":
                case "OLD SYNTAX":
                case "OLDSYNTAX":
                    return Keyword.OldSyntax;
                case "SHARED MEMORY NAME":
                    return Keyword.SharedMemoryName;
                case "ALLOW BATCH":
                    return Keyword.AllowBatch;
                case "CONVERT ZERO DATETIME":
                case "CONVERTZERODATETIME":
                    return Keyword.ConvertZeroDatetime;
                case "PERSIST SECURITY INFO":
                    return Keyword.PersistSecurityInfo;
                case "INITIAL CATALOG":
                case "DATABASE":
                    return Keyword.Database;
                case "CONNECTION TIMEOUT":
                case "CONNECT TIMEOUT":
                    return Keyword.ConnectionTimeout;
                case "PORT":
                    return Keyword.Port;
                case "POOLING":
                    return Keyword.Pooling;
                case "MIN POOL SIZE":
                case "MINIMUM POOL SIZE":
                    return Keyword.MinimumPoolSize;
                case "MAX POOL SIZE":
                case "MAXIMUM POOL SIZE":
                    return Keyword.MaximumPoolSize;
                case "CONNECTION LIFETIME":
                    return Keyword.ConnectionLifetime;
                case "DRIVER":
                    return Keyword.DriverType;
                case "PROTOCOL":
                case "CONNECTION PROTOCOL":
                    return Keyword.Protocol;
                case "ALLOW ZERO DATETIME":
                case "ALLOWZERODATETIME":
                    return Keyword.AllowZeroDatetime;
                case "USEPERFORMANCEMONITOR":
                case "USE PERFORMANCE MONITOR":
                    return Keyword.UsePerformanceMonitor;
                case "PROCEDURE CACHE SIZE":
                case "PROCEDURECACHESIZE":
                case "PROCEDURE CACHE":
                case "PROCEDURECACHE":
                    return Keyword.ProcedureCacheSize;
                case "CONNECTION RESET":
                    return Keyword.ConnectionReset;
                case "IGNORE PREPARE":
                    return Keyword.IgnorePrepare;
                case "ENCRYPT":
                    return Keyword.UseSSL;
                case "PROCEDURE BODIES":
                case "USE PROCEDURE BODIES":
                    return Keyword.UseProcedureBodies;
                case "AUTO ENLIST":
                    return Keyword.AutoEnlist;
                case "RESPECT BINARY FLAGS":
                    return Keyword.RespectBinaryFlags;
                case "BLOBASUTF8EXCLUDEPATTERN":
                    return Keyword.BlobAsUTF8ExcludePattern;
                case "BLOBASUTF8INCLUDEPATTERN":
                    return Keyword.BlobAsUTF8IncludePattern;
                case "TREATBLOBSASUTF8":
                case "TREAT BLOBS AS UTF8":
                    return Keyword.TreatBlobsAsUTF8;
                case "DEFAULT COMMAND TIMEOUT":
                    return Keyword.DefaultCommandTimeout;
                case "TREAT TINY AS BOOLEAN":
                    return Keyword.TreatTinyAsBoolean;
                case "ALLOW USER VARIABLES":
                    return Keyword.AllowUserVariables;
                case "INTERACTIVE":
                case "INTERACTIVE SESSION":
                    return Keyword.InteractiveSession;
                case "FUNCTIONS RETURN STRING":
                    return Keyword.FunctionsReturnString;
                case "use affected rows":
                    return Keyword.UseAffectedRows;
            }
            throw new ArgumentException(Resources.KeywordNotSupported, key);
        }

        private object GetValue(Keyword kw)
        {
            switch (kw)
            {
                case Keyword.UserID:
                    return UserID;
                case Keyword.Password:
                    return Password;
                case Keyword.Port:
                    return Port;
                case Keyword.Server:
                    return Server;
                case Keyword.UseUsageAdvisor:
                    return UseUsageAdvisor;
                case Keyword.CharacterSet:
                    return CharacterSet;
                case Keyword.Compress:
                    return UseCompression;
                case Keyword.PipeName:
                    return PipeName;
                case Keyword.Logging:
                    return Logging;
                case Keyword.OldSyntax:
                    return UseOldSyntax;
                case Keyword.SharedMemoryName:
                    return SharedMemoryName;
                case Keyword.AllowBatch:
                    return AllowBatch;
                case Keyword.ConvertZeroDatetime:
                    return ConvertZeroDateTime;
                case Keyword.PersistSecurityInfo:
                    return PersistSecurityInfo;
                case Keyword.Database:
                    return Database;
                case Keyword.ConnectionTimeout:
                    return ConnectionTimeout;
                case Keyword.Pooling:
                    return Pooling;
                case Keyword.MinimumPoolSize:
                    return MinimumPoolSize;
                case Keyword.MaximumPoolSize:
                    return MaximumPoolSize;
                case Keyword.ConnectionLifetime:
                    return ConnectionLifeTime;
                case Keyword.DriverType:
                    return DriverType;
                case Keyword.Protocol:
                    return ConnectionProtocol;
                case Keyword.ConnectionReset:
                    return ConnectionReset;
                case Keyword.ProcedureCacheSize:
                    return ProcedureCacheSize;
                case Keyword.AllowZeroDatetime:
                    return AllowZeroDateTime;
                case Keyword.UsePerformanceMonitor:
                    return UsePerformanceMonitor;
                case Keyword.IgnorePrepare:
                    return IgnorePrepare;
                case Keyword.UseSSL:
                    return UseSSL;
                case Keyword.UseProcedureBodies:
                    return UseProcedureBodies;
                case Keyword.AutoEnlist:
                    return AutoEnlist;
                case Keyword.RespectBinaryFlags:
                    return RespectBinaryFlags;
                case Keyword.TreatBlobsAsUTF8:
                    return TreatBlobsAsUTF8;
                case Keyword.BlobAsUTF8ExcludePattern:
                    return blobAsUtf8ExcludePattern;
                case Keyword.BlobAsUTF8IncludePattern:
                    return blobAsUtf8IncludePattern;
                case Keyword.DefaultCommandTimeout:
                    return defaultCommandTimeout;
                case Keyword.TreatTinyAsBoolean:
                    return treatTinyAsBoolean;
                case Keyword.AllowUserVariables:
                    return allowUserVariables;
                case Keyword.InteractiveSession:
                    return interactiveSession;
                case Keyword.FunctionsReturnString:
                    return functionsReturnString;
                case Keyword.UseAffectedRows:
                    return useAffectedRows;
                default:
                    return null; /* this will never happen */
            }
        }

        private void SetValue(string keyword, object value)
        {
            if (value == null)
                throw new ArgumentException(Resources.KeywordNoNull, keyword);
            object out_obj;
            TryGetValue(keyword, out out_obj);

            Keyword kw = GetKey(keyword);
            SetValue(kw, value);
            base[keyword] = value;
            if (kw != Keyword.Password)
            {
                /* Nothing bad happens if the substring is not found */
                persistConnString.Replace(keyword + "=" + out_obj + ";", "");
                persistConnString.AppendFormat(CultureInfo.InvariantCulture, "{0}={1};", keyword, value);
            }
        }

        private void SetValue(Keyword kw, object value)
        {
            string valueAsString = value as string;
            switch (kw)
            {
                case Keyword.UserID: 
                    userId = valueAsString; break;
                case Keyword.Password:
                    password = valueAsString; break;
                case Keyword.Port: 
                    port = ConvertToUInt(value); break;
                case Keyword.Server:
                    server = valueAsString; break;
                case Keyword.UseUsageAdvisor: 
                    useUsageAdvisor = ConvertToBool(value); break;
                case Keyword.CharacterSet:
                    charSet = valueAsString; break;
                case Keyword.Compress: 
                    compress = ConvertToBool(value); break;
                case Keyword.PipeName:
                    pipeName = valueAsString; break;
                case Keyword.Logging: 
                    logging = ConvertToBool(value); break;
                case Keyword.OldSyntax: 
                    oldSyntax = ConvertToBool(value);
                    if (!clearing)
                        Logger.LogWarning("Use Old Syntax is now obsolete.  Please see documentation");
                    break;
                case Keyword.SharedMemoryName:
                    sharedMemName = valueAsString; break;
                case Keyword.AllowBatch: 
                    allowBatch = ConvertToBool(value); break;
                case Keyword.ConvertZeroDatetime: 
                    convertZeroDatetime = ConvertToBool(value); break;
                case Keyword.PersistSecurityInfo: 
                    persistSI = ConvertToBool(value); break;
                case Keyword.Database:
                    database = valueAsString; break;
                case Keyword.ConnectionTimeout: 
                    connectionTimeout = ConvertToUInt(value); break;
                case Keyword.Pooling: 
                    pooling = ConvertToBool(value); break;
                case Keyword.MinimumPoolSize: 
                    minPoolSize = ConvertToUInt(value); break;
                case Keyword.MaximumPoolSize: 
                    maxPoolSize = ConvertToUInt(value); break;
                case Keyword.ConnectionLifetime: 
                    connectionLifetime = ConvertToUInt(value); break;
                case Keyword.DriverType: 
                    driverType = ConvertToDriverType(value); break;
                case Keyword.Protocol: 
                    protocol = ConvertToProtocol(value); break;
                case Keyword.ConnectionReset: 
                    connectionReset = ConvertToBool(value); break;
                case Keyword.UsePerformanceMonitor: 
                    usePerfMon = ConvertToBool(value); break;
                case Keyword.AllowZeroDatetime: 
                    allowZeroDatetime = ConvertToBool(value); break;
                case Keyword.ProcedureCacheSize: 
                    procCacheSize = ConvertToUInt(value); break;
                case Keyword.IgnorePrepare: 
                    ignorePrepare = ConvertToBool(value); break;
                case Keyword.UseSSL: 
                    useSSL = ConvertToBool(value); break;
                case Keyword.UseProcedureBodies: 
                    useProcedureBodies = ConvertToBool(value); break;
                case Keyword.AutoEnlist:
                    autoEnlist = ConvertToBool(value); break;
                case Keyword.RespectBinaryFlags:
                    respectBinaryFlags = ConvertToBool(value); break;
                case Keyword.TreatBlobsAsUTF8:
                    treatBlobsAsUTF8 = ConvertToBool(value); break;
                case Keyword.BlobAsUTF8ExcludePattern:
                    blobAsUtf8ExcludePattern = valueAsString; break;
                case Keyword.BlobAsUTF8IncludePattern:
                    blobAsUtf8IncludePattern = valueAsString; break;
                case Keyword.DefaultCommandTimeout:
                    defaultCommandTimeout = ConvertToUInt(value); break;
                case Keyword.TreatTinyAsBoolean:
                    treatTinyAsBoolean = ConvertToBool(value); break;
                case Keyword.AllowUserVariables:
                    allowUserVariables = ConvertToBool(value); break;
                case Keyword.InteractiveSession:
                    interactiveSession = ConvertToBool(value); break;
                case Keyword.FunctionsReturnString:
                    functionsReturnString = ConvertToBool(value); break;
                case Keyword.UseAffectedRows:
                    useAffectedRows = ConvertToBool(value); break;
            }
        }

        /// <summary>
        /// Gets or sets the value associated with the specified key. In C#, this property 
        /// is the indexer. 
        /// </summary>
        /// <param name="keyword">The key of the item to get or set.</param>
        /// <returns>The value associated with the specified key. </returns>
        public override object this[string keyword]
        {
            get
            {
                Keyword kw = GetKey(keyword);
                return GetValue(kw);
            }
            set
            {
                if (value == null)
                    Remove(keyword);
                else
                    SetValue(keyword, value);
            }
        }

#if !CF
        protected override void GetProperties(System.Collections.Hashtable propertyDescriptors)
        {
            base.GetProperties(propertyDescriptors);

            // use a custom type descriptor for connection protocol
            PropertyDescriptor pd = (PropertyDescriptor)propertyDescriptors["Connection Protocol"];
            Attribute[] myAttr = new Attribute[pd.Attributes.Count];
            pd.Attributes.CopyTo(myAttr, 0);
            ConnectionProtocolDescriptor mypd;
            mypd = new ConnectionProtocolDescriptor(pd.Name, myAttr);
            propertyDescriptors["Connection Protocol"] = mypd;
        }

        /// <summary>
        /// Removes the entry with the specified key from the <see cref="T:System.Data.Common.DbConnectionStringBuilder"></see> instance.
        /// </summary>
        /// <param name="keyword">The key of the key/value pair to be removed from the connection string in this <see cref="T:System.Data.Common.DbConnectionStringBuilder"></see>.</param>
        /// <returns>
        /// true if the key existed within the connection string and was removed; false if the key did not exist.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Data.Common.DbConnectionStringBuilder"></see> is read-only, or the <see cref="T:System.Data.Common.DbConnectionStringBuilder"></see> has a fixed size.</exception>
        /// <exception cref="T:System.ArgumentNullException">keyword is null (Nothing in Visual Basic)</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" PathDiscovery="*AllFiles*"/></PermissionSet>
        public override bool Remove(string keyword)
        {
            // first we need to set this keys value to the default
            Keyword kw = GetKey(keyword);
            SetValue(kw, defaultValues[kw]);

            // then we remove this keyword from the base collection
            return base.Remove(keyword);
        }

        /// <summary>
        /// Retrieves a value corresponding to the supplied key from this <see cref="T:System.Data.Common.DbConnectionStringBuilder"></see>.
        /// </summary>
        /// <param name="keyword">The key of the item to retrieve.</param>
        /// <param name="value">The value corresponding to the key.</param>
        /// <returns>
        /// true if keyword was found within the connection string, false otherwise.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">keyword contains a null value (Nothing in Visual Basic).</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" PathDiscovery="*AllFiles*"/></PermissionSet>
        public override bool TryGetValue(string keyword, out object value)
        {
            try
            {
                Keyword kw = GetKey(keyword);
                value = GetValue(kw);
                return true;
            }
            catch (ArgumentException)
            {
            }
            value = null;
            return false;
        }
#endif
    }

    #region ConnectionProtocolDescriptor

    internal class ConnectionProtocolDescriptor : PropertyDescriptor
    {
        public ConnectionProtocolDescriptor(string name, Attribute[] attr) : base(name, attr)
        {
        }

        public override bool CanResetValue(object component)
        {
            return true;
        }

        public override Type ComponentType
        {
            get { return typeof(MySqlConnectionStringBuilder); }
        }

        public override object GetValue(object component)
        {
            MySqlConnectionStringBuilder cb = (MySqlConnectionStringBuilder) component;
            return cb.ConnectionProtocol;
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override Type PropertyType
        {
            get { return typeof(MySqlConnectionProtocol); }
        }

        public override void ResetValue(object component)
        {
            MySqlConnectionStringBuilder cb = (MySqlConnectionStringBuilder)component;
            cb.ConnectionProtocol = MySqlConnectionProtocol.Sockets;
        }

        public override void SetValue(object component, object value)
        {
            MySqlConnectionStringBuilder cb = (MySqlConnectionStringBuilder)component;
            cb.ConnectionProtocol = (MySqlConnectionProtocol) value;
        }

        public override bool ShouldSerializeValue(object component)
        {
            MySqlConnectionStringBuilder cb = (MySqlConnectionStringBuilder)component;
            return cb.ConnectionProtocol != MySqlConnectionProtocol.Sockets;
        }
    }

    #endregion

    internal enum Keyword
    {
        UserID,
        Password,
        Server,
        Port,
        UseUsageAdvisor,
        CharacterSet,
        Compress,
        PipeName,
        Logging,
        OldSyntax,
        SharedMemoryName,
        AllowBatch,
        ConvertZeroDatetime,
        PersistSecurityInfo,
        Database,
        ConnectionTimeout,
        Pooling,
        MinimumPoolSize,
        MaximumPoolSize,
        ConnectionLifetime,
        DriverType,
        Protocol,
        ConnectionReset,
        AllowZeroDatetime,
        UsePerformanceMonitor,
        ProcedureCacheSize,
        IgnorePrepare,
        UseSSL,
        UseProcedureBodies,
        AutoEnlist,
        RespectBinaryFlags,
        TreatBlobsAsUTF8,
        BlobAsUTF8IncludePattern,
        BlobAsUTF8ExcludePattern,
        DefaultCommandTimeout,
        TreatTinyAsBoolean,
        AllowUserVariables,
        InteractiveSession,
        FunctionsReturnString,
        UseAffectedRows
    }
}
