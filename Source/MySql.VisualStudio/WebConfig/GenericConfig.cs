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

using System;
using System.Configuration;
using System.Web.Configuration;
using System.Collections;
using System.Xml;

namespace MySql.Data.VisualStudio.WebConfig
{
  internal struct Options
  {
    public string ProviderName;
    public string AppName;
    public string AppDescription;
    public bool WriteExceptionToLog;
    public bool AutoGenSchema;
    public string ConnectionStringName;
    public string ConnectionString;
    public bool EnableExpireCallback;
  }

  internal abstract class GenericConfig
  {
    internal bool NotInstalled = false;
    protected bool OriginallyEnabled;

    protected string sectionName;
    protected string typeName;
    protected abstract ProviderSettings GetMachineSettings();

    public bool Enabled;
    public string DefaultProvider;
    public string ProviderType;

    protected Options defaults = new Options();
    protected Options values;

    public Options GenericOptions
    {
      get { return values; }
      set { values = value; }
    }

    public virtual void GetDefaults()
    {
      ProviderSettings p = GetMachineSettings();
      if (p != null)
      {
        ProviderType = p.Type;
        defaults.ProviderName = p.Name;
        defaults.ConnectionStringName = GetStringValue(p.Parameters["connectionStringName"]);
        defaults.AppName = GetStringValue(p.Parameters["applicationName"]);
        defaults.AppDescription = GetStringValue(p.Parameters["description"]);
        defaults.AutoGenSchema = GetBoolValue(p.Parameters["autogenerateschema"], false);
        defaults.WriteExceptionToLog = GetBoolValue(p.Parameters["writeExceptionsToEventLog"], false);
        defaults.EnableExpireCallback = GetBoolValue(p.Parameters["enableExpireCallback"], false);
      }
    }

    protected string GetStringValue(string s)
    {
      if (String.IsNullOrEmpty(s)) return "";
      return s;
    }

    protected bool GetBoolValue(string s, bool defaultValue)
    {
      if (!String.IsNullOrEmpty(s))
      {
        string lower = s.ToLowerInvariant();
        if (lower == "true" || lower == "false")
          return Convert.ToBoolean(lower);
      }
      return defaultValue;
    }

    public virtual void Initialize(AppConfig wc)
    {
      // first load up the defaults
      GetDefaults();
      values = defaults;
      if (string.IsNullOrEmpty(values.ProviderName))
        NotInstalled = true;

      // get the default provider
      XmlElement e = wc.GetProviderSection(sectionName);
      if (e != null)
      {
        //OrininallyEnabled property should be true just when the current provider is already configured as default
        string currentProvider = e.GetAttribute("defaultProvider");
        if (!currentProvider.Equals(typeName, StringComparison.InvariantCultureIgnoreCase))
        {
          DefaultProvider = typeName;
          OriginallyEnabled = false;
        }
        else
        {
          DefaultProvider = currentProvider;
          OriginallyEnabled = true;
        }
      }

      e = wc.GetProviderElement(sectionName);
      if (e != null)
      {
        //use the ProviderName configured just when is the current Provider, because this will cause conflicts when two providers are in the same section with a different name
        if (e.HasAttribute("name"))
        {
          string providerName = e.GetAttribute("name");
          values.ProviderName = !OriginallyEnabled ? typeName : providerName;
        }
        if (e.HasAttribute("connectionStringName"))
          values.ConnectionStringName = e.GetAttribute("connectionStringName");
        if (e.HasAttribute("description"))
          values.AppDescription = e.GetAttribute("description");
        if (e.HasAttribute("applicationName"))
          values.AppName = e.GetAttribute("applicationName");
        if (e.HasAttribute("writeExceptionsToEventLog"))
          values.WriteExceptionToLog = GetBoolValue(e.GetAttribute("writeExceptionsToEventLog"), false);
        if (e.HasAttribute("autogenerateschema"))
          values.AutoGenSchema = GetBoolValue(e.GetAttribute("autogenerateschema"), false);
        if (e.HasAttribute("enableExpireCallback"))
          values.EnableExpireCallback = GetBoolValue(e.GetAttribute("enableExpireCallback"), false);
      }
      values.ConnectionString = wc.GetConnectionString(values.ConnectionStringName);
      //enable the provider by default just when it was already configured as default
      Enabled = OriginallyEnabled;
      //Enabled = OriginallyEnabled = DefaultProvider != null && (DefaultProvider == values.ProviderName || DefaultProvider == defaults.ProviderName);
    }

    protected virtual void SaveProvider(XmlElement provider)
    {
      provider.SetAttribute("type", ProviderType);
      provider.SetAttribute("applicationName", values.AppName);
      provider.SetAttribute("description", values.AppDescription);
      provider.SetAttribute("connectionStringName", values.ConnectionStringName);
      provider.SetAttribute("writeExceptionsToEventLog", values.WriteExceptionToLog.ToString());
      provider.SetAttribute("autogenerateschema", values.AutoGenSchema.ToString());
      provider.SetAttribute("enableExpireCallback", values.EnableExpireCallback.ToString());
    }

    public virtual void Save(AppConfig wc)
    {
      if (OriginallyEnabled)
        wc.RemoveProvider(sectionName, defaults.ProviderName, values.ProviderName);

      if (!Enabled) return;

      // we need to save our connection strings even if we are using the default
      // provider definition
      wc.SaveConnectionString(defaults.ConnectionStringName, values.ConnectionStringName,
          values.ConnectionString);
      // we do this so our equality comparison that follows can work
      defaults.ConnectionString = values.ConnectionString;

      // we are enabled so we want to set our defaultProvider attribute
      wc.SetDefaultProvider(sectionName, values.ProviderName);

      //we need to skip this validation, because if the user enables a Provider and don't do any changes the tool will add just the provider section without adding the provider configuration
      //if (defaults.Equals(values)) return;

      // our defaults do not equal our new values so we need to redefine our provider
      XmlElement provider = wc.AddProvider(sectionName, defaults.ProviderName, values.ProviderName);

      SaveProvider(provider);
    }
  }
}
