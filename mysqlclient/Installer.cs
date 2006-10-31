using System.Configuration.Install;
using System.ComponentModel;
using System.Reflection;
using System;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Xml;
using System.IO;
using System.Diagnostics;

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
			InstallPerfMonItems();
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

			bool alreadyThere = false;
			foreach (XmlNode node in nodes[0].ChildNodes)
			{
				string typeValue = node.Attributes["type"].Value;
				if (typeValue == type)
				{
					alreadyThere = true;
					break;
				}
			}

			if (! alreadyThere)
				nodes[0].AppendChild(newNode);

			// Save the document to a file and auto-indent the output.
			XmlTextWriter writer = new XmlTextWriter(configPath, null);
			writer.Formatting = Formatting.Indented;
			doc.Save(writer);
		}

		private void InstallPerfMonItems()
		{
			string categoryName = Resources.PerfMonCategoryName;

			if (!PerformanceCounterCategory.Exists(categoryName))
			{
				CounterCreationDataCollection ccdc = new CounterCreationDataCollection();
				ccdc.Add(new CounterCreationData(Resources.PerfMonHardProcName,
					Resources.PerfMonHardProcHelp, PerformanceCounterType.NumberOfItems32));
				ccdc.Add(new CounterCreationData(Resources.PerfMonSoftProcName,
					Resources.PerfMonSoftProcHelp, PerformanceCounterType.NumberOfItems32));
				PerformanceCounterCategory.Create(categoryName, Resources.PerfMonCategoryHelp,
					PerformanceCounterCategoryType.SingleInstance, ccdc);
			}
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
			RemovePerfMonItems();
		}

		private void RemoveProviderFromMachineConfig()
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

			XmlNodeList nodes = doc.GetElementsByTagName("DbProviderFactories");
			foreach (XmlNode node in nodes[0].ChildNodes)
			{
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
		}

		private void RemovePerfMonItems()
		{
			string categoryName = Resources.PerfMonCategoryName;

			// TODO: add code to inspect registry and make sure no other connector/net 5.x
			// installs are present.
			if (PerformanceCounterCategory.Exists(categoryName))
				PerformanceCounterCategory.Delete(categoryName);
		}
	}
}
