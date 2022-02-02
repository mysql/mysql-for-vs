// Copyright (c) 2015, 2017, Oracle and/or its affiliates. All rights reserved.
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License as
// published by the Free Software Foundation; version 2 of the
// License.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301  USA

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using MySql.Utility.Enums;
using MySqlX.Shell;

namespace MySql.Utility.Classes.MySqlX
{
  /// <summary>
  /// Proxy class for the <see cref="BaseShell"/>.
  /// </summary>
  public class MySqlXProxy
  {
    #region Constants

    /// <summary>
    /// Statement used to clean the session in use
    /// </summary>
    private const string CLOSE_SESSION_STATEMENT = "session.close()";

    #endregion Constants

    #region Fields

    /// <summary>
    /// Variable to store the connection string for the wrapper.
    /// </summary>
    private string _connString;

    /// <summary>
    /// Stores the value that specifies if all the statements will be executed in the same session
    /// </summary>
    private bool _keepSession;

    /// <summary>
    /// Shell object used to execute the queries through the shell
    /// </summary>
    private BaseShell _baseShell;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlXProxy"/> class.
    /// </summary>
    /// <param name="connectionString">Connection string that will be used when a script is executed. Format: "user:pass@server:port"</param>
    /// <param name="keepXSession">Specifies if all the statements will be executed in the same session</param>
    /// <param name="scriptLanguage">The language of the statement.</param>
    public MySqlXProxy(string connectionString, bool keepXSession, ScriptLanguageType scriptLanguage)
    {
      _connString = connectionString;
      _keepSession = keepXSession;
      if (keepXSession)
      {
        _baseShell = new BaseShell();
        _baseShell.MakeConnection(_connString);
      }

      AppendAdditionalModulePaths(scriptLanguage);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlXProxy"/> class.
    /// </summary>
    /// <param name="connection">Connection object that will be used."</param>
    /// <param name="keepXSession">Specifies if all the statements will be executed in the same session</param>
    /// <param name="scriptLanguage">The language of the statement.</param>
    public MySqlXProxy(DbConnection connection, bool keepXSession, ScriptLanguageType scriptLanguage)
      : this(connection.GetXConnectionString(), keepXSession, scriptLanguage)
    {
    }

    /// <summary>
    /// Dispose the resources used by the current session
    /// </summary>
    public void CleanConnection()
    {
      if (_baseShell == null)
      {
        return;
      }

      _baseShell.Execute(CLOSE_SESSION_STATEMENT);
      _baseShell.Dispose();
      _baseShell = null;
    }

    /// <summary>
    /// Execute a single command using the <see cref="BaseShell"/>.
    /// </summary>
    /// <param name="statement">Single statement to execute.</param>
    /// <param name="scriptLanguage">The language of the statement.</param>
    /// <returns>An object containing the results and execution time returned from the server for the query execution.</returns>
    public string ExecuteQuery(string statement, ScriptLanguageType scriptLanguage)
    {
      if (!_keepSession)
      {
        _baseShell = new BaseShell();
        _baseShell.MakeConnection(_connString);
      }

      var mode = Mode.JScript;
      switch (scriptLanguage)
      {
        case ScriptLanguageType.Sql:
          mode = Mode.SQL;
          break;

        case ScriptLanguageType.Python:
          mode = Mode.Python;
          break;

        case ScriptLanguageType.JavaScript:
          mode = Mode.JScript;
          break;
      }

      _baseShell.SwitchMode(mode);
      string result = _baseShell.Execute(statement);
      if (!_keepSession)
      {
        CleanConnection();
      }

      return result;
    }

    /// <summary>
    /// Executes a JavaScript or Python query using the <see cref="BaseShell"/>, returning processed dictionaries of results and information about them.
    /// </summary>
    /// <param name="statement">The statement to execute.</param>
    /// <param name="scriptLanguage">The language of the statement.</param>
    /// <returns>Returns an empty list of dictionary objects if the result returned from the server doesnt belong to the BaseResult hierarchy</returns>
    public List<Dictionary<string, object>> ExecuteSingleStatementAsResultObject(string statement, ScriptLanguageType scriptLanguage)
    {
      if (string.IsNullOrEmpty(statement))
      {
        return null;
      }

      var results = ExecuteStatementsAsResultObject(new[] { statement }, scriptLanguage);
      return results.FirstOrDefault();
    }

    /// <summary>
    /// Executes an array of statements using the <see cref="BaseShell"/>, returning processed dictionaries of results and information about them.
    /// </summary>
    /// <param name="statements">An array of statements to execute.</param>
    /// <param name="scriptLanguage">The language of the statements.</param>
    /// <returns>A List of MySqlXResult objects containing the returned objects from the server and the execution time for each query executed.</returns>
    public List<List<Dictionary<string, object>>> ExecuteStatementsAsResultObject(string[] statements, ScriptLanguageType scriptLanguage)
    {
      var boxedResults = ExecuteStatementsBaseAsResultObject(statements, scriptLanguage);
      if (boxedResults == null)
      {
        return null;
      }

      var processedResults = new List<List<Dictionary<string, object>>>(boxedResults.Count);
      foreach (var boxedResult in boxedResults)
      {
        string executionTime;
        processedResults.Add(boxedResult.ToDictionariesList(out executionTime));
      }

      return processedResults;
    }

    /// <summary>
    /// Executes an array of statements using the <see cref="BaseShell"/>, returning a raw list of boxed <see cref="BaseShell"/> results.
    /// </summary>
    /// <param name="statements">An array of statements to execute.</param>
    /// <param name="scriptLanguage">The language of the statements.</param>
    /// <returns>A list of objects containing the returned objects from the server and the execution time for each query executed.</returns>
    public List<object> ExecuteStatementsBaseAsResultObject(string[] statements, ScriptLanguageType scriptLanguage)
    {
      if (statements == null || statements.Length <= 0)
      {
        return null;
      }

      switch (scriptLanguage)
      {
        case ScriptLanguageType.Sql:
          return ExecuteQueryAsResultObject(Mode.SQL, statements);

        case ScriptLanguageType.Python:
          return ExecuteQueryAsResultObject(Mode.Python, statements);

        case ScriptLanguageType.JavaScript:
          return ExecuteQueryAsResultObject(Mode.JScript, statements);
      }

      return null;
    }

    /// <summary>
    /// Execute a SQL script using the <see cref="BaseShell"/>
    /// </summary>
    /// <param name="script">Script to execute</param>
    /// <returns>RowResult returned from the server</returns>
    public RowResult ExecuteSqlQuery(string script)
    {
      if (string.IsNullOrEmpty(script))
      {
        return null;
      }

      var result = ExecuteQueryAsResultObject(Mode.SQL, script).FirstOrDefault();
      return result as RowResult;
    }

    /// <summary>
    /// Set the additional modules paths.
    /// </summary>
    /// <param name="scriptType">Type of the script.</param>
    private void AppendAdditionalModulePaths(ScriptLanguageType scriptType)
    {
      string modulesPath = string.Format("{0}{1}", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"\Oracle\MySQL For Visual Studio\modules").Replace(@"\", "/");
      StringBuilder script = new StringBuilder();
      List<string> statements;

      switch (scriptType)
      {
        case ScriptLanguageType.Python:
          // Add modules for Python
          script.AppendFormat("import sys{0}", Environment.NewLine);
          script.AppendFormat("sys.path.append('{0}/python'){1}", modulesPath, Environment.NewLine);
          script.AppendFormat("sys.path.append('{0}'){1}", modulesPath, Environment.NewLine);
          statements = script.ToString().BreakIntoPythonStatements();
          ExecuteStatementsAsResultObject(statements.ToArray(), scriptType);
          break;
        case ScriptLanguageType.JavaScript:
          // Add modules for Javascript
          script.AppendFormat("sys.path.append('{0}/js');", modulesPath);
          script.AppendFormat("sys.path.append('{0}/'); ", modulesPath);
          statements = script.ToString().BreakIntoJavaScriptStatements();
          ExecuteStatementsAsResultObject(statements.ToArray(), scriptType);
          break;
      }
    }

    /// <summary>
    /// Execute a command using the BaseShell
    /// </summary>
    /// <param name="mode">Mode that will be used to execute the script received</param>
    /// <param name="statements">Statements to execute</param>
    /// <returns>A List of MySqlXResult objects containing the results objects returned from the server and the execution time for each query executed.</returns>
    private List<object> ExecuteQueryAsResultObject(Mode mode, params string[] statements)
    {
      var result = new List<object>();
      if (!_keepSession)
      {
        _baseShell = new BaseShell();
        _baseShell.MakeConnection(_connString);
      }

      _baseShell.SwitchMode(mode);
      foreach (string statement in statements)
      {
        if (string.IsNullOrEmpty(statement))
        {
          continue;
        }

        var jsonString = _baseShell.Execute(statement);
        result.Add(ExtensionMethods.ToBaseShellResultObject(jsonString));
      }

      if (!_keepSession)
      {
        CleanConnection();
      }

      return result;
    }

    /// <summary>
    /// Execute a command using the BaseShell
    /// </summary>
    /// <param name="mode">Mode that will be used to execute the script received</param>
    /// <param name="statements">Statements to execute</param>
    /// <returns>A List of MySqlXResult objects containing the results objects returned from the server and the execution time for each query executed.</returns>
    private List<string> ExecuteQuery(Mode mode, params string[] statements)
    {
      var result = new List<string>();
      if (!_keepSession)
      {
        _baseShell = new BaseShell();
        _baseShell.MakeConnection(_connString);
      }

      _baseShell.SwitchMode(mode);
      foreach (string statement in statements)
      {
        if (string.IsNullOrEmpty(statement))
        {
          continue;
        }

        var jsonString = _baseShell.Execute(statement);
        result.Add(jsonString);
      }

      if (!_keepSession)
      {
        CleanConnection();
      }

      return result;
    }
  }
}
