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
      this.dataSourceConfiguration1 = new MySql.Data.VisualStudio.Wizards.Web.DataSourceConfiguration();
      this.providerConfiguration1 = new MySql.Data.VisualStudio.Wizards.Web.ProviderConfiguration();
      this.modelConfiguration1 = new MySql.Data.VisualStudio.Wizards.Web.ModelConfiguration();
      this.tablesSelection1 = new MySql.Data.VisualStudio.Wizards.Web.TablesSelection();
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
      // dataSourceConfiguration1
      // 
      this.dataSourceConfiguration1.BackColor = System.Drawing.SystemColors.Control;
      this.dataSourceConfiguration1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.dataSourceConfiguration1.Location = new System.Drawing.Point(260, 83);
      this.dataSourceConfiguration1.Name = "dataSourceConfiguration1";
      this.dataSourceConfiguration1.Size = new System.Drawing.Size(581, 357);
      this.dataSourceConfiguration1.TabIndex = 18;
      // 
      // providerConfiguration1
      // 
      this.providerConfiguration1.BackColor = System.Drawing.SystemColors.Control;
      this.providerConfiguration1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.providerConfiguration1.Location = new System.Drawing.Point(260, 83);
      this.providerConfiguration1.Name = "providerConfiguration1";
      this.providerConfiguration1.Size = new System.Drawing.Size(581, 357);
      this.providerConfiguration1.TabIndex = 19;
      // 
      // modelConfiguration1
      // 
      this.modelConfiguration1.BackColor = System.Drawing.SystemColors.Control;
      this.modelConfiguration1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.modelConfiguration1.Location = new System.Drawing.Point(260, 83);
      this.modelConfiguration1.Name = "modelConfiguration1";
      this.modelConfiguration1.Size = new System.Drawing.Size(581, 357);
      this.modelConfiguration1.TabIndex = 20;
      // 
      // tablesSelection1
      // 
      this.tablesSelection1.Font = new System.Drawing.Font("Segoe UI", 8.25F);
      this.tablesSelection1.Location = new System.Drawing.Point(260, 83);
      this.tablesSelection1.Name = "tablesSelection1";
      this.tablesSelection1.Size = new System.Drawing.Size(581, 357);
      this.tablesSelection1.TabIndex = 21;
      // 
      // WebWizardForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(873, 510);
      this.Controls.Add(this.tablesSelection1);
      this.Controls.Add(this.modelConfiguration1);
      this.Controls.Add(this.providerConfiguration1);
      this.Controls.Add(this.dataSourceConfiguration1);
      this.Name = "WebWizardForm";
      this.Text = "MySQL MVC Template Configuration";
      this.Load += new System.EventHandler(this.WebWizardForm_Load);
      this.Controls.SetChildIndex(this.dataSourceConfiguration1, 0);
      this.Controls.SetChildIndex(this.providerConfiguration1, 0);
      this.Controls.SetChildIndex(this.modelConfiguration1, 0);
      this.Controls.SetChildIndex(this.btnNext, 0);
      this.Controls.SetChildIndex(this.btnBack, 0);
      this.Controls.SetChildIndex(this.btnCancel, 0);
      this.Controls.SetChildIndex(this.tablesSelection1, 0);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private DataSourceConfiguration dataSourceConfiguration1;
    private ProviderConfiguration providerConfiguration1;
    private ModelConfiguration modelConfiguration1;
    private TablesSelection tablesSelection1;


  }
}