// Copyright © 2014 Oracle and/or its affiliates. All rights reserved.
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
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web.Configuration;
using System.Xml;

namespace MySql.Data.VisualStudio.WebConfig
{
  internal class PersonalizationConfig : GenericConfig
  {

    private bool _membershipEnabled = false;

     public PersonalizationConfig()
      : base()
    {
      typeName = "MySqlPersonalizationProvider";
      sectionName = "webParts";
      ProviderType = "MySql.Web.Security.MySQLPersonalizationProvider";
    }


    protected override ProviderSettings GetMachineSettings()
    {
      Configuration machineConfig = ConfigurationManager.OpenMachineConfiguration();
      WebPartsSection section = (WebPartsSection)machineConfig.SectionGroups["system.web"].Sections[sectionName];
      WebPartsPersonalization p = section.Personalization;

      foreach (ProviderSettings item in p.Providers)
      {
        if (item.Type.Contains(typeName))
          return item;        
      }
      return null;
    }

    public override void Initialize(WebConfig wc)
    {
      GetDefaults();
      values = defaults;
      if (string.IsNullOrEmpty(values.ProviderName))
        NotInstalled = true;


      //Personalization provider needs membership enabled
      XmlElement membership = wc.GetProviderSection("membership");

      if (membership == null || membership.Attributes["defaultProvider"] == null ||
         !membership.Attributes["defaultProvider"].Value.Equals("MySQLMembershipProvider", StringComparison.InvariantCultureIgnoreCase))      
      {
        _membershipEnabled = false;
        return;
      }
      else
        _membershipEnabled = true;

      // get the default provider
      XmlElement e = wc.GetProviderSection(sectionName);
      if (e == null || e.FirstChild == null)
        return;

      var personalizationNode = e.FirstChild as XmlElement;  // move to the <personalization> element
		    
      if (personalizationNode == null || personalizationNode.FirstChild == null)
        return;			
		
      string defaultProvider = personalizationNode.GetAttribute("defaultProvider");

     
      Enabled = OriginallyEnabled = defaultProvider != null &&
           (defaultProvider == values.ProviderName || defaultProvider == defaults.ProviderName);

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
     
    }

    public override void Save(WebConfig wc)
    {

      if (!_membershipEnabled)
        return;

      if (OriginallyEnabled)
      {
        XmlElement e = wc.GetProviderSection(sectionName);
        e.ParentNode.RemoveChild(e);
      }

      if (!Enabled) return;

      // we need to save our connection strings even if we are using the default
      // provider definition
      wc.SaveConnectionString(defaults.ConnectionStringName, values.ConnectionStringName,
          values.ConnectionString);
      
      defaults.ConnectionString = values.ConnectionString;

      XmlElement provider = wc.AddProvider(sectionName, defaults.ProviderName, values.ProviderName);

      var webPartsNode = wc.GetSystemWebNode(sectionName, true, false) as XmlElement;
      var personalization = webPartsNode.FirstChild as XmlElement;
      personalization.SetAttribute("defaultProvider", defaults.ProviderName);

      SaveProvider(provider);
      
    }
  }
}
