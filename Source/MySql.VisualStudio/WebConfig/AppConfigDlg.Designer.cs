namespace MySql.Data.VisualStudio.WebConfig
{
  partial class AppConfigDlg
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AppConfigDlg));
      this.connectionString = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.writeExToLog = new System.Windows.Forms.CheckBox();
      this.appName = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.autogenSchema = new System.Windows.Forms.CheckBox();
      this.appDescription = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.editConnString = new System.Windows.Forms.Button();
      this.cancelButton = new System.Windows.Forms.Button();
      this.nextButton = new System.Windows.Forms.Button();
      this.configPanel = new System.Windows.Forms.Panel();
      this.useProvider = new System.Windows.Forms.CheckBox();
      this.pnlSimpleMembership = new System.Windows.Forms.Panel();
      this.chbAutoGenTbl = new System.Windows.Forms.CheckBox();
      this.txtUserNameCol = new System.Windows.Forms.TextBox();
      this.lblUserNameCol = new System.Windows.Forms.Label();
      this.txtUserIdCol = new System.Windows.Forms.TextBox();
      this.lblUserIdCol = new System.Windows.Forms.Label();
      this.txtUserTable = new System.Windows.Forms.TextBox();
      this.lblUserTable = new System.Windows.Forms.Label();
      this.btnEditSM = new System.Windows.Forms.Button();
      this.txtConnStringSM = new System.Windows.Forms.TextBox();
      this.lblConnString = new System.Windows.Forms.Label();
      this.txtConnStringName = new System.Windows.Forms.TextBox();
      this.lblConnStringName = new System.Windows.Forms.Label();
      this.controlPanel = new System.Windows.Forms.Panel();
      this.enableExpCallback = new System.Windows.Forms.CheckBox();
      this.advancedBtn = new System.Windows.Forms.Button();
      this.entityFrameworkPanel = new System.Windows.Forms.Panel();
      this.radioBtnEF5 = new System.Windows.Forms.RadioButton();
      this.radioBtnEF6 = new System.Windows.Forms.RadioButton();
      this.label4 = new System.Windows.Forms.Label();
      this.pageLabel = new System.Windows.Forms.Label();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.pageDesc = new System.Windows.Forms.Label();
      this.backButton = new System.Windows.Forms.Button();
      this.pnlSteps = new System.Windows.Forms.Panel();
      this.pictureBox2 = new System.Windows.Forms.PictureBox();
      this.configPanel.SuspendLayout();
      this.pnlSimpleMembership.SuspendLayout();
      this.controlPanel.SuspendLayout();
      this.entityFrameworkPanel.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.pnlSteps.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
      this.SuspendLayout();
      // 
      // connectionString
      // 
      this.connectionString.Location = new System.Drawing.Point(114, 63);
      this.connectionString.Multiline = true;
      this.connectionString.Name = "connectionString";
      this.connectionString.Size = new System.Drawing.Size(276, 40);
      this.connectionString.TabIndex = 2;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(4, 65);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(94, 13);
      this.label2.TabIndex = 3;
      this.label2.Text = "Connection String:";
      // 
      // writeExToLog
      // 
      this.writeExToLog.AutoSize = true;
      this.writeExToLog.Location = new System.Drawing.Point(114, 134);
      this.writeExToLog.Name = "writeExToLog";
      this.writeExToLog.Size = new System.Drawing.Size(164, 17);
      this.writeExToLog.TabIndex = 5;
      this.writeExToLog.Text = "Write exceptions to event log";
      this.writeExToLog.UseVisualStyleBackColor = true;
      // 
      // appName
      // 
      this.appName.Location = new System.Drawing.Point(114, 9);
      this.appName.Name = "appName";
      this.appName.Size = new System.Drawing.Size(363, 20);
      this.appName.TabIndex = 0;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(60, 12);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(38, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Name:";
      // 
      // autogenSchema
      // 
      this.autogenSchema.AutoSize = true;
      this.autogenSchema.Location = new System.Drawing.Point(114, 111);
      this.autogenSchema.Name = "autogenSchema";
      this.autogenSchema.Size = new System.Drawing.Size(132, 17);
      this.autogenSchema.TabIndex = 4;
      this.autogenSchema.Text = "Autogenerate Schema";
      this.autogenSchema.UseVisualStyleBackColor = true;
      // 
      // appDescription
      // 
      this.appDescription.Location = new System.Drawing.Point(114, 36);
      this.appDescription.Name = "appDescription";
      this.appDescription.Size = new System.Drawing.Size(363, 20);
      this.appDescription.TabIndex = 1;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(35, 38);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(63, 13);
      this.label3.TabIndex = 6;
      this.label3.Text = "Description:";
      // 
      // editConnString
      // 
      this.editConnString.Location = new System.Drawing.Point(396, 63);
      this.editConnString.Name = "editConnString";
      this.editConnString.Size = new System.Drawing.Size(81, 25);
      this.editConnString.TabIndex = 3;
      this.editConnString.Text = "Edit...";
      this.editConnString.UseVisualStyleBackColor = true;
      this.editConnString.Click += new System.EventHandler(this.editConnString_Click);
      // 
      // cancelButton
      // 
      this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cancelButton.Location = new System.Drawing.Point(586, 303);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new System.Drawing.Size(75, 23);
      this.cancelButton.TabIndex = 2;
      this.cancelButton.Text = "Cancel";
      this.cancelButton.UseVisualStyleBackColor = true;
      // 
      // nextButton
      // 
      this.nextButton.Location = new System.Drawing.Point(499, 303);
      this.nextButton.Name = "nextButton";
      this.nextButton.Size = new System.Drawing.Size(75, 23);
      this.nextButton.TabIndex = 1;
      this.nextButton.Text = "Next";
      this.nextButton.UseVisualStyleBackColor = true;
      this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
      // 
      // configPanel
      // 
      this.configPanel.Controls.Add(this.useProvider);
      this.configPanel.Controls.Add(this.pnlSimpleMembership);
      this.configPanel.Location = new System.Drawing.Point(188, 63);
      this.configPanel.Name = "configPanel";
      this.configPanel.Size = new System.Drawing.Size(492, 225);
      this.configPanel.TabIndex = 8;
      this.configPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.configPanel_Paint);
      // 
      // useProvider
      // 
      this.useProvider.AutoSize = true;
      this.useProvider.Location = new System.Drawing.Point(118, 11);
      this.useProvider.Name = "useProvider";
      this.useProvider.Size = new System.Drawing.Size(80, 17);
      this.useProvider.TabIndex = 10;
      this.useProvider.Text = "checkBox1";
      this.useProvider.UseVisualStyleBackColor = true;
      this.useProvider.CheckStateChanged += new System.EventHandler(this.useProvider_CheckStateChanged);
      // 
      // pnlSimpleMembership
      // 
      this.pnlSimpleMembership.Controls.Add(this.chbAutoGenTbl);
      this.pnlSimpleMembership.Controls.Add(this.txtUserNameCol);
      this.pnlSimpleMembership.Controls.Add(this.lblUserNameCol);
      this.pnlSimpleMembership.Controls.Add(this.txtUserIdCol);
      this.pnlSimpleMembership.Controls.Add(this.lblUserIdCol);
      this.pnlSimpleMembership.Controls.Add(this.txtUserTable);
      this.pnlSimpleMembership.Controls.Add(this.lblUserTable);
      this.pnlSimpleMembership.Controls.Add(this.btnEditSM);
      this.pnlSimpleMembership.Controls.Add(this.txtConnStringSM);
      this.pnlSimpleMembership.Controls.Add(this.lblConnString);
      this.pnlSimpleMembership.Controls.Add(this.txtConnStringName);
      this.pnlSimpleMembership.Controls.Add(this.lblConnStringName);
      this.pnlSimpleMembership.Enabled = false;
      this.pnlSimpleMembership.Location = new System.Drawing.Point(3, 30);
      this.pnlSimpleMembership.Name = "pnlSimpleMembership";
      this.pnlSimpleMembership.Size = new System.Drawing.Size(486, 187);
      this.pnlSimpleMembership.TabIndex = 13;
      this.pnlSimpleMembership.Visible = false;
      // 
      // chbAutoGenTbl
      // 
      this.chbAutoGenTbl.AutoSize = true;
      this.chbAutoGenTbl.Checked = true;
      this.chbAutoGenTbl.CheckState = System.Windows.Forms.CheckState.Checked;
      this.chbAutoGenTbl.Location = new System.Drawing.Point(115, 171);
      this.chbAutoGenTbl.Name = "chbAutoGenTbl";
      this.chbAutoGenTbl.Size = new System.Drawing.Size(117, 17);
      this.chbAutoGenTbl.TabIndex = 11;
      this.chbAutoGenTbl.Text = "Auto Create Tables";
      this.chbAutoGenTbl.UseVisualStyleBackColor = true;
      // 
      // txtUserNameCol
      // 
      this.txtUserNameCol.Location = new System.Drawing.Point(114, 141);
      this.txtUserNameCol.Name = "txtUserNameCol";
      this.txtUserNameCol.Size = new System.Drawing.Size(132, 20);
      this.txtUserNameCol.TabIndex = 10;
      this.txtUserNameCol.Text = "UserName";
      // 
      // lblUserNameCol
      // 
      this.lblUserNameCol.AutoSize = true;
      this.lblUserNameCol.Location = new System.Drawing.Point(-3, 144);
      this.lblUserNameCol.Name = "lblUserNameCol";
      this.lblUserNameCol.Size = new System.Drawing.Size(101, 13);
      this.lblUserNameCol.TabIndex = 9;
      this.lblUserNameCol.Text = "User Name Column:";
      // 
      // txtUserIdCol
      // 
      this.txtUserIdCol.Location = new System.Drawing.Point(115, 111);
      this.txtUserIdCol.Name = "txtUserIdCol";
      this.txtUserIdCol.Size = new System.Drawing.Size(131, 20);
      this.txtUserIdCol.TabIndex = 8;
      this.txtUserIdCol.Text = "UserId";
      // 
      // lblUserIdCol
      // 
      this.lblUserIdCol.AutoSize = true;
      this.lblUserIdCol.Location = new System.Drawing.Point(16, 114);
      this.lblUserIdCol.Name = "lblUserIdCol";
      this.lblUserIdCol.Size = new System.Drawing.Size(82, 13);
      this.lblUserIdCol.TabIndex = 7;
      this.lblUserIdCol.Text = "User Id Column:";
      // 
      // txtUserTable
      // 
      this.txtUserTable.Location = new System.Drawing.Point(115, 81);
      this.txtUserTable.Name = "txtUserTable";
      this.txtUserTable.Size = new System.Drawing.Size(131, 20);
      this.txtUserTable.TabIndex = 6;
      this.txtUserTable.Text = "Users";
      // 
      // lblUserTable
      // 
      this.lblUserTable.AutoSize = true;
      this.lblUserTable.Location = new System.Drawing.Point(5, 84);
      this.lblUserTable.Name = "lblUserTable";
      this.lblUserTable.Size = new System.Drawing.Size(93, 13);
      this.lblUserTable.TabIndex = 5;
      this.lblUserTable.Text = "User Table Name:";
      // 
      // btnEditSM
      // 
      this.btnEditSM.Location = new System.Drawing.Point(395, 37);
      this.btnEditSM.Name = "btnEditSM";
      this.btnEditSM.Size = new System.Drawing.Size(81, 25);
      this.btnEditSM.TabIndex = 4;
      this.btnEditSM.Text = "Edit...";
      this.btnEditSM.UseVisualStyleBackColor = true;
      this.btnEditSM.Click += new System.EventHandler(this.btnEditSM_Click);
      // 
      // txtConnStringSM
      // 
      this.txtConnStringSM.Location = new System.Drawing.Point(115, 33);
      this.txtConnStringSM.Multiline = true;
      this.txtConnStringSM.Name = "txtConnStringSM";
      this.txtConnStringSM.Size = new System.Drawing.Size(275, 40);
      this.txtConnStringSM.TabIndex = 3;
      // 
      // lblConnString
      // 
      this.lblConnString.AutoSize = true;
      this.lblConnString.Location = new System.Drawing.Point(4, 40);
      this.lblConnString.Name = "lblConnString";
      this.lblConnString.Size = new System.Drawing.Size(94, 13);
      this.lblConnString.TabIndex = 2;
      this.lblConnString.Text = "Connection String:";
      // 
      // txtConnStringName
      // 
      this.txtConnStringName.Location = new System.Drawing.Point(115, 5);
      this.txtConnStringName.Name = "txtConnStringName";
      this.txtConnStringName.Size = new System.Drawing.Size(361, 20);
      this.txtConnStringName.TabIndex = 1;
      this.txtConnStringName.Text = "LocalMySqlServer";
      // 
      // lblConnStringName
      // 
      this.lblConnStringName.AutoSize = true;
      this.lblConnStringName.Location = new System.Drawing.Point(3, 8);
      this.lblConnStringName.Name = "lblConnStringName";
      this.lblConnStringName.Size = new System.Drawing.Size(95, 13);
      this.lblConnStringName.TabIndex = 0;
      this.lblConnStringName.Text = "Connection Name:";
      // 
      // controlPanel
      // 
      this.controlPanel.Controls.Add(this.enableExpCallback);
      this.controlPanel.Controls.Add(this.label1);
      this.controlPanel.Controls.Add(this.advancedBtn);
      this.controlPanel.Controls.Add(this.connectionString);
      this.controlPanel.Controls.Add(this.autogenSchema);
      this.controlPanel.Controls.Add(this.appDescription);
      this.controlPanel.Controls.Add(this.appName);
      this.controlPanel.Controls.Add(this.label2);
      this.controlPanel.Controls.Add(this.editConnString);
      this.controlPanel.Controls.Add(this.writeExToLog);
      this.controlPanel.Controls.Add(this.label3);
      this.controlPanel.Location = new System.Drawing.Point(191, 96);
      this.controlPanel.Name = "controlPanel";
      this.controlPanel.Size = new System.Drawing.Size(486, 186);
      this.controlPanel.TabIndex = 11;
      // 
      // enableExpCallback
      // 
      this.enableExpCallback.AutoSize = true;
      this.enableExpCallback.Location = new System.Drawing.Point(114, 157);
      this.enableExpCallback.Name = "enableExpCallback";
      this.enableExpCallback.Size = new System.Drawing.Size(171, 17);
      this.enableExpCallback.TabIndex = 7;
      this.enableExpCallback.Text = "Callback for session end event";
      this.enableExpCallback.UseVisualStyleBackColor = true;
      // 
      // advancedBtn
      // 
      this.advancedBtn.Location = new System.Drawing.Point(395, 153);
      this.advancedBtn.Name = "advancedBtn";
      this.advancedBtn.Size = new System.Drawing.Size(81, 25);
      this.advancedBtn.TabIndex = 6;
      this.advancedBtn.Text = "Advanced...";
      this.advancedBtn.UseVisualStyleBackColor = true;
      this.advancedBtn.Click += new System.EventHandler(this.advancedBtn_Click);
      // 
      // entityFrameworkPanel
      // 
      this.entityFrameworkPanel.Controls.Add(this.radioBtnEF5);
      this.entityFrameworkPanel.Controls.Add(this.radioBtnEF6);
      this.entityFrameworkPanel.Controls.Add(this.label4);
      this.entityFrameworkPanel.Location = new System.Drawing.Point(191, 99);
      this.entityFrameworkPanel.Name = "entityFrameworkPanel";
      this.entityFrameworkPanel.Size = new System.Drawing.Size(486, 186);
      this.entityFrameworkPanel.TabIndex = 12;
      // 
      // radioBtnEF5
      // 
      this.radioBtnEF5.AutoSize = true;
      this.radioBtnEF5.Location = new System.Drawing.Point(115, 58);
      this.radioBtnEF5.Name = "radioBtnEF5";
      this.radioBtnEF5.Size = new System.Drawing.Size(124, 17);
      this.radioBtnEF5.TabIndex = 2;
      this.radioBtnEF5.TabStop = true;
      this.radioBtnEF5.Text = "Entity Framework 5.0";
      this.radioBtnEF5.UseVisualStyleBackColor = true;
      // 
      // radioBtnEF6
      // 
      this.radioBtnEF6.AutoSize = true;
      this.radioBtnEF6.Location = new System.Drawing.Point(115, 32);
      this.radioBtnEF6.Name = "radioBtnEF6";
      this.radioBtnEF6.Size = new System.Drawing.Size(123, 17);
      this.radioBtnEF6.TabIndex = 1;
      this.radioBtnEF6.TabStop = true;
      this.radioBtnEF6.Text = "Entity Framework 6.x";
      this.radioBtnEF6.UseVisualStyleBackColor = true;
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label4.Location = new System.Drawing.Point(112, 11);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(270, 13);
      this.label4.TabIndex = 0;
      this.label4.Text = "Which version of Entity Framework do you want to use?\r\n";
      // 
      // pageLabel
      // 
      this.pageLabel.AutoSize = true;
      this.pageLabel.BackColor = System.Drawing.Color.White;
      this.pageLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.pageLabel.Location = new System.Drawing.Point(205, 10);
      this.pageLabel.Name = "pageLabel";
      this.pageLabel.Size = new System.Drawing.Size(124, 13);
      this.pageLabel.TabIndex = 9;
      this.pageLabel.Text = "Page Title Goes Here";
      // 
      // pictureBox1
      // 
      this.pictureBox1.Image = global::MySql.Data.VisualStudio.Properties.Resources.bannrbmp;
      this.pictureBox1.Location = new System.Drawing.Point(187, 1);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(493, 60);
      this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
      this.pictureBox1.TabIndex = 10;
      this.pictureBox1.TabStop = false;
      // 
      // pageDesc
      // 
      this.pageDesc.AutoSize = true;
      this.pageDesc.BackColor = System.Drawing.Color.White;
      this.pageDesc.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.pageDesc.Location = new System.Drawing.Point(231, 32);
      this.pageDesc.Name = "pageDesc";
      this.pageDesc.Size = new System.Drawing.Size(140, 13);
      this.pageDesc.TabIndex = 11;
      this.pageDesc.Text = "Page Description Goes Here";
      // 
      // backButton
      // 
      this.backButton.Location = new System.Drawing.Point(412, 303);
      this.backButton.Name = "backButton";
      this.backButton.Size = new System.Drawing.Size(75, 23);
      this.backButton.TabIndex = 0;
      this.backButton.Text = "Back";
      this.backButton.UseVisualStyleBackColor = true;
      this.backButton.Click += new System.EventHandler(this.backButton_Click);
      // 
      // pnlSteps
      // 
      this.pnlSteps.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(41)))), ((int)(((byte)(41)))));
      this.pnlSteps.Controls.Add(this.pictureBox2);
      this.pnlSteps.Location = new System.Drawing.Point(0, 0);
      this.pnlSteps.Name = "pnlSteps";
      this.pnlSteps.Size = new System.Drawing.Size(187, 342);
      this.pnlSteps.TabIndex = 13;
      // 
      // pictureBox2
      // 
      this.pictureBox2.Image = global::MySql.Data.VisualStudio.Properties.Resources.mysql_WebsiteConfig;
      this.pictureBox2.Location = new System.Drawing.Point(12, 12);
      this.pictureBox2.Name = "pictureBox2";
      this.pictureBox2.Size = new System.Drawing.Size(160, 52);
      this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
      this.pictureBox2.TabIndex = 1;
      this.pictureBox2.TabStop = false;
      // 
      // WebConfigDlg
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.cancelButton;
      this.ClientSize = new System.Drawing.Size(679, 337);
      this.Controls.Add(this.pnlSteps);
      this.Controls.Add(this.entityFrameworkPanel);
      this.Controls.Add(this.controlPanel);
      this.Controls.Add(this.backButton);
      this.Controls.Add(this.pageDesc);
      this.Controls.Add(this.pageLabel);
      this.Controls.Add(this.configPanel);
      this.Controls.Add(this.cancelButton);
      this.Controls.Add(this.nextButton);
      this.Controls.Add(this.pictureBox1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "WebConfigDlg";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "MySQL Website Configuration";
      this.configPanel.ResumeLayout(false);
      this.configPanel.PerformLayout();
      this.pnlSimpleMembership.ResumeLayout(false);
      this.pnlSimpleMembership.PerformLayout();
      this.controlPanel.ResumeLayout(false);
      this.controlPanel.PerformLayout();
      this.entityFrameworkPanel.ResumeLayout(false);
      this.entityFrameworkPanel.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.pnlSteps.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox connectionString;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.CheckBox writeExToLog;
    private System.Windows.Forms.TextBox appName;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button editConnString;
    private System.Windows.Forms.TextBox appDescription;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.CheckBox autogenSchema;
    private System.Windows.Forms.Button cancelButton;
    private System.Windows.Forms.Button nextButton;
    private System.Windows.Forms.Panel configPanel;
    private System.Windows.Forms.Label pageLabel;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Button advancedBtn;
    private System.Windows.Forms.Label pageDesc;
    private System.Windows.Forms.CheckBox useProvider;
    private System.Windows.Forms.Button backButton;
    private System.Windows.Forms.Panel controlPanel;
    private System.Windows.Forms.CheckBox enableExpCallback;
    private System.Windows.Forms.Panel pnlSimpleMembership;
    private System.Windows.Forms.CheckBox chbAutoGenTbl;
    private System.Windows.Forms.TextBox txtUserNameCol;
    private System.Windows.Forms.Label lblUserNameCol;
    private System.Windows.Forms.TextBox txtUserIdCol;
    private System.Windows.Forms.Label lblUserIdCol;
    private System.Windows.Forms.TextBox txtUserTable;
    private System.Windows.Forms.Label lblUserTable;
    private System.Windows.Forms.Button btnEditSM;
    private System.Windows.Forms.TextBox txtConnStringSM;
    private System.Windows.Forms.Label lblConnString;
    private System.Windows.Forms.TextBox txtConnStringName;
    private System.Windows.Forms.Label lblConnStringName;
    private System.Windows.Forms.Panel entityFrameworkPanel;
    private System.Windows.Forms.RadioButton radioBtnEF5;
    private System.Windows.Forms.RadioButton radioBtnEF6;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Panel pnlSteps;
    private System.Windows.Forms.PictureBox pictureBox2;
  }
}