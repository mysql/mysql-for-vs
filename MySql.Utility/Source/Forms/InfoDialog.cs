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
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using MySql.Utility.Classes;
using MySql.Utility.Classes.VisualStyles;
using MySql.Utility.Structs;

namespace MySql.Utility.Forms
{
  /// <summary>
  /// Generic Information Dialog to show information to users, this can be used as a OK, OK/Cancel, Yes/No, Yes/No/Cancel or a generic dialog with 1-3 buttons.
  /// </summary>
  public partial class InfoDialog : AutoStyleableBaseDialog
  {
    #region Constants

    /// <summary>
    /// The buttons minimum width in pixels.
    /// </summary>
    private const int BUTTON_MIN_WIDTH = 75;

    /// <summary>
    /// The buttons text padding to left and right sides of the button text, measured in pixels.
    /// </summary>
    private const int BUTTON_TEXT_PADDING = 20;

    /// <summary>
    /// The buttons margin separating them, measured in pixels.
    /// </summary>
    private const int BUTTONS_MARGIN = 6;

    /// <summary>
    /// Collapsed dialog height in pixels
    /// </summary>
    private const int COLLAPSED_HEIGHT = 215;

    /// <summary>
    /// Default minimum dialog width in pixels
    /// </summary>
    private const int DEFAULT_MIN_WIDTH = 580;

    /// <summary>
    /// Expanded dialog height in pixels
    /// </summary>
    private const int EXPANDED_HEIGHT = 350;

    /// <summary>
    /// Step for decreasing the font size in points.
    /// </summary>
    private const float FONT_SIZE_STEP_IN_POINTS = 0.25F;

    /// <summary>
    /// Step for increasing or decreasing the dialog width in pixels.
    /// </summary>
    private const int FORM_WIDTH_STEP_IN_PIXELS = 10;

    /// <summary>
    /// Number of steps of form width increasement after which font reduction takes place.
    /// </summary>
    private const int FORM_WIDTH_STEPS_TO_SWITCH_TO_FONT_REDUCTION = 10;

    /// <summary>
    /// Default height for details and sub-details labels, in pixels.
    /// </summary>
    private const int LABELS_DEFAULT_HEIGHT = 15;

    /// <summary>
    /// Minimum font size in points used for labels.
    /// </summary>
    private const float MIN_FONT_SIZE_IN_POINTS = 6.0F;

    /// <summary>
    /// Screen ratio to multiply by the screen width to get the max width for the dialog.
    /// </summary>
    private const int SCREEN_WIDTH_PERCENTAGE_FOR_MAX_WIDTH = 95;

    #endregion Constants

    #region Fields

    /// <summary>
    /// The current display screen.
    /// </summary>
    private Screen _currentDisplay;

    /// <summary>
    /// The ellapsed seconds the dialog has been shown to compare against the default seconds for the default button auto-selection.
    /// </summary>
    private int _defaultButtonEllapsedSeconds;

    /// <summary>
    /// The data source for the command area left side combobox.
    /// </summary>
    private Dictionary<string, string> _infoComboBoxDataSource;

    /// <summary>
    /// Minimum dialog width in pixels
    /// </summary>
    private int _minimumWidth;

    /// <summary>
    /// Flag indicating whether the dialog buttons must be repositioned when their text changes.
    /// </summary>
    private bool _repositionButtons;

    /// <summary>
    /// Action to perform on text that gets truncated because it does not fit within the label controls.
    /// </summary>
    private FitTextsAction _truncatedTextAction;

    #endregion Fields

    /// <summary>
    /// Initializes a new instance of the <see cref="InfoDialog"/> class.
    /// </summary>
    /// <param name="properties">A <see cref="InfoDialogProperties"/> with properties to initialize the dialog.</param>
    public InfoDialog(InfoDialogProperties properties = null)
    {
      _defaultButtonEllapsedSeconds = 0;
      _infoComboBoxDataSource = null;
      _minimumWidth = DEFAULT_MIN_WIDTH;
      _repositionButtons = true;
      if (properties == null)
      {
        properties = new InfoDialogProperties();
      }

      InitializeComponent();

      SetProperties(properties);
      var moreInfoAlignmentButton = MoreInfoWidthAlignedWithButton1 ? Info1Button : Info2Button;
      MoreInfoTextBox.Width = moreInfoAlignmentButton.Location.X + moreInfoAlignmentButton.Width - MoreInfoTextBox.Location.X;
    }

    #region Enumerations

    /// <summary>
    /// Specifies the default button of the form which will be automatically selected after a specific time.
    /// </summary>
    public enum DefaultButtonType
    {
      /// <summary>
      /// The default is the <see cref="InfoDialog.Info1Button"/>.
      /// </summary>
      Button1,

      /// <summary>
      /// The default is the <see cref="InfoDialog.Info2Button"/>.
      /// </summary>
      Button2,

      /// <summary>
      /// The default is the <see cref="InfoDialog.Info3Button"/>.
      /// </summary>
      Button3,

      /// <summary>
      /// No default button.
      /// </summary>
      None
    }

    /// <summary>
    /// Specifies the action to perform on texts that do not fit within the dialog.
    /// </summary>
    public enum FitTextsAction
    {
      /// <summary>
      /// Increases the dialog width and decreases the font size of texts that do not fit in the dialog balancing the strategy.
      /// </summary>
      BalancedStrategy,

      /// <summary>
      /// Decreases the font size of texts that do not fit in the dialog.
      /// </summary>
      DecreaseFontSize,

      /// <summary>
      /// Increases the dialog width to the point where the text fits without exceeding the screen width.
      /// </summary>
      IncreaseDialogWidth,

      /// <summary>
      /// Do nothing for texts that do not fit in the dialog so they are truncated.
      /// </summary>
      TruncateText
    }

    /// <summary>
    /// Specifies the type of information the dialog will display to users.
    /// </summary>
    public enum InfoType
    {
      /// <summary>
      /// Information about a successful operation.
      /// </summary>
      Success,

      /// <summary>
      /// Information about an error ocurred during an operation.
      /// </summary>
      Error,

      /// <summary>
      /// Information about warnings generated during a successful operation.
      /// </summary>
      Warning,

      /// <summary>
      /// Generic information.
      /// </summary>
      Info
    }

    #endregion Enumerations

    #region Static Properties

    /// <summary>
    /// Gets or sets the icon to be displayed in <see cref="InfoDialog"/> dialogs, set by consumer applications.
    /// </summary>
    public static Icon ApplicationIcon { get; set; }

    /// <summary>
    /// Gets or sets the consumer application name, displayed in the title bar of <see cref="InfoDialog"/> dialogs.
    /// </summary>
    public static string ApplicationName { get; set; }

    /// <summary>
    /// Gets or sets the default text displayed on the <see cref="Info1Button"/>.
    /// </summary>
    public static string DefaultButton1Text { get; set; }

    /// <summary>
    /// Gets or sets the default text displayed on the <see cref="Info2Button"/>.
    /// </summary>
    public static string DefaultButton2Text { get; set; }

    /// <summary>
    /// Gets or sets the default text displayed on the <see cref="Info3Button"/>.
    /// </summary>
    public static string DefaultButton3Text { get; set; }

    /// <summary>
    /// Gets or sets the error logo to be displayed in <see cref="InfoDialog"/> dialogs, set by consumer applications.
    /// </summary>
    public static Image ErrorLogo { get; set; }

    /// <summary>
    /// Gets or sets the information logo to be displayed in <see cref="InfoDialog"/> dialogs, set by consumer applications.
    /// </summary>
    public static Image InformationLogo { get; set; }

    /// <summary>
    /// Gets or sets the success logo to be displayed in <see cref="InfoDialog"/> dialogs, set by consumer applications.
    /// </summary>
    public static Image SuccessLogo { get; set; }

    /// <summary>
    /// Gets or sets the warning logo to be displayed in <see cref="InfoDialog"/> dialogs, set by consumer applications.
    /// </summary>
    public static Image WarningLogo { get; set; }

    #endregion Static Properties

    #region Properties

    /// <summary>
    /// Gets or sets the dialog result assigned to the <see cref="Info1Button"/>.
    /// </summary>
    [Category("Behavior"), DefaultValue(DialogResult.None), Description("The dialog result assigned to Button1.")]
    public DialogResult Button1DialogResult
    {
      get => Info1Button?.DialogResult ?? DialogResult.None;
      set
      {
        if (Info1Button == null)
        {
          return;
        }

        Info1Button.DialogResult = value;
      }
    }

    /// <summary>
    /// Gets or sets the text displayed on the <see cref="Info1Button"/>.
    /// </summary>
    [Category("Appearance"), DefaultValue("OK"), Description("The text displayed on the Button1 button.")]
    public string Button1Text { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="Info2Button"/> is displayed in the dialog.
    /// </summary>
    [Category("Appearance"), DefaultValue(false), Description("Indicates if the Button2 button is displayed in the dialog.")]
    public bool Button2Available
    {
      get => Info2Button != null && Info2Button.Visible;
      set
      {
        if (Info2Button == null)
        {
          return;
        }

        Info2Button.Visible = value;
      }
    }

    /// <summary>
    /// Gets or sets the dialog result assigned to the <see cref="Info2Button"/>.
    /// </summary>
    [Category("Behavior"), DefaultValue(DialogResult.None), Description("The dialog result assigned to Button2.")]
    public DialogResult Button2DialogResult
    {
      get => Info2Button?.DialogResult ?? DialogResult.None;
      set
      {
        if (Info2Button == null)
        {
          return;
        }

        Info2Button.DialogResult = value;
      }
    }

    /// <summary>
    /// Gets or sets the text displayed on the <see cref="Info2Button"/>.
    /// </summary>
    [Category("Appearance"), DefaultValue("Cancel"), Description("The text displayed on the Button2 button.")]
    public string Button2Text { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="Info3Button"/> is displayed in the dialog.
    /// </summary>
    [Category("Appearance"), DefaultValue(false), Description("Indicates if the Button3 is displayed in the dialog.")]
    public bool Button3Available
    {
      get => Info3Button != null && Info3Button.Visible;
      set
      {
        if (Info3Button == null)
        {
          return;
        }

        Info3Button.Visible = value;
      }
    }

    /// <summary>
    /// Gets or sets the dialog result assigned to the <see cref="Info3Button"/>.
    /// </summary>
    [Category("Behavior"), DefaultValue(DialogResult.None), Description("The dialog result assigned to Button3.")]
    public DialogResult Button3DialogResult
    {
      get => Info3Button?.DialogResult ?? DialogResult.None;
      set
      {
        if (Info3Button == null)
        {
          return;
        }

        Info3Button.DialogResult = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="Info3Button"/> is treated as the <see cref="MoreInfoButton"/>.
    /// </summary>
    [Category("Behavior"), DefaultValue(false), Description("Flag indicating whether the Info3Button is treated as the MoreInfoButton.")]
    public bool Button3IsMoreInfo { get; set; }

    /// <summary>
    /// Gets or sets the text displayed on the <see cref="Info3Button"/>.
    /// </summary>
    [Category("Appearance"), DefaultValue("Other"), Description("The text displayed on the Button3.")]
    public string Button3Text { get; set; }

    /// <summary>
    /// Gets or sets the default button of the form which will be automatically selected after a specific time.
    /// </summary>
    [Category("Behavior"), DefaultValue(DefaultButtonType.None), Description("The default button of the form which will be automatically selected after a specific time.")]
    public DefaultButtonType DefaultButton { get; set; }

    /// <summary>
    /// Gets or sets the time in seconds after which the default button's click event will be automatically fired.
    /// </summary>
    [Category("Behavior"), DefaultValue(10), Description("The time in seconds after which the default button's click event will be automatically fired.")]
    public int DefaultButtonTimeout { get; set; }

    /// <summary>
    /// Gets or sets the text that optionally further describes information details to users, supports 1 line of text.
    /// </summary>
    [Category("Appearance"), DefaultValue("Sub detail text."), Description("The text that optionally further describes information details to users, supports 1 line of text.")]
    public string DetailSubText
    {
      get => DetailSubLabel.Text;

      set
      {
        DetailSubLabel.Text = value;
        DetailSubLabel.Visible = !string.IsNullOrEmpty(DetailSubLabel.Text);
        DetailLabel.Height = LABELS_DEFAULT_HEIGHT * (DetailSubLabel.Visible ? 1 : 2);
      }
    }

    /// <summary>
    /// Gets or sets the text describing information details to the users, supports 2 lines of text.
    /// </summary>
    [Category("Appearance"), DefaultValue("Information detail text."), Description("The text describing information details to the users, supports 2 lines of text.")]
    public string DetailText
    {
      get => DetailLabel.Text;
      set => DetailLabel.Text = value;
    }

    /// <summary>
    /// Gets a value indicating whether the dialog is expanded to show the More Information text box.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool ExpandedState { get; private set; }

    /// <summary>
    /// Gets or sets the text for the command area left side checkbox.
    /// </summary>
    [Category("Appearance"), DefaultValue("Do not show this dialog again for this session"), Description("The text displayed on the Button3.")]
    public string InfoCheckBoxText
    {
      get => InfoCheckBox.Text;
      set
      {
        InfoCheckBox.Text = value;
        InfoCheckBox.AccessibleName = value;
      }
    }

    /// <summary>
    /// Gets or sets the current value for the command area left side checkbox.
    /// </summary>
    [Category("Behavior"), DefaultValue(false), Description("The current value of an optional checkbox to get user input.")]
    public bool InfoCheckBoxChecked
    {
      get => InfoCheckBox.Checked;
      set => InfoCheckBox.Checked = value;
    }

    /// <summary>
    /// Gets or sets the data source for the command area left side combobox.
    /// </summary>
    [Category("Behavior"), DefaultValue(null), Description("The data source for the command area left side combobox.")]
    public Dictionary<string, string> InfoComboBoxDataSource
    {
      get => _infoComboBoxDataSource;
      set
      {
        _infoComboBoxDataSource = value;
        if (_infoComboBoxDataSource == null || _infoComboBoxDataSource.Count == 0)
        {
          return;
        }

        InfoComboBox.DataSource = new BindingSource(_infoComboBoxDataSource, null);
        InfoComboBox.DisplayMember = "Key";
        InfoComboBox.ValueMember = "Key";
        InfoComboBox.DropDownWidth = _infoComboBoxDataSource.GetMaxElementLength(false, InfoComboBox.Font, (SystemInformation.VerticalScrollBarWidth * 2) + 2);
      }
    }

    /// <summary>
    /// Gets the selected index of the command area left side combobox.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int InfoComboBoxSelectedIndex => InfoComboBox.SelectedIndex;

    /// <summary>
    /// Gets the selected value of the command area left side combobox.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string InfoComboBoxSelectedValue
    {
      get
      {
        var kvp = InfoComboBox.SelectedItem as KeyValuePair<string, string>?;
        return kvp?.Key;
      }
    }

    /// <summary>
    /// Gets or sets a fixed width for the command area left side combobox. If <c>0</c> or negative it adapts to the size of its contents.
    /// </summary>
    [Category("Appearance"), DefaultValue(null), Description("A fixed width for the command area left side combobox. If 0 or negative it adapts to the size of its contents.")]
    public int InfoComboBoxWidth
    {
      get => InfoComboBox.Width;
      set
      {
        if (value <= 0 && InfoComboBoxDataSource == null)
        {
          return;
        }

        InfoComboBox.Width = value <= 0
          ? InfoComboBoxDataSource.GetMaxElementLength(true, InfoComboBox.Font, (SystemInformation.VerticalScrollBarWidth * 2) + 2)
          : value;
      }
    }

    /// <summary>
    /// Gets or sets the image displayed in the dialog.
    /// </summary>
    [Category("Appearance"), DefaultValue(null), Description("The image displayed in the dialog.")]
    public Image LogoImage
    {
      get => LogoPictureBox.Image;
      set => LogoPictureBox.Image = value;
    }

    /// <summary>
    /// Gets or sets the text displayed on the More Information button when the dialog is collapsed.
    /// </summary>
    [Category("Appearance"), DefaultValue("Show Details"), Description("The text displayed on the More Information button when the dialog is collapsed.")]
    public string MoreInfoCollapsedButtonText { get; set; }

    /// <summary>
    /// Gets or sets the text displayed on the More Information button when the dialog is expanded.
    /// </summary>
    [Category("Appearance"), DefaultValue("Hide Details"), Description("The text displayed on the More Information button when the dialog is expanded.")]
    public string MoreInfoExpandedButtonText { get; set; }

    /// <summary>
    /// Gets or sets the extended text users can see in the More Information text box.
    /// </summary>
    [Category("Appearance"), DefaultValue(""), Description("The extended text users can see in the More Information text box.")]
    public string MoreInfoText
    {
      get => MoreInfoTextBox.Text;
      set => MoreInfoTextBox.Text = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the More Information text box width is aligned with the right border of the <see cref="Info1Button"/> or the right corner of the <see cref="Info2Button"/>.
    /// </summary>
    [Category("Appearance"), DefaultValue(false), Description("Indicates if the More Information text box width is aligned with the right border of the Button1 or the right corner of the Button2.")]
    public bool MoreInfoWidthAlignedWithButton1 { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="Info1Button"/>, <see cref="Info2Button"/> and <see cref="Info3Button"/> widths are maintained the same.
    /// </summary>
    [Category("Appearance"), DefaultValue(false), Description("Indicates whether the Button1, Button2 and Button3 widths are maintained the same.")]
    public bool SameButtonWidths { get; set; }

    /// <summary>
    /// Gets or sets the title text of the dialog, displayed in blue color and with a bigger font.
    /// </summary>
    [Category("Appearance"), DefaultValue("Title Text"), Description("The title text of the dialog, displayed in blue color and with a bigger font.")]
    public string TitleText
    {
      get => TitleLabel.Text;
      set => TitleLabel.Text = value;
    }

    /// <summary>
    /// Gets or sets the action to perform on text that gets truncated because it does not fit within the label controls.
    /// </summary>
    [Category("Behavior"), DefaultValue(FitTextsAction.TruncateText), Description("The action to perform on text that gets truncated because it does not fit within the label controls.")]
    public FitTextsAction TruncatedTextAction
    {
      get => _truncatedTextAction;
      set
      {
        _truncatedTextAction = value;
        FitLabelTexts();
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the More Information text box word wraps the text.
    /// </summary>
    [Category("Appearance"), DefaultValue(false), Description("Indicates if the More Information text box word wraps the text.")]
    public bool WordWrapMoreInfo
    {
      get => MoreInfoTextBox.WordWrap;
      set => MoreInfoTextBox.WordWrap = value;
    }

    /// <summary>
    /// Gets a value indicating whether the width of the dialog can be increased or if it reached the limit for the current display.
    /// </summary>
    private bool CanIncreaseWidth => _minimumWidth < MaximumWidth;

    /// <summary>
    /// Gets a centered location for this dialog.
    /// </summary>
    private Point CenterScreenLocation => new Point(CurrentDisplay.Bounds.Location.X + (CurrentDisplay.Bounds.Width / 2) - (Width / 2), CurrentDisplay.Bounds.Location.Y + (CurrentDisplay.Bounds.Height / 2) - (Height / 2));

    /// <summary>
    /// Gets the current display screen.
    /// </summary>
    private Screen CurrentDisplay => _currentDisplay ?? (_currentDisplay = Screen.FromControl(this));

    /// <summary>
    /// The maximum width of the dialog given by multiplying the actual current display width by a factor of <see cref="SCREEN_WIDTH_PERCENTAGE_FOR_MAX_WIDTH"/>.
    /// </summary>
    private int MaximumWidth => CurrentDisplay.Bounds.Width * SCREEN_WIDTH_PERCENTAGE_FOR_MAX_WIDTH / 100;

    #endregion Properties

    /// <summary>
    /// Shows information to users through an <see cref="InfoDialog"/> dialog.
    /// </summary>
    /// <param name="properties">A <see cref="InfoDialogProperties"/> with properties to initialize the dialog.</param>
    /// <returns>An <see cref="InfoDialogResult"/>.</returns>
    public static InfoDialogResult ShowDialog(InfoDialogProperties properties)
    {
      var activeForm = Utilities.GetActiveForm();
      if (activeForm != null
          && activeForm.InvokeRequired)
      {
        Func<InfoDialogProperties, InfoDialogResult> safeShowDialog = ShowDialogSafe;
        return (InfoDialogResult)activeForm.Invoke(safeShowDialog, properties);
      }

      return ShowDialogSafe(properties);
    }

    /// <summary>
    /// Shows information to users through an <see cref="InfoDialog"/> dialog.
    /// </summary>
    /// <param name="properties">A <see cref="InfoDialogProperties"/> with properties to initialize the dialog.</param>
    /// <returns>An <see cref="InfoDialogResult"/>.</returns>
    private static InfoDialogResult ShowDialogSafe(InfoDialogProperties properties)
    {
      InfoDialogResult idr;
      using (var infoDialog = new InfoDialog(properties))
      {
        var dr = infoDialog.ShowDialog();
        idr = new InfoDialogResult(dr, infoDialog.InfoCheckBoxChecked, infoDialog.InfoComboBoxSelectedIndex, infoDialog.InfoComboBoxSelectedValue);
      }

      return idr;
    }

    /// <summary>
    /// Adjusts the Accept and Cancel buttons text, for use with the default button timeout.
    /// </summary>
    private void AdjustDialogButtonsForDefaultButtonTimeout()
    {
      Button defaultButton;
      string buttonText;

      switch (DefaultButton)
      {
        case DefaultButtonType.Button1:
          defaultButton = Info1Button;
          buttonText = Button1Text;
          break;

        case DefaultButtonType.Button2:
          defaultButton = Info2Button;
          buttonText = Button2Text;
          break;

        case DefaultButtonType.Button3:
          defaultButton = Info3Button;
          buttonText = Button3Text;
          break;

        default:
          DefaultButtonTimer.Stop();
          return;
      }

      if (_defaultButtonEllapsedSeconds == DefaultButtonTimeout)
      {
        DefaultButtonTimer.Stop();
        defaultButton.PerformClick();
      }
      else
      {
        int timeoutSeconds = DefaultButtonTimeout - _defaultButtonEllapsedSeconds;
        defaultButton.Text = buttonText + (timeoutSeconds > 0 ? $" ({timeoutSeconds})" : string.Empty);
      }
    }

    /// <summary>
    /// Expands or collapses the dialog to show/hide the extended information text box
    /// </summary>
    private void ChangeHeight()
    {
      FormBorderStyle = ExpandedState ? FormBorderStyle.Sizable : FormBorderStyle.FixedDialog;
      MoreInfoButton.Text = ExpandedState ? MoreInfoExpandedButtonText : MoreInfoCollapsedButtonText;
      MoreInfoTextBox.Visible = ExpandedState;
      var standardDpiHeight = ExpandedState ? EXPANDED_HEIGHT : COLLAPSED_HEIGHT;
      var currentDpiHeight = HandleDpiSizeConversions
        ? (int) (standardDpiHeight * this.GetDpiScaleY())
        : standardDpiHeight;
      var currentDpiWidth = HandleDpiSizeConversions
        ? (int) (_minimumWidth * this.GetDpiScaleX())
        : _minimumWidth;
      Size = MinimumSize = new Size(currentDpiWidth, currentDpiHeight);
      MaximumSize = ExpandedState ? new Size(0, 0) : MinimumSize;
      if (Button3IsMoreInfo)
      {
        Info3Button.Text = MoreInfoButton.Text;
        RepositionButtons();
      }

      CheckCommandAreaControlsForCollisions();
      Location = CenterScreenLocation;
    }

    /// <summary>
    /// Checks if there is a collision between a control at the left side of the command area and its nearest visible right button.
    /// </summary>
    private void CheckCommandAreaControlsForCollisions()
    {
      Control leftControl = null;
      if (MoreInfoButton.Visible)
      {
        leftControl = MoreInfoButton;
      }
      else if (InfoCheckBox.Visible)
      {
        leftControl = InfoCheckBox;
      }
      else if (InfoComboBox.Visible)
      {
        leftControl = InfoComboBox;
      }

      if (leftControl == null)
      {
        return;
      }

      var leftControlLowerRightPoint = new Point(leftControl.Location.X + leftControl.Width, leftControl.Location.Y + leftControl.Height);
      var nearestVisibleButton = Info3Button.Visible ? Info3Button : (Info2Button.Visible ? Info2Button : Info1Button);
      if (leftControlLowerRightPoint.X < nearestVisibleButton.Location.X)
      {
        // No collision
        return;
      }

      // Collision detected
      var deltaWidthPlusPadding = leftControlLowerRightPoint.X - nearestVisibleButton.Location.X + BUTTON_TEXT_PADDING;
      if (CanIncreaseWidth)
      {
        IncreaseDialogWidth(deltaWidthPlusPadding);
      }
      else
      {
        leftControl.Width -= deltaWidthPlusPadding;
      }
    }

    /// <summary>
    /// Checks if a label contains text that does not fit in its drawing rectangle area and adjusts its font or the dialog width to make it fit.
    /// </summary>
    /// <param name="label"><see cref="Label"/> control to check for overflowing text.</param>
    /// <param name="singleLineText">Flag indicating if the label text is on a single line.</param>
    private void CheckLabelTextForTextOverflow(Label label, bool singleLineText = true)
    {
      switch (TruncatedTextAction)
      {
        case FitTextsAction.BalancedStrategy:
          PerformBalancedStrategy(label, singleLineText);
          break;

        case FitTextsAction.DecreaseFontSize:
          DecreaseLabelFontSize(label, singleLineText, false);
          break;

        case FitTextsAction.IncreaseDialogWidth:
          IncreaseDialogWidth(label, singleLineText);
          break;
      }
    }

    /// <summary>
    /// Decreases the given label's font size until its text fits within the label display area.
    /// </summary>
    /// <param name="label"><see cref="Label"/> control to check for overflowing text.</param>
    /// <param name="singleLineText">Flag indicating if the label text is on a single line.</param>
    /// <param name="stopAtTextSizeChange">Flag indicating if the font size should be reduced only to the point where the label's text size changes.</param>
    private void DecreaseLabelFontSize(Label label, bool singleLineText, bool stopAtTextSizeChange)
    {
      int texPerLineWidth = label.GetWidthOfTextSplitInLines(singleLineText ? 1 : 2);
      int overflowingWidth = texPerLineWidth - label.Width;
      if (overflowingWidth <= 0)
      {
        return;
      }

      int originalPreferredWidth = label.PreferredWidth;
      float newFontSize = label.Font.SizeInPoints;
      label.SuspendLayout();
      while (overflowingWidth > 0 && newFontSize >= MIN_FONT_SIZE_IN_POINTS)
      {
        newFontSize -= FONT_SIZE_STEP_IN_POINTS;
        label.Font = new Font(label.Font.FontFamily, newFontSize, label.Font.Style, label.Font.Unit);
        bool labelTextSizeChanged = label.PreferredWidth != originalPreferredWidth;
        if (stopAtTextSizeChange && labelTextSizeChanged)
        {
          break;
        }

        texPerLineWidth = label.PreferredWidth / (singleLineText ? 1 : 2);
        overflowingWidth = texPerLineWidth - label.Width;
      }

      label.ResumeLayout();
    }

    /// <summary>
    /// Event delegate method fired when the <see cref="DefaultButtonTimer"/> timer's interval ellapses.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void DefaultButtonTimer_Tick(object sender, EventArgs e)
    {
      _defaultButtonEllapsedSeconds++;
      AdjustDialogButtonsForDefaultButtonTimeout();
    }

    /// <summary>
    /// Checks if dialog label texts fits within the dialog width and fits them given the <see cref="FitTextsAction"/> in <see cref="TruncatedTextAction"/>.
    /// </summary>
    private void FitLabelTexts()
    {
      if (TruncatedTextAction == FitTextsAction.TruncateText)
      {
        return;
      }

      CheckLabelTextForTextOverflow(TitleLabel);
      CheckLabelTextForTextOverflow(DetailLabel, !string.IsNullOrEmpty(DetailSubText));
      CheckLabelTextForTextOverflow(DetailSubLabel);
    }

    /// <summary>
    /// Increases the dialog's width as per the given value as possible.
    /// </summary>
    /// <param name="increasingWidth">The width, in pixels, to increase.</param>
    private void IncreaseDialogWidth(int increasingWidth)
    {
      if (increasingWidth <= 0)
      {
        return;
      }

      _minimumWidth = Math.Min(Width + increasingWidth, MaximumWidth);
      ChangeHeight();
    }

    /// <summary>
    /// Increases the dialog's width until the text in the given label control fits.
    /// </summary>
    /// <param name="label"><see cref="Label"/> control to check for overflowing text.</param>
    /// <param name="singleLineText">Flag indicating if the label text is on a single line.</param>
    private void IncreaseDialogWidth(Label label, bool singleLineText)
    {
      int textPerLineWidth = label.GetWidthOfTextSplitInLines(singleLineText ? 1 : 2);
      int overflowingWidth = textPerLineWidth - label.Width;
      IncreaseDialogWidth(overflowingWidth);
    }

    /// <summary>
    /// Event delegate method fired when the <see cref="Info3Button"/> is clicked.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void Info3Button_Click(object sender, EventArgs e)
    {
      if (!Button3IsMoreInfo)
      {
        return;
      }

      MoreInfoButton_Click(MoreInfoButton, e);
    }

    /// <summary>
    /// Event delegate method fired when the <see cref="InfoComboBox"/> combo box's draws each internal item.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void InfoComboBox_DrawItem(object sender, DrawItemEventArgs e)
    {
      e.DrawBackground();
      if (e.Index >= 0)
      {
        var comboItem = InfoComboBox.Items[e.Index];
        string itemText = comboItem is KeyValuePair<string, string>
          ? (InfoComboBox.DroppedDown ? ((KeyValuePair<string, string>) comboItem).Value : ((KeyValuePair<string, string>)comboItem).Key)
          : comboItem.ToString();
        var graphics = e.Graphics;
        using (var backBrush = new SolidBrush(e.BackColor))
        {
          graphics.FillRectangle(backBrush, e.Bounds);
        }

        using (var foreBrush = new SolidBrush(e.ForeColor))
        {
          graphics.DrawString(itemText, e.Font, foreBrush, e.Bounds, StringFormat.GenericDefault);
        }
      }

      e.DrawFocusRectangle();
    }

    /// <summary>
    /// Event delegate method fired when the <see cref="InfoDialog"/> is loaded.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void InfoDialog_Load(object sender, EventArgs e)
    {
      SetupDialogButtonPropertiesAndTimeouts();
    }

    /// <summary>
    /// Event delegate method fired when the <see cref="InfoDialog"/> is first shown.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.</param>
    private void InfoDialog_Shown(object sender, EventArgs e)
    {
      ChangeHeight();
      if (DefaultButton != DefaultButtonType.None && DefaultButtonTimeout > 0)
      {
        DefaultButtonTimer.Start();
      }

      switch (DefaultButton)
      {
        case DefaultButtonType.Button1:
        case DefaultButtonType.None:
          Info1Button.Focus();
          break;

        case DefaultButtonType.Button2:
          Info2Button.Focus();
          break;

        case DefaultButtonType.Button3:
          Info3Button.Focus();
          break;
      }
    }

    /// <summary>
    /// Event delegate method fired when the <see cref="MoreInfoButton"/> button is clicked.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event arguments.t</param>
    private void MoreInfoButton_Click(object sender, EventArgs e)
    {
      ExpandedState = !ExpandedState;
      ChangeHeight();
    }

    /// <summary>
    /// Balances the increasing of the dialog width and the decreasing of label font size to make the label text fit.
    /// </summary>
    /// <param name="label"><see cref="Label"/> control to check for overflowing text.</param>
    /// <param name="singleLineText">Flag indicating if the label text is on a single line.</param>
    private void PerformBalancedStrategy(Label label, bool singleLineText)
    {
      int texPerLineWidth = label.GetWidthOfTextSplitInLines(singleLineText ? 1 : 2);
      int overflowingWidth = texPerLineWidth - label.Width;
      if (overflowingWidth <= 0)
      {
        return;
      }

      // Increase the dialog's width
      bool fontReduced = false;
      int widthIncreasementStep = 0;
      while (overflowingWidth > 0 && label.Font.SizeInPoints >= MIN_FONT_SIZE_IN_POINTS)
      {
        // Switch to font size decreasing after several steps of dialog width increasement.
        if (widthIncreasementStep == FORM_WIDTH_STEPS_TO_SWITCH_TO_FONT_REDUCTION)
        {
          widthIncreasementStep = 0;
          DecreaseLabelFontSize(label, singleLineText, true);
          fontReduced = true;
          texPerLineWidth = label.PreferredWidth / (singleLineText ? 1 : 2);
          overflowingWidth = texPerLineWidth - label.Width;
          continue;
        }

        _minimumWidth += FORM_WIDTH_STEP_IN_PIXELS;
        widthIncreasementStep++;
        ChangeHeight();
        texPerLineWidth = label.PreferredWidth / (singleLineText ? 1 : 2);
        overflowingWidth = texPerLineWidth - label.Width;
      }

      if (!fontReduced)
      {
        return;
      }

      // If the font size was reduced maybe the dialog width can be decreased a little
      while (overflowingWidth < 0)
      {
        _minimumWidth -= FORM_WIDTH_STEP_IN_PIXELS;
        ChangeHeight();
        texPerLineWidth = label.PreferredWidth / (singleLineText ? 1 : 2);
        overflowingWidth = texPerLineWidth - label.Width;
      }

      _minimumWidth += FORM_WIDTH_STEP_IN_PIXELS;
      ChangeHeight();
    }

    /// <summary>
    /// Repositions the dialog buttons based on the lengths of their texts.
    /// </summary>
    private void RepositionButtons()
    {
      if (!_repositionButtons)
      {
        return;
      }

      SizeF button1TextWidth = TextRenderer.MeasureText(Info1Button.Text, Info1Button.Font);
      SizeF button2TextWidth = TextRenderer.MeasureText(Info2Button.Text, Info2Button.Font);
      SizeF button3TextWidth = TextRenderer.MeasureText(Info3Button.Text, Info3Button.Font);
      SizeF moreInfoButtonWidth = TextRenderer.MeasureText(MoreInfoButton.Text, MoreInfoButton.Font);

      const int BUTTON_TOTAL_PADDING = BUTTON_TEXT_PADDING * 2;
      int button1NewWidth = Convert.ToInt32(Math.Max(BUTTON_MIN_WIDTH, button1TextWidth.Width + BUTTON_TOTAL_PADDING));
      int button2NewWidth = Convert.ToInt32(Math.Max(BUTTON_MIN_WIDTH, button2TextWidth.Width + BUTTON_TOTAL_PADDING));
      int button3NewWidth = Convert.ToInt32(Math.Max(BUTTON_MIN_WIDTH, button3TextWidth.Width + BUTTON_TOTAL_PADDING));
      int moreInfoButtonNewWidth = Convert.ToInt32(Math.Max(BUTTON_MIN_WIDTH, moreInfoButtonWidth.Width + BUTTON_TOTAL_PADDING));
      int buttonsSameNewWidth = Convert.ToInt32(Math.Max(Math.Max(button1NewWidth, button2NewWidth), button3NewWidth));
      Info1Button.Anchor = AnchorStyles.None;
      Info2Button.Anchor = AnchorStyles.None;
      Info3Button.Anchor = AnchorStyles.None;
      Info1Button.Width = SameButtonWidths ? buttonsSameNewWidth : button1NewWidth;
      Info2Button.Width = SameButtonWidths ? buttonsSameNewWidth : button2NewWidth;
      Info3Button.Width = SameButtonWidths ? buttonsSameNewWidth : button3NewWidth;
      MoreInfoButton.Width = moreInfoButtonNewWidth;
      Info1Button.Location = new Point(CommandAreaPanel.Width - (BUTTONS_MARGIN * 2) - Info1Button.Width, Info1Button.Location.Y);
      Info2Button.Location = new Point(Info1Button.Location.X - BUTTONS_MARGIN - Info2Button.Width, Info2Button.Location.Y);
      Info3Button.Location = new Point(Info2Button.Location.X - BUTTONS_MARGIN - Info3Button.Width, Info3Button.Location.Y);
      Info1Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      Info2Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      Info3Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
    }

    /// <summary>
    /// Sets the command area properties.
    /// </summary>
    /// <param name="commandAreaProperties">Properties for buttons and controls used in the command area of the <see cref="InfoDialog"/>.</param>
    private void SetCommandAreaProperties(CommandAreaProperties commandAreaProperties)
    {
      if (commandAreaProperties == null)
      {
        commandAreaProperties = new CommandAreaProperties(CommandAreaProperties.ButtonsLayoutType.OkOnly);
      }

      _repositionButtons = false;
      Button1Text = commandAreaProperties.Button1Text;
      Button1DialogResult = commandAreaProperties.Button1DialogResult;
      Button2Text = commandAreaProperties.Button2Text;
      Button2DialogResult = commandAreaProperties.Button2DialogResult;
      Button3Text = Button3IsMoreInfo
        ? (ExpandedState ? commandAreaProperties.MoreInfoExpandedButtonText : commandAreaProperties.MoreInfoCollapsedButtonText)
        : commandAreaProperties.Button3Text;
      Button3DialogResult = commandAreaProperties.Button3DialogResult;
      _repositionButtons = true;
      Button3IsMoreInfo = commandAreaProperties.Button3IsMoreInfo;
      DefaultButton = commandAreaProperties.DefaultButton;
      DefaultButtonTimeout = commandAreaProperties.DefaultButtonTimeout;
      InfoCheckBoxText = commandAreaProperties.LeftAreaCheckBoxText;
      InfoCheckBoxChecked = commandAreaProperties.LeftAreaCheckBoxChecked;
      InfoComboBoxDataSource = commandAreaProperties.LeftAreaComboBoxDataSource;
      InfoComboBoxWidth = commandAreaProperties.LeftAreaComboBoxWidth;
      MoreInfoCollapsedButtonText = commandAreaProperties.MoreInfoCollapsedButtonText;
      MoreInfoExpandedButtonText = commandAreaProperties.MoreInfoExpandedButtonText;
      MoreInfoButton.Text = ExpandedState ? MoreInfoExpandedButtonText : MoreInfoCollapsedButtonText;
      SameButtonWidths = commandAreaProperties.SameButtonWidths;

      switch (commandAreaProperties.ButtonsLayout)
      {
        case CommandAreaProperties.ButtonsLayoutType.BackOnly:
        case CommandAreaProperties.ButtonsLayoutType.OkOnly:
        case CommandAreaProperties.ButtonsLayoutType.Generic1Button:
          Button2Available = false;
          Button3Available = false;
          AcceptButton = Info1Button;
          CancelButton = Info1Button;
          break;

        case CommandAreaProperties.ButtonsLayoutType.Generic2Buttons:
        case CommandAreaProperties.ButtonsLayoutType.OkCancel:
        case CommandAreaProperties.ButtonsLayoutType.YesNo:
          Button2Available = true;
          Button3Available = commandAreaProperties.Button3IsMoreInfo;
          AcceptButton = Info2Button;
          CancelButton = Info1Button;
          break;

        case CommandAreaProperties.ButtonsLayoutType.Generic3Buttons:
        case CommandAreaProperties.ButtonsLayoutType.YesNoCancel:
          Button2Available = true;
          Button3Available = true;
          AcceptButton = commandAreaProperties.ButtonsLayout == CommandAreaProperties.ButtonsLayoutType.YesNoCancel
            ? Info3Button
            : null;
          CancelButton = Info1Button;
          break;
      }

      RepositionButtons();

      MoreInfoButton.Visible = commandAreaProperties.LeftAreaControl == CommandAreaProperties.LeftAreaControlType.MoreInfoButton && !string.IsNullOrEmpty(MoreInfoText);
      InfoCheckBox.Visible = commandAreaProperties.LeftAreaControl == CommandAreaProperties.LeftAreaControlType.InfoCheckBox && !string.IsNullOrEmpty(InfoCheckBoxText);
      InfoComboBox.Visible = commandAreaProperties.LeftAreaControl == CommandAreaProperties.LeftAreaControlType.InfoComboBox && InfoComboBoxDataSource != null;
    }

    /// <summary>
    /// Sets <see cref="InfoDialog"/> properties based on a given <see cref="InfoDialogProperties"/> object.
    /// </summary>
    /// <param name="properties"></param>
    private void SetProperties(InfoDialogProperties properties = null)
    {
      // Initialize the properties if none were sent as argument.
      if (properties == null)
      {
        properties = new InfoDialogProperties();
      }

      // Set the dialog's icon if possible if set in the ApplicationIcon
      if (ApplicationIcon != null)
      {
        Icon = ApplicationIcon;
        ShowIcon = true;
      }

      // Set properties dependent on the InfoType
      switch (properties.InfoType)
      {
        case InfoType.Error:
          LogoImage = properties.LogoImage ?? (ErrorLogo ?? Resources.SakilaLogo);
          Text =  string.IsNullOrEmpty(properties.TitleBarText)
            ? (string.IsNullOrEmpty(ApplicationName) ? Resources.ErrorText : ApplicationName)
            : properties.TitleBarText;
          break;

        case InfoType.Info:
          LogoImage = properties.LogoImage ??  (InformationLogo ?? Resources.SakilaLogo);
          Text = string.IsNullOrEmpty(properties.TitleBarText)
            ? (string.IsNullOrEmpty(ApplicationName) ? Resources.InformationText : ApplicationName)
            : properties.TitleBarText;
          break;

        case InfoType.Success:
          LogoImage = properties.LogoImage ?? (SuccessLogo ?? Resources.SakilaLogo);
          Text = string.IsNullOrEmpty(properties.TitleBarText)
            ? (string.IsNullOrEmpty(ApplicationName) ? Resources.SuccessText : ApplicationName)
            : properties.TitleBarText;
          break;

        case InfoType.Warning:
          LogoImage = properties.LogoImage ?? (WarningLogo ?? Resources.SakilaLogo);
          Text = string.IsNullOrEmpty(properties.TitleBarText)
            ? (string.IsNullOrEmpty(ApplicationName) ? Resources.WarningsText : ApplicationName)
            : properties.TitleBarText;
          break;
      }

      // Set other properties
      DetailSubText = properties.DetailSubText;
      DetailText = properties.DetailText;
      ExpandedState = properties.IsExpanded;
      MoreInfoText = properties.MoreInfoText;
      MoreInfoWidthAlignedWithButton1 = properties.MoreInfoWidthAlignedWithButton1;
      TitleText = properties.TitleText;
      TruncatedTextAction = properties.FitTextStrategy;
      WordWrapMoreInfo = properties.WordWrapMoreInfo;

      // Set command area properties
      SetCommandAreaProperties(properties.CommandAreaProperties);
    }

    /// <summary>
    /// Sets up the buttons width, font style and text.
    /// </summary>
    private void SetupDialogButtonPropertiesAndTimeouts()
    {
      Info1Button.Text = DefaultButton == DefaultButtonType.Button1
        ? $"{Button1Text} ({DefaultButtonTimeout})"
        : Button1Text;
      Info1Button.AccessibleName = Info1Button.Text;
      Info2Button.Text = DefaultButton == DefaultButtonType.Button2
        ? $"{Button2Text} ({DefaultButtonTimeout})"
        : Button2Text;
      Info2Button.AccessibleName = Info2Button.Text;
      Info3Button.Text = DefaultButton == DefaultButtonType.Button3
        ? $"{Button3Text} ({DefaultButtonTimeout})"
        : Button3Text;
      Info3Button.AccessibleName = Info3Button.Text;

      switch (DefaultButton)
      {
        case DefaultButtonType.Button1:
          Info1Button.Font = new Font(Info1Button.Font, FontStyle.Bold);
          break;

        case DefaultButtonType.Button2:
          if (Button2Available)
          {
            Info2Button.Font = new Font(Info2Button.Font, FontStyle.Bold);
          }

          break;

        case DefaultButtonType.Button3:
          if (Button3Available && !Button3IsMoreInfo)
          {
            Info3Button.Font = new Font(Info3Button.Font, FontStyle.Bold);
          }

          break;
      }

      RepositionButtons();
    }
  }
}