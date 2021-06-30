// Copyright (c) 2014, 2021, Oracle and/or its affiliates.
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
    private const string VISUAL_STUDIO_2019_DEFAULT_INSTALLATION_PATH = @"C:\Program Files (x86)\Microsoft Visual Studio\2019\";
    private const string VS2017_VERSION_NUMBER = "15.0";
    private const string VS2019_VERSION_NUMBER = "16.0";

    private const string VS2017_COMMUNITY_INSTALL_FEATURE = "VS2017ComInstall";
    private const string VS2017_ENTERPRISE_INSTALL_FEATURE = "VS2017EntInstall";
    private const string VS2017_PROFESSIONAL_INSTALL_FEATURE = "VS2017ProInstall";
    private const string VS2019_COMMUNITY_INSTALL_FEATURE = "VS2019ComInstall";
    private const string VS2019_ENTERPRISE_INSTALL_FEATURE = "VS2019EntInstall";
    private const string VS2019_PROFESSIONAL_INSTALL_FEATURE = "VS2019ProInstall";

    #endregion

    #region Fields

    /// <summary>
    /// The app data path for this application.
    /// </summary>
    private static string _appDataPath;

    private static Version _internalMySqlDataVersion = new Version("8.0.24.0");
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

    #region Properties

    /// <summary>
    /// Gets the path for this application relative to the application data folder of the user where settings can be saved.
    /// </summary>
    internal static string AppDataPath
    {
      get
      {
        if (string.IsNullOrEmpty(_appDataPath))
        {
          _appDataPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}\\MySQL\\{APPLICATION_NAME}\\";
        }

        return _appDataPath;
      }
    }

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

    /// <summary>
    /// The installation path for Visual Studio 2019 Community edition.
    /// </summary>
    public static string VS2019CommunityInstallationPath => _vs2019CommunityInstallationPath;

    /// <summary>
    /// The installation path for Visual Studio 2019 Enteprise edition.
    /// </summary>
    public static string VS2019EnterpriseInstallationPath => _vs2019EnterpriseInstallationPath;

    /// <summary>
    /// The installation path for Visual Studio 2019 Professional edition.
    /// </summary>
    public static string VS2019ProfessionalInstallationPath => _vs2019ProfessionalInstallationPath;

    #endregion

    /// <summary>
    /// Static constructor that initializes VS2017/2019's paths for all VS2017/2019 flavors.
    /// </summary>
    static CustomActions()
    {
      Logger.Initialize(AppDataPath.Substring(0, AppDataPath.Length - 1), APPLICATION_NAME, false, false, APPLICATION_NAME);
      SetVSInstallationPaths();

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

      if (!string.IsNullOrEmpty(_vs2019CommunityInstallationPath))
      {
        _VS2019CommunityX64ExtensionsFilePath = _vs2019CommunityInstallationPath + @"\";
        _VS2019CommunityX86ExtensionsFilePath = _vs2019CommunityInstallationPath.Replace(" (86)","") + @"\";
      }
      else
      {
        var partialPath = $@"{VISUAL_STUDIO_2019_DEFAULT_INSTALLATION_PATH}Community\";
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
        var partialPath = $@"{VISUAL_STUDIO_2019_DEFAULT_INSTALLATION_PATH}Enterprise\";
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
        var partialPath = $@"{VISUAL_STUDIO_2019_DEFAULT_INSTALLATION_PATH}Professional\";
        _VS2019ProfessionalX64ExtensionsFilePath = partialPath;
        _VS2019ProfessionalX86ExtensionsFilePath = partialPath.Replace(" (86)","");
      }
    }

    #region Custom Actions

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
    public static ActionResult UpdateFlagPackagesFileForVs2019(Session session)
    {
      if (session == null)
      {
        return ActionResult.Failure;
      }

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
      if (session == null)
      {
        return ActionResult.Failure;
      }

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
      if (session == null)
      {
        return ActionResult.Failure;
      }

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
    /// Sets the installation paths for all VS2017+ flavors.
    /// </summary>
    /// <param name="session">The session object containing the parameters sent by Wix.</param>
    /// <returns>Always returns ActionResult.Success since its return value isn't relevant.</returns>
    [CustomAction]
    public static ActionResult SetVSInstallationPaths(Session session)
    {
      if (session == null)
      {
        return ActionResult.Failure;
      }

      session.Log(string.Format(Resources.ExecutingCustomAction, nameof(SetVSInstallationPaths)));

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

      if (!string.IsNullOrEmpty(_vs2019CommunityInstallationPath))
      {
        session["VS_2019_COM_PATH_MAIN"] = session["VS_2019_COM_PATH"] = _vs2019CommunityInstallationPath;
        session["VS2019COMINSTALL"] = "1";
      }

      if (!string.IsNullOrEmpty(_vs2019EnterpriseInstallationPath))
      {
        session["VS_2019_ENT_PATH_MAIN"] = session["VS_2019_ENT_PATH"] = _vs2019EnterpriseInstallationPath;
        session["VS2019ENTINSTALL"] = "1";
      }

      if (!string.IsNullOrEmpty(_vs2019ProfessionalInstallationPath))
      {
        session["VS_2019_PRO_PATH_MAIN"] = session["VS_2019_PRO_PATH"] = _vs2019ProfessionalInstallationPath;
        session["VS2019PROINSTALL"] = "1";
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
      string[] features =
      {
        VS2017_COMMUNITY_INSTALL_FEATURE,
        VS2017_ENTERPRISE_INSTALL_FEATURE,
        VS2017_PROFESSIONAL_INSTALL_FEATURE,
        VS2019_COMMUNITY_INSTALL_FEATURE,
        VS2019_ENTERPRISE_INSTALL_FEATURE,
        VS2019_PROFESSIONAL_INSTALL_FEATURE
      };
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
      session.Log(string.Format(Resources.ExecutingCustomAction, nameof(CreateRegKeyAndExtensionsFile)));
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
    public static ActionResult ShowWindows7InstallationWarning(Session session)
    {
      if (session == null)
      {
        return ActionResult.Failure;
      }

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
              session.Log(string.Format(Resources.ActivityLogAccessDeniedError, vsInstanceId));
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
      session.Log(string.Format(Resources.ExecutingCustomAction, nameof(CreateDeleteRegKeyAndExtensionsFile)));
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
          string registryKey = string.Empty;
          switch (vsVersion)
          {
            case SupportedVisualStudioVersions.Vs2017Community:
              vsPath = Environment.Is64BitOperatingSystem ? _VS2017CommunityX64ExtensionsFilePath : _VS2017CommunityX86ExtensionsFilePath;
              vsVersionNumber = VS2017_VERSION_NUMBER;
              registryKey = "VS2017_REGISTRYFIX_CREATED";
              break;
            case SupportedVisualStudioVersions.Vs2017Enterprise:
              vsPath = Environment.Is64BitOperatingSystem ? _VS2017EnterpriseX64ExtensionsFilePath : _VS2017EnterpriseX86ExtensionsFilePath;
              vsVersionNumber = VS2017_VERSION_NUMBER;
              registryKey = "VS2017_ENT_REGISTRYFIX_CREATED";
              break;
            case SupportedVisualStudioVersions.Vs2017Professional:
              vsPath = Environment.Is64BitOperatingSystem ? _VS2017ProfessionalX64ExtensionsFilePath : _VS2017ProfessionalX86ExtensionsFilePath;
              vsVersionNumber = VS2017_VERSION_NUMBER;
              registryKey = "VS2017_PRO_REGISTRYFIX_CREATED";
              break;
            case SupportedVisualStudioVersions.Vs2019Community:
              vsPath = Environment.Is64BitOperatingSystem ? _VS2019CommunityX64ExtensionsFilePath : _VS2019CommunityX86ExtensionsFilePath;
              vsVersionNumber = VS2019_VERSION_NUMBER;
              registryKey = "VS2019_REGISTRYFIX_CREATED";
              break;
            case SupportedVisualStudioVersions.Vs2019Enterprise:
              vsPath = Environment.Is64BitOperatingSystem ? _VS2019EnterpriseX64ExtensionsFilePath : _VS2019EnterpriseX86ExtensionsFilePath;
              vsVersionNumber = VS2019_VERSION_NUMBER;
              registryKey = "VS2019_ENT_REGISTRYFIX_CREATED";
              break;
            case SupportedVisualStudioVersions.Vs2019Professional:
              vsPath = Environment.Is64BitOperatingSystem ? _VS2019ProfessionalX64ExtensionsFilePath : _VS2019ProfessionalX86ExtensionsFilePath;
              vsVersionNumber = VS2019_VERSION_NUMBER;
              registryKey = "VS2019_PRO_REGISTRYFIX_CREATED";
              break;
            default:
              throw new Exception(Resources.FailedToParseVSVersion);
          }

          // Registry key handling.
          string keyPath = GetRegKeyPath(vsVersionNumber);
          string keyName = "ShellFolder";
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
                session.CustomActionData[registryKey] = "1";
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
            if (session.CustomActionData[registryKey] == "0")
            {
              session.Log(string.Format(Resources.RegistryKeyDeleteSkipped, $@"{keyPath}\{keyName}"));
              return true;
            }

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
              session.Log(string.Format(Resources.ExtensionsFileCreated, vsVersion));

              //Remove leftover folders.
              if (vsVersion == SupportedVisualStudioVersions.Vs2017Community
                  || vsVersion == SupportedVisualStudioVersions.Vs2017Enterprise
                  || vsVersion == SupportedVisualStudioVersions.Vs2017Professional
                  || vsVersion == SupportedVisualStudioVersions.Vs2019Community
                  || vsVersion == SupportedVisualStudioVersions.Vs2019Enterprise
                  || vsVersion == SupportedVisualStudioVersions.Vs2019Professional)
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
    /// <returns>The key path for the specified Visual Studio version</returns>
    private static string GetRegKeyPath(string vsVersion)
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
    private static void SetSessionValue(Session session, string sessionName, string value)
    {
      session.Log(string.Format(Resources.SetSessionVariableValue, sessionName, value));
      session[sessionName] = value;
    }

    /// <summary>
    /// Sets the installation paths for all VS2017+ flavors.
    /// </summary>
    public static void SetVSInstallationPaths()
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
              // Determine the instance's flavor.
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
       catch (Exception ex)
       {
         Logger.LogException(ex);
       }
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
      SupportedVisualStudioVersions visualStudioVersion;
      switch (feature)
      {
        case VS2017_COMMUNITY_INSTALL_FEATURE:
          visualStudioInstallationPath = session.CustomActionData["VS2017ComPath"];
          visualStudioVersion = SupportedVisualStudioVersions.Vs2017Community;
          break;

        case VS2017_ENTERPRISE_INSTALL_FEATURE:
          visualStudioInstallationPath = session.CustomActionData["VS2017EntPath"];
          visualStudioVersion = SupportedVisualStudioVersions.Vs2017Enterprise;
          break;

        case VS2017_PROFESSIONAL_INSTALL_FEATURE:
          visualStudioInstallationPath = session.CustomActionData["VS2017ProPath"];
          visualStudioVersion = SupportedVisualStudioVersions.Vs2017Professional;
          break;

        case VS2019_COMMUNITY_INSTALL_FEATURE:
          visualStudioInstallationPath = session.CustomActionData["VS2019ComPath"];
          visualStudioVersion = SupportedVisualStudioVersions.Vs2019Community;
          break;

        case VS2019_ENTERPRISE_INSTALL_FEATURE:
          visualStudioInstallationPath = session.CustomActionData["VS2019EntPath"];
          visualStudioVersion = SupportedVisualStudioVersions.Vs2019Enterprise;
          break;

        case VS2019_PROFESSIONAL_INSTALL_FEATURE:
          visualStudioInstallationPath = session.CustomActionData["VS2019ProPath"];
          visualStudioVersion = SupportedVisualStudioVersions.Vs2019Professional;
          break;

        default:
          session.Log(string.Format(Resources.FeatureNotSupported, feature));
          return false;
      }

      if (!string.IsNullOrEmpty(visualStudioInstallationPath))
      {
        session.Log(string.Format(Resources.ProductInstallationPath, "Visual Studio", visualStudioInstallationPath));
        if (UpdatePkgdefFile(visualStudioVersion, visualStudioInstallationPath, new Version(mysqlForVisualStudioVersion), installedMySqlDataVersion, internalMySqlDataVersion, out var logMessage, Encoding.Unicode))
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
    /// <param name="visualStudioVersion">The Visual Studio installation to check.</param>
    /// <param name="visualStudioInstallationPath">The installation path for the Visual Studio version where the PKGDEF file exists.</param>
    /// <param name="mySqlForVisualStudioVersion">The version number of MySQL for Visual Studio.</param>
    /// <param name="installedMySqlDataVersion">The version number of the MySql.Data library found in the Connector/NET installation.</param>
    /// <param name="logData">The log message to display.</param>
    /// <param name="internalMySqlDataVersion">The version number of the MySql.Data included in this MySQL for Visual Studio installlation.</param>
    /// <param name="encoding">The encoding to use when updating the PKGDEF file.</param>
    /// <returns></returns>
    public static bool UpdatePkgdefFile(
      SupportedVisualStudioVersions visualStudioVersion,
      string visualStudioInstallationPath,
      Version mySqlForVisualStudioVersion,
      Version installedMySqlDataVersion,
      Version internalMySqlDataVersion,
      out string logData,
      Encoding encoding = null)
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

      var currentFileStatus = GetPkgdefFileStatus(visualStudioVersion, mySqlForVisualStudioVersion);
      var expectedFileStatus = PkgdefFileStatus.Unknown;
      if (installedMySqlDataVersion == null)
      {
        expectedFileStatus = PkgdefFileStatus.NoBindingRedirectEntries;
      }
      else if (installedMySqlDataVersion < internalMySqlDataVersion)
      {
        expectedFileStatus = PkgdefFileStatus.RedirectFromOlderToInternalMySqlDataEntry;
      }
      else if (installedMySqlDataVersion > internalMySqlDataVersion)
      {
        expectedFileStatus = PkgdefFileStatus.RedirectFromInternalToInstalledMySqlDataEntry;
      }

      if (currentFileStatus == expectedFileStatus)
      {
        logBuilder.AppendLine(Resources.PkgdefFileUpdateNotRequired);
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
    internal static PkgdefFileStatus GetPkgdefFileStatus(SupportedVisualStudioVersions visualStudioVersion, Version mysqlForVisualStudioVersion)
    {
      if (mysqlForVisualStudioVersion == null)
      {
        return PkgdefFileStatus.Unknown;
      }

      string visualStudioInstallationPath = null;
      if (visualStudioVersion >= SupportedVisualStudioVersions.Vs2017Community)
      {
        SetVSInstallationPaths();
      }

      switch (visualStudioVersion)
      {
        case SupportedVisualStudioVersions.Vs2017Community:
          visualStudioInstallationPath = _vs2017CommunityInstallationPath;
          break;

        case SupportedVisualStudioVersions.Vs2017Enterprise:
          visualStudioInstallationPath = _vs2017EnterpriseInstallationPath;
          break;

        case SupportedVisualStudioVersions.Vs2017Professional:
          visualStudioInstallationPath = _vs2017ProfessionalInstallationPath;
          break;

        case SupportedVisualStudioVersions.Vs2019Community:
          visualStudioInstallationPath = _vs2019CommunityInstallationPath;
          break;

        case SupportedVisualStudioVersions.Vs2019Enterprise:
          visualStudioInstallationPath = _vs2019EnterpriseInstallationPath;
          break;

        case SupportedVisualStudioVersions.Vs2019Professional:
          visualStudioInstallationPath = _vs2019ProfessionalInstallationPath;
          break;

        default:
          throw new NotSupportedException(Resources.VisualStudioVersionNotSupported);
      }

      if (string.IsNullOrEmpty(visualStudioInstallationPath))
      {
        return PkgdefFileStatus.Unknown;
      }

      return ReadPkgdefFileStatus(Utility.Utilities.GetPkgdefFilePath(visualStudioInstallationPath, mysqlForVisualStudioVersion), visualStudioVersion);
    }

    /// <summary>
    /// Gets the status for all supported versions of Visual Studio.
    /// </summary>
    /// <param name="mysqlForVisualStudioVersion">The version number of the installed MySQL for Visual Studio product.</param>
    /// <returns>A tuple list with the status of the PKGDEF files where MySQL for Visual Studio is installed.</returns>
    public static List<Tuple<SupportedVisualStudioVersions, PkgdefFileStatus>> GetPkgdefFileStatuses(Version mysqlForVisualStudioVersion)
    {
      if (mysqlForVisualStudioVersion == null)
      {
        return null;
      }

      var list = new List<Tuple<SupportedVisualStudioVersions, PkgdefFileStatus>>();
      list.Add(new Tuple<SupportedVisualStudioVersions, PkgdefFileStatus>(SupportedVisualStudioVersions.Vs2017Community, GetPkgdefFileStatus(SupportedVisualStudioVersions.Vs2017Community, mysqlForVisualStudioVersion)));
      list.Add(new Tuple<SupportedVisualStudioVersions, PkgdefFileStatus>(SupportedVisualStudioVersions.Vs2017Enterprise, GetPkgdefFileStatus(SupportedVisualStudioVersions.Vs2017Enterprise, mysqlForVisualStudioVersion)));
      list.Add(new Tuple<SupportedVisualStudioVersions, PkgdefFileStatus>(SupportedVisualStudioVersions.Vs2017Professional, GetPkgdefFileStatus(SupportedVisualStudioVersions.Vs2017Professional, mysqlForVisualStudioVersion)));
      list.Add(new Tuple<SupportedVisualStudioVersions, PkgdefFileStatus>(SupportedVisualStudioVersions.Vs2019Community, GetPkgdefFileStatus(SupportedVisualStudioVersions.Vs2019Community, mysqlForVisualStudioVersion)));
      list.Add(new Tuple<SupportedVisualStudioVersions, PkgdefFileStatus>(SupportedVisualStudioVersions.Vs2019Enterprise, GetPkgdefFileStatus(SupportedVisualStudioVersions.Vs2019Enterprise, mysqlForVisualStudioVersion)));
      list.Add(new Tuple<SupportedVisualStudioVersions, PkgdefFileStatus>(SupportedVisualStudioVersions.Vs2019Professional, GetPkgdefFileStatus(SupportedVisualStudioVersions.Vs2019Professional, mysqlForVisualStudioVersion)));

      return list;
    }

    /// <summary>
    /// Checks if any of the PKGDEF files need to be updated.
    /// </summary>
    public static bool IsConfigurationUpdateRequired(Version mysqlForVisualStudioVersion, Version installedMySqlDataVersion, Version internalMySqlDataVersion)
    {
      if (mysqlForVisualStudioVersion == null
          || internalMySqlDataVersion == null)
      {
        return false;
      }

      // Get PKGDEF file status.
      var pkgdefFileStatuses = GetPkgdefFileStatuses(mysqlForVisualStudioVersion);
      if (pkgdefFileStatuses == null)
      {
        Logger.LogError("Failed to get the MySQL for Visual Studio configurations of the Visual Studio installations.");
        return false;
      }

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
          && pkgdefFileStatuses.Any(o => o.Item2 != PkgdefFileStatus.NoBindingRedirectEntries && o.Item2 != PkgdefFileStatus.Unknown))
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

    /// <summary>
    /// Reads the specified pkdef file for any MySql.Data entries and identifies if updates are required.
    /// </summary>
    /// <param name="pkgdefFilePath">The path to the pkgdef file.</param>
    /// <returns>A value indicating the type of update required on the pkgdef file.</returns>
    internal static PkgdefFileStatus ReadPkgdefFileStatus(string pkgdefFilePath, SupportedVisualStudioVersions visualStudioVersion)
    {
      if (string.IsNullOrEmpty(pkgdefFilePath)
          || !File.Exists(pkgdefFilePath))
      {
        Logger.LogError(Resources.InvalidPkgdefFile);
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
        }

        return PkgdefFileStatus.NoBindingRedirectEntries;
      }
      catch (Exception)
      {
        Logger.LogError(string.Format(Resources.FailedToReadThePkgdefFile, visualStudioVersion.GetDescription(), pkgdefFilePath));
        return PkgdefFileStatus.Unknown;
      }
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
