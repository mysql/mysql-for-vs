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

namespace MySql.Data.VisualStudio.Wizards.Web
{
  public partial class ProviderConfiguration : WizardPage
  {
   
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

    internal bool createAdministratorUser
    {
      get {
        return createAdministratorUserCheck.Checked;
      }    
    }

    internal string adminPassword
    {
      get {
        return txtPwd.Text;
      }
    }

    internal bool writeExceptionsToLog
    {
      get
      {
        return chkWriteExceptions.Checked;
      }
    }

    internal int minimumPasswordLenght
    {
      get
      {
        return int.Parse(txtMinimumPasswordLenght.Text);
      }
    }

    internal bool requireQuestionAndAnswer
    {
      get {
        return chkQuestionAndAnswerRequired.Checked;
      }
    }

    public ProviderConfiguration()
    {
      InitializeComponent();
      /* Loading defaults */      
      ConnectionStringNameTextBox.Text = "LocalMySqlServer";
      includeRoleProviderCheck.Checked = true;
      includeProfileProviderCheck.Checked = true;
      chkWriteExceptions.Checked = true;
      chkQuestionAndAnswerRequired.Checked = true;
      createAdministratorUserCheck.Checked = true;
      txtMinimumPasswordLenght.Text = "7";

      /* assign events */
      ConnectionStringTextBox.TextChanged += new EventHandler(ConnectionStringTextBox_TextChanged);
      txtPwd.TextChanged += new EventHandler(txtPwd_TextChanged);
      txtPwdConfirm.TextChanged += new EventHandler(txtPwdConfirm_TextChanged);
      txtMinimumPasswordLenght.TextChanged += new EventHandler(txtMinimumPasswordLenght_TextChanged);
    }

    void txtMinimumPasswordLenght_TextChanged(object sender, EventArgs e)
    {
      var pwdLenght = 7;
      if (int.TryParse(txtMinimumPasswordLenght.Text, out pwdLenght))
        errorProvider1.SetError(txtMinimumPasswordLenght, "");      
    }    

    private void editConnString_Click(object sender, EventArgs e)
    {
      ConnectDialog dlg = new ConnectDialog();
      try
      {

        DialogResult res = dlg.ShowDialog();
        if (res == DialogResult.OK)
        {
          ConnectionStringTextBox.Text = ((MySqlConnection)dlg.Connection).ConnectionString;
          var csb = (MySqlConnectionStringBuilder)((MySqlConnection)dlg.Connection).GetType().GetProperty("Settings", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(((MySqlConnection)dlg.Connection), null);
          if (csb != null)
          {
            ConnectionStringTextBox.Tag = csb.ConnectionString;
          }
          var conn = new MySqlConnection(ConnectionStringTextBox.Tag.ToString());
          conn.Open();
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(string.Format("The connection string is not valid: {0}", ex.Message));
      }      
    }

    private void ConnectionStringTextBox_TextChanged(object sender, EventArgs e)
    {
      if (!String.IsNullOrEmpty(ConnectionStringTextBox.Text))
        errorProvider1.SetError(ConnectionStringTextBox, "");
    }

    void txtPwd_TextChanged(object sender, EventArgs e)
    {
      if (txtPwd.Text.Trim().Equals(txtPwdConfirm.Text.Trim()))
        errorProvider1.SetError(txtPwd, "");
    }

    void txtPwdConfirm_TextChanged(object sender, EventArgs e)
    {
      if (txtPwd.Text.Trim().Equals(txtPwdConfirm.Text.Trim()))
        errorProvider1.SetError(txtPwd, "");    
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
      if (!IsConnectionValid())
      {
        e.Cancel = true;
        errorProvider1.SetError(ConnectionStringTextBox, "A valid connection string must be entered.");
      }
      else
      {
        errorProvider1.SetError(ConnectionStringTextBox, "");
      }

      if (createAdministratorUserCheck.Checked)
      {
        if (string.IsNullOrEmpty(txtPwd.Text) || string.IsNullOrEmpty(txtPwdConfirm.Text))
        {
          e.Cancel = true;
          errorProvider1.SetError(txtPwd, "Passowrd cannot be empty");
        }
        if (!txtPwd.Text.Trim().Equals(txtPwdConfirm.Text.Trim()))
        {
          e.Cancel = true;
          errorProvider1.SetError(txtPwd, "Passowrd doesn't match with the password confirmation");
        }
      }

      var pwdLenght = 7;
      if (!int.TryParse(txtMinimumPasswordLenght.Text, out pwdLenght))
      {
        e.Cancel = true;
        errorProvider1.SetError(txtMinimumPasswordLenght, "Password lenght should be a integer number");
      }
    }

    private bool IsConnectionValid()
    {
      if (String.IsNullOrEmpty(ConnectionStringTextBox.Text))
        return false;

      var cnn = new MySqlConnection(ConnectionStringTextBox.Tag.ToString());
      try
      {
        cnn.Open();
        cnn.Close();
      }
      catch { return false; }

      return true;
    }


  }
}
