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
 * This file contains implementation of "Create Object" command handler.
 */
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Data;
using System.Diagnostics;
using MySql.Data.VisualStudio.Utils;
using Microsoft.VisualStudio.Shell.Interop;
using System.Globalization;
using MySql.Data.VisualStudio.Properties;
using MySql.Data.VisualStudio.DocumentView;
using MySql.Data.VisualStudio.Descriptors;
using System.Data;

namespace MySql.Data.VisualStudio.Commands
{
    /// <summary>
    /// This is command handler for "Create Object" command.
    /// </summary>
    [CommandHandler(GuidList.guidMySqlProviderCmdSetString, GuidList.cmdidCreateTable, typeof(CreateCommand))]
    [CommandHandler(GuidList.guidMySqlProviderCmdSetString, GuidList.cmdidCreateView, typeof(CreateCommand))]    
    class CreateCommand: OpenEditorCommand
    {
        #region OpenEditorCommand overridings
        /// <summary>
        /// Creates new contextless node and returns its identifier.
        /// </summary>
        /// <param name="hierarchy">Server explorer facade object to be used for Server Explorer hierarchy interaction.</param>
        /// <param name="item">Item identifier.</param>
        /// <returns>Returns hierarchy item ID for editor.</returns>
        protected override int GetItemForEditor(ServerExplorerFacade hierarchy, int item)
        {
            if (hierarchy == null)
                throw new ArgumentNullException("hierarchy");
            return hierarchy.CreateObjectNode();
        }

        /// <summary>
        /// Always returns true.
        /// </summary>
        /// <param name="hierarchy">Server explorer facade object to be used for Server Explorer hierarchy interaction.</param>
        /// <param name="item">Item identifier.</param>
        /// <returns>Returns true, if new object will be created as a result f this command.</returns>
        protected override bool GetIsNewFlagForObject(ServerExplorerFacade hierarchy, int item)
        {
            return true;
        }

        /// <summary>
        /// Generates new object identifier. For objects owned directly by schema, generates 
        /// variation of "schema.{object type}¹". For nested objects returns variation of 
        /// "{parent ID}.{object type}¹".
        /// </summary>
        /// <param name="hierarchy">Server explorer facade object to be used for Server Explorer hierarchy interaction.</param>
        /// <param name="item">Item identifier.</param>
        /// <param name="typeName">Object type name.</param>
        /// <returns>Returns array with multipart identifier for the object.</returns>
        protected override object[] GetObjectID(ServerExplorerFacade hierarchy, string typeName, int item)
        {
            if (hierarchy == null)
                throw new ArgumentNullException("hierarchy");
            if (item < 0)
                throw new ArgumentOutOfRangeException("item");
            if (typeName == null)
                throw new ArgumentNullException("typeName");

            
            // Get identifier length
            int idLength = ObjectDescriptor.GetIdentifierLength(typeName);
            if (idLength <= 0)
                throw new NotSupportedException(String.Format(
                                CultureInfo.CurrentCulture,
                                Resources.Error_ObjectTypeNotSupported,
                                typeName));
            
            // Create object ID template
            object[] id;

            // Get parent object identifier
            object[] parentID = hierarchy.GetObjectIdentifier(item);

            // For embedded objects ID is ID of owner object with one additional slot.
            if (parentID != null && parentID.Length == idLength - 1)
            {
                // Initialize object identifier template
                id = new object[idLength];

                // Copy parent object identifier to the template
                parentID.CopyTo(id, 0);
            }
            // For root objects (objects owned directly by schema) ID has three slots with 
            // schema name in the middle
            else
            {
                id = CreateNewIDBase(hierarchy, typeName);
                if (id == null || id.Length != idLength)
                    throw new NotSupportedException(String.Format(
                                                    CultureInfo.CurrentCulture,
                                                    Resources.Error_ObjectTypeNotSupported,
                                                    typeName));
            }

            // Extract template for the new name
            string template = GetTemplate(typeName, id);
            if (template == null)
                template = typeName;

            // Generate object name (fill the last slot)
            CompleteNewObjectID(hierarchy, typeName, ref id, template);

            // Return result
            return id;
        }

        /// <summary>
        /// Enumerates child types for given node and tries to find supported 
        /// by document/view factory object type.
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

            // Get posible child types
            string[] childTypes = hierarchy.GetChildTypes(item);
            if (childTypes == null)
                return null;

            // Extract server version
            Debug.Assert(hierarchy.Connection != null, "Hierarchy has no connection object!");
            Version serverVersion = hierarchy.Connection != null ? hierarchy.Connection.ServerVersion : null;

            // Try to find supported objct type
            foreach (string typeName in childTypes)
            {
                if (DocumentViewFactory.Instance.IsObjectTypeRegistered(typeName))
                {
                    // Extract descriptor
                    IObjectDescriptor descriptor = ObjectDescriptorFactory.Instance.CreateDescriptor(typeName);
                    if (descriptor == null)
                    {
                        Debug.Fail("Failed to get descriptor for object type " + typeName);
                        continue;
                    }

                    // Check required MySQL version for this object.
                    Version minVersion = descriptor.RequiredVersion;
                    if (serverVersion != null && minVersion != null && serverVersion < minVersion)
                        continue;

                    // This is the one. Return it.
                    return typeName;
                }
            }

            // Type not found, command nod supported
            return null;
        } 
        #endregion

        #region New object ID generation
        /// <summary>
        /// Creates new object identifier template without name part. Base implementation supports 
        /// only 3-parts identifiers.
        /// </summary>
        /// <param name="hierarchy">Server explorer facade object to be used for Server Explorer hierarchy interaction.</param>
        /// <param name="typeName">Object type name to create identifier.</param>
        /// <returns>Returns new object identifier template without name part.</returns>
        protected virtual object[] CreateNewIDBase(ServerExplorerFacade hierarchy, string typeName)
        {
            if (hierarchy == null)
                throw new ArgumentNullException("hierarchy");
            if (typeName == null)
                throw new ArgumentNullException("typeName");

            // Only objects with identifier length 3 is supported
            if (ObjectDescriptor.GetIdentifierLength(typeName) != 3)
                throw new NotSupportedException(String.Format(
                                                CultureInfo.CurrentCulture,
                                                Resources.Error_ObjectTypeNotSupported,
                                                typeName));

            // Get default schema
            string schema = hierarchy.Connection.Schema;
            Debug.Assert(!String.IsNullOrEmpty(schema), "Failed to retrive schema name!");
            if (schema == null)
                return null;

            // Create template ID
            return new object[] { null, schema, null };
        }

        /// <summary>
        /// Returns template for the new object name.
        /// </summary>
        /// <param name="typeName">Type name of the object.</param>
        /// <param name="id">Identifier base for the object created so far.</param>
        /// <returns>Returns template for the new object name.</returns>
        protected virtual string GetTemplate(string typeName, object[] id)
        {
            return typeName;
        }
        
        /// <summary>
        /// Completes identifier for new object and changes hierarchy item name. 
        /// Last element of id array considered as the name of new object. This
        /// method sequentially generates new names in form {template}{N} and 
        /// checks for existing object with same name. To check it this method 
        /// tries to enumerate objects with restriction to whole id.
        /// </summary>
        /// <param name="hierarchy">Server explorer facade object to be used for Server Explorer hierarchy interaction.</param>
        /// <param name="typeName">Object type name.</param>
        /// <param name="id">Array with object identifier.</param>
        /// <param name="template">Template for the new object identifier.</param>
        protected virtual void CompleteNewObjectID(ServerExplorerFacade hierarchy, string typeName, ref object[] id, string template)
        {
            if (hierarchy == null)
                throw new ArgumentNullException("hierarchy");
            if (typeName == null)
                throw new ArgumentNullException("typeName");
            if (id == null)
                throw new ArgumentNullException("id");
            if (String.IsNullOrEmpty(template))
                throw new ArgumentException(Resources.Error_EmptyString, "template");

            ObjectDescriptor.CompleteNewObjectID(hierarchy, typeName, ref id, template);
        } 
        #endregion
    }
}
