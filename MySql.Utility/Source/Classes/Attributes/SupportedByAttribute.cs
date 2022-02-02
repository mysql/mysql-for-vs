// Copyright (c) 2017, 2019, Oracle and/or its affiliates. All rights reserved.
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
  /// Represents an attribute that describes if something is supported by a consumer program.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Delegate, Inherited = false)]
  public class SupportedByAttribute : Attribute
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="SupportedByAttribute"/> class.
    /// </summary>
    /// <param name="name">The name of the consumer program name.</param>
    public SupportedByAttribute(string name)
    {
      Name = name;
    }

    /// <summary>
    /// Gets the name of the consumer program name.
    /// </summary>
    public string Name { get; private set; }
  }
}
