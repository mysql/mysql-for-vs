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
 * This file contains implementation of descriptors factory.
 */
using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.VisualStudio.Descriptors;
using System.Collections;

namespace MySql.Data.VisualStudio.Descriptors
{
    /// <summary>
    /// Descriptor object factory. Used to get descriptors for given object type name.
    /// </summary>
    public class ObjectDescriptorFactory
    {
        #region Singleton implementation
        /// <summary>
        /// Stores unique instance of the factory
        /// </summary>
        private static ObjectDescriptorFactory instanceRef;

        /// <summary>
        /// Returns unique instance of the factory
        /// </summary>
        public static ObjectDescriptorFactory Instance
        {
            get
            {
                if (instanceRef == null)
                    instanceRef = new ObjectDescriptorFactory();
                return instanceRef;
            }
        }

        /// <summary>
        /// Private constructor. Initializes internal collection.
        /// </summary>
        private ObjectDescriptorFactory()
        {
            descriptorsDictionary = new Dictionary<string, CreateDescriptorMethod>();            
        }
        #endregion

        #region Command handlers create delegates collections
        /// <summary>
        /// Collection of the registered descriptor creation methods
        /// </summary>
        Dictionary<string, CreateDescriptorMethod> descriptorsDictionary;

        /// <summary>
        /// Delegate, used for descriptor creation.
        /// </summary>
        /// <returns>Instance of the descriptor.</returns>
        public delegate IObjectDescriptor CreateDescriptorMethod();
        #endregion

        #region Registration methods
        /// <summary>
        /// Registers new descriptor in the factory.
        /// </summary>
        /// <param name="typeName">Object type name for the descriptor.</param>
        /// <param name="createMethod">Create descriptor method delegate.</param>
        internal void RegisterDescriptor(string typeName, CreateDescriptorMethod createMethod)
        {
            if (typeName == null)
                throw new ArgumentNullException("typeName");
            if (createMethod == null)
                throw new ArgumentNullException("createMethod");

            // Check, if descriptor already registered
            if (descriptorsDictionary.ContainsKey(typeName))
                return;
            descriptorsDictionary.Add(typeName, createMethod);
        }
        #endregion

        #region Method for testing
#if DEBUG
        public static IEnumerable DescriptorNames
        {
            get
            {
                return Instance.descriptorsDictionary.Keys;
            }
        }
#endif
        #endregion

        #region Create methods
        /// <summary>
        /// Returns instance of the descriptor.
        /// </summary>
        /// <param name="typeName">Object type name for the descriptor.</param>
        /// <returns>Returns instance of the descriptor.</returns>
        public IObjectDescriptor CreateDescriptor(string typeName)
        {
            // Check descriptor registration
            if (!descriptorsDictionary.ContainsKey(typeName))
                return null;

            // Get create method and create instance
            CreateDescriptorMethod createMethod = descriptorsDictionary[typeName];
            if (createMethod == null)
                return null;
            return createMethod.Invoke();   
        }
        #endregion

        #region Validate methods
        /// <summary>
        /// Checks, if descriptor create method is in the descriptors collection.
        /// </summary>
        /// <param name="typeName">Object type name for the descriptor.</param>
        /// <returns>Rturns true if descriptor create method is in the descriptors collection.</returns>
        public bool IsDescriptorRegistered(string typeName)
        {
            return descriptorsDictionary.ContainsKey(typeName);
        }
        #endregion
    }
}
