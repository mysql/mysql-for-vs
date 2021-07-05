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
using MySql.Data.VisualStudio;
using MySql.Data.MySqlClient;
using MySql.Data.VisualStudio.Common;

namespace MySql.Data.VisualStudio.Wizards.WindowsForms
{
  public partial class DataAccessTechnologyConfig : WizardPage
  {    
    private string _constraintTable = "";
    private AdvancedWizardForm wizardForm;
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
      set
      {
        if ((GuiType)value == GuiType.IndividualControls)
        radControls.Checked = true;
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

    public DataAccessTechnologyConfig()
    {
      InitializeComponent();
      chkEnableAdvanced.CheckedChanged -= chkEnableAdvanced_CheckedChanged;
      chkEnableAdvanced.CheckedChanged += chkEnableAdvanced_CheckedChanged;
      cmbFkConstraints.DropDown -= cmbFkConstraints_DropDown;
      cmbFkConstraints.DropDown += cmbFkConstraints_DropDown;
      radMasterDetail.CheckedChanged -= radMasterDetail_CheckedChanged;
      radMasterDetail.CheckedChanged += radMasterDetail_CheckedChanged;
    }

    void chkEnableAdvanced_CheckedChanged(object sender, EventArgs e)
    {
      var control = (CheckBox)sender;
      
      wizardForm.SetSkipPage(WindowsFormsWizardForm.DETAIL_VALIDATION_CONFIG_PAGE_IDX, !control.Checked || !radMasterDetail.Checked );
      wizardForm.SetSkipPage(WindowsFormsWizardForm.VALIDATION_CONFIG_PAGE_IDX, !control.Checked);
    }

    private void SetDefaults()
    {
      radControls.Checked = true;
      radControls_CheckedChanged(radControls, EventArgs.Empty);
      chkEnableAdvanced.Checked = false;
      chkEnableAdvanced_CheckedChanged(chkEnableAdvanced, EventArgs.Empty);
    }

    void cmbFkConstraints_DropDown(object sender, EventArgs e)
    {
      GetForeignKeyConstraints();
    }

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
      {
        _con.OpenWithDefaultTimeout();
      }

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
        wizardForm.SetSkipPage(WindowsFormsWizardForm.DETAIL_VALIDATION_CONFIG_PAGE_IDX, !chkEnableAdvanced.Checked);
        wizardForm.SetSkipPage(WindowsFormsWizardForm.VALIDATION_CONFIG_PAGE_IDX, !chkEnableAdvanced.Checked);
      }
      else
      {
        cmbFkConstraints.Items.Clear();
        cmbFkConstraints.Enabled = false;
        wizardForm.SetSkipPage(WindowsFormsWizardForm.DETAIL_VALIDATION_CONFIG_PAGE_IDX, true);
        wizardForm.SetSkipPage(WindowsFormsWizardForm.VALIDATION_CONFIG_PAGE_IDX, !chkEnableAdvanced.Checked);
      }
    }

    private void radGrid_CheckedChanged(object sender, EventArgs e)
    {
      var control = (RadioButton)sender;
      cmbFkConstraints.Enabled = !control.Checked;
      wizardForm.SetSkipPage(WindowsFormsWizardForm.VALIDATION_CONFIG_PAGE_IDX, !chkEnableAdvanced.Checked);
      wizardForm.SetSkipPage(WindowsFormsWizardForm.DETAIL_VALIDATION_CONFIG_PAGE_IDX, true );
      if (control.Checked)
        errorProvider1.SetError(cmbFkConstraints, "");
    }

    private void radControls_CheckedChanged(object sender, EventArgs e)
    {
      var control = (RadioButton)sender;
      cmbFkConstraints.Enabled = !control.Checked;
      wizardForm.SetSkipPage(WindowsFormsWizardForm.VALIDATION_CONFIG_PAGE_IDX, !chkEnableAdvanced.Checked);
      wizardForm.SetSkipPage(WindowsFormsWizardForm.DETAIL_VALIDATION_CONFIG_PAGE_IDX, true);
      if (control.Checked)
         errorProvider1.SetError(cmbFkConstraints, "");
    }

    internal override void OnStarting(BaseWizardForm wizard)
    {
      wizardForm = (AdvancedWizardForm)wizard;
      SetDefaults();   
      _tableName = wizardForm.TableName;
       lblTableName.Text = "Select form layout for table " + _tableName;
      _con = new MySqlConnection(wizardForm.ConnectionString);
      _con.OpenWithDefaultTimeout();
    }   
  }
}
