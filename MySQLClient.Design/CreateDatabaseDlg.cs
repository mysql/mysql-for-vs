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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MySql.Data.MySqlClient
{
	/// <summary>
	/// Summary description for CreateDatabaseDlg.
	/// </summary>
	public class CreateDatabaseDlg : System.Windows.Forms.Form
	{
		#region Fields
		private System.Windows.Forms.ComboBox serverList;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox dbName;
		private System.Windows.Forms.Button helpBtn;
		private System.Windows.Forms.Button cancelBtn;
		private System.Windows.Forms.Button okBtn;
		private System.Windows.Forms.TextBox password;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox userid;
		private System.Windows.Forms.Label label3;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		#endregion

		public CreateDatabaseDlg()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

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
			this.serverList = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.dbName = new System.Windows.Forms.TextBox();
			this.helpBtn = new System.Windows.Forms.Button();
			this.cancelBtn = new System.Windows.Forms.Button();
			this.okBtn = new System.Windows.Forms.Button();
			this.password = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.userid = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// serverList
			// 
			this.serverList.Location = new System.Drawing.Point(136, 16);
			this.serverList.Name = "serverList";
			this.serverList.Size = new System.Drawing.Size(212, 21);
			this.serverList.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(116, 23);
			this.label1.TabIndex = 1;
			this.label1.Text = "Server:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 44);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(116, 23);
			this.label2.TabIndex = 2;
			this.label2.Text = "New Database Name:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// dbName
			// 
			this.dbName.Location = new System.Drawing.Point(136, 44);
			this.dbName.Name = "dbName";
			this.dbName.Size = new System.Drawing.Size(212, 20);
			this.dbName.TabIndex = 1;
			this.dbName.Text = "";
			// 
			// helpBtn
			// 
			this.helpBtn.Enabled = false;
			this.helpBtn.Location = new System.Drawing.Point(276, 160);
			this.helpBtn.Name = "helpBtn";
			this.helpBtn.TabIndex = 4;
			this.helpBtn.Text = "&Help";
			// 
			// cancelBtn
			// 
			this.cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelBtn.Location = new System.Drawing.Point(196, 160);
			this.cancelBtn.Name = "cancelBtn";
			this.cancelBtn.TabIndex = 3;
			this.cancelBtn.Text = "&Cancel";
			// 
			// okBtn
			// 
			this.okBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okBtn.Location = new System.Drawing.Point(116, 160);
			this.okBtn.Name = "okBtn";
			this.okBtn.TabIndex = 2;
			this.okBtn.Text = "&OK";
			this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
			// 
			// password
			// 
			this.password.Location = new System.Drawing.Point(137, 108);
			this.password.Name = "password";
			this.password.PasswordChar = '*';
			this.password.Size = new System.Drawing.Size(211, 20);
			this.password.TabIndex = 10;
			this.password.Text = "";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(12, 112);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(112, 16);
			this.label4.TabIndex = 12;
			this.label4.Text = "&Password:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// userid
			// 
			this.userid.Location = new System.Drawing.Point(137, 84);
			this.userid.Name = "userid";
			this.userid.Size = new System.Drawing.Size(211, 20);
			this.userid.TabIndex = 9;
			this.userid.Text = "";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(12, 88);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(112, 16);
			this.label3.TabIndex = 11;
			this.label3.Text = "&Administrative User:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// CreateDatabaseDlg
			// 
			this.AcceptButton = this.okBtn;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.cancelBtn;
			this.ClientSize = new System.Drawing.Size(362, 195);
			this.ControlBox = false;
			this.Controls.Add(this.password);
			this.Controls.Add(this.userid);
			this.Controls.Add(this.dbName);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.okBtn);
			this.Controls.Add(this.cancelBtn);
			this.Controls.Add(this.helpBtn);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.serverList);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "CreateDatabaseDlg";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Create MySQL Database";
			this.ResumeLayout(false);

		}
		#endregion

		private void okBtn_Click(object sender, System.EventArgs e)
		{
		
		}
	}
}
