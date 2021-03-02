// Copyright (c) 2015, 2021, Oracle and/or its affiliates.
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

using System;
using System.Linq;
using System.Configuration;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using EnvDTE;
using EnvDTE80;
using MySql.Utility.Classes.Logging;
using MySql.Data.VisualStudio.Wizards.Web;
#if NET_461_OR_GREATER
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.ComponentModelHost;
using NuGet.VisualStudio;
#endif
using VSLangProj;

namespace MySql.Data.VisualStudio.WebConfig
{
  /// <summary>
  /// Struct used to hold the Entity Framework available versions that can be used.
  /// </summary>
  internal struct EntityFrameworkOptions
  {
    public bool EF5;
    public bool EF6;
  }

  /// <summary>
  /// Enumerator used to get the .Net Framework version from the value returned by the .Net Target Framework Moniker.
  /// </summary>
  internal enum TargetFramework
  {
    Fx472 = 262663,
    Fx461 = 262406,
    Fx46 = 262150,
    Fx452 = 262661,
    Fx451 = 262405,
    Fx45 = 262149,
    Fx40 = 262144,
    Fx35 = 196613,
    Fx30 = 196608,
    Fx20 = 131072
  }

  internal class EntityFrameworkConfig : GenericConfig
  {
    private new EntityFrameworkOptions defaults = new EntityFrameworkOptions();
    private new EntityFrameworkOptions values;
    private bool _entityFrameworkEnabled = false;
    private const string EF5Version = "5.0.0";
    private const string EF6Version = "6.4.4";
    private const string mySQLData = "MySql.Data";
    private const string mySQLEF5Version = "6.9.12";
    private string _mySQLEF = "MySql.Data.Entity";
    private string _mySQLEF6Version;

    /// <summary>
    /// The name of the configuration file for the current project.
    /// </summary>
    private string _configFileName;

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityFrameworkConfig"/> class.
    /// </summary>
    public EntityFrameworkConfig(string configFileName)
      : base()
    {
      _configFileName = configFileName;
      typeName = "MySQLEntityFrameworkProvider";
      sectionName = "entityFramework";
      var factory = DbProviderFactories.GetFactory("MySql.Data.MySqlClient");
      _mySQLEF6Version = factory != null
        ? factory.GetType().Assembly.GetName().Version.ToString(3)
        : mySQLEF5Version;
      if (_mySQLEF6Version.StartsWith("8")) _mySQLEF = "MySql.Data.EntityFramework";
    }

    /// <summary>
    /// Gets or sets the entity framework options.
    /// </summary>
    /// <value>
    /// The entity framework options.
    /// </value>
    public EntityFrameworkOptions EntityFrameworkOptions
    {
      get { return values; }
      set { values = value; }
    }

    /// <summary>
    /// Implementation of the base abstract method used to get the machine settings.
    /// </summary>
    /// <returns>null</returns>
    protected override ProviderSettings GetMachineSettings()
    {
      return null;
    }

    /// <summary>
    /// Initializes the specified web config object, setting the Entity Framework section and the config section of the config file.
    /// </summary>
    /// <param name="wc">The Webconfig object.</param>
    public override void Initialize(AppConfig wc)
    {
      _entityFrameworkEnabled = false;
      XmlElement entityFramework = wc.GetProviderSection("entityFramework");
      XmlElement configSections = wc.GetProviderSection("configSections");
      if (entityFramework != null)
      {
        XElement entityFrameworkSection = XElement.Parse(entityFramework.OuterXml);
        XElement mySQLEFProvider = entityFrameworkSection.Elements("defaultConnectionFactory")
                                    .FirstOrDefault(a => a.Attribute("type") != null && a.Attribute("type").Value.Contains("MySql.Data.Entity"));
        if (mySQLEFProvider != null)
        {
          _entityFrameworkEnabled = true;
        }
      }

      Enabled = OriginallyEnabled = _entityFrameworkEnabled;
      XElement configSectionXElement = configSections != null ? XElement.Parse(configSections.OuterXml) : null;
      SetDefaults(configSectionXElement);
      values = defaults;
    }

    /// <summary>
    /// Sets the default values for an XElement config section.
    /// </summary>
    /// <param name="configSections">The XElement configuration section.</param>
    private void SetDefaults(XElement configSections)
    {
      if (configSections != null)
      {
        XElement entityFrameworkSection = configSections.Elements("section").FirstOrDefault(a => a.Attribute("name") != null
                                            && a.Attribute("name").Value == "entityFramework");
        if (entityFrameworkSection != null)
        {
          defaults.EF5 = entityFrameworkSection.Attribute("type").Value.Contains("Version=5");
          defaults.EF6 = entityFrameworkSection.Attribute("type").Value.Contains("Version=6");
        }
      }
      else
      {
        defaults.EF5 = false;
        defaults.EF6 = false;
      }
    }

    /// <summary>
    /// Saves the specified WebConfig object.
    /// </summary>
    /// <param name="wc">The WebConfig object.</param>
    public override void Save(AppConfig wc)
    {
      if (!Enabled && OriginallyEnabled)
      {
        RemoveEFReferences(true);
        RemoveEntityFrameworkSection();
        return;
      }
      else if (Enabled && !OriginallyEnabled)
      {
        if (values.EF5)
        {
          SaveEFConfig(true, EF5Version, mySQLEF5Version);
        }

        if (values.EF6)
        {
          SaveEFConfig(true, EF6Version, _mySQLEF6Version);
        }
      }

      if (defaults.EF5 != values.EF5 && values.EF5)
      {
        SaveEFConfig(true, EF5Version, mySQLEF5Version);
      }

      if (defaults.EF6 != values.EF6 && values.EF6)
      {
        SaveEFConfig(true, EF6Version, _mySQLEF6Version);
      }
    }

    /// <summary>
    /// Saves the Entity Framework configuration, removing unused references and adding the proper NuGet packages and the config section values.
    /// </summary>
    /// <param name="removeEFReferences">When set to <c>true</c> will [remove the Entity Framework references] from the project.</param>
    /// <param name="EFVersion">The Entity Framework version.</param>
    /// <param name="mySQLVersion">The My SQL version.</param>
    private void SaveEFConfig(bool removeEFReferences, string EFVersion, string mySQLVersion)
    {
      RemoveEFReferences(removeEFReferences);
      AddEFNugetPackage(EFVersion);
      AddEntityFrameworkSection(EFVersion, mySQLVersion);
    }

    /// <summary>
    /// Removes the Entity Framework references from the project.
    /// </summary>
    /// <param name="removeEFReferences">When set to <c>true</c> will [remove the Entity Framework references] from the project.</param>
    private void RemoveEFReferences(bool removeEFReferences)
    {
      Project activeProj = null;
      VSProject vsProj = null;
      DTE2 vsApp = MySqlDataProviderPackage.GetGlobalService(typeof(EnvDTE.DTE)) as EnvDTE80.DTE2;
      Array activeProjects = (Array)vsApp.ActiveSolutionProjects;
      if (activeProjects.Length > 0)
      {
        activeProj = (Project)activeProjects.GetValue(0);
        vsProj = activeProj.Object as VSProject;

        // Remove reference to MySql.Data.
        if (vsProj.References.Find(mySQLData) != null)
        {
          vsProj.References.Find(mySQLData).Remove();
        }

        // Remove reference to MySql.Data.Entity.EF5.
        if (vsProj.References.Find(string.Format("{0}.EF5", _mySQLEF)) != null)
        {
          vsProj.References.Find(string.Format("{0}.EF5", _mySQLEF)).Remove();
        }

        // Remove reference to MySql.Data.Entity.EF6.
        if (vsProj.References.Find(string.Format("{0}.EF6", _mySQLEF)) != null)
        {
          vsProj.References.Find(string.Format("{0}.EF6", _mySQLEF)).Remove();
        }

        // Remove reference to MySql.Data.EntityFramework.
        if (vsProj.References.Find(_mySQLEF) != null)
        {
          vsProj.References.Find(_mySQLEF).Remove();
        }

        // Remove EF references.
        if (removeEFReferences)
        {
          if (vsProj.References.Find("EntityFramework") != null)
          {
            vsProj.References.Find("EntityFramework").Remove();
          }

          if (vsProj.References.Find("EntityFramework.SqlServer") != null)
          {
            vsProj.References.Find("EntityFramework.SqlServer").Remove();
          }
        }
      }
    }

    /// <summary>
    /// Adds the Entity Framework nuget package to the project.
    /// </summary>
    /// <param name="EFVersion">The Entity Framework version.</param>
    private void AddEFNugetPackage(string EFVersion)
    {
      Project activeProj = null;
      VSProject vsProj = null;
      DTE2 vsApp = MySqlDataProviderPackage.GetGlobalService(typeof(EnvDTE.DTE)) as EnvDTE80.DTE2;
      Array activeProjects = (Array)vsApp.ActiveSolutionProjects;
      if (activeProjects.Length > 0)
      {
        activeProj = (Project)activeProjects.GetValue(0);
        vsProj = activeProj.Object as VSProject;
        string projectPath = System.IO.Path.GetDirectoryName(activeProj.FullName);
        TargetFramework targetNetworkMoniker = (TargetFramework)Enum.ToObject(typeof(TargetFramework), vsApp.Solution.Projects.Item(1).Properties.Item("TargetFramework").Value);
        string NetFxVersion = GetNetFxVersion(targetNetworkMoniker);
        AddNugetPackage(vsProj, projectPath, NetFxVersion, "EntityFramework", EFVersion, true);
        AddNugetPackage(vsProj, projectPath, NetFxVersion, mySQLData, _mySQLEF6Version, true);
        switch (EFVersion)
        {
          case EF5Version:
            AddNugetPackage(vsProj, projectPath, NetFxVersion, "MySql.Data.Entity", mySQLEF5Version, true);
            break;
          case EF6Version:
            AddNugetPackage(vsProj, projectPath, NetFxVersion, _mySQLEF, _mySQLEF6Version, true);
            break;
          default:
            throw new Exception("Not supported Entity Framework version.");
        }

        activeProj.Save();
        vsProj.Project.Save();
      }
    }

    /// <summary>
    /// Gets the .NET Framework version from the Target Framework Moniker.
    /// </summary>
    /// <param name="targetNetworkMoniker">The target network moniker.</param>
    /// <returns>A string with the .NET Framework version, or empty when the Target Framework Moniker does not match.</returns>
    private string GetNetFxVersion(TargetFramework targetNetworkMoniker)
    {
      switch (targetNetworkMoniker)
      {
        case TargetFramework.Fx20: return "2.0";
        case TargetFramework.Fx30: return "3.0";
        case TargetFramework.Fx35: return "3.5";
        case TargetFramework.Fx40: return "4.0";
        case TargetFramework.Fx45: return "4.5";
        case TargetFramework.Fx451: return "4.5.1";
        case TargetFramework.Fx452: return "4.5.2";
        case TargetFramework.Fx46: return "4.6";
        case TargetFramework.Fx461: return "4.6.1";
        case TargetFramework.Fx472: return "4.7.2";
      }

      return string.Empty;
    }

    /// <summary>
    /// Adds the specified nuget package to the project.
    /// </summary>
    /// <param name="VsProj">The vs proj.</param>
    /// <param name="projectPath">The project path.</param>
    /// <param name="NetFxVersion">The net fx version.</param>
    /// <param name="PackageName">Name of the package.</param>
    /// <param name="Version">The version of the package.</param>
    /// <param name="addReference">if set to <c>true</c> then [add reference] to the project.</param>
    private void AddNugetPackage(VSProject VsProj, string projectPath, string NetFxVersion, string PackageName, string Version, bool addReference)
    {
      DirectoryInfo di = new DirectoryInfo(projectPath);
      string solPath = di.Parent.FullName;
      try
      {
#if NET_461_OR_GREATER
        object componentModelObj = Package.GetGlobalService(typeof(SComponentModel));
        var componentModel = (IComponentModel) componentModelObj;
        var packageInstaller = componentModel.GetService<IVsPackageInstaller>();
        packageInstaller.InstallPackage(null, VsProj.Project, PackageName, Version, true);
#else
        Assembly nugetAssembly = Assembly.Load("nuget.core");
        Type packageRepositoryFactoryType = nugetAssembly.GetType("NuGet.PackageRepositoryFactory");
        PropertyInfo piDefault = packageRepositoryFactoryType.GetProperty("Default");
        MethodInfo miCreateRepository = packageRepositoryFactoryType.GetMethod("CreateRepository");
        object repo = miCreateRepository.Invoke(piDefault.GetValue(null, null), new object[] { "https://packages.nuget.org/api/v2" });
        Type packageManagerType = nugetAssembly.GetType("NuGet.PackageManager");
        ConstructorInfo ciPackageManger = packageManagerType.GetConstructor(new Type[] { System.Reflection.Assembly.Load("nuget.core").GetType("NuGet.IPackageRepository"), typeof(string) });
        string installPath = di.Parent.CreateSubdirectory("packages").FullName;
        object packageManager = ciPackageManger.Invoke(new object[] { repo, installPath });
        MethodInfo miInstallPackage = packageManagerType.GetMethod("InstallPackage", new Type[] { typeof(string), System.Reflection.Assembly.Load("nuget.core").GetType("NuGet.SemanticVersion") });
        string packageID = PackageName;
        MethodInfo miParse = nugetAssembly.GetType("NuGet.SemanticVersion").GetMethod("Parse");

        // Special condition to handle NuGet package suffixes
        if (PackageName.StartsWith("MySql.Data"))
        {
          if (Version == "8.0.10") Version += "-rc";
        }

        object semanticVersion = miParse.Invoke(null, new object[] { Version });
        miInstallPackage.Invoke(packageManager, new object[] { packageID, semanticVersion });
#endif
        if (addReference)
        {
          AddPackageReference(VsProj, NetFxVersion, solPath, PackageName, Version);
        }
      }
      catch (Exception ex)
      {
        Logger.LogError($"{PackageName} installation package failure.{Environment.NewLine}Please check that you have the latest Nuget version and that you have an internet connection.{Environment.NewLine}{ex.Message}");
        // The inner exception contains the error message that actually informs of the error.
        if (ex.InnerException != null)
        {
          Logger.LogException(ex.InnerException);
        }
        return;
      }
    }

    /// <summary>
    /// Adds the specified package reference to the project.
    /// </summary>
    /// <param name="VsProj">The vs proj.</param>
    /// <param name="NetFxVersion">The net fx version.</param>
    /// <param name="BasePath">The base path of the project.</param>
    /// <param name="PackageName">Name of the package.</param>
    /// <param name="Version">The package version.</param>
    private void AddPackageReference(VSProject VsProj, string NetFxVersion, string BasePath, string PackageName, string Version)
    {
      string efPath = Path.Combine("packages", string.Format("{0}.{1}\\lib", PackageName, Version));
      string packagePath = Path.Combine(BasePath, efPath);

      // Remove suffix from Version if it has one.
      int suffixStartIndex = Version.IndexOf('-');
      if (suffixStartIndex != -1) Version = Version.Substring(0, suffixStartIndex);

      if (NetFxVersion.StartsWith("4.5") || NetFxVersion.StartsWith("4.6") || NetFxVersion.StartsWith("4.7"))
      {
        // If .NET version is greater than 4.5 then extract the version numnber from NetFxVersion variable.
        packagePath = Path.Combine(
          packagePath,
          "net" + ((PackageName==mySQLData || PackageName==_mySQLEF) && Version==_mySQLEF6Version && !Version.StartsWith("6.9") ?
                      "452" :
                      "45"));
      }
      else if (NetFxVersion.StartsWith("4.0"))
      {
        // If .NET version is greater than 4.0 then extract the version numnber from NetFxVersion variable.
        packagePath = Path.Combine(
          packagePath,
          "net" + ((PackageName==mySQLData || PackageName==_mySQLEF) && Version==_mySQLEF6Version && !Version.StartsWith("6.9") ?
                      NetFxVersion.Replace(".", "") :
                      "40"));
      }
      else
      {
        var message = $"{Properties.Resources.WrongNetFxVersionMessage} (received version {NetFxVersion})";
        Logger.LogError(message);
        throw new Exception(message);
      }

      // Applies for MySql.Data.Entity.EF5 and EF6 packages.
      if (PackageName == "MySql.Data.Entity")
      {
        if (Version == mySQLEF5Version)
        {
          packagePath = Path.Combine(packagePath, PackageName + ".EF5.dll");
        }

        if (Version == _mySQLEF6Version)
        {
          packagePath = Path.Combine(packagePath, PackageName + ".EF6.dll");
        }
      }
      // Applies for MySql.Data.Entity.Framework and EF packages.
      else
      {
        packagePath = Path.Combine(packagePath, PackageName + ".dll");
      }

      VsProj.References.Add(packagePath);
      if (PackageName.Contains("EntityFramework") && Version == EF6Version)
      {
        packagePath = packagePath.Substring(0, packagePath.LastIndexOf("\\") + 1);
        packagePath = Path.Combine(packagePath, PackageName + ".SqlServer.dll");
        VsProj.References.Add(packagePath);
      }
    }

    /// <summary>
    /// Adds the entity framework section to an existing web config file.
    /// </summary>
    /// <param name="EFVersion">The Entity Framework version.</param>
    /// <param name="mySqlVersion">The My SQL version.</param>
    private void AddEntityFrameworkSection(string EFVersion, string mySqlVersion)
    {
      Project activeProj = null;
      VSProject vsProj = null;
      DTE2 vsApp = MySqlDataProviderPackage.GetGlobalService(typeof(EnvDTE.DTE)) as EnvDTE80.DTE2;
      Array activeProjects = (Array)vsApp.ActiveSolutionProjects;
      if (activeProjects.Length > 0)
      {
        activeProj = (Project)activeProjects.GetValue(0);
        vsProj = activeProj.Object as VSProject;
        string projectPath = Path.GetDirectoryName(activeProj.FullName);
        AppConfigTools.EFWebConfigTransformation(projectPath, EFVersion, mySqlVersion, _mySQLEF6Version, _configFileName);
      }
    }

    /// <summary>
    /// Removes the entity framework section from an existing web config file.
    /// </summary>
    private void RemoveEntityFrameworkSection()
    {
      Project activeProj = null;
      VSProject vsProj = null;
      DTE2 vsApp = MySqlDataProviderPackage.GetGlobalService(typeof(EnvDTE.DTE)) as EnvDTE80.DTE2;
      Array activeProjects = (Array)vsApp.ActiveSolutionProjects;
      if (activeProjects.Length > 0)
      {
        activeProj = (Project)activeProjects.GetValue(0);
        vsProj = activeProj.Object as VSProject;
        string projectPath = System.IO.Path.GetDirectoryName(activeProj.FullName);
        AppConfigTools.RemoveEFWebConfig(projectPath, _configFileName);
      }
    }
  }
}
