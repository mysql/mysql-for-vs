// Copyright (c) 2004, 2019, Oracle and/or its affiliates. All rights reserved.
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
using System.Linq;
using System.Text;
using MySql.Debugger;
using MySql.Data.MySqlClient;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using MySql.Utility.Classes.Logging;

namespace MySql.Debugger.VisualStudio
{
  public class DebuggerManager
  {
    public static DebuggerManager Instance;

    /// <summary>
    /// Reinitializes the instance of debugger manager for a new debugging session.
    /// </summary>
    public static void Init(AD7Events events, AD7ProgramNode node, AD7Breakpoint breakpoint)
    {
      Instance = new DebuggerManager(events, node, breakpoint);
    }

    internal AD7Events _events;
    private AD7ProgramNode _node;
    private Debugger _debugger;
    private AutoResetEvent _autoRE;
    private MySqlConnection _connection;
    private AD7Breakpoint _breakpoint;
    private string _moniker;
    private string _SpBody;
    private string _SpName;
    public AD7Breakpoint Breakpoint { get { return _breakpoint; } set { _breakpoint = value; } }

    public MySqlConnection Connection
    {
      get { return _connection; }
      set { _connection = value; }
    }

    public string Moniker
    {
      get { return _moniker; }
      set
      {
        _moniker = value;
        _SpBody = GetStoredProcedureBody();
      }
    }

    private Dictionary<Breakpoint, AD7Breakpoint> _breakpoints;

    public void BindBreakpoint(AD7Breakpoint ad7bp)
    { 
      Breakpoint bp = this.Debugger.SetBreakpoint(_debugger.CurrentScope.OwningRoutine.SourceCode, ad7bp.LineNumber );
      ad7bp.CoreBreakpoint = bp;
      bp.Disabled = ad7bp.Disabled;
      _breakpoints.Add(ad7bp.CoreBreakpoint, ad7bp);
    }

    public event MySql.Debugger.Debugger.EndDebugger OnEndDebugger { 
      add { _debugger.OnEndDebugger += value; }
      remove { _debugger.OnEndDebugger -= value; } 
    }
    
    public void DoEndProgram()
    {
      try
      {        
        this._debugger.Stop();
        ReleaseCoreDebugger();
      }
      catch (Exception ex)
      {
        Logger.LogException(ex);
      }
    }

    private DebuggerManager(AD7Events events, AD7ProgramNode node, AD7Breakpoint breakpoint)
    {
      _breakpoints = new Dictionary<Breakpoint, AD7Breakpoint>(new BreakpointDictionaryComparer());
      _events = events;
      _node = node;
      _breakpoint = breakpoint;
      _debugger = new Debugger();
      _autoRE = new AutoResetEvent(false);

      if (string.IsNullOrEmpty(_node.ConnectionString))
      {
        var message = "Debugger expected a non-null connection string";
        Logger.LogError(message);
        throw new Exception(message);
      }
      
      _connection = new MySqlConnection(_node.ConnectionString);
      _debugger.Connection = new MySqlConnection(_connection.ConnectionString);
      _debugger.UtilityConnection = new MySqlConnection(_connection.ConnectionString);
      _debugger.Connection.Open();
      _debugger.OnStartDebugger += () => { _debugger.CurrentScope.FileName = this.tempFileName; };

      Moniker = _node.FileName;
      _debugger.SqlInput = _SpBody;
    }

    private bool? RequestArguments( out Dictionary<string, StoreType> args )
    {
      // Set initial arguments values.
      string sql = _SpBody;
      args = this._debugger.ParseArgs(sql);

      if (args.Count(i => i.Value.ArgType != ArgTypeEnum.Out) == 0) return null;
      StoredRoutineArgumentsDlg dlg = new StoredRoutineArgumentsDlg();
      foreach (StoreType st in args.Values)
      {
        if (st.ArgType != ArgTypeEnum.Out)
          dlg.AddNameValue(st.Name, "", st.Type + (st.Unsigned ? " unsigned" : string.Empty));
      }
      dlg.DataBind();

      DialogResult res = dlg.ShowDialog(_node.ParentWindow);
      if (res == DialogResult.Cancel)
      {
        return false;
      }
      foreach (NameValue nv in dlg.GetNameValues())
      {
        if (nv.IsNull)
          nv.Value = "NULL";
        args[nv.Name].Value = nv.Value;
      }
      return true;
    }

    public void BreakpointHit()
    {
      _events.BreakpointHit(_breakpoint, _node);
    }

    public void BreakpointHitNull()
    {
      _events.BreakpointHit(null, _node);
    }

    public void Step()
    {
      _events.StepCompleted(_node);
    }

    internal SteppingTypeEnum SteppingType {
      get { return _debugger.SteppingType; }
      set { _debugger.SteppingType = value; }
    }

    private void WriteLocalsBack()
    {
      // TODO: 
      _debugger.CommitLocals();
    }

    private void ReleaseCoreDebugger()
    {
      _autoRE.Set();
    }

    internal void Run()
    {
      if (_debugger.IsRunning)
      { 
        // write values back to locals
        //WriteLocalsBack();
        _autoRE.Set();
        return;
      }
      if (_connection == null)
      {
        //MessageBox.Show("Please set up a database connection first.");
        return;
      }
      // Parse formal arguments
      try
      {
        Dictionary<string, StoreType> args;
        Nullable<bool> result = RequestArguments(out args);
        if (result ?? true)
        {
          if (_autoRE != null)
            _autoRE.Close();
          _autoRE = new AutoResetEvent(false);
          _debugger.SqlInput = _SpBody;
          DoRun(args);
        }
        else
        {
          _debugger.RaiseEndDebugger();
        }
      }
      catch (DebuggerException dse)
      {
        Logger.LogError($"{_node.ParentWindow}. {dse.GetBaseException().Message}", true);
        _debugger.RaiseEndDebugger();
        return;
      }
      catch (Exception ex)
      {
        Logger.LogError($"{_node.ParentWindow}. {ex.GetBaseException().Message} ", true);
        _debugger.RaiseEndDebugger();
        return;
      }
    }

    private void DoRun(Dictionary<string, StoreType> args)
    {
      Debugger.BreakpointHandler bph = (bp) =>
      {
        if (bp.IsFake)
        {
          this.Breakpoint.CoreBreakpoint = bp;
          this.BreakpointHitNull();
        }
        else
        {
          this.Breakpoint = _breakpoints[bp];
          this.Breakpoint.CoreBreakpoint = bp;
          this.BreakpointHit();
        }
        // Sync primitives, wait for user input (like Setp Into, Step Out, etc.)
        _autoRE.WaitOne();
      };
      _debugger.OnBreakpoint -= bph;
      _debugger.OnBreakpoint += bph;
      
      // Setup args
      string[] values = new string[ args.Count ];
      List<string> inOutValues = new List<string>();
      int i = 0;
      foreach (StoreType st in args.Values)
      {
        if (st.ArgType == ArgTypeEnum.In)
        {
          if (st.Value.ToString().Equals("NULL", StringComparison.OrdinalIgnoreCase))
            values[i] = "NULL";
          else
            values[i] = "'" + st.Value.ToString().Trim('\'') + "'";
        }
        else
        {
          values[i] = string.Format("@dbg_var{0}", i );
          if (st.ArgType == ArgTypeEnum.InOut)
            inOutValues.Add(string.Format("{0} = '{1}'", values[i], st.Value.ToString()));
        }
        i++;
      }
      try
      {
      _debugger.Run(values, inOutValues.ToArray());
      }
      catch (ThreadAbortException) { }
      catch (Exception ex)
      {
        Logger.LogError($"Error while debugging: {ex.GetBaseException().Message}", true);
        _events.ProgramDestroyed(_node);
      }
    }

    internal Debugger Debugger { get { return Instance._debugger; } }

    public Dictionary<string, StoreType> ScopeVariables { get { return _debugger.ScopeVariables; } }

    public AD7Breakpoint CurrentBreakpoint { 
      get {
        return _breakpoint;
      } 
    }

    public void SetLocalNewValue(string name, string value)
    {
      _debugger.ScopeVariables[name].Value = value;
      _debugger.CommitLocals();
    }

    private string tempFileName = string.Empty;

    private string GetStoredProcedureBody()
    {
      if (_moniker.StartsWith("mysql://", StringComparison.OrdinalIgnoreCase))
      {
        try
        {
          string[] data = _moniker.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
          _SpName = string.Format("`{0}`.`{1}`", data[2], data[3]);
          string sql = String.Format("SHOW CREATE PROCEDURE " + _SpName);
          string body;

          MySqlCommand cmd = new MySqlCommand();
          cmd.Connection = _connection;
          cmd.CommandText = sql;
          _connection.Open();
          using (MySqlDataReader reader = cmd.ExecuteReader())
          {
            reader.Read();
            if (reader.IsDBNull(2))
              throw new ApplicationException("You do not have sufficient database privileges to debug this routine.");
            body = reader.GetString(2);
            body = body.Replace("\r", string.Empty).Replace("\n", Environment.NewLine);
          }
          int truncPos = body.IndexOf("PROCEDURE", StringComparison.OrdinalIgnoreCase);
          if (truncPos != -1)
          {
            body = "CREATE " + body.Substring(truncPos);
          }

          _node.ProgramContents = MySql.Debugger.Debugger.NormalizeTag(body);
          return body;
        }
        finally
        {
          if (_connection != null && _connection.State != System.Data.ConnectionState.Closed)
            _connection.Close();
        }
      }
      else
      {
        string body = File.ReadAllText(_moniker);
        _node.ProgramContents = body;
        return body;
      }
    }
  }

  public class BreakpointDictionaryComparer : IEqualityComparer<Breakpoint>
  {

    bool IEqualityComparer<Breakpoint>.Equals(Breakpoint x, Breakpoint y)
    {
      if (x == null)
      {
        if (y == null) return true;
        else return false;
      }
      else
      {
        if (y == null) return false;
        else
          return (x.Line == y.Line) && (x.Hash == y.Hash);
      }
    }

    int IEqualityComparer<Breakpoint>.GetHashCode(Breakpoint obj)
    {
      if (obj == null) return 0;
      return unchecked(obj.Hash + obj.Line);
    }
  }
}
