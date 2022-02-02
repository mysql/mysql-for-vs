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
  /// Represents two-dimensional objects.
  /// </summary>
  public abstract class Surface : Geometry
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="Surface"/> class.
    /// </summary>
    /// <param name="srid">The ID of the Spatial Reference System used by the new <see cref="Surface"/>.</param>
    protected Surface(int srid)
      : base(srid)
    {
      Shell = null;
      Holes = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Surface"/> class.
    /// </summary>
    /// <param name="shell">A <see cref="LinearRing"/> representing the outer boundary of the new <see cref="Surface"/>.</param>
    /// <param name="srid">The ID of the Spatial Reference System used by the new <see cref="Surface"/>.</param>
    protected Surface(LinearRing shell, int srid)
      : this(srid)
    {
      Shell = shell;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Surface"/> class.
    /// </summary>
    /// <param name="shell">The outer <see cref="LinearRing"/> defining the closed shell of the <see cref="Surface"/>.</param>
    /// <param name="holes">A list of <see cref="LinearRing"/>s defining holes within the <see cref="Surface"/>..</param>
    /// <param name="srid">The ID of the Spatial Reference System used by the new <see cref="Surface"/>.</param>
    protected Surface(LinearRing shell, List<LinearRing> holes, int srid)
      : this(shell, srid)
    {
      if (holes != null && holes.Any(h => h == null))
      {
        holes.RemoveAll(h => h == null);
      }

      if ((shell == null || shell.IsEmpty) && holes != null && holes.Any(h => !h.IsEmpty))
      {
        throw new Exception("Shell is empty but holes are not.");
      }

      Holes = holes;
    }

    #region Properties

    /// <summary>
    /// Gets the area of this <see cref="Surface"/>.
    /// </summary>
    public virtual double Area
    {
      get
      {
        double area = 0.0;
        if (IsEmpty)
        {
          return area;
        }

        area += Shell.Area;
        if (Holes != null)
        {
          area = Holes.Aggregate(area, (current, hole) => current - hole.Area);
        }

        return area;
      }
    }

    /// <summary>
    /// Gets the number of <see cref="Coordinate"/>s the geometry is made of.
    /// </summary>
    public override int CoordinatesCount
    {
      get
      {
        if (IsEmpty)
        {
          return 0;
        }

        int numCoordinates = Shell.CoordinatesCount;
        if (Holes != null)
        {
          numCoordinates += Holes.Sum(hole => hole.CoordinatesCount);
        }

        return numCoordinates;
      }
    }

    /// <summary>
    /// Gets the number of dimensions used by this <see cref="Geometry"/>.
    /// </summary>
    public override int Dimensions
    {
      get
      {
        return 2;
      }
    }

    /// <summary>
    /// Gets a list of <see cref="LinearRing"/>s defining holes within the <see cref="Surface"/>.
    /// </summary>
    public List<LinearRing> Holes { get; protected set; }

    /// <summary>
    /// Gets the number of holes in this <see cref="Surface"/>.
    /// </summary>
    public int HolesCount
    {
      get
      {
        return Holes == null ? 0 : Holes.Count;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="Geometry"/> is an empty one.
    /// </summary>
    public override bool IsEmpty
    {
      get
      {
        return Shell == null || Shell.IsEmpty;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="Surface"/> is a rectangle.
    /// </summary>
    public bool IsRectangle
    {
      get
      {
        return !IsEmpty
               && Shell.IsRectangle
               && (Holes == null || Holes.Count == 0);
      }
    }

    /// <summary>
    /// Gets the linear length of this <see cref="Surface"/>, which is the <see cref="Perimeter"/> plus the lenghts of its <see cref="Holes"/>.
    /// </summary>
    public virtual double LinearLength
    {
      get
      {
        var length = Perimeter;
        if (Holes != null)
        {
          length += Holes.Sum(hole => hole.LinearLength);
        }

        return length;
      }
    }

    /// <summary>
    /// Gets the perimeter of the <see cref="Shell"/> of this <see cref="Surface"/>.
    /// </summary>
    public virtual double Perimeter
    {
      get
      {
        return IsEmpty ? 0.0 : Shell.LinearLength;
      }
    }

    /// <summary>
    /// Gets the outer <see cref="LinearRing"/> defining the closed shell of the <see cref="Surface"/>.
    /// </summary>
    public LinearRing Shell { get; protected set; }

    #endregion Properties

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
      builder.Append(Shell.GetCoordinatesAsGeoJsonString());
      if (Holes != null)
      {
        foreach (var hole in Holes)
        {
          builder.Append(", ");
          builder.Append(hole.GetCoordinatesAsGeoJsonString());
        }
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

      var builder = new StringBuilder("<gml:outerBoundaryIs><gml:LinearRing>");
      builder.Append(Shell.GetCoordinatesAsGmlString());
      builder.Append("</gml:LinearRing></gml:outerBoundaryIs>");
      if (Holes != null)
      {
        foreach (var hole in Holes)
        {
          builder.Append("<gml:innerBoundaryIs><gml:LinearRing>");
          builder.Append(hole.GetCoordinatesAsGmlString());
          builder.Append("</gml:LinearRing></gml:innerBoundaryIs>");
        }
      }

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

      var builder = new StringBuilder("<outerBoundaryIs><LinearRing>");
      builder.Append(Shell.GetCoordinatesAsKmlString());
      builder.Append("</LinearRing></outerBoundaryIs>");
      if (Holes != null)
      {
        foreach (var hole in Holes)
        {
          builder.Append("<innerBoundaryIs><LinearRing>");
          builder.Append(hole.GetCoordinatesAsKmlString());
          builder.Append("</LinearRing></innerBoundaryIs>");
        }
      }

      return builder.ToString();
    }

    /// <summary>
    /// Returns a WKT text representation of just the coordinates of this <see cref="Surface"/>.
    /// </summary>
    /// <returns>A WKT text representation of just the coordinates of this <see cref="Surface"/>.</returns>
    internal string GetCoordinatesAsWktString()
    {
      if (IsEmpty)
      {
        return string.Empty;
      }

      var builder = new StringBuilder("(");
      builder.Append(Shell.GetCoordinatesAsWktString());
      if (Holes != null)
      {
        foreach (var hole in Holes)
        {
          builder.Append(",");
          builder.Append(hole.GetCoordinatesAsWktString());
        }
      }

      builder.Append(")");
      return builder.ToString();
    }
  }
}
