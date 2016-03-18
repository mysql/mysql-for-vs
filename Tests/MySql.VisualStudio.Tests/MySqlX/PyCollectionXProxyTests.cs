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

using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using MySql.Data.VisualStudio.Editors;
using MySql.Data.VisualStudio.MySqlX;
using MySql.VisualStudio.Tests.MySqlX.Base;
using MySQL.Utility.Classes;
using Xunit;

namespace MySql.VisualStudio.Tests.MySqlX
{
  public class PyCollectionXProxyTests : PyCollectionTests, IUseFixture<SetUpXShell>
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
    // [Fact]
    public void AddFind()
    {
      OpenConnection();

      try
      {
        InitXProxy();
        _xProxy.ExecuteScript(DROP_SCHEMA_IF_EXISTS, ScriptType.Python);
        _xProxy.ExecuteScript(CREATE_SCHEMA_TEST, ScriptType.Python);
        _xProxy.ExecuteScript(USE_SCHEMA_TEST, ScriptType.Python);
        _xProxy.ExecuteScript(CREATE_COLLECTION_TEST, ScriptType.Python);
        Command = new MySqlCommand(string.Format(SEARCH_TABLE, TEST_COLLECTION_NAME, TEST_SCHEMA_NAME), Connection);

        var result = Command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(SCHEMA_NOT_FOUND, TEST_COLLECTION_NAME));

        _xProxy.ExecuteScript(SET_COLLECTION_VAR, ScriptType.Python);

        //Test single add
        _xProxy.ExecuteScript(ADD_JSON_DOCUMENT1, ScriptType.Python);
        List<Dictionary<string, object>> selectResult = _xProxy.ExecuteScript(FIND_ALL_DOCUMENTS_IN_COLLECTION, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 1, DATA_NOT_MATCH);

        //Test single add again
        _xProxy.ExecuteScript(ADD_JSON_DOCUMENT2, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(FIND_ALL_DOCUMENTS_IN_COLLECTION, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 2, DATA_NOT_MATCH);

        //Test multiple documents add statement
        _xProxy.ExecuteScript(ADD_MULTIPLE_DOCUMENTS_SINGLE_ADD_STATEMENT, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(FIND_ALL_DOCUMENTS_IN_COLLECTION, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 5, DATA_NOT_MATCH);

        //Test multiple add statements with single documents
        _xProxy.ExecuteScript(ADD_MULTIPLE_DOCUMENTS_MULTIPLE_ADD_STATEMENTS, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(FIND_ALL_DOCUMENTS_IN_COLLECTION, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 7, DATA_NOT_MATCH);

        selectResult = _xProxy.ExecuteScript(FIND_SPECIFIC_DOCUMENT_TEST, ScriptType.Python);
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
    // [Fact]
    public void CreateCollection()
    {
      OpenConnection();

      try
      {
        InitXProxy();
        _xProxy.ExecuteScript(DROP_SCHEMA_IF_EXISTS, ScriptType.Python);
        _xProxy.ExecuteScript(CREATE_SCHEMA_TEST, ScriptType.Python);
        _xProxy.ExecuteScript(USE_SCHEMA_TEST, ScriptType.Python);
        _xProxy.ExecuteScript(CREATE_COLLECTION_TEST, ScriptType.Python);
        Command = new MySqlCommand(string.Format(SEARCH_TABLE, TEST_COLLECTION_NAME, TEST_SCHEMA_NAME), Connection);
        var result = Command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(COLLECTION_NOT_FOUND, TEST_COLLECTION_NAME));

        _xProxy.ExecuteScript(DELETE_COLLECTION_TEST, ScriptType.Python);
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
    // [Fact]
    public void CreateSchema()
    {
      OpenConnection();
      MySqlDataReader reader = null;

      try
      {
        InitXProxy();
        _xProxy.ExecuteScript(DROP_SCHEMA_IF_EXISTS, ScriptType.Python);
        _xProxy.ExecuteScript(CREATE_SCHEMA_TEST, ScriptType.Python);
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
        _xProxy.ExecuteScript(DROP_SCHEMA_TEST, ScriptType.Python);
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
    /// Test to Insert, Update and Delete record from a table using the <see cref="MySqlXProxy"/>, executing the commands in multiple lines and in a single script
    /// </summary>
    [Fact]
    public void Insert_JsonFormat_AllTests()
    {
      OpenConnection();
      MySqlDataReader reader = null;

      try
      {
        InitXProxy();

        //Create Schema test
        _xProxy.ExecuteScript(DROP_SCHEMA_IF_EXISTS, ScriptType.Python);
        _xProxy.ExecuteScript(CREATE_SCHEMA_TEST, ScriptType.Python);
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

        //Create Collection test
        var script = new StringBuilder();
        script.AppendLine(USE_SCHEMA_TEST);
        _xProxy.ExecuteScript(CREATE_COLLECTION_TEST, ScriptType.Python);
        Command = new MySqlCommand(string.Format(SEARCH_TABLE, TEST_COLLECTION_NAME, TEST_SCHEMA_NAME), Connection);
        var result = Command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(COLLECTION_NOT_FOUND, TEST_COLLECTION_NAME));

        //Batch Script Test & Single Insert Test
        script.AppendLine(SET_SCHEMA_VAR);
        script.AppendLine(SET_COLLECTION_VAR);
        script.AppendLine(ADD_JSON_DOCUMENT1);
        script.AppendLine(ADD_JSON_DOCUMENT2);
        var tokenizer = new MyPythonTokenizer(script.ToString());
        _xProxy.ExecuteScript(tokenizer.BreakIntoStatements().ToArray(), ScriptType.Python);

        List<Dictionary<string, object>> selectResult = _xProxy.ExecuteScript(FIND_ALL_DOCUMENTS_IN_COLLECTION, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 2, DATA_NOT_MATCH);

        //Test multiple documents add statement
        _xProxy.ExecuteScript(ADD_MULTIPLE_DOCUMENTS_SINGLE_ADD_STATEMENT, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(FIND_ALL_DOCUMENTS_IN_COLLECTION, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 5, DATA_NOT_MATCH);

        //Test multiple add statements with single documents
        _xProxy.ExecuteScript(ADD_MULTIPLE_DOCUMENTS_MULTIPLE_ADD_STATEMENTS, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(FIND_ALL_DOCUMENTS_IN_COLLECTION, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 7, DATA_NOT_MATCH);

        //Find Test
        selectResult = _xProxy.ExecuteScript(FIND_SPECIFIC_DOCUMENT_TEST, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 1, DATA_NOT_MATCH);

        //Update record test
        _xProxy.ExecuteScript(MODIFY_DOCUMENT1, ScriptType.Python);
        _xProxy.ExecuteScript(MODIFY_DOCUMENT2, ScriptType.Python);
        _xProxy.ExecuteScript(MODIFY_DOCUMENT3, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(SELECT_UPDATED_RECORD, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 1, DATA_NOT_MATCH);

        //Remove Documents test
        _xProxy.ExecuteScript(REMOVE_DOCUMENT, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(FIND_ALL_DOCUMENTS_IN_COLLECTION, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 6, DATA_NOT_MATCH);

        //Drop Collection test
        _xProxy.ExecuteScript(DELETE_COLLECTION_TEST, ScriptType.Python);
        Command = new MySqlCommand(string.Format(SEARCH_TABLE, TEST_COLLECTION_NAME, TEST_SCHEMA_NAME), Connection);
        result = Command.ExecuteScalar();
        int.TryParse(result.ToString(), out count);
        Assert.True(count == 0, string.Format(COLLECTION_NOT_DELETED, TEST_COLLECTION_NAME));

        //Drop Schema test
        _xProxy.ExecuteScript(DROP_SCHEMA_TEST, ScriptType.Python);
        Command = new MySqlCommand(SHOW_DBS, Connection);
        reader = Command.ExecuteReader();
        success = true;
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
    /// Test to Insert, Update and Delete record from a table using the <see cref="MySqlXProxy"/>, executing the commands in multiple lines and in a single script.
    /// </summary>
    // [Fact]
    public void Insert_JsonFormat_SingleScript()
    {
      OpenConnection();

      try
      {
        InitXProxy();
        var script = new StringBuilder();
        script.AppendLine(DROP_SCHEMA_IF_EXISTS);
        script.AppendLine(CREATE_SCHEMA_TEST);
        script.AppendLine(USE_SCHEMA_TEST);
        script.AppendLine(CREATE_COLLECTION_TEST);

        script.AppendLine(SET_SCHEMA_VAR);
        script.AppendLine(SET_COLLECTION_VAR);
        script.AppendLine(ADD_JSON_DOCUMENT1);
        script.AppendLine(ADD_JSON_DOCUMENT2);

        var tokenizer = new MyPythonTokenizer(script.ToString());
        _xProxy.ExecuteScript(tokenizer.BreakIntoStatements().ToArray(), ScriptType.Python);

        Command = new MySqlCommand(string.Format(SEARCH_TABLE, TEST_COLLECTION_NAME, TEST_SCHEMA_NAME), Connection);

        var result = Command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(COLLECTION_NOT_FOUND, TEST_COLLECTION_NAME));

        var selectResult = _xProxy.ExecuteScript(FIND_ALL_DOCUMENTS_IN_COLLECTION, ScriptType.Python);
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
    /// Test to Modify data from a collection using the <see cref="MySqlXProxy"/>.
    /// </summary>
    // [Fact]
    public void Modify()
    {
      OpenConnection();

      try
      {
        InitXProxy();
        _xProxy.ExecuteScript(DROP_SCHEMA_IF_EXISTS, ScriptType.Python);
        _xProxy.ExecuteScript(CREATE_SCHEMA_TEST, ScriptType.Python);
        _xProxy.ExecuteScript(USE_SCHEMA_TEST, ScriptType.Python);
        _xProxy.ExecuteScript(CREATE_COLLECTION_TEST, ScriptType.Python);
        Command = new MySqlCommand(string.Format(SEARCH_TABLE, TEST_COLLECTION_NAME, TEST_SCHEMA_NAME), Connection);

        var result = Command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(SCHEMA_NOT_FOUND, TEST_SCHEMA_NAME));

        _xProxy.ExecuteScript(SET_COLLECTION_VAR, ScriptType.Python);
        _xProxy.ExecuteScript(ADD_MULTIPLE_DOCUMENTS_SINGLE_ADD_STATEMENT, ScriptType.Python);
        _xProxy.ExecuteScript(MODIFY_DOCUMENT1, ScriptType.Python);
        _xProxy.ExecuteScript(MODIFY_DOCUMENT2, ScriptType.Python);
        _xProxy.ExecuteScript(MODIFY_DOCUMENT3, ScriptType.Python);
        var selectResult = _xProxy.ExecuteScript(SELECT_UPDATED_RECORD, ScriptType.Python);
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
    // [Fact]
    public void Remove()
    {
      OpenConnection();

      try
      {
        InitXProxy();
        _xProxy.ExecuteScript(DROP_SCHEMA_IF_EXISTS, ScriptType.Python);
        _xProxy.ExecuteScript(CREATE_SCHEMA_TEST, ScriptType.Python);
        _xProxy.ExecuteScript(USE_SCHEMA_TEST, ScriptType.Python);
        _xProxy.ExecuteScript(CREATE_COLLECTION_TEST, ScriptType.Python);
        Command = new MySqlCommand(string.Format(SEARCH_TABLE, TEST_COLLECTION_NAME, TEST_SCHEMA_NAME), Connection);

        var result = Command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(SCHEMA_NOT_FOUND, TEST_SCHEMA_NAME));

        _xProxy.ExecuteScript(SET_COLLECTION_VAR, ScriptType.Python);
        _xProxy.ExecuteScript(ADD_MULTIPLE_DOCUMENTS_SINGLE_ADD_STATEMENT, ScriptType.Python);
        _xProxy.ExecuteScript(REMOVE_DOCUMENT, ScriptType.Python);
        var selectResult = _xProxy.ExecuteScript(FIND_ALL_DOCUMENTS_IN_COLLECTION, ScriptType.Python);
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
    /// Setup information about the current schema test
    /// </summary>
    /// <param name="data">Current test instance configuration</param>
    public override void SetFixture(SetUpXShell data)
    {
      base.SetFixture(data);
      _xProxy = new MySqlXProxy(XConnString, true);

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
