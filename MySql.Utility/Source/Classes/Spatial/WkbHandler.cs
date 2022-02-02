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
using System.IO;
using MySql.Utility.Enums;

namespace MySql.Utility.Classes.Spatial
{
  /// <summary>
  /// Contains functionality to process a byte array in WKB format.
  /// </summary>
  public class WkbHandler : IDisposable
  {
    #region Fields

    /// <summary>
    /// The <see cref="Stream"/> used to read or write WKB data.
    /// </summary>
    private readonly Stream _stream;

    /// <summary>
    /// The <see cref="ByteOrderType"/> used to encode the binary data.
    /// </summary>
    private ByteOrderType _byteOrder;

    /// <summary>
    /// Flag indicating whether the <seealso cref="Dispose"/> method has already been called.
    /// </summary>
    private bool _disposed;

    /// <summary>
    /// Gets the SRID if present in the encoded geometry.
    /// </summary>
    private int _srid;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of the <see cref="WkbHandler"/> class.
    /// </summary>
    /// <param name="stream">The <see cref="Stream"/> used to read or write WKB data.</param>
    public WkbHandler(Stream stream)
      : this()
    {
      if (stream == null)
      {
        throw new ArgumentNullException(nameof(stream));
      }

      _stream = stream;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WkbHandler"/> class.
    /// </summary>
    /// <remarks>The handler is meant to be used only to read and not to write since it is initialized with a <see cref="byte"/> array.</remarks>
    /// <param name="bytes">A <see cref="byte"/> array to read data from.</param>
    public WkbHandler(byte[] bytes)
      : this()
    {
      _stream = new MemoryStream(bytes, false);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WkbHandler"/> class.
    /// </summary>
    private WkbHandler()
    {
      _disposed = false;
      _byteOrder = DefaultByteOrder;
      _srid = 0;
      ErrorMessage = null;
    }

    #region Properties

    /// <summary>
    /// Gets a default <see cref="ByteOrderType"/> based on the "endianness" of this computer architecture.
    /// </summary>
    public static ByteOrderType DefaultByteOrder
    {
      get
      {
        return BitConverter.IsLittleEndian
          ? ByteOrderType.NetworkDataRepresentation
          : ByteOrderType.ExternalDataRepresentation;
      }
    }

    /// <summary>
    /// Gets an error message (if any) related to the last operation performed.
    /// </summary>
    public string ErrorMessage { get; private set; }

    #endregion Properties

    /// <summary>
    /// Returns a byte array representing a <see cref="Geometry"/> encoded in WKB format.
    /// </summary>
    /// <param name="geometry">A <see cref="Geometry"/> to be encoded in WKB format.</param>
    /// <param name="byteOrder">The <see cref="ByteOrderType"/> used to encode the binary data.</param>
    /// <returns>A byte array representing a <see cref="Geometry"/> encoded in WKB format.</returns>
    public static byte[] GetBinaryWkbFromGeometry(Geometry geometry, ByteOrderType byteOrder)
    {
      string errorMessage;
      return GetBinaryWkbFromGeometry(geometry, byteOrder, out errorMessage);
    }

    /// <summary>
    /// Returns a byte array representing a <see cref="Geometry"/> encoded in WKB format.
    /// </summary>
    /// <param name="geometry">A <see cref="Geometry"/> to be encoded in WKB format.</param>
    /// <param name="byteOrder">The <see cref="ByteOrderType"/> used to encode the binary data.</param>
    /// <param name="errorMessage">The error message (if any) related to getting the binary array.</param>
    /// <returns>A byte array representing a <see cref="Geometry"/> encoded in WKB format.</returns>
    public static byte[] GetBinaryWkbFromGeometry(Geometry geometry, ByteOrderType byteOrder, out string errorMessage)
    {
      errorMessage = null;
      if (geometry == null)
      {
        return null;
      }

      using (var memStream = new MemoryStream())
      {
        using (var wkbHandler = new WkbHandler(memStream))
        {
          wkbHandler.Write(geometry, byteOrder);
          errorMessage = wkbHandler.ErrorMessage;
          return memStream.ToArray();
        }
      }
    }

    /// <summary>
    /// Returns a <see cref="Geometry"/> decoded from a byte array encoded in WKB format.
    /// </summary>
    /// <param name="wkbData">A binary array containing a geometry encoded in WKB format.</param>
    /// <returns>A <see cref="Geometry"/> decoded from a byte array encoded in WKB format.</returns>
    public static Geometry GetGeometryFromBinaryWkb(byte[] wkbData)
    {
      string errorMessage;
      return GetGeometryFromBinaryWkb(wkbData, out errorMessage);
    }

    /// <summary>
    /// Returns a <see cref="Geometry"/> decoded from a byte array encoded in WKB format.
    /// </summary>
    /// <param name="wkbData">A binary array containing a geometry encoded in WKB format.</param>
    /// <param name="errorMessage">The error message (if any) related to getting the <see cref="Geometry"/>.</param>
    /// <returns>A <see cref="Geometry"/> decoded from a byte array encoded in WKB format.</returns>
    public static Geometry GetGeometryFromBinaryWkb(byte[] wkbData, out string errorMessage)
    {
      errorMessage = null;
      if (wkbData == null)
      {
        return null;
      }

      using (var wkbHandler = new WkbHandler(wkbData))
      {
        var geom = wkbHandler.Read();
        errorMessage = wkbHandler.ErrorMessage;
        return geom;
      }
    }

    /// <summary>
    /// Returns a <see cref="Geometry"/> decoded from a hexadecimal string encoded in WKB format.
    /// </summary>
    /// <param name="wkbData">A binary array containing a geometry encoded in WKB format.</param>
    /// <returns>A <see cref="Geometry"/> decoded from a hexadecimal string encoded in WKB format.</returns>
    public static Geometry GetGeometryFromHexWkb(string wkbData)
    {
      string errorMessage;
      return GetGeometryFromHexWkb(wkbData, out errorMessage);
    }

    /// <summary>
    /// Returns a <see cref="Geometry"/> decoded from an hexadecimal string encoded in WKB format.
    /// </summary>
    /// <param name="wkbData">A binary array containing a geometry encoded in WKB format.</param>
    /// <param name="errorMessage">The error message (if any) related to getting the <see cref="Geometry"/>.</param>
    /// <returns>A <see cref="Geometry"/> decoded from a hexadecimal string encoded in WKB format.</returns>
    public static Geometry GetGeometryFromHexWkb(string wkbData, out string errorMessage)
    {
      return GetGeometryFromBinaryWkb(wkbData.ToByteArray(), out errorMessage);
    }

    /// <summary>
    /// Returns a hexadecimal string representing a <see cref="Geometry"/> encoded in WKB format.
    /// </summary>
    /// <param name="geometry">A <see cref="Geometry"/> to be encoded in WKB format.</param>
    /// <param name="byteOrder">The <see cref="ByteOrderType"/> used to encode the binary data.</param>
    /// <returns>A hexadecimal string representing a <see cref="Geometry"/> encoded in WKB format.</returns>
    public static string GetHexWkbFromGeometry(Geometry geometry, ByteOrderType byteOrder)
    {
      string errorMessage;
      return GetHexWkbFromGeometry(geometry, byteOrder, out errorMessage);
    }

    /// <summary>
    /// Returns a hexadecimal string representing a <see cref="Geometry"/> encoded in WKB format.
    /// </summary>
    /// <param name="geometry">A <see cref="Geometry"/> to be encoded in WKB format.</param>
    /// <param name="byteOrder">The <see cref="ByteOrderType"/> used to encode the binary data.</param>
    /// <param name="errorMessage">The error message (if any) related to getting the binary array.</param>
    /// <returns>A hexadecimal string representing a <see cref="Geometry"/> encoded in WKB format.</returns>
    public static string GetHexWkbFromGeometry(Geometry geometry, ByteOrderType byteOrder, out string errorMessage)
    {
      var binaryWkb = GetBinaryWkbFromGeometry(geometry, byteOrder, out errorMessage);
      return binaryWkb.ToHexadecimal();
    }

    /// <summary>
    /// Releases all resources used by the <see cref="WkbHandler"/> class
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Reads a single <see cref="Geometry"/> encoded in WKB format.
    /// </summary>
    /// <returns>A <see cref="Geometry"/> instance.</returns>
    public Geometry Read()
    {
      if (_stream == null || !_stream.CanRead)
      {
        return null;
      }

      try
      {
        _srid = 0;
        ErrorMessage = null;
        using (var reader = new BinaryReader(_stream))
        {
          return ReadGeometry(reader, true);
        }
      }
      catch (EndOfStreamException eosEx)
      {
        ErrorMessage = string.Format(Resources.UnexpectedEndOfStreamError, eosEx.Message);
      }
      catch (ObjectDisposedException odEx)
      {
        ErrorMessage = string.Format(Resources.UnexpectedStreamClosingError, odEx.Message);
      }
      catch (IOException ioEx)
      {
        ErrorMessage = string.Format(Resources.UnknownIOError, ioEx.Message);
      }
      catch (Exception ex)
      {
        ErrorMessage = ex.Message;
      }

      return null;
    }

    /// <summary>
    /// Writes a <see cref="Geometry"/> in WKB format.
    /// </summary>
    /// <param name="geometry">A <see cref="Geometry"/> instance.</param>
    /// <param name="byteOrder">The <see cref="ByteOrderType"/> used to encode the binary data.</param>
    public void Write(Geometry geometry, ByteOrderType byteOrder)
    {
      if (_stream == null || !_stream.CanWrite)
      {
        return;
      }

      try
      {
        ErrorMessage = null;
        using (var writer = new BinaryWriter(_stream))
        {
          _srid = geometry.SRID;
          _byteOrder = byteOrder;
          WriteGeometry(writer, geometry, true);
        }
      }
      catch (ObjectDisposedException odEx)
      {
        ErrorMessage = string.Format(Resources.UnexpectedStreamClosingError, odEx.Message);
      }
      catch (IOException ioEx)
      {
        ErrorMessage = string.Format(Resources.UnknownIOError, ioEx.Message);
      }
      catch (Exception ex)
      {
        ErrorMessage = ex.Message;
      }
    }

    /// <summary>
    /// Releases all resources used by the <see cref="WkbHandler"/> class
    /// </summary>
    /// <param name="disposing">If true this is called by <see cref="Dispose"/>, otherwise it is called by the finalizer</param>
    protected virtual void Dispose(bool disposing)
    {
      if (_disposed)
      {
        return;
      }

      // Free managed resources
      if (disposing)
      {
        if (_stream != null)
        {
          _stream.Dispose();
        }
      }

      // Add class finalizer if unmanaged resources are added to the class
      // Free unmanaged resources if there are any
      _disposed = true;
    }

    /// <summary>
		/// Reads a list of <see cref="Coordinate"/>s rom the WKB.
		/// </summary>
		/// <param name="reader">The <see cref="BinaryReader"/> used to read the data from.</param>
		/// <returns>A list of <see cref="Coordinate"/>s.</returns>
    private List<Coordinate> ReadCoordinates(BinaryReader reader)
    {
      if (reader == null)
      {
        return null;
      }

      // Get the number of coordinates, presumably in this curve.
      int coordinatesCount = (int)reader.ReadUIntUsingByteOrder(_byteOrder);
      if (coordinatesCount == 0)
      {
        return null;
      }

      // Create an empty list of coordinates
      var coordinatesList = new List<Coordinate>(coordinatesCount);

      // Fill in the list
      for (int i = 0; i < coordinatesCount; i++)
      {
        var point = ReadPoint(reader);
        if (point == null)
        {
          continue;
        }

        coordinatesList.Add(point.Coordinate);
      }

      return coordinatesList;
    }

    /// <summary>
    /// Reads a single <see cref="Geometry"/> in WKB format from a <see cref="BinaryReader"/>.
    /// </summary>
    /// <param name="reader">The <see cref="BinaryReader"/> used to read the data from.</param>
    /// <param name="readSrid">Flag indicating whether the first 4 bytes are assumed to be the SRID.</param>
    /// <returns>A single <see cref="Geometry"/> in WKB format from a <see cref="BinaryReader"/>.</returns>
    private Geometry ReadGeometry(BinaryReader reader, bool readSrid)
    {
      if (reader == null)
      {
        return null;
      }

      // First 4 bytes of the WKB represent the SRID, read them with default byte order.
      if (readSrid)
      {
        _srid = (int)reader.ReadUIntUsingByteOrder(_byteOrder);
      }

      // Next 1 byte of the WKB represent the byte order used to encode the binary data
      // The byte order is either 1 or 0 to indicate little-endian (NDR) or big-endian (XDR) storage.
      var byteOrderNumeric = reader.ReadByte();
      ByteOrderType byteOrderFromWkb;
      if (Enum.TryParse(byteOrderNumeric.ToString(), out byteOrderFromWkb))
      {
        if (_byteOrder != byteOrderFromWkb)
        {
          _srid = _srid.ReverseBytes();
        }

        _byteOrder = byteOrderFromWkb;
      }

      // Next 4 bytes of the WKB represent the geometry type
      // Values from 1 through 7 indicate Point, LineString, Polygon, MultiPoint, MultiLineString, MultiPolygon, and GeometryCollection.
      var typeGeom = (int)reader.ReadUIntUsingByteOrder(_byteOrder);
      GeometryType geometryType;
      if (!Enum.TryParse(typeGeom.ToString(), out geometryType))
      {
        throw new Exception("Unknown geometry type.");
      }

      // The rest of the bytes represent the geometry data
      Geometry geom = null;
      switch (geometryType)
      {
        case GeometryType.Point:
          geom = ReadPoint(reader);
          break;

        case GeometryType.LineString:
          geom = ReadLineString(reader);
          break;

        case GeometryType.Polygon:
          geom = ReadPolygon(reader);
          break;

        case GeometryType.MultiPoint:
          geom = ReadMultiPoint(reader);
          break;

        case GeometryType.MultiLineString:
          geom = ReadMultiLineString(reader);
          break;

        case GeometryType.MultiPolygon:
          geom = ReadMultiPolygon(reader);
          break;

        case GeometryType.GeometryCollection:
          geom = ReadGeometryCollection(reader);
          break;
      }

      return geom;
    }

    /// <summary>
		/// Reads a <see cref="GeometryCollection"/> geometry from the WKB.
		/// </summary>
		/// <param name="reader">The <see cref="BinaryReader"/> used to read the data from.</param>
		/// <returns>A <see cref="GeometryCollection"/> geometry.</returns>
		private GeometryCollection ReadGeometryCollection(BinaryReader reader)
    {
      if (reader == null)
      {
        return null;
      }

      // Get the number of Polygons.
      int geomsCount = (int)reader.ReadUIntUsingByteOrder(_byteOrder);

      // Create a new list for the geometries.
      var geomsList = new List<Geometry>(geomsCount);

      // Read the Geometries and fill them in.
      for (int i = 0; i < geomsCount; i++)
      {
        var geom = ReadGeometry(reader, false);
        geomsList.Add(geom);
      }

      // Create and return the GeometryCollection.
      return new GeometryCollection(geomsList, _srid);
    }

    /// <summary>
		/// Reads a <see cref="LineString"/> geometry from the WKB.
		/// </summary>
		/// <param name="reader">The <see cref="BinaryReader"/> used to read the data from.</param>
		/// <returns>A <see cref="LineString"/> geometry.</returns>
		private LineString ReadLineString(BinaryReader reader)
    {
      if (reader == null)
      {
        return null;
      }

      var coordinatesList = ReadCoordinates(reader);
      return new LineString(coordinatesList, _srid);
    }

    /// <summary>
		/// Reads a <see cref="MultiLineString"/> geometry from the WKB.
		/// </summary>
		/// <param name="reader">The <see cref="BinaryReader"/> used to read the data from.</param>
		/// <returns>A <see cref="MultiLineString"/> geometry.</returns>
		private MultiLineString ReadMultiLineString(BinaryReader reader)
    {
      if (reader == null)
      {
        return null;
      }

      // Get the number of LineStrings.
      int lineStringsCount = (int)reader.ReadUIntUsingByteOrder(_byteOrder);

      // Create a new list for the LineStrings.
      var lineStringsList = new List<LineString>(lineStringsCount);

      // Read the LineStrings and fill them in.
      for (int i = 0; i < lineStringsCount; i++)
      {
        var lineString = ReadGeometry(reader, false) as LineString;
        if (lineString == null)
        {
          throw new Exception(string.Format(Resources.InvalidGeometryParsedError, typeof(MultiLineString)));
        }

        lineStringsList.Add(lineString);
      }

      // Create and return the MultiLineString.
      return new MultiLineString(lineStringsList, _srid);
    }

    /// <summary>
		/// Reads a <see cref="MultiPoint"/> geometry from the WKB.
		/// </summary>
		/// <param name="reader">The <see cref="BinaryReader"/> used to read the data from.</param>
		/// <returns>A <see cref="MultiPoint"/> geometry.</returns>
		private MultiPoint ReadMultiPoint(BinaryReader reader)
    {
      if (reader == null)
      {
        return null;
      }

      // Get the number of points.
      int pointsCount = (int)reader.ReadUIntUsingByteOrder(_byteOrder);

      // Create a new list for the points.
      var points = new List<Point>(pointsCount);

      // Read the points and fill them in.
      for (int i = 0; i < pointsCount; i++)
      {
        var point = ReadGeometry(reader, false) as Point;
        if (point == null)
        {
          throw new Exception(string.Format(Resources.InvalidGeometryParsedError, typeof(MultiPoint)));
        }

        points.Add(point);
      }

      // Create and return the MultiPoint.
      return new MultiPoint(points, _srid);
    }

    /// <summary>
		/// Reads a <see cref="MultiPolygon"/> geometry from the WKB.
		/// </summary>
		/// <param name="reader">The <see cref="BinaryReader"/> used to read the data from.</param>
		/// <returns>A <see cref="MultiPolygon"/> geometry.</returns>
		private MultiPolygon ReadMultiPolygon(BinaryReader reader)
    {
      if (reader == null)
      {
        return null;
      }

      // Get the number of Polygons.
      int polygonsCount = (int)reader.ReadUIntUsingByteOrder(_byteOrder);

      // Create a new list for the Polygons.
      var polygonsList = new List<Polygon>(polygonsCount);

      // Read the LineStrings and fill them in.
      for (int i = 0; i < polygonsCount; i++)
      {
        var polygon = ReadGeometry(reader, false) as Polygon;
        if (polygon == null)
        {
          throw new Exception(string.Format(Resources.InvalidGeometryParsedError, typeof(MultiPolygon)));
        }

        polygonsList.Add(polygon);
      }

      // Create and return the MultiLineString.
      return new MultiPolygon(polygonsList, _srid);
    }

    /// <summary>
		/// Reads a <see cref="Point"/> geometry from the WKB.
		/// </summary>
		/// <param name="reader">The <see cref="BinaryReader"/> used to read the data from.</param>
		/// <returns>A <see cref="Point"/> geometry.</returns>
		private Point ReadPoint(BinaryReader reader)
    {
      if (reader == null)
      {
        return null;
      }

      if (reader.BaseStream.Position >= reader.BaseStream.Length)
      {
        return new Point(_srid);
      }

      var x = reader.ReadDoubleUsingByteOrder(_byteOrder);
      var y = reader.ReadDoubleUsingByteOrder(_byteOrder);
      return double.IsNaN(x) && double.IsNaN(y)
        ? new Point(_srid)
        : new Point(new Coordinate(x, y), _srid);
    }

    /// <summary>
		/// Reads a <see cref="Polygon"/> geometry from the WKB.
		/// </summary>
		/// <param name="reader">The <see cref="BinaryReader"/> used to read the data from.</param>
		/// <returns>A <see cref="Polygon"/> geometry.</returns>
		private Polygon ReadPolygon(BinaryReader reader)
    {
      if (reader == null)
      {
        return null;
      }

      // Get the number of rings in the polygon.
      int ringsCount = (int)reader.ReadUIntUsingByteOrder(_byteOrder);

      if (ringsCount == 0)
      {
        return new Polygon(_srid);
      }

      // Read shell coordinates
      var coordinates = ReadCoordinates(reader);
      var shell = new LinearRing(coordinates, _srid);

      // Read holes coordinates
      List<LinearRing> holes = null;
      if (ringsCount > 1)
      {
        holes = new List<LinearRing>(ringsCount - 1);
        for (int holeIdx = 1; holeIdx < ringsCount; holeIdx++)
        {
          coordinates = ReadCoordinates(reader);
          holes.Add(new LinearRing(coordinates, _srid));
        }
      }

      return new Polygon(shell, holes, _srid);
    }

    /// <summary>
    /// Writes a <see cref="Coordinate"/> to the given <see cref="BinaryWriter"/>.
    /// </summary>
    /// <param name="writer">The <see cref="BinaryWriter"/> used to write the data to.</param>
    /// <param name="coordinate">A <see cref="Coordinate"/> instance.</param>
    private void WriteCoordinate(BinaryWriter writer, Coordinate coordinate)
    {
      if (writer == null || coordinate == null)
      {
        return;
      }

      writer.WriteDoubleUsingByteOrder(coordinate.X, _byteOrder);
      writer.WriteDoubleUsingByteOrder(coordinate.Y, _byteOrder);
    }

    /// <summary>
    /// Writes a <see cref="Geometry"/> in WKB format.
    /// </summary>
    /// <param name="writer">The <see cref="BinaryWriter"/> used to write the data to.</param>
    /// <param name="geometry">A <see cref="Geometry"/> instance.</param>
    /// <param name="writeSrid">Flag indicating whether 4 bytes are written at the start of the data to encode the SRID.</param>
    private void WriteGeometry(BinaryWriter writer, Geometry geometry, bool writeSrid)
    {
      if (writer == null || geometry == null)
      {
        return;
      }

      if (writeSrid)
      // First 4 bytes of the WKB represent the SRID
      {
        writer.WriteUIntUsingByteOrder((uint)_srid, _byteOrder);
      }

      // Next 1 byte of the WKB represent the byte order used to encode the binary data
      // The byte order is either 1 or 0 to indicate little-endian (NDR) or big-endian (XDR) storage.
      writer.Write((byte)_byteOrder);

      // Next 4 bytes of the WKB represent the geometry type
      // Values from 1 through 7 indicate Point, LineString, Polygon, MultiPoint, MultiLineString, MultiPolygon, and GeometryCollection.
      writer.WriteUIntUsingByteOrder((uint)geometry.Type, _byteOrder);

      // The rest of the bytes represent the geometry data
      switch (geometry.Type)
      {
        case GeometryType.Point:
          WritePoint(writer, geometry as Point);
          break;

        case GeometryType.LineString:
          WriteLineString(writer, geometry as LineString);
          break;

        case GeometryType.Polygon:
          WritePolygon(writer, geometry as Polygon);
          break;

        case GeometryType.MultiPoint:
          WriteMultiPoint(writer, geometry as MultiPoint);
          break;

        case GeometryType.MultiLineString:
          WriteMultiLineString(writer, geometry as MultiLineString);
          break;

        case GeometryType.MultiPolygon:
          WriteMultiPolygon(writer, geometry as MultiPolygon);
          break;

        case GeometryType.GeometryCollection:
          WriteGeometryCollection(writer, geometry as GeometryCollection);
          break;
      }
    }

    /// <summary>
		/// Writes a <see cref="MultiPolygon"/> to the given <see cref="BinaryWriter"/>.
		/// </summary>
		/// <param name="writer">The <see cref="BinaryWriter"/> used to write the data to.</param>
    /// <param name="geometryCollection">A <see cref="GeometryCollection"/> instance.</param>
		private void WriteGeometryCollection(BinaryWriter writer, GeometryCollection geometryCollection)
    {
      if (writer == null || geometryCollection == null)
      {
        return;
      }

      // Write the number of geometries in this GeometryCollection.
      writer.WriteUIntUsingByteOrder((uint)geometryCollection.GeometriesCount, _byteOrder);

      // Write Polygons.
      foreach (var geom in geometryCollection.Geometries)
      {
        WriteGeometry(writer, geom, false);
      }
    }

    /// <summary>
		/// Writes a <see cref="LineString"/> to the given <see cref="BinaryWriter"/>.
		/// </summary>
		/// <param name="writer">The <see cref="BinaryWriter"/> used to write the data to.</param>
    /// <param name="lineString">A <see cref="LineString"/> instance.</param>
		private void WriteLineString(BinaryWriter writer, LineString lineString)
    {
      if (writer == null || lineString == null)
      {
        return;
      }

      // Write the number of points contained in the LineString.
      writer.WriteUIntUsingByteOrder((uint)lineString.CoordinatesCount, _byteOrder);

      if (lineString.IsEmpty)
      {
        return;
      }

      // Write the point coordinates.
      foreach (var coord in lineString.Coordinates)
      {
        WriteCoordinate(writer, coord);
      }
    }

    /// <summary>
		/// Writes a <see cref="MultiLineString"/> to the given <see cref="BinaryWriter"/>.
		/// </summary>
		/// <param name="writer">The <see cref="BinaryWriter"/> used to write the data to.</param>
    /// <param name="multiLineString">A <see cref="MultiLineString"/> instance.</param>
		private void WriteMultiLineString(BinaryWriter writer, MultiLineString multiLineString)
    {
      if (writer == null || multiLineString == null)
      {
        return;
      }

      // Write the number of LineStrings in this MultiLineString.
      writer.WriteUIntUsingByteOrder((uint)multiLineString.GeometriesCount, _byteOrder);

      // Write LineStrings.
      foreach (var geom in multiLineString.Geometries)
      {
        var lineString = geom as LineString;
        if (lineString == null)
        {
          continue;
        }

        WriteGeometry(writer, lineString, false);
      }
    }

    /// <summary>
		/// Writes a <see cref="MultiPoint"/> to the given <see cref="BinaryWriter"/>.
		/// </summary>
		/// <param name="writer">The <see cref="BinaryWriter"/> used to write the data to.</param>
    /// <param name="multiPoint">A <see cref="MultiPoint"/> instance.</param>
		private void WriteMultiPoint(BinaryWriter writer, MultiPoint multiPoint)
    {
      if (writer == null || multiPoint == null)
      {
        return;
      }

      // Write the number of points in this multipoint..
      writer.WriteUIntUsingByteOrder((uint)multiPoint.GeometriesCount, _byteOrder);

      // Write points.
      foreach (var geom in multiPoint.Geometries)
      {
        var point = geom as Point;
        if (point == null)
        {
          continue;
        }

        WriteGeometry(writer, point, false);
      }
    }

    /// <summary>
		/// Writes a <see cref="MultiPolygon"/> to the given <see cref="BinaryWriter"/>.
		/// </summary>
		/// <param name="writer">The <see cref="BinaryWriter"/> used to write the data to.</param>
    /// <param name="multiPolygon">A <see cref="MultiPolygon"/> instance.</param>
		private void WriteMultiPolygon(BinaryWriter writer, MultiPolygon multiPolygon)
    {
      if (writer == null || multiPolygon == null)
      {
        return;
      }

      // Write the number of Polygons in this MultiPolygon.
      writer.WriteUIntUsingByteOrder((uint)multiPolygon.GeometriesCount, _byteOrder);

      // Write Polygons.
      foreach (var geom in multiPolygon.Geometries)
      {
        var polygon = geom as Polygon;
        if (polygon == null)
        {
          continue;
        }

        WriteGeometry(writer, polygon, false);
      }
    }

    /// <summary>
    /// Writes a <see cref="Point"/> to the given <see cref="BinaryWriter"/>.
    /// </summary>
    /// <param name="writer">The <see cref="BinaryWriter"/> used to write the data to.</param>
    /// <param name="point">A <see cref="Point"/> instance.</param>
    private void WritePoint(BinaryWriter writer, Point point)
    {
      if (writer == null || point == null)
      {
        return;
      }

      var coord = point.IsEmpty
        ? new Coordinate(double.NaN, double.NaN)
        : point.Coordinate;
      WriteCoordinate(writer, coord);
    }

    /// <summary>
		/// Writes a <see cref="Polygon"/> to the given <see cref="BinaryWriter"/>.
		/// </summary>
		/// <param name="writer">The <see cref="BinaryWriter"/> used to write the data to.</param>
    /// <param name="polygon">A <see cref="Polygon"/> instance.</param>
		private void WritePolygon(BinaryWriter writer, Polygon polygon)
    {
      if (writer == null || polygon == null)
      {
        return;
      }

      // Write the number of rings in this polygon (holes + shell).
      var numRings = polygon.IsEmpty ? 0 : (uint)polygon.HolesCount + 1;
      writer.WriteUIntUsingByteOrder(numRings, _byteOrder);

      if (polygon.IsEmpty)
      {
        return;
      }

      // Write the shell of the polygon.
      WriteLineString(writer, polygon.Shell);

      if (polygon.Holes == null)
      {
        return;
      }

      // Write the holes of the polygon.
      foreach (var hole in polygon.Holes)
      {
        WriteLineString(writer, hole);
      }
    }
  }
}
