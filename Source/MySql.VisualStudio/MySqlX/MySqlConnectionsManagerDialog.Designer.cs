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

namespace MySql.Data.VisualStudio.MySqlX
{
  partial class MySqlConnectionsManagerDialog
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Available", System.Windows.Forms.HorizontalAlignment.Left);
      System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Similar already in Server Explorer", System.Windows.Forms.HorizontalAlignment.Left);
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MySqlConnectionsManagerDialog));
      this.ConnectionNameLabel = new System.Windows.Forms.Label();
      this.FilterTextBox = new System.Windows.Forms.TextBox();
      this.MySQLConnectionsHelpLabel = new System.Windows.Forms.Label();
      this.MySQLConnectionsHyperTitleLabel = new System.Windows.Forms.Label();
      this.WorkbenchConnectionsListView = new System.Windows.Forms.ListView();
      this.ConnectionNameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.HostnameIdColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.ConnectionTypeColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.ConnectionsContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.DeleteConnectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.EditConnectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.ToolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.RefreshConnectionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.ViewAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.ConnectionTypesImageList = new System.Windows.Forms.ImageList(this.components);
      this.DialogCancelButton = new System.Windows.Forms.Button();
      this.AddConnectionButton = new System.Windows.Forms.Button();
      this.DialogOKButton = new System.Windows.Forms.Button();
      this.FilterTimer = new System.Windows.Forms.Timer(this.components);
      this.ContentAreaPanel.SuspendLayout();
      this.CommandAreaPanel.SuspendLayout();
      this.ConnectionsContextMenuStrip.SuspendLayout();
      this.SuspendLayout();
      // 
      // FootnoteAreaPanel
      // 
      this.FootnoteAreaPanel.Location = new System.Drawing.Point(0, 292);
      this.FootnoteAreaPanel.Size = new System.Drawing.Size(634, 0);
      // 
      // ContentAreaPanel
      // 
      this.ContentAreaPanel.Controls.Add(this.CommandAreaPanel);
      this.ContentAreaPanel.Controls.Add(this.ConnectionNameLabel);
      this.ContentAreaPanel.Controls.Add(this.FilterTextBox);
      this.ContentAreaPanel.Controls.Add(this.MySQLConnectionsHelpLabel);
      this.ContentAreaPanel.Controls.Add(this.MySQLConnectionsHyperTitleLabel);
      this.ContentAreaPanel.Controls.Add(this.WorkbenchConnectionsListView);
      this.ContentAreaPanel.Size = new System.Drawing.Size(744, 571);
      this.ContentAreaPanel.Controls.SetChildIndex(this.WorkbenchConnectionsListView, 0);
      this.ContentAreaPanel.Controls.SetChildIndex(this.MySQLConnectionsHyperTitleLabel, 0);
      this.ContentAreaPanel.Controls.SetChildIndex(this.MySQLConnectionsHelpLabel, 0);
      this.ContentAreaPanel.Controls.SetChildIndex(this.FilterTextBox, 0);
      this.ContentAreaPanel.Controls.SetChildIndex(this.ConnectionNameLabel, 0);
      this.ContentAreaPanel.Controls.SetChildIndex(this.CommandAreaPanel, 0);
      // 
      // CommandAreaPanel
      // 
      this.CommandAreaPanel.BackColor = System.Drawing.SystemColors.Control;
      this.CommandAreaPanel.Controls.Add(this.DialogCancelButton);
      this.CommandAreaPanel.Controls.Add(this.AddConnectionButton);
      this.CommandAreaPanel.Controls.Add(this.DialogOKButton);
      this.CommandAreaPanel.Location = new System.Drawing.Point(0, 526);
      this.CommandAreaPanel.Size = new System.Drawing.Size(744, 45);
      this.CommandAreaPanel.TabIndex = 6;
      // 
      // ConnectionNameLabel
      // 
      this.ConnectionNameLabel.AutoSize = true;
      this.ConnectionNameLabel.Location = new System.Drawing.Point(25, 109);
      this.ConnectionNameLabel.Name = "ConnectionNameLabel";
      this.ConnectionNameLabel.Size = new System.Drawing.Size(36, 15);
      this.ConnectionNameLabel.TabIndex = 2;
      this.ConnectionNameLabel.Text = "Filter:";
      // 
      // FilterTextBox
      // 
      this.FilterTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.FilterTextBox.Location = new System.Drawing.Point(67, 106);
      this.FilterTextBox.Name = "FilterTextBox";
      this.FilterTextBox.Size = new System.Drawing.Size(652, 23);
      this.FilterTextBox.TabIndex = 3;
      this.FilterTextBox.TextChanged += new System.EventHandler(this.FilterTextBox_TextChanged);
      this.FilterTextBox.Validated += new System.EventHandler(this.FilterTextBox_Validated);
      // 
      // MySQLConnectionsHelpLabel
      // 
      this.MySQLConnectionsHelpLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.MySQLConnectionsHelpLabel.Location = new System.Drawing.Point(24, 53);
      this.MySQLConnectionsHelpLabel.Name = "MySQLConnectionsHelpLabel";
      this.MySQLConnectionsHelpLabel.Size = new System.Drawing.Size(695, 38);
      this.MySQLConnectionsHelpLabel.TabIndex = 1;
      this.MySQLConnectionsHelpLabel.Text = "Select a MySQL connection from the list to add it to Visual Studio\'s Server Explo" +
    "rer.\r\nYou can filter the list by typing into the filter control.";
      // 
      // MySQLConnectionsHyperTitleLabel
      // 
      this.MySQLConnectionsHyperTitleLabel.AutoSize = true;
      this.MySQLConnectionsHyperTitleLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.MySQLConnectionsHyperTitleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(73)))), ((int)(((byte)(161)))));
      this.MySQLConnectionsHyperTitleLabel.Location = new System.Drawing.Point(20, 23);
      this.MySQLConnectionsHyperTitleLabel.Name = "MySQLConnectionsHyperTitleLabel";
      this.MySQLConnectionsHyperTitleLabel.Size = new System.Drawing.Size(261, 21);
      this.MySQLConnectionsHyperTitleLabel.TabIndex = 0;
      this.MySQLConnectionsHyperTitleLabel.Text = "Choose a MySQL Server connection:";
      // 
      // WorkbenchConnectionsListView
      // 
      this.WorkbenchConnectionsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.WorkbenchConnectionsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ConnectionNameColumnHeader,
            this.HostnameIdColumnHeader,
            this.ConnectionTypeColumnHeader});
      this.WorkbenchConnectionsListView.ContextMenuStrip = this.ConnectionsContextMenuStrip;
      this.WorkbenchConnectionsListView.FullRowSelect = true;
      listViewGroup1.Header = "Available";
      listViewGroup1.Name = "AvailableListViewGroup";
      listViewGroup2.Header = "Similar already in Server Explorer";
      listViewGroup2.Name = "InServerExplorerListViewGroup";
      this.WorkbenchConnectionsListView.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2});
      this.WorkbenchConnectionsListView.HideSelection = false;
      this.WorkbenchConnectionsListView.LargeImageList = this.ConnectionTypesImageList;
      this.WorkbenchConnectionsListView.Location = new System.Drawing.Point(25, 135);
      this.WorkbenchConnectionsListView.MultiSelect = false;
      this.WorkbenchConnectionsListView.Name = "WorkbenchConnectionsListView";
      this.WorkbenchConnectionsListView.Size = new System.Drawing.Size(694, 363);
      this.WorkbenchConnectionsListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
      this.WorkbenchConnectionsListView.TabIndex = 5;
      this.WorkbenchConnectionsListView.TileSize = new System.Drawing.Size(330, 65);
      this.WorkbenchConnectionsListView.UseCompatibleStateImageBehavior = false;
      this.WorkbenchConnectionsListView.View = System.Windows.Forms.View.Tile;
      this.WorkbenchConnectionsListView.SelectedIndexChanged += new System.EventHandler(this.WorkbenchConnectionsListView_SelectedIndexChanged);
      this.WorkbenchConnectionsListView.DoubleClick += new System.EventHandler(this.WorkbenchConnectionsListView_DoubleClick);
      // 
      // ConnectionNameColumnHeader
      // 
      this.ConnectionNameColumnHeader.DisplayIndex = 1;
      this.ConnectionNameColumnHeader.Text = "Name";
      this.ConnectionNameColumnHeader.Width = 235;
      // 
      // HostnameIdColumnHeader
      // 
      this.HostnameIdColumnHeader.DisplayIndex = 2;
      this.HostnameIdColumnHeader.Text = "Host";
      this.HostnameIdColumnHeader.Width = 199;
      // 
      // ConnectionTypeColumnHeader
      // 
      this.ConnectionTypeColumnHeader.DisplayIndex = 0;
      this.ConnectionTypeColumnHeader.Text = "Type";
      this.ConnectionTypeColumnHeader.Width = 101;
      // 
      // ConnectionsContextMenuStrip
      // 
      this.ConnectionsContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DeleteConnectionToolStripMenuItem,
            this.EditConnectionToolStripMenuItem,
            this.ToolStripSeparator1,
            this.RefreshConnectionsToolStripMenuItem,
            this.ViewAsToolStripMenuItem});
      this.ConnectionsContextMenuStrip.Name = "ConnectionsContextMenuStrip";
      this.ConnectionsContextMenuStrip.Size = new System.Drawing.Size(184, 120);
      this.ConnectionsContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.ConnectionsContextMenuStrip_Opening);
      // 
      // DeleteConnectionToolStripMenuItem
      // 
      this.DeleteConnectionToolStripMenuItem.Image = global::MySql.Data.VisualStudio.Properties.Resources.delete;
      this.DeleteConnectionToolStripMenuItem.Name = "DeleteConnectionToolStripMenuItem";
      this.DeleteConnectionToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
      this.DeleteConnectionToolStripMenuItem.Text = "Delete Connection";
      this.DeleteConnectionToolStripMenuItem.Click += new System.EventHandler(this.DeleteConnectionToolStripMenuItem_Click);
      // 
      // EditConnectionToolStripMenuItem
      // 
      this.EditConnectionToolStripMenuItem.Image = global::MySql.Data.VisualStudio.Properties.Resources.edit;
      this.EditConnectionToolStripMenuItem.Name = "EditConnectionToolStripMenuItem";
      this.EditConnectionToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
      this.EditConnectionToolStripMenuItem.Text = "Edit Connection";
      this.EditConnectionToolStripMenuItem.Click += new System.EventHandler(this.EditConnectionToolStripMenuItem_Click);
      // 
      // ToolStripSeparator1
      // 
      this.ToolStripSeparator1.Name = "ToolStripSeparator1";
      this.ToolStripSeparator1.Size = new System.Drawing.Size(180, 6);
      // 
      // RefreshConnectionsToolStripMenuItem
      // 
      this.RefreshConnectionsToolStripMenuItem.Image = global::MySql.Data.VisualStudio.Properties.Resources.refresh;
      this.RefreshConnectionsToolStripMenuItem.Name = "RefreshConnectionsToolStripMenuItem";
      this.RefreshConnectionsToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
      this.RefreshConnectionsToolStripMenuItem.Text = "Refresh Connections";
      this.RefreshConnectionsToolStripMenuItem.Click += new System.EventHandler(this.RefreshConnectionsToolStripMenuItem_Click);
      // 
      // ViewAsToolStripMenuItem
      // 
      this.ViewAsToolStripMenuItem.Name = "ViewAsToolStripMenuItem";
      this.ViewAsToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
      this.ViewAsToolStripMenuItem.Text = "View as...";
      this.ViewAsToolStripMenuItem.Click += new System.EventHandler(this.ViewAsListToolStripMenuItem_Click);
      // 
      // ConnectionTypesImageList
      // 
      this.ConnectionTypesImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ConnectionTypesImageList.ImageStream")));
      this.ConnectionTypesImageList.TransparentColor = System.Drawing.Color.Transparent;
      this.ConnectionTypesImageList.Images.SetKeyName(0, "Connection-Fabric.png");
      this.ConnectionTypesImageList.Images.SetKeyName(1, "Connection-Socket-32x32.png");
      this.ConnectionTypesImageList.Images.SetKeyName(2, "Connection-SSH.png");
      this.ConnectionTypesImageList.Images.SetKeyName(3, "Connection-TCP-32x32.png");
      this.ConnectionTypesImageList.Images.SetKeyName(4, "Connection-X-32x32.png");
      // 
      // DialogCancelButton
      // 
      this.DialogCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.DialogCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.DialogCancelButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.DialogCancelButton.Location = new System.Drawing.Point(644, 11);
      this.DialogCancelButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.DialogCancelButton.Name = "DialogCancelButton";
      this.DialogCancelButton.Size = new System.Drawing.Size(75, 23);
      this.DialogCancelButton.TabIndex = 2;
      this.DialogCancelButton.Text = "Cancel";
      this.DialogCancelButton.UseVisualStyleBackColor = true;
      // 
      // AddConnectionButton
      // 
      this.AddConnectionButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.AddConnectionButton.Location = new System.Drawing.Point(25, 11);
      this.AddConnectionButton.Name = "AddConnectionButton";
      this.AddConnectionButton.Size = new System.Drawing.Size(157, 23);
      this.AddConnectionButton.TabIndex = 0;
      this.AddConnectionButton.Text = "Add New Connection...";
      this.AddConnectionButton.UseVisualStyleBackColor = true;
      this.AddConnectionButton.Click += new System.EventHandler(this.AddConnectionButton_Click);
      // 
      // DialogOKButton
      // 
      this.DialogOKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.DialogOKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.DialogOKButton.Enabled = false;
      this.DialogOKButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.DialogOKButton.Location = new System.Drawing.Point(563, 11);
      this.DialogOKButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.DialogOKButton.Name = "DialogOKButton";
      this.DialogOKButton.Size = new System.Drawing.Size(75, 23);
      this.DialogOKButton.TabIndex = 1;
      this.DialogOKButton.Text = "OK";
      this.DialogOKButton.UseVisualStyleBackColor = true;
      // 
      // FilterTimer
      // 
      this.FilterTimer.Interval = 500;
      this.FilterTimer.Tick += new System.EventHandler(this.FilterTimer_Tick);
      // 
      // MySqlConnectionsManagerDialog
      // 
      this.AcceptButton = this.DialogOKButton;
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
      this.CancelButton = this.DialogCancelButton;
      this.ClientSize = new System.Drawing.Size(744, 571);
      this.CommandAreaVisible = true;
      this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
      this.FootnoteAreaHeight = 0;
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MinimumSize = new System.Drawing.Size(400, 300);
      this.Name = "MySqlConnectionsManagerDialog";
      this.Text = "MySQL Connections Manager";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MySqlConnectionsManagerDialog_FormClosing);
      this.Shown += new System.EventHandler(this.MySqlConnectionsManagerDialog_Shown);
      this.Controls.SetChildIndex(this.ContentAreaPanel, 0);
      this.Controls.SetChildIndex(this.FootnoteAreaPanel, 0);
      this.ContentAreaPanel.ResumeLayout(false);
      this.ContentAreaPanel.PerformLayout();
      this.CommandAreaPanel.ResumeLayout(false);
      this.ConnectionsContextMenuStrip.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button DialogCancelButton;
    private System.Windows.Forms.Button DialogOKButton;
    private System.Windows.Forms.ListView WorkbenchConnectionsListView;
    private System.Windows.Forms.ColumnHeader ConnectionNameColumnHeader;
    private System.Windows.Forms.ColumnHeader HostnameIdColumnHeader;
    private System.Windows.Forms.Label MySQLConnectionsHelpLabel;
    private System.Windows.Forms.Label MySQLConnectionsHyperTitleLabel;
    private System.Windows.Forms.Button AddConnectionButton;
    private System.Windows.Forms.ColumnHeader ConnectionTypeColumnHeader;
    private System.Windows.Forms.Label ConnectionNameLabel;
    private System.Windows.Forms.TextBox FilterTextBox;
    private System.Windows.Forms.Timer FilterTimer;
    private System.Windows.Forms.ContextMenuStrip ConnectionsContextMenuStrip;
    private System.Windows.Forms.ToolStripMenuItem RefreshConnectionsToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem DeleteConnectionToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem EditConnectionToolStripMenuItem;
    private System.Windows.Forms.ImageList ConnectionTypesImageList;
    private System.Windows.Forms.ToolStripMenuItem ViewAsToolStripMenuItem;
    private System.Windows.Forms.ToolStripSeparator ToolStripSeparator1;
  }
}