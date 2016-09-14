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

using MySql.Utility.Enums;

namespace MySql.VisualStudio.Tests.MySqlX.Base
{
  /// <summary>
  /// Defines properties specific to collections tests in JavaScript.
  /// </summary>
  public class JsCollectionTestsProperties : CollectionTestsProperties
  {
    #region Collection Queries

    /// <summary>
    /// Gets a statement to enable the use of mysqlx within script.
    /// </summary>
    public override string IncludeMysqlx { get; } = "var mysqlx = require('mysqlx')";

    /// <summary>
    /// Gets a statement to attempt to add a duplicate movie to the sakila_x.movies collection.
    /// </summary>
    public override string AddDuplicateMovie { get; } = "coll.add({ title: '" + BaseCollectionTests.MOVIES_DUPLICATE_TITLE
                                                        + "', description: 'Trying to insert a duplicate title into the movies collection', release_year: 2006, language: 'English'"
                                                        + " , duration: '182 min', rating: 'DEL', actors: ['MICHELLE MCCONAUGHEY'] }).execute()";

    /// <summary>
    /// Gets a statement to insert multiple records on a single statement chaining add methods.
    /// </summary>
    public override string AddMultipleUsersMultipleAdd { get; } = "coll.add({ name: 'Javier Rivera', email: 'javier.rivera@oracle.com', password: 'Amjfadur01', test: 'yes' })"
                                                                  + ".add({ name: 'Francisco Tirado', email: 'francisco.tirado@oracle.com', password: 'Amjfadur02', test: 'yes' })"
                                                                  + ".add({ name: 'Ignacio Galarza', email: 'iggy.galarza@oracle.com', password: 'Amjfadur03', test: 'yes' }).execute()";

    /// <summary>
    /// Gets a statement to insert multiple records on a single statement with a single add but an array of documents.
    /// </summary>
    public override string AddMultipleUsersSingleAdd { get; } = "coll.add([{ name: 'Mike Zinner', email: 'mike.zinner@oracle.com', password: 'Supr3m3B0ss', test: 'yes' },"
                                                                + "{ name: 'Reggie Burnett', email: 'reggie.burnett@oracle.com', password: 'Burn3tt2016', test: 'yes' },"
                                                                + "{ name: 'Johannes Taxacher', email: 'johannes.taxacher@oracle.com', password: 'Tax98uk,', test: 'yes' }]).execute()";

    /// <summary>
    /// Gets a statement to add a single user to the sakila_x.users collection.
    /// </summary>
    public override string AddSingleUser1 { get; } = "coll.add({ name: 'Javier Trevino', email: 'javier.trevino@oracle.com', password: 'Lopkhue01', test: 'yes' }).execute()";

    /// <summary>
    /// Gets a statement to add a single user to the sakila_x.users collection.
    /// </summary>
    public override string AddSingleUser2 { get; } = "coll.add({ name: 'Alfonso Penunuri', email: 'luis.penunuri@oracle.com', password: 'Lopkhue02', test: 'yes' }).execute()";

    /// <summary>
    /// Gets a statement to create a collection.
    /// </summary>
    public override string CreateCollection { get; } = "session.getSchema('{0}').createCollection('{1}')";

    /// <summary>
    /// Gets a statement to create a non-unique index.
    /// </summary>
    public override string CreateNonUniqueIndexMovies { get; } = "coll.createIndex('" +  BaseCollectionTests.MOVIES_NON_UNIQUE_INDEX_NAME + "').field('rating', 'text(5)', true).execute()";

    /// <summary>
    /// Gets a statement to create a schema.
    /// </summary>
    public override string CreateSchema { get; } = "session.createSchema('{0}')";

    /// <summary>
    /// Gets a statement to create a unique index.
    /// </summary>
    public override string CreateUniqueIndexMovies { get; } = "coll.createIndex('" + BaseCollectionTests.MOVIES_UNIQUE_INDEX_NAME + "', mysqlx.IndexType.UNIQUE).field('title', 'text(255)', true).execute()";

    /// <summary>
    /// Gets a statement to drop a collection.
    /// </summary>
    public override string DropCollection { get; } = "session.dropCollection('{0}', '{1}')";

    /// <summary>
    /// Gets a statement to drop non-unique index.
    /// </summary>
    public override string DropNonUniqueIndexMovies { get; } = "coll.dropIndex('" + BaseCollectionTests.MOVIES_NON_UNIQUE_INDEX_NAME + "').execute()";

    /// <summary>
    /// Gets a statement to drop the test database
    /// </summary>
    public override string DropSchema { get; } = "session.dropSchema('{0}')";

    /// <summary>
    /// Gets a statement to drop unique index.
    /// </summary>
    public override string DropUniqueIndexMovies { get; } = "coll.dropIndex('" + BaseCollectionTests.MOVIES_UNIQUE_INDEX_NAME + "').execute()";

    /// <summary>
    /// Gets a specific colletion and assign it to a variable for persistence. 
    /// </summary>
    public override string GetCollection { get; } = "coll = session.getSchema('{0}').getCollection('{1}')";

    /// <summary>
    /// Gets a schema and assign it to a variable for persistence.
    /// </summary>
    public override string GetSchema { get; } = "schema = session.getSchema('{0}')";

    /// <summary>
    /// Gets a statement to modify a SakilaX.User value and append a value to a specific array.
    /// </summary>
    public override string ModifyArrayAppendUser { get; } = "coll.modify('name like :param1').arrayAppend('ratings', 'PG-13').bind('param1', 'Iggy%').execute()";

    /// <summary>
    /// Gets a statement to modify a SakilaX.User value and insert a value to a specific array in a specified position.
    /// </summary>
    public override string ModifyArrayDeleteUser { get; } = "coll.modify('name like :param1').arrayDelete('ratings[2]').bind('param1', 'Iggy%').execute()";

    /// <summary>
    /// Gets a statement to modify a SakilaX.User value and insert a value to a specific array in a specified position.
    /// </summary>
    public override string ModifyArrayInsertUser { get; } = "coll.modify('name like :param1').arrayInsert('ratings[2]', 'G').bind('param1', 'Iggy%').execute()";

    /// <summary>
    /// Gets a statement to modify a SakilaX.User value and set additional values.
    /// </summary>
    public override string ModifyMergeUser { get; } = "coll.modify('name like :param1').merge({ status: 'inactive', ratings: "
                                                      + BaseCollectionTests.MOVIES_RATING_ARRAY_VALUE
                                                      + " }).bind('param1', 'Iggy%').execute()";

    #endregion Collection Queries

    #region Session Queries

    /// <summary>
    /// Gets a statement to validate whether the active session is open.
    /// </summary>
    public override string IsSessionOpen { get; } = "session.isOpen()";

    /// <summary>
    /// Gets a statement to get the parsed Uri of the active session Uri.
    /// </summary>
    public override string ShellParseUriFromSessionUri { get; } = "shell.parseUri(session.getUri())";

    #endregion Session Queries

    /// <summary>
    /// Initializes a new instance of the <see cref="JsCollectionTestsProperties"/> class.
    /// </summary>
    /// <param name="scriptLanguage">A <see cref="ScriptLanguageType"/> value.</param>
    public JsCollectionTestsProperties(ScriptLanguageType scriptLanguage)
      : base(scriptLanguage)
    {
      CreateCollectionTest = string.Format(CreateCollection, BaseTests.TEMP_SCHEMA_NAME, BaseTests.TEST_COLLECTION_NAME);
      CreateSchemaTempSchema = string.Format(CreateSchema, BaseTests.TEMP_SCHEMA_NAME);
      DropCollectionTest = string.Format(DropCollection, BaseTests.TEMP_SCHEMA_NAME, BaseTests.TEST_COLLECTION_NAME);
      DropSchemaTempSchema = string.Format(DropSchema, BaseTests.TEMP_SCHEMA_NAME);
      GetSchemaTempSchema = string.Format(GetSchema, BaseTests.TEMP_SCHEMA_NAME);
      GetCollectionTestSchemaTest = string.Format(GetCollection, BaseTests.TEMP_SCHEMA_NAME, BaseTests.TEST_COLLECTION_NAME);
      GetCollectionXTestMovies = string.Format(GetCollection, BaseTests.X_TEST_SCHEMA_NAME, BaseTests.MOVIES_COLLECTION_NAME);
      GetCollectionXTextUser = string.Format(GetCollection, BaseTests.X_TEST_SCHEMA_NAME, BaseTests.USERS_COLLECTION_NAME);
      GetSchemaXTest = string.Format(GetSchema, BaseTests.X_TEST_SCHEMA_NAME);
      ScriptLanguage = ScriptLanguageType.JavaScript;
    }
  }
}
