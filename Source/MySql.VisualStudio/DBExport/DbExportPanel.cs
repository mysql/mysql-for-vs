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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Microsoft.VisualStudio.Data.Services;
using MySql.Data.VisualStudio.Properties;
using MySql.Data.VisualStudio.SchemaComparer;
using System.IO;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio;

namespace MySql.Data.VisualStudio.DBExport
{
  public partial class DbExportPanel : UserControl
  {
    private const string CANCEL = "Cancel";
    private const string EXPORT = "Export";
    private const string SCHEMA = "Schema";
    private const string START_EXPORT = "Start Export";

    private IVsOutputWindowPane _generalPane;
    private List<IVsDataExplorerConnection> _explorerMySqlConnections;
    private string _ownerSchema;
    private BackgroundWorker _worker;
    private Cursor _cursor;
    private MySqlDbExport _mysqlDbExport;
    private ToolWindowPane _windowHandler;
    private bool _actionCancelled;
    private string _fileSavedSettingsName;

    internal MySqlDbExportOptions BndOptions;
    internal List<Schema> Schemas = new List<Schema>();
    internal BindingList<DbSelectedObjects> DbObjects = new BindingList<DbSelectedObjects>();
    internal Dictionary<string, BindingList<DbSelectedObjects>> Dictionary = new Dictionary<string, BindingList<DbSelectedObjects>>();

    public MySqlServerExplorerConnection SelectedConnection { get; set; }
    public List<MySqlServerExplorerConnection> Connections { get; set; }

    private BindingSource _sourceSchemas = new BindingSource();

    public DbExportPanel()
    {
      InitializeComponent();
      _ownerSchema = string.Empty;
      dbSchemasList.CellClick += dbSchemasList_CellClick;
      dbSchemasList.RowLeave += dbSchemasList_RowLeave;
      dbSchemasList.RowEnter += dbSchemasList_RowEnter;

      cmbConnections.SelectionChangeCommitted += cmbConnections_SelectionChangeCommitted;

      IVsOutputWindow outWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;
      Guid generalPaneGuid = VSConstants.GUID_OutWindowGeneralPane;
      if (outWindow != null)
      {
        outWindow.CreatePane(ref generalPaneGuid, "General", 1, 0);
        outWindow.GetPane(ref generalPaneGuid, out _generalPane);
      }
      BndOptions = new MySqlDbExportOptions();
      max_allowed_packet.Text = BndOptions.max_allowed_packet.ToString();
      mySqlDbExportOptionsBindingSource.Add(BndOptions);

      //Click commands
      btnExport.Click += btnExport_Click;
      btnAdvanced.Click += btnAdvanced_Click;
      btnRefresh.Click += btnRefresh_Click;
      btnReturn.Click += btnReturn_Click;
      btnSelectAll.Click += btnSelectAll_Click;
      btnUnSelect.Click += btnUnSelect_Click;
      btnFilter.Click += btnFilter_Click;
      btnSaveFile.Click += btnSaveFile_Click;
      btnSaveSettings.Click += btnSaveSettings_Click;
      btnLoadSettingsFile.Click += btnLoadSettingsFile_Click;
      cmbConnections.DropDown += cmbConnections_DropDown;

      //KeyDown events
      txtFilter.KeyDown += txtFilter_KeyDown;
      txtFileName.KeyDown += txtFileName_KeyDown;
      _sourceSchemas.ListChanged += sourceSchemas_ListChanged;
      _fileSavedSettingsName = string.Empty;

    }

    private void sourceSchemas_ListChanged(object sender, ListChangedEventArgs e)
    {
      if (_windowHandler == null)
      {
        return;
      }

      if (_windowHandler.Caption.Equals(Resources.DbExportToolCaptionFrame))
      {
        _windowHandler.Caption = string.Format("DBExportDoc_{0:MMddyyyyhmmss}.dumps", DateTime.Now);
      }

      if (_fileSavedSettingsName != string.Empty)
      {
        _windowHandler.Caption = Path.GetFileName(_fileSavedSettingsName);
      }

      _windowHandler.Caption = !_windowHandler.Caption.Contains("*") ? _windowHandler.Caption += "*" : _windowHandler.Caption;
    }

    private void dbSchemasList_RowEnter(object sender, DataGridViewCellEventArgs e)
    {
      if (e.ColumnIndex != 1)
      {
        return;
      }

      string schemaSelected = (string)dbSchemasList.Rows[e.RowIndex].Cells[1].Value;
      string prevSchema = GetTreeViewDb();
      if (prevSchema == schemaSelected)
      {
        return;
      }

      if (!string.IsNullOrEmpty(prevSchema))
      {
        PullObjectListFromTree(prevSchema);
      }

      LoadDbObjects(schemaSelected);
    }

    private void dbSchemasList_RowLeave(object sender, DataGridViewCellEventArgs e)
    {
      if (string.IsNullOrEmpty(_ownerSchema))
      {
        return;
      }

      try
      {
        if (dbSchemasList.CurrentCell.ColumnIndex != 0 || !(bool) dbSchemasList.CurrentCell.Value)
        {
          return;
        }

        if (Dictionary.ContainsKey(_ownerSchema))
        {
          Dictionary.Remove(_ownerSchema);
        }

        var dictionaryValue = new BindingList<DbSelectedObjects>(DbObjects.ToList());
        Dictionary.Add(_ownerSchema, dictionaryValue);
      }
      catch
      {
        // ignored
      }
    }

    private void dbSchemasList_CellValueChanged(object sender, ListChangedEventArgs e)
    {
      if (dbSchemasList.CurrentCell == null || dbSchemasList.CurrentCell.ColumnIndex != 0)
      {
        return;
      }

      int currentRow = dbSchemasList.CurrentRow != null ? dbSchemasList.CurrentRow.Index : -1;
      int currentColumn = dbSchemasList.CurrentCell.ColumnIndex;
      if (currentRow >= 0 && dbSchemasList.Rows[currentRow].Cells.Count <= 1)
      {
        return;
      }

      var currentSchema = currentRow >= 0
        ? dbSchemasList.Rows[currentRow].Cells[1].Value.ToString()
        : new MySqlConnectionStringBuilder(SelectedConnection.ConnectionString).Database;
      if (_ownerSchema.Equals(currentSchema, StringComparison.InvariantCultureIgnoreCase) && DbObjects != null)
      {
        //update to UI
        foreach (var item in DbObjects)
        {
          item.Selected = (bool)dbSchemasList.Rows[currentRow].Cells[currentColumn].Value;
        }

        dbObjectsList.Refresh();

      }

      var databaseObjects = GetDbObjects(currentSchema);
      foreach (var item in databaseObjects)
      {
        item.Selected = ((bool)dbSchemasList.Rows[currentRow].Cells[currentColumn].Value);
      }

      if (Dictionary.ContainsKey(currentSchema))
        Dictionary.Remove(currentSchema);

      if (currentRow >= 0 && (bool)dbSchemasList.Rows[currentRow].Cells[currentColumn].Value)
        Dictionary.Add(currentSchema, databaseObjects);
    }

    void cmbConnections_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if ((cmbConnections.SelectedItem as MySqlServerExplorerConnection) == null) return;

      SelectedConnection = new MySqlServerExplorerConnection();
      SelectedConnection.ConnectionString = ((MySqlServerExplorerConnection)cmbConnections.SelectedItem).ConnectionString;
      SelectedConnection.DisplayName = ((MySqlServerExplorerConnection)cmbConnections.SelectedItem).DisplayName;
      LoadSchemasForSelectedConnection();
      Dictionary.Clear();
      DbObjects = null;
      dbObjectsList.Nodes.Clear();
      dbObjectsList.Refresh();
    }

    void dbSchemasList_CellClick(object sender, DataGridViewCellEventArgs e)
    {
      if (e.RowIndex == -1) return;
      if (string.IsNullOrEmpty(dbSchemasList.Rows[e.RowIndex].Cells[1].Value as string))
        return;

      if (e.ColumnIndex == 0)
      {
        var selected = Schemas.Single(t => t.Name.Equals((string)dbSchemasList.Rows[e.RowIndex].Cells[1].Value,
          StringComparison.InvariantCultureIgnoreCase));
        selected.CheckSchema(!selected.Export);
        if (!selected.Export)
        {
          ChangeAllSelectedDbObjects(false);
          Dictionary.Remove(selected.Name);
          return;
        }
        string prevSchema = GetTreeViewDb();
        if (prevSchema != selected.Name)
        {
          if (!string.IsNullOrEmpty(prevSchema))
            PullObjectListFromTree(prevSchema);
          LoadDbObjects(selected.Name);
        }
        ChangeAllSelectedDbObjects(selected.Export);
        dbSchemasList.Refresh();
      }

      if (e.ColumnIndex == 1)
      {
        var schemaSelected = (string)dbSchemasList.Rows[e.RowIndex].Cells[1].Value;
        string prevSchema = GetTreeViewDb();
        if (!string.IsNullOrEmpty(prevSchema))
          PullObjectListFromTree(prevSchema);
        LoadDbObjects(schemaSelected);
      }
    }

    void cmbConnections_DropDown(object sender, EventArgs e)
    {
      _cursor = Cursor;
      Cursor = Cursors.WaitCursor;

      Application.DoEvents();

      IVsDataExplorerConnectionManager connectionManager = GetService(typeof(IVsDataExplorerConnectionManager)) as IVsDataExplorerConnectionManager;
      if (connectionManager == null) return;
      IDictionary<string, IVsDataExplorerConnection> connections = connectionManager.Connections;

      Connections = new List<MySqlServerExplorerConnection>();

      _explorerMySqlConnections = new List<IVsDataExplorerConnection>();

      foreach (var connection in connections)
      {
        if (GuidList.Provider.Equals(connection.Value.Provider))
        {
          Connections.Add(new MySqlServerExplorerConnection { DisplayName = connection.Value.DisplayName, ConnectionString = connection.Value.Connection.DisplayConnectionString });
          _explorerMySqlConnections.Add(connection.Value);
        }
      }

      Cursor = _cursor;

      SetConnectionsList();

      cmbConnections.Refresh();

      if (Connections.Count <= 0)
        return;

      if (Connections.Where(t => t.ConnectionString == SelectedConnection.ConnectionString).SingleOrDefault() != null)
        cmbConnections.SelectedValue = SelectedConnection.ConnectionString;
      else
        cmbConnections.SelectedValue = Connections[0].ConnectionString;
    }

    public void LoadDbObjects(string databaseName)
    {
      _ownerSchema = databaseName;
      DbObjects = null;
      if (Dictionary.ContainsKey(databaseName))
        Dictionary.TryGetValue(databaseName, out DbObjects);
      else
      {
        DbObjects = GetDbObjects(_ownerSchema);
      }

      BindDbObjectsToTree(DbObjects, databaseName);
    }

    private void SetConnectionsList()
    {
      if (Connections == null)
        return;

      BindingSource bindingSource = new BindingSource();
      bindingSource.DataSource = Connections;

      cmbConnections.DataSource = bindingSource.DataSource;
      cmbConnections.DisplayMember = "DisplayName";
      cmbConnections.ValueMember = "ConnectionString";
    }

    private void LoadSchemasForSelectedConnection()
    {
      Schemas = new List<Schema>();

      List<string> schemaNames = null;

      if (SelectedConnection != null)
      {
        schemaNames = SelectObjects.GetSchemas(new MySqlConnectionStringBuilder(GetCompleteConnectionString(SelectedConnection.DisplayName, true)));
      }
      else if (Connections.Count > 0)
        schemaNames = SelectObjects.GetSchemas(new MySqlConnectionStringBuilder(GetCompleteConnectionString(Connections[0].ConnectionString, true)));

      if (schemaNames == null)
      {
        MessageBox.Show(Resources.dbExportPanel_SchemasLoadError, Resources.MessageBoxErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        _sourceSchemas.DataSource = null;
        dbSchemasList.Refresh();
        return;
      }

      schemaNames.ForEach(t =>
      {
        if (t.StartsWith(".")) return;
        if (Dictionary.ContainsKey(t))
          Schemas.Add(new Schema(true, t));
        else
          Schemas.Add(new Schema(false, t));
      });

      _sourceSchemas.DataSource = Schemas;
      _sourceSchemas.ListChanged += dbSchemasList_CellValueChanged;
      dbSchemasList.DataSource = _sourceSchemas;
      FormatSchemasList();

    }

    public void FormatSchemasList()
    {
      if (dbSchemasList.Rows.Count <= 1 && dbSchemasList.Columns.Count == 1)
      {
        _sourceSchemas.DataSource = new BindingList<Schema>();
        dbSchemasList.DataSource = _sourceSchemas;
        dbSchemasList.Update();
      }

      dbSchemasList.Columns[0].HeaderText = EXPORT;
      dbSchemasList.Columns[0].Width = 45;

      dbSchemasList.Columns[1].HeaderText = SCHEMA;
      dbSchemasList.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      dbSchemasList.Columns[1].ReadOnly = true;

      dbSchemasList.Refresh();
    }

    public void LoadConnections(List<IVsDataExplorerConnection> connections, string selectedConnectionName, ToolWindowPane windowHandler)
    {
      MySqlServerExplorerConnection selected = null;

      try
      {
        _explorerMySqlConnections = connections;
        Connections = new List<MySqlServerExplorerConnection>();
        foreach (var item in connections)
        {
          Connections.Add(new MySqlServerExplorerConnection { DisplayName = item.DisplayName, ConnectionString = item.Connection.DisplayConnectionString });
          if (selectedConnectionName.Equals(item.DisplayName, StringComparison.InvariantCultureIgnoreCase))
          {
            selected = new MySqlServerExplorerConnection { DisplayName = item.DisplayName, ConnectionString = item.Connection.DisplayConnectionString };
          }
        }
        SetConnectionsList();
        if (selected == null)
        {
          MessageBox.Show(Resources.dbExportPanel_LoadConnectionsError, Resources.MessageBoxErrorTitle, MessageBoxButtons.OK);
          return;
        }
        cmbConnections.SelectedValue = selected.ConnectionString;
        SelectedConnection = selected;
        LoadSchemasForSelectedConnection();
        _windowHandler = windowHandler;
      }
      catch (Exception)
      {
        MessageBox.Show(Resources.UnableToRetrieveDatabaseList, Resources.MessageBoxErrorTitle, MessageBoxButtons.OK);
      }
    }

    private void EnableControls(bool enabled)
    {
      // enables/disables all input controls save btnCancel
      cmbConnections.Enabled = enabled;
      txtFilter.Enabled = enabled;
      btnFilter.Enabled = enabled;
      dbSchemasList.Enabled = enabled;
      dbObjectsList.Enabled = enabled;
      btnRefresh.Enabled = enabled;
      btnSelectAll.Enabled = enabled;
      btnUnSelect.Enabled = enabled;
      txtFileName.Enabled = enabled;
      btnSaveFile.Enabled = enabled;
      btnAdvanced.Enabled = enabled;
      no_data.Enabled = enabled;
      single_transaction.Enabled = enabled;
      routines.Enabled = enabled;
      btnSaveSettings.Enabled = enabled;
      btnLoadSettingsFile.Enabled = enabled;
      chkAlwaysCreateNewFile.Enabled = enabled;
    }

    private void LockUI()
    {
      _cursor = Cursor;
      Cursor = Cursors.WaitCursor;
      EnableControls(false);
      btnExport.Text = CANCEL;
    }

    private void UnlockUI()
    {
      if (_worker != null && _worker.IsBusy)
      {
        return;
      }

      Cursor = _cursor;
      EnableControls(true);
      btnExport.Text = START_EXPORT;
    }

    private void btnExport_Click(object sender, EventArgs e)
    {
      if (btnExport.Text != START_EXPORT && _mysqlDbExport != null)
      {
        _actionCancelled = true;
        CancelExport();
        return;
      }

      string mysqlFilePath = txtFileName.Text.Trim();
      _actionCancelled = false;
      try
      {
        if (Dictionary.Count == 0)
        {
          MessageBox.Show(Resources.DbExportPanel_NoDbSelectedError, Resources.MySqlDataProviderPackage_Information, MessageBoxButtons.OK, MessageBoxIcon.Information);
          return;
        }

        LockUI();
        if (string.IsNullOrEmpty(mysqlFilePath))
        {
          MessageBox.Show(Resources.DbExportPathNotProvided, Resources.MessageBoxErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
          txtFileName.Focus();
          return;
        }

        if (string.IsNullOrEmpty(Path.GetDirectoryName(mysqlFilePath)))
        {
          mysqlFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\" + Path.GetFileNameWithoutExtension(mysqlFilePath);
        }
        else
        {
          if (CheckPathIsValid(mysqlFilePath))
          {
            MessageBox.Show(Resources.PathNotValid, Resources.MessageBoxErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
          }
          else
          {
            mysqlFilePath = Path.GetFullPath(Path.GetDirectoryName(mysqlFilePath)) + @"\" + Path.GetFileNameWithoutExtension(mysqlFilePath);
          }
        }

        mysqlFilePath += ".mysql";

        // pull last version of db objects
        if (!string.IsNullOrEmpty(_ownerSchema))
          PullObjectListFromTree(_ownerSchema);

        var maxAllowedPacket = 0;
        bool overWriteExistingFile = chkAlwaysCreateNewFile.Checked;

        if (!int.TryParse(max_allowed_packet.Text, out maxAllowedPacket))
        {
          MessageBox.Show(Resources.InvalidMaxAllowedPacketValue, Resources.MessageBoxErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
          return;
        }

        BndOptions.default_character_set = default_character_set.Text.Trim();
        BndOptions.max_allowed_packet = maxAllowedPacket;
        try
        {
          if (overWriteExistingFile && File.Exists(mysqlFilePath))
          {
            File.Delete(mysqlFilePath);
            overWriteExistingFile = false;
          }
        }
        catch
        {
          MessageBox.Show(Resources.DbExportPanel_ExportFileCreationError, Resources.MessageBoxErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
          return;
        }

        var files = new List<string>();
        DoWorkEventHandler doWorker = (worker, doWorkerArgs) =>
        {
          foreach (var item in Dictionary)
          {
            var csb = !SelectedConnection.ConnectionString.ToLower().Contains("password")
              ? new MySqlConnectionStringBuilder(GetCompleteConnectionString(SelectedConnection.DisplayName, true))
              : new MySqlConnectionStringBuilder(SelectedConnection.ConnectionString);
            csb.Database = item.Key;
            BndOptions.database = item.Key;

            var allObjectsSelected = item.Value.All(dbObject => dbObject.Selected);
            _mysqlDbExport = null;
            string resultsTempFile = Path.GetTempFileName();
            if (allObjectsSelected)
            {
              _mysqlDbExport = new MySqlDbExport(BndOptions, resultsTempFile, new MySqlConnection(csb.ConnectionString), null, overWriteExistingFile);
            }
            else
            {
              if (item.Value != null)
              {
                var objects = (from s in item.Value
                                where s.Selected
                                select s.DbObjectName).ToList();
                _mysqlDbExport = new MySqlDbExport(BndOptions, resultsTempFile, new MySqlConnection(csb.ConnectionString), objects, overWriteExistingFile);
              }
            }

            if (_mysqlDbExport == null)
            {
              continue;
            }

            _mysqlDbExport.Export();
            files.Add(resultsTempFile);
            if (_mysqlDbExport.ErrorsOutput == null)
            {
              continue;
            }

            if (_generalPane != null)
            {
              // Wrap in Invoke API since now is a background thread
              Invoke((Action)(() =>
              {
                _generalPane.OutputString(Environment.NewLine + _mysqlDbExport.ErrorsOutput);
              }));
            }
          }

          //concentrate all files on the user's results file          
          using (var finalFile = new FileStream(mysqlFilePath, FileMode.Append))
          {
            foreach (var file in files)
            {
              using (var sourceStream = File.Open(file, FileMode.Open))
              {
                byte[] buffer = new byte[32768];
                while (true)
                {
                  int qty = sourceStream.Read(buffer, 0, (int)buffer.Length);
                  if (qty == 0)
                    break;
                  finalFile.Write(buffer, 0, qty);
                }
              }
              File.Delete(file);
            }
          }

          if (_generalPane == null)
          {
            return;
          }

          _generalPane.OutputString(Environment.NewLine + "File: " + mysqlFilePath);
          _generalPane.Activate();
        };
        if (_worker != null)
        {
          // detach previous handlers to avoid possible memory leaks
          _worker.DoWork -= doWorker;
          _worker.RunWorkerCompleted -= _worker_RunWorkerCompleted;
          _worker.Dispose();
        }
        _worker = new BackgroundWorker();
        _worker.WorkerSupportsCancellation = true;
        _worker.DoWork += doWorker;
        _worker.RunWorkerCompleted += _worker_RunWorkerCompleted;
        _worker.RunWorkerAsync();
      }
      finally
      {
        UnlockUI();
      }
    }

    private void btnSaveFile_Click(object sender, EventArgs e)
    {
      SaveFileDialog saveFileDlg = new SaveFileDialog();
      saveFileDlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
      saveFileDlg.Filter = @"(*.mysql)|*.mysql";
      saveFileDlg.FilterIndex = 1;
      saveFileDlg.FileName = txtFileName.Text;
      saveFileDlg.ValidateNames = true;
      saveFileDlg.SupportMultiDottedExtensions = false;
      if (saveFileDlg.ShowDialog() == DialogResult.OK)
      {
        txtFileName.Text = saveFileDlg.FileName;
      }
    }

    private bool CheckPathIsValid(string path)
    {
      if (path.Trim() == string.Empty)
      {
        return false;
      }

      string pathname;
      string filename;

      try
      {
        pathname = Path.GetPathRoot(path);
        filename = Path.GetFileName(path);
      }
      catch (ArgumentException)
      {
        return false;
      }

      if (filename.Trim() == string.Empty)
        return false;

      if (Directory.Exists(pathname))
        return false;

      return true;
    }

    private BindingList<DbSelectedObjects> GetDbObjects(string currentSchema)
    {
      string connectionString;
      BindingList<DbSelectedObjects> databaseObjects = new BindingList<DbSelectedObjects>();
      connectionString = SelectedConnection != null
        ? GetCompleteConnectionString(SelectedConnection.DisplayName, true)
        : Connections[0].ConnectionString;

      if (string.IsNullOrEmpty(connectionString))
      {
        return null;
      }

      var csb = new MySqlConnectionStringBuilder(connectionString);
      if (!csb.Database.Equals(currentSchema, StringComparison.InvariantCultureIgnoreCase))
      {
        csb.Database = currentSchema;
        connectionString = csb.ConnectionString;
      }

      SelectObjects.GetTables(new MySqlConnection(connectionString), null, null, false).
        ForEach(t => { databaseObjects.Add(new DbSelectedObjects(t, DbObjectKind.Table, false)); });
      SelectObjects.GetViews(new MySqlConnection(connectionString)).
        ForEach(v => { databaseObjects.Add(new DbSelectedObjects(v, DbObjectKind.View, false)); });

      return databaseObjects;
    }

    private string GetSelectedSchema(bool addToSelection)
    {
      int currentRow = dbSchemasList.CurrentRow != null ? dbSchemasList.CurrentRow.Index : -1;
      if (currentRow < 0)
      {
        return _ownerSchema = new MySqlConnectionStringBuilder(SelectedConnection.ConnectionString).Database;
      }

      dbSchemasList.Rows[currentRow].Cells[0].Value = addToSelection;
      return _ownerSchema = dbSchemasList.Rows[currentRow].Cells[1].Value.ToString();
    }

    private void btnSelectAll_Click(object sender, EventArgs e)
    {
      ChangeAllSelectedDbObjects(true);
    }

    private void btnUnSelect_Click(object sender, EventArgs e)
    {
      ChangeAllSelectedDbObjects(false);
    }

    private string GetTreeViewDb()
    {
      if (dbObjectsList.Nodes.Count == 0)
      {
        return string.Empty;
      }

      string db = dbObjectsList.Nodes[0].Text;
      return Schemas.Any(p => p.Name == db && p.Export) ? db : string.Empty;
    }

    private void ChangeAllSelectedDbObjects(bool selected)
    {
      // Unselect is done in tree itself, to avoid rebuilding it with the consequential collapsing.
      if (dbObjectsList.Nodes.Count == 0) return;
      TreeNode node = dbObjectsList.Nodes[0];
      node.Checked = selected;
      EnableDbObjectsListAfterCheck(false);
      try { UpdateCheckState(node, selected); }
      finally { EnableDbObjectsListAfterCheck(true); }
      string schema = GetSelectedSchema(true);
      PullObjectListFromTree(schema);
    }

    private void btnFilter_Click(object sender, EventArgs e)
    {
      _sourceSchemas.DataSource = from s in Schemas
                                 where s.Name.StartsWith(txtFilter.Text.Trim())
                                 select s;
      dbSchemasList.DataSource = _sourceSchemas;
      dbSchemasList.Update();
      FormatSchemasList();
    }

    private void btnReturn_Click(object sender, EventArgs e)
    {
      pnlAdvanced.Visible = !pnlAdvanced.Visible;
      pnlGeneral.Visible = pnlGeneral.Visible == false;
    }

    private void btnAdvanced_Click(object sender, EventArgs e)
    {
      pnlGeneral.Visible = !pnlGeneral.Visible;
      pnlAdvanced.Visible = pnlAdvanced.Visible == false;
    }

    private string GetCompleteConnectionString(string connectionDisplayName, bool persistSecurityInfo)
    {
      MySqlConnection connection;
      MySqlConnectionStringBuilder csb = null;
      IVsDataConnection s = null;
      Action a = () =>
      {
        s = (from cnn in _explorerMySqlConnections
             where cnn.DisplayName.Equals(connectionDisplayName, StringComparison.InvariantCultureIgnoreCase)
             select cnn.Connection).FirstOrDefault();
        if (s == null)
          return;
        connection = (MySqlConnection)s.GetLockedProviderObject();
        try
        {
          csb = (MySqlConnectionStringBuilder)connection.GetType().GetProperty("Settings", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(connection, null);
        }
        finally
        {
          s.UnlockProviderObject();
        }
      };
      if (InvokeRequired)
      {
        Invoke(a);
      }
      else
      {
        a();
      }

      if (!persistSecurityInfo || csb == null)
      {
        return s != null ? s.DisplayConnectionString : string.Empty;
      }

      csb.PersistSecurityInfo = true;
      return csb.ConnectionString;
    }

    private void btnRefresh_Click(object sender, EventArgs e)
    {
      LoadSchemasForSelectedConnection();
      if (string.IsNullOrEmpty(txtFilter.Text))
      {
        return;
      }

      _sourceSchemas.DataSource = Schemas;
      dbSchemasList.DataSource = _sourceSchemas;
      dbSchemasList.Update();
      dbSchemasList.Refresh();
      txtFilter.Text = string.Empty;
    }

    private bool _afterCheckRunning;

    private void EnableDbObjectsListAfterCheck(bool enable)
    {
      _afterCheckRunning = !enable;
    }

    private void dbObjectsList_AfterCheck(object sender, TreeViewEventArgs e)
    {
      if (_afterCheckRunning) return;
      EnableDbObjectsListAfterCheck(false);
      try
      {
        UpdateCheckState(e.Node, e.Node.Checked);
        // If marked, mark parents all the way up
        if (!e.Node.Checked)
        {
          return;
        }

        TreeNode parent = e.Node.Parent;
        while (parent != null)
        {
          parent.Checked = true;
          parent = parent.Parent;
        }
      }
      finally
      {
        if (_afterCheckRunning)
        {
          EnableDbObjectsListAfterCheck(true);
          // Propagate results into schema list
          if (e.Node.Checked)
          {
            string schemaName = dbObjectsList.Nodes[0].Text;
            foreach (Schema t in Schemas)
            {
              if (t.Name != schemaName)
              {
                continue;
              }

              t.CheckSchema(true);
              dbSchemasList.Refresh();
            }
          }
        }
      }
    }

    private void UpdateCheckState(TreeNode node, bool check)
    {
      for (int i = 0; i < node.Nodes.Count; i++)
      {
        TreeNode n = node.Nodes[i];
        n.Checked = check;
        UpdateCheckState(n, check);
      }
    }

    private void PullObjectListFromTree(string schema)
    {
      BindingList<DbSelectedObjects> dbList = new BindingList<DbSelectedObjects>();
      TreeNodeCollection nodes = dbObjectsList.Nodes;
      if (nodes.Count != 0 && nodes[0].Nodes.Count != 0)
      {
        var tnParent = nodes[0];
        for (int j = 0; j < tnParent.Nodes.Count; j++)
        {
          TreeNode tnParent2 = tnParent.Nodes[j];
          for (int i = 0; i < tnParent2.Nodes.Count; i++)
          {
            TreeNode node = tnParent2.Nodes[i];
            DbObjectKind kind = (j == 0) ? DbObjectKind.Table : DbObjectKind.View;
            dbList.Add(new DbSelectedObjects(node.Text, kind, node.Checked));
          }
        }
      }

      DbObjects = dbList;
      //if (dictionary.ContainsKey(schema))
      Dictionary[schema] = DbObjects;

    }

    private void BindDbObjectsToTree(BindingList<DbSelectedObjects> data, string schema)
    {
      dbObjectsList.BeginUpdate();
      try
      {
        // Optmization, temporarily disable recursive checks
        EnableDbObjectsListAfterCheck(false);
        dbObjectsList.Nodes.Clear();
        var root = dbObjectsList.Nodes.Add(schema);
        TreeNode tnTables = root.Nodes.Add("Tables");
        TreeNode tnViews = root.Nodes.Add("Views");
        foreach (DbSelectedObjects t in data)
        {
          TreeNode node = null;
          DbSelectedObjects dbo = t;
          switch (dbo.Kind)
          {
            case DbObjectKind.Table:
              node = tnTables.Nodes.Add(dbo.DbObjectName);
              break;
            case DbObjectKind.View:
              node = tnViews.Nodes.Add(dbo.DbObjectName);
              break;
          }

          if (node != null)
          {
            node.Checked = dbo.Selected;
          }
        }

        dbObjectsList.ExpandAll();
      }
      finally
      {
        EnableDbObjectsListAfterCheck(true);
        dbObjectsList.EndUpdate();
      }
    }

    private void dbObjectsList_DoubleClick(object sender, EventArgs e)
    {
      // Inhibite normal double click behavior
    }

    private void CancelExport()
    {

      try
      {
        if (!_worker.IsBusy)
        {
          return;
        }
      }
      catch
      {
        return;
      }

      _worker.CancelAsync();
      _mysqlDbExport.CancelExport();
      UnlockUI();
    }

    private void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      UnlockUI();
      if (e.Error != null)
      {
        Invoke((Action)(() =>
        {
          MessageBox.Show(
            string.Format("The following error ocurred while exporting: {0}", e.Error.Message),
            Resources.MessageBoxErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }));
      }
      else
      {
        if (_actionCancelled)
          _generalPane.OutputString(Environment.NewLine + "Data Export cancelled by the user.");
        else
          _generalPane.OutputString(Environment.NewLine + string.Format(Resources.MySqlDumpSummary, Dictionary.Count()));

        _generalPane.Activate();
      }
    }


    private void btnLoadSettingsFile_Click(object sender, EventArgs e)
    {
      Dictionary = new Dictionary<string, BindingList<DbSelectedObjects>>();
      using (var openSettingsFileDlg = new OpenFileDialog())
      {
        openSettingsFileDlg.Filter = @"(*.dumps)|*.dumps";
        openSettingsFileDlg.FilterIndex = 1;
        openSettingsFileDlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        openSettingsFileDlg.RestoreDirectory = true;
        if (openSettingsFileDlg.ShowDialog() != DialogResult.OK)
        {
          return;
        }

        var settingsFile = openSettingsFileDlg.FileName;
        if (File.Exists(settingsFile))
        {
          var settings = MySqlDbExportSaveOptions.LoadSettingsFile(settingsFile);
          if (settings == null)
          {
            return;
          }

          //TODO create the connection if it not exists 
          _fileSavedSettingsName = settingsFile;
          string displayConnectionName = (from cnn in _explorerMySqlConnections
            where cnn.Connection.DisplayConnectionString.Contains(settings.Connection)
            select cnn.DisplayName).FirstOrDefault();

          if (displayConnectionName != null)
          {
            var completeConnectionString = GetCompleteConnectionString(displayConnectionName, true);
            if (string.IsNullOrEmpty(completeConnectionString))
            {
              MessageBox.Show(Resources.DbExportPanel_ConnStringNotCorrectlySetError,
                Resources.MessageBoxErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
              return;
            }

            SelectedConnection = new MySqlServerExplorerConnection
            {
              DisplayName = displayConnectionName,
              ConnectionString = completeConnectionString
            };
            LoadSchemasForSelectedConnection();
            // load the path of dump file
            txtFileName.Text = settings.PathToMySqlFile;
            var listObjectsInSettings = settings.Dictionary.ToList();
            var listOfSchemas = from t in listObjectsInSettings
              group t by t.DatabaseName
              into d
              select d;

            foreach (var schema in listOfSchemas)
            {
              var selected = Schemas.Single(t => t.Name.Equals(schema.Key));
              if (selected != null)
              {
                selected.CheckSchema(true);
                var listDbObjectsSelected = from o in listObjectsInSettings
                  where o.DatabaseName == schema.Key
                  select o;
                DbObjects = new BindingList<DbSelectedObjects>();
                foreach (var dbObject in listDbObjectsSelected)
                {
                  DbObjects.Add(new DbSelectedObjects(dbObject.ObjectName, dbObject.ObjectType, dbObject.Selected));
                }

                if (Dictionary.ContainsKey(schema.Key))
                  Dictionary.Remove(schema.Key);

                Dictionary.Add(schema.Key, DbObjects);
              }
            }

            dbSchemasList.Refresh();
            BndOptions = settings.DumpOptions;
            max_allowed_packet.Text = BndOptions.max_allowed_packet.ToString();
            mySqlDbExportOptionsBindingSource.RemoveAt(0);
            mySqlDbExportOptionsBindingSource.Add(BndOptions);
            mySqlDbExportOptionsBindingSource.ResetBindings(true);

            Application.DoEvents();
            _windowHandler.Caption = Path.GetFileName(_fileSavedSettingsName);
            MessageBox.Show(Resources.DbExportPanel_SettingsLoadSuccess, Resources.MySqlDataProviderPackage_Information, MessageBoxButtons.OK,
              MessageBoxIcon.Information);
          }
          else
            MessageBox.Show(Resources.DbExportPanel_ConnectionNotFoundError, Resources.MessageBoxErrorTitle, MessageBoxButtons.OK,
              MessageBoxIcon.Error);
        }
        else
          MessageBox.Show(Resources.DbExportPanel_FileNotFoundError);
      }
    }

    private void btnSaveSettings_Click(object sender, EventArgs e)
    {
      string prevSchema = GetTreeViewDb();
      if (!string.IsNullOrEmpty(prevSchema))
        PullObjectListFromTree(prevSchema);
      SaveSettings(false);
    }

    private void txtFilter_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)
      {
        if (string.IsNullOrEmpty(txtFilter.Text) && (dbSchemasList.RowCount < Schemas.Count))
        {
          _sourceSchemas.DataSource = Schemas;
          dbSchemasList.DataSource = _sourceSchemas;
          dbSchemasList.Update();
          FormatSchemasList();
          return;
        }
      }
      if (e.KeyCode == Keys.Enter)
      {
        btnFilter_Click(sender, null);
      }
    }

    private void txtFileName_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        btnSaveFile_Click(sender, null);
      }
    }

    public void SaveSettings(bool useDefaults)
    {
      DialogResult result = DialogResult.OK;
      using (var saveSettingsFileDlg = new SaveFileDialog())
      {
        saveSettingsFileDlg.InitialDirectory = _fileSavedSettingsName != string.Empty
          ? Path.GetFullPath(_fileSavedSettingsName)
          : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        saveSettingsFileDlg.Filter = @"(*.dumps)|*.dumps";
        saveSettingsFileDlg.FilterIndex = 1;
        saveSettingsFileDlg.FileName = _windowHandler.Caption.Replace("*", "");

        if (!useDefaults)
        {
          result = saveSettingsFileDlg.ShowDialog();
        }

        if (result != DialogResult.OK)
        {
          return;
        }

        string connectionStringInUse = string.Empty;
        if (SelectedConnection != null)
        {
          connectionStringInUse = GetCompleteConnectionString(SelectedConnection.DisplayName, false);
        }
        else if (Connections.Count > 0)
        {
          connectionStringInUse = GetCompleteConnectionString(Connections[0].ConnectionString, false);
        }

        if (string.IsNullOrEmpty(connectionStringInUse))
        {
          MessageBox.Show(Resources.DbExportPanel_NoConnectionSelected, Resources.MessageBoxErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
          return;
        }

        var saveToFile = new MySqlDbExportSaveOptions(BndOptions, txtFileName.Text, Dictionary, connectionStringInUse);
        try
        {
          string completePath = string.IsNullOrEmpty(Path.GetDirectoryName(saveSettingsFileDlg.FileName))
            ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            : Path.GetDirectoryName(saveSettingsFileDlg.FileName);
          Cursor = Cursors.WaitCursor;
          saveToFile.WriteSettingsFile(completePath, Path.GetFileNameWithoutExtension(saveSettingsFileDlg.FileName));
          Cursor = Cursors.Default;
          _fileSavedSettingsName = Path.Combine(completePath, saveSettingsFileDlg.FileName);
          MessageBox.Show(Resources.DbExportPanel_SaveSettingSuccess, Resources.MySqlDataProviderPackage_Information, MessageBoxButtons.OK, MessageBoxIcon.Information);
          _windowHandler.Caption = Path.GetFileName(saveSettingsFileDlg.FileName);
        }
        catch (Exception ex)
        {
          Cursor = Cursors.Default;
          MessageBox.Show(Resources.DbExportPanel_SaveSettingsError + ex.Message, Resources.MessageBoxErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
      }
    }

    private void dbObjectsList_Leave(object sender, EventArgs e)
    {
      string db = GetTreeViewDb();
      if (!string.IsNullOrEmpty(db))
      {
        PullObjectListFromTree(db);
      }
    }
  }

  public class Schema : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    public bool Export { get; private set; }

    public string Name { get; }

    public Schema(bool export, string name)
    {
      Export = export;
      Name = name;
    }

    private void NotifyPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    public void CheckSchema(bool select)
    {
      Export = select;
      NotifyPropertyChanged("Export");
    }
  }

  public enum DbObjectKind
  {
    Table = 1,
    View = 2
  }

  public class DbSelectedObjects : INotifyPropertyChanged
  {
    bool _selected;
    public event PropertyChangedEventHandler PropertyChanged;

    [DisplayName(@"Export")]
    public bool Selected
    {
      get
      {
        return _selected;
      }
      set
      {
        _selected = value;
        NotifyPropertyChanged("Selected");
      }
    }

    [DisplayName(@"Schema Object")]
    public string DbObjectName { get; }

    [DisplayName(@"Kind")]
    public DbObjectKind Kind { get; }

    private bool _notifiedProperty;

    private void NotifyPropertyChanged(string propertyName)
    {
      // Avoid reentrancy
      if (_notifiedProperty) return;
      _notifiedProperty = true;
      try
      {
        if (PropertyChanged != null)
        {
          PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
      }
      finally
      {
        if (_notifiedProperty) _notifiedProperty = false;
      }
    }

    public DbSelectedObjects(string objectName, DbObjectKind kind, bool selected)
    {
      DbObjectName = objectName.ToLowerInvariant();
      _selected = selected;
      Kind = kind;
    }
  }

  public class MySqlServerExplorerConnection
  {
    public string DisplayName { get; set; }

    public string ConnectionString { get; set; }

    public MySqlServerExplorerConnection()
    {
      DisplayName = string.Empty;
      ConnectionString = string.Empty;
    }
  }

}