// Copyright (c) 2014, 2016, Oracle and/or its affiliates. All rights reserved.
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


using System.Diagnostics;
using System.IO;
using Xunit;

namespace MySql.VisualStudio.Tests
{
  public static class Dependencies
  {
    public const string C_NET_VERSIONV40 = "6.7.4.0";
    public const string C_NET_VERSIONV45= "6.8.3.0";
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
