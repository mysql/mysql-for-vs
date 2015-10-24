namespace MySql.Data.VisualStudio.Wizards.ItemTemplates
{
  partial class ModelEntitySelection_WinForms : ModelEntitySelection
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
      this.radControls = new System.Windows.Forms.RadioButton();
      this.radGrid = new System.Windows.Forms.RadioButton();
      this.radMasterDetail = new System.Windows.Forms.RadioButton();
      this.cmbFkConstraints = new System.Windows.Forms.ComboBox();
      this.label5 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.lblTableName = new System.Windows.Forms.Label();
      this.pnlLayout = new System.Windows.Forms.Panel();
      this.pnlLayout.SuspendLayout();
      this.SuspendLayout();
      // 
      // radControls
      // 
      this.radControls.AutoSize = true;
      this.radControls.Checked = true;
      this.radControls.Location = new System.Drawing.Point(6, 27);
      this.radControls.Name = "radControls";
      this.radControls.Size = new System.Drawing.Size(139, 19);
      this.radControls.TabIndex = 81;
      this.radControls.TabStop = true;
      this.radControls.Text = "Single-column layout";
      this.radControls.UseVisualStyleBackColor = true;
      // 
      // radGrid
      // 
      this.radGrid.AutoSize = true;
      this.radGrid.Location = new System.Drawing.Point(6, 76);
      this.radGrid.Name = "radGrid";
      this.radGrid.Size = new System.Drawing.Size(47, 19);
      this.radGrid.TabIndex = 82;
      this.radGrid.Text = "Grid";
      this.radGrid.UseVisualStyleBackColor = true;
      // 
      // radMasterDetail
      // 
      this.radMasterDetail.AutoSize = true;
      this.radMasterDetail.Location = new System.Drawing.Point(6, 123);
      this.radMasterDetail.Name = "radMasterDetail";
      this.radMasterDetail.Size = new System.Drawing.Size(96, 19);
      this.radMasterDetail.TabIndex = 83;
      this.radMasterDetail.TabStop = true;
      this.radMasterDetail.Text = "Master-Detail";
      this.radMasterDetail.UseVisualStyleBackColor = true;
      // 
      // cmbFkConstraints
      // 
      this.cmbFkConstraints.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.cmbFkConstraints.Enabled = false;
      this.cmbFkConstraints.FormattingEnabled = true;
      this.cmbFkConstraints.Location = new System.Drawing.Point(26, 192);
      this.cmbFkConstraints.Name = "cmbFkConstraints";
      this.cmbFkConstraints.Size = new System.Drawing.Size(265, 23);
      this.cmbFkConstraints.TabIndex = 84;
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label5.Location = new System.Drawing.Point(23, 47);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(265, 15);
      this.label5.TabIndex = 85;
      this.label5.Text = "A form based interface with individual text boxes.";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(23, 96);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(294, 15);
      this.label2.TabIndex = 86;
      this.label2.Text = "A table layout with rows and columns of a single table.";
      // 
      // label1
      // 
      this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(23, 143);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(388, 17);
      this.label1.TabIndex = 87;
      this.label1.Text = "A Parent-child relationship form. This layout is very useful for two related tabl" +
    "es";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(23, 174);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(126, 15);
      this.label4.TabIndex = 88;
      this.label4.Text = "Select the detail entity:";
      // 
      // lblTableName
      // 
      this.lblTableName.AutoSize = true;
      this.lblTableName.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblTableName.Location = new System.Drawing.Point(3, 11);
      this.lblTableName.Name = "lblTableName";
      this.lblTableName.Size = new System.Drawing.Size(84, 13);
      this.lblTableName.TabIndex = 89;
      this.lblTableName.Text = "Choose layout:";
      // 
      // pnlLayout
      // 
      this.pnlLayout.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pnlLayout.Controls.Add(this.lblTableName);
      this.pnlLayout.Controls.Add(this.label4);
      this.pnlLayout.Controls.Add(this.label1);
      this.pnlLayout.Controls.Add(this.label2);
      this.pnlLayout.Controls.Add(this.label5);
      this.pnlLayout.Controls.Add(this.cmbFkConstraints);
      this.pnlLayout.Controls.Add(this.radMasterDetail);
      this.pnlLayout.Controls.Add(this.radGrid);
      this.pnlLayout.Controls.Add(this.radControls);
      this.pnlLayout.Enabled = false;
      this.pnlLayout.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.pnlLayout.Location = new System.Drawing.Point(3, 127);
      this.pnlLayout.Name = "pnlLayout";
      this.pnlLayout.Size = new System.Drawing.Size(488, 219);
      this.pnlLayout.TabIndex = 97;
      // 
      // ModelEntitySelection_WinForms
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.pnlLayout);
      this.Name = "ModelEntitySelection_WinForms";
      this.Size = new System.Drawing.Size(495, 352);
      this.Controls.SetChildIndex(this.pnlLayout, 0);
      this.pnlLayout.ResumeLayout(false);
      this.pnlLayout.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Panel pnlLayout;
    private System.Windows.Forms.Label lblTableName;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.ComboBox cmbFkConstraints;
    private System.Windows.Forms.RadioButton radMasterDetail;
    private System.Windows.Forms.RadioButton radGrid;
    private System.Windows.Forms.RadioButton radControls;

  }
}
