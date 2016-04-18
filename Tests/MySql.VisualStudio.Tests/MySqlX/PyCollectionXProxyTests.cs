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
using MySQL.Utility.Classes;
using Xunit;

namespace MySql.VisualStudio.Tests.MySqlX
{
  public class PyCollectionXProxyTests : BaseCollectionTests, IUseFixture<SetUpXShell>
  {
    #region Fields

    /// <summary>
    /// Object to access and execute commands to the current database connection through the mysqlx protocol
    /// </summary>
    private MySqlXProxy _xProxy;

    #endregion Fields

    /// <summary>
    /// Test to Add, Modify, Delete and Find a record from a collection using the <see cref="MySqlXProxy"/>, executing the commands in a single line
    /// </summary>
    // [Fact]
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
        Command?.Dispose();
        CloseConnection();
      }
    }

    /// <summary>
    /// Test to Insert, Update and Delete record from a table using the <see cref="MySqlXProxy"/>, executing the commands in multiple lines and in a single script
    /// </summary>
    [Fact]
    public void AllTests()
    {
      OpenConnection();
      MySqlDataReader reader = null;

      try
      {
        InitXProxy();

        // Create Schema test
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

        // Create Collection test
        _xProxy.ExecuteScript(CreateCollectionTest, ScriptType.Python);
        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, TEST_COLLECTION_NAME, TEST_SCHEMA_NAME), Connection);
        var result = Command.ExecuteScalar();
        int count;
        object foundValue = null;
        int usersCount = USERS_COUNT;
        int.TryParse(result.ToString(), out count);
        Assert.True(count > 0, string.Format(COLLECTION_NOT_FOUND, TEST_COLLECTION_NAME));

        // Comments test
        _xProxy.ExecuteScript(PYTHON_COMMENT_SINGLE_LINE_1, ScriptType.Python);
        _xProxy.ExecuteScript(PYTHON_COMMENT_SINGLE_LINE_2, ScriptType.Python);

        // Drop Collection test
        _xProxy.ExecuteScript(DropCollectionTest, ScriptType.Python);
        Command = new MySqlCommand(string.Format(SEARCH_TABLE_SQL_SYNTAX, TEST_COLLECTION_NAME, TEST_SCHEMA_NAME), Connection);
        result = Command.ExecuteScalar();
        int.TryParse(result.ToString(), out count);
        Assert.True(count == 0, string.Format(COLLECTION_NOT_DELETED, TEST_COLLECTION_NAME));

        // Drop Schema test
        _xProxy.ExecuteScript(DropSchemaTest, ScriptType.Python);
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

        // Single Inserts Test
        _xProxy.ExecuteScript(GetSchemaSakilaX, ScriptType.Python);
        _xProxy.ExecuteScript(GetCollectionSakilaXUser, ScriptType.Python);

        // Test single add
        _xProxy.ExecuteScript(PYTHON_ADD_SINGLE_USER1, ScriptType.Python);
        var selectResult = _xProxy.ExecuteScript(FIND_ALL_DOCUMENTS_IN_COLLECTION, ScriptType.Python);
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

        // Find Test
        selectResult = _xProxy.ExecuteScript(FIND_SPECIFIC_USER_TEST, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == 1, DATA_NOT_MATCH);

        // More comments test
        var script = new StringBuilder();
        script.AppendLine(PYTHON_COMMENT_MULTI_LINE_1);
        script.AppendLine(PYTHON_COMMENT_MULTI_LINE_2);
        script.AppendLine(PYTHON_COMMENT_MULTI_LINE_3);
        _xProxy.ExecuteScript(script.ToString(), ScriptType.Python);
        var tokenizer = new MyPythonTokenizer(script.ToString());
        _xProxy.ExecuteScript(tokenizer.BreakIntoStatements().ToArray(), ScriptType.Python);

        // Modify Set
        _xProxy.ExecuteScript(MODIFY_SET_USER, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(FIND_MODIFIED_USER, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        var docResult = selectResult?.FirstOrDefault();
        if (docResult != null)
        {

          docResult.TryGetValue("status", out foundValue);
        }

        Assert.True(foundValue != null && foundValue.ToString().Equals("inactive", StringComparison.InvariantCultureIgnoreCase), DATA_NOT_MATCH);

        // Modify Set binding array
        _xProxy.ExecuteScript(MODIFY_SET_BINDING_ARRAY_USER, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(FIND_MODIFIED_USER, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        docResult = selectResult?.FirstOrDefault();
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
        docResult = selectResult?.FirstOrDefault();
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(docResult != null && !docResult.ContainsKey("age"), DATA_NOT_MATCH);

        // Modify unset list of keys
        _xProxy.ExecuteScript(MODIFY_UNSET_LIST_USER, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(FIND_MODIFIED_USER, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        docResult = selectResult?.FirstOrDefault();
        Assert.True(docResult != null && !docResult.ContainsKey("status") && !docResult.ContainsKey("ratings"), DATA_NOT_MATCH);

        // Modify merge
        _xProxy.ExecuteScript(PYTHON_MODIFY_MERGE_USER, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(FIND_MODIFIED_USER, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        docResult = selectResult?.FirstOrDefault();
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
        docResult = selectResult?.FirstOrDefault();
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
        docResult = selectResult?.FirstOrDefault();
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
        docResult = selectResult?.FirstOrDefault();
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

        // Remove test
        _xProxy.ExecuteScript(REMOVE_USER, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(FIND_ALL_DOCUMENTS_IN_COLLECTION, ScriptType.Python);
        usersCount -= 1;
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

        // Remove back the added users and test
        _xProxy.ExecuteScript(REVERT_ADDED_USERS, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(FIND_ALL_DOCUMENTS_IN_COLLECTION, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == USERS_COUNT, DATA_NOT_MATCH);

        // Find complex
        _xProxy.ExecuteScript(GetCollectionSakilaXMovies, ScriptType.Python);
        _xProxy.ExecuteScript(FIND_MOVIES_COMPLEX_QUERY1, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(FIND_MOVIES_COMPLEX_QUERY3, ScriptType.Python);
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
    /// Test to create a Table using the <see cref="MySqlXProxy"/>.
    /// </summary>
    // [Fact]
    public void CreateCollection()
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
        Command?.Dispose();
        CloseConnection();
      }
    }

    /// <summary>
    /// Test to create a Database using the <see cref="MySqlXProxy"/>.
    /// </summary>
    // [Fact]
    public void CreateSchema()
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
        Command?.Dispose();
        CloseConnection();
      }
    }

    /// <summary>
    /// Test to Insert, Update and Delete record from a table using the <see cref="MySqlXProxy"/>, executing the commands in multiple lines and in a single script.
    /// </summary>
    // [Fact]
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
        Command?.Dispose();
        CloseConnection();
      }
    }

    /// <summary>
    /// Test to Modify data from a collection using the <see cref="MySqlXProxy"/>.
    /// </summary>
    // [Fact]
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
        var docResult = selectResult?.FirstOrDefault();
        if (docResult != null)
        {

          docResult.TryGetValue("status", out foundValue);
        }

        Assert.True(foundValue != null && foundValue.ToString().Equals("inactive", StringComparison.InvariantCultureIgnoreCase), DATA_NOT_MATCH);

        // Modify Set binding array
        _xProxy.ExecuteScript(MODIFY_SET_BINDING_ARRAY_USER, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(FIND_MODIFIED_USER, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        docResult = selectResult?.FirstOrDefault();
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
        docResult = selectResult?.FirstOrDefault();
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(docResult != null && !docResult.ContainsKey("age"), DATA_NOT_MATCH);

        // Modify unset list of keys
        _xProxy.ExecuteScript(MODIFY_UNSET_LIST_USER, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(FIND_MODIFIED_USER, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        docResult = selectResult?.FirstOrDefault();
        Assert.True(docResult != null && !docResult.ContainsKey("status") && !docResult.ContainsKey("ratings"), DATA_NOT_MATCH);

        // Modify merge
        _xProxy.ExecuteScript(PYTHON_MODIFY_MERGE_USER, ScriptType.Python);
        selectResult = _xProxy.ExecuteScript(FIND_MODIFIED_USER, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        docResult = selectResult?.FirstOrDefault();
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
        docResult = selectResult?.FirstOrDefault();
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
        docResult = selectResult?.FirstOrDefault();
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
        docResult = selectResult?.FirstOrDefault();
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
        Command?.Dispose();
        CloseConnection();
      }
    }

    /// <summary>
    /// Test to Add and Find data from a collection using the <see cref="MySqlXProxy"/>.
    /// </summary>
    // [Fact]
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

        _xProxy.ExecuteScript(REMOVE_USER, ScriptType.Python);
        usersCount--;
        var selectResult = _xProxy.ExecuteScript(FIND_ALL_DOCUMENTS_IN_COLLECTION, ScriptType.Python);
        Assert.True(selectResult != null, string.Format(NULL_OBJECT, "selectResult"));
        Assert.True(selectResult != null && selectResult.Count == usersCount, DATA_NOT_MATCH);

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
        Command?.Dispose();
        CloseConnection();
      }
    }

    /// <summary>
    /// Setup information about the current schema test
    /// </summary>
    /// <param name="data">Current test instance configuration</param>
    public override void SetFixture(SetUpXShell data)
    {
      base.SetFixture(data);
      _xProxy = new MySqlXProxy(XConnString, true);

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
