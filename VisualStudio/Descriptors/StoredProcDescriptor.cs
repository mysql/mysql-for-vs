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

using System;
using System.Data;
using System.Text;
using MySql.Data.VisualStudio.Utils;
using System.Globalization;
using MySql.Data.VisualStudio.Properties;

namespace MySql.Data.VisualStudio.Descriptors
{
    /// <summary>
    /// Stored procedure descriptor
    /// </summary>
    [ObjectDescriptor(StoredProcDescriptor.TypeName, typeof(StoredProcDescriptor))]
    [IdLength(3)]
    public class StoredProcDescriptor : ObjectDescriptor
    {
        #region Type name
        /// <summary>A type name of the descriptor</summary>
        public new const string TypeName = "StoredProcedure";
        #endregion

        #region Constants for the SQL request
        /// <summary>Template of the request itself</summary>
        protected new const string EnumerateSqlTemplate =
            "SELECT " +
            "r.ROUTINE_CATALOG, " +
            "r.ROUTINE_SCHEMA, " +
            "r.ROUTINE_TYPE, " +
            "r.ROUTINE_NAME, " +
            "r.SPECIFIC_NAME, " +
            "r.DTD_IDENTIFIER, " +
            "r.ROUTINE_BODY, " +
            "r.ROUTINE_DEFINITION, " +
            "r.EXTERNAL_NAME, " +
            "r.EXTERNAL_LANGUAGE, " +
            "r.PARAMETER_STYLE, " +
            "r.IS_DETERMINISTIC, " +
            "r.SQL_DATA_ACCESS, " +
            "r.SQL_PATH, " +
            "r.SECURITY_TYPE, " +
            "r.CREATED, " +
            "r.LAST_ALTERED, " +
            "r.SQL_MODE, " +
            "r.ROUTINE_COMMENT, " +
            "r.`DEFINER`, " +
            "CONVERT (p.PARAM_LIST, CHAR) AS PARAM_LIST " +
            "FROM information_schema.ROUTINES r " +
            "JOIN mysql.PROC p ON r.SPECIFIC_NAME = p.SPECIFIC_NAME " +
            "AND r.ROUTINE_SCHEMA = p.DB " +
            "AND r.ROUTINE_TYPE = p.`TYPE` " +
            "WHERE r.ROUTINE_SCHEMA = {1} AND r.ROUTINE_TYPE = {2} AND r.ROUTINE_NAME = {3}";

        /// <summary>Default restrictions</summary>
        protected new static readonly string[] DefaultRestrictions = 
		{
			"", "r.ROUTINE_SCHEMA", "r.ROUTINE_TYPE", "r.ROUTINE_NAME"
		};

        /// <summary>Default sort fields</summary>
        protected new const string DefaultSortString = "";
        #endregion

        #region Public constants
        /// <summary>Type of a stored procedure</summary>
        public const string Procedure = "PROCEDURE";

        /// <summary>Type of a scalar function</summary>
        public const string Function = "FUNCTION";
        #endregion

        #region Attributes
        /// <summary>
        /// Attributes of the View object
        /// </summary>
        public static new class Attributes
        {
            [Identifier(IsSchema=true)]
            [Field(FieldType = TypeCode.String)]
            public const string Schema = "ROUTINE_SCHEMA";
            [Identifier(IsName = true)]
            [Field(FieldType = TypeCode.String)]
            public const string Name = "ROUTINE_NAME";
            [Identifier]
            [Field(FieldType = TypeCode.String)]
            public const string Type = "ROUTINE_TYPE";
            [Field(FieldType = TypeCode.String)]
            public const string SpecificName = "SPECIFIC_NAME";
            [Field(FieldType = TypeCode.String)]
            public const string Returns = "DTD_IDENTIFIER";
            [Field(FieldType = TypeCode.String)]
            public const string Body = "ROUTINE_BODY";
            [Field(FieldType = TypeCode.String)]
            public const string Definition = "ROUTINE_DEFINITION";
            [Field(FieldType = TypeCode.String)]
            public const string ExternalName = "EXTERNAL_NAME";
            [Field(FieldType = TypeCode.String)]
            public const string ExternalLanguage = "EXTERNAL_LANGUAGE";
            [Field(FieldType = TypeCode.String)]
            public const string ParameterStyle = "PARAMETER_STYLE";
            [Field(FieldType = TypeCode.String)]
            public const string IsDeterministic = "IS_DETERMINISTIC";
            [Field(FieldType = TypeCode.String)]
            public const string DataAccess = "SQL_DATA_ACCESS";
            [Field(FieldType = TypeCode.String)]
            public const string SqlPath = "SQL_PATH";
            [Field(FieldType = TypeCode.String)]
            public const string SecurityType = "SECURITY_TYPE";
            [Field(FieldType = TypeCode.DateTime)]
            public const string CreationTime = "CREATED";
            [Field(FieldType = TypeCode.DateTime)]
            public const string LastModified = "LAST_ALTERED";
            [Field(FieldType = TypeCode.String)]
            public const string Mode = "SQL_MODE";
            [Field(FieldType = TypeCode.String)]
            public const string Comment = "ROUTINE_COMMENT";
            [Field(FieldType = TypeCode.String)]
            public const string Definer = "DEFINER";
            [Field(FieldType = TypeCode.String)]
            public const string ParameterList = "PARAM_LIST";
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

        #region Dropping
        /// <summary>
        /// Procedures can be dropped. Returns true.
        /// </summary>
        public override bool CanBeDropped
        {
            get { return true; }
        }

        /// <summary>
        /// Returns DROP PROCEDURE/FUNCTION statement.
        /// </summary>
        /// <param name="identifier">Database object identifier.</param>
        /// <returns>Returns DROP TABLE statement.</returns>
        public override string BuildDropSql(object[] identifier)
        {
            if (identifier == null)
                throw new ArgumentNullException("identifier");
            if (identifier.Length != 4 || String.IsNullOrEmpty(identifier[1] as string)
                || String.IsNullOrEmpty(identifier[2] as string) || String.IsNullOrEmpty(identifier[3] as string))
                throw new ArgumentException(
                    String.Format(
                        CultureInfo.CurrentCulture,
                        Resources.Error_InvlaidIdentifier,
                        identifier.Length,
                        TypeName,
                        4),
                     "identifier");

            // Build query
            StringBuilder query = new StringBuilder("DROP ");
            query.Append(identifier[2]);
            query.Append(' ');

            QueryBuilder.WriteIdentifier(identifier[1] as string, identifier[3] as string, query);
            return query.ToString();
        }
        #endregion

        #region Enumerate method
        /// <summary>
        /// Enumerates stored procedures with given restrictions into DataTable
        /// </summary>
        /// <param name="connection">DataConnectionWrapper to be used for the 
        /// enumeration</param>
        /// <param name="restrictions">Restrictions to be applied to the retrieved 
        /// objects set</param>
        /// <returns>DataTable containing all stored procedures which satisfy the 
        /// given restrictions</returns>
        public static DataTable Enumerate(DataConnectionWrapper connection, object[] restrictions)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            return ObjectDescriptor.EnumerateObjects(connection, TypeName, restrictions);
        }
        #endregion
    }
}