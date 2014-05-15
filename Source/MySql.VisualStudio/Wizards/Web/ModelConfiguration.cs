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
using EnvDTE;
using MySql.Data.VisualStudio.DBExport;

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
        return ConnectionStringTextBox.Text;
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

       cmbConnections.Enabled = !control.Checked;
       ModelNameTextBox.Enabled = !control.Checked;      
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
        cnn.Open();
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
  }
}
