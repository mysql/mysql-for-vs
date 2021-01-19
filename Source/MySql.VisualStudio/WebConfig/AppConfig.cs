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
using System.Xml;
using System.Configuration;
using System.Web.Configuration;
using System.Web.Security;
using System.Diagnostics;
using System.Web.Profile;
using System.Collections.Generic;

namespace MySql.Data.VisualStudio.WebConfig
{
  internal class AppConfig
  {
    /// <summary>
    /// The name of the configuration file for the current project.
    /// </summary>
    private string _configFile;
    private XmlDocument webDoc;

    public AppConfig(string filename)
    {
      _configFile = filename;
      Initialize();
    }

    private void Initialize()
    {
      if (webDoc == null && System.IO.File.Exists(_configFile))
      {
        webDoc = new XmlDocument();
        webDoc.Load(_configFile);
      }
    }

    public void Save()
    {
      webDoc.Save(_configFile);
    }

    public XmlElement GetProviderSection(string type)
    {
      if (webDoc == null) return null;
      XmlNodeList nodes = webDoc.GetElementsByTagName(type);
      if (nodes.Count == 0) return null;
      return nodes[0] as XmlElement;
    }

    public XmlElement GetProviderElement(string type)
    {
      XmlNode el = GetSystemWebNode(type, false, false);
      if (el == null || el.FirstChild == null) return null;
      el = el.FirstChild;  // move to the <providers> element

      if (type.Equals("webParts", StringComparison.InvariantCultureIgnoreCase))
      {
        if (el.ChildNodes.Count > 0)
          el = el.FirstChild;
      }

      if (el.ChildNodes.Count == 0) return null;

      foreach (XmlNode node in el.ChildNodes)
      {
        if (string.Compare(node.Name, "remove", true) == 0 ||
            string.Compare(node.Name, "clear", true) == 0) continue;
        if (node.Attributes != null && node.Attributes.Count > 0)
        {
          string typeName = node.Attributes["type"].Value;
          if (typeName.StartsWith("MySql.Web.", StringComparison.OrdinalIgnoreCase)) return node as XmlElement;
        }
      }

      return null;
    }

    public XmlElement GetListItem(string topNode, string nodeName, string itemName)
    {
      Debug.Assert(webDoc != null);
      XmlNodeList nodes = webDoc.GetElementsByTagName(topNode);
      if (nodes.Count == 0) return null;

      // nodeName == null just means return the top node
      XmlNode node = nodes[0];
      if (nodeName == null)
        return node as XmlElement;

      // we are looking for something lower but there is nothing here
      if (node.ChildNodes.Count == 0) return null;

      // if we are looking in a provider list, then step over the providers element
      if (node.FirstChild.Name == "providers")
        node = node.FirstChild;

      foreach (XmlNode child in node.ChildNodes)
      {
        if (child.Name != nodeName) continue;
        if (String.Compare(child.Attributes["name"].Value, itemName, true) == 0)
          return child as XmlElement;
      }
      return null;
    }

    public string GetConnectionString(string name)
    {
      XmlElement el = GetListItem("connectionStrings", "add", name);
      if (el == null) return null;
      return el.Attributes["connectionString"].Value;
    }

    public void SaveConnectionString(string defaultName, string name, string connectionString)
    {
      Debug.Assert(webDoc != null);
      XmlNode connStrNode = null;

      XmlNodeList nodes = webDoc.GetElementsByTagName("connectionStrings");
      if (nodes.Count == 0)
      {
        XmlNode topNode = webDoc.GetElementsByTagName("configuration")[0];
        connStrNode = webDoc.CreateElement("connectionStrings");
        XmlNode syswebElement = webDoc.GetElementsByTagName("system.web")[0];
        topNode.InsertBefore(connStrNode, syswebElement);
      }
      else
        connStrNode = nodes[0];

      // remove all traces of the old connection strings
      RemoveConnectionString(connStrNode, name);

      if (defaultName == name)
      {
        XmlElement remove = webDoc.CreateElement("remove");
        remove.SetAttribute("name", defaultName);
        connStrNode.AppendChild(remove);
      }

      XmlElement add = webDoc.CreateElement("add");
      add.SetAttribute("name", name);
      add.SetAttribute("connectionString", connectionString);
      add.SetAttribute("providerName", "MySql.Data.MySqlClient");
      connStrNode.AppendChild(add);
    }

    private void RemoveConnectionString(XmlNode parentNode, string name)
    {
      List<XmlNode> toBeDeleted = new List<XmlNode>();

      foreach (XmlNode node in parentNode.ChildNodes)
      {
        if (node.Attributes != null && String.Compare(node.Attributes["name"].Value, name, true) == 0)
          toBeDeleted.Add(node);
      }
      foreach (XmlNode node in toBeDeleted)
        parentNode.RemoveChild(node);
    }

    public void SetDefaultProvider(string sectionName, string providerName)
    {
      XmlElement e = GetSystemWebNode(sectionName, true, false) as XmlElement;
      e.SetAttribute("defaultProvider", providerName);
    }

    public void RemoveProvider(string sectionName, string defaultName, string name)
    {
      XmlElement section = GetProviderSection(sectionName);
      if (section == null) return;

      section.RemoveAttribute("defaultProvider");

      if (section.FirstChild == null) return;
      XmlElement providers = section.FirstChild as XmlElement;

      List<XmlNode> toBeDeleted = new List<XmlNode>();
      foreach (XmlNode node in providers.ChildNodes)
      {
        if (String.Compare("clear", node.Name, true) == 0) continue;
        string nodeName = node.Attributes["name"].Value;
        if ((node.Name == "remove" && String.Compare(nodeName, defaultName, true) == 0) ||
            String.Compare(nodeName, name, true) == 0)
          toBeDeleted.Add(node);
      }
      foreach (XmlNode node in toBeDeleted)
        providers.RemoveChild(node);
      if (providers.ChildNodes.Count == 0)
        section.ParentNode.RemoveChild(section);
    }

    public XmlNode GetSystemWebNode(string name, bool createTopNode, bool createProvidersNode)
    {
      XmlNode webNode = null;
      XmlNode systemWebNode = webDoc.GetElementsByTagName("system.web")[0];
      if (systemWebNode == null)
        return null;

      foreach (XmlNode node in systemWebNode.ChildNodes)
        if (node.Name == name)
        {
          webNode = node;
          break;
        }
      if (webNode == null && createTopNode)
      {
        webNode = (XmlNode)webDoc.CreateElement(name);
        if (webNode.ChildNodes.Count == 0 && name.Equals("webParts", StringComparison.InvariantCultureIgnoreCase))
        {
          var personalizationNode = (XmlElement)webDoc.CreateNode(XmlNodeType.Element, "personalization", "");
          webNode.AppendChild(personalizationNode);                 
        }               
        systemWebNode.InsertBefore(webNode, systemWebNode.FirstChild);        
      }
      if (createProvidersNode && webNode != null)
      {
        if (webNode.ChildNodes.Count >0 && name.Equals("webParts", StringComparison.InvariantCultureIgnoreCase))
        {
          webNode = webNode.FirstChild; //locate on personalization section
        }

        //verify if the system.web node already has the providers node, if not exists then add it
        if(!HasNode(webNode, "providers"))
          webNode.AppendChild(webDoc.CreateElement("providers"));
      }
      return webNode;
    }

    private bool HasNode(XmlNode parentNode, string nodeName)
    {
      foreach (XmlNode childNode in parentNode.ChildNodes)
      {
        if (childNode.Name.Equals(nodeName, StringComparison.InvariantCultureIgnoreCase))
          return true;
      }
      return false;
    }

    private string GetDefaultRoleProvider()
    {
      XmlElement el = (XmlElement)GetSystemWebNode("roleManager", false, false);
      if (el == null) return null;
      if (!el.HasAttribute("defaultProvider")) return null;
      return el.Attributes["defaultProvider"].Value;
    }

    public XmlElement AddProvider(string sectionName, string defaultName, string name)
    {
      XmlElement e = (XmlElement)GetSystemWebNode(sectionName, true, true);

      if (e == null)
        return null;

      e = e.FirstChild as XmlElement;

      // if we are adding a provider def with the same name as default then we
      // need to remove the default
      if (String.Compare(defaultName, name, true) == 0)
      {
        XmlElement remove = webDoc.CreateElement("remove");
        remove.SetAttribute("name", defaultName);
        e.AppendChild(remove);
      }

      XmlElement add = webDoc.CreateElement("add");
      add.SetAttribute("name", name);
      e.AppendChild(add);
      return add;
    }

  }
}
