// Copyright � 2004-2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
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

#if !MONO && !PocketPC
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Collections;
using System.IO;
using Microsoft.Win32;
using System.Xml;
using System.Reflection;

namespace MySql.Web.Security
{
    /// <summary>
    /// 
    /// </summary>
    [RunInstaller(true)]
    public class CustomInstaller : Installer
    {
        /// <summary>
        /// When overridden in a derived class, performs the installation.
        /// </summary>
        /// <param name="stateSaver">An <see cref="T:System.Collections.IDictionary"/> used to save information needed to perform a commit, rollback, or uninstall operation.</param>
        /// <exception cref="T:System.ArgumentException">
        /// The <paramref name="stateSaver"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Exception">
        /// An exception occurred in the <see cref="E:System.Configuration.Install.Installer.BeforeInstall"/> event handler of one of the installers in the collection.
        /// -or-
        /// An exception occurred in the <see cref="E:System.Configuration.Install.Installer.AfterInstall"/> event handler of one of the installers in the collection.
        /// </exception>
        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);

            AddProviderToMachineConfig();
        }

        /// <summary>
        /// When overridden in a derived class, removes an installation.
        /// </summary>
        /// <param name="savedState">An <see cref="T:System.Collections.IDictionary"/> that contains the state of the computer after the installation was complete.</param>
        /// <exception cref="T:System.ArgumentException">
        /// The saved-state <see cref="T:System.Collections.IDictionary"/> might have been corrupted.
        /// </exception>
        /// <exception cref="T:System.Configuration.Install.InstallException">
        /// An exception occurred while uninstalling. This exception is ignored and the uninstall continues. However, the application might not be fully uninstalled after the uninstallation completes.
        /// </exception>
        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);

            RemoveProviderFromMachineConfig();
        }

        private void AddProviderToMachineConfig()
        {
            object installRoot = Registry.GetValue(
                @"HKEY_LOCAL_MACHINE\Software\Microsoft\.NETFramework\",
                "InstallRoot", null);
            if (installRoot == null)
                throw new Exception("Unable to retrieve install root for .NET framework");

            AddProviderToMachineConfigInDir(installRoot.ToString());

            string installRoot64 = installRoot.ToString();
            installRoot64 = installRoot64.Substring(0, installRoot64.Length - 1);
            installRoot64 = string.Format("{0}64{1}", installRoot64,
                Path.DirectorySeparatorChar);
            if (Directory.Exists(installRoot64))
                AddProviderToMachineConfigInDir(installRoot64);
        }

        private void AddProviderToMachineConfigInDir(string path)
        {
            string configPath = String.Format(@"{0}v2.0.50727\CONFIG\machine.config",
                path);

            // now read the config file into memory
            StreamReader sr = new StreamReader(configPath);
            string configXML = sr.ReadToEnd();
            sr.Close();

            // load the XML into the XmlDocument
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(configXML);

            AddDefaultConnectionString(doc);
            AddMembershipProvider(doc);
            AddRoleProvider(doc);
            AddProfileProvider(doc);

            // Save the document to a file and auto-indent the output.
            XmlTextWriter writer = new XmlTextWriter(configPath, null);
            writer.Formatting = Formatting.Indented;
            doc.Save(writer);
            writer.Flush();
            writer.Close();
        }

        private void AddDefaultConnectionString(XmlDocument doc)
        {
            // create our new node
            XmlElement newNode = (XmlElement)doc.CreateNode(XmlNodeType.Element, "add", "");

            // add the proper attributes
            newNode.SetAttribute("name", "LocalMySqlServer");
            newNode.SetAttribute("connectionString", "");

            XmlNodeList nodes = doc.GetElementsByTagName("connectionStrings");
            XmlNode connectionStringList = nodes[0];

            bool alreadyThere = false;
            foreach (XmlNode node in connectionStringList.ChildNodes)
            {
                string nameValue = node.Attributes["name"].Value;
                if (nameValue == "LocalMySqlServer")
                {
                    alreadyThere = true;
                    break;
                }
            }

            if (!alreadyThere)
                connectionStringList.AppendChild(newNode);
        }

        private void AddMembershipProvider(XmlDocument doc)
        {
            // create our new node
            XmlElement newNode = (XmlElement)doc.CreateNode(XmlNodeType.Element, "add", "");

            // add the proper attributes
            newNode.SetAttribute("name", "MySQLMembershipProvider");

            // add the type attribute by reflecting on the executing assembly
            Assembly a = Assembly.GetExecutingAssembly();
            string type = String.Format("MySql.Web.Security.MySQLMembershipProvider, {0}", a.FullName);
            newNode.SetAttribute("type", type);

            newNode.SetAttribute("connectionStringName", "LocalMySqlServer");
            newNode.SetAttribute("enablePasswordRetrieval", "false");
            newNode.SetAttribute("enablePasswordReset", "true");
            newNode.SetAttribute("requiresQuestionAndAnswer", "true");
            newNode.SetAttribute("applicationName", "/");
            newNode.SetAttribute("requiresUniqueEmail", "false");
            newNode.SetAttribute("passwordFormat", "Clear");
            newNode.SetAttribute("maxInvalidPasswordAttempts", "5");
            newNode.SetAttribute("minRequiredPasswordLength", "7");
            newNode.SetAttribute("minRequiredNonalphanumericCharacters", "1");
            newNode.SetAttribute("passwordAttemptWindow", "10");
            newNode.SetAttribute("passwordStrengthRegularExpression", "");

            XmlNodeList nodes = doc.GetElementsByTagName("membership");
            XmlNode providerList = nodes[0].FirstChild;

            foreach (XmlNode node in providerList.ChildNodes)
            {
                string typeValue = node.Attributes["type"].Value;
                if (typeValue.StartsWith("MySql.Web.Security.MySQLMembershipProvider"))
                {
                    providerList.RemoveChild(node);
                    break;
                }
            }

            providerList.AppendChild(newNode);
        }

        private void AddRoleProvider(XmlDocument doc)
        {
            // create our new node
            XmlElement newNode = (XmlElement)doc.CreateNode(XmlNodeType.Element, "add", "");

            // add the proper attributes
            newNode.SetAttribute("name", "MySQLRoleProvider");

            // add the type attribute by reflecting on the executing assembly
            Assembly a = Assembly.GetExecutingAssembly();
            string type = String.Format("MySql.Web.Security.MySQLRoleProvider, {0}", a.FullName);
            newNode.SetAttribute("type", type);

            newNode.SetAttribute("connectionStringName", "LocalMySqlServer");
            newNode.SetAttribute("applicationName", "/");

            XmlNodeList nodes = doc.GetElementsByTagName("roleManager");
            XmlNode providerList = nodes[0].FirstChild;

            foreach (XmlNode node in providerList.ChildNodes)
            {
                string typeValue = node.Attributes["type"].Value;
                if (typeValue.StartsWith("MySql.Web.Security.MySQLRoleProvider"))
                {
                    providerList.RemoveChild(node);
                    break;
                }
            }

            providerList.AppendChild(newNode);
        }

        private void AddProfileProvider(XmlDocument doc)
        {
            // create our new node
            XmlElement newNode = (XmlElement)doc.CreateNode(XmlNodeType.Element, "add", "");

            // add the proper attributes
            newNode.SetAttribute("name", "MySQLProfileProvider");

            // add the type attribute by reflecting on the executing assembly
            Assembly a = Assembly.GetExecutingAssembly();
            string type = String.Format("MySql.Web.Profile.MySQLProfileProvider, {0}", a.FullName);
            newNode.SetAttribute("type", type);

            newNode.SetAttribute("connectionStringName", "LocalMySqlServer");
            newNode.SetAttribute("applicationName", "/");

            XmlNodeList nodes = doc.GetElementsByTagName("profile");
            XmlNode providerList = nodes[0].FirstChild;

            foreach (XmlNode node in providerList.ChildNodes)
            {
                string typeValue = node.Attributes["type"].Value;
                if (typeValue.StartsWith("MySql.Web.Profile.MySQLProfileProvider"))
                {
                    providerList.RemoveChild(node);
                    break;
                }
            }

            providerList.AppendChild(newNode);
        }

        private void RemoveProviderFromMachineConfig()
        {
            object installRoot = Registry.GetValue(
                @"HKEY_LOCAL_MACHINE\Software\Microsoft\.NETFramework\",
                "InstallRoot", null);
            if (installRoot == null)
                throw new Exception("Unable to retrieve install root for .NET framework");

            string installRoot64 = installRoot.ToString();
            installRoot64 = installRoot64.Substring(0, installRoot64.Length - 1);
            installRoot64 = string.Format("{0}64{1}", installRoot64,
                Path.DirectorySeparatorChar);
            if (Directory.Exists(installRoot64))
                RemoveProviderFromMachineConfigInDir(installRoot64);

            RemoveProviderFromMachineConfigInDir(installRoot.ToString());
        }

        private void RemoveProviderFromMachineConfigInDir(string path)
        {
            string configPath = String.Format(@"{0}v2.0.50727\CONFIG\machine.config",
                path);

            // now read the config file into memory
            StreamReader sr = new StreamReader(configPath);
            string configXML = sr.ReadToEnd();
            sr.Close();

            // load the XML into the XmlDocument
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(configXML);

            RemoveDefaultConnectionString(doc);
            RemoveMembershipProvider(doc);
            RemoveRoleProvider(doc);
            RemoveProfileProvider(doc);

            // Save the document to a file and auto-indent the output.
            XmlTextWriter writer = new XmlTextWriter(configPath, null);
            writer.Formatting = Formatting.Indented;
            doc.Save(writer);
            writer.Flush();
            writer.Close();
        }

        private void RemoveDefaultConnectionString(XmlDocument doc)
        {
            XmlNodeList nodes = doc.GetElementsByTagName("connectionStrings");
            XmlNode connectionStringList = nodes[0];
            foreach (XmlNode node in connectionStringList.ChildNodes)
            {
                string name = node.Attributes["name"].Value;
                if (name == "LocalMySqlServer")
                {
                    connectionStringList.RemoveChild(node);
                    break;
                }
            }
        }

        private void RemoveMembershipProvider(XmlDocument doc)
        {
            XmlNodeList nodes = doc.GetElementsByTagName("membership");
            XmlNode providersNode = nodes[0].FirstChild;
            foreach (XmlNode node in providersNode.ChildNodes)
            {
                string name = node.Attributes["name"].Value;
                if (name == "MySQLMembershipProvider")
                {
                    providersNode.RemoveChild(node);
                    break;
                }
            }
        }

        private void RemoveRoleProvider(XmlDocument doc)
        {
            XmlNodeList nodes = doc.GetElementsByTagName("roleManager");
            XmlNode providersNode = nodes[0].FirstChild;
            foreach (XmlNode node in providersNode.ChildNodes)
            {
                string name = node.Attributes["name"].Value;
                if (name == "MySQLRoleProvider")
                {
                    providersNode.RemoveChild(node);
                    break;
                }
            }
        }

        private void RemoveProfileProvider(XmlDocument doc)
        {
            XmlNodeList nodes = doc.GetElementsByTagName("profile");
            XmlNode providersNode = nodes[0].FirstChild;
            foreach (XmlNode node in providersNode.ChildNodes)
            {
                string name = node.Attributes["name"].Value;
                if (name == "MySQLProfileProvider")
                {
                    providersNode.RemoveChild(node);
                    break;
                }
            }
        }
    }
}

#endif