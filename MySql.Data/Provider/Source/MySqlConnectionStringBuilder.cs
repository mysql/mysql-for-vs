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
using System.Data.Common;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Text;
using MySql.Data.MySqlClient.Properties;
using System.Collections;
using System.Globalization;

namespace MySql.Data.MySqlClient
{
    public class MySqlConnectionStringBuilder : DbConnectionStringBuilder
    {
        private static Dictionary<string, string> validKeywords =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private static Dictionary<string, PropertyDefaultValue> defaultValues =
            new Dictionary<string, PropertyDefaultValue>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, object> values =
            new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        static MySqlConnectionStringBuilder()
        {
            // load up our valid keywords and default values only once
            Initialize();
        }

        public MySqlConnectionStringBuilder()
        {
            Clear();
        }

        public MySqlConnectionStringBuilder(string connStr)
            : this()
        {
            ConnectionString = connStr;
        }

        #region Server Properties

        /// <summary>
        /// Gets or sets the name of the server.
        /// </summary>
        /// <value>The server.</value>
        [Category("Connection")]
        [Description("Server to connect to")]
        [DefaultValue("")]
        [ValidKeywords("host, data source, datasource, address, addr, network address")]
        [RefreshProperties(RefreshProperties.All)]
        public string Server
        {
            get { return values["server"] as string; }
            set { SetValue("server", value); }
        }

        /// <summary>
        /// Gets or sets the name of the database the connection should 
        /// initially connect to.
        /// </summary>
        [Category("Connection")]
        [Description("Database to use initially")]
        [DefaultValue("")]
        [ValidKeywords("initial catalog")]
        [RefreshProperties(RefreshProperties.All)]
        public string Database
        {
            get { return values["database"] as string; }
            set { SetValue("database", value); }
        }

        /// <summary>
        /// Gets or sets the protocol that should be used for communicating
        /// with MySQL.
        /// </summary>
        [Category("Connection")]
        [DisplayName("Connection Protocol")]
        [Description("Protocol to use for connection to MySQL")]
        [DefaultValue(MySqlConnectionProtocol.Sockets)]
        [ValidKeywords("protocol")]
        [RefreshProperties(RefreshProperties.All)]
        public MySqlConnectionProtocol ConnectionProtocol
        {
            get { return (MySqlConnectionProtocol)values["Connection Protocol"]; }
            set { SetValue("Connection Protocol", value); }
        }

        /// <summary>
        /// Gets or sets the name of the named pipe that should be used
        /// for communicating with MySQL.
        /// </summary>
        [Category("Connection")]
        [DisplayName("Pipe Name")]
        [Description("Name of pipe to use when connecting with named pipes (Win32 only)")]
        [DefaultValue("MYSQL")]
        [ValidKeywords("pipe")]
        [RefreshProperties(RefreshProperties.All)]
        public string PipeName
        {
            get { return (string)values["Pipe Name"]; }
            set { SetValue("Pipe Name", value); }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether this connection
        /// should use compression.
        /// </summary>
        [Category("Connection")]
        [DisplayName("Use Compression")]
        [Description("Should the connection ues compression")]
        [DefaultValue(false)]
        [ValidKeywords("compress")]
        [RefreshProperties(RefreshProperties.All)]
        public bool UseCompression
        {
            get { return (bool)values["Use Compression"]; }
            set { SetValue("Use Compression", value); }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether this connection will allow
        /// commands to send multiple SQL statements in one execution.
        /// </summary>
        [Category("Connection")]
        [DisplayName("Allow Batch")]
        [Description("Allows execution of multiple SQL commands in a single statement")]
        [DefaultValue(true)]
        [RefreshProperties(RefreshProperties.All)]
        public bool AllowBatch
        {
            get { return (bool)values["Allow Batch"]; }
            set { SetValue("Allow Batch", value); }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether logging is enabled.
        /// </summary>
        [Category("Connection")]
        [Description("Enables output of diagnostic messages")]
        [DefaultValue(false)]
        [RefreshProperties(RefreshProperties.All)]
        public bool Logging
        {
            get { return (bool)values["Logging"]; }
            set { SetValue("Logging", value); }
        }

        /// <summary>
        /// Gets or sets the base name of the shared memory objects used to 
        /// communicate with MySQL when the shared memory protocol is being used.
        /// </summary>
        [Category("Connection")]
        [DisplayName("Shared Memory Name")]
        [Description("Name of the shared memory object to use")]
        [DefaultValue("MYSQL")]
        [RefreshProperties(RefreshProperties.All)]
        public string SharedMemoryName
        {
            get { return (string)values["Shared Memory Name"]; }
            set { SetValue("Shared Memory Name", value); }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates whether this connection uses
        /// the old style (@) parameter markers or the new (?) style.
        /// </summary>
        [Category("Connection")]
        [DisplayName("Use Old Syntax")]
        [Description("Allows the use of old style @ syntax for parameters")]
        [DefaultValue(false)]
        [ValidKeywords("old syntax, oldsyntax")]
        [RefreshProperties(RefreshProperties.All)]
        [Obsolete("Use Old Syntax is no longer needed.  See documentation")]
        public bool UseOldSyntax
        {
            get { return (bool)values["Use Old Syntax"]; }
            set { SetValue("Use Old Syntax", value); }
        }

        /// <summary>
        /// Gets or sets the port number that is used when the socket
        /// protocol is being used.
        /// </summary>
        [Category("Connection")]
        [Description("Port to use for TCP/IP connections")]
        [DefaultValue(3306)]
        [RefreshProperties(RefreshProperties.All)]
        public uint Port
        {
            get { return (uint)values["Port"]; }
            set { SetValue("Port", value); }
        }

        /// <summary>
        /// Gets or sets the connection timeout.
        /// </summary>
        [Category("Connection")]
        [DisplayName("Connect Timeout")]
        [Description("The length of time (in seconds) to wait for a connection " +
                     "to the server before terminating the attempt and generating an error.")]
        [DefaultValue(15)]
        [ValidKeywords("connection timeout")]
        [RefreshProperties(RefreshProperties.All)]
        public uint ConnectionTimeout
        {
            get { return (uint)values["Connect Timeout"]; }
            
            set 
            {
                // Timeout in milliseconds should not exceed maximum for 32 bit
                // signed integer (~24 days). We truncate the value if it exceeds 
                // maximum (MySqlCommand.CommandTimeout uses the same technique
                uint timeout = Math.Min(value, Int32.MaxValue / 1000);
                if (timeout != value)
                {
                   MySqlTrace.LogWarning(-1, "Connection timeout value too large (" 
                       + value + " seconds). Changed to max. possible value" + 
                       + timeout + " seconds)");
                }
                SetValue("Connect Timeout", timeout); 
            }
        }

        /// <summary>
        /// Gets or sets the default command timeout.
        /// </summary>
        [Category("Connection")]
        [DisplayName("Default Command Timeout")]
        [Description(@"The default timeout that MySqlCommand objects will use
                     unless changed.")]
        [DefaultValue(30)]
        [ValidKeywords("command timeout")]
        [RefreshProperties(RefreshProperties.All)]
        public uint DefaultCommandTimeout
        {
            get { return (uint)values["Default Command Timeout"]; }
            set { SetValue("Default Command Timeout", value); }
        }

        #endregion

        #region Authentication Properties

        /// <summary>
        /// Gets or sets the user id that should be used to connect with.
        /// </summary>
        [Category("Security")]
        [DisplayName("User Id")]
        [Description("Indicates the user ID to be used when connecting to the data source.")]
        [DefaultValue("")]
        [ValidKeywords("uid, username, user name, user")]
        [RefreshProperties(RefreshProperties.All)]
        public string UserID
        {
            get { return (string)values["User Id"]; }
            set { SetValue("User Id", value); }
        }

        /// <summary>
        /// Gets or sets the password that should be used to connect with.
        /// </summary>
        [Category("Security")]
        [Description("Indicates the password to be used when connecting to the data source.")]
        [PasswordPropertyText(true)]
        [DefaultValue("")]
        [ValidKeywords("pwd")]
        [RefreshProperties(RefreshProperties.All)]
        public string Password
        {
            get { return (string)values["Password"]; }
            set { SetValue("Password", value); }
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates if the password should be persisted
        /// in the connection string.
        /// </summary>
        [Category("Security")]
        [DisplayName("Persist Security Info")]
        [Description("When false, security-sensitive information, such as the password, " +
                     "is not returned as part of the connection if the connection is open or " +
                     "has ever been in an open state.")]
        [DefaultValue(false)]
        [RefreshProperties(RefreshProperties.All)]
        public bool PersistSecurityInfo
        {
            get { return (bool)values["Persist Security Info"]; }
            set { SetValue("Persist Security Info", value); }
        }

        [Category("Authentication")]
        [Description("Should the connection use SSL.")]
        [DefaultValue(false)]
        [Obsolete("Use Ssl Mode instead.")]
        internal bool Encrypt
        {
            get { return SslMode != MySqlSslMode.None; }
            set
            {
                SetValue("Ssl Mode", value ? MySqlSslMode.Prefered : MySqlSslMode.None);
            }
        }

        [Category("Authentication")]
        [DisplayName("Certificate File")]
        [Description("Certificate file in PKCS#12 format (.pfx)")]
        [DefaultValue(null)]
        public string CertificateFile
        {
            get { return (string) values["Certificate File"];}
            set
            {
                SetValue("Certificate File", value);
            }
        }

        [Category("Authentication")]
        [DisplayName("Certificate Password")]
        [Description("Password for certificate file")]
        [DefaultValue(null)]
        public string CertificatePassword
        {
            get { return (string)values["Certificate Password"];}
            set
            {
                SetValue("Certificate Password", value);
            }
        }

        [Category("Authentication")]
        [DisplayName("Certificate Store Location")]
        [Description("Certificate Store Location for client certificates")]
        [DefaultValue(MySqlCertificateStoreLocation.None)]
        public MySqlCertificateStoreLocation CertificateStoreLocation
        {
            get { return (MySqlCertificateStoreLocation)values["Certificate Store Location"]; }
            set
            {
                SetValue("Certificate Store Location", value);
            }
        }

        [Category("Authentication")]
        [DisplayName("Certificate Thumbprint")]
        [Description("Certificate thumbprint. Can be used together with Certificate "+ 
            "Store Location parameter to uniquely identify certificate to be used "+
            "for SSL authentication.")]
        [DefaultValue(null)]
        public string CertificateThumbprint
        {
            get { return (string)values["Certificate Thumbprint"]; }
            set
            {
                SetValue("Certificate Thumbprint", value);
            }
        }
        #endregion

        #region Other Properties

        /// <summary>
        /// Gets or sets a boolean value that indicates if zero date time values are supported.
        /// </summary>
        [Category("Advanced")]
        [DisplayName("Allow Zero Datetime")]
        [Description("Should zero datetimes be supported")]
        [DefaultValue(false)]
        [RefreshProperties(RefreshProperties.All)]
        public bool AllowZeroDateTime
        {
            get { return (bool)values["Allow Zero Datetime"]; }
            set { SetValue("Allow Zero DateTime", value); }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating if zero datetime values should be 
        /// converted to DateTime.MinValue.
        /// </summary>
        [Category("Advanced")]
        [DisplayName("Convert Zero Datetime")]
        [Description("Should illegal datetime values be converted to DateTime.MinValue")]
        [DefaultValue(false)]
        [RefreshProperties(RefreshProperties.All)]
        public bool ConvertZeroDateTime
        {
            get { return (bool)values["Convert Zero Datetime"]; }
            set { SetValue("Convert Zero DateTime", value); }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating if the Usage Advisor should be enabled.
        /// </summary>
        [Category("Advanced")]
        [DisplayName("Use Usage Advisor")]
        [Description("Logs inefficient database operations")]
        [DefaultValue(false)]
        [ValidKeywords("usage advisor")]
        [RefreshProperties(RefreshProperties.All)]
        public bool UseUsageAdvisor
        {
            get { return (bool)values["Use Usage Advisor"]; }
            set { SetValue("Use Usage Advisor", value); }
        }

        /// <summary>
        /// Gets or sets the size of the stored procedure cache.
        /// </summary>
        [Category("Advanced")]
        [DisplayName("Procedure Cache Size")]
        [Description("Indicates how many stored procedures can be cached at one time. " +
                     "A value of 0 effectively disables the procedure cache.")]
        [DefaultValue(25)]
        [ValidKeywords("procedure cache, procedurecache")]
        [RefreshProperties(RefreshProperties.All)]
        public uint ProcedureCacheSize
        {
            get { return (uint)values["Procedure Cache Size"]; }
            set { SetValue("Procedure Cache Size", value); }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating if the permon hooks should be enabled.
        /// </summary>
        [Category("Advanced")]
        [DisplayName("Use Performance Monitor")]
        [Description("Indicates that performance counters should be updated during execution.")]
        [DefaultValue(false)]
        [ValidKeywords("userperfmon, perfmon")]
        [RefreshProperties(RefreshProperties.All)]
        public bool UsePerformanceMonitor
        {
            get { return (bool)values["Use Performance Monitor"]; }
            set { SetValue("Use Performance Monitor", value); }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating if calls to Prepare() should be ignored.
        /// </summary>
        [Category("Advanced")]
        [DisplayName("Ignore Prepare")]
        [Description("Instructs the provider to ignore any attempts to prepare a command.")]
        [DefaultValue(true)]
        [RefreshProperties(RefreshProperties.All)]
        public bool IgnorePrepare
        {
            get { return (bool)values["Ignore Prepare"]; }
            set { SetValue("Ignore Prepare", value); }
        }

        [Category("Advanced")]
        [DisplayName("Use Procedure Bodies")]
        [Description("Indicates if stored procedure bodies will be available for parameter detection.")]
        [DefaultValue(true)]
        [ValidKeywords("procedure bodies")]
        [RefreshProperties(RefreshProperties.All)]
        public bool UseProcedureBodies
        {
            get { return (bool)values["Use Procedure Bodies"]; }
            set { SetValue("Use Procedure Bodies", value); }
        }

        [Category("Advanced")]
        [DisplayName("Auto Enlist")]
        [Description("Should the connetion automatically enlist in the active connection, if there are any.")]
        [DefaultValue(true)]
        [RefreshProperties(RefreshProperties.All)]
        public bool AutoEnlist
        {
            get { return (bool)values["Auto Enlist"]; }
            set { SetValue("Auto Enlist", value); }
        }

        [Category("Advanced")]
        [DisplayName("Respect Binary Flags")]
        [Description("Should binary flags on column metadata be respected.")]
        [DefaultValue(true)]
        [RefreshProperties(RefreshProperties.All)]
        public bool RespectBinaryFlags
        {
            get { return (bool)values["Respect Binary Flags"]; }
            set { SetValue("Respect Binary Flags", value); }
        }

        [Category("Advanced")]
        [DisplayName("Treat Tiny As Boolean")]
        [Description("Should the provider treat TINYINT(1) columns as boolean.")]
        [DefaultValue(true)]
        [RefreshProperties(RefreshProperties.All)]
        public bool TreatTinyAsBoolean
        {
            get { return (bool)values["Treat Tiny As Boolean"]; }
            set { SetValue("Treat Tiny As Boolean", value); }
        }

        [Category("Advanced")]
        [DisplayName("Allow User Variables")]
        [Description("Should the provider expect user variables to appear in the SQL.")]
        [DefaultValue(false)]
        [RefreshProperties(RefreshProperties.All)]
        public bool AllowUserVariables
        {
            get { return (bool)values["Allow User Variables"]; }
            set { SetValue("Allow User Variables", value); }
        }

        [Category("Advanced")]
        [DisplayName("Interactive Session")]
        [Description("Should this session be considered interactive?")]
        [DefaultValue(false)]
        [ValidKeywords("interactive")]
        [RefreshProperties(RefreshProperties.All)]
        public bool InteractiveSession
        {
            get { return (bool)values["Interactive Session"]; }
            set { SetValue("Interactive Session", value); }
        }

        [Category("Advanced")]
        [DisplayName("Functions Return String")]
        [Description("Should all server functions be treated as returning string?")]
        [DefaultValue(false)]
        public bool FunctionsReturnString
        {
            get { return (bool)values["Functions Return String"]; }
            set { SetValue("Functions Return String", value); }
        }

        [Category("Advanced")]
        [DisplayName("Use Affected Rows")]
        [Description("Should the returned affected row count reflect affected rows instead of found rows?")]
        [DefaultValue(false)]
        public bool UseAffectedRows
        {
            get { return (bool)values["Use Affected Rows"]; }
            set { SetValue("Use Affected Rows", value); }
        }


        [Category("Advanced")]
        [DisplayName("Old Guids")]
        [Description("Treat binary(16) columns as guids")]
        [DefaultValue(false)]
        public bool OldGuids
        {
            get { return (bool)values["Old Guids"]; }
            set { SetValue("Old Guids", value); }
        }

        [DisplayName("Keep Alive")]
        [Description("For TCP connections, idle connection time measured in seconds, before the first keepalive packet is sent." +
            "A value of 0 indicates that keepalive is not used.")]
        [DefaultValue(0)]
        public uint Keepalive
        {
            get { return (uint)values["Keep Alive"]; }
            set { SetValue("Keep Alive", value); }
        }

        #endregion

        #region Pooling Properties

        /// <summary>
        /// Gets or sets the lifetime of a pooled connection.
        /// </summary>
        [Category("Pooling")]
        [DisplayName("Connection Lifetime")]
        [Description("The minimum amount of time (in seconds) for this connection to " +
                     "live in the pool before being destroyed.")]
        [DefaultValue(0)]
        [RefreshProperties(RefreshProperties.All)]
        public uint ConnectionLifeTime
        {
            get { return (uint)values["Connection LifeTime"]; }
            set { SetValue("Connection LifeTime", value); }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating if connection pooling is enabled.
        /// </summary>
        [Category("Pooling")]
        [Description("When true, the connection object is drawn from the appropriate " +
                     "pool, or if necessary, is created and added to the appropriate pool.")]
        [DefaultValue(true)]
        [RefreshProperties(RefreshProperties.All)]
        public bool Pooling
        {
            get { return (bool)values["Pooling"]; }
            set { SetValue("Pooling", value); }
        }

        /// <summary>
        /// Gets the minimum connection pool size.
        /// </summary>
        [Category("Pooling")]
        [DisplayName("Minimum Pool Size")]
        [Description("The minimum number of connections allowed in the pool.")]
        [DefaultValue(0)]
        [ValidKeywords("min pool size")]
        [RefreshProperties(RefreshProperties.All)]
        public uint MinimumPoolSize
        {
            get { return (uint)values["Minimum Pool Size"]; }
            set { SetValue("Minimum Pool Size", value); }
        }

        /// <summary>
        /// Gets or sets the maximum connection pool setting.
        /// </summary>
        [Category("Pooling")]
        [DisplayName("Maximum Pool Size")]
        [Description("The maximum number of connections allowed in the pool.")]
        [DefaultValue(100)]
        [ValidKeywords("max pool size")]
        [RefreshProperties(RefreshProperties.All)]
        public uint MaximumPoolSize
        {
            get { return (uint)values["Maximum Pool Size"]; }
            set { SetValue("Maximum Pool Size", value); }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating if the connection should be reset when retrieved
        /// from the pool.
        /// </summary>
        [Category("Pooling")]
        [DisplayName("Connection Reset")]
        [Description("When true, indicates the connection state is reset when " +
                     "removed from the pool.")]
        [DefaultValue(false)]
        [RefreshProperties(RefreshProperties.All)]
        public bool ConnectionReset
        {
            get { return (bool)values["Connection Reset"]; }
            set { SetValue("Connection Reset", value); }
        }

        #endregion

        #region Language and Character Set Properties

        /// <summary>
        /// Gets or sets the character set that should be used for sending queries to the server.
        /// </summary>
        [DisplayName("Character Set")]
        [Category("Advanced")]
        [Description("Character set this connection should use")]
        [DefaultValue("")]
        [ValidKeywords("charset")]
        [RefreshProperties(RefreshProperties.All)]
        public string CharacterSet
        {
            get { return (string)values["Character Set"]; }
            set { SetValue("Character Set", value); }
        }

        /// <summary>
        /// Indicates whether the driver should treat binary blobs as UTF8
        /// </summary>
        [DisplayName("Treat Blobs As UTF8")]
        [Category("Advanced")]
        [Description("Should binary blobs be treated as UTF8")]
        [DefaultValue(false)]
        [RefreshProperties(RefreshProperties.All)]
        public bool TreatBlobsAsUTF8
        {
            get { return (bool)values["Treat Blobs As UTF8"]; }
            set { SetValue("Treat Blobs As UTF8", value); }
        }

        /// <summary>
        /// Gets or sets the pattern that matches the columns that should be treated as UTF8
        /// </summary>
        [Category("Advanced")]
        [Description("Pattern that matches columns that should be treated as UTF8")]
        [DefaultValue("")]
        [RefreshProperties(RefreshProperties.All)]
        public string BlobAsUTF8IncludePattern
        {
            get { return (string)values["BlobAsUTF8IncludePattern"]; }
            set { SetValue("BlobAsUTF8IncludePattern", value); }
        }

        /// <summary>
        /// Gets or sets the pattern that matches the columns that should not be treated as UTF8
        /// </summary>
        [Category("Advanced")]
        [Description("Pattern that matches columns that should not be treated as UTF8")]
        [DefaultValue("")]
        [RefreshProperties(RefreshProperties.All)]
        public string BlobAsUTF8ExcludePattern
        {
            get { return (string)values["BlobAsUTF8ExcludePattern"]; }
            set { SetValue("BlobAsUTF8ExcludePattern", value); }
        }

        /// <summary>
        /// Indicates whether to use SSL connections and how to handle server certificate errors.
        /// </summary>
        [DisplayName("Ssl Mode")]
        [Category("Security")]
        [Description("SSL properties for connection")]
        [DefaultValue(MySqlSslMode.None)]
        public MySqlSslMode SslMode
        {
            get { return (MySqlSslMode)values["Ssl Mode"]; }
            set { SetValue("Ssl Mode", value); }
        }

        #endregion

        internal Regex GetBlobAsUTF8IncludeRegex()
        {
            if (String.IsNullOrEmpty(BlobAsUTF8IncludePattern)) return null;
            return new Regex(BlobAsUTF8IncludePattern);
        }

        internal Regex GetBlobAsUTF8ExcludeRegex()
        {
            if (String.IsNullOrEmpty(BlobAsUTF8ExcludePattern)) return null;
            return new Regex(BlobAsUTF8ExcludePattern);
        }

        public override object this[string keyword]
        {
            get { return values[validKeywords[keyword]]; }
            set
            {
                ValidateKeyword(keyword);
                if (value == null)
                    Remove(keyword);
                else
                    SetValue(keyword, value);
            }
        }

        public override void Clear()
        {
            base.Clear();

            // make a copy of our default values array
            foreach (string key in defaultValues.Keys)
                values[key] = defaultValues[key].DefaultValue;
        }

#if !CF

        public override bool Remove(string keyword)
        {
            ValidateKeyword(keyword);
            string primaryKey = validKeywords[keyword];

            values.Remove(primaryKey);
            base.Remove(primaryKey);

            values[primaryKey] = defaultValues[primaryKey].DefaultValue;
            return true;
        }

        public override bool TryGetValue(string keyword, out object value)
        {
            ValidateKeyword(keyword);
            return values.TryGetValue(validKeywords[keyword], out value);
        }

#endif

        public string GetConnectionString(bool includePass)
        {
            if (includePass) return ConnectionString;

            StringBuilder conn = new StringBuilder();
            string delimiter = "";
            foreach (string key in this.Keys)
            {
                if (String.Compare(key, "password", true) == 0 ||
                    String.Compare(key, "pwd", true) == 0) continue;
                conn.AppendFormat(CultureInfo.CurrentCulture, "{0}{1}={2}", 
                    delimiter, key, this[key]);
                delimiter = ";";
            }
            return conn.ToString();
        }

        private void SetValue(string keyword, object value)
        {
            ValidateKeyword(keyword);
            keyword = validKeywords[keyword];

            Remove(keyword);

            object val = null;
            if (value is string && defaultValues[keyword].DefaultValue is Enum)
                val = ParseEnum(defaultValues[keyword].Type, (string)value, keyword);
            else
                val = ChangeType(value, defaultValues[keyword].Type);
            values[keyword] = val;
            base[keyword] = val;
        }

        private object ParseEnum(Type t, string requestedValue, string key)
        {
            try
            {
                return Enum.Parse(t, requestedValue, true);
            }
            catch (ArgumentException)
            {
                throw new InvalidOperationException(String.Format(
                    Resources.InvalidConnectionStringValue, requestedValue, key));
            }
        }

        private object ChangeType(object value, Type t)
        {
            if (t == typeof(bool) && value is string)
            {
                string s = value.ToString().ToLower(CultureInfo.InvariantCulture);
                if (s == "yes" || s == "true") return true;
                if (s == "no" || s == "false") return false;
                throw new FormatException(String.Format(Resources.InvalidValueForBoolean, value));
            }
            else
                return Convert.ChangeType(value, t, CultureInfo.CurrentCulture);
        }

        private void ValidateKeyword(string keyword)
        {
            string key = keyword.ToLower(CultureInfo.InvariantCulture);
            if (!validKeywords.ContainsKey(key))
                throw new ArgumentException(Resources.KeywordNotSupported, keyword);
            if (validKeywords[key] == "Use Old Syntax")
                MySqlTrace.LogWarning(-1, "Use Old Syntax is now obsolete.  Please see documentation");
            if (validKeywords[key] == "Encrypt")
                MySqlTrace.LogWarning(-1, "Encrypt is now obsolete. Use Ssl Mode instead");
        }

        private static void Initialize()
        {
            PropertyInfo[] properties = typeof(MySqlConnectionStringBuilder).GetProperties();
            foreach (PropertyInfo pi in properties)
                AddKeywordFromProperty(pi);

            // remove this starting with 6.4
            PropertyInfo encrypt = typeof(MySqlConnectionStringBuilder).GetProperty(
                "Encrypt", BindingFlags.Instance | BindingFlags.NonPublic);
            AddKeywordFromProperty(encrypt);
        }

        private static void AddKeywordFromProperty(PropertyInfo pi)
        {
            string name = pi.Name.ToLower(CultureInfo.InvariantCulture);
            string displayName = name;

            // now see if we have defined a display name for this property
            object[] attr = pi.GetCustomAttributes(false);
            foreach (Attribute a in attr)
                if (a is DisplayNameAttribute)
                {
                    displayName = (a as DisplayNameAttribute).DisplayName;
                    break;
                }

            validKeywords[name] = displayName;
            validKeywords[displayName] = displayName;

            foreach (Attribute a in attr)
            {
                if (a is ValidKeywordsAttribute)
                {
                    foreach (string keyword in (a as ValidKeywordsAttribute).Keywords)
                        validKeywords[keyword.ToLower(CultureInfo.InvariantCulture).Trim()] = displayName;
                }
                else if (a is DefaultValueAttribute)
                {
                    defaultValues[displayName] = new PropertyDefaultValue(pi.PropertyType, 
                            Convert.ChangeType((a as DefaultValueAttribute).Value, pi.PropertyType, CultureInfo.CurrentCulture));
                }
            }
        }
    }

    internal struct PropertyDefaultValue
    {
        public PropertyDefaultValue(Type t, object v)
        {
            Type = t;
            DefaultValue = v;
        }

        public Type Type;
        public object DefaultValue;
    }

    internal class ValidKeywordsAttribute : Attribute
    {
        private string keywords;

        public ValidKeywordsAttribute(string keywords)
        {
            this.keywords = keywords.ToLower(CultureInfo.InvariantCulture);
        }

        public string[] Keywords
        {
            get { return keywords.Split(','); }
        }
    }
}
