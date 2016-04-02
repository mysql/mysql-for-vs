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
using MySql.VisualStudio.Tests.MySqlX.Base;
using MySqlX;
using MySqlX.Shell;
using Xunit;

namespace MySql.VisualStudio.Tests.MySqlX
{
  /// <summary>
  /// Class to test the CRUD operations through the NgShell Wrapper on Relational DB
  /// </summary>
  public class JsTableXShellTests : BaseTableTests, IUseFixture<SetUpXShell>
  {
    #region Fields

    /// <summary>
    /// Object to access and execute commands to the current database connection through the mysqlx protocol
    /// </summary>
    private MySqlShellClient _shellClient;

    #endregion Fields

    /// <summary>
    /// Test to create a Database using the <see cref="ShellClient"/> direclty.
    /// </summary>
    [Fact]
    public void CreateDatabase()
    {
      OpenConnection();
      MySqlDataReader reader = null;

      try
      {
        InitXShell();
        _shellClient.ExecuteToJavaScript(DROP_TEST_DATABASE);
        _shellClient.ExecuteToJavaScript(CREATE_TEST_DATABASE);
        Command = new MySqlCommand(SHOW_DBS_SQL_SYNTAX, Connection);
        reader = Command.ExecuteReader();
        bool success = false;

        while (reader.Read())
        {
          var retDb = reader.GetString(0);
          if (retDb != TEST_DATABASE_NAME)
            continue;
          success = true;
          reader.Close();
          break;
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
    /// Test to create a Table using the <see cref="ShellClient"/> direclty.
    /// </summary>
    [Fact]
    public void CreateTable()
    {
      OpenConnection();

      try
      {
        InitXShell();
        _shellClient.ExecuteToJavaScript(DROP_TEST_DATABASE);
        _shellClient.ExecuteToJavaScript(CREATE_TEST_DATABASE);
        _shellClient.ExecuteToJavaScript(USE_TEST_DATABASE);
        _shellClient.ExecuteToJavaScript(CREATE_TEST_TABLE);
        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, TEST_TABLE_NAME, TEST_DATABASE_NAME), Connection);
        var result = Command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, TEST_TABLE_NAME));

        _shellClient.ExecuteToJavaScript(DELETE_TEST_TABLE);
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
    /// Test to Insert, Update and Delete data from a table using the <see cref="ShellClient"/> direclty, executing the commands in a single line
    /// </summary>
    [Fact]
    public void InsertUpdateDelete()
    {
      OpenConnection();

      try
      {
        InitXShell();
        _shellClient.ExecuteToJavaScript(DROP_TEST_DATABASE);
        _shellClient.ExecuteToJavaScript(CREATE_TEST_DATABASE);
        _shellClient.ExecuteToJavaScript(USE_TEST_DATABASE);
        _shellClient.ExecuteToJavaScript(CREATE_TEST_TABLE);
        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, TEST_TABLE_NAME, TEST_DATABASE_NAME), Connection);

        var result = Command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, TEST_TABLE_NAME));

        _shellClient.ExecuteToJavaScript(SET_SCHEMA_VAR);
        _shellClient.ExecuteToJavaScript(SET_TABLE_VAR);
        _shellClient.ExecuteToJavaScript(INSERT_TWO_RECORDS);
        var selectResult = _shellClient.ExecuteToJavaScript(SELECT_TEST_TABLE) as RowResult;

        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == 2, DATA_NOT_MATCH);

        _shellClient.ExecuteToJavaScript(UPDATE_RECORD_SINGLE_LINE);
        selectResult = _shellClient.ExecuteToJavaScript(SELECT_UPDATED_RECORD) as RowResult;

        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == 1, DATA_NOT_MATCH);

        _shellClient.ExecuteToJavaScript(DELETE_RECORD);
        selectResult = _shellClient.ExecuteToJavaScript(SELECT_TEST_TABLE) as RowResult;

        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == 1, DATA_NOT_MATCH);
      }
      finally
      {
        Command?.Dispose();
        SetUp.ExecuteSql(DROP_TEST_DB_SQL_SYNTAX);
        CloseConnection();
      }
    }

    /// <summary>
    /// Test to Insert, Update and Delete data from a table using the <see cref="ShellClient"/> direclty, executing the commands in multiple lines
    /// </summary>
    [Fact]
    public void InsertUpdateDelete_JsonFormat()
    {
      OpenConnection();

      try
      {
        InitXShell();
        _shellClient.ExecuteToJavaScript(DROP_TEST_DATABASE);
        _shellClient.ExecuteToJavaScript(CREATE_TEST_DATABASE);
        _shellClient.ExecuteToJavaScript(USE_TEST_DATABASE);
        _shellClient.ExecuteToJavaScript(CREATE_TEST_TABLE);
        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, TEST_TABLE_NAME, TEST_DATABASE_NAME), Connection);

        var result = Command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, TEST_TABLE_NAME));

        _shellClient.ExecuteToJavaScript(SET_SCHEMA_VAR);
        _shellClient.ExecuteToJavaScript(SET_TABLE_VAR);
        _shellClient.ExecuteToJavaScript(INSERT_RECORD_JSON1);
        _shellClient.ExecuteToJavaScript(INSERT_RECORD_JSON2);
        var selectResult = _shellClient.ExecuteToJavaScript(SELECT_TEST_TABLE) as RowResult;

        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == 2, DATA_NOT_MATCH);

        _shellClient.ExecuteToJavaScript(UPDATE_RECORD_CMD1);
        _shellClient.ExecuteToJavaScript(UPDATE_RECORD_CMD2);
        _shellClient.ExecuteToJavaScript(UPDATE_RECORD_CMD3);
        selectResult = _shellClient.ExecuteToJavaScript(SELECT_UPDATED_RECORD) as RowResult;

        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == 1, DATA_NOT_MATCH);

        _shellClient.ExecuteToJavaScript(DELETE_RECORD);
        selectResult = _shellClient.ExecuteToJavaScript(SELECT_TEST_TABLE) as RowResult;

        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == 1, DATA_NOT_MATCH);
      }
      finally
      {
        Command?.Dispose();
        SetUp.ExecuteSql(DROP_TEST_DB_SQL_SYNTAX);
        CloseConnection();
      }
    }

    /// <summary>
    /// Initializes the <see cref="MySqlShellClient"/> instance with common statements.
    /// </summary>
    private void InitXShell()
    {
      if (_shellClient != null)
        return;

      _shellClient = new MySqlShellClient();
      _shellClient.MakeConnection(XConnString);
      _shellClient.SwitchMode(Mode.JScript);
    }
  }
}
