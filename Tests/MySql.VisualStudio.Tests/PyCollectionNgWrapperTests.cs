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
using System.Text;
using MySQL.Utility.Classes;

namespace MySql.VisualStudio.Tests
{
  class PyCollectionNgWrapperTests : IUseFixture<SetUpXShell>, IDisposable
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
    private NgShellWrapper _ngShell;
    /// <summary>
    /// Stores the connection string format used by the mysqlx protocol
    /// </summary>
    private string _ngConnString;
    #endregion

    #region CommonShellQueries
    /// <summary>
    /// Get and set the mysqlx protocol instance
    /// </summary>
    private const string _setMysqlxVar = "import mysqlx";

    /// <summary>
    /// Get and set the node session from the mysqlx protocol instance
    /// </summary>
    private const string _setSessionVar = "session = mysqlx.getNodeSession('root:@localhost:33060')";

    /// <summary>
    /// Database test name
    /// </summary>
    private const string _testSchemaName = "py_schema_test";

    //TODO: [MYSQLFORVS-413] Adjust this test for when this method is implemented in x-Shell for the JS sintaxis. It should look like:
    //private const string _dropTestDatabase = "session.dropSchema('" + _testSchemaName + "')";
    /// <summary>
    /// Statement to drop the test database
    /// </summary>
    private const string _dropSchemaTest = "session.sql('drop schema if exists " + _testSchemaName + ";').execute()";

    /// <summary>
    /// Statement to create the test database
    /// </summary>
    private const string _createSchemaTest = "session.createSchema('" + _testSchemaName + "')";

    /// <summary>
    /// Statement to use the test database
    /// </summary>
    private const string _useSchemaTest = "session.sql('use " + _testSchemaName + ";').execute()";

    /// <summary>
    /// Table test name
    /// </summary>
    private const string _testCollectionName = "collection1py";

    /// <summary>
    /// Statement to create the test table
    /// </summary>
    private const string _createCollectionTest = "session." + _testSchemaName + ".createCollection('" + _testCollectionName + "')";

    //TODO: [MYSQLFORVS-414] Adjust this test for when this method is fully implemented in x-Shell for the JS sintaxis. It should look like:
    //private const string _deleteCollectionTest = "session." + _testSchemaName + "." + _testCollectionName + ".drop()";
    /// <summary>
    /// Statement to delete the test table
    /// </summary>
    private const string _deleteCollectionTest = "session.sql('drop table " + _testSchemaName + "." + _testCollectionName + "').execute()";

    /// <summary>
    /// Get and set the test database
    /// </summary>
    private const string _setSchemaVar = "schema = session.getSchema('" + _testSchemaName + "')";

    /// <summary>
    /// Get and set the test table
    /// </summary>
    private const string _setCollectionVar = "coll = session." + _testSchemaName + ".getCollection('" + _testCollectionName + "')";

    /// <summary>
    /// Statement to insert multiple records at the same time on a single statement to the test collection
    /// </summary>
    private const string _addMultipleDocumentsSingleAddStatement = "result = coll.add([{'name' : 'my third', 'passed' : 'once again', 'count' : 3},{'name': 'my fourth', 'passed' : 'and again', 'count' : 4}, {'name' : 'my fifth', 'passed' : 'and finally', 'count' : 5}]).execute()";

    /// <summary>
    /// Statement to insert multiple records at the same time on a single statement to the test collection
    /// </summary>
    private const string _addMultipleDocumentsMultipleAddStatements = "result = coll.add({'name' : 'my sixth', 'passed' : 'aaaand again', 'count' : 6}).add({'name' : 'my seventh', 'passed' : 'aaaand once again', 'count' : 7}).execute()";

    /// <summary>
    /// Statement to insert a record to the test table
    /// </summary>
    private const string _addJsonDocument1 = "result = coll.add({'name' : 'my first', 'passed' : 'document', 'count' : 1}).execute()";

    /// <summary>
    /// Statement to insert a record to the test table
    /// </summary>
    private const string _addJsonDocument2 = "result = coll.add({'name' : 'my second', 'passed' : 'again', 'count' : 2}).execute()";

    /// <summary>
    /// Statement to get all the records from the test table as TableResultSet
    /// </summary>
    private const string _findAllDocumentsInCollection = "coll.find().execute()";

    /// <summary>
    /// Statements to update a record in the test table.
    /// </summary>
    private const string _modifyDocument1 = "modr = coll.modify(\"name like 'my fourth'\")";
    private const string _modifyDocument2 = "setr = modr.set('name','dummy')";
    private const string _modifyDocument3 = "sortr = setr.sort(['name']).execute()";

    /// <summary>
    /// Statement to select the update record from the test table
    /// </summary>
    private const string _selectUpdatedRecord = "coll.find('name like \"dummy\"').execute()";

    /// <summary>
    /// Statement to select the update record from the test table
    /// </summary>
    private const string _findSpecificDocumentTest = "coll.find(\"name like 'my second'\").execute()";

    /// <summary>
    /// Statement to delete a record in the test table in a single command
    /// </summary>
    private const string _removeDocument = "coll.remove(\"name like 'my fifth'\").execute()";

    #endregion

    #region CommonAssertQueries
    /// <summary>
    /// Sql statement to select the current databases in the server
    /// </summary>
    private const string _showDbs = "show databases;";

    /// <summary>
    /// Search for a table in the schema.tables information. Use: string.format(_searchTable, "myTable")
    /// </summary>
    private const string _searchTable = "select count(*) from information_schema.TABLES where table_name='{0}'";

    /// <summary>
    /// Sql statement to drop the test database
    /// </summary>
    private const string _dropTestDbSqlSyntax = "drop schema if exists " + _testSchemaName + ";";

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
      _ngShell = new NgShellWrapper(_ngConnString, true);

    }

    /// <summary>
    /// Test to Insert, Update and Delete record from a table using our custom implementation of the NgWrapper, executing the commands in multiple lines and in a single script
    /// </summary>
    [Fact]
    public void Insert_JsonFormat_AllTests_CustomXShell()
    {
      OpenConnection();

      try
      {
        InitNgShell();

        //Create Schema test
        _ngShell.ExecuteScript(_setMysqlxVar, ScriptType.Python);
        _ngShell.ExecuteScript(_setSessionVar, ScriptType.Python);
        _ngShell.ExecuteScript(_dropSchemaTest, ScriptType.Python);
        _ngShell.ExecuteScript(_createSchemaTest, ScriptType.Python);
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

        //Create Collection test
        var script = new StringBuilder();
        script.AppendLine(_useSchemaTest);
        _ngShell.ExecuteScript(_createCollectionTest, ScriptType.Python);
        _command = new MySqlCommand(string.Format(_searchTable, _testCollectionName), _connection);
        var result = _command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(_collectionNotFound, _testCollectionName));

        //Batch Script Test & Single Insert Test
        script.AppendLine(_setSchemaVar);
        script.AppendLine(_setCollectionVar);
        script.AppendLine(_addJsonDocument1);
        script.AppendLine(_addJsonDocument2);
        var tokenizer = new MyPythonTokenizer(script.ToString());
        _ngShell.ExecuteScript(tokenizer.BreakIntoStatements().ToArray(), ScriptType.Python);

        var selectResult = _ngShell.ExecuteScript(_findAllDocumentsInCollection, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(_nullObject, "selectResult"));
        Assert.True(selectResult.GetData().Count == 2, _dataNotMatch);

        //Test multiple documents add statement
        _ngShell.ExecuteScript(_addMultipleDocumentsSingleAddStatement, ScriptType.Python);
        selectResult = _ngShell.ExecuteScript(_findAllDocumentsInCollection, ScriptType.Python) as DocumentResultSet;
        Assert.True(selectResult != null, string.Format(_nullObject, "selectResult"));
        Assert.True(selectResult.GetData().Count == 5, _dataNotMatch);

        //Test multiple add statements with single documents
        _ngShell.ExecuteScript(_addMultipleDocumentsMultipleAddStatements, ScriptType.Python);
        selectResult = _ngShell.ExecuteScript(_findAllDocumentsInCollection, ScriptType.Python) as DocumentResultSet;
        Assert.True(selectResult != null, string.Format(_nullObject, "selectResult"));
        Assert.True(selectResult.GetData().Count == 7, _dataNotMatch);

        //Find Test
        selectResult = _ngShell.ExecuteScript(_findSpecificDocumentTest, ScriptType.Python) as DocumentResultSet;
        Assert.True(selectResult != null, string.Format(_nullObject, "selectResult"));
        Assert.True(selectResult.GetData().Count == 1, _dataNotMatch);

        //Update record test
        _ngShell.ExecuteScript(_modifyDocument1, ScriptType.Python);
        _ngShell.ExecuteScript(_modifyDocument2, ScriptType.Python);
        _ngShell.ExecuteScript(_modifyDocument3, ScriptType.Python);
        selectResult = _ngShell.ExecuteScript(_selectUpdatedRecord, ScriptType.Python) as DocumentResultSet;
        Assert.True(selectResult != null, string.Format(_nullObject, "selectResult"));
        Assert.True(selectResult.GetData().Count == 1, _dataNotMatch);

        //Remove Documents test
        _ngShell.ExecuteScript(_removeDocument, ScriptType.Python);
        selectResult = _ngShell.ExecuteScript(_findAllDocumentsInCollection, ScriptType.Python) as DocumentResultSet;
        Assert.True(selectResult != null, string.Format(_nullObject, "selectResult"));
        Assert.True(selectResult.GetData().Count == 6, _dataNotMatch);

        //Drop Collection test
        _ngShell.ExecuteScript(_deleteCollectionTest, ScriptType.Python);
        _command = new MySqlCommand(string.Format(_searchTable, _testCollectionName), _connection);
        result = _command.ExecuteScalar();
        int.TryParse(result.ToString(), out count);
        Assert.True(count == 0, string.Format(_collectionNotDeleted, _testCollectionName));

        //Drop Schema test
        _ngShell.ExecuteScript(_dropSchemaTest, ScriptType.Python);
        _command = new MySqlCommand(_showDbs, _connection);
        reader = _command.ExecuteReader();
        success = true;
        while (reader.Read())
        {
          if (reader.GetString(0) == _testSchemaName)
          {
            success = false;
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
    /// Test to Insert, Update and Delete record from a table using our custom implementation of the NgWrapper, executing the commands in multiple lines and in a single script
    /// </summary>
    //[Fact]
    public void Insert_JsonFormat_SingleScript_CustomXShell()
    {
      OpenConnection();

      try
      {
        InitNgShell();
        _ngShell.ExecuteScript(_setMysqlxVar, ScriptType.Python);
        _ngShell.ExecuteScript(_setSessionVar, ScriptType.Python);
        var script = new StringBuilder();
        script.AppendLine(_dropSchemaTest);
        script.AppendLine(_createSchemaTest);
        script.AppendLine(_useSchemaTest);
        script.AppendLine(_createCollectionTest);

        script.AppendLine(_setSchemaVar);
        script.AppendLine(_setCollectionVar);
        script.AppendLine(_addJsonDocument1);
        script.AppendLine(_addJsonDocument2);

        var tokenizer = new MyPythonTokenizer(script.ToString());
        _ngShell.ExecuteScript(tokenizer.BreakIntoStatements().ToArray(), ScriptType.Python);

        _command = new MySqlCommand(string.Format(_searchTable, _testCollectionName), _connection);

        var result = _command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(_collectionNotFound, _testCollectionName));

        var selectResult = _ngShell.ExecuteScript(_findAllDocumentsInCollection, ScriptType.Python);
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
    /// Test to create a Database using our custom implementation of the NgWrapper
    /// </summary>
    //[Fact]
    public void CreateSchema_CustomXShell()
    {
      OpenConnection();

      try
      {
        InitNgShell();
        _ngShell.ExecuteScript(_setMysqlxVar, ScriptType.Python);
        _ngShell.ExecuteScript(_setSessionVar, ScriptType.Python);
        _ngShell.ExecuteScript(_dropSchemaTest, ScriptType.Python);
        _ngShell.ExecuteScript(_createSchemaTest, ScriptType.Python);
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
    //[Fact]
    public void CreateCollection_CustomXShell()
    {
      OpenConnection();

      try
      {
        InitNgShell();
        _ngShell.ExecuteScript(_setMysqlxVar, ScriptType.Python);
        _ngShell.ExecuteScript(_setSessionVar, ScriptType.Python);
        _ngShell.ExecuteScript(_dropSchemaTest, ScriptType.Python);
        _ngShell.ExecuteScript(_createSchemaTest, ScriptType.Python);
        _ngShell.ExecuteScript(_useSchemaTest, ScriptType.Python);
        _ngShell.ExecuteScript(_createCollectionTest, ScriptType.Python);
        _command = new MySqlCommand(string.Format(_searchTable, _testCollectionName), _connection);
        var result = _command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(_collectionNotFound, _testCollectionName));

        _ngShell.ExecuteScript(_deleteCollectionTest, ScriptType.Python);
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
    //[Fact]
    public void AddFind_CustomXShell()
    {
      OpenConnection();

      try
      {
        InitNgShell();
        _ngShell.ExecuteScript(_setMysqlxVar, ScriptType.Python);
        _ngShell.ExecuteScript(_setSessionVar, ScriptType.Python);
        _ngShell.ExecuteScript(_dropSchemaTest, ScriptType.Python);
        _ngShell.ExecuteScript(_createSchemaTest, ScriptType.Python);
        _ngShell.ExecuteScript(_useSchemaTest, ScriptType.Python);
        _ngShell.ExecuteScript(_createCollectionTest, ScriptType.Python);
        _command = new MySqlCommand(string.Format(_searchTable, _testCollectionName), _connection);

        var result = _command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(_schemaNotFound, _testCollectionName));

        _ngShell.ExecuteScript(_setCollectionVar, ScriptType.Python);

        //Test single add
        _ngShell.ExecuteScript(_addJsonDocument1, ScriptType.Python);
        var selectResult = _ngShell.ExecuteScript(_findAllDocumentsInCollection, ScriptType.Python) as DocumentResultSet;
        Assert.True(selectResult != null, string.Format(_nullObject, "selectResult"));
        Assert.True(selectResult.GetData().Count == 1, _dataNotMatch);

        //Test single add again
        _ngShell.ExecuteScript(_addJsonDocument2, ScriptType.Python);
        selectResult = _ngShell.ExecuteScript(_findAllDocumentsInCollection, ScriptType.Python) as DocumentResultSet;
        Assert.True(selectResult != null, string.Format(_nullObject, "selectResult"));
        Assert.True(selectResult.GetData().Count == 2, _dataNotMatch);

        //Test multiple documents add statement
        _ngShell.ExecuteScript(_addMultipleDocumentsSingleAddStatement, ScriptType.Python);
        selectResult = _ngShell.ExecuteScript(_findAllDocumentsInCollection, ScriptType.Python) as DocumentResultSet;
        Assert.True(selectResult != null, string.Format(_nullObject, "selectResult"));
        Assert.True(selectResult.GetData().Count == 5, _dataNotMatch);

        //Test multiple add statements with single documents
        _ngShell.ExecuteScript(_addMultipleDocumentsMultipleAddStatements, ScriptType.Python);
        selectResult = _ngShell.ExecuteScript(_findAllDocumentsInCollection, ScriptType.Python) as DocumentResultSet;
        Assert.True(selectResult != null, string.Format(_nullObject, "selectResult"));
        Assert.True(selectResult.GetData().Count == 7, _dataNotMatch);

        selectResult = _ngShell.ExecuteScript(_findSpecificDocumentTest, ScriptType.Python) as DocumentResultSet;
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
    //[Fact]
    public void Remove_CustomXShell()
    {
      OpenConnection();

      try
      {
        InitNgShell();
        _ngShell.ExecuteScript(_setMysqlxVar, ScriptType.Python);
        _ngShell.ExecuteScript(_setSessionVar, ScriptType.Python);
        _ngShell.ExecuteScript(_dropSchemaTest, ScriptType.Python);
        _ngShell.ExecuteScript(_createSchemaTest, ScriptType.Python);
        _ngShell.ExecuteScript(_useSchemaTest, ScriptType.Python);
        _ngShell.ExecuteScript(_createCollectionTest, ScriptType.Python);
        _command = new MySqlCommand(string.Format(_searchTable, _testCollectionName), _connection);

        var result = _command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(_schemaNotFound, _testSchemaName));

        _ngShell.ExecuteScript(_setCollectionVar, ScriptType.Python);
        _ngShell.ExecuteScript(_addMultipleDocumentsSingleAddStatement, ScriptType.Python);
        _ngShell.ExecuteScript(_removeDocument, ScriptType.Python);
        var selectResult = _ngShell.ExecuteScript(_findAllDocumentsInCollection, ScriptType.Python) as DocumentResultSet;
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
    //[Fact]
    public void Modify_CustomXShell()
    {
      OpenConnection();

      try
      {
        InitNgShell();
        _ngShell.ExecuteScript(_setMysqlxVar, ScriptType.Python);
        _ngShell.ExecuteScript(_setSessionVar, ScriptType.Python);
        _ngShell.ExecuteScript(_dropSchemaTest, ScriptType.Python);
        _ngShell.ExecuteScript(_createSchemaTest, ScriptType.Python);
        _ngShell.ExecuteScript(_useSchemaTest, ScriptType.Python);
        _ngShell.ExecuteScript(_createCollectionTest, ScriptType.Python);
        _command = new MySqlCommand(string.Format(_searchTable, _testCollectionName), _connection);

        var result = _command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(_schemaNotFound, _testSchemaName));

        _ngShell.ExecuteScript(_setCollectionVar, ScriptType.Python);
        _ngShell.ExecuteScript(_addMultipleDocumentsSingleAddStatement, ScriptType.Python);
        _ngShell.ExecuteScript(_modifyDocument1, ScriptType.Python);
        _ngShell.ExecuteScript(_modifyDocument2, ScriptType.Python);
        _ngShell.ExecuteScript(_modifyDocument3, ScriptType.Python);
        var selectResult = _ngShell.ExecuteScript(_selectUpdatedRecord, ScriptType.Python) as DocumentResultSet;
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
      {
        return;
      }

      _ngShell = new NgShellWrapper(_ngConnString, true);
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
