using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MySql.Data.VisualStudio.Editors
{
    partial class ForeignKeyDialog : Form
    {
        TableNode tableNode;

        public ForeignKeyDialog(TableNode node)
        {
            tableNode = node;
            Application.EnableVisualStyles();
            InitializeComponent();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void fkList_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool good = fkList.SelectedIndex != -1;
            fkName.Enabled = good;
            refTable.Enabled = good;
            updateAction.Enabled = good;
            //deleteAction.Enabled = good;
            columnGrid.Enabled = good;
        }
    }
}
