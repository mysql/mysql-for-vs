﻿// VsPkg.cs : Implementation of VSPackage3
//

using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio;
using MySql.Data.VisualStudio.Properties;
using System.Reflection;
using EnvDTE;
using Microsoft.VisualStudio.CommandBars;

namespace MySql.Data.VisualStudio
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    [ComVisible(true)]
    // This attribute tells the registration utility (regpkg.exe) that this class needs
    // to be registered as package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // A Visual Studio component can be registered under different regitry roots; for instance
    // when you debug your package you want to register it in the experimental hive. This
    // attribute specifies the registry root to use if no one is provided to regpkg.exe with
    // the /root switch.
    [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\9.0Exp")]
    // This attribute is used to register the informations needed to show the this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration(true, null, null, null)]
    [ProvideService(typeof(MySqlProviderObjectFactory), ServiceName = "MySQL Provider Object Factory")]
    [ProvideService(typeof(MySqlLanguageService))]
    [ProvideLanguageService(typeof(MySqlLanguageService), MySqlLanguageService.LanguageName, 101,
        RequestStockColors = true)]
    // In order be loaded inside Visual Studio in a machine that has not the VS SDK installed, 
    // package needs to have a valid load key (it can be requested at 
    // http://msdn.microsoft.com/vstudio/extend/). This attributes tells the shell that this 
    // package has a load key embedded in its resources.
    [ProvideLoadKey("Standard", "1.0", "MySQL Tools for Visual Studio", "MySQL AB c/o MySQL, Inc.", 100)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource(1000, 1)]
    // This attribute registers a tool window exposed by this package.
    [Guid(GuidList.PackageGUIDString)]
    public sealed class MySqlDataProviderPackage : Package, IVsInstalledProduct
    {
        private MySqlLanguageService languageService;
        public static MySqlDataProviderPackage Instance;

        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public MySqlDataProviderPackage() : base()
        {
            if (Instance != null)
                throw new Exception("Creating second instance of package");
            Instance = this;
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }

        /////////////////////////////////////////////////////////////////////////////
        // Overriden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initilaization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Trace.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));

            MySqlProviderObjectFactory factory = new MySqlProviderObjectFactory();

            ((IServiceContainer)this).AddService(
                typeof(MySqlProviderObjectFactory), factory, true);

            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                // Create the command for the menu item.
                CommandID menuCommandID = new CommandID(GuidList.guidMySqlDataPackageCmdSet,
                    (int)PkgCmdIDList.cmdidConfig);
                OleMenuCommand menuItem = new OleMenuCommand(ConfigCallback, menuCommandID);
                menuItem.BeforeQueryStatus += new EventHandler(menuItem_BeforeQueryStatus);
                mcs.AddCommand(menuItem);
            }

            languageService = new MySqlLanguageService();
            languageService.SetSite(this);

            IServiceContainer serviceContainer = (IServiceContainer)this;
            serviceContainer.AddService(typeof(MySqlLanguageService), languageService, true);
        }

        #endregion

        void menuItem_BeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand configButton = sender as OleMenuCommand;
            configButton.Visible = false;

            DTE dte = GetService(typeof(DTE)) as DTE;
            Array a = (Array)dte.ActiveSolutionProjects;
            if (a.Length != 1) return;

            Project p = (Project)a.GetValue(0);
            configButton.Visible = p.Kind == "{E24C65DC-7377-472b-9ABA-BC803B73C61A}";
        }

        private void ConfigCallback(object sender, EventArgs e)
        {
            WebConfig.WebConfigDlg w = new WebConfig.WebConfigDlg();
            w.ShowDialog();
        }

        #region IVsInstalledProduct Members

        int IVsInstalledProduct.IdBmpSplash(out uint pIdBmp)
        {
            pIdBmp = 400;
            return VSConstants.S_OK;
        }

        int IVsInstalledProduct.IdIcoLogoForAboutbox(out uint pIdIco)
        {
            pIdIco = 400;
            return VSConstants.S_OK;
        }

        int IVsInstalledProduct.OfficialName(out string pbstrName)
        {
            pbstrName = Resources.ProductName;
            return VSConstants.S_OK;
        }

        int IVsInstalledProduct.ProductDetails(out string pbstrProductDetails)
        {
            pbstrProductDetails = Resources.ProductDetails;
            return VSConstants.S_OK;
        }

        int IVsInstalledProduct.ProductID(out string pbstrPID)
        {
            string fullname = Assembly.GetExecutingAssembly().FullName;
            string[] parts = fullname.Split(new char[] { '=' });
            string[] versionParts = parts[1].Split(new char[] { '.' });

            pbstrPID = String.Format("{0}.{1}.{2}", versionParts[0], versionParts[1], versionParts[2]);
            return VSConstants.S_OK;
        }

        #endregion
    }
}