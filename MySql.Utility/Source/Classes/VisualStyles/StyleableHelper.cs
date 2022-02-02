// Copyright (c) 2012, 2019, Oracle and/or its affiliates. All rights reserved.
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
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace MySql.Utility.Classes.VisualStyles
{
  /// <summary>
  /// Defines methods that help with the automatic styling of windows forms and dialogs.
  /// </summary>
  public static class StyleableHelper
  {
    #region Constants

    /// <summary>
    /// The standard value for DPI settings at 100%.
    /// </summary>
    public const float STANDARD_DPI = 96;

    #endregion Constants

    #region Properties

    /// <summary>
    /// Gets a value indicating whether Vista dialog themes are supported.
    /// </summary>
    public static bool AreVistaDialogsThemeSupported => IsWindowsVistaOrLater && VisualStyleRenderer.IsSupported && Application.RenderWithVisualStyles;

    /// <summary>
    /// Gets a <see cref="ColorMatrix"/> to convert colors to a greyscale palette.
    /// </summary>
    public static ColorMatrix GrayScaleColorMatrix => new ColorMatrix(
      new[]
      {
        new[] {.3f, .3f, .3f, 0, 0},
        new[] {.59f, .59f, .59f, 0, 0},
        new[] {.11f, .11f, .11f, 0, 0},
        new float[] {0, 0, 0, 1, 0},
        new float[] {0, 0, 0, 0, 1}
      });

    /// <summary>
    /// Gets a <see cref="ColorMatrix"/> to convert colors to an inverted palette.
    /// </summary>
    public static ColorMatrix InvertedColorMatrix => new ColorMatrix(
      new[]
      {
        new float[] {-1, 0, 0, 0, 0},
        new float[] {0, -1, 0, 0, 0},
        new float[] {0, 0, -1, 0, 0},
        new float[] {0, 0, 0, 1, 0},
        new float[] {1, 1, 1, 0, 1}
      });

    /// <summary>
    /// Gets a value indicating whether the OS version is Windows.
    /// </summary>
    public static bool IsWindows
    {
      get
      {
        var platform = Environment.OSVersion.Platform;
        return platform == PlatformID.Win32NT
               || platform == PlatformID.Win32S
               || platform == PlatformID.Win32Windows;
      }
    }

    /// <summary>
    /// Gets a value indicating whether the Windows version is Vista or later (7, 8).
    /// </summary>
    public static bool IsWindowsVistaOrLater => IsWindows && Environment.OSVersion.Version >= new Version(6, 0, 6000);

    /// <summary>
    /// Gets a value indicating whether the Windows version is XP or later (Vista, 7, 8).
    /// </summary>
    public static bool IsWindowsXpOrLater => IsWindows && Environment.OSVersion.Version >= new Version(5, 1, 2600);
  
    #endregion Properties

    /// <summary>
    /// Draws a text string within the given device context on the given location and using a specific visual style element.
    /// </summary>
    /// <param name="deviceContext">Windows device context.</param>
    /// <param name="text">Text string to draw.</param>
    /// <param name="element">Visual style element to draw the text for.</param>
    /// <param name="fallbackFont">Font to use in case Vista dialog themes are not supported.</param>
    /// <param name="location">Top-left coordinates where to start drawing the text.</param>
    /// <param name="measureOnly">Flag indicating if text will not be drawn but only the drawing area will be measured.</param>
    /// <param name="width">Width of the area where text is going to be drawn.</param>
    /// <returns>Bottom-left coordinates of the drawn text rectangle.</returns>
    public static Point DrawText(IDeviceContext deviceContext, string text, VisualStyleElement element, Font fallbackFont, Point location, bool measureOnly, int width)
    {
      var newLocation = location;
      if (string.IsNullOrEmpty(text))
      {
        return newLocation;
      }

      var textRect = new Rectangle(location.X, location.Y, width, (IsWindowsXpOrLater ? int.MaxValue : 100000));
      const TextFormatFlags FLAGS = TextFormatFlags.WordBreak;
      if (AreVistaDialogsThemeSupported)
      {
        var renderer = new VisualStyleRenderer(element);
        var textSize = renderer.GetTextExtent(deviceContext, textRect, text, FLAGS);
        newLocation = location + new Size(0, textSize.Height);
        if (!measureOnly)
        {
          renderer.DrawText(deviceContext, textSize, text, false, FLAGS);
        }
      }
      else
      {
        if (!measureOnly)
        {
          TextRenderer.DrawText(deviceContext, text, fallbackFont, textRect, SystemColors.WindowText, FLAGS);
        }

        var textSize = TextRenderer.MeasureText(deviceContext, text, fallbackFont, new Size(textRect.Width, textRect.Height), FLAGS);
        newLocation = location + new Size(0, textSize.Height);
      }

      return newLocation;
    }

    /// <summary>
    /// Gets the multiplying factor to convert physical pixels to DIPs (Device-Independent Pixels).
    /// </summary>
    /// <param name="control">A <see cref="Control"/> instance.</param>
    /// <returns>The multiplying factor to convert physical pixels to DIPs (Device-Independent Pixels).</returns>
    public static float GetDpiScaleX(this Control control)
    {
      if (control == null)
      {
        return 1;
      }

      using (var graphics = control.CreateGraphics())
      {
        return graphics.DpiX / STANDARD_DPI;
      }
    }

    /// <summary>
    /// Gets the multiplying factor to convert physical pixels to DIPs (Device-Independent Pixels).
    /// </summary>
    /// <param name="control">A <see cref="Control"/> instance.</param>
    /// <returns>The multiplying factor to convert physical pixels to DIPs (Device-Independent Pixels).</returns>
    public static float GetDpiScaleY(this Control control)
    {
      if (control == null)
      {
        return 1;
      }

      using (var graphics = control.CreateGraphics())
      {
        return graphics.DpiY / STANDARD_DPI;
      }
    }
  }
}