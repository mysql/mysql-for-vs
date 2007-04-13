// Copyright (C) 2004-2007 MySQL AB
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 2 as published by
// the Free Software Foundation
//
// There are special exceptions to the terms and conditions of the GPL 
// as it is applied to this software. View the full text of the 
// exception in file EXCEPTIONS in the directory of this software 
// distribution.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.IO;
using System.Collections.Specialized;
using System.Reflection;
using System.Xml;

namespace MySql.Data.MySqlClient.Tests
{
    public static class ConfigurationManager
    {
        private static string configurationFile;
        private static NameValueCollection appSettings = new NameValueCollection();

        public static NameValueCollection AppSettings
        {
            get { return appSettings; }
        }

        static ConfigurationManager()
        {
            ConfigurationManager.configurationFile = String.Format("{0}.config", 
                System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);

            if (!File.Exists(ConfigurationManager.configurationFile))
            {
                throw new FileNotFoundException(String.Format(
                    "Configuration file ({0}) not be found.", 
                    ConfigurationManager.configurationFile));
            }

            XmlDocument configXmlDocument = new XmlDocument();
            configXmlDocument.Load(ConfigurationManager.configurationFile);

            // Add keys and values to the AppSettings NameValueCollection
            foreach (XmlNode node in configXmlDocument.SelectNodes("/configuration/appSettings/add"))
            {
                ConfigurationManager.AppSettings.Add(
                    node.Attributes["key"].Value, 
                    node.Attributes["value"].Value);
            }
        }

        public static void Save()
        {
            XmlDocument configXmlDocument = new XmlDocument();
            configXmlDocument.Load(ConfigurationManager.configurationFile);

            XmlNode node = configXmlDocument.SelectSingleNode("/configuration/appSettings");

            if (node != null)
            {
                // Remove all previous appSetting nodes
                node.RemoveAll();

                foreach (string key in AppSettings.AllKeys)
                {
                    // Create a new appSetting node
                    XmlElement appNode = configXmlDocument.CreateElement("add");

                    // Create the key attribute and assign its value
                    XmlAttribute keyAttribute = configXmlDocument.CreateAttribute("key");
                    keyAttribute.Value = key;

                    // Create the value attribute and assign its value
                    XmlAttribute valueAttribute = configXmlDocument.CreateAttribute("value");
                    valueAttribute.Value = AppSettings[key];

                    // Append the key and value attribute to the appSetting node
                    appNode.Attributes.Append(keyAttribute);
                    appNode.Attributes.Append(valueAttribute);

                    // Append the appSetting node to the appSettings node
                    node.AppendChild(appNode);
                }
            }

            // Save config file
            configXmlDocument.Save(ConfigurationManager.configurationFile);
        }
    }
}
