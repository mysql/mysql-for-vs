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
	public class CreateDatabaseDlg : System.Windows.Forms.Form
	{
		#region Fields

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
		private System.Windows.Forms.TextBox serverName;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.ComboBox charSet;
		private System.Windows.Forms.ComboBox collation;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton useCurrent;
		private System.Windows.Forms.RadioButton useAlternate;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		#endregion

		private ServerConfig	config;
		private Hashtable		collations;

		public CreateDatabaseDlg()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			collations = new Hashtable();
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

		public DialogResult ShowDialog( IWin32Window owner, ServerConfig sc ) 
		{
			serverName.Text = sc.host;
			config = sc;
			PopulateCollations();
			return ShowDialog( owner );
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
			this.dbName = new System.Windows.Forms.TextBox();
			this.helpBtn = new System.Windows.Forms.Button();
			this.cancelBtn = new System.Windows.Forms.Button();
			this.okBtn = new System.Windows.Forms.Button();
			this.password = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.userid = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.serverName = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.charSet = new System.Windows.Forms.ComboBox();
			this.collation = new System.Windows.Forms.ComboBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.useCurrent = new System.Windows.Forms.RadioButton();
			this.useAlternate = new System.Windows.Forms.RadioButton();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
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
			this.dbName.Location = new System.Drawing.Point(140, 44);
			this.dbName.Name = "dbName";
			this.dbName.Size = new System.Drawing.Size(212, 20);
			this.dbName.TabIndex = 1;
			this.dbName.Text = "";
			// 
			// helpBtn
			// 
			this.helpBtn.Enabled = false;
			this.helpBtn.Location = new System.Drawing.Point(277, 330);
			this.helpBtn.Name = "helpBtn";
			this.helpBtn.TabIndex = 4;
			this.helpBtn.Text = "&Help";
			// 
			// cancelBtn
			// 
			this.cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelBtn.Location = new System.Drawing.Point(196, 330);
			this.cancelBtn.Name = "cancelBtn";
			this.cancelBtn.TabIndex = 3;
			this.cancelBtn.Text = "&Cancel";
			// 
			// okBtn
			// 
			this.okBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okBtn.Location = new System.Drawing.Point(114, 330);
			this.okBtn.Name = "okBtn";
			this.okBtn.TabIndex = 2;
			this.okBtn.Text = "&OK";
			this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
			// 
			// password
			// 
			this.password.Location = new System.Drawing.Point(80, 99);
			this.password.Name = "password";
			this.password.PasswordChar = '*';
			this.password.ReadOnly = true;
			this.password.Size = new System.Drawing.Size(244, 20);
			this.password.TabIndex = 10;
			this.password.Text = "";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(12, 102);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(60, 16);
			this.label4.TabIndex = 12;
			this.label4.Text = "&Password:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// userid
			// 
			this.userid.Location = new System.Drawing.Point(80, 72);
			this.userid.Name = "userid";
			this.userid.ReadOnly = true;
			this.userid.Size = new System.Drawing.Size(244, 20);
			this.userid.TabIndex = 9;
			this.userid.Text = "";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(12, 76);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(60, 16);
			this.label3.TabIndex = 11;
			this.label3.Text = "&User ID:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// serverName
			// 
			this.serverName.Location = new System.Drawing.Point(140, 18);
			this.serverName.Name = "serverName";
			this.serverName.ReadOnly = true;
			this.serverName.Size = new System.Drawing.Size(212, 20);
			this.serverName.TabIndex = 13;
			this.serverName.Text = "";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.collation);
			this.groupBox1.Controls.Add(this.charSet);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Location = new System.Drawing.Point(8, 84);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(344, 92);
			this.groupBox1.TabIndex = 14;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Options";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(4, 20);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(116, 23);
			this.label5.TabIndex = 15;
			this.label5.Text = "Default Character Set:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(4, 51);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(116, 23);
			this.label6.TabIndex = 16;
			this.label6.Text = "Default Collation:";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// charSet
			// 
			this.charSet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.charSet.Location = new System.Drawing.Point(124, 20);
			this.charSet.Name = "charSet";
			this.charSet.Size = new System.Drawing.Size(200, 21);
			this.charSet.Sorted = true;
			this.charSet.TabIndex = 17;
			this.charSet.SelectedIndexChanged += new System.EventHandler(this.charSet_SelectedIndexChanged);
			// 
			// collation
			// 
			this.collation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.collation.Location = new System.Drawing.Point(124, 52);
			this.collation.Name = "collation";
			this.collation.Size = new System.Drawing.Size(200, 21);
			this.collation.TabIndex = 18;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.useAlternate);
			this.groupBox2.Controls.Add(this.useCurrent);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.password);
			this.groupBox2.Controls.Add(this.userid);
			this.groupBox2.Location = new System.Drawing.Point(8, 184);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(344, 132);
			this.groupBox2.TabIndex = 15;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Credentials";
			// 
			// useCurrent
			// 
			this.useCurrent.Checked = true;
			this.useCurrent.Location = new System.Drawing.Point(16, 20);
			this.useCurrent.Name = "useCurrent";
			this.useCurrent.Size = new System.Drawing.Size(288, 16);
			this.useCurrent.TabIndex = 13;
			this.useCurrent.TabStop = true;
			this.useCurrent.Text = "Use current credentials";
			this.useCurrent.Click += new System.EventHandler(this.useCurrent_Click);
			// 
			// useAlternate
			// 
			this.useAlternate.Location = new System.Drawing.Point(16, 44);
			this.useAlternate.Name = "useAlternate";
			this.useAlternate.Size = new System.Drawing.Size(288, 16);
			this.useAlternate.TabIndex = 14;
			this.useAlternate.Text = "Use alternate credentials";
			this.useAlternate.Click += new System.EventHandler(this.useAlternate_Click);
			// 
			// CreateDatabaseDlg
			// 
			this.AcceptButton = this.okBtn;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.cancelBtn;
			this.ClientSize = new System.Drawing.Size(362, 363);
			this.ControlBox = false;
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.serverName);
			this.Controls.Add(this.dbName);
			this.Controls.Add(this.okBtn);
			this.Controls.Add(this.cancelBtn);
			this.Controls.Add(this.helpBtn);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "CreateDatabaseDlg";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Create MySQL Database";
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void PopulateCollations()
		{
			MySqlConnection conn = new MySqlConnection( config.GetConnectString(false));
			MySqlDataReader reader = null;
			Hashtable charSets = new Hashtable();

			try 
			{
				conn.Open();
				string version = conn.ServerVersion;
				if (!version.StartsWith("3") && !version.StartsWith("4.0"))
				{
					MySqlCommand cmd = new MySqlCommand("SHOW COLLATION", conn);
					reader = cmd.ExecuteReader();
					while (reader.Read()) 
					{
						charSets[ reader["Charset"] ] = 1;
						collations[ reader["Collation"] ] = reader["Charset"];
					}
				
					// now add the character sets to our combobox
					foreach (string charset in charSets.Keys)
						this.charSet.Items.Add( charset );
					charSet.SelectedIndex = 0;
				}
				else 
				{
					collation.Enabled = false;
					charSet.Enabled = false;
				}
			}
			catch (Exception) 
			{
			}
			finally 
			{
				if (reader != null) reader.Close();
				conn.Close();
			}
		}

		private void okBtn_Click(object sender, System.EventArgs e)
		{
			ServerConfig sc = config.Clone();
			if (this.useAlternate.Checked) 
			{
				sc.userId = this.userid.Text.Trim();
				sc.password = this.password.Text.Trim();
			}

			MySqlConnection conn = new MySqlConnection( sc.GetConnectString(false) );
			try 
			{
				conn.Open();
				string sql = "CREATE DATABASE " + this.dbName.Text.Trim();
				if (charSet.Enabled) 
				{
					sql += " CHARACTER SET " + charSet.Text;
					sql += " COLLATE " + collation.Text;
				}

				MySqlCommand cmd = new MySqlCommand( sql, conn );
				cmd.ExecuteNonQuery();
			}
			catch (MySqlException mex) 
			{
				MessageBox.Show("Error creating database: " + mex.Message);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Unexpected error creating database: " + ex.Message);
			}
			finally 
			{
				conn.Close();
			}

		}

		private void useCurrent_Click(object sender, System.EventArgs e)
		{
			userid.ReadOnly = true;
			password.ReadOnly = true;
		}

		private void useAlternate_Click(object sender, System.EventArgs e)
		{
			userid.ReadOnly = false;
			password.ReadOnly = false;
		}

		private void charSet_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			// the user has selected a different character set from our list so 
			// we need to show only the collations for that character set
			this.collation.Items.Clear();

			foreach (string key in collations.Keys) 
			{
				if (collations[key].ToString() == this.charSet.Text)
					collation.Items.Add( key );
			}
			collation.SelectedIndex = 0;
		}
	}
}
