namespace MySql.VSTools
{
    partial class ConnectionDlg
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.generalTab = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.compress = new System.Windows.Forms.CheckBox();
            this.timeout = new System.Windows.Forms.NumericUpDown();
            this.label14 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.pipeName = new System.Windows.Forms.TextBox();
            this.host = new System.Windows.Forms.TextBox();
            this.socketNameLabel = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.connectionType = new System.Windows.Forms.ComboBox();
            this.connectionName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.authenticationTab = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.useSSL = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.database = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.savePassword = new System.Windows.Forms.CheckBox();
            this.password = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.user = new System.Windows.Forms.TextBox();
            this.advancedTab = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.characterSet = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.driverType = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.connectLifetime = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.minPoolSize = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.maxPoolSize = new System.Windows.Forms.NumericUpDown();
            this.enablePooling = new System.Windows.Forms.CheckBox();
            this.cancelbtn = new System.Windows.Forms.Button();
            this.okbtn = new System.Windows.Forms.Button();
            this.testbtn = new System.Windows.Forms.Button();
            this.port = new System.Windows.Forms.NumericUpDown();
            this.tabControl1.SuspendLayout();
            this.generalTab.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timeout)).BeginInit();
            this.authenticationTab.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.advancedTab.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.connectLifetime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minPoolSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxPoolSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.port)).BeginInit();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(57, 10);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(178, 20);
            this.textBox1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "label2";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.generalTab);
            this.tabControl1.Controls.Add(this.authenticationTab);
            this.tabControl1.Controls.Add(this.advancedTab);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControl1.Location = new System.Drawing.Point(5, 5);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(353, 307);
            this.tabControl1.TabIndex = 0;
            // 
            // generalTab
            // 
            this.generalTab.Controls.Add(this.groupBox1);
            this.generalTab.Controls.Add(this.connectionName);
            this.generalTab.Controls.Add(this.label3);
            this.generalTab.Location = new System.Drawing.Point(4, 22);
            this.generalTab.Name = "generalTab";
            this.generalTab.Padding = new System.Windows.Forms.Padding(6);
            this.generalTab.Size = new System.Drawing.Size(345, 281);
            this.generalTab.TabIndex = 0;
            this.generalTab.Text = "General";
            this.generalTab.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.port);
            this.groupBox1.Controls.Add(this.compress);
            this.groupBox1.Controls.Add(this.timeout);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.pipeName);
            this.groupBox1.Controls.Add(this.host);
            this.groupBox1.Controls.Add(this.socketNameLabel);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.connectionType);
            this.groupBox1.Location = new System.Drawing.Point(9, 53);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(6);
            this.groupBox1.Size = new System.Drawing.Size(323, 161);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Connection Information";
            // 
            // compress
            // 
            this.compress.AutoSize = true;
            this.compress.Location = new System.Drawing.Point(104, 128);
            this.compress.Name = "compress";
            this.compress.Size = new System.Drawing.Size(127, 17);
            this.compress.TabIndex = 10;
            this.compress.Text = "Enable compression?";
            this.compress.UseVisualStyleBackColor = true;
            // 
            // timeout
            // 
            this.timeout.Location = new System.Drawing.Point(104, 101);
            this.timeout.Name = "timeout";
            this.timeout.Size = new System.Drawing.Size(99, 20);
            this.timeout.TabIndex = 9;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(9, 103);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(48, 13);
            this.label14.TabIndex = 8;
            this.label14.Text = "Timeout:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Host:";
            // 
            // pipeName
            // 
            this.pipeName.Location = new System.Drawing.Point(104, 75);
            this.pipeName.Name = "pipeName";
            this.pipeName.Size = new System.Drawing.Size(210, 20);
            this.pipeName.TabIndex = 7;
            this.pipeName.Visible = false;
            // 
            // host
            // 
            this.host.Location = new System.Drawing.Point(104, 22);
            this.host.Name = "host";
            this.host.Size = new System.Drawing.Size(210, 20);
            this.host.TabIndex = 5;
            // 
            // socketNameLabel
            // 
            this.socketNameLabel.AutoSize = true;
            this.socketNameLabel.Location = new System.Drawing.Point(9, 78);
            this.socketNameLabel.Name = "socketNameLabel";
            this.socketNameLabel.Size = new System.Drawing.Size(29, 13);
            this.socketNameLabel.TabIndex = 6;
            this.socketNameLabel.Text = "Port:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 51);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(34, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Type:";
            // 
            // connectionType
            // 
            this.connectionType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.connectionType.FormattingEnabled = true;
            this.connectionType.Items.AddRange(new object[] {
            "TCP/IP",
            "Shared memory",
            "Named pipe",
            "Unix socket"});
            this.connectionType.Location = new System.Drawing.Point(104, 48);
            this.connectionType.Name = "connectionType";
            this.connectionType.Size = new System.Drawing.Size(141, 21);
            this.connectionType.TabIndex = 4;
            this.connectionType.SelectionChangeCommitted += new System.EventHandler(this.connectionType_SelectionChangeCommitted);
            // 
            // connectionName
            // 
            this.connectionName.Location = new System.Drawing.Point(113, 14);
            this.connectionName.Name = "connectionName";
            this.connectionName.Size = new System.Drawing.Size(210, 20);
            this.connectionName.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Connection Name:";
            // 
            // authenticationTab
            // 
            this.authenticationTab.Controls.Add(this.groupBox5);
            this.authenticationTab.Controls.Add(this.groupBox2);
            this.authenticationTab.Location = new System.Drawing.Point(4, 22);
            this.authenticationTab.Name = "authenticationTab";
            this.authenticationTab.Padding = new System.Windows.Forms.Padding(6);
            this.authenticationTab.Size = new System.Drawing.Size(345, 281);
            this.authenticationTab.TabIndex = 2;
            this.authenticationTab.Text = "Authentication";
            this.authenticationTab.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.useSSL);
            this.groupBox5.Location = new System.Drawing.Point(10, 147);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(6);
            this.groupBox5.Size = new System.Drawing.Size(321, 100);
            this.groupBox5.TabIndex = 11;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "SSL";
            // 
            // useSSL
            // 
            this.useSSL.AutoSize = true;
            this.useSSL.Enabled = false;
            this.useSSL.Location = new System.Drawing.Point(9, 22);
            this.useSSL.Name = "useSSL";
            this.useSSL.Size = new System.Drawing.Size(82, 17);
            this.useSSL.TabIndex = 12;
            this.useSSL.Text = "Enable SSL";
            this.useSSL.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.database);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.savePassword);
            this.groupBox2.Controls.Add(this.password);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.user);
            this.groupBox2.Location = new System.Drawing.Point(9, 9);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(6);
            this.groupBox2.Size = new System.Drawing.Size(322, 131);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Credentials";
            // 
            // database
            // 
            this.database.FormattingEnabled = true;
            this.database.Location = new System.Drawing.Point(103, 97);
            this.database.Name = "database";
            this.database.Size = new System.Drawing.Size(210, 21);
            this.database.TabIndex = 6;
            this.database.DropDown += new System.EventHandler(this.database_DropDown);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(10, 100);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(56, 13);
            this.label9.TabIndex = 5;
            this.label9.Text = "Database:";
            // 
            // savePassword
            // 
            this.savePassword.AutoSize = true;
            this.savePassword.Location = new System.Drawing.Point(103, 73);
            this.savePassword.Name = "savePassword";
            this.savePassword.Size = new System.Drawing.Size(111, 17);
            this.savePassword.TabIndex = 4;
            this.savePassword.Text = "Persist password?";
            this.savePassword.UseVisualStyleBackColor = true;
            // 
            // password
            // 
            this.password.Location = new System.Drawing.Point(103, 46);
            this.password.Name = "password";
            this.password.Size = new System.Drawing.Size(210, 20);
            this.password.TabIndex = 3;
            this.password.UseSystemPasswordChar = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(10, 49);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(56, 13);
            this.label8.TabIndex = 2;
            this.label8.Text = "Password:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 23);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(32, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "User:";
            // 
            // user
            // 
            this.user.Location = new System.Drawing.Point(103, 20);
            this.user.Name = "user";
            this.user.Size = new System.Drawing.Size(210, 20);
            this.user.TabIndex = 0;
            // 
            // advancedTab
            // 
            this.advancedTab.Controls.Add(this.groupBox4);
            this.advancedTab.Controls.Add(this.groupBox3);
            this.advancedTab.Location = new System.Drawing.Point(4, 22);
            this.advancedTab.Name = "advancedTab";
            this.advancedTab.Padding = new System.Windows.Forms.Padding(6);
            this.advancedTab.Size = new System.Drawing.Size(345, 281);
            this.advancedTab.TabIndex = 1;
            this.advancedTab.Text = "Advanced";
            this.advancedTab.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.characterSet);
            this.groupBox4.Controls.Add(this.label13);
            this.groupBox4.Controls.Add(this.driverType);
            this.groupBox4.Controls.Add(this.label12);
            this.groupBox4.Location = new System.Drawing.Point(10, 147);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(6);
            this.groupBox4.Size = new System.Drawing.Size(322, 87);
            this.groupBox4.TabIndex = 1;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Other";
            // 
            // characterSet
            // 
            this.characterSet.FormattingEnabled = true;
            this.characterSet.Location = new System.Drawing.Point(93, 49);
            this.characterSet.Name = "characterSet";
            this.characterSet.Size = new System.Drawing.Size(220, 21);
            this.characterSet.TabIndex = 3;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(9, 52);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(73, 13);
            this.label13.TabIndex = 2;
            this.label13.Text = "Character set:";
            // 
            // driverType
            // 
            this.driverType.FormattingEnabled = true;
            this.driverType.Items.AddRange(new object[] {
            "Native",
            "Client",
            "Embedded"});
            this.driverType.Location = new System.Drawing.Point(93, 22);
            this.driverType.Name = "driverType";
            this.driverType.Size = new System.Drawing.Size(220, 21);
            this.driverType.TabIndex = 1;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(9, 25);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(65, 13);
            this.label12.TabIndex = 0;
            this.label12.Text = "Driver Type:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.connectLifetime);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.minPoolSize);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.maxPoolSize);
            this.groupBox3.Controls.Add(this.enablePooling);
            this.groupBox3.Location = new System.Drawing.Point(9, 9);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(6);
            this.groupBox3.Size = new System.Drawing.Size(323, 131);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Connection Pooling";
            // 
            // connectLifetime
            // 
            this.connectLifetime.Location = new System.Drawing.Point(168, 97);
            this.connectLifetime.Name = "connectLifetime";
            this.connectLifetime.Size = new System.Drawing.Size(102, 20);
            this.connectLifetime.TabIndex = 6;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(7, 99);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(152, 13);
            this.label11.TabIndex = 5;
            this.label11.Text = "Connection Lifetime (seconds):";
            // 
            // minPoolSize
            // 
            this.minPoolSize.Location = new System.Drawing.Point(168, 71);
            this.minPoolSize.Name = "minPoolSize";
            this.minPoolSize.Size = new System.Drawing.Size(102, 20);
            this.minPoolSize.TabIndex = 4;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(10, 73);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(74, 13);
            this.label10.TabIndex = 3;
            this.label10.Text = "Min Pool Size:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 47);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Max Pool Size:";
            // 
            // maxPoolSize
            // 
            this.maxPoolSize.Location = new System.Drawing.Point(168, 45);
            this.maxPoolSize.Name = "maxPoolSize";
            this.maxPoolSize.Size = new System.Drawing.Size(102, 20);
            this.maxPoolSize.TabIndex = 1;
            // 
            // enablePooling
            // 
            this.enablePooling.AutoSize = true;
            this.enablePooling.Location = new System.Drawing.Point(10, 23);
            this.enablePooling.Name = "enablePooling";
            this.enablePooling.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.enablePooling.Size = new System.Drawing.Size(154, 17);
            this.enablePooling.TabIndex = 0;
            this.enablePooling.Text = "Enable Connection Pooling";
            this.enablePooling.UseVisualStyleBackColor = true;
            // 
            // cancelbtn
            // 
            this.cancelbtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelbtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelbtn.Location = new System.Drawing.Point(280, 323);
            this.cancelbtn.Name = "cancelbtn";
            this.cancelbtn.Size = new System.Drawing.Size(75, 23);
            this.cancelbtn.TabIndex = 1;
            this.cancelbtn.Text = "Cancel";
            this.cancelbtn.UseVisualStyleBackColor = true;
            // 
            // okbtn
            // 
            this.okbtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okbtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okbtn.Location = new System.Drawing.Point(199, 323);
            this.okbtn.Name = "okbtn";
            this.okbtn.Size = new System.Drawing.Size(75, 23);
            this.okbtn.TabIndex = 2;
            this.okbtn.Text = "OK";
            this.okbtn.UseVisualStyleBackColor = true;
            // 
            // testbtn
            // 
            this.testbtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.testbtn.Location = new System.Drawing.Point(8, 323);
            this.testbtn.Name = "testbtn";
            this.testbtn.Size = new System.Drawing.Size(95, 23);
            this.testbtn.TabIndex = 3;
            this.testbtn.Text = "Test Connection";
            this.testbtn.UseVisualStyleBackColor = true;
            this.testbtn.Click += new System.EventHandler(this.testbtn_Click);
            // 
            // port
            // 
            this.port.Location = new System.Drawing.Point(104, 75);
            this.port.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.port.Name = "port";
            this.port.Size = new System.Drawing.Size(99, 20);
            this.port.TabIndex = 11;
            // 
            // ConnectionDlg
            // 
            this.AcceptButton = this.okbtn;
            this.CancelButton = this.cancelbtn;
            this.ClientSize = new System.Drawing.Size(363, 354);
            this.Controls.Add(this.testbtn);
            this.Controls.Add(this.okbtn);
            this.Controls.Add(this.cancelbtn);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ConnectionDlg";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Text = "Connect to MySQL...";
            this.tabControl1.ResumeLayout(false);
            this.generalTab.ResumeLayout(false);
            this.generalTab.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timeout)).EndInit();
            this.authenticationTab.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.advancedTab.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.connectLifetime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minPoolSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxPoolSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.port)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage generalTab;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox connectionName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TabPage advancedTab;
        private System.Windows.Forms.TextBox pipeName;
        private System.Windows.Forms.Label socketNameLabel;
        private System.Windows.Forms.TextBox host;
        private System.Windows.Forms.ComboBox connectionType;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button cancelbtn;
        private System.Windows.Forms.Button okbtn;
        private System.Windows.Forms.Button testbtn;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox enablePooling;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.NumericUpDown connectLifetime;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown minPoolSize;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown maxPoolSize;
        private System.Windows.Forms.ComboBox driverType;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.NumericUpDown timeout;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ComboBox characterSet;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.CheckBox compress;
        private System.Windows.Forms.TabPage authenticationTab;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckBox useSSL;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox database;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox savePassword;
        private System.Windows.Forms.TextBox password;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox user;
        private System.Windows.Forms.NumericUpDown port;
    }
}