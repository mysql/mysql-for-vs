// Copyright © 2016, Oracle and/or its affiliates. All rights reserved.
//
// MySQL for Visual Studio is licensed under the terms of the GPLv2
// <http://www.gnu.org/licenses/old-licenses/gpl-2.0.html>, like most
// MySQL Connectors. There are special exceptions to the terms and
// conditions of the GPLv2 as it is applied to this software, see the
// FLOSS License Exception
// <http://www.mysql.com/about/legal/licensing/foss-exception.html>.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published
// by the Free Software Foundation; version 2 of the License.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License
// for more details.
//
// You should have received a copy of the GNU General Public License along
// with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using System;
using System.Data;
using MySql.Data.MySqlClient;
using MySql.Utility.Classes;
using MySql.Utility.Classes.MySqlX;
using MySql.Utility.Enums;
using MySql.Utility.Tests;
using Xunit;
using System.Collections.Generic;

namespace MySql.VisualStudio.Tests.MySqlX.Base
{
  public abstract class BaseTests : IUseFixture<SetUpXShell>, IDisposable
  {
    #region Constant Names

    /// <summary>
    /// SakilaX character table name.
    /// </summary>
    public const string CHARACTER_TABLE_NAME = "character";

    /// <summary>
    /// SakilaX movies collection name.
    /// </summary>
    public const string MOVIES_COLLECTION_NAME = "movies";

    /// <summary>
    /// Test schema name
    /// </summary>
    public const string TEMP_SCHEMA_NAME = "temp_schema";

    /// <summary>
    /// Database test name
    /// </summary>
    public const string TEMP_TEST_DATABASE_NAME = "temp_test";
    /// <summary>
    /// Table test name
    /// </summary>
    public const string TEST_COLLECTION_NAME = "test";

    /// <summary>
    /// SakilaX users collection name.
    /// </summary>
    public const string USERS_COLLECTION_NAME = "users";

    /// <summary>
    /// The test schema name for all tests.
    /// </summary>
    public const string X_TEST_SCHEMA_NAME = "x_test";

    #endregion Constant Names

    #region Common SQL Queries

    /// <summary>
    /// Statement to drop a given database if it exists.
    /// </summary>
    public const string DROP_DATABASE_IF_EXISTS = "session.sql('DROP DATABASE IF EXISTS `{0}`;').execute()";

    /// <summary>
    /// Statement to drop a given schema if it exists.
    /// </summary>
    public const string DROP_SCHEMA_IF_EXISTS = "session.sql('DROP SCHEMA IF EXISTS `{0}`;').execute()";

    /// <summary>
    /// Search for a created collection index in the information schema. Use: string.format(SEARCH_INDEX_SQL_SYNTAX, "schema", "collectionName", "indexName")
    /// </summary>
    protected const string SEARCH_INDEX_SQL_SYNTAX = "SELECT COUNT(*) FROM information_schema.INNODB_SYS_TABLES as `tables` join information_schema.INNODB_SYS_INDEXES as `indexes` where `tables`.TABLE_ID = `indexes`.TABLE_ID AND `tables`.NAME = '{0}/{1}' AND `indexes`.NAME = '{2}';";

    /// <summary>
    /// Search for a table in the schema.tables information. Use: string.format(SEARCH_TABLE_SQL_SYNTAX, "myTable", "myDatabase")
    /// </summary>
    protected const string SEARCH_TABLE_SQL_SYNTAX = "SELECT COUNT(*) FROM information_schema.tables WHERE table_name='{0}' AND table_schema='{1}';";

    /// <summary>
    /// Sql statement to select the current databases in the server
    /// </summary>
    protected const string SHOW_DBS_SQL_SYNTAX = "SHOW DATABASES;";

    #endregion Common SQL Queries

    #region Assert Fail Messages

    /// <summary>
    /// Message to display when a collection is not deleted. Usage: string.format(COLLECTION_NOT_DELETED, "myTable")
    /// </summary>
    protected const string COLLECTION_NOT_DELETED = "Collection {0} was not deleted";

    /// <summary>
    /// Message to display when a collection is not found. Usage: string.format(COLLECTION_NOT_FOUND, "myTable")
    /// </summary>
    protected const string COLLECTION_NOT_FOUND = "Collection {0} not found";

    /// <summary>
    /// Message to display when the data received doesn't match the data expected
    /// </summary>
    protected const string DATA_NOT_MATCH = "Data doesn't match";

    /// <summary>
    /// Message to display when duplicate data could be inserted meaning a unique index is not working.
    /// </summary>
    protected const string DATA_NOT_UNIQUE = "Duplicate data found, unique index not working.";

    /// <summary>
    /// Message to display when a database is not deleted. Usage: string.format(DB_NOT_DELETED, "myDatabase").
    /// </summary>
    protected const string DB_NOT_DELETED = "DB {0} was not deleted";

    /// <summary>
    /// Message to display when a database is not found. Usage: string.format(DB_NOT_FOUND, "myDatabase").
    /// </summary>
    protected const string DB_NOT_FOUND = "DB {0} not found";
    /// <summary>
    /// Message to display when a schema is not found. Usage: string.format(INDEX_NOT_FOUND, "indexName")
    /// </summary>
    protected const string INDEX_NOT_FOUND = "Index {0} not found";

    /// <summary>
    /// Message to display when an object is null. Usage: string.format(NULL_OBJECT, "myObject")
    /// </summary>
    protected const string NULL_OBJECT = "The object {0} is null";

    /// <summary>
    /// Message to display when a schema is not found. Usage: string.format(SCHEMA_NOT_DELETED, "myDatabase")
    /// </summary>
    protected const string SCHEMA_NOT_DELETED = "Schema {0} was not deleted";

    /// <summary>
    /// Message to display when a schema is not found. Usage: string.format(SCHEMA_NOT_FOUND, "myDatabase")
    /// </summary>
    protected const string SCHEMA_NOT_FOUND = "Schema {0} not found";

    /// <summary>
    /// Message to display when a table is not deleted. Usage: string.format(TABLE_NOT_DELETED, "myTable").
    /// </summary>
    protected const string TABLE_NOT_DELETED = "Table {0} was not deleted";

    /// <summary>
    /// Message to display when a table is not found. Usage: string.format(TABLE_NOT_FOUND, "myTable").
    /// </summary>
    protected const string TABLE_NOT_FOUND = "Table {0} not found";

    #endregion Assert Fail Messages

    #region Fields

    /// <summary>
    /// Gets or sets the <see cref="MySqlXProxy"/> to execute commands using the X Protocol.
    /// </summary>
    private MySqlXProxy _xProxy;

    /// <summary>
    /// Gets or sets the <see cref="MySqlShellClient"/> to execute commands using the X Protocol.
    /// </summary>
    private MySqlShellClient _xShellClient;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseTests"/> class.
    /// </summary>
    /// <param name="scriptLanguage">The language used for the tests.</param>
    /// <param name="xecutor">The type of class that will run X Protocol statements.</param>
    protected BaseTests(ScriptLanguageType scriptLanguage, XecutorType xecutor)
    {
      ScriptLanguage = scriptLanguage;
      Xecutor = xecutor;
    }

    #region Properties

    /// <summary>
    /// Gets the <see cref="MySqlCommand"/> to execute statements.
    /// </summary>
    public MySqlCommand Command { get; protected set; }

    /// <summary>
    /// Gets the <see cref="MySqlConnection"/> to connect to a server.
    /// </summary>
    public MySqlConnection Connection { get; protected set; }

    /// <summary>
    /// Gets the language used for the tests.
    /// </summary>
    public ScriptLanguageType ScriptLanguage { get; protected set; }

    /// <summary>
    /// Gets the object that stores and accesses the current configuration for the test database.
    /// </summary>
    public SetUpXShell SetUp { get; protected set; }

    /// <summary>
    /// Gets thee connection string format used by the X Protocol;
    /// </summary>
    public string XConnString { get; protected set; }

    /// <summary>
    /// Gets the type of class that will run X Protocol statements.
    /// </summary>
    public XecutorType Xecutor { get; protected set; }

    #endregion Properties

    /// <summary>
    /// Dispose implementation of the current class
    /// </summary>
    public void Dispose()
    {
      if (SetUp != null)
      {
        SetUp.Dispose();
      }

      if (Connection != null)
      {
        Connection.Dispose();
      }

      if (Command != null)
      {
        Command.Dispose();
      }
    }

    /// <summary>
    /// Setup information about the current schema test.
    /// </summary>
    /// <param name="data">Current test instance configuration.</param>
    public virtual void SetFixture(SetUpXShell data)
    {
      SetUp = data;
      Connection = new MySqlConnection(SetUpDatabaseTestsBase.GetConnectionString(SetUp.HostName, SetUp.Port, SetUp.UserName, SetUp.Password, false, null));
      XConnString = string.Format("{0}:{1}@{2}:{3}", SetUp.UserName, SetUp.Password, SetUp.HostName, SetUp.XPort);
    }

    /// <summary>
    /// Close a MySqlConnection when it is not closed.
    /// </summary>
    protected virtual void CloseConnection()
    {
      if (Connection.State != ConnectionState.Closed)
      {
        Connection.Close();
      }
    }

    /// <summary>
    /// Frees resources on the specified <see cref="Xecutor"/>.
    /// </summary>
    protected void DisposeXecutor()
    {
      switch (Xecutor)
      {
        case XecutorType.XProxy:
          DisposeXProxy();
          break;

        case XecutorType.XShell:
          DisposeXShellClient();
          break;
      }
    }

    /// <summary>
    /// Executes a query using the specified <see cref="Xecutor"/>.
    /// </summary>
    /// <param name="statement">A statement.</param>
    /// <returns>An object with the result of the execution.</returns>
    protected object ExecuteQuery(string statement)
    {
      switch(Xecutor)
      {
        case XecutorType.XProxy:
          return _xProxy.ExecuteQuery(statement, ScriptLanguage);

        case XecutorType.XShell:
          switch (ScriptLanguage)
          {
            case ScriptLanguageType.JavaScript:
              return _xShellClient.ExecuteToJavaScript(statement);

            case ScriptLanguageType.Python:
              return _xShellClient.Execute(statement);
          }

          break;
      }

      return null;
    }

    /// <summary>
    /// Executes a query using the specified <see cref="Xecutor"/>.
    /// </summary>
    /// <param name="statement">A statement.</param>
    /// <returns>A list of dictionaries with the result of the execution.</returns>
    protected List<Dictionary<string, object>> ExecuteSingleStatement(string statement)
    {
      switch (Xecutor)
      {
        case XecutorType.XProxy:
          return _xProxy.ExecuteSingleStatement(statement, ScriptLanguage);

        case XecutorType.XShell:
          object result = null;
          switch (ScriptLanguage)
          {
            case ScriptLanguageType.JavaScript:
              result = _xShellClient.ExecuteToJavaScript(statement);
              break;

            case ScriptLanguageType.Python:
              result = _xShellClient.Execute(statement);
              break;
          }

          string executionTime;
          return result.ToDictionariesList(out executionTime);
      }

      return null;
    }

    /// <summary>
    /// Initializes the specified <see cref="Xecutor"/>.
    /// </summary>
    protected void InitXecutor()
    {
      switch (Xecutor)
      {
        case XecutorType.XProxy:
          InitXProxy();
          break;

        case XecutorType.XShell:
          InitXShellClient();
          break;
      }
    }

    /// <summary>
    /// Frees resources on the <see cref="_xProxy"/>.
    /// </summary>
    private void DisposeXProxy()
    {
      if (_xProxy != null)
      {
        _xProxy.CleanConnection();
      }
    }

    /// <summary>
    /// Frees resources on the <see cref="_xShellClient"/>.
    /// </summary>
    private void DisposeXShellClient()
    {
      if (_xShellClient != null)
      {
        _xShellClient.Dispose();
      }
    }

    /// <summary>
    /// Initializes the <see cref="MySqlXProxy"/> instance with common statements
    /// </summary>
    private void InitXProxy()
    {
      DisposeXProxy();
      _xProxy = new MySqlXProxy(XConnString, true, ScriptLanguage);
    }

    /// <summary>
    /// Initializes the <see cref="MySqlShellClient"/> instance with common statements.
    /// </summary>
    private void InitXShellClient()
    {
      DisposeXShellClient();
      _xShellClient = new MySqlShellClient();
      _xShellClient.MakeConnection(XConnString);
      _xShellClient.SwitchMode(ScriptLanguage.ToMode());
      _xShellClient.AppendAdditionalModulePaths(ScriptLanguage);
    }

    /// <summary>
    /// Open a MySqlConnection when it is not opened.
    /// </summary>
    protected virtual void OpenConnection()
    {
      if (Connection.State != ConnectionState.Open)
      {
        Connection.Open();
      }
    }
  }
}

