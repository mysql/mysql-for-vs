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

namespace MySql.Data.MySqlClient.Design
{
	/// <summary>
	/// Summary description for TableDesigner.
	/// </summary>
	public class TableDesigner : System.Windows.Forms.UserControl
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

		private DataTable	columns;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox columnComment;
		private System.Windows.Forms.TextBox defaultValue;
		private System.Windows.Forms.CheckBox autoinc;
		private System.Windows.Forms.CheckBox unsigned;
		private System.Windows.Forms.CheckBox zerofill;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox tableTitle;

//		private DBConnection	dbConn;
		private string			tableName;
		private System.Windows.Forms.TextBox decimals;
		private System.Windows.Forms.DataGridTableStyle dataGridTableStyle1;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn1;
		private DataGridComboBoxColumn dataGridTextBoxColumn2;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn3;
		private System.Windows.Forms.DataGridBoolColumn dataGridBoolColumn1;
		private System.Windows.Forms.CheckBox binary;

		private string[] columnTypes = new string[] { "TINYINT", "SMALLINT",
														"MEDIUMINT", "INT", "BIGINT", "REAL", "DOUBLE", "FLOAT",
														"DECIMAL", "NUMERIC", "CHAR", "VARCHAR", "DATE", "TIME",
														"TIMESTAMP", "DATETIME", "TINYBLOB", "BLOB", "MEDIUMBLOB",
														"LONGBLOB", "TINYTEXT", "TEXT", "MEDIUMTEXT", "LONGTEXT", 
														"ENUM", "SET" };

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public TableDesigner()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			columns = new DataTable();
			columns.Columns.Add( "name", typeof(string));
			columns.Columns.Add( "datatype", typeof(string));
			columns.Columns.Add( "length", typeof(string));
			columns.Columns.Add( "allow_nulls", typeof(bool));
			columns.Columns.Add( "unsigned", typeof(bool));
			columns.Columns.Add( "comment", typeof(string));
			columns.Columns.Add( "auto_inc", typeof(bool));
			columns.Columns.Add( "decimals", typeof(string));
			columns.Columns.Add( "binary", typeof(bool));
			columns.Columns.Add( "default", typeof(string));
			columns.Columns.Add( "zero_fill", typeof(bool));

			columnGrid.DataSource = columns;
			defaultValue.DataBindings.Add( "Text", columns, "default" );
			decimals.DataBindings.Add( "Text", columns, "decimals" );
			autoinc.DataBindings.Add( "Checked", columns, "auto_inc" );
			unsigned.DataBindings.Add( "Checked", columns, "unsigned" );
			zerofill.DataBindings.Add( "Checked", columns, "zero_fill" );

			dataGridTextBoxColumn2.ListSource = columnTypes;

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
			columns.Rows.Clear();
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
			}
		}

		private void LoadTableInfo( DataRow row )
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(TableDesigner));
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.tableTitle = new System.Windows.Forms.TextBox();
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.binary = new System.Windows.Forms.CheckBox();
			this.decimals = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.zerofill = new System.Windows.Forms.CheckBox();
			this.unsigned = new System.Windows.Forms.CheckBox();
			this.autoinc = new System.Windows.Forms.CheckBox();
			this.defaultValue = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.columnComment = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.columnGrid = new System.Windows.Forms.DataGrid();
			this.dataGridTableStyle1 = new System.Windows.Forms.DataGridTableStyle();
			this.dataGridTextBoxColumn1 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridTextBoxColumn2 = new MySql.Data.MySqlClient.Design.DataGridComboBoxColumn();
			this.dataGridTextBoxColumn3 = new System.Windows.Forms.DataGridTextBoxColumn();
			this.dataGridBoolColumn1 = new System.Windows.Forms.DataGridBoolColumn();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.columnGrid)).BeginInit();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Location = new System.Drawing.Point(8, 8);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(420, 352);
			this.tabControl1.TabIndex = 0;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.tableTitle);
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
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Size = new System.Drawing.Size(412, 326);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Properties";
			// 
			// tableTitle
			// 
			this.tableTitle.Location = new System.Drawing.Point(12, 32);
			this.tableTitle.Name = "tableTitle";
			this.tableTitle.Size = new System.Drawing.Size(372, 20);
			this.tableTitle.TabIndex = 18;
			this.tableTitle.Text = "";
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
			this.checksum.TabIndex = 16;
			this.checksum.Text = "Maintain Checksum";
			// 
			// packKeys
			// 
			this.packKeys.Location = new System.Drawing.Point(252, 180);
			this.packKeys.Name = "packKeys";
			this.packKeys.Size = new System.Drawing.Size(116, 24);
			this.packKeys.TabIndex = 15;
			this.packKeys.Text = "Pack Keys";
			// 
			// maxRows
			// 
			this.maxRows.Location = new System.Drawing.Point(124, 188);
			this.maxRows.Name = "maxRows";
			this.maxRows.TabIndex = 14;
			this.maxRows.Text = "";
			// 
			// minRows
			// 
			this.minRows.Location = new System.Drawing.Point(12, 188);
			this.minRows.Name = "minRows";
			this.minRows.TabIndex = 13;
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
			this.delayKeyWrite.TabIndex = 10;
			this.delayKeyWrite.Text = "Delay Key Write";
			// 
			// rowLength
			// 
			this.rowLength.Location = new System.Drawing.Point(124, 136);
			this.rowLength.Name = "rowLength";
			this.rowLength.Size = new System.Drawing.Size(108, 20);
			this.rowLength.TabIndex = 9;
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
			this.autoIncrement.TabIndex = 7;
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
			this.rowFormat.TabIndex = 5;
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
			this.comment.TabIndex = 2;
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
			this.tabPage2.Controls.Add(this.panel1);
			this.tabPage2.Controls.Add(this.splitter1);
			this.tabPage2.Controls.Add(this.columnGrid);
			this.tabPage2.DockPadding.All = 4;
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Size = new System.Drawing.Size(412, 326);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Columns";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.binary);
			this.panel1.Controls.Add(this.decimals);
			this.panel1.Controls.Add(this.label11);
			this.panel1.Controls.Add(this.zerofill);
			this.panel1.Controls.Add(this.unsigned);
			this.panel1.Controls.Add(this.autoinc);
			this.panel1.Controls.Add(this.defaultValue);
			this.panel1.Controls.Add(this.label10);
			this.panel1.Controls.Add(this.columnComment);
			this.panel1.Controls.Add(this.label9);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(4, 175);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(404, 147);
			this.panel1.TabIndex = 2;
			// 
			// binary
			// 
			this.binary.Location = new System.Drawing.Point(32, 116);
			this.binary.Name = "binary";
			this.binary.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.binary.Size = new System.Drawing.Size(96, 16);
			this.binary.TabIndex = 9;
			this.binary.Text = "Binary";
			// 
			// decimals
			// 
			this.decimals.Location = new System.Drawing.Point(92, 56);
			this.decimals.Name = "decimals";
			this.decimals.Size = new System.Drawing.Size(292, 20);
			this.decimals.TabIndex = 8;
			this.decimals.Text = "";
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(12, 60);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(76, 16);
			this.label11.TabIndex = 7;
			this.label11.Text = "Decimals:";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// zerofill
			// 
			this.zerofill.Location = new System.Drawing.Point(284, 92);
			this.zerofill.Name = "zerofill";
			this.zerofill.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.zerofill.Size = new System.Drawing.Size(64, 16);
			this.zerofill.TabIndex = 6;
			this.zerofill.Text = "Zerofill";
			// 
			// unsigned
			// 
			this.unsigned.Location = new System.Drawing.Point(152, 92);
			this.unsigned.Name = "unsigned";
			this.unsigned.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.unsigned.Size = new System.Drawing.Size(96, 16);
			this.unsigned.TabIndex = 5;
			this.unsigned.Text = "Unsigned";
			// 
			// autoinc
			// 
			this.autoinc.Location = new System.Drawing.Point(32, 92);
			this.autoinc.Name = "autoinc";
			this.autoinc.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.autoinc.Size = new System.Drawing.Size(96, 16);
			this.autoinc.TabIndex = 4;
			this.autoinc.Text = "Autoincrement";
			// 
			// defaultValue
			// 
			this.defaultValue.Location = new System.Drawing.Point(92, 32);
			this.defaultValue.Name = "defaultValue";
			this.defaultValue.Size = new System.Drawing.Size(292, 20);
			this.defaultValue.TabIndex = 3;
			this.defaultValue.Text = "";
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(12, 36);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(76, 16);
			this.label10.TabIndex = 2;
			this.label10.Text = "Default Value:";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// columnComment
			// 
			this.columnComment.Enabled = false;
			this.columnComment.Location = new System.Drawing.Point(92, 8);
			this.columnComment.Name = "columnComment";
			this.columnComment.Size = new System.Drawing.Size(292, 20);
			this.columnComment.TabIndex = 1;
			this.columnComment.Text = "";
			// 
			// label9
			// 
			this.label9.Enabled = false;
			this.label9.Location = new System.Drawing.Point(20, 12);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(68, 16);
			this.label9.TabIndex = 0;
			this.label9.Text = "Description:";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// splitter1
			// 
			this.splitter1.BackColor = System.Drawing.SystemColors.ControlDark;
			this.splitter1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
			this.splitter1.Location = new System.Drawing.Point(4, 172);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(404, 3);
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
			this.columnGrid.Size = new System.Drawing.Size(404, 168);
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
																												  this.dataGridTextBoxColumn2,
																												  this.dataGridTextBoxColumn3,
																												  this.dataGridBoolColumn1});
			this.dataGridTableStyle1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dataGridTableStyle1.MappingName = "";
			// 
			// dataGridTextBoxColumn1
			// 
			this.dataGridTextBoxColumn1.Format = "";
			this.dataGridTextBoxColumn1.FormatInfo = null;
			this.dataGridTextBoxColumn1.HeaderText = "Column Name";
			this.dataGridTextBoxColumn1.MappingName = "name";
			this.dataGridTextBoxColumn1.Width = 75;
			// 
			// dataGridTextBoxColumn2
			// 
			this.dataGridTextBoxColumn2.HeaderText = "Data Type";
			this.dataGridTextBoxColumn2.MappingName = "datatype";
			this.dataGridTextBoxColumn2.Width = 75;
			// 
			// dataGridTextBoxColumn3
			// 
			this.dataGridTextBoxColumn3.Format = "";
			this.dataGridTextBoxColumn3.FormatInfo = null;
			this.dataGridTextBoxColumn3.HeaderText = "Length";
			this.dataGridTextBoxColumn3.MappingName = "length";
			this.dataGridTextBoxColumn3.Width = 75;
			// 
			// dataGridBoolColumn1
			// 
			this.dataGridBoolColumn1.FalseValue = false;
			this.dataGridBoolColumn1.HeaderText = "Allow Nulls";
			this.dataGridBoolColumn1.MappingName = "allow_nulls";
			this.dataGridBoolColumn1.NullValue = ((object)(resources.GetObject("dataGridBoolColumn1.NullValue")));
			this.dataGridBoolColumn1.TrueValue = true;
			this.dataGridBoolColumn1.Width = 75;
			// 
			// TableDesigner
			// 
			this.Controls.Add(this.tabControl1);
			this.Name = "TableDesigner";
			this.Size = new System.Drawing.Size(436, 368);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.columnGrid)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void columnGrid_CurrentCellChanged(object sender, System.EventArgs e)
		{
			DataRow row = columns.Rows[columnGrid.CurrentRowIndex];
			string type = row["datatype"].ToString().ToLower();
			autoinc.Enabled = (type.IndexOf("int") != -1);
			binary.Enabled = (type.IndexOf("char") != -1 ||
				              type.IndexOf("blob") != -1 ||
								type.IndexOf("text") != -1);
			zerofill.Enabled = (type.IndexOf("int") != -1 ||
				type == "real" || type == "double" ||
				type == "float" || type == "decimal" ||
				type == "numeric");
			unsigned.Enabled = zerofill.Enabled;
		}
	}
}
