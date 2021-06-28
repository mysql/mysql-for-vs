// Copyright (c) 2008, 2021, Oracle and/or its affiliates.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License, version 2.0, as
// published by the Free Software Foundation.
//
// This program is also distributed with certain software (including
// but not limited to OpenSSL) that is licensed under separate terms,
// as designated in a particular file or component or in included license
// documentation.  The authors of MySQL hereby grant you an
// additional permission to link the program and your derivative works
// with the separately licensed software that they have included with
// MySQL.
//
// Without limiting anything contained in the foregoing, this file,
// which is part of MySQL for Visual Studio, is also subject to the
// Universal FOSS Exception, version 1.0, a copy of which can be found at
// http://oss.oracle.com/licenses/universal-foss-exception.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU General Public License, version 2.0, for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Data.AdoDotNet;
using Microsoft.VisualStudio.Data;
using System.Data.Common;
using Microsoft.Win32;
using System.Reflection;
using System.IO;
using MySql.Utility.Classes;

namespace MySql.Data.VisualStudio
{
  [Guid("D949EA95-EDA1-4b65-8A9E-266949A99360")]
  class MySqlProviderObjectFactory : AdoDotNetProviderObjectFactory
  {
    /// <summary>
    /// The provider factory object.
    /// </summary>
    private static DbProviderFactory _factory;

    /// <summary>
    /// The error message for the exception raised when calling DbProviderFactories.GetFactory (if any).
    /// </summary>
    private static string _factoryErrorMessage;

    public MySqlProviderObjectFactory()
    {
    }

    /// <summary>
    /// Gets the error message for the exception raised when calling DbProviderFactories.GetFactory (if any).
    /// </summary>
    public static string FactoryErrorMessage => _factoryErrorMessage;

    internal static DbProviderFactory Factory
    {
      get
      {
        _factoryErrorMessage = null;
        if (_factory != null)
        {
          return _factory;
        }

        // Try to get it from DbProviders table.
        try
        {
          _factory = DbProviderFactories.GetFactory("MySql.Data.MySqlClient");
        }
        catch (ArgumentException ex)
        {
          _factoryErrorMessage = ex.Message;
          throw ex;
        }

        return _factory;
      }
    }

    public override object CreateObject(Type objType)
    {
      if (objType == typeof(DataConnectionUIControl))
      {
        return new MySqlDataConnectionUI();
      }
      else if (objType == typeof(DataConnectionProperties))
      {
        return new MySqlConnectionProperties();
      }
      else if (objType == typeof(DataConnectionSupport))
      {
        return new MySqlConnectionSupport();
      }
      else if (objType == typeof(DataSourceSpecializer))
      {
        return new MySqlDataSourceSpecializer();
      }
      else if (objType == typeof(DataConnectionPromptDialog))
      {
        return new MySqlDataConnectionPromptDialog();
      }
      else
      {
        return base.CreateObject(objType);
      }
    }
  }
}
