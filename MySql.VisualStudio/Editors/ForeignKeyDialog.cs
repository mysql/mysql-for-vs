// Copyright (c) 2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
//
// This file is part of MySQL Tools for Visual Studio.
// MySQL Tools for Visual Studio is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public 
// License version 2.1 as published by the Free Software Foundation
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MySql.Data.VisualStudio.DbObjects;
using System.Collections;
using MySql.Data.VisualStudio.Properties;

namespace MySql.Data.VisualStudio.Editors
{
    partial class ForeignKeyDialog : Form
    {
        TableNode tableNode;
        List<string> columnNames = new List<string>();
        List<string> fkColumnNames = new List<string>();
        const string None = "<None>";

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
            colGridColumn.Items.Add(None);
            foreach (Column c in tableNode.Table.Columns)
            {
                if (c.ColumnName == null) continue;
                columnNames.Add(c.ColumnName);
                colGridColumn.Items.Add(c.ColumnName);
            }

            foreignKeyBindingSource.DataSource = tableNode.Table.ForeignKeys;
            fkList.DataSource = foreignKeyBindingSource;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            ForeignKey key = new ForeignKey(tableNode.Table, null);
            if (refTable.SelectedValue != null)
            {
                key.SetName(String.Format("FK_{0}_{1}", tableNode.Table.Name,
                    refTable.SelectedValue), true);
                key.ReferencedTable = refTable.SelectedValue.ToString();
            }
            foreignKeyBindingSource.Add(key);
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            foreignKeyBindingSource.RemoveCurrent();
        }

        private void refTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            string refTableName = refTable.Items[refTable.SelectedIndex].ToString();
            fkGridColumn.HeaderText = refTableName;

            //reset the items list for the fk column
            string sql = @"SELECT column_name FROM INFORMATION_SCHEMA.COLUMNS WHERE 
                TABLE_SCHEMA='{0}' AND TABLE_NAME='{1}'";
            DataTable dt = tableNode.GetDataTable(String.Format(sql, tableNode.Database, refTableName));
            fkColumnNames.Clear();
            foreach (DataRow row in dt.Rows)
                fkColumnNames.Add(row[0].ToString());

            fkGridColumn.Items.Clear();
            fkGridColumn.Items.Add(None);
            foreach (string col in fkColumnNames)
                fkGridColumn.Items.Add(col);

            if (foreignKeyBindingSource.Current == null) return;

            // update the key name if it is not already finalized
            ForeignKey key = foreignKeyBindingSource.Current as ForeignKey;
            if (!key.NameSet)
            {
                string name = String.Format("FK_{0}_{1}", tableNode.Table.Name, refTableName);
                key.SetName(name, true);
            }
        }

        private void fkName_KeyPress(object sender, KeyPressEventArgs e)
        {
            ForeignKey key = foreignKeyBindingSource.Current as ForeignKey;
            key.NameSet = true;
        }

        private void columnGrid_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            Type t = e.Control.GetType();
            if (t != typeof(DataGridViewComboBoxEditingControl)) return;

            DataGridViewComboBoxEditingControl ec = e.Control as DataGridViewComboBoxEditingControl;
            ec.DrawMode = DrawMode.OwnerDrawFixed;
            ec.DrawItem += new DrawItemEventHandler(dropdown_DrawItem);

            // now update the items that should be seen in this control
            ec.Items.Clear();
            ec.Items.Add(None);
            int index = columnGrid.CurrentCell.ColumnIndex;
            List<string> cols = index == 0 ? columnNames : fkColumnNames;
            ForeignKey key = foreignKeyBindingSource.Current as ForeignKey;

            foreach (string s in cols)
            {
                bool alreadyUsed = false;
                if (s != (string)columnGrid.CurrentCell.Value)
                    foreach (FKColumnPair pair in key.Columns)
                        if ((index == 0 && pair.ReferencedColumn == s) ||
                            (index == 1 && pair.Column == s))
                        {
                            alreadyUsed = true;
                            break;
                        }
                if (!alreadyUsed)
                    ec.Items.Add(s);
            }
            int selIndex = ec.FindStringExact(columnGrid.CurrentCell.Value as string);
            if (selIndex > 0)
                ec.SelectedIndex = selIndex;
        }

        void dropdown_DrawItem(object sender, DrawItemEventArgs e)
        {
            MyComboBox.DrawComboBox(sender as ComboBox, e);
        }

        private void foreignKeyBindingSource_CurrentChanged(object sender, EventArgs e)
        {
            if (foreignKeyBindingSource.Current == null)
            {
                columnGrid.Rows.Clear();
                return;
            }
            ForeignKey key = foreignKeyBindingSource.Current as ForeignKey;
            fkColumnsBindingSource.DataSource = key.Columns;
        }

        private void columnGrid_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            int index = e.ColumnIndex;
            DataGridViewComboBoxCell cell =
                (DataGridViewComboBoxCell)columnGrid.Rows[e.RowIndex].Cells[e.ColumnIndex];

            FKColumnPair pair = fkColumnsBindingSource.Current as FKColumnPair;
            string value = e.FormattedValue as string;

            if (value == None)
            {
                cell.Value = null;
                if (index == 0)
                    pair.ReferencedColumn = null;
                else
                    pair.Column = null;
            }
            else
                cell.Value = e.FormattedValue as string;
        }

        private void columnGrid_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            int index = e.RowIndex;

            DataGridViewCell parentCell = columnGrid.Rows[e.RowIndex].Cells[0];
            DataGridViewCell childCell = columnGrid.Rows[e.RowIndex].Cells[1];
            string parent = parentCell.Value as string;
            string child = childCell.Value as string;

            bool bad = false;
            parentCell.ErrorText = childCell.ErrorText = null;

            if ((String.IsNullOrEmpty(parent) || parent == None) &&
                (!String.IsNullOrEmpty(child) && child != None))
            {
                parentCell.ErrorText = Resources.FKNeedColumn;
                bad = true;
            }
            else if ((String.IsNullOrEmpty(child) || child == None) &&
                (!String.IsNullOrEmpty(parent) && parent != None))
            {
                childCell.ErrorText = Resources.FKNeedColumn;
                bad = true;
            }
            if (bad)
            {
                MessageBox.Show(Resources.FKColumnsNotMatched, null, MessageBoxButtons.OK, MessageBoxIcon.Information);
                e.Cancel = true;
                return;
            }
        }
    }
}
