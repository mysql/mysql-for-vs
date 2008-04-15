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
using System.Globalization;
using MySql.Data.VisualStudio.Properties;
using System.Text;
using MySql.Data.VisualStudio.Utils;

namespace MySql.Data.VisualStudio.Descriptors
{
    /// <summary>
    /// The descriptor of database views
    /// </summary>
    [ObjectDescriptor(ViewDescriptor.TypeName, typeof(ViewDescriptor))]
    [IdLength(3)]
    public class ViewDescriptor : ObjectDescriptor
    {
        #region Type name
        /// <summary>Type name of the descriptor</summary>
        public new const string TypeName = "View";
        #endregion

        #region Constants for the SQL request
        /// <summary>Template of the request itself</summary>
        protected new const string EnumerateSqlTemplate =
            "SELECT " +
            "TABLE_CATALOG, " +
            "TABLE_SCHEMA, " +
            "TABLE_NAME, " +
            "VIEW_DEFINITION, " +
            "CHECK_OPTION, " +
            "IS_UPDATABLE, " +
            "`DEFINER`, " +
            "SECURITY_TYPE, " +
            "'' AS ALGORITHM " +
            "FROM information_schema.`VIEWS` " +
            "WHERE TABLE_SCHEMA = {1} AND TABLE_NAME = {2}";
        
        /// <summary>Default restrictions</summary>
        protected new static readonly string[] DefaultRestrictions = 
		{
			"", "TABLE_SCHEMA", "TABLE_NAME"
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

        #region Dropping
        /// <summary>
        /// Views can be dropped. Returns true.
        /// </summary>
        public override bool CanBeDropped
        {
            get { return true; }
        }

        /// <summary>
        /// Returns DROP VIEW statement.
        /// </summary>
        /// <param name="identifier">Database object identifier.</param>
        /// <returns>Returns DROP VIEW statement.</returns>
        public override string BuildDropSql(object[] identifier)
        {
            if (identifier == null)
                throw new ArgumentNullException("identifier");
            if (identifier.Length != 3 || String.IsNullOrEmpty(identifier[1] as string) || String.IsNullOrEmpty(identifier[2] as string))
                throw new ArgumentException(
                    String.Format(
                        CultureInfo.CurrentCulture,
                        Resources.Error_InvlaidIdentifier,
                        identifier.Length,
                        TypeName,
                        3),
                     "id");

            // Build query
            StringBuilder query = new StringBuilder("DROP VIEW ");
            QueryBuilder.WriteIdentifier(identifier[1] as string, identifier[2] as string, query);
            return query.ToString();
        }
        #endregion

        #region Attributes
        /// <summary>
        /// Attributes of the View object
        /// </summary>
        public static new class Attributes
        {
            [Field(FieldType = TypeCode.String)]
            public const string Database = "TABLE_CATALOG";
            [Field(FieldType = TypeCode.String)]
            [Identifier(IsSchema = true)]
            public const string Schema = "TABLE_SCHEMA";
            [Field(FieldType = TypeCode.String)]
            [Identifier(IsName=true)]
            public const string Name = "TABLE_NAME";
            [Field(FieldType = TypeCode.String)]
            public const string Definition = "VIEW_DEFINITION";
            [Field(FieldType = TypeCode.String)]
            public const string CheckOption = "CHECK_OPTION";
            [Field(FieldType = TypeCode.String)]
            public const string IsUpdatable = "IS_UPDATABLE";
            [Field(FieldType = TypeCode.String)]
            public const string Definer = "DEFINER";
            [Field(FieldType = TypeCode.String)]
            public const string SecurityType = "SECURITY_TYPE";
            [Field(FieldType = TypeCode.String)]
            public const string Algorithm = "ALGORITHM";
        }
        #endregion

        #region Enumerate method
        /// <summary>
        /// Enumerates views with given restrictions into DataTable.
        /// </summary>
        /// <param name="connection">The DataConnectionWrapper to be used for enumeration.</param>
        /// <param name="restrictions">The restrictions to be putted on the retrieved objects set.</param>
        /// <returns>
        /// Returns DataTable which contains all views which satisfy given restrictions.
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
