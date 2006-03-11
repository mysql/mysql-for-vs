using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Data.Common;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System.Runtime.InteropServices;

namespace Vsip.MyVSTools
{
	/// <summary>
	/// Summary description for StoredProcedureEditor.
	/// </summary>
	public class StoredProcedureEditor : BaseEditor 
	{
		private System.Windows.Forms.RichTextBox procText;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		//private ServerConfig	config;
		private string			spName;
		private string			dbName;
        private bool changed;
        private EnvDTE.DTE dte;

        public StoredProcedureEditor(string name, string database, string body,
            DbConnection conn)
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            spName = name;
            dbName = database;
            procText.Text = body;

            // set text editor font
            if (dte == null)
                dte = (EnvDTE.DTE)Marshal.GetActiveObject("VisualStudio.DTE.8.0");
            EnvDTE.Properties props = dte.get_Properties("FontsAndColors", "TextEditor");
            EnvDTE.Property family = props.Item("FontFamily");
            EnvDTE.Property size = props.Item("FontSize");
            procText.Font = new Font(family.Value.ToString(), 
                float.Parse(size.Value.ToString()));
        }
		

        protected override Guid EditorGuid
        {
            get { return GuidList.guidProcedureEditor; }
        }

        public override string Filename
        {
            get { return dbName + "." + spName; }
        }

        protected override bool CanCopyAndCut
        {
            get { return procText.SelectedText.Length > 0; }
        }

        protected override bool CanPaste
        {
            get { return procText.CanPaste(DataFormats.GetFormat(DataFormats.Text)); }
        }

        protected override bool CanRedo
        {
            get { return procText.CanRedo; }
        }

        protected override bool CanUndo
        {
            get { return procText.CanUndo; }
        }

		/*public void Edit(string spName, string db, ServerConfig sc) 
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.procText = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // procText
            // 
            this.procText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.procText.Location = new System.Drawing.Point(2, 2);
            this.procText.Margin = new System.Windows.Forms.Padding(0);
            this.procText.Name = "procText";
            this.procText.Size = new System.Drawing.Size(436, 308);
            this.procText.TabIndex = 0;
            this.procText.Text = "";
            this.procText.TextChanged += new System.EventHandler(this.procText_TextChanged);
            // 
            // StoredProcedureEditor
            // 
            this.Controls.Add(this.procText);
            this.Name = "StoredProcedureEditor";
            this.Padding = new System.Windows.Forms.Padding(2);
            this.Size = new System.Drawing.Size(440, 312);
            this.ResumeLayout(false);

		}

		#endregion

        private void procText_TextChanged(object sender, EventArgs e)
        {
            if (!IsDirty)
                IsDirty = true;
        }

/*        protected override void PostCreateInit()
        {
            base.PostCreateInit();

            if (!CanEdit())
                procText.ReadOnly = true;
        }
        */
/*		private void saveBtn_Click(object sender, System.EventArgs e)
		{
			//string connStr = config.GetConnectString(false);
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
		}*/

    }
}
