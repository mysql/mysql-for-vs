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

using System.Data;
using System.Windows.Forms;
using Microsoft.VisualStudio.PlatformUI;

namespace MySql.Data.VisualStudio.Editors
{
  sealed partial class SqlEditor
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
      if (disposing)
      {
        if (components != null)
        {
          components.Dispose();
        }

        if (_command != null)
        {
          _command.Dispose();
        }

        if (Connection != null)
        {
          if (Connection.State != ConnectionState.Closed)
          {
            Connection.Close();
          }

          Connection.Dispose();
        }

#if !VS_SDK_2010
        VSColorTheme.ThemeChanged -= VSColorTheme_ThemeChanged;
#endif
      }

      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SqlEditor));
      this.EditorToolStrip = new System.Windows.Forms.ToolStrip();
      this.ConnectToolStripButton = new System.Windows.Forms.ToolStripButton();
      this.SwitchConnectionToolStripDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
      this.DisconnectToolStripButton = new System.Windows.Forms.ToolStripButton();
      this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.RunScriptToolStripButton = new System.Windows.Forms.ToolStripButton();
      this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
      this.ConnectionInfoToolStripDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
      this.ConnectionMethodToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.HostIdToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.ServerVersionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.UserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.SchemaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.splitter1 = new System.Windows.Forms.Splitter();
      this.ResultsTabControl = new System.Windows.Forms.TabControl();
      this.ResultsTabPage = new System.Windows.Forms.TabPage();
      this.ResultsDataGridView = new System.Windows.Forms.DataGridView();
      this.messagesPage = new System.Windows.Forms.TabPage();
      this.messages = new System.Windows.Forms.Label();
      this.ResultsImageList = new System.Windows.Forms.ImageList(this.components);
      this.CodeEditor = new MySql.Data.VisualStudio.Editors.VSCodeEditorUserControl();
      this.EditorToolStrip.SuspendLayout();
      this.ResultsTabControl.SuspendLayout();
      this.ResultsTabPage.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ResultsDataGridView)).BeginInit();
      this.messagesPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // EditorToolStrip
      // 
      this.EditorToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
      this.EditorToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ConnectToolStripButton,
            this.SwitchConnectionToolStripDropDownButton,
            this.DisconnectToolStripButton,
            this.toolStripSeparator1,
            this.RunScriptToolStripButton,
            this.toolStripSeparator2,
            this.ConnectionInfoToolStripDropDownButton});
      this.EditorToolStrip.Location = new System.Drawing.Point(0, 0);
      this.EditorToolStrip.Name = "EditorToolStrip";
      this.EditorToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
      this.EditorToolStrip.Size = new System.Drawing.Size(604, 25);
      this.EditorToolStrip.TabIndex = 1;
      this.EditorToolStrip.Text = "toolStrip1";
      // 
      // ConnectToolStripButton
      // 
      this.ConnectToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.ConnectToolStripButton.Image = global::MySql.Data.VisualStudio.Properties.Resources.database_connect;
      this.ConnectToolStripButton.ImageTransparentColor = System.Drawing.Color.Transparent;
      this.ConnectToolStripButton.Name = "ConnectToolStripButton";
      this.ConnectToolStripButton.Size = new System.Drawing.Size(23, 22);
      this.ConnectToolStripButton.Text = "Connect to MySQL...";
      this.ConnectToolStripButton.ToolTipText = "Connect to MySQL...";
      // 
      // SwitchConnectionToolStripDropDownButton
      // 
      this.SwitchConnectionToolStripDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.None;
      this.SwitchConnectionToolStripDropDownButton.Image = ((System.Drawing.Image)(resources.GetObject("SwitchConnectionToolStripDropDownButton.Image")));
      this.SwitchConnectionToolStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.SwitchConnectionToolStripDropDownButton.Name = "SwitchConnectionToolStripDropDownButton";
      this.SwitchConnectionToolStripDropDownButton.Size = new System.Drawing.Size(13, 22);
      this.SwitchConnectionToolStripDropDownButton.ToolTipText = "Switch connection";
      // 
      // DisconnectToolStripButton
      // 
      this.DisconnectToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.DisconnectToolStripButton.Enabled = false;
      this.DisconnectToolStripButton.Image = global::MySql.Data.VisualStudio.Properties.Resources.database_disconnect;
      this.DisconnectToolStripButton.ImageTransparentColor = System.Drawing.Color.Transparent;
      this.DisconnectToolStripButton.Name = "DisconnectToolStripButton";
      this.DisconnectToolStripButton.Size = new System.Drawing.Size(23, 22);
      this.DisconnectToolStripButton.Text = "Disconnect";
      // 
      // toolStripSeparator1
      // 
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
      // 
      // RunScriptToolStripButton
      // 
      this.RunScriptToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.RunScriptToolStripButton.Enabled = false;
      this.RunScriptToolStripButton.Image = global::MySql.Data.VisualStudio.Properties.Resources.play;
      this.RunScriptToolStripButton.ImageTransparentColor = System.Drawing.Color.Transparent;
      this.RunScriptToolStripButton.Name = "RunScriptToolStripButton";
      this.RunScriptToolStripButton.Size = new System.Drawing.Size(23, 22);
      this.RunScriptToolStripButton.Text = "Run script";
      this.RunScriptToolStripButton.ToolTipText = "Run script";
      this.RunScriptToolStripButton.Click += new System.EventHandler(this.RunScriptToolStripButton_Click);
      // 
      // toolStripSeparator2
      // 
      this.toolStripSeparator2.Name = "toolStripSeparator2";
      this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
      // 
      // ConnectionInfoToolStripDropDownButton
      // 
      this.ConnectionInfoToolStripDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ConnectionMethodToolStripMenuItem,
            this.HostIdToolStripMenuItem,
            this.ServerVersionToolStripMenuItem,
            this.UserToolStripMenuItem,
            this.SchemaToolStripMenuItem});
      this.ConnectionInfoToolStripDropDownButton.Image = global::MySql.Data.VisualStudio.Properties.Resources.sql_id;
      this.ConnectionInfoToolStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.ConnectionInfoToolStripDropDownButton.Name = "ConnectionInfoToolStripDropDownButton";
      this.ConnectionInfoToolStripDropDownButton.Size = new System.Drawing.Size(107, 22);
      this.ConnectionInfoToolStripDropDownButton.Text = "Connection...";
      // 
      // ConnectionMethodToolStripMenuItem
      // 
      this.ConnectionMethodToolStripMenuItem.Image = global::MySql.Data.VisualStudio.Properties.Resources.info;
      this.ConnectionMethodToolStripMenuItem.Name = "ConnectionMethodToolStripMenuItem";
      this.ConnectionMethodToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
      this.ConnectionMethodToolStripMenuItem.Text = "Connection Method: <none>";
      // 
      // HostIdToolStripMenuItem
      // 
      this.HostIdToolStripMenuItem.Image = global::MySql.Data.VisualStudio.Properties.Resources.info;
      this.HostIdToolStripMenuItem.Name = "HostIdToolStripMenuItem";
      this.HostIdToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
      this.HostIdToolStripMenuItem.Text = "Host ID: <none>";
      // 
      // ServerVersionToolStripMenuItem
      // 
      this.ServerVersionToolStripMenuItem.Image = global::MySql.Data.VisualStudio.Properties.Resources.info;
      this.ServerVersionToolStripMenuItem.Name = "ServerVersionToolStripMenuItem";
      this.ServerVersionToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
      this.ServerVersionToolStripMenuItem.Text = "Server Version: <none>";
      // 
      // UserToolStripMenuItem
      // 
      this.UserToolStripMenuItem.Image = global::MySql.Data.VisualStudio.Properties.Resources.info;
      this.UserToolStripMenuItem.Name = "UserToolStripMenuItem";
      this.UserToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
      this.UserToolStripMenuItem.Text = "User: <none>";
      // 
      // SchemaToolStripMenuItem
      // 
      this.SchemaToolStripMenuItem.Image = global::MySql.Data.VisualStudio.Properties.Resources.info;
      this.SchemaToolStripMenuItem.Name = "SchemaToolStripMenuItem";
      this.SchemaToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
      this.SchemaToolStripMenuItem.Text = "Schema: <none>";
      // 
      // splitter1
      // 
      this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
      this.splitter1.Location = new System.Drawing.Point(0, 271);
      this.splitter1.Name = "splitter1";
      this.splitter1.Size = new System.Drawing.Size(604, 3);
      this.splitter1.TabIndex = 3;
      this.splitter1.TabStop = false;
      // 
      // ResultsTabControl
      // 
      this.ResultsTabControl.Controls.Add(this.ResultsTabPage);
      this.ResultsTabControl.Controls.Add(this.messagesPage);
      this.ResultsTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ResultsTabControl.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
      this.ResultsTabControl.ImageList = this.ResultsImageList;
      this.ResultsTabControl.Location = new System.Drawing.Point(0, 274);
      this.ResultsTabControl.Name = "ResultsTabControl";
      this.ResultsTabControl.SelectedIndex = 0;
      this.ResultsTabControl.Size = new System.Drawing.Size(604, 192);
      this.ResultsTabControl.TabIndex = 4;
      // 
      // ResultsTabPage
      // 
      this.ResultsTabPage.Controls.Add(this.ResultsDataGridView);
      this.ResultsTabPage.ImageIndex = 1;
      this.ResultsTabPage.Location = new System.Drawing.Point(4, 23);
      this.ResultsTabPage.Name = "ResultsTabPage";
      this.ResultsTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.ResultsTabPage.Size = new System.Drawing.Size(596, 165);
      this.ResultsTabPage.TabIndex = 0;
      this.ResultsTabPage.Text = "Results";
      this.ResultsTabPage.UseVisualStyleBackColor = true;
      // 
      // ResultsDataGridView
      // 
      this.ResultsDataGridView.AllowUserToAddRows = false;
      this.ResultsDataGridView.AllowUserToDeleteRows = false;
      this.ResultsDataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
      this.ResultsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.ResultsDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ResultsDataGridView.Location = new System.Drawing.Point(3, 3);
      this.ResultsDataGridView.Name = "ResultsDataGridView";
      this.ResultsDataGridView.Size = new System.Drawing.Size(590, 159);
      this.ResultsDataGridView.TabIndex = 0;
      // 
      // messagesPage
      // 
      this.messagesPage.Controls.Add(this.messages);
      this.messagesPage.ImageIndex = 0;
      this.messagesPage.Location = new System.Drawing.Point(4, 23);
      this.messagesPage.Name = "messagesPage";
      this.messagesPage.Padding = new System.Windows.Forms.Padding(3);
      this.messagesPage.Size = new System.Drawing.Size(596, 158);
      this.messagesPage.TabIndex = 1;
      this.messagesPage.Text = "Messages";
      this.messagesPage.UseVisualStyleBackColor = true;
      // 
      // messages
      // 
      this.messages.Dock = System.Windows.Forms.DockStyle.Fill;
      this.messages.Location = new System.Drawing.Point(3, 3);
      this.messages.Name = "messages";
      this.messages.Size = new System.Drawing.Size(590, 152);
      this.messages.TabIndex = 0;
      // 
      // ResultsImageList
      // 
      this.ResultsImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ResultsImageList.ImageStream")));
      this.ResultsImageList.TransparentColor = System.Drawing.Color.Transparent;
      this.ResultsImageList.Images.SetKeyName(0, "messages_icon.png");
      this.ResultsImageList.Images.SetKeyName(1, "results_icon.png");
      // 
      // CodeEditor
      // 
      this.CodeEditor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.CodeEditor.Dock = System.Windows.Forms.DockStyle.Top;
      this.CodeEditor.IsDirty = false;
      this.CodeEditor.Location = new System.Drawing.Point(0, 25);
      this.CodeEditor.Name = "CodeEditor";
      this.CodeEditor.Size = new System.Drawing.Size(604, 246);
      this.CodeEditor.TabIndex = 2;
      // 
      // SqlEditor
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.ResultsTabControl);
      this.Controls.Add(this.splitter1);
      this.Controls.Add(this.CodeEditor);
      this.Controls.Add(this.EditorToolStrip);
      this.Name = "SqlEditor";
      this.Size = new System.Drawing.Size(604, 466);
      this.EditorToolStrip.ResumeLayout(false);
      this.EditorToolStrip.PerformLayout();
      this.ResultsTabControl.ResumeLayout(false);
      this.ResultsTabPage.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.ResultsDataGridView)).EndInit();
      this.messagesPage.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ToolStrip EditorToolStrip;
    private VSCodeEditorUserControl CodeEditor;
    private System.Windows.Forms.Splitter splitter1;
    private System.Windows.Forms.TabControl ResultsTabControl;
    private System.Windows.Forms.TabPage ResultsTabPage;
    private System.Windows.Forms.TabPage messagesPage;
    private System.Windows.Forms.ToolStripButton ConnectToolStripButton;
    private System.Windows.Forms.ToolStripButton RunScriptToolStripButton;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    private System.Windows.Forms.ToolStripButton DisconnectToolStripButton;
    private System.Windows.Forms.DataGridView ResultsDataGridView;
    private System.Windows.Forms.Label messages;
    private System.Windows.Forms.ImageList ResultsImageList;
    private ToolStripDropDownButton ConnectionInfoToolStripDropDownButton;
    private ToolStripMenuItem ConnectionMethodToolStripMenuItem;
    private ToolStripMenuItem HostIdToolStripMenuItem;
    private ToolStripMenuItem ServerVersionToolStripMenuItem;
    private ToolStripMenuItem UserToolStripMenuItem;
    private ToolStripMenuItem SchemaToolStripMenuItem;
    private ToolStripDropDownButton SwitchConnectionToolStripDropDownButton;
  }
}
