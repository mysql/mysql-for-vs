// Copyright (c) 2012, 2018, Oracle and/or its affiliates. All rights reserved.
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License as
// published by the Free Software Foundation; version 2 of the
// License.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301  USA

namespace MySql.Utility.Forms
{
  partial class AutoStyleableBaseDialog
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

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.ContentAreaPanel = new System.Windows.Forms.Panel();
      this.CommandAreaPanel = new System.Windows.Forms.Panel();
      this.FootnoteAreaPanel = new System.Windows.Forms.Panel();
      this.SuspendLayout();
      // 
      // ContentAreaPanel
      // 
      this.ContentAreaPanel.AccessibleDescription = "A panel defining the main area where most of the content will be placed";
      this.ContentAreaPanel.AccessibleName = "Content Area";
      this.ContentAreaPanel.BackColor = System.Drawing.SystemColors.Window;
      this.ContentAreaPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ContentAreaPanel.Location = new System.Drawing.Point(0, 0);
      this.ContentAreaPanel.Name = "ContentAreaPanel";
      this.ContentAreaPanel.Size = new System.Drawing.Size(634, 212);
      this.ContentAreaPanel.TabIndex = 0;
      this.ContentAreaPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.ContentAreaPanel_Paint);
      // 
      // CommandAreaPanel
      // 
      this.CommandAreaPanel.AccessibleDescription = "A panel defining an optional area where action buttons are normally placed";
      this.CommandAreaPanel.AccessibleName = "Command Area";
      this.CommandAreaPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.CommandAreaPanel.Location = new System.Drawing.Point(0, 167);
      this.CommandAreaPanel.Name = "CommandAreaPanel";
      this.CommandAreaPanel.Size = new System.Drawing.Size(634, 45);
      this.CommandAreaPanel.TabIndex = 1;
      this.CommandAreaPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.CommandAreaPanel_Paint);
      // 
      // FootnoteAreaPanel
      // 
      this.FootnoteAreaPanel.AccessibleDescription = "A panel defining an optional area at the very bottom of the form";
      this.FootnoteAreaPanel.AccessibleName = "Footnote Area";
      this.FootnoteAreaPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.FootnoteAreaPanel.Location = new System.Drawing.Point(0, 212);
      this.FootnoteAreaPanel.Name = "FootnoteAreaPanel";
      this.FootnoteAreaPanel.Size = new System.Drawing.Size(634, 80);
      this.FootnoteAreaPanel.TabIndex = 2;
      this.FootnoteAreaPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.FootNoteAreaPanel_Paint);
      // 
      // AutoStyleableBaseDialog
      // 
      this.AccessibleDescription = "A base class for forms or dialogs that can have a body, action and footer areas";
      this.AccessibleName = "Auto Styleable Dialog";
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(634, 292);
      this.Controls.Add(this.CommandAreaPanel);
      this.Controls.Add(this.ContentAreaPanel);
      this.Controls.Add(this.FootnoteAreaPanel);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "AutoStyleableBaseDialog";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "AutoStyleableDialog";
      this.ResumeLayout(false);

    }

    #endregion

    protected System.Windows.Forms.Panel FootnoteAreaPanel;
    protected System.Windows.Forms.Panel ContentAreaPanel;
    protected System.Windows.Forms.Panel CommandAreaPanel;
  }
}