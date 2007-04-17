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
using System.Collections.Generic;
using System.Text;
using System.Data;
using MySql.Data.VisualStudio.Utils;

namespace MySql.Data.VisualStudio.Descriptors
{
    /// <summary>
    /// Descriptor for Collation type. Not used in the Data Object Support XML,
    /// But use to get list of availabel collations for combobox.
    /// </summary>
    [ObjectDescriptor(CollationDescriptor.TypeName, typeof(CollationDescriptor))]
    [IdLength(2)]
    class CollationDescriptor : ObjectDescriptor
    {
        /// <summary>
        /// Collation type name.
        /// </summary>        
        public new const string TypeName = "Collation";

        #region Enumerate SQL
        /// <summary>
        /// Collation enumeration template.
        /// </summary>
        protected new const string EnumerateSqlTemplate =
            "SELECT "
            + "COLLATION_NAME AS `Collation`, "
            + "CHARACTER_SET_NAME AS `Charset`, "
            + "ID AS `Id`, "
            + "IS_DEFAULT AS `Default`, "
            + "IS_COMPILED AS `Compiled`, "
            + "SORTLEN AS `Sortlen`"
            + "FROM information_schema.COLLATIONS "
            + "WHERE CHARACTER_SET_NAME = {0} "
            + "AND COLLATION_NAME = {1} "
            + "AND IS_DEFAULT = {2} ";

        /// <summary>
        /// Collation enumeration defaults.
        /// </summary>
        protected new static readonly string[] DefaultRestrictions =
		{
            "CHARACTER_SET_NAME",
            "COLLATION_NAME",
            "IS_DEFAULT"
		};

        /// <summary>
        /// Collation default sort fields.
        /// </summary>
        protected new const string DefaultSortString = "";
        #endregion

        #region Attributes
        /// <summary>
        /// List of known attributes for Collation object
        /// </summary>
        public static new class Attributes
        {
            [Identifier(IsName=true)]
            public const string Name = "Collation";
            [Identifier]
            public const string CharacterSetName = "Charset";
            public const string ID = "Id";
            public const string IsDefault = "Default";
            public const string IsCompiled = "Compiled";
            public const string Sortlen = "Sortlen";
        }
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
            if (serverVersion == null || serverVersion.Major >= 5)
                return base.GetEnumerateSqlTemplate(connection);

            // For legacy version use SHOW DATABASES
            return "SHOW COLLATION LIKE {1}";
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
            return new string[] { "", "'%'" };
        }

        /// <summary>
        /// Reads table with Database Objects which satisfy given restriction. Applies filter for aditional restriction
        /// if MySQL version is less then 5.0.
        /// </summary>
        /// <param name="connection">The DataConnectionWrapper to be used for enumeration.</param>
        /// <param name="restrictions">The restrictions to be putted on the retrieved objects set.</param>
        /// <param name="sort">Sort expresion to append after ORDER BY clause.</param>
        /// <returns>Returns table with Database Objects which satisfy given restriction.</returns>
        protected override DataTable ReadTable(DataConnectionWrapper connection, object[] restrictions, string sort)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            // Use base method to read table
            DataTable result = base.ReadTable(connection, restrictions, sort);

            // If there is now result from bas, return immediately
            if (result == null)
                return result;

            // Extract server version
            Version serverVersion = connection.ServerVersion;

            // For latest version just call base
            if (serverVersion == null || serverVersion.Major >= 5)
                return result;

            // For legacy version apply restrictions to result manualy (first slot - character set name, third slot - flag is default)
            
            // At first check if there is any restrictions
            if (restrictions == null || restrictions.Length <= 0)
                return result;
            if (String.IsNullOrEmpty(restrictions[0] as String)
                && (restrictions.Length < 3 || String.IsNullOrEmpty(restrictions[2] as String)))
                return result;

            // Iterates through rows and filter them
            foreach (DataRow collation in result.Select())
            {
                // Apply character set name filter
                if (!String.IsNullOrEmpty(restrictions[0] as String)
                    && !DataInterpreter.CompareInvariant(
                            restrictions[0] as String,
                            DataInterpreter.GetStringNotNull(collation, Attributes.CharacterSetName)))
                {
                    collation.Delete();
                    continue;
                }

                // Apply is_default constraint
                if (restrictions.Length >= 3 && !String.IsNullOrEmpty(restrictions[2] as String)
                    && !DataInterpreter.CompareInvariant(
                            restrictions[2] as String,
                            DataInterpreter.GetStringNotNull(collation, Attributes.IsDefault)))
                {
                    collation.Delete();
                    continue;
                }
            }

            // Accept changes and return results
            result.AcceptChanges();
            return result;
        }
        #endregion

        #region Enumerate method
        /// <summary>
        /// Enumerates collations with given restrictions into DataTable.
        /// </summary>
        /// <param name="connection">The DataConnectionWrapper to be used for enumeration.</param>
        /// <param name="restrictions">The restrictions to be putted on the retrieved objects set.</param>
        /// <returns>
        /// Returns DataTable which contains all collations which satisfy given restrictions.
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
