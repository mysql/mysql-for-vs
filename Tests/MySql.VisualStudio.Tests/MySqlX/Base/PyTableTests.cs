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
  public abstract class PyTableTests : BaseTests
  {
    #region CommonShellQueries

    /// <summary>
    /// Statement to create the test database
    /// </summary>
    protected const string CREATE_TEST_DATABASE = "session.sql('create schema " + TEST_DATABASE_NAME + ";').execute()";

    /// <summary>
    /// Statement to create the test table
    /// </summary>
    protected const string CREATE_TEST_TABLE = "session.sql('create table " + TEST_TABLE_NAME + " (name varchar(50), age integer, gender varchar(20));').execute()";

    /// <summary>
    /// First statement to delete a record in the test table in multiple commands
    /// </summary>
    protected const string DELETE_RECORD_CMD1 = "table.delete().where(\"gender='male'\").execute()";

    /// <summary>
    /// Statement to delete a record in the test table in a single command
    /// </summary>
    protected const string DELETE_RECORD_SINGLE_LINE = "res = table.delete().where(\"gender='male'\").execute()";

    /// <summary>
    /// Statement to delete the test table
    /// </summary>
    protected const string DELETE_TEST_TABLE = "session.sql('drop table " + TEST_TABLE_NAME + ";').execute()";

    /// <summary>
    /// Statement to drop the test database
    /// </summary>
    protected const string DROP_TEST_DATABASE = "session.sql('drop schema if exists " + TEST_DATABASE_NAME + ";').execute()";

    /// <summary>
    /// Statement to insert a record to the test table
    /// </summary>
    protected const string INSERT_RECORD_JSON1 = "res = table.insert({'name' : 'jack', 'age' : 17, 'gender' : 'male'}).execute()";

    /// <summary>
    /// Statement to insert a record to the test table
    /// </summary>
    protected const string INSERT_RECORD_JSON2 = "res = table.insert({'name' : 'jacky', 'age' : 17, 'gender' : 'male'}).execute()";

    /// <summary>
    /// Statement to insert two records at the same time to the test table
    /// </summary>
    protected const string INSERT_TWO_RECORDS = "res = table.insert('name', 'age', 'gender').values('jack', 17,'male').values('jacky', 17,'male').execute()";

    /// <summary>
    /// Statement to get all the records from the test table as RowResult
    /// </summary>
    protected const string SELECT_FOR_TABLE_RESULT = "table.select().execute()";

    /// <summary>
    /// Statement to get all the records from the test table as DocResult
    /// </summary>
    protected const string SELECT_TEST_TABLE = "table.select().execute()";

    /// <summary>
    /// Statement to select the update record from the test table
    /// </summary>
    protected const string SELECT_UPDATED_RECORD = "table.select().where(\"name = 'jacky' and gender='female'\").execute();";

    /// <summary>
    /// Get and set the test database
    /// </summary>
    protected const string SET_SCHEMA_VAR = "schema = session.getSchema('" + TEST_DATABASE_NAME + "')";

    /// <summary>
    /// Get and set the test table
    /// </summary>
    protected const string SET_TABLE_VAR = "table = schema.getTable('" + TEST_TABLE_NAME + "')";

    /// <summary>
    /// First statement to update a record in the test table in multiple commands
    /// </summary>
    protected const string UPDATE_RECORD_CMD1 = "upd = table.update()";

    /// <summary>
    /// Second statement to update a record in the test table in multiple commands
    /// </summary>
    protected const string UPDATE_RECORD_CMD2 = "upd.set('gender', 'female').where(\"name = 'jacky'\");";

    /// <summary>
    /// Third statement to update a record in the test table in multiple commands
    /// </summary>
    protected const string UPDATE_RECORD_CMD3 = "upd.execute()";

    /// <summary>
    /// Statement to update a record in the test table in a single command
    /// </summary>
    protected const string UPDATE_RECORD_SINGLE_LINE = "res = table.update().set('gender', 'female').where(\"name = 'jacky'\").execute()";

    /// <summary>
    /// Statement to use the test database
    /// </summary>
    protected const string USE_TEST_DATABASE = "session.sql('use " + TEST_DATABASE_NAME + ";').execute()";

    #endregion CommonShellQueries
  }
}
