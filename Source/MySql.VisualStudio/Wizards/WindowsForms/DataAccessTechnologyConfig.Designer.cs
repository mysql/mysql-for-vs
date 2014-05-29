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
      this.grpboxTechnology = new System.Windows.Forms.GroupBox();
      this.radEF6 = new System.Windows.Forms.RadioButton();
      this.radEF5 = new System.Windows.Forms.RadioButton();
      this.radTechTypedDataSet = new System.Windows.Forms.RadioButton();
      this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
      this.grpboxType = new System.Windows.Forms.GroupBox();
      this.label4 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.cmbFkConstraints = new System.Windows.Forms.ComboBox();
      this.radMasterDetail = new System.Windows.Forms.RadioButton();
      this.radGrid = new System.Windows.Forms.RadioButton();
      this.radControls = new System.Windows.Forms.RadioButton();
      this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
      this.grpboxTechnology.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
      this.grpboxType.SuspendLayout();
      this.SuspendLayout();
      // 
      // grpboxTechnology
      // 
      this.grpboxTechnology.Controls.Add(this.radEF6);
      this.grpboxTechnology.Controls.Add(this.radEF5);
      this.grpboxTechnology.Controls.Add(this.radTechTypedDataSet);
      this.grpboxTechnology.Location = new System.Drawing.Point(26, 258);
      this.grpboxTechnology.Name = "grpboxTechnology";
      this.grpboxTechnology.Size = new System.Drawing.Size(522, 75);
      this.grpboxTechnology.TabIndex = 2;
      this.grpboxTechnology.TabStop = false;
      this.grpboxTechnology.Text = "Select the Data Access Technology to connect with the database";
      // 
      // radEF6
      // 
      this.radEF6.AutoSize = true;
      this.radEF6.Enabled = false;
      this.radEF6.Location = new System.Drawing.Point(339, 35);
      this.radEF6.Name = "radEF6";
      this.radEF6.Size = new System.Drawing.Size(132, 17);
      this.radEF6.TabIndex = 2;
      this.radEF6.TabStop = true;
      this.radEF6.Text = "Entity Framework 6.0";
      this.radEF6.UseVisualStyleBackColor = true;
      // 
      // radEF5
      // 
      this.radEF5.AutoSize = true;
      this.radEF5.Location = new System.Drawing.Point(161, 35);
      this.radEF5.Name = "radEF5";
      this.radEF5.Size = new System.Drawing.Size(132, 17);
      this.radEF5.TabIndex = 1;
      this.radEF5.TabStop = true;
      this.radEF5.Text = "Entity Framework 5.0";
      this.radEF5.UseVisualStyleBackColor = true;
      // 
      // radTechTypedDataSet
      // 
      this.radTechTypedDataSet.AutoSize = true;
      this.radTechTypedDataSet.Checked = true;
      this.radTechTypedDataSet.Location = new System.Drawing.Point(17, 35);
      this.radTechTypedDataSet.Name = "radTechTypedDataSet";
      this.radTechTypedDataSet.Size = new System.Drawing.Size(98, 17);
      this.radTechTypedDataSet.TabIndex = 0;
      this.radTechTypedDataSet.TabStop = true;
      this.radTechTypedDataSet.Text = "Typed DataSet";
      this.radTechTypedDataSet.UseVisualStyleBackColor = true;
      // 
      // errorProvider1
      // 
      this.errorProvider1.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
      this.errorProvider1.ContainerControl = this;
      // 
      // grpboxType
      // 
      this.grpboxType.Controls.Add(this.label4);
      this.grpboxType.Controls.Add(this.label3);
      this.grpboxType.Controls.Add(this.label2);
      this.grpboxType.Controls.Add(this.label1);
      this.grpboxType.Controls.Add(this.cmbFkConstraints);
      this.grpboxType.Controls.Add(this.radMasterDetail);
      this.grpboxType.Controls.Add(this.radGrid);
      this.grpboxType.Controls.Add(this.radControls);
      this.grpboxType.Location = new System.Drawing.Point(26, 16);
      this.grpboxType.Name = "grpboxType";
      this.grpboxType.Size = new System.Drawing.Size(522, 224);
      this.grpboxType.TabIndex = 5;
      this.grpboxType.TabStop = false;
      this.grpboxType.Text = "Select the graphical user interface type";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(43, 190);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(201, 13);
      this.label4.TabIndex = 7;
      this.label4.Text = "Select the constraint you want to use:";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.Location = new System.Drawing.Point(34, 154);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(384, 13);
      this.label3.TabIndex = 6;
      this.label3.Text = "A Parent-child relationship form. This layout is very useful for two related tables";    
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(34, 93);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(266, 13);
      this.label2.TabIndex = 5;
      this.label2.Text = "A table layout with rows and columns of a single table.";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(34, 45);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(234, 13);
      this.label1.TabIndex = 4;
      this.label1.Text = "A form based interface with individual text boxes.";
      // 
      // cmbFkConstraints
      // 
      this.cmbFkConstraints.Enabled = false;
      this.cmbFkConstraints.FormattingEnabled = true;
      this.cmbFkConstraints.Location = new System.Drawing.Point(250, 187);
      this.cmbFkConstraints.Name = "cmbFkConstraints";
      this.cmbFkConstraints.Size = new System.Drawing.Size(255, 21);
      this.cmbFkConstraints.TabIndex = 3;
      this.toolTip1.SetToolTip(this.cmbFkConstraints, "Select child table through a Foreign Key constraint ");
      // 
      // radMasterDetail
      // 
      this.radMasterDetail.AutoSize = true;
      this.radMasterDetail.Location = new System.Drawing.Point(17, 134);
      this.radMasterDetail.Name = "radMasterDetail";
      this.radMasterDetail.Size = new System.Drawing.Size(94, 17);
      this.radMasterDetail.TabIndex = 2;
      this.radMasterDetail.TabStop = true;
      this.radMasterDetail.Text = "Master-Detail";
      this.radMasterDetail.UseVisualStyleBackColor = true;
      this.radMasterDetail.CheckedChanged += new System.EventHandler(this.radMasterDetail_CheckedChanged);
      // 
      // radGrid
      // 
      this.radGrid.AutoSize = true;
      this.radGrid.Location = new System.Drawing.Point(17, 73);
      this.radGrid.Name = "radGrid";
      this.radGrid.Size = new System.Drawing.Size(47, 17);
      this.radGrid.TabIndex = 1;
      this.radGrid.Text = "Grid";
      this.radGrid.UseVisualStyleBackColor = true;
      this.radGrid.CheckedChanged += new System.EventHandler(this.radGrid_CheckedChanged);
      // 
      // radControls
      // 
      this.radControls.AutoSize = true;
      this.radControls.Checked = true;
      this.radControls.Location = new System.Drawing.Point(17, 25);
      this.radControls.Name = "radControls";
      this.radControls.Size = new System.Drawing.Size(134, 17);
      this.radControls.TabIndex = 0;
      this.radControls.TabStop = true;
      this.radControls.Text = "Single-column layout";
      this.radControls.UseVisualStyleBackColor = true;
      this.radControls.CheckedChanged += new System.EventHandler(this.radControls_CheckedChanged);
      // 
      // toolTip1
      // 
      this.toolTip1.ShowAlways = true;
      // 
      // DataAccessTechnologyConfig
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.grpboxType);
      this.Controls.Add(this.grpboxTechnology);
      this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
      this.Name = "DataAccessTechnologyConfig";
      this.Size = new System.Drawing.Size(584, 380);
      this.Validating += new System.ComponentModel.CancelEventHandler(this.DataAccessTechnologyConfig_Validating);
      this.grpboxTechnology.ResumeLayout(false);
      this.grpboxTechnology.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
      this.grpboxType.ResumeLayout(false);
      this.grpboxType.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox grpboxTechnology;
    private System.Windows.Forms.RadioButton radEF6;
    private System.Windows.Forms.RadioButton radEF5;
    private System.Windows.Forms.RadioButton radTechTypedDataSet;
    private System.Windows.Forms.ErrorProvider errorProvider1;
    private System.Windows.Forms.GroupBox grpboxType;
    private System.Windows.Forms.RadioButton radMasterDetail;
    private System.Windows.Forms.RadioButton radGrid;
    private System.Windows.Forms.RadioButton radControls;
    private System.Windows.Forms.ComboBox cmbFkConstraints;
    private System.Windows.Forms.ToolTip toolTip1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label3;
  }
}
