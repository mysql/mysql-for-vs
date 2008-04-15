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
 * This class contains implementation of the base class for all command handlers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.VisualStudio.Utils;
using System.Globalization;
using MySql.Data.VisualStudio.Properties;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Data;
using Microsoft.VisualStudio;

namespace MySql.Data.VisualStudio.Commands
{
    /// <summary>
    /// This is the base class for all command handlers. It implements 
    /// ICommand interface and provides several template methods. In 
    /// addition, it implements several useful utility methods.
    /// </summary>
    abstract class BaseCommand: ICommand
    {
        #region Initialization
        /// <summary>
        /// Main goal of the constructor is to extract command text template 
        /// for the resources.
        /// </summary>
        protected BaseCommand()
        {
            // Univeral prefix for all commands
            string resourceName = "Command";

            // Determine group specific prefix part
            if (GroupID == GuidList.guidMySqlProviderCmdSet)
            {
                resourceName += '_';
            }
            else if (GroupID == GuidList.guidDataCmdSet)
            {
                resourceName += "BuiltIn_";
            }
            else
            {
                resourceName += GroupID.ToString();
            }                    

            // Append command ID
            resourceName += CommandID.ToString("X", CultureInfo.InvariantCulture);

            // Extract resource string
            commandTextVal = Resources.ResourceManager.GetString(resourceName);
        }
        #endregion

        #region Virtual methods and properties
        /// <summary>
        /// Indicates, if command can be executed for empty set of items. 
        /// Returns false by default.
        /// </summary>
        protected virtual bool IsEmptyAllowed
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Indicates, if command can be executed for set with several 
        /// (more then one) items. Returns false by default.
        /// </summary>
        protected virtual bool IsMultiselectAllowed
        {
            get
            {
                return false;
            }
        }
        /// <summary>
        /// Returns localized command text template initialized in constructor.
        /// </summary>
        protected virtual string CommandText
        {
            get
            {
                return commandTextVal;
            }
        }

        /// <summary>
        /// Virtual method for formatting command text template. By default retrieves 
        /// object type for first node and uses String.Format to place it in the 
        /// correct place.
        /// </summary>
        /// <param name="template">Command text template.</param>
        /// <param name="hierarchy">Server explorer facade object to be used for Server Explorer hierarchy interaction..</param>
        /// <param name="items">Array with item’s identifiers.</param>
        /// <returns>Localized command text.</returns>
        protected virtual string FormatText(string template, ServerExplorerFacade hierarchy, int[] items)
        {
            if (template == null)
                throw new ArgumentNullException("template");
            if (hierarchy == null)
                throw new ArgumentNullException("hierarchy");
            if (items == null)
                throw new ArgumentNullException("items");

            // Only single item format supported by default
            if (items.Length != 1)
                return template;

            // Get localized object type name for the item
            string typeName = GetLocalizedItemType(hierarchy, items[0]);

            // Format comand text template with localized type name
            if (!String.IsNullOrEmpty(typeName))
                return String.Format(CultureInfo.CurrentCulture, template, typeName);

            return template;
        }

        /// <summary>
        /// Checks command accessibility for single item in hierarchy.
        /// </summary>
        /// <param name="hierarchy">Server explorer facade object to be used for Server Explorer hierarchy interaction.</param>
        /// <param name="item">Item identifier.</param>
        /// <returns>Returns true if command is valid for given item, otherwise returns false.</returns>
        protected abstract bool CheckSingleItem(ServerExplorerFacade hierarchy, int item);

        /// <summary>
        /// Executes command for single item.
        /// </summary>
        /// <param name="hierarchy">Server explorer facade object to be used for Server Explorer hierarchy interaction.</param>
        /// <param name="item">Item identifier.</param>
        /// <returns>Result of execution. Typicaly null.</returns>
        protected abstract object ExecuteSingleItem(ServerExplorerFacade hierarchy, int item);

        /// <summary>
        /// Returns object type name, which should be used to create document 
        /// and view objects instances.
        /// </summary>
        /// <param name="hierarchy">Server explorer facade object to be used for Server Explorer hierarchy interaction.</param>
        /// <param name="item">Item identifier.</param>
        /// <returns>
        /// Returns object type name, which should be used to create document 
        /// and view objects instances.
        /// </returns>
        protected abstract string GetObjectType(ServerExplorerFacade hierarchy, int item);       
        #endregion
        
        #region ICommandHandler Members
        /// <summary>
        /// Returns command group ID using reflection.
        /// </summary>
        public Guid GroupID
        {
            get 
            {
                CommandHandlerAttribute attribute = ReflectionHelper.GetCommandHandlerAttribute(this.GetType());
                if (attribute == null)
                    throw new NotSupportedException(Resources.Error_NotMarkedAsCommand);
                return attribute.GroupID;
            }
        }

        /// <summary>
        /// Returns command ID using reflection.
        /// </summary>
        public int CommandID
        {
            get
            {
                CommandHandlerAttribute attribute = ReflectionHelper.GetCommandHandlerAttribute(this.GetType());
                if (attribute == null)
                    throw new NotSupportedException(Resources.Error_NotMarkedAsCommand);
                return attribute.CommandID;
            }
        }

        /// <summary>
        /// Executes command for given set of items. Calls to virtual 
        /// execution method to process each item.
        /// </summary>
        /// <param name="hierarchy">Server explorer facade object to be used for Server Explorer hierarchy interaction..</param>
        /// <param name="items">Array with item’s identifiers.</param>
        /// <returns>Always returns null.</returns>
        public virtual object[] Execute(ServerExplorerFacade hierarchy, int[] items)
        {
            if (hierarchy == null)
                throw new ArgumentNullException("hierarchy");
            if (items == null)
                throw new ArgumentNullException("items");
            
            // Creating array to store results
            object[] result = new object[items.Length];

            for (int i = 0; i <items.Length; i++)
                result[i] = ExecuteSingleItem(hierarchy, items[i]);

            // Always returns nothing.
            return result;
        }

        /// <summary>
        /// Checks accessibility of command for given items. First checks item array 
        /// length – most command supports only single selection. After length is checked, 
        /// process each item and if for any item command is inaccessible, returns false.
        /// </summary>
        /// <param name="hierarchy">Server explorer facade object to be used for Server Explorer hierarchy interaction.</param>
        /// <param name="items">Array with item’s identifiers.</param>
        /// <returns>Returns true if command should be shown and false otherwise</returns>
        public virtual bool GetIsVisible(ServerExplorerFacade hierarchy, int[] items)
        {
            if (hierarchy == null)
                throw new ArgumentNullException("hierarchy");
            if (items == null)
                throw new ArgumentNullException("items");
            
            // Check length restrictions
            if (items.Length <= 0 && !IsEmptyAllowed)
                return false;
            if (items.Length > 1 && !IsMultiselectAllowed)
                return false;
            
            // Check each item using virtual method
            foreach (int i in items)
                if (!CheckSingleItem(hierarchy, i))
                    return false;

            return true;
        }

        /// <summary>
        /// Returns customized text for the command. Firstly retrieves command text 
        /// template fro the resources using virtual property, then calls virtual 
        /// method to format template for given items.
        /// </summary>
        /// <param name="hierarchy">Server explorer facade object to be used for Server Explorer hierarchy interaction..</param>
        /// <param name="items">Array with item’s identifiers.</param>
        /// <returns>Returns customized text for the command.</returns>
        public string GetText(ServerExplorerFacade hierarchy, int[] items)
        {
            if (hierarchy == null)
                throw new ArgumentNullException("hierarchy");
            if (items == null)
                throw new ArgumentNullException("items");

            // Extract command text format template
            string template = CommandText;

            // Format command text using template and items information
            if (!String.IsNullOrEmpty(template))
                return FormatText(template, hierarchy, items);

            return String.Empty;
        }
        #endregion

        #region Utilities
        /// <summary>
        /// Returns localized object type name for the given node id.
        /// </summary>
        /// <param name="hierarchy">Server explorer facade object to be used for Server Explorer hierarchy interaction..</param>
        /// <param name="item">Item identifier.</param>
        /// <returns>Returns object type name for the given node id.</returns>
        protected string GetLocalizedItemType(ServerExplorerFacade hierarchy, int item)
        {
            if (hierarchy == null)
                throw new ArgumentNullException("hierarchy");
            if (item < 0)
                throw new ArgumentOutOfRangeException("item");

            // Get object type for item
            string typeName = GetObjectType(hierarchy, item);

            return LocalizeTypeName(typeName);
        }

        /// <summary>
        /// Looks project resources for the localized name of the object type.
        /// </summary>
        /// <param name="typeName">Object type name to localize.</param>
        /// <returns>If localized string is founded, returns it. Otherwise returns typeName itself.</returns>
        protected static string LocalizeTypeName(string typeName)
        {
            if (!String.IsNullOrEmpty(typeName))
            {
                // Get localized name, if available
                string localizedTypeName = Resources.ResourceManager.GetString("Type_" + typeName);
                if (!String.IsNullOrEmpty(localizedTypeName))
                    return localizedTypeName;
            }

            return typeName;
        }
        #endregion

        #region Private variables to store properties
        private string commandTextVal;
        #endregion
    }
}
