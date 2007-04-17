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
 * This file contains implementation of SQL query builder utility.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using Microsoft.VisualStudio.Data;
using System.Data;
using MySql.Data.VisualStudio.Descriptors;
using MySql.Data.VisualStudio.Properties;

namespace MySql.Data.VisualStudio.Utils
{
    /// <summary>
    /// This class supplies SQL query building and used in different parts of package.
    /// </summary>
    static class QueryBuilder
    {
        #region Write values (whithout conditions)
        /// <summary>
        /// Writes value of given column of given data row to given string builder.
        /// </summary>
        /// <param name="row">Data row to extract value.</param>
        /// <param name="column">Column name to look for value.</param>        
        /// <param name="target">String builder to write.</param>
        /// <param name="quote">Indicates if value should be quoted.</param>
        public static void WriteValue(DataRow row, string column, StringBuilder target, bool quote)
        {
            if (row == null)
                throw new ArgumentNullException("row");
            if (String.IsNullOrEmpty(column))
                throw new ArgumentException(Resources.Error_EmptyString, "column");
            if (target == null)
                throw new ArgumentNullException("target");

            // Reading string value
            string toWrite = DataInterpreter.GetString(row, column);

            // If value is not integer, we need quotes and empty strings are allowed
            if (!DataInterpreter.IsInteger(row, column))
            {
                // Substitute null with empty string
                if (toWrite == null)
                    toWrite = String.Empty;
                if (quote)
                    toWrite = EscapeAndQuoteString(toWrite);
            }

            // Append values
            target.Append(toWrite);
        }

        /// <summary>
        /// Writes value of given column of given data row to given string builder.
        /// </summary>
        /// <param name="row">Data row to extract value.</param>
        /// <param name="column">Column name to look for value.</param>        
        /// <param name="target">String builder to write.</param>
        public static void WriteValue(object value, StringBuilder target, bool quote)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            string toWrite;

            // Check is value should be quoted
            if (quote)
            {
                // Substitute null with empty string if quote
                toWrite = EscapeAndQuoteString(value != null ? value.ToString() : String.Empty);
            }
            else
            {
                // If value not quted, empty string is not allowed
                Debug.Assert(value != null, "Empty string for unquoted value!");
                if (value == null)
                    return;
                toWrite = value.ToString();
            }

            // Append values
            target.Append(toWrite);
        }

        /// <summary>
        /// Writes value of given column of given data row to given string builder.
        /// </summary>
        /// <param name="row">Data row to extract value.</param>
        /// <param name="column">Column name to look for value.</param>        
        /// <param name="target">String builder to write.</param>
        public static void WriteValue(DataRow row, string column, StringBuilder target)
        {
            if (row == null)
                throw new ArgumentNullException("row");
            if (String.IsNullOrEmpty(column))
                throw new ArgumentException(Resources.Error_EmptyString, "column");
            if (target == null)
                throw new ArgumentNullException("target");

            WriteValue(row, column, target, true);
        }

        /// <summary>
        /// Writes value of given column of given data row to given string builder.
        /// </summary>
        /// <param name="row">Data row to extract value.</param>
        /// <param name="column">Column name to look for value.</param>        
        /// <param name="target">String builder to write.</param>
        public static void WriteValue(object value, StringBuilder target)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            WriteValue(value, target, true);
        }
        #endregion

        #region Write values (emptines conditions)
        /// <summary>
        /// Writes value of given column of given data row to given string builder with
        /// given prefix if this value is not empty string.
        /// </summary>
        /// <param name="row">Data row to extract value.</param>
        /// <param name="column">Column name to look for value.</param>
        /// <param name="prefix">Prefix to write.</param>
        /// <param name="target">String builder to write.</param>
        public static void WriteIfNotEmptyString(DataRow row, string column, string prefix, StringBuilder target)
        {
            if (row == null)
                throw new ArgumentNullException("row");
            if (String.IsNullOrEmpty(column))
                throw new ArgumentException(Resources.Error_EmptyString, "column");
            if (target == null)
                throw new ArgumentNullException("target");

            WriteIfNotEmptyString(row, column, prefix, target, true);
        }

        /// <summary>
        /// Writes given value to given string builder with given prefix, 
        /// if value is not empty string.
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <param name="prefix">Prefix to write.</param>
        /// <param name="target">String builder to write.</param>
        public static void WriteIfNotEmptyString(object value, string prefix, StringBuilder target)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            WriteIfNotEmptyString(value, prefix, target, true);
        }

        /// <summary>
        /// Writes value of given column of given data row to given string builder with
        /// given prefix if this value is not empty string.
        /// </summary>
        /// <param name="row">Data row to extract value.</param>
        /// <param name="column">Column name to look for value.</param>
        /// <param name="prefix">Prefix to write.</param>
        /// <param name="target">String builder to write.</param>
        /// <param name="quote">Indicates if value should be quoted.</param>
        public static void WriteIfNotEmptyString(DataRow row, string column, string prefix, StringBuilder target, bool quote)
        {
            if (row == null)
                throw new ArgumentNullException("row");
            if (String.IsNullOrEmpty(column))
                throw new ArgumentException(Resources.Error_EmptyString, "column");
            if (target == null)
                throw new ArgumentNullException("target");

            if (DataInterpreter.IsNotEmptyString(row, column))
            {
                if (!String.IsNullOrEmpty(prefix))
                    target.Append(prefix);
                WriteValue(row, column, target, quote);
            }
        }

        /// <summary>
        /// Writes given value to given string builder with given prefix, 
        /// if value is not empty string.
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <param name="prefix">Prefix to write.</param>
        /// <param name="target">String builder to write.</param>
        /// <param name="quote">Indicates if value should be quoted.</param>
        public static void WriteIfNotEmptyString(object value, string prefix, StringBuilder target, bool quote)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            string stringValue = value as string;
            if (!String.IsNullOrEmpty(stringValue))
            {
                if (!String.IsNullOrEmpty(prefix))
                    target.Append(prefix);
                WriteValue(stringValue, target, quote);
            }
        }

        /// <summary>
        /// Writes value of given column of given data row to given string builder with
        /// given prefix if this value is not null.
        /// </summary>
        /// <param name="row">Data row to extract value.</param>
        /// <param name="column">Column name to look for value.</param>
        /// <param name="prefix">Prefix to write.</param>
        /// <param name="target">String builder to write.</param>
        public static void WriteIfNotNull(DataRow row, string column, string prefix, StringBuilder target)
        {
            if (row == null)
                throw new ArgumentNullException("row");
            if (String.IsNullOrEmpty(column))
                throw new ArgumentException(Resources.Error_EmptyString, "column");
            if (target == null)
                throw new ArgumentNullException("target");

            WriteIfNotNull(row, column, prefix, target, true);
        }

        /// <summary>
        /// Writes value of given column of given data row to given string builder with
        /// given prefix if this value is not null.
        /// </summary>
        /// <param name="row">Data row to extract value.</param>
        /// <param name="column">Column name to look for value.</param>
        /// <param name="prefix">Prefix to write.</param>
        /// <param name="target">String builder to write.</param>
        /// <param name="quote">Indicates if value should be quoted.</param>
        public static void WriteIfNotNull(DataRow row, string column, string prefix, StringBuilder target, bool quote)
        {
            if (row == null)
                throw new ArgumentNullException("row");
            if (String.IsNullOrEmpty(column))
                throw new ArgumentException(Resources.Error_EmptyString, "column");
            if (target == null)
                throw new ArgumentNullException("target");

            if (DataInterpreter.IsNotNull(row, column))
            {
                if (!String.IsNullOrEmpty(prefix))
                    target.Append(prefix);
                WriteValue(row, column, target, quote);
            }
        }
        #endregion

        #region Write values (changed conditions)
        /// <summary>
        /// Writes value of given column of given data row to given string builder with
        /// given prefix if this value was changed.
        /// </summary>
        /// <param name="row">Data row to extract value.</param>
        /// <param name="column">Column name to look for value.</param>
        /// <param name="prefix">Prefix to write.</param>
        /// <param name="target">String builder to write.</param>
        public static void WriteIfChanged(DataRow row, string column, string prefix, StringBuilder target)
        {
            if (row == null)
                throw new ArgumentNullException("row");
            if (String.IsNullOrEmpty(column))
                throw new ArgumentException(Resources.Error_EmptyString, "column");
            if (target == null)
                throw new ArgumentNullException("target");

            WriteIfChanged(row, column, prefix, target, null, true);
        }

        /// <summary>
        /// Writes value of given column of given data row to given string builder with
        /// given prefix if this value was changed.
        /// </summary>
        /// <param name="row">Data row to extract value.</param>
        /// <param name="column">Column name to look for value.</param>
        /// <param name="prefix">Prefix to write.</param>
        /// <param name="target">String builder to write.</param>
        /// <param name="suffix">Suffix to append after value.</param>
        public static void WriteIfChanged(DataRow row, string column, string prefix, StringBuilder target, string suffix)
        {
            if (row == null)
                throw new ArgumentNullException("row");
            if (String.IsNullOrEmpty(column))
                throw new ArgumentException(Resources.Error_EmptyString, "column");
            if (target == null)
                throw new ArgumentNullException("target");

            WriteIfChanged(row, column, prefix, target, suffix, true);
        }

        /// <summary>
        /// Writes value of given column of given data row to given string builder with
        /// given prefix if this value was changed.
        /// </summary>
        /// <param name="row">Data row to extract value.</param>
        /// <param name="column">Column name to look for value.</param>
        /// <param name="prefix">Prefix to write.</param>
        /// <param name="target">String builder to write.</param>
        /// <param name="quote">Indicates if value should be quoted.</param>
        public static void WriteIfChanged(DataRow row, string column, string prefix, StringBuilder target, bool quote)
        {
            if (row == null)
                throw new ArgumentNullException("row");
            if (String.IsNullOrEmpty(column))
                throw new ArgumentException(Resources.Error_EmptyString, "column");
            if (target == null)
                throw new ArgumentNullException("target");

            WriteIfChanged(row, column, prefix, target, null, quote);
        }

        /// <summary>
        /// Writes value of given column of given data row to given string builder with
        /// given prefix if this value was changed.
        /// </summary>
        /// <param name="row">Data row to extract value.</param>
        /// <param name="column">Column name to look for value.</param>
        /// <param name="prefix">Prefix to write.</param>
        /// <param name="target">String builder to write.</param>
        /// <param name="suffix">Suffix to append after value.</param>
        /// <param name="quote">Indicates if value should be quoted.</param>
        public static void WriteIfChanged(DataRow row, string column, string prefix, StringBuilder target, string suffix, bool quote)
        {
            if (row == null)
                throw new ArgumentNullException("row");
            if (String.IsNullOrEmpty(column))
                throw new ArgumentException(Resources.Error_EmptyString, "column");
            if (target == null)
                throw new ArgumentNullException("target");

            if (DataInterpreter.HasChanged(row, column))
            {
                if (!String.IsNullOrEmpty(prefix))
                    target.Append(prefix);
                WriteValue(row, column, target, quote);
                if (!String.IsNullOrEmpty(suffix))
                    target.Append(suffix);
            }
        }
        #endregion

        #region Write identifiers
        /// <summary>
        /// Writes value of given column of given data row as identifier to given 
        /// string builder with given prefix if this value was changed.
        /// </summary>
        /// <param name="row">Data row to extract value.</param>
        /// <param name="column">Column name to look for value.</param>
        /// <param name="prefix">Prefix to write.</param>
        /// <param name="target">String builder to write.</param>
        public static void WriteIdentifierIfChanged(DataRow row, string column, string prefix, StringBuilder target)
        {
            if (row == null)
                throw new ArgumentNullException("row");
            if (String.IsNullOrEmpty(column))
                throw new ArgumentException(Resources.Error_EmptyString, "column");
            if (target == null)
                throw new ArgumentNullException("target");

            WriteIdentifierIfChanged(row, column, prefix, null, target);
        }

        /// <summary>
        /// Writes value of given column of given data row as identifier to given 
        /// string builder with given prefix if this value was changed.
        /// </summary>
        /// <param name="row">Data row to extract value.</param>
        /// <param name="column">Column name to look for value.</param>
        /// <param name="prefix">Prefix to write before.</param>
        /// <param name="suffix">Suffix to write after.</param>
        /// <param name="target">String builder to write.</param>
        public static void WriteIdentifierIfChanged(DataRow row, string column, string prefix, string suffix, StringBuilder target)
        {
            if (row == null)
                throw new ArgumentNullException("row");
            if (String.IsNullOrEmpty(column))
                throw new ArgumentException(Resources.Error_EmptyString, "column");
            if (target == null)
                throw new ArgumentNullException("target");

            if (DataInterpreter.HasChanged(row, column))
            {
                if (!String.IsNullOrEmpty(prefix))
                    target.Append(prefix);
                target.Append(EscapeAndQuoteIdentifier(DataInterpreter.GetString(row, column)));
                if (!String.IsNullOrEmpty(suffix))
                    target.Append(suffix);
            }
        }

        /// <summary>
        /// Writes given value as identifier to given string builder.
        /// </summary>
        /// <param name="identifier">String with unquoted identifier.</param>
        /// <param name="column">Column name to look for value.</param>
        /// <param name="target">String builder to write.</param>
        public static void WriteIdentifier(string identifier, StringBuilder target)
        {
            if (String.IsNullOrEmpty(identifier))
                throw new ArgumentException(Resources.Error_EmptyString, "identifier");
            if (target == null)
                throw new ArgumentNullException("target");

            target.Append(EscapeAndQuoteIdentifier(identifier));
        }

        /// <summary>
        /// Writes two-part identifier to given 
        /// string builder.
        /// </summary>
        /// <param name="identifier0">String with first unquoted identifier.</param>
        /// <param name="identifier1">String with second unquoted identifier.</param>
        /// <param name="column">Column name to look for value.</param>
        /// <param name="target">String builder to write.</param>
        public static void WriteIdentifier(string identifier0, string identifier1, StringBuilder target)
        {
            if (String.IsNullOrEmpty(identifier0))
                throw new ArgumentException(Resources.Error_EmptyString, "identifier0");
            if (String.IsNullOrEmpty(identifier1))
                throw new ArgumentException(Resources.Error_EmptyString, "identifier1");
            if (target == null)
                throw new ArgumentNullException("target");

            target.Append(EscapeAndQuoteIdentifier(identifier0));
            target.Append('.');
            target.Append(EscapeAndQuoteIdentifier(identifier1));
        }


        /// <summary>
        /// Writes value of given column of given data row as identifier to given 
        /// string builder.
        /// </summary>
        /// <param name="row">Data row to extract value.</param>
        /// <param name="column">Column name to look for value.</param>
        /// <param name="target">String builder to write.</param>
        public static void WriteIdentifier(DataRow row, string column, StringBuilder target)
        {
            if (row == null)
                throw new ArgumentNullException("row");
            if (String.IsNullOrEmpty(column))
                throw new ArgumentException(Resources.Error_EmptyString, "column");
            if (target == null)
                throw new ArgumentNullException("target");

            target.Append(EscapeAndQuoteIdentifier(DataInterpreter.GetString(row, column)));
        }

        /// <summary>
        /// Writes original value of given column of given data row as identifier 
        /// to given string builder.
        /// </summary>
        /// <param name="row">Data row to extract value.</param>
        /// <param name="column">Column name to look for value.</param>
        /// <param name="target">String builder to write.</param>
        public static void WriteOldIdentifier(DataRow row, string column, StringBuilder target)
        {
            if (row == null)
                throw new ArgumentNullException("row");
            if (String.IsNullOrEmpty(column))
                throw new ArgumentException(Resources.Error_EmptyString, "column");
            if (target == null)
                throw new ArgumentNullException("target");

            target.Append(EscapeAndQuoteIdentifier(DataInterpreter.GetString(row, column, DataRowVersion.Original)));
        }

        /// <summary>
        /// Writes given value as user name to given string builder with given prefix 
        /// if this value is not empty string.
        /// </summary>
        /// <param name="name">Username to wirte.</param>
        /// <param name="prefix">Prefix to write.</param>
        /// <param name="target">String builder to write.</param>
        public static void WriteUserNameIfNotEmpty(string name, string prefix, StringBuilder target)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            // Return if empty
            if (String.IsNullOrEmpty(name))
                return;

            if (!String.IsNullOrEmpty(prefix))
                target.Append(prefix);

            // Extract new user name
            string user = name;
            user = user.Trim();

            // Need to split and quote 
            string[] nameandhost = user.Split('@');
            if (nameandhost == null)
            {
                Debug.Fail("Unable to parse user name!");
                return;
            }

            // Write user name
            Debug.Assert(nameandhost.Length > 0, "User name is absent!");
            if (nameandhost.Length > 0)
                target.Append(EscapeAndQuoteIdentifier(nameandhost[0]));

            // Write host delimeter
            target.Append('@');

            // Write host name
            Debug.Assert(nameandhost.Length > 1, "Host name is absent!");
            if (nameandhost.Length > 1)
                target.Append(EscapeAndQuoteIdentifier(nameandhost[1]));

            Debug.Assert(nameandhost.Length < 3, "To many parts in the user name!");
        }
        #endregion

        #region Write expression (boolean conditions)
        /// <summary>
        /// Writes given expression to given string builder if value in given column 
        /// of given data row is equivalent to True.
        /// </summary>
        /// <param name="row">Data row to extract value.</param>
        /// <param name="column">Column name to look for value.</param>
        /// <param name="expression">Expression to write to string builder.</param>
        /// <param name="target">String builder to write.</param>
        public static void WriteIfTrue(DataRow row, string column, string expression, StringBuilder target)
        {
            if (row == null)
                throw new ArgumentNullException("row");
            if (String.IsNullOrEmpty(column))
                throw new ArgumentException(Resources.Error_EmptyString, "column");
            if (target == null)
                throw new ArgumentNullException("target");

            if (DataInterpreter.GetSqlBool(row, column))
                target.Append(expression);
        }

        /// <summary>
        /// Writes given expression to given string builder if value in given column 
        /// of given data row is equivalent to False.
        /// </summary>
        /// <param name="row">Data row to extract value.</param>
        /// <param name="column">Column name to look for value.</param>
        /// <param name="expression">Expression to write to string builder.</param>
        /// <param name="target">String builder to write.</param>
        public static void WriteIfFalse(DataRow row, string column, string expression, StringBuilder target)
        {
            if (row == null)
                throw new ArgumentNullException("row");
            if (String.IsNullOrEmpty(column))
                throw new ArgumentException(Resources.Error_EmptyString, "column");
            if (target == null)
                throw new ArgumentNullException("target");

            if (!DataInterpreter.GetSqlBool(row, column))
                target.Append(expression);
        }
        #endregion

        #region Escaping methods
        /// <summary>
        /// Escapes string and wrap it into quotes.
        /// </summary>
        /// <param name="value">String to process</param>
        /// <returns>Escaped and quoted string.</returns>
        public static string EscapeAndQuoteString(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            // TODO: DEals with SQL_MODE
            // Escape quotes first
            string quoted = value.Replace("'", "''");

            // Then escape backslash
            quoted = quoted.Replace("\\", "\\\\");

            // Wrap in quotes
            return '\'' + quoted + '\'';
        }

        /// <summary>
        /// Escapes identifier and wrap it into identifiers quotes.
        /// </summary>
        /// <param name="value">String to process</param>
        /// <returns>Escaped and quoted identifier.</returns>
        public static string EscapeAndQuoteIdentifier(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            char open = Char.MinValue;
            char close = Char.MinValue;

            if (!value.StartsWith("`"))
                open = '`';
            if (!value.EndsWith("`"))
                close = '`';
            if (open == Char.MinValue && close == Char.MinValue)
                return value;
            value = value.Replace("`", "``");
            return String.Format("{0}{1}{2}", open, value, close);
        } 
        #endregion

        #region Public constants
        /// <summary>Current user function name.</summary>
        public const string CurretnUser = "CURRENT_USER";
        #endregion
    }
}
