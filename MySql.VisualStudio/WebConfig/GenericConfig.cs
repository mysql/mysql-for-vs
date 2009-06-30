// Copyright (c) 2009 Sun Microsystems, Inc.
//
// This file is part of MySQL Tools for Visual Studio.
// MySQL Tools for Visual Studio is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public 
// License version 2.1 as published by the Free Software Foundation
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

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
    }

    internal abstract class GenericConfig
    {
        private bool OriginallyEnabled;

        protected string sectionName;
        protected string typeName;
        protected abstract ProviderSettings GetMachineSettings();

        public bool Enabled;
        public string DefaultProvider;
        public string ProviderType;

        protected Options defaults = new Options();
        private Options values;

        public Options GenericOptions
        {
            get { return values; }
            set { values = value; }
        }

        public virtual void GetDefaults()
        {
            ProviderSettings p = GetMachineSettings();
            ProviderType = p.Type;

            defaults.ProviderName = p.Name;
            defaults.ConnectionStringName = GetStringValue(p.Parameters["connectionStringName"]);
            defaults.AppName = GetStringValue(p.Parameters["applicationName"]);
            defaults.AppDescription = GetStringValue(p.Parameters["description"]);
            defaults.AutoGenSchema = GetBoolValue(p.Parameters["autogenerateschema"], false);
            defaults.WriteExceptionToLog = GetBoolValue(p.Parameters["writeExceptionsToEventLog"], false);
        }

        private string GetStringValue(string s)
        {
            if (String.IsNullOrEmpty(s)) return "";
            return s;
        }

        private bool GetBoolValue(string s, bool defaultValue)
        {
            if (!String.IsNullOrEmpty(s))
            {
                string lower = s.ToLowerInvariant();
                if (lower == "true" || lower == "false")
                    return Convert.ToBoolean(lower);
            }
            return defaultValue;
        }

        public virtual void Initialize(WebConfig wc)
        {
            // first load up the defaults
            GetDefaults();
            values = defaults;

            // get the default provider
            XmlElement e = wc.GetProviderSection(sectionName);
            if (e != null)
                DefaultProvider = e.GetAttribute("defaultProvider");

            e = wc.GetProviderElement(sectionName);
            if (e != null)
            {
                values.ProviderName = e.GetAttribute("name");
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
            }
            values.ConnectionString = wc.GetConnectionString(values.ConnectionStringName);
            Enabled = OriginallyEnabled = (DefaultProvider == values.ProviderName ||
                DefaultProvider == defaults.ProviderName);
        }

        protected virtual void SaveProvider(XmlElement provider)
        {
            provider.SetAttribute("type", ProviderType);
            provider.SetAttribute("applicationName", values.AppName);
            provider.SetAttribute("description", values.AppDescription);
            provider.SetAttribute("connectionStringName", values.ConnectionStringName);
            provider.SetAttribute("writeExceptionsToEventLog", values.WriteExceptionToLog.ToString());
            provider.SetAttribute("autogenerateschema", values.AutoGenSchema.ToString());
        }

        public virtual void Save(WebConfig wc)
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

            if (defaults.Equals(values)) return;

            // our defaults do not equal our new values so we need to redefine our provider
            XmlElement provider = wc.AddProvider(sectionName, defaults.ProviderName, values.ProviderName);

            SaveProvider(provider);
        }
    }
}
