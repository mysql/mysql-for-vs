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
      this.grdColumnsDetail = new System.Windows.Forms.DataGridView();
      this.chkValidations = new System.Windows.Forms.CheckBox();
      ((System.ComponentModel.ISupportInitialize)(this.grdColumnsDetail)).BeginInit();
      this.SuspendLayout();
      // 
      // lblTitleDetail
      // 
      this.lblTitleDetail.AutoSize = true;
      this.lblTitleDetail.Font = new System.Drawing.Font("Segoe UI", 11.25F);
      this.lblTitleDetail.Location = new System.Drawing.Point(18, 18);
      this.lblTitleDetail.Name = "lblTitleDetail";
      this.lblTitleDetail.Size = new System.Drawing.Size(38, 20);
      this.lblTitleDetail.TabIndex = 62;
      this.lblTitleDetail.Text = "Title";
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
      // chkValidations
      // 
      this.chkValidations.AutoSize = true;
      this.chkValidations.Checked = true;
      this.chkValidations.CheckState = System.Windows.Forms.CheckState.Checked;
      this.chkValidations.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.chkValidations.Location = new System.Drawing.Point(465, 342);
      this.chkValidations.Name = "chkValidations";
      this.chkValidations.Size = new System.Drawing.Size(121, 19);
      this.chkValidations.TabIndex = 64;
      this.chkValidations.Text = "Enable validations";
      this.chkValidations.UseVisualStyleBackColor = true;
      this.chkValidations.CheckedChanged += new System.EventHandler(this.chkValidations_CheckedChanged);
      // 
      // DetailValidationConfig
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.chkValidations);
      this.Controls.Add(this.grdColumnsDetail);
      this.Controls.Add(this.lblTitleDetail);
      this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
      this.Name = "DetailValidationConfig";
      this.Size = new System.Drawing.Size(610, 380);
      ((System.ComponentModel.ISupportInitialize)(this.grdColumnsDetail)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label lblTitleDetail;
    private System.Windows.Forms.DataGridView grdColumnsDetail;
    private System.Windows.Forms.CheckBox chkValidations;

  }
}
