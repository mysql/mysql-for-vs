using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace MySql.VSTools
{
    internal partial class TableEditor : BaseEditor
    {
        private TableNode table;

        public TableEditor(TableNode table)
        {
            InitializeComponent();

            this.table = table;
            tableName.Text = table.Caption;
            tableSchema.Text = table.Schema;
            tableType.SelectedItem = table.TypeName;
            tableType.SelectedText = table.TypeName;
            dataDirectory.Text = table.DataDirectory;
            indexDirectory.Text = table.IndexDirectory;
            rowFormat.SelectedItem = table.RowFormat;
            useChecksum.Checked = table.UseChecksum;
            avgRowLen.Text = AsString(table.AverageRowLength);
            minRows.Text = AsString(table.MinimumRowCount);
            maxRows.Text = AsString(table.MaximumRowCount);
            
            PopulateGrid();
        }

        private string AsString(object val)
        {
            if (val.Equals(0))
                return String.Empty;
            return val.ToString();
        }

        private void PopulateGrid()
        {
            columnList.Items.Clear();

            ArrayList cols = table.GetColumns();
            foreach (ColumnNode node in cols)
            {
                ListViewItem item = columnList.Items.Add(node.Caption);
                item.SubItems.Add(node.Typename);
                item.SubItems.Add(node.LengthAsString);
                item.SubItems.Add(node.CanBeNull.ToString());
                item.SubItems.Add(node.IsBinary.ToString());
                item.SubItems.Add(node.ZeroFill.ToString());
            }
        }

        private void columnList_SizeChanged(object sender, EventArgs e)
        {

        }

        private void optionsPage_Click(object sender, EventArgs e)
        {

        }

    }
}
