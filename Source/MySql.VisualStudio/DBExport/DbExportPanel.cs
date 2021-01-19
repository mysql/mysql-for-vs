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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Microsoft.VisualStudio.Data.Services;
using MySql.Data.VisualStudio.Properties;
using MySql.Data.VisualStudio.SchemaComparer;
using MySql.Data.VisualStudio.DbObjects;
using System.Runtime.CompilerServices;
using System.IO;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio;
using System.Runtime.InteropServices;
using EnvDTE;
using MySql.Utility.Classes.Logging;

namespace MySql.Data.VisualStudio.DBExport
{
    public partial class dbExportPanel : UserControl
    {
      private IVsOutputWindowPane _generalPane;
      private List<IVsDataExplorerConnection> _explorerMySqlConnections;
      private string _ownerSchema { get; set; }
      private BackgroundWorker _worker;
      private Cursor _cursor;
      private MySqlDbExport _mysqlDbExport;
      private ToolWindowPane _windowHandler;
      private bool _actionCancelled;
      private string _fileSavedSettingsName;
      
      internal MySqlDbExportOptions bndOptions;
      internal List<Schema> schemas = new List<Schema>();      
      internal BindingList<DbSelectedObjects> dbObjects = new BindingList<DbSelectedObjects>();      
      internal Dictionary<string, BindingList<DbSelectedObjects>> dictionary = 
        new Dictionary<string, BindingList<DbSelectedObjects>>();

      public MySqlServerExplorerConnection SelectedConnection { get; set; }
      public List<MySqlServerExplorerConnection> Connections { get; set; }

      BindingSource sourceSchemas = new BindingSource();
      BindingSource sourceDbObjects = new BindingSource();      

      public dbExportPanel()        
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
          bndOptions = new MySqlDbExportOptions();
          max_allowed_packet.Text = bndOptions.max_allowed_packet.ToString();
          mySqlDbExportOptionsBindingSource.Add(bndOptions);          

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
          sourceSchemas.ListChanged += sourceSchemas_ListChanged;       
          _fileSavedSettingsName = string.Empty;
        
      }     

      void sourceSchemas_ListChanged(object sender, ListChangedEventArgs e)
      {
        if (_windowHandler == null) return;
       

        if (_windowHandler.Caption.Equals(Properties.Resources.DbExportToolCaptionFrame))
        {          
          _windowHandler.Caption = String.Format("DBExportDoc_{0:MMddyyyyhmmss}.dumps", DateTime.Now);
        }
        if (_fileSavedSettingsName != String.Empty)
        {
          _windowHandler.Caption = Path.GetFileName(_fileSavedSettingsName);          
        }
        _windowHandler.Caption = !_windowHandler.Caption.Contains("*") ? _windowHandler.Caption += "*" : _windowHandler.Caption;        
      }
      
      void dbSchemasList_RowEnter(object sender, DataGridViewCellEventArgs e)
      {
        if (e.ColumnIndex == 1)
        {
          string schemaSelected = (string)dbSchemasList.Rows[e.RowIndex].Cells[1].Value;
          string prevSchema = GetTreeViewDb();
          if ( prevSchema != schemaSelected )
          {
            if (!string.IsNullOrEmpty( prevSchema ))
              PullObjectListFromTree(prevSchema);
            LoadDbObjects(schemaSelected);
          }
        }
      }

      void dbSchemasList_RowLeave(object sender, DataGridViewCellEventArgs e)
      {
       
        if (String.IsNullOrEmpty(_ownerSchema))
          return;

        try 
	       {
           if (dbSchemasList.CurrentCell.ColumnIndex == 0 && (bool)dbSchemasList.CurrentCell.Value)
           {
             if (dictionary.ContainsKey(_ownerSchema))
               dictionary.Remove(_ownerSchema);

             var dictionaryValue = new BindingList<DbSelectedObjects>(dbObjects.ToList());
             dictionary.Add(_ownerSchema, dictionaryValue);
           }
	       }
	      catch
	      {}        
      }

      void dbSchemasList_CellValueChanged(object sender, ListChangedEventArgs e)
      {
        if (dbSchemasList.CurrentCell == null || dbSchemasList.CurrentCell.ColumnIndex != 0)
          return;
               
        int currentRow = dbSchemasList.CurrentRow.Index;
        int currentColumn = dbSchemasList.CurrentCell.ColumnIndex;
        var currentSchema = string.Empty;

        if (dbSchemasList.Rows[currentRow].Cells.Count <= 1)
          return;

        if (currentRow >= 0)
        {       
          currentSchema = dbSchemasList.Rows[currentRow].Cells[1].Value.ToString();
        }
        else
        {
          currentSchema = (new MySqlConnectionStringBuilder(SelectedConnection.ConnectionString)).Database;
        }

        if (_ownerSchema.Equals(currentSchema, StringComparison.InvariantCultureIgnoreCase) && dbObjects != null)
        {
          //update to UI
          foreach (var item in dbObjects)
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

          if (dictionary.ContainsKey(currentSchema))
            dictionary.Remove(currentSchema);

          if (currentRow >= 0 && (bool)dbSchemasList.Rows[currentRow].Cells[currentColumn].Value)
            dictionary.Add(currentSchema, databaseObjects);
      }

      void cmbConnections_SelectionChangeCommitted(object sender, EventArgs e)
      {
        if ((cmbConnections.SelectedItem as MySqlServerExplorerConnection) == null) return;

        SelectedConnection = new MySqlServerExplorerConnection();
        SelectedConnection.ConnectionString = ((MySqlServerExplorerConnection)cmbConnections.SelectedItem).ConnectionString;
        SelectedConnection.DisplayName = ((MySqlServerExplorerConnection)cmbConnections.SelectedItem).DisplayName;
        LoadSchemasForSelectedConnection();
        dictionary.Clear();
        dbObjects = null;
        dbObjectsList.Nodes.Clear();
        dbObjectsList.Refresh();
      }

      void dbSchemasList_CellClick(object sender, DataGridViewCellEventArgs e)
      {
        if (e.RowIndex == -1) return;
        if (String.IsNullOrEmpty(dbSchemasList.Rows[e.RowIndex].Cells[1].Value as string))
          return;

        if (e.ColumnIndex == 0)
        {
          var selected = schemas.Single(t => t.Name.Equals((string)dbSchemasList.Rows[e.RowIndex].Cells[1].Value, 
            StringComparison.InvariantCultureIgnoreCase));
          selected.CheckSchema(!selected.Export);
          if (!selected.Export)
          {
            LoadDbObjects(selected.Name);
            ChangeAllSelectedDbObjects(false);
            dictionary.Remove(selected.Name);
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
        _cursor = this.Cursor;
        this.Cursor = Cursors.WaitCursor;
        
        Application.DoEvents();

        IVsDataExplorerConnectionManager connectionManager = GetService(typeof(IVsDataExplorerConnectionManager)) as IVsDataExplorerConnectionManager;
        if (connectionManager == null) return;
        System.Collections.Generic.IDictionary<string, IVsDataExplorerConnection> connections = connectionManager.Connections;

        Connections = new List<MySqlServerExplorerConnection>();

        _explorerMySqlConnections = new List<IVsDataExplorerConnection>();

        foreach (var connection in connections)
        {
          if (Guids.Provider.Equals(connection.Value.Provider))
          {
            Connections.Add(new MySqlServerExplorerConnection { DisplayName = connection.Value.DisplayName, ConnectionString = connection.Value.Connection.DisplayConnectionString });
            _explorerMySqlConnections.Add(connection.Value);
          }
        }

        this.Cursor = _cursor;

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
        var connectionString = String.Empty;        

        _ownerSchema = databaseName;

        dbObjects = null;

        if (dictionary.ContainsKey(databaseName))
          dictionary.TryGetValue(databaseName, out dbObjects);
        else
        {
          dbObjects = GetDbObjects(_ownerSchema);
        }       

        BindDbObjectsToTree(dbObjects, databaseName);
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
        schemas = new List<Schema>();

        List<string> schemaNames = null;

        if (SelectedConnection != null)
        {
          schemaNames = SelectObjects.GetSchemas(new MySqlConnectionStringBuilder(GetCompleteConnectionString(SelectedConnection.DisplayName, true)));
        }
        else if (Connections.Count > 0)
          schemaNames = SelectObjects.GetSchemas(new MySqlConnectionStringBuilder(GetCompleteConnectionString(Connections[0].ConnectionString, true)));

        if (schemaNames == null)
        {
           Logger.LogError("Cannot retrieve the list of schemas of the selected connection. Verify parameter's connection.", true);
           sourceSchemas.DataSource = null;
           dbSchemasList.Refresh();
           return;
        }

        schemaNames.ForEach(t =>
        {
          if (t.StartsWith(".")) return;
          if (dictionary.ContainsKey(t))
            schemas.Add(new Schema(true, t));
          else
            schemas.Add(new Schema(false, t));
        });
        
        sourceSchemas.DataSource = schemas;        
        sourceSchemas.ListChanged += new ListChangedEventHandler(dbSchemasList_CellValueChanged);
        dbSchemasList.DataSource = sourceSchemas;
        FormatSchemasList();       
        
      }

      public void FormatSchemasList()
      {
        if (dbSchemasList.Rows.Count <= 1 && dbSchemasList.Columns.Count == 1)
        {
          sourceSchemas.DataSource = new BindingList<Schema>();
          dbSchemasList.DataSource = sourceSchemas;
          dbSchemasList.Update();
        }
          dbSchemasList.Columns[0].HeaderText = "Export";
          dbSchemasList.Columns[0].Width = 45;

          dbSchemasList.Columns[1].HeaderText = "Schema";
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
            Logger.LogError("Error: Cannot find any MySQL connection. Please review your settings.", true);
            return;
          }
          cmbConnections.SelectedValue = selected.ConnectionString;
          SelectedConnection = selected;
          LoadSchemasForSelectedConnection();         
          _windowHandler = windowHandler;
        }
        catch (Exception)
        {
          Logger.LogError(Properties.Resources.UnableToRetrieveDatabaseList, true);
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
        _cursor = this.Cursor;
        this.Cursor = Cursors.WaitCursor;
        EnableControls(false);
        btnExport.Text = "Cancel";
      }

      private void UnlockUI()
      {
        bool workerStarted = false;
        if ( _worker != null && _worker.IsBusy )
        {
          workerStarted = true;
        }
        if (!workerStarted)
        {
          this.Cursor = _cursor;
          EnableControls(true);
          btnExport.Text = "Start Export";
        }
      }

      private void btnExport_Click(object sender, EventArgs e)
      {
        if (btnExport.Text != "Start Export" && _mysqlDbExport != null)
        {
          _actionCancelled = true;
          CancelExport();          
          return;
        }

        string mysqlFilePath = txtFileName.Text.Trim();
        _actionCancelled = false;
        try
        {
          if (dictionary.Count == 0)
          {
            Logger.LogInformation("No database or objects are selected", true);
            return;
          }

          LockUI();
          if (String.IsNullOrEmpty(mysqlFilePath))
          {
            Logger.LogError(Properties.Resources.DbExportPathNotProvided, true);
            txtFileName.Focus();
            return;
          }

          if (String.IsNullOrEmpty(Path.GetDirectoryName(mysqlFilePath)))
          {
            mysqlFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\" + Path.GetFileNameWithoutExtension(mysqlFilePath);
          }
          else
          {
            if (CheckPathIsValid(mysqlFilePath))
            {
              Logger.LogError(Properties.Resources.PathNotValid, true);
              return;
            }
            else
            {
              mysqlFilePath = Path.GetFullPath(Path.GetDirectoryName(mysqlFilePath)) + @"\" + Path.GetFileNameWithoutExtension(mysqlFilePath);
            }
          }

          mysqlFilePath += ".mysql";

          var maxAllowedPacket = 0;
          bool overWriteExistingFile = chkAlwaysCreateNewFile.Checked;

          if (!int.TryParse(max_allowed_packet.Text, out maxAllowedPacket))
          {
            Logger.LogError(Properties.Resources.InvalidMaxAllowedPacketValue, true);
            return;
          }

          bndOptions.default_character_set = default_character_set.Text.Trim();
          bndOptions.max_allowed_packet = maxAllowedPacket;
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
            Logger.LogError("An error occured when creating a new file for the Export operation. File is locked.", true);
            return;
          }

          List<String> files = new List<string>();

          DoWorkEventHandler doWorker = (worker, doWorkerArgs) =>
          {
            foreach (var item in dictionary)
            {
              MySqlConnectionStringBuilder csb;
              if (!SelectedConnection.ConnectionString.ToLower().Contains("password"))
              {
                csb = new MySqlConnectionStringBuilder(GetCompleteConnectionString(SelectedConnection.DisplayName, true));
              }
              else
                csb = new MySqlConnectionStringBuilder(SelectedConnection.ConnectionString);

              csb.Database = item.Key;
              bndOptions.database = item.Key;

              var allObjectsSelected = true;

              foreach (var dbObject in item.Value)
              {
                if (dbObject.Selected == false)
                {
                  allObjectsSelected = false;
                  // exit as soon as possible
                  break;
                }
              }

              _mysqlDbExport = null;

              string resultsTempFile = Path.GetTempFileName(); 
              if (allObjectsSelected)
                _mysqlDbExport = new MySqlDbExport(bndOptions, resultsTempFile, new MySqlConnection(csb.ConnectionString), null, overWriteExistingFile);
              else
              {
                if (item.Value != null)
                {
                  List<String> objects = (from s in item.Value
                                          where s.Selected
                                          select s.DbObjectName).ToList();

                  _mysqlDbExport = new MySqlDbExport(bndOptions, resultsTempFile, new MySqlConnection(csb.ConnectionString), objects, overWriteExistingFile);
                }
              }

              if (_mysqlDbExport != null)
              {
                _mysqlDbExport.Export();
                files.Add(resultsTempFile);
                if (_mysqlDbExport.ErrorsOutput != null)
                {
                  if (_generalPane != null)
                  {
                    // Wrap in Invoke API since now is a background thread
                    this.Invoke((Action)(() =>
                    {
                      _generalPane.OutputString(Environment.NewLine + _mysqlDbExport.ErrorsOutput.ToString());                     
                    }));                    
                  }
                }                  
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
            
            if (_generalPane != null)
            {
              _generalPane.OutputString(Environment.NewLine + "File: " + mysqlFilePath);
              _generalPane.Activate();
            }
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
          _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_worker_RunWorkerCompleted);
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
        saveFileDlg.Filter = "(*.mysql)|*.mysql" ; 
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
        string connectionString = String.Empty;
        BindingList<DbSelectedObjects> databaseObjects = new BindingList<DbSelectedObjects>();

        if (SelectedConnection != null)
          connectionString = GetCompleteConnectionString(SelectedConnection.DisplayName, true);
        else
          connectionString = Connections[0].ConnectionString;

        if (String.IsNullOrEmpty(connectionString))
          return null;

        MySqlConnectionStringBuilder csb = new MySqlConnectionStringBuilder(connectionString);

        if (!csb.Database.Equals(currentSchema, StringComparison.InvariantCultureIgnoreCase))
        {
          csb.Database = currentSchema;
          connectionString = csb.ConnectionString;
        }
        SelectObjects.GetTables(new MySqlConnection(connectionString), null, null, false).
          ForEach(t => { databaseObjects.Add(new DbSelectedObjects(t, DbObjectKind.Table, false )); });
        SelectObjects.GetViews(new MySqlConnection(connectionString)).
          ForEach(v => { databaseObjects.Add(new DbSelectedObjects(v, DbObjectKind.View, false )); });

        return databaseObjects;            
      }

      private string GetSelectedSchema(bool addToSelection)
      {
        int currentRow = dbSchemasList.CurrentRow.Index;

        if (currentRow >= 0)
        {
          dbSchemasList.Rows[currentRow].Cells[0].Value = addToSelection;
          return _ownerSchema = dbSchemasList.Rows[currentRow].Cells[1].Value.ToString();
        }
        else
        {
          return _ownerSchema = ( new MySqlConnectionStringBuilder(SelectedConnection.ConnectionString)).Database;
        }
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
        if (dbObjectsList.Nodes.Count != 0)
        {
          string db = dbObjectsList.Nodes[0].Text;
          if (schemas.Where(p => p.Name == db && p.Export ).Count() == 0)
            return "";
          else
            return db;
        }
        else
          return "";
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
        PullObjectListFromTree( schema );
      }

      private void btnFilter_Click(object sender, EventArgs e)
      {
        sourceSchemas.DataSource = from s in schemas
                                   where s.Name.StartsWith(txtFilter.Text.Trim())
                                   select s;
        dbSchemasList.DataSource = sourceSchemas;                                                                             
        dbSchemasList.Update();
        FormatSchemasList();
      }

      private void btnReturn_Click(object sender, EventArgs e)
      {
        pnlAdvanced.Visible = this.pnlAdvanced.Visible ? false : true;
        pnlGeneral.Visible = this.pnlGeneral.Visible == false ? true : false;
      }

      private void btnAdvanced_Click(object sender, EventArgs e)
      {
        pnlGeneral.Visible = this.pnlGeneral.Visible ? false : true;
        pnlAdvanced.Visible = this.pnlAdvanced.Visible == false ? true : false;
      }
        
      private string GetCompleteConnectionString(string connectionDisplayName, bool persistSecurityInfo)
      {
        MySqlConnection connection = null;
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
        if (this.InvokeRequired)
        {
          this.Invoke(a);
        }
        else
        {
          a();
        }

        if (persistSecurityInfo && csb != null)
        {
          csb.PersistSecurityInfo = true;
          return csb.ConnectionString;
        }
        else
        { 
          if (s != null)  
           return s.DisplayConnectionString;
        }
        return string.Empty;
      }

      private void btnRefresh_Click(object sender, EventArgs e)
      {
        LoadSchemasForSelectedConnection();
        if (!String.IsNullOrEmpty(txtFilter.Text))
        {
          sourceSchemas.DataSource = schemas;
          dbSchemasList.DataSource = sourceSchemas;
          dbSchemasList.Update();
          dbSchemasList.Refresh();
          txtFilter.Text = string.Empty;
          return;
        }
      }

      private bool _afterCheckRunning = false;

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
          if (e.Node.Checked)
          { 
            TreeNode parent = e.Node.Parent;
            while( parent != null )
            {
              parent.Checked = true;
              parent = parent.Parent;
            }
          }
        }
        finally
        {
          if( _afterCheckRunning ) 
          {
            EnableDbObjectsListAfterCheck(true);
            // Propagate results into schema list
            if (e.Node.Checked)
            {
              string schemaName = dbObjectsList.Nodes[ 0 ].Text;              
              for (int i = 0; i < schemas.Count; i++)
              {
                if (schemas[i].Name == schemaName)
                {
                  schemas[i].CheckSchema(true);
                  dbSchemasList.Refresh();
                }
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

      private void PullObjectListFromTree( string schema )
      {
        BindingList<DbSelectedObjects> dbList = new BindingList<DbSelectedObjects>();
        TreeNode tnParent = null;
        TreeNodeCollection nodes = dbObjectsList.Nodes;
        
        if (nodes.Count != 0 && nodes[0].Nodes.Count != 0)
        {
          tnParent = nodes[0];
          for (int j = 0; j < tnParent.Nodes.Count; j++)
          {
            TreeNode tnParent2 = tnParent.Nodes[j];
            for (int i = 0; i < tnParent2.Nodes.Count; i++)
            {
              TreeNode node = tnParent2.Nodes[i];
              DbObjectKind kind = (j == 0) ? DbObjectKind.Table : DbObjectKind.View;
              dbList.Add(new DbSelectedObjects(node.Text, kind, node.Checked ) );
            }
          }
        }
        
        dbObjects = dbList;
        //if (dictionary.ContainsKey(schema))
          dictionary[schema] = dbObjects;

      }

      private void BindDbObjectsToTree(BindingList<DbSelectedObjects> data, string schema)
      {
        TreeNode root = null;
        dbObjectsList.BeginUpdate();
        try
        {
          // Optmization, temporarily disable recursive checks
          EnableDbObjectsListAfterCheck(false);
          dbObjectsList.Nodes.Clear();
          root = dbObjectsList.Nodes.Add(schema);
          TreeNode tnTables = root.Nodes.Add("Tables");
          TreeNode tnViews = root.Nodes.Add("Views");
          for (int i = 0; i < data.Count; i++)
          {
            TreeNode node = null;
            DbSelectedObjects dbo = data[i];
            if (dbo.Kind == DbObjectKind.Table)
            {
              node = tnTables.Nodes.Add(dbo.DbObjectName);
            }
            else if (dbo.Kind == DbObjectKind.View)
            {
              node = tnViews.Nodes.Add(dbo.DbObjectName);
            }
            node.Checked = dbo.Selected;
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
        return;
      }

      private void CancelExport()
      {

        try
        {
          if (!_worker.IsBusy)
            return;
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
          this.Invoke((Action)(() =>
          {
            Logger.LogError(string.Format("The following error ocurred while exporting: {0}", e.Error.Message), true);
          }));
        }
        else
        {
          if (_actionCancelled)
            _generalPane.OutputString(Environment.NewLine + string.Format("Data Export cancelled by the user."));
          else
            _generalPane.OutputString(Environment.NewLine + string.Format(Properties.Resources.MySqlDumpSummary, dictionary.Count()));

          _generalPane.Activate();
        }
      }

      private void btnLoadSettingsFile_Click(object sender, EventArgs e)
      {
        string settingsFile = string.Empty;
        
        dictionary = new Dictionary<string, BindingList<DbSelectedObjects>>();

        OpenFileDialog openSettingsFileDlg = new OpenFileDialog();
        
        openSettingsFileDlg.Filter = "(*.dumps)|*.dumps";        
        openSettingsFileDlg.FilterIndex = 1;
        openSettingsFileDlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        openSettingsFileDlg.RestoreDirectory = true;

        if (openSettingsFileDlg.ShowDialog() == DialogResult.OK)
        {
          settingsFile = openSettingsFileDlg.FileName;
          string completeConnectionString = String.Empty;

          if (File.Exists(settingsFile))
          {
            MySqlDbExportSaveOptions settings = MySqlDbExportSaveOptions.LoadSettingsFile(settingsFile);
            if (settings != null)
            {
              //TODO create the connection if it not exists 
              _fileSavedSettingsName = settingsFile;              
              string DisplayConnectionName = (from cnn in _explorerMySqlConnections
                                              where cnn.Connection.DisplayConnectionString.Contains(settings.Connection)
                                              select cnn.DisplayName).FirstOrDefault();

              if (DisplayConnectionName != null)
              {
                completeConnectionString = GetCompleteConnectionString(DisplayConnectionName, true);
                if (String.IsNullOrEmpty(completeConnectionString))
                {
                  Logger.LogError("The saved connection string was not correctly set. No Database objects are loaded", true);
                  return;
                }

                SelectedConnection = new MySqlServerExplorerConnection();
                SelectedConnection.DisplayName = DisplayConnectionName;
                SelectedConnection.ConnectionString = completeConnectionString;
                LoadSchemasForSelectedConnection();
                // load the path of dump file
                txtFileName.Text = settings.PathToMySqlFile;
                var listObjectsInSettings = settings.Dictionary.ToList();
                var listOfSchemas = from t in listObjectsInSettings
                                    group t by t.DatabaseName into d
                                    select d;

                foreach (var schema in listOfSchemas)
                {
                  var selected = schemas.Single(t => t.Name.Equals(schema.Key));
                  if (selected != null)
                  {
                    selected.CheckSchema(true);
                    var listDbObjectsSelected = from o in listObjectsInSettings
                                                where o.DatabaseName == schema.Key
                                                select o;
                    dbObjects = new BindingList<DbSelectedObjects>();
                    foreach (var dbObject in listDbObjectsSelected)
                    {
                      dbObjects.Add(new DbSelectedObjects(dbObject.ObjectName, dbObject.ObjectType, dbObject.Selected));
                    }
                    
                    if (dictionary.ContainsKey(schema.Key))
                      dictionary.Remove(schema.Key);

                    dictionary.Add(schema.Key, dbObjects);
                  }
                }
                dbSchemasList.Refresh();
                bndOptions = settings.DumpOptions;
                max_allowed_packet.Text = bndOptions.max_allowed_packet.ToString();                
                mySqlDbExportOptionsBindingSource.RemoveAt(0);
                mySqlDbExportOptionsBindingSource.Add(bndOptions);
                mySqlDbExportOptionsBindingSource.ResetBindings(true);
                
                Application.DoEvents();
                _windowHandler.Caption = Path.GetFileName(_fileSavedSettingsName);
                Logger.LogInformation("The saved settings were loaded correctly", true);
              }
              else
                Logger.LogError("Connection was not found on available connections", true);
            }
          }
          else
            Logger.LogError("File was not found. Please check the path.", true);
        }
      }

      private void btnSaveSettings_Click(object sender, EventArgs e)
      {
        string prevSchema = GetTreeViewDb();
        if (!string.IsNullOrEmpty(prevSchema))
          PullObjectListFromTree(prevSchema);
        SaveSettings(false);
      }

      void txtFilter_KeyDown(object sender, KeyEventArgs e)
      {
        if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)
        {
          if (String.IsNullOrEmpty(txtFilter.Text) && (dbSchemasList.RowCount < schemas.Count))
          {
            sourceSchemas.DataSource = schemas;
            dbSchemasList.DataSource = sourceSchemas;
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

      void txtFileName_KeyDown(object sender, KeyEventArgs e)
      {
        if (e.KeyCode == Keys.Enter)
        {
          btnSaveFile_Click(sender, null);
        }
      }

      public void SaveSettings(bool useDefaults)
      {
        string settingsFile = string.Empty;
        DialogResult result = DialogResult.OK;
        SaveFileDialog saveSettingsFileDlg = new SaveFileDialog();

        if (_fileSavedSettingsName != String.Empty)
        {
          saveSettingsFileDlg.InitialDirectory = Path.GetFullPath(_fileSavedSettingsName);
        }
        else
        {
          saveSettingsFileDlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }


        saveSettingsFileDlg.Filter = "(*.dumps)|*.dumps";
        saveSettingsFileDlg.FilterIndex = 1;
        saveSettingsFileDlg.FileName = _windowHandler.Caption.Replace("*", "");

        if (!useDefaults)                 
          result = saveSettingsFileDlg.ShowDialog();
        
        
        if (result == DialogResult.OK)
        {
          string connectionStringInUse = String.Empty;

          if (SelectedConnection != null)
          {
            connectionStringInUse = GetCompleteConnectionString(SelectedConnection.DisplayName, false);
          }
          else if (Connections.Count > 0)
            connectionStringInUse = GetCompleteConnectionString(Connections[0].ConnectionString, false);

          if (String.IsNullOrEmpty(connectionStringInUse))
          {
            Logger.LogError("No connection is selected. Please select one to continue.", true);
            return;
          }

          var saveToFile = new MySqlDbExportSaveOptions(bndOptions, txtFileName.Text, dictionary, connectionStringInUse);

          try
          {
            string completePath = String.IsNullOrEmpty(Path.GetDirectoryName(saveSettingsFileDlg.FileName)) ?
                                  Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) :
                                  Path.GetDirectoryName(saveSettingsFileDlg.FileName);
            this.Cursor = Cursors.WaitCursor;
            saveToFile.WriteSettingsFile(completePath, Path.GetFileNameWithoutExtension(saveSettingsFileDlg.FileName));
            this.Cursor = Cursors.Default;
            _fileSavedSettingsName = Path.Combine(completePath, saveSettingsFileDlg.FileName);
            Logger.LogInformation("All selected settings were saved correctly.", true);
            _windowHandler.Caption = Path.GetFileName(saveSettingsFileDlg.FileName);
          }
          catch (Exception ex)
          {
            this.Cursor = Cursors.Default;
            Logger.LogError($"An error occured when saving the settings file: {ex.Message}", true);
          }
        }      
      }

      private void dbObjectsList_Leave(object sender, EventArgs e)
      {
        string db = GetTreeViewDb();
        if (!string.IsNullOrEmpty(db))
          PullObjectListFromTree(db);
      }     

    }


    public class Schema : INotifyPropertyChanged
    {

      private bool _export { get; set; }
      private string _name { get; set; }

      public event PropertyChangedEventHandler PropertyChanged;

      public bool Export
      {
        get
        {
          return _export;
        }
      }

      public string Name
      {
        get
        {
          return _name;
        }
      }

      public Schema(bool export, string name)
      {
        _export = export;
        _name = name;
      }

      private void NotifyPropertyChanged(String propertyName)
      {
        if (PropertyChanged != null)
        {
          PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
      }

      public void CheckSchema(bool select)
      {
        _export = select;
        NotifyPropertyChanged("Export");
      }
    }

  public enum DbObjectKind : int
  {
    Table = 1,
    View = 2
  }

  public class DbSelectedObjects : INotifyPropertyChanged
  {    
    bool _selected;
    string _dbObjectName;
    DbObjectKind _kind;

    public event PropertyChangedEventHandler PropertyChanged;

    [DisplayName("Export")]
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

    [DisplayName("Schema Object")]
    public string DbObjectName
    {
      get
      {
        return _dbObjectName;
      }     
    }

    [DisplayName("Kind")]
    public DbObjectKind Kind
    {
      get
      {
        return _kind;
      }
    }

    private bool _notifiedProperty = false;
    private void NotifyPropertyChanged(String propertyName)
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
        if( _notifiedProperty ) _notifiedProperty = false;
      }
    }

    public DbSelectedObjects(string objectName, DbObjectKind kind, bool selected)
    {
      _dbObjectName = objectName.ToLowerInvariant();
      _selected = selected;
      _kind = kind;
    }
  }

  public class MySqlServerExplorerConnection
  {
    string _displayName;
    string _connectionString;

    public string DisplayName
    {
      get
      {
        return _displayName;
      }
      set
      {
        _displayName = value;
      }
    }

    public string ConnectionString
    {
      get
      {
        return _connectionString;
      }
      set
      {
        _connectionString = value;
      }
    }

    public MySqlServerExplorerConnection()
    {
      _displayName = string.Empty;
      _connectionString = string.Empty;
    }
  }

}