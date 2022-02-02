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
using MySql.Utility.Classes;
using MySql.Utility.Classes.Spatial;
using Xunit;

namespace MySql.Utility.RegularExpressions.Tests
{
  /// <summary>
  /// Tests for the regexes contained in the MySql.Utility.RegularExpressions.dll.
  /// </summary>
  public class RegexTests
  {
    /// <summary>
    /// Performs unit tests on the regex classes that handle text in GeoJSON format.
    /// </summary>
    [Fact]
    public void TestGeoJsonRegexes()
    {
      // Read GeoJSON data file
      var geoJsonData = Utilities.GetScriptFromResource("MySql.Utility.RegularExpressions.Tests.Properties.GeoJsonData.txt");
      Assert.True(!string.IsNullOrEmpty(geoJsonData));
      var geoJsonLines = geoJsonData.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
      Assert.True(geoJsonLines.Length == 56);

      // Allowed Geoms
      var allowedGeomsRegex = new GeoJsonAllowedGeomClassesRegex();
      var points = 0;
      var lineStrings = 0;
      var polygons = 0;
      var multiPoints = 0;
      var multiLineStrings = 0;
      var multiPolygons = 0;
      var geometryCollections = 0;
      foreach (var geoJsonLine in geoJsonLines)
      {
        var match = allowedGeomsRegex.Match(geoJsonLine);
        if (!match.Success)
        {
          continue;
        }

        switch (match.Groups["Class"].Value.ToLowerInvariant())
        {
          case "point":
            points++;
            break;

          case "linestring":
            lineStrings++;
            break;

          case "polygon":
            polygons++;
            break;

          case "multipoint":
            multiPoints++;
            break;

          case "multilinestring":
            multiLineStrings++;
            break;

          case "multipolygon":
            multiPolygons++;
            break;

          case "geometrycollection":
            geometryCollections++;
            break;
        }
      }

      Assert.True(points == 8);
      Assert.True(lineStrings == 8);
      Assert.True(polygons == 8);
      Assert.True(multiPoints == 8);
      Assert.True(multiLineStrings == 8);
      Assert.True(multiPolygons == 8);
      Assert.True(geometryCollections == 8);

      // Point
      var geoJsonPointRegex = new GeoJsonPointRegex();
      for (int i = 0; i < 4; i++)
      {
        Assert.True(geoJsonPointRegex.IsMatch(geoJsonLines[i]));
      }

      for (int i = 4; i < 8; i++)
      {
        Assert.False(geoJsonPointRegex.IsMatch(geoJsonLines[i]));
      }

      // LineString
      var geoJsonLineStringRegex = new GeoJsonLineStringRegex();
      for (int i = 8; i < 12; i++)
      {
        Assert.True(geoJsonLineStringRegex.IsMatch(geoJsonLines[i]));
      }

      for (int i = 12; i < 16; i++)
      {
        Assert.False(geoJsonLineStringRegex.IsMatch(geoJsonLines[i]));
      }

      // Polygon
      var geoJsonPolygonRegex = new GeoJsonPolygonRegex();
      for (int i = 16; i < 20; i++)
      {
        Assert.True(geoJsonPolygonRegex.IsMatch(geoJsonLines[i]));
      }

      for (int i = 20; i < 24; i++)
      {
        Assert.False(geoJsonPolygonRegex.IsMatch(geoJsonLines[i]));
      }

      // MultiPoint
      var geoJsonMultiPointRegex = new GeoJsonMultiPointRegex();
      for (int i = 24; i < 28; i++)
      {
        Assert.True(geoJsonMultiPointRegex.IsMatch(geoJsonLines[i]));
      }

      for (int i = 28; i < 32; i++)
      {
        Assert.False(geoJsonMultiPointRegex.IsMatch(geoJsonLines[i]));
      }

      // MultiLineString
      var geoJsonMultiLineStringRegex = new GeoJsonMultiLineStringRegex();
      for (int i = 32; i < 36; i++)
      {
        Assert.True(geoJsonMultiLineStringRegex.IsMatch(geoJsonLines[i]));
      }

      for (int i = 36; i < 40; i++)
      {
        Assert.False(geoJsonMultiLineStringRegex.IsMatch(geoJsonLines[i]));
      }

      // MultiPolygon
      var geoJsonMultiPolygonRegex = new GeoJsonMultiPolygonRegex();
      for (int i = 40; i < 44; i++)
      {
        Assert.True(geoJsonMultiPolygonRegex.IsMatch(geoJsonLines[i]));
      }

      for (int i = 44; i < 48; i++)
      {
        Assert.False(geoJsonMultiPolygonRegex.IsMatch(geoJsonLines[i]));
      }

      // GeometryCollection
      var geoJsonGeometryCollectionRegex = new GeoJsonGeometryCollectionRegex();
      for (int i = 48; i < 52; i++)
      {
        Assert.True(geoJsonGeometryCollectionRegex.IsMatch(geoJsonLines[i]));
      }

      for (int i = 52; i < 56; i++)
      {
        Assert.False(geoJsonGeometryCollectionRegex.IsMatch(geoJsonLines[i]));
      }
    }

    /// <summary>
    /// Performs unit tests on the regex classes that handle text in GML format.
    /// </summary>
    [Fact]
    public void TestGmlRegexes()
    {
      // Read GML data file
      var gmlData = Utilities.GetScriptFromResource("MySql.Utility.RegularExpressions.Tests.Properties.GmlData.txt");
      Assert.True(!string.IsNullOrEmpty(gmlData));
      var gmlLines = gmlData.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
      Assert.True(gmlLines.Length == 56);

      // Allowed Geoms
      var allowedGeomsRegex = new GmlAllowedGeomClassesRegex();
      var points = 0;
      var lineStrings = 0;
      var polygons = 0;
      var multiPoints = 0;
      var multiLineStrings = 0;
      var multiPolygons = 0;
      var geometryCollections = 0;
      foreach (var gmlLine in gmlLines)
      {
        var match = allowedGeomsRegex.Match(gmlLine);
        if (!match.Success)
        {
          continue;
        }

        switch (match.Groups["Class"].Value.ToLowerInvariant())
        {
          case "point":
            points++;
            break;

          case "linestring":
            lineStrings++;
            break;

          case "polygon":
            polygons++;
            break;

          case "multipoint":
            multiPoints++;
            break;

          case "multilinestring":
            multiLineStrings++;
            break;

          case "multipolygon":
            multiPolygons++;
            break;

          case "multigeometry":
            geometryCollections++;
            break;
        }
      }

      Assert.True(points == 8);
      Assert.True(lineStrings == 8);
      Assert.True(polygons == 8);
      Assert.True(multiPoints == 8);
      Assert.True(multiLineStrings == 8);
      Assert.True(multiPolygons == 8);
      Assert.True(geometryCollections == 8);

      // Point
      var gmlPointRegex = new GmlPointRegex();
      for (int i = 0; i < 4; i++)
      {
        Assert.True(gmlPointRegex.IsMatch(gmlLines[i]));
      }

      for (int i = 4; i < 8; i++)
      {
        Assert.False(gmlPointRegex.IsMatch(gmlLines[i]));
      }

      // LineString
      var gmlLineStringRegex = new GmlLineStringRegex();
      for (int i = 8; i < 12; i++)
      {
        Assert.True(gmlLineStringRegex.IsMatch(gmlLines[i]));
      }

      for (int i = 12; i < 16; i++)
      {
        Assert.False(gmlLineStringRegex.IsMatch(gmlLines[i]));
      }

      // Polygon
      var gmlPolygonRegex = new GmlPolygonRegex();
      for (int i = 16; i < 20; i++)
      {
        Assert.True(gmlPolygonRegex.IsMatch(gmlLines[i]));
      }

      for (int i = 20; i < 24; i++)
      {
        Assert.False(gmlPolygonRegex.IsMatch(gmlLines[i]));
      }

      // MultiPoint
      var gmlMultiPointRegex = new GmlMultiPointRegex();
      for (int i = 24; i < 28; i++)
      {
        Assert.True(gmlMultiPointRegex.IsMatch(gmlLines[i]));
      }

      for (int i = 28; i < 32; i++)
      {
        Assert.False(gmlMultiPointRegex.IsMatch(gmlLines[i]));
      }

      // MultiLineString
      var gmlMultiLineStringRegex = new GmlMultiLineStringRegex();
      for (int i = 32; i < 36; i++)
      {
        Assert.True(gmlMultiLineStringRegex.IsMatch(gmlLines[i]));
      }

      for (int i = 36; i < 40; i++)
      {
        Assert.False(gmlMultiLineStringRegex.IsMatch(gmlLines[i]));
      }

      // MultiPolygon
      var gmlMultiPolygonRegex = new GmlMultiPolygonRegex();
      for (int i = 40; i < 44; i++)
      {
        Assert.True(gmlMultiPolygonRegex.IsMatch(gmlLines[i]));
      }

      for (int i = 44; i < 48; i++)
      {
        Assert.False(gmlMultiPolygonRegex.IsMatch(gmlLines[i]));
      }

      // GeometryCollection
      var gmlGeometryCollectionRegex = new GmlGeometryCollectionRegex();
      for (int i = 48; i < 52; i++)
      {
        Assert.True(gmlGeometryCollectionRegex.IsMatch(gmlLines[i]));
      }

      for (int i = 52; i < 56; i++)
      {
        Assert.False(gmlGeometryCollectionRegex.IsMatch(gmlLines[i]));
      }
    }

    /// <summary>
    /// Performs unit tests on the regex classes that handle text in KML format.
    /// </summary>
    [Fact]
    public void TestKmlRegexes()
    {
      // Read KML data file
      var kmlData = Utilities.GetScriptFromResource("MySql.Utility.RegularExpressions.Tests.Properties.KmlData.txt");
      Assert.True(!string.IsNullOrEmpty(kmlData));
      var kmlLines = kmlData.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
      Assert.True(kmlLines.Length == 32);

      // Allowed Geoms
      var allowedGeomsRegex = new KmlAllowedGeomClassesRegex();
      var points = 0;
      var lineStrings = 0;
      var polygons = 0;
      var multiPoints = 0;
      var multiLineStrings = 0;
      var multiPolygons = 0;
      var geometryCollections = 0;
      foreach (var kmlLine in kmlLines)
      {
        var match = allowedGeomsRegex.Match(kmlLine);
        if (!match.Success)
        {
          continue;
        }

        switch (match.Groups["Class"].Value.ToLowerInvariant())
        {
          case "point":
            points++;
            break;

          case "linestring":
            lineStrings++;
            break;

          case "polygon":
            polygons++;
            break;

          case "multigeometry":
            geometryCollections++;
            break;
        }
      }

      Assert.True(points == 8);
      Assert.True(lineStrings == 8);
      Assert.True(polygons == 8);
      Assert.True(multiPoints == 0);
      Assert.True(multiLineStrings == 0);
      Assert.True(multiPolygons == 0);
      Assert.True(geometryCollections == 8);

      // Point
      var kmlPointRegex = new KmlPointRegex();
      for (int i = 0; i < 4; i++)
      {
        Assert.True(kmlPointRegex.IsMatch(kmlLines[i]));
      }

      for (int i = 4; i < 8; i++)
      {
        Assert.False(kmlPointRegex.IsMatch(kmlLines[i]));
      }

      // LineString
      var kmlLineStringRegex = new KmlLineStringRegex();
      for (int i = 8; i < 12; i++)
      {
        Assert.True(kmlLineStringRegex.IsMatch(kmlLines[i]));
      }

      for (int i = 12; i < 16; i++)
      {
        Assert.False(kmlLineStringRegex.IsMatch(kmlLines[i]));
      }

      // Polygon
      var kmlPolygonRegex = new KmlPolygonRegex();
      for (int i = 16; i < 20; i++)
      {
        Assert.True(kmlPolygonRegex.IsMatch(kmlLines[i]));
      }

      for (int i = 20; i < 24; i++)
      {
        Assert.False(kmlPolygonRegex.IsMatch(kmlLines[i]));
      }

      // MultiGeometry
      var kmlGeometryCollectionRegex = new KmlGeometryCollectionRegex();
      for (int i = 24; i < 29; i++)
      {
        Assert.True(kmlGeometryCollectionRegex.IsMatch(kmlLines[i]));
      }

      for (int i = 29; i < 32; i++)
      {
        Assert.False(kmlGeometryCollectionRegex.IsMatch(kmlLines[i]));
      }
    }

    /// <summary>
    /// Performs unit tests on the regex classes that handle text in WKT format.
    /// </summary>
    [Fact]
    public void TestWktRegexes()
    {
      // Read WKT data file
      var wktData = Utilities.GetScriptFromResource("MySql.Utility.RegularExpressions.Tests.Properties.WktData.txt");
      Assert.True(!string.IsNullOrEmpty(wktData));
      var wktLines = wktData.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
      Assert.True(wktLines.Length == 56);

      // Allowed Geoms
      var allowedGeomsRegex = new WktAllowedGeomClassesRegex();
      var points = 0;
      var lineStrings = 0;
      var polygons = 0;
      var multiPoints = 0;
      var multiLineStrings = 0;
      var multiPolygons = 0;
      var geometryCollections = 0;
      foreach (var wktLine in wktLines)
      {
        var match = allowedGeomsRegex.Match(wktLine);
        if (!match.Success)
        {
          continue;
        }

        switch (match.Groups["Class"].Value.ToLowerInvariant())
        {
          case "point":
            points++;
            break;

          case "linestring":
            lineStrings++;
            break;

          case "polygon":
            polygons++;
            break;

          case "multipoint":
            multiPoints++;
            break;

          case "multilinestring":
            multiLineStrings++;
            break;

          case "multipolygon":
            multiPolygons++;
            break;

          case "geometrycollection":
            geometryCollections++;
            break;
        }
      }

      Assert.True(points == 8);
      Assert.True(lineStrings == 8);
      Assert.True(polygons == 8);
      Assert.True(multiPoints == 8);
      Assert.True(multiLineStrings == 8);
      Assert.True(multiPolygons == 8);
      Assert.True(geometryCollections == 8);

      // Point
      var wktPointRegex = new WktPointRegex();
      for (int i = 0; i < 5; i++)
      {
        Assert.True(wktPointRegex.IsMatch(wktLines[i]));
      }

      for (int i = 5; i < 8; i++)
      {
        Assert.False(wktPointRegex.IsMatch(wktLines[i]));
      }

      // LineString
      var wktLineStringRegex = new WktLineStringRegex();
      for (int i = 8; i < 13; i++)
      {
        Assert.True(wktLineStringRegex.IsMatch(wktLines[i]));
      }

      for (int i = 13; i < 16; i++)
      {
        Assert.False(wktLineStringRegex.IsMatch(wktLines[i]));
      }

      // Polygon
      var wktPolygonRegex = new WktPolygonRegex();
      for (int i = 16; i < 21; i++)
      {
        Assert.True(wktPolygonRegex.IsMatch(wktLines[i]));
      }

      for (int i = 21; i < 24; i++)
      {
        Assert.False(wktPolygonRegex.IsMatch(wktLines[i]));
      }

      // MultiPoint
      var wktMultiPointRegex = new WktMultiPointRegex();
      for (int i = 24; i < 29; i++)
      {
        Assert.True(wktMultiPointRegex.IsMatch(wktLines[i]));
      }

      for (int i = 29; i < 32; i++)
      {
        Assert.False(wktMultiPointRegex.IsMatch(wktLines[i]));
      }

      // MultiLineString
      var wktMultiLineStringRegex = new WktMultiLineStringRegex();
      for (int i = 32; i < 37; i++)
      {
        Assert.True(wktMultiLineStringRegex.IsMatch(wktLines[i]));
      }

      for (int i = 37; i < 40; i++)
      {
        Assert.False(wktMultiLineStringRegex.IsMatch(wktLines[i]));
      }

      // MultiPolygon
      var wktMultiPolygonRegex = new WktMultiPolygonRegex();
      for (int i = 40; i < 45; i++)
      {
        Assert.True(wktMultiPolygonRegex.IsMatch(wktLines[i]));
      }

      for (int i = 45; i < 48; i++)
      {
        Assert.False(wktMultiPolygonRegex.IsMatch(wktLines[i]));
      }

      // GeometryCollection
      var wktGeometryCollectionRegex = new WktGeometryCollectionRegex();
      for (int i = 48; i < 53; i++)
      {
        Assert.True(wktGeometryCollectionRegex.IsMatch(wktLines[i]));
      }

      for (int i = 53; i < 56; i++)
      {
        Assert.False(wktGeometryCollectionRegex.IsMatch(wktLines[i]));
      }
    }
  }
}
