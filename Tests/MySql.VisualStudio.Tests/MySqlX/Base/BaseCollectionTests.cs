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
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using MySql.Utility.Enums;
using Xunit;

namespace MySql.VisualStudio.Tests.MySqlX.Base
{
  public abstract class BaseCollectionTests : BaseTests
  {
    #region Constant Values

    /// <summary>
    /// Bound array value.
    /// </summary>
    protected const string MOVIES_ACTORS_ARRAY_VALUE = "['ADAM GRANT', 'CATE MCQUEEN', 'GRETA KEITEL']";

    /// <summary>
    /// The duplicate title used for testing uniqueness in sakila_x.movies.
    /// </summary>
    public const string MOVIES_DUPLICATE_TITLE = "BAKED CLEOPATRA";

    /// <summary>
    /// The name of the non-unique index created on the movies collection.
    /// </summary>
    public const string MOVIES_NON_UNIQUE_INDEX_NAME = "rating_index";

    /// <summary>
    /// Bound array value.
    /// </summary>
    public const string MOVIES_RATING_ARRAY_VALUE = "['R', 'NC-17']";

    /// <summary>
    /// The total count of rating = R movies.
    /// </summary>
    protected const int MOVIES_RATING_R_COUNT = 195;

    /// <summary>
    /// The name of the unique index created on the movies collection.
    /// </summary>
    public const string MOVIES_UNIQUE_INDEX_NAME = "title_index";

    /// <summary>
    /// The total users count.
    /// </summary>
    protected const int USERS_COUNT = 10;

    #endregion Constant Values

    #region Common Collection Queries

    /// <summary>
    /// Statement to get all the records from the test table as RowResult
    /// </summary>
    protected const string FIND_ALL_DOCUMENTS_IN_COLLECTION = "coll.find().execute()";

    /// <summary>
    /// Statement to get all the movies with a duplicate movie title of MOVIES_DUPLICATE_TITLE.
    /// </summary>
    protected const string FIND_DUPLICATE_MOVIE_TITLE = "coll.find('title = :param1').bind('param1', '" + MOVIES_DUPLICATE_TITLE + "').execute()";

    /// <summary>
    /// Statement to find the updated SakilaX.User document.
    /// </summary>
    protected const string FIND_MODIFIED_J_USERS = "coll.find('name like :param1 and status = :param2').bind('param1', 'Javier%').bind('param2', 'tocayo').execute()";

    /// <summary>
    /// Statement to find the updated SakilaX.User document.
    /// </summary>
    protected const string FIND_MODIFIED_USER = "coll.find('name like :findParam').bind('findParam', 'Iggy%').execute()";

    /// <summary>
    /// Statement to find a specific record using a bound array.
    /// </summary>
    protected const string FIND_MOVIES_BOUND_ARRAY = "coll.find('actors = :param1').fields(['_id', 'title', 'duration', 'actors']).bind('param1', " + MOVIES_ACTORS_ARRAY_VALUE + ").execute()";

    /// <summary>
    /// Statement to find records with a complex query.
    /// </summary>
    protected const string FIND_MOVIES_COMPLEX_QUERY1 = "complexQuery = coll.find('rating = :param1').fields(['rating', 'title', 'description', 'duration']).sort(['release_year'])";

    /// <summary>
    /// Statement to find records with a complex query.
    /// </summary>
    protected const string FIND_MOVIES_COMPLEX_QUERY2 = "complexQuery2 = coll.find('rating = :param1').fields(['rating', 'title', 'description', 'duration']).sort(['release_year']).limit(10).skip(5)";

    /// <summary>
    /// Statement to find records with a complex query.
    /// </summary>
    protected const string FIND_MOVIES_COMPLEX_QUERY3 = "complexQuery.bind('param1', 'R').execute()";

    /// <summary>
    /// Statement to find records with a complex query.
    /// </summary>
    protected const string FIND_MOVIES_COMPLEX_QUERY4 = "complexQuery2.bind('param1', 'R').execute()";

    /// <summary>
    /// Statement to find users that were removed with REMOVE_SORT_USER.
    /// </summary>
    protected const string FIND_REMOVED_USERS = "coll.find('name like :param1 or name like :param2').bind('param1', 'Fr%').bind('param2', 'Ig%').execute()";

    /// <summary>
    /// Statement to select the update record from the test table
    /// </summary>
    protected const string FIND_SPECIFIC_USER_TEST = "coll.find('name like :param1').bind('param1', 'Reggie%').execute()";

    /// <summary>
    /// Statement to set a SakilaX.User with a new value using a bound array.
    /// </summary>
    protected const string MODIFY_SET_BINDING_ARRAY_USER = "coll.modify('name like :param1').set('ratings', " + MOVIES_RATING_ARRAY_VALUE + ").bind('param1', 'Iggy%').execute()";

    /// <summary>
    /// Expression evaluation.
    /// </summary>
    protected const string MODIFY_SET_EXPRESSION = "mysqlx.expr('60 / 2 + 8')";

    /// <summary>
    /// Statement to modify a SakilaX.User value and set additional values.
    /// </summary>
    protected const string MODIFY_SET_USER = "coll.modify('name like :param1').set('name', 'Iggy Galarza').set('status', 'inactive').set('age', "+ MODIFY_SET_EXPRESSION + ").bind('param1', 'Ignacio%').execute()";

    /// <summary>
    /// Statement to modify a SakilaX.User value and set additional values in a specific order.
    /// </summary>
    protected const string MODIFY_SORT_USER = "coll.modify('name like :param1').set('status', 'tocayo').sort(['name']).limit(2).bind('param1', 'J%').execute()";

    /// <summary>
    /// Statement to unset a SakilaX.User a list of keys.
    /// </summary>
    protected const string MODIFY_UNSET_LIST_USER = "coll.modify('name like :param1').unset(['status', 'ratings']).bind('param1', 'Iggy%').execute()";

    /// <summary>
    /// Statement to unset a SakilaX.User value.
    /// </summary>
    protected const string MODIFY_UNSET_USER = "coll.modify('name like :param1').unset('age').bind('param1', 'Iggy%').execute()";
    /// <summary>
    /// Statement to delete a test duplicate movie record.
    /// </summary>
    protected const string REMOVE_DUPLICATE_MOVIE = "coll.remove('title = :param1 and rating = :param2').bind('param1', '" + MOVIES_DUPLICATE_TITLE + "').bind('param2', 'DEL').execute()";

    /// <summary>
    /// Statement to delete the first x sorted records in SakilaX.User
    /// </summary>
    protected const string REMOVE_SORT_USER = "coll.remove('test = :param1').sort(['name']).limit(2).bind('param1', 'yes').execute()";

    /// <summary>
    /// Statement to delete a specific record in SakilaX.User
    /// </summary>
    protected const string REMOVE_USER = "coll.remove('name like :param').bind('param', 'Alfonso%').execute()";

    /// <summary>
    /// Statement to remove the added users and revert the collection back to how it was.
    /// </summary>
    protected const string REVERT_ADDED_USERS = "coll.remove('test = :param1').bind('param1', 'yes').execute()";

    #endregion Common Collection Queries

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseCollectionTests"/> class.
    /// </summary>
    /// <param name="scriptLanguage">The language used for the tests.</param>
    /// <param name="xecutor">The type of class that will run X Protocol statements.</param>
    protected BaseCollectionTests(ScriptLanguageType scriptLanguage, XecutorType xecutor)
      : base (scriptLanguage, xecutor)
    {
      CollectionTestProps = CollectionTestsPropertiesFactory.GetCollectionTestsProperties(scriptLanguage);
    }

    #region Properties

    /// <summary>
    /// Gets properties used for a specific language.
    /// </summary>
    public CollectionTestsProperties CollectionTestProps { get; protected set; }

    #endregion Properties

    /// <summary>
    /// Test to add, modify, delete and find a record from a collection, executing the commands in a single line.
    /// </summary>
    [Fact]
    public void AddFind()
    {
      OpenConnection();

      try
      {
        InitXecutor();
        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, USERS_COLLECTION_NAME, X_TEST_SCHEMA_NAME), Connection);

        var result = Command.ExecuteScalar();
        int count;
        int usersCount = USERS_COUNT;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, USERS_COLLECTION_NAME));

        ExecuteQuery(CollectionTestProps.GetSchemaXTest);
        ExecuteQuery(CollectionTestProps.GetCollectionXTextUser);

        // Test single add
        ExecuteQuery(CollectionTestProps.AddSingleUser1);
        var selectResult = ExecuteSingleStatement(FIND_ALL_DOCUMENTS_IN_COLLECTION);
        usersCount += 1;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == usersCount, DATA_NOT_MATCH);

        // Test single add again
        ExecuteQuery(CollectionTestProps.AddSingleUser2);
        selectResult = ExecuteSingleStatement(FIND_ALL_DOCUMENTS_IN_COLLECTION);
        usersCount += 1;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == usersCount, DATA_NOT_MATCH);

        //Test multiple documents add statement
        ExecuteQuery(CollectionTestProps.AddMultipleUsersSingleAdd);
        selectResult = ExecuteSingleStatement(FIND_ALL_DOCUMENTS_IN_COLLECTION);
        usersCount += 3;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == usersCount, DATA_NOT_MATCH);

        //Test multiple add statements with single documents
        ExecuteQuery(CollectionTestProps.AddMultipleUsersMultipleAdd);
        selectResult = ExecuteSingleStatement(FIND_ALL_DOCUMENTS_IN_COLLECTION);
        usersCount += 3;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == usersCount, DATA_NOT_MATCH);

        selectResult = ExecuteSingleStatement(FIND_SPECIFIC_USER_TEST);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 1, DATA_NOT_MATCH);

        // Remove back the added users and test
        ExecuteQuery(REVERT_ADDED_USERS);
        selectResult = ExecuteSingleStatement(FIND_ALL_DOCUMENTS_IN_COLLECTION);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == USERS_COUNT, DATA_NOT_MATCH);
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
    /// Test to create and drop a collection.
    /// </summary>
    [Fact]
    public void CreateAndDropCollection()
    {
      OpenConnection();

      try
      {
        InitXecutor();
        ExecuteQuery(CollectionTestProps.DropSchemaTempSchemaIfExists);
        ExecuteQuery(CollectionTestProps.CreateSchemaTempSchema);
        ExecuteQuery(CollectionTestProps.CreateCollectionTest);
        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, TEST_COLLECTION_NAME, TEMP_SCHEMA_NAME), Connection);
        var result = Command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(COLLECTION_NOT_FOUND, TEST_COLLECTION_NAME));

        ExecuteQuery(CollectionTestProps.DropCollectionTest);
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

        CloseConnection();
        DisposeXecutor();
      }
    }

    /// <summary>
    /// Test to create and drop unique and non-unique indexes.
    /// </summary>
    [Fact]
    public void CreateAndDropIndex()
    {
      OpenConnection();
      int duplicateMovieCount = 0;

      try
      {
        InitXecutor();

        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, MOVIES_COLLECTION_NAME, X_TEST_SCHEMA_NAME), Connection);
        var result = Command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, MOVIES_COLLECTION_NAME));

        ExecuteQuery(CollectionTestProps.GetSchemaXTest);
        ExecuteQuery(CollectionTestProps.GetCollectionXTestMovies);

        // Add non-unique index
        ExecuteQuery(CollectionTestProps.CreateNonUniqueIndexMovies);
        Command = new MySqlCommand(string.Format(SEARCH_INDEX_SQL_SYNTAX, X_TEST_SCHEMA_NAME, MOVIES_COLLECTION_NAME, MOVIES_NON_UNIQUE_INDEX_NAME), Connection);
        result = Command.ExecuteScalar();
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(INDEX_NOT_FOUND, MOVIES_NON_UNIQUE_INDEX_NAME));

        // Add unique index
        ExecuteQuery(CollectionTestProps.IncludeMysqlx);
        ExecuteQuery(CollectionTestProps.CreateUniqueIndexMovies);
        Command = new MySqlCommand(string.Format(SEARCH_INDEX_SQL_SYNTAX, X_TEST_SCHEMA_NAME, MOVIES_COLLECTION_NAME, MOVIES_UNIQUE_INDEX_NAME), Connection);
        result = Command.ExecuteScalar();
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(INDEX_NOT_FOUND, MOVIES_UNIQUE_INDEX_NAME));

        // Test data uniqueness
        ExecuteQuery(CollectionTestProps.AddDuplicateMovie);
        var selectResult = ExecuteSingleStatement(FIND_DUPLICATE_MOVIE_TITLE);
        duplicateMovieCount = selectResult != null ? selectResult.Count : 0;
        Assert.True(duplicateMovieCount == 1, DATA_NOT_UNIQUE);

        // Drop non-unique index
        ExecuteQuery(CollectionTestProps.DropNonUniqueIndexMovies);
        Command = new MySqlCommand(string.Format(SEARCH_INDEX_SQL_SYNTAX, X_TEST_SCHEMA_NAME, MOVIES_COLLECTION_NAME, MOVIES_NON_UNIQUE_INDEX_NAME), Connection);
        result = Command.ExecuteScalar();
        int.TryParse(result.ToString(), out count);
        Assert.True(count == 0, string.Format(INDEX_NOT_FOUND, MOVIES_NON_UNIQUE_INDEX_NAME));

        // Drop unique index
        ExecuteQuery(CollectionTestProps.DropUniqueIndexMovies);
        Command = new MySqlCommand(string.Format(SEARCH_INDEX_SQL_SYNTAX, X_TEST_SCHEMA_NAME, MOVIES_COLLECTION_NAME, MOVIES_UNIQUE_INDEX_NAME), Connection);
        result = Command.ExecuteScalar();
        int.TryParse(result.ToString(), out count);
        Assert.True(count == 0, string.Format(INDEX_NOT_FOUND, MOVIES_UNIQUE_INDEX_NAME));
      }
      finally
      {
        // Remove duplicate data in case test failed
        if (duplicateMovieCount > 1)
        {
          ExecuteSingleStatement(REMOVE_DUPLICATE_MOVIE);
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
    /// Test to create and drop a schema.
    /// </summary>
    [Fact]
    public void CreateAndDropSchema()
    {
      OpenConnection();
      MySqlDataReader reader = null;

      try
      {
        InitXecutor();
        ExecuteQuery(CollectionTestProps.DropSchemaTempSchemaIfExists);
        ExecuteQuery(CollectionTestProps.CreateSchemaTempSchema);
        Command = new MySqlCommand(SHOW_DBS_SQL_SYNTAX, Connection);
        reader = Command.ExecuteReader();
        bool success = false;
        while (reader.Read())
        {
          var retSchema = reader.GetString(0);
          if (retSchema == TEMP_SCHEMA_NAME)
          {
            success = true;
            reader.Close();
            break;
          }
        }

        Assert.True(success, string.Format(SCHEMA_NOT_FOUND, TEMP_SCHEMA_NAME));

        ExecuteQuery(CollectionTestProps.DropSchemaTempSchema);
        Command = new MySqlCommand(SHOW_DBS_SQL_SYNTAX, Connection);
        reader = Command.ExecuteReader();
        while (reader.Read())
        {
          var retSchema = reader.GetString(0);
          if (retSchema != TEMP_SCHEMA_NAME)
            continue;
          success = false;
          reader.Close();
          break;
        }

        Assert.True(success, string.Format(SCHEMA_NOT_DELETED, TEMP_SCHEMA_NAME));
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
    /// Test to add data and use all of the features of Collection.find.
    /// </summary>
    [Fact]
    public void FindComplete()
    {
      OpenConnection();

      try
      {
        InitXecutor();
        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, USERS_COLLECTION_NAME, X_TEST_SCHEMA_NAME), Connection);

        var result = Command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, USERS_COLLECTION_NAME));

        ExecuteQuery(CollectionTestProps.GetSchemaXTest);
        ExecuteQuery(CollectionTestProps.GetCollectionXTestMovies);

        // Find complex
        ExecuteQuery(FIND_MOVIES_COMPLEX_QUERY1);
        var selectResult = ExecuteSingleStatement(FIND_MOVIES_COMPLEX_QUERY3);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == MOVIES_RATING_R_COUNT, DATA_NOT_MATCH);

        ExecuteQuery(FIND_MOVIES_COMPLEX_QUERY2);
        selectResult = ExecuteSingleStatement(FIND_MOVIES_COMPLEX_QUERY4);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 10, DATA_NOT_MATCH);

        // Find bound array
        object foundTitle = null;
        var singleResult = ExecuteSingleStatement(FIND_MOVIES_BOUND_ARRAY).FirstOrDefault();
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        if (singleResult != null)
        {
          singleResult.TryGetValue("title", out foundTitle);
        }

        Assert.True(foundTitle != null && foundTitle.ToString().Equals("ANNIE IDENTITY", StringComparison.InvariantCultureIgnoreCase), DATA_NOT_MATCH);
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
    /// Test to Modify data from a collection.
    /// </summary>
    [Fact]
    public void Modify()
    {
      OpenConnection();

      try
      {
        InitXecutor();
        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, USERS_COLLECTION_NAME, X_TEST_SCHEMA_NAME), Connection);

        var result = Command.ExecuteScalar();
        int count;
        object foundValue = null;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, USERS_COLLECTION_NAME));

        ExecuteQuery(CollectionTestProps.GetSchemaXTest);
        ExecuteQuery(CollectionTestProps.GetCollectionXTextUser);
        ExecuteQuery(CollectionTestProps.AddSingleUser1);
        ExecuteQuery(CollectionTestProps.AddSingleUser2);
        ExecuteQuery(CollectionTestProps.AddMultipleUsersSingleAdd);
        ExecuteQuery(CollectionTestProps.AddMultipleUsersMultipleAdd);

        // Modify Set
        ExecuteQuery(CollectionTestProps.IncludeMysqlx);
        ExecuteQuery(MODIFY_SET_USER);
        var selectResult = ExecuteSingleStatement(FIND_MODIFIED_USER);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        var docResult = selectResult.FirstOrDefault();
        if (docResult != null)
        {

          docResult.TryGetValue("status", out foundValue);
        }

        Assert.True(foundValue != null && foundValue.ToString().Equals("inactive", StringComparison.InvariantCultureIgnoreCase), DATA_NOT_MATCH);

        // Modify Set binding array
        ExecuteQuery(MODIFY_SET_BINDING_ARRAY_USER);
        selectResult = ExecuteSingleStatement(FIND_MODIFIED_USER);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        docResult = selectResult.FirstOrDefault();
        List<object> foundRatingList = null;
        if (docResult != null)
        {
          docResult.TryGetValue("ratings", out foundValue);
          foundRatingList = foundValue as List<object>;
        }

        string cleanRatingArrayString = MOVIES_RATING_ARRAY_VALUE.Replace("'", string.Empty).Replace(" ", string.Empty).Trim('[', ']');
        var clearRatingArray = cleanRatingArrayString.Split(',');
        Assert.True(foundRatingList != null && foundRatingList.Select(o => o.ToString()).SequenceEqual(clearRatingArray), DATA_NOT_MATCH);

        // Modify unset single key
        ExecuteQuery(MODIFY_UNSET_USER);
        selectResult = ExecuteSingleStatement(FIND_MODIFIED_USER);
        docResult = selectResult != null ? selectResult.FirstOrDefault() : null;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(docResult != null && !docResult.ContainsKey("age"), DATA_NOT_MATCH);

        // Modify unset list of keys
        ExecuteQuery(MODIFY_UNSET_LIST_USER);
        selectResult = ExecuteSingleStatement(FIND_MODIFIED_USER);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        docResult = selectResult.FirstOrDefault();
        Assert.True(docResult != null && !docResult.ContainsKey("status") && !docResult.ContainsKey("ratings"), DATA_NOT_MATCH);

        // Modify merge
        ExecuteQuery(CollectionTestProps.ModifyMergeUser);
        selectResult = ExecuteSingleStatement(FIND_MODIFIED_USER);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        docResult = selectResult.FirstOrDefault();
        if (docResult != null)
        {
          docResult.TryGetValue("status", out foundValue);
          object foundValue2;
          docResult.TryGetValue("ratings", out foundValue2);
          foundRatingList = foundValue2 as List<object>;
        }

        Assert.True(foundValue != null && foundValue.ToString().Equals("inactive", StringComparison.InvariantCultureIgnoreCase), DATA_NOT_MATCH);
        Assert.True(foundRatingList != null && foundRatingList.Select(o => o.ToString()).SequenceEqual(clearRatingArray), DATA_NOT_MATCH);

        // Modify array append
        ExecuteQuery(CollectionTestProps.ModifyArrayAppendUser);
        selectResult = ExecuteSingleStatement(FIND_MODIFIED_USER);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        docResult = selectResult.FirstOrDefault();
        if (docResult != null)
        {
          docResult.TryGetValue("ratings", out foundValue);
          foundRatingList = foundValue as List<object>;
        }

        Assert.True(foundRatingList != null && foundRatingList.Select(o => o.ToString()).Contains("PG-13"), DATA_NOT_MATCH);

        // Modify array insert
        ExecuteQuery(CollectionTestProps.ModifyArrayInsertUser);
        selectResult = ExecuteSingleStatement(FIND_MODIFIED_USER);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        docResult = selectResult.FirstOrDefault();
        if (docResult != null)
        {
          docResult.TryGetValue("ratings", out foundValue);
          foundRatingList = foundValue as List<object>;
        }

        Assert.True(foundRatingList != null && foundRatingList.Select(o => o.ToString()).ToList()[2].Equals("G", StringComparison.InvariantCultureIgnoreCase), DATA_NOT_MATCH);

        // Modify array delete
        ExecuteQuery(CollectionTestProps.ModifyArrayDeleteUser);
        selectResult = ExecuteSingleStatement(FIND_MODIFIED_USER);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        docResult = selectResult.FirstOrDefault();
        if (docResult != null)
        {
          docResult.TryGetValue("ratings", out foundValue);
          foundRatingList = foundValue as List<object>;
        }

        Assert.True(foundRatingList != null && !foundRatingList.Select(o => o.ToString()).ToList()[2].Equals("G", StringComparison.InvariantCultureIgnoreCase), DATA_NOT_MATCH);

        // Modify sort
        ExecuteQuery(MODIFY_SORT_USER);
        selectResult = ExecuteSingleStatement(FIND_MODIFIED_J_USERS);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 2, DATA_NOT_MATCH);
      }
      finally
      {
        ExecuteQuery(REVERT_ADDED_USERS);
        if (Command != null)
        {
          Command.Dispose();
        }

        CloseConnection();
        DisposeXecutor();
      }
    }

    /// <summary>
    /// Test to Add and Find data from a collection.
    /// </summary>
    [Fact]
    public void Remove()
    {
      OpenConnection();

      try
      {
        InitXecutor();
        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, USERS_COLLECTION_NAME, X_TEST_SCHEMA_NAME), Connection);

        var result = Command.ExecuteScalar();
        int count;
        int usersCount = USERS_COUNT;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, USERS_COLLECTION_NAME));

        ExecuteQuery(CollectionTestProps.GetSchemaXTest);
        ExecuteQuery(CollectionTestProps.GetCollectionXTextUser);
        ExecuteQuery(CollectionTestProps.AddSingleUser1);
        usersCount++;
        ExecuteQuery(CollectionTestProps.AddSingleUser2);
        usersCount++;
        ExecuteQuery(CollectionTestProps.AddMultipleUsersSingleAdd);
        usersCount += 3;
        ExecuteQuery(CollectionTestProps.AddMultipleUsersMultipleAdd);
        usersCount += 3;

        ExecuteQuery(REMOVE_USER);
        usersCount--;
        var selectResult = ExecuteSingleStatement(FIND_ALL_DOCUMENTS_IN_COLLECTION);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == usersCount, DATA_NOT_MATCH);

        ExecuteQuery(REMOVE_SORT_USER);
        usersCount -= 2;
        selectResult = ExecuteSingleStatement(FIND_ALL_DOCUMENTS_IN_COLLECTION);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == usersCount, DATA_NOT_MATCH);
        selectResult = ExecuteSingleStatement(FIND_REMOVED_USERS);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 0, DATA_NOT_MATCH);
      }
      finally
      {
        ExecuteQuery(REVERT_ADDED_USERS);
        if (Command != null)
        {
          Command.Dispose();
        }

        CloseConnection();
        DisposeXecutor();
      }
    }

    /// <summary>
    /// Test to validate whether the active session is active and can be parsed.
    /// </summary>
    [Fact]
    public void TestSessions()
    {
      OpenConnection();

      try
      {
        InitXecutor();

        // Validate session is open
        var sessionIsOpen = ExecuteQuery(CollectionTestProps.IsSessionOpen);
        bool result;
        Assert.True(sessionIsOpen != null && bool.TryParse(sessionIsOpen.ToString(), out result));

        // Parse Uri of active session
        var shellParseSessionResult = ExecuteQuery(CollectionTestProps.ShellParseUriFromSessionUri);
        Assert.True(shellParseSessionResult != null && shellParseSessionResult.ToString().Contains("dbUser"));
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
