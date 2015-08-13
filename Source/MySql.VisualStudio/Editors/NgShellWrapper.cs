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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlX.Shell;
using System.Data;
using System.Data.Common;

namespace MySql.Data.VisualStudio.Editors
{
  public class NgShellWrapper
  {
    private bool _keepSession;
    ShellClient _shellClient;

    /// <summary>
    /// Variable to store the connection of the wrapper.
    /// </summary>
    private DbConnection _connection;
    /// <summary>
    /// Variable to store the connection string for the wrapper.
    /// </summary>
    private string _connString;
    /// <summary>
    /// Creates an instance of NgShellWrapper
    /// </summary>
    /// <param name="connectionString">Connection string that will be used when a script is executed. Format: "user:pass@server:port"</param>
    public NgShellWrapper(string connectionString, bool keepNgSession)
    {
      _connString = connectionString;
      _keepSession = keepNgSession;
      _shellClient = new ShellClient();
      _shellClient.MakeConnection(_connString);
    }

    /// <summary>
    /// Creates an instance of NgShellWrapper
    /// </summary>
    /// <param name="connectionString">Connection object that will be used to set the connection string. Format: "user:pass@server:port"</param>
    public NgShellWrapper(DbConnection connection)
    {
      _connection = connection;

      var parameters = _connection.ConnectionString.Split(';');
      Dictionary<string, string> conn = new Dictionary<string, string>();
      foreach (var parameter in parameters)
      {
        var keyAndValue = parameter.Split('=');
        if (keyAndValue.Length > 1)
        {
          var key = keyAndValue[0];
          var value = keyAndValue[1];
          conn.Add(key, value);
        }
      }

      //TODO: find a way to unhardcode this value and obtain it directly from the connection object;
      //It seems the port should be defaulted to 3306 if the connection string doesn't specify it.
      conn.Add("port", "3306");

      _connString =
        conn["user id"] + ":" + //TODO: this value could also come in the form of "user"
        conn["password"] + "@" +
        conn["server"] + ":" +
        conn["port"];
    }

    /// <summary>
    /// Execute a javascript command using the NG Shell
    /// </summary>
    /// <param name="script">The script to execute</param>
    /// <returns>Null if a string empty query is received, otherwise a document resultset returned from the server</returns>
    public DocumentResultSet ExecuteJavaScript(string script)
    {
      if (string.IsNullOrEmpty(script))
      {
        return null;
      }
      ResultSet result = ExecuteQuery(Mode.JScript, script);
      if ((result as DocumentResultSet) == null)
      {
        ExecutionResult = string.Format("Script executed in {0}. Affected Rows: {1} - Warnings: {2}.", result.GetExecutionTime(), result.GetAffectedRows(), result.GetWarningCount());
      }
      return result as DocumentResultSet;
    }

    /// <summary>
    /// Execute a list of javascript commands using the NG Shell
    /// </summary>
    /// <param name="script">The script to execute</param>
    /// <returns>A list of ResultSet returned by each of the command executed.</returns>
    public List<ResultSet> ExecuteJavaScript(string[] script)
    {
      if (script == null || script.Length <= 0)
      {
        return null;
      }

      List<ResultSet> result = new List<ResultSet>();
      for (int counter = 0; counter < script.Length; counter++)
      {
        if (string.IsNullOrEmpty(script[counter]))
        {
          continue;
        }

        result.Add(ExecuteQuery(Mode.JScript, script[counter]));
      }

      return result;
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
      ResultSet result = ExecuteQuery(Mode.SQL, script);
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
    /// <returns>A resultset with the data returned from the server</returns>
    private ResultSet ExecuteQuery(Mode mode, string script)
    {
      ExecutionResult = "";
      if (!_keepSession)
      {
        _shellClient = new ShellClient();
        _shellClient.MakeConnection(_connString);
      }

      _shellClient.SwitchMode(mode);
      ResultSet result = _shellClient.Execute(script);

      if (!_keepSession)
      {
        _shellClient.Dispose();
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
