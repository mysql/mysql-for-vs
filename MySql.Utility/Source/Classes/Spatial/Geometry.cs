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
using System.Text.RegularExpressions;
using MySql.Utility.Enums;
using MySql.Utility.Interfaces;

namespace MySql.Utility.Classes.Spatial
{
  /// <summary>
  /// Represents a planar, linear vector geometry and the base for other spatial classes.
  /// </summary>
  public abstract class Geometry : ISpatialElement, ICloneable
  {
    #region Constants

    /// <summary>
    /// The text identifier for empty geometries in GeoJSON.
    /// </summary>
    public const string GEOJSON_EMPTY_GEOM = "[ ]";

    /// <summary>
    /// The text identifier for empty geometries in WKT.
    /// </summary>
    public const string WKT_EMPTY_GEOM = "EMPTY";

    #endregion Constants

    /// <summary>
    /// Initializes a new instance of the <see cref="Geometry"/> class.
    /// </summary>
    /// <param name="srid">The ID of the Spatial Reference System used by the <see cref="Geometry"/>.</param>
    protected Geometry(int srid)
    {
      MetaData = null;
      SRID = srid;
      TextFormat = GeometryAsTextFormatType.WKT;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Geometry"/> class.
    /// </summary>
    protected Geometry()
      : this(0)
    {
    }

    #region Properties

    /// <summary>
    /// Gets the number of <see cref="Coordinate"/>s the geometry is made of.
    /// </summary>
    public abstract int CoordinatesCount { get; }

    /// <summary>
    /// Gets the number of dimensions used by this <see cref="Geometry"/>.
    /// </summary>
    public abstract int Dimensions { get; }

    /// <summary>
    /// Gets a value indicating whether this <see cref="Geometry"/> is an empty one.
    /// </summary>
    public abstract bool IsEmpty { get; }

    /// <summary>
    /// Gets a value indicating whether this <see cref="Geometry"/> is a simple one.
    /// </summary>
    public abstract bool IsSimple { get; }

    /// <summary>
    /// Gets or sets user defined meta data related to this geometry.
    /// </summary>
    public object MetaData { get; set; }

    /// <summary>
    /// Gets or sets the ID of the Spatial Reference System used by this <see cref="Geometry"/>.
    /// </summary>
    public int SRID { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="GeometryAsTextFormatType"/> used to get a string representation of this <see cref="Geometry"/> when calling <see cref="ToString"/>.
    /// </summary>
    public GeometryAsTextFormatType TextFormat { get; set; }

    /// <summary>
    /// Gets the <see cref="GeometryType"/> of this <see cref="Geometry"/>.
    /// </summary>
    public abstract GeometryType Type { get; }

    #endregion Properties

    /// <summary>
    /// Checks whether the given text is a valid representation of a <see cref="Geometry"/> in the given <see cref="GeometryAsTextFormatType"/>.
    /// </summary>
    /// <param name="geometryAsText">The string representation of a <see cref="Geometry"/> in the given format.</param>
    /// <param name="format">A <see cref="GeometryAsTextFormatType"/> value.</param>
    /// <returns><c>true</c> if the given text is a valid representation of a <see cref="Geometry"/> in the given <see cref="GeometryAsTextFormatType"/>, <c>false</c> otherwise.</returns>
    public static bool IsValid(string geometryAsText, GeometryAsTextFormatType format)
    {
      string geometryClass;
      return IsValid(geometryAsText, format, out geometryClass);
    }

    /// <summary>
    /// Checks whether the given text is a valid representation of a <see cref="Geometry"/> in the given <see cref="GeometryAsTextFormatType"/>.
    /// </summary>
    /// <param name="geometryAsText">The string representation of a <see cref="Geometry"/> in the given format.</param>
    /// <param name="format">A <see cref="GeometryAsTextFormatType"/> value.</param>
    /// <param name="geometryClass">The name of the <see cref="Geometry"/> type.</param>
    /// <returns><c>true</c> if the given text is a valid representation of a <see cref="Geometry"/> in the given <see cref="GeometryAsTextFormatType"/>, <c>false</c> otherwise.</returns>
    public static bool IsValid(string geometryAsText, GeometryAsTextFormatType format, out string geometryClass)
    {
      geometryClass = null;
      if (string.IsNullOrEmpty(geometryAsText))
      {
        return false;
      }

      var allowedGeomsRegex = GetAllowedGeomsRegex(format);
      var match = allowedGeomsRegex.Match(geometryAsText);
      if (!match.Success)
      {
        return false;
      }

      var classGroup = match.Groups["Class"];
      if (!classGroup.Success)
      {
        return false;
      }

      geometryClass = classGroup.Captures[0].ToString();
      switch (geometryClass.ToLowerInvariant())
      {
        case "point":
          return Point.IsValid(geometryAsText, format);

        case "linestring":
          return LineString.IsValid(geometryAsText, format);

        case "polygon":
          return Polygon.IsValid(geometryAsText, format);

        case "multipoint":
          return MultiPoint.IsValid(geometryAsText, format);

        case "multilinestring":
          return MultiLineString.IsValid(geometryAsText, format);

        case "multipolygon":
          return MultiPolygon.IsValid(geometryAsText, format);

        case "multigeometry":
          geometryClass = typeof(GeometryCollection).Name;
          return GeometryCollection.IsValid(geometryAsText, format);

        case "geometrycollection":
          return GeometryCollection.IsValid(geometryAsText, format);
      }

      return false;
    }

    /// <summary>
    /// Converts the string representation of a <see cref="Geometry"/> in the given <see cref="GeometryAsTextFormatType"/> to a <see cref="Geometry"/> equivalent.
    /// </summary>
    /// <param name="geometryAsText">The string representation of a <see cref="Geometry"/> in the given format.</param>
    /// <param name="format">A <see cref="GeometryAsTextFormatType"/> value.</param>
    /// <returns>A <see cref="Geometry"/> equivalent to the string representation of a <see cref="Geometry"/> in the given <see cref="GeometryAsTextFormatType"/>.</returns>
    public static Geometry Parse(string geometryAsText, GeometryAsTextFormatType format)
    {
      if (string.IsNullOrEmpty(geometryAsText))
      {
        return null;
      }

      var allowedGeomsRegex = GetAllowedGeomsRegex(format);
      var match = allowedGeomsRegex.Match(geometryAsText);
      if (!match.Success)
      {
        return null;
      }

      var classGroup = match.Groups["Class"];
      if (!classGroup.Success)
      {
        return null;
      }

      var geomClass = classGroup.Captures[0].ToString().ToLowerInvariant();
      switch (geomClass)
      {
        case "point":
          return Point.Parse(geometryAsText, format);

        case "linestring":
          return LineString.Parse(geometryAsText, format);

        case "polygon":
          return Polygon.Parse(geometryAsText, format);

        case "multipoint":
          return MultiPoint.Parse(geometryAsText, format);

        case "multilinestring":
          return MultiLineString.Parse(geometryAsText, format);

        case "multipolygon":
          return MultiPolygon.Parse(geometryAsText, format);

        case "multigeometry":
        case "geometrycollection":
          return GeometryCollection.Parse(geometryAsText, format);
      }

      return null;
    }

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public virtual object Clone()
    {
      var point = this as Point;
      if (point != null)
      {
        return point.Clone();
      }

      var lineString = this as LineString;
      if (lineString != null)
      {
        return lineString.Clone();
      }

      var polygon = this as Polygon;
      if (polygon != null)
      {
        return polygon.Clone();
      }

      var multiPoint = this as MultiPoint;
      if (multiPoint != null)
      {
        return multiPoint.Clone();
      }

      var multiLineString = this as MultiLineString;
      if (multiLineString != null)
      {
        return multiLineString.Clone();
      }

      var multiPolygon = this as MultiPolygon;
      if (multiPolygon != null)
      {
        return multiPolygon.Clone();
      }

      var geometryCollection = this as GeometryCollection;
      if (geometryCollection != null)
      {
        return geometryCollection.Clone();
      }

      return MemberwiseClone();
    }

    /// <summary>
    /// Calculates the <see cref="BoundingBox"/> for this <see cref="Geometry"/>.
    /// </summary>
    /// <returns>The <see cref="BoundingBox"/> for this <see cref="Geometry"/>.</returns>
    public abstract BoundingBox GetBoundingBox();

    /// <summary>
    /// Returns the type of the <see cref="Geometry"/> based on the given <see cref="GeometryAsTextFormatType"/>.
    /// </summary>
    /// <param name="format">A <see cref="GeometryAsTextFormatType"/> value.</param>
    /// <returns>The type of the <see cref="Geometry"/> based on the given <see cref="GeometryAsTextFormatType"/>.</returns>
    public virtual string GetGeometryType(GeometryAsTextFormatType format)
    {
      var className = GetType().Name;
      switch (format)
      {
        case GeometryAsTextFormatType.WKT:
          return className.ToUpper();

        default:
          return className;
      }
    }

    /// <summary>
    /// Returns a GeoJSON text representation of this spatial element.
    /// </summary>
    /// <returns>A GeoJSON text representation of this spatial element.</returns>
    public virtual string ToGeoJsonString()
    {
      var point = this as Point;
      if (point != null)
      {
        return point.ToGeoJsonString();
      }

      var lineString = this as LineString;
      if (lineString != null)
      {
        return lineString.ToGeoJsonString();
      }

      var polygon = this as Polygon;
      if (polygon != null)
      {
        return polygon.ToGeoJsonString();
      }

      var multiPoint = this as MultiPoint;
      if (multiPoint != null)
      {
        return multiPoint.ToGeoJsonString();
      }

      var multiLineString = this as MultiLineString;
      if (multiLineString != null)
      {
        return multiLineString.ToGeoJsonString();
      }

      var multiPolygon = this as MultiPolygon;
      if (multiPolygon != null)
      {
        return multiPolygon.ToGeoJsonString();
      }

      var geometryCollection = this as GeometryCollection;
      if (geometryCollection != null)
      {
        return geometryCollection.ToGeoJsonString();
      }

      return string.Empty;
    }

    /// <summary>
    /// Returns a GML text representation of this spatial element.
    /// </summary>
    /// <returns>A GML text representation of this spatial element.</returns>
    public virtual string ToGmlString()
    {
      var point = this as Point;
      if (point != null)
      {
        return point.ToGmlString();
      }

      var lineString = this as LineString;
      if (lineString != null)
      {
        return lineString.ToGmlString();
      }

      var polygon = this as Polygon;
      if (polygon != null)
      {
        return polygon.ToGmlString();
      }

      var multiPoint = this as MultiPoint;
      if (multiPoint != null)
      {
        return multiPoint.ToGmlString();
      }

      var multiLineString = this as MultiLineString;
      if (multiLineString != null)
      {
        return multiLineString.ToGmlString();
      }

      var multiPolygon = this as MultiPolygon;
      if (multiPolygon != null)
      {
        return multiPolygon.ToGmlString();
      }

      var geometryCollection = this as GeometryCollection;
      if (geometryCollection != null)
      {
        return geometryCollection.ToGmlString();
      }

      return string.Empty;
    }

    /// <summary>
    /// Returns a KML text representation of this spatial element.
    /// </summary>
    /// <returns>A KML text representation of this spatial element.</returns>
    public virtual string ToKmlString()
    {
      var point = this as Point;
      if (point != null)
      {
        return point.ToKmlString();
      }

      var lineString = this as LineString;
      if (lineString != null)
      {
        return lineString.ToKmlString();
      }

      var polygon = this as Polygon;
      if (polygon != null)
      {
        return polygon.ToKmlString();
      }

      var multiPoint = this as MultiPoint;
      if (multiPoint != null)
      {
        return multiPoint.ToKmlString();
      }

      var multiLineString = this as MultiLineString;
      if (multiLineString != null)
      {
        return multiLineString.ToKmlString();
      }

      var multiPolygon = this as MultiPolygon;
      if (multiPolygon != null)
      {
        return multiPolygon.ToKmlString();
      }

      var geometryCollection = this as GeometryCollection;
      if (geometryCollection != null)
      {
        return geometryCollection.ToKmlString();
      }

      return string.Empty;
    }

    /// <summary>
    /// Returns a string that represents the current <see cref="Geometry"/>.
    /// </summary>
    /// <returns>A string that represents the current <see cref="Geometry"/>.</returns>
    public override string ToString()
    {
      return ToString(TextFormat);
    }

    /// <summary>
    /// Returns a text representation of this spatial element.
    /// </summary>
    /// <param name="format">A <see cref="GeometryAsTextFormatType"/> value.</param>
    /// <returns>A text representation of this spatial element.</returns>
    public virtual string ToString(GeometryAsTextFormatType format)
    {
      switch (format)
      {
        case GeometryAsTextFormatType.WKT:
          return ToWktString();

        case GeometryAsTextFormatType.KML:
          return ToKmlString();

        case GeometryAsTextFormatType.GML:
          return ToGmlString();

        case GeometryAsTextFormatType.GeoJSON:
          return ToGeoJsonString();
      }

      return string.Empty;
    }

    /// <summary>
    /// Returns a WKT text representation of this spatial element.
    /// </summary>
    /// <returns>A WKT text representation of this spatial element.</returns>
    public virtual string ToWktString()
    {
      var point = this as Point;
      if (point != null)
      {
        return point.ToWktString();
      }

      var lineString = this as LineString;
      if (lineString != null)
      {
        return lineString.ToWktString();
      }

      var polygon = this as Polygon;
      if (polygon != null)
      {
        return polygon.ToWktString();
      }

      var multiPoint = this as MultiPoint;
      if (multiPoint != null)
      {
        return multiPoint.ToWktString();
      }

      var multiLineString = this as MultiLineString;
      if (multiLineString != null)
      {
        return multiLineString.ToWktString();
      }

      var multiPolygon = this as MultiPolygon;
      if (multiPolygon != null)
      {
        return multiPolygon.ToWktString();
      }

      var geometryCollection = this as GeometryCollection;
      if (geometryCollection != null)
      {
        return geometryCollection.ToWktString();
      }

      return string.Empty;
    }

    /// <summary>
    /// Returns a compiled regex corresponding to the given <see cref="GeometryAsTextFormatType"/> specific to this geometry class.
    /// </summary>
    /// <param name="format">A <see cref="GeometryAsTextFormatType"/> value.</param>
    /// <returns>A compiled regex corresponding to the given <see cref="GeometryAsTextFormatType"/> specific to this geometry class.</returns>
    protected static Regex GetAllowedGeomsRegex(GeometryAsTextFormatType format)
    {
      Regex regex = null;
      switch (format)
      {
        case GeometryAsTextFormatType.WKT:
          regex = new WktAllowedGeomClassesRegex();
          break;

        case GeometryAsTextFormatType.KML:
          regex = new KmlAllowedGeomClassesRegex();
          break;

        case GeometryAsTextFormatType.GML:
          regex = new GmlAllowedGeomClassesRegex();
          break;

        case GeometryAsTextFormatType.GeoJSON:
          regex = new GeoJsonAllowedGeomClassesRegex();
          break;
      }

      return regex;
    }
  }
}
