namespace MySql.Data.VisualStudio.Wizards.Web
{
  partial class ModelConfiguration
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
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.rdbNoModel = new System.Windows.Forms.RadioButton();
      this.Ef6 = new System.Windows.Forms.RadioButton();
      this.Ef5 = new System.Windows.Forms.RadioButton();
      this.label7 = new System.Windows.Forms.Label();
      this.ModelNameTextBox = new System.Windows.Forms.TextBox();
      this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
      this.cmbConnections = new System.Windows.Forms.ComboBox();
      this.newConnString = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.ConnectionStringTextBox = new System.Windows.Forms.TextBox();
      this.chkUseSameConnection = new System.Windows.Forms.CheckBox();
      this.groupBox1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
      this.SuspendLayout();
      // 
      // groupBox1
      // 
      this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(242)))));
      this.groupBox1.Controls.Add(this.rdbNoModel);
      this.groupBox1.Controls.Add(this.Ef6);
      this.groupBox1.Controls.Add(this.Ef5);
      this.groupBox1.Location = new System.Drawing.Point(22, 61);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(540, 51);
      this.groupBox1.TabIndex = 55;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Entity Framework version";
      // 
      // rdbNoModel
      // 
      this.rdbNoModel.AutoSize = true;
      this.rdbNoModel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(242)))));
      this.rdbNoModel.Location = new System.Drawing.Point(132, 23);
      this.rdbNoModel.Name = "rdbNoModel";
      this.rdbNoModel.Size = new System.Drawing.Size(146, 17);
      this.rdbNoModel.TabIndex = 21;
      this.rdbNoModel.TabStop = true;
      this.rdbNoModel.Text = "Do not include a model";
      this.rdbNoModel.UseVisualStyleBackColor = false;
      // 
      // Ef6
      // 
      this.Ef6.AutoSize = true;
      this.Ef6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(242)))));
      this.Ef6.Location = new System.Drawing.Point(75, 23);
      this.Ef6.Name = "Ef6";
      this.Ef6.Size = new System.Drawing.Size(40, 17);
      this.Ef6.TabIndex = 20;
      this.Ef6.TabStop = true;
      this.Ef6.Text = "6.0";
      this.Ef6.UseVisualStyleBackColor = false;
      // 
      // Ef5
      // 
      this.Ef5.AutoSize = true;
      this.Ef5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(242)))));
      this.Ef5.Checked = true;
      this.Ef5.Location = new System.Drawing.Point(18, 23);
      this.Ef5.Name = "Ef5";
      this.Ef5.Size = new System.Drawing.Size(40, 17);
      this.Ef5.TabIndex = 19;
      this.Ef5.TabStop = true;
      this.Ef5.Text = "5.0";
      this.Ef5.UseVisualStyleBackColor = false;
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(242)))));
      this.label7.Location = new System.Drawing.Point(22, 281);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(75, 13);
      this.label7.TabIndex = 57;
      this.label7.Text = "Model Name:";
      // 
      // ModelNameTextBox
      // 
      this.ModelNameTextBox.Location = new System.Drawing.Point(22, 302);
      this.ModelNameTextBox.Name = "ModelNameTextBox";
      this.ModelNameTextBox.Size = new System.Drawing.Size(540, 22);
      this.ModelNameTextBox.TabIndex = 56;
      // 
      // errorProvider1
      // 
      this.errorProvider1.ContainerControl = this;
      // 
      // cmbConnections
      // 
      this.cmbConnections.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cmbConnections.FormattingEnabled = true;
      this.cmbConnections.Location = new System.Drawing.Point(22, 150);
      this.cmbConnections.Name = "cmbConnections";
      this.cmbConnections.Size = new System.Drawing.Size(414, 21);
      this.cmbConnections.TabIndex = 59;
      // 
      // newConnString
      // 
      this.newConnString.Location = new System.Drawing.Point(443, 149);
      this.newConnString.Name = "newConnString";
      this.newConnString.Size = new System.Drawing.Size(120, 25);
      this.newConnString.TabIndex = 58;
      this.newConnString.Text = "New Connection...";
      this.newConnString.UseVisualStyleBackColor = true;
      this.newConnString.Click += new System.EventHandler(this.newConnString_Click);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(242)))));
      this.label1.Location = new System.Drawing.Point(22, 128);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(340, 13);
      this.label1.TabIndex = 60;
      this.label1.Text = "Select the database connection to use in your model generation:";
      // 
      // ConnectionStringTextBox
      // 
      this.ConnectionStringTextBox.Location = new System.Drawing.Point(22, 201);
      this.ConnectionStringTextBox.Multiline = true;
      this.ConnectionStringTextBox.Name = "ConnectionStringTextBox";
      this.ConnectionStringTextBox.ReadOnly = true;
      this.ConnectionStringTextBox.Size = new System.Drawing.Size(541, 63);
      this.ConnectionStringTextBox.TabIndex = 62;
      // 
      // chkUseSameConnection
      // 
      this.chkUseSameConnection.AutoSize = true;
      this.chkUseSameConnection.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.chkUseSameConnection.Location = new System.Drawing.Point(25, 26);
      this.chkUseSameConnection.Name = "chkUseSameConnection";
      this.chkUseSameConnection.Size = new System.Drawing.Size(284, 17);
      this.chkUseSameConnection.TabIndex = 64;
      this.chkUseSameConnection.Text = "Use a different connection for the Data Entity Model";
      this.chkUseSameConnection.UseVisualStyleBackColor = true;
      this.chkUseSameConnection.CheckedChanged += new System.EventHandler(this.chkUseSameConnection_CheckedChanged);
      // 
      // ModelConfiguration
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.Control;
      this.Controls.Add(this.chkUseSameConnection);
      this.Controls.Add(this.ConnectionStringTextBox);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.cmbConnections);
      this.Controls.Add(this.newConnString);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.label7);
      this.Controls.Add(this.ModelNameTextBox);
      this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Name = "ModelConfiguration";
      this.Size = new System.Drawing.Size(584, 380);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.RadioButton Ef6;
    private System.Windows.Forms.RadioButton Ef5;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.TextBox ModelNameTextBox;
    private System.Windows.Forms.RadioButton rdbNoModel;
    private System.Windows.Forms.ErrorProvider errorProvider1;
    private System.Windows.Forms.ComboBox cmbConnections;
    private System.Windows.Forms.Button newConnString;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox ConnectionStringTextBox;
    private System.Windows.Forms.CheckBox chkUseSameConnection;
  }
}
