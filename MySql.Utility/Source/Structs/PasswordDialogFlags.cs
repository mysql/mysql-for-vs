// Copyright (c) 2016, 2018, Oracle and/or its affiliates. All rights reserved.
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

using MySql.Utility.Enums;
using MySql.Utility.Forms;

namespace MySql.Utility.Structs
{
  /// <summary>
  /// Specifies flags about password-related operations.
  /// </summary>
  public struct PasswordDialogFlags
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="PasswordDialogFlags"/> struct.
    /// </summary>
    /// <param name="currentPassword">The current password.</param>
    public PasswordDialogFlags(string currentPassword)
    {
      Cancelled = false;
      ConnectionResultType = ConnectionResultType.None;
      NewPassword = null;
      OldPassword = currentPassword;
    }

    /// <summary>
    /// Flag indicating whether the operation (password connection or reset password) was cancelled by the user.
    /// </summary>
    public bool Cancelled;

    /// <summary>
    /// Indicates the result of a connection test.
    /// </summary>
    public ConnectionResultType ConnectionResultType;

    /// <summary>
    /// Gets a value indicating whether the connection was made sucessfully or if the password was just reset by the user.
    /// </summary>
    public bool ConnectionSuccess => ConnectionResultType == ConnectionResultType.ConnectionSuccess || ConnectionResultType == ConnectionResultType.PasswordReset;

    /// <summary>
    /// The new password entered by the user.
    /// </summary>
    public string NewPassword;

    /// <summary>
    /// The original password provided to the <see cref="PasswordDialog"/> .
    /// </summary>
    public string OldPassword;

    /// <summary>
    /// Gets a value indicating whether the connection could not be made because of a wrong password.
    /// </summary>
    public bool WrongPassword => ConnectionResultType == ConnectionResultType.WrongPassword;
  }
}