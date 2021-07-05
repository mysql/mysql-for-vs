// Copyright (c) 2013, 2021, Oracle and/or its affiliates.
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
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using MySql.Data.VisualStudio.Common;
using MySql.Utility.Classes.Logging;

namespace MySql.Data.VisualStudio
{
  public partial class MySqlNewPasswordDialog : Form
  {
    MySqlConnection _connection;

    public MySqlNewPasswordDialog(MySqlConnection connection)
    {
      _connection = connection;
      InitializeComponent();
    }

    private void btnOk_Click(object sender, EventArgs e)
    {
      try
      {
        errorProvider1.Clear();
        if (txtPassword.Text != txtConfirm.Text)
        {
          errorProvider1.SetError(txtConfirm, Properties.Resources.NewPassword_PasswordNotMatch);
          return;
        }
        if (string.IsNullOrEmpty(txtPassword.Text))
        {
          errorProvider1.SetError(txtPassword, Properties.Resources.NewPassword_ProvideNewPassword);
          return;
        }

        MySqlCommand cmd = new MySqlCommand(string.Format("SET PASSWORD = PASSWORD('{0}')", txtPassword.Text), _connection);
        cmd.ExecuteNonQuery();
        _connection.Close();
        _connection.OpenWithDefaultTimeout();
        this.Close();
      }
      catch (Exception ex)
      {
        Logger.LogError(ex.Message, true);
      }
    }
    

    private void btnCancel_Click(object sender, EventArgs e)
    {

    }
  }
}
