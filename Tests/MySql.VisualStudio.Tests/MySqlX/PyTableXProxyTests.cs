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

using System.Text;
using MySql.Data.MySqlClient;
using MySql.Data.VisualStudio.Editors;
using MySql.Data.VisualStudio.MySqlX;
using MySql.VisualStudio.Tests.MySqlX.Base;
using MySQL.Utility.Classes;
using Xunit;

namespace MySql.VisualStudio.Tests.MySqlX
{
  /// <summary>
  /// Class to test the CRUD operations through the NgShell Wrapper on Relational DB
  /// </summary>
  public class PyTableXProxyTests : PyTableTests, IUseFixture<SetUpXShell>
  {
    #region Fields

    /// <summary>
    /// Object to access and execute commands to the current database connection through the mysqlx protocol
    /// </summary>
    private MySqlXProxy _xProxy;

    #endregion Fields

    /// <summary>
    /// Test to create a Database using our custom implementation of the NgWrapper
    /// </summary>
    //[Fact]
    public void CreateDatabase()
    {
      OpenConnection();
      MySqlDataReader reader = null;

      try
      {
        InitXProxy();

        _xProxy.ExecuteScript(DROP_TEST_DATABASE, ScriptType.Python);
        _xProxy.ExecuteScript(CREATE_TEST_DATABASE, ScriptType.Python);
        Command = new MySqlCommand(SHOW_DBS, Connection);
        reader = Command.ExecuteReader();
        bool success = false;
        while (reader.Read())
        {
          var retDb = reader.GetString(0);
          if (retDb == TEST_DATABASE_NAME)
          {
            success = true;
            reader.Close();
            break;
          }
        }
        Assert.True(success, string.Format(DB_NOT_FOUND, TEST_DATABASE_NAME));
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
    /// Test to create a Table using our custom implementation of the NgWrapper
    /// </summary>
    //[Fact]
    public void CreateTable()
    {
      OpenConnection();

      try
      {
        InitXProxy();

        _xProxy.ExecuteScript(DROP_TEST_DATABASE, ScriptType.Python);
        _xProxy.ExecuteScript(CREATE_TEST_DATABASE, ScriptType.Python);
        _xProxy.ExecuteScript(USE_TEST_DATABASE, ScriptType.Python);
        _xProxy.ExecuteScript(CREATE_TEST_TABLE, ScriptType.Python);
        Command = new MySqlCommand(string.Format(SEARCH_TABLE, TEST_TABLE_NAME, TEST_DATABASE_NAME), Connection);
        var result = Command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, TEST_TABLE_NAME));

        _xProxy.ExecuteScript(DELETE_TEST_TABLE, ScriptType.Python);
        result = Command.ExecuteScalar();
        int.TryParse(result.ToString(), out count);
        Assert.True(count == 0, string.Format(TABLE_NOT_DELETED, TEST_TABLE_NAME));
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
    /// Test to Insert, Update and Delete record from a table using our custom implementation of the NgWrapper, executing the commands in multiple lines and in a single script
    /// </summary>
    //[Fact]
    public void Insert_JsonFormat_SingleScript()
    {
      OpenConnection();

      try
      {
        InitXProxy();

        var script = new StringBuilder();
        script.AppendLine(DROP_TEST_DATABASE);
        script.AppendLine(CREATE_TEST_DATABASE);
        script.AppendLine(USE_TEST_DATABASE);
        script.AppendLine(CREATE_TEST_TABLE);

        script.AppendLine(SET_SCHEMA_VAR);
        script.AppendLine(SET_TABLE_VAR);
        script.AppendLine(INSERT_RECORD_JSON1);
        script.AppendLine(INSERT_RECORD_JSON2);

        Command = new MySqlCommand(string.Format(SEARCH_TABLE, TEST_TABLE_NAME, TEST_DATABASE_NAME), Connection);

        var result = Command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, TEST_TABLE_NAME));

        var selectResult = _xProxy.ExecuteScript(SELECT_TEST_TABLE, ScriptType.Python);
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
    /// Test to Insert, Update and Delete data from a table using the NgWrapper, executing the commands in a single line
    /// </summary>
    [Fact]
    public void InsertUpdateDelete_AllTests()
    {
      OpenConnection();
      MySqlDataReader reader = null;

      try
      {
        InitXProxy();

        //Tokenizer and batch script tests
        var script = new StringBuilder();
        script.AppendLine(COMMENT_SINGLE_LINE_1);
        script.AppendLine(COMMENT_SINGLE_LINE_2);
        script.AppendLine(DROP_TEST_DATABASE);
        script.AppendLine(CREATE_TEST_DATABASE);
        script.AppendLine(USE_TEST_DATABASE);
        script.AppendLine(CREATE_TEST_TABLE);
        script.AppendLine(SET_SCHEMA_VAR);
        script.AppendLine(SET_TABLE_VAR);
        script.AppendLine(COMMENT_MULTI_LINE_1);
        script.AppendLine(COMMENT_MULTI_LINE_2);
        script.AppendLine(COMMENT_MULTI_LINE_3);
        script.AppendLine(INSERT_RECORD_JSON1);
        script.AppendLine(INSERT_RECORD_JSON2);

        var tokenizer = new MyPythonTokenizer(script.ToString());
        _xProxy.ExecuteScript(tokenizer.BreakIntoStatements().ToArray(), ScriptType.Python);
        Command = new MySqlCommand(string.Format(SEARCH_TABLE, TEST_TABLE_NAME, TEST_DATABASE_NAME), Connection);
        var result = Command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, TEST_TABLE_NAME));
        var selectResult = _xProxy.ExecuteScript(SELECT_TEST_TABLE, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 2, DATA_NOT_MATCH);

        //Create Schema
        _xProxy.ExecuteScript(DROP_TEST_DATABASE, ScriptType.Python);
        _xProxy.ExecuteScript(CREATE_TEST_DATABASE, ScriptType.Python);
        Command = new MySqlCommand(SHOW_DBS, Connection);
        reader = Command.ExecuteReader();
        bool success = false;
        while (reader.Read())
        {
          var retDb = reader.GetString(0);
          if (retDb == TEST_DATABASE_NAME)
          {
            success = true;
            reader.Close();
            break;
          }
        }

        Assert.True(success, string.Format(DB_NOT_FOUND, TEST_DATABASE_NAME));

        //Create Table
        _xProxy.ExecuteScript(USE_TEST_DATABASE, ScriptType.Python);
        _xProxy.ExecuteScript(CREATE_TEST_TABLE, ScriptType.Python);
        Command = new MySqlCommand(string.Format(SEARCH_TABLE, TEST_TABLE_NAME, TEST_DATABASE_NAME), Connection);
        result = Command.ExecuteScalar();
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, TEST_TABLE_NAME));

        //Insert Rows
        _xProxy.ExecuteScript(SET_SCHEMA_VAR, ScriptType.Python);
        _xProxy.ExecuteScript(SET_TABLE_VAR, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_TWO_RECORDS, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(SELECT_TEST_TABLE, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 2, DATA_NOT_MATCH);

        //Update Rows
        _xProxy.ExecuteScript(UPDATE_RECORD_SINGLE_LINE, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(SELECT_UPDATED_RECORD, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 1, DATA_NOT_MATCH);

        //Delete Rows
        _xProxy.ExecuteScript(DELETE_RECORD_SINGLE_LINE, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(SELECT_TEST_TABLE, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 1, DATA_NOT_MATCH);

        //Delete Table
        _xProxy.ExecuteScript(DELETE_TEST_TABLE, ScriptType.Python);
        result = Command.ExecuteScalar();
        int.TryParse(result.ToString(), out count);
        Assert.True(count == 0, string.Format(TABLE_NOT_DELETED, TEST_TABLE_NAME));

        //Delete Schema
        _xProxy.ExecuteScript(DROP_TEST_DATABASE, ScriptType.Python);
        Command = new MySqlCommand(SHOW_DBS, Connection);
        reader = Command.ExecuteReader();
        success = true;
        while (reader.Read())
        {
          var retDb = reader.GetString(0);
          if (retDb == TEST_DATABASE_NAME)
          {
            success = false;
            reader.Close();
            break;
          }
        }

        Assert.True(success, string.Format(DB_NOT_FOUND, TEST_DATABASE_NAME));
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
    /// Test to Insert, Update and Delete record from a table using our custom implementation of the NgWrapper, executing the commands in a single line
    /// </summary>
    //[Fact]
    public void InsertUpdateDelete()
    {
      OpenConnection();

      try
      {
        InitXProxy();

        _xProxy.ExecuteScript(DROP_TEST_DATABASE, ScriptType.Python);
        _xProxy.ExecuteScript(CREATE_TEST_DATABASE, ScriptType.Python);
        _xProxy.ExecuteScript(USE_TEST_DATABASE, ScriptType.Python);
        _xProxy.ExecuteScript(CREATE_TEST_TABLE, ScriptType.Python);
        Command = new MySqlCommand(string.Format(SEARCH_TABLE, TEST_TABLE_NAME, TEST_DATABASE_NAME), Connection);

        var result = Command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, TEST_TABLE_NAME));

        _xProxy.ExecuteScript(SET_SCHEMA_VAR, ScriptType.Python);
        _xProxy.ExecuteScript(SET_TABLE_VAR, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_TWO_RECORDS, ScriptType.Python);
        var selectResult = _xProxy.ExecuteScript(SELECT_TEST_TABLE, ScriptType.Python);

        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 2, DATA_NOT_MATCH);

        _xProxy.ExecuteScript(UPDATE_RECORD_SINGLE_LINE, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(SELECT_UPDATED_RECORD, ScriptType.Python);

        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 1, DATA_NOT_MATCH);

        _xProxy.ExecuteScript(DELETE_RECORD_SINGLE_LINE, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(SELECT_TEST_TABLE, ScriptType.Python);

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
    /// Test to Insert, Update and Delete record from a table using our custom implementation of the NgWrapper, executing the commands in multiple lines
    /// </summary>
    //[Fact]
    public void InsertUpdateDelete_JsonFormat()
    {
      OpenConnection();

      try
      {
        InitXProxy();

        _xProxy.ExecuteScript(DROP_TEST_DATABASE, ScriptType.Python);
        _xProxy.ExecuteScript(CREATE_TEST_DATABASE, ScriptType.Python);
        _xProxy.ExecuteScript(USE_TEST_DATABASE, ScriptType.Python);
        _xProxy.ExecuteScript(CREATE_TEST_TABLE, ScriptType.Python);
        Command = new MySqlCommand(string.Format(SEARCH_TABLE, TEST_TABLE_NAME, TEST_DATABASE_NAME), Connection);

        var result = Command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, TEST_TABLE_NAME));

        _xProxy.ExecuteScript(SET_SCHEMA_VAR, ScriptType.Python);
        _xProxy.ExecuteScript(SET_TABLE_VAR, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_RECORD_JSON1, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_RECORD_JSON2, ScriptType.Python);
        var selectResult = _xProxy.ExecuteScript(SELECT_TEST_TABLE, ScriptType.Python);

        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 2, DATA_NOT_MATCH);

        _xProxy.ExecuteScript(UPDATE_RECORD_CMD1, ScriptType.Python);
        _xProxy.ExecuteScript(UPDATE_RECORD_CMD2, ScriptType.Python);
        _xProxy.ExecuteScript(UPDATE_RECORD_CMD3, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(SELECT_UPDATED_RECORD, ScriptType.Python);

        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 1, DATA_NOT_MATCH);

        _xProxy.ExecuteScript(DELETE_RECORD_CMD1, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(SELECT_TEST_TABLE, ScriptType.Python);

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
    /// Test to Insert, Update and Delete record from a table using our custom implementation of the NgWrapper, executing the commands in multiple lines and in a single script
    /// </summary>
    //[Fact]
    public void InsertUpdateDelete_JsonFormat_SingleScript()
    {
      OpenConnection();

      try
      {
        InitXProxy();

        _xProxy.ExecuteScript(DROP_TEST_DATABASE, ScriptType.Python);
        _xProxy.ExecuteScript(CREATE_TEST_DATABASE, ScriptType.Python);
        _xProxy.ExecuteScript(USE_TEST_DATABASE, ScriptType.Python);
        _xProxy.ExecuteScript(CREATE_TEST_TABLE, ScriptType.Python);

        _xProxy.ExecuteScript(SET_SCHEMA_VAR, ScriptType.Python);
        _xProxy.ExecuteScript(SET_TABLE_VAR, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_RECORD_JSON1, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_RECORD_JSON2, ScriptType.Python);
        _xProxy.ExecuteScript(UPDATE_RECORD_CMD1, ScriptType.Python);
        _xProxy.ExecuteScript(UPDATE_RECORD_CMD2, ScriptType.Python);
        _xProxy.ExecuteScript(UPDATE_RECORD_CMD3, ScriptType.Python);
        _xProxy.ExecuteScript(DELETE_RECORD_CMD1, ScriptType.Python);

        Command = new MySqlCommand(string.Format(SEARCH_TABLE, TEST_TABLE_NAME, TEST_DATABASE_NAME), Connection);

        var result = Command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, TEST_TABLE_NAME));

        var selectResult = _xProxy.ExecuteScript(SELECT_TEST_TABLE, ScriptType.Python);
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
    /// Test to Insert, Update and Delete record from a table using our custom implementation of the NgWrapper, executing the commands in a single line and in a single script
    /// </summary>
    //[Fact]
    public void InsertUpdateDelete_SingleScript()
    {
      OpenConnection();

      try
      {
        InitXProxy();

        _xProxy.ExecuteScript(DROP_TEST_DATABASE, ScriptType.Python);
        _xProxy.ExecuteScript(CREATE_TEST_DATABASE, ScriptType.Python);
        _xProxy.ExecuteScript(USE_TEST_DATABASE, ScriptType.Python);
        _xProxy.ExecuteScript(CREATE_TEST_TABLE, ScriptType.Python);
        _xProxy.ExecuteScript(SET_SCHEMA_VAR, ScriptType.Python);
        _xProxy.ExecuteScript(SET_TABLE_VAR, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_TWO_RECORDS, ScriptType.Python);
        _xProxy.ExecuteScript(UPDATE_RECORD_SINGLE_LINE, ScriptType.Python);
        _xProxy.ExecuteScript(DELETE_RECORD_SINGLE_LINE, ScriptType.Python);

        Command = new MySqlCommand(string.Format(SEARCH_TABLE, TEST_TABLE_NAME, TEST_DATABASE_NAME), Connection);

        var result = Command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, TEST_TABLE_NAME));

        var selectResult = _xProxy.ExecuteScript(SELECT_TEST_TABLE, ScriptType.Python);
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
    /// Parse a RowResult to a DocResult data returned from the server when a statment that returns a result set is executed
    /// </summary>
    //[Fact]
    public void ParseTableResultToDocumentResult()
    {
      OpenConnection();

      try
      {
        InitXProxy();

        _xProxy.ExecuteScript(DROP_TEST_DATABASE, ScriptType.Python);
        _xProxy.ExecuteScript(CREATE_TEST_DATABASE, ScriptType.Python);
        _xProxy.ExecuteScript(USE_TEST_DATABASE, ScriptType.Python);
        _xProxy.ExecuteScript(CREATE_TEST_TABLE, ScriptType.Python);
        Command = new MySqlCommand(string.Format(SEARCH_TABLE, TEST_TABLE_NAME, TEST_DATABASE_NAME), Connection);

        var result = Command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, TEST_TABLE_NAME));

        _xProxy.ExecuteScript(SET_SCHEMA_VAR, ScriptType.Python);
        _xProxy.ExecuteScript(SET_TABLE_VAR, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_TWO_RECORDS, ScriptType.Python);
        var selectResult = _xProxy.ExecuteScript(SELECT_FOR_TABLE_RESULT, ScriptType.Python);

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
