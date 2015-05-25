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
using MySqlX.Shell;

namespace MySql.Data.VisualStudio.Editors
{
  public partial class TreeViewResult : UserControl
  {
    public TreeViewResult()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data to the tree view
    /// </summary>
    /// <param name="document">Data to load</param>
    public void SetData(DocumentResultSet document)
    {
      GenerateItem(document.GetData());
    }

    /// <summary>
    /// Create tree nodes from the data received to add it to the tree view and display the information
    /// </summary>
    /// <param name="rows">Data to generate the tree nodes</param>
    private void GenerateItem(List<Dictionary<string, object>> rows)
    {
      int rowCtr = 1;
      string nodeId = "";
      TreeNode[] currnodes = null;
      foreach (Dictionary<string, object> row in rows)
      {
        nodeId = string.Format("row_{0}", rowCtr);
        tvData.Nodes.Add(nodeId, string.Format("Row {0}", rowCtr));
        foreach(KeyValuePair<string, object> column in row)
        {
          currnodes = tvData.Nodes.Find(nodeId, true);
          currnodes[0].Nodes.Add(new TreeNode(column.Key, new TreeNode[] { new TreeNode(column.Value.ToString()) }));
        }
        rowCtr++;
      }
    }
  }
}
