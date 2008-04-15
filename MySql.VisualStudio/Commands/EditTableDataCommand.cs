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
 * This file contains implementation of the handler for the "Edit Table Data" command
 */
using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.VisualStudio.Utils;
using MySql.Data.VisualStudio.Descriptors;

namespace MySql.Data.VisualStudio.Commands
{
    /// <summary>
    /// This is a command handler for the "Edit Table Data" command/
    /// </summary>
    [CommandHandler(GuidList.guidMySqlProviderCmdSetString, GuidList.cmdidEditTableData, typeof(EditTableDataCommand))]
    class EditTableDataCommand: OpenEditorCommand
    {
        #region Overridings
        /// <summary>
        /// Returns identifier of the given node.
        /// </summary>
        /// <param name="hierarchy">Server explorer facade object to be used for Server Explorer hierarchy interaction.</param>
        /// <param name="item">Item identifier.</param>
        /// <returns>Returns hierarchy item ID for editor.</returns>
        protected override int GetItemForEditor(ServerExplorerFacade hierarchy, int item)
        {
            if (item < 0)
                throw new ArgumentOutOfRangeException("item");

            // Item is always the same as the given item
            return item;
        }

        /// <summary>
        /// Always returns false.
        /// </summary>
        /// <param name="hierarchy">Server explorer facade object to be used for Server Explorer hierarchy interaction.</param>
        /// <param name="item">Item identifier.</param>
        /// <returns>Returns true, if new object will be created as a result f this command.</returns>
        protected override bool GetIsNewFlagForObject(ServerExplorerFacade hierarchy, int item)
        {
            // Always not new object
            return false;
        }

        /// <summary>
        /// Returns identifier of the object for the given node.
        /// </summary>
        /// <param name="hierarchy">Server explorer facade object to be used for Server Explorer hierarchy interaction.</param>
        /// <param name="item">Item identifier.</param>
        /// <returns>Returns array with multipart identifier for the object.</returns>
        protected override object[] GetObjectID(ServerExplorerFacade hierarchy, string typeName, int item)
        {
            if (hierarchy == null)
                throw new ArgumentNullException("hierarchy");
            if (item < 0)
                throw new ArgumentOutOfRangeException("item");
            if (typeName == null)
                throw new ArgumentNullException("typeName");

            // If this is not a table or a view, data editing is not supported.
            if (!DataInterpreter.CompareInvariant(typeName, TableDataDescriptor.TypeName))
                return null;

            // ID is the object ID of the hierarchy item
            return hierarchy.GetObjectIdentifier(item);
        }

        /// <summary>
        /// Checks object type for the given node (must be Table or View) and returns TableData
        /// it if checking succeeds. 
        /// </summary>
        /// <param name="hierarchy">Server explorer facade object to be used for Server Explorer hierarchy interaction.</param>
        /// <param name="item">Item identifier.</param>
        /// <returns>
        /// Returns TableData if object type name of given hierarchy item is Table or View.
        /// Otherwize returns null.
        /// </returns>
        protected override string GetObjectType(ServerExplorerFacade hierarchy, int item)
        {
            if (hierarchy == null)
                throw new ArgumentNullException("hierarchy");
            if (item < 0)
                throw new ArgumentOutOfRangeException("item");

            // Get type of the current node
            string typeName = hierarchy.GetObjectType(item);

            // If this is a table or a view, data editing is supported.
            if (DataInterpreter.CompareInvariant(typeName, TableDescriptor.TypeName)
                || DataInterpreter.CompareInvariant(typeName, ViewDescriptor.TypeName))
                return TableDataDescriptor.TypeName;

            // Else data editing is not supported
            return null;
        } 
        #endregion
    }
}
