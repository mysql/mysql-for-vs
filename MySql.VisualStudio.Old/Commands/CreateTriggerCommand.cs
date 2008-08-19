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
 * This file contains implementation of the Create Trigger command handler.
 */
using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.VisualStudio.Descriptors;
using MySql.Data.VisualStudio.Properties;
using System.Globalization;

namespace MySql.Data.VisualStudio.Commands
{
    /// <summary>
    /// Create command for triggers has specific new name generation (names
    /// must be unique for a whole database). This class implements proper
    /// name generation.
    /// </summary>
    [CommandHandler(GuidList.guidMySqlProviderCmdSetString, GuidList.cmdidCreateTrigger, typeof(CreateTriggerCommand))]
    class CreateTriggerCommand: CreateCommand
    {
        /// <summary>
        /// Template for trigger includes owner table name.
        /// </summary>
        /// <param name="typeName">Type name of the object.</param>
        /// <param name="id">Identifier base for the object created so far.</param>
        /// <returns>Returns template for the new object name.</returns>
        protected override string GetTemplate(string typeName, object[] id)
        {
            if (typeName == null)
                throw new ArgumentNullException("typeName");
            if(id == null || id.Length < 3 || id[2] == null)
                throw new ArgumentException(
                            String.Format(
                                CultureInfo.CurrentCulture,
                                Resources.Error_InvlaidIdentifier,
                                id.Length,
                                typeName,
                                ObjectDescriptor.GetIdentifierLength(typeName)),
                             "id");

            return id[2].ToString() + '_' + typeName;
        }

        /// <summary>
        /// This function is overriden to control trigger names uniques for whole database and
        /// not for each table.
        /// </summary>
        /// <param name="hierarchy">Server explorer facade object to be used for Server Explorer hierarchy interaction.</param>
        /// <param name="typeName">Object type name.</param>
        /// <param name="id">Array with object identifier.</param>
        /// <param name="template">Template for the new object identifier.</param>
        protected override void CompleteNewObjectID(ServerExplorerFacade hierarchy, string typeName, ref object[] id, string template)
        {
            if (hierarchy == null)
                throw new ArgumentNullException("hierarchy");
            if (typeName == null)
                throw new ArgumentNullException("typeName");
            if (id == null)
                throw new ArgumentNullException("id");
            if(id.Length < 3 || id[2] == null)
                throw new ArgumentException(
                            String.Format(
                                CultureInfo.CurrentCulture,
                                Resources.Error_InvlaidIdentifier,
                                id.Length,
                                typeName,
                                ObjectDescriptor.GetIdentifierLength(typeName)),
                             "id");
            if (String.IsNullOrEmpty(template))
                throw new ArgumentException(Resources.Error_EmptyString, "template");

            // Initialize complete parameters
            string actualTameplate = template;
            int counter = 0;
            
            do
            {
                // Stores table id and reset it to null in template, because trigger names must be
                // unique for a whole database.
                object tableID = id[2];
                id[2] = null;

                // Use base method for completion
                base.CompleteNewObjectID(hierarchy, typeName, ref id, actualTameplate);

                // Restores table id
                id[2] = tableID;

                // Change template and add new counter
                actualTameplate = template + (++counter).ToString();
            }
            // We need to control open editors here separately, because base method will miss them 
            // (it doesn't know table ID)
            while (hierarchy.HasDocument(typeName, id));
        }
    }
}
