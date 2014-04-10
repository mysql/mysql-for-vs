namespace MySql.Data.VisualStudio.Wizards.WindowsForms
{
  partial class ValidationConfig
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
      this.grdColumns = new System.Windows.Forms.DataGridView();
      this.lblTitle = new System.Windows.Forms.Label();
      this.chkValidation = new System.Windows.Forms.CheckBox();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.label3 = new System.Windows.Forms.Label();
      this.pictureBox2 = new System.Windows.Forms.PictureBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.grdColumns)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
      this.SuspendLayout();
      // 
      // grdColumns
      // 
      this.grdColumns.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.grdColumns.Location = new System.Drawing.Point(455, 76);
      this.grdColumns.Name = "grdColumns";
      this.grdColumns.Size = new System.Drawing.Size(376, 290);
      this.grdColumns.TabIndex = 0;
      this.grdColumns.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.grdColumns_CellValidating);
      // 
      // lblTitle
      // 
      this.lblTitle.AutoSize = true;
      this.lblTitle.Location = new System.Drawing.Point(272, 76);
      this.lblTitle.Name = "lblTitle";
      this.lblTitle.Size = new System.Drawing.Size(141, 13);
      this.lblTitle.TabIndex = 1;
      this.lblTitle.Text = "Columns to Validate in Table";
      // 
      // chkValidation
      // 
      this.chkValidation.AutoSize = true;
      this.chkValidation.Checked = true;
      this.chkValidation.CheckState = System.Windows.Forms.CheckState.Checked;
      this.chkValidation.Location = new System.Drawing.Point(455, 394);
      this.chkValidation.Name = "chkValidation";
      this.chkValidation.Size = new System.Drawing.Size(110, 17);
      this.chkValidation.TabIndex = 2;
      this.chkValidation.Text = "Include Validation";
      this.chkValidation.UseVisualStyleBackColor = true;
      this.chkValidation.CheckedChanged += new System.EventHandler(this.chkValidation_CheckedChanged);
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
      this.label3.Size = new System.Drawing.Size(248, 23);
      this.label3.TabIndex = 52;
      this.label3.Text = "Validation Rules Configuration";
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
      this.label1.TabIndex = 54;
      this.label1.Text = "Configuration";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Calibri", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.ForeColor = System.Drawing.Color.Black;
      this.label2.Location = new System.Drawing.Point(6, 99);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(234, 33);
      this.label2.TabIndex = 53;
      this.label2.Text = "WinForms Template";
      // 
      // ValidationConfig
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.pictureBox2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.chkValidation);
      this.Controls.Add(this.lblTitle);
      this.Controls.Add(this.grdColumns);
      this.Controls.Add(this.pictureBox1);
      this.Name = "ValidationConfig";
      this.Size = new System.Drawing.Size(861, 443);
      ((System.ComponentModel.ISupportInitialize)(this.grdColumns)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.DataGridView grdColumns;
    private System.Windows.Forms.Label lblTitle;
    private System.Windows.Forms.CheckBox chkValidation;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.PictureBox pictureBox2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
  }
}
