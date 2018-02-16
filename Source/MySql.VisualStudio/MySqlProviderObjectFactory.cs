// Copyright © 2008, 2018, Oracle and/or its affiliates. All rights reserved.
//
// MySQL for Visual Studio is licensed under the terms of the GPLv2
// <http://www.gnu.org/licenses/old-licenses/gpl-2.0.html>, like most 
// MySQL Connectors. There are special exceptions to the terms and 
// conditions of the GPLv2 as it is applied to this software, see the 
// FLOSS License Exception
// <http://www.mysql.com/about/legal/licensing/foss-exception.html>.
//
// This program is free software; you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published 
// by the Free Software Foundation; version 2 of the License.
//
// This program is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
//
// You should have received a copy of the GNU General Public License along 
// with this program; if not, write to the Free Software Foundation, Inc., 
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Data.AdoDotNet;
using Microsoft.VisualStudio.Data;
using System.Data.Common;
using Microsoft.Win32;
using System.Reflection;
using System.IO;


namespace MySql.Data.VisualStudio
{
  [Guid("D949EA95-EDA1-4b65-8A9E-266949A99360")]
  class MySqlProviderObjectFactory : AdoDotNetProviderObjectFactory
  {
    private static DbProviderFactory _factory;
    private static Assembly _connectorAssembly;
    private static Version _minConnectorVersion;

    public MySqlProviderObjectFactory()
    {
      _connectorAssembly = null;
      _minConnectorVersion = null;
    }

    internal static Assembly ConnectorAssembly
    {
      get
      {
        if (_connectorAssembly == null)
        {
          _connectorAssembly = File.Exists(ConnectorAssemblyPath)
            ? Assembly.LoadFrom(ConnectorAssemblyPath)
            : null;
        }

        return _connectorAssembly;
      }
    }

    private static string ConnectorAssemblyPath
    {
      get
      {
#if DEBUG
        return System.IO.Path.Combine(System.IO.Path.GetFullPath(System.IO.Path.Combine(Environment.CurrentDirectory, @"..\..\..\..\..")), @"Dependencies\v4.0\Release\MySql.Data.dll");
#else
        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"PrivateAssemblies\MySql.Data.dll");
#endif
      }
    }

    private static Version MinConnectorVersion
    {
      get
      {
        if (_minConnectorVersion == null && ConnectorAssembly != null)
        {
          _minConnectorVersion = ConnectorAssembly.GetName().Version;
        }

        return _minConnectorVersion;
      }
    }

    internal static DbProviderFactory Factory
    {
      get
      {
        if (_factory != null)
        {
          return _factory;
        }

        //try to get it from DbProviders table        
        _factory = DbProviderFactories.GetFactory("MySql.Data.MySqlClient");
        if (_factory == null || (MinConnectorVersion != null &&
              _factory.GetType().Assembly.GetName().Version.CompareTo(MinConnectorVersion) < 0))
        {
          _factory = GetConnectorFromPrivateAssembly();
        }
        
        return _factory;
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

    private static DbProviderFactory GetConnectorFromPrivateAssembly()
    {
      if (ConnectorAssembly == null)
      {
        return null;
      }

      Type dbProviderInstance = ConnectorAssembly.GetType("MySql.Data.MySqlClient.MySqlClientFactory");
      if (dbProviderInstance == null)
      {
        return null;
      }

      var fieldInfo = dbProviderInstance.GetField("Instance", BindingFlags.Public | BindingFlags.Static);
      _factory = (DbProviderFactory)fieldInfo.GetValue(dbProviderInstance);
      return _factory;
    }
  }
}
