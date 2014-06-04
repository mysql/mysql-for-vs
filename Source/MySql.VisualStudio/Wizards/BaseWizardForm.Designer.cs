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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BaseWizardForm));
      this.btnFinish = new System.Windows.Forms.Button();
      this.btnCancel = new System.Windows.Forms.Button();
      this.btnBack = new System.Windows.Forms.Button();
      this.btnNext = new System.Windows.Forms.Button();
      this.lblDescription = new System.Windows.Forms.Label();
      this.lblWizardName = new System.Windows.Forms.Label();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.lblStep = new System.Windows.Forms.Label();
      this.lblStepTitle = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.panel1 = new System.Windows.Forms.Panel();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // btnFinish
      // 
      this.btnFinish.Location = new System.Drawing.Point(757, 466);
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
      this.btnCancel.Location = new System.Drawing.Point(515, 466);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(75, 23);
      this.btnCancel.TabIndex = 10;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      // 
      // btnBack
      // 
      this.btnBack.Location = new System.Drawing.Point(596, 466);
      this.btnBack.Name = "btnBack";
      this.btnBack.Size = new System.Drawing.Size(75, 23);
      this.btnBack.TabIndex = 9;
      this.btnBack.Text = "Back";
      this.btnBack.UseVisualStyleBackColor = true;
      this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
      // 
      // btnNext
      // 
      this.btnNext.Location = new System.Drawing.Point(677, 466);
      this.btnNext.Name = "btnNext";
      this.btnNext.Size = new System.Drawing.Size(75, 23);
      this.btnNext.TabIndex = 8;
      this.btnNext.Text = "Next";
      this.btnNext.UseVisualStyleBackColor = true;
      this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
      // 
      // lblDescription
      // 
      this.lblDescription.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblDescription.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
      this.lblDescription.Location = new System.Drawing.Point(20, 154);
      this.lblDescription.Name = "lblDescription";
      this.lblDescription.Size = new System.Drawing.Size(213, 228);
      this.lblDescription.TabIndex = 2;
      this.lblDescription.Text = resources.GetString("lblDescription.Text");
      // 
      // lblWizardName
      // 
      this.lblWizardName.AutoSize = true;
      this.lblWizardName.Font = new System.Drawing.Font("Segoe UI Semilight", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblWizardName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
      this.lblWizardName.Location = new System.Drawing.Point(20, 119);
      this.lblWizardName.Name = "lblWizardName";
      this.lblWizardName.Size = new System.Drawing.Size(124, 25);
      this.lblWizardName.TabIndex = 1;
      this.lblWizardName.Text = "Wizard Name";
      // 
      // pictureBox1
      // 
	  this.pictureBox1.Image = global::MySql.Data.VisualStudio.Properties.Resources.mysql_project_wizard;
      this.pictureBox1.Location = new System.Drawing.Point(19, 23);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(214, 88);
      this.pictureBox1.TabIndex = 0;
      this.pictureBox1.TabStop = false;
      // 
      // lblStep
      // 
      this.lblStep.AutoSize = true;
      this.lblStep.BackColor = System.Drawing.Color.White;
      this.lblStep.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblStep.Location = new System.Drawing.Point(768, 26);
      this.lblStep.Name = "lblStep";
      this.lblStep.Size = new System.Drawing.Size(90, 25);
      this.lblStep.TabIndex = 21;
      this.lblStep.Text = "Step 3 / 3";
      // 
      // lblStepTitle
      // 
      this.lblStepTitle.AutoSize = true;
      this.lblStepTitle.BackColor = System.Drawing.Color.White;
      this.lblStepTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblStepTitle.Location = new System.Drawing.Point(275, 26);
      this.lblStepTitle.Name = "lblStepTitle";
      this.lblStepTitle.Size = new System.Drawing.Size(91, 25);
      this.lblStepTitle.TabIndex = 20;
      this.lblStepTitle.Text = "Step Title";
      // 
      // label2
      // 
      this.label2.BackColor = System.Drawing.Color.White;
      this.label2.Location = new System.Drawing.Point(254, 2);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(686, 80);
      this.label2.TabIndex = 19;
      // 
      // panel1
      // 
      this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(41)))), ((int)(((byte)(41)))));
      this.panel1.Controls.Add(this.lblDescription);
      this.panel1.Controls.Add(this.lblWizardName);
      this.panel1.Controls.Add(this.pictureBox1);
      this.panel1.Location = new System.Drawing.Point(-1, -1);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(255, 542);
      this.panel1.TabIndex = 18;
      // 
      // BaseWizardForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnCancel;
      this.ClientSize = new System.Drawing.Size(873, 510);
      this.Controls.Add(this.lblStep);
      this.Controls.Add(this.lblStepTitle);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.panel1);
      this.Controls.Add(this.btnFinish);
      this.Controls.Add(this.btnCancel);
      this.Controls.Add(this.btnBack);
      this.Controls.Add(this.btnNext);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.MaximizeBox = false;
      this.Name = "BaseWizardForm";
      this.Text = "BaseWizardForm";
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected System.Windows.Forms.Button btnNext;
    protected System.Windows.Forms.Button btnCancel;
    protected System.Windows.Forms.Button btnBack;
    internal System.Windows.Forms.Label lblDescription;
    private System.Windows.Forms.Label lblWizardName;
    private System.Windows.Forms.PictureBox pictureBox1;
    internal System.Windows.Forms.Label lblStep;
    internal System.Windows.Forms.Label lblStepTitle;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Panel panel1;
    public System.Windows.Forms.Button btnFinish;
  }
}