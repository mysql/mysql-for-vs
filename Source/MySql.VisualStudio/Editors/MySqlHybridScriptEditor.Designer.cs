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
      this.toolStrip1 = new System.Windows.Forms.ToolStrip();
      this.connectButton = new System.Windows.Forms.ToolStripButton();
      this.disconnectButton = new System.Windows.Forms.ToolStripButton();
      this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.runScriptButton = new System.Windows.Forms.ToolStripButton();
      this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
      this.serverLabel = new System.Windows.Forms.ToolStripLabel();
      this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
      this.userLabel = new System.Windows.Forms.ToolStripLabel();
      this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
      this.dbLabel = new System.Windows.Forms.ToolStripLabel();
      this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
      this.toolStripSplitButton = new System.Windows.Forms.ToolStripSplitButton();
      this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
      this.tssbExecutionModeButton = new System.Windows.Forms.ToolStripSplitButton();
      this.tsmiBatchExecutionModeButton = new System.Windows.Forms.ToolStripMenuItem();
      this.tsmiConsoleExecutionModeButton = new System.Windows.Forms.ToolStripMenuItem();
      this.splitter1 = new System.Windows.Forms.Splitter();
      this.tabControl1 = new System.Windows.Forms.TabControl();
      this.resultsPage = new System.Windows.Forms.TabPage();
      this.resultsGrid = new System.Windows.Forms.DataGridView();
      this.messagesPage = new System.Windows.Forms.TabPage();
      this.messages = new System.Windows.Forms.Label();
      this.imageList1 = new System.Windows.Forms.ImageList(this.components);
      this.codeEditor = new MySql.Data.VisualStudio.Editors.VSCodeEditorUserControl();
      this.panel1 = new System.Windows.Forms.Panel();
      this.xShellConsoleEditor1 = new MySql.Data.VisualStudio.Editors.XShellConsoleEditor();
      this.toolStrip1.SuspendLayout();
      this.tabControl1.SuspendLayout();
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
            this.connectButton,
            this.disconnectButton,
            this.toolStripSeparator1,
            this.runScriptButton,
            this.toolStripSeparator2,
            this.serverLabel,
            this.toolStripSeparator3,
            this.userLabel,
            this.toolStripSeparator4,
            this.dbLabel,
            this.toolStripSeparator5,
            this.toolStripSplitButton,
            this.toolStripSeparator6,
            this.tssbExecutionModeButton});
      this.toolStrip1.Location = new System.Drawing.Point(0, 0);
      this.toolStrip1.Name = "toolStrip1";
      this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
      this.toolStrip1.Size = new System.Drawing.Size(604, 25);
      this.toolStrip1.TabIndex = 1;
      this.toolStrip1.Text = "toolStrip1";
      // 
      // connectButton
      // 
      this.connectButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.connectButton.Image = global::MySql.Data.VisualStudio.Properties.Resources.sql_editor_connect;
      this.connectButton.ImageTransparentColor = System.Drawing.Color.Transparent;
      this.connectButton.Name = "connectButton";
      this.connectButton.Size = new System.Drawing.Size(23, 22);
      this.connectButton.Text = "connectButton";
      this.connectButton.ToolTipText = "Connect to...";
      this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
      // 
      // disconnectButton
      // 
      this.disconnectButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.disconnectButton.Enabled = false;
      this.disconnectButton.Image = global::MySql.Data.VisualStudio.Properties.Resources.sql_editor_disconnect;
      this.disconnectButton.ImageTransparentColor = System.Drawing.Color.Transparent;
      this.disconnectButton.Name = "disconnectButton";
      this.disconnectButton.Size = new System.Drawing.Size(23, 22);
      this.disconnectButton.Text = "Disconnect";
      this.disconnectButton.Click += new System.EventHandler(this.disconnectButton_Click);
      // 
      // toolStripSeparator1
      // 
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
      // 
      // runScriptButton
      // 
      this.runScriptButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.runScriptButton.Enabled = false;
      this.runScriptButton.Image = global::MySql.Data.VisualStudio.Properties.Resources.sql_editor_runsql;
      this.runScriptButton.ImageTransparentColor = System.Drawing.Color.Transparent;
      this.runScriptButton.Name = "runScriptButton";
      this.runScriptButton.Size = new System.Drawing.Size(23, 22);
      this.runScriptButton.Text = "runScriptButton";
      this.runScriptButton.ToolTipText = "Run Js";
      this.runScriptButton.Click += new System.EventHandler(this.runScriptButton_Click);
      // 
      // toolStripSeparator2
      // 
      this.toolStripSeparator2.Name = "toolStripSeparator2";
      this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
      // 
      // serverLabel
      // 
      this.serverLabel.Name = "serverLabel";
      this.serverLabel.Size = new System.Drawing.Size(88, 22);
      this.serverLabel.Text = "Server: <none>";
      // 
      // toolStripSeparator3
      // 
      this.toolStripSeparator3.Name = "toolStripSeparator3";
      this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
      // 
      // userLabel
      // 
      this.userLabel.Name = "userLabel";
      this.userLabel.Size = new System.Drawing.Size(79, 22);
      this.userLabel.Text = "User: <none>";
      // 
      // toolStripSeparator4
      // 
      this.toolStripSeparator4.Name = "toolStripSeparator4";
      this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
      // 
      // dbLabel
      // 
      this.dbLabel.Name = "dbLabel";
      this.dbLabel.Size = new System.Drawing.Size(104, 22);
      this.dbLabel.Text = "Database: <none>";
      // 
      // toolStripSeparator5
      // 
      this.toolStripSeparator5.Name = "toolStripSeparator5";
      this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
      // 
      // toolStripSplitButton
      // 
      this.toolStripSplitButton.DefaultItem = this.toolStripMenuItem1;
      this.toolStripSplitButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2});
      this.toolStripSplitButton.Name = "toolStripSplitButton";
      this.toolStripSplitButton.Size = new System.Drawing.Size(102, 22);
      this.toolStripSplitButton.Text = "Session Option";
      // 
      // toolStripMenuItem1
      // 
      this.toolStripMenuItem1.AutoToolTip = true;
      this.toolStripMenuItem1.Checked = true;
      this.toolStripMenuItem1.CheckOnClick = true;
      this.toolStripMenuItem1.CheckState = System.Windows.Forms.CheckState.Checked;
      this.toolStripMenuItem1.Name = "toolStripMenuItem1";
      this.toolStripMenuItem1.Size = new System.Drawing.Size(204, 22);
      this.toolStripMenuItem1.Tag = MySql.Data.VisualStudio.Editors.SessionOption.UseSameSession;
      this.toolStripMenuItem1.Text = "Preserve JS Variables";
      this.toolStripMenuItem1.ToolTipText = "Use this option to always run scripts in the same session.";
      this.toolStripMenuItem1.Click += new System.EventHandler(this.ToolStripMenuItemClickHandler);
      // 
      // toolStripMenuItem2
      // 
      this.toolStripMenuItem2.AutoToolTip = true;
      this.toolStripMenuItem2.CheckOnClick = true;
      this.toolStripMenuItem2.Name = "toolStripMenuItem2";
      this.toolStripMenuItem2.Size = new System.Drawing.Size(204, 22);
      this.toolStripMenuItem2.Tag = MySql.Data.VisualStudio.Editors.SessionOption.UseNewSession;
      this.toolStripMenuItem2.Text = "Not Preserve JS Variables";
      this.toolStripMenuItem2.ToolTipText = "Use this option to always run scripts in a new session.";
      this.toolStripMenuItem2.Click += new System.EventHandler(this.ToolStripMenuItemClickHandler);
      // 
      // toolStripSeparator6
      // 
      this.toolStripSeparator6.Name = "toolStripSeparator6";
      this.toolStripSeparator6.Size = new System.Drawing.Size(6, 25);
      // 
      // tssbExecutionModeButton
      // 
      this.tssbExecutionModeButton.DefaultItem = this.toolStripMenuItem1;
      this.tssbExecutionModeButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiBatchExecutionModeButton,
            this.tsmiConsoleExecutionModeButton});
      this.tssbExecutionModeButton.Name = "tssbExecutionModeButton";
      this.tssbExecutionModeButton.Size = new System.Drawing.Size(54, 22);
      this.tssbExecutionModeButton.Text = "Mode";
      // 
      // tsmiBatchExecutionModeButton
      // 
      this.tsmiBatchExecutionModeButton.AutoToolTip = true;
      this.tsmiBatchExecutionModeButton.Checked = true;
      this.tsmiBatchExecutionModeButton.CheckOnClick = true;
      this.tsmiBatchExecutionModeButton.CheckState = System.Windows.Forms.CheckState.Checked;
      this.tsmiBatchExecutionModeButton.Name = "tsmiBatchExecutionModeButton";
      this.tsmiBatchExecutionModeButton.Size = new System.Drawing.Size(151, 22);
      this.tsmiBatchExecutionModeButton.Tag = MySql.Data.VisualStudio.Editors.SessionOption.UseSameSession;
      this.tsmiBatchExecutionModeButton.Text = "Batch Mode";
      this.tsmiBatchExecutionModeButton.ToolTipText = "Use this option to always run scripts in batch mode.";
      this.tsmiBatchExecutionModeButton.Click += new System.EventHandler(this.ToolStripMenuItemExecutionMode_ClickHandler);
      // 
      // tsmiConsoleExecutionModeButton
      // 
      this.tsmiConsoleExecutionModeButton.AutoToolTip = true;
      this.tsmiConsoleExecutionModeButton.CheckOnClick = true;
      this.tsmiConsoleExecutionModeButton.Name = "tsmiConsoleExecutionModeButton";
      this.tsmiConsoleExecutionModeButton.Size = new System.Drawing.Size(151, 22);
      this.tsmiConsoleExecutionModeButton.Tag = MySql.Data.VisualStudio.Editors.SessionOption.UseNewSession;
      this.tsmiConsoleExecutionModeButton.Text = "Console mode";
      this.tsmiConsoleExecutionModeButton.ToolTipText = "Use this option to always run scripts in console session.";
      this.tsmiConsoleExecutionModeButton.Click += new System.EventHandler(this.ToolStripMenuItemExecutionMode_ClickHandler);
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
      // tabControl1
      // 
      this.tabControl1.Controls.Add(this.resultsPage);
      this.tabControl1.Controls.Add(this.messagesPage);
      this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl1.ImageList = this.imageList1;
      this.tabControl1.Location = new System.Drawing.Point(0, 294);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new System.Drawing.Size(604, 147);
      this.tabControl1.TabIndex = 4;
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
      this.messagesPage.Size = new System.Drawing.Size(596, 140);
      this.messagesPage.TabIndex = 1;
      this.messagesPage.Text = "Messages";
      this.messagesPage.UseVisualStyleBackColor = true;
      // 
      // messages
      // 
      this.messages.Dock = System.Windows.Forms.DockStyle.Fill;
      this.messages.Location = new System.Drawing.Point(3, 3);
      this.messages.Name = "messages";
      this.messages.Size = new System.Drawing.Size(590, 134);
      this.messages.TabIndex = 0;
      // 
      // imageList1
      // 
      this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
      this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // codeEditor
      // 
      this.codeEditor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.codeEditor.Dock = System.Windows.Forms.DockStyle.Top;
      this.codeEditor.IsDirty = false;
      this.codeEditor.Location = new System.Drawing.Point(0, 0);
      this.codeEditor.Name = "codeEditor";
      this.codeEditor.Size = new System.Drawing.Size(604, 291);
      this.codeEditor.TabIndex = 2;
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.tabControl1);
      this.panel1.Controls.Add(this.splitter1);
      this.panel1.Controls.Add(this.codeEditor);
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
      this.tabControl1.ResumeLayout(false);
      this.resultsPage.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.resultsGrid)).EndInit();
      this.messagesPage.ResumeLayout(false);
      this.panel1.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ToolStrip toolStrip1;
    private VSCodeEditorUserControl codeEditor;
    private System.Windows.Forms.Splitter splitter1;
    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.TabPage resultsPage;
    private System.Windows.Forms.TabPage messagesPage;
    private System.Windows.Forms.ToolStripButton connectButton;
    private System.Windows.Forms.ToolStripLabel serverLabel;
    private System.Windows.Forms.ToolStripButton runScriptButton;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    private System.Windows.Forms.ToolStripButton disconnectButton;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
    private System.Windows.Forms.ToolStripLabel userLabel;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
    private System.Windows.Forms.ToolStripLabel dbLabel;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
    private System.Windows.Forms.ToolStripSplitButton toolStripSplitButton;
    private System.Windows.Forms.ToolStripSplitButton tssbExecutionModeButton;
    private System.Windows.Forms.DataGridView resultsGrid;
    private System.Windows.Forms.Label messages;
    private System.Windows.Forms.ImageList imageList1;
    private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
    private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
    private System.Windows.Forms.ToolStripMenuItem tsmiBatchExecutionModeButton;
    private System.Windows.Forms.ToolStripMenuItem tsmiConsoleExecutionModeButton;
    private Panel panel1;
    private XShellConsoleEditor xShellConsoleEditor1;
  }
}
