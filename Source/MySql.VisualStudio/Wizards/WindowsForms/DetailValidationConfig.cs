// Copyright (c) 2008, 2016, Oracle and/or its affiliates. All rights reserved.
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
  public partial class DetailValidationConfig : WizardPage
  {
    private Dictionary<string, Column> _detailColumns;       
    private List<ColumnValidation> _colValidationsDetail;    
    private Dictionary<string, ColumnValidation> _colValsByName;
    private string _detailTable;
    private string _connectionString;

    
    internal Dictionary<string, Column> DetailColumns
    {
      get
      {
        return _detailColumns;
      }
    }

    internal List<ColumnValidation> DetailValidationColumns
    {
      get
      {
        if (chkValidations.Checked) return _colValidationsDetail;
        else return new List<ColumnValidation>();
      }
    }

    public DetailValidationConfig()
    {
      InitializeComponent();
      _colValidationsDetail = new List<ColumnValidation>();
      grdColumnsDetail.CellValidating += grdColumnsDetail_CellValidating;
      grdColumnsDetail.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(grdColumnsDetail_EditingControlShowing);
      SetDefaults();
    }

    private void SetDefaults()
    {
      chkValidations.Checked = true;
      chkValidations_CheckedChanged(chkValidations, EventArgs.Empty);
    }

    private void grdColumnsDetail_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
    {
      if (e.Control is DataGridViewComboBoxEditingControl)
      {
        DataGridViewComboBoxEditingControl cb = e.Control as DataGridViewComboBoxEditingControl;
        ColumnValidation cv = _colValsByName[(string)grdColumnsDetail.CurrentRow.Cells[0].Value];
        cb.DataSource = cv.ReferenceableColumns;
      }
    }

    internal override bool IsValid()
    {
      return true;
    }

    void grdColumnsDetail_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
    {
      DataGridViewRow row = grdColumnsDetail.Rows[e.RowIndex];
      object value = e.FormattedValue;
      e.Cancel = false;
      if (row.IsNewRow) return;
      if (e.ColumnIndex == ValidationsGrid.IdxColMinValue) // Min Value
      {
        double v = 0;
        if ((value is DBNull) || string.IsNullOrEmpty(value.ToString())) { row.ErrorText = ""; return; }
        if (!(value is double))
        {
          if (!double.TryParse(value.ToString(), out v))
          {
            e.Cancel = true;
            row.ErrorText = "The minimum value must be an integer.";
            return;
          }
        }
        else
        {
          v = (double)value;
        }
        row.ErrorText = "";
        // Compare min vs max value
        object value2 = row.Cells[5].Value;
        double v2 = 0;
        if ((value2 is DBNull) || string.IsNullOrEmpty(row.Cells[5].FormattedValue.ToString())) { row.ErrorText = ""; return; }
        if (!(value2 is double))
        {
          if (!double.TryParse(value2.ToString(), out v2))
          {
            e.Cancel = true;
            row.ErrorText = "The maximum value must be an integer.";
            return;
          }
        }
        else
        {
          v2 = (double)value2;
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
      else if (e.ColumnIndex == ValidationsGrid.IdxColMaxValue)  // Max Value
      {
        double v = 0;
        if ((value is DBNull) || string.IsNullOrEmpty(value.ToString())) { row.ErrorText = ""; return; }
        if (!(value is double))
        {
          if (!double.TryParse(value.ToString(), out v))
          {
            e.Cancel = true;
            row.ErrorText = "The maximum value must be an integer.";
            return;
          }
        }
        else
        {
          v = (double)value;
        }
        row.ErrorText = "";
        // Compare max vs min value
        object value2 = row.Cells[4].Value;
        double v2 = 0;
        if ((value2 is DBNull) || string.IsNullOrEmpty(row.Cells[4].FormattedValue.ToString())) { row.ErrorText = ""; return; }
        if (!(value2 is double))
        {
          if (!double.TryParse(value2.ToString(), out v2))
          {
            e.Cancel = true;
            row.ErrorText = "The minimun value must be an integer.";
          }
        }
        else
        {
          v2 = (double)value2;
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
      else if (e.ColumnIndex == ValidationsGrid.IdxColMaxLength)  // MaxLength
      {
        int v = 0;
        if ((value is DBNull) || string.IsNullOrEmpty(value.ToString())) { row.ErrorText = ""; return; }
        if (!(value is int))
        {
          if (!int.TryParse(value.ToString(), out v))
          {
            e.Cancel = true;
            row.ErrorText = "The MaxLength value must be an integer.";
            return;
          }
        }
      }
    }

    internal void GenerateModel(AdvancedWizardForm wiz)
    {
      if (wiz.DetailForeignKeys != null)
        wiz.DetailForeignKeys.Clear();
      _connectionString = wiz.Connection.ConnectionString;
      _detailTable = wiz.DetailTableName;
      if (string.IsNullOrEmpty(_detailTable)) return;
      _detailColumns = BaseWizard<BaseWizardForm, WindowsFormsCodeGeneratorStrategy>.GetColumnsFromTable(_detailTable, wiz.Connection);

      var wizardDetailForeignKeys = wiz.DetailForeignKeys;
      wiz.Wizard.RetrieveAllFkInfo(wiz.Connection, _detailTable, out wizardDetailForeignKeys);
      wiz.DetailForeignKeys = wizardDetailForeignKeys;

      _colValidationsDetail = ValidationsGrid.GetColumnValidationList(_detailTable, _detailColumns, wiz.DetailForeignKeys);
    }

    internal override void OnStarting(BaseWizardForm wizard)
    {
      AdvancedWizardForm wiz = (AdvancedWizardForm)wizard;

      // Populate grid
      grdColumnsDetail.DataSource = null;
      GenerateModel(wiz);
      ValidationsGrid.LoadGridColumns(grdColumnsDetail, wiz.Connection, _detailTable, _colValidationsDetail, _detailColumns, wiz.DetailForeignKeys);
      lblTitleDetail.Text = string.Format("Setup the validations for the columns in the table {0}", _detailTable);
      _colValsByName = new Dictionary<string, ColumnValidation>();
      for (int i = 0; i < _colValidationsDetail.Count; i++)
      {
        _colValsByName.Add(_colValidationsDetail[i].Name, _colValidationsDetail[i]);
      }

    }
     
    private void chkValidations_CheckedChanged(object sender, EventArgs e)
    {
      grdColumnsDetail.Enabled = chkValidations.Checked;   
    }
  }
}
