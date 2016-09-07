// Copyright © 2016, Oracle and/or its affiliates. All rights reserved.
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
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using MySql.Data.VisualStudio.DDEX;
using MySql.Data.VisualStudio.Properties;
using MySQL.Utility.Classes;
using MySQL.Utility.Classes.MySQL;
using MySQL.Utility.Forms;
using MySQL.Utility.Classes.MySQLWorkbench;
using Microsoft.VisualStudio.Data.Services;

namespace MySql.Data.VisualStudio.Editors
{
  /// <summary>
  /// A dialog containing parameters to create a new connection to MySQL Server instances.
  /// </summary>
  public partial class ConnectDialog : AutoStyleableBaseDialog
  {
    #region Constants

    /// <summary>
    /// Collapsed dialog height in pixels
    /// </summary>
    private const int COLLAPSED_HEIGHT = 400;

    /// <summary>
    /// The default hostname used for a MySQL Server instance.
    /// </summary>
    private const string DEFAULT_MYSQL_HOSTNAME = "localhost";

    /// <summary>
    /// The default port used for a MySQL Server instance.
    /// </summary>
    private const int DEFAULT_MYSQL_PORT = 3306;

    /// <summary>
    /// Expanded dialog height in pixels
    /// </summary>
    private const int EXPANDED_HEIGHT = 580;

    /// <summary>
    /// Connection value corresponding to the hostname when it is blank.
    /// </summary>
    private const string NO_HOSTNAME = "No hostname";

    /// <summary>
    /// Connection value corresponding to the schema when it is blank.
    /// </summary>
    private const string NO_SCHEMA = "no schema";

    #endregion Constants

    #region Fields

    /// <summary>
    /// The connection string builder associated to the connection parameters in this dialog.
    /// </summary>
    private MySqlConnectionStringBuilder _connectionStringBuilder;

    /// <summary>
    /// The factory that creates connections.
    /// </summary>
    private DbProviderFactory _factory;

    /// <summary>
    /// Table containing information about all schemas in the server the connection points to.
    /// </summary>
    private DataTable _schemasTable;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectDialog"/> class.
    /// </summary>
    public ConnectDialog()
    {
      _schemasTable = null;
      InitializeComponent();

      _factory = MySqlClientFactory.Instance;
      if (_factory == null)
      {
        throw new Exception(Resources.ConnectDialog_DataProviderRegisteredError);
      }

      _connectionStringBuilder = _factory.CreateConnectionStringBuilder() as MySqlConnectionStringBuilder;
      if (_connectionStringBuilder != null)
      {
        _connectionStringBuilder.Server = DEFAULT_MYSQL_HOSTNAME;
        _connectionStringBuilder.Port = DEFAULT_MYSQL_PORT;
      }

      AdvancedPropertyGrid.SelectedObject = _connectionStringBuilder;
      ExpandedState = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectDialog"/> class.
    /// </summary>
    /// <param name="connectionStringBuilder">Existing connection settings to pre-fill the dialog.</param>
    public ConnectDialog(DbConnectionStringBuilder connectionStringBuilder)
      : this()
    {
      if (connectionStringBuilder == null)
      {
        return;
      }

      ConnectionString = connectionStringBuilder.ConnectionString;
    }

    #region Properties

    /// <summary>
    /// Gets or sets a database connection based on the connection parameters in the dialog.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DbConnection Connection
    {
      get
      {
        return GetConnection(false, false);
      }

      set
      {
        if (value == null)
        {
          return;
        }

        ConnectionString = value.ConnectionString;
      }
    }

    /// <summary>
    /// Gets the name of the connection if added to the Server Explorer.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string ConnectionName
    {
      get
      {
        return ConnectionNameTextBox.Text.Trim();
      }
    }

    /// <summary>
    /// Gets or sets the connection string related to the connection parameters in this dialog.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string ConnectionString
    {
      get
      {
        RefreshConnectionStringBuilder();
        if (!string.IsNullOrEmpty(_connectionStringBuilder.Password))
        {
          _connectionStringBuilder.PersistSecurityInfo = true;
        }

        return _connectionStringBuilder.ConnectionString;
      }

      set
      {
        _connectionStringBuilder.ConnectionString = value;
        if (string.IsNullOrEmpty(_connectionStringBuilder.Password))
        {
          _connectionStringBuilder.PersistSecurityInfo = false;
        }

        RefreshConnectionParameterControls();
      }
    }

    /// <summary>
    /// Gets a value indicating whether the dialog is expanded or collapsed.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool ExpandedState { get; private set; }

    #endregion Properties

    /// <summary>
    /// Event delegate method fired when the <see cref="AddToServerExplorerCheckBox"/> checked status changes.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void AddToServerExplorerCheckBox_CheckedChanged(object sender, EventArgs e)
    {
      ConnectionNameTextBox.Enabled = AddToServerExplorerCheckBox.Checked;
      ConnectionNameTextBox.Text = AddToServerExplorerCheckBox.Checked
        ? GetProposedServerExplorerConnectionName(null)
        : null;
    }

    /// <summary>
    /// Event delegate method fired when the <see cref="RefreshButton"/> is clicked.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void AdvancedButton_Click(object sender, EventArgs e)
    {
      ExpandedState = !ExpandedState;
      SuspendLayout();
      if (ExpandedState)
      {
        AdvancedButton.Text = Resources.ConnectDialog_SimpleButtonText;
        Height = EXPANDED_HEIGHT;
        SimpleGroupBox.Visible = false;
        AdvancedPropertyGrid.Visible = true;
      }
      else
      {
        AdvancedButton.Text = Resources.ConnectDialog_AdvancedButtonText;
        Height = COLLAPSED_HEIGHT;
        SimpleGroupBox.Visible = true;
        AdvancedPropertyGrid.Visible = false;
        RefreshConnectionParameterControls();
      }

      ResumeLayout();
    }

    /// <summary>
    /// Event delegate method fired when the <see cref="ConnectDialog"/> is being closed.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void ConnectDialogNew_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (DialogResult == DialogResult.Cancel)
      {
        return;
      }

      if (GetConnection(true, true) == null)
      {
        e.Cancel = true;
        return;
      }

      if (!AddToServerExplorerCheckBox.Checked)
      {
        return;
      }

      // Check if the Server Explorer connection name is not empty, and if so prompt the user to use a suggested connection name
      var validateConnectionName = true;
      var serverExplorerConnectionName = ConnectionNameTextBox.Text.Trim();
      if (string.IsNullOrEmpty(serverExplorerConnectionName))
      {
        serverExplorerConnectionName = GetProposedServerExplorerConnectionName(null);
        var infoResult = InfoDialog.ShowDialog(InfoDialogProperties.GetYesNoDialogProperties(InfoDialog.InfoType.Warning,
          Resources.ConnectDialog_EmptyConnectionNameTitle,
          Resources.ConnectDialog_EmptyConnectionNameDetail,
          string.Format(Resources.ConnectDialog_EmptyConnectionNameSubDetail, serverExplorerConnectionName)));
        if (infoResult.DialogResult == DialogResult.No)
        {
          e.Cancel = true;
          return;
        }

        validateConnectionName = false;
      }

      // Validate if there is an existing connection in the Server Explorer with the same name, if so prompt the user for an action.
      IVsDataExplorerConnection existingConnection = null;
      if (validateConnectionName)
      {
        existingConnection = MySqlDataProviderPackage.Instance.GetServerExplorerConnection(serverExplorerConnectionName);
        if (existingConnection != null)
        {
          var newConnectionName = GetProposedServerExplorerConnectionName(serverExplorerConnectionName);
          var infoProperties = InfoDialogProperties.GetInfoDialogProperties(InfoDialog.InfoType.Warning,
            CommandAreaProperties.ButtonsLayoutType.Generic3Buttons,
            Resources.ConnectDialog_ConnectionAlreadyExistsTitle,
            string.Format(Resources.ConnectDialog_ConnectionAlreadyExistsDetail, serverExplorerConnectionName),
            string.Format(Resources.ConnectDialog_ConnectionAlreadyExistsSubDetail, newConnectionName),
            string.Format(Resources.ConnectDialog_ConnectionAlreadyExistsMoreInfo, newConnectionName, serverExplorerConnectionName));
          infoProperties.FitTextStrategy = InfoDialog.FitTextsAction.IncreaseDialogWidth;
          infoProperties.WordWrapMoreInfo = false;
          infoProperties.CommandAreaProperties.Button1Text = Resources.ConnectDialog_Rename;
          infoProperties.CommandAreaProperties.Button1DialogResult = DialogResult.Yes;
          infoProperties.CommandAreaProperties.Button2Text = Resources.ConnectDialog_Replace;
          infoProperties.CommandAreaProperties.Button1DialogResult = DialogResult.No;
          infoProperties.CommandAreaProperties.Button3Text = Resources.ConnectDialog_Cancel;
          infoProperties.CommandAreaProperties.Button3DialogResult = DialogResult.Cancel;
          var infoResult = InfoDialog.ShowDialog(infoProperties);
          switch (infoResult.DialogResult)
          {
            case DialogResult.Yes:
              // Set values needed to Rename the connection.
              serverExplorerConnectionName = newConnectionName;
              existingConnection = null;
              break;

            case DialogResult.No:
              // Do nothing, the serverExplorerConnectionName and existingConnection variables have the values needed to Replace.
              break;

            case DialogResult.Cancel:
              // Abort the closing of the dialog.
              e.Cancel = true;
              return;
          }
        }
      }

      // Create the Server Explorer connection
      MySqlDataProviderPackage.Instance.AddServerExplorerConnection(serverExplorerConnectionName, ConnectionString, existingConnection);
    }

    /// <summary>
    /// Attempts to create a new schema using a connection related to the parameters in the dialog.
    /// </summary>
    /// <param name="schemaName">The name of the new schema.</param>
    /// <returns><c>true</c> if the schema is created successfully, <c>false</c> otherwise.</returns>
    private bool CreateSchema(string schemaName)
    {
      bool success = true;
      try
      {
        using (var conn = new MySqlConnectionSupport())
        {
          var connectionStringBuilder = new MySqlConnectionStringBuilder(ConnectionString)
          {
            Database = string.Empty
          };
          conn.Initialize(null);
          conn.ConnectionString = connectionStringBuilder.ConnectionString;
          conn.Open(false);
          conn.ExecuteWithoutResults(string.Format("CREATE DATABASE `{0}`", schemaName), 1, null, 0);
        }
      }
      catch (Exception ex)
      {
        success = false;
        MySqlSourceTrace.WriteAppErrorToLog(ex, Resources.ErrorTitle, string.Format(Resources.ErrorAttemptingToCreateDB, schemaName), true);
      }

      return success;
    }

    /// <summary>
    /// Returns a <see cref="DbConnection"/> created from the connection parameters in this dialog.
    /// </summary>
    /// <param name="askToCreateSchemaIfNotExists">Flag indicating whether a prompt is shown to ask for the schema creation in case the specified schema does not exist.</param>
    /// <param name="testOnly">Flag indicating whether the method only tests if a connection can be created and disposes of the connection after the test is done.</param>
    /// <returns>A <see cref="DbConnection"/> if <see cref="testOnly"/> is <c>false</c>.</returns>
    private DbConnection GetConnection(bool askToCreateSchemaIfNotExists, bool testOnly)
    {
      var newConnection = _factory.CreateConnection();
      if (newConnection == null)
      {
        return null;
      }

      newConnection.ConnectionString = ConnectionString;
      try
      {
        newConnection.Open();
      }
      catch (MySqlException mysqlException)
      {
        string schema = _connectionStringBuilder.Database;
        bool showError = true;
        newConnection = null;
        if (mysqlException.InnerException != null
            && string.Compare(mysqlException.InnerException.Message, string.Format("Unknown database '{0}'", schema), StringComparison.InvariantCultureIgnoreCase) == 0
            && askToCreateSchemaIfNotExists)
        {
          var infoResult = InfoDialog.ShowDialog(InfoDialogProperties.GetYesNoDialogProperties(InfoDialog.InfoType.Warning,
                                                  Resources.ConnectDialog_CreateSchemaTitle,
                                                  string.Format(Resources.ConnectDialog_CreateSchemaDetail, schema),
                                                  Resources.ConnectDialog_CreateSchemaSubDetail));
          if (infoResult.DialogResult == DialogResult.Yes && CreateSchema(schema))
          {
            newConnection = GetConnection(false, testOnly);
            showError = false;
          }
        }

        if (showError)
        {
          MySqlSourceTrace.WriteAppErrorToLog(mysqlException, Resources.ErrorTitle, Resources.ConnectDialog_GetConnectionError, true);
        }
      }
      finally
      {
        if (testOnly && newConnection != null)
        {
          if (newConnection.State == ConnectionState.Open)
          {
            newConnection.Close();
          }

          newConnection.Dispose();
        }
      }

      return newConnection;
    }

    /// <summary>
    /// Returns a suggested connection name for a Server Explorer connection ensuring there is no connection with the proposed name.
    /// </summary>
    /// <param name="proposedName">The proposed connection name. If <c>null</c> a default name like HOSTNAME(SCHEMA) is used.</param>
    /// <returns>A suggested connection name for a Server Explorer connection.</returns>
    private string GetProposedServerExplorerConnectionName(string proposedName)
    {
      if (string.IsNullOrEmpty(proposedName))
      {
        var hostname = _connectionStringBuilder.Server;
        if (string.IsNullOrEmpty(hostname))
        {
          hostname = NO_HOSTNAME;
        }

        var schema = _connectionStringBuilder.Database;
        if (string.IsNullOrEmpty(schema))
        {
          schema = NO_SCHEMA;
        }

        proposedName = string.Format("{0}({1})", hostname, schema);
      }

      string newSeConnectionName = proposedName;
      IVsDataExplorerConnection existingConnection;
      int copyIndex = 2;
      do
      {
        existingConnection = MySqlDataProviderPackage.Instance.GetServerExplorerConnection(newSeConnectionName);
        newSeConnectionName = string.Format("{0} ({1})", proposedName, copyIndex++);
        if (existingConnection != null)
        {
          proposedName = newSeConnectionName;
        }
      }
      while (existingConnection != null);

      return proposedName;
    }

    /// <summary>
    /// Fills the <see cref="SchemaComboBox"/> with a list of schemas in the connected MySQL Server.
    /// </summary>
    private void InitializeSchemasComboBox()
    {
      Cursor = Cursors.WaitCursor;
      SchemaComboBox.Items.Clear();
      try
      {
        using (var connection = _factory.CreateConnection())
        {
          var mySqlConnection = connection as MySqlConnection;
          if (mySqlConnection == null)
          {
            return;
          }

          mySqlConnection.ConnectionString = ConnectionString;
          mySqlConnection.Open();
          _schemasTable = mySqlConnection.GetSchema("Databases");
          if (_schemasTable == null || _schemasTable.Rows.Count == 0)
          {
            return;
          }

          // Remove rows containing system schemas
          foreach (var sysSchemaName in MySqlWorkbenchConnection.SystemSchemaNames)
          {
            var result = _schemasTable.Select(string.Format("DATABASE_NAME = '{0}'", sysSchemaName));
            if (result.Length == 0)
            {
              continue;
            }

            foreach (var row in result)
            {
              row.Delete();
            }
          }

          _schemasTable.AcceptChanges();
          SchemaComboBox.DisplayMember = "DATABASE_NAME";
          SchemaComboBox.ValueMember = "DATABASE_NAME";
          SchemaComboBox.DataSource = _schemasTable;
        }
      }
      catch (Exception ex)
      {
        MySqlSourceTrace.WriteAppErrorToLog(ex, Resources.ErrorTitle, Resources.ConnectDialog_SchemasFetchError, true);
      }
      finally
      {
        Cursor = Cursors.Default;
      }
    }

    /// <summary>
    /// Event delegate method fired when the <see cref="PortTextBox"/> is being validated.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void PortTextBox_Validating(object sender, CancelEventArgs e)
    {
      uint port;
      if (!uint.TryParse(PortTextBox.Text, out port))
      {
        e.Cancel = true;
      }
    }

    /// <summary>
    /// Event delegate method fired when the <see cref="RefreshButton"/> is clicked.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void RefreshButton_Click(object sender, EventArgs e)
    {
      InitializeSchemasComboBox();
    }

    /// <summary>
    /// Refreshes the values of the dialog controls based on the connection string builder.
    /// </summary>
    private void RefreshConnectionParameterControls()
    {
      HostnameTextBox.Text = _connectionStringBuilder["server"].ToString();
      if (_connectionStringBuilder["port"] != null)
      {
        int port;
        int.TryParse(_connectionStringBuilder["port"].ToString(), out port);
        PortTextBox.Text = port.ToString();
      }
      else
      {
        PortTextBox.Text = string.Empty;
      }

      UsernameTextBox.Text = _connectionStringBuilder["userid"].ToString();
      PasswordTextBox.Text = _connectionStringBuilder["password"].ToString();
      SchemaComboBox.Text = _connectionStringBuilder["database"].ToString();
    }

    /// <summary>
    /// Refreshes the values of the connection string builder based on the dialog controls.
    /// </summary>
    private void RefreshConnectionStringBuilder()
    {
      _connectionStringBuilder["server"] = HostnameTextBox.Text.Trim();
      _connectionStringBuilder["port"] = int.Parse(PortTextBox.Text);
      _connectionStringBuilder["userid"] = UsernameTextBox.Text.Trim();
      _connectionStringBuilder["password"] = PasswordTextBox.Text.Trim();
      _connectionStringBuilder["database"] = SchemaComboBox.Text.Trim();
    }

    /// <summary>
    /// Event delegate method fired when the <see cref="SchemaComboBox"/> drop-down is being opened.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void SchemaComboBox_DropDown(object sender, EventArgs e)
    {
      if (SchemaComboBox.Items.Count > 0)
      {
        return;
      }

      InitializeSchemasComboBox();
    }

    /// <summary>
    /// Event delegate method fired when a control's validation has ended.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void SimpleControlValidated(object sender, EventArgs e)
    {
      if (sender.Equals(PortTextBox))
      {
        _connectionStringBuilder.Port = uint.Parse(PortTextBox.Text);
        return;
      }

      if (sender.Equals(UsernameTextBox))
      {
        _connectionStringBuilder.UserID = UsernameTextBox.Text.Trim();
        return;
      }

      if (sender.Equals(PasswordTextBox))
      {
        _connectionStringBuilder.Password = PasswordTextBox.Text.Trim();
        return;
      }

      var hostName = HostnameTextBox.Text.Trim();
      var schemaName = SchemaComboBox.Text.Trim();
      if (sender.Equals(HostnameTextBox))
      {
        _connectionStringBuilder.Server = hostName;
      }
      else if (sender.Equals(SchemaComboBox))
      {
        _connectionStringBuilder.Database = schemaName;
      }
    }
  }
}
