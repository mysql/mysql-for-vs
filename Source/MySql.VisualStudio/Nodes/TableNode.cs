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
using System.Data.Common;
using System.Data;
using Microsoft.VisualStudio.Data;
using MySql.Data.VisualStudio.DbObjects;
using System.Windows.Forms;
using MySql.Data.VisualStudio.Editors;
using System.Diagnostics;

namespace MySql.Data.VisualStudio
{
  class TableNode : DocumentNode
  {
    private Table table;

    public TableNode(DataViewHierarchyAccessor hierarchyAccessor, int id) :
      base(hierarchyAccessor, id)
    {
      NodeId = "Table";
      //commandGroupGuid = GuidList.DavinciCommandSet;
      DocumentNode.RegisterNode(this);
      Saving += new EventHandler(TableNode_Saving);
    }

    void TableNode_Saving(object sender, EventArgs e)
    {
      // Some useful validations
      bool hasAutoIncr = false;
      bool hasPK = false;
      for (int i = 0; i < table.Columns.Count; i++)
      {
        if (table.Columns[i].AutoIncrement)
        {
          hasAutoIncr = true;
          break;
        }
      }

      for (int i = 0; i < table.Indexes.Count; i++)
      {
        if (table.Indexes[i].IsPrimary)
        {
          hasPK = true;
          break;
        }
      }
      if (hasAutoIncr && !hasPK)
        throw new ArgumentException( Properties.Resources.AutoIncrementPrimaryKey );
    }

    #region Properties

    public Table Table
    {
      get { return table; }
    }

    public override bool Dirty
    {
      get
      {
        if (table == null)
          return false;

        return table.HasChanges();
      }
    }

    #endregion

    protected override string GetCurrentName()
    {
      return table.Name;
    }

    /// <summary>
    /// We override Save here because we want to prompt for a new name if this table is new and the user has
    /// not changed the default name
    /// </summary>
    /// <returns></returns>
    protected override bool Save()
    {
      if (table.IsNew && table.Name == Name)
      {
        TableNamePromptDialog dlg = new TableNamePromptDialog();
        dlg.TableName = table.Name;
        if (DialogResult.Cancel == dlg.ShowDialog()) return false;
        table.Name = dlg.TableName;
      }
      try
      {
        return base.Save();
      }
      catch ( MySql.Data.MySqlClient.MySqlException ex)
      {
        // Undo name edited
        Debug.WriteLine(ex.Message);
        table.Name = Name;
        throw;
      }
    }

    public static void CreateNew(DataViewHierarchyAccessor HierarchyAccessor)
    {
      TableNode node = new TableNode(HierarchyAccessor, 0);
      node.Edit();
    }

    protected override void Load()
    {
      if (IsNew)
      {
        table = new Table(this, null, null);
        table.Name = Name;
      }
      else
      {
        DbConnection connection = AcquireHierarchyAccessorConnection();
        try
        {
          string[] restrictions = new string[4] { null, connection.Database, Name, null };
          DataTable columnsTable = connection.GetSchema("Columns", restrictions);

          DataTable dt = connection.GetSchema("Tables", restrictions);
          table = new Table(this, dt.Rows[0], columnsTable);
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
#if NET_472_OR_GREATER
      return new TableEditorWPF(this);
#else
      return new TableEditorPane(this);
#endif
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
        cmd.CommandText = string.Format("show create table `{0}`", base.Name );
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
