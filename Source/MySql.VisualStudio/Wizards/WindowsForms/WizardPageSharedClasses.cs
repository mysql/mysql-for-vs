// Copyright (c) 2008, 2021, Oracle and/or its affiliates.
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

using MySql.Data.MySqlClient;
using MySql.Data.VisualStudio.SchemaComparer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace MySql.Data.VisualStudio.Wizards
{

  internal class ColumnValidation
  {
    private Column _column;
    private double? _maxValue;
    private double? _minValue;
    private bool _required;
    private ColumnType _columnType;
    private object _defaultValue;
    private bool _hasLookup;
    private string _lookupColumn;
    private ForeignKeyColumnInfo _fkInfo;
    private string _efColumnMapping;
    private string _efLookupColumnMapping;
    private int? _maxLength;

    // It is important to make the properties public, otherwise DataGridView doesn't like and doesn't display the real values
    // (this seems to be a known issue with DataGridView over stackoverflow).
    public Column Column { get { return _column; } }
    public double? MaxValue { get { return _maxValue; } set { _maxValue = value; } }
    public double? MinValue { get { return _minValue; } set { _minValue = value; } }
    public bool Required { get { return _required; } set { _required = value; } }
    public ColumnType ColumnType { get { return _columnType; } set { _columnType = value; } }
    public object DefaultValue { get { return _defaultValue; } set { _defaultValue = value; } }
    public string Name { get { return _column.ColumnName; } set { _column.ColumnName = value; } }
    public string DataType { get { return _column.DataType; } set { _column.DataType = value; } }
    public bool HasLookup { get { return _hasLookup; } set { _hasLookup = value; } }
    public string LookupColumn { get { return _lookupColumn; } set { _lookupColumn = value; } }
    public ForeignKeyColumnInfo FkInfo { get { return _fkInfo; } set { _fkInfo = value; } }
    public List<string> ReferenceableColumns { get { return _fkInfo.ReferenceableColumns; } }
    public string EfColumnMapping { 
      get {
        return _efColumnMapping; 
      } 
      set 
      { 
        _efColumnMapping = value; 
      } 
    }
    public string EfLookupColumnMapping
    {
      get { return _efLookupColumnMapping; }
      set { _efLookupColumnMapping = value; }
    }
    public int? MaxLength { get { return _maxLength; } set { _maxLength = value; } }

    internal ColumnValidation(Column column)
    {
      _column = column;
    }

    internal bool IsNumericType()
    {
      return IsFloatingPointType() || IsIntegerType();
    }

    internal bool IsFloatingPointType()
    {
      string dt = DataType;
      if (dt == "decimal" || dt == "numeric" || dt == "float" || dt == "double")
      {
        return true;
      }
      return false;
    }

    internal bool IsIntegerType()
    {
      string dt = DataType;
      if (dt == "int" || dt == "integer" || dt == "smallint" || dt == "tinyint" || dt == "mediumint" || dt == "bigint")
      {
        return true;
      }
      return false;
    }

    internal bool IsDateType()
    {
      return Column.IsDateType();
    }

    internal bool IsDateTimeType()
    {
      return Column.IsDateTimeType();
    }

    internal bool IsReadOnly()
    {
      return Column.IsReadOnly();
    }

    internal bool IsBooleanType()
    {
      return Column.IsBooleanType();
    }
  }

  internal enum ColumnType : int
  {
    Text = 1,       // TextBox
    DateTime = 2    // DatePicker
  }

  internal enum DataAccessTechnology : int
  {
    None = 0,
    TypedDataSet = 1,
    EntityFramework5 = 2,
    EntityFramework6 = 3
  }

  internal enum GuiType : int
  {
    None = 0,
    IndividualControls = 1,
    Grid = 2,
    MasterDetail = 3
  }

  internal class MyListItem
  {
    public string Value { get; set; }
    public string Name { get; set; }

    internal MyListItem(string Name, string Value)
    {
      this.Name = Name;
      this.Value = Value;
    }
  }

  internal static class ValidationsGrid
  {
    internal static int IdxColName { get; private set; }
    internal static int IdxColRequired { get; private set; }
    internal static int IdxColDataType { get; private set; }
    internal static int IdxColDefaultValue { get; private set; }
    internal static int IdxColMaxLength { get; private set; }
    internal static int IdxColMinValue { get; private set; }
    internal static int IdxColMaxValue { get; private set; }
    internal static int IdxColHaslookup { get; private set; }
    internal static int IdxColLookupColumn { get; private set; }

    private static Dictionary<string, List<ColumnValidation>> _metadata = new Dictionary<string, List<ColumnValidation>>();

    internal static void ClearMetadataCache()
    {
      _metadata.Clear();
    }

    internal static List<ColumnValidation> GetColumnValidationList(
      string table,
      Dictionary<string, Column> columns, Dictionary<string, ForeignKeyColumnInfo> FKs)
    {
      List<ColumnValidation> colsValidation = null;
      // First examine the cache
      if (_metadata.TryGetValue(table, out colsValidation)) return colsValidation;
      colsValidation = new List<ColumnValidation>();
      foreach (KeyValuePair<string, Column> kvp in columns)
      {
        ColumnValidation cv = new ColumnValidation(kvp.Value);
        cv.ColumnType = ColumnType.Text;
        cv.DefaultValue = "";
        cv.MinValue = null;
        cv.MaxValue = null;
        cv.Required = true;
        ForeignKeyColumnInfo fk = null;
        if (FKs != null)
        {
          if (FKs.TryGetValue(cv.Column.ColumnName, out fk))
          {
            cv.HasLookup = true;
            cv.LookupColumn = fk.ReferencedColumnName;
            cv.FkInfo = fk;
          }
        }
        colsValidation.Add(cv);
      }
      _metadata[table] = colsValidation;
      return colsValidation;
    }
   
    internal static void LoadGridColumns(DataGridView grid, MySqlConnection con, string Table,
      List<ColumnValidation> colsValidation, Dictionary<string, Column> columns,
      Dictionary<string, ForeignKeyColumnInfo> FKs)
    {
#if NET_461_OR_GREATER
      SortedSet<string> allColumns = new SortedSet<string>();
      BindingSource binding = new BindingSource();
      
      //reset grid
      grid.DataSource = null;
      grid.Columns.Clear();
      grid.Rows.Clear();
      grid.Update();

      for (int i = 0; i < colsValidation.Count; i++)
      {
        ColumnValidation cv = colsValidation[i];
        if (FKs != null)
        {
          ForeignKeyColumnInfo fk = null;
          if (FKs.TryGetValue(cv.Column.ColumnName, out fk))
          {
            allColumns.UnionWith(fk.ReferenceableColumns);
          }
        }
      }

      grid.AutoGenerateColumns = false;
      
      DataGridViewTextBoxColumn colName = new DataGridViewTextBoxColumn();
      colName.DataPropertyName = "Name";
      colName.HeaderText = "Column Name";
      colName.Name = "colName";
      colName.ReadOnly = true;
      grid.Columns.Add(colName);
      IdxColName = colName.Index;

      DataGridViewCheckBoxColumn colRequired = new DataGridViewCheckBoxColumn();
      colRequired.DataPropertyName = "Required";
      colRequired.HeaderText = "Req.";
      colRequired.ToolTipText = "Required";
      colRequired.Name = "colRequired";
      colRequired.ReadOnly = true;
      grid.Columns.Add(colRequired);
      IdxColRequired = colRequired.Index;

      DataGridViewTextBoxColumn colDataType = new DataGridViewTextBoxColumn();
      colDataType.DataPropertyName = "DataType";
      colDataType.HeaderText = "Data Type";
      colDataType.ToolTipText = "Data Type";
      colDataType.Name = "colDataType";
      colDataType.ReadOnly = true;
      grid.Columns.Add(colDataType);
      IdxColDataType = colDataType.Index;

      DataGridViewTextBoxColumn colDefaultValue = new DataGridViewTextBoxColumn();
      colDefaultValue.DataPropertyName = "DefaultValue";
      colDefaultValue.HeaderText = "Def.";
      colDefaultValue.ToolTipText = "Default value";
      colDefaultValue.Name = "colDefaultValue";
      colDefaultValue.Width = 90;
      grid.Columns.Add(colDefaultValue);
      IdxColDefaultValue = colDefaultValue.Index;

      DataGridViewTextBoxColumn colMaxLength = new DataGridViewTextBoxColumn();
      colMaxLength.DataPropertyName = "MaxLength";
      colMaxLength.HeaderText = "Max Len";
      colMaxLength.ToolTipText = "Max Length";
      colMaxLength.Name = "colMaxLength";
      grid.Columns.Add(colMaxLength);
      IdxColMaxLength = colMaxLength.Index;

      DataGridViewTextBoxColumn colMinValue = new DataGridViewTextBoxColumn();
      colMinValue.DataPropertyName = "MinValue";
      colMinValue.HeaderText = "Min Val.";
      colMinValue.ToolTipText = "Min Value";
      colMinValue.Name = "colMinValue";
      grid.Columns.Add(colMinValue);
      IdxColMinValue = colMinValue.Index;

      DataGridViewTextBoxColumn colMaxValue = new DataGridViewTextBoxColumn();
      colMaxValue.DataPropertyName = "MaxValue";
      colMaxValue.HeaderText = "Max Val.";
      colMaxValue.ToolTipText = "Max Value";
      colMaxValue.Name = "colMaxValue";
      grid.Columns.Add(colMaxValue);
      IdxColMaxValue = colMaxValue.Index;
      
      if (FKs != null && FKs.Count > 0)
      {
        DataGridViewCheckBoxColumn colHasLookup = new DataGridViewCheckBoxColumn();
        colHasLookup.DataPropertyName = "HasLookup";
        colHasLookup.HeaderText = "Has Lookup";
        colHasLookup.Name = "colHasLookup";
        colHasLookup.ToolTipText = "Checked to enable Lookup ComboBox, uncheck to use simple textbox, only available for Foreign key columns";
        grid.Columns.Add(colHasLookup);
        IdxColHaslookup = colHasLookup.Index;

        DataGridViewComboBoxColumn colLookupColumn = new DataGridViewComboBoxColumn();
        colLookupColumn.DataSource = allColumns.ToList();
        colLookupColumn.DataPropertyName = "LookupColumn";
        colLookupColumn.HeaderText = "Lookup Column";
        colLookupColumn.Name = "colLookupColumn";
        colLookupColumn.ToolTipText = "Pick the column from the foreign table to use as friendly value for this lookup.";
        grid.Columns.Add(colLookupColumn);
        IdxColLookupColumn = colLookupColumn.Index;
        grid.Columns[IdxColHaslookup].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        grid.Columns[IdxColLookupColumn].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
      }

      grid.DataSource = colsValidation;

      grid.Columns[IdxColName].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;   //DataGridViewAutoSizeColumnMode.ColumnHeader;
      grid.Columns[IdxColRequired].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;  //DataGridViewAutoSizeColumnMode.DisplayedCells;      
      grid.Columns[IdxColDataType].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;  //DataGridViewAutoSizeColumnMode.DisplayedCells;
      grid.Columns[IdxColDefaultValue].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
      grid.Columns[IdxColMaxLength].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      grid.Columns[IdxColMinValue].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      grid.Columns[IdxColMaxValue].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      if (FKs != null && FKs.Count > 0)
      {
        grid.Columns[IdxColHaslookup].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        grid.Columns[IdxColLookupColumn].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
      }
#endif      
      grid.AllowUserToAddRows = false;
      grid.Refresh();

      if (FKs != null && FKs.Count > 0)
      {
        // highlight the rows that are FK columns, disable the FK cells for columns (datagrid rows) which are not FKs.
        foreach (DataGridViewRow row in grid.Rows)
        {
          if (!(bool)(row.Cells[IdxColHaslookup].Value))
          {
            row.Cells[IdxColHaslookup].ReadOnly = true;
            row.Cells[IdxColLookupColumn].ReadOnly = true;
          }
          else
          {
            row.DefaultCellStyle.BackColor = Color.FromArgb(255, 197, 206, 216);
          }
        }
      }
    }
  }

  public class DbTables : INotifyPropertyChanged
  {

    private bool _selected { get; set; }
    private string _name { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    public bool Selected
    {
      get
      {
        return _selected;
      }
    }

    public string Name
    {
      get
      {
        return _name;
      }
    }

    public DbTables(bool selected, string name)
    {
      _selected = selected;
      _name = name;
    }

    private void NotifyPropertyChanged(String propertyName)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    public void CheckObject(bool selected)
    {
      _selected = selected;
      NotifyPropertyChanged("Selected");
    }
  }
}
