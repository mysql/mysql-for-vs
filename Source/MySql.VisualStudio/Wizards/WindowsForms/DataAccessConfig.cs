// Copyright © 2008, 2016, Oracle and/or its affiliates. All rights reserved.
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
using MySql.Data.MySqlClient;
using EnvDTE;
using MySql.Data.VisualStudio.DBExport;
using MySql.Data.VisualStudio.ServerInstances;

namespace MySql.Data.VisualStudio.Wizards.WindowsForms
{
  public partial class DataAccessConfig : WizardPage
  {
    private BaseWizardForm _baseWizardForm;
    private DTE _dte;    

    internal MySqlConnection Con
    {        
        get
        {
          return !string.IsNullOrEmpty(ConnectionStringTextBox.Tag.ToString())
            ? new MySqlConnection(ConnectionStringTextBox.Tag.ToString())
            : null;
        }
    }

    internal string ConnectionString
    {
      get
      {
        return ConnectionStringTextBox.Tag.ToString();
      }
    }

    internal string ConnectionName
    {
      get {
        return cmbConnections.Text;
      }
    }

    internal bool IncludePassword
    {
        get
        {
            return includeSensitiveInformationCheck.Checked;
        }
    }
  
    internal DataAccessTechnology DataAccessTechnology
    {
      get
      {
        if (radTechTypedDataSet.Checked) return DataAccessTechnology.TypedDataSet;
        else if (radEF5.Checked) return DataAccessTechnology.EntityFramework5;
        else if (radEF6.Checked) return DataAccessTechnology.EntityFramework6;
        else return DataAccessTechnology.None;
      }
    }
   
    public DataAccessConfig()
    {
      InitializeComponent();

      /* assign events */
      ConnectionStringTextBox.TextChanged += ConnectionStringTextBox_TextChanged;
      cmbConnections.SelectionChangeCommitted += cmbConnections_SelectionChangeCommitted;            
    }
    
    private void cmbConnections_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if ((cmbConnections.SelectedItem as MySqlServerExplorerConnection) == null) return;
      ConnectionStringTextBox.Text = MySqlServerExplorerConnections.MaskPassword(((MySqlServerExplorerConnection)cmbConnections.SelectedItem).ConnectionString);
      ConnectionStringTextBox.Tag = ((MySqlServerExplorerConnection)cmbConnections.SelectedItem).ConnectionString;

      if (!IsConnectionStringValid(((MySqlServerExplorerConnection)cmbConnections.SelectedItem).ConnectionString))
      {
        ShowConnectionDialog(false);
      }
      
    }

    private void ConnectionStringTextBox_TextChanged(object sender, EventArgs e)
    {
      if (!string.IsNullOrEmpty(ConnectionStringTextBox.Text))
        errorProvider1.SetError(ConnectionStringTextBox, "");
    }

    private void btnConnConfig_Click(object sender, EventArgs e)
    {
      ShowConnectionDialog(true);
    }

    private void DataAccessConfig_Validating(object sender, CancelEventArgs e)
    {
      e.Cancel = false;

      if (!IsConnectionStringValid(ConnectionStringTextBox.Tag.ToString()))
      {
        e.Cancel = true;
        errorProvider1.SetError(cmbConnections, "A valid connection string must be selected.");
      }
      else
      {
        errorProvider1.SetError(cmbConnections, "");
      }      
    }


    private bool IsConnectionStringValid(string connectionString)
    {
      if (string.IsNullOrEmpty(connectionString))
        return false;

      if (string.IsNullOrEmpty(cmbConnections.Text))
        return false;

      if (string.IsNullOrEmpty(ConnectionStringTextBox.Text))
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

    internal override bool IsValid()
    {
      CancelEventArgs args = new CancelEventArgs();
      DataAccessConfig_Validating(this, args);
      if (args.Cancel) return false;
      else return true;
    }

    private void ShowConnectionDialog(bool addSeConnection)
    {
      MySqlServerExplorerConnections.ShowNewConnectionDialog(ConnectionStringTextBox, _dte, cmbConnections, addSeConnection);
      var connections = (List<MySqlServerExplorerConnection>)cmbConnections.DataSource;

      if (addSeConnection)
      {
        _baseWizardForm.connections.DataSource = connections;
      }
      else
      {
        cmbConnections.DataSource = connections;
        cmbConnections.Refresh();
      }
    }


    internal override void OnStarting(BaseWizardForm wizard)
    {
      _baseWizardForm = wizard;
      WindowsFormsWizardForm wizardForm = (WindowsFormsWizardForm)wizard;
      _dte = ((WindowsFormsWizardForm)wizard).dte;

      MySqlServerExplorerConnections.LoadConnectionsForWizard(wizard.connections, cmbConnections, ConnectionStringTextBox, "CSharpWinForms");

      if (ConnectionStringTextBox.Tag != null && !IsConnectionStringValid(ConnectionStringTextBox.Tag.ToString()))
      {
        ShowConnectionDialog(false);
      }
      // Enable EF6 only if we are in VS2013 or major
      double version = double.Parse(wizardForm.Wizard.GetVisualStudioVersion());
      if (version >= 12.0)
      {
        radEF6.Enabled = true;
      }
      else
      {
        radEF6.Enabled = false;
      }
    }

    private void newConnString_Click(object sender, EventArgs e)
    {
      ShowConnectionDialog(true);
    }
  }

  
}
