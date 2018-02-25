// Copyright (c) 2008, 2014, Oracle and/or its affiliates. All rights reserved.
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
using MySql.Data.MySqlClient;

namespace MySql.Data.VisualStudio.Wizards.Web
{
  public partial class ProviderConfiguration : WizardPage
  {    
      
    internal bool createAdministratorUser
    {
      get {
        return createAdministratorUserCheck.Checked;
      }    
    }

    internal string adminName
    {
      get {
        return txtUserName.Text;
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

    internal string userQuestion
    {
      get {
        return txtQuestion.Text;
      }    
    }

    internal string userAnswer
    {
      get {
        return txtAnswer.Text;
      }
    }

    public ProviderConfiguration()
    {
      InitializeComponent();
      /* Loading defaults */            
      chkWriteExceptions.Checked = true;
      chkQuestionAndAnswerRequired.Checked = true;
      createAdministratorUserCheck.Checked = true;
      txtMinimumPasswordLenght.Text = "7";

      /* assign events */
      txtUserName.TextChanged += txtUserName_TextChanged;
      txtPwd.TextChanged += new EventHandler(txtPwd_TextChanged);
      txtPwdConfirm.TextChanged += new EventHandler(txtPwdConfirm_TextChanged);
      txtMinimumPasswordLenght.TextChanged += new EventHandler(txtMinimumPasswordLenght_TextChanged);
      createAdministratorUserCheck.CheckedChanged += createAdministratorUserCheck_CheckedChanged;
      txtQuestion.TextChanged += txtQuestion_TextChanged;
      txtAnswer.TextChanged += txtAnswer_TextChanged;
    }

    void txtAnswer_TextChanged(object sender, EventArgs e)
    {      
      if (!string.IsNullOrEmpty(txtAnswer.Text))
        errorProvider1.SetError(txtAnswer, "");      
    }

    void txtQuestion_TextChanged(object sender, EventArgs e)
    {
      if (!string.IsNullOrEmpty(txtQuestion.Text))
        errorProvider1.SetError(txtQuestion, "");      
    }

    void txtUserName_TextChanged(object sender, EventArgs e)
    {
      if (!string.IsNullOrEmpty(txtUserName.Text))
        errorProvider1.SetError(txtUserName, "");      
    }

    void createAdministratorUserCheck_CheckedChanged(object sender, EventArgs e)
    {
      txtPwd.Enabled = createAdministratorUserCheck.Checked;
      txtQuestion.Enabled = createAdministratorUserCheck.Checked;
      txtUserName.Enabled = createAdministratorUserCheck.Checked;
      txtAnswer.Enabled = createAdministratorUserCheck.Checked;
      txtPwdConfirm.Enabled = createAdministratorUserCheck.Checked;

      if (!createAdministratorUserCheck.Checked)
      {
        txtUserName.Text = string.Empty;
        txtPwd.Text = string.Empty;
        txtPwdConfirm.Text = string.Empty;
        errorProvider1.SetError(txtUserName, "");
        errorProvider1.SetError(txtPwd, "");
        errorProvider1.SetError(txtPwdConfirm, "");        
      }
    }

    void txtMinimumPasswordLenght_TextChanged(object sender, EventArgs e)
    {
      var pwdLenght = 7;
      if (int.TryParse(txtMinimumPasswordLenght.Text, out pwdLenght))
        errorProvider1.SetError(txtMinimumPasswordLenght, "");      
    }      

    void txtPwd_TextChanged(object sender, EventArgs e)
    {
      if (txtPwd.Text.Trim().Equals(txtPwdConfirm.Text.Trim()))
         errorProvider1.SetError(txtPwdConfirm, "");      
      
      var pwdLenght = 0;
      if (int.TryParse(txtMinimumPasswordLenght.Text, out pwdLenght))
      {
        if (txtPwd.Text.Length >= pwdLenght)
          errorProvider1.SetError(txtPwdConfirm, "");      
      }      
    }

    void txtPwdConfirm_TextChanged(object sender, EventArgs e)
    {
      if (txtPwdConfirm.Text.Trim().Equals(txtPwdConfirm.Text.Trim()))
        errorProvider1.SetError(txtPwdConfirm, "");

      var pwdLenght = 0;
      if (int.TryParse(txtMinimumPasswordLenght.Text, out pwdLenght))
      {
        if (txtPwdConfirm.Text.Length >= pwdLenght)
          errorProvider1.SetError(txtPwdConfirm, "");
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
     
      if (createAdministratorUserCheck.Checked)
      {
        if (string.IsNullOrEmpty(txtUserName.Text))
        {
          e.Cancel = true;
          errorProvider1.SetError(txtUserName, "User name cannot be empty");
        }

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


        var pwdLenght = 7;
        if (!int.TryParse(txtMinimumPasswordLenght.Text, out pwdLenght))
        {
          e.Cancel = true;
          errorProvider1.SetError(txtMinimumPasswordLenght, "Password length should be a integer number");
        }

        if (txtPwd.Text.Length < pwdLenght)
        {
          e.Cancel = true;
          errorProvider1.SetError(txtPwd, "Administrator Password length is not valid");
        }


        if (chkQuestionAndAnswerRequired.Checked)
        {
          if (string.IsNullOrEmpty(txtQuestion.Text))
          {
            e.Cancel = true;
            errorProvider1.SetError(txtQuestion, "Question is required.");
          }
          
          
          if (string.IsNullOrEmpty(txtAnswer.Text))
          {
            e.Cancel = true;
            errorProvider1.SetError(txtAnswer, "Answer is required.");
          }
        
        }
      }

  
    }

    private void chkQuestionAndAnswerRequired_CheckedChanged(object sender, EventArgs e)
    {
      var control = (CheckBox)sender;       
      if (createAdministratorUserCheck.Checked)
      {
        txtAnswer.Enabled = control.Checked;
        txtQuestion.Enabled = control.Checked;
        if (!control.Checked)
        {
          txtQuestion.Text = string.Empty;
          txtAnswer.Text = string.Empty;
          errorProvider1.SetError(txtAnswer, "");
          errorProvider1.SetError(txtQuestion, "");
        }
      }
    }
  }
}
