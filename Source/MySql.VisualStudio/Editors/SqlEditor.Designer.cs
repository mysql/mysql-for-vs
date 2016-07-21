using System.Data;
using System.Windows.Forms;

namespace MySql.Data.VisualStudio.Editors
{
  partial class SqlEditor
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

        if (Connection != null)
        {
          if (Connection.State != ConnectionState.Closed)
          {
            Connection.Close();
          }

          Connection.Dispose();
        }
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
      this.toolStrip1 = new System.Windows.Forms.ToolStrip();
      this.ConnectToolStripButton = new System.Windows.Forms.ToolStripButton();
      this.DisconnectToolStripButton = new System.Windows.Forms.ToolStripButton();
      this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.RunSqlToolStripButton = new System.Windows.Forms.ToolStripButton();
      this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
      this.ConnectionInfoToolStripDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
      this.ConnectionMethodToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.HostIdToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.ServerVersionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.UserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.SchemaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.splitter1 = new System.Windows.Forms.Splitter();
      this.ResultsTabControl = new System.Windows.Forms.TabControl();
      this.resultsPage = new System.Windows.Forms.TabPage();
      this.ResultsDataGridView = new System.Windows.Forms.DataGridView();
      this.messagesPage = new System.Windows.Forms.TabPage();
      this.messages = new System.Windows.Forms.Label();
      this.imageList1 = new System.Windows.Forms.ImageList(this.components);
      this.CodeEditor = new MySql.Data.VisualStudio.Editors.VSCodeEditorUserControl();
      this.toolStrip1.SuspendLayout();
      this.ResultsTabControl.SuspendLayout();
      this.resultsPage.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ResultsDataGridView)).BeginInit();
      this.messagesPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // toolStrip1
      // 
      this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
      this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ConnectToolStripButton,
            this.DisconnectToolStripButton,
            this.toolStripSeparator1,
            this.RunSqlToolStripButton,
            this.toolStripSeparator2,
            this.ConnectionInfoToolStripDropDownButton});
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
      this.ConnectToolStripButton.ToolTipText = "Connect to MySQL...";
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
      this.DisconnectToolStripButton.Text = "Disconnect from MySQL";
      this.DisconnectToolStripButton.Click += new System.EventHandler(this.disconnectButton_Click);
      // 
      // toolStripSeparator1
      // 
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
      // 
      // RunSqlToolStripButton
      // 
      this.RunSqlToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.RunSqlToolStripButton.Enabled = false;
      this.RunSqlToolStripButton.Image = global::MySql.Data.VisualStudio.Properties.Resources.sql_editor_runsql;
      this.RunSqlToolStripButton.ImageTransparentColor = System.Drawing.Color.Transparent;
      this.RunSqlToolStripButton.Name = "RunSqlToolStripButton";
      this.RunSqlToolStripButton.Size = new System.Drawing.Size(23, 22);
      this.RunSqlToolStripButton.Text = "runSqlButton";
      this.RunSqlToolStripButton.ToolTipText = "Run script";
      this.RunSqlToolStripButton.Click += new System.EventHandler(this.runSqlButton_Click);
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
      this.ResultsTabControl.Controls.Add(this.resultsPage);
      this.ResultsTabControl.Controls.Add(this.messagesPage);
      this.ResultsTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ResultsTabControl.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
      this.ResultsTabControl.ImageList = this.imageList1;
      this.ResultsTabControl.Location = new System.Drawing.Point(0, 274);
      this.ResultsTabControl.Name = "ResultsTabControl";
      this.ResultsTabControl.SelectedIndex = 0;
      this.ResultsTabControl.Size = new System.Drawing.Size(604, 192);
      this.ResultsTabControl.TabIndex = 4;
      // 
      // resultsPage
      // 
      this.resultsPage.Controls.Add(this.ResultsDataGridView);
      this.resultsPage.ImageIndex = 1;
      this.resultsPage.Location = new System.Drawing.Point(4, 23);
      this.resultsPage.Name = "resultsPage";
      this.resultsPage.Padding = new System.Windows.Forms.Padding(3);
      this.resultsPage.Size = new System.Drawing.Size(596, 165);
      this.resultsPage.TabIndex = 0;
      this.resultsPage.Text = "Results";
      this.resultsPage.UseVisualStyleBackColor = true;
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
      // imageList1
      // 
      this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
      this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
      this.imageList1.Images.SetKeyName(0, "messages_icon.png");
      this.imageList1.Images.SetKeyName(1, "results_icon.png");
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
      this.Controls.Add(this.toolStrip1);
      this.Name = "SqlEditor";
      this.Size = new System.Drawing.Size(604, 466);
      this.toolStrip1.ResumeLayout(false);
      this.toolStrip1.PerformLayout();
      this.ResultsTabControl.ResumeLayout(false);
      this.resultsPage.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.ResultsDataGridView)).EndInit();
      this.messagesPage.ResumeLayout(false);
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
    private System.Windows.Forms.ToolStripButton RunSqlToolStripButton;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    private System.Windows.Forms.ToolStripButton DisconnectToolStripButton;
    private System.Windows.Forms.DataGridView ResultsDataGridView;
    private System.Windows.Forms.Label messages;
    private System.Windows.Forms.ImageList imageList1;
    private ToolStripDropDownButton ConnectionInfoToolStripDropDownButton;
    private ToolStripMenuItem ConnectionMethodToolStripMenuItem;
    private ToolStripMenuItem HostIdToolStripMenuItem;
    private ToolStripMenuItem ServerVersionToolStripMenuItem;
    private ToolStripMenuItem UserToolStripMenuItem;
    private ToolStripMenuItem SchemaToolStripMenuItem;
  }
}
