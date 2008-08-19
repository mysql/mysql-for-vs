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
using System.Globalization;

using MySql.Data.VisualStudio.Descriptors;
using MySql.Data.VisualStudio.Properties;
using MySql.Data.VisualStudio.Utils;

namespace MySql.Data.VisualStudio.Commands
{
    /// <summary>
    /// Implements creation of a new user defined function
    /// </summary>
    [CommandHandler(GuidList.guidMySqlProviderCmdSetString, GuidList.cmdidCreateUDF, typeof(CreateUdfCommand))]
    class CreateUdfCommand : CreateCommand
    {
        /// <summary>
        /// Creates a new user defined function identifier template
        /// </summary>
        /// <param name="hierarchy">A server explorer facade object to interact with 
        /// Server Explorer hierarchy</param>
        /// <param name="typeName">An object type name to create</param>
        /// <returns>Returns a new user defined function identifier template</returns>
        protected override object[] CreateNewIDBase(ServerExplorerFacade hierarchy, string typeName)
        {
            if (hierarchy == null)
                throw new ArgumentNullException("hierarchy");
            if (typeName == null)
                throw new ArgumentNullException("typeName");

            // Checking a type name
            if (!DataInterpreter.CompareInvariant(typeName, UdfDescriptor.TypeName))
                throw new NotSupportedException(
                    String.Format(
                        CultureInfo.CurrentCulture,
                        Resources.Error_ObjectTypeNotSupported,
                        typeName)
                    );

            // Get a default schema
            string schema = hierarchy.Connection.Schema;
            if (schema == null)
                return null;

            return new object[] { null, null };
        }
    }
}