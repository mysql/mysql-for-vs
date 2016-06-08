// Copyright © 2015, 2016, Oracle and/or its affiliates. All rights reserved.
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

using System.Windows.Forms;

namespace MySql.Data.VisualStudio.Editors
{
  partial class MySqlHybridScriptEditor
  {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MySqlHybridScriptEditor));
      this.toolStrip1 = new System.Windows.Forms.ToolStrip();
      this.ConnectToolStripButton = new System.Windows.Forms.ToolStripButton();
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
      this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
      this.ExecutionModeToolStripDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
      this.BatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.ConsoleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
      this.SessionOptionsToolStripDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
      this.PreserveVariablesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.splitter1 = new System.Windows.Forms.Splitter();
      this.ResultsTabControl = new System.Windows.Forms.TabControl();
      this.resultsPage = new System.Windows.Forms.TabPage();
      this.resultsGrid = new System.Windows.Forms.DataGridView();
      this.messagesPage = new System.Windows.Forms.TabPage();
      this.messages = new System.Windows.Forms.Label();
      this.imageList1 = new System.Windows.Forms.ImageList(this.components);
      this.CodeEditor = new MySql.Data.VisualStudio.Editors.VSCodeEditorUserControl();
      this.panel1 = new System.Windows.Forms.Panel();
      this.xShellConsoleEditor1 = new MySql.Data.VisualStudio.Editors.XShellConsoleEditor();
      this.toolStrip1.SuspendLayout();
      this.ResultsTabControl.SuspendLayout();
      this.resultsPage.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.resultsGrid)).BeginInit();
      this.messagesPage.SuspendLayout();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // toolStrip1
      // 
      this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
      this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ConnectToolStripButton,
            this.DisconnectToolStripButton,
            this.toolStripSeparator1,
            this.RunScriptToolStripButton,
            this.toolStripSeparator2,
            this.ConnectionInfoToolStripDropDownButton,
            this.toolStripSeparator3,
            this.ExecutionModeToolStripDropDownButton,
            this.toolStripSeparator4,
            this.SessionOptionsToolStripDropDownButton});
      this.toolStrip1.Location = new System.Drawing.Point(0, 0);
      this.toolStrip1.Name = "toolStrip1";
      this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
      this.toolStrip1.Size = new System.Drawing.Size(604, 25);
      this.toolStrip1.TabIndex = 1;
      this.toolStrip1.Text = "toolStrip1";
      // 
      // ConnectToolStripButton
      // 
      this.ConnectToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.ConnectToolStripButton.Image = global::MySql.Data.VisualStudio.Properties.Resources.sql_editor_connect;
      this.ConnectToolStripButton.ImageTransparentColor = System.Drawing.Color.Transparent;
      this.ConnectToolStripButton.Name = "ConnectToolStripButton";
      this.ConnectToolStripButton.Size = new System.Drawing.Size(23, 22);
      this.ConnectToolStripButton.Text = "connectButton";
      this.ConnectToolStripButton.ToolTipText = "Connect to...";
      this.ConnectToolStripButton.Click += new System.EventHandler(this.connectButton_Click);
      // 
      // DisconnectToolStripButton
      // 
      this.DisconnectToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.DisconnectToolStripButton.Enabled = false;
      this.DisconnectToolStripButton.Image = global::MySql.Data.VisualStudio.Properties.Resources.sql_editor_disconnect;
      this.DisconnectToolStripButton.ImageTransparentColor = System.Drawing.Color.Transparent;
      this.DisconnectToolStripButton.Name = "DisconnectToolStripButton";
      this.DisconnectToolStripButton.Size = new System.Drawing.Size(23, 22);
      this.DisconnectToolStripButton.Text = "Disconnect";
      this.DisconnectToolStripButton.Click += new System.EventHandler(this.disconnectButton_Click);
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
      this.RunScriptToolStripButton.Image = global::MySql.Data.VisualStudio.Properties.Resources.sql_editor_runsql;
      this.RunScriptToolStripButton.ImageTransparentColor = System.Drawing.Color.Transparent;
      this.RunScriptToolStripButton.Name = "RunScriptToolStripButton";
      this.RunScriptToolStripButton.Size = new System.Drawing.Size(23, 22);
      this.RunScriptToolStripButton.Text = "runScriptButton";
      this.RunScriptToolStripButton.ToolTipText = "Run Js";
      this.RunScriptToolStripButton.Click += new System.EventHandler(this.runScriptButton_Click);
      // 
      // toolStripSeparator2
      // 
      this.toolStripSeparator2.Name = "toolStripSeparator2";
      this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
      // 
      // ConnectionInfoToolStripDropDownButton
      // 
      this.ConnectionInfoToolStripDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.ConnectionInfoToolStripDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ConnectionMethodToolStripMenuItem,
            this.HostIdToolStripMenuItem,
            this.ServerVersionToolStripMenuItem,
            this.UserToolStripMenuItem,
            this.SchemaToolStripMenuItem});
      this.ConnectionInfoToolStripDropDownButton.Image = ((System.Drawing.Image)(resources.GetObject("ConnectionInfoToolStripDropDownButton.Image")));
      this.ConnectionInfoToolStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.ConnectionInfoToolStripDropDownButton.Name = "ConnectionInfoToolStripDropDownButton";
      this.ConnectionInfoToolStripDropDownButton.Size = new System.Drawing.Size(91, 22);
      this.ConnectionInfoToolStripDropDownButton.Text = "Connection...";
      // 
      // ConnectionMethodToolStripMenuItem
      // 
      this.ConnectionMethodToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.ConnectionMethodToolStripMenuItem.Name = "ConnectionMethodToolStripMenuItem";
      this.ConnectionMethodToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
      this.ConnectionMethodToolStripMenuItem.Text = "Connection Method: <none>";
      // 
      // HostIdToolStripMenuItem
      // 
      this.HostIdToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.HostIdToolStripMenuItem.Name = "HostIdToolStripMenuItem";
      this.HostIdToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
      this.HostIdToolStripMenuItem.Text = "Host ID: <none>";
      // 
      // ServerVersionToolStripMenuItem
      // 
      this.ServerVersionToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.ServerVersionToolStripMenuItem.Name = "ServerVersionToolStripMenuItem";
      this.ServerVersionToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
      this.ServerVersionToolStripMenuItem.Text = "Server Version: <none>";
      // 
      // UserToolStripMenuItem
      // 
      this.UserToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.UserToolStripMenuItem.Name = "UserToolStripMenuItem";
      this.UserToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
      this.UserToolStripMenuItem.Text = "User: <none>";
      // 
      // SchemaToolStripMenuItem
      // 
      this.SchemaToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.SchemaToolStripMenuItem.Name = "SchemaToolStripMenuItem";
      this.SchemaToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
      this.SchemaToolStripMenuItem.Text = "Schema: <none>";
      // 
      // toolStripSeparator3
      // 
      this.toolStripSeparator3.Name = "toolStripSeparator3";
      this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
      // 
      // ExecutionModeToolStripDropDownButton
      // 
      this.ExecutionModeToolStripDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.BatchToolStripMenuItem,
            this.ConsoleToolStripMenuItem});
      this.ExecutionModeToolStripDropDownButton.Name = "ExecutionModeToolStripDropDownButton";
      this.ExecutionModeToolStripDropDownButton.Size = new System.Drawing.Size(84, 22);
      this.ExecutionModeToolStripDropDownButton.Text = "Batch Mode";
      // 
      // BatchToolStripMenuItem
      // 
      this.BatchToolStripMenuItem.AutoToolTip = true;
      this.BatchToolStripMenuItem.Checked = true;
      this.BatchToolStripMenuItem.CheckOnClick = true;
      this.BatchToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
      this.BatchToolStripMenuItem.Name = "BatchToolStripMenuItem";
      this.BatchToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
      this.BatchToolStripMenuItem.Tag = MySql.Data.VisualStudio.Editors.SessionOption.UseSameSession;
      this.BatchToolStripMenuItem.Text = "Batch Mode";
      this.BatchToolStripMenuItem.ToolTipText = "Use this option to always run scripts in batch mode.";
      this.BatchToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItemExecutionMode_ClickHandler);
      // 
      // ConsoleToolStripMenuItem
      // 
      this.ConsoleToolStripMenuItem.AutoToolTip = true;
      this.ConsoleToolStripMenuItem.CheckOnClick = true;
      this.ConsoleToolStripMenuItem.Name = "ConsoleToolStripMenuItem";
      this.ConsoleToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
      this.ConsoleToolStripMenuItem.Tag = MySql.Data.VisualStudio.Editors.SessionOption.UseNewSession;
      this.ConsoleToolStripMenuItem.Text = "Console Mode";
      this.ConsoleToolStripMenuItem.ToolTipText = "Use this option to always run scripts in console session.";
      this.ConsoleToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItemExecutionMode_ClickHandler);
      // 
      // toolStripSeparator4
      // 
      this.toolStripSeparator4.Name = "toolStripSeparator4";
      this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
      // 
      // SessionOptionsToolStripDropDownButton
      // 
      this.SessionOptionsToolStripDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.SessionOptionsToolStripDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.PreserveVariablesToolStripMenuItem});
      this.SessionOptionsToolStripDropDownButton.Name = "SessionOptionsToolStripDropDownButton";
      this.SessionOptionsToolStripDropDownButton.Size = new System.Drawing.Size(104, 22);
      this.SessionOptionsToolStripDropDownButton.Text = "Session Options";
      // 
      // PreserveVariablesToolStripMenuItem
      // 
      this.PreserveVariablesToolStripMenuItem.AutoToolTip = true;
      this.PreserveVariablesToolStripMenuItem.Checked = true;
      this.PreserveVariablesToolStripMenuItem.CheckOnClick = true;
      this.PreserveVariablesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
      this.PreserveVariablesToolStripMenuItem.Name = "PreserveVariablesToolStripMenuItem";
      this.PreserveVariablesToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
      this.PreserveVariablesToolStripMenuItem.Tag = "";
      this.PreserveVariablesToolStripMenuItem.Text = "Preserve Variables";
      this.PreserveVariablesToolStripMenuItem.ToolTipText = "Use this option to always run scripts in the same session.";
      this.PreserveVariablesToolStripMenuItem.Click += new System.EventHandler(this.PreserveVariablesToolStripMenuItem_Click);
      // 
      // splitter1
      // 
      this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
      this.splitter1.Location = new System.Drawing.Point(0, 291);
      this.splitter1.Name = "splitter1";
      this.splitter1.Size = new System.Drawing.Size(604, 3);
      this.splitter1.TabIndex = 3;
      this.splitter1.TabStop = false;
      // 
      // ResultsTabControl
      // 
      this.ResultsTabControl.Controls.Add(this.resultsPage);
      this.ResultsTabControl.Controls.Add(this.messagesPage);
      this.ResultsTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ResultsTabControl.ImageList = this.imageList1;
      this.ResultsTabControl.Location = new System.Drawing.Point(0, 294);
      this.ResultsTabControl.Name = "ResultsTabControl";
      this.ResultsTabControl.SelectedIndex = 0;
      this.ResultsTabControl.Size = new System.Drawing.Size(604, 147);
      this.ResultsTabControl.TabIndex = 4;
      // 
      // resultsPage
      // 
      this.resultsPage.Controls.Add(this.resultsGrid);
      this.resultsPage.ImageIndex = 1;
      this.resultsPage.Location = new System.Drawing.Point(4, 23);
      this.resultsPage.Name = "resultsPage";
      this.resultsPage.Padding = new System.Windows.Forms.Padding(3);
      this.resultsPage.Size = new System.Drawing.Size(596, 120);
      this.resultsPage.TabIndex = 0;
      this.resultsPage.Text = "Results";
      this.resultsPage.UseVisualStyleBackColor = true;
      // 
      // resultsGrid
      // 
      this.resultsGrid.AllowUserToAddRows = false;
      this.resultsGrid.AllowUserToDeleteRows = false;
      this.resultsGrid.BackgroundColor = System.Drawing.SystemColors.Window;
      this.resultsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.resultsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
      this.resultsGrid.Location = new System.Drawing.Point(3, 3);
      this.resultsGrid.Name = "resultsGrid";
      this.resultsGrid.Size = new System.Drawing.Size(590, 114);
      this.resultsGrid.TabIndex = 0;
      // 
      // messagesPage
      // 
      this.messagesPage.Controls.Add(this.messages);
      this.messagesPage.ImageIndex = 0;
      this.messagesPage.Location = new System.Drawing.Point(4, 23);
      this.messagesPage.Name = "messagesPage";
      this.messagesPage.Padding = new System.Windows.Forms.Padding(3);
      this.messagesPage.Size = new System.Drawing.Size(596, 120);
      this.messagesPage.TabIndex = 1;
      this.messagesPage.Text = "Messages";
      this.messagesPage.UseVisualStyleBackColor = true;
      // 
      // messages
      // 
      this.messages.Dock = System.Windows.Forms.DockStyle.Fill;
      this.messages.Location = new System.Drawing.Point(3, 3);
      this.messages.Name = "messages";
      this.messages.Size = new System.Drawing.Size(590, 114);
      this.messages.TabIndex = 0;
      // 
      // imageList1
      // 
      this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
      this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // CodeEditor
      // 
      this.CodeEditor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.CodeEditor.Dock = System.Windows.Forms.DockStyle.Top;
      this.CodeEditor.IsDirty = false;
      this.CodeEditor.Location = new System.Drawing.Point(0, 0);
      this.CodeEditor.Name = "CodeEditor";
      this.CodeEditor.Size = new System.Drawing.Size(604, 291);
      this.CodeEditor.TabIndex = 2;
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.ResultsTabControl);
      this.panel1.Controls.Add(this.splitter1);
      this.panel1.Controls.Add(this.CodeEditor);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.panel1.Location = new System.Drawing.Point(0, 25);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(604, 441);
      this.panel1.TabIndex = 5;
      // 
      // xShellConsoleEditor1
      // 
      this.xShellConsoleEditor1.Dock = System.Windows.Forms.DockStyle.Top;
      this.xShellConsoleEditor1.Font = new System.Drawing.Font("Courier New", 8F);
      this.xShellConsoleEditor1.IsDirty = false;
      this.xShellConsoleEditor1.Location = new System.Drawing.Point(0, 0);
      this.xShellConsoleEditor1.MinimumSize = new System.Drawing.Size(0, 17);
      this.xShellConsoleEditor1.Name = "xShellConsoleEditor1";
      this.xShellConsoleEditor1.PromptColor = System.Drawing.SystemColors.ControlText;
      this.xShellConsoleEditor1.PromptString = ">";
      this.xShellConsoleEditor1.Size = new System.Drawing.Size(604, 437);
      this.xShellConsoleEditor1.TabIndex = 0;
      this.xShellConsoleEditor1.Command += new MySql.Data.VisualStudio.Editors.XShellConsoleEditor.CommandEventHandler(this.xShellConsoleEditor1_Command);
      // 
      // MySqlHybridScriptEditor
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.panel1);
      this.Controls.Add(this.toolStrip1);
      this.Name = "MySqlHybridScriptEditor";
      this.Size = new System.Drawing.Size(604, 466);
      this.toolStrip1.ResumeLayout(false);
      this.toolStrip1.PerformLayout();
      this.ResultsTabControl.ResumeLayout(false);
      this.resultsPage.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.resultsGrid)).EndInit();
      this.messagesPage.ResumeLayout(false);
      this.panel1.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ToolStrip toolStrip1;
    private VSCodeEditorUserControl CodeEditor;
    private System.Windows.Forms.Splitter splitter1;
    private System.Windows.Forms.TabControl ResultsTabControl;
    private System.Windows.Forms.TabPage resultsPage;
    private System.Windows.Forms.TabPage messagesPage;
    private System.Windows.Forms.ToolStripButton ConnectToolStripButton;
    private System.Windows.Forms.ToolStripButton RunScriptToolStripButton;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    private System.Windows.Forms.ToolStripButton DisconnectToolStripButton;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
    private System.Windows.Forms.DataGridView resultsGrid;
    private System.Windows.Forms.Label messages;
    private System.Windows.Forms.ImageList imageList1;
    private Panel panel1;
    private XShellConsoleEditor xShellConsoleEditor1;
    private ToolStripSeparator toolStripSeparator4;
    private ToolStripDropDownButton ConnectionInfoToolStripDropDownButton;
    private ToolStripMenuItem ConnectionMethodToolStripMenuItem;
    private ToolStripMenuItem HostIdToolStripMenuItem;
    private ToolStripMenuItem ServerVersionToolStripMenuItem;
    private ToolStripMenuItem UserToolStripMenuItem;
    private ToolStripMenuItem SchemaToolStripMenuItem;
    private ToolStripDropDownButton ExecutionModeToolStripDropDownButton;
    private ToolStripMenuItem BatchToolStripMenuItem;
    private ToolStripMenuItem ConsoleToolStripMenuItem;
    private ToolStripDropDownButton SessionOptionsToolStripDropDownButton;
    private ToolStripMenuItem PreserveVariablesToolStripMenuItem;
  }
}
