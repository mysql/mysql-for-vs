// Copyright (c) 2021, Oracle and/or its affiliates
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

using MySql.Data.VisualStudio.DbObjects;
using MySql.Utility.Classes.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace MySql.Data.VisualStudio.Editors
{
  /// <summary>
  /// Interaction logic for TableEditorWPF.xaml
  /// Provides an alternate table editor compatible with VS2019+.
  /// </summary>
  public partial class TableEditorWPF : System.Windows.Controls.UserControl
  {
    #region Fields

    /// <summary>
    /// The list of supported MySQL data types.
    /// </summary>
    private string[] _dataTypes;

    /// <summary>
    /// The object representing the Allow Null column.
    /// </summary>
    private DataGridViewCheckBoxColumn _allowNullColumn;

    /// <summary>
    /// The binding source object used to persist the changes to the table columns.
    /// </summary>
    private BindingSource _columnBindingSource;

    /// <summary>
    /// A Windows forms grid view object containing the main column properties.
    /// </summary>
    private DataGridView _columnGrid;

    /// <summary>
    /// A Windows forms column properties object containing the complete list of
    /// editable column properties.
    /// </summary>
    private VS2005PropertyGrid _columnPropertiesGrid;

    /// <summary>
    /// The object representing the Column Name column.
    /// </summary>
    private DataGridViewTextBoxColumn _nameColumn;

    /// <summary>
    /// The Server Explorer node representing the table identified by this table editor.
    /// </summary>
    private TableNode _tableNode;

    /// <summary>
    /// The object representing the Data Type column.
    /// </summary>
    private DataGridViewComboBoxColumn _typeColumn;

    #endregion

    internal TableEditorWPF(TableNode tableNode)
    {
      _tableNode = tableNode;
      tableNode.Saving += delegate { EndGridEditMode(); };
      InitializeComponent();
      Initialize();
      tableNode.DataLoaded += new EventHandler(OnDataLoaded);
      _columnGrid.RowTemplate.HeaderCell = new MyDataGridViewRowHeaderCell();
    }

    #region Properties

    /// <summary>
    /// The indexes assigned to the table being edited.
    /// </summary>
    List<Index> Indexes
    {
      get { return _tableNode.Table.Indexes; }
    }

    /// <summary>
    /// The columns assigned to the table being edited.
    /// </summary>
    List<Column> Columns
    {
      get { return _tableNode.Table.Columns; }
    }

    #endregion

    /// <summary>
    /// Adjusts the Data Types comboBox object to add any variant of the already defined data types.
    /// </summary>
    /// <param name="comboBox">The comboBox object to adjust.</param>
    /// <param name="type">The data type to add.</param>
    private void AdjustComboBox(System.Windows.Forms.ComboBox comboBox, string type)
    {
      if (string.IsNullOrEmpty(type)) return;
      int index = type.IndexOf("(");
      if (index == -1)
      {
        comboBox.Items.Add(type);
      }
      else
      {
        var baseType = type.Substring(0, index);
        for (int i = 0; i < comboBox.Items.Count; i++)
        {
          var item = comboBox.Items[i] as string;
          if (item.StartsWith(baseType, StringComparison.OrdinalIgnoreCase))
          {
            comboBox.Items[i] = type;
            break;
          }
        }
      }
    }

    /// <summary>
    /// Initializes the Windows forms objects not originally handled by WPF.
    /// </summary>
    private void Initialize()
    {
      _columnBindingSource = new BindingSource();
      _columnBindingSource.AllowNew = true;
      _columnBindingSource.DataSource = typeof(Column);

      _columnGrid = new DataGridView();
      _columnGrid.AllowUserToResizeRows = false;
      _columnGrid.AutoGenerateColumns = false;
      _columnGrid.ColumnHeadersHeight = 50;
      _columnGrid.CellClick += new DataGridViewCellEventHandler(ColumnGrid_CellClick);
      _columnGrid.CellContentClick += new DataGridViewCellEventHandler(ColumnGrid_CellContentClick);
      _columnGrid.CellEnter += new DataGridViewCellEventHandler(ColumnGrid_CellEnter);
      _columnGrid.CellLeave += new DataGridViewCellEventHandler(ColumnGrid_CellLeave);
      _columnGrid.CellValidating += new DataGridViewCellValidatingEventHandler(ColumnGrid_CellValidating);
      _columnGrid.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(ColumnGrid_EditingControlShowing);
      _columnGrid.UserDeletingRow += new DataGridViewRowCancelEventHandler(ColumnGrid_UserDeletingRow);
      _columnGrid.DataError += new DataGridViewDataErrorEventHandler(ColumnGrid_DataError);

      _nameColumn = new DataGridViewTextBoxColumn();
      _nameColumn.DataPropertyName = "ColumnName";
      _nameColumn.HeaderText = "Column Name";
      _nameColumn.MinimumWidth = 200;
      _nameColumn.Name = "NameColumn";
      _nameColumn.Width = 200;

      _typeColumn = new DataGridViewComboBoxColumn();
      _typeColumn.DataPropertyName = "DataType";
      _typeColumn.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
      _typeColumn.DisplayStyleForCurrentCellOnly = true;
      _typeColumn.HeaderText = "Data Type";
      _typeColumn.MinimumWidth = 185;
      _typeColumn.Name = "TypeColumn";
      _typeColumn.Width = 185;
      _dataTypes = Metadata.GetDataTypes(true);
      _typeColumn.Items.AddRange((object[])_dataTypes);

      _allowNullColumn = new DataGridViewCheckBoxColumn();
      _allowNullColumn.DataPropertyName = "AllowNull";
      _allowNullColumn.HeaderText = "Allow Nulls";
      _allowNullColumn.MinimumWidth = 130;
      _allowNullColumn.Name = "AllowNullColumn";
      _allowNullColumn.Width = 130;

      _columnGrid.Columns.AddRange(new DataGridViewColumn[] {
            _nameColumn,
            _typeColumn,
            _allowNullColumn});
      _columnGrid.DataSource = _columnBindingSource;
    }

    /// <summary>
    /// Commits a change on the columns grid.
    /// </summary>
    private void EndGridEditMode()
    {
      if (_columnGrid.IsCurrentCellDirty)
      {
        DataGridViewCell c = _columnGrid.CurrentCell;
        DataGridViewCellValidatingEventArgs args =
          (DataGridViewCellValidatingEventArgs)typeof(DataGridViewCellValidatingEventArgs).GetConstructor(
          BindingFlags.NonPublic | BindingFlags.Instance,
          null, new Type[] { typeof(int), typeof(int), typeof(object) }, null).Invoke(new object[] { c.ColumnIndex, c.RowIndex, c.Value });
        ColumnGrid_CellValidating(_columnGrid, args);
        if (!args.Cancel)
        {
          _columnGrid.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }
      }
    }

    /// <summary>
    /// Event handler triggered when a new item is attempted to be added to the columns grid.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">The events object.</param>
    private void ColumnBindingSource_AddingNew(object sender, AddingNewEventArgs e)
    {
      ColumnWithTypeDescriptor column = new ColumnWithTypeDescriptor();
      column.OwningTable = _tableNode.Table;
      e.NewObject = column;
      column.DataType = "varchar(10)";
    }

    /// <summary>
    /// Event handler triggered when an updated list of columns have been retrieved from the database.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">The events object.</param>
    private void Table_DataUpdated(object sender, EventArgs e)
    {
      _columnGrid.Refresh();
    }

    /// <summary>
    /// Event handler triggered when a column is selected in the columns grid.
    /// This allows to keep tracked of the currently selected column.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">The events object.</param>
    private void ColumnBindingSource_CurrentChanged(object sender, EventArgs e)
    {
      var currentObject = _columnBindingSource.Current as Column;
      _columnPropertiesGrid.SelectedObject = currentObject;
    }

    /// <summary>
    /// Event handler triggered when clicking a cell in the columns grid.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">The events object.</param>
    private void ColumnGrid_CellClick(object sender, DataGridViewCellEventArgs e)
    {
      if (e.ColumnIndex == -1)
      {
        _columnGrid.EndEdit();
      }
      else
      {
        _columnGrid.BeginEdit(true);
      }
    }

    /// <summary>
    /// Event handler triggered when clicking on the content of a cell in the columns grid.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">The events object.</param>
    private void ColumnGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {
      if (e.ColumnIndex != 2)
      {
        return;
      }

      DataGridViewCheckBoxCell cell = _columnGrid[e.ColumnIndex, e.RowIndex] as DataGridViewCheckBoxCell;
      Column column = Columns[e.RowIndex];
      column.AllowNull = (bool)cell.EditingCellFormattedValue;
    }

    /// <summary>
    /// Event handler triggered when entering a cell in the columns grid.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">The events object.</param>
    private void ColumnGrid_CellEnter(object sender, DataGridViewCellEventArgs e)
    {
      if (_columnGrid.SelectedCells.Count == 1)
      {
        _columnGrid.BeginEdit(true);
      }
    }

    /// <summary>
    /// Event handler triggered when leaving a cell in the columns grid.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">The events object.</param>
    private void ColumnGrid_CellLeave(object sender, DataGridViewCellEventArgs e)
    {
      if (_columnPropertiesGrid == null)
      {
        return;
      }

      int index = _columnGrid.CurrentRow.Index;
      if (index >= 0 && index < _tableNode.Table.Columns.Count)
      {
        _columnPropertiesGrid.SelectedObject = _tableNode.Table.Columns[index];
      }
    }

    /// <summary>
    /// Event handler used to validate the contents of a cell.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">The events object.</param>
    private void ColumnGrid_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
    {
      if (e.ColumnIndex != 1)
      {
        return;
      }

      string type = e.FormattedValue as string;
      if (string.IsNullOrEmpty(type))
      {
        return;
      }

      if (!_typeColumn.Items.Contains(type))
      {
        var typeToAdd = type.IndexOf('(') >= 0 ? type.Substring(0, type.IndexOf('(')) : type;
        List<string> types = new List<string>(_dataTypes.Length);
        types.AddRange(Metadata.GetDataTypes(false));
        if (types.Contains(typeToAdd.ToLowerInvariant()))
        {
          _typeColumn.Items.Add(type);
        }
        else
        {
          Logger.LogError(Properties.Resources.InvalidDataType, true);
          _columnGrid.CurrentCell.Value = _dataTypes[0];
          _columnGrid.CurrentCell = _columnGrid.Rows[e.RowIndex].Cells[e.ColumnIndex];
          _columnGrid.CurrentCell.Selected = true;
          _columnGrid.BeginEdit(true);
          return;
        }
      }

      _columnGrid.CurrentCell.Value = e.FormattedValue;
    }

    /// <summary>
    /// Event handler for errors raised when commiting changes to the columns grid.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">The events object.</param>
    private void ColumnGrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
    {
      e.ThrowException = false;
      if (e.Context != DataGridViewDataErrorContexts.Display &&
        e.Context != DataGridViewDataErrorContexts.Formatting)
      {
        return;
      }
    }

    /// <summary>
    /// Event handler triggered when an editable object in the columns grid is selected.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">The events object.</param>
    private void ColumnGrid_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
    {
      Type t = e.Control.GetType();
      if (t == typeof(DataGridViewComboBoxEditingControl))
      {
        DataGridViewComboBoxEditingControl ec = e.Control as DataGridViewComboBoxEditingControl;
        ec.DropDownStyle = ComboBoxStyle.DropDown;
        _columnGrid.NotifyCurrentCellDirty(true);
        ec.Items.Clear();
        foreach (string s in _dataTypes)
        {
          ec.Items.Add(s);
        }

        if (_tableNode.Table.Columns.Count > _columnGrid.CurrentRow.Index)
        {
          Column c = _tableNode.Table.Columns[_columnGrid.CurrentRow.Index];
          AdjustComboBox(ec, c.DataType);
        }

        ec.TextChanged += new EventHandler(DataType_TextChanged);
      }
      else if (t == typeof(DataGridViewTextBoxEditingControl))
      {
        DataGridViewTextBoxEditingControl tec = e.Control as DataGridViewTextBoxEditingControl;
        tec.Multiline = true;
        tec.Dock = DockStyle.Fill;
        tec.BorderStyle = BorderStyle.Fixed3D;
        tec.TextChanged += new EventHandler(ColumnName_TextChanged);
      }
    }

    /// <summary>
    /// Event handler triggered when removing a row from the columns grid.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">The events object.</param>
    private void ColumnGrid_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
    {
      Column column = Columns[e.Row.Index];
      if (column.OldColumn != null)
      {
        _tableNode.Table.Columns.Delete(column);
      }
    }

    /// <summary>
    /// Event handler for changes in the text in the Column Name text box for a specific column.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">The events object.</param>
    private void ColumnName_TextChanged(object sender, EventArgs e)
    {
      if ((string)_columnGrid.CurrentCell.Value != ((DataGridViewTextBoxEditingControl)sender).Text)
      {
        _tableNode.Table.Columns[_columnGrid.CurrentRow.Index].ColumnName = ((DataGridViewTextBoxEditingControl)sender).Text;
      }
    }

    /// <summary>
    /// Event handler triggered when a value is changed in the column properties Windows forms object for a specific column.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">The events object.</param>
    private void ColumnPropertiesGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
    {
      if (e.ChangedItem.PropertyDescriptor.Name == "PrimaryKey")
      {
        _columnGrid.Refresh();
      }
    }

    /// <summary>
    /// Event handler triggered when the column properties Windows forms object has been loaded..
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">The events object.</param>
    private void ColumnPropertiesTab_Loaded(object sender, RoutedEventArgs e)
    {
      var host = new WindowsFormsHost();
      _columnPropertiesGrid = new VS2005PropertyGrid();
      host.Child = _columnPropertiesGrid;
      TabGrid.Children.Add(host);
      _columnPropertiesGrid.PropertyValueChanged += new PropertyValueChangedEventHandler(ColumnPropertiesGrid_PropertyValueChanged);
      _columnBindingSource.CurrentChanged += new EventHandler(ColumnBindingSource_CurrentChanged);
      ColumnBindingSource_CurrentChanged(this, EventArgs.Empty);
    }

    /// <summary>
    /// Event handler for changes in the text in the Data Type combobox for a specific column.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">The events object.</param>
    private void DataType_TextChanged(object sender, EventArgs e)
    {
      Column column = _tableNode.Table.Columns[_columnGrid.CurrentRow.Index];
      string oldType = column.DataType;
      string newType = ((DataGridViewComboBoxEditingControl)sender).Text;
      if (!string.IsNullOrEmpty(newType))
      {
        column.DataType = newType;
        if (oldType != column.DataType)
        {
          // Reset properties, since some may not make sense after change of type.
          column.ResetProperties();
        }
      }
    }

    /// <summary>
    /// Event handler triggered when the main WPF grid has been loaded.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">The events object.</param>
    private void Grid_Loaded(object sender, RoutedEventArgs e)
    {
      var host = new WindowsFormsHost();
      host.Child = _columnGrid;
      ColumnPanel.Children.Add(host);
    }

    /// <summary>
    /// Event handler triggered whenever the column data is loaded in the columns grid.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">The events object.</param>
    private void OnDataLoaded(object sender, EventArgs e)
    {
      _columnBindingSource.DataSource = _tableNode.Table.Columns;
      _columnBindingSource.AddingNew -= new AddingNewEventHandler(ColumnBindingSource_AddingNew);
      _columnBindingSource.AddingNew += new AddingNewEventHandler(ColumnBindingSource_AddingNew);

      // If our type column doesn't already contain our data types then we add
      // them so our grid won't complain about unsupported values.
      foreach (Column column in _tableNode.Table.Columns)
      {
        if (!_typeColumn.Items.Contains(column.DataType))
        {
          _typeColumn.Items.Add(column.DataType);
        }
      }

      _tableNode.Table.DataUpdated -= new EventHandler(Table_DataUpdated);
      _tableNode.Table.DataUpdated += new EventHandler(Table_DataUpdated);
    }
  }
}
