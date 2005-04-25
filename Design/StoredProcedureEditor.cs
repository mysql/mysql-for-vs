using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace MySql.Design
{
	/// <summary>
	/// Summary description for StoredProcedureEditor.
	/// </summary>
	public class StoredProcedureEditor : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.RichTextBox procText;
		private System.Windows.Forms.Button saveBtn;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private ServerConfig	config;
		private string			spName;
		private string			dbName;


		public StoredProcedureEditor()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

		}

		public void Edit(string spName, string db, ServerConfig sc) 
		{
			this.spName = spName;
			dbName = db;
			config = sc;
			string connStr = sc.GetConnectString(false);
			MySqlConnection conn = new MySqlConnection(connStr);
			try 
			{
				conn.Open();
				conn.ChangeDatabase(db);
				MySqlCommand cmd = new MySqlCommand("SHOW CREATE PROCEDURE " + spName, conn);
				using (MySqlDataReader reader = cmd.ExecuteReader()) 
				{
					reader.Read();
					string body = reader.GetString(2);
					body = body.Replace("\n", "\r\n");
					procText.Text = body;
				}
			}
			catch (Exception ex) 
			{
				MessageBox.Show("Error editing stored procedure: " + ex.Message);
			}
			finally 
			{
				if (conn != null)
					conn.Close();
			}
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.procText = new System.Windows.Forms.RichTextBox();
			this.saveBtn = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// procText
			// 
			this.procText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.procText.Location = new System.Drawing.Point(2, 32);
			this.procText.Name = "procText";
			this.procText.Size = new System.Drawing.Size(436, 276);
			this.procText.TabIndex = 0;
			this.procText.Text = "";
			// 
			// saveBtn
			// 
			this.saveBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.saveBtn.Location = new System.Drawing.Point(356, 4);
			this.saveBtn.Name = "saveBtn";
			this.saveBtn.TabIndex = 1;
			this.saveBtn.Text = "Save";
			this.saveBtn.Click += new System.EventHandler(this.saveBtn_Click);
			// 
			// StoredProcedureEditor
			// 
			this.Controls.Add(this.saveBtn);
			this.Controls.Add(this.procText);
			this.DockPadding.All = 2;
			this.Name = "StoredProcedureEditor";
			this.Size = new System.Drawing.Size(440, 312);
			this.ResumeLayout(false);

		}
		#endregion

		private void saveBtn_Click(object sender, System.EventArgs e)
		{
			string connStr = config.GetConnectString(false);
			MySqlConnection conn = new MySqlConnection(connStr);
			try 
			{
				conn.Open();
				conn.ChangeDatabase(dbName);
				string sql = "DROP PROCEDURE IF EXISTS " + spName + "; " + procText.Text;
				MySqlCommand cmd = new MySqlCommand(sql, conn);
				cmd.ExecuteNonQuery();
			}
			catch (Exception ex) 
			{
				MessageBox.Show("Error creating stored procedure: " + ex.Message);
			}
			finally 
			{
				if (conn != null)
					conn.Close();
			}
		}
	}
}
