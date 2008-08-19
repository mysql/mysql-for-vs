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
 * This file contains implementation of DataInterpreter utility.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Globalization;
using System.Data.SqlTypes;
using System.ComponentModel;
using System.Diagnostics;
using MySql.Data.VisualStudio.Properties;

namespace MySql.Data.VisualStudio.Utils
{
    /// <summary>
    /// This class contains several static methods, used to interpret data, 
    /// retrieved from database schema information.
    /// </summary>
    static class DataInterpreter
    {
        #region Constants
        /// <summary>
        /// Represents string value for true
        /// </summary>
        public const string True = "YES";
        /// <summary>
        /// Represents string value for false
        /// </summary>
        public const string False = "NO"; 
        #endregion

        #region Data row version specific
        /// <summary>
        /// Converts given column of given row to boolean value. "YES" string 
        /// interpreted as True and "NO" as False. Over values interpreted as
        /// Null.
        /// </summary>
        /// <param name="row">DataRow with values to check.</param>
        /// <param name="column">Column name.</param>
        /// <param name="version">Data row version, used to retrieve value.</param>
        /// <returns>
        /// Returns True, if value of given column of given row equivalent to "YES",
        /// False, if value of given column of given row equivalent to "NO", and
        /// Null otherwise.
        /// </returns>
        public static SqlBoolean GetSqlBool(DataRow row, string column, DataRowVersion version)
        {
            if (row == null)
                throw new ArgumentNullException("row");
            if (String.IsNullOrEmpty(column))
                throw new ArgumentException(Resources.Error_EmptyString, "column");

            // If column is boolean
            if (row[column, version] is bool)
                return new SqlBoolean((bool)row[column, version]);

            // Else, if column is string
            if (CompareInvariant(row[column, version] as string, True))
                return SqlBoolean.True;
            if (CompareInvariant(row[column, version] as string, False))
                return SqlBoolean.False;

            return SqlBoolean.Null;
        }

        /// <summary>
        /// Converts given column of given row to string value.
        /// </summary>
        /// <param name="row">DataRow with values to check.</param>
        /// <param name="column">Column name.</param>
        /// <param name="version">Data row version, used to retrieve value.</param>
        /// <returns>
        /// Returns string representation of the value or null, if value is empty.
        /// </returns>
        public static string GetString(DataRow row, string column, DataRowVersion version)
        {
            if (row == null)
                throw new ArgumentNullException("row");
            if (String.IsNullOrEmpty(column))
                throw new ArgumentException(Resources.Error_EmptyString, "column");

            // Get object value
            object value = row[column, version];

            return value != null && !(value is DBNull) ? value.ToString() : null;
        }

        /// <summary>
        /// Converts given column of given row to string value.
        /// </summary>
        /// <param name="row">DataRow with values to check.</param>
        /// <param name="column">Column name.</param>
        /// <param name="version">Data row version, used to retrieve value.</param>
        /// <returns>
        /// Returns string representation of the value or String.Empty, if value is empty.
        /// </returns>
        public static string GetStringNotNull(DataRow row, string column, DataRowVersion version)
        {
            if (row == null)
                throw new ArgumentNullException("row");
            if (String.IsNullOrEmpty(column))
                throw new ArgumentException(Resources.Error_EmptyString, "column");

            // Get object value
            object value = row[column, version];

            return value != null && !(value is DBNull) ? value.ToString() : String.Empty;
        }

        /// <summary>
        /// Converts given column of given row to integer value. Uses platform 
        /// depended conversion.
        /// </summary>
        /// <param name="row">DataRow with values to check.</param>
        /// <param name="column">Column name.</param>
        /// <param name="version">Data row version, used to retrieve value.</param>
        /// <returns>
        /// Returns converted to int value.
        /// </returns>
        public static Nullable<Int64> GetInt(DataRow row, string column, DataRowVersion version)
        {
            if (row == null)
                throw new ArgumentNullException("row");
            if (String.IsNullOrEmpty(column))
                throw new ArgumentException(Resources.Error_EmptyString, "column");

            // Get object value
            object value = row[column, version];

            // If type is correct return "as is"
            if (IsInteger(value))
                return (Int64)value;

            // Try to convert using converter
            if (value != null && converter.CanConvertFrom(value.GetType()))
                return (Int64)converter.ConvertFrom(value);

            // Try to convert to string and parse
            Int64 result;
            if (Int64.TryParse(value.ToString(), out result))
                return result;            

            // Failed to get integer
            return null;
        }
        #endregion

        #region Data row default version
        /// <summary>
        /// Converts given column of given row to boolean value. "YES" string 
        /// interpreted as True and "NO" as False. Over values interpreted as
        /// Null. Works with default data row version.
        /// </summary>
        /// <param name="row">DataRow with values to check.</param>
        /// <param name="column">Column name.</param>
        /// <returns>
        /// Returns True, if value of given column of given row equivalent to "YES",
        /// False, if value of given column of given row equivalent to "NO", and
        /// Null otherwise.
        /// </returns>
        public static SqlBoolean GetSqlBool(DataRow row, string column)
        {
            if (row == null)
                throw new ArgumentNullException("row");
            if (String.IsNullOrEmpty(column))
                throw new ArgumentException(Resources.Error_EmptyString, "column");

            return DataInterpreter.GetSqlBool(row, column, DataRowVersion.Default);
        }

        /// <summary>
        /// Converts given column of given row to string value. Works with 
        /// default data row version.
        /// </summary>
        /// <param name="row">DataRow with values to check.</param>
        /// <param name="column">Column name.</param>
        /// <returns>
        /// Returns string representation of the value or null, if value is empty.
        /// </returns>
        public static string GetString(DataRow row, string column)
        {
            if (row == null)
                throw new ArgumentNullException("row");
            if (String.IsNullOrEmpty(column))
                throw new ArgumentException(Resources.Error_EmptyString, "column");

            return DataInterpreter.GetString(row, column, DataRowVersion.Default);
        }

        /// <summary>
        /// Converts given column of given row to string value. Works with 
        /// default data row version.
        /// </summary>
        /// <param name="row">DataRow with values to check.</param>
        /// <param name="column">Column name.</param>
        /// <returns>
        /// Returns string representation of the value or String.Empty, if value is empty.
        /// </returns>
        public static string GetStringNotNull(DataRow row, string column)
        {
            if (row == null)
                throw new ArgumentNullException("row");
            if (String.IsNullOrEmpty(column))
                throw new ArgumentException(Resources.Error_EmptyString, "column");

            return DataInterpreter.GetStringNotNull(row, column, DataRowVersion.Default);
        }


        /// <summary>
        /// Converts given column of given row to integer value. Uses platform 
        /// depended conversion. Works with default data row version.
        /// </summary>
        /// <param name="row">DataRow with values to check.</param>
        /// <param name="column">Column name.</param>
        /// <returns>
        /// Returns converted to int value.
        /// </returns>
        public static Nullable<Int64> GetInt(DataRow row, string column)
        {
            if (row == null)
                throw new ArgumentNullException("row");
            if (String.IsNullOrEmpty(column))
                throw new ArgumentException(Resources.Error_EmptyString, "column");

            return DataInterpreter.GetInt(row, column, DataRowVersion.Default);
        }
        #endregion

        #region Changes detection and comparasion
        /// <summary>
        /// Checks, if value of given column of given row is changed.
        /// </summary>
        /// <param name="row">DataRow with values to check.</param>
        /// <param name="column">Column name.</param>
        /// <returns>
        /// If original value is not null, use returns Equals. Otherwise returns 
        /// true if current value is null too.
        /// </returns>
        public static bool HasChanged(DataRow row, string column)
        {
            if (row == null)
                throw new ArgumentNullException("row");
            if (String.IsNullOrEmpty(column))
                throw new ArgumentException(Resources.Error_EmptyString, "column");

            // If new row or deleted row, then it definetelly changed
            if (row.RowState == DataRowState.Deleted || row.RowState == DataRowState.Added)
                return true;

            // Check for availble versions
            if (!row.HasVersion(DataRowVersion.Original)
                || !row.HasVersion(DataRowVersion.Current))
            {
                // Nothing to compare
                return false;
            }

            // Get values
            object original = row[column, DataRowVersion.Original];
            object current = row[column, DataRowVersion.Current];

            // If original is not null, use Equals. Otherwise current should be null to.
            return !CompareObjects(original, current);
        }

        /// <summary>
        /// Checks, if any value in the row has changed.
        /// </summary>
        /// <param name="row">Row to check.</param>
        /// <param name="except">Array with names of columns to exclude from check</param>
        /// <returns>
        /// Returns true if any value in the row has changed and false otherwise.
        /// </returns>
        public static bool HasChanged(DataRow row, string[] except)
        {
            if (row == null)
                throw new ArgumentNullException("row");

            // Check for table
            if (row.Table == null)
            {
                Debug.Fail("Detached rows are unsupported!");
                return false;
            }

            // Check all row values
            foreach (DataColumn column in row.Table.Columns)
            {
                if (IsExcludeColumn(except, column.ColumnName))
                    continue;
                if (HasChanged(row, column.ColumnName))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks, if any value in the row has changed.
        /// </summary>
        /// <param name="row">Row to check.</param>
        /// <returns>
        /// Returns true if any value in the row has changed and false otherwise.
        /// </returns>
        public static bool HasChanged(DataRow row)
        {
            if (row == null)
                throw new ArgumentNullException("row");

            // Check for table
            if (row.Table == null)
            {
                Debug.Fail("Detached rows are unsupported!");
                return false;
            }

            return HasChanged(row, (string[])null);
        }



        /// <summary>
        /// Checks, if any value in the table has changed.
        /// </summary>
        /// <param name="table">Table to check.</param>
        /// <returns>
        /// Returns true if any value in the table has changed and false otherwise.
        /// </returns>
        public static bool HasChanged(DataTable table)
        {
            if (table == null)
                throw new ArgumentNullException("table");

            return HasChanged(table, null);
        }

        /// <summary>
        /// Checks, if any value in the table has changed.
        /// </summary>
        /// <param name="table">Table to check.</param>
        /// <param name="except">Array with names of columns to exclude from check</param>
        /// <returns>
        /// Returns true if any value in the table has changed and false otherwise.
        /// </returns>
        public static bool HasChanged(DataTable table, string[] except)
        {
            if (table == null)
                throw new ArgumentNullException("table");

            // Check all row values
            foreach (DataColumn column in table.Columns)
            {
                if (IsExcludeColumn(except, column.ColumnName))
                    continue;
                foreach (DataRow row in table.Rows)
                {
                    // Check if rpw was added or deleted
                    if (HasChanged(row, column.ColumnName))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks column name for membership in the exclude list.
        /// </summary>
        /// <param name="except">Array with except names.</param>
        /// <param name="columnName">Name to check.</param>
        /// <returns>Returns true if column should be excluded and false otherwize.</returns>
        private static bool IsExcludeColumn(string[] except, string columnName)
        {
            if (except != null)
            {
                foreach (string excludeColumn in except)
                    if (CompareInvariant(columnName, excludeColumn))
                        return true;
            }
            return false;
        }

        /// <summary>
        /// Compares two given objects. If first object is not null, uses Equals method. 
        /// Otherwise second object should be null too.
        /// </summary>
        /// <param name="first">First object.</param>
        /// <param name="second">Second object.</param>
        /// <returns>
        /// If first value is not null, use returns Equals. Otherwise returns 
        /// true if second value is null too.
        /// </returns>
        public static bool CompareObjects(object first, object second)
        {
            return first != null ? first.Equals(second) : second == null;
        }

        /// <summary>
        /// Returns true if two strings are equals in invariant culture ignoring case.
        /// </summary>
        /// <param name="first">Frist string to compare.</param>
        /// <param name="second">Second string to compare.</param>
        /// <returns>Returns true if two strings are equals in invariant culture ignoring case.</returns>
        public static bool CompareInvariant(string first, string second)
        {
            return StringComparer.InvariantCultureIgnoreCase.Equals(first, second);
        }

        /// <summary>
        /// Returns true if value of given column of given row equals to given string
        /// in invariant culture ignoring case.
        /// </summary>
        /// <param name="row">DataRow with data to compare.</param>
        /// <param name="column">Column name to use to extract data from row.</param>
        /// <param name="second">String to compare with.</param>
        /// <returns>
        /// Returns true if value of given column of given row equals to given string
        /// in invariant culture ignoring case.
        /// </returns>
        public static bool CompareInvariant(DataRow row, string column, string second)
        {
            if (row == null)
                throw new ArgumentNullException("row");
            if (String.IsNullOrEmpty(column))
                throw new ArgumentException(Resources.Error_EmptyString, column);

            return StringComparer.InvariantCultureIgnoreCase.Equals(GetStringNotNull(row, column), second);
        }
        #endregion

        #region Row state filters
        /// <summary>
        /// Returns DataRow from DataTable which is not in Deleted state.
        /// </summary>
        /// <param name="table">DataTable to search for row.</param>
        /// <returns>Returns DataRow from DataTable which is not in Deleted state.</returns>
        public static DataRow GetNotDeletedRow(DataTable table)
        {
            if (table == null)
                throw new ArgumentNullException("table");

            foreach (DataRow candidate in table.Rows)
                if (candidate.RowState != DataRowState.Deleted)
                    return candidate;

            return null;
        }  
        #endregion

        #region Select methods
        /// <summary>
        /// Returns simple filter for Select operation (not database SELECT but DataTable.Select filter).
        /// </summary>
        /// <param name="column">Column name to use in the filter.</param>
        /// <param name="value">Value to use in the filter.</param>
        /// <returns>
        /// Returns simple filter for Select operation (not database SELECT but DataTable.Select filter).
        /// </returns>
        public static string BuildFilter(string column, object value)
        {
            if (String.IsNullOrEmpty(column))
                throw new ArgumentException(Resources.Error_EmptyString, "column");
            if (value == null)
                throw new ArgumentNullException("value");

            // If value is integer, use unquoted restriction, otherwize quoted.
            // TODO: Decide are QueryBuilder quoting apropriate here?
            return String.Format(
                "{0} = {1}",
                column,
                IsInteger(value) ? value.ToString() : QueryBuilder.EscapeAndQuoteString(value.ToString()));
        }

        /// <summary>
        /// Selects data rows form table using simple restriction on the one attribute.
        /// </summary>
        /// <param name="table">DataTable to select rows from.</param>
        /// <param name="column">Column name to use in the restriction.</param>
        /// <param name="value">Value to use in the restriction.</param>
        /// <returns>Returns data rows form table which conform simple restriction on the one attribute.</returns>
        public static DataRow[] Select(DataTable table, string column, object value)
        {
            if (table == null)
                throw new ArgumentNullException("table");
            if (String.IsNullOrEmpty(column))
                throw new ArgumentException(Resources.Error_EmptyString, "column");
            if (value == null)
                throw new ArgumentNullException("value");

            return table.Select(BuildFilter(column, value));
        }

        /// <summary>
        /// Selects data rows form table using restriction on the two attributes.
        /// </summary>
        /// <param name="table">DataTable to select rows from.</param>
        /// <param name="column1">Column name to use in the first restriction.</param>
        /// <param name="value1">Value to use in the first restriction.</param>
        /// <param name="column2">Column name to use in the second restriction.</param>
        /// <param name="value2">Value to use in the second restriction.</param>
        /// <returns>Returns data rows form table which conform restriction on the two attributes.</returns>
        public static DataRow[] Select(DataTable table, string column1, object value1, string column2, object value2)
        {
            if (table == null)
                throw new ArgumentNullException("table");
            if (String.IsNullOrEmpty(column1))
                throw new ArgumentException(Resources.Error_EmptyString, "column1");
            if (value1 == null)
                throw new ArgumentNullException("value1");
            if (String.IsNullOrEmpty(column2))
                throw new ArgumentException(Resources.Error_EmptyString, "column2");
            if (value2 == null)
                throw new ArgumentNullException("value2");

            return table.Select(BuildFilter(column1, value1) + " AND " + BuildFilter(column2, value2));
        }


        /// <summary>
        /// Selects data rows form table using simple restriction on the one attribute and sort.
        /// </summary>
        /// <param name="table">DataTable to select rows from.</param>
        /// <param name="column">Column name to use in the restriction.</param>
        /// <param name="value">Value to use in the restriction.</param>
        /// <param name="sort">Sort expression for data rows.</param>
        /// <returns>Returns sorted data rows form table which conform simple restriction on the one attribute.</returns>
        public static DataRow[] Select(DataTable table, string column, object value, string sort)
        {
            if (table == null)
                throw new ArgumentNullException("table");
            if (String.IsNullOrEmpty(column))
                throw new ArgumentException(Resources.Error_EmptyString, "column");
            if (value == null)
                throw new ArgumentNullException("value");
            if (sort == null)
                throw new ArgumentNullException("sort");

            return table.Select(BuildFilter(column, value), sort);
        }
        #endregion

        #region Validation
        /// <summary>
        /// Checks, if given value has integer type.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>
        /// Returns true if value is assignable to the Int64 and false otherwise.
        /// </returns>
        public static bool IsInteger(object value)
        {
            if (value == null)
                return false;
            return typeof(Int64).IsAssignableFrom(value.GetType());
        }

        /// <summary>
        /// Checks, if given value has integer type.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>
        /// Returns true if value is assignable to the Int64 and false otherwise.
        /// </returns>
        public static bool IsInteger(DataRow row, string column)
        {
            if (row == null)
                throw new ArgumentNullException("row");
            if (String.IsNullOrEmpty(column))
                throw new ArgumentException(Resources.Error_EmptyString, "column");

            Debug.Assert(row.Table != null, "Row doesn't assigned to the DataTable");
            DataColumn tableColumn = row.Table.Columns[column];

            Debug.Assert(tableColumn != null, "Unable to get teble column '" + column + "'");
            if(tableColumn == null)
                return false;

            return typeof(Int64).IsAssignableFrom(tableColumn.DataType);
        }

                /// <summary>
        /// Checks value of given column of given row for emptiness, 
        /// considering value as string. Works with default data row 
        /// version.
        /// </summary>
        /// <param name="row">DataRow with values to check.</param>
        /// <param name="column">Column name.</param>
        /// <returns>
        /// Returns true, if value of given column of given row is not empty string,
        /// and false otherwise.
        /// </returns>
        public static bool IsNotEmptyString(DataRow row, string column)
        {
            if (row == null)
                throw new ArgumentNullException("row");
            if (String.IsNullOrEmpty(column))
                throw new ArgumentException(Resources.Error_EmptyString, "column");
            
            return DataInterpreter.IsNotEmptyString(row, column, DataRowVersion.Default);
        }

        /// <summary>
        /// Checks value of given column for emptines.
        /// </summary>
        /// <param name="row">DataRow with values to check.</param>
        /// <param name="column">Column name.</param>
        /// <returns>
        /// Returns true, if value of given column of given row is not null or DBNull,
        /// and false otherwise.
        /// </returns>
        public static bool IsNotNull(DataRow row, string column)
        {
            if (row == null)
                throw new ArgumentNullException("row");
            if (String.IsNullOrEmpty(column))
                throw new ArgumentException(Resources.Error_EmptyString, "column");

            return IsNotNull(row, column, DataRowVersion.Default);
        }

        /// <summary>
        /// Checks value of given column of given row for emptiness, 
        /// considering value as string.
        /// </summary>
        /// <param name="row">DataRow with values to check.</param>
        /// <param name="column">Column name.</param>
        /// <param name="version">Data row version, used to retrieve value.</param>
        /// <returns>
        /// Returns true, if value of given column of given row is not empty string,
        /// and false otherwise.
        /// </returns>
        public static bool IsNotEmptyString(DataRow row, string column, DataRowVersion version)
        {
            if (row == null)
                throw new ArgumentNullException("row");
            if (String.IsNullOrEmpty(column))
                throw new ArgumentException(Resources.Error_EmptyString, "column");
            return IsNotNull(row, column, version) && !String.IsNullOrEmpty(GetString(row,column));
        }

        /// <summary>
        /// Checks value of given column for emptines.
        /// </summary>
        /// <param name="row">DataRow with values to check.</param>
        /// <param name="column">Column name.</param>
        /// <param name="version">Data row version, used to retrieve value.</param>
        /// <returns>
        /// Returns true, if value of given column of given row is not null or DBNull,
        /// and false otherwise.
        /// </returns>
        public static bool IsNotNull(DataRow row, string column, DataRowVersion version)
        {
            if (row == null)
                throw new ArgumentNullException("row");
            if (String.IsNullOrEmpty(column))
                throw new ArgumentException(Resources.Error_EmptyString, "column");
            
            object value = row[column, version];

            return value != null && !(value is DBNull);
        } 
        #endregion

        #region Set method
        /// <summary>
        /// Sets value of given column of given data row if it differs from current value.
        /// </summary>
        /// <param name="row">DataRow with values to check and set.</param>
        /// <param name="column">Name of the column to set.</param>
        /// <param name="value">New value for the column.</param>
        public static void SetValueIfChanged(DataRow row, string column, object value)
        {
            if (row == null)
                throw new ArgumentNullException("row");
            if (String.IsNullOrEmpty(column))
                throw new ArgumentException(Resources.Error_EmptyString, "column");

            // Validate attribute name
            if (!row.Table.Columns.Contains(column))
            {
                Debug.Fail("Unknown column '" + column + "'");
                return;
            }

            // Convert null to DBNull
            object valueToSet = value != null ? value : DBNull.Value;

            // Set if differs
            if (!CompareObjects(row[column], valueToSet))
                row[column] = valueToSet;
        }
        #endregion

        #region Converters
        /// <summary>
        /// Number converter, used to get integer representation of values.
        /// </summary>
        private static readonly BaseNumberConverter converter = new Int64Converter();
        #endregion
    }
}
