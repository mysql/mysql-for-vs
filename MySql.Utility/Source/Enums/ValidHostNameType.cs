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
  /// The type of hostname that can be specified in host name fields.
  /// </summary>
  [Flags]
  public enum ValidHostNameType
  {
    /// <summary>
    /// No value set.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// A domain name system (DNS) style host name.
    /// </summary>
    DNS = 1 << 0,

    /// <summary>
    /// An Internet Protocol (IP) version 4 host address.
    /// </summary>
    IPv4 = 1 << 1,

    /// <summary>
    /// An Internet Protocol (IP) version 6 host address.
    /// </summary>
    IPv6 = 1 << 2,

    /// <summary>
    /// Any valid host name type (DNS. IPv4 or IPv6).
    /// </summary>
    Any = DNS | IPv4 | IPv6
  }
}
