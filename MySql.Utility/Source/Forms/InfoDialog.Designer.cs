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
  partial class InfoDialog
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
      this.components = new System.ComponentModel.Container();
      this.Info1Button = new System.Windows.Forms.Button();
      this.MoreInfoButton = new System.Windows.Forms.Button();
      this.LogoPictureBox = new System.Windows.Forms.PictureBox();
      this.TitleLabel = new System.Windows.Forms.Label();
      this.DetailLabel = new System.Windows.Forms.Label();
      this.DetailSubLabel = new System.Windows.Forms.Label();
      this.MoreInfoTextBox = new System.Windows.Forms.TextBox();
      this.Info2Button = new System.Windows.Forms.Button();
      this.DefaultButtonTimer = new System.Windows.Forms.Timer(this.components);
      this.Info3Button = new System.Windows.Forms.Button();
      this.InfoCheckBox = new System.Windows.Forms.CheckBox();
      this.InfoComboBox = new System.Windows.Forms.ComboBox();
      this.ContentAreaPanel.SuspendLayout();
      this.CommandAreaPanel.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.LogoPictureBox)).BeginInit();
      this.SuspendLayout();
      // 
      // FootnoteAreaPanel
      // 
      this.FootnoteAreaPanel.Location = new System.Drawing.Point(0, 292);
      this.FootnoteAreaPanel.Size = new System.Drawing.Size(634, 0);
      // 
      // ContentAreaPanel
      // 
      this.ContentAreaPanel.Controls.Add(this.MoreInfoTextBox);
      this.ContentAreaPanel.Controls.Add(this.DetailSubLabel);
      this.ContentAreaPanel.Controls.Add(this.DetailLabel);
      this.ContentAreaPanel.Controls.Add(this.LogoPictureBox);
      this.ContentAreaPanel.Controls.Add(this.TitleLabel);
      this.ContentAreaPanel.Size = new System.Drawing.Size(564, 312);
      // 
      // CommandAreaPanel
      // 
      this.CommandAreaPanel.Controls.Add(this.InfoComboBox);
      this.CommandAreaPanel.Controls.Add(this.InfoCheckBox);
      this.CommandAreaPanel.Controls.Add(this.Info3Button);
      this.CommandAreaPanel.Controls.Add(this.Info2Button);
      this.CommandAreaPanel.Controls.Add(this.MoreInfoButton);
      this.CommandAreaPanel.Controls.Add(this.Info1Button);
      this.CommandAreaPanel.Location = new System.Drawing.Point(0, 267);
      this.CommandAreaPanel.Size = new System.Drawing.Size(564, 45);
      // 
      // Info1Button
      // 
      this.Info1Button.AccessibleDescription = "A generic action button displayed first from right to left in the command area";
      this.Info1Button.AccessibleName = "Button One";
      this.Info1Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.Info1Button.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.Info1Button.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Info1Button.Location = new System.Drawing.Point(477, 11);
      this.Info1Button.Name = "Info1Button";
      this.Info1Button.Size = new System.Drawing.Size(75, 23);
      this.Info1Button.TabIndex = 0;
      this.Info1Button.Text = "Button1";
      this.Info1Button.UseVisualStyleBackColor = true;
      // 
      // MoreInfoButton
      // 
      this.MoreInfoButton.AccessibleDescription = "An optional button at the left side of the command area that expands the dialog t" +
    "o show more information";
      this.MoreInfoButton.AccessibleName = "More Information";
      this.MoreInfoButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.MoreInfoButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.MoreInfoButton.Location = new System.Drawing.Point(12, 11);
      this.MoreInfoButton.Name = "MoreInfoButton";
      this.MoreInfoButton.Size = new System.Drawing.Size(109, 23);
      this.MoreInfoButton.TabIndex = 3;
      this.MoreInfoButton.Text = "Show Details";
      this.MoreInfoButton.UseVisualStyleBackColor = true;
      this.MoreInfoButton.Click += new System.EventHandler(this.MoreInfoButton_Click);
      // 
      // LogoPictureBox
      // 
      this.LogoPictureBox.AccessibleDescription = "A picture box displaying an icon related to the consumer application and the dial" +
    "og\'s purpose";
      this.LogoPictureBox.AccessibleName = "Icon";
      this.LogoPictureBox.Location = new System.Drawing.Point(21, 22);
      this.LogoPictureBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.LogoPictureBox.Name = "LogoPictureBox";
      this.LogoPictureBox.Size = new System.Drawing.Size(64, 64);
      this.LogoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
      this.LogoPictureBox.TabIndex = 24;
      this.LogoPictureBox.TabStop = false;
      // 
      // TitleLabel
      // 
      this.TitleLabel.AccessibleDescription = "A generic title label";
      this.TitleLabel.AccessibleName = "Title";
      this.TitleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.TitleLabel.AutoEllipsis = true;
      this.TitleLabel.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.TitleLabel.ForeColor = System.Drawing.Color.Navy;
      this.TitleLabel.Location = new System.Drawing.Point(91, 29);
      this.TitleLabel.Name = "TitleLabel";
      this.TitleLabel.Size = new System.Drawing.Size(461, 20);
      this.TitleLabel.TabIndex = 0;
      this.TitleLabel.Text = "Title Text";
      // 
      // DetailLabel
      // 
      this.DetailLabel.AccessibleDescription = "A generic detail text";
      this.DetailLabel.AccessibleName = "Detail Text";
      this.DetailLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.DetailLabel.AutoEllipsis = true;
      this.DetailLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.DetailLabel.Location = new System.Drawing.Point(92, 56);
      this.DetailLabel.Name = "DetailLabel";
      this.DetailLabel.Size = new System.Drawing.Size(460, 15);
      this.DetailLabel.TabIndex = 1;
      this.DetailLabel.Text = "Information detail text.";
      // 
      // DetailSubLabel
      // 
      this.DetailSubLabel.AccessibleDescription = "A generic sub detail text";
      this.DetailSubLabel.AccessibleName = "Sub Detail Text";
      this.DetailSubLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.DetailSubLabel.AutoEllipsis = true;
      this.DetailSubLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.DetailSubLabel.Location = new System.Drawing.Point(92, 73);
      this.DetailSubLabel.Name = "DetailSubLabel";
      this.DetailSubLabel.Size = new System.Drawing.Size(460, 15);
      this.DetailSubLabel.TabIndex = 2;
      this.DetailSubLabel.Text = "Sub detail text.";
      // 
      // MoreInfoTextBox
      // 
      this.MoreInfoTextBox.AccessibleDescription = "A text box showing more information";
      this.MoreInfoTextBox.AccessibleName = "More Information Text";
      this.MoreInfoTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.MoreInfoTextBox.BackColor = System.Drawing.SystemColors.Window;
      this.MoreInfoTextBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.MoreInfoTextBox.Location = new System.Drawing.Point(95, 102);
      this.MoreInfoTextBox.Multiline = true;
      this.MoreInfoTextBox.Name = "MoreInfoTextBox";
      this.MoreInfoTextBox.ReadOnly = true;
      this.MoreInfoTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.MoreInfoTextBox.Size = new System.Drawing.Size(376, 140);
      this.MoreInfoTextBox.TabIndex = 3;
      this.MoreInfoTextBox.WordWrap = false;
      // 
      // Info2Button
      // 
      this.Info2Button.AccessibleDescription = "A generic action button displayed second from right to left in the command area";
      this.Info2Button.AccessibleName = "Button Two";
      this.Info2Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.Info2Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.Info2Button.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Info2Button.Location = new System.Drawing.Point(396, 11);
      this.Info2Button.Name = "Info2Button";
      this.Info2Button.Size = new System.Drawing.Size(75, 23);
      this.Info2Button.TabIndex = 1;
      this.Info2Button.Text = "Button2";
      this.Info2Button.UseVisualStyleBackColor = true;
      // 
      // DefaultButtonTimer
      // 
      this.DefaultButtonTimer.Interval = 1000;
      this.DefaultButtonTimer.Tick += new System.EventHandler(this.DefaultButtonTimer_Tick);
      // 
      // Info3Button
      // 
      this.Info3Button.AccessibleDescription = "A generic action button displayed third from right to left in the command area";
      this.Info3Button.AccessibleName = "Button Three";
      this.Info3Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.Info3Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.Info3Button.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Info3Button.Location = new System.Drawing.Point(315, 11);
      this.Info3Button.Name = "Info3Button";
      this.Info3Button.Size = new System.Drawing.Size(75, 23);
      this.Info3Button.TabIndex = 2;
      this.Info3Button.Text = "Button3";
      this.Info3Button.UseVisualStyleBackColor = true;
      this.Info3Button.Click += new System.EventHandler(this.Info3Button_Click);
      // 
      // InfoCheckBox
      // 
      this.InfoCheckBox.AccessibleDescription = "An optional check box displayed at the left side of the command area";
      this.InfoCheckBox.AccessibleName = "Check Box";
      this.InfoCheckBox.AutoSize = true;
      this.InfoCheckBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.InfoCheckBox.Location = new System.Drawing.Point(12, 14);
      this.InfoCheckBox.Name = "InfoCheckBox";
      this.InfoCheckBox.Size = new System.Drawing.Size(264, 19);
      this.InfoCheckBox.TabIndex = 4;
      this.InfoCheckBox.Text = "Do not show this dialog again for this session";
      this.InfoCheckBox.UseVisualStyleBackColor = true;
      // 
      // InfoComboBox
      // 
      this.InfoComboBox.AccessibleDescription = "An optional combo box displayed at the left side of the command area";
      this.InfoComboBox.AccessibleName = "Generic Drop Down";
      this.InfoComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
      this.InfoComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
      this.InfoComboBox.BackColor = System.Drawing.SystemColors.InactiveBorder;
      this.InfoComboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      this.InfoComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.InfoComboBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.InfoComboBox.FormattingEnabled = true;
      this.InfoComboBox.Location = new System.Drawing.Point(12, 12);
      this.InfoComboBox.Name = "InfoComboBox";
      this.InfoComboBox.Size = new System.Drawing.Size(200, 24);
      this.InfoComboBox.TabIndex = 5;
      this.InfoComboBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.InfoComboBox_DrawItem);
      // 
      // InfoDialog
      // 
      this.AcceptButton = this.Info1Button;
      this.AccessibleDescription = "A modal dialog with a common visual style to display generic information";
      this.AccessibleName = "Information Dialog";
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.Info2Button;
      this.ClientSize = new System.Drawing.Size(564, 312);
      this.CommandAreaVisible = true;
      this.FootnoteAreaHeight = 0;
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
      this.MinimumSize = new System.Drawing.Size(580, 350);
      this.Name = "InfoDialog";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Info Dialog";
      this.Load += new System.EventHandler(this.InfoDialog_Load);
      this.Shown += new System.EventHandler(this.InfoDialog_Shown);
      this.ContentAreaPanel.ResumeLayout(false);
      this.ContentAreaPanel.PerformLayout();
      this.CommandAreaPanel.ResumeLayout(false);
      this.CommandAreaPanel.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.LogoPictureBox)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button Info1Button;
    private System.Windows.Forms.Button MoreInfoButton;
    private System.Windows.Forms.Label DetailLabel;
    private System.Windows.Forms.PictureBox LogoPictureBox;
    private System.Windows.Forms.Label TitleLabel;
    private System.Windows.Forms.Label DetailSubLabel;
    private System.Windows.Forms.TextBox MoreInfoTextBox;
    private System.Windows.Forms.Button Info2Button;
    private System.Windows.Forms.Timer DefaultButtonTimer;
    private System.Windows.Forms.Button Info3Button;
    private System.Windows.Forms.CheckBox InfoCheckBox;
    private System.Windows.Forms.ComboBox InfoComboBox;
  }
}