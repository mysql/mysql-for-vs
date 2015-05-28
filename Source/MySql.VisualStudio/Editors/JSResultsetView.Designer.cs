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
  partial class JSResultsetView
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
      this.ctrlGridView = new MySql.Data.VisualStudio.Editors.GridViewResult();
      this.ctrlTreeView = new MySql.Data.VisualStudio.Editors.TreeViewResult();
      this.ctrlMenu = new MySql.Data.VisualStudio.Editors.VerticalMenu();
      this.ctrlTextView = new MySql.Data.VisualStudio.Editors.TextViewPane();
      this.SuspendLayout();
      // 
      // ctrlGridView
      // 
      this.ctrlGridView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ctrlGridView.Location = new System.Drawing.Point(0, 0);
      this.ctrlGridView.Name = "ctrlGridView";
      this.ctrlGridView.Size = new System.Drawing.Size(600, 500);
      this.ctrlGridView.TabIndex = 3;
      // 
      // ctrlTreeView
      // 
      this.ctrlTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ctrlTreeView.Location = new System.Drawing.Point(0, 0);
      this.ctrlTreeView.Name = "ctrlTreeView";
      this.ctrlTreeView.Size = new System.Drawing.Size(600, 500);
      this.ctrlTreeView.TabIndex = 2;
      // 
      // ctrlMenu
      // 
      this.ctrlMenu.Dock = System.Windows.Forms.DockStyle.Right;
      this.ctrlMenu.Location = new System.Drawing.Point(600, 0);
      this.ctrlMenu.Name = "ctrlMenu";
      this.ctrlMenu.Size = new System.Drawing.Size(58, 500);
      this.ctrlMenu.TabIndex = 0;
      // 
      // ctrlTextView
      // 
      this.ctrlTextView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ctrlTextView.Location = new System.Drawing.Point(0, 0);
      this.ctrlTextView.Name = "ctrlTextView";
      this.ctrlTextView.Size = new System.Drawing.Size(658, 500);
      this.ctrlTextView.TabIndex = 1;
      // 
      // JSResultsetView
      // 
      this.Controls.Add(this.ctrlGridView);
      this.Controls.Add(this.ctrlTreeView);
      this.Controls.Add(this.ctrlMenu);
      this.Controls.Add(this.ctrlTextView);
      this.Name = "JSResultsetView";
      this.Size = new System.Drawing.Size(658, 500);
      this.ResumeLayout(false);

    }

    #endregion

    private VerticalMenu ctrlMenu;
    private TextViewPane ctrlTextView;
    private TreeViewResult ctrlTreeView;
    private GridViewResult ctrlGridView;
  }
}
