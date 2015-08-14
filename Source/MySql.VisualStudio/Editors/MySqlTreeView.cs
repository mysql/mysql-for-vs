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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace MySql.Data.VisualStudio.Editors
{
  /// <summary>
  /// Displays data in a TreeView using a column format
  /// </summary>
  public partial class MySqlTreeView : UserControl
  {
    /// <summary>
    /// Variable to store the headers used in the columns that display the data
    /// </summary>
    private string[] _colHeaders;

    /// <summary>
    /// Creates an instance of MySqlTreeView
    /// </summary>
    public MySqlTreeView()
    {
      InitializeComponent();
      SetImageList();
    }

    /// <summary>
    /// Creates an instance of MySqlTreeView
    /// </summary>
    /// <param name="columnHeaders">Headers that will be used as column identifiers</param>
    public MySqlTreeView(params string[] columnHeaders)
    {
      InitializeComponent();
      ColumnHeaders = columnHeaders;
      SetImageList();
    }

    /// <summary>
    /// Get or set the headers used for the columns
    /// </summary>
    public string[] ColumnHeaders
    {
      get
      {
        return _colHeaders;
      }

      set
      {
        _colHeaders = value;
        SetColumnHeader(_colHeaders);
      }
    }

    /// <summary>
    /// Get the TreeView used to display data in the control
    /// </summary>
    public TreeView TreeView
    {
      get
      {
        return btvData;
      }
    }

    /// <summary>
    /// Add the headers to the columns
    /// </summary>
    /// <param name="columnHeaders"></param>
    private void SetColumnHeader(string[] columnHeaders)
    {
      int width = this.Width / columnHeaders.Length;
      lvHeaders.Columns.Clear();
      foreach (string header in columnHeaders)
      {
        lvHeaders.Columns.Add(header, width, HorizontalAlignment.Left);
      }
    }

    /// <summary>
    /// Click event for the control that contains the column headers
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event arguments</param>
    private void lvHeaders_Click(object sender, EventArgs e)
    {
      btvData.Focus();
    }

    /// <summary>
    /// ColumnWidthChanged event for the control that contains the column headers
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event arguments</param>
    private void lvHeaders_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
    {
      btvData.Focus();
      //this line re-adjust the node width to match the column width
      btvData.Invalidate();
    }

    /// <summary>
    /// ColumnWidthChanging event for the control that contains the column headers
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event arguments</param>
    private void lvHeaders_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
    {
      btvData.Focus();
      //this line re-adjust the node width to match the column width
      btvData.Invalidate();
    }

    /// <summary>
    /// Click event for the TreeView that displays the data
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event arguments</param>
    private void btvData_Click(object sender, EventArgs e)
    {
      Point point = btvData.PointToClient(Control.MousePosition);
      TreeNode node = btvData.GetNodeAt(point);
      if (node != null)
      {
        btvData.SelectedNode = node;
      }
    }

    /// <summary>
    /// DrawNode event for the TreeView that displays the data. This method overrides the normal draw of the TreeView nodes to add the extra info as a columns
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event arguments</param>
    private void btvData_DrawNode(object sender, DrawTreeNodeEventArgs e)
    {
      //This line draws the node and add the expand/collapse icon
      e.DrawDefault = true;
      //Get the size enad location for the current node, this will be used to draw the extra information
      Rectangle rect = e.Bounds;

      //get background color for the current node basis on its state
      Brush background = (((e.State & TreeNodeStates.Selected) != 0) && ((e.State & TreeNodeStates.Focused) != 0)) ? new SolidBrush(Color.LightBlue) : SystemBrushes.Control;
      e.Graphics.FillRectangle(background, rect);
      e.Graphics.DrawRectangle(SystemPens.Control, rect);

      //loop to access the column value and data type to be drawn in the current node
      for (int intColumn = 1; intColumn < lvHeaders.Columns.Count; intColumn++)
      {
        //set the node column width basis on the header width
        rect.Offset(lvHeaders.Columns[intColumn - 1].Width, 0);
        rect.Width = lvHeaders.Columns[intColumn].Width;

        //get the column information (value and type)
        string strColumnText;
        string[] tagInfo = e.Node.Tag as string[];
        if (tagInfo != null && intColumn <= tagInfo.Length)
        {
          //get the data that correspond to the current column
          strColumnText = tagInfo[intColumn - 1];
        }
        else
        {
          //if no data is specified the default info that will be displayed will be something like "'n' NodeText" where 'n' represent the column number
          strColumnText = string.Format("{0} {1}", intColumn, e.Node.Text);
        }

        //set the text align basis on the header align
        TextFormatFlags flags = TextFormatFlags.EndEllipsis;
        switch (lvHeaders.Columns[intColumn].TextAlign)
        {
          case HorizontalAlignment.Center:
            flags |= TextFormatFlags.HorizontalCenter;
            break;
          case HorizontalAlignment.Left:
            flags |= TextFormatFlags.Left;
            break;
          case HorizontalAlignment.Right:
            flags |= TextFormatFlags.Right;
            break;
        }

        //set the icon that the current node will use, the icon will be the same when the node is selected or not, in case no data is received the generic icon will be used
        e.Node.SelectedImageIndex = e.Node.ImageIndex = tagInfo != null ? GetImageIndex(tagInfo[1]) : 0;

        rect.Y++;
        //get the forecolor for the text basis on the data type only when the text to draw is the column value, in case no data is received then use black color
        Color foreColor = tagInfo != null ? (intColumn == 1 ? GetForeColor(tagInfo[1]) : Color.Black) : Color.Black;
        //draw the column information (value or type)
        TextRenderer.DrawText(e.Graphics, strColumnText, e.Node.NodeFont, rect, foreColor, flags);
        rect.Y--;
      }
    }

    /// <summary>
    /// Returns the forecolor used to write text in a node basis on the value data type.
    /// </summary>
    /// <param name="stringType">Data type of the node</param>
    /// <returns>Color that corresponds to the data type</returns>
    private Color GetForeColor(string stringType)
    {
      if (string.IsNullOrEmpty(stringType))
      {
        return Color.Black;
      }

      if (stringType.EndsWith("Nullable"))
      {
        return Color.Gray;
      }
      else if ((stringType.EndsWith("[]")))
      {
        return Color.Red;
      }

      string type = string.Format("System.{0}", stringType);
      switch (type)
      {
        case "System.Int16":
        case "System.Int32":
        case "System.Int64":
        case "System.Decimal":
        case "System.Double":
        case "System.SByte":
        case "System.Single":
        case "System.UInt16":
        case "System.UInt32":
        case "System.UInt64":
          return Color.Blue;
        case "System.Byte":
          return Color.LightSeaGreen;
        case "System.String":
        case "System.Char":
          return Color.Green;
        case "System.Boolean":
          return Color.MediumPurple;
        case "System.DateTime":
          return Color.LightBlue;
        case "System.DBNull":
        case "System.Empty":
          return Color.Gray;
        case "System.Object":
          return Color.Orange;
        default:
          return Color.Black;
      }
    }

    /// <summary>
    /// Get the image index basis on the value data type
    /// </summary>
    /// <param name="stringType">Data type of the node</param>
    /// <returns>Image index that corresponds to the data type</returns>
    private int GetImageIndex(string stringType)
    {
      if (string.IsNullOrEmpty(stringType))
      {
        return (int)NodeImageIndex.GenericIcon;
      }

      if (stringType.EndsWith("Nullable"))
      {
        return (int)NodeImageIndex.NullIcon;
      }
      else if ((stringType.EndsWith("[]")))
      {
        return (int)NodeImageIndex.ArrayIcon;
      }

      string type = string.Format("System.{0}", stringType);
      switch (type)
      {
        case "System.Int16":
        case "System.Int32":
        case "System.Int64":
        case "System.Decimal":
        case "System.Double":
        case "System.SByte":
        case "System.Single":
        case "System.UInt16":
        case "System.UInt32":
        case "System.UInt64":
          return (int)NodeImageIndex.NumberIcon;
        case "System.Byte":
          return (int)NodeImageIndex.BinIcon;
        case "System.String":
        case "System.Char":
          return (int)NodeImageIndex.StringIcon;
        case "System.Boolean":
          return (int)NodeImageIndex.BoolIcon;
        case "System.DateTime":
          return (int)NodeImageIndex.DateIcon;
        case "System.DBNull":
        case "System.Empty":
          return (int)NodeImageIndex.NullIcon;
        case "System.Object":
          return (int)NodeImageIndex.ObjectIcon;
        default:
          return (int)NodeImageIndex.GenericIcon;
      }
    }

    /// <summary>
    /// Load the images from the form resources file and set it to the tree view
    /// </summary>
    private void SetImageList()
    {
      ImageList imgList = new ImageList();
      ComponentResourceManager resources = new ComponentResourceManager(typeof(MySqlTreeView));
      //the order of this addition match the value in the NodeImageIndex enum, any update in this order must match the enum items value
      imgList.Images.AddRange(new Image[]{
        (System.Drawing.Image)resources.GetObject("genericIcon"), //0
        (System.Drawing.Image)resources.GetObject("number"),      //1
        (System.Drawing.Image)resources.GetObject("string"),      //2
        (System.Drawing.Image)resources.GetObject("bool"),        //3
        (System.Drawing.Image)resources.GetObject("date"),        //4
        (System.Drawing.Image)resources.GetObject("null"),        //5
        (System.Drawing.Image)resources.GetObject("object"),      //6
        (System.Drawing.Image)resources.GetObject("array"),       //7
        (System.Drawing.Image)resources.GetObject("bin"),         //8
        (System.Drawing.Image)resources.GetObject("objectId")     //9
      });

      btvData.ImageList = imgList;
    }

    /// <summary>
    /// Options of possible Icons that can be used as Node icon
    /// </summary>
    private enum NodeImageIndex : int
    {
      /// <summary>
      /// Gray diamond icon
      /// </summary>
      GenericIcon = 0,
      /// <summary>
      /// Blue hashtag icon
      /// </summary>
      NumberIcon = 1,
      /// <summary>
      /// Green quotes icon
      /// </summary>
      StringIcon = 2,
      /// <summary>
      /// Purple 01 icon
      /// </summary>
      BoolIcon = 3,
      /// <summary>
      /// Light blue watch icon
      /// </summary>
      DateIcon = 4,
      /// <summary>
      /// Gray null icon
      /// </summary>
      NullIcon = 5,
      /// <summary>
      /// Orange braces icon
      /// </summary>
      ObjectIcon = 6,
      /// <summary>
      /// Red brackets icon
      /// </summary>
      ArrayIcon = 7,
      /// <summary>
      /// Light green bin icon
      /// </summary>
      BinIcon = 8,
      /// <summary>
      /// Brown id icon
      /// </summary>
      ObjectIdIcon = 9
    }
  }
}
