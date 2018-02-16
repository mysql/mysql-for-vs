// Copyright © 2014, 2018, Oracle and/or its affiliates. All rights reserved.
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


using System.Diagnostics;
using System.IO;
using Xunit;

namespace MySql.VisualStudio.Tests
{
  public static class Dependencies
  {
    public const string C_NET_VERSIONV40 = "6.9.8.0";
    public const string C_NET_VERSIONV45= "6.10.6.0";
  }

  public class DependenciesTests
  {
    [Fact]
    public void IsUsingCorrectVersion()
    {
      var pathAssemblyv40 = Path.GetFullPath(@"..\..\..\..\..\Dependencies\v4.0\Release");
      var pathAssemblyv45 = Path.GetFullPath(@"..\..\..\..\..\Dependencies\v4.5\Release");

      if (File.Exists(pathAssemblyv40 + @"\MySql.Data.Entity.dll"))
      {
        var efAssemblyVersion = FileVersionInfo.GetVersionInfo(pathAssemblyv40 + @"\MySql.Data.Entity.dll");
        string version = efAssemblyVersion.FileVersion;
        Assert.True(Dependencies.C_NET_VERSIONV40 == version);
      }
      else
        Assert.True(false, "MySql.Data.Entity.dll for v4.0 was not found on the expected path");


      if (File.Exists(pathAssemblyv45 + @"\MySql.Data.Entity.EF6.dll"))
      {
        var efAssemblyVersion = FileVersionInfo.GetVersionInfo(pathAssemblyv45 + @"\MySql.Data.Entity.EF6.dll");
        string version = efAssemblyVersion.FileVersion;
        Assert.True(Dependencies.C_NET_VERSIONV45 == version);
      }
      else
        Assert.True(false, "MySql.Data.Entity.EF6.dll was not found on the expected path");
    }
  }
}
