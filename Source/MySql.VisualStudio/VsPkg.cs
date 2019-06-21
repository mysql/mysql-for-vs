// Copyright (c) 2008, 2019, Oracle and/or its affiliates. All rights reserved.
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
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio;
using MySql.Data.VisualStudio.Properties;
using System.Reflection;
using EnvDTE;
using Microsoft.VisualStudio.CommandBars;
using MySql.Data.VisualStudio.Editors;
using MySql.Utility;
using MySql.Utility.Forms;
using Microsoft.VisualStudio.Data;
using Microsoft.VisualStudio.Data.Services;
using Microsoft.VisualStudio.Data.Interop;
using System.Linq;
using System.Data;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using Microsoft.VisualStudio.Data.Core;
using System.Collections.Generic;
using Microsoft.VisualStudio.Shell;
using MySql.Data.MySqlClient;
using System.Text;
using MySql.Data.VisualStudio.SchemaComparer;
using MySql.Data.VisualStudio.DBExport;
using MySql.Utility.Classes;
using MySql.Utility.Classes.MySqlWorkbench;
using System.IO;
using System.Windows.Forms;
using MySql.Data.VisualStudio.Wizards;
using MySql.Utility.Enums;
using Process = System.Diagnostics.Process;
using MySql.Utility.Classes.Logging;
using MySql.VisualStudio.CustomAction;
using System.Drawing;
using System.Resources;
using MySql.VisualStudio.CustomAction.Enums;
#if NET_40_OR_GREATER
using Microsoft.VSDesigner.ServerExplorer;
#endif


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
  [ProvideEditorFactory(typeof(SqlEditorFactory), 200,
      TrustLevel = __VSEDITORTRUSTLEVEL.ETL_AlwaysTrusted)]
  [ProvideEditorExtension(typeof(SqlEditorFactory), ".mysql", 32,
      ProjectGuid = "{A2FE74E1-B743-11D0-AE1A-00A0C90FFFC3}",
      TemplateDir = @"..\..\Templates",
      NameResourceID = 105,
      DefaultName = "MySQL SQL Editor")]
  [ProvideEditorLogicalView(typeof(SqlEditorFactory), "{7651a703-06e5-11d1-8ebd-00a0c90f26ea}")]
  [ProvideService(typeof(MySqlProviderObjectFactory), ServiceName = "MySQL Provider Object Factory")]
  // In order be loaded inside Visual Studio in a machine that has not the VS SDK installed, 
  // package needs to have a valid load key (it can be requested at 
  // http://msdn.microsoft.com/vstudio/extend/). This attributes tells the shell that this 
  // package has a load key embedded in its resources.
  [ProvideLoadKey("Standard", "1.0", "MySQL Tools for Visual Studio", "MySQL AB c/o MySQL, Inc.", 100)]
  // This attribute is needed to let the shell know that this package exposes some menus.
  [ProvideMenuResource(1000, 1)]
  [ProvideToolWindow(typeof(DbExportWindowPane), Style = VsDockStyle.Tabbed, Window = EnvDTE.Constants.vsWindowKindMainWindow)]
  [ProvideAutoLoad("ADFC4E64-0397-11D1-9F4E-00A0C911004F")]
  // This attribute registers a tool window exposed by this package.
  [Guid(GuidStrings.Package)]
  public sealed class MySqlDataProviderPackage : Package, IVsInstalledProduct, IDisposable
  {
    #region Fields

    /// <summary>
    /// The app data path for this application.
    /// </summary>
    private string _appDataPath;

    /// <summary>
    /// The installation path for Connector/NET.
    /// </summary>
    private string _connectorNETInstallationPath;

    /// <summary>
    /// A monitor to detect changes in the Windows registry for Connector/NET.
    /// </summary>
    private RegistryMonitor _registryMonitor;

    /// <summary>
    /// The settings for MySQL for VisualStudio.
    /// </summary>
    private PluginSettings _settings;

    /// <summary>
    /// The path for the settings file.
    /// </summary>
    private string _settingsFilePath;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the path for this application relative to the application data folder of the user where settings can be saved.
    /// </summary>
    internal string AppDataPath
    {
      get
      {
        if (string.IsNullOrEmpty(_appDataPath))
        {
          _appDataPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}\\MySQL\\{APPLICATION_NAME}\\";
        }

        return _appDataPath;
      }
    }

    /// <summary>
    /// Gets the path to the location of the settings.xml file.
    /// </summary>
    internal string SettingsFilePath
    {
      get
      {
        if (string.IsNullOrEmpty(_settingsFilePath))
        {
          _settingsFilePath = $"{AppDataPath}settings.xml";
        }

        return _settingsFilePath;
      }
    }

    public static MySqlDataProviderPackage Instance;

    public MySqlConnection MysqlConnectionSelected;

    internal string ConnectionName { get; set; }

    internal List<IVsDataExplorerConnection> _mysqlConnectionsList;

    #endregion

    #region Constants

    private const string APPLICATION_NAME = "MySQL for Visual Studio";
    private const string CONNECTOR_NET_ENVIRONMENT_VARIABLE = "MYSQLCONNECTOR_ASSEMBLIESPATH";
    private const string CONNECTOR_NET_REGISTRY_KEY = "Software\\Wow6432Node\\MySQL AB\\MySQL Connector/Net";
    private const string MYSQL_FOR_VISUAL_STUDIO_REGISTRY_KEY = "Software\\Wow6432Node\\MySQL AB\\MySQL for Visual Studio";
    private const string DEPENDENCIES_FOLDER = @"..\..\..\..\..\Dependencies\v4.0\Release\";

    #endregion    /// <summary>

    /// <summary>
    /// Default constructor of the package.
    /// Inside this method you can place any initialization code that does not require
    /// any Visual Studio service because at this point the package object is created but
    /// not sited yet inside Visual Studio environment. The place to do all the other
    /// initialization is the Initialize method.
    /// </summary>
    public MySqlDataProviderPackage()
      : base()
    {
      if (Instance != null)
      {
        throw new Exception("Creating second instance of package");
      }

      Instance = this;
      Logger.Initialize(AppDataPath.Substring(0, AppDataPath.Length - 1), APPLICATION_NAME, false, false, APPLICATION_NAME);
      Logger.LogInformation(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
    }

    // Overriden Package Implementation
    #region Package Members

    /// <summary>
    /// Initialization of the package; this method is called right after the package is sited, so this is the place
    /// where you can put all the initilaization code that rely on services provided by VisualStudio.
    /// </summary>
    protected override void Initialize()
    {
      Logger.LogInformation(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
      MySqlProviderObjectFactory factory = new MySqlProviderObjectFactory();

      ((IServiceContainer)this).AddService(
          typeof(MySqlProviderObjectFactory), factory, true);

      base.Initialize();

      RegisterEditorFactory(new SqlEditorFactory());

      // Load our connections.
      _mysqlConnectionsList = GetMySqlConnections();

      // Add our command handlers for menu (commands must exist in the .vsct file).
      OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;

      if (null != mcs)
      {
        // Create the command for the menu item.
        CommandID menuCommandID = new CommandID(Guids.CmdSet, (int)PkgCmdIDList.cmdidConfig);
        OleMenuCommand menuItem = new OleMenuCommand(ConfigCallback, menuCommandID);
        menuItem.BeforeQueryStatus += new EventHandler(configWizard_BeforeQueryStatus);
        mcs.AddCommand(menuItem);

        CommandID cmdOpenUtilitiesPrompt = new CommandID(Guids.CmdSet, (int)PkgCmdIDList.cmdidOpenUtilitiesPrompt);
        OleMenuCommand cmdItem = new OleMenuCommand(OpenMySQLUtilitiesCallback, cmdOpenUtilitiesPrompt);
        cmdItem.BeforeQueryStatus += new EventHandler(cmdOpenUtilitiesPrompt_BeforeQueryStatus);
        mcs.AddCommand(cmdItem);

        CommandID cmdLaunchWB = new CommandID(Guids.CmdSet, (int)PkgCmdIDList.cmdidLaunchWorkbench);
        OleMenuCommand cmdMenuLaunchWB = new OleMenuCommand(LaunchWBCallback, cmdLaunchWB);
        cmdMenuLaunchWB.BeforeQueryStatus += new EventHandler(cmdLaunchWB_BeforeQueryStatus);
        mcs.AddCommand(cmdMenuLaunchWB);

        CommandID menuGenDbScript = new CommandID(Guids.CmdSet, (int)PkgCmdIDList.cmdidGenerateDatabaseScript);
        OleMenuCommand menuItemGenDbScript = new OleMenuCommand(GenDbScriptCallback, menuGenDbScript);
        menuItemGenDbScript.BeforeQueryStatus += new EventHandler(GenDbScript_BeforeQueryStatus);
        mcs.AddCommand(menuItemGenDbScript);

        CommandID cmdDbExportTool = new CommandID(Guids.CmdSet, (int)PkgCmdIDList.cmdidDBExport);
        OleMenuCommand cmdMenuDbExport = new OleMenuCommand(cmdDbExport_Callback, cmdDbExportTool);
        cmdMenuDbExport.BeforeQueryStatus += new EventHandler(cmdMenuDbExport_BeforeQueryStatus);
        mcs.AddCommand(cmdMenuDbExport);

        CommandID cmdAddConnection = new CommandID(GuidList.guidIDEToolbarCmdSet, (int)PkgCmdIDList.cmdidAddConnection);
        OleMenuCommand cmdMenuAddConnection = new OleMenuCommand(cmdAddConnection_Callback, cmdAddConnection);
        mcs.AddCommand(cmdMenuAddConnection);
        var dynamicList = new MySqlConnectionListMenu(ref mcs, _mysqlConnectionsList);
      }

      // Register and initialize language service
      MySqlLanguageService languageService = new MySqlLanguageService();
      languageService.SetSite(this);
      ((IServiceContainer)this).AddService(typeof(MySqlLanguageService), languageService, true);

      // Determine whether the environment variable "MYSQLCONNECTOR_ASSEMBLIESPATH" exists.
#if NET_45_OR_GREATER
      string mySqlConnectorAssembliesVersion = "v4.5.2";
#else
      string mySqlConnectorAssembliesVersion = "v4.0";
#endif
      string mySqlConnectorPath = Utilities.GetMySqlAppInstallLocation("MySQL Connector/Net");
      _connectorNETInstallationPath = !string.IsNullOrEmpty(mySqlConnectorPath)
                            ? string.Format(@"{0}Assemblies\{1}", mySqlConnectorPath, mySqlConnectorAssembliesVersion)
                            : string.Empty;

      // If the environment variable doesn't exist, create it.
      string mySqlConnectorEnvironmentVariableValue = Environment.GetEnvironmentVariable(_connectorNETInstallationPath, EnvironmentVariableTarget.User);
      if (mySqlConnectorEnvironmentVariableValue == null)
      {
        if (!string.IsNullOrEmpty(_connectorNETInstallationPath))
        {
          SetEnvironmentVariableValues(_connectorNETInstallationPath);
        }
      }
      else
      {
        // If already exists, check if its original value has changed.
        if (!mySqlConnectorEnvironmentVariableValue.Contains(_connectorNETInstallationPath, StringComparison.InvariantCultureIgnoreCase) && !string.IsNullOrEmpty(_connectorNETInstallationPath))
        {
          SetEnvironmentVariableValues(_connectorNETInstallationPath);
        }
      }

      // Create program data directory.
      if (!Directory.Exists(AppDataPath))
      {
        try
        {
          Directory.CreateDirectory(AppDataPath);
        }
        catch (Exception exception)
        {
          Logger.LogError($"Failed to create the folder {AppDataPath} with message: {exception.Message}");
        }
      }

      // Execute if a change in Connector/NET installation was identified.
      _settings = PluginSettings.LoadFromFile(SettingsFilePath);
      if (!_settings.AskToExecuteConfigurationUpdateTool)
      {
        return;
      }

      // Initialize and start the Connector/NET registry monitor.
      _registryMonitor = new RegistryMonitor(RegistryHive.LocalMachine, CONNECTOR_NET_REGISTRY_KEY)
      {
        RegistryChangeNotifyFilter = RegistryChangeNotifyFilter.Value | RegistryChangeNotifyFilter.Key
      };
      _registryMonitor.RegistryChanged += ConnectorNETRegistryKeyChanged;
      _registryMonitor.Error += ConnectorNETRegistryKeyMonitorError;
      _registryMonitor?.Start();

      var mysqlForVisualStudioVersion = AssemblyName.GetAssemblyName(this.GetType().Assembly.Location).Version;
      var installedMySqlDataVersionString = GetVersionStringFromRegistry(CONNECTOR_NET_REGISTRY_KEY);
      Version.TryParse(installedMySqlDataVersionString, out var installedMySqlDataVersion);
      Version internalMySqlDataVersion = null;
#if DEBUG
      internalMySqlDataVersion = AssemblyName.GetAssemblyName($"{DEPENDENCIES_FOLDER}MySql.Data.dll").Version;
#else
      internalMySqlDataVersion = new Version(GetVersionStringFromRegistry(MYSQL_FOR_VISUAL_STUDIO_REGISTRY_KEY));
#endif
      if (!CustomActions.IsConfigurationUpdateRequired(mysqlForVisualStudioVersion, installedMySqlDataVersion, internalMySqlDataVersion))
      {
        return;
      }

      ConnectorNETRegistryKeyChanged(this, EventArgs.Empty);
    }

    #endregion

    private void SetEnvironmentVariableValues(string mySqlConnectorPath)
    {
      Environment.SetEnvironmentVariable(CONNECTOR_NET_ENVIRONMENT_VARIABLE, mySqlConnectorPath, EnvironmentVariableTarget.User);
      Environment.SetEnvironmentVariable(CONNECTOR_NET_ENVIRONMENT_VARIABLE, mySqlConnectorPath, EnvironmentVariableTarget.Process);
    }

    void cmdOpenUtilitiesPrompt_BeforeQueryStatus(object sender, EventArgs e)
    {
      OleMenuCommand openUtilities = sender as OleMenuCommand;
      EnvDTE80.DTE2 _applicationObject = GetDTE2();
      UIHierarchy uih = _applicationObject.ToolWindows.GetToolWindow(EnvDTE.Constants.vsWindowKindServerExplorer) as UIHierarchy;
      Array selectedItems = (Array)uih.SelectedItems;

      if (selectedItems != null)
        ConnectionName = ((UIHierarchyItem)selectedItems.GetValue(0)).Name;
      if (GetConnection(ConnectionName) != null)
      {
        if (MySqlWorkbench.IsInstalled)
          openUtilities.Visible = openUtilities.Enabled = true;
        else
        {
          openUtilities.Enabled = false;
          openUtilities.Visible = true;
        }
      }
      else
        openUtilities.Visible = openUtilities.Enabled = false;
    }

    void cmdLaunchWB_BeforeQueryStatus(object sender, EventArgs e)
    {
      OleMenuCommand launchWBbtn = sender as OleMenuCommand;

      EnvDTE80.DTE2 _applicationObject = GetDTE2();
      UIHierarchy uih = _applicationObject.ToolWindows.GetToolWindow(EnvDTE.Constants.vsWindowKindServerExplorer) as UIHierarchy;
      Array selectedItems = (Array)uih.SelectedItems;

      if (selectedItems != null)
        ConnectionName = ((UIHierarchyItem)selectedItems.GetValue(0)).Name;

      if (GetConnection(ConnectionName) != null)
      {
        if (MySqlWorkbench.IsInstalled)
          launchWBbtn.Visible = launchWBbtn.Enabled = true;
        else
        {
          launchWBbtn.Enabled = false;
          launchWBbtn.Visible = true;
        }
      }
      else
        launchWBbtn.Visible = launchWBbtn.Enabled = false;
    }

    void configWizard_BeforeQueryStatus(object sender, EventArgs e)
    {
      OleMenuCommand configButton = sender as OleMenuCommand;
      configButton.Visible = false;

      ////this feature can be shown only if Connector/Net is installed too
      if (String.IsNullOrEmpty(Utilities.GetMySqlAppInstallLocation("MySQL Connector/Net")))
        return;

      DTE dte = GetService(typeof(DTE)) as DTE;
      Array a = (Array)dte.ActiveSolutionProjects;
      if (a.Length != 1) return;

      Project p = (Project)a.GetValue(0);
      configButton.Visible = false;
      foreach (Property prop in p.Properties)
      {
        if (prop.Name == "WebSiteType" || prop.Name.StartsWith("WebApplication", StringComparison.OrdinalIgnoreCase))
        {
          configButton.Visible = true;
          break;
        }
      }
    }

    /// <summary>
    /// Event delegate method fired when the registry key associated with Connector/NET changes.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">The event arguments.</param>
    private void ConnectorNETRegistryKeyChanged(object sender, EventArgs e)
    {
      var infoDialogProperties = InfoDialogProperties.GetYesNoDialogProperties(
        InfoDialog.InfoType.Warning,
        Properties.Resources.ConfigurationUpdateToolDialogTitle,
        Properties.Resources.ConfigurationUpdateToolDialogDetail,
        Properties.Resources.ConfigurationUpdateToolDialogSubDetail);
      infoDialogProperties.FitTextStrategy = InfoDialog.FitTextsAction.IncreaseDialogWidth;
      infoDialogProperties.CommandAreaProperties.LeftAreaControl = CommandAreaProperties.LeftAreaControlType.InfoCheckBox;
      infoDialogProperties.CommandAreaProperties.LeftAreaCheckBoxText = Properties.Resources.ConfigurationUpdateToolAskCheckBox;
      infoDialogProperties.LogoImage = Properties.Resources.MySQLforVisualStudio;
      using (var yesNoDialog = new InfoDialog(infoDialogProperties))
      {
        var dialogResult = yesNoDialog.ShowDialog();
        _settings.AskToExecuteConfigurationUpdateTool = !yesNoDialog.InfoCheckBoxChecked;
        PluginSettings.SaveObjectToXml(_settings, SettingsFilePath);
        if (dialogResult != DialogResult.Yes)
        {
          return;
        }

        ExecuteMySqlForVisualStudioConfigurationUpdateTool();
      }
    }

    /// <summary>
    /// Event delegate method fired when an error occurs while monitoring changes to the registry key associated with Connector/NET.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">The event arguments.</param>
    private void ConnectorNETRegistryKeyMonitorError(object sender, ErrorEventArgs e)
    {
      Logger.LogException(e.GetException());
    }

    void GenDbScript_BeforeQueryStatus(object sender, EventArgs e)
    {
      OleMenuCommand cmd = sender as OleMenuCommand;
      cmd.Visible = false;
    }

    void cmdMenuDbExport_BeforeQueryStatus(object sender, EventArgs e)
    {
      OleMenuCommand dbExportButton = sender as OleMenuCommand;
      EnvDTE80.DTE2 _applicationObject = GetDTE2();
      UIHierarchy uih = _applicationObject.ToolWindows.GetToolWindow(EnvDTE.Constants.vsWindowKindServerExplorer) as UIHierarchy;
      Array selectedItems = (Array)uih.SelectedItems;

      dbExportButton.Visible = false;

      if (selectedItems != null)
        ConnectionName = ((UIHierarchyItem)selectedItems.GetValue(0)).Name;
      if (GetConnection(ConnectionName) != null)
      {
        dbExportButton.Visible = true;
        dbExportButton.Enabled = true;
      }
      else
      {
        dbExportButton.Enabled = false;
      }
    }

    private void cmdDbExport_Callback(object sender, EventArgs e)
    {
      MySqlConnection connection = GetCurrentConnection();
      string currentConnectionName = GetCurrentConnectionName();
      if (connection != null)
      {
        for (int i = 0; ; i++)
        {
          ToolWindowPane existingDbExportToolWindow = this.FindToolWindow(typeof(DbExportWindowPane), i, false);
          if (existingDbExportToolWindow == null)
          {
            var window = (ToolWindowPane)this.CreateToolWindow(typeof(DbExportWindowPane), i);
            if (window == null || window.Frame == null)
              throw new Exception("Cannot create a new window for data export");

            window.Caption = Properties.Resources.DbExportToolCaptionFrame;

            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;

            DbExportWindowPane windowPanel = (DbExportWindowPane)window;

            if (_mysqlConnectionsList == null || _mysqlConnectionsList.Count <= 0)
              _mysqlConnectionsList = GetMySqlConnections();

            windowPanel.Connections = _mysqlConnectionsList;
            windowPanel.SelectedConnectionName = currentConnectionName;
            windowPanel.WindowHandler = window;
            windowPanel.InitializeDbExportPanel();

            GetDTE2().Windows.Item(EnvDTE.Constants.vsWindowKindOutput).Visible = true;

            object currentFrameMode;
            windowFrame.GetProperty((int)__VSFPROPID.VSFPROPID_FrameMode, out currentFrameMode);
            // switch to dock mode.                  
            if ((VSFRAMEMODE)currentFrameMode == VSFRAMEMODE.VSFM_Float)
              windowFrame.SetProperty((int)__VSFPROPID.VSFPROPID_FrameMode, VSFRAMEMODE.VSFM_Dock);


            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
            break;
          }
        }
      }
    }

    private void OpenMySQLUtilitiesCallback(object sender, EventArgs e)
    {
      if (String.IsNullOrEmpty(Utilities.GetMySqlAppInstallLocation("MySQL Utilities")))
      {
        var pathWorkbench = Utilities.GetMySqlAppInstallLocation("Workbench");
        var pathUtilities = Path.Combine(pathWorkbench, "Utilities");

        if (!Directory.Exists(pathUtilities))
        {
          if (MessageBox.Show("The command line MySQL Utilities could not be found." + Environment.NewLine
                         + @"To use them you must download and install the utilities package from http://dev.mysql.com/downloads/tools/utilities/" +
                          Environment.NewLine + "Click OK to go to the page or Cancel to continue", "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
          {
            ProcessStartInfo browserInfo = new ProcessStartInfo("http://dev.mysql.com/downloads/tools/utilities/");
            Process.Start(browserInfo);
          }
          else
            return;
        }
      }
      else
        MySqlWorkbench.LaunchUtilitiesShell();
    }

    private void LaunchWBCallback(object sender, EventArgs e)
    {
      IVsDataExplorerConnection connection = GetConnection(ConnectionName);
      if (connection != null)
      {
        var connList = MySqlWorkbench.Connections;
        var connStr = connection.Connection.DisplayConnectionString;
        ConnectionParameters parameters = ParseConnectionString(connStr);
        MySqlWorkbench.LaunchSqlEditor(FindMathchingWorkbenchConnection(parameters));
      }
    }

    private void ConfigCallback(object sender, EventArgs e)
    {
      WebConfig.WebConfigDlg w = new WebConfig.WebConfigDlg();
      w.ShowDialog();
    }

    private void GenDbScriptCallback(object sender, EventArgs e)
    {
      // Get current connection
      string conStr = "";
      string script = "";
      EnvDTE80.DTE2 _applicationObject = GetDTE2();
      UIHierarchy uih = _applicationObject.ToolWindows.GetToolWindow(EnvDTE.Constants.vsWindowKindServerExplorer) as UIHierarchy;
      Array selectedItems = (Array)uih.SelectedItems;

      if (selectedItems != null)
        conStr = ((UIHierarchyItem)selectedItems.GetValue(0)).Name;

      IVsDataExplorerConnection con = GetConnection(conStr);
      // Get script
      MySqlConnection myCon = new MySqlConnection(con.Connection.DisplayConnectionString);
      myCon.Open();
      try
      {
        script = SelectObjects.GetDbScript(myCon);
      }
      finally
      {
        myCon.Close();
      }
      // show script window
      MySqlScriptDialog dlg = new MySqlScriptDialog();
      dlg.TextScript = script;
      dlg.ShowDialog();
    }

    private void cmdAddConnection_Callback(object sender, EventArgs e)
    {
      ConnectDialog d = new ConnectDialog();
      DialogResult r = d.ShowDialog();
      if (r == DialogResult.Cancel) return;
      try
      {
        MysqlConnectionSelected = (MySqlConnection)d.Connection;
        DTE env = (DTE)GetService(typeof(DTE));
        Microsoft.VisualStudio.Shell.ServiceProvider sp = new Microsoft.VisualStudio.Shell.ServiceProvider((IOleServiceProvider)env);
        IVsDataExplorerConnectionManager seConnectionsMgr = (IVsDataExplorerConnectionManager)sp.GetService(typeof(IVsDataExplorerConnectionManager).GUID);
        seConnectionsMgr.AddConnection(string.Format("{0}({1})", MysqlConnectionSelected.DataSource, MysqlConnectionSelected.Database), Guids.Provider, MysqlConnectionSelected.ConnectionString, false);
        ItemOperations ItemOp = env.ItemOperations;
        ItemOp.NewFile(@"MySQL\MySQL Script", null, "{A2FE74E1-B743-11D0-AE1A-00A0C90FFFC3}");
      }
      catch (MySqlException)
      {
        Logger.LogError(@"Error establishing the database connection. Check that the server is running, the database exist and the user credentials are valid.", true);
        return;
      }
      MysqlConnectionSelected = null;
    }

    public string GetCurrentConnectionName()
    {
      EnvDTE80.DTE2 _applicationObject = GetDTE2();
      UIHierarchy uih = _applicationObject.ToolWindows.GetToolWindow(EnvDTE.Constants.vsWindowKindServerExplorer) as UIHierarchy;
      Array selectedItems = (Array)uih.SelectedItems;

      if (selectedItems != null)
        return ((UIHierarchyItem)selectedItems.GetValue(0)).Name;

      return string.Empty;
    }

    private MySqlConnection GetCurrentConnection()
    {
      IVsDataExplorerConnection con = GetConnection(GetCurrentConnectionName());
      try
      {
        MySqlConnection connection = (MySqlConnection)con.Connection.GetLockedProviderObject();
        try
        {
          if (connection != null)
            return new MySqlConnection(connection.ConnectionString);
          return null;
        }
        finally
        {
          con.Connection.UnlockProviderObject();
        }
      }
      catch
      {
        return null;
      }
    }

    internal EnvDTE80.DTE2 GetDTE2()
    {
      return GetGlobalService(typeof(DTE)) as EnvDTE80.DTE2;
    }

    public IVsDataExplorerConnection GetConnection(string connectionName)
    {
      IVsDataExplorerConnectionManager connectionManager = GetService(typeof(IVsDataExplorerConnectionManager)) as IVsDataExplorerConnectionManager;
      if (connectionManager == null) return null;

      System.Collections.Generic.IDictionary<string, IVsDataExplorerConnection> connections = connectionManager.Connections;

      foreach (var connection in connections)
      {
        if (Guids.Provider.Equals(connection.Value.Provider) && connection.Value.DisplayName.Equals(connectionName))
          return connection.Value;
      }
      return null;
    }

    public List<IVsDataExplorerConnection> GetMySqlConnections()
    {
      try
      {
        IVsDataExplorerConnectionManager connectionManager = GetService(typeof(IVsDataExplorerConnectionManager)) as IVsDataExplorerConnectionManager;
        if (connectionManager == null) return new List<IVsDataExplorerConnection>();

        System.Collections.Generic.IDictionary<string, IVsDataExplorerConnection> connections = connectionManager.Connections;
        _mysqlConnectionsList = new List<IVsDataExplorerConnection>();
        foreach (var connection in connections)
        {
          if (Guids.Provider.Equals(connection.Value.Provider))
            _mysqlConnectionsList.Add(connection.Value);
        }
        return _mysqlConnectionsList;
      }
      catch
      { }

      return new List<IVsDataExplorerConnection>();
    }

    public string GetConnectionStringBasedOnNode(string name)
    {
      try
      {
        IVsDataExplorerConnectionManager connectionManager = GetService(typeof(IVsDataExplorerConnectionManager)) as IVsDataExplorerConnectionManager;
        if (connectionManager == null) return null;

        System.Collections.Generic.IDictionary<string, IVsDataExplorerConnection> connections = connectionManager.Connections;
        string activeConnectionString = string.Empty;

        foreach (var connection in connections)
        {
          if (Guids.Provider.Equals(connection.Value.Provider))
          {
            var selectedNodes = connection.Value.SelectedNodes;
            foreach (var node in selectedNodes)
            {
              if (node.Caption.Equals(name, StringComparison.InvariantCultureIgnoreCase))
              {
                try
                {
                  var activeConnection = (MySqlConnection)connection.Value.Connection.GetLockedProviderObject();
                  if (activeConnection != null)
                  {
                    var csb = (MySqlConnectionStringBuilder)activeConnection.GetType().GetProperty("Settings", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(activeConnection, null);
                    if (csb != null)
                    {
                      activeConnectionString = csb.ConnectionString;
                    }
                  }
                }
                catch { }
                finally
                {
                  connection.Value.Connection.UnlockProviderObject();
                }
              }
            }
          }
        }
        return activeConnectionString;
      }
      catch
      { }

      return null;

    }

    public ConnectionParameters ParseConnectionString(string connStr)
    {

      var connStringBuilder = new MySql.Data.MySqlClient.MySqlConnectionStringBuilder(connStr);
      var parameters = new ConnectionParameters();
      parameters.UserId = connStringBuilder.UserID;
      parameters.HostName = connStringBuilder.Server;
      parameters.HostIPv4 = Utilities.GetIPv4ForHostName(connStringBuilder.Server);
      parameters.Port = Convert.ToInt32(connStringBuilder.Port);
      parameters.DataBaseName = connStringBuilder.Database;
      parameters.NamedPipesEnabled = String.IsNullOrEmpty(connStringBuilder.PipeName) ? false : true;
      parameters.PipeName = connStringBuilder.PipeName;

      return parameters;
    }

    private string FindMathchingWorkbenchConnection(ConnectionParameters parameters)
    {
      var filteredConnections = MySqlWorkbench.Connections.Where(t => !String.IsNullOrEmpty(t.Name) && t.Port == parameters.Port);

      if (filteredConnections != null)
      {
        foreach (MySqlWorkbenchConnection c in filteredConnections)
        {
          switch (c.ConnectionMethod)
          {

            case MySqlWorkbenchConnection.ConnectionMethodType.LocalUnixSocketOrWindowsPipe:
              if (!parameters.NamedPipesEnabled || String.Compare(c.UnixSocketOrWindowsPipe, parameters.PipeName, true) != 0) continue;
              break;
            case MySqlWorkbenchConnection.ConnectionMethodType.Ssh:
              continue;
            case MySqlWorkbenchConnection.ConnectionMethodType.Tcp:
              if (c.Port != parameters.Port) continue;
              break;
            case MySqlWorkbenchConnection.ConnectionMethodType.Unknown:
              continue;
          }

          if (!Utilities.IsValidIpAddress(c.Host)) //matching connections by Ip
          {
            if (Utilities.GetIPv4ForHostName(c.Host) != parameters.HostIPv4) continue;
          }
          else
          {
            if (c.Host != parameters.HostIPv4) continue;
          }
          return c.Name;
        }
      }
      return String.Empty;
    }

    private void CreateNewMySqlProject(string projectType)
    {
      DTE env = (DTE)GetService(typeof(DTE));
      WizardNewProjectDialog dlg;

      MySql.Data.VisualStudio.Wizards.ValidationsGrid.ClearMetadataCache();

      if (String.IsNullOrEmpty(Settings.Default.NewProjectDialogSelected))
        dlg = new WizardNewProjectDialog(projectType);
      else
        dlg = new WizardNewProjectDialog(Settings.Default.NewProjectDialogSelected);

      DialogResult result = dlg.ShowDialog();
      if (result != DialogResult.OK) return;

      EnvDTE80.Solution2 sol = (EnvDTE80.Solution2)env.Solution;
      var solutionName = dlg.SolutionName;
      var solutionPath = dlg.ProjectPath;
      Settings.Default.NewProjectDialogSelected = dlg.ProjectType;
      Settings.Default.NewProjectLanguageSelected = dlg.Language.IndexOf("CSharp") >= 0 ? "Visual C#" : "Visual Basic";
      Settings.Default.NewProjectSavedPath = dlg.ProjectPath;
      Settings.Default.CreateDirectoryForSolution = dlg.CreateDirectoryForSolution;
      Settings.Default.CreateNewSolution = dlg.CreateNewSolution ? "Create new solution" : "Add to solution";
      Settings.Default.Save();

      if (dlg.CreateDirectoryForSolution)
      {
        solutionPath = Path.Combine(Path.Combine(solutionPath, dlg.SolutionName), dlg.ProjectName);
      }
      else
      {
        solutionPath = Path.Combine(solutionPath, dlg.ProjectName);
      }
      Directory.CreateDirectory(solutionPath);

      string templatePath = string.Empty;
      templatePath = sol.GetProjectTemplate(dlg.ProjectType, dlg.Language);
      sol.AddFromTemplate(templatePath, solutionPath, dlg.ProjectName, dlg.CreateNewSolution);
    }

    /// <summary>
    /// Executes the Configuration Update Tool that updates PKGDEF files.
    /// </summary>
    private void ExecuteMySqlForVisualStudioConfigurationUpdateTool()
    {
      var mysqlForVisualStudioVersion = AssemblyName.GetAssemblyName(this.GetType().Assembly.Location).Version;
      var installedMySqlDataVersionString = GetVersionStringFromRegistry(CONNECTOR_NET_REGISTRY_KEY);
#if DEBUG
      var internalMySqlDataVersion = AssemblyName.GetAssemblyName($"{DEPENDENCIES_FOLDER}MySql.Data.dll").Version;
      var fileName = @"..\..\..\..\MySql.VisualStudio.Updater\bin\Debug\";
#else
      var internalMySqlDataVersion = new Version(GetVersionStringFromRegistry(MYSQL_FOR_VISUAL_STUDIO_REGISTRY_KEY));
      var fileName = $@"{GetValueFromRegistry(MYSQL_FOR_VISUAL_STUDIO_REGISTRY_KEY, "Location")}\Dependencies\";
#endif

      try
      {
        var startInfo = new ProcessStartInfo
        {
          FileName = $"{fileName}MySql.VisualStudio.Updater.exe",
          Arguments = $"{mysqlForVisualStudioVersion.ToString()} {internalMySqlDataVersion.ToString()} {installedMySqlDataVersionString}",
          Verb = "runas"
        };

        Process.Start(startInfo);
      }
      catch (Exception exception)
      {
        Logger.LogException(exception);
      }
    }

    /// <summary>
    /// Gets the value of the specified Windows registry key.
    /// </summary>
    /// <param name="registryKeyPath">The path to registry key to check for.</param>
    /// <param name="keyName">The name of the key to check for.</param>
    /// <returns>The value of the specified registry key.</returns>
    public string GetValueFromRegistry(string registryKeyPath, string keyName)
    {
      RegistryKey key = null;
      string stringValue = null;
      try
      {
        key = RegistryHive.LocalMachine.OpenRegistryKey(registryKeyPath);
        var value = key?.GetValue(keyName);
        if (value != null)
        {
          stringValue = value.ToString();
        }
      }
      catch (Exception ex)
      {
        Logger.LogException(ex);
      }
      finally
      {
        key?.Close();
      }

      return stringValue;
    }

    /// <summary>
    /// Gets the version number of the specified product from the Windows registry.
    /// </summary>
    /// <param name="registryKey">The registry key to check for.</param>
    /// <returns>The version number as a string.</returns>
    public string GetVersionStringFromRegistry(string registryKey)
    {
      var version = GetValueFromRegistry(registryKey, "Version");

      if (version != null)
      {
        return $"{version}.0";
      }

      return null;
    }

    public struct ConnectionParameters
    {
      public string HostName;
      public string HostIPv4;
      public int Port;
      public string PipeName;
      public bool NamedPipesEnabled;
      public string UserId;
      public string DataBaseName;
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
      pbstrName = Properties.Resources.ProductName;
      return VSConstants.S_OK;
    }

    int IVsInstalledProduct.ProductDetails(out string pbstrProductDetails)
    {
      pbstrProductDetails = Properties.Resources.ProductDetails;
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

    /// <summary>
    /// Disposes unused items.
    /// </summary>
    public void Dispose()
    {
      // Stop registry monitor and dispose of it.
      if (_registryMonitor != null)
      {
        _registryMonitor.Stop();
        _registryMonitor.RegistryChanged -= ConnectorNETRegistryKeyChanged;
        _registryMonitor.Error -= ConnectorNETRegistryKeyMonitorError;
        _registryMonitor.Dispose();
      }
    }

    #endregion
  }
}