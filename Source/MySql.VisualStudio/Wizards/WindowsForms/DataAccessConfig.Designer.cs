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
      this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
      this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
      this.label3 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.ConnectionStringTextBox = new System.Windows.Forms.TextBox();
      this.cmbConnections = new System.Windows.Forms.ComboBox();
      this.newConnString = new System.Windows.Forms.Button();
      this.noIncludeSensitiveInformationCheck = new System.Windows.Forms.RadioButton();
      this.includeSensitiveInformationCheck = new System.Windows.Forms.RadioButton();
      this.grpboxTechnology = new System.Windows.Forms.GroupBox();
      this.radEF6 = new System.Windows.Forms.RadioButton();
      this.radEF5 = new System.Windows.Forms.RadioButton();
      this.radTechTypedDataSet = new System.Windows.Forms.RadioButton();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
      this.grpboxTechnology.SuspendLayout();
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
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(242)))));
      this.label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.Location = new System.Drawing.Point(28, 25);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(428, 15);
      this.label3.TabIndex = 30;
      this.label3.Text = "&Which data connection should your application use to connect to the database?";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(242)))));
      this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(28, 75);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(105, 15);
      this.label1.TabIndex = 65;
      this.label1.Text = "Connection string:";
      // 
      // ConnectionStringTextBox
      // 
      this.ConnectionStringTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.ConnectionStringTextBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ConnectionStringTextBox.Location = new System.Drawing.Point(28, 96);
      this.ConnectionStringTextBox.Multiline = true;
      this.ConnectionStringTextBox.Name = "ConnectionStringTextBox";
      this.ConnectionStringTextBox.ReadOnly = true;
      this.ConnectionStringTextBox.Size = new System.Drawing.Size(541, 84);
      this.ConnectionStringTextBox.TabIndex = 63;
      // 
      // cmbConnections
      // 
      this.cmbConnections.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.cmbConnections.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cmbConnections.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.cmbConnections.FormattingEnabled = true;
      this.cmbConnections.Location = new System.Drawing.Point(28, 45);
      this.cmbConnections.Name = "cmbConnections";
      this.cmbConnections.Size = new System.Drawing.Size(395, 23);
      this.cmbConnections.TabIndex = 62;
      // 
      // newConnString
      // 
      this.newConnString.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.newConnString.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.newConnString.Location = new System.Drawing.Point(429, 45);
      this.newConnString.Name = "newConnString";
      this.newConnString.Size = new System.Drawing.Size(140, 25);
      this.newConnString.TabIndex = 61;
      this.newConnString.Text = "New Connection...";
      this.newConnString.UseVisualStyleBackColor = true;
      this.newConnString.Click += new System.EventHandler(this.newConnString_Click);
      // 
      // noIncludeSensitiveInformationCheck
      // 
      this.noIncludeSensitiveInformationCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.noIncludeSensitiveInformationCheck.AutoSize = true;
      this.noIncludeSensitiveInformationCheck.Checked = true;
      this.noIncludeSensitiveInformationCheck.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.noIncludeSensitiveInformationCheck.Location = new System.Drawing.Point(31, 193);
      this.noIncludeSensitiveInformationCheck.Name = "noIncludeSensitiveInformationCheck";
      this.noIncludeSensitiveInformationCheck.Size = new System.Drawing.Size(515, 19);
      this.noIncludeSensitiveInformationCheck.TabIndex = 67;
      this.noIncludeSensitiveInformationCheck.TabStop = true;
      this.noIncludeSensitiveInformationCheck.Text = "No, Exclude sensitive information in the connection string. I will set it in my a" +
    "pplication code.";
      this.noIncludeSensitiveInformationCheck.UseVisualStyleBackColor = true;
      // 
      // includeSensitiveInformationCheck
      // 
      this.includeSensitiveInformationCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.includeSensitiveInformationCheck.AutoSize = true;
      this.includeSensitiveInformationCheck.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.includeSensitiveInformationCheck.Location = new System.Drawing.Point(31, 216);
      this.includeSensitiveInformationCheck.Name = "includeSensitiveInformationCheck";
      this.includeSensitiveInformationCheck.Size = new System.Drawing.Size(331, 19);
      this.includeSensitiveInformationCheck.TabIndex = 66;
      this.includeSensitiveInformationCheck.Text = "Yes, Include sensitive information in the connection string";
      this.includeSensitiveInformationCheck.UseVisualStyleBackColor = true;
      // 
      // grpboxTechnology
      // 
      this.grpboxTechnology.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.grpboxTechnology.Controls.Add(this.radEF6);
      this.grpboxTechnology.Controls.Add(this.radEF5);
      this.grpboxTechnology.Controls.Add(this.radTechTypedDataSet);
      this.grpboxTechnology.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.grpboxTechnology.Location = new System.Drawing.Point(28, 261);
      this.grpboxTechnology.Name = "grpboxTechnology";
      this.grpboxTechnology.Size = new System.Drawing.Size(541, 66);
      this.grpboxTechnology.TabIndex = 68;
      this.grpboxTechnology.TabStop = false;
      this.grpboxTechnology.Text = "Select the Data Access Technology to connect with the database";
      // 
      // radEF6
      // 
      this.radEF6.AutoSize = true;
      this.radEF6.Enabled = false;
      this.radEF6.Location = new System.Drawing.Point(373, 28);
      this.radEF6.Name = "radEF6";
      this.radEF6.Size = new System.Drawing.Size(135, 19);
      this.radEF6.TabIndex = 2;
      this.radEF6.TabStop = true;
      this.radEF6.Text = "Entity Framework 6.0";
      this.radEF6.UseVisualStyleBackColor = true;
      // 
      // radEF5
      // 
      this.radEF5.AutoSize = true;
      this.radEF5.Location = new System.Drawing.Point(184, 28);
      this.radEF5.Name = "radEF5";
      this.radEF5.Size = new System.Drawing.Size(135, 19);
      this.radEF5.TabIndex = 1;
      this.radEF5.TabStop = true;
      this.radEF5.Text = "Entity Framework 5.0";
      this.radEF5.UseVisualStyleBackColor = true;
      // 
      // radTechTypedDataSet
      // 
      this.radTechTypedDataSet.AutoSize = true;
      this.radTechTypedDataSet.Checked = true;
      this.radTechTypedDataSet.Location = new System.Drawing.Point(17, 28);
      this.radTechTypedDataSet.Name = "radTechTypedDataSet";
      this.radTechTypedDataSet.Size = new System.Drawing.Size(101, 19);
      this.radTechTypedDataSet.TabIndex = 0;
      this.radTechTypedDataSet.TabStop = true;
      this.radTechTypedDataSet.Text = "Typed DataSet";
      this.radTechTypedDataSet.UseVisualStyleBackColor = true;
      // 
      // DataAccessConfig
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.grpboxTechnology);
      this.Controls.Add(this.noIncludeSensitiveInformationCheck);
      this.Controls.Add(this.includeSensitiveInformationCheck);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.ConnectionStringTextBox);
      this.Controls.Add(this.cmbConnections);
      this.Controls.Add(this.newConnString);
      this.Controls.Add(this.label3);
      this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
      this.Name = "DataAccessConfig";
      this.Size = new System.Drawing.Size(584, 347);
      this.Validating += new System.ComponentModel.CancelEventHandler(this.DataAccessConfig_Validating);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
      this.grpboxTechnology.ResumeLayout(false);
      this.grpboxTechnology.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ErrorProvider errorProvider1;
    private System.Windows.Forms.ToolTip toolTip1;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox ConnectionStringTextBox;
    private System.Windows.Forms.ComboBox cmbConnections;
    private System.Windows.Forms.Button newConnString;
    private System.Windows.Forms.RadioButton noIncludeSensitiveInformationCheck;
    private System.Windows.Forms.RadioButton includeSensitiveInformationCheck;
    private System.Windows.Forms.GroupBox grpboxTechnology;
    private System.Windows.Forms.RadioButton radEF6;
    private System.Windows.Forms.RadioButton radEF5;
    private System.Windows.Forms.RadioButton radTechTypedDataSet;
  }
}
