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
  /// Defines properties specific to table tests.
  /// </summary>
  public abstract class TableTestsProperties
  {
    #region Table Queries

    /// <summary>
    /// Gets a statement to delete a record using parameter binding.
    /// </summary>
    public abstract string DeleteWithLimit { get; }

    /// <summary>
    /// Statement to enable the use of mysqlx within script.
    /// </summary>
    public abstract string IncludeMysqlx { get; }

    /// <summary>
    /// Gets the test schema and assigns it to a variable for persistence.
    /// </summary>
    public abstract string GetSchema { get; }

    /// <summary>
    /// Get a specific table and assign it to a variable for persistence. 
    /// </summary>
    public abstract string GetTable { get; }

    /// <summary>
    /// Get a specific table and assign it to a variable for persistence. 
    /// </summary>
    public abstract string GetTableInSchema { get; }

    /// <summary>
    /// Gets a list of all tables in the current schema.
    /// </summary>
    public abstract string GetTables { get; }

    /// <summary>
    /// Statement to insert JSON documents into a table.
    /// </summary>
    public abstract string InsertJsonDocument1 { get; }

    /// <summary>
    /// Statement to insert JSON documents into a table.
    /// </summary>
    public abstract string InsertJsonDocument2 { get; }

    /// <summary>
    /// Gets a statement to get all records ordered by a criteria.
    /// </summary>
    public abstract string SelectWithOrderByDesc { get; }

    /// <summary>
    /// Checks if a table object corresponds actually a view.
    /// </summary>
    public abstract string TableIsView { get; }

    #endregion Table Queries

    #region Properties

    /// <summary>
    /// Gets a statement to create the test database.
    /// </summary>
    public string CreateTestDatabase { get; protected set; }

    /// <summary>
    /// Gets a statement to drop the test database.
    /// </summary>
    public string DropTestDatabaseIfExists { get; protected set; }

    /// <summary>
    /// Gets a statement to drop the test table if it already exists.
    /// </summary>
    public string DropTestTableIfExists { get; protected set; }

    /// <summary>
    /// Gets a statement to get the database_test schema.
    /// </summary>
    public string GetDatabaseTest { get; protected set; }

    /// <summary>
    /// Gets a statement to get the table_test collection from the database_test schema.
    /// </summary>
    public string GetDatabaseTestTableTest { get; protected set; }

    /// <summary>
    /// Gets a statement to get the character collection from the x_test schema.
    /// </summary>
    public string GetTableXTestCharacter { get; protected set; }

    /// <summary>
    /// Gets the language used for the tests.
    /// </summary>
    public ScriptLanguageType ScriptLanguage { get; protected set; }

    /// <summary>
    /// Gets a statement to use the x_test database.
    /// </summary>
    public string UseXTestDatabase { get; protected set; }

    /// <summary>
    /// Gets a statement to use the test database.
    /// </summary>
    public string UseTestDatabase { get; protected set; }

    #endregion Properties

    /// <summary>
    /// Initializes a new instance of the <see cref="TableTestsProperties"/> class.
    /// </summary>
    /// <param name="scriptLanguage">A <see cref="ScriptLanguageType"/> value.</param>
    protected TableTestsProperties(ScriptLanguageType scriptLanguage)
    {
      CreateTestDatabase = string.Format(BaseTableTests.CREATE_DATABASE, BaseTests.TEMP_TEST_DATABASE_NAME);
      DropTestDatabaseIfExists = string.Format(BaseTests.DROP_DATABASE_IF_EXISTS, BaseTests.TEMP_TEST_DATABASE_NAME);
      DropTestTableIfExists = string.Format(BaseTableTests.DROP_TABLE_IF_EXISTS, BaseTableTests.TEST_TABLE_NAME);
      UseXTestDatabase = string.Format(BaseTableTests.USE_DATABASE, BaseTests.X_TEST_SCHEMA_NAME);
      UseTestDatabase = string.Format(BaseTableTests.USE_DATABASE, BaseTests.TEMP_TEST_DATABASE_NAME);
      ScriptLanguage = scriptLanguage;
    }
  }
}
