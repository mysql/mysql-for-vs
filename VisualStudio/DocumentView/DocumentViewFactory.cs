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
 * This file contains the implementation of Document/View
 * objects factory
 */
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Data;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Globalization;
using MySql.Data.VisualStudio.Properties;

namespace MySql.Data.VisualStudio.DocumentView
{
    /// <summary>
    /// Document/View factory is bridge between command handlers and document/view 
    /// objects. Command handlers use factory to create instance of document or view 
    /// by given type and document/view objects use factory to register themselves 
    /// on a specific type name. This is not exact implementation of AbstractFactory 
    /// pattern, but it is very close to it. 
    /// </summary>
    class DocumentViewFactory
    {        
        #region Singleton implementation
        /// <summary>
        /// Stores unique instance of the factory
        /// </summary>
        private static DocumentViewFactory instanceRef;

        /// <summary>
        /// Returns unique instance of the factory
        /// </summary>
        public static DocumentViewFactory Instance
        {
            get
            {
                if (instanceRef == null)
                    instanceRef = new DocumentViewFactory();
                return instanceRef;
            }
        }

        /// <summary>
        /// Private constructor. Initializes internal collections.
        /// </summary>
        private DocumentViewFactory()
        {
            createDocumentDictionary = new Dictionary<string,CreateDocumentMethod>();
            createViewDictionary = new Dictionary<string, CreateViewMethod>();
        }
        #endregion

        #region Creation delegates
        /// <summary>
        /// This delegate represents method for creation of new document object.
        /// </summary>
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
        public delegate IDocument CreateDocumentMethod(ServerExplorerFacade hierarchy, bool isNew, object[] id);

        /// <summary>
        /// This delegate represents method for creation of new view object.
        /// </summary>
        /// <param name="document">
        /// Reference to the instance of the document object
        /// </param>
        /// <returns>
        /// Instance of the new view object
        /// </returns>
        public delegate IEditor CreateViewMethod(IDocument document); 
        #endregion

        #region Description collections
        /// <summary>
        /// Collection of the registered document creation methods
        /// </summary>
        Dictionary<string, CreateDocumentMethod> createDocumentDictionary;
        
        /// <summary>
        /// Collection of the registered biew creation methods
        /// </summary>
        Dictionary<string, CreateViewMethod> createViewDictionary;
        #endregion

        #region Registration methods
        /// <summary>
        /// Registers new document creation method for given object type name.
        /// </summary>
        /// <param name="typeName">Object type name</param>
        /// <param name="createMethod">Create method to register</param>
        public void RegisterDocument(string typeName, CreateDocumentMethod createMethod)
        {
            if (typeName == null)
                throw new ArgumentNullException("typeName");
            if (createMethod == null)
                throw new ArgumentNullException("createMethod");

            // Check, if document already registered
            if (createDocumentDictionary.ContainsKey(typeName))
                return;
            createDocumentDictionary.Add(typeName, createMethod);
        }

        /// <summary>
        /// Registers new view creation method for given object type name.
        /// </summary>
        /// <param name="typeName">Object type name</param>
        /// <param name="createMethod">Create method to register</param>
        public void RegisterView(string typeName, CreateViewMethod createMethod)
        {
            if (typeName == null)
                throw new ArgumentNullException("typeName");
            if (createMethod == null)
                throw new ArgumentNullException("createMethod");

            // Check, if view already registered
            if (createViewDictionary.ContainsKey(typeName))
                return;
            createViewDictionary.Add(typeName, createMethod);
        }
        #endregion

        #region Create methods
        /// <summary>
        /// Creates new document object. Extracts creation method from the collection 
        /// of registered creation methods and use it for creation.
        /// </summary>
        /// <param name="typeName">
        /// Object type name for the document
        /// </param>
        /// <param name="hierarchy">
        /// Server explorer facade object to be used for Server Explorer hierarchy interaction.
        /// Also used to extract connection.
        /// </param>
        /// <param name="isNew">
        /// Indicates if this instance represents new database object doesn’t fixed in 
        /// database yet.
        /// </param>
        /// <param name="id">
        /// Array with new object identifier.
        /// </param>
        /// <returns>
        /// Instance of the new document object
        /// </returns>
        public IDocument CreateDocument(string typeName, ServerExplorerFacade hierarchy, object[] id, bool isNew)
        {
            // Check input paramaters
            if (typeName == null)
                throw new ArgumentNullException("typeName");
            if (hierarchy == null)
                throw new ArgumentNullException("hierarchy");
            if (id == null)
                throw new ArgumentNullException("id");

            // Retrieve and invoke method
            CreateDocumentMethod createMethod = createDocumentDictionary[typeName];
            if (createMethod == null)
                throw new NotSupportedException(String.Format(
                    CultureInfo.CurrentCulture,
                    Resources.Error_DocumentNotRegistered,
                    typeName));
            return createMethod.Invoke(hierarchy, isNew, id);            
        }

        /// <summary>
        /// Creates new document object. Extracts creation method from the collection 
        /// of registered creation methods and use it for creation.
        /// </summary>
        /// <param name="document">
        /// Reference to the instance of the document object
        /// </param>
        /// <returns>
        /// Instance of the new document object
        /// </returns>
        public IEditor CreateView(IDocument document)
        {
            // Check input paramaters
            if (document == null)
                throw new ArgumentNullException("document");

            // Retrieve and invoke method
            CreateViewMethod createMethod = createViewDictionary[document.TypeName];
            if (createMethod == null)
                throw new NotSupportedException(String.Format(
                    CultureInfo.CurrentCulture,
                    Resources.Error_ViewNotRegistered,
                    document.TypeName));
            return createMethod.Invoke(document);     
        }
        #endregion

        #region Validate methods
        /// <summary>
        /// Checks, if document and view objects are registered for given object type.
        /// </summary>
        /// <param name="typeName">Object type name<</param>
        /// <returns>
        /// Returns true if document and view objects are registered for 
        /// given object type, returns false otherwise.
        /// </returns>
        public bool IsObjectTypeRegistered(string typeName)
        {
            if (String.IsNullOrEmpty(typeName))
                return false;
            return createDocumentDictionary.ContainsKey(typeName)
                && createViewDictionary.ContainsKey(typeName);
        }
        #endregion
    }
}
