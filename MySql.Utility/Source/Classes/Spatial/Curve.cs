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
using System.Linq;
using System.Text;

namespace MySql.Utility.Classes.Spatial
{
  /// <summary>
  /// Represents one-dimensional objects.
  /// </summary>
  public abstract class Curve : Geometry
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="Curve"/> class.
    /// </summary>
    /// <param name="srid">The ID of the Spatial Reference System used by the new <see cref="Curve"/>.</param>
    protected Curve(int srid)
      : base(srid)
    {
      Coordinates = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LineString"/> class.
    /// </summary>
    /// <param name="coordinates">The <see cref="Coordinate"/>s that define the <see cref="LineString"/>.</param>
    /// <param name="srid">The ID of the Spatial Reference System used by the new <see cref="Curve"/>.</param>
    protected Curve(Coordinate[] coordinates, int srid)
      : this(srid)
    {
      if (coordinates == null)
      {
        return;
      }

      if (coordinates.Length < 2)
      {
        throw new ArgumentOutOfRangeException(nameof(coordinates), Resources.CurveInvalidNumberOfCoordinatesError);
      }

      Coordinates = new List<Coordinate>(coordinates);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LineString"/> class.
    /// </summary>
    /// <param name="coordinates">The <see cref="Coordinate"/>s that define the <see cref="LineString"/>.</param>
    /// <param name="srid">The ID of the Spatial Reference System used by the new <see cref="Curve"/>.</param>
    protected Curve(List<Coordinate> coordinates, int srid)
      : this(srid)
    {
      if (coordinates == null)
      {
        return;
      }

      if (coordinates.Count < 2)
      {
        throw new ArgumentOutOfRangeException(nameof(coordinates), Resources.CurveInvalidNumberOfCoordinatesError);
      }

      Coordinates = new List<Coordinate>(coordinates);
    }

    #region Properties

    /// <summary>
    /// Gets an array with the vertices of this <see cref="Geometry"/> in the order they occur.
    /// </summary>
    public List<Coordinate> Coordinates { get; protected set; }

    /// <summary>
    /// Gets the number of <see cref="Coordinate"/>s the geometry is made of.
    /// </summary>
    public override int CoordinatesCount
    {
      get
      {
        return IsEmpty ? 0 : Coordinates.Count;
      }
    }

    /// <summary>
    /// Gets the number of dimensions used by this <see cref="Geometry"/>.
    /// </summary>
    public override int Dimensions
    {
      get
      {
        return 1;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="LineString"/> is a closed one.
    /// </summary>
    public bool IsClosed
    {
      get
      {
        return !IsEmpty && Coordinates.First() == Coordinates.Last();
      }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="Geometry"/> is an empty one.
    /// </summary>
    public override bool IsEmpty
    {
      get
      {
        return Coordinates == null || Coordinates.Count == 0;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="Curve"/> is a rectangle.
    /// </summary>
    public bool IsRectangle
    {
      get
      {
        if (CoordinatesCount != 5 || !IsClosed)
        {
          return false;
        }

        // Check vertices have correct values
        var boundingBox = GetBoundingBox();
        for (int i = 0; i < 5; i++)
        {
          var x = Coordinates[i].X;
          if (!(x.IsPracticallyEqual(boundingBox.MinX) || x.IsPracticallyEqual(boundingBox.MaxX)))
          {
            return false;
          }

          var y = Coordinates[i].Y;
          if (!(y.IsPracticallyEqual(boundingBox.MinY) || y.IsPracticallyEqual(boundingBox.MaxY)))
          {
            return false;
          }
        }

        // Check vertices are in right order
        var prevX = Coordinates[0].X;
        var prevY = Coordinates[0].Y;
        for (int i = 1; i <= 4; i++)
        {
          var x = Coordinates[i].X;
          var y = Coordinates[i].Y;
          var xChanged = !x.IsPracticallyEqual(prevX);
          var yChanged = !y.IsPracticallyEqual(prevY);
          if (xChanged == yChanged)
          {
            return false;
          }

          prevX = x;
          prevY = y;
        }

        return true;
      }
    }

    /// <summary>
    /// Gets the linear length of this <see cref="Curve"/>.
    /// </summary>
    public virtual double LinearLength
    {
      get
      {
        if (Coordinates == null || Coordinates.Count <= 1)
        {
          return 0.0;
        }

        double len = 0.0;
        var coord = Coordinates.First();
        var x0 = coord.X;
        var y0 = coord.Y;
        for (int i = 1; i < Coordinates.Count; i++)
        {
          coord = Coordinates[i];
          double x1 = coord.X;
          double y1 = coord.Y;
          double dx = x1 - x0;
          double dy = y1 - y0;

          len += Math.Sqrt(dx * dx + dy * dy);

          x0 = x1;
          y0 = y1;
        }

        return len;
      }
    }

    #endregion Properties

    /// <summary>
    /// Returns a list of <see cref="LineSegment"/>s that compose this <see cref="Curve"/>.
    /// </summary>
    public List<LineSegment> GetSegments()
    {
      if (Coordinates == null)
      {
        return null;
      }

      var segments = new List<LineSegment>(Coordinates.Count - 1);
      for (int index = 1; index < Coordinates.Count - 1; index++)
      {
        segments.Add(new LineSegment(Coordinates[index - 1], Coordinates[index]));
      }

      return segments;
    }

    /// <summary>
    /// Returns a GeoJSON text representation of just the coordinates of this <see cref="Curve"/>.
    /// </summary>
    /// <returns>A GeoJSON text representation of just the coordinates of this <see cref="Curve"/>.</returns>
    internal string GetCoordinatesAsGeoJsonString()
    {
      if (IsEmpty)
      {
        return string.Empty;
      }

      var builder = new StringBuilder("[ ");
      builder.Append(Coordinates[0].ToGeoJsonString());
      for (int i = 1; i < Coordinates.Count; i++)
      {
        builder.AppendFormat(", {0}", Coordinates[i].ToGeoJsonString());
      }

      builder.Append(" ]");
      return builder.ToString();
    }

    /// <summary>
    /// Returns a GML text representation of just the coordinates of this <see cref="Curve"/>.
    /// </summary>
    /// <returns>A GML text representation of just the coordinates of this <see cref="Curve"/>.</returns>
    internal string GetCoordinatesAsGmlString()
    {
      if (IsEmpty)
      {
        return string.Empty;
      }

      var builder = new StringBuilder("<gml:coordinates>");
      builder.Append(Coordinates[0].ToGmlString());
      for (int i = 1; i < Coordinates.Count; i++)
      {
        builder.AppendFormat(" {0}", Coordinates[i].ToGmlString());
      }

      builder.Append("</gml:coordinates>");
      return builder.ToString();
    }

    /// <summary>
    /// Returns a KML text representation of just the coordinates of this <see cref="Curve"/>.
    /// </summary>
    /// <returns>A KML text representation of just the coordinates of this <see cref="Curve"/>.</returns>
    internal string GetCoordinatesAsKmlString()
    {
      if (IsEmpty)
      {
        return string.Empty;
      }

      var builder = new StringBuilder("<coordinates>");
      builder.Append(Coordinates[0].ToKmlString());
      for (int i = 1; i < Coordinates.Count; i++)
      {
        builder.AppendFormat(" {0}", Coordinates[i].ToKmlString());
      }

      builder.Append("</coordinates>");
      return builder.ToString();
    }

    /// <summary>
    /// Returns a WKT text representation of just the coordinates of this <see cref="Curve"/>.
    /// </summary>
    /// <returns>A WKT text representation of just the coordinates of this <see cref="Curve"/>.</returns>
    internal string GetCoordinatesAsWktString()
    {
      if (IsEmpty)
      {
        return string.Empty;
      }

      var builder = new StringBuilder("(");
      builder.Append(Coordinates[0].ToWktString());
      for (int i = 1; i < Coordinates.Count; i++)
      {
        builder.AppendFormat(",{0}", Coordinates[i].ToWktString());
      }

      builder.Append(")");
      return builder.ToString();
    }
  }
}
