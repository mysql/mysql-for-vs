// Copyright © 2016, Oracle and/or its affiliates. All rights reserved.
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
  partial class BaseShellConsoleEditor
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
      this.components = new System.ComponentModel.Container();
      this.rtbMessages = new System.Windows.Forms.RichTextBox();
      this.toolTipCommand = new System.Windows.Forms.ToolTip(this.components);
      this.lblPrompt = new System.Windows.Forms.Label();
      this.txtInput = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // rtbMessages
      // 
      this.rtbMessages.BackColor = System.Drawing.SystemColors.Window;
      this.rtbMessages.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.rtbMessages.Dock = System.Windows.Forms.DockStyle.Top;
      this.rtbMessages.Location = new System.Drawing.Point(0, 0);
      this.rtbMessages.Name = "rtbMessages";
      this.rtbMessages.ReadOnly = true;
      this.rtbMessages.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
      this.rtbMessages.Size = new System.Drawing.Size(200, 78);
      this.rtbMessages.TabIndex = 2;
      this.rtbMessages.TabStop = false;
      this.rtbMessages.Text = "";
      this.rtbMessages.Click += new System.EventHandler(this.rtbMessages_Click);
      this.rtbMessages.TextChanged += new System.EventHandler(this.rtbMessages_TextChanged);
      // 
      // toolTipCommand
      // 
      this.toolTipCommand.AutoPopDelay = 5000;
      this.toolTipCommand.InitialDelay = 0;
      this.toolTipCommand.ReshowDelay = 100;
      // 
      // lblPrompt
      // 
      this.lblPrompt.AutoSize = true;
      this.lblPrompt.BackColor = System.Drawing.SystemColors.Window;
      this.lblPrompt.Dock = System.Windows.Forms.DockStyle.Left;
      this.lblPrompt.Location = new System.Drawing.Point(0, 78);
      this.lblPrompt.Margin = new System.Windows.Forms.Padding(0);
      this.lblPrompt.Name = "lblPrompt";
      this.lblPrompt.Size = new System.Drawing.Size(14, 14);
      this.lblPrompt.TabIndex = 0;
      this.lblPrompt.Text = ">";
      // 
      // txtInput
      // 
      this.txtInput.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.txtInput.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtInput.Location = new System.Drawing.Point(14, 78);
      this.txtInput.Name = "txtInput";
      this.txtInput.Size = new System.Drawing.Size(186, 13);
      this.txtInput.TabIndex = 1;
      this.txtInput.TextChanged += new System.EventHandler(this.txtInput_TextChanged);
      this.txtInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtInput_KeyDown);
      this.txtInput.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtInput_KeyPress);
      // 
      // BaseShellConsoleEditor
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.txtInput);
      this.Controls.Add(this.lblPrompt);
      this.Controls.Add(this.rtbMessages);
      this.Font = new System.Drawing.Font("Courier New", 8F);
      this.MinimumSize = new System.Drawing.Size(0, 17);
      this.Name = "BaseShellConsoleEditor";
      this.Size = new System.Drawing.Size(200, 95);
      this.BackColorChanged += new System.EventHandler(this.Prompt_BackColorChanged);
      this.FontChanged += new System.EventHandler(this.Prompt_FontChanged);
      this.ForeColorChanged += new System.EventHandler(this.Prompt_ForeColorChanged);
      this.Resize += new System.EventHandler(this.CommandPrompt_Resize);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion
    private System.Windows.Forms.RichTextBox rtbMessages;
    private System.Windows.Forms.ToolTip toolTipCommand;
    private System.Windows.Forms.TextBox txtInput;
    private System.Windows.Forms.Label lblPrompt;
  }
}
