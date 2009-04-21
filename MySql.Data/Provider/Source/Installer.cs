// Copyright (c) 2004-2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
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

using System.Configuration.Install;
using System.ComponentModel;
using System.Reflection;
using System;
using Microsoft.Win32;
using System.Xml;
using System.IO;
using System.Diagnostics;
using System.Security;
using System.Security.Permissions;

namespace MySql.Data.MySqlClient
{
	/// <summary>
	/// We are adding a custom installer class to our assembly so our installer
	/// can make proper changes to the machine.config file.
	/// </summary>
    [RunInstaller(true)]
	public class CustomInstaller : Installer
	{
		/// <summary>
		/// We override Install so we can add our assembly to the proper
		/// machine.config files.
		/// </summary>
		/// <param name="stateSaver"></param>
		public override void Install(System.Collections.IDictionary stateSaver)
		{
			base.Install(stateSaver);

            AddProviderToMachineConfig();
		}

		private static void AddProviderToMachineConfig()
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

		private static void AddProviderToMachineConfigInDir(string path)
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

			// create our new node
			XmlElement newNode = (XmlElement)doc.CreateNode(XmlNodeType.Element, "add", "");

			// add the proper attributes
			newNode.SetAttribute("name", "MySQL Data Provider");
			newNode.SetAttribute("invariant", "MySql.Data.MySqlClient");
			newNode.SetAttribute("description", ".Net Framework Data Provider for MySQL");

			// add the type attribute by reflecting on the executing assembly
			Assembly a = Assembly.GetExecutingAssembly();
			string type = String.Format("MySql.Data.MySqlClient.MySqlClientFactory, {0}", a.FullName);
			newNode.SetAttribute("type", type);

			XmlNodeList nodes = doc.GetElementsByTagName("DbProviderFactories");

			foreach (XmlNode node in nodes[0].ChildNodes)
			{
                if (node.Attributes == null) continue;
                foreach (XmlAttribute attr in node.Attributes)
                {
                    if (attr.Name == "invariant" && attr.Value == "MySql.Data.MySqlClient")
                    {
                        nodes[0].RemoveChild(node);
                        break;
                    }
                }
            }

    		nodes[0].AppendChild(newNode);

			// Save the document to a file and auto-indent the output.
			XmlTextWriter writer = new XmlTextWriter(configPath, null);
			writer.Formatting = Formatting.Indented;
			doc.Save(writer);
            writer.Flush();
            writer.Close();
		}

		/// <summary>
		/// We override Uninstall so we can remove out assembly from the
		/// machine.config files.
		/// </summary>
		/// <param name="savedState"></param>
		public override void Uninstall(System.Collections.IDictionary savedState)
		{
			base.Uninstall(savedState);

			RemoveProviderFromMachineConfig();
		}

		private static void RemoveProviderFromMachineConfig()
		{
			object installRoot = Registry.GetValue(
				@"HKEY_LOCAL_MACHINE\Software\Microsoft\.NETFramework\",
				"InstallRoot", null);
			if (installRoot == null)
				throw new Exception("Unable to retrieve install root for .NET framework");


			RemoveProviderFromMachineConfigInDir(installRoot.ToString());

			string installRoot64 = installRoot.ToString();
			installRoot64 = installRoot64.Substring(0, installRoot64.Length - 1);
			installRoot64 = string.Format("{0}64{1}", installRoot64,
				Path.DirectorySeparatorChar);
			if (Directory.Exists(installRoot64))
				RemoveProviderFromMachineConfigInDir(installRoot64);
		}

		private static void RemoveProviderFromMachineConfigInDir(string path)
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

			XmlNodeList nodes = doc.GetElementsByTagName("DbProviderFactories");
			foreach (XmlNode node in nodes[0].ChildNodes)
			{
                if (node.Attributes == null) continue;
				string name = node.Attributes["name"].Value;
				if (name == "MySQL Data Provider")
				{
					nodes[0].RemoveChild(node);
					break;
				}
			}

			// Save the document to a file and auto-indent the output.
			XmlTextWriter writer = new XmlTextWriter(configPath, null);
			writer.Formatting = Formatting.Indented;
			doc.Save(writer);
            writer.Flush();
            writer.Close();
        }
	}
}

#endif
