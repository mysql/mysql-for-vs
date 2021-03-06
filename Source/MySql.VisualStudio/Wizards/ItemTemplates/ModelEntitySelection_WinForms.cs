﻿// Copyright © 2015, 2016, Oracle and/or its affiliates. All rights reserved.
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
using System.Data;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using MySql.Utility.Classes.MySql;

namespace MySql.Data.VisualStudio.Wizards.ItemTemplates
{
  public partial class ModelEntitySelection_WinForms : ModelEntitySelection
  {
    private string _constraintTable = "";
    List<MyListItem> _constraints = new List<MyListItem>();
    private MySqlConnection _con;

    public ModelEntitySelection_WinForms()
    {
      InitializeComponent();
      cmbFkConstraints.DropDown -= cmbFkConstraints_DropDown;
      cmbFkConstraints.DropDown += cmbFkConstraints_DropDown;
      radMasterDetail.CheckedChanged -= radMasterDetail_CheckedChanged;
      radMasterDetail.CheckedChanged += radMasterDetail_CheckedChanged;
      radGrid.CheckedChanged -= radGrid_CheckedChanged;
      radGrid.CheckedChanged += radGrid_CheckedChanged;
      radControls.CheckedChanged -= radControls_CheckedChanged;
      radControls.CheckedChanged += radControls_CheckedChanged;
      radControls.Checked = true;
      radControls_CheckedChanged(radControls, EventArgs.Empty);
    }

    internal ComboBox CmbFkConstraints { get { return cmbFkConstraints; } }
    internal System.Windows.Forms.RadioButton RadControls { get { return radControls; } }
    internal System.Windows.Forms.RadioButton RadGrid { get { return radGrid; } }
    internal System.Windows.Forms.RadioButton RadMasterDetail { get { return radMasterDetail; } }
    internal System.Windows.Forms.Panel PnlLayout { get { return pnlLayout; } }
    internal string DetailEntityName
    {
      get
      {
        return cmbFkConstraints.SelectedIndex == -1
          ? null
          : ((MyListItem)(cmbFkConstraints.SelectedItem)).Value;
      }
    }
    internal string ConstraintName
    {
      get
      {
        return cmbFkConstraints.SelectedIndex == -1
          ? null
          : ((MyListItem)(cmbFkConstraints.SelectedItem)).Name;
      }
    }

    internal override void EntitySelection_Validating(object sender, CancelEventArgs e)
    {
      base.EntitySelection_Validating(sender, e);

      if (e.Cancel)
      {
        return;
      }

      if (radMasterDetail.Checked &&
          (string.IsNullOrEmpty(cmbFkConstraints.Text) || cmbFkConstraints.Items.Cast<MyListItem>().FirstOrDefault(i => i.Value == cmbFkConstraints.Text) == null))
      {
        e.Cancel = true;
      }
    }

    private void cmbFkConstraints_DropDown(object sender, EventArgs e)
    {
      GetForeignKeyConstraints();
    }

    internal void radControls_CheckedChanged(object sender, EventArgs e)
    {
      if (radControls.Checked)
      {
        cmbFkConstraints.Enabled = false;
      }
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
        cmbFkConstraints.DisplayMember = "Value";
      }
      else
      {
        cmbFkConstraints.Items.Clear();
        cmbFkConstraints.Enabled = false;
      }
    }

    private void radGrid_CheckedChanged(object sender, EventArgs e)
    {
      cmbFkConstraints.Enabled = !radGrid.Checked;
    }

    private List<MyListItem> GetForeignKeyConstraints()
    {
      _con = new MySqlConnection(ConnectionString);
      _con.Open();

      if (SelectedEntity == _constraintTable)
      {
        return _constraints;
      }

      _constraintTable = SelectedEntity;
      _constraints.Clear();
      if (_con == null)
      {
        return _constraints;
      }

      string sql = string.Format(
        @"select `constraint_name`, `referenced_table_name` from information_schema.referential_constraints
          where `constraint_schema` = '{0}' and `table_name` = '{1}'
          union
          select `constraint_name`, `table_name` from information_schema.referential_constraints
          where `constraint_schema` = '{0}' and `referenced_table_name` = '{1}';", _con.Database, _constraintTable);

      if ((_con.State & ConnectionState.Open) == 0)
      {
        _con.Open();
      }

      try
      {
        MySqlCommand cmd = new MySqlCommand(sql, _con);
        using (MySqlDataReader r = cmd.ExecuteReader())
        {
          while (r.Read())
          {
            _constraints.Add(new MyListItem(r.GetString(0), r.GetString(1)));
          }
        }
      }
      catch (MySqlException ex)
      {
        MySqlSourceTrace.WriteAppErrorToLog(ex, true);
      }

      return _constraints;
    }

    internal override void comboEntities_Toggle()
    {
      cmbFkConstraints.Items.Clear();
      cmbFkConstraints.Text = string.Empty;
      cmbFkConstraints.Enabled = false;
      pnlLayout.Enabled = false;
      radControls.Checked = true;

      var enable = Entities_IsValid();
      ToggleWizardBtnFinish(enable);
      pnlLayout.Enabled = enable;
    }
  }
}
