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
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.VisualStudio.Data;
using MySql.Data.VisualStudio.DbObjects;
using MySql.Data.VisualStudio.Editors;
using MySql.Data.VisualStudio.Properties;
using MySql.Data.VisualStudio.DDEX;

namespace MySql.Data.VisualStudio.Nodes
{
  class TableNode : DocumentNode
  {
    private Table _table;

    public TableNode(DataViewHierarchyAccessor hierarchyAccessor, int id) :
      base(hierarchyAccessor, id)
    {
      NodeId = "Table";
      //commandGroupGuid = GuidList.DavinciCommandSet;
      RegisterNode(this);
      Saving += TableNode_Saving;
    }

    void TableNode_Saving(object sender, EventArgs e)
    {
      // Some useful validations
      bool hasAutoIncr = false;
      bool hasPk = false;
      for (int i = 0; i < _table.Columns.Count; i++)
      {
        if (_table.Columns[i].AutoIncrement)
        {
          hasAutoIncr = true;
          break;
        }
      }

      for (int i = 0; i < _table.Indexes.Count; i++)
      {
        if (_table.Indexes[i].IsPrimary)
        {
          hasPk = true;
          break;
        }
      }
      if (hasAutoIncr && !hasPk)
        throw new ArgumentException( Resources.AutoIncrementPrimaryKey );
    }

    #region Properties

    public Table Table
    {
      get { return _table; }
    }

    public override bool Dirty
    {
      get
      {
        if (_table == null)
          return false;

        return _table.HasChanges();
      }
    }

    #endregion

    protected override string GetCurrentName()
    {
      return _table.Name;
    }

    /// <summary>
    /// We override Save here because we want to prompt for a new name if this table is new and the user has
    /// not changed the default name
    /// </summary>
    /// <returns></returns>
    protected override bool Save()
    {
      if (_table.IsNew && _table.Name == Name)
      {
        TableNamePromptDialog dlg = new TableNamePromptDialog();
        dlg.TableName = _table.Name;
        if (DialogResult.Cancel == dlg.ShowDialog()) return false;
        _table.Name = dlg.TableName;
      }
      try
      {
        return base.Save();
      }
      catch (MySqlClient.MySqlException ex)
      {
        // Undo name edited
        Debug.WriteLine(ex.Message);
        _table.Name = Name;
        throw;
      }
    }

    public static void CreateNew(DataViewHierarchyAccessor hierarchyAccessor)
    {
      TableNode node = new TableNode(hierarchyAccessor, 0);
      node.Edit();
    }

    protected override void Load()
    {
      if (IsNew)
      {
        _table = new Table(this, null, null);
        _table.Name = Name;
      }
      else
      {
        DbConnection connection = AcquireHierarchyAccessorConnection();
        try
        {
          string[] restrictions = new string[] { null, connection.Database, Name, null };
          DataTable columnsTable = connection.GetSchema("Columns", restrictions);

          DataTable dt = connection.GetSchema("Tables", restrictions);
          _table = new Table(this, dt.Rows[0], columnsTable);
        }
        finally
        {
          ReleaseHierarchyAccessorConnection();
        }
      }
      OnDataLoaded();
    }

    public override string GetSaveSql()
    {
      return Table.GetSql();
    }

    public override object GetEditor()
    {
      return new TableEditorPane(this);
    }

    public override void ExecuteCommand(int command)
    {
      if (command == PkgCmdIDList.cmdCreateTrigger)
      {
        TriggerNode.CreateNew(HierarchyAccessor, this);
      }
      else
        base.ExecuteCommand(command);
    }

    public override void GenerateTableScript()
    {
      // Popup Window if table script.
      MySqlScriptDialog dlg = new MySqlScriptDialog();
      DbConnection conn = (DbConnection)HierarchyAccessor.Connection.GetLockedProviderObject();
      try
      {
        DbCommand cmd = conn.CreateCommand();
        cmd.CommandText = string.Format("show create table `{0}`", Name );
        using (DbDataReader r = cmd.ExecuteReader())
        {
          r.Read();
          dlg.TextScript = r.GetString(1);
        }
        dlg.ShowDialog();
      }
      finally
      {
        HierarchyAccessor.Connection.UnlockProviderObject();
      }
    }
  }
}
