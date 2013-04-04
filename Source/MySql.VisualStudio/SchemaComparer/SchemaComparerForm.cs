// Copyright © 2008, 2013, Oracle and/or its affiliates. All rights reserved.
//
// MySQL Connector/NET is licensed under the terms of the GPLv2
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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;


namespace MySql.Data.VisualStudio.SchemaComparer
{
  public partial class SchemaComparerForm : Form
  {
    public SchemaComparerForm()
    {
      InitializeComponent();
    }

    internal MySqlConnection SourceConnection { get; set; }

    internal MySqlConnection DestinyConnection { get; set; }

    private Comparer cmp;
    private ComparerResult result;

    private void EnsureConnectionsOpened()
    {
      if ((SourceConnection.State & ConnectionState.Open) == 0)
        SourceConnection.Open();
      if ((DestinyConnection.State & ConnectionState.Open) == 0)
        DestinyConnection.Open();
    }

    private void SchemaComparerForm_Load(object sender, EventArgs e)
    {
      // TODO: Connections for now hardcoded, until Server Explorer connection context menu is enabled:
      MySqlConnection src = new MySqlConnection("server=localhost; userid=root; database=DbCmp1;");
      MySqlConnection dst = new MySqlConnection("server=localhost; userid=root; database=DbCmp2;");
      SourceConnection = src;
      DestinyConnection = dst;

      DataTable tbl = new DataTable();
      // Left Script, Item (db1.Custmers, Table), Status, Right Script
      tbl.Columns.Add("Left Script", typeof(string));
      tbl.Columns.Add("Item", typeof(string));
      tbl.Columns.Add("Status", typeof(string));
      tbl.Columns.Add("Right Script", typeof(string));
      
      // Populate table:
      EnsureConnectionsOpened();
      cmp = new Comparer(SourceConnection, DestinyConnection);
      result = cmp.Compare();

      foreach (ComparerResultItem cri in result.DiffsInSrc)
      {
        DataRow dr = tbl.NewRow();
        dr["Left Script"] = cri.MtObject.GetScript(cri.Type);
        dr["Item"] = cri.MtObject.FullName;
        dr["Status"] = cri.Type.ToString();
        dr["Right Script"] = "";
        tbl.Rows.Add(dr);
      }

      foreach (ComparerResultItem cri in result.DiffsInDst)
      {
        DataRow dr = tbl.NewRow();
        dr["Left Script"] = "";
        dr["Item"] = cri.MtObject.FullName;
        dr["Status"] = cri.Type.ToString();
        dr["Right Script"] = cri.MtObject.GetScript(cri.Type);
        tbl.Rows.Add(dr);
      }

      // Populate results back to grid
      dgDiffSummary.DataSource = tbl;
      foreach (DataGridViewRow row in dgDiffSummary.Rows)
      {
        if ((string)row.Cells[0].Value == "")
        {
          row.DefaultCellStyle.BackColor = Color.Red;
          //row.Cells[0].ToolTipText = "";
        }
      }
    }

    private void btnGetLeftChange_Click(object sender, EventArgs e)
    {
      DataGridViewRow row = dgDiffSummary.SelectedRows[0];
      if (row == null) return;
      string sql = ( string )row.Cells[3].Value;
      if (string.IsNullOrEmpty(sql)) return;
      MySqlScriptDialog dlg = new MySqlScriptDialog();
      dlg.TextScript = sql;
      dlg.ShowDialog();
    }

    private void btnGetAllLeftChanges_Click(object sender, EventArgs e)
    {
      string sqlSrc;
      string sqlDst;

      cmp.GetScript( result, true, out sqlSrc, out sqlDst );
      MySqlScriptDialog dlg = new MySqlScriptDialog();
      dlg.TextScript = sqlSrc;
      dlg.Title = "Scripts to apply in Right Database";
      dlg.ShowDialog();

      //dlg.Text = sqlDst;
      //dlg.Title = "Scripts to apply in Left Database";
      //dlg.ShowDialog();
    }

    private void btnGetRightChange_Click(object sender, EventArgs e)
    {
      DataGridViewRow row = dgDiffSummary.SelectedRows[0];
      if (row == null) return;
      string sql = (string)row.Cells[0].Value;
      if (string.IsNullOrEmpty(sql)) return;
      MySqlScriptDialog dlg = new MySqlScriptDialog();
      dlg.TextScript = sql;
      dlg.ShowDialog();
    }

    private void btnGetAllRightChanges_Click(object sender, EventArgs e)
    {
      string sqlSrc;
      string sqlDst;

      cmp.GetScript(result, false, out sqlSrc, out sqlDst);
      MySqlScriptDialog dlg = new MySqlScriptDialog();
      dlg.TextScript = sqlSrc;
      dlg.Title = "Scripts to apply in Left Database";
      dlg.ShowDialog();

      //dlg.Text = sqlDst;
      //dlg.Title = "Scripts to apply in Right Database";
      //dlg.ShowDialog();
    }
  }
}
