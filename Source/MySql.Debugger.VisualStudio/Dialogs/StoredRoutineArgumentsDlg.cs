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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MySql.Debugger.VisualStudio
{
  public partial class StoredRoutineArgumentsDlg : Form
  {
    public StoredRoutineArgumentsDlg()
    {
      InitializeComponent();
      Init();
    }

    private void Init()
    {
      Arguments = new DataTable();
      Arguments.Columns.Add("Name", typeof(string));
      Arguments.Columns.Add("Value", typeof(string));
      Arguments.Columns.Add("IsNull", typeof(bool));
      Arguments.Columns.Add("Type", typeof(string));
    }

    private void btnOK_Click(object sender, EventArgs e)
    {
      this.DialogResult = DialogResult.OK;
      this.Close();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
      this.DialogResult = DialogResult.Cancel;
      this.Close();
    }

    private void StoredRoutineArguments_Load(object sender, EventArgs e)
    {

    }

    internal DataTable Arguments { get; set; }

    internal void AddNameValue( string Name, string Value, string type )
    {
      Arguments.Rows.Add(Name, Value, false, type);
    }

    internal void DataBind()
    {
      gridArguments.AutoGenerateColumns = false;
      gridArguments.DataSource = Arguments;
    }

    internal IEnumerable<NameValue> GetNameValues()
    {
      foreach (DataRow dr in Arguments.Rows)
      {
        yield return new NameValue() { Name = ( string )dr[ 0 ], Value = ( string )dr[ 1 ], IsNull = (bool)dr[2] };
      }
    }

    private void gridArguments_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {
      // checkbox change event
      if (e.RowIndex > -1 && e.ColumnIndex == gridArguments.Columns["colNull"].Index)
      {
        bool isChecked = (bool)gridArguments[e.ColumnIndex, e.RowIndex].EditedFormattedValue;
        int valueColumnIndex = gridArguments.Columns["colValue"].Index;
        gridArguments[valueColumnIndex, e.RowIndex].ReadOnly = isChecked;
        gridArguments[valueColumnIndex, e.RowIndex].Style.ForeColor = isChecked ? SystemColors.GrayText : SystemColors.ControlText;
      }
    }

    private void gridArguments_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
      gridArguments_CellContentClick(sender, e);
    }
  }

  internal class NameValue
  {
    internal string Name { get; set; }
    internal string Value { get; set; }
    internal bool IsNull { get; set; }
  }
}
