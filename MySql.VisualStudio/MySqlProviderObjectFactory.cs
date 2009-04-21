// Copyright © 2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
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
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Data.AdoDotNet;
using Microsoft.VisualStudio.Data;
using System.Data.Common;

namespace MySql.Data.VisualStudio
{
	[Guid("D949EA95-EDA1-4b65-8A9E-266949A99360")]
	class MySqlProviderObjectFactory : AdoDotNetProviderObjectFactory
	{
        private static DbProviderFactory factory;

        internal static DbProviderFactory Factory
        {
            get 
            { 
                if (factory== null)
                    factory = DbProviderFactories.GetFactory("MySql.Data.MySqlClient");
                return factory; 
            }
        }

		public override object CreateObject(Type objType)
		{
            if (objType == typeof(DataConnectionUIControl))
                return new MySqlDataConnectionUI();
            else if (objType == typeof(DataConnectionProperties))
                return new MySqlConnectionProperties();
            else if (objType == typeof(DataConnectionSupport))
                return new MySqlConnectionSupport();
            if (objType == typeof(DataSourceSpecializer))
                return new MySqlDataSourceSpecializer();
            else if (objType == typeof(DataConnectionPromptDialog))
                return new MySqlDataConnectionPromptDialog();
            else
                return base.CreateObject(objType);
		}
	}
}
