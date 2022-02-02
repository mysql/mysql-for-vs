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
using System.Text.RegularExpressions;
using MySql.Utility.Enums;

namespace MySql.Utility.Classes.Spatial
{
  /// <summary>
  /// Represents a <see cref="Geometry"/> that is a collection of one or more geometries of any class.
  /// </summary>
  public class GeometryCollection : Geometry, IEquatable<GeometryCollection>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="GeometryCollection"/> class.
    /// </summary>
    /// <param name="geometries">An array of <see cref="Geometry"/> instances that compose the collection.</param>
    /// <param name="srid">The ID of the Spatial Reference System used by this <see cref="Point"/>.</param>
    public GeometryCollection(Geometry[] geometries, int srid)
      : this(srid)
    {
      Geometries = geometries == null
        ? null
        : new List<Geometry>(geometries);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GeometryCollection"/> class.
    /// </summary>
    /// <param name="geometries">A list of <see cref="Geometry"/> instances that compose the collection.</param>
    /// <param name="srid">The ID of the Spatial Reference System used by this <see cref="Point"/>.</param>
    public GeometryCollection(List<Geometry> geometries, int srid)
      : this(srid)
    {
      Geometries = geometries == null
        ? null
        : new List<Geometry>(geometries);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GeometryCollection"/> class.
    /// </summary>
    /// <param name="srid">The ID of the Spatial Reference System used by the new <see cref="GeometryCollection"/>.</param>
    protected GeometryCollection(int srid)
      : base(srid)
    {
      Geometries = null;
    }

    #region Properties

    /// <summary>
    /// Gets the total area of the <see cref="Surface"/>s contained in the collection.
    /// </summary>
    public double Area
    {
      get
      {
        double area = 0.0;
        if (IsEmpty)
        {
          return area;
        }

        return Geometries.OfType<Surface>().Sum(surface => surface.Area);
      }
    }

    /// <summary>
    /// Gets the number of <see cref="Coordinate"/>s the geometry is made of.
    /// </summary>
    public override int CoordinatesCount
    {
      get
      {
        return IsEmpty ? 0 : Geometries.Sum(g => g.CoordinatesCount);
      }
    }

    /// <summary>
    /// Gets the number of dimensions used by this <see cref="Geometry"/>.
    /// </summary>
    public override int Dimensions
    {
      get
      {
        return IsEmpty ? -1 : Geometries.Max(g => g.Dimensions);
      }
    }

    /// <summary>
    /// Gets collection of <see cref="Geometry"/> objects contained in this <see cref="GeometryCollection"/>.
    /// </summary>
    public List<Geometry> Geometries { get; protected set; }

    /// <summary>
    /// Gets the number of <see cref="Geometry"/> objects contained in this <see cref="GeometryCollection"/>.
    /// </summary>
    public int GeometriesCount
    {
      get
      {
        return IsEmpty ? 0 : Geometries.Count;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="Geometry"/> is an empty one.
    /// </summary>
    public override bool IsEmpty
    {
      get
      {
        return Geometries == null || Geometries.Count == 0;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="Geometry"/> is a simple one.
    /// </summary>
    public override bool IsSimple
    {
      get
      {
        return IsEmpty || Geometries.TrueForAll(g => g.IsSimple);
      }
    }

    /// <summary>
    /// Gets the total linear length of the <see cref="Curve"/>s and <see cref="Surface"/>s contained in the collection.
    /// </summary>
    public double LinearLength
    {
      get
      {
        var length = 0.0;
        if (IsEmpty)
        {
          return length;
        }

        foreach (var geom in Geometries)
        {
          var curve = geom as Curve;
          if (curve != null)
          {
            length += curve.LinearLength;
            continue;
          }

          var surface = geom as Surface;
          if (surface != null)
          {
            length += surface.LinearLength;
          }
        }

        return length;
      }
    }

    /// <summary>
    /// Gets the <see cref="GeometryType"/> of this <see cref="Geometry"/>.
    /// </summary>
    public override GeometryType Type
    {
      get
      {
        return GeometryType.GeometryCollection;
      }
    }

    #endregion Properties

    /// <summary>
    /// Checks whether the given text is a valid representation of a <see cref="GeometryCollection"/> in the given <see cref="GeometryAsTextFormatType"/>.
    /// </summary>
    /// <param name="geometryAsText">The string representation of a <see cref="GeometryCollection"/> in the given format.</param>
    /// <param name="format">A <see cref="GeometryAsTextFormatType"/> value.</param>
    /// <returns><c>true</c> if the given text is a valid representation of a <see cref="GeometryCollection"/> in the given <see cref="GeometryAsTextFormatType"/>, <c>false</c> otherwise.</returns>
    public new static bool IsValid(string geometryAsText, GeometryAsTextFormatType format)
    {
      if (string.IsNullOrEmpty(geometryAsText))
      {
        return false;
      }

      var regex = GetRegex(format);
      return regex == null ? false : regex.IsMatch(geometryAsText);
    }

    public static bool operator !=(GeometryCollection lhs, GeometryCollection rhs)
    {
      return !(lhs == rhs);
    }

    public static bool operator ==(GeometryCollection lhs, GeometryCollection rhs)
    {
      return ReferenceEquals(lhs, null)
        ? ReferenceEquals(rhs, null)
        : lhs.Equals(rhs);
    }

    /// <summary>
    /// Converts the string representation of a <see cref="GeometryCollection"/> in the given <see cref="GeometryAsTextFormatType"/> to a <see cref="GeometryCollection"/> equivalent.
    /// </summary>
    /// <param name="geometryAsText">The string representation of a <see cref="GeometryCollection"/> in the given format.</param>
    /// <param name="format">A <see cref="GeometryAsTextFormatType"/> value.</param>
    /// <returns>A <see cref="GeometryCollection"/> equivalent to the string representation of a <see cref="GeometryCollection"/> in the given <see cref="GeometryAsTextFormatType"/>.</returns>
    public new static GeometryCollection Parse(string geometryAsText, GeometryAsTextFormatType format)
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

      var geomsGroup = match.Groups["BasicGeometry"];
      if (!geomsGroup.Success)
      {
        return new GeometryCollection(srid);
      }

      var allowedGeomsRegex = GetAllowedGeomsRegex(format);
      var geometries = new List<Geometry>(geomsGroup.Captures.Count);
      int pointsCount = 0;
      int lineStringsCount = 0;
      int polygonsCount = 0;
      foreach (var geomCapture in geomsGroup.Captures)
      {
        var geomText = geomCapture.ToString();
        var allowedGeomMatch = allowedGeomsRegex.Match(geomText);
        if (!allowedGeomMatch.Success)
        {
          throw new FormatException(string.Format("{0} is not a valid representation of a Geometry allowed in a collection.", geomText));
        }

        var classGroup = allowedGeomMatch.Groups["Class"];
        if (!classGroup.Success)
        {
          throw new FormatException("Did not find a recognized Geometry class.");
        }

        var geomClass = classGroup.Captures[0].ToString().ToLowerInvariant();
        Geometry geom = null;
        switch (geomClass)
        {
          case "point":
            pointsCount++;
            geom = Point.Parse(geomText, format);
            break;

          case "linestring":
            lineStringsCount++;
            geom = LineString.Parse(geomText, format);
            break;

          case "polygon":
            polygonsCount++;
            geom = Polygon.Parse(geomText, format);
            break;
        }

        if (geom == null)
        {
          continue;
        }

        geom.SRID = srid;
        geometries.Add(geom);
      }

      if (format == GeometryAsTextFormatType.KML)
      {
        var geomsCount = geometries.Count;
        if (geomsCount == pointsCount)
        {
          return new MultiPoint(geometries.Cast<Point>().ToArray(), srid);
        }

        if (geomsCount == lineStringsCount)
        {
          return new MultiLineString(geometries.Cast<LineString>().ToArray(), srid);
        }

        if (geomsCount == polygonsCount)
        {
          return new MultiPolygon(geometries.Cast<Polygon>().ToArray(), srid);
        }
      }

      return geometries.Count == 0
        ? new GeometryCollection(srid)
        : new GeometryCollection(geometries, srid);
    }

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public override object Clone()
    {
      return IsEmpty
        ? new GeometryCollection(SRID)
        : new GeometryCollection(Geometries.Select(geom => geom.Clone() as Geometry).ToList(), SRID);
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
    {
      return Equals(obj as GeometryCollection);
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns><c>true</c> if the current object is equal to the other parameter; otherwise, <c>false</c>.</returns>
    public bool Equals(GeometryCollection other)
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
      if (IsEmpty && other.IsEmpty)
      {
        return true;
      }

      if (GeometriesCount != other.GeometriesCount)
      {
        return false;
      }

      for (int i = 0; i < GeometriesCount; i++)
      {
        var geometry = Geometries[i];
        var otherGeometry = other.Geometries[i];
        var geometryType = geometry.GetType();
        var otherGeometryType = otherGeometry.GetType();

        if (geometryType != otherGeometryType)
        {
          return false;
        }

        switch (geometryType.Name)
        {
          case "Point":
            var point = geometry as Point;
            var otherPoint = otherGeometry as Point;
            if (point == otherPoint) continue;
            break;

          case "LineString":
            var lineString = geometry as LineString;
            var otherLineString = otherGeometry as LineString;
            if (lineString == otherLineString) continue;
            break;

          case "Polygon":
            var polygon = geometry as Polygon;
            var otherPolygon = otherGeometry as Polygon;
            if (polygon == otherPolygon) continue;
            break;

          default:
            if (geometry == otherGeometry) continue;
            break;
        }

        return false;
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
    /// Returns the type of the <see cref="Geometry"/> based on the given <see cref="GeometryAsTextFormatType"/>.
    /// </summary>
    /// <param name="format">A <see cref="GeometryAsTextFormatType"/> value.</param>
    /// <returns>The type of the <see cref="Geometry"/> based on the given <see cref="GeometryAsTextFormatType"/>.</returns>
    public override string GetGeometryType(GeometryAsTextFormatType format)
    {
      var className = GetType().Name;
      switch (format)
      {
        case GeometryAsTextFormatType.WKT:
          return className.ToUpper();

        case GeometryAsTextFormatType.KML:
          return "MultiGeometry";

        default:
          return className;
      }
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
        int hashCode = Geometries.GetHashCode();
        return Geometries.Aggregate(hashCode, (current, geom) => (current * hashCodeMultiplier) ^ geom.GetHashCode());
      }
    }

    /// <summary>
    /// Creates a <see cref="GeometryCollection"/> with every component reversed.
    /// The order of the components in the collection are not reversed.
    /// </summary>
    /// <returns>A <see cref="GeometryCollection"/> with every component reversed.</returns>
    public GeometryCollection Reverse()
    {
      if (IsEmpty)
      {
        return new GeometryCollection(SRID);
      }

      var reversedGeometries = new List<Geometry>(Geometries);
      reversedGeometries.Reverse();
      return new GeometryCollection(reversedGeometries, SRID);
    }

    /// <summary>
    /// Returns a GeoJSON text representation of this spatial element.
    /// </summary>
    /// <returns>A GeoJSON text representation of this spatial element.</returns>
    public override string ToGeoJsonString()
    {
      var builder = new StringBuilder("{ \"type\": \"GeometryCollection\"");
      builder.Append(", \"geometries\": [ ");
      if (!IsEmpty)
      {
        string separator = string.Empty;
        foreach (var geom in Geometries)
        {
          builder.Append(separator);
          builder.Append(geom.ToString(GeometryAsTextFormatType.GeoJSON));
          separator = ", ";
        }
      }

      builder.Append(" ]");
      builder.Append(" }");
      return builder.ToString();
    }

    /// <summary>
    /// Returns a GML text representation of this spatial element.
    /// </summary>
    /// <returns>A GML text representation of this spatial element.</returns>
    public override string ToGmlString()
    {
      var builder = new StringBuilder("<gml:MultiGeometry srsName=\"EPSG:4326\">");
      if (!IsEmpty)
      {
        foreach (var geom in Geometries)
        {
          builder.Append("<gml:geometryMember>");
          builder.Append(geom.ToString(GeometryAsTextFormatType.GML));
          builder.Append("</gml:geometryMember>");
        }
      }

      builder.Append("</gml:MultiGeometry>");
      return builder.ToString();
    }

    /// <summary>
    /// Returns a KML text representation of this spatial element.
    /// </summary>
    /// <returns>A KML text representation of this spatial element.</returns>
    public override string ToKmlString()
    {
      var builder = new StringBuilder("<MultiGeometry>");
      if (!IsEmpty)
      {
        foreach (var geom in Geometries)
        {
          builder.Append(geom.ToString(GeometryAsTextFormatType.KML));
        }
      }

      builder.Append("</MultiGeometry>");
      return builder.ToString();
    }

    /// <summary>
    /// Returns a WKT text representation of this spatial element.
    /// </summary>
    /// <returns>A WKT text representation of this spatial element.</returns>
    public override string ToWktString()
    {
      var geomType = GetType().Name.ToUpper();
      if (IsEmpty)
      {
        return geomType + " " + WKT_EMPTY_GEOM;
      }

      var builder = new StringBuilder(geomType);
      builder.Append(" (");
      if (!IsEmpty)
      {
        string separator = string.Empty;
        foreach (var geom in Geometries)
        {
          builder.Append(separator);
          builder.Append(geom.ToString(GeometryAsTextFormatType.WKT));
          separator = ",";
        }
      }

      builder.Append(")");
      return builder.ToString();
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
          regex = new WktGeometryCollectionRegex();
          break;

        case GeometryAsTextFormatType.KML:
          regex = new KmlGeometryCollectionRegex();
          break;

        case GeometryAsTextFormatType.GML:
          regex = new GmlGeometryCollectionRegex();
          break;

        case GeometryAsTextFormatType.GeoJSON:
          regex = new GeoJsonGeometryCollectionRegex();
          break;
      }

      return regex;
    }
  }
}
