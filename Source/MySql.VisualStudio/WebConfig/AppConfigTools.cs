// Copyright (c) 2015, 2019, Oracle and/or its affiliates. All rights reserved.
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MySql.Data.VisualStudio.WebConfig
{
  public static class AppConfigTools
  {
    private const string EF5Version = "5.0.0";
    private const string EF6Version = "6.1.3";

    private const string EF5SectionTypeValue = "System.Data.Entity.Internal.ConfigFile.EntityFrameworkSection, EntityFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
    private const string EF6SectionTypeValue = "System.Data.Entity.Internal.ConfigFile.EntityFrameworkSection, EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
    private const string defaultConnectionFactoryEF5TypeValue = "MySql.Data.Entity.MySqlConnectionFactory, MySql.Data.Entity.EF5";
    private const string EF5MySqlClientFactoryTypeValue = "MySql.Data.MySqlClient.MySqlClientFactory, MySql.Data, Version=6.9.12.0, Culture=neutral, PublicKeyToken=c5687fc88969c44d";
    private const string EF5MySQLProviderTypeValue = "MySql.Data.MySqlClient.MySqlProviderServices, MySql.Data.Entity.EF5, Version=6.9.12.0, Culture=neutral, PublicKeyToken=c5687fc88969c44d";
    private const string SQLServerProviderTypeValue = "System.Data.Entity.SqlServer.SqlProviderServices, EntityFramework.SqlServer";

    private static Version _connectorVersion;
    private static string _defaultConnectionFactoryEF6TypeValue = "MySql.Data.Entity.MySqlConnectionFactory, MySql.Data.Entity.EF6";


    /// <summary>
    /// Transforms a config file in order to add the Entity Framework settings to it.
    /// </summary>
    /// <param name="projectPath">The project path.</param>
    /// <param name="EFVersion">The Entity Framework version.</param>
    /// <param name="mySQLVersion">The My SQL version.</param>
    /// <param name="connectorVersion">The version of the connector/net assembly installed.</param>
    /// <param name="configFileName">The name of the configuration file of the current project.</param>
    public static void EFWebConfigTransformation(string projectPath, string EFVersion, string mySQLVersion, string connectorVersion, string configFileName)
    {
      if (!string.IsNullOrEmpty(projectPath) && File.Exists(Path.Combine(projectPath, configFileName)))
      {
        _connectorVersion = new Version(connectorVersion);
        if (_connectorVersion >= new Version(8,0))
          _defaultConnectionFactoryEF6TypeValue = "MySql.Data.EntityFramework.MySqlConnectionFactory, MySql.Data.EntityFramework";

        XElement webConfig = XElement.Load(Path.Combine(projectPath, configFileName));
        RemoveEFSettings(webConfig);
        ConfigureEntityFrameworkSection(webConfig, EFVersion, mySQLVersion);
        ConfigureSystemDataSection(webConfig, EFVersion, mySQLVersion, connectorVersion);
        webConfig.Save(Path.Combine(projectPath, configFileName));
      }
    }

    /// <summary>
    /// Removes all the Entity Framework configuration from a config file.
    /// </summary>
    /// <param name="projectPath">The project path.</param>
    /// <param name="configFileName">The name of the configuration file of the current project.</param>
    public static void RemoveEFWebConfig(string projectPath, string configFileName)
    {
      if (!string.IsNullOrEmpty(projectPath) && File.Exists(Path.Combine(projectPath, configFileName)))
      {
        XElement webConfig = XElement.Load(Path.Combine(projectPath, configFileName));
        var mySQLDBProviderFactories = webConfig.Elements("system.data").Elements("DbProviderFactories").Elements()
                                        .Where(a => a.Attribute("invariant") != null && a.Attribute("invariant").Value == "MySql.Data.MySqlClient");
        if (mySQLDBProviderFactories != null)
        {
          mySQLDBProviderFactories.Remove();
        }

        var mySQLDefaultConnFactory = webConfig.Elements("entityFramework").Elements("defaultConnectionFactory")
                                        .Where(a => a.Attribute("type") != null && a.Attribute("type").Value.Contains("MySql.Data.Entity"));
        if (mySQLDefaultConnFactory != null)
        {
          mySQLDefaultConnFactory.Remove();
        }

        var mySQLEFProvider = webConfig.Elements("entityFramework").Elements("providers").Elements()
                                .Where(a => a.Attribute("invariantName") != null && a.Attribute("invariantName").Value.Contains("MySql.Data.MySqlClient"));
        if (mySQLEFProvider != null)
        {
          mySQLEFProvider.Remove();
        }

        webConfig.Save(Path.Combine(projectPath, configFileName));
      }
    }

    /// <summary>
    /// Removes the Entity Framework settings from a config file.
    /// </summary>
    /// <param name="webConfig">The Xelement web config file.</param>
    private static void RemoveEFSettings(XElement webConfig)
    {
      if (webConfig != null)
      {
        var EFConfigSection = webConfig.Elements("configSections").Elements()
                                .FirstOrDefault(a => a.Attribute("name") != null && a.Attribute("name").Value == "entityFramework");
        if (EFConfigSection != null)
        {
          EFConfigSection.Remove();
        }

        var dbProviderFactories = webConfig.Elements("system.data").Elements("DbProviderFactories").Elements()
                                    .Where(a => a.Attribute("invariant") != null && a.Attribute("invariant").Value == "MySql.Data.MySqlClient");
        if (dbProviderFactories != null)
        {
          dbProviderFactories.Remove();
        }

        var EFSection = webConfig.Element("entityFramework");
        if (EFSection != null)
        {
          EFSection.Remove();
        }
      }
    }

    /// <summary>
    /// Configures a web config file in order to add the Entity Framework sections.
    /// </summary>
    /// <param name="webConfig">The Xelement web config file.</param>
    /// <param name="EFVersion">The Entity Framework version.</param>
    /// <param name="mySqlVersion">The My SQL version.</param>
    /// <param name="connectorVersion">The version of the connector/net assembly installed.</param>
    private static void ConfigureEntityFrameworkSection(XElement webConfig, string EFVersion, string mySqlVersion)
    {
      if (webConfig != null)
      {
        ConfigureConfigSection(webConfig, EFVersion);
        XElement entityFrameworkSection = new XElement("entityFramework");
        XElement defaultConnectionFactory = new XElement("defaultConnectionFactory");
        if (EFVersion == EF5Version)
        {
          defaultConnectionFactory.Add(new XAttribute("type", defaultConnectionFactoryEF5TypeValue));
        }

        if (EFVersion == EF6Version)
        {
          defaultConnectionFactory.Add(new XAttribute("type", _defaultConnectionFactoryEF6TypeValue));
        }

        XElement parameters = new XElement("parameters");
        XElement parameter = new XElement("parameter");
        parameter.Add(new XAttribute("value", "v11.0"));
        parameters.Add(parameter);
        defaultConnectionFactory.Add(parameters);
        entityFrameworkSection.Add(defaultConnectionFactory);
        if (EFVersion == EF6Version)
        {
          XElement providers = new XElement("providers");
          entityFrameworkSection.Add(providers);
          webConfig.Add(entityFrameworkSection);
          CreateEFProvidersSection(webConfig, EFVersion, mySqlVersion, true);
        }
        else
        {
          webConfig.Add(entityFrameworkSection);
        }
      }
    }

    /// <summary>
    /// Configures the config section of a web config file, in order to add the Entity Framework settings.
    /// </summary>
    /// <param name="webConfig">The Xelement web config file.</param>
    /// <param name="EFVersion">The Entity Framework version.</param>
    private static void ConfigureConfigSection(XElement webConfig, string EFVersion)
    {
      if (webConfig != null)
      {
        XElement configSection = webConfig.Element("configSections");
        if (configSection == null)
        {
          configSection = new XElement("configSections");
          XElement section = new XElement("section");
          section.Add(new XAttribute("name", "entityFramework"));
          string EFValue = EFVersion == EF5Version ? string.Format(EF5SectionTypeValue, EFVersion) : string.Format(EF6SectionTypeValue, EFVersion);
          section.Add(new XAttribute("type", EFValue));
          section.Add(new XAttribute("requirePermission", "false"));
          configSection.Add(section);
          webConfig.AddFirst(configSection);
        }
        else
        {
          XElement section = new XElement("section");
          section.Add(new XAttribute("name", "entityFramework"));
          string EFValue = EFVersion == EF5Version ? string.Format(EF5SectionTypeValue, EFVersion) : string.Format(EF6SectionTypeValue, EFVersion);
          section.Add(new XAttribute("type", EFValue));
          section.Add(new XAttribute("requirePermission", "false"));
          configSection.Add(section);
        }
      }
    }

    /// <summary>
    /// Creates the Entity Framework Providers section for a web config file.
    /// </summary>
    /// <param name="webConfig">The Xelement web config file.</param>
    /// <param name="EFVersion">The Entity Framework version.</param>
    /// <param name="mySqlVersion">The My SQL version.</param>
    /// <param name="addSqlServerProvider">if set to <c>true</c> [add SQL server provider].</param>
    /// <param name="connectorVersion">The version of the connector/net assembly installed.</param>
    private static void CreateEFProvidersSection(XElement webConfig, string EFVersion, string mySqlVersion, bool addSqlServerProvider)
    {
      if (webConfig != null)
      {
        XElement provider = new XElement("provider");
        provider.Add(new XAttribute("invariantName", "MySql.Data.MySqlClient"));
        string typeValue = string.Empty;
        if (EFVersion == EF5Version)
        {
          typeValue = string.Format(EF5MySQLProviderTypeValue, mySqlVersion);
        }

        if (EFVersion == EF6Version)
        {
          string EF6MySQLProviderTypeValue = string.Format("MySql.Data.MySqlClient.MySqlProviderServices, MySql.Data.{0}, Version={1}, Culture=neutral, PublicKeyToken=c5687fc88969c44d",
                                               _connectorVersion >= new Version(8,0) ? "EntityFramework" : "Entity.EF6",
                                               _connectorVersion.ToString());
          typeValue = string.Format(EF6MySQLProviderTypeValue, mySqlVersion);
        }

        provider.Add(new XAttribute("type", typeValue));
        webConfig.Element("entityFramework").Element("providers").Add(provider);
        if (addSqlServerProvider)
        {
          provider = new XElement("provider");
          provider.Add(new XAttribute("invariantName", "System.Data.SqlClient"));
          provider.Add(new XAttribute("type", SQLServerProviderTypeValue));
          webConfig.Element("entityFramework").Element("providers").Add(provider);
        }
      }
    }

    /// <summary>
    /// Configures the System Data section for a web config file.
    /// </summary>
    /// <param name="webConfig">The XElement web config file.</param>
    /// <param name="EFVersion">The Entity Framework version.</param>
    /// <param name="MySqlVersion">The My SQL version.</param>
    /// <param name="connectorVersion">The version of the connector/net assembly installed.</param>
    private static void ConfigureSystemDataSection(XElement webConfig, string EFVersion, string mySqlVersion, string connectorVersion)
    {
      if (webConfig != null)
      {
        XElement systemData = webConfig.Element("system.data");
        XElement dbProviderFactories = null;
        if (systemData != null)
        {
          dbProviderFactories = webConfig.Element("system.data").Element("DbProviderFactories");
          if (dbProviderFactories == null)
          {
            dbProviderFactories = new XElement("DbProviderFactories");
          }
        }
        else
        {
          systemData = new XElement("system.data");
          dbProviderFactories = new XElement("DbProviderFactories");
        }

        XElement mySQLProviderRemoveElement = new XElement("remove");
        mySQLProviderRemoveElement.Add(new XAttribute("invariant", "MySql.Data.MySqlClient"));
        dbProviderFactories.Add(mySQLProviderRemoveElement);
        XElement mySQLProviderAddElement = new XElement("add");
        mySQLProviderAddElement.Add(new XAttribute("name", "MySQL Data Provider"));
        mySQLProviderAddElement.Add(new XAttribute("invariant", "MySql.Data.MySqlClient"));
        mySQLProviderAddElement.Add(new XAttribute("description", ".Net Framework Data Provider for MySQL"));
        string typeValue = string.Empty;
        if (EFVersion == EF5Version)
        {
          typeValue = string.Format(EF5MySqlClientFactoryTypeValue, mySqlVersion);
        }

        if (EFVersion == EF6Version)
        {
          string EF6MySqlClientFactoryTypeValue = string.Format("MySql.Data.MySqlClient.MySqlClientFactory, MySql.Data, Version={0}, Culture=neutral, PublicKeyToken=c5687fc88969c44d", connectorVersion);
          typeValue = string.Format(EF6MySqlClientFactoryTypeValue, mySqlVersion);
        }

        mySQLProviderAddElement.Add(new XAttribute("type", typeValue));
        dbProviderFactories.Add(mySQLProviderAddElement);
        if (webConfig.Elements("system.data").Elements("DbProviderFactories").FirstOrDefault() == null)
        {
          systemData.Add(dbProviderFactories);
        }

        if (webConfig.Element("system.data") == null)
        {
          webConfig.Add(systemData);
        }
      }
    }
  }
}
