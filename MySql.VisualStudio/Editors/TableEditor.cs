using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using MySql.Data.VisualStudio.Editors;
using System.Drawing;
using System.Collections;
using MySql.Data.VisualStudio.Properties;
using System.ComponentModel;
using Microsoft.VisualStudio.OLE.Interop;
using MySql.Data.VisualStudio.DbObjects;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio;

namespace MySql.Data.VisualStudio
{
	class TableEditor : UserControl
	{
        private DataGridView columnGrid;
        private TableNode tableNode;
        private BindingSource columnBindingSource;
        private System.ComponentModel.IContainer components;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewComboBoxColumn dataGridViewComboBoxColumn1;
        private DataGridViewTextBoxColumn NameColumn;
        private DataGridViewComboBoxColumn TypeColumn;
        private DataGridViewCheckBoxColumn AllowNullColumn;
        private MySplitter splitter1;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private VS2005PropertyGrid columnProperties;
        private Panel panel1;
        private TableEditorPane pane;

		public TableEditor(TableEditorPane pane, TableNode node)
		{
            this.pane = pane;
			tableNode = node;
			InitializeComponent();
			columnGrid.AutoGenerateColumns = false;
            string[] types = Metadata.GetDataTypes(false);
            TypeColumn.Items.AddRange((object[])types); //Metadata.GetDataTypes(false));
            columnGrid.EditingPanel.BackColor = Color.Red;
            tableNode.DataLoaded += new EventHandler(OnDataLoaded);

            SetupCommands();
		}

		void OnDataLoaded(object sender, EventArgs e)
		{
            columnBindingSource.DataSource = tableNode.Table;
            columnBindingSource.DataMember = "Columns";
            columnGrid.DataSource = columnBindingSource;
            pane.SelectObject(tableNode.Table);
		}

		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.columnGrid = new System.Windows.Forms.DataGridView();
            this.NameColumn = new DataGridViewTextBoxColumn();
            this.TypeColumn = new DataGridViewComboBoxColumn();
            this.AllowNullColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.columnBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.splitter1 = new MySql.Data.VisualStudio.Editors.MySplitter();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.columnProperties = new MySql.Data.VisualStudio.Editors.VS2005PropertyGrid();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewComboBoxColumn1 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.columnGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.columnBindingSource)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // columnGrid
            // 
            this.columnGrid.AllowUserToResizeRows = false;
            this.columnGrid.BackgroundColor = System.Drawing.SystemColors.Window;
            this.columnGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.columnGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.columnGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.columnGrid.ColumnHeadersHeight = 24;
            this.columnGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.columnGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NameColumn,
            this.TypeColumn,
            this.AllowNullColumn});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.ActiveCaption;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.columnGrid.DefaultCellStyle = dataGridViewCellStyle2;
            this.columnGrid.Dock = System.Windows.Forms.DockStyle.Top;
            this.columnGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.columnGrid.GridColor = System.Drawing.SystemColors.Control;
            this.columnGrid.Location = new System.Drawing.Point(0, 0);
            this.columnGrid.Margin = new System.Windows.Forms.Padding(0);
            this.columnGrid.MultiSelect = false;
            this.columnGrid.Name = "columnGrid";
            this.columnGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.columnGrid.RowHeadersWidth = 24;
            this.columnGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.columnGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.columnGrid.Size = new System.Drawing.Size(624, 130);
            this.columnGrid.TabIndex = 1;
            this.columnGrid.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.columnGrid_EditingControlShowing);
            // 
            // NameColumn
            // 
            this.NameColumn.DataPropertyName = "ColumnName";
            this.NameColumn.HeaderText = "Column Name";
            this.NameColumn.Name = "NameColumn";
            this.NameColumn.Width = 150;
            // 
            // TypeColumn
            // 
            this.TypeColumn.DataPropertyName = "DataType";
            this.TypeColumn.DisplayStyleForCurrentCellOnly = true;
            this.TypeColumn.HeaderText = "Data Type";
            this.TypeColumn.Name = "TypeColumn";
            // 
            // AllowNullColumn
            // 
            this.AllowNullColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.AllowNullColumn.DataPropertyName = "AllowNull";
            this.AllowNullColumn.HeaderText = "Allow Nulls";
            this.AllowNullColumn.Name = "AllowNullColumn";
            this.AllowNullColumn.Width = 74;
            // 
            // columnBindingSource
            // 
            this.columnBindingSource.ListChanged += new System.ComponentModel.ListChangedEventHandler(this.columnBindingSource_ListChanged);
            // 
            // splitter1
            // 
            this.splitter1.BackColor = System.Drawing.SystemColors.Control;
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 130);
            this.splitter1.MaximumSize = new System.Drawing.Size(0, 6);
            this.splitter1.MinimumSize = new System.Drawing.Size(0, 6);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(624, 6);
            this.splitter1.TabIndex = 8;
            this.splitter1.TabStop = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(6, 6);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(612, 279);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.columnProperties);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(604, 251);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Column Properties";
            // 
            // columnProperties
            // 
            this.columnProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.columnProperties.Location = new System.Drawing.Point(3, 3);
            this.columnProperties.Name = "columnProperties";
            this.columnProperties.Size = new System.Drawing.Size(598, 245);
            this.columnProperties.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.tabControl1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 136);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(6);
            this.panel1.Size = new System.Drawing.Size(624, 291);
            this.panel1.TabIndex = 9;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "ColumnName";
            this.dataGridViewTextBoxColumn1.HeaderText = "Column Name";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.Width = 150;
            // 
            // dataGridViewComboBoxColumn1
            // 
            this.dataGridViewComboBoxColumn1.DataPropertyName = "DataType";
            this.dataGridViewComboBoxColumn1.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.dataGridViewComboBoxColumn1.DisplayStyleForCurrentCellOnly = true;
            this.dataGridViewComboBoxColumn1.HeaderText = "Data Type";
            this.dataGridViewComboBoxColumn1.Name = "dataGridViewComboBoxColumn1";
            // 
            // TableEditor
            // 
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.columnGrid);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "TableEditor";
            this.Size = new System.Drawing.Size(624, 427);
            ((System.ComponentModel.ISupportInitialize)(this.columnGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.columnBindingSource)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #region Command Handling

        private void SetupCommands()
        {
            pane.AddCommand(GuidList.DavinciCommandSet, SharedCommands.cmdidIndexesAndKeys,
                new EventHandler(OnIndexesAndKeys), null);
            pane.AddCommand(GuidList.DavinciCommandSet, SharedCommands.cmdidForeignKeys,
                new EventHandler(OnForeignKeys), null);
            pane.AddCommand(VSConstants.GUID_VSStandardCommandSet97, (int)VSConstants.VSStd97CmdID.PrimaryKey,
                new EventHandler(OnPrimaryKey), new EventHandler(OnQueryPrimaryKey));
            pane.AddCommand(VSConstants.GUID_VSStandardCommandSet97, (int)VSConstants.VSStd97CmdID.GenerateChangeScript,
                new EventHandler(OnGenerateChangeScript), null);
        }

        private void OnQueryPrimaryKey(object sender, EventArgs e)
        {
            OleMenuCommand cmd = sender as OleMenuCommand;
            cmd.Enabled = false;
        }

        private void OnPrimaryKey(object sender, EventArgs e)
        {
        }

        private void OnIndexesAndKeys(object sender, EventArgs e)
        {
            TableIndexDialog dlg = new TableIndexDialog(tableNode);
            dlg.ShowDialog();
        }

        private void OnForeignKeys(object sender, EventArgs e)
        {
            ForeignKeyDialog dlg = new ForeignKeyDialog(tableNode);
            dlg.ShowDialog();
        }

        private void OnGenerateChangeScript(object sender, EventArgs e)
        {
            GenerateChangeScriptDialog dlg = new GenerateChangeScriptDialog(tableNode);
            dlg.ShowDialog();
        }

        #endregion

		private void columnGrid_SelectionChanged(object sender, EventArgs e)
		{
			if (columnGrid.SelectedCells.Count == 0) return;

			if (columnGrid.SelectedCells.Count > 1)
				columnProperties.SelectedObject = null;
//			columnProperties.SelectedObject =
//				tableNode.Columns[columnGrid.SelectedCells[0].RowIndex];
		}

        private void columnGrid_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            Type t = e.Control.GetType();
            if (t == typeof(DataGridViewComboBoxEditingControl))
            {
                DataGridViewComboBoxEditingControl ec = e.Control as DataGridViewComboBoxEditingControl;
                ec.DropDownStyle = ComboBoxStyle.DropDown;
            }
            else if (t == typeof(DataGridViewTextBoxEditingControl))
            {
                DataGridViewTextBoxEditingControl tec = e.Control as DataGridViewTextBoxEditingControl;
                tec.Multiline = true;
                tec.Dock = DockStyle.Fill;
                tec.BorderStyle = BorderStyle.Fixed3D;
            }
        }

//        void ec_KeyUp(object sender, KeyEventArgs e)
  //      {
//            if (e.KeyCode == Keys.Tab)
  //          {
    //            columnGrid.EndEdit();
      //      }
    //    }

/*        void columnTypeValidating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DataGridViewComboBoxEditingControl cbo = sender as DataGridViewComboBoxEditingControl;
            DataGridView grid = cbo.EditingControlDataGridView;
            string value = cbo.Text;
            if (true)  // replace this with a test that checks the validity of the type
            {
                DataGridViewComboBoxColumn cboCol = grid.Columns[grid.CurrentCell.ColumnIndex] as DataGridViewComboBoxColumn;
                // Must add to both the current combobox as well as the template, to avoid duplicate entries...
                cbo.Items.Add(value);
                cboCol.Items.Add(value);
                grid.CurrentCell.Value = value;
            }
        }
        */
/*        private string newValue;

        private void columnGrid_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            // if we are not in the first cell then we are not interested
            if (e.ColumnIndex == 0)
                ValidatingColumnName(e);
//            else if (e.ColumnIndex == 1)
 //               ValidatingColumnType(e);
        }

        private void ValidatingColumnName(DataGridViewCellValidatingEventArgs e)
        {
            DataGridViewCell cell = columnGrid.Rows[e.RowIndex].Cells[e.ColumnIndex];
            object cellValue = cell.EditedFormattedValue;
            Column column = tableNode.Table.Columns[e.RowIndex];
            if (String.IsNullOrEmpty(cellValue as string) && !String.IsNullOrEmpty(column.ColumnName))
            {
                DialogResult result = MessageBox.Show(Resources.ColumnWillBeDeleted, "MySQL",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                if (result == DialogResult.OK)
                    columnBindingSource.RemoveCurrent();
                else
                {
                    cell.Value = tableNode.Table.Columns[e.RowIndex].ColumnName;
                    columnGrid.BeginEdit(true);
                    e.Cancel = true;
                }
            }
        }*/

//            if (e.ColumnIndex != 1) return;
  //          DataGridViewComboBoxCell cell = columnGrid.Rows[e.RowIndex].Cells[e.ColumnIndex] as
    //            DataGridViewComboBoxCell;
      //      cell.Items.Add(e.FormattedValue);
        ///    cell.Value = e.FormattedValue;
           // newValue = (string)e.FormattedValue;
//        }

//        private void columnGrid_CellValidated(object sender, DataGridViewCellEventArgs e)
  //      {
//            if (e.ColumnIndex != 1) return;
  //          DataGridViewComboBoxCell cell = columnGrid.Rows[e.RowIndex].Cells[e.ColumnIndex] as
    //            DataGridViewComboBoxCell;
      //      cell.Items.Add(newValue);
        //    cell.Value = newValue;
    //    }

//        private void columnGrid_CurrentCellDirtyStateChanged(object sender, EventArgs e)
  //      {
          //  columnGrid.EndEdit();
    //    }

      //  private void columnGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        //{
//            if (e.RowIndex == -1) return;

  //                  columnGrid.CurrentCell = columnGrid.Rows[e.RowIndex].Cells[e.ColumnIndex + 1];
        //}
        /*
        private void columnGrid_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (tableNode.Table.Columns.Count <= e.RowIndex) return;
            Column c = tableNode.Table.Columns[e.RowIndex];
            if (String.IsNullOrEmpty(c.ColumnName))
                columnProperties.SelectedObject = null;
            else
                columnProperties.SelectedObject = c;
        }
        */
        /// <summary>
        /// This notifies us that we have added a column to our table.  We do it this way
        /// because we have to set the owning table of new columns and handling the
        /// adding new event handler doesn't *seem* to work.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void columnBindingSource_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType != ListChangedType.ItemAdded) return;
            Column c = tableNode.Table.Columns[e.NewIndex];
            c.OwningTable = tableNode.Table;
        }

        private void columnGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void columnGrid_CellValidated(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void columnGrid_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {

        }

        private void columnGrid_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {

        }

        private void columnGrid_CellEnter(object sender, DataGridViewCellEventArgs e)
        {

        }
	}
}
