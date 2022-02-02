// Copyright (c) 2012-2014, Oracle and/or its affiliates. All rights reserved.
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License as
// published by the Free Software Foundation; version 2 of the
// License.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301  USA

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Text.RegularExpressions;

namespace MySql.Utility.Classes
{
  public class Service
  {
    /// <summary>
    /// Gets all services that match a given filter
    /// </summary>
    /// <returns></returns>
    public static List<ManagementObject> GetInstances(string filter)
    {
      List<ManagementObject> list = new List<ManagementObject>();
      Regex regex = null;

      if (!string.IsNullOrEmpty(filter))
      {
        regex = new Regex(filter);
      }

      ManagementClass mc = new ManagementClass("Win32_Service");
      var instances = mc.GetInstances().Cast<ManagementObject>().ToList();
      foreach (ManagementObject o in instances)
      {
        if (String.IsNullOrEmpty(filter))
        {
          list.Add(o);
        }
        else
        {
          object path = o.GetPropertyValue("PathName");
          if (path == null)
          {
            continue;
          }

          if (regex != null && regex.Match(path.ToString()).Success)
          {
            list.Add(o);
          }
        }
      }
      return list;
    }

    public static bool ExistsServiceInstance(string serviceName)
    {
      ServiceController[] services = ServiceController.GetServices();
      var service = services.Count(s => s.ServiceName == serviceName);
      return service > 0;
    }

    /// <summary>
    /// Determines whether a given executable path contains a call to a MySQL executable.
    /// </summary>
    /// <param name="executablePath">The path to the executable program.</param>
    /// <returns></returns>
    public static bool IsMySqlExecutable(string executablePath)
    {
      if (string.IsNullOrEmpty(executablePath))
      {
        return false;
      }

      var args = Utilities.SplitArgs(executablePath);
      if (args.Length <= 0)
      {
        return false;
      }

      var exeName = args[0];
      return exeName.EndsWith("mysqld.exe") || exeName.EndsWith("mysqld-nt.exe") || exeName.EndsWith("mysqld") || exeName.EndsWith("mysqld-nt");
    }

    /// <summary>
    /// Determines whether the instance is a real MySQL service or not.
    /// </summary>
    /// <param name="serviceName">Name of the service.</param>
    /// <returns><c>true</c> if the instance is a real MySQL service, <c>false otherwise.</c>.</returns>
    public static bool IsRealMySqlService(string serviceName)
    {
      string imagePath;
      return IsRealMySqlService(serviceName, out imagePath);
    }

    /// <summary>
    /// Determines whether the instance is a real MySQL service or not.
    /// </summary>
    /// <param name="serviceName">Name of the service.</param>
    /// <param name="imagePath">ImagePath property of the registry key from the provided service name.</param>
    /// <returns><c>true</c> if the instance is a real MySQL service, <c>false otherwise.</c>.</returns>
    public static bool IsRealMySqlService(string serviceName, out string imagePath)
    {
      imagePath = string.Empty;
      using (var key = RegistryHive.LocalMachine.OpenRegistryKey(@"SYSTEM\CurrentControlSet\Services\" + serviceName))
      {
        if (key == null)
        {
          return false;
        }

        imagePath = key.GetValue("ImagePath", null).ToString();
      }

      return IsMySqlExecutable(imagePath);
    }
  }
}
