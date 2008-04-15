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
 * This file contains implementation of the localizable variant 
 * of the DisplayName attribute
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using MySql.Data.VisualStudio.Utils;

namespace MySql.Data.VisualStudio.DocumentView
{
    /// <summary>
    /// Localizable variant of the DisplayName attribute
    /// </summary>
    class LocalizableDisplayNameAttribute: DisplayNameAttribute
    {
        /// <summary>
        /// This constructor tries to resolve resource name befor calling 
        /// the base constructor.
        /// </summary>
        /// <param name="resourceName">Resource name to load.</param>
        public LocalizableDisplayNameAttribute(string resourceName)
            :base(ResourceHelper.GetLocalizedString(resourceName))
        {
        }
    }
}
