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
 * This file contains descriptor for the foreign key database object.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using MySql.Data.VisualStudio.Utils;
using System.Diagnostics;

namespace MySql.Data.VisualStudio.Descriptors
{
    /// <summary>
    /// The descriptor for foreign key database object.
    /// </summary>
    [ObjectDescriptor(ForeignKeyDescriptor.TypeName, typeof(ForeignKeyDescriptor))]
    [IdLength(4)]
    class ForeignKeyDescriptor: ObjectDescriptor
    {
        #region Type name
        /// <summary>Type name of the descriptor</summary>
        public new const string TypeName = "ForeignKey";
        #endregion

        #region Constants for the SQL request
        /// <summary>Template of the request itself</summary>
        protected new const string EnumerateSqlTemplate =
            "SELECT "
            + "tc.CONSTRAINT_CATALOG, "
            + "tc.CONSTRAINT_SCHEMA, "
            + "tc.CONSTRAINT_NAME, "
            + "tc.TABLE_SCHEMA, "
            + "tc.TABLE_NAME, "
            + "tc.CONSTRAINT_CATALOG AS REFERENCED_TABLE_CATALOG, "
            + "kcu.REFERENCED_TABLE_SCHEMA, "
            + "kcu.REFERENCED_TABLE_NAME, "
            + "'' AS CONSTRAINT_OPTIONS "
            + "FROM information_schema.TABLE_CONSTRAINTS tc "
            + "LEFT JOIN information_schema.KEY_COLUMN_USAGE kcu "
            + "ON kcu.CONSTRAINT_SCHEMA = tc.CONSTRAINT_SCHEMA "
            + "AND kcu.CONSTRAINT_NAME = tc.CONSTRAINT_NAME "
            + "AND kcu.POSITION_IN_UNIQUE_CONSTRAINT = 1 "            
            + "WHERE tc.CONSTRAINT_TYPE = 'FOREIGN KEY' "
            + "AND tc.CONSTRAINT_SCHEMA = {1} "
            + "AND tc.TABLE_NAME = {2} "
            + "AND tc.CONSTRAINT_NAME = {3}";

        /// <summary>Default restrictions</summary>
        protected new static readonly string[] DefaultRestrictions = 
		{
			"", "tc.TABLE_SCHEMA", "tc.TABLE_NAME", "tc.CONSTRAINT_NAME"
		};

        /// <summary>Default sort fields</summary>
        protected new const string DefaultSortString = "";
        #endregion

        #region Attributes
        /// <summary>
        /// Attributes of the foreign keys object
        /// </summary>
        public static new class Attributes
        {
            [Field(FieldType = TypeCode.String)]
            public const string Database = "CONSTRAINT_CATALOG";
            [Field(FieldType = TypeCode.String)]
            [Identifier(IsSchema=true)]
            public const string Schema = "CONSTRAINT_SCHEMA";
            [Field(FieldType = TypeCode.String)]
            [Identifier]
            public const string Table = "TABLE_NAME";
            [Field(FieldType = TypeCode.String)]
            [Identifier(IsName = true)]
            public const string Name = "CONSTRAINT_NAME";
            [Field(FieldType = TypeCode.String)]
            public const string TableSchema = "TABLE_SCHEMA";
            [Field(FieldType = TypeCode.String)]
            public const string ReferencedTableCatalog = "REFERENCED_TABLE_CATALOG";
            [Field(FieldType = TypeCode.String)]
            public const string ReferencedTableSchema = "REFERENCED_TABLE_SCHEMA";
            [Field(FieldType = TypeCode.String)]
            public const string ReferencedTableName = "REFERENCED_TABLE_NAME";
            [Field(FieldType = TypeCode.String)]
            [OptionString]
            public const string Options = "CONSTRAINT_OPTIONS";
            [Field(OptionName = OnDeleteField, FieldType = TypeCode.String)]
            public const string OnDelete = "ON_DELETE_ACTION";
            [Field(OptionName = OnUpdateField, FieldType = TypeCode.String)]
            public const string OnUpdate = "ON_UPDATE_ACTION";

        }
        #endregion

        #region Virtual property
        /// <summary>
        /// The lowest version of the MySQL Server where the descriptor's object 
        /// appears
        /// </summary>
        public override Version RequiredVersion
        {
            get
            {
                return new Version(5, 0);
            }
        }
        #endregion

        #region Enumerate method
        /// <summary>
        /// Enumerates foreign keys with given restrictions into DataTable.
        /// </summary>
        /// <param name="connection">The DataConnectionWrapper to be used for enumeration.</param>
        /// <param name="restrictions">The restrictions to be putted on the retrieved objects set.</param>
        /// <returns>
        /// Returns DataTable which contains all foreign keys which satisfy given restrictions.
        /// </returns>
        public static DataTable Enumerate(DataConnectionWrapper connection, object[] restrictions)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            return ObjectDescriptor.EnumerateObjects(connection, TypeName, restrictions);
        }
        #endregion

        #region Private constants
        private const string OnDeleteField = "on_delete";
        private const string OnUpdateField = "on_update";
        private const string ON = "ON";
        private const string Delete = "DELETE";
        private const string Update = "UPDATE";
        private const string OnDelete = ON + " " + Delete;
        private const string OnUpdate = ON + " " +Update;
        #endregion

        #region Actions values
        public const string RESTRICT = "RESTRICT";
        public const string CASCADE = "CASCADE";
        public const string SETNULL = "SET NULL";
        public const string NOACTION = "NO ACTION";
        #endregion

        #region OnDelete and OnUpdate parsing
        /// <summary>
        /// Parses create table and extract values for on delete and on update fields.
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

            // Call to base 
            Dictionary<string, string> result = base.ExtractOptions(connection, row);
            if (result == null)
                result = new Dictionary<string, string>();

            // Restrict by default
            result.Add(OnDeleteField, RESTRICT);
            result.Add(OnUpdateField, RESTRICT);

            // Extract CREATE TABLE sql
            string createTableQuery = GetCreateTableQuery(connection, row);
            if (String.IsNullOrEmpty(createTableQuery))
                return result;

            int pos = LocateFlagsPosition(row, createTableQuery);
            if (pos < 0)
            {
                Debug.Fail("Unable to locate begining of the flags!");
                return result;
            }

            // Read first token, if not ON, then exit.
            string token = Parser.ExtractToken(createTableQuery, pos);
            if (!DataInterpreter.CompareInvariant(token, ON))
                return result;

            // Read value for the first field
            pos += token.Length + 1;
            string value = ReadAndFillAction(createTableQuery, pos, result);
            if (value == null)
                return result;

            // Read value for the second field (note that ON DELETE and ON UPDATE has the same length)
            pos += OnDelete.Length + value.Length + 2;
            value = ReadAndFillAction(createTableQuery, pos, result);

            // Return result
            return result;
        }

        /// <summary>
        /// Reads one action flag and initialize proper field.
        /// </summary>
        /// <param name="sql">String with CREATE TABLE script.</param>
        /// <param name="pos">Position of the field to read in the string.</param>
        /// <param name="fields">Dictionary with fields to right to.</param>
        /// <returns>Returns read value.</returns>
        private static string ReadAndFillAction(string sql, int pos, Dictionary<string, string> fields)
        {
            // Read field name from sql
            string token = Parser.ExtractToken(sql, pos);
            if (token == null)
                return null;

            // Select field name for dictionary
            string fieldName = null;
            if (DataInterpreter.CompareInvariant(token, Delete))
                fieldName = OnDeleteField;
            if (DataInterpreter.CompareInvariant(token, Update))
                fieldName = OnUpdateField;

            // Check if field recognized
            if (fieldName == null)
                return null;

            // Read value from sql (note that DELETE and UPDATE has the same length)
            token = Parser.ExtractToken(sql, pos + Delete.Length + 1);

            // Determine value
            string value = null;
            if (DataInterpreter.CompareInvariant(token, RESTRICT))
                value = RESTRICT;
            if (DataInterpreter.CompareInvariant(token, CASCADE))
                value = CASCADE;
            if (DataInterpreter.CompareInvariant(token, "SET"))
                value = SETNULL;
            if (DataInterpreter.CompareInvariant(token, "NO"))
                value = NOACTION;

            // Check if value recognized
            if (value == null)
                return null;

            // Set field and return value
            fields[fieldName] = value;
            return value;

        }

        /// <summary>
        /// Locates position of the action flags definition for given foreign key
        /// int hte owner table create script.
        /// </summary>
        /// <param name="row">DataRow with information about foreign key.</param>
        /// <param name="createTableQuery">String with CREATE TABLE script.</param>
        /// <returns>Returns position of the firs action flag for the foreign key.</returns>
        private static int LocateFlagsPosition(DataRow row, string createTableQuery)
        {
            // Build beginig of the definition.
            StringBuilder queryPart = new StringBuilder();
            queryPart.Append("CONSTRAINT ");
            QueryBuilder.WriteIdentifier(row, Attributes.Name, queryPart);
            queryPart.Append(" FOREIGN KEY ");
            string subExpression = queryPart.ToString();

            // Locate begining of the definition
            int pos = Parser.LocateUnquoted(createTableQuery, subExpression);
            if (pos < 0)
            {
                Debug.Fail("Unable to locate foreign key definition!");
                return pos;
            }

            // Build references part
            queryPart.Remove(0, queryPart.Length);
            queryPart.Append(" REFERENCES ");
            QueryBuilder.WriteIdentifier(row, Attributes.ReferencedTableName, queryPart);
            subExpression = queryPart.ToString();

            // Locate references part
            pos = Parser.LocateUnquoted(createTableQuery, subExpression, pos);
            if (pos < 0)
            {
                Debug.Fail("Unable to locate references definition!");
                return pos;
            }

            // Locate closing brace for referenced column list
            pos = Parser.LocateUnquoted(createTableQuery, ")", pos);
            if (pos < 0)
            {
                Debug.Fail("Unable to locate closing brace for referenced column list!");
                return pos;
            }
            
            // Flags mus be right behind this closing brace and one space
            return pos + 2;
        }

        /// <summary>
        /// Returns string with CREATE TABLE sql for the table which owns this key.
        /// </summary>
        /// <param name="connection">Connection to use to execute query.</param>
        /// <param name="row">DataRow with information about table.</param>
        /// <returns>Returns string with CREATE TABLE sql for this table.</returns>
        private static string GetCreateTableQuery(DataConnectionWrapper connection, DataRow row)
        {
            // Extract schema and table name
            string schemaName = DataInterpreter.GetString(row, Attributes.Schema);
            string tableName = DataInterpreter.GetString(row, Attributes.Table);
            if (String.IsNullOrEmpty(schemaName) || String.IsNullOrEmpty(tableName))
            {
                Debug.Fail("Unable to get table or schema name");
                return String.Empty;
            }

            return TableDescriptor.GetCreateTableQuery(connection, schemaName, tableName);
        }
        #endregion
    }
}
