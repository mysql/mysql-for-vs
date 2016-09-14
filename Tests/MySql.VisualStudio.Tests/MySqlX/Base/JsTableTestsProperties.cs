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
  /// Defines properties specific to table tests in JavaScript.
  /// </summary>
  public class JsTableTestsProperties : TableTestsProperties
  {
    #region Table Queries

    /// <summary>
    /// Gets a statement to delete a record using parameter binding.
    /// </summary>
    public override string DeleteWithLimit { get; } = "table.delete().where('age > :param1 and not base').orderBy(['age']).limit(2).bind('param1', 30).execute()";

    /// <summary>
    /// Gets a statement to enable the use of mysqlx within script.
    /// </summary>
    public override string IncludeMysqlx { get; } = "var mysqlx = require('mysqlx')";

    /// <summary>
    /// Gets the test schema and assigns it to a variable for persistence.
    /// </summary>
    public override string GetSchema { get; } = "schema = session.getSchema('{0}')";

    /// <summary>
    /// Gets a specific table and assign it to a variable for persistence. 
    /// </summary>
    public override string GetTable { get; } = "table = session.getSchema('{0}').getTable('{1}')";

    /// <summary>
    /// Gets a specific table and assign it to a variable for persistence. 
    /// </summary>
    public override string GetTableInSchema { get; } = "table = schema.getTable('{0}')";

    /// <summary>
    /// Gets a list of all tables in the current schema.
    /// </summary>
    public override string GetTables { get; } = "session.getSchema('{0}').getTables()";

    /// <summary>
    /// Statement to insert JSON documents into a table.
    /// </summary>
    public override string InsertJsonDocument1 { get; } = "table.insert({ age: 28, gender: 'female', name: 'Ashley Williams', universe: 'Mass Effect' }).execute();";

    /// <summary>
    /// Gets a statement to insert JSON documents into a table.
    /// </summary>
    public override string InsertJsonDocument2 { get; } = "table.insert({ age: 850, gender: 'female', name: 'Samara', universe: 'Mass Effect 2' }).execute();";

    /// <summary>
    /// Gets a statement to get all records ordered by a criteria.
    /// </summary>
    public override string SelectWithOrderByDesc { get; } = "table.select().orderBy(['age DESC']).execute()";

    /// <summary>
    /// Gets a statement checks if a table object corresponds actually a view.
    /// </summary>
    public override string TableIsView { get; } = "table.isView()";

    #endregion Table Queries

    /// <summary>
    /// Initializes a new instance of the <see cref="JsTableTestsProperties"/> class.
    /// </summary>
    /// <param name="scriptLanguage">A <see cref="ScriptLanguageType"/> value.</param>
    public JsTableTestsProperties(ScriptLanguageType scriptLanguage)
      : base(scriptLanguage)
    {
      GetDatabaseTest = string.Format(GetSchema, BaseTests.TEMP_TEST_DATABASE_NAME);
      GetDatabaseTestTableTest = string.Format(GetTableInSchema, BaseTableTests.TEST_TABLE_NAME);
      GetTableXTestCharacter = string.Format(GetTable, BaseTests.X_TEST_SCHEMA_NAME, BaseTests.CHARACTER_TABLE_NAME);
    }
  }
}
