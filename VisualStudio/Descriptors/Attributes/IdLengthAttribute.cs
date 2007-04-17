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
 * This file contains implementation of IdLength custom attribute.
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace MySql.Data.VisualStudio.Descriptors
{
    /// <summary>
    /// This simple attribute is used to declare amount of object identifier 
    /// parts for descriptor object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    sealed class IdLengthAttribute: Attribute
    {
        /// <summary>
        /// Constructor by a given length.
        /// </summary>
        /// <param name="length">Length of identifier</param>
        public IdLengthAttribute(int length)
        {
            this.lengthVal = length;
        }

        /// <summary>
        /// Returns amount of identifier parts for the object.
        /// </summary>
        public int Length
        {
            get { return lengthVal; }
        }

        /// <summary>
        /// Private variable, used to store length.
        /// </summary>
        private int lengthVal;
    }
}
