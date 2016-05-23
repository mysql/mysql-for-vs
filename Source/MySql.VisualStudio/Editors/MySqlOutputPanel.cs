// Copyright © 2016, Oracle and/or its affiliates. All rights reserved.
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

using System;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.VisualStudio.Properties;

namespace MySql.Data.VisualStudio.Editors
{
  public partial class MySqlOutputPanel : UserControl
  {
    /// <summary>
    /// Integer variable used to hold the grid's zero based rows count.
    /// </summary>
    private int _index;

    /// <summary>
    /// Variable to hold the datagridview clicked cell
    /// </summary>
    DataGridViewCell _clickedCell;

    /// <summary>
    /// Enum to hold the icons that can be displayed in the grid first column. It can be Success, Warning, or Error.
    /// </summary>
    public enum IconType
    {
      Success,
      Warning,
      Error
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlOutputPanel"/> class.
    /// </summary>
    public MySqlOutputPanel()
    {
      InitializeComponent();
      CreateGridHeader();
    }

    /// <summary>
    /// Handles the MouseClick event of the dataGridView1 control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
    private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
    {
      if (e.Button != MouseButtons.Right)
      {
        return;
      }

      DataGridView dgv = (DataGridView)sender;
      ContextMenu m = new ContextMenu();
      // Get the current row and column of the gridview from the click event.
      int rowIndex = dataGridView1.HitTest(e.X, e.Y).RowIndex;
      int colIndex = dataGridView1.HitTest(e.X, e.Y).ColumnIndex;
      if (rowIndex != -1 && colIndex != -1)
      {
        _clickedCell = dgv.Rows[rowIndex].Cells[colIndex];
        m.MenuItems.Add(new MenuItem("Copy", CopyMySqlOutputGridCellContent));
      }

      m.MenuItems.Add(new MenuItem("Clear Results", ClearMySqlOutputGrid));
      m.Show(dataGridView1, new Point(e.X, e.Y));
    }

    /// <summary>
    /// Creates the grid header.
    /// </summary>
    private void CreateGridHeader()
    {
      var column = new DataGridViewColumn(new DataGridViewImageCell())
      {
        Name = "IconsColumn",
        HeaderText = string.Empty,
        Width = 20,
        Resizable = DataGridViewTriState.False
      };
      dataGridView1.Columns.Add(column);
      column = new DataGridViewColumn(new DataGridViewTextBoxCell())
      {
        Name = "IndexColumn",
        HeaderText = string.Empty,
        Width = 20,
        Resizable = DataGridViewTriState.False
      };
      dataGridView1.Columns.Add(column);
      column = new DataGridViewColumn(new DataGridViewTextBoxCell())
      {
        Name = "TimeColumn",
        HeaderText = "Time",
        Width = 65,
        Resizable = DataGridViewTriState.False
      };
      dataGridView1.Columns.Add(column);
      column = new DataGridViewColumn(new DataGridViewTextBoxCell())
      {
        Name = "ActionColumn",
        HeaderText = "Action",
        AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
      };
      dataGridView1.Columns.Add(column);
      column = new DataGridViewColumn(new DataGridViewTextBoxCell())
      {
        Name = "MessageColumn",
        HeaderText = "Message",
        AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
      };
      dataGridView1.Columns.Add(column);
      column = new DataGridViewColumn(new DataGridViewTextBoxCell())
      {
        Name = "DurationColumn",
        HeaderText = "Duration/Fetch",
        Width = 140,
        Resizable = DataGridViewTriState.False
      };
      dataGridView1.Columns.Add(column);
    }

    /// <summary>
    /// Adds a row to the MySql Output grid.
    /// </summary>
    /// <param name="iconType">Type of the icon.</param>
    /// <param name="action">The action.</param>
    /// <param name="message">The message.</param>
    /// <param name="duration">The duration.</param>
    public void AddMySqlOutputGridRow(IconType iconType, string action, string message, string duration)
    {
      Icon icon = null;
      switch (iconType)
      {
        case IconType.Success:
          icon = Resources.successIcon;
          break;
        case IconType.Warning:
          icon = Resources.warningIcon;
          break;
        case IconType.Error:
          icon = Resources.errorIcon;
          break;
      }

      dataGridView1.Rows.Add(null, (_index + 1).ToString(), DateTime.Now.ToString("HH:mm:ss"), action, message, duration);
      DataGridViewImageCell iconCell = (DataGridViewImageCell)dataGridView1.Rows[_index].Cells["IconsColumn"];
      iconCell.Value = icon;
      _index++;
    }

    /// <summary>
    /// Clears My Sql output grid.
    /// </summary>
    private void ClearMySqlOutputGrid(object sender, EventArgs e)
    {
      dataGridView1.Rows.Clear();
      _index = 0;
    }

    /// <summary>
    /// Copies the content of my SQL output grid cell to the clipboard.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private void CopyMySqlOutputGridCellContent(object sender, EventArgs e)
    {
      Clipboard.SetText(_clickedCell.Value.ToString());
    }
  }
}