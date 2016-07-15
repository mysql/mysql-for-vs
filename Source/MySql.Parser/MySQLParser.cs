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
using System.Diagnostics;
using System.Text;
using MySql.Data.MySqlClient;
using Parsers;

namespace MySql.Parser
{
  public class MySqlWbParser : IMySQLParsingDataProvider
  {
    private static MySQLParseService _service;
    private const string MYSQL_CASESENSITIVITY_COMMAND = "lower_case_table_names";
    private const string MYSQL_MODE_COMMAND = "GLOBAL.sql_mode";

    public MySqlWbParser(MySqlConnection mySqlConnection)
    {
      try
      {
        // Create the Parser service, getting the MySql mode and version from the server.
        _service = MySQLParseService.CreateServiceW(this, GetVersion(null),
          GetMySqlMode(mySqlConnection), GetMySqlCaseSensitivity(mySqlConnection));
      }
      catch (Exception ex)
      {
        // If cannot create the Parser service, throw a more friendly error message
        throw new Exception(Resources.ParserServiceError, ex);
      }
    }

    public MySqlWbParser(MySqlConnection mySqlConnection, Version version)
    {
      try
      {
        // Create the Parser service, with an specific version and getting the MySql mode from the server.
        _service = MySQLParseService.CreateServiceW(this, version,
          GetMySqlMode(mySqlConnection), GetMySqlCaseSensitivity(mySqlConnection));
      }
      catch (Exception ex)
      {
        // If cannot create the Parser service, throw a more friendly error message
        throw new Exception(Resources.ParserServiceError, ex);
      }
    }

    public List<Tuple<string, string>> RunQuery(string query)
    {
      return new List<Tuple<string, string>>();
    }

    public string CheckSyntax(string query)
    {
      if (_service.SyntaxCheck(query))
      {
        return string.Empty;
      }

      StringBuilder errors = new StringBuilder();
      List<ParseError> errorsList = _service.GetParseErrorsWithOffset(0);
      foreach (ParseError error in errorsList)
      {
        errors.AppendLine(error.message);
      }

      return errors.ToString();
    }

    private Version GetVersion(string versionString)
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

    private string GetMySqlMode(MySqlConnection mySqlConnection)
    {
      if (mySqlConnection == null)
      {
        return string.Empty;
      }

      try
      {
        string sql = string.Format("SELECT @@{0};", MYSQL_MODE_COMMAND);
        var cmdResult = MySqlHelper.ExecuteScalar(mySqlConnection, sql);
        return cmdResult != null ? cmdResult.ToString() : string.Empty;
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex.Message);
        return string.Empty;
      }
    }

    private bool GetMySqlCaseSensitivity(MySqlConnection mySqlConnection)
    {
      if (mySqlConnection == null)
      {
        return false;
      }

      try
      {
        string sql = string.Format("SELECT @@{0};", MYSQL_CASESENSITIVITY_COMMAND);
        var cmdResult = MySqlHelper.ExecuteScalar(mySqlConnection, sql);
        return cmdResult != null && cmdResult.ToString().Equals("1");
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex.Message);
        return false;
      }
    }
  }
}
