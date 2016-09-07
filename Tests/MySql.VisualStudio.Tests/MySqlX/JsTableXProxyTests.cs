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
using MySql.Utility.Enums;
using MySql.VisualStudio.Tests.MySqlX.Base;
using Xunit;

namespace MySql.VisualStudio.Tests.MySqlX
{
  public class JsTableXProxyTests : BaseTableTests
  {
    /// <summary>
    /// Test to create a Database using the <see cref="MyTestXProxy"/> direclty.
    /// </summary>
    [Fact]
    public void CreateDatabase()
    {
      OpenConnection();
      MySqlDataReader reader = null;

      try
      {
        InitXProxy(ScriptLanguageType.JavaScript);

        XProxy.ExecuteScript(DropTestDatabaseIfExists, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(CreateTestDatabase, ScriptLanguageType.JavaScript);
        Command = new MySqlCommand(SHOW_DBS_SQL_SYNTAX, Connection);
        reader = Command.ExecuteReader();
        bool success = false;

        while (reader.Read())
        {
          var retDb = reader.GetString(0);
          if (retDb != TEMP_TEST_DATABASE_NAME)
            continue;
          success = true;
          reader.Close();
          break;
        }

        Assert.True(success, string.Format(DB_NOT_FOUND, TEMP_TEST_DATABASE_NAME));

        XProxy.ExecuteScript(DropTestDatabaseIfExists, ScriptLanguageType.JavaScript);
        Command = new MySqlCommand(SHOW_DBS_SQL_SYNTAX, Connection);
        reader = Command.ExecuteReader();
        while (reader.Read())
        {
          var retSchema = reader.GetString(0);
          if (retSchema != TEMP_TEST_DATABASE_NAME)
            continue;
          success = false;
          reader.Close();
          break;
        }

        Assert.True(success, string.Format(DB_NOT_DELETED, TEMP_TEST_DATABASE_NAME));
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
        DisposeProxy();
      }
    }

    /// <summary>
    /// Test to create a Table using the <see cref="MyTestXProxy"/> direclty.
    /// </summary>
    [Fact]
    public void CreateTable()
    {
      OpenConnection();

      try
      {
        InitXProxy(ScriptLanguageType.JavaScript);

        XProxy.ExecuteScript(DropTestDatabaseIfExists, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(CreateTestDatabase, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(UseTestDatabase, ScriptLanguageType.JavaScript);

        XProxy.ExecuteScript(CREATE_TEST_TABLE, ScriptLanguageType.JavaScript);
        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, TEST_TABLE_NAME, TEMP_TEST_DATABASE_NAME), Connection);
        var result = Command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, TEST_TABLE_NAME));

        XProxy.ExecuteScript(DropTestTableIfExists, ScriptLanguageType.JavaScript);
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
        DisposeProxy();
      }
    }

    /// <summary>
    /// Test to Delete records in a table using the <see cref="MyTestXProxy"/>.
    /// </summary>
    [Fact]
    public void Delete()
    {
      OpenConnection();

      try
      {
        InitXProxy(ScriptLanguageType.JavaScript);

        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, CHARACTERS_COLLECTION_NAME, X_TEST_SCHEMA_NAME), Connection);
        var result = Command.ExecuteScalar();
        int count;
        int charactersCount = CHARACTERS_FULL_COUNT;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, CHARACTERS_COLLECTION_NAME));

        // Create test table
        XProxy.ExecuteScript(DropTestDatabaseIfExists, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(CreateTestDatabase, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(UseTestDatabase, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(CREATE_TEST_TABLE, ScriptLanguageType.JavaScript);
        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, TEST_TABLE_NAME, TEMP_TEST_DATABASE_NAME), Connection);
        result = Command.ExecuteScalar();
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, TEST_TABLE_NAME));

        // Insert test table data for delete all
        XProxy.ExecuteScript(GetDatabaseTest, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(GetDatabaseTestTableTest, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(INSERT_TEST_ROW1, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(INSERT_TEST_ROW2, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(INSERT_TEST_ROW3, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(INSERT_TEST_ROW4, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(INSERT_TEST_ROW5, ScriptLanguageType.JavaScript);
        var selectResult = XProxy.ExecuteScript(SELECT_ALL_TABLE, ScriptLanguageType.JavaScript);
        Assert.True(selectResult != null && selectResult.Count == TEST_COUNT, DATA_NOT_MATCH);

        // Delete full
        XProxy.ExecuteScript(DELETE_FULL, ScriptLanguageType.JavaScript);
        selectResult = XProxy.ExecuteScript(SELECT_ALL_TABLE, ScriptLanguageType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 0, DATA_NOT_MATCH);

        // Insert test character rows
        XProxy.ExecuteScript(UseSakilaXDatabase, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(GetTableSakilaXCharacter, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(INSERT_NO_COLUMN_SPECIFICATION, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(INSERT_COLUMNS_AS_LIST, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY1, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY2, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY3, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY4, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(JAVASCRIPT_INSERT_JSON_DOCUMENT1, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(JAVASCRIPT_INSERT_JSON_DOCUMENT2, ScriptLanguageType.JavaScript);
        charactersCount += 8;
        selectResult = XProxy.ExecuteScript(SELECT_ALL_TABLE, ScriptLanguageType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == charactersCount, DATA_NOT_MATCH);

        // Delete simple, using parameter binding
        XProxy.ExecuteScript(DELETE_SIMPLE_WITH_BINDING, ScriptLanguageType.JavaScript);
        selectResult = XProxy.ExecuteScript(SELECT_UPDATED_TALI, ScriptLanguageType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 0, DATA_NOT_MATCH);

        // Delete with limit
        XProxy.ExecuteScript(DELETE_WITH_LIMIT, ScriptLanguageType.JavaScript);
        selectResult = XProxy.ExecuteScript(SELECT_NON_BASE_AGE_GREATER_THAN_30, ScriptLanguageType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == CHARACTERS_NON_BASE_AGE_GREATER_THAN_30_COUNT - 2, DATA_NOT_MATCH);

        // Delete with limit again
        XProxy.ExecuteScript(DELETE_WITH_LIMIT, ScriptLanguageType.JavaScript);
        selectResult = XProxy.ExecuteScript(SELECT_NON_BASE_AGE_GREATER_THAN_30, ScriptLanguageType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == CHARACTERS_NON_BASE_AGE_GREATER_THAN_30_COUNT - 4, DATA_NOT_MATCH);

        // Delete inserted test rows
        XProxy.ExecuteScript(REVERT_INSERTED_CHARACTERS, ScriptLanguageType.JavaScript);
        selectResult = XProxy.ExecuteScript(SELECT_ALL_TABLE, ScriptLanguageType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == CHARACTERS_FULL_COUNT, DATA_NOT_MATCH);

        // Drop test table
        XProxy.ExecuteScript(UseTestDatabase, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(DropTestTableIfExists, ScriptLanguageType.JavaScript);
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

        XProxy.ExecuteScript(REVERT_INSERTED_CHARACTERS, ScriptLanguageType.JavaScript);
        CloseConnection();
        DisposeProxy();
      }
    }

    /// <summary>
    /// Test to Insert records into a table using the <see cref="MyTestXProxy"/>.
    /// </summary>
    [Fact]
    public void Insert()
    {
      OpenConnection();

      try
      {
        InitXProxy(ScriptLanguageType.JavaScript);

        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, CHARACTERS_COLLECTION_NAME, X_TEST_SCHEMA_NAME), Connection);
        var result = Command.ExecuteScalar();
        int count;
        int charactersCount = CHARACTERS_FULL_COUNT;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, CHARACTERS_COLLECTION_NAME));

        // Insert without specifying any columns
        XProxy.ExecuteScript(GetTableSakilaXCharacter, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(INSERT_NO_COLUMN_SPECIFICATION, ScriptLanguageType.JavaScript);
        charactersCount += 2;
        var selectResult = XProxy.ExecuteScript(SELECT_ALL_TABLE, ScriptLanguageType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == charactersCount, DATA_NOT_MATCH);

        // Insert specifying a comma delimited list of columns
        XProxy.ExecuteScript(INSERT_COLUMNS_AS_LIST, ScriptLanguageType.JavaScript);
        charactersCount += 2;
        selectResult = XProxy.ExecuteScript(SELECT_ALL_TABLE, ScriptLanguageType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == charactersCount, DATA_NOT_MATCH);

        // Insert specifying columns as an array, also in different lines
        XProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY1, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY2, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY3, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY4, ScriptLanguageType.JavaScript);
        charactersCount += 2;
        selectResult = XProxy.ExecuteScript(SELECT_ALL_TABLE, ScriptLanguageType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == charactersCount, DATA_NOT_MATCH);

        // Insert JSON documents
        XProxy.ExecuteScript(JAVASCRIPT_INSERT_JSON_DOCUMENT1, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(JAVASCRIPT_INSERT_JSON_DOCUMENT2, ScriptLanguageType.JavaScript);
        charactersCount += 2;
        selectResult = XProxy.ExecuteScript(SELECT_ALL_TABLE, ScriptLanguageType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == charactersCount, DATA_NOT_MATCH);

        // Delete inserted rows
        XProxy.ExecuteScript(REVERT_INSERTED_CHARACTERS, ScriptLanguageType.JavaScript);
        selectResult = XProxy.ExecuteScript(SELECT_ALL_TABLE, ScriptLanguageType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == CHARACTERS_FULL_COUNT, DATA_NOT_MATCH);
      }
      finally
      {
        if (Command != null)
        {
          Command.Dispose();
        }

        XProxy.ExecuteScript(REVERT_INSERTED_CHARACTERS, ScriptLanguageType.JavaScript);
        CloseConnection();
        DisposeProxy();
      }
    }

    /// <summary>
    /// Test to Select records in a table using the <see cref="MyTestXProxy"/>.
    /// </summary>
    [Fact]
    public void Select()
    {
      OpenConnection();

      try
      {
        InitXProxy(ScriptLanguageType.JavaScript);

        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, CHARACTERS_COLLECTION_NAME, X_TEST_SCHEMA_NAME), Connection);
        var result = Command.ExecuteScalar();
        int count;
        int charactersCount = CHARACTERS_FULL_COUNT;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, CHARACTERS_COLLECTION_NAME));

        // Insert test character rows
        XProxy.ExecuteScript(GetTableSakilaXCharacter, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(INSERT_NO_COLUMN_SPECIFICATION, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(INSERT_COLUMNS_AS_LIST, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY1, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY2, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY3, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY4, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(JAVASCRIPT_INSERT_JSON_DOCUMENT1, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(JAVASCRIPT_INSERT_JSON_DOCUMENT2, ScriptLanguageType.JavaScript);
        charactersCount += 8;

        // Select all
        var selectResult = XProxy.ExecuteScript(SELECT_ALL_TABLE, ScriptLanguageType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == charactersCount, DATA_NOT_MATCH);

        // Select female
        selectResult = XProxy.ExecuteScript(SELECT_FEMALE_CHARACTERS, ScriptLanguageType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == CHARACTERS_FEMALE_COUNT, DATA_NOT_MATCH);

        // Select male
        selectResult = XProxy.ExecuteScript(SELECT_MALE_CHARACTERS, ScriptLanguageType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == CHARACTERS_MALE_COUNT, DATA_NOT_MATCH);

        // Select with field selection
        selectResult = XProxy.ExecuteScript(SELECT_WITH_FIELD_SELECTION, ScriptLanguageType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == charactersCount, DATA_NOT_MATCH);
        Assert.True(selectResult != null && selectResult.Count > 0 && selectResult[0].Count == 2, DATA_NOT_MATCH);

        // Select with order by descending
        selectResult = XProxy.ExecuteScript(SELECT_WITH_ORDER_BY_DESC, ScriptLanguageType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        var singleResult = selectResult.FirstOrDefault();
        int fetchedAge = singleResult != null ? Convert.ToInt32(singleResult["age"]) : 0;
        Assert.True(fetchedAge == CHARACTERS_HIGHEST_AGE, DATA_NOT_MATCH);
        singleResult = selectResult.ElementAtOrDefault(1);
        fetchedAge = singleResult != null ? Convert.ToInt32(singleResult["age"]) : 0;
        Assert.True(fetchedAge == CHARACTERS_SECOND_HIGHEST_AGE, DATA_NOT_MATCH);

        // Select by paging (limit + offset)
        selectResult = XProxy.ExecuteScript(SELECT_PAGING1, ScriptLanguageType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == CHARACTERS_PAGE_SIZE, DATA_NOT_MATCH);
        selectResult = XProxy.ExecuteScript(SELECT_PAGING2, ScriptLanguageType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == charactersCount - CHARACTERS_PAGE_SIZE, DATA_NOT_MATCH);

        // Delete inserted test rows
        XProxy.ExecuteScript(REVERT_INSERTED_CHARACTERS, ScriptLanguageType.JavaScript);
        selectResult = XProxy.ExecuteScript(SELECT_ALL_TABLE, ScriptLanguageType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == CHARACTERS_FULL_COUNT, DATA_NOT_MATCH);
      }
      finally
      {
        if (Command != null)
        {
          Command.Dispose();
        }

        XProxy.ExecuteScript(REVERT_INSERTED_CHARACTERS, ScriptLanguageType.JavaScript);
        CloseConnection();
        DisposeProxy();
      }
    }

    /// <summary>
    /// Test to Update records in a table using the <see cref="MyTestXProxy"/>.
    /// </summary>
    [Fact]
    public void Update()
    {
      OpenConnection();

      try
      {
        InitXProxy(ScriptLanguageType.JavaScript);

        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, CHARACTERS_COLLECTION_NAME, X_TEST_SCHEMA_NAME), Connection);
        var result = Command.ExecuteScalar();
        int count;
        int charactersCount = CHARACTERS_FULL_COUNT;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, CHARACTERS_COLLECTION_NAME));

        // Insert test rows
        XProxy.ExecuteScript(GetTableSakilaXCharacter, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(INSERT_NO_COLUMN_SPECIFICATION, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(INSERT_COLUMNS_AS_LIST, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY1, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY2, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY3, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(INSERT_COLUMNS_AS_ARRAY4, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(JAVASCRIPT_INSERT_JSON_DOCUMENT1, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(JAVASCRIPT_INSERT_JSON_DOCUMENT2, ScriptLanguageType.JavaScript);
        charactersCount += 8;
        var selectResult = XProxy.ExecuteScript(SELECT_ALL_TABLE, ScriptLanguageType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == charactersCount, DATA_NOT_MATCH);

        // Update simple, 1 record 1 value, using parameter binding
        XProxy.ExecuteScript(UPDATE_SIMPLE, ScriptLanguageType.JavaScript);
        selectResult = XProxy.ExecuteScript(SELECT_UPDATED_TALI, ScriptLanguageType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        var singleResult = selectResult.FirstOrDefault();
        Assert.True(singleResult != null && singleResult["universe"].ToString().Equals("Mass Effect 3", StringComparison.InvariantCultureIgnoreCase), DATA_NOT_MATCH);

        // Update a singe value with statements in different lines, using parameter binding
        XProxy.ExecuteScript(UPDATE_IN_SEVERAL_LINES1, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(UPDATE_IN_SEVERAL_LINES2, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(UPDATE_IN_SEVERAL_LINES3, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(UPDATE_IN_SEVERAL_LINES4, ScriptLanguageType.JavaScript);
        selectResult = XProxy.ExecuteScript(SELECT_UPDATED_TALI, ScriptLanguageType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        singleResult = selectResult.FirstOrDefault();
        Assert.True(singleResult != null && singleResult["name"].ToString().Equals(TALI_MASS_EFFECT_3, StringComparison.InvariantCultureIgnoreCase), DATA_NOT_MATCH);

        // Update using an expression
        XProxy.ExecuteScript(JAVASCRIPT_INCLUDE_MYSQLX, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(UPDATE_WITH_EXPRESSION, ScriptLanguageType.JavaScript);
        selectResult = XProxy.ExecuteScript(SELECT_UPDATED_TALI, ScriptLanguageType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        singleResult = selectResult.FirstOrDefault();
        Assert.True(singleResult != null && singleResult["age"].ToString().Equals("25", StringComparison.InvariantCultureIgnoreCase), DATA_NOT_MATCH);

        // Update with limit
        XProxy.ExecuteScript(UPDATE_WITH_LIMIT, ScriptLanguageType.JavaScript);
        selectResult = XProxy.ExecuteScript(SELECT_UPDATED_OLD, ScriptLanguageType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 2, DATA_NOT_MATCH);

        // Update with limit
        XProxy.ExecuteScript(UPDATE_FULL, ScriptLanguageType.JavaScript);
        selectResult = XProxy.ExecuteScript(SELECT_FROM_VIDEOGAMES, ScriptLanguageType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == charactersCount, DATA_NOT_MATCH);

        // Delete inserted test rows
        XProxy.ExecuteScript(REVERT_INSERTED_CHARACTERS, ScriptLanguageType.JavaScript);
        selectResult = XProxy.ExecuteScript(SELECT_ALL_TABLE, ScriptLanguageType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == CHARACTERS_FULL_COUNT, DATA_NOT_MATCH);
      }
      finally
      {
        if (Command != null)
        {
          Command.Dispose();
        }

        XProxy.ExecuteScript(REVERT_INSERTED_CHARACTERS, ScriptLanguageType.JavaScript);
        CloseConnection();
        DisposeProxy();
      }
    }
  }
}
