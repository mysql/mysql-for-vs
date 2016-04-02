// Copyright © 2015, Oracle and/or its affiliates. All rights reserved.
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
using System.Data.Common;
using System.Linq;
using MySql.Data.MySqlClient;
using MySql.Data.VisualStudio.Editors;
using MySqlX;
using MySqlX.Shell;

namespace MySql.Data.VisualStudio.MySqlX
{
  public class MySqlXProxy
  {
    /// <summary>
    /// Stores the value that specifies if all the statements will be executed in the same session
    /// </summary>
    private bool _keepSession;

    /// <summary>
    /// Shell object used to execute the queries through the shell
    /// </summary>
    ShellClient _shellClient;

    /// <summary>
    /// Variable to store the connection string for the wrapper.
    /// </summary>
    private string _connString;

    /// <summary>
    /// Statement used to clean the session in use
    /// </summary>
    private const string _cleanSession = "session.close()";

    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlXProxy"/> class.
    /// </summary>
    /// <param name="connectionString">Connection string that will be used when a script is executed. Format: "user:pass@server:port"</param>
    /// <param name="keepXSession">Specifies if all the statements will be executed in the same session</param>
    public MySqlXProxy(string connectionString, bool keepXSession)
    {
      _connString = connectionString;
      _keepSession = keepXSession;
      if (keepXSession)
      {
        _shellClient = new ShellClient();
        _shellClient.MakeConnection(_connString);
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlXProxy"/> class.
    /// </summary>
    /// <param name="connection">Connection object that will be used."</param>
    /// /// <param name="keepXSession">Specifies if all the statements will be executed in the same session</param>
    public MySqlXProxy(DbConnection connection, bool keepXSession)
    {
      _connString = ((MySqlConnection)connection).ToNgFormat();
      _keepSession = keepXSession;

      if (keepXSession)
      {
        _shellClient = new ShellClient();
        _shellClient.MakeConnection(_connString);
      }
    }

    /// <summary>
    /// Executes a JavaScript or Python query using the X Shell
    /// </summary>
    /// <param name="script">The script to execute</param>
    /// <param name="scriptType">The type of language used.</param>
    /// <returns>Returns an empty list of dictionary objects if the result returned from the server doesnt belong to the BaseResult hierarchy</returns>
    public List<Dictionary<string, object>> ExecuteScript(string script, ScriptType scriptType)
    {
      if (string.IsNullOrEmpty(script))
      {
        return null;
      }

      object result = ExecuteScript(new[] { script }, scriptType).First();
      var baseResult = result as BaseResult;
      return baseResult != null
        ? BaseResultToDictionaryList(baseResult)
        : new List<Dictionary<string, object>>();
    }

    /// <summary>
    /// Forces the convertion of the the inputResult object to a document inputResult.
    /// </summary>
    /// <param name="inputResult">The inputResult object.</param>
    /// <returns>A list of dictionary objects.</returns>
    private List<Dictionary<string, object>> BaseResultToDictionaryList(BaseResult inputResult)
    {
      if (inputResult is Result)
      {
        var result = inputResult as Result;
        ExecutionResult = string.Format("Script executed in {0}. Affected Rows: {1} - Warnings: {2}.", result.GetExecutionTime(), result.GetAffectedItemCount(), result.GetWarningCount());
      }
      else if (inputResult is RowResult)
      {
        var rowResult = inputResult as RowResult;
        ExecutionResult = string.Format("Script executed in {0}. Affected Rows: {1} - Warnings: {2}.", rowResult.GetExecutionTime(), rowResult.FetchAll().Count, rowResult.GetWarningCount());
        return RowResultToDictionaryList(rowResult);
      }
      else if (inputResult is DocResult)
      {
        var docResult = inputResult as DocResult;
        ExecutionResult = string.Format("Script executed in {0}. Affected Rows: {1} - Warnings: {2}.", docResult.GetExecutionTime(), docResult.FetchAll().Count, docResult.GetWarningCount());
        return docResult.FetchAll();
      }

      return new List<Dictionary<string, object>>();
    }

    /// <summary>
    /// Execute a list of javascript commands using the NG Shell
    /// </summary>
    /// <param name="script">The script to execute.</param>
    /// <param name="scriptType">Indicates the script mode, default is JavaScript.</param>
    /// <returns>A list of objects returned by each of the command executed.</returns>
    public List<object> ExecuteScript(string[] script, ScriptType scriptType)
    {
      if (script == null || script.Length <= 0)
      {
        return null;
      }

      switch (scriptType)
      {
        case ScriptType.Sql:
          return ExecuteQuery(Mode.SQL, script);
        case ScriptType.Python:
          return ExecuteQuery(Mode.Python, script);
        case ScriptType.JavaScript:
        default:
          return ExecuteQuery(Mode.JScript, script);
      }
    }

    /// <summary>
    /// Execute a sql command using the NG Shell
    /// </summary>
    /// <param name="script">Script to execute</param>
    /// <returns>RowResult returned from the server</returns>
    public RowResult ExecuteSqlQuery(string script)
    {
      if (string.IsNullOrEmpty(script))
      {
        return null;
      }

      Object result = ExecuteQuery(Mode.SQL, script).First();
      ExecutionResult = string.Format("Script executed in {0}. Affected Rows: {1} - Warnings: {2}.",
        ((BaseResult)result).GetExecutionTime(),
        result is RowResult ? ((RowResult)result).FetchAll().Count : 0,
        ((BaseResult)result).GetWarningCount());

      return result as RowResult;
    }

    /// <summary>
    /// Execute a command using the NG Shell
    /// </summary>
    /// <param name="mode">Mode that will be used to execute the script received</param>
    /// <param name="statements">Statements to execute</param>
    /// <returns>A list of objects returned from the server for each query executed.</returns>
    private List<object> ExecuteQuery(Mode mode, params string[] statements)
    {
      ExecutionResult = "";
      List<object> result = new List<object>();

      if (!_keepSession)
      {
        _shellClient = new ShellClient();
        _shellClient.MakeConnection(_connString);
      }

      _shellClient.SwitchMode(mode);
      foreach (string statement in statements)
      {
        if (string.IsNullOrEmpty(statement))
        {
          continue;
        }

        result.Add(_shellClient.Execute(statement));
      }

      if (!_keepSession)
      {
        CleanConnection();
      }

      return result;
    }

    /// <summary>
    /// This property will contains a message about the script executed when the script doesn't return a resultset
    /// </summary>
    public string ExecutionResult
    {
      private set;
      get;
    }

    /// <summary>
    /// Parse a RowResult object to a Dictionary list
    /// </summary>
    /// <param name="rowResult">Object to parse</param>
    /// <returns>Null if invalid data given otherwise parsed data</returns>
    public List<Dictionary<string, object>> RowResultToDictionaryList(RowResult rowResult)
    {
      if (rowResult == null)
      {
        return null;
      }

      List<Dictionary<string, object>> parsedData = new List<Dictionary<string, object>>();
      List<string> metaData = rowResult.GetColumnNames();
      object[][] data = rowResult.FetchAll().ToArray();

      for (int row = 0; row < data.Length; row++)
      {
        Dictionary<string, object> rowData = new Dictionary<string, object>();
        for (int column = 0; column < metaData.Count; column++)
        {
          rowData.Add(metaData[column], data[row][column]);
        }
        parsedData.Add(rowData);
      }

      return parsedData;
    }

    /// <summary>
    /// Dispose the resources used by the current session
    /// </summary>
    public void CleanConnection()
    {
      if (_shellClient == null)
      {
        return;
      }

      _shellClient.Execute(_cleanSession);
      _shellClient.Dispose();
      _shellClient = null;
    }
  }
}
