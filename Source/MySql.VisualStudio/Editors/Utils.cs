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

using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySqlX.Shell;
using System.Data;

namespace MySql.Data.VisualStudio.Editors
{
  /// <summary>
  /// This class contains reusable methods for the project
  /// </summary>
  internal static class Utils
  {
    /// <summary>
    /// Create a cell style to apply it to a Grid View Header
    /// </summary>
    /// <returns>The heder style</returns>
    public static DataGridViewCellStyle GetHeaderStyle()
    {
      return new DataGridViewCellStyle
      {
        Alignment = DataGridViewContentAlignment.MiddleCenter,
        BackColor = Color.WhiteSmoke,
        ForeColor = Color.Black
      };
    }

    /// <summary>
    /// Create a row style to apply it to a Grid View row similar to Workbench grid result style
    /// </summary>
    /// <returns>The row style</returns>
    public static DataGridViewCellStyle GetRowStyle()
    {
      return new DataGridViewCellStyle
      {
        Alignment = DataGridViewContentAlignment.MiddleLeft,
        BackColor = Color.White,
        ForeColor = Color.Black,
        SelectionBackColor = Color.FromArgb(1, 120, 208),
        SelectionForeColor = Color.White,
        WrapMode = DataGridViewTriState.False,
        NullValue = string.Empty
      };
    }

    /// <summary>
    /// Create a alternative row style to apply it to a Grid View row similar to Workbench grid result style
    /// </summary>
    /// <returns>The alternative row style</returns>
    public static DataGridViewCellStyle GetAlternateRowStyle()
    {
      return new DataGridViewCellStyle
      {
        Alignment = DataGridViewContentAlignment.MiddleLeft,
        BackColor = Color.FromArgb(237, 243, 253),
        ForeColor = Color.Black,
        SelectionBackColor = Color.FromArgb(1, 120, 208),
        SelectionForeColor = Color.White,
        WrapMode = DataGridViewTriState.False,
        NullValue = string.Empty
      };
    }

    /// <summary>
    /// This method handle the blob values to not show it as garbage or unreadable data
    /// </summary>
    /// <param name="gridView">The gridview that can have blob values</param>
    public static void SanitizeBlobs(ref DataGridView gridView)
    {
      if (gridView == null)
      {
        return;
      }

      bool[] _isColBlob = null;
      DataGridViewColumnCollection coll = gridView.Columns;
      _isColBlob = new bool[coll.Count];
      for (int i = 0; i < coll.Count; i++)
      {
        DataGridViewColumn col = coll[i];
        DataGridViewTextBoxColumn newCol = null;
        if (!(col is DataGridViewImageColumn)) continue;
        coll.Insert(i, newCol = new DataGridViewTextBoxColumn()
        {
          DataPropertyName = col.DataPropertyName,
          HeaderText = col.HeaderText,
          ReadOnly = true
        });
        coll.Remove(col);
        _isColBlob[i] = true;
      }

      // Adding this delegate to the CellFormating handler we can customize the format suitable for display blob values.
      // This format will be applied when the grid cells are being painted, that's why is added as a delegate.
      gridView.CellFormatting += delegate (object sender, DataGridViewCellFormattingEventArgs e) {
        if (e.ColumnIndex == -1) return;
        if (_isColBlob[e.ColumnIndex])
        {
          if (e.Value == null || e.Value is DBNull)
            e.Value = "<NULL>";
          else
            e.Value = "<BLOB>";
        }
      };
    }

    /// <summary>
    /// Write a messages to the VS Output window under de MySQL category
    /// </summary>
    /// <param name="message">Message to write</param>
    /// <param name="type">Kind of meesage</param>
    public static void WriteToOutputWindow(string message, Messagetype type)
    {
      if (string.IsNullOrEmpty(message))
      {
        return;
      }

      IVsOutputWindow outWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;
      if (outWindow != null)
      {
        Guid generalPaneGuid = VSConstants.GUID_OutWindowGeneralPane;
        IVsOutputWindowPane outputPane = null;

        if (outWindow.GetPane(ref generalPaneGuid, out outputPane) < 0)
        {
          outWindow.CreatePane(ref generalPaneGuid, "MySQL", 1, 0);
          outWindow.GetPane(ref generalPaneGuid, out outputPane);
        }

        if (outputPane != null)
        {
          outputPane.OutputString(string.Format("[{0}] - {1}", type.ToString(), message) + Environment.NewLine);
        }
      }
    }

    /// <summary>
    /// Separates multiple MySql query statements contained in a single line
    /// </summary>
    /// <param name="sqlStatements">MySql statements line</param>
    /// <returns>String list of separated statements</returns>
    public static List<string> BreakSqlStatements(this string sqlStatements)
    {
      if (string.IsNullOrEmpty(sqlStatements))
      {
        return null;
      }

      MySQL.Utility.Classes.MySqlTokenizer tokenizer = new MySQL.Utility.Classes.MySqlTokenizer(sqlStatements.Trim());
      return tokenizer.BreakIntoStatements();
    }

    /// <summary>
    /// Parse a DocumentResultSet object to string with JSON format
    /// </summary>
    /// <param name="document">The document to parse</param>
    /// <returns>String with JSON format</returns>
    public static string ToJson(this DocumentResultSet document)
    {
      StringBuilder sbData = new StringBuilder();
      Dictionary<string, object>[] data = document.GetData().ToArray();
      sbData.AppendLine("{");
      for (int idx = 0; idx < data.Length;idx++)
      {
        sbData.AppendLine("\t{");
        int ctr = 0;
        foreach (KeyValuePair<string, object> kvp in data[idx])
        {
          sbData.AppendFormat("\t\t\"{0}\" : \"{1}\"{2}\n", kvp.Key, kvp.Value, (ctr < data[idx].Count - 1) ? "," : "");
          ctr++;
        }
        sbData.AppendLine(idx < data.Length - 1 ? "\t}," : "\t}");
      }
      sbData.AppendLine("}");
      return sbData.ToString();
    }

    /// <summary>
    /// Parse a TableResultSet to a DataTable object
    /// </summary>
    /// <param name="resultSet">TableResultSet to parse</param>
    /// <returns>Object parse to DataTable object</returns>
    public static System.Data.DataTable ToDataTable(this TableResultSet resultSet)
    {
      DataTable result = new DataTable("Result");
      foreach (MySqlX.Shell.ResultSetMetadata column in resultSet.GetMetadata())
      {
        result.Columns.Add(column.GetName());
      }

      foreach (object[] row in resultSet.GetData())
      {
        result.Rows.Add(row);
      }
      return result;
    }
  }

  /// <summary>
  /// Enum used to know what kind of message will be written to the output window
  /// </summary>
  internal enum Messagetype
  {
    /// <summary>
    /// Use this option to clasify the message as Error
    /// </summary>
    Error,
    /// <summary>
    /// Use this option to clasify the message as Information
    /// </summary>
    Information,
    /// <summary>
    /// Use this option to clasify the message as Warning
    /// </summary>
    Warning
  }
}
