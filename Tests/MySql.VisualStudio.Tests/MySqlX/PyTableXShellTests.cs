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

using System;
using MySql.Data.MySqlClient;
using MySql.Data.VisualStudio.Editors;
using MySql.VisualStudio.Tests.MySqlX.Base;
using MySqlX;
using MySqlX.Shell;
using Xunit;

namespace MySql.VisualStudio.Tests.MySqlX
{
  /// <summary>
  /// Class to test the CRUD operations through the XShell Wrapper on Relational DB
  /// </summary>
  public class PyTableXShellTests : BaseTableTests, IUseFixture<SetUpXShell>
  {
    #region Fields

    /// <summary>
    /// Object to access and execute commands to the current database connection through the mysqlx protocol
    /// </summary>
    private MySqlShellClient _shellClient;

    #endregion

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

        _shellClient.Execute(DropTestDatabaseIfExists);
        _shellClient.Execute(CreateTestDatabase);
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

        _shellClient.Execute(DropTestDatabaseIfExists);
        Command = new MySqlCommand(SHOW_DBS_SQL_SYNTAX, Connection);
        reader = Command.ExecuteReader();
        while (reader.Read())
        {
          var retSchema = reader.GetString(0);
          if (retSchema != TEST_DATABASE_NAME)
            continue;
          success = false;
          reader.Close();
          break;
        }

        Assert.True(success, string.Format(DB_NOT_DELETED, TEST_DATABASE_NAME));
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

        CloseConnection();
        DisposeShellClient();
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

        _shellClient.Execute(DropTestDatabaseIfExists);
        _shellClient.Execute(CreateTestDatabase);
        _shellClient.Execute(UseTestDatabase);

        _shellClient.Execute(CREATE_TEST_TABLE);
        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, TEST_TABLE_NAME, TEST_DATABASE_NAME), Connection);
        var result = Command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, TEST_TABLE_NAME));

        _shellClient.Execute(DropTestTableIfExists);
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

        CloseConnection();
        DisposeShellClient();
      }
    }

    /// <summary>
    /// Test to Delete records in a table using the <see cref="ShellClient"/>.
    /// </summary>
    [Fact]
    public void Delete()
    {
      OpenConnection();

      try
      {
        // We need to dispose the shell client, to avoid the error thrown by Python when handling multiple "sessions"
        // ToDo: Research how to fix the shell to avoid the error thrown by Python when running multiple tests sessions
        if (_shellClient != null)
        {
          _shellClient.Dispose();
        }

        InitXShell();

        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, SAKILA_X_CHARACTER_TABLE, SAKILA_X_SCHEMA_NAME), Connection);
        var result = Command.ExecuteScalar();
        int count;
        int charactersCount = CHARACTERS_FULL_COUNT;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, SAKILA_X_CHARACTER_TABLE));

        // Create test table
        _shellClient.Execute(DropTestDatabaseIfExists);
        _shellClient.Execute(CreateTestDatabase);
        _shellClient.Execute(UseTestDatabase);
        _shellClient.Execute(CREATE_TEST_TABLE);
        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, TEST_TABLE_NAME, TEST_DATABASE_NAME), Connection);
        result = Command.ExecuteScalar();
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, TEST_TABLE_NAME));

        // Insert test table data for delete all
        _shellClient.Execute(GetDatabaseTest);
        _shellClient.Execute(GetDatabaseTestTableTest);
        _shellClient.Execute(INSERT_TEST_ROW1);
        _shellClient.Execute(INSERT_TEST_ROW2);
        _shellClient.Execute(INSERT_TEST_ROW3);
        _shellClient.Execute(INSERT_TEST_ROW4);
        _shellClient.Execute(INSERT_TEST_ROW5);
        var selectResult = _shellClient.Execute(SELECT_ALL_TABLE) as RowResult;
        Assert.True(selectResult != null && selectResult.FetchAll().Count == TEST_COUNT, DATA_NOT_MATCH);

        // Delete full
        _shellClient.Execute(DELETE_FULL);
        selectResult = _shellClient.Execute(SELECT_ALL_TABLE) as RowResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == 0, DATA_NOT_MATCH);

        // Insert test character rows
        _shellClient.Execute(UseSakilaXDatabase);
        _shellClient.Execute(GetTableSakilaXCharacter);
        _shellClient.Execute(INSERT_NO_COLUMN_SPECIFICATION);
        _shellClient.Execute(INSERT_COLUMNS_AS_LIST);
        _shellClient.Execute(INSERT_COLUMNS_AS_ARRAY1);
        _shellClient.Execute(INSERT_COLUMNS_AS_ARRAY2);
        _shellClient.Execute(INSERT_COLUMNS_AS_ARRAY3);
        _shellClient.Execute(INSERT_COLUMNS_AS_ARRAY4);
        _shellClient.Execute(PYTHON_INSERT_JSON_DOCUMENT1);
        _shellClient.Execute(PYTHON_INSERT_JSON_DOCUMENT2);
        charactersCount += 8;
        selectResult = _shellClient.Execute(SELECT_ALL_TABLE) as RowResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == charactersCount, DATA_NOT_MATCH);

        // Delete simple, using parameter binding
        _shellClient.Execute(DELETE_SIMPLE_WITH_BINDING);
        selectResult = _shellClient.Execute(SELECT_UPDATED_TALI) as RowResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == 0, DATA_NOT_MATCH);

        // Delete with limit
        _shellClient.Execute(DELETE_WITH_LIMIT);
        selectResult = _shellClient.Execute(SELECT_NON_BASE_AGE_GREATER_THAN_30) as RowResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == CHARACTERS_NON_BASE_AGE_GREATER_THAN_30_COUNT - 2, DATA_NOT_MATCH);

        // Delete with limit again
        _shellClient.Execute(DELETE_WITH_LIMIT);
        selectResult = _shellClient.Execute(SELECT_NON_BASE_AGE_GREATER_THAN_30) as RowResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == CHARACTERS_NON_BASE_AGE_GREATER_THAN_30_COUNT - 4, DATA_NOT_MATCH);

        // Delete inserted test rows
        _shellClient.Execute(REVERT_INSERTED_CHARACTERS);
        selectResult = _shellClient.Execute(SELECT_ALL_TABLE) as RowResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == CHARACTERS_FULL_COUNT, DATA_NOT_MATCH);

        // Drop test table
        _shellClient.Execute(UseTestDatabase);
        _shellClient.Execute(DropTestTableIfExists);
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

        _shellClient.Execute(REVERT_INSERTED_CHARACTERS);
        CloseConnection();
        DisposeShellClient();
      }
    }

    /// <summary>
    /// Test to Insert records into a table using the <see cref="ShellClient"/>.
    /// </summary>
    [Fact]
    public void Insert()
    {
      OpenConnection();

      try
      {
        InitXShell();

        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, SAKILA_X_CHARACTER_TABLE, SAKILA_X_SCHEMA_NAME), Connection);
        var result = Command.ExecuteScalar();
        int count;
        int charactersCount = CHARACTERS_FULL_COUNT;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, SAKILA_X_CHARACTER_TABLE));

        // Insert without specifying any columns
        _shellClient.Execute(GetTableSakilaXCharacter);
        _shellClient.Execute(INSERT_NO_COLUMN_SPECIFICATION);
        charactersCount += 2;
        var selectResult = _shellClient.Execute(SELECT_ALL_TABLE) as RowResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == charactersCount, DATA_NOT_MATCH);

        // Insert specifying a comma delimited list of columns
        _shellClient.Execute(INSERT_COLUMNS_AS_LIST);
        charactersCount += 2;
        selectResult = _shellClient.Execute(SELECT_ALL_TABLE) as RowResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == charactersCount, DATA_NOT_MATCH);

        // Insert specifying columns as an array, also in different lines
        _shellClient.Execute(INSERT_COLUMNS_AS_ARRAY1);
        _shellClient.Execute(INSERT_COLUMNS_AS_ARRAY2);
        _shellClient.Execute(INSERT_COLUMNS_AS_ARRAY3);
        _shellClient.Execute(INSERT_COLUMNS_AS_ARRAY4);
        charactersCount += 2;
        selectResult = _shellClient.Execute(SELECT_ALL_TABLE) as RowResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == charactersCount, DATA_NOT_MATCH);

        // Insert JSON documents
        _shellClient.Execute(PYTHON_INSERT_JSON_DOCUMENT1);
        _shellClient.Execute(PYTHON_INSERT_JSON_DOCUMENT2);
        charactersCount += 2;
        selectResult = _shellClient.Execute(SELECT_ALL_TABLE) as RowResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == charactersCount, DATA_NOT_MATCH);

        // Delete inserted rows
        _shellClient.Execute(REVERT_INSERTED_CHARACTERS);
        selectResult = _shellClient.Execute(SELECT_ALL_TABLE) as RowResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == CHARACTERS_FULL_COUNT, DATA_NOT_MATCH);
      }
      finally
      {
        if (Command != null)
        {
          Command.Dispose();
        }

        _shellClient.Execute(REVERT_INSERTED_CHARACTERS);
        CloseConnection();
        DisposeShellClient();
      }
    }

    /// <summary>
    /// Test to Select records in a table using the <see cref="ShellClient"/>.
    /// </summary>
    [Fact]
    public void Select()
    {
      OpenConnection();

      try
      {
        InitXShell();

        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, SAKILA_X_CHARACTER_TABLE, SAKILA_X_SCHEMA_NAME), Connection);
        var result = Command.ExecuteScalar();
        int count;
        int charactersCount = CHARACTERS_FULL_COUNT;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, SAKILA_X_CHARACTER_TABLE));

        // Insert test character rows
        _shellClient.Execute(GetTableSakilaXCharacter);
        _shellClient.Execute(INSERT_NO_COLUMN_SPECIFICATION);
        _shellClient.Execute(INSERT_COLUMNS_AS_LIST);
        _shellClient.Execute(INSERT_COLUMNS_AS_ARRAY1);
        _shellClient.Execute(INSERT_COLUMNS_AS_ARRAY2);
        _shellClient.Execute(INSERT_COLUMNS_AS_ARRAY3);
        _shellClient.Execute(INSERT_COLUMNS_AS_ARRAY4);
        _shellClient.Execute(PYTHON_INSERT_JSON_DOCUMENT1);
        _shellClient.Execute(PYTHON_INSERT_JSON_DOCUMENT2);
        charactersCount += 8;

        // Select all
        var selectResult = _shellClient.Execute(SELECT_ALL_TABLE) as RowResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == charactersCount, DATA_NOT_MATCH);

        // Select female
        selectResult = _shellClient.Execute(SELECT_FEMALE_CHARACTERS) as RowResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == CHARACTERS_FEMALE_COUNT, DATA_NOT_MATCH);

        // Select male
        selectResult = _shellClient.Execute(SELECT_MALE_CHARACTERS) as RowResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == CHARACTERS_MALE_COUNT, DATA_NOT_MATCH);

        // Select with field selection
        selectResult = _shellClient.Execute(SELECT_WITH_FIELD_SELECTION) as RowResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        var allResults = selectResult != null ? selectResult.FetchAll() : null;
        Assert.True(allResults != null && allResults.Count == charactersCount, DATA_NOT_MATCH);
        Assert.True(allResults != null && allResults.Count > 0 && allResults[0].Length == 2, DATA_NOT_MATCH);

        // Select with order by descending
        selectResult = _shellClient.Execute(SELECT_WITH_ORDER_BY_DESC) as RowResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        var singleResult = selectResult != null ? selectResult.FetchOne() : null;
        int fetchedAge = singleResult != null ? Convert.ToInt32(singleResult[2]) : 0;
        Assert.True(fetchedAge == CHARACTERS_HIGHEST_AGE, DATA_NOT_MATCH);
        singleResult = selectResult != null ? selectResult.FetchOne() : null;
        fetchedAge = singleResult != null ? Convert.ToInt32(singleResult[2]) : 0;
        Assert.True(fetchedAge == CHARACTERS_SECOND_HIGHEST_AGE, DATA_NOT_MATCH);

        // Select by paging (limit + offset)
        selectResult = _shellClient.Execute(SELECT_PAGING1) as RowResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == CHARACTERS_PAGE_SIZE, DATA_NOT_MATCH);
        selectResult = _shellClient.Execute(SELECT_PAGING2) as RowResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == charactersCount - CHARACTERS_PAGE_SIZE, DATA_NOT_MATCH);

        // Delete inserted test rows
        _shellClient.Execute(REVERT_INSERTED_CHARACTERS);
        selectResult = _shellClient.Execute(SELECT_ALL_TABLE) as RowResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == CHARACTERS_FULL_COUNT, DATA_NOT_MATCH);
      }
      finally
      {
        if (Command != null)
        {
          Command.Dispose();
        }

        _shellClient.Execute(REVERT_INSERTED_CHARACTERS);
        CloseConnection();
        DisposeShellClient();
      }
    }

    /// <summary>
    /// Test to Update records in a table using the <see cref="ShellClient"/>.
    /// </summary>
    [Fact]
    public void Update()
    {
      OpenConnection();

      try
      {
        InitXShell();

        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, SAKILA_X_CHARACTER_TABLE, SAKILA_X_SCHEMA_NAME), Connection);
        var result = Command.ExecuteScalar();
        int count, foundAge = 0;
        object foundValue1 = null;
        int charactersCount = CHARACTERS_FULL_COUNT;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, SAKILA_X_CHARACTER_TABLE));

        // Insert test rows
        _shellClient.Execute(GetTableSakilaXCharacter);
        _shellClient.Execute(INSERT_NO_COLUMN_SPECIFICATION);
        _shellClient.Execute(INSERT_COLUMNS_AS_LIST);
        _shellClient.Execute(INSERT_COLUMNS_AS_ARRAY1);
        _shellClient.Execute(INSERT_COLUMNS_AS_ARRAY2);
        _shellClient.Execute(INSERT_COLUMNS_AS_ARRAY3);
        _shellClient.Execute(INSERT_COLUMNS_AS_ARRAY4);
        _shellClient.Execute(PYTHON_INSERT_JSON_DOCUMENT1);
        _shellClient.Execute(PYTHON_INSERT_JSON_DOCUMENT2);
        charactersCount += 8;
        var selectResult = _shellClient.Execute(SELECT_ALL_TABLE) as RowResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == charactersCount, DATA_NOT_MATCH);

        // Update simple, 1 record 1 value, using parameter binding
        _shellClient.Execute(UPDATE_SIMPLE);
        selectResult = _shellClient.Execute(SELECT_UPDATED_TALI) as RowResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        var rowResult = selectResult != null ? selectResult.FetchOne() : null;
        if (rowResult != null)
        {
          foundValue1 = rowResult[5];
        }

        Assert.True(foundValue1 != null && foundValue1.ToString().Equals("Mass Effect 3", StringComparison.InvariantCultureIgnoreCase), DATA_NOT_MATCH);

        // Update a singe value with statements in different lines, using parameter binding
        _shellClient.Execute(UPDATE_IN_SEVERAL_LINES1);
        _shellClient.Execute(UPDATE_IN_SEVERAL_LINES2);
        _shellClient.Execute(UPDATE_IN_SEVERAL_LINES3);
        _shellClient.Execute(UPDATE_IN_SEVERAL_LINES4);
        selectResult = _shellClient.Execute(SELECT_UPDATED_TALI) as RowResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        rowResult = selectResult != null ? selectResult.FetchOne() : null;
        if (rowResult != null)
        {
          foundValue1 = rowResult[1];
        }

        Assert.True(foundValue1 != null && foundValue1.ToString().Equals(TALI_MASS_EFFECT_3, StringComparison.InvariantCultureIgnoreCase), DATA_NOT_MATCH);

        // Update using an expression
        _shellClient.Execute(PYTHON_INCLUDE_MYSQLX);
        _shellClient.Execute(UPDATE_WITH_EXPRESSION);
        selectResult = _shellClient.Execute(SELECT_UPDATED_TALI) as RowResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        rowResult = selectResult != null ? selectResult.FetchOne() : null;
        if (rowResult != null)
        {
          foundValue1 = rowResult[2];
          if (foundValue1 != null)
          {
            int.TryParse(foundValue1.ToString(), out foundAge);
          }
        }

        Assert.True(foundAge == 25, DATA_NOT_MATCH);

        // Update with limit
        _shellClient.Execute(UPDATE_WITH_LIMIT);
        selectResult = _shellClient.Execute(SELECT_UPDATED_OLD) as RowResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == 2, DATA_NOT_MATCH);

        // Update with limit
        _shellClient.Execute(UPDATE_FULL);
        selectResult = _shellClient.Execute(SELECT_FROM_VIDEOGAMES) as RowResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == charactersCount, DATA_NOT_MATCH);

        // Delete inserted test rows
        _shellClient.Execute(REVERT_INSERTED_CHARACTERS);
        selectResult = _shellClient.Execute(SELECT_ALL_TABLE) as RowResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == CHARACTERS_FULL_COUNT, DATA_NOT_MATCH);
      }
      finally
      {
        if (Command != null)
        {
          Command.Dispose();
        }

        _shellClient.Execute(REVERT_INSERTED_CHARACTERS);
        CloseConnection();
        DisposeShellClient();
      }
    }

    /// <summary>
    /// Initializes the XShell instance with common statements
    /// </summary>
    private void InitXShell()
    {
      // For now we always create a new instance of the shell client, to avoid the error thrown by Python when handling multiple "sessions"
      // ToDo: Research how to fix the shell to avoid the error thrown by Python when running multiple tests sessions
      _shellClient = new MySqlShellClient(ScriptType.Python);
      _shellClient.MakeConnection(XConnString);
      _shellClient.SwitchMode(Mode.Python);
      _shellClient.AppendAdditionalModulePaths(ScriptType.Python);
    }

    /// <summary>
    /// Dispose the XShell instance
    /// </summary>
    private void DisposeShellClient()
    {
      if (_shellClient != null)
      {
        _shellClient.Dispose();
      }
    }
  }
}
