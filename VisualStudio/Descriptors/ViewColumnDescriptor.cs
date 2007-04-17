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

using System.Data;
using System;
namespace MySql.Data.VisualStudio.Descriptors
{
    /// <summary>
    /// The descriptor of database view's column
    /// </summary>
    [ObjectDescriptor(ViewColumnDescriptor.TypeName, typeof(ViewColumnDescriptor))]
    [IdLength(4)]
    public class ViewColumnDescriptor : ObjectDescriptor
    {
        #region Type name
        /// <summary>Type name of the descriptor</summary>
        public new const string TypeName = "ViewColumn";
        #endregion

        #region Constants for the SQL request
        /// <summary>Template of the request itself</summary>
        protected new const string EnumerateSqlTemplate =
            "SELECT " +
            "NULL as VIEW_CATALOG, " +
            "C.TABLE_SCHEMA AS VIEW_SCHEMA, " +
            "C.TABLE_NAME AS VIEW_NAME, " +
            "C.COLUMN_NAME, " +
            "C.ORDINAL_POSITION, " +
            "C.COLUMN_DEFAULT, " +
            "C.IS_NULLABLE, " +
            "C.DATA_TYPE, " +
            "C.CHARACTER_MAXIMUM_LENGTH, " +
            "C.CHARACTER_OCTET_LENGTH, " +
            "C.NUMERIC_PRECISION, " +
            "C.NUMERIC_SCALE, " +
            "C.CHARACTER_SET_NAME, " +
            "C.COLLATION_NAME, " +
            "C.COLUMN_TYPE, " +
            "C.COLUMN_KEY, " +
            "C.EXTRA, " +
            "C.`PRIVILEGES`, " +
            "C.COLUMN_COMMENT " +
            "FROM information_schema.COLUMNS C " +
            "JOIN information_schema.VIEWS V " +
            "ON C.TABLE_SCHEMA = V.TABLE_SCHEMA AND C.TABLE_NAME = V.TABLE_NAME " +
            "WHERE C.TABLE_SCHEMA = {1} AND C.TABLE_NAME = {2} AND C.COLUMN_NAME = {3}";

        /// <summary>Default restrictions</summary>
        protected new static readonly string[] DefaultRestrictions = 
		{
			"", "C.TABLE_SCHEMA", "C.TABLE_NAME", "C.COLUMN_NAME"
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
        /// Attributes of the View object
        /// </summary>
        public static new class Attributes
        {
            [Field(FieldType = TypeCode.String)]
            public const string Database = "VIEW_CATALOG";
            [Field(FieldType = TypeCode.String)]
            [Identifier(IsSchema = true)]            
            public const string Schema = "VIEW_SCHEMA";
            [Field(FieldType = TypeCode.String)]
            [Identifier]
            public const string Table = "VIEW_NAME";
            [Identifier(IsName=true)]
            [Field(FieldType = TypeCode.String)]
            public const string Name = "COLUMN_NAME";
            [Field(FieldType = TypeCode.Int64)]
            public const string Ordinal = "ORDINAL_POSITION";
            [Field(FieldType = TypeCode.String)]
            public const string Default = "COLUMN_DEFAULT";
            [Field(FieldType = TypeCode.Boolean)]
            public const string Nullable = "IS_NULLABLE";
            [Field(FieldType = TypeCode.String)]
            public const string SqlType = "DATA_TYPE";
            [Field(FieldType = TypeCode.Int64)]
            public const string Length = "CHARACTER_MAXIMUM_LENGTH";
            [Field(FieldType = TypeCode.Int64)]
            public const string OctetLength = "CHARACTER_OCTET_LENGTH";
            [Field(FieldType = TypeCode.Int64)]
            public const string Precision = "NUMERIC_PRECISION";
            [Field(FieldType = TypeCode.Int64)]
            public const string Scale = "NUMERIC_SCALE";
            [Field(FieldType = TypeCode.String)]
            public const string CharacterSet = "CHARACTER_SET_NAME";
            [Field(FieldType = TypeCode.String)]
            public const string Collation = "COLLATION_NAME";
            [Field(FieldType = TypeCode.String)]
            public const string MySqlType = "COLUMN_TYPE";
            [Field(FieldType = TypeCode.String)]
            public const string ColumnKey = "COLUMN_KEY";
            [Field(FieldType = TypeCode.String)]
            public const string Extra = "EXTRA";
            [Field(FieldType = TypeCode.String)]
            public const string Privileges = "PRIVILEGES";
            [Field(FieldType = TypeCode.String)]
            public const string Comments = "COLUMN_COMMENT";
        }
        #endregion

        #region Enumerate method
        /// <summary>
        /// Enumerates view columns with given restrictions into DataTable.
        /// </summary>
        /// <param name="connection">The DataConnectionWrapper to be used for enumeration.</param>
        /// <param name="restrictions">The restrictions to be putted on the retrieved objects set.</param>
        /// <returns>
        /// Returns DataTable which contains all columns which satisfy given restrictions.
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