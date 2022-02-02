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
using MySql.Utility.Enums;

namespace MySql.Utility.Classes.Spatial
{
  /// <summary>
  /// Contains functionality to read or store primitive numeric data in a given byte order.
  /// </summary>
  public static class ByteOrderUtils
  {
    /// <summary>
    /// Returns a double value converted from a bytes array in the given byte order.
    /// </summary>
    /// <param name="buffer">A bytes array.</param>
    /// <param name="byteOrder">The <see cref="ByteOrderType"/> used for encoding numbers.</param>
    /// <returns>A double value from a bytes array.</returns>
    public static double GetDouble(byte[] buffer, ByteOrderType byteOrder)
    {
      var longVal = GetLong(buffer, byteOrder);
      return BitConverter.Int64BitsToDouble(longVal);
    }

    /// <summary>
    /// Sets a double value converted to a bytes array in the given byte order.
    /// </summary>
    /// <param name="doubleValue">A double value.</param>
    /// <param name="buffer">The bytes array where the value is to be set.</param>
    /// <param name="byteOrder">The <see cref="ByteOrderType"/> used for encoding numbers.</param>
    public static void SetDouble(double doubleValue, ref byte[] buffer, ByteOrderType byteOrder)
    {
      var longVal = BitConverter.DoubleToInt64Bits(doubleValue);
      SetLong(longVal, ref buffer, byteOrder);
    }

    /// <summary>
    /// Returns an integer converted from a bytes array in the given byte order.
    /// </summary>
    /// <param name="buffer">A bytes array.</param>
    /// <param name="byteOrder">The <see cref="ByteOrderType"/> used for encoding numbers.</param>
    /// <returns>An integer from a bytes array.</returns>
    public static int GetInt(byte[] buffer, ByteOrderType byteOrder)
    {
      switch (byteOrder)
      {
        case ByteOrderType.ExternalDataRepresentation:
          return ((int)(buffer[0] & 0xff) << 24)
                | ((int)(buffer[1] & 0xff) << 16)
                | ((int)(buffer[2] & 0xff) << 8)
                | ((int)(buffer[3] & 0xff));

        case ByteOrderType.NetworkDataRepresentation:
          return ((int)(buffer[3] & 0xff) << 24)
                | ((int)(buffer[2] & 0xff) << 16)
                | ((int)(buffer[1] & 0xff) << 8)
                | ((int)(buffer[0] & 0xff));

        default:
          return 0;
      }
    }

    /// <summary>
    /// Sets an integer converted to a bytes array in the given byte order.
    /// </summary>
    /// <param name="intValue">An integer value.</param>
    /// <param name="buffer">The bytes array where the value is to be set.</param>
    /// <param name="byteOrder">The <see cref="ByteOrderType"/> used for encoding numbers.</param>
    public static void SetInt(int intValue, ref byte[] buffer, ByteOrderType byteOrder)
    {
      switch (byteOrder)
      {
        case ByteOrderType.ExternalDataRepresentation:
          buffer[0] = (byte)(intValue >> 24);
          buffer[1] = (byte)(intValue >> 16);
          buffer[2] = (byte)(intValue >> 8);
          buffer[3] = (byte)intValue;
          break;

        case ByteOrderType.NetworkDataRepresentation:
          buffer[0] = (byte)intValue;
          buffer[1] = (byte)(intValue >> 8);
          buffer[2] = (byte)(intValue >> 16);
          buffer[3] = (byte)(intValue >> 24);
          break;
      }
    }

    /// <summary>
    /// Returns a long value converted from a bytes array in the given byte order.
    /// </summary>
    /// <param name="buffer">A bytes array.</param>
    /// <param name="byteOrder">The <see cref="ByteOrderType"/> used for encoding numbers.</param>
    /// <returns>A long value from a bytes array.</returns>
    public static long GetLong(byte[] buffer, ByteOrderType byteOrder)
    {
      switch (byteOrder)
      {
        case ByteOrderType.ExternalDataRepresentation:
          return (long)(buffer[0] & 0xff) << 56
                | (long)(buffer[1] & 0xff) << 48
                | (long)(buffer[2] & 0xff) << 40
                | (long)(buffer[3] & 0xff) << 32
                | (long)(buffer[4] & 0xff) << 24
                | (long)(buffer[5] & 0xff) << 16
                | (long)(buffer[6] & 0xff) << 8
                | (long)(buffer[7] & 0xff);

        case ByteOrderType.NetworkDataRepresentation:
          return (long)(buffer[7] & 0xff) << 56
                | (long)(buffer[6] & 0xff) << 48
                | (long)(buffer[5] & 0xff) << 40
                | (long)(buffer[4] & 0xff) << 32
                | (long)(buffer[3] & 0xff) << 24
                | (long)(buffer[2] & 0xff) << 16
                | (long)(buffer[1] & 0xff) << 8
                | (long)(buffer[0] & 0xff);

        default:
          return 0;
      }
    }

    /// <summary>
    /// Sets a long value converted to a bytes array in the given byte order.
    /// </summary>
    /// <param name="longValue">An integer value.</param>
    /// <param name="buffer">The bytes array where the value is to be set.</param>
    /// <param name="byteOrder">The <see cref="ByteOrderType"/> used for encoding numbers.</param>
    public static void SetLong(long longValue, ref byte[] buffer, ByteOrderType byteOrder)
    {
      switch (byteOrder)
      {
        case ByteOrderType.ExternalDataRepresentation:
          buffer[0] = (byte)(longValue >> 56);
          buffer[1] = (byte)(longValue >> 48);
          buffer[2] = (byte)(longValue >> 40);
          buffer[3] = (byte)(longValue >> 32);
          buffer[4] = (byte)(longValue >> 24);
          buffer[5] = (byte)(longValue >> 16);
          buffer[6] = (byte)(longValue >> 8);
          buffer[7] = (byte)longValue;
          break;

        case ByteOrderType.NetworkDataRepresentation:
          buffer[0] = (byte)longValue;
          buffer[1] = (byte)(longValue >> 8);
          buffer[2] = (byte)(longValue >> 16);
          buffer[3] = (byte)(longValue >> 24);
          buffer[4] = (byte)(longValue >> 32);
          buffer[5] = (byte)(longValue >> 40);
          buffer[6] = (byte)(longValue >> 48);
          buffer[7] = (byte)(longValue >> 56);
          break;
      }
    }
  }
}
