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
using System.Data;
using System.Windows.Forms;
using System.Text.RegularExpressions;


namespace MySql.VSTools
{
	/// <summary>
	/// Summary description for TableDesigner.
	/// </summary>
	public class TableEditor : BaseEditor
	{
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.DataGrid columnGrid;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox tableType;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox autoIncrement;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.CheckBox delayKeyWrite;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox minRows;
		private System.Windows.Forms.TextBox maxRows;
		private System.Windows.Forms.CheckBox packKeys;
		private System.Windows.Forms.CheckBox checksum;
		private System.Windows.Forms.TextBox comment;
		private System.Windows.Forms.ComboBox rowFormat;
		private System.Windows.Forms.TextBox rowLength;
		private System.Windows.Forms.Label label8;

		private DataTable	columnsData;
		private System.Windows.Forms.Splitter splitter1;

		private System.Windows.Forms.DataGridTableStyle dataGridTableStyle1;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn1;
		private DataGridComboBoxColumn	typeColumn;
		private System.Windows.Forms.DataGridBoolColumn dataGridBoolColumn1;

		private string[] columnTypes = new string[] { "TINYINT", "SMALLINT",
														"MEDIUMINT", "INT", "BIGINT", "REAL", "DOUBLE", "FLOAT",
														"DECIMAL", "NUMERIC", "CHAR", "VARCHAR", "DATE", "TIME",
														"TIMESTAMP", "DATETIME", "TINYBLOB", "BLOB", "MEDIUMBLOB",
														"LONGBLOB", "TINYTEXT", "TEXT", "MEDIUMTEXT", "LONGTEXT", 
														"ENUM", "SET" };
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.TabPage tabPage4;
		private System.Windows.Forms.TextBox sql;
		private System.Windows.Forms.PropertyGrid propertyGrid1;

		private TableDesignerColumnCollection	columns;
		private System.Windows.Forms.TextBox tableName;
		private TableInfo tableInfo;
		private bool		dirty;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public TableEditor()
		{
			tableInfo = new TableInfo();
			dirty = false;

			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			DataGridTableStyle ts = columnGrid.TableStyles["TableDesignerColumnCollection"];
			typeColumn = new DataGridComboBoxColumn( "Data Type", columnTypes, 
				"ColumnTypeForGrid" );
			typeColumn.NullText = "";

			ts.GridColumnStyles.Clear();
			dataGridTableStyle1.GridColumnStyles.AddRange(
				new System.Windows.Forms.DataGridColumnStyle[] {
					   this.dataGridTextBoxColumn1, typeColumn, this.dataGridBoolColumn1});

			columns = new TableDesignerColumnCollection();
			columnGrid.DataSource = columns; 
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

        public override string Filename
        {
            get { return "tablename"; }
        }

/*		public void EditTable( MySqlConnectionString conn, string tableName )
		{
			dbConn = conn;
			this.tableName = tableName;

			if (tableName == null || tableName.Trim().Length == 0) return;

			MySqlConnection c = new MySqlConnection( conn.CreateConnectionString() );
			MySqlDataAdapter da = new MySqlDataAdapter("SHOW TABLE STATUS", c);
			DataTable dt = new DataTable();
			da.Fill(dt);
			foreach (DataRow row in dt.Rows)
			{
				if ((string)row[0] == tableName) 
				{
					LoadTableInfo(row); break;
				}
			}

			da.SelectCommand.CommandText = "SHOW COLUMNS FROM " + tableName;
			dt.Clear();
			da.Fill(dt);
			LoadColumnInfo( dt );
		}
*/
		private string Normalize( string colValue )
		{
			if (colValue == "NULL") return String.Empty;
			return colValue;
		}

		private void LoadColumnInfo( DataTable dt )
		{
/*			columns.Rows.Clear();
			Regex reg = new Regex( @"([^( ]+)(?:[(](\d+)[,]*(\d*)[)])*.*" );

			foreach (DataRow row in dt.Rows)
			{
				DataRow colRow = columns.NewRow();

				string colExtra = row["Extra"].ToString();
				string type = row["Type"].ToString();

				Match m = reg.Match(type);
				if (! m.Success) throw new Exception("Error reading column information");

				colRow["datatype"] = m.Groups[1].Value;
				colRow["length"] = m.Groups[2].Value;
				colRow["decimals"] = m.Groups[3].Value;
				colRow["name"] = row["Field"];
				colRow["allow_nulls"] = row["Null"].ToString() == "YES";
				colRow["default"] = Normalize(row["Default"].ToString());
				colRow["auto_inc"] = colExtra.IndexOf("auto_increment") != -1;
				colRow["unsigned"] = type.IndexOf("unsigned") != -1;
				colRow["binary"] = type.IndexOf("binary") != -1;
				colRow["zero_fill"] = type.IndexOf("zerofill") != -1;
				columns.Rows.Add( colRow );
//				columns.Columns.Add( "comment", typeof(string));
			}*/
		}

/*		private void LoadTableInfo( DataRow row )
		{
			tableTitle.Text = row["name"].ToString();
			comment.Text = row["Comment"].ToString();
			tableType.SelectedIndex = tableType.FindStringExact(row["type"].ToString());
			rowFormat.SelectedIndex = rowFormat.FindStringExact( row["row_format"].ToString());
			autoIncrement.Text = row["Auto_increment"].ToString();
			rowLength.Text = row["avg_row_length"].ToString();

			string optionString = row["Create_options"].ToString();
			string[] options = optionString.Split(' ');
			foreach (string option in options)
			{
				string[] parts = option.Split('=');
				string key = parts[0].ToLower().Trim();
				string val = parts[1].Trim();

				if (key == "min_rows")
					minRows.Text = val;
				else if (key == "max_rows")
					maxRows.Text = val;
				else if (key == "delay_key_write")
					delayKeyWrite.Checked = (val == "1");
				else if (key == "pack_keys")
					packKeys.Checked = (val == "1");
				else if (key == "checksum")
					checksum.Checked = (val == "1");
			}
		}
*/
		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(TableEditor));
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.tableName = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.checksum = new System.Windows.Forms.CheckBox();
			this.packKeys = new System.Windows.Forms.CheckBox();
			this.maxRows = new System.Windows.Forms.TextBox();
			this.minRows = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.delayKeyWrite = new System.Windows.Forms.CheckBox();
			this.rowLength = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.autoIncrement = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.rowFormat = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.comment = new System.Windows.Forms.TextBox();
			this.tableType = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.columnGrid = new System.Windows.Forms.DataGrid();
			this.dataGridTableStyle1 = new System.Windows.Forms.DataGridTableStyle();
			this.dataGridTextBoxColumn1 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridBoolColumn1 = new System.Windows.Forms.DataGridBoolColumn();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.tabPage4 = new System.Windows.Forms.TabPage();
			this.sql = new System.Windows.Forms.TextBox();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.columnGrid)).BeginInit();
			this.tabPage4.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Controls.Add(this.tabPage4);
			this.tabControl1.Location = new System.Drawing.Point(2, 2);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(432, 588);
			this.tabControl1.TabIndex = 0;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.tableName);
			this.tabPage1.Controls.Add(this.label8);
			this.tabPage1.Controls.Add(this.checksum);
			this.tabPage1.Controls.Add(this.packKeys);
			this.tabPage1.Controls.Add(this.maxRows);
			this.tabPage1.Controls.Add(this.minRows);
			this.tabPage1.Controls.Add(this.label7);
			this.tabPage1.Controls.Add(this.label6);
			this.tabPage1.Controls.Add(this.delayKeyWrite);
			this.tabPage1.Controls.Add(this.rowLength);
			this.tabPage1.Controls.Add(this.label5);
			this.tabPage1.Controls.Add(this.autoIncrement);
			this.tabPage1.Controls.Add(this.label4);
			this.tabPage1.Controls.Add(this.rowFormat);
			this.tabPage1.Controls.Add(this.label3);
			this.tabPage1.Controls.Add(this.label2);
			this.tabPage1.Controls.Add(this.comment);
			this.tabPage1.Controls.Add(this.tableType);
			this.tabPage1.Controls.Add(this.label1);
			this.tabPage1.Location = new System.Drawing.Point(4, 25);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Size = new System.Drawing.Size(424, 559);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Properties";
			// 
			// tableName
			// 
			this.tableName.Location = new System.Drawing.Point(12, 32);
			this.tableName.Name = "tableName";
			this.tableName.Size = new System.Drawing.Size(332, 20);
			this.tableName.TabIndex = 0;
			this.tableName.Text = "";
			this.tableName.TextChanged += new System.EventHandler(this.ItemChanged);
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(12, 12);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(44, 20);
			this.label8.TabIndex = 17;
			this.label8.Text = "Name:";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checksum
			// 
			this.checksum.Location = new System.Drawing.Point(252, 204);
			this.checksum.Name = "checksum";
			this.checksum.Size = new System.Drawing.Size(132, 24);
			this.checksum.TabIndex = 9;
			this.checksum.Text = "Maintain Checksum";
			// 
			// packKeys
			// 
			this.packKeys.Location = new System.Drawing.Point(252, 180);
			this.packKeys.Name = "packKeys";
			this.packKeys.Size = new System.Drawing.Size(116, 24);
			this.packKeys.TabIndex = 8;
			this.packKeys.Text = "Pack Keys";
			// 
			// maxRows
			// 
			this.maxRows.Location = new System.Drawing.Point(124, 188);
			this.maxRows.Name = "maxRows";
			this.maxRows.TabIndex = 6;
			this.maxRows.Text = "";
			// 
			// minRows
			// 
			this.minRows.Location = new System.Drawing.Point(12, 188);
			this.minRows.Name = "minRows";
			this.minRows.TabIndex = 5;
			this.minRows.Text = "";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(124, 168);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(100, 20);
			this.label7.TabIndex = 12;
			this.label7.Text = "Max Rows:";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(12, 168);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(100, 20);
			this.label6.TabIndex = 11;
			this.label6.Text = "Min Rows:";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// delayKeyWrite
			// 
			this.delayKeyWrite.Location = new System.Drawing.Point(252, 156);
			this.delayKeyWrite.Name = "delayKeyWrite";
			this.delayKeyWrite.Size = new System.Drawing.Size(116, 24);
			this.delayKeyWrite.TabIndex = 7;
			this.delayKeyWrite.Text = "Delay Key Write";
			// 
			// rowLength
			// 
			this.rowLength.Location = new System.Drawing.Point(124, 136);
			this.rowLength.Name = "rowLength";
			this.rowLength.Size = new System.Drawing.Size(108, 20);
			this.rowLength.TabIndex = 4;
			this.rowLength.Text = "";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(124, 116);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(160, 20);
			this.label5.TabIndex = 8;
			this.label5.Text = "Average Row Length (bytes):";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// autoIncrement
			// 
			this.autoIncrement.Location = new System.Drawing.Point(12, 136);
			this.autoIncrement.Name = "autoIncrement";
			this.autoIncrement.TabIndex = 3;
			this.autoIncrement.Text = "";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(12, 116);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(100, 20);
			this.label4.TabIndex = 6;
			this.label4.Text = "Auto Increment:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// rowFormat
			// 
			this.rowFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.rowFormat.Items.AddRange(new object[] {
														   "Fixed",
														   "Dynamic"});
			this.rowFormat.Location = new System.Drawing.Point(184, 84);
			this.rowFormat.Name = "rowFormat";
			this.rowFormat.Size = new System.Drawing.Size(160, 21);
			this.rowFormat.TabIndex = 2;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(184, 64);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(76, 20);
			this.label3.TabIndex = 4;
			this.label3.Text = "Row Format:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(12, 220);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(80, 20);
			this.label2.TabIndex = 3;
			this.label2.Text = "Comment";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// comment
			// 
			this.comment.Location = new System.Drawing.Point(12, 240);
			this.comment.MaxLength = 60;
			this.comment.Multiline = true;
			this.comment.Name = "comment";
			this.comment.Size = new System.Drawing.Size(372, 48);
			this.comment.TabIndex = 10;
			this.comment.Text = "";
			// 
			// tableType
			// 
			this.tableType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.tableType.Items.AddRange(new object[] {
														   "BerkeleyDB",
														   "HEAP",
														   "ISAM",
														   "InnoDB",
														   "Merge",
														   "MyISAM"});
			this.tableType.Location = new System.Drawing.Point(12, 84);
			this.tableType.Name = "tableType";
			this.tableType.Size = new System.Drawing.Size(160, 21);
			this.tableType.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 64);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(44, 20);
			this.label1.TabIndex = 0;
			this.label1.Text = "Type:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.propertyGrid1);
			this.tabPage2.Controls.Add(this.splitter1);
			this.tabPage2.Controls.Add(this.columnGrid);
			this.tabPage2.DockPadding.All = 4;
			this.tabPage2.Location = new System.Drawing.Point(4, 25);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Size = new System.Drawing.Size(424, 559);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Columns";
			// 
			// propertyGrid1
			// 
			this.propertyGrid1.CommandsVisibleIfAvailable = true;
			this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGrid1.HelpVisible = false;
			this.propertyGrid1.LargeButtons = false;
			this.propertyGrid1.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.propertyGrid1.Location = new System.Drawing.Point(4, 263);
			this.propertyGrid1.Name = "propertyGrid1";
			this.propertyGrid1.Size = new System.Drawing.Size(416, 292);
			this.propertyGrid1.TabIndex = 3;
			this.propertyGrid1.Text = "propertyGrid1";
			this.propertyGrid1.ToolbarVisible = false;
			this.propertyGrid1.ViewBackColor = System.Drawing.SystemColors.Window;
			this.propertyGrid1.ViewForeColor = System.Drawing.SystemColors.WindowText;
			// 
			// splitter1
			// 
			this.splitter1.BackColor = System.Drawing.SystemColors.ControlDark;
			this.splitter1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
			this.splitter1.Location = new System.Drawing.Point(4, 260);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(416, 3);
			this.splitter1.TabIndex = 1;
			this.splitter1.TabStop = false;
			// 
			// columnGrid
			// 
			this.columnGrid.CaptionVisible = false;
			this.columnGrid.DataMember = "";
			this.columnGrid.Dock = System.Windows.Forms.DockStyle.Top;
			this.columnGrid.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.columnGrid.Location = new System.Drawing.Point(4, 4);
			this.columnGrid.Name = "columnGrid";
			this.columnGrid.Size = new System.Drawing.Size(416, 256);
			this.columnGrid.TabIndex = 0;
			this.columnGrid.TableStyles.AddRange(new System.Windows.Forms.DataGridTableStyle[] {
																								   this.dataGridTableStyle1});
			this.columnGrid.CurrentCellChanged += new System.EventHandler(this.columnGrid_CurrentCellChanged);
			// 
			// dataGridTableStyle1
			// 
			this.dataGridTableStyle1.DataGrid = this.columnGrid;
			this.dataGridTableStyle1.GridColumnStyles.AddRange(new System.Windows.Forms.DataGridColumnStyle[] {
																												  this.dataGridTextBoxColumn1,
																												  this.dataGridBoolColumn1});
			this.dataGridTableStyle1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dataGridTableStyle1.MappingName = "TableDesignerColumnCollection";
			// 
			// dataGridTextBoxColumn1
			// 
			this.dataGridTextBoxColumn1.Format = "";
			this.dataGridTextBoxColumn1.FormatInfo = null;
			this.dataGridTextBoxColumn1.HeaderText = "Column Name";
			this.dataGridTextBoxColumn1.MappingName = "ColumnName";
			this.dataGridTextBoxColumn1.Width = 75;
			// 
			// dataGridBoolColumn1
			// 
			this.dataGridBoolColumn1.AllowNull = false;
			this.dataGridBoolColumn1.FalseValue = false;
			this.dataGridBoolColumn1.HeaderText = "Allow Nulls";
			this.dataGridBoolColumn1.MappingName = "AllowNull";
			this.dataGridBoolColumn1.NullValue = ((object)(resources.GetObject("dataGridBoolColumn1.NullValue")));
			this.dataGridBoolColumn1.TrueValue = true;
			this.dataGridBoolColumn1.Width = 75;
			// 
			// tabPage3
			// 
			this.tabPage3.Location = new System.Drawing.Point(4, 25);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Size = new System.Drawing.Size(424, 559);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "Indexes";
			// 
			// tabPage4
			// 
			this.tabPage4.Controls.Add(this.sql);
			this.tabPage4.Location = new System.Drawing.Point(4, 25);
			this.tabPage4.Name = "tabPage4";
			this.tabPage4.Size = new System.Drawing.Size(424, 559);
			this.tabPage4.TabIndex = 3;
			this.tabPage4.Text = "SQL";
			// 
			// sql
			// 
			this.sql.Dock = System.Windows.Forms.DockStyle.Fill;
			this.sql.Location = new System.Drawing.Point(0, 0);
			this.sql.Multiline = true;
			this.sql.Name = "sql";
			this.sql.Size = new System.Drawing.Size(424, 559);
			this.sql.TabIndex = 0;
			this.sql.Text = "textBox1";
			// 
			// TableDesigner
			// 
			this.Controls.Add(this.tabControl1);
			this.Name = "TableDesigner";
			this.Size = new System.Drawing.Size(436, 592);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.columnGrid)).EndInit();
			this.tabPage4.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void columnGrid_CurrentCellChanged(object sender, System.EventArgs e)
		{
			int row = columnGrid.CurrentRowIndex;
			this.propertyGrid1.SelectedObject = (TableDesignerColumn)columns[ row ];
		}

		private void ItemChanged(object sender, System.EventArgs e)
		{
			dirty = false;

			if (tableName.Text.Trim() != tableInfo.Name)
				dirty = true;

			EnvDTE.Window w = (EnvDTE.Window)this.Tag;

			if (tableName.Text.Trim().Length > 0)
				w.Caption = tableName.Text.Trim();
			else if (tableInfo.Name.Length > 0)
				w.Caption = tableInfo.Name;
			else
				w.Caption = "New Table";

			if (dirty)
				w.Caption += "*";
		}

	}

	internal class TableInfo 
	{
		public TableInfo() 
		{
			Name = String.Empty;
		}

		public string Name;
	}
}
