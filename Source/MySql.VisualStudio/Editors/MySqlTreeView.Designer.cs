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
  partial class MySqlTreeView
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
      this.lvHeaders = new System.Windows.Forms.ListView();
      this.btvData = new MySql.Data.VisualStudio.Editors.BufferedTreeView();
      this.SuspendLayout();
      // 
      // lvHeaders
      // 
      this.lvHeaders.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.lvHeaders.Dock = System.Windows.Forms.DockStyle.Top;
      this.lvHeaders.Location = new System.Drawing.Point(0, 0);
      this.lvHeaders.Name = "lvHeaders";
      this.lvHeaders.Scrollable = false;
      this.lvHeaders.Size = new System.Drawing.Size(600, 20);
      this.lvHeaders.TabIndex = 0;
      this.lvHeaders.UseCompatibleStateImageBehavior = false;
      this.lvHeaders.View = System.Windows.Forms.View.Details;
      this.lvHeaders.ColumnWidthChanged += new System.Windows.Forms.ColumnWidthChangedEventHandler(this.lvHeaders_ColumnWidthChanged);
      this.lvHeaders.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.lvHeaders_ColumnWidthChanging);
      this.lvHeaders.Click += new System.EventHandler(this.lvHeaders_Click);
      // 
      // btvData
      // 
      this.btvData.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.btvData.Dock = System.Windows.Forms.DockStyle.Fill;
      this.btvData.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawAll;
      this.btvData.Location = new System.Drawing.Point(0, 20);
      this.btvData.Name = "btvData";
      this.btvData.ShowLines = false;
      this.btvData.Size = new System.Drawing.Size(600, 480);
      this.btvData.TabIndex = 2;
      this.btvData.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.btvData_DrawNode);
      this.btvData.Click += new System.EventHandler(this.btvData_Click);
      // 
      // MySqlTreeView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.btvData);
      this.Controls.Add(this.lvHeaders);
      this.Name = "MySqlTreeView";
      this.Size = new System.Drawing.Size(600, 500);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ListView lvHeaders;
    private BufferedTreeView btvData;
  }
}
