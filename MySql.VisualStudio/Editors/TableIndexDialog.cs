// Copyright © 2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
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

namespace MySql.Data.VisualStudio.Editors
{
    partial class TableIndexDialog : Form
    {
        private TableNode tableNode;
        private Table table;

        public TableIndexDialog(TableNode node)
        {
            tableNode = node;
            table = tableNode.Table;
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
            Index index = table.CreateIndexWithUniqueName(false);
            IndexColumn ic = new IndexColumn();
            ic.OwningIndex = index;
            ic.ColumnName = table.Columns[0].ColumnName;
            ic.SortOrder = IndexSortOrder.Ascending;
            index.Columns.Add(ic);
            table.Indexes.Add(index);
            indexList.SelectedIndex = indexList.Items.Add(index.Name);
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            int index = indexList.SelectedIndex;
            table.Indexes.RemoveAt(index);
            indexList.Items.RemoveAt(index);
            index --;
            if (index == -1 && indexList.Items.Count > 0)
                index = 0;
            indexList.SelectedIndex = index;
        }
    }
}
