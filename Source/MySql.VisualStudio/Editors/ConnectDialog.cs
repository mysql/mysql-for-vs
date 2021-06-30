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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.Common;
using MySql.Data.MySqlClient;
using MySql.Data.VisualStudio.Properties;
using MySql.Utility.Classes.Logging;
using MySql.Utility.Forms;
using MySql.Utility.Classes.VisualStyles;

namespace MySql.Data.VisualStudio
{
  public partial class ConnectDialog : Form
  {
    #region Constants

    private const int BIG_SIZE = 522;

    private const int SMALL_SIZE = 340;

    #endregion

    #region Fields

    /// <summary>
    /// Flag to indicate if the advanced view is currently visible.
    /// </summary>
    private bool _advancedViewEnabled;

    private DbConnectionStringBuilder _connectionStringBuilder;

    private DbProviderFactory _factory;

    private bool _populated = false;

    #endregion

    public ConnectDialog()
    {
      InitializeComponent();
      HandleDpiSizeConversions = true;
      _advancedViewEnabled = false;
      _factory = MySqlClientFactory.Instance;
      if (_factory == null)
      {
        throw new Exception("MySql Data Provider is not correctly registered");
      }

      _connectionStringBuilder = _factory.CreateConnectionStringBuilder();
      connectionProperties.SelectedObject = _connectionStringBuilder;
      btnRefresh.Click += btnRefresh_Click;
      txtPort.Leave += txtPort_Leave;
    }

    void database_GotFocus(object sender, EventArgs e)
    {
      //if (populated && !NeedsUpdate) return;
      //GetDatabases();
    }

    public ConnectDialog(MySqlConnectionStringBuilder settings)
      : this()
    {
      if (settings != null)
      {
        serverName.Text = settings.Server;
        userId.Text = settings.UserID;
        password.Text = settings.Password;
        database.Text = settings.Database;
        _connectionStringBuilder["Port"] = txtPort.Text = settings.Port.ToString();
        ReadFields();
      }
    }

    public DbConnection Connection
    {
      get
      {
        return GetConnection(false);
      }
      set
      {
        if (value != null)
        {
          _connectionStringBuilder.ConnectionString = value.ConnectionString;
          Rebind();
        }
      }
    }

    public bool HandleDpiSizeConversions { get; set; }

    private void advancedButton_Click(object sender, EventArgs e)
    {
      this.SuspendLayout();
      if (_advancedViewEnabled)
      {
        advancedButton.Text = "Advanced >>";
        Height = HandleDpiSizeConversions
          ? (int) (SMALL_SIZE * this.GetDpiScaleY())
          : SMALL_SIZE;
        simplePanel.Visible = true;
        connectionProperties.Visible = false;
        Rebind();
      }
      else
      {
        advancedButton.Text = "Simple <<";
        Height = HandleDpiSizeConversions
          ? (int) (BIG_SIZE * this.GetDpiScaleY())
          : BIG_SIZE;
        simplePanel.Visible = false;
        connectionProperties.Visible = true;
      }

      _advancedViewEnabled = !_advancedViewEnabled;
      this.ResumeLayout();
    }

    private void Rebind()
    {
      serverName.Text = _connectionStringBuilder["server"] as string;
      userId.Text = _connectionStringBuilder["userid"] as string;
      password.Text = _connectionStringBuilder["password"] as string;
      database.Text = _connectionStringBuilder["database"] as string;

      int port = 0;
      if (_connectionStringBuilder["port"] != null)
      {
        int.TryParse(_connectionStringBuilder["port"].ToString(), out port);
      }
      else
      {
        port = 3306;
      }

      txtPort.Text = port.ToString();
    }

    private void database_DropDown(object sender, EventArgs e)
    {
      if (_populated)
      {
        return;
      }

      database.Items.Add("Loading databases...");
      GetDatabases();
    }

    private void serverName_Leave(object sender, EventArgs e)
    {
      _connectionStringBuilder["server"] = serverName.Text.Trim();
    }

    private void userId_Leave(object sender, EventArgs e)
    {
      _connectionStringBuilder["userid"] = userId.Text.Trim();
    }

    private void password_Leave(object sender, EventArgs e)
    {
      _connectionStringBuilder["password"] = password.Text.Trim();
    }

    private void database_Leave(object sender, EventArgs e)
    {
      _connectionStringBuilder["database"] = database.Text.Trim();
    }

    private void txtPort_Leave(object sender, EventArgs e)
    {
      int port;
      _connectionStringBuilder["Port"] = !int.TryParse(txtPort.Text, out port) ? 3306 : port;
    }

    private void GetDatabases()
    {
      try
      {
        ReadFields();
        
        using (DbConnection c = _factory.CreateConnection())
        {
          c.ConnectionString = _connectionStringBuilder.ConnectionString;
          c.Open();
          DbCommand cmd = c.CreateCommand();
          cmd.CommandText = "SHOW DATABASES";
          database.Items.Clear();
          using (DbDataReader reader = cmd.ExecuteReader())
          {
            while (reader.Read())
            {
              var name = reader.GetString(0);
              if (name.IndexOf("information_schema", StringComparison.InvariantCultureIgnoreCase) != 0
                  && name.IndexOf("performance_schema", StringComparison.InvariantCultureIgnoreCase) != 0)
                database.Items.Add(reader.GetString(0));
            }
          }
        }

        _populated = true;
      }
      catch (MySqlException ex)
      {
        Logger.LogError(ex.Message, true);
      }
    }

    private void connectButton_Click(object sender, EventArgs e)
    {
      int port;
      // Ensure all data is populated into the connection string builder.
      _connectionStringBuilder["server"] = serverName.Text.Trim() == string.Empty ? "localhost" : serverName.Text.Trim();
      _connectionStringBuilder["userid"] = userId.Text.Trim() == string.Empty ? "root" : userId.Text.Trim();
      _connectionStringBuilder["database"] = database.Text.Trim() == string.Empty ? "test" : database.Text.Trim();
      _connectionStringBuilder["port"] = !int.TryParse(txtPort.Text, out port) ? 3306 : port;
      _connectionStringBuilder["persistsecurityinfo"] = true;
      password_Leave(serverName, EventArgs.Empty);

      if (!_populated)
      {
        GetConnection(true);
      }
    }

    private void ReadFields()
    {
      int port;
      _connectionStringBuilder["server"] = serverName.Text.Trim();
      _connectionStringBuilder["userid"] = userId.Text.Trim();
      _connectionStringBuilder["password"] = password.Text.Trim();
      _connectionStringBuilder["database"] = database.Text.Trim();
      _connectionStringBuilder["port"] = !int.TryParse(txtPort.Text, out port) ? 3306 : port;
    }

    private bool AttemptToCreateDatabase(string connectionString)
    {
      try
      {
        using (MySqlConnectionSupport conn = new MySqlConnectionSupport())
        {
          var csb = new MySqlConnectionStringBuilder(connectionString);
          csb.Database = string.Empty;
          conn.Initialize(null);
          conn.ConnectionString = csb.ConnectionString;
          conn.Open(false);
          conn.ExecuteWithoutResults("CREATE DATABASE `" + database.Text + "`", 1, null, 0);
        }
        return true;
      }
      catch (Exception)
      {
        Logger.LogError(string.Format(Properties.Resources.ErrorAttemptingToCreateDB, database.Text), true);
        return false;
      }
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if ((this.ActiveControl == database) && (keyData == Keys.Return))
      {
        _connectionStringBuilder["database"] = database.Text.Trim();
        try
        {
          var c = GetConnection(true);
          if (c == null || c.State != ConnectionState.Open)
            return false;
        }
        catch (MySqlException mysqlexception)
        {
          Logger.LogError(mysqlexception.Message, true);
          return false;
        }        
      }           
      return base.ProcessCmdKey(ref msg, keyData);
    }

     private DbConnection GetConnection(bool askToCreate)
     {
       DbConnection c = _factory.CreateConnection();
       c.ConnectionString = _connectionStringBuilder.ConnectionString;
       try
        {
           c.Open();         
        }
        catch (MySqlException mysqlException)
        {
          if (((System.Exception)(mysqlException).InnerException) != null)
          {
            var messageInnerException = ((System.Exception)(mysqlException).InnerException).Message;
            if (string.Compare(messageInnerException, string.Format("Unknown database '{0}'", database.Text), StringComparison.InvariantCultureIgnoreCase) == 0)
            {
              using (var yesNoDialog = Common.Utilities.GetYesNoInfoDialog(
                                         Utility.Forms.InfoDialog.InfoType.Info,
                                         false,
                                         "Create database",
                                         $"The database '{database.Text}' doesn't exist or you don't have permission to see it.",
                                         "Would you like to create it?"
              ))
              {
                if (askToCreate && yesNoDialog.ShowDialog() == DialogResult.Yes)
                {
                  try
                  {
                    if (AttemptToCreateDatabase(c.ConnectionString))
                    {
                      c.Open();
                    }
                  }
                  catch
                  {
                    throw;
                  }
                }
                else
                {
                  DialogResult = DialogResult.Cancel;
                  return null;
                } 
              }
            }
           else
            {
              throw;
            }
          }
          else
          {
            throw;
          }
        }

       return c;
     }

     private void btnRefresh_Click(object sender, EventArgs e)
     {
       GetDatabases();
     }
  }
}
