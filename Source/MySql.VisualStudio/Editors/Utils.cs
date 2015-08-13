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

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using MySqlX.Shell;
using System.Data;
using System.Drawing;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.Win32;
using Color = System.Drawing.Color;

namespace MySql.Data.VisualStudio.Editors
{


  /// <summary>
  /// This class contains reusable methods for the project
  /// </summary>
  internal static class Utils
  {
    /// <summary>
    /// True when updating controls to match _currentVsTheme and avoid modifying its value until the process has finished. 
    /// </summary>
    private static bool _isUpdating = false;

    private static VsTheme? _currentVsTheme;

    /// <summary>
    /// Exposes the current Visual Studio theme to the exterior of this class.
    /// </summary>
    public static VsTheme? CurrentVsTheme
    {
      get
      {
        if (_currentVsTheme == null)
        {
          SetCurrentVsTheme();
        }

        return _currentVsTheme;
      }
    }

    private static Color _editorBackgroundColor;
    private static Color _backgroundColor;
    private static Color _fontColor;
    private static Color _dataGridViewCellStyleBackColor;

    /// <summary>
    /// Gets the color of the editor background.
    /// </summary>
    public static Color EditorBackgroundColor
    {
      get
      {
        return _editorBackgroundColor;
      }
    }

    /// <summary>
    /// Gets the color of the background.
    /// </summary>
    public static Color BackgroundColor
    {
      get
      {
        return _backgroundColor;
      }
    }

    /// <summary>
    /// Gets the color of the font.
    /// </summary>
    public static Color FontColor
    {
      get
      {
        return _fontColor;
      }
    }

    /// <summary>
    /// Gets the color of the data grid view cell style back.
    /// </summary>
    public static Color DataGridViewCellStyleBackColor
    {
      get
      {
        return _dataGridViewCellStyleBackColor;
      }
    }

    /// <summary>
    /// Dictionary with the Guid from the default Visual Studio themes and their corresponding enum value.
    /// </summary>
    private static readonly IDictionary<string, VsTheme> Themes = new Dictionary<string, VsTheme>()
    {
        { "de3dbbcd-f642-433c-8353-8f1df4370aba", VsTheme.Light },
        { "1ded0138-47ce-435e-84ef-9ec1f439b749", VsTheme.Dark },
        { "a4d6a176-b948-4b29-8c66-53c97a1ed7d0", VsTheme.Blue }
    };

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
        BackColor = DataGridViewCellStyleBackColor,
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
      gridView.CellFormatting += delegate(object sender, DataGridViewCellFormattingEventArgs e)
      {
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
        return new List<string>();
      }

      MySQL.Utility.Classes.MySqlTokenizer tokenizer = new MySQL.Utility.Classes.MySqlTokenizer(sqlStatements.Trim());
      return tokenizer.BreakIntoStatements();
    }

    /// <summary>
    /// Separates multiple javascript statements into single ones
    /// </summary>
    /// <param name="jsStatements">Javascript statements</param>
    /// <returns>List of single statements</returns>
    public static List<string> BreakJavaScriptStatements(this string jsStatements)
    {
      if (string.IsNullOrEmpty(jsStatements))
      {
        return null;
      }

      MySQL.Utility.Classes.MyJsTokenizer tokenizer = new MySQL.Utility.Classes.MyJsTokenizer(jsStatements.Trim());
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
      for (int idx = 0; idx < data.Length; idx++)
      {
        sbData.AppendLine("  {");
        int ctr = 0;
        foreach (KeyValuePair<string, object> kvp in data[idx])
        {
          sbData.AppendLine(string.Format("    \"{0}\" : \"{1}\"{2}\n", kvp.Key, kvp.Value, (ctr < data[idx].Count - 1) ? "," : ""));
          ctr++;
        }
        sbData.AppendLine(idx < data.Length - 1 ? "  }," : "  }");
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

    /// <summary>
    /// Parse a MySqlConnection object to a string format useb by the NgWrapper
    /// </summary>
    /// <param name="connection">Connection to parse</param>
    /// <returns>Connection string with the format "user:pass@server:port"</returns>
    public static string ToNgFormat(this MySql.Data.MySqlClient.MySqlConnection connection)
    {
      MySqlConnectionProperties connProp = new MySqlConnectionProperties();
      connProp.ConnectionStringBuilder.ConnectionString = connection.ConnectionString;
      string user = connProp["User Id"] as string;
      string pass = connProp["Password"] as string;
      string server = connProp["server"] as string;
      UInt32 port = 3306; //assign the default port

      //verify if the user is not using the default port, if not then extract the value
      object givenPort = connProp["Port"];
      if (givenPort != null)
      {
        port = (UInt32)givenPort;
      }

      return string.Format("{0}:{1}@{2}:{3}", user, pass, server, port);
    }

    /// <summary>
    /// Creates the result page.
    /// </summary>
    /// <param name="counter">The counter.</param>
    /// <returns>A tab page that will contain the results from the queries ran by the user.</returns>
    public static TabPage CreateResultPage(int counter)
    {
      TabPage newResPage = new TabPage();
      newResPage.AutoScroll = true;
      newResPage.Text = string.Format("Result{0}", (counter > 0 ? counter.ToString() : ""));
      newResPage.ImageIndex = 1;
      newResPage.Padding = new Padding(3);
      return newResPage;
    }

    /// <summary>
    /// Gets the corresponding enum for the current Visual Studio theme selected.
    /// </summary>
    private static void SetCurrentVsTheme()
    {
      if (_isUpdating)
      {
        return;
      }

      _isUpdating = true;
      _currentVsTheme = VsTheme.Unknown;

      string themeId = GetThemeId();
      if (string.IsNullOrWhiteSpace(themeId) == false)
      {
        VsTheme theme;
        if (Themes.TryGetValue(themeId, out theme))
        {
          _currentVsTheme = theme;
        }
      }

      switch (CurrentVsTheme)
      {
        case VsTheme.Dark:
          _editorBackgroundColor = Color.FromArgb(255, 37, 37, 38);
          _backgroundColor = Color.FromArgb(255, 45, 45, 48);
          _fontColor = Color.FromKnownColor(KnownColor.WhiteSmoke);
          _dataGridViewCellStyleBackColor = Color.LightGray;
          break;
        case VsTheme.Blue:
          _editorBackgroundColor = Color.White;
          _backgroundColor = Color.FromArgb(255, 207, 214, 229);
          _fontColor = Color.FromKnownColor(KnownColor.ControlText);
          _dataGridViewCellStyleBackColor = Color.FromArgb(237, 243, 253);
          break;
        case VsTheme.Unknown:
        case VsTheme.Light:
        default:
          _editorBackgroundColor = Color.FromArgb(255, 245, 245, 245);
          _backgroundColor = Color.FromArgb(255, 238, 238, 242);
          _fontColor = Color.FromKnownColor(KnownColor.ControlText);
          _dataGridViewCellStyleBackColor = Color.LightGray;
          break;
      }
    }

    /// <summary>
    /// Gets the current Visual Studio theme Guid from the register.
    /// </summary>
    /// <returns>The current Visual Studio theme guid</returns>
    public static string GetThemeId()
    {
      var modules = System.Diagnostics.Process.GetCurrentProcess().Modules;
      string processpathfilename;
      if (modules.Count > 0)
      {
        processpathfilename = modules[0].FileName;
      }
      else
      {
        return null;
      }

      const string CategoryName = "General";
      const string ThemePropertyName = "CurrentTheme";
      string VisualStudioVersion = "10.0";

      if (processpathfilename.Contains("11.0")) VisualStudioVersion = "11.0";
      else if (processpathfilename.Contains("12.0")) VisualStudioVersion = "12.0";
      else if (processpathfilename.Contains("14.0")) VisualStudioVersion = "14.0";

      string keyName = string.Format(@"Software\Microsoft\VisualStudio\{1}\{0}", CategoryName, VisualStudioVersion);
      using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyName))
      {
        if (key != null)
        {
          return (string)key.GetValue(ThemePropertyName, string.Empty);
        }
      }

      return null;
    }


    /// <summary>
    /// Triggers the specific SetColors method of every control received in the collection.
    /// </summary>
    /// <param name="controls">The controls from the editor window</param>
    public static void SetColors(this Control.ControlCollection controls)
    {
      SetCurrentVsTheme();

      if (controls == null)
      {
        return;
      }

      try
      {
        foreach (var control in controls)
        {
          if (control == null) continue;

          if (control is ToolStrip)
          {
            ((ToolStrip)control).SetColor();
            continue;
          }

          if (control is ToolStripLabel)
          {
            ((ToolStripLabel)control).SetColor();
            continue;
          }

          if (control is ToolStripButton)
          {
            ((ToolStripButton)control).SetColor();
            continue;
          }

          if (control is TabControl)
          {
            ((TabControl)control).SetColor();
            continue;
          }

          if (control is GridViewResult)
          {
            ((GridViewResult)control).SetColor();
            continue;
          }

          if (control is FieldTypesGrid)
          {
            ((FieldTypesGrid)control).SetColor();
            continue;
          }

          if (control is TreeViewResult)
          {
            ((TreeViewResult)control).SetColor();
            continue;
          }

          if (control is TextViewPane)
          {
            ((TextViewPane)control).SetColor();
            continue;
          }

          if (control is DataGridView)
          {
            ((DataGridView)control).SetColor();
            continue;
          }

          if (control is TreeView)
          {
            ((TreeView)control).SetColor();
            continue;
          }

          if (control is TextBox)
          {
            ((TextBox)control).SetColor();
            continue;
          }

          if (control is GridResultSet)
          {
            ((GridResultSet)control).SetColor();
          }
        }
      }
      finally
      {
        _isUpdating = false;
      }
    }

    /// <summary>
    /// Sets the color of the Splitter control to match the current visual studio theme selected.
    /// </summary>
    /// <param name="control">The Splitter control to be updated.</param>
    public static void SetColor(this Splitter control)
    {
      control.BackColor = BackgroundColor;
    }

    /// <summary>
    /// Sets the color of the ToolStrip control to match the current visual studio theme selected
    /// and triggers the SetColor method of contained ToolStripLabel and ToolStripButton controls.
    /// </summary>
    /// <param name="control">The ToolStrip control to be updated.</param>
    public static void SetColor(this ToolStrip control)
    {
      control.BackColor = BackgroundColor;
      control.Renderer = new CustomToolStripRenderer();

      foreach (var item in control.Items)
      {
        var itemType = item.GetType();
        if (itemType != typeof(ToolStripLabel) || itemType == typeof(ToolStripButton))
        {
          continue;
        }

        if (itemType == typeof(ToolStripLabel))
        {
          ((ToolStripLabel)item).SetColor();
          continue;
        }
        if (itemType == typeof(ToolStripButton))
        {
          ((ToolStripButton)item).SetColor();
        }
      }
    }

    /// <summary>
    /// Sets the color of the ToolStripLabel control to match the current visual studio theme selected.
    /// </summary>
    /// <param name="control">The ToolStripLabel control to be updated.</param>
    public static void SetColor(this ToolStripLabel control)
    {
      control.ForeColor = FontColor;
    }

    /// <summary>
    /// Sets the color of the ToolStripButton control to match the current visual studio theme selected.
    /// </summary>
    /// <param name="control">The ToolStripButton control to be updated.</param>
    public static void SetColor(this ToolStripButton control)
    {
      control.ForeColor = FontColor;
    }

    /// <summary>
    /// Sets the color of the TabControl control to match the current visual studio theme selected.
    /// </summary>
    /// <param name="control">The TabControl control to be updated.</param>
    public static void SetColor(this TabControl control)
    {
      control.DrawItem -= OnDrawItem;
      control.DrawItem += OnDrawItem;
    }

    /// <summary>
    /// Sets the color of the DataGridView control to match the current visual studio theme selected.
    /// </summary>
    /// <param name="control">The DataGridView control to be updated.</param>
    public static void SetColor(this DataGridView control)
    {
      control.BorderStyle = BorderStyle.None;
      control.BackgroundColor = EditorBackgroundColor;
    }

    /// <summary>
    /// Sets the color of the TreeView control to match the current visual studio theme selected.
    /// </summary>
    /// <param name="control">The TreeView control to be updated.</param>
    public static void SetColor(this TreeView control)
    {
      control.BorderStyle = BorderStyle.None;
      control.BackColor = EditorBackgroundColor;
      control.ForeColor = FontColor;
    }

    /// <summary>
    /// Sets the color of the TextBox control to match the current visual studio theme selected.
    /// </summary>
    /// <param name="control">The TextBox control to be updated.</param>
    public static void SetColor(this TextBox control)
    {
      control.BorderStyle = BorderStyle.None;
      control.BackColor = EditorBackgroundColor;
      control.ForeColor = FontColor;
    }

    /// <summary>
    /// Sets the color of the GridResultSet control to match the current visual studio theme selected.
    /// </summary>
    /// <param name="control">The GridResultSet control to be updated.</param>
    public static void SetColor(this GridResultSet control)
    {
      control.Controls.SetColors();
      control.SetDataGridStyle();
    }

    /// <summary>
    /// Sets the color of the GridViewResult control to match the current visual studio theme selected.
    /// </summary>
    /// <param name="control">The GridViewResult control to be updated.</param>
    public static void SetColor(this GridViewResult control)
    {
      control.Controls.SetColors();
      control.SetDataGridStyle();
    }

    /// <summary>
    /// Sets the color of the FieldTypesGrid control to match the current visual studio theme selected.
    /// </summary>
    /// <param name="control">The FieldTypesGrid control to be updated.</param>
    public static void SetColor(this FieldTypesGrid control)
    {
      control.Controls.SetColors();
      control.SetDataGridStyle();
    }

    /// <summary>
    /// Sets the color of the TreeViewResult control to match the current visual studio theme selected.
    /// </summary>
    /// <param name="control">The TreeViewResult control to be updated.</param>
    public static void SetColor(this TreeViewResult control)
    {
      control.Controls.SetColors();
    }

    /// <summary>
    /// Sets the color of the TextViewPane control to match the current visual studio theme selected.
    /// </summary>
    /// <param name="control">The TextViewPane control to be updated.</param>
    public static void SetColor(this TextViewPane control)
    {
      control.Controls.SetColors();
    }

    //Since there is a known issue with the ToolStripSystemRenderer class, Microsoft suggests to Create a subclass of ToolStripSystemRenderer,
    //and overriding OnRenderToolStripBorder and making it a no-op.
    public class CustomToolStripRenderer : ToolStripSystemRenderer
    {
      public CustomToolStripRenderer() { }
      protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e) { }
    }

    /// <summary>
    /// Custom OnDrawItem method designed to override TabControl's and make them match the current theme.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="DrawItemEventArgs"/> instance containing the event data.</param>
    private static void OnDrawItem(Object sender, DrawItemEventArgs e)
    {
      // fill in the whole rect (control Background)
      using (SolidBrush br = new SolidBrush(BackgroundColor))
      {
        e.Graphics.FillRectangle(br, ((TabControl)sender).ClientRectangle);
      }

      TabPage CurrentTab = ((TabControl)sender).TabPages[e.Index];
      Rectangle ItemRect = ((TabControl)sender).GetTabRect(e.Index);
      SolidBrush FillBrush = new SolidBrush(BackgroundColor);
      SolidBrush TextBrush = new SolidBrush(FontColor);
      StringFormat sf = new StringFormat();
      sf.Alignment = StringAlignment.Center;
      sf.LineAlignment = StringAlignment.Center;

      //Set up rotation for left and right aligned tabs
      if (((TabControl)sender).Alignment == TabAlignment.Left || ((TabControl)sender).Alignment == TabAlignment.Right)
      {
        float RotateAngle = 90;
        if (((TabControl)sender).Alignment == TabAlignment.Left)
          RotateAngle = 270;
        PointF cp = new PointF(ItemRect.Left + (ItemRect.Width / 2), ItemRect.Top + (ItemRect.Height / 2));
        e.Graphics.TranslateTransform(cp.X, cp.Y);
        e.Graphics.RotateTransform(RotateAngle);
        ItemRect = new Rectangle(-(ItemRect.Height / 2), -(ItemRect.Width / 2), ItemRect.Height, ItemRect.Width);
      }

      //Next we'll paint the TabItem with our Fill Brush
      e.Graphics.FillRectangle(FillBrush, ItemRect);

      //Now draw the text.
      e.Graphics.DrawString(CurrentTab.Text, e.Font, TextBrush, ItemRect, sf);

      //Reset any Graphics rotation
      e.Graphics.ResetTransform();

      //Draw unselected tab pages
      for (int i = 0; i < ((TabControl)sender).TabPages.Count; i++)
      {
        ((TabControl)sender).TabPages[i].BackColor = EditorBackgroundColor;
        ((TabControl)sender).TabPages[i].ForeColor = FontColor;
        TextBrush.Color = FontColor;
        ItemRect = ((TabControl)sender).GetTabRect(i);
        ItemRect.Inflate(2, 2);
        e.Graphics.DrawString(((TabControl)sender).TabPages[i].Text, e.Font, TextBrush, (RectangleF)ItemRect, sf);
      }

      //Finally, we should Dispose of our brushes.
      FillBrush.Dispose();
      TextBrush.Dispose();
    }

    /// <summary>
    /// Enum used to define the list of available default themes for Visual Studio
    /// </summary>
    public enum VsTheme
    {
      Unknown = 0,
      /// <summary>
      /// The dark {1ded0138-47ce-435e-84ef-9ec1f439b749}
      /// </summary>
      Dark,
      /// <summary>
      /// The blue {a4d6a176-b948-4b29-8c66-53c97a1ed7d0}
      /// </summary>
      Blue,
      /// <summary>
      /// The light {de3dbbcd-f642-433c-8353-8f1df4370aba}
      /// </summary>
      Light
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

  /// <summary>
  /// Enum used to know which information view will be shown to the user
  /// </summary>
  internal enum DataViewOption
  {
    /// <summary>
    /// Result Set pane for Sql Editor
    /// </summary>
    ResultSet,
    /// <summary>
    /// Field Types pane for Sql Editor
    /// </summary>
    FieldTypes,
    /// <summary>
    /// Execution Plan pane for Sql Editor
    /// </summary>
    ExecutionPlan,
    /// <summary>
    /// Query Stats Pane for Sql Editor
    /// </summary>
    Querystats,
    /// <summary>
    /// Text view Pane for JS Editor
    /// </summary>
    TextView,
    /// <summary>
    /// Tree view Pane for JS Editor
    /// </summary>
    TreeView
  }
}
