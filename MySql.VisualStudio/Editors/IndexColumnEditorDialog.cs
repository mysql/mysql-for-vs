using System;
using System.Windows.Forms;
using System.Drawing.Design;
using MySql.Data.VisualStudio.DbObjects;
using System.Collections.Generic;
using System.ComponentModel;

namespace MySql.Data.VisualStudio.Editors
{
    internal partial class IndexColumnEditorDialog : Form
    {
        private Table table;
        private List<IndexColumn> indexColumns;
        private List<IndexColumnGridRow> gridRows = new List<IndexColumnGridRow>();
        private List<string> columnNames = new List<string>();

        public IndexColumnEditorDialog(List<IndexColumn> ic)
        {
            table = ic[0].OwningIndex.Table;
            indexColumns = ic;
            InitializeComponent();

            foreach (Column c in table.Columns)
            {
                if (!String.IsNullOrEmpty(c.ColumnName))
                    columnNames.Add(c.ColumnName);
            }

            for (int i=0; i < indexColumns.Count; i++)
            {
                IndexColumnGridRow row = new IndexColumnGridRow();
                row.ColumnName = indexColumns[i].ColumnName;
                row.SortOrder = indexColumns[i].SortOrder.ToString();
                gridRows.Add(row);
            }

            columnName.Items.Add("<None>");
            columnName.Items.AddRange((object[])columnNames.ToArray());

            sortOrder.Items.Add("Ascending");
            sortOrder.Items.Add("Descending");

            indexColumnBindingSource.DataSource = gridRows;
        }

        private void indexGrid_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            Type t = e.Control.GetType();
            if (t != typeof(DataGridViewComboBoxEditingControl)) return;

            DataGridViewComboBoxEditingControl ec = e.Control as DataGridViewComboBoxEditingControl;
            ec.DrawMode = DrawMode.OwnerDrawFixed;
            ec.DrawItem += new DrawItemEventHandler(dropdown_DrawItem);

            if (indexGrid.CurrentCell.ColumnIndex == 0)
            {
                // now we need to set the item list to all non used columns and the option of 
                // NONE
                ec.Items.Clear();
                ec.Items.Add("<None>");
                foreach (string s in columnNames)
                {
                    bool alreadyUsed = false;
                    if (s != (string)indexGrid.CurrentRow.Cells[0].Value)
                        foreach (IndexColumnGridRow row in gridRows)
                            if (row.ColumnName == s)
                            {
                                alreadyUsed = true;
                                break;
                            }
                    if (!alreadyUsed)
                        ec.Items.Add(s);
                }
                int index = ec.FindStringExact(indexGrid.CurrentRow.Cells[0].Value as string);
                if (index > 0)
                    ec.SelectedIndex = index;
            }
        }

        void dropdown_DrawItem(object sender, DrawItemEventArgs e)
        {
            MyComboBox.DrawComboBox(sender as ComboBox, e);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            indexColumns.Clear();
            foreach (IndexColumnGridRow row in gridRows)
            {
                if (String.IsNullOrEmpty(row.ColumnName)) continue;
                IndexColumn ic = new IndexColumn();
                ic.ColumnName = row.ColumnName;
                ic.SortOrder = (IndexSortOrder)Enum.Parse(typeof(IndexSortOrder), row.SortOrder);
                indexColumns.Add(ic);
            }
        }

        private void indexGrid_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex != 0) return;

            System.Diagnostics.Trace.WriteLine("Validate starting");
            DataGridViewComboBoxCell colCell = 
                (DataGridViewComboBoxCell)indexGrid.Rows[e.RowIndex].Cells[0];
            DataGridViewComboBoxCell sortCell = 
                (DataGridViewComboBoxCell)indexGrid.Rows[e.RowIndex].Cells[1];

            IndexColumnGridRow gr = indexColumnBindingSource.Current as IndexColumnGridRow;
            string value = e.FormattedValue as string;

            if (value == "<None>")
            {
                colCell.Value = null;
                sortCell.Value = null;
                gr.ColumnName = null;
                gr.SortOrder = null;
            }
            else
            {
                colCell.Value = e.FormattedValue;

                if (!String.IsNullOrEmpty(colCell.Value as string) && 
                    String.IsNullOrEmpty(sortCell.Value as string))
                    gr.SortOrder = "Ascending";
                sortCell.Value = gr.SortOrder;
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            String s = "my value";
            int i = 0;
        }

        private void IndexColumnEditorDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            int i = 0;
            CloseReason r = e.CloseReason;
        }

        private void indexGrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            int i = e.ColumnIndex;
        }

        private void IndexColumnEditorDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            int i = 0;
            CloseReason r = e.CloseReason;
        }
    }

    public class IndexColumnEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context,
            IServiceProvider provider, object value)
        {
            List<IndexColumn> ic = value as List<IndexColumn>;
            Index index = ic[0].OwningIndex;
            Table t = index.Table;
            IndexColumnEditorDialog dlg = new IndexColumnEditorDialog(ic);
            DialogResult result = dlg.ShowDialog();
            if (index.Type != IndexType.Primary) return value;
            foreach (Column c in t.Columns)
                c.PrimaryKey = false;
            foreach (IndexColumn i in ic)
            {
                i.OwningIndex = index;
                foreach (Column c in t.Columns)
                    if (c.ColumnName == i.ColumnName)
                        c.PrimaryKey = true;
            }
            t.NotifyUpdate();
            return value;
        }
    }

    public class IndexColumnGridRow
    {
        public string ColumnName { get; set; }
        public string SortOrder { get; set; }
    }

}
