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
using MySql.Utility.Enums;
using MySql.VisualStudio.Tests.MySqlX.Base;
using Xunit;

namespace MySql.VisualStudio.Tests.MySqlX
{
  public class JsCollectionXProxyTests : BaseCollectionTests
  {
    /// <summary>
    /// Test to Add, Modify, Delete and Find a record from a collection using the <see cref="MyTestXProxy"/>, executing the commands in a single line
    /// </summary>
    [Fact]
    public void AddFind()
    {
      OpenConnection();

      try
      {
        InitXProxy(ScriptLanguageType.JavaScript);
        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, USERS_COLLECTION_NAME, X_TEST_SCHEMA_NAME), Connection);

        var result = Command.ExecuteScalar();
        int count;
        int usersCount = USERS_COUNT;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, USERS_COLLECTION_NAME));

        XProxy.ExecuteScript(GetSchemaXTest, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(GetCollectionXTextUser, ScriptLanguageType.JavaScript);

        //Test single add
        XProxy.ExecuteScript(JAVASCRIPT_ADD_SINGLE_USER1, ScriptLanguageType.JavaScript);
        var selectResult = XProxy.ExecuteScript(FIND_ALL_DOCUMENTS_IN_COLLECTION, ScriptLanguageType.JavaScript);
        usersCount += 1;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == usersCount, DATA_NOT_MATCH);

        //Test single add again
        XProxy.ExecuteScript(JAVASCRIPT_ADD_SINGLE_USER2, ScriptLanguageType.JavaScript);
        selectResult = XProxy.ExecuteScript(FIND_ALL_DOCUMENTS_IN_COLLECTION, ScriptLanguageType.JavaScript);
        usersCount += 1;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == usersCount, DATA_NOT_MATCH);

        //Test multiple documents add statement
        XProxy.ExecuteScript(JAVASCRIPT_ADD_MULTIPLE_USERS_SINGLE_ADD, ScriptLanguageType.JavaScript);
        selectResult = XProxy.ExecuteScript(FIND_ALL_DOCUMENTS_IN_COLLECTION, ScriptLanguageType.JavaScript);
        usersCount += 3;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == usersCount, DATA_NOT_MATCH);

        //Test multiple add statements with single documents
        XProxy.ExecuteScript(JAVASCRIPT_ADD_MULTIPLE_USERS_MULTIPLE_ADD, ScriptLanguageType.JavaScript);
        selectResult = XProxy.ExecuteScript(FIND_ALL_DOCUMENTS_IN_COLLECTION, ScriptLanguageType.JavaScript);
        usersCount += 3;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == usersCount, DATA_NOT_MATCH);

        selectResult = XProxy.ExecuteScript(FIND_SPECIFIC_USER_TEST, ScriptLanguageType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 1, DATA_NOT_MATCH);

        // Remove back the added users and test
        XProxy.ExecuteScript(REVERT_ADDED_USERS, ScriptLanguageType.JavaScript);
        selectResult = XProxy.ExecuteScript(FIND_ALL_DOCUMENTS_IN_COLLECTION, ScriptLanguageType.JavaScript);
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
        DisposeProxy();
      }
    }

    /// <summary>
    /// Test to create and drop a Collection using the <see cref="MyTestXProxy"/>.
    /// </summary>
    [Fact]
    public void CreateAndDropCollection()
    {
      OpenConnection();

      try
      {
        InitXProxy(ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(DropSchemaTempSchemaIfExists, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(CreateSchemaTempSchema, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(CreateCollectionTest, ScriptLanguageType.JavaScript);
        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, TEST_COLLECTION_NAME, TEMP_SCHEMA_NAME), Connection);
        var result = Command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(COLLECTION_NOT_FOUND, TEST_COLLECTION_NAME));

        XProxy.ExecuteScript(DropCollectionTest, ScriptLanguageType.JavaScript);
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
        DisposeProxy();
      }
    }

    /// <summary>
    /// Test to create and drop unique and non-unique indexes using the <see cref="MyTestXProxy"/>.
    /// </summary>
    [Fact]
    public void CreateAndDropIndex()
    {
      OpenConnection();
      int duplicateMovieCount = 0;

      try
      {
        InitXProxy(ScriptLanguageType.JavaScript);

        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, MOVIES_COLLECTION_NAME, X_TEST_SCHEMA_NAME), Connection);
        var result = Command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, MOVIES_COLLECTION_NAME));

        XProxy.ExecuteScript(GetSchemaXTest, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(GetCollectionXTestMovies, ScriptLanguageType.JavaScript);

        // Add non-unique index
        XProxy.ExecuteScript(CREATE_NON_UNIQUE_INDEX_MOVIES, ScriptLanguageType.JavaScript);
        Command = new MySqlCommand(string.Format(SEARCH_INDEX_SQL_SYNTAX, X_TEST_SCHEMA_NAME, MOVIES_COLLECTION_NAME, MOVIES_NON_UNIQUE_INDEX_NAME), Connection);
        result = Command.ExecuteScalar();
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(INDEX_NOT_FOUND, MOVIES_NON_UNIQUE_INDEX_NAME));

        // Add unique index
        XProxy.ExecuteScript(JAVASCRIPT_INCLUDE_MYSQLX, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(CREATE_UNIQUE_INDEX_MOVIES, ScriptLanguageType.JavaScript);
        Command = new MySqlCommand(string.Format(SEARCH_INDEX_SQL_SYNTAX, X_TEST_SCHEMA_NAME, MOVIES_COLLECTION_NAME, MOVIES_UNIQUE_INDEX_NAME), Connection);
        result = Command.ExecuteScalar();
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(INDEX_NOT_FOUND, MOVIES_UNIQUE_INDEX_NAME));

        // Test data uniqueness
        XProxy.ExecuteScript(JAVASCRIPT_ADD_DUPLICATE_MOVIE, ScriptLanguageType.JavaScript);
        var selectResult = XProxy.ExecuteScript(FIND_DUPLICATE_MOVIE_TITLE, ScriptLanguageType.JavaScript);
        duplicateMovieCount = selectResult != null ? selectResult.Count : 0;
        Assert.True(duplicateMovieCount == 1, DATA_NOT_UNIQUE);

        // Drop non-unique index
        XProxy.ExecuteScript(DROP_NON_UNIQUE_INDEX_MOVIES, ScriptLanguageType.JavaScript);
        Command = new MySqlCommand(string.Format(SEARCH_INDEX_SQL_SYNTAX, X_TEST_SCHEMA_NAME, MOVIES_COLLECTION_NAME, MOVIES_NON_UNIQUE_INDEX_NAME), Connection);
        result = Command.ExecuteScalar();
        int.TryParse(result.ToString(), out count);
        Assert.True(count == 0, string.Format(INDEX_NOT_FOUND, MOVIES_NON_UNIQUE_INDEX_NAME));

        // Drop unique index
        XProxy.ExecuteScript(DROP_UNIQUE_INDEX_MOVIES, ScriptLanguageType.JavaScript);
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
          XProxy.ExecuteScript(REMOVE_DUPLICATE_MOVIE, ScriptLanguageType.JavaScript);
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
    /// Test to create and drop a Schema using the <see cref="MyTestXProxy"/>.
    /// </summary>
    [Fact]
    public void CreateAndDropSchema()
    {
      OpenConnection();
      MySqlDataReader reader = null;

      try
      {
        InitXProxy(ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(DropSchemaTempSchemaIfExists, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(CreateSchemaTempSchema, ScriptLanguageType.JavaScript);
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

        XProxy.ExecuteScript(DropSchemaTempSchema, ScriptLanguageType.JavaScript);
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
        DisposeProxy();
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
        InitXProxy(ScriptLanguageType.JavaScript);
        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, USERS_COLLECTION_NAME, X_TEST_SCHEMA_NAME), Connection);

        var result = Command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, USERS_COLLECTION_NAME));

        XProxy.ExecuteScript(GetSchemaXTest, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(GetCollectionXTestMovies, ScriptLanguageType.JavaScript);

        // Find complex
        XProxy.ExecuteScript(FIND_MOVIES_COMPLEX_QUERY1, ScriptLanguageType.JavaScript);
        var selectResult = XProxy.ExecuteScript(FIND_MOVIES_COMPLEX_QUERY3, ScriptLanguageType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == MOVIES_RATING_R_COUNT, DATA_NOT_MATCH);

        XProxy.ExecuteScript(FIND_MOVIES_COMPLEX_QUERY2, ScriptLanguageType.JavaScript);
        selectResult = XProxy.ExecuteScript(FIND_MOVIES_COMPLEX_QUERY4, ScriptLanguageType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 10, DATA_NOT_MATCH);

        // Find bound array
        object foundTitle = null;
        var singleResult = XProxy.ExecuteScript(FIND_MOVIES_BOUND_ARRAY, ScriptLanguageType.JavaScript).FirstOrDefault();
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
        DisposeProxy();
      }
    }

    /// <summary>
    /// Test to Modify data from a collection using the <see cref="MyTestXProxy"/>.
    /// </summary>
    [Fact]
    public void Modify()
    {
      OpenConnection();

      try
      {
        InitXProxy(ScriptLanguageType.JavaScript);
        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, USERS_COLLECTION_NAME, X_TEST_SCHEMA_NAME), Connection);

        var result = Command.ExecuteScalar();
        int count;
        object foundValue = null;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, USERS_COLLECTION_NAME));

        XProxy.ExecuteScript(GetSchemaXTest, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(GetCollectionXTextUser, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(JAVASCRIPT_ADD_SINGLE_USER1, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(JAVASCRIPT_ADD_SINGLE_USER2, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(JAVASCRIPT_ADD_MULTIPLE_USERS_SINGLE_ADD, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(JAVASCRIPT_ADD_MULTIPLE_USERS_MULTIPLE_ADD, ScriptLanguageType.JavaScript);

        // Modify Set
        XProxy.ExecuteScript(JAVASCRIPT_INCLUDE_MYSQLX, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(MODIFY_SET_USER, ScriptLanguageType.JavaScript);
        var selectResult = XProxy.ExecuteScript(FIND_MODIFIED_USER, ScriptLanguageType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        var docResult = selectResult.FirstOrDefault();
        if (docResult != null)
        {

          docResult.TryGetValue("status", out foundValue);
        }

        Assert.True(foundValue != null && foundValue.ToString().Equals("inactive", StringComparison.InvariantCultureIgnoreCase), DATA_NOT_MATCH);

        // Modify Set binding array
        XProxy.ExecuteScript(MODIFY_SET_BINDING_ARRAY_USER, ScriptLanguageType.JavaScript);
        selectResult = XProxy.ExecuteScript(FIND_MODIFIED_USER, ScriptLanguageType.JavaScript);
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
        XProxy.ExecuteScript(MODIFY_UNSET_USER, ScriptLanguageType.JavaScript);
        selectResult = XProxy.ExecuteScript(FIND_MODIFIED_USER, ScriptLanguageType.JavaScript);
        docResult = selectResult != null ? selectResult.FirstOrDefault() : null;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(docResult != null && !docResult.ContainsKey("age"), DATA_NOT_MATCH);

        // Modify unset list of keys
        XProxy.ExecuteScript(MODIFY_UNSET_LIST_USER, ScriptLanguageType.JavaScript);
        selectResult = XProxy.ExecuteScript(FIND_MODIFIED_USER, ScriptLanguageType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        docResult = selectResult.FirstOrDefault();
        Assert.True(docResult != null && !docResult.ContainsKey("status") && !docResult.ContainsKey("ratings"), DATA_NOT_MATCH);

        // Modify merge
        XProxy.ExecuteScript(JAVASCRIPT_MODIFY_MERGE_USER, ScriptLanguageType.JavaScript);
        selectResult = XProxy.ExecuteScript(FIND_MODIFIED_USER, ScriptLanguageType.JavaScript);
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
        XProxy.ExecuteScript(MODIFY_ARRAY_APPEND_USER, ScriptLanguageType.JavaScript);
        selectResult = XProxy.ExecuteScript(FIND_MODIFIED_USER, ScriptLanguageType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        docResult = selectResult.FirstOrDefault();
        if (docResult != null)
        {
          docResult.TryGetValue("ratings", out foundValue);
          foundRatingList = foundValue as List<object>;
        }

        Assert.True(foundRatingList != null && foundRatingList.Select(o => o.ToString()).Contains("PG-13"), DATA_NOT_MATCH);

        // Modify array insert
        XProxy.ExecuteScript(MODIFY_ARRAY_INSERT_USER, ScriptLanguageType.JavaScript);
        selectResult = XProxy.ExecuteScript(FIND_MODIFIED_USER, ScriptLanguageType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        docResult = selectResult.FirstOrDefault();
        if (docResult != null)
        {
          docResult.TryGetValue("ratings", out foundValue);
          foundRatingList = foundValue as List<object>;
        }

        Assert.True(foundRatingList != null && foundRatingList.Select(o => o.ToString()).ToList()[2].Equals("G", StringComparison.InvariantCultureIgnoreCase), DATA_NOT_MATCH);

        // Modify array delete
        XProxy.ExecuteScript(MODIFY_ARRAY_DELETE_USER, ScriptLanguageType.JavaScript);
        selectResult = XProxy.ExecuteScript(FIND_MODIFIED_USER, ScriptLanguageType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        docResult = selectResult.FirstOrDefault();
        if (docResult != null)
        {
          docResult.TryGetValue("ratings", out foundValue);
          foundRatingList = foundValue as List<object>;
        }

        Assert.True(foundRatingList != null && !foundRatingList.Select(o => o.ToString()).ToList()[2].Equals("G", StringComparison.InvariantCultureIgnoreCase), DATA_NOT_MATCH);

        // Modify sort
        XProxy.ExecuteScript(MODIFY_SORT_USER, ScriptLanguageType.JavaScript);
        selectResult = XProxy.ExecuteScript(FIND_MODIFIED_J_USERS, ScriptLanguageType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 2, DATA_NOT_MATCH);
      }
      finally
      {
        XProxy.ExecuteScript(REVERT_ADDED_USERS, ScriptLanguageType.JavaScript);
        if (Command != null)
        {
          Command.Dispose();
        }

        CloseConnection();
        DisposeProxy();
      }
    }

    /// <summary>
    /// Test to Add and Find data from a collection using the <see cref="MyTestXProxy"/>.
    /// </summary>
    [Fact]
    public void Remove()
    {
      OpenConnection();

      try
      {
        InitXProxy(ScriptLanguageType.JavaScript);
        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, USERS_COLLECTION_NAME, X_TEST_SCHEMA_NAME), Connection);

        var result = Command.ExecuteScalar();
        int count;
        int usersCount = USERS_COUNT;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(TABLE_NOT_FOUND, USERS_COLLECTION_NAME));

        XProxy.ExecuteScript(GetSchemaXTest, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(GetCollectionXTextUser, ScriptLanguageType.JavaScript);
        XProxy.ExecuteScript(JAVASCRIPT_ADD_SINGLE_USER1, ScriptLanguageType.JavaScript);
        usersCount++;
        XProxy.ExecuteScript(JAVASCRIPT_ADD_SINGLE_USER2, ScriptLanguageType.JavaScript);
        usersCount++;
        XProxy.ExecuteScript(JAVASCRIPT_ADD_MULTIPLE_USERS_SINGLE_ADD, ScriptLanguageType.JavaScript);
        usersCount += 3;
        XProxy.ExecuteScript(JAVASCRIPT_ADD_MULTIPLE_USERS_MULTIPLE_ADD, ScriptLanguageType.JavaScript);
        usersCount += 3;

        XProxy.ExecuteScript(REMOVE_USER, ScriptLanguageType.JavaScript);
        usersCount--;
        var selectResult = XProxy.ExecuteScript(FIND_ALL_DOCUMENTS_IN_COLLECTION, ScriptLanguageType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == usersCount, DATA_NOT_MATCH);

        XProxy.ExecuteScript(REMOVE_SORT_USER, ScriptLanguageType.JavaScript);
        usersCount -= 2;
        selectResult = XProxy.ExecuteScript(FIND_ALL_DOCUMENTS_IN_COLLECTION, ScriptLanguageType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == usersCount, DATA_NOT_MATCH);
        selectResult = XProxy.ExecuteScript(FIND_REMOVED_USERS, ScriptLanguageType.JavaScript);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 0, DATA_NOT_MATCH);
      }
      finally
      {
        XProxy.ExecuteScript(REVERT_ADDED_USERS, ScriptLanguageType.JavaScript);
        if (Command != null)
        {
          Command.Dispose();
        }

        CloseConnection();
        DisposeProxy();
      }
    }

    /// <summary>
    /// Test to validate whether the active session is active and can be parsed, using the <see cref="MyTestXProxy"/>
    /// </summary>
    [Fact]
    public void TestSessions()
    {
      OpenConnection();

      try
      {
        InitXProxy(ScriptLanguageType.JavaScript);

        // Validate session is open
        var sessionIsOpen = XProxy.ExecuteQuery(IS_SESSION_OPEN, ScriptLanguageType.JavaScript).Result;
        bool result;
        Assert.True(sessionIsOpen != null && bool.TryParse(sessionIsOpen.ToString(), out result));

        // Parse Uri of active session
        var shellParseSessionResult = XProxy.ExecuteQuery(SHELL_PARSE_URI_FROM_SESSION_URI, ScriptLanguageType.JavaScript).Result;
        Assert.True(shellParseSessionResult != null && shellParseSessionResult.ToString().Contains("dbUser"));
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
  }
}
