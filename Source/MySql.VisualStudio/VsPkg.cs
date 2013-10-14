// Copyright © 2008, 2013, Oracle and/or its affiliates. All rights reserved.
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
using MySQL.Utility;
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
using System.Windows.Forms;


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
  [ProvideLoadKey("Standard", "1.0", "MySQL for Visual Studio", "MySQL AB c/o MySQL, Inc.", 100)]
  // This attribute is needed to let the shell know that this package exposes some menus.
  [ProvideMenuResource(1000, 1)]
  [ProvideToolWindow(typeof(DbExportWindowPane),Style = VsDockStyle.Tabbed, Window = EnvDTE.Constants.vsWindowKindMainWindow)]    
  // This attribute registers a tool window exposed by this package.
  [Guid(GuidStrings.Package)]  
  public sealed class MySqlDataProviderPackage : Package, IVsInstalledProduct
  {
    public static MySqlDataProviderPackage Instance;

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
        throw new Exception("Creating second instance of package");
      Instance = this;
      Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
    }

    internal string ConnectionName { get; set; }

    /////////////////////////////////////////////////////////////////////////////
    // Overriden Package Implementation
    #region Package Members

    /// <summary>
    /// Initialization of the package; this method is called right after the package is sited, so this is the place
    /// where you can put all the initilaization code that rely on services provided by VisualStudio.
    /// </summary>
    protected override void Initialize()
    {
      Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));

      MySqlProviderObjectFactory factory = new MySqlProviderObjectFactory();

      ((IServiceContainer)this).AddService(
          typeof(MySqlProviderObjectFactory), factory, true);

      base.Initialize();

      RegisterEditorFactory(new SqlEditorFactory());

      // Add our command handlers for menu (commands must exist in the .vsct file)
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

        CommandID cmdSchemaCompare = new CommandID(Guids.CmdSet, (int)PkgCmdIDList.cmdidSchemaCompare );
        OleMenuCommand cmdMenuSchemaCompare = new OleMenuCommand(SchemaCompareCallback, cmdSchemaCompare);
        cmdMenuSchemaCompare.BeforeQueryStatus += new EventHandler(cmdSchemaCompare_BeforeQueryStatus);
        mcs.AddCommand(cmdMenuSchemaCompare);

        CommandID cmdSchemaCompareTo = new CommandID(Guids.CmdSet, (int)PkgCmdIDList.cmdidSchemaCompareTo);
        OleMenuCommand cmdMenuSchemaCompareTo = new OleMenuCommand(SchemaCompareToCallback, cmdSchemaCompareTo);
        cmdMenuSchemaCompareTo.BeforeQueryStatus += new EventHandler(cmdSchemaCompareTo_BeforeQueryStatus);
        mcs.AddCommand(cmdMenuSchemaCompareTo);

        CommandID cmdDbExportTool = new CommandID(Guids.CmdSet, (int)PkgCmdIDList.cmdidDBExport);
        OleMenuCommand cmdMenuDbExport = new OleMenuCommand(cmdDbExport_Callback, cmdDbExportTool);
        cmdMenuDbExport.BeforeQueryStatus += new EventHandler(cmdMenuDbExport_BeforeQueryStatus);
        mcs.AddCommand(cmdMenuDbExport);

      }

      // Register and initialize language service
      MySqlLanguageService languageService = new MySqlLanguageService();
      languageService.SetSite(this);
      ((IServiceContainer)this).AddService(typeof(MySqlLanguageService), languageService, true);
    }

    #endregion
    
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
          openUtilities.Enabled = false;
      }
      else
        openUtilities.Visible =  openUtilities.Enabled = false;
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
          launchWBbtn.Enabled = false;
      }
      else
        launchWBbtn.Visible = launchWBbtn.Enabled = false;
    }

    void configWizard_BeforeQueryStatus(object sender, EventArgs e)
    {
      OleMenuCommand configButton = sender as OleMenuCommand;
      configButton.Visible = false;

      ////this feature can be shown only if Connector/Net is installed too
      //if (String.IsNullOrEmpty(Utility.GetInstallLocation("MySQL Connector/Net")))
      //  return;

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

    void GenDbScript_BeforeQueryStatus(object sender, EventArgs e)
    {
      OleMenuCommand cmd = sender as OleMenuCommand;
      cmd.Visible = false;
    }

    private MySqlConnection firstCon = null;
    private MySqlConnection secondCon = null;

    void cmdSchemaCompare_BeforeQueryStatus(object sender, EventArgs e)
    {
      OleMenuCommand configButton = sender as OleMenuCommand;
      configButton.Visible = false;
      //if( firstCon != null )
      //  configButton.Visible = true;
    }

    void cmdSchemaCompareTo_BeforeQueryStatus(object sender, EventArgs e)
    {
      OleMenuCommand configButton = sender as OleMenuCommand;
      configButton.Visible = false;
    }

    void cmdMenuDbExport_BeforeQueryStatus(object sender, EventArgs e)
    {
      OleMenuCommand dbExportButton = sender as OleMenuCommand;
      dbExportButton.Visible = true;
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
                      throw new Exception("Cannot create new window");

                  window.Caption = String.Format(Resources.DbExportToolCaptionFrame, currentConnectionName);
                
                  IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;

                  DbExportWindowPane windowPanel = (DbExportWindowPane)window;

                  windowPanel.Connections = GetMySqlConnections();
                  windowPanel.SelectedConnectionName = currentConnectionName;
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
        MySqlWorkbench.LaunchSQLEditor(FindMathchingWorkbenchConnection(parameters));      
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

    /// <summary>
    /// Compare stores the second connection and triggers the comparison.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SchemaCompareCallback(object sender, EventArgs e)
    {
      secondCon = GetCurrentConnection();
      SchemaComparerForm form = new SchemaComparerForm();
      form.SourceConnection = firstCon;
      form.DestinyConnection = secondCon;
      form.ShowDialog();
    }

    /// <summary>
    /// CompareTo just stores the first connection
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SchemaCompareToCallback(object sender, EventArgs e)
    {
      firstCon = GetCurrentConnection();
    }


    private string GetCurrentConnectionName()
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
        if (connection != null)
          return new MySqlConnection(connection.ConnectionString);
        return null;
      }
      catch 
      {
        return null;
      }
    }   

    private EnvDTE80.DTE2 GetDTE2()
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

    internal List<IVsDataExplorerConnection> GetMySqlConnections()
    {
      IVsDataExplorerConnectionManager connectionManager = GetService(typeof(IVsDataExplorerConnectionManager)) as IVsDataExplorerConnectionManager;
      if (connectionManager == null) return null;

      System.Collections.Generic.IDictionary<string, IVsDataExplorerConnection> connections = connectionManager.Connections;

      List<IVsDataExplorerConnection> mysqlConnections = new List<IVsDataExplorerConnection>();        
      foreach (var connection in connections)
      {
          if (Guids.Provider.Equals(connection.Value.Provider))
              mysqlConnections.Add(connection.Value);              
      }
      return mysqlConnections;
    }


    public ConnectionParameters ParseConnectionString(string connStr)
    {

      var connStringBuilder = new MySql.Data.MySqlClient.MySqlConnectionStringBuilder(connStr);
      var parameters = new ConnectionParameters();
      parameters.UserId = connStringBuilder.UserID;
      parameters.HostName = connStringBuilder.Server;
      parameters.HostIPv4 = Utility.GetIPv4ForHostName(connStringBuilder.Server);
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
          switch (c.DriverType)
          {

            case MySqlWorkbenchConnectionType.NamedPipes:
              if (!parameters.NamedPipesEnabled || String.Compare(c.Socket, parameters.PipeName, true) != 0) continue;
              break;
            case MySqlWorkbenchConnectionType.Ssh:
              continue;
            case MySqlWorkbenchConnectionType.Tcp:
              if (c.Port != parameters.Port) continue;
              break;
            case MySqlWorkbenchConnectionType.Unknown:
              continue;
          }

          if (!Utility.IsValidIpAddress(c.Host)) //matching connections by Ip
          {
            if (Utility.GetIPv4ForHostName(c.Host) != parameters.HostIPv4) continue;
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