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

namespace MySql.VSTools
{
	/// <summary>
	/// Summary description for StoredProcedureEditor.
	/// </summary>
	public class SqlTextEditor : BaseEditor 
	{
		private System.Windows.Forms.RichTextBox sqlText;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		//private ServerConfig	config;
		private string			spName;
		private string			dbName;
        private bool changed;
        private EnvDTE.DTE dte;

        public SqlTextEditor(string name, string database, string body,
            DbConnection conn)
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            spName = name;
            dbName = database;
            sqlText.Text = body;

            // set text editor font
            if (dte == null)
                dte = (EnvDTE.DTE)Marshal.GetActiveObject("VisualStudio.DTE.8.0");
            EnvDTE.Properties props = dte.get_Properties("FontsAndColors", "TextEditor");
            EnvDTE.Property family = props.Item("FontFamily");
            EnvDTE.Property size = props.Item("FontSize");
            sqlText.Font = new Font(family.Value.ToString(), 
                float.Parse(size.Value.ToString()));
        }

        public string SqlText
        {
            get { return sqlText.Text; }
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
            get { return sqlText.SelectedText.Length > 0; }
        }

        protected override bool CanPaste
        {
            get { return sqlText.CanPaste(DataFormats.GetFormat(DataFormats.Text)); }
        }

        protected override bool CanRedo
        {
            get { return sqlText.CanRedo; }
        }

        protected override bool CanUndo
        {
            get { return sqlText.CanUndo; }
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
					sqlText.Text = body;
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
            this.sqlText = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // sqlText
            // 
            this.sqlText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sqlText.Location = new System.Drawing.Point(2, 2);
            this.sqlText.Margin = new System.Windows.Forms.Padding(0);
            this.sqlText.Name = "sqlText";
            this.sqlText.Size = new System.Drawing.Size(436, 308);
            this.sqlText.TabIndex = 0;
            this.sqlText.Text = "";
            this.sqlText.TextChanged += new System.EventHandler(this.sqlText_TextChanged);
            // 
            // StoredProcedureEditor
            // 
            this.Controls.Add(this.sqlText);
            this.Name = "StoredProcedureEditor";
            this.Padding = new System.Windows.Forms.Padding(2);
            this.Size = new System.Drawing.Size(440, 312);
            this.ResumeLayout(false);

		}

		#endregion

        private void sqlText_TextChanged(object sender, EventArgs e)
        {
            if (!IsDirty)
                IsDirty = true;
        }

/*        protected override void PostCreateInit()
        {
            base.PostCreateInit();

            if (!CanEdit())
                sqlText.ReadOnly = true;
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
				string sql = "DROP PROCEDURE IF EXISTS " + spName + "; " + sqlText.Text;
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
