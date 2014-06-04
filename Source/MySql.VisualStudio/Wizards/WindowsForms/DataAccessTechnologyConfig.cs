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
  public partial class DataAccessTechnologyConfig : WizardPage
  {    
    private string _constraintTable = "";
    private WindowsFormsWizardForm wizardForm;
    List<MyListItem> _constraints = new List<MyListItem>();
    private string _tableName;
    private MySqlConnection _con;

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
  
    internal string ConstraintName
    {
      get
      {
        if (cmbFkConstraints.SelectedIndex == -1) return null;
        else return ((MyListItem)(cmbFkConstraints.SelectedItem)).Name;
      }
    }

    internal string DetailTableName
    {
      get
      {
        if (cmbFkConstraints.SelectedIndex == -1) return null;
        else return ((MyListItem)(cmbFkConstraints.SelectedItem)).Value;
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

    public DataAccessTechnologyConfig()
    {
      InitializeComponent();
      //cmbFkConstraints.DropDown += cmbFkConstraints_DropDown;
    }

    private void SetDefaults()
    {
      radControls.Checked = true;
      radControls_CheckedChanged(radControls, EventArgs.Empty);
    }

    //void cmbFkConstraints_DropDown(object sender, EventArgs e)
    //{
    //  GetForeignKeyConstraints();
    //}

    private List<MyListItem> GetForeignKeyConstraints()
    {
      if (_tableName == _constraintTable) return _constraints;
      else { _constraintTable = _tableName; _constraints.Clear(); }
      if (_con == null) return _constraints;
      string sql = string.Format(
        @"select `constraint_name`, `referenced_table_name` from information_schema.referential_constraints 
          where `constraint_schema` = '{0}' and `table_name` = '{1}'
          union
          select `constraint_name`, `table_name` from information_schema.referential_constraints 
          where `constraint_schema` = '{0}' and `referenced_table_name` = '{1}';", _con.Database, _constraintTable);
      if ((_con.State & ConnectionState.Open) == 0)
        _con.Open();
      MySqlCommand cmd = new MySqlCommand(sql, _con);
      using (MySqlDataReader r = cmd.ExecuteReader())
      {
        while (r.Read())
        {
          _constraints.Add(new MyListItem(r.GetString(0), r.GetString(1)));
        }
      }
      return _constraints;
    }


    private void DataAccessTechnologyConfig_Validating(object sender, CancelEventArgs e)
    {
      e.Cancel = false;
  
      if ( ( radMasterDetail.Checked ) && 
        ( cmbFkConstraints.SelectedIndex == -1) )
      {
        e.Cancel = true;
        errorProvider1.SetError(cmbFkConstraints, "A constraint name must be chosen for a Master Detail layout.");
      }
      else
      {
        errorProvider1.SetError(cmbFkConstraints, "");
      }
    }
   

    internal override bool IsValid()
    {
      CancelEventArgs args = new CancelEventArgs();
      DataAccessTechnologyConfig_Validating(this, args);
      if (args.Cancel) return false;
      else return true;
    }

    private void radMasterDetail_CheckedChanged(object sender, EventArgs e)
    {
      if (radMasterDetail.Checked)
      {
        List<MyListItem> constraints = GetForeignKeyConstraints();
        cmbFkConstraints.Items.Clear();
        for (int i = 0; i < constraints.Count; i++)
        {
          cmbFkConstraints.Items.Add(constraints[i]);
        }
        cmbFkConstraints.Enabled = true;
        cmbFkConstraints.ValueMember = "Value";
        cmbFkConstraints.DisplayMember = "Name";
        wizardForm.SetSkipPage(WindowsFormsWizardForm.DETAIL_VALIDATION_CONFIG_PAGE_IDX, false);
      }
      else
      {
        cmbFkConstraints.Items.Clear();
        cmbFkConstraints.Enabled = false;
        wizardForm.SetSkipPage(WindowsFormsWizardForm.DETAIL_VALIDATION_CONFIG_PAGE_IDX, true);
      }
    }

    private void radGrid_CheckedChanged(object sender, EventArgs e)
    {
      var control = (RadioButton)sender;
      cmbFkConstraints.Enabled = !control.Checked;
      wizardForm.SetSkipPage(WindowsFormsWizardForm.DETAIL_VALIDATION_CONFIG_PAGE_IDX, true);
      if (control.Checked)
        errorProvider1.SetError(cmbFkConstraints, "");
    }

    private void radControls_CheckedChanged(object sender, EventArgs e)
    {
      var control = (RadioButton)sender;
      cmbFkConstraints.Enabled = !control.Checked;
      wizardForm.SetSkipPage(WindowsFormsWizardForm.DETAIL_VALIDATION_CONFIG_PAGE_IDX, true);
      if (control.Checked)
         errorProvider1.SetError(cmbFkConstraints, "");
    }

    internal override void OnStarting(BaseWizardForm wizard)
    {
      wizardForm = (WindowsFormsWizardForm)wizard;
      SetDefaults();
      // Enable EF6 only if we are in VS2013 or major
      double version = double.Parse(wizardForm.Wizard.GetVisualStudioVersion());
      _tableName = wizardForm.TableName;
      _con = wizardForm.Connection;

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
}
