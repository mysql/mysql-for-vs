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
      this.lblTitle = new System.Windows.Forms.Label();
      this.grdColumns = new System.Windows.Forms.DataGridView();
      ((System.ComponentModel.ISupportInitialize)(this.grdColumns)).BeginInit();
      this.SuspendLayout();
      // 
      // lblTitle
      // 
      this.lblTitle.AutoSize = true;
      this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblTitle.Location = new System.Drawing.Point(16, 11);
      this.lblTitle.Name = "lblTitle";
      this.lblTitle.Size = new System.Drawing.Size(38, 20);
      this.lblTitle.TabIndex = 58;
      this.lblTitle.Text = "Title";
      // 
      // grdColumns
      // 
      this.grdColumns.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.grdColumns.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
      this.grdColumns.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
      this.grdColumns.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.grdColumns.Location = new System.Drawing.Point(16, 46);
      this.grdColumns.MinimumSize = new System.Drawing.Size(570, 290);
      this.grdColumns.Name = "grdColumns";
      this.grdColumns.Size = new System.Drawing.Size(577, 290);
      this.grdColumns.TabIndex = 57;
      // 
      // ValidationConfig
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
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

    private System.Windows.Forms.Label lblTitle;
    private System.Windows.Forms.DataGridView grdColumns;

  }
}
