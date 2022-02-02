// Copyright (c) 2018, 2019, Oracle and/or its affiliates. All rights reserved.
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
using System.Linq;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using MySql.Utility.Classes.Logging;
using MySql.Utility.Enums;
using MySql.Installer.Core.Enums;

namespace MySql.Utility.Classes.MySql
{
  /// <summary>
  /// Contains information about a MySQL Server instance.
  /// </summary>
  public class MySqlServerInstance
  {
    #region Constants

    /// <summary>
    /// The maximum length allowed for MySQL cluster names.
    /// </summary>
    public const int CLUSTER_NAME_MAX_LENGTH = 40;

    /// <summary>
    /// The maximum length allowed for MySQL schemas and tables.
    /// </summary>
    public const int MAX_MYSQL_SCHEMA_OR_TABLE_NAME_LENGTH = 64;

    /// <summary>
    /// The maximum port number allowed.
    /// </summary>
    public const uint MAX_PORT_NUMBER_ALLOWED = ushort.MaxValue;

    /// <summary>
    /// The maximum port number allowed.
    /// </summary>
    public const uint MIN_PORT_NUMBER_ALLOWED = 1;

    /// <summary>
    /// The minimum port number allowed for MySQL connections.
    /// </summary>
    public const uint MIN_MYSQL_PORT_NUMBER_ALLOWED = 80;

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

    /// <summary>
    /// The regex used to validate MySQL user and cluster names.
    /// </summary>
    public const string NAME_REGEX_VALIDATION = @"^(\w|\d|_|\s)+$";

    /// <summary>
    /// The minimum suggested length for a password.
    /// </summary>
    public const int PASSWORD_MIN_LENGTH = 4;

    /// <summary>
    /// The sandbox InnoDB Cluster seed instance port.
    /// </summary>
    public const uint SANDBOX_INNODB_CLUSTER_SEED_INSTANCE_PORT = 3310;

    /// <summary>
    /// The maximum length allowed for MySQL user names.
    /// </summary>
    public const int USERNAME_MAX_LENGTH = 32;

    /// <summary>
    /// The waiting time in milliseconds between connection attempts.
    /// </summary>
    public const int WAITING_TIME_BETWEEN_CONNECTIONS_IN_MILLISECONDS = 3000;
  
    #endregion Constants

    #region Fields

    /// <summary>
    /// The member role of this instance in a group replication cluster.
    /// </summary>
    private GroupReplicationMemberRoleType _groupReplicationMemberRole;

    /// <summary>
    /// The member count in the group replication cluster.
    /// </summary>
    private int _groupReplicationMemberCount;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlServerInstance"/> class.
    /// </summary>
    /// <param name="port">The port where this instance listens for connections.</param>
    /// <param name="reportStatusDelegate">An <seealso cref="System.Action"/> to output status messages.</param>
    public MySqlServerInstance(uint port, Action<string> reportStatusDelegate = null)
    {
      _groupReplicationMemberRole = GroupReplicationMemberRoleType.Unknown;
      _groupReplicationMemberCount = -1;
      ConnectionProtocol = MySqlConnectionProtocol.Tcp;
      DisableReportStatus = false;
      PipeOrSharedMemoryName = null;
      Port = port;
      ReportStatusDelegate = reportStatusDelegate;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlServerInstance"/> class.
    /// </summary>
    /// <param name="port">The port where this instance listens for connections.</param>
    /// <param name="userAccount">The <see cref="MySqlServerUser"/> to establish connections.</param>
    /// <param name="reportStatusDelegate">An <seealso cref="System.Action"/> to output status messages.</param>
    public MySqlServerInstance(uint port, MySqlServerUser userAccount, Action<string> reportStatusDelegate = null)
      : this(port, reportStatusDelegate)
    {
      UserAccount = userAccount;
    }

    #region Properties

    /// <summary>
    /// Gets or sets the <see cref="MySqlConnectionProtocol"/> for establishing connections.
    /// </summary>
    public MySqlConnectionProtocol ConnectionProtocol { get; set; }

    /// <summary>
    /// Flag indicating if any reporting of statuses is disabled even if the <seealso cref="ReportStatusDelegate"/> exists.
    /// </summary>
    protected bool DisableReportStatus { get; set; }

    /// <summary>
    /// Gets the member role of this instance in a group replication cluster.
    /// </summary>
    public GroupReplicationMemberRoleType GroupReplicationMemberRole
    {
      get
      {
        if (_groupReplicationMemberRole == GroupReplicationMemberRoleType.Unknown)
        {
          _groupReplicationMemberRole = GetGroupReplicationMemberRole();
        }

        return _groupReplicationMemberRole;
      }
    }

    /// <summary>
    /// Gets the member count belonging to the cluster the local instance belongs to.
    /// </summary>
    public int GroupReplicationMemberCount
    {
      get
      {
        if (_groupReplicationMemberCount == -1)
        {
          _groupReplicationMemberCount = GetGroupReplicationMemberCount();
        }

        return _groupReplicationMemberCount;
      }
    }

    /// <summary>
    /// Gets a value indicating if the username is valid.
    /// </summary>
    public bool IsUsernameValid
    {
      get
      {
        var errorMessage = ValidateUserName(UserAccount.Username, true);
        if (string.IsNullOrEmpty(errorMessage))
        {
          return true;
        }

        Logger.LogError(errorMessage);
        return false;
      }
    }

    /// <summary>
    /// Gets the name of this instance containing its Server version.
    /// </summary>
    public virtual string NameWithVersion => $"{UserAccount.Host}:{Port}";

    /// <summary>
    /// Gets or sets the name of the Windows pipe or the shared memory to use when establishing connections.
    /// </summary>
    public string PipeOrSharedMemoryName { get; set; }

    /// <summary>
    /// Gets the port where this instance listens for connections.
    /// </summary>
    public uint Port { get; protected set; }

    /// <summary>
    /// Gets an <seealso cref="System.Action"/> to output status messages.
    /// </summary>
    public Action<string> ReportStatusDelegate { get; }

    /// <summary>
    /// Gets the Server ID of this instance.
    /// </summary>
    public virtual uint ServerId
    {
      get
      {
        const string SQL = "SELECT @@server_id";
        var id = ExecuteScalar(SQL, out var error);
        return string.IsNullOrEmpty(error)
          ? (uint)id
          : 0;
      }
    }

    /// <summary>
    /// Gets the <see cref="MySqlServerUser"/> to establish connections.
    /// </summary>
    public MySqlServerUser UserAccount { get; set; }

    #endregion Properties

    /// <summary>
    /// Validates that the given MySQL cluster name is well formed.
    /// </summary>
    /// <param name="clusterName">A MySQL cluster name.</param>
    /// <returns>An empty string if the cluster name is well formed, otherwise an error message.</returns>
    public static string ValidateClusterName(string clusterName)
    {
      if (string.IsNullOrWhiteSpace(clusterName))
      {
        return Resources.MySqlServerClusterNameRequired;
      }

      var trimmedClusterName = clusterName.Trim();
      if (trimmedClusterName.Length > CLUSTER_NAME_MAX_LENGTH)
      {
        return Resources.MySqlServerClusterNameMaxLengthExceeded;
      }

      var clusterNameRegex = new Regex(NAME_REGEX_VALIDATION);
      return clusterNameRegex.IsMatch(trimmedClusterName)
        ? string.Empty
        : Resources.MySqlServerClusterNameInvalid;
    }

    /// <summary>
    /// Validates that the given host name or IP address is well formed.
    /// </summary>
    /// <param name="hostNameOrIpAddress">A host name or IP address.</param>
    /// <param name="validHostNameType">The type of hostnames to validate against (a combination of flags can be given).</param>
    /// <returns>An empty string if the host name or IP address is well formed, otherwise an error message.</returns>
    public static string ValidateHostNameOrIpAddress(string hostNameOrIpAddress, ValidHostNameType validHostNameType = ValidHostNameType.DNS | ValidHostNameType.IPv4)
    {
      if (string.IsNullOrWhiteSpace(hostNameOrIpAddress))
      {
        return Resources.MySqlServerInstanceRequiredHostOrIpError;
      }

      return !validHostNameType.HasFlag(Uri.CheckHostName(hostNameOrIpAddress).ToValidHostNameType())
        ? Resources.MySqlServerInstanceInvalidHostOrIpError
        : string.Empty;
    }

    /// <summary>
    /// Validates that the given user password meets requirements.
    /// </summary>
    /// <param name="password">A MySQL cluster name.</param>
    /// <param name="validateBlank"></param>
    /// <returns>An empty string if the password meets requirements, otherwise an error message.</returns>
    public static string ValidatePassword(string password, bool validateBlank)
    {
      if (validateBlank && string.IsNullOrWhiteSpace(password))
      {
        return Resources.MySqlServerPasswordRequired;
      }

      if (password.Length < PASSWORD_MIN_LENGTH)
      {
        return Resources.MySqlServerPasswordNotGoodEnough;
      }

      return string.Empty;
    }

    /// <summary>
    /// Validates a Windows pipe or shared memory stream.
    /// </summary>
    /// <param name="namedPipe">Flag indicating if the name is for a named pipe or shared memory.</param>
    /// <param name="name">A name for either a Windows pipe or shared memory stream.</param>
    /// <returns>An empty string if the pipe or shared memory name is valid, otherwise an error message.</returns>
    public static string ValidatePipeOrSharedMemoryName(bool namedPipe, string name)
    {
      var element = namedPipe ? "pipe" : "shared memory";
      var errorMessage = string.Empty;
      if (string.IsNullOrWhiteSpace(name))
      {
        errorMessage = Resources.PipeOrSharedMemoryNameRequiredError;
      }
      else if (name.Length > 256)
      {
        errorMessage = Resources.PipeOrSharedMemoryNameLengthError;
      }
      else if (name.Any(c => c == '\\'))
      {
        errorMessage = Resources.PipeOrSharedMemoryNameBackSlashesError;
      }

      return string.IsNullOrEmpty(errorMessage)
        ? errorMessage
        : string.Format(errorMessage, element);
    }

    /// <summary>
    /// Validates the given port number.
    /// </summary>
    /// <param name="port">A text representation of the port number.</param>
    /// <param name="validateNotInUse">Check if the port is not already being used.</param>
    /// <param name="oldPort">An optional port already configured, if validating not in use but the given port equals the old port no error message is returned.</param>
    /// <param name="validateMySqlPort">Flag indicating whether the given port is to be used with a MySQL Server or with any other TCP/IP port.</param>
    /// <returns>An empty string if the port is a number and within a valid range, otherwise an error message.</returns>
    public static string ValidatePortNumber(string port, bool validateNotInUse, uint? oldPort = null, bool validateMySqlPort = true)
    {
      if (string.IsNullOrWhiteSpace(port))
      {
        return Resources.MySqlServerPortNumberRequired;
      }

      var isValid = uint.TryParse(port, out var numericPort);
      if (!isValid)
      {
        return Resources.MySqlServerPortNumberInvalid;
      }

      if (validateMySqlPort ? numericPort < MIN_MYSQL_PORT_NUMBER_ALLOWED : numericPort < MIN_PORT_NUMBER_ALLOWED
          || numericPort > MAX_PORT_NUMBER_ALLOWED)
      {
        return string.Format(Resources.MySqlServerInvalidPortRange, MIN_MYSQL_PORT_NUMBER_ALLOWED, MAX_PORT_NUMBER_ALLOWED);
      }

      if (validateNotInUse
          && !Utilities.PortIsAvailable(numericPort)
          && oldPort.HasValue
          && oldPort.Value != numericPort)
      {
        return Resources.MySqlServerPortInUse;
      }

      return string.Empty;
    }

    /// <summary>
    /// Validates the given MySQL schema or table name.
    /// </summary>
    /// <param name="name">A MySQL schema or table name.</param>
    /// <returns>An empty string if the MySQL schema or table name is valid, otherwise an error message.</returns>
    public static string ValidateSchemaOrTableName(string name)
    {
      if (string.IsNullOrWhiteSpace(name))
      {
        return Resources.MySqlSchemaTableNameEmptyOrWhiteSpaceError;
      }

      if (name.Length > MAX_MYSQL_SCHEMA_OR_TABLE_NAME_LENGTH)
      {
        return string.Format(Resources.MySqlSchemaTableNameExceedsMaxLengthError, MAX_MYSQL_SCHEMA_OR_TABLE_NAME_LENGTH);
      }

      if (name.EndsWith(" "))
      {
        return Resources.MySqlSchemaTableNameEndsWithWhiteSpaceError;
      }

      if (name.All(char.IsDigit))
      {
        return Resources.MySqlSchemaTableNameAllDigitsError;
      }

      return string.Empty;
    }

    /// <summary>
    /// Validates that the given MySQL user name is well formed.
    /// </summary>
    /// <param name="username">A MySQL user name.</param>
    /// <param name="allowRoot">Flag indicating if root is allowed or an error message is thrown.</param>
    /// <returns>An empty string if the user name is well formed, otherwise an error message.</returns>
    public static string ValidateUserName(string username, bool allowRoot)
    {
      if (string.IsNullOrWhiteSpace(username))
      {
        return Resources.MySqlServerUsernameRequired;
      }

      var trimmedUserName = username.Trim();
      if (trimmedUserName.Length > USERNAME_MAX_LENGTH)
      {
        return Resources.MySqlServerUsernameMaxLengthExceeded;
      }

      if (!allowRoot
          && username.Equals(MySqlServerUser.ROOT_USERNAME, StringComparison.OrdinalIgnoreCase))
      {
        return Resources.MySqlServerUserNameInvalidRoot;
      }

      var clusterNameRegex = new Regex(NAME_REGEX_VALIDATION);
      return clusterNameRegex.IsMatch(trimmedUserName)
        ? string.Empty
        : Resources.MySqlServerUsernameInvalid;
    }

    /// <summary>
    /// Checks if a connection to this instance can be established with the credentials in <see cref="UserAccount"/>.
    /// </summary>
    /// <param name="fallbackAuthenticationPlugin">Flag indicating if the connection must be retried with a different authentication plugin if it fails.</param>
    /// <returns>A <see cref="ConnectionResultType"/> value.</returns>
    public ConnectionResultType CanConnect(bool fallbackAuthenticationPlugin)
    {
      var connectionResult = CanConnect();
      if (connectionResult == ConnectionResultType.ConnectionError
          && fallbackAuthenticationPlugin
          && UserAccount.AuthenticationPlugin != MySqlAuthenticationPluginType.None
          && UserAccount.AuthenticationPlugin != MySqlAuthenticationPluginType.Windows)
      {
        var backupAccount = UserAccount;
        var fallbackAccount = UserAccount.Clone() as MySqlServerUser;
        if (fallbackAccount == null)
        {
          return connectionResult;
        }

        fallbackAccount.AuthenticationPlugin = UserAccount.AuthenticationPlugin == MySqlAuthenticationPluginType.CachingSha2Password
                                               || UserAccount.AuthenticationPlugin == MySqlAuthenticationPluginType.Sha256Password
          ? MySqlAuthenticationPluginType.MysqlNativePassword
          : MySqlAuthenticationPluginType.CachingSha2Password;
        UserAccount = fallbackAccount;
        connectionResult = CanConnect();
        UserAccount = backupAccount;
      }

      return connectionResult;
    }

    /// <summary>
    /// Checks if a connection to this instance can be established with the credentials in <see cref="UserAccount"/>.
    /// </summary>
    /// <returns>A <see cref="ConnectionResultType"/> value.</returns>
    public virtual ConnectionResultType CanConnect()
    {
      if (!IsUsernameValid)
      {
        return ConnectionResultType.InvalidUserName;
      }

      ConnectionResultType connectionResult;
      var mySqlConnection = new MySqlConnection(GetConnectionStringBuilder().ConnectionString);
      using (mySqlConnection)
      {
        try
        {
          mySqlConnection.Open();
          connectionResult = ConnectionResultType.ConnectionSuccess;
        }
        catch (MySqlException mySqlException)
        {
          switch (mySqlException.Number)
          {
            // Connection could not be made.
            case MYSQL_EXCEPTION_NUMBER_SERVER_UNREACHABLE:
              connectionResult = ConnectionResultType.HostUnreachable;
              break;

            // Wrong password.
            case MYSQL_EXCEPTION_NUMBER_WRONG_PASSWORD:
              connectionResult = ConnectionResultType.WrongPassword;
              break;

            // Password has expired so any statement can't be run before resetting the expired password.
            case MYSQL_EXCEPTION_NUMBER_EXPIRED_PASSWORD:
              connectionResult = ConnectionResultType.WrongPassword;
              break;

            // Any other code
            default:
              connectionResult = ConnectionResultType.ConnectionError;
              Logger.LogException(mySqlException);
              break;
          }
        }
        catch (Exception ex)
        {
          connectionResult = ConnectionResultType.ConnectionError;
          Logger.LogException(ex);
        }
      }

      return connectionResult;
    }

    /// <summary>
    /// Executes a query that returns a single value packed as an object.
    /// </summary>
    /// <param name="sqlQuery">A query that returns a single value.</param>
    /// <param name="error">An error message if an error occurred.</param>
    /// <returns>A single value packed as an object.</returns>
    public object ExecuteScalar(string sqlQuery, out string error)
    {
      error = null;
      object result = null;
      using (var connection = new MySqlConnection(GetConnectionStringBuilder().ConnectionString))
      {
        try
        {
          connection.Open();
          result = MySqlHelper.ExecuteScalar(connection, sqlQuery);
        }
        catch (Exception ex)
        {
          error = ex.Message;
        }
      }

      return result;
    }

    /// <summary>
    /// Executes a query that does not return any values.
    /// </summary>
    /// <param name="sqlQuery">A query that returns a single value.</param>
    /// <param name="error">An error message if an error occurred.</param>
    /// <returns>The number of affected records.</returns>
    public int ExecuteNonQuery(string sqlQuery, out string error)
    {
      error = null;
      int affectedRecordsCount = 0;
      using (var connection = new MySqlConnection(GetConnectionStringBuilder().ConnectionString))
      {
        try
        {
          connection.Open();
          affectedRecordsCount = MySqlHelper.ExecuteNonQuery(connection, sqlQuery);
        }
        catch (Exception ex)
        {
          error = ex.Message;
        }
      }

      return affectedRecordsCount;
    }

    /// <summary>
    /// Executes the given SQL scripts connecting to this instance.
    /// </summary>
    /// <param name="outputScriptToStatus">Flag indicating whether feedback about the scripts being executed is sent to the output.</param>
    /// <param name="sqlScripts">An array of SQL scripts to execute.</param>
    /// <returns>The number of scripts that executed successfully.</returns>
    public virtual int ExecuteScripts(bool outputScriptToStatus, params string[] sqlScripts)
    {
      if (sqlScripts.Length == 0
          || !IsUsernameValid)
      {
        return 0;
      }

      int successfulScriptsCount = 0;
      using (var connection = new MySqlConnection(GetConnectionStringBuilder().ConnectionString))
      {
        try
        {
          connection.Open();
        }
        catch
        {
          ReportStatus(string.Format(Resources.MySqlServerInstanceInfoExecuteScriptsCannotConnectError, NameWithVersion, Port));
          return 0;
        }

        var mySqlScript = new MySqlScript(connection);
        foreach (var sqlScript in sqlScripts.Where(sqlScript => !string.IsNullOrEmpty(sqlScript)))
        {
          try
          {
            if (outputScriptToStatus)
            {
              ReportStatus(string.Format("{0}{1}{2}{1}{1}", Resources.MySqlServerInstanceInfoExecutingScript, Environment.NewLine, sqlScript));
            }

            mySqlScript.Query = sqlScript;
            mySqlScript.Execute();
            successfulScriptsCount++;
            if (outputScriptToStatus)
            {
              ReportStatus(Resources.MySqlServerInstanceInfoExecuteScriptExecutionSuccess);
            }
          }
          catch (Exception e)
          {
            ReportStatus(string.Format(Resources.MySqlServerInstanceInfoExecuteScriptsExecutionError, e.Message, Environment.NewLine));
          }
        }
      }

      return successfulScriptsCount;
    }

    /// <summary>
    /// Gets the connection string builder used to establish a connection to this instance.
    /// </summary>
    /// <param name="schemaName">The name of the default schema to work with.</param>
    /// <returns>The connection string builder used to establish a connection to this instance.</returns>
    public virtual MySqlConnectionStringBuilder GetConnectionStringBuilder(string schemaName = null)
    {
      if (UserAccount == null)
      {
        return null;
      }

      var builder = new MySqlConnectionStringBuilder()
      {
        Server = string.IsNullOrEmpty(UserAccount.Host) ? MySqlServerUser.LOCALHOST : UserAccount.Host,
        DefaultCommandTimeout = 120,
        Pooling = false,
        UserID = UserAccount.Username,
        Password = UserAccount.Password,
        ConnectionProtocol = ConnectionProtocol
      };

      // Previous versions of Connector/NET had SslMode=None now SslMode=Required is the default value
      // and only works when using a SHA256 authentication plugin. For other plugins it needs to explicitly bet set to None.
      if (UserAccount.AuthenticationPlugin != MySqlAuthenticationPluginType.Sha256Password
          && UserAccount.AuthenticationPlugin != MySqlAuthenticationPluginType.CachingSha2Password)
      {
        builder.SslMode = MySqlSslMode.None;
      }

      if (!string.IsNullOrEmpty(schemaName))
      {
        builder.Database = schemaName;
      }

      switch (ConnectionProtocol)
      {
        case MySqlConnectionProtocol.Tcp:
          builder.Port = Port;
          break;

        case MySqlConnectionProtocol.Pipe:
        case MySqlConnectionProtocol.SharedMemory:
          builder.PipeName = PipeOrSharedMemoryName;
          break;
      }

      return builder;
    }

    /// <summary>
    /// Attempts to flush the binary log.
    /// </summary>
    public void FlushBinaryLogAndResetMaster()
    {
      ReportStatus(Resources.ServerInstanceFlushingBinaryLog);
      const string SQL1 = "SET SQL_LOG_BIN = 0;";
      const string SQL2 = "FLUSH NO_WRITE_TO_BINLOG BINARY LOGS;";
      const string SQL3 = "SET SQL_LOG_BIN = 1;";
      const string SQL4 = "RESET MASTER;";
      ReportStatus(ExecuteScripts(false, SQL1, SQL2, SQL3, SQL4) == 4
        ? Resources.ServerInstanceBinaryLogFlushedSuccess
        : Resources.ServerInstanceBinaryLogFlushedError);
    }

    /// <summary>
    /// Gets a value indicating the member count for the group replication cluster.
    /// </summary>
    /// <returns>A value indicating the member count for the group replication cluster.</returns>
    private int GetGroupReplicationMemberCount()
    {
      const string SQL = "SELECT COUNT(*) FROM `performance_schema`.`replication_group_members`";
      var count = ExecuteScalar(SQL, out var error);
      if (string.IsNullOrEmpty(error))
      {
        if (count == null)
        {
          return -1;
        }
        return Convert.ToInt32(count.ToString());
      }

      ReportStatus($"An error occurred when trying to retrieve the member count of the cluster: {error}");
      return -1;
    }

    /// <summary>
    /// Gets a value indicating the member role of this instance in a group replication cluster.
    /// </summary>
    /// <returns>A value indicating the member role of this instance in a group replication cluster.</returns>
    private GroupReplicationMemberRoleType GetGroupReplicationMemberRole()
    {
      const string SQL = "SELECT `member_role` FROM `performance_schema`.`replication_group_members` AS `rgmems` WHERE `rgmems`.`member_id`= @@server_uuid";
      var status = ExecuteScalar(SQL, out var error);
      if (string.IsNullOrEmpty(error))
      {
        if (status == null)
        {
          return GroupReplicationMemberRoleType.None;
        }

        return Enum.TryParse(status.ToString(), true, out GroupReplicationMemberRoleType parsed)
          ? parsed
          : GroupReplicationMemberRoleType.Unknown;
      }

      ReportStatus($"{Resources.ServerInstanceGetGroupReplicationMemberRoleError} {error}");
      return GroupReplicationMemberRoleType.Unknown;
    }

    /// <summary>
    /// Gets the value of the specified variable.
    /// </summary>
    /// <param name="name">The name of the variable.</param>
    /// <param name="isGlobal">Flag indicating if the variable is global.</param>
    /// <returns>A string representing the value of the specified variable.</returns>
    public object GetVariable(string name, bool isGlobal)
    {
      string sql = $"SELECT variable_value FROM performance_schema.{(isGlobal ? "global_variables" : "session_variables")} WHERE variable_name = '{name}'";
      var value = ExecuteScalar(sql, out var error);
      if (string.IsNullOrEmpty(error))
      {
        return value;
      }

      ReportStatus($"{string.Format(Resources.ServerInstanceGetVariableFail, name)} {error}");
      return null;
    }

    /// <summary>
    /// Sets the value of the specified variable.
    /// </summary>
    /// <param name="name">The name of the variable.</param>
    /// <param name="value">The value of the variable.</param>
    /// <param name="isGlobal">Flag indicating if the variable is global.</param>
    /// <returns><c>true</c> if setting the variable was successful; otherwise, <c>false</c>.</returns>
    public bool SetVariable(string name, object value, bool isGlobal)
    {
      string sql = $"SET {(isGlobal ? "GLOBAL" : string.Empty)} {name}={value}";
      ExecuteNonQuery(sql, out var error);
      if (string.IsNullOrEmpty(error))
      {
        return true;
      }

      ReportStatus($"{string.Format(Resources.ServerInstanceSetVariableFail, name)} {error}");
      return false;
    }

    /// <summary>
    /// Outputs a status message using the <seealso cref="ReportStatusDelegate"/>.
    /// </summary>
    /// <param name="statusMessage">The status message.</param>
    protected void ReportStatus(string statusMessage)
    {
      if (DisableReportStatus
          || ReportStatusDelegate == null
          || string.IsNullOrEmpty(statusMessage))
      {
        return;
      }

      ReportStatusDelegate(statusMessage);
    }
  }
}