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

using System.Collections.Generic;
using System.Text;
using MySql.Data.VisualStudio.DocumentView;
using System;

namespace MySql.Data.VisualStudio.Commands
{
    /// <summary>
    /// This class implements "Alter Object" command handler.
    /// </summary>
    [CommandHandler(GuidList.guidMySqlProviderCmdSetString, GuidList.cmdidAlterTable, typeof(AlterCommand))]
    [CommandHandler(GuidList.guidMySqlProviderCmdSetString, GuidList.cmdidAlterView, typeof(AlterCommand))]
    [CommandHandler(GuidList.guidMySqlProviderCmdSetString, GuidList.cmdidAlterProcedure, typeof(AlterCommand))]
    [CommandHandler(GuidList.guidMySqlProviderCmdSetString, GuidList.cmdidAlterTrigger, typeof(AlterCommand))]
    [CommandHandler(GuidList.guidMySqlProviderCmdSetString, GuidList.cmdidAlterUDF, typeof(AlterCommand))]
    class AlterCommand: OpenEditorCommand
    {
        #region OpenEditorCommand overridings
        /// <summary>
        /// Returns identifier of the given node.
        /// </summary>
        /// <param name="hierarchy">Server explorer facade object to be used for Server Explorer hierarchy interaction.</param>
        /// <param name="item">Item identifier.</param>
        /// <returns>Returns hierarchy item ID for editor.</returns>
        protected override int GetItemForEditor(ServerExplorerFacade hierarchy, int item)
        {
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
        #endregion
    }
}
