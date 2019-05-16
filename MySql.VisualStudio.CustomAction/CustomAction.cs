// Copyright (c) 2014, 2019, Oracle and/or its affiliates. All rights reserved.
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

using Microsoft.Deployment.WindowsInstaller;
using Microsoft.VisualStudio.Setup.Configuration;
using Microsoft.Win32;
using MySql.Utility.Classes;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml;

namespace MySql.VisualStudio.CustomAction
{
  public class CustomActions
  {
    /// <summary>
    /// Enum used to list the supported Visual Studio versions that execute the "UpdateFlagPackagesFile" methods.
    /// </summary>
    enum SupportedVsVersions
    {
      Vs2015, Vs2017Community, Vs2017Enterprise, Vs2017Professional, Vs2019Community, Vs2019Enterprise, Vs2019Professional
    }

    #region [Constants]
    private const string VS2015_VERSION_NUMBER = "14.0";
    private const string VS2017_VERSION_NUMBER = "15.0";
    private const string VS2019_VERSION_NUMBER = "16.0";
    private const string VS2015_X64_EXTENSIONS_FILE_PATH = @"C:\Program Files (x86)\Microsoft Visual Studio 14.0\";
    private const string VS2015_X86_EXTENSIONS_FILE_PATH = @"C:\Program Files\Microsoft Visual Studio 14.0\";
    private const string EXTENSION_FILE_NAME = "extensions.configurationchanged";
    private const int REGDB_E_CLASSNOTREG = unchecked((int)0x80040154);
    #endregion

    #region [Fields]
    private static string _vs2017CommunityInstallationPath;
    private static string _VS2017CommunityX64ExtensionsFilePath;
    private static string _VS2017CommunityX86ExtensionsFilePath;
    private static string _vs2017EnterpriseInstallationPath;
    private static string _VS2017EnterpriseX64ExtensionsFilePath;
    private static string _VS2017EnterpriseX86ExtensionsFilePath;
    private static string _vs2017ProfessionalInstallationPath;
    private static string _VS2017ProfessionalX64ExtensionsFilePath;
    private static string _VS2017ProfessionalX86ExtensionsFilePath;
    private static string _vs2017CommunityInstanceId;
    private static string _vs2017EnterpriseInstanceId;
    private static string _vs2017ProfessionalInstanceId;

    private static string _vs2019CommunityInstallationPath;
    private static string _VS2019CommunityX64ExtensionsFilePath;
    private static string _VS2019CommunityX86ExtensionsFilePath;
    private static string _vs2019EnterpriseInstallationPath;
    private static string _VS2019EnterpriseX64ExtensionsFilePath;
    private static string _VS2019EnterpriseX86ExtensionsFilePath;
    private static string _vs2019ProfessionalInstallationPath;
    private static string _VS2019ProfessionalX64ExtensionsFilePath;
    private static string _VS2019ProfessionalX86ExtensionsFilePath;
    private static string _vs2019CommunityInstanceId;
    private static string _vs2019EnterpriseInstanceId;
    private static string _vs2019ProfessionalInstanceId;
    #endregion

    /// <summary>
    /// Static constructor that initializes VS2017/2019's paths for all VS2017/2019 flavors.
    /// </summary>
    static CustomActions()
    {
      SetVSInstallationPaths();

      if (!string.IsNullOrEmpty(_vs2017CommunityInstallationPath))
      {
        _VS2017CommunityX64ExtensionsFilePath = _vs2017CommunityInstallationPath + @"\";
        _VS2017CommunityX86ExtensionsFilePath = _vs2017CommunityInstallationPath.Replace(" (86)","") + @"\";
      }
      else
      {
        var partialPath = @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\";
        _VS2017CommunityX64ExtensionsFilePath = partialPath;
        _VS2017CommunityX86ExtensionsFilePath = partialPath.Replace(" (86)","");
      }

      if (!string.IsNullOrEmpty(_vs2017EnterpriseInstallationPath))
      {
        _VS2017EnterpriseX64ExtensionsFilePath = _vs2017EnterpriseInstallationPath + @"\";
        _VS2017EnterpriseX86ExtensionsFilePath = _vs2017EnterpriseInstallationPath.Replace(" (86)","") + @"\";
      }
      else
      {
        var partialPath = @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\";
        _VS2017EnterpriseX64ExtensionsFilePath = partialPath;
        _VS2017EnterpriseX86ExtensionsFilePath = partialPath.Replace(" (86)","");
      }

      if (!string.IsNullOrEmpty(_vs2017ProfessionalInstallationPath))
      {
        _VS2017ProfessionalX64ExtensionsFilePath = _vs2017ProfessionalInstallationPath + @"\";
        _VS2017ProfessionalX86ExtensionsFilePath = _vs2017ProfessionalInstallationPath.Replace(" (86)","") + @"\";
      }
      else
      {
        var partialPath = @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\";
        _VS2017ProfessionalX64ExtensionsFilePath = partialPath;
        _VS2017ProfessionalX86ExtensionsFilePath = partialPath.Replace(" (86)","");
      }

      if (!string.IsNullOrEmpty(_vs2019CommunityInstallationPath))
      {
        _VS2019CommunityX64ExtensionsFilePath = _vs2019CommunityInstallationPath + @"\";
        _VS2019CommunityX86ExtensionsFilePath = _vs2019CommunityInstallationPath.Replace(" (86)","") + @"\";
      }
      else
      {
        var partialPath = @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\";
        _VS2019CommunityX64ExtensionsFilePath = partialPath;
        _VS2019CommunityX86ExtensionsFilePath = partialPath.Replace(" (86)","");
      }

      if (!string.IsNullOrEmpty(_vs2019EnterpriseInstallationPath))
      {
        _VS2019EnterpriseX64ExtensionsFilePath = _vs2019EnterpriseInstallationPath + @"\";
        _VS2019EnterpriseX86ExtensionsFilePath = _vs2019EnterpriseInstallationPath.Replace(" (86)","") + @"\";
      }
      else
      {
        var partialPath = @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\";
        _VS2019EnterpriseX64ExtensionsFilePath = partialPath;
        _VS2019EnterpriseX86ExtensionsFilePath = partialPath.Replace(" (86)","");
      }

      if (!string.IsNullOrEmpty(_vs2019ProfessionalInstallationPath))
      {
        _VS2019ProfessionalX64ExtensionsFilePath = _vs2019ProfessionalInstallationPath + @"\";
        _VS2019ProfessionalX86ExtensionsFilePath = _vs2019ProfessionalInstallationPath.Replace(" (86)","") + @"\";
      }
      else
      {
        var partialPath = @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\";
        _VS2019ProfessionalX64ExtensionsFilePath = partialPath;
        _VS2019ProfessionalX86ExtensionsFilePath = partialPath.Replace(" (86)","");
      }
    }

    #region Custom Actions
    [CustomAction]
    public static ActionResult UpdateFlagPackagesFileForVs2015(Session session)
    {
      string vsPath = Path.Combine(session.CustomActionData["VS2015_PathProp"], @"Common7\IDE\Extensions\extensions.configurationchanged");

      if (File.Exists(vsPath))
      {
        File.WriteAllText(vsPath, string.Empty);
      }

      return ActionResult.Success;
    }

    [CustomAction]
    public static ActionResult UpdateFlagPackagesFileForVs2017(Session session)
    {
      string vsPath = Path.Combine(session.CustomActionData["VS2017_PathProp"], @"Common7\IDE\Extensions\extensions.configurationchanged");

      if (File.Exists(vsPath))
      {
        File.WriteAllText(vsPath, string.Empty);
      }

      return ActionResult.Success;
    }

    [CustomAction]
    public static ActionResult UpdateFlagPackagesFileForVs2017Enterprise(Session session)
    {
      string vsPath = Path.Combine(session.CustomActionData["VS2017_Ent_PathProp"], @"Common7\IDE\Extensions\extensions.configurationchanged");

      if (File.Exists(vsPath))
      {
        File.WriteAllText(vsPath, string.Empty);
      }

      return ActionResult.Success;
    }

    [CustomAction]
    public static ActionResult UpdateFlagPackagesFileForVs2017Professional(Session session)
    {
      string vsPath = Path.Combine(session.CustomActionData["VS2017_Pro_PathProp"], @"Common7\IDE\Extensions\extensions.configurationchanged");

      if (File.Exists(vsPath))
      {
        File.WriteAllText(vsPath, string.Empty);
      }

      return ActionResult.Success;
    }

    [CustomAction]
    public static ActionResult UpdateFlagPackagesFileForVs2019(Session session)
    {
      string vsPath = Path.Combine(session.CustomActionData["VS2019_PathProp"], @"Common7\IDE\Extensions\extensions.configurationchanged");

      if (File.Exists(vsPath))
      {
        File.WriteAllText(vsPath, string.Empty);
      }

      return ActionResult.Success;
    }

    [CustomAction]
    public static ActionResult UpdateFlagPackagesFileForVs2019Enterprise(Session session)
    {
      string vsPath = Path.Combine(session.CustomActionData["VS2019_Ent_PathProp"], @"Common7\IDE\Extensions\extensions.configurationchanged");

      if (File.Exists(vsPath))
      {
        File.WriteAllText(vsPath, string.Empty);
      }

      return ActionResult.Success;
    }

    [CustomAction]
    public static ActionResult UpdateFlagPackagesFileForVs2019Professional(Session session)
    {
      string vsPath = Path.Combine(session.CustomActionData["VS2019_Pro_PathProp"], @"Common7\IDE\Extensions\extensions.configurationchanged");

      if (File.Exists(vsPath))
      {
        File.WriteAllText(vsPath, string.Empty);
      }

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
          {
            method.Invoke(null, null);
          }
          else
          {
            session.Log("Method information was null ");
          }

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
      var installedPath = Utilities.GetMySqlAppInstallLocation("MySQL Connector/Net");

      session["CNETINSTALLED"] = "0";
      session.Log("Executing GetConnectorNetVersion " + session["CNETINSTALLED"]);

      try
      {
        if (string.IsNullOrEmpty(installedPath))
        {
          return ActionResult.Success;
        }

        installedPath = Path.Combine(installedPath, @"Assemblies\v4.0\MySql.data.dll");
        if (!File.Exists(installedPath))
        {
          return ActionResult.Success;
        }

        var a = Assembly.LoadFile(installedPath);
        if (a != null)
        {
          var version = a.GetName().Version;
          if (version >= new Version(6, 7))
          {
            return ActionResult.Success;
          }

          session["CNETINSTALLED"] = "1";
          session.Log("Cnet Installed is 1");
          return ActionResult.Success;
        }
        else
        {
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

    /// <summary>
    /// Sets the installation paths for all VS2017+ flavors.
    /// </summary>
    /// <param name="session">The session object containing the parameters sent by Wix.</param>
    /// <returns>Always returns ActionResult.Success since its return value isn't relevant.</returns>
    [CustomAction]
    public static ActionResult SetVSInstallationPaths(Session session)
    {
      if (!string.IsNullOrEmpty(_vs2017CommunityInstallationPath))
      {
        session["VS_2017_COM_PATH_MAIN"] = session["VS_2017_COM_PATH"] = _vs2017CommunityInstallationPath;
      }

      if (!string.IsNullOrEmpty(_vs2017EnterpriseInstallationPath))
      {
        session["VS_2017_ENT_PATH_MAIN"] = session["VS_2017_ENT_PATH"] = _vs2017EnterpriseInstallationPath;
      }

      if (!string.IsNullOrEmpty(_vs2017ProfessionalInstallationPath))
      {
        session["VS_2017_PRO_PATH_MAIN"] = session["VS_2017_PRO_PATH"] = _vs2017ProfessionalInstallationPath;
      }

      if (!string.IsNullOrEmpty(_vs2019CommunityInstallationPath))
      {
        session["VS_2019_COM_PATH_MAIN"] = session["VS_2019_COM_PATH"] = _vs2019CommunityInstallationPath;
      }

      if (!string.IsNullOrEmpty(_vs2019EnterpriseInstallationPath))
      {
        session["VS_2019_ENT_PATH_MAIN"] = session["VS_2019_ENT_PATH"] = _vs2019EnterpriseInstallationPath;
      }

      if (!string.IsNullOrEmpty(_vs2019ProfessionalInstallationPath))
      {
        session["VS_2019_PRO_PATH_MAIN"] = session["VS_2019_PRO_PATH"] = _vs2019ProfessionalInstallationPath;
      }

      return ActionResult.Success;
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
    #endregion

    /// <summary>
    /// Displays a warning message for Win7 users if the devenv /updateconfiguration command failed to execute.
    /// </summary>
    /// <param name="session">The session object containing the parameters sent by Wix.</param>
    /// <returns>An <see cref="ActionResult.Success"> object since the results of the code executed in this method are irrelevant to the overall installation process."</returns>
    [CustomAction]
    public static ActionResult ShowWindows7InstallationWarning(Session session)
    {
      bool showWarning = false;

      // Read the ActivityLog from the user's AppData folder.
      if (!string.IsNullOrEmpty(_vs2017CommunityInstallationPath))
      {
        showWarning = ReadActivityLog(_vs2017CommunityInstanceId, 15, session["AppDataFolder"], session);
      }
      else if (!string.IsNullOrEmpty(_vs2017EnterpriseInstallationPath))
      {
        showWarning = ReadActivityLog(_vs2017EnterpriseInstanceId, 15, session["AppDataFolder"], session);
      }
      else if (!string.IsNullOrEmpty(_vs2017ProfessionalInstallationPath))
      {
        showWarning = ReadActivityLog(_vs2017ProfessionalInstanceId, 15, session["AppDataFolder"], session);
      }
      else if (!string.IsNullOrEmpty(_vs2019CommunityInstallationPath))
      {
        showWarning = ReadActivityLog(_vs2019CommunityInstanceId, 16, session["AppDataFolder"], session);
      }
      else if (!string.IsNullOrEmpty(_vs2019EnterpriseInstallationPath))
      {
        showWarning = ReadActivityLog(_vs2019EnterpriseInstanceId, 16, session["AppDataFolder"], session);
      }
      else if (!string.IsNullOrEmpty(_vs2019ProfessionalInstallationPath))
      {
        showWarning = ReadActivityLog(_vs2019ProfessionalInstanceId, 16, session["AppDataFolder"], session);
      }

      if (showWarning)
      {
        string message = "WARNING: The \"devenv /updateconfiguration\" command may have failed to execute succesfully preventing Visual Studio from registering/unregistering MySQL for Visual Studio. If affected, run the command manually using \"Developer Command Prompt for VS\". For additional details, see the product documentation.";
        session.Message(InstallMessage.Warning, new Record { FormatString = message });
      }

      return ActionResult.Success;
    }

    /// <summary>
    /// Reads the ActivityLog of the specified VS instance.
    /// </summary>
    /// <param name="vsInstanceId">The instance id.</param>
    /// <param name="vsVersion">The Visual Studio version for which to read the activity log.</param>
    /// <param name="appDataFolder">The path to the user's app data folder.</param>
    /// <returns><c>True</c> if the warning message should be displayed or if an error prevented from reading the ActivityLog; otherwise, <c>false</c>.</returns>
    private static bool ReadActivityLog(string vsInstanceId, int vsVersion, string appDataFolder, Session session)
    {
      try
      {
        var activityLog = string.Format("{0}Microsoft\\VisualStudio\\{1}.0_{2}\\ActivityLog.xml", appDataFolder, vsVersion, vsInstanceId);
        session.Log("ActivityLog path: " + activityLog);

        if (File.Exists(activityLog))
        {
          var file = File.ReadAllText(activityLog);
          var document = new XmlDocument();
          document.LoadXml(file);
          var entries = document.DocumentElement.SelectNodes("/activity/entry");

          // Read entries for ACCESS_DENIED errors.
          foreach (XmlNode item in entries)
          {
            var errorNode = item.SelectSingleNode("type");
            if (errorNode.InnerText != "Error") continue;

            var hrText = item.SelectSingleNode("hr").InnerText;
            if (!string.IsNullOrEmpty(hrText) && hrText.Contains("E_ACCESSDENIED"))
            {
              session.Log($"Error found in the ActivityLog. Warning message will be displayed for instance: {vsVersion}.0_{vsInstanceId}");
              return true;
            }
          }
        }
        else return true;

        return false;
      }
      catch (Exception ex)
      {
        session.Log("An exception was raised when attempting to read the ActivityLog: " + ex.Message);
        return true;
      }
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
          string vsRootPath;
          switch (vsVersion)
          {
            case SupportedVsVersions.Vs2015:
              vsPath = Environment.Is64BitOperatingSystem ? VS2015_X64_EXTENSIONS_FILE_PATH : VS2015_X86_EXTENSIONS_FILE_PATH;
              vsVersionNumber = VS2015_VERSION_NUMBER;
              break;
            case SupportedVsVersions.Vs2017Community:
              vsPath = Environment.Is64BitOperatingSystem ? _VS2017CommunityX64ExtensionsFilePath : _VS2017CommunityX86ExtensionsFilePath;
              vsVersionNumber = VS2017_VERSION_NUMBER;
              break;
            case SupportedVsVersions.Vs2017Enterprise:
              vsPath = Environment.Is64BitOperatingSystem ? _VS2017EnterpriseX64ExtensionsFilePath : _VS2017EnterpriseX86ExtensionsFilePath;
              vsVersionNumber = VS2017_VERSION_NUMBER;
              break;
            case SupportedVsVersions.Vs2017Professional:
              vsPath = Environment.Is64BitOperatingSystem ? _VS2017ProfessionalX64ExtensionsFilePath : _VS2017ProfessionalX86ExtensionsFilePath;
              vsVersionNumber = VS2017_VERSION_NUMBER;
              break;
            case SupportedVsVersions.Vs2019Community:
              vsPath = Environment.Is64BitOperatingSystem ? _VS2019CommunityX64ExtensionsFilePath : _VS2019CommunityX86ExtensionsFilePath;
              vsVersionNumber = VS2019_VERSION_NUMBER;
              break;
            case SupportedVsVersions.Vs2019Enterprise:
              vsPath = Environment.Is64BitOperatingSystem ? _VS2019EnterpriseX64ExtensionsFilePath : _VS2019EnterpriseX86ExtensionsFilePath;
              vsVersionNumber = VS2019_VERSION_NUMBER;
              break;
            case SupportedVsVersions.Vs2019Professional:
              vsPath = Environment.Is64BitOperatingSystem ? _VS2019ProfessionalX64ExtensionsFilePath : _VS2019ProfessionalX86ExtensionsFilePath;
              vsVersionNumber = VS2019_VERSION_NUMBER;
              break;
            default:
              throw new Exception("Could not parse parameter VSVersion to a valid 'SupportedVSVersions' value.");
          }

          // Registry key handling
          string keyPath = GetRegKeyPath(vsVersionNumber);
          string keyName = "ShellFolder";
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
          vsRootPath = vsPath;
          string extensionsDirectory = string.Format(@"{0}{1}", vsPath, @"Common7\IDE\Extensions");
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

              //Remove leftover folders.
              if (vsVersion == SupportedVsVersions.Vs2017Community
                  || vsVersion == SupportedVsVersions.Vs2017Enterprise
                  || vsVersion == SupportedVsVersions.Vs2017Professional
                  || vsVersion == SupportedVsVersions.Vs2019Community
                  || vsVersion == SupportedVsVersions.Vs2019Enterprise
                  || vsVersion == SupportedVsVersions.Vs2019Professional)
              {
                DeleteEmptyFolders(vsRootPath);
              }
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
    /// Recursevily delete any empty folders which are childs of the parent folder.
    /// </summary>
    /// <param name="parentFolder">Parent folder.</param>
    private static void DeleteEmptyFolders(string parentFolder)
    {
      if (string.IsNullOrEmpty(parentFolder))
      {
        return;
      }

      foreach (var folder in Directory.GetDirectories(parentFolder))
      {
        DeleteEmptyFolders(folder);

        try
        {
          if (Directory.GetFiles(folder).Length == 0 && Directory.GetDirectories(folder).Length == 0)
          {
            Directory.Delete(folder, false);
          }
        }
        catch (Exception ex)
        {
          throw new Exception(string.Format("Error executing '" + nameof(DeleteEmptyFolders) + "'. {0}. {1}", ex.Message, ex.InnerException != null ? ex.InnerException.ToString() : string.Empty));
        }
      }
    }

    /// <summary>
    /// Generates the reg key path for the specified Visual Studio version.
    /// </summary>
    /// <param name="vsVersion">The Visual Studio version.</param>
    /// <returns>The key path for the specified Visual Studio version</returns>
    private static string GetRegKeyPath(string vsVersion)
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

      return keyPath;
    }

    /// <summary>
    /// Gets Visual Studio's setup configuration.
    /// </summary>
    /// <returns>An <see cref="ISetupConfiguration"/> instance with information about VS2017+ instances.</returns>
    private static ISetupConfiguration GetVSSetupConfiguration()
    {
      try
      {
        // Try to CoCreate the class object.
        return new SetupConfiguration();
      }
      catch (COMException ex) when (ex.HResult == REGDB_E_CLASSNOTREG)
      {
        // Try to get the class object using app-local call.
        ISetupConfiguration query;
        GetSetupConfiguration(out query, IntPtr.Zero);
        return query;
      }
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

    /// <summary>
    /// Sets the installation paths for all VS2017+ flavors.
    /// </summary>
    private static void SetVSInstallationPaths()
    {
      try
      {
        var setupConfiguration = GetVSSetupConfiguration();
        if (setupConfiguration == null)
        {
          return;
        }

        var setupInstances = ((ISetupConfiguration2)setupConfiguration).EnumAllInstances();
        var setupInstance = new ISetupInstance[1];
        int fetched;
        do
        {
          setupInstances.Next(1, setupInstance, out fetched);
          if (fetched > 0)
          {
            var vsInstance = (ISetupInstance2)setupInstance[0];
            var state = vsInstance.GetState();
            const string FLAVOR_COMMUNITY = "Microsoft.VisualStudio.Product.Community";
            const string FLAVOR_ENTERPRISE = "Microsoft.VisualStudio.Product.Enterprise";
            const string FLAVOR_PROFESSIONAL = "Microsoft.VisualStudio.Product.Professional";
            if ((state & InstanceState.Local) == InstanceState.Local)
            {
              //Determine the instance's flavor.
              var flavor = vsInstance.GetProduct().GetId();
              var version = vsInstance.GetInstallationVersion();

              if (version.StartsWith("15"))
              {
                if (flavor == FLAVOR_COMMUNITY && string.IsNullOrEmpty(_vs2017CommunityInstallationPath))
                {
                  _vs2017CommunityInstallationPath = vsInstance.GetInstallationPath();
                  _vs2017CommunityInstanceId = vsInstance.GetInstanceId();
                }

                if (flavor == FLAVOR_ENTERPRISE && string.IsNullOrEmpty(_vs2017EnterpriseInstallationPath))
                {
                  _vs2017EnterpriseInstallationPath = vsInstance.GetInstallationPath();
                  _vs2017EnterpriseInstanceId = vsInstance.GetInstanceId();
                }

                if (flavor == FLAVOR_PROFESSIONAL && string.IsNullOrEmpty(_vs2017ProfessionalInstallationPath))
                {
                  _vs2017ProfessionalInstallationPath = vsInstance.GetInstallationPath();
                  _vs2017ProfessionalInstanceId = vsInstance.GetInstanceId();
                }
              }
              else if (version.StartsWith("16"))
              {
                if (flavor == FLAVOR_COMMUNITY && string.IsNullOrEmpty(_vs2019CommunityInstallationPath))
                {
                  _vs2019CommunityInstallationPath = vsInstance.GetInstallationPath();
                  _vs2019CommunityInstanceId = vsInstance.GetInstanceId();
                }

                if (flavor == FLAVOR_ENTERPRISE && string.IsNullOrEmpty(_vs2019EnterpriseInstallationPath))
                {
                  _vs2019EnterpriseInstallationPath = vsInstance.GetInstallationPath();
                  _vs2019EnterpriseInstanceId = vsInstance.GetInstanceId();
                }

                if (flavor == FLAVOR_PROFESSIONAL && string.IsNullOrEmpty(_vs2019ProfessionalInstallationPath))
                {
                  _vs2019ProfessionalInstallationPath = vsInstance.GetInstallationPath();
                  _vs2019ProfessionalInstanceId = vsInstance.GetInstanceId();
                }
              }
            }
          }
         }
         while (fetched > 0);
       }
       catch (Exception) {}
    }

    #region External Methods
    /// <summary>
    /// External method included in the <see cref="Setup.Configuration.Native.dll"/> used to get VS2017+'s setup configuration.
    /// </summary>
    /// <param name="configuration"><see cref="ISetupConfiguration"/> instance to hold the return value of the method.</param>
    /// <param name="reserved">Pointer object.</param>
    /// <returns>An <see cref="ISetupConfiguration"/> instance via an out parameter.</returns>
    [DllImport("Microsoft.VisualStudio.Setup.Configuration.Native.dll", ExactSpelling = true, PreserveSig = true)]
    private static extern int GetSetupConfiguration([MarshalAs(UnmanagedType.Interface), Out] out ISetupConfiguration configuration,IntPtr reserved);
    #endregion
  }
}
