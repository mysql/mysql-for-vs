// Copyright (c) 2009, 2019, Oracle and/or its affiliates. All rights reserved.
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

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Web.Configuration;
using System.Web.Security;
using System.Xml;
using System.Reflection;

namespace MySql.Data.VisualStudio.WebConfig
{
  internal struct SimpleMembershipOptions
  {
    public string UserTableName;
    public string UserIdColumn;
    public string UserNameColumn;
    public bool AutoGenerateTables;
  }

  internal class SimpleMembershipConfig : GenericConfig
  {
    private new SimpleMembershipOptions defaults = new SimpleMembershipOptions();
    private new SimpleMembershipOptions values;
    private bool _mySqlSimpleMembershipFound;
    private string _connectorVerInstalled = string.Empty;

    public SimpleMembershipConfig()
      : base()
    {
      typeName = "MySqlSimpleMembershipProvider";
      sectionName = "membership";
    }

    public SimpleMembershipOptions SimpleMemberOptions
    {
      get { return values; }
      set { values = value; }
    }

    public override void Initialize(AppConfig wc)
    {
      base.Initialize(wc);

      XmlElement e = wc.GetProviderSection(sectionName);
      if (e != null)
      {
        string currentProvider = e.GetAttribute("defaultProvider");
        if (!currentProvider.Equals(typeName, StringComparison.InvariantCultureIgnoreCase))
        {
          base.DefaultProvider = typeName;
          OriginallyEnabled = false;
        }
        else
        {
          base.DefaultProvider = currentProvider;
          OriginallyEnabled = true;
        }
      }

      e = wc.GetProviderElement(sectionName);
      if (e != null)
      {
        if (e.HasAttribute("name"))
        {
          string providerName = e.GetAttribute("name");
          base.values.ProviderName = !OriginallyEnabled ? typeName : providerName;
        }
        if (e.HasAttribute("connectionStringName"))
        {
          string connStrName = e.GetAttribute("connectionStringName");
          base.values.ConnectionStringName = !OriginallyEnabled ? "LocalMySqlServer" : connStrName;
        }
        if (e.HasAttribute("description"))
          base.values.AppDescription = e.GetAttribute("description");
        if (e.HasAttribute("applicationName"))
          base.values.AppName = e.GetAttribute("applicationName");
      }
      base.values.ConnectionString = wc.GetConnectionString(base.values.ConnectionStringName);

      NotInstalled = !_mySqlSimpleMembershipFound;

      Enabled = OriginallyEnabled;

      values = defaults;
      e = wc.GetProviderElement(sectionName);
      if (e == null) return;

      if (e.HasAttribute("userTableName"))
        values.UserTableName = e.GetAttribute("userTableName");
      if (e.HasAttribute("userIdColumn"))
        values.UserIdColumn = e.GetAttribute("userIdColumn");
      if (e.HasAttribute("userNameColumn"))
        values.UserNameColumn = e.GetAttribute("userNameColumn");
      if (e.HasAttribute("autoGenerateTables"))
        values.AutoGenerateTables = Convert.ToBoolean(e.GetAttribute("autoGenerateTables"));
    }

    public override void GetDefaults()
    {
      _mySqlSimpleMembershipFound = MySqlSimpleMembershipFound();

      ProviderSettings providerSet = GetMachineSettings();
      if (providerSet != null)
      {
        base.ProviderType = providerSet.Type.Replace("MySQLMembershipProvider", typeName);
      }
      else
      {
        base.ProviderType = string.Format("MySql.Web.Security.MySqlSimpleMembershipProvider,MySql.Web,Version={0},Culture=neutral,PublicKeyToken=c5687fc88969c44d", _connectorVerInstalled);
      }
      base.defaults.ProviderName = typeName;
      base.defaults.ConnectionStringName = "LocalMySqlServer";
      base.defaults.AppName = "/";
      base.defaults.AppDescription = "MySqlSimpleMembership Application";
      base.defaults.AutoGenSchema = true;
      base.defaults.WriteExceptionToLog = false;
      base.defaults.EnableExpireCallback = false;
    }

    protected override void SaveProvider(XmlElement provider)
    {
      base.SaveProvider(provider);

      provider.SetAttribute("userTableName", values.UserTableName.ToString());
      provider.SetAttribute("userIdColumn", values.UserIdColumn.ToString());
      provider.SetAttribute("userNameColumn", values.UserNameColumn.ToString());
      provider.SetAttribute("autoGenerateTables", values.AutoGenerateTables.ToString());
    }

    protected override ProviderSettings GetMachineSettings()
    {
      Configuration machineConfig = ConfigurationManager.OpenMachineConfiguration();
      MembershipSection section = (MembershipSection)machineConfig.SectionGroups["system.web"].Sections[sectionName];
      foreach (ProviderSettings p in section.Providers)
        if (p.Type.Contains("MySQLMembershipProvider")) return p;
      return null;
    }

    internal bool MySqlSimpleMembershipFound()
    {
       ProviderSettings machineSettings = this.GetMachineSettings();
       if (machineSettings != null)
       {
         var providerType = machineSettings.Type;
         if (providerType != null)
         {
           var name = providerType.ToString();
           if (name.IndexOf("Version=") != -1)
           {
             int startIndex = name.IndexOf("Version=") + 8;
             _connectorVerInstalled = name.Substring(startIndex, name.IndexOf(", Culture") - startIndex);
             Assembly assembly = Assembly.Load(string.Format("MySql.Web, Version={0}, Culture=neutral, PublicKeyToken=c5687fc88969c44d", _connectorVerInstalled));
             if (assembly != null)
             {
               var mysqlSimpleMembership = assembly.GetType("MySql.Web.Security.MySqlSimpleMembershipProvider");
               if (mysqlSimpleMembership != null)
                 return true;
             }
           }
         }
       }
       return false;
    }
  }
}
