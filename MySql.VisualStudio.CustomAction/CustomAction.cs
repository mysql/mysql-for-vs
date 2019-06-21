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
using MySql.Utility.Classes.Logging;
using MySql.VisualStudio.CustomAction.Enums;
using MySql.VisualStudio.CustomAction.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;

namespace MySql.VisualStudio.CustomAction
{
  public class CustomActions
  {
    #region Constants

    private const string APPLICATION_NAME = "MySQL for Visual Studio";
    private const string BINDING_REDIRECT_ROOT_KEY = @"[$RootKey$\RuntimeConfiguration\dependentAssembly\bindingRedirection\{EE3E8305-3E91-51CD-0B2D-8E8EFFDD081C}]";
    private const string EXTENSION_FILE_NAME = "extensions.configurationchanged";
    private const int REGDB_E_CLASSNOTREG = unchecked((int)0x80040154);
    private const string VISUAL_STUDIO_2017_DEFAULT_INSTALLATION_PATH = @"C:\Program Files (x86)\Microsoft Visual Studio\2017\";

    private const string VS2012_VERSION_NUMBER = "11.0";
    private const string VS2013_VERSION_NUMBER = "12.0";
    private const string VS2015_VERSION_NUMBER = "14.0";
    private const string VS2017_VERSION_NUMBER = "15.0";

    private const string VS2012_X64_EXTENSIONS_FILE_PATH = @"C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\";
    private const string VS2012_X86_EXTENSIONS_FILE_PATH = @"C:\Program Files\Microsoft Visual Studio 11.0\Common7\IDE\";
    private const string VS2013_X64_EXTENSIONS_FILE_PATH = @"C:\Program Files (x86)\Microsoft Visual Studio 12.0\";
    private const string VS2013_X86_EXTENSIONS_FILE_PATH = @"C:\Program Files\Microsoft Visual Studio 12.0\";
    private const string VS2015_X64_EXTENSIONS_FILE_PATH = @"C:\Program Files (x86)\Microsoft Visual Studio 14.0\";
    private const string VS2015_X86_EXTENSIONS_FILE_PATH = @"C:\Program Files\Microsoft Visual Studio 14.0\";

    private const string VS2015_INSTALL_FEATURE = "VS2015Install";
    private const string VS2017_COMMUNITY_INSTALL_FEATURE = "VS2017ComInstall";
    private const string VS2017_ENTERPRISE_INSTALL_FEATURE = "VS2017EntInstall";
    private const string VS2017_PROFESSIONAL_INSTALL_FEATURE = "VS2017ProInstall";

    #endregion

    #region Fields

    private static Version _internalMySqlDataVersion = new Version("8.0.16.0");
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

    #endregion

    #region Properties

    /// <summary>
    /// The installation path for Visual Studio 2017 Community edition.
    /// </summary>
    public static string VS2017CommunityInstallationPath => _vs2017CommunityInstallationPath;

    /// <summary>
    /// The installation path for Visual Studio 2017 Enteprise edition.
    /// </summary>
    public static string VS2017EnterpriseInstallationPath => _vs2017EnterpriseInstallationPath;

    /// <summary>
    /// The installation path for Visual Studio 2017 Professional edition.
    /// </summary>
    public static string VS2017ProfessionalInstallationPath => _vs2017ProfessionalInstallationPath;

    #endregion

    /// <summary>
    /// Static constructor that initializes VS2017's paths for all VS2017 flavors.
    /// </summary>
    static CustomActions()
    {
      SetVS2017InstallationPaths();

      if (!string.IsNullOrEmpty(_vs2017CommunityInstallationPath))
      {
        _VS2017CommunityX64ExtensionsFilePath = _vs2017CommunityInstallationPath + @"\";
        _VS2017CommunityX86ExtensionsFilePath = _vs2017CommunityInstallationPath.Replace(" (86)","") + @"\";
      }
      else
      {
        var partialPath = $@"{VISUAL_STUDIO_2017_DEFAULT_INSTALLATION_PATH}Community\";
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
        var partialPath = $@"{VISUAL_STUDIO_2017_DEFAULT_INSTALLATION_PATH}Enterprise\";
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
        var partialPath = $@"{VISUAL_STUDIO_2017_DEFAULT_INSTALLATION_PATH}Professional\";
        _VS2017ProfessionalX64ExtensionsFilePath = partialPath;
        _VS2017ProfessionalX86ExtensionsFilePath = partialPath.Replace(" (86)","");
      }
    }

    #region Custom Actions

    [CustomAction]
    public static ActionResult UpdateFlagPackagesFileForVs2012(Session session)
    {
      if (session == null)
      {
        return ActionResult.Failure;
      }

      string vsPath = Path.Combine(session.CustomActionData["VS2012_PathProp"], @"Extensions\extensions.configurationchanged");

      if (File.Exists(vsPath))
      {
        File.WriteAllText(vsPath, string.Empty);
      }

      return ActionResult.Success;
    }

    [CustomAction]
    public static ActionResult UpdateFlagPackagesFileForVs2013(Session session)
    {
      if (session == null)
      {
        return ActionResult.Failure;
      }

      string vsPath = Path.Combine(session.CustomActionData["VS2013_PathProp"], @"Common7\IDE\Extensions\extensions.configurationchanged");

      if (File.Exists(vsPath))
      {
        File.WriteAllText(vsPath, string.Empty);
      }

      return ActionResult.Success;
    }

    [CustomAction]
    public static ActionResult UpdateFlagPackagesFileForVs2015(Session session)
    {
      if (session == null)
      {
        return ActionResult.Failure;
      }

      string vsPath = Path.Combine(session.CustomActionData["VS2015_PathProp"], @"Common7\IDE\Extensions\extensions.configurationchanged");

      if (File.Exists(vsPath))
      {
        File.WriteAllText(vsPath, string.Empty);
        session["VS2015INSTALL"] = "1";
      }

      return ActionResult.Success;
    }

    [CustomAction]
    public static ActionResult UpdateFlagPackagesFileForVs2017(Session session)
    {
      if (session == null)
      {
        return ActionResult.Failure;
      }

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
      if (session == null)
      {
        return ActionResult.Failure;
      }

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
      if (session == null)
      {
        return ActionResult.Failure;
      }

      string vsPath = Path.Combine(session.CustomActionData["VS2017_Pro_PathProp"], @"Common7\IDE\Extensions\extensions.configurationchanged");

      if (File.Exists(vsPath))
      {
        File.WriteAllText(vsPath, string.Empty);
      }

      return ActionResult.Success;
    }

    [CustomAction]
    public static ActionResult UpdateMachineConfigFile(Session session)
    {
      if (session == null)
      {
        return ActionResult.Failure;
      }

      var installedPath = MySql.Utility.Classes.Utilities.GetMySqlAppInstallLocation(APPLICATION_NAME);
      if (string.IsNullOrEmpty(installedPath))
      {
        session.Log(string.Format(Resources.ProductNotInstalled, "MySQL for Visual Studio"));
        return ActionResult.NotExecuted;
      }

      installedPath = Path.Combine(installedPath, @"Assemblies\MySql.Data.dll");
      if (!File.Exists(installedPath))
      {
        session.Log(Resources.MySqlDataNotFound);
        return ActionResult.NotExecuted;
      }

      Assembly a = Assembly.LoadFile(installedPath);
      Type customInstallerType = a.GetType("MySql.Data.MySqlClient.CustomInstaller");

      if (customInstallerType != null)
      {
        try
        {
          session.Log(Resources.InvokingMethodOnCustomInstaller);
          var method = customInstallerType.GetMethod("AddProviderToMachineConfig", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod);

          if (method != null)
          {
            method.Invoke(null, null);
          }
          else
          {
            session.Log(Resources.MethodNullInformation);
          }

          return ActionResult.Success;
        }
        catch (Exception ex)
        {
          session.Log(string.Format(Resources.CallingMethodError, ex.Message, ex.InnerException.Message));
          return ActionResult.NotExecuted;
        }
      }
      else
      {
        session.Log(string.Format(Resources.FailedToLoadAssembly, string.Empty));
        return ActionResult.NotExecuted;
      }
    }

    /// <summary>
    /// Identifies if Connector/NET is installed and its version number.
    /// </summary>
    /// <param name="session">The session object containing the parameters sent by Wix.</param>
    /// <returns>An <see cref="ActionResult"/> object set with a success status if no errors were found; otherwise, a failure status is reported.</returns>
    [CustomAction]
    public static ActionResult GetConnectorNETVersion(Session session)
    {
      if (session == null)
      {
        return ActionResult.Failure;
      }

      const string CONNECTOR_NET_INSTALLED_VARIABLE_NAME = "CNETINSTALLED";
      const string CONNECTOR_NET_VERSION_VARIABLE_NAME = "CNETVERSION";
      session.Log(string.Format(Resources.ExecutingCustomAction, nameof(GetConnectorNETVersion)));
      session[CONNECTOR_NET_INSTALLED_VARIABLE_NAME] = "No";
      session[CONNECTOR_NET_VERSION_VARIABLE_NAME] = "0";

      try
      {
        var installedPath = MySql.Utility.Classes.Utilities.GetMySqlAppInstallLocation("MySQL Connector/Net");
        if (string.IsNullOrEmpty(installedPath))
        {
          session.Log(string.Format(Resources.ProductNotInstalled, "Connector/NET"));
          return ActionResult.Success;
        }

        installedPath = Path.Combine(installedPath, @"Assemblies\v4.5.2\MySql.Data.dll");
        if (!File.Exists(installedPath))
        {
          session.Log(Resources.MySqlDataNotFound);
          return ActionResult.Success;
        }

        var assembly = Assembly.LoadFile(installedPath);
        if (assembly != null)
        {
          session.Log(string.Format(Resources.AssemblyFoundAt, "MySql.Data", installedPath));
          var version = assembly.GetName().Version;

          // Checks if an integrated version of Connector/NET is installed and needs to be removed first.
          if (version < new Version(6, 7, 4))
          {
            session[CONNECTOR_NET_INSTALLED_VARIABLE_NAME] = "Unsupported";
            return ActionResult.Success;
          }

          session[CONNECTOR_NET_INSTALLED_VARIABLE_NAME] = "Yes";
          session[CONNECTOR_NET_VERSION_VARIABLE_NAME] = version.ToString();
          session.Log($"Connector/NET version {session[CONNECTOR_NET_VERSION_VARIABLE_NAME]} is installed.");

          return ActionResult.Success;
        }
        else
        {
          session.Log(string.Format(Resources.FailedToLoadAssembly, "Connector/NET"));
        }

        return ActionResult.Success;
      }
      catch (Exception ex)
      {
        session.Log($"{Resources.ConnectorNETGeneralError}: {ex.Message}");
        return ActionResult.Failure;
      }
    }

    /// <summary>
    /// Sets the installation paths for all VS2017 flavors.
    /// </summary>
    /// <param name="session">The session object containing the parameters sent by Wix.</param>
    /// <returns>Always returns ActionResult.Success since its return value isn't relevant.</returns>
    [CustomAction]
    public static ActionResult SetVS2017InstallationPaths(Session session)
    {
      if (session == null)
      {
        return ActionResult.Failure;
      }

      session.Log(string.Format(Resources.ExecutingCustomAction, nameof(SetVS2017InstallationPaths)));

      if (!string.IsNullOrEmpty(_vs2017CommunityInstallationPath))
      {
        session["VS_2017_COM_PATH_MAIN"] = session["VS_2017_COM_PATH"] = _vs2017CommunityInstallationPath;
        session["VS2017COMINSTALL"] = "1";
      }

      if (!string.IsNullOrEmpty(_vs2017EnterpriseInstallationPath))
      {
        session["VS_2017_ENT_PATH_MAIN"] = session["VS_2017_ENT_PATH"] = _vs2017EnterpriseInstallationPath;
        session["VS2017ENTINSTALL"] = "1";
      }

      if (!string.IsNullOrEmpty(_vs2017ProfessionalInstallationPath))
      {
        session["VS_2017_PRO_PATH_MAIN"] = session["VS_2017_PRO_PATH"] = _vs2017ProfessionalInstallationPath;
        session["VS2017PROINSTALL"] = "1";
      }

      return ActionResult.Success;
    }

    /// <summary>
    /// Adds binding redirection entries to the PKGDEF file based on the version of Connector/NET installed (if any).
    /// </summary>
    /// <param name="session">The session object containing the parameters sent by Wix.</param>
    /// <returns>An <see cref="ActionResult"/> object set with a success status if no errors were found; otherwise, a failure status is reported.</returns>
    [CustomAction]
    public static ActionResult AddRedirectEntries(Session session)
    {
      if (session == null)
      {
        return ActionResult.Failure;
      }

      session.Log(string.Format(Resources.ExecutingCustomAction, nameof(AddRedirectEntries)));

      var customActionData = session.CustomActionData;
      var installedMySqlDataVersion = new Version(customActionData["ConnectorVersion"]);
      if (installedMySqlDataVersion <= new Version(0, 0))
      {
        session.Log(string.Format(Resources.ProductNotInstalled, "Connector/NET"));
        return ActionResult.Success;
      }

      var mysqlForVisualStudioVersion = customActionData["MySqlForVisualStudioVersion"];
      session.Log(string.Format(Resources.InstalledMySqlDataVersion, installedMySqlDataVersion.ToString()));
      session.Log(string.Format(Resources.InternalMySqlDataVersion, _internalMySqlDataVersion.ToString()));
      if (installedMySqlDataVersion == _internalMySqlDataVersion)
      {
        session.Log(Resources.MySqlDataVersionsMatch);
        return ActionResult.Success;
      }

      // Find affected Visual Studio versions.
      string[] features = { VS2015_INSTALL_FEATURE, VS2017_COMMUNITY_INSTALL_FEATURE, VS2017_ENTERPRISE_INSTALL_FEATURE, VS2017_PROFESSIONAL_INSTALL_FEATURE };
      foreach (var feature in features)
      {
        if (!UpdatePkgdefFileForVersion(session, feature, mysqlForVisualStudioVersion, installedMySqlDataVersion, _internalMySqlDataVersion))
        {
          return ActionResult.Failure;
        }
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
      if (session == null)
      {
        return ActionResult.Failure;
      }

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
      if (session == null)
      {
        return ActionResult.Failure;
      }

      return CreateDeleteRegKeyAndExtensionsFile(session, true) ? ActionResult.Success : ActionResult.Failure;
    }

    #endregion

    /// <summary>
    /// Displays a warning message for Win7 users if the devenv /updateconfiguration command failed to execute.
    /// </summary>
    /// <param name="session">The session object containing the parameters sent by Wix.</param>
    /// <returns>An <see cref="ActionResult.Success"> object since the results of the code executed in this method are irrelevant to the overall installation process."</returns>
    [CustomAction]
    public static ActionResult ShowInstallationWarning(Session session)
    {
      if (session == null)
      {
        return ActionResult.Failure;
      }

      bool showWarning = false;

      // Read the ActivityLog from the user's AppData folder.
      if (!string.IsNullOrEmpty(_vs2017CommunityInstallationPath))
      {
        showWarning = ReadActivityLog(_vs2017CommunityInstanceId, session["AppDataFolder"], session);
      }
      else if (!string.IsNullOrEmpty(_vs2017EnterpriseInstallationPath))
      {
        showWarning = ReadActivityLog(_vs2017EnterpriseInstanceId, session["AppDataFolder"], session);
      }
      else if (!string.IsNullOrEmpty(_vs2017ProfessionalInstallationPath))
      {
        showWarning = ReadActivityLog(_vs2017ProfessionalInstanceId, session["AppDataFolder"], session);
      }

      if (showWarning)
      {
        session.Message(InstallMessage.Warning, new Record { FormatString = Resources.FailedToRefreshExtensions });
      }

      return ActionResult.Success;
    }

    /// <summary>
    /// Reads the ActivityLog of the specified VS2017 instance.
    /// </summary>
    /// <param name="vs2017InstanceId">The instance id.</param>
    /// <param name="appDataFolder">The path to the user's app data folder.</param>
    /// <returns><c>True</c> if the warning message should be displayed or if an error prevented from reading the ActivityLog; otherwise, <c>false</c>.</returns>
    private static bool ReadActivityLog(string vs2017InstanceId, string appDataFolder, Session session)
    {
      try
      {
        var activityLog = string.Format("{0}Microsoft\\VisualStudio\\15.0_{1}\\ActivityLog.xml", appDataFolder, vs2017InstanceId);
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
              session.Log(string.Format(Resources.ActivityLogAccessDeniedError, vs2017InstanceId));
              return true;
            }
          }
        }
        else return true;

        return false;
      }
      catch (Exception ex)
      {
        session.Log(string.Format(Resources.FailedToReadActivityLog, ex.Message));
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

        SupportedVisualStudioVersions vsVersion;
        if (Enum.TryParse(sVsVersion, true, out vsVersion))
        {
          string vsVersionNumber;
          string vsPath;
          string vsRootPath;
          switch (vsVersion)
          {
            case SupportedVisualStudioVersions.Vs2012:
              vsPath = Environment.Is64BitOperatingSystem ? VS2012_X64_EXTENSIONS_FILE_PATH : VS2012_X86_EXTENSIONS_FILE_PATH;
              vsVersionNumber = VS2012_VERSION_NUMBER;
              break;
            case SupportedVisualStudioVersions.Vs2013:
              vsPath = Environment.Is64BitOperatingSystem ? VS2013_X64_EXTENSIONS_FILE_PATH : VS2013_X86_EXTENSIONS_FILE_PATH;
              vsVersionNumber = VS2013_VERSION_NUMBER;
              break;
            case SupportedVisualStudioVersions.Vs2015:
              vsPath = Environment.Is64BitOperatingSystem ? VS2015_X64_EXTENSIONS_FILE_PATH : VS2015_X86_EXTENSIONS_FILE_PATH;
              vsVersionNumber = VS2015_VERSION_NUMBER;
              break;
            case SupportedVisualStudioVersions.Vs2017Community:
              vsPath = Environment.Is64BitOperatingSystem ? _VS2017CommunityX64ExtensionsFilePath : _VS2017CommunityX86ExtensionsFilePath;
              vsVersionNumber = VS2017_VERSION_NUMBER;
              break;
            case SupportedVisualStudioVersions.Vs2017Enterprise:
              vsPath = Environment.Is64BitOperatingSystem ? _VS2017EnterpriseX64ExtensionsFilePath : _VS2017EnterpriseX86ExtensionsFilePath;
              vsVersionNumber = VS2017_VERSION_NUMBER;
              break;
            case SupportedVisualStudioVersions.Vs2017Professional:
              vsPath = Environment.Is64BitOperatingSystem ? _VS2017ProfessionalX64ExtensionsFilePath : _VS2017ProfessionalX86ExtensionsFilePath;
              vsVersionNumber = VS2017_VERSION_NUMBER;
              break;
            default:
              throw new Exception(Resources.FailedToParseVSVersion);
          }

          // Registry key handling.
          string keyPath = GetRegKeyPath(vsVersionNumber, vsVersion == SupportedVisualStudioVersions.Vs2012);
          string keyName = vsVersion == SupportedVisualStudioVersions.Vs2012 ? "EnvironmentDirectory" : "ShellFolder";
          var key = Registry.LocalMachine.OpenSubKey(keyPath, true);
          if (!isDeleting)
          {
            session.Log(string.Format(Resources.CreatingRegistryKey, keyPath, keyName));
            if (key == null)
            {
              key = Registry.LocalMachine.CreateSubKey(keyPath, RegistryKeyPermissionCheck.ReadWriteSubTree);
              if (key != null)
              {
                key.SetValue(keyName, vsPath, RegistryValueKind.String);
                session.Log(string.Format(Resources.CreatedRegistryKey, keyPath, keyName));
              }
            }
            else
            {
              var keyValue = key.GetValue(keyName);
              if (keyValue == null)
              {
                key.SetValue(keyName, vsPath, RegistryValueKind.String);
                session.Log(string.Format(Resources.CreatedRegistryKey, keyPath, keyName));
              }
              key.Close();
            }
          }
          else
          {
            session.Log(string.Format(Resources.DeletingRegistryKey, keyPath, keyName));
            if (key != null)
            {
              var keyValue = key.GetValue(keyName);
              if (keyValue != null)
              {
                key.DeleteValue(keyName);
                session.Log(string.Format(Resources.DeletedRegistryKey, keyPath, keyName));
              }
              key.Close();
            }
          }

          // "extensions.configurationchanged" file handling.
          vsRootPath = vsPath;
          string extensionsDirectory = string.Format(@"{0}{1}",
            vsPath,
            vsVersion == SupportedVisualStudioVersions.Vs2012
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
              session.Log(string.Format(Resources.ExtensionsFileCreated, vsVersion));

              // Remove leftover folders.
              if (vsVersion == SupportedVisualStudioVersions.Vs2017Community || vsVersion == SupportedVisualStudioVersions.Vs2017Enterprise || vsVersion == SupportedVisualStudioVersions.Vs2017Professional)
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
              session.Log(string.Format(Resources.ExtensionsFileDeleted, vsVersion));
            }
          }

          return true;
        }
        else
        {
          session.Log(Resources.FailedToParseVSVersion);
          return false;
        }
      }
      catch (Exception ex)
      {
        session.Log(string.Format(Resources.FailedToExecuteCustomAction, nameof(CreateDeleteRegKeyAndExtensionsFile), ex.Message, ex.InnerException != null ? ex.InnerException.ToString() : string.Empty));
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
          throw new Exception(string.Format(Resources.FailedToExecuteCustomAction, nameof(DeleteEmptyFolders), ex.Message, ex.InnerException != null ? ex.InnerException.ToString() : string.Empty));
        }
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
      if (string.IsNullOrEmpty(vsVersion))
      {
        return null;
      }

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
    /// Gets VS2017's setup configuration.
    /// </summary>
    /// <returns>An <see cref="ISetupConfiguration"/> instance with information about VS2017's instances.</returns>
    private static ISetupConfiguration GetVS2017SetupConfiguration()
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
    private static void SetSessionValue(Session session, string sessionName, string value)
    {
      session.Log(string.Format(Resources.SetSessionVariableValue, sessionName, value));
      session[sessionName] = value;
    }

    /// <summary>
    /// Sets the installation paths for all VS2017 flavors.
    /// </summary>
    public static void SetVS2017InstallationPaths()
    {
      try
      {
        var setupConfiguration = GetVS2017SetupConfiguration();
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
            if ((state & InstanceState.Local) == InstanceState.Local)
            {
              //Determine the instance's flavor.
              var flavor = vsInstance.GetProduct().GetId();

              if (flavor == "Microsoft.VisualStudio.Product.Community" && string.IsNullOrEmpty(_vs2017CommunityInstallationPath))
              {
                _vs2017CommunityInstallationPath = vsInstance.GetInstallationPath();
                _vs2017CommunityInstanceId = vsInstance.GetInstanceId();
              }

              if (flavor == "Microsoft.VisualStudio.Product.Enterprise" && string.IsNullOrEmpty(_vs2017EnterpriseInstallationPath))
              {
                _vs2017EnterpriseInstallationPath = vsInstance.GetInstallationPath();
                _vs2017EnterpriseInstanceId = vsInstance.GetInstanceId();
              }

              if (flavor == "Microsoft.VisualStudio.Product.Professional" && string.IsNullOrEmpty(_vs2017ProfessionalInstallationPath))
              {
                _vs2017ProfessionalInstallationPath = vsInstance.GetInstallationPath();
                _vs2017ProfessionalInstanceId = vsInstance.GetInstanceId();
              }
            }
          }
         }
         while (fetched > 0);
       }
       catch (Exception) {}
    }

    /// <summary>
    /// Updates the the PKGDEF file for the specified Visual Studio version.
    /// </summary>
    /// <param name="session">The session object to interact with the installer variables.</param>
    /// <param name="feature">The MSI user selected feature.</param>
    /// <param name="mysqlForVisualStudioVersion">The version number of MySQL for Visual Studio.</param>
    /// <param name="installedMySqlDataVersion">The version number of the MySql.Data library found in the Connector/NET installation.</param>
    /// <param name="internalMySqlDataVersion">The version number of the MySql.Data included in this MySQL for Visual Studio installlation.</param>
    private static bool UpdatePkgdefFileForVersion(Session session, string feature, string mysqlForVisualStudioVersion, Version installedMySqlDataVersion, Version internalMySqlDataVersion)
    {
      session.Log(string.Format(Resources.UpdatingPkgdefFileForFeature, feature));
      if (!string.Equals(session.CustomActionData[feature], "1"))
      {
        session.Log(Resources.VisualStudioVersionNotSelected);
        return true;
      }

      string visualStudioInstallationPath = null;
      switch (feature)
      {
        case VS2015_INSTALL_FEATURE:
          visualStudioInstallationPath = session.CustomActionData["VS2015Path"];
          break;

        case VS2017_COMMUNITY_INSTALL_FEATURE:
          visualStudioInstallationPath = session.CustomActionData["VS2017ComPath"];
          break;

        case VS2017_ENTERPRISE_INSTALL_FEATURE:
          visualStudioInstallationPath = session.CustomActionData["VS2017EntPath"];
          break;

        case VS2017_PROFESSIONAL_INSTALL_FEATURE:
          visualStudioInstallationPath = session.CustomActionData["VS2017ProPath"];
          break;

        default:
          session.Log(string.Format(Resources.FeatureNotSupported, feature));
          return false;
      }

      if (!string.IsNullOrEmpty(visualStudioInstallationPath))
      {
        session.Log(string.Format(Resources.ProductInstallationPath, "Visual Studio", visualStudioInstallationPath));
        if (UpdatePkgdefFile(visualStudioInstallationPath, new Version(mysqlForVisualStudioVersion), installedMySqlDataVersion, internalMySqlDataVersion, out var logMessage, Encoding.Unicode))
        {
          session.Log(logMessage);
        }
        else
        {
          session.Log(string.Format(Resources.PkgdefFileUpdateFailedForFeature, feature, logMessage));
        }
      }
      else
      {
        session.Log(Resources.VisualStudioInstallationPathNotFound);
        return false;
      }

      return true;
    }

    /// <summary>
    /// Updated the PKGDEF file located in the MySQL for Visual Studio extensions folder for the specified Visual Studio version.
    /// </summary>
    /// <param name="visualStudioInstallationPath">The installation path for the Visual Studio version where the PKGDEF file exists.</param>
    /// <param name="mysqlForVisualStudioVersion">The version number of MySQL for Visual Studio.</param>
    /// <param name="installedMySqlDataVersion">The version number of the MySql.Data library found in the Connector/NET installation.</param>
    /// <param name="internalMySqlDataVersion">The version number of the MySql.Data included in this MySQL for Visual Studio installlation.</param>
    /// <param name="encoding">The encoding to use when updating the PKGDEF file.</param>
    /// <returns></returns>
    public static bool UpdatePkgdefFile(string visualStudioInstallationPath, Version mySqlForVisualStudioVersion, Version installedMySqlDataVersion, Version internalMySqlDataVersion, out string logData, Encoding encoding = null)
    {
      if (string.IsNullOrEmpty(visualStudioInstallationPath) || mySqlForVisualStudioVersion == null || internalMySqlDataVersion == null)
      {
        logData = "Invalid parameters.";
        return false;
      }

      var logBuilder = new StringBuilder();
      var pkgdefFilePath = Utility.Utilities.GetPkgdefFilePath(visualStudioInstallationPath, mySqlForVisualStudioVersion);
      if (!File.Exists(pkgdefFilePath))
      {
        logBuilder.AppendLine(Resources.MySQLForVisualStudioNotInstalledNoUpdateRequired);
        logData = logBuilder.ToString();
        return true;
      }

      if (installedMySqlDataVersion != null && installedMySqlDataVersion == internalMySqlDataVersion)
      {
        logBuilder.AppendLine(Resources.MySqlDataVersionsMatchNoUpdateRequired);
        logData = logBuilder.ToString();
        return true;
      }

      try
      {
        // Remove any existing binding redirect entries in the PKGDEF file.
        var builder = new StringBuilder();
        using (var reader = new StreamReader(pkgdefFilePath))
        {
          string line;
          bool omitLine = false;
          while ((line = reader.ReadLine()) != null)
          {
            if (!omitLine && line.Equals(BINDING_REDIRECT_ROOT_KEY))
            {
              omitLine = true;
              continue;
            }
            else if (omitLine && line.StartsWith("[$RootKey$"))
            {
              omitLine = false;
            }
            else if (omitLine)
            {
              continue;
            }

            builder.AppendLine(line);
          }
        }

        // Update PKGDEF file without including binding redirect entries added by MySQL for Visual Studio.
        if (!Utility.Utilities.WriteAllText(pkgdefFilePath, false, builder.ToString(), encoding))
        {
          logBuilder.Append(Resources.FailedToWriteToPkgdefFile);
          logData = logBuilder.ToString();
          return false;
        }

        logBuilder.AppendLine(Resources.BindingRedirectEntriesRemoved);

        // If Connector/NET is not installed then removing binding redirect entries should suffice.
        if (installedMySqlDataVersion == null)
        {
          logBuilder.Append(Resources.PkgdefFileUpdated);
          logData = logBuilder.ToString();
          return true;
        }

        // Update PKGDEF file with binding redirect updated entries.
        string bindingRedirectionEntry = $"{Environment.NewLine}{Environment.NewLine}{BINDING_REDIRECT_ROOT_KEY}{Environment.NewLine}\"Name\" = \"MySql.Data\"{Environment.NewLine}\"PublicKeyToken\" = \"c5687fc88969c44d\"{Environment.NewLine}\"Culture\" = \"neutral\"{Environment.NewLine}";
        if (installedMySqlDataVersion < internalMySqlDataVersion)
        {
          if (!Utility.Utilities.WriteAllText(pkgdefFilePath, true, $"{bindingRedirectionEntry}\"OldVersion\" = \"6.7.4.0 - {internalMySqlDataVersion.Major}.{internalMySqlDataVersion.Minor}.{internalMySqlDataVersion.Build - 1}.{internalMySqlDataVersion.Revision}\"{Environment.NewLine}\"NewVersion\" = \"{internalMySqlDataVersion.ToString()}\"", encoding))
          {
            logBuilder.Append(Resources.FailedToWriteToPkgdefFile);
            logData = logBuilder.ToString();
            return false;
          }
        }
        else if (installedMySqlDataVersion > internalMySqlDataVersion)
        {
          if (!Utility.Utilities.WriteAllText(pkgdefFilePath, true, bindingRedirectionEntry + $"\"OldVersion\" = \"{internalMySqlDataVersion.ToString()}\"" + Environment.NewLine + $"\"NewVersion\" = \"{installedMySqlDataVersion.ToString()}\"", encoding))
          {
            logBuilder.Append(Resources.FailedToWriteToPkgdefFile);
            logData = logBuilder.ToString();
            return false;
          }
        }

        logBuilder.AppendLine(Resources.PkgdefFileUpdated);
      }
      catch (Exception exception)
      {
        logBuilder.AppendLine(exception.Message);
        logData = logBuilder.ToString();
        return false;
      }

      logData = logBuilder.ToString();
      return true;
    }

    /// <summary>
    /// Identifies the type of binding redirect entry (if any) found in the PKGDEF file for the specified MySQL for Visual Studio installation. 
    /// </summary>
    /// <param name="visualStudioVersion">The version of Visual Studio where the PKGDEF file will be searched for.</param>
    /// <param name="mysqlForVisualStudioVersion">The version number of the installed MySQL for Visual Studio product.</param>
    /// <returns>The status of the PKGDEF file.</returns>
    private static PkgdefFileStatus GetPkgdefFileStatus(SupportedVisualStudioVersions visualStudioVersion, Version mysqlForVisualStudioVersion)
    {
      string visualStudioInstallationPath = null;
      if (visualStudioVersion > SupportedVisualStudioVersions.Vs2015)
      {
        SetVS2017InstallationPaths();
      }

      switch (visualStudioVersion)
      {
        case SupportedVisualStudioVersions.Vs2015:
          visualStudioInstallationPath = Utility.Utilities.GetVisualStudio2015InstallationPath();
          break;

        case SupportedVisualStudioVersions.Vs2017Community:
          visualStudioInstallationPath = _vs2017CommunityInstallationPath;
          break;

        case SupportedVisualStudioVersions.Vs2017Enterprise:
          visualStudioInstallationPath = _vs2017EnterpriseInstallationPath;
          break;

        case SupportedVisualStudioVersions.Vs2017Professional:
          visualStudioInstallationPath = _vs2017ProfessionalInstallationPath;
          break;

        default:
          throw new NotSupportedException(Resources.VisualStudioVersionNotSupported);
      }

      if (string.IsNullOrEmpty(visualStudioInstallationPath))
      {
        return PkgdefFileStatus.Unknown;
      }

      var pkgdefFilePath = Utility.Utilities.GetPkgdefFilePath(visualStudioInstallationPath, mysqlForVisualStudioVersion);
      if (!File.Exists(pkgdefFilePath))
      {
        return PkgdefFileStatus.Unknown;
      }

      try
      {
        using (var reader = new StreamReader(pkgdefFilePath))
        {
          string line;
          bool lookForVersionEntries = false;
          while ((line = reader.ReadLine()) != null)
          {
            if (lookForVersionEntries)
            {
              if (line.StartsWith("\"OldVersion"))
              {
                var versionString = Utility.Utilities.SanitizePkgdegFileVersionEntry(line);
                if (string.IsNullOrEmpty(versionString))
                {
                  Logger.LogError(string.Format(Resources.FailedToParseOldVersionEntry, line, visualStudioVersion.GetDescription()));
                  return PkgdefFileStatus.Unknown;
                }

                if (!Version.TryParse(versionString, out var version))
                {
                  return PkgdefFileStatus.RedirectFromOlderToInternalMySqlDataEntry;
                }

                if (_internalMySqlDataVersion != version)
                {
                  Logger.LogError(string.Format(Resources.IncorrectReferenceInOldVersionEntry, _internalMySqlDataVersion.ToString()));
                  return PkgdefFileStatus.Unknown;
                }

                return PkgdefFileStatus.RedirectFromInternalToInstalledMySqlDataEntry;
              }

              continue;
            }
            else if (line.StartsWith(BINDING_REDIRECT_ROOT_KEY))
            {
              lookForVersionEntries = true;
            }
          }

          return PkgdefFileStatus.NoBindingRedirectEntries;
        }
      }
      catch (Exception)
      {
        Logger.LogError(string.Format(Resources.FailedToReadThePkgdefFile, visualStudioVersion.GetDescription(), pkgdefFilePath));
        return PkgdefFileStatus.Unknown;
      }
    }

    /// <summary>
    /// Gets the status for all supported versions of Visual Studio.
    /// </summary>
    /// <param name="mysqlForVisualStudioVersion">The version number of the installed MySQL for Visual Studio product.</param>
    /// <returns>A tuple list with the status of the PKGDEF files where MySQL for Visual Studio is installed.</returns>
    public static List<Tuple<SupportedVisualStudioVersions, PkgdefFileStatus>> GetPkgdefFileStatuses(Version mysqlForVisualStudioVersion)
    {
      var list = new List<Tuple<SupportedVisualStudioVersions, PkgdefFileStatus>>();
      list.Add(new Tuple<SupportedVisualStudioVersions, PkgdefFileStatus>(SupportedVisualStudioVersions.Vs2015, GetPkgdefFileStatus(SupportedVisualStudioVersions.Vs2015, mysqlForVisualStudioVersion)));
      list.Add(new Tuple<SupportedVisualStudioVersions, PkgdefFileStatus>(SupportedVisualStudioVersions.Vs2017Community, GetPkgdefFileStatus(SupportedVisualStudioVersions.Vs2017Community, mysqlForVisualStudioVersion)));
      list.Add(new Tuple<SupportedVisualStudioVersions, PkgdefFileStatus>(SupportedVisualStudioVersions.Vs2017Enterprise, GetPkgdefFileStatus(SupportedVisualStudioVersions.Vs2017Enterprise, mysqlForVisualStudioVersion)));
      list.Add(new Tuple<SupportedVisualStudioVersions, PkgdefFileStatus>(SupportedVisualStudioVersions.Vs2017Professional, GetPkgdefFileStatus(SupportedVisualStudioVersions.Vs2017Professional, mysqlForVisualStudioVersion)));

      return list;
    }

    /// <summary>
    /// Checks if any of the PKGDEF files need to be updated.
    /// </summary>
    public static bool IsConfigurationUpdateRequired(Version mysqlForVisualStudioVersion, Version installedMySqlDataVersion, Version internalMySqlDataVersion)
    {
      if (mysqlForVisualStudioVersion== null || internalMySqlDataVersion == null)
      {
        return false;
      }

      // Get PKGDEF file status.  
      var pkgdefFileStatuses = GetPkgdefFileStatuses(mysqlForVisualStudioVersion);

      // Connector/NET is not installed.
      if (installedMySqlDataVersion == null)
      {
        // PKGDEF files have no binding redirect entries.
        if (pkgdefFileStatuses.Any(o => o.Item2 == PkgdefFileStatus.RedirectFromInternalToInstalledMySqlDataEntry
                                                   || o.Item2 == PkgdefFileStatus.RedirectFromOlderToInternalMySqlDataEntry))
        {
          return true;
        }

        return false;
      }

      // Connector/NET is installed.
      if ((internalMySqlDataVersion == installedMySqlDataVersion
          && pkgdefFileStatuses.Any(o => o.Item2 != PkgdefFileStatus.NoBindingRedirectEntries))
          ||
          (internalMySqlDataVersion > installedMySqlDataVersion
           && pkgdefFileStatuses.Any(o => o.Item2 == PkgdefFileStatus.RedirectFromInternalToInstalledMySqlDataEntry
                                          || o.Item2 == PkgdefFileStatus.NoBindingRedirectEntries))
          ||
          (internalMySqlDataVersion < installedMySqlDataVersion
           && pkgdefFileStatuses.Any(o => o.Item2 == PkgdefFileStatus.RedirectFromOlderToInternalMySqlDataEntry
                                          || o.Item2 == PkgdefFileStatus.NoBindingRedirectEntries)))
      {
        return true;
      }

      return false;
    }

    #region External Methods
    /// <summary>
    /// External method included in the <see cref="Setup.Configuration.Native.dll"/> used to get VS2017's setup configuration.
    /// </summary>
    /// <param name="configuration"><see cref="ISetupConfiguration"/> instance to hold the return value of the method.</param>
    /// <param name="reserved">Pointer object.</param>
    /// <returns>An <see cref="ISetupConfiguration"/> instance via an out parameter.</returns>
    [DllImport("Microsoft.VisualStudio.Setup.Configuration.Native.dll", ExactSpelling = true, PreserveSig = true)]
    private static extern int GetSetupConfiguration([MarshalAs(UnmanagedType.Interface), Out] out ISetupConfiguration configuration,IntPtr reserved);
#endregion
  }
}
