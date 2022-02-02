// Copyright (c) 2014, 2016, Oracle and/or its affiliates. All rights reserved.
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

using System.Windows.Forms;

namespace MySql.Utility.Structs
{
  /// <summary>
  /// Struct that holds the InfoDialog return values.
  /// </summary>
  public struct InfoDialogResult
  {
    /// <summary>
    /// Gets or sets the dialog result for the InfoDialog.
    /// </summary>
    public DialogResult DialogResult { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the command area CheckBox from the dialog is checked.
    /// </summary>
    public bool InfoCheckboxValue { get; set; }

    /// <summary>
    /// Gets or sets the index of the selected item of the command area ComboBox.
    /// </summary>
    public int InfoComboBoxSelectedIndex { get; set; }

    /// <summary>
    /// Gets or sets the value of the selected item of the command area ComboBox.
    /// </summary>
    public string InfoComboBoxSelectedValue { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="InfoDialogResult"/> struct.
    /// </summary>
    public InfoDialogResult(DialogResult dialogResult, bool infoCheckboxValue, int infoComboBoxSelectedIndex, string infoComboBoxSelectedValue)
      : this()
    {
      DialogResult = dialogResult;
      InfoCheckboxValue = infoCheckboxValue;
      InfoComboBoxSelectedIndex = infoComboBoxSelectedIndex;
      InfoComboBoxSelectedValue = infoComboBoxSelectedValue;
    }
  }
}
