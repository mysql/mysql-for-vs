// Copyright © 2008, 2016, Oracle and/or its affiliates. All rights reserved.
//
// MySQL for Visual Studio is licensed under the terms of the GPLv2
// <http://www.gnu.org/licenses/old-licenses/gpl-2.0.html>, like most 
// MySQL Connectors. There are special exceptions to the terms and 
// conditions of the GPLv2 as it is applied to this software, see the 
// FLOSS License Exception
// <http://www.mysql.com/about/legal/licensing/foss-exception.html>.
//
// This program is free software; you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published 
// by the Free Software Foundation; version 2 of the License.
//
// This program is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
//
// You should have received a copy of the GNU General Public License along 
// with this program; if not, write to the Free Software Foundation, Inc., 
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using System;
using System.Windows.Forms;
using MySql.Data.VisualStudio.DbObjects;
using MySql.Data.VisualStudio.Nodes;

namespace MySql.Data.VisualStudio.Editors
{
  partial class TableIndexDialog : Form
  {
    private TableNode _tableNode;
    private Table _table;

    public TableIndexDialog(TableNode node)
    {
      _tableNode = node;
      _table = _tableNode.Table;
      InitializeComponent();

      foreach (Index i in _tableNode.Table.Indexes)
        indexList.Items.Add(i.Name);

      bool isOk = _tableNode.Table.Columns.Count > 0 &&
                  !String.IsNullOrEmpty(_tableNode.Table.Columns[0].ColumnName) &&
                  !String.IsNullOrEmpty(_tableNode.Table.Columns[0].DataType);
      addButton.Enabled = isOk;
      deleteButton.Enabled = false;
      indexList.Enabled = isOk;
    }

    private void indexList_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (indexList.SelectedIndex == -1)
        indexProps.SelectedObject = null;
      else
        indexProps.SelectedObject = _tableNode.Table.Indexes[indexList.SelectedIndex];
      deleteButton.Enabled = indexList.SelectedIndex != -1;
    }

    private void closeButton_Click(object sender, EventArgs e)
    {
      Close();
    }

    private void addButton_Click(object sender, EventArgs e)
    {
      Index index = _table.CreateIndexWithUniqueName(false);
      IndexColumn ic = new IndexColumn();
      ic.OwningIndex = index;
      ic.ColumnName = _table.Columns[0].ColumnName;
      ic.SortOrder = IndexSortOrder.Ascending;
      index.Columns.Add(ic);
      _table.Indexes.Add(index);
      indexList.SelectedIndex = indexList.Items.Add(index.Name);
    }

    private void deleteButton_Click(object sender, EventArgs e)
    {
      int index = indexList.SelectedIndex;
      _table.Indexes.Delete(index);
      indexList.Items.RemoveAt(index);
      index--;
      if (index == -1 && indexList.Items.Count > 0)
        index = 0;
      indexList.SelectedIndex = index;
    }

    private void indexProps_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
    {
      if (e.ChangedItem.PropertyDescriptor != null && e.ChangedItem.PropertyDescriptor.Name == "Name")
        indexList.Items[indexList.SelectedIndex] = e.ChangedItem.Value;
    }
  }
}
