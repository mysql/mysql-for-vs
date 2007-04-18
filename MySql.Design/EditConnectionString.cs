// Copyright (C) 2004 MySQL AB
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace MySql.Data.MySqlClient.Design
{
	/// <summary>
	/// Summary description for EditConnectionString.
	/// </summary>
	internal class EditConnectionString : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textServer;
		private System.Windows.Forms.TextBox textUser;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textPassword;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textPort;
		private System.Windows.Forms.CheckBox useCompression;
		private System.Windows.Forms.TextBox database;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public EditConnectionString()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		public string ConnectionString;

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
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.textServer = new System.Windows.Forms.TextBox();
			this.textUser = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textPassword = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.textPort = new System.Windows.Forms.TextBox();
			this.useCompression = new System.Windows.Forms.CheckBox();
			this.database = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(67, 200);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.TabIndex = 6;
			this.buttonOK.Text = "&OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(187, 200);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.TabIndex = 7;
			this.buttonCancel.Text = "&Cancel";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(104, 16);
			this.label1.TabIndex = 1;
			this.label1.Text = "Mysql server name:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textServer
			// 
			this.textServer.Location = new System.Drawing.Point(112, 20);
			this.textServer.Name = "textServer";
			this.textServer.Size = new System.Drawing.Size(168, 20);
			this.textServer.TabIndex = 0;
			this.textServer.Text = "";
			// 
			// textUser
			// 
			this.textUser.Location = new System.Drawing.Point(112, 76);
			this.textUser.Name = "textUser";
			this.textUser.Size = new System.Drawing.Size(168, 20);
			this.textUser.TabIndex = 2;
			this.textUser.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(68, 76);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(40, 16);
			this.label2.TabIndex = 1;
			this.label2.Text = "User:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPassword
			// 
			this.textPassword.Location = new System.Drawing.Point(112, 108);
			this.textPassword.Name = "textPassword";
			this.textPassword.PasswordChar = '*';
			this.textPassword.Size = new System.Drawing.Size(168, 20);
			this.textPassword.TabIndex = 3;
			this.textPassword.Text = "";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(44, 108);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(64, 16);
			this.label3.TabIndex = 1;
			this.label3.Text = "Password:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(4, 164);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(104, 16);
			this.label4.TabIndex = 1;
			this.label4.Text = "Select a database:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(72, 52);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(36, 16);
			this.label5.TabIndex = 1;
			this.label5.Text = "Port:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPort
			// 
			this.textPort.Location = new System.Drawing.Point(112, 48);
			this.textPort.Name = "textPort";
			this.textPort.Size = new System.Drawing.Size(168, 20);
			this.textPort.TabIndex = 1;
			this.textPort.Text = "";
			// 
			// useCompression
			// 
			this.useCompression.Location = new System.Drawing.Point(10, 132);
			this.useCompression.Name = "useCompression";
			this.useCompression.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.useCompression.Size = new System.Drawing.Size(116, 24);
			this.useCompression.TabIndex = 4;
			this.useCompression.Text = "Use compression";
			this.useCompression.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// database
			// 
			this.database.Location = new System.Drawing.Point(112, 160);
			this.database.Name = "database";
			this.database.Size = new System.Drawing.Size(168, 20);
			this.database.TabIndex = 5;
			this.database.Text = "";
			// 
			// EditConnectionString
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(298, 239);
			this.Controls.Add(this.database);
			this.Controls.Add(this.useCompression);
			this.Controls.Add(this.textServer);
			this.Controls.Add(this.textUser);
			this.Controls.Add(this.textPassword);
			this.Controls.Add(this.textPort);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label5);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "EditConnectionString";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Edit Connection String";
			this.Load += new System.EventHandler(this.EditConnectionString_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void EditConnectionString_Load(object sender, System.EventArgs e)
		{
			MySqlConnection conn = new MySqlConnection( ConnectionString );
/*			textServer.Text = conn.Server;
			MySqlConnectionString str = new MySqlConnectionString( ConnectionString );
			textServer.Text = str.Host;
			textUser.Text = str.Username;
			textPassword.Text = str.Password;
			textPort.Text = str.Port.ToString();
			useCompression.Checked = str.UseCompression;
			database.Text = str.Database;*/
		}

		string GetString()
		{
			System.Text.StringBuilder s;
			s = new System.Text.StringBuilder();
			s.AppendFormat("server={0};port={1};uid={2};pwd={3};use compression={4};database={5}",
				textServer.Text, textPort.Text, textUser.Text, textPassword.Text,
				useCompression.Checked ? true : false, database.Text );
			return s.ToString();
		}

/*		private void comboDatabase_DropDown(object sender, System.EventArgs e)
		{
			comboDatabase.Items.Clear();
			comboDatabase.SelectedIndex = -1;
		
			try
			{
				MySqlConnection conn = new MySqlConnection(GetString());
				conn.Open();
				MySqlCommand comm = new MySqlCommand("show databases",conn);
				MySqlDataReader r = (MySqlDataReader)comm.ExecuteReader();
				while(r.Read())
				{
					comboDatabase.Items.Add(r[0]);
				}
				r.Close();
				conn.Close();
			}
			catch(MySqlException)
			{
			}
		}*/

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			ConnectionString = GetString();
			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
