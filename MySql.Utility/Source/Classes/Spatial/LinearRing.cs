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

using System;
using System.Collections.Generic;

namespace MySql.Utility.Classes.Spatial
{
  /// <summary>
  /// Represents a closed and simple <see cref="LineString"/>.
  /// </summary>
  public class LinearRing : LineString
  {
    #region Constants

    /// <summary>
    /// The minimum number of vertices allowed in a valid non-empty ring.
    /// </summary>
    public const int MINIMUM_VALID_COORDINATES_COUNT = 4;

    #endregion Constants

    /// <summary>
    /// Initializes a new instance of the <see cref="LinearRing"/> class.
    /// </summary>
    /// <param name="srid"></param>
    public LinearRing(int srid)
      : base(srid)
    {
      ValidateConstruction();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinearRing"/> class.
    /// </summary>
    /// <param name="coordinates">The <see cref="Coordinate"/>s that define the <see cref="LinearRing"/>.</param>
    /// <param name="srid">The ID of the Spatial Reference System used by this <see cref="Geometry"/>.</param>
    public LinearRing(Coordinate[] coordinates, int srid)
      : base(coordinates, srid)
    {
      ValidateConstruction();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinearRing"/> class.
    /// </summary>
    /// <param name="coordinates">The <see cref="Coordinate"/>s that define the <see cref="LinearRing"/>.</param>
    /// <param name="srid">The ID of the Spatial Reference System used by this <see cref="Geometry"/>.</param>
    public LinearRing(List<Coordinate> coordinates, int srid)
      : base(coordinates, srid)
    {
      ValidateConstruction();
    }

    #region Properties

    /// <summary>
    /// Gets the area encompassed by the closed <see cref="LinearRing"/>.
    /// </summary>
    public double Area
    {
      get
      {
        if (Coordinates == null || Coordinates.Count < 3)
        {
          return 0.0;
        }

        // Based on the Shoelace formula: http://en.wikipedia.org/wiki/Shoelace_formula
        double sumOfProductsLeftToRight = 0.0;
        double sumOfProductsRightToLeft = 0.0;
        for (int i = 0; i < Coordinates.Count - 1; i++)
        {
          sumOfProductsLeftToRight += Coordinates[i].X * Coordinates[i + 1].Y;
          sumOfProductsRightToLeft += Coordinates[i].Y * Coordinates[i + 1].X;
        }

        return Math.Abs(sumOfProductsLeftToRight - sumOfProductsRightToLeft) / 2.0;
      }
    }

    #endregion Properties

    /// <summary>
    /// Creates a <see cref="LinearRing"/> whose coordinates are in the reverse order of this instance.
    /// </summary>
    /// <returns>A <see cref="LinearRing"/> whose coordinates are in the reverse order of this instance.</returns>
    public new LinearRing Reverse()
    {
      if (IsEmpty)
      {
        return this;
      }

      var reversedCoordinates = new List<Coordinate>(Coordinates);
      reversedCoordinates.Reverse();
      return new LinearRing(reversedCoordinates, SRID);
    }

    /// <summary>
    /// Validates that the 
    /// </summary>
    private void ValidateConstruction()
    {
      if (!IsEmpty && !IsClosed)
      {
        throw new Exception("Points of LinearRing do not form a closed LineString.");
      }

      // Empty rings with 0 vertices are valid.
      if (Coordinates.Count >= 1 && CoordinatesCount < MINIMUM_VALID_COORDINATES_COUNT)
      {
        throw new Exception(string.Format("Invalid number of coordinates in LinearRing (found {0} - must be 0 or >= {1}).", Coordinates.Count, MINIMUM_VALID_COORDINATES_COUNT));
      }
    }
  }
}
