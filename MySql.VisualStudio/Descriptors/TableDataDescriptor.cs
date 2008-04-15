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
 * This file contains implementation of the descriptor for the table data pseudo-object. 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using MySql.Data.VisualStudio.Utils;

namespace MySql.Data.VisualStudio.Descriptors
{
    /// <summary>
    /// This is descriptor for table data pseudo-object it is used to transparently integrate 
    /// table data editing in the provider infrastructure.
    /// </summary>
    [ObjectDescriptor(TableDataDescriptor.TypeName, typeof(TableDataDescriptor))]
    [IdLength(3)]
    class TableDataDescriptor : ObjectDescriptor
    {
        #region Type name
        /// <summary>A type name for the descriptor</summary>
        public new const string TypeName = "TableData";
        #endregion

        #region Enumerate SQL
        /// <summary>
        /// This SQL enumerates table and views with their names and IS_UPDATABLE option.
        /// </summary>
        protected new const string EnumerateSqlTemplate =
            "SELECT "
            + "t.TABLE_CATALOG, "
            + "t.TABLE_SCHEMA, "
            + "t.TABLE_NAME, "
            + "IF(t.TABLE_TYPE = 'BASE TABLE' OR v.IS_UPDATABLE = 'YES', 'YES', 'NO') AS IS_UPDATABLE "
            + "FROM information_schema.`TABLES` t "
            + "LEFT OUTER JOIN information_schema.VIEWS v "
            + "ON t.TABLE_SCHEMA = v.TABLE_SCHEMA "
            + "AND t.TABLE_NAME = v.TABLE_NAME "
            + "WHERE t.TABLE_SCHEMA = {1} AND t.TABLE_NAME = {2}";

        /// <summary>
        /// Default restrictions.
        /// </summary>
        protected new static readonly string[] DefaultRestrictions =
		{
			"",
            "t.TABLE_SCHEMA",
			"t.TABLE_NAME"			
		};

        /// <summary>
        /// DEfault sorting.
        /// </summary>
        protected new const string DefaultSortString = "";
        #endregion

        #region Legacy MySQL version support
        /// <summary>
        /// Reads table with Database Objects which satisfy given restriction. Uses table enumeration for legacy
        /// version.
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
                return base.ReadTable(connection, restrictions, sort);;

            // For legacy version enumerate tables
            DataTable result = TableDescriptor.Enumerate(connection, restrictions);

            // If result is empty, return it
            if (result == null)
                return null;

            // Add is updatable column
            result.Columns.Add(Attributes.IsUpdatable, typeof(string));

            // Set is updatable to true for each row
            foreach (DataRow table in result.Rows)
                DataInterpreter.SetValueIfChanged(table, Attributes.IsUpdatable, DataInterpreter.True);

            // Finaly, return result
            return result;
        }
        #endregion

        #region Attributes
        /// <summary>
        /// Attributes of the table data pseudo-object
        /// </summary>
        public static new class Attributes
        {
            public const string Database = "TABLE_CATALOG";
            [Identifier(IsSchema = true)]
            public const string Schema = "TABLE_SCHEMA";
            [Identifier(IsName = true)]
            public const string Name = "TABLE_NAME";
            public const string IsUpdatable = "IS_UPDATABLE";
        }
        #endregion

        #region Enumerate method
        /// <summary>
        /// Enumerates tables with given restrictions into DataTable.
        /// </summary>
        /// <param name="connection">The DataConnectionWrapper to be used for enumeration.</param>
        /// <param name="restrictions">The restrictions to be putted on the retrieved objects set.</param>
        /// <returns>
        /// Returns DataTable which contains all tables which satisfy given restrictions.
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
