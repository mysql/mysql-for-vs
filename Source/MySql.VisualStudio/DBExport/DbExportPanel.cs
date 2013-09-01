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
      
      internal MySqlDbExportOptions bndOptions;
      
      internal List<DbSelectedObjects> dbObjects = new List<DbSelectedObjects>();
      internal Dictionary<string, List<DbSelectedObjects>> dictionary = new Dictionary<string, List<DbSelectedObjects>>();

      public MySqlServerExplorerConnection SelectedConnection { get; set; }
      public List<MySqlServerExplorerConnection> Connections { get; set; }
      
      
      public dbExportPanel()        
      {
          InitializeComponent();                  
          dbSchemasList.CellClick += dbSchemasList_CellClick;
          dbSchemasList.RowLeave += dbSchemasList_RowLeave;
          dbSchemasList.RowEnter += dbSchemasList_RowEnter;
          dbSchemasList.CurrentCellDirtyStateChanged += dbSchemasList_CurrentCellDirtyStateChanged;
          dbSchemasList.CellValueChanged += dbSchemasList_CellValueChanged;
        
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
        try
        {
          if (e.ColumnIndex != 1)
            return;
          string schema = dbSchemasList.Rows[e.RowIndex].Cells[1].Value.ToString();
          LoadDbObjects(schema, (bool)dbSchemasList.Rows[e.RowIndex].Cells[0].Value);
        }
        catch
        {
        }        
      }

      void dbSchemasList_RowLeave(object sender, DataGridViewCellEventArgs e)
      {
        try 
	       {	        
	        string schema = dbSchemasList.Rows[e.RowIndex].Cells[1].Value.ToString();
          List<DbSelectedObjects> selectedObjects = new List<DbSelectedObjects>();

            if (dictionary.TryGetValue(schema, out selectedObjects))
              dictionary.Remove(schema);

            bool dbSelected = false;
            bool.TryParse(dbSchemasList.Rows[e.RowIndex].Cells[0].Value.ToString(), out dbSelected);                                        
            if (dbSelected)
            {
                dictionary.Add(schema, dbObjects);               
            }           
	       }
	      catch
	      {
		    }        
      }

      void dbSchemasList_CurrentCellDirtyStateChanged(object sender, EventArgs e)
      {
        if (dbSchemasList.IsCurrentCellDirty)
        {
          dbSchemasList.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }
      }
     
      void dbSchemasList_CellValueChanged(object sender, DataGridViewCellEventArgs e)
      {
        if (e.ColumnIndex == 0)
        {
          dbObjects.ForEach(t => { t.ChangeState((bool)dbSchemasList.Rows[e.RowIndex].Cells[e.ColumnIndex].Value); });
          dbObjectsList.Refresh();
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
        if (e.ColumnIndex == 1)
        {
            var selected = (string)dbSchemasList.Rows[e.RowIndex].Cells[1].Value;
            LoadDbObjects(selected, (bool)dbSchemasList.Rows[e.RowIndex].Cells[0].Value);
        }
      }

      void cmbConnections_DropDown(object sender, EventArgs e)
      {
        //TODO
        //check how to access server explorer window in case 
        //there's a new connection
      }

        

      public void LoadDbObjects(string databaseName, bool checkedDb)
      {       
        var connectionString = String.Empty;
        BindingList<DbSelectedObjects> source = new BindingList<DbSelectedObjects>();

        if (dictionary.TryGetValue(databaseName, out dbObjects))
        {
          dbObjects.ForEach(t => { source.Add(t); });
          dbObjectsList.DataSource = new BindingSource { DataSource = source };
          dbObjectsList.Update();          
          return;
        }

        dbObjects = new List<DbSelectedObjects>();

        if (SelectedConnection != null)
          connectionString = GetCompleteConnectionString(SelectedConnection.DisplayName);
        else
          connectionString = Connections[0].ConnectionString;

        if (String.IsNullOrEmpty(connectionString))
          return;

        MySqlConnectionStringBuilder csb = new MySqlConnectionStringBuilder(connectionString);

        if (!csb.Database.Equals(databaseName, StringComparison.InvariantCultureIgnoreCase))
        {
          csb.Database = databaseName;
          connectionString = csb.ConnectionString;
        }

        SelectObjects.GetTables(new MySqlConnection(connectionString), null, null, false).ForEach(t => { dbObjects.Add(new DbSelectedObjects(t)); });
        SelectObjects.GetViews(new MySqlConnection(connectionString)).ForEach(v => { dbObjects.Add(new DbSelectedObjects(v)); });

        dbObjects.Sort((o1,o2)=>string.Compare(o1.DbObjectName, o2.DbObjectName,true));
        dbObjectsList.Rows.Clear();

        dbObjects.ForEach(t => { t.ChangeState(checkedDb); });
        dbObjects.ForEach(t => { source.Add(t); });
        
        dbObjectsList.DataSource = new BindingSource { DataSource = source };
        dbObjectsList.Columns[0].HeaderText = "Export";
        dbObjectsList.Columns[0].Width = 45;
        
        dbObjectsList.Columns[1].HeaderText = "Object Name";
        dbObjectsList.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

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
        List<String> schemas = null;

        if (SelectedConnection != null)
        {         
          schemas = SelectObjects.GetSchemas(new MySqlConnectionStringBuilder(GetCompleteConnectionString(SelectedConnection.DisplayName)));
        }
        else if (Connections.Count > 0)
          schemas = SelectObjects.GetSchemas(new MySqlConnectionStringBuilder(GetCompleteConnectionString(Connections[0].ConnectionString)));

        if (schemas == null)
          return;

        DataTable dtSchemas = new DataTable();
        DataColumn columnSelected = new DataColumn();
        columnSelected.ColumnName = "selected";
        columnSelected.DataType = typeof(Boolean);

        DataColumn columnSchema = new DataColumn();
        columnSchema.ColumnName = "schema";
        columnSchema.DataType = typeof(String);

        dtSchemas.Columns.Add(columnSelected);
        dtSchemas.Columns.Add(columnSchema);

        foreach (var schema in schemas)
        {
          if (dictionary.ContainsKey(schema))          
             dtSchemas.Rows.Add(new object[] { true, schema });          
          else
             dtSchemas.Rows.Add(new object[] { false, schema });
        }
        dbSchemasList.DataSource = dtSchemas;
        dbSchemasList.Columns[0].HeaderText = "Export";
        dbSchemasList.Columns[0].Width = 45;

        dbSchemasList.Columns[1].HeaderText = "Schema";        
        dbSchemasList.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        dbSchemasList.Columns[1].ReadOnly = true;
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
          MessageBox.Show(Resources.DbExportPathNotProvided, "Error", MessageBoxButtons.OK);
          return;                
        }

        if (CheckPathIsValid(txtFileName.Text))
        {
          MessageBox.Show(Resources.PathNotValid);
          return;           
        }

        var maxAllowedPacket = 0;

        if (!int.TryParse(max_allowed_packet.Text, out maxAllowedPacket))
        {
          MessageBox.Show("Invalid value on max-allowed-packet option.");
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
          item.Value.ForEach(t => { if (t.Selected == false) allObjectsSelected = false; });          

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

      private void btnUnSelect_Click(object sender, EventArgs e)
      {
        BindingList<DbSelectedObjects> source = new BindingList<DbSelectedObjects>();

        string schema = GetSelectedSchema(dbSchemasList.CurrentRow.Index != -1 ?
          (bool)dbSchemasList.Rows[dbSchemasList.CurrentRow.Index].Cells[0].Value :
          (bool)dbSchemasList.Rows[0].Cells[0].Value);
                    
        if (dbObjects != null)
        {
            dbObjects.ForEach(t => { t.ChangeState(false); });
            dbObjects.ForEach(t => { source.Add(t); });        
        }
        dbObjectsList.DataSource = source;
        dbObjectsList.Update();

        if (dictionary.ContainsKey(schema))
         LoadDbObjects(schema, false);
         
        return;
      }

      private string GetSelectedSchema(bool addToSelection)
      {
        int currentRow = dbSchemasList.CurrentRow.Index;

        if (currentRow >= 0)
        {
          dbSchemasList.Rows[currentRow].Cells[0].Value = addToSelection;
          return dbSchemasList.Rows[currentRow].Cells[1].Value.ToString();
        }
        else
        {
          return new MySqlConnection(SelectedConnection.ConnectionString).Database;
        }
      }

      private void btnSelectAll_Click(object sender, EventArgs e)
      {
        string schema = GetSelectedSchema(true);

        if (dbObjects != null)
        {
          dbObjects.ForEach(t => { t.ChangeState(true); });         
          if (!dictionary.ContainsKey(schema))
            dictionary.Add(schema, dbObjects);
        }

        LoadDbObjects(schema, true);
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
  
  public class DbSelectedObjects : INotifyPropertyChanged
  {    
    bool _selected;
    string _dbObjectName;

    public event PropertyChangedEventHandler PropertyChanged;

    [DisplayName("Export")]
    public bool Selected
    {
      get
      {
        return _selected;
      }
      set {
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

    private void NotifyPropertyChanged(String propertyName)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }
    

    public DbSelectedObjects(string objectName)
    {
      _dbObjectName = objectName.ToLowerInvariant();
      _selected = false;
    }


    public void ChangeState(bool value)
    {
       _selected=value;
    }
  }
}