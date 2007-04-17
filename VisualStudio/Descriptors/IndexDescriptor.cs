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
 * This file contains the implementation of the descriptor for the index 
 * database object. 
 */
using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.VisualStudio.Utils;
using System.Data;
using System.Diagnostics;
using MySql.Data.VisualStudio.Properties;
using System.Globalization;

namespace MySql.Data.VisualStudio.Descriptors
{
    /// <summary>
    /// The descriptor for the index database object.
    /// </summary>
    [ObjectDescriptor(IndexDescriptor.TypeName, typeof(IndexDescriptor))]
    [IdLength(4)]
    class IndexDescriptor : ObjectDescriptor
    {
        #region Type name
        /// <summary>Type name of the descriptor</summary>
        public new const string TypeName = "Index";
        #endregion

        #region Overriden enumeration
        /// <summary>
        /// Reads list of indexes using SHOW INDEX FROM query. Enumerates tables using
        /// given restrictions first and then reads indexes for each table.
        /// </summary>
        /// <param name="connection">The DataConnectionWrapper to be used for enumeration.</param>
        /// <param name="restrictions">The restrictions to be putted on the retrieved objects set.</param>
        /// <param name="sort">Sort expresion to append after ORDER BY clause.</param>
        /// <returns>Returns table with Database Objects which satisfy given restriction.</returns>
        protected override DataTable ReadTable(DataConnectionWrapper connection, object[] restrictions, string sort)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            DataTable result = PrepareTable();

            // Fill restrictions for the tables
            object[] tableRestrictions = restrictions;
            // If necessary, cut the restrictions
            if (restrictions != null && restrictions.Length > 3)
            {
                tableRestrictions = new object[3];
                tableRestrictions[0] = restrictions[0];
                tableRestrictions[1] = restrictions[1];
                tableRestrictions[2] = restrictions[2];
            }

            // Read tables
            DataTable tables = ObjectDescriptor.EnumerateObjects(connection, TableDescriptor.TypeName, tableRestrictions, sort);
            if (tables == null || tables.Rows == null)
                return result;
            
            // StringBuilder for query
            StringBuilder query = new StringBuilder(EnumerateSql);

            // Extract indexes for all tables
            foreach (DataRow table in tables.Rows)
            {
                // Truncate query
                if (query.Length > EnumerateSql.Length)
                    query.Remove(EnumerateSql.Length, query.Length - EnumerateSql.Length);

                // Write table idintifier
                QueryBuilder.WriteIdentifier(
                    DataInterpreter.GetStringNotNull(table, TableDescriptor.Attributes.Schema),
                    DataInterpreter.GetStringNotNull(table, TableDescriptor.Attributes.Name),
                    query);

                // Execute query to select indexes
                DataTable indexes = connection.ExecuteSelectTable(query.ToString());
                if (indexes == null || indexes.Rows == null)
                {
                    Debug.Fail("Failed to read indexes using query:\n" + query.ToString());
                    continue;
                }

                // Fetch data
                FetchData(restrictions, table, indexes, result);
            }

            // Accept changes for result
            result.AcceptChanges();
            
            // Return resulting table
            return result;
        }
        #endregion

        #region Protected methods
        /// <summary>
        /// Returns DataTable with initialized schema.
        /// </summary>
        /// <returns>Returns DataTable with initialized schema.</returns>
        protected virtual DataTable PrepareTable()
        {
            // Initiaize schema table
            DataTable result = new DataTable("Indexes");
            result.Columns.Add(Attributes.Database, typeof(string));
            result.Columns.Add(Attributes.Schema, typeof(string));
            result.Columns.Add(Attributes.Table, typeof(string));
            result.Columns.Add(Attributes.Name, typeof(string));
            result.Columns.Add(Attributes.Unique, typeof(string));
            result.Columns.Add(Attributes.Primary, typeof(string));
            result.Columns.Add(Attributes.IndexType, typeof(string));
            return result;
        }

        /// <summary>
        /// Fetches data from the results of the SHOW INDEX FROM query to the 
        /// DataTable with database objects descriptions.
        /// </summary>
        /// <param name="restrictions">Restricton to apply to the read objects.</param>
        /// <param name="table">DataRow with table descriptions for which indexes are enumerated.</param>
        /// <param name="indexes">DataTable with results of the SHOW INDEX FROM.</param>
        /// <param name="result">DataTable to fill with data.</param>
        protected virtual void FetchData(object[] restrictions, DataRow table, DataTable indexes, DataTable result)
        {
            if (table == null)
                throw new ArgumentNullException("table");
            if (indexes == null)
                throw new ArgumentNullException("indexes");
            if (result == null)
                throw new ArgumentNullException("result");

            // Iterate through results of SHOW INDEX FROM
            foreach (DataRow index in indexes.Rows)
            {
                // Only first columns should be considered
                if (DataInterpreter.GetInt(index, SeqInIndex) != 1)
                    continue;

                // Apply restrictions on the index name, if any
                if (!CheckIndexName(restrictions, index))
                    continue;

                // Create new index row
                DataRow row = result.NewRow();

                // Fill row with data
                DataInterpreter.SetValueIfChanged(row, Attributes.Database, null);
                DataInterpreter.SetValueIfChanged(row, Attributes.Schema, table[TableDescriptor.Attributes.Schema]);
                DataInterpreter.SetValueIfChanged(row, Attributes.Table, index[Table]);
                DataInterpreter.SetValueIfChanged(row, Attributes.Name, index[KeyName]);
                DataInterpreter.SetValueIfChanged(row, Attributes.Unique,
                    DataInterpreter.GetInt(index, NonUnique) == 0 ?
                    DataInterpreter.True : DataInterpreter.False);
                DataInterpreter.SetValueIfChanged(row, Attributes.Primary,
                    DataInterpreter.CompareInvariant(DataInterpreter.GetStringNotNull(index, KeyName), PRIMARY) ?
                    DataInterpreter.True : DataInterpreter.False);
                DataInterpreter.SetValueIfChanged(row, Attributes.IndexType, index[IndexType]);
                
                // Add filled index row to the results table
                result.Rows.Add(row);
            }
        }

        /// <summary>
        /// Returns true if given DataRow conforms to the restriction on the index name.
        /// </summary>
        /// <param name="restrictions">Restrictions to check.</param>
        /// <param name="index">DataRow from SHOW INDEX FROM results to be checked.</param>
        /// <returns>Returns true if given DataRow conforms to the restriction on the index name.</returns>
        protected static bool CheckIndexName(object[] restrictions, DataRow index)
        {
            return !(restrictions != null && restrictions.Length >= 4 && restrictions[3] != null
                                && !DataInterpreter.CompareObjects(index[KeyName], restrictions[3]));
        }
        #endregion

        #region Attributes
        /// <summary>
        /// Attributes of the index object
        /// </summary>
        public static new class Attributes
        {
            public const string Database = "INDEX_CATALOG";
            [Identifier(IsSchema = true)]
            public const string Schema = "INDEX_SCHEMA";
            [Identifier]
            public const string Table = "TABLE_NAME";
            [Identifier(IsName = true)]
            public const string Name = "INDEX_NAME";
            public const string Unique = "UNIQUE";
            public const string Primary = "PRIMARY";
            public const string IndexType = "INDEX_TYPE";
            [Field(OptionName = "index_kind", FieldType = TypeCode.String)]
            public const string IndexKind = "INDEX_KIND";
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
        private const string IndexKindField = "index_kind";
        #endregion
        
        #region Index types
        public const string BTREE = "BTREE";
        public const string HASH = "HASH";
        #endregion

        #region Index kinds
        public const string INDEX = "INDEX";
        public const string PRIMARY = "PRIMARY";
        public const string UNIQUE = "UNIQUE";
        public const string FULLTEXT = "FULLTEXT";
        public const string SPATIAL = "SPATIAL";
        #endregion

        #region Enumeration query and columns
        private const string EnumerateSql = "SHOW INDEX FROM ";

        protected const string SeqInIndex = "SEQ_IN_INDEX";
        protected const string KeyName = "KEY_NAME";
        protected const string Table = "TABLE";
        protected const string NonUnique = "NON_UNIQUE";
        protected const string IndexType = "INDEX_TYPE";
        protected const string ColumnName = "COLUMN_NAME";
        protected const string SubPart = "SUB_PART";
        #endregion

        #region Index kind extraction
        /// <summary>
        /// Parses create table and extracts index kind (INDEX, PRIMARY, UNIQUE, SPATIAL, FULLTEXT).
        /// </summary>
        /// <param name="connection">The DataConnectionWrapper to be used for enumeration.</param>
        /// <param name="row">DataRow to extract values.</param>
        /// <returns>Returns field values for given DataRow.</returns>
        protected override Dictionary<string, string> ExtractOptions(DataConnectionWrapper connection, System.Data.DataRow row)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (row == null)
                throw new ArgumentNullException("row");

            // Call to base
            Dictionary<string, string> result = base.ExtractOptions(connection, row);

            // Default is index
            result[IndexKindField] = INDEX;

            // Check if this is primary key
            if (DataInterpreter.GetSqlBool(row, Attributes.Primary))
            {
                result[IndexKindField] = PRIMARY;
                return result;
            }

            // Check if this is unique constraint
            if (DataInterpreter.GetSqlBool(row, Attributes.Unique))
            {
                result[IndexKindField] = UNIQUE;
                return result;
            }

            // Extract CREATE TABLE sql
            string createTableQuery = GetCreateTableQuery(connection, row);
            if (String.IsNullOrEmpty(createTableQuery))
                return result;

            // Locate beginig of the definition.
            int pos = LocateDefinition(row, createTableQuery);
            if (pos < 0)
            {
                Debug.Fail("Unable to locate begining of the definition!");
                return result;
            }

            // Truncate and trim string
            createTableQuery = createTableQuery.Substring(0, pos).Trim();

            // Check for spatial
            if (createTableQuery.EndsWith(SPATIAL, StringComparison.InvariantCultureIgnoreCase))
                result[IndexKindField] = SPATIAL;

            // Check for fulltext
            if (createTableQuery.EndsWith(FULLTEXT, StringComparison.InvariantCultureIgnoreCase))
                result[IndexKindField] = FULLTEXT;

            return result;
        }

        /// <summary>
        /// Locates position of the definition for given index in the owner table create script.
        /// </summary>
        /// <param name="row">DataRow with information about index.</param>
        /// <param name="createTableQuery">String with CREATE TABLE script.</param>
        /// <returns>Returns position of the definition for the index.</returns>
        private static int LocateDefinition(DataRow row, string createTableQuery)
        {
            // Build beginig of the definition.
            StringBuilder queryPart = new StringBuilder();
            queryPart.Append("KEY ");
            QueryBuilder.WriteIdentifier(row, Attributes.Name, queryPart);
            string subExpression = queryPart.ToString();

            // Locate begining of the definition
            int pos = Parser.LocateUnquoted(createTableQuery, subExpression);
            if (pos < 0)
            {
                Debug.Fail("Unable to locate index definition!");
                return pos;
            }

            return pos;
        }

        /// <summary>
        /// Returns string with CREATE TABLE sql for the table which owns this index.
        /// </summary>
        /// <param name="connection">Connection to use to execute query.</param>
        /// <param name="row">DataRow with information about index.</param>
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
