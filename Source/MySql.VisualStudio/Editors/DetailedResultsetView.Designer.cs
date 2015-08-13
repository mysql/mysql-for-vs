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

namespace MySql.Data.VisualStudio.Editors
{
  partial class DetailedResultsetView
  {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.ctrlMenu = new MySql.Data.VisualStudio.Editors.VerticalMenu();
      this.ctrlQueryStats = new MySql.Data.VisualStudio.Editors.QueryStatsView();
      this.ctrlExecPlan = new MySql.Data.VisualStudio.Editors.ExecutionPlanView();
      this.ctrlFieldtypes = new MySql.Data.VisualStudio.Editors.FieldTypesGrid();
      this.ctrlResultSet = new MySql.Data.VisualStudio.Editors.GridResultSet();
      this.SuspendLayout();
      // 
      // ctrlMenu
      // 
      this.ctrlMenu.Dock = System.Windows.Forms.DockStyle.Right;
      this.ctrlMenu.Location = new System.Drawing.Point(600, 0);
      this.ctrlMenu.Name = "ctrlMenu";
      this.ctrlMenu.Size = new System.Drawing.Size(58, 500);
      this.ctrlMenu.TabIndex = 0;
      // 
      // ctrlQueryStats
      // 
      this.ctrlQueryStats.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ctrlQueryStats.Location = new System.Drawing.Point(0, 0);
      this.ctrlQueryStats.Name = "ctrlQueryStats";
      this.ctrlQueryStats.Size = new System.Drawing.Size(600, 500);
      this.ctrlQueryStats.TabIndex = 1;
      // 
      // ctrlExecPlan
      // 
      this.ctrlExecPlan.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ctrlExecPlan.Location = new System.Drawing.Point(0, 0);
      this.ctrlExecPlan.Name = "ctrlExecPlan";
      this.ctrlExecPlan.Size = new System.Drawing.Size(600, 500);
      this.ctrlExecPlan.TabIndex = 2;
      // 
      // ctrlFieldtypes
      // 
      this.ctrlFieldtypes.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ctrlFieldtypes.Location = new System.Drawing.Point(0, 0);
      this.ctrlFieldtypes.Name = "ctrlFieldtypes";
      this.ctrlFieldtypes.Size = new System.Drawing.Size(600, 500);
      this.ctrlFieldtypes.TabIndex = 3;
      // 
      // ctrlResultSet
      // 
      this.ctrlResultSet.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ctrlResultSet.Location = new System.Drawing.Point(0, 0);
      this.ctrlResultSet.Name = "ctrlResultSet";
      this.ctrlResultSet.Size = new System.Drawing.Size(600, 500);
      this.ctrlResultSet.TabIndex = 4;
      // 
      // DetailedResultsetView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.ctrlResultSet);
      this.Controls.Add(this.ctrlFieldtypes);
      this.Controls.Add(this.ctrlExecPlan);
      this.Controls.Add(this.ctrlQueryStats);
      this.Controls.Add(this.ctrlMenu);
      this.Name = "DetailedResultsetView";
      this.Size = new System.Drawing.Size(658, 500);
      this.Load += new System.EventHandler(this.DetailedResultsetView_Load);
      this.ResumeLayout(false);

    }

    #endregion

    private VerticalMenu ctrlMenu;
    private QueryStatsView ctrlQueryStats;
    private ExecutionPlanView ctrlExecPlan;
    private FieldTypesGrid ctrlFieldtypes;
    private GridResultSet ctrlResultSet;


  }
}
