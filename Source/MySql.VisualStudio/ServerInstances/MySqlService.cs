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


using Microsoft.Win32;
using MySql.Utility.Classes;
using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.VisualStudio.DBExport;
using MySql.Data.MySqlClient;
using Microsoft.VisualStudio.Data.Services;
using EnvDTE;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using System.Text.RegularExpressions;
using MySql.Data.VisualStudio.Properties;
using MySql.Utility.Classes.Logging;

namespace MySql.Data.VisualStudio
{
  public class MySqlService
  {
    internal ServiceController winService;
    internal string serviceName;
    internal ServiceControllerStatus status;
    internal MySqlStartupParameters parameters;
    internal bool realMySqlService;

    public MySqlService(string serviceName)
    {
      winService = new ServiceController(serviceName);
      status = winService.Status;
      GetStartupParameters();
    }

    private bool IsRealMySQLService(string cmd)
    {
      return cmd.EndsWith("mysqld.exe") || cmd.EndsWith("mysqld-nt.exe") || cmd.EndsWith("mysqld") || cmd.EndsWith("mysqld-nt");
    }

    private MySqlStartupParameters GetStartupParameters()
    {

      parameters.PipeName = "mysql";

      // get our host information
      parameters.HostName = winService.MachineName == "." ? "localhost" : winService.MachineName;
      parameters.HostIPv4 = Utilities.GetIPv4ForHostName(parameters.HostName);

      RegistryKey key = Registry.LocalMachine.OpenSubKey(String.Format(@"SYSTEM\CurrentControlSet\Services\{0}", winService.ServiceName));
      string imagepath = (string)key.GetValue("ImagePath", null);
      key.Close();
      if (imagepath == null) return parameters;

      string[] args = Utilities.SplitArgs(imagepath);
      realMySqlService = IsRealMySQLService(args[0]);

      // Parse our command line args
      Mono.Options.OptionSet p = new Mono.Options.OptionSet()
        .Add("defaults-file=", "", v => parameters.DefaultsFile = v)
        .Add("port=|P=", "", v => Int32.TryParse(v, out parameters.Port))
        .Add("enable-named-pipe", v => parameters.NamedPipesEnabled = true)
        .Add("socket=", "", v => parameters.PipeName = v);
      p.Parse(args);
      if (parameters.DefaultsFile == null) return parameters;

      // we have a valid defaults file
      try
      {
        IniFile f = new IniFile(parameters.DefaultsFile);
        Int32.TryParse(f.ReadValue("mysqld", "port", parameters.Port.ToString()), out parameters.Port);
        parameters.PipeName = f.ReadValue("mysqld", "socket", parameters.PipeName);
        // now see if named pipes are enabled
        parameters.NamedPipesEnabled = parameters.NamedPipesEnabled || f.HasKey("mysqld", "enable-named-pipe");
      }
      catch
      { }
      
      return parameters;
    }

  }


  public struct MySqlStartupParameters
  {
    public string DefaultsFile;
    public string HostName;
    public string HostIPv4;
    public int Port;
    public string PipeName;
    public bool NamedPipesEnabled;
  }


  public static class MySqlServiceInstances
  { 
    public static List<MySqlService> GetMySqlInstalledInstances()
    {
      var foundMySqlServices = new List<MySqlService>();
      var services = Service.GetInstances(".*mysqld.*");
      
      foreach (var item in services)
         foundMySqlServices.Add(new MySqlService(item.Properties["DisplayName"].Value.ToString()));

      if (foundMySqlServices.Count > 0)
        foundMySqlServices = foundMySqlServices.Where(t => t.realMySqlService).ToList();

      return foundMySqlServices;
    }  
  }

  internal static class MySqlServerExplorerConnections
  {
    internal static void ShowNewConnectionDialog(TextBox connectionStringTextBox, DTE dte, ComboBox cmbConnections, bool addSEConnection)
    {
      ConnectDialog dlg;

      if (dte == null)
      {
         throw new ArgumentNullException("dte");         
      }

      try
      {

        MySqlConnectionStringBuilder settings = connectionStringTextBox.Tag != null ? new MySqlConnectionStringBuilder(connectionStringTextBox.Tag.ToString()) : new MySqlConnectionStringBuilder();

        dlg = connectionStringTextBox.Tag == null ? new ConnectDialog() : new ConnectDialog(settings);

        DialogResult res = dlg.ShowDialog();
        if (res == DialogResult.OK)
        {

          if ((MySqlConnection)dlg.Connection == null) return;
          
          var csb = (MySqlConnectionStringBuilder)((MySqlConnection)dlg.Connection).GetType().GetProperty("Settings", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(((MySqlConnection)dlg.Connection), null);
          if (csb == null) return;
          
           //make sure we don't have already the same connection
           if (cmbConnections.FindString(String.Format("{0}({1})", csb.Server, csb.Database)) < 0)
           {              
              connectionStringTextBox.Tag = csb.ConnectionString;
              if (!String.IsNullOrEmpty(connectionStringTextBox.Tag.ToString()) && addSEConnection)
              {
                // adding connection to server explorer connections          
                Microsoft.VisualStudio.Shell.ServiceProvider sp = new Microsoft.VisualStudio.Shell.ServiceProvider((IOleServiceProvider)dte);
                IVsDataExplorerConnectionManager seConnectionsMgr = (IVsDataExplorerConnectionManager)sp.GetService(typeof(IVsDataExplorerConnectionManager).GUID);
                seConnectionsMgr.AddConnection(string.Format("{0}({1})", csb.Server, csb.Database), Guids.Provider, connectionStringTextBox.Tag.ToString(), false);

                var connections = (List<MySqlServerExplorerConnection>)cmbConnections.DataSource;
                connections.Add(new MySqlServerExplorerConnection { DisplayName = string.Format("{0}({1})", csb.Server, csb.Database), ConnectionString = csb.ConnectionString });
                cmbConnections.DataSource = null;                
                cmbConnections.DataSource = connections;
                cmbConnections.ValueMember = "ConnectionString";
                cmbConnections.DisplayMember = "DisplayName";                
              }
            }
           cmbConnections.Text = String.Format("{0}({1})", csb.Server, csb.Database);
           connectionStringTextBox.Text = MaskPassword(csb.ConnectionString);
           connectionStringTextBox.Tag = csb.ConnectionString;
        }    
      }
      catch (Exception ex)
      {
        Logger.LogError($"The connection string is not valid: {ex.Message}", true);
      }      
    }


    internal static BindingSource LoadMySqlConnectionsFromServerExplorer(DTE dte)
    {
      Microsoft.VisualStudio.Shell.ServiceProvider sp = new Microsoft.VisualStudio.Shell.ServiceProvider((IOleServiceProvider)dte);
      IVsDataExplorerConnectionManager seConnectionsMgr = (IVsDataExplorerConnectionManager)sp.GetService(typeof(IVsDataExplorerConnectionManager).GUID);

      var mysqlDataExplorerConnections = new List<IVsDataExplorerConnection>();

      if (seConnectionsMgr != null)
      {
        IDictionary<string, IVsDataExplorerConnection> serverExplorerconnections = seConnectionsMgr.Connections;
        foreach (var connection in serverExplorerconnections)
        {
          if (Guids.Provider.Equals(connection.Value.Provider))
            mysqlDataExplorerConnections.Add(connection.Value);
        }
      }
      
      var connections = new BindingSource();
      connections.DataSource = new List<MySqlServerExplorerConnection>();
      if (mysqlDataExplorerConnections != null)
      {
          foreach (IVsDataExplorerConnection con in mysqlDataExplorerConnections)
          {
             // get complete connections
              try
              {
                  var activeConnection = (MySqlConnection)(con.Connection).GetLockedProviderObject();
                  if (activeConnection != null)
                  {
                      var csb = (MySqlConnectionStringBuilder)activeConnection.GetType().GetProperty("Settings", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(activeConnection, null);
                      if (csb != null)
                      {
                          connections.Add(new MySqlServerExplorerConnection { DisplayName = con.DisplayName, ConnectionString = csb.ConnectionString });
                      }
                  }
              }
              catch { }
              finally
              {
                  (con.Connection).UnlockProviderObject();
              }              
          }
      }

      return connections;    
    }


    internal static void LoadConnectionsForWizard(BindingSource mySqlConnections, ComboBox cmbConnections, TextBox connectionStringTextBox, string wizardName)
    {

      string connectionFromSettings = string.Empty;

      switch (wizardName)
      {
        case "CSharpMVC":
          connectionFromSettings = Settings.Default.MVCWizardConnection;   
          break;
        case "CSharpWinForms":
          connectionFromSettings = Settings.Default.WinFormsWizardConnection;   
          break;
        case "VBMVC":
          break;
        case "VBWinForms":
          break;        
      }

 
      if (mySqlConnections == null)
          return;

      var connections = mySqlConnections.DataSource as List<MySqlServerExplorerConnection>; 

      if (mySqlConnections != null)
      {
     
        if (connections != null && connections.Count() == 0)
        {
          var mysqlInstances = MySqlServiceInstances.GetMySqlInstalledInstances();
          if (mysqlInstances.Count > 0)
          {
            foreach (var instance in mysqlInstances)
            {
              var mysqlConnection = new MySqlServerExplorerConnection();
              mysqlConnection.DisplayName = instance.serviceName;
              var csb = new MySqlConnectionStringBuilder();
              csb.Server = instance.parameters.HostName;
              csb.Port = (uint)instance.parameters.Port;
              csb.UserID = "root";
              csb.Password = "";
              mysqlConnection.ConnectionString = csb.ConnectionString;
              connections.Add(mysqlConnection);
            }
          }
        }
      }

      if (cmbConnections.DataSource != null)
        cmbConnections.DataSource = null;

      cmbConnections.DataSource = connections; 
      cmbConnections.DisplayMember = "DisplayName";
      cmbConnections.ValueMember = "ConnectionString";
      
      if (!String.IsNullOrEmpty(connectionFromSettings) && cmbConnections.Items.Count > 0)
      {
        cmbConnections.Text = connectionFromSettings;
        connectionStringTextBox.Text = MaskPassword(cmbConnections.SelectedValue.ToString());
        connectionStringTextBox.Tag = cmbConnections.SelectedValue.ToString();
      }
      else if (connections != null && connections.Count > 0)
      {
        cmbConnections.SelectedValue = ((MySqlServerExplorerConnection)cmbConnections.Items[0]).ConnectionString;
        connectionStringTextBox.Text = MaskPassword(((MySqlServerExplorerConnection)cmbConnections.Items[0]).ConnectionString);
        connectionStringTextBox.Tag = ((MySqlServerExplorerConnection)cmbConnections.Items[0]).ConnectionString;
      }         
    }  
  
    internal static string MaskPassword(string connectionString)
    {
      if (!(connectionString.IndexOf("password", StringComparison.InvariantCultureIgnoreCase) != -1))
            return connectionString;

      var regex = new Regex("password=[^;]*;", RegexOptions.IgnoreCase);
      return regex.Replace(connectionString, string.Format("password={0};", new string('*',8)));
    }  
  }
}
