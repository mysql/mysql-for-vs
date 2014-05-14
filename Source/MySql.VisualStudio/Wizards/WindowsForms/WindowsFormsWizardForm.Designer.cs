namespace MySql.Data.VisualStudio.Wizards.WindowsForms
{
  partial class WindowsFormsWizardForm
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
      this.dataAccessConfig1 = new MySql.Data.VisualStudio.Wizards.WindowsForms.DataAccessConfig();
      this.validationConfig1 = new MySql.Data.VisualStudio.Wizards.WindowsForms.ValidationConfig();
      this.dataAccessTechnologyConfig1 = new MySql.Data.VisualStudio.Wizards.WindowsForms.DataAccessTechnologyConfig();
      this.detailValidationConfig1 = new MySql.Data.VisualStudio.Wizards.WindowsForms.DetailValidationConfig();
      this.SuspendLayout();
      // 
      // btnNext
      // 
      this.btnNext.Location = new System.Drawing.Point(702, 469);
      // 
      // btnCancel
      // 
      this.btnCancel.Location = new System.Drawing.Point(540, 469);
      // 
      // btnBack
      // 
      this.btnBack.Location = new System.Drawing.Point(621, 469);
      // 
      // dataAccessConfig1
      // 
      this.dataAccessConfig1.Font = new System.Drawing.Font("Segoe UI", 8.25F);
      this.dataAccessConfig1.Location = new System.Drawing.Point(260, 83);
      this.dataAccessConfig1.Name = "dataAccessConfig1";
      this.dataAccessConfig1.Size = new System.Drawing.Size(610, 380);
      this.dataAccessConfig1.TabIndex = 22;
      // 
      // validationConfig1
      // 
      this.validationConfig1.Font = new System.Drawing.Font("Segoe UI", 8.25F);
      this.validationConfig1.Location = new System.Drawing.Point(260, 83);
      this.validationConfig1.Name = "validationConfig1";
      this.validationConfig1.Size = new System.Drawing.Size(610, 380);
      this.validationConfig1.TabIndex = 23;
      // 
      // dataAccessTechnologyConfig1
      // 
      this.dataAccessTechnologyConfig1.Font = new System.Drawing.Font("Segoe UI", 8.25F);
      this.dataAccessTechnologyConfig1.Location = new System.Drawing.Point(260, 83);
      this.dataAccessTechnologyConfig1.Name = "dataAccessTechnologyConfig1";
      this.dataAccessTechnologyConfig1.Size = new System.Drawing.Size(610, 380);
      this.dataAccessTechnologyConfig1.TabIndex = 24;
      // 
      // detailValidationConfig1
      // 
      this.detailValidationConfig1.Font = new System.Drawing.Font("Segoe UI", 8.25F);
      this.detailValidationConfig1.Location = new System.Drawing.Point(260, 83);
      this.detailValidationConfig1.Name = "detailValidationConfig1";
      this.detailValidationConfig1.Size = new System.Drawing.Size(610, 380);
      this.detailValidationConfig1.TabIndex = 25;
      // 
      // WindowsFormsWizardForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(873, 510);
      this.Controls.Add(this.detailValidationConfig1);
      this.Controls.Add(this.dataAccessTechnologyConfig1);
      this.Controls.Add(this.validationConfig1);
      this.Controls.Add(this.dataAccessConfig1);
      this.Name = "WindowsFormsWizardForm";
      this.Text = "Windows Forms Configuration ";
      this.Load += new System.EventHandler(this.WizardForm_Load);
      this.Controls.SetChildIndex(this.btnNext, 0);
      this.Controls.SetChildIndex(this.btnBack, 0);
      this.Controls.SetChildIndex(this.btnCancel, 0);
      this.Controls.SetChildIndex(this.dataAccessConfig1, 0);
      this.Controls.SetChildIndex(this.validationConfig1, 0);
      this.Controls.SetChildIndex(this.dataAccessTechnologyConfig1, 0);
      this.Controls.SetChildIndex(this.detailValidationConfig1, 0);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private DataAccessConfig dataAccessConfig1;
    private ValidationConfig validationConfig1;
    private DataAccessTechnologyConfig dataAccessTechnologyConfig1;
    private DetailValidationConfig detailValidationConfig1;

  }
}