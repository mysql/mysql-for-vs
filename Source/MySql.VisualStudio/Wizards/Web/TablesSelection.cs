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
using MySql.Utility.Classes.Logging;
using MySql.Data.VisualStudio.Common;

namespace MySql.Data.VisualStudio.Wizards.Web
{
  public partial class TablesSelection : WizardPage
  {

    protected BindingList<DbTables> _tables = new BindingList<DbTables>();
    internal BindingSource _sourceTables = new BindingSource();
    private BackgroundWorker _worker;
    private Cursor _cursor;
    private BaseWizardForm _wiz;

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
      LockUI();
      try
      {
        DoWorkEventHandler doWorker = (worker, doWorkerArgs) =>
        {          
          Application.DoEvents();          
          var cnn = new MySqlConnection(connectionString);
          cnn.OpenWithDefaultTimeout();
          var dtTables = cnn.GetSchema("Tables", new string[] { null, cnn.Database });
          cnn.Close();
          _tables = new BindingList<DbTables>();
          this.Invoke((Action)(() =>
          {
            for (int i = 0; i < dtTables.Rows.Count; i++)
              _tables.Add(new DbTables(false, dtTables.Rows[i][2].ToString()));

            _sourceTables.DataSource = _tables;
            listTables.DataSource = _sourceTables;
            FormatTablesList();
            TablesFilled();            
          }));
        };
        if (_worker != null)
        {
          _worker.DoWork -= doWorker;
          _worker.RunWorkerCompleted -= _worker_RunWorkerCompleted;
          _worker.Dispose();
        }
        _worker = new BackgroundWorker();
        _worker.WorkerSupportsCancellation = true;        
        _worker.DoWork += doWorker;
        _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_worker_RunWorkerCompleted);
        _worker.RunWorkerAsync();
      }
      finally 
      {
        UnlockUI();             
      }
    }
    

    private void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      if (e.Error != null)
      {
        this.Invoke((Action)(() =>
        {
          Logger.LogError($"The following error ocurred while exporting: {e.Error.Message}", true);
        }));
      }
      UnlockUI();     
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
      _wiz = (BaseWizardForm)wizard;
      
      ConnectionString = _wiz.ConnectionString;
      if (!string.IsNullOrEmpty(_wiz.ConnectionString))
      {
        var cnn = new MySqlConnection(_wiz.ConnectionString);
        try
        {
          cnn.OpenWithDefaultTimeout();
        }
        catch (Exception)
        {
          using (var dialog = Common.Utilities.GetDualButtonInfoDialog(
            Utility.Forms.InfoDialog.InfoType.Error,
            "Retry",
            "Cancel",
            "Failed connection",
            Properties.Resources.ErrorOnConnection
          ))
          {
            if (dialog.ShowDialog() == DialogResult.Cancel)
            {
              listTables.Enabled = false;
            }
          }
        }
        FillTables(_wiz.ConnectionString);
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

    private void LockUI()
    {
      _cursor = this.Cursor;
      this.Cursor = Cursors.WaitCursor;
      EnableControls(false);
    }

    private void EnableControls(bool enabling)
    {
      txtFilter.Enabled = enabling;
      btnFilter.Enabled = enabling;
      _wiz.btnFinish.Enabled = enabling;
      _wiz.btnBack.Enabled = enabling;
      chkSelectAllTables.Enabled = enabling;      
    }

    private void UnlockUI()
    {
      if (!(_worker != null && _worker.IsBusy))
      {        
        this.Cursor = _cursor;
        EnableControls(true);       
      }
    }
  }
}
