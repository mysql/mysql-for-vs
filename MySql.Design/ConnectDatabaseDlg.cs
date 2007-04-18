// Copyright (C) 2004 MySQL AB
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 2 as published by
// the Free Software Foundation
//
// There are special exceptions to the terms and conditions of the GPL 
// as it is applied to this software. View the full text of the 
// exception in file EXCEPTIONS in the directory of this software 
// distribution.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace MySql.Design
{
	/// <summary>
	/// Summary description for CreateDatabaseDlg.
	/// </summary>
	public class ConnectDatabaseDlg : System.Windows.Forms.Form
	{
		#region Fields
		private System.Windows.Forms.Button cancelBtn;
		private System.Windows.Forms.Button okBtn;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox host;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox port;
		private System.Windows.Forms.TextBox userID;
		private System.Windows.Forms.TextBox password;
		private System.Windows.Forms.CheckBox savePassword;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox name;
		private System.Windows.Forms.ComboBox database;
		private System.Windows.Forms.Button testBtn;
		#endregion

		private string connectString;

		public ConnectDatabaseDlg(string connStr)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

//			MySqlConnectionString cs = new MySqlConnectionString();
//			cs.SetConnectionString( connStr );
//			properties.SelectedObject = cs;
		}

//		internal MySqlConnectionString GetConnectStringObject()
//		{
//			return (properties.SelectedObject as MySqlConnectionString);
//		}
//
//		public string GetConnectionString()
//		{
//			MySqlConnectionString cs = (properties.SelectedObject as MySqlConnectionString);
//			return cs.CreateConnectionString();
//		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.cancelBtn = new System.Windows.Forms.Button();
			this.okBtn = new System.Windows.Forms.Button();
			this.testBtn = new System.Windows.Forms.Button();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.name = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.database = new System.Windows.Forms.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			this.savePassword = new System.Windows.Forms.CheckBox();
			this.password = new System.Windows.Forms.TextBox();
			this.userID = new System.Windows.Forms.TextBox();
			this.port = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.host = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.SuspendLayout();
			// 
			// cancelBtn
			// 
			this.cancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelBtn.CausesValidation = false;
			this.cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelBtn.Location = new System.Drawing.Point(219, 369);
			this.cancelBtn.Name = "cancelBtn";
			this.cancelBtn.TabIndex = 11;
			this.cancelBtn.Text = "&Cancel";
			// 
			// okBtn
			// 
			this.okBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okBtn.Location = new System.Drawing.Point(138, 369);
			this.okBtn.Name = "okBtn";
			this.okBtn.TabIndex = 10;
			this.okBtn.Text = "&OK";
			this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
			// 
			// testBtn
			// 
			this.testBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.testBtn.Location = new System.Drawing.Point(164, 280);
			this.testBtn.Name = "testBtn";
			this.testBtn.Size = new System.Drawing.Size(104, 28);
			this.testBtn.TabIndex = 9;
			this.testBtn.Text = "&Test Connection";
			this.testBtn.Click += new System.EventHandler(this.testBtn_Click);
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Location = new System.Drawing.Point(4, 12);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(296, 348);
			this.tabControl1.TabIndex = 25;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.name);
			this.tabPage1.Controls.Add(this.label7);
			this.tabPage1.Controls.Add(this.database);
			this.tabPage1.Controls.Add(this.label6);
			this.tabPage1.Controls.Add(this.savePassword);
			this.tabPage1.Controls.Add(this.password);
			this.tabPage1.Controls.Add(this.userID);
			this.tabPage1.Controls.Add(this.port);
			this.tabPage1.Controls.Add(this.label5);
			this.tabPage1.Controls.Add(this.label4);
			this.tabPage1.Controls.Add(this.label3);
			this.tabPage1.Controls.Add(this.label2);
			this.tabPage1.Controls.Add(this.host);
			this.tabPage1.Controls.Add(this.label1);
			this.tabPage1.Controls.Add(this.testBtn);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Size = new System.Drawing.Size(288, 322);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Connection";
			this.tabPage1.Click += new System.EventHandler(this.tabPage1_Click);
			// 
			// name
			// 
			this.name.Location = new System.Drawing.Point(20, 248);
			this.name.Name = "name";
			this.name.Size = new System.Drawing.Size(248, 20);
			this.name.TabIndex = 13;
			this.name.Text = "";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(20, 224);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(108, 20);
			this.label7.TabIndex = 12;
			this.label7.Text = "Connection Name:";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// database
			// 
			this.database.Location = new System.Drawing.Point(92, 188);
			this.database.Name = "database";
			this.database.Size = new System.Drawing.Size(176, 21);
			this.database.TabIndex = 11;
			this.database.DropDown += new System.EventHandler(this.database_DropDown);
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(20, 188);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(60, 20);
			this.label6.TabIndex = 10;
			this.label6.Text = "Database:";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// savePassword
			// 
			this.savePassword.Location = new System.Drawing.Point(92, 152);
			this.savePassword.Name = "savePassword";
			this.savePassword.TabIndex = 9;
			this.savePassword.Text = "Save Password";
			// 
			// password
			// 
			this.password.Location = new System.Drawing.Point(92, 128);
			this.password.Name = "password";
			this.password.PasswordChar = '*';
			this.password.Size = new System.Drawing.Size(176, 20);
			this.password.TabIndex = 8;
			this.password.Text = "";
			// 
			// userID
			// 
			this.userID.Location = new System.Drawing.Point(92, 96);
			this.userID.Name = "userID";
			this.userID.Size = new System.Drawing.Size(176, 20);
			this.userID.TabIndex = 7;
			this.userID.Text = "";
			// 
			// port
			// 
			this.port.Location = new System.Drawing.Point(92, 64);
			this.port.Name = "port";
			this.port.Size = new System.Drawing.Size(88, 20);
			this.port.TabIndex = 6;
			this.port.Text = "3306";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(20, 128);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(60, 20);
			this.label5.TabIndex = 5;
			this.label5.Text = "Password:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(28, 96);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(60, 20);
			this.label4.TabIndex = 4;
			this.label4.Text = "User ID:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(28, 64);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(60, 20);
			this.label3.TabIndex = 3;
			this.label3.Text = "Port:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(32, 36);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(60, 20);
			this.label2.TabIndex = 2;
			this.label2.Text = "Host:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// host
			// 
			this.host.Location = new System.Drawing.Point(92, 36);
			this.host.Name = "host";
			this.host.Size = new System.Drawing.Size(176, 20);
			this.host.TabIndex = 1;
			this.host.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(296, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "Enter the following to connect to a MySQL server";
			// 
			// tabPage2
			// 
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Size = new System.Drawing.Size(288, 322);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Advanced";
			// 
			// ConnectDatabaseDlg
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(304, 405);
			this.ControlBox = false;
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.okBtn);
			this.Controls.Add(this.cancelBtn);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "ConnectDatabaseDlg";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Connect To MySQL Database";
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion


		private void testBtn_Click(object sender, System.EventArgs e)
		{
			connectString = GetConfig().GetConnectString(true);

			Cursor curr = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			try 
			{
				MySqlConnection conn = new MySqlConnection( connectString );
				conn.Open();
				conn.Close();
				MessageBox.Show("Connection test succeeded");
			}
			catch (Exception ex) 
			{
				MessageBox.Show("Connect test failed: " + ex.Message);
			}
			finally 
			{
				Cursor.Current = curr;
			}
		}

		private void okBtn_Click(object sender, System.EventArgs e)
		{
/*			DialogResult = DialogResult.None;
			MySqlConnectionString cs = (properties.SelectedObject as MySqlConnectionString);

			if (cs.Server == null || cs.Server.Trim().Length == 0)
			{
				MessageBox.Show("You must enter or select a server");
				return;
			}
			DialogResult = DialogResult.OK;*/
		}

		internal ServerConfig GetConfig() 
		{
			ServerConfig sc = new ServerConfig();
			sc.name = name.Text.Trim();
			sc.host = host.Text.Trim();
			sc.userId = userID.Text.Trim();
			sc.password = password.Text.Trim();
			sc.savePassword = savePassword.Checked;
			sc.port = Int32.Parse( port.Text.Trim() );
			if (database.SelectedIndex >= 0)
				sc.database = database.SelectedText;
			return sc;
		}

		private void tabPage1_Click(object sender, System.EventArgs e)
		{
		
		}

		private void database_DropDown(object sender, System.EventArgs e)
		{
			string connString = GetConfig().GetConnectString(false);
			MySqlConnection conn = new MySqlConnection(connString);
			MySqlDataReader reader = null;

			database.Items.Clear();
			try 
			{
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("SHOW DATABASES", conn);
				reader = cmd.ExecuteReader();
				while (reader.Read()) 
				{
					database.Items.Add( reader.GetString(0) );
				}
			}
			catch (Exception) 
			{
				MessageBox.Show("Error retrieving database list");
			}
			finally 
			{
				if (reader != null) reader.Close();
				conn.Close();
			}
		}

	}
}
