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
using System.Runtime.Serialization;
using MySqlX;
using MySqlX.Shell;

namespace MySql.Data.VisualStudio.Editors
{
  /// <summary>
  /// View to display data in a TreeView
  /// </summary>
  public partial class TreeViewResult : UserControl
  {
    /// <summary>
    /// Object used to display the data as TreeView with columns
    /// </summary>
    private MySqlTreeView _treeData;

    /// <summary>
    /// Creates an instance of TreeViewResult
    /// </summary>
    public TreeViewResult()
    {
      InitializeComponent();
      SetUp();
    }

    /// <summary>
    /// Load data to the tree view
    /// </summary>
    /// <param name="data">Data to load</param>
    public void SetData(List<Dictionary<string, object>> data)
    {
      GenerateTreeView(data);
    }

    /// <summary>
    /// Load data to the tree view
    /// </summary>
    /// <param name="document">Data to load</param>
    public void SetData(DocResult document)
    {
      SetData(document.FetchAll());
    }

    /// <summary>
    /// Create tree nodes from the data received to add it to the tree view and display the information
    /// </summary>
    /// <param name="rows">Data to generate the tree nodes</param>
    private void GenerateTreeView(List<Dictionary<string, object>> rows)
    {
      if (rows == null)
      {
        return;
      }

      int rowCounter = 1;
      string nodeId = "";
      foreach (Dictionary<string, object> row in rows)
      {
        nodeId = string.Format("row_{0}", rowCounter);
        TreeNode parentNode = new TreeNode(string.Format("Row {0}", rowCounter));
        parentNode.Name = nodeId;
        parentNode.Tag = new string[] { string.Format("{0} Fields", row.Count), "Object" };
        parentNode.ImageIndex = 0;

        foreach (KeyValuePair<string, object> column in row)
        {
          TreeNode node = GenerateNode(column);
          if (node != null)
          {
            parentNode.Nodes.Add(node);
          }
        }

        _treeData.TreeView.Nodes.Add(parentNode);
        rowCounter++;
      }
    }

    /// <summary>
    /// Create a TreeView Node from the data received
    /// </summary>
    /// <param name="column">Object that contains the Node text as well as the value to be displayed</param>
    /// <returns></returns>
    private TreeNode GenerateNode(KeyValuePair<string, object> column)
    {
      //since KeyValuePair is a non-nullable type, we verify if it has default values assigned if it then discard this item
      if (column.Equals(default(KeyValuePair<string, object>)))
      {
        return null;
      }

      TreeNode parentNode = new TreeNode();
      parentNode.ImageIndex = 0;
      if (column.Value == null || column.Value.GetType() != typeof(Dictionary<string, object>))
      {
        parentNode.Text = column.Key;
        parentNode.Tag = new string[] { GetNodeValue(column.Value), GetNodeType(column.Value) };
      }
      else
      {
        var cols = (Dictionary<string, object>)column.Value;
        parentNode.Text = column.Key;
        parentNode.Tag = new string[] { string.Format("{0} Fields", cols.Count), "Object" };
        foreach (KeyValuePair<string, object> col in cols)
        {
          parentNode.Nodes.Add(GenerateNode(col));
        }
      }

      return parentNode;
    }

    /// <summary>
    /// Get the data that will be displayed as the Node value
    /// </summary>
    /// <param name="value">Object that contains the value to be displayed</param>
    /// <returns>A string representation of the value when the object is not a type of Array otherwise a string with the item count is returned</returns>
    private string GetNodeValue(object value)
    {
      if (value == null)
      {
        return "Null";
      }

      Type valType = value.GetType();
      if (valType.BaseType == typeof(Array))
      {
        return string.Format("{0} Items", ((Array)value).Length);
      }
      else
      {
        return value.ToString();
      }
    }

    /// <summary>
    /// Get the type of the value that will be displayed
    /// </summary>
    /// <param name="value">Object that contains the value</param>
    /// <returns>String representation of the data type</returns>
    private string GetNodeType(object value)
    {
      if (value == null)
      {
        return typeof(Nullable).ToString().Replace("System.", "");
      }

      //currently the shell is handling the dates as string, we need to try to parse it to verify if is a valid date
      DateTime tmpDate;
      if (DateTime.TryParse(value.ToString(), out tmpDate))
      {
        return tmpDate.GetType().ToString().Replace("System.", "");
      }

      return value.GetType().ToString().Replace("System.", "");
    }

    /// <summary>
    /// Initializes controls that will display the data in the view
    /// </summary>
    private void SetUp()
    {
      _treeData = new MySqlTreeView("Field", "Value", "Type");
      _treeData.Location = new Point(0, 0);
      _treeData.Dock = DockStyle.Fill;
      this.Controls.Add(_treeData);
    }
  }
}
