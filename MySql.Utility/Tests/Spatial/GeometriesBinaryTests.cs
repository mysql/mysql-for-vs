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
  /// Tests for the geometry classes binary representations.
  /// </summary>
  public class GeometriesBinaryTests
  {
    #region Fields

    /// <summary>
    /// Holds lines of data in WKT format.
    /// </summary>
    private string[] _wktDataLines;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of the <see cref="GeometriesBinaryTests"/> class.
    /// </summary>
    public GeometriesBinaryTests()
    {
      _wktDataLines = null;
    }

    /// <summary>
    /// Performs unit tests on the <see cref="GeometryCollection"/> class.
    /// </summary>
    [Fact]
    public void TestGeometryCollection()
    {
      // Read data files
      ReadData();

      for(int i = 48; i < 53; i++)
      {
        // Parse text data
        Assert.True(GeometryCollection.IsValid(_wktDataLines[i], GeometryAsTextFormatType.WKT));
        var multiGeometryOriginal = GeometryCollection.Parse(_wktDataLines[i], GeometryAsTextFormatType.WKT);
        Assert.True(multiGeometryOriginal != null);

        // Clone the geometry
        var clonedGeometry = multiGeometryOriginal.Clone() as GeometryCollection;
        Assert.True(clonedGeometry != null);

        // Compare the geometries
        Assert.True(multiGeometryOriginal == clonedGeometry);

        foreach (ByteOrderType byteOrder in Enum.GetValues(typeof(ByteOrderType)))
        {
          // Convert to binary
          var binaryData = WkbHandler.GetBinaryWkbFromGeometry(multiGeometryOriginal, byteOrder);
          Assert.True(binaryData != null);

          // Convert back to geometry
          var geometryConverted = WkbHandler.GetGeometryFromBinaryWkb(binaryData) as GeometryCollection;
          Assert.True(geometryConverted != null);

          // Compare the geometries
          Assert.True(multiGeometryOriginal == geometryConverted);

          // Convert to hexadecimal
          var hexData = WkbHandler.GetHexWkbFromGeometry(multiGeometryOriginal, byteOrder);
          Assert.True(!string.IsNullOrEmpty(hexData));

          // Convert back to geometry
          geometryConverted = WkbHandler.GetGeometryFromHexWkb(hexData) as GeometryCollection;
          Assert.True(geometryConverted != null);

          // Compare the geometries
          Assert.True(multiGeometryOriginal == geometryConverted);
        }
      }
    }

    /// <summary>
    /// Performs unit tests on the <see cref="LineString"/> class.
    /// </summary>
    [Fact]
    public void TestLineString()
    {
      // Read data files
      ReadData();

      for (int i = 8; i < 13; i++)
      {
        // Parse text data
        Assert.True(LineString.IsValid(_wktDataLines[i], GeometryAsTextFormatType.WKT));
        var lineStringOriginal = LineString.Parse(_wktDataLines[i], GeometryAsTextFormatType.WKT);
        Assert.True(lineStringOriginal != null);

        // Clone the geometry
        var clonedGeometry = lineStringOriginal.Clone() as LineString;
        Assert.True(clonedGeometry != null);

        // Compare the geometries
        Assert.True(lineStringOriginal == clonedGeometry);

        foreach (ByteOrderType byteOrder in Enum.GetValues(typeof(ByteOrderType)))
        {
          // Convert to binary
          var binaryData = WkbHandler.GetBinaryWkbFromGeometry(lineStringOriginal, byteOrder);
          Assert.True(binaryData != null);

          // Convert back to geometry
          var geometryConverted = WkbHandler.GetGeometryFromBinaryWkb(binaryData) as LineString;
          Assert.True(geometryConverted != null);

          // Compare the geometries
          Assert.True(lineStringOriginal == geometryConverted);

          // Convert to hexadecimal
          var hexData = WkbHandler.GetHexWkbFromGeometry(lineStringOriginal, byteOrder);
          Assert.True(!string.IsNullOrEmpty(hexData));

          // Convert back to geometry
          geometryConverted = WkbHandler.GetGeometryFromHexWkb(hexData) as LineString;
          Assert.True(geometryConverted != null);

          // Compare the geometries
          Assert.True(lineStringOriginal == geometryConverted);
        }
      }
    }

    /// <summary>
    /// Performs unit tests on the <see cref="MultiLineString"/> class.
    /// </summary>
    [Fact]
    public void TestMultiLineString()
    {
      // Read data files
      ReadData();

      for (int i = 32; i < 37; i++)
      {
        // Parse text data
        Assert.True(MultiLineString.IsValid(_wktDataLines[i], GeometryAsTextFormatType.WKT));
        var multiLineStringOriginal = MultiLineString.Parse(_wktDataLines[i], GeometryAsTextFormatType.WKT);
        Assert.True(multiLineStringOriginal != null);

        // Clone the geometry
        var clonedGeometry = multiLineStringOriginal.Clone() as MultiLineString;
        Assert.True(clonedGeometry != null);

        // Compare the geometries
        Assert.True(multiLineStringOriginal == clonedGeometry);

        foreach (ByteOrderType byteOrder in Enum.GetValues(typeof(ByteOrderType)))
        {
          // Convert to binary
          var binaryData = WkbHandler.GetBinaryWkbFromGeometry(multiLineStringOriginal, byteOrder);
          Assert.True(binaryData != null);

          // Convert back to geometry
          var geometryConverted = WkbHandler.GetGeometryFromBinaryWkb(binaryData) as MultiLineString;
          Assert.True(geometryConverted != null);

          // Compare the geometries
          Assert.True(multiLineStringOriginal == geometryConverted);

          // Convert to hexadecimal
          var hexData = WkbHandler.GetHexWkbFromGeometry(multiLineStringOriginal, byteOrder);
          Assert.True(!string.IsNullOrEmpty(hexData));

          // Convert back to geometry
          geometryConverted = WkbHandler.GetGeometryFromHexWkb(hexData) as MultiLineString;
          Assert.True(geometryConverted != null);

          // Compare the geometries
          Assert.True(multiLineStringOriginal == geometryConverted);
        }
      }
    }

    /// <summary>
    /// Performs unit tests on the <see cref="MultiPoint"/> class.
    /// </summary>
    [Fact]
    public void TestMultiPoint()
    {
      // Read data files
      ReadData();

      for (int i = 24; i < 29; i++)
      {
        // Parse text data
        Assert.True(MultiPoint.IsValid(_wktDataLines[i], GeometryAsTextFormatType.WKT));
        var multiPointOriginal = MultiPoint.Parse(_wktDataLines[i], GeometryAsTextFormatType.WKT);
        Assert.True(multiPointOriginal != null);

        // Clone the geometry
        var clonedGeometry = multiPointOriginal.Clone() as MultiPoint;
        Assert.True(clonedGeometry != null);

        // Compare the geometries
        Assert.True(multiPointOriginal == clonedGeometry);

        foreach (ByteOrderType byteOrder in Enum.GetValues(typeof(ByteOrderType)))
        {
          // Convert to binary
          var binaryData = WkbHandler.GetBinaryWkbFromGeometry(multiPointOriginal, byteOrder);
          Assert.True(binaryData != null);

          // Convert back to geometry
          var geometryConverted = WkbHandler.GetGeometryFromBinaryWkb(binaryData) as MultiPoint;
          Assert.True(geometryConverted != null);

          // Compare the geometries
          Assert.True(multiPointOriginal == geometryConverted);

          // Convert to hexadecimal
          var hexData = WkbHandler.GetHexWkbFromGeometry(multiPointOriginal, byteOrder);
          Assert.True(!string.IsNullOrEmpty(hexData));

          // Convert back to geometry
          geometryConverted = WkbHandler.GetGeometryFromHexWkb(hexData) as MultiPoint;
          Assert.True(geometryConverted != null);

          // Compare the geometries
          Assert.True(multiPointOriginal == geometryConverted);
        }
      }
    }

    /// <summary>
    /// Performs unit tests on the <see cref="MultiPolygon"/> class.
    /// </summary>
    [Fact]
    public void TestMultiPolygon()
    {
      // Read data files
      ReadData();

      for (int i = 40; i < 45; i++)
      {
        // Parse text data
        Assert.True(MultiPolygon.IsValid(_wktDataLines[i], GeometryAsTextFormatType.WKT));
        var multiPolygonOriginal = MultiPolygon.Parse(_wktDataLines[i], GeometryAsTextFormatType.WKT);
        Assert.True(multiPolygonOriginal != null);

        // Clone the geometry
        var clonedGeometry = multiPolygonOriginal.Clone() as MultiPolygon;
        Assert.True(clonedGeometry != null);

        // Compare the geometries
        Assert.True(multiPolygonOriginal == clonedGeometry);

        foreach (ByteOrderType byteOrder in Enum.GetValues(typeof(ByteOrderType)))
        {
          // Convert to binary
          var binaryData = WkbHandler.GetBinaryWkbFromGeometry(multiPolygonOriginal, byteOrder);
          Assert.True(binaryData != null);

          // Convert back to geometry
          var geometryConverted = WkbHandler.GetGeometryFromBinaryWkb(binaryData) as MultiPolygon;
          Assert.True(geometryConverted != null);

          // Compare the geometries
          Assert.True(multiPolygonOriginal == geometryConverted);

          // Convert to hexadecimal
          var hexData = WkbHandler.GetHexWkbFromGeometry(multiPolygonOriginal, byteOrder);
          Assert.True(!string.IsNullOrEmpty(hexData));

          // Convert back to geometry
          geometryConverted = WkbHandler.GetGeometryFromHexWkb(hexData) as MultiPolygon;
          Assert.True(geometryConverted != null);

          // Compare the geometries
          Assert.True(multiPolygonOriginal == geometryConverted);
        }
      }
    }

    /// <summary>
    /// Performs unit tests on the <see cref="Point"/> class.
    /// </summary>
    [Fact]
    public void TestPoint()
    {
      // Read data files
      ReadData();

      for (int i = 0; i < 5; i++)
      {
        // Parse text data
        Assert.True(Point.IsValid(_wktDataLines[i], GeometryAsTextFormatType.WKT));
        var pointOriginal = Point.Parse(_wktDataLines[i], GeometryAsTextFormatType.WKT);
        Assert.True(pointOriginal != null);

        // Clone the geometry
        var clonedGeometry = pointOriginal.Clone() as Point;
        Assert.True(clonedGeometry != null);

        // Compare the geometries
        Assert.True(pointOriginal == clonedGeometry);

        foreach (ByteOrderType byteOrder in Enum.GetValues(typeof(ByteOrderType)))
        {
          // Convert to binary
          var binaryData = WkbHandler.GetBinaryWkbFromGeometry(pointOriginal, byteOrder);
          Assert.True(binaryData != null);

          // Convert back to geometry
          var geometryConverted = WkbHandler.GetGeometryFromBinaryWkb(binaryData) as Point;
          Assert.True(geometryConverted != null);

          // Compare the geometries
          Assert.True(pointOriginal == geometryConverted);

          // Convert to hexadecimal
          var hexData = WkbHandler.GetHexWkbFromGeometry(pointOriginal, byteOrder);
          Assert.True(!string.IsNullOrEmpty(hexData));

          // Convert back to geometry
          geometryConverted = WkbHandler.GetGeometryFromHexWkb(hexData) as Point;
          Assert.True(geometryConverted != null);

          // Compare the geometries
          Assert.True(pointOriginal == geometryConverted);
        }
      }
    }

    /// <summary>
    /// Performs unit tests on the <see cref="Polygon"/> class.
    /// </summary>
    [Fact]
    public void TestPolygon()
    {
      // Read data files
      ReadData();

      for (int i = 16; i < 21; i++)
      {
        // Parse text data
        Assert.True(Polygon.IsValid(_wktDataLines[i], GeometryAsTextFormatType.WKT));
        var polygonOriginal = Polygon.Parse(_wktDataLines[i], GeometryAsTextFormatType.WKT);
        Assert.True(polygonOriginal != null);

        // Clone the geometry
        var clonedGeometry = polygonOriginal.Clone() as Polygon;
        Assert.True(clonedGeometry != null);

        // Compare the geometries
        Assert.True(polygonOriginal == clonedGeometry);

        foreach (ByteOrderType byteOrder in Enum.GetValues(typeof(ByteOrderType)))
        {
          // Convert to binary
          var binaryData = WkbHandler.GetBinaryWkbFromGeometry(polygonOriginal, byteOrder);
          Assert.True(binaryData != null);

          // Convert back to geometry
          var geometryConverted = WkbHandler.GetGeometryFromBinaryWkb(binaryData) as Polygon;
          Assert.True(geometryConverted != null);

          // Compare the geometries
          Assert.True(polygonOriginal == geometryConverted);

          // Convert to hexadecimal
          var hexData = WkbHandler.GetHexWkbFromGeometry(polygonOriginal, byteOrder);
          Assert.True(!string.IsNullOrEmpty(hexData));

          // Convert back to geometry
          geometryConverted = WkbHandler.GetGeometryFromHexWkb(hexData) as Polygon;
          Assert.True(geometryConverted != null);

          // Compare the geometries
          Assert.True(polygonOriginal == geometryConverted);
        }
      }
    }

    /// <summary>
    /// Reads test data from embedded resource files.
    /// </summary>
    private void ReadData()
    {
      var wktData = Utilities.GetScriptFromResource(Assembly.GetAssembly(typeof(RegexTests)), "MySql.Utility.RegularExpressions.Tests.Properties.WktData.txt");
      Assert.True(!string.IsNullOrEmpty(wktData));
      _wktDataLines = wktData.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
      Assert.True(_wktDataLines.Length == 56);
    }
  }
}
