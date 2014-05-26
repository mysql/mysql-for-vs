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
    
    private Dictionary<string, Column> _columns;
    private List<ColumnValidation> _colValidations;
    //private List<ColumnValidation> _colValidationsDetail;
    private string _table;
    
    private string _connectionString;

    internal Dictionary<string, Column> Columns
    {
      get
      {
        return _columns;
      }
    }

    internal List<ColumnValidation> ValidationColumns {
      get {
        if (chkValidations.Checked) return _colValidations;
        else return new List<ColumnValidation>();
      }
    }    

    public ValidationConfig()
    {
      InitializeComponent();

      _colValidations = new List<ColumnValidation>();

      grdColumns.CellValidating += grdColumns_CellValidating;
      SetDefaults();
    }

    private void SetDefaults()
    {
      chkValidations.Checked = true;
      chkValidations_CheckedChanged(chkValidations, EventArgs.Empty);
    }

    internal override void OnStarting(BaseWizardForm wizard)
    {
      WindowsFormsWizardForm wiz = (WindowsFormsWizardForm)wizard;

      lblTitle.Text = string.Format("Columns to add validations from table: {0}", _table);

      // Populate grid
      if ( ( _table != wiz.TableName ) || ( _connectionString != wiz.Connection.ConnectionString ) )
      {
        _table = wiz.TableName;
        _connectionString = wiz.Connection.ConnectionString;
        _columns = BaseWizard<BaseWizardForm, WindowsFormsCodeGeneratorStrategy>.GetColumnsFromTable(_table, wiz.Connection);
        _colValidations.Clear();
        ValidationsGrid.LoadGridColumns(grdColumns, wiz.Connection, _table, _colValidations, _columns);
      }     
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
        double v = 0;
        if ( (value is DBNull) || string.IsNullOrEmpty( value.ToString() )) { row.ErrorText = ""; return; }
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
          v = ( double )value;
        }
        row.ErrorText = "";
        // Compare min vs max value
        object value2 = row.Cells[5].Value;
        double v2 = 0;
        if ((value2 is DBNull) || string.IsNullOrEmpty(row.Cells[5].FormattedValue.ToString()) ) { row.ErrorText = ""; return; }
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
      else if (e.ColumnIndex == 5)  // Max Value
      {
        double v = 0;
        if ( (value is DBNull) || string.IsNullOrEmpty( value.ToString() )) { row.ErrorText = ""; return; }
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
        if ( (value2 is DBNull) || string.IsNullOrEmpty( row.Cells[ 4 ].FormattedValue.ToString() ) ) { row.ErrorText = ""; return; }
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
          v2 = ( double )value2;
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

    private void chkValidations_CheckedChanged(object sender, EventArgs e)
    {
      grdColumns.Enabled = chkValidations.Checked;
    }
  }
}
