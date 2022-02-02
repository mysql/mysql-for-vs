// Copyright © 2016, 2019, Oracle and/or its affiliates. All rights reserved.
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

using MySql.Utility.Enums;

namespace MySql.Utility.Tests.MySqlX.Base
{
  /// <summary>
  /// Defines properties specific to collection tests.
  /// </summary>
  public abstract class CollectionTestsProperties
  {
    #region Collection Queries

    /// <summary>
    /// Gets a statement to enable the use of mysqlx within script.
    /// </summary>
    public abstract string IncludeMysqlx { get; }

    /// <summary>
    /// Gets a statement to attempt to add a duplicate movie to the sakila_x.movies collection.
    /// </summary>
    public abstract string AddDuplicateMovie { get; }

    /// <summary>
    /// Gets a statement to insert multiple records on a single statement chaining add methods.
    /// </summary>
    public abstract string AddMultipleUsersMultipleAdd { get; }

    /// <summary>
    /// Gets a statement to insert multiple records on a single statement with a single add but an array of documents.
    /// </summary>
    public abstract string AddMultipleUsersSingleAdd { get; }

    /// <summary>
    /// Gets a statement to add a single user to the sakila_x.users collection.
    /// </summary>
    public abstract string AddSingleUser1 { get; }

    /// <summary>
    /// Gets a statement to add a single user to the sakila_x.users collection.
    /// </summary>
    public abstract string AddSingleUser2 { get; }

    /// <summary>
    /// Gets a statement to create a collection.
    /// </summary>
    public abstract string CreateCollection { get; }

    /// <summary>
    /// Gets a statement to create a non-unique index.
    /// </summary>
    public abstract string CreateNonUniqueIndexMovies { get; }

    /// <summary>
    /// Gets a statement to create a schema.
    /// </summary>
    public abstract string CreateSchema { get; }

    /// <summary>
    /// Gets a statement to create a unique index.
    /// </summary>
    public abstract string CreateUniqueIndexMovies { get; }

    /// <summary>
    /// Gets a statement to drop a collection.
    /// </summary>
    public abstract string DropCollection { get; }

    /// <summary>
    /// Gets a statement to drop non-unique index.
    /// </summary>
    public abstract string DropNonUniqueIndexMovies { get; }

    /// <summary>
    /// Gets a statement to drop the test database
    /// </summary>
    public abstract string DropSchema { get; }

    /// <summary>
    /// Gets a statement to drop unique index.
    /// </summary>
    public abstract string DropUniqueIndexMovies { get; }

    /// <summary>
    /// Gets a specific collection and assign it to a variable for persistence. 
    /// </summary>
    public abstract string GetCollection { get; }

    /// <summary>
    /// Gets a schema and assign it to a variable for persistence.
    /// </summary>
    public abstract string GetSchema { get; }

    /// <summary>
    /// Gets a statement to modify a SakilaX.User value and append a value to a specific array.
    /// </summary>
    public abstract string ModifyArrayAppendUser { get; }

    /// <summary>
    /// Gets a statement to modify a SakilaX.User value and insert a value to a specific array in a specified position.
    /// </summary>
    public abstract string ModifyArrayDeleteUser { get; }

    /// <summary>
    /// Gets a statement to modify a SakilaX.User value and insert a value to a specific array in a specified position.
    /// </summary>
    public abstract string ModifyArrayInsertUser { get; }

    /// <summary>
    /// Gets a statement to modify a SakilaX.User value and set additional values.
    /// </summary>
    public abstract string ModifyMergeUser { get; }

    #endregion Collection Queries

    #region Session Queries

    /// <summary>
    /// Gets a statement to validate whether the active session is open.
    /// </summary>
    public abstract string IsSessionOpen { get; }

    /// <summary>
    /// Gets a statement to get the parsed Uri of the active session Uri.
    /// </summary>
    public abstract string ShellParseUriFromSessionUri { get; }

    #endregion Session Queries

    #region Properties

    /// <summary>
    /// Gets a statement to create the test collection.
    /// </summary>
    public string CreateCollectionTest { get; protected set; }

    /// <summary>
    /// Gets a statement to create the test schema.
    /// </summary>
    public string CreateSchemaTempSchema { get; protected set; }

    /// <summary>
    /// Gets a statement to drop the test collection.
    /// </summary>
    public string DropCollectionTest { get; protected set; }

    /// <summary>
    /// Gets a statement to drop the test schema.
    /// </summary>
    public string DropSchemaTempSchema { get; protected set; }

    /// <summary>
    /// Gets a statement to drop the test database
    /// </summary>
    public string DropSchemaTempSchemaIfExists { get; protected set; }

    /// <summary>
    /// Gets a statement to get the collection_test collection from the Test schema.
    /// </summary>
    public string GetCollectionTestSchemaTest { get; protected set; }

    /// <summary>
    /// Gets a statement to get the movies collection from the x_test schema.
    /// </summary>
    public string GetCollectionXTestMovies { get; protected set; }

    /// <summary>
    /// Gets a statement to get the user collection from the x_test schema.
    /// </summary>
    public string GetCollectionXTextUser { get; protected set; }

    /// <summary>
    /// Gets a statement to get the Test schema.
    /// </summary>
    public string GetSchemaTempSchema { get; protected set; }

    /// <summary>
    /// Gets a statement to get the x_test schema.
    /// </summary>
    public string GetSchemaXTest { get; protected set; }

    /// <summary>
    /// Gets the language used for the tests.
    /// </summary>
    public ScriptLanguageType ScriptLanguage { get; protected set; }

    #endregion Properties

    /// <summary>
    /// Initializes a new instance of the <see cref="CollectionTestsProperties"/> class.
    /// </summary>
    /// <param name="scriptLanguage">A <see cref="ScriptLanguageType"/> value.</param>
    protected CollectionTestsProperties(ScriptLanguageType scriptLanguage)
    {
      DropSchemaTempSchemaIfExists = string.Format(BaseTests.DROP_SCHEMA_IF_EXISTS, BaseTests.TEMP_SCHEMA_NAME);
      ScriptLanguage = scriptLanguage;
    }
  }
}
