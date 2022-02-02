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
  /// A <see cref="MultiCurve"/> geometry collection composed only of <see cref="LineString"/> elements.
  /// </summary>
  public class MultiLineString : MultiCurve
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="MultiLineString"/> class.
    /// </summary>
    /// <param name="lineStrings">An array of <see cref="LineString"/>s that compose the collection.</param>
    /// <param name="srid">The ID of the Spatial Reference System used by this <see cref="MultiLineString"/>.</param>
    public MultiLineString(LineString[] lineStrings, int srid)
      : this(srid)
    {
      if (lineStrings != null)
      {
        Geometries = new List<Geometry>(lineStrings);
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiLineString"/> class.
    /// </summary>
    /// <param name="lineStrings">A list of <see cref="LineString"/>s that compose the collection.</param>
    /// <param name="srid">The ID of the Spatial Reference System used by this <see cref="MultiLineString"/>.</param>
    public MultiLineString(List<LineString> lineStrings, int srid)
      : this(srid)
    {
      if (lineStrings != null)
      {
        Geometries = new List<Geometry>(lineStrings);
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiLineString"/> class.
    /// </summary>
    /// <param name="srid">The ID of the Spatial Reference System used by the new <see cref="MultiLineString"/>.</param>
    public MultiLineString(int srid)
      : base(srid)
    {
    }

    #region Properties

    /// <summary>
    /// Gets a value indicating whether all the <see cref="LineString"/>s contained in the collection are closed.
    /// </summary>
    public bool AreClosed
    {
      get
      {
        if (IsEmpty)
        {
          return false;
        }

        foreach (var geom in Geometries)
        {
          var lineString = geom as LineString;
          if (lineString == null)
          {
            continue;
          }

          if (!lineString.IsClosed)
          {
            return false;
          }
        }

        return true;
      }
    }

    /// <summary>
    /// Gets the number of dimensions used by this <see cref="MultiLineString"/>.
    /// </summary>
    public override int Dimensions
    {
      get
      {
        return 1;
      }
    }

    /// <summary>
    /// Gets the <see cref="GeometryType"/> of this <see cref="Geometry"/>.
    /// </summary>
    public override GeometryType Type
    {
      get
      {
        return GeometryType.MultiLineString;
      }
    }

    #endregion Properties

    /// <summary>
    /// Checks whether the given text is a valid representation of a <see cref="MultiLineString"/> in the given <see cref="GeometryAsTextFormatType"/>.
    /// </summary>
    /// <param name="geometryAsText">The string representation of a <see cref="MultiLineString"/> in the given format.</param>
    /// <param name="format">A <see cref="GeometryAsTextFormatType"/> value.</param>
    /// <returns><c>true</c> if the given text is a valid representation of a <see cref="MultiLineString"/> in the given <see cref="GeometryAsTextFormatType"/>, <c>false</c> otherwise.</returns>
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
    /// Converts the string representation of a <see cref="MultiLineString"/> in the given <see cref="GeometryAsTextFormatType"/> to a <see cref="MultiLineString"/> equivalent.
    /// </summary>
    /// <param name="geometryAsText">The string representation of a <see cref="MultiLineString"/> in the given format.</param>
    /// <param name="format">A <see cref="GeometryAsTextFormatType"/> value.</param>
    /// <returns>A <see cref="MultiLineString"/> equivalent to the string representation of a <see cref="MultiLineString"/> in the given <see cref="GeometryAsTextFormatType"/>.</returns>
    public new static MultiLineString Parse(string geometryAsText, GeometryAsTextFormatType format)
    {
      if (string.IsNullOrEmpty(geometryAsText) || format == GeometryAsTextFormatType.KML)
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

      var lineStringsGroup = format == GeometryAsTextFormatType.GML
        ? match.Groups["BasicGeometry"]
        : match.Groups["LineStringCoords"];
      if (!lineStringsGroup.Success)
      {
        return new MultiLineString(srid);
      }

      var lineStrings = new List<LineString>(lineStringsGroup.Captures.Count);
      foreach (var lineStringCapture in lineStringsGroup.Captures)
      {
        LineString lineString;
        if (format == GeometryAsTextFormatType.GML)
        {
          lineString = LineString.Parse(lineStringCapture.ToString(), format);
          lineString.SRID = srid;
        }
        else
        {
          lineString = LineString.ParseFromCoordsSet(lineStringCapture.ToString(), format, srid);
        }

        lineStrings.Add(lineString);
      }

      return lineStrings.Count == 0
        ? new MultiLineString(srid)
        : new MultiLineString(lineStrings, srid);
    }

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public override object Clone()
    {
      return IsEmpty
        ? new MultiLineString(SRID)
        : new MultiLineString(Geometries.Select(lineString => lineString.Clone() as LineString).ToList(), SRID);
    }

    /// <summary>
    /// Creates a <see cref="MultiLineString"/> with both the order of the component <see cref="LineString"/>s and the order of their <see cref="Coordinate"/>s are reversed.
    /// </summary>
    /// <returns>A <see cref="MultiLineString"/> with both the order of the component <see cref="LineString"/>s and the order of their <see cref="Coordinate"/>s are reversed.</returns>
    public new MultiLineString Reverse()
    {
      if (IsEmpty)
      {
        return new MultiLineString(SRID);
      }

      var reversedGeometries = new List<Geometry>(Geometries);
      reversedGeometries.Reverse();
      var reversedLineStrings = new List<LineString>(reversedGeometries.Count);
      foreach (var geom in reversedGeometries)
      {
        var lineString = geom as LineString;
        if (lineString == null)
        {
          continue;
        }

        reversedLineStrings.Add(lineString.Reverse());
      }

      return new MultiLineString(reversedLineStrings, SRID);
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
        builder.Append("[] }");
        return builder.ToString();
      }

      builder.Append("[ ");
      string separator = string.Empty;
      foreach (var geom in Geometries)
      {
        var lineString = geom as LineString;
        if (lineString == null)
        {
          continue;
        }

        builder.Append(separator);
        builder.Append(lineString.IsEmpty ? GEOJSON_EMPTY_GEOM : lineString.GetCoordinatesAsGeoJsonString());
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
          var lineString = geom as LineString;
          if (lineString == null)
          {
            continue;
          }

          builder.Append("<gml:lineStringMember>");
          builder.Append(lineString.ToGmlString(false));
          builder.Append("</gml:lineStringMember>");
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
        var lineString = geom as LineString;
        if (lineString == null)
        {
          continue;
        }

        builder.Append(separator);
        builder.Append(lineString.IsEmpty ? WKT_EMPTY_GEOM : lineString.GetCoordinatesAsWktString());
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
          regex = new WktMultiLineStringRegex();
          break;

        case GeometryAsTextFormatType.KML:
          regex = new KmlGeometryCollectionRegex();
          break;

        case GeometryAsTextFormatType.GML:
          regex = new GmlMultiLineStringRegex();
          break;

        case GeometryAsTextFormatType.GeoJSON:
          regex = new GeoJsonMultiLineStringRegex();
          break;
      }

      return regex;
    }
  }
}
