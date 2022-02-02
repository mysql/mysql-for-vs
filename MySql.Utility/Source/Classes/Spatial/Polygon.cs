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
using System.Text.RegularExpressions;
using MySql.Utility.Enums;

namespace MySql.Utility.Classes.Spatial
{
  /// <summary>
  /// A planar <see cref="Surface"/> representing a multisided geometry.
  /// It is defined by a single exterior boundary and zero or more interior boundaries, where each interior boundary defines a hole in the <see cref="Polygon"/>.
  /// </summary>
  public class Polygon : Surface, IEquatable<Polygon>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="Polygon"/> class.
    /// </summary>
    /// <param name="srid">The ID of the Spatial Reference System used by the new <see cref="Polygon"/>.</param>
    public Polygon(int srid)
      : base(srid)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Polygon"/> class.
    /// </summary>
    /// <param name="shell">A <see cref="LinearRing"/> representing the outer boundary of the new <see cref="Polygon"/>.</param>
    /// <param name="srid">The ID of the Spatial Reference System used by the new <see cref="Polygon"/>.</param>
    public Polygon(LinearRing shell, int srid)
      : base(shell, srid)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Polygon"/> class.
    /// </summary>
    /// <param name="shell">The outer <see cref="LinearRing"/> defining the closed shell of the <see cref="Polygon"/>.</param>
    /// <param name="holes">A list of <see cref="LinearRing"/>s defining holes within the <see cref="Polygon"/>..</param>
    /// <param name="srid">The ID of the Spatial Reference System used by the new <see cref="Polygon"/>.</param>
    public Polygon(LinearRing shell, List<LinearRing> holes, int srid)
      : base (shell, holes, srid)
    {
    }

    #region Properties

    /// <summary>
    /// Gets a value indicating whether this <see cref="Geometry"/> is a simple one.
    /// </summary>
    public override bool IsSimple
    {
      get
      {
        // Always true for polygons.
        return true;
      }
    }

    /// <summary>
    /// Gets the <see cref="GeometryType"/> of this <see cref="Geometry"/>.
    /// </summary>
    public override GeometryType Type
    {
      get
      {
        return GeometryType.Polygon;
      }
    }

    #endregion Properties

    /// <summary>
    /// Checks whether the given text is a valid representation of a <see cref="Polygon"/> in the given <see cref="GeometryAsTextFormatType"/>.
    /// </summary>
    /// <param name="geometryAsText">The string representation of a <see cref="Polygon"/> in the given format.</param>
    /// <param name="format">A <see cref="GeometryAsTextFormatType"/> value.</param>
    /// <returns><c>true</c> if the given text is a valid representation of a <see cref="Polygon"/> in the given <see cref="GeometryAsTextFormatType"/>, <c>false</c> otherwise.</returns>
    public new static bool IsValid(string geometryAsText, GeometryAsTextFormatType format)
    {
      if (string.IsNullOrEmpty(geometryAsText))
      {
        return false;
      }

      var regex = GetRegex(format);
      return regex == null ? false : regex.IsMatch(geometryAsText);
    }

    public static bool operator !=(Polygon lhs, Polygon rhs)
    {
      return !(lhs == rhs);
    }

    public static bool operator ==(Polygon lhs, Polygon rhs)
    {
      return ReferenceEquals(lhs, null)
        ? ReferenceEquals(rhs, null)
        : lhs.Equals(rhs);
    }

    /// <summary>
    /// Converts the string representation of a <see cref="Polygon"/> in the given <see cref="GeometryAsTextFormatType"/> to a <see cref="Polygon"/> equivalent.
    /// </summary>
    /// <param name="geometryAsText">The string representation of a <see cref="Polygon"/> in the given format.</param>
    /// <param name="format">A <see cref="GeometryAsTextFormatType"/> value.</param>
    /// <returns>A <see cref="Polygon"/> equivalent to the string representation of a <see cref="Polygon"/> in the given <see cref="GeometryAsTextFormatType"/>.</returns>
    public new static Polygon Parse(string geometryAsText, GeometryAsTextFormatType format)
    {
      if (string.IsNullOrEmpty(geometryAsText))
      {
        return null;
      }

      var regex = GetRegex(format);
      if (regex == null)
      {
        return null;
      }

      var match = regex.Match(geometryAsText);
      if (!match.Success)
      {
        return null;
      }

      int srid = 0;
      if (format == GeometryAsTextFormatType.WKT)
      {
        var sridGroup = match.Groups["SRID"];
        if (sridGroup.Success)
        {
          srid = int.Parse(sridGroup.Value);
        }
      }

      var ringsGroup = match.Groups["PolygonRings"];
      return ringsGroup.Success
        ? ParseFromRings(ringsGroup.Value, format, srid)
        : new Polygon(srid);
    }

    /// <summary>
    /// Returns a <see cref="Polygon"/> from a piece of text containing a coordinate set in the given <see cref="GeometryAsTextFormatType"/>.
    /// </summary>
    /// <param name="ringsText">The string representation of a <see cref="Polygon"/>'s rings.</param>
    /// <param name="format">A <see cref="GeometryAsTextFormatType"/> value.</param>
    /// <param name="srid">The ID of the Spatial Reference System used by this <see cref="Geometry"/>.</param>
    /// <returns>A <see cref="Polygon"/> from a piece of text containing a coordinate set in the given <see cref="GeometryAsTextFormatType"/>.</returns>
    internal static Polygon ParseFromRings(string ringsText, GeometryAsTextFormatType format, int srid = 0)
    {
      if (string.IsNullOrEmpty(ringsText))
      {
        return null;
      }

      var ringsRegex = GetRingsRegex(format);
      if (ringsRegex == null)
      {
        return null;
      }

      var match = ringsRegex.Match(ringsText);
      if (!match.Success)
      {
        return null;
      }

      var ringCoordsGroup = match.Groups["RingCoords"];
      if (!ringCoordsGroup.Success)
      {
        return new Polygon(srid);
      }

      var ringCoordsRegex = GetRingCoordsRegex(format);
      if (ringCoordsRegex == null)
      {
        return null;
      }

      var ringsCount = ringCoordsGroup.Captures.Count;
      LinearRing shell = null;
      var holes = ringsCount > 1 ? new List<LinearRing>(ringsCount - 1) : null;
      for (int ringIdx = 0; ringIdx < ringsCount; ringIdx++)
      {
        var ringCoords = ringCoordsGroup.Captures[ringIdx];
        var ringCoordsText = ringCoords.ToString();
        var ringMatch = ringCoordsRegex.Match(ringCoordsText);
        var decimalsGroup = ringMatch.Groups["DecimalNumber"];
        if (!ringMatch.Success || !decimalsGroup.Success)
        {
          throw new FormatException(string.Format("{0} is not a valid representation of a Polygon's ring.", ringCoordsText));
        }

        var coordinates = new List<Coordinate>(decimalsGroup.Captures.Count);
        for (int i = 0; i < decimalsGroup.Captures.Count; i++)
        {
          var coordinate = new Coordinate(double.Parse(decimalsGroup.Captures[i].ToString()), double.Parse(decimalsGroup.Captures[++i].ToString()));
          coordinates.Add(coordinate);
        }

        if (ringIdx == 0)
        {
          shell = new LinearRing(coordinates, srid);
        }
        else if (holes != null)
        {
          holes.Add(new LinearRing(coordinates, srid));
        }
      }

      return new Polygon(shell, holes, srid);
    }

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public override object Clone()
    {
      return IsEmpty
        ? new Polygon(SRID)
        : new Polygon(new LinearRing(Shell.Coordinates.Select(coord => coord.Clone() as Coordinate).ToList(), Shell.SRID),
                      Holes != null
                        ? Holes.Select(hole => hole.IsEmpty ? new LinearRing(hole.SRID) : new LinearRing(hole.Coordinates.Select(coord => coord.Clone() as Coordinate).ToList(), hole.SRID)).ToList()
                        : null,
                      SRID);
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
    {
      return Equals(obj as Polygon);
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns><c>true</c> if the current object is equal to the other parameter; otherwise, <c>false</c>.</returns>
    public bool Equals(Polygon other)
    {
      // If parameter is null, return false.
      if (ReferenceEquals(other, null))
      {
        return false;
      }

      // Optimization for a common success case.
      if (ReferenceEquals(this, other))
      {
        return true;
      }

      // If run-time types are not exactly the same, return false.
      if (GetType() != other.GetType())
      {
        return false;
      }

      // Return true if the fields match.
      // Note that the base class is not invoked because it is
      // System.Object, which defines Equals as reference equality.
      if (Shell != other.Shell
          || HolesCount != other.HolesCount)
      {
        return false;
      }

      for (int i = 0; i < HolesCount; i++)
      {
        if (Holes[i] != other.Holes[i])
        {
          return false;
        }
      }

      return true;
    }

    /// <summary>
    /// Calculates the <see cref="BoundingBox"/> for this <see cref="Geometry"/>.
    /// </summary>
    /// <returns>The <see cref="BoundingBox"/> for this <see cref="Geometry"/>.</returns>
    public override BoundingBox GetBoundingBox()
    {
      return BoundingBox.FromGeometry(this);
    }

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
      // Arbitrary number to generate the hash code.
      const int hashCodeMultiplier = 397;
      if (IsEmpty)
      {
        return hashCodeMultiplier;
      }

      unchecked
      {
        int hashCode = Shell.GetHashCode();
        if (Holes != null)
        {
          hashCode = Holes.Aggregate(hashCode, (current, hole) => (current*hashCodeMultiplier) ^ hole.GetHashCode());
        }

        return hashCode;
      }
    }

    /// <summary>
    /// Creates a <see cref="Polygon"/> whose coordinates are in the reverse order of this instance.
    /// </summary>
    /// <returns>A <see cref="Polygon"/> whose coordinates are in the reverse order of this instance.</returns>
    public Polygon Reverse()
    {
      if (IsEmpty)
      {
        return new Polygon(SRID);
      }

      var reversedShell = Shell.Reverse();
      List<LinearRing> reversedHoles = null;
      if (Holes != null)
      {
        reversedHoles = new List<LinearRing>(Holes.Count);
        reversedHoles.AddRange(Holes.Select(hole => hole.Reverse()));
      }

      return new Polygon(reversedShell, reversedHoles, SRID);
    }

    /// <summary>
    /// Returns a GeoJSON text representation of this spatial element.
    /// </summary>
    /// <returns>A GeoJSON text representation of this spatial element.</returns>
    public override string ToGeoJsonString()
    {
      var className = GetGeometryType(GeometryAsTextFormatType.GeoJSON);
      return string.Format("{{ \"type\": \"{0}\", \"coordinates\": {1} }}",
        className,
        IsEmpty ? GEOJSON_EMPTY_GEOM : GetCoordinatesAsGeoJsonString());
    }

    /// <summary>
    /// Returns a GML text representation of this spatial element.
    /// </summary>
    /// <returns>A GML text representation of this spatial element.</returns>
    public override string ToGmlString()
    {
      return ToGmlString(true);
    }

    /// <summary>
    /// Returns a GML text representation of this spatial element.
    /// </summary>
    /// <param name="outputSrsName">Flag indicating whether the srsName attribute is included.</param>
    /// <returns>A GML text representation of this spatial element.</returns>
    public string ToGmlString(bool outputSrsName)
    {
      var className = GetGeometryType(GeometryAsTextFormatType.GML);
      return string.Format("<gml:{0}{1}>{2}</gml:{0}>",
        className,
        outputSrsName ? " srsName=\"EPSG:4326\"" : string.Empty,
        GetCoordinatesAsGmlString());
    }

    /// <summary>
    /// Returns a KML text representation of this spatial element.
    /// </summary>
    /// <returns>A KML text representation of this spatial element.</returns>
    public override string ToKmlString()
    {
      var className = GetGeometryType(GeometryAsTextFormatType.KML);
      return string.Format("<{0}>{1}</{0}>", className, GetCoordinatesAsKmlString());
    }

    /// <summary>
    /// Returns a WKT text representation of this spatial element.
    /// </summary>
    /// <returns>A WKT text representation of this spatial element.</returns>
    public override string ToWktString()
    {
      var className = GetGeometryType(GeometryAsTextFormatType.WKT);
      return IsEmpty
        ? className + " " + WKT_EMPTY_GEOM
        : string.Format("{0} {1}", className, GetCoordinatesAsWktString());
    }

    /// <summary>
    /// Returns a compiled regex corresponding to the given <see cref="GeometryAsTextFormatType"/> specific to this geometry class.
    /// </summary>
    /// <param name="format">A <see cref="GeometryAsTextFormatType"/> value.</param>
    /// <returns>A compiled regex corresponding to the given <see cref="GeometryAsTextFormatType"/> specific to this geometry class.</returns>
    private static Regex GetRegex(GeometryAsTextFormatType format)
    {
      Regex regex = null;
      switch (format)
      {
        case GeometryAsTextFormatType.WKT:
          regex = new WktPolygonRegex();
          break;

        case GeometryAsTextFormatType.KML:
          regex = new KmlPolygonRegex();
          break;

        case GeometryAsTextFormatType.GML:
          regex = new GmlPolygonRegex();
          break;

        case GeometryAsTextFormatType.GeoJSON:
          regex = new GeoJsonPolygonRegex();
          break;
      }

      return regex;
    }

    /// <summary>
    /// Returns a compiled regex corresponding to the given <see cref="GeometryAsTextFormatType"/> specific to this geometry class.
    /// </summary>
    /// <param name="format">A <see cref="GeometryAsTextFormatType"/> value.</param>
    /// <returns>A compiled regex corresponding to the given <see cref="GeometryAsTextFormatType"/> specific to this geometry class.</returns>
    private static Regex GetRingCoordsRegex(GeometryAsTextFormatType format)
    {
      Regex regex = null;
      switch (format)
      {
        case GeometryAsTextFormatType.WKT:
          regex = new WktLinearRingCoordsRegex();
          break;

        case GeometryAsTextFormatType.KML:
          regex = new KmlLinearRingCoordsRegex();
          break;

        case GeometryAsTextFormatType.GML:
          regex = new GmlLinearRingCoordsRegex();
          break;

        case GeometryAsTextFormatType.GeoJSON:
          regex = new GeoJsonLinearRingCoordsRegex();
          break;
      }

      return regex;
    }

    /// <summary>
    /// Returns a compiled regex corresponding to the given <see cref="GeometryAsTextFormatType"/> specific to this geometry class.
    /// </summary>
    /// <param name="format">A <see cref="GeometryAsTextFormatType"/> value.</param>
    /// <returns>A compiled regex corresponding to the given <see cref="GeometryAsTextFormatType"/> specific to this geometry class.</returns>
    private static Regex GetRingsRegex(GeometryAsTextFormatType format)
    {
      Regex regex = null;
      switch (format)
      {
        case GeometryAsTextFormatType.WKT:
          regex = new WktPolygonRingsRegex();
          break;

        case GeometryAsTextFormatType.KML:
          regex = new KmlPolygonRingsRegex();
          break;

        case GeometryAsTextFormatType.GML:
          regex = new GmlPolygonRingsRegex();
          break;

        case GeometryAsTextFormatType.GeoJSON:
          regex = new GeoJsonPolygonRingsRegex();
          break;
      }

      return regex;
    }
  }
}
