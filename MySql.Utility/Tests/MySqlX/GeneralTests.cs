// Copyright © 2017, 2019, Oracle and/or its affiliates. All rights reserved.
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

using MySql.Data.MySqlClient;
using MySql.Utility.Enums;
using MySql.Utility.Tests.MySqlX.Base;
using Xunit;

namespace MySql.Utility.Tests.MySqlX
{
  public class GeneralTests : BaseTests
  {
    #region Commands

    /// <summary>
    /// Estalish classic session for test user.
    /// </summary>
    public const string CONNECT_USING_CLASSIC_SESSION = "\\c -c  test%21%23%24%26%28%29%2A%2B%2C%2F%3A%3B%3D%3F%40%5B%5D:guidev!@{0}:{1}";

    /// <summary>
    /// Establish connection using shell.connect.
    /// </summary>
    public const string CONNECT_USING_SHELL_OBJECT = "shell.connect('{0}:{1}@{2}:{3}')";

    /// <summary>
    /// X session connection.
    /// </summary>
    public const string CREATE_X_SESSION = "\\connect -x {0}:{1}@{2}:{3}";

    /// <summary>
    /// X session connection.
    /// </summary>
    public const string CREATE_NODE_SESSION = "\\connect -n {0}:{1}@{2}:{3}";

    /// <summary>
    /// X session connection.
    /// </summary>
    public const string CREATE_CLASSIC_SESSION = "\\connect -c {0}:{1}@{2}:{3}";

    /// <summary>
    /// Creates test user.
    /// </summary>
    public const string CREATE_USER_WITH_SPECIAL_CHARACTERS = "CREATE USER 'test!#$&()*+,/:;=?@[]'@'localhost' IDENTIFIED BY 'guidev!';";

    /// <summary>
    /// Drop test user.
    /// </summary>
    public const string DROP_USER = "DROP USER 'test!#$&()*+,/:;=?@[]'@'localhost';";

    /// <summary>
    /// Grant privileges to test user.
    /// </summary>
    public const string GRANT_USER_PRIVILEGES = "GRANT ALL PRIVILEGES ON *.* TO 'test!#$&()*+,/:;=?@[]'@'localhost' WITH GRANT OPTION;";

    /// <summary>
    /// Lists stored sessions.
    /// </summary>
    public const string LIST_STORED_SESSIONS = "\\lsconn";

    /// <summary>
    /// Removes a stored session.
    /// </summary>
    public const string REMOVE_STORED_SESSION = "\\rmconn";

    /// <summary>
    /// Saves a stored session.
    /// </summary>
    public const string SAVE_STORED_SESSION = "\\saveconn";

    #endregion

    #region Assert Fail Messages

    /// <summary>
    /// Indicates that a previouslt deprectated command is executable.
    /// </summary>
    public const string COMMAND_NOT_DEPRECATED = "Command \"{0}\" should have been deprecated.";

    /// <summary>
    /// The specified object is missing.
    /// </summary>
    public const string OBJECT_MISSING = "Object {0} is missing.";

    /// <summary>
    /// Message indicating failure to establish a connection.
    /// </summary>
    public const string OPEN_SESSION_FAILED = "Unable to establish connection.";

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="GeneralTests"/> class.
    /// </summary>
    public GeneralTests()
    : base (ScriptLanguageType.JavaScript, XecutorType.XProxy)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GeneralTests"/> class.
    /// </summary>
    /// <param name="type">The type of language used in scripts.</param>
    public GeneralTests(ScriptLanguageType type)
    : base (type, XecutorType.XProxy)
    {
    }

    /// <summary>
    /// Test to validate bugs which do not fit into other unit tests.
    /// </summary>
    [Fact]
    public void CheckBugFixes()
    {
      OpenConnection();

      try
      {
        InitXecutor();

        // Help command not displaying all available global objects.
        var result = ExecuteQuery("\\h");
        Assert.True(result.Contains("dba"),OBJECT_MISSING);
        Assert.True(result.Contains("mysql"),OBJECT_MISSING);
        Assert.True(result.Contains("mysqlx"),OBJECT_MISSING);
        Assert.True(result.Contains("session"),OBJECT_MISSING);
        Assert.True(result.Contains("shell"),OBJECT_MISSING);
      }
      finally
      {
        Command?.Dispose();
        CloseConnection();
        DisposeXecutor();
      }
    }

    /// <summary>
    /// Test to validate connection methods.
    /// </summary>
    [Fact]
    public void Connect()
    {
      OpenConnection();

      try
      {
        InitXecutor();

        // X session type removed, Node session set as default.
        var userName = SetUpDatabaseTestsBase.DEFAULT_USER_NAME;
        var password = SetUpDatabaseTestsBase.DEFAULT_PASSWORD;
        var host = SetUpDatabaseTestsBase.DEFAULT_HOSTNAME;
        Assert.True(ExecuteQuery(string.Format(CREATE_X_SESSION, userName, password, host, SetUpDatabaseTestsBase.DEFAULT_X_PORT)).Contains("error"),COMMAND_NOT_DEPRECATED);
        var result = ExecuteQuery(string.Format(CREATE_CLASSIC_SESSION, userName, password, host, SetUpDatabaseTestsBase.DEFAULT_PORT));
        Assert.True(result.Contains("Session successfully established."),result);
        result = ExecuteQuery(string.Format(CREATE_NODE_SESSION, userName, password, host, SetUpDatabaseTestsBase.DEFAULT_X_PORT));
        Assert.True(result.Contains("Session successfully established."), result);

        // shell.connect command added
        Assert.True(ExecuteQuery(string.Format(CONNECT_USING_SHELL_OBJECT, userName, password, host, SetUpDatabaseTestsBase.DEFAULT_X_PORT)).Contains("Node Session successfully established."), OPEN_SESSION_FAILED);
        Assert.True(ExecuteQuery(string.Format(CONNECT_USING_SHELL_OBJECT, userName, password, host, SetUpDatabaseTestsBase.DEFAULT_PORT)).Contains("Classic Session successfully established."), OPEN_SESSION_FAILED);

        //Character decoding when URI is parsed
        ExecuteQuery(string.Format(CREATE_CLASSIC_SESSION, userName, password, host, SetUpDatabaseTestsBase.DEFAULT_PORT));
        Command = new MySqlCommand(CREATE_USER_WITH_SPECIAL_CHARACTERS, Connection);
        Command.ExecuteNonQuery();
        Command.CommandText = GRANT_USER_PRIVILEGES;
        Command.ExecuteNonQuery();
        Assert.True(ExecuteQuery(string.Format(CONNECT_USING_CLASSIC_SESSION,host,SetUpDatabaseTestsBase.DEFAULT_PORT)).Contains("Session successfully established"),result);
        Command.CommandText = DROP_USER;
        Command.ExecuteNonQuery();
      }
      finally
      {
        Command?.Dispose();
        CloseConnection();
        DisposeXecutor();
      }
    }

    /// <summary>
    /// Checks for any general deprecated commands
    /// </summary>
    [Fact]
    public void ValidateDeprecatedCommands()
    {
      OpenConnection();

      try
      {
        InitXecutor();

        // Stored sessions commands removed, Bug #24949016
        Assert.True(ExecuteQuery(LIST_STORED_SESSIONS).Contains("SyntaxError"),COMMAND_NOT_DEPRECATED);
        Assert.True(ExecuteQuery(SAVE_STORED_SESSION).Contains("SyntaxError"),COMMAND_NOT_DEPRECATED);
        Assert.True(ExecuteQuery(REMOVE_STORED_SESSION).Contains("SyntaxError"),COMMAND_NOT_DEPRECATED);
      }
      finally
      {
        Command?.Dispose();
        CloseConnection();
        DisposeXecutor();
      }
    }
  }
}
