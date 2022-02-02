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

namespace MySql.Utility.Classes.Spatial
{
  /// <summary>
  /// Defines a rectangular region of the 2D coordinate plane, which often represents the bounding box of a <see cref="Geometry"/>.
  /// </summary>
  public class BoundingBox : IComparable, IEquatable<BoundingBox>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="BoundingBox"/> class.
    /// </summary>
    /// <remarks>This constructor sets the <see cref="BoundingBox"/> instance to null.</remarks>
    public BoundingBox()
    {
      Empty();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BoundingBox"/> class.
    /// </summary>
    /// <param name="x1">The first x-value.</param>
    /// <param name="x2">The second x-value.</param>
    /// <param name="y1">The first y-value.</param>
    /// <param name="y2">The second y-value.</param>
    public BoundingBox(double x1, double x2, double y1, double y2)
    {
      if (x1 < x2)
      {
        MinX = x1;
        MaxX = x2;
      }
      else
      {
        MinX = x2;
        MaxX = x1;
      }

      if (y1 < y2)
      {
        MinY = y1;
        MaxY = y2;
      }
      else
      {
        MinY = y2;
        MaxY = y1;
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BoundingBox"/> class.
    /// </summary>
    /// <param name="coordinate1">The first coordinate.</param>
    /// <param name="coordinate2">The second coordinate.</param>
    public BoundingBox(Coordinate coordinate1, Coordinate coordinate2)
      : this(coordinate1.X, coordinate2.X, coordinate1.Y, coordinate2.Y)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BoundingBox"/> class.
    /// </summary>
    /// <param name="coordinate">A <see cref="Coordinate"/> instance.</param>
    /// <remarks>Creates an <see cref="BoundingBox"/> for a region defined by a single coordinate.</remarks>
    public BoundingBox(Coordinate coordinate)
      : this(coordinate.X, coordinate.X, coordinate.Y, coordinate.Y)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BoundingBox"/> class.
    /// </summary>
    /// <param name="boundingBox">An existing <see cref="BoundingBox"/>.</param>
    public BoundingBox(BoundingBox boundingBox)
    {
      if (boundingBox == null)
      {
        return;
      }

      MinX = boundingBox.MinX;
      MaxX = boundingBox.MaxX;
      MinY = boundingBox.MinY;
      MaxY = boundingBox.MaxY;
    }

    #region Properties

    /// <summary>
    /// Gets the area of this <see cref="BoundingBox"/>.
    /// </summary>
    public double Area
    {
      get
      {
        return Width * Height;
      }
    }

    /// <summary>
    /// Gets the <see cref="Coordinate"/> representing the center of this <see cref="BoundingBox"/>.
    /// </summary>
    public Coordinate Center
    {
      get
      {
        return IsEmpty
          ? null
          : new Coordinate((MinX + MaxX) / 2, (MinY + MaxY) / 2);
      }
    }

    /// <summary>
    /// Gets the height of this <see cref="BoundingBox"/>.
    /// </summary>
    public double Height
    {
      get
      {
        return IsEmpty
          ? 0.0
          : MaxY - MinY;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="BoundingBox"/> is an empty one.
    /// </summary>
    public bool IsEmpty
    {
      get
      {
        return MaxX < MinX;
      }
    }

    /// <summary>
    /// Gets the maximum x-coordinate.
    /// </summary>
    public double MaxX { get; private set; }

    /// <summary>
    /// Gets the maximum y-coordinate.
    /// </summary>
    public double MaxY { get; private set; }

    /// <summary>
    /// Gets the minimum x-coordinate.
    /// </summary>
    public double MinX { get; private set; }

    /// <summary>
    /// Gets the minimum y-coordinate.
    /// </summary>
    public double MinY { get; private set; }

    /// <summary>
    /// Gets the width of this <see cref="BoundingBox"/>.
    /// </summary>
    public double Width
    {
      get
      {
        return IsEmpty
          ? 0.0
          : MaxX - MinX;
      }
    }

    #endregion Properties

    public static bool operator !=(BoundingBox lhs, BoundingBox rhs)
    {
      return !(lhs == rhs);
    }

    public static bool operator ==(BoundingBox lhs, BoundingBox rhs)
    {
      return ReferenceEquals(lhs, null)
        ? ReferenceEquals(rhs, null)
        : lhs.Equals(rhs);
    }

    /// <summary>
    /// Creates a <see cref="BoundingBox"/> from the given <see cref="Geometry"/>.
    /// </summary>
    /// <param name="point">A <see cref="Point"/> instance.</param>
    /// <returns>A <see cref="BoundingBox"/> from the given <see cref="Geometry"/>.</returns>
    public static BoundingBox FromGeometry(Point point)
    {
      if (point == null)
      {
        return null;
      }

      var emptyBoundingBox = new BoundingBox();
      if (point.IsEmpty)
      {
        return emptyBoundingBox;
      }

      emptyBoundingBox.ExpandToInclude(point.Coordinate.X, point.Coordinate.Y);
      return emptyBoundingBox;
    }

    /// <summary>
    /// Creates a <see cref="BoundingBox"/> from the given <see cref="Geometry"/>.
    /// </summary>
    /// <param name="curve">A <see cref="Curve"/> instance.</param>
    /// <returns>A <see cref="BoundingBox"/> from the given <see cref="Geometry"/>.</returns>
    public static BoundingBox FromGeometry(Curve curve)
    {
      if (curve == null)
      {
        return null;
      }

      var emptyBoundingBox = new BoundingBox();
      if (curve.IsEmpty)
      {
        return emptyBoundingBox;
      }

      foreach (var coordinate in curve.Coordinates)
      {
        emptyBoundingBox.ExpandToInclude(coordinate.X, coordinate.Y);
      }

      return emptyBoundingBox;
    }

    /// <summary>
    /// Creates a <see cref="BoundingBox"/> from the given <see cref="Geometry"/>.
    /// </summary>
    /// <param name="surface">A <see cref="Surface"/> instance.</param>
    /// <returns>A <see cref="BoundingBox"/> from the given <see cref="Geometry"/>.</returns>
    public static BoundingBox FromGeometry(Surface surface)
    {
      return surface == null
        ? null
        : FromGeometry(surface.Shell);
    }

    /// <summary>
    /// Creates a <see cref="BoundingBox"/> from the given <see cref="Geometry"/>.
    /// </summary>
    /// <param name="geometryCollection">A <see cref="GeometryCollection"/> instance.</param>
    /// <returns>A <see cref="BoundingBox"/> from the given <see cref="Geometry"/>.</returns>
    public static BoundingBox FromGeometry(GeometryCollection geometryCollection)
    {
      if (geometryCollection == null)
      {
        return null;
      }

      var boundingBox = new BoundingBox();
      foreach (var geom in geometryCollection.Geometries)
      {
        boundingBox.ExpandToInclude(geom.GetBoundingBox());
      }

      return boundingBox;
    }

    /// <summary>
    /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
    /// </summary>
    /// <param name="obj">An object to compare with this instance.</param>
    /// <returns>A value that indicates the relative order of the objects being compared.</returns>
    public int CompareTo(object obj)
    {
      var other = (BoundingBox)obj;
      if (IsEmpty)
      {
        if (other.IsEmpty) return 0;
        return -1;
      }
      if (other.IsEmpty) return 1;

      if (MinX < other.MinX
          || MinX > other.MinX
          || MinY < other.MinY
          || MinY > other.MinY
          || MaxX < other.MaxX
          || MaxX > other.MaxX
          || MaxY < other.MaxY
          || MaxY > other.MaxY)
        return 1;
      return 0;
    }

    /// <summary>
    /// Tests if the given <see cref="BoundingBox"/> lies completely inside this <see cref="BoundingBox"/> (inclusive of the boundary).
    /// </summary>
    /// <remarks>This is NOT the same definition as the SFS contains, which would exclude the bounding box boundary.</remarks>
    /// <param name="other">The <see cref="BoundingBox"/> to check.</param>
    /// <returns><c>true</c> if the given <see cref="BoundingBox"/> is contained in this <see cref="BoundingBox"/>, <c>false</c> otherwise.</returns>
    /// <seealso cref="Covers(BoundingBox)"/>
    public bool Contains(BoundingBox other)
    {
      return Covers(other);
    }

    /// <summary>
    /// Tests if the given <see cref="Coordinate"/> lies in or on this <see cref="BoundingBox"/>.
    /// </summary>
    /// <remarks>This is NOT the same definition as the SFS contains, which would exclude the bounding box boundary.</remarks>
    /// <param name="coordinate">A <see cref="Coordinate"/> to test.</param>
    /// <returns><c>true</c> if the given <see cref="Coordinate"/> lies in the interior or on the boundary of this <see cref="BoundingBox"/>, <c>false</c> otherwise.</returns>
    /// <seealso cref="Covers(Coordinate)"/>
    public bool Contains(Coordinate coordinate)
    {
      return Covers(coordinate);
    }

    /// <summary>
    /// Tests if the given point lies in or on this <see cref="BoundingBox"/>.
    /// </summary>
    /// <remarks>This is NOT the same definition as the SFS contains, which would exclude the bounding box boundary.</remarks>
    /// <param name="x">The x-coordinate of the point which this <see cref="BoundingBox"/> is being checked for containing.</param>
    /// <param name="y">The y-coordinate of the point which this <see cref="BoundingBox"/> is being checked for containing.</param>
    /// <returns><c>true</c> if the given point lies in the interior or on the boundary of this <see cref="BoundingBox"/>, <c>false</c> otherwise.</returns>
    /// <seealso cref="Covers(double,double)"/>
    public bool Contains(double x, double y)
    {
      return Covers(x, y);
    }

    /// <summary>
    /// Tests if the given point lies in or on this <see cref="BoundingBox"/>.
    /// </summary>
    /// <param name="x">The x-coordinate of the point which this <see cref="BoundingBox"/> is being checked for containing.</param>
    /// <param name="y">The y-coordinate of the point which this <see cref="BoundingBox"/> is being checked for containing.</param>
    /// <returns><c>true</c> if the given point lies in the interior or on the boundary of this <see cref="BoundingBox"/>, <c>false</c> otherwise.</returns>
    public bool Covers(double x, double y)
    {
      if (IsEmpty)
      {
        return false;
      }

      return x >= MinX
             && x <= MaxX
             && y >= MinY
             && y <= MaxY;
    }

    /// <summary>
    /// Tests if the given <see cref="Coordinate"/> lies in or on this <see cref="BoundingBox"/>.
    /// </summary>
    /// <param name="coordinate">A <see cref="Coordinate"/> to test.</param>
    /// <returns><c>true</c> if the given <see cref="Coordinate"/> lies in the interior or on the boundary of this <see cref="BoundingBox"/>, <c>false</c> otherwise.</returns>
    public bool Covers(Coordinate coordinate)
    {
      return coordinate != null && Covers(coordinate.X, coordinate.Y);
    }

    /// <summary>
    /// Tests if the given <see cref="BoundingBox"/> lies completely inside this <see cref="BoundingBox"/> (inclusive of the boundary).
    /// </summary>
    /// <remarks>This is NOT the same definition as the SFS contains, which would exclude the bounding box boundary.</remarks>
    /// <param name="other">The <see cref="BoundingBox"/> to check.</param>
    /// <returns><c>true</c> if the given <see cref="BoundingBox"/> is contained in this <see cref="BoundingBox"/>, <c>false</c> otherwise.</returns>
    public bool Covers(BoundingBox other)
    {
      if (other == null || IsEmpty || other.IsEmpty)
      {
        return false;
      }

      return other.MinX >= MinX
             && other.MaxX <= MaxX
             && other.MinY >= MinY
             && other.MaxY <= MaxY;
    }

    /// <summary>
    /// Makes this <see cref="BoundingBox"/> an empty one, i.e. the boundingBox of an empty geometry.
    /// </summary>
    public void Empty()
    {
      MinX = 0;
      MaxX = -1;
      MinY = 0;
      MinY = -1;
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns><c>true</c> if the current object is equal to the other parameter; otherwise, <c>false</c>.</returns>
    public bool Equals(BoundingBox other)
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
      return MaxX.IsPracticallyEqual(other.MaxX)
              && MaxY.IsPracticallyEqual(other.MaxY)
              && MinX.IsPracticallyEqual(other.MinX)
              && MinY.IsPracticallyEqual(other.MinY);
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
    {
      return Equals(obj as BoundingBox);
    }

    /// <summary>
    /// Expands this <see cref="BoundingBox"/> by a given distance in all directions.
    /// Both positive and negative distances are supported.
    /// </summary>
    /// <param name="deltaX">The distance to expand the boundingBox along the the X axis.</param>
    /// <param name="deltaY">The distance to expand the boundingBox along the the Y axis.</param>
    public void ExpandBy(double deltaX, double deltaY)
    {
      if (IsEmpty)
      {
        return;
      }

      MinX -= deltaX;
      MaxX += deltaX;
      MinY -= deltaY;
      MaxY += deltaY;

      // Check for boundingBox disappearing
      if (MinX > MaxX || MinY > MaxY)
      {
        Empty();
      }
    }

    /// <summary>
    /// Expands this <see cref="BoundingBox"/> by a given distance in all directions.
    /// Both positive and negative distances are supported.
    /// </summary>
    /// <param name="distance">The distance to expand the boundingBox.</param>
    public void ExpandBy(double distance)
    {
      ExpandBy(distance, distance);
    }

    /// <summary>
    /// Enlarges this <see cref="BoundingBox"/> so that it contains the given point.
    /// Has no effect if the point is already on or within the bounding box.
    /// </summary>
    /// <param name="x">The value to lower the minimum x to or to raise the maximum x to.</param>
    /// <param name="y">The value to lower the minimum y to or to raise the maximum y to.</param>
    public void ExpandToInclude(double x, double y)
    {
      if (IsEmpty)
      {
        MinX = x;
        MaxX = x;
        MinY = y;
        MaxY = y;
      }
      else
      {
        if (x < MinX)
        {
          MinX = x;
        }

        if (x > MaxX)
        {
          MaxX = x;
        }

        if (y < MinY)
        {
          MinY = y;
        }

        if (y > MaxY)
        {
          MaxY = y;
        }
      }
    }

    /// <summary>
    /// Enlarges this <see cref="BoundingBox"/> so that it contains the given <see cref="Coordinate"/>.
    /// Has no effect if the point is already on or within the bounding box.
    /// </summary>
    /// <param name="coordinate">The <see cref="Coordinate"/> to expand to include.</param>
    public void ExpandToInclude(Coordinate coordinate)
    {
      if (coordinate == null)
      {
        return;
      }

      ExpandToInclude(coordinate.X, coordinate.Y);
    }

    /// <summary>
    /// Enlarges this <see cref="BoundingBox"/> so that it contains the given one.
    /// Has no effect if the given <see cref="BoundingBox"/> is on or within the bounding box.
    /// </summary>
    /// <param name="other">The <see cref="BoundingBox"/> to expand to include.</param>
    public void ExpandToInclude(BoundingBox other)
    {
      if (other == null || other.IsEmpty)
      {
        return;
      }

      if (IsEmpty)
      {
        MinX = other.MinX;
        MaxX = other.MaxX;
        MinY = other.MinY;
        MaxY = other.MaxY;
      }
      else
      {
        if (other.MinX < MinX)
        {
          MinX = other.MinX;
        }

        if (other.MaxX > MaxX)
        {
          MaxX = other.MaxX;
        }

        if (other.MinY < MinY)
        {
          MinY = other.MinY;
        }

        if (other.MaxY > MaxY)
        {
          MaxY = other.MaxY;
        }
      }
    }

    /// <summary>
    /// Returns a <see cref="LineSegment"/> representing the diagonal line of this <see cref="BoundingBox"/>.
    /// </summary>
    /// <returns>A <see cref="LineSegment"/> representing the diagonal line of this <see cref="BoundingBox"/>.</returns>
    public LineSegment GetDiagonalSegment()
    {
      return IsEmpty
        ? null
        : new LineSegment(MinX, MinY, MaxX, MaxX);
    }

    /// <summary>
    /// Computes the distance between this and another <see cref="BoundingBox"/>.
    /// </summary>
    /// <param name="boundingBox">The <see cref="BoundingBox"/> to compute the distance to.</param>
    /// <returns>The distance between overlapping bounding boxes is <c>0.0</c>, otherwise the distance is the Euclidean distance between the closest points.</returns>
    public double GetDistance(BoundingBox boundingBox)
    {
      if (Intersects(boundingBox))
      {
        return 0.0;
      }

      double dx = 0.0;
      if (MaxX < boundingBox.MinX)
      {
        dx = boundingBox.MinX - MaxX;
      }
      else if (MinX > boundingBox.MaxX)
      {
        dx = MinX - boundingBox.MaxX;
      }

      double dy = 0.0;
      if (MaxY < boundingBox.MinY)
      {
        dy = boundingBox.MinY - MaxY;
      }
      else if (MinY > boundingBox.MaxY)
      {
        dy = MinY - boundingBox.MaxY;
      }

      // If either is zero, the bounding boxes overlap either vertically or horizontally
      if (dx.IsPracticallyEqual(0.0))
      {
        return dy;
      }

      if (dy.IsPracticallyEqual(0.0))
      {
        return dx;
      }

      return Math.Sqrt(dx * dx + dy * dy);
    }

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
      // Arbitrary number to generate the hash code.
      const int hashCodeMultiplier = 397;
      unchecked
      {
        int hashCode = MinX.GetHashCode();
        hashCode = (hashCode * hashCodeMultiplier) ^ MaxX.GetHashCode();
        hashCode = (hashCode * hashCodeMultiplier) ^ MinY.GetHashCode();
        hashCode = (hashCode * hashCodeMultiplier) ^ MaxY.GetHashCode();
        return hashCode;
      }
    }

    /// <summary>
    /// Computes the intersection of this and the given <see cref="BoundingBox"/>.
    /// </summary>
    /// <param name="other">The <see cref="BoundingBox"/> to intersect with.</param>
    /// <returns>A new <see cref="BoundingBox"/> representing the intersection of the 2 bounding boxes, <c>null</c> if either argument is empty or if they do not intersect.</returns>
    public BoundingBox GetIntersection(BoundingBox other)
    {
      if (other == null || IsEmpty || other.IsEmpty || !Intersects(other))
      {
        return null;
      }

      double intMinX = MinX > other.MinX ? MinX : other.MinX;
      double intMinY = MinY > other.MinY ? MinY : other.MinY;
      double intMaxX = MaxX < other.MaxX ? MaxX : other.MaxX;
      double intMaxY = MaxY < other.MaxY ? MaxY : other.MaxY;
      return new BoundingBox(intMinX, intMaxX, intMinY, intMaxY);
    }

    /// <summary>
    /// Checks if the region defined by another <see cref="BoundingBox"/> overlaps/intersects the region of this <see cref="BoundingBox"/>.
    /// </summary>
    /// <param name="other">The <see cref="BoundingBox"/> which this <see cref="BoundingBox"/> is being checked for intersection.</param>
    /// <returns><c>true</c> if the <see cref="BoundingBox"/>es intersect, <c>false</c> otherwise.</returns>
    public bool Intersects(BoundingBox other)
    {
      if (other == null || IsEmpty || other.IsEmpty)
      {
        return false;
      }

      return !(other.MinX > MaxX
               || other.MaxX < MinX
               || other.MinY > MaxY
               || other.MaxY < MinY);
    }

    /// <summary>
    /// Checks if the given <see cref="Coordinate"/> lies inside the region of this <see cref="BoundingBox"/>.
    /// </summary>
    /// <param name="coordinate">The <see cref="Coordinate"/> to be tested.</param>
    /// <returns><c>true</c> if the <see cref="Coordinate"/> lies inside this <see cref="BoundingBox"/>, <c>false</c> otherwise.</returns>
    public bool Intersects(Coordinate coordinate)
    {
      return coordinate != null && Intersects(coordinate.X, coordinate.Y);
    }

    /// <summary>
    /// Checks if the given point lies inside the region of this <see cref="BoundingBox"/>.
    /// </summary>
    /// <param name="x">The x-ordinate of the point.</param>
    /// <param name="y">The y-ordinate of the point.</param>
    /// <returns><c>true</c> if the point lies inside this <see cref="BoundingBox"/>, <c>false</c> otherwise.</returns>
    public bool Intersects(double x, double y)
    {
      if (IsEmpty)
      {
        return false;
      }

      return !(x > MaxX
              || x < MinX
              || y > MaxY
              || y < MinY);
    }

    /// <summary>
    /// Converts this <see cref="BoundingBox"/> to a <see cref="Polygon"/> that describes its vertices.
    /// </summary>
    /// <returns></returns>
    /// <param name="srid">The ID of the Spatial Reference System used by the <see cref="Geometry"/>.</param>
    public Geometry ToGeometry(int srid = 0)
    {
      // Empty bounding box - return empty point geometry
      if (IsEmpty)
      {
        return new Point(null, srid);
      }

      // point?
      if (MinX.IsPracticallyEqual(MaxX)
          && MinY.IsPracticallyEqual(MaxY))
      {
        return new Point(new Coordinate(MinX, MinY), srid);
      }

      // vertical or horizontal line?
      if (MinX.IsPracticallyEqual(MaxX)
          || MinY.IsPracticallyEqual(MaxY))
      {
        var lineCoordinates = new List<Coordinate>(2)
        {
          new Coordinate(MinX, MinY),
          new Coordinate(MaxX, MaxY)
        };
        return new LineString(lineCoordinates, srid);
      }

      // create a CW ring for the polygon
      var startAndEndCoordinate = new Coordinate(MinX, MinY);
      var polygonCoordinates = new List<Coordinate>(5)
        {
          startAndEndCoordinate,
          new Coordinate(MinX, MaxY),
          new Coordinate(MaxX, MaxY),
          new Coordinate(MaxX, MinY),
          startAndEndCoordinate
        };
      return new Polygon(new LinearRing(polygonCoordinates, srid), srid);
    }

    /// <summary>
    /// Returns a string that represents this <see cref="BoundingBox"/>'s corner coordinates.
    /// </summary>
    /// <returns>A string that represents this <see cref="BoundingBox"/>'s corner coordinates.</returns>
    public override string ToString()
    {
      return string.Format("BBOX({0},{1},{2},{3})", MinX, MaxX, MinY, MaxY);
    }

    /// <summary>
    /// Translates this <see cref="BoundingBox"/> by given amounts in the X and Y direction.
    /// </summary>
    /// <param name="deltaX">The amount to translate along the X axis.</param>
    /// <param name="deltaY">The amount to translate along the Y axis.</param>
    public void Translate(double deltaX, double deltaY)
    {
      if (IsEmpty)
      {
        return;
      }

      MinX += deltaX;
      MaxX += deltaX;
      MinY += deltaY;
      MaxY += deltaY;
    }
  }
}
