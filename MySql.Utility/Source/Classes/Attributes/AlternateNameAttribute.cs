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

namespace MySql.Utility.Classes.Attributes
{
  /// <summary>
  /// Represents a custom attribute for properties to set an alternate name.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
  public class AlternateNameAttribute : Attribute
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="AlternateNameAttribute"/> class.
    /// </summary>
    /// <param name="name">An alternate name.</param>
    public AlternateNameAttribute(string name)
    {
      Name = name;
    }

    /// <summary>
    /// Gets or sets an alternate name.
    /// </summary>
    public string Name { get; set; }
  }
}