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
 * This file contains base class for commands, which needs to open editors.
 */
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Data;
using System.Diagnostics;
using MySql.Data.VisualStudio.Utils;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio;
using System.Runtime.InteropServices;
using MySql.Data.VisualStudio.Properties;
using MySql.Data.VisualStudio.DocumentView;

namespace MySql.Data.VisualStudio.Commands
{
    /// <summary>
    /// This is the base class for all commands which open editor as a result. 
    /// It provides template method execution method and several utility methods.
    /// </summary>
    abstract class OpenEditorCommand: BaseCommand
    {
        #region BaseCommand overridings
        /// <summary>
        /// Command considered as valid if object type name could be determined.
        /// </summary>
        /// <param name="hierarchy">Server explorer facade object to be used for Server Explorer hierarchy interaction.</param>
        /// <param name="item">Item identifier.</param>
        /// <returns>Returns true if command is valid for given item, otherwise returns false.</returns>
        protected override bool CheckSingleItem(ServerExplorerFacade hierarchy, int item)
        {
            if (hierarchy == null)
                throw new ArgumentNullException("hierarchy");
            if (item < 0)
                throw new ArgumentOutOfRangeException("item");

            // Get object type name
            string typeName = GetObjectType(hierarchy, item);
            if (String.IsNullOrEmpty(typeName))
                return false;
            
            // Check object type registration
            return DocumentViewFactory.Instance.IsObjectTypeRegistered(typeName);
        }
        /// <summary>
        /// Template method initializes editor parameters, including object type name, 
        /// object identifier, new flag and hierarchy item identifier for editor, using 
        /// virtual methods, then looks for existing editor. If existing editor is not 
        /// found, creates new document and view objects and calls OpenEditor method.
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

            // Get object type name for editor to be open
            string typeName = GetObjectType(hierarchy, item);
            if (typeName == null)
                throw new NotSupportedException(Resources.Error_UnableToGetObjectType);

            // Get object identifier
            object[] objectID = GetObjectID(hierarchy, typeName, item);
            if (objectID == null || objectID.Length <= 0)
                throw new NotSupportedException(Resources.Error_UnableToGetObjectID);

            // Get is new flag value
            bool isNew = GetIsNewFlagForObject(hierarchy, item);

            // Get hierarchy item id for editor to be open (could differs from "item")
            int itemForEditor = GetItemForEditor(hierarchy, item);
            if (itemForEditor < 0)
                throw new NotSupportedException(Resources.Error_UnableToGetHierarchyItem);

            // Set node name for new item
            if (isNew)
            {
                hierarchy.SetName(itemForEditor, objectID[objectID.Length - 1] as string);
            }

            // Try to find if not new
            if (!isNew)
                if (hierarchy.FindAndActivateEditor(itemForEditor, typeName, objectID))
                    return null;

            // Build document and view object using factory
            IDocument document = CreateDocument(hierarchy, typeName, objectID, isNew);
            if (document == null)
                throw new NotSupportedException(Resources.Error_UnableToGetDocumentObject);
            IEditor view = CreateEditor(document);
            if (view == null)
                throw new NotSupportedException(Resources.Error_UnableToGetViewObject);

            // Open new editor instance
            hierarchy.RegisterEditor(itemForEditor, document, view);


            // Returns no result
            return null;
        }
        #endregion

        #region View and Document creation
        /// <summary>
        /// Creates new instance of editor. Base implementation simply calls factory.
        /// </summary>
        /// <param name="document">
        /// Reference to the instance of the document object
        /// </param>
        /// <returns>
        /// Instance of the new document object
        /// </returns>
        protected virtual IEditor CreateEditor(IDocument document)
        {
            if (document == null)
                throw new ArgumentNullException("document");
            return DocumentViewFactory.Instance.CreateView(document);
        }

        /// <summary>
        /// Creates new document object. Base implementation simply calls factory.
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
        /// Instance of the new document object
        /// </returns>
        protected virtual IDocument CreateDocument(ServerExplorerFacade hierarchy, string typeName, object[] objectID, bool isNew)
        {
            if (hierarchy == null)
                throw new ArgumentNullException("hierarchy");
            if (typeName == null)
                throw new ArgumentNullException("typeName");
            if (objectID == null)
                throw new ArgumentNullException("objectID");
            return DocumentViewFactory.Instance.CreateDocument(typeName, hierarchy, objectID, isNew);
        }
        #endregion

        #region Abstract methods
        /// <summary>
        /// Returns hierarchy item ID for editor. It can be the same item, 
        /// if the command it is changing command and it can item of new 
        /// contextless node if it is creating command.
        /// </summary>
        /// <param name="hierarchy">Server explorer facade object to be used for Server Explorer hierarchy interaction.</param>
        /// <param name="item">Item identifier.</param>
        /// <returns>Returns hierarchy item ID for editor.</returns>
        protected abstract int GetItemForEditor(ServerExplorerFacade hierarchy, int item);

        /// <summary>
        /// Returns true, if new object will be created as a result of this command.
        /// </summary>
        /// <param name="hierarchy">Server explorer facade object to be used for Server Explorer hierarchy interaction.</param>
        /// <param name="item">Item identifier.</param>
        /// <returns>Returns true, if new object will be created as a result f this command.</returns>
        protected abstract bool GetIsNewFlagForObject(ServerExplorerFacade hierarchy, int item);

        /// <summary>
        /// Returns array with multipart identifier for the object.
        /// </summary>
        /// <param name="hierarchy">Server explorer facade object to be used for Server Explorer hierarchy interaction.</param>
        /// <param name="item">Item identifier.</param>
        /// <returns>Returns array with multipart identifier for the object.</returns>
        protected abstract object[] GetObjectID(ServerExplorerFacade hierarchy, string typeName, int item);
        #endregion
    }
}
