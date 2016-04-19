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
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using MySql.VisualStudio.Tests.MySqlX.Base;
using MySqlX;
using MySqlX.Shell;
using Xunit;

namespace MySql.VisualStudio.Tests.MySqlX
{
  /// <summary>
  /// Python tests related to collections using the XShell directly.
  /// </summary>
  public class PyCollectionXShellTests : BaseCollectionTests, IUseFixture<SetUpXShell>
  {
    #region Fields

    /// <summary>
    /// Object to access and execute commands to the current database connection through the mysqlx protocol
    /// </summary>
    private MySqlShellClient _shellClient;

    #endregion Fields

    /// <summary>
    /// Test to Add and Find data from a collection using the <see cref="ShellClient"/> direclty.
    /// </summary>
    //[Fact]
    public void AddFind()
    {
      OpenConnection();

      try
      {
        InitXShell();
        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, SAKILA_X_USERS_COLLECTION, SAKILA_X_SCHEMA_NAME), Connection);

        var result = Command.ExecuteScalar();
        int count;
        int usersCount = USERS_COUNT;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(SCHEMA_NOT_FOUND, SAKILA_X_SCHEMA_NAME));

        _shellClient.Execute(GetSchemaSakilaX);
        _shellClient.Execute(GetCollectionSakilaXUser);

        //Test single add
        _shellClient.Execute(PYTHON_ADD_SINGLE_USER1);
        var selectResult = _shellClient.Execute(FIND_ALL_DOCUMENTS_IN_COLLECTION) as DocResult;
        usersCount += 1;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == usersCount, DATA_NOT_MATCH);

        //Test single add again
        _shellClient.Execute(PYTHON_ADD_SINGLE_USER2);
        selectResult = _shellClient.Execute(FIND_ALL_DOCUMENTS_IN_COLLECTION) as DocResult;
        usersCount += 1;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == usersCount, DATA_NOT_MATCH);

        //Test multiple documents add statement
        _shellClient.Execute(PYTHON_ADD_MULTIPLE_USERS_SINGLE_ADD);
        selectResult = _shellClient.Execute(FIND_ALL_DOCUMENTS_IN_COLLECTION) as DocResult;
        usersCount += 3;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == usersCount, DATA_NOT_MATCH);

        //Test multiple add statements with single documents
        _shellClient.Execute(PYTHON_ADD_MULTIPLE_USERS_MULTIPLE_ADD);
        selectResult = _shellClient.Execute(FIND_ALL_DOCUMENTS_IN_COLLECTION) as DocResult;
        usersCount += 3;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == usersCount, DATA_NOT_MATCH);

        selectResult = _shellClient.Execute(FIND_SPECIFIC_USER_TEST) as DocResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == 1, DATA_NOT_MATCH);

        // Remove back the added users and test
        _shellClient.Execute(REVERT_ADDED_USERS);
        selectResult = _shellClient.Execute(FIND_ALL_DOCUMENTS_IN_COLLECTION) as DocResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == USERS_COUNT, DATA_NOT_MATCH);
      }
      finally
      {
        Command?.Dispose();
        CloseConnection();
      }
    }

    /// <summary>
    /// Test to Insert, Update and Delete record from a table using the <see cref="ShellClient"/> direclty, executing the commands in multiple lines and in a single script
    /// </summary>
    [Fact]
    public void AllTests()
    {
      OpenConnection();
      MySqlDataReader reader = null;

      try
      {
        InitXShell();

        #region CreateAndDropSchema

        // Create Schema test
        _shellClient.Execute(DropSchemaTestIfExists);
        _shellClient.Execute(CreateSchemaTest);
        Command = new MySqlCommand(SHOW_DBS_SQL_SYNTAX, Connection);
        reader = Command.ExecuteReader();
        bool success = false;
        while (reader.Read())
        {
          var retSchema = reader.GetString(0);
          if (retSchema != TEST_SCHEMA_NAME)
            continue;
          success = true;
          reader.Close();
          break;
        }

        Assert.True(success, string.Format(SCHEMA_NOT_FOUND, TEST_SCHEMA_NAME));

        #region CreateAndDropCollection

        // Create Collection test
        _shellClient.Execute(CreateCollectionTest);
        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, TEST_COLLECTION_NAME, TEST_SCHEMA_NAME), Connection);
        var result = Command.ExecuteScalar();
        int count;
        int usersCount = USERS_COUNT;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(COLLECTION_NOT_FOUND, TEST_COLLECTION_NAME));

        // Comments test
        _shellClient.Execute(PYTHON_COMMENT_SINGLE_LINE_1);
        _shellClient.Execute(PYTHON_COMMENT_SINGLE_LINE_2);

        // Delete Collection test
        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, TEST_COLLECTION_NAME, TEST_SCHEMA_NAME), Connection);
        _shellClient.Execute(DropCollectionTest);
        result = Command.ExecuteScalar();
        int.TryParse(result.ToString(), out count);
        Assert.True(count == 0, string.Format(COLLECTION_NOT_DELETED, TEST_COLLECTION_NAME));

        #endregion CreateAndDropCollection

        // Drop Schema test
        _shellClient.Execute(DropSchemaTest);
        Command = new MySqlCommand(SHOW_DBS_SQL_SYNTAX, Connection);
        reader = Command.ExecuteReader();
        success = true;
        while (reader.Read())
        {
          var retSchema = reader.GetString(0);
          if (retSchema != TEST_SCHEMA_NAME)
            continue;
          success = false;
          reader.Close();
          break;
        }

        Assert.True(success, string.Format(SCHEMA_NOT_DELETED, TEST_SCHEMA_NAME));

        #endregion CreateAndDropSchema
        #region AddFind
        // Single Inserts Test
        _shellClient.Execute(GetSchemaSakilaX);
        _shellClient.Execute(GetCollectionSakilaXUser);

        // Test single add
        _shellClient.Execute(PYTHON_ADD_SINGLE_USER1);
        var selectResult = _shellClient.Execute(FIND_ALL_DOCUMENTS_IN_COLLECTION) as DocResult;
        usersCount += 1;
        object foundValue = null;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == usersCount, DATA_NOT_MATCH);

        // Test single add again
        _shellClient.Execute(PYTHON_ADD_SINGLE_USER2);
        selectResult = _shellClient.Execute(FIND_ALL_DOCUMENTS_IN_COLLECTION) as DocResult;
        usersCount += 1;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == usersCount, DATA_NOT_MATCH);

        // Test multiple documents add statement
        _shellClient.Execute(PYTHON_ADD_MULTIPLE_USERS_SINGLE_ADD);
        selectResult = _shellClient.Execute(FIND_ALL_DOCUMENTS_IN_COLLECTION) as DocResult;
        usersCount += 3;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == usersCount, DATA_NOT_MATCH);

        // Test multiple add statements with single documents
        _shellClient.Execute(PYTHON_ADD_MULTIPLE_USERS_MULTIPLE_ADD);
        selectResult = _shellClient.Execute(FIND_ALL_DOCUMENTS_IN_COLLECTION) as DocResult;
        usersCount += 3;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == usersCount, DATA_NOT_MATCH);

        selectResult = _shellClient.Execute(FIND_SPECIFIC_USER_TEST) as DocResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == 1, DATA_NOT_MATCH);

        #endregion AddFind
        #region Modify

        // Modify Set
        _shellClient.Execute(MODIFY_SET_USER);
        selectResult = _shellClient.Execute(FIND_MODIFIED_USER) as DocResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        var docResult = selectResult?.FetchOne();
        if (docResult != null)
        {

          docResult.TryGetValue("status", out foundValue);
        }

        Assert.True(foundValue != null && foundValue.ToString().Equals("inactive", StringComparison.InvariantCultureIgnoreCase), DATA_NOT_MATCH);

        // Modify Set binding array
        _shellClient.Execute(MODIFY_SET_BINDING_ARRAY_USER);
        selectResult = _shellClient.Execute(FIND_MODIFIED_USER) as DocResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        docResult = selectResult?.FetchOne();
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
        _shellClient.Execute(MODIFY_UNSET_USER);
        selectResult = _shellClient.Execute(FIND_MODIFIED_USER) as DocResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && !selectResult.FetchOne().ContainsKey("age"), DATA_NOT_MATCH);

        // Modify unset list of keys
        _shellClient.Execute(MODIFY_UNSET_LIST_USER);
        selectResult = _shellClient.Execute(FIND_MODIFIED_USER) as DocResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        docResult = selectResult?.FetchOne();
        Assert.True(docResult != null && !docResult.ContainsKey("status") && !docResult.ContainsKey("ratings"), DATA_NOT_MATCH);

        // Modify merge
        _shellClient.Execute(PYTHON_MODIFY_MERGE_USER);
        selectResult = _shellClient.Execute(FIND_MODIFIED_USER) as DocResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        docResult = selectResult?.FetchOne();
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
        _shellClient.Execute(MODIFY_ARRAY_APPEND_USER);
        selectResult = _shellClient.Execute(FIND_MODIFIED_USER) as DocResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        docResult = selectResult?.FetchOne();
        if (docResult != null)
        {
          docResult.TryGetValue("ratings", out foundValue);
          foundRatingList = foundValue as List<object>;
        }

        Assert.True(foundRatingList != null && foundRatingList.Select(o => o.ToString()).Contains("PG-13"), DATA_NOT_MATCH);

        // Modify array insert
        _shellClient.Execute(MODIFY_ARRAY_INSERT_USER);
        selectResult = _shellClient.Execute(FIND_MODIFIED_USER) as DocResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        docResult = selectResult?.FetchOne();
        if (docResult != null)
        {
          docResult.TryGetValue("ratings", out foundValue);
          foundRatingList = foundValue as List<object>;
        }

        Assert.True(foundRatingList != null && foundRatingList.Select(o => o.ToString()).ToList()[2].Equals("G", StringComparison.InvariantCultureIgnoreCase), DATA_NOT_MATCH);

        // Modify array delete
        _shellClient.Execute(MODIFY_ARRAY_DELETE_USER);
        selectResult = _shellClient.Execute(FIND_MODIFIED_USER) as DocResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        docResult = selectResult?.FetchOne();
        if (docResult != null)
        {
          docResult.TryGetValue("ratings", out foundValue);
          foundRatingList = foundValue as List<object>;
        }

        Assert.True(foundRatingList != null && !foundRatingList.Select(o => o.ToString()).ToList()[2].Equals("G", StringComparison.InvariantCultureIgnoreCase), DATA_NOT_MATCH);

        // Modify sort
        _shellClient.Execute(MODIFY_SORT_USER);
        selectResult = _shellClient.Execute(FIND_MODIFIED_J_USERS) as DocResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == 2, DATA_NOT_MATCH);

        #endregion Modify
        #region Remove

        // Remove test
        _shellClient.Execute(REMOVE_USER);
        selectResult = _shellClient.Execute(FIND_ALL_DOCUMENTS_IN_COLLECTION) as DocResult;
        usersCount -= 1;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == usersCount, DATA_NOT_MATCH);

        // Remove sort test
        _shellClient.Execute(REMOVE_SORT_USER);
        usersCount -= 2;
        selectResult = _shellClient.Execute(FIND_ALL_DOCUMENTS_IN_COLLECTION) as DocResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == usersCount, DATA_NOT_MATCH);
        selectResult = _shellClient.Execute(FIND_REMOVED_USERS) as DocResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == 0, DATA_NOT_MATCH);

        // Remove back the added users and test
        _shellClient.Execute(REVERT_ADDED_USERS);
        selectResult = _shellClient.Execute(FIND_ALL_DOCUMENTS_IN_COLLECTION) as DocResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == USERS_COUNT, DATA_NOT_MATCH);

        #endregion Remove
        #region FindComplete

        // Find complex
        _shellClient.Execute(GetCollectionSakilaXMovies);
        _shellClient.Execute(FIND_MOVIES_COMPLEX_QUERY1);
        selectResult = _shellClient.Execute(FIND_MOVIES_COMPLEX_QUERY3) as DocResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == MOVIES_RATING_R_COUNT, DATA_NOT_MATCH);

        _shellClient.Execute(FIND_MOVIES_COMPLEX_QUERY2);
        selectResult = _shellClient.Execute(FIND_MOVIES_COMPLEX_QUERY4) as DocResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == 10, DATA_NOT_MATCH);

        // Find bound array
        object foundTitle = null;
        selectResult = _shellClient.Execute(FIND_MOVIES_BOUND_ARRAY) as DocResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        docResult = selectResult?.FetchOne();
        if (docResult != null)
        {
          docResult.TryGetValue("title", out foundTitle);
        }

        Assert.True(foundTitle != null && foundTitle.ToString().Equals("ANNIE IDENTITY", StringComparison.InvariantCultureIgnoreCase), DATA_NOT_MATCH);

        #endregion FindComplete
        #region CreateAndDropIndex

        _shellClient.Execute(GetSchemaSakilaX);
        _shellClient.Execute(GetCollectionSakilaXMovies);

        // Add non-unique index
        _shellClient.Execute(CREATE_NON_UNIQUE_INDEX_MOVIES);
        Command = new MySqlCommand(string.Format(SEARCH_INDEX_SQL_SYNTAX, SAKILA_X_SCHEMA_NAME, SAKILA_X_MOVIES_COLLECTION, MOVIES_NON_UNIQUE_INDEX_NAME), Connection);
        result = Command.ExecuteScalar();
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(INDEX_NOT_FOUND, MOVIES_NON_UNIQUE_INDEX_NAME));

        // Add unique index
        _shellClient.Execute(PYTHON_INCLUDE_MYSQLX);
        _shellClient.Execute(CREATE_UNIQUE_INDEX_MOVIES);
        Command = new MySqlCommand(string.Format(SEARCH_INDEX_SQL_SYNTAX, SAKILA_X_SCHEMA_NAME, SAKILA_X_MOVIES_COLLECTION, MOVIES_UNIQUE_INDEX_NAME), Connection);
        result = Command.ExecuteScalar();
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(INDEX_NOT_FOUND, MOVIES_UNIQUE_INDEX_NAME));

        // Test data uniqueness
        _shellClient.Execute(PYTHON_ADD_DUPLICATE_MOVIE);
        selectResult = _shellClient.Execute(FIND_DUPLICATE_MOVIE_TITLE) as DocResult;
        int duplicateMovieCount = selectResult?.FetchAll().Count ?? 0;
        Assert.True(duplicateMovieCount == 1, DATA_NOT_UNIQUE);
        if (duplicateMovieCount > 1)
        {
          _shellClient.Execute(REMOVE_DUPLICATE_MOVIE);
        }

        // Drop non-unique index
        _shellClient.Execute(DROP_NON_UNIQUE_INDEX_MOVIES);
        Command = new MySqlCommand(string.Format(SEARCH_INDEX_SQL_SYNTAX, SAKILA_X_SCHEMA_NAME, SAKILA_X_MOVIES_COLLECTION, MOVIES_NON_UNIQUE_INDEX_NAME), Connection);
        result = Command.ExecuteScalar();
        int.TryParse(result.ToString(), out count);
        Assert.True(count == 0, string.Format(INDEX_NOT_FOUND, MOVIES_NON_UNIQUE_INDEX_NAME));

        // Drop unique index
        _shellClient.Execute(DROP_UNIQUE_INDEX_MOVIES);
        Command = new MySqlCommand(string.Format(SEARCH_INDEX_SQL_SYNTAX, SAKILA_X_SCHEMA_NAME, SAKILA_X_MOVIES_COLLECTION, MOVIES_UNIQUE_INDEX_NAME), Connection);
        result = Command.ExecuteScalar();
        int.TryParse(result.ToString(), out count);
        Assert.True(count == 0, string.Format(INDEX_NOT_FOUND, MOVIES_UNIQUE_INDEX_NAME));

        #endregion CreateAndDropIndex
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
    /// Test to create and drop a Collection using the <see cref="ShellClient"/> direclty.
    /// </summary>
    //[Fact]
    public void CreateAndDropCollection()
    {
      OpenConnection();

      try
      {
        InitXShell();
        _shellClient.Execute(DropSchemaTestIfExists);
        _shellClient.Execute(CreateSchemaTest);
        _shellClient.Execute(CreateCollectionTest);
        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, TEST_COLLECTION_NAME, TEST_SCHEMA_NAME), Connection);
        var result = Command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(COLLECTION_NOT_FOUND, TEST_COLLECTION_NAME));

        _shellClient.Execute(DropCollectionTest);
        result = Command.ExecuteScalar();
        int.TryParse(result.ToString(), out count);
        Assert.True(count == 0, string.Format(COLLECTION_NOT_DELETED, TEST_COLLECTION_NAME));
      }
      finally
      {
        Command?.Dispose();
        CloseConnection();
      }
    }

    /// <summary>
    /// Test to create and drop unique and non-unique indexes using the <see cref="ShellClient"/> direclty.
    /// </summary>
    //[Fact]
    public void CreateAndDropIndex()
    {
      OpenConnection();
      int duplicateMovieCount = 0;

      try
      {
        InitXShell();

        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, SAKILA_X_MOVIES_COLLECTION, SAKILA_X_SCHEMA_NAME), Connection);
        var result = Command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(SCHEMA_NOT_FOUND, SAKILA_X_SCHEMA_NAME));

        _shellClient.Execute(GetSchemaSakilaX);
        _shellClient.Execute(GetCollectionSakilaXMovies);

        // Add non-unique index
        _shellClient.Execute(CREATE_NON_UNIQUE_INDEX_MOVIES);
        Command = new MySqlCommand(string.Format(SEARCH_INDEX_SQL_SYNTAX, SAKILA_X_SCHEMA_NAME, SAKILA_X_MOVIES_COLLECTION, MOVIES_NON_UNIQUE_INDEX_NAME), Connection);
        result = Command.ExecuteScalar();
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(INDEX_NOT_FOUND, MOVIES_NON_UNIQUE_INDEX_NAME));

        // Add unique index
        _shellClient.Execute(PYTHON_INCLUDE_MYSQLX);
        _shellClient.Execute(CREATE_UNIQUE_INDEX_MOVIES);
        Command = new MySqlCommand(string.Format(SEARCH_INDEX_SQL_SYNTAX, SAKILA_X_SCHEMA_NAME, SAKILA_X_MOVIES_COLLECTION, MOVIES_UNIQUE_INDEX_NAME), Connection);
        result = Command.ExecuteScalar();
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(INDEX_NOT_FOUND, MOVIES_UNIQUE_INDEX_NAME));

        // Test data uniqueness
        _shellClient.Execute(PYTHON_ADD_DUPLICATE_MOVIE);
        var selectResult = _shellClient.Execute(FIND_DUPLICATE_MOVIE_TITLE) as DocResult;
        duplicateMovieCount = selectResult?.FetchAll().Count ?? 0;
        Assert.True(duplicateMovieCount == 1, DATA_NOT_UNIQUE);

        // Drop non-unique index
        _shellClient.Execute(DROP_NON_UNIQUE_INDEX_MOVIES);
        Command = new MySqlCommand(string.Format(SEARCH_INDEX_SQL_SYNTAX, SAKILA_X_SCHEMA_NAME, SAKILA_X_MOVIES_COLLECTION, MOVIES_NON_UNIQUE_INDEX_NAME), Connection);
        result = Command.ExecuteScalar();
        int.TryParse(result.ToString(), out count);
        Assert.True(count == 0, string.Format(INDEX_NOT_FOUND, MOVIES_NON_UNIQUE_INDEX_NAME));

        // Drop unique index
        _shellClient.Execute(DROP_UNIQUE_INDEX_MOVIES);
        Command = new MySqlCommand(string.Format(SEARCH_INDEX_SQL_SYNTAX, SAKILA_X_SCHEMA_NAME, SAKILA_X_MOVIES_COLLECTION, MOVIES_UNIQUE_INDEX_NAME), Connection);
        result = Command.ExecuteScalar();
        int.TryParse(result.ToString(), out count);
        Assert.True(count == 0, string.Format(INDEX_NOT_FOUND, MOVIES_UNIQUE_INDEX_NAME));
      }
      finally
      {
        // Remove duplicate data in case test failed
        if (duplicateMovieCount > 1)
        {
          _shellClient.Execute(REMOVE_DUPLICATE_MOVIE);
        }

        Command?.Dispose();
        CloseConnection();
      }
    }

    /// <summary>
    /// Test to create and drop a Schema using the <see cref="ShellClient"/> direclty.
    /// </summary>
    //[Fact]
    public void CreateAndDropSchema()
    {
      OpenConnection();
      MySqlDataReader reader = null;

      try
      {
        InitXShell();
        _shellClient.Execute(DropSchemaTestIfExists);
        _shellClient.Execute(CreateSchemaTest);
        Command = new MySqlCommand(SHOW_DBS_SQL_SYNTAX, Connection);
        reader = Command.ExecuteReader();
        bool success = false;
        while (reader.Read())
        {
          var retSchema = reader.GetString(0);
          if (retSchema != TEST_SCHEMA_NAME)
            continue;
          success = true;
          reader.Close();
          break;
        }

        Assert.True(success, string.Format(SCHEMA_NOT_FOUND, TEST_SCHEMA_NAME));

        _shellClient.Execute(DropSchemaTest);
        Command = new MySqlCommand(SHOW_DBS_SQL_SYNTAX, Connection);
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

        Assert.True(success, string.Format(SCHEMA_NOT_DELETED, TEST_SCHEMA_NAME));
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
    /// Test to add data and use all of the features of Collection.find.
    /// </summary>
    //[Fact]
    public void FindComplete()
    {
      OpenConnection();

      try
      {
        InitXShell();
        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, SAKILA_X_USERS_COLLECTION, SAKILA_X_SCHEMA_NAME), Connection);

        var result = Command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(SCHEMA_NOT_FOUND, TEST_SCHEMA_NAME));

        _shellClient.Execute(GetSchemaSakilaX);
        _shellClient.Execute(GetCollectionSakilaXMovies);

        // Find complex
        _shellClient.Execute(FIND_MOVIES_COMPLEX_QUERY1);
        var selectResult = _shellClient.Execute(FIND_MOVIES_COMPLEX_QUERY3) as DocResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == MOVIES_RATING_R_COUNT, DATA_NOT_MATCH);

        _shellClient.Execute(FIND_MOVIES_COMPLEX_QUERY2);
        selectResult = _shellClient.Execute(FIND_MOVIES_COMPLEX_QUERY4) as DocResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == 10, DATA_NOT_MATCH);

        // Find bound array
        object foundTitle = null;
        selectResult = _shellClient.Execute(FIND_MOVIES_BOUND_ARRAY) as DocResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        var docResult = selectResult?.FetchOne();
        if (docResult != null)
        {
          docResult.TryGetValue("title", out foundTitle);
        }

        Assert.True(foundTitle != null && foundTitle.ToString().Equals("ANNIE IDENTITY", StringComparison.InvariantCultureIgnoreCase), DATA_NOT_MATCH);
      }
      finally
      {
        Command?.Dispose();
        CloseConnection();
      }
    }

    /// <summary>
    /// Test to Modify data from a collection using the <see cref="ShellClient"/> direclty.
    /// </summary>
    //[Fact]
    public void Modify()
    {
      OpenConnection();

      try
      {
        InitXShell();
        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, SAKILA_X_USERS_COLLECTION, SAKILA_X_SCHEMA_NAME), Connection);

        var result = Command.ExecuteScalar();
        int count;
        object foundValue = null;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(SCHEMA_NOT_FOUND, SAKILA_X_SCHEMA_NAME));

        _shellClient.Execute(GetSchemaSakilaX);
        _shellClient.Execute(GetCollectionSakilaXUser);
        _shellClient.Execute(JAVASCRIPT_ADD_SINGLE_USER1);
        _shellClient.Execute(JAVASCRIPT_ADD_SINGLE_USER2);
        _shellClient.Execute(JAVASCRIPT_ADD_MULTIPLE_USERS_SINGLE_ADD);
        _shellClient.Execute(JAVASCRIPT_ADD_MULTIPLE_USERS_MULTIPLE_ADD);

        // Modify Set
        _shellClient.Execute(MODIFY_SET_USER);
        var selectResult = _shellClient.Execute(FIND_MODIFIED_USER) as DocResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        var docResult = selectResult?.FetchOne();
        if (docResult != null)
        {

          docResult.TryGetValue("status", out foundValue);
        }

        Assert.True(foundValue != null && foundValue.ToString().Equals("inactive", StringComparison.InvariantCultureIgnoreCase), DATA_NOT_MATCH);

        // Modify Set binding array
        _shellClient.Execute(MODIFY_SET_BINDING_ARRAY_USER);
        selectResult = _shellClient.Execute(FIND_MODIFIED_USER) as DocResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        docResult = selectResult?.FetchOne();
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
        _shellClient.Execute(MODIFY_UNSET_USER);
        selectResult = _shellClient.Execute(FIND_MODIFIED_USER) as DocResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && !selectResult.FetchOne().ContainsKey("age"), DATA_NOT_MATCH);

        // Modify unset list of keys
        _shellClient.Execute(MODIFY_UNSET_LIST_USER);
        selectResult = _shellClient.Execute(FIND_MODIFIED_USER) as DocResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        docResult = selectResult?.FetchOne();
        Assert.True(docResult != null && !docResult.ContainsKey("status") && !docResult.ContainsKey("ratings"), DATA_NOT_MATCH);

        // Modify merge
        _shellClient.Execute(PYTHON_MODIFY_MERGE_USER);
        selectResult = _shellClient.Execute(FIND_MODIFIED_USER) as DocResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        docResult = selectResult?.FetchOne();
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
        _shellClient.Execute(MODIFY_ARRAY_APPEND_USER);
        selectResult = _shellClient.Execute(FIND_MODIFIED_USER) as DocResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        docResult = selectResult?.FetchOne();
        if (docResult != null)
        {
          docResult.TryGetValue("ratings", out foundValue);
          foundRatingList = foundValue as List<object>;
        }

        Assert.True(foundRatingList != null && foundRatingList.Select(o => o.ToString()).Contains("PG-13"), DATA_NOT_MATCH);

        // Modify array insert
        _shellClient.Execute(MODIFY_ARRAY_INSERT_USER);
        selectResult = _shellClient.Execute(FIND_MODIFIED_USER) as DocResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        docResult = selectResult?.FetchOne();
        if (docResult != null)
        {
          docResult.TryGetValue("ratings", out foundValue);
          foundRatingList = foundValue as List<object>;
        }

        Assert.True(foundRatingList != null && foundRatingList.Select(o => o.ToString()).ToList()[2].Equals("G", StringComparison.InvariantCultureIgnoreCase), DATA_NOT_MATCH);

        // Modify array delete
        _shellClient.Execute(MODIFY_ARRAY_DELETE_USER);
        selectResult = _shellClient.Execute(FIND_MODIFIED_USER) as DocResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        docResult = selectResult?.FetchOne();
        if (docResult != null)
        {
          docResult.TryGetValue("ratings", out foundValue);
          foundRatingList = foundValue as List<object>;
        }

        Assert.True(foundRatingList != null && !foundRatingList.Select(o => o.ToString()).ToList()[2].Equals("G", StringComparison.InvariantCultureIgnoreCase), DATA_NOT_MATCH);

        // Modify sort
        _shellClient.Execute(MODIFY_SORT_USER);
        selectResult = _shellClient.Execute(FIND_MODIFIED_J_USERS) as DocResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == 2, DATA_NOT_MATCH);
      }
      finally
      {
        _shellClient.Execute(REVERT_ADDED_USERS);
        Command?.Dispose();
        CloseConnection();
      }
    }

    /// <summary>
    /// Test to Add and Find data from a collection using the <see cref="ShellClient"/> direclty.
    /// </summary>
    //[Fact]
    public void Remove()
    {
      OpenConnection();

      try
      {
        InitXShell();
        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, SAKILA_X_USERS_COLLECTION, SAKILA_X_SCHEMA_NAME), Connection);

        var result = Command.ExecuteScalar();
        int count;
        int usersCount = USERS_COUNT;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(SCHEMA_NOT_FOUND, SAKILA_X_SCHEMA_NAME));

        _shellClient.Execute(GetSchemaSakilaX);
        _shellClient.Execute(GetCollectionSakilaXUser);
        _shellClient.Execute(JAVASCRIPT_ADD_SINGLE_USER1);
        usersCount++;
        _shellClient.Execute(JAVASCRIPT_ADD_SINGLE_USER2);
        usersCount++;
        _shellClient.Execute(JAVASCRIPT_ADD_MULTIPLE_USERS_SINGLE_ADD);
        usersCount += 3;
        _shellClient.Execute(JAVASCRIPT_ADD_MULTIPLE_USERS_MULTIPLE_ADD);
        usersCount += 3;

        // Remove test
        _shellClient.Execute(REMOVE_USER);
        usersCount--;
        var selectResult = _shellClient.Execute(FIND_ALL_DOCUMENTS_IN_COLLECTION) as DocResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == usersCount, DATA_NOT_MATCH);

        // Remove sort test
        _shellClient.Execute(REMOVE_SORT_USER);
        usersCount -= 2;
        selectResult = _shellClient.Execute(FIND_ALL_DOCUMENTS_IN_COLLECTION) as DocResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == usersCount, DATA_NOT_MATCH);
        selectResult = _shellClient.Execute(FIND_REMOVED_USERS) as DocResult;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.FetchAll().Count == 0, DATA_NOT_MATCH);
      }
      finally
      {
        _shellClient.Execute(REVERT_ADDED_USERS);
        Command?.Dispose();
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
      _shellClient.SwitchMode(Mode.Python);
    }
  }
}
