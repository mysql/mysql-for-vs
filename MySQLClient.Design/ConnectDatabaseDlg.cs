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
using MySql.Data.MySqlClient;

namespace MySql.Data.MySqlClient.Design
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
		private System.Windows.Forms.PropertyGrid properties;
		private System.Windows.Forms.Button testBtn;
		private System.Windows.Forms.Label label1;
		#endregion

		public ConnectDatabaseDlg(string connStr)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			MySqlConnectionString cs = new MySqlConnectionString();
			cs.SetConnectionString( connStr );
			properties.SelectedObject = cs;
		}

		internal MySqlConnectionString GetConnectStringObject()
		{
			return (properties.SelectedObject as MySqlConnectionString);
		}

		public string GetConnectionString()
		{
			MySqlConnectionString cs = (properties.SelectedObject as MySqlConnectionString);
			return cs.CreateConnectionString();
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
			this.cancelBtn = new System.Windows.Forms.Button();
			this.okBtn = new System.Windows.Forms.Button();
			this.properties = new System.Windows.Forms.PropertyGrid();
			this.testBtn = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// cancelBtn
			// 
			this.cancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelBtn.CausesValidation = false;
			this.cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelBtn.Location = new System.Drawing.Point(299, 422);
			this.cancelBtn.Name = "cancelBtn";
			this.cancelBtn.TabIndex = 11;
			this.cancelBtn.Text = "&Cancel";
			// 
			// okBtn
			// 
			this.okBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okBtn.Location = new System.Drawing.Point(218, 422);
			this.okBtn.Name = "okBtn";
			this.okBtn.TabIndex = 10;
			this.okBtn.Text = "&OK";
			this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
			// 
			// properties
			// 
			this.properties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.properties.CommandsVisibleIfAvailable = true;
			this.properties.LargeButtons = false;
			this.properties.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.properties.Location = new System.Drawing.Point(8, 28);
			this.properties.Name = "properties";
			this.properties.Size = new System.Drawing.Size(366, 376);
			this.properties.TabIndex = 24;
			this.properties.Text = "PropertyGrid";
			this.properties.ToolbarVisible = false;
			this.properties.ViewBackColor = System.Drawing.SystemColors.Window;
			this.properties.ViewForeColor = System.Drawing.SystemColors.WindowText;
			// 
			// testBtn
			// 
			this.testBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.testBtn.Location = new System.Drawing.Point(8, 420);
			this.testBtn.Name = "testBtn";
			this.testBtn.Size = new System.Drawing.Size(72, 23);
			this.testBtn.TabIndex = 9;
			this.testBtn.Text = "&Test";
			this.testBtn.Click += new System.EventHandler(this.testBtn_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(104, 20);
			this.label1.TabIndex = 26;
			this.label1.Text = "Connection Details:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// ConnectDatabaseDlg
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(384, 458);
			this.ControlBox = false;
			this.Controls.Add(this.label1);
			this.Controls.Add(this.properties);
			this.Controls.Add(this.testBtn);
			this.Controls.Add(this.okBtn);
			this.Controls.Add(this.cancelBtn);
			this.Name = "ConnectDatabaseDlg";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Connect To MySQL Database";
			this.ResumeLayout(false);

		}
		#endregion


		private void testBtn_Click(object sender, System.EventArgs e)
		{
			string connStr = (properties.SelectedObject as MySqlConnectionString).CreateConnectionString();

			Cursor curr = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			try 
			{
				MySqlConnection conn = new MySqlConnection( connStr );
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
			DialogResult = DialogResult.None;
			MySqlConnectionString cs = (properties.SelectedObject as MySqlConnectionString);

			if (cs.Server == null || cs.Server.Trim().Length == 0)
			{
				MessageBox.Show("You must enter or select a server");
				return;
			}
			DialogResult = DialogResult.OK;
		}

	}
}
