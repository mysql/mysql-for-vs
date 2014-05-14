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
      this.chkValidation = new System.Windows.Forms.CheckBox();
      this.lblTitle = new System.Windows.Forms.Label();
      this.grdColumns = new System.Windows.Forms.DataGridView();
      this.chkNoValidations = new System.Windows.Forms.CheckBox();
      ((System.ComponentModel.ISupportInitialize)(this.grdColumns)).BeginInit();
      this.SuspendLayout();
      // 
      // chkValidation
      // 
      this.chkValidation.AutoSize = true;
      this.chkValidation.Checked = true;
      this.chkValidation.CheckState = System.Windows.Forms.CheckState.Checked;
      this.chkValidation.Font = new System.Drawing.Font("Segoe UI", 8.25F);
      this.chkValidation.Location = new System.Drawing.Point(209, 381);
      this.chkValidation.Name = "chkValidation";
      this.chkValidation.Size = new System.Drawing.Size(120, 17);
      this.chkValidation.TabIndex = 59;
      this.chkValidation.Text = "Include Validation";
      this.chkValidation.UseVisualStyleBackColor = true;
      // 
      // lblTitle
      // 
      this.lblTitle.AutoSize = true;
      this.lblTitle.Location = new System.Drawing.Point(13, 20);
      this.lblTitle.Name = "lblTitle";
      this.lblTitle.Size = new System.Drawing.Size(172, 13);
      this.lblTitle.TabIndex = 58;
      this.lblTitle.Text = "Columns to Validate in Table {0}:";
      // 
      // grdColumns
      // 
      this.grdColumns.AllowUserToResizeColumns = false;
      this.grdColumns.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
      this.grdColumns.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
      this.grdColumns.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.grdColumns.Location = new System.Drawing.Point(16, 46);
      this.grdColumns.MinimumSize = new System.Drawing.Size(570, 290);
      this.grdColumns.Name = "grdColumns";
      this.grdColumns.Size = new System.Drawing.Size(570, 290);
      this.grdColumns.TabIndex = 57;
      // 
      // chkNoValidations
      // 
      this.chkNoValidations.AutoSize = true;
      this.chkNoValidations.Location = new System.Drawing.Point(263, 342);
      this.chkNoValidations.Name = "chkNoValidations";
      this.chkNoValidations.Size = new System.Drawing.Size(323, 17);
      this.chkNoValidations.TabIndex = 60;
      this.chkNoValidations.Text = "I will handle columns validations on my Application Code.";
      this.chkNoValidations.UseVisualStyleBackColor = true;
      this.chkNoValidations.CheckedChanged += new System.EventHandler(this.chkNoValidations_CheckedChanged);
      // 
      // ValidationConfig
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.chkNoValidations);
      this.Controls.Add(this.chkValidation);
      this.Controls.Add(this.lblTitle);
      this.Controls.Add(this.grdColumns);
      this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
      this.Name = "ValidationConfig";
      this.Size = new System.Drawing.Size(610, 380);
      ((System.ComponentModel.ISupportInitialize)(this.grdColumns)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.CheckBox chkValidation;
    private System.Windows.Forms.Label lblTitle;
    private System.Windows.Forms.DataGridView grdColumns;
    private System.Windows.Forms.CheckBox chkNoValidations;

  }
}
