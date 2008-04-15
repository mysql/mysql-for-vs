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
 * This file contins implementation of the handler for the "Clone Object" command
 */
using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.VisualStudio.DocumentView;
using System.Diagnostics;

namespace MySql.Data.VisualStudio.Commands
{
    /// <summary>
    /// This is a command handler for the "Clone Object" command
    /// </summary>
    [CommandHandler(GuidList.guidMySqlProviderCmdSetString, GuidList.cmdidCloneTable, typeof(CloneCommand))]
    [CommandHandler(GuidList.guidMySqlProviderCmdSetString, GuidList.cmdidCloneView, typeof(CloneCommand))]
    [CommandHandler(GuidList.guidMySqlProviderCmdSetString, GuidList.cmdidCloneProcedure, typeof(CloneCommand))]
    class CloneCommand: CreateCommand
    {
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

            return hierarchy.GetObjectIdentifier(item);
        }

        /// <summary>
        /// Checks object type for the given node using document/view factory and returns 
        /// it if checking succeeds. 
        /// </summary>
        /// <param name="hierarchy">Server explorer facade object to be used for Server Explorer hierarchy interaction.</param>
        /// <param name="item">Item identifier.</param>
        /// <returns>
        /// Returns object type name, which should be used to create document 
        /// and view objects instances.
        /// </returns>
        protected override string GetObjectType(ServerExplorerFacade hierarchy, int item)
        {
            if (hierarchy == null)
                throw new ArgumentNullException("hierarchy");
            if (item < 0)
                throw new ArgumentOutOfRangeException("item");

            // Get object type for given node
            string typeName = hierarchy.GetObjectType(item);

            // Checks object type using factory
            if (!String.IsNullOrEmpty(typeName)
                && DocumentViewFactory.Instance.IsObjectTypeRegistered(typeName))
                return typeName;

            return null;
        }

        /// <summary>
        /// Firstly creates new document as exists document and then forces it to become a clone under new name.
        /// </summary>
        /// <param name="typeName">
        /// Object type name for the document
        /// </param>
        /// <param name="hierarchy">
        /// Server explorer facade object to be used for Server Explorer hierarchy interaction.
        /// Also used to extract connection.
        /// </param>
        /// <param name="id">
        /// Array with new object identifier.
        /// </param>
        /// <param name="isNew">
        /// Indicates if this instance represents new database object doesn’t fixed in 
        /// database yet.
        /// </param>
        /// <returns>
        /// Returns cloned document object.
        /// </returns>
        protected override IDocument CreateDocument(ServerExplorerFacade hierarchy, string typeName, object[] objectID, bool isNew)
        {
            // Firstly create new document object, but not as new
            IDocument result = base.CreateDocument(hierarchy, typeName, objectID, false);

            // If failed to create document, return null
            if (result == null)
                return null;

            // Create new object name using current identifier as the base
            CompleteNewObjectID(
                hierarchy,
                typeName,
                ref objectID,
                (result.Name != null ? result.Name : String.Empty) + '_');

            // Check that last id part is filled
            if (objectID[objectID.Length - 1] == null)
            {
                Debug.Fail("Failed to generate cloned name!");
                return null;
            }

            // Force object to clone
            result.CloneToName(objectID[objectID.Length - 1].ToString());
            
            return result;
        }
    }
}
