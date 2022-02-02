// Copyright (c) 2012, 2016, Oracle and/or its affiliates. All rights reserved.
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
using Xunit;

namespace MySql.Utility.Tests.MySqlWorkbench
{
  public class MySqlWorkBenchTests
  {
    [Fact]
    public void CanLoadWorkBenchConnections()
    {
      if (!MySql.Utility.Classes.MySqlWorkbench.MySqlWorkbench.IsInstalled)
      {
        return;
      }

      var connections = MySql.Utility.Classes.MySqlWorkbench.MySqlWorkbench.Connections;
      foreach (var item in connections)
      {
        Assert.False(string.IsNullOrEmpty(item.Host));
        Assert.False(string.IsNullOrEmpty(item.HostIdentifier));
      }
    }

    [Fact]
    public void VerifiedWorkbenchVersion()
    {
      if (!MySql.Utility.Classes.MySqlWorkbench.MySqlWorkbench.IsInstalled)
      {
        return;
      }

      Assert.True(new Version(MySql.Utility.Classes.MySqlWorkbench.MySqlWorkbench.ProductVersion) >= new Version("5.2.40.0"));
    }
  }
}
