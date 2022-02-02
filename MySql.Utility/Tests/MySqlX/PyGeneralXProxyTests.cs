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

using MySql.Utility.Enums;
using Xunit;

namespace MySql.Utility.Tests.MySqlX
{
  public class PyGeneralXProxyTests : GeneralTests
  {
    #region Constant values

    /// <summary>
    /// mysql module.
    /// </summary>
    public const string MYSQL_MODULE = "mysql";

    /// <summary>
    /// mysqlx module.
    /// </summary>
    public const string MYSQLX_MODULE = "mysqlx";

    #endregion

    #region Commands

    /// <summary>
    /// Statement to load mysql module.
    /// </summary>
    public const string LOAD_MYSQL_MODULE_CORRECTLY = "from mysqlsh import " + MYSQL_MODULE;

    /// <summary>
    /// Statement that fails to load mysql module.
    /// </summary>
    public const string LOAD_MYSQL_MODULE_INCORRECTLY = "import " + MYSQL_MODULE;

    /// <summary>
    /// Statement to load mysqlx module.
    /// </summary>
    public const string LOAD_MYSQLX_MODULE_CORRECTLY = "from mysqlsh import " + MYSQLX_MODULE;

    /// <summary>
    /// Statement that fails to load mysqlx module.
    /// </summary>
    public const string LOAD_MYSQLX_MODULE_INCORRECTLY = "import " + MYSQLX_MODULE;

    #endregion

    #region Assert Fail Messages

    /// <summary>
    /// Message that indicates failue to load a module.
    /// </summary>
    public const string MODULE_NOT_LOADED = "Module not loaded";

    #endregion

    public PyGeneralXProxyTests()
      : base(ScriptLanguageType.Python)
    {
    }

    /// <summary>
    /// Test that imports mysql/mysqlx modules using the new format.
    /// </summary>
    [Fact]
    public void ImportMySqlModules()
    {
      OpenConnection();

      try
      {
        InitXecutor();

        // Fail to load mysql module.
        Assert.True(ExecuteQuery(LOAD_MYSQL_MODULE_INCORRECTLY).Contains("error"),COMMAND_NOT_DEPRECATED);

        // Fail to load mysql module.
        Assert.True(ExecuteQuery(LOAD_MYSQLX_MODULE_INCORRECTLY).Contains("error"), COMMAND_NOT_DEPRECATED);

        // Load mysql module.
        Assert.True(ExecuteQuery(LOAD_MYSQL_MODULE_CORRECTLY) == string.Empty, MODULE_NOT_LOADED);

        // Load mysqlx module.
        Assert.True(ExecuteQuery(LOAD_MYSQLX_MODULE_CORRECTLY) == string.Empty, MODULE_NOT_LOADED);
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