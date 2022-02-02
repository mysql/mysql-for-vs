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
  /// A <see cref="MultiSurface"/> composed only of <see cref="Polygon"/> elements.
  /// </summary>
  public class MultiPolygon : MultiSurface
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="MultiPolygon"/> class.
    /// </summary>
    /// <param name="polygons">An array of <see cref="Polygon"/>s that compose the collection.</param>
    /// <param name="srid">The ID of the Spatial Reference System used by this <see cref="MultiPolygon"/>.</param>
    public MultiPolygon(Polygon[] polygons, int srid)
      : this(srid)
    {
      if (polygons != null)
      {
        Geometries = new List<Geometry>(polygons);
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiPolygon"/> class.
    /// </summary>
    /// <param name="polygons">A list of <see cref="Polygon"/>s that compose the collection.</param>
    /// <param name="srid">The ID of the Spatial Reference System used by this <see cref="MultiPolygon"/>.</param>
    public MultiPolygon(List<Polygon> polygons, int srid)
      : this(srid)
    {
      if (polygons != null)
      {
        Geometries = new List<Geometry>(polygons);
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiPolygon"/> class.
    /// </summary>
    /// <param name="srid">The ID of the Spatial Reference System used by the new <see cref="MultiPolygon"/>.</param>
    public MultiPolygon(int srid)
      : base(srid)
    {
    }

    #region Properties

    /// <summary>
    /// Gets the number of dimensions used by this <see cref="MultiLineString"/>.
    /// </summary>
    public override int Dimensions
    {
      get
      {
        return 2;
      }
    }

    /// <summary>
    /// Gets the <see cref="GeometryType"/> of this <see cref="Geometry"/>.
    /// </summary>
    public override GeometryType Type
    {
      get
      {
        return GeometryType.MultiPolygon;
      }
    }

    #endregion Properties

    /// <summary>
    /// Checks whether the given text is a valid representation of a <see cref="MultiPolygon"/> in the given <see cref="GeometryAsTextFormatType"/>.
    /// </summary>
    /// <param name="geometryAsText">The string representation of a <see cref="MultiPolygon"/> in the given format.</param>
    /// <param name="format">A <see cref="GeometryAsTextFormatType"/> value.</param>
    /// <returns><c>true</c> if the given text is a valid representation of a <see cref="MultiPolygon"/> in the given <see cref="GeometryAsTextFormatType"/>, <c>false</c> otherwise.</returns>
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
    /// Converts the string representation of a <see cref="MultiPolygon"/> in the given <see cref="GeometryAsTextFormatType"/> to a <see cref="MultiPolygon"/> equivalent.
    /// </summary>
    /// <param name="geometryAsText">The string representation of a <see cref="MultiPolygon"/> in the given format.</param>
    /// <param name="format">A <see cref="GeometryAsTextFormatType"/> value.</param>
    /// <returns>A <see cref="MultiPolygon"/> equivalent to the string representation of a <see cref="MultiPolygon"/> in the given <see cref="GeometryAsTextFormatType"/>.</returns>
    public new static MultiPolygon Parse(string geometryAsText, GeometryAsTextFormatType format)
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

      var polygonsGroup = format == GeometryAsTextFormatType.GML
        ? match.Groups["BasicGeometry"]
        : match.Groups["PolygonRings"];
      if (!polygonsGroup.Success)
      {
        return new MultiPolygon(srid);
      }

      var polygons = new List<Polygon>(polygonsGroup.Captures.Count);
      foreach (var polygonCapture in polygonsGroup.Captures)
      {
        Polygon polygon;
        if (format == GeometryAsTextFormatType.GML)
        {
          polygon = Polygon.Parse(polygonCapture.ToString(), format);
          polygon.SRID = srid;
        }
        else
        {
          polygon = Polygon.ParseFromRings(polygonCapture.ToString(), format, srid);
        }

        polygons.Add(polygon);
      }

      return polygons.Count == 0
        ? new MultiPolygon(srid)
        : new MultiPolygon(polygons, srid);
    }

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public override object Clone()
    {
      return IsEmpty
        ? new MultiPolygon(SRID)
        : new MultiPolygon(Geometries.Select(polygon => polygon.Clone() as Polygon).ToList(), SRID);
    }

    /// <summary>
    /// Creates a <see cref="MultiPolygon"/> with the order of the <see cref="Coordinate"/>s in each <see cref="Polygon"/> reversed.
    /// The order of the <see cref="Polygon"/>s within the collection is not reversed.
    /// </summary>
    /// <returns>A <see cref="MultiPolygon"/> with the order of the <see cref="Coordinate"/>s in each <see cref="Polygon"/> reversed.</returns>
    public new MultiPolygon Reverse()
    {
      return Reverse(false);
    }

    /// <summary>
    /// Creates a <see cref="MultiPolygon"/> with the order of the <see cref="Coordinate"/>s in each <see cref="Polygon"/> reversed.
    /// Optionally the order of the <see cref="Polygon"/>s within the collection can be reversed as well.
    /// </summary>
    /// <param name="reverseElementsOrder">Flag indicating whether the order of the elements within the collection is reversed as well.</param>
    /// <returns>A <see cref="MultiPolygon"/> with the order of the <see cref="Coordinate"/>s in each <see cref="Polygon"/> reversed.</returns>
    public MultiPolygon Reverse(bool reverseElementsOrder)
    {
      if (IsEmpty)
      {
        return new MultiPolygon(SRID);
      }

      var reversedGeometries = new List<Geometry>(Geometries);
      if (reverseElementsOrder)
      {
        reversedGeometries.Reverse();
      }

      var reversedPolygons = new List<Polygon>(reversedGeometries.Count);
      foreach (var geom in reversedGeometries)
      {
        var polygon = geom as Polygon;
        if (polygon == null)
        {
          continue;
        }

        reversedPolygons.Add(polygon.Reverse());
      }

      return new MultiPolygon(reversedPolygons, SRID);
    }

    /// <summary>
    /// Returns a GeoJSON text representation of this spatial element.
    /// </summary>
    /// <returns>A GeoJSON text representation of this spatial element.</returns>
    public override string ToGeoJsonString()
    {
      var className = GetGeometryType(GeometryAsTextFormatType.GML);
      var builder = new StringBuilder();
      builder.AppendFormat("{{ \"type\": \"{0}\", \"coordinates\": ", className);
      if (IsEmpty)
      {
        builder.Append("[] }");
        return builder.ToString();
      }

      builder.Append("[ ");
      string separator = string.Empty;
      foreach (var geom in Geometries)
      {
        var polygon = geom as Polygon;
        if (polygon == null)
        {
          continue;
        }

        builder.Append(separator);
        builder.Append(polygon.IsEmpty ? GEOJSON_EMPTY_GEOM : polygon.GetCoordinatesAsGeoJsonString());
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
        foreach (var geom in Geometries)
        {
          var polygon = geom as Polygon;
          if (polygon == null)
          {
            continue;
          }

          builder.Append("<gml:polygonMember>");
          builder.Append(polygon.ToGmlString(false));
          builder.Append("</gml:polygonMember>");
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
        var polygon = geom as Polygon;
        if (polygon == null)
        {
          continue;
        }

        builder.Append(separator);
        builder.Append(polygon.IsEmpty ? WKT_EMPTY_GEOM : polygon.GetCoordinatesAsWktString());
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
          regex = new WktMultiPolygonRegex();
          break;

        case GeometryAsTextFormatType.KML:
          regex = new KmlGeometryCollectionRegex();
          break;

        case GeometryAsTextFormatType.GML:
          regex = new GmlMultiPolygonRegex();
          break;

        case GeometryAsTextFormatType.GeoJSON:
          regex = new GeoJsonMultiPolygonRegex();
          break;
      }

      return regex;
    }
  }
}
