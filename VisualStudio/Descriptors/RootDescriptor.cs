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
 * This file contains implementation of schema descriptor
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Diagnostics;
using MySql.Data.VisualStudio.Utils;

namespace MySql.Data.VisualStudio.Descriptors
{
    /// <summary>
    /// Descriptor for the root database object - schema.
    /// </summary>
    [ObjectDescriptor(RootDescriptor.TypeName, typeof(RootDescriptor))]
    [IdLength(2)]
    public class RootDescriptor: ObjectDescriptor
    {
        /// <summary>
        /// Root type name.
        /// </summary>
        public new const string TypeName = "";

        #region Enumerate SQL
        /// <summary>
        /// Schema enumeration template.
        /// </summary>
        protected new const string EnumerateSqlTemplate =
            "SELECT {0} AS SERVER_NAME, NULL AS CATALOG_NAME, database() AS SCHEMA_NAME";
/*            "SELECT "
            + "{0} AS SERVER_NAME, "
            + "CATALOG_NAME, "
            + "SCHEMA_NAME, "
            + "DEFAULT_CHARACTER_SET_NAME, "
            + "DEFAULT_COLLATION_NAME, "
            + "SQL_PATH "
            + "FROM information_schema.SCHEMATA "
            + "WHERE SCHEMA_NAME = {1} ";*/
        
        /// <summary>
        /// Schema enumeration defaults.
        /// </summary>
        protected new static readonly string[] DefaultRestrictions =
		{
			"RIGHT(USER(), LENGTH(USER()) - INSTR(USER(),'@'))",
            "DATABASE()"
		};

        /// <summary>
        /// Schema default sort fields.
        /// </summary>
        protected new const string DefaultSortString = "";
        #endregion

        #region Legacy MySQL version support
        /// <summary>
        /// Returns enumeration SQL template for a given server version.
        /// </summary>
        /// <param name="connection">The DataConnectionWrapper to be used for enumeration.</param>
        /// <returns>Returns enumeration SQL template for a given server version.</returns>
        protected override string GetEnumerateSqlTemplate(DataConnectionWrapper connection)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            // Extract server version
            Version serverVersion = connection.ServerVersion;

            // For latest version just call base
            if( serverVersion == null || serverVersion.Major >= 5 )
                return base.GetEnumerateSqlTemplate(connection);

            // For legacy version use SHOW DATABASES
            return EnumerateSqlTemplate; // "SHOW DATABASES LIKE {1}";
        }

        /// <summary>
        /// Returns default enumerate restrictions for a given server version.
        /// </summary>
        /// <param name="connection">The DataConnectionWrapper to be used for enumeration.</param>
        /// <returns>Returns default enumerate restrictions for a given server version.</returns>
        protected override string[] GetDefaultRestrictions(DataConnectionWrapper connection)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            // Extract server version
            Version serverVersion = connection.ServerVersion;

            // For latest version just call base
            if (serverVersion == null || serverVersion.Major >= 5)
                return base.GetDefaultRestrictions(connection);

            // For legacy version return array with current conection information
            return new string[] { connection.ServerName, "'" + connection.Schema + "'" };
        }

        /// <summary>
        /// Post process enumeration data. Check server version and add aditional processing for
        /// legacy version.
        /// </summary>
        /// <param name="connection">The DataConnectionWrapper to be used for enumeration.</param>
        /// <param name="table">Table with data to post process.</param>
        protected override void PostProcessData(DataConnectionWrapper connection, DataTable table)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (table == null)
                throw new ArgumentNullException("table");

            // Extract server version
/*            Version serverVersion = connection.ServerVersion;
            
            // For legacy version rename column returned from SHOW DATABASES to SCHEMA_NAME
            if (serverVersion != null && serverVersion.Major < 5)
            {
                // Table is strange, call base method to try to process it
                if (table.Columns == null || table.Columns.Count != 1)
                {
                    Debug.Fail("Table has invalid columns list. It was not returned by SHOW DATABASES!");
                    base.PostProcessData(connection, table);
                    return;
                }

                // Rename only one column to SCHEMA_NAME
                table.Columns[0].ColumnName = Attributes.Schema;
            }
            
            base.PostProcessData(connection, table);*/
        }

        /// <summary>
        /// Extracts field values for given DataRow. For legacy version extract server name, character set and collation.
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

            // Call base to extract fields
            Dictionary<string, string> result = base.ExtractOptions(connection, row);

            // Extract server version
            Version serverVersion = connection.ServerVersion;

            // For latest version just return base results
            if (serverVersion == null || serverVersion.Major >= 5)
                return result;

            // Add server name
            result[ServerField] = connection.ServerName;

            //Extract CREATE DATABASE
            string createDatabase = GetCreateDatabase(connection, row);
            if (String.IsNullOrEmpty(createDatabase))
                return result;

            // Extract default character set
            Parser.ExtractAdvancedFieldToken(result, createDatabase, "DEFAULT CHARACTER SET ", DefaultCharsetField);
            // If default character set is not extracted, return
            if (!result.ContainsKey(DefaultCharsetField))
                return result;

            // Extract default collation
            Parser.ExtractAdvancedFieldToken(result, createDatabase, "COLLATE ", DefaultCollationField);
            // If default collation is not extracted, use default collation for character set
            if (!result.ContainsKey(DefaultCollationField))
            {
                // Extract default character set field
                string defaultCharSet = result[DefaultCharsetField];
                // Extract default collation for this character set
                if (!String.IsNullOrEmpty(defaultCharSet))
                {
                    string defaultCollation = connection.GetDefaultCollationForCharacterSet(defaultCharSet);
                    if (!String.IsNullOrEmpty(defaultCollation))
                        result[DefaultCollationField] = defaultCollation;
                }
            }            

            // Return result
            return result;            
        }

        /// <summary>
        /// Returns CREATE DATABASE statement for given schema.
        /// </summary>
        /// <param name="connection">Connection to use to execute query.</param>
        /// <param name="schema">DataRow with information about schema.</param>
        /// <returns>Returns CREATE DATABASE statement for given schema.</returns>
        private string GetCreateDatabase(DataConnectionWrapper connection, DataRow schema)
        {
            // Extract schema and table name
            string schemaName = DataInterpreter.GetString(schema, Attributes.Schema);
            if (String.IsNullOrEmpty(schemaName))
            {
                Debug.Fail("Unable to get schema name");
                return String.Empty;
            }

            // Build SHOW CREATE DATABASE
            StringBuilder query = new StringBuilder();
            query.Append("SHOW CREATE DATABASE ");
            QueryBuilder.WriteIdentifier(schemaName, query);

            // Execute query and check result table
            DataTable createDatabase = connection.ExecuteSelectTable(query.ToString());
            if (createDatabase == null || createDatabase.Rows.Count <= 0)
            {
                Debug.Fail("Failed to read CREATE TABLE query!");
                return String.Empty;
            }

            // Extract result
            string result = DataInterpreter.GetString(createDatabase.Rows[0], "Create Database");

            // Dispose table and exit
            createDatabase.Dispose();
            return result;
        }
        #endregion

        #region Attributes
        /// <summary>
        /// List of known attributes for root object
        /// </summary>
        public static new class Attributes
        {
            [Identifier]
            [Field(OptionName = ServerField, FieldType = TypeCode.String)]
            public const string Server = "SERVER_NAME";

            [Field(OptionName = DatabaseField, FieldType = TypeCode.String)]
            public const string Database = "CATALOG_NAME";

            [Identifier(IsName=true,IsSchema=true)]
            public const string Schema = "SCHEMA_NAME";

            [Field(OptionName = DefaultCharsetField, FieldType = TypeCode.String)]
            public const string DefaultCharset = "DEFAULT_CHARACTER_SET_NAME";

            [Field(OptionName = DefaultCollationField, FieldType = TypeCode.String)]
            public const string DefaultCollation = "DEFAULT_COLLATION_NAME";

            [Field(OptionName = SqlPathField, FieldType = TypeCode.String)]
            public const string SqlPath = "SQL_PATH";            
        }
        #endregion

        #region Field names
        private const string ServerField = "server";
        private const string DatabaseField = "database";
        private const string DefaultCharsetField = "defaultcharset";
        private const string DefaultCollationField = "defaultcollation";
        private const string SqlPathField = "sqlpath";
        #endregion

        #region Enumerate method
        /// <summary>
        /// Enumerates schemas with given restrictions into DataTable.
        /// </summary>
        /// <param name="connection">The DataConnectionWrapper to be used for enumeration.</param>
        /// <param name="restrictions">The restrictions to be putted on the retrieved objects set.</param>
        /// <returns>
        /// Returns DataTable which contains all schemas which satisfy given restrictions.
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
