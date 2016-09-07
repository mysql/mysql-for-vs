namespace MySql.Data.VisualStudio.Editors
{
  partial class ConnectDialog
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
      this.MySqlLogoPictureBox = new System.Windows.Forms.PictureBox();
      this.AdvancedButton = new System.Windows.Forms.Button();
      this.ConnectButton = new System.Windows.Forms.Button();
      this.CancelDlgButton = new System.Windows.Forms.Button();
      this.AdvancedPropertyGrid = new System.Windows.Forms.PropertyGrid();
      this.HelpToolTip = new System.Windows.Forms.ToolTip(this.components);
      this.ConnectionNameTextBox = new System.Windows.Forms.TextBox();
      this.AddToServerExplorerCheckBox = new System.Windows.Forms.CheckBox();
      this.PortTextBox = new System.Windows.Forms.TextBox();
      this.RefreshButton = new System.Windows.Forms.Button();
      this.UsernameTextBox = new System.Windows.Forms.TextBox();
      this.PasswordTextBox = new System.Windows.Forms.TextBox();
      this.HostnameTextBox = new System.Windows.Forms.TextBox();
      this.SimpleGroupBox = new System.Windows.Forms.GroupBox();
      this.SchemaComboBox = new System.Windows.Forms.ComboBox();
      this.ConnectionNameLabel = new System.Windows.Forms.Label();
      this.PortLabel = new System.Windows.Forms.Label();
      this.PasswordLabel = new System.Windows.Forms.Label();
      this.UsernameLabel = new System.Windows.Forms.Label();
      this.SchemaLabel = new System.Windows.Forms.Label();
      this.HostnameLabel = new System.Windows.Forms.Label();
      this.ContentAreaPanel.SuspendLayout();
      this.CommandAreaPanel.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.MySqlLogoPictureBox)).BeginInit();
      this.SimpleGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // FootnoteAreaPanel
      // 
      this.FootnoteAreaPanel.Location = new System.Drawing.Point(0, 292);
      this.FootnoteAreaPanel.Size = new System.Drawing.Size(634, 0);
      // 
      // ContentAreaPanel
      // 
      this.ContentAreaPanel.Controls.Add(this.SimpleGroupBox);
      this.ContentAreaPanel.Controls.Add(this.MySqlLogoPictureBox);
      this.ContentAreaPanel.Controls.Add(this.AdvancedPropertyGrid);
      this.ContentAreaPanel.Size = new System.Drawing.Size(484, 361);
      // 
      // CommandAreaPanel
      // 
      this.CommandAreaPanel.Controls.Add(this.ConnectButton);
      this.CommandAreaPanel.Controls.Add(this.CancelDlgButton);
      this.CommandAreaPanel.Controls.Add(this.AdvancedButton);
      this.CommandAreaPanel.Location = new System.Drawing.Point(0, 316);
      this.CommandAreaPanel.Size = new System.Drawing.Size(484, 45);
      this.CommandAreaPanel.TabIndex = 2;
      // 
      // MySqlLogoPictureBox
      // 
      this.MySqlLogoPictureBox.Dock = System.Windows.Forms.DockStyle.Top;
      this.MySqlLogoPictureBox.Image = global::MySql.Data.VisualStudio.Properties.Resources.sql_editor_banner;
      this.MySqlLogoPictureBox.Location = new System.Drawing.Point(0, 0);
      this.MySqlLogoPictureBox.Name = "MySqlLogoPictureBox";
      this.MySqlLogoPictureBox.Size = new System.Drawing.Size(484, 89);
      this.MySqlLogoPictureBox.TabIndex = 6;
      this.MySqlLogoPictureBox.TabStop = false;
      // 
      // AdvancedButton
      // 
      this.AdvancedButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.AdvancedButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.AdvancedButton.Location = new System.Drawing.Point(12, 11);
      this.AdvancedButton.Name = "AdvancedButton";
      this.AdvancedButton.Size = new System.Drawing.Size(108, 25);
      this.AdvancedButton.TabIndex = 0;
      this.AdvancedButton.Text = "Advanced >>";
      this.AdvancedButton.UseVisualStyleBackColor = true;
      this.AdvancedButton.Click += new System.EventHandler(this.AdvancedButton_Click);
      // 
      // ConnectButton
      // 
      this.ConnectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.ConnectButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.ConnectButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ConnectButton.Location = new System.Drawing.Point(286, 11);
      this.ConnectButton.Name = "ConnectButton";
      this.ConnectButton.Size = new System.Drawing.Size(90, 25);
      this.ConnectButton.TabIndex = 1;
      this.ConnectButton.Text = "Connect";
      this.ConnectButton.UseVisualStyleBackColor = true;
      // 
      // CancelDlgButton
      // 
      this.CancelDlgButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.CancelDlgButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.CancelDlgButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.CancelDlgButton.Location = new System.Drawing.Point(382, 11);
      this.CancelDlgButton.Name = "CancelDlgButton";
      this.CancelDlgButton.Size = new System.Drawing.Size(90, 25);
      this.CancelDlgButton.TabIndex = 2;
      this.CancelDlgButton.Text = "Cancel";
      this.CancelDlgButton.UseVisualStyleBackColor = true;
      // 
      // AdvancedPropertyGrid
      // 
      this.AdvancedPropertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.AdvancedPropertyGrid.LineColor = System.Drawing.SystemColors.Control;
      this.AdvancedPropertyGrid.Location = new System.Drawing.Point(12, 95);
      this.AdvancedPropertyGrid.Name = "AdvancedPropertyGrid";
      this.AdvancedPropertyGrid.Size = new System.Drawing.Size(460, 202);
      this.AdvancedPropertyGrid.TabIndex = 1;
      this.AdvancedPropertyGrid.Visible = false;
      // 
      // ConnectionNameTextBox
      // 
      this.ConnectionNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.ConnectionNameTextBox.Enabled = false;
      this.ConnectionNameTextBox.Location = new System.Drawing.Point(98, 160);
      this.ConnectionNameTextBox.Name = "ConnectionNameTextBox";
      this.ConnectionNameTextBox.Size = new System.Drawing.Size(335, 23);
      this.ConnectionNameTextBox.TabIndex = 13;
      this.HelpToolTip.SetToolTip(this.ConnectionNameTextBox, "The name of the Server Explorer connection.");
      // 
      // AddToServerExplorerCheckBox
      // 
      this.AddToServerExplorerCheckBox.AutoSize = true;
      this.AddToServerExplorerCheckBox.Location = new System.Drawing.Point(98, 135);
      this.AddToServerExplorerCheckBox.Name = "AddToServerExplorerCheckBox";
      this.AddToServerExplorerCheckBox.Size = new System.Drawing.Size(205, 19);
      this.AddToServerExplorerCheckBox.TabIndex = 11;
      this.AddToServerExplorerCheckBox.Text = "Add connection to Server Explorer";
      this.HelpToolTip.SetToolTip(this.AddToServerExplorerCheckBox, "Adds the connection to the Server Explorer so it can be used in the future.");
      this.AddToServerExplorerCheckBox.UseVisualStyleBackColor = true;
      this.AddToServerExplorerCheckBox.CheckedChanged += new System.EventHandler(this.AddToServerExplorerCheckBox_CheckedChanged);
      // 
      // PortTextBox
      // 
      this.PortTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.PortTextBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.PortTextBox.Location = new System.Drawing.Point(386, 19);
      this.PortTextBox.Name = "PortTextBox";
      this.PortTextBox.Size = new System.Drawing.Size(47, 23);
      this.PortTextBox.TabIndex = 3;
      this.PortTextBox.Text = "3306";
      this.HelpToolTip.SetToolTip(this.PortTextBox, "TCP/IP port.");
      this.PortTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.PortTextBox_Validating);
      this.PortTextBox.Validated += new System.EventHandler(this.SimpleControlValidated);
      // 
      // RefreshButton
      // 
      this.RefreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.RefreshButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.RefreshButton.Location = new System.Drawing.Point(348, 104);
      this.RefreshButton.Name = "RefreshButton";
      this.RefreshButton.Size = new System.Drawing.Size(85, 25);
      this.RefreshButton.TabIndex = 10;
      this.RefreshButton.Text = "Refresh";
      this.HelpToolTip.SetToolTip(this.RefreshButton, "Refreshes the list of schemas in the combo box.");
      this.RefreshButton.UseVisualStyleBackColor = true;
      this.RefreshButton.Click += new System.EventHandler(this.RefreshButton_Click);
      // 
      // UsernameTextBox
      // 
      this.UsernameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.UsernameTextBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.UsernameTextBox.Location = new System.Drawing.Point(99, 48);
      this.UsernameTextBox.Name = "UsernameTextBox";
      this.UsernameTextBox.Size = new System.Drawing.Size(334, 23);
      this.UsernameTextBox.TabIndex = 5;
      this.HelpToolTip.SetToolTip(this.UsernameTextBox, "Name of the user to connect with.");
      this.UsernameTextBox.Validated += new System.EventHandler(this.SimpleControlValidated);
      // 
      // PasswordTextBox
      // 
      this.PasswordTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.PasswordTextBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.PasswordTextBox.Location = new System.Drawing.Point(98, 77);
      this.PasswordTextBox.Name = "PasswordTextBox";
      this.PasswordTextBox.PasswordChar = '*';
      this.PasswordTextBox.Size = new System.Drawing.Size(335, 23);
      this.PasswordTextBox.TabIndex = 7;
      this.HelpToolTip.SetToolTip(this.PasswordTextBox, "The user\'s password.");
      this.PasswordTextBox.UseSystemPasswordChar = true;
      this.PasswordTextBox.Validated += new System.EventHandler(this.SimpleControlValidated);
      // 
      // HostnameTextBox
      // 
      this.HostnameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.HostnameTextBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.HostnameTextBox.Location = new System.Drawing.Point(98, 19);
      this.HostnameTextBox.Name = "HostnameTextBox";
      this.HostnameTextBox.Size = new System.Drawing.Size(244, 23);
      this.HostnameTextBox.TabIndex = 1;
      this.HostnameTextBox.Text = "localhost";
      this.HelpToolTip.SetToolTip(this.HostnameTextBox, "Name or IP address of the server host .");
      this.HostnameTextBox.Validated += new System.EventHandler(this.SimpleControlValidated);
      // 
      // SimpleGroupBox
      // 
      this.SimpleGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.SimpleGroupBox.Controls.Add(this.SchemaComboBox);
      this.SimpleGroupBox.Controls.Add(this.ConnectionNameLabel);
      this.SimpleGroupBox.Controls.Add(this.ConnectionNameTextBox);
      this.SimpleGroupBox.Controls.Add(this.AddToServerExplorerCheckBox);
      this.SimpleGroupBox.Controls.Add(this.PortTextBox);
      this.SimpleGroupBox.Controls.Add(this.PortLabel);
      this.SimpleGroupBox.Controls.Add(this.RefreshButton);
      this.SimpleGroupBox.Controls.Add(this.UsernameTextBox);
      this.SimpleGroupBox.Controls.Add(this.PasswordLabel);
      this.SimpleGroupBox.Controls.Add(this.UsernameLabel);
      this.SimpleGroupBox.Controls.Add(this.SchemaLabel);
      this.SimpleGroupBox.Controls.Add(this.PasswordTextBox);
      this.SimpleGroupBox.Controls.Add(this.HostnameLabel);
      this.SimpleGroupBox.Controls.Add(this.HostnameTextBox);
      this.SimpleGroupBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.SimpleGroupBox.Location = new System.Drawing.Point(12, 95);
      this.SimpleGroupBox.Name = "SimpleGroupBox";
      this.SimpleGroupBox.Size = new System.Drawing.Size(460, 202);
      this.SimpleGroupBox.TabIndex = 0;
      this.SimpleGroupBox.TabStop = false;
      // 
      // SchemaComboBox
      // 
      this.SchemaComboBox.FormattingEnabled = true;
      this.SchemaComboBox.Location = new System.Drawing.Point(99, 106);
      this.SchemaComboBox.Name = "SchemaComboBox";
      this.SchemaComboBox.Size = new System.Drawing.Size(243, 23);
      this.SchemaComboBox.TabIndex = 9;
      this.SchemaComboBox.DropDown += new System.EventHandler(this.SchemaComboBox_DropDown);
      this.SchemaComboBox.Validated += new System.EventHandler(this.SimpleControlValidated);
      // 
      // ConnectionNameLabel
      // 
      this.ConnectionNameLabel.AutoSize = true;
      this.ConnectionNameLabel.Location = new System.Drawing.Point(50, 163);
      this.ConnectionNameLabel.Name = "ConnectionNameLabel";
      this.ConnectionNameLabel.Size = new System.Drawing.Size(42, 15);
      this.ConnectionNameLabel.TabIndex = 12;
      this.ConnectionNameLabel.Text = "Name:";
      // 
      // PortLabel
      // 
      this.PortLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.PortLabel.AutoSize = true;
      this.PortLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.PortLabel.Location = new System.Drawing.Point(348, 22);
      this.PortLabel.Name = "PortLabel";
      this.PortLabel.Size = new System.Drawing.Size(32, 15);
      this.PortLabel.TabIndex = 2;
      this.PortLabel.Text = "Port:";
      // 
      // PasswordLabel
      // 
      this.PasswordLabel.AutoSize = true;
      this.PasswordLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.PasswordLabel.Location = new System.Drawing.Point(32, 79);
      this.PasswordLabel.Name = "PasswordLabel";
      this.PasswordLabel.Size = new System.Drawing.Size(60, 15);
      this.PasswordLabel.TabIndex = 6;
      this.PasswordLabel.Text = "Password:";
      // 
      // UsernameLabel
      // 
      this.UsernameLabel.AutoSize = true;
      this.UsernameLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.UsernameLabel.Location = new System.Drawing.Point(30, 51);
      this.UsernameLabel.Name = "UsernameLabel";
      this.UsernameLabel.Size = new System.Drawing.Size(63, 15);
      this.UsernameLabel.TabIndex = 4;
      this.UsernameLabel.Text = "Username:";
      // 
      // SchemaLabel
      // 
      this.SchemaLabel.AutoSize = true;
      this.SchemaLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.SchemaLabel.Location = new System.Drawing.Point(40, 109);
      this.SchemaLabel.Name = "SchemaLabel";
      this.SchemaLabel.Size = new System.Drawing.Size(52, 15);
      this.SchemaLabel.TabIndex = 8;
      this.SchemaLabel.Text = "Schema:";
      // 
      // HostnameLabel
      // 
      this.HostnameLabel.AutoSize = true;
      this.HostnameLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.HostnameLabel.Location = new System.Drawing.Point(27, 22);
      this.HostnameLabel.Name = "HostnameLabel";
      this.HostnameLabel.Size = new System.Drawing.Size(65, 15);
      this.HostnameLabel.TabIndex = 0;
      this.HostnameLabel.Text = "Hostname:";
      // 
      // ConnectDialog
      // 
      this.AcceptButton = this.ConnectButton;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.CancelDlgButton;
      this.ClientSize = new System.Drawing.Size(484, 361);
      this.CommandAreaVisible = true;
      this.FootnoteAreaHeight = 0;
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
      this.MinimumSize = new System.Drawing.Size(470, 400);
      this.Name = "ConnectDialog";
      this.Text = "Connect to MySQL";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConnectDialogNew_FormClosing);
      this.ContentAreaPanel.ResumeLayout(false);
      this.CommandAreaPanel.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.MySqlLogoPictureBox)).EndInit();
      this.SimpleGroupBox.ResumeLayout(false);
      this.SimpleGroupBox.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.PictureBox MySqlLogoPictureBox;
    private System.Windows.Forms.Button AdvancedButton;
    private System.Windows.Forms.Button ConnectButton;
    private System.Windows.Forms.Button CancelDlgButton;
    private System.Windows.Forms.PropertyGrid AdvancedPropertyGrid;
    private System.Windows.Forms.ToolTip HelpToolTip;
    private System.Windows.Forms.GroupBox SimpleGroupBox;
    private System.Windows.Forms.Label ConnectionNameLabel;
    private System.Windows.Forms.TextBox ConnectionNameTextBox;
    private System.Windows.Forms.CheckBox AddToServerExplorerCheckBox;
    private System.Windows.Forms.TextBox PortTextBox;
    private System.Windows.Forms.Label PortLabel;
    private System.Windows.Forms.Button RefreshButton;
    private System.Windows.Forms.TextBox UsernameTextBox;
    private System.Windows.Forms.Label PasswordLabel;
    private System.Windows.Forms.Label UsernameLabel;
    private System.Windows.Forms.Label SchemaLabel;
    private System.Windows.Forms.TextBox PasswordTextBox;
    private System.Windows.Forms.Label HostnameLabel;
    private System.Windows.Forms.TextBox HostnameTextBox;
    private System.Windows.Forms.ComboBox SchemaComboBox;
  }
}