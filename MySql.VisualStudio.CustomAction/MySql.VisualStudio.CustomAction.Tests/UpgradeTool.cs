// Copyright (c) 2021, Oracle and/or its affiliates.
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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MySql.VisualStudio.CustomAction.Tests
{
  [TestClass]
  public class UpgradeTool
  {
    [TestMethod]
    public void NullValues()
    {
      var mySqlForVisualStudioVersion = new Version(1, 2, 10);
      var internalMySqlDataVersion = new Version(8, 0, 24);
      var installedMySqlDataVersion = new Version(8, 0, 23);
      Assert.IsFalse(CustomActions.IsConfigurationUpdateRequired(null, null, null));
      Assert.IsFalse(CustomActions.IsConfigurationUpdateRequired(mySqlForVisualStudioVersion, null, null));
      Assert.IsFalse(CustomActions.IsConfigurationUpdateRequired(mySqlForVisualStudioVersion, installedMySqlDataVersion, null));
    }

    [TestMethod]
    public void GetUpdateStatus()
    {
      var mySqlForVisualStudioVersion = new Version(1, 2, 10);

      // If specified VS version is not installed.
      Assert.IsTrue(CustomActions.GetPkgdefFileStatus(SupportedVisualStudioVersions.Vs2015, mySqlForVisualStudioVersion) == Enums.PkgdefFileStatus.Unknown);

      // Validate the correct status is returned based on the state of the pkgdef file.
      Assert.IsTrue(CustomActions.ReadPkgdefFileStatus("../../Files/RedirectFromInternalToInstalled.pkgdef", SupportedVisualStudioVersions.Vs2019Enterprise) == Enums.PkgdefFileStatus.RedirectFromInternalToInstalledMySqlDataEntry);
      Assert.IsTrue(CustomActions.ReadPkgdefFileStatus("../../Files/RedirectFromOlderToInternal.pkgdef", SupportedVisualStudioVersions.Vs2019Enterprise) == Enums.PkgdefFileStatus.RedirectFromOlderToInternalMySqlDataEntry);
      Assert.IsTrue(CustomActions.ReadPkgdefFileStatus("../../Files/NoRedirection.pkgdef", SupportedVisualStudioVersions.Vs2019Enterprise) == Enums.PkgdefFileStatus.NoBindingRedirectEntries);
    }
  }
}
