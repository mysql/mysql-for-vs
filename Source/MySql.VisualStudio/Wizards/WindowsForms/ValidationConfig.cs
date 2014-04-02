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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using MySql.Data.VisualStudio.SchemaComparer;
using MySql.Data.VisualStudio.Wizards;


namespace MySql.Data.VisualStudio.Wizards.WindowsForms
{
  public partial class ValidationConfig : WizardPage
  {
    private List<ColumnValidation> _colValidations;
    private string _table;
    private string _connectionString;

    internal List<ColumnValidation> ValidationColumns {
      get {
        if (chkValidation.Checked) return _colValidations;
        else return null;
      }
    }

    public ValidationConfig()
    {
      InitializeComponent();
    }

    internal override void OnStarting(BaseWizardForm wizard)
    {
      WindowsFormsWizardForm wiz = (WindowsFormsWizardForm)wizard;
      if ( ( _table != wiz.TableName ) || ( _connectionString != wiz.Connection.ConnectionString ) )
      {
        _table = wiz.TableName;
        _connectionString = wiz.Connection.ConnectionString;
        LoadGridColumns(wiz.Connection, _table);
      }
    }

    private void chkValidation_CheckedChanged(object sender, EventArgs e)
    {
      grdColumns.Enabled = chkValidation.Checked;
    }

    private void LoadGridColumns(MySqlConnection con, string Table)
    {
      _table = Table;
      _colValidations = new List<ColumnValidation>();
      Dictionary<string,Column> columns = BaseWizard<BaseWizardForm>.GetColumnsFromTable(Table, con);
      BindingSource binding = new BindingSource();
      foreach (KeyValuePair<string, Column> kvp in columns)
      {
        ColumnValidation cv = new ColumnValidation(kvp.Value);
        cv.ColumnType = ColumnType.Text;
        cv.DefaultValue = "";
        cv.MinValue = null;
        cv.MaxValue = null;
        cv.Required = true;
        _colValidations.Add(cv);
        //binding.Add(cv);
      }
      grdColumns.AutoGenerateColumns = false;
      //grdColumns.AutoSize = true;

      binding.DataSource = _colValidations;
      grdColumns.DataSource = binding;
      DataGridViewTextBoxColumn colName = new DataGridViewTextBoxColumn();
      colName.DataPropertyName = "Name";
      colName.HeaderText = "ColumnName";
      colName.Name = "colName";
      colName.ReadOnly = true;
      grdColumns.Columns.Add(colName);

      DataGridViewCheckBoxColumn colRequired = new DataGridViewCheckBoxColumn();
      colRequired.DataPropertyName = "Required";
      colRequired.HeaderText = "Required";
      colRequired.Name = "colRequired";
      colRequired.ReadOnly = true;
      grdColumns.Columns.Add(colRequired);

      DataGridViewTextBoxColumn colDataType = new DataGridViewTextBoxColumn();
      colDataType.DataPropertyName = "DataType";
      colDataType.HeaderText = "Data Type";
      colDataType.Name = "colDataType";
      colDataType.ReadOnly = true;
      grdColumns.Columns.Add(colDataType);

      DataGridViewTextBoxColumn colDefaultValue = new DataGridViewTextBoxColumn();
      colDefaultValue.DataPropertyName = "DefaultValue";
      colDefaultValue.HeaderText = "Default";
      colDefaultValue.Name = "colDefaultValue";
      grdColumns.Columns.Add(colDefaultValue);
      
      DataGridViewTextBoxColumn colMinValue = new DataGridViewTextBoxColumn();
      colMinValue.DataPropertyName = "MinValue";
      colMinValue.HeaderText = "Min Value";
      colMinValue.Name = "colMinValue";
      grdColumns.Columns.Add(colMinValue);

      DataGridViewTextBoxColumn colMaxValue = new DataGridViewTextBoxColumn();
      colMaxValue.DataPropertyName = "MaxValue";
      colMaxValue.HeaderText = "Max Value";
      colMaxValue.Name = "colMaxValue";
      grdColumns.Columns.Add(colMaxValue);

      grdColumns.DataSource = _colValidations;
      //grdColumns.Update();
    }

    internal override bool IsValid()
    {
      return true;
    }

    private void grdColumns_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
    {
      DataGridViewRow row = grdColumns.Rows[e.RowIndex];
      object value = e.FormattedValue;
      e.Cancel = false;
      if (row.IsNewRow) return;
      if (e.ColumnIndex == 4) // Min Value
      {
        int v = 0;
        if ( (value is DBNull) || string.IsNullOrEmpty( value.ToString() )) { row.ErrorText = ""; return; }
        if (!(value is int))
        {
          if (!int.TryParse((string)value, out v))
          {
            e.Cancel = true;
            row.ErrorText = "The minimum value must be an integer.";
            return;
          }
        }
        else
        {
          v = ( int )value;
        }
        row.ErrorText = "";
        // Compare min vs max value
        object value2 = row.Cells[5].Value;
        int v2 = 0;
        if ((value2 is DBNull) || string.IsNullOrEmpty(row.Cells[5].FormattedValue.ToString()) ) { row.ErrorText = ""; return; }
        if (!(value2 is int))
        {
          if (!int.TryParse((string)value2, out v2))
          {
            e.Cancel = true;
            row.ErrorText = "The maximum value must be an integer.";
            return;
          }
        }
        else
        {
          v2 = (int)value2;
        }
        if (v2 < v)
        {
          e.Cancel = true;
          row.ErrorText = "The minimum value must be less or equal than maximun value.";
        }
        else
        {
          row.ErrorText = "";
        }
      }
      else if (e.ColumnIndex == 5)  // Max Value
      {
        int v = 0;
        if ( (value is DBNull) || string.IsNullOrEmpty( value.ToString() )) { row.ErrorText = ""; return; }
        if (!(value is int))
        {
          if (!int.TryParse((string)value, out v))
          {
            e.Cancel = true;
            row.ErrorText = "The maximum value must be an integer.";
            return;
          }
        }
        else
        {
          v = (int)value;
        }
        row.ErrorText = "";
        // Compare max vs min value
        object value2 = row.Cells[4].Value;
        int v2 = 0;
        if ( (value2 is DBNull) || string.IsNullOrEmpty( row.Cells[ 4 ].FormattedValue.ToString() ) ) { row.ErrorText = ""; return; }
        if (!(value2 is int))
        {
          if (!int.TryParse((string)value2, out v2))
          {
            e.Cancel = true;
            row.ErrorText = "The minimun value must be an integer.";
          }
        }
        else
        {
          v2 = ( int )value2;
        }
        if (v2 > v)
        {
          e.Cancel = true;
          row.ErrorText = "The minimum value must be less or equal than maximum value.";
        }
        else
        {
          row.ErrorText = "";
        }
      }
    }
  }

  internal class ColumnValidation
  {
    private Column _column;
    private int? _maxValue;
    private int? _minValue;
    private bool _required;
    private ColumnType _columnType;
    private object _defaultValue;

    // It is important to make the properties public, otherwise DataGridView doesn't like and doesn't display the real values
    // (this seems to be a known issue with DataGridView over stackoverflow).
    public Column Column { get { return _column; } }
    public int? MaxValue { get { return _maxValue; } set { _maxValue = value; } }
    public int? MinValue { get { return _minValue; } set { _minValue = value; } }
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
}
