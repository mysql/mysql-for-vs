﻿using System;
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
        return ConnectionStringTextBox.Text;
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
          var conn = new MySqlConnection(ConnectionStringTextBox.Text);
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

      var cnn = new MySqlConnection(ConnectionStringTextBox.Text);
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
