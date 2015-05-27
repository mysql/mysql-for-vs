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
    public NgShellWrapper(string connectionString)
    {
      _connString = connectionString;
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
      return ExecuteQuery(Mode.JScript, script) as DocumentResultSet;
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
      return ExecuteQuery(Mode.SQL, script) as TableResultSet;
    }

    /// <summary>
    /// Execute a command using the NG Shell
    /// </summary>
    /// <param name="mode">Mode that will be used to execute the script received</param>
    /// <param name="script">Script to execute</param>
    /// <returns>A resultset with the data returned from the server</returns>
    private ResultSet ExecuteQuery(Mode mode, string script)
    {
      ShellClient shellClient = new ShellClient();
      shellClient.MakeConnection(_connString);
      shellClient.SwitchMode(mode);
      ResultSet result = shellClient.Execute(script);
      shellClient.Dispose();
      return result;
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
