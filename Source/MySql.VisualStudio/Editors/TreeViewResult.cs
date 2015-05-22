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
