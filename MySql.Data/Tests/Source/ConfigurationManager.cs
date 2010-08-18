// Copyright (c) 2004-2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
//
// MySQL Connector/NET is licensed under the terms of the GPLv2
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
