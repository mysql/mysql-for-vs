// Copyright © 2015, 2016, Oracle and/or its affiliates. All rights reserved.
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

using MySql.Data.MySqlClient;
using MySql.Data.VisualStudio.Editors;
using MySql.Data.VisualStudio.MySqlX;
using MySql.VisualStudio.Tests.MySqlX.Base;
using Xunit;

namespace MySql.VisualStudio.Tests.MySqlX
{
  public class JsCollectionXProxyTests : JsCollectionTests, IUseFixture<SetUpXShell>
  {
    #region Fields

    /// <summary>
    /// Object to access and execute commands to the current database connection through the mysqlx protocol
    /// </summary>
    private MySqlXProxy _xProxy;

    #endregion Fields

    /// <summary>
    /// Test to Add, Modify, Delete and Find a record from a collection using the <see cref="MySqlXProxy"/>, executing the commands in a single line
    /// </summary>
    [Fact]
    public void AddFind()
    {
      OpenConnection();

      try
      {
        InitXProxy();
        _xProxy.ExecuteScript(DROP_SCHEMA_IF_EXISTS, ScriptType.JavaScript);
        _xProxy.ExecuteScript(CREATE_SCHEMA_TEST, ScriptType.JavaScript);
        _xProxy.ExecuteScript(USE_SCHEMA_TEST, ScriptType.JavaScript);
        _xProxy.ExecuteScript(CREATE_COLLECTION_TEST, ScriptType.JavaScript);
        Command = new MySqlCommand(string.Format(SEARCH_TABLE, TEST_COLLECTION_NAME, TEST_SCHEMA_NAME), Connection);

        var result = Command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(SCHEMA_NOT_FOUND, TEST_COLLECTION_NAME));

        _xProxy.ExecuteScript(SET_SCHEMA_VAR, ScriptType.JavaScript);
        _xProxy.ExecuteScript(SET_COLLECTION_VAR, ScriptType.JavaScript);

        //Test single add
        _xProxy.ExecuteScript(ADD_JSON_DOCUMENT1, ScriptType.JavaScript);
        var selectResult = _xProxy.ExecuteScript(FIND_ALL_DOCUMENTS_IN_COLLECTION, ScriptType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 1, DATA_NOT_MATCH);

        //Test single add again
        _xProxy.ExecuteScript(ADD_JSON_DOCUMENT2, ScriptType.JavaScript);
        selectResult = _xProxy.ExecuteScript(FIND_ALL_DOCUMENTS_IN_COLLECTION, ScriptType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 2, DATA_NOT_MATCH);

        //Test multiple documents add statement
        _xProxy.ExecuteScript(ADD_MULTIPLE_DOCUMENTS_SINGLE_ADD_STATEMENT, ScriptType.JavaScript);
        selectResult = _xProxy.ExecuteScript(FIND_ALL_DOCUMENTS_IN_COLLECTION, ScriptType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 5, DATA_NOT_MATCH);

        //Test multiple add statements with single documents
        _xProxy.ExecuteScript(ADD_MULTIPLE_DOCUMENTS_MULTIPLE_ADD_STATEMENTS, ScriptType.JavaScript);
        selectResult = _xProxy.ExecuteScript(FIND_ALL_DOCUMENTS_IN_COLLECTION, ScriptType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 7, DATA_NOT_MATCH);

        selectResult = _xProxy.ExecuteScript(FIND_SPECIFIC_DOCUMENT_TEST, ScriptType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 1, DATA_NOT_MATCH);
      }
      finally
      {
        if (Command != null)
        {
          Command.Dispose();
        }

        SetUp.ExecuteSql(DROP_TEST_DB_SQL_SYNTAX);
        CloseConnection();
      }
    }

    /// <summary>
    /// Test to create a Table using the <see cref="MySqlXProxy"/>.
    /// </summary>
    [Fact]
    public void CreateCollection()
    {
      OpenConnection();

      try
      {
        InitXProxy();
        _xProxy.ExecuteScript(DROP_SCHEMA_IF_EXISTS, ScriptType.JavaScript);
        _xProxy.ExecuteScript(CREATE_SCHEMA_TEST, ScriptType.JavaScript);
        _xProxy.ExecuteScript(USE_SCHEMA_TEST, ScriptType.JavaScript);
        _xProxy.ExecuteScript(CREATE_COLLECTION_TEST, ScriptType.JavaScript);
        Command = new MySqlCommand(string.Format(SEARCH_TABLE, TEST_COLLECTION_NAME, TEST_SCHEMA_NAME), Connection);
        var result = Command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(COLLECTION_NOT_FOUND, TEST_COLLECTION_NAME));

        _xProxy.ExecuteScript(DELETE_COLLECTION_TEST, ScriptType.JavaScript);
        result = Command.ExecuteScalar();
        int.TryParse(result.ToString(), out count);
        Assert.True(count == 0, string.Format(COLLECTION_NOT_DELETED, TEST_COLLECTION_NAME));
      }
      finally
      {
        if (Command != null)
        {
          Command.Dispose();
        }

        SetUp.ExecuteSql(DROP_TEST_DB_SQL_SYNTAX);
        CloseConnection();
      }
    }

    /// <summary>
    /// Test to create a Database using the <see cref="MySqlXProxy"/>.
    /// </summary>
    [Fact]
    public void CreateSchema()
    {
      OpenConnection();
      MySqlDataReader reader = null;

      try
      {
        InitXProxy();
        _xProxy.ExecuteScript(DROP_SCHEMA_IF_EXISTS, ScriptType.JavaScript);
        _xProxy.ExecuteScript(CREATE_SCHEMA_TEST, ScriptType.JavaScript);
        Command = new MySqlCommand(SHOW_DBS, Connection);
        reader = Command.ExecuteReader();
        bool success = false;
        while (reader.Read())
        {
          var retSchema = reader.GetString(0);
          if (retSchema == TEST_SCHEMA_NAME)
          {
            success = true;
            reader.Close();
            break;
          }
        }

        Assert.True(success, string.Format(SCHEMA_NOT_FOUND, TEST_SCHEMA_NAME));
        _xProxy.ExecuteScript(DROP_SCHEMA_TEST, ScriptType.JavaScript);
        Command = new MySqlCommand(SHOW_DBS, Connection);
        reader = Command.ExecuteReader();
        while (reader.Read())
        {
          var retSchema = reader.GetString(0);
          if (retSchema == TEST_SCHEMA_NAME)
          {
            success = false;
            reader.Close();
            break;
          }
        }

        Assert.True(success, string.Format(SCHEMA_NOT_FOUND, TEST_SCHEMA_NAME));
      }
      finally
      {
        if (reader != null)
        {
          if (!reader.IsClosed)
          {
            reader.Close();
          }

          reader.Dispose();
        }

        if (Command != null)
        {
          Command.Dispose();
        }

        SetUp.ExecuteSql(DROP_TEST_DB_SQL_SYNTAX);
        CloseConnection();
      }
    }

    /// <summary>
    /// Test to Modify data from a collection using the <see cref="MySqlXProxy"/>.
    /// </summary>
    [Fact]
    public void Modify()
    {
      OpenConnection();

      try
      {
        InitXProxy();
        _xProxy.ExecuteScript(DROP_SCHEMA_IF_EXISTS, ScriptType.JavaScript);
        _xProxy.ExecuteScript(CREATE_SCHEMA_TEST, ScriptType.JavaScript);
        _xProxy.ExecuteScript(USE_SCHEMA_TEST, ScriptType.JavaScript);
        _xProxy.ExecuteScript(CREATE_COLLECTION_TEST, ScriptType.JavaScript);
        Command = new MySqlCommand(string.Format(SEARCH_TABLE, TEST_COLLECTION_NAME, TEST_SCHEMA_NAME), Connection);

        var result = Command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(SCHEMA_NOT_FOUND, TEST_SCHEMA_NAME));

        _xProxy.ExecuteScript(SET_SCHEMA_VAR, ScriptType.JavaScript);
        _xProxy.ExecuteScript(SET_COLLECTION_VAR, ScriptType.JavaScript);
        _xProxy.ExecuteScript(ADD_MULTIPLE_DOCUMENTS_SINGLE_ADD_STATEMENT, ScriptType.JavaScript);
        _xProxy.ExecuteScript(MODIFY_DOCUMENT, ScriptType.JavaScript);
        var selectResult = _xProxy.ExecuteScript(SELECT_UPDATED_RECORD, ScriptType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 1, DATA_NOT_MATCH);
      }
      finally
      {
        if (Command != null)
        {
          Command.Dispose();
        }

        SetUp.ExecuteSql(DROP_TEST_DB_SQL_SYNTAX);
        CloseConnection();
      }
    }

    /// <summary>
    /// Test to Add and Find data from a collection using the <see cref="MySqlXProxy"/>.
    /// </summary>
    [Fact]
    public void Remove()
    {
      OpenConnection();

      try
      {
        InitXProxy();
        _xProxy.ExecuteScript(DROP_SCHEMA_IF_EXISTS, ScriptType.JavaScript);
        _xProxy.ExecuteScript(CREATE_SCHEMA_TEST, ScriptType.JavaScript);
        _xProxy.ExecuteScript(USE_SCHEMA_TEST, ScriptType.JavaScript);
        _xProxy.ExecuteScript(CREATE_COLLECTION_TEST, ScriptType.JavaScript);
        Command = new MySqlCommand(string.Format(SEARCH_TABLE, TEST_COLLECTION_NAME, TEST_SCHEMA_NAME), Connection);

        var result = Command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(SCHEMA_NOT_FOUND, TEST_SCHEMA_NAME));

        _xProxy.ExecuteScript(SET_SCHEMA_VAR, ScriptType.JavaScript);
        _xProxy.ExecuteScript(SET_COLLECTION_VAR, ScriptType.JavaScript);
        _xProxy.ExecuteScript(ADD_MULTIPLE_DOCUMENTS_SINGLE_ADD_STATEMENT, ScriptType.JavaScript);
        _xProxy.ExecuteScript(REMOVE_DOCUMENT, ScriptType.JavaScript);
        var selectResult = _xProxy.ExecuteScript(FIND_ALL_DOCUMENTS_IN_COLLECTION, ScriptType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 2, DATA_NOT_MATCH);
      }
      finally
      {
        if (Command != null)
        {
          Command.Dispose();
        }

        SetUp.ExecuteSql(DROP_TEST_DB_SQL_SYNTAX);
        CloseConnection();
      }
    }

    /// <summary>
    /// Initializes the <see cref="MySqlXProxy"/> instance with common statements
    /// </summary>
    private void InitXProxy()
    {
      if (_xProxy != null)
      {
        return;
      }

      _xProxy = new MySqlXProxy(XConnString, true);
    }
  }
}
