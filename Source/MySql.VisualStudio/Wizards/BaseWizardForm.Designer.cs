namespace MySql.Data.VisualStudio.Wizards
{
  partial class BaseWizardForm
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
      this.btnFinish = new System.Windows.Forms.Button();
      this.btnCancel = new System.Windows.Forms.Button();
      this.btnBack = new System.Windows.Forms.Button();
      this.btnNext = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // btnFinish
      // 
      this.btnFinish.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.btnFinish.Location = new System.Drawing.Point(354, 411);
      this.btnFinish.Name = "btnFinish";
      this.btnFinish.Size = new System.Drawing.Size(75, 23);
      this.btnFinish.TabIndex = 11;
      this.btnFinish.Text = "Finish";
      this.btnFinish.UseVisualStyleBackColor = true;
      this.btnFinish.Click += new System.EventHandler(this.btnFinish_Click);
      // 
      // btnCancel
      // 
      this.btnCancel.CausesValidation = false;
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.Location = new System.Drawing.Point(112, 411);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(75, 23);
      this.btnCancel.TabIndex = 10;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      // 
      // btnBack
      // 
      this.btnBack.Location = new System.Drawing.Point(193, 411);
      this.btnBack.Name = "btnBack";
      this.btnBack.Size = new System.Drawing.Size(75, 23);
      this.btnBack.TabIndex = 9;
      this.btnBack.Text = "Back";
      this.btnBack.UseVisualStyleBackColor = true;
      this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
      // 
      // btnNext
      // 
      this.btnNext.Location = new System.Drawing.Point(274, 411);
      this.btnNext.Name = "btnNext";
      this.btnNext.Size = new System.Drawing.Size(75, 23);
      this.btnNext.TabIndex = 8;
      this.btnNext.Text = "Next";
      this.btnNext.UseVisualStyleBackColor = true;
      this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
      // 
      // BaseWizardForm
      // 
      this.AcceptButton = this.btnFinish;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnCancel;
      this.ClientSize = new System.Drawing.Size(441, 446);
      this.Controls.Add(this.btnFinish);
      this.Controls.Add(this.btnCancel);
      this.Controls.Add(this.btnBack);
      this.Controls.Add(this.btnNext);
      this.Name = "BaseWizardForm";
      this.Text = "BaseWizardForm";
      this.ResumeLayout(false);

    }

    #endregion

    protected System.Windows.Forms.Button btnFinish;
    protected System.Windows.Forms.Button btnNext;
    protected System.Windows.Forms.Button btnCancel;
    protected System.Windows.Forms.Button btnBack;
  }
}