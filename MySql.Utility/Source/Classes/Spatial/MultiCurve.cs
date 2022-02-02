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

using System.Collections.Generic;

namespace MySql.Utility.Classes.Spatial
{
  /// <summary>
  /// A <see cref="GeometryCollection"/> composed of only <see cref="Curve"/> elements.
  /// </summary>
  public abstract class MultiCurve : GeometryCollection
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="MultiCurve"/> class.
    /// </summary>
    /// <param name="curves">An array of <see cref="Curve"/>s that compose the collection.</param>
    /// <param name="srid">The ID of the Spatial Reference System used by this <see cref="MultiCurve"/>.</param>
    protected MultiCurve(Curve[] curves, int srid)
      : this(srid)
    {
      if (curves != null)
      {
        Geometries = new List<Geometry>(curves);
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiCurve"/> class.
    /// </summary>
    /// <param name="curves">A list of <see cref="Curve"/>s that compose the collection.</param>
    /// <param name="srid">The ID of the Spatial Reference System used by this <see cref="MultiCurve"/>.</param>
    protected MultiCurve(List<Curve> curves, int srid)
      : this (srid)
    {
      if (curves != null)
      {
        Geometries = new List<Geometry>(curves);
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiCurve"/> class.
    /// </summary>
    /// <param name="srid">The ID of the Spatial Reference System used by the new <see cref="MultiCurve"/>.</param>
    protected MultiCurve(int srid)
      : base(srid)
    {
    }
  }
}
