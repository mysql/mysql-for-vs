// Copyright (c) 2016, Oracle and/or its affiliates. All rights reserved.
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
using System.Configuration;
using System.Text;
using MySql.Data.MySqlClient;
using MySql.Utility.Classes;
using MySql.Utility.Classes.MySqlX;
using MySql.Utility.Enums;

namespace MySql.Utility.Tests
{
  /// <summary>
  /// Contains a base implementation to connect up to three schemas for testing purposes.
  /// </summary>
  public abstract class SetUpDatabaseTestsBase : IDisposable
  {
    #region Constants

    /// <summary>
    /// Default MySQL host name.
    /// </summary>
    public const string DEFAULT_HOSTNAME = "localhost";

    /// <summary>
    /// Default MySQL memory name.
    /// </summary>
    public const string DEFAULT_MEMORYNAME = "MYSQL";

    /// <summary>
    /// Default password.
    /// </summary>
    public const string DEFAULT_PASSWORD = "test";

    /// <summary>
    /// Default MySQL pipe name.
    /// </summary>
    public const string DEFAULT_PIPENAME = "MYSQL";

    /// <summary>
    /// Default MySQL port.
    /// </summary>
    public const int DEFAULT_PORT = 3357;

    /// <summary>
    /// Default first schema name.
    /// </summary>
    public const string DEFAULT_SCHEMA_NAME_1 = "DumpTest";

    /// <summary>
    /// Default second database name.
    /// </summary>
    public const string DEFAULT_SCHEMA_NAME_2 = "SecondTest";

    /// <summary>
    /// Default third database name.
    /// </summary>
    public const string DEFAULT_SCHEMA_NAME_3 = "ThirdTest";

    /// <summary>
    /// Default MySQL user name.
    /// </summary>
    public const string DEFAULT_USER_NAME = "test";

    /// <summary>
    /// Default port for the X protocol.
    /// </summary>
    public const int DEFAULT_X_PORT = 33570;

    #endregion Constants

    #region Fields

    /// <summary>
    /// The connection string used for classic protocol.
    /// </summary>
    private string _connectionString;

    /// <summary>
    /// The connection string used for X protocol.
    /// </summary>
    private string _xConnectionString;

    #endregion Fields

    /// <summary>
    /// Initialize a new instance of the <see cref="SetUpDatabaseTestsBase"/> class.
    /// </summary>
    protected SetUpDatabaseTestsBase()
    {
      DropSchemasOnDispose = true;
      LoadConfiguration();
    }

    #region Properties

    /// <summary>
    /// Gets the connection string used for the classic protocol.
    /// </summary>
    public string ConnectionString
    {
      get
      {
        if (string.IsNullOrEmpty(_connectionString))
        {
          _connectionString = GetConnectionString(HostName, Port, UserName, Password, false, null);
        }

        return _connectionString;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether schemas are dropped from the database when the instance is disposed.
    /// </summary>
    public bool DropSchemasOnDispose { get; set; }

    /// <summary>
    /// Gets the host name used for the connection.
    /// </summary>
    public string HostName { get; protected set; }

    /// <summary>
    /// Gets the name used for memory connections.
    /// </summary>
    public string MemoryName { get; protected set; }

    /// <summary>
    /// Gets the password used for the connection.
    /// </summary>
    public string Password { get; protected set; }

    /// <summary>
    /// Gets the name used for pipe connections.
    /// </summary>
    public string PipeName { get; protected set; }

    /// <summary>
    /// Gets the port used for the connections using the classic protocol.
    /// </summary>
    public int Port { get; protected set; }

    /// <summary>
    /// Gets the first schema name.
    /// </summary>
    public string SchemaName1 { get; protected set; }

    /// <summary>
    /// Gets the second schema name.
    /// </summary>
    public string SchemaName2 { get; protected set; }

    /// <summary>
    /// Gets the third schema name.
    /// </summary>
    public string SchemaName3 { get; protected set; }

    /// <summary>
    /// Gets the user name used for the connection.
    /// </summary>
    public string UserName { get; protected set; }

    /// <summary>
    /// Gets connection string used for the X protocol.
    /// </summary>
    public string XConnectionString
    {
      get
      {
        if (string.IsNullOrEmpty(_xConnectionString))
        {
          _xConnectionString = GetXConnectionString(HostName, XPort, UserName, Password);
        }

        return _xConnectionString;
      }
    }

    /// <summary>
    /// Gets the port used for the connections using the X protocol.
    /// </summary>
    public int XPort { get; protected set; }

    #endregion Properties

    /// <summary>
    /// Returns a connection string for the classic protocol.
    /// </summary>
    /// <param name="hostName">The name of the host to connect to.</param>
    /// <param name="port">The port of the host.</param>
    /// <param name="userName">The user name.</param>
    /// <param name="password">The password.</param>
    /// <param name="persistSecurityInfo">Flag indicating whether the password is saved.</param>
    /// <param name="schemaName">The name of the schema to use.</param>
    /// <returns>A connection string for the classic protocol.</returns>
    public static string GetConnectionString(string hostName, int port, string userName, string password, bool persistSecurityInfo, string schemaName)
    {
      string connStr = string.Format("server={0};user id={1};pooling=false;persist security info={2};connection reset=true;allow user variables=true;port={3};",
          hostName,
          userName,
          persistSecurityInfo.ToString().ToLowerInvariant(),
          port);
      if (!string.IsNullOrEmpty(password))
        connStr += string.Format(";password={0};", password);
      if (!string.IsNullOrEmpty(schemaName))
        connStr += string.Format("database={0};", schemaName);
      return connStr;
    }

    /// <summary>
    /// Returns a sample JavaScript setup script.
    /// </summary>
    /// <returns></returns>
    public static string GetJavaScriptSetupScript()
    {
      return Utilities.GetScriptFromResource("MySql.Utility.Tests.Properties.SetupBaseShell.js");
    }

    /// <summary>
    /// Returns a sample Python setup script.
    /// </summary>
    /// <returns></returns>
    public static string GetPythonSetupScript()
    {
      return Utilities.GetScriptFromResource("MySql.Utility.Tests.Properties.SetupBaseShell.py");
    }

    /// <summary>
    /// Returns a sample SQL setup script.
    /// </summary>
    /// <returns></returns>
    public static string GetSqlSetupScript()
    {
      return Utilities.GetScriptFromResource("MySql.Utility.Tests.Properties.SetupBaseShell.sql");
    }

    /// <summary>
    /// Returns a connection string for the X protocol.
    /// </summary>
    /// <param name="hostName">The name of the host to connect to.</param>
    /// <param name="port">The port of the host.</param>
    /// <param name="userName">The user name.</param>
    /// <param name="password">The password.</param>
    /// <returns></returns>
    public static string GetXConnectionString(string hostName, int port, string userName, string password)
    {
      return string.Format("{0}:{1}@{2}:{3}", userName, password, hostName, port);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public virtual void Dispose()
    {
      if (DropSchemasOnDispose)
      {
        var queryStringBuilder = new StringBuilder();
        if (!string.IsNullOrEmpty(SchemaName1))
        {
          queryStringBuilder.AppendFormat("DROP DATABASE IF EXISTS {0};", SchemaName1);
        }

        if (!string.IsNullOrEmpty(SchemaName2))
        {
          queryStringBuilder.AppendFormat("DROP DATABASE IF EXISTS {0};", SchemaName2);
        }

        if (!string.IsNullOrEmpty(SchemaName3))
        {
          queryStringBuilder.AppendFormat("DROP DATABASE IF EXISTS {0};", SchemaName3);
        }

        var query = queryStringBuilder.ToString().Trim();
        if (string.IsNullOrEmpty(query))
        {
          return;
        }

        ExecuteSql(query);
      }
    }

    /// <summary>
    /// Executes the given script using the specified language.
    /// </summary>
    /// <param name="script">The script to execute.</param>
    /// <param name="scriptLanguage">The language used to execute the script.</param>
    public void ExecuteScript(string script, ScriptLanguageType scriptLanguage)
    {
      if (string.IsNullOrEmpty(script))
      {
        return;
      }

      switch (scriptLanguage)
      {
        case ScriptLanguageType.JavaScript:
          ExecuteJavaScript(script);
          break;

        case ScriptLanguageType.Python:
          ExecutePython(script);
          break;

        case ScriptLanguageType.Sql:
          ExecuteSql(script);
          break;
      }
    }

    /// <summary>
    /// Executes the given JavaScript script.
    /// </summary>
    /// <param name="script">The script to execute.</param>
    /// <returns>A list of result sets and execution information.</returns>
    protected List<Dictionary<string, object>> ExecuteJavaScript(string script)
    {
      var xProxy = new MySqlXProxy(XConnectionString, true, ScriptLanguageType.JavaScript);
      return xProxy.ExecuteSingleStatementAsResultObject(script, ScriptLanguageType.JavaScript);
    }

    /// <summary>
    /// Executes the given Python script.
    /// </summary>
    /// <param name="script">The script to execute.</param>
    /// <returns>A list of result sets and execution information.</returns>
    protected List<Dictionary<string, object>> ExecutePython(string script)
    {
      var xProxy = new MySqlXProxy(XConnectionString, true, ScriptLanguageType.Python);
      return xProxy.ExecuteSingleStatementAsResultObject(script, ScriptLanguageType.Python);
    }

    /// <summary>
    /// Executes the given script, replacing placeholders with actual schema names, using the specified language.
    /// </summary>
    /// <param name="script">The script to execute.</param>
    /// <param name="scriptLanguage">The language used to execute the script.</param>
    protected void ExecuteScriptReplacingSchemas(string script, ScriptLanguageType scriptLanguage)
    {
      if (string.IsNullOrEmpty(script))
      {
        return;
      }

      var scriptBuilder = new StringBuilder(script);
      scriptBuilder.Replace("{0}", SchemaName1);
      scriptBuilder.Replace("{1}", SchemaName2);
      scriptBuilder.Replace("{2}", SchemaName3);
      ExecuteScript(scriptBuilder.ToString(), scriptLanguage);
    }

    /// <summary>
    /// Executes the given SQL script.
    /// </summary>
    /// <param name="script">The script to execute.</param>
    /// <returns>The number of affected rows in the database.</returns>
    protected int ExecuteSql(string script)
    {
      var s = new MySqlScript(new MySqlConnection(ConnectionString), script);
      return s.Execute();
    }

    /// <summary>
    /// Loads connection settings from the configuration settings to the properties in this instance.
    /// </summary>
    protected void LoadConfiguration()
    {
      var appHost = ConfigurationManager.AppSettings["host"];
      var appUser = ConfigurationManager.AppSettings["user"];
      var appPassword = ConfigurationManager.AppSettings["password"];
      var appPort = ConfigurationManager.AppSettings["port"];
      var appPipeName = ConfigurationManager.AppSettings["pipename"];
      var appMemoryName = ConfigurationManager.AppSettings["memory_name"];
      var appDataBase = ConfigurationManager.AppSettings["database"];
      var appDataBase1 = ConfigurationManager.AppSettings["database1"];
      var appDataBase2 = ConfigurationManager.AppSettings["database2"];
      string xPortString = ConfigurationManager.AppSettings["x_port"];

      HostName = string.IsNullOrEmpty(appHost) ? DEFAULT_HOSTNAME : appHost;
      UserName = string.IsNullOrEmpty(appUser) ? DEFAULT_USER_NAME : appUser;
      Password = string.IsNullOrEmpty(appPassword) ? DEFAULT_PASSWORD : appPassword;
      Port = string.IsNullOrEmpty(appPort) ? DEFAULT_PORT : int.Parse(appPort);
      PipeName = string.IsNullOrEmpty(appPipeName) ? DEFAULT_PIPENAME : appPipeName;
      MemoryName = string.IsNullOrEmpty(appMemoryName) ? DEFAULT_MEMORYNAME : appMemoryName;
      SchemaName1 = string.IsNullOrEmpty(appDataBase) ? DEFAULT_SCHEMA_NAME_1 : appDataBase;
      SchemaName2 = string.IsNullOrEmpty(appDataBase) ? DEFAULT_SCHEMA_NAME_2 : appDataBase1;
      SchemaName3 = string.IsNullOrEmpty(appDataBase) ? DEFAULT_SCHEMA_NAME_3 : appDataBase2;
      XPort = string.IsNullOrEmpty(xPortString) ? DEFAULT_X_PORT : int.Parse(xPortString);
    }

    /// <summary>
    /// Returns a new script with placeholders for databases replaced with the actual schema names.
    /// </summary>
    /// <param name="script">The script.</param>
    /// <returns>A new script with placeholders for databases replaced with the actual schema names.</returns>
    protected string ReplaceSchemasInScript(string script)
    {
      if (string.IsNullOrEmpty(script))
      {
        return script;
      }

      var scriptBuilder = new StringBuilder(script);
      scriptBuilder.Replace("{0}", SchemaName1);
      scriptBuilder.Replace("{1}", SchemaName2);
      scriptBuilder.Replace("{2}", SchemaName3);
      return scriptBuilder.ToString();
    }
  }
}
