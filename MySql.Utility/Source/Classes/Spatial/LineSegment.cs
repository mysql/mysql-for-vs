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

namespace MySql.Utility.Classes.Spatial
{
  /// <summary>
  /// Represents a linear segment between 2 points in space.
  /// </summary>
  public class LineSegment : IEquatable<LineSegment>, ICloneable
  {
    #region Fields

    /// <summary>
    /// The <see cref="BoundingBox"/> of the segment.
    /// </summary>
    private BoundingBox _boundingBox;

    /// <summary>
    /// The value of the y intercept, which is the "b" component in the linear equation y = m*x + b.
    /// </summary>
    private double? _intercept;

    /// <summary>
    /// Flag indicating whether the segment is vertical.
    /// </summary>
    private bool? _isVertical;

    /// <summary>
    /// The length of the segment.
    /// </summary>
    private double? _length;

    /// <summary>
    /// The <see cref="Coordinate"/> that represents a point in the middle of the segment.
    /// </summary>
    private Coordinate _midPoint;

    /// <summary>
    /// The slope of the segment, which is the "m" component in the linear equation y = m*x + b.
    /// </summary>
    private double? _slope;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of the <see cref="LineSegment"/> class.
    /// </summary>
    /// <param name="start">The <see cref="Coordinate"/> that defines the start of the segment.</param>
    /// <param name="end">The <see cref="Coordinate"/> that defines the end of the segment.</param>
    public LineSegment(Coordinate start, Coordinate end)
    {
      if (start == null)
      {
        throw new ArgumentNullException(nameof(start));
      }

      if (end == null)
      {
        throw new ArgumentNullException(nameof(end));
      }

      ResetFieldsDependantOnStartEndCoordinates();
      Start = start;
      End = end;

      Start.PropertyChanged += CoordinatePropertyChanged;
      End.PropertyChanged += CoordinatePropertyChanged;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LineSegment"/> class.
    /// </summary>
    /// <param name="startX">The x-coordinate of the start of the segment.</param>
    /// <param name="startY">The y-coordinate of the start of the segment.</param>
    /// <param name="endX">The x-coordinate of the end of the segment.</param>
    /// <param name="endY">The y-coordinate of the end of the segment.</param>
    public LineSegment(double startX, double startY, double endX, double endY)
      : this(new Coordinate(startX, startY), new Coordinate(endX, endY))
    {
    }

    #region Properties

    /// <summary>
    /// Gets the <see cref="BoundingBox"/> of the segment.
    /// </summary>
    public BoundingBox BoundingBox
    {
      get
      {
        return _boundingBox ?? (_boundingBox = new BoundingBox(Start, End));
      }
    }

    /// <summary>
    /// Gets the <see cref="Coordinate"/> that defines the end of the segment.
    /// </summary>
    public Coordinate End { get; private set; }

    /// <summary>
    /// Gets the value of the y intercept, which is the "b" component in the linear equation y = m*x + b.
    /// </summary>
    public double Intercept
    {
      get
      {
        if (_intercept == null)
        {
          _intercept = IsVertical
            ? double.NaN
            : Start.Y - (Slope * Start.X);
        }

        return _intercept.Value;
      }
    }

    /// <summary>
    /// Gets a value indicating whether the segment is vertical.
    /// </summary>
    public bool IsVertical
    {
      get
      {
        if (_isVertical == null)
        {
          _isVertical = Start.X.IsPracticallyEqual(End.X);
        }

        return _isVertical.Value;
      }
    }

    /// <summary>
    /// Gets the length of the segment.
    /// </summary>
    public double Length
    {
      get
      {
        if (_length == null)
        {
          var dx = End.X - Start.X;
          var dy = End.Y - Start.Y;
          _length = Math.Sqrt(dx * dx + dy * dy);
        }

        return _length.Value;
      }
    }

    /// <summary>
    /// Gets the <see cref="Coordinate"/> that represents a point in the middle of the segment.
    /// </summary>
    public Coordinate MidPoint
    {
      get
      {
        return _midPoint ?? (_midPoint = new Coordinate((End.X + Start.X) / 2, (End.Y + Start.Y) / 2));
      }
    }

    /// <summary>
    /// Gets the slope of the segment, which is the "m" component in the linear equation y = m*x + b.
    /// </summary>
    public double Slope
    {
      get
      {
        if (_slope == null)
        {
          _slope = IsVertical
            ? double.NaN
            : (End.Y - Start.Y) / (End.X - Start.X);
        }

        return _slope.Value;
      }
    }

    /// <summary>
    /// Gets the <see cref="Coordinate"/> that defines the start of the segment.
    /// </summary>
    public Coordinate Start { get; private set; }

    #endregion Properties

    public static bool operator !=(LineSegment lhs, LineSegment rhs)
    {
      return !(lhs == rhs);
    }

    public static bool operator ==(LineSegment lhs, LineSegment rhs)
    {
      return ReferenceEquals(lhs, null)
        ? ReferenceEquals(rhs, null)
        : lhs.Equals(rhs);
    }

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public object Clone()
    {
      return new LineSegment(Start, End);
    }

    /// <summary>
    /// Checks if the given <see cref="Coordinate"/> lies within this <see cref="LineSegment"/>.
    /// </summary>
    /// <param name="coordinate">The <see cref="Coordinate"/> to check.</param>
    /// <returns><c>true</c> if the given <see cref="Coordinate"/> lies within this <see cref="LineSegment"/>, <c>false</c> otherwise.</returns>
    public bool Encompasses(Coordinate coordinate)
    {
      return coordinate != null && Encompasses(coordinate.X, coordinate.Y);
    }

    /// <summary>
    /// Checks if the point defined by the given x and y coordinates lies within this <see cref="LineSegment"/>.
    /// </summary>
    /// <param name="x">The x-coordinate to check.</param>
    /// <param name="y">The y-coordinate to check.</param>
    /// <returns><c>true</c> if the point defined by the given x and y coordinates lies within this <see cref="LineSegment"/>, <c>false</c> otherwise.</returns>
    public bool Encompasses(double x, double y)
    {
      // Not using Math functions intentionally, this is faster.
      var minX = Start.X > End.X ? End.X : Start.X;
      var maxX = Start.X > End.X ? Start.X : End.X;
      var minY = Start.Y > End.Y ? End.Y : Start.Y;
      var maxY = Start.Y > End.Y ? Start.Y : End.Y;
      return x >= minX && x <= maxX && y >= minY && y <= maxY;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
    {
      return Equals(obj as LineSegment);
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns><c>true</c> if the current object is equal to the other parameter; otherwise, <c>false</c>.</returns>
    public bool Equals(LineSegment other)
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
      return Start == other.Start && End == other.End;
    }

    /// <summary>
    /// Returns the y-coordinate of an arbitrary point in the line projected by the slope of this segment.
    /// </summary>
    /// <param name="x">An arbitrary x-coordinate.</param>
    /// <returns>The y-coordinate of an arbitrary point in the line projected by the slope of this segment.</returns>
    public double GetArbitraryYOnSlope(double x)
    {
      if (IsVertical)
      {
        return double.NaN;
      }

      return Slope * x + Intercept;
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
        int hashCode = Start.GetHashCode();
        hashCode = (hashCode * hashCodeMultiplier) ^ End.GetHashCode();
        return hashCode;
      }
    }

    /// <summary>
    /// Returns a <see cref="Coordinate"/> or a <see cref="LineSegment"/> that represents an intersection between this <see cref="LineSegment"/> and the given one.
    /// </summary>
    /// <param name="other">A <see cref="LineSegment"/> to check for an intersection.</param>
    /// <param name="ignoreEndPointsIntersection">Ignore any intersection of end points between segments.</param>
    /// <returns>A <see cref="Coordinate"/> or a <see cref="LineSegment"/> that represents an intersection between this <see cref="LineSegment"/> and the given one, <c>null</c> if no intersection is found.</returns>
    public object GetIntersection(LineSegment other, bool ignoreEndPointsIntersection = false)
    {
      // 1. Check if bounding boxes overlap, if they do not, we are certain there is no intersection
      if (!BoundingBox.Intersects(other.BoundingBox))
      {
        return null;
      }

      bool intersectingCoordinateIsEndPoint = false;
      Coordinate intersectingCoordinate;

      // 2. Check if any of the segments is vertical
      if (IsVertical || other.IsVertical)
      {
        if (IsVertical && other.IsVertical)
        {
          // 2a. If both are vertical and S1.X1 != S2.X1, then no intersection.
          if (!Start.X.IsPracticallyEqual(other.Start.X))
          {
            return null;
          }

          // 2b. If both are vertical and S1.X1 == S1.X1, check if (S1.Y1, S1.Y2) and (S2.Y1, S1.Y2) overlap.
          //     They may overlap in a single point or in a whole segment.
          var intersectingBoundingBox = BoundingBox.GetIntersection(other.BoundingBox);
          if (intersectingBoundingBox != null)
          {
            var diagonal = intersectingBoundingBox.GetDiagonalSegment();
            if (diagonal.Length.IsPracticallyEqual(0.0))
            {
              // The segments overlap in a single point, so it must be an end-point.
              return ignoreEndPointsIntersection
                ? null
                : diagonal.Start;
            }

            // The segments overlap in a whole segment, so a LineSegment is returned.
            return diagonal;
          }
        }

        // 2c. Only one is vertical. Build the equation of the non-vertical segment and find the point where the two segments intersect
        //     (by substituting VS.X1 into the equation of the non-vertical segment) and check if this point is within both segments.
        var verticalSegment = IsVertical ? this : other;
        var nonVerticalsegment = IsVertical ? other : this;
        var intersectingX = verticalSegment.Start.X;
        var intersectingY = nonVerticalsegment.GetArbitraryYOnSlope(intersectingX);
        intersectingCoordinate = new Coordinate(intersectingX, intersectingY);
        intersectingCoordinateIsEndPoint = IsEndPoint(intersectingCoordinate) || other.IsEndPoint(intersectingCoordinate);
        if (intersectingCoordinateIsEndPoint)
        {
          return ignoreEndPointsIntersection
            ? null
            : intersectingCoordinate;
        }

        if (Encompasses(intersectingCoordinate) || other.Encompasses(intersectingCoordinate))
        {
          return intersectingCoordinate;
        }
      }

      // 3. Check if segments are parallel (have same slope).
      if (Slope.IsPracticallyEqual(other.Slope))
      {
        // 3a. Check if segments have the same intercept.
        if (Intercept.IsPracticallyEqual(other.Intercept))
        {
          // Check overlapping end points.
          intersectingCoordinate = IsEndPoint(other.Start)
            ? other.Start
            : (IsEndPoint(other.End) ? other.End : null);
          intersectingCoordinateIsEndPoint = intersectingCoordinate != null;
          if (intersectingCoordinateIsEndPoint)
          {
            return ignoreEndPointsIntersection
              ? null
              : intersectingCoordinate;
          }

          // Check for a whole overlapping segment.
          var intersectingBoundingBox = BoundingBox.GetIntersection(other.BoundingBox);
          if (intersectingBoundingBox != null)
          {
            // The slope of the diagonal of the intersecting bounding box is always positive.
            var diagonal = intersectingBoundingBox.GetDiagonalSegment();

            // If the slopes of the segments are positive, then the diagonal of the intersecting bounding box is exactly the intersecting segment.
            if (Slope > 0)
            {
              return diagonal;
            }
            
            // If the slopes are negative, then negate the x-coordinates of both end points of the diagonal.
            return new LineSegment(diagonal.Start.NegateX(), diagonal.End.NegateX());
          }
        }

        // 3b. Not the same intercept, segments do not touch.
        return null;
      }

      // 4. If segments are not parallel, the point of intersection is a solution of a system of linear equations y = m1*x + b1 and y = m2*x + b2 for each segment respectively.
      //    The 2 equations can be equated as m1*x + b1 = m2*x + b2 which gives us a solution for x (we'll call it x0) as x0 = (b2 - b1) / (m1 - m2).
      var x0 = (other.Intercept - Intercept) / (Slope - other.Slope);

      // 4b. Get the intersecting y coordinate (y0) by substitution of x0 into any of the linear equations.
      var y0 = GetArbitraryYOnSlope(x0);

      // 4c. Check if the intersecting coordinate is one of the end points
      intersectingCoordinate = new Coordinate(x0, y0);
      intersectingCoordinateIsEndPoint = IsEndPoint(intersectingCoordinate) || other.IsEndPoint(intersectingCoordinate);
      if (intersectingCoordinateIsEndPoint)
      {
        return ignoreEndPointsIntersection
          ? null
          : intersectingCoordinate;
      }

      // 4d. The resulting coordinate may NOT be within the segments (may be a projection along the slope of segment).
      return Encompasses(intersectingCoordinate) && other.Encompasses(intersectingCoordinate)
        ? intersectingCoordinate
        :null;
    }

    /// <summary>
    /// Checks if this <see cref="LineSegment"/> intersects with the given one.
    /// </summary>
    /// <param name="other">A <see cref="LineSegment"/> to check for an intersection.</param>
    /// <param name="ignoreEndPointsIntersection">Ignore any intersection of end points between segments.</param>
    /// <returns><c>true</c> if both <see cref="LineSegment"/>s intersect at some point, <c>false</c> otherwise.</returns>
    public bool IntersectsWith(LineSegment other, bool ignoreEndPointsIntersection = false)
    {
      return GetIntersection(other, ignoreEndPointsIntersection) != null;
    }

    /// <summary>
    /// Checks if the given <see cref="Coordinate"/> is an end-point (<see cref="Start"/> or <see cref="End"/>) of this <see cref="LineSegment"/>.
    /// </summary>
    /// <param name="coordinate">The <see cref="Coordinate"/> to check.</param>
    /// <returns><c>true</c> if the given <see cref="Coordinate"/> is an end-point (<see cref="Start"/> or <see cref="End"/>) of this <see cref="LineSegment"/>, <c>false</c> otherwise.</returns>
    public bool IsEndPoint(Coordinate coordinate)
    {
      return coordinate != null && IsEndPoint(coordinate.X, coordinate.Y);
    }

    /// <summary>
    /// Checks if the given <see cref="Coordinate"/> is an end-point (<see cref="Start"/> or <see cref="End"/>) of this <see cref="LineSegment"/>.
    /// </summary>
    /// <param name="x">The x-coordinate to check.</param>
    /// <param name="y">The y-coordinate to check.</param>
    /// <returns><c>true</c> if the given <see cref="Coordinate"/> is an end-point (<see cref="Start"/> or <see cref="End"/>) of this <see cref="LineSegment"/>, <c>false</c> otherwise.</returns>
    public bool IsEndPoint(double x, double y)
    {
      return (Start.X.IsPracticallyEqual(x) && Start.Y.IsPracticallyEqual(y))
             || (End.X.IsPracticallyEqual(x) && End.Y.IsPracticallyEqual(y));
    }

    /// <summary>
    /// Event delegate method fired when either coordinate in the segment changes values.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void CoordinatePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      ResetFieldsDependantOnStartEndCoordinates();
    }

    /// <summary>
    /// Resets the values of the fields that depend on the <see cref="Start"/> and <see cref="End"/> coordinates.
    /// </summary>
    private void ResetFieldsDependantOnStartEndCoordinates()
    {
      _boundingBox = null;
      _intercept = null;
      _isVertical = null;
      _length = null;
      _midPoint = null;
      _slope = null;
    }
  }
}
