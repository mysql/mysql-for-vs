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
 * This file contains the implementation of the descriptor for the index column
 * database object. 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using MySql.Data.VisualStudio.Utils;

namespace MySql.Data.VisualStudio.Descriptors
{
    /// <summary>
    /// The descriptor for the index column database object.
    /// </summary>
    [ObjectDescriptor(IndexColumnDescriptor.TypeName, typeof(IndexColumnDescriptor))]
    [IdLength(5)]
    class IndexColumnDescriptor : IndexDescriptor
    {
        #region Type name
        /// <summary>Type name of the descriptor</summary>
        public new const string TypeName = "IndexColumn";
        #endregion

        #region Overriden enumeration
        /// <summary>
        /// Returns DataTable with initialized schema.
        /// </summary>
        /// <returns>Returns DataTable with initialized schema.</returns>
        protected override DataTable PrepareTable()
        {
            DataTable result = new DataTable("IndexColumns");
            result.Columns.Add(Attributes.Database, typeof(string));
            result.Columns.Add(Attributes.Schema, typeof(string));
            result.Columns.Add(Attributes.Table, typeof(string));
            result.Columns.Add(Attributes.Index, typeof(string));
            result.Columns.Add(Attributes.Name, typeof(string));
            result.Columns.Add(Attributes.Ordinal, typeof(Int64));
            result.Columns.Add(Attributes.IndexLength, typeof(Int64));

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
        protected override void FetchData(object[] restrictions, DataRow table, DataTable indexes, DataTable result)
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
                // Check index name restriction
                if (!CheckIndexName(restrictions, index))
                    continue;
                // Check index column name restriction
                if (!CheckIndexColumnName(restrictions, index)) 
                    continue;

                // Create new row for index column
                DataRow row = result.NewRow();
                
                // Extract data
                DataInterpreter.SetValueIfChanged(row, Attributes.Database, null);
                DataInterpreter.SetValueIfChanged(row, Attributes.Schema, table[TableDescriptor.Attributes.Schema]);
                DataInterpreter.SetValueIfChanged(row, Attributes.Table, index[Table]);
                DataInterpreter.SetValueIfChanged(row, Attributes.Index, index[KeyName]);
                DataInterpreter.SetValueIfChanged(row, Attributes.Name, index[ColumnName]);
                DataInterpreter.SetValueIfChanged(row, Attributes.Ordinal, index[SeqInIndex]);
                DataInterpreter.SetValueIfChanged(row, Attributes.IndexLength, index[SubPart]);

                // Add new row to the results table
                result.Rows.Add(row);
            }
        }

        /// <summary>
        /// Returns true if given DataRow conforms to the restriction on the index column name.
        /// </summary>
        /// <param name="restrictions">Restrictions to check.</param>
        /// <param name="index">DataRow from SHOW INDEX FROM results to be checked.</param>
        /// <returns>Returns true if given DataRow conforms to the restriction on the index column name.</returns>
        private static bool CheckIndexColumnName(object[] restrictions, DataRow index)
        {
            return !(restrictions != null && restrictions.Length == 5 && restrictions[4] != null
                && !index[ColumnName].Equals(restrictions[4]));
        }
        #endregion

        #region Attributes
        /// <summary>
        /// Attributes of the index column object
        /// </summary>
        public static new class Attributes
        {
            public const string Database = "INDEX_CATALOG";
            [Identifier(IsSchema = true)]
            public const string Schema = "INDEX_SCHEMA";
            [Identifier]
            public const string Table = "TABLE_NAME";
            [Identifier]
            public const string Index = "INDEX_NAME";
            [Identifier(IsName=true)]
            public const string Name = "COLUMN_NAME";
            public const string Ordinal = "ORDINAL_POSITION";
            public const string IndexLength = "INDEX_LENGTH";
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
        public static new DataTable Enumerate(DataConnectionWrapper connection, object[] restrictions)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            return ObjectDescriptor.EnumerateObjects(connection, TypeName, restrictions);
        }
        #endregion
    }
}
