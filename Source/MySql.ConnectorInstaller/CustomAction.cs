using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using System.IO;
using System.Reflection;
using MySQL.Utility;

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
      var installedPath = Utility.GetInstallLocation("MySQL for Visual Studio");

      if (String.IsNullOrEmpty(installedPath))
        return ActionResult.NotExecuted;
      
      installedPath = System.IO.Path.Combine(installedPath, @"Assemblies\v2.0\MySql.data.dll");

      if (!File.Exists(installedPath))
        return ActionResult.NotExecuted;
            
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
