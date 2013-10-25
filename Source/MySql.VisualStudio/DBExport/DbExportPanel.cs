// Copyright © 2008, 2013, Oracle and/or its affiliates. All rights reserved.
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

namespace MySql.Data.VisualStudio.DBExport
{
    public partial class dbExportPanel : UserControl
    {
      private IVsOutputWindowPane _generalPane;
      private List<IVsDataExplorerConnection> _explorerMySqlConnections;
      private string _ownerSchema { get; set; }
      private string _prevSchema;
      
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
          // TODO:
          //dbSchemasList.CurrentCellDirtyStateChanged += dbSchemasList_CurrentCellDirtyStateChanged;
          //dbObjectsList.CurrentCellDirtyStateChanged += dbObjectsList_CurrentCellDirtyStateChanged;
        
          cmbConnections.SelectedIndexChanged += cmbConnections_SelectedIndexChanged;

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
      }

      void dbSchemasList_RowEnter(object sender, DataGridViewCellEventArgs e)
      {
        if (e.ColumnIndex == 1)
        {
          string schemaSelected = (string)dbSchemasList.Rows[e.RowIndex].Cells[1].Value;
          if (_prevSchema != schemaSelected )
          {
            if (!string.IsNullOrEmpty(_prevSchema))
              PullObjectListFromTree(_prevSchema);
            _prevSchema = schemaSelected;
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
          if (dictionary.ContainsKey(_ownerSchema))
            dictionary.Remove(_ownerSchema);

          var dictionaryValue = new BindingList<DbSelectedObjects>(dbObjects.ToList());          
          dictionary.Add(_ownerSchema, dictionaryValue );               

	       }
	      catch
	      {}        
      }

      //void dbSchemasList_CurrentCellDirtyStateChanged(object sender, EventArgs e)
      //{
      //  if (dbSchemasList.IsCurrentCellDirty)
      //  {
      //    dbSchemasList.CommitEdit(DataGridViewDataErrorContexts.Commit);
      //  }
      //}

      void dbSchemasList_CellValueChanged(object sender, ListChangedEventArgs e)
      {
        if (dbSchemasList.CurrentCell == null || dbSchemasList.CurrentCell.ColumnIndex != 0)
          return;
               
        int currentRow = dbSchemasList.CurrentRow.Index;
        int currentColumn = dbSchemasList.CurrentCell.ColumnIndex;


        var currentSchema = string.Empty;

        if (currentRow >= 0)
        {       
          currentSchema = dbSchemasList.Rows[currentRow].Cells[1].Value.ToString();
        }
        else
        {
          currentSchema = (new MySqlConnectionStringBuilder(SelectedConnection.ConnectionString)).Database;
        }

        if (_ownerSchema.Equals(currentSchema, StringComparison.InvariantCultureIgnoreCase))
        {
          //update to UI
          foreach (var item in dbObjects)
          {
            item.Selected = (bool)dbSchemasList.Rows[currentRow].Cells[currentColumn].Value;
          }
          
          dbObjectsList.Refresh();
        }
        else
        {
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
      }    


      void cmbConnections_SelectedIndexChanged(object sender, EventArgs e)
      {
        if ((cmbConnections.SelectedItem as MySqlServerExplorerConnection) == null) return;

        SelectedConnection.ConnectionString = ((MySqlServerExplorerConnection)cmbConnections.SelectedItem).ConnectionString;
        SelectedConnection.DisplayName = ((MySqlServerExplorerConnection)cmbConnections.SelectedItem).DisplayName;
        LoadSchemasForSelectedConnection();
      }

      void dbSchemasList_CellClick(object sender, DataGridViewCellEventArgs e)
      {
        if (e.ColumnIndex == 0)
        {
          var selected = schemas.Single(t => t.Name.Equals((string)dbSchemasList.Rows[e.RowIndex].Cells[1].Value, 
            StringComparison.InvariantCultureIgnoreCase));
          selected.CheckSchema(!selected.Export);
          //sourceSchemas.DataSource = schemas;
          dbSchemasList.Refresh();
        }
        
        if (e.ColumnIndex == 1)
        {
          var schemaSelected = (string)dbSchemasList.Rows[e.RowIndex].Cells[1].Value;
          LoadDbObjects(schemaSelected);
          _prevSchema = schemaSelected;
        }        
      }

      void cmbConnections_DropDown(object sender, EventArgs e)
      {
        //TODO
        //check how to access server explorer window in case 
        //there's a new connection
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

        // TODO:
        //dbObjects.ListChanged += new ListChangedEventHandler(dbObjectsList_ListChanged);
        BindDbObjectsToTree(dbObjects, databaseName);
      }

      // TODO:
      //void dbObjectsList_CurrentCellDirtyStateChanged(object sender, EventArgs e)
      //{
      //  if (dbObjectsList.IsCurrentCellDirty)
      //  {
      //    dbObjectsList.CommitEdit(DataGridViewDataErrorContexts.Commit);
      //  }
      //}

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
          schemaNames = SelectObjects.GetSchemas(new MySqlConnectionStringBuilder(GetCompleteConnectionString(SelectedConnection.DisplayName)));
        }
        else if (Connections.Count > 0)
          schemaNames = SelectObjects.GetSchemas(new MySqlConnectionStringBuilder(GetCompleteConnectionString(Connections[0].ConnectionString)));

        if (schemaNames == null)
          return;

        schemaNames.ForEach(t =>
        {
          if (dictionary.ContainsKey(t))
            schemas.Add(new Schema(true, t));
          else
            schemas.Add(new Schema(false, t));
        });
        
        sourceSchemas.DataSource = schemas;        
        sourceSchemas.ListChanged += new ListChangedEventHandler(dbSchemasList_CellValueChanged); 


        dbSchemasList.DataSource = sourceSchemas;
        
        dbSchemasList.Columns[0].HeaderText = "Export";
        dbSchemasList.Columns[0].Width = 45;

        dbSchemasList.Columns[1].HeaderText = "Schema";        
        dbSchemasList.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        dbSchemasList.Columns[1].ReadOnly = true;

        dbSchemasList.Refresh();
      }

      public void LoadConnections(List<IVsDataExplorerConnection> connections, string selectedConnectionName)
      {           
        try
        {
          _explorerMySqlConnections = connections;
          Connections = new List<MySqlServerExplorerConnection>(); 
          foreach (var item in connections)
          {
              Connections.Add(new MySqlServerExplorerConnection { DisplayName = item.DisplayName, ConnectionString = item.Connection.DisplayConnectionString });
              if (selectedConnectionName.Equals(item.DisplayName, StringComparison.InvariantCultureIgnoreCase))
              {
                  SelectedConnection = new MySqlServerExplorerConnection { DisplayName = item.DisplayName, ConnectionString = item.Connection.DisplayConnectionString};
              }
          }
          SetConnectionsList();
          cmbConnections.SelectedValue = SelectedConnection.ConnectionString;
        }
        catch (Exception)
        {
          MessageBox.Show(Resources.UnableToRetrieveDatabaseList, "Error", MessageBoxButtons.OK);
        }        
      }

      private void btnExport_Click(object sender, EventArgs e)
      {
        if (String.IsNullOrEmpty(txtFileName.Text))
        {
          MessageBox.Show(Resources.DbExportPathNotProvided, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
          return;                
        }

        if (CheckPathIsValid(txtFileName.Text))
        {
          MessageBox.Show(Resources.PathNotValid, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
          return;           
        }
        // pull last version of db objects
        if (!string.IsNullOrEmpty(_ownerSchema))
          PullObjectListFromTree(_ownerSchema);

        var maxAllowedPacket = 0;

        if (!int.TryParse(max_allowed_packet.Text, out maxAllowedPacket))
        {
          MessageBox.Show(Resources.InvalidMaxAllowedPacketValue, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
          return;
        }

        bndOptions.default_character_set = default_character_set.Text.Trim();
        bndOptions.max_allowed_packet = maxAllowedPacket;
        
        foreach (var item in dictionary)
        {
          MySqlConnectionStringBuilder csb;
          if (!SelectedConnection.ConnectionString.ToLower().Contains("password"))
          {
            csb = new MySqlConnectionStringBuilder(GetCompleteConnectionString(SelectedConnection.DisplayName));
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

          MySqlDbExport mysqlDbExport = null;

          if (allObjectsSelected)
            mysqlDbExport = new MySqlDbExport(bndOptions, txtFileName.Text.Trim(), new MySqlConnection(csb.ConnectionString), null);
          else
          {
            if (item.Value != null)
            {
              List<String> objects = (from s in item.Value
                                      where s.Selected
                                      select s.DbObjectName).ToList();
              mysqlDbExport = new MySqlDbExport(bndOptions, txtFileName.Text.Trim(), new MySqlConnection(csb.ConnectionString), objects);
            }
          }

          if (mysqlDbExport != null)
          {
            mysqlDbExport.Export();
            if (mysqlDbExport.ErrorsOutput != null)
            {
              if (_generalPane != null)
              {
                _generalPane.OutputString(Environment.NewLine + mysqlDbExport.ErrorsOutput.ToString());
                _generalPane.Activate();
              }
            }
          }
        }
      }

      private void btnSaveFile_Click(object sender, EventArgs e)
      {
        SaveFileDialog saveFileDlg = new SaveFileDialog(); 
        saveFileDlg.InitialDirectory = Convert.ToString(Environment.SpecialFolder.MyDocuments); 
        saveFileDlg.Filter = "(*.mysql)|*.mysql|All Files (*.*)|*.*" ; 
        saveFileDlg.FilterIndex = 1;
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
          connectionString = GetCompleteConnectionString(SelectedConnection.DisplayName);
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
          ForEach(t => { databaseObjects.Add(new DbSelectedObjects(t, DbObjectKind.Table)); });
        SelectObjects.GetViews(new MySqlConnection(connectionString)).
          ForEach(v => { databaseObjects.Add(new DbSelectedObjects(v, DbObjectKind.View)); });

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
        DataView dvSchemas = new DataView((DataTable)dbSchemasList.DataSource);        
        dvSchemas.RowFilter = string.Format("schema LIKE '{0}%'", txtFilter.Text.Trim());
        dbSchemasList.DataSource = dvSchemas;               
        dbSchemasList.Update();
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

        
      private string GetCompleteConnectionString(string connectionDisplayName)
      {
        IVsDataConnection s = (from cnn in _explorerMySqlConnections
                               where cnn.DisplayName.Equals(connectionDisplayName, StringComparison.InvariantCultureIgnoreCase)
                               select cnn.Connection).First();
        MySqlConnection connection = (MySqlConnection)s.GetLockedProviderObject();
        MySqlConnectionStringBuilder   csb = (MySqlConnectionStringBuilder)connection.GetType().GetProperty("Settings", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(connection, null);
        csb.PersistSecurityInfo = true;
        return csb.ConnectionString;
      }

      private void btnRefresh_Click(object sender, EventArgs e)
      {
        LoadSchemasForSelectedConnection();       
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
              Schema s = null;
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
              dbList.Add(new DbSelectedObjects(node.Text, kind) { Selected = node.Checked });
            }
          }
        }
        
        dbObjects = dbList;
        dictionary[schema] = dbObjects;
      }

      private void BindDbObjectsToTree(BindingList<DbSelectedObjects> data, string schema)
      {
        TreeNode root = null;
        dbObjectsList.BeginUpdate();
        try
        {
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
        }
        finally
        {
          dbObjectsList.EndUpdate();
        }
      }

      private void dbObjectsList_DoubleClick(object sender, EventArgs e)
      {
        // Inhibite normal double click behavior
        return;
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

    public DbSelectedObjects(string objectName, DbObjectKind kind)
    {
      _dbObjectName = objectName.ToLowerInvariant();
      _selected = false;
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