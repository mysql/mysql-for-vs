// Copyright (c) 2014, 2018, Oracle and/or its affiliates. All rights reserved.
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

using System.Collections.Generic;
using System.Windows.Forms;
using MySql.Utility.Forms;

namespace MySql.Utility.Classes
{
  /// <summary>
  /// Defines properties for buttons used in the <see cref="InfoDialog"/> class.
  /// </summary>
  public class CommandAreaProperties
  {
    #region Constants

    /// <summary>
    /// The default time in seconds after which the default button's click event will be automatically fired.
    /// </summary>
    public const int DEFAULT_BUTTONS_TIMEOUT_IN_SECONDS = 10;

    /// <summary>
    /// The default text used for the left area checkbox control.
    /// </summary>
    public const string DEFAULT_LEFT_AREA_CHECKBOX_TEXT = "Do not show this dialog again for this session";

    #endregion Constants

    #region Fields

    /// <summary>
    /// The dialog result assigned to the <see cref="InfoDialog.Info1Button"/>.
    /// </summary>
    private DialogResult _button1DialogResult;

    /// <summary>
    /// The text displayed on the <see cref="InfoDialog.Info1Button"/>.
    /// </summary>
    private string _button1Text;

    /// <summary>
    /// The dialog result assigned to the <see cref="InfoDialog.Info2Button"/>.
    /// </summary>
    private DialogResult _button2DialogResult;

    /// <summary>
    /// The text displayed on the <see cref="InfoDialog.Info2Button"/>.
    /// </summary>
    private string _button2Text;

    /// <summary>
    /// The dialog result assigned to the <see cref="InfoDialog.Info3Button"/>.
    /// </summary>
    private DialogResult _button3DialogResult;

    /// <summary>
    /// Flag indicating whether the <see cref="InfoDialog.Info3Button"/> is treated as the <see cref="InfoDialog.MoreInfoButton"/>.
    /// </summary>
    private bool _button3IsMoreInfo;

    /// <summary>
    /// The text displayed on the <see cref="InfoDialog.Info3Button"/>.
    /// </summary>
    private string _button3Text;

    /// <summary>
    /// The <see cref="ButtonsLayoutType"/> used to specify the buttons available to the <see cref="InfoDialog"/>.
    /// </summary>
    private ButtonsLayoutType _buttonsLayout;

    /// <summary>
    /// The <see cref="LeftAreaControlType"/> used to specify the control to display at the left of the command area.
    /// </summary>
    private LeftAreaControlType _leftAreaControl;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandAreaProperties"/> class.
    /// </summary>
    /// <param name="buttonsLayout">The <see cref="ButtonsLayoutType"/> used to specify the buttons available to the <see cref="InfoDialog"/>.</param>
    /// <param name="leftAreaControl">The <see cref="LeftAreaControlType"/> used to specify the control to display at the left of the command area.</param>
    /// <param name="allowOverridingOfNonGenericLayoutType">Flag indicating whether buttons properties can be overriden when the <see cref="ButtonsLayout"/> is set to a non-generic one.</param>
    public CommandAreaProperties(ButtonsLayoutType buttonsLayout, LeftAreaControlType leftAreaControl = LeftAreaControlType.MoreInfoButton, bool allowOverridingOfNonGenericLayoutType = false)
    {
      _button1DialogResult = DialogResult.None;
      _button1Text = Resources.GenericButton1DefaultText;
      _button2DialogResult = DialogResult.None;
      _button2Text = Resources.GenericButton2DefaultText;
      _button3DialogResult = DialogResult.None;
      _button3IsMoreInfo = false;
      _button3Text = Resources.GenericButton3DefaultText;
      _leftAreaControl = leftAreaControl;
      AllowOverridingOfNonGenericLayoutType = allowOverridingOfNonGenericLayoutType;
      DefaultButton = InfoDialog.DefaultButtonType.None;
      DefaultButtonTimeout = DEFAULT_BUTTONS_TIMEOUT_IN_SECONDS;
      ButtonsLayout = buttonsLayout;
      LeftAreaCheckBoxText = DEFAULT_LEFT_AREA_CHECKBOX_TEXT;
      LeftAreaCheckBoxChecked = false;
      LeftAreaComboBoxDataSource = null;
      LeftAreaComboBoxWidth = -1;
      MoreInfoExpandedButtonText = string.Format(Resources.HidePrefixText, Resources.MoreInfoText);
      MoreInfoCollapsedButtonText = string.Format(Resources.ShowPrefixText, Resources.MoreInfoText);
      SameButtonWidths = false;
    }

    #region Enumerations

    /// <summary>
    /// Specifies the buttons available on the dialog.
    /// </summary>
    public enum ButtonsLayoutType
    {
      /// <summary>
      /// Only a Back button is displayed on the dialog.
      /// </summary>
      BackOnly,

      /// <summary>
      /// Only 1 generic button is displayed on the dialog.
      /// </summary>
      Generic1Button,

      /// <summary>
      /// Two generic buttons are displayed on the dialog.
      /// </summary>
      Generic2Buttons,

      /// <summary>
      /// Three generic buttons are displayed on the dialog.
      /// </summary>
      Generic3Buttons,

      /// <summary>
      /// Only an OK button is displayed on the dialog.
      /// </summary>
      OkOnly,

      /// <summary>
      /// OK and Cancel buttons are displayed on the dialog.
      /// </summary>
      OkCancel,

      /// <summary>
      /// Yes and No buttons are displayed on the dialog.
      /// </summary>
      YesNo,

      /// <summary>
      /// Yes, No and Cancel buttons are displayed on the dialog.
      /// </summary>
      YesNoCancel
    }

    /// <summary>
    /// Specifies the type of control shown at the left area of the dialog.
    /// </summary>
    public enum LeftAreaControlType
    {
      /// <summary>
      /// Show a <see cref="CheckBox"/> control.
      /// </summary>
      InfoCheckBox,

      /// <summary>
      /// Show a <see cref="ComboBox"/> control.
      /// </summary>
      InfoComboBox,

      /// <summary>
      /// Show a <see cref="Button"/> control used to toggle the display of more information related to the <see cref="InfoDialog"/>.
      /// </summary>
      MoreInfoButton
    }

    #endregion Enumerations

    #region Properties

    /// <summary>
    /// Gets or sets a value indicating whether buttons properties can be overriden when the <see cref="ButtonsLayout"/> is set to a non-generic one.
    /// </summary>
    public bool AllowOverridingOfNonGenericLayoutType { get; set; }

    /// <summary>
    /// Gets or sets the dialog result assigned to the <see cref="InfoDialog.Info1Button"/>.
    /// </summary>
    public DialogResult Button1DialogResult
    {
      get
      {
        return _button1DialogResult;
      }

      set
      {
        if (CanSetButtonPropertyValue)
        {
          _button1DialogResult = value;
        }
      }
    }

    /// <summary>
    /// Gets or sets the text displayed on the <see cref="InfoDialog.Info1Button"/>.
    /// </summary>
    public string Button1Text
    {
      get
      {
        return _button1Text;
      }

      set
      {
        if (CanSetButtonPropertyValue)
        {
          _button1Text = value;
        }
      }
    }

    /// <summary>
    /// Gets or sets the dialog result assigned to the <see cref="InfoDialog.Info2Button"/>.
    /// </summary>
    public DialogResult Button2DialogResult
    {
      get
      {
        return _button2DialogResult;
      }

      set
      {
        if (CanSetButtonPropertyValue)
        {
          _button2DialogResult = value;
        }
      }
    }

    /// <summary>
    /// Gets or sets the text displayed on the <see cref="InfoDialog.Info2Button"/>.
    /// </summary>
    public string Button2Text
    {
      get
      {
        return _button2Text;
      }

      set
      {
        if (CanSetButtonPropertyValue)
        {
          _button2Text = value;
        }
      }
    }

    /// <summary>
    /// Gets or sets the dialog result assigned to the <see cref="InfoDialog.Info3Button"/>.
    /// </summary>
    public DialogResult Button3DialogResult
    {
      get
      {
        return Button3IsMoreInfo
          ? DialogResult.None
          : _button3DialogResult;
      }

      set
      {
        if (CanSetButtonPropertyValue && !Button3IsMoreInfo)
        {
          _button3DialogResult = value;
        }
      }
    }

    /// <summary>
    /// Gets or sets the text displayed on the <see cref="InfoDialog.Info3Button"/>.
    /// </summary>
    public string Button3Text
    {
      get
      {
        return Button3IsMoreInfo
          ? MoreInfoCollapsedButtonText
          : _button3Text;
      }

      set
      {
        if (CanSetButtonPropertyValue && !Button3IsMoreInfo)
        {
          _button3Text = value;
        }
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="InfoDialog.Info3Button"/> is treated as the <see cref="InfoDialog.MoreInfoButton"/>.
    /// </summary>
    public bool Button3IsMoreInfo
    {
      get
      {
        return _button3IsMoreInfo;
      }

      set
      {
        _button3IsMoreInfo = value && ButtonsLayout.Is2Button() && LeftAreaControl != LeftAreaControlType.MoreInfoButton;
      }
    }

    /// <summary>
    /// Gets or sets the default button of the form which will be automatically selected after a specific time.
    /// </summary>
    public InfoDialog.DefaultButtonType DefaultButton { get; set; }

    /// <summary>
    /// Gets or sets the time in seconds after which the default button's click event will be automatically fired.
    /// </summary>
    public int DefaultButtonTimeout { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="ButtonsLayoutType"/> used to specify the buttons available to the <see cref="InfoDialog"/>.
    /// </summary>
    public ButtonsLayoutType ButtonsLayout
    {
      get
      {
        return _buttonsLayout;
      }

      set
      {
        _buttonsLayout = value;
        SetPropertiesForInfoType();
        ResetButton3IsMoreInfoValue();
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the command area left side checkbox is checked.
    /// </summary>
    public bool LeftAreaCheckBoxChecked { get; set; }

    /// <summary>
    /// Gets or sets the text used for the command area left side checkbox.
    /// </summary>
    public string LeftAreaCheckBoxText { get; set; }

    /// <summary>
    /// Gets or sets the dictionary used as a data source for the command area left side combobox.
    /// </summary>
    public Dictionary<string, string> LeftAreaComboBoxDataSource { get; set; }

    /// <summary>
    /// Gets or sets a fixed width for the command area left side combobox. If <c>0</c> or negative it adapts to the size of its contents.
    /// </summary>
    public int LeftAreaComboBoxWidth { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="LeftAreaControlType"/> used to specify the control to display at the left of the command area.
    /// </summary>
    public LeftAreaControlType LeftAreaControl
    {
      get
      {
        return _leftAreaControl;
      }

      set
      {
        _leftAreaControl = value;
        ResetButton3IsMoreInfoValue();
      }
    }

    /// <summary>
    /// Gets or sets the text displayed on the More Information button when the dialog is collapsed.
    /// </summary>
    public string MoreInfoCollapsedButtonText { get; set; }

    /// <summary>
    /// Gets or sets the text displayed on the More Information button when the dialog is expanded.
    /// </summary>
    public string MoreInfoExpandedButtonText { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="InfoDialog.Info1Button"/>, <see cref="InfoDialog.Info2Button"/> and <see cref="InfoDialog.Info3Button"/> widths are maintained the same.
    /// </summary>
    public bool SameButtonWidths { get; set; }

    /// <summary>
    /// Gets a value indicating whether a button text can be overriden from its default.
    /// </summary>
    /// <returns></returns>
    private bool CanSetButtonPropertyValue
    {
      get
      {
        return ButtonsLayout.IsGeneric() || AllowOverridingOfNonGenericLayoutType;
      }
    }

    #endregion Properties

    /// <summary>
    /// Resets the value of the <see cref="_button3IsMoreInfo"/> field.
    /// </summary>
    private void ResetButton3IsMoreInfoValue()
    {
      if ((!_buttonsLayout.Is2Button() || _leftAreaControl == LeftAreaControlType.MoreInfoButton) &&  _button3IsMoreInfo)
      {
        _button3IsMoreInfo = false;
      }
    }

    /// <summary>
    /// Sets the button properties accordingly to the given <see cref="ButtonsLayoutType"/>.
    /// </summary>
    private void SetPropertiesForInfoType()
    {
      switch (ButtonsLayout)
      {
        case ButtonsLayoutType.BackOnly:
          _button1Text = Resources.BackButtonText;
          _button1DialogResult = DialogResult.Cancel;
          break;

        case ButtonsLayoutType.OkOnly:
          _button1Text = Resources.OkButtonDefaultText;
          _button1DialogResult = DialogResult.OK;
          break;

        case ButtonsLayoutType.OkCancel:
          _button1Text = Resources.CancelButtonDefaultText;
          _button2Text = Resources.OkButtonDefaultText;
          _button1DialogResult = DialogResult.Cancel;
          _button2DialogResult = DialogResult.OK;
          break;

        case ButtonsLayoutType.YesNo:
          _button1Text = Resources.NoButtonText;
          _button2Text = Resources.YesButtonText;
          _button1DialogResult = DialogResult.No;
          _button2DialogResult = DialogResult.Yes;
          break;

        case ButtonsLayoutType.YesNoCancel:
          _button1Text = Resources.CancelButtonDefaultText;
          _button2Text = Resources.NoButtonText;
          _button3Text = Resources.YesButtonText;
          _button1DialogResult = DialogResult.Cancel;
          _button2DialogResult = DialogResult.No;
          _button3DialogResult = DialogResult.Yes;
          break;
      }
    }
  }
}
