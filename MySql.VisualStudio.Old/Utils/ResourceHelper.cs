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

using MySql.Data.VisualStudio.Properties;
using System.Diagnostics;

namespace MySql.Data.VisualStudio.Utils
{
    /// <summary>
    /// Checks existence of project resources
    /// </summary>
    public static class ResourceHelper
    {
#if DEBUG
        /// <summary>
        /// Checks whether a resource with a given name exists
        /// </summary>
        /// <param name="name">The name of the resource</param>
        /// <returns>True if the resource exists</returns>
        public static bool HasResource(string name)
        {
            object resource = Resources.ResourceManager.GetObject(name);
            return resource != null;
        } 
#endif
        /// <summary>
        /// Used to localize string values. Returns read from 
        /// the resources string, if any. Otherwise returns the 
        /// resource name.
        /// </summary>
        /// <param name="resourceName">Resource string name.</param>
        /// <returns>
        /// Returns read from the resources string, if any. 
        /// Otherwise returns the resource name.
        /// </returns>
        public static string GetLocalizedString(string resourceName)
        {
            string result = Resources.ResourceManager.GetObject(resourceName) as string;
            if (result == null)
                Trace.TraceWarning("Failed to load localaizable value for {0}!", new object[] { resourceName });
            return result != null ? result : resourceName;
        }
    }
}