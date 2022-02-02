// Copyright (c) 2016, Oracle and/or its affiliates. All rights reserved.
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License as
// published by the Free Software Foundation; version 2 of the
// License.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301  USA

using System.Collections.Generic;
using System.Linq;
using MySql.Utility.Classes.MySqlX;
using MySql.Utility.Classes.Tokenizers;
using MySql.Utility.Enums;
using MySql.Utility.Interfaces;
using Xunit;

namespace MySql.Utility.Tests.Tokenizers
{
  public abstract class TokenizerBaseTests : SetUpDatabaseTestsBase
  {
    #region Constants

    protected const string COLLECTION_1_NAME = "movies";
    protected const string COLLECTION_2_NAME = "users";
    protected const string COLLECTION_3_NAME = "characters";

    protected const int COLLECTION_1_DOCUMENTS_COUNT = 1000;
    protected const int COLLECTION_2_DOCUMENTS_COUNT = 10;
    protected const int COLLECTION_3_DOCUMENTS_COUNT = 27;

    #endregion Constants

    #region Fields

    /// <summary>
    /// The tokenizer to use for the tests.
    /// </summary>
    private ITokenizer _tokenizer;

    /// <summary>
    ///The <see cref="MySqlXProxy"/> to execute statements.
    /// </summary>
    private MySqlXProxy _xProxy;

    #endregion Fields

    #region Properties

    /// <summary>
    /// Gets the script for the tests.
    /// </summary>
    public string Script
    {
      get
      {
        return Tokenizer.DataToTokenize;
      }

      protected set
      {
        Tokenizer.DataToTokenize = value;
      }
    }

    /// <summary>
    /// Gets the <see cref="ScriptLanguageType"/> used for the tests.
    /// </summary>
    public ScriptLanguageType ScriptLanguage { get; protected set; }

    /// <summary>
    /// Gets the number of statements in the loaded script.
    /// </summary>
    public int ScriptStatementsCount { get; protected set; }

    /// <summary>
    /// Gets a list of individual statements from the <see cref="Script"/>.
    /// </summary>
    public List<string> StatementsList
    {
      get
      {
        return Tokenizer.BreakIntoStatements();
      }
    }

    /// <summary>
    /// Gets the tokenizer to use for the tests.
    /// </summary>
    public ITokenizer Tokenizer
    {
      get
      {
        if (_tokenizer == null)
        {
          _tokenizer = TokenizerFactory.GetTokenizer(ScriptLanguage, null);
        }

        return _tokenizer;
      }
    }

    /// <summary>
    /// Gets the <see cref="MySqlXProxy"/> to execute statements.
    /// </summary>
    public MySqlXProxy XProxy
    {
      get
      {
        if (_xProxy == null)
        {
          _xProxy = new MySqlXProxy(XConnectionString, true, ScriptLanguage);
        }

        return _xProxy;
      }
    }

    #endregion Properties

    /// <summary>
    /// Tests the splitting of statements on the <see cref="Script"/>.
    /// </summary>
    [Fact]
    public virtual void SplitStatements()
    {
      Assert.True(!string.IsNullOrEmpty(Script));

      var statements = StatementsList;
      Assert.True(statements.Count == ScriptStatementsCount);

      ExecuteScript(statements.ToArray());

      // Test schema created
      var schemas = GetSchemas();
      Assert.True(schemas != null
                  && !string.IsNullOrEmpty(SchemaName1)
                  && schemas.Any(dic => dic.ContainsValue(SchemaName1)));

      // Test collections created
      var collections = GetCollections(SchemaName1);
      Assert.True(collections != null
                  && collections.Any(dic => dic.ContainsValue(COLLECTION_1_NAME)));
      Assert.True(collections != null
                  && collections.Any(dic => dic.ContainsValue(COLLECTION_2_NAME)));
      Assert.True(collections != null
                  && collections.Any(dic => dic.ContainsValue(COLLECTION_3_NAME)));

      // Test data
      var results = FindAll(SchemaName1, COLLECTION_1_NAME);
      Assert.True(results != null && results.Count == COLLECTION_1_DOCUMENTS_COUNT);
      results = FindAll(SchemaName1, COLLECTION_2_NAME);
      Assert.True(results != null && results.Count == COLLECTION_2_DOCUMENTS_COUNT);
      results = FindAll(SchemaName1, COLLECTION_3_NAME);
      Assert.True(results != null && results.Count == COLLECTION_3_DOCUMENTS_COUNT);
    }

    /// <summary>
    /// Executes a list of statements using the <see cref="XProxy"/>.
    /// </summary>
    /// <param name="statements">An array of statements to execute.</param>
    /// <returns>A list of boxed BaseShell results.</returns>
    protected List<object> ExecuteScript(string[] statements)
    {
      return XProxy.ExecuteStatementsBaseAsResultObject(statements, ScriptLanguage);
    }

    /// <summary>
    /// Executes a single statement using the <see cref="XProxy"/>.
    /// </summary>
    /// <param name="statement">A single statement to execute.</param>
    /// <returns>A list of dictionaries with results.</returns>
    protected List<Dictionary<string, object>> ExecuteStatement(string statement)
    {
      return XProxy.ExecuteSingleStatementAsResultObject(statement, ScriptLanguage);
    }

    /// <summary>
    /// Executes a find with no filters (to return all documents) in the specific schema and collection.
    /// </summary>
    /// <param name="schemaName">The name of the schema.</param>
    /// <param name="collectionName">The name of the collection.</param>
    /// <returns>A list of dictionaries with results.</returns>
    protected List<Dictionary<string, object>> FindAll(string schemaName, string collectionName)
    {
      string getCollectionStatement = ScriptLanguage == ScriptLanguageType.JavaScript
        ? "getCollection"
        : "get_collection";
      string getSchemaStatement = ScriptLanguage == ScriptLanguageType.JavaScript
        ? "getSchema"
        : "get_schema";
      return ExecuteStatement(string.Format("session.{0}('{1}').{2}('{3}').find().execute()", getSchemaStatement, schemaName, getCollectionStatement, collectionName));
    }

    /// <summary>
    /// Gets a list of dictionaries with the names of all collections in the given schema.
    /// </summary>
    /// <param name="schemaName">The name of the schema.</param>
    /// <returns>A list of dictionaries with the names of all collections in the given schema.</returns>
    protected List<Dictionary<string, object>> GetCollections(string schemaName)
    {
      string getCollectionsStatement = ScriptLanguage == ScriptLanguageType.JavaScript
        ? "getCollections"
        : "get_collections";
      string getSchemaStatement = ScriptLanguage == ScriptLanguageType.JavaScript
        ? "getSchema"
        : "get_schema";
      return XProxy.ExecuteSingleStatementAsResultObject(string.Format("session.{0}('{1}').{2}()", getSchemaStatement, schemaName, getCollectionsStatement), ScriptLanguage);
    }

    /// <summary>
    /// Gets a list of dictionaries with the names of all schemas in the current session.
    /// </summary>
    /// <returns>A list of dictionaries with the names of all schemas in the current session.</returns>
    protected List<Dictionary<string, object>> GetSchemas()
    {
      string getSchemasStatement = ScriptLanguage == ScriptLanguageType.JavaScript
        ? "getSchemas"
        : "get_schemas";
      return XProxy.ExecuteSingleStatementAsResultObject(string.Format("session.{0}()", getSchemasStatement), ScriptLanguage);
    }
  }
}
