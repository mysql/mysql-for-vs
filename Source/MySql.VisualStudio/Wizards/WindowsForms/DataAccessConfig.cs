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
using MySql.Data.VisualStudio;
using MySql.Data.MySqlClient;
using EnvDTE;
using MySql.Data.VisualStudio.DBExport;


namespace MySql.Data.VisualStudio.Wizards.WindowsForms
{
  public partial class DataAccessConfig : WizardPage
  {
    private BaseWizardForm baseWizardForm;
    private DTE _dte;    

    internal MySqlConnection _con
    {        
        get
        {
           if (!string.IsNullOrEmpty(ConnectionStringTextBox.Tag.ToString()))
                return new MySqlConnection(ConnectionStringTextBox.Tag.ToString());
            else
                return null;
       }
    }
        
    List<MyListItem> _constraints = new List<MyListItem>();

    internal string connectionString
    {
      get
      {
        return ConnectionStringTextBox.Tag.ToString();
      }
    }

    internal string connectionName
    {
      get {
        return cmbConnections.Text;
      }
    }

    internal string TableName
    {
      get {
        if (cmbTable.SelectedIndex == -1) return null;
        else return ( string )cmbTable.SelectedItem;
      }
    }   
   
    public DataAccessConfig()
    {
      InitializeComponent();

      /* assign events */
      ConnectionStringTextBox.TextChanged += new EventHandler(ConnectionStringTextBox_TextChanged);
      cmbConnections.SelectionChangeCommitted += new EventHandler(cmbConnections_SelectionChangeCommitted);
      cmbTable.DropDown += cmbTable_DropDown;
      
    }

    private void cmbTable_DropDown(object sender, EventArgs e)
    {
      if (ConnectionStringTextBox.Tag != null)
      {
        FillTables();
      }
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

      cmbTable.Text = string.Empty;
    }

    private void ConnectionStringTextBox_TextChanged(object sender, EventArgs e)
    {
      if (!String.IsNullOrEmpty(ConnectionStringTextBox.Text))
        errorProvider1.SetError(ConnectionStringTextBox, "");
    }

    private void btnConnConfig_Click(object sender, EventArgs e)
    {
      ShowConnectionDialog(true);
    }

    private void FillTables()
    {      
      if (ConnectionStringTextBox.Tag == null || String.IsNullOrEmpty(ConnectionStringTextBox.Tag.ToString())) return;

      try
      {
          var cnn = new MySqlConnection(ConnectionStringTextBox.Tag.ToString());
          cnn.Open();
          
          string[] restrictions = new string[4];
          restrictions[1] = cnn.Database;
          DataTable t = cnn.GetSchema("Tables", restrictions);
          cmbTable.Items.Clear();
          for (int i = 0; i < t.Rows.Count; i++)
          {
              cmbTable.Items.Add(t.Rows[i][2]);
          }
          cmbTable.Text = string.Empty;
      }
      catch  {                    
      }
      
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

      if (string.IsNullOrEmpty((string)cmbTable.SelectedItem) || (cmbTable.SelectedIndex == -1))
      {
        e.Cancel = true;
        errorProvider1.SetError(cmbTable, "A table must be selected from the list.");
      }
      else
      {
        errorProvider1.SetError(cmbTable, "");
      } 
    }


    private bool IsConnectionStringValid(string connectionString)
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

    internal override bool IsValid()
    {
      CancelEventArgs args = new CancelEventArgs();
      DataAccessConfig_Validating(this, args);
      if (args.Cancel) return false;
      else return true;
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


    internal override void OnStarting(BaseWizardForm wizard)
    {
      baseWizardForm = wizard;
      _dte = ((WindowsFormsWizardForm)wizard).dte;

      MySqlServerExplorerConnections.LoadConnectionsForWizard(wizard.connections, cmbConnections, ConnectionStringTextBox, "CSharpWinForms");

      if (ConnectionStringTextBox.Tag != null && !IsConnectionStringValid(ConnectionStringTextBox.Tag.ToString()))
      {
        ShowConnectionDialog(false);
      }
    }

    private void newConnString_Click(object sender, EventArgs e)
    {
      ShowConnectionDialog(true);
    }
  }

  
}
