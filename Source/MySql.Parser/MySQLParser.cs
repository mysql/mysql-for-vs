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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using MySql.Utility.Classes;
using Parsers;

namespace MySql.Parser
{
  /// <summary>
  /// Contains methods to check the syntax of SQL scripts.
  /// </summary>
  public class MySqlWbParser : IMySQLParsingDataProvider, IDisposable
  {
    #region Fields

    /// <summary>
    /// A <see cref="MySQLParseService"/> instance containing the logic to actually parse code.
    /// </summary>
    private static MySQLParseService _service;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlWbParser"/> class.
    /// </summary>
    /// <param name="mySqlConnection">A <see cref="MySqlConnection"/> to get the SQL mode from the server.</param>
    public MySqlWbParser(MySqlConnection mySqlConnection)
      : this(mySqlConnection, GetVersion(null))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlWbParser"/> class.
    /// </summary>
    /// <param name="mySqlConnection">A <see cref="MySqlConnection"/> to get the SQL mode from the server.</param>
    /// <param name="version">The Server version.</param>
    public MySqlWbParser(MySqlConnection mySqlConnection, Version version)
    {
      Errors = null;

      try
      {
        // Create the Parser service, with an specific version and getting the MySql mode from the server.
        _service = new MySQLParseService(this, version, mySqlConnection.GetMySqlServerGlobalMode(), string.Empty);
      }
      catch (Exception ex)
      {
        // If cannot create the Parser service, throw a more friendly error message
        throw new Exception(Resources.ParserServiceError, ex);
      }
    }

    #region Properties

    /// <summary>
    /// Gets a list of <see cref="ParseError"/> containing information about each error found by running <see cref="CheckSyntax"/>, such as error message, position and length of the token causing the error.
    /// </summary>
    public List<ParseError> Errors { get; private set; }

    /// <summary>
    /// Gets a list of error messages found by running <see cref="CheckSyntax"/>.
    /// </summary>
    public List<string> ErrorMessages
    {
      get
      {
        if (Errors == null)
        {
          return null;
        }

        var syntaxErrorMessage = Resources.SyntaxErrorMessage;
        var errorMessasges = new List<string>(Errors.Count);
        errorMessasges.AddRange(Errors.Select(error => string.Format(syntaxErrorMessage, error.message, error.position)));
        return errorMessasges;
      }
    }

    /// <summary>
    /// Gets a string containing all error messages found by <see cref="CheckSyntax"/> appended in separate lines.
    /// </summary>
    public string ErrorMessagesInSingleText
    {
      get
      {
        var errorMessages = ErrorMessages;
        if (errorMessages == null)
        {
          return string.Empty;
        }

        var singleErrorTextBuilder = new StringBuilder();
        foreach (var errorMessage in errorMessages)
        {
          singleErrorTextBuilder.AppendLine(errorMessage);
        }

        return singleErrorTextBuilder.ToString().Trim();
      }
    }

    #endregion Properties

    #region IMySQLParsingDataProvider implementation

    public List<Tuple<string, string>> RunQuery(string query)
    {
      return new List<Tuple<string, string>>();
    }

    public void DataRetrievalInProgress(bool busy)
    {
    }

    #endregion IMySQLParsingDataProvider implementation

    /// <summary>
    /// Checks the syntax of the given SQL script.
    /// </summary>
    /// <param name="sqlScript">A SQL script.</param>
    /// <returns><c>true</c> if no errors were found, <c>false</c> otherwise.</returns>
    public bool CheckSyntax(string sqlScript)
    {
      bool noErrors = _service.SyntaxCheck(sqlScript);
      Errors = noErrors
        ? null
        : _service.GetParseErrorsWithOffset(0);
      return noErrors;
    }

    /// <summary>
    /// Frees resources and performs cleanup operations.
    /// </summary>
    public void Dispose()
    {
      GC.SuppressFinalize(_service);
    }

    /// <summary>
    /// Gets a <see cref="Version"/> value from a given string.
    /// </summary>
    /// <param name="versionString">A string containing a value.</param>
    /// <returns>A <see cref="Version"/> value from a given string.</returns>
    private static Version GetVersion(string versionString)
    {
      if (string.IsNullOrEmpty(versionString))
      {
        return new Version(5, 5, 0);
      }

      int i = 0;
      while (i < versionString.Length && (char.IsDigit(versionString[i]) || versionString[i] == '.'))
      {
        i++;
      }

      return new Version(versionString.Substring(0, i));
    }
  }
}
