// Copyright © 2016, Oracle and/or its affiliates. All rights reserved.
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
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualStudio.Data.Services;
using MySql.Data.VisualStudio.Properties;
using MySQL.Utility.Classes;
using MySQL.Utility.Classes.MySQLWorkbench;
using MySQL.Utility.Forms;

namespace MySql.Data.VisualStudio.MySqlX
{
  /// <summary>
  /// Dialog window showing MySQL Server Instances to select for monitoring.
  /// </summary>
  public partial class MySqlConnectionsManagerDialog : AutoStyleableBaseDialog
  {
    #region Fields

    /// <summary>
    /// Last filter for connection names to only list ones containing the filter text.
    /// </summary>
    private string _lastServicesNameFilter;

    /// <summary>
    /// List of MySQL connections already present in the Server Explorer window.
    /// </summary>
    private List<IVsDataExplorerConnection> _serverExplorerConnections;

    /// <summary>
    /// Flag indicating whether the connections list is being shown as a tiles view.
    /// </summary>
    private bool _viewAsTiles;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlConnectionsManagerDialog"/> class.
    /// </summary>
    public MySqlConnectionsManagerDialog()
    {
      _serverExplorerConnections = null;
      _viewAsTiles = true;

      InitializeComponent();

      ResetConnectionsViewMode(false);
      _lastServicesNameFilter = FilterTextBox.Text;
      if (MySqlDataProviderPackage.Instance != null)
      {
        _serverExplorerConnections = MySqlDataProviderPackage.Instance.GetMySqlConnections();
      }
    }

    #region Properties

    /// <summary>
    /// Gets the <see cref="IVsDataExplorerConnection"/> related to the <see cref="SelectedWorkbenchConnection"/> if any.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IVsDataExplorerConnection RelatedServerExplorerConnection { get; private set; }

    /// <summary>
    /// Gets the Workbench connection selected to be monitored.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public MySqlWorkbenchConnection SelectedWorkbenchConnection { get; private set; }

    /// <summary>
    /// Gets a value indicating whether there is a selected connection and is valid (i.e. it is not a Fabric Managed one).
    /// </summary>
    private bool IsSelectedConnectionValid
    {
      get
      {
        if (WorkbenchConnectionsListView.SelectedItems.Count == 0)
        {
          return false;
        }

        var selectedListViewItem = WorkbenchConnectionsListView.SelectedItems[0];
        var selectedWorkbenchConnection = selectedListViewItem.Tag as MySqlWorkbenchConnection;
        return selectedWorkbenchConnection != null && !selectedWorkbenchConnection.IsFabricManaged;
      }
    }

    #endregion Properties

    /// <summary>
    /// Event delegate method fired when the <see cref="AddConnectionButton"/> button is clicked.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void AddConnectionButton_Click(object sender, EventArgs e)
    {
      using (var instanceConnectionDialog = new MySqlWorkbenchConnectionDialog(null))
      {
        instanceConnectionDialog.Icon = Resources.__TemplateIcon;
        instanceConnectionDialog.ShowIcon = true;
        if (instanceConnectionDialog.ShowIfWorkbenchNotRunning() != DialogResult.OK)
        {
          return;
        }
      }

      RefreshConnectionsList(false);
    }

    /// <summary>
    /// Adds a MySQL Workbench connection to the list of connections.
    /// </summary>
    /// <param name="workbenchConnection">Workbench connection to add to the list.</param>
    private void AddWorkbenchConnectionToConnectionsList(MySqlWorkbenchConnection workbenchConnection)
    {
     var similarFound = CheckIfSimilarConnectionExistsInServerExplorer(workbenchConnection);
      var newItem = new ListViewItem(new[]
        {
          workbenchConnection.Name,
          workbenchConnection.HostIdentifier,
          workbenchConnection.ConnectionMethod.GetDescription()
        }, GetConnetionImageIndexFromType(workbenchConnection),
        WorkbenchConnectionsListView.Groups[similarFound ? 1 : 0]);

      newItem.ForeColor = workbenchConnection.IsFabricManaged ? SystemColors.InactiveCaption : SystemColors.WindowText;
      newItem.Tag = workbenchConnection;
      WorkbenchConnectionsListView.Items.Add(newItem);
    }

    /// <summary>
    /// Checks if a similar connection to the given <see cref="MySqlWorkbenchConnection"/> already exists in the Server Explorer.
    /// </summary>
    /// <param name="workbenchConnection">A <see cref="MySqlWorkbenchConnection"/> instance.</param>
    /// <returns><c>true</c> if a similar connection to the given <see cref="MySqlWorkbenchConnection"/> already exists in the Server Explorer, <c>false</c> otherwise.</returns>
    private bool CheckIfSimilarConnectionExistsInServerExplorer(MySqlWorkbenchConnection workbenchConnection)
    {
      if (workbenchConnection == null || _serverExplorerConnections == null)
      {
        return false;
      }

      // Check for identical names first.
      if (_serverExplorerConnections.Any(sec => sec.DisplayName.Equals(workbenchConnection.Name, StringComparison.InvariantCultureIgnoreCase)))
      {
        workbenchConnection.Existing = true;
        return true;
      }

      // Check then for similar host parameters
      return _serverExplorerConnections.Any(sec => sec.Connection.SafeConnectionString.CompareHostParameters(workbenchConnection.ConnectionString, false));
    }

    /// <summary>
    /// Event delegate method fired when the <see cref="ConnectionsContextMenuStrip"/> context menu is being opened.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments</param>
    private void ConnectionsContextMenuStrip_Opening(object sender, CancelEventArgs e)
    {
      bool itemIsSelected = WorkbenchConnectionsListView.SelectedItems.Count > 0;
      DeleteConnectionToolStripMenuItem.Visible = itemIsSelected;
      EditConnectionToolStripMenuItem.Visible = itemIsSelected;
      ToolStripSeparator1.Visible = itemIsSelected;
    }

    /// <summary>
    /// Event delegate method fired when the <see cref="DeleteConnectionToolStripMenuItem"/> context menu item is clicked.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void DeleteConnectionToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (WorkbenchConnectionsListView.SelectedItems.Count == 0)
      {
        return;
      }

      var workbenchConnection = WorkbenchConnectionsListView.SelectedItems[0].Tag as MySqlWorkbenchConnection;
      if (workbenchConnection == null || !MySqlWorkbench.Connections.DeleteConnection(workbenchConnection.Id))
      {
        return;
      }

      RefreshConnectionsList(false);
    }

    /// <summary>
    /// Event delegate method fired when the <see cref="EditConnectionToolStripMenuItem"/> context menu item is clicked.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void EditConnectionToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (WorkbenchConnectionsListView.SelectedItems.Count == 0)
      {
        return;
      }

      var workbenchConnection = WorkbenchConnectionsListView.SelectedItems[0].Tag as MySqlWorkbenchConnection;
      if (workbenchConnection == null)
      {
        return;
      }

      using (var instanceConnectionDialog = new MySqlWorkbenchConnectionDialog(workbenchConnection))
      {
        instanceConnectionDialog.Icon = Resources.__TemplateIcon;
        instanceConnectionDialog.ShowIcon = true;
        if (instanceConnectionDialog.ShowIfWorkbenchNotRunning() != DialogResult.OK)
        {
          return;
        }
      }

      RefreshConnectionsList(false);
    }

    /// <summary>
    /// Event delegate method fired when the <see cref="FilterTextBox"/> textbox's text changes.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void FilterTextBox_TextChanged(object sender, EventArgs e)
    {
      FilterTimer.Stop();
      FilterTimer.Start();
    }

    /// <summary>
    /// Event delegate method fired when the <see cref="FilterTextBox"/> textbox was validated.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void FilterTextBox_Validated(object sender, EventArgs e)
    {
      FilterTimer_Tick(FilterTimer, EventArgs.Empty);
    }

    /// <summary>
    /// Event delegate method fired when the <see cref="FilterTimer"/> timer's elapses.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void FilterTimer_Tick(object sender, EventArgs e)
    {
      bool filter = FilterTimer.Enabled;
      FilterTimer.Stop();
      if (filter)
      {
        RefreshConnectionsList(false);
      }
    }

    /// <summary>
    /// Returns the index of the image that corresponds to the <see cref="MySqlWorkbenchConnection.ConnectionMethodType"/> of the given <see cref="MySqlWorkbenchConnection"/>.
    /// </summary>
    /// <param name="connection">A <see cref="MySqlWorkbenchConnection"/> instance.</param>
    /// <returns>The index of the image that corresponds to the <see cref="MySqlWorkbenchConnection.ConnectionMethodType"/>.</returns>
    private int GetConnetionImageIndexFromType(MySqlWorkbenchConnection connection)
    {
      if (connection == null)
      {
        return -1;
      }

      if (connection.IsFabricManaged)
      {
        return 0;
      }

      switch (connection.ConnectionMethod)
      {
        case MySqlWorkbenchConnection.ConnectionMethodType.FabricManaged:
          return 0;
        case MySqlWorkbenchConnection.ConnectionMethodType.LocalUnixSocketOrWindowsPipe:
          return 1;
        case MySqlWorkbenchConnection.ConnectionMethodType.Ssh:
          return 2;
        case MySqlWorkbenchConnection.ConnectionMethodType.Tcp:
          return 3;
        case MySqlWorkbenchConnection.ConnectionMethodType.XProtocol:
          return 4;
        default:
          return -1;
      }
    }

    /// <summary>
    /// Event delegate method fired before the <see cref="MySqlConnectionsManagerDialog"/> dialog is closed.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void MySqlConnectionsManagerDialog_FormClosing(object sender, FormClosingEventArgs e)
    {
      SelectedWorkbenchConnection = null;
      if (DialogResult != DialogResult.OK || WorkbenchConnectionsListView.SelectedItems.Count == 0)
      {
        return;
      }

      var selectedListViewItem = WorkbenchConnectionsListView.SelectedItems[0];
      SelectedWorkbenchConnection = selectedListViewItem.Tag as MySqlWorkbenchConnection;
      if (SelectedWorkbenchConnection == null || !SelectedWorkbenchConnection.Existing)
      {
        return;
      }

      using (var yesNoDialog = new InfoDialog(InfoDialogProperties.GetYesNoDialogProperties(
        InfoDialog.InfoType.Info,
        Resources.MySqlConnectionsManagerDialogExistingConnectionTitle,
        string.Format(Resources.MySqlConnectionsManagerDialogExistingConnectionDetail, SelectedWorkbenchConnection.HostIdentifier),
        Resources.MySqlConnectionsManagerDialogExistingConnectionSubDetail)))
      {
        yesNoDialog.DefaultButton = InfoDialog.DefaultButtonType.Button2;
        yesNoDialog.DefaultButtonTimeout = 10;
        if (yesNoDialog.ShowDialog() != DialogResult.No)
        {
          SelectedWorkbenchConnection = null;
          e.Cancel = true;
        }
      }

      if (SelectedWorkbenchConnection != null && SelectedWorkbenchConnection.Existing)
      {
        RelatedServerExplorerConnection = _serverExplorerConnections.FirstOrDefault(seConn => seConn.Connection.DisplayConnectionString.Equals(SelectedWorkbenchConnection.ConnectionString));
      }
    }

    /// <summary>
    /// Event delegate method fired when the <see cref="MySqlConnectionsManagerDialog"/> dialog is shown.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void MySqlConnectionsManagerDialog_Shown(object sender, EventArgs e)
    {
      RefreshConnectionsList(true);
    }

    /// <summary>
    /// Reloads the list of MySQL Server instances from the ones contained in the MySQL Workbench connections file.
    /// </summary>
    /// <param name="forceRefresh">Flag indicating if the refresh must be done although filters haven't changed.</param>
    private void RefreshConnectionsList(bool forceRefresh)
    {
      if (_lastServicesNameFilter != null && _lastServicesNameFilter != FilterTextBox.Text)
      {
        _lastServicesNameFilter = FilterTextBox.Text;
      }

      if (forceRefresh)
      {
        MySqlWorkbench.Connections.Load();
      }

      RefreshConnectionsList(_lastServicesNameFilter);
    }

    /// <summary>
    /// Reloads the list of MySQL Server instances from the ones contained in the MySQL Workbench connections file.
    /// </summary>
    /// <param name="connectionNameFilter">Filter for connection names to only list ones containing the filter text.</param>
    private void RefreshConnectionsList(string connectionNameFilter)
    {
      if (MySqlWorkbench.Connections.Count == 0)
      {
        return;
      }

      if (!string.IsNullOrEmpty(connectionNameFilter))
      {
        connectionNameFilter = connectionNameFilter.ToLowerInvariant();
      }

      WorkbenchConnectionsListView.Items.Clear();
      WorkbenchConnectionsListView.BeginUpdate();

      foreach (var connection in MySqlWorkbench.Connections
                                  .OrderBy(conn => conn.Name)
                                  .Where(connection => !connection.IsUnknownConnection && connectionNameFilter != null && (string.IsNullOrEmpty(connectionNameFilter) || connection.Name.ToLowerInvariant().Contains(connectionNameFilter))))
      {
        AddWorkbenchConnectionToConnectionsList(connection);
      }

      WorkbenchConnectionsListView.EndUpdate();
      DialogOKButton.Enabled = false;
    }

    /// <summary>
    /// Event delegate method fired when the <see cref="RefreshConnectionsToolStripMenuItem"/> is clicked.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void RefreshConnectionsToolStripMenuItem_Click(object sender, EventArgs e)
    {
      RefreshConnectionsList(true);
    }

    /// <summary>
    /// Resets the view mode of the connections list view control.
    /// </summary>
    /// <param name="flipViewMode">Flag indicating whether the view mode is flipped before resetting it.</param>
    private void ResetConnectionsViewMode(bool flipViewMode)
    {
      if (flipViewMode)
      {
        _viewAsTiles = !_viewAsTiles;
      }

      if (_viewAsTiles)
      {
        WorkbenchConnectionsListView.View = View.Tile;
        ViewAsToolStripMenuItem.Text = Resources.MySqlConnectionsManagerDialogViewAsDetails;
        ViewAsToolStripMenuItem.Image = Resources.list_view;
      }
      else
      {
        WorkbenchConnectionsListView.View = View.Details;
        ViewAsToolStripMenuItem.Text = Resources.MySqlConnectionsManagerDialogViewAsTiles;
        ViewAsToolStripMenuItem.Image = Resources.tile_view;
      }
    }

    /// <summary>
    /// Event delegate method fired when the <see cref="ViewAsToolStripMenuItem"/> is clicked.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void ViewAsListToolStripMenuItem_Click(object sender, EventArgs e)
    {
      ResetConnectionsViewMode(true);
    }

    /// <summary>
    /// Event delegate method fired when the <see cref="WorkbenchConnectionsListView"/> control is double-clicked.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void WorkbenchConnectionsListView_DoubleClick(object sender, EventArgs e)
    {
      if (!IsSelectedConnectionValid)
      {
        return;
      }

      DialogOKButton.PerformClick();
    }

    /// <summary>
    /// Event delegate method fired when the <see cref="WorkbenchConnectionsListView"/> selected itemText's index changes.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void WorkbenchConnectionsListView_SelectedIndexChanged(object sender, EventArgs e)
    {
      DialogOKButton.Enabled = IsSelectedConnectionValid;
    }
  }
}