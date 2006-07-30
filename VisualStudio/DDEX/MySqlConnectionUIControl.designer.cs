namespace MySql.Data.VisualStudio
{
	partial class MySqlConnectionUIControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MySqlConnectionUIControl));
            this.serverNameLabel = new System.Windows.Forms.Label();
            this.serverName = new System.Windows.Forms.TextBox();
            this.loginDetailsGroupBox = new System.Windows.Forms.GroupBox();
            this.loginTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.userNameLabel = new System.Windows.Forms.Label();
            this.userName = new System.Windows.Forms.TextBox();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.password = new System.Windows.Forms.TextBox();
            this.savePassword = new System.Windows.Forms.CheckBox();
            this.databaseNameLabel = new System.Windows.Forms.Label();
            this.databaseName = new System.Windows.Forms.TextBox();
            this.loginDetailsGroupBox.SuspendLayout();
            this.loginTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // serverNameLabel
            // 
            resources.ApplyResources(this.serverNameLabel, "serverNameLabel");
            this.serverNameLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.serverNameLabel.Name = "serverNameLabel";
            // 
            // serverName
            // 
            resources.ApplyResources(this.serverName, "serverName");
            this.serverName.Name = "serverName";
            this.serverName.Leave += new System.EventHandler(this.TrimControlText);
            this.serverName.TextChanged += new System.EventHandler(this.SetProperty);
            // 
            // loginDetailsGroupBox
            // 
            resources.ApplyResources(this.loginDetailsGroupBox, "loginDetailsGroupBox");
            this.loginDetailsGroupBox.Controls.Add(this.loginTableLayoutPanel);
            this.loginDetailsGroupBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.loginDetailsGroupBox.Name = "loginDetailsGroupBox";
            this.loginDetailsGroupBox.TabStop = false;
            // 
            // loginTableLayoutPanel
            // 
            resources.ApplyResources(this.loginTableLayoutPanel, "loginTableLayoutPanel");
            this.loginTableLayoutPanel.Controls.Add(this.userNameLabel, 0, 0);
            this.loginTableLayoutPanel.Controls.Add(this.userName, 1, 0);
            this.loginTableLayoutPanel.Controls.Add(this.passwordLabel, 0, 1);
            this.loginTableLayoutPanel.Controls.Add(this.password, 1, 1);
            this.loginTableLayoutPanel.Controls.Add(this.savePassword, 1, 2);
            this.loginTableLayoutPanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.loginTableLayoutPanel.Name = "loginTableLayoutPanel";
            // 
            // userNameLabel
            // 
            resources.ApplyResources(this.userNameLabel, "userNameLabel");
            this.userNameLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.userNameLabel.Name = "userNameLabel";
            // 
            // userName
            // 
            resources.ApplyResources(this.userName, "userName");
            this.userName.Name = "userName";
            this.userName.Leave += new System.EventHandler(this.TrimControlText);
            this.userName.TextChanged += new System.EventHandler(this.SetProperty);
            // 
            // passwordLabel
            // 
            resources.ApplyResources(this.passwordLabel, "passwordLabel");
            this.passwordLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.passwordLabel.Name = "passwordLabel";
            // 
            // password
            // 
            resources.ApplyResources(this.password, "password");
            this.password.Name = "password";
            this.password.UseSystemPasswordChar = true;
            this.password.TextChanged += new System.EventHandler(this.SetProperty);
            // 
            // savePassword
            // 
            resources.ApplyResources(this.savePassword, "savePassword");
            this.savePassword.Name = "savePassword";
            this.savePassword.CheckedChanged += new System.EventHandler(this.SetProperty);
            // 
            // databaseNameLabel
            // 
            resources.ApplyResources(this.databaseNameLabel, "databaseNameLabel");
            this.databaseNameLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.databaseNameLabel.Name = "databaseNameLabel";
            // 
            // databaseName
            // 
            resources.ApplyResources(this.databaseName, "databaseName");
            this.databaseName.Name = "databaseName";
            this.databaseName.Leave += new System.EventHandler(this.TrimControlText);
            this.databaseName.TextChanged += new System.EventHandler(this.SetProperty);
            // 
            // MySqlConnectionUIControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.databaseName);
            this.Controls.Add(this.databaseNameLabel);
            this.Controls.Add(this.loginDetailsGroupBox);
            this.Controls.Add(this.serverName);
            this.Controls.Add(this.serverNameLabel);
            this.Name = "MySqlConnectionUIControl";
            this.loginDetailsGroupBox.ResumeLayout(false);
            this.loginDetailsGroupBox.PerformLayout();
            this.loginTableLayoutPanel.ResumeLayout(false);
            this.loginTableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label serverNameLabel;
		private System.Windows.Forms.TextBox serverName;
        private System.Windows.Forms.GroupBox loginDetailsGroupBox;
		private System.Windows.Forms.TableLayoutPanel loginTableLayoutPanel;
		private System.Windows.Forms.Label userNameLabel;
        private System.Windows.Forms.TextBox userName;
		private System.Windows.Forms.Label passwordLabel;
		private System.Windows.Forms.TextBox password;
		private System.Windows.Forms.CheckBox savePassword;
		private System.Windows.Forms.Label databaseNameLabel;
		private System.Windows.Forms.TextBox databaseName;
	}
}
