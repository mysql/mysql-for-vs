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

namespace MySql.Data.VisualStudio
{
    [Guid("7363513B-298D-49eb-B9D9-264CE6C47540")]
	class TableEditor : BaseEditor
	{
        private DataGridView columnGrid;
        private VS2005PropertyGrid columnProperties;
        private TableNode tableNode;
        private BindingSource columnBindingSource;
        private SplitContainer splitContainer1;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private MyDataGridViewTextBoxColumn NameColumn;
        private MyDataGridViewComboBoxColumn TypeColumn;
        private DataGridViewCheckBoxColumn AllowNullColumn;
        private System.ComponentModel.IContainer components;

		public TableEditor(TableNode node)
		{
			tableNode = node;
			InitializeComponent();
			columnGrid.AutoGenerateColumns = false;
            string[] types = Metadata.GetDataTypes(false);
            TypeColumn.Items.AddRange((object[])types); //Metadata.GetDataTypes(false));
            columnGrid.EditingPanel.BackColor = Color.Red;
            tableNode.DataLoaded += new EventHandler(OnDataLoaded);
		}

		void OnDataLoaded(object sender, EventArgs e)
		{
            columnBindingSource.DataSource = tableNode.Table;
            columnBindingSource.DataMember = "Columns";
            columnGrid.DataSource = columnBindingSource;
            SelectObject(tableNode.Table);
		}

		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.columnGrid = new System.Windows.Forms.DataGridView();
            this.columnBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.columnProperties = new MySql.Data.VisualStudio.Editors.VS2005PropertyGrid();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.NameColumn = new MySql.Data.VisualStudio.Editors.MyDataGridViewTextBoxColumn();
            this.TypeColumn = new MyDataGridViewComboBoxColumn();
            this.AllowNullColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.columnGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.columnBindingSource)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // columnGrid
            // 
            this.columnGrid.AllowUserToResizeRows = false;
            this.columnGrid.BackgroundColor = System.Drawing.SystemColors.Window;
            this.columnGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.columnGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.columnGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NameColumn,
            this.TypeColumn,
            this.AllowNullColumn});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.ActiveCaption;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.columnGrid.DefaultCellStyle = dataGridViewCellStyle1;
            this.columnGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.columnGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.columnGrid.Location = new System.Drawing.Point(0, 0);
            this.columnGrid.Margin = new System.Windows.Forms.Padding(0);
            this.columnGrid.MultiSelect = false;
            this.columnGrid.Name = "columnGrid";
            this.columnGrid.RowHeadersWidth = 30;
            this.columnGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.columnGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.columnGrid.Size = new System.Drawing.Size(620, 170);
            this.columnGrid.TabIndex = 1;
            this.columnGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.columnGrid_CellValueChanged);
            this.columnGrid.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.columnGrid_CellValidated);
            this.columnGrid.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.columnGrid_CellValidating);
            this.columnGrid.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.columnGrid_EditingControlShowing);
            this.columnGrid.CurrentCellDirtyStateChanged += new System.EventHandler(this.columnGrid_CurrentCellDirtyStateChanged);
            this.columnGrid.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.columnGrid_CellEnter);
            this.columnGrid.SelectionChanged += new System.EventHandler(this.columnGrid_SelectionChanged);
            // 
            // columnBindingSource
            // 
            this.columnBindingSource.ListChanged += new System.ComponentModel.ListChangedEventHandler(this.columnBindingSource_ListChanged);
            // 
            // columnProperties
            // 
            this.columnProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.columnProperties.Location = new System.Drawing.Point(3, 3);
            this.columnProperties.Name = "columnProperties";
            this.columnProperties.Size = new System.Drawing.Size(596, 203);
            this.columnProperties.TabIndex = 0;
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.columnGrid);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(5);
            this.splitContainer1.Size = new System.Drawing.Size(624, 427);
            this.splitContainer1.SplitterDistance = 174;
            this.splitContainer1.TabIndex = 7;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(5, 5);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(610, 235);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.columnProperties);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(602, 209);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Column Properties";
            this.tabPage1.UseVisualStyleBackColor = true;
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
            this.TypeColumn.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.TypeColumn.HeaderText = "Data Type";
            this.TypeColumn.Name = "TypeColumn";
            // 
            // AllowNullColumn
            // 
            this.AllowNullColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.AllowNullColumn.DataPropertyName = "AllowNull";
            this.AllowNullColumn.HeaderText = "Allow Nulls";
            this.AllowNullColumn.Name = "AllowNullColumn";
            this.AllowNullColumn.Width = 64;
            // 
            // TableEditor
            // 
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.splitContainer1);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "TableEditor";
            this.Size = new System.Drawing.Size(624, 427);
            ((System.ComponentModel.ISupportInitialize)(this.columnGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.columnBindingSource)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.ResumeLayout(false);

		}

        public override OLECMDF GetCommandStatus(Guid group, uint cmdId)
        {
            bool show = false;
            if (group == GuidList.DavinciCommandSet && cmdId == SharedCommands.cmdidIndexesAndKeys)
                show = true;
            else if (group == GuidList.DavinciCommandSet && cmdId == SharedCommands.cmdidForeignKeys)
                show = true;
            else if (group == GuidList.StandardCommandSet && cmdId == SharedCommands.cmdidPrimaryKey)
                show = true;
            else if (group == GuidList.StandardCommandSet && cmdId == SharedCommands.cmdidGenerateChangeScript)
                show = true;
            if (show)
            {
                return OLECMDF.OLECMDF_ENABLED | OLECMDF.OLECMDF_SUPPORTED;
            }
            return base.GetCommandStatus(group, cmdId);
        }

        public override void ExecuteCommand(Guid group, uint cmdId)
        {
            if (group == GuidList.DavinciCommandSet && cmdId == SharedCommands.cmdidIndexesAndKeys)
            {
                TableIndexDialog dlg = new TableIndexDialog(tableNode);
                dlg.ShowDialog();
            }
            else if (group == GuidList.DavinciCommandSet && cmdId == SharedCommands.cmdidForeignKeys)
            {
                ForeignKeyDialog dlg = new ForeignKeyDialog(tableNode);
                dlg.ShowDialog();
            }
            else if (group == GuidList.StandardCommandSet && cmdId == SharedCommands.cmdidGenerateChangeScript)
            {
                GenerateChangeScriptDialog dlg = new GenerateChangeScriptDialog(tableNode);
                dlg.ShowDialog();
            }
        }

        public override void Showing(int showing)
        {
            //ShowToolBar("Table Designer", showing == 1);
        }

		private void columnGrid_SelectionChanged(object sender, EventArgs e)
		{
/*			if (columnGrid.SelectedCells.Count == 0) return;

			if (columnGrid.SelectedCells.Count > 1)
				columnProperties.SelectedObject = null;
			columnProperties.SelectedObject =
				tableNode.Columns[columnGrid.SelectedCells[0].RowIndex];*/
		}

        private void columnGrid_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            Type t = e.Control.GetType();
            if (e.Control.GetType() != typeof(DataGridViewComboBoxEditingControl)) return;

            DataGridViewComboBoxEditingControl ec = e.Control as DataGridViewComboBoxEditingControl;
            ec.DrawMode = DrawMode.OwnerDrawFixed;
            ec.DrawItem += new DrawItemEventHandler(ec_DrawItem);
        }

        void ec_DrawItem(object sender, DrawItemEventArgs e)
        {
            MyComboBox.DrawComboBox(sender as ComboBox, e);
        }

        void ec_KeyUp(object sender, KeyEventArgs e)
        {
//            if (e.KeyCode == Keys.Tab)
  //          {
    //            columnGrid.EndEdit();
      //      }
        }

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
        private string newValue;

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
        }

//            if (e.ColumnIndex != 1) return;
  //          DataGridViewComboBoxCell cell = columnGrid.Rows[e.RowIndex].Cells[e.ColumnIndex] as
    //            DataGridViewComboBoxCell;
      //      cell.Items.Add(e.FormattedValue);
        ///    cell.Value = e.FormattedValue;
           // newValue = (string)e.FormattedValue;
//        }

        private void columnGrid_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
//            if (e.ColumnIndex != 1) return;
  //          DataGridViewComboBoxCell cell = columnGrid.Rows[e.RowIndex].Cells[e.ColumnIndex] as
    //            DataGridViewComboBoxCell;
      //      cell.Items.Add(newValue);
        //    cell.Value = newValue;
        }

        private void columnGrid_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
          //  columnGrid.EndEdit();
        }

        private void columnGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
//            if (e.RowIndex == -1) return;

  //                  columnGrid.CurrentCell = columnGrid.Rows[e.RowIndex].Cells[e.ColumnIndex + 1];
        }

        private void columnGrid_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (tableNode.Table.Columns.Count <= e.RowIndex) return;
            Column c = tableNode.Table.Columns[e.RowIndex];
            if (String.IsNullOrEmpty(c.ColumnName))
                columnProperties.SelectedObject = null;
            else
                columnProperties.SelectedObject = c;
        }

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
	}
}
