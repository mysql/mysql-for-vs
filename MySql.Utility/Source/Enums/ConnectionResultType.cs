// Copyright (c) 2012, 2018, Oracle and/or its affiliates. All rights reserved.
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

using System.ComponentModel;

namespace MySql.Utility.Enums
{
  /// <summary>
  /// Specifies identifiers to indicate the result of a connection test.
  /// </summary>
  public enum ConnectionResultType
  {
    /// <summary>
    /// No connection attempt was made.
    /// </summary>
    [Description("No connection attempt was made.")]
    None,

    /// <summary>
    /// An error was thrown by the server and was shown to the user.
    /// </summary>
    [Description("An error was thrown by the MySQL server (please see the logs).")]
    ConnectionError,

    /// <summary>
    /// Connection was successful.
    /// </summary>
    [Description("Connection successful.")]
    ConnectionSuccess,

    /// <summary>
    /// The local host is not running.
    /// </summary>
    [Description("MySQL server is not running, a connection cannot be established.")]
    HostNotRunning,

    /// <summary>
    /// Could not connect to the specified MySQL host.
    /// </summary>
    [Description("Could not connect to MySQL, the service is possibly down or the host is unreachable.")]
    HostUnreachable,

    /// <summary>
    /// User name cannot be empty or just contain whitespaces.
    /// </summary>
    [Description("User name cannot be empty or just contain whitespaces.")]
    InvalidUserName,

    /// <summary>
    /// The password of the current user has expired and must be reset.
    /// </summary>
    [Description("The password of the current user has expired and must be reset.")]
    PasswordExpired,

    /// <summary>
    /// The password of the current user has been reset.
    /// </summary>
    [Description("The password of the current user has been reset.")]
    PasswordReset,

    /// <summary>
    /// Could not connect to the MySQL host with the specified password for the current user.
    /// </summary>
    [Description("Could not connect to MySQL with the given password.")]
    WrongPassword
  }
}
