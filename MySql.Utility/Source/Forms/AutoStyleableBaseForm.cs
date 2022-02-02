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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using static System.Drawing.SystemFonts;

namespace MySql.Utility.Forms
{
  /// <summary>
  /// Represents a window or dialog that inherits a style on its child elements, like fonts, colors and panels to emulate Vista-style dialogs.
  /// </summary>
  public partial class AutoStyleableBaseForm : Form
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="AutoStyleableBaseForm"/> class.
    /// </summary>
    public AutoStyleableBaseForm()
    {
      InitializeComponent();

      UseSystemFont = true;
      InheritSystemFontToControls = true;
      InheritFontToControlsExceptionList = new List<string>();
    }

    #region Properties

    /// <summary>
    /// Gets or sets a value indicating if the consumer application needs manual handling of DPI conversions to set the form's sizes.
    /// </summary>
    public static bool HandleDpiSizeConversions { get; set; }

    /// <summary>
    /// Gets or sets a list of control names that will not inherit the form's font.
    /// </summary>
    [Category("Appearance"), Description("List of control names that will not inherit the form's font.")]
    [Editor(@"System.Windows.Forms.Design.StringCollectionEditor," +
        "System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
       typeof(System.Drawing.Design.UITypeEditor))]
    public List<string> InheritFontToControlsExceptionList { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the form's font will be inherited to children controls.
    /// </summary>
    [Category("Appearance"), DefaultValue(true), Description("Indicates if the form's font will be inherited to children controls.")]
    public bool InheritSystemFontToControls { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the form automatically uses the system default font.
    /// </summary>
    [Category("Appearance"), DefaultValue(true), Description("Indicates whether the form automatically uses the system default font.")]
    public bool UseSystemFont { get; set; }

    #endregion Properties

    /// <summary>
    /// Inherits the form's font to controls in the controls collection, applying exceptions in the <see cref="InheritFontToControlsExceptionList"/>.
    /// </summary>
    /// <param name="controls">Collection of controls to recursively inherit the form's font to.</param>
    protected virtual void InheritFontToControls(Control.ControlCollection controls)
    {
      if (controls == null || controls.Count == 0)
      {
        return;
      }

      foreach (Control c in controls)
      {
        InheritFontToControls(c.Controls);
        if (InheritFontToControlsExceptionList != null && InheritFontToControlsExceptionList.Contains(c.Name))
        {
          continue;
        }

        if (c.Font.Name != Font.Name)
        {
          c.Font = new Font(Font.FontFamily, c.Font.Size, c.Font.Style);
        }
      }
    }

    /// <summary>
    /// Raises the <see cref="System.Windows.Forms.Form.FormClosed"/> event.
    /// </summary>
    /// <param name="e">Event arguments.</param>
    protected override void OnFormClosed(FormClosedEventArgs e)
    {
      base.OnFormClosed(e);
      Microsoft.Win32.SystemEvents.UserPreferenceChanged -= SystemEvents_UserPreferenceChanged;
    }

    /// <summary>
    /// Raises the <see cref="System.Windows.Forms.Form.Load"/> event.
    /// </summary>
    /// <param name="e">Event arguments.</param>
    protected override void OnLoad(EventArgs e)
    {
      if (!DesignMode && UseSystemFont)
      {
        Font = new Font(IconTitleFont.FontFamily, Font.Size, Font.Style, Font.Unit, Font.GdiCharSet, Font.GdiVerticalFont);
        Microsoft.Win32.SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;
        if (InheritSystemFontToControls)
        {
          InheritFontToControls(Controls);
        }
      }

      base.OnLoad(e);
    }

    /// <summary>
    /// Event delegate method fired when the <see cref="Microsoft.Win32.SystemEvents"/> object's user preferences change.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void SystemEvents_UserPreferenceChanged(object sender, Microsoft.Win32.UserPreferenceChangedEventArgs e)
    {
      if (e.Category == Microsoft.Win32.UserPreferenceCategory.Window && UseSystemFont)
      {
        Font = new Font(IconTitleFont.FontFamily, Font.Size, Font.Style, Font.Unit, Font.GdiCharSet, Font.GdiVerticalFont);
      }
    }
  }
}