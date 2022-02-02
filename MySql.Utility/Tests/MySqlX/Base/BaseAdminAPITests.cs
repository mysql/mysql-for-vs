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

namespace MySql.Utility.Tests.MySqlX.Base
{
  public class BaseAdminApiTests : BaseTests
  {
    #region Assert Fail Messages

    /// <summary>
    /// Message to indicate that a method was not found.
    /// </summary>
    public const string METHOD_NOT_FOUND = "Method {0} not found.";

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseAdminApiTests"/> class.
    /// </summary>
    /// <param name="scriptLanguage">The language used for the tests.</param>
    /// <param name="xecutor">The type of class that will run X Protocol statements.</param>
    protected BaseAdminApiTests(ScriptLanguageType scriptLanguage, XecutorType xecutor)
      : base (scriptLanguage, xecutor)
    {
    }

    /// <summary>
    /// Test to validate availability of the Admin API.
    /// </summary>
    [Fact]
    public void ValidateAdminApiAvailability()
    {
      OpenConnection();

      try
      {
        InitXecutor();
        var properties = new BaseAdminAPIProperties(ScriptLanguage);
        Assert.True(!ExecuteQuery(properties.CheckInstanceConfiguration).Contains(properties.MethodDoesNotExist),string.Format(METHOD_NOT_FOUND,properties.CheckInstanceConfiguration));
        Assert.True(!ExecuteQuery(properties.ConfigureLocalInstance).Contains(properties.MethodDoesNotExist),string.Format(METHOD_NOT_FOUND,properties.ConfigureLocalInstance));
        Assert.True(!ExecuteQuery(properties.CreateLocalCluster).Contains(properties.MethodDoesNotExist),string.Format(METHOD_NOT_FOUND,properties.CreateLocalCluster));
        Assert.True(!ExecuteQuery(properties.DeleteSandboxInstance).Contains(properties.MethodDoesNotExist),string.Format(METHOD_NOT_FOUND,properties.DeleteSandboxInstance));
        Assert.True(!ExecuteQuery(properties.DeploySandboxInstance).Contains(properties.MethodDoesNotExist),string.Format(METHOD_NOT_FOUND,properties.DeploySandboxInstance));
        Assert.True(!ExecuteQuery(properties.DropMetadataSchema).Contains(properties.MethodDoesNotExist),string.Format(METHOD_NOT_FOUND,properties.DropMetadataSchema));
        Assert.True(!ExecuteQuery(properties.GetCluster).Contains(properties.MethodDoesNotExist),string.Format(METHOD_NOT_FOUND,properties.GetCluster));
        Assert.True(!ExecuteQuery(properties.Help).Contains(properties.MethodDoesNotExist),string.Format(METHOD_NOT_FOUND,properties.Help));
        Assert.True(!ExecuteQuery(properties.KillSandboxInstance).Contains(properties.MethodDoesNotExist),string.Format(METHOD_NOT_FOUND,properties.KillSandboxInstance));
        Assert.True(!ExecuteQuery(properties.RebootClusterFromCompleteOutage).Contains(properties.MethodDoesNotExist),string.Format(METHOD_NOT_FOUND,properties.RebootClusterFromCompleteOutage));
        Assert.True(!ExecuteQuery(properties.ResetSession).Contains(properties.MethodDoesNotExist),string.Format(METHOD_NOT_FOUND,properties.ResetSession));
        Assert.True(!ExecuteQuery(properties.StartSandboxInstance).Contains(properties.MethodDoesNotExist),string.Format(METHOD_NOT_FOUND,properties.StartSandboxInstance));
        Assert.True(!ExecuteQuery(properties.StopSandboxInstance).Contains(properties.MethodDoesNotExist),string.Format(METHOD_NOT_FOUND,properties.StopSandboxInstance));
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
