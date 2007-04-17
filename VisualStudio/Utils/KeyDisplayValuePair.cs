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
 * This file contains key-displayabe value pair utility used for listboxes and comboboxes.
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace MySql.Data.VisualStudio.Utils
{
    /// <summary>
    /// This small utility is used to add items to listboxses and comboboxes. Key may be any 
    /// object and value is the string that should be displayed in the list.
    /// </summary>
    class KeyDisplayValuePair
    {
        /// <summary>
        /// Key object.
        /// </summary>
        public readonly object Key;
        /// <summary>
        /// Displayable string value.
        /// </summary>
        public readonly string DisplayValue;

        /// <summary>
        /// Constructor which takes key and displayable value.
        /// </summary>
        /// <param name="key">Key object.</param>
        /// <param name="value">Displayable value.</param>
        public KeyDisplayValuePair(object key, string value)
        {
            this.Key = key;
            this.DisplayValue = value;
        }

        /// <summary>
        /// Returns displayable value if any and empty string otherwize.
        /// </summary>
        /// <returns>Returns displayable value if any and empty string otherwize.</returns>
        public override string ToString()
        {
            return DisplayValue != null ? DisplayValue : String.Empty;
        }

        /// <summary>
        /// Returns Key.Equlas(obj) if key is not nul and obj == null otherwize.
        /// </summary>
        /// <param name="obj">Object to compare.</param>
        /// <returns>Returns Key.Equlas(obj) if key is not nul and obj == null otherwize.</returns>
        public override bool Equals(object obj)
        {
            return Key != null ? Key.Equals(obj) : obj == null;
        }

        /// <summary>
        /// Returns hash code for the key or 0 if key is null.
        /// </summary>
        /// <returns>Returns hash code for the key or 0 if key is null.</returns>
        public override int GetHashCode()
        {
            return Key != null ? Key.GetHashCode() : 0;
        }
    }
}
