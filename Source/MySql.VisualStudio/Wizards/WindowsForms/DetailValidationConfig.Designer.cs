namespace MySql.Data.VisualStudio.Wizards.WindowsForms
{
  partial class DetailValidationConfig
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
      this.lblTitleDetail = new System.Windows.Forms.Label();
      this.chkValidation = new System.Windows.Forms.CheckBox();
      this.grdColumnsDetail = new System.Windows.Forms.DataGridView();
      this.chkNoValidations = new System.Windows.Forms.CheckBox();
      ((System.ComponentModel.ISupportInitialize)(this.grdColumnsDetail)).BeginInit();
      this.SuspendLayout();
      // 
      // lblTitleDetail
      // 
      this.lblTitleDetail.AutoSize = true;
      this.lblTitleDetail.Location = new System.Drawing.Point(13, 20);
      this.lblTitleDetail.Name = "lblTitleDetail";
      this.lblTitleDetail.Size = new System.Drawing.Size(205, 13);
      this.lblTitleDetail.TabIndex = 62;
      this.lblTitleDetail.Text = "Columns to Validate in Detail Table {0}:";
      // 
      // chkValidation
      // 
      this.chkValidation.AutoSize = true;
      this.chkValidation.Checked = true;
      this.chkValidation.CheckState = System.Windows.Forms.CheckState.Checked;
      this.chkValidation.Location = new System.Drawing.Point(209, 381);
      this.chkValidation.Name = "chkValidation";
      this.chkValidation.Size = new System.Drawing.Size(120, 17);
      this.chkValidation.TabIndex = 59;
      this.chkValidation.Text = "Include Validation";
      this.chkValidation.UseVisualStyleBackColor = true;
      // 
      // grdColumnsDetail
      // 
      this.grdColumnsDetail.AllowUserToResizeColumns = false;
      this.grdColumnsDetail.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
      this.grdColumnsDetail.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
      this.grdColumnsDetail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.grdColumnsDetail.Location = new System.Drawing.Point(16, 46);
      this.grdColumnsDetail.MinimumSize = new System.Drawing.Size(570, 290);
      this.grdColumnsDetail.Name = "grdColumnsDetail";
      this.grdColumnsDetail.Size = new System.Drawing.Size(570, 290);
      this.grdColumnsDetail.TabIndex = 63;
      // 
      // chkNoValidations
      // 
      this.chkNoValidations.AutoSize = true;
      this.chkNoValidations.Location = new System.Drawing.Point(263, 348);
      this.chkNoValidations.Name = "chkNoValidations";
      this.chkNoValidations.Size = new System.Drawing.Size(323, 17);
      this.chkNoValidations.TabIndex = 64;
      this.chkNoValidations.Text = "I will handle columns validations on my Application Code.";
      this.chkNoValidations.UseVisualStyleBackColor = true;
      this.chkNoValidations.CheckedChanged += new System.EventHandler(this.chkNoValidations_CheckedChanged);
      // 
      // DetailValidationConfig
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.chkNoValidations);
      this.Controls.Add(this.grdColumnsDetail);
      this.Controls.Add(this.lblTitleDetail);
      this.Controls.Add(this.chkValidation);
      this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
      this.Name = "DetailValidationConfig";
      this.Size = new System.Drawing.Size(610, 380);
      ((System.ComponentModel.ISupportInitialize)(this.grdColumnsDetail)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label lblTitleDetail;
    private System.Windows.Forms.CheckBox chkValidation;
    private System.Windows.Forms.DataGridView grdColumnsDetail;
    private System.Windows.Forms.CheckBox chkNoValidations;

  }
}
