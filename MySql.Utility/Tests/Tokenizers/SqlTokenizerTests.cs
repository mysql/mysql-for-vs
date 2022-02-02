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

using System;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;
using MySql.Utility.Classes.Tokenizers;
using Xunit;

namespace MySql.Utility.Tests.Tokenizers
{
  public class SqlTokenizerTests : SetUpDatabaseTestsBase
  {
    /// <summary>
    /// The test schema name for all tests.
    /// </summary>
    public const string X_TEST_SCHEMA_NAME = "x_test";

    private const int SCRIPT_STATEMENT_COUNT = 14;
    private const int CHARACTER_TABLE_ROWS_COUNT = 27;
    private const int HALO_CHARACTERS_VIEW_ROWS_COUNT = 17;
    private const int MASS_EFFECT_CHARACTERS_VIEW_ROWS_COUNT = 10;
    private const int SPARTAN_CHARACTERS_VIEW_ROWS_COUNT = 9;

    private MySqlTokenizer _tokenizer;

    public SqlTokenizerTests()
    {
      _tokenizer = null;
      SchemaName1 = X_TEST_SCHEMA_NAME;
      SchemaName2 = null;
      SchemaName3 = null;
      DropSchemasOnDispose = false;
    }

    #region Properties

    /// <summary>
    /// Gets the <see cref="MySqlCommand"/> to execute statements.
    /// </summary>
    public MySqlCommand Command { get; protected set; }

    /// <summary>
    /// Gets the <see cref="MySqlConnection"/> to connect to a server.
    /// </summary>
    public MySqlConnection Connection { get; protected set; }

    #endregion Properties

    public override void Dispose()
    {
      base.Dispose();
      if (Connection != null)
      {
        Connection.Dispose();
      }

      if (Command != null)
      {
        Command.Dispose();
      }
    }

    [Fact]
    public void SplitStatements()
    {
      var script = GetSqlSetupScript();
      bool isScriptEmpty = string.IsNullOrEmpty(script);
      Assert.True(!isScriptEmpty);

      script = ReplaceSchemasInScript(script.Trim());
      _tokenizer = new MySqlTokenizer(script);
      var statementsList = _tokenizer.BreakIntoStatements();
      Assert.True(statementsList.Count == SCRIPT_STATEMENT_COUNT);

      OpenConnection();
      try
      {
        using (Command = new MySqlCommand())
        {
          Command.Connection = Connection;
          foreach (var statement in statementsList)
          {
            Command.CommandText = statement;
            Command.ExecuteNonQuery();
          }

          // Test schema created
          var schemas = Connection.GetSchema("Databases");
          Assert.True(schemas != null
                      && !string.IsNullOrEmpty(SchemaName1)
                      && schemas.Rows.Cast<DataRow>().Any(row => row["DATABASE_NAME"].ToString().Equals(SchemaName1, StringComparison.Ordinal)));

          // Test table created
          var tables = Connection.GetSchema("Tables", new[] {null, SchemaName1});
          Assert.True(schemas != null
                      && tables.Rows.Cast<DataRow>().Any(row => row["TABLE_NAME"].ToString().Equals("character", StringComparison.Ordinal)));

          // Test data
          Command.CommandText = string.Format("USE `{0}`;", SchemaName1);
          Command.ExecuteNonQuery();

          Command.CommandText = "SELECT COUNT(*) FROM `character`;";
          var res = Command.ExecuteScalar();
          Assert.True((long)res == CHARACTER_TABLE_ROWS_COUNT);

          Command.CommandText = "SELECT COUNT(*) FROM `halo_characters`;";
          res = Command.ExecuteScalar();
          Assert.True((long)res == HALO_CHARACTERS_VIEW_ROWS_COUNT);

          Command.CommandText = "SELECT COUNT(*) FROM `mass_effect_characters`;";
          res = Command.ExecuteScalar();
          Assert.True((long)res == MASS_EFFECT_CHARACTERS_VIEW_ROWS_COUNT);

          Command.CommandText = "SELECT COUNT(*) FROM `spartan_characters`;";
          res = Command.ExecuteScalar();
          Assert.True((long)res == SPARTAN_CHARACTERS_VIEW_ROWS_COUNT);
        }
      }
      finally
      {
        if (Connection.State != ConnectionState.Closed)
        {
          Connection.Close();
        }
      }
    }

    /// <summary>
    /// Open a MySqlConnection when it is not opened.
    /// </summary>
    private void OpenConnection()
    {
      if (Connection == null)
      {
        Connection = new MySqlConnection(ConnectionString);
      }

      if (Connection.State != ConnectionState.Open)
      {
        Connection.Open();
      }
    }
  }
}
