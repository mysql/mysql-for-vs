// Copyright © 2008, 2014, Oracle and/or its affiliates. All rights reserved.
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

using MySql.Data.MySqlClient;
using MySql.Data.VisualStudio.SchemaComparer;
using System;
using System.Collections.Generic;
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
   
    internal static void LoadGridColumns(DataGridView grid, MySqlConnection con, string Table, 
      List<ColumnValidation> colsValidation, Dictionary<string, Column> columns)
    {
      BindingSource binding = new BindingSource();
      //reset grid 
      grid.DataSource = null;
      grid.Columns.Clear();
      grid.Rows.Clear();
      grid.Update();

      foreach (KeyValuePair<string, Column> kvp in columns)
      {
        ColumnValidation cv = new ColumnValidation(kvp.Value);
        cv.ColumnType = ColumnType.Text;
        cv.DefaultValue = "";
        cv.MinValue = null;
        cv.MaxValue = null;
        cv.Required = true;
        colsValidation.Add(cv);
      }
      grid.AutoGenerateColumns = false;
      
      DataGridViewTextBoxColumn colName = new DataGridViewTextBoxColumn();
      colName.DataPropertyName = "Name";
      colName.HeaderText = "Column Name";
      colName.Name = "colName";
      colName.ReadOnly = true;
      grid.Columns.Add(colName);

      DataGridViewCheckBoxColumn colRequired = new DataGridViewCheckBoxColumn();
      colRequired.DataPropertyName = "Required";
      colRequired.HeaderText = "Required";
      colRequired.Name = "colRequired";
      colRequired.ReadOnly = true;
      grid.Columns.Add(colRequired);

      DataGridViewTextBoxColumn colDataType = new DataGridViewTextBoxColumn();
      colDataType.DataPropertyName = "DataType";
      colDataType.HeaderText = "Data Type";
      colDataType.Name = "colDataType";
      colDataType.ReadOnly = true;
      grid.Columns.Add(colDataType);

      DataGridViewTextBoxColumn colDefaultValue = new DataGridViewTextBoxColumn();
      colDefaultValue.DataPropertyName = "DefaultValue";
      colDefaultValue.HeaderText = "Default";
      colDefaultValue.Name = "colDefaultValue";
      grid.Columns.Add(colDefaultValue);
      
      DataGridViewTextBoxColumn colMinValue = new DataGridViewTextBoxColumn();
      colMinValue.DataPropertyName = "MinValue";
      colMinValue.HeaderText = "Min Value";
      colMinValue.Name = "colMinValue";
      grid.Columns.Add(colMinValue);

      DataGridViewTextBoxColumn colMaxValue = new DataGridViewTextBoxColumn();
      colMaxValue.DataPropertyName = "MaxValue";
      colMaxValue.HeaderText = "Max Value";
      colMaxValue.Name = "colMaxValue";
      grid.Columns.Add(colMaxValue);
      grid.DataSource = colsValidation;

      grid.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;   //DataGridViewAutoSizeColumnMode.ColumnHeader;
      grid.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;  //DataGridViewAutoSizeColumnMode.DisplayedCells;      
      grid.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;  //DataGridViewAutoSizeColumnMode.DisplayedCells;
      grid.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
      grid.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      grid.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;      

      grid.Refresh();
    }
  
  
  }
}
