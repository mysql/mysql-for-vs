namespace MySql.Data.VisualStudio.Wizards.Web
{
    partial class DataSourceConfiguration
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
      this.includeProfileProviderCheck = new System.Windows.Forms.CheckBox();
      this.label4 = new System.Windows.Forms.Label();
      this.ConnectionStringNameTextBox = new System.Windows.Forms.TextBox();
      this.newConnString = new System.Windows.Forms.Button();
      this.label3 = new System.Windows.Forms.Label();
      this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
      this.cmbConnections = new System.Windows.Forms.ComboBox();
      this.includeRoleProviderCheck = new System.Windows.Forms.CheckBox();
      this.ConnectionStringTextBox = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.includeSensitiveInformationCheck = new System.Windows.Forms.RadioButton();
      this.noIncludeSensitiveInformationCheck = new System.Windows.Forms.RadioButton();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
      this.SuspendLayout();
      // 
      // includeProfileProviderCheck
      // 
      this.includeProfileProviderCheck.AutoSize = true;
      this.includeProfileProviderCheck.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(242)))));
      this.includeProfileProviderCheck.Location = new System.Drawing.Point(25, 291);
      this.includeProfileProviderCheck.Name = "includeProfileProviderCheck";
      this.includeProfileProviderCheck.Size = new System.Drawing.Size(165, 17);
      this.includeProfileProviderCheck.TabIndex = 43;
      this.includeProfileProviderCheck.Text = "Use MySQL profile provider";
      this.includeProfileProviderCheck.UseVisualStyleBackColor = false;
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(242)))));
      this.label4.Location = new System.Drawing.Point(25, 238);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(310, 13);
      this.label4.TabIndex = 33;
      this.label4.Text = "Save Connection string in application configuration file as:";
      // 
      // ConnectionStringNameTextBox
      // 
      this.ConnectionStringNameTextBox.Location = new System.Drawing.Point(25, 258);
      this.ConnectionStringNameTextBox.Name = "ConnectionStringNameTextBox";
      this.ConnectionStringNameTextBox.Size = new System.Drawing.Size(541, 22);
      this.ConnectionStringNameTextBox.TabIndex = 30;
      // 
      // newConnString
      // 
      this.newConnString.Location = new System.Drawing.Point(427, 38);
      this.newConnString.Name = "newConnString";
      this.newConnString.Size = new System.Drawing.Size(137, 25);
      this.newConnString.TabIndex = 32;
      this.newConnString.Text = "New Connection...";
      this.newConnString.UseVisualStyleBackColor = true;
      this.newConnString.Click += new System.EventHandler(this.editConnString_Click);
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(242)))));
      this.label3.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.Location = new System.Drawing.Point(25, 18);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(409, 13);
      this.label3.TabIndex = 29;
      this.label3.Text = "&Which data connection should your application use to connect to the database?";
      // 
      // errorProvider1
      // 
      this.errorProvider1.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
      this.errorProvider1.ContainerControl = this;
      // 
      // cmbConnections
      // 
      this.cmbConnections.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cmbConnections.FormattingEnabled = true;
      this.cmbConnections.Location = new System.Drawing.Point(25, 40);
      this.cmbConnections.Name = "cmbConnections";
      this.cmbConnections.Size = new System.Drawing.Size(395, 21);
      this.cmbConnections.TabIndex = 49;
      // 
      // includeRoleProviderCheck
      // 
      this.includeRoleProviderCheck.AutoSize = true;
      this.includeRoleProviderCheck.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(242)))));
      this.includeRoleProviderCheck.Location = new System.Drawing.Point(25, 315);
      this.includeRoleProviderCheck.Name = "includeRoleProviderCheck";
      this.includeRoleProviderCheck.Size = new System.Drawing.Size(151, 17);
      this.includeRoleProviderCheck.TabIndex = 56;
      this.includeRoleProviderCheck.Text = "Use MySQL role provider";
      this.includeRoleProviderCheck.UseVisualStyleBackColor = false;
      // 
      // ConnectionStringTextBox
      // 
      this.ConnectionStringTextBox.Location = new System.Drawing.Point(25, 93);
      this.ConnectionStringTextBox.Multiline = true;
      this.ConnectionStringTextBox.Name = "ConnectionStringTextBox";
      this.ConnectionStringTextBox.ReadOnly = true;
      this.ConnectionStringTextBox.Size = new System.Drawing.Size(541, 84);
      this.ConnectionStringTextBox.TabIndex = 57;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(242)))));
      this.label1.Location = new System.Drawing.Point(25, 72);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(103, 13);
      this.label1.TabIndex = 60;
      this.label1.Text = "Connection string:";
      // 
      // includeSensitiveInformationCheck
      // 
      this.includeSensitiveInformationCheck.AutoSize = true;
      this.includeSensitiveInformationCheck.Location = new System.Drawing.Point(25, 211);
      this.includeSensitiveInformationCheck.Name = "includeSensitiveInformationCheck";
      this.includeSensitiveInformationCheck.Size = new System.Drawing.Size(323, 17);
      this.includeSensitiveInformationCheck.TabIndex = 61;
      this.includeSensitiveInformationCheck.TabStop = true;
      this.includeSensitiveInformationCheck.Text = "Yes, Include sensitive information in the connection string";
      this.includeSensitiveInformationCheck.UseVisualStyleBackColor = true;
      // 
      // noIncludeSensitiveInformationCheck
      // 
      this.noIncludeSensitiveInformationCheck.AutoSize = true;
      this.noIncludeSensitiveInformationCheck.Checked = true;
      this.noIncludeSensitiveInformationCheck.Location = new System.Drawing.Point(25, 188);
      this.noIncludeSensitiveInformationCheck.Name = "noIncludeSensitiveInformationCheck";
      this.noIncludeSensitiveInformationCheck.Size = new System.Drawing.Size(503, 17);
      this.noIncludeSensitiveInformationCheck.TabIndex = 62;
      this.noIncludeSensitiveInformationCheck.TabStop = true;
      this.noIncludeSensitiveInformationCheck.Text = "No, Exclude sensitive information in the connection string. I will set it in my a" +
    "pplication code.";
      this.noIncludeSensitiveInformationCheck.UseVisualStyleBackColor = true;
      // 
      // DataSourceConfiguration
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.Control;
      this.Controls.Add(this.noIncludeSensitiveInformationCheck);
      this.Controls.Add(this.includeSensitiveInformationCheck);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.ConnectionStringTextBox);
      this.Controls.Add(this.includeRoleProviderCheck);
      this.Controls.Add(this.cmbConnections);
      this.Controls.Add(this.includeProfileProviderCheck);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.ConnectionStringNameTextBox);
      this.Controls.Add(this.newConnString);
      this.Controls.Add(this.label3);
      this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Name = "DataSourceConfiguration";
      this.Size = new System.Drawing.Size(584, 347);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.CheckBox includeProfileProviderCheck;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.TextBox ConnectionStringNameTextBox;
    private System.Windows.Forms.Button newConnString;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.ErrorProvider errorProvider1;
    private System.Windows.Forms.ComboBox cmbConnections;
    private System.Windows.Forms.CheckBox includeRoleProviderCheck;
    private System.Windows.Forms.TextBox ConnectionStringTextBox;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.RadioButton noIncludeSensitiveInformationCheck;
    private System.Windows.Forms.RadioButton includeSensitiveInformationCheck;
  }
}
