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
        List<Index> indexes;

        public TableIndexDialog(TableNode node)
        {
            tableNode = node;
            InitializeComponent();
        }

        private void indexList_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool good = indexList.SelectedIndex != -1;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void columnGrid_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control.GetType() != typeof(DataGridViewComboBoxEditingControl)) return;

            DataGridViewComboBoxEditingControl ec = e.Control as DataGridViewComboBoxEditingControl;
            ec.DrawMode = DrawMode.OwnerDrawFixed;
            ec.DrawItem += new DrawItemEventHandler(ec_DrawItem);
        }

        void ec_DrawItem(object sender, DrawItemEventArgs e)
        {
            MyComboBox.DrawComboBox(sender as ComboBox, e);
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            Index i = new Index();
            indexProps.SelectedObject = i;
        }
    }
}
