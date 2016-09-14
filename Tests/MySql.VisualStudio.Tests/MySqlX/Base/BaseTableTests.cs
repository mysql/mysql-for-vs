// Copyright © 2016, Oracle and/or its affiliates. All rights reserved.
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
using Xunit;

namespace MySql.VisualStudio.Tests.MySqlX.Base
{
  public abstract class BaseTableTests : BaseTests
  {
    #region Constant Values

    /// <summary>
    /// The female characters count.
    /// </summary>
    protected const int CHARACTERS_FEMALE_COUNT = 15;

    /// <summary>
    /// The total characters count.
    /// </summary>
    protected const int CHARACTERS_FULL_COUNT = 27;

    /// <summary>
    /// The highest age among all characters.
    /// </summary>
    protected const int CHARACTERS_HIGHEST_AGE = 1505;

    /// <summary>
    /// The male characters count.
    /// </summary>
    protected const int CHARACTERS_MALE_COUNT = 20;

    /// <summary>
    /// The number of non-base characters with an age > 30.
    /// </summary>
    protected const int CHARACTERS_NON_BASE_AGE_GREATER_THAN_30_COUNT = 5;

    /// <summary>
    /// The page size for selecting characters.
    /// </summary>
    protected const int CHARACTERS_PAGE_SIZE = 20;

    /// <summary>
    /// The second highest age among all characters.
    /// </summary>
    protected const int CHARACTERS_SECOND_HIGHEST_AGE = 850;

    /// <summary>
    /// The name of one of the views in schema x_test.
    /// </summary>
    protected const string HALO_CHARACTERS_VIEW_NAME = "halo_characters";

    /// <summary>
    /// The total count in one of the views in schema x_test.
    /// </summary>
    protected const int HALO_CHARACTERS_VIEW_ROWS_COUNT = 17;

    /// <summary>
    /// The name of one of the views in schema x_test.
    /// </summary>
    protected const string MASS_EFFECT_CHARACTERS_VIEW_NAME = "mass_effect_characters";

    /// <summary>
    /// The total count in one of the views in schema x_test.
    /// </summary>
    protected const int MASS_EFFECT_CHARACTERS_VIEW_ROWS_COUNT = 10;

    /// <summary>
    /// The name of one of the views in schema x_test.
    /// </summary>
    protected const string SPARTAN_CHARACTERS_VIEW_NAME = "spartan_characters";

    /// <summary>
    /// The total count in one of the views in schema x_test.
    /// </summary>
    protected const int SPARTAN_CHARACTERS_VIEW_ROWS_COUNT = 9;

    /// <summary>
    /// Tali's name on Mass Effect.
    /// </summary>
    protected const string TALI_MASS_EFFECT = "Tali'Zorah nar Rayya";

    /// <summary>
    /// Tali's name on Mass Effect 3.
    /// </summary>
    protected const string TALI_MASS_EFFECT_3 = "Tali'Zorah vas Normandy";

    /// <summary>
    /// The total table_test count.
    /// </summary>
    protected const int TEST_COUNT = 5;

    /// <summary>
    /// Table test name
    /// </summary>
    public const string TEST_TABLE_NAME = "test_table";

    /// <summary>
    /// The number of tables in schema x_test.
    /// </summary>
    protected const int XTEST_TABLES_COUNT = 4;

    #endregion Constant Values

    #region Common SQL Queries

    /// <summary>
    /// Statement to create a database
    /// </summary>
    public const string CREATE_DATABASE = "session.sql('CREATE SCHEMA `{0}`;').execute()";

    /// <summary>
    /// Statement to create the test table
    /// </summary>
    protected const string CREATE_TEST_TABLE = "session.sql('CREATE TABLE " + TEST_TABLE_NAME + " (name VARCHAR(50), age INTEGER, gender VARCHAR(20));').execute()";

    /// <summary>
    /// Statement to drop a table if it already exists.
    /// </summary>
    public const string DROP_TABLE_IF_EXISTS = "session.sql('DROP TABLE IF EXISTS `{0}`;').execute()";

    /// <summary>
    /// Statement to use a database.
    /// </summary>
    public const string USE_DATABASE = "session.sql('USE `{0}`;').execute()";

    #endregion Common SQL Queries

    #region Common Table Queries

    /// <summary>
    /// Statement to delete all records in table.
    /// </summary>
    protected const string DELETE_FULL = "table.delete().execute()";

    /// <summary>
    /// Statement to delete a record using parameter binding.
    /// </summary>
    protected const string DELETE_SIMPLE_WITH_BINDING = "table.delete().where('name like :param1').bind('param1', 'Tali%').execute()";

    /// <summary>
    /// Statement to insert a couple of recors specifying the columns as an array.
    /// </summary>
    protected const string INSERT_COLUMNS_AS_ARRAY1 = "insert_query = table.insert(['universe', 'age', 'name', 'gender'])";

    /// <summary>
    /// Statement to insert a couple of recors specifying the columns as an array.
    /// </summary>
    protected const string INSERT_COLUMNS_AS_ARRAY2 = "insert_query.values('Mass Effect 2', 35, 'Miranda Lawson', 'female')";

    /// <summary>
    /// Statement to insert a couple of recors specifying the columns as an array.
    /// </summary>
    protected const string INSERT_COLUMNS_AS_ARRAY3 = "insert_query.values('Mass Effect', 31, 'John Shepard', 'male')";

    /// <summary>
    /// Statement to insert a couple of recors specifying the columns as an array.
    /// </summary>
    protected const string INSERT_COLUMNS_AS_ARRAY4 = "insert_query.execute()";

    /// <summary>
    /// Statement to insert a couple of recors specifying the columns as a comma delimited list.
    /// </summary>
    protected const string INSERT_COLUMNS_AS_LIST = "table.insert('universe', 'name', 'age', 'gender').values('Mass Effect', 'Urdnot Wrex', 1505, 'male').values('Mass Effect', \"" + TALI_MASS_EFFECT + "\", 24, 'female').execute()";

    /// <summary>
    /// Statement to insert a couple of recors without specifying the columns, just values.
    /// </summary>
    protected const string INSERT_NO_COLUMN_SPECIFICATION = "table.insert().values(28, 'Garrus Vakarian', 30, 'male', '', 'Mass Effect', 0).values(29, \"Liara T'Soni\", 109, 'female', '', 'Mass Effect', 0).execute()";

    /// <summary>
    /// Statement to insert a record in the test_table.
    /// </summary>
    protected const string INSERT_TEST_ROW1 = "table.insert().values('Adam', 15, 'male').execute()";

    /// <summary>
    /// Statement to insert a record in the test_table.
    /// </summary>
    protected const string INSERT_TEST_ROW2 = "table.insert().values('Brian', 14, 'male').execute()";

    /// <summary>
    /// Statement to insert a record in the test_table.
    /// </summary>
    protected const string INSERT_TEST_ROW3 = "table.insert().values('Alma', 13, 'female').execute()";

    /// <summary>
    /// Statement to insert a record in the test_table.
    /// </summary>
    protected const string INSERT_TEST_ROW4 = "table.insert().values('Carol', 14, 'female').execute()";

    /// <summary>
    /// Statement to insert a record in the test_table.
    /// </summary>
    protected const string INSERT_TEST_ROW5 = "table.insert().values('Donna', 16, 'female').execute()";

    /// <summary>
    /// Statement to delete the added persons and revert the table back to how it was.
    /// </summary>
    protected const string REVERT_INSERTED_CHARACTERS = "table.delete().where('base = FALSE').execute()";

    /// <summary>
    /// Statement to get all the records from the test table
    /// </summary>
    protected const string SELECT_ALL_TABLE = "table.select().execute()";

    /// <summary>
    /// Statement to get all female characters.
    /// </summary>
    protected const string SELECT_FEMALE_CHARACTERS = "table.select().where('gender = :param1').bind('param1', 'female').execute()";

    /// <summary>
    /// Statement to get specific fields from all records.
    /// </summary>
    protected const string SELECT_WITH_FIELD_SELECTION = "table.select(['name', 'age']).execute()";

    /// <summary>
    /// Statement to select all records with a value of videogames in the from column.
    /// </summary>
    protected const string SELECT_FROM_VIDEOGAMES = "table.select().where('from = :param1').bind('param1', 'videogames').execute()";

    /// <summary>
    /// Statement to get all male characters.
    /// </summary>
    protected const string SELECT_MALE_CHARACTERS = "table.select().where('gender = :param1').bind('param1', 'male').execute()";

    /// <summary>
    /// Statement to get all characters with an age > 30.
    /// </summary>
    protected const string SELECT_NON_BASE_AGE_GREATER_THAN_30 = "table.select().where('age > 30 and not base').execute()";

    /// <summary>
    /// Statement to select using paging.
    /// </summary>
    protected const string SELECT_PAGING1 = "table.select().limit(20).execute()";

    /// <summary>
    /// Statement to select using paging.
    /// </summary>
    protected const string SELECT_PAGING2 = "table.select().limit(20).offset(20).execute()";

    /// <summary>
    /// Statement to select updated records from the character table.
    /// </summary>
    protected const string SELECT_UPDATED_OLD = "table.select().where('age > 50 and universe like :param1 and not base').bind('param1', 'Old%').execute()";

    /// <summary>
    /// Statement to select an updated record from the character table.
    /// </summary>
    protected const string SELECT_UPDATED_TALI = "table.select().where('name like :param1').bind('param1', 'Tali%').execute()";

    /// <summary>
    /// Statement to update a value in all records.
    /// </summary>
    protected const string UPDATE_FULL = "table.update().set('from', 'videogames').execute()";

    /// <summary>
    /// Statement to update a record in several lines of code and using parameter binding.
    /// </summary>
    protected const string UPDATE_IN_SEVERAL_LINES1 = "update_query = table.update()";

    /// <summary>
    /// Statement to update a record in several lines of code and using parameter binding.
    /// </summary>
    protected const string UPDATE_IN_SEVERAL_LINES2 = "update_query.set('name', \"" + TALI_MASS_EFFECT_3 + "\").where('name like :param')";

    /// <summary>
    /// Statement to update a record in several lines of code and using parameter binding.
    /// </summary>
    protected const string UPDATE_IN_SEVERAL_LINES3 = "update_query.bind('param', 'Tali%')";

    /// <summary>
    /// Statement to update a record in several lines of code and using parameter binding.
    /// </summary>
    protected const string UPDATE_IN_SEVERAL_LINES4 = "update_query.execute()";

    /// <summary>
    /// Statement to update a record in a simple way.
    /// </summary>
    protected const string UPDATE_SIMPLE = "table.update().set('universe', 'Mass Effect 3').where('name like :param1').bind('param1', 'Tali%').execute()";

    /// <summary>
    /// Statement to update a record using an expression.
    /// </summary>
    protected const string UPDATE_WITH_EXPRESSION = "table.update().set('age', mysqlx.expr('20+5')).where('name like :param1').bind('param1', 'Tali%').execute()";

    /// <summary>
    /// Statement to update a record using an expression.
    /// </summary>
    protected const string UPDATE_WITH_LIMIT = "table.update().set('universe', 'Old People').where('age > 50 and not base').limit(2).execute()";

    #endregion Common Table Queries

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseTableTests"/> class.
    /// </summary>
    /// <param name="scriptLanguage">The language used for the tests.</param>
    /// <param name="xecutor">The type of class that will run X Protocol statements.</param>
    protected BaseTableTests(ScriptLanguageType scriptLanguage, XecutorType xecutor)
      : base (scriptLanguage, xecutor)
    {
      TableTestProps = TableTestsPropertiesFactory.GetTableTestsProperties(scriptLanguage);
    }

    #region Properties

    /// <summary>
    /// Gets properties used for a specific language.
    /// </summary>
    public TableTestsProperties TableTestProps { get; protected set; }

    #endregion Properties

    /// <summary>
    /// Test to create a database.
    /// </summary>
    [Fact]
    public virtual void CreateDatabase()
    {
      OpenConnection();
      MySqlDataReader reader = null;

      try
      {
        InitXecutor();

        ExecuteQuery(TableTestProps.DropTestDatabaseIfExists);
        ExecuteQuery(TableTestProps.CreateTestDatabase);
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

        ExecuteQuery(TableTestProps.DropTestDatabaseIfExists);
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
        DisposeXecutor();
      }
    }

    /// <summary>
    /// Test to create a table.
    /// </summary>
    [Fact]
    public virtual void CreateTable()
    {
      OpenConnection();

      try
      {
        InitXecutor();

        ExecuteQuery(TableTestProps.DropTestDatabaseIfExists);
        ExecuteQuery(TableTestProps.CreateTestDatabase);
        ExecuteQuery(TableTestProps.UseTestDatabase);

        ExecuteQuery(CREATE_TEST_TABLE);
        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, TEST_TABLE_NAME, TEMP_TEST_DATABASE_NAME), Connection);
        var result = Command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, TEST_TABLE_NAME));

        ExecuteQuery(TableTestProps.DropTestTableIfExists);
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
        DisposeXecutor();
      }
    }

    /// <summary>
    /// Test to delete records in a table.
    /// </summary>
    [Fact]
    public virtual void Delete()
    {
      OpenConnection();

      try
      {
        InitXecutor();

        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, CHARACTER_TABLE_NAME, X_TEST_SCHEMA_NAME), Connection);
        var result = Command.ExecuteScalar();
        int count;
        int charactersCount = CHARACTERS_FULL_COUNT;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, CHARACTER_TABLE_NAME));

        // Create test table
        ExecuteQuery(TableTestProps.DropTestDatabaseIfExists);
        ExecuteQuery(TableTestProps.CreateTestDatabase);
        ExecuteQuery(TableTestProps.UseTestDatabase);
        ExecuteQuery(CREATE_TEST_TABLE);
        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, TEST_TABLE_NAME, TEMP_TEST_DATABASE_NAME), Connection);
        result = Command.ExecuteScalar();
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, TEST_TABLE_NAME));

        // Insert test table data for delete all
        ExecuteQuery(TableTestProps.GetDatabaseTest);
        ExecuteQuery(TableTestProps.GetDatabaseTestTableTest);
        ExecuteQuery(INSERT_TEST_ROW1);
        ExecuteQuery(INSERT_TEST_ROW2);
        ExecuteQuery(INSERT_TEST_ROW3);
        ExecuteQuery(INSERT_TEST_ROW4);
        ExecuteQuery(INSERT_TEST_ROW5);
        var selectResult = ExecuteSingleStatement(SELECT_ALL_TABLE);
        Assert.True(selectResult != null && selectResult.Count == TEST_COUNT, DATA_NOT_MATCH);

        // Delete full
        ExecuteQuery(DELETE_FULL);
        selectResult = ExecuteSingleStatement(SELECT_ALL_TABLE);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 0, DATA_NOT_MATCH);

        // Insert test character rows
        ExecuteQuery(TableTestProps.UseXTestDatabase);
        ExecuteQuery(TableTestProps.GetTableXTestCharacter);
        ExecuteQuery(INSERT_NO_COLUMN_SPECIFICATION);
        ExecuteQuery(INSERT_COLUMNS_AS_LIST);
        ExecuteQuery(INSERT_COLUMNS_AS_ARRAY1);
        ExecuteQuery(INSERT_COLUMNS_AS_ARRAY2);
        ExecuteQuery(INSERT_COLUMNS_AS_ARRAY3);
        ExecuteQuery(INSERT_COLUMNS_AS_ARRAY4);
        ExecuteQuery(TableTestProps.InsertJsonDocument1);
        ExecuteQuery(TableTestProps.InsertJsonDocument2);
        charactersCount += 8;
        selectResult = ExecuteSingleStatement(SELECT_ALL_TABLE);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == charactersCount, DATA_NOT_MATCH);

        // Delete simple, using parameter binding
        ExecuteQuery(DELETE_SIMPLE_WITH_BINDING);
        selectResult = ExecuteSingleStatement(SELECT_UPDATED_TALI);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 0, DATA_NOT_MATCH);

        // Delete with limit
        ExecuteQuery(TableTestProps.DeleteWithLimit);
        selectResult = ExecuteSingleStatement(SELECT_NON_BASE_AGE_GREATER_THAN_30);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == CHARACTERS_NON_BASE_AGE_GREATER_THAN_30_COUNT - 2, DATA_NOT_MATCH);

        // Delete with limit again
        ExecuteQuery(TableTestProps.DeleteWithLimit);
        selectResult = ExecuteSingleStatement(SELECT_NON_BASE_AGE_GREATER_THAN_30);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == CHARACTERS_NON_BASE_AGE_GREATER_THAN_30_COUNT - 4, DATA_NOT_MATCH);

        // Delete inserted test rows
        ExecuteQuery(REVERT_INSERTED_CHARACTERS);
        selectResult = ExecuteSingleStatement(SELECT_ALL_TABLE);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == CHARACTERS_FULL_COUNT, DATA_NOT_MATCH);

        // Drop test table
        ExecuteQuery(TableTestProps.UseTestDatabase);
        ExecuteQuery(TableTestProps.DropTestTableIfExists);
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

        ExecuteQuery(REVERT_INSERTED_CHARACTERS);
        CloseConnection();
        DisposeXecutor();
      }
    }

    /// <summary>
    /// Test to insert records into a table.
    /// </summary>
    [Fact]
    public virtual void Insert()
    {
      OpenConnection();

      try
      {
        InitXecutor();

        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, CHARACTER_TABLE_NAME, X_TEST_SCHEMA_NAME), Connection);
        var result = Command.ExecuteScalar();
        int count;
        int charactersCount = CHARACTERS_FULL_COUNT;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, CHARACTER_TABLE_NAME));

        // Insert without specifying any columns
        ExecuteQuery(TableTestProps.GetTableXTestCharacter);
        ExecuteQuery(INSERT_NO_COLUMN_SPECIFICATION);
        charactersCount += 2;
        var selectResult = ExecuteSingleStatement(SELECT_ALL_TABLE);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == charactersCount, DATA_NOT_MATCH);

        // Insert specifying a comma delimited list of columns
        ExecuteQuery(INSERT_COLUMNS_AS_LIST);
        charactersCount += 2;
        selectResult = ExecuteSingleStatement(SELECT_ALL_TABLE);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == charactersCount, DATA_NOT_MATCH);

        // Insert specifying columns as an array, also in different lines
        ExecuteQuery(INSERT_COLUMNS_AS_ARRAY1);
        ExecuteQuery(INSERT_COLUMNS_AS_ARRAY2);
        ExecuteQuery(INSERT_COLUMNS_AS_ARRAY3);
        ExecuteQuery(INSERT_COLUMNS_AS_ARRAY4);
        charactersCount += 2;
        selectResult = ExecuteSingleStatement(SELECT_ALL_TABLE);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == charactersCount, DATA_NOT_MATCH);

        // Insert JSON documents
        ExecuteQuery(TableTestProps.InsertJsonDocument1);
        ExecuteQuery(TableTestProps.InsertJsonDocument2);
        charactersCount += 2;
        selectResult = ExecuteSingleStatement(SELECT_ALL_TABLE);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == charactersCount, DATA_NOT_MATCH);

        // Delete inserted rows
        ExecuteQuery(REVERT_INSERTED_CHARACTERS);
        selectResult = ExecuteSingleStatement(SELECT_ALL_TABLE);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == CHARACTERS_FULL_COUNT, DATA_NOT_MATCH);
      }
      finally
      {
        if (Command != null)
        {
          Command.Dispose();
        }

        ExecuteQuery(REVERT_INSERTED_CHARACTERS);
        CloseConnection();
        DisposeXecutor();
      }
    }

    /// <summary>
    /// Test to select records in a table.
    /// </summary>
    [Fact]
    public virtual void Select()
    {
      OpenConnection();

      try
      {
        InitXecutor();

        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, CHARACTER_TABLE_NAME, X_TEST_SCHEMA_NAME), Connection);
        var result = Command.ExecuteScalar();
        int count;
        int charactersCount = CHARACTERS_FULL_COUNT;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, CHARACTER_TABLE_NAME));

        // Insert test character rows
        ExecuteQuery(TableTestProps.GetTableXTestCharacter);
        ExecuteQuery(INSERT_NO_COLUMN_SPECIFICATION);
        ExecuteQuery(INSERT_COLUMNS_AS_LIST);
        ExecuteQuery(INSERT_COLUMNS_AS_ARRAY1);
        ExecuteQuery(INSERT_COLUMNS_AS_ARRAY2);
        ExecuteQuery(INSERT_COLUMNS_AS_ARRAY3);
        ExecuteQuery(INSERT_COLUMNS_AS_ARRAY4);
        ExecuteQuery(TableTestProps.InsertJsonDocument1);
        ExecuteQuery(TableTestProps.InsertJsonDocument2);
        charactersCount += 8;

        // Select all
        var selectResult = ExecuteSingleStatement(SELECT_ALL_TABLE);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == charactersCount, DATA_NOT_MATCH);

        // Select female
        selectResult = ExecuteSingleStatement(SELECT_FEMALE_CHARACTERS);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == CHARACTERS_FEMALE_COUNT, DATA_NOT_MATCH);

        // Select male
        selectResult = ExecuteSingleStatement(SELECT_MALE_CHARACTERS);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == CHARACTERS_MALE_COUNT, DATA_NOT_MATCH);

        // Select with field selection
        selectResult = ExecuteSingleStatement(SELECT_WITH_FIELD_SELECTION);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == charactersCount, DATA_NOT_MATCH);
        Assert.True(selectResult != null && selectResult.Count > 0 && selectResult[0].Count == 2, DATA_NOT_MATCH);

        // Select with order by descending
        selectResult = ExecuteSingleStatement(TableTestProps.SelectWithOrderByDesc);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        var singleResult = selectResult.FirstOrDefault();
        int fetchedAge = singleResult != null ? Convert.ToInt32(singleResult["age"]) : 0;
        Assert.True(fetchedAge == CHARACTERS_HIGHEST_AGE, DATA_NOT_MATCH);
        singleResult = selectResult.ElementAtOrDefault(1);
        fetchedAge = singleResult != null ? Convert.ToInt32(singleResult["age"]) : 0;
        Assert.True(fetchedAge == CHARACTERS_SECOND_HIGHEST_AGE, DATA_NOT_MATCH);

        // Select by paging (limit + offset)
        selectResult = ExecuteSingleStatement(SELECT_PAGING1);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == CHARACTERS_PAGE_SIZE, DATA_NOT_MATCH);
        selectResult = ExecuteSingleStatement(SELECT_PAGING2);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == charactersCount - CHARACTERS_PAGE_SIZE, DATA_NOT_MATCH);

        // Delete inserted test rows
        ExecuteQuery(REVERT_INSERTED_CHARACTERS);
        selectResult = ExecuteSingleStatement(SELECT_ALL_TABLE);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == CHARACTERS_FULL_COUNT, DATA_NOT_MATCH);
      }
      finally
      {
        if (Command != null)
        {
          Command.Dispose();
        }

        ExecuteQuery(REVERT_INSERTED_CHARACTERS);
        CloseConnection();
        DisposeXecutor();
      }
    }

    /// <summary>
    /// Test to update records in a table.
    /// </summary>
    [Fact]
    public virtual void Update()
    {
      OpenConnection();

      try
      {
        InitXecutor();

        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, CHARACTER_TABLE_NAME, X_TEST_SCHEMA_NAME), Connection);
        var result = Command.ExecuteScalar();
        int count;
        int charactersCount = CHARACTERS_FULL_COUNT;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, CHARACTER_TABLE_NAME));

        // Insert test rows
        ExecuteQuery(TableTestProps.GetTableXTestCharacter);
        ExecuteQuery(INSERT_NO_COLUMN_SPECIFICATION);
        ExecuteQuery(INSERT_COLUMNS_AS_LIST);
        ExecuteQuery(INSERT_COLUMNS_AS_ARRAY1);
        ExecuteQuery(INSERT_COLUMNS_AS_ARRAY2);
        ExecuteQuery(INSERT_COLUMNS_AS_ARRAY3);
        ExecuteQuery(INSERT_COLUMNS_AS_ARRAY4);
        ExecuteQuery(TableTestProps.InsertJsonDocument1);
        ExecuteQuery(TableTestProps.InsertJsonDocument2);
        charactersCount += 8;
        var selectResult = ExecuteSingleStatement(SELECT_ALL_TABLE);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == charactersCount, DATA_NOT_MATCH);

        // Update simple, 1 record 1 value, using parameter binding
        ExecuteQuery(UPDATE_SIMPLE);
        selectResult = ExecuteSingleStatement(SELECT_UPDATED_TALI);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        var singleResult = selectResult.FirstOrDefault();
        Assert.True(singleResult != null && singleResult["universe"].ToString().Equals("Mass Effect 3", StringComparison.InvariantCultureIgnoreCase), DATA_NOT_MATCH);

        // Update a singe value with statements in different lines, using parameter binding
        ExecuteQuery(UPDATE_IN_SEVERAL_LINES1);
        ExecuteQuery(UPDATE_IN_SEVERAL_LINES2);
        ExecuteQuery(UPDATE_IN_SEVERAL_LINES3);
        ExecuteQuery(UPDATE_IN_SEVERAL_LINES4);
        selectResult = ExecuteSingleStatement(SELECT_UPDATED_TALI);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        singleResult = selectResult.FirstOrDefault();
        Assert.True(singleResult != null && singleResult["name"].ToString().Equals(TALI_MASS_EFFECT_3, StringComparison.InvariantCultureIgnoreCase), DATA_NOT_MATCH);

        // Update using an expression
        ExecuteQuery(TableTestProps.IncludeMysqlx);
        ExecuteQuery(UPDATE_WITH_EXPRESSION);
        selectResult = ExecuteSingleStatement(SELECT_UPDATED_TALI);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        singleResult = selectResult.FirstOrDefault();
        Assert.True(singleResult != null && singleResult["age"].ToString().Equals("25", StringComparison.InvariantCultureIgnoreCase), DATA_NOT_MATCH);

        // Update with limit
        ExecuteQuery(UPDATE_WITH_LIMIT);
        selectResult = ExecuteSingleStatement(SELECT_UPDATED_OLD);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 2, DATA_NOT_MATCH);

        // Update with limit
        ExecuteQuery(UPDATE_FULL);
        selectResult = ExecuteSingleStatement(SELECT_FROM_VIDEOGAMES);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == charactersCount, DATA_NOT_MATCH);

        // Delete inserted test rows
        ExecuteQuery(REVERT_INSERTED_CHARACTERS);
        selectResult = ExecuteSingleStatement(SELECT_ALL_TABLE);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == CHARACTERS_FULL_COUNT, DATA_NOT_MATCH);
      }
      finally
      {
        if (Command != null)
        {
          Command.Dispose();
        }

        ExecuteQuery(REVERT_INSERTED_CHARACTERS);
        CloseConnection();
        DisposeXecutor();
      }
    }

    /// <summary>
    /// Test to get views as table objects, check they are actually views, and query data.
    /// </summary>
    [Fact]
    public void Views()
    {
      OpenConnection();

      try
      {
        InitXecutor();

        // Check that getting tables return views as well
        var tables = ExecuteSingleStatement(string.Format(TableTestProps.GetTables, X_TEST_SCHEMA_NAME));
        Assert.True(tables != null && tables.Count == XTEST_TABLES_COUNT);

        // Get first view
        ExecuteQuery(string.Format(TableTestProps.GetTable, X_TEST_SCHEMA_NAME, HALO_CHARACTERS_VIEW_NAME));
        var result = ExecuteQuery(TableTestProps.TableIsView);
        Assert.True(result != null && Convert.ToBoolean(result));

        // Test the rows in first view
        var results = ExecuteSingleStatement(SELECT_ALL_TABLE);
        Assert.True(results != null && results.Count == HALO_CHARACTERS_VIEW_ROWS_COUNT);

        // Get second view
        ExecuteQuery(string.Format(TableTestProps.GetTable, X_TEST_SCHEMA_NAME, MASS_EFFECT_CHARACTERS_VIEW_NAME));
        result = ExecuteQuery(TableTestProps.TableIsView);
        Assert.True(result != null && Convert.ToBoolean(result));

        // Test the rows in second view
        results = ExecuteSingleStatement(SELECT_ALL_TABLE);
        Assert.True(results != null && results.Count == MASS_EFFECT_CHARACTERS_VIEW_ROWS_COUNT);

        // Get third view
        ExecuteQuery(string.Format(TableTestProps.GetTable, X_TEST_SCHEMA_NAME, SPARTAN_CHARACTERS_VIEW_NAME));
        result = ExecuteQuery(TableTestProps.TableIsView);
        Assert.True(result != null && Convert.ToBoolean(result));

        // Test the rows in third view
        results = ExecuteSingleStatement(SELECT_ALL_TABLE);
        Assert.True(results != null && results.Count == SPARTAN_CHARACTERS_VIEW_ROWS_COUNT);
      }
      finally
      {
        if (Command != null)
        {
          Command.Dispose();
        }

        CloseConnection();
        DisposeXecutor();
      }
    }
  }
}
