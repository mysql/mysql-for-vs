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
    NgShellWrapper _ngWrapper;
    private MySqlConnection mySqlConnection;
    private string js;

    /// <summary>
    /// Creates a new instance of JSResultsetView
    /// </summary>
    public JSResultsetView()
    {
      InitializeComponent();
      ConfigureMenu();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JSResultsetView"/> class.
    /// </summary>
    /// <param name="mySqlConnection">The MySQL connection.</param>
    /// <param name="js">The js content of the editor window.</param>
    public JSResultsetView(MySqlConnection mySqlConnection, string js)
      : this()
    {
      Dock = DockStyle.Fill;
      SetScript(mySqlConnection, js);
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
    }

    /// <summary>
    /// Set the script that will be used to get the information that will be displayed in the views
    /// </summary>
    /// <param name="connection">Connection taht will be used to execute the script</param>
    /// <param name="script">Script that will be executed</param>
    public void SetScript(MySqlConnection connection, string script)
    {
      if (string.IsNullOrEmpty(script))
      {
        return;
      }

      if (_ngWrapper == null)
      {
        _ngWrapper = new NgShellWrapper(connection.ToNgFormat(), true);
      }

      LoadData(script);
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
    /// Executes the script and if data is returned then is loaded into the views, otherwise a message in the output window will display info about the script executed
    /// </summary>
    /// <param name="cmd">Script that will be executed</param>
    private void LoadData(string cmd)
    {
      DocumentResultSet result = _ngWrapper.ExecuteJavaScript(cmd);
      if (result != null)
      {
        HasResultSet = true;
        ctrlGridView.SetData(result);
        ctrlTreeView.SetData(result);
        ctrlTextView.SetData(result);
      }
      else
      {
        HasResultSet = false;
        Utils.WriteToOutputWindow(_ngWrapper.ExecutionResult, Messagetype.Information);
      }
    }

    public bool HasResultSet
    {
      private set;
      get;
    }
  }
}
