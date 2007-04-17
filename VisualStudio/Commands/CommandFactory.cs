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
 * This file contains implementation of comand handlers factory.
 */
using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.VisualStudio.Commands;

namespace MySql.Data.VisualStudio.Commands
{
    /// <summary>
    /// This class is used to create command handlers by given group GUID 
    /// and command identifier.
    /// </summary>
    public class CommandFactory
    {
        #region Singleton implementation
        /// <summary>
        /// Stores unique instance of the factory
        /// </summary>
        private static CommandFactory instanceRef;

        /// <summary>
        /// Returns unique instance of the factory
        /// </summary>
        public static CommandFactory Instance
        {
            get
            {
                if (instanceRef == null)
                    instanceRef = new CommandFactory();
                return instanceRef;
            }
        }

        /// <summary>
        /// Private constructor. Initializes internal collection.
        /// </summary>
        private CommandFactory()
        {
            commandHandlersDictionary = new Dictionary<CommandIDKey, CreateCommandMethod>();            
        }
        #endregion

        #region Command handlers create delegates collections
        /// <summary>
        /// Structure, use to identify command handlers in the dictionary by 
        /// group GUID an command ID.
        /// </summary>
        private struct CommandIDKey
        {
            public readonly Guid commandGroupID;
            public readonly int commandID;

            public CommandIDKey(Guid commandGroupID, int commandID)
            {
                this.commandGroupID = commandGroupID;
                this.commandID = commandID;
            }
        }

        /// <summary>
        /// Collection of the registered command handlers
        /// </summary>
        Dictionary<CommandIDKey, CreateCommandMethod> commandHandlersDictionary;

        /// <summary>
        /// Delegate, used for command handler creation.
        /// </summary>
        /// <returns>Instance of the command handler.</returns>
        public delegate ICommand CreateCommandMethod();
        #endregion

        #region Registration methods
        /// <summary>
        /// Registers new command handler in the factory.
        /// </summary>
        /// <param name="groupID">Command group GUID.</param>
        /// <param name="commandID">Command identifier.</param>
        /// <param name="createMethod">Create command handler method delegate.</param>
        public void RegisterCommandHandler(Guid groupID, int commandID, CreateCommandMethod createMethod)
        {
            if (createMethod == null)
                throw new ArgumentNullException("createMethod");

            // Create key structure
            CommandIDKey key = new CommandIDKey(groupID, commandID);

            // Check, if handler already registered
            if (commandHandlersDictionary.ContainsKey(key))
                return;
            commandHandlersDictionary.Add(key, createMethod);
        }
        #endregion

        #region Create methods
        /// <summary>
        /// Returns instance of the command handler.
        /// </summary>
        /// <param name="groupID">Command group GUID.</param>
        /// <param name="commandID">Command identifier.</param>
        /// <returns>Returns instance of the command handler.</returns>
        public ICommand CreateCommandHandler(Guid groupID, int commandID)
        {
            // Creating key structure
            CommandIDKey key = new CommandIDKey(groupID, commandID);

            // Check handler registration
            if( !commandHandlersDictionary.ContainsKey(key))
                return null;

            // Get create method and create instance
            CreateCommandMethod createMethod = commandHandlersDictionary[key];
            if (createMethod == null)
                return null;
            return createMethod.Invoke();   
        }
        #endregion

        #region Validate methods
        /// <summary>
        /// Checks, if command handler is in the handlers collection.
        /// </summary>
        /// <param name="groupID">Command group GUID.</param>
        /// <param name="commandID">Command identifier.</param>
        /// <returns>Rturns true if command handler is in the handlers collection.</returns>
        public bool IsCommandHandlerRegistered(Guid groupID, int commandID)
        {
            return commandHandlersDictionary.ContainsKey(new CommandIDKey(groupID, commandID));
        }
        #endregion
    }
}
