// Copyright (c) 2019, Oracle and/or its affiliates. All rights reserved.
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

namespace MySql.Utility.Enums
{
  /// <summary>
  /// Filter for notifications reported by <see cref="MySql.Utility.Classes.RegistryMonitor"/>.
  /// </summary>
  [Flags]
  public enum RegistryChangeNotifyFilter
  {
    /// <summary>
    /// No filter.
    /// </summary>
    None = 0,

    /// <summary>
    /// Notify the caller if a sub key is added or deleted.
    /// </summary>
    Key = 1 << 0,

    /// <summary>
    /// Notify the caller of changes to the attributes of the key, such as the security descriptor information.
    /// </summary>
    Attribute = 1 << 1,

    /// <summary>
    /// Notify the caller of changes to a value of the key.
    /// This can include adding or deleting a value, or changing an existing value.
    /// </summary>
    Value = 1 << 2,

    /// <summary>
    /// Notify the caller of changes to the security descriptor of the key.
    /// </summary>
    Security = 1 << 3,

    /// <summary>
    /// Any of the valid filters.
    /// </summary>
    Any = Key | Attribute | Value | Security
  }
}
