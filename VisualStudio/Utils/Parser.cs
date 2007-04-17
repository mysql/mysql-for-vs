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
 * This file contains implementation of the Parser utility.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using MySql.Data.VisualStudio.Properties;
using System.Diagnostics;

namespace MySql.Data.VisualStudio.Utils
{
    /// <summary>
    /// This class is used to parse field strings and SQL statements.
    /// </summary>
    static class Parser
    {
        #region Fields string parsing
        /// <summary>
        /// Extracts otions form the string into key-value ductionary. " " is 
        /// used as fields delimeter and "=" it used as key-value delimeter.
        /// </summary>
        /// <param name="fields">String with fields to parse.</param>
        /// <returns>Returns dictionary with extracted fields</returns>
        public static Dictionary<string, string> ExtractFieldsDictionary(string fields)
        {
            Dictionary<string, string> fieldsDictionary = new Dictionary<string, string>();

            // If string is empty, return empty dictionary
            if (String.IsNullOrEmpty(fields))
                return fieldsDictionary;

            // Extract field pairs
            string[] fieldsArray = fields.Split(new char[] { ' ' });
            if (fieldsArray != null)
            {
                // Split each field pair
                string[] nameValue;
                for (int i = 0; i < fieldsArray.Length; i++)
                {
                    if (fieldsArray[i] == null)
                        continue;
                    nameValue = fieldsArray[i].Split(new char[] { '=' });
                    if (nameValue == null)
                        continue;
                    // Field with name only
                    if (nameValue.Length == 1)
                        fieldsDictionary.Add(nameValue[0], String.Empty);
                    // Field with name and value
                    if (nameValue.Length > 1)
                        fieldsDictionary.Add(nameValue[0], nameValue[1]);
                }
            }

            // Return results
            return fieldsDictionary;
        }

        /// <summary>
        /// Extracts otions form the value of given column of given data row into 
        /// key-value ductionary. " " is used as fields delimeter and "=" it used 
        /// as key-value delimeter.
        /// </summary>
        /// <param name="row">DataRow with values to check.</param>
        /// <param name="column">Column name.</param>
        /// <returns>Returns dictionary with extracted fields</returns>
        public static Dictionary<string, string> ExtractFieldsDictionary(DataRow row, string column)
        {
            if (row == null)
                throw new ArgumentNullException("row");
            if (String.IsNullOrEmpty(column))
                throw new ArgumentException(Resources.Error_EmptyString, "column");

            return ExtractFieldsDictionary(DataInterpreter.GetString(row, column));
        } 
        #endregion

        #region Fields extraction from SQL queries
        /// <summary>
        /// Extracts advanced field value from the CREATE SQL as a single token.
        /// </summary>
        /// <param name="result">Dictionary with fields which is used to store extracted field.</param>
        /// <param name="createQuery">CREATE SQL string to parse.</param>
        /// <param name="queryPart">Part of SQL which precedes field value.</param>
        /// <param name="fieldName">Name of the field to extract.</param>
        public static void ExtractAdvancedFieldToken(Dictionary<string, string> result, string createQuery, string queryPart, string fieldName)
        {
            // Search for query part
            int pos = Parser.LocateUnquoted(createQuery, queryPart);
            // Query part was found
            if (pos > 0)
            {
                // Extract field value
                string fieldValue = Parser.ExtractToken(createQuery, pos + queryPart.Length);
                // Add field to the list, if value is not empty
                if (!String.IsNullOrEmpty(fieldValue))
                    result.Add(fieldName, fieldValue);
            }
        }

        /// <summary>
        /// Extracts advanced field value from the TABLE SQL and unquote it.
        /// </summary>
        /// <param name="result">Dictionary with fields which is used to store extracted field.</param>
        /// <param name="createQuery">CREATE SQL string to parse.</param>
        /// <param name="queryPart">Part of SQL which precedes field value.</param>
        /// <param name="fieldName">Name of the field to extract.</param>
        public static void ExtractAdvancedFieldUnquoted(Dictionary<string, string> result, string createQuery, string queryPart, string fieldName)
        {
            // Search for query part
            int pos = Parser.LocateUnquoted(createQuery, queryPart);
            // Query part was found
            if (pos > 0)
            {
                // Extract field value
                string fieldValue = Parser.ExtractUnquotedValue(createQuery, pos + queryPart.Length);
                // Add field to the list, if value is not empty
                if (!String.IsNullOrEmpty(fieldValue))
                    result.Add(fieldName, fieldValue);
            }
        }

        /// <summary>
        /// Extracts advanced field value from the CREATE SQL and unbrace it.
        /// </summary>
        /// <param name="result">Dictionary with fields which is used to store extracted field.</param>
        /// <param name="createQuery">CREATE SQL string to parse.</param>
        /// <param name="queryPart">Part of SQL which precedes field value.</param>
        /// <param name="fieldName">Name of the field to extract.</param>
        public static void ExtractAdvancedFieldUnbraced(Dictionary<string, string> result, string createQuery, string queryPart, string fieldName)
        {
            // Search for query part
            int pos = Parser.LocateUnquoted(createQuery, queryPart);
            // Query part was found
            if (pos > 0)
            {
                // Extract field value
                string fieldValue = Parser.ExtractUnbracedExpression(createQuery, pos + queryPart.Length);
                // Add field to the list, if value is not empty
                if (!String.IsNullOrEmpty(fieldValue))
                    result.Add(fieldName, fieldValue);
            }
        }
        #endregion

        #region General parsing
        /// <summary>
        /// Checks if a given character is whitespace.
        /// </summary>
        /// <param name="p">Character to check.</param>
        /// <returns>Returns true if given character is whitespace and false otherwize.</returns>
        public static bool IsWhiteSpace(char p)
        {
            for (int i = 0; i < WhiteSpaces.Length; i++)
                if (WhiteSpaces[i] == p)
                    return true;
            return false;
        }

        /// <summary>
        /// Checks if a given character is delimiter.
        /// </summary>
        /// <param name="p">Character to check.</param>
        /// <returns>Returns true if given character is delimiter and false otherwize.</returns>
        public static bool IsDelimiter(char p)
        {
            for (int i = 0; i < Delimiters.Length; i++)
                if (Delimiters[i] == p)
                    return true;
            return false;
        }

        /// <summary>
        /// Returns position of the first unquoted entry of the subExpresion
        /// into expression.
        /// </summary>
        /// <param name="expression">Expression string.</param>
        /// <param name="subExpression">Subexpression string</param>
        /// <param name="startFrom">Position to start search from.</param>
        /// <returns>
        /// Returns position of the first unquoted entry of the subExpresion
        /// into expression.
        /// </returns>
        public static int LocateUnquoted(string expression, string subExpression, int startFrom)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");
            if (subExpression == null)
                throw new ArgumentNullException("subExpression");
            if (startFrom < 0 || startFrom > expression.Length)
                throw new ArgumentOutOfRangeException("startFrom");

            // Initial search
            int result = expression.IndexOf(subExpression, startFrom, StringComparison.InvariantCultureIgnoreCase);
            if (result < 0)
                return result;

            bool inValueQuotes = false, inIdentifierQuotes = false;

            // Iterate through string
            for (int i = startFrom; i < expression.Length; i++)
            {
                // Check for quotes
                if (IsQuotedPosition(expression, ref inValueQuotes, ref inIdentifierQuotes, ref i))
                    continue;

                // We have not reach needed place
                if (i != result)
                    continue;

                // In quotes, need to search again
                if (inValueQuotes || inIdentifierQuotes)
                {
                    result = expression.IndexOf(subExpression, i + 1, StringComparison.InvariantCultureIgnoreCase);
                    if (result < 0)
                        return result;
                }
                else
                {
                    // Found what we need
                    return result;
                }
            }

            // If the cycle works fine, we should not get here
            Debug.Fail("Should not reach this code!");
            return -1;
        }

        /// <summary>
        /// Returns position of the first unquoted entry of the subExpresion
        /// into expression.
        /// </summary>
        /// <param name="expression">Expression string.</param>
        /// <param name="subExpression">Subexpression string</param>
        /// <returns>
        /// Returns position of the first unquoted entry of the subExpresion
        /// into expression.
        /// </returns>
        public static int LocateUnquoted(string expression, string subExpression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");
            if (subExpression == null)
                throw new ArgumentNullException("subExpression");

            return LocateUnquoted(expression, subExpression, 0);
        }

        /// <summary>
        /// Extracts value from the expresion and unqote it.
        /// </summary>
        /// <param name="expression">Expression to extract value.</param>
        /// <param name="startFrom">Position of the open quote.</param>
        /// <returns>Returns extracted unquoted value.</returns>
        public static string ExtractUnquotedValue(string expression, int startFrom)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");
            if (startFrom < 0 || startFrom >= expression.Length)
                throw new ArgumentOutOfRangeException("startFrom");

            // Stub to store end position
            int end;
            
            // Return extracted substring
            return ExtractUnquotedValue(expression, startFrom, out end);
        }

        /// <summary>
        /// Extracts value from the expresion and unqote it.
        /// </summary>
        /// <param name="expression">Expression to extract value.</param>
        /// <param name="startFrom">Position of the open quote.</param>
        /// <returns>Returns extracted unquoted value.</returns>
        public static string ExtractUnquotedValue(string expression, int startFrom, out int end)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");
            if (startFrom < 0 || startFrom >= expression.Length)
                throw new ArgumentOutOfRangeException("startFrom");

            int openQuote = -1, closeQuote = -1;

            for (int i = startFrom; i < expression.Length; i++)
            {
                // Non quote
                if (expression[i] != ValueQuote)
                    continue;

                // Open quote found
                if (openQuote < 0)
                {
                    openQuote = i;
                    continue;
                }

                // Quote is escaped using escape symbol
                if (i > 1 && expression[i - 1] == EscapeSymbol)
                    continue;

                // Quote is escaped using second quote
                if (i < expression.Length - 1 && expression[i + 1] == ValueQuote)
                {
                    i++;
                    continue;
                }

                // Close quote found
                closeQuote = i;
                break;
            }

            // Closing quote position is th end of the string
            end = closeQuote;

            // Check if we found both qoutes
            if (openQuote < 0 || closeQuote < 0)
            {
                Debug.Fail("Failed to locate quotes!");
                return String.Empty;
            }

            // Extract substring
            string substr = expression.Substring(openQuote + 1, closeQuote - openQuote - 1);
            
            // Unquote double quotes
            substr = substr.Replace(ValueQuote.ToString() + ValueQuote.ToString(), ValueQuote.ToString());


            // Unquote quotes with escaped symbol quotes
            return substr.Replace(EscapeSymbol.ToString() + ValueQuote.ToString(), ValueQuote.ToString());
        }

        /// <summary>
        /// Extracts token from the expresion (reads all until first space).
        /// </summary>
        /// <param name="expression">Expression to extract value.</param>
        /// <param name="startFrom">Position of the token</param>
        /// <returns>Returns token text.</returns>
        public static string ExtractToken(string expression, int startFrom)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");
            if (startFrom < 0 || startFrom >= expression.Length)
                throw new ArgumentOutOfRangeException("startFrom");

            // Skip sapces and find start
            int start = startFrom;
            for (; IsWhiteSpace(expression[start]) && start < expression.Length; start++) ;
            if (start >= expression.Length)
                return String.Empty;

            // Find position of the trailing white space
            int end = start;
            for (; !IsWhiteSpace(expression[end]) && !IsDelimiter(expression[end]) && end < expression.Length; end++) ;
            
            // Return extracted substring
            return expression.Substring(start, end - start);
        }

        /// <summary>
        /// Extracts content of the braces from the expresion.
        /// </summary>
        /// <param name="expression">Expression to extract value.</param>
        /// <param name="startFrom">Position of the open brace.</param>
        /// <returns>Returns extracted content of the braces.</returns>
        public static string ExtractUnbracedExpression(string expression, int startFrom)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");
            if (startFrom < 0 || startFrom >= expression.Length)
                throw new ArgumentOutOfRangeException("startFrom");

            int openBrace = -1;
            int closeBrace = -1;
            int balance = 0;

            bool inValueQuotes = false, inIdentifierQuotes = false, skip = false;

            // Iterate through string
            for (int i = startFrom; i < expression.Length; i++)
            {
                // Check for quotes
                skip = IsQuotedPosition(expression, ref inValueQuotes, ref inIdentifierQuotes, ref i);

                // Skip, because in quotas
                if (skip || inValueQuotes || inIdentifierQuotes)
                    continue;

                // Open brace found
                if (expression[i] == OpenBrace)
                {
                    balance++;
                    // This is the first brace
                    if (openBrace < 0)
                    {
                        openBrace = i;
                        continue;
                    }                    
                }

                // Close brace found
                if (expression[i] == CloseBrace)
                {
                    balance--;
                    // This close brace closed last open brace
                    if (balance == 0)
                    {
                        closeBrace = i;
                        break;
                    }
                }

                // Check for balance
                if (balance < 0)
                {
                    Debug.Fail("Unbalanced braces!");
                    break;
                }
            }

            // Check if we found both qoutes
            if (openBrace < 0 || closeBrace < 0)
            {
                Debug.Fail("Failed to locate quotes!");
                return String.Empty;
            }

            // Return extracted substring
            return expression.Substring(openBrace + 1, closeBrace - openBrace - 1);
        }

        /// <summary>
        /// Returns true if given string has any punctuation characters.
        /// </summary>
        /// <param name="expression">String to check.</param>
        /// <returns>Returns true if given string has any punctuation characters.</returns>
        public static bool HasPunctuation(string expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            for (int i = 0; i < expression.Length; i++)
                if (Char.IsPunctuation(expression, i) && expression[i] != OpenBrace
                    && expression[i] != CloseBrace && expression[i] != Undescore)
                    return true;

            return false;
        }
        #endregion

        #region Type parsing
        /// <summary>
        /// Returns true if given string is a numeric datatype.
        /// </summary>
        /// <param name="dataType">String to check.</param>
        /// <returns>Returns true if given string is a numeric datatype.</returns>
        public static bool IsNumericType(string dataType)
        {
            if (dataType == null)
                throw new ArgumentNullException("dataType");

            return dataType.StartsWith(TINYINT, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(SMALLINT, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(MEDIUMINT, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(INT, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(INTEGER, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(BIGINT, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(REAL, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(FLOAT, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(DECIMAL, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(NUMERIC, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(BIT, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Returns true if given string is a spatial datatype.
        /// </summary>
        /// <param name="dataType">String to check.</param>
        /// <returns>Returns true if given string is a spatial datatype.</returns>
        public static bool IsSpatialType(string dataType)
        {
            if (dataType == null)
                throw new ArgumentNullException("dataType");

            return dataType.StartsWith(GEOMETRY, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(POINT, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(LINESTRING, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(POLYGON, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(MULTIPOINT, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(MULTILINESTRING, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(MULTIPOLYGON, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(GEOMETRYCOLLECTION, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Returns true if given string is a BLOB datatype.
        /// </summary>
        /// <param name="dataType">String to check.</param>
        /// <returns>Returns true if given string is a BLOB datatype.</returns>
        public static bool IsBlobType(string dataType)
        {
            if (dataType == null)
                throw new ArgumentNullException("dataType");

            return dataType.StartsWith(TINYBLOB, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(BLOB, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(MEDIUMBLOB, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(LONGBLOB, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Returns true if given string is a TEXT datatype.
        /// </summary>
        /// <param name="dataType">String to check.</param>
        /// <returns>Returns true if given string is a TEXT datatype.</returns>
        public static bool IsTextType(string dataType)
        {
            if (dataType == null)
                throw new ArgumentNullException("dataType");

            return dataType.StartsWith(TINYTEXT, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(TEXT, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(MEDIUMTEXT, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(LONGTEXT, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Returns true if given string is a character datatype.
        /// </summary>
        /// <param name="dataType">String to check.</param>
        /// <returns>
        /// Returns true if given string is a character datatyp.
        /// </returns>
        public static bool IsCharacterType(string dataType)
        {
            if (dataType == null)
                throw new ArgumentNullException("dataType");

            return dataType.StartsWith(CHAR, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(VARCHAR, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(TINYTEXT, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(TEXT, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(MEDIUMTEXT, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(LONGTEXT, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Returns true if given string is a datetime datatype.
        /// </summary>
        /// <param name="dataType">String to check.</param>
        /// <returns>
        /// Returns true if given string is a datetime datatyp.
        /// </returns>
        public static bool IsDateTimeType(string dataType)
        {
            if (dataType == null)
                throw new ArgumentNullException("dataType");

            return dataType.StartsWith(DATE, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(TIME, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(DATETIME, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(TIMESTAMP, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Returns true if given string is a numeric datatype which supports UNSIGNED 
        /// and ZEROFILL fields.
        /// </summary>
        /// <param name="dataType">String to check.</param>
        /// <returns>
        /// Returns true if given string is a numeric datatype.which supports UNSIGNED 
        /// and ZEROFILL fields.
        /// </returns>
        public static bool SupportUnsignedAndZerofill(string dataType)
        {
            if (dataType == null)
                throw new ArgumentNullException("dataType");

            return dataType.StartsWith(TINYINT, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(SMALLINT, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(MEDIUMINT, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(INT, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(INTEGER, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(BIGINT, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(REAL, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(FLOAT, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(DECIMAL, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(NUMERIC, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Returns true if given string is a character datatype which supports BINARY field.
        /// </summary>
        /// <param name="dataType">String to check.</param>
        /// <returns>
        /// Returns true if given string is a character datatype which supports BINARY field.
        /// </returns>
        public static bool SupportBinary(string dataType)
        {
            if (dataType == null)
                throw new ArgumentNullException("dataType");

            return dataType.StartsWith(CHAR, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(VARCHAR, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(TINYTEXT, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(TEXT, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(MEDIUMTEXT, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(LONGTEXT, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Returns true if given string is a character datatype which supports 
        /// CHARACTER SET and COLLATION fields.
        /// </summary>
        /// <param name="dataType">String to check.</param>
        /// <returns>
        /// Returns true if given string is a character datatype which supports 
        /// CHARACTER SET and COLLATION fields.
        /// </returns>
        public static bool SupportCharacterSet(string dataType)
        {
            if (dataType == null)
                throw new ArgumentNullException("dataType");

            return dataType.StartsWith(CHAR, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(VARCHAR, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(TINYTEXT, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(TEXT, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(MEDIUMTEXT, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(LONGTEXT, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(ENUM, StringComparison.InvariantCultureIgnoreCase) ||
                    dataType.StartsWith(SET, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Returns true if given string is a character datatype which supports ASCII 
        /// and UNICODE fields.
        /// </summary>
        /// <param name="dataType">String to check.</param>
        /// <returns>
        /// Returns true if given string is a character datatype which supports ASCII 
        /// and UNICODE fields.
        /// </returns>
        public static bool SupportAsciiAndUnicode(string dataType)
        {
            if (dataType == null)
                throw new ArgumentNullException("dataType");

            return dataType.StartsWith(CHAR, StringComparison.InvariantCultureIgnoreCase);
        }
        #endregion

        #region Validation
        /// <summary>
        /// Returns true if given string is valid identifier.
        /// </summary>
        /// <param name="identifier">String to check.</param>
        /// <returns>Returns true if given string is valid identifier.</returns>
        public static bool IsValidIdentifier(string identifier)
        {
            // Identifier can't be empty
            if (String.IsNullOrEmpty(identifier))
                return false;

            // Identifier should be not greater then 64 symbols (RTFM 9.2)
            if (identifier.Length >= 64)
                return false;

            // Identifier should not contain '.', '\' and '/' symbols
            if (identifier.IndexOfAny(Forbidden) >= 0)
                return false;

            // Identifier should not ends with whitespace
            if (IsWhiteSpace(identifier[identifier.Length - 1]))
                return false;

            // If everething is ok, returns true
            return true;
        }

        /// <summary>
        /// Returns true if given string is valid datatype.
        /// </summary>
        /// <param name="identifier">String to check.</param>
        /// <returns>Returns true if given string is valid datatype.</returns>
        public static bool IsValidDatatype(string datatype)
        {
            // Datatype can't be empty
            if (String.IsNullOrEmpty(datatype))
                return false;

            // TODO: Implement other checks
            return true;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Checks if current position in the string is quoted. Uses context from previous check.
        /// </summary>
        /// <param name="expression">Expression to check.</param>
        /// <param name="inValueQuotes">Flag, indicating that previous position is in the value quotes.</param>
        /// <param name="inIdentifierQuotes">Flag, indicating that previous position is in the idntifier quotes.</param>
        /// <param name="i">Position to check.</param>
        /// <returns>Returns true if escaped quote was founded and we need to continue cycle.</returns>
        private static bool IsQuotedPosition(string expression, ref bool inValueQuotes, ref bool inIdentifierQuotes, ref int i)
        {
            // Value quote founded
            if (expression[i] == ValueQuote && !inIdentifierQuotes)
            {
                // Check for escaped quotes if we are in quotes already
                if (inValueQuotes)
                {
                    // Quote is escaped using escape symbol
                    if (i > 1 && expression[i - 1] == EscapeSymbol)
                        return true;

                    // Quote is escaped using second quote
                    if (i < expression.Length - 1 && expression[i + 1] == ValueQuote)
                    {
                        i++;
                        return true;
                    }
                }

                //Switch in quote flag
                inValueQuotes = !inValueQuotes;
            }

            // Identifier quote found
            if (expression[i] == IdentifierQuote && !inValueQuotes)
            {
                // Check for escaped quotes if we are in quotes already
                if (inIdentifierQuotes)
                {
                    // Quote is escaped using escape symbol
                    if (i > 1 && expression[i - 1] == EscapeSymbol)
                        return true;

                    // Quote is escaped using second quote
                    if (i < expression.Length - 1 && expression[i + 1] == IdentifierQuote)
                    {
                        i++;
                        return true;
                    }
                }

                //Switch in quote flag
                inIdentifierQuotes = !inIdentifierQuotes;
            }
            return false;
        }
        #endregion

        #region Constants
        
        #region Symbols
        /// <summary>
        /// Symbol used to quote values.
        /// </summary>
        public const char ValueQuote = '\'';
        /// <summary>
        /// Symbol used to quote identifiers.
        /// </summary>
        public const char IdentifierQuote = '`';
        /// <summary>
        /// Symbol used to escape quotes.
        /// </summary>
        public const char EscapeSymbol = '\\';
        /// <summary>
        /// Open brace symbol.
        /// </summary>
        public const char OpenBrace = '(';
        /// <summary>
        /// Close brace symbol.
        /// </summary>
        public const char CloseBrace = ')';
        /// <summary>
        /// Close brace symbol.
        /// </summary>
        public const char Undescore = '_';

        /// <summary>
        /// White spaces
        /// </summary>
        public static readonly char[] WhiteSpaces = new char[] { ' ', '\n', '\t', '\r' };
        /// <summary>
        /// Delimiters
        /// </summary>
        public static readonly char[] Delimiters = new char[] { ';', '.', ',' };
        /// <summary>
        /// Forbidden identifier symbols
        /// </summary>
        public static readonly char[] Forbidden = new char[] { '.', '\\', '/' };
        #endregion

        #region Datatypes
        // Numeric types
        public const string TINYINT = "TINYINT";
        public const string SMALLINT = "SMALLINT";
        public const string MEDIUMINT = "MEDIUMINT";
        public const string INT = "INT";
        public const string INTEGER = "INTEGER";
        public const string BIGINT = "BIGINT";
        public const string REAL = "REAL";
        public const string FLOAT = "FLOAT";
        public const string DECIMAL = "DECIMAL";
        public const string NUMERIC = "NUMERIC";
        public const string BIT = "BIT";

        // Character types
        public const string CHAR = "CHAR";
        public const string VARCHAR = "VARCHAR";
        public const string TINYTEXT = "TINYTEXT";
        public const string TEXT = "TEXT";
        public const string MEDIUMTEXT = "MEDIUMTEXT";
        public const string LONGTEXT = "LONGTEXT";

        // Binary types
        public const string BINARY = "BINARY";
        public const string VARBINARY = "VARBINARY";
        public const string TINYBLOB = "TINYBLOB";
        public const string BLOB = "BLOB";
        public const string MEDIUMBLOB = "MEDIUMBLOB";
        public const string LONGBLOB = "LONGBLOB";

        // MySQL types
        public const string ENUM = "ENUM";
        public const string SET = "SET";

        // Spatial types
        public const string GEOMETRY = "GEOMETRY";
        public const string POINT = "POINT";
        public const string LINESTRING = "LINESTRING";
        public const string POLYGON = "POLYGON";

        // Spatial collections
        public const string GEOMETRYCOLLECTION = "GEOMETRYCOLLECTION";
        public const string MULTIPOINT = "MULTIPOINT";
        public const string MULTILINESTRING = "MULTILINESTRING";
        public const string MULTIPOLYGON = "MULTIPOLYGON";
        
        // Date and time
        public const string DATE = "DATE";
        public const string TIME = "TIME";
        public const string DATETIME = "DATETIME";
        public const string TIMESTAMP = "TIMESTAMP";
        #endregion

        #endregion
    }
}
