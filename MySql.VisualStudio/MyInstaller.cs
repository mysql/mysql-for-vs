// Copyright (c) 2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using Microsoft.Win32;
using System.Reflection;


namespace MySql.Data.VisualStudio
{
    [RunInstaller(true)]
    public class MyInstaller : Installer
    {
        private string GetRoot()
        {
            string root = String.Empty;

            bool ranu = false;
            if (Context.Parameters["RANU"] == "true")
                ranu = true;

            if (Context.Parameters["version"] == "VS2005")
            {
                if (ranu)
                    throw new NotSupportedException("RANU not supported for Visual Studio 2005");
                root = "8.0";
            }
            else if (Context.Parameters["version"] == "VS2008")
                root = "9.0";
            else if (Context.Parameters["version"] == "VS2010")
                root = "10.0";
            else
                throw new NotSupportedException();

            if (Context.Parameters["debug"] == "true")
                root += "Exp";
            if (ranu)
                root += @"\Configuration";
            return root;
        }

        public override void Install(IDictionary stateSaver)
        {
            string root = GetRoot();
            Console.WriteLine("Installing to root " + root);
            InstallInternal(root);
        }

        public override void Uninstall(IDictionary savedState)
        {
            string root = GetRoot();
            Console.WriteLine("Removing from root " + root);
            UnInstallInternal(root);
        }

        private RegistryKey GetRootKey()
        {
            if (Context.Parameters["RANU"] == "true")
                return Registry.CurrentUser;
            return Registry.LocalMachine;
        }

        private void InstallInternal(string version)
        {
            RegistryKey rootKey = GetRootKey();

            // Data Source
            string keyPath = String.Format(@"Software\Microsoft\VisualStudio\{0}\DataSources\{1}",
                version, "{98FBE4D8-5583-4233-B219-70FF8C7FBBBD}");
            RegistryKey dsKey = rootKey.CreateSubKey(keyPath);
            dsKey.SetValue(null, "MySQL Server");
            RegistryKey dsSubKey = dsKey.CreateSubKey("SupportingProviders").CreateSubKey(
                GuidList.ProviderGUID.ToString("B"));
            dsSubKey.SetValue("Description", 
                "Provider_Description, MySql.Data.VisualStudio.Properties.Resources");
            dsSubKey.SetValue("DisplayName", 
                "Datasource_Displayname, MySql.Data.VisualStudio.Properties.Resources");

//"AssociatedSource"="{067EA0D9-BA62-43f7-9106-34930C60C528}"
//"PlatformVersion"="2.0"

            // Data Provider
            keyPath = String.Format(@"Software\Microsoft\VisualStudio\{0}\DataProviders\{1}",
                version, GuidList.ProviderGUID.ToString("B"));
            RegistryKey dpKey = rootKey.CreateSubKey(keyPath);
            dpKey.SetValue(null, ".NET Framework Data Provider for MySQL");
            dpKey.SetValue("DisplayName",
                "Provider_DisplayName, MySql.Data.VisualStudio.Properties.Resources");
            dpKey.SetValue("ShortDisplayName", 
                "Provider_ShortDisplayName, MySql.Data.VisualStudio.Properties.Resources");
            dpKey.SetValue("Description",
                "Provider_Description, MySql.Data.VisualStudio.Properties.Resources");
            dpKey.SetValue("Technology", "{77AB9A9D-78B9-4ba7-91AC-873F5338F1D2}");
            dpKey.SetValue("FactoryService", GuidList.EditorFactoryCLSID.ToString("B"));
            dpKey.SetValue("InvariantName", "MySql.Data.MySqlClient");
            RegistryKey dpKeySO = dpKey.CreateSubKey("SupportedObjects");
            dpKeySO.CreateSubKey("DataConnectionPromptDialog");
            dpKeySO.CreateSubKey("DataConnectionProperties");
            dpKeySO.CreateSubKey("DataConnectionSupport");
            dpKeySO.CreateSubKey("DataConnectionUIControl");
            dpKeySO.CreateSubKey("DataObjectSupport");
            dpKeySO.CreateSubKey("DataSourceInformation");
            dpKeySO.CreateSubKey("DataViewSupport");

            // Menus
            keyPath = String.Format(@"Software\Microsoft\VisualStudio\{0}\Menus", version);
            RegistryKey menuKey = rootKey.OpenSubKey(keyPath, true);
            menuKey.SetValue(GuidList.PackageGUID.ToString("B"), ",1000,1");

            // Service
            keyPath = String.Format(@"Software\Microsoft\VisualStudio\{0}\Services\{1}",
                version, GuidList.EditorFactoryCLSID.ToString("B"));
            RegistryKey srvKey = rootKey.CreateSubKey(keyPath);
            srvKey.SetValue(null, GuidList.PackageGUID.ToString("B"));
            srvKey.SetValue("Name", "MySQL Provider Object Factory");

            Assembly a = Assembly.GetExecutingAssembly();
            Version v = a.GetName().Version;
            string assemblyVersion = String.Format("{0}.{1}.{2}", v.Major, v.Minor, v.Build);

            // Installed products
            keyPath = String.Format(@"Software\Microsoft\VisualStudio\{0}\InstalledProducts\MySQL Connector Net {1}",
                version, assemblyVersion);
            RegistryKey ipKey = rootKey.CreateSubKey(keyPath);
            ipKey.SetValue(null, String.Format("MySQL Connector Net {0}", assemblyVersion));
            ipKey.SetValue("Package", GuidList.PackageGUID.ToString("B"));
            ipKey.SetValue("UseInterface", 1);

            keyPath = String.Format(@"Software\Microsoft\VisualStudio\{0}\Languages\Language Services\MySQL", version);
            RegistryKey langKey = rootKey.CreateSubKey(keyPath);
            langKey.SetValue(null, "{fa498a2d-116a-4f25-9b55-7938e8e6dda7}");
            langKey.SetValue("Package", "{79a115c9-b133-4891-9e7b-242509dad272}");
            langKey.SetValue("LangResID", 101);
            langKey.SetValue("RequestStockColors", 1);

            // Package
            keyPath = String.Format(@"Software\Microsoft\VisualStudio\{0}\Packages\{1}",
                version, GuidList.PackageGUID.ToString("B"));
            RegistryKey packageKey = rootKey.CreateSubKey(keyPath);
            packageKey.SetValue(null, String.Format("MySQL Connector Net {0}", assemblyVersion));
            packageKey.SetValue("InprocServer32", 
                String.Format(@"{0}\system32\mscoree.dll", 
                Environment.GetEnvironmentVariable("windir")));
            packageKey.SetValue("Class", typeof(MySqlDataProviderPackage).ToString());
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            if (codeBase.StartsWith("file:///"))
                codeBase = codeBase.Substring(8);
            packageKey.SetValue("CodeBase", codeBase);
            packageKey.SetValue("ProductName", "MySQL Tools for Visual Studio");
            packageKey.SetValue("ProductVersion", "1.1");
            packageKey.SetValue("CompanyName", "MySQL AB c/o MySQL, Inc.");
            packageKey.SetValue("MinEdition", "standard");
            packageKey.SetValue("ID", 100);

            // our package should autoload if a solution exists
            keyPath = String.Format(@"Software\Microsoft\VisualStudio\{0}\AutoLoadPackages\{{F1536EF8-92EC-443C-9ED7-FDADF150DA82}}",
                version);
            RegistryKey autoLoadKey = rootKey.OpenSubKey(keyPath, true);
            autoLoadKey.SetValue(GuidList.PackageGUID.ToString("B"), 0);
        }

        private void UnInstallInternal(string version)
        {
            RegistryKey rootKey = GetRootKey();

            // Data Source
            string keyPath = String.Format(@"Software\Microsoft\VisualStudio\{0}\DataSources\{1}",
                version, "{98FBE4D8-5583-4233-B219-70FF8C7FBBBD}");
            RegistryKey key = rootKey.OpenSubKey(keyPath);
            if (key != null)
            {
                key.Close();
                rootKey.DeleteSubKeyTree(keyPath);
            }

            // Data Provider
            keyPath = String.Format(@"Software\Microsoft\VisualStudio\{0}\DataProviders\{1}",
                version, GuidList.ProviderGUID.ToString("B"));
            key = rootKey.OpenSubKey(keyPath);
            if (key != null)
            {
                key.Close();
                rootKey.DeleteSubKeyTree(keyPath);
            }

            // Menus
            keyPath = String.Format(@"Software\Microsoft\VisualStudio\{0}\Menus", version);
            RegistryKey menuKey = rootKey.OpenSubKey(keyPath, true);
            menuKey.DeleteValue(GuidList.PackageGUID.ToString("B"), false);

            // Service
            keyPath = String.Format(@"Software\Microsoft\VisualStudio\{0}\Services\{1}",
                version, GuidList.EditorFactoryCLSID.ToString("B"));
            key = rootKey.OpenSubKey(keyPath);
            if (key != null)
            {
                key.Close();
                rootKey.DeleteSubKeyTree(keyPath);
            }

            Assembly a = Assembly.GetExecutingAssembly();
            string assemblyVersion = a.GetName().Version.ToString();

            // Installed products
            keyPath = String.Format(@"Software\Microsoft\VisualStudio\{0}\InstalledProducts\MySQL Connector/Net {1}",
                version, assemblyVersion);
            key = rootKey.OpenSubKey(keyPath);
            if (key != null)
            {
                key.Close();
                rootKey.DeleteSubKeyTree(keyPath);
            }

            keyPath = String.Format(@"Software\Microsoft\VisualStudio\{0}\Languages\Language Services\MySQL",
                assemblyVersion);
            key = rootKey.OpenSubKey(keyPath);
            if (key != null)
            {
                key.Close();
                rootKey.DeleteSubKey(keyPath);
            }

            // Package
            keyPath = String.Format(@"Software\Microsoft\VisualStudio\{0}\Packages\{1}",
                version, GuidList.PackageGUID.ToString("B"));
            key = rootKey.OpenSubKey(keyPath);
            if (key != null)
            {
                key.Close();
                rootKey.DeleteSubKeyTree(keyPath);
            }

            // Autoload
            keyPath = String.Format(@"Software\Microsoft\VisualStudio\{0}\AutoLoadPackages\{{F1536EF8-92EC-443C-9ED7-FDADF150DA82}}",
                version);
            RegistryKey autoLoadKey = rootKey.OpenSubKey(keyPath, true);
            autoLoadKey.DeleteValue(GuidList.PackageGUID.ToString("B"), false);
        }
    }
}
