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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using MySql.Utility.Classes.VisualStyles;

namespace MySql.Utility.Forms
{
  /// <summary>
  /// Represents a dialog that emulates the standard Vista-style Windows Task Panel containing a contents and a command panel.
  /// </summary>
  public partial class AutoStyleableBaseDialog : AutoStyleableBaseForm
  {
    #region Constants

    /// <summary>
    /// Default command area height in pixels.
    /// </summary>
    private const int DEFAULT_COMMAND_AREA_HEIGHT = 45;

    /// <summary>
    /// Default command area height in pixels.
    /// </summary>
    private const int DEFAULT_FOOTNOTE_AREA_HEIGHT = 80;

    /// <summary>
    /// Deefault panels separator thickness.
    /// </summary>
    private const int DEFAULT_PANELS_SEPARATOR_THICKNESS = 1;

    #endregion Constants

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoStyleableBaseDialog"/> class.
    /// </summary>
    public AutoStyleableBaseDialog()
    {
      AutoStyleDialog = false;
      DrawPanelsSeparator = true;
      MainInstruction = string.Empty;
      MainInstructionLocation = new Point(12, 9);
      MainInstructionLocationOffset = new Size(0, 0);
      MainInstructionImage = null;
      PanelsSeparatorColor = SystemColors.ControlDark;
      PanelsSeparatorThickness = DEFAULT_PANELS_SEPARATOR_THICKNESS;

      InitializeComponent();
    }

    #region Properties
    
    /// <summary>
    /// Gets or sets a value indicating whether the dialog should draw visual styles depending on the Windows version.
    /// </summary>
    [Category("Appearance"), DefaultValue(false), Description("Indicates if the dialog should draw visual styles depending on the Windows version.")]
    public bool AutoStyleDialog { get; set; }

    /// <summary>
    /// Gets or sets the command area background color.
    /// </summary>
    [Category("Appearance"), DefaultValue(typeof(Color), "Control"), Description("Sets the command area background color.")]
    public Color CommandAreaColor
    {
      get => CommandAreaPanel.BackColor;
      set => CommandAreaPanel.BackColor = value;
    }

    /// <summary>
    /// Gets or sets the command area height; when less than 0 the area is hidden.
    /// </summary>
    [Category("Layout"), DefaultValue(DEFAULT_COMMAND_AREA_HEIGHT), Description("Sets the command area height; when less than 0 the area is hidden.")]
    public int CommandAreaHeight
    {
      get => CommandAreaPanel.Height;
      set
      {
        if (value <= 0)
        {
          CommandAreaPanel.Height = 0;
          CommandAreaPanel.Visible = false;
        }
        else
        {
          CommandAreaPanel.Height = value;
        }
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the command area (normally for command buttons) at the middle of the dialog is visible.
    /// </summary>
    [Category("Layout"), DefaultValue(true), Description("Displays or hides command area (normally for command buttons) at the middle of the dialog.")]
    public bool CommandAreaVisible
    {
      get => CommandAreaPanel.Visible;
      set
      {
        CommandAreaPanel.Visible = value;
        CommandAreaHeight = value ? DEFAULT_COMMAND_AREA_HEIGHT : 0;
      }
    }

    /// <summary>
    /// Gets or sets the content area background color.
    /// </summary>
    [Category("Appearance"), DefaultValue(typeof(Color), "Window"), Description("Sets the content area background color.")]
    public Color ContentAreaColor
    {
      get => ContentAreaPanel.BackColor;
      set => ContentAreaPanel.BackColor = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether a thin line to separate panels is drawn.
    /// </summary>
    [Category("Appearance"), DefaultValue(true), Description("Draws a thin line to separate panels.")]
    public bool DrawPanelsSeparator { get; set; }

    /// <summary>
    /// Gets or sets the footnote area background color.
    /// </summary>
    [Category("Appearance"), DefaultValue(typeof(Color), "Control"), Description("Sets the footnote area background color.")]
    public Color FootnoteAreaColor
    {
      get => FootnoteAreaPanel.BackColor;
      set => FootnoteAreaPanel.BackColor = value;
    }
    
    /// <summary>
    /// Gets or sets the footnote area height; when 0 hides the footnote area.
    /// </summary>
    [Category("Layout"), DefaultValue(DEFAULT_FOOTNOTE_AREA_HEIGHT), Description("Sets the footnote area height; when 0 hides the footnote area.")]
    public int FootnoteAreaHeight
    {
      get => FootnoteAreaPanel.Height;
      set
      {
        if (value <= 0)
        {
          FootnoteAreaPanel.Height = 0;
          FootnoteAreaPanel.Visible = false;
        }
        else
        {
          FootnoteAreaPanel.Height = value;
        }
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the footnote area at the bottom of the dialog is visible.
    /// </summary>
    [Category("Layout"), DefaultValue(true), Description("Displays or hides the footnote area at the bottom of the dialog.")]
    public bool FootnoteAreaVisible
    {
      get => FootnoteAreaPanel.Visible;
      set
      {
        FootnoteAreaPanel.Visible = value;
        FootnoteAreaHeight = value ? DEFAULT_FOOTNOTE_AREA_HEIGHT : 0;
      }
    }

    /// <summary>
    /// Gets or sets the main instruction text for a Windows compliant dialog box.
    /// </summary>
    [Category("Appearance"), DefaultValue(""), Description("Main instruction text for a Windows compliant dialog box.")]
    public string MainInstruction { get; set; }

    /// <summary>
    /// Gets or sets the main instruction optional icon.
    /// </summary>
    [Category("Appearance"), DefaultValue(null), Description("Main instruction optional icon.")]
    public Image MainInstructionImage { get; set; }

    /// <summary>
    /// Gets or sets the main instruction image or text initial location.
    /// </summary>
    [Category("Layout"), DefaultValue(typeof(Point), "12, 9"), Description("Main instruction image or text initial location.")]
    public Point MainInstructionLocation { get; set; }

    /// <summary>
    /// Gets or sets the offset applied to the <see cref="MainInstructionLocation"/> property.
    /// </summary>
    [Category("Layout"), DefaultValue(typeof(Size), "0, 0"), Description("Offset applied to MainInstructionLocation property.")]
    public Size MainInstructionLocationOffset { get; set; }

    /// <summary>
    /// Gets or sets the color of the panels separator.
    /// </summary>
    [Category("Appearance"), DefaultValue(typeof(Color), "ControlDark"), Description("Color of the panels separator.")]
    public Color PanelsSeparatorColor { get; set; }

    /// <summary>
    /// Gets or sets the thickness in pixels of the lines separating panels.
    /// </summary>
    [Category("Appearance"), DefaultValue(DEFAULT_PANELS_SEPARATOR_THICKNESS), Description("Thickness in pixels of the lines separating panels.")]
    public int PanelsSeparatorThickness { get; set; }

    #endregion Properties

    /// <summary>
    /// Event delegate method fired when the <see cref="CommandAreaPanel"/> panel is being painted.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void CommandAreaPanel_Paint(object sender, PaintEventArgs e)
    {
      if (!CommandAreaVisible)
      {
        return;
      }

      if (AutoStyleDialog)
      {
        DrawThemeBackground(e.Graphics, CustomVisualStyleElements.TaskDialog.SecondaryPanel, CommandAreaPanel.ClientRectangle, e.ClipRectangle);
      }
      else if (DrawPanelsSeparator && PanelsSeparatorThickness > 0)
      {
        using (var separatorPen = new Pen(PanelsSeparatorColor, Math.Min(PanelsSeparatorThickness, CommandAreaHeight)))
        {
          e.Graphics.DrawLine(separatorPen, 0, 0, CommandAreaPanel.Width, 0);
        }
      }
    }

    /// <summary>
    /// Event delegate method fired when the <see cref="CommandAreaPanel"/> panel is being painted.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void ContentAreaPanel_Paint(object sender, PaintEventArgs e)
    {
      if (!AutoStyleDialog)
      {
        return;
      }

      DrawThemeBackground(e.Graphics, CustomVisualStyleElements.TaskDialog.PrimaryPanel, ContentAreaPanel.ClientRectangle, e.ClipRectangle);
      Point renderingStartingPoint = DrawImage(e.Graphics, MainInstructionImage, MainInstructionLocation);
      renderingStartingPoint.Offset(MainInstructionLocation.X, 0);
      renderingStartingPoint.Offset(MainInstructionLocationOffset.Width, MainInstructionLocationOffset.Height);
      StyleableHelper.DrawText(e.Graphics, MainInstruction, CustomVisualStyleElements.TextStyle.MainInstruction, new Font(Font, FontStyle.Bold), renderingStartingPoint, false, ClientSize.Width - renderingStartingPoint.X - MainInstructionLocation.X);
    }

    /// <summary>
    /// Draws an image within the dialog.
    /// </summary>
    /// <param name="graphics">Graphics object.</param>
    /// <param name="img">Image to draw.</param>
    /// <param name="location">Top-left corner coordinates where the image will be drawn.</param>
    /// <returns>Lower-right corner coordinates of the image drawn.</returns>
    private Point DrawImage(Graphics graphics, Image img, Point location)
    {
      var newLocation = location;
      if (img == null)
      {
        return newLocation;
      }

      newLocation = new Point(location.X + img.Width, location.Y);
      graphics.DrawImage(img, location);
      return newLocation;
    }

    /// <summary>
    /// Draws the background of the given visual element depending on the system's <see cref="VisualStyleRenderer"/> which is different among Windows versions.
    /// </summary>
    /// <param name="deviceContext">Windows device context.</param>
    /// <param name="element">Visual style element to draw the background for.</param>
    /// <param name="bounds">Size of the visual area.</param>
    /// <param name="clipRectangle">Rectangle in which to paint.</param>
    private void DrawThemeBackground(IDeviceContext deviceContext, VisualStyleElement element, Rectangle bounds, Rectangle clipRectangle)
    {
      if (!StyleableHelper.AreVistaDialogsThemeSupported)
      {
        return;
      }

      var renderer = new VisualStyleRenderer(element);
      renderer.DrawBackground(deviceContext, bounds, clipRectangle);
    }

    /// <summary>
    /// Event delegate method fired when the <see cref="FootnoteAreaPanel"/> panel is being painted.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void FootNoteAreaPanel_Paint(object sender, PaintEventArgs e)
    {
      if (!FootnoteAreaVisible)
      {
        return;
      }

      if (AutoStyleDialog)
      {
        DrawThemeBackground(e.Graphics, CustomVisualStyleElements.TaskDialog.SecondaryPanel, FootnoteAreaPanel.ClientRectangle, e.ClipRectangle);
      }
      else if (DrawPanelsSeparator && PanelsSeparatorThickness > 0)
      {
        using (var separatorPen = new Pen(PanelsSeparatorColor, Math.Min(PanelsSeparatorThickness, FootnoteAreaHeight)))
        {
          e.Graphics.DrawLine(separatorPen, 0, 0, FootnoteAreaPanel.Width, 0);
        }
      }
    }
  }
}