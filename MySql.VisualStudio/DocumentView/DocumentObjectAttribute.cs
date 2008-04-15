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
 * This file contains implementation of the custom attribute, used to mark
 * classes as documents.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Microsoft.VisualStudio.Data;
using MySql.Data.VisualStudio.Properties;

namespace MySql.Data.VisualStudio.DocumentView
{
    /// <summary>
    /// Attribute, used to mark class as an document class with corresponded 
    /// type name in terms of DataObjectSupport XML file. Document type must 
    /// be implement IDocument and must have constructor with 
    /// (HierarchyAccessorMediator, bool, object[]) signature.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    sealed class DocumentObjectAttribute: Attribute
    {
        #region Initialization
        /// <summary>
        /// Attribute constructor
        /// </summary>
        /// <param name="typeName">
        /// Name of the document type. Corresponds to the type name in the 
        /// DataObjectSupport XML file.
        /// </param>
        /// <param name="documentType">
        /// Reflection Type object with current type description
        /// </param>
        public DocumentObjectAttribute(string typeName, Type documentType)
        {
            if (typeName == null)
                throw new ArgumentNullException("typeName");
            if (documentType == null)
                throw new ArgumentNullException("documentType");

            this.documentTypeNameVal = typeName;

            // Must be assignable to IDocument interface
            if (!typeof(IDocument).IsAssignableFrom(documentType))
                throw new NotSupportedException(Resources.Error_NotImplementIDocument);
            
            // Retrieve proper constructor
            construtorInfo = documentType.GetConstructor(new Type[] { typeof(ServerExplorerFacade), typeof(bool), typeof(object[]) });
            if (construtorInfo == null)
                throw new NotSupportedException(Resources.Error_NoConstructorForDocument);

            // Register document type in the factory
            DocumentViewFactory.Instance.RegisterDocument(typeName, new DocumentViewFactory.CreateDocumentMethod(CreateDocument));
        } 
        #endregion

        #region Public properties and methods
        /// <summary>
        /// Method, used to create document object instance.
        /// </summary>
        /// <param name="hierarchy">
        /// Server explorer facade object to be used for Server Explorer hierarchy interaction.
        /// Also used to extract connection.
        /// </param>
        /// <param name="id">
        /// Array with the object identifier.
        /// </param>
        /// <param name="isNew">
        /// Indicates if this instance represents new database object doesn’t fixed in 
        /// database yet.
        /// </param>
        /// <returns>New instance of the document object</returns>
        public IDocument CreateDocument(ServerExplorerFacade hierarchy, bool isNew, object[] id)
        {
            return (IDocument)construtorInfo.Invoke(new object[] { hierarchy, isNew, id });
        }

        /// <summary>
        /// Name of the document type. Corresponds to the type name in the 
        /// DataObjectSupport XML file.
        /// </summary>
        public string DocumentTypeName
        {
            get
            {
                return documentTypeNameVal;
            }
        } 
        #endregion

        #region Private variables
        private ConstructorInfo construtorInfo;
        private string documentTypeNameVal;
        #endregion
    }
}
