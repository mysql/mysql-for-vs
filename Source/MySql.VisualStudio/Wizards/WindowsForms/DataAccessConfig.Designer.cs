namespace MySql.Data.VisualStudio.Wizards.WindowsForms
{
  partial class DataAccessConfig
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
      this.btnConnConfig = new System.Windows.Forms.Button();
      this.txtConnStr = new System.Windows.Forms.TextBox();
      this.grpboxTechnology = new System.Windows.Forms.GroupBox();
      this.radEF6 = new System.Windows.Forms.RadioButton();
      this.radEF5 = new System.Windows.Forms.RadioButton();
      this.radTechTypedDataSet = new System.Windows.Forms.RadioButton();
      this.cmbTable = new System.Windows.Forms.ComboBox();
      this.lnlConnStr = new System.Windows.Forms.Label();
      this.lblTable = new System.Windows.Forms.Label();
      this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
      this.grpboxType = new System.Windows.Forms.GroupBox();
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
      // btnConnConfig
      // 
      this.btnConnConfig.Location = new System.Drawing.Point(334, 55);
      this.btnConnConfig.Name = "btnConnConfig";
      this.btnConnConfig.Size = new System.Drawing.Size(32, 21);
      this.btnConnConfig.TabIndex = 0;
      this.btnConnConfig.Text = "...";
      this.btnConnConfig.UseVisualStyleBackColor = true;
      this.btnConnConfig.Click += new System.EventHandler(this.btnConnConfig_Click);
      // 
      // txtConnStr
      // 
      this.txtConnStr.Location = new System.Drawing.Point(26, 55);
      this.txtConnStr.Name = "txtConnStr";
      this.txtConnStr.ReadOnly = true;
      this.txtConnStr.Size = new System.Drawing.Size(302, 20);
      this.txtConnStr.TabIndex = 1;
      // 
      // grpboxTechnology
      // 
      this.grpboxTechnology.Controls.Add(this.radEF6);
      this.grpboxTechnology.Controls.Add(this.radEF5);
      this.grpboxTechnology.Controls.Add(this.radTechTypedDataSet);
      this.grpboxTechnology.Location = new System.Drawing.Point(26, 153);
      this.grpboxTechnology.Name = "grpboxTechnology";
      this.grpboxTechnology.Size = new System.Drawing.Size(330, 98);
      this.grpboxTechnology.TabIndex = 2;
      this.grpboxTechnology.TabStop = false;
      this.grpboxTechnology.Text = "Data Access Technology";
      // 
      // radEF6
      // 
      this.radEF6.AutoSize = true;
      this.radEF6.Enabled = false;
      this.radEF6.Location = new System.Drawing.Point(23, 67);
      this.radEF6.Name = "radEF6";
      this.radEF6.Size = new System.Drawing.Size(124, 17);
      this.radEF6.TabIndex = 2;
      this.radEF6.TabStop = true;
      this.radEF6.Text = "Entity Framework 6.0";
      this.radEF6.UseVisualStyleBackColor = true;
      // 
      // radEF5
      // 
      this.radEF5.AutoSize = true;
      this.radEF5.Enabled = false;
      this.radEF5.Location = new System.Drawing.Point(23, 44);
      this.radEF5.Name = "radEF5";
      this.radEF5.Size = new System.Drawing.Size(124, 17);
      this.radEF5.TabIndex = 1;
      this.radEF5.TabStop = true;
      this.radEF5.Text = "Entity Framework 5.0";
      this.radEF5.UseVisualStyleBackColor = true;
      // 
      // radTechTypedDataSet
      // 
      this.radTechTypedDataSet.AutoSize = true;
      this.radTechTypedDataSet.Checked = true;
      this.radTechTypedDataSet.Location = new System.Drawing.Point(23, 21);
      this.radTechTypedDataSet.Name = "radTechTypedDataSet";
      this.radTechTypedDataSet.Size = new System.Drawing.Size(97, 17);
      this.radTechTypedDataSet.TabIndex = 0;
      this.radTechTypedDataSet.TabStop = true;
      this.radTechTypedDataSet.Text = "Typed DataSet";
      this.radTechTypedDataSet.UseVisualStyleBackColor = true;
      // 
      // cmbTable
      // 
      this.cmbTable.FormattingEnabled = true;
      this.cmbTable.Location = new System.Drawing.Point(26, 117);
      this.cmbTable.Name = "cmbTable";
      this.cmbTable.Size = new System.Drawing.Size(330, 21);
      this.cmbTable.TabIndex = 3;
      // 
      // lnlConnStr
      // 
      this.lnlConnStr.AutoSize = true;
      this.lnlConnStr.Location = new System.Drawing.Point(23, 27);
      this.lnlConnStr.Name = "lnlConnStr";
      this.lnlConnStr.Size = new System.Drawing.Size(94, 13);
      this.lnlConnStr.TabIndex = 4;
      this.lnlConnStr.Text = "Connection String:";
      // 
      // lblTable
      // 
      this.lblTable.AutoSize = true;
      this.lblTable.Location = new System.Drawing.Point(23, 88);
      this.lblTable.Name = "lblTable";
      this.lblTable.Size = new System.Drawing.Size(37, 13);
      this.lblTable.TabIndex = 3;
      this.lblTable.Text = "Table:";
      // 
      // errorProvider1
      // 
      this.errorProvider1.ContainerControl = this;
      // 
      // grpboxType
      // 
      this.grpboxType.Controls.Add(this.cmbFkConstraints);
      this.grpboxType.Controls.Add(this.radMasterDetail);
      this.grpboxType.Controls.Add(this.radGrid);
      this.grpboxType.Controls.Add(this.radControls);
      this.grpboxType.Location = new System.Drawing.Point(26, 267);
      this.grpboxType.Name = "grpboxType";
      this.grpboxType.Size = new System.Drawing.Size(330, 100);
      this.grpboxType.TabIndex = 5;
      this.grpboxType.TabStop = false;
      this.grpboxType.Text = "Gui Layout";
      // 
      // cmbFkConstraints
      // 
      this.cmbFkConstraints.Enabled = false;
      this.cmbFkConstraints.FormattingEnabled = true;
      this.cmbFkConstraints.Location = new System.Drawing.Point(140, 65);
      this.cmbFkConstraints.Name = "cmbFkConstraints";
      this.cmbFkConstraints.Size = new System.Drawing.Size(184, 21);
      this.cmbFkConstraints.TabIndex = 3;
      this.toolTip1.SetToolTip(this.cmbFkConstraints, "Select child table through a Foreign Key constraint ");
      // 
      // radMasterDetail
      // 
      this.radMasterDetail.AutoSize = true;
      this.radMasterDetail.Enabled = false;
      this.radMasterDetail.Location = new System.Drawing.Point(36, 65);
      this.radMasterDetail.Name = "radMasterDetail";
      this.radMasterDetail.Size = new System.Drawing.Size(87, 17);
      this.radMasterDetail.TabIndex = 2;
      this.radMasterDetail.TabStop = true;
      this.radMasterDetail.Text = "Master Detail";
      this.radMasterDetail.UseVisualStyleBackColor = true;
      this.radMasterDetail.CheckedChanged += new System.EventHandler(this.radMasterDetail_CheckedChanged);
      // 
      // radGrid
      // 
      this.radGrid.AutoSize = true;
      this.radGrid.Enabled = false;
      this.radGrid.Location = new System.Drawing.Point(36, 42);
      this.radGrid.Name = "radGrid";
      this.radGrid.Size = new System.Drawing.Size(44, 17);
      this.radGrid.TabIndex = 1;
      this.radGrid.Text = "Grid";
      this.radGrid.UseVisualStyleBackColor = true;
      this.radGrid.CheckedChanged += new System.EventHandler(this.radGrid_CheckedChanged);
      // 
      // radControls
      // 
      this.radControls.AutoSize = true;
      this.radControls.Checked = true;
      this.radControls.Location = new System.Drawing.Point(36, 19);
      this.radControls.Name = "radControls";
      this.radControls.Size = new System.Drawing.Size(111, 17);
      this.radControls.TabIndex = 0;
      this.radControls.TabStop = true;
      this.radControls.Text = "Individual Controls";
      this.radControls.UseVisualStyleBackColor = true;
      this.radControls.CheckedChanged += new System.EventHandler(this.radControls_CheckedChanged);
      // 
      // toolTip1
      // 
      this.toolTip1.ShowAlways = true;
      // 
      // DataAccessConfig
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.grpboxType);
      this.Controls.Add(this.lblTable);
      this.Controls.Add(this.lnlConnStr);
      this.Controls.Add(this.cmbTable);
      this.Controls.Add(this.grpboxTechnology);
      this.Controls.Add(this.txtConnStr);
      this.Controls.Add(this.btnConnConfig);
      this.Name = "DataAccessConfig";
      this.Size = new System.Drawing.Size(414, 393);
      this.Validating += new System.ComponentModel.CancelEventHandler(this.DataAccessConfig_Validating);
      this.grpboxTechnology.ResumeLayout(false);
      this.grpboxTechnology.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
      this.grpboxType.ResumeLayout(false);
      this.grpboxType.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btnConnConfig;
    private System.Windows.Forms.TextBox txtConnStr;
    private System.Windows.Forms.GroupBox grpboxTechnology;
    private System.Windows.Forms.RadioButton radEF6;
    private System.Windows.Forms.RadioButton radEF5;
    private System.Windows.Forms.RadioButton radTechTypedDataSet;
    private System.Windows.Forms.ComboBox cmbTable;
    private System.Windows.Forms.Label lnlConnStr;
    private System.Windows.Forms.Label lblTable;
    private System.Windows.Forms.ErrorProvider errorProvider1;
    private System.Windows.Forms.GroupBox grpboxType;
    private System.Windows.Forms.RadioButton radMasterDetail;
    private System.Windows.Forms.RadioButton radGrid;
    private System.Windows.Forms.RadioButton radControls;
    private System.Windows.Forms.ComboBox cmbFkConstraints;
    private System.Windows.Forms.ToolTip toolTip1;
  }
}
