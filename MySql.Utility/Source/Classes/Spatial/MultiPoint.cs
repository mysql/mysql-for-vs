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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MySql.Utility.Enums;

namespace MySql.Utility.Classes.Spatial
{
  /// <summary>
  /// Represents a <see cref="GeometryCollection"/> composed of only Point elements.
  /// The points are not connected or ordered in any way.
  /// </summary>
  public class MultiPoint : GeometryCollection
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="MultiPoint"/> class.
    /// </summary>
    /// <param name="points">An array of <see cref="Point"/>s that compose the collection.</param>
    /// <param name="srid">The ID of the Spatial Reference System used by this <see cref="MultiPoint"/>.</param>
    public MultiPoint(Point[] points, int srid)
      : this(srid)
    {
      if (points != null)
      {
        Geometries = new List<Geometry>(points);
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiPoint"/> class.
    /// </summary>
    /// <param name="points">A list of <see cref="Point"/>s that compose the collection.</param>
    /// <param name="srid">The ID of the Spatial Reference System used by this <see cref="MultiPoint"/>.</param>
    public MultiPoint(List<Point> points, int srid)
      : this (srid)
    {
      if (points != null)
      {
        Geometries = new List<Geometry>(points);
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiPoint"/> class.
    /// </summary>
    /// <param name="srid">The ID of the Spatial Reference System used by the new <see cref="MultiPoint"/>.</param>
    public MultiPoint(int srid)
      : base(srid)
    {
    }

    #region Properties

    /// <summary>
    /// Gets the <see cref="GeometryType"/> of this <see cref="Geometry"/>.
    /// </summary>
    public override GeometryType Type
    {
      get
      {
        return GeometryType.MultiPoint;
      }
    }

    #endregion Properties

    /// <summary>
    /// Checks whether the given text is a valid representation of a <see cref="MultiPoint"/> in the given <see cref="GeometryAsTextFormatType"/>.
    /// </summary>
    /// <param name="geometryAsText">The string representation of a <see cref="MultiPoint"/> in the given format.</param>
    /// <param name="format">A <see cref="GeometryAsTextFormatType"/> value.</param>
    /// <returns><c>true</c> if the given text is a valid representation of a <see cref="MultiPoint"/> in the given <see cref="GeometryAsTextFormatType"/>, <c>false</c> otherwise.</returns>
    public new static bool IsValid(string geometryAsText, GeometryAsTextFormatType format)
    {
      if (string.IsNullOrEmpty(geometryAsText))
      {
        return false;
      }

      var regex = GetRegex(format);
      return regex == null ? false : regex.IsMatch(geometryAsText);
    }

    /// <summary>
    /// Converts the string representation of a <see cref="MultiPoint"/> in the given <see cref="GeometryAsTextFormatType"/> to a <see cref="MultiPoint"/> equivalent.
    /// </summary>
    /// <param name="geometryAsText">The string representation of a <see cref="MultiPoint"/> in the given format.</param>
    /// <param name="format">A <see cref="GeometryAsTextFormatType"/> value.</param>
    /// <returns>A <see cref="MultiPoint"/> equivalent to the string representation of a <see cref="MultiPoint"/> in the given <see cref="GeometryAsTextFormatType"/>.</returns>
    public new static MultiPoint Parse(string geometryAsText, GeometryAsTextFormatType format)
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

      var pointsGroup = format == GeometryAsTextFormatType.GML
        ? match.Groups["BasicGeometry"]
        : match.Groups["PointCoord"];
      if (!pointsGroup.Success)
      {
        return new MultiPoint(srid);
      }

      var points = new List<Point>(pointsGroup.Captures.Count);
      foreach (var pointCapture in pointsGroup.Captures)
      {
        Point point;
        switch (format)
        {
          case GeometryAsTextFormatType.GML:
            point = Point.Parse(pointCapture.ToString(), format);
            point.SRID = srid;
            break;

          case GeometryAsTextFormatType.WKT:
            point = Point.ParseFromWktMultiPointCoord(pointCapture.ToString(), srid);
            break;

          default:
            point = Point.ParseFromCoord(pointCapture.ToString(), format, srid);
            break;
        }

        points.Add(point);
      }

      return points.Count == 0
        ? new MultiPoint(srid)
        : new MultiPoint(points, srid);
    }

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public override object Clone()
    {
      return IsEmpty
        ? new MultiPoint(SRID)
        : new MultiPoint(Geometries.Select(point => point.Clone() as Point).ToList(), SRID);
    }

    /// <summary>
    /// Returns a GeoJSON text representation of this spatial element.
    /// </summary>
    /// <returns>A GeoJSON text representation of this spatial element.</returns>
    public override string ToGeoJsonString()
    {
      var className = GetGeometryType(GeometryAsTextFormatType.GeoJSON);
      var builder = new StringBuilder();
      builder.AppendFormat("{{ \"type\": \"{0}\", \"coordinates\": ", className);
      if (IsEmpty)
      {
        builder.Append(GEOJSON_EMPTY_GEOM);
        builder.Append(" }");
        return builder.ToString();
      }

      builder.Append("[ ");
      string separator = string.Empty;
      foreach (var geom in Geometries)
      {
        var point = geom as Point;
        if (point == null)
        {
          continue;
        }

        builder.Append(separator);
        builder.Append(point.IsEmpty ? GEOJSON_EMPTY_GEOM : point.Coordinate.ToGeoJsonString());
        separator = ", ";
      }

      builder.Append(" ] }");
      return builder.ToString();
    }

    /// <summary>
    /// Returns a GML text representation of this spatial element.
    /// </summary>
    /// <returns>A GML text representation of this spatial element.</returns>
    public override string ToGmlString()
    {
      var className = GetGeometryType(GeometryAsTextFormatType.GML);
      var builder = new StringBuilder();
      builder.AppendFormat("<gml:{0} srsName=\"EPSG:4326\">", className);
      if (!IsEmpty)
      {
        foreach (var geometry in Geometries)
        {
          var point = geometry as Point;
          if (point == null)
          {
            continue;
          }

          builder.Append("<gml:pointMember>");
          builder.Append(point.ToGmlString(false));
          builder.Append("</gml:pointMember>");
        }
      }

      builder.AppendFormat("</gml:{0}>", className);
      return builder.ToString();
    }

    /// <summary>
    /// Returns a WKT text representation of this spatial element.
    /// </summary>
    /// <returns>A WKT text representation of this spatial element.</returns>
    public override string ToWktString()
    {
      var className = GetGeometryType(GeometryAsTextFormatType.WKT);
      if (IsEmpty)
      {
        return className + " " + WKT_EMPTY_GEOM;
      }

      var builder = new StringBuilder(className);
      builder.Append(" (");
      string separator = string.Empty;
      foreach (var geom in Geometries)
      {
        var point = geom as Point;
        if (point == null)
        {
          continue;
        }

        builder.Append(separator);
        builder.Append(point.IsEmpty ? WKT_EMPTY_GEOM : point.Coordinate.ToWktString());
        separator = ",";
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
          regex = new WktMultiPointRegex();
          break;

        case GeometryAsTextFormatType.KML:
          regex = new KmlGeometryCollectionRegex();
          break;

        case GeometryAsTextFormatType.GML:
          regex = new GmlMultiPointRegex();
          break;

        case GeometryAsTextFormatType.GeoJSON:
          regex = new GeoJsonMultiPointRegex();
          break;
      }

      return regex;
    }
  }
}
