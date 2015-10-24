namespace MySql.Data.VisualStudio.Wizards.WindowsForms
{
  partial class DataAccessTechnologyConfig
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
      this.components = new System.ComponentModel.Container();
      this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
      this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
      this.cmbFkConstraints = new System.Windows.Forms.ComboBox();
      this.chkEnableAdvanced = new System.Windows.Forms.CheckBox();
      this.label4 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.radMasterDetail = new System.Windows.Forms.RadioButton();
      this.radGrid = new System.Windows.Forms.RadioButton();
      this.radControls = new System.Windows.Forms.RadioButton();
      this.lblTableName = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
      this.SuspendLayout();
      // 
      // errorProvider1
      // 
      this.errorProvider1.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
      this.errorProvider1.ContainerControl = this;
      // 
      // toolTip1
      // 
      this.toolTip1.ShowAlways = true;
      // 
      // cmbFkConstraints
      // 
      this.cmbFkConstraints.Enabled = false;
      this.cmbFkConstraints.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.cmbFkConstraints.FormattingEnabled = true;
      this.cmbFkConstraints.Location = new System.Drawing.Point(62, 264);
      this.cmbFkConstraints.Name = "cmbFkConstraints";
      this.cmbFkConstraints.Size = new System.Drawing.Size(420, 23);
      this.cmbFkConstraints.TabIndex = 11;
      this.toolTip1.SetToolTip(this.cmbFkConstraints, "Select child table through a Foreign Key constraint ");
      // 
      // chkEnableAdvanced
      // 
      this.chkEnableAdvanced.AutoSize = true;
      this.chkEnableAdvanced.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.chkEnableAdvanced.Location = new System.Drawing.Point(321, 342);
      this.chkEnableAdvanced.Name = "chkEnableAdvanced";
      this.chkEnableAdvanced.Size = new System.Drawing.Size(244, 19);
      this.chkEnableAdvanced.TabIndex = 17;
      this.chkEnableAdvanced.Text = "Show all the advanced steps in the wizard";
      this.toolTip1.SetToolTip(this.chkEnableAdvanced, "When this option is checked the wizard will show addtional steps\r\nto configure va" +
        "lidation rules for each column.");
      this.chkEnableAdvanced.UseVisualStyleBackColor = true;
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label4.Location = new System.Drawing.Point(58, 246);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(292, 15);
      this.label4.TabIndex = 15;
      this.label4.Text = "Select the constraint you want to use for a detail table:";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.Location = new System.Drawing.Point(59, 214);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(423, 15);
      this.label3.TabIndex = 14;
      this.label3.Text = "A Parent-child relationship form. This layout is very useful for two related tabl" +
    "es";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(59, 147);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(294, 15);
      this.label2.TabIndex = 13;
      this.label2.Text = "A table layout with rows and columns of a single table.";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(59, 83);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(265, 15);
      this.label1.TabIndex = 12;
      this.label1.Text = "A form based interface with individual text boxes.";
      // 
      // radMasterDetail
      // 
      this.radMasterDetail.AutoSize = true;
      this.radMasterDetail.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.radMasterDetail.Location = new System.Drawing.Point(42, 192);
      this.radMasterDetail.Name = "radMasterDetail";
      this.radMasterDetail.Size = new System.Drawing.Size(96, 19);
      this.radMasterDetail.TabIndex = 10;
      this.radMasterDetail.TabStop = true;
      this.radMasterDetail.Text = "Master-Detail";
      this.radMasterDetail.UseVisualStyleBackColor = true;
      // 
      // radGrid
      // 
      this.radGrid.AutoSize = true;
      this.radGrid.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.radGrid.Location = new System.Drawing.Point(42, 125);
      this.radGrid.Name = "radGrid";
      this.radGrid.Size = new System.Drawing.Size(47, 19);
      this.radGrid.TabIndex = 9;
      this.radGrid.Text = "Grid";
      this.radGrid.UseVisualStyleBackColor = true;
      // 
      // radControls
      // 
      this.radControls.AutoSize = true;
      this.radControls.Checked = true;
      this.radControls.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.radControls.Location = new System.Drawing.Point(42, 61);
      this.radControls.Name = "radControls";
      this.radControls.Size = new System.Drawing.Size(139, 19);
      this.radControls.TabIndex = 8;
      this.radControls.TabStop = true;
      this.radControls.Text = "Single-column layout";
      this.radControls.UseVisualStyleBackColor = true;
      // 
      // lblTableName
      // 
      this.lblTableName.AutoSize = true;
      this.lblTableName.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblTableName.Location = new System.Drawing.Point(23, 18);
      this.lblTableName.Name = "lblTableName";
      this.lblTableName.Size = new System.Drawing.Size(53, 21);
      this.lblTableName.TabIndex = 18;
      this.lblTableName.Text = "label5";
      // 
      // DataAccessTechnologyConfig
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.lblTableName);
      this.Controls.Add(this.chkEnableAdvanced);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.cmbFkConstraints);
      this.Controls.Add(this.radMasterDetail);
      this.Controls.Add(this.radGrid);
      this.Controls.Add(this.radControls);
      this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
      this.Name = "DataAccessTechnologyConfig";
      this.Size = new System.Drawing.Size(584, 380);
      this.Validating += new System.ComponentModel.CancelEventHandler(this.DataAccessTechnologyConfig_Validating);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ErrorProvider errorProvider1;
    private System.Windows.Forms.ToolTip toolTip1;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ComboBox cmbFkConstraints;
    private System.Windows.Forms.RadioButton radMasterDetail;
    private System.Windows.Forms.RadioButton radGrid;
    private System.Windows.Forms.RadioButton radControls;
    private System.Windows.Forms.CheckBox chkEnableAdvanced;
    private System.Windows.Forms.Label lblTableName;
  }
}
