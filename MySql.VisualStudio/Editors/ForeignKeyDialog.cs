using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MySql.Data.VisualStudio.DbObjects;
using System.Collections;

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

            // create a list of all tables in this database
            DataTable dt = tableNode.GetDataTable(
                String.Format(@"SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES 
                  WHERE TABLE_SCHEMA = '{0}' AND TABLE_NAME <> '{1}'",
                  tableNode.Database, tableNode.Table.Name));
            List<string> tables = new List<string>();
            foreach (DataRow row in dt.Rows)
                tables.Add(row[0].ToString());
            refTable.DataSource = tables;

            colGridColumn.HeaderText = tableNode.Table.Name;

            foreignKeyBindingSource.DataSource = tableNode.Table.ForeignKeys;
            fkList.DataSource = foreignKeyBindingSource;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void fkList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //foreignKeyBindingSource.Position = fkList.SelectedIndex;
/*            bool good = fkList.SelectedIndex != -1;
            deleteButton.Enabled = good;
            if (!good) return;

            ForeignKey key = tableNode.Table.ForeignKeys[fkList.SelectedIndex];
            fkName.Text = key.Name;
            updateAction.SelectedValue = key.UpdateAction;
            deleteAction.SelectedValue = key.DeleteAction;
            refTable.Items.Clear();
            foreach (string table in tables)
                if (String.Compare(table, tableNode.Table.Name, true) != 0)
                    refTable.Items.Add(table);
            refTable.SelectedValue = key.ReferencedTable;*/
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            ForeignKey key = new ForeignKey(tableNode.Table);
            foreignKeyBindingSource.Add(key);
        }

        private void ClearControls()
        {
            fkName.Text = String.Empty;
            refTable.SelectedIndex = -1;
            updateAction.SelectedIndex = -1;
            deleteAction.SelectedIndex = -1;
            matchType.SelectedIndex = -1;
            columnGrid.Rows.Clear();
            fkGridColumn.HeaderText = String.Empty;
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            tableNode.Table.ForeignKeys.RemoveAt(fkList.SelectedIndex);
            int index = fkList.SelectedIndex;
            fkList.Items.RemoveAt(index);
            index--;
            if (index == -1 && fkList.Items.Count > 0)
                fkList.SelectedIndex = 0;
            else if (index > -1)
                fkList.SelectedIndex = index;

            if (fkList.SelectedIndex == -1)
                ClearControls();
        }

        private void refTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            string refTableName = refTable.Items[refTable.SelectedIndex].ToString();
            fkGridColumn.HeaderText = refTableName;
            if (foreignKeyBindingSource.Current == null) return;

            ForeignKey key = foreignKeyBindingSource.Current as ForeignKey;
            if (key.NameSet) return;
            string name = String.Format("FK_{0}_{1}", tableNode.Table.Name, refTableName);
            key.SetName(name, true);
        }

        private void fkName_KeyPress(object sender, KeyPressEventArgs e)
        {
            ForeignKey key = foreignKeyBindingSource.Current as ForeignKey;
            key.NameSet = true;
        }
    }
}
