// Copyright (c) 2016, 2017, Oracle and/or its affiliates. All rights reserved.
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

using MySql.Data.MySqlClient;

namespace MySql.Utility.Enums
{
  /// <summary>
  /// Specifies identifiers to indicate the schema information queried from a MySQL Server.
  /// </summary>
  public enum SchemaInformationType
  {
    /// <summary>
    /// Character sets information given by a "SHOW CHARSET" statement.
    /// </summary>
    CharacterSets,

    /// <summary>
    /// Collations information related to a character set given by a "SHOW COLLATION" statement.
    /// </summary>
    Collations,

    /// <summary>
    /// Columns information of a table given by a "SHOW COLUMNS" statement.
    /// </summary>
    ColumnsSimple,

    /// <summary>
    /// Columns information of a table given by a "SHOW FULL COLUMNS" statement.
    /// </summary>
    ColumnsFull,

    /// <summary>
    /// Databases information in a server given by a "SHOW DATABASES" statement.
    /// </summary>
    Databases,

    /// <summary>
    /// Data source information (for internal use).
    /// </summary>
    DataSourceInformation,

    /// <summary>
    /// Information about data types supported by Connector/NET.
    /// </summary>
    DataTypes,

    /// <summary>
    /// Database storage engines information stored in the information schema's ENGINES table.
    /// </summary>
    Engines,

    /// <summary>
    /// Information about columns in foreign keys.
    /// </summary>
    ForeignKeyColumns,

    /// <summary>
    /// Information about foreign key relationships.
    /// </summary>
    ForeignKeys,

    /// <summary>
    /// Information about columns in indexes.
    /// </summary>
    IndexColumns,

    /// <summary>
    /// Information about indexes.
    /// </summary>
    Indexes,

    /// <summary>
    /// Information about the schema collections that can be passed in the first paramenter of a <see cref="MySqlConnection.GetSchema(string, string[])"/> call.
    /// </summary>
    MetaDataCollections,

    /// <summary>
    /// Information about stored procedure parameters.
    /// </summary>
    ProcedureParameters,

    /// <summary>
    /// Information about stored procedures in a schema.
    /// </summary>
    Procedures,

    /// <summary>
    /// Information about stored procedures and their parameters.
    /// </summary>
    ProceduresWithParameters,

    /// <summary>
    /// Information about the restrictions that can be passed in the second paramenter of a <see cref="MySqlConnection.GetSchema(string, string[])"/> call.
    /// </summary>
    Restrictions,

    /// <summary>
    /// Information about MySQL reserved words.
    /// </summary>
    ReservedWords,

    /// <summary>
    /// Information about stored procedures in a schema.
    /// </summary>
    /// <remarks>Normally <see cref="Procedures"/> should be used but it does not currenlty work with 8.0 Servers using C/NET 6.9.9.</remarks>
    Routines,

    /// <summary>
    /// Information about tables in a schema.
    /// </summary>
    Tables,

    /// <summary>
    /// Information about triggers defined in the database.
    /// </summary>
    Triggers,

    /// <summary>
    /// Information about User Defined Functions, stored in the MYSQL.FUNC table.
    /// </summary>
    UDF,

    /// <summary>
    /// Information about users in the Server, stored in the MYSQL.USER table.
    /// </summary>
    Users,

    /// <summary>
    /// Information about columns in a view.
    /// </summary>
    ViewColumns,

    /// <summary>
    /// Information about views in a schema.
    /// </summary>
    Views
  }
}
