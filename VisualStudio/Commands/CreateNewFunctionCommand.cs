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
 * This file contains implementation of create new function command handler.
 */
using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.VisualStudio.DocumentView;
using MySql.Data.VisualStudio.Descriptors;
using System.Globalization;
using MySql.Data.VisualStudio.Properties;
using System.Diagnostics;
using MySql.Data.VisualStudio.Utils;

namespace MySql.Data.VisualStudio.Commands
{
    /// <summary>
    /// Implementation of create new function command. Overrides identifier creation 
    /// method and sets document to be a function rather than a procedure.
    /// </summary>
    [CommandHandler(GuidList.guidMySqlProviderCmdSetString, GuidList.cmdidCreateFunction, typeof(CreateNewFunctionCommand))]
    class CreateNewFunctionCommand: CreateCommand
    {
        /// <summary>
        /// Creates new stored function identifier template without name part.
        /// </summary>
        /// <param name="hierarchy">Server explorer facade object to be used for Server Explorer hierarchy interaction.</param>
        /// <param name="typeName">Object type name to create identifier.</param>
        /// <returns>Returns new stored function identifier template without name part.</returns>
        protected override object[] CreateNewIDBase(ServerExplorerFacade hierarchy, string typeName)
        {
            if (hierarchy == null)
                throw new ArgumentNullException("hierarchy");
            if (typeName == null)
                throw new ArgumentNullException("typeName");
            
            // Only stored procedures are supported
            if (!DataInterpreter.CompareInvariant(typeName, StoredProcDescriptor.TypeName))
                throw new NotSupportedException(String.Format(
                                CultureInfo.CurrentCulture,
                                Resources.Error_ObjectTypeNotSupported,
                                typeName));

            // Get default schema
            string schema = hierarchy.Connection.Schema;
            Debug.Assert(!String.IsNullOrEmpty(schema), "Failed to retrive schema name!");
            if (schema == null)
                return null;

            return new object[] { null, schema, StoredProcDescriptor.Function, null };
        }

        /// <summary>
        /// Returns overriden new name template for stored functions.
        /// </summary>
        /// <param name="typeName">Type name of the object.</param>
        /// <param name="id">Identifier base for the object created so far.</param>
        /// <returns>
        /// Returns overriden new name template for stored functions.
        /// </returns>
        protected override string GetTemplate(string typeName, object[] id)
        {
            return "Function";
        }
    }
}
