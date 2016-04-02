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
using MySql.VisualStudio.Tests.MySqlX.Base;
using MySQL.Utility.Classes;
using Xunit;

namespace MySql.VisualStudio.Tests.MySqlX
{
  public class JsTableXProxyTests : BaseTableTests, IUseFixture<SetUpXShell>
  {
    #region Fields

    /// <summary>
    /// Object to access and execute commands to the current database connection through the mysqlx protocol
    /// </summary>
    private MyTestXProxy _xProxy;

    #endregion Fields

    /// <summary>
    /// Test to create a Database using the <see cref="MyTestXProxy"/>.
    /// </summary>
    [Fact]
    public void CreateDatabase()
    {
      OpenConnection();
      MySqlDataReader reader = null;

      try
      {
        InitXProxy();
        _xProxy.ExecuteScript(DROP_TEST_DATABASE, ScriptType.JavaScript);
        _xProxy.ExecuteScript(CREATE_TEST_DATABASE, ScriptType.JavaScript);
        Command = new MySqlCommand(SHOW_DBS_SQL_SYNTAX, Connection);
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

        Command?.Dispose();
        SetUp.ExecuteSql(DROP_TEST_DB_SQL_SYNTAX);
        CloseConnection();
      }
    }

    /// <summary>
    /// Test to create a Table using the <see cref="MyTestXProxy"/>.
    /// </summary>
    [Fact]
    public void CreateTable()
    {
      OpenConnection();

      try
      {
        InitXProxy();
        _xProxy.ExecuteScript(DROP_TEST_DATABASE, ScriptType.JavaScript);
        _xProxy.ExecuteScript(CREATE_TEST_DATABASE, ScriptType.JavaScript);
        _xProxy.ExecuteScript(USE_TEST_DATABASE, ScriptType.JavaScript);
        _xProxy.ExecuteScript(CREATE_TEST_TABLE, ScriptType.JavaScript);
        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, TEST_TABLE_NAME, TEST_DATABASE_NAME), Connection);
        var result = Command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, TEST_TABLE_NAME));

        _xProxy.ExecuteScript(DELETE_TEST_TABLE, ScriptType.JavaScript);
        result = Command.ExecuteScalar();
        int.TryParse(result.ToString(), out count);
        Assert.True(count == 0, string.Format(TABLE_NOT_DELETED, TEST_TABLE_NAME));
      }
      finally
      {
        Command?.Dispose();
        SetUp.ExecuteSql(DROP_TEST_DB_SQL_SYNTAX);
        CloseConnection();
      }
    }

    /// <summary>
    /// Test to Insert, Update and Delete record from a table using the <see cref="MyTestXProxy"/>, executing the commands in a single line
    /// </summary>
    [Fact]
    public void InsertUpdateDelete()
    {
      OpenConnection();

      try
      {
        InitXProxy();
        _xProxy.ExecuteScript(DROP_TEST_DATABASE, ScriptType.JavaScript);
        _xProxy.ExecuteScript(CREATE_TEST_DATABASE, ScriptType.JavaScript);
        _xProxy.ExecuteScript(USE_TEST_DATABASE, ScriptType.JavaScript);
        _xProxy.ExecuteScript(CREATE_TEST_TABLE, ScriptType.JavaScript);
        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, TEST_TABLE_NAME, TEST_DATABASE_NAME), Connection);

        var result = Command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, TEST_TABLE_NAME));

        _xProxy.ExecuteScript(SET_SCHEMA_VAR, ScriptType.JavaScript);
        _xProxy.ExecuteScript(SET_TABLE_VAR, ScriptType.JavaScript);
        _xProxy.ExecuteScript(INSERT_TWO_RECORDS, ScriptType.JavaScript);
        var selectResult = _xProxy.ExecuteScript(SELECT_TEST_TABLE, ScriptType.JavaScript);

        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 2, DATA_NOT_MATCH);

        _xProxy.ExecuteScript(UPDATE_RECORD_SINGLE_LINE, ScriptType.JavaScript);
        selectResult = _xProxy.ExecuteScript(SELECT_UPDATED_RECORD, ScriptType.JavaScript);

        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 1, DATA_NOT_MATCH);

        _xProxy.ExecuteScript(DELETE_RECORD, ScriptType.JavaScript);
        selectResult = _xProxy.ExecuteScript(SELECT_TEST_TABLE, ScriptType.JavaScript);

        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 1, DATA_NOT_MATCH);
      }
      finally
      {
        Command?.Dispose();
        SetUp.ExecuteSql(DROP_TEST_DB_SQL_SYNTAX);
        CloseConnection();
      }
    }

    /// <summary>
    /// Test to Insert, Update and Delete record from a table using the <see cref="MyTestXProxy"/>, executing the commands in multiple lines
    /// </summary>
    [Fact]
    public void InsertUpdateDelete_JsonFormat()
    {
      OpenConnection();

      try
      {
        InitXProxy();
        _xProxy.ExecuteScript(DROP_TEST_DATABASE, ScriptType.JavaScript);
        _xProxy.ExecuteScript(CREATE_TEST_DATABASE, ScriptType.JavaScript);
        _xProxy.ExecuteScript(USE_TEST_DATABASE, ScriptType.JavaScript);
        _xProxy.ExecuteScript(CREATE_TEST_TABLE, ScriptType.JavaScript);
        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, TEST_TABLE_NAME, TEST_DATABASE_NAME), Connection);

        var result = Command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, TEST_TABLE_NAME));

        _xProxy.ExecuteScript(SET_SCHEMA_VAR, ScriptType.JavaScript);
        _xProxy.ExecuteScript(SET_TABLE_VAR, ScriptType.JavaScript);
        _xProxy.ExecuteScript(INSERT_RECORD_JSON1, ScriptType.JavaScript);
        _xProxy.ExecuteScript(INSERT_RECORD_JSON2, ScriptType.JavaScript);
        var selectResult = _xProxy.ExecuteScript(SELECT_TEST_TABLE, ScriptType.JavaScript);

        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 2, DATA_NOT_MATCH);

        _xProxy.ExecuteScript(UPDATE_RECORD_CMD1, ScriptType.JavaScript);
        _xProxy.ExecuteScript(UPDATE_RECORD_CMD2, ScriptType.JavaScript);
        _xProxy.ExecuteScript(UPDATE_RECORD_CMD3, ScriptType.JavaScript);
        selectResult = _xProxy.ExecuteScript(SELECT_UPDATED_RECORD, ScriptType.JavaScript);

        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 1, DATA_NOT_MATCH);

        _xProxy.ExecuteScript(DELETE_RECORD, ScriptType.JavaScript);
        selectResult = _xProxy.ExecuteScript(SELECT_TEST_TABLE, ScriptType.JavaScript);

        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 1, DATA_NOT_MATCH);
      }
      finally
      {
        Command?.Dispose();
        SetUp.ExecuteSql(DROP_TEST_DB_SQL_SYNTAX);
        CloseConnection();
      }
    }

    /// <summary>
    /// Test to Insert, Update and Delete record from a table using the <see cref="MyTestXProxy"/>, executing the commands in a single line and in a single script
    /// </summary>
    [Fact]
    public void InsertUpdateDelete_SingleScript()
    {
      OpenConnection();

      try
      {
        InitXProxy();
        var script = new StringBuilder();
        script.AppendLine(JAVASCRIPT_COMMENT_SINGLE_LINE_1);
        script.AppendLine(JAVASCRIPT_COMMENT_SINGLE_LINE_2);
        script.AppendLine(DROP_TEST_DATABASE);
        script.AppendLine(CREATE_TEST_DATABASE);
        script.AppendLine(USE_TEST_DATABASE);
        script.AppendLine(CREATE_TEST_TABLE);
        script.AppendLine(SET_SCHEMA_VAR);
        script.AppendLine(SET_TABLE_VAR);
        script.AppendLine(INSERT_TWO_RECORDS);
        script.AppendLine(JAVASCRIPT_COMMENT_MULTI_LINE_1);
        script.AppendLine(JAVASCRIPT_COMMENT_MULTI_LINE_2);
        script.AppendLine(JAVASCRIPT_COMMENT_MULTI_LINE_3);
        script.AppendLine(UPDATE_RECORD_SINGLE_LINE);
        script.AppendLine(DELETE_RECORD);

        var tokenizer = new MyJsTokenizer(script.ToString());
        _xProxy.ExecuteScript(tokenizer.BreakIntoStatements().ToArray(), ScriptType.JavaScript);

        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, TEST_TABLE_NAME, TEST_DATABASE_NAME), Connection);

        var result = Command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, TEST_TABLE_NAME));

        var selectResult = _xProxy.ExecuteScript(SELECT_TEST_TABLE, ScriptType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 1, DATA_NOT_MATCH);
      }
      finally
      {
        Command?.Dispose();
        SetUp.ExecuteSql(DROP_TEST_DB_SQL_SYNTAX);
        CloseConnection();
      }
    }

    /// <summary>
    /// Test to Insert, Update and Delete record from a table using the <see cref="MyTestXProxy"/>, executing the commands in multiple lines and in a single script
    /// </summary>
    [Fact]
    public void InsertUpdateDelete_JsonFormat_SingleScript()
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
        script.AppendLine(UPDATE_RECORD_CMD1);
        script.AppendLine(UPDATE_RECORD_CMD2);
        script.AppendLine(UPDATE_RECORD_CMD3);
        script.AppendLine(DELETE_RECORD);

        var tokenizer = new MyJsTokenizer(script.ToString());
        _xProxy.ExecuteScript(tokenizer.BreakIntoStatements().ToArray(), ScriptType.JavaScript);

        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, TEST_TABLE_NAME, TEST_DATABASE_NAME), Connection);

        var result = Command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, TEST_TABLE_NAME));

        var selectResult = _xProxy.ExecuteScript(SELECT_TEST_TABLE, ScriptType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 1, DATA_NOT_MATCH);
      }
      finally
      {
        Command?.Dispose();
        SetUp.ExecuteSql(DROP_TEST_DB_SQL_SYNTAX);
        CloseConnection();
      }
    }

    /// <summary>
    /// Parse a RowResult to a RowResult data returned from the server when a statement that returns a result set is executed
    /// </summary>
    [Fact]
    public void ParseTableResultToDocumentResult()
    {
      OpenConnection();

      try
      {
        InitXProxy();
        _xProxy.ExecuteScript(DROP_TEST_DATABASE, ScriptType.JavaScript);
        _xProxy.ExecuteScript(CREATE_TEST_DATABASE, ScriptType.JavaScript);
        _xProxy.ExecuteScript(USE_TEST_DATABASE, ScriptType.JavaScript);
        _xProxy.ExecuteScript(CREATE_TEST_TABLE, ScriptType.JavaScript);
        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, TEST_TABLE_NAME, TEST_DATABASE_NAME), Connection);

        var result = Command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, TEST_TABLE_NAME));

        _xProxy.ExecuteScript(SET_SCHEMA_VAR, ScriptType.JavaScript);
        _xProxy.ExecuteScript(SET_TABLE_VAR, ScriptType.JavaScript);
        _xProxy.ExecuteScript(INSERT_TWO_RECORDS, ScriptType.JavaScript);
        var selectResult = _xProxy.ExecuteScript(SELECT_TEST_TABLE, ScriptType.JavaScript);

        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 2, DATA_NOT_MATCH);
      }
      finally
      {
        Command?.Dispose();
        SetUp.ExecuteSql(DROP_TEST_DB_SQL_SYNTAX);
        CloseConnection();
      }
    }

    /// <summary>
    /// Initializes the <see cref="MyTestXProxy"/> instance with common statements
    /// </summary>
    private void InitXProxy()
    {
      if (_xProxy != null)
      {
        return;
      }

      _xProxy = new MyTestXProxy(XConnString, true);
    }
  }
}
