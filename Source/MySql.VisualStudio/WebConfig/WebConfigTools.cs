// Copyright © 2015, Oracle and/or its affiliates. All rights reserved.
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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MySql.Data.VisualStudio.WebConfig
{
  public static class WebConfigTools
  {
    private const string EF5Version = "5.0.0";
    private const string EF6Version = "6.1.3";
    private const string defaultConnectionFactoryEF5TypeValue = "MySql.Data.Entity.MySqlConnectionFactory, MySql.Data.Entity.EF5";
    private const string defaultConnectionFactoryEF6TypeValue = "MySql.Data.Entity.MySqlConnectionFactory, MySql.Data.Entity.EF6";
    private const string EF5SectionTypeValue = "System.Data.Entity.Internal.ConfigFile.EntityFrameworkSection, EntityFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
    private const string EF6SectionTypeValue = "System.Data.Entity.Internal.ConfigFile.EntityFrameworkSection, EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
    private const string EF5MySQLProviderTypeValue = "MySql.Data.MySqlClient.MySqlProviderServices, MySql.Data.Entity.EF5, Version=6.7.8.0, Culture=neutral, PublicKeyToken=c5687fc88969c44d";
    private const string EF6MySQLProviderTypeValue = "MySql.Data.MySqlClient.MySqlProviderServices, MySql.Data.Entity.EF6, Version=6.9.6.0, Culture=neutral, PublicKeyToken=c5687fc88969c44d";
    private const string SQLServerProviderTypeValue = "System.Data.Entity.SqlServer.SqlProviderServices, EntityFramework.SqlServer";
    private const string EF5MySqlClientFactoryTypeValue = "MySql.Data.MySqlClient.MySqlClientFactory, MySql.Data, Version=6.7.8.0, Culture=neutral, PublicKeyToken=c5687fc88969c44d";
    private const string EF6MySqlClientFactoryTypeValue = "MySql.Data.MySqlClient.MySqlClientFactory, MySql.Data, Version=6.9.6.0, Culture=neutral, PublicKeyToken=c5687fc88969c44d";
    private const string webConfigFileName = "web.config";
    /// <summary>
    /// Transforms a config file in order to add the Entity Framework settings to it.
    /// </summary>
    /// <param name="projectPath">The project path.</param>
    /// <param name="EFVersion">The Entity Framework version.</param>
    /// <param name="mySQLVersion">The My SQL version.</param>
    public static void EFWebConfigTransformation(string projectPath, string EFVersion, string mySQLVersion)
    {
      if (!string.IsNullOrEmpty(projectPath) && File.Exists(Path.Combine(projectPath, webConfigFileName)))
      {
        XElement webConfig = XElement.Load(Path.Combine(projectPath,  webConfigFileName));
        RemoveEFSettings(webConfig);
        ConfigureEntityFrameworkSection(webConfig, EFVersion, mySQLVersion);
        ConfigureSystemDataSection(webConfig, EFVersion, mySQLVersion);
        webConfig.Save(Path.Combine(projectPath, webConfigFileName));
      }
    }

    /// <summary>
    /// Removes all the Entity Framework configuration from a config file.
    /// </summary>
    /// <param name="projectPath">The project path.</param>
    public static void RemoveEFWebConfig(string projectPath)
    {
      if (!string.IsNullOrEmpty(projectPath) && File.Exists(Path.Combine(projectPath, webConfigFileName)))
      {
        XElement webConfig = XElement.Load(Path.Combine(projectPath, webConfigFileName));
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

        webConfig.Save(Path.Combine(projectPath, webConfigFileName));
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
          defaultConnectionFactory.Add(new XAttribute("type", defaultConnectionFactoryEF6TypeValue));
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
    private static void ConfigureSystemDataSection(XElement webConfig, string EFVersion, string MySqlVersion)
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
          typeValue = string.Format(EF5MySqlClientFactoryTypeValue, MySqlVersion);
        }

        if (EFVersion == EF6Version)
        {
          typeValue = string.Format(EF6MySqlClientFactoryTypeValue, MySqlVersion);
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
