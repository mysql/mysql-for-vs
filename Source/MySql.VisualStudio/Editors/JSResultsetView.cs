// Copyright © 2015, Oracle and/or its affiliates. All rights reserved.
//
// MySQL for Visual Studio is licensed under the terms of the GPLv2
// <http://www.gnu.org/licenses/old-licenses/gpl-2.0.html>, like most
// MySQL Connectors. There are special exceptions to the terms and
// conditions of the GPLv2 as it is applied to this software, see the
// FLOSS License Exception
// <http://www.mysql.com/about/legal/licensing/foss-exception.html>.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published
// by the Free Software Foundation; version 2 of the License.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License
// for more details.
//
// You should have received a copy of the GNU General Public License along
// with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using Microsoft.VisualStudio.PlatformUI;
using MySql.Data.MySqlClient;
using MySqlX.Shell;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MySql.Data.VisualStudio.Editors
{
  /// <summary>
  /// ResultSet control for Js files
  /// </summary>
  public partial class JSResultsetView : UserControl
  {
    /// <summary>
    /// Creates a new instance of JSResultsetView
    /// </summary>
    public JSResultsetView()
    {
      InitializeComponent();
      ConfigureMenu();
#if VS_SDK_2013
      VSColorTheme.ThemeChanged += VSColorTheme_ThemeChanged;
      Controls.SetColors();
    }

    /// <summary>
    /// Set colors to match the selected visual studio theme.
    /// </summary>
    /// <param name="e">The <see cref="ThemeChangedEventArgs"/> instance containing the event data.</param>
    void VSColorTheme_ThemeChanged(ThemeChangedEventArgs e)
    {
      Controls.SetColors();
#endif
    }

    /// <summary>
    /// Create the list of buttons that will be loaded in the vertical menu control
    /// </summary>
    private void ConfigureMenu()
    {
      List<VerticalMenuButton> buttons = new List<VerticalMenuButton>() {
        new VerticalMenuButton() {
                                    ButtonText = "Grid\nView",
                                    Name = "btnGridView",
                                    ToolTip = "Grid View",
                                    ImageToLoad = ImageType.Resultset,
                                    ClickEvent = delegate(object sender, EventArgs e) { ShowControl(DataViewOption.ResultSet); } },
        new VerticalMenuButton() {
                                    ButtonText = "Tree\nView",
                                    Name = "btnTreeView",
                                    ToolTip = "Tree View",
                                    ImageToLoad = ImageType.TreeView,
                                    ClickEvent = delegate(object sender, EventArgs e) { ShowControl(DataViewOption.TreeView); } },
        new VerticalMenuButton() {
                                    ButtonText = "Text\nView",
                                    Name = "btnTextView",
                                    ToolTip = "Text View",
                                    ImageToLoad = ImageType.TextView,
                                    ClickEvent = delegate(object sender, EventArgs e) { ShowControl(DataViewOption.TextView); } }
      };

      ctrlMenu.ConfigureControl(buttons);
    }

    /// <summary>
    /// Choose wich information view will be shown to the user basis in the enum option given
    /// </summary>
    /// <param name="controlToShow">Pane that will be displayed</param>
    private void ShowControl(DataViewOption controlToShow)
    {
      ctrlGridView.Visible = (controlToShow == DataViewOption.ResultSet);
      ctrlTextView.Visible = (controlToShow == DataViewOption.TextView);
      ctrlTreeView.Visible = (controlToShow == DataViewOption.TreeView);
    }

    /// <summary>
    /// Load the data received into the views if it is valid
    /// </summary>
    /// <param name="data">Data to be loaded</param>
    public void LoadData(DocumentResultSet data)
    {
      if (data == null)
      {
        return;
      }

      ctrlGridView.SetData(data);
      ctrlTreeView.SetData(data);
      ctrlTextView.SetData(data);
    }
  }
}
