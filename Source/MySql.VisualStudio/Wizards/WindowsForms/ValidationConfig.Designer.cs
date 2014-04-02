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
      ((System.ComponentModel.ISupportInitialize)(this.grdColumns)).BeginInit();
      this.SuspendLayout();
      // 
      // grdColumns
      // 
      this.grdColumns.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.grdColumns.Location = new System.Drawing.Point(20, 76);
      this.grdColumns.Name = "grdColumns";
      this.grdColumns.Size = new System.Drawing.Size(376, 242);
      this.grdColumns.TabIndex = 0;
      this.grdColumns.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.grdColumns_CellValidating);
      // 
      // lblTitle
      // 
      this.lblTitle.AutoSize = true;
      this.lblTitle.Location = new System.Drawing.Point(17, 26);
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
      this.chkValidation.Location = new System.Drawing.Point(20, 53);
      this.chkValidation.Name = "chkValidation";
      this.chkValidation.Size = new System.Drawing.Size(110, 17);
      this.chkValidation.TabIndex = 2;
      this.chkValidation.Text = "Include Validation";
      this.chkValidation.UseVisualStyleBackColor = true;
      this.chkValidation.CheckedChanged += new System.EventHandler(this.chkValidation_CheckedChanged);
      // 
      // ValidationConfig
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.chkValidation);
      this.Controls.Add(this.lblTitle);
      this.Controls.Add(this.grdColumns);
      this.Name = "ValidationConfig";
      this.Size = new System.Drawing.Size(414, 393);
      ((System.ComponentModel.ISupportInitialize)(this.grdColumns)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.DataGridView grdColumns;
    private System.Windows.Forms.Label lblTitle;
    private System.Windows.Forms.CheckBox chkValidation;
  }
}
