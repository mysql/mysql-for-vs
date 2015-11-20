// Copyright © 2015, Oracle and/or its affiliates. All rights reserved.
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
using System.Text;
using MySqlX.Shell;
using Xunit;
using MySql.Data.MySqlClient;
using MySql.Data.VisualStudio.Editors;
using MySqlX;
using MySQL.Utility.Classes;

namespace MySql.VisualStudio.Tests
{
  /// <summary>
  /// Class to test the CRUD operations through the NgShell Wrapper on Relational DB
  /// </summary>
  public class TablexShellTests : IUseFixture<SetUpXShell>, IDisposable
  {
    #region CommonShellQueries
    /// <summary>
    /// Get and set the mysqlx protocol instance
    /// </summary>
    private const string _setMysqlxVar = "var mysqlx = require('mysqlx').mysqlx;";
    /// <summary>
    /// Get and set the node session from the mysqlx protocol instance
    /// </summary>
    private const string _setSessionVar = "var session = mysqlx.getNodeSession('root:@localhost:33060');";
    /// <summary>
    /// Database test name
    /// </summary>
    private const string _testDatabaseName = "js_shell_test";
    /// <summary>
    /// Statement to drop the test database
    /// </summary>
    private const string _dropTestDatabase = "session.sql('drop schema if exists " + _testDatabaseName + ";').execute();";
    /// <summary>
    /// Statement to create the test database
    /// </summary>
    private const string _createTestDatabase = "session.sql('create schema " + _testDatabaseName + ";').execute();";
    /// <summary>
    /// Statement to use the test database
    /// </summary>
    private const string _useTestDatabase = "session.sql('use " + _testDatabaseName + ";').execute();";
    /// <summary>
    /// Table test name
    /// </summary>
    private const string _testTableName = "xshelltest";
    /// <summary>
    /// Statement to create the test table
    /// </summary>
    private const string _createTestTable = "session.sql('create table xshelltest (name varchar(50), age integer, gender varchar(20));').execute();";
    /// <summary>
    /// Statement to delete the test table
    /// </summary>
    private const string _deleteTestTable = "session.sql('drop table xshelltest;').execute();";
    /// <summary>
    /// Get and set the test database
    /// </summary>
    private const string _setSchemaVar = "var schema = session.getSchema('" + _testDatabaseName + "');";
    /// <summary>
    /// Get and set the test table
    /// </summary>
    private const string _setTableVar = "var table = schema.getTable('xshelltest');";
    /// <summary>
    /// Statement to insert two records at the same time to the test table
    /// </summary>
    private const string _insertTwoRecords = "var res = table.insert('name', 'age', 'gender').values('jack', 17,'male').values('jacky', 17,'male').execute();";
    /// <summary>
    /// Statement to insert a record to the test table
    /// </summary>
    private const string _insertRecordJson1 = "var res = table.insert({name: 'jack', age: 17, gender: 'male'}).execute();";
    /// <summary>
    /// Statement to insert a record to the test table
    /// </summary>
    private const string _insertRecordJson2 = "var res = table.insert({name: 'jacky', age: 17, gender: 'male'}).execute();";
    /// <summary>
    /// Statement to get all the records from the test table as RowResult
    /// </summary>
    private const string _selectTestTable = "table.select().execute();";
    /// <summary>
    /// Statement to get all the records from the test table as RowResult
    /// </summary>
    private const string _selectForTableResult = "table.select().execute();";
    /// <summary>
    /// Statement to update a record in the test table in a single command
    /// </summary>
    private const string _updateRecordSingleLine = "var res = table.update().set('gender', 'female').where(\"name = 'jacky'\").execute();";
    /// <summary>
    /// First statement to update a record in the test table in multiple commands
    /// </summary>
    private const string _updateRecordCmd1 = "var upd = table.update();";
    /// <summary>
    /// Second statement to update a record in the test table in multiple commands
    /// </summary>
    private const string _updateRecordCmd2 = "upd.set('gender', 'female').where(\"name = 'jacky'\");";
    /// <summary>
    /// Third statement to update a record in the test table in multiple commands
    /// </summary>
    private const string _updateRecordCmd3 = "upd.execute();";
    /// <summary>
    /// Statement to select the update record from the test table
    /// </summary>
    private const string _selectUpdatedRecord = "table.select().where(\"name = 'jacky' and gender='female'\").execute();";
    /// <summary>
    /// Statement to delete a record in the test table in a single command
    /// </summary>
    private const string _deleteRecordSingleLine = "var res = table.delete().where(\"gender='male'\").execute();";
    /// <summary>
    /// First statement to delete a record in the test table in multiple commands
    /// </summary>
    private const string _deleteRecordCmd1 = "var del = table.delete();";
    /// <summary>
    /// Second statement to delete a record in the test table in multiple commands
    /// </summary>
    private const string _deleteRecordCmd2 = "del.where(\"gender='male'\");";
    /// <summary>
    /// Third statement to delete a record in the test table in multiple commands
    /// </summary>
    private const string _deleteRecordCmd3 = "del.execute();";
    #endregion

    #region CommonAssertQueries
    /// <summary>
    /// Sql statement to select the current databases in the server
    /// </summary>
    private const string _showDbs = "show databases;";
    /// <summary>
    /// Search for a table in the schema.tables information. Use: string.format(_searchTable, "myTable", "myDatabase")
    /// </summary>
    private const string _searchTable = "select count(*) from information_schema.TABLES where table_name='{0}' and table_schema='{1}';";
    /// <summary>
    /// Sql statement to drop the test database
    /// </summary>
    private const string _dropTestDbSqlSyntax = "drop schema if exists " + _testDatabaseName + ";";
    #endregion

    #region AssertFailMessages
    /// <summary>
    /// Message to display when a database is not found. Usage: string.format(_dbNotFound, "myDatabase")
    /// </summary>
    private const string _dbNotFound = "DB {0} not found";
    /// <summary>
    /// Message to display when a table is not found. Usage: string.format(_tableNotFound, "myTable")
    /// </summary>
    private const string _tableNotFound = "Table {0} not found";
    /// <summary>
    /// Message to display when a table is not deleted. Usage: string.format(_tableNotDeleted, "myTable")
    /// </summary>
    private const string _tableNotDeleted = "Table {0} was not deleted";
    /// <summary>
    /// Message to display when an object is null. Usage: string.format(_nullObject, "myObject")
    /// </summary>
    private const string _nullObject = "The object {0} is null";
    /// <summary>
    /// Message to display when the data received doesn't match the data expected
    /// </summary>
    private const string _dataNotMatch = "Data doesn't match";
    #endregion

    /// <summary>
    /// Object to store and access the current configuration for the test database
    /// </summary>
    private SetUpXShell _setUp;
    /// <summary>
    /// Object to store and get access to a MySqlConnection in the current server
    /// </summary>
    private MySqlConnection _connection;
    /// <summary>
    /// Object to execute commands to the current server connection
    /// </summary>
    private MySqlCommand _command;
    /// <summary>
    /// Object to access and execute commands to the current database connection through the mysqlx protocol
    /// </summary>
    private MySimpleClientShell _ngShell;
    /// <summary>
    /// Stores the connection string format used by the mysqlx protocol
    /// </summary>
    private string _ngConnString;

    /// <summary>
    /// Setup information about the current database test
    /// </summary>
    /// <param name="data">Current test instance configuration</param>
    public void SetFixture(SetUpXShell data)
    {
      _setUp = data;
      _connection = new MySqlConnection(_setUp.GetConnectionString(_setUp.rootUser, _setUp.rootPassword, false, false));
      _ngConnString = string.Format("{0}:{1}@{2}:{3}", _setUp.rootUser, _setUp.rootPassword, _setUp.host, 33060);
    }

    /// <summary>
    /// Test to create a Database using the NgWrapper
    /// </summary>
    [Fact]
    public void CreateDatabase_XShellDirectly()
    {
      OpenConnection();

      try
      {
        InitNgShell();
        _ngShell.Execute(_dropTestDatabase);
        _ngShell.Execute(_createTestDatabase);
        _command = new MySqlCommand(_showDbs, _connection);
        var reader = _command.ExecuteReader();
        bool success = false;

        while (reader.Read())
        {
          if (reader.GetString(0) == _testDatabaseName)
          {
            success = true;
            reader.Close();
            break;
          }
        }

        Assert.True(success, string.Format(_dbNotFound, _testDatabaseName));
      }
      catch (Exception ex)
      {
        throw ex;
      }
      finally
      {
        _setUp.ExecuteSQLAsRoot(_dropTestDbSqlSyntax);
        CloseConnection();
      }
    }

    /// <summary>
    /// Test to create a Table using the NgWrapper
    /// </summary>
    [Fact]
    public void CreateTable_XShellDirectly()
    {
      OpenConnection();

      try
      {
        InitNgShell();

        _ngShell.Execute(_dropTestDatabase);
        _ngShell.Execute(_createTestDatabase);
        _ngShell.Execute(_useTestDatabase);
        _ngShell.Execute(_createTestTable);
        _command = new MySqlCommand(string.Format(_searchTable, _testTableName, _testDatabaseName), _connection);
        var result = _command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(_tableNotFound, _testTableName));

        _ngShell.Execute(_deleteTestTable);
        result = _command.ExecuteScalar();
        int.TryParse(result.ToString(), out count);
        Assert.True(count == 0, string.Format(_tableNotDeleted, _testTableName));
      }
      catch (Exception ex)
      {
        throw ex;
      }
      finally
      {
        _setUp.ExecuteSQLAsRoot(_dropTestDbSqlSyntax);
        CloseConnection();
      }
    }

    /// <summary>
    /// Test to Insert, Update and Delete data from a table using the NgWrapper, executing the commands in a single line
    /// </summary>
    [Fact]
    public void InsertUpdateDelete_XShellDirectly()
    {
      OpenConnection();

      try
      {
        InitNgShell();
        _ngShell.Execute(_dropTestDatabase);
        _ngShell.Execute(_createTestDatabase);
        _ngShell.Execute(_useTestDatabase);
        _ngShell.Execute(_createTestTable);
        _command = new MySqlCommand(string.Format(_searchTable, _testTableName, _testDatabaseName), _connection);

        var result = _command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(_tableNotFound, _testTableName));

        _ngShell.Execute(_setSchemaVar);
        _ngShell.Execute(_setTableVar);
        _ngShell.Execute(_insertTwoRecords);
        var selectResult = _ngShell.Execute(_selectTestTable) as RowResult;

        Assert.True(selectResult != null, string.Format(_nullObject, "selectResult"));
        Assert.True(selectResult.FetchAll().Count == 2, _dataNotMatch);

        _ngShell.Execute(_updateRecordSingleLine);
        selectResult = _ngShell.Execute(_selectUpdatedRecord) as RowResult;

        Assert.True(selectResult != null, string.Format(_nullObject, "selectResult"));
        Assert.True(selectResult.FetchAll().Count == 1, _dataNotMatch);

        _ngShell.Execute(_deleteRecordSingleLine);
        selectResult = _ngShell.Execute(_selectTestTable) as RowResult;

        Assert.True(selectResult != null, string.Format(_nullObject, "selectResult"));
        Assert.True(selectResult.FetchAll().Count == 1, _dataNotMatch);
      }
      catch (Exception ex)
      {
        throw ex;
      }
      finally
      {
        _setUp.ExecuteSQLAsRoot(_dropTestDbSqlSyntax);
        CloseConnection();
      }
    }

    /// <summary>
    /// Test to Insert, Update and Delete data from a table using the NgWrapper, executing the commands in multiple lines
    /// </summary>
    [Fact]
    public void InsertUpdateDelete_JsonFormat_XShellDirectly()
    {
      OpenConnection();

      try
      {
        InitNgShell();
        _ngShell.Execute(_dropTestDatabase);
        _ngShell.Execute(_createTestDatabase);
        _ngShell.Execute(_useTestDatabase);
        _ngShell.Execute(_createTestTable);
        _command = new MySqlCommand(string.Format(_searchTable, _testTableName, _testDatabaseName), _connection);

        var result = _command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(_tableNotFound, _testTableName));

        _ngShell.Execute(_setSchemaVar);
        _ngShell.Execute(_setTableVar);
        _ngShell.Execute(_insertRecordJson1);
        _ngShell.Execute(_insertRecordJson2);
        var selectResult = _ngShell.Execute(_selectTestTable) as RowResult;

        Assert.True(selectResult != null, string.Format(_nullObject, "selectResult"));
        Assert.True(selectResult.FetchAll().Count == 2, _dataNotMatch);

        _ngShell.Execute(_updateRecordCmd1);
        _ngShell.Execute(_updateRecordCmd2);
        _ngShell.Execute(_updateRecordCmd3);
        selectResult = _ngShell.Execute(_selectUpdatedRecord) as RowResult;

        Assert.True(selectResult != null, string.Format(_nullObject, "selectResult"));
        Assert.True(selectResult.FetchAll().Count == 1, _dataNotMatch);

        _ngShell.Execute(_deleteRecordCmd1);
        _ngShell.Execute(_deleteRecordCmd2);
        _ngShell.Execute(_deleteRecordCmd3);
        selectResult = _ngShell.Execute(_selectTestTable) as RowResult;

        Assert.True(selectResult != null, string.Format(_nullObject, "selectResult"));
        Assert.True(selectResult.FetchAll().Count == 1, _dataNotMatch);
      }
      catch (Exception ex)
      {
        throw ex;
      }
      finally
      {
        _setUp.ExecuteSQLAsRoot(_dropTestDbSqlSyntax);
        CloseConnection();
      }
    }

    /// <summary>
    /// Test to create a Database using our custom implementation of the NgWrapper
    /// </summary>
    [Fact]
    public void CreateDatabase_CustomXShell()
    {
      OpenConnection();

      try
      {
        var xshell = new NgShellWrapper(_ngConnString, true);
        xshell.ExecuteScript(_setMysqlxVar, ScriptType.JavaScript);
        xshell.ExecuteScript(_setSessionVar, ScriptType.JavaScript);
        xshell.ExecuteScript(_dropTestDatabase, ScriptType.JavaScript);
        xshell.ExecuteScript(_createTestDatabase, ScriptType.JavaScript);
        _command = new MySqlCommand(_showDbs, _connection);
        var reader = _command.ExecuteReader();
        bool success = false;
        while (reader.Read())
        {
          if (reader.GetString(0) == _testDatabaseName)
          {
            success = true;
            reader.Close();
            break;
          }
        }
        Assert.True(success, string.Format(_dbNotFound, _testDatabaseName));
      }
      catch (Exception ex)
      {
        throw ex;
      }
      finally
      {
        _setUp.ExecuteSQLAsRoot(_dropTestDbSqlSyntax);
        CloseConnection();
      }
    }

    /// <summary>
    /// Test to create a Table using our custom implementation of the NgWrapper
    /// </summary>
    [Fact]
    public void CreateTable_CustomXShell()
    {
      OpenConnection();

      try
      {
        var xshell = new NgShellWrapper(_ngConnString, true);
        xshell.ExecuteScript(_setMysqlxVar, ScriptType.JavaScript);
        xshell.ExecuteScript(_setSessionVar, ScriptType.JavaScript);
        xshell.ExecuteScript(_dropTestDatabase, ScriptType.JavaScript);
        xshell.ExecuteScript(_createTestDatabase, ScriptType.JavaScript);
        xshell.ExecuteScript(_useTestDatabase, ScriptType.JavaScript);
        xshell.ExecuteScript(_createTestTable, ScriptType.JavaScript);
        _command = new MySqlCommand(string.Format(_searchTable, _testTableName, _testDatabaseName), _connection);
        var result = _command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(_tableNotFound, _testTableName));

        xshell.ExecuteScript(_deleteTestTable, ScriptType.JavaScript);
        result = _command.ExecuteScalar();
        int.TryParse(result.ToString(), out count);
        Assert.True(count == 0, string.Format(_tableNotDeleted, _testTableName));
      }
      catch (Exception ex)
      {
        throw ex;
      }
      finally
      {
        _setUp.ExecuteSQLAsRoot(_dropTestDbSqlSyntax);
        CloseConnection();
      }
    }

    /// <summary>
    /// Test to Insert, Update and Delete record from a table using our custom implementation of the NgWrapper, executing the commands in a single line
    /// </summary>
    [Fact]
    public void InsertUpdateDelete_CustomXShell()
    {
      OpenConnection();

      try
      {
        var xshell = new NgShellWrapper(_ngConnString, true);
        xshell.ExecuteScript(_setMysqlxVar, ScriptType.JavaScript);
        xshell.ExecuteScript(_setSessionVar, ScriptType.JavaScript);
        xshell.ExecuteScript(_dropTestDatabase, ScriptType.JavaScript);
        xshell.ExecuteScript(_createTestDatabase, ScriptType.JavaScript);
        xshell.ExecuteScript(_useTestDatabase, ScriptType.JavaScript);
        xshell.ExecuteScript(_createTestTable, ScriptType.JavaScript);
        _command = new MySqlCommand(string.Format(_searchTable, _testTableName, _testDatabaseName), _connection);

        var result = _command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(_tableNotFound, _testTableName));

        xshell.ExecuteScript(_setSchemaVar, ScriptType.JavaScript);
        xshell.ExecuteScript(_setTableVar, ScriptType.JavaScript);
        xshell.ExecuteScript(_insertTwoRecords, ScriptType.JavaScript);
        var selectResult = xshell.ExecuteScript(_selectTestTable, ScriptType.JavaScript);

        Assert.True(selectResult != null, string.Format(_nullObject, "selectResult"));
        Assert.True(selectResult.Count == 2, _dataNotMatch);

        xshell.ExecuteScript(_updateRecordSingleLine, ScriptType.JavaScript);
        selectResult = xshell.ExecuteScript(_selectUpdatedRecord, ScriptType.JavaScript);

        Assert.True(selectResult != null, string.Format(_nullObject, "selectResult"));
        Assert.True(selectResult.Count == 1, _dataNotMatch);

        xshell.ExecuteScript(_deleteRecordSingleLine, ScriptType.JavaScript);
        selectResult = xshell.ExecuteScript(_selectTestTable, ScriptType.JavaScript);

        Assert.True(selectResult != null, string.Format(_nullObject, "selectResult"));
        Assert.True(selectResult.Count == 1, _dataNotMatch);
      }
      catch (Exception ex)
      {
        throw ex;
      }
      finally
      {
        _setUp.ExecuteSQLAsRoot(_dropTestDbSqlSyntax);
        CloseConnection();
      }
    }

    /// <summary>
    /// Test to Insert, Update and Delete record from a table using our custom implementation of the NgWrapper, executing the commands in multiple lines
    /// </summary>
    [Fact]
    public void InsertUpdateDelete_JsonFormat_CustomXShell()
    {
      OpenConnection();

      try
      {
        var xshell = new NgShellWrapper(_ngConnString, true);
        xshell.ExecuteScript(_setMysqlxVar, ScriptType.JavaScript);
        xshell.ExecuteScript(_setSessionVar, ScriptType.JavaScript);
        xshell.ExecuteScript(_dropTestDatabase, ScriptType.JavaScript);
        xshell.ExecuteScript(_createTestDatabase, ScriptType.JavaScript);
        xshell.ExecuteScript(_useTestDatabase, ScriptType.JavaScript);
        xshell.ExecuteScript(_createTestTable, ScriptType.JavaScript);
        _command = new MySqlCommand(string.Format(_searchTable, _testTableName, _testDatabaseName), _connection);

        var result = _command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(_tableNotFound, _testTableName));

        xshell.ExecuteScript(_setSchemaVar, ScriptType.JavaScript);
        xshell.ExecuteScript(_setTableVar, ScriptType.JavaScript);
        xshell.ExecuteScript(_insertRecordJson1, ScriptType.JavaScript);
        xshell.ExecuteScript(_insertRecordJson2, ScriptType.JavaScript);
        var selectResult = xshell.ExecuteScript(_selectTestTable, ScriptType.JavaScript);

        Assert.True(selectResult != null, string.Format(_nullObject, "selectResult"));
        Assert.True(selectResult.Count == 2, _dataNotMatch);

        xshell.ExecuteScript(_updateRecordCmd1, ScriptType.JavaScript);
        xshell.ExecuteScript(_updateRecordCmd2, ScriptType.JavaScript);
        xshell.ExecuteScript(_updateRecordCmd3, ScriptType.JavaScript);
        selectResult = xshell.ExecuteScript(_selectUpdatedRecord, ScriptType.JavaScript);

        Assert.True(selectResult != null, string.Format(_nullObject, "selectResult"));
        Assert.True(selectResult.Count == 1, _dataNotMatch);

        xshell.ExecuteScript(_deleteRecordCmd1, ScriptType.JavaScript);
        xshell.ExecuteScript(_deleteRecordCmd2, ScriptType.JavaScript);
        xshell.ExecuteScript(_deleteRecordCmd3, ScriptType.JavaScript);
        selectResult = xshell.ExecuteScript(_selectTestTable, ScriptType.JavaScript);

        Assert.True(selectResult != null, string.Format(_nullObject, "selectResult"));
        Assert.True(selectResult.Count == 1, _dataNotMatch);
      }
      catch (Exception ex)
      {
        throw ex;
      }
      finally
      {
        _setUp.ExecuteSQLAsRoot(_dropTestDbSqlSyntax);
        CloseConnection();
      }
    }

    /// <summary>
    /// Test to Insert, Update and Delete record from a table using our custom implementation of the NgWrapper, executing the commands in a single line and in a single script
    /// </summary>
    [Fact]
    public void InsertUpdateDelete_SingleScript_CustomXShell()
    {
      OpenConnection();

      try
      {
        var xshell = new NgShellWrapper(_ngConnString, true);
        xshell.ExecuteScript(_setMysqlxVar, ScriptType.JavaScript);
        xshell.ExecuteScript(_setSessionVar, ScriptType.JavaScript);
        var script = new StringBuilder();
        script.AppendLine(_dropTestDatabase);
        script.AppendLine(_createTestDatabase);
        script.AppendLine(_useTestDatabase);
        script.AppendLine(_createTestTable);

        script.AppendLine(_setSchemaVar);
        script.AppendLine(_setTableVar);
        script.AppendLine(_insertTwoRecords);
        script.AppendLine(_updateRecordSingleLine);
        script.AppendLine(_deleteRecordSingleLine);

        var tokenizer = new MyJsTokenizer(script.ToString());
        xshell.ExecuteScript(tokenizer.BreakIntoStatements().ToArray(), ScriptType.JavaScript);

        _command = new MySqlCommand(string.Format(_searchTable, _testTableName, _testDatabaseName), _connection);

        var result = _command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(_tableNotFound, _testTableName));

        var selectResult = xshell.ExecuteScript(_selectTestTable, ScriptType.JavaScript);
        Assert.True(selectResult != null, string.Format(_nullObject, "selectResult"));
        Assert.True(selectResult.Count == 1, _dataNotMatch);
      }
      catch (Exception ex)
      {
        throw ex;
      }
      finally
      {
        _setUp.ExecuteSQLAsRoot(_dropTestDbSqlSyntax);
        CloseConnection();
      }
    }

    /// <summary>
    /// Test to Insert, Update and Delete record from a table using our custom implementation of the NgWrapper, executing the commands in multiple lines and in a single script
    /// </summary>
    [Fact]
    public void InsertUpdateDelete_JsonFormat_SingleScript_CustomXShell()
    {
      OpenConnection();

      try
      {
        var xshell = new NgShellWrapper(_ngConnString, true);
        xshell.ExecuteScript(_setMysqlxVar, ScriptType.JavaScript);
        xshell.ExecuteScript(_setSessionVar, ScriptType.JavaScript);
        var script = new StringBuilder();
        script.AppendLine(_dropTestDatabase);
        script.AppendLine(_createTestDatabase);
        script.AppendLine(_useTestDatabase);
        script.AppendLine(_createTestTable);

        script.AppendLine(_setSchemaVar);
        script.AppendLine(_setTableVar);
        script.AppendLine(_insertRecordJson1);
        script.AppendLine(_insertRecordJson2);
        script.AppendLine(_updateRecordCmd1);
        script.AppendLine(_updateRecordCmd2);
        script.AppendLine(_updateRecordCmd3);
        script.AppendLine(_deleteRecordCmd1);
        script.AppendLine(_deleteRecordCmd2);
        script.AppendLine(_deleteRecordCmd3);

        var tokenizer = new MyJsTokenizer(script.ToString());
        xshell.ExecuteScript(tokenizer.BreakIntoStatements().ToArray(), ScriptType.JavaScript);

        _command = new MySqlCommand(string.Format(_searchTable, _testTableName, _testDatabaseName), _connection);

        var result = _command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(_tableNotFound, _testTableName));

        var selectResult = xshell.ExecuteScript(_selectTestTable, ScriptType.JavaScript);
        Assert.True(selectResult != null, string.Format(_nullObject, "selectResult"));
        Assert.True(selectResult.Count == 1, _dataNotMatch);
      }
      catch (Exception ex)
      {
        throw ex;
      }
      finally
      {
        _setUp.ExecuteSQLAsRoot(_dropTestDbSqlSyntax);
        CloseConnection();
      }
    }

    /// <summary>
    /// Parse a RowResult to a RowResult data returned from the server when a statment that returns a result set is executed
    /// </summary>
    [Fact]
    public void ParseTableResultToDocumentResult_CustomXShell()
    {
      OpenConnection();

      try
      {
        var xshell = new NgShellWrapper(_ngConnString, true);
        xshell.ExecuteScript(_setMysqlxVar, ScriptType.JavaScript);
        xshell.ExecuteScript(_setSessionVar, ScriptType.JavaScript);
        xshell.ExecuteScript(_dropTestDatabase, ScriptType.JavaScript);
        xshell.ExecuteScript(_createTestDatabase, ScriptType.JavaScript);
        xshell.ExecuteScript(_useTestDatabase, ScriptType.JavaScript);
        xshell.ExecuteScript(_createTestTable, ScriptType.JavaScript);
        _command = new MySqlCommand(string.Format(_searchTable, _testTableName, _testDatabaseName), _connection);

        var result = _command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(_tableNotFound, _testTableName));

        xshell.ExecuteScript(_setSchemaVar, ScriptType.JavaScript);
        xshell.ExecuteScript(_setTableVar, ScriptType.JavaScript);
        xshell.ExecuteScript(_insertTwoRecords, ScriptType.JavaScript);
        var selectResult = xshell.ExecuteScript(_selectForTableResult, ScriptType.JavaScript);

        Assert.True(selectResult != null, string.Format(_nullObject, "selectResult"));
        Assert.True(selectResult.Count == 2, _dataNotMatch);
      }
      catch (Exception ex)
      {
        throw ex;
      }
      finally
      {
        _setUp.ExecuteSQLAsRoot(_dropTestDbSqlSyntax);
        CloseConnection();
      }
    }

    /// <summary>
    /// Open a MySqlConnection when it is not opened
    /// </summary>
    private void OpenConnection()
    {
      if (_connection.State != System.Data.ConnectionState.Open)
      {
        _connection.Open();
      }
    }

    /// <summary>
    /// Close a MySqlConnection when it is not closed
    /// </summary>
    private void CloseConnection()
    {
      if (_connection.State != System.Data.ConnectionState.Closed)
      {
        _connection.Close();
      }
    }

    /// <summary>
    /// Initializes the NgShell instance with common statements
    /// </summary>
    private void InitNgShell()
    {
      if (_ngShell != null)
        return;

      _ngShell = new MySimpleClientShell();
      _ngShell.MakeConnection(_ngConnString);
      _ngShell.SwitchMode(Mode.JScript);
    }

    /// <summary>
    /// Dispose implementation of the current class
    /// </summary>
    public void Dispose()
    {
      _setUp.Dispose();
    }
  }
}
