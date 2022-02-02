// Copyright (c) 2016, Oracle and/or its affiliates. All rights reserved.
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

namespace MySql.Utility.Classes.MySqlWorkbench
{
  /// <summary>
  /// Contains data to indicate if a migration was successful and any error message related to it.
  /// </summary>
  public class MigrationResult
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="MigrationResult"/> class.
    /// </summary>
    /// <param name="success">Flag indicating whether the migration was successful or not.</param>
    /// <param name="errorMessage">An error message related to the migration.</param>
    public MigrationResult(bool success, string errorMessage)
    {
      ErrorMessage = errorMessage;
      Success = success;
    }

    /// <summary>
    /// Gets an error message related to the migration.
    /// </summary>
    public string ErrorMessage { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the migration was successful or not.
    /// </summary>
    public bool Success { get; private set; }
  }
}
