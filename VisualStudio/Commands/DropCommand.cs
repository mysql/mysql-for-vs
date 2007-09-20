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
 * This file contins implementation of the handler for the "Drop Object" command
 */
using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.VisualStudio.Descriptors;
using System.Diagnostics;
using MySql.Data.VisualStudio.Dialogs;
using MySql.Data.VisualStudio.Utils;
using MySql.Data.VisualStudio.Properties;
using System.Windows.Forms;
using System.Globalization;
using System.Data.Common;

namespace MySql.Data.VisualStudio.Commands
{
    /// <summary>
    /// This is a command handler for "Drop Object" command implementation
    /// </summary>
    [CommandHandler(GuidList.guidMySqlProviderCmdSetString, GuidList.cmdidDropTable, typeof(DropCommand))]
    [CommandHandler(GuidList.guidMySqlProviderCmdSetString, GuidList.cmdidDropView, typeof(DropCommand))]
    [CommandHandler(GuidList.guidMySqlProviderCmdSetString, GuidList.cmdidDropProcedure, typeof(DropCommand))]
    [CommandHandler(GuidList.guidMySqlProviderCmdSetString, GuidList.cmdidDropTrigger, typeof(DropCommand))]
    [CommandHandler(GuidList.guidMySqlProviderCmdSetString, GuidList.cmdidDropUDF, typeof(DropCommand))]
    class DropCommand: BaseCommand
    {
        #region Overridings
        /// <summary>
        /// Returns true if given hierarchy item is object node and object descriptor 
        /// supports dropping.
        /// </summary>
        /// <param name="hierarchy">Server explorer facade object to be used for Server Explorer hierarchy interaction.</param>
        /// <param name="item">Item identifier.</param>
        /// <returns>
        /// Returns true if given hierarchy item is object node and object descriptor 
        /// supports dropping.
        /// </returns>
        protected override bool CheckSingleItem(ServerExplorerFacade hierarchy, int item)
        {
            if (hierarchy == null)
                throw new ArgumentNullException("hierarchy");
            if (item < 0)
                throw new ArgumentOutOfRangeException("item");

            // Extract type name
            string typeName = GetObjectType(hierarchy, item);
            if (String.IsNullOrEmpty(typeName))
                return false;

            // Extract descriptor
            IObjectDescriptor descriptor = ObjectDescriptorFactory.Instance.CreateDescriptor(typeName);

            // Return true if descriptor could build drop query.
            return descriptor != null ? descriptor.CanBeDropped : false;
        }

        /// <summary>
        /// Drops single item. Uses object descriptor to build DROP statement.
        /// </summary>
        /// <param name="hierarchy">Server explorer facade object to be used for Server Explorer hierarchy interaction.</param>
        /// <param name="item">Item identifier.</param>
        /// <returns>Result of execution. Typicaly null.</returns>
        protected override object ExecuteSingleItem(ServerExplorerFacade hierarchy, int item)
        {
            if (hierarchy == null)
                throw new ArgumentNullException("hierarchy");
            if (item < 0)
                throw new ArgumentOutOfRangeException("item");

            // Extract type name
            string typeName = GetObjectType(hierarchy, item);
            if (String.IsNullOrEmpty(typeName))
            {
                Debug.Fail("Failed to get object type!");
                return false;
            }

            // Extract descriptor
            IObjectDescriptor descriptor = ObjectDescriptorFactory.Instance.CreateDescriptor(typeName);
            if (descriptor == null || !descriptor.CanBeDropped)
            {
                Debug.Fail("Failed to get descriptor or descriptor doesn't support dropping!");
                return null;
            }

            // Extract object identifier
            object[] identifier = hierarchy.GetObjectIdentifier(item);
            if (identifier == null)
            {
                Debug.Fail("Failed to get object identifier!");
                return null;
            }

            // TODO: Note the seconde check for TableData document. It should be replaced in the future
            // by the generic mechanism.
            
            // Check if editor is registered for the database object
            if (hierarchy.HasDocument(typeName, identifier) || hierarchy.HasDocument(TableDataDescriptor.TypeName, identifier))
            {
                UIHelper.ShowError(String.Format(
                    CultureInfo.InvariantCulture,
                    Resources.Error_CantDropBecauseOfEditor,
                    hierarchy.GetName(item)));
                return null;
            }

            // Build DROP query
			string dropQuery = String.Empty;
			if (descriptor is StoredProcDescriptor)
				dropQuery = (descriptor as StoredProcDescriptor).BuildDropSql(
					hierarchy, item, identifier);
			else
				dropQuery = descriptor.BuildDropSql(identifier);

			if (String.IsNullOrEmpty(dropQuery))
            {
                Debug.Fail("Failed to build DROP query!");
                return null;
            }

            // Extract connection
            DataConnectionWrapper connection = hierarchy.Connection;
            if (connection == null)
            {
                Debug.Fail("Failed to extract connection!");
                return null;
            }

            try
            {
                // Execute drop query
                connection.ExecuteScalar(dropQuery);

                // Drop hierarchy node
                hierarchy.DropObjectNode(item);                

                // Notify everybody about object dropping
                connection.ObjectChangeEvents.RaiseObjectRemoved(typeName, identifier);
            }
            catch (DbException e)
            {
                Trace.TraceError("Error during saving object:\n{0}", e.ToString());
                SqlErrorDialog.ShowError(e, connection.GetFullStatus());
            }
            catch (Exception e)
            {
                Trace.TraceError("Error during saving object:\n{0}", e.ToString());
                UIHelper.ShowError(e);
            }

            return null;
        }

        /// <summary>
        /// Returns object type name for the given hierarchy item.
        /// </summary>
        /// <param name="hierarchy">Server explorer facade object to be used for Server Explorer hierarchy interaction.</param>
        /// <param name="item">Item identifier.</param>
        /// <returns>
        /// Returns object type name for the given hierarchy item.
        /// </returns>
        protected override string GetObjectType(ServerExplorerFacade hierarchy, int item)
        {
            if (hierarchy == null)
                throw new ArgumentNullException("hierarchy");
            if (item < 0)
                throw new ArgumentOutOfRangeException("item");

            return hierarchy.GetObjectType(item);
        }

        /// <summary>
        /// This method is overriden to display warning message before deletion. Then just calls base.
        /// </summary>
        /// <param name="hierarchy">Server explorer facade object to be used for Server Explorer hierarchy interaction..</param>
        /// <param name="items">Array with item’s identifiers.</param>
        /// <returns>Always returns null.</returns>
        public override object[] Execute(ServerExplorerFacade hierarchy, int[] items)
        {
            if (hierarchy == null)
                throw new ArgumentNullException("hierarchy");
            if (items == null)
                throw new ArgumentNullException("items");

            // Build object names list
            StringBuilder list = new StringBuilder();
            foreach (int item in items)
            {
                // Extract name for item
                string name = hierarchy.GetName(item);
                if (String.IsNullOrEmpty(name))
                    continue;

                // If builder is not empty, add ', '
                if (list.Length > 0)
                    list.Append(Resources.Comma);

                // Append object name
                list.Append(name);
            }

            // Build warning message
            string message = String.Format(CultureInfo.CurrentCulture, Resources.Warning_ConfirmDelete, list.ToString());

            // First, ask user for confirmation
            if (UIHelper.ShowWarning(message, MessageBoxButtons.YesNo)
                != DialogResult.Yes)
                return null;

            // Only if user confirmed delete, call to base class to execute command
            return base.Execute(hierarchy, items);
        } 
        #endregion
    }
}
