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

using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using MySql.Data.MySqlClient;
using MySqlX.Shell;

namespace MySql.Data.VisualStudio.Editors
{
  public class NgShellWrapper
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
    /// Creates an instance of NgShellWrapper
    /// </summary>
    /// <param name="connectionString">Connection string that will be used when a script is executed. Format: "user:pass@server:port"</param>
    /// <param name="keepNgSession">Specifies if all the statements will be executed in the same session</param>
    public NgShellWrapper(string connectionString, bool keepNgSession)
    {
      _connString = connectionString;
      _keepSession = keepNgSession;
      if (keepNgSession)
      {
        _shellClient = new ShellClient();
        _shellClient.MakeConnection(_connString);
      }
    }

    /// <summary>
    /// Creates an instance of NgShellWrapper
    /// </summary>
    /// <param name="connection">Connection object that will be used to set the connection string. Format: "user:pass@server:port"</param>
    /// /// <param name="keepNgSession">Specifies if all the statements will be executed in the same session</param>
    public NgShellWrapper(DbConnection connection, bool keepNgSession)
    {
      _connString = ((MySqlConnection)connection).ToNgFormat();
      _keepSession = keepNgSession;
      if (keepNgSession)
      {
        _shellClient = new ShellClient();
        _shellClient.MakeConnection(_connString);
      }
    }

    /// <summary>
    /// Execute a javascript command using the NG Shell
    /// </summary>
    /// <param name="script">The script to execute</param>
    /// <param name="scriptType"></param>
    /// <returns>Null if a string empty query is received, otherwise a document resultset returned from the server</returns>
    public DocumentResultSet ExecuteScript(string script, ScriptType scriptType)
    {
      if (string.IsNullOrEmpty(script))
      {
        return null;
      }

      var result = ExecuteScript(new string[] { script }, scriptType).First();
      return ResultSetToDocumentResult(result);
    }

    /// <summary>
    /// Forces the convertion of the the result object to a document result.
    /// </summary>
    /// <param name="result">The result object.</param>
    /// <returns></returns>
    private DocumentResultSet ResultSetToDocumentResult(ResultSet result)
    {
      TableResultSet tableResult = result as TableResultSet;

      if (tableResult != null)
      {
        return TableResultToDocumentResult(tableResult);
      }

      if ((result as DocumentResultSet) == null)
      {
        ExecutionResult = string.Format("Script executed in {0}. Affected Rows: {1} - Warnings: {2}.", result.GetExecutionTime(), result.GetAffectedRows(), result.GetWarningCount());
      }

      return result as DocumentResultSet;
    }

    /// <summary>
    /// Execute a list of javascript commands using the NG Shell
    /// </summary>
    /// <param name="script">The script to execute.</param>
    /// <param name="scriptType">Indicates the script mode, default is JavaScript.</param>
    /// <returns>A list of ResultSet returned by each of the command executed.</returns>
    public List<ResultSet> ExecuteScript(string[] script, ScriptType scriptType)
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
    /// <returns>Null if a string empty query is received, otherwise a table resultset returned from the server</returns>
    public TableResultSet ExecuteSqlQuery(string script)
    {
      if (string.IsNullOrEmpty(script))
      {
        return null;
      }

      ResultSet result = ExecuteQuery(Mode.SQL, script).First();

      if ((result as TableResultSet) == null)
      {
        ExecutionResult = string.Format("Script executed in {0}. Affected Rows: {1} - Warnings: {2}.", result.GetExecutionTime(), result.GetAffectedRows(), result.GetWarningCount());
      }

      return result as TableResultSet;
    }

    /// <summary>
    /// Execute a command using the NG Shell
    /// </summary>
    /// <param name="mode">Mode that will be used to execute the script received</param>
    /// <param name="script">Script to execute</param>
    /// <returns>A list of resultset with the data returned from the server</returns>
    private List<ResultSet> ExecuteQuery(Mode mode, params string[] statements)
    {
      ExecutionResult = "";
      List<ResultSet> result = new List<ResultSet>();

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
    /// Parse a TableResultSet object to a DocumentResultSet object
    /// </summary>
    /// <param name="tableResult">Object to parse</param>
    /// <returns>Null if invalid data given otherwise parsed data</returns>
    public DocumentResultSet TableResultToDocumentResult(TableResultSet tableResult)
    {
      if (tableResult == null)
      {
        return null;
      }

      List<Dictionary<string, object>> parsedData = new List<Dictionary<string, object>>();
      ResultSetMetadata[] metaData = tableResult.GetMetadata().ToArray();
      object[][] data = tableResult.GetData().ToArray();

      for (int row = 0; row < data.Length; row++)
      {
        Dictionary<string, object> rowData = new Dictionary<string, object>();
        for (int column = 0; column < metaData.Length; column++)
        {
          rowData.Add(metaData[column].GetName(), data[row][column]);
        }
        parsedData.Add(rowData);
      }

      return new DocumentResultSet(parsedData, tableResult.GetAffectedRows(), tableResult.GetWarningCount(), tableResult.GetExecutionTime());
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

  /// <summary>
  /// Class that inherits from the SimpleClientShell to override some methods with custom actions
  /// </summary>
  class ShellClient : SimpleClientShell
  {
    /// <summary>
    /// Write the message received to the output window
    /// </summary>
    /// <param name="text">Text to write</param>
    public override void Print(string text)
    {
      Utils.WriteToOutputWindow(text, Messagetype.Information);
    }

    /// <summary>
    /// Write the error received to the output window
    /// </summary>
    /// <param name="text">Test to write</param>
    public override void PrintError(string text)
    {
      Utils.WriteToOutputWindow(text, Messagetype.Error);
    }
  }
}
