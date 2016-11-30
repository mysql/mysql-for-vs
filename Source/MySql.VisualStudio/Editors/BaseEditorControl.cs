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
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using System.IO;
using System.Globalization;
using MySql.Data.MySqlClient;
using MySql.Data.VisualStudio.Properties;
using MySql.Utility.Classes;
using MySql.Utility.Classes.MySql;
using MySql.Utility.Forms;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace MySql.Data.VisualStudio.Editors
{
  public class BaseEditorControl : UserControl, IVsPersistDocData, IPersistFileFormat
  {
    /// <summary>
    /// Text for inexisting values.
    /// </summary>
    public const string NONE_TEXT = "<none>";

    /// <summary>
    /// Text for untitled connections.
    /// </summary>
    public const string UNTITLED_CONNECTION = "Untitled Connection";

    /// <summary>
    /// Text for displaying the connection's method/protocol.
    /// </summary>
    public const string CONNECTION_METHOD_FORMAT_TEXT = "Connection Method: {0}";

    /// <summary>
    /// Text for displaying the connection's host identifier.
    /// </summary>
    public const string HOST_ID_FORMAT_TEXT = "Host ID: {0}";

    /// <summary>
    /// Text for displaying the connected server version.
    /// </summary>
    public const string SERVER_VERSION_FORMAT_TEXT = "Server Version: {0}";

    /// <summary>
    /// Text for displaying the user name used in the connection.
    /// </summary>
    public const string USER_FORMAT_TEXT = "User: {0}";

    /// <summary>
    /// Text for displaying the connected schema.
    /// </summary>
    public const string SCHEMA_FORMAT_TEXT = "Schema: {0}";

    /// <summary>
    /// The connection used by the editor.
    /// </summary>
    private DbConnection _connection;

    private bool _savingFile;
    protected ServiceProvider ServiceProvider;
    protected string FileName;

    /// <summary>
    /// Gests the <see cref="MySqlDataProviderPackage"/> of this plugin.
    /// </summary>
    public MySqlDataProviderPackage Package { get; protected set; }

    /// <summary>
    /// Gets the connection used yb the editor.
    /// </summary>
    public DbConnection Connection
    {
      get
      {
        return _connection;
      }

      private set
      {
        _connection = value;
        if (_connection == null || _connection.State == ConnectionState.Open)
        {
          return;
        }

        // Open the connection in case it was closed.
        var mySqlConnection = _connection as MySqlConnection;
        if (mySqlConnection != null)
        {
          _connection.ConnectionString = mySqlConnection.GetCompleteConnectionString();
        }

        _connection.Open();
      }
    }

    /// <summary>
    /// Gets the display name of the connection used for the editor.
    /// </summary>
    public string ConnectionName { get; private set; }

    /// <summary>
    /// Gets the name of the database being used by the code editor.
    /// </summary>
    public string CurrentDatabase { get; protected set; }

    /// <summary>
    /// Gets or sets the <see cref="ToolStrip"/> containing actions for the editor.
    /// </summary>
    protected ToolStrip EditorActionsToolStrip { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the editor is used for JavaScript and Python.
    /// </summary>
    protected bool IsHybrid { get; set; }

    protected DbProviderFactory Factory;

    protected bool[] IsColBlob = null;

    #region IVsPersistDocData Members

    int IVsPersistDocData.Close()
    {
      return VSConstants.S_OK;
    }

    int IVsPersistDocData.GetGuidEditorType(out Guid pClassID)
    {
      throw new NotImplementedException();
    }

    int IVsPersistDocData.IsDocDataDirty(out int pfDirty)
    {
      return ((IPersistFileFormat)this).IsDirty(out pfDirty);
    }

    int IVsPersistDocData.IsDocDataReloadable(out int pfReloadable)
    {
      pfReloadable = 1;
      return VSConstants.S_OK;
    }

    int IVsPersistDocData.LoadDocData(string pszMkDocument)
    {
      return ((IPersistFileFormat)this).Load(pszMkDocument, 0, 0);
    }

    int IVsPersistDocData.OnRegisterDocData(uint docCookie, IVsHierarchy pHierNew, uint itemidNew)
    {
      return VSConstants.S_OK;
    }

    int IVsPersistDocData.ReloadDocData(uint grfFlags)
    {
      return ((IPersistFileFormat)this).Load(null, grfFlags, 0);
    }

    int IVsPersistDocData.RenameDocData(uint grfAttribs, IVsHierarchy pHierNew, uint itemidNew, string pszMkDocumentNew)
    {
      FileName = pszMkDocumentNew;
      return VSConstants.S_OK;
    }

    private int QuerySave(out tagVSQuerySaveResult qsResult)
    {
      uint result;
      IVsQueryEditQuerySave2 querySave =
        (IVsQueryEditQuerySave2)ServiceProvider.GetService(typeof(SVsQueryEditQuerySave));
      int hr = querySave.QuerySaveFile(FileName, 0, null, out result);
      qsResult = (tagVSQuerySaveResult)result;
      return hr;
    }

    int IVsPersistDocData.SaveDocData(VSSAVEFLAGS dwSave, out string pbstrMkDocumentNew, out int pfSaveCanceled)
    {
      pbstrMkDocumentNew = null;
      pfSaveCanceled = 0;
      int hr;

      IVsUIShell uiShell = (IVsUIShell)ServiceProvider.GetService(typeof(IVsUIShell));

      switch (dwSave)
      {
        case VSSAVEFLAGS.VSSAVE_Save:
        case VSSAVEFLAGS.VSSAVE_SilentSave:
          {
            tagVSQuerySaveResult qsResult;
            hr = QuerySave(out qsResult);
            if (ErrorHandler.Failed(hr)) return hr;

            if (qsResult == tagVSQuerySaveResult.QSR_NoSave_Cancel)
              pfSaveCanceled = ~0;
            else if (qsResult == tagVSQuerySaveResult.QSR_SaveOK)
            {
              hr = uiShell.SaveDocDataToFile(dwSave, this, FileName,
                  out pbstrMkDocumentNew, out pfSaveCanceled);
              if (ErrorHandler.Failed(hr)) return hr;
            }
            else if (qsResult == tagVSQuerySaveResult.QSR_ForceSaveAs)
            {
              hr = uiShell.SaveDocDataToFile(VSSAVEFLAGS.VSSAVE_SaveAs, this, FileName,
                  out pbstrMkDocumentNew, out pfSaveCanceled);
              if (ErrorHandler.Failed(hr)) return hr;
            }
            break;
          }

        case VSSAVEFLAGS.VSSAVE_SaveAs:
        case VSSAVEFLAGS.VSSAVE_SaveCopyAs:
          {
            // --- Make sure the file name as the right extension
            if (String.Compare(".mysql", Path.GetExtension(FileName), true,
              CultureInfo.CurrentCulture) != 0)
              FileName += ".mysql";

            // --- Call the shell to do the save for us
            hr = uiShell.SaveDocDataToFile(dwSave, this, FileName,
              out pbstrMkDocumentNew, out pfSaveCanceled);
            if (ErrorHandler.Failed(hr)) return hr;
            break;
          }
        default:
          throw new ArgumentException("Unable to save file");
      }
      return VSConstants.S_OK;
    }

    int IVsPersistDocData.SetUntitledDocPath(string pszDocDataPath)
    {
      FileName = pszDocDataPath;
      return VSConstants.S_OK;
    }

    #endregion

    #region IPersistFileFormat Members

    int IPersistFileFormat.GetClassID(out Guid pClassID)
    {
      throw new NotImplementedException();
    }

    int IPersistFileFormat.GetCurFile(out string ppszFilename, out uint pnFormatIndex)
    {
      ppszFilename = FileName;
      pnFormatIndex = 0;
      return VSConstants.S_OK;
    }

    int IPersistFileFormat.GetFormatList(out string ppszFormatList)
    {
      ppszFormatList = GetFileFormatList();
      return VSConstants.S_OK;
    }

    int IPersistFileFormat.InitNew(uint nFormatIndex)
    {
      return VSConstants.S_OK;
    }

    int IPersistFileFormat.IsDirty(out int pfIsDirty)
    {
      pfIsDirty = IsDirty ? 1 : 0;
      return VSConstants.S_OK;
    }

    int IPersistFileFormat.Load(string pszFilename, uint grfMode, int fReadOnly)
    {
      // --- A valid file name is required.
      if ((pszFilename == null) && ((FileName == null) || (FileName.Length == 0)))
        throw new ArgumentNullException("pszFilename");

      int hr = VSConstants.S_OK;
      // --- If the new file name is null, then this operation is a reload
      bool isReload = pszFilename == null;
      // --- Set the new file name
      if (!isReload)
      {
        FileName = pszFilename;
      }
      // --- Load the file
      LoadFile(FileName);
      IsDirty = false;
      // --- Notify the load or reload
      //NotifyDocChanged();
      return hr;
    }

    int IPersistFileFormat.Save(string pszFilename, int fRemember, uint nFormatIndex)
    {
      // --- switch into the NoScribble mode
      _savingFile = true;
      try
      {
        // --- If file is null or same --> SAVE
        if (pszFilename == null || pszFilename == FileName)
        {
          SaveFile(FileName);
          IsDirty = false;
        }
        else
        {
          // --- If remember --> SaveAs
          if (fRemember != 0)
          {
            FileName = pszFilename;
            SaveFile(FileName);
            IsDirty = false;
            EditorBroker.UpdateEditorDocumentPath(EditorBroker.Broker.GetActiveDocumentFullName(), FileName);
          }
          else // --- Else, Save a Copy As
          {
            SaveFile(pszFilename);
          }
        }
      }
      finally
      {
        // --- Switch into the Normal mode
        _savingFile = false;
      }
      return VSConstants.S_OK;
    }

    int IPersistFileFormat.SaveCompleted(string pszFilename)
    {
      if (_savingFile)
        return VSConstants.S_FALSE;
      return VSConstants.S_OK;
    }

    #endregion

    #region IPersist Members

    public int GetClassID(out Guid pClassID)
    {
      throw new NotImplementedException();
    }

    #endregion

    #region Virtuals

    /// <summary>
    /// Gets the file format list for the 'Save File' Dialog.
    /// </summary>
    /// <returns>Imlementing Editor's file extensions</returns>
    protected virtual string GetFileFormatList() { return null; }

    /// <summary>
    /// Intended to be overwriten at inheriting child, this method should provide access to the
    /// DocumentPath of the Pane property from a given Editor class, without requiring Editor's
    /// consumers to cast the object to a given type.
    /// </summary>
    internal virtual string GetDocumentPath() { return null; }

    /// <summary>
    /// Intended to be overwriten at inheriting child, this method should allow to update the
    /// DocumentPath of the Pane property from a given Editor class, without requiring Editor's
    /// consumers to cast the object to a given type.
    /// </summary>
    /// <param name="documentPath">New document path.</param>
    internal virtual void SetDocumentPath(string documentPath) { }

    protected virtual void SaveFile(string newFileName) { }
    protected virtual void LoadFile(string newFileName) { }
    protected virtual bool IsDirty { get; set; }

    #endregion

    #region IPersist Members

    int IPersist.GetClassID(out Guid pClassID)
    {
      throw new NotImplementedException();
    }

    #endregion

    /// <summary>
    /// Sets the <see cref="Connection"/> and <see cref="ConnectionName"/> property values.
    /// </summary>
    /// <param name="connection">A <see cref="DbConnection"/> to set in <see cref="Connection"/>.</param>
    /// <param name="connectionName">A connection name to set in <see cref="ConnectionName"/>.</param>
    public void SetConnection(DbConnection connection, string connectionName)
    {
      Connection = connection;
      ConnectionName = connection == null
        ? string.Empty
        : connectionName;
      UpdateButtons();
    }


    /// <summary>
    /// Hides the results pane.
    /// </summary>
    protected void ClearResults()
    {
      var foundControl = this.GetChildControl("ResultsTabControl");
      if (foundControl == null)
      {
        return;
      }

      var resultsTab = foundControl as TabControl;
      if (resultsTab == null)
      {
        return;
      }

      // Clear tab pages.
      resultsTab.TabPages.Clear();

      // The tab control needs to be invisible when it has 0 tabs so the background matches the theme.
      resultsTab.Visible = false;

      foundControl = this.GetChildControl("CodeEditor");
      if (foundControl == null)
      {
        return;
      }

      var codeEditor = foundControl as VSCodeEditorUserControl;
      if (codeEditor == null)
      {
        return;
      }

      codeEditor.Dock = DockStyle.Fill;
    }

    /// <summary>
    /// Event delegate method fired when the button to connect to the database is clicked.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    protected void ConnectButtonClick(object sender, EventArgs e)
    {
      try
      {
        using (var connectDialog = new ConnectDialog())
        {
          connectDialog.Connection = Connection;
          if (connectDialog.ShowDialog() == DialogResult.Cancel)
          {
            return;
          }

          // Check if the MySQL Server version supports the X Protocol.
          if (IsHybrid && !connectDialog.Connection.ServerVersionSupportsXProtocol(false))
          {
            InfoDialog.ShowDialog(InfoDialogProperties.GetWarningDialogProperties(Resources.WarningText,
              Resources.NewConnectionNotXProtocolCompatibleDetail, null,
              Resources.NewConnectionNotXProtocolCompatibleMoreInfo));
            return;
          }

          SetConnection(connectDialog.Connection, connectDialog.ConnectionName);
          ClearResults();
        }
      }
      catch (MySqlException ex)
      {
        MySqlSourceTrace.WriteAppErrorToLog(ex, Resources.NewConnectionErrorDetail, Resources.NewConnectionErrorSubDetail, true);
      }
    }

    /// <summary>
    /// Event delegate method fired when the button to disconnect from the database is clicked.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    protected void DisconnectButtonClick(object sender, EventArgs e)
    {
      Connection.Close();
      SetConnection(null, null);
    }

    /// <summary>
    /// Frees resources and performs cleanup.
    /// </summary>
    /// <param name="disposing"></param>
    protected override void Dispose(bool disposing)
    {
      SetBaseEvents(false);
      base.Dispose(disposing);
    }

    /// <summary>
    /// Subscribes some controls to events common to all editors declared in the base <see cref="BaseEditorControl"/>.
    /// </summary>
    /// <param name="subscribe">Flag indicating whether the methods will be subscribed or unsubscribed.</param>
    protected void SetBaseEvents(bool subscribe)
    {
      if (EditorActionsToolStrip == null)
      {
        return;
      }

      if (EditorActionsToolStrip.Items.ContainsKey("ConnectToolStripButton"))
      {
        var connectButton = EditorActionsToolStrip.Items["ConnectToolStripButton"] as ToolStripButton;
        if (connectButton != null)
        {
          connectButton.Click -= ConnectButtonClick;
          if (subscribe)
          {
            connectButton.Click += ConnectButtonClick;
          }
        }
      }

      if (EditorActionsToolStrip.Items.ContainsKey("DisconnectToolStripButton"))
      {
        var disconnectButton = EditorActionsToolStrip.Items["DisconnectToolStripButton"] as ToolStripButton;
        if (disconnectButton != null)
        {
          disconnectButton.Click -= DisconnectButtonClick;
          if (subscribe)
          {
            disconnectButton.Click += DisconnectButtonClick;
          }
        }
      }

      if (EditorActionsToolStrip.Items.ContainsKey("SwitchConnectionToolStripDropDownButton"))
      {
        var switchConnectionButton = EditorActionsToolStrip.Items["SwitchConnectionToolStripDropDownButton"] as ToolStripDropDownButton;
        if (switchConnectionButton != null)
        {
          switchConnectionButton.DropDownOpening -= SwitchConnectionDropDownOpening;
          if (subscribe)
          {
            switchConnectionButton.DropDownOpening += SwitchConnectionDropDownOpening;
          }
        }
      }
    }

    /// <summary>
    /// Reads the current database from the last query executed or batch
    /// of queries.
    /// </summary>
    protected virtual void StoreCurrentDatabase()
    {
      var con = Connection as MySqlConnection;
      if (con == null)
      {
        return;
      }

      try
      {
        MySqlCommand cmd = new MySqlCommand("select database();", con);
        object val = cmd.ExecuteScalar();
        CurrentDatabase = val is DBNull ? string.Empty : val.ToString();
      }
      catch (Exception ex)
      {
        WriteToMySqlOutput(Resources.ConnectionClosedErrorTitle, Resources.ConnectionClosedErrorMessage, null, MessageType.Error);
        MySqlSourceTrace.WriteAppErrorToLog(ex, Resources.ConnectionClosedErrorTitle, Resources.ConnectionClosedErrorMessage, false);
      }
    }

    /// <summary>
    /// Event delegate method fired when one of the items in the fast-switch connections drop-down menu is clicked.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    protected virtual void SwitchConnectionItemClick(object sender, EventArgs e)
    {
      var connectionMenuItem = sender as ToolStripMenuItem;
      if (connectionMenuItem == null)
      {
        return;
      }

      var parent = connectionMenuItem.GetCurrentParent();
      if (parent == null)
      {
        return;
      }

      var connectionName = connectionMenuItem.Text;
      var connection = Package.GetMySqlConnection(connectionName);
      if (connection == null)
      {
        // The connection is no longer present in the Server Explorer
        InfoDialog.ShowDialog(InfoDialogProperties.GetErrorDialogProperties(
          Resources.Editors_SeConnectionNotFoundTitle,
          string.Format(Resources.Editors_SeConnectionNotFoundDetail, connectionName)));
        return;
      }

      // Switch to the selected connection
      SetConnection(connection, connectionName);
      ClearResults();
    }

    /// <summary>
    /// Event delegate method fired when the fast-switch connections drop-down menu is being opened.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    protected virtual void SwitchConnectionDropDownOpening(object sender, EventArgs e)
    {
      var dropDownButton = sender as ToolStripDropDownButton;
      if (dropDownButton == null)
      {
        return;
      }

      Cursor = Cursors.WaitCursor;
      dropDownButton.DropDownItems.Clear();
      var mySqlServerExplorerConnections = Package.GetMySqlConnections();
      foreach (var mySqlServerExplorerConnection in mySqlServerExplorerConnections)
      {
        var mySqlConnection = Package.GetMySqlConnection(mySqlServerExplorerConnection);
        if (mySqlConnection == null)
        {
          continue;
        }

        var isCurrentConnection = Connection != null && (mySqlConnection.Equals(Connection) || mySqlConnection.ConnectionString.Equals(Connection.ConnectionString));
        if (IsHybrid && !isCurrentConnection && !mySqlConnection.ServerVersionSupportsXProtocol(true))
        {
          continue;
        }

        var newItem = new ToolStripMenuItem(mySqlServerExplorerConnection.DisplayName, Resources.database_connect);
        newItem.Click += SwitchConnectionItemClick;
        if (isCurrentConnection)
        {
          newItem.Font = new Font(newItem.Font, FontStyle.Bold);
          newItem.Enabled = false;
        }

        dropDownButton.DropDownItems.Add(newItem);
      }

      Cursor = Cursors.Default;
    }

    /// <summary>
    /// Updates the toolbar buttons.
    /// </summary>
    protected virtual void UpdateButtons()
    {
      if (EditorActionsToolStrip == null)
      {
        return;
      }

      bool connected = Connection != null && Connection.State == ConnectionState.Open;
      EditorActionsToolStrip.Items["RunScriptToolStripButton"].Enabled = connected;
      EditorActionsToolStrip.Items["DisconnectToolStripButton"].Enabled = connected;
      EditorActionsToolStrip.Items["ConnectToolStripButton"].Enabled = !connected;
      UpdateConnectionInformationToolStripMenuItems(connected);
    }

    /// <summary>
    /// Updates the tool strip menu items text.
    /// </summary>
    /// <param name="connected">if set to <c>true</c> [connected].</param>
    protected virtual void UpdateConnectionInformationToolStripMenuItems(bool connected)
    {
      if (EditorActionsToolStrip == null)
      {
        return;
      }

      var connectionInformationDropDown = EditorActionsToolStrip.Items["ConnectionInfoToolStripDropDownButton"] as ToolStripDropDownButton;
      if (connectionInformationDropDown == null)
      {
        return;
      }

      var connectionStringBuilder = connected
        ? new MySqlConnectionStringBuilder(Connection.ConnectionString)
        : null;
      connectionInformationDropDown.Text = connected
        ? (!string.IsNullOrEmpty(ConnectionName) ? ConnectionName : UNTITLED_CONNECTION)
        : NONE_TEXT;
      connectionInformationDropDown.DropDownItems["ConnectionMethodToolStripMenuItem"].Text = string.Format(CONNECTION_METHOD_FORMAT_TEXT,
        connected
          ? connectionStringBuilder.ConnectionProtocol.GetConnectionProtocolDescription()
          : NONE_TEXT);
      connectionInformationDropDown.DropDownItems["HostIdToolStripMenuItem"].Text = string.Format(HOST_ID_FORMAT_TEXT,
        connected
          ? connectionStringBuilder.GetHostIdentifier()
          : NONE_TEXT);
      connectionInformationDropDown.DropDownItems["ServerVersionToolStripMenuItem"].Text = string.Format(SERVER_VERSION_FORMAT_TEXT, connected ? Connection.ServerVersion : NONE_TEXT);
      connectionInformationDropDown.DropDownItems["UserToolStripMenuItem"].Text = string.Format(USER_FORMAT_TEXT,
        connected
          ? connectionStringBuilder.UserID
          : NONE_TEXT);
      connectionInformationDropDown.DropDownItems["SchemaToolStripMenuItem"].Text = string.Format(SCHEMA_FORMAT_TEXT,
        connected
          ? connectionStringBuilder.Database
          : NONE_TEXT);
    }

    /// <summary>
    /// Writes to the My SQL Output Tool Window.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="message">The message.</param>
    /// <param name="duration">The duration.</param>
    /// <param name="messageType">Type of the message.</param>
    protected virtual void WriteToMySqlOutput(string action, string message, string duration, MessageType messageType)
    {
      Utils.WriteToMySqlOutputWindow(action, message, duration, messageType);
    }
  }
}