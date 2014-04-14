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

namespace MySql.Data.VisualStudio.Wizards.Web
{

  internal enum DataEntityVersion : int
  {    
    None= 0,
    EntityFramework5 = 1,
    EntityFramework6 = 2
  }

  public partial class ModelConfiguration : WizardPage
  {
    internal string modelName
    {
      get {
        return ModelNameTextBox.Text;
      }    
    }

    internal DataEntityVersion dEVersion
    {
      get {
        if (this.Ef5.Checked)
          return DataEntityVersion.EntityFramework5;
        if (this.Ef6.Checked)
          return DataEntityVersion.EntityFramework6;
        else
          return DataEntityVersion.None;
      }    
    }

    internal bool includeSensitiveInformation
    {
      get {
        return includeSensitiveInformationCheck.Checked;      
      }    
    }

    internal List<DbTables> selectedTables
    {
      get {
        return _tables.Where(t => t.Selected).ToList();      
      }
    }

    private BindingList<DbTables> _tables;
    BindingSource _sourceTables = new BindingSource();

    public ModelConfiguration()
    {
      InitializeComponent();
      ModelNameTextBox.Text = "Model1";
      listTables.CellContentClick +=new DataGridViewCellEventHandler(listTables_CellContentClick);
      rdbNoModel.CheckedChanged += new EventHandler(rdbNoModel_CheckedChanged);
    }

    void rdbNoModel_CheckedChanged(object sender, EventArgs e)
    {
      RadioButton control = (RadioButton)sender;
      var previousColor = Color.Black;
      if (control.Checked)
      {
        listTables.Enabled = false;        
        listTables.ForeColor = Color.LightGray;
        _sourceTables = new BindingSource();
      }
      else
      {
        listTables.Enabled = true;
        listTables.ForeColor = previousColor;        
      }
    }

    private void FillTables(string connectionString)
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
    }

    private void FormatTablesList()
    {
      if (listTables.Rows.Count <= 1 && listTables.Columns.Count == 1)
      {
        _sourceTables.DataSource = new BindingList<DbTables>();
        listTables.DataSource = _sourceTables;
        listTables.Update();
      }
      listTables.Columns[0].HeaderText = "Select";
      listTables.Columns[0].Width = 45;

      listTables.Columns[1].HeaderText = "Table";
      listTables.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      listTables.Columns[1].ReadOnly = true;

      listTables.Refresh();
    }

    private void listTables_CellContentClick(object sender, DataGridViewCellEventArgs e)
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
      WebWizardForm wiz = (WebWizardForm)wizard;
      if (!string.IsNullOrEmpty(wiz.connectionStringForModel))
      {
        var cnn = new MySqlConnection(wiz.connectionStringForModel);
        try 
          {	        
		        cnn.Open();
          }
        catch (Exception)
        {		  
           DialogResult result = MessageBox.Show(Properties.Resources.ErrorOnConnection, "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
           if(result == DialogResult.Cancel)
           {
              listTables.Enabled = false;           
           }
        }
        FillTables(wiz.connectionStringForModel);  
      }
    }

    internal override bool IsValid()
    {
      CancelEventArgs args = new CancelEventArgs();
      ModelConfiguration_Validating(this, args);
      if (args.Cancel) return false;
      else return true;
    }

    void ModelConfiguration_Validating(object sender, CancelEventArgs e)
    {
      if (!this.rdbNoModel.Checked)
      {
        if (_tables == null && _tables.Count == 0)
        {
          e.Cancel = true;
          errorProvider1.SetError(listTables, "At least a table should be selected");
        }
        else
        {
          errorProvider1.SetError(listTables, "");
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
