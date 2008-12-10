using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MySql.Data.VisualStudio.DbObjects;

namespace MySql.Data.VisualStudio.Editors
{
    partial class TableIndexDialog : Form
    {
        private TableNode tableNode;

        public TableIndexDialog(TableNode node)
        {
            tableNode = node;
            InitializeComponent();

            foreach (Index i in tableNode.Table.Indexes)
                indexList.Items.Add(i.Name);

            bool isOk = tableNode.Table.Columns.Count > 0 &&
                        !String.IsNullOrEmpty(tableNode.Table.Columns[0].ColumnName) &&
                        !String.IsNullOrEmpty(tableNode.Table.Columns[0].DataType);
            addButton.Enabled = isOk;
            deleteButton.Enabled = false;
            indexList.Enabled = isOk;
        }

        private void indexList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (indexList.SelectedIndex == -1)
                indexProps.SelectedObject = null;
            else
                indexProps.SelectedObject = tableNode.Table.Indexes[indexList.SelectedIndex];
            deleteButton.Enabled = indexList.SelectedIndex != -1;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            Index index = tableNode.Table.CreateIndexWithUniqueName(false);
            IndexColumn ic = new IndexColumn();
            ic.OwningIndex = index;
            ic.ColumnName = tableNode.Table.Columns[0].ColumnName;
            ic.SortOrder = IndexSortOrder.Ascending;
            index.Columns.Add(ic);
            tableNode.Table.Indexes.Add(index);
            indexList.SelectedIndex = indexList.Items.Add(index.Name);
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            int index = indexList.SelectedIndex;
            tableNode.Table.Indexes.RemoveAt(index);
            indexList.Items.RemoveAt(index);
            index --;
            if (index == -1 && indexList.Items.Count > 0)
                index = 0;
            indexList.SelectedIndex = index;
        }
    }
}
