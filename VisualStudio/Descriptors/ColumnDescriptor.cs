// Copyright (C) 2006-2007 MySQL AB
//
// This file is part of MySQL Tools for Visual Studio.
// MySQL Tools for Visual Studio is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public 
// License version 2.1 as published by the Free Software Foundation
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA using System;

/*
 * This file contains implementation for the column database object descriptor.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using MySql.Data.VisualStudio.Utils;
using MySql.Data.VisualStudio.Properties;
using System.Diagnostics;

namespace MySql.Data.VisualStudio.Descriptors
{
    /// <summary>
    /// This is the descriptor for column database object.
    /// </summary>
    [ObjectDescriptor(ColumnDescriptor.TypeName, typeof(ColumnDescriptor))]
    [IdLength(4)]
    public class ColumnDescriptor: ObjectDescriptor
    {
        /// <summary>
        /// Object type name.
        /// </summary>
        public new const string TypeName = "Column";

        #region Enumerate SQL
        /// <summary>
        /// Column enumeration template.
        /// </summary>
        protected new const string EnumerateSqlTemplate =
                "SELECT "
                + "TABLE_CATALOG, "
                + "TABLE_SCHEMA, "
                + "TABLE_NAME, "
                + "COLUMN_NAME, "
                + "ORDINAL_POSITION, "
                + "COLUMN_DEFAULT, "
                + "IS_NULLABLE, "
                + "DATA_TYPE, "
                + "CHARACTER_MAXIMUM_LENGTH, "
                + "CHARACTER_OCTET_LENGTH, "
                + "NUMERIC_PRECISION, "
                + "NUMERIC_SCALE, "
                + "CHARACTER_SET_NAME, "
                + "COLLATION_NAME, "
                + "COLUMN_TYPE, "
                + "COLUMN_KEY, "
                + "EXTRA, "
                + "`PRIVILEGES`, "
                + "COLUMN_COMMENT, "
                + "IF(DATA_TYPE = 'bit', IF(COLUMN_DEFAULT IS NULL, NULL, IF(ASCII(COLUMN_DEFAULT) = 1 OR COLUMN_DEFAULT = '1', 1, 0)), COLUMN_DEFAULT) AS TRUE_DEFAULT,"
                + "TRIM(TRAILING ' unsigned' FROM TRIM(TRAILING ' zerofill' FROM COLUMN_TYPE)) AS MYSQL_TYPE, "
                + "IF(COLUMN_KEY = 'PRI', 'YES', 'NO') AS `IsPrimaryKey`, "
                + "IF(COLUMN_TYPE LIKE '% zerofill%', 'YES', 'NO') AS `ZEROFILL`, "
                + "IF(COLUMN_TYPE LIKE '% unsigned%', 'YES', 'NO') AS `UNSIGNED`, "
                + "IF(COLLATION_NAME LIKE '%_bin', 'YES', 'NO') AS `BINARY` "
                + "FROM information_schema.`COLUMNS`"
                + "WHERE TABLE_SCHEMA = {1} AND TABLE_NAME = {2} AND COLUMN_NAME = {3}";
        
        /// <summary>
        /// Column enumeration default.
        /// </summary>
        protected new static readonly string[] DefaultRestrictions =
		{
			"",
            "TABLE_SCHEMA",
			"TABLE_NAME",
			"COLUMN_NAME"			
		};

        /// <summary>
        /// Column default sort fields.
        /// </summary>
        protected new const string DefaultSortString = ""; 
        #endregion

        #region Attributes
        /// <summary>
        /// List of known attributes for Column object
        /// </summary>
        public static new class Attributes
        {
            [Field(FieldType = TypeCode.String)]
            public const string Database = "TABLE_CATALOG";
            [Field(FieldType = TypeCode.String)]
            [Identifier(IsSchema=true)]
            public const string Schema = "TABLE_SCHEMA";
            [Field(FieldType = TypeCode.String)]
            [Identifier]
            public const string Table = "TABLE_NAME";
            [Field(FieldType = TypeCode.String)]
            [Identifier(IsName=true)]
            public const string Name = "COLUMN_NAME";
            [Field(OptionName = OrdinalOption, FieldType = TypeCode.Int64)]
            public const string Ordinal = "ORDINAL_POSITION";
            [Field(FieldType = TypeCode.String)]
            public const string Default = "COLUMN_DEFAULT";
            [Field(FieldType = TypeCode.Boolean)]
            public const string Nullable = "IS_NULLABLE";
            [Field(OptionName = DataTypeOption, FieldType = TypeCode.String)]
            public const string SqlType = "DATA_TYPE";
            [Field(OptionName = "ch_max_length", FieldType = TypeCode.Int64)]
            public const string Length = "CHARACTER_MAXIMUM_LENGTH";            
            [Field(OptionName = "octet_max_length", FieldType = TypeCode.Int64)]
            public const string OctetLength = "CHARACTER_OCTET_LENGTH";
            [Field(OptionName = "precision", FieldType = TypeCode.Int64)]
            public const string Precision = "NUMERIC_PRECISION";
            [Field(OptionName = "scale", FieldType = TypeCode.Int64)]
            public const string Scale = "NUMERIC_SCALE";
            [Field(OptionName = CharacterSetOption, FieldType = TypeCode.String)]
            public const string CharacterSet = "CHARACTER_SET_NAME";
            [Field(OptionName = CollationOption, FieldType = TypeCode.String)]
            public const string Collation = "COLLATION_NAME";
            [Field(FieldType = TypeCode.String)]
            public const string ColumnType = "COLUMN_TYPE";
            [Field(FieldType = TypeCode.String)]
            public const string ColumnKey = "COLUMN_KEY";
            [Field(FieldType = TypeCode.String)]
            [OptionString]
            public const string Extra = "EXTRA";
            [Field(OptionName = "privileges", FieldType = TypeCode.String)]
            public const string Privileges = "PRIVILEGES";
            [Field(OptionName = "comments", FieldType = TypeCode.String)]
            public const string Comments = "COLUMN_COMMENT";
            [Field(OptionName = MySqlTypeOption, FieldType = TypeCode.String)]
            public const string MySqlType = "MYSQL_TYPE";
            [Field(OptionName = PrimaryKeyOption, FieldType = TypeCode.Boolean)]
            public const string IsPrimaryKey = "IsPrimaryKey";
            [Field(OptionName = "auto_increment", FieldType = TypeCode.Boolean)]
            public const string IsAutoIncrement = "IsAutoIncrement";
            [Field(OptionName = UnsignedOption, FieldType = TypeCode.Boolean)]
            public const string Unsigned = "UNSIGNED";
            [Field(OptionName = ZerofillOption, FieldType = TypeCode.Boolean)]
            public const string Zerofill = "ZEROFILL";
            [Field(OptionName = BinaryOption, FieldType = TypeCode.Boolean)]
            public const string Binary = "BINARY";
            [Field(OptionName = "ascii", FieldType = TypeCode.String)]
            public const string Ascii = "ASCII";
            [Field(OptionName = "unicode", FieldType = TypeCode.String)]
            public const string Unicode = "UNICODE";
        }
        #endregion

        #region Legacy MySQL version support
        /// <summary>
        /// Builds enumerate SQL query for object of this type with given restrictions. For legacy version uses
        /// SHOW TABLE STATUS instead INFORMATION_SCHEMA.
        /// </summary>
        /// <param name="connection">The DataConnectionWrapper to be used for enumeration.</param>
        /// <param name="restrictions">Restrictions to enumerated objects.</param>
        /// <param name="sort">Sort expression to use.</param>
        /// <returns>Enumerating SQL query string.</returns>
        protected override string BuildEnumerateSql(DataConnectionWrapper connection, object[] restrictions, string sort)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            // Extract server version
            Version serverVersion = connection.ServerVersion;

            // For latest version just call base
            if (serverVersion == null || serverVersion.Major >= 5)
                return base.BuildEnumerateSql(connection, restrictions, sort);

            // Check restrictions for table
            if (restrictions == null || restrictions.Length < 3 || String.IsNullOrEmpty(restrictions[2] as string))
                throw new NotSupportedException(Resources.Error_EnumeratingAllColumns);
            
            // Build SHOW TABLE STATUS
            StringBuilder query = new StringBuilder("SHOW COLUMNS FROM ");
            
            // Write table restrictions
            QueryBuilder.WriteIdentifier(restrictions[2] as string, query);

            // If there is a restriction on schema, apply it
            if (restrictions != null && restrictions.Length >= 2 && !String.IsNullOrEmpty(restrictions[1] as string))
            {
                query.Append(" FROM ");
                QueryBuilder.WriteIdentifier(restrictions[1] as string, query);
            }

            // If there is a restriction on column, apply it
            if (restrictions != null && restrictions.Length >= 4)
                QueryBuilder.WriteIfNotEmptyString(restrictions[3], " LIKE ", query);

            // Return result
            return query.ToString();
        }

        /// <summary>
        /// Reads table with Database Objects which satisfy given restriction. Base implementation 
        /// uses direct SQL query to the INFORMATION_SCHEMA.
        /// </summary>
        /// <param name="connection">The DataConnectionWrapper to be used for enumeration.</param>
        /// <param name="restrictions">The restrictions to be putted on the retrieved objects set.</param>
        /// <param name="sort">Sort expresion to append after ORDER BY clause.</param>
        /// <returns>Returns table with Database Objects which satisfy given restriction.</returns>
        protected override DataTable ReadTable(DataConnectionWrapper connection, object[] restrictions, string sort)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            // Extract server version
            Version serverVersion = connection.ServerVersion;

            // For latest version just return base result
            if (serverVersion == null || serverVersion.Major >= 5)
                return base.ReadTable(connection, restrictions, sort);
            

            // Temporary table to store result
            DataTable merged = new DataTable();

            // Enumerate all tables
            DataTable tables = TableDescriptor.Enumerate(connection, restrictions);
            // If there is now tables, return empty table with columns
            if (tables == null || tables.Rows == null || tables.Rows.Count <= 0)
                return merged;                
            
            // Calculate column restrriction
            object column = restrictions != null && restrictions.Length >= 4 ? restrictions[3] : null;

            // Enumerate columns for each table
            foreach (DataRow tableRow in tables.Rows)
            {
                // Enumerate columns
                DataTable columns = ReadColumnsForTable(
                    connection,
                    new object[] {
                        null, 
                        DataInterpreter.GetString(tableRow, TableDescriptor.Attributes.Schema),
                        DataInterpreter.GetString(tableRow, TableDescriptor.Attributes.Name),
                        column },
                    sort);
                
                // Merge results if any
                if (columns != null)
                    merged.Merge(columns);
            }

            // Return results
            return merged;
        }

        /// <summary>
        /// Returns DataTable with description of table's columns.
        /// </summary>
        /// <param name="connection">Connection object to use for queries.</param>
        /// <param name="restrictions">Restrictions on table and propably on column.</param>
        /// <param name="sort">Sort string to use.</param>
        /// <returns>Returns DataTable with description of table's columns.</returns>
        private DataTable ReadColumnsForTable(DataConnectionWrapper connection, object[] restrictions, string sort)
        {
            // Execute base method
            DataTable result = base.ReadTable(connection, restrictions, sort);

            // If result is null, exit
            if (result == null)
                return null;


            // Extract table name
            string table = restrictions[2] as string;

            // For legacy version rename columns
            RenameColumn("Field", Attributes.Name, result);
            RenameColumn("Type", Attributes.ColumnType, result);
            RenameColumn("Null", Attributes.Nullable, result);
            RenameColumn("Key", Attributes.ColumnKey, result);
            RenameColumn("Default", Attributes.Default, result);
            RenameColumn("Extra", Attributes.Extra, result);

            // Calculate schema name
            string schema;
            if (restrictions.Length >= 2 && !String.IsNullOrEmpty(restrictions[1] as string))
                schema = restrictions[1] as string;
            else
                schema = connection.Schema;

            // Add catalog, schema and type column
            result.Columns.Add(Attributes.Schema, typeof(string));
            result.Columns.Add(Attributes.Table, typeof(string));

            // Set schema and table name for each row
            foreach (DataRow column in result.Rows)
            {
                DataInterpreter.SetValueIfChanged(column, Attributes.Schema, schema);
                DataInterpreter.SetValueIfChanged(column, Attributes.Table, table);
                // Empty value in IS_NULABLE column means NO
                if (!DataInterpreter.IsNotEmptyString(column, Attributes.Nullable))
                    DataInterpreter.SetValueIfChanged(column, Attributes.Nullable, DataInterpreter.False);
            }

            // Finaly, return result
            return result;
        }

        /// <summary>
        /// Extracts option values for given DataRow. For legacy version calculate aditional fields by parsing type
        /// </summary>
        /// <param name="connection">The DataConnectionWrapper to be used for enumeration.</param>
        /// <param name="row">DataRow to extract values.</param>
        /// <returns>Returns field values for given DataRow.</returns>
        protected override Dictionary<string, string> ExtractOptions(DataConnectionWrapper connection, DataRow row)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (row == null)
                throw new ArgumentNullException("row");

            // Call base method
            Dictionary<string,string> result = base.ExtractOptions(connection, row);

            // Extract server version
            Version serverVersion = connection.ServerVersion;

            // For latest version just return base result
            if (serverVersion == null || serverVersion.Major >= 5)
                return result;

            // Add an ordinal option
            result[OrdinalOption] = ExtractRowIndex(row);

            // Extract data type
            string datatype = DataInterpreter.GetStringNotNull(row, Attributes.ColumnType);
            Debug.Assert(datatype.Length > 0, "Empty data type!");

            // Calculate MySQL type
            string mySqlType = datatype;
            // Remove unsigned
            int pos = mySqlType.IndexOf(" unsigned");
            if (pos >= 0)
                mySqlType = mySqlType.Remove(pos);
            // Remove zerofill
            pos = mySqlType.IndexOf(" zerofill");
            if (pos >= 0)
                mySqlType = mySqlType.Remove(pos);
            // Add MySQL type option
            Debug.Assert(!String.IsNullOrEmpty(mySqlType), "MySQL type is empty!");
            result[MySqlTypeOption] = mySqlType;

            // Add a primary key option
            if (DataInterpreter.CompareInvariant("PRI", DataInterpreter.GetStringNotNull(row, Attributes.ColumnKey)))
                result[PrimaryKeyOption] = DataInterpreter.True;

            // Add an unsigned option
            if (datatype.IndexOf(" unsigned") >= 0)
                result[UnsignedOption] = DataInterpreter.True;

            // Add a zerofill option
            if (datatype.IndexOf(" zerofill") >= 0)
                result[ZerofillOption] = DataInterpreter.True;

            // Add a datatype option
            result[DataTypeOption] = datatype;

            // TODO: Parse create table for right character set
            // Add a character set option
            if (!String.IsNullOrEmpty(connection.DefaultCharacterSet))
                result[CharacterSetOption] = connection.DefaultCharacterSet;
            
            // Add collation field
            if (!String.IsNullOrEmpty(connection.DefaultCollation))
                result[CollationOption] = connection.DefaultCollation;

            // Finaly, return result
            return result;
        }

        /// <summary>
        /// Returns string with row ordinal position in the table.
        /// </summary>
        /// <param name="row">DataRow to process.</param>
        /// <returns>Returns string with row ordinal position in the table.</returns>
        private string ExtractRowIndex(DataRow row)
        {
            // Extract and check table
            DataTable table = row.Table;
            if (table == null || table.Rows == null)
            {
                Debug.Fail("Detached rows are unsupported!");
                return String.Empty;
            }

            // TODO: This way may fail if restriction on the column name was applied.
            // Search for row
            for (int i = 0; i < table.Rows.Count; i++)
                if (table.Rows[i] == row)
                    return (i+1).ToString();

            // This should never happens
            Debug.Fail("Failed to find row!");
            return String.Empty;
        }
        #endregion

        #region Option names
        private const string OrdinalOption = "ordinal";
        private const string DataTypeOption = "data_type";
        private const string CharacterSetOption = "character_set";
        private const string CollationOption = "collation";
        private const string MySqlTypeOption = "mysql_type";
        private const string PrimaryKeyOption = "primary_key";
        private const string UnsignedOption = "unsigned";
        private const string ZerofillOption = "zerofill";
        private const string BinaryOption = "binary";
        #endregion

        #region Overridings
        /// <summary>
        /// Resets default for not nullable columns to empty string if it is null. Just to get behavior
        /// more like Query Browser.
        /// </summary>
        /// <param name="connection">The DataConnectionWrapper to be used for enumeration.</param>
        /// <param name="table">Table with data to post process.</param>>
        protected override void PostProcessData(DataConnectionWrapper connection, DataTable table)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (table == null)
                throw new ArgumentNullException("table");

            // Call to base
            base.PostProcessData(connection, table);

            // Do not process table whithout rows
            if(table.Rows == null)
                return;

            // Reset default value for each not nullable column to empty string if
            // it is null
            foreach (DataRow column in table.Rows)
            {
                if (DataInterpreter.GetSqlBool(column, Attributes.Nullable).IsFalse
                    && !DataInterpreter.IsNotNull(column, Attributes.Default))
                    DataInterpreter.SetValueIfChanged(column, Attributes.Default, String.Empty);
            }
        }
        #endregion

        #region Enumerate method
        /// <summary>
        /// Enumerates columns with given restrictions into DataTable.
        /// </summary>
        /// <param name="connection">The DataConnectionWrapper to be used for enumeration.</param>
        /// <param name="restrictions">The restrictions to be putted on the retrieved objects set.</param>
        /// <returns>
        /// Returns DataTable which contains all columns which satisfy given restrictions.
        /// </returns>
        public static DataTable Enumerate(DataConnectionWrapper connection, object[] restrictions)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            return ObjectDescriptor.EnumerateObjects(connection, TypeName, restrictions);
        }
        #endregion
    }
}
