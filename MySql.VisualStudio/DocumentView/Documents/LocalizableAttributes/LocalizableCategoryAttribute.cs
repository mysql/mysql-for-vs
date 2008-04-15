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
 * of the Category attribute
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using MySql.Data.VisualStudio.Utils;

namespace MySql.Data.VisualStudio.DocumentView
{
    /// <summary>
    /// Localizable variant of the Category attribute
    /// </summary>
    class LocalizableCategoryAttribute: CategoryAttribute
    {
        /// <summary>
        /// Constructor doesn't look for resources, the GetLocalizedString does.
        /// </summary>
        /// <param name="resourceName">Resource or predefined category name.</param>
        public LocalizableCategoryAttribute(string resourceName)
            :base(resourceName)
        {
        }
        
        /// <summary>
        /// Returns localized value for the string. If it is predefined category, 
        /// base method will return not null, otherwise we look for resource string.
        /// </summary>
        /// <param name="value">Resource or predefined category name.</param>
        /// <returns> Returns localized value for the string.</returns>
        protected override string GetLocalizedString(string value)
        {
            string result = base.GetLocalizedString(value);
            return result == null ? ResourceHelper.GetLocalizedString(value) : result;
        }
    }
}
