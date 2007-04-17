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
 * This file contains Reflection utility, used to retrieve type attributes.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using MySql.Data.VisualStudio.Properties;
using MySql.Data.VisualStudio.DocumentView;
using MySql.Data.VisualStudio.Commands;
using MySql.Data.VisualStudio.Descriptors;

namespace MySql.Data.VisualStudio.Utils
{
    /// <summary>
    /// This class provides set of useful methods for retrieving custom 
    /// type attributes using reflection.
    /// </summary>
    static class ReflectionHelper
    {
        #region Custom attributes retrieving
        /// <summary>
        /// Returns attribute of type DocumentObjectAttribute.
        /// </summary>
        /// <param name="sourceType">Source type, which is marked with custom attribute.</param>
        /// <returns>Returns attribute of type DocumentObjectAttribute.</returns>
        public static DocumentObjectAttribute GetDocumentObjectAttribute(ICustomAttributeProvider sourceType)
        {
            if (sourceType == null)
                throw new ArgumentNullException("sourceType");
            return GetCustomAttribute(typeof(DocumentObjectAttribute), sourceType) as DocumentObjectAttribute;
        }

        /// <summary>
        /// Returns attribute of type ViewObjectAttribute.
        /// </summary>
        /// <param name="sourceType">Source type, which is marked with custom attribute.</param>
        /// <returns>Returns attribute of type ViewObjectAttribute.</returns>
        public static ViewObjectAttribute GetViewObjectAttribute(ICustomAttributeProvider sourceType)
        {
            if (sourceType == null)
                throw new ArgumentNullException("sourceType");
            return GetCustomAttribute(typeof(ViewObjectAttribute), sourceType) as ViewObjectAttribute;
        }

        /// <summary>
        /// Returns attribute of type CommandHandlerAttribute.
        /// </summary>
        /// <param name="sourceType">Source type, which is marked with custom attribute.</param>
        /// <returns>Returns attribute of type CommandHandlerAttribute.</returns>
        public static CommandHandlerAttribute GetCommandHandlerAttribute(ICustomAttributeProvider sourceType)
        {
            if (sourceType == null)
                throw new ArgumentNullException("CommandHandlerAttribute");
            return GetCustomAttribute(typeof(CommandHandlerAttribute), sourceType) as CommandHandlerAttribute;
        }

        /// <summary>
        /// Returns attribute of type ObjectDescriptorAttribute.
        /// </summary>
        /// <param name="sourceType">Source type, which is marked with custom attribute.</param>
        /// <returns>Returns attribute of type ObjectDescriptorAttribute.</returns>
        public static ObjectDescriptorAttribute GetObjectDescriptorAttribute(ICustomAttributeProvider sourceType)
        {
            if (sourceType == null)
                throw new ArgumentNullException("ObjectDescriptorAttribute");
            return GetCustomAttribute(typeof(ObjectDescriptorAttribute), sourceType) as ObjectDescriptorAttribute;
        }

        /// <summary>
        /// Returns attribute of type IdLengthAttribute.
        /// </summary>
        /// <param name="sourceType">Source type, which is marked with custom attribute.</param>
        /// <returns>Returns attribute of type IdLengthAttribute.</returns>
        public static IdLengthAttribute GetIdLengthAttribute(ICustomAttributeProvider sourceType)
        {
            if (sourceType == null)
                throw new ArgumentNullException("IdLengthAttribute");
            return GetCustomAttribute(typeof(IdLengthAttribute), sourceType) as IdLengthAttribute;
        }

        /// <summary>
        /// Returns attribute of type IdentifierAttribute.
        /// </summary>
        /// <param name="sourceType">Source type, which is marked with custom attribute.</param>
        /// <returns>Returns attribute of type IdentifierAttribute.</returns>
        public static IdentifierAttribute GetIdentifierAttribute(ICustomAttributeProvider sourceType)
        {
            if (sourceType == null)
                throw new ArgumentNullException("IdLengthAttribute");
            return GetCustomAttribute(typeof(IdentifierAttribute), sourceType) as IdentifierAttribute;
        }

        /// <summary>
        /// Returns attribute of type FieldStringAttribute.
        /// </summary>
        /// <param name="sourceType">Source type, which is marked with custom attribute.</param>
        /// <returns>Returns attribute of type FieldStringAttribute.</returns>
        public static OptionStringAttribute GetFieldStringAttribute(ICustomAttributeProvider sourceType)
        {
            if (sourceType == null)
                throw new ArgumentNullException("FieldStringAttribute");
            return GetCustomAttribute(typeof(OptionStringAttribute), sourceType) as OptionStringAttribute;
        }

        /// <summary>
        /// Returns attribute of the FieldAttribute type
        /// </summary>
        /// <param name="sourceType">Source type, which is marked with the custom attribute</param>
        /// <returns>Returns attribute of the FieldAttribute type</returns>
        public static FieldAttribute GetFieldAttribute(ICustomAttributeProvider sourceType)
        {
            if (sourceType == null)
                throw new ArgumentNullException("FieldAttribute");

            return GetCustomAttribute(typeof(FieldAttribute), sourceType) as FieldAttribute;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Returns custom attribute of given type as un-typed object value.
        /// </summary>
        /// <param name="attributeType">Type of custom attribute to retrieve.</param>
        /// <param name="sourceType">Source type, which is marked with custom attribute.</param>
        /// <returns>Returns custom attribute of given type as un-typed object value.</returns>
        private static object GetCustomAttribute(Type attributeType, ICustomAttributeProvider sourceType)
        {
            Object[] attributes = sourceType.GetCustomAttributes(attributeType, false);
            if (attributes == null || attributes.Length < 1
                || !(attributes[0].GetType() == attributeType))
                return null;
            return attributes[0];
        }
        #endregion
    }
}