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
      this.dataAccessConfig1 = new DataAccessConfig();
      this.tablesSelection1 = new TablesSelectionWinForms();
      this.SuspendLayout();
      // 
      // btnNext
      // 
      this.btnNext.Location = new System.Drawing.Point(676, 466);
      // 
      // btnCancel
      // 
      this.btnCancel.Location = new System.Drawing.Point(514, 466);
      // 
      // btnBack
      // 
      this.btnBack.Location = new System.Drawing.Point(595, 466);
      // 
      // dataAccessConfig1
      // 
      this.dataAccessConfig1.Font = new System.Drawing.Font("Segoe UI", 8.25F);
      this.dataAccessConfig1.Location = new System.Drawing.Point(260, 83);
      this.dataAccessConfig1.Name = "dataAccessConfig1";
      this.dataAccessConfig1.Size = new System.Drawing.Size(610, 380);
      this.dataAccessConfig1.TabIndex = 22;
      // 
      // tablesSelection1
      // 
      this.tablesSelection1.Font = new System.Drawing.Font("Segoe UI", 8.25F);
      this.tablesSelection1.Location = new System.Drawing.Point(259, 84);
      this.tablesSelection1.Name = "tablesSelection1";
      this.tablesSelection1.Size = new System.Drawing.Size(584, 380);
      this.tablesSelection1.TabIndex = 23;
      // 
      // WindowsFormsWizardForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(873, 510);
      this.Controls.Add(this.tablesSelection1);
      this.Controls.Add(this.dataAccessConfig1);
      this.Name = "WindowsFormsWizardForm";
      this.Text = "Windows Forms Configuration ";
      this.Load += new System.EventHandler(this.WizardForm_Load);
      this.Controls.SetChildIndex(this.btnFinish, 0);
      this.Controls.SetChildIndex(this.btnNext, 0);
      this.Controls.SetChildIndex(this.btnBack, 0);
      this.Controls.SetChildIndex(this.btnCancel, 0);
      this.Controls.SetChildIndex(this.dataAccessConfig1, 0);
      this.Controls.SetChildIndex(this.tablesSelection1, 0);
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    #endregion

    private DataAccessConfig dataAccessConfig1;
    private TablesSelectionWinForms tablesSelection1;
  }
}