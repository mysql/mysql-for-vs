// Copyright (c) 2012, 2019, Oracle and/or its affiliates. All rights reserved.
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License as
// published by the Free Software Foundation; version 2 of the
// License.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301  USA

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Xml;
using MySql.Data.MySqlClient;
using MySql.Utility.Classes.Attributes;
using MySql.Utility.Classes.Logging;
using MySql.Utility.Enums;
using MySql.Utility.Forms;
using MySql.Utility.Structs;

namespace MySql.Utility.Classes.MySqlWorkbench
{
  /// <summary>
  /// Represents a connection to a MySQL server instance as saved and maintained by MySQL Workbench.
  /// </summary>
  public class MySqlWorkbenchConnection : MySqlWorkbenchConnectionExtraParameters
  {
    #region Constants

    /// <summary>
    /// Default command timeout for connections.
    /// </summary>
    public const uint DEFAULT_COMMAND_TIMEOUT = 30;

    /// <summary>
    /// Default connection timeout for connections.
    /// </summary>
    public const uint DEFAULT_CONNECTION_TIMEOUT = 60;

    /// <summary>
    /// Default database used in connection's host identifier.
    /// </summary>
    public const string DEFAULT_DATABASE_DRIVER_NAME = "Mysql";

    /// <summary>
    /// Default host name for connections.
    /// </summary>
    public const string DEFAULT_HOSTNAME = "localhost";

    /// <summary>
    /// Default connection port.
    /// </summary>
    public const int DEFAULT_PORT = 3306;

    /// <summary>
    /// Default Unix socket or Windows pipe.
    /// </summary>
    public const string DEFAULT_SOCKET_OR_PIPE = ".";

    /// <summary>
    /// Default user name proposed for new connections.
    /// </summary>
    public const string DEFAULT_USERNAME = "root";

    /// <summary>
    /// Localhost IP address.
    /// </summary>
    public const string LOCAL_IP = "127.0.0.1";

    /// <summary>
    /// The <see cref="MySqlException"/> number related to an expired password error.
    /// </summary>
    public const int MYSQL_EXCEPTION_NUMBER_EXPIRED_PASSWORD = 1820;

    /// <summary>
    /// The <see cref="MySqlException"/> number related to an unsuccessful connection to a MySQL Server instance.
    /// </summary>
    public const int MYSQL_EXCEPTION_NUMBER_SERVER_UNREACHABLE = 1042;

    /// <summary>
    /// The <see cref="MySqlException"/> number related to a wrong password error.
    /// </summary>
    public const int MYSQL_EXCEPTION_NUMBER_WRONG_PASSWORD = 0;

    #endregion Constants

    #region Fields

    /// <summary>
    /// An array containing schema names considered system schemas.
    /// </summary>
    private static string[] _systemSchemaNames;

    /// <summary>
    /// The connection method used to connect to the MySQL server instance.
    /// </summary>
    private ConnectionMethodType _connectionMethod;

    /// <summary>
    /// The status of this connection.
    /// </summary>
    private ConnectionStatusType _connectionStatus;

    /// <summary>
    /// The database driver name output in the host identifier (normally Mysql).
    /// </summary>
    private string _databaseDriverName;

    /// <summary>
    /// The default schema for this connection.
    /// </summary>
    private string _defaultSchema;

    /// <summary>
    /// The name of the host (or IP address) of the computer where the MySQL server instance resides.
    /// </summary>
    private string _host;

    /// <summary>
    /// Flag indicating whether the connection uses Windows integrated security.
    /// </summary>
    private bool _integratedSecurity;

    /// <summary>
    /// The password used for this connection.
    /// </summary>
    private string _password;

    /// <summary>
    /// The port used for the connection.
    /// </summary>
    private uint _port;

    /// <summary>
    /// The character set used by the MySQL server to encode text.
    /// </summary>
    private string _serverCharSet;

    /// <summary>
    /// The default collation of the character set used by the MySQL server to encode text.
    /// </summary>
    private string _serverCollation;

    /// <summary>
    /// The version of the connected MySQL server.
    /// </summary>
    private string _serverVersion;

    /// <summary>
    /// The name of the host (or IP address) of the computer the SSH tunnel directs to.
    /// </summary>
    private string _sshHost;

    /// <summary>
    /// The SSH password used for this connection.
    /// </summary>
    private string _sshPassword;

    /// <summary>
    /// The filepath of the private key file used for the SSH tunnel.
    /// </summary>
    private string _sshPrivateKeyFile;

    /// <summary>
    /// The SSH passphrase of the private key file used for the SSH tunnel.
    /// </summary>
    private string _sshPassPhrase;

    /// <summary>
    /// The SSH user name for this connection.
    /// </summary>
    private string _sshUserName;

    /// <summary>
    /// The path to the Certification Authority file for SSL.
    /// </summary>
    private string _sslCertificationAuthorityFile;

    /// <summary>
    /// The path to the Client Certificate file for SSL.
    /// </summary>
    private string _sslClientCertificateFile;

    /// <summary>
    /// The path to the Key file for SSL.
    /// </summary>
    private string _sslKeyFile;

    /// <summary>
    /// The Unix Socket or Windows Pipe name used by this connection.
    /// </summary>
    private string _unixSocketOrWindowsPipe;

    /// <summary>
    /// The user name for this connection.
    /// </summary>
    private string _userName;

    /// <summary>
    /// Value indicating whether SSL ecnryption is used for this connection.
    /// </summary>
    private UseSslType _useSsl;

    #endregion Fields

    /// <summary>
    /// Initializes the <see cref="MySqlWorkbench"/> class.
    /// </summary>
    static MySqlWorkbenchConnection()
    {
      _systemSchemaNames = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlWorkbenchConnection"/> class.
    /// </summary>
    public MySqlWorkbenchConnection()
      : this(Guid.NewGuid().ToString("B"))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlWorkbenchConnection"/> class.
    /// </summary>
    /// <param name="newId">The identifier for the new connection.</param>
    public MySqlWorkbenchConnection(string newId) : base(newId)
    {
      _host = DEFAULT_HOSTNAME;
      _integratedSecurity = false;
      _password = null;
      _port = DEFAULT_PORT;
      _serverCharSet = null;
      _serverCollation = null;
      _serverVersion = null;
      _sshPrivateKeyFile = null;
      _sshPassPhrase = null;
      _sslCertificationAuthorityFile = null;
      _sslClientCertificateFile = null;
      _sslKeyFile = null;
      _sshUserName = null;
      _unixSocketOrWindowsPipe = DEFAULT_SOCKET_OR_PIPE;
      _userName = DEFAULT_USERNAME;
      _useSsl = UseSslType.IfAvailable;
      ConnectionStatus = ConnectionStatusType.Unknown;
      ConnectionTimeout = DEFAULT_CONNECTION_TIMEOUT;
      DatabaseDriverName = DEFAULT_DATABASE_DRIVER_NAME;
      DefaultCommandTimeout = DEFAULT_COMMAND_TIMEOUT;
      Existing = false;
      MigrationStatus = MigrationStatusType.NotMigrated;
      PasswordChanged = false;
      PasswordExpired = false;
      RetrieveRsaKeysForInsecureConnection = false;
      SavedStatus = SavedStatusType.New;
      SshHost = null;
      SshPassword = null;
      SshPasswordChanged = false;
      SshPassPhraseChanged = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlWorkbenchConnection"/> class.
    /// </summary>
    /// <param name="connection">A base <see cref="IDbConnection"/> instance.</param>
    public MySqlWorkbenchConnection(IDbConnection connection)
      : this()
    {
      if (connection == null)
      {
        return;
      }

      var cs = new MySqlConnectionStringBuilder(connection.ConnectionString);
      switch (cs.ConnectionProtocol)
      {
        case MySqlConnectionProtocol.Tcp:
          ConnectionMethod = ConnectionMethodType.Tcp;
          break;

        case MySqlConnectionProtocol.NamedPipe:
          ConnectionMethod = ConnectionMethodType.LocalUnixSocketOrWindowsPipe;
          UnixSocketOrWindowsPipe = cs.PipeName;
          break;

        default:
          ConnectionMethod = ConnectionMethodType.Unknown;
          break;
      }

      if (ConnectionMethod != ConnectionMethodType.LocalUnixSocketOrWindowsPipe)
      {
        Host = cs.Server;
        cs.Port = Port;
      }

      UserName = cs.UserID;
      Password = cs.Password;
      Schema = cs.Database;
      switch (cs.SslMode)
      {
        case MySqlSslMode.None:
          UseSsl = UseSslType.No;
          break;

        case MySqlSslMode.Preferred:
          UseSsl = UseSslType.IfAvailable;
          break;

        case MySqlSslMode.Required:
          UseSsl = UseSslType.Require;
          break;

        case MySqlSslMode.VerifyCA:
          UseSsl = UseSslType.RequireAndVerifyCertificationAuthority;
          break;

        case MySqlSslMode.VerifyFull:
          UseSsl = UseSslType.RequireAndVerifyIdentity;
          break;
      }

      if (cs.SslMode > MySqlSslMode.Required)
      {
        SslCertificationAuthorityFile = cs.SslCa;
        SslClientCertificateFile = cs.SslCert;
        SslKeyFile = cs.SslKey;
      }

      UseCompression = cs.UseCompression;
      AllowZeroDateTimeValues = cs.AllowZeroDateTime;
      IntegratedSecurity = cs.IntegratedSecurity;
      DefaultCommandTimeout = cs.DefaultCommandTimeout;
      ConnectionTimeout = cs.ConnectionTimeout;
      TreatTinyIntAsBoolean = cs.TreatTinyAsBoolean;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlWorkbenchConnection"/> class.
    /// </summary>
    /// <param name="workbenchConnectionElement">XML element representing a serialized <see cref="MySqlWorkbenchConnection"/>.</param>
    /// <param name="useMySqlWorkbenchConnectionsFile">Flag indicating whether the MySQL Workbench connections file is the one used for this collection.</param>
    internal MySqlWorkbenchConnection(XmlElement workbenchConnectionElement, bool useMySqlWorkbenchConnectionsFile)
      : this()
    {
      if (workbenchConnectionElement == null)
      {
        return;
      }

      Id = workbenchConnectionElement.Attributes["id"].Value;
      SavedStatus = SavedStatusType.InDisk;
      foreach (XmlElement childEl in workbenchConnectionElement.ChildNodes)
      {
        ProcessElement(childEl);
      }

      // Attempt to retrieve the MySQL password from the secured password vault
      string storedPassword = MySqlWorkbenchPasswordVault.FindPassword(useMySqlWorkbenchConnectionsFile, HostIdentifier, UserName);
      if (!string.IsNullOrEmpty(storedPassword))
      {
        _password = storedPassword;
      }

      // Attempt to retrieve the SSH password from the secured password vault
      storedPassword = MySqlWorkbenchPasswordVault.FindPassword(useMySqlWorkbenchConnectionsFile, SshHostIdentifier, SshUserName);
      if (!string.IsNullOrEmpty(storedPassword))
      {
        _sshPassword = storedPassword;
      }

      // Attempt to retrieve the SSH private key file pass phrase from the secured password vault
      storedPassword = MySqlWorkbenchPasswordVault.FindPassword(useMySqlWorkbenchConnectionsFile, SshHostIdentifier, SshPrivateKeyFile);
      if (!string.IsNullOrEmpty(storedPassword))
      {
        _sshPassPhrase = storedPassword;
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlWorkbenchConnection"/> class.
    /// </summary>
    /// <param name="workbenchConnectionElement">XML element representing a serialized <see cref="MySqlWorkbenchConnection"/>.</param>
    /// <param name="extraParameters">A <see cref="MySqlWorkbenchConnectionExtraParameters"/> containing extra parameters.</param>
    /// <param name="useMySqlWorkbenchConnectionsFile">Flag indicating whether the MySQL Workbench connections file is the one used for this collection.</param>
    internal MySqlWorkbenchConnection(XmlElement workbenchConnectionElement, MySqlWorkbenchConnectionExtraParameters extraParameters, bool useMySqlWorkbenchConnectionsFile)
      : this(workbenchConnectionElement, useMySqlWorkbenchConnectionsFile)
    {
      var connName = Name;
      Sync(extraParameters);

      // Reassign the value of the Name property in case the extraParameters.Name is not identical to the connection name.
      if (!Name.Equals(connName, StringComparison.InvariantCulture))
      {
        Name = connName;
      }
    }

    #region Enumerations

    /// <summary>
    /// Specifies the connection methods used for Workbench connections.
    /// </summary>
    public enum ConnectionMethodType
    {
      /// <summary>
      /// Standard TCP/IP connection.
      /// </summary>
      [Description("Standard (TCP/IP)"), SupportedBy("*")]
      Tcp = 0,

      /// <summary>
      /// Local Unix Socket or Windows Pipe connection.
      /// </summary>
      [Description("Local Socket/Pipe"), SupportedBy("*")]
      LocalUnixSocketOrWindowsPipe = 1,

      /// <summary>
      /// TCP/IP over SSH tunneled connection.
      /// </summary>
      [Description("Standard TCP/IP over SSH"), SupportedBy("*")]
      Ssh = 2,

      /// <summary>
      /// MySQL X Protocol
      /// </summary>
      [Description("X Protocol"), SupportedBy("VisualStudio")]
      XProtocol = 3,

      /// <summary>
      /// Unknown type.
      /// </summary>
      [Description("Unknown")]
      Unknown = 4
    }

    /// <summary>
    /// Specifies the connection status of a connection.
    /// </summary>
    public enum ConnectionStatusType
    {
      /// <summary>
      /// MySQL instance of this connection is accepting connections.
      /// </summary>
      AcceptingConnections = 0,

      /// <summary>
      /// MySQL instance of this connection is refusing connections.
      /// </summary>
      RefusingConnections = 1,

      /// <summary>
      /// Connection has not been tested, so status is not available.
      /// </summary>
      Unknown = 2
    }

    /// <summary>
    /// Specifies identifiers for the status of a connection migrated to the Workbench connections file.
    /// </summary>
    public enum MigrationStatusType
    {
      /// <summary>
      /// An identical connection already exists in the Workbench connections file, so it was not migrated.
      /// </summary>
      AlreadyExists,

      /// <summary>
      /// The connection was successfully migrated exactly as it was created in the external application's file.
      /// </summary>
      MigratedAsIs,

      /// <summary>
      /// The connection has not been migrated.
      /// </summary>
      NotMigrated,

      /// <summary>
      /// A connection with the same name (but not identical) already exists in the Workbench connections file, so it was renamed before the migration.
      /// </summary>
      RenamedAndMigrated,
    }

    /// <summary>
    /// Specifies the saved status of a connection.
    /// </summary>
    public enum SavedStatusType
    {
      /// <summary>
      /// The connection has just been created and has not been saved in disk.
      /// </summary>
      New,

      /// <summary>
      /// The connection has been loaded from disk or recently saved to disk.
      /// </summary>
      InDisk,

      /// <summary>
      /// The connection has been changed and changes have not been saved to disk.
      /// </summary>
      Updated
    }

    /// <summary>
    /// Specifies the different values for using SSL encryption.
    /// </summary>
    public enum UseSslType
    {
      /// <summary>
      /// Use regular non-SSL connection.
      /// </summary>
      [Description("No")]
      No,

      /// <summary>
      /// Use SSL encryption if available at the server, otherwise fallback to a regular connection.
      /// </summary>
      [Description("If available")]
      IfAvailable,

      /// <summary>
      /// Force the use of SSL encryption, if not available the connection fails.
      /// </summary>
      [Description("Require")]
      Require,

      /// <summary>
      /// Force the use of SSL encryption, and verifies the CA.
      /// </summary>
      [Description("Require and Verify CA")]
      RequireAndVerifyCertificationAuthority,

      /// <summary>
      /// Force the use of SSL encryption, and verifies identity.
      /// </summary>
      [Description("Require and Verify Identity")]
      RequireAndVerifyIdentity
    }

    #endregion Enumerations

    #region Static Properties

    /// <summary>
    /// Gets the MySQL Workbench connections file path.
    /// </summary>
    public static string ConnectionsFilePath => MySqlWorkbench.ConnectionsFilePath;

    /// <summary>
    /// Gets an array containing schema names considered system schemas.
    /// </summary>
    public static string[] SystemSchemaNames => _systemSchemaNames ??
                                                (_systemSchemaNames = new[] {"mysql", "information_schema", "performance_schema", "sys"});

    #endregion Static Properties

    #region Properties

    /// <summary>
    /// Gets or sets a boolean value that indicates if user variables are supported.
    /// </summary>
    public bool AllowUserVariables { get; set; }

    /// <summary>
    /// Gets or sets a boolean value that indicates if zero date time values are supported.
    /// </summary>
    public bool AllowZeroDateTimeValues { get; set; }

    /// <summary>
    /// Gets or sets the character set used to encode all queries sent to the server.
    /// </summary>
    public string CharacterSet { get; set; }

    /// <summary>
    /// Gets or sets the connection method used to connect to the MySQL server instance.
    /// </summary>
    public ConnectionMethodType ConnectionMethod
    {
      get => _connectionMethod;
      set
      {
        if (_connectionMethod == value)
        {
          return;
        }

        _connectionMethod = value;
        NotifyPropertyChanged("ConnectionMethod");
      }
    }

    /// <summary>
    /// Gets or sets the status of this connection.
    /// </summary>
    public ConnectionStatusType ConnectionStatus
    {
      get => _connectionStatus;
      private set
      {
        _connectionStatus = value;
        NotifyPropertyChanged("ConnectionStatus");
      }
    }

    /// <summary>
    /// Gets a summary text for connection information displayed on clients.
    /// </summary>
    public string DisplayConnectionSummaryText { get; private set; }

    /// <summary>
    /// Gets a description of the status of this connection.
    /// </summary>
    public string ConnectionStatusText => GetConnectionStatusDisplayText(ConnectionStatus);

    /// <summary>
    /// Gets the MySQL connection string produced from this instance.
    /// </summary>
    public string ConnectionString => GetConnectionStringBuilder().ConnectionString;

    /// <summary>
    /// Gets or sets the length of time (in seconds) to wait for a connection to the server before terminating the attempt and generating an error.
    /// </summary>
    public uint ConnectionTimeout { get; set; }

    /// <summary>
    /// Gets or sets the database driver name output in the host identifier (normally Mysql).
    /// </summary>
    public string DatabaseDriverName
    {
      get => _databaseDriverName;
      set
      {
        _databaseDriverName = string.IsNullOrEmpty(value) ? DEFAULT_DATABASE_DRIVER_NAME : value;
        SetHostIdentifier();
        NotifyPropertyChanged("DatabaseDriverName");
      }
    }

    /// <summary>
    /// Gets or sets the default timeout that MySqlCommand objects will use unless changed.
    /// </summary>
    public uint DefaultCommandTimeout { get; set; }

    /// <summary>
    /// Gets or sets the name of the default schema stored with this connection.
    /// </summary>
    public string DefaultSchema
    {
      get => _defaultSchema;
      set => _defaultSchema = Schema = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this connection already exists in a client program.
    /// </summary>
    public bool Existing { get; set; }

    /// <summary>
    /// Gets or sets the name of the host (or IP address) of the computer where the MySQL server instance resides.
    /// </summary>
    public string Host
    {
      get => _host;
      set
      {
        if (string.Equals(_host, value, StringComparison.InvariantCultureIgnoreCase))
        {
          return;
        }

        _host = IsNamedPipesConnection || value == string.Empty
          ? DEFAULT_HOSTNAME
          : value;
        SetHostIdentifier();
        NotifyPropertyChanged("Host");
      }
    }

    /// <summary>
    /// Gets the host identifier describing where the MySQL server instance can be reached at.
    /// </summary>
    public string HostIdentifier { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether the connection uses Windows integrated security.
    /// </summary>
    public bool IntegratedSecurity
    {
      get => _integratedSecurity;
      set
      {
        if (_integratedSecurity == value)
        {
          return;
        }

        _integratedSecurity = value;
        NotifyPropertyChanged("IntegratedSecurity");
      }
    }

    /// <summary>
    /// Gets a value indicating whether the connection is local.
    /// </summary>
    public bool IsLocalConnection => IsHostLocal(IsSshConnection ? SshHost : Host);

    /// <summary>
    /// Gets a value indicating whether the connection type is Named Pipes/Socket.
    /// </summary>
    public bool IsNamedPipesConnection => ConnectionMethod == ConnectionMethodType.LocalUnixSocketOrWindowsPipe;

    /// <summary>
    /// Gets a value indicating whether the connection type is standard TCP/IP over a SSH tunnel.
    /// </summary>
    public bool IsSshConnection => ConnectionMethod == ConnectionMethodType.Ssh;

    /// <summary>
    /// Gets a value indicating whether the connection type is standard TCP/IP.
    /// </summary>
    public bool IsTcpConnection => ConnectionMethod == ConnectionMethodType.Tcp;

    /// <summary>
    /// Gets a value indicating whether the connection type is a new one, unknown to the current version.
    /// </summary>
    public bool IsUnknownConnection => ConnectionMethod == ConnectionMethodType.Unknown;

    /// <summary>
    /// Gets a value indicating whether the connection type is for the X Protocol.
    /// </summary>
    public bool IsXConnection => ConnectionMethod == ConnectionMethodType.XProtocol;

    /// <summary>
    /// Gets the status of the migration to the connection to a Workbench connections file.
    /// </summary>
    public MigrationStatusType MigrationStatus { get; internal set; }

    /// <summary>
    /// Gets or sets the password used for this connection.
    /// </summary>
    public string Password
    {
      get => _password;
      set
      {
        if (string.Equals(_password, value, StringComparison.InvariantCulture))
        {
          return;
        }

        _password = value;
        PasswordChanged = true;
        NotifyPropertyChanged("Password");
      }
    }

    /// <summary>
    /// Gets a value indicating whether the password was changed by the user so it needs to be saved back to the password vault.
    /// </summary>
    public bool PasswordChanged { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether the password has expired and the user has to change it before running statements on the server.
    /// </summary>
    public bool PasswordExpired { get; private set; }

    /// <summary>
    /// Gets or sets the port used for the connection.
    /// </summary>
    public uint Port
    {
      get => _port;
      set
      {
        if (_port == value)
        {
          return;
        }

        _port = ConnectionMethod != ConnectionMethodType.LocalUnixSocketOrWindowsPipe ? value : DEFAULT_PORT;
        SetHostIdentifier();
        NotifyPropertyChanged("Port");
      }
    }

    /// <summary>
    /// Gets a value indicating whether the Server is using SSL (secure connections) but the client is not using SSL so RSA keys need to be retrieved from the server to encrypt the password.
    /// </summary>
    public bool RetrieveRsaKeysForInsecureConnection { get; private set; }

    /// <summary>
    /// Gets or sets the name of the schema currently used by this connection.
    /// </summary>
    public string Schema { get; set; }

    /// <summary>
    /// Gets the character set used by the MySQL server to encode text.
    /// </summary>
    public string ServerCharSet
    {
      get
      {
        if (!string.IsNullOrEmpty(_serverCharSet))
        {
          return _serverCharSet;
        }

        var serverCharSetAndCollation = GetMySqlServerCharSetAndCollation();
        _serverCharSet = serverCharSetAndCollation?.Item1;
        _serverCollation = serverCharSetAndCollation?.Item2;
        return _serverCharSet;
      }
    }

    /// <summary>
    /// Gets the default collation of the character set used by the MySQL server to encode text.
    /// </summary>
    public string ServerCollation
    {
      get
      {
        if (!string.IsNullOrEmpty(_serverCollation))
        {
          return _serverCollation;
        }

        var serverCharSetAndCollation = GetMySqlServerCharSetAndCollation();
        _serverCharSet = serverCharSetAndCollation?.Item1;
        _serverCollation = serverCharSetAndCollation?.Item2;
        return _serverCollation;
      }
    }

    /// <summary>
    /// Gets the version of the connected MySQL server.
    /// </summary>
    public string ServerVersion
    {
      get
      {
        if (string.IsNullOrEmpty(_serverVersion))
        {
          _serverVersion = GetMySqlServerVersion();
        }

        return _serverVersion;
      }
    }

    /// <summary>
    /// Gets or sets the name of the SSH host (or IP address) of the computer the SSH tunnel directs to.
    /// </summary>
    public string SshHost
    {
      get => _sshHost;
      set
      {
        if (string.Equals(_sshHost, value, StringComparison.InvariantCultureIgnoreCase))
        {
          return;
        }

        _sshHost = value;
        SetHostIdentifier();
        NotifyPropertyChanged("SshHost");
      }
    }

    /// <summary>
    /// Gets the host identifier describing where the SSH computer is reached at.
    /// </summary>
    public string SshHostIdentifier => "ssh@" + SshHost;

    /// <summary>
    /// Gets or sets the SSH password used for this connection.
    /// </summary>
    public string SshPassword
    {
      get => _sshPassword;
      set
      {
        if (string.Equals(_sshPassword, value, StringComparison.InvariantCulture))
        {
          return;
        }

        _sshPassword = value;
        SshPasswordChanged = true;
        NotifyPropertyChanged("SshPassword");
      }
    }

    /// <summary>
    /// Gets a value indicating whether the SSH password was changed by the user so it needs to be saved back to the password vault.
    /// </summary>
    public bool SshPasswordChanged { get; private set; }

    /// <summary>
    /// Gets or sets the filepath of the private key file used for the SSH tunnel.
    /// </summary>
    public string SshPrivateKeyFile
    {
      get => _sshPrivateKeyFile;
      set
      {
        if (string.Equals(_sshPrivateKeyFile, value, StringComparison.InvariantCultureIgnoreCase))
        {
          return;
        }

        _sshPrivateKeyFile = value;
        NotifyPropertyChanged("SshPrivateKeyFile");
      }
    }

    /// <summary>
    /// Gets or sets the SSH pass phrase used for this connection.
    /// </summary>
    public string SshPassPhrase
    {
      get => _sshPassPhrase;
      set
      {
        if (string.Equals(_sshPassPhrase, value, StringComparison.InvariantCulture))
        {
          return;
        }

        _sshPassPhrase = value;
        SshPassPhraseChanged = true;
        NotifyPropertyChanged("SshPassPhrase");
      }
    }

    /// <summary>
    /// Gets a value indicating whether the SSH pass phrase was changed by the user so it needs to be saved back to the password vault.
    /// </summary>
    public bool SshPassPhraseChanged { get; private set; }

    /// <summary>
    /// Gets or sets the SSH user name for this connection.
    /// </summary>
    public string SshUserName
    {
      get => _sshUserName;
      set
      {
        if (string.Equals(_sshUserName, value, StringComparison.InvariantCulture))
        {
          return;
        }

        _sshUserName = value;
        SetHostIdentifier();
        NotifyPropertyChanged("SshUserName");
      }
    }

    /// <summary>
    /// Gets or sets the path to the Certification Authority file for SSL.
    /// </summary>
    public string SslCertificationAuthorityFile
    {
      get => _sslCertificationAuthorityFile;
      set
      {
        if (string.Equals(_sslCertificationAuthorityFile, value, StringComparison.InvariantCultureIgnoreCase))
        {
          return;
        }

        _sslCertificationAuthorityFile = value;
        NotifyPropertyChanged("SslCertificationAuthorityFile");
      }
    }

    /// <summary>
    /// Gets or sets the path to the Client Certificate file for SSL.
    /// </summary>
    public string SslClientCertificateFile
    {
      get => _sslClientCertificateFile;
      set
      {
        if (string.Equals(_sslClientCertificateFile, value, StringComparison.InvariantCultureIgnoreCase))
        {
          return;
        }

        _sslClientCertificateFile = value;
        NotifyPropertyChanged("SslClientCertificateFile");
      }
    }

    /// <summary>
    /// Gets or sets the path to the Key file for SSL.
    /// </summary>
    public string SslKeyFile
    {
      get => _sslKeyFile;
      set
      {
        if (string.Equals(_sslKeyFile, value, StringComparison.InvariantCultureIgnoreCase))
        {
          return;
        }

        _sslKeyFile = value;
        NotifyPropertyChanged("SslKeyFile");
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether tinyint values are automatically converted to boolean.
    /// </summary>
    public bool TreatTinyIntAsBoolean { get; set; }

    /// <summary>
    /// Gets or sets the Unix Socket or Windows Pipe name used by this connection.
    /// </summary>
    public string UnixSocketOrWindowsPipe
    {
      get => _unixSocketOrWindowsPipe;
      set
      {
        if (string.Equals(_unixSocketOrWindowsPipe, value, StringComparison.InvariantCulture))
        {
          return;
        }

        _unixSocketOrWindowsPipe = value == string.Empty ? "." : value;
        SetHostIdentifier();
        NotifyPropertyChanged("UnixSocketOrWindowsPipe");
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether ANSI quotes are used.
    /// </summary>
    public bool UseAnsiQuotes { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the connection uses compression.
    /// </summary>
    public bool UseCompression { get; set; }

    /// <summary>
    /// Gets or sets the user name for this connection.
    /// </summary>
    public string UserName
    {
      get => _userName;
      set
      {
        if (string.Equals(_userName, value, StringComparison.InvariantCulture))
        {
          return;
        }

        _userName = value;
        NotifyPropertyChanged("UserName");
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether SSL encryption is used for this connection.
    /// </summary>
    public UseSslType UseSsl
    {
      get => _useSsl;
      set
      {
        if (_useSsl == value)
        {
          return;
        }

        _useSsl = value;
        NotifyPropertyChanged("UseSsl");
      }
    }

    #endregion Properties

    /// <summary>
    /// Gets a connection status descriptive text based on a connection status.
    /// </summary>
    /// <param name="connectionStatus">The connection status.</param>
    /// <returns>The descriptive text based on a connection status.</returns>
    public static string GetConnectionStatusDisplayText(ConnectionStatusType connectionStatus)
    {
      string statusText = string.Empty;
      switch (connectionStatus)
      {
        case ConnectionStatusType.AcceptingConnections:
          statusText = Resources.ConnectionStatusGood;
          break;

        case ConnectionStatusType.RefusingConnections:
          statusText = Resources.ConnectionStatusBad;
          break;

        case ConnectionStatusType.Unknown:
          statusText = Resources.ConnectionStatusNA;
          break;
      }

      return statusText;
    }

    /// <summary>
    /// Verifies if a given hostname represents a local connection.
    /// </summary>
    /// <param name="hostName">Name of the host to connect to.</param>
    /// <returns><c>true</c> if the hostname represents a local connection, <c>false</c> otherwise.</returns>
    public static bool IsHostLocal(string hostName)
    {
      if (string.IsNullOrEmpty(hostName))
      {
        return true;
      }

      var colonPosition = hostName.LastIndexOf(':');
      hostName = colonPosition >= 0
        ? hostName.Split(':')[0]
        : hostName;
      return hostName.ToLowerInvariant() == DEFAULT_HOSTNAME || hostName == LOCAL_IP;
    }

    public static bool operator !=(MySqlWorkbenchConnection left, MySqlWorkbenchConnection right)
    {
      return !Equals(left, right);
    }

    public static bool operator ==(MySqlWorkbenchConnection left, MySqlWorkbenchConnection right)
    {
      return Equals(left, right);
    }

    /// <summary>
    /// Clones the connection into a new <see cref="MySqlWorkbenchConnection"/> object.
    /// </summary>
    /// <param name="savedStatus">Indicates the saved status type for the cloned connection.</param>
    /// <param name="cloneId">Flag indicating whether the connection id should be cloned as well, otherwise a new one will be generated.</param>
    /// <returns>The cloned object.</returns>
    public MySqlWorkbenchConnection Clone(SavedStatusType savedStatus, bool cloneId)
    {
      var c = cloneId ? new MySqlWorkbenchConnection(Id) : new MySqlWorkbenchConnection();
      c.AllowUserVariables = AllowUserVariables;
      c.AllowZeroDateTimeValues = AllowZeroDateTimeValues;
      c.CharacterSet = CharacterSet;
      c.ConnectionMethod = ConnectionMethod;
      c.ConnectionStatus = ConnectionStatus;
      c.ConnectionTimeout = ConnectionTimeout;
      c.DatabaseDriverName = DatabaseDriverName;
      c.DefaultCommandTimeout = DefaultCommandTimeout;
      c.DefaultSchema = DefaultSchema;
      c.Existing = Existing;
      c.Host = Host;
      c.IntegratedSecurity = IntegratedSecurity;
      c.Name = Name;
      c.Password = Password;
      c.Port = Port;
      c.SavedStatus = savedStatus;
      c.Schema = Schema;
      c.SshHost = SshHost;
      c.SshPassword = SshPassword;
      c.SshPrivateKeyFile = SshPrivateKeyFile;
      c.SshPassPhrase = SshPassPhrase;
      c.SshUserName = SshUserName;
      c.SslCertificationAuthorityFile = SslCertificationAuthorityFile;
      c.SslClientCertificateFile = SslClientCertificateFile;
      c.SslKeyFile = SslKeyFile;
      c.TreatTinyIntAsBoolean = TreatTinyIntAsBoolean;
      c.UnixSocketOrWindowsPipe = UnixSocketOrWindowsPipe;
      c.UseAnsiQuotes = UseAnsiQuotes;
      c.UseCompression = UseCompression;
      c.UserName = UserName;
      c.UseSsl = UseSsl;
      return c;
    }

    /// <summary>
    ///  Determines whether two <see cref="object"/> are equal.
    /// </summary>
    /// <param name="obj">An <see cref="object"/> to compare to this one.</param>
    /// <returns><c>true</c> if the specified <see cref="object"/> is equal to the current <see cref="object"/>, <c>false</c> otherwise.</returns>
    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj))
      {
        return false;
      }

      if (ReferenceEquals(this, obj))
      {
        return true;
      }

      return obj.GetType() == typeof(MySqlWorkbenchConnection) && Equals((MySqlWorkbenchConnection)obj);
    }

    /// <summary>
    /// Gets a list of all MySQL character sets along with their available collations.
    /// </summary>
    /// <param name="firstElement">A custom string for the first element of the dictionary.</param>
    /// <returns>A list of all MySQL character sets along with their available collations.</returns>
    public Dictionary<string, string[]> GetCollationsDictionary(string firstElement = null)
    {
      return Utilities.GetCollationsDictionary(ConnectionString, firstElement);
    }

    /// <summary>
    /// Gets the connection string builder for this Workbench connection.
    /// </summary>
    /// <returns>The connection string builder object for this Workbench connection.</returns>
    public MySqlConnectionStringBuilder GetConnectionStringBuilder()
    {
      var cs = new MySqlConnectionStringBuilder();
      switch (ConnectionMethod)
      {
        case ConnectionMethodType.Tcp:
        case ConnectionMethodType.XProtocol:
          cs.ConnectionProtocol = MySqlConnectionProtocol.Tcp;
          cs.Server = Host;
          cs.Port = Port;
          break;

        case ConnectionMethodType.LocalUnixSocketOrWindowsPipe:
          cs.ConnectionProtocol = MySqlConnectionProtocol.Pipe;
          cs.PipeName = UnixSocketOrWindowsPipe;
          break;

        case ConnectionMethodType.Ssh:
          cs.ConnectionProtocol = MySqlConnectionProtocol.Tcp;
          cs.Server = Host;
          cs.Port = Port;
          cs.SshUserName = SshUserName;
          cs.SshKeyFile = SshPrivateKeyFile;
          var index = SshHost?.LastIndexOf(':') ?? -1;
          if (SshHost != null && index >= 0)
          {
            cs.SshHostName = SshHost.Substring(0, index);
            cs.SshPort = Convert.ToUInt32(SshHost.Substring(index + 1));
          }
          else
          {
            cs.SshHostName = SshHost;
          }

          if (!string.IsNullOrEmpty(SshPassword))
          {
            cs.SshPassword = SshPassword;
          }

          if (!string.IsNullOrEmpty(SshPassPhrase))
          {
            cs.SshPassphrase = SshPassPhrase;
          }

          break;
      }

      cs.UserID = UserName;
      if (!string.IsNullOrEmpty(Password))
      {
        cs.Password = Password;
        cs.PersistSecurityInfo = true;
      }

      if (!string.IsNullOrEmpty(Schema))
      {
        cs.Database = Schema;
      }

      switch (UseSsl)
      {
        case UseSslType.No:
          cs.SslMode = MySqlSslMode.None;
          if (RetrieveRsaKeysForInsecureConnection)
          {
            cs.AllowPublicKeyRetrieval = true;
          }
          break;

        case UseSslType.IfAvailable:
          cs.SslMode = MySqlSslMode.Preferred;
          break;

        case UseSslType.Require:
          cs.SslMode = MySqlSslMode.Required;
          break;

        case UseSslType.RequireAndVerifyCertificationAuthority:
          cs.SslMode = MySqlSslMode.VerifyCA;
          break;

        case UseSslType.RequireAndVerifyIdentity:
          cs.SslMode = MySqlSslMode.VerifyFull;
          break;
      }

      if (cs.SslMode > MySqlSslMode.Required)
      {
        if (!string.IsNullOrEmpty(SslCertificationAuthorityFile))
        {
          cs.SslCa = SslCertificationAuthorityFile;
        }

        if (!string.IsNullOrEmpty(SslClientCertificateFile))
        {
          cs.SslCert = SslClientCertificateFile;
        }

        if (!string.IsNullOrEmpty(SslKeyFile))
        {
          cs.SslKey = SslKeyFile;
        }
      }

      cs.UseCompression = UseCompression;
      cs.AllowUserVariables = AllowUserVariables;
      cs.AllowZeroDateTime = AllowZeroDateTimeValues;
      cs.CharacterSet = CharacterSet;
      cs.IntegratedSecurity = IntegratedSecurity;
      cs.TreatTinyAsBoolean = TreatTinyIntAsBoolean;
      cs.DefaultCommandTimeout = DefaultCommandTimeout;
      cs.ConnectionTimeout = ConnectionTimeout;
      return cs;
    }

    /// <summary>
    /// Gets the default collation corresponding to the given character set name.
    /// </summary>
    /// <param name="charSet"></param>
    /// <returns>The default collation corresponding to the given character set name.</returns>
    public string GetDefaultCollationFromCharSet(string charSet)
    {
      return Utilities.GetDefaultCollationFromCharSet(ConnectionString, charSet);
    }

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
      // Arbitrary number to generate the hash code.
      const int HASH_CODE_MULTIPLIER = 397;
      unchecked
      {
        int hashCode = AllowUserVariables.GetHashCode();
        hashCode = (hashCode * HASH_CODE_MULTIPLIER) ^ AllowZeroDateTimeValues.GetHashCode();
        hashCode = (hashCode * HASH_CODE_MULTIPLIER) ^ (CharacterSet != null ? CharacterSet.GetHashCode() : 0);
        hashCode = (hashCode * HASH_CODE_MULTIPLIER) ^ ConnectionMethod.GetHashCode();
        hashCode = (hashCode * HASH_CODE_MULTIPLIER) ^ ConnectionStatus.GetHashCode();
        hashCode = (hashCode * HASH_CODE_MULTIPLIER) ^ (int)ConnectionTimeout;
        hashCode = (hashCode * HASH_CODE_MULTIPLIER) ^ (DatabaseDriverName != null ? DatabaseDriverName.GetHashCode() : 0);
        hashCode = (hashCode * HASH_CODE_MULTIPLIER) ^ (int)DefaultCommandTimeout;
        hashCode = (hashCode * HASH_CODE_MULTIPLIER) ^ (DefaultSchema != null ? DefaultSchema.GetHashCode() : 0);
        hashCode = (hashCode * HASH_CODE_MULTIPLIER) ^ Existing.GetHashCode();
        hashCode = (hashCode * HASH_CODE_MULTIPLIER) ^ (Host != null ? Host.GetHashCode() : 0);
        hashCode = (hashCode * HASH_CODE_MULTIPLIER) ^ IntegratedSecurity.GetHashCode();
        hashCode = (hashCode * HASH_CODE_MULTIPLIER) ^ (Name != null ? Name.GetHashCode() : 0);
        hashCode = (hashCode * HASH_CODE_MULTIPLIER) ^ (Password != null ? Password.GetHashCode() : 0);
        hashCode = (hashCode * HASH_CODE_MULTIPLIER) ^ (int)Port;
        hashCode = (hashCode * HASH_CODE_MULTIPLIER) ^ (Schema != null ? Schema.GetHashCode() : 0);
        hashCode = (hashCode * HASH_CODE_MULTIPLIER) ^ (SshHost != null ? SshHost.GetHashCode() : 0);
        hashCode = (hashCode * HASH_CODE_MULTIPLIER) ^ (SshPassword != null ? SshPassword.GetHashCode() : 0);
        hashCode = (hashCode * HASH_CODE_MULTIPLIER) ^ (SshPassPhrase != null ? SshPassPhrase.GetHashCode() : 0);
        hashCode = (hashCode * HASH_CODE_MULTIPLIER) ^ (SshPrivateKeyFile != null ? SshPrivateKeyFile.GetHashCode() : 0);
        hashCode = (hashCode * HASH_CODE_MULTIPLIER) ^ (SshUserName != null ? SshUserName.GetHashCode() : 0);
        hashCode = (hashCode * HASH_CODE_MULTIPLIER) ^ (SslCertificationAuthorityFile != null ? SslCertificationAuthorityFile.GetHashCode() : 0);
        hashCode = (hashCode * HASH_CODE_MULTIPLIER) ^ (SslClientCertificateFile != null ? SslClientCertificateFile.GetHashCode() : 0);
        hashCode = (hashCode * HASH_CODE_MULTIPLIER) ^ (SslKeyFile != null ? SslKeyFile.GetHashCode() : 0);
        hashCode = (hashCode * HASH_CODE_MULTIPLIER) ^ TreatTinyIntAsBoolean.GetHashCode();
        hashCode = (hashCode * HASH_CODE_MULTIPLIER) ^ (UnixSocketOrWindowsPipe != null ? UnixSocketOrWindowsPipe.GetHashCode() : 0);
        hashCode = (hashCode * HASH_CODE_MULTIPLIER) ^ UseAnsiQuotes.GetHashCode();
        hashCode = (hashCode * HASH_CODE_MULTIPLIER) ^ UseCompression.GetHashCode();
        hashCode = (hashCode * HASH_CODE_MULTIPLIER) ^ (UserName != null ? UserName.GetHashCode() : 0);
        hashCode = (hashCode * HASH_CODE_MULTIPLIER) ^ UseSsl.GetHashCode();
        return hashCode;
      }
    }

    /// <summary>
    /// Gets the character set and its collation used by the connected MySQL server.
    /// </summary>
    /// <returns>The character set and its collation used by the connected MySQL server.</returns>
    public Tuple<string, string> GetMySqlServerCharSetAndCollation()
    {
      return Utilities.GetMySqlServerCharSetAndCollation(ConnectionString);
    }

    /// <summary>
    /// Gets the value of the DEFAULT_STORAGE_ENGINE MySQL Server variable indicating the default DB engine used for new table creations.
    /// </summary>
    /// <returns>The default DB engine used for new table creations.</returns>
    public string GetMySqlServerDefaultEngine()
    {
      return Utilities.GetMySqlServerDefaultEngine(ConnectionString);
    }

    /// <summary>
    /// Gets the value of the global SQL_MODE MySQL Server variable.
    /// </summary>
    /// <returns>The value of the global SQL_MODE system variable.</returns>
    public string GetMySqlServerGlobalMode()
    {
      return Utilities.GetMySqlServerGlobalMode(ConnectionString);
    }

    /// <summary>
    /// Gets the value of the LOWER_CASE_TABLE_NAMES MySQL Server variable indicating the case sensitiv.
    /// </summary>
    /// <returns><c>true</c> if table names are stored in lowercase on disk and comparisons are not case sensitive, <c>false</c> if table names are stored as specified and comparisons are case sensitive.</returns>
    public bool GetMySqlServerLowerCaseTableNames()
    {
      return Utilities.GetMySqlServerLowerCaseTableNames(ConnectionString);
    }

    /// <summary>
    /// Gets the value of the MAX_ALLOWED_PACKET MySQL Server variable indicating the max size in bytes of the packet returned by a single query.
    /// </summary>
    /// <returns>The max size in bytes of the packet returned by a single query.</returns>
    public int GetMySqlServerMaxAllowedPacket()
    {
      return Utilities.GetMySqlServerMaxAllowedPacket(ConnectionString);
    }

    /// <summary>
    /// Gets the version of the connected MySQL server.
    /// </summary>
    /// <returns>The version of the connected MySQL server.</returns>
    public string GetMySqlServerVersion()
    {
      return Utilities.GetMySqlServerVersion(ConnectionString);
    }

    /// <summary>
    /// Gets the character set and its collation used by the currently selected schema.
    /// </summary>
    /// <param name="schemaName">The name of a database schema where the table resides.</param>
    /// <returns>The character set and its collation used by the currently selected schema.</returns>
    public Tuple<string, string> GetSchemaCharSetAndCollation(string schemaName = null)
    {
      schemaName = string.IsNullOrEmpty(schemaName) ? Schema : schemaName;
      return Utilities.GetSchemaCharSetAndCollation(ConnectionString, schemaName);
    }

    /// <summary>
    /// Gets the schema information ofr the given database collection.
    /// </summary>
    /// <param name="schemaInformation">The type of schema information to query.</param>
    /// <param name="showErrors">Flag indicating whether errors are shown to the users if any or just logged.</param>
    /// <param name="restrictions">Specific parameters that vary among database collections.</param>
    /// <returns>Schema information within a data table.</returns>
    public DataTable GetSchemaInformation(SchemaInformationType schemaInformation, bool showErrors, params string[] restrictions)
    {
      return Utilities.GetSchemaInformation(ConnectionString, schemaInformation, showErrors, restrictions);
    }

    /// <summary>
    /// Gets the collation defined on a MySQL table with the given name.
    /// </summary>
    /// <param name="schemaName">The name of a database schema where the table resides.</param>
    /// <param name="tableName">The name of the database table.</param>
    /// <returns>The collation defined on a MySQL table with the given name.</returns>
    public string GetTableCollation(string schemaName, string tableName)
    {
      return GetTableCollation(schemaName, tableName, out _);
    }

    /// <summary>
    /// Gets the collation defined on a MySQL table with the given name.
    /// </summary>
    /// <param name="schemaName">The name of a database schema where the table resides.</param>
    /// <param name="tableName">The name of the database table.</param>
    /// <param name="charSet">The character set that belongs to the table collation.</param>
    /// <returns>The collation defined on a MySQL table with the given name.</returns>
    public string GetTableCollation(string schemaName, string tableName, out string charSet)
    {
      return Utilities.GetTableCollation(ConnectionString, schemaName, tableName, out charSet);
    }

    /// <summary>
    /// Resets the password for the current user with a given new clear text password.
    /// </summary>
    /// <param name="newPassword">New password in unprotected clear text.</param>
    /// <returns><c>true</c> if the password reset is successful, <c>false</c> otherwise.</returns>
    public void ResetPassword(string newPassword)
    {
      var success = true;
      try
      {
        MySqlHelper.ExecuteNonQuery(ConnectionString, $"SET PASSWORD = PASSWORD('{newPassword}')");
      }
      catch (Exception ex)
      {
        success = false;
        Logger.LogException(ex, true, Resources.PasswordResetErrorDetail, Resources.ErrorText);
      }

      if (success && PasswordExpired)
      {
        PasswordExpired = false;
      }
    }

    /// <summary>
    /// Sets the net_write_timeout and net_read_timeout MySQL server variables to the given value for the duration of the current client session.
    /// </summary>
    /// <param name="timeoutInSeconds">
    /// The number of seconds to wait for more data from a connection before aborting the read or for a block to be written to a connection before aborting the write.
    /// If the parameter is omitted or a value of <c>0</c> is passed to it, the <see cref="DefaultCommandTimeout"/> property value is used.
    /// </param>
    public void SetClientSessionReadWriteTimeouts(uint timeoutInSeconds = 0)
    {
      if (timeoutInSeconds == 0)
      {
        timeoutInSeconds = DefaultCommandTimeout;
      }

      Utilities.SetClientSessionReadWriteTimeouts(ConnectionString, timeoutInSeconds);
    }

    /// <summary>
    /// Syncs the connection properties with the ones of the given <see cref="MySqlWorkbenchConnection"/> object.
    /// </summary>
    /// <param name="sourceConnection">The source connection to sync properties from.</param>
    /// <param name="setSavedStatusToUpdated">Flag indicating if the SavedStatus property is set to Updated if values changed during the synchronization.</param>
    public void Sync(MySqlWorkbenchConnection sourceConnection, bool setSavedStatusToUpdated)
    {
      bool updateNeeded = SyncProperty("AllowUserVariables", sourceConnection);
      updateNeeded = SyncProperty("AllowZeroDateTimeValues", sourceConnection) || updateNeeded;
      updateNeeded = SyncProperty("CharacterSet", sourceConnection) || updateNeeded;
      updateNeeded = SyncProperty("ConnectionMethod", sourceConnection) || updateNeeded;
      updateNeeded = SyncProperty("ConnectionStatus", sourceConnection) || updateNeeded;
      updateNeeded = SyncProperty("ConnectionTimeout", sourceConnection) || updateNeeded;
      updateNeeded = SyncProperty("DatabaseDriverName", sourceConnection) || updateNeeded;
      updateNeeded = SyncProperty("DefaultCommandTimeout", sourceConnection) || updateNeeded;
      updateNeeded = SyncProperty("DefaultSchema", sourceConnection) || updateNeeded;
      updateNeeded = SyncProperty("Existing", sourceConnection) || updateNeeded;
      updateNeeded = SyncProperty("Host", sourceConnection) || updateNeeded;
      updateNeeded = SyncProperty("IntegratedSecurity", sourceConnection) || updateNeeded;
      updateNeeded = SyncProperty("Name", sourceConnection) || updateNeeded;
      updateNeeded = SyncProperty("Password", sourceConnection) || updateNeeded;
      updateNeeded = SyncProperty("Port", sourceConnection) || updateNeeded;
      updateNeeded = SyncProperty("Schema", sourceConnection) || updateNeeded;
      updateNeeded = SyncProperty("SshHost", sourceConnection) || updateNeeded;
      updateNeeded = SyncProperty("SshPassword", sourceConnection) || updateNeeded;
      updateNeeded = SyncProperty("SshPrivateKeyFile", sourceConnection) || updateNeeded;
      updateNeeded = SyncProperty("SshPassPhrase", sourceConnection) || updateNeeded;
      updateNeeded = SyncProperty("SshUserName", sourceConnection) || updateNeeded;
      updateNeeded = SyncProperty("SslCertificationAuthorityFile", sourceConnection) || updateNeeded;
      updateNeeded = SyncProperty("SslClientCertificateFile", sourceConnection) || updateNeeded;
      updateNeeded = SyncProperty("SslKeyFile", sourceConnection) || updateNeeded;
      updateNeeded = SyncProperty("UnixSocketOrWindowsPipe", sourceConnection) || updateNeeded;
      updateNeeded = SyncProperty("UseAnsiQuotes", sourceConnection) || updateNeeded;
      updateNeeded = SyncProperty("UseCompression", sourceConnection) || updateNeeded;
      updateNeeded = SyncProperty("UserName", sourceConnection) || updateNeeded;
      updateNeeded = SyncProperty("UseSsl", sourceConnection) || updateNeeded;
      SavedStatus = setSavedStatusToUpdated && updateNeeded ? SavedStatusType.Updated : sourceConnection.SavedStatus;
    }

    /// <summary>
    /// Tests the current connection until the user enters a correct password.
    /// </summary>
    /// <param name="tryConnectionBeforeAskingForPassword">Flag indicating whether a connection test is made with the connection as is before asking for a password</param>
    /// <returns>A <see cref="PasswordDialogFlags"/> containing data about the operation.</returns>
    public PasswordDialogFlags TestConnectionAndRetryOnWrongPassword(bool tryConnectionBeforeAskingForPassword = true)
    {
      var passwordFlags = new PasswordDialogFlags(_password)
      {
        // Assume a wrong password at first so if the connection is not tested without a password we ensure to ask for one.
        ConnectionResultType = ConnectionResultType.WrongPassword
      };

      // First connection attempt with the connection exactly as loaded (maybe without a password).
      if (tryConnectionBeforeAskingForPassword)
      {
        passwordFlags.ConnectionResultType = TestConnectionShowingErrors(false);
        passwordFlags.Cancelled = passwordFlags.ConnectionResultType == ConnectionResultType.PasswordExpired;

        // If on the first attempt a connection could not be made and not because of a bad password, exit.
        if (!passwordFlags.ConnectionSuccess && !passwordFlags.WrongPassword)
        {
          return passwordFlags;
        }
      }

      // If the connection does not have a stored password or the stored password failed then ask for one and retry.
      while (!passwordFlags.ConnectionSuccess && passwordFlags.WrongPassword)
      {
        passwordFlags = PasswordDialog.ShowConnectionPasswordDialog(this, true);
        if (passwordFlags.Cancelled)
        {
          break;
        }

        Password = passwordFlags.NewPassword;
      }

      return passwordFlags;
    }

    /// <summary>
    /// Tests the current connection, showing errors to uers, to check if it can successfully connect to the corresponding MySQL instance.
    /// </summary>
    /// <param name="displayErrorOnEmptyPassword">Flag indicating whether errors caused by a blank or null password are displayed to the user.</param>
    /// <returns>Enumeration indicating the result of the connection test.</returns>
    public ConnectionResultType TestConnectionShowingErrors(bool displayErrorOnEmptyPassword)
    {
      return TestConnectionAndReturnResult(false, displayErrorOnEmptyPassword, out _);
    }

    /// <summary>
    /// Tests the current connection in a silent way (not displaying errors to users) to check if it can successfully connect to the corresponding MySQL instance.
    /// </summary>
    /// <param name="connectionException">The exception thrown by the test.</param>
    /// <returns>Enumeration indicating the result of the connection test.</returns>
    public ConnectionResultType TestConnectionSilently(out Exception connectionException)
    {
      return TestConnectionAndReturnResult(true, false, out connectionException);
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>The name of this Workbench connection.</returns>
    public override string ToString()
    {
      return Name;
    }

    /// <summary>
    /// Returns this connection as an XmlElement
    /// </summary>
    /// <param name="doc">The XML document where this connection will be saved to.</param>
    /// <returns>A XML element representing a base Workbench connection XML node.</returns>
    internal new XmlElement ToElement(XmlDocument doc)
    {
      var parameterValues = GetParameterValuesElementWithBaseConnectionParent(doc);
      if (parameterValues == null)
      {
        return null;
      }

      AppendPropertyValueToElement(doc, parameterValues, "hostName", "string", IsNamedPipesConnection ? LOCAL_IP : Host);
      if (!IsNamedPipesConnection)
      {
        AppendPropertyValueToElement(doc, parameterValues, "port", "int", Port);
      }
      else
      {
        AppendPropertyValueToElement(doc, parameterValues, "socket", "string", UnixSocketOrWindowsPipe);
      }

      AppendPropertyValueToElement(doc, parameterValues, "userName", "string", UserName);
      AppendPropertyValueToElement(doc, parameterValues, "password", "string", string.Empty);

      if (!string.IsNullOrEmpty(DefaultSchema))
      {
        AppendPropertyValueToElement(doc, parameterValues, "schema", "string", DefaultSchema);
      }

      if (!string.IsNullOrEmpty(SslCertificationAuthorityFile))
      {
        AppendPropertyValueToElement(doc, parameterValues, "sslCA", "string", SslCertificationAuthorityFile);
      }

      if (!string.IsNullOrEmpty(SslClientCertificateFile))
      {
        AppendPropertyValueToElement(doc, parameterValues, "sslCert", "string", SslClientCertificateFile);
      }

      if (!string.IsNullOrEmpty(SslKeyFile))
      {
        AppendPropertyValueToElement(doc, parameterValues, "sslKey", "string", SslKeyFile);
      }

      if (UseCompression)
      {
        AppendPropertyValueToElement(doc, parameterValues, "CLIENT_COMPRESS", "int", 1);
      }

      if (UseAnsiQuotes)
      {
        AppendPropertyValueToElement(doc, parameterValues, "useAnsiQuotes", "int", 1);
      }

      if (ConnectionTimeout != DEFAULT_CONNECTION_TIMEOUT)
      {
        AppendPropertyValueToElement(doc, parameterValues, "timeout", "int", ConnectionTimeout);
      }

      switch (UseSsl)
      {
        case UseSslType.No:
          AppendPropertyValueToElement(doc, parameterValues, "useSSL", "int", 0);
          break;

        case UseSslType.IfAvailable:
          AppendPropertyValueToElement(doc, parameterValues, "useSSL", "int", 1);
          break;

        case UseSslType.Require:
          AppendPropertyValueToElement(doc, parameterValues, "useSSL", "int", 2);
          break;

        case UseSslType.RequireAndVerifyCertificationAuthority:
          AppendPropertyValueToElement(doc, parameterValues, "useSSL", "int", 3);
          break;

        case UseSslType.RequireAndVerifyIdentity:
          AppendPropertyValueToElement(doc, parameterValues, "useSSL", "int", 4);
          break;
      }

      if (IsSshConnection)
      {
        AppendPropertyValueToElement(doc, parameterValues, "sshHost", "string", SshHost);
        AppendPropertyValueToElement(doc, parameterValues, "sshKeyFile", "string", SshPrivateKeyFile);
        AppendPropertyValueToElement(doc, parameterValues, "sshPassword", "string", string.Empty);
        AppendPropertyValueToElement(doc, parameterValues, "sshUserName", "string", SshUserName);
      }

      return parameterValues.ParentNode as XmlElement;
    }

    /// <summary>
    /// Determines whether two <see cref="MySqlWorkbenchConnection"/> are equal.
    /// </summary>
    /// <param name="other">A <see cref="MySqlWorkbenchConnection"/> to compare to this one.</param>
    /// <returns><c>true</c> if the specified <see cref="MySqlWorkbenchConnection"/> is equal to the current <see cref="MySqlWorkbenchConnection"/>, <c>false</c> otherwise.</returns>
    protected bool Equals(MySqlWorkbenchConnection other)
    {
      return AllowUserVariables == other.AllowUserVariables
        && AllowZeroDateTimeValues == other.AllowZeroDateTimeValues
        && string.Equals(CharacterSet, other.CharacterSet, StringComparison.Ordinal)
        && ConnectionMethod == other.ConnectionMethod
        && ConnectionTimeout == other.ConnectionTimeout
        && string.Equals(DatabaseDriverName, other.DatabaseDriverName, StringComparison.Ordinal)
        && DefaultCommandTimeout == other.DefaultCommandTimeout
        && string.Equals(DefaultSchema, other.DefaultSchema, StringComparison.Ordinal)
        && Existing == other.Existing
        && string.Equals(Host, other.Host, StringComparison.Ordinal)
        && IntegratedSecurity.Equals(other.IntegratedSecurity)
        && string.Equals(Name, other.Name, StringComparison.Ordinal)
        && string.Equals(Password, other.Password, StringComparison.Ordinal)
        && Port == other.Port
        && string.Equals(Schema, other.Schema, StringComparison.Ordinal)
        && string.Equals(SshHost, other.SshHost, StringComparison.Ordinal)
        && string.Equals(SshPassword, other.SshPassword, StringComparison.Ordinal)
        && string.Equals(SshPrivateKeyFile, other.SshPrivateKeyFile, StringComparison.Ordinal)
        && string.Equals(SshPassPhrase, other.SshPassPhrase, StringComparison.Ordinal)
        && string.Equals(SshUserName, other.SshUserName, StringComparison.Ordinal)
        && string.Equals(SslCertificationAuthorityFile, other.SslCertificationAuthorityFile, StringComparison.Ordinal)
        && string.Equals(SslClientCertificateFile, other.SslClientCertificateFile, StringComparison.Ordinal)
        && string.Equals(SslKeyFile, other.SslKeyFile, StringComparison.Ordinal)
        && TreatTinyIntAsBoolean == other.TreatTinyIntAsBoolean
        && string.Equals(UnixSocketOrWindowsPipe, other.UnixSocketOrWindowsPipe, StringComparison.Ordinal)
        && UseAnsiQuotes == other.UseAnsiQuotes
        && UseCompression == other.UseCompression
        && string.Equals(UserName, other.UserName, StringComparison.Ordinal)
        && UseSsl == other.UseSsl;
    }

    /// <summary>
    /// Creates the base structure for a Workbench connection XML node with a child parameters value element and returns the latter.
    /// </summary>
    /// <param name="doc">The XML document where this connection will be saved to.</param>
    /// <returns>The parameters value (child) element containing a parent node representing a base Workbench connection XML node.</returns>
    protected new XmlElement GetParameterValuesElementWithBaseConnectionParent(XmlDocument doc)
    {
      var parameterValues = base.GetParameterValuesElementWithBaseConnectionParent(doc);
      var connectionElement = parameterValues?.ParentNode as XmlElement;
      if (connectionElement == null)
      {
        return null;
      }

      XmlElement link = doc.CreateElement("link");
      link.SetAttribute("type", "object");
      link.SetAttribute("struct-name", "db.mgmt.Driver");
      link.SetAttribute("key", "driver");

      switch (ConnectionMethod)
      {
        case ConnectionMethodType.Tcp:
          link.InnerText = "com.mysql.rdbms.mysql.driver.native";
          break;

        case ConnectionMethodType.LocalUnixSocketOrWindowsPipe:
          link.InnerText = "com.mysql.rdbms.mysql.driver.native_socket";
          break;

        case ConnectionMethodType.Ssh:
          link.InnerText = "com.mysql.rdbms.mysql.driver.native_sshtun";
          break;

        case ConnectionMethodType.XProtocol:
          link.InnerText = "com.mysql.rdbms.mysql.driver.xplugin";
          break;

        default:
          link.InnerText = "";
          break;
      }

      connectionElement.InsertBefore(link, parameterValues);

      XmlElement modules = doc.CreateElement("value");
      modules.SetAttribute("_ptr_", connectionElement.RoughSizeOf().ToString("X16"));
      modules.SetAttribute("type", "dict");
      modules.SetAttribute("key", "modules");
      connectionElement.InsertAfter(modules, link);

      InsertPropertyValueToElementAfter(doc, connectionElement, link, "isDefault", "int", 0);
      InsertPropertyValueToElementAfter(doc, connectionElement, link, "hostIdentifier", "string", HostIdentifier);

      return parameterValues;
    }

    /// <summary>
    /// Raises the <see cref="MySqlWorkbenchConnectionExtraParameters.PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed.</param>
    protected override void NotifyPropertyChanged(string propertyName)
    {
      if (propertyName != "ConnectionStatus" && propertyName != "DatabaseDriverName")
      {
        ConnectionStatus = ConnectionStatusType.Unknown;
      }

      base.NotifyPropertyChanged(propertyName);
    }

    /// <summary>
    /// Sets properties from serialized values.
    /// </summary>
    /// <param name="el">XML element representing this serialized object.</param>
    protected new void ProcessElement(XmlElement el)
    {
      string type = el.Attributes["type"].Value;
      if (type == "dict")
      {
        foreach (XmlElement childEl in el.ChildNodes)
        {
          ProcessElement(childEl);
        }

        return;
      }

      string key = el.Attributes["key"].Value;
      string value = el.InnerText;
      switch (key)
      {
        case "hostIdentifier":
          HostIdentifier = value;
          break;

        case "hostName":
          Host = value;
          break;

        case "port":
          Port = uint.Parse(value);
          break;

        case "userName":
          UserName = value;
          break;

        case "name":
          Name = value;
          break;

        case "socket":
          UnixSocketOrWindowsPipe = value;
          break;

        case "CLIENT_COMPRESS":
          UseCompression = value == "1" || value.ToLower() == "true";
          break;

        case "schema":
          DefaultSchema = Schema = value;
          break;

        case "sslCA":
          SslCertificationAuthorityFile = value;
          break;

        case "sslCert":
          SslClientCertificateFile = value;
          break;

        case "sslKey":
          SslKeyFile = value;
          break;

        case "sshHost":
          SshHost = value;
          break;

        case "sshUserName":
          SshUserName = value;
          break;

        case "sshKeyFile":
          SshPrivateKeyFile = value;
          break;

        case "useAnsiQuotes":
          UseAnsiQuotes = value == "1" || value.ToLower() == "true";
          break;

        case "useSSL":
          switch (value)
          {
            case "0":
              UseSsl = UseSslType.No;
              break;

            case "1":
              UseSsl = UseSslType.IfAvailable;
              break;

            case "2":
              UseSsl = UseSslType.Require;
              break;

            case "3":
              UseSsl = UseSslType.RequireAndVerifyCertificationAuthority;
              break;

            case "4":
              UseSsl = UseSslType.RequireAndVerifyIdentity;
              break;
          }
          break;

        case "driver":
          var endsWith = value.Substring("com.mysql.rdbms.mysql.driver".Length);
          switch (endsWith)
          {
            case ".native":
              ConnectionMethod = ConnectionMethodType.Tcp;
              break;

            case ".native_sshtun":
              ConnectionMethod = ConnectionMethodType.Ssh;
              break;

            case ".native_socket":
              ConnectionMethod = ConnectionMethodType.LocalUnixSocketOrWindowsPipe;
              break;

            case ".xplugin":
              ConnectionMethod = ConnectionMethodType.XProtocol;
              break;

            default:
              ConnectionMethod = ConnectionMethodType.Unknown;
              break;
          }
          break;

        case "timeout":
          ConnectionTimeout = uint.Parse(value);
          break;
      }
    }

    /// <summary>
    /// Builds the host identifier describing where the MySQL server instance can be reached at.
    /// </summary>
    private void SetHostIdentifier()
    {
      switch (ConnectionMethod)
      {
        case ConnectionMethodType.LocalUnixSocketOrWindowsPipe:
          DisplayConnectionSummaryText = "Localhost via pipe";
          HostIdentifier = $"{DatabaseDriverName}@local:{UnixSocketOrWindowsPipe}";
          break;

        case ConnectionMethodType.Ssh:
          DisplayConnectionSummaryText = $"{SshUserName}@{SshHost} (SSH)";
          HostIdentifier = $"{DatabaseDriverName}@{Host}:{Port}@{SshHost}";
          break;

        case ConnectionMethodType.Tcp:
        case ConnectionMethodType.XProtocol:
          DisplayConnectionSummaryText = $"{Host}:{Port}";
          HostIdentifier = $"{DatabaseDriverName}@{Host}:{Port}";
          break;
      }
    }

    /// <summary>
    /// Copies the value of the given property name in the source connection object to this object if needed.
    /// </summary>
    /// <param name="propertyName">The name of the property whose values are going to be compared and synchronized.</param>
    /// <param name="sourceConnection">The source connection to sync the property from.</param>
    /// <returns><c>true</c> if the values were different so the property value in the source connection is copied to this object, <c>false</c> otherwise.</returns>
    private bool SyncProperty(string propertyName, MySqlWorkbenchConnection sourceConnection)
    {
      if (sourceConnection == null)
      {
        return false;
      }

      object sourceValue = sourceConnection.GetType().GetProperty(propertyName)?.GetValue(sourceConnection, null);
      object targetValue = GetType().GetProperty(propertyName)?.GetValue(this, null);
      if (sourceValue == targetValue)
      {
        return false;
      }

      GetType().GetProperty(propertyName)?.SetValue(this, sourceValue, null);
      return true;
    }

    /// <summary>
    /// Tests the current connection to check if it can successfully connect to the corresponding MySQL instance.
    /// </summary>
    /// <param name="connectionException">The exception thrown by the MySQL server instance in case the connection fails.</param>
    /// <returns><c>true</c> if successfully connects, <c>false</c> otherwise.</returns>
    private bool TestConnection(out Exception connectionException)
    {
      connectionException = null;
      MySqlConnection connection = null;
      try
      {
        connection = new MySqlConnection(ConnectionString);
        connection.Open();
        MySqlHelper.ExecuteNonQuery(connection, "SELECT CURRENT_USER()", null);
      }
      catch (MySqlException mySqlEx)
      {
        connectionException = mySqlEx;
        if (mySqlEx.Number == MYSQL_EXCEPTION_NUMBER_EXPIRED_PASSWORD)
        {
          PasswordExpired = true;
        }
        else if (mySqlEx.Message.IndexOf(Resources.ConnectionIsInsecureAndRequiresRsaKeyRetrievalError, StringComparison.OrdinalIgnoreCase) >= 0)
        {
          var dialogResult = InfoDialog.ShowDialog(InfoDialogProperties.GetYesNoDialogProperties(InfoDialog.InfoType.Warning,
            Resources.ConnectionIsInsecureAndRequiresRsaKeyRetrievalTitle,
            Resources.ConnectionIsInsecureAndRequiresRsaKeyRetrievalDetail,
            Resources.ConnectionIsInsecureAndRequiresRsaKeyRetrievalSubDetail,
            Resources.ConnectionIsInsecureAndRequiresRsaKeyRetrievalMoreInfo));
          if (dialogResult.DialogResult == DialogResult.Yes)
          {
            RetrieveRsaKeysForInsecureConnection = true;
            return TestConnection(out connectionException);
          }

          // Override the Exception with a new one that contains a more meaningful message about the secure connection not being established
          connectionException = new Exception(Resources.ConnectionInsecureError);
        }
      }
      catch (Exception ex)
      {
        connectionException = ex;
      }
      finally
      {
        connection?.Close();
      }

      var connectionSuccess = connectionException == null;
      ConnectionStatus = connectionSuccess
        ? ConnectionStatusType.AcceptingConnections
        : ConnectionStatusType.RefusingConnections;
      return connectionSuccess;
    }

    /// <summary>
    /// Tests the current connection to check if it can successfully connect to the corresponding MySQL instance.
    /// </summary>
    /// <param name="silent">Flag indicating whether any error is displayed to users.</param>
    /// <param name="displayErrorOnEmptyPassword">Flag indicating whether errors caused by a blank or null password are displayed to the user.</param>
    /// <param name="connectionException">The exception thrown by the test.</param>
    /// <returns>Enumeration indicating the result of the connection test.</returns>
    private ConnectionResultType TestConnectionAndReturnResult(bool silent, bool displayErrorOnEmptyPassword, out Exception connectionException)
    {
      ConnectionResultType connectionResultType;
      MySqlWorkbench.ChangeCurrentCursor(Cursors.WaitCursor);
      if (TestConnection(out connectionException))
      {
        connectionResultType = ConnectionResultType.ConnectionSuccess;
        MySqlWorkbench.ChangeCurrentCursor(Cursors.Default);
        return connectionResultType;
      }

      // If the error returned is about the connection failing the password check, it may be because either the stored password is wrong or no password.
      connectionResultType = ConnectionResultType.ConnectionError;
      if (connectionException is MySqlException mySqlException)
      {
        switch (mySqlException.Number)
        {
          // Connection could not be made.
          case MYSQL_EXCEPTION_NUMBER_SERVER_UNREACHABLE:
            connectionResultType = ConnectionResultType.HostUnreachable;
            if (!silent)
            {
              InfoDialog.ShowDialog(InfoDialogProperties.GetErrorDialogProperties(
                Resources.ConnectFailedWarningTitle,
                mySqlException.Message,
                null,
                mySqlException.InnerException?.Message));
            }
            break;

          // Wrong password.
          case MYSQL_EXCEPTION_NUMBER_WRONG_PASSWORD:
            connectionResultType = ConnectionResultType.WrongPassword;
            if (!silent && (!string.IsNullOrEmpty(Password) || displayErrorOnEmptyPassword))
            {
              InfoDialog.ShowDialog(InfoDialogProperties.GetWarningDialogProperties(
                Resources.ConnectFailedWarningTitle,
                mySqlException.Message,
                null,
                UseSsl != UseSslType.No ? Resources.ConnectSSLFailedDetailWarning : null));
            }
            break;

          // Password has expired so any statement can't be run before resetting the expired password.
          case MYSQL_EXCEPTION_NUMBER_EXPIRED_PASSWORD:
            var passwordFlags = PasswordDialog.ShowExpiredPasswordDialog(this, false);
            if (!passwordFlags.Cancelled)
            {
              Password = passwordFlags.NewPassword;
            }

            connectionResultType = passwordFlags.Cancelled ? ConnectionResultType.PasswordExpired : ConnectionResultType.PasswordReset;
            break;

          // Any other exception.
          default:
            if (!silent)
            {
              InfoDialog.ShowDialog(InfoDialogProperties.GetErrorDialogProperties(
                Resources.ConnectFailedWarningTitle,
                string.Format(Resources.GenericMySQLError, mySqlException.Number, mySqlException.Message),
                null,
                mySqlException.InnerException?.Message));
            }
            break;
        }
      }
      else if (!silent)
      {
        InfoDialog.ShowDialog(InfoDialogProperties.GetErrorDialogProperties(
          Resources.ConnectFailedWarningTitle,
          connectionException.Message,
          null,
          connectionException.InnerException?.Message));
      }

      MySqlWorkbench.ChangeCurrentCursor(Cursors.Default);
      return connectionResultType;
    }
  }
}