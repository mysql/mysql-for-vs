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
using System.Text;
using System.Windows.Forms;
using MySql.Data.VisualStudio.Properties;
using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Data;
using System.Diagnostics;
using Microsoft.VisualStudio;
using System.Data.Common;
using System.Resources;
using System.Globalization;
using System.Data;
using Microsoft.VisualStudio.Data.AdoDotNet;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using MySql.Data.VisualStudio.Editors;
using Microsoft.VisualStudio.Shell;
using MySql.Data.MySqlClient;
using MySql.Utility.Classes.Logging;
using MySql.Data.VisualStudio.Common;

namespace MySql.Data.VisualStudio
{
  abstract class BaseNode
  {
    protected Guid editorGuid;
    protected Guid commandGroupGuid;
    protected string name;
    private static string defaultStorageEngine;
    public DataViewHierarchyAccessor HierarchyAccessor;
    public bool IsNew;
    protected const string SEPARATOR = "/* separator */";

    protected string OldObjectDefinition { get; set; }

    public BaseNode(DataViewHierarchyAccessor hierarchyAccessor, int id)
    {
      HierarchyAccessor = hierarchyAccessor;
      ItemId = id;
      IsNew = ItemId == 0;

      // set the server and database from our active connection
      DbConnection conn = (DbConnection)HierarchyAccessor.Connection.GetLockedProviderObject();
      Server = conn.DataSource;
      Database = conn.Database;
      HierarchyAccessor.Connection.UnlockProviderObject();

      if (!IsNew)
        Name = hierarchyAccessor.GetNodeName(id);
      else
        ItemId = hierarchyAccessor.CreateObjectNode();

      NameIndex = 2;
      commandGroupGuid = VSConstants.GUID_TextEditorFactory;
      editorGuid = Guid.Empty;
    }

    protected DbConnection AcquireHierarchyAccessorConnection()
    {
      DbConnection con = (DbConnection)HierarchyAccessor.Connection.GetLockedProviderObject();
      var connStringBuilder = ((MySqlConnectionStringBuilder)con.GetType().GetProperty("Settings", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(con, null));
      connStringBuilder.AllowUserVariables = true;
      _con = new MySqlConnection(connStringBuilder.ConnectionString);     
      _con.OpenWithDefaultTimeout();
      return _con;
    }

    protected void ReleaseHierarchyAccessorConnection()
    {
      HierarchyAccessor.Connection.UnlockProviderObject();
      _con.Close();
    }

    #region Properties

    protected MySqlConnection _con;

    public string Name
    {
      get
      {
        if (name == null)
          Name = GenerateUniqueName();
        return name;
      }
      protected set
      {
        name = value;
      }
    }

    private int _itemId;
    public int ItemId
    {
      get { return _itemId; }
      protected set { _itemId = value; }
    }

    private string _nodeId;
    public string NodeId
    {
      get { return _nodeId; }
      protected set { _nodeId = value; }
    }

    private string _server;
    public string Server
    {
      get { return _server; }
      private set { _server = value; }
    }

    private string _database;
    public string Database
    {
      get { return _database; }
      private set { _database = value; }
    }

    private bool _dirty;
    public virtual bool Dirty
    {
      get { return _dirty; }
      protected set { _dirty = value; }
    }

    protected internal string Moniker
    {
      get { return String.Format("mysql://{0}/{1}/{2}", Server, Database, Name); }
    }

    private int _nameIndex;
    public int NameIndex
    {
      get { return _nameIndex; }
      protected set { _nameIndex = value; }
    }

    public virtual string SchemaCollection
    {
      get{ return NodeId!="UDF" ? NodeId + "s" : NodeId; }
    }

    public virtual string LocalizedTypeString
    {
      get
      {
        return Properties.Resources.ResourceManager.GetString("Type_" + NodeId);
      }
    }

    public string DefaultStorageEngine
    {
      get
      {
        if (defaultStorageEngine == null)
        {
          DataTable dt = GetDataTable("SHOW VARIABLES LIKE 'storage_engine'");
          defaultStorageEngine = "MyISAM";
          if (dt != null && dt.Rows.Count == 1)
            defaultStorageEngine = (string)dt.Rows[0][1];
        }
        return defaultStorageEngine;
      }
    }

    #endregion

    #region Virtuals

    public virtual void ExecuteCommand(int command)
    {
      switch ((uint)command)
      {
        case 12291:  // design
        case PkgCmdIDList.cmdAlterTable:
        case PkgCmdIDList.cmdAlterTrigger:
        case PkgCmdIDList.cmdAlterProcedure:
        case PkgCmdIDList.cmdAlterView:
          Alter();
          break;

        case (uint)VSConstants.VSStd97CmdID.Delete:
          Drop();
          break;

        case PkgCmdIDList.cmdDebugProcedure:
          LaunchDebugger();
          break;

        case PkgCmdIDList.cmdGenerateTableScript:
          GenerateTableScript();
          break;
      }
    }


    public virtual void Edit()
    {
      if (!HierarchyAccessor.ActivateDocumentIfOpen(Moniker))
        OpenEditor();
    }

    public virtual object GetEditor()
    {
      throw new NotImplementedException();
    }

    public virtual void Alter()
    {
      Edit();
    }

    public virtual void LaunchDebugger()
    {
      throw new NotImplementedException();
    }

    public virtual void GenerateTableScript()
    {
      throw new NotImplementedException();
    }

    public virtual string GetDropSQL()
    {
      return string.Format("DROP {0} `{1}`.`{2}`", NodeId, Database, Name);
    }

    private void Drop()
    {
      string typeString = LocalizedTypeString.ToLower(CultureInfo.CurrentCulture);

      using (var yesNoDialog = Common.Utilities.GetYesNoInfoDialog(
                                 Utility.Forms.InfoDialog.InfoType.Info,
                                 false,
                                 string.Format(Properties.Resources.DropConfirmationCaption, typeString),
                                 string.Format(Properties.Resources.DropConfirmation, typeString, Name)
      ))
      {
        if (yesNoDialog.ShowDialog() == DialogResult.No)
        {
          throw new OperationCanceledException();
        }
      }
 
      string sql = GetDropSQL();
      try
      {
        ExecuteSQL(sql);

        // Now we drop the node from the hierarchy.
        HierarchyAccessor.DropObjectNode(ItemId);
      }
      catch (Exception ex)
      {
        Logger.LogError(
            string.Format(Properties.Resources.ErrorAttemptingToDrop,
            LocalizedTypeString, Name, ex.Message), true);
        throw new OperationCanceledException();
      }
    }

    protected virtual string GenerateUniqueName()
    {
      DbConnection conn = AcquireHierarchyAccessorConnection();
      string[] restrictions = new string[2] { null, Database };
      try
      {
        DataTable dt = conn.GetSchema(SchemaCollection, restrictions);

        int uniqueIndex = 1;
        string objectName = String.Empty;

        foreach (DataRow row in dt.Rows)
        {
          objectName = String.Format("{0}{1}", NodeId, uniqueIndex).Replace(" ", "");
          if (row[NameIndex].ToString().ToLowerInvariant() == objectName.ToLowerInvariant())
            uniqueIndex++;
        }
        Name = String.Format("{0}{1}", NodeId, uniqueIndex).Replace(" ", "");
        HierarchyAccessor.SetProperty(ItemId, (int)__VSHPROPID.VSHPROPID_Name, Name);
        return Name;
      }
      finally
      {
        ReleaseHierarchyAccessorConnection();
      }
    }

    #endregion

    #region Private Methods

    private void OpenEditor()
    {
      IVsUIShellOpenDocument shell =
        MySqlDataProviderPackage.GetGlobalService(typeof(SVsUIShellOpenDocument)) as IVsUIShellOpenDocument;

      IVsWindowFrame winFrame = null;

      object editor = GetEditor();
      object coreEditor = editor;
      editorGuid = editor.GetType().GUID;
      if (editor is VSCodeEditor)
        coreEditor = (editor as VSCodeEditor).CodeWindow;

      IntPtr viewPunk = Marshal.GetIUnknownForObject(coreEditor);
      IntPtr dataPunk = Marshal.GetIUnknownForObject(this);
      Guid viewGuid = VSConstants.LOGVIEWID_TextView;

      // Initialize IDE editor infrastracture
      int result = shell.InitializeEditorInstance(
        (uint)0,                // Initialization flags. We need default behavior here
        viewPunk,               // View object reference (should implement IVsWindowPane)
        dataPunk,               // Docuemnt object reference (should implement IVsPersistDocData)
        Moniker,                // Document moniker
        ref editorGuid,         // GUID of the editor type
        null,                   // Name of the physical view. We use default
        ref viewGuid,           // GUID identifying the logical view.
        null,                   // Initial caption defined by the document owner. Will be initialized by the editor later
        null,                   // Initial caption defined by the document editor. Will be initialized by the editor later
        // Pointer to the IVsUIHierarchy interface of the project that contains the document
        HierarchyAccessor.Hierarchy,
        (uint)ItemId,           // UI hierarchy item identifier of the document in the project system
        IntPtr.Zero,            // Pointer to the IUnknown interface of the document data object if the document data object already exists in the running document table
        // Project-specific service provider.
        HierarchyAccessor.ServiceProvider as Microsoft.VisualStudio.OLE.Interop.IServiceProvider,
        ref commandGroupGuid,   // Command UI GUID of the commands to display for this editor.
        out winFrame            // The window frame that contains the editor
        );

      Debug.Assert(winFrame != null &&
             ErrorHandler.Succeeded(result), "Failed to initialize editor");

      // Prevent the editor window from showing if there was an error loading the code.
      var vsCodeEditor = editor as VSCodeEditor;
      if (vsCodeEditor?.Text == string.Empty)
      {
        return;
      }

      if (winFrame == null)
      {
        var message = Properties.Resources.DesignWindowCreationFailed;
        if (!string.IsNullOrEmpty(MySqlProviderObjectFactory.FactoryErrorMessage))
        {
          var factoryMessage = MySqlProviderObjectFactory.FactoryErrorMessage.Contains(Properties.Resources.NetFrameworkDataProviderNotFound)
                                ? Properties.Resources.ConnectorNetNotFound
                                : MySqlProviderObjectFactory.FactoryErrorMessage;
          message += $" Creation failed with message: {factoryMessage}";
        }

        throw new Exception(message);
      }

      winFrame.Show();
    }

    protected void SaveNode()
    {
      string nodePath = String.Format("/Connection/{0}s/{1}", NodeId, Name);
      HierarchyAccessor.SetNodePath(ItemId, nodePath);
    }

    protected void ExecuteSQL(string sql)
    {
      DbConnection conn = AcquireHierarchyAccessorConnection();
      try
      {
        DbCommand cmd = conn.CreateCommand();
        if (!string.IsNullOrEmpty(OldObjectDefinition))
        {
          int idx = sql.IndexOf( SEPARATOR );
          if (idx != -1)
          {
            cmd.CommandText = sql.Substring(0, idx);
            cmd.ExecuteNonQuery();

            try
            {
              cmd.CommandText = sql.Substring(idx + SEPARATOR.Length);
              cmd.ExecuteNonQuery();
            }
            catch (MySqlException)
            {
              // Some objects (like views/stored routines) are altered by dropping/recreating them, if the recreation part fails, 
              // we ensure here the old object is created again (will work most of the time, save of connection lost scensarios)
              cmd = conn.CreateCommand();
              cmd.CommandText = OldObjectDefinition;
              cmd.ExecuteNonQuery();
            }
          }
        }
        else
        {
          cmd.CommandText = sql;
          cmd.ExecuteNonQuery();
        }
      }
      finally
      {
        ReleaseHierarchyAccessorConnection();
      }
    }

    public DataTable GetSchema(string schemaName, string[] restrictions)
    {
      DbConnection conn = AcquireHierarchyAccessorConnection();
      try
      {
        return conn.GetSchema(schemaName, restrictions);
      }
      catch (Exception ex)
      {
        Logger.LogException(ex);
        return null;
      }
      finally
      {
        ReleaseHierarchyAccessorConnection();
      }
    }

    public DataTable GetDataTable(string sql)
    {
      DbConnection conn = AcquireHierarchyAccessorConnection();
      try
      {
        DbDataAdapter da = MySqlProviderObjectFactory.Factory.CreateDataAdapter();
        DbCommand cmd = MySqlProviderObjectFactory.Factory.CreateCommand();
        cmd.Connection = conn;
        cmd.CommandText = sql;
        da.SelectCommand = cmd;
        DataTable dt = new DataTable();
        da.Fill(dt);
        return dt;
      }
      finally
      {
        ReleaseHierarchyAccessorConnection();
      }
    }

    /// <summary>
    /// Selects connection node in the Server Explorer window.
    /// </summary>
    private void SelectConnectionNode()
    {
      //MySqlDataProviderPackage package = MySqlDataProviderPackage.Instance;
      // Extracts connection mamanger global service
      DataExplorerConnectionManager manager = Package.GetGlobalService(typeof(DataExplorerConnectionManager))
          as DataExplorerConnectionManager;
      if (manager == null)
      {
        Debug.Fail("Failed to get connection manager!");
        return;
      }

      // Searches for connection using connection string for current connection
      DataExplorerConnection connection = manager.FindConnection(
          Guids.Provider, HierarchyAccessor.Connection.EncryptedConnectionString, true);
      if (connection == null)
      {
        Debug.Fail("Failed to find proper connection node!");
        return;
      }

      // Select connection node
      manager.SelectConnection(connection);
    }

    #endregion

    /// <summary>
    /// Refresh database node in server explorer
    /// </summary>
    public void Refresh()
    {
      SelectConnectionNode();
      IVsUIHierarchy hier = HierarchyAccessor.Hierarchy as IVsUIHierarchy;
      Guid g = VSConstants.GUID_VSStandardCommandSet97;
      hier.ExecCommand(VSConstants.VSITEMID_ROOT, ref g, (uint)VSConstants.VSStd97CmdID.Refresh,
          (uint)OleCommandExecutionOption.DoDefault, IntPtr.Zero, IntPtr.Zero);
    }
  }
}
