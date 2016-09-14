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
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Data;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using MySql.Data.MySqlClient;
using MySql.Data.VisualStudio.Editors;
using MySql.Data.VisualStudio.LanguageService;
using MySql.Data.VisualStudio.Properties;
using MySql.Utility.Classes.MySql;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace MySql.Data.VisualStudio.Nodes
{
  internal class TriggerNode : DocumentNode, IVsTextBufferProvider
  {
    private string _sqlMode;
    private VSCodeEditor _editor;
    string _table;

    public TriggerNode(DataViewHierarchyAccessor hierarchyAccessor, int id) :
      base(hierarchyAccessor, id)
    {
      NodeId = "Trigger";
      NameIndex = 2;
      _editor = new VSCodeEditor((IOleServiceProvider)hierarchyAccessor.ServiceProvider);
      RegisterNode(this);
    }

    #region Properties

    public TableNode ParentTable;

    public override string SchemaCollection
    {
      get { return "triggers"; }
    }

    public override bool Dirty
    {
      get { return _editor.Dirty; }
      protected set { _editor.Dirty = value; }
    }

    #endregion

    public static void CreateNew(DataViewHierarchyAccessor hierarchyAccessor, TableNode parent)
    {
      TriggerNode node = new TriggerNode(hierarchyAccessor, 0);
      node.ParentTable = parent;
      node.Edit();
    }

    public override object GetEditor()
    {
      return _editor;
    }

    public override string GetDropSql()
    {
      return GetDropSql(Name);
    }

    public override string GetSaveSql()
    {
      string sql = ChangeSqlTypeTo(_editor.Text, "CREATE").Trim();
      if (IsNew)
      {
        return sql;
      }
      else
      {
        return string.Format("{0}{1}{2};", GetDropSql(), SEPARATOR, sql);
      }
    }

    private string GetDropSql(string triggerName)
    {
      triggerName = triggerName.Trim('`');
      return string.Format("DROP TRIGGER `{0}`.`{1}`", Database, triggerName);
    }

    private string GetNewTriggerText()
    {
      var sb = new StringBuilder("CREATE TRIGGER ");
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
        _editor.Text = GetNewTriggerText();
        _sqlMode = string.Empty;
      }
      else
      {
        try
        {
          DataTable dt = GetDataTable(string.Format("SHOW CREATE TRIGGER `{0}`.`{1}`",
                  Database, Name));

          _sqlMode = dt.Rows[0][1].ToString();
          string sql = dt.Rows[0][2].ToString();
		      OldObjectDefinition = sql;
          _editor.Text = sql;
          Dirty = false;
          OnDataLoaded();
        }
        catch (Exception ex)
        {
          MySqlSourceTrace.WriteAppErrorToLog(ex, null, Resources.TriggerNode_UnableToLoadObjectError, true);
        }
      }

      _table = GetTargetedTable(_editor.Text);
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
        string sql = _editor.Text.Trim();
        if (!IsNew)
        {
          MakeSureWeAreNotChangingTables(sql);
          CheckSyntax();
        }
        ExecuteSql(GetSaveSql());
        return true;
      }
      catch (Exception ex)
      {
        MySqlSourceTrace.WriteAppErrorToLog(ex, null, Resources.TriggerNode_UnableToSaveObjectError, true);
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
      if (_table != null && newTable != null &&
          newTable.ToLowerInvariant() != _table.ToLowerInvariant())
        throw new InvalidOperationException(
            string.Format(Resources.AlterTriggerOnWrongTable, Name, newTable));
    }

    private string GetTargetedTable(string sql)
    {
      MySqlTokenizer tokenizer = new MySqlTokenizer(sql);
      tokenizer.ReturnComments = false;
      tokenizer.AnsiQuotes = _sqlMode.ToLowerInvariant().Contains("ansi_quotes");
      tokenizer.BackslashEscapes = !_sqlMode.ToLowerInvariant().Contains("no_backslash_escapes");

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
      string sql = _editor.Text.Trim();
      StringBuilder sb;
      LanguageServiceUtil.ParseSql(sql, false, out sb, con.ServerVersion);
      if (sb.Length != 0)
        throw new Exception(string.Format("Syntax Error: {0}", sb));
    }

    private string ChangeSqlTypeTo(string sql, string type)
    {
      int index = sql.IndexOf(' ');
      string startingCommand = sql.Substring(0, index).ToUpperInvariant();
      if (startingCommand != "CREATE" && startingCommand != "ALTER")
        throw new Exception(Resources.UnableToExecuteProcScript);
      return type + sql.Substring(index);
    }

    protected override string GetCurrentName()
    {
      return LanguageServiceUtil.GetRoutineName(_editor.Text);
    }

    #region IVsTextBufferProvider Members

    private IVsTextLines _buffer;

    int IVsTextBufferProvider.GetTextBuffer(out IVsTextLines ppTextBuffer)
    {
      if (_buffer == null)
      {
        Type bufferType = typeof(IVsTextLines);
        Guid riid = bufferType.GUID;
        Guid clsid = typeof(VsTextBufferClass).GUID;
        _buffer = (IVsTextLines)MySqlDataProviderPackage.Instance.CreateInstance(
                             ref clsid, ref riid, typeof(object));
      }
      ppTextBuffer = _buffer;
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
