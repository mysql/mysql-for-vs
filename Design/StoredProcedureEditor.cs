using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace MySQL.Design
{
	/// <summary>
	/// Summary description for StoredProcedureEditor.
	/// </summary>
	public class StoredProcedureEditor : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.RichTextBox procText;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public StoredProcedureEditor()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

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
			this.SuspendLayout();
			// 
			// procText
			// 
			this.procText.Dock = System.Windows.Forms.DockStyle.Fill;
			this.procText.Location = new System.Drawing.Point(2, 2);
			this.procText.Name = "procText";
			this.procText.Size = new System.Drawing.Size(436, 308);
			this.procText.TabIndex = 0;
			this.procText.Text = "";
			// 
			// StoredProcedureEditor
			// 
			this.Controls.Add(this.procText);
			this.DockPadding.All = 2;
			this.Name = "StoredProcedureEditor";
			this.Size = new System.Drawing.Size(440, 312);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
