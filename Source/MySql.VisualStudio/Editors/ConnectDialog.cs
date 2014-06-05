﻿// Copyright © 2008, 2014, Oracle and/or its affiliates. All rights reserved.
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
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.Common;
using MySql.Data.MySqlClient;
using MySql.Data.VisualStudio.Properties;

namespace MySql.Data.VisualStudio
{
  public partial class ConnectDialog : Form
  {
    private int bigSize = 522;
    private int smallSize = 340;
    private DbProviderFactory factory;
    private DbConnectionStringBuilder connectionStringBuilder;
    private bool populated = false;

    public ConnectDialog()
    {
      InitializeComponent();
      factory = MySqlClientFactory.Instance;
      if (factory == null)
        throw new Exception("MySql Data Provider is not correctly registered");
      connectionStringBuilder = factory.CreateConnectionStringBuilder();
      connectionProperties.SelectedObject = connectionStringBuilder;
      btnRefresh.Click += btnRefresh_Click;
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
        ReadFields();
        connectionStringBuilder["Port"] = settings.Port;
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
          connectionStringBuilder.ConnectionString = value.ConnectionString;
          Rebind();
        }
      }
    }

    private void advancedButton_Click(object sender, EventArgs e)
    {
      this.SuspendLayout();
      if (this.Size.Height > 400)
      {
        advancedButton.Text = "Advanced >>";
        Height = smallSize;
        simplePanel.Visible = true;
        connectionProperties.Visible = false;
        Rebind();
      }
      else
      {
        advancedButton.Text = "Simple <<";
        Height = bigSize;
        simplePanel.Visible = false;
        connectionProperties.Visible = true;
      }
      this.ResumeLayout();
    }

    private void Rebind()
    {
      serverName.Text = connectionStringBuilder["server"] as string;
      userId.Text = connectionStringBuilder["userid"] as string;
      password.Text = connectionStringBuilder["password"] as string;
      database.Text = connectionStringBuilder["database"] as string;
    }

    private void database_DropDown(object sender, EventArgs e)
    {
      if (populated) return;
      database.Items.Add("Loading databases...");
      GetDatabases();
    }

    private void serverName_Leave(object sender, EventArgs e)
    {
      connectionStringBuilder["server"] = serverName.Text.Trim();
    }

    private void userId_Leave(object sender, EventArgs e)
    {
      connectionStringBuilder["userid"] = userId.Text.Trim();         
    }

    private void password_Leave(object sender, EventArgs e)
    {
      connectionStringBuilder["password"] = password.Text.Trim();      
    }

    private void database_Leave(object sender, EventArgs e)
    {
      connectionStringBuilder["database"] = database.Text.Trim();
    }

    private void GetDatabases()
    {
      try
      {
        ReadFields();
        
        using (DbConnection c = factory.CreateConnection())
        {
          c.ConnectionString = connectionStringBuilder.ConnectionString;
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
        populated = true;
      }
      catch (MySqlException ex)
      {
        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK);
      }      
    }

    private void connectButton_Click(object sender, EventArgs e)
    {
      // Ensure all data is populated into the connection string builder
      connectionStringBuilder["server"] = serverName.Text.Trim() == String.Empty ? "localhost" : serverName.Text.Trim();
      connectionStringBuilder["userid"] = userId.Text.Trim() == String.Empty ? "root" : userId.Text.Trim();
      connectionStringBuilder["database"] = database.Text.Trim() == string.Empty ? "test" : database.Text.Trim();
      password_Leave(serverName, EventArgs.Empty);
      
      if (!populated)
        GetConnection(true);
    }

    private void ReadFields()
    {
      connectionStringBuilder["server"] = serverName.Text.Trim();
      connectionStringBuilder["userid"] = userId.Text.Trim();
      connectionStringBuilder["password"] = password.Text.Trim();
      connectionStringBuilder["database"] = database.Text.Trim();
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
        MessageBox.Show(String.Format(Resources.ErrorAttemptingToCreateDB, database.Text));
        return false;
      }      
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if ((this.ActiveControl == database) && (keyData == Keys.Return))
      {
        connectionStringBuilder["database"] = database.Text.Trim();
        var c = GetConnection(true);
        if (c == null || c.State != ConnectionState.Open)
          return false;
      }           
      return base.ProcessCmdKey(ref msg, keyData);
      //    return base.ProcessCmdKey(ref msg, keyData);
      //  else
      //    return true;
      //}
      //else
      //{
        
      //}
    }

     private DbConnection GetConnection(bool askToCreate)
     {
       DbConnection c = factory.CreateConnection();
       c.ConnectionString = connectionStringBuilder.ConnectionString;
       try
        {
           c.Open();         
        }
        catch (MySqlException mysqlException)
        {
          if (((System.Exception)(mysqlException).InnerException) != null)
          {
            var messageInnerException = ((System.Exception)(mysqlException).InnerException).Message;
            if (String.Compare(messageInnerException, String.Format("Unknown database '{0}'", database.Text), StringComparison.InvariantCultureIgnoreCase) == 0)
            {
              if (askToCreate && MessageBox.Show(String.Format("The database '{0}' doesn't exist or you don't have permission to see it." + "\n\r" + "Would you like to create it?", database.Text), "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
              {
                try
                {
                  if (AttemptToCreateDatabase(c.ConnectionString))
                    c.Open();
                }
                catch
                {
                  throw;
                }
              }
              else
              {
                DialogResult = System.Windows.Forms.DialogResult.Cancel;
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
