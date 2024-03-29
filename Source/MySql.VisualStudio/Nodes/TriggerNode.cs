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
using System.Text;
using System.Data.Common;
using Microsoft.VisualStudio.Data;
using System.Windows.Forms;
using Microsoft.VisualStudio.TextManager.Interop;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.Shell.Interop;
using System.Diagnostics;
using OleInterop = Microsoft.VisualStudio.OLE.Interop;
using System.Data;
using MySql.Data.VisualStudio.Editors;
using MySql.Data.VisualStudio.Properties;
using MySql.Data.MySqlClient;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using MySql.Utility.Classes.Logging;

namespace MySql.Data.VisualStudio
{
  class TriggerNode : DocumentNode, IVsTextBufferProvider
  {
    private string sql_mode;
    private VSCodeEditor editor;
    string table;

    public TriggerNode(DataViewHierarchyAccessor hierarchyAccessor, int id) :
      base(hierarchyAccessor, id)
    {
      NodeId = "Trigger";
      NameIndex = 2;
      editor = new VSCodeEditor((IOleServiceProvider)hierarchyAccessor.ServiceProvider);
      DocumentNode.RegisterNode(this);
    }

    #region Properties

    public TableNode ParentTable;

    public override string SchemaCollection
    {
      get { return "triggers"; }
    }

    public override bool Dirty
    {
      get { return editor.Dirty; }
      protected set { editor.Dirty = value; }
    }

    #endregion

    public static void CreateNew(DataViewHierarchyAccessor HierarchyAccessor, TableNode parent)
    {
      TriggerNode node = new TriggerNode(HierarchyAccessor, 0);
      node.ParentTable = parent;
      node.Edit();
    }

    public override object GetEditor()
    {
      return editor;
    }

    public override string GetDropSQL()
    {
      return GetDropSQL(Name);
    }

    public override string GetSaveSql()
    {
      string sql = ChangeSqlTypeTo(editor.Text, "CREATE").Trim();
      if (IsNew)
      {
        return sql;
      }
      else
      {
        return string.Format("{0}{1}{2};", GetDropSQL(), BaseNode.SEPARATOR, sql);
      }
    }

    private string GetDropSQL(string triggerName)
    {
      triggerName = triggerName.Trim('`');
      return String.Format("DROP TRIGGER `{0}`.`{1}`", Database, triggerName);
    }

    private string GetNewTriggerText()
    {
      StringBuilder sb = new StringBuilder("CREATE TRIGGER ");
      sb.AppendFormat("{0}\r\n", Name);
      sb.AppendFormat("/* [BEFORE|AFTER] [INSERT|UPDATE|DELETE] */\r\n");
      sb.AppendFormat("ON {0}\r\n", ParentTable.Name);
      sb.Append("FOR EACH ROW\r\n");
      sb.Append("BEGIN\r\n");
      sb.Append("/* sql commands go here */\r\n");
      sb.Append("END");
      return sb.ToString();
    }

    protected override void Load()
    {
      if (IsNew)
      {
        editor.Text = GetNewTriggerText();
        sql_mode = string.Empty;
      }
      else
      {
        try
        {
          DataTable dt = GetDataTable(String.Format("SHOW CREATE TRIGGER `{0}`.`{1}`",
                  Database, Name));

          sql_mode = dt.Rows[0][1] as string;
          string sql = dt.Rows[0][2] as string;
		  OldObjectDefinition = sql;
          byte[] bytes = UTF8Encoding.UTF8.GetBytes(sql);
          editor.Text = sql;
          Dirty = false;
          OnDataLoaded();
        }
        catch (Exception ex)
        {
          Logger.LogError($"Unable to load object with error: {ex.Message}", true);
        }
      }
      table = GetTargetedTable(editor.Text);
    }

    /// <summary>
    /// We override save here so we can change the sql from create to alter on
    /// first save
    /// </summary>
    /// <returns></returns>
    protected override bool Save()
    {
      try
      {
        string sql = editor.Text.Trim();
        if (!IsNew)
        {
          MakeSureWeAreNotChangingTables(sql);
          CheckSyntax();
        }
        ExecuteSQL(GetSaveSql());
        return true;
      }
      catch (Exception ex)
      {
        Logger.LogError(ex.Message, true);
        return false;
      }
    }

    /// <summary>
    ///  This method will attempt to extract the table this trigger script is targeting 
    ///  and make sure it matches the table the trigger was originally created for.  We do 
    ///  this because we don't want the user using an 'ALTER' script to move a trigger to a 
    ///  different table
    /// </summary>
    private void MakeSureWeAreNotChangingTables(string sql)
    {
      string newTable = GetTargetedTable(sql);
      if (table != null && newTable != null &&
          newTable.ToLowerInvariant() != table.ToLowerInvariant())
        throw new InvalidOperationException(
            String.Format(Properties.Resources.AlterTriggerOnWrongTable, Name, newTable));
    }

    private string GetTargetedTable(string sql)
    {
      MySqlTokenizer tokenizer = new MySqlTokenizer(sql);
      tokenizer.ReturnComments = false;
      tokenizer.AnsiQuotes = sql_mode.ToLowerInvariant().Contains("ansi_quotes");
      tokenizer.BackslashEscapes = !sql_mode.ToLowerInvariant().Contains("no_backslash_escapes");

      string token = null;
      while (token != "ON" || tokenizer.Quoted)
        token = tokenizer.NextToken();

      string tableName = tokenizer.NextToken();
      if (tokenizer.NextToken() == ".")
        tableName = tokenizer.NextToken();
      if (tableName.StartsWith("`", StringComparison.Ordinal))
        return tableName.Trim('`');
      if (tableName.StartsWith("\"", StringComparison.Ordinal) && tokenizer.AnsiQuotes)
        return tableName.Trim('"');
      return tableName;
    }

    private void CheckSyntax()
    {
      MySqlConnection con = ( MySqlConnection )GetCurrentConnection();
      string sql = editor.Text.Trim();
      StringBuilder sb;
      LanguageServiceUtil.ParseSql(sql, false, out sb, con.ServerVersion);
      if (sb.Length != 0)
        throw new Exception(string.Format("Syntax Error: {0}", sb.ToString()));
    }

    private string ChangeSqlTypeTo(string sql, string type)
    {
      int index = sql.IndexOf(' ');
      string startingCommand = sql.Substring(0, index).ToUpperInvariant();
      if (startingCommand != "CREATE" && startingCommand != "ALTER")
        throw new Exception(Properties.Resources.UnableToExecuteProcScript);
      return type + sql.Substring(index);
    }

    protected override string GetCurrentName()
    {
      return LanguageServiceUtil.GetRoutineName(editor.Text);
    }

    #region IVsTextBufferProvider Members

    private IVsTextLines buffer;

    int IVsTextBufferProvider.GetTextBuffer(out IVsTextLines ppTextBuffer)
    {
      if (buffer == null)
      {
        Type bufferType = typeof(IVsTextLines);
        Guid riid = bufferType.GUID;
        Guid clsid = typeof(VsTextBufferClass).GUID;
        buffer = (IVsTextLines)MySqlDataProviderPackage.Instance.CreateInstance(
                             ref clsid, ref riid, typeof(object));
      }
      ppTextBuffer = buffer;
      return VSConstants.S_OK;
    }

    int IVsTextBufferProvider.LockTextBuffer(int fLock)
    {
      return VSConstants.S_OK;
    }

    int IVsTextBufferProvider.SetTextBuffer(IVsTextLines pTextBuffer)
    {
      return VSConstants.S_OK;
    }

    #endregion
  }
}
