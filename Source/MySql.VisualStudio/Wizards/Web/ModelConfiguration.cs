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
using EnvDTE;
using MySql.Data.VisualStudio.DBExport;
using MySql.Data.VisualStudio.Common;

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

    private BaseWizardForm baseWizardForm;
    private DTE _dte; 

    internal string modelName
    {
      get {
        return ModelNameTextBox.Text;
      }    
    }

    internal string connectionStringName
    {
      get
      {
        return ModelNameTextBox.Text;
      }
    }

    internal string connectionString
    {
      get
      {
        return ConnectionStringTextBox.Tag.ToString();
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
        {
          baseWizardForm.SetSkipNextPageFromCurrent(this, true);
          return DataEntityVersion.None;
        }
      }    
    }
   
    public ModelConfiguration()
    {
      InitializeComponent();
      ModelNameTextBox.Text = "Model1";            
      rdbNoModel.CheckedChanged += rdbNoModel_CheckedChanged;
      cmbConnections.SelectionChangeCommitted += cmbConnections_SelectionChangeCommitted;
    }

    void cmbConnections_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if ((cmbConnections.SelectedItem as MySqlServerExplorerConnection) == null) return;

      if (!IsConnectionValid(((MySqlServerExplorerConnection)cmbConnections.SelectedItem).ConnectionString))
      {
        ShowConnectionDialog(false);
      }
      else
      {
        ConnectionStringTextBox.Text = MySqlServerExplorerConnections.MaskPassword(((MySqlServerExplorerConnection)cmbConnections.SelectedItem).ConnectionString);
        ConnectionStringTextBox.Tag = ((MySqlServerExplorerConnection)cmbConnections.SelectedItem).ConnectionString;
      }
    }

    void rdbNoModel_CheckedChanged(object sender, EventArgs e)
    {
      RadioButton control = (RadioButton)sender;      
     
      if (control.Checked)
       ModelNameTextBox.Text = String.Empty;
       
       ModelNameTextBox.Enabled = !control.Checked;    
       cmbConnections.Enabled = !control.Checked;         
       chkUseSameConnection.Enabled = !control.Checked;
       newConnString.Enabled = !control.Checked;
       ConnectionStringTextBox.Enabled = !control.Checked;
       baseWizardForm.btnFinish.Enabled = control.Checked;
       baseWizardForm.SetSkipNextPageFromCurrent(this, control.Checked);
    }
 

    internal override void OnStarting(BaseWizardForm wizard)
    {
      WebWizardForm wiz = (WebWizardForm)wizard;

      baseWizardForm = wizard;
      _dte = ((WebWizardForm)wizard).dte;

      MySqlServerExplorerConnections.LoadConnectionsForWizard(wizard.connections, cmbConnections, ConnectionStringTextBox,"CSharpMVC");

      chkUseSameConnection.Checked = true;
      cmbConnections.SelectedValue = wiz.connectionStringForAspNetTables;
      ConnectionStringTextBox.Text = MySqlServerExplorerConnections.MaskPassword(wiz.connectionStringForAspNetTables);
      ConnectionStringTextBox.Tag = wiz.connectionStringForAspNetTables;
      
      double version = double.Parse(wiz.Wizard.GetVisualStudioVersion());
      if (version >= 12.0)
      {
        Ef6.Enabled = true;
      }
      else
      {
        Ef6.Enabled = false;
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
      e.Cancel = false;
      if (Ef5.Checked || Ef6.Checked)
      { 
         if (!IsConnectionValid(ConnectionStringTextBox.Tag.ToString()))
          {
            e.Cancel = true;
            errorProvider1.SetError(cmbConnections, "A valid connection string must be selected.");
          }
        else
         {
           errorProvider1.SetError(cmbConnections, "");
          }
         if (String.IsNullOrEmpty(ModelNameTextBox.Text))
         {
           e.Cancel = true;
           errorProvider1.SetError(ModelNameTextBox, "Model name cannot be empty.");
         }
         else
         {
           errorProvider1.SetError(ModelNameTextBox, "");
         }
      }      
    }

    private bool IsConnectionValid(string connectionString)
    {
      if (String.IsNullOrEmpty(connectionString))
        return false;

      var cnn = new MySqlConnection(connectionString);
      try
      {
        cnn.OpenWithDefaultTimeout();
        cnn.Close();
      }
      catch { return false; }

      return true;
    }

    private void newConnString_Click(object sender, EventArgs e)
    {
      ShowConnectionDialog(true);
    }

    private void ShowConnectionDialog(bool addSEConnection)
    {

      MySqlServerExplorerConnections.ShowNewConnectionDialog(ConnectionStringTextBox, _dte, cmbConnections, addSEConnection);     
      baseWizardForm.connections = cmbConnections.DataSource as BindingSource;
       
    }

    private void chkUseSameConnection_CheckedChanged(object sender, EventArgs e)
    {
      var control = (CheckBox)sender;
      cmbConnections.Enabled = control.Checked;      
      newConnString.Enabled = control.Checked;
      ConnectionStringTextBox.Enabled = control.Checked;
    }     
  }
}
