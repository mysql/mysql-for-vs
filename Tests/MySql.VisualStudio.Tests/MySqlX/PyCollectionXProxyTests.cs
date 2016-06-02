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
using System.Text;
using MySql.Data.MySqlClient;
using MySql.Data.VisualStudio.Editors;
using MySql.Data.VisualStudio.MySqlX;
using MySql.VisualStudio.Tests.MySqlX.Base;
using MySQL.Utility.Classes.Tokenizers;
using Xunit;

namespace MySql.VisualStudio.Tests.MySqlX
{
  public class PyCollectionXProxyTests : BaseCollectionTests, IUseFixture<SetUpXShell>
  {
    /// <summary>
    /// Test to Add, Modify, Delete and Find a record from a collection using the <see cref="MySqlXProxy"/>, executing the commands in a single line
    /// </summary>
    [Fact]
    public void AddFind()
    {
      OpenConnection();

      try
      {
        InitXProxy();
        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, SAKILA_X_USERS_COLLECTION, SAKILA_X_SCHEMA_NAME), Connection);

        var result = Command.ExecuteScalar();
        int count;
        int usersCount = USERS_COUNT;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(SCHEMA_NOT_FOUND, SAKILA_X_SCHEMA_NAME));

        _xProxy.ExecuteScript(GetSchemaSakilaX, ScriptType.Python);
        _xProxy.ExecuteScript(GetCollectionSakilaXUser, ScriptType.Python);

        // Test single add
        _xProxy.ExecuteScript(PYTHON_ADD_SINGLE_USER1, ScriptType.Python);
        List<Dictionary<string, object>> selectResult = _xProxy.ExecuteScript(FIND_ALL_DOCUMENTS_IN_COLLECTION, ScriptType.Python);
        usersCount += 1;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == usersCount, DATA_NOT_MATCH);

        // Test single add again
        _xProxy.ExecuteScript(PYTHON_ADD_SINGLE_USER2, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(FIND_ALL_DOCUMENTS_IN_COLLECTION, ScriptType.Python);
        usersCount += 1;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == usersCount, DATA_NOT_MATCH);

        // Test multiple documents add statement
        _xProxy.ExecuteScript(PYTHON_ADD_MULTIPLE_USERS_SINGLE_ADD, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(FIND_ALL_DOCUMENTS_IN_COLLECTION, ScriptType.Python);
        usersCount += 3;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == usersCount, DATA_NOT_MATCH);

        // Test multiple add statements with single documents
        _xProxy.ExecuteScript(PYTHON_ADD_MULTIPLE_USERS_MULTIPLE_ADD, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(FIND_ALL_DOCUMENTS_IN_COLLECTION, ScriptType.Python);
        usersCount += 3;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == usersCount, DATA_NOT_MATCH);

        selectResult = _xProxy.ExecuteScript(FIND_SPECIFIC_USER_TEST, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 1, DATA_NOT_MATCH);

        // Remove back the added users and test
        _xProxy.ExecuteScript(REVERT_ADDED_USERS, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(FIND_ALL_DOCUMENTS_IN_COLLECTION, ScriptType.Python);
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
    /// Test to create and drop a Collection using the <see cref="MySqlXProxy"/>.
    /// </summary>
    [Fact]
    public void CreateAndDropCollection()
    {
      OpenConnection();

      try
      {
        InitXProxy();
        _xProxy.ExecuteScript(DropSchemaTestIfExists, ScriptType.Python);
        _xProxy.ExecuteScript(CreateSchemaTest, ScriptType.Python);
        _xProxy.ExecuteScript(CreateCollectionTest, ScriptType.Python);
        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, TEST_COLLECTION_NAME, TEST_SCHEMA_NAME), Connection);
        var result = Command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(COLLECTION_NOT_FOUND, TEST_COLLECTION_NAME));

        _xProxy.ExecuteScript(DropCollectionTest, ScriptType.Python);
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
    //[Fact]
    public void CreateAndDropIndex()
    {
      OpenConnection();
      int duplicateMovieCount = 0;

      try
      {
        InitXProxy();

        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, SAKILA_X_MOVIES_COLLECTION, SAKILA_X_SCHEMA_NAME), Connection);
        var result = Command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(SCHEMA_NOT_FOUND, SAKILA_X_SCHEMA_NAME));

        _xProxy.ExecuteScript(GetSchemaSakilaX, ScriptType.Python);
        _xProxy.ExecuteScript(GetCollectionSakilaXMovies, ScriptType.Python);

        // Add non-unique index
        _xProxy.ExecuteScript(CREATE_NON_UNIQUE_INDEX_MOVIES, ScriptType.Python);
        Command = new MySqlCommand(string.Format(SEARCH_INDEX_SQL_SYNTAX, SAKILA_X_SCHEMA_NAME, SAKILA_X_MOVIES_COLLECTION, MOVIES_NON_UNIQUE_INDEX_NAME), Connection);
        result = Command.ExecuteScalar();
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(INDEX_NOT_FOUND, MOVIES_NON_UNIQUE_INDEX_NAME));

        // Add unique index
        _xProxy.ExecuteScript(PYTHON_INCLUDE_MYSQLX, ScriptType.Python);
        _xProxy.ExecuteScript(CREATE_UNIQUE_INDEX_MOVIES, ScriptType.Python);
        Command = new MySqlCommand(string.Format(SEARCH_INDEX_SQL_SYNTAX, SAKILA_X_SCHEMA_NAME, SAKILA_X_MOVIES_COLLECTION, MOVIES_UNIQUE_INDEX_NAME), Connection);
        result = Command.ExecuteScalar();
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(INDEX_NOT_FOUND, MOVIES_UNIQUE_INDEX_NAME));

        // Test data uniqueness
        _xProxy.ExecuteScript(PYTHON_ADD_DUPLICATE_MOVIE, ScriptType.Python);
        var selectResult = _xProxy.ExecuteScript(FIND_DUPLICATE_MOVIE_TITLE, ScriptType.Python);
        duplicateMovieCount = selectResult != null ? selectResult.Count : 0;
        Assert.True(duplicateMovieCount == 1, DATA_NOT_UNIQUE);

        // Drop non-unique index
        _xProxy.ExecuteScript(DROP_NON_UNIQUE_INDEX_MOVIES, ScriptType.Python);
        Command = new MySqlCommand(string.Format(SEARCH_INDEX_SQL_SYNTAX, SAKILA_X_SCHEMA_NAME, SAKILA_X_MOVIES_COLLECTION, MOVIES_NON_UNIQUE_INDEX_NAME), Connection);
        result = Command.ExecuteScalar();
        int.TryParse(result.ToString(), out count);
        Assert.True(count == 0, string.Format(INDEX_NOT_FOUND, MOVIES_NON_UNIQUE_INDEX_NAME));

        // Drop unique index
        _xProxy.ExecuteScript(DROP_UNIQUE_INDEX_MOVIES, ScriptType.Python);
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
          _xProxy.ExecuteScript(REMOVE_DUPLICATE_MOVIE, ScriptType.Python);
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
    /// Test to create and drop a Schema using the <see cref="MySqlXProxy"/>.
    /// </summary>
    // [Fact]
    public void CreateAndDropSchema()
    {
      OpenConnection();
      MySqlDataReader reader = null;

      try
      {
        InitXProxy();
        _xProxy.ExecuteScript(DropSchemaTestIfExists, ScriptType.Python);
        _xProxy.ExecuteScript(CreateSchemaTest, ScriptType.Python);
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

        _xProxy.ExecuteScript(DropSchemaTest, ScriptType.Python);
        Command = new MySqlCommand(SHOW_DBS_SQL_SYNTAX, Connection);
        reader = Command.ExecuteReader();
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
        InitXProxy();
        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, SAKILA_X_USERS_COLLECTION, SAKILA_X_SCHEMA_NAME), Connection);

        var result = Command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(SCHEMA_NOT_FOUND, TEST_SCHEMA_NAME));

        _xProxy.ExecuteScript(GetSchemaSakilaX, ScriptType.Python);
        _xProxy.ExecuteScript(GetCollectionSakilaXMovies, ScriptType.Python);

        // Find complex
        _xProxy.ExecuteScript(FIND_MOVIES_COMPLEX_QUERY1, ScriptType.Python);
        var selectResult = _xProxy.ExecuteScript(FIND_MOVIES_COMPLEX_QUERY3, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == MOVIES_RATING_R_COUNT, DATA_NOT_MATCH);

        _xProxy.ExecuteScript(FIND_MOVIES_COMPLEX_QUERY2, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(FIND_MOVIES_COMPLEX_QUERY4, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 10, DATA_NOT_MATCH);

        // Find bound array
        object foundTitle = null;
        var singleResult = _xProxy.ExecuteScript(FIND_MOVIES_BOUND_ARRAY, ScriptType.Python).FirstOrDefault();
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
    /// Test to Insert, Update and Delete record from a table using the <see cref="MySqlXProxy"/>, executing the commands in multiple lines and in a single script.
    /// </summary>
    [Fact]
    // TODO: Modify to correctly use or move the AppendLine strategy to the AllTests method.
    public void Insert_JsonFormat_SingleScript()
    {
      OpenConnection();

      try
      {
        InitXProxy();
        var script = new StringBuilder();
        script.AppendLine(DropSchemaTestIfExists);
        script.AppendLine(CreateSchemaTest);
        script.AppendLine(CreateCollectionTest);

        script.AppendLine(GetSchemaSakilaX);
        script.AppendLine(GetCollectionSakilaXUser);
        script.AppendLine(JAVASCRIPT_ADD_SINGLE_USER1);
        script.AppendLine(JAVASCRIPT_ADD_SINGLE_USER2);

        var tokenizer = new MyPythonTokenizer(script.ToString());
        _xProxy.ExecuteScript(tokenizer.BreakIntoStatements().ToArray(), ScriptType.Python);

        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, TEST_COLLECTION_NAME, TEST_SCHEMA_NAME), Connection);

        var result = Command.ExecuteScalar();
        int count;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(COLLECTION_NOT_FOUND, TEST_COLLECTION_NAME));

        var selectResult = _xProxy.ExecuteScript(FIND_ALL_DOCUMENTS_IN_COLLECTION, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 2, DATA_NOT_MATCH);
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
    /// Test to Modify data from a collection using the <see cref="MySqlXProxy"/>.
    /// </summary>
    [Fact]
    public void Modify()
    {
      OpenConnection();

      try
      {
        InitXProxy();
        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, SAKILA_X_USERS_COLLECTION, SAKILA_X_SCHEMA_NAME), Connection);

        var result = Command.ExecuteScalar();
        int count;
        object foundValue = null;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(SCHEMA_NOT_FOUND, SAKILA_X_SCHEMA_NAME));

        _xProxy.ExecuteScript(GetSchemaSakilaX, ScriptType.Python);
        _xProxy.ExecuteScript(GetCollectionSakilaXUser, ScriptType.Python);
        _xProxy.ExecuteScript(JAVASCRIPT_ADD_SINGLE_USER1, ScriptType.Python);
        _xProxy.ExecuteScript(JAVASCRIPT_ADD_SINGLE_USER2, ScriptType.Python);
        _xProxy.ExecuteScript(JAVASCRIPT_ADD_MULTIPLE_USERS_SINGLE_ADD, ScriptType.Python);
        _xProxy.ExecuteScript(JAVASCRIPT_ADD_MULTIPLE_USERS_MULTIPLE_ADD, ScriptType.Python);

        // Modify Set
        _xProxy.ExecuteScript(MODIFY_SET_USER, ScriptType.Python);
        var selectResult = _xProxy.ExecuteScript(FIND_MODIFIED_USER, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        var docResult = selectResult != null ? selectResult.FirstOrDefault() : null;
        if (docResult != null)
        {

          docResult.TryGetValue("status", out foundValue);
        }

        Assert.True(foundValue != null && foundValue.ToString().Equals("inactive", StringComparison.InvariantCultureIgnoreCase), DATA_NOT_MATCH);

        // Modify Set binding array
        _xProxy.ExecuteScript(MODIFY_SET_BINDING_ARRAY_USER, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(FIND_MODIFIED_USER, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        docResult = selectResult != null ? selectResult.FirstOrDefault() : null;
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
        _xProxy.ExecuteScript(MODIFY_UNSET_USER, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(FIND_MODIFIED_USER, ScriptType.Python);
        docResult = selectResult != null ? selectResult.FirstOrDefault() : null;
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(docResult != null && !docResult.ContainsKey("age"), DATA_NOT_MATCH);

        // Modify unset list of keys
        _xProxy.ExecuteScript(MODIFY_UNSET_LIST_USER, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(FIND_MODIFIED_USER, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        docResult = selectResult != null ? selectResult.FirstOrDefault() : null;
        Assert.True(docResult != null && !docResult.ContainsKey("status") && !docResult.ContainsKey("ratings"), DATA_NOT_MATCH);

        // Modify merge
        _xProxy.ExecuteScript(PYTHON_MODIFY_MERGE_USER, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(FIND_MODIFIED_USER, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        docResult = selectResult != null ? selectResult.FirstOrDefault() : null;
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
        _xProxy.ExecuteScript(MODIFY_ARRAY_APPEND_USER, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(FIND_MODIFIED_USER, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        docResult = selectResult != null ? selectResult.FirstOrDefault() : null;
        if (docResult != null)
        {
          docResult.TryGetValue("ratings", out foundValue);
          foundRatingList = foundValue as List<object>;
        }

        Assert.True(foundRatingList != null && foundRatingList.Select(o => o.ToString()).Contains("PG-13"), DATA_NOT_MATCH);

        // Modify array insert
        _xProxy.ExecuteScript(MODIFY_ARRAY_INSERT_USER, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(FIND_MODIFIED_USER, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        docResult = selectResult != null ? selectResult.FirstOrDefault() : null;
        if (docResult != null)
        {
          docResult.TryGetValue("ratings", out foundValue);
          foundRatingList = foundValue as List<object>;
        }

        Assert.True(foundRatingList != null && foundRatingList.Select(o => o.ToString()).ToList()[2].Equals("G", StringComparison.InvariantCultureIgnoreCase), DATA_NOT_MATCH);

        // Modify array delete
        _xProxy.ExecuteScript(MODIFY_ARRAY_DELETE_USER, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(FIND_MODIFIED_USER, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        docResult = selectResult != null ? selectResult.FirstOrDefault() : null;
        if (docResult != null)
        {
          docResult.TryGetValue("ratings", out foundValue);
          foundRatingList = foundValue as List<object>;
        }

        Assert.True(foundRatingList != null && !foundRatingList.Select(o => o.ToString()).ToList()[2].Equals("G", StringComparison.InvariantCultureIgnoreCase), DATA_NOT_MATCH);

        // Modify sort
        _xProxy.ExecuteScript(MODIFY_SORT_USER, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(FIND_MODIFIED_J_USERS, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 2, DATA_NOT_MATCH);
      }
      finally
      {
        _xProxy.ExecuteScript(REVERT_ADDED_USERS, ScriptType.Python);
        if (Command != null)
        {
          Command.Dispose();
        }

        CloseConnection();
        DisposeProxy();
      }
    }

    /// <summary>
    /// Test to Add and Find data from a collection using the <see cref="MySqlXProxy"/>.
    /// </summary>
    [Fact]
    public void Remove()
    {
      OpenConnection();

      try
      {
        InitXProxy();
        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, SAKILA_X_USERS_COLLECTION, SAKILA_X_SCHEMA_NAME), Connection);

        var result = Command.ExecuteScalar();
        int count;
        int usersCount = USERS_COUNT;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(SCHEMA_NOT_FOUND, SAKILA_X_SCHEMA_NAME));

        _xProxy.ExecuteScript(GetSchemaSakilaX, ScriptType.Python);
        _xProxy.ExecuteScript(GetCollectionSakilaXUser, ScriptType.Python);
        _xProxy.ExecuteScript(JAVASCRIPT_ADD_SINGLE_USER1, ScriptType.Python);
        usersCount++;
        _xProxy.ExecuteScript(JAVASCRIPT_ADD_SINGLE_USER2, ScriptType.Python);
        usersCount++;
        _xProxy.ExecuteScript(JAVASCRIPT_ADD_MULTIPLE_USERS_SINGLE_ADD, ScriptType.Python);
        usersCount += 3;
        _xProxy.ExecuteScript(JAVASCRIPT_ADD_MULTIPLE_USERS_MULTIPLE_ADD, ScriptType.Python);
        usersCount += 3;

        // Remove test
        _xProxy.ExecuteScript(REMOVE_USER, ScriptType.Python);
        usersCount--;
        var selectResult = _xProxy.ExecuteScript(FIND_ALL_DOCUMENTS_IN_COLLECTION, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == usersCount, DATA_NOT_MATCH);

        // Remove sort test
        _xProxy.ExecuteScript(REMOVE_SORT_USER, ScriptType.Python);
        usersCount -= 2;
        selectResult = _xProxy.ExecuteScript(FIND_ALL_DOCUMENTS_IN_COLLECTION, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == usersCount, DATA_NOT_MATCH);
        selectResult = _xProxy.ExecuteScript(FIND_REMOVED_USERS, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 0, DATA_NOT_MATCH);
      }
      finally
      {
        _xProxy.ExecuteScript(REVERT_ADDED_USERS, ScriptType.Python);
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