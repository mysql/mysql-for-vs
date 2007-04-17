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
 * This file contains the implementation of the descriptor for the stored procedure 
 * parameter database object.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace MySql.Data.VisualStudio.Descriptors
{
    /// <summary>
    /// The descriptor for the stored procedure parameter database object
    /// </summary>
    [ObjectDescriptor(StoredProcParameterDescriptor.TypeName, typeof(StoredProcParameterDescriptor))]
    [IdLength(5)]
    class StoredProcParameterDescriptor : AdoNet20Descriptor
    {
        #region Type name
        /// <summary>Type name of the descriptor</summary>
        public new const string TypeName = "StoredProcedureParameter";
        #endregion

        #region Overriden enumeration
        /// <summary>
        /// Returns name of collection to use in GetSchema call.
        /// </summary>
        protected override string CollectionName
        {
            get { return "Procedure Parameters"; }
        }

        /// <summary>
        /// Stored procedures are supported only since 5.0
        /// </summary>
        public override Version RequiredVersion
        {
            get
            {
                return new Version(5, 0);
            }
        }

        /// <summary>
        /// In extention method we replace DbNull's on ReturnValue.
        /// </summary>
        /// <param name="connection">The DataConnectionWrapper to be used for enumeration.</param>
        /// <param name="table">Table with data to extend.</param>
        protected override void PostProcessData(DataConnectionWrapper connection, DataTable table)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (table == null)
                throw new ArgumentNullException("table");

            // Replacing null values with empty strings; it's required for return 
            // values of functions are anonymous and therefore have null names. Null 
            // values are forbidden for primary key columns
            foreach (DataRow dr in table.Rows)
            {
                if (dr[Attributes.Name] == DBNull.Value)
                    dr[Attributes.Name] = "ReturnValue";
            }

            // Call to base method
            base.PostProcessData(connection, table);
        }
        #endregion
        
        #region Attributes
        /// <summary>
        /// Attributes of the stored procedure parameter object
        /// </summary>
        public static new class Attributes
        {
            [Field(FieldType = TypeCode.String)]
            public const string Database = "ROUTINE_CATALOG";            
            [Identifier(IsSchema = true)]
            [Field(FieldType = TypeCode.String)]
            public const string Schema = "ROUTINE_SCHEMA";
            [Identifier]
            [Field(FieldType = TypeCode.String)]
            public const string RoutineName = "ROUTINE_NAME";
            [Identifier]
            [Field(FieldType = TypeCode.String)]
            public const string RoutineType = "ROUTINE_TYPE";
            [Identifier(IsName = true)]
            [Field(FieldType = TypeCode.String)]
            public const string Name = "PARAMETER_NAME";
            [Field(FieldType = TypeCode.Int64)]
            public const string OrdinalPosition = "ORDINAL_POSITION";
            [Field(FieldType = TypeCode.String)]
            public const string Mode = "PARAMETER_MODE";
            [Field(FieldType = TypeCode.Boolean)]
            public const string IsResult = "IS_RESULT";
            [Field(FieldType = TypeCode.String)]
            public const string DataType = "DATA_TYPE";
            [Field(FieldType = TypeCode.String)]
            public const string Flags = "FLAGS";
            [Field(FieldType = TypeCode.String)]
            public const string CharacterSet = "CHARACTER_SET";
            [Field(FieldType = TypeCode.Int64)]
            public const string CharacterMaxLength = "CHARACTER_MAXIMUM_LENGTH";
            [Field(FieldType = TypeCode.Int64)]
            public const string Precision = "NUMERIC_PRECISION";
            [Field(FieldType = TypeCode.Int64)]
            public const string Scale = "NUMERIC_SCALE";
        }
        #endregion

        #region Enumerate method
        /// <summary>
        /// Enumerates stored procedure parameters with given restrictions into DataTable.
        /// </summary>
        /// <param name="connection">The DataConnectionWrapper to be used for enumeration.</param>
        /// <param name="restrictions">The restrictions to be putted on the retrieved objects set.</param>
        /// <returns>
        /// Returns DataTable which contains all stored procedure parameters which satisfy given restrictions.
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
