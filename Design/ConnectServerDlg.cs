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

namespace MySql.Data.MySqlClient.Design
{
	/// <summary>
	/// Summary description for ConnectServerDlg.
	/// </summary>
	public class ConnectServerDlg : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox computer;
		private System.Windows.Forms.Button cancelBtn;
		private System.Windows.Forms.Button okBtn;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox userid;
		private System.Windows.Forms.TextBox password;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ConnectServerDlg()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

/*		public Server Server
		{
			get 
			{ 
				Server s = new Server(); 
				s.Host = computer.Text.Trim();
				s.UserId = userid.Text.Trim();
				s.Password = password.Text.Trim();
				return s;
			}
		}*/

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
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.computer = new System.Windows.Forms.TextBox();
			this.cancelBtn = new System.Windows.Forms.Button();
			this.okBtn = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.userid = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.password = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(320, 32);
			this.label1.TabIndex = 0;
			this.label1.Text = "To connect to a new server, enter the computer name, or IP address below:";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(12, 68);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(72, 16);
			this.label2.TabIndex = 1;
			this.label2.Text = "&Computer:";
			// 
			// computer
			// 
			this.computer.Location = new System.Drawing.Point(84, 64);
			this.computer.Name = "computer";
			this.computer.Size = new System.Drawing.Size(248, 20);
			this.computer.TabIndex = 0;
			this.computer.Text = "";
			// 
			// cancelBtn
			// 
			this.cancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelBtn.Location = new System.Drawing.Point(256, 156);
			this.cancelBtn.Name = "cancelBtn";
			this.cancelBtn.TabIndex = 4;
			this.cancelBtn.Text = "&Cancel";
			// 
			// okBtn
			// 
			this.okBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okBtn.Location = new System.Drawing.Point(172, 156);
			this.okBtn.Name = "okBtn";
			this.okBtn.TabIndex = 3;
			this.okBtn.Text = "&OK";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(12, 100);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(112, 16);
			this.label3.TabIndex = 6;
			this.label3.Text = "&Administrative User:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// userid
			// 
			this.userid.Location = new System.Drawing.Point(128, 95);
			this.userid.Name = "userid";
			this.userid.Size = new System.Drawing.Size(204, 20);
			this.userid.TabIndex = 1;
			this.userid.Text = "";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(12, 124);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(112, 16);
			this.label4.TabIndex = 8;
			this.label4.Text = "&Password:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// password
			// 
			this.password.Location = new System.Drawing.Point(128, 120);
			this.password.Name = "password";
			this.password.Size = new System.Drawing.Size(204, 20);
			this.password.TabIndex = 2;
			this.password.Text = "";
			// 
			// ConnectServerDlg
			// 
			this.AcceptButton = this.okBtn;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.cancelBtn;
			this.ClientSize = new System.Drawing.Size(338, 191);
			this.ControlBox = false;
			this.Controls.Add(this.password);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.userid);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.okBtn);
			this.Controls.Add(this.cancelBtn);
			this.Controls.Add(this.computer);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ConnectServerDlg";
			this.ShowInTaskbar = false;
			this.Text = "Connect To MySQL Server";
			this.ResumeLayout(false);

		}
		#endregion
	}
}
