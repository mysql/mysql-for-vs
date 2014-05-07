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


namespace MySql.Data.VisualStudio.Wizards.WindowsForms
{
  public partial class DataAccessConfig : WizardPage
  {
    private MySqlConnection _con;
    private string _constraintTable = "";
    List<string> _constraints = new List<string>();

    internal MySqlConnection Connection
    {
      get { return _con; }
    }

    internal GuiType GuiType
    {
      get
      {
        if (radControls.Checked) return GuiType.IndividualControls;
        else if (radGrid.Checked) return GuiType.Grid;
        else if (radMasterDetail.Checked) return GuiType.MasterDetail;
        else return GuiType.None;
      }
    }

    internal string TableName
    {
      get {
        if (cmbTable.SelectedIndex == -1) return null;
        else return ( string )cmbTable.SelectedItem;
      }
    }

    internal string ConstraintName
    {
      get
      {
        if (cmbFkConstraints.SelectedIndex == -1) return null;
        else return (string)cmbFkConstraints.SelectedItem;
      }
    }

    internal DataAccessTechnology DataAccessTechnology
    {
      get {
        if (radTechTypedDataSet.Checked) return DataAccessTechnology.TypedDataSet;
        else if (radEF5.Checked) return DataAccessTechnology.EntityFramework5;
        else if (radEF6.Checked) return DataAccessTechnology.EntityFramework6;
        else return DataAccessTechnology.None;
      }
    }

    public DataAccessConfig()
    {
      InitializeComponent();
    }

    private void btnConnConfig_Click(object sender, EventArgs e)
    {
      // Code for openning connection dialog.
      ConnectDialog dlg;
      
      dlg = _con == null ?  new ConnectDialog() : new ConnectDialog(new MySqlConnectionStringBuilder(_con.ConnectionString));         

      DialogResult res = dlg.ShowDialog();
      if (res == DialogResult.OK)
      {
        try
        {
          _con = (MySqlConnection)dlg.Connection;
          FillTables();
        }
        catch( Exception ex )
        {
          MessageBox.Show(string.Format("The connection string is not valid: {0}", ex.Message));
        }
        txtConnStr.Text = _con.ConnectionString;
      }
    }

    private void FillTables()
    {
      string[] restrictions = new string[ 4 ];
      restrictions[ 1 ] = _con.Database;
      DataTable t = _con.GetSchema( "Tables", restrictions );
      cmbTable.Items.Clear();
      for (int i = 0; i < t.Rows.Count; i++)
      {
        cmbTable.Items.Add(t.Rows[i][2]);
      }
    }

    private void DataAccessConfig_Validating(object sender, CancelEventArgs e)
    {
      e.Cancel = false;
      if (!IsConnectionStringValid())
      {
        e.Cancel = true;
        errorProvider1.SetError(txtConnStr, "A valid connection string must be entered.");
      }
      else
      {
        errorProvider1.SetError(txtConnStr, "");
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

      if ( ( radMasterDetail.Checked ) && 
        ( string.IsNullOrEmpty((string)cmbFkConstraints.SelectedItem) || (cmbFkConstraints.SelectedIndex == -1) ))
      {
        e.Cancel = true;
        errorProvider1.SetError(cmbFkConstraints, "A constraint name must be chosen for a Master Detail layout.");
      }
      else
      {
        errorProvider1.SetError(cmbFkConstraints, "");
      }
    }

    private bool IsConnectionStringValid()
    {
      if (_con != null && !string.IsNullOrEmpty(txtConnStr.Text) && 
        (_con.ConnectionString == txtConnStr.Text) && ((_con.State & ConnectionState.Open) != 0))
        return true;
      else
        return false;
    }

    internal override bool IsValid()
    {
      CancelEventArgs args = new CancelEventArgs();
      DataAccessConfig_Validating(this, args);
      if (args.Cancel) return false;
      else return true;
    }

    private void radMasterDetail_CheckedChanged(object sender, EventArgs e)
    {
      if (radMasterDetail.Checked)
      {
        List<string> constraints = GetForeignKeyConstraints();
        cmbFkConstraints.Items.Clear();
        for (int i = 0; i < constraints.Count; i++)
        {
          cmbFkConstraints.Items.Add(constraints[i]);
        }
        cmbFkConstraints.Enabled = true;
      }
    }

    private void radGrid_CheckedChanged(object sender, EventArgs e)
    {
      cmbFkConstraints.Enabled = false;
    }

    private void radControls_CheckedChanged(object sender, EventArgs e)
    {
      cmbFkConstraints.Enabled = false;
    }

    private List<string> GetForeignKeyConstraints()
    {
      if (TableName == _constraintTable) return _constraints;
      else { _constraintTable = TableName; _constraints.Clear(); }
      if (_con == null) return _constraints;
      string sql = string.Format(
        @"select `constraint_name` from information_schema.referential_constraints 
          where `constraint_schema` = '{0}' and `table_name` = '{1}';", _con.Database, _constraintTable);
      if ((_con.State & ConnectionState.Open) == 0)
        _con.Open();
      MySqlCommand cmd = new MySqlCommand(sql, _con);
      using (MySqlDataReader r = cmd.ExecuteReader())
      {
        while (r.Read())
        {
          _constraints.Add(r.GetString(0));
        }
      }
      return _constraints;
    }

    internal override void OnStarting(BaseWizardForm wizard)
    {
      // Enable EF6 only if we are in VS2013 or major
      double version = double.Parse( (( WindowsFormsWizardForm )wizard).Wizard.GetVisualStudioVersion() );
      if (version >= 12.0)
      {
        radEF6.Enabled = true;
      }
      else
      {
        radEF6.Enabled = false;
      }
    }
  }

  internal enum DataAccessTechnology : int
  {
    None = 0,
    TypedDataSet = 1,
    EntityFramework5 = 2,
    EntityFramework6 = 3
  }

  internal enum GuiType : int
  {
    None = 0,
    IndividualControls = 1,
    Grid = 2,
    MasterDetail = 3
  }
}
