// Copyright © 2014, 2016, Oracle and/or its affiliates. All rights reserved.
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
using Xunit;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace MySql.Parser.Tests
{
  public static class Utility
  {
    internal const string DEFAULT_HOST = "localhost";
    internal const string DEFAULT_USER = "test";
    internal const string DEFAULT_PASSWORD = "test";
    internal const string DEFAULT_DATABASE = "sakila";
    internal const string DEFAULT_PORT = "3357";

    public static string ParseSql(string sql)
    {
      return ParseSql(sql, false);
    }

    public static string ParseSql(string sql, bool expectErrors)
    {
      return ParseSql(sql, expectErrors, new Version(5, 7, 12));
    }

    public static string ParseSql(string sql, bool expectErrors, Version version)
    {
      var mySqlConnection = CreateMySqlConnection();
      var mySqlParser = new MySqlWbParser(mySqlConnection, version);
      var noErrors = mySqlParser.CheckSyntax(sql);
      Assert.True((expectErrors && !noErrors) || (!expectErrors && noErrors));
      return mySqlParser.ErrorMessagesInSingleText;
    }

    private static MySqlConnection CreateMySqlConnection()
    {
      // Get conn string from config file
      string host = ConfigurationManager.AppSettings["host"];
      host = !string.IsNullOrEmpty(host) ? host : DEFAULT_HOST;
      string user = ConfigurationManager.AppSettings["user"];
      user = !string.IsNullOrEmpty(user) ? user : DEFAULT_USER;
      string password = ConfigurationManager.AppSettings["password"];
      password = !string.IsNullOrEmpty(password) ? password : DEFAULT_PASSWORD;
      string database = ConfigurationManager.AppSettings["database"];
      database = !string.IsNullOrEmpty(database) ? database : DEFAULT_DATABASE;
      string port = ConfigurationManager.AppSettings["port"];
      port = !string.IsNullOrEmpty(port) ? port : DEFAULT_PORT;
      var conn = new MySqlConnection(string.Format("server={0};uid={1};pwd={2};database={3};port={4}", host, user, password, database, port));
      conn.Open();
      return conn;
    }
  }
}
