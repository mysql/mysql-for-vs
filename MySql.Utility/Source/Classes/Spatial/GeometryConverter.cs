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
using System.Globalization;
using MySql.Utility.Enums;

namespace MySql.Utility.Classes.Spatial
{
  /// <summary>
  /// Defines functionality to natively convert a <see cref="Geometry"/> to <see cref="string"/>.
  /// </summary>
  public class GeometryConverter : TypeConverter
  {
    #region Fields

    /// <summary>
    /// The <see cref="GeometryAsTextFormatType"/> used to encode a <see cref="Geometry"/> as text.
    /// </summary>
    private GeometryAsTextFormatType _textFormat;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of the <see cref="GeometryConverter"/> class.
    /// </summary>
    public GeometryConverter()
    {
      TextFormat = GeometryAsTextFormatType.None;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GeometryConverter"/> class.
    /// </summary>
    /// <param name="textFormat">The <see cref="GeometryAsTextFormatType"/> used to encode a <see cref="Geometry"/> as text.</param>
    public GeometryConverter(GeometryAsTextFormatType textFormat)
    {
      TextFormat = textFormat;
    }

    #region Properties

    /// <summary>
    /// Gets or sets the <see cref="GeometryAsTextFormatType"/> used to encode a <see cref="Geometry"/> as text.
    /// </summary>
    public GeometryAsTextFormatType TextFormat
    {
      get
      {
        if (_textFormat == GeometryAsTextFormatType.None)
        {
          TextFormat = GeometryAsTextFormatType.WKT;
        }

        return _textFormat;
      }

      set
      {
        _textFormat = value;
      }
    }

    #endregion Properties

    /// <summary>
    /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
    /// </summary>
    /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
    /// <param name="sourceType">A <see cref="Type"/> that represents the type you want to convert from. </param>
    /// <returns><c>true</c> if this converter can perform the conversion, <c>false</c> otherwise.</returns>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    /// <summary>
    /// Returns whether this converter can convert the object to the specified type, using the specified context.
    /// </summary>
    /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
    /// <param name="destinationType">A <see cref="Type"/> that represents the type you want to convert to. </param>
    /// <returns><c>true</c> if this converter can perform the conversion, <c>false</c> otherwise.</returns>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
    }

    /// <summary>
    /// Converts the given object to the type of this converter, using the specified context and culture information.
    /// </summary>
    /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
    /// <param name="culture">The <see cref="CultureInfo"/> to use as the current culture. </param>
    /// <param name="value">The <see cref="object"/> to convert. </param>
    /// <returns>An <see cref="object"/> that represents the converted value.</returns>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      if (value is string)
      {
        var stringValue = value.ToString();
        if (!Geometry.IsValid(stringValue, TextFormat))
        {
          throw new NotSupportedException(stringValue + " is not a valid value for " + typeof(Geometry).Name + ".");
        }

        var geometry = Geometry.Parse(stringValue, TextFormat);
        return geometry;
      }

      return base.ConvertFrom(context, culture, value);
    }

    /// <summary>
    /// Converts the given value object to the specified type, using the specified context and culture information.
    /// </summary>
    /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
    /// <param name="culture">A <see cref="CultureInfo"/>. If null is passed, the current culture is assumed. </param>
    /// <param name="value">The <see cref="object"/> to convert. </param>
    /// <param name="destinationType">The <see cref="Type"/> to convert the <seealso cref="value"/> parameter to. </param>
    /// <returns>An <see cref="object"/> that represents the converted value.</returns>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
      if (destinationType == typeof(string))
      {
        var geom = value as Geometry;
        if (geom != null)
        {
          return geom.ToString(TextFormat);
        }
      }

      return base.ConvertTo(context, culture, value, destinationType);
    }

    /// <summary>
    /// Returns whether the given value object is valid for this type and for the specified context.
    /// </summary>
    /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
    /// <param name="value">The <see cref="object"/> to test for validity. </param>
    /// <returns><c>true</c> if the specified value is valid for this object, <c>false</c> otherwise.</returns>
    public override bool IsValid(ITypeDescriptorContext context, object value)
    {
      return Geometry.IsValid(value.ToString(), TextFormat);
    }
  }
}
