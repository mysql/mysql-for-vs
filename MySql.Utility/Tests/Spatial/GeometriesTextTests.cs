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
using System.Reflection;
using MySql.Utility.Classes;
using MySql.Utility.Classes.Spatial;
using MySql.Utility.Enums;
using MySql.Utility.RegularExpressions.Tests;
using Xunit;

namespace MySql.Utility.Tests.Spatial
{
  /// <summary>
  /// Tests for the geometry classes text representations.
  /// </summary>
  public class GeometriesTextTests
  {
    #region Fields

    /// <summary>
    /// Holds lines of data in GeoJSON format.
    /// </summary>
    private string[] _geoJsonDataLines;

    /// <summary>
    /// Holds lines of data in GML format.
    /// </summary>
    private string[] _gmlDataLines;

    /// <summary>
    /// Holds lines of data in KML format.
    /// </summary>
    private string[] _kmlDataLines;

    /// <summary>
    /// Holds lines of data in WKT format.
    /// </summary>
    private string[] _wktDataLines;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of the <see cref="GeometriesTextTests"/> class.
    /// </summary>
    public GeometriesTextTests()
    {
      _geoJsonDataLines = null;
      _gmlDataLines = null;
      _kmlDataLines = null;
      _wktDataLines = null;
    }

    /// <summary>
    /// Performs unit tests on the <see cref="GeometryCollection"/> class using data in all supported formats.
    /// </summary>
    [Fact]
    public void TestGeometryCollection()
    {
      // Read data files
      ReadData();

      // Test GeoJSON and GML
      Point point;
      LineString lineString;
      GeometryCollection multiGeometry;
      string[] dataLines;
      foreach (GeometryAsTextFormatType format in Enum.GetValues(typeof(GeometryAsTextFormatType)))
      {
        switch (format)
        {
          case GeometryAsTextFormatType.GeoJSON:
            dataLines = _geoJsonDataLines;
            break;

          case GeometryAsTextFormatType.GML:
            dataLines = _gmlDataLines;
            break;

          default:
            continue;
        }

        Assert.True(GeometryCollection.IsValid(dataLines[48], format));
        multiGeometry = GeometryCollection.Parse(dataLines[48], format);
        Assert.True(multiGeometry != null);
        Assert.True(multiGeometry.GeometriesCount == 3);
        point = multiGeometry.Geometries[0] as Point;
        Assert.True(point != null);
        Assert.True(point.Coordinate == new Coordinate(10.0, 10.0));
        point = multiGeometry.Geometries[1] as Point;
        Assert.True(point != null);
        Assert.True(point.Coordinate == new Coordinate(30.0, 30.0));
        lineString = multiGeometry.Geometries[2] as LineString;
        Assert.True(lineString != null);
        Assert.True(lineString.CoordinatesCount == 5);
        Assert.True(lineString.Coordinates[0] == new Coordinate(15.0, 15.0));
        Assert.True(lineString.Coordinates[1] == new Coordinate(15.0, 20.0));
        Assert.True(lineString.Coordinates[2] == new Coordinate(20.0, 20.0));
        Assert.True(lineString.Coordinates[3] == new Coordinate(20.0, 15.0));
        Assert.True(lineString.Coordinates[4] == new Coordinate(15.0, 15.0));
        ValidateGeometryCollectionToString(multiGeometry, format);
        Assert.True(GeometryCollection.IsValid(dataLines[49], format));
        multiGeometry = GeometryCollection.Parse(dataLines[49], format);
        Assert.True(multiGeometry != null);
        Assert.True(multiGeometry.GeometriesCount == 2);
        point = multiGeometry.Geometries[0] as Point;
        Assert.True(point != null);
        Assert.True(point.Coordinate == new Coordinate(10.0, 10.0));
        point = multiGeometry.Geometries[1] as Point;
        Assert.True(point != null);
        Assert.True(point.IsEmpty);
        ValidateGeometryCollectionToString(multiGeometry, format);
        Assert.True(GeometryCollection.IsValid(dataLines[50], format));
        multiGeometry = GeometryCollection.Parse(dataLines[50], format);
        Assert.True(multiGeometry != null);
        Assert.True(multiGeometry.IsEmpty);
        ValidateGeometryCollectionToString(multiGeometry, format);
        Assert.True(GeometryCollection.IsValid(dataLines[51], format));
        multiGeometry = GeometryCollection.Parse(dataLines[51], format);
        Assert.True(multiGeometry != null);
        Assert.True(multiGeometry.GeometriesCount == 1);
        lineString = multiGeometry.Geometries[0] as LineString;
        Assert.True(lineString != null);
        Assert.True(lineString.CoordinatesCount == 5);
        Assert.True(lineString.Coordinates[0] == new Coordinate(15.0, 15.0));
        Assert.True(lineString.Coordinates[1] == new Coordinate(15.0, 20.0));
        Assert.True(lineString.Coordinates[2] == new Coordinate(20.0, 20.0));
        Assert.True(lineString.Coordinates[3] == new Coordinate(20.0, 15.0));
        Assert.True(lineString.Coordinates[4] == new Coordinate(15.0, 15.0));
        ValidateGeometryCollectionToString(multiGeometry, format);
        for (int i = 52; i < 56; i++)
        {
          Assert.False(GeometryCollection.IsValid(dataLines[i], format));
          multiGeometry = GeometryCollection.Parse(dataLines[i], format);
          Assert.True(multiGeometry == null);
        }
      }

      // Test KML
      dataLines = _kmlDataLines;
      Assert.True(GeometryCollection.IsValid(dataLines[24], GeometryAsTextFormatType.KML));
      multiGeometry = GeometryCollection.Parse(dataLines[24], GeometryAsTextFormatType.KML);
      Assert.True(multiGeometry != null);
      Assert.True(multiGeometry.GeometriesCount == 3);
      point = multiGeometry.Geometries[0] as Point;
      Assert.True(point != null);
      Assert.True(point.Coordinate == new Coordinate(10.0, 10.0));
      point = multiGeometry.Geometries[1] as Point;
      Assert.True(point != null);
      Assert.True(point.Coordinate == new Coordinate(30.0, 30.0));
      lineString = multiGeometry.Geometries[2] as LineString;
      Assert.True(lineString != null);
      Assert.True(lineString.CoordinatesCount == 5);
      Assert.True(lineString.Coordinates[0] == new Coordinate(15.0, 15.0));
      Assert.True(lineString.Coordinates[1] == new Coordinate(15.0, 20.0));
      Assert.True(lineString.Coordinates[2] == new Coordinate(20.0, 20.0));
      Assert.True(lineString.Coordinates[3] == new Coordinate(20.0, 15.0));
      Assert.True(lineString.Coordinates[4] == new Coordinate(15.0, 15.0));
      ValidateGeometryCollectionToString(multiGeometry, GeometryAsTextFormatType.KML);
      Assert.True(GeometryCollection.IsValid(dataLines[25], GeometryAsTextFormatType.KML));
      var multiPoint = GeometryCollection.Parse(dataLines[25], GeometryAsTextFormatType.KML) as MultiPoint;
      Assert.True(multiPoint != null);
      Assert.True(multiPoint.GeometriesCount == 3);
      point = multiPoint.Geometries[0] as Point;
      Assert.True(point != null);
      Assert.True(point.Coordinate == new Coordinate(1.0, 1.0));
      point = multiPoint.Geometries[1] as Point;
      Assert.True(point != null);
      Assert.True(point.Coordinate == new Coordinate(2.0, 2.0));
      point = multiPoint.Geometries[2] as Point;
      Assert.True(point != null);
      Assert.True(point.IsEmpty);
      ValidateGeometryCollectionToString(multiGeometry, GeometryAsTextFormatType.KML);
      Assert.True(GeometryCollection.IsValid(dataLines[26], GeometryAsTextFormatType.KML));
      multiGeometry = GeometryCollection.Parse(dataLines[26], GeometryAsTextFormatType.KML);
      Assert.True(multiGeometry != null);
      Assert.True(multiGeometry.IsEmpty);
      ValidateGeometryCollectionToString(multiGeometry, GeometryAsTextFormatType.KML);
      Assert.True(GeometryCollection.IsValid(dataLines[27], GeometryAsTextFormatType.KML));
      var multiLineString = GeometryCollection.Parse(dataLines[27], GeometryAsTextFormatType.KML) as MultiLineString;
      Assert.True(multiLineString != null);
      Assert.True(multiLineString.GeometriesCount == 3);
      lineString = multiLineString.Geometries[0] as LineString;
      Assert.True(lineString != null);
      Assert.True(lineString.CoordinatesCount == 2);
      Assert.True(lineString.Coordinates[0] == new Coordinate(10.0, 10.0));
      Assert.True(lineString.Coordinates[1] == new Coordinate(20.0, 20.0));
      lineString = multiLineString.Geometries[1] as LineString;
      Assert.True(lineString != null);
      Assert.True(lineString.CoordinatesCount == 2);
      Assert.True(lineString.Coordinates[0] == new Coordinate(15.0, 15.0));
      Assert.True(lineString.Coordinates[1] == new Coordinate(30.0, 15.0));
      lineString = multiLineString.Geometries[2] as LineString;
      Assert.True(lineString != null);
      Assert.True(lineString.CoordinatesCount == 2);
      Assert.True(lineString.Coordinates[0] == new Coordinate(5.0, 5.0));
      Assert.True(lineString.Coordinates[1] == new Coordinate(10.0, 9.0));
      ValidateGeometryCollectionToString(multiGeometry, GeometryAsTextFormatType.KML);
      Assert.True(GeometryCollection.IsValid(dataLines[28], GeometryAsTextFormatType.KML));
      var multiPolygon = GeometryCollection.Parse(dataLines[28], GeometryAsTextFormatType.KML) as MultiPolygon;
      Assert.True(multiPolygon != null);
      Assert.True(multiPolygon.GeometriesCount == 2);
      var polygon = multiPolygon.Geometries[0] as Polygon;
      Assert.True(polygon != null);
      Assert.True(polygon.HolesCount == 2);
      Assert.True(polygon.Shell.IsRing);
      Assert.True(polygon.Shell.CoordinatesCount == 5);
      Assert.True(polygon.Shell.Coordinates[0] == new Coordinate(0.0, 0.0));
      Assert.True(polygon.Shell.Coordinates[1] == new Coordinate(10.0, 0.0));
      Assert.True(polygon.Shell.Coordinates[2] == new Coordinate(10.0, 10.0));
      Assert.True(polygon.Shell.Coordinates[3] == new Coordinate(0.0, 10.0));
      Assert.True(polygon.Shell.Coordinates[4] == new Coordinate(0.0, 0.0));
      Assert.True(polygon.Holes[0].IsRing);
      Assert.True(polygon.Holes[0].CoordinatesCount == 5);
      Assert.True(polygon.Holes[0].Coordinates[0] == new Coordinate(5.0, 5.0));
      Assert.True(polygon.Holes[0].Coordinates[1] == new Coordinate(7.0, 5.0));
      Assert.True(polygon.Holes[0].Coordinates[2] == new Coordinate(7.0, 7.0));
      Assert.True(polygon.Holes[0].Coordinates[3] == new Coordinate(5.0, 7.0));
      Assert.True(polygon.Holes[0].Coordinates[4] == new Coordinate(5.0, 5.0));
      Assert.True(polygon.Holes[1].IsRing);
      Assert.True(polygon.Holes[1].CoordinatesCount == 5);
      Assert.True(polygon.Holes[1].Coordinates[0] == new Coordinate(5.0, 1.0));
      Assert.True(polygon.Holes[1].Coordinates[1] == new Coordinate(7.0, 1.0));
      Assert.True(polygon.Holes[1].Coordinates[2] == new Coordinate(7.0, 3.0));
      Assert.True(polygon.Holes[1].Coordinates[3] == new Coordinate(5.0, 3.0));
      Assert.True(polygon.Holes[1].Coordinates[4] == new Coordinate(5.0, 1.0));
      polygon = multiPolygon.Geometries[1] as Polygon;
      Assert.True(polygon != null);
      Assert.True(polygon.HolesCount == 2);
      Assert.True(polygon.Shell.IsRing);
      Assert.True(polygon.Shell.CoordinatesCount == 5);
      Assert.True(polygon.Shell.Coordinates[0] == new Coordinate(2.0, 2.0));
      Assert.True(polygon.Shell.Coordinates[1] == new Coordinate(12.0, 2.0));
      Assert.True(polygon.Shell.Coordinates[2] == new Coordinate(12.0, 12.0));
      Assert.True(polygon.Shell.Coordinates[3] == new Coordinate(2.0, 12.0));
      Assert.True(polygon.Shell.Coordinates[4] == new Coordinate(2.0, 2.0));
      Assert.True(polygon.Holes[0].IsRing);
      Assert.True(polygon.Holes[0].CoordinatesCount == 5);
      Assert.True(polygon.Holes[0].Coordinates[0] == new Coordinate(7.0, 7.0));
      Assert.True(polygon.Holes[0].Coordinates[1] == new Coordinate(9.0, 7.0));
      Assert.True(polygon.Holes[0].Coordinates[2] == new Coordinate(9.0, 9.0));
      Assert.True(polygon.Holes[0].Coordinates[3] == new Coordinate(7.0, 9.0));
      Assert.True(polygon.Holes[0].Coordinates[4] == new Coordinate(7.0, 7.0));
      Assert.True(polygon.Holes[1].IsRing);
      Assert.True(polygon.Holes[1].CoordinatesCount == 5);
      Assert.True(polygon.Holes[1].Coordinates[0] == new Coordinate(7.0, 3.0));
      Assert.True(polygon.Holes[1].Coordinates[1] == new Coordinate(9.0, 3.0));
      Assert.True(polygon.Holes[1].Coordinates[2] == new Coordinate(9.0, 5.0));
      Assert.True(polygon.Holes[1].Coordinates[3] == new Coordinate(7.0, 5.0));
      Assert.True(polygon.Holes[1].Coordinates[4] == new Coordinate(7.0, 3.0));
      ValidateGeometryCollectionToString(multiGeometry, GeometryAsTextFormatType.KML);
      for (int i = 29; i < 32; i++)
      {
        Assert.False(GeometryCollection.IsValid(dataLines[i], GeometryAsTextFormatType.KML));
        multiGeometry = GeometryCollection.Parse(dataLines[i], GeometryAsTextFormatType.KML);
        Assert.True(multiGeometry == null);
      }

      // Test WKT
      dataLines = _wktDataLines;
      Assert.True(GeometryCollection.IsValid(dataLines[48], GeometryAsTextFormatType.WKT));
      multiGeometry = GeometryCollection.Parse(dataLines[48], GeometryAsTextFormatType.WKT);
      Assert.True(multiGeometry != null);
      Assert.True(multiGeometry.GeometriesCount == 3);
      point = multiGeometry.Geometries[0] as Point;
      Assert.True(point != null);
      Assert.True(point.Coordinate == new Coordinate(10.0, 10.0));
      point = multiGeometry.Geometries[1] as Point;
      Assert.True(point != null);
      Assert.True(point.Coordinate == new Coordinate(30.0, 30.0));
      lineString = multiGeometry.Geometries[2] as LineString;
      Assert.True(lineString != null);
      Assert.True(lineString.CoordinatesCount == 5);
      Assert.True(lineString.Coordinates[0] == new Coordinate(15.0, 15.0));
      Assert.True(lineString.Coordinates[1] == new Coordinate(15.0, 20.0));
      Assert.True(lineString.Coordinates[2] == new Coordinate(20.0, 20.0));
      Assert.True(lineString.Coordinates[3] == new Coordinate(20.0, 15.0));
      Assert.True(lineString.Coordinates[4] == new Coordinate(15.0, 15.0));
      ValidateGeometryCollectionToString(multiGeometry, GeometryAsTextFormatType.WKT);
      Assert.True(GeometryCollection.IsValid(dataLines[49], GeometryAsTextFormatType.WKT));
      multiGeometry = GeometryCollection.Parse(dataLines[49], GeometryAsTextFormatType.WKT);
      Assert.True(multiGeometry != null);
      Assert.True(multiGeometry.SRID == 70);
      Assert.True(multiGeometry.GeometriesCount == 2);
      point = multiGeometry.Geometries[0] as Point;
      Assert.True(point != null);
      Assert.True(point.SRID == 70);
      Assert.True(point.Coordinate == new Coordinate(10.0, 10.0));
      lineString = multiGeometry.Geometries[1] as LineString;
      Assert.True(lineString != null);
      Assert.True(lineString.SRID == 70);
      Assert.True(lineString.IsEmpty);
      ValidateGeometryCollectionToString(multiGeometry, GeometryAsTextFormatType.WKT);
      for (int i = 50; i < 52; i++)
      {
        Assert.True(GeometryCollection.IsValid(dataLines[i], GeometryAsTextFormatType.WKT));
        multiGeometry = GeometryCollection.Parse(dataLines[i], GeometryAsTextFormatType.WKT);
        Assert.True(multiGeometry != null);
        Assert.True(multiGeometry.IsEmpty);
        ValidateGeometryCollectionToString(multiGeometry, GeometryAsTextFormatType.WKT);
      }

      Assert.True(GeometryCollection.IsValid(dataLines[52], GeometryAsTextFormatType.WKT));
      multiGeometry = GeometryCollection.Parse(dataLines[52], GeometryAsTextFormatType.WKT);
      Assert.True(multiGeometry != null);
      Assert.True(multiGeometry.GeometriesCount == 2);
      point = multiGeometry.Geometries[0] as Point;
      Assert.True(point != null);
      Assert.True(point.Coordinate == new Coordinate(10.0, 10.0));
      point = multiGeometry.Geometries[1] as Point;
      Assert.True(point != null);
      Assert.True(point.Coordinate == new Coordinate(30.0, 30.0));
      ValidateGeometryCollectionToString(multiGeometry, GeometryAsTextFormatType.WKT);
      for (int i = 53; i < 56; i++)
      {
        Assert.False(GeometryCollection.IsValid(dataLines[i], GeometryAsTextFormatType.WKT));
        multiGeometry = GeometryCollection.Parse(dataLines[i], GeometryAsTextFormatType.WKT);
        Assert.True(multiGeometry == null);
      }
    }

    /// <summary>
    /// Performs unit tests on the <see cref="LineString"/> class using data in all supported formats.
    /// </summary>
    [Fact]
    public void TestLineString()
    {
      // Read data files
      ReadData();

      // Test GeoJSON, GML and KML
      LineString lineString;
      string[] dataLines;
      foreach (GeometryAsTextFormatType format in Enum.GetValues(typeof(GeometryAsTextFormatType)))
      {
        switch (format)
        {
          case GeometryAsTextFormatType.GeoJSON:
            dataLines = _geoJsonDataLines;
            break;

          case GeometryAsTextFormatType.GML:
            dataLines = _gmlDataLines;
            break;

          case GeometryAsTextFormatType.KML:
            dataLines = _kmlDataLines;
            break;

          default:
            continue;
        }

        Assert.True(LineString.IsValid(dataLines[8], format));
        lineString = LineString.Parse(dataLines[8], format);
        Assert.True(lineString != null);
        Assert.True(lineString.Coordinates.Count == 7);
        Assert.True(lineString.Coordinates[0] == new Coordinate(-9.0480327, 38.5825645));
        Assert.True(lineString.Coordinates[1] == new Coordinate(-9.0490129, 38.5830875));
        Assert.True(lineString.Coordinates[2] == new Coordinate(-9.0503459, 38.5837924));
        Assert.True(lineString.Coordinates[3] == new Coordinate(-9.0516338, 38.5845331));
        Assert.True(lineString.Coordinates[4] == new Coordinate(-9.0526929, 38.5851422));
        Assert.True(lineString.Coordinates[5] == new Coordinate(-9.0549149, 38.586514));
        Assert.True(lineString.Coordinates[6] == new Coordinate(-9.0565145, 38.5875012));
        ValidateLineStringToString(lineString, format);
        Assert.True(LineString.IsValid(dataLines[9], format));
        lineString = LineString.Parse(dataLines[9], format);
        Assert.True(lineString != null);
        Assert.True(lineString.Coordinates.Count == 4);
        Assert.True(lineString.Coordinates[0] == new Coordinate(10, 10));
        Assert.True(lineString.Coordinates[1] == new Coordinate(20, 10));
        Assert.True(lineString.Coordinates[2] == new Coordinate(20, 20));
        Assert.True(lineString.Coordinates[3] == new Coordinate(10, 20));
        ValidateLineStringToString(lineString, format);
        Assert.True(LineString.IsValid(dataLines[10], format));
        lineString = LineString.Parse(dataLines[10], format);
        Assert.True(lineString != null);
        Assert.True(lineString.IsEmpty);
        ValidateLineStringToString(lineString, format);
        Assert.True(LineString.IsValid(dataLines[11], format));
        lineString = LineString.Parse(dataLines[11], format);
        Assert.True(lineString != null);
        Assert.True(lineString.Coordinates.Count == 2);
        Assert.True(lineString.Coordinates[0] == new Coordinate(-9.0762654, 38.8474268));
        Assert.True(lineString.Coordinates[1] == new Coordinate(-9.0768976, 38.8464239));
        ValidateLineStringToString(lineString, format);
        for (int i = 12; i < 16; i++)
        {
          Assert.False(LineString.IsValid(dataLines[i], format));
          lineString = LineString.Parse(dataLines[i], format);
          Assert.True(lineString == null);
        }
      }

      // Test WKT
      dataLines = _wktDataLines;
      Assert.True(LineString.IsValid(dataLines[8], GeometryAsTextFormatType.WKT));
      lineString = LineString.Parse(dataLines[8], GeometryAsTextFormatType.WKT);
      Assert.True(lineString != null);
      Assert.True(lineString.Coordinates.Count == 7);
      Assert.True(lineString.Coordinates[0] == new Coordinate(-9.0480327, 38.5825645));
      Assert.True(lineString.Coordinates[1] == new Coordinate(-9.0490129, 38.5830875));
      Assert.True(lineString.Coordinates[2] == new Coordinate(-9.0503459, 38.5837924));
      Assert.True(lineString.Coordinates[3] == new Coordinate(-9.0516338, 38.5845331));
      Assert.True(lineString.Coordinates[4] == new Coordinate(-9.0526929, 38.5851422));
      Assert.True(lineString.Coordinates[5] == new Coordinate(-9.0549149, 38.586514));
      Assert.True(lineString.Coordinates[6] == new Coordinate(-9.0565145, 38.5875012));
      ValidateLineStringToString(lineString, GeometryAsTextFormatType.WKT);
      Assert.True(LineString.IsValid(dataLines[9], GeometryAsTextFormatType.WKT));
      lineString = LineString.Parse(dataLines[9], GeometryAsTextFormatType.WKT);
      Assert.True(lineString != null);
      Assert.True(lineString.SRID == 20);
      Assert.True(lineString.Coordinates.Count == 4);
      Assert.True(lineString.Coordinates[0] == new Coordinate(10, 10));
      Assert.True(lineString.Coordinates[1] == new Coordinate(20, 10));
      Assert.True(lineString.Coordinates[2] == new Coordinate(20, 20));
      Assert.True(lineString.Coordinates[3] == new Coordinate(10, 20));
      ValidateLineStringToString(lineString, GeometryAsTextFormatType.WKT);
      for (int i = 10; i < 12; i++)
      {
        Assert.True(LineString.IsValid(dataLines[i], GeometryAsTextFormatType.WKT));
        lineString = LineString.Parse(dataLines[i], GeometryAsTextFormatType.WKT);
        Assert.True(lineString != null);
        Assert.True(lineString.IsEmpty);
        ValidateLineStringToString(lineString, GeometryAsTextFormatType.WKT);
      }

      Assert.True(LineString.IsValid(dataLines[12], GeometryAsTextFormatType.WKT));
      lineString = LineString.Parse(dataLines[12], GeometryAsTextFormatType.WKT);
      Assert.True(lineString != null);
      Assert.True(lineString.Coordinates.Count == 4);
      Assert.True(lineString.Coordinates[0] == new Coordinate(10, 10));
      Assert.True(lineString.Coordinates[1] == new Coordinate(20, 10));
      Assert.True(lineString.Coordinates[2] == new Coordinate(20, 20));
      Assert.True(lineString.Coordinates[3] == new Coordinate(10, 20));
      ValidateLineStringToString(lineString, GeometryAsTextFormatType.WKT);
      for (int i = 13; i < 16; i++)
      {
        Assert.False(LineString.IsValid(dataLines[i], GeometryAsTextFormatType.WKT));
        lineString = LineString.Parse(dataLines[i], GeometryAsTextFormatType.WKT);
        Assert.True(lineString == null);
      }
    }

    /// <summary>
    /// Performs unit tests on the <see cref="MultiLineString"/> class using data in all supported formats.
    /// </summary>
    [Fact]
    public void TestMultiLineString()
    {
      // Read data files
      ReadData();

      // Test GeoJSON and GML
      LineString lineString;
      MultiLineString multiLineString;
      string[] dataLines;
      foreach (GeometryAsTextFormatType format in Enum.GetValues(typeof(GeometryAsTextFormatType)))
      {
        switch (format)
        {
          case GeometryAsTextFormatType.GeoJSON:
            dataLines = _geoJsonDataLines;
            break;

          case GeometryAsTextFormatType.GML:
            dataLines = _gmlDataLines;
            break;

          default:
            continue;
        }

        Assert.True(MultiLineString.IsValid(dataLines[32], format));
        multiLineString = MultiLineString.Parse(dataLines[32], format);
        Assert.True(multiLineString != null);
        Assert.True(multiLineString.GeometriesCount == 3);
        lineString = multiLineString.Geometries[0] as LineString;
        Assert.True(lineString != null);
        Assert.True(lineString.CoordinatesCount == 2);
        Assert.True(lineString.Coordinates[0] == new Coordinate(10.0, 10.0));
        Assert.True(lineString.Coordinates[1] == new Coordinate(20.0, 20.0));
        lineString = multiLineString.Geometries[1] as LineString;
        Assert.True(lineString != null);
        Assert.True(lineString.CoordinatesCount == 2);
        Assert.True(lineString.Coordinates[0] == new Coordinate(15.0, 15.0));
        Assert.True(lineString.Coordinates[1] == new Coordinate(30.0, 15.0));
        lineString = multiLineString.Geometries[2] as LineString;
        Assert.True(lineString != null);
        Assert.True(lineString.CoordinatesCount == 2);
        Assert.True(lineString.Coordinates[0] == new Coordinate(5.0, 5.0));
        Assert.True(lineString.Coordinates[1] == new Coordinate(10.0, 9.0));
        ValidateMultiLineStringToString(multiLineString, format);
        Assert.True(MultiLineString.IsValid(dataLines[33], format));
        multiLineString = MultiLineString.Parse(dataLines[33], format);
        Assert.True(multiLineString != null);
        Assert.True(multiLineString.GeometriesCount == 4);
        lineString = multiLineString.Geometries[0] as LineString;
        Assert.True(lineString != null);
        Assert.True(lineString.IsEmpty);
        lineString = multiLineString.Geometries[1] as LineString;
        Assert.True(lineString != null);
        Assert.True(lineString.CoordinatesCount == 3);
        Assert.True(lineString.Coordinates[0] == new Coordinate(10.0, 10.0));
        Assert.True(lineString.Coordinates[1] == new Coordinate(20.0, 20.0));
        Assert.True(lineString.Coordinates[2] == new Coordinate(30.0, 30.0));
        lineString = multiLineString.Geometries[2] as LineString;
        Assert.True(lineString != null);
        Assert.True(lineString.CoordinatesCount == 3);
        Assert.True(lineString.Coordinates[0] == new Coordinate(15.0, 15.0));
        Assert.True(lineString.Coordinates[1] == new Coordinate(30.0, 15.0));
        Assert.True(lineString.Coordinates[2] == new Coordinate(45.0, 15.0));
        lineString = multiLineString.Geometries[3] as LineString;
        Assert.True(lineString != null);
        Assert.True(lineString.CoordinatesCount == 3);
        Assert.True(lineString.Coordinates[0] == new Coordinate(5.0, 5.0));
        Assert.True(lineString.Coordinates[1] == new Coordinate(10.0, 9.0));
        Assert.True(lineString.Coordinates[2] == new Coordinate(8.0, 12.0));
        ValidateMultiLineStringToString(multiLineString, format);
        Assert.True(MultiLineString.IsValid(dataLines[34], format));
        multiLineString = MultiLineString.Parse(dataLines[34], format);
        Assert.True(multiLineString != null);
        Assert.True(multiLineString.IsEmpty);
        ValidateMultiLineStringToString(multiLineString, format);
        Assert.True(MultiLineString.IsValid(dataLines[35], format));
        multiLineString = MultiLineString.Parse(dataLines[35], format);
        Assert.True(multiLineString != null);
        Assert.True(multiLineString.GeometriesCount == 3);
        lineString = multiLineString.Geometries[0] as LineString;
        Assert.True(lineString != null);
        Assert.True(lineString.CoordinatesCount == 2);
        Assert.True(lineString.Coordinates[0] == new Coordinate(10.0, 10.0));
        Assert.True(lineString.Coordinates[1] == new Coordinate(20.0, 20.0));
        lineString = multiLineString.Geometries[1] as LineString;
        Assert.True(lineString != null);
        Assert.True(lineString.CoordinatesCount == 2);
        Assert.True(lineString.Coordinates[0] == new Coordinate(15.0, 15.0));
        Assert.True(lineString.Coordinates[1] == new Coordinate(30.0, 15.0));
        lineString = multiLineString.Geometries[2] as LineString;
        Assert.True(lineString != null);
        Assert.True(lineString.IsEmpty);
        ValidateMultiLineStringToString(multiLineString, format);
        for (int i = 36; i < 40; i++)
        {
          Assert.False(MultiLineString.IsValid(dataLines[i], format));
          multiLineString = MultiLineString.Parse(dataLines[i], format);
          Assert.True(multiLineString == null);
        }
      }

      // Test WKT
      dataLines = _wktDataLines;
      Assert.True(MultiLineString.IsValid(dataLines[32], GeometryAsTextFormatType.WKT));
      multiLineString = MultiLineString.Parse(dataLines[32], GeometryAsTextFormatType.WKT);
      Assert.True(multiLineString != null);
      Assert.True(multiLineString.GeometriesCount == 3);
      lineString = multiLineString.Geometries[0] as LineString;
      Assert.True(lineString != null);
      Assert.True(lineString.CoordinatesCount == 2);
      Assert.True(lineString.Coordinates[0] == new Coordinate(10.0, 10.0));
      Assert.True(lineString.Coordinates[1] == new Coordinate(20.0, 20.0));
      lineString = multiLineString.Geometries[1] as LineString;
      Assert.True(lineString != null);
      Assert.True(lineString.CoordinatesCount == 2);
      Assert.True(lineString.Coordinates[0] == new Coordinate(15.0, 15.0));
      Assert.True(lineString.Coordinates[1] == new Coordinate(30.0, 15.0));
      lineString = multiLineString.Geometries[2] as LineString;
      Assert.True(lineString != null);
      Assert.True(lineString.CoordinatesCount == 2);
      Assert.True(lineString.Coordinates[0] == new Coordinate(5.0, 5.0));
      Assert.True(lineString.Coordinates[1] == new Coordinate(10.0, 9.0));
      ValidateMultiLineStringToString(multiLineString, GeometryAsTextFormatType.WKT);
      Assert.True(MultiLineString.IsValid(dataLines[33], GeometryAsTextFormatType.WKT));
      multiLineString = MultiLineString.Parse(dataLines[33], GeometryAsTextFormatType.WKT);
      Assert.True(multiLineString != null);
      Assert.True(multiLineString.SRID == 50);
      Assert.True(multiLineString.GeometriesCount == 5);
      lineString = multiLineString.Geometries[0] as LineString;
      Assert.True(lineString != null);
      Assert.True(lineString.SRID == 50);
      Assert.True(lineString.IsEmpty);
      lineString = multiLineString.Geometries[1] as LineString;
      Assert.True(lineString != null);
      Assert.True(lineString.SRID == 50);
      Assert.True(lineString.CoordinatesCount == 3);
      Assert.True(lineString.Coordinates[0] == new Coordinate(10.0, 10.0));
      Assert.True(lineString.Coordinates[1] == new Coordinate(20.0, 20.0));
      Assert.True(lineString.Coordinates[2] == new Coordinate(30.0, 30.0));
      lineString = multiLineString.Geometries[2] as LineString;
      Assert.True(lineString != null);
      Assert.True(lineString.SRID == 50);
      Assert.True(lineString.CoordinatesCount == 3);
      Assert.True(lineString.Coordinates[0] == new Coordinate(15.0, 15.0));
      Assert.True(lineString.Coordinates[1] == new Coordinate(30.0, 15.0));
      Assert.True(lineString.Coordinates[2] == new Coordinate(45.0, 15.0));
      lineString = multiLineString.Geometries[3] as LineString;
      Assert.True(lineString != null);
      Assert.True(lineString.SRID == 50);
      Assert.True(lineString.CoordinatesCount == 3);
      Assert.True(lineString.Coordinates[0] == new Coordinate(5.0, 5.0));
      Assert.True(lineString.Coordinates[1] == new Coordinate(10.0, 9.0));
      Assert.True(lineString.Coordinates[2] == new Coordinate(8.0, 12.0));
      lineString = multiLineString.Geometries[4] as LineString;
      Assert.True(lineString != null);
      Assert.True(lineString.SRID == 50);
      Assert.True(lineString.IsEmpty);
      ValidateMultiLineStringToString(multiLineString, GeometryAsTextFormatType.WKT);
      for (int i = 34; i < 36; i++)
      {
        Assert.True(MultiLineString.IsValid(dataLines[i], GeometryAsTextFormatType.WKT));
        multiLineString = MultiLineString.Parse(dataLines[i], GeometryAsTextFormatType.WKT);
        Assert.True(multiLineString != null);
        Assert.True(multiLineString.IsEmpty);
        ValidateMultiLineStringToString(multiLineString, GeometryAsTextFormatType.WKT);
      }

      Assert.True(MultiLineString.IsValid(dataLines[36], GeometryAsTextFormatType.WKT));
      multiLineString = MultiLineString.Parse(dataLines[36], GeometryAsTextFormatType.WKT);
      Assert.True(multiLineString != null);
      Assert.True(multiLineString.GeometriesCount == 3);
      lineString = multiLineString.Geometries[0] as LineString;
      Assert.True(lineString != null);
      Assert.True(lineString.CoordinatesCount == 2);
      Assert.True(lineString.Coordinates[0] == new Coordinate(10.0, 10.0));
      Assert.True(lineString.Coordinates[1] == new Coordinate(20.0, 20.0));
      lineString = multiLineString.Geometries[1] as LineString;
      Assert.True(lineString != null);
      Assert.True(lineString.CoordinatesCount == 2);
      Assert.True(lineString.Coordinates[0] == new Coordinate(15.0, 15.0));
      Assert.True(lineString.Coordinates[1] == new Coordinate(30.0, 15.0));
      lineString = multiLineString.Geometries[2] as LineString;
      Assert.True(lineString != null);
      Assert.True(lineString.CoordinatesCount == 2);
      Assert.True(lineString.Coordinates[0] == new Coordinate(5.0, 5.0));
      Assert.True(lineString.Coordinates[1] == new Coordinate(10.0, 9.0));
      ValidateMultiLineStringToString(multiLineString, GeometryAsTextFormatType.WKT);
      for (int i = 37; i < 40; i++)
      {
        Assert.False(MultiLineString.IsValid(dataLines[i], GeometryAsTextFormatType.WKT));
        multiLineString = MultiLineString.Parse(dataLines[i], GeometryAsTextFormatType.WKT);
        Assert.True(multiLineString == null);
      }
    }

    /// <summary>
    /// Performs unit tests on the <see cref="MultiPoint"/> class using data in all supported formats.
    /// </summary>
    [Fact]
    public void TestMultiPoint()
    {
      // Read data files
      ReadData();

      // Test GeoJSON and GML
      Point point;
      MultiPoint multiPoint;
      string[] dataLines;
      foreach (GeometryAsTextFormatType format in Enum.GetValues(typeof(GeometryAsTextFormatType)))
      {
        switch (format)
        {
          case GeometryAsTextFormatType.GeoJSON:
            dataLines = _geoJsonDataLines;
            break;

          case GeometryAsTextFormatType.GML:
            dataLines = _gmlDataLines;
            break;

          default:
            continue;
        }

        Assert.True(MultiPoint.IsValid(dataLines[24], format));
        multiPoint = MultiPoint.Parse(dataLines[24], format);
        Assert.True(multiPoint != null);
        Assert.True(multiPoint.GeometriesCount == 3);
        point = multiPoint.Geometries[0] as Point;
        Assert.True(point != null);
        Assert.True(point.Coordinate == new Coordinate(1.0, 1.0));
        point = multiPoint.Geometries[1] as Point;
        Assert.True(point != null);
        Assert.True(point.Coordinate == new Coordinate(2.0, 2.0));
        point = multiPoint.Geometries[2] as Point;
        Assert.True(point != null);
        Assert.True(point.Coordinate == new Coordinate(3.0, 3.0));
        ValidateMultiPointToString(multiPoint, format);
        Assert.True(MultiPoint.IsValid(dataLines[25], format));
        multiPoint = MultiPoint.Parse(dataLines[25], format);
        Assert.True(multiPoint != null);
        Assert.True(multiPoint.GeometriesCount == 2);
        point = multiPoint.Geometries[0] as Point;
        Assert.True(point != null);
        Assert.True(point.IsEmpty);
        point = multiPoint.Geometries[1] as Point;
        Assert.True(point != null);
        Assert.True(point.Coordinate == new Coordinate(1.0, 1.0));
        ValidateMultiPointToString(multiPoint, format);
        Assert.True(MultiPoint.IsValid(dataLines[26], format));
        multiPoint = MultiPoint.Parse(dataLines[26], format);
        Assert.True(multiPoint != null);
        Assert.True(multiPoint.IsEmpty);
        ValidateMultiPointToString(multiPoint, format);
        Assert.True(MultiPoint.IsValid(dataLines[27], format));
        multiPoint = MultiPoint.Parse(dataLines[27], format);
        Assert.True(multiPoint != null);
        Assert.True(multiPoint.GeometriesCount == 3);
        point = multiPoint.Geometries[0] as Point;
        Assert.True(point != null);
        Assert.True(point.Coordinate == new Coordinate(1.0, 1.0));
        point = multiPoint.Geometries[1] as Point;
        Assert.True(point != null);
        Assert.True(point.Coordinate == new Coordinate(2.0, 2.0));
        point = multiPoint.Geometries[2] as Point;
        Assert.True(point != null);
        Assert.True(point.Coordinate == new Coordinate(3.0, 3.0));
        ValidateMultiPointToString(multiPoint, format);
        for (int i = 28; i < 32; i++)
        {
          Assert.False(MultiPoint.IsValid(dataLines[i], format));
          multiPoint = MultiPoint.Parse(dataLines[i], format);
          Assert.True(multiPoint == null);
        }
      }

      // Test WKT
      dataLines = _wktDataLines;
      Assert.True(MultiPoint.IsValid(dataLines[24], GeometryAsTextFormatType.WKT));
      multiPoint = MultiPoint.Parse(dataLines[24], GeometryAsTextFormatType.WKT);
      Assert.True(multiPoint != null);
      Assert.True(multiPoint.GeometriesCount == 3);
      point = multiPoint.Geometries[0] as Point;
      Assert.True(point != null);
      Assert.True(point.Coordinate == new Coordinate(1.0, 1.0));
      point = multiPoint.Geometries[1] as Point;
      Assert.True(point != null);
      Assert.True(point.Coordinate == new Coordinate(2.0, 2.0));
      point = multiPoint.Geometries[2] as Point;
      Assert.True(point != null);
      Assert.True(point.Coordinate == new Coordinate(3.0, 3.0));
      ValidateMultiPointToString(multiPoint, GeometryAsTextFormatType.WKT);
      Assert.True(MultiPoint.IsValid(dataLines[25], GeometryAsTextFormatType.WKT));
      multiPoint = MultiPoint.Parse(dataLines[25], GeometryAsTextFormatType.WKT);
      Assert.True(multiPoint != null);
      Assert.True(multiPoint.SRID == 40);
      Assert.True(multiPoint.GeometriesCount == 3);
      point = multiPoint.Geometries[0] as Point;
      Assert.True(point != null);
      Assert.True(point.SRID == 40);
      Assert.True(point.IsEmpty);
      point = multiPoint.Geometries[1] as Point;
      Assert.True(point != null);
      Assert.True(point.SRID == 40);
      Assert.True(point.Coordinate == new Coordinate(1.0, 1.0));
      point = multiPoint.Geometries[2] as Point;
      Assert.True(point != null);
      Assert.True(point.SRID == 40);
      Assert.True(point.Coordinate == new Coordinate(2.0, 2.0));
      ValidateMultiPointToString(multiPoint, GeometryAsTextFormatType.WKT);
      for (int i = 26; i < 28; i++)
      {
        Assert.True(MultiPoint.IsValid(dataLines[i], GeometryAsTextFormatType.WKT));
        multiPoint = MultiPoint.Parse(dataLines[i], GeometryAsTextFormatType.WKT);
        Assert.True(multiPoint != null);
        Assert.True(multiPoint.IsEmpty);
        ValidateMultiPointToString(multiPoint, GeometryAsTextFormatType.WKT);
      }

      Assert.True(MultiPoint.IsValid(dataLines[28], GeometryAsTextFormatType.WKT));
      multiPoint = MultiPoint.Parse(dataLines[28], GeometryAsTextFormatType.WKT);
      Assert.True(multiPoint != null);
      Assert.True(multiPoint.GeometriesCount == 3);
      point = multiPoint.Geometries[0] as Point;
      Assert.True(point != null);
      Assert.True(point.Coordinate == new Coordinate(1.0, 1.0));
      point = multiPoint.Geometries[1] as Point;
      Assert.True(point != null);
      Assert.True(point.Coordinate == new Coordinate(2.0, 2.0));
      point = multiPoint.Geometries[2] as Point;
      Assert.True(point != null);
      Assert.True(point.Coordinate == new Coordinate(3.0, 3.0));
      ValidateMultiPointToString(multiPoint, GeometryAsTextFormatType.WKT);
      for (int i = 29; i < 32; i++)
      {
        Assert.False(MultiPoint.IsValid(dataLines[i], GeometryAsTextFormatType.WKT));
        multiPoint = MultiPoint.Parse(dataLines[i], GeometryAsTextFormatType.WKT);
        Assert.True(multiPoint == null);
      }
    }

    /// <summary>
    /// Performs unit tests on the <see cref="MultiPolygon"/> class using data in all supported formats.
    /// </summary>
    [Fact]
    public void TestMultiPolygon()
    {
      // Read data files
      ReadData();

      // Test GeoJSON and GML
      Polygon polygon;
      MultiPolygon multiPolygon;
      string[] dataLines;
      foreach (GeometryAsTextFormatType format in Enum.GetValues(typeof(GeometryAsTextFormatType)))
      {
        switch (format)
        {
          case GeometryAsTextFormatType.GeoJSON:
            dataLines = _geoJsonDataLines;
            break;

          case GeometryAsTextFormatType.GML:
            dataLines = _gmlDataLines;
            break;

          default:
            continue;
        }

        Assert.True(MultiPolygon.IsValid(dataLines[40], format));
        multiPolygon = MultiPolygon.Parse(dataLines[40], format);
        Assert.True(multiPolygon != null);
        Assert.True(multiPolygon.GeometriesCount == 2);
        polygon = multiPolygon.Geometries[0] as Polygon;
        Assert.True(polygon != null);
        Assert.True(polygon.HolesCount == 2);
        Assert.True(polygon.Shell.CoordinatesCount == 5);
        Assert.True(polygon.Shell.IsRing);
        Assert.True(polygon.Shell.Coordinates[0] == new Coordinate(0.0, 0.0));
        Assert.True(polygon.Shell.Coordinates[1] == new Coordinate(10.0, 0.0));
        Assert.True(polygon.Shell.Coordinates[2] == new Coordinate(10.0, 10.0));
        Assert.True(polygon.Shell.Coordinates[3] == new Coordinate(0.0, 10.0));
        Assert.True(polygon.Shell.Coordinates[4] == new Coordinate(0.0, 0.0));
        Assert.True(polygon.Holes[0].CoordinatesCount == 5);
        Assert.True(polygon.Holes[0].IsRing);
        Assert.True(polygon.Holes[0].Coordinates[0] == new Coordinate(5.0, 5.0));
        Assert.True(polygon.Holes[0].Coordinates[1] == new Coordinate(7.0, 5.0));
        Assert.True(polygon.Holes[0].Coordinates[2] == new Coordinate(7.0, 7.0));
        Assert.True(polygon.Holes[0].Coordinates[3] == new Coordinate(5.0, 7.0));
        Assert.True(polygon.Holes[0].Coordinates[4] == new Coordinate(5.0, 5.0));
        Assert.True(polygon.Holes[1].CoordinatesCount == 5);
        Assert.True(polygon.Holes[1].IsRing);
        Assert.True(polygon.Holes[1].Coordinates[0] == new Coordinate(5.0, 1.0));
        Assert.True(polygon.Holes[1].Coordinates[1] == new Coordinate(7.0, 1.0));
        Assert.True(polygon.Holes[1].Coordinates[2] == new Coordinate(7.0, 3.0));
        Assert.True(polygon.Holes[1].Coordinates[3] == new Coordinate(5.0, 3.0));
        Assert.True(polygon.Holes[1].Coordinates[4] == new Coordinate(5.0, 1.0));
        polygon = multiPolygon.Geometries[1] as Polygon;
        Assert.True(polygon != null);
        Assert.True(polygon.HolesCount == 2);
        Assert.True(polygon.Shell.CoordinatesCount == 5);
        Assert.True(polygon.Shell.IsRing);
        Assert.True(polygon.Shell.Coordinates[0] == new Coordinate(2.0, 2.0));
        Assert.True(polygon.Shell.Coordinates[1] == new Coordinate(12.0, 2.0));
        Assert.True(polygon.Shell.Coordinates[2] == new Coordinate(12.0, 12.0));
        Assert.True(polygon.Shell.Coordinates[3] == new Coordinate(2.0, 12.0));
        Assert.True(polygon.Shell.Coordinates[4] == new Coordinate(2.0, 2.0));
        Assert.True(polygon.Holes[0].CoordinatesCount == 5);
        Assert.True(polygon.Holes[0].IsRing);
        Assert.True(polygon.Holes[0].Coordinates[0] == new Coordinate(7.0, 7.0));
        Assert.True(polygon.Holes[0].Coordinates[1] == new Coordinate(9.0, 7.0));
        Assert.True(polygon.Holes[0].Coordinates[2] == new Coordinate(9.0, 9.0));
        Assert.True(polygon.Holes[0].Coordinates[3] == new Coordinate(7.0, 9.0));
        Assert.True(polygon.Holes[0].Coordinates[4] == new Coordinate(7.0, 7.0));
        Assert.True(polygon.Holes[1].CoordinatesCount == 5);
        Assert.True(polygon.Holes[1].IsRing);
        Assert.True(polygon.Holes[1].Coordinates[0] == new Coordinate(7.0, 3.0));
        Assert.True(polygon.Holes[1].Coordinates[1] == new Coordinate(9.0, 3.0));
        Assert.True(polygon.Holes[1].Coordinates[2] == new Coordinate(9.0, 5.0));
        Assert.True(polygon.Holes[1].Coordinates[3] == new Coordinate(7.0, 5.0));
        Assert.True(polygon.Holes[1].Coordinates[4] == new Coordinate(7.0, 3.0));
        ValidateMultiPolygonToString(multiPolygon, format);
        Assert.True(MultiPolygon.IsValid(dataLines[41], format));
        multiPolygon = MultiPolygon.Parse(dataLines[41], format);
        Assert.True(multiPolygon != null);
        Assert.True(multiPolygon.GeometriesCount == 2);
        polygon = multiPolygon.Geometries[0] as Polygon;
        Assert.True(polygon != null);
        Assert.True(polygon.IsEmpty);
        polygon = multiPolygon.Geometries[1] as Polygon;
        Assert.True(polygon != null);
        Assert.True(polygon.HolesCount == 2);
        Assert.True(polygon.Shell.CoordinatesCount == 5);
        Assert.True(polygon.Shell.IsRing);
        Assert.True(polygon.Shell.Coordinates[0] == new Coordinate(2.0, 2.0));
        Assert.True(polygon.Shell.Coordinates[1] == new Coordinate(12.0, 2.0));
        Assert.True(polygon.Shell.Coordinates[2] == new Coordinate(12.0, 12.0));
        Assert.True(polygon.Shell.Coordinates[3] == new Coordinate(2.0, 12.0));
        Assert.True(polygon.Shell.Coordinates[4] == new Coordinate(2.0, 2.0));
        Assert.True(polygon.Holes[0].CoordinatesCount == 5);
        Assert.True(polygon.Holes[0].IsRing);
        Assert.True(polygon.Holes[0].Coordinates[0] == new Coordinate(7.0, 7.0));
        Assert.True(polygon.Holes[0].Coordinates[1] == new Coordinate(9.0, 7.0));
        Assert.True(polygon.Holes[0].Coordinates[2] == new Coordinate(9.0, 9.0));
        Assert.True(polygon.Holes[0].Coordinates[3] == new Coordinate(7.0, 9.0));
        Assert.True(polygon.Holes[0].Coordinates[4] == new Coordinate(7.0, 7.0));
        Assert.True(polygon.Holes[1].CoordinatesCount == 5);
        Assert.True(polygon.Holes[1].IsRing);
        Assert.True(polygon.Holes[1].Coordinates[0] == new Coordinate(7.0, 3.0));
        Assert.True(polygon.Holes[1].Coordinates[1] == new Coordinate(9.0, 3.0));
        Assert.True(polygon.Holes[1].Coordinates[2] == new Coordinate(9.0, 5.0));
        Assert.True(polygon.Holes[1].Coordinates[3] == new Coordinate(7.0, 5.0));
        Assert.True(polygon.Holes[1].Coordinates[4] == new Coordinate(7.0, 3.0));
        ValidateMultiPolygonToString(multiPolygon, format);
        Assert.True(MultiPolygon.IsValid(dataLines[42], format));
        multiPolygon = MultiPolygon.Parse(dataLines[42], format);
        Assert.True(multiPolygon != null);
        Assert.True(multiPolygon.IsEmpty);
        ValidateMultiPolygonToString(multiPolygon, format);
        Assert.True(MultiPolygon.IsValid(dataLines[43], format));
        multiPolygon = MultiPolygon.Parse(dataLines[43], format);
        Assert.True(multiPolygon != null);
        Assert.True(multiPolygon.GeometriesCount == 1);
        polygon = multiPolygon.Geometries[0] as Polygon;
        Assert.True(polygon != null);
        Assert.True(polygon.HolesCount == 2);
        Assert.True(polygon.Shell.CoordinatesCount == 5);
        Assert.True(polygon.Shell.IsRing);
        Assert.True(polygon.Shell.Coordinates[0] == new Coordinate(0.0, 0.0));
        Assert.True(polygon.Shell.Coordinates[1] == new Coordinate(10.0, 0.0));
        Assert.True(polygon.Shell.Coordinates[2] == new Coordinate(10.0, 10.0));
        Assert.True(polygon.Shell.Coordinates[3] == new Coordinate(0.0, 10.0));
        Assert.True(polygon.Shell.Coordinates[4] == new Coordinate(0.0, 0.0));
        Assert.True(polygon.Holes[0].CoordinatesCount == 5);
        Assert.True(polygon.Holes[0].IsRing);
        Assert.True(polygon.Holes[0].Coordinates[0] == new Coordinate(5.0, 5.0));
        Assert.True(polygon.Holes[0].Coordinates[1] == new Coordinate(7.0, 5.0));
        Assert.True(polygon.Holes[0].Coordinates[2] == new Coordinate(7.0, 7.0));
        Assert.True(polygon.Holes[0].Coordinates[3] == new Coordinate(5.0, 7.0));
        Assert.True(polygon.Holes[0].Coordinates[4] == new Coordinate(5.0, 5.0));
        Assert.True(polygon.Holes[1].CoordinatesCount == 5);
        Assert.True(polygon.Holes[1].IsRing);
        Assert.True(polygon.Holes[1].Coordinates[0] == new Coordinate(5.0, 1.0));
        Assert.True(polygon.Holes[1].Coordinates[1] == new Coordinate(7.0, 1.0));
        Assert.True(polygon.Holes[1].Coordinates[2] == new Coordinate(7.0, 3.0));
        Assert.True(polygon.Holes[1].Coordinates[3] == new Coordinate(5.0, 3.0));
        Assert.True(polygon.Holes[1].Coordinates[4] == new Coordinate(5.0, 1.0));
        ValidateMultiPolygonToString(multiPolygon, format);
        for (int i = 44; i < 48; i++)
        {
          Assert.False(MultiPolygon.IsValid(dataLines[i], format));
          multiPolygon = MultiPolygon.Parse(dataLines[i], format);
          Assert.True(multiPolygon == null);
        }
      }

      // Test WKT
      dataLines = _wktDataLines;
      Assert.True(MultiPolygon.IsValid(dataLines[40], GeometryAsTextFormatType.WKT));
      multiPolygon = MultiPolygon.Parse(dataLines[40], GeometryAsTextFormatType.WKT);
      Assert.True(multiPolygon != null);
      Assert.True(multiPolygon.GeometriesCount == 2);
      polygon = multiPolygon.Geometries[0] as Polygon;
      Assert.True(polygon != null);
      Assert.True(polygon.HolesCount == 2);
      Assert.True(polygon.Shell.CoordinatesCount == 5);
      Assert.True(polygon.Shell.IsRing);
      Assert.True(polygon.Shell.Coordinates[0] == new Coordinate(0.0, 0.0));
      Assert.True(polygon.Shell.Coordinates[1] == new Coordinate(10.0, 0.0));
      Assert.True(polygon.Shell.Coordinates[2] == new Coordinate(10.0, 10.0));
      Assert.True(polygon.Shell.Coordinates[3] == new Coordinate(0.0, 10.0));
      Assert.True(polygon.Shell.Coordinates[4] == new Coordinate(0.0, 0.0));
      Assert.True(polygon.Holes[0].CoordinatesCount == 5);
      Assert.True(polygon.Holes[0].IsRing);
      Assert.True(polygon.Holes[0].Coordinates[0] == new Coordinate(5.0, 5.0));
      Assert.True(polygon.Holes[0].Coordinates[1] == new Coordinate(7.0, 5.0));
      Assert.True(polygon.Holes[0].Coordinates[2] == new Coordinate(7.0, 7.0));
      Assert.True(polygon.Holes[0].Coordinates[3] == new Coordinate(5.0, 7.0));
      Assert.True(polygon.Holes[0].Coordinates[4] == new Coordinate(5.0, 5.0));
      Assert.True(polygon.Holes[1].CoordinatesCount == 5);
      Assert.True(polygon.Holes[1].IsRing);
      Assert.True(polygon.Holes[1].Coordinates[0] == new Coordinate(5.0, 1.0));
      Assert.True(polygon.Holes[1].Coordinates[1] == new Coordinate(7.0, 1.0));
      Assert.True(polygon.Holes[1].Coordinates[2] == new Coordinate(7.0, 3.0));
      Assert.True(polygon.Holes[1].Coordinates[3] == new Coordinate(5.0, 3.0));
      Assert.True(polygon.Holes[1].Coordinates[4] == new Coordinate(5.0, 1.0));
      polygon = multiPolygon.Geometries[1] as Polygon;
      Assert.True(polygon != null);
      Assert.True(polygon.HolesCount == 2);
      Assert.True(polygon.Shell.CoordinatesCount == 5);
      Assert.True(polygon.Shell.IsRing);
      Assert.True(polygon.Shell.Coordinates[0] == new Coordinate(2.0, 2.0));
      Assert.True(polygon.Shell.Coordinates[1] == new Coordinate(12.0, 2.0));
      Assert.True(polygon.Shell.Coordinates[2] == new Coordinate(12.0, 12.0));
      Assert.True(polygon.Shell.Coordinates[3] == new Coordinate(2.0, 12.0));
      Assert.True(polygon.Shell.Coordinates[4] == new Coordinate(2.0, 2.0));
      Assert.True(polygon.Holes[0].CoordinatesCount == 5);
      Assert.True(polygon.Holes[0].IsRing);
      Assert.True(polygon.Holes[0].Coordinates[0] == new Coordinate(7.0, 7.0));
      Assert.True(polygon.Holes[0].Coordinates[1] == new Coordinate(9.0, 7.0));
      Assert.True(polygon.Holes[0].Coordinates[2] == new Coordinate(9.0, 9.0));
      Assert.True(polygon.Holes[0].Coordinates[3] == new Coordinate(7.0, 9.0));
      Assert.True(polygon.Holes[0].Coordinates[4] == new Coordinate(7.0, 7.0));
      Assert.True(polygon.Holes[1].CoordinatesCount == 5);
      Assert.True(polygon.Holes[1].IsRing);
      Assert.True(polygon.Holes[1].Coordinates[0] == new Coordinate(7.0, 3.0));
      Assert.True(polygon.Holes[1].Coordinates[1] == new Coordinate(9.0, 3.0));
      Assert.True(polygon.Holes[1].Coordinates[2] == new Coordinate(9.0, 5.0));
      Assert.True(polygon.Holes[1].Coordinates[3] == new Coordinate(7.0, 5.0));
      Assert.True(polygon.Holes[1].Coordinates[4] == new Coordinate(7.0, 3.0));
      ValidateMultiPolygonToString(multiPolygon, GeometryAsTextFormatType.WKT);
      Assert.True(MultiPolygon.IsValid(dataLines[41], GeometryAsTextFormatType.WKT));
      multiPolygon = MultiPolygon.Parse(dataLines[41], GeometryAsTextFormatType.WKT);
      Assert.True(multiPolygon != null);
      Assert.True(multiPolygon.SRID == 60);
      Assert.True(multiPolygon.GeometriesCount == 3);
      polygon = multiPolygon.Geometries[0] as Polygon;
      Assert.True(polygon != null);
      Assert.True(polygon.SRID == 60);
      Assert.True(polygon.IsEmpty);
      polygon = multiPolygon.Geometries[1] as Polygon;
      Assert.True(polygon != null);
      Assert.True(polygon.SRID == 60);
      Assert.True(polygon.HolesCount == 2);
      Assert.True(polygon.Shell.CoordinatesCount == 5);
      Assert.True(polygon.Shell.IsRing);
      Assert.True(polygon.Shell.Coordinates[0] == new Coordinate(2.0, 2.0));
      Assert.True(polygon.Shell.Coordinates[1] == new Coordinate(12.0, 2.0));
      Assert.True(polygon.Shell.Coordinates[2] == new Coordinate(12.0, 12.0));
      Assert.True(polygon.Shell.Coordinates[3] == new Coordinate(2.0, 12.0));
      Assert.True(polygon.Shell.Coordinates[4] == new Coordinate(2.0, 2.0));
      Assert.True(polygon.Holes[0].CoordinatesCount == 5);
      Assert.True(polygon.Holes[0].IsRing);
      Assert.True(polygon.Holes[0].Coordinates[0] == new Coordinate(7.0, 7.0));
      Assert.True(polygon.Holes[0].Coordinates[1] == new Coordinate(9.0, 7.0));
      Assert.True(polygon.Holes[0].Coordinates[2] == new Coordinate(9.0, 9.0));
      Assert.True(polygon.Holes[0].Coordinates[3] == new Coordinate(7.0, 9.0));
      Assert.True(polygon.Holes[0].Coordinates[4] == new Coordinate(7.0, 7.0));
      Assert.True(polygon.Holes[1].CoordinatesCount == 5);
      Assert.True(polygon.Holes[1].IsRing);
      Assert.True(polygon.Holes[1].Coordinates[0] == new Coordinate(7.0, 3.0));
      Assert.True(polygon.Holes[1].Coordinates[1] == new Coordinate(9.0, 3.0));
      Assert.True(polygon.Holes[1].Coordinates[2] == new Coordinate(9.0, 5.0));
      Assert.True(polygon.Holes[1].Coordinates[3] == new Coordinate(7.0, 5.0));
      Assert.True(polygon.Holes[1].Coordinates[4] == new Coordinate(7.0, 3.0));
      polygon = multiPolygon.Geometries[2] as Polygon;
      Assert.True(polygon != null);
      Assert.True(polygon.SRID == 60);
      Assert.True(polygon.IsEmpty);
      ValidateMultiPolygonToString(multiPolygon, GeometryAsTextFormatType.WKT);
      for (int i = 42; i < 44; i++)
      {
        Assert.True(MultiPolygon.IsValid(dataLines[i], GeometryAsTextFormatType.WKT));
        multiPolygon = MultiPolygon.Parse(dataLines[i], GeometryAsTextFormatType.WKT);
        Assert.True(multiPolygon != null);
        Assert.True(multiPolygon.IsEmpty);
        ValidateMultiPolygonToString(multiPolygon, GeometryAsTextFormatType.WKT);
      }


      Assert.True(MultiPolygon.IsValid(dataLines[44], GeometryAsTextFormatType.WKT));
      multiPolygon = MultiPolygon.Parse(dataLines[44], GeometryAsTextFormatType.WKT);
      Assert.True(multiPolygon != null);
      Assert.True(multiPolygon.GeometriesCount == 1);
      polygon = multiPolygon.Geometries[0] as Polygon;
      Assert.True(polygon != null);
      Assert.True(polygon.HolesCount == 1);
      Assert.True(polygon.Shell.CoordinatesCount == 5);
      Assert.True(polygon.Shell.IsRing);
      Assert.True(polygon.Shell.Coordinates[0] == new Coordinate(0.0, 0.0));
      Assert.True(polygon.Shell.Coordinates[1] == new Coordinate(10.0, 0.0));
      Assert.True(polygon.Shell.Coordinates[2] == new Coordinate(10.0, 10.0));
      Assert.True(polygon.Shell.Coordinates[3] == new Coordinate(0.0, 10.0));
      Assert.True(polygon.Shell.Coordinates[4] == new Coordinate(0.0, 0.0));
      Assert.True(polygon.Holes[0].CoordinatesCount == 5);
      Assert.True(polygon.Holes[0].IsRing);
      Assert.True(polygon.Holes[0].Coordinates[0] == new Coordinate(5.0, 5.0));
      Assert.True(polygon.Holes[0].Coordinates[1] == new Coordinate(7.0, 5.0));
      Assert.True(polygon.Holes[0].Coordinates[2] == new Coordinate(7.0, 7.0));
      Assert.True(polygon.Holes[0].Coordinates[3] == new Coordinate(5.0, 7.0));
      Assert.True(polygon.Holes[0].Coordinates[4] == new Coordinate(5.0, 5.0));
      ValidateMultiPolygonToString(multiPolygon, GeometryAsTextFormatType.WKT);
      for (int i = 45; i < 48; i++)
      {
        Assert.False(MultiPolygon.IsValid(dataLines[i], GeometryAsTextFormatType.WKT));
        multiPolygon = MultiPolygon.Parse(dataLines[i], GeometryAsTextFormatType.WKT);
        Assert.True(multiPolygon == null);
      }
    }

    /// <summary>
    /// Performs unit tests on the <see cref="Point"/> class using data in all supported formats.
    /// </summary>
    [Fact]
    public void TestPoint()
    {
      // Read data files
      ReadData();

      // Test GeoJSON, GML and KML
      Point point;
      string[] dataLines;
      foreach (GeometryAsTextFormatType format in Enum.GetValues(typeof(GeometryAsTextFormatType)))
      {
        switch (format)
        {
          case GeometryAsTextFormatType.GeoJSON:
            dataLines = _geoJsonDataLines;
            break;

          case GeometryAsTextFormatType.GML:
            dataLines = _gmlDataLines;
            break;

          case GeometryAsTextFormatType.KML:
            dataLines = _kmlDataLines;
            break;

          default:
            continue;
        }

        Assert.True(Point.IsValid(dataLines[0], format));
        point = Point.Parse(dataLines[0], format);
        Assert.True(point != null);
        Assert.True(point.Coordinate == new Coordinate(-112.8185647, 49.6999986));
        ValidatePointToString(point, format);
        Assert.True(Point.IsValid(dataLines[1], format));
        var point2 = Point.Parse(dataLines[1], format);
        Assert.True(point2 != null);
        Assert.True(point == point2);
        ValidatePointToString(point, format);
        Assert.True(Point.IsValid(dataLines[2], format));
        point = Point.Parse(dataLines[2], format);
        Assert.True(point != null);
        Assert.True(point.IsEmpty);
        ValidatePointToString(point, format);
        Assert.True(Point.IsValid(dataLines[3], format));
        point = Point.Parse(dataLines[3], format);
        Assert.True(point != null);
        Assert.True(point.Coordinate == new Coordinate(153.1408538, -27.6333361));
        ValidatePointToString(point, format);
        for (int i = 4; i < 8; i++)
        {
          Assert.False(Point.IsValid(dataLines[i], format));
          point = Point.Parse(dataLines[i], format);
          Assert.True(point == null);
        }
      }

      // Test WKT
      dataLines = _wktDataLines;
      Assert.True(Point.IsValid(dataLines[0], GeometryAsTextFormatType.WKT));
      point = Point.Parse(dataLines[0], GeometryAsTextFormatType.WKT);
      Assert.True(point != null);
      Assert.True(point.Coordinate == new Coordinate(-112.8185647, 49.6999986));
      ValidatePointToString(point, GeometryAsTextFormatType.WKT);
      Assert.True(Point.IsValid(dataLines[1], GeometryAsTextFormatType.WKT));
      point = Point.Parse(dataLines[1], GeometryAsTextFormatType.WKT);
      Assert.True(point != null);
      Assert.True(point.SRID == 10);
      Assert.True(point.Coordinate == new Coordinate(1, 1));
      ValidatePointToString(point, GeometryAsTextFormatType.WKT);
      for (int i = 2; i < 4; i++)
      {
        Assert.True(Point.IsValid(dataLines[i], GeometryAsTextFormatType.WKT));
        point = Point.Parse(dataLines[i], GeometryAsTextFormatType.WKT);
        Assert.True(point != null);
        Assert.True(point.IsEmpty);
        ValidatePointToString(point, GeometryAsTextFormatType.WKT);
      }

      Assert.True(Point.IsValid(dataLines[4], GeometryAsTextFormatType.WKT));
      point = Point.Parse(dataLines[4], GeometryAsTextFormatType.WKT);
      Assert.True(point != null);
      Assert.True(point.Coordinate == new Coordinate(30, 305));
      ValidatePointToString(point, GeometryAsTextFormatType.WKT);
      for (int i = 5; i < 8; i++)
      {
        Assert.False(Point.IsValid(dataLines[i], GeometryAsTextFormatType.WKT));
        point = Point.Parse(dataLines[i], GeometryAsTextFormatType.WKT);
        Assert.True(point == null);
      }
    }

    /// <summary>
    /// Performs unit tests on the <see cref="Polygon"/> class using data in all supported formats.
    /// </summary>
    [Fact]
    public void TestPolygon()
    {
      // Read data files
      ReadData();

      // Test GeoJSON, GML and KML
      Polygon polygon;
      string[] dataLines;
      foreach (GeometryAsTextFormatType format in Enum.GetValues(typeof(GeometryAsTextFormatType)))
      {
        switch (format)
        {
          case GeometryAsTextFormatType.GeoJSON:
            dataLines = _geoJsonDataLines;
            break;

          case GeometryAsTextFormatType.GML:
            dataLines = _gmlDataLines;
            break;

          case GeometryAsTextFormatType.KML:
            dataLines = _kmlDataLines;
            break;

          default:
            continue;
        }

        Assert.True(Polygon.IsValid(dataLines[16], format));
        polygon = Polygon.Parse(dataLines[16], format);
        Assert.True(polygon != null);
        Assert.True(polygon.HolesCount == 0);
        Assert.True(polygon.Shell.CoordinatesCount == 7);
        Assert.True(polygon.Shell.IsRing);
        Assert.True(polygon.Shell.Coordinates[0] == new Coordinate(105.07547, 78.30689));
        Assert.True(polygon.Shell.Coordinates[1] == new Coordinate(99.43814, 77.921));
        Assert.True(polygon.Shell.Coordinates[2] == new Coordinate(101.2649, 79.23399));
        Assert.True(polygon.Shell.Coordinates[3] == new Coordinate(102.08635, 79.34641));
        Assert.True(polygon.Shell.Coordinates[4] == new Coordinate(102.837815, 79.28129));
        Assert.True(polygon.Shell.Coordinates[5] == new Coordinate(105.37243, 78.713340000000102));
        Assert.True(polygon.Shell.Coordinates[6] == new Coordinate(105.07547, 78.30689));
        ValidatePolygonToString(polygon, format);
        Assert.True(Polygon.IsValid(dataLines[17], format));
        polygon = Polygon.Parse(dataLines[17], format);
        Assert.True(polygon != null);
        Assert.True(polygon.HolesCount == 2);
        Assert.True(polygon.Shell.CoordinatesCount == 5);
        Assert.True(polygon.Shell.IsRing);
        Assert.True(polygon.Shell.Coordinates[0] == new Coordinate(0.0, 0.0));
        Assert.True(polygon.Shell.Coordinates[1] == new Coordinate(10.0, 0.0));
        Assert.True(polygon.Shell.Coordinates[2] == new Coordinate(10.0, 10.0));
        Assert.True(polygon.Shell.Coordinates[3] == new Coordinate(0.0, 10.0));
        Assert.True(polygon.Shell.Coordinates[4] == new Coordinate(0.0, 0.0));
        Assert.True(polygon.Holes[0].CoordinatesCount == 5);
        Assert.True(polygon.Holes[0].IsRing);
        Assert.True(polygon.Holes[0].Coordinates[0] == new Coordinate(5.0, 5.0));
        Assert.True(polygon.Holes[0].Coordinates[1] == new Coordinate(7.0, 5.0));
        Assert.True(polygon.Holes[0].Coordinates[2] == new Coordinate(7.0, 7.0));
        Assert.True(polygon.Holes[0].Coordinates[3] == new Coordinate(5.0, 7.0));
        Assert.True(polygon.Holes[0].Coordinates[4] == new Coordinate(5.0, 5.0));
        Assert.True(polygon.Holes[1].CoordinatesCount == 5);
        Assert.True(polygon.Holes[1].IsRing);
        Assert.True(polygon.Holes[1].Coordinates[0] == new Coordinate(5.0, 1.0));
        Assert.True(polygon.Holes[1].Coordinates[1] == new Coordinate(7.0, 1.0));
        Assert.True(polygon.Holes[1].Coordinates[2] == new Coordinate(7.0, 3.0));
        Assert.True(polygon.Holes[1].Coordinates[3] == new Coordinate(5.0, 3.0));
        Assert.True(polygon.Holes[1].Coordinates[4] == new Coordinate(5.0, 1.0));
        ValidatePolygonToString(polygon, format);
        Assert.True(Polygon.IsValid(dataLines[18], format));
        polygon = Polygon.Parse(dataLines[18], format);
        Assert.True(polygon != null);
        Assert.True(polygon.IsEmpty);
        ValidatePolygonToString(polygon, format);
        Assert.True(Polygon.IsValid(dataLines[19], format));
        polygon = Polygon.Parse(dataLines[19], format);
        Assert.True(polygon != null);
        Assert.True(polygon.HolesCount == 0);
        Assert.True(polygon.Shell.CoordinatesCount == 4);
        Assert.True(polygon.Shell.IsRing);
        Assert.True(polygon.Shell.Coordinates[0] == new Coordinate(0.0, 0.0));
        Assert.True(polygon.Shell.Coordinates[1] == new Coordinate(10.0, 0.0));
        Assert.True(polygon.Shell.Coordinates[2] == new Coordinate(0.0, 10.0));
        Assert.True(polygon.Shell.Coordinates[3] == new Coordinate(0.0, 0.0));
        ValidatePolygonToString(polygon, format);
        for (int i = 20; i < 24; i++)
        {
          Assert.False(Polygon.IsValid(dataLines[i], format));
          polygon = Polygon.Parse(dataLines[i], format);
          Assert.True(polygon == null);
        }
      }

      // Test WKT
      dataLines = _wktDataLines;
      Assert.True(Polygon.IsValid(dataLines[16], GeometryAsTextFormatType.WKT));
      polygon = Polygon.Parse(dataLines[16], GeometryAsTextFormatType.WKT);
      Assert.True(polygon != null);
      Assert.True(polygon.HolesCount == 0);
      Assert.True(polygon.Shell.CoordinatesCount == 7);
      Assert.True(polygon.Shell.Coordinates[0] == new Coordinate(105.07547, 78.30689));
      Assert.True(polygon.Shell.Coordinates[1] == new Coordinate(99.43814, 77.921));
      Assert.True(polygon.Shell.Coordinates[2] == new Coordinate(101.2649, 79.23399));
      Assert.True(polygon.Shell.Coordinates[3] == new Coordinate(102.08635, 79.34641));
      Assert.True(polygon.Shell.Coordinates[4] == new Coordinate(102.837815, 79.28129));
      Assert.True(polygon.Shell.Coordinates[5] == new Coordinate(105.37243, 78.713340000000102));
      Assert.True(polygon.Shell.Coordinates[6] == new Coordinate(105.07547, 78.30689));
      ValidatePolygonToString(polygon, GeometryAsTextFormatType.WKT);
      Assert.True(Polygon.IsValid(dataLines[17], GeometryAsTextFormatType.WKT));
      polygon = Polygon.Parse(dataLines[17], GeometryAsTextFormatType.WKT);
      Assert.True(polygon != null);
      Assert.True(polygon.SRID == 30);
      Assert.True(polygon.HolesCount == 2);
      Assert.True(polygon.Shell.CoordinatesCount == 5);
      Assert.True(polygon.Shell.Coordinates[0] == new Coordinate(0.0, 0.0));
      Assert.True(polygon.Shell.Coordinates[1] == new Coordinate(10.0, 0.0));
      Assert.True(polygon.Shell.Coordinates[2] == new Coordinate(10.0, 10.0));
      Assert.True(polygon.Shell.Coordinates[3] == new Coordinate(0.0, 10.0));
      Assert.True(polygon.Shell.Coordinates[4] == new Coordinate(0.0, 0.0));
      Assert.True(polygon.Holes[0].CoordinatesCount == 5);
      Assert.True(polygon.Holes[0].Coordinates[0] == new Coordinate(5.0, 5.0));
      Assert.True(polygon.Holes[0].Coordinates[1] == new Coordinate(7.0, 5.0));
      Assert.True(polygon.Holes[0].Coordinates[2] == new Coordinate(7.0, 7.0));
      Assert.True(polygon.Holes[0].Coordinates[3] == new Coordinate(5.0, 7.0));
      Assert.True(polygon.Holes[0].Coordinates[4] == new Coordinate(5.0, 5.0));
      Assert.True(polygon.Holes[1].CoordinatesCount == 5);
      Assert.True(polygon.Holes[1].Coordinates[0] == new Coordinate(5.0, 1.0));
      Assert.True(polygon.Holes[1].Coordinates[1] == new Coordinate(7.0, 1.0));
      Assert.True(polygon.Holes[1].Coordinates[2] == new Coordinate(7.0, 3.0));
      Assert.True(polygon.Holes[1].Coordinates[3] == new Coordinate(5.0, 3.0));
      Assert.True(polygon.Holes[1].Coordinates[4] == new Coordinate(5.0, 1.0));
      ValidatePolygonToString(polygon, GeometryAsTextFormatType.WKT);
      for (int i = 18; i < 20; i++)
      {
        Assert.True(Polygon.IsValid(dataLines[i], GeometryAsTextFormatType.WKT));
        polygon = Polygon.Parse(dataLines[i], GeometryAsTextFormatType.WKT);
        Assert.True(polygon != null);
        Assert.True(polygon.IsEmpty);
        ValidatePolygonToString(polygon, GeometryAsTextFormatType.WKT);
      }

      Assert.True(Polygon.IsValid(dataLines[20], GeometryAsTextFormatType.WKT));
      polygon = Polygon.Parse(dataLines[20], GeometryAsTextFormatType.WKT);
      Assert.True(polygon != null);
      Assert.True(polygon.HolesCount == 0);
      Assert.True(polygon.Shell.CoordinatesCount == 5);
      Assert.True(polygon.Shell.Coordinates[0] == new Coordinate(0.0, 0.0));
      Assert.True(polygon.Shell.Coordinates[1] == new Coordinate(10.0, 0.0));
      Assert.True(polygon.Shell.Coordinates[2] == new Coordinate(10.0, 10.0));
      Assert.True(polygon.Shell.Coordinates[3] == new Coordinate(0.0, 10.0));
      Assert.True(polygon.Shell.Coordinates[4] == new Coordinate(0.0, 0.0));
      ValidatePolygonToString(polygon, GeometryAsTextFormatType.WKT);
      for (int i = 21; i < 24; i++)
      {
        Assert.False(Polygon.IsValid(dataLines[i], GeometryAsTextFormatType.WKT));
        polygon = Polygon.Parse(dataLines[i], GeometryAsTextFormatType.WKT);
        Assert.True(polygon == null);
      }
    }

    /// <summary>
    /// Reads test data from embedded resource files.
    /// </summary>
    private void ReadData()
    {
      // GeeJSON data
      var geoJsonData = Utilities.GetScriptFromResource(Assembly.GetAssembly(typeof(RegexTests)), "MySql.Utility.RegularExpressions.Tests.Properties.GeoJsonData.txt");
      Assert.True(!string.IsNullOrEmpty(geoJsonData));
      _geoJsonDataLines = geoJsonData.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
      Assert.True(_geoJsonDataLines.Length == 56);

      // GML data
      var gmlData = Utilities.GetScriptFromResource(Assembly.GetAssembly(typeof(RegexTests)), "MySql.Utility.RegularExpressions.Tests.Properties.GmlData.txt");
      Assert.True(!string.IsNullOrEmpty(gmlData));
      _gmlDataLines = gmlData.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
      Assert.True(_gmlDataLines.Length == 56);

      // KML data
      var kmlData = Utilities.GetScriptFromResource(Assembly.GetAssembly(typeof(RegexTests)), "MySql.Utility.RegularExpressions.Tests.Properties.KmlData.txt");
      Assert.True(!string.IsNullOrEmpty(kmlData));
      _kmlDataLines = kmlData.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
      Assert.True(_kmlDataLines.Length == 32);

      // WKT data
      var wktData = Utilities.GetScriptFromResource(Assembly.GetAssembly(typeof(RegexTests)), "MySql.Utility.RegularExpressions.Tests.Properties.WktData.txt");
      Assert.True(!string.IsNullOrEmpty(wktData));
      _wktDataLines = wktData.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
      Assert.True(_wktDataLines.Length == 56);
    }

    /// <summary>
    /// Validates that a <see cref="GeometryCollection"/> produces a valid string representation by parsing it again and comparing the original to the parsed copy.
    /// </summary>
    /// <param name="multiGeometry">A <see cref="GeometryCollection"/> instance.</param>
    /// <param name="format">A <see cref="GeometryAsTextFormatType"/> value.</param>
    private void ValidateGeometryCollectionToString(GeometryCollection multiGeometry, GeometryAsTextFormatType format)
    {
      if (multiGeometry == null || format == GeometryAsTextFormatType.None)
      {
        return;
      }

      var asText = multiGeometry.ToString(format);
      Assert.True(GeometryCollection.IsValid(asText, format));
      var parsedMultiGeometry = GeometryCollection.Parse(asText, format);
      Assert.True(parsedMultiGeometry != null);
      Assert.True(multiGeometry == parsedMultiGeometry);
    }

    /// <summary>
    /// Validates that a <see cref="LineString"/> produces a valid string representation by parsing it again and comparing the original to the parsed copy.
    /// </summary>
    /// <param name="lineString">A <see cref="LineString"/> instance.</param>
    /// <param name="format">A <see cref="GeometryAsTextFormatType"/> value.</param>
    private void ValidateLineStringToString(LineString lineString, GeometryAsTextFormatType format)
    {
      if (lineString == null || format == GeometryAsTextFormatType.None)
      {
        return;
      }

      var asText = lineString.ToString(format);
      Assert.True(LineString.IsValid(asText, format));
      var parsedLineString = LineString.Parse(asText, format);
      Assert.True(parsedLineString != null);
      Assert.True(lineString == parsedLineString);
    }

    /// <summary>
    /// Validates that a <see cref="MultiLineString"/> produces a valid string representation by parsing it again and comparing the original to the parsed copy.
    /// </summary>
    /// <param name="multiLineString">A <see cref="MultiLineString"/> instance.</param>
    /// <param name="format">A <see cref="GeometryAsTextFormatType"/> value.</param>
    private void ValidateMultiLineStringToString(MultiLineString multiLineString, GeometryAsTextFormatType format)
    {
      if (multiLineString == null || format == GeometryAsTextFormatType.None)
      {
        return;
      }

      var asText = multiLineString.ToString(format);
      Assert.True(MultiLineString.IsValid(asText, format));
      var parsedMultiLineString = MultiLineString.Parse(asText, format);
      Assert.True(parsedMultiLineString != null);
      Assert.True(multiLineString == parsedMultiLineString);
    }

    /// <summary>
    /// Validates that a <see cref="MultiPoint"/> produces a valid string representation by parsing it again and comparing the original to the parsed copy.
    /// </summary>
    /// <param name="multiPoint">A <see cref="MultiPoint"/> instance.</param>
    /// <param name="format">A <see cref="GeometryAsTextFormatType"/> value.</param>
    private void ValidateMultiPointToString(MultiPoint multiPoint, GeometryAsTextFormatType format)
    {
      if (multiPoint == null || format == GeometryAsTextFormatType.None)
      {
        return;
      }

      var asText = multiPoint.ToString(format);
      Assert.True(MultiPoint.IsValid(asText, format));
      var parsedMultiPoint = MultiPoint.Parse(asText, format);
      Assert.True(parsedMultiPoint != null);
      Assert.True(multiPoint == parsedMultiPoint);
    }

    /// <summary>
    /// Validates that a <see cref="MultiPolygon"/> produces a valid string representation by parsing it again and comparing the original to the parsed copy.
    /// </summary>
    /// <param name="multiPolygon">A <see cref="MultiPolygon"/> instance.</param>
    /// <param name="format">A <see cref="GeometryAsTextFormatType"/> value.</param>
    private void ValidateMultiPolygonToString(MultiPolygon multiPolygon, GeometryAsTextFormatType format)
    {
      if (multiPolygon == null || format == GeometryAsTextFormatType.None)
      {
        return;
      }

      var asText = multiPolygon.ToString(format);
      Assert.True(MultiPolygon.IsValid(asText, format));
      var parsedMultiPolygon = MultiPolygon.Parse(asText, format);
      Assert.True(parsedMultiPolygon != null);
      Assert.True(multiPolygon == parsedMultiPolygon);
    }

    /// <summary>
    /// Validates that a <see cref="Point"/> produces a valid string representation by parsing it again and comparing the original to the parsed copy.
    /// </summary>
    /// <param name="point">A <see cref="Point"/> instance.</param>
    /// <param name="format">A <see cref="GeometryAsTextFormatType"/> value.</param>
    private void ValidatePointToString(Point point, GeometryAsTextFormatType format)
    {
      if (point == null || format == GeometryAsTextFormatType.None)
      {
        return;
      }

      var asText = point.ToString(format);
      Assert.True(Point.IsValid(asText, format));
      var parsedPoint = Point.Parse(asText, format);
      Assert.True(parsedPoint != null);
      Assert.True(point == parsedPoint);
    }

    /// <summary>
    /// Validates that a <see cref="Polygon"/> produces a valid string representation by parsing it again and comparing the original to the parsed copy.
    /// </summary>
    /// <param name="polygon">A <see cref="Polygon"/> instance.</param>
    /// <param name="format">A <see cref="GeometryAsTextFormatType"/> value.</param>
    private void ValidatePolygonToString(Polygon polygon, GeometryAsTextFormatType format)
    {
      if (polygon == null || format == GeometryAsTextFormatType.None)
      {
        return;
      }

      var asText = polygon.ToString(format);
      Assert.True(Polygon.IsValid(asText, format));
      var parsedPolygon = Polygon.Parse(asText, format);
      Assert.True(parsedPolygon != null);
      Assert.True(polygon == parsedPolygon);
    }
  }
}
