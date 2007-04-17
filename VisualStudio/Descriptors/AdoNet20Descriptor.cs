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
 * This file contains implementation of the base class for all descriptors which
 * are using GetSchema for enumeration.
 */
using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.VisualStudio.Properties;
using System.Data;

namespace MySql.Data.VisualStudio.Descriptors
{
    /// <summary>
    /// Base class for all descriptors, which are using GetSchema instead of direct 
    /// query to INFORMATION_SCHEMA. Expose one abstract property to specify collection 
    /// name, which will be used instead of EnumerationSqlTemplate.
    /// </summary>
    abstract class AdoNet20Descriptor : ObjectDescriptor
    {
        /// <summary>
        /// Returns collection name to use with GetSchema call.
        /// </summary>
        protected abstract string CollectionName
        {
            get;
        }

        /// <summary>
        /// Uses connection GetSchema instead of direct query to INFORMATION_SCHEMA.
        /// </summary>
        /// <param name="connection">The DataConnectionWrapper to be used for enumeration.</param>
        /// <param name="restrictions">The restrictions to be putted on the retrieved objects set.</param>
        /// <param name="sort">Sort expresion to append after ORDER BY clause.</param>
        /// <returns>Returns table with Database Objects which satisfy given restriction.</returns>
        protected override DataTable ReadTable(DataConnectionWrapper connection, object[] restrictions, string sort)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (String.IsNullOrEmpty(CollectionName))
                throw new NotSupportedException(Resources.Error_NoCollectionName);

            if (connection.ServerVersion != null && RequiredVersion > connection.ServerVersion)
                // This object requires a higher version of the MySql Server
                return new DataTable();

            return connection.GetSchema(CollectionName, restrictions);
        }
    }
}
