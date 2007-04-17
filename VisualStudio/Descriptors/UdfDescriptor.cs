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

namespace MySql.Data.VisualStudio.Descriptors
{
    /// <summary>
    /// A descriptor of user defined function
    /// </summary>
    [ObjectDescriptor(UdfDescriptor.TypeName, typeof(UdfDescriptor))]
    [IdLength(2)]
    public class UdfDescriptor : ObjectDescriptor
    {
        #region Type name
        /// <summary>A type name of the descriptor</summary>
        public new const string TypeName = "UDF";
        #endregion

        #region Constants for the SQL request
        /// <summary>Template of the request itself</summary>
        protected new const string EnumerateSqlTemplate =
            "SELECT " +
            "NULL AS CATALOG, " +
            "NAME, " +
            "RET, " +
            "DL, " +
            "`TYPE` " +
            "FROM mysql.func " +
            "WHERE NAME = {1}";

        /// <summary>Default restrictions</summary>
        protected new static readonly string[] DefaultRestrictions = 
		{
			"", "NAME"
		};

        /// <summary>Default sort fields</summary>
        protected new const string DefaultSortString = "";
        #endregion

        #region Attributes
        /// <summary>
        /// Attributes of the View object
        /// </summary>
        public static new class Attributes
        {
            public const string Database = "CATALOG";
            [Identifier(IsName = true)]
            public const string Name = "NAME";
            public const string Returns = "RET";
            public const string Dll = "DL";
            public const string Type = "TYPE";
        }
        #endregion

        #region Dropping
        /// <summary>
        /// UDF can be dropped. Returns true.
        /// </summary>
        public override bool CanBeDropped
        {
            get { return true; }
        }

        /// <summary>
        /// Returns DROP FUNCTION statement.
        /// </summary>
        /// <param name="identifier">Database object identifier.</param>
        /// <returns>Returns DROP FUNCTION statement.</returns>
        public override string BuildDropSql(object[] identifier)
        {
            if (identifier == null)
                throw new ArgumentNullException("identifier");
            if (identifier.Length != 2 || String.IsNullOrEmpty(identifier[1] as string))
                throw new ArgumentException(
                    String.Format(
                        CultureInfo.CurrentCulture,
                        Resources.Error_InvlaidIdentifier,
                        identifier.Length,
                        TypeName,
                        2),
                     "id");

            // Build query
            return "DROP FUNCTION " + (identifier[1] as string);
        }
        #endregion

        #region Enumerate method
        /// <summary>
        /// Enumerates user defined functions with given restrictions into a DataTable 
        /// object
        /// </summary>
        /// <param name="connection">A DataConnectionWrapper to be used for 
        /// enumeration</param>
        /// <param name="restrictions">Restrictions to be put on the retrieved 
        /// objects' set</param>
        /// <returns>A data table containing all triggers which satisfy the given 
        /// restrictions</returns>
        public static DataTable Enumerate(DataConnectionWrapper connection, object[] restrictions)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            return ObjectDescriptor.EnumerateObjects(connection, TypeName, restrictions);
        }
        #endregion
    }
}