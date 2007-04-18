using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace ByteFX.Data.Common.Design
{
	/// <summary>
	/// Summary description for DataAdapterPreviewDialog.
	/// </summary>
	public class DataAdapterPreviewDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListView parameterList;
		private System.Windows.Forms.ColumnHeader adapterColumn;
		private System.Windows.Forms.ColumnHeader nameColumn;
		private System.Windows.Forms.ColumnHeader dataTypeColumn;
		private System.Windows.Forms.ColumnHeader valueColumn;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.StatusBar statusBar1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.DataGrid dataGrid1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public DataAdapterPreviewDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
			this.label1 = new System.Windows.Forms.Label();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.parameterList = new System.Windows.Forms.ListView();
			this.adapterColumn = new System.Windows.Forms.ColumnHeader();
			this.nameColumn = new System.Windows.Forms.ColumnHeader();
			this.dataTypeColumn = new System.Windows.Forms.ColumnHeader();
			this.valueColumn = new System.Windows.Forms.ColumnHeader();
			this.button1 = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.statusBar1 = new System.Windows.Forms.StatusBar();
			this.label3 = new System.Windows.Forms.Label();
			this.dataGrid1 = new System.Windows.Forms.DataGrid();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Data Adapters";
			// 
			// comboBox1
			// 
			this.comboBox1.Location = new System.Drawing.Point(8, 24);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(200, 21);
			this.comboBox1.TabIndex = 1;
			this.comboBox1.Text = "comboBox1";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(216, 8);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 16);
			this.label2.TabIndex = 2;
			this.label2.Text = "Parameters";
			// 
			// parameterList
			// 
			this.parameterList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.parameterList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																							this.adapterColumn,
																							this.nameColumn,
																							this.dataTypeColumn,
																							this.valueColumn});
			this.parameterList.Location = new System.Drawing.Point(216, 24);
			this.parameterList.Name = "parameterList";
			this.parameterList.Size = new System.Drawing.Size(392, 104);
			this.parameterList.TabIndex = 3;
			this.parameterList.View = System.Windows.Forms.View.Details;
			// 
			// adapterColumn
			// 
			this.adapterColumn.Text = "Adapter";
			// 
			// nameColumn
			// 
			this.nameColumn.Text = "Name";
			// 
			// dataTypeColumn
			// 
			this.dataTypeColumn.Text = "Data Type";
			// 
			// valueColumn
			// 
			this.valueColumn.Text = "Value";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(8, 56);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(128, 32);
			this.button1.TabIndex = 4;
			this.button1.Text = "&Fill Dataset";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.comboBox1);
			this.panel1.Controls.Add(this.button1);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.parameterList);
			this.panel1.Controls.Add(this.label2);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(576, 136);
			this.panel1.TabIndex = 5;
			// 
			// splitter1
			// 
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
			this.splitter1.Location = new System.Drawing.Point(0, 136);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(576, 3);
			this.splitter1.TabIndex = 6;
			this.splitter1.TabStop = false;
			// 
			// statusBar1
			// 
			this.statusBar1.Location = new System.Drawing.Point(0, 296);
			this.statusBar1.Name = "statusBar1";
			this.statusBar1.Size = new System.Drawing.Size(576, 22);
			this.statusBar1.TabIndex = 7;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(224, 144);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 16);
			this.label3.TabIndex = 8;
			this.label3.Text = "Results";
			// 
			// dataGrid1
			// 
			this.dataGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.dataGrid1.DataMember = "";
			this.dataGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dataGrid1.Location = new System.Drawing.Point(224, 168);
			this.dataGrid1.Name = "dataGrid1";
			this.dataGrid1.Size = new System.Drawing.Size(344, 104);
			this.dataGrid1.TabIndex = 9;
			// 
			// DataAdapterPreviewDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(576, 318);
			this.Controls.Add(this.dataGrid1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.statusBar1);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.panel1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DataAdapterPreviewDialog";
			this.Text = "Preview Data";
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion
	}
}
