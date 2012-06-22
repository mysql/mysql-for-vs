// Copyright © 2004, 2012, Oracle and/or its affiliates. All rights reserved.
//
// MySQL Connector/NET is licensed under the terms of the GPLv2
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
using System.Linq;
using System.Text;
using MySql.Debugger;
using MySql.Data.MySqlClient;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

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
    //private AD7Breakpoint _breakpoint;
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

    public delegate void EndProgram();
    public event EndProgram OnEndProgram;
    public void RaiseEndProgram()
    {
      try
      {
        //if (!string.IsNullOrWhiteSpace(_SpBody))
        //{
        //  using (MySqlConnection conn = new MySqlConnection(_connection.ConnectionString))
        //  {
        //    conn.Open();
        //    MySqlScript script = new MySqlScript(conn, string.Format("delimiter // drop procedure if exists {0}; {1} //", _SpName, _SpBody));
        //    script.Execute();
        //  }
        //}
        // Restores all routines instrumented as per last debug session.
        this._debugger.RestoreRoutinesBackup();
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.ToString());
      }
      if (OnEndProgram != null)
        OnEndProgram();
    }

    private DebuggerManager(AD7Events events, AD7ProgramNode node, AD7Breakpoint breakpoint)
    {
      _breakpoints = new Dictionary<Breakpoint, AD7Breakpoint>(new BreakpointDictionaryComparer());
      _events = events;
      _node = node;
      _breakpoint = breakpoint;
      _debugger = new Debugger();
      _autoRE = new AutoResetEvent(false);
      _debugger.OnEndDebugger += RaiseEndProgram;

      if (string.IsNullOrEmpty(_node.ConnectionString))
      {
        throw new Exception( "Debugger expected a non-null connection string" );
      }
      
      _connection = new MySqlConnection(_node.ConnectionString);
      _debugger.Connection = new MySqlConnection(_connection.ConnectionString);
      _debugger.UtilityConnection = new MySqlConnection(_connection.ConnectionString);
      _debugger.LockingConnection = new MySqlConnection(_connection.ConnectionString);
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

      if (args.Count == 0) return null;
      StoredRoutineArgumentsDlg dlg = new StoredRoutineArgumentsDlg();
      foreach (StoreType st in args.Values)
      {
        dlg.AddNameValue(st.Name, "");
      }
      dlg.DataBind();
      DialogResult res = dlg.ShowDialog();
      if (res == DialogResult.Cancel)
      {
        return false;
      }
      foreach (NameValue nv in dlg.GetNameValues())
      {
        args[nv.Name].Value = nv.Value;
      }
      return true;
    }

    //internal string GetCurrentScopeFileName()
    //{
    //  if (string.IsNullOrEmpty(_debugger.CurrentScope.FileName))
    //  {
    //    string s = Path.GetTempFileName(); 
    //    File.WriteAllText(s, _debugger.CurrentScope.OwningRoutine.SourceCode );
    //    _debugger.CurrentScope.FileName = s;
    //  }
    //  _node.FileName = _debugger.CurrentScope.FileName;
    //  return _debugger.CurrentScope.FileName;
    //}

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
        Nullable<bool> result = RequestArguments( out args );
        if (result ?? true)
        {
          if (_autoRE != null)
            _autoRE.Close();
          _autoRE = new AutoResetEvent(false);
          _debugger.SqlInput = _SpBody;
          DoRun( args );
        }
        else
        {
          _debugger.RaiseEndDebugger();
        }
      }
      catch (DebugSyntaxException dse)
      {
        throw;
        //MessageBox.Show(dse.Message, "Syntax Error in arguments.");
        return;
      }
    }

    private void DoRun(Dictionary<string, StoreType> args)
    {
      Debugger.BreakpointHandler bph = (bp) =>
      {
        //if (_worker.CancellationPending)
        //{
        //  _debugger.Stop();
        //}
        //// Make UI update.
        //DoGuiUpdate(bp);
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
        // Sync primitives
        _autoRE.WaitOne();
      };
      _debugger.OnBreakpoint -= bph;
      _debugger.OnBreakpoint += bph;
      
      // Setup args
      string[] values = new string[ args.Count ];
      int i = 0;
      foreach (StoreType st in args.Values)
      {
        values[i] = st.Value.ToString();
        i++;
      }
      try
      {
        _debugger.Run( values );
      }
      catch (MySqlException mysqlex)
      {
        throw;
        //MessageBox.Show(string.Format("Error while debugging: ", mysqlex.Message));
      }
      catch (DebugSyntaxException dse)
      {
        throw;
        //MessageBox.Show(string.Format("Syntax error: {0}", dse.Message));
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

    private string tempFileName;

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
            body = reader.GetString(2);
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
