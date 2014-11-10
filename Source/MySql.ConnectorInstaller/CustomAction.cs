// Copyright © 2014 Oracle and/or its affiliates. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using MySQL.Utility.Classes;

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
    public static ActionResult UpdateFlagPackagesFileForVS2013(Session session)
    {
      string VSpath = System.IO.Path.Combine(session.CustomActionData["VS2013_PathProp"], @"Common7\IDE\Extensions\extensions.configurationchanged");
      System.IO.File.WriteAllText(VSpath, string.Empty);

      return ActionResult.Success;
    }

    [CustomAction]
    public static ActionResult UpdateMachineConfigFile(Session session)
    {
      var installedPath = Utility.GetInstallLocation("MySQL for Visual Studio");

      if (String.IsNullOrEmpty(installedPath))
      {
        session.Log("UpdateMachineConfig: not found installed path");
        return ActionResult.NotExecuted;
      }
      
      installedPath = System.IO.Path.Combine(installedPath, @"Assemblies\v2.0\MySql.data.dll");

      if (!File.Exists(installedPath))
      {
        session.Log("UpdateMachineConfig: MySql.data.dll does not exists.");
        return ActionResult.NotExecuted;
      }
            
      Assembly a = Assembly.LoadFile(installedPath);        
      Type customInstallerType = a.GetType("MySql.Data.MySqlClient.CustomInstaller");
        
      if (customInstallerType != null)
        {          
          try
          {
            session.Log("about to invoke method on customInstallerType");            
            var method = customInstallerType.GetMethod("AddProviderToMachineConfig", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
            
            if (method != null)                          
              method.Invoke(null, null);                                       
            else
              session.Log("Method information was null ");

            return ActionResult.Success;
          }
         catch (Exception ex)
         {
            session.Log("error when calling the method " + ex.Message + " " + ex.InnerException.Message);
            return ActionResult.NotExecuted;
         }
        }
        else
        {
          session.Log("Assembly wasn't loaded correctly");
          return ActionResult.NotExecuted;
        }
    }


    [CustomAction]
    public static ActionResult GetConnectorNetVersion(Session session)
    {
      var installedPath = Utility.GetInstallLocation("MySQL Connector/Net");

      session["CNETINSTALLED"] = "0";
      session.Log("Executing GetConnectorNetVersion " + session["CNETINSTALLED"]);

      try
      {
        if (!String.IsNullOrEmpty(installedPath))
        {
          
          installedPath = System.IO.Path.Combine(installedPath, @"Assemblies\v2.0\MySql.data.dll");

          if (!File.Exists(installedPath))
            return ActionResult.Success;

          Assembly a = Assembly.LoadFile(installedPath);

          if (a != null)
          {

            var version = a.GetName().Version;
            if (version < new Version(6, 7))
            {
              session["CNETINSTALLED"] = "1";
              session.Log("Cnet Installed is 1");
              return ActionResult.Success;
            }
          }
          else
            session.Log("Error - Assembly of Connector Net not found");
        }
        return ActionResult.Success;
      }
      catch (Exception ex)
      {
        session.Log("An exception has been caught " + ex.Message);
        return ActionResult.Failure;
      }      
    }
  }
}
