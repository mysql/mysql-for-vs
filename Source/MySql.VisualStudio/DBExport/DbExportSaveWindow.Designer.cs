namespace MySql.Data.VisualStudio.DBExport
{
  partial class DbExportSaveWindow
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
      this.button1 = new System.Windows.Forms.Button();
      this.button2 = new System.Windows.Forms.Button();
      this.button3 = new System.Windows.Forms.Button();
      this.ExportScript = new System.Windows.Forms.TabControl();
      this.tabPage1 = new System.Windows.Forms.TabPage();
      this.tabPage2 = new System.Windows.Forms.TabPage();
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.button4 = new System.Windows.Forms.Button();
      this.textBox2 = new System.Windows.Forms.TextBox();
      this.ExportScript.SuspendLayout();
      this.tabPage1.SuspendLayout();
      this.tabPage2.SuspendLayout();
      this.SuspendLayout();
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(26, 458);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(109, 26);
      this.button1.TabIndex = 2;
      this.button1.Text = "Save As";
      this.button1.UseVisualStyleBackColor = true;
      // 
      // button2
      // 
      this.button2.Location = new System.Drawing.Point(381, 457);
      this.button2.Name = "button2";
      this.button2.Size = new System.Drawing.Size(109, 27);
      this.button2.TabIndex = 3;
      this.button2.Text = "Back";
      this.button2.UseVisualStyleBackColor = true;
      // 
      // button3
      // 
      this.button3.Location = new System.Drawing.Point(496, 455);
      this.button3.Name = "button3";
      this.button3.Size = new System.Drawing.Size(108, 28);
      this.button3.TabIndex = 4;
      this.button3.Text = "Close";
      this.button3.UseVisualStyleBackColor = true;
      // 
      // ExportScript
      // 
      this.ExportScript.Controls.Add(this.tabPage1);
      this.ExportScript.Controls.Add(this.tabPage2);
      this.ExportScript.Location = new System.Drawing.Point(14, 12);
      this.ExportScript.Name = "ExportScript";
      this.ExportScript.SelectedIndex = 0;
      this.ExportScript.Size = new System.Drawing.Size(590, 437);
      this.ExportScript.TabIndex = 6;
      // 
      // tabPage1
      // 
      this.tabPage1.Controls.Add(this.button4);
      this.tabPage1.Controls.Add(this.textBox1);
      this.tabPage1.Location = new System.Drawing.Point(4, 22);
      this.tabPage1.Name = "tabPage1";
      this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage1.Size = new System.Drawing.Size(582, 411);
      this.tabPage1.TabIndex = 0;
      this.tabPage1.Text = "Export Script";
      this.tabPage1.UseVisualStyleBackColor = true;
      // 
      // tabPage2
      // 
      this.tabPage2.Controls.Add(this.textBox2);
      this.tabPage2.Location = new System.Drawing.Point(4, 22);
      this.tabPage2.Name = "tabPage2";
      this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage2.Size = new System.Drawing.Size(582, 411);
      this.tabPage2.TabIndex = 1;
      this.tabPage2.Text = "Message Log";
      this.tabPage2.UseVisualStyleBackColor = true;
      // 
      // textBox1
      // 
      this.textBox1.Location = new System.Drawing.Point(17, 14);
      this.textBox1.Multiline = true;
      this.textBox1.Name = "textBox1";
      this.textBox1.Size = new System.Drawing.Size(549, 343);
      this.textBox1.TabIndex = 2;
      // 
      // button4
      // 
      this.button4.Location = new System.Drawing.Point(385, 370);
      this.button4.Name = "button4";
      this.button4.Size = new System.Drawing.Size(181, 27);
      this.button4.TabIndex = 3;
      this.button4.Text = "Send to MySQL Script Window";
      this.button4.UseVisualStyleBackColor = true;
      // 
      // textBox2
      // 
      this.textBox2.Location = new System.Drawing.Point(14, 14);
      this.textBox2.Multiline = true;
      this.textBox2.Name = "textBox2";
      this.textBox2.Size = new System.Drawing.Size(553, 386);
      this.textBox2.TabIndex = 0;
      // 
      // DbExportSaveWindow
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(615, 494);
      this.Controls.Add(this.ExportScript);
      this.Controls.Add(this.button3);
      this.Controls.Add(this.button2);
      this.Controls.Add(this.button1);
      this.MaximizeBox = false;
      this.Name = "DbExportSaveWindow";
      this.Text = "Export MySQL Database ";
      this.ExportScript.ResumeLayout(false);
      this.tabPage1.ResumeLayout(false);
      this.tabPage1.PerformLayout();
      this.tabPage2.ResumeLayout(false);
      this.tabPage2.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Button button2;
    private System.Windows.Forms.Button button3;
    private System.Windows.Forms.TabControl ExportScript;
    private System.Windows.Forms.TabPage tabPage1;
    private System.Windows.Forms.TextBox textBox1;
    private System.Windows.Forms.TabPage tabPage2;
    private System.Windows.Forms.Button button4;
    private System.Windows.Forms.TextBox textBox2;
  }
}