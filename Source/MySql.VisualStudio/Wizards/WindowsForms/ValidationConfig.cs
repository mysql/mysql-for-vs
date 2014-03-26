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

    internal List<ColumnValidation> UserSettingsForColumnValidation {
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
      grdColumns.AutoSize = true;

      //binding.DataSource = _colValidations;
      //grdColumns.DataSource = binding;
      DataGridViewTextBoxColumn colName = new DataGridViewTextBoxColumn();
      colName.DataPropertyName = "Name";
      colName.HeaderText = "ColumnName";
      colName.Name = "colName";
      grdColumns.Columns.Add(colName);

      DataGridViewCheckBoxColumn colRequired = new DataGridViewCheckBoxColumn();
      colRequired.DataPropertyName = "Required";
      colRequired.HeaderText = "Required";
      colRequired.Name = "colRequired";
      grdColumns.Columns.Add(colRequired);

      DataGridViewTextBoxColumn colDataType = new DataGridViewTextBoxColumn();
      colDataType.DataPropertyName = "DataType";
      colDataType.HeaderText = "Data Type";
      colDataType.Name = "colDataType";
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
  }

  internal class ColumnValidation
  {
    private Column _column;
    private int? _maxValue;
    private int? _minValue;
    private bool _required;
    private ColumnType _columnType;
    private object _defaultValue;

    internal Column Column { get { return _column; } }
    internal int? MaxValue { get { return _maxValue; } set { _maxValue = value; } }
    internal int? MinValue { get { return _minValue; } set { _minValue = value; } }
    internal bool Required { get { return _required; } set { _required = value; } }
    internal ColumnType ColumnType { get { return _columnType; } set { _columnType = value; } }
    internal object DefaultValue { get { return _defaultValue; } set { _defaultValue = value; } }
    internal string Name { get { return _column.ColumnName; } set { _column.ColumnName = value; } }
    internal string DataType { get { return _column.DataType; } set { _column.DataType = value; } }

    internal ColumnValidation(Column column)
    {
      _column = column;
    }
  }

  internal enum ColumnType : int
  {
    Text = 1,       // TextBox
    DateTime = 2    // DatePicker
  }
}
