// Copyright (c) 2008, 2010, Oracle and/or its affiliates. All rights reserved.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License, version 2.0, as
// published by the Free Software Foundation.
//
// This program is also distributed with certain software (including
// but not limited to OpenSSL) that is licensed under separate terms,
// as designated in a particular file or component or in included license
// documentation.  The authors of MySQL hereby grant you an
// additional permission to link the program and your derivative works
// with the separately licensed software that they have included with
// MySQL.
//
// Without limiting anything contained in the foregoing, this file,
// which is part of MySQL for Visual Studio, is also subject to the
// Universal FOSS Exception, version 1.0, a copy of which can be found at
// http://oss.oracle.com/licenses/universal-foss-exception.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU General Public License, version 2.0, for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

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
      table.Indexes.Delete(index);
      indexList.Items.RemoveAt(index);
      index--;
      if (index == -1 && indexList.Items.Count > 0)
        index = 0;
      indexList.SelectedIndex = index;
    }

    private void indexProps_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
    {
      if (e.ChangedItem.PropertyDescriptor.Name == "Name")
        indexList.Items[indexList.SelectedIndex] = e.ChangedItem.Value;
    }
  }
}
