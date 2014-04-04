namespace MySql.Data.VisualStudio.Wizards.Web
{
  partial class ModelConfiguration
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
      this.pictureBox2 = new System.Windows.Forms.PictureBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.label3 = new System.Windows.Forms.Label();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.rdbNoModel = new System.Windows.Forms.RadioButton();
      this.Ef6 = new System.Windows.Forms.RadioButton();
      this.Ef5 = new System.Windows.Forms.RadioButton();
      this.label6 = new System.Windows.Forms.Label();
      this.includeSensitiveInformationCheck = new System.Windows.Forms.CheckBox();
      this.label7 = new System.Windows.Forms.Label();
      this.ModelNameTextBox = new System.Windows.Forms.TextBox();
      this.listTables = new System.Windows.Forms.DataGridView();
      this.label5 = new System.Windows.Forms.Label();
      this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.groupBox1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.listTables)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
      this.SuspendLayout();
      // 
      // pictureBox2
      // 
      this.pictureBox2.Image = global::MySql.Data.VisualStudio.Properties.Resources.MySQL;
      this.pictureBox2.Location = new System.Drawing.Point(68, 182);
      this.pictureBox2.Name = "pictureBox2";
      this.pictureBox2.Size = new System.Drawing.Size(108, 111);
      this.pictureBox2.TabIndex = 50;
      this.pictureBox2.TabStop = false;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.ForeColor = System.Drawing.Color.Black;
      this.label1.Location = new System.Drawing.Point(58, 139);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(128, 26);
      this.label1.TabIndex = 49;
      this.label1.Text = "Configuration";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Calibri", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.ForeColor = System.Drawing.Color.Black;
      this.label2.Location = new System.Drawing.Point(35, 99);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(174, 33);
      this.label2.TabIndex = 48;
      this.label2.Text = "MVC Template";
      // 
      // pictureBox1
      // 
      this.pictureBox1.BackColor = System.Drawing.SystemColors.Control;
      this.pictureBox1.Image = global::MySql.Data.VisualStudio.Properties.Resources.Background;
      this.pictureBox1.Location = new System.Drawing.Point(245, 1);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(614, 442);
      this.pictureBox1.TabIndex = 47;
      this.pictureBox1.TabStop = false;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(242)))));
      this.label3.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.Location = new System.Drawing.Point(271, 17);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(264, 23);
      this.label3.TabIndex = 51;
      this.label3.Text = "Entity Data Model Configuration";
      // 
      // groupBox1
      // 
      this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(242)))));
      this.groupBox1.Controls.Add(this.rdbNoModel);
      this.groupBox1.Controls.Add(this.Ef6);
      this.groupBox1.Controls.Add(this.Ef5);
      this.groupBox1.Location = new System.Drawing.Point(459, 53);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(310, 42);
      this.groupBox1.TabIndex = 55;
      this.groupBox1.TabStop = false;
      // 
      // rdbNoModel
      // 
      this.rdbNoModel.AutoSize = true;
      this.rdbNoModel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(242)))));
      this.rdbNoModel.Location = new System.Drawing.Point(132, 15);
      this.rdbNoModel.Name = "rdbNoModel";
      this.rdbNoModel.Size = new System.Drawing.Size(116, 17);
      this.rdbNoModel.TabIndex = 21;
      this.rdbNoModel.TabStop = true;
      this.rdbNoModel.Text = "No include a model";
      this.rdbNoModel.UseVisualStyleBackColor = false;
      // 
      // Ef6
      // 
      this.Ef6.AutoSize = true;
      this.Ef6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(242)))));
      this.Ef6.Location = new System.Drawing.Point(75, 15);
      this.Ef6.Name = "Ef6";
      this.Ef6.Size = new System.Drawing.Size(40, 17);
      this.Ef6.TabIndex = 20;
      this.Ef6.TabStop = true;
      this.Ef6.Text = "6.0";
      this.Ef6.UseVisualStyleBackColor = false;
      // 
      // Ef5
      // 
      this.Ef5.AutoSize = true;
      this.Ef5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(242)))));
      this.Ef5.Checked = true;
      this.Ef5.Location = new System.Drawing.Point(18, 15);
      this.Ef5.Name = "Ef5";
      this.Ef5.Size = new System.Drawing.Size(40, 17);
      this.Ef5.TabIndex = 19;
      this.Ef5.TabStop = true;
      this.Ef5.Text = "5.0";
      this.Ef5.UseVisualStyleBackColor = false;
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(242)))));
      this.label6.Location = new System.Drawing.Point(292, 65);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(88, 13);
      this.label6.TabIndex = 54;
      this.label6.Text = "Entity Framework";
      // 
      // includeSensitiveInformationCheck
      // 
      this.includeSensitiveInformationCheck.AutoSize = true;
      this.includeSensitiveInformationCheck.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(242)))));
      this.includeSensitiveInformationCheck.Checked = true;
      this.includeSensitiveInformationCheck.CheckState = System.Windows.Forms.CheckState.Checked;
      this.includeSensitiveInformationCheck.Location = new System.Drawing.Point(460, 404);
      this.includeSensitiveInformationCheck.Name = "includeSensitiveInformationCheck";
      this.includeSensitiveInformationCheck.Size = new System.Drawing.Size(258, 17);
      this.includeSensitiveInformationCheck.TabIndex = 58;
      this.includeSensitiveInformationCheck.Text = "Include sensitive information on connection string";
      this.includeSensitiveInformationCheck.UseVisualStyleBackColor = false;
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(242)))));
      this.label7.Location = new System.Drawing.Point(292, 119);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(70, 13);
      this.label7.TabIndex = 57;
      this.label7.Text = "Model Name:";
      // 
      // ModelNameTextBox
      // 
      this.ModelNameTextBox.Location = new System.Drawing.Point(459, 116);
      this.ModelNameTextBox.Name = "ModelNameTextBox";
      this.ModelNameTextBox.Size = new System.Drawing.Size(310, 20);
      this.ModelNameTextBox.TabIndex = 56;
      // 
      // listTables
      // 
      this.listTables.AllowUserToAddRows = false;
      this.listTables.BackgroundColor = System.Drawing.Color.White;
      this.listTables.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.listTables.Location = new System.Drawing.Point(459, 189);
      this.listTables.Name = "listTables";
      this.listTables.RowHeadersVisible = false;
      this.listTables.Size = new System.Drawing.Size(310, 205);
      this.listTables.TabIndex = 53;
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(242)))));
      this.label5.Location = new System.Drawing.Point(292, 161);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(162, 13);
      this.label5.TabIndex = 52;
      this.label5.Text = "Select tables to include in model:";
      // 
      // errorProvider1
      // 
      this.errorProvider1.ContainerControl = this;
      // 
      // ModelConfiguration
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.Control;
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.label6);
      this.Controls.Add(this.includeSensitiveInformationCheck);
      this.Controls.Add(this.label7);
      this.Controls.Add(this.ModelNameTextBox);
      this.Controls.Add(this.listTables);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.pictureBox2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.pictureBox1);
      this.Name = "ModelConfiguration";
      this.Size = new System.Drawing.Size(861, 443);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.listTables)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.PictureBox pictureBox2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.RadioButton Ef6;
    private System.Windows.Forms.RadioButton Ef5;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.CheckBox includeSensitiveInformationCheck;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.TextBox ModelNameTextBox;
    private System.Windows.Forms.DataGridView listTables;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.RadioButton rdbNoModel;
    private System.Windows.Forms.ErrorProvider errorProvider1;
  }
}
