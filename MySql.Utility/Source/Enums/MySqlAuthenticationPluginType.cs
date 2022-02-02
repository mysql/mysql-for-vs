/* Copyright (c) 2018, Oracle and/or its affiliates. All rights reserved.

 This program is free software; you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation; version 2 of the License.

 This program is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public License
 along with this program; if not, write to the Free Software
 Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA */

using System.ComponentModel;

namespace MySql.Utility.Enums
{
  /// <summary>
  /// Specifies identifiers to indicate the type of authentication plugin to use when establishing connections to the Server.
  /// </summary>
  public enum MySqlAuthenticationPluginType
  {
    /// <summary>
    /// Not set or unknown.
    /// </summary>
    None,

    /// <summary>
    /// Use MySQL Native Pluggable Authentication.
    /// </summary>
    [Description("mysql_native_password")]
    MysqlNativePassword,

    /// <summary>
    /// Use SHA-256 Pluggable Authentication.
    /// </summary>
    [Description("sha256_password")]
    Sha256Password,

    /// <summary>
    /// Use Caching SHA-2 Pluggable Authentication.
    /// </summary>
    [Description("caching_sha2_password")]
    CachingSha2Password,

    /// <summary>
    /// Use Windows Pluggable Authentication (Commercial-only).
    /// </summary>
    [Description("authentication_windows")]
    Windows
  }
}
