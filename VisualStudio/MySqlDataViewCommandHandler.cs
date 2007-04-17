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
 * This file contains implementation of data view commands handler.
 */

using System;
using System.Windows.Forms.Design;
using Microsoft.VisualStudio.Data;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Data;
using MySql.Data.VisualStudio.Commands;
using Microsoft.VisualStudio.OLE.Interop;

namespace MySql.Data.VisualStudio
{
	/// <summary>
	/// Represents a custom data view command handler. 
	/// </summary>
	public class MySqlDataViewCommandHandler : DataViewCommandHandler
	{        
        #region Overridings
        /// <summary>
        /// This method supplies information about a command's status. At this point all 
        /// commands are supported and visible.
        /// </summary>
        /// <param name="itemIds">
        /// Array of identifiers for the items in the data view hierarchy on which this 
        /// command should be invoked.
        /// </param>
        /// <param name="command">
        /// The OleCommand object representing the command to invoke.
        /// </param>
        /// <param name="textType">
        /// The OleCommandTextType object instance for the specified command.
        /// </param>
        /// <param name="status">
        /// The OleCommandStatus object instance for the specified command.
        /// </param>
        /// <returns>
        /// Returns an OleCommandStatus object instance representing the status returned by the specified commands. /// 
        /// </returns>        
        public override OleCommandStatus GetCommandStatus(
            int[] itemIds,
            OleCommand command,
            OleCommandTextType textType,
            OleCommandStatus status)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            if (status == null)
                throw new ArgumentNullException("status");

            OleCommandStatus result = new OleCommandStatus();

            // Get command handler instance
            ICommand commandHandler = CommandFactory.Instance.CreateCommandHandler(command.GroupGuid, command.CommandId);
            if (commandHandler == null)
                return base.GetCommandStatus(itemIds, command, textType, status);
            result.Supported = true;

            // Determine visibility
            if (!commandHandler.GetIsVisible(Hierarchy, itemIds))
            {
                result.Visible = false;
                return result;
            }
            result.Visible = true;
            result.Enabled = true;

            // TODO: Find out why this doesn't work at all!
            // Localize text if possible
            string localizedText = commandHandler.GetText(Hierarchy, itemIds);
            if (!String.IsNullOrEmpty(localizedText))
                result.Text = localizedText;
            
            return result;
        }

        /// <summary>
        /// Executes a specified command, potentially based on parameters passed in 
        /// from the data view support XML.
        /// </summary>
        /// <param name="itemIds">
        /// Array of identifiers for the items in the data view hierarchy on which this 
        /// command should be invoked.
        /// </param>
        /// <param name="command">
        /// The OleCommand object representing the command to invoke.
        /// </param>
        /// <param name="executionOption">
        /// Any OleCommandExecutionOption object instance representing options on the 
        /// invoked command.
        /// NOT USED.
        /// </param>
        /// <param name="arguments">
        /// An object representing arguments to the command.
        /// NOT USED.
        /// </param>
        /// <returns>
        /// Returns an object instance representing the value returned by the specified 
        /// command, which is typically nothing. 
        /// </returns>
        public override object[] ExecuteCommand(
            int[] itemIds,
            OleCommand command,
            OleCommandExecutionOption executionOption,
            object arguments)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            // Get command handler instance
            ICommand commandHandler = CommandFactory.Instance.CreateCommandHandler(command.GroupGuid, command.CommandId);
            if (commandHandler == null)
                return base.ExecuteCommand(itemIds, command, executionOption, arguments);

            // Executes command
            return commandHandler.Execute(Hierarchy, itemIds);
        }

        /// <summary>
        /// Executes a specified command, potentially based on parameters passed in 
        /// from the data view support XML.
        /// </summary>
        /// <param name="itemId">
        /// Identifier of hierarchy item for which command should be executed.
        /// </param>
        /// <param name="command">
        /// The OleCommand object representing the command to invoke.
        /// </param>
        /// <param name="executionOption">
        /// Any OleCommandExecutionOption object instance representing options on the 
        /// invoked command.
        /// NOT USED.
        /// </param>
        /// <param name="arguments">
        /// An object representing arguments to the command.
        /// NOT USED.
        /// </param>
        /// <returns>
        /// Returns an object instance representing the value returned by the specified 
        /// command, which is typically nothing. 
        /// </returns>
        public override object ExecuteCommand(int itemId, OleCommand command, OleCommandExecutionOption executionOption, object arguments)
        {
            return ExecuteCommand(new int[] { itemId }, command, executionOption, arguments);
        }
        #endregion

        #region Hierarchy accessor mediator
        /// <summary>
        /// Returns hierarchy accessor mediator instance. Create instance at the first call.
        /// </summary>
        protected ServerExplorerFacade Hierarchy
        {
            get
            {
                // Check if not created yet
                if (hierarchyRef == null)
                {
                    Debug.Assert(DataViewHierarchyAccessor != null, "Hierarchy accessor is not initialized!");
                    hierarchyRef = new ServerExplorerFacade(DataViewHierarchyAccessor);
                }
                // Here must be already created
                Debug.Assert(hierarchyRef != null, "Failed to initialize hierarchy accessor mediator!");
                return hierarchyRef;
            }
        }

        /// <summary>
        /// Used to store hierarchy accessor mediator instance
        /// </summary>
        private ServerExplorerFacade hierarchyRef; 
        #endregion
    }
}
