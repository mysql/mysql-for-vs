// Copyright (c) 2008, 2015, Oracle and/or its affiliates. All rights reserved.
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

namespace MySql.Data.VisualStudio.Wizards.WindowsForms
{
  partial class WindowsFormsWizardForm
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
      this.dataAccessConfig1 = new DataAccessConfig();
      this.tablesSelection1 = new TablesSelectionWinForms();
      this.SuspendLayout();
      // 
      // btnNext
      // 
      this.btnNext.Location = new System.Drawing.Point(702, 475);
      // 
      // btnCancel
      // 
      this.btnCancel.Location = new System.Drawing.Point(540, 475);
      // 
      // btnBack
      // 
      this.btnBack.Location = new System.Drawing.Point(621, 475);
      // 
      // dataAccessConfig1
      // 
      this.dataAccessConfig1.Font = new System.Drawing.Font("Segoe UI", 8.25F);
      this.dataAccessConfig1.Location = new System.Drawing.Point(260, 83);
      this.dataAccessConfig1.Name = "dataAccessConfig1";
      this.dataAccessConfig1.Size = new System.Drawing.Size(610, 380);
      this.dataAccessConfig1.TabIndex = 22;
      // 
      // tablesSelection1
      // 
      this.tablesSelection1.Font = new System.Drawing.Font("Segoe UI", 8.25F);
      this.tablesSelection1.Location = new System.Drawing.Point(259, 84);
      this.tablesSelection1.Name = "tablesSelection1";
      this.tablesSelection1.Size = new System.Drawing.Size(599, 380);
      this.tablesSelection1.TabIndex = 23;
      // 
      // WindowsFormsWizardForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(873, 510);
      this.Controls.Add(this.tablesSelection1);
      this.Controls.Add(this.dataAccessConfig1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.MinimizeBox = false;
      this.Name = "WindowsFormsWizardForm";
      this.Text = "Windows Forms Configuration ";
      this.Load += new System.EventHandler(this.WizardForm_Load);
      this.Controls.SetChildIndex(this.btnFinish, 0);
      this.Controls.SetChildIndex(this.btnNext, 0);
      this.Controls.SetChildIndex(this.btnBack, 0);
      this.Controls.SetChildIndex(this.btnCancel, 0);
      this.Controls.SetChildIndex(this.dataAccessConfig1, 0);
      this.Controls.SetChildIndex(this.tablesSelection1, 0);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private DataAccessConfig dataAccessConfig1;
    private TablesSelectionWinForms tablesSelection1;
  }
}