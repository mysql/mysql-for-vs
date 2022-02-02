// Copyright (c) 2015, 2016, Oracle and/or its affiliates. All rights reserved.
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
using MySql.Utility.Forms;

namespace MySql.Utility.Classes
{
  /// <summary>
  /// Defines properties used to create a new instance of the <see cref="InfoDialog"/> class.
  /// </summary>
  public class InfoDialogProperties
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="InfoDialogProperties"/> class.
    /// </summary>
    public InfoDialogProperties()
    {
      CommandAreaProperties = null;
      DetailSubText = null;
      DetailText = string.Empty;
      FitTextStrategy = InfoDialog.FitTextsAction.IncreaseDialogWidth;
      InfoType = InfoDialog.InfoType.Info;
      IsExpanded = false;
      LogoImage = null;
      MoreInfoText = null;
      MoreInfoWidthAlignedWithButton1 = false;
      TitleText = string.Empty;
      WordWrapMoreInfo = true;
    }

    #region Properties

    /// <summary>
    /// Gets or sets the properties for buttons used in the <see cref="InfoDialog"/> class.
    /// </summary>
    public CommandAreaProperties CommandAreaProperties { get; set; }

    /// <summary>
    /// Gets or sets the text that optionally further describes information details to users, supports 1 line of text.
    /// </summary>
    public string DetailSubText { get; set; }

    /// <summary>
    /// Gets or sets the text describing information details to the users, supports 2 lines of text.
    /// </summary>
    public string DetailText { get; set; }

    /// <summary>
    /// Gets or sets the strategy to use on text that gets truncated because it does not fit within the label controls.
    /// </summary>
    public InfoDialog.FitTextsAction FitTextStrategy { get; set; }

    /// <summary>
    /// Gets or sets the type of information the dialog will display to users.
    /// </summary>
    public InfoDialog.InfoType InfoType { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the dialog is expanded to show the More Information text box.
    /// </summary>
    public bool IsExpanded { get; set; }

    /// <summary>
    /// Gets or sets the image displayed in the dialog.
    /// </summary>
    public Image LogoImage { get; set; }

    /// <summary>
    /// Gets or sets the extended text users can see in the More Information text box.
    /// </summary>
    public string MoreInfoText { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the More Information text box width is aligned with the right border of the <see cref="InfoDialog.Info1Button"/> or the right corner of the <see cref="InfoDialog.Info2Button"/>.
    /// </summary>
    public bool MoreInfoWidthAlignedWithButton1 { get; set; }

    /// <summary>
    /// Gets or sets the text shown in the dialog's upper title bar.
    /// </summary>
    public string TitleBarText { get; set; }

    /// <summary>
    /// Gets or sets the title text of the dialog, displayed in blue color and with a bigger font.
    /// </summary>
    public string TitleText { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the More Information text box word wraps the text.
    /// </summary>
    public bool WordWrapMoreInfo { get; set; }

    #endregion Properties

    /// <summary>
    /// Gets an <see cref="InfoDialogProperties"/> object containing properties suitable to define an <see cref="InfoDialog"/> that displays error messages.
    /// </summary>
    /// <param name="title">The title text of the dialog, displayed in blue color and with a bigger font.</param>
    /// <param name="detail">The text describing information details to the users, supports 2 lines of text.</param>
    /// <param name="subDetail">The text that optionally further describes information details to users, supports 1 line of text.</param>
    /// <param name="moreInformation">The extended text users can see in the More Information text box.</param>
    /// <returns>A <see cref="InfoDialogProperties"/> object that can be used to initialize an <see cref="InfoDialog"/> instance.</returns>
    public static InfoDialogProperties GetErrorDialogProperties(string title, string detail, string subDetail = null, string moreInformation = null)
    {
      return GetOkDialogProperties(InfoDialog.InfoType.Error, title, detail, subDetail, moreInformation);
    }

    /// <summary>
    /// Gets an <see cref="InfoDialogProperties"/> object containing properties suitable to define an <see cref="InfoDialog"/> that displays messages.
    /// </summary>
    /// <param name="infoType">The type of information the dialog will display to users.</param>
    /// <param name="buttonsType">The type of buttons the dialog will display to users.</param>
    /// <param name="title">The title text of the dialog, displayed in blue color and with a bigger font.</param>
    /// <param name="detail">The text describing information details to the users, supports 2 lines of text.</param>
    /// <param name="subDetail">The text that optionally further describes information details to users, supports 1 line of text.</param>
    /// <param name="moreInformation">The extended text users can see in the More Information text box.</param>
    /// <returns>A <see cref="InfoDialogProperties"/> object that can be used to initialize an <see cref="InfoDialog"/> instance.</returns>
    public static InfoDialogProperties GetInfoDialogProperties(InfoDialog.InfoType infoType, CommandAreaProperties.ButtonsLayoutType buttonsType, string title, string detail, string subDetail = null, string moreInformation = null)
    {
      return new InfoDialogProperties
      {
        TitleText = title,
        DetailText = detail,
        DetailSubText = subDetail,
        MoreInfoText = moreInformation,
        InfoType = infoType,
        CommandAreaProperties = new CommandAreaProperties(buttonsType)
      };
    }

    /// <summary>
    /// Gets an <see cref="InfoDialogProperties"/> object containing properties suitable to define an <see cref="InfoDialog"/> that displays informational messages.
    /// </summary>
    /// <param name="title">The title text of the dialog, displayed in blue color and with a bigger font.</param>
    /// <param name="detail">The text describing information details to the users, supports 2 lines of text.</param>
    /// <param name="subDetail">The text that optionally further describes information details to users, supports 1 line of text.</param>
    /// <param name="moreInformation">The extended text users can see in the More Information text box.</param>
    /// <returns>A <see cref="InfoDialogProperties"/> object that can be used to initialize an <see cref="InfoDialog"/> instance.</returns>
    public static InfoDialogProperties GetInformationDialogProperties(string title, string detail, string subDetail = null, string moreInformation = null)
    {
      return GetOkDialogProperties(InfoDialog.InfoType.Info, title, detail, subDetail, moreInformation);
    }

    /// <summary>
    /// Gets an <see cref="InfoDialogProperties"/> object containing properties suitable to define an <see cref="InfoDialog"/> that has an OK and Cancel buttons available.
    /// </summary>
    /// <param name="infoType">The type of information the dialog will display to users.</param>
    /// <param name="title">The title text of the dialog, displayed in blue color and with a bigger font.</param>
    /// <param name="detail">The text describing information details to the users, supports 2 lines of text.</param>
    /// <param name="subDetail">The text that optionally further describes information details to users, supports 1 line of text.</param>
    /// <param name="moreInformation">The extended text users can see in the More Information text box.</param>
    /// <returns>A <see cref="InfoDialogProperties"/> object that can be used to initialize an <see cref="InfoDialog"/> instance.</returns>
    public static InfoDialogProperties GetOkCancelDialogProperties(InfoDialog.InfoType infoType, string title, string detail, string subDetail = null, string moreInformation = null)
    {
      return GetInfoDialogProperties(infoType, CommandAreaProperties.ButtonsLayoutType.OkCancel, title, detail, subDetail, moreInformation);
    }

    /// <summary>
    /// Gets an <see cref="InfoDialogProperties"/> object containing properties suitable to define an <see cref="InfoDialog"/> that has only an OK button available.
    /// </summary>
    /// <param name="infoType">The type of information the dialog will display to users.</param>
    /// <param name="title">The title text of the dialog, displayed in blue color and with a bigger font.</param>
    /// <param name="detail">The text describing information details to the users, supports 2 lines of text.</param>
    /// <param name="subDetail">The text that optionally further describes information details to users, supports 1 line of text.</param>
    /// <param name="moreInformation">The extended text users can see in the More Information text box.</param>
    /// <returns>A <see cref="InfoDialogProperties"/> object that can be used to initialize an <see cref="InfoDialog"/> instance.</returns>
    public static InfoDialogProperties GetOkDialogProperties(InfoDialog.InfoType infoType, string title, string detail, string subDetail = null, string moreInformation = null)
    {
      return GetInfoDialogProperties(infoType, CommandAreaProperties.ButtonsLayoutType.OkOnly, title, detail, subDetail, moreInformation);
    }

    /// <summary>
    /// Gets an <see cref="InfoDialogProperties"/> object containing properties suitable to define an <see cref="InfoDialog"/> that displays success information.
    /// </summary>
    /// <param name="title">The title text of the dialog, displayed in blue color and with a bigger font.</param>
    /// <param name="detail">The text describing information details to the users, supports 2 lines of text.</param>
    /// <param name="subDetail">The text that optionally further describes information details to users, supports 1 line of text.</param>
    /// <param name="moreInformation">The extended text users can see in the More Information text box.</param>
    /// <returns>A <see cref="InfoDialogProperties"/> object that can be used to initialize an <see cref="InfoDialog"/> instance.</returns>
    public static InfoDialogProperties GetSuccessDialogProperties(string title, string detail, string subDetail = null, string moreInformation = null)
    {
      return GetInfoDialogProperties(InfoDialog.InfoType.Success, CommandAreaProperties.ButtonsLayoutType.OkOnly, title, detail, subDetail, moreInformation);
    }

    /// <summary>
    /// Gets an <see cref="InfoDialogProperties"/> object containing properties suitable to define an <see cref="InfoDialog"/> that displays warning information.
    /// </summary>
    /// <param name="title">The title text of the dialog, displayed in blue color and with a bigger font.</param>
    /// <param name="detail">The text describing information details to the users, supports 2 lines of text.</param>
    /// <param name="subDetail">The text that optionally further describes information details to users, supports 1 line of text.</param>
    /// <param name="moreInformation">The extended text users can see in the More Information text box.</param>
    /// <returns>A <see cref="InfoDialogProperties"/> object that can be used to initialize an <see cref="InfoDialog"/> instance.</returns>
    public static InfoDialogProperties GetWarningDialogProperties(string title, string detail, string subDetail = null, string moreInformation = null)
    {
      return GetInfoDialogProperties(InfoDialog.InfoType.Warning, CommandAreaProperties.ButtonsLayoutType.OkOnly, title, detail, subDetail, moreInformation);
    }

    /// <summary>
    /// Gets an <see cref="InfoDialogProperties"/> object containing properties suitable to define an <see cref="InfoDialog"/> that has a Yes and a No buttons available.
    /// </summary>
    /// <param name="infoType">The type of information the dialog will display to users.</param>
    /// <param name="title">The title text of the dialog, displayed in blue color and with a bigger font.</param>
    /// <param name="detail">The text describing information details to the users, supports 2 lines of text.</param>
    /// <param name="subDetail">The text that optionally further describes information details to users, supports 1 line of text.</param>
    /// <param name="moreInformation">The extended text users can see in the More Information text box.</param>
    /// <returns>A <see cref="InfoDialogProperties"/> object that can be used to initialize an <see cref="InfoDialog"/> instance.</returns>
    public static InfoDialogProperties GetYesNoDialogProperties(InfoDialog.InfoType infoType, string title, string detail, string subDetail = null, string moreInformation = null)
    {
      return GetInfoDialogProperties(infoType, CommandAreaProperties.ButtonsLayoutType.YesNo, title, detail, subDetail, moreInformation);
    }
  }
}
