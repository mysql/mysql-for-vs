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
 * This file contains implementation of the ObjectDescriptor custom attribute.
 */
using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.VisualStudio.Descriptors;
using System.Reflection;
using MySql.Data.VisualStudio.Properties;

namespace MySql.Data.VisualStudio.Descriptors
{
    /// <summary>
    /// This attribute is used to mark descriptor objects. Object type name and 
    /// descriptor type must be specified for this attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    sealed public class ObjectDescriptorAttribute: Attribute
    {
        #region Initialization
        /// <summary>
        /// Constructor stores type name in the local variable and registers object 
        /// descriptor in the factory.
        /// </summary>
        /// <param name="typeName">Object type name.</param>
        /// <param name="descriptorType">Type of descriptor object.</param>
        public ObjectDescriptorAttribute(string typeName, Type descriptorType)
        {
            if (typeName == null)
                throw new ArgumentNullException("typeName");
            if (descriptorType == null)
                throw new ArgumentNullException("descriptorType");

            // Check interface implementation
            if (!typeof(IObjectDescriptor).IsAssignableFrom(descriptorType))
                throw new NotSupportedException(Resources.Error_NotImplementIObjectDescriptor);

            // Get default construction
            this.constructorRef = descriptorType.GetConstructor(new Type[] { });
            if (this.constructorRef == null)
                throw new NotSupportedException(Resources.Error_NoConstructorForDescriptor);

            // Store type name information
            this.typeNameVal = typeName;

            // Register descriptor within factory
            ObjectDescriptorFactory.Instance.RegisterDescriptor(
                typeName, 
                new ObjectDescriptorFactory.CreateDescriptorMethod(CreateDescriptor));
        } 
        #endregion

        #region Public property
        /// <summary>
        /// Returns object type name
        /// </summary>
        public string TypeName
        {
            get
            {
                return typeNameVal;
            }
        } 
        #endregion

        #region Create method
        /// <summary>
        /// Returns descriptor instance. Creates it at the first call.
        /// </summary>
        /// <returns>Returns descriptor instance.</returns>
        public IObjectDescriptor CreateDescriptor()
        {
            // Create handler instance if not yet created
            if (descriptorInstance == null)
                descriptorInstance = constructorRef.Invoke(new object[] { }) as IObjectDescriptor;

            return descriptorInstance;
        }
        #endregion

        #region Private variables
        /// <summary>
        /// Used to store object type name
        /// </summary>
        private string typeNameVal;
        /// <summary>
        /// Constructor, used to create descriptor instance
        /// </summary>
        private ConstructorInfo constructorRef;
        /// <summary>
        /// Reference to the created descriptor instance
        /// </summary>
        private IObjectDescriptor descriptorInstance;
        #endregion
    }
}
