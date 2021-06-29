namespace MySql.Data.VisualStudio
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectDialog));
      this.label1 = new System.Windows.Forms.Label();
      this.serverName = new System.Windows.Forms.TextBox();
      this.database = new System.Windows.Forms.ComboBox();
      this.label5 = new System.Windows.Forms.Label();
      this.password = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.userId = new System.Windows.Forms.TextBox();
      this.cancelButton = new System.Windows.Forms.Button();
      this.connectButton = new System.Windows.Forms.Button();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.advancedButton = new System.Windows.Forms.Button();
      this.label4 = new System.Windows.Forms.Label();
      this.simplePanel = new System.Windows.Forms.Panel();
      this.txtPort = new System.Windows.Forms.TextBox();
      this.label6 = new System.Windows.Forms.Label();
      this.btnRefresh = new System.Windows.Forms.Button();
      this.connectionProperties = new System.Windows.Forms.PropertyGrid();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.simplePanel.SuspendLayout();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(16, 42);
      this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(130, 25);
      this.label1.TabIndex = 0;
      this.label1.Text = "Server name:";
      // 
      // serverName
      // 
      this.serverName.Location = new System.Drawing.Point(194, 39);
      this.serverName.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
      this.serverName.Name = "serverName";
      this.serverName.Size = new System.Drawing.Size(406, 29);
      this.serverName.TabIndex = 1;
      this.serverName.Text = "localhost";
      this.serverName.Leave += new System.EventHandler(this.serverName_Leave);
      // 
      // database
      // 
      this.database.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
      this.database.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
      this.database.FormattingEnabled = true;
      this.database.Location = new System.Drawing.Point(194, 190);
      this.database.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
      this.database.Name = "database";
      this.database.Size = new System.Drawing.Size(604, 32);
      this.database.TabIndex = 4;
      this.database.DropDown += new System.EventHandler(this.database_DropDown);
      this.database.Leave += new System.EventHandler(this.database_Leave);
      // 
      // label5
      // 
      this.label5.Location = new System.Drawing.Point(16, 194);
      this.label5.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(166, 56);
      this.label5.TabIndex = 8;
      this.label5.Text = "Select or enter a database name:";
      // 
      // password
      // 
      this.password.Location = new System.Drawing.Point(194, 135);
      this.password.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
      this.password.Name = "password";
      this.password.PasswordChar = '*';
      this.password.Size = new System.Drawing.Size(313, 29);
      this.password.TabIndex = 3;
      this.password.Leave += new System.EventHandler(this.password_Leave);
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(16, 90);
      this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(80, 25);
      this.label2.TabIndex = 2;
      this.label2.Text = "User Id:";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(16, 138);
      this.label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(104, 25);
      this.label3.TabIndex = 4;
      this.label3.Text = "Password:";
      // 
      // userId
      // 
      this.userId.Location = new System.Drawing.Point(194, 87);
      this.userId.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
      this.userId.Name = "userId";
      this.userId.Size = new System.Drawing.Size(406, 29);
      this.userId.TabIndex = 2;
      this.userId.Leave += new System.EventHandler(this.userId_Leave);
      // 
      // cancelButton
      // 
      this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cancelButton.Location = new System.Drawing.Point(450, 466);
      this.cancelButton.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new System.Drawing.Size(165, 46);
      this.cancelButton.TabIndex = 6;
      this.cancelButton.Text = "Cancel";
      this.cancelButton.UseVisualStyleBackColor = true;
      // 
      // connectButton
      // 
      this.connectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.connectButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.connectButton.Location = new System.Drawing.Point(265, 466);
      this.connectButton.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
      this.connectButton.Name = "connectButton";
      this.connectButton.Size = new System.Drawing.Size(165, 46);
      this.connectButton.TabIndex = 5;
      this.connectButton.Text = "Connect";
      this.connectButton.UseVisualStyleBackColor = true;
      this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
      // 
      // pictureBox1
      // 
      this.pictureBox1.BackColor = System.Drawing.Color.White;
      this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Top;
      this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
      this.pictureBox1.Location = new System.Drawing.Point(0, 0);
      this.pictureBox1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(825, 166);
      this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
      this.pictureBox1.TabIndex = 5;
      this.pictureBox1.TabStop = false;
      // 
      // advancedButton
      // 
      this.advancedButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.advancedButton.Location = new System.Drawing.Point(632, 466);
      this.advancedButton.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
      this.advancedButton.Name = "advancedButton";
      this.advancedButton.Size = new System.Drawing.Size(165, 46);
      this.advancedButton.TabIndex = 7;
      this.advancedButton.Text = "Advanced >>";
      this.advancedButton.UseVisualStyleBackColor = true;
      this.advancedButton.Click += new System.EventHandler(this.advancedButton_Click);
      // 
      // label4
      // 
      this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.label4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.label4.Location = new System.Drawing.Point(32, 450);
      this.label4.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(777, 3);
      this.label4.TabIndex = 12;
      // 
      // simplePanel
      // 
      this.simplePanel.Controls.Add(this.txtPort);
      this.simplePanel.Controls.Add(this.label6);
      this.simplePanel.Controls.Add(this.btnRefresh);
      this.simplePanel.Controls.Add(this.database);
      this.simplePanel.Controls.Add(this.userId);
      this.simplePanel.Controls.Add(this.label3);
      this.simplePanel.Controls.Add(this.label2);
      this.simplePanel.Controls.Add(this.label5);
      this.simplePanel.Controls.Add(this.password);
      this.simplePanel.Controls.Add(this.label1);
      this.simplePanel.Controls.Add(this.serverName);
      this.simplePanel.Location = new System.Drawing.Point(0, 166);
      this.simplePanel.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
      this.simplePanel.Name = "simplePanel";
      this.simplePanel.Size = new System.Drawing.Size(825, 280);
      this.simplePanel.TabIndex = 0;
      // 
      // txtPort
      // 
      this.txtPort.Location = new System.Drawing.Point(590, 136);
      this.txtPort.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
      this.txtPort.Name = "txtPort";
      this.txtPort.Size = new System.Drawing.Size(208, 29);
      this.txtPort.TabIndex = 4;
      this.txtPort.Text = "3306";
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(522, 144);
      this.label6.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(53, 25);
      this.label6.TabIndex = 17;
      this.label6.Text = "Port:";
      // 
      // btnRefresh
      // 
      this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnRefresh.Location = new System.Drawing.Point(632, 26);
      this.btnRefresh.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
      this.btnRefresh.Name = "btnRefresh";
      this.btnRefresh.Size = new System.Drawing.Size(165, 46);
      this.btnRefresh.TabIndex = 16;
      this.btnRefresh.Text = "Refresh";
      this.btnRefresh.UseVisualStyleBackColor = true;
      // 
      // connectionProperties
      // 
      this.connectionProperties.Location = new System.Drawing.Point(23, 166);
      this.connectionProperties.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
      this.connectionProperties.Name = "connectionProperties";
      this.connectionProperties.Size = new System.Drawing.Size(781, 580);
      this.connectionProperties.TabIndex = 14;
      this.connectionProperties.Visible = false;
      // 
      // ConnectDialog
      // 
      this.AcceptButton = this.connectButton;
      this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.cancelButton;
      this.ClientSize = new System.Drawing.Size(825, 544);
      this.Controls.Add(this.simplePanel);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.advancedButton);
      this.Controls.Add(this.pictureBox1);
      this.Controls.Add(this.connectButton);
      this.Controls.Add(this.cancelButton);
      this.Controls.Add(this.connectionProperties);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Margin = new System.Windows.Forms.Padding(7, 9, 7, 9);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ConnectDialog";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Connect to MySQL";
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.simplePanel.ResumeLayout(false);
      this.simplePanel.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox serverName;
    private System.Windows.Forms.Button cancelButton;
    private System.Windows.Forms.Button connectButton;
    private System.Windows.Forms.TextBox password;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox userId;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.ComboBox database;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Button advancedButton;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Panel simplePanel;
    private System.Windows.Forms.PropertyGrid connectionProperties;
    private System.Windows.Forms.Button btnRefresh;
    private System.Windows.Forms.TextBox txtPort;
    private System.Windows.Forms.Label label6;
  }
}