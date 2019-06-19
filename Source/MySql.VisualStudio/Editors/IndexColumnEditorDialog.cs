// Copyright (c) 2008, 2019, Oracle and/or its affiliates. All rights reserved.
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
using System.Windows.Forms;
using System.Drawing.Design;
using MySql.Data.VisualStudio.DbObjects;
using System.Collections.Generic;
using System.ComponentModel;
using MySql.Utility.Classes.Logging;

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
      
      InitializeComponent();
      table = ic[0].OwningIndex.Table;
      indexColumns = ic;

      foreach (Column c in table.Columns)
      {
        if (!String.IsNullOrEmpty(c.ColumnName))
          columnNames.Add(c.ColumnName);
      }

      for (int i = 0; i < indexColumns.Count; i++)
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
        ic.OwningIndex = new Index(table, null);
        ic.SortOrder = (IndexSortOrder)Enum.Parse(typeof(IndexSortOrder), row.SortOrder);
        indexColumns.Add(ic);
      }
    }

    private void indexGrid_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
    {
      if (e.ColumnIndex != 0) return;

      Logger.LogInformation("Validation starting...");
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
      if (index.Type != IndexType.Primary)
        return value;
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
    private string _columnName;
    public string ColumnName
    {
      get { return _columnName; }
      set { _columnName = value; }
    }

    private string _sortOrder;
    public string SortOrder
    {
      get { return _sortOrder; }
      set { _sortOrder = value; }
    }
  }

}
