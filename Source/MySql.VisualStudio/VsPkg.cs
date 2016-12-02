// Copyright © 2008, 2016, Oracle and/or its affiliates. All rights reserved.
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

using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Data.Services;
using System.Linq;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using System.Collections.Generic;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using MySql.Data.MySqlClient;
using MySql.Data.VisualStudio.DBExport;
using MySql.Data.VisualStudio.Editors;
using MySql.Data.VisualStudio.Properties;
using MySql.Data.VisualStudio.SchemaComparer;
using MySql.Data.VisualStudio.Wizards;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml.Linq;
using MySqlConnectionStringBuilder = MySql.Data.MySqlClient.MySqlConnectionStringBuilder;
using ServiceProvider = Microsoft.VisualStudio.Shell.ServiceProvider;
using MySql.Data.VisualStudio.DDEX;
using MySql.Utility.Classes;
using MySql.Utility.Classes.MySql;
using MySql.Utility.Classes.MySqlWorkbench;
using MySql.Utility.Enums;
using MySql.Utility.Forms;

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
  [ProvideEditorExtension(typeof(SqlEditorFactory), SQL_EXTENSION, 32,
      ProjectGuid = "{A2FE74E1-B743-11D0-AE1A-00A0C90FFFC3}",
      TemplateDir = @"..\..\Templates",
      NameResourceID = 105,
      DefaultName = "MySQL SQL Editor")]
  [ProvideEditorExtension(typeof(SqlEditorFactory), JAVASCRIPT_EXTENSION, 32,
      ProjectGuid = "{A2FE74E1-B743-11D0-AE1A-00A0C90FFFC3}",
      TemplateDir = @"..\..\Templates",
      NameResourceID = 114,
      DefaultName = "MySQL JavaScript Editor")]
  [ProvideEditorExtension(typeof(SqlEditorFactory), PYTHON_EXTENSION, 32,
      ProjectGuid = "{A2FE74E1-B743-11D0-AE1A-00A0C90FFFC3}",
      TemplateDir = @"..\..\Templates",
      NameResourceID = 115,
      DefaultName = "MySQL Python Editor")]
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
  [ProvideToolWindow(typeof(MySqlOutputWindowPane), Style = VsDockStyle.Tabbed, Window = EnvDTE.Constants.vsWindowKindMainWindow)]
  [ProvideAutoLoad("ADFC4E64-0397-11D1-9F4E-00A0C911004F")]
  // This attribute registers a tool window exposed by this package.
  [Guid(GuidStrings.PACKAGE)]
  public sealed class MySqlDataProviderPackage : Package, IVsInstalledProduct
  {
    private const string MYSQL_CONNECTOR_ENVIRONMENT_VARIABLE = "MYSQLCONNECTOR_ASSEMBLIESPATH";

    /// <summary>
    /// The number of seconds in 1 hour.
    /// </summary>
    private const int MILLISECONDS_IN_HOUR = 3600000;

    private string _appDataPath;

    private MySqlX.MySqlConnectionsManagerDialog _connectionsManagerDialog;

    /// <summary>
    /// The timer that checks for automatic connetions migration.
    /// </summary>
    private Timer _connectionsMigrationTimer;

    /// <summary>
    /// Flag indicating whether the code that migrates connections is in progress.
    /// </summary>
    private bool _migratingStoredConnections;

    private MySqlConnection _selectedMySqlConnection;
    private MySqlWorkbenchConnection _selectedMySqlWorkbenchConnection;

    public static MySqlDataProviderPackage Instance;

    /// <summary>
    /// Gets a <see cref="DateTime"/> value for when the next automatic connections migration will occur.
    /// </summary>
    public DateTime NextAutomaticConnectionsMigration
    {
      get
      {
        var alreadyMigrated = Settings.Default.WorkbenchMigrationSucceeded;
        var delay = Settings.Default.WorkbenchMigrationRetryDelay;
        var lastAttempt = Settings.Default.WorkbenchMigrationLastAttempt;
        return alreadyMigrated || (lastAttempt.Equals(DateTime.MinValue) && delay == 0)
          ? DateTime.MinValue
          : (delay == -1 ? DateTime.MaxValue : lastAttempt.AddHours(delay));
      }
    }

    public MySqlConnection SelectedMySqlConnection
    {
      get
      {
        _selectedMySqlConnection = GetMySqlConnection(GetCurrentConnectionName());
        return _selectedMySqlConnection;
      }

      set
      {
        _selectedMySqlConnection = value;
        _selectedMySqlWorkbenchConnection = null;
      }
    }

    public MySqlWorkbenchConnection SelectedMySqlWorkbenchConnection
    {
      get
      {
        if (_selectedMySqlWorkbenchConnection == null)
        {
          _selectedMySqlWorkbenchConnection = GetMySqlWorkbenchConnection(SelectedMySqlConnectionName, SelectedMySqlConnection);
        }

        return _selectedMySqlWorkbenchConnection;
      }
    }

    public string SelectedMySqlConnectionName { get; set; }

    /// <summary>
    /// Variable used to hold how many MySqlOutputWindow objects have been created
    /// </summary>
    private int _mySqlOutputWindowCounter = 0;

    /// <summary>
    /// The Sql extension
    /// </summary>
    public const string SQL_EXTENSION = ".mysql";

    /// <summary>
    /// The JavaScrip extension
    /// </summary>
    public const string JAVASCRIPT_EXTENSION = ".myjs";

    /// <summary>
    /// The Python extension
    /// </summary>
    public const string PYTHON_EXTENSION = ".mypy";

    /// <summary>
    /// Default constructor of the package.
    /// Inside this method you can place any initialization code that does not require
    /// any Visual Studio service because at this point the package object is created but
    /// not sited yet inside Visual Studio environment. The place to do all the other
    /// initialization is the Initialize method.
    /// </summary>
    public MySqlDataProviderPackage()
    {
      _connectionsManagerDialog = null;
      _connectionsMigrationTimer = null;
      _migratingStoredConnections = false;
      if (Instance != null)
        throw new Exception("Creating second instance of package");
      Instance = this;
      Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", ToString()));

      // Update the settings file with new default values.
      UpdateSettingsFile();
    }

    internal string ConnectionName { get; set; }

    /// <summary>
    /// Gets the path for this application relative to the user's application data folder where settings can be saved.
    /// </summary>
    internal string AppDataPath
    {
      get
      {
        if (string.IsNullOrEmpty(_appDataPath))
        {
          _appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Oracle\MySQL For Visual Studio\";
        }

        return _appDataPath;
      }
    }

    internal List<IVsDataExplorerConnection> MysqlConnectionsList;

    /////////////////////////////////////////////////////////////////////////////
    // Overriden Package Implementation
    #region Package Members

    /// <summary>
    /// Initialization of the package; this method is called right after the package is sited, so this is the place
    /// where you can put all the initilaization code that rely on services provided by VisualStudio.
    /// </summary>
    protected override void Initialize()
    {
      Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", ToString()));

      MySqlProviderObjectFactory factory = new MySqlProviderObjectFactory();

      ((IServiceContainer)this).AddService(
          typeof(MySqlProviderObjectFactory), factory, true);

      base.Initialize();

      RegisterEditorFactory(new SqlEditorFactory());

      // Initialize settings related to MySQL Workbench in the MySQL Utility and InfoDialog
      InitializeMySqlWorkbenchStaticSettings();
      CustomizeUtilityDialogs();

      // load our connections
      MysqlConnectionsList = GetMySqlConnections();

      // Start timer that checks for automatic connections migration.
      StartConnectionsMigrationTimer();

      // Add our command handlers for menu (commands must exist in the .vsct file)
      OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;

      if (null != mcs)
      {
        // Create the command for the menu item.
        CommandID menuCommandId = new CommandID(GuidList.CmdSet, (int)PkgCmdIDList.cmdidConfig);
        OleMenuCommand menuItem = new OleMenuCommand(ConfigCallback, menuCommandId);
        menuItem.BeforeQueryStatus += configWizard_BeforeQueryStatus;
        mcs.AddCommand(menuItem);

        CommandID cmdOpenUtilitiesPrompt = new CommandID(GuidList.CmdSet, (int)PkgCmdIDList.cmdidOpenUtilitiesPrompt);
        OleMenuCommand cmdItem = new OleMenuCommand(OpenMySQLUtilitiesCallback, cmdOpenUtilitiesPrompt);
        cmdItem.BeforeQueryStatus += cmdOpenUtilitiesPrompt_BeforeQueryStatus;
        mcs.AddCommand(cmdItem);

        CommandID cmdLaunchWb = new CommandID(GuidList.CmdSet, (int)PkgCmdIDList.cmdidLaunchWorkbench);
        OleMenuCommand cmdMenuLaunchWb = new OleMenuCommand(LaunchWbCallback, cmdLaunchWb);
        cmdMenuLaunchWb.BeforeQueryStatus += cmdLaunchWB_BeforeQueryStatus;
        mcs.AddCommand(cmdMenuLaunchWb);

        CommandID cmdNewMySqlScript = new CommandID(GuidList.CmdSet, (int)PkgCmdIDList.cmdidNewMySQLScript);
        OleMenuCommand cmdMenuNewMySqlScript = new OleMenuCommand(NewMySqlScriptCallback, cmdNewMySqlScript);
        cmdMenuNewMySqlScript.BeforeQueryStatus += cmdMenuNewScript_BeforeQueryStatus;
        mcs.AddCommand(cmdMenuNewMySqlScript);

        CommandID cmdNewJavascript = new CommandID(GuidList.CmdSet, (int)PkgCmdIDList.cmdidNewJavascript);
        OleMenuCommand cmdMenuNewJavascript = new OleMenuCommand(NewJavascriptCallback, cmdNewJavascript);
        cmdMenuNewJavascript.BeforeQueryStatus += cmdMenuNewScript_BeforeQueryStatus;
        mcs.AddCommand(cmdMenuNewJavascript);

        CommandID cmdNewPythonScript = new CommandID(GuidList.CmdSet, (int)PkgCmdIDList.cmdidNewPythonscript);
        OleMenuCommand cmdMenuNewPythonscript = new OleMenuCommand(NewPythonScriptCallback, cmdNewPythonScript);
        cmdMenuNewPythonscript.BeforeQueryStatus += cmdMenuNewScript_BeforeQueryStatus;
        mcs.AddCommand(cmdMenuNewPythonscript);

        CommandID menuGenDbScript = new CommandID(GuidList.CmdSet, (int)PkgCmdIDList.cmdidGenerateDatabaseScript);
        OleMenuCommand menuItemGenDbScript = new OleMenuCommand(GenDbScriptCallback, menuGenDbScript);
        menuItemGenDbScript.BeforeQueryStatus += GenDbScript_BeforeQueryStatus;
        mcs.AddCommand(menuItemGenDbScript);

        CommandID cmdDbExportTool = new CommandID(GuidList.CmdSet, (int)PkgCmdIDList.cmdidDBExport);
        OleMenuCommand cmdMenuDbExport = new OleMenuCommand(cmdDbExport_Callback, cmdDbExportTool);
        cmdMenuDbExport.BeforeQueryStatus += cmdMenuDbExport_BeforeQueryStatus;
        mcs.AddCommand(cmdMenuDbExport);

        CommandID cmdAddConnection = new CommandID(GuidList.GuidIdeToolbarCmdSet, (int)PkgCmdIDList.cmdidAddConnection);
        OleMenuCommand cmdMenuAddConnection = new OleMenuCommand(cmdAddConnection_Callback, cmdAddConnection);
        mcs.AddCommand(cmdMenuAddConnection);

        CommandID cmdOpenConnectionsManager = new CommandID(GuidList.ServerExplorerToolbarCmdSet, (int)PkgCmdIDList.cmdOpenConnectionsManager);
        OleMenuCommand cmdMenuOpenConnectionsManager = new OleMenuCommand(OpenConnectionsManager_Callback, cmdOpenConnectionsManager);
        mcs.AddCommand(cmdMenuOpenConnectionsManager);

        CommandID cmdMySqlOutputWindowTool = new CommandID(GuidList.GuidMySqlOutputWindowsCmdSet, (int)PkgCmdIDList.MySqlOutputWindowCommandId);
        OleMenuCommand cmdMenuMySqlOutputWindow = new OleMenuCommand(MySqlOutputWindow_Callback, cmdMySqlOutputWindowTool);
        cmdMenuMySqlOutputWindow.BeforeQueryStatus += cmdMenuMySqlOutputWindow_BeforeQueryStatus;
        mcs.AddCommand(cmdMenuMySqlOutputWindow);
      }

      // Register and initialize language services
      MySqlLanguageService languageService = new MySqlLanguageService();
      languageService.SetSite(this);
      ((IServiceContainer)this).AddService(typeof(MySqlLanguageService), languageService, true);

      MyJsLanguageService jslanguageService = new MyJsLanguageService();
      jslanguageService.SetSite(this);
      ((IServiceContainer)this).AddService(typeof(MyJsLanguageService), jslanguageService, true);

      MyPyLanguageService pyLanguageService = new MyPyLanguageService();
      pyLanguageService.SetSite(this);
      ((IServiceContainer)this).AddService(typeof(MyPyLanguageService), pyLanguageService, true);

      // Determine whether the environment variable "MYSQLCONNECTOR_ASSEMBLIESPATH" exists.
#if NET_45_OR_GREATER
      string mySqlConnectorAssembliesVersion = "v4.5";
#else
      string mySqlConnectorAssembliesVersion = "v4.0";
#endif
      string mySqlConnectorPath = Utilities.GetMySqlAppInstallLocation("MySQL Connector/Net");
      mySqlConnectorPath = !string.IsNullOrEmpty(mySqlConnectorPath)
                            ? string.Format(@"{0}Assemblies\{1}", mySqlConnectorPath, mySqlConnectorAssembliesVersion)
                            : string.Empty;
      // If the environment variable doesn't exist, create it.
      string mySqlConnectorEnvironmentVariableValue = Environment.GetEnvironmentVariable(MYSQL_CONNECTOR_ENVIRONMENT_VARIABLE, EnvironmentVariableTarget.User);
      if (mySqlConnectorEnvironmentVariableValue == null)
      {
        if (!string.IsNullOrEmpty(mySqlConnectorPath))
        {
          SetEnvironmentVariableValues(mySqlConnectorPath);
        }
      }
      else
      {
        // If already exists, check if its original value has changed
        if (!mySqlConnectorEnvironmentVariableValue.Contains(mySqlConnectorPath, StringComparison.InvariantCultureIgnoreCase) && !string.IsNullOrEmpty(mySqlConnectorPath))
        {
          SetEnvironmentVariableValues(mySqlConnectorPath);
        }
      }
    }

    private void SetEnvironmentVariableValues(string mySqlConnectorPath)
    {
      Environment.SetEnvironmentVariable(MYSQL_CONNECTOR_ENVIRONMENT_VARIABLE, mySqlConnectorPath, EnvironmentVariableTarget.User);
      Environment.SetEnvironmentVariable(MYSQL_CONNECTOR_ENVIRONMENT_VARIABLE, mySqlConnectorPath, EnvironmentVariableTarget.Process);
    }

    /// <summary>
    /// Handles the <see cref="OleMenuCommand.BeforeQueryStatus"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private void cmdMenuNewScript_BeforeQueryStatus(object sender, EventArgs e)
    {
      var newScriptButton = sender as OleMenuCommand;
      if (newScriptButton == null)
      {
        return;
      }

      EnvDTE80.DTE2 applicationObject = GetDTE2();
      UIHierarchy uih = applicationObject.ToolWindows.GetToolWindow(EnvDTE.Constants.vsWindowKindServerExplorer) as UIHierarchy;
      if (uih == null)
      {
        return;
      }

      var selectedItems = uih.SelectedItems as Array;
      if (selectedItems != null)
      {
        ConnectionName = ((UIHierarchyItem)selectedItems.GetValue(0)).Name;
      }

      var dataExplorerConnection = GetServerExplorerConnection(ConnectionName);
      bool connected = dataExplorerConnection != null
        && dataExplorerConnection.Connection != null
        && dataExplorerConnection.Connection.State == DataConnectionState.Open;
      bool showNewScriptButton = false;
      if (connected)
      {
        if (newScriptButton.CommandID.ID == PkgCmdIDList.cmdidNewMySQLScript)
        {
          showNewScriptButton = true;
        }
        else
        {
          // Hide the option from servers that do not support the X-Protocol.
          var currentConnection = dataExplorerConnection.Connection.GetLockedProviderObject() as MySqlConnection;
          showNewScriptButton = currentConnection.ServerVersionSupportsXProtocol(false);
        }
      }

      newScriptButton.Visible = showNewScriptButton;
      newScriptButton.Enabled = showNewScriptButton;
    }

    /// <summary>
    /// News the javascript callback.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private void NewJavascriptCallback(object sender, EventArgs e)
    {
      // Set the selected connection so when the editor window is open it can work with.
      if (SelectedMySqlConnection == null)
      {
        return;
      }

      // Create New JavaScript file and open the editor with it.
      CreateNewScript(ScriptLanguageType.JavaScript);
    }

    /// <summary>
    /// News the PythonScript callback.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private void NewPythonScriptCallback(object sender, EventArgs e)
    {
      // Set the selected connection so when the editor window is open it can work with.
      if (SelectedMySqlConnection == null)
      {
        return;
      }

      // Create New PythonScript file and open the editor with it.
      CreateNewScript(ScriptLanguageType.Python);
    }

    #endregion

    private void cmdOpenUtilitiesPrompt_BeforeQueryStatus(object sender, EventArgs e)
    {
      var openUtilities = sender as OleMenuCommand;
      if (openUtilities == null)
      {
        return;
      }

      EnvDTE80.DTE2 applicationObject = GetDTE2();
      UIHierarchy uih = applicationObject.ToolWindows.GetToolWindow(EnvDTE.Constants.vsWindowKindServerExplorer) as UIHierarchy;
      if (uih == null)
      {
        return;
      }

      var selectedItems = uih.SelectedItems as Array;
      if (selectedItems != null)
        ConnectionName = ((UIHierarchyItem)selectedItems.GetValue(0)).Name;
      if (GetServerExplorerConnection(ConnectionName) != null)
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

    private void cmdLaunchWB_BeforeQueryStatus(object sender, EventArgs e)
    {
      OleMenuCommand launchWbButton = sender as OleMenuCommand;
      if (launchWbButton == null)
      {
        return;
      }

      EnvDTE80.DTE2 applicationObject = GetDTE2();
      UIHierarchy uih = applicationObject.ToolWindows.GetToolWindow(EnvDTE.Constants.vsWindowKindServerExplorer) as UIHierarchy;
      if (uih == null)
      {
        return;
      }

      var selectedItems = uih.SelectedItems as Array;
      if (selectedItems != null)
      {
        ConnectionName = ((UIHierarchyItem)selectedItems.GetValue(0)).Name;
      }

      if (GetServerExplorerConnection(ConnectionName) != null)
      {
        if (MySqlWorkbench.IsInstalled)
          launchWbButton.Visible = launchWbButton.Enabled = true;
        else
        {
          launchWbButton.Enabled = false;
          launchWbButton.Visible = true;
        }
      }
      else
        launchWbButton.Visible = launchWbButton.Enabled = false;
    }

    private void configWizard_BeforeQueryStatus(object sender, EventArgs e)
    {
      var configButton = sender as OleMenuCommand;
      if (configButton == null)
      {
        return;
      }

      configButton.Visible = false;

      ////this feature can be shown only if Connector/Net is installed too
      if (string.IsNullOrEmpty(Utilities.GetMySqlAppInstallLocation("MySQL Connector/Net")))
      {
        return;
      }

      DTE dte = GetService(typeof(DTE)) as DTE;
      if (dte == null)
      {
        return;
      }

      var a = dte.ActiveSolutionProjects as Array;
      if (a == null || a.Length != 1)
      {
        return;
      }

      Project p = (Project)a.GetValue(0);
      configButton.Visible = false;
      if (p.Properties == null)
      {
        return;
      }

      foreach (Property prop in p.Properties)
      {
        if (prop.Name != "WebSiteType" && !prop.Name.StartsWith("WebApplication", StringComparison.OrdinalIgnoreCase))
        {
          continue;
        }

        configButton.Visible = true;
        break;
      }
    }

    private void GenDbScript_BeforeQueryStatus(object sender, EventArgs e)
    {
      OleMenuCommand cmd = sender as OleMenuCommand;
      if (cmd != null)
      {
        cmd.Visible = false;
      }
    }

    private void cmdMenuDbExport_BeforeQueryStatus(object sender, EventArgs e)
    {
      OleMenuCommand dbExportButton = sender as OleMenuCommand;
      if (dbExportButton == null)
      {
        return;
      }

      EnvDTE80.DTE2 applicationObject = GetDTE2();
      UIHierarchy uih = applicationObject.ToolWindows.GetToolWindow(EnvDTE.Constants.vsWindowKindServerExplorer) as UIHierarchy;
      if (uih == null)
      {
        return;
      }

      var selectedItems = uih.SelectedItems as Array;
      dbExportButton.Visible = false;
      if (selectedItems != null)
      {
        ConnectionName = ((UIHierarchyItem)selectedItems.GetValue(0)).Name;
      }

      if (GetServerExplorerConnection(ConnectionName) != null)
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
      string currentConnectionName = GetCurrentConnectionName();
      if (SelectedMySqlConnection == null)
      {
        return;
      }

      for (int i = 0; ; i++)
      {
        ToolWindowPane existingDbExportToolWindow = FindToolWindow(typeof(DbExportWindowPane), i, false);
        if (existingDbExportToolWindow != null)
        {
          continue;
        }

        var window = CreateToolWindow(typeof(DbExportWindowPane), i) as ToolWindowPane;
        if (window == null || window.Frame == null)
        {
          throw new Exception("Cannot create a new window for data export");
        }

        window.Caption = Resources.DbExportToolCaptionFrame;
        var windowFrame = window.Frame as IVsWindowFrame;
        var windowPanel = window as DbExportWindowPane;
        if (windowFrame == null || windowPanel == null)
        {
          continue;
        }

        if (MysqlConnectionsList == null || MysqlConnectionsList.Count <= 0)
        {
          MysqlConnectionsList = GetMySqlConnections();
        }

        windowPanel.Connections = MysqlConnectionsList;
        windowPanel.SelectedConnectionName = currentConnectionName;
        windowPanel.WindowHandler = window;
        windowPanel.InitializeDbExportPanel();
        GetDTE2().Windows.Item(EnvDTE.Constants.vsWindowKindOutput).Visible = true;
        object currentFrameMode;
        windowFrame.GetProperty((int)__VSFPROPID.VSFPROPID_FrameMode, out currentFrameMode);
        // switch to dock mode.
        if ((VSFRAMEMODE) currentFrameMode == VSFRAMEMODE.VSFM_Float)
        {
          windowFrame.SetProperty((int)__VSFPROPID.VSFPROPID_FrameMode, VSFRAMEMODE.VSFM_Dock);
        }

        ErrorHandler.ThrowOnFailure(windowFrame.Show());
        break;
      }
    }

    private void OpenMySQLUtilitiesCallback(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(Utilities.GetMySqlAppInstallLocation("MySQL Utilities")))
      {
        var pathWorkbench = Utilities.GetMySqlAppInstallLocation("Workbench");
        var pathUtilities = Path.Combine(pathWorkbench, "Utilities");
        if (Directory.Exists(pathUtilities))
        {
          return;
        }

        var infoResult = InfoDialog.ShowDialog(
          InfoDialogProperties.GetOkCancelDialogProperties(
            InfoDialog.InfoType.Warning,
            Resources.MySqlDataProviderPackage_MySqlUtilitiesNotFoundError,
            @"To use them you must download and install the utilities package from http://dev.mysql.com/downloads/tools/utilities/",
            Resources.MySqlDataProviderPackage_ClickOkOrCancel));
        if (infoResult.DialogResult == DialogResult.OK)
        {
          var browserInfo = new ProcessStartInfo("http://dev.mysql.com/downloads/tools/utilities/");
          System.Diagnostics.Process.Start(browserInfo);
        }
      }
      else
      {
        MySqlWorkbench.LaunchUtilitiesShell();
      }
    }

    /// <summary>
    /// Event delegate method fired when the action command to open the connections manager is triggered.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="args">Event arguments.</param>
    private void OpenConnectionsManager_Callback(object sender, EventArgs args)
    {
      // Attemtp to migrate all locally stored connections to the MySQL Workbench connections file.
      CheckForNextAutomaticConnectionsMigration(false);

      IVsDataExplorerConnection relatedServerExplorerConnection;
      MySqlWorkbenchConnection selectedMySqlWorkbenchConnection;
      using (_connectionsManagerDialog = new MySqlX.MySqlConnectionsManagerDialog())
      {
        if (_connectionsManagerDialog.ShowDialog() != DialogResult.OK)
        {
          _connectionsManagerDialog = null;
          return;
        }

        selectedMySqlWorkbenchConnection = _connectionsManagerDialog.SelectedWorkbenchConnection;
        relatedServerExplorerConnection = _connectionsManagerDialog.RelatedServerExplorerConnection;
      }

      _connectionsManagerDialog = null;
      if (selectedMySqlWorkbenchConnection == null || Instance == null)
      {
        return;
      }

      string newConnectionName = selectedMySqlWorkbenchConnection.Name;
      if (selectedMySqlWorkbenchConnection.Existing)
      {
        newConnectionName = relatedServerExplorerConnection.DisplayName;
      }
      else
      {
        relatedServerExplorerConnection = null;
      }

      AddServerExplorerConnection(newConnectionName, selectedMySqlWorkbenchConnection.ConnectionString, relatedServerExplorerConnection);
    }

    /// <summary>
    /// Adds a new MySQL connection to the Server Explorer.
    /// </summary>
    /// <param name="connectionName">The name of the connection being added.</param>
    /// <param name="connectionString">The connection string of the connection being added.</param>
    /// <param name="removeConnectionBeforeAdd">A <see cref="IVsDataExplorerConnection"/> to remove before adding a new one.</param>
    /// <returns>A <see cref="IVsDataExplorerConnection"/> correspnding to the connection added to the Server Explorer.</returns>
    public IVsDataExplorerConnection AddServerExplorerConnection(string connectionName, string connectionString, IVsDataExplorerConnection removeConnectionBeforeAdd = null)
    {
      if (string.IsNullOrEmpty(connectionString))
      {
        return null;
      }

      var env = (DTE)GetService(typeof(DTE));
      var mySqlServiceProvider = new ServiceProvider((IOleServiceProvider)env);
      var connectionManager = mySqlServiceProvider.GetService(typeof(IVsDataExplorerConnectionManager).GUID) as IVsDataExplorerConnectionManager;
      if (connectionManager == null)
      {
        return null;
      }

      if (removeConnectionBeforeAdd != null)
      {
        connectionManager.RemoveConnection(removeConnectionBeforeAdd);
      }

      if (string.IsNullOrEmpty(connectionName))
      {
        connectionName = BaseEditorControl.UNTITLED_CONNECTION;
      }

      // Set AllowUserVariables to true
      var csb = new MySqlConnectionStringBuilder(connectionString) { AllowUserVariables = true };

      var connection = connectionManager.AddConnection(connectionName, GuidList.Provider, csb.ToString(), false);
      connection.Connection.EnsureConnected();
      return connection;
    }

    /// <summary>
    /// News the script callback.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private void NewMySqlScriptCallback(object sender, EventArgs e)
    {
      // Set the selected connection so when the editor window is open it can work with.
      if (SelectedMySqlConnection == null)
      {
        return;
      }

      // Create New SQL Script file and open the editor with it.
      CreateNewScript(ScriptLanguageType.Sql);
    }

    private void LaunchWbCallback(object sender, EventArgs e)
    {
      IVsDataExplorerConnection connection = GetServerExplorerConnection(ConnectionName);
      if (connection == null)
      {
        return;
      }

      var connStr = connection.Connection.DisplayConnectionString;
      ConnectionParameters parameters = ParseConnectionString(connStr);
      MySqlWorkbench.LaunchSqlEditor(FindMathchingWorkbenchConnection(parameters));
    }

    private void ConfigCallback(object sender, EventArgs e)
    {
      WebConfig.WebConfigDlg w = new WebConfig.WebConfigDlg();
      w.ShowDialog();
    }

    private void GenDbScriptCallback(object sender, EventArgs e)
    {
      // Get current connection
      string conStr = string.Empty;
      string script;
      EnvDTE80.DTE2 applicationObject = GetDTE2();
      UIHierarchy uih = applicationObject.ToolWindows.GetToolWindow(EnvDTE.Constants.vsWindowKindServerExplorer) as UIHierarchy;
      if (uih == null)
      {
        return;
      }

      var selectedItems = uih.SelectedItems as Array;
      if (selectedItems != null)
      {
        conStr = ((UIHierarchyItem)selectedItems.GetValue(0)).Name;
      }

      IVsDataExplorerConnection con = GetServerExplorerConnection(conStr);
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
      using (var dlg = new MySqlScriptDialog())
      {
        dlg.TextScript = script;
        dlg.ShowDialog();
      }
    }

    private void cmdAddConnection_Callback(object sender, EventArgs e)
    {
      ConnectDialog d = new ConnectDialog();
      DialogResult r = d.ShowDialog();
      if (r == DialogResult.Cancel) return;
      try
      {
        SelectedMySqlConnection = (MySqlConnection)d.Connection;
        DTE env = (DTE)GetService(typeof(DTE));
        AddServerExplorerConnection(string.Format("{0}({1})", SelectedMySqlConnection.DataSource, SelectedMySqlConnection.Database), SelectedMySqlConnection.ConnectionString);
        ItemOperations itemOp = env.ItemOperations;
        itemOp.NewFile(@"MySQL\MySQL Script", null, "{A2FE74E1-B743-11D0-AE1A-00A0C90FFFC3}");
      }
      catch (MySqlException ex)
      {
        MySqlSourceTrace.WriteAppErrorToLog(ex, Resources.VsPkg_AddConnectionErrorTitle, Resources.VsPkg_AddConnectionErrorDetail, true);
        return;
      }

      SelectedMySqlConnection = null;
    }

    public string GetCurrentConnectionName()
    {
      EnvDTE80.DTE2 applicationObject = GetDTE2();
      UIHierarchy uih = applicationObject.ToolWindows.GetToolWindow(EnvDTE.Constants.vsWindowKindServerExplorer) as UIHierarchy;
      var selectedItems = uih != null ? uih.SelectedItems as Array : null;
      SelectedMySqlConnectionName = selectedItems != null
        ? ((UIHierarchyItem)selectedItems.GetValue(0)).Name
        : string.Empty;
      return SelectedMySqlConnectionName;
    }

    /// <summary>
    /// Gets the corresponding <see cref="MySqlConnection"/>
    /// </summary>
    /// <param name="serverExplorerConnectionName"></param>
    /// <returns></returns>
    public MySqlConnection GetMySqlConnection(string serverExplorerConnectionName)
    {
      return GetMySqlConnection(GetServerExplorerConnection(serverExplorerConnectionName));
    }

    /// <summary>
    /// Returns a <see cref="MySqlConnection"/> corresponding to the given <see cref="IVsDataExplorerConnection"/>.
    /// </summary>
    /// <param name="serverExplorerConnection">A <see cref="IVsDataExplorerConnection"/>.</param>
    /// <returns>A <see cref="MySqlConnection"/> corresponding to the given <see cref="IVsDataExplorerConnection"/>.</returns>
    public MySqlConnection GetMySqlConnection(IVsDataExplorerConnection serverExplorerConnection)
    {
      if (serverExplorerConnection == null)
      {
        return null;
      }

      var mySqlConnection = serverExplorerConnection.Connection.GetLockedProviderObject() as MySqlConnection;
      if (mySqlConnection == null)
      {
        serverExplorerConnection.Connection.UnlockProviderObject();
        return null;
      }

      var csb = new MySqlConnectionStringBuilder(mySqlConnection.ConnectionString);
      csb.AllowUserVariables = true;
      mySqlConnection = new MySqlConnection(csb.ToString());
      // Get settings from current connection to assign them to the SelectedMySqlConnection
      var settingsValue = GetSettingsPropertyFromConnection(mySqlConnection) != null
                          ? GetSettingsPropertyFromConnection(mySqlConnection).GetValue(mySqlConnection, null)
                          : null;
      if (settingsValue != null)
      {
        GetSettingsPropertyFromConnection(mySqlConnection).SetValue(mySqlConnection, settingsValue, null);
      }

      serverExplorerConnection.Connection.UnlockProviderObject();
      return mySqlConnection;
    }

    /// <summary>
    /// Gets the Settings property from MySql connection.
    /// </summary>
    /// <param name="connection">The MySql connection.</param>
    /// <returns>A PropertyInfo object containing the description of the property.</returns>
    internal PropertyInfo GetSettingsPropertyFromConnection(MySqlConnection connection)
    {
      if (connection == null)
      {
        return null;
      }

      return connection.GetType().GetProperty("Settings", BindingFlags.Instance | BindingFlags.NonPublic);
    }

    internal EnvDTE80.DTE2 GetDTE2()
    {
      return GetGlobalService(typeof(DTE)) as EnvDTE80.DTE2;
    }

    /// <summary>
    /// Returns a <see cref="IVsDataExplorerConnectionManager"/> from the ones in the Server Explorer with the given connection name.
    /// </summary>
    /// <param name="connectionName">The name of the connection.</param>
    /// <returns>A <see cref="IVsDataExplorerConnectionManager"/> from the ones in the Server Explorer with the given connection name.</returns>
    public IVsDataExplorerConnection GetServerExplorerConnection(string connectionName)
    {
      if (string.IsNullOrEmpty(connectionName))
      {
        return null;
      }

      IVsDataExplorerConnectionManager connectionManager = GetService(typeof(IVsDataExplorerConnectionManager)) as IVsDataExplorerConnectionManager;
      if (connectionManager == null)
      {
        return null;
      }

      var connections = connectionManager.Connections;
      foreach (var connection in connections)
      {
        if (GuidList.Provider.Equals(connection.Value.Provider) && connection.Value.DisplayName.Equals(connectionName))
          return connection.Value;
      }

      return null;
    }

    /// <summary>
    /// Returns a <see cref="MySqlWorkbenchConnection"/> with the given connection name or if not found, one with the same connection parameters as the given <see cref="MySqlConnection"/>.
    /// </summary>
    /// <param name="connectionName">The name of the connection.</param>
    /// <param name="mySqlConnection">A <see cref="MySqlConnection"/> to compare connection parameters with.</param>
    /// <returns>A <see cref="MySqlWorkbenchConnection"/> with the given connection name or if not found, one with the same connection parameters as the given <see cref="MySqlConnection"/>.</returns>
    public MySqlWorkbenchConnection GetMySqlWorkbenchConnection(string connectionName, MySqlConnection mySqlConnection)
    {
      // Try first to retrieve a related Workbench connection using the connection name
      var mySqlWorkbenchConnection = !string.IsNullOrEmpty(connectionName)
        ? MySqlWorkbench.Connections.GetConnectionForName(connectionName)
        : null;

      // In case connection names do not match, try to retrieve a related Workbench connection by its exact connection string, for example if a user renamed the connection
      if (mySqlWorkbenchConnection == null)
      {
        mySqlWorkbenchConnection = MySqlWorkbench.Connections.GetConnectionFromMySqlConnection(mySqlConnection, true);
      }

      return mySqlWorkbenchConnection;
    }

    /// <summary>
    /// Returns a list of Server Explorer connections corresponding to MySQL Server connections.
    /// </summary>
    /// <returns>A list of Server Explorer connections corresponding to MySQL Server connections.</returns>
    public List<IVsDataExplorerConnection> GetMySqlConnections()
    {
      try
      {
        IVsDataExplorerConnectionManager connectionManager = GetService(typeof(IVsDataExplorerConnectionManager)) as IVsDataExplorerConnectionManager;
        if (connectionManager == null)
        {
          return new List<IVsDataExplorerConnection>();
        }

        var connections = connectionManager.Connections;
        MysqlConnectionsList = new List<IVsDataExplorerConnection>();
        foreach (var connection in connections)
        {
          if (GuidList.Provider.Equals(connection.Value.Provider))
            MysqlConnectionsList.Add(connection.Value);
        }

        return MysqlConnectionsList;
      }
      catch
      {
        // ignored
      }

      return new List<IVsDataExplorerConnection>();
    }

    public string GetConnectionStringBasedOnNode(string name)
    {
      try
      {
        var connectionManager = GetService(typeof(IVsDataExplorerConnectionManager)) as IVsDataExplorerConnectionManager;
        if (connectionManager == null)
        {
          return null;
        }

        var connections = connectionManager.Connections;
        string activeConnectionString = string.Empty;
        foreach (var connection in connections)
        {
          if (!GuidList.Provider.Equals(connection.Value.Provider))
          {
            continue;
          }

          var selectedNodes = connection.Value.SelectedNodes;
          foreach (var node in selectedNodes)
          {
            if (!node.Caption.Equals(name, StringComparison.InvariantCultureIgnoreCase))
            {
              continue;
            }

            try
            {
              var activeConnection = connection.Value.Connection.GetLockedProviderObject() as MySqlConnection;
              if (activeConnection != null)
              {
                var csb = (MySqlConnectionStringBuilder)activeConnection.GetType().GetProperty("Settings", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(activeConnection, null);
                if (csb != null)
                {
                  activeConnectionString = csb.ConnectionString;
                }
              }
            }
            catch
            {
              // ignored
            }
            finally
            {
              connection.Value.Connection.UnlockProviderObject();
            }
          }
        }
        return activeConnectionString;
      }
      catch
      {
        // ignored
      }

      return null;
    }

    public ConnectionParameters ParseConnectionString(string connStr)
    {
      var connStringBuilder = new MySqlConnectionStringBuilder(connStr);
      var parameters = new ConnectionParameters
      {
        UserId = connStringBuilder.UserID,
        HostName = connStringBuilder.Server,
        HostIPv4 = Utilities.GetIPv4ForHostName(connStringBuilder.Server),
        Port = Convert.ToInt32(connStringBuilder.Port),
        DataBaseName = connStringBuilder.Database,
        NamedPipesEnabled = !string.IsNullOrEmpty(connStringBuilder.PipeName),
        PipeName = connStringBuilder.PipeName
      };

      return parameters;
    }

    private string FindMathchingWorkbenchConnection(ConnectionParameters parameters)
    {
      var filteredConnections = MySqlWorkbench.Connections.Where(t => !string.IsNullOrEmpty(t.Name) && t.Port == parameters.Port);
      foreach (var c in filteredConnections)
      {
        switch (c.ConnectionMethod)
        {

          case MySqlWorkbenchConnection.ConnectionMethodType.LocalUnixSocketOrWindowsPipe:
            if (!parameters.NamedPipesEnabled || string.Compare(c.UnixSocketOrWindowsPipe, parameters.PipeName, StringComparison.OrdinalIgnoreCase) != 0)
            {
              continue;
            }
            break;

          case MySqlWorkbenchConnection.ConnectionMethodType.Tcp:
            if (c.Port != parameters.Port) continue;
            break;

          case MySqlWorkbenchConnection.ConnectionMethodType.Ssh:
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

      return string.Empty;
    }

    private void CreateNewMySqlProject(string projectType)
    {
      DTE env = (DTE)GetService(typeof(DTE));
      WizardNewProjectDialog dlg;
      ValidationsGrid.ClearMetadataCache();
      using (dlg = string.IsNullOrEmpty(Settings.Default.NewProjectDialogSelected)
            ? new WizardNewProjectDialog(projectType)
            : new WizardNewProjectDialog(Settings.Default.NewProjectDialogSelected))
      {
        var result = dlg.ShowDialog();
        if (result != DialogResult.OK)
        {
          return;
        }

        EnvDTE80.Solution2 sol = (EnvDTE80.Solution2) env.Solution;
        var solutionName = dlg.SolutionName;
        var solutionPath = dlg.ProjectPath;
        Settings.Default.NewProjectDialogSelected = dlg.ProjectType;
        Settings.Default.NewProjectLanguageSelected = dlg.Language.IndexOf("CSharp", StringComparison.Ordinal) >= 0 ? "Visual C#" : "Visual Basic";
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
        var templatePath = sol.GetProjectTemplate(dlg.ProjectType, dlg.Language);
        sol.AddFromTemplate(templatePath, solutionPath, dlg.ProjectName, dlg.CreateNewSolution);
      }
    }

    /// <summary>
    /// Creates the new script file with its corresponding extension according to the type.
    /// </summary>
    /// <param name="scriptType">Type of the script to be created.</param>
    private void CreateNewScript(ScriptLanguageType scriptType)
    {
      if (Instance == null)
      {
        return;
      }

      try
      {
        string scriptExtension = string.Empty;
        switch (scriptType)
        {
          case ScriptLanguageType.Sql:
            scriptExtension = SQL_EXTENSION;
            break;
          case ScriptLanguageType.JavaScript:
            scriptExtension = JAVASCRIPT_EXTENSION;
            break;
          case ScriptLanguageType.Python:
            scriptExtension = PYTHON_EXTENSION;
            break;
        }

        var itemOp = Instance.GetDTE2().ItemOperations;
        const string fileNameBase = "MySQL Script";
        itemOp.NewFile(@"General\Text File", fileNameBase + scriptExtension, "{A2FE74E1-B743-11D0-AE1A-00A0C90FFFC3}");
      }
      catch (Exception ex)
      {
        MySqlSourceTrace.WriteAppErrorToLog(ex, null, Resources.MySqlDataProviderPackage_CreateNewScriptError, true);
      }
    }

    /// <summary>
    /// Customizes the looks of some dialogs found in the MySQL.Utility for MySQL for Visual Studio.
    /// </summary>
    private void CustomizeUtilityDialogs()
    {
      InfoDialog.ApplicationName = AssemblyInfo.AssemblyTitle;
      InfoDialog.SuccessLogo = Resources.MySQLforVisualStudio_Success;
      InfoDialog.ErrorLogo = Resources.MySQLforVisualStudio_Error;
      InfoDialog.WarningLogo = Resources.MySQLforVisualStudio_Warning;
      InfoDialog.InformationLogo = Resources.MySQLforVisualStudio;
      PasswordDialog.ApplicationIcon = Resources.__TemplateIcon;
      PasswordDialog.SecurityLogo = Resources.MySQLforVisualStudio_Security;
    }

    /// <summary>
    /// Initializes settings for the <see cref="MySqlWorkbench"/> and <see cref="MySqlWorkbenchPasswordVault"/> classes.
    /// </summary>
    private void InitializeMySqlWorkbenchStaticSettings()
    {
      const string appName = "MySQLForVisualStudio";
      MySqlSourceTrace.LogFilePath = AppDataPath + appName + ".log";
      MySqlSourceTrace.SourceTraceClass = appName;
      MySqlWorkbench.ExternalApplicationName = AssemblyInfo.AssemblyTitle;
      MySqlWorkbenchPasswordVault.ApplicationPasswordVaultFilePath = AppDataPath + "user_data.dat";
      MySqlWorkbench.ExternalConnections.CreateDefaultConnections = !MySqlWorkbench.ConnectionsFileExists && MySqlWorkbench.Connections.Count == 0;
      MySqlWorkbench.ExternalApplicationConnectionsFilePath = AppDataPath + "connections.xml";
      SetChangeCursorDelegate();
    }

    public void SetChangeCursorDelegate()
    {
      MySqlWorkbench.ChangeCurrentCursor = delegate (Cursor cursor)
      {
      };
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

    /// <summary>
    /// Handles the Callback event of the MySqlOutputWindow control, to show the MySQL Output window.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    /// <exception cref="System.Exception">Cannot create a new window for output panel.</exception>
    private void MySqlOutputWindow_Callback(object sender, EventArgs e)
    {
      CreateMySqlOutputWindow();
    }

    /// <summary>
    /// Handles the BeforeQueryStatus event of the cmdMenuMySqlOutputWindow control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    void cmdMenuMySqlOutputWindow_BeforeQueryStatus(object sender, EventArgs e)
    {
      OleMenuCommand mySqlOutputWindowMenu = sender as OleMenuCommand;
      if (mySqlOutputWindowMenu == null)
      {
        return;
      }

      mySqlOutputWindowMenu.Visible = false;
      // Check if any "SQLEditor" or "MySqlHybridScriptEditor" editor window is visible, in order to show/hide the MySqlOutput menu item
      if (CountEditorWindows() > 0)
      {
        mySqlOutputWindowMenu.Visible = true;
        mySqlOutputWindowMenu.Enabled = true;
      }
    }

    /// <summary>
    /// Shows the MySQL Output window.
    /// </summary>
    /// <exception cref="System.Exception">Cannot create a new window for output panel.</exception>
    public void CreateMySqlOutputWindow()
    {
      ToolWindowPane window = GetMySqlOutputWindow();
      if (window == null)
      {
        window = (ToolWindowPane)CreateToolWindow(typeof(MySqlOutputWindowPane), _mySqlOutputWindowCounter);
        if (window == null || window.Frame == null)
        {
          throw new Exception("Cannot create a new window for MySql output panel.");
        }

        window.Caption = Resources.MySqlOutputToolCaptionFrame;
        _mySqlOutputWindowCounter++;
      }

      IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
      MySqlOutputWindowPane windowPanel = (MySqlOutputWindowPane)window;
      windowPanel.WindowHandler = window;
      GetDTE2().Windows.Item(EnvDTE.Constants.vsWindowKindOutput).Visible = true;
      object currentFrameMode;
      windowFrame.GetProperty((int)__VSFPROPID.VSFPROPID_FrameMode, out currentFrameMode);
      // switch to dock mode.
      if ((VSFRAMEMODE)currentFrameMode == VSFRAMEMODE.VSFM_Float)
      {
        windowFrame.SetProperty((int)__VSFPROPID.VSFPROPID_FrameMode, VSFRAMEMODE.VSFM_Dock);
      }

      ErrorHandler.ThrowOnFailure(windowFrame.Show());
    }

    /// <summary>
    /// Closes the MySQL Output window.
    /// </summary>
    public void CloseMySqlOutputWindow()
    {
      // If we don't have any MySqlHybridScriptEditor or SQLEditor windows opened, close the current MySqlOutput window (_mySqlOutputWindowCounter - 1)
      if (CountEditorWindows() > 0)
      {
        return;
      }

      var existingMySqlOutputToolWindow = FindToolWindow(typeof(MySqlOutputWindowPane), _mySqlOutputWindowCounter - 1, false);
      if (existingMySqlOutputToolWindow == null)
      {
        return;
      }

      IVsWindowFrame windowFrame = (IVsWindowFrame)existingMySqlOutputToolWindow.Frame;
      if (windowFrame != null)
      {
        ErrorHandler.ThrowOnFailure(windowFrame.CloseFrame((uint)__FRAMECLOSE.FRAMECLOSE_NoSave));
      }
    }

    /// <summary>
    /// Get the current MySqlOutputWindowPane tool window pane.
    /// </summary>
    public MySqlOutputWindowPane GetMySqlOutputWindow()
    {
      var existingMySqlOutputToolWindow = FindToolWindow(typeof(MySqlOutputWindowPane), _mySqlOutputWindowCounter - 1, false);
      return existingMySqlOutputToolWindow != null ? (MySqlOutputWindowPane)existingMySqlOutputToolWindow : null;
    }

    /// <summary>
    /// Counts all the opened editor windows.
    /// </summary>
    /// <returns></returns>
    private int CountEditorWindows()
    {
      EnvDTE80.DTE2 applicationObject = GetDTE2();
      Documents documents = applicationObject.Documents;
      return documents.Cast<Document>().Count(document =>
        document.FullName.Contains(JAVASCRIPT_EXTENSION, StringComparison.InvariantCultureIgnoreCase) ||
        document.FullName.Contains(PYTHON_EXTENSION, StringComparison.InvariantCultureIgnoreCase) ||
        document.FullName.Contains(SQL_EXTENSION, StringComparison.InvariantCultureIgnoreCase));
    }

    /// <summary>
    /// Event delegate that checks if it's time to display the dialog for connections migration.
    /// </summary>
    /// <param name="fromTimer">Flag indicating whether this method is called from a timer.</param>
    private void CheckForNextAutomaticConnectionsMigration(bool fromTimer)
    {
      // If the execution of the code that migrates connections is sitll executing, then exit.
      if (_migratingStoredConnections)
      {
        return;
      }

      // Temporarily disable the timer.
      if (fromTimer)
      {
        _connectionsMigrationTimer.Enabled = false;
      }

      // Check if the next connections migration is due now.
      bool doMigration = true;
      var nextMigrationAttempt = NextAutomaticConnectionsMigration;
      if (!fromTimer && !nextMigrationAttempt.Equals(DateTime.MinValue) && (nextMigrationAttempt.Equals(DateTime.MaxValue) || DateTime.Now.CompareTo(nextMigrationAttempt) < 0))
      {
        doMigration = false;
      }
      else if (fromTimer && nextMigrationAttempt.Equals(DateTime.MinValue) || nextMigrationAttempt.Equals(DateTime.MaxValue) || DateTime.Now.CompareTo(nextMigrationAttempt) < 0)
      {
        doMigration = false;
      }

      if (doMigration)
      {
        MigrateExternalConnectionsToWorkbench(true);
      }

      // Re-enable the timer.
      if (fromTimer)
      {
        _connectionsMigrationTimer.Enabled = true;
      }
    }

    /// <summary>
    /// Event delegate method fired when the <see cref="_connectionsMigrationTimer"/> ticks.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void ConnectionsMigrationTimer_Tick(object sender, EventArgs e)
    {
      CheckForNextAutomaticConnectionsMigration(true);
    }

    /// <summary>
    /// Attempts to migrate connections created in the MySQL for Excel's connections file to the Workbench's one.
    /// </summary>
    /// <param name="showDelayOptions">Flag indicating whether options to delay the migration are shown in case the user chooses not to migrate connections now.</param>
    public void MigrateExternalConnectionsToWorkbench(bool showDelayOptions)
    {
      _migratingStoredConnections = true;

      // If the method is not being called from the Connections Manager dialog itself, then force close the dialog.
      // This is necessary since when this code is executed from another thread the dispatch is posted to the main thread, so we don't have control over when the code
      // starts and when finishes in order to prevent the users from doing a manual migration in the options dialog, and we can't update the automatic migration date either.
      if (showDelayOptions && _connectionsManagerDialog != null)
      {
        _connectionsManagerDialog.Close();
        _connectionsManagerDialog.Dispose();
        _connectionsManagerDialog = null;
      }

      // Attempt to perform the migration
      MySqlWorkbench.MigrateExternalConnectionsToWorkbench(showDelayOptions);

      // Update settings depending on the migration outcome.
      Settings.Default.WorkbenchMigrationSucceeded = MySqlWorkbench.ConnectionsMigrationStatus == MySqlWorkbench.ConnectionsMigrationStatusType.MigrationNeededAlreadyMigrated;
      if (MySqlWorkbench.ConnectionsMigrationStatus == MySqlWorkbench.ConnectionsMigrationStatusType.MigrationNeededButNotMigrated)
      {
        Settings.Default.WorkbenchMigrationLastAttempt = DateTime.Now;
        if (showDelayOptions)
        {
          Settings.Default.WorkbenchMigrationRetryDelay = MySqlWorkbench.ConnectionsMigrationDelay.ToHours();
        }
      }
      else
      {
        Settings.Default.WorkbenchMigrationLastAttempt = DateTime.MinValue;
        Settings.Default.WorkbenchMigrationRetryDelay = 0;
      }

      Settings.Default.Save();

      // If the migration was done successfully, no need to keep the timer running.
      if (Settings.Default.WorkbenchMigrationSucceeded && _connectionsMigrationTimer != null)
      {
        _connectionsMigrationTimer.Enabled = false;
      }

      _migratingStoredConnections = false;
    }

    /// <summary>
    /// Starts the global timer that fires connections migration checks.
    /// </summary>
    private void StartConnectionsMigrationTimer()
    {
      _connectionsMigrationTimer = null;
      _migratingStoredConnections = false;

      // Determine if the timer is needed
      if (Settings.Default.WorkbenchMigrationSucceeded && !MySqlWorkbench.ExternalApplicationConnectionsFileExists)
      {
        return;
      }

      _connectionsMigrationTimer = new Timer();
      _connectionsMigrationTimer.Tick += ConnectionsMigrationTimer_Tick; ;
      _connectionsMigrationTimer.Interval = MILLISECONDS_IN_HOUR;
      _connectionsMigrationTimer.Start();
    }

    private static void UpdateSettingsFile()
    {
      // Fix the error where the settings file had main element as MySQLForExcel
      var settingsFilePath = MySqlForVisualStudioSettings.SettingsFilePath;
      if (File.Exists(settingsFilePath))
      {
        var xdoc = XDocument.Load(settingsFilePath);
        var element = xdoc.Elements("MySQLForExcel").FirstOrDefault();
        if (element != null)
        {
          element.Name = AssemblyInfo.AssemblyTitle.Replace(" ", string.Empty);
          xdoc.Save(settingsFilePath);
        }
      }
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

    int IVsInstalledProduct.ProductID(out string pbstrPid)
    {
      string fullname = Assembly.GetExecutingAssembly().FullName;
      string[] parts = fullname.Split(new char[] { '=' });
      string[] versionParts = parts[1].Split(new char[] { '.' });

      pbstrPid = string.Format("{0}.{1}.{2}", versionParts[0], versionParts[1], versionParts[2]);
      return VSConstants.S_OK;
    }

    #endregion
  }
}