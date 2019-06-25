// Copyright (c) 2008, 2019, Oracle and/or its affiliates. All rights reserved.
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

/*
 * This file contains implementation of custom connection dialog. 
 */
using System;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.VisualStudio.Data.AdoDotNet;
using MySql.Data.VisualStudio.Properties;
using System.Data.Common;
using Microsoft.VisualStudio.Data;
using MySql.Utility.Classes.Logging;

namespace MySql.Data.VisualStudio
{
  /// <summary>
  /// Represents a custom data connection UI control for entering
  /// connection information.
  /// </summary>
  internal partial class MySqlDataConnectionUI : DataConnectionUIControl //Stub
  {
    private bool dbListPopulated;

    public MySqlDataConnectionUI()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Initializes GUI elements with properties values.
    /// </summary>
    public override void LoadProperties()
    {
      if (ConnectionProperties == null)
      {
        Logger.LogError(Properties.Resources.ConnectionPropertiesNull, true);
        throw new Exception(Properties.Resources.ConnectionPropertiesNull);
      }

      Button okButton = this.ParentForm.AcceptButton as Button;
      okButton.Click += new EventHandler(okButton_Click);

      MySqlConnectionProperties prop =
                (ConnectionProperties as MySqlConnectionProperties);
      DbConnectionStringBuilder cb = prop.ConnectionStringBuilder;

      loadingInProcess = true;
      try
      {
        serverNameTextBox.Text = (string)cb["Server"];
        userNameTextBox.Text = (string)cb["User Id"];
        passwordTextBox.Text = (string)cb["Password"];
        dbList.Text = (string)cb["Database"];
        savePasswordCheckBox.Checked = (bool)cb["Persist Security Info"];
      }
      finally
      {
        loadingInProcess = false;
      }
    }

    /// <summary>
    /// We hook the ok button of our parent form so we can implement our 
    /// 'create database' functionality
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void okButton_Click(object sender, EventArgs e)
    {
      // Verify if the connection is a MySql connection.
      if (!IsMySqlConnection())
      {
        return;
      }

      bool exists = DatabaseExists();
      if (exists)
      {
        return;
      }

      var prompt = string.Format(Properties.Resources.UnknownDbPromptCreate, dbList.Text);
      prompt = prompt.Replace(@"\n", @"\n");
      using (var yesNoDialog = Common.Utilities.GetYesNoInfoDialog(
                                 Utility.Forms.InfoDialog.InfoType.Info,
                                 false,
                                 "Create database",
                                 prompt
      ))
      {
        if (yesNoDialog.ShowDialog() == DialogResult.Yes)
        {
          if (!AttemptToCreateDatabase())
          {
            Logger.LogError(string.Format(Properties.Resources.ErrorAttemptingToCreateDB, dbList.Text), true);
          }
          else
          {
            this.ParentForm.DialogResult = DialogResult.OK;
          }
        }

        this.ParentForm.DialogResult = DialogResult.None;
      }
    }

    /// <summary>
    /// Verify if the stored connection properties can be parsed to a MySql connection properties type
    /// </summary>
    /// <returns>True for MySql connection properties or false for other types</returns>
    private bool IsMySqlConnection()
    {
      MySqlConnectionProperties prop = (ConnectionProperties as MySqlConnectionProperties);
      return !string.IsNullOrEmpty(prop.ConnectionStringBuilder.ConnectionString);
    }

    /// <summary>
    /// Handles TextChanged event from all textboxes.
    /// </summary>
    /// <param name="sender">Reference to sender object.</param>
    /// <param name="e">Additional event data. Not used.</param>
    private void SetProperty(object sender, EventArgs e)
    {
      try
      {
        // Only set properties if we are not currently loading them
        if (!loadingInProcess)
        {
          Control source = sender as Control;

          // if the user changes the host or user id then
          // we need to repopulate our db list
          if (source.Tag.Equals("Server") ||
              source.Tag.Equals("User id"))
            dbListPopulated = false;

          // Tag is used to determine property name
          if (source != null && source.Tag is string)
            ConnectionProperties[source.Tag as string] = source.Text;
        }
        dbList.Enabled = serverNameTextBox.Text.Trim().Length > 0 &&
            userNameTextBox.Text.Trim().Length > 0;
      }
      catch { }
    }

    /// <summary>
    /// Handles Leave event and trims text.
    /// </summary>
    /// <param name="sender">Reference to sender object.</param>
    /// <param name="e">Additional event data. Not used.</param>
    private void TrimControlText(object sender, EventArgs e)
    {
      Control c = sender as Control;
      c.Text = c.Text.Trim();
    }

    /// <summary>
    /// Used to prevent OnChange event handling during initialization.
    /// </summary>
    private bool loadingInProcess = false;

    private void SavePasswordChanged(object sender, EventArgs e)
    {
      if (ConnectionProperties == null) return;
      // Only set properties if we are not currently loading them
      if (!loadingInProcess)
        ConnectionProperties[savePasswordCheckBox.Tag as string]
            = savePasswordCheckBox.Checked;
    }

    private void dbList_DropDown(object sender, EventArgs e)
    {
      if (dbListPopulated) return;

      MySqlConnectionProperties prop =
          (ConnectionProperties as MySqlConnectionProperties);
      DbConnectionStringBuilder cb = prop.ConnectionStringBuilder;

      try
      {
        using (MySqlConnectionSupport conn = new MySqlConnectionSupport())
        {
          conn.Initialize(null);
          conn.ConnectionString = cb.ConnectionString;
          conn.Open(false);
          dbList.Items.Clear();
          using (DataReader reader = conn.Execute("SHOW DATABASES", 1, null, 0))
          {
            while (reader.Read())
            {
              string dbName = reader.GetItem(0).ToString().ToLowerInvariant();
              if (dbName == "information_schema") continue;
              if (dbName == "mysql") continue;
              dbList.Items.Add(reader.GetItem(0));
            }
            dbListPopulated = true;
          }
        }
      }
      catch (Exception)
      {
        Logger.LogError(Properties.Resources.UnableToRetrieveDatabaseList, true);
      }
    }

    private bool DatabaseExists()
    {
      MySqlConnectionProperties prop = ConnectionProperties as MySqlConnectionProperties;
      DbConnectionStringBuilder cb = prop.ConnectionStringBuilder;
      try
      {
        using (MySqlConnectionSupport conn = new MySqlConnectionSupport())
        {
          conn.Initialize(null);
          conn.ConnectionString = cb.ConnectionString;
          conn.Open(false);
        }

        return true;
      }
      catch (DbException exception)
      {
        string msg = exception.Message.ToLowerInvariant();
        if (msg.ToLower().Contains("unknown database"))
        {
          return false;
        }

        if (exception.InnerException == null)
        {
          throw exception;
        }

        var throwException = Common.Utilities.GetExceptionWithFullNestedMessage(exception);
        Logger.LogException(throwException);
        throw throwException;
      }
    }

    private bool AttemptToCreateDatabase()
    {
      MySqlConnectionProperties prop =
          (ConnectionProperties as MySqlConnectionProperties);
      DbConnectionStringBuilder cb = prop.ConnectionStringBuilder;

      string olddb = (string)cb["Database"];
      cb["Database"] = "";
      try
      {
        using (MySqlConnectionSupport conn = new MySqlConnectionSupport())
        {
          conn.Initialize(null);
          conn.ConnectionString = cb.ConnectionString;
          conn.Open(false);
          conn.ExecuteWithoutResults("CREATE DATABASE `" + dbList.Text + "`", 1, null, 0);
        }
        return true;
      }
      catch (Exception)
      {
        Logger.LogError(string.Format(Properties.Resources.ErrorAttemptingToCreateDB, dbList.Text), true);
        return false;
      }
      finally
      {
        cb["Database"] = olddb;
      }
    }
  }
}
