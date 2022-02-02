// Copyright (c) 2012, 2019, Oracle and/or its affiliates. All rights reserved.
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License as
// published by the Free Software Foundation; version 2 of the
// License.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301  USA

using System.ComponentModel;
using MySql.Utility.Classes.MySqlWorkbench;

namespace MySql.Utility.Forms
{
  partial class MySqlWorkbenchConnectionDialog
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

        if (WorkbenchConnection != null)
        {
          WorkbenchConnection.PropertyChanged -= new PropertyChangedEventHandler(WorkbenchConnection_PropertyChanged);
        }

        if (_hiddenTabPages != null)
        {
          foreach (var tabPage in _hiddenTabPages)
          {
            tabPage.Dispose();
          }
        }
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MySqlWorkbenchConnectionDialog));
      this.TestConnectionButton = new System.Windows.Forms.Button();
      this.CancelationButton = new System.Windows.Forms.Button();
      this.OKButton = new System.Windows.Forms.Button();
      this.ConnectionBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.HelpConnectionNameLabel = new System.Windows.Forms.Label();
      this.ConnectionMethodComboBox = new System.Windows.Forms.ComboBox();
      this.HelpConnectionMethodLabel = new System.Windows.Forms.Label();
      this.ConnectionMethodLabel = new System.Windows.Forms.Label();
      this.ConnectionNameTextBox = new System.Windows.Forms.TextBox();
      this.ConnectionNameLabel = new System.Windows.Forms.Label();
      this.ConnectionStatusLabel = new System.Windows.Forms.Label();
      this.ConnectionStatusPictureBox = new System.Windows.Forms.PictureBox();
      this.ConnectionStatusDescriptionLabel = new System.Windows.Forms.Label();
      this.StatusImageList = new System.Windows.Forms.ImageList(this.components);
      this.AdvancedTabPage = new System.Windows.Forms.TabPage();
      this.TimeoutNumericUpDown = new System.Windows.Forms.NumericUpDown();
      this.TimeoutLabel = new System.Windows.Forms.Label();
      this.HelpTimeoutLabel = new System.Windows.Forms.Label();
      this.HelpUseCompressionLabel = new System.Windows.Forms.Label();
      this.UseCompressionCheckBox = new System.Windows.Forms.CheckBox();
      this.SslTabPage = new System.Windows.Forms.TabPage();
      this.SslPemMoreInfoLinkLabel = new System.Windows.Forms.LinkLabel();
      this.SslCertFileButton = new System.Windows.Forms.Button();
      this.UseSslComboBox = new System.Windows.Forms.ComboBox();
      this.SslCaFileButton = new System.Windows.Forms.Button();
      this.HelpUseSslLabel = new System.Windows.Forms.Label();
      this.SslKeyFileButton = new System.Windows.Forms.Button();
      this.UseSslLabel = new System.Windows.Forms.Label();
      this.SslKeyFileTextBox = new System.Windows.Forms.TextBox();
      this.SslCaFileLabel = new System.Windows.Forms.Label();
      this.HelpSslKeyLabel = new System.Windows.Forms.Label();
      this.SslCaFileTextBox = new System.Windows.Forms.TextBox();
      this.SslKeyLabel = new System.Windows.Forms.Label();
      this.HelpSslCaFileLabel = new System.Windows.Forms.Label();
      this.SslCertFileTextBox = new System.Windows.Forms.TextBox();
      this.SslCertFileLabel = new System.Windows.Forms.Label();
      this.HelpSslCertFileLabel = new System.Windows.Forms.Label();
      this.SshParametersTabPage = new System.Windows.Forms.TabPage();
      this.MySqlDefaultSchemaComboBox = new System.Windows.Forms.ComboBox();
      this.SshPassPhraseTextBox = new System.Windows.Forms.TextBox();
      this.HelpSshPassPhraseLabel = new System.Windows.Forms.Label();
      this.SshPassPhraseLabel = new System.Windows.Forms.Label();
      this.MySqlPasswordTextBox = new System.Windows.Forms.TextBox();
      this.HelpMySqlDefaultSchemaLabel = new System.Windows.Forms.Label();
      this.MySqlPortTextBox = new System.Windows.Forms.TextBox();
      this.MySqlUsernameTextBox = new System.Windows.Forms.TextBox();
      this.MySqlHostNameTextBox = new System.Windows.Forms.TextBox();
      this.SshKeyFileTextBox = new System.Windows.Forms.TextBox();
      this.SshUsernameTextBox = new System.Windows.Forms.TextBox();
      this.SshHostnameTextBox = new System.Windows.Forms.TextBox();
      this.MySqlDefaultSchemaLabel = new System.Windows.Forms.Label();
      this.HelpMySqlPasswordLabel = new System.Windows.Forms.Label();
      this.MySqlPasswordLabel = new System.Windows.Forms.Label();
      this.HelpMySqlUsernameLabel = new System.Windows.Forms.Label();
      this.HelpMySqlHostNameLabel = new System.Windows.Forms.Label();
      this.MySqlPortLabel = new System.Windows.Forms.Label();
      this.MySqlUsernameLabel = new System.Windows.Forms.Label();
      this.MySqlHostNameLabel = new System.Windows.Forms.Label();
      this.HelpMySqlPortLabel = new System.Windows.Forms.Label();
      this.SshKeyFileButton = new System.Windows.Forms.Button();
      this.HelpSshKeyFileLabel = new System.Windows.Forms.Label();
      this.SshKeyFileLabel = new System.Windows.Forms.Label();
      this.SshPasswordTextBox = new System.Windows.Forms.TextBox();
      this.HelpSshPasswordLabel = new System.Windows.Forms.Label();
      this.SshPasswordLabel = new System.Windows.Forms.Label();
      this.HelpSshUsernameLabel = new System.Windows.Forms.Label();
      this.HelpSshHostPortLabel = new System.Windows.Forms.Label();
      this.SshUsernameLabel = new System.Windows.Forms.Label();
      this.SshHostnameLabel = new System.Windows.Forms.Label();
      this.ParametersTabPage = new System.Windows.Forms.TabPage();
      this.DefaultSchemaComboBox = new System.Windows.Forms.ComboBox();
      this.MySqlXPortWarningLabel = new System.Windows.Forms.Label();
      this.MySqlXPortWarningPictureBox = new System.Windows.Forms.PictureBox();
      this.PasswordTextBox = new System.Windows.Forms.TextBox();
      this.HelpDefaultSchemaLabel = new System.Windows.Forms.Label();
      this.DefaultSchemaLabel = new System.Windows.Forms.Label();
      this.HelpPasswordLabel = new System.Windows.Forms.Label();
      this.PasswordLabel = new System.Windows.Forms.Label();
      this.HelpUsernameLabel = new System.Windows.Forms.Label();
      this.PortTextBox = new System.Windows.Forms.TextBox();
      this.UsernameTextBox = new System.Windows.Forms.TextBox();
      this.HostNameTextBox = new System.Windows.Forms.TextBox();
      this.SocketTextBox = new System.Windows.Forms.TextBox();
      this.HelpHostPortLabel = new System.Windows.Forms.Label();
      this.PortLabel = new System.Windows.Forms.Label();
      this.UsernameLabel = new System.Windows.Forms.Label();
      this.HostnameLabel = new System.Windows.Forms.Label();
      this.SocketLabel = new System.Windows.Forms.Label();
      this.HelpSocketLabel = new System.Windows.Forms.Label();
      this.ParametersTabControl = new System.Windows.Forms.TabControl();
      ((System.ComponentModel.ISupportInitialize)(this.ValidationsErrorProvider)).BeginInit();
      this.ContentAreaPanel.SuspendLayout();
      this.CommandAreaPanel.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ConnectionBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ConnectionStatusPictureBox)).BeginInit();
      this.AdvancedTabPage.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.TimeoutNumericUpDown)).BeginInit();
      this.SslTabPage.SuspendLayout();
      this.SshParametersTabPage.SuspendLayout();
      this.ParametersTabPage.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.MySqlXPortWarningPictureBox)).BeginInit();
      this.ParametersTabControl.SuspendLayout();
      this.SuspendLayout();
      // 
      // FootnoteAreaPanel
      // 
      this.FootnoteAreaPanel.Location = new System.Drawing.Point(0, 292);
      this.FootnoteAreaPanel.Size = new System.Drawing.Size(634, 0);
      // 
      // ContentAreaPanel
      // 
      this.ContentAreaPanel.Controls.Add(this.ParametersTabControl);
      this.ContentAreaPanel.Controls.Add(this.ConnectionStatusDescriptionLabel);
      this.ContentAreaPanel.Controls.Add(this.ConnectionStatusPictureBox);
      this.ContentAreaPanel.Controls.Add(this.ConnectionStatusLabel);
      this.ContentAreaPanel.Controls.Add(this.HelpConnectionNameLabel);
      this.ContentAreaPanel.Controls.Add(this.ConnectionMethodComboBox);
      this.ContentAreaPanel.Controls.Add(this.HelpConnectionMethodLabel);
      this.ContentAreaPanel.Controls.Add(this.ConnectionMethodLabel);
      this.ContentAreaPanel.Controls.Add(this.ConnectionNameTextBox);
      this.ContentAreaPanel.Controls.Add(this.ConnectionNameLabel);
      this.ContentAreaPanel.Size = new System.Drawing.Size(864, 522);
      // 
      // CommandAreaPanel
      // 
      this.CommandAreaPanel.Controls.Add(this.CancelationButton);
      this.CommandAreaPanel.Controls.Add(this.OKButton);
      this.CommandAreaPanel.Controls.Add(this.TestConnectionButton);
      this.CommandAreaPanel.Location = new System.Drawing.Point(0, 477);
      this.CommandAreaPanel.Size = new System.Drawing.Size(864, 45);
      // 
      // TestConnectionButton
      // 
      this.TestConnectionButton.AccessibleDescription = "A button to perform a connection test with the specified connection information";
      this.TestConnectionButton.AccessibleName = "Test Connection";
      this.TestConnectionButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.TestConnectionButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.TestConnectionButton.Location = new System.Drawing.Point(13, 10);
      this.TestConnectionButton.Name = "TestConnectionButton";
      this.TestConnectionButton.Size = new System.Drawing.Size(110, 23);
      this.TestConnectionButton.TabIndex = 0;
      this.TestConnectionButton.Text = "Test Connection";
      this.TestConnectionButton.UseVisualStyleBackColor = true;
      this.TestConnectionButton.Click += new System.EventHandler(this.TestConnectionButton_Click);
      // 
      // CancelationButton
      // 
      this.CancelationButton.AccessibleDescription = "A button to cancel any ongoing changes done to the connection and close the dialo" +
    "g";
      this.CancelationButton.AccessibleName = "Cancel";
      this.CancelationButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.CancelationButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.CancelationButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.CancelationButton.Location = new System.Drawing.Point(786, 10);
      this.CancelationButton.Name = "CancelationButton";
      this.CancelationButton.Size = new System.Drawing.Size(68, 23);
      this.CancelationButton.TabIndex = 2;
      this.CancelationButton.Text = "Cancel";
      this.CancelationButton.UseVisualStyleBackColor = true;
      // 
      // OKButton
      // 
      this.OKButton.AccessibleDescription = "A button to save the changes done to the connection information";
      this.OKButton.AccessibleName = "OK";
      this.OKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.OKButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.OKButton.Location = new System.Drawing.Point(716, 10);
      this.OKButton.Name = "OKButton";
      this.OKButton.Size = new System.Drawing.Size(64, 23);
      this.OKButton.TabIndex = 1;
      this.OKButton.Text = "OK";
      this.OKButton.UseVisualStyleBackColor = true;
      // 
      // ConnectionBindingSource
      // 
      this.ConnectionBindingSource.DataSource = typeof(MySql.Utility.Classes.MySqlWorkbench.MySqlWorkbenchConnection);
      // 
      // HelpConnectionNameLabel
      // 
      this.HelpConnectionNameLabel.AccessibleDescription = "A label displaying help text related to the connection name";
      this.HelpConnectionNameLabel.AccessibleName = "Connection Name Help";
      this.HelpConnectionNameLabel.AutoSize = true;
      this.HelpConnectionNameLabel.BackColor = System.Drawing.Color.Transparent;
      this.HelpConnectionNameLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.HelpConnectionNameLabel.Location = new System.Drawing.Point(519, 18);
      this.HelpConnectionNameLabel.Name = "HelpConnectionNameLabel";
      this.HelpConnectionNameLabel.Size = new System.Drawing.Size(175, 15);
      this.HelpConnectionNameLabel.TabIndex = 2;
      this.HelpConnectionNameLabel.Text = "Type a name for the connection";
      // 
      // ConnectionMethodComboBox
      // 
      this.ConnectionMethodComboBox.AccessibleDescription = "A combo box containing different connection method values to select from";
      this.ConnectionMethodComboBox.AccessibleName = "Connection Method";
      this.ConnectionMethodComboBox.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.ConnectionBindingSource, "ConnectionMethod", true));
      this.ConnectionMethodComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.ConnectionMethodComboBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ConnectionMethodComboBox.FormattingEnabled = true;
      this.ConnectionMethodComboBox.Location = new System.Drawing.Point(139, 44);
      this.ConnectionMethodComboBox.Name = "ConnectionMethodComboBox";
      this.ConnectionMethodComboBox.Size = new System.Drawing.Size(353, 23);
      this.ConnectionMethodComboBox.TabIndex = 4;
      this.ConnectionMethodComboBox.SelectedIndexChanged += new System.EventHandler(this.ConnectionMethodComboBox_SelectedIndexChanged);
      // 
      // HelpConnectionMethodLabel
      // 
      this.HelpConnectionMethodLabel.AccessibleDescription = "A label displaying help text related to the connection method";
      this.HelpConnectionMethodLabel.AccessibleName = "Connection Method Help";
      this.HelpConnectionMethodLabel.AutoSize = true;
      this.HelpConnectionMethodLabel.BackColor = System.Drawing.Color.Transparent;
      this.HelpConnectionMethodLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.HelpConnectionMethodLabel.Location = new System.Drawing.Point(519, 47);
      this.HelpConnectionMethodLabel.Name = "HelpConnectionMethodLabel";
      this.HelpConnectionMethodLabel.Size = new System.Drawing.Size(220, 15);
      this.HelpConnectionMethodLabel.TabIndex = 5;
      this.HelpConnectionMethodLabel.Text = "Method to use to connect to the RDBMS";
      // 
      // ConnectionMethodLabel
      // 
      this.ConnectionMethodLabel.AccessibleDescription = "A label displaying the text connection method";
      this.ConnectionMethodLabel.AccessibleName = "Connection Method Text";
      this.ConnectionMethodLabel.AutoSize = true;
      this.ConnectionMethodLabel.BackColor = System.Drawing.Color.Transparent;
      this.ConnectionMethodLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ConnectionMethodLabel.Location = new System.Drawing.Point(17, 47);
      this.ConnectionMethodLabel.Name = "ConnectionMethodLabel";
      this.ConnectionMethodLabel.Size = new System.Drawing.Size(117, 15);
      this.ConnectionMethodLabel.TabIndex = 3;
      this.ConnectionMethodLabel.Text = "Connection Method:";
      // 
      // ConnectionNameTextBox
      // 
      this.ConnectionNameTextBox.AccessibleDescription = "A text box to input the name that identifies the connection information";
      this.ConnectionNameTextBox.AccessibleName = "Connection Name";
      this.ConnectionNameTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ConnectionBindingSource, "Name", true));
      this.ConnectionNameTextBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ConnectionNameTextBox.Location = new System.Drawing.Point(138, 15);
      this.ConnectionNameTextBox.MaxLength = 358;
      this.ConnectionNameTextBox.Name = "ConnectionNameTextBox";
      this.ConnectionNameTextBox.Size = new System.Drawing.Size(354, 23);
      this.ConnectionNameTextBox.TabIndex = 1;
      this.ConnectionNameTextBox.TextChanged += new System.EventHandler(this.TextChangedHandler);
      this.ConnectionNameTextBox.Validated += new System.EventHandler(this.ValidatedHandler);
      // 
      // ConnectionNameLabel
      // 
      this.ConnectionNameLabel.AccessibleDescription = "A label displaying the text connection name";
      this.ConnectionNameLabel.AccessibleName = "Connection Name Text";
      this.ConnectionNameLabel.AutoSize = true;
      this.ConnectionNameLabel.BackColor = System.Drawing.Color.Transparent;
      this.ConnectionNameLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
      this.ConnectionNameLabel.Location = new System.Drawing.Point(25, 18);
      this.ConnectionNameLabel.Name = "ConnectionNameLabel";
      this.ConnectionNameLabel.Size = new System.Drawing.Size(107, 15);
      this.ConnectionNameLabel.TabIndex = 0;
      this.ConnectionNameLabel.Text = "Connection Name:";
      // 
      // ConnectionStatusLabel
      // 
      this.ConnectionStatusLabel.AccessibleDescription = "A label displaying the text connection status";
      this.ConnectionStatusLabel.AccessibleName = "Connection Status Text";
      this.ConnectionStatusLabel.AutoSize = true;
      this.ConnectionStatusLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ConnectionStatusLabel.Location = new System.Drawing.Point(25, 74);
      this.ConnectionStatusLabel.Name = "ConnectionStatusLabel";
      this.ConnectionStatusLabel.Size = new System.Drawing.Size(107, 15);
      this.ConnectionStatusLabel.TabIndex = 6;
      this.ConnectionStatusLabel.Text = "Connection Status:";
      // 
      // ConnectionStatusPictureBox
      // 
      this.ConnectionStatusPictureBox.AccessibleDescription = "A picture box displaying an icon representing the connection test result";
      this.ConnectionStatusPictureBox.AccessibleName = "Connection Status Icon";
      this.ConnectionStatusPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("ConnectionStatusPictureBox.Image")));
      this.ConnectionStatusPictureBox.Location = new System.Drawing.Point(138, 73);
      this.ConnectionStatusPictureBox.Name = "ConnectionStatusPictureBox";
      this.ConnectionStatusPictureBox.Size = new System.Drawing.Size(16, 16);
      this.ConnectionStatusPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
      this.ConnectionStatusPictureBox.TabIndex = 17;
      this.ConnectionStatusPictureBox.TabStop = false;
      // 
      // ConnectionStatusDescriptionLabel
      // 
      this.ConnectionStatusDescriptionLabel.AccessibleDescription = "A label displaying the result of a connection test with the stored information";
      this.ConnectionStatusDescriptionLabel.AccessibleName = "Connection Status";
      this.ConnectionStatusDescriptionLabel.AutoSize = true;
      this.ConnectionStatusDescriptionLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ConnectionStatusDescriptionLabel.Location = new System.Drawing.Point(158, 74);
      this.ConnectionStatusDescriptionLabel.Name = "ConnectionStatusDescriptionLabel";
      this.ConnectionStatusDescriptionLabel.Size = new System.Drawing.Size(58, 15);
      this.ConnectionStatusDescriptionLabel.TabIndex = 7;
      this.ConnectionStatusDescriptionLabel.Text = "Unknown";
      // 
      // StatusImageList
      // 
      this.StatusImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("StatusImageList.ImageStream")));
      this.StatusImageList.TransparentColor = System.Drawing.Color.Transparent;
      this.StatusImageList.Images.SetKeyName(0, "yes.png");
      this.StatusImageList.Images.SetKeyName(1, "no.png");
      this.StatusImageList.Images.SetKeyName(2, "help.png");
      // 
      // AdvancedTabPage
      // 
      this.AdvancedTabPage.AccessibleDescription = "A tab page containing advanced connection options";
      this.AdvancedTabPage.AccessibleName = "Advanced Tab";
      this.AdvancedTabPage.Controls.Add(this.TimeoutNumericUpDown);
      this.AdvancedTabPage.Controls.Add(this.TimeoutLabel);
      this.AdvancedTabPage.Controls.Add(this.HelpTimeoutLabel);
      this.AdvancedTabPage.Controls.Add(this.HelpUseCompressionLabel);
      this.AdvancedTabPage.Controls.Add(this.UseCompressionCheckBox);
      this.AdvancedTabPage.Location = new System.Drawing.Point(4, 24);
      this.AdvancedTabPage.Name = "AdvancedTabPage";
      this.AdvancedTabPage.Size = new System.Drawing.Size(606, 81);
      this.AdvancedTabPage.TabIndex = 2;
      this.AdvancedTabPage.Text = "Advanced";
      this.AdvancedTabPage.UseVisualStyleBackColor = true;
      // 
      // TimeoutNumericUpDown
      // 
      this.TimeoutNumericUpDown.AccessibleDescription = "A numeric up down control to enter a connection timeout in seconds before abortin" +
    "g a connection attempt";
      this.TimeoutNumericUpDown.AccessibleName = "Connection Timeout";
      this.TimeoutNumericUpDown.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.ConnectionBindingSource, "ConnectionTimeout", true));
      this.TimeoutNumericUpDown.Location = new System.Drawing.Point(123, 39);
      this.TimeoutNumericUpDown.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
      this.TimeoutNumericUpDown.Name = "TimeoutNumericUpDown";
      this.TimeoutNumericUpDown.Size = new System.Drawing.Size(89, 23);
      this.TimeoutNumericUpDown.TabIndex = 3;
      this.TimeoutNumericUpDown.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
      // 
      // TimeoutLabel
      // 
      this.TimeoutLabel.AccessibleDescription = "A label displaying the text timeout";
      this.TimeoutLabel.AccessibleName = "Connection Timeout Text";
      this.TimeoutLabel.AutoSize = true;
      this.TimeoutLabel.Location = new System.Drawing.Point(62, 41);
      this.TimeoutLabel.Name = "TimeoutLabel";
      this.TimeoutLabel.Size = new System.Drawing.Size(55, 15);
      this.TimeoutLabel.TabIndex = 2;
      this.TimeoutLabel.Text = "Timeout:";
      // 
      // HelpTimeoutLabel
      // 
      this.HelpTimeoutLabel.AccessibleDescription = "A label displaying help text related to the connection timeout before aborting a " +
    "connection attempt";
      this.HelpTimeoutLabel.AccessibleName = "Connection Timeout Help";
      this.HelpTimeoutLabel.AutoSize = true;
      this.HelpTimeoutLabel.Location = new System.Drawing.Point(442, 44);
      this.HelpTimeoutLabel.Name = "HelpTimeoutLabel";
      this.HelpTimeoutLabel.Size = new System.Drawing.Size(372, 15);
      this.HelpTimeoutLabel.TabIndex = 4;
      this.HelpTimeoutLabel.Text = "Maximum number of seconds to wait before a connection is aborted.";
      // 
      // HelpUseCompressionLabel
      // 
      this.HelpUseCompressionLabel.AccessibleDescription = "A label displaying help text related to the use of compression in the communicati" +
    "on protocol";
      this.HelpUseCompressionLabel.AccessibleName = "Use Compression Help";
      this.HelpUseCompressionLabel.AutoSize = true;
      this.HelpUseCompressionLabel.Location = new System.Drawing.Point(442, 15);
      this.HelpUseCompressionLabel.Name = "HelpUseCompressionLabel";
      this.HelpUseCompressionLabel.Size = new System.Drawing.Size(215, 15);
      this.HelpUseCompressionLabel.TabIndex = 1;
      this.HelpUseCompressionLabel.Text = "Select this option for WAN connections";
      // 
      // UseCompressionCheckBox
      // 
      this.UseCompressionCheckBox.AccessibleDescription = "A check box to use compression in the communication protocol";
      this.UseCompressionCheckBox.AccessibleName = "Use Compression";
      this.UseCompressionCheckBox.AutoSize = true;
      this.UseCompressionCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.ConnectionBindingSource, "UseCompression", true));
      this.UseCompressionCheckBox.Location = new System.Drawing.Point(123, 14);
      this.UseCompressionCheckBox.Name = "UseCompressionCheckBox";
      this.UseCompressionCheckBox.Size = new System.Drawing.Size(167, 19);
      this.UseCompressionCheckBox.TabIndex = 0;
      this.UseCompressionCheckBox.Text = "Use compression protocol.";
      this.UseCompressionCheckBox.UseVisualStyleBackColor = true;
      // 
      // SslTabPage
      // 
      this.SslTabPage.AccessibleDescription = "A tab page containing SSL connection options";
      this.SslTabPage.AccessibleName = "SSL Tab";
      this.SslTabPage.Controls.Add(this.SslPemMoreInfoLinkLabel);
      this.SslTabPage.Controls.Add(this.SslCertFileButton);
      this.SslTabPage.Controls.Add(this.UseSslComboBox);
      this.SslTabPage.Controls.Add(this.SslCaFileButton);
      this.SslTabPage.Controls.Add(this.HelpUseSslLabel);
      this.SslTabPage.Controls.Add(this.SslKeyFileButton);
      this.SslTabPage.Controls.Add(this.UseSslLabel);
      this.SslTabPage.Controls.Add(this.SslKeyFileTextBox);
      this.SslTabPage.Controls.Add(this.SslCaFileLabel);
      this.SslTabPage.Controls.Add(this.HelpSslKeyLabel);
      this.SslTabPage.Controls.Add(this.SslCaFileTextBox);
      this.SslTabPage.Controls.Add(this.SslKeyLabel);
      this.SslTabPage.Controls.Add(this.HelpSslCaFileLabel);
      this.SslTabPage.Controls.Add(this.SslCertFileTextBox);
      this.SslTabPage.Controls.Add(this.SslCertFileLabel);
      this.SslTabPage.Controls.Add(this.HelpSslCertFileLabel);
      this.SslTabPage.Location = new System.Drawing.Point(4, 24);
      this.SslTabPage.Name = "SslTabPage";
      this.SslTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.SslTabPage.Size = new System.Drawing.Size(606, 81);
      this.SslTabPage.TabIndex = 1;
      this.SslTabPage.Text = "SSL";
      this.SslTabPage.UseVisualStyleBackColor = true;
      // 
      // SslPemMoreInfoLinkLabel
      // 
      this.SslPemMoreInfoLinkLabel.AccessibleDescription = "A link label to open a web page with information about creating SSL certificate a" +
    "nd key files";
      this.SslPemMoreInfoLinkLabel.AccessibleName = "SSL PEM Files Help Link";
      this.SslPemMoreInfoLinkLabel.AutoSize = true;
      this.SslPemMoreInfoLinkLabel.Location = new System.Drawing.Point(122, 142);
      this.SslPemMoreInfoLinkLabel.Name = "SslPemMoreInfoLinkLabel";
      this.SslPemMoreInfoLinkLabel.Size = new System.Drawing.Size(352, 15);
      this.SslPemMoreInfoLinkLabel.TabIndex = 14;
      this.SslPemMoreInfoLinkLabel.TabStop = true;
      this.SslPemMoreInfoLinkLabel.Text = "See more information about creating SSL Certificate and Key files.";
      this.SslPemMoreInfoLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.SslPemMoreInfoLinkLabel_LinkClicked);
      // 
      // SslCertFileButton
      // 
      this.SslCertFileButton.AccessibleDescription = "A button  to browse the file system for a SSL client certificate file";
      this.SslCertFileButton.AccessibleName = "SSL Client Certificate File Browse";
      this.SslCertFileButton.Location = new System.Drawing.Point(452, 72);
      this.SslCertFileButton.Name = "SslCertFileButton";
      this.SslCertFileButton.Size = new System.Drawing.Size(26, 23);
      this.SslCertFileButton.TabIndex = 8;
      this.SslCertFileButton.Text = "...";
      this.SslCertFileButton.UseVisualStyleBackColor = true;
      this.SslCertFileButton.Click += new System.EventHandler(this.SslCertFileButton_Click);
      // 
      // UseSslComboBox
      // 
      this.UseSslComboBox.AccessibleDescription = "A combo box with different SSL encryption options";
      this.UseSslComboBox.AccessibleName = "Use SSL Encryption";
      this.UseSslComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
      this.UseSslComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
      this.UseSslComboBox.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.ConnectionBindingSource, "UseSsl", true));
      this.UseSslComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.UseSslComboBox.FormattingEnabled = true;
      this.UseSslComboBox.Location = new System.Drawing.Point(123, 14);
      this.UseSslComboBox.Name = "UseSslComboBox";
      this.UseSslComboBox.Size = new System.Drawing.Size(355, 23);
      this.UseSslComboBox.TabIndex = 1;
      this.UseSslComboBox.SelectedIndexChanged += new System.EventHandler(this.UseSslComboBox_SelectedIndexChanged);
      // 
      // SslCaFileButton
      // 
      this.SslCaFileButton.AccessibleDescription = "A button  to browse the file system for a SSL certification authority file";
      this.SslCaFileButton.AccessibleName = "SSL Certification Authority File Browse";
      this.SslCaFileButton.Location = new System.Drawing.Point(452, 101);
      this.SslCaFileButton.Name = "SslCaFileButton";
      this.SslCaFileButton.Size = new System.Drawing.Size(26, 23);
      this.SslCaFileButton.TabIndex = 12;
      this.SslCaFileButton.Text = "...";
      this.SslCaFileButton.UseVisualStyleBackColor = true;
      this.SslCaFileButton.Click += new System.EventHandler(this.SslCaFileButton_Click);
      // 
      // HelpUseSslLabel
      // 
      this.HelpUseSslLabel.AccessibleDescription = "A label displaying help text related to the SSL encryption options";
      this.HelpUseSslLabel.AccessibleName = "Use SSL Help";
      this.HelpUseSslLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.HelpUseSslLabel.AutoSize = true;
      this.HelpUseSslLabel.Location = new System.Drawing.Point(5552, 18);
      this.HelpUseSslLabel.Name = "HelpUseSslLabel";
      this.HelpUseSslLabel.Size = new System.Drawing.Size(224, 15);
      this.HelpUseSslLabel.TabIndex = 47;
      this.HelpUseSslLabel.Text = "Connection will fail if SSL is not available.";
      // 
      // SslKeyFileButton
      // 
      this.SslKeyFileButton.Location = new System.Drawing.Point(452, 43);
      this.SslKeyFileButton.Name = "SslKeyFileButton";
      this.SslKeyFileButton.Size = new System.Drawing.Size(26, 23);
      this.SslKeyFileButton.TabIndex = 4;
      this.SslKeyFileButton.Text = "...";
      this.SslKeyFileButton.UseVisualStyleBackColor = true;
      this.SslKeyFileButton.Click += new System.EventHandler(this.SslKeyButton_Click);
      // 
      // UseSslLabel
      // 
      this.UseSslLabel.AccessibleDescription = "A label displaying the text use SSL";
      this.UseSslLabel.AccessibleName = "Use SSL Text";
      this.UseSslLabel.AutoSize = true;
      this.UseSslLabel.Location = new System.Drawing.Point(67, 17);
      this.UseSslLabel.Name = "UseSslLabel";
      this.UseSslLabel.Size = new System.Drawing.Size(50, 15);
      this.UseSslLabel.TabIndex = 0;
      this.UseSslLabel.Text = "Use SSL:";
      // 
      // SslKeyFileTextBox
      // 
      this.SslKeyFileTextBox.AccessibleDescription = "A text box to input the full file path of the SSL client key file";
      this.SslKeyFileTextBox.AccessibleName = "SSL Client Key File";
      this.SslKeyFileTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ConnectionBindingSource, "SslKeyFile", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.SslKeyFileTextBox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.SslKeyFileTextBox.Location = new System.Drawing.Point(123, 43);
      this.SslKeyFileTextBox.MaxLength = 679;
      this.SslKeyFileTextBox.Name = "SslKeyFileTextBox";
      this.SslKeyFileTextBox.Size = new System.Drawing.Size(323, 23);
      this.SslKeyFileTextBox.TabIndex = 3;
      this.SslKeyFileTextBox.TextChanged += new System.EventHandler(this.TextChangedHandler);
      this.SslKeyFileTextBox.Validated += new System.EventHandler(this.ValidatedHandler);
      // 
      // SslCaFileLabel
      // 
      this.SslCaFileLabel.AccessibleDescription = "A label displaying the text SSL CA file";
      this.SslCaFileLabel.AccessibleName = "SSL Certification Authority File Text";
      this.SslCaFileLabel.AutoSize = true;
      this.SslCaFileLabel.Location = new System.Drawing.Point(49, 104);
      this.SslCaFileLabel.Name = "SslCaFileLabel";
      this.SslCaFileLabel.Size = new System.Drawing.Size(68, 15);
      this.SslCaFileLabel.TabIndex = 10;
      this.SslCaFileLabel.Text = "SSL CA File:";
      // 
      // HelpSslKeyLabel
      // 
      this.HelpSslKeyLabel.AccessibleDescription = "A label displaying help text related to the SSL client key file";
      this.HelpSslKeyLabel.AccessibleName = "SSL Client Key File Help";
      this.HelpSslKeyLabel.AutoSize = true;
      this.HelpSslKeyLabel.Location = new System.Drawing.Point(505, 47);
      this.HelpSslKeyLabel.Name = "HelpSslKeyLabel";
      this.HelpSslKeyLabel.Size = new System.Drawing.Size(162, 15);
      this.HelpSslKeyLabel.TabIndex = 5;
      this.HelpSslKeyLabel.Text = "Path to Client Key file for SSL.";
      // 
      // SslCaFileTextBox
      // 
      this.SslCaFileTextBox.AccessibleDescription = "A text box to input the full file path of the SSL certification authority file";
      this.SslCaFileTextBox.AccessibleName = "SSL Certification Authority File";
      this.SslCaFileTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ConnectionBindingSource, "SslCertificationAuthorityFile", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.SslCaFileTextBox.Location = new System.Drawing.Point(123, 101);
      this.SslCaFileTextBox.MaxLength = 679;
      this.SslCaFileTextBox.Name = "SslCaFileTextBox";
      this.SslCaFileTextBox.Size = new System.Drawing.Size(323, 23);
      this.SslCaFileTextBox.TabIndex = 11;
      this.SslCaFileTextBox.TextChanged += new System.EventHandler(this.TextChangedHandler);
      this.SslCaFileTextBox.Validated += new System.EventHandler(this.ValidatedHandler);
      // 
      // SslKeyLabel
      // 
      this.SslKeyLabel.AccessibleDescription = "A label displaying the text SSL Key File";
      this.SslKeyLabel.AccessibleName = "SSL Client Key File Text";
      this.SslKeyLabel.AutoSize = true;
      this.SslKeyLabel.BackColor = System.Drawing.Color.Transparent;
      this.SslKeyLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.SslKeyLabel.Location = new System.Drawing.Point(46, 47);
      this.SslKeyLabel.Name = "SslKeyLabel";
      this.SslKeyLabel.Size = new System.Drawing.Size(71, 15);
      this.SslKeyLabel.TabIndex = 2;
      this.SslKeyLabel.Text = "SSL Key File:";
      // 
      // HelpSslCaFileLabel
      // 
      this.HelpSslCaFileLabel.AccessibleDescription = "A label displaying help text related to the SSL certification authority file";
      this.HelpSslCaFileLabel.AccessibleName = "SSL Certification Authority File Help";
      this.HelpSslCaFileLabel.AutoSize = true;
      this.HelpSslCaFileLabel.Location = new System.Drawing.Point(505, 104);
      this.HelpSslCaFileLabel.Name = "HelpSslCaFileLabel";
      this.HelpSslCaFileLabel.Size = new System.Drawing.Size(227, 15);
      this.HelpSslCaFileLabel.TabIndex = 13;
      this.HelpSslCaFileLabel.Text = "Path to Certification Authority file for SSL.";
      // 
      // SslCertFileTextBox
      // 
      this.SslCertFileTextBox.AccessibleDescription = "A text box to input the full file path of the SSL client certificate file";
      this.SslCertFileTextBox.AccessibleName = "SSL Client Certificate File";
      this.SslCertFileTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ConnectionBindingSource, "SslClientCertificateFile", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.SslCertFileTextBox.Location = new System.Drawing.Point(123, 72);
      this.SslCertFileTextBox.MaxLength = 679;
      this.SslCertFileTextBox.Name = "SslCertFileTextBox";
      this.SslCertFileTextBox.Size = new System.Drawing.Size(323, 23);
      this.SslCertFileTextBox.TabIndex = 7;
      this.SslCertFileTextBox.TextChanged += new System.EventHandler(this.TextChangedHandler);
      this.SslCertFileTextBox.Validated += new System.EventHandler(this.ValidatedHandler);
      // 
      // SslCertFileLabel
      // 
      this.SslCertFileLabel.AccessibleDescription = "A label displaying the text SSL certificate file";
      this.SslCertFileLabel.AccessibleName = "SSL Client Certificate File Text";
      this.SslCertFileLabel.AutoSize = true;
      this.SslCertFileLabel.BackColor = System.Drawing.Color.Transparent;
      this.SslCertFileLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.SslCertFileLabel.Location = new System.Drawing.Point(38, 76);
      this.SslCertFileLabel.Name = "SslCertFileLabel";
      this.SslCertFileLabel.Size = new System.Drawing.Size(79, 15);
      this.SslCertFileLabel.TabIndex = 6;
      this.SslCertFileLabel.Text = "SSL CERT File:";
      // 
      // HelpSslCertFileLabel
      // 
      this.HelpSslCertFileLabel.AccessibleDescription = "A label displaying help text related to the SSL client certificate file";
      this.HelpSslCertFileLabel.AccessibleName = "SSL Client Certificate File Help";
      this.HelpSslCertFileLabel.AutoSize = true;
      this.HelpSslCertFileLabel.Location = new System.Drawing.Point(505, 76);
      this.HelpSslCertFileLabel.Name = "HelpSslCertFileLabel";
      this.HelpSslCertFileLabel.Size = new System.Drawing.Size(197, 15);
      this.HelpSslCertFileLabel.TabIndex = 9;
      this.HelpSslCertFileLabel.Text = "Path to Client Certificate file for SSL.";
      // 
      // SshParametersTabPage
      // 
      this.SshParametersTabPage.AccessibleDescription = "A tab page containing SSH tunnel connection parameters";
      this.SshParametersTabPage.AccessibleName = "SSH Parameters Tab";
      this.SshParametersTabPage.Controls.Add(this.MySqlDefaultSchemaComboBox);
      this.SshParametersTabPage.Controls.Add(this.SshPassPhraseTextBox);
      this.SshParametersTabPage.Controls.Add(this.HelpSshPassPhraseLabel);
      this.SshParametersTabPage.Controls.Add(this.SshPassPhraseLabel);
      this.SshParametersTabPage.Controls.Add(this.MySqlPasswordTextBox);
      this.SshParametersTabPage.Controls.Add(this.HelpMySqlDefaultSchemaLabel);
      this.SshParametersTabPage.Controls.Add(this.MySqlPortTextBox);
      this.SshParametersTabPage.Controls.Add(this.MySqlUsernameTextBox);
      this.SshParametersTabPage.Controls.Add(this.MySqlHostNameTextBox);
      this.SshParametersTabPage.Controls.Add(this.SshKeyFileTextBox);
      this.SshParametersTabPage.Controls.Add(this.SshUsernameTextBox);
      this.SshParametersTabPage.Controls.Add(this.SshHostnameTextBox);
      this.SshParametersTabPage.Controls.Add(this.MySqlDefaultSchemaLabel);
      this.SshParametersTabPage.Controls.Add(this.HelpMySqlPasswordLabel);
      this.SshParametersTabPage.Controls.Add(this.MySqlPasswordLabel);
      this.SshParametersTabPage.Controls.Add(this.HelpMySqlUsernameLabel);
      this.SshParametersTabPage.Controls.Add(this.HelpMySqlHostNameLabel);
      this.SshParametersTabPage.Controls.Add(this.MySqlPortLabel);
      this.SshParametersTabPage.Controls.Add(this.MySqlUsernameLabel);
      this.SshParametersTabPage.Controls.Add(this.MySqlHostNameLabel);
      this.SshParametersTabPage.Controls.Add(this.HelpMySqlPortLabel);
      this.SshParametersTabPage.Controls.Add(this.SshKeyFileButton);
      this.SshParametersTabPage.Controls.Add(this.HelpSshKeyFileLabel);
      this.SshParametersTabPage.Controls.Add(this.SshKeyFileLabel);
      this.SshParametersTabPage.Controls.Add(this.SshPasswordTextBox);
      this.SshParametersTabPage.Controls.Add(this.HelpSshPasswordLabel);
      this.SshParametersTabPage.Controls.Add(this.SshPasswordLabel);
      this.SshParametersTabPage.Controls.Add(this.HelpSshUsernameLabel);
      this.SshParametersTabPage.Controls.Add(this.HelpSshHostPortLabel);
      this.SshParametersTabPage.Controls.Add(this.SshUsernameLabel);
      this.SshParametersTabPage.Controls.Add(this.SshHostnameLabel);
      this.SshParametersTabPage.Location = new System.Drawing.Point(4, 24);
      this.SshParametersTabPage.Name = "SshParametersTabPage";
      this.SshParametersTabPage.Size = new System.Drawing.Size(606, 81);
      this.SshParametersTabPage.TabIndex = 3;
      this.SshParametersTabPage.Text = "Parameters";
      this.SshParametersTabPage.UseVisualStyleBackColor = true;
      // 
      // MySqlDefaultSchemaComboBox
      // 
      this.MySqlDefaultSchemaComboBox.AccessibleDescription = "A combo box displaying all schemas in the server to select the default one";
      this.MySqlDefaultSchemaComboBox.AccessibleName = "MySQL Default Schema";
      this.MySqlDefaultSchemaComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
      this.MySqlDefaultSchemaComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
      this.MySqlDefaultSchemaComboBox.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.ConnectionBindingSource, "DefaultSchema", true));
      this.MySqlDefaultSchemaComboBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ConnectionBindingSource, "DefaultSchema", true));
      this.MySqlDefaultSchemaComboBox.FormattingEnabled = true;
      this.MySqlDefaultSchemaComboBox.Location = new System.Drawing.Point(123, 276);
      this.MySqlDefaultSchemaComboBox.Name = "MySqlDefaultSchemaComboBox";
      this.MySqlDefaultSchemaComboBox.Size = new System.Drawing.Size(355, 23);
      this.MySqlDefaultSchemaComboBox.TabIndex = 29;
      this.MySqlDefaultSchemaComboBox.DropDown += new System.EventHandler(this.MySqlDefaultSchemaComboBox_DropDown);
      this.MySqlDefaultSchemaComboBox.TextChanged += new System.EventHandler(this.TextChangedHandler);
      this.MySqlDefaultSchemaComboBox.Validated += new System.EventHandler(this.ValidatedHandler);
      // 
      // SshPassPhraseTextBox
      // 
      this.SshPassPhraseTextBox.AccessibleDescription = "A text box to input the SSH key file\'s pass phrase to connect to the SSH tunnel";
      this.SshPassPhraseTextBox.AccessibleName = "SSH Pass Phrase";
      this.SshPassPhraseTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ConnectionBindingSource, "SshPassPhrase", true));
      this.SshPassPhraseTextBox.Location = new System.Drawing.Point(123, 131);
      this.SshPassPhraseTextBox.MaxLength = 679;
      this.SshPassPhraseTextBox.Name = "SshPassPhraseTextBox";
      this.SshPassPhraseTextBox.ReadOnly = true;
      this.SshPassPhraseTextBox.Size = new System.Drawing.Size(355, 23);
      this.SshPassPhraseTextBox.TabIndex = 14;
      this.SshPassPhraseTextBox.UseSystemPasswordChar = true;
      this.SshPassPhraseTextBox.TextChanged += new System.EventHandler(this.TextChangedHandler);
      this.SshPassPhraseTextBox.Validated += new System.EventHandler(this.ValidatedHandler);
      // 
      // HelpSshPassPhraseLabel
      // 
      this.HelpSshPassPhraseLabel.AccessibleDescription = "A label displaying help text related to the pass phrase for the SSH key file";
      this.HelpSshPassPhraseLabel.AccessibleName = "SSH Pass Phrase Help";
      this.HelpSshPassPhraseLabel.AutoSize = true;
      this.HelpSshPassPhraseLabel.Location = new System.Drawing.Point(505, 134);
      this.HelpSshPassPhraseLabel.Name = "HelpSshPassPhraseLabel";
      this.HelpSshPassPhraseLabel.Size = new System.Drawing.Size(182, 15);
      this.HelpSshPassPhraseLabel.TabIndex = 15;
      this.HelpSshPassPhraseLabel.Text = "SSH private key file\'s pass phrase.";
      // 
      // SshPassPhraseLabel
      // 
      this.SshPassPhraseLabel.AccessibleDescription = "A label displaying the text SSH pass phrase";
      this.SshPassPhraseLabel.AccessibleName = "SSH Pass Phrase Text";
      this.SshPassPhraseLabel.AutoSize = true;
      this.SshPassPhraseLabel.BackColor = System.Drawing.Color.Transparent;
      this.SshPassPhraseLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.SshPassPhraseLabel.Location = new System.Drawing.Point(22, 134);
      this.SshPassPhraseLabel.Name = "SshPassPhraseLabel";
      this.SshPassPhraseLabel.Size = new System.Drawing.Size(95, 15);
      this.SshPassPhraseLabel.TabIndex = 13;
      this.SshPassPhraseLabel.Text = "SSH Pass Phrase:";
      // 
      // MySqlPasswordTextBox
      // 
      this.MySqlPasswordTextBox.AccessibleDescription = "A text box to input the MySQL user\'s password";
      this.MySqlPasswordTextBox.AccessibleName = "MySQL Password";
      this.MySqlPasswordTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ConnectionBindingSource, "Password", true));
      this.MySqlPasswordTextBox.Location = new System.Drawing.Point(123, 247);
      this.MySqlPasswordTextBox.MaxLength = 679;
      this.MySqlPasswordTextBox.Name = "MySqlPasswordTextBox";
      this.MySqlPasswordTextBox.Size = new System.Drawing.Size(355, 23);
      this.MySqlPasswordTextBox.TabIndex = 26;
      this.MySqlPasswordTextBox.UseSystemPasswordChar = true;
      // 
      // HelpMySqlDefaultSchemaLabel
      // 
      this.HelpMySqlDefaultSchemaLabel.AccessibleDescription = "A label displaying help text related to the default schema";
      this.HelpMySqlDefaultSchemaLabel.AccessibleName = "MySQL Default Schema Help";
      this.HelpMySqlDefaultSchemaLabel.AutoSize = true;
      this.HelpMySqlDefaultSchemaLabel.Location = new System.Drawing.Point(505, 280);
      this.HelpMySqlDefaultSchemaLabel.Name = "HelpMySqlDefaultSchemaLabel";
      this.HelpMySqlDefaultSchemaLabel.Size = new System.Drawing.Size(262, 15);
      this.HelpMySqlDefaultSchemaLabel.TabIndex = 30;
      this.HelpMySqlDefaultSchemaLabel.Text = "The default schema, leave blank to select it later.";
      // 
      // MySqlPortTextBox
      // 
      this.MySqlPortTextBox.AccessibleDescription = "A text box to input the port number of the MySQL server";
      this.MySqlPortTextBox.AccessibleName = "MySQL Port Number";
      this.MySqlPortTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ConnectionBindingSource, "Port", true));
      this.MySqlPortTextBox.Location = new System.Drawing.Point(123, 189);
      this.MySqlPortTextBox.Name = "MySqlPortTextBox";
      this.MySqlPortTextBox.Size = new System.Drawing.Size(355, 23);
      this.MySqlPortTextBox.TabIndex = 20;
      this.MySqlPortTextBox.TextChanged += new System.EventHandler(this.TextChangedHandler);
      this.MySqlPortTextBox.Validated += new System.EventHandler(this.ValidatedHandler);
      // 
      // MySqlUsernameTextBox
      // 
      this.MySqlUsernameTextBox.AccessibleDescription = "A text box to input the MySQL user name to connect with";
      this.MySqlUsernameTextBox.AccessibleName = "MySQL User Name";
      this.MySqlUsernameTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ConnectionBindingSource, "UserName", true));
      this.MySqlUsernameTextBox.Location = new System.Drawing.Point(123, 218);
      this.MySqlUsernameTextBox.MaxLength = 679;
      this.MySqlUsernameTextBox.Name = "MySqlUsernameTextBox";
      this.MySqlUsernameTextBox.Size = new System.Drawing.Size(355, 23);
      this.MySqlUsernameTextBox.TabIndex = 23;
      this.MySqlUsernameTextBox.TextChanged += new System.EventHandler(this.TextChangedHandler);
      this.MySqlUsernameTextBox.Validated += new System.EventHandler(this.ValidatedHandler);
      // 
      // MySqlHostNameTextBox
      // 
      this.MySqlHostNameTextBox.AccessibleDescription = "A text box to input the host name of the MySQL server to connect to";
      this.MySqlHostNameTextBox.AccessibleName = "MySQL Host Name";
      this.MySqlHostNameTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ConnectionBindingSource, "Host", true));
      this.MySqlHostNameTextBox.Location = new System.Drawing.Point(123, 160);
      this.MySqlHostNameTextBox.MaxLength = 484;
      this.MySqlHostNameTextBox.Name = "MySqlHostNameTextBox";
      this.MySqlHostNameTextBox.Size = new System.Drawing.Size(355, 23);
      this.MySqlHostNameTextBox.TabIndex = 17;
      this.MySqlHostNameTextBox.TextChanged += new System.EventHandler(this.TextChangedHandler);
      this.MySqlHostNameTextBox.Validated += new System.EventHandler(this.ValidatedHandler);
      // 
      // SshKeyFileTextBox
      // 
      this.SshKeyFileTextBox.AccessibleDescription = "A text box to input the full SSH key file path";
      this.SshKeyFileTextBox.AccessibleName = "SSH Key File";
      this.SshKeyFileTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ConnectionBindingSource, "SshPrivateKeyFile", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.SshKeyFileTextBox.Location = new System.Drawing.Point(123, 102);
      this.SshKeyFileTextBox.MaxLength = 679;
      this.SshKeyFileTextBox.Name = "SshKeyFileTextBox";
      this.SshKeyFileTextBox.Size = new System.Drawing.Size(323, 23);
      this.SshKeyFileTextBox.TabIndex = 10;
      this.SshKeyFileTextBox.TextChanged += new System.EventHandler(this.TextChangedHandler);
      this.SshKeyFileTextBox.Validated += new System.EventHandler(this.ValidatedHandler);
      // 
      // SshUsernameTextBox
      // 
      this.SshUsernameTextBox.AccessibleDescription = "A text box to input the SSH user name to connect to the SSH tunnel";
      this.SshUsernameTextBox.AccessibleName = "SSH User Name";
      this.SshUsernameTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ConnectionBindingSource, "SshUserName", true));
      this.SshUsernameTextBox.Location = new System.Drawing.Point(123, 44);
      this.SshUsernameTextBox.MaxLength = 679;
      this.SshUsernameTextBox.Name = "SshUsernameTextBox";
      this.SshUsernameTextBox.Size = new System.Drawing.Size(355, 23);
      this.SshUsernameTextBox.TabIndex = 4;
      this.SshUsernameTextBox.TextChanged += new System.EventHandler(this.TextChangedHandler);
      this.SshUsernameTextBox.Validated += new System.EventHandler(this.ValidatedHandler);
      // 
      // SshHostnameTextBox
      // 
      this.SshHostnameTextBox.AccessibleDescription = "A text box to input the SSH server host name in a bridged connection";
      this.SshHostnameTextBox.AccessibleName = "SSH Host Name";
      this.SshHostnameTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ConnectionBindingSource, "SshHost", true));
      this.SshHostnameTextBox.Location = new System.Drawing.Point(123, 14);
      this.SshHostnameTextBox.MaxLength = 484;
      this.SshHostnameTextBox.Name = "SshHostnameTextBox";
      this.SshHostnameTextBox.Size = new System.Drawing.Size(355, 23);
      this.SshHostnameTextBox.TabIndex = 1;
      this.SshHostnameTextBox.TextChanged += new System.EventHandler(this.TextChangedHandler);
      this.SshHostnameTextBox.Validated += new System.EventHandler(this.ValidatedHandler);
      // 
      // MySqlDefaultSchemaLabel
      // 
      this.MySqlDefaultSchemaLabel.AccessibleDescription = "A label displaying the text default schema";
      this.MySqlDefaultSchemaLabel.AccessibleName = "MySQL Default Schema Text";
      this.MySqlDefaultSchemaLabel.AutoSize = true;
      this.MySqlDefaultSchemaLabel.Location = new System.Drawing.Point(24, 278);
      this.MySqlDefaultSchemaLabel.Name = "MySqlDefaultSchemaLabel";
      this.MySqlDefaultSchemaLabel.Size = new System.Drawing.Size(93, 15);
      this.MySqlDefaultSchemaLabel.TabIndex = 28;
      this.MySqlDefaultSchemaLabel.Text = "Default Schema:";
      // 
      // HelpMySqlPasswordLabel
      // 
      this.HelpMySqlPasswordLabel.AccessibleDescription = "A label displaying help text related to the MySQL password in the SSH parameters " +
    "section";
      this.HelpMySqlPasswordLabel.AccessibleName = "MySQL Password Help";
      this.HelpMySqlPasswordLabel.AutoSize = true;
      this.HelpMySqlPasswordLabel.Location = new System.Drawing.Point(505, 252);
      this.HelpMySqlPasswordLabel.Name = "HelpMySqlPasswordLabel";
      this.HelpMySqlPasswordLabel.Size = new System.Drawing.Size(291, 15);
      this.HelpMySqlPasswordLabel.TabIndex = 27;
      this.HelpMySqlPasswordLabel.Text = "The MySQL user\'s password, stored in a secured vault.";
      // 
      // MySqlPasswordLabel
      // 
      this.MySqlPasswordLabel.AccessibleDescription = "A label displaying the text password";
      this.MySqlPasswordLabel.AccessibleName = "MySQL Password Text";
      this.MySqlPasswordLabel.AutoSize = true;
      this.MySqlPasswordLabel.BackColor = System.Drawing.Color.Transparent;
      this.MySqlPasswordLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.MySqlPasswordLabel.Location = new System.Drawing.Point(57, 249);
      this.MySqlPasswordLabel.Name = "MySqlPasswordLabel";
      this.MySqlPasswordLabel.Size = new System.Drawing.Size(60, 15);
      this.MySqlPasswordLabel.TabIndex = 25;
      this.MySqlPasswordLabel.Text = "Password:";
      // 
      // HelpMySqlUsernameLabel
      // 
      this.HelpMySqlUsernameLabel.AccessibleDescription = "A label displaying help text related to the MySQL user name in the SSH parameters" +
    " section";
      this.HelpMySqlUsernameLabel.AccessibleName = "MySQL User Name Help";
      this.HelpMySqlUsernameLabel.AutoSize = true;
      this.HelpMySqlUsernameLabel.Location = new System.Drawing.Point(505, 221);
      this.HelpMySqlUsernameLabel.Name = "HelpMySqlUsernameLabel";
      this.HelpMySqlUsernameLabel.Size = new System.Drawing.Size(187, 15);
      this.HelpMySqlUsernameLabel.TabIndex = 24;
      this.HelpMySqlUsernameLabel.Text = "Name of the user to connect with.";
      // 
      // HelpMySqlHostNameLabel
      // 
      this.HelpMySqlHostNameLabel.AccessibleDescription = "A label displaying help text related to the MySQL host name in the SSH parameters" +
    " section";
      this.HelpMySqlHostNameLabel.AccessibleName = "MySQL Host Name Help";
      this.HelpMySqlHostNameLabel.AutoSize = true;
      this.HelpMySqlHostNameLabel.Location = new System.Drawing.Point(505, 165);
      this.HelpMySqlHostNameLabel.Name = "HelpMySqlHostNameLabel";
      this.HelpMySqlHostNameLabel.Size = new System.Drawing.Size(242, 15);
      this.HelpMySqlHostNameLabel.TabIndex = 18;
      this.HelpMySqlHostNameLabel.Text = "MySQL Server host relative to the SSH server.";
      // 
      // MySqlPortLabel
      // 
      this.MySqlPortLabel.AccessibleDescription = "A label displaying the text MySQL server port";
      this.MySqlPortLabel.AccessibleName = "MySQL Port Number Text";
      this.MySqlPortLabel.AutoSize = true;
      this.MySqlPortLabel.Location = new System.Drawing.Point(9, 191);
      this.MySqlPortLabel.Name = "MySqlPortLabel";
      this.MySqlPortLabel.Size = new System.Drawing.Size(108, 15);
      this.MySqlPortLabel.TabIndex = 19;
      this.MySqlPortLabel.Text = "MySQL Server Port:";
      // 
      // MySqlUsernameLabel
      // 
      this.MySqlUsernameLabel.AccessibleDescription = "A label displaying the text user name";
      this.MySqlUsernameLabel.AccessibleName = "MySQL User Name Text";
      this.MySqlUsernameLabel.AutoSize = true;
      this.MySqlUsernameLabel.Location = new System.Drawing.Point(54, 219);
      this.MySqlUsernameLabel.Name = "MySqlUsernameLabel";
      this.MySqlUsernameLabel.Size = new System.Drawing.Size(63, 15);
      this.MySqlUsernameLabel.TabIndex = 22;
      this.MySqlUsernameLabel.Text = "Username:";
      // 
      // MySqlHostNameLabel
      // 
      this.MySqlHostNameLabel.AccessibleDescription = "A label displaying the text MySQL host name";
      this.MySqlHostNameLabel.AccessibleName = "MySQL Host Name Text";
      this.MySqlHostNameLabel.AutoSize = true;
      this.MySqlHostNameLabel.Location = new System.Drawing.Point(11, 162);
      this.MySqlHostNameLabel.Name = "MySqlHostNameLabel";
      this.MySqlHostNameLabel.Size = new System.Drawing.Size(106, 15);
      this.MySqlHostNameLabel.TabIndex = 16;
      this.MySqlHostNameLabel.Text = "MySQL Hostname:";
      // 
      // HelpMySqlPortLabel
      // 
      this.HelpMySqlPortLabel.AccessibleDescription = "A label displaying help text related to the MySQL port number in the SSH paramete" +
    "rs section";
      this.HelpMySqlPortLabel.AccessibleName = "MySQL Port Number Help";
      this.HelpMySqlPortLabel.AutoSize = true;
      this.HelpMySqlPortLabel.Location = new System.Drawing.Point(505, 193);
      this.HelpMySqlPortLabel.Name = "HelpMySqlPortLabel";
      this.HelpMySqlPortLabel.Size = new System.Drawing.Size(180, 15);
      this.HelpMySqlPortLabel.TabIndex = 21;
      this.HelpMySqlPortLabel.Text = "TCP/IP port of the MySQL server.";
      // 
      // SshKeyFileButton
      // 
      this.SshKeyFileButton.AccessibleDescription = "A button to browse the file system for the SSH key file";
      this.SshKeyFileButton.AccessibleName = "SSH Key File Browse";
      this.SshKeyFileButton.Location = new System.Drawing.Point(452, 102);
      this.SshKeyFileButton.Name = "SshKeyFileButton";
      this.SshKeyFileButton.Size = new System.Drawing.Size(26, 23);
      this.SshKeyFileButton.TabIndex = 11;
      this.SshKeyFileButton.Text = "...";
      this.SshKeyFileButton.UseVisualStyleBackColor = true;
      this.SshKeyFileButton.Click += new System.EventHandler(this.SshKeyFileButton_Click);
      // 
      // HelpSshKeyFileLabel
      // 
      this.HelpSshKeyFileLabel.AccessibleDescription = "A button  to browse the file system for a private key file";
      this.HelpSshKeyFileLabel.AccessibleName = "SSH Private Key File Browse";
      this.HelpSshKeyFileLabel.AutoSize = true;
      this.HelpSshKeyFileLabel.Location = new System.Drawing.Point(505, 106);
      this.HelpSshKeyFileLabel.Name = "HelpSshKeyFileLabel";
      this.HelpSshKeyFileLabel.Size = new System.Drawing.Size(151, 15);
      this.HelpSshKeyFileLabel.TabIndex = 12;
      this.HelpSshKeyFileLabel.Text = "Path to SSH private key file.";
      // 
      // SshKeyFileLabel
      // 
      this.SshKeyFileLabel.AccessibleDescription = "A label displaying the text SSH key file";
      this.SshKeyFileLabel.AccessibleName = "SSH Key File Text";
      this.SshKeyFileLabel.AutoSize = true;
      this.SshKeyFileLabel.BackColor = System.Drawing.Color.Transparent;
      this.SshKeyFileLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.SshKeyFileLabel.Location = new System.Drawing.Point(43, 104);
      this.SshKeyFileLabel.Name = "SshKeyFileLabel";
      this.SshKeyFileLabel.Size = new System.Drawing.Size(74, 15);
      this.SshKeyFileLabel.TabIndex = 9;
      this.SshKeyFileLabel.Text = "SSH Key File:";
      // 
      // SshPasswordTextBox
      // 
      this.SshPasswordTextBox.AccessibleDescription = "A text box to input the SSH user\'s password to connect to the SSH tunnel";
      this.SshPasswordTextBox.AccessibleName = "SSH Password";
      this.SshPasswordTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ConnectionBindingSource, "SshPassword", true));
      this.SshPasswordTextBox.Location = new System.Drawing.Point(123, 73);
      this.SshPasswordTextBox.MaxLength = 679;
      this.SshPasswordTextBox.Name = "SshPasswordTextBox";
      this.SshPasswordTextBox.Size = new System.Drawing.Size(355, 23);
      this.SshPasswordTextBox.TabIndex = 7;
      this.SshPasswordTextBox.UseSystemPasswordChar = true;
      // 
      // HelpSshPasswordLabel
      // 
      this.HelpSshPasswordLabel.AccessibleDescription = "A label displaying help text related to the password for the SSH tunnel";
      this.HelpSshPasswordLabel.AccessibleName = "SSH Password Help";
      this.HelpSshPasswordLabel.AutoSize = true;
      this.HelpSshPasswordLabel.Location = new System.Drawing.Point(505, 77);
      this.HelpSshPasswordLabel.Name = "HelpSshPasswordLabel";
      this.HelpSshPasswordLabel.Size = new System.Drawing.Size(272, 15);
      this.HelpSshPasswordLabel.TabIndex = 8;
      this.HelpSshPasswordLabel.Text = "SSH user\'s password to connect to the SSH tunnel.";
      // 
      // SshPasswordLabel
      // 
      this.SshPasswordLabel.AccessibleDescription = "A label displaying the text SSH password";
      this.SshPasswordLabel.AccessibleName = "SSH Password Text";
      this.SshPasswordLabel.AutoSize = true;
      this.SshPasswordLabel.BackColor = System.Drawing.Color.Transparent;
      this.SshPasswordLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.SshPasswordLabel.Location = new System.Drawing.Point(33, 75);
      this.SshPasswordLabel.Name = "SshPasswordLabel";
      this.SshPasswordLabel.Size = new System.Drawing.Size(84, 15);
      this.SshPasswordLabel.TabIndex = 6;
      this.SshPasswordLabel.Text = "SSH Password:";
      // 
      // HelpSshUsernameLabel
      // 
      this.HelpSshUsernameLabel.AccessibleDescription = "A label displaying help text related to the user name for the SSH tunnel";
      this.HelpSshUsernameLabel.AccessibleName = "SSH User Name Help";
      this.HelpSshUsernameLabel.AutoSize = true;
      this.HelpSshUsernameLabel.Location = new System.Drawing.Point(505, 47);
      this.HelpSshUsernameLabel.Name = "HelpSshUsernameLabel";
      this.HelpSshUsernameLabel.Size = new System.Drawing.Size(225, 15);
      this.HelpSshUsernameLabel.TabIndex = 5;
      this.HelpSshUsernameLabel.Text = "Name of the SSH user on the connection.";
      // 
      // HelpSshHostPortLabel
      // 
      this.HelpSshHostPortLabel.AccessibleDescription = "A label displaying help text related to the SSH host name with optional port numb" +
    "er";
      this.HelpSshHostPortLabel.AccessibleName = "SSH Host Name And Port Help";
      this.HelpSshHostPortLabel.AutoSize = true;
      this.HelpSshHostPortLabel.Location = new System.Drawing.Point(505, 17);
      this.HelpSshHostPortLabel.Name = "HelpSshHostPortLabel";
      this.HelpSshHostPortLabel.Size = new System.Drawing.Size(267, 15);
      this.HelpSshHostPortLabel.TabIndex = 2;
      this.HelpSshHostPortLabel.Text = "SSH server hostname, with optional port number.";
      // 
      // SshUsernameLabel
      // 
      this.SshUsernameLabel.AccessibleDescription = "A label displaying the text SSH user name";
      this.SshUsernameLabel.AccessibleName = "SSH User Name Text";
      this.SshUsernameLabel.AutoSize = true;
      this.SshUsernameLabel.Location = new System.Drawing.Point(30, 46);
      this.SshUsernameLabel.Name = "SshUsernameLabel";
      this.SshUsernameLabel.Size = new System.Drawing.Size(87, 15);
      this.SshUsernameLabel.TabIndex = 3;
      this.SshUsernameLabel.Text = "SSH Username:";
      // 
      // SshHostnameLabel
      // 
      this.SshHostnameLabel.AccessibleDescription = "A label displaying the text SSH host name";
      this.SshHostnameLabel.AccessibleName = "SSH Host Name Text";
      this.SshHostnameLabel.AutoSize = true;
      this.SshHostnameLabel.Location = new System.Drawing.Point(28, 17);
      this.SshHostnameLabel.Name = "SshHostnameLabel";
      this.SshHostnameLabel.Size = new System.Drawing.Size(89, 15);
      this.SshHostnameLabel.TabIndex = 0;
      this.SshHostnameLabel.Text = "SSH Hostname:";
      // 
      // ParametersTabPage
      // 
      this.ParametersTabPage.AccessibleDescription = "A tab page containing connection parameters";
      this.ParametersTabPage.AccessibleName = "Parameters Tab";
      this.ParametersTabPage.Controls.Add(this.DefaultSchemaComboBox);
      this.ParametersTabPage.Controls.Add(this.MySqlXPortWarningLabel);
      this.ParametersTabPage.Controls.Add(this.MySqlXPortWarningPictureBox);
      this.ParametersTabPage.Controls.Add(this.PasswordTextBox);
      this.ParametersTabPage.Controls.Add(this.HelpDefaultSchemaLabel);
      this.ParametersTabPage.Controls.Add(this.DefaultSchemaLabel);
      this.ParametersTabPage.Controls.Add(this.HelpPasswordLabel);
      this.ParametersTabPage.Controls.Add(this.PasswordLabel);
      this.ParametersTabPage.Controls.Add(this.HelpUsernameLabel);
      this.ParametersTabPage.Controls.Add(this.PortTextBox);
      this.ParametersTabPage.Controls.Add(this.UsernameTextBox);
      this.ParametersTabPage.Controls.Add(this.HostNameTextBox);
      this.ParametersTabPage.Controls.Add(this.SocketTextBox);
      this.ParametersTabPage.Controls.Add(this.HelpHostPortLabel);
      this.ParametersTabPage.Controls.Add(this.PortLabel);
      this.ParametersTabPage.Controls.Add(this.UsernameLabel);
      this.ParametersTabPage.Controls.Add(this.HostnameLabel);
      this.ParametersTabPage.Controls.Add(this.SocketLabel);
      this.ParametersTabPage.Controls.Add(this.HelpSocketLabel);
      this.ParametersTabPage.Location = new System.Drawing.Point(4, 24);
      this.ParametersTabPage.Name = "ParametersTabPage";
      this.ParametersTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.ParametersTabPage.Size = new System.Drawing.Size(836, 311);
      this.ParametersTabPage.TabIndex = 0;
      this.ParametersTabPage.Text = "Parameters";
      this.ParametersTabPage.UseVisualStyleBackColor = true;
      // 
      // DefaultSchemaComboBox
      // 
      this.DefaultSchemaComboBox.AccessibleDescription = "A combo box displaying all schemas in the server to select the default one";
      this.DefaultSchemaComboBox.AccessibleName = "Default Schema";
      this.DefaultSchemaComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
      this.DefaultSchemaComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
      this.DefaultSchemaComboBox.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.ConnectionBindingSource, "DefaultSchema", true));
      this.DefaultSchemaComboBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ConnectionBindingSource, "DefaultSchema", true));
      this.DefaultSchemaComboBox.FormattingEnabled = true;
      this.DefaultSchemaComboBox.Location = new System.Drawing.Point(124, 102);
      this.DefaultSchemaComboBox.Name = "DefaultSchemaComboBox";
      this.DefaultSchemaComboBox.Size = new System.Drawing.Size(354, 23);
      this.DefaultSchemaComboBox.TabIndex = 13;
      this.DefaultSchemaComboBox.DropDown += new System.EventHandler(this.DefaultSchemaComboBox_DropDown);
      this.DefaultSchemaComboBox.TextChanged += new System.EventHandler(this.TextChangedHandler);
      this.DefaultSchemaComboBox.Validated += new System.EventHandler(this.ValidatedHandler);
      // 
      // MySqlXPortWarningLabel
      // 
      this.MySqlXPortWarningLabel.AccessibleDescription = "A label displaying a disclaimer about X Protocol connections set as TCP IP ones";
      this.MySqlXPortWarningLabel.AccessibleName = "X Protocol Warning Disclaimer Text";
      this.MySqlXPortWarningLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.MySqlXPortWarningLabel.ForeColor = System.Drawing.Color.Red;
      this.MySqlXPortWarningLabel.Location = new System.Drawing.Point(122, 140);
      this.MySqlXPortWarningLabel.Name = "MySqlXPortWarningLabel";
      this.MySqlXPortWarningLabel.Size = new System.Drawing.Size(510, 55);
      this.MySqlXPortWarningLabel.TabIndex = 15;
      this.MySqlXPortWarningLabel.Text = resources.GetString("MySqlXPortWarningLabel.Text");
      // 
      // MySqlXPortWarningPictureBox
      // 
      this.MySqlXPortWarningPictureBox.AccessibleDescription = "A picture box displaying a warning icon that goes along with the X Protocol discl" +
    "aimer";
      this.MySqlXPortWarningPictureBox.AccessibleName = "X Protocol Warning Disclaimer Icon";
      this.MySqlXPortWarningPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("MySqlXPortWarningPictureBox.Image")));
      this.MySqlXPortWarningPictureBox.Location = new System.Drawing.Point(96, 140);
      this.MySqlXPortWarningPictureBox.Name = "MySqlXPortWarningPictureBox";
      this.MySqlXPortWarningPictureBox.Size = new System.Drawing.Size(20, 20);
      this.MySqlXPortWarningPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
      this.MySqlXPortWarningPictureBox.TabIndex = 59;
      this.MySqlXPortWarningPictureBox.TabStop = false;
      // 
      // PasswordTextBox
      // 
      this.PasswordTextBox.AccessibleDescription = "A text box to input the connection password";
      this.PasswordTextBox.AccessibleName = "Password";
      this.PasswordTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ConnectionBindingSource, "Password", true));
      this.PasswordTextBox.Location = new System.Drawing.Point(124, 73);
      this.PasswordTextBox.MaxLength = 679;
      this.PasswordTextBox.Name = "PasswordTextBox";
      this.PasswordTextBox.Size = new System.Drawing.Size(354, 23);
      this.PasswordTextBox.TabIndex = 10;
      this.PasswordTextBox.UseSystemPasswordChar = true;
      // 
      // HelpDefaultSchemaLabel
      // 
      this.HelpDefaultSchemaLabel.AccessibleDescription = "A label displaying help text related to the default schema";
      this.HelpDefaultSchemaLabel.AccessibleName = "Default Schema Help";
      this.HelpDefaultSchemaLabel.AutoSize = true;
      this.HelpDefaultSchemaLabel.Location = new System.Drawing.Point(505, 105);
      this.HelpDefaultSchemaLabel.Name = "HelpDefaultSchemaLabel";
      this.HelpDefaultSchemaLabel.Size = new System.Drawing.Size(262, 15);
      this.HelpDefaultSchemaLabel.TabIndex = 14;
      this.HelpDefaultSchemaLabel.Text = "The default schema, leave blank to select it later.";
      // 
      // DefaultSchemaLabel
      // 
      this.DefaultSchemaLabel.AccessibleDescription = "A label displaying the text default schema";
      this.DefaultSchemaLabel.AccessibleName = "Default Schema Text";
      this.DefaultSchemaLabel.AutoSize = true;
      this.DefaultSchemaLabel.Location = new System.Drawing.Point(25, 105);
      this.DefaultSchemaLabel.Name = "DefaultSchemaLabel";
      this.DefaultSchemaLabel.Size = new System.Drawing.Size(93, 15);
      this.DefaultSchemaLabel.TabIndex = 12;
      this.DefaultSchemaLabel.Text = "Default Schema:";
      // 
      // HelpPasswordLabel
      // 
      this.HelpPasswordLabel.AccessibleDescription = "A label displaying help text related to the connection password";
      this.HelpPasswordLabel.AccessibleName = "Password Help";
      this.HelpPasswordLabel.AutoSize = true;
      this.HelpPasswordLabel.Location = new System.Drawing.Point(505, 76);
      this.HelpPasswordLabel.Name = "HelpPasswordLabel";
      this.HelpPasswordLabel.Size = new System.Drawing.Size(250, 15);
      this.HelpPasswordLabel.TabIndex = 11;
      this.HelpPasswordLabel.Text = "The user\'s password, stored in a secured vault.";
      // 
      // PasswordLabel
      // 
      this.PasswordLabel.AccessibleDescription = "A label displaying the text password";
      this.PasswordLabel.AccessibleName = "Password Text";
      this.PasswordLabel.AutoSize = true;
      this.PasswordLabel.BackColor = System.Drawing.Color.Transparent;
      this.PasswordLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.PasswordLabel.Location = new System.Drawing.Point(58, 76);
      this.PasswordLabel.Name = "PasswordLabel";
      this.PasswordLabel.Size = new System.Drawing.Size(60, 15);
      this.PasswordLabel.TabIndex = 9;
      this.PasswordLabel.Text = "Password:";
      // 
      // HelpUsernameLabel
      // 
      this.HelpUsernameLabel.AccessibleDescription = "A label displaying help text related to the user name to connect to the server";
      this.HelpUsernameLabel.AccessibleName = "User Name Help";
      this.HelpUsernameLabel.AutoSize = true;
      this.HelpUsernameLabel.Location = new System.Drawing.Point(505, 47);
      this.HelpUsernameLabel.Name = "HelpUsernameLabel";
      this.HelpUsernameLabel.Size = new System.Drawing.Size(201, 15);
      this.HelpUsernameLabel.TabIndex = 8;
      this.HelpUsernameLabel.Text = "Name of the user on the connection.";
      // 
      // PortTextBox
      // 
      this.PortTextBox.AccessibleDescription = "A text box to input the connection port number";
      this.PortTextBox.AccessibleName = "Port Number";
      this.PortTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ConnectionBindingSource, "Port", true));
      this.PortTextBox.Location = new System.Drawing.Point(427, 15);
      this.PortTextBox.Name = "PortTextBox";
      this.PortTextBox.Size = new System.Drawing.Size(51, 23);
      this.PortTextBox.TabIndex = 4;
      this.PortTextBox.TextChanged += new System.EventHandler(this.TextChangedHandler);
      this.PortTextBox.Validated += new System.EventHandler(this.ValidatedHandler);
      // 
      // UsernameTextBox
      // 
      this.UsernameTextBox.AccessibleDescription = "A text box to input the user name to connect to the server";
      this.UsernameTextBox.AccessibleName = "User Name";
      this.UsernameTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ConnectionBindingSource, "UserName", true));
      this.UsernameTextBox.Location = new System.Drawing.Point(124, 44);
      this.UsernameTextBox.MaxLength = 679;
      this.UsernameTextBox.Name = "UsernameTextBox";
      this.UsernameTextBox.Size = new System.Drawing.Size(354, 23);
      this.UsernameTextBox.TabIndex = 7;
      this.UsernameTextBox.TextChanged += new System.EventHandler(this.TextChangedHandler);
      this.UsernameTextBox.Validated += new System.EventHandler(this.ValidatedHandler);
      // 
      // HostNameTextBox
      // 
      this.HostNameTextBox.AccessibleDescription = "A text box to input the host name for the connection";
      this.HostNameTextBox.AccessibleName = "Host Name";
      this.HostNameTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ConnectionBindingSource, "Host", true));
      this.HostNameTextBox.Location = new System.Drawing.Point(124, 15);
      this.HostNameTextBox.MaxLength = 484;
      this.HostNameTextBox.Name = "HostNameTextBox";
      this.HostNameTextBox.Size = new System.Drawing.Size(276, 23);
      this.HostNameTextBox.TabIndex = 2;
      this.HostNameTextBox.TextChanged += new System.EventHandler(this.TextChangedHandler);
      this.HostNameTextBox.Validated += new System.EventHandler(this.ValidatedHandler);
      // 
      // SocketTextBox
      // 
      this.SocketTextBox.AccessibleDescription = "A text box to input the connection socket name";
      this.SocketTextBox.AccessibleName = "Socket";
      this.SocketTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.ConnectionBindingSource, "UnixSocketOrWindowsPipe", true));
      this.SocketTextBox.Location = new System.Drawing.Point(124, 15);
      this.SocketTextBox.MaxLength = 484;
      this.SocketTextBox.Name = "SocketTextBox";
      this.SocketTextBox.Size = new System.Drawing.Size(354, 23);
      this.SocketTextBox.TabIndex = 5;
      this.SocketTextBox.Visible = false;
      this.SocketTextBox.TextChanged += new System.EventHandler(this.TextChangedHandler);
      this.SocketTextBox.Validated += new System.EventHandler(this.ValidatedHandler);
      // 
      // HelpHostPortLabel
      // 
      this.HelpHostPortLabel.AccessibleDescription = "A label displaying help text related to the port number";
      this.HelpHostPortLabel.AccessibleName = "Port Number Help";
      this.HelpHostPortLabel.AutoSize = true;
      this.HelpHostPortLabel.Location = new System.Drawing.Point(505, 18);
      this.HelpHostPortLabel.Name = "HelpHostPortLabel";
      this.HelpHostPortLabel.Size = new System.Drawing.Size(278, 15);
      this.HelpHostPortLabel.TabIndex = 5;
      this.HelpHostPortLabel.Text = "Name or IP address of the server host - TCP/IP port.";
      // 
      // PortLabel
      // 
      this.PortLabel.AccessibleDescription = "A label displaying the text port";
      this.PortLabel.AccessibleName = "Port Text";
      this.PortLabel.AutoSize = true;
      this.PortLabel.Location = new System.Drawing.Point(320, 17);
      this.PortLabel.Name = "PortLabel";
      this.PortLabel.Size = new System.Drawing.Size(32, 15);
      this.PortLabel.TabIndex = 3;
      this.PortLabel.Text = "Port:";
      // 
      // UsernameLabel
      // 
      this.UsernameLabel.AccessibleDescription = "A label displaying the text user name";
      this.UsernameLabel.AccessibleName = "User Name Text";
      this.UsernameLabel.AutoSize = true;
      this.UsernameLabel.Location = new System.Drawing.Point(55, 47);
      this.UsernameLabel.Name = "UsernameLabel";
      this.UsernameLabel.Size = new System.Drawing.Size(63, 15);
      this.UsernameLabel.TabIndex = 6;
      this.UsernameLabel.Text = "Username:";
      // 
      // HostnameLabel
      // 
      this.HostnameLabel.AccessibleDescription = "A label displaying the text host name";
      this.HostnameLabel.AccessibleName = "Host Name Text";
      this.HostnameLabel.AutoSize = true;
      this.HostnameLabel.Location = new System.Drawing.Point(53, 18);
      this.HostnameLabel.Name = "HostnameLabel";
      this.HostnameLabel.Size = new System.Drawing.Size(65, 15);
      this.HostnameLabel.TabIndex = 1;
      this.HostnameLabel.Text = "Hostname:";
      // 
      // SocketLabel
      // 
      this.SocketLabel.AccessibleDescription = "A label displaying the text socket pipe path";
      this.SocketLabel.AccessibleName = "Socket Text";
      this.SocketLabel.AutoSize = true;
      this.SocketLabel.Location = new System.Drawing.Point(18, 18);
      this.SocketLabel.Name = "SocketLabel";
      this.SocketLabel.Size = new System.Drawing.Size(100, 15);
      this.SocketLabel.TabIndex = 0;
      this.SocketLabel.Text = "Socket/Pipe Path:";
      this.SocketLabel.Visible = false;
      // 
      // HelpSocketLabel
      // 
      this.HelpSocketLabel.AccessibleDescription = "A label displaying help text related to the connection socket name";
      this.HelpSocketLabel.AccessibleName = "Socket Help";
      this.HelpSocketLabel.AutoSize = true;
      this.HelpSocketLabel.Location = new System.Drawing.Point(505, 18);
      this.HelpSocketLabel.Name = "HelpSocketLabel";
      this.HelpSocketLabel.Size = new System.Drawing.Size(303, 15);
      this.HelpSocketLabel.TabIndex = 8;
      this.HelpSocketLabel.Text = "Path to local socket or pipe file. Leave empty for default.";
      this.HelpSocketLabel.Visible = false;
      // 
      // ParametersTabControl
      // 
      this.ParametersTabControl.AccessibleDescription = "A tabbed control to hold all the different configuration tab pages";
      this.ParametersTabControl.AccessibleName = "Parameters Tabbed Control";
      this.ParametersTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.ParametersTabControl.Controls.Add(this.ParametersTabPage);
      this.ParametersTabControl.Controls.Add(this.SshParametersTabPage);
      this.ParametersTabControl.Controls.Add(this.SslTabPage);
      this.ParametersTabControl.Controls.Add(this.AdvancedTabPage);
      this.ParametersTabControl.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ParametersTabControl.Location = new System.Drawing.Point(10, 100);
      this.ParametersTabControl.Name = "ParametersTabControl";
      this.ParametersTabControl.SelectedIndex = 0;
      this.ParametersTabControl.Size = new System.Drawing.Size(844, 339);
      this.ParametersTabControl.TabIndex = 1;
      this.ParametersTabControl.SelectedIndexChanged += new System.EventHandler(this.ParametersTabControl_SelectedIndexChanged);
      // 
      // MySqlWorkbenchConnectionDialog
      // 
      this.AcceptButton = this.OKButton;
      this.AccessibleDescription = "A modal dialog with MySQL connection information";
      this.AccessibleName = "MySQL Server Connection";
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.CancelationButton;
      this.ClientSize = new System.Drawing.Size(864, 522);
      this.CommandAreaVisible = true;
      this.FootnoteAreaHeight = 0;
      this.Name = "MySqlWorkbenchConnectionDialog";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "MySQL Server Connection";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MySqlWorkbenchConnectionDialog_FormClosing);
      this.Shown += new System.EventHandler(this.MySqlWorkbenchConnectionDialog_Shown);
      ((System.ComponentModel.ISupportInitialize)(this.ValidationsErrorProvider)).EndInit();
      this.ContentAreaPanel.ResumeLayout(false);
      this.ContentAreaPanel.PerformLayout();
      this.CommandAreaPanel.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.ConnectionBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ConnectionStatusPictureBox)).EndInit();
      this.AdvancedTabPage.ResumeLayout(false);
      this.AdvancedTabPage.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.TimeoutNumericUpDown)).EndInit();
      this.SslTabPage.ResumeLayout(false);
      this.SslTabPage.PerformLayout();
      this.SshParametersTabPage.ResumeLayout(false);
      this.SshParametersTabPage.PerformLayout();
      this.ParametersTabPage.ResumeLayout(false);
      this.ParametersTabPage.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.MySqlXPortWarningPictureBox)).EndInit();
      this.ParametersTabControl.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.Label HelpConnectionNameLabel;
    private System.Windows.Forms.ComboBox ConnectionMethodComboBox;
    private System.Windows.Forms.Label HelpConnectionMethodLabel;
    private System.Windows.Forms.Label ConnectionMethodLabel;
    private System.Windows.Forms.TextBox ConnectionNameTextBox;
    private System.Windows.Forms.Label ConnectionNameLabel;
    private System.Windows.Forms.Button CancelationButton;
    private System.Windows.Forms.Button OKButton;
    private System.Windows.Forms.Button TestConnectionButton;
    private System.Windows.Forms.BindingSource ConnectionBindingSource;
    private System.Windows.Forms.Label ConnectionStatusDescriptionLabel;
    private System.Windows.Forms.PictureBox ConnectionStatusPictureBox;
    private System.Windows.Forms.Label ConnectionStatusLabel;
    private System.Windows.Forms.ImageList StatusImageList;
    private System.Windows.Forms.TabControl ParametersTabControl;
    private System.Windows.Forms.TabPage ParametersTabPage;
    private System.Windows.Forms.ComboBox DefaultSchemaComboBox;
    private System.Windows.Forms.Label MySqlXPortWarningLabel;
    private System.Windows.Forms.PictureBox MySqlXPortWarningPictureBox;
    private System.Windows.Forms.TextBox PasswordTextBox;
    private System.Windows.Forms.Label HelpDefaultSchemaLabel;
    private System.Windows.Forms.Label DefaultSchemaLabel;
    private System.Windows.Forms.Label HelpPasswordLabel;
    private System.Windows.Forms.Label PasswordLabel;
    private System.Windows.Forms.Label HelpUsernameLabel;
    private System.Windows.Forms.TextBox PortTextBox;
    private System.Windows.Forms.TextBox UsernameTextBox;
    private System.Windows.Forms.TextBox HostNameTextBox;
    private System.Windows.Forms.TextBox SocketTextBox;
    private System.Windows.Forms.Label HelpHostPortLabel;
    private System.Windows.Forms.Label PortLabel;
    private System.Windows.Forms.Label UsernameLabel;
    private System.Windows.Forms.Label HostnameLabel;
    private System.Windows.Forms.Label SocketLabel;
    private System.Windows.Forms.Label HelpSocketLabel;
    private System.Windows.Forms.TabPage SshParametersTabPage;
    private System.Windows.Forms.TextBox MySqlPasswordTextBox;
    private System.Windows.Forms.Label HelpMySqlDefaultSchemaLabel;
    private System.Windows.Forms.TextBox MySqlPortTextBox;
    private System.Windows.Forms.TextBox MySqlUsernameTextBox;
    private System.Windows.Forms.TextBox MySqlHostNameTextBox;
    private System.Windows.Forms.TextBox SshKeyFileTextBox;
    private System.Windows.Forms.TextBox SshUsernameTextBox;
    private System.Windows.Forms.TextBox SshHostnameTextBox;
    private System.Windows.Forms.Label MySqlDefaultSchemaLabel;
    private System.Windows.Forms.Label HelpMySqlPasswordLabel;
    private System.Windows.Forms.Label MySqlPasswordLabel;
    private System.Windows.Forms.Label HelpMySqlUsernameLabel;
    private System.Windows.Forms.Label HelpMySqlHostNameLabel;
    private System.Windows.Forms.Label MySqlPortLabel;
    private System.Windows.Forms.Label MySqlUsernameLabel;
    private System.Windows.Forms.Label MySqlHostNameLabel;
    private System.Windows.Forms.Label HelpMySqlPortLabel;
    private System.Windows.Forms.Button SshKeyFileButton;
    private System.Windows.Forms.Label HelpSshKeyFileLabel;
    private System.Windows.Forms.Label SshKeyFileLabel;
    private System.Windows.Forms.TextBox SshPasswordTextBox;
    private System.Windows.Forms.Label HelpSshPasswordLabel;
    private System.Windows.Forms.Label SshPasswordLabel;
    private System.Windows.Forms.Label HelpSshUsernameLabel;
    private System.Windows.Forms.Label HelpSshHostPortLabel;
    private System.Windows.Forms.Label SshUsernameLabel;
    private System.Windows.Forms.Label SshHostnameLabel;
    private System.Windows.Forms.TabPage SslTabPage;
    private System.Windows.Forms.LinkLabel SslPemMoreInfoLinkLabel;
    private System.Windows.Forms.Button SslCertFileButton;
    private System.Windows.Forms.Button SslCaFileButton;
    private System.Windows.Forms.Button SslKeyFileButton;
    private System.Windows.Forms.TextBox SslKeyFileTextBox;
    private System.Windows.Forms.Label HelpSslKeyLabel;
    private System.Windows.Forms.Label SslKeyLabel;
    private System.Windows.Forms.TextBox SslCertFileTextBox;
    private System.Windows.Forms.Label HelpSslCertFileLabel;
    private System.Windows.Forms.Label SslCertFileLabel;
    private System.Windows.Forms.Label HelpSslCaFileLabel;
    private System.Windows.Forms.TextBox SslCaFileTextBox;
    private System.Windows.Forms.Label SslCaFileLabel;
    private System.Windows.Forms.ComboBox UseSslComboBox;
    private System.Windows.Forms.Label HelpUseSslLabel;
    private System.Windows.Forms.Label UseSslLabel;
    private System.Windows.Forms.TabPage AdvancedTabPage;
    private System.Windows.Forms.NumericUpDown TimeoutNumericUpDown;
    private System.Windows.Forms.Label TimeoutLabel;
    private System.Windows.Forms.Label HelpTimeoutLabel;
    private System.Windows.Forms.Label HelpUseCompressionLabel;
    private System.Windows.Forms.CheckBox UseCompressionCheckBox;
    private System.Windows.Forms.TextBox SshPassPhraseTextBox;
    private System.Windows.Forms.Label HelpSshPassPhraseLabel;
    private System.Windows.Forms.Label SshPassPhraseLabel;
    private System.Windows.Forms.ComboBox MySqlDefaultSchemaComboBox;
  }
}