// Copyright (c) 2013, Oracle and/or its affiliates. All rights reserved.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License, version 2.0, as
// published by the Free Software Foundation.
//
// This program is also distributed with certain software (including
// but not limited to OpenSSL) that is licensed under separate terms,
// as designated in a particular file or component or in included license
// documentation.  The authors of MySQL hereby grant you an
// additional permission to link the program and your derivative works
// with the separately licensed software that they have included with
// MySQL.
//
// Without limiting anything contained in the foregoing, this file,
// which is part of MySQL for Visual Studio, is also subject to the
// Universal FOSS Exception, version 1.0, a copy of which can be found at
// http://oss.oracle.com/licenses/universal-foss-exception.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU General Public License, version 2.0, for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Installer.Tests
{
  public class RegressionTests : IUseFixture<SetUpClass>, IDisposable
  {
    private SetUpClass st;
    private bool disposed = false;

    public void SetFixture(SetUpClass data)
    {
      st = data;
    }

    /// <summary>
    /// Checks that the UpdateMachineConfig custom action does not exist in the installer.
    /// </summary>
    [Fact]
    public void UpdateMachineConfigAction()
    {
      string val;
      // The UpdateMachineConfigFile action must not exists neither in table CustomAction nor table InstallExecuteSequence
      st.GetValue("select `Action` from `CustomAction` where `Action` = 'UpdateMachineConfigFile'", out val);
      Assert.Equal(val, null /*"UpdateMachineConfigFile" */);
      st.GetValue("select `Action` from `InstallExecuteSequence` where `Action` = 'UpdateMachineConfigFile'", out val);
      Assert.Equal(val, null /*"UpdateMachineConfigFile" */);
    }

    [Fact]
    public void NoInstallUtilCustomAction()
    {
      string val;
      st.GetValue("select `Action` from `CustomAction` where `Action` = 'ManagedDataInstallSetup'", out val);
      Assert.Equal(val, null);
      st.GetValue("select `Action` from `CustomAction` where `Action` = 'ManagedDataUnInstallSetup'", out val);
      Assert.Equal(val, null);
    }

    public virtual void Dispose()
    {
    }
  }
}
