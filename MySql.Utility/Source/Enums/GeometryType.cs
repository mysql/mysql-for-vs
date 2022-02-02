// Copyright (c) 2017, Oracle and/or its affiliates. All rights reserved.
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

namespace MySql.Utility.Enums
{
  /// <summary>
  /// Specifies identifiers to indicate the type of geometry supported by spatial extensions.
  /// </summary>
  public enum GeometryType
  {
    /// <summary>
    /// A Point geometry.
    /// </summary>
    Point = 1,

    /// <summary>
    /// A LineString geometry.
    /// </summary>
    LineString = 2,

    /// <summary>
    /// A Polygon geometry.
    /// </summary>
    Polygon = 3,

    /// <summary>
    /// A MultiPoint geometry.
    /// </summary>
    MultiPoint = 4,

    /// <summary>
    /// A MultiLineString geometry.
    /// </summary>
    MultiLineString = 5,

    /// <summary>
    /// A MultiPolygon geometry.
    /// </summary>
    MultiPolygon = 6,

    /// <summary>
    /// A GeometryCollection geometry.
    /// </summary>
    GeometryCollection = 7
  }
}
