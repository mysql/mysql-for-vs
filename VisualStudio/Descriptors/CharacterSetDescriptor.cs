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
 * This file contains character set Database Object descriptor.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace MySql.Data.VisualStudio.Descriptors
{
    /// <summary>
    /// Descriptor for character set type. Not used in the Data Object Support XML,
    /// But use to get list of availabel character sets for combobox.
    /// </summary>
    [ObjectDescriptor(CharacterSetDescriptor.TypeName, typeof(CharacterSetDescriptor))]
    [IdLength(1)]
    class CharacterSetDescriptor: ObjectDescriptor
    {
        /// <summary>
        /// Character set type name.
        /// </summary>        
        public new const string TypeName = "CharacterSet";

        #region Enumerate SQL
        /// <summary>
        /// Character set enumeration template.
        /// </summary>
        protected new const string EnumerateSqlTemplate = "SHOW CHARACTER SET LIKE {0}";

        /// <summary>
        /// Character set enumeration defaults.
        /// </summary>
        protected new static readonly string[] DefaultRestrictions =
		{
            "'%'"
		};

        /// <summary>
        /// Character set default sort fields.
        /// </summary>
        protected new const string DefaultSortString = "";
        #endregion

        #region Attributes
        /// <summary>
        /// List of known attributes for Character set object
        /// </summary>
        public static new class Attributes
        {
            [Identifier(IsName=true)]
            public const string Name = "Charset";
            public const string DefaultCollation = "Default collation";
            public const string Description = "Description";
            public const string MaximumLength = "Maxlen";
        }
        #endregion

        #region Enumerate method
        /// <summary>
        /// Enumerates character sets with given restrictions into DataTable.
        /// </summary>
        /// <param name="connection">The DataConnectionWrapper to be used for enumeration.</param>
        /// <param name="restrictions">The restrictions to be putted on the retrieved objects set.</param>
        /// <returns>
        /// Returns DataTable which contains all character sets which satisfy given restrictions.
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
