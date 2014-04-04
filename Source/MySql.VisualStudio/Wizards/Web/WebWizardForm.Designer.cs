namespace MySql.Data.VisualStudio.Wizards.Web
{
  partial class WebWizardForm
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
      this.modelConfiguration1 = new MySql.Data.VisualStudio.Wizards.Web.ModelConfiguration();
      this.providerConfiguration1 = new MySql.Data.VisualStudio.Wizards.Web.ProviderConfiguration();
      this.SuspendLayout();
      // 
      // btnFinish
      // 
      this.btnFinish.Location = new System.Drawing.Point(764, 453);
      // 
      // btnNext
      // 
      this.btnNext.Location = new System.Drawing.Point(683, 453);
      // 
      // btnCancel
      // 
      this.btnCancel.Location = new System.Drawing.Point(521, 453);
      // 
      // btnBack
      // 
      this.btnBack.Location = new System.Drawing.Point(602, 453);
      // 
      // modelConfiguration1
      // 
      this.modelConfiguration1.BackColor = System.Drawing.SystemColors.Control;
      this.modelConfiguration1.Location = new System.Drawing.Point(1, -2);
      this.modelConfiguration1.Name = "modelConfiguration1";
      this.modelConfiguration1.Size = new System.Drawing.Size(861, 445);
      this.modelConfiguration1.TabIndex = 12;
      // 
      // providerConfiguration1
      // 
      this.providerConfiguration1.BackColor = System.Drawing.SystemColors.Control;
      this.providerConfiguration1.Location = new System.Drawing.Point(3, 2);
      this.providerConfiguration1.Name = "providerConfiguration1";
      this.providerConfiguration1.Size = new System.Drawing.Size(861, 445);
      this.providerConfiguration1.TabIndex = 13;
      // 
      // WebWizardForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(870, 506);
      this.Controls.Add(this.providerConfiguration1);
      this.Controls.Add(this.modelConfiguration1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.MaximizeBox = false;
      this.Name = "WebWizardForm";
      this.Text = "BaseWebWizardForm";
      this.Load += new System.EventHandler(this.WebWizardForm_Load);
      this.Controls.SetChildIndex(this.btnNext, 0);
      this.Controls.SetChildIndex(this.btnBack, 0);
      this.Controls.SetChildIndex(this.btnCancel, 0);
      this.Controls.SetChildIndex(this.btnFinish, 0);
      this.Controls.SetChildIndex(this.modelConfiguration1, 0);
      this.Controls.SetChildIndex(this.providerConfiguration1, 0);
      this.ResumeLayout(false);

    }

    #endregion

    private ModelConfiguration modelConfiguration1;
    private ProviderConfiguration providerConfiguration1;


  }
}