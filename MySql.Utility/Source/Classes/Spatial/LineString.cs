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
  /// Represents a <see cref="Curve"/> with linear interpolation between points.
  /// </summary>
  public class LineString : Curve, IEquatable<LineString>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="LineString"/> class.
    /// </summary>
    /// <param name="srid"></param>
    public LineString(int srid)
      : base(srid)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LineString"/> class.
    /// </summary>
    /// <param name="coordinates">The <see cref="Coordinate"/>s that define the <see cref="LineString"/>.</param>
    /// <param name="srid">The ID of the Spatial Reference System used by this <see cref="Geometry"/>.</param>
    public LineString(Coordinate[] coordinates, int srid)
      : base(coordinates, srid)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LineString"/> class.
    /// </summary>
    /// <param name="coordinates">The <see cref="Coordinate"/>s that define the <see cref="LineString"/>.</param>
    /// <param name="srid">The ID of the Spatial Reference System used by this <see cref="Geometry"/>.</param>
    public LineString(List<Coordinate> coordinates, int srid)
      : base(coordinates, srid)
    {
    }

    #region Properties

    /// <summary>
    /// Gets a value indicating whether this <see cref="LineString"/> can be considered a ring.
    /// </summary>
    public bool IsRing
    {
      get
      {
        return IsClosed && IsSimple;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="Geometry"/> is a simple one, meaning its segments do not intersect with each other.
    /// </summary>
    public override bool IsSimple
    {
      get
      {
        var intersectingSegments = GetIntersectingSegments(true, true);
        return intersectingSegments == null || intersectingSegments.Count == 0;
      }
    }

    /// <summary>
    /// Gets the <see cref="GeometryType"/> of this <see cref="Geometry"/>.
    /// </summary>
    public override GeometryType Type
    {
      get
      {
        return GeometryType.LineString;
      }
    }

    #endregion Properties

    /// <summary>
    /// Checks whether the given text is a valid representation of a <see cref="LineString"/> in the given <see cref="GeometryAsTextFormatType"/>.
    /// </summary>
    /// <param name="geometryAsText">The string representation of a <see cref="LineString"/> in the given format.</param>
    /// <param name="format">A <see cref="GeometryAsTextFormatType"/> value.</param>
    /// <returns><c>true</c> if the given text is a valid representation of a <see cref="LineString"/> in the given <see cref="GeometryAsTextFormatType"/>, <c>false</c> otherwise.</returns>
    public new static bool IsValid(string geometryAsText, GeometryAsTextFormatType format)
    {
      if (string.IsNullOrEmpty(geometryAsText))
      {
        return false;
      }

      var regex = GetRegex(format);
      return regex == null ? false : regex.IsMatch(geometryAsText);
    }

    public static bool operator !=(LineString lhs, LineString rhs)
    {
      return !(lhs == rhs);
    }

    public static bool operator ==(LineString lhs, LineString rhs)
    {
      return ReferenceEquals(lhs, null)
        ? ReferenceEquals(rhs, null)
        : lhs.Equals(rhs);
    }

    /// <summary>
    /// Converts the string representation of a <see cref="LineString"/> in the given <see cref="GeometryAsTextFormatType"/> to a <see cref="LineString"/> equivalent.
    /// </summary>
    /// <param name="geometryAsText">The string representation of a <see cref="LineString"/> in the given format.</param>
    /// <param name="format">A <see cref="GeometryAsTextFormatType"/> value.</param>
    /// <returns>A <see cref="LineString"/> equivalent to the string representation of a <see cref="LineString"/> in the given <see cref="GeometryAsTextFormatType"/>.</returns>
    public new static LineString Parse(string geometryAsText, GeometryAsTextFormatType format)
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

      var coordsGroup = match.Groups["LineStringCoords"];
      return coordsGroup.Success
        ? ParseFromCoordsSet(coordsGroup.Value, format, srid)
        : new LineString(srid);
    }

    /// <summary>
    /// Returns a <see cref="LineString"/> from a piece of text containing a coordinate set in the given <see cref="GeometryAsTextFormatType"/>.
    /// </summary>
    /// <param name="coordsSetText">The string representation of a coordinate set.</param>
    /// <param name="format">A <see cref="GeometryAsTextFormatType"/> value.</param>
    /// <param name="srid">The ID of the Spatial Reference System used by this <see cref="Geometry"/>.</param>
    /// <returns>A <see cref="LineString"/> from a piece of text containing a coordinate set in the given <see cref="GeometryAsTextFormatType"/>.</returns>
    internal static LineString ParseFromCoordsSet(string coordsSetText, GeometryAsTextFormatType format, int srid = 0)
    {
      if (string.IsNullOrEmpty(coordsSetText))
      {
        return null;
      }

      var coordsRegex = GetCoordsRegex(format);
      if (coordsRegex == null)
      {
        return null;
      }

      var match = coordsRegex.Match(coordsSetText);
      if (!match.Success)
      {
        return null;
      }

      var decimalsGroup = match.Groups["DecimalNumber"];
      if (!decimalsGroup.Success)
      {
        return new LineString(srid);
      }

      var coordinates = new List<Coordinate>(decimalsGroup.Captures.Count);
      for (int i = 0; i < decimalsGroup.Captures.Count; i++)
      {
        var coordinate = new Coordinate(double.Parse(decimalsGroup.Captures[i].ToString()), double.Parse(decimalsGroup.Captures[++i].ToString()));
        coordinates.Add(coordinate);
      }

      return coordinates.Count == 0
        ? new LineString(srid)
        : new LineString(coordinates, srid);
    }

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public override object Clone()
    {
      return IsEmpty
        ? new LineString(SRID)
        : new LineString(Coordinates.Select(coord => coord.Clone() as Coordinate).ToList(), SRID);
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
    {
      return Equals(obj as LineString);
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns><c>true</c> if the current object is equal to the other parameter; otherwise, <c>false</c>.</returns>
    public bool Equals(LineString other)
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
      if (Coordinates == null && other.Coordinates == null)
      {
        return true;
      }

      if (Coordinates == null || other.Coordinates == null)
      {
        return false;
      }

      var firstNotSecond = Coordinates.Except(other.Coordinates).ToList();
      var secondNotFirst = other.Coordinates.Except(Coordinates).ToList();
      return !firstNotSecond.Any() && !secondNotFirst.Any();
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
        int hashCode = Coordinates[0].GetHashCode();
        for (int i = 1; i < Coordinates.Count; i++)
        {
          hashCode = (hashCode * hashCodeMultiplier) ^ Coordinates[i].GetHashCode();
        }

        return hashCode;
      }
    }

    /// <summary>
    /// Calculates intersections among the <see cref="LineSegment"/>s of this <see cref="LineString"/>.
    /// </summary>
    /// <param name="stopAtFirstOccurrence">Flag indicating whether only the first intersection is returned or all intersections.</param>
    /// <param name="ignoreEndPointsIntersection">Ignore any intersection of end points between segments.</param>
    /// <returns>A list of intersections if any occurs.</returns>
    public List<Tuple<LineSegment, LineSegment>> GetIntersectingSegments(bool stopAtFirstOccurrence, bool ignoreEndPointsIntersection = false)
    {
      if (IsEmpty)
      {
        return null;
      }

      var segments = GetSegments();

      // If we have less than 3 segments, there are no intersections.
      if (segments.Count < 3)
      {
        return null;
      }

      var intersectingSegments = new List<Tuple<LineSegment, LineSegment>>();
      for (int i = 0; i < segments.Count - 2; i++)
      {
        for (int j = i + 2; i < segments.Count; i++)
        {
          var foundIntersection = segments[i].IntersectsWith(segments[j], ignoreEndPointsIntersection);
          if (!foundIntersection)
          {
            continue;
          }

          intersectingSegments.Add(new Tuple<LineSegment, LineSegment>(segments[i], segments[j]));
          if (stopAtFirstOccurrence)
          {
            return intersectingSegments;
          }
        }
      }

      return intersectingSegments;
    }

    /// <summary>
    /// Creates a <see cref="LineString"/> whose coordinates are in the reverse order of this instance.
    /// </summary>
    /// <returns>A <see cref="LineString"/> whose coordinates are in the reverse order of this instance.</returns>
    public LineString Reverse()
    {
      if (IsEmpty)
      {
        return this;
      }

      var reversedCoordinates = new List<Coordinate>(Coordinates);
      reversedCoordinates.Reverse();
      return new LineString(reversedCoordinates, SRID);
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
    private static Regex GetCoordsRegex(GeometryAsTextFormatType format)
    {
      Regex regex = null;
      switch (format)
      {
        case GeometryAsTextFormatType.WKT:
          regex = new WktLineStringCoordsRegex();
          break;

        case GeometryAsTextFormatType.KML:
          regex = new KmlLineStringCoordsRegex();
          break;

        case GeometryAsTextFormatType.GML:
          regex = new GmlLineStringCoordsRegex();
          break;

        case GeometryAsTextFormatType.GeoJSON:
          regex = new GeoJsonLineStringCoordsRegex();
          break;
      }

      return regex;
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
          regex = new WktLineStringRegex();
          break;

        case GeometryAsTextFormatType.KML:
          regex = new KmlLineStringRegex();
          break;

        case GeometryAsTextFormatType.GML:
          regex = new GmlLineStringRegex();
          break;

        case GeometryAsTextFormatType.GeoJSON:
          regex = new GeoJsonLineStringRegex();
          break;
      }

      return regex;
    }
  }
}
