// Copyright © 2014, 2016 Oracle and/or its affiliates. All rights reserved.
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
using Microsoft.Deployment.WindowsInstaller;
using System.IO;
using System.Reflection;
using Microsoft.Win32;
using MySql.Utility.Classes;

namespace MySql.VisualStudio.CustomAction
{
  public class CustomActions
  {
    /// <summary>
    /// Enum used to list the supported Visual Studio versions that execute the "UpdateFlagPackagesFile" methods.
    /// </summary>
    enum SupportedVsVersions
    {
      Vs2012, Vs2013, Vs2015
    }

    #region [Constants]
    private const string VS2012_VERSION_NUMBER = "11.0";
    private const string VS2013_VERSION_NUMBER = "12.0";
    private const string VS2015_VERSION_NUMBER = "14.0";
    private const string VS2012_X64_EXTENSIONS_FILE_PATH = @"C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\";
    private const string VS2012_X86_EXTENSIONS_FILE_PATH = @"C:\Program Files\Microsoft Visual Studio 11.0\Common7\IDE\";
    private const string VS2013_X64_EXTENSIONS_FILE_PATH = @"C:\Program Files (x86)\Microsoft Visual Studio 12.0\";
    private const string VS2013_X86_EXTENSIONS_FILE_PATH = @"C:\Program Files\Microsoft Visual Studio 12.0\";
    private const string VS2015_X64_EXTENSIONS_FILE_PATH = @"C:\Program Files (x86)\Microsoft Visual Studio 14.0\";
    private const string VS2015_X86_EXTENSIONS_FILE_PATH = @"C:\Program Files\Microsoft Visual Studio 14.0\";
    private const string EXTENSION_FILE_NAME = "extensions.configurationchanged";
    #endregion

    [CustomAction]
    public static ActionResult UpdateFlagPackagesFileForVs2012(Session session)
    {
      string vsPath = Path.Combine(session.CustomActionData["VS2012_PathProp"], @"Extensions\extensions.configurationchanged");

      if (File.Exists(vsPath))
        File.WriteAllText(vsPath, string.Empty);

      return ActionResult.Success;
    }

    [CustomAction]
    public static ActionResult UpdateFlagPackagesFileForVs2013(Session session)
    {
      string vsPath = Path.Combine(session.CustomActionData["VS2013_PathProp"], @"Common7\IDE\Extensions\extensions.configurationchanged");

      if (File.Exists(vsPath))
        File.WriteAllText(vsPath, string.Empty);

      return ActionResult.Success;
    }

    [CustomAction]
    public static ActionResult UpdateFlagPackagesFileForVs2015(Session session)
    {
      string vsPath = Path.Combine(session.CustomActionData["VS2015_PathProp"], @"Common7\IDE\Extensions\extensions.configurationchanged");

      if (File.Exists(vsPath))
        File.WriteAllText(vsPath, string.Empty);

      return ActionResult.Success;
    }

    [CustomAction]
    public static ActionResult UpdateMachineConfigFile(Session session)
    {
      var installedPath = Utilities.GetMySqlAppInstallLocation("MySQL for Visual Studio");

      if (string.IsNullOrEmpty(installedPath))
      {
        session.Log("UpdateMachineConfig: not found installed file");
        return ActionResult.NotExecuted;
      }

      installedPath = Path.Combine(installedPath, @"Assemblies\MySql.data.dll");
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
          session.Log("error when calling the method " + ex.Message + " " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
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
      var installedPath = Utilities.GetMySqlAppInstallLocation("MySQL Connector/Net");

      session["CNETINSTALLED"] = "0";
      session.Log("Executing GetConnectorNetVersion " + session["CNETINSTALLED"]);

      try
      {
        if (string.IsNullOrEmpty(installedPath))
          return ActionResult.Success;

        installedPath = Path.Combine(installedPath, @"Assemblies\v4.0\MySql.data.dll");
        if (!File.Exists(installedPath))
          return ActionResult.Success;

        var a = Assembly.LoadFile(installedPath);
        if (a != null)
        {
          var version = a.GetName().Version;
          if (version >= new Version(6, 7))
            return ActionResult.Success;

          session["CNETINSTALLED"] = "1";
          session.Log("Cnet Installed is 1");
          return ActionResult.Success;
        }
        else
          session.Log("Error - Assembly of Connector Net not found");

        return ActionResult.Success;
      }
      catch (Exception ex)
      {
        session.Log("An exception has been caught " + ex.Message);
        return ActionResult.Failure;
      }
    }

    /// <summary>
    /// Method to handle the registry key and the extensions file creation, for all the supported Visual Studio versions.
    /// </summary>
    /// <param name="session">The session object containing the parameters sent by Wix.</param>
    /// <returns>Will return Failure status in case of any errors. Otherwise, Success</returns>
    [CustomAction]
    public static ActionResult CreateRegKeyAndExtensionsFile(Session session)
    {
      return CreateDeleteRegKeyAndExtensionsFile(session, false) ? ActionResult.Success : ActionResult.Failure;
    }

    /// <summary>
    /// Method to handle the registry key and the extensions file deletion, for all the supported Visual Studio versions.
    /// </summary>
    /// <param name="session">The session object containing the parameters sent by Wix.</param>
    /// <returns>Will return Failure status in case of any errors. Otherwise, Success</returns>
    [CustomAction]
    public static ActionResult DeleteRegKeyAndExtensionsFile(Session session)
    {
      return CreateDeleteRegKeyAndExtensionsFile(session, true) ? ActionResult.Success : ActionResult.Failure;
    }

    /// <summary>
    /// Method to handle the registry key and the extensions file deletion, for all the supported Visual Studio versions.
    /// </summary>
    /// <param name="session">The session object containing the parameters sent by Wix.</param>
    /// <param name="isDeleting">Flag indicating whether the key is being deleted.</param>
    /// <returns>Will return false in case of any errors. True in case of success.</returns>
    private static bool CreateDeleteRegKeyAndExtensionsFile(Session session, bool isDeleting)
    {
      try
      {
        string sVsVersion = session.CustomActionData["VSVersion"];
        if (string.IsNullOrEmpty(sVsVersion))
        {
          return false;
        }

        SupportedVsVersions vsVersion;
        if (Enum.TryParse(sVsVersion, true, out vsVersion))
        {
          string vsVersionNumber;
          string vsPath;
          switch (vsVersion)
          {
            case SupportedVsVersions.Vs2012:
              vsPath = Environment.Is64BitOperatingSystem ? VS2012_X64_EXTENSIONS_FILE_PATH : VS2012_X86_EXTENSIONS_FILE_PATH;
              vsVersionNumber = VS2012_VERSION_NUMBER;
              break;
            case SupportedVsVersions.Vs2013:
              vsPath = Environment.Is64BitOperatingSystem ? VS2013_X64_EXTENSIONS_FILE_PATH : VS2013_X86_EXTENSIONS_FILE_PATH;
              vsVersionNumber = VS2013_VERSION_NUMBER;
              break;
            case SupportedVsVersions.Vs2015:
              vsPath = Environment.Is64BitOperatingSystem ? VS2015_X64_EXTENSIONS_FILE_PATH : VS2015_X86_EXTENSIONS_FILE_PATH;
              vsVersionNumber = VS2015_VERSION_NUMBER;
              break;
            default:
              throw new Exception("Could not parse parameter VSVersion to a valid 'SupportedVSVersions' value.");
          }

          // Registry key handling
          string keyPath = GetRegKeyPath(vsVersionNumber, vsVersion == SupportedVsVersions.Vs2012);
          string keyName = vsVersion == SupportedVsVersions.Vs2012 ? "EnvironmentDirectory" : "ShellFolder";
          var key = Registry.LocalMachine.OpenSubKey(keyPath, true);
          if (!isDeleting)
          {
            session.Log(string.Format("Creating Registry key. keyPath = '{0}'. keyName='{1}'", keyPath, keyName));
            if (key == null)
            {
              key = Registry.LocalMachine.CreateSubKey(keyPath, RegistryKeyPermissionCheck.ReadWriteSubTree);
              if (key != null)
              {
                key.SetValue(keyName, vsPath, RegistryValueKind.String);
                session.Log(string.Format("Created Registry key. keyPath = '{0}'. keyName='{1}'", keyPath, keyName));
              }
            }
            else
            {
              var keyValue = key.GetValue(keyName);
              if (keyValue == null)
              {
                key.SetValue(keyName, vsPath, RegistryValueKind.String);
                session.Log(string.Format("Created Registry key. keyPath = '{0}'. keyName='{1}'", keyPath, keyName));
              }
              key.Close();
            }
          }
          else
          {
            session.Log(string.Format("Deleting Registry key. keyPath = '{0}'. keyName='{1}'", keyPath, keyName));
            if (key != null)
            {
              var keyValue = key.GetValue(keyName);
              if (keyValue != null)
              {
                key.DeleteValue(keyName);
                session.Log(string.Format("Deleted Registry key. keyPath = '{0}'. keyName='{1}'", keyPath, keyName));
              }
              key.Close();
            }
          }

          // "extensions.configurationchanged" file handling
          string extensionsDirectory = string.Format(@"{0}{1}",
            vsPath,
            vsVersion == SupportedVsVersions.Vs2012
              ? @"Extensions"
              : @"Common7\IDE\Extensions");
          string extensionsFile = Path.Combine(extensionsDirectory, EXTENSION_FILE_NAME);
          if (!isDeleting)
          {
            if (!Directory.Exists(extensionsDirectory))
            {
              Directory.CreateDirectory(extensionsDirectory);
            }

            if (!File.Exists(extensionsFile))
            {
              File.Create(extensionsFile);
              session.Log(string.Format("Extensions file created for {0}.", vsVersion));
            }
          }
          else
          {
            if (File.Exists(extensionsFile))
            {
              File.Delete(extensionsFile);
              session.Log(string.Format("File deleted for {0}_EXTENSIONSFILE_CREATED.", vsVersion));
            }
          }

          return true;
        }
        else
        {
          throw new Exception("Could not parse parameter VSVersion to a valid 'SupportedVSVersions' value.");
        }
      }
      catch (Exception ex)
      {
        session.Log(string.Format("Error executing 'CreateDeleteRegKeyAndExtensionsFile'. {0}. {1}", ex.Message, ex.InnerException != null ? ex.InnerException.ToString() : string.Empty));
        return false;
      }
    }

    /// <summary>
    /// Generates the reg key path for the specified Visual Studio version.
    /// </summary>
    /// <param name="vsVersion">The Visual Studio version.</param>
    /// <param name="isVs2012">Flag used to differentiate whether the registry key is for Visual Studio 2012, in which case should be handled differently.</param>
    /// <returns>The key path for the specified Visual Studio version</returns>
    private static string GetRegKeyPath(string vsVersion, bool isVs2012)
    {
      string keyPath;
      if (Environment.Is64BitOperatingSystem)
      {
        keyPath = string.Format(@"Software\Wow6432Node\Microsoft\VisualStudio\{0}", vsVersion);
      }
      else
      {
        keyPath = string.Format(@"Software\Microsoft\VisualStudio\{0}", vsVersion);
      }

      if (isVs2012)
      {
        keyPath += @"\Setup\VS";
      }

      return keyPath;
    }

    /// <summary>
    /// Sets the specified value into the specific session variable.
    /// </summary>
    /// <param name="session">The session object to interact with the installer variables.</param>
    /// <param name="sessionName">The name of the session to set the value.</param>
    /// <param name="value">The value to be set.</param>
    public static void SetSessionValue(Session session, string sessionName, string value)
    {
      session.Log("Set session value. " + sessionName + " - value = " + value);
      session[sessionName] = value;
    }
  }
}
