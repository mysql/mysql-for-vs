// Copyright (C) 2004-2007 MySQL AB
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
using MySql.Data.Common;
using System.Globalization;
using System.Collections.Generic;
using System.Text;

namespace MySql.Data.MySqlClient
{
    /// <include file='docs/MySqlConnectionStringBuilder.xml' path='docs/Class/*'/>
    public sealed class MySqlConnectionStringBuilder : DbConnectionStringBuilder
    {
        string userId, password, server;
        string database, sharedMemName, pipeName, charSet;
        string optionFile;
        string originalConnectionString;
        StringBuilder persistConnString;
        uint port, connectionTimeout, minPoolSize, maxPoolSize;
        uint procCacheSize, connectionLifetime;
        MySqlConnectionProtocol protocol;
        MySqlDriverType driverType;
        bool compress, connectionReset, allowBatch, logging;
        bool oldSyntax, persistSI, usePerfMon, pooling;
        bool allowZeroDatetime, convertZeroDatetime;
        bool useUsageAdvisor, useSSL;
        bool ignorePrepare;
        bool useProcedureBodies;

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
            : base()
        {
            originalConnectionString = connectionString;
            persistConnString = new StringBuilder();
            ConnectionString = connectionString;
        }

        #region Server Properties

        /// <summary>
        /// Gets or sets the name of the server.
        /// </summary>
#if !PocketPC && !MONO
        [Category("Connection")]
        [Description("Server to connect to")]
#endif
        public string Server
        {
            get { return this.server; }
            set { CheckNullAndSet("Server", value); server = value; }
        }

        /// <summary>
        /// Gets or sets the name of the database the connection should 
        /// initially connect to.
        /// </summary>
#if !PocketPC && !MONO
        [Category("Connection")]
        [Description("Database to use initially")]
#endif
        public string Database
        {
            get { return this.database; }
            set { CheckNullAndSet("Database", value); database = value; }
        }

        /// <summary>
        /// Gets or sets the protocol that should be used for communicating
        /// with MySQL.
        /// </summary>
#if !PocketPC && !MONO
        [Category("Connection")]
        [DisplayName("Connection Protocol")]
        [Description("Protocol to use for connection to MySQL")]
        [DefaultValue(MySqlConnectionProtocol.Sockets)]
#endif
        public MySqlConnectionProtocol ConnectionProtocol
        {
            get { return this.protocol; }
            set { base["Protocol"] = value; protocol = value; }
        }

        /// <summary>
        /// Gets or sets the name of the named pipe that should be used
        /// for communicating with MySQL.
        /// </summary>
#if !PocketPC && !MONO
        [Category("Connection")]
        [DisplayName("Pipe Name")]
        [Description("Name of pipe to use when connecting with named pipes (Win32 only)")]
#endif
        public string PipeName
        {
            get { return this.pipeName; }
            set { CheckNullAndSet("Pipe Name", value); pipeName = value; }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether this connection
        /// should use compression.
        /// </summary>
#if !PocketPC && !MONO
        [Category("Connection")]
        [DisplayName("Use Compression")]
        [Description("Should the connection ues compression")]
        [DefaultValue(false)]
#endif
        public bool UseCompression
        {
            get { return this.compress; }
            set { base["compress"] = value; compress = value; }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether this connection will allow
        /// commands to send multiple SQL statements in one execution.
        /// </summary>
#if !PocketPC && !MONO
        [Category("Connection")]
        [DisplayName("Allow Batch")]
        [Description("Allows execution of multiple SQL commands in a single statement")]
        [DefaultValue(true)]
#endif
        public bool AllowBatch
        {
            get { return this.allowBatch; }
            set { base["allow batch"] = value; allowBatch = value; }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether logging is enabled.
        /// </summary>
#if !PocketPC && !MONO
        [Category("Connection")]
        [Description("Enables output of diagnostic messages")]
        [DefaultValue(false)]
#endif
        public bool Logging
        {
            get { return this.logging; }
            set { base["logging"] = value; logging = value; }
        }

        /// <summary>
        /// Gets or sets the base name of the shared memory objects used to 
        /// communicate with MySQL when the shared memory protocol is being used.
        /// </summary>
#if !PocketPC && !MONO
        [Category("Connection")]
        [DisplayName("Shared Memory Name")]
        [Description("Name of the shared memory object to use")]
        [DefaultValue("MYSQL")]
#endif
        public string SharedMemoryName
        {
            get { return this.sharedMemName; }
            set { CheckNullAndSet("Shared Memory Name", value); sharedMemName = value; }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether this connection uses
        /// the old style (@) parameter markers or the new (?) style.
        /// </summary>
#if !PocketPC && !MONO
        [Category("Connection")]
        [DisplayName("Use Old Syntax")]
        [Description("Allows the use of old style @ syntax for parameters")]
        [DefaultValue(false)]
#endif
        public bool UseOldSyntax
        {
            get { return this.oldSyntax; }
            set { base["Old Syntax"] = value; oldSyntax = value; }
        }

        /// <summary>
        /// Gets or sets the driver type that should be used for this connection.
        /// </summary>
        /// <remarks>
        /// There is only one valid value for this setting currently.
        /// </remarks>
#if !PocketPC && !MONO
        [Category("Connection")]
        [DisplayName("Driver Type")]
        [Description("Specifies the type of driver to use for this connection")]
        [DefaultValue(MySqlDriverType.Native)]
#endif
        public MySqlDriverType DriverType
        {
            get { return this.driverType; }
            set { base["Driver Type"] = value; driverType = value; }
        }

        internal string OptionFile
        {
            get { return this.optionFile; }
            set { CheckNullAndSet("Option File", value); optionFile = value; }
        }

        /// <summary>
        /// Gets or sets the port number that is used when the socket
        /// protocol is being used.
        /// </summary>
#if !PocketPC && !MONO
        [Category("Connection")]
        [Description("Port to use for TCP/IP connections")]
        [DefaultValue(3306)]
#endif
        public uint Port
        {
            get { return this.port; }
            set { base["Port"] = value; port = value; }
        }

        /// <summary>
        /// Gets or sets the connection timeout.
        /// </summary>
#if !PocketPC && !MONO
        [Category("Connection")]
        [DisplayName("Connect Timeout")]
        [Description("The length of time (in seconds) to wait for a connection " +
             "to the server before terminating the attempt and generating an error.")]
        [DefaultValue(15)]
#endif
        public uint ConnectionTimeout
        {
            get { return this.connectionTimeout; }
            set { base["Connection Timeout"] = value; connectionTimeout = value; }
        }

        #endregion

        #region Authentication Properties

        /// <summary>
        /// Gets or sets the user id that should be used to connect with.
        /// </summary>
#if !PocketPC && !MONO
        [Category("Security")]
        [DisplayName("User ID")]
        [Description("Indicates the user ID to be used when connecting to the data source.")]
#endif
        public string UserID
        {
            get { return this.userId; }
            set { CheckNullAndSet("User Id", value); userId = value; }
        }

        /// <summary>
        /// Gets or sets the password that should be used to connect with.
        /// </summary>
#if !PocketPC && !MONO
        [Category("Security")]
        [Description("Indicates the password to be used when connecting to the data source.")]
#endif
        public string Password
        {
            get { return this.password; }
            set { CheckNullAndSet("Password", value); password = value; }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates if the password should be persisted
        /// in the connection string.
        /// </summary>
#if !PocketPC && !MONO
        [Category("Security")]
        [DisplayName("Persist Security Info")]
        [Description("When false, security-sensitive information, such as the password, " +
             "is not returned as part of the connection if the connection is open or " +
             "has ever been in an open state.")]
#endif
        public bool PersistSecurityInfo
        {
            get { return persistSI; }
            set { base["Persist Security Info"] = value; persistSI = value; }
        }

#if !PocketPC && !MONO
        [Category("Authentication")]
        [Description("Should the connection use SSL.  This currently has no effect.")]
        [DefaultValue(false)]
#endif
        internal bool UseSSL
        {
            get { return useSSL; }
            set { base["usessl"] = value; useSSL = value; }
        }

        #endregion

        #region Other Properties

        /// <summary>
        /// Gets or sets a boolean value that indicates if zero date time values are supported.
        /// </summary>
#if !PocketPC && !MONO
        [Category("Advanced")]
        [DisplayName("Allow Zero Datetime")]
        [Description("Should zero datetimes be supported")]
        [DefaultValue(false)]
#endif
        public bool AllowZeroDateTime
        {
            get { return this.allowZeroDatetime; }
            set { base["Allow Zero DateTime"] = value; allowZeroDatetime = value; }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating if zero datetime values should be 
        /// converted to DateTime.MinValue.
        /// </summary>
#if !PocketPC && !MONO
        [Category("Advanced")]
        [DisplayName("Convert Zero Datetime")]
        [Description("Should illegal datetime values be converted to DateTime.MinValue")]
        [DefaultValue(false)]
#endif
        public bool ConvertZeroDateTime
        {
            get { return this.convertZeroDatetime; }
            set { base["Convert Zero DateTime"] = value; convertZeroDatetime = value; }
        }

        /// <summary>
        /// Gets or sets the character set that should be used for sending queries to the server.
        /// </summary>
#if !PocketPC && !MONO
        [Category("Advanced")]
        [Description("Character set this connection should use")]
#endif
        public string CharacterSet
        {
            get { return this.charSet; }
            set { CheckNullAndSet("Character Set", value); charSet = value; }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating if the Usage Advisor should be enabled.
        /// </summary>
#if !PocketPC && !MONO
        [Category("Advanced")]
        [DisplayName("Use Usage Advisor")]
        [Description("Logs inefficient database operations")]
        [DefaultValue(false)]
#endif
        public bool UseUsageAdvisor
        {
            get { return useUsageAdvisor; }
            set { base["Use Usage Advisor"] = value; useUsageAdvisor = value; }
        }

        /// <summary>
        /// Gets or sets the size of the stored procedure cache.
        /// </summary>
#if !PocketPC && !MONO
        [Category("Advanced")]
        [DisplayName("Procedure Cache Size")]
        [Description("Indicates how many stored procedures can be cached at one time. " +
                "A value of 0 effectively disables the procedure cache.")]
        [DefaultValue(25)]
#endif
        public uint ProcedureCacheSize
        {
            get { return this.procCacheSize; }
            set { base["Procedure Cache Size"] = value; procCacheSize = value; }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating if the permon hooks should be enabled.
        /// </summary>
#if !PocketPC && !MONO
        [Category("Advanced")]
        [DisplayName("Use Performance Monitor")]
        [Description("Indicates that performance counters should be updated during execution.")]
        [DefaultValue(false)]
#endif
        public bool UsePerformanceMonitor
        {
            get { return usePerfMon; }
            set { base["Use Performance Monitor"] = value; usePerfMon = value; }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating if calls to Prepare() should be ignored.
        /// </summary>
#if !PocketPC && !MONO
        [Category("Advanced")]
        [DisplayName("Ignore Prepare")]
        [Description("Instructs the provider to ignore any attempts to prepare a command.")]
        [DefaultValue(true)]
#endif
        public bool IgnorePrepare
        {
            get { return ignorePrepare; }
            set { base["Ignore Prepare"] = value; ignorePrepare = value; }
        }

#if !PocketPC && !MONO
        [Category("Advanced")]
        [DisplayName("Use Procedure Bodies")]
        [Description("Indicates if stored procedure bodies will be available for parameter detection.")]
        [DefaultValue(true)]
#endif
        public bool UseProcedureBodies
        {
            get { return useProcedureBodies; }
            set { base["Use Procedure Bodies"] = value; useProcedureBodies = value; }
        }

        #endregion

        #region Pooling Properties

        /// <summary>
        /// Gets or sets the lifetime of a pooled connection.
        /// </summary>
#if !PocketPC && !MONO
        [Category("Pooling")]
        [DisplayName("Load Balance Timeout")]
        [Description("The minimum amount of time (in seconds) for this connection to " +
             "live in the pool before being destroyed.")]
        [DefaultValue(0)]
#endif
        public uint ConnectionLifeTime
        {
            get { return connectionLifetime; }
            set { base["Connection Lifetime"] = value; connectionLifetime = value; }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating if connection pooling is enabled.
        /// </summary>
#if !PocketPC && !MONO
        [Category("Pooling")]
        [Description("When true, the connection object is drawn from the appropriate " +
             "pool, or if necessary, is created and added to the appropriate pool.")]
        [DefaultValue(true)]
#endif
        public bool Pooling
        {
            get { return pooling; }
            set { base["Pooling"] = value; pooling = value; }
        }

        /// <summary>
        /// Gets the minimum connection pool size.
        /// </summary>
#if !PocketPC && !MONO
        [Category("Pooling")]
        [DisplayName("Min Pool Size")]
        [Description("The minimum number of connections allowed in the pool.")]
        [DefaultValue(0)]
#endif
        public uint MinimumPoolSize
        {
            get { return minPoolSize; }
            set { base["Minimum Pool Size"] = value; minPoolSize = value; }
        }

        /// <summary>
        /// Gets or sets the maximum connection pool setting.
        /// </summary>
#if !PocketPC && !MONO
        [Category("Pooling")]
        [DisplayName("Max Pool Size")]
        [Description("The maximum number of connections allowed in the pool.")]
        [DefaultValue(100)]
#endif
        public uint MaximumPoolSize
        {
            get { return maxPoolSize; }
            set { base["Maximum Pool Size"] = value; maxPoolSize = value; }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating if the connection should be reset when retrieved
        /// from the pool.
        /// </summary>
#if !PocketPC && !MONO
        [Category("Pooling")]
        [DisplayName("Connection Reset")]
        [Description("When true, indicates the connection state is reset when " +
             "removed from the pool.")]
        [DefaultValue(true)]
#endif
        public bool ConnectionReset
        {
            get { return connectionReset; }
            set { base["Connection Reset"] = value; connectionReset = value; }
        }

        #endregion

        #region Conversion Routines

        private void CheckNullAndSet(string keyword, object value)
        {
            if (value == null)
                throw new ArgumentException(Resources.KeywordNoNull, keyword);
            base[keyword] = value;
        }

        private uint ConvertToUInt(object value)
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

        private bool ConvertToBool(object value)
        {
            if (value is string)
            {
                string s = value.ToString().ToLower();
                if (s == "yes" || s == "true") return true;
                if (s == "no" || s == "false") return false;
                throw new ArgumentException(Resources.ImproperValueFormat, (string)value);
            }
            else
            {
                try
                {
                    return (value as IConvertible).ToBoolean(
                         CultureInfo.InvariantCulture);
                }
                catch (InvalidCastException)
                {
                    throw new ArgumentException(Resources.ImproperValueFormat, value.ToString());
                }
            }
        }

        private MySqlConnectionProtocol ConvertToProtocol(object value)
        {
            try
            {
                if (value is MySqlConnectionProtocol) return (MySqlConnectionProtocol)value;
                return (MySqlConnectionProtocol)Enum.Parse(
                     typeof(MySqlConnectionProtocol), value.ToString(), true);
            }
            catch (Exception)
            {
                if (value is string)
                {
                    string lowerString = (value as string).ToLower();
                    if (lowerString == "socket" || lowerString == "tcp")
                        return MySqlConnectionProtocol.Sockets;
                    else if (lowerString == "pipe")
                        return MySqlConnectionProtocol.NamedPipe;
                    else if (lowerString == "unix")
                        return MySqlConnectionProtocol.UnixSocket;
                    else if (lowerString == "memory")
                        return MySqlConnectionProtocol.SharedMemory;
                }
            }
            throw new ArgumentException(Resources.ImproperValueFormat, value.ToString());
        }

        private MySqlDriverType ConvertToDriverType(object value)
        {
            if (value is MySqlDriverType) return (MySqlDriverType)value;
            return (MySqlDriverType)Enum.Parse(
                 typeof(MySqlDriverType), value.ToString(), true);
            throw new ArgumentException(Resources.ImproperValueFormat, value.ToString());
        }

        #endregion

        #region Private Methods

        private void Reset()
        {
            connectionTimeout = 15;
            pooling = true;
            port = 3306;
            server = String.Empty;
            persistSI = false;
            connectionLifetime = 0;
            connectionReset = false;
            minPoolSize = 0;
            maxPoolSize = 100;
            userId = "";
            password = "";
            useUsageAdvisor = false;
            charSet = "";
            compress = false;
            pipeName = "MYSQL";
            logging = false;
            oldSyntax = false;
            sharedMemName = "MYSQL";
            allowBatch = true;
            convertZeroDatetime = false;
            database = "";
            driverType = MySqlDriverType.Native;
            protocol = MySqlConnectionProtocol.Sockets;
            allowZeroDatetime = false;
            usePerfMon = false;
            procCacheSize = 25;
            useSSL = false;
            ignorePrepare = true;
            useProcedureBodies = true;
        }

        #endregion

        /// <summary>
        /// Takes a given connection string and returns it, possible
        /// stripping out the password info
        /// </summary>
        /// <returns></returns>
        public string GetConnectionString(bool includePass)
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
            Reset();
        }

        private Keyword GetKey(string key)
        {
            string lowerKey = key.ToLower();
            switch (lowerKey)
            {
                case "uid":
                case "username":
                case "user id":
                case "user name":
                case "userid":
                case "user":
                    return Keyword.UserID;
                case "host":
                case "server":
                case "data source":
                case "datasource":
                case "address":
                case "addr":
                case "network address":
                    return Keyword.Server;
                case "password":
                case "pwd":
                    return Keyword.Password;
                case "useusageadvisor":
                case "usage advisor":
                case "use usage advisor":
                    return Keyword.UseUsageAdvisor;
                case "character set":
                case "charset":
                    return Keyword.CharacterSet;
                case "use compression":
                case "compress":
                    return Keyword.Compress;
                case "pipe name":
                case "pipe":
                    return Keyword.PipeName;
                case "logging":
                    return Keyword.Logging;
                case "old syntax":
                case "oldsyntax":
                    return Keyword.OldSyntax;
                case "shared memory name":
                    return Keyword.SharedMemoryName;
                case "allow batch":
                    return Keyword.AllowBatch;
                case "convert zero datetime":
                case "convertzerodatetime":
                    return Keyword.ConvertZeroDatetime;
                case "persist security info":
                    return Keyword.PersistSecurityInfo;
                case "initial catalog":
                case "database":
                    return Keyword.Database;
                case "connection timeout":
                case "connect timeout":
                    return Keyword.ConnectionTimeout;
                case "port":
                    return Keyword.Port;
                case "pooling":
                    return Keyword.Pooling;
                case "min pool size":
                case "minimum pool size":
                    return Keyword.MinimumPoolSize;
                case "max pool size":
                case "maximum pool size":
                    return Keyword.MaximumPoolSize;
                case "connection lifetime":
                    return Keyword.ConnectionLifetime;
                case "driver":
                    return Keyword.DriverType;
                case "protocol":
                    return Keyword.Protocol;
                case "allow zero datetime":
                case "allowzerodatetime":
                    return Keyword.AllowZeroDatetime;
                case "useperformancemonitor":
                case "use performance monitor":
                    return Keyword.UsePerformanceMonitor;
                case "procedure cache size":
                case "procedurecachesize":
                case "procedure cache":
                case "procedurecache":
                    return Keyword.ProcedureCacheSize;
                case "connection reset":
                    return Keyword.ConnectionReset;
                case "ignore prepare":
                    return Keyword.IgnorePrepare;
                case "encrypt":
                    return Keyword.UseSSL;
                case "procedure bodies":
                case "use procedure bodies":
                    return Keyword.UseProcedureBodies;
            }
            throw new ArgumentException(Resources.KeywordNotSupported, key);
        }

        private object GetValue(Keyword kw)
        {
            switch (kw)
            {
                case Keyword.UserID: return UserID;
                case Keyword.Password: return Password;
                case Keyword.Port: return Port;
                case Keyword.Server: return Server;
                case Keyword.UseUsageAdvisor: return UseUsageAdvisor;
                case Keyword.CharacterSet: return CharacterSet;
                case Keyword.Compress: return UseCompression;
                case Keyword.PipeName: return PipeName;
                case Keyword.Logging: return Logging;
                case Keyword.OldSyntax: return UseOldSyntax;
                case Keyword.SharedMemoryName: return SharedMemoryName;
                case Keyword.AllowBatch: return AllowBatch;
                case Keyword.ConvertZeroDatetime: return ConvertZeroDateTime;
                case Keyword.PersistSecurityInfo: return PersistSecurityInfo;
                case Keyword.Database: return Database;
                case Keyword.ConnectionTimeout: return ConnectionTimeout;
                case Keyword.Pooling: return Pooling;
                case Keyword.MinimumPoolSize: return MinimumPoolSize;
                case Keyword.MaximumPoolSize: return MaximumPoolSize;
                case Keyword.ConnectionLifetime: return ConnectionLifeTime;
                case Keyword.DriverType: return DriverType;
                case Keyword.Protocol: return ConnectionProtocol;
                case Keyword.ConnectionReset: return ConnectionReset;
                case Keyword.ProcedureCacheSize: return ProcedureCacheSize;
                case Keyword.AllowZeroDatetime: return AllowZeroDateTime;
                case Keyword.UsePerformanceMonitor: return UsePerformanceMonitor;
                case Keyword.IgnorePrepare: return IgnorePrepare;
                case Keyword.UseSSL: return UseSSL;
                case Keyword.UseProcedureBodies: return UseProcedureBodies;
                default: return null;  /* this will never happen */
            }
        }

        private void SetValue(Keyword kw, object value)
        {
            switch (kw)
            {
                case Keyword.UserID: UserID = (string)value; break;
                case Keyword.Password: Password = (string)value; break;
                case Keyword.Port: Port = ConvertToUInt(value); break;
                case Keyword.Server: Server = (string)value; break;
                case Keyword.UseUsageAdvisor: UseUsageAdvisor = ConvertToBool(value); break;
                case Keyword.CharacterSet: CharacterSet = (string)value; break;
                case Keyword.Compress: UseCompression = ConvertToBool(value); break;
                case Keyword.PipeName: PipeName = (string)value; break;
                case Keyword.Logging: Logging = ConvertToBool(value); break;
                case Keyword.OldSyntax: UseOldSyntax = ConvertToBool(value); break;
                case Keyword.SharedMemoryName: SharedMemoryName = (string)value; break;
                case Keyword.AllowBatch: AllowBatch = ConvertToBool(value); break;
                case Keyword.ConvertZeroDatetime: ConvertZeroDateTime = ConvertToBool(value); break;
                case Keyword.PersistSecurityInfo: PersistSecurityInfo = ConvertToBool(value); break;
                case Keyword.Database: Database = (string)value; break;
                case Keyword.ConnectionTimeout: ConnectionTimeout = ConvertToUInt(value); break;
                case Keyword.Pooling: Pooling = ConvertToBool(value); break;
                case Keyword.MinimumPoolSize: MinimumPoolSize = ConvertToUInt(value); break;
                case Keyword.MaximumPoolSize: MaximumPoolSize = ConvertToUInt(value); break;
                case Keyword.ConnectionLifetime: ConnectionLifeTime = ConvertToUInt(value); break;
                case Keyword.DriverType: DriverType = ConvertToDriverType(value); break;
                case Keyword.Protocol: ConnectionProtocol = ConvertToProtocol(value); break;
                case Keyword.ConnectionReset: ConnectionReset = ConvertToBool(value); break;
                case Keyword.UsePerformanceMonitor: UsePerformanceMonitor = ConvertToBool(value); break;
                case Keyword.AllowZeroDatetime: AllowZeroDateTime = ConvertToBool(value); break;
                case Keyword.ProcedureCacheSize: ProcedureCacheSize = ConvertToUInt(value); break;
                case Keyword.IgnorePrepare: IgnorePrepare = ConvertToBool(value); break;
                case Keyword.UseSSL: UseSSL = ConvertToBool(value); break;
                case Keyword.UseProcedureBodies: UseProcedureBodies = ConvertToBool(value); break;
            }
        }

        /// <summary>
        /// Gets or sets the value associated with the specified key. In C#, this property 
        /// is the indexer. 
        /// </summary>
        /// <param name="key">The key of the item to get or set.</param>
        /// <returns>The value associated with the specified key. </returns>
		public override object this[string key]
		{
			get
			{
				Keyword kw = GetKey(key);
				return GetValue(kw);
			}
			set
			{
				Keyword kw = GetKey(key);
				if (kw != Keyword.Password)
					persistConnString.AppendFormat(CultureInfo.InvariantCulture, "{0}={1};", key, value);
				SetValue(kw, value);
			}
		}
	}

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
        UseProcedureBodies
    }

/*    internal class ProtocolConverter : TypeConverter.StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(new string[] { "Sockets", "NamedPipe", "UnixSocket", "SharedMemory" });
        }
    }*/
}
