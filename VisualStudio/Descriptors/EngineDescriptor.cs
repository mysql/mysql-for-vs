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

namespace MySql.Data.VisualStudio.Descriptors
{
    /// <summary>
    /// Descriptor for Engine type. Not used in the Data Object Support XML,
    /// But use to get list of availabel character sets for combobox.
    /// </summary>
    [ObjectDescriptor(EngineDescriptor.TypeName, typeof(EngineDescriptor))]
    [IdLength(1)]
    class EngineDescriptor : ObjectDescriptor
    {
        /// <summary>
        /// Engine type name.
        /// </summary>        
        public new const string TypeName = "Engine";

        #region Enumerate SQL
        /// <summary>
        /// Engine enumeration template.
        /// </summary>
        protected new const string EnumerateSqlTemplate = "SHOW ENGINES";

        /// <summary>
        /// Engine enumeration defaults.
        /// </summary>
        protected new static readonly string[] DefaultRestrictions =
		{
            ""
		};

        /// <summary>
        /// Engine default sort fields.
        /// </summary>
        protected new const string DefaultSortString = "";
        #endregion

        #region Attributes
        /// <summary>
        /// List of known attributes for Engine object
        /// </summary>
        public static new class Attributes
        {
            [Identifier(IsName=true)]
            public const string Name = "Engine";
            public const string IsSupported = "Support";
            public const string Comments = "Comment";
        }
        #endregion

        #region Enumerate method
        /// <summary>
        /// Enumerates engines with given restrictions into DataTable.
        /// </summary>
        /// <param name="connection">The DataConnectionWrapper to be used for enumeration.</param>
        /// <param name="restrictions">The restrictions to be putted on the retrieved objects set.</param>
        /// <returns>
        /// Returns DataTable which contains all engines which satisfy given restrictions.
        /// </returns>
        public static DataTable Enumerate(DataConnectionWrapper connection, object[] restrictions)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            return ObjectDescriptor.EnumerateObjects(connection, TypeName, restrictions);
        }

        #endregion

        protected override DataTable ReadTable(DataConnectionWrapper connection, 
            object[] restrictions, string sort)
        {
            DataTable dt = base.ReadTable(connection, restrictions, sort);

			return MySqlConnectionSupport.ConvertAllBinaryColumns(dt);
        }
    }
}
