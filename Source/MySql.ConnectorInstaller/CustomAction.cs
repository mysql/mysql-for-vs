// Copyright © 2013, Oracle and/or its affiliates. All rights reserved.
//
// MySQL Connector/NET is licensed under the terms of the GPLv2
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using System.Reflection;
using Microsoft.Win32;

namespace MySql.ConnectorInstaller
{
  public class CustomActions
  {
    [CustomAction]
    public static ActionResult UpdateFlagPackagesFileForVS2012(Session session)
    {
      string VSpath = System.IO.Path.Combine(session.CustomActionData["VS2012_PathProp"], @"Extensions\extensions.configurationchanged");
      System.IO.File.WriteAllText(VSpath, string.Empty);

      return ActionResult.Success;
    }

    [CustomAction]
    public static ActionResult UpdateMachineConfigFile(Session session)
    {
      var installedPath = GetInstallLocation("MySQL Connector/Net");

      if (String.IsNullOrEmpty(installedPath))
        return ActionResult.NotExecuted;

      installedPath = System.IO.Path.Combine(installedPath + @"\Assemblies\v2.0\MySql.data.dll");

      Assembly a = Assembly.LoadFile(installedPath);
      Type customInstallerType = a.GetType("MySql.Data.MySqlClient.CustomInstaller");

      if (customInstallerType != null)
      {
        try
        {
          var method = customInstallerType.GetMethod("AddProviderToMachineConfig", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod);

          if (method != null)
            method.Invoke(null, null);
          else
            session.Log("Method information was null ");

          return ActionResult.Success;
        }
        catch (Exception ex)
        {
          session.Log("High severity error when calling the method UpdateMachineConfigFile " + ex.Message + " " + ex.InnerException.Message);
          return ActionResult.NotExecuted;
        }
      }
      else
      {
        session.Log("Assembly wasn't loaded correctly");
        return ActionResult.NotExecuted;
      }
    }

    private static string GetInstallLocation(string applicationName)
    {

      string[] keys = new string[3] { @"Software\MySQL AB", @"Software\Wow6432Node\MySQL AB", @"Software\MySQL AB" };
      string location = string.Empty;

      foreach (var item in keys)
      {
        var key = Registry.LocalMachine.OpenSubKey(@item);
        if (key != null) location = GetInstallLocationFromRegistryKey(key, applicationName);

        if (String.IsNullOrEmpty(location))
          key = Registry.CurrentUser.OpenSubKey(@item);

        if (key != null) location = GetInstallLocationFromRegistryKey(key, applicationName);

        if (!String.IsNullOrEmpty(location)) return location;
      }
      return string.Empty;
    }

    private static string GetInstallLocationFromRegistryKey(RegistryKey key, string applicationName)
    {
      if (key == null) return null;
      string[] keys = key.GetSubKeyNames();
      foreach (string subKeyName in keys)
      {
        if (!subKeyName.Contains(applicationName)) continue;
        RegistryKey subKey = key.OpenSubKey(subKeyName);
        return (string)subKey.GetValue("Location", null);
      }
      return null;
    }
  }
}
