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
using System.Text;
using System.Text.RegularExpressions;
using MySql.Utility.Enums;

namespace MySql.Utility.Classes.Spatial
{
  /// <summary>
  /// Represents zero-dimensional objects.
  /// </summary>
  public class Point : Geometry, IEquatable<Point>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="Point"/> class.
    /// </summary>
    /// <param name="srid">The ID of the Spatial Reference System used by this <see cref="Point"/>.</param>
    public Point(int srid)
      : this(null, srid)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Point"/> class.
    /// </summary>
    /// <param name="coordinate">A <see cref="Coordinate"/> on which to base this <see cref="Point"/> on, or <c>null</c> to create an empty one.</param>
    /// <param name="srid">The ID of the Spatial Reference System used by this <see cref="Point"/>.</param> 
    public Point(Coordinate coordinate, int srid)
      : base(srid)
    {
      Coordinate = coordinate;
    }

    #region Properties

    /// <summary>
    /// Gets the <see cref="Coordinate"/> that represents this <see cref="Point"/> in space.
    /// </summary>
    public Coordinate Coordinate { get; set; }

    /// <summary>
    /// Gets the number of <see cref="Coordinate"/>s the geometry is made of.
    /// </summary>
    public override int CoordinatesCount
    {
      get
      {
        return IsEmpty ? 0 : 1;
      }
    }

    /// <summary>
    /// Gets the number of dimensions used by this <see cref="Geometry"/>.
    /// </summary>
    public override int Dimensions
    {
      get
      {
        return 0;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="Geometry"/> is an empty one.
    /// </summary>
    public override bool IsEmpty
    {
      get
      {
        return Coordinate == null; 
      }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="Geometry"/> is a simple one.
    /// </summary>
    public override bool IsSimple
    {
      get
      {
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
        return GeometryType.Point;
      }
    }

    #endregion Properties

    /// <summary>
    /// Checks whether the given text is a valid representation of a <see cref="Point"/> in the given <see cref="GeometryAsTextFormatType"/>.
    /// </summary>
    /// <param name="geometryAsText">The string representation of a <see cref="Point"/> in the given format.</param>
    /// <param name="format">A <see cref="GeometryAsTextFormatType"/> value.</param>
    /// <returns><c>true</c> if the given text is a valid representation of a <see cref="Point"/> in the given <see cref="GeometryAsTextFormatType"/>, <c>false</c> otherwise.</returns>
    public new static bool IsValid(string geometryAsText, GeometryAsTextFormatType format)
    {
      if (string.IsNullOrEmpty(geometryAsText))
      {
        return false;
      }

      var regex = GetRegex(format);
      return regex == null ? false : regex.IsMatch(geometryAsText);
    }

    public static bool operator !=(Point lhs, Point rhs)
    {
      return !(lhs == rhs);
    }

    public static bool operator ==(Point lhs, Point rhs)
    {
      return ReferenceEquals(lhs, null)
        ? ReferenceEquals(rhs, null)
        : lhs.Equals(rhs);
    }

    /// <summary>
    /// Converts the string representation of a <see cref="Point"/> in the given <see cref="GeometryAsTextFormatType"/> to a <see cref="Point"/> equivalent.
    /// </summary>
    /// <param name="geometryAsText">The string representation of a <see cref="Point"/> in the given format.</param>
    /// <param name="format">A <see cref="GeometryAsTextFormatType"/> value.</param>
    /// <returns>A <see cref="Point"/> equivalent to the string representation of a <see cref="Point"/> in the given <see cref="GeometryAsTextFormatType"/>.</returns>
    public new static Point Parse(string geometryAsText, GeometryAsTextFormatType format)
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

      var coordGroup = match.Groups["PointCoord"];
      return coordGroup.Success
        ? ParseFromCoord(coordGroup.Value, format, srid)
        : new Point(srid);
    }

    /// <summary>
    /// Returns a <see cref="Point"/> from a piece of text containing a coordinate in the given <see cref="GeometryAsTextFormatType"/>.
    /// </summary>
    /// <param name="coordText">The string representation of a coordinate.</param>
    /// <param name="format">A <see cref="GeometryAsTextFormatType"/> value.</param>
    /// <param name="srid">The ID of the Spatial Reference System used by this <see cref="Geometry"/>.</param>
    /// <returns>A <see cref="Point"/> from a piece of text containing a coordinate in the given <see cref="GeometryAsTextFormatType"/>.</returns>
    internal static Point ParseFromCoord(string coordText, GeometryAsTextFormatType format, int srid = 0)
    {
      if (string.IsNullOrEmpty(coordText))
      {
        return null;
      }

      var coordRegex = GetCoordRegex(format);
      if (coordRegex == null)
      {
        return null;
      }

      var match = coordRegex.Match(coordText);
      if (!match.Success)
      {
        return null;
      }

      var decimalsGroup = match.Groups["DecimalNumber"];
      var decimalsCount = decimalsGroup.Success ? decimalsGroup.Captures.Count : 0;
      if (decimalsCount == 0)
      {
        return new Point(srid);
      }

      return decimalsCount != 2
        ? null
        : new Point(new Coordinate(double.Parse(decimalsGroup.Captures[0].ToString()), double.Parse(decimalsGroup.Captures[1].ToString())), srid);
    }

    /// <summary>
    /// Returns a <see cref="Point"/> from a piece of text containing a coordinate in the given <see cref="GeometryAsTextFormatType"/>.
    /// </summary>
    /// <param name="coordText">The string representation of a coordinate.</param>
    /// <param name="srid">The ID of the Spatial Reference System used by this <see cref="Geometry"/>.</param>
    /// <returns>A <see cref="Point"/> from a piece of text containing a coordinate in the given <see cref="GeometryAsTextFormatType"/>.</returns>
    internal static Point ParseFromWktMultiPointCoord(string coordText, int srid = 0)
    {
      if (string.IsNullOrEmpty(coordText))
      {
        return null;
      }

      var coordRegex = new WktMultiPointCoordRegex();
      var match = coordRegex.Match(coordText);
      if (!match.Success)
      {
        return null;
      }

      var decimalsGroup = match.Groups["DecimalNumber"];
      var decimalsCount = decimalsGroup.Success ? decimalsGroup.Captures.Count : 0;
      if (decimalsCount == 0)
      {
        return new Point(srid);
      }

      return decimalsCount != 2
        ? null
        : new Point(new Coordinate(double.Parse(decimalsGroup.Captures[0].ToString()), double.Parse(decimalsGroup.Captures[1].ToString())), srid);
    }

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public override object Clone()
    {
      return IsEmpty
        ? new Point(SRID)
        : new Point(Coordinate.Clone() as Coordinate, SRID);
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
    {
      return Equals(obj as Point);
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns><c>true</c> if the current object is equal to the other parameter; otherwise, <c>false</c>.</returns>
    public bool Equals(Point other)
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
      return Coordinate == other.Coordinate;
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
        int hashCode = Coordinate.X.GetHashCode();
        hashCode = (hashCode * hashCodeMultiplier) ^ Coordinate.Y.GetHashCode();
        return hashCode;
      }
    }

    /// <summary>
    /// Returns a GeoJSON text representation of this spatial element.
    /// </summary>
    /// <returns>A GeoJSON text representation of this spatial element.</returns>
    public override string ToGeoJsonString()
    {
      var className = GetGeometryType(GeometryAsTextFormatType.GeoJSON);
      var builder = new StringBuilder();
      builder.Append("{ \"type\": \"");
      builder.Append(className);
      builder.Append("\", \"coordinates\": ");
      builder.Append(IsEmpty ? GEOJSON_EMPTY_GEOM : Coordinate.ToGeoJsonString());
      builder.Append(" }");
      return builder.ToString();
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
      var builder = new StringBuilder();
      builder.Append("<gml:");
      builder.Append(className);
      if (outputSrsName)
      {
        builder.Append(" srsName=\"EPSG:4326\"");
      }

      builder.Append(">");
      if (!IsEmpty)
      {
        builder.Append("<gml:coordinates>");
        builder.Append(Coordinate.ToGmlString());
        builder.Append("</gml:coordinates>");
      }

      builder.AppendFormat("</gml:{0}>", className);
      return builder.ToString();
    }

    /// <summary>
    /// Returns a KML text representation of this spatial element.
    /// </summary>
    /// <returns>A KML text representation of this spatial element.</returns>
    public override string ToKmlString()
    {
      var className = GetGeometryType(GeometryAsTextFormatType.KML);
      var builder = new StringBuilder();
      builder.AppendFormat("<{0}>", className);
      if (!IsEmpty)
      {
        builder.Append("<coordinates>");
        builder.Append(Coordinate.ToKmlString());
        builder.Append("</coordinates>");
      }

      builder.AppendFormat("</{0}>", className);
      return builder.ToString();
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
        : string.Format("{0} ({1})", className, Coordinate.ToWktString());
    }

    /// <summary>
    /// Returns a compiled regex corresponding to the given <see cref="GeometryAsTextFormatType"/> specific to this geometry class.
    /// </summary>
    /// <param name="format">A <see cref="GeometryAsTextFormatType"/> value.</param>
    /// <returns>A compiled regex corresponding to the given <see cref="GeometryAsTextFormatType"/> specific to this geometry class.</returns>
    private static Regex GetCoordRegex(GeometryAsTextFormatType format)
    {
      Regex regex = null;
      switch (format)
      {
        case GeometryAsTextFormatType.WKT:
          regex = new WktPointCoordRegex();
          break;

        case GeometryAsTextFormatType.KML:
          regex = new KmlPointCoordRegex();
          break;

        case GeometryAsTextFormatType.GML:
          regex = new GmlPointCoordRegex();
          break;

        case GeometryAsTextFormatType.GeoJSON:
          regex = new GeoJsonPointCoordRegex();
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
          regex = new WktPointRegex();
          break;

        case GeometryAsTextFormatType.KML:
          regex = new KmlPointRegex();
          break;

        case GeometryAsTextFormatType.GML:
          regex = new GmlPointRegex();
          break;

        case GeometryAsTextFormatType.GeoJSON:
          regex = new GeoJsonPointRegex();
          break;
      }

      return regex;
    }
  }
}
