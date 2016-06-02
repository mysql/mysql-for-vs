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

namespace MySql.Data.VisualStudio
{
  /// <summary>
  /// Strings that will enable us to identify vs-package components with a unique id.
  /// </summary>
  internal static class GuidStrings
  {
    public const string SQL_EDITOR_FACTORY = "CAA648E8-D6BD-465e-A1B3-2A0BF9DA5581";
    public const string PACKAGE = "79A115C9-B133-4891-9E7B-242509DAD272";
    public const string CMD_SET = "B87CB51F-8A01-4c5e-BF3E-5D0565D5397D";
    public const string PROVIDER = "C6882346-E592-4da5-80BA-D2EADCDA0359";
    public const string DAVINCI_COMMAND_SET = "732ABE75-CD80-11D0-A2DB-00AA00A3EFFF";
    public const string STANDARD_COMMAND_SET = "{5EFC7975-14BC-11CF-9B2B-00AA00573819}";
    public const string IDE_TOOLBAR_CMD_SET = "FD607F05-3661-4E12-A327-6D71AD2E269B";
    public const string MySqlOutputWindowsCmdSet = "6ca7d57d-ae56-4844-a6d9-45d0da3767f4";
    public const string SERVER_EXPLORER_TOOLBAR_CMD_SET = "379E1B3D-A0E1-4D1D-97E9-CE04114BD345";
  }

  /// <summary>
  /// Guid objects created from the guid strings defined in this same file and linked to objects.
  /// </summary>
  internal static class GuidList
  {
    public static readonly Guid Package = new Guid(GuidStrings.PACKAGE);
    public static readonly Guid Provider = new Guid(GuidStrings.PROVIDER);
    public static readonly Guid CmdSet = new Guid(GuidStrings.CMD_SET);
    public static readonly Guid SqlEditorFactoryGuid = new Guid(GuidStrings.SQL_EDITOR_FACTORY);
    public static readonly Guid DavinciCommandSet = new Guid(GuidStrings.DAVINCI_COMMAND_SET);
    public static readonly Guid StandardCommandSet = new Guid(GuidStrings.STANDARD_COMMAND_SET);
    public static readonly Guid GuidIdeToolbarCmdSet = new Guid(GuidStrings.IDE_TOOLBAR_CMD_SET);
    public static readonly  Guid ServerExplorerToolbarCmdSet = new Guid(GuidStrings.SERVER_EXPLORER_TOOLBAR_CMD_SET);
    public static readonly Guid GuidMySqlOutputWindowsCmdSet = new Guid(GuidStrings.MySqlOutputWindowsCmdSet);
  };
}