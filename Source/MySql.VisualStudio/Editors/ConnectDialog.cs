// Copyright © 2008, 2016, Oracle and/or its affiliates. All rights reserved.
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
using System.Data;
using System.Data.Common;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using MySql.Data.VisualStudio.DDEX;
using MySql.Data.VisualStudio.Properties;
using MySqlConnectionStringBuilder = MySQL.Utility.Classes.MySQL.MySqlConnectionStringBuilder;

namespace MySql.Data.VisualStudio.Editors
{
  public partial class ConnectDialog : Form
  {
    private const int BIG_SIZE = 522;
    private const int SMALL_SIZE = 340;

    private DbProviderFactory _factory;
    private DbConnectionStringBuilder _connectionStringBuilder;
    private bool _populated;

    public ConnectDialog()
    {
      InitializeComponent();
      _factory = MySqlClientFactory.Instance;
      if (_factory == null)
        throw new Exception("MySql Data Provider is not correctly registered");
      _connectionStringBuilder = _factory.CreateConnectionStringBuilder();
      connectionProperties.SelectedObject = _connectionStringBuilder;
      btnRefresh.Click += btnRefresh_Click;
      txtPort.Leave += txtPort_Leave;
    }

    public ConnectDialog(MySqlConnectionStringBuilder settings)
      : this()
    {
      if (settings == null)
      {
        return;
      }

      serverName.Text = settings.Server;
      userId.Text = settings.UserID;
      password.Text = settings.Password;
      database.Text = settings.Database;
      _connectionStringBuilder["Port"] = txtPort.Text = settings.Port.ToString();        
      ReadFields();
    }
    
    public DbConnection Connection
    {
      get
      {
        return GetConnection(false);       
      }
      set
      {
        if (value == null)
        {
          return;
        }

        _connectionStringBuilder.ConnectionString = value.ConnectionString;
        Rebind();
      }
    }

    private void advancedButton_Click(object sender, EventArgs e)
    {
      SuspendLayout();
      if (Size.Height > 400)
      {
        advancedButton.Text = Resources.ConnectDialog_AdvancedButtonText;
        Height = SMALL_SIZE;
        simplePanel.Visible = true;
        connectionProperties.Visible = false;
        Rebind();
      }
      else
      {
        advancedButton.Text = Resources.ConnectDialog_SimpleButtonText;
        Height = BIG_SIZE;
        simplePanel.Visible = false;
        connectionProperties.Visible = true;
      }

      ResumeLayout();
    }

    private void Rebind()
    {
      serverName.Text = _connectionStringBuilder["server"] as string;
      userId.Text = _connectionStringBuilder["userid"] as string;
      password.Text = _connectionStringBuilder["password"] as string;
      database.Text = _connectionStringBuilder["database"] as string;
      
      int port;
      if (_connectionStringBuilder["port"] != null)
         int.TryParse(_connectionStringBuilder["port"].ToString(), out port);
      else
        port = 3306;
      txtPort.Text = port.ToString();
 
    }

    private void database_DropDown(object sender, EventArgs e)
    {
      if (_populated) return;
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
          if (c != null)
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
        }
        _populated = true;
      }
      catch (MySqlException ex)
      {
        MessageBox.Show(ex.Message, Resources.ErrorTitle, MessageBoxButtons.OK);
      }      
    }

    private void connectButton_Click(object sender, EventArgs e)
    {
      int port;
      // Ensure all data is populated into the connection string builder
      _connectionStringBuilder["server"] = serverName.Text.Trim() == String.Empty ? "localhost" : serverName.Text.Trim();
      _connectionStringBuilder["userid"] = userId.Text.Trim() == String.Empty ? "root" : userId.Text.Trim();
      _connectionStringBuilder["database"] = database.Text.Trim() == string.Empty ? "test" : database.Text.Trim();
      _connectionStringBuilder["port"] = !int.TryParse(txtPort.Text, out port) ? 3306 : port;
      password_Leave(serverName, EventArgs.Empty);
      
      if (!_populated)
       GetConnection(true);
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
        MessageBox.Show(string.Format(Resources.ErrorAttemptingToCreateDB, database.Text));
        return false;
      }      
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if ((ActiveControl == database) && (keyData == Keys.Return))
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
          MessageBox.Show(mysqlexception.Message, Resources.ErrorTitle, MessageBoxButtons.OK);          
          return false;
        }        
      }

      return base.ProcessCmdKey(ref msg, keyData);  
    }

     private DbConnection GetConnection(bool askToCreate)
     {
       DbConnection c = _factory.CreateConnection();
       if (c == null)
       {
         return null;
       }

       c.ConnectionString = _connectionStringBuilder.ConnectionString;
       try
        {
           c.Open();         
        }
        catch (MySqlException mysqlException)
        {
          if (mysqlException.InnerException != null)
          {
            var messageInnerException = mysqlException.InnerException.Message;
            if (string.Compare(messageInnerException, string.Format("Unknown database '{0}'", database.Text), StringComparison.InvariantCultureIgnoreCase) == 0)
            {
              if (askToCreate && MessageBox.Show(string.Format("The database '{0}' doesn't exist or you don't have permission to see it." + "\n\r" + "Would you like to create it?", database.Text), Resources.MySqlDataProviderPackage_Information, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
              {
                if (AttemptToCreateDatabase(c.ConnectionString))
                {
                  c.Open();
                }
              }
              else
              {
                DialogResult = DialogResult.Cancel;
                return null;
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
