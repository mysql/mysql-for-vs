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
 * This file contains implementation of custom attribute, used to mark view 
 * classes.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using MySql.Data.VisualStudio.Properties;

namespace MySql.Data.VisualStudio.DocumentView
{
    /// <summary>
    /// This attribute is used to mark view classes. Each view must implement 
    /// IEditor interface and support public constructor with single parameter 
    /// of IDocument type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    sealed class ViewObjectAttribute : Attribute
    {
        #region Initialization
        /// <summary>
        /// Attribute constructor
        /// </summary>
        /// <param name="typeName">
        /// Name of the underlying document type. Corresponds to the type name in the 
        /// DataObjectSupport XML file.
        /// </param>
        /// <param name="viewType">
        /// Reflection Type object with current type description
        /// </param>
        public ViewObjectAttribute(string typeName, Type viewType)
        {
            if (typeName == null)
                throw new ArgumentNullException("typeName");
            if (viewType == null)
                throw new ArgumentNullException("viewType");

            this.viewTypeNameVal = typeName;
            
            // Must be assignable to IEditor interface
            if (!typeof(IEditor).IsAssignableFrom(viewType))
                throw new NotSupportedException(Resources.Error_NotImplementIEditor);

            // Retrieve proper constructor
            construtorInfo = viewType.GetConstructor(new Type[] { typeof(IDocument) });
            if (construtorInfo == null)
                throw new NotSupportedException(Resources.Error_NoConstructorForEditor);

            // Register document type in the factory
            DocumentViewFactory.Instance.RegisterView(typeName, new DocumentViewFactory.CreateViewMethod(CreateView));
        } 
        #endregion

        #region Public properties and methods
        /// <summary>
        /// Method, used to create view object instance.
        /// </summary>
        /// <param name="document">
        /// Reference to the underlying document object.
        /// </param>
        /// <returns>New instance of the document object</returns>
        public IEditor CreateView(IDocument document)
        {
            return (IEditor)construtorInfo.Invoke(new object[] { document });
        }

        /// <summary>
        /// Name of the underlying document type. Corresponds to the type name in the 
        /// DataObjectSupport XML file.
        /// </summary>
        public string ViewTypeName
        {
            get
            {
                return viewTypeNameVal;
            }
        } 
        #endregion

        #region Private variables
        private ConstructorInfo construtorInfo;
        private string viewTypeNameVal;
        #endregion
    }
}
