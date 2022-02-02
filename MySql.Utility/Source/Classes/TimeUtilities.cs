// Copyright (c) 2012-2014, Oracle and/or its affiliates. All rights reserved.
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

namespace MySql.Utility.Classes
{
  /// <summary>
  /// Contains methods to work with time intervals.
  /// </summary>
  public class TimeUtilities
  {
    /// <summary>
    /// Unit of measure used for time intervals.
    /// </summary>
    public enum IntervalUnitOfMeasure
    {
      /// <summary>
      /// Interval measured in seconds.
      /// </summary>
      Seconds = 0,

      /// <summary>
      /// Interval measured in minutes.
      /// </summary>
      Minutes = 1,

      /// <summary>
      /// Interval measured in hours.
      /// </summary>
      Hours = 2,

      /// <summary>
      /// Interval measured in days.
      /// </summary>
      Days = 3
    }

    /// <summary>
    /// Converts an interval in a given time unit of measure to seconds.
    /// </summary>
    /// <param name="baseUoM">Base time unit of measure.</param>
    /// <param name="timeInterval">Interval to convert to seconds.</param>
    /// <returns>Time interval in seconds.</returns>
    public static double ConvertToSeconds(IntervalUnitOfMeasure baseUoM, double timeInterval)
    {
      switch (baseUoM)
      {
        case IntervalUnitOfMeasure.Seconds:
          return timeInterval;

        case IntervalUnitOfMeasure.Minutes:
          return TimeSpan.FromMinutes(timeInterval).TotalSeconds;

        case IntervalUnitOfMeasure.Hours:
          return TimeSpan.FromHours(timeInterval).TotalSeconds;

        case IntervalUnitOfMeasure.Days:
          return TimeSpan.FromDays(timeInterval).TotalSeconds;
      }

      return 0;
    }
  }
}
