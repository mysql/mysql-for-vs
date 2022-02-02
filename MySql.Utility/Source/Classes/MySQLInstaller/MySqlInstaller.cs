// Copyright (c) 2012, 2018, Oracle and/or its affiliates. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Win32;
using MySql.Utility.Classes.Logging;

namespace MySql.Utility.Classes.MySqlInstaller
{
  /// <summary>
  /// Class that holds all properties and methods required to interact with the MySql Installer product, specially updates.
  /// </summary>
  public static class MySqlInstaller
  {
    #region Constants
    /// <summary>
    /// The default number of miliseconds to wait for updates before dismissing communication trial and disconnecting.
    /// </summary>
    private const int DEFAULT_CHECK_FOR_UPDATES_TIMEOUT = 10000;

    #endregion Constants

    #region Fields

    /// <summary>
    /// Holds the reference to the the external api (Core Assembly).
    /// </summary>
    private static Assembly _coreAssembly;

    /// <summary>
    /// Holds the Type from the the external api (Core Assembly).
    /// </summary>
    private static Type _coreAssemblyType;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlInstaller"/> class. This class posses everything the MySqlInstaller class requires to work with.
    /// Architecture improves performance, avoiding recurrent calls to recalculate properties.
    /// </summary>
    static MySqlInstaller()
    {
      InstallerLegacyDllPath = string.Empty;
    }

    /// <summary>
    /// Enums the type of license or licenses from installed MySqlInstaller products.
    /// </summary>
    [Flags]
    public enum LicenseType
    {
      /// <summary>
      /// The MySQL Installer is not installed in this system.
      /// </summary>
      NotInstalled = 0,

      /// <summary>
      /// The MySQL Installer installed in this system is 1.3 or lower version.
      /// </summary>
      Legacy = 1 << 0,

      /// <summary>
      /// The MySQL Installer installed in this system is 1.4 or superior and the Community license for it was found.
      /// </summary>
      Community = 1 << 1,

      /// <summary>
      /// The MySQL Installer installed in this system is 1.4 or superior and the Commercial license for it was found.
      /// </summary>
      Commercial = 1 << 2,

      /// <summary>
      /// The MySQL Installer installed in this system cannot be recognized.
      /// </summary>
      Unknown = 1 << 3
    }

    #region Properties

    /// <summary>
    /// Gets the executable filename with extension from the executable file for the installed MySql Installer.
    /// </summary>
    public static string ExeFilePath => System.IO.Path.Combine(Path, ExeFilename);

    /// <summary>
    /// Gets the installer legacy path.
    /// </summary>
    public static string InstallerLegacyDllPath { get; set; }

    /// <summary>
    /// Gets a value indicating whether a MySql Installer is installed in the system.
    /// </summary>
    public static bool IsInstalled => License != LicenseType.NotInstalled;

    /// <summary>
    /// Gets the value for the version of the installed MySql Installer product, true if 1.4 or newer.
    /// </summary>
    public static bool IsNewer => License.HasFlag(LicenseType.Commercial) || License.HasFlag(LicenseType.Community);

    /// <summary>
    /// Gets the available license(s) from the installed MySql Installer.
    /// </summary>
    public static LicenseType License { get; private set; }

    /// <summary>
    /// Holds the working MySql Installer path.
    /// </summary>
    public static string Path { get; private set; }

    /// <summary>
    /// Holds the installer version in the form of a string.
    /// </summary>
    public static string Version { get; private set; }

    /// <summary>
    /// Gets the external api (Core Dll) file path.
    /// </summary>
    private static string CoreDllFilePath => License == LicenseType.Legacy
      ? System.IO.Path.Combine(InstallerLegacyDllPath, Filename + Resources.CoreDllFileExtension)
      : System.IO.Path.Combine(Path, Filename + Resources.CoreDllFileExtension);

    /// <summary>
    /// Gets the executable filename with extension from the executable file for the installed MySql Installer.
    /// </summary>
    private static string ExeFilename => Resources.NewInstallerFilename + Resources.ExeFileExtension;

    /// <summary>
    /// Gets the Core Assembly filename for the current installed Product version.
    /// </summary>
    private static string Filename => License.HasFlag(LicenseType.Legacy) ? Resources.LegacyInstallerFilename : Resources.NewInstallerFilename;

    #endregion Properties

    /// <summary>
    /// Checks for updates of installed MySql software and returns an integer representing the number of available updates for the provided type of License.
    /// </summary>
    /// <param name="timeout">The timeout.</param>
    /// <param name="license">The licence to check updates availability for.</param>
    /// <returns></returns>
    public static int CheckForUpdates(LicenseType license, int timeout = DEFAULT_CHECK_FOR_UPDATES_TIMEOUT)
    {
      if (_coreAssembly == null || _coreAssemblyType == null)
      {
        return -1;
      }

      string methodName;
      object[] parameters = null;
      switch (license)
      {
        case LicenseType.Legacy:
          methodName = "CheckForUpdates";
          parameters = new object[] { timeout };
          break;
        case LicenseType.Commercial:
          methodName = "CommercialUpdatesAvailable";
          break;
        case LicenseType.Community:
          methodName = "CommunityUpdatesAvailable";
          break;
        default:
          return -1;
      }

      try
      {
        AppDomain.CurrentDomain.SetupInformation.ShadowCopyFiles = "true";
        var coreAssemblyInstance = AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(@CoreDllFilePath, _coreAssemblyType.ToString());
        var returnValue = coreAssemblyInstance
            .GetType()
            .GetMethod(methodName)
            .Invoke(coreAssemblyInstance, parameters);
        if (license == LicenseType.Legacy)
        {
          int result;
          int.TryParse(returnValue.ToString(), out result);
          return result;
        }
        else
        {
          var result = returnValue as IEnumerable<object>;
          return result?.Count() ?? -1;
        }
      }
      catch (Exception e)
      {
        Logger.LogException(e);
        return -1;
      }
    }

    /// <summary>
    /// Initializes the MySqlInstaller data, and loads the external core assemblies.
    /// </summary>
    public static void LoadData()
    {
      SetLicense();
      SetPath();
      SetAssembly();
    }

    /// <summary>
    /// Gets the the working MySql Installer path and sets the local property Path to hold it.
    /// </summary>
    private static void SetAssembly()
    {
      if (!IsInstalled || String.IsNullOrEmpty(Path))
      {
        _coreAssembly = null;
        _coreAssemblyType = null;
        return;
      }

      _coreAssembly = File.Exists(CoreDllFilePath) ? Assembly.LoadFile(CoreDllFilePath) : null;
      if (_coreAssembly == null)
      {
        _coreAssemblyType = null;
      }
      else
      {
        _coreAssemblyType = License.HasFlag(LicenseType.Legacy)
          ? _coreAssembly.GetType("WexInstaller.Core.ExternalAPI")
          : _coreAssembly.GetType("MySQLInstaller.Core.ExternalAPI");
        SetVersion();
      }
    }

    /// <summary>
    /// Gets the installed MySql Installer available license(s) and sets the local Licence property that holds them.
    /// </summary>
    private static void SetLicense()
    {
      SetLicenseByRegistryKey();
      if (License != LicenseType.NotInstalled)
      {
        return;
      }

      SetLicenseByConfigFile();
    }

    /// <summary>
    /// Gets the installed MySql Installer available license(s) by browsing the file system for key MySql Installer files and then sets the local Licence property that will hold them.
    /// </summary>
    private static void SetLicenseByConfigFile()
    {
      if (File.Exists(Resources.CommercialXMLFilePath))
      {
        License |= LicenseType.Commercial;
      }

      if (File.Exists(Resources.CommunityXMLFilePath))
      {
        License |= LicenseType.Community;
      }

      if (License == LicenseType.NotInstalled)
      {
        License = File.Exists(
        System.IO.Path.Combine(Environment.Is64BitOperatingSystem
          ? Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)
          : Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
          Resources.BaseInstallerPath,
          Resources.LegacyInstallerFilename,
          Resources.ExeFileExtension)) ? LicenseType.Legacy : LicenseType.NotInstalled;
      }
    }

    /// <summary>
    /// Gets the installed MySql Installer available license(s) by browsing the registry for MySql Installer keys and then sets the local Licence property that will hold them.
    /// </summary>
    private static void SetLicenseByRegistryKey()
    {
      License = LicenseType.NotInstalled;
      var key = RegistryHive.LocalMachine.OpenRegistryKey(@"Software\MySQL\MySQL Installer - Community");
      var value = key?.GetValue("installed");
      int installed;
      if (value != null)
      {
        int.TryParse(value.ToString(), out installed);
        if (installed == 1)
        {
          License = LicenseType.Community;
        }
      }

      key = RegistryHive.LocalMachine.OpenRegistryKey(@"Software\MySQL\MySQL Installer - Commercial");
      value = key?.GetValue("installed");
      if (value != null)
      {
        int.TryParse(value.ToString(), out installed);
        if (installed == 1)
        {
          if (License.HasFlag(LicenseType.Community))
          {
            License |= LicenseType.Commercial;
          }
          else
          {
            License = LicenseType.Commercial;
          }
        }
      }

      if (License != LicenseType.NotInstalled)
      {
        return;
      }

      key = Utilities.OpenRegistryKey(@"Software\MySQL\MySQL Installer");
      value = key?.GetValue("installed");
      if (value == null)
      {
        return;
      }

      int.TryParse(value.ToString(), out installed);
      License = installed == 1 ? LicenseType.Legacy : LicenseType.Unknown;
    }

    /// <summary>
    /// Gets the the working MySql Installer path and sets the local property Path to hold it.
    /// </summary>
    private static void SetPath()
    {
      if (License == LicenseType.NotInstalled || License == LicenseType.Unknown)
      {
        Path = string.Empty;
        return;
      }

      var pathBuilder = new StringBuilder(Resources.BaseInstallerPath);

      if (!License.HasFlag(LicenseType.Legacy))
      {
        pathBuilder.Append(@" for Windows");
      }

      pathBuilder.Append(@"\");

      pathBuilder.Insert(0, Environment.Is64BitOperatingSystem
        ? Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)
        : Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
      var path = pathBuilder.ToString();
      Path = File.Exists(System.IO.Path.Combine(path, ExeFilename)) ? path : string.Empty;
      if (string.IsNullOrEmpty(InstallerLegacyDllPath))
      {
        InstallerLegacyDllPath = Path;
      }
    }

    /// <summary>
    /// Gets the MySql Installer version and sets the local property that holds it.
    /// </summary>
    private static void SetVersion()
    {
      Version = Utilities.GetProductVersion(CoreDllFilePath);
    }
  }
}
