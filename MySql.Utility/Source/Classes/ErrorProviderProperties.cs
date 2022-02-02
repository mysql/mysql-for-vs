// Copyright (c) 2019, Oracle and/or its affiliates. All rights reserved.
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

using System.Drawing;
using System.Windows.Forms;

namespace MySql.Utility.Classes
{
  /// <summary>
  /// Represents properties to pass to an <see cref="ErrorProvider"/>
  /// </summary>
  public class ErrorProviderProperties
  {
    #region Constants

    /// <summary>
    /// The default icon padding used for the error provider used to validate fields.
    /// </summary>
    public const int DEFAULT_VALIDATIONS_PROVIDER_ICON_PADDING = 6;

    #endregion Constants

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorProviderProperties"/> class.
    /// </summary>
    /// <param name="errorMessage">The error message to associate with the <see cref="ErrorProvider"/>.</param>
    public ErrorProviderProperties(string errorMessage)
      : this(errorMessage, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorProviderProperties"/> class.
    /// </summary>
    /// <param name="errorMessage">The error message to associate with the <see cref="ErrorProvider"/>.</param>
    /// <param name="errorIcon">The <see cref="Icon"/> to assign to the <see cref="ErrorProvider"/> different to the default one.</param>
    public ErrorProviderProperties(string errorMessage, Icon errorIcon)
    {
      Clear = false;
      ErrorIcon = errorIcon;
      ErrorMessage = errorMessage;
      IconAlignment = ErrorIconAlignment.MiddleRight;
      IconPadding = DEFAULT_VALIDATIONS_PROVIDER_ICON_PADDING;
    }

    #region Properties

    /// <summary>
    /// Gets a default <see cref="ErrorProviderProperties"/> instance.
    /// </summary>
    public static ErrorProviderProperties Empty => new ErrorProviderProperties(null);

    /// <summary>
    /// Gets or sets a value indicating whether the error provider must be cleared for all currently attached controls.
    /// </summary>
    public bool Clear { get; set; }

    /// <summary>
    /// Gets or sets an optional <see cref="Icon"/> to assign to the <see cref="ErrorProvider"/> different to the default one.
    /// </summary>
    public Icon ErrorIcon { get; set; }

    /// <summary>
    /// Gets or sets the error message to associate with the <see cref="ErrorProvider"/>.
    /// </summary>
    public string ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the location where the error icon should be placed in relation to the control.
    /// </summary>
    public ErrorIconAlignment IconAlignment { get; set; }

    /// <summary>
    /// Gets or sets the amount of extra space to leave between the specified control and the error icon.
    /// </summary>
    public int IconPadding { get; set; }

    #endregion Properties
  }
}
