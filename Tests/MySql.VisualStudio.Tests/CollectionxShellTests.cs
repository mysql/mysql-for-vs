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
using System.Data;
using MySql.Data.MySqlClient;
using MySql.Data.VisualStudio.Editors;
using MySqlX.Shell;
using Xunit;

namespace MySql.VisualStudio.Tests
{
  class CollectionxShellTests : IUseFixture<SetUpXShell>, IDisposable
  {
    #region Fields
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
    #endregion

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
    private const string _testSchemaName = "js_shell_test";

    //TODO: Adjust this test for when this method is implemented in x-Shell for the JS sintaxis. It should look like:
    //private const string _dropTestDatabase = "session.dropSchema('js_shell_test;').execute();";
    /// <summary>
    /// Statement to drop the test database
    /// </summary>
    private const string _dropSchemaTest = "session.sql('drop schema if exists js_shell_test;').execute();";

    //TODO: Adjust this test for when this method is implemented in x-Shell for the JS sintaxis. It should look like:
    //private const string _createTestDatabase = "session.createSchema('js_shell_test;').execute();";
    /// <summary>
    /// Statement to create the test database
    /// </summary>
    private const string _createSchemaTest = "session.sql('create schema js_shell_test;').execute();";

    /// <summary>
    /// Statement to use the test database
    /// </summary>
    private const string _useSchemaTest = "session.sql('use js_shell_test;').execute();";
    /// <summary>
    /// Table test name
    /// </summary>
    private const string _testCollectionName = "collection1";
    /// <summary>
    /// Statement to create the test table
    /// </summary>
    private const string _createCollectionTest = "session.js_shell_test.createCollection('" + _testCollectionName + "');";

    //TODO: Adjust this test for when this method is fully implemented in x-Shell for the JS sintaxis. It should look like:
    //private const string _deleteCollectionTest = "session." + _testCollectionName + ".drop();";
    /// <summary>
    /// Statement to delete the test table
    /// </summary>
    private const string _deleteCollectionTest = "session.sql('drop table " + _testCollectionName + ";').execute();";

    /// <summary>
    /// Get and set the test database
    /// </summary>
    private const string _setSchemaVar = "var schema = session.getSchema('js_shell_test');";
    /// <summary>
    /// Get and set the test table
    /// </summary>
    private const string _setCollectionVar = "var coll = session.js_shell_test.getCollection('collection1');";
    /// <summary>
    /// Statement to insert multiple records at the same time on a single statement to the test collection
    /// </summary>
    private const string _addMultipleDocumentsSingleAddStatement = "var result = coll.add([{name: 'my third', passed: 'once again', count: 3},{name: 'my fourth', passed: 'and again', count: 4}, {name: 'my fifth', passed: 'and finally', count: 5}]).execute();";
    /// <summary>
    /// Statement to insert multiple records at the same time on a single statement to the test collection
    /// </summary>
    private const string _addMultipleDocumentsMultipleAddStatements = "var result = coll.add({name: 'my sixth', passed: 'aaaand again', count: 6}).add({name: 'my seventh', passed: 'aaaand once again', count: 7}).execute();";
    /// <summary>
    /// Statement to insert a record to the test table
    /// </summary>
    private const string _addJsonDocument1 = "var result = coll.add({ name: 'my first', passed: 'document', count: 1}).execute();";
    /// <summary>
    /// Statement to insert a record to the test table
    /// </summary>
    private const string _addJsonDocument2 = "var result = coll.add({ name: 'my second', passed: 'again', count: 2}).execute();";
    /// <summary>
    /// Statement to get all the records from the test table as TableResultSet
    /// </summary>
    private const string _findAllDocumentsInCollection = "var docs = coll.find(); docs.execute();";
    /// <summary>
    /// Statement to update a record in the test table in a single command
    /// </summary>
    private const string _modifyDocument = "var modr = coll.modify(\"name like 'my fourth'\"); var setr = modr.set('name','dummy'); var sortr = setr.sort(['name']); sortr.execute();";
    /// <summary>
    /// Statement to select the update record from the test table
    /// </summary>
    private const string _selectUpdatedRecord = "var urecord = coll.find('name like \"dummy\"'); urecord.execute();";
    /// <summary>
    /// Statement to select the update record from the test table
    /// </summary>
    private const string _findSpecificDocumentTest = "var records = coll.find('name like \"my second\"'); records.execute();";
    /// <summary>
    /// Statement to delete a record in the test table in a single command
    /// </summary>
    private const string _removeDocument = "var crud = coll.remove(\"name like 'my fifth'\"); crud.execute();";
    #endregion

    #region CommonAssertQueries
    /// <summary>
    /// Sql statement to select the current databases in the server
    /// </summary>
    private const string _showDbs = "show databases;";
    /// <summary>
    /// Search for a table in the schema.tables information. Use: string.format(_searchTable, "myTable")
    /// </summary>
    private const string _searchTable = "select count(*) from information_schema.TABLES where table_name='{0}';";
    /// <summary>
    /// Sql statement to drop the test database
    /// </summary>
    private const string _dropTestDbSqlSyntax = "drop schema if exists js_shell_test;";
    #endregion

    #region AssertFailMessages
    /// <summary>
    /// Message to display when a schema is not found. Usage: string.format(_dbNotFound, "myDatabase")
    /// </summary>
    private const string _schemaNotFound = "Schema {0} not found";
    /// <summary>
    /// Message to display when a collection is not found. Usage: string.format(_tableNotFound, "myTable")
    /// </summary>
    private const string _collectionNotFound = "Collection {0} not found";
    /// <summary>
    /// Message to display when a collection is not deleted. Usage: string.format(_tableNotDeleted, "myTable")
    /// </summary>
    private const string _collectionNotDeleted = "Collection {0} was not deleted";
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
    /// Setup information about the current schema test
    /// </summary>
    /// <param name="data">Current test instance configuration</param>
    public void SetFixture(SetUpXShell data)
    {
      _setUp = data;
      _connection = new MySqlConnection(_setUp.GetConnectionString(_setUp.rootUser, _setUp.rootPassword, false, false));
      _ngConnString = string.Format("{0}:{1}@{2}:{3}", _setUp.rootUser, _setUp.rootPassword, _setUp.host, 33060);
    }

    /// <summary>
    /// Test to create a Schema using the NgWrapper
    /// </summary>
    [Fact]
    public void CreateSchema_XShellDirectly()
    {
      OpenConnection();

      try
      {
        InitNgShell();
        _ngShell.Execute(_dropSchemaTest);
        _ngShell.Execute(_createSchemaTest);
        _command = new MySqlCommand(_showDbs, _connection);
        var reader = _command.ExecuteReader();
        bool success = false;

        while (reader.Read())
        {
          if (reader.GetString(0) == _testSchemaName)
          {
            success = true;
            reader.Close();
            break;
          }
        }

        Assert.True(success, string.Format(_schemaNotFound, _testSchemaName));
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
    /// Test to create a Collection using the NgWrapper
    /// </summary>
    [Fact]
    public void CreateCollection_XShellDirectly()
    {
      OpenConnection();

      try
      {
        InitNgShell();
        _ngShell.Execute(_dropSchemaTest);
        _ngShell.Execute(_createSchemaTest);
        _ngShell.Execute(_useSchemaTest);
        _ngShell.Execute(_createCollectionTest);
        _command = new MySqlCommand(string.Format(_searchTable, _testCollectionName), _connection);
        var result = _command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(_collectionNotFound, _testCollectionName));

        _ngShell.Execute(_deleteCollectionTest);
        result = _command.ExecuteScalar();
        int.TryParse(result.ToString(), out count);
        Assert.True(count == 0, string.Format(_collectionNotDeleted, _testCollectionName));
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
    /// Test to Add and Find data from a collection using the NgWrapper.
    /// </summary>
    [Fact]
    public void AddFind_XShellDirectly()
    {
      OpenConnection();

      try
      {
        InitNgShell();
        _ngShell.Execute(_dropSchemaTest);
        _ngShell.Execute(_createSchemaTest);
        _ngShell.Execute(_useSchemaTest);
        _ngShell.Execute(_createCollectionTest);
        _command = new MySqlCommand(string.Format(_searchTable, _testCollectionName), _connection);

        var result = _command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(_schemaNotFound, _testSchemaName));

        _ngShell.Execute(_setSchemaVar);
        _ngShell.Execute(_setCollectionVar);

        //Test single add
        _ngShell.Execute(_addJsonDocument1);
        var selectResult = _ngShell.Execute(_findAllDocumentsInCollection) as DocumentResultSet;
        Assert.True(selectResult != null, string.Format(_nullObject, "selectResult"));
        Assert.True(selectResult.GetData().Count == 1, _dataNotMatch);

        //Test single add again
        _ngShell.Execute(_addJsonDocument2);
        selectResult = _ngShell.Execute(_findAllDocumentsInCollection) as DocumentResultSet;
        Assert.True(selectResult != null, string.Format(_nullObject, "selectResult"));
        Assert.True(selectResult.GetData().Count == 2, _dataNotMatch);

        //Test multiple documents add statement
        _ngShell.Execute(_addMultipleDocumentsSingleAddStatement);
        selectResult = _ngShell.Execute(_findAllDocumentsInCollection) as DocumentResultSet;
        Assert.True(selectResult != null, string.Format(_nullObject, "selectResult"));
        Assert.True(selectResult.GetData().Count == 5, _dataNotMatch);

        //Test multiple add statements with single documents
        _ngShell.Execute(_addMultipleDocumentsMultipleAddStatements);
        selectResult = _ngShell.Execute(_findAllDocumentsInCollection) as DocumentResultSet;
        Assert.True(selectResult != null, string.Format(_nullObject, "selectResult"));
        Assert.True(selectResult.GetData().Count == 7, _dataNotMatch);

        selectResult = _ngShell.Execute(_findSpecificDocumentTest) as DocumentResultSet;
        Assert.True(selectResult != null, string.Format(_nullObject, "selectResult"));
        Assert.True(selectResult.GetData().Count == 1, _dataNotMatch);
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
    /// Test to Add and Find data from a collection using the NgWrapper.
    /// </summary>
    [Fact]
    public void Remove_XShellDirectly()
    {
      OpenConnection();

      try
      {
        InitNgShell();
        _ngShell.Execute(_dropSchemaTest);
        _ngShell.Execute(_createSchemaTest);
        _ngShell.Execute(_useSchemaTest);
        _ngShell.Execute(_createCollectionTest);
        _command = new MySqlCommand(string.Format(_searchTable, _testCollectionName), _connection);

        var result = _command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(_schemaNotFound, _testSchemaName));

        _ngShell.Execute(_setSchemaVar);
        _ngShell.Execute(_setCollectionVar);
        _ngShell.Execute(_addMultipleDocumentsSingleAddStatement);
        _ngShell.Execute(_removeDocument);
        var selectResult = _ngShell.Execute(_findAllDocumentsInCollection) as DocumentResultSet;
        Assert.True(selectResult != null, string.Format(_nullObject, "selectResult"));
        Assert.True(selectResult.GetData().Count == 2, _dataNotMatch);
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
    /// Test to Modify data from a collection using the NgWrapper.
    /// </summary>
    [Fact]
    public void Modify_XShellDirectly()
    {
      OpenConnection();

      try
      {
        InitNgShell();
        _ngShell.Execute(_dropSchemaTest);
        _ngShell.Execute(_createSchemaTest);
        _ngShell.Execute(_useSchemaTest);
        _ngShell.Execute(_createCollectionTest);
        _command = new MySqlCommand(string.Format(_searchTable, _testCollectionName), _connection);

        var result = _command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(_schemaNotFound, _testSchemaName));

        _ngShell.Execute(_setSchemaVar);
        _ngShell.Execute(_setCollectionVar);
        _ngShell.Execute(_addMultipleDocumentsSingleAddStatement);
        _ngShell.Execute(_modifyDocument);
        var selectResult = _ngShell.Execute(_selectUpdatedRecord) as DocumentResultSet;
        Assert.True(selectResult != null, string.Format(_nullObject, "selectResult"));
        Assert.True(selectResult.GetData().Count == 1, _dataNotMatch);
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
    public void CreateSchema_CustomXShell()
    {
      OpenConnection();

      try
      {
        var xshell = new NgShellWrapper(_ngConnString, true);
        xshell.ExecuteJavaScript(_setMysqlxVar);
        xshell.ExecuteJavaScript(_setSessionVar);
        xshell.ExecuteJavaScript(_dropSchemaTest);
        xshell.ExecuteJavaScript(_createSchemaTest);
        _command = new MySqlCommand(_showDbs, _connection);
        var reader = _command.ExecuteReader();
        bool success = false;
        while (reader.Read())
        {
          if (reader.GetString(0) == _testSchemaName)
          {
            success = true;
            reader.Close();
            break;
          }
        }
        Assert.True(success, string.Format(_schemaNotFound, _testSchemaName));
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
    public void CreateCollection_CustomXShell()
    {
      OpenConnection();

      try
      {
        var xshell = new NgShellWrapper(_ngConnString, true);
        xshell.ExecuteJavaScript(_setMysqlxVar);
        xshell.ExecuteJavaScript(_setSessionVar);
        xshell.ExecuteJavaScript(_dropSchemaTest);
        xshell.ExecuteJavaScript(_createSchemaTest);
        xshell.ExecuteJavaScript(_useSchemaTest);
        xshell.ExecuteJavaScript(_createCollectionTest);
        _command = new MySqlCommand(string.Format(_searchTable, _testCollectionName), _connection);
        var result = _command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(_collectionNotFound, _testCollectionName));

        xshell.ExecuteJavaScript(_deleteCollectionTest);
        result = _command.ExecuteScalar();
        int.TryParse(result.ToString(), out count);
        Assert.True(count == 0, string.Format(_collectionNotDeleted, _testCollectionName));
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
    /// Test to Add, Modify, Delete and Find a record from a collection using our custom implementation of the NgWrapper, executing the commands in a single line
    /// </summary>
    [Fact]
    public void AddFind_CustomXShell()
    {
      OpenConnection();

      try
      {
        var xshell = new NgShellWrapper(_ngConnString, true);
        xshell.ExecuteJavaScript(_setMysqlxVar);
        xshell.ExecuteJavaScript(_setSessionVar);
        xshell.ExecuteJavaScript(_dropSchemaTest);
        xshell.ExecuteJavaScript(_createSchemaTest);
        xshell.ExecuteJavaScript(_useSchemaTest);
        xshell.ExecuteJavaScript(_createCollectionTest);
        _command = new MySqlCommand(string.Format(_searchTable, _testCollectionName), _connection);

        var result = _command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(_schemaNotFound, _testCollectionName));

        xshell.ExecuteJavaScript(_setSchemaVar);
        xshell.ExecuteJavaScript(_setCollectionVar);

        //Test single add
        xshell.ExecuteJavaScript(_addJsonDocument1);
        var selectResult = xshell.ExecuteJavaScript(_findAllDocumentsInCollection) as DocumentResultSet;
        Assert.True(selectResult != null, string.Format(_nullObject, "selectResult"));
        Assert.True(selectResult.GetData().Count == 1, _dataNotMatch);

        //Test single add again
        xshell.ExecuteJavaScript(_addJsonDocument2);
        selectResult = xshell.ExecuteJavaScript(_findAllDocumentsInCollection) as DocumentResultSet;
        Assert.True(selectResult != null, string.Format(_nullObject, "selectResult"));
        Assert.True(selectResult.GetData().Count == 2, _dataNotMatch);

        //Test multiple documents add statement
        xshell.ExecuteJavaScript(_addMultipleDocumentsSingleAddStatement);
        selectResult = xshell.ExecuteJavaScript(_findAllDocumentsInCollection) as DocumentResultSet;
        Assert.True(selectResult != null, string.Format(_nullObject, "selectResult"));
        Assert.True(selectResult.GetData().Count == 5, _dataNotMatch);

        //Test multiple add statements with single documents
        xshell.ExecuteJavaScript(_addMultipleDocumentsMultipleAddStatements);
        selectResult = xshell.ExecuteJavaScript(_findAllDocumentsInCollection) as DocumentResultSet;
        Assert.True(selectResult != null, string.Format(_nullObject, "selectResult"));
        Assert.True(selectResult.GetData().Count == 7, _dataNotMatch);

        selectResult = xshell.ExecuteJavaScript(_findSpecificDocumentTest) as DocumentResultSet;
        Assert.True(selectResult != null, string.Format(_nullObject, "selectResult"));
        Assert.True(selectResult.GetData().Count == 1, _dataNotMatch);
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
    /// Test to Add and Find data from a collection using the NgWrapper.
    /// </summary>
    [Fact]
    public void Remove_CustomXShell()
    {
      OpenConnection();

      try
      {
        var xshell = new NgShellWrapper(_ngConnString, true);
        xshell.ExecuteJavaScript(_setMysqlxVar);
        xshell.ExecuteJavaScript(_setSessionVar);
        xshell.ExecuteJavaScript(_dropSchemaTest);
        xshell.ExecuteJavaScript(_createSchemaTest);
        xshell.ExecuteJavaScript(_useSchemaTest);
        xshell.ExecuteJavaScript(_createCollectionTest);
        _command = new MySqlCommand(string.Format(_searchTable, _testCollectionName), _connection);

        var result = _command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(_schemaNotFound, _testSchemaName));

        xshell.ExecuteJavaScript(_setSchemaVar);
        xshell.ExecuteJavaScript(_setCollectionVar);
        xshell.ExecuteJavaScript(_addMultipleDocumentsSingleAddStatement);
        xshell.ExecuteJavaScript(_removeDocument);
        var selectResult = xshell.ExecuteJavaScript(_findAllDocumentsInCollection) as DocumentResultSet;
        Assert.True(selectResult != null, string.Format(_nullObject, "selectResult"));
        Assert.True(selectResult.GetData().Count == 2, _dataNotMatch);
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
    /// Test to Modify data from a collection using the NgWrapper.
    /// </summary>
    [Fact]
    public void Modify_CustomXShell()
    {
      OpenConnection();

      try
      {
        var xshell = new NgShellWrapper(_ngConnString, true);
        xshell.ExecuteJavaScript(_setMysqlxVar);
        xshell.ExecuteJavaScript(_setSessionVar);
        xshell.ExecuteJavaScript(_dropSchemaTest);
        xshell.ExecuteJavaScript(_createSchemaTest);
        xshell.ExecuteJavaScript(_useSchemaTest);
        xshell.ExecuteJavaScript(_createCollectionTest);
        _command = new MySqlCommand(string.Format(_searchTable, _testCollectionName), _connection);

        var result = _command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(_schemaNotFound, _testSchemaName));

        xshell.ExecuteJavaScript(_setSchemaVar);
        xshell.ExecuteJavaScript(_setCollectionVar);
        xshell.ExecuteJavaScript(_addMultipleDocumentsSingleAddStatement);
        xshell.ExecuteJavaScript(_modifyDocument);
        var selectResult = xshell.ExecuteJavaScript(_selectUpdatedRecord) as DocumentResultSet;
        Assert.True(selectResult != null, string.Format(_nullObject, "selectResult"));
        Assert.True(selectResult.GetData().Count == 1, _dataNotMatch);
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
      if (_connection.State != ConnectionState.Open)
      {
        _connection.Open();
      }
    }

    /// <summary>
    /// Close a MySqlConnection when it is not closed
    /// </summary>
    private void CloseConnection()
    {
      if (_connection.State != ConnectionState.Closed)
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
