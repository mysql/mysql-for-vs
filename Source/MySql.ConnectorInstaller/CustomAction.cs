using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;

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
  }
}
