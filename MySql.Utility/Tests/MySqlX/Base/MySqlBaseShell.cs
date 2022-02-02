// Copyright © 2015, 2019, Oracle and/or its affiliates. All rights reserved.
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
using System.Text;
using MySql.Utility.Enums;
using MySqlX.Shell;

namespace MySql.Utility.Tests.MySqlX.Base
{
  /// <summary>
  /// Custom direct BaseShell Wrapper Implementation for the current tests
  /// </summary>
  public class MySqlBaseShell : BaseShell
  {
    /// <summary>
    /// Result message after successfully executing a command.
    /// </summary>
    private string _resultMessage;

    /// <summary>
    /// Variable declaration in JavaScript.
    /// </summary>
    private const string VAR_KEYWORD = "var ";

    public string ResultMessage => _resultMessage;

    /// <summary>
    /// Set the additional modules paths.
    /// </summary>
    /// <param name="scriptType">Type of the script.</param>
    public void AppendAdditionalModulePaths(ScriptLanguageType scriptType)
    {
      string modulesPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}{@"\Oracle\MySQL For Visual Studio\modules"}".Replace(@"\", "/");
      switch (scriptType)
      {
        case ScriptLanguageType.Python:
          // Add modules for Python
          Execute("import sys");
          Execute($"sys.path.append('{modulesPath}/python') ");
          Execute($"sys.path.append('{modulesPath}') ");
          break;

        case ScriptLanguageType.JavaScript:
          // Add modules for Javascript
          Execute($"sys.path = ['{modulesPath}/js','{modulesPath}']");
          break;
      }
    }

    /// <summary>
    /// Executes a base query converting it first to JavaScript format.
    /// </summary>
    /// <param name="baseQuery"></param>
    public string ExecuteToJavaScript(string baseQuery)
    {
      _resultMessage = string.Empty;
      Execute(ToJavaScript(baseQuery));
      return _resultMessage;
    }

    /// <summary>
    /// Overrides the Input Method to write and return data in a custom way
    /// </summary>
    /// <param name="text">Text to write</param>
    /// <param name="ret">Value to return</param>
    /// <returns></returns>
    public override bool Input(string text, ref string ret)
    {
      Console.WriteLine(text);
      ret = Console.ReadLine();
      return ret != null;
    }

    /// <summary>
    /// Overrides the Password Method to return data in a custom way
    /// </summary>
    /// <param name="text">Data information</param>
    /// <param name="ret">Data to Return</param>
    /// <returns></returns>
    public override bool Password(string text, ref string ret)
    {
      return Input(text, ref ret);
    }

    /// <summary>
    /// Overrides the Print Method to write data in a custom way
    /// </summary>
    /// <param name="text">Text to write</param>
    public override void Print(string text)
    {
      _resultMessage = text;
      Console.WriteLine(_resultMessage);
    }

    /// <summary>
    /// Overrides the PrintError Method to write data in a custom way
    /// </summary>
    /// <param name="text">Text to write</param>
    public override void PrintError(string text)
    {
      _resultMessage = text;
      Console.WriteLine(@"***ERROR***{0}", _resultMessage);
    }

    /// <summary>
    /// Returns a base Protocol X query that runs in JavaScript, adding var for a variable and semicolon at the end.
    /// </summary>
    /// <param name="statement">Base Protocol X query, language-agnostic.</param>
    /// <returns>A base Protocol X query that runs in JavaScript</returns>
    public string ToJavaScript(string statement)
    {
      if (string.IsNullOrEmpty(statement))
      {
        return string.Empty;
      }

      var queryBuilder = new StringBuilder(statement.Length + 10);
      var dotIndex = statement.IndexOf(".", StringComparison.InvariantCultureIgnoreCase);
      var equalsSignIndex = statement.IndexOf("=", StringComparison.InvariantCultureIgnoreCase);

      if (equalsSignIndex > 0 && equalsSignIndex < dotIndex && !statement.StartsWith(VAR_KEYWORD))
      {
        queryBuilder.Append(VAR_KEYWORD);
      }

      queryBuilder.Append(statement);
      if (!statement.EndsWith(";", StringComparison.InvariantCultureIgnoreCase))
      {
        queryBuilder.Append(";");
      }

      return queryBuilder.ToString();
    }
  }
}
