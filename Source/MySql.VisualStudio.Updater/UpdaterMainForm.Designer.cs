// Copyright (c) 2019, Oracle and/or its affiliates. All rights reserved.
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

namespace MySql.VisualStudio.Updater
{
  partial class UpdaterMainForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdaterMainForm));
      this.UpdateConfigurationProgressBar = new System.Windows.Forms.ProgressBar();
      this.CloseButton = new System.Windows.Forms.Button();
      this.ConfigurationDetailsTextBox = new System.Windows.Forms.TextBox();
      this.RestartLabel = new System.Windows.Forms.Label();
      this.LogoPictureBox = new System.Windows.Forms.PictureBox();
      this.TitleLabel = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.LogoPictureBox)).BeginInit();
      this.SuspendLayout();
      // 
      // UpdateConfigurationProgressBar
      // 
      this.UpdateConfigurationProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.UpdateConfigurationProgressBar.Location = new System.Drawing.Point(12, 317);
      this.UpdateConfigurationProgressBar.Name = "UpdateConfigurationProgressBar";
      this.UpdateConfigurationProgressBar.Size = new System.Drawing.Size(554, 28);
      this.UpdateConfigurationProgressBar.TabIndex = 2;
      // 
      // CloseButton
      // 
      this.CloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.CloseButton.Enabled = false;
      this.CloseButton.Location = new System.Drawing.Point(576, 320);
      this.CloseButton.Name = "CloseButton";
      this.CloseButton.Size = new System.Drawing.Size(75, 25);
      this.CloseButton.TabIndex = 3;
      this.CloseButton.Text = "Close";
      this.CloseButton.UseVisualStyleBackColor = true;
      this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
      // 
      // ConfigurationDetailsTextBox
      // 
      this.ConfigurationDetailsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.ConfigurationDetailsTextBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
      this.ConfigurationDetailsTextBox.Location = new System.Drawing.Point(12, 83);
      this.ConfigurationDetailsTextBox.Multiline = true;
      this.ConfigurationDetailsTextBox.Name = "ConfigurationDetailsTextBox";
      this.ConfigurationDetailsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.ConfigurationDetailsTextBox.Size = new System.Drawing.Size(636, 190);
      this.ConfigurationDetailsTextBox.TabIndex = 1;
      // 
      // RestartLabel
      // 
      this.RestartLabel.AutoSize = true;
      this.RestartLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.RestartLabel.ForeColor = System.Drawing.Color.Navy;
      this.RestartLabel.Location = new System.Drawing.Point(12, 288);
      this.RestartLabel.Name = "RestartLabel";
      this.RestartLabel.Size = new System.Drawing.Size(0, 17);
      this.RestartLabel.TabIndex = 4;
      // 
      // LogoPictureBox
      // 
      this.LogoPictureBox.Image = global::MySql.VisualStudio.Updater.Properties.Resources.MySQLforVisualStudio;
      this.LogoPictureBox.Location = new System.Drawing.Point(13, 13);
      this.LogoPictureBox.Name = "LogoPictureBox";
      this.LogoPictureBox.Size = new System.Drawing.Size(64, 64);
      this.LogoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
      this.LogoPictureBox.TabIndex = 1;
      this.LogoPictureBox.TabStop = false;
      // 
      // TitleLabel
      // 
      this.TitleLabel.AccessibleDescription = "";
      this.TitleLabel.AccessibleName = "";
      this.TitleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.TitleLabel.AutoEllipsis = true;
      this.TitleLabel.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.TitleLabel.ForeColor = System.Drawing.Color.Navy;
      this.TitleLabel.Location = new System.Drawing.Point(83, 19);
      this.TitleLabel.Name = "TitleLabel";
      this.TitleLabel.Size = new System.Drawing.Size(461, 20);
      this.TitleLabel.TabIndex = 5;
      this.TitleLabel.Text = "Configuration Update Tool";
      // 
      // UpdaterMainForm
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
      this.ClientSize = new System.Drawing.Size(660, 357);
      this.ControlBox = false;
      this.Controls.Add(this.TitleLabel);
      this.Controls.Add(this.RestartLabel);
      this.Controls.Add(this.ConfigurationDetailsTextBox);
      this.Controls.Add(this.CloseButton);
      this.Controls.Add(this.UpdateConfigurationProgressBar);
      this.Controls.Add(this.LogoPictureBox);
      this.Font = new System.Drawing.Font("Segoe UI", 9F);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "UpdaterMainForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "MySQL for Visual Studio Configuration Update Tool";
      ((System.ComponentModel.ISupportInitialize)(this.LogoPictureBox)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion
    private System.Windows.Forms.PictureBox LogoPictureBox;
    private System.Windows.Forms.ProgressBar UpdateConfigurationProgressBar;
    private System.Windows.Forms.Button CloseButton;
    private System.Windows.Forms.TextBox ConfigurationDetailsTextBox;
    private System.Windows.Forms.Label RestartLabel;
    private System.Windows.Forms.Label TitleLabel;
  }
}

