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
 * This file contains implementation of the foreign key column descriptor.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace MySql.Data.VisualStudio.Descriptors
{
    /// <summary>
    /// The descriptor for foreign key column database object.
    /// </summary>
    [ObjectDescriptor(ForeignKeyColumnDescriptor.TypeName, typeof(ForeignKeyColumnDescriptor))]
    [IdLength(5)]
    class ForeignKeyColumnDescriptor: ObjectDescriptor
    {
        #region Type name
        /// <summary>Type name of the descriptor</summary>
        public new const string TypeName = "ForeignKeyColumn";
        #endregion

        #region Constants for the SQL request
        /// <summary>Template of the request itself</summary>
        protected new const string EnumerateSqlTemplate =
            "SELECT "
            + "kcu.CONSTRAINT_CATALOG, "
            + "kcu.CONSTRAINT_SCHEMA, "
            + "kcu.TABLE_CATALOG, "
            + "kcu.TABLE_SCHEMA, "
            + "kcu.TABLE_NAME, "
            + "kcu.CONSTRAINT_NAME, "
            + "kcu.COLUMN_NAME, "
            + "kcu.ORDINAL_POSITION, "
            + "kcu.POSITION_IN_UNIQUE_CONSTRAINT, "
            + "kcu.CONSTRAINT_CATALOG AS REFERENCED_TABLE_CATALOG, "
            + "kcu.REFERENCED_TABLE_SCHEMA, "
            + "kcu.REFERENCED_TABLE_NAME, "
            + "kcu.REFERENCED_COLUMN_NAME "
            + "FROM information_schema.KEY_COLUMN_USAGE kcu "
            + "LEFT JOIN information_schema.TABLE_CONSTRAINTS tc "
            + "ON kcu.CONSTRAINT_SCHEMA = tc.CONSTRAINT_SCHEMA "
            + "AND kcu.CONSTRAINT_NAME = tc.CONSTRAINT_NAME "
            + "WHERE tc.CONSTRAINT_TYPE = 'FOREIGN KEY' "
            + "AND kcu.CONSTRAINT_SCHEMA = {1} "
            + "AND kcu.TABLE_NAME = {2} "
            + "AND kcu.CONSTRAINT_NAME = {3} "
            + "AND kcu.COLUMN_NAME = {4} ";

        /// <summary>Default restrictions</summary>
        protected new static readonly string[] DefaultRestrictions = 
		{
			"", "kcu.CONSTRAINT_SCHEMA", "kcu.TABLE_NAME", "kcu.CONSTRAINT_NAME", "kcu.COLUMN_NAME"
		};

        /// <summary>Default sort fields</summary>
        protected new const string DefaultSortString = "";
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

        #region Attributes
        /// <summary>
        /// Attributes of the foreign key column object
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
            [Identifier]
            public const string ForeignKeyName = "CONSTRAINT_NAME";
            [Field(FieldType = TypeCode.String)]
            [Identifier(IsName = true)]
            public const string Name = "COLUMN_NAME";
            [Field(FieldType = TypeCode.String)]
            public const string TableCatalog = "TABLE_CATALOG";
            [Field(FieldType = TypeCode.String)]
            public const string TableSchema = "TABLE_SCHEMA";
            [Field(FieldType = TypeCode.Int64)]
            public const string OrdinalPosition = "ORDINAL_POSITION";
            [Field(FieldType = TypeCode.Int64)]
            public const string PositionInKey = "POSITION_IN_UNIQUE_CONSTRAINT";
            [Field(FieldType = TypeCode.String)]
            public const string ReferencedTableCatalog = "REFERENCED_TABLE_CATALOG";
            [Field(FieldType = TypeCode.String)]
            public const string ReferencedTableSchema = "REFERENCED_TABLE_SCHEMA";
            [Field(FieldType = TypeCode.String)]
            public const string ReferencedTableName = "REFERENCED_TABLE_NAME";
            [Field(FieldType = TypeCode.String)]
            public const string ReferencedColumn = "REFERENCED_COLUMN_NAME";
        }
        #endregion

        #region Enumerate method
        /// <summary>
        /// Enumerates foreign key columns with given restrictions into DataTable.
        /// </summary>
        /// <param name="connection">The DataConnectionWrapper to be used for enumeration.</param>
        /// <param name="restrictions">The restrictions to be putted on the retrieved objects set.</param>
        /// <returns>
        /// Returns DataTable which contains all foreign key columns which satisfy given restrictions.
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
