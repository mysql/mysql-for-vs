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
  public abstract class PyCollectionTests : BaseTests
  {
    #region CommonShellQueries

    /// <summary>
    /// Statement to insert a record to the test table
    /// </summary>
    protected const string ADD_JSON_DOCUMENT1 = "result = coll.add({'name' : 'my first', 'passed' : 'document', 'count' : 1}).execute()";

    /// <summary>
    /// Statement to insert a record to the test table
    /// </summary>
    protected const string ADD_JSON_DOCUMENT2 = "result = coll.add({'name' : 'my second', 'passed' : 'again', 'count' : 2}).execute()";

    /// <summary>
    /// Statement to insert multiple records at the same time on a single statement to the test collection
    /// </summary>
    protected const string ADD_MULTIPLE_DOCUMENTS_MULTIPLE_ADD_STATEMENTS = "result = coll.add({'name' : 'my sixth', 'passed' : 'aaaand again', 'count' : 6}).add({'name' : 'my seventh', 'passed' : 'aaaand once again', 'count' : 7}).execute()";

    /// <summary>
    /// Statement to insert multiple records at the same time on a single statement to the test collection
    /// </summary>
    protected const string ADD_MULTIPLE_DOCUMENTS_SINGLE_ADD_STATEMENT = "result = coll.add([{'name' : 'my third', 'passed' : 'once again', 'count' : 3},{'name': 'my fourth', 'passed' : 'and again', 'count' : 4}, {'name' : 'my fifth', 'passed' : 'and finally', 'count' : 5}]).execute()";

    /// <summary>
    /// Statement to create the test table
    /// </summary>
    protected const string CREATE_COLLECTION_TEST = "session." + TEST_SCHEMA_NAME + ".createCollection('" + TEST_COLLECTION_NAME + "')";

    /// <summary>
    /// Statement to create the test database
    /// </summary>
    protected const string CREATE_SCHEMA_TEST = "session.createSchema('" + TEST_SCHEMA_NAME + "')";

    //TODO: [MYSQLFORVS-414] Adjust this test for when this method is fully implemented in x-Shell for the JS sintaxis. It should look like:
    //private const string _deleteCollectionTest = "session." + _testCollectionName + ".drop();";
    /// <summary>
    /// Statement to delete the test table
    /// </summary>
    protected const string DELETE_COLLECTION_TEST = "session.sql('drop table " + TEST_SCHEMA_NAME + "." + TEST_COLLECTION_NAME + "').execute()";

    /// <summary>
    /// Statement to drop the test database
    /// </summary>
    protected const string DROP_SCHEMA_IF_EXISTS = "session.sql('drop schema if exists " + TEST_SCHEMA_NAME + ";').execute()";

    /// <summary>
    /// Statement to drop the test database
    /// </summary>
    protected const string DROP_SCHEMA_TEST = "session.dropSchema('" + TEST_SCHEMA_NAME + "')";

    /// <summary>
    /// Statement to get all the records from the test table as RowResult
    /// </summary>
    protected const string FIND_ALL_DOCUMENTS_IN_COLLECTION = "coll.find().execute()";

    /// <summary>
    /// Statement to select the update record from the test table
    /// </summary>
    protected const string FIND_SPECIFIC_DOCUMENT_TEST = "coll.find(\"name like 'my second'\").execute()";

    /// <summary>
    /// Statement to update a record in the test table.
    /// </summary>
    protected const string MODIFY_DOCUMENT1 = "modr = coll.modify(\"name like 'my fourth'\")";

    /// <summary>
    /// Statement to update a record in the test table.
    /// </summary>
    protected const string MODIFY_DOCUMENT2 = "setr = modr.set('name','dummy')";

    /// <summary>
    /// Statement to update a record in the test table.
    /// </summary>
    protected const string MODIFY_DOCUMENT3 = "sortr = setr.sort(['name']).execute()";

    /// <summary>
    /// Statement to delete a record in the test table in a single command
    /// </summary>
    protected const string REMOVE_DOCUMENT = "coll.remove(\"name like 'my fifth'\").execute()";

    /// <summary>
    /// Statement to select the update record from the test table
    /// </summary>
    protected const string SELECT_UPDATED_RECORD = "coll.find('name like \"dummy\"').execute()";

    /// <summary>
    /// Get and set the test table
    /// </summary>
    protected const string SET_COLLECTION_VAR = "coll = session." + TEST_SCHEMA_NAME + ".getCollection('" + TEST_COLLECTION_NAME + "')";

    /// <summary>
    /// Get and set the test database
    /// </summary>
    protected const string SET_SCHEMA_VAR = "schema = session.getSchema('" + TEST_SCHEMA_NAME + "')";

    /// <summary>
    /// Statement to use the test database
    /// </summary>
    protected const string USE_SCHEMA_TEST = "session.sql('use " + TEST_SCHEMA_NAME + ";').execute()";

    #endregion CommonShellQueries
  }
}
