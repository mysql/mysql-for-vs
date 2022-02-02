// Copyright (c) 2012, 2019, Oracle and/or its affiliates. All rights reserved.
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License as
// published by the Free Software Foundation; version 2 of the
// License.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301  USA

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Windows.Forms;
using MySql.Utility.Classes;
using MySql.Utility.Classes.MySql;
using MySql.Utility.Classes.MySqlWorkbench;
using MySql.Utility.Enums;

namespace MySql.Utility.Forms
{
  /// <summary>
  /// Dialog where a <see cref="MySqlWorkbenchConnection"/> object is updated or created.
  /// </summary>
  public partial class MySqlWorkbenchConnectionDialog : ValidatingBaseDialog
  {
    #region Constants

    /// <summary>
    /// Link for documentation page about creating SSL configuration files using the MySQL Server.
    /// </summary>
    public const string CREATING_SSL_FILES_WITH_MYSQL_LINK = "https://dev.mysql.com/doc/refman/5.7/en/creating-ssl-rsa-files-using-mysql.html";

    /// <summary>
    /// Link for documentation page about SSL configuration for Connector/NET.
    /// </summary>
    public const string SSL_TUTORIAL_FOR_CONNECTOR_NET_LINK = "https://dev.mysql.com/doc/connector-net/en/connector-net-tutorials-ssl.html";

    #endregion Constants

    #region Fields

    /// <summary>
    /// The connection method being used to create a connection.
    /// </summary>
    private MySqlWorkbenchConnection.ConnectionMethodType _connectionMethodType;

    /// <summary>
    /// Flag indicating whether the default schema field must contain a value before the connection is saved to disk.
    /// </summary>
    private bool _enforceSchema;

    /// <summary>
    /// The original Workbench connection when the dialog is used to edit an existing connection.
    /// </summary>
    private MySqlWorkbenchConnection _existingConnection;

    /// <summary>
    /// List of <see cref="TabPage"/> controls that are not shown to the user.
    /// </summary>
    private List<TabPage> _hiddenTabPages = new List<TabPage>();

    /// <summary>
    /// The name of the default schema the form is initialized with.
    /// </summary>
    private string _initialSchema;

    /// <summary>
    /// Table containing information about all schemas in the server the connection points to.
    /// </summary>
    private DataTable _schemasTable;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlWorkbenchConnectionDialog"/> class.
    /// </summary>
    /// <param name="existingConnection">An existing Workbench connection to edit, if null a new one is created.</param>
    /// <param name="enforceSchema">Flag indicating whether the default schema field must contain a value before the connection is saved to disk.</param>
    public MySqlWorkbenchConnectionDialog(MySqlWorkbenchConnection existingConnection, bool enforceSchema)
    {
      string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
      if (string.IsNullOrEmpty(MySqlWorkbench.ExternalApplicationConnectionsFilePath))
      {
        MySqlWorkbench.ExternalApplicationConnectionsFilePath = appDataFolder + @"\MySQL\connections.xml";
      }

      if (string.IsNullOrEmpty(MySqlWorkbenchPasswordVault.ApplicationPasswordVaultFilePath))
      {
        MySqlWorkbenchPasswordVault.ApplicationPasswordVaultFilePath = appDataFolder + @"\MySQL\user_data.dat";
      }

      IsUserInput = false;
      _schemasTable = null;
      _enforceSchema = enforceSchema;
      _existingConnection = existingConnection;
      _initialSchema = existingConnection != null ? existingConnection.DefaultSchema : null;
      ConnectionResultType = ConnectionResultType.None;
      WorkbenchConnection = existingConnection == null ? new MySqlWorkbenchConnection() : _existingConnection.Clone(MySqlWorkbenchConnection.SavedStatusType.InDisk, true);
      InitializeComponent();
      ParametersTabControl.SetDoubleBuffered();
      HelpDefaultSchemaLabel.Text = _enforceSchema
        ? Resources.DefaultSchemaRequiredHelpText
        : Resources.DefaultSchemaNotRequiredHelpText;
      _connectionMethodType = MySqlWorkbenchConnection.ConnectionMethodType.Tcp;
      var useSslType = WorkbenchConnection.UseSsl;
      UseSslComboBox.InitializeComboBoxFromEnum(MySqlWorkbenchConnection.UseSslType.No);
      UseSslComboBox.SelectedIndex = (int)useSslType;
      ConnectionMethodComboBox.InitializeComboBoxFromEnum(MySqlWorkbenchConnection.ConnectionMethodType.Unknown, true, true, true);
      SaveSuccessful = false;
      WorkbenchConnection.PropertyChanged += WorkbenchConnection_PropertyChanged;
      if (_existingConnection != null && _existingConnection.ConnectionStatus != MySqlWorkbenchConnection.ConnectionStatusType.Unknown)
      {
        // Set the connection status with the one in the existing connection
        WorkbenchConnection_PropertyChanged(_existingConnection, new PropertyChangedEventArgs("ConnectionStatus"));
        switch (_existingConnection.ConnectionStatus)
        {
          case MySqlWorkbenchConnection.ConnectionStatusType.Unknown:
            ConnectionResultType = ConnectionResultType.None;
            break;

          case MySqlWorkbenchConnection.ConnectionStatusType.AcceptingConnections:
            ConnectionResultType = ConnectionResultType.ConnectionSuccess;
            break;

          case MySqlWorkbenchConnection.ConnectionStatusType.RefusingConnections:
            ConnectionResultType = ConnectionResultType.ConnectionError;
            break;
        }
      }

      ConnectionBindingSource.DataSource = WorkbenchConnection;
    }

    #region Enumerations

    /// <summary>
    /// Specifies values for the type of files needed by connection parameters.
    /// </summary>
    public enum ParameterFileType
    {
      /// <summary>
      /// SSH Private Key file (.ppk)
      /// </summary>
      SshPrivateKey,

      /// <summary>
      /// SSL PEM Certification Authority file (.pem)
      /// </summary>
      SslPemCertificationAuthority,

      /// <summary>
      /// SSL PEM Client Certificate file (.pem)
      /// </summary>
      SslPemClientCertificate,

      /// <summary>
      /// SSL PEM Key file (.pem)
      /// </summary>
      SslPemKey
    }

    #endregion Enumerations

    #region Properties

    /// <summary>
    /// Gets a value indicating whether the connection was saved successfully after the OK button is clicked.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool SaveSuccessful { get; private set; }

    /// <summary>
    /// Gets a value indicating the result of testing the current connection.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ConnectionResultType ConnectionResultType { get; private set; }

    /// <summary>
    /// Gets the Workbench connection being edited or created.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public MySqlWorkbenchConnection WorkbenchConnection { get; }

    #endregion Properties

    /// <summary>
    /// Verifies if MysQL Workbench is running to see if connections can be added or deleted, and shows the <see cref="MySqlWorkbenchConnectionDialog"/> dialog.
    /// </summary>
    /// <returns>Dialog result.</returns>
    public DialogResult ShowIfWorkbenchNotRunning()
    {
      var dr = DialogResult.None;
      if (ValidateAddingOrEditingConnections())
      {
        dr = ShowDialog();
      }

      return dr;
    }

    /// <summary>
    /// Handles the TextChanged event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected override void TextChangedHandler(object sender, EventArgs e)
    {
      // Looks like we could get rid of this empty override, but it is necessary to avoid an error of:
      // The method 'xxx' cannot be the method for an event because a class this class derives from already defines the method
      base.TextChangedHandler(sender, e);
    }

    /// <summary>
    /// Handles the TextValidated event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    /// <remarks>This event method is meant to be used with the <see cref="Control.Validated"/> event.</remarks>
    protected override void ValidatedHandler(object sender, EventArgs e)
    {
      // Looks like we could get rid of this empty override, but it is necessary to avoid an error of:
      // The method 'xxx' cannot be the method for an event because a class this class derives from already defines the method
      base.ValidatedHandler(sender, e);
    }

    /// <summary>
    /// Contains calls to methods that validate the given control's value.
    /// </summary>
    /// <returns>An error message or <c>null</c> / <see cref="string.Empty"/> if everything is valid.</returns>
    protected override string ValidateFields()
    {
      if (ErrorProviderControl == null)
      {
        return null;
      }

      string errorMessage = null;
      switch (ErrorProviderControl.Name)
      {
        case nameof(ConnectionNameTextBox):
          errorMessage = ValidateConnectionName();
          break;

        case nameof(HostNameTextBox):
          errorMessage = _connectionMethodType == MySqlWorkbenchConnection.ConnectionMethodType.Tcp
            ? MySqlServerInstance.ValidateHostNameOrIpAddress(HostNameTextBox.Text)
            : null;
          break;

        case nameof(SocketTextBox):
          errorMessage = _connectionMethodType == MySqlWorkbenchConnection.ConnectionMethodType.LocalUnixSocketOrWindowsPipe
            ? MySqlServerInstance.ValidatePipeOrSharedMemoryName(true, SocketTextBox.Text)
            : null;
          break;

        case nameof(UsernameTextBox):
          errorMessage = (_connectionMethodType == MySqlWorkbenchConnection.ConnectionMethodType.Tcp
                          || _connectionMethodType == MySqlWorkbenchConnection.ConnectionMethodType.LocalUnixSocketOrWindowsPipe)
                         && string.IsNullOrEmpty(UsernameTextBox.Text)
            ? Resources.MySqlUserNameRequiredError
            : null;
          break;

        case nameof(SshUsernameTextBox):
        case nameof(MySqlUsernameTextBox):
          errorMessage = _connectionMethodType == MySqlWorkbenchConnection.ConnectionMethodType.Ssh
                         && string.IsNullOrEmpty(ErrorProviderControl.Text)
            ? Resources.MySqlUserNameRequiredError
            : null;
          break;

        case nameof(PortTextBox):
          errorMessage = _connectionMethodType == MySqlWorkbenchConnection.ConnectionMethodType.Tcp
            ? MySqlServerInstance.ValidatePortNumber(PortTextBox.Text, false)
            : null;
          break;

        case nameof(MySqlPortTextBox):
          errorMessage = _connectionMethodType == MySqlWorkbenchConnection.ConnectionMethodType.Ssh
            ? MySqlServerInstance.ValidatePortNumber(ErrorProviderControl.Text, false)
            :null;
          break;

        case nameof(SshHostnameTextBox):
          if (_connectionMethodType != MySqlWorkbenchConnection.ConnectionMethodType.Ssh)
          {
            break;
          }

          var lastColonIndex = SshHostnameTextBox.Text.LastIndexOf(':');
          var specifiesPort = lastColonIndex >= 0;
          var hostName = specifiesPort ? SshHostnameTextBox.Text.Substring(0, lastColonIndex) : SshHostnameTextBox.Text;
          var port = specifiesPort ? SshHostnameTextBox.Text.Substring(lastColonIndex + 1) : null;
          errorMessage = MySqlServerInstance.ValidateHostNameOrIpAddress(hostName);
          if (string.IsNullOrEmpty(errorMessage)
              && !string.IsNullOrEmpty(port))
          {
            errorMessage = MySqlServerInstance.ValidatePortNumber(port, false, null, false);
          }
          break;

        case nameof(DefaultSchemaComboBox):
          errorMessage = _connectionMethodType == MySqlWorkbenchConnection.ConnectionMethodType.Tcp
                         || _connectionMethodType == MySqlWorkbenchConnection.ConnectionMethodType.LocalUnixSocketOrWindowsPipe
            ? ValidateSchema(DefaultSchemaComboBox.Text, false)
            : null;
          break;

        case nameof(MySqlDefaultSchemaComboBox):
          errorMessage = _connectionMethodType == MySqlWorkbenchConnection.ConnectionMethodType.Ssh
            ? ValidateSchema(MySqlDefaultSchemaComboBox.Text, false)
            : null;
          break;

        case nameof(SshKeyFileTextBox):
          if (_connectionMethodType != MySqlWorkbenchConnection.ConnectionMethodType.Ssh)
          {
            break;
          }

          errorMessage = ValidateFilePath(SshKeyFileTextBox.Text, false);
          var enablePassPhrase = string.IsNullOrEmpty(errorMessage)
                                 && !string.IsNullOrEmpty(SshKeyFileTextBox.Text);
          SshPassPhraseTextBox.ReadOnly = !enablePassPhrase;
          if (!enablePassPhrase)
          {
            SshPassPhraseTextBox.Text = string.Empty;
          }

          SshPassPhraseTextBox.Validate();
          ErrorProviderControl = SshKeyFileButton;
          break;

        case nameof(SslKeyFileTextBox):
          errorMessage = ErrorProviderControlEnabled ? ValidateFilePath(SslKeyFileTextBox.Text, true) : null;
          ErrorProviderControl = SslKeyFileButton;
          break;

        case nameof(SslCertFileTextBox):
          errorMessage = ErrorProviderControlEnabled ? ValidateFilePath(SslCertFileTextBox.Text, true) : null;
          ErrorProviderControl = SslCertFileButton;
          break;

        case nameof(SslCaFileTextBox):
          errorMessage = ErrorProviderControlEnabled ? ValidateFilePath(SslCaFileTextBox.Text, true) : null;
          ErrorProviderControl = SslCaFileButton;
          break;

        case nameof(SshPassPhraseTextBox):
          errorMessage = _connectionMethodType == MySqlWorkbenchConnection.ConnectionMethodType.Ssh
                         && ErrorProviderControlEnabled
            ? ValidateSshPassPhrase()
            : null;
          break;
      }

      return errorMessage;
    }

    /// <summary>
    /// Event delegate method fired when the <see cref="ConnectionMethodComboBox"/> selected item's index changes.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void ConnectionMethodComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (ConnectionMethodComboBox.SelectedValue == null)
      {
        return;
      }

      ParametersTabControl.SuspendLayout();
      _connectionMethodType = (MySqlWorkbenchConnection.ConnectionMethodType)ConnectionMethodComboBox.SelectedValue;
      SetParametersPageVisibility();
      SetSshParametersPageVisibility();
      InitializeSslParametersPage();
      SetAdvancedPageVisibility();
      ParametersTabControl.ResumeLayout();
      ParametersTabControl.SelectedTab = ParametersTabControl.TabPages[0];
      FireAllValidationsAsync();
    }

    /// <summary>
    /// Event delegate method fired after the <see cref="DefaultSchemaComboBox"/> drop-down list is displayed.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void DefaultSchemaComboBox_DropDown(object sender, EventArgs e)
    {
      InitializeDefaultSchemaComboBox();
    }

    /// <summary>
    /// Initializes the <see cref="ConnectionMethodComboBox"/> values.
    /// </summary>
    private void InitializeDefaultSchemaComboBox()
    {
      TestConnection(true);
      if (ConnectionResultType != ConnectionResultType.ConnectionSuccess)
      {
        DefaultSchemaComboBox.DataSource = null;
        DefaultSchemaComboBox.Items.Clear();
        
        // A connection could not be established, so there is no way to retrieve the schemas.
        return;
      }

      _schemasTable = WorkbenchConnection.GetSchemaInformation(SchemaInformationType.Databases, true);
      if (_schemasTable == null || _schemasTable.Rows.Count == 0)
      {
        return;
      }

      // Remove rows containing system schemas
      foreach (var sysSchemaName in MySqlWorkbenchConnection.SystemSchemaNames)
      {
        var result = _schemasTable.Select($"DATABASE_NAME = '{sysSchemaName}'");
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

      if (_connectionMethodType == MySqlWorkbenchConnection.ConnectionMethodType.Ssh)
      {
        MySqlDefaultSchemaComboBox.DisplayMember = "DATABASE_NAME";
        MySqlDefaultSchemaComboBox.ValueMember = "DATABASE_NAME";
        MySqlDefaultSchemaComboBox.DataSource = _schemasTable;
      }
      else
      {
        DefaultSchemaComboBox.DisplayMember = "DATABASE_NAME";
        DefaultSchemaComboBox.ValueMember = "DATABASE_NAME";
        DefaultSchemaComboBox.DataSource = _schemasTable;
      }
    }

    /// <summary>
    /// Event delegate method fired when the <see cref="MySqlWorkbenchConnectionDialog"/> is being closed.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void MySqlWorkbenchConnectionDialog_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (DialogResult != DialogResult.OK)
      {
        return;
      }

      var errorMessage = PerformBeforeClosingValidations();
      if (!string.IsNullOrEmpty(errorMessage))
      {
        InfoDialog.ShowDialog(InfoDialogProperties.GetInformationDialogProperties(Resources.GenericErrorTitle, errorMessage));
        DialogResult = DialogResult.None;
        e.Cancel = true;
        return;
      }

      // If a connection is being updated set the saved status to updated so the edited connection is replaced with the edited one.
      if (_existingConnection != null && !_existingConnection.Equals(WorkbenchConnection))
      {
        WorkbenchConnection.SavedStatus = MySqlWorkbenchConnection.SavedStatusType.Updated;
      }

      // Save the connection in the corresponding XML file.
      SaveSuccessful = MySqlWorkbench.Connections.SaveConnection(WorkbenchConnection);
      if (_existingConnection != null && SaveSuccessful)
      {
        _existingConnection.Sync(WorkbenchConnection, false);
      }
    }

    /// <summary>
    /// Event delegate method fired when the <see cref="MySqlWorkbenchConnectionDialog"/> is being closed.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void MySqlWorkbenchConnectionDialog_Shown(object sender, EventArgs e)
    {
      IsUserInput = true;
      FireAllValidationsAsync();
    }

    /// <summary>
    /// Event delegate method fired when the selected tab of the <see cref="ParametersTabControl"/> changes.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void ParametersTabControl_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (ParametersTabControl.SelectedIndex < 0)
      {
        return;
      }

      FireAllValidationsAsync();
    }

    /// <summary>
    /// Performs validations before closing the dialog and saving the connection.
    /// </summary>
    /// <returns>An error message if not valid, <c>string.Empty</c> otherwise.</returns>
    private string PerformBeforeClosingValidations()
    {
      // Validate if MySQL Workbench is running and if we can save the connection and password.
      var errorMessage = !ValidateAddingOrEditingConnections(false)
        ? Resources.AddOrEditConnectionsNotAllowed
        : ValidateSchema(_connectionMethodType == MySqlWorkbenchConnection.ConnectionMethodType.Ssh ? MySqlDefaultSchemaComboBox.Text : DefaultSchemaComboBox.Text, true);
      return errorMessage;
    }

    /// <summary>
    /// Sets the visibility of controls in the regular parameters page depending on the connection method.
    /// </summary>
    private void SetParametersPageVisibility()
    {
      bool pageVisible = _connectionMethodType != MySqlWorkbenchConnection.ConnectionMethodType.Ssh;
      SetTabPageVisibility(ParametersTabPage, 0, pageVisible);
      if (!pageVisible)
      {
        return;
      }

      // Hostname and Port
      var hostNameAndPortVisible = _connectionMethodType != MySqlWorkbenchConnection.ConnectionMethodType.LocalUnixSocketOrWindowsPipe;
      HostnameLabel.Visible = hostNameAndPortVisible;
      HostNameTextBox.Visible = hostNameAndPortVisible;
      HelpMySqlHostNameLabel.Visible = hostNameAndPortVisible;
      PortLabel.Visible = hostNameAndPortVisible;
      PortTextBox.Visible = hostNameAndPortVisible;
      HelpHostPortLabel.Visible = hostNameAndPortVisible;
      if (!hostNameAndPortVisible)
      {
        SetControlTextSkippingValidation(HostNameTextBox, null);
        SetControlTextSkippingValidation(PortTextBox, null);
      }

      // Local Unix Socket / Windows Pipe
      var socketVisible = _connectionMethodType == MySqlWorkbenchConnection.ConnectionMethodType.LocalUnixSocketOrWindowsPipe;
      SocketLabel.Visible = socketVisible;
      SocketTextBox.Visible = socketVisible;
      HelpSocketLabel.Visible = socketVisible;
      if (!socketVisible)
      {
        SetControlTextSkippingValidation(SocketTextBox, null);
      }

      // MySQL X Protocol port warning
      var mySqlXPortWarningVisible = _connectionMethodType == MySqlWorkbenchConnection.ConnectionMethodType.XProtocol;
      MySqlXPortWarningPictureBox.Visible = mySqlXPortWarningVisible;
      MySqlXPortWarningLabel.Visible = mySqlXPortWarningVisible;
    }

    /// <summary>
    /// Sets the visibility of controls in the SSH parameters page depending on the connection method.
    /// </summary>
    private void SetSshParametersPageVisibility()
    {
      var sshVisible = _connectionMethodType == MySqlWorkbenchConnection.ConnectionMethodType.Ssh;
      SetTabPageVisibility(SshParametersTabPage, 0, sshVisible);
      if (sshVisible)
      {
        return;
      }

      SetControlTextSkippingValidation(MySqlDefaultSchemaComboBox, null);
      foreach (Control control in SshParametersTabPage.Controls)
      {
        if (control is TextBox)
        {
          SetControlTextSkippingValidation(control, null);
        }
      }
    }

    /// <summary>
    /// Sets the visibility of controls in the Advanced parameters page depending on the connection method.
    /// </summary>
    private void SetAdvancedPageVisibility()
    {
      SetTabPageVisibility(AdvancedTabPage, 2, _connectionMethodType != MySqlWorkbenchConnection.ConnectionMethodType.XProtocol);
    }

    /// <summary>
    /// Initializes the SSL parameters page .
    /// </summary>
    private void InitializeSslParametersPage()
    {
      SetTabPageVisibility(SslTabPage, 1, true);
      UseSslComboBox.SelectedIndex = (int)WorkbenchConnection.UseSsl;
    }

    /// <summary>
    /// Sets the visibility for <see cref="TabPage"/> controls.
    /// </summary>
    /// <param name="tabPage">The <see cref="TabPage"/> to show or hide.</param>
    /// <param name="ordinal">The order of the page within the collection.</param>
    /// <param name="visible">Flag indicating if the control is shown or hidden.</param>
    private void SetTabPageVisibility(TabPage tabPage, int ordinal, bool visible)
    {
      if (visible)
      {
        _hiddenTabPages.Remove(tabPage);
        if (!ParametersTabControl.Contains(tabPage))
        {
          ParametersTabControl.TabPages.Insert(ordinal, tabPage);
        }
      }
      else
      {
        ParametersTabControl.TabPages.Remove(tabPage);
        if (!_hiddenTabPages.Contains(tabPage))
        {
          _hiddenTabPages.Add(tabPage);
        }
      }
    }

    /// <summary>
    /// Shows an <see cref="OpenFileDialog"/> configured to select a specific type of file needed for the connection configuration.
    /// </summary>
    /// <param name="parameterFileType">The type of parameter file.</param>
    /// <returns>The selected file's path and name.</returns>
    private string ShowSslFileDialog(ParameterFileType parameterFileType)
    {
      using (var fileDialog = new OpenFileDialog())
      {
        string invalidFileTitle = string.Empty;
        string invalidFileDetail = string.Empty;
        switch (parameterFileType)
        {
          case ParameterFileType.SshPrivateKey:
            fileDialog.DefaultExt = "ppk";
            fileDialog.Filter = Resources.SshPrivateKeyFilter;
            fileDialog.Title = Resources.SshPrivateKeyTitle;
            invalidFileTitle = Resources.InvalidPrivateKeyFileTitle;
            invalidFileDetail = Resources.InvalidPrivateKeyFileDetail;
            break;

          case ParameterFileType.SslPemCertificationAuthority:
            fileDialog.DefaultExt = "pem";
            fileDialog.Filter = Resources.SslCertificateFilter;
            fileDialog.Title = Resources.SslCertificationAuthorityTitle;
            invalidFileTitle = Resources.InvalidPemFileTitle;
            invalidFileDetail = Resources.InvalidPemFileDetail;
            break;

          case ParameterFileType.SslPemClientCertificate:
            fileDialog.DefaultExt = "pem";
            fileDialog.Filter = Resources.SslCertificateFilter;
            fileDialog.Title = Resources.SslClientCertificateTitle;
            invalidFileTitle = Resources.InvalidPemFileTitle;
            invalidFileDetail = Resources.InvalidPemFileDetail;
            break;

          case ParameterFileType.SslPemKey:
            fileDialog.DefaultExt = "pem";
            fileDialog.Filter = Resources.SslCertificateFilter;
            fileDialog.Title = Resources.SslKeyTitle;
            invalidFileTitle = Resources.InvalidPemFileTitle;
            invalidFileDetail = Resources.InvalidPemFileDetail;
            break;
        }

        if (fileDialog.ShowDialog() != DialogResult.OK)
        {
          return null;
        }

        if (string.IsNullOrEmpty(fileDialog.FileName) || !File.Exists(fileDialog.FileName))
        {
          InfoDialog.ShowDialog(InfoDialogProperties.GetErrorDialogProperties(invalidFileTitle, invalidFileDetail));
        }

        return fileDialog.FileName;
      }
    }

    /// <summary>
    /// Event delegate method fired when the <see cref="SshKeyFileButton"/> button is clicked.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void SshKeyFileButton_Click(object sender, EventArgs e)
    {
      string selectedFileName = ShowSslFileDialog(ParameterFileType.SshPrivateKey);
      if (string.IsNullOrEmpty(selectedFileName))
      {
        return;
      }

      SshKeyFileTextBox.Focus();
      SshKeyFileTextBox.Text = selectedFileName;
    }

    /// <summary>
    /// Event delegate method fired when the <see cref="SslCaFileButton"/> button is clicked.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void SslCaFileButton_Click(object sender, EventArgs e)
    {
      string selectedFileName = ShowSslFileDialog(ParameterFileType.SslPemCertificationAuthority);
      if (string.IsNullOrEmpty(selectedFileName))
      {
        return;
      }

      SslCaFileTextBox.Focus();
      SslCaFileTextBox.Text = selectedFileName;
    }

    /// <summary>
    /// Event delegate method fired when the <see cref="SslCertFileButton"/> button is clicked.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void SslCertFileButton_Click(object sender, EventArgs e)
    {
      string selectedFileName = ShowSslFileDialog(ParameterFileType.SslPemClientCertificate);
      if (string.IsNullOrEmpty(selectedFileName))
      {
        return;
      }

      SslCertFileTextBox.Focus();
      SslCertFileTextBox.Text = selectedFileName;
    }

    /// <summary>
    /// Event delegate method fired when the <see cref="SslKeyFileButton"/> button is clicked.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void SslKeyButton_Click(object sender, EventArgs e)
    {
      string selectedFileName = ShowSslFileDialog(ParameterFileType.SslPemKey);
      if (string.IsNullOrEmpty(selectedFileName))
      {
        return;
      }

      SslKeyFileTextBox.Focus();
      SslKeyFileTextBox.Text = selectedFileName;
    }

    /// <summary>
    /// Event delegate method fired when the <see cref="SslPemMoreInfoLinkLabel"/> link label is clicked.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void SslPemMoreInfoLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      System.Diagnostics.Process.Start(CREATING_SSL_FILES_WITH_MYSQL_LINK);
    }

    /// <summary>
    /// Tests the current <see cref="WorkbenchConnection"/> to check if the connection can be established.
    /// </summary>
    /// <param name="silent">Flag indicating whether the test is performed silently without prompting the result to the users.</param>
    private void TestConnection(bool silent)
    {
      if (WorkbenchConnection == null)
      {
        return;
      }

      MySqlWorkbench.ChangeCurrentCursor(Cursors.WaitCursor);
      OKButton.Enabled = false;
      TestConnectionButton.Enabled = false;
      bool hasPassword = !string.IsNullOrEmpty(WorkbenchConnection.Password);
      ConnectionResultType = WorkbenchConnection.TestConnectionSilently(out var connectionException);

      if (!silent)
      {
        string connectionTestDetails;
        string connectionTestTitle;
        string connectionTestSubDetails = null;
        if (ConnectionResultType == ConnectionResultType.ConnectionSuccess)
        {
          connectionTestTitle = Resources.ConnectionSuccessTitle;
          connectionTestDetails = string.Format(Resources.ConnectionSuccessDetail, WorkbenchConnection.HostIdentifier, WorkbenchConnection.UserName);
        }
        else
        {
          connectionTestTitle = Resources.ConnectionErrorTitle;
          connectionTestDetails = string.Format(hasPassword ? Resources.ConnectionErrorDetailWithPassword : Resources.ConnectionErrorDetailNoPassword, WorkbenchConnection.HostIdentifier, WorkbenchConnection.UserName);
          if (connectionException != null)
          {
            connectionTestDetails += $"{Environment.NewLine} {connectionException.Message}" +
              $"{Environment.NewLine}";
            connectionTestSubDetails = connectionException.InnerException?.Message;
          }
        }

        InfoDialog.ShowDialog(ConnectionResultType == ConnectionResultType.ConnectionSuccess
          ? InfoDialogProperties.GetSuccessDialogProperties(connectionTestTitle, connectionTestDetails)
          : InfoDialogProperties.GetErrorDialogProperties(connectionTestTitle, connectionTestDetails, connectionTestSubDetails));
      }

      MySqlWorkbench.ChangeCurrentCursor(Cursors.Default);
      TestConnectionButton.Enabled = true;
      UpdateAcceptButton();
    }

    /// <summary>
    /// Event delegate method fired when the <see cref="TestConnectionButton"/> button is clicked.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void TestConnectionButton_Click(object sender, EventArgs e)
    {
      TestConnection(false);
    }

    /// <summary>
    /// Event delegate method fired when the selected index of the <see cref="UseSslComboBox"/> changes.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void UseSslComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (UseSslComboBox.SelectedIndex < 0
          || UseSslComboBox.SelectedValue == null)
      {
        return;
      }

      var useSsl = (MySqlWorkbenchConnection.UseSslType) UseSslComboBox.SelectedValue;
      SslKeyFileButton.Enabled =
        SslCertFileButton.Enabled = useSsl == MySqlWorkbenchConnection.UseSslType.RequireAndVerifyIdentity;
      SslKeyFileTextBox.ReadOnly =
        SslCertFileTextBox.ReadOnly =
          !SslKeyFileButton.Enabled;
      SslCaFileButton.Enabled = useSsl == MySqlWorkbenchConnection.UseSslType.RequireAndVerifyCertificationAuthority
                                || useSsl == MySqlWorkbenchConnection.UseSslType.RequireAndVerifyIdentity;
      SslCaFileTextBox.ReadOnly = !SslCaFileButton.Enabled;
      SslKeyFileTextBox.Validate();
      SslCertFileTextBox.Validate();
      SslCaFileTextBox.Validate();
    }

    /// <summary>
    /// Validates if MySQL Workbench is running and displays an error if so since adding or editing connections is not allowed while MySQL Workbench is running.
    /// </summary>
    /// <param name="displayError">A flag indicating if an error message is displayed to users.</param>
    /// <returns><c>true</c> if adding or editing connections is allowed, <c>false</c> otherwise.</returns>
    private bool ValidateAddingOrEditingConnections(bool displayError = true)
    {
      bool allowAddingOrEditingConnections = !MySqlWorkbench.UseWorkbenchConnections || !MySqlWorkbench.IsRunning;
      if (!allowAddingOrEditingConnections && displayError)
      {
        InfoDialog.ShowDialog(InfoDialogProperties.GetErrorDialogProperties(Resources.GenericErrorTitle, Resources.AddOrEditConnectionsNotAllowed, Resources.CloseWBAdviceToAddOrEditConnections));
      }

      return allowAddingOrEditingConnections;
    }

    /// <summary>
    /// Event delegate method fired when a property value of the <see cref="WorkbenchConnection"/> object changes.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void WorkbenchConnection_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      switch (e.PropertyName)
      {
        case "ConnectionStatus":
          ConnectionStatusDescriptionLabel.Text = WorkbenchConnection.ConnectionStatusText;
          if (StatusImageList.Images.Count > 0)
          {
            ConnectionStatusPictureBox.Image = StatusImageList.Images[(int)WorkbenchConnection.ConnectionStatus];
          }
          break;
      }
    }

    /// <summary>
    /// Event delegate method fired after the <see cref="MySqlDefaultSchemaComboBox"/> drop-down list is displayed.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void MySqlDefaultSchemaComboBox_DropDown(object sender, EventArgs e)
    {
      InitializeDefaultSchemaComboBox();
    }

    /// <summary>
    /// Validates that connection name is not empty and does not exist already if saving a new connection.
    /// </summary>
    /// <returns>An error message if not valid, <c>string.Empty</c> otherwise.</returns>
    private string ValidateConnectionName()
    {
      var errorMessage = string.Empty;
      if (string.IsNullOrEmpty(WorkbenchConnection.Name))
      {
        errorMessage = Resources.ConnectionNameRequiredError;
      }
      else if (WorkbenchConnection.SavedStatus == MySqlWorkbenchConnection.SavedStatusType.New
               && MySqlWorkbench.Connections.Any(conn => string.Equals(conn.Name, WorkbenchConnection.Name, StringComparison.OrdinalIgnoreCase)))
      {
        errorMessage = string.Format(Resources.ConnectionAlreadyExistsError, WorkbenchConnection.Name);
      }

      return errorMessage;
    }

    /// <summary>
    /// Validates a file path.
    /// </summary>
    /// <param name="filePath">A file path.</param>
    /// <param name="isRequired">Flag indicating whether a file is required.</param>
    /// <returns>An error message if not valid, <c>string.Empty</c> otherwise.</returns>
    private string ValidateFilePath(string filePath, bool isRequired)
    {
      bool emptyFilePath = string.IsNullOrEmpty(filePath);
      if (!isRequired && emptyFilePath)
      {
        return null;
      }

      return emptyFilePath
        ? Resources.FileRequiredError
        : File.Exists(filePath)
          ? string.Empty
          : Resources.FileDoesNotExistError;
    }

    /// <summary>
    /// Validates the default schema.
    /// </summary>
    /// <param name="schema">The schema name.</param>
    /// <param name="againstDatabaseValidation">Flag indicating whether a validation to check if the schema really exists is done, which requires a conenction to the server.</param>
    /// <returns>An error message if not valid, <c>string.Empty</c> otherwise.</returns>
    private string ValidateSchema(string schema, bool againstDatabaseValidation)
    {
      if (!_enforceSchema)
      {
        return string.Empty;
      }

      var errorMessage = string.Empty;
      if (string.IsNullOrEmpty(errorMessage))
      {
        errorMessage = Resources.DefaultSchemaRequiredError;
      }
      else if (!string.Equals(schema, _initialSchema, StringComparison.InvariantCultureIgnoreCase)
               && againstDatabaseValidation)
      {
        if (WorkbenchConnection.ConnectionStatus == MySqlWorkbenchConnection.ConnectionStatusType.Unknown)
        {
          TestConnection(true);
        }

        if (WorkbenchConnection.ConnectionStatus == MySqlWorkbenchConnection.ConnectionStatusType.RefusingConnections)
        {
          errorMessage = string.Format(Resources.SchemaValidationFailConnectionFailure, WorkbenchConnection.HostIdentifier);
        }
        else
        {
          if (_schemasTable == null)
          {
            InitializeDefaultSchemaComboBox();
          }

          if (_schemasTable == null || _schemasTable.Rows.Count == 0)
          {
            errorMessage = Resources.SchemaValidationFailNoSchemas;
          }
          else
          {
            var results = _schemasTable.Select($"DATABASE_NAME = '{(_connectionMethodType == MySqlWorkbenchConnection.ConnectionMethodType.Ssh ? MySqlDefaultSchemaComboBox.Text : DefaultSchemaComboBox.Text)}'");
            if (results.Length == 0)
            {
              errorMessage = string.Format(Resources.SchemaValidationFailSchemaNotFound, schema);
            }
          }
        }
      }

      return errorMessage;
    }

    /// <summary>
    /// Validates the SSH pass phrase against the specified SSH key file.
    /// </summary>
    /// <returns>An error message if not valid, <c>string.Empty</c> otherwise.</returns>
    private string ValidateSshPassPhrase()
    {
      var errorMessage = string.Empty;
      Renci.SshNet.PrivateKeyFile privateKeyFile = null;
      try
      {
        privateKeyFile = new Renci.SshNet.PrivateKeyFile(SshKeyFileTextBox.Text, SshPassPhraseTextBox.Text);
      }
      catch (Exception ex)
      {
        errorMessage = ex.Message;
      }
      finally
      {
        privateKeyFile?.Dispose();
      }

      return errorMessage;
    }
  }
}