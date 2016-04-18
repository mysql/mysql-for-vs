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

namespace MySql.VisualStudio.Tests.MySqlX.Base
{
  public class BaseTableTests : BaseTests
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
    protected const int CHARACTERS_MALE_COUNT = + 20;

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

    #endregion Constant Values

    #region JavaScript specific

    /// <summary>
    /// Statement to insert JSON documents into a table.
    /// </summary>
    protected const string JAVASCRIPT_INSERT_JSON_DOCUMENT1 = "table.insert({ age: 28, gender: 'female', name: 'Ashley Williams', universe: 'Mass Effect' }).execute();";

    /// <summary>
    /// Statement to insert JSON documents into a table.
    /// </summary>
    protected const string JAVASCRIPT_INSERT_JSON_DOCUMENT2 = "table.insert({ age: 850, gender: 'female', name: 'Samara', universe: 'Mass Effect 2' }).execute();";

    #endregion JavaScript specific

    #region Python specific

    /// <summary>
    /// Statement to insert JSON documents into a table.
    /// </summary>
    protected const string PYTHON_INSERT_JSON_DOCUMENT1 = "table.insert({ 'age': 28, 'gender': 'female', 'name': 'Ashley Williams', 'universe': 'Mass Effect' }).execute()";

    /// <summary>
    /// Statement to insert JSON documents into a table.
    /// </summary>
    protected const string PYTHON_INSERT_JSON_DOCUMENT2 = "table.insert({ 'age': 850, 'gender': 'female', 'name': 'Samara', 'universe': 'Mass Effect 2' }).execute()";

    #endregion Python specific

    #region Common SQL Queries

    /// <summary>
    /// Statement to create a database
    /// </summary>
    protected const string CREATE_DATABASE = "session.sql('CREATE SCHEMA `{0}`;').execute()";

    /// <summary>
    /// Statement to create the test table
    /// </summary>
    protected const string CREATE_TEST_TABLE = "session.sql('CREATE TABLE " + TEST_TABLE_NAME + " (name VARCHAR(50), age INTEGER, gender VARCHAR(20));').execute()";

    /// <summary>
    /// Statement to drop a table if it already exists.
    /// </summary>
    protected const string DROP_TABLE_IF_EXISTS = "session.sql('DROP TABLE IF EXISTS `{0}`;').execute()";

    /// <summary>
    /// Statement to use a database.
    /// </summary>
    protected const string USE_DATABASE = "session.sql('USE `{0}`;').execute()";

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
    /// Statement to delete a record using parameter binding.
    /// </summary>
    protected const string DELETE_WITH_LIMIT = "table.delete().where('age > :param1 and not base').orderBy(['age']).limit(2).bind('param1', 30).execute()";

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
    /// Gets the test schema and assigns it to a variable for persistence.
    /// </summary>
    protected const string GET_SCHEMA = "schema = session.getSchema('{0}')";

    /// <summary>
    /// Get a specific table and assign it to a variable for persistence. 
    /// </summary>
    protected const string GET_TABLE = "table = session.{0}.getTable('{1}')";

    /// <summary>
    /// Get a specific table and assign it to a variable for persistence. 
    /// </summary>
    protected const string GET_TABLE_IN_SCHEMA = "table = schema.getTable('{0}')";

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
    /// Statement to get all records ordered by a criteria.
    /// </summary>
    protected const string SELECT_WITH_ORDER_BY_DESC = "table.select().orderBy(['age DESC']).execute()";

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

    #region Properties

    /// <summary>
    /// Statement to create the test database.
    /// </summary>
    public string CreateTestDatabase { get; private set; }

    /// <summary>
    /// Statement to drop the test database.
    /// </summary>
    public string DropTestDatabaseIfExists { get; private set; }

    /// <summary>
    /// Statement to drop the test table if it already exists.
    /// </summary>
    public string DropTestTableIfExists { get; private set; }

    /// <summary>
    /// Statement to get the database_test schema.
    /// </summary>
    public string GetDatabaseTest { get; private set; }

    /// <summary>
    /// Statement to get the table_test collection from the database_test schema.
    /// </summary>
    public string GetDatabaseTestTableTest { get; private set; }

    /// <summary>
    /// Statement to get the character collection from the SakilaX schema.
    /// </summary>
    public string GetTableSakilaXCharacter { get; private set; }

    /// <summary>
    /// Statement to use the SakilaX database.
    /// </summary>
    public string UseSakilaXDatabase { get; private set; }

    /// <summary>
    /// Statement to use the test database.
    /// </summary>
    public string UseTestDatabase { get; private set; }

    #endregion Properties

    public BaseTableTests()
    {
      CreateTestDatabase = string.Format(CREATE_DATABASE, TEST_DATABASE_NAME);
      DropTestDatabaseIfExists = string.Format(DROP_DATABASE_IF_EXISTS, TEST_DATABASE_NAME);
      DropTestTableIfExists = string.Format(DROP_TABLE_IF_EXISTS, TEST_TABLE_NAME);
      GetDatabaseTest = string.Format(GET_SCHEMA, TEST_DATABASE_NAME);
      GetDatabaseTestTableTest = string.Format(GET_TABLE_IN_SCHEMA, TEST_TABLE_NAME);
      GetTableSakilaXCharacter = string.Format(GET_TABLE, SAKILA_X_SCHEMA_NAME, SAKILA_X_CHARACTER_TABLE);
      UseSakilaXDatabase = string.Format(USE_DATABASE, SAKILA_X_SCHEMA_NAME);
      UseTestDatabase = string.Format(USE_DATABASE, TEST_DATABASE_NAME);
    }

  }
}
