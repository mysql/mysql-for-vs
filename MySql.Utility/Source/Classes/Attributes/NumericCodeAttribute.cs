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
  /// Represents a custom attribute to set an generic numeric code.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Delegate)]
  public class NumericCodeAttribute : Attribute
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="NumericCodeAttribute"/> class.
    /// </summary>
    /// <param name="numericCode">A generic numeric code.</param>
    public NumericCodeAttribute(int numericCode)
    {
      NumericCode = numericCode;
    }

    /// <summary>
    /// Gets or sets a generic numeric code.
    /// </summary>
    public int NumericCode { get; set; }
  }
}