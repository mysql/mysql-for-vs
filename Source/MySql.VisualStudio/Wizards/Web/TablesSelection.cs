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
using MySql.Data.VisualStudio.Wizards;

namespace MySql.Data.VisualStudio.Wizards.Web
{
  public partial class TablesSelection : WizardPage
  {

    protected BindingList<DbTables> _tables = new BindingList<DbTables>();
    internal BindingSource _sourceTables = new BindingSource();

    internal string ConnectionString { get; set; }

    public TablesSelection()
    {
      InitializeComponent();      
      listTables.CellContentClick +=new DataGridViewCellEventHandler(listTables_CellContentClick);
      chkSelectAllTables.CheckedChanged += chkSelectAllTables_CheckedChanged;
      txtFilter.KeyDown += txtFilter_KeyDown;

      listTables.AutoGenerateColumns = false;
      AddMoreColumns();
    }

    internal virtual void AddMoreColumns()
    {
      DataGridViewCheckBoxColumn colSelect = new DataGridViewCheckBoxColumn();
      colSelect.DataPropertyName = "Selected";
      colSelect.HeaderText = "Select";
      colSelect.Name = "colSelect";
      colSelect.Width = 45;
      listTables.Columns.Add(colSelect);

      DataGridViewTextBoxColumn colTable = new DataGridViewTextBoxColumn();
      colTable.DataPropertyName = "Name";
      colTable.HeaderText = "Table";
      colTable.Name = "colTable";
      colTable.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      colTable.ReadOnly = true;
      listTables.Columns.Add(colTable);
    }

    internal virtual void chkSelectAllTables_CheckedChanged(object sender, EventArgs e)
    {
        _tables.Where(t => t.Selected == !chkSelectAllTables.Checked).ToList().ForEach(t => { t.CheckObject(chkSelectAllTables.Checked); });
      listTables.Refresh();
    }

    internal List<DbTables> selectedTables
    {
      get
      {
        return _tables.Where(t => t.Selected).ToList();
      }
    }


    internal void FillTables(string connectionString)
    {
      var cnn = new MySqlConnection(connectionString);
      cnn.Open();

      var dtTables = cnn.GetSchema("Tables", new string[] { null, cnn.Database });
      _tables = new BindingList<DbTables>();

      for (int i = 0; i < dtTables.Rows.Count; i++)
      {
        _tables.Add(new DbTables(false, dtTables.Rows[i][2].ToString()));
      }

      _sourceTables.DataSource = _tables;
      listTables.DataSource = _sourceTables;
      FormatTablesList();
      TablesFilled();
    }

    internal virtual void TablesFilled()
    {
      errorProvider1.SetError(listTables, "");
    }

    internal void FormatTablesList()
    {
      if (listTables.Rows.Count <= 1 && listTables.Columns.Count == 1)
      {
        _sourceTables.DataSource = new BindingList<DbTables>();
        //listTables.AutoGenerateColumns = false;
        listTables.DataSource = _sourceTables;
        listTables.Update();
      }

      listTables.Refresh();
    }

    internal virtual void listTables_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {
      if (e.RowIndex == -1) return;
      if (String.IsNullOrEmpty(listTables.Rows[e.RowIndex].Cells[1].Value as string))
        return;

      if (e.ColumnIndex == 0)
      {
        var selected = _tables.Single(t => t.Name.Equals((string)listTables.Rows[e.RowIndex].Cells[1].Value,
          StringComparison.InvariantCultureIgnoreCase));
        selected.CheckObject(!selected.Selected);
        listTables.Refresh();
      }
    }


    internal override void OnStarting(BaseWizardForm wizard)
    {
      BaseWizardForm wiz = (BaseWizardForm)wizard;
      ConnectionString = wiz.ConnectionString;
      if (!string.IsNullOrEmpty(wiz.ConnectionString))
      {
        var cnn = new MySqlConnection(wiz.ConnectionString);
        try
        {
          cnn.Open();
        }
        catch (Exception)
        {
          DialogResult result = MessageBox.Show(Properties.Resources.ErrorOnConnection, "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
          if (result == DialogResult.Cancel)
          {
            listTables.Enabled = false;
          }
        }
        FillTables(wiz.ConnectionString);
      }
    }

    internal override bool IsValid()
    {
      CancelEventArgs args = new CancelEventArgs();
      TableSelection_Validating(this, args);
      if (args.Cancel) return false;
      else return true;
    }


    internal void TableSelection_Validating(object sender, CancelEventArgs e)
    {   
        if (_tables != null)
        {
          if (_tables.Where(t => t.Selected).Count() <= 0)
          {
            e.Cancel = true;
            errorProvider1.SetError(listTables, "At least a table should be selected");
          }
        }
        else
        {
          errorProvider1.SetError(listTables, "");
        }
    }

    internal void txtFilter_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)
      {
        if (String.IsNullOrEmpty(txtFilter.Text) && (listTables.RowCount < _tables.Count))
        {
          _sourceTables.DataSource = _tables;
          listTables.DataSource = _sourceTables;
          listTables.Update();
          FormatTablesList();
          return;
        }
      }
      if (e.KeyCode == Keys.Enter)
      {
        btnFilter_Click(sender, null);
      }
    }

    internal void btnFilter_Click(object sender, EventArgs e)
    {
      _sourceTables.DataSource = _tables.Where(t => t.Name.StartsWith(txtFilter.Text.Trim())).ToList();   
      listTables.DataSource = _sourceTables;
      listTables.Update();
      FormatTablesList();
    }

    internal void txtFilter_TextChanged(object sender, EventArgs e)
    {
    }
  }
}
