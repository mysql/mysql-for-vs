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
 * This file contains implementation of support entities factory.
 */

using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Data;
using Microsoft.VisualStudio.Data.AdoDotNet;

namespace MySql.Data.VisualStudio
{
	/// <summary>
    /// Implements support entities factory object. This object is used by 
    /// IDE to create root DDEX support entities.
	/// </summary>
	[Guid("D949EA95-EDA1-4b65-8A9E-266949A99360")]
	public class MySqlProviderObjectFactory : AdoDotNetProviderObjectFactory
	{
		/// <summary>
		/// Creates support entity object by given type. Following types 
        /// are currently supported:
        /// DataConnectionUIControl – customized connection dialog.
        /// DataConnectionProperties – customized connection properties.
        /// DataConnectionSupport – customized connection support.
		/// </summary>
		/// <param name="objType">Etity type to create.</param>
		/// <returns>Reference to created entity.</returns>
        public override object CreateObject(Type objType)
		{
			if (objType == typeof(DataConnectionUIControl))
			{
				return new MySqlConnectionUIControl();
			}
			if (objType == typeof(DataConnectionProperties))
			{
				return new MySqlConnectionProperties();
			}
			if (objType == typeof(DataConnectionSupport))
			{
				return new MySqlConnectionSupport();
			}
            if (objType == typeof(DataSourceSpecializer))
            {
                return new MySqlDataSourceSpecializer();
            }
            if (objType == typeof(DataConnectionPromptDialog))
            {
                return new MySqlDataConnectionPromptDialog();
            }            
			return base.CreateObject(objType);
		}
	}
}
