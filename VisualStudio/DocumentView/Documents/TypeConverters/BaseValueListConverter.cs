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
 * This file contains implementation of the base class for converters 
 * with dynamic list of supported values.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Data;
using System.Collections;
using MySql.Data.VisualStudio.Properties;
using System.Globalization;
using MySql.Data.VisualStudio.Utils;

namespace MySql.Data.VisualStudio.DocumentView
{
    /// <summary>
    /// This is the base class for type converters for properties with dynamic list
    /// of supported values. It implements values reading as template method and it
    /// is very easy to customize it.
    /// </summary>
    abstract class BaseValueListConverter: StringConverter
    {
        #region StringConverter overridings
        /// <summary>
        /// Indicates if standard values are supported. Always returns true.
        /// </summary>
        /// <param name="context">
        /// Context for standard values extraction. Used to get underlying document 
        /// object.
        /// </param>
        /// <returns>Always returns true to show drop-down button.</returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            //true means show a combobox
            return true;
        }

        /// <summary>
        /// Indicate if ONLY standard values are supported.
        /// </summary>
        /// <param name="context">
        /// Context for standard values extraction. Used to get underlying document 
        /// object.
        /// </param>
        /// <returns>Returns the value of the virtual property.</returns>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            //true will limit to list. false will show the list, 
            //but allow free-form entry
            return DenyTextEdit;
        }

        /// <summary>
        /// Main method. Builds collection of supported standard values.
        /// </summary>
        /// <param name="context">
        /// Context for standard values extraction. Used to get underlying document 
        /// object.
        /// </param>
        /// <returns>Returns the collection of the supported standard values.</returns>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (context == null)
            {
                Debug.Fail("Context not specified!");
                return null;
            }
            if (!(context.Instance is IDocument))
            {
                Debug.Fail("Context has no document!");
                return null;
            }

            // Visual Studio doesn't like exceptions here
            try
            {
                // Extract document from the context
                IDocument document = context.Instance as IDocument;

                // Extracting connection
                if (document.Hierarchy == null || document.Hierarchy.Connection == null)
                {
                    Debug.Fail(Resources.Error_InvalidDocumentNoFacadeOrWrapper);
                    return null;
                }
                DataConnectionWrapper connection = document.Hierarchy.Connection;

                // Call virtual method to extract values
                string[] values = ReadStandartValues(document, connection);
                if (values == null)
                    return null;

                return new StandardValuesCollection(values);                 
            }
            catch (Exception e)
            {
                Debug.Fail("Error enumerating default values!", e.ToString());
                return null;
            }
        }


        #endregion

        #region Virtual methods and properties
        /// <summary>
        /// Returns true if ONLY standard values can be used and false if user can 
        /// enter any string.
        /// </summary>
        protected virtual bool DenyTextEdit
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Abstract method for reading standard values using connection and document 
        /// objects.
        /// </summary>
        /// <param name="document">Document object for which we need standard values.</param>
        /// <param name="connection">Connection to use for standard values extraction.</param>
        /// <returns>Returns string array with standard values.</returns>
        protected abstract string[] ReadStandartValues(IDocument document, DataConnectionWrapper connection);
        #endregion
    }
}
