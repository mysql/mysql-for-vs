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
 * This file contains definition of common descriptor interface.
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace MySql.Data.VisualStudio.Descriptors
{
    /// <summary>
    /// Common interface for all descriptor objects. Provides properties for enumeration 
    /// SQL retrieving and general type information. Interface is used only by flexible
    /// mechanisms, f.e. query builder general methods. From specialized classes you may
    /// use descriptors directly.
    /// </summary>
    public interface IObjectDescriptor
    {
        /// <summary>
        /// Amount of parts in the object identifier. Root objects have only three 
        /// parts – catalog name, schema name and their own name.
        /// </summary>
        int IdLength
        {
            get;
        }

        /// <summary>
        /// Type name how it was introduced in DataObject XML file.
        /// </summary>
        string TypeName
        {
            get;
        }
        
        /// <summary>
        /// Returns array with object attributes names.
        /// </summary>
        string[] ObjectAttributes
        {
            get;
        }

        /// <summary>
        /// Returns array with object identifier parts names.
        /// </summary>
        string[] Identifier
        {
            get;
        }

        /// <summary>
        /// Returns name of the Schema attribute for this database object.
        /// </summary>
        string SchemaAttributeName
        {
            get;
        }
        /// <summary>
        /// Returns name of the Name attribute for this database object.
        /// </summary>
        string NameAttributeName
        {
            get;
        }

        /// <summary>
        /// Returns true if objects of this type can be dropped.
        /// </summary>
        bool CanBeDropped
        {
            get;
        }
        
        /// <summary>
        /// The lowest version of the MySQL Server where the descriptor's object 
        /// appears
        /// </summary>
        Version RequiredVersion
        {
            get;
        }

        /// <summary>
        /// Returns DROP statement for a given object.
        /// </summary>
        /// <param name="identifier">Database object identifier.</param>
        /// <returns>Returns DROP statement for a given object.</returns>
        string BuildDropSql(object[] identifier);
    }
}
