// Copyright (c) 2008, 2021, Oracle and/or its affiliates.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License, version 2.0, as
// published by the Free Software Foundation.
//
// This program is also distributed with certain software (including
// but not limited to OpenSSL) that is licensed under separate terms,
// as designated in a particular file or component or in included license
// documentation.  The authors of MySQL hereby grant you an
// additional permission to link the program and your derivative works
// with the separately licensed software that they have included with
// MySQL.
//
// Without limiting anything contained in the foregoing, this file,
// which is part of MySQL for Visual Studio, is also subject to the
// Universal FOSS Exception, version 1.0, a copy of which can be found at
// http://oss.oracle.com/licenses/universal-foss-exception.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU General Public License, version 2.0, for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software Foundation, Inc.,
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
using MySql.Data.VisualStudio.Common;

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
      {
        SourceConnection.OpenWithDefaultTimeout();
      }

      if ((DestinyConnection.State & ConnectionState.Open) == 0)
      {
        DestinyConnection.OpenWithDefaultTimeout();
      }
    }

    private void SchemaComparerForm_Load(object sender, EventArgs e)
    {
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
        //if ( ((string)row.Cells[0].Value == "") && (( string )row.Cells[ 3 ].Value != "" ))
        //{
        //  // differences in right side
        //  row.DefaultCellStyle.BackColor = Color.Red;          
        //}
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
