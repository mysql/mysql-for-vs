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
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace MySql.Data.VisualStudio.Editors
{
  /// <summary>
  /// ExecutionPlanView control class
  /// </summary>
  public partial class ExecutionPlanView : UserControl
  {
    /// <summary>
    /// If not valid data is received this message will be displayed to the user
    /// </summary>
    private const string _noValidDataMessage = "No valid data received for Execution Plan.";

    /// <summary>
    /// Initializes a new instance of the ExecutionPlanView class.
    /// </summary>
    public ExecutionPlanView()
    {
      InitializeComponent();
#if !VS_SDK_2010
      VSColorTheme.ThemeChanged += VSColorTheme_ThemeChanged;
      SetColors();
    }


    /// <summary>
    /// Responds to the event when Visual Studio theme changed.
    /// </summary>
    /// <param name="e">The <see cref="ThemeChangedEventArgs"/> instance containing the event data.</param>
    void VSColorTheme_ThemeChanged(ThemeChangedEventArgs e)
    {
      SetColors();
#endif
    }

    /// <summary>
    /// Sets the colors corresponding to current Visual Studio theme.
    /// </summary>
    private void SetColors()
    {
      Controls.SetColors();
      BackColor = Utils.BackgroundColor;
    }

    /// <summary>
    /// Loads the data received into a TextBox with Json format
    /// </summary>
    /// <param name="data">The data to load</param>
    public void SetData(string data)
    {
      if (!string.IsNullOrEmpty(data))
      {
        txtExecPlan.AppendText(data.Replace("\n", "\r\n"));
      }
      else
      {
        txtExecPlan.AppendText(_noValidDataMessage);
      }

      txtExecPlan.Visible = true;
      dgvExecPlan.Visible = false;
    }

    /// <summary>
    /// Load the data received into a GridView
    /// </summary>
    /// <param name="data">Data to load</param>
    public void SetData(DataTable data)
    {
      if (data == null)
      {
        txtExecPlan.AppendText(_noValidDataMessage);
        txtExecPlan.Visible = true;
        dgvExecPlan.Visible = false;
        return;
      }

      dgvExecPlan.DataSource = data;
      txtExecPlan.Visible = false;
      dgvExecPlan.Visible = true;
      SetDataGridStyle();
      Utils.SanitizeBlobs(ref dgvExecPlan);
    }

    /// <summary>
    /// Apply style to the grid used to show the data
    /// </summary>
    private void SetDataGridStyle()
    {
      dgvExecPlan.ColumnHeadersDefaultCellStyle = Utils.GetHeaderStyle();
      dgvExecPlan.RowsDefaultCellStyle = Utils.GetRowStyle();
      dgvExecPlan.AlternatingRowsDefaultCellStyle = Utils.GetAlternateRowStyle();
    }
  }
}
