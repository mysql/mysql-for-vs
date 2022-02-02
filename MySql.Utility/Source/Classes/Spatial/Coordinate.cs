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
using System.ComponentModel;
using MySql.Utility.Interfaces;

namespace MySql.Utility.Classes.Spatial
{
  /// <summary>
  /// Represents a coordinate in 2D space using double numbers for the X and Y elements.
  /// </summary>
  public class Coordinate : ISpatialElement, IComparable, ICloneable, IEquatable<Coordinate>, INotifyPropertyChanged
  {
    #region Fields

    /// <summary>
    /// The x-coordinate.
    /// </summary>
    private double _x;

    /// <summary>
    /// The y-coordinate.
    /// </summary>
    private double _y;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of the <see cref="Coordinate"/> class.
    /// </summary>
    /// <param name="x">The x-coordinate.</param>
    /// <param name="y">The y-coordinate.</param>
    public Coordinate(double x, double y)
    {
      _x = x;
      _y = y;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Coordinate"/> class.
    /// </summary>
    public Coordinate()
      : this(0.0, 0.0)
    {
    }

    #region Properties

    /// <summary>
    /// Gets or sets the x-coordinate.
    /// </summary>
    public double X
    {
      get
      {
        return _x;
      }

      set
      {
        _x = value;
        OnPropertyChanged(nameof(X));
      }
    }

    /// <summary>
    /// Gets or sets the y-coordinate.
    /// </summary>
    public double Y
    {
      get
      {
        return _y;
      }

      set
      {
        _y = value;
        OnPropertyChanged(nameof(Y));
      }
    }

    #endregion Properties

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    public static bool operator !=(Coordinate lhs, Coordinate rhs)
    {
      return !(lhs == rhs);
    }

    public static bool operator ==(Coordinate lhs, Coordinate rhs)
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
      return new Coordinate(X, Y);
    }

    /// <summary>
    /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
    /// </summary>
    /// <param name="obj">An object to compare with this instance.</param>
    /// <returns>A value that indicates the relative order of the objects being compared.</returns>
    public int CompareTo(object obj)
    {
      var other = (Coordinate)obj;
      if (X < other.X) return -1;
      if (X > other.X) return 1;
      if (Y < other.Y) return -1;
      if (Y > other.Y) return 1;
      return 0;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
    {
      return Equals(obj as Coordinate);
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns><c>true</c> if the current object is equal to the other parameter; otherwise, <c>false</c>.</returns>
    public bool Equals(Coordinate other)
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
      return X.IsPracticallyEqual(other.X)
             && Y.IsPracticallyEqual(other.Y);
    }

    /// <summary>
    /// Computes the distance between this coordinate and a given one.
    /// </summary>
    /// <param name="other">Another coordinate.</param>
    /// <returns>The distance between this coordinate and a given one.</returns>
    public double GetDistance(Coordinate other)
    {
      if (other == null)
      {
        return 0;
      }

      double dx = X - other.X;
      double dy = Y - other.Y;
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
        int hashCode = X.GetHashCode();
        hashCode = (hashCode * hashCodeMultiplier) ^ Y.GetHashCode();
        return hashCode;
      }
    }

    /// <summary>
    /// Returns a new <see cref="Coordinate"/> with the the x and y coordinates negated.
    /// </summary>
    /// <returns>A new <see cref="Coordinate"/> with the the x and y coordinates negated.</returns>
    public Coordinate Negate()
    {
      return new Coordinate(-X, -Y);
    }

    /// <summary>
    /// Returns a new <see cref="Coordinate"/> with the the x coordinate negated.
    /// </summary>
    /// <returns>A new <see cref="Coordinate"/> with the the x coordinate negated.</returns>
    public Coordinate NegateX()
    {
      return new Coordinate(-X, Y);
    }

    /// <summary>
    /// Returns a new <see cref="Coordinate"/> with the the y coordinate negated.
    /// </summary>
    /// <returns>A new <see cref="Coordinate"/> with the the y coordinate negated.</returns>
    public Coordinate NegateY()
    {
      return new Coordinate(X, -Y);
    }

    /// <summary>
    /// Returns a GeoJSON text representation of this spatial element.
    /// </summary>
    /// <returns>A GeoJSON text representation of this spatial element.</returns>
    public string ToGeoJsonString()
    {
      return string.Format("[ {0}, {1} ]", X, Y);
    }

    /// <summary>
    /// Returns a GML text representation of this spatial element.
    /// </summary>
    /// <returns>A GML text representation of this spatial element.</returns>
    public string ToGmlString()
    {
      return string.Format("{0},{1}", X, Y);
    }

    /// <summary>
    /// Returns a KML text representation of this spatial element.
    /// </summary>
    /// <returns>A KML text representation of this spatial element.</returns>
    public string ToKmlString()
    {
      return ToGmlString();
    }

    /// <summary>
    /// Returns a string that represents the current <see cref="Coordinate"/>.
    /// </summary>
    /// <returns>A string that represents the current <see cref="Coordinate"/>.</returns>
    public override string ToString()
    {
      return ToWktString();
    }

    /// <summary>
    /// Returns a WKT text representation of this spatial element.
    /// </summary>
    /// <returns>A WKT text representation of this spatial element.</returns>
    public string ToWktString()
    {
      return string.Format("{0} {1}", X, Y);
    }

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName"></param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
      if (PropertyChanged == null)
      {
        return;
      }

      PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
