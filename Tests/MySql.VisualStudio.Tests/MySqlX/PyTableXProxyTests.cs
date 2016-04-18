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
using System.Linq;
using MySql.Data.MySqlClient;
using MySql.Data.VisualStudio.Editors;
using MySql.Data.VisualStudio.MySqlX;
using MySql.VisualStudio.Tests.MySqlX.Base;
using Xunit;

namespace MySql.VisualStudio.Tests.MySqlX
{
  /// <summary>
  /// Class to test the CRUD operations through the NgShell Wrapper on Relational DB
  /// </summary>
  public class PyTableXProxyTests : BaseTableTests, IUseFixture<SetUpXShell>
  {
    #region Fields

    /// <summary>
    /// Object to access and execute commands to the current database connection through the mysqlx protocol
    /// </summary>
    private MySqlXProxy _xProxy;

    #endregion Fields

    /// <summary>
    /// Test to create a Database using the NgWrapper
    /// </summary>
    [Fact]
    public void AllTests()
    {
      OpenConnection();

      try
      {
        InitXProxy();

        #region Delete

        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, SAKILA_X_CHARACTER_TABLE, SAKILA_X_SCHEMA_NAME), Connection);
        var result = Command.ExecuteScalar();
        int count;
        int charactersCount = CHARACTERS_FULL_COUNT;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, SAKILA_X_CHARACTER_TABLE));

        // Create test table
        _xProxy.ExecuteScript(DropTestDatabaseIfExists, ScriptType.Python);
        _xProxy.ExecuteScript(CreateTestDatabase, ScriptType.Python);
        _xProxy.ExecuteScript(UseTestDatabase, ScriptType.Python);
        _xProxy.ExecuteScript(CREATE_TEST_TABLE, ScriptType.Python);
        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, TEST_TABLE_NAME, TEST_DATABASE_NAME), Connection);
        result = Command.ExecuteScalar();
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, TEST_TABLE_NAME));

        // Insert test table data for delete all
        _xProxy.ExecuteScript(GetDatabaseTest, ScriptType.Python);
        _xProxy.ExecuteScript(GetDatabaseTestTableTest, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_TEST_ROW1, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_TEST_ROW2, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_TEST_ROW3, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_TEST_ROW4, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_TEST_ROW5, ScriptType.Python);
        var selectResult = _xProxy.ExecuteScript(SELECT_ALL_TABLE, ScriptType.Python);
        Assert.True(selectResult != null && selectResult.Count == TEST_COUNT, DATA_NOT_MATCH);

        // Delete full
        _xProxy.ExecuteScript(DELETE_FULL, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(SELECT_ALL_TABLE, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 0, DATA_NOT_MATCH);

        // Insert test character rows
        _xProxy.ExecuteScript(UseSakilaXDatabase, ScriptType.Python);
        _xProxy.ExecuteScript(GetTableSakilaXCharacter, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_NO_COLUMN_SPECIFICATION, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_COLUMNS_AS_LIST, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY1, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY2, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY3, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY4, ScriptType.Python);
        _xProxy.ExecuteScript(PYTHON_INSERT_JSON_DOCUMENT1, ScriptType.Python);
        _xProxy.ExecuteScript(PYTHON_INSERT_JSON_DOCUMENT2, ScriptType.Python);
        charactersCount += 8;
        selectResult = _xProxy.ExecuteScript(SELECT_ALL_TABLE, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == charactersCount, DATA_NOT_MATCH);

        // Delete simple, using parameter binding
        _xProxy.ExecuteScript(DELETE_SIMPLE_WITH_BINDING, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(SELECT_UPDATED_TALI, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 0, DATA_NOT_MATCH);

        // Delete with limit
        _xProxy.ExecuteScript(DELETE_WITH_LIMIT, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(SELECT_NON_BASE_AGE_GREATER_THAN_30, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == CHARACTERS_NON_BASE_AGE_GREATER_THAN_30_COUNT - 2, DATA_NOT_MATCH);

        // Delete with limit again
        _xProxy.ExecuteScript(DELETE_WITH_LIMIT, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(SELECT_NON_BASE_AGE_GREATER_THAN_30, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == CHARACTERS_NON_BASE_AGE_GREATER_THAN_30_COUNT - 4, DATA_NOT_MATCH);

        // Delete inserted test rows
        _xProxy.ExecuteScript(REVERT_INSERTED_CHARACTERS, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(SELECT_ALL_TABLE, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == CHARACTERS_FULL_COUNT, DATA_NOT_MATCH);

        // Drop test table
        _xProxy.ExecuteScript(UseTestDatabase, ScriptType.Python);
        _xProxy.ExecuteScript(DropTestTableIfExists, ScriptType.Python);
        result = Command.ExecuteScalar();
        int.TryParse(result.ToString(), out count);
        Assert.True(count == 0, string.Format(TABLE_NOT_DELETED, TEST_TABLE_NAME));

        // Drop test database
        _xProxy.ExecuteScript(DropTestDatabaseIfExists, ScriptType.Python);
        Command = new MySqlCommand(SHOW_DBS_SQL_SYNTAX, Connection);
        var reader = Command.ExecuteReader();
        bool success = true;
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
        if (!reader.IsClosed)
        {
          reader.Close();
        }

        reader.Dispose();

        #endregion Delete
        #region Insert

        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, SAKILA_X_CHARACTER_TABLE, SAKILA_X_SCHEMA_NAME), Connection);
        result = Command.ExecuteScalar();
        charactersCount = CHARACTERS_FULL_COUNT;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, SAKILA_X_CHARACTER_TABLE));

        // Insert without specifying any columns
        _xProxy.ExecuteScript(GetTableSakilaXCharacter, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_NO_COLUMN_SPECIFICATION, ScriptType.Python);
        charactersCount += 2;
        selectResult = _xProxy.ExecuteScript(SELECT_ALL_TABLE, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == charactersCount, DATA_NOT_MATCH);

        // Insert specifying a comma delimited list of columns
        _xProxy.ExecuteScript(INSERT_COLUMNS_AS_LIST, ScriptType.Python);
        charactersCount += 2;
        selectResult = _xProxy.ExecuteScript(SELECT_ALL_TABLE, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == charactersCount, DATA_NOT_MATCH);

        // Insert specifying columns as an array, also in different lines
        _xProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY1, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY2, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY3, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY4, ScriptType.Python);
        charactersCount += 2;
        selectResult = _xProxy.ExecuteScript(SELECT_ALL_TABLE, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == charactersCount, DATA_NOT_MATCH);

        // Insert JSON documents
        _xProxy.ExecuteScript(PYTHON_INSERT_JSON_DOCUMENT1, ScriptType.Python);
        _xProxy.ExecuteScript(PYTHON_INSERT_JSON_DOCUMENT2, ScriptType.Python);
        charactersCount += 2;
        selectResult = _xProxy.ExecuteScript(SELECT_ALL_TABLE, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == charactersCount, DATA_NOT_MATCH);

        // Delete inserted rows
        _xProxy.ExecuteScript(REVERT_INSERTED_CHARACTERS, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(SELECT_ALL_TABLE, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == CHARACTERS_FULL_COUNT, DATA_NOT_MATCH);

        #endregion Insert
        #region Select

        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, SAKILA_X_CHARACTER_TABLE, SAKILA_X_SCHEMA_NAME), Connection);
        result = Command.ExecuteScalar();
        charactersCount = CHARACTERS_FULL_COUNT;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, SAKILA_X_CHARACTER_TABLE));

        // Insert test character rows
        _xProxy.ExecuteScript(GetTableSakilaXCharacter, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_NO_COLUMN_SPECIFICATION, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_COLUMNS_AS_LIST, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY1, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY2, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY3, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY4, ScriptType.Python);
        _xProxy.ExecuteScript(PYTHON_INSERT_JSON_DOCUMENT1, ScriptType.Python);
        _xProxy.ExecuteScript(PYTHON_INSERT_JSON_DOCUMENT2, ScriptType.Python);
        charactersCount += 8;

        // Select all
        selectResult = _xProxy.ExecuteScript(SELECT_ALL_TABLE, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == charactersCount, DATA_NOT_MATCH);

        // Select female
        selectResult = _xProxy.ExecuteScript(SELECT_FEMALE_CHARACTERS, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == CHARACTERS_FEMALE_COUNT, DATA_NOT_MATCH);

        // Select male
        selectResult = _xProxy.ExecuteScript(SELECT_MALE_CHARACTERS, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == CHARACTERS_MALE_COUNT, DATA_NOT_MATCH);

        // Select with field selection
        selectResult = _xProxy.ExecuteScript(SELECT_WITH_FIELD_SELECTION, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == charactersCount, DATA_NOT_MATCH);
        Assert.True(selectResult != null && selectResult.Count > 0 && selectResult[0].Count == 2, DATA_NOT_MATCH);

        // Select with order by descending
        selectResult = _xProxy.ExecuteScript(SELECT_WITH_ORDER_BY_DESC, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        var singleResult = selectResult?.FirstOrDefault();
        int fetchedAge = singleResult != null ? Convert.ToInt32(singleResult["age"]) : 0;
        Assert.True(fetchedAge == CHARACTERS_HIGHEST_AGE, DATA_NOT_MATCH);
        singleResult = selectResult?.ElementAtOrDefault(1);
        fetchedAge = singleResult != null ? Convert.ToInt32(singleResult["age"]) : 0;
        Assert.True(fetchedAge == CHARACTERS_SECOND_HIGHEST_AGE, DATA_NOT_MATCH);

        // Select by paging (limit + offset)
        selectResult = _xProxy.ExecuteScript(SELECT_PAGING1, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == CHARACTERS_PAGE_SIZE, DATA_NOT_MATCH);
        selectResult = _xProxy.ExecuteScript(SELECT_PAGING2, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == charactersCount - CHARACTERS_PAGE_SIZE, DATA_NOT_MATCH);

        // Delete inserted test rows
        _xProxy.ExecuteScript(REVERT_INSERTED_CHARACTERS, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(SELECT_ALL_TABLE, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == CHARACTERS_FULL_COUNT, DATA_NOT_MATCH);

        #endregion Select
        #region Update

        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, SAKILA_X_CHARACTER_TABLE, SAKILA_X_SCHEMA_NAME), Connection);
        result = Command.ExecuteScalar();
        charactersCount = CHARACTERS_FULL_COUNT;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, SAKILA_X_CHARACTER_TABLE));

        // Insert test rows
        //_xProxy.ExecuteScript(PYTHON_INCLUDE_MYSQLX, ScriptType.Python);
        _xProxy.ExecuteScript(GetTableSakilaXCharacter, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_NO_COLUMN_SPECIFICATION, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_COLUMNS_AS_LIST, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY1, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY2, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY3, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY4, ScriptType.Python);
        _xProxy.ExecuteScript(PYTHON_INSERT_JSON_DOCUMENT1, ScriptType.Python);
        _xProxy.ExecuteScript(PYTHON_INSERT_JSON_DOCUMENT2, ScriptType.Python);
        charactersCount += 8;
        selectResult = _xProxy.ExecuteScript(SELECT_ALL_TABLE, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == charactersCount, DATA_NOT_MATCH);

        // Update simple, 1 record 1 value, using parameter binding
        _xProxy.ExecuteScript(UPDATE_SIMPLE, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(SELECT_UPDATED_TALI, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        singleResult = selectResult?.FirstOrDefault();
        Assert.True(singleResult != null && singleResult["universe"].ToString().Equals("Mass Effect 3", StringComparison.InvariantCultureIgnoreCase), DATA_NOT_MATCH);

        // Update a singe value with statements in different lines, using parameter binding
        _xProxy.ExecuteScript(UPDATE_IN_SEVERAL_LINES1, ScriptType.Python);
        _xProxy.ExecuteScript(UPDATE_IN_SEVERAL_LINES2, ScriptType.Python);
        _xProxy.ExecuteScript(UPDATE_IN_SEVERAL_LINES3, ScriptType.Python);
        _xProxy.ExecuteScript(UPDATE_IN_SEVERAL_LINES4, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(SELECT_UPDATED_TALI, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        singleResult = selectResult?.FirstOrDefault();
        Assert.True(singleResult != null && singleResult["name"].ToString().Equals(TALI_MASS_EFFECT_3, StringComparison.InvariantCultureIgnoreCase), DATA_NOT_MATCH);

        // Update using an expression
        _xProxy.ExecuteScript(PYTHON_INCLUDE_MYSQLX, ScriptType.Python);
        _xProxy.ExecuteScript(UPDATE_WITH_EXPRESSION, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(SELECT_UPDATED_TALI, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        singleResult = selectResult?.FirstOrDefault();
        Assert.True(singleResult != null && singleResult["age"].ToString().Equals("25", StringComparison.InvariantCultureIgnoreCase), DATA_NOT_MATCH);

        // Update with limit
        _xProxy.ExecuteScript(UPDATE_WITH_LIMIT, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(SELECT_UPDATED_OLD, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 2, DATA_NOT_MATCH);

        // Update with limit
        _xProxy.ExecuteScript(UPDATE_FULL, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(SELECT_FROM_VIDEOGAMES, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == charactersCount, DATA_NOT_MATCH);

        // Delete inserted test rows
        _xProxy.ExecuteScript(REVERT_INSERTED_CHARACTERS, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(SELECT_ALL_TABLE, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == CHARACTERS_FULL_COUNT, DATA_NOT_MATCH);

        #endregion Update
      }
      finally
      {
        Command?.Dispose();
        CloseConnection();
      }
    }

    /// <summary>
    /// Test to create a Database using the <see cref="MyTestXProxy"/> direclty.
    /// </summary>
    //[Fact]
    public void CreateDatabase()
    {
      OpenConnection();
      MySqlDataReader reader = null;

      try
      {
        InitXProxy();

        _xProxy.ExecuteScript(DropTestDatabaseIfExists, ScriptType.Python);
        _xProxy.ExecuteScript(CreateTestDatabase, ScriptType.Python);
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

        _xProxy.ExecuteScript(DropTestDatabaseIfExists, ScriptType.Python);
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

        Command?.Dispose();
        CloseConnection();
      }
    }

    /// <summary>
    /// Test to create a Table using the <see cref="MyTestXProxy"/> direclty.
    /// </summary>
    //[Fact]
    public void CreateTable()
    {
      OpenConnection();

      try
      {
        InitXProxy();

        _xProxy.ExecuteScript(DropTestDatabaseIfExists, ScriptType.Python);
        _xProxy.ExecuteScript(CreateTestDatabase, ScriptType.Python);
        _xProxy.ExecuteScript(UseTestDatabase, ScriptType.Python);

        _xProxy.ExecuteScript(CREATE_TEST_TABLE, ScriptType.Python);
        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, TEST_TABLE_NAME, TEST_DATABASE_NAME), Connection);
        var result = Command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, TEST_TABLE_NAME));

        _xProxy.ExecuteScript(DropTestTableIfExists, ScriptType.Python);
        result = Command.ExecuteScalar();
        int.TryParse(result.ToString(), out count);
        Assert.True(count == 0, string.Format(TABLE_NOT_DELETED, TEST_TABLE_NAME));
      }
      finally
      {
        Command?.Dispose();
        CloseConnection();
      }
    }

    /// <summary>
    /// Test to Delete records in a table using the <see cref="MyTestXProxy"/>.
    /// </summary>
    //[Fact]
    public void Delete()
    {
      OpenConnection();

      try
      {
        InitXProxy();

        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, SAKILA_X_CHARACTER_TABLE, SAKILA_X_SCHEMA_NAME), Connection);
        var result = Command.ExecuteScalar();
        int count;
        int charactersCount = CHARACTERS_FULL_COUNT;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, SAKILA_X_CHARACTER_TABLE));

        // Create test table
        _xProxy.ExecuteScript(DropTestDatabaseIfExists, ScriptType.Python);
        _xProxy.ExecuteScript(CreateTestDatabase, ScriptType.Python);
        _xProxy.ExecuteScript(UseTestDatabase, ScriptType.Python);
        _xProxy.ExecuteScript(CREATE_TEST_TABLE, ScriptType.Python);
        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, TEST_TABLE_NAME, TEST_DATABASE_NAME), Connection);
        result = Command.ExecuteScalar();
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, TEST_TABLE_NAME));

        // Insert test table data for delete all
        _xProxy.ExecuteScript(GetDatabaseTest, ScriptType.Python);
        _xProxy.ExecuteScript(GetDatabaseTestTableTest, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_TEST_ROW1, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_TEST_ROW2, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_TEST_ROW3, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_TEST_ROW4, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_TEST_ROW5, ScriptType.Python);
        var selectResult = _xProxy.ExecuteScript(SELECT_ALL_TABLE, ScriptType.Python);
        Assert.True(selectResult != null && selectResult.Count == TEST_COUNT, DATA_NOT_MATCH);

        // Delete full
        _xProxy.ExecuteScript(DELETE_FULL, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(SELECT_ALL_TABLE, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 0, DATA_NOT_MATCH);

        // Insert test character rows
        _xProxy.ExecuteScript(UseSakilaXDatabase, ScriptType.Python);
        _xProxy.ExecuteScript(GetTableSakilaXCharacter, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_NO_COLUMN_SPECIFICATION, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_COLUMNS_AS_LIST, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY1, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY2, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY3, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY4, ScriptType.Python);
        _xProxy.ExecuteScript(PYTHON_INSERT_JSON_DOCUMENT1, ScriptType.Python);
        _xProxy.ExecuteScript(PYTHON_INSERT_JSON_DOCUMENT2, ScriptType.Python);
        charactersCount += 8;
        selectResult = _xProxy.ExecuteScript(SELECT_ALL_TABLE, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == charactersCount, DATA_NOT_MATCH);

        // Delete simple, using parameter binding
        _xProxy.ExecuteScript(DELETE_SIMPLE_WITH_BINDING, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(SELECT_UPDATED_TALI, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 0, DATA_NOT_MATCH);

        // Delete with limit
        _xProxy.ExecuteScript(DELETE_WITH_LIMIT, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(SELECT_NON_BASE_AGE_GREATER_THAN_30, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == CHARACTERS_NON_BASE_AGE_GREATER_THAN_30_COUNT - 2, DATA_NOT_MATCH);

        // Delete with limit again
        _xProxy.ExecuteScript(DELETE_WITH_LIMIT, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(SELECT_NON_BASE_AGE_GREATER_THAN_30, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == CHARACTERS_NON_BASE_AGE_GREATER_THAN_30_COUNT - 4, DATA_NOT_MATCH);

        // Delete inserted test rows
        _xProxy.ExecuteScript(REVERT_INSERTED_CHARACTERS, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(SELECT_ALL_TABLE, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == CHARACTERS_FULL_COUNT, DATA_NOT_MATCH);

        // Drop test table
        _xProxy.ExecuteScript(UseTestDatabase, ScriptType.Python);
        _xProxy.ExecuteScript(DropTestTableIfExists, ScriptType.Python);
        result = Command.ExecuteScalar();
        int.TryParse(result.ToString(), out count);
        Assert.True(count == 0, string.Format(TABLE_NOT_DELETED, TEST_TABLE_NAME));
      }
      finally
      {
        Command?.Dispose();
        _xProxy.ExecuteScript(REVERT_INSERTED_CHARACTERS, ScriptType.Python);
        CloseConnection();
      }
    }

    /// <summary>
    /// Test to Insert records into a table using the <see cref="MyTestXProxy"/>.
    /// </summary>
    //[Fact]
    public void Insert()
    {
      OpenConnection();

      try
      {
        InitXProxy();

        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, SAKILA_X_CHARACTER_TABLE, SAKILA_X_SCHEMA_NAME), Connection);
        var result = Command.ExecuteScalar();
        int count;
        int charactersCount = CHARACTERS_FULL_COUNT;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, SAKILA_X_CHARACTER_TABLE));

        // Insert without specifying any columns
        _xProxy.ExecuteScript(GetTableSakilaXCharacter, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_NO_COLUMN_SPECIFICATION, ScriptType.Python);
        charactersCount += 2;
        var selectResult = _xProxy.ExecuteScript(SELECT_ALL_TABLE, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == charactersCount, DATA_NOT_MATCH);

        // Insert specifying a comma delimited list of columns
        _xProxy.ExecuteScript(INSERT_COLUMNS_AS_LIST, ScriptType.Python);
        charactersCount += 2;
        selectResult = _xProxy.ExecuteScript(SELECT_ALL_TABLE, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == charactersCount, DATA_NOT_MATCH);

        // Insert specifying columns as an array, also in different lines
        _xProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY1, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY2, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY3, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY4, ScriptType.Python);
        charactersCount += 2;
        selectResult = _xProxy.ExecuteScript(SELECT_ALL_TABLE, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == charactersCount, DATA_NOT_MATCH);

        // Insert JSON documents
        _xProxy.ExecuteScript(PYTHON_INSERT_JSON_DOCUMENT1, ScriptType.Python);
        _xProxy.ExecuteScript(PYTHON_INSERT_JSON_DOCUMENT2, ScriptType.Python);
        charactersCount += 2;
        selectResult = _xProxy.ExecuteScript(SELECT_ALL_TABLE, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == charactersCount, DATA_NOT_MATCH);

        // Delete inserted rows
        _xProxy.ExecuteScript(REVERT_INSERTED_CHARACTERS, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(SELECT_ALL_TABLE, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == CHARACTERS_FULL_COUNT, DATA_NOT_MATCH);
      }
      finally
      {
        Command?.Dispose();
        _xProxy.ExecuteScript(REVERT_INSERTED_CHARACTERS, ScriptType.Python);
        CloseConnection();
      }
    }

    /// <summary>
    /// Test to Select records in a table using the <see cref="MyTestXProxy"/>.
    /// </summary>
    //[Fact]
    public void Select()
    {
      OpenConnection();

      try
      {
        InitXProxy();

        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, SAKILA_X_CHARACTER_TABLE, SAKILA_X_SCHEMA_NAME), Connection);
        var result = Command.ExecuteScalar();
        int count;
        int charactersCount = CHARACTERS_FULL_COUNT;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, SAKILA_X_CHARACTER_TABLE));

        // Insert test character rows
        _xProxy.ExecuteScript(GetTableSakilaXCharacter, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_NO_COLUMN_SPECIFICATION, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_COLUMNS_AS_LIST, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY1, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY2, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY3, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY4, ScriptType.Python);
        _xProxy.ExecuteScript(PYTHON_INSERT_JSON_DOCUMENT1, ScriptType.Python);
        _xProxy.ExecuteScript(PYTHON_INSERT_JSON_DOCUMENT2, ScriptType.Python);
        charactersCount += 8;

        // Select all
        var selectResult = _xProxy.ExecuteScript(SELECT_ALL_TABLE, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == charactersCount, DATA_NOT_MATCH);

        // Select female
        selectResult = _xProxy.ExecuteScript(SELECT_FEMALE_CHARACTERS, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == CHARACTERS_FEMALE_COUNT, DATA_NOT_MATCH);

        // Select male
        selectResult = _xProxy.ExecuteScript(SELECT_MALE_CHARACTERS, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == CHARACTERS_MALE_COUNT, DATA_NOT_MATCH);

        // Select with field selection
        selectResult = _xProxy.ExecuteScript(SELECT_WITH_FIELD_SELECTION, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == charactersCount, DATA_NOT_MATCH);
        Assert.True(selectResult != null && selectResult.Count > 0 && selectResult[0].Count == 2, DATA_NOT_MATCH);

        // Select with order by descending
        selectResult = _xProxy.ExecuteScript(SELECT_WITH_ORDER_BY_DESC, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        var singleResult = selectResult?.FirstOrDefault();
        int fetchedAge = singleResult != null ? Convert.ToInt32(singleResult["age"]) : 0;
        Assert.True(fetchedAge == CHARACTERS_HIGHEST_AGE, DATA_NOT_MATCH);
        singleResult = selectResult?.ElementAtOrDefault(1);
        fetchedAge = singleResult != null ? Convert.ToInt32(singleResult["age"]) : 0;
        Assert.True(fetchedAge == CHARACTERS_SECOND_HIGHEST_AGE, DATA_NOT_MATCH);

        // Select by paging (limit + offset)
        selectResult = _xProxy.ExecuteScript(SELECT_PAGING1, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == CHARACTERS_PAGE_SIZE, DATA_NOT_MATCH);
        selectResult = _xProxy.ExecuteScript(SELECT_PAGING2, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == charactersCount - CHARACTERS_PAGE_SIZE, DATA_NOT_MATCH);

        // Delete inserted test rows
        _xProxy.ExecuteScript(REVERT_INSERTED_CHARACTERS, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(SELECT_ALL_TABLE, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == CHARACTERS_FULL_COUNT, DATA_NOT_MATCH);
      }
      finally
      {
        Command?.Dispose();
        _xProxy.ExecuteScript(REVERT_INSERTED_CHARACTERS, ScriptType.Python);
        CloseConnection();
      }
    }

    /// <summary>
    /// Test to Update records in a table using the <see cref="MyTestXProxy"/>.
    /// </summary>
    //[Fact]
    public void Update()
    {
      OpenConnection();

      try
      {
        InitXProxy();

        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, SAKILA_X_CHARACTER_TABLE, SAKILA_X_SCHEMA_NAME), Connection);
        var result = Command.ExecuteScalar();
        int count;
        int charactersCount = CHARACTERS_FULL_COUNT;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, SAKILA_X_CHARACTER_TABLE));

        // Insert test rows
        _xProxy.ExecuteScript(GetTableSakilaXCharacter, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_NO_COLUMN_SPECIFICATION, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_COLUMNS_AS_LIST, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY1, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY2, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY3, ScriptType.Python);
        _xProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY4, ScriptType.Python);
        _xProxy.ExecuteScript(PYTHON_INSERT_JSON_DOCUMENT1, ScriptType.Python);
        _xProxy.ExecuteScript(PYTHON_INSERT_JSON_DOCUMENT2, ScriptType.Python);
        charactersCount += 8;
        var selectResult = _xProxy.ExecuteScript(SELECT_ALL_TABLE, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == charactersCount, DATA_NOT_MATCH);

        // Update simple, 1 record 1 value, using parameter binding
        _xProxy.ExecuteScript(UPDATE_SIMPLE, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(SELECT_UPDATED_TALI, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        var singleResult = selectResult?.FirstOrDefault();
        Assert.True(singleResult != null && singleResult["universe"].ToString().Equals("Mass Effect 3", StringComparison.InvariantCultureIgnoreCase), DATA_NOT_MATCH);

        // Update a singe value with statements in different lines, using parameter binding
        _xProxy.ExecuteScript(UPDATE_IN_SEVERAL_LINES1, ScriptType.Python);
        _xProxy.ExecuteScript(UPDATE_IN_SEVERAL_LINES2, ScriptType.Python);
        _xProxy.ExecuteScript(UPDATE_IN_SEVERAL_LINES3, ScriptType.Python);
        _xProxy.ExecuteScript(UPDATE_IN_SEVERAL_LINES4, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(SELECT_UPDATED_TALI, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        singleResult = selectResult?.FirstOrDefault();
        Assert.True(singleResult != null && singleResult["name"].ToString().Equals(TALI_MASS_EFFECT_3, StringComparison.InvariantCultureIgnoreCase), DATA_NOT_MATCH);

        // Update using an expression
        _xProxy.ExecuteScript(PYTHON_INCLUDE_MYSQLX, ScriptType.Python);
        _xProxy.ExecuteScript(UPDATE_WITH_EXPRESSION, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(SELECT_UPDATED_TALI, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        singleResult = selectResult?.FirstOrDefault();
        Assert.True(singleResult != null && singleResult["age"].ToString().Equals("25", StringComparison.InvariantCultureIgnoreCase), DATA_NOT_MATCH);

        // Update with limit
        _xProxy.ExecuteScript(UPDATE_WITH_LIMIT, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(SELECT_UPDATED_OLD, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 2, DATA_NOT_MATCH);

        // Update with limit
        _xProxy.ExecuteScript(UPDATE_FULL, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(SELECT_FROM_VIDEOGAMES, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == charactersCount, DATA_NOT_MATCH);

        // Delete inserted test rows
        _xProxy.ExecuteScript(REVERT_INSERTED_CHARACTERS, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(SELECT_ALL_TABLE, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == CHARACTERS_FULL_COUNT, DATA_NOT_MATCH);
      }
      finally
      {
        Command?.Dispose();
        _xProxy.ExecuteScript(REVERT_INSERTED_CHARACTERS, ScriptType.Python);
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
