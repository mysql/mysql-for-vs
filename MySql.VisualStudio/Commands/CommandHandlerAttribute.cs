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
 * This file contains attribute class used to mark command handlers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.VisualStudio.Commands;
using System.Reflection;
using MySql.Data.VisualStudio.Properties;

namespace MySql.Data.VisualStudio.Commands
{
    /// <summary>
    /// This attribute is used to mark command handlers with command 
    /// group GUID and command ID.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    sealed class CommandHandlerAttribute: Attribute
    {
        #region Initialization
        /// <summary>
        /// Constructor stores group GUID and command identifier in local variables and 
        /// registers command handler in factory.
        /// </summary>
        /// <param name="groupID">Command group GUID.</param>
        /// <param name="commandID">Command identifier.</param>
        public CommandHandlerAttribute(string groupID, int commandID, Type handlerType)
        {
            if (String.IsNullOrEmpty(groupID))
                throw new ArgumentException(Resources.Error_InvalidGroupID, "groupID");
            if (handlerType == null)
                throw new ArgumentNullException("handlerType");
            
            // Check interface implementation
            if( !typeof(ICommand).IsAssignableFrom(handlerType) )
                throw new NotSupportedException(Resources.Error_NotImplementICommand);

            // Get default construction
            this.constructorRef = handlerType.GetConstructor(new Type[] { });
            if (this.constructorRef == null)
                throw new NotSupportedException(Resources.Error_NoDefaultConstructorForCommand);

            // Store handler identity information
            this.groupIDVal = new Guid(groupID);
            this.commandIDVal = commandID;

            // Register handler in the factory
            CommandFactory.Instance.RegisterCommandHandler(
                this.groupIDVal, 
                commandID, 
                new CommandFactory.CreateCommandMethod(CreateCommandHandler));
        } 
        #endregion

        #region Properties
        /// <summary>
        /// Comand group GUID.
        /// </summary>
        public Guid GroupID
        {
            get
            {
                return groupIDVal;
            }
        }

        /// <summary>
        /// Command identifier.
        /// </summary>
        public int CommandID
        {
            get
            {
                return commandIDVal;
            }
        } 
        #endregion

        #region Create method
        /// <summary>
        /// Returns comand handler instance. Creates it at the first call.
        /// </summary>
        /// <returns>Returns comand handler instance.</returns>
        public ICommand CreateCommandHandler()
        {
            // Create handler instance if not yet created
            if (handlerInstance == null)
                handlerInstance = constructorRef.Invoke(new object[] { }) as ICommand;

            return handlerInstance;
        }
        #endregion

        #region Private variables
        /// <summary>
        /// Command group GUID.
        /// </summary>
        private Guid groupIDVal;
        /// <summary>
        /// Command identifier.
        /// </summary>
        private int commandIDVal;
        /// <summary>
        /// Constructor refernce.
        /// </summary>
        private ConstructorInfo constructorRef;
        /// <summary>
        /// Handler to already created handler instance.
        /// </summary>
        private ICommand handlerInstance;
        #endregion
    }
}
