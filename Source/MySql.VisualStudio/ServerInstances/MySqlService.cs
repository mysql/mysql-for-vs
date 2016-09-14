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

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.Data.Services;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using MySql.Data.VisualStudio.DBExport;
using MySql.Data.VisualStudio.Editors;
using MySql.Data.VisualStudio.Properties;
using MySql.Utility.Classes;
using MySql.Utility.Classes.MySql;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace MySql.Data.VisualStudio.ServerInstances
{
  public class MySqlService
  {
    internal ServiceController WinService;
    internal string ServiceName;
    internal ServiceControllerStatus Status;
    internal MySqlStartupParameters Parameters;
    internal bool RealMySqlService;

    public MySqlService(string serviceName)
    {
      WinService = new ServiceController(serviceName);
      Status = WinService.Status;
      FillStartupParameters();
    }

    private bool IsRealMySQLService(string cmd)
    {
      return cmd.EndsWith("mysqld.exe") || cmd.EndsWith("mysqld-nt.exe") || cmd.EndsWith("mysqld") || cmd.EndsWith("mysqld-nt");
    }

    private void FillStartupParameters()
    {
      Parameters.PipeName = "mysql";

      // get our host information
      Parameters.HostName = WinService.MachineName == "." ? "localhost" : WinService.MachineName;
      Parameters.HostIPv4 = Utilities.GetIPv4ForHostName(Parameters.HostName);

      RegistryKey key = Registry.LocalMachine.OpenSubKey(string.Format(@"SYSTEM\CurrentControlSet\Services\{0}", WinService.ServiceName));
      if (key != null)
      {
        string imagepath = (string)key.GetValue("ImagePath", null);
        key.Close();
        if (imagepath == null)
        {
          return;
        }

        string[] args = Utilities.SplitArgs(imagepath);
        RealMySqlService = IsRealMySQLService(args[0]);

        // Parse our command line args
        var p = new Mono.Options.OptionSet()
          .Add("defaults-file=", "", v => Parameters.DefaultsFile = v)
          .Add("port=|P=", "", v => int.TryParse(v, out Parameters.Port))
          .Add("enable-named-pipe", v => Parameters.NamedPipesEnabled = true)
          .Add("socket=", "", v => Parameters.PipeName = v);
        p.Parse(args);
      }
      if (Parameters.DefaultsFile == null)
      {
        return;
      }

      // we have a valid defaults file
      try
      {
        IniFile f = new IniFile(Parameters.DefaultsFile);
        int.TryParse(f.ReadValue("mysqld", "port", Parameters.Port.ToString()), out Parameters.Port);
        Parameters.PipeName = f.ReadValue("mysqld", "socket", Parameters.PipeName);
        // now see if named pipes are enabled
        Parameters.NamedPipesEnabled = Parameters.NamedPipesEnabled || f.HasKey("mysqld", "enable-named-pipe");
      }
      catch
      {
        // ignored
      }
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
      var services = Service.GetInstances(".*mysqld.*");
      var foundMySqlServices = services.Select(item => new MySqlService(item.Properties["DisplayName"].Value.ToString())).ToList();
      if (foundMySqlServices.Count > 0)
      {
        foundMySqlServices = foundMySqlServices.Where(t => t.RealMySqlService).ToList();
      }

      return foundMySqlServices;
    }
  }

  internal static class MySqlServerExplorerConnections
  {
    internal static void ShowNewConnectionDialog(TextBox connectionStringTextBox, DTE dte, ComboBox cmbConnections, bool addSeConnection)
    {
      if (dte == null)
      {
        throw new ArgumentNullException("dte");
      }

      try
      {
        var settings = connectionStringTextBox.Tag != null
          ? new MySqlConnectionStringBuilder(connectionStringTextBox.Tag.ToString())
          : new MySqlConnectionStringBuilder();

        var dlg = connectionStringTextBox.Tag == null ? new ConnectDialog() : new ConnectDialog(settings);

        DialogResult res = dlg.ShowDialog();
        if (res != DialogResult.OK)
        {
          return;
        }

        if ((MySqlConnection) dlg.Connection == null)
        {
          return;
        }

        var csb = (MySqlConnectionStringBuilder)((MySqlConnection)dlg.Connection).GetType().GetProperty("Settings", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(((MySqlConnection)dlg.Connection), null);
        if (csb == null)
        {
          return;
        }

        //make sure we don't have already the same connection
        if (cmbConnections.FindString(string.Format("{0}({1})", csb.Server, csb.Database)) < 0)
        {
          connectionStringTextBox.Tag = csb.ConnectionString;
          if (!string.IsNullOrEmpty(connectionStringTextBox.Tag.ToString()) && addSeConnection)
          {
            // adding connection to server explorer connections
            Microsoft.VisualStudio.Shell.ServiceProvider sp = new Microsoft.VisualStudio.Shell.ServiceProvider((IOleServiceProvider)dte);
            IVsDataExplorerConnectionManager seConnectionsMgr = (IVsDataExplorerConnectionManager)sp.GetService(typeof(IVsDataExplorerConnectionManager).GUID);
            seConnectionsMgr.AddConnection(string.Format("{0}({1})", csb.Server, csb.Database), GuidList.Provider, connectionStringTextBox.Tag.ToString(), false);

            var connections = (List<MySqlServerExplorerConnection>)cmbConnections.DataSource;
            connections.Add(new MySqlServerExplorerConnection { DisplayName = string.Format("{0}({1})", csb.Server, csb.Database), ConnectionString = csb.ConnectionString });
            cmbConnections.DataSource = null;
            cmbConnections.DataSource = connections;
            cmbConnections.ValueMember = "ConnectionString";
            cmbConnections.DisplayMember = "DisplayName";
          }
        }
        cmbConnections.Text = string.Format("{0}({1})", csb.Server, csb.Database);
        connectionStringTextBox.Text = MaskPassword(csb.ConnectionString);
        connectionStringTextBox.Tag = csb.ConnectionString;
      }
      catch (Exception ex)
      {
        MySqlSourceTrace.WriteAppErrorToLog(ex, null, "The connection string is not valid.", true);
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
          if (GuidList.Provider.Equals(connection.Value.Provider))
            mysqlDataExplorerConnections.Add(connection.Value);
        }
      }

      var connections = new BindingSource();
      connections.DataSource = new List<MySqlServerExplorerConnection>();
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
        catch
        {
          // ignored
        }
        finally
        {
          (con.Connection).UnlockProviderObject();
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
      {
        return;
      }

      var connections = mySqlConnections.DataSource as List<MySqlServerExplorerConnection>;
      if (connections != null && connections.Count == 0)
      {
        var mysqlInstances = MySqlServiceInstances.GetMySqlInstalledInstances();
        if (mysqlInstances.Count > 0)
        {
          foreach (var instance in mysqlInstances)
          {
            var mysqlConnection = new MySqlServerExplorerConnection();
            mysqlConnection.DisplayName = instance.ServiceName;
            var csb = new MySqlConnectionStringBuilder
            {
              Server = instance.Parameters.HostName,
              Port = (uint) instance.Parameters.Port,
              UserID = "root",
              Password = ""
            };
            mysqlConnection.ConnectionString = csb.ConnectionString;
            connections.Add(mysqlConnection);
          }
        }
      }

      if (cmbConnections.DataSource != null)
      {
        cmbConnections.DataSource = null;
      }

      cmbConnections.DataSource = connections;
      cmbConnections.DisplayMember = "DisplayName";
      cmbConnections.ValueMember = "ConnectionString";

      if (!string.IsNullOrEmpty(connectionFromSettings) && cmbConnections.Items.Count > 0)
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
      return regex.Replace(connectionString, string.Format("password={0};", new string('*', 8)));
    }
  }
}
