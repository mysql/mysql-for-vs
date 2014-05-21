﻿// Copyright © 2008, 2014, Oracle and/or its affiliates. All rights reserved.
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
  public partial class DataSourceConfiguration : WizardPage
  {
    private BaseWizardForm baseWizardForm;
    private DTE _dte; 

    internal bool includeSensitiveInformation
    {
      get
      {
         return includeSensitiveInformationCheck.Checked;
       }
    }
  
    
    internal string connectionStringName
    {
      get
      {
        return ConnectionStringNameTextBox.Text;
      }
    }

    internal string connectionString
    {
      get
      {        
        return ConnectionStringTextBox.Tag.ToString();
      }
    }   
   

    internal bool includeRoleProvider
    {
      get
      {
        return includeRoleProviderCheck.Checked;
      }
    }


    internal bool includeProfileProvider
    {
      get
      {
        return includeProfileProviderCheck.Checked;
      }    
    }

    internal string selectedConnection
    {
      get {
        return cmbConnections.Text;      
      }
    }

    public DataSourceConfiguration()
    {
      InitializeComponent();
      /* Loading defaults */      
      ConnectionStringNameTextBox.Text = "LocalMySqlServer";
      includeRoleProviderCheck.Checked = true;
      includeProfileProviderCheck.Checked = true;      
     
      /* assign events */
      ConnectionStringTextBox.TextChanged += new EventHandler(ConnectionStringTextBox_TextChanged);     
      cmbConnections.SelectionChangeCommitted += new EventHandler(cmbConnections_SelectionChangeCommitted);      
    }
   

    private void editConnString_Click(object sender, EventArgs e)
    {
      ShowConnectionDialog(true);
    }

    private void ConnectionStringTextBox_TextChanged(object sender, EventArgs e)
    {
      if (!String.IsNullOrEmpty(ConnectionStringTextBox.Text))
        errorProvider1.SetError(ConnectionStringTextBox, "");
    }


    private void ShowConnectionDialog(bool addSEConnection)
    {
      MySqlServerExplorerConnections.ShowNewConnectionDialog(ConnectionStringTextBox, _dte, cmbConnections, addSEConnection);
      var connections = (List<MySqlServerExplorerConnection>)cmbConnections.DataSource;

      if (addSEConnection)
      {          
          baseWizardForm.connections.DataSource = connections;
      }
      else
      {       
        cmbConnections.DataSource = connections;
        cmbConnections.Refresh();
      }      
    }


    internal override bool IsValid()
    {
      CancelEventArgs args = new CancelEventArgs();
      ProviderConfiguration_Validating(this, args);
      if (args.Cancel) return false;
      else return true;
    }

    private void ProviderConfiguration_Validating(object sender, CancelEventArgs e)
    {
      e.Cancel = false;
      if (!IsConnectionValid(ConnectionStringTextBox.Tag.ToString()))
      {
        e.Cancel = true;
        errorProvider1.SetError(cmbConnections, "A valid connection string must be selected.");
      }
      else
      {
        errorProvider1.SetError(cmbConnections, "");
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



    void cmbConnections_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if ((cmbConnections.SelectedItem as MySqlServerExplorerConnection) == null) return;     
        ConnectionStringTextBox.Text = MySqlServerExplorerConnections.MaskPassword(((MySqlServerExplorerConnection)cmbConnections.SelectedItem).ConnectionString);
        ConnectionStringTextBox.Tag = ((MySqlServerExplorerConnection)cmbConnections.SelectedItem).ConnectionString;

        if (!IsConnectionValid(((MySqlServerExplorerConnection)cmbConnections.SelectedItem).ConnectionString))
        {
            ShowConnectionDialog(false);
        }
    }     

    /// <summary>
    /// Sets caption for description and title according to this step
    /// in the wizard form container
    /// </summary>
    /// <param name="wizard"></param>
    internal override void OnStarting(BaseWizardForm wizard)    
    {
       baseWizardForm = wizard;
       _dte = ((WebWizardForm)wizard).dte;

      MySqlServerExplorerConnections.LoadConnectionsForWizard(wizard.connections, cmbConnections, ConnectionStringTextBox, "CSharpMVC");

      if (ConnectionStringTextBox.Tag != null && !IsConnectionValid(ConnectionStringTextBox.Tag.ToString()))
      {
          ShowConnectionDialog(false);
      }
    }                     
  }
}
