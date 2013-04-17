namespace MySql.Data.VisualStudio.DBExport
{
  partial class DBExportSelectObjects
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
      this.listBox1 = new System.Windows.Forms.ListBox();
      this.label1 = new System.Windows.Forms.Label();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.treeView1 = new System.Windows.Forms.TreeView();
      this.label2 = new System.Windows.Forms.Label();
      this.button1 = new System.Windows.Forms.Button();
      this.button2 = new System.Windows.Forms.Button();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.label4 = new System.Windows.Forms.Label();
      this.checkBox1 = new System.Windows.Forms.CheckBox();
      this.checkBox2 = new System.Windows.Forms.CheckBox();
      this.checkBox3 = new System.Windows.Forms.CheckBox();
      this.checkBox4 = new System.Windows.Forms.CheckBox();
      this.checkBox5 = new System.Windows.Forms.CheckBox();
      this.checkBox6 = new System.Windows.Forms.CheckBox();
      this.checkBox7 = new System.Windows.Forms.CheckBox();
      this.button4 = new System.Windows.Forms.Button();
      this.button5 = new System.Windows.Forms.Button();
      this.button3 = new System.Windows.Forms.Button();
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.groupBox3 = new System.Windows.Forms.GroupBox();
      this.checkBox9 = new System.Windows.Forms.CheckBox();
      this.checkBox8 = new System.Windows.Forms.CheckBox();
      this.checkBox10 = new System.Windows.Forms.CheckBox();
      this.groupBox1.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.groupBox3.SuspendLayout();
      this.SuspendLayout();
      // 
      // listBox1
      // 
      this.listBox1.FormattingEnabled = true;
      this.listBox1.Location = new System.Drawing.Point(79, 33);
      this.listBox1.Name = "listBox1";
      this.listBox1.Size = new System.Drawing.Size(269, 17);
      this.listBox1.TabIndex = 0;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(15, 35);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(53, 13);
      this.label1.TabIndex = 1;
      this.label1.Text = "Database";
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.button2);
      this.groupBox1.Controls.Add(this.button1);
      this.groupBox1.Controls.Add(this.label2);
      this.groupBox1.Controls.Add(this.treeView1);
      this.groupBox1.Controls.Add(this.label1);
      this.groupBox1.Controls.Add(this.listBox1);
      this.groupBox1.Location = new System.Drawing.Point(14, 23);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(380, 468);
      this.groupBox1.TabIndex = 2;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Source";
      // 
      // treeView1
      // 
      this.treeView1.Location = new System.Drawing.Point(20, 93);
      this.treeView1.Name = "treeView1";
      this.treeView1.Size = new System.Drawing.Size(328, 327);
      this.treeView1.TabIndex = 2;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(15, 67);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(124, 13);
      this.label2.TabIndex = 3;
      this.label2.Text = "Select Objects to Export:";
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(186, 432);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(91, 23);
      this.button1.TabIndex = 4;
      this.button1.Text = "Select All";
      this.button1.UseVisualStyleBackColor = true;
      // 
      // button2
      // 
      this.button2.Location = new System.Drawing.Point(283, 432);
      this.button2.Name = "button2";
      this.button2.Size = new System.Drawing.Size(91, 23);
      this.button2.TabIndex = 5;
      this.button2.Text = "Unselect All";
      this.button2.UseVisualStyleBackColor = true;
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.checkBox7);
      this.groupBox2.Controls.Add(this.checkBox6);
      this.groupBox2.Controls.Add(this.checkBox5);
      this.groupBox2.Controls.Add(this.checkBox4);
      this.groupBox2.Controls.Add(this.checkBox3);
      this.groupBox2.Controls.Add(this.checkBox2);
      this.groupBox2.Controls.Add(this.checkBox1);
      this.groupBox2.Controls.Add(this.label4);
      this.groupBox2.Location = new System.Drawing.Point(418, 23);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(380, 227);
      this.groupBox2.TabIndex = 3;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Export Options";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(15, 32);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(42, 13);
      this.label4.TabIndex = 0;
      this.label4.Text = "Include";
      // 
      // checkBox1
      // 
      this.checkBox1.AutoSize = true;
      this.checkBox1.Location = new System.Drawing.Point(28, 53);
      this.checkBox1.Name = "checkBox1";
      this.checkBox1.Size = new System.Drawing.Size(63, 17);
      this.checkBox1.TabIndex = 1;
      this.checkBox1.Text = "Indexes";
      this.checkBox1.UseVisualStyleBackColor = true;
      // 
      // checkBox2
      // 
      this.checkBox2.AutoSize = true;
      this.checkBox2.Location = new System.Drawing.Point(28, 76);
      this.checkBox2.Name = "checkBox2";
      this.checkBox2.Size = new System.Drawing.Size(135, 17);
      this.checkBox2.TabIndex = 2;
      this.checkBox2.Text = "Foreign key constraints";
      this.checkBox2.UseVisualStyleBackColor = true;
      // 
      // checkBox3
      // 
      this.checkBox3.AutoSize = true;
      this.checkBox3.Location = new System.Drawing.Point(28, 99);
      this.checkBox3.Name = "checkBox3";
      this.checkBox3.Size = new System.Drawing.Size(108, 17);
      this.checkBox3.TabIndex = 3;
      this.checkBox3.Text = "Engine table type";
      this.checkBox3.UseVisualStyleBackColor = true;
      // 
      // checkBox4
      // 
      this.checkBox4.AutoSize = true;
      this.checkBox4.Location = new System.Drawing.Point(28, 122);
      this.checkBox4.Name = "checkBox4";
      this.checkBox4.Size = new System.Drawing.Size(91, 17);
      this.checkBox4.TabIndex = 4;
      this.checkBox4.Text = "Character Set";
      this.checkBox4.UseVisualStyleBackColor = true;
      // 
      // checkBox5
      // 
      this.checkBox5.AutoSize = true;
      this.checkBox5.Location = new System.Drawing.Point(28, 145);
      this.checkBox5.Name = "checkBox5";
      this.checkBox5.Size = new System.Drawing.Size(94, 17);
      this.checkBox5.TabIndex = 5;
      this.checkBox5.Text = "Autoincrement";
      this.checkBox5.UseVisualStyleBackColor = true;
      // 
      // checkBox6
      // 
      this.checkBox6.AutoSize = true;
      this.checkBox6.Location = new System.Drawing.Point(28, 168);
      this.checkBox6.Name = "checkBox6";
      this.checkBox6.Size = new System.Drawing.Size(95, 17);
      this.checkBox6.TabIndex = 6;
      this.checkBox6.Text = "User accounts";
      this.checkBox6.UseVisualStyleBackColor = true;
      // 
      // checkBox7
      // 
      this.checkBox7.AutoSize = true;
      this.checkBox7.Location = new System.Drawing.Point(28, 191);
      this.checkBox7.Name = "checkBox7";
      this.checkBox7.Size = new System.Drawing.Size(104, 17);
      this.checkBox7.TabIndex = 7;
      this.checkBox7.Text = "Grant sentences";
      this.checkBox7.UseVisualStyleBackColor = true;
      // 
      // button4
      // 
      this.button4.Location = new System.Drawing.Point(706, 492);
      this.button4.Name = "button4";
      this.button4.Size = new System.Drawing.Size(91, 23);
      this.button4.TabIndex = 10;
      this.button4.Text = "Cancel";
      this.button4.UseVisualStyleBackColor = true;
      // 
      // button5
      // 
      this.button5.Location = new System.Drawing.Point(609, 492);
      this.button5.Name = "button5";
      this.button5.Size = new System.Drawing.Size(91, 23);
      this.button5.TabIndex = 9;
      this.button5.Text = "Export";
      this.button5.UseVisualStyleBackColor = true;
      // 
      // button3
      // 
      this.button3.Location = new System.Drawing.Point(276, 499);
      this.button3.Name = "button3";
      this.button3.Size = new System.Drawing.Size(97, 22);
      this.button3.TabIndex = 13;
      this.button3.Text = "Refresh";
      this.button3.UseVisualStyleBackColor = true;
      // 
      // textBox1
      // 
      this.textBox1.Location = new System.Drawing.Point(53, 499);
      this.textBox1.Name = "textBox1";
      this.textBox1.Size = new System.Drawing.Size(217, 20);
      this.textBox1.TabIndex = 12;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(16, 501);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(32, 13);
      this.label3.TabIndex = 11;
      this.label3.Text = "Filter:";
      // 
      // groupBox3
      // 
      this.groupBox3.Controls.Add(this.checkBox10);
      this.groupBox3.Controls.Add(this.checkBox8);
      this.groupBox3.Controls.Add(this.checkBox9);
      this.groupBox3.Location = new System.Drawing.Point(420, 263);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Size = new System.Drawing.Size(377, 152);
      this.groupBox3.TabIndex = 14;
      this.groupBox3.TabStop = false;
      this.groupBox3.Text = "Data Options";
      // 
      // checkBox9
      // 
      this.checkBox9.AutoSize = true;
      this.checkBox9.Location = new System.Drawing.Point(26, 38);
      this.checkBox9.Name = "checkBox9";
      this.checkBox9.Size = new System.Drawing.Size(128, 17);
      this.checkBox9.TabIndex = 9;
      this.checkBox9.Text = "Insert data sentences";
      this.checkBox9.UseVisualStyleBackColor = true;
      // 
      // checkBox8
      // 
      this.checkBox8.AutoSize = true;
      this.checkBox8.Location = new System.Drawing.Point(26, 61);
      this.checkBox8.Name = "checkBox8";
      this.checkBox8.Size = new System.Drawing.Size(100, 17);
      this.checkBox8.TabIndex = 10;
      this.checkBox8.Text = "Use transaction";
      this.checkBox8.UseVisualStyleBackColor = true;
      // 
      // checkBox10
      // 
      this.checkBox10.AutoSize = true;
      this.checkBox10.Location = new System.Drawing.Point(26, 84);
      this.checkBox10.Name = "checkBox10";
      this.checkBox10.Size = new System.Drawing.Size(222, 17);
      this.checkBox10.TabIndex = 11;
      this.checkBox10.Text = "Add DROP TABLE before each CREATE";
      this.checkBox10.UseVisualStyleBackColor = true;
      // 
      // DBExportSelectObjects
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(815, 534);
      this.Controls.Add(this.groupBox3);
      this.Controls.Add(this.button3);
      this.Controls.Add(this.textBox1);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.button4);
      this.Controls.Add(this.button5);
      this.Controls.Add(this.groupBox2);
      this.Controls.Add(this.groupBox1);
      this.MaximizeBox = false;
      this.Name = "DBExportSelectObjects";
      this.Text = "Export MySQL Database {name}";
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      this.groupBox3.ResumeLayout(false);
      this.groupBox3.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ListBox listBox1;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TreeView treeView1;
    private System.Windows.Forms.Button button2;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.CheckBox checkBox7;
    private System.Windows.Forms.CheckBox checkBox6;
    private System.Windows.Forms.CheckBox checkBox5;
    private System.Windows.Forms.CheckBox checkBox4;
    private System.Windows.Forms.CheckBox checkBox3;
    private System.Windows.Forms.CheckBox checkBox2;
    private System.Windows.Forms.CheckBox checkBox1;
    private System.Windows.Forms.Button button4;
    private System.Windows.Forms.Button button5;
    private System.Windows.Forms.Button button3;
    private System.Windows.Forms.TextBox textBox1;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.GroupBox groupBox3;
    private System.Windows.Forms.CheckBox checkBox10;
    private System.Windows.Forms.CheckBox checkBox8;
    private System.Windows.Forms.CheckBox checkBox9;
  }
}