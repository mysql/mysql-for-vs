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
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.InteropServices;
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
using MySql.Utility.Classes;
using MySql.Utility.Classes.MySql;
using MySql.Utility.Forms;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace MySql.Data.VisualStudio.Nodes
{
  class StoredProcedureNode : DocumentNode, IVsTextBufferProvider
  {
    private string _sqlMode;
    private bool _isFunction;
    private VSCodeEditor _editor;

    public StoredProcedureNode(DataViewHierarchyAccessor hierarchyAccessor, int id, bool isFunc) :
      base(hierarchyAccessor, id)
    {
      NodeId = isFunc ? "StoredFunction" : "StoredProcedure";
      _isFunction = isFunc;
      NameIndex = 3;
      _editor = new VSCodeEditor((IOleServiceProvider)hierarchyAccessor.ServiceProvider);
      if( Dte == null )
        Dte = (EnvDTE.DTE)hierarchyAccessor.ServiceProvider.GetService(typeof(EnvDTE.DTE));
      RegisterNode(this);
    }

    #region Properties

    public override string SchemaCollection
    {
      get { return "procedures"; }
    }

    public override bool Dirty
    {
      get { return _editor.Dirty; }
      protected set { _editor.Dirty = value; }
    }

    private bool IsFunction
    {
      get { return NodeId.ToLowerInvariant() == "storedfunction"; }
    }

    #endregion

    public static void CreateNew(DataViewHierarchyAccessor hierarchyAccessor, bool isFunc)
    {
      StoredProcedureNode node = new StoredProcedureNode(hierarchyAccessor, 0, isFunc);
      RegisterNode( node );
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
      if (IsNew)
      {
        return _editor.Text;
      }
      else
      {
        return string.Format("{0}{1}{2};", GetDropSql(), SEPARATOR, _editor.Text);
      }
    }

    private string GetDropSql(string procName)
    {
      procName = procName.Trim('`');
      return string.Format("DROP {0} `{1}`.`{2}`",
          IsFunction ? "FUNCTION" : "PROCEDURE", Database, procName);
    }

    private string GetNewRoutineText()
    {
      StringBuilder sb = new StringBuilder("CREATE ");
      sb.AppendFormat("{0} {1}()\r\n", _isFunction ? "FUNCTION" : "PROCEDURE", Name);
      sb.Append("/*\r\n(\r\n");
      sb.Append("parameter1 INT\r\nOUT parameter2 datatype\r\n");
      sb.Append(")\r\n*/\r\n");
      if (_isFunction)
        sb.Append("RETURNS /* datatype */\r\n");
      sb.Append("BEGIN\r\n");
      if (_isFunction)
        sb.Append("RETURN /* return value */\r\n");
      sb.Append("END");
      return sb.ToString();
    }

    protected override void Load()
    {
      if (IsNew)
        _editor.Text = GetNewRoutineText();
      else
      {
        _editor.Text = OldObjectDefinition = GetRoutineBody();
        Dirty = false;
      }
    }

    private string GetRoutineBody()
    {
      string sql = "";
      try
      {
        sql = GetStoredProcedureBody(string.Format(
            "SHOW CREATE {0} `{1}`.`{2}`",
            IsFunction ? "FUNCTION" : "PROCEDURE", Database, Name), out _sqlMode);
      }
      catch (Exception ex)
      {
        MySqlSourceTrace.WriteAppErrorToLog(ex, Resources.MessageBoxErrorTitle, Resources.StoredProcedureNode_StoredProcedureLoadError, true);
        Debug.WriteLine(ex.Message);
      }
      return sql;
    }

    public override void LaunchDebugger()
    {
      LaunchDebugTarget();
    }

    private string GetStoredProcedureBody(string sql, out string sqlMode)
    {
      DbConnection conn = AcquireHierarchyAccessorConnection();
      try
      {
        DbCommand cmd = MySqlProviderObjectFactory.Factory.CreateCommand();
        cmd.Connection = conn;
        cmd.CommandText = sql;
        string body;
        using (DbDataReader reader = cmd.ExecuteReader())
        {
          reader.Read();
          sqlMode = reader.GetString(1);
          body = reader.GetString(2);
        }

        return body;
      }
      finally
      {
        ReleaseHierarchyAccessorConnection();
      }
    }

    /// <summary>
    /// We override save here so we can change the sql from create to alter on
    /// first save
    /// </summary>
    /// <returns></returns>
    protected override bool Save()
    {
      // since MySQL doesn't support altering the body of a proc we have
      // to do some "magic"

      try
      {
        if (!IsNew)
        {
          // first we need to check the syntax of our changes.  THis will throw
          // an exception if the syntax is bad
          CheckSyntax();
        }
        ExecuteSql(GetSaveSql());
        return true;
      }
      catch (Exception ex)
      {
        MySqlSourceTrace.WriteAppErrorToLog(ex, Resources.MessageBoxErrorTitle, Resources.StoredProcedureNode_StoredProcedureSaveError, true);
        return false;
      }
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

    protected void LaunchDebugTarget()
    {
      Microsoft.VisualStudio.Shell.ServiceProvider sp =
           new Microsoft.VisualStudio.Shell.ServiceProvider((IOleServiceProvider)Dte);

      IVsDebugger dbg = (IVsDebugger)sp.GetService(typeof(SVsShellDebugger));

      VsDebugTargetInfo info = new VsDebugTargetInfo();     
      

      info.cbSize = (uint)Marshal.SizeOf(info);
      info.dlo = DEBUG_LAUNCH_OPERATION.DLO_CreateProcess;
      info.bstrExe = Moniker;
      info.bstrCurDir = @"C:\";
      string connectionString = HierarchyAccessor.Connection.ConnectionSupport.ConnectionString + ";Allow User Variables=true;Allow Zero DateTime=true;";
      if (connectionString.IndexOf("password", StringComparison.OrdinalIgnoreCase) == -1)
      {
        var connection = (MySqlConnection)HierarchyAccessor.Connection.GetLockedProviderObject();
        try
        {
          var settings = (MySqlConnectionStringBuilder)connection.GetType().GetProperty("Settings", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(connection, null);
          connectionString += "password=" + settings.Password + ";Persist Security Info=true;";
        }
        finally
        {
          HierarchyAccessor.Connection.UnlockProviderObject();
        }
      }
      info.bstrArg = connectionString;
      info.bstrRemoteMachine = null; // Environment.MachineName; // debug locally
      info.fSendStdoutToOutputWindow = 0; // Let stdout stay with the application.
      info.clsidCustom = new Guid("{EEEE0740-10F7-4e5f-8BC4-1CC0AC9ED5B0}"); // Set the launching engine the sample engine guid
      info.grfLaunch = 0;

      IntPtr pInfo = Marshal.AllocCoTaskMem((int)info.cbSize);
      Marshal.StructureToPtr(info, pInfo, false);

      try
      {
        int result = dbg.LaunchDebugTargets(1, pInfo);
        if (result != 0 && result != VSConstants.E_ABORT)
        {
          throw new ApplicationException("COM error " + result);
        }
      }
      catch (Exception ex)
      {
        InfoDialog.ShowDialog(InfoDialogProperties.GetErrorDialogProperties("Debugger Error", ex.GetBaseException().Message));
      }
      finally
      {
        if (pInfo != IntPtr.Zero)
        {
          Marshal.FreeCoTaskMem(pInfo);
        }
      }
    }
  }
}
