namespace MySql.Data.VisualStudio.Wizards.Web
{
  partial class ProviderConfiguration
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
      this.chkWriteExceptions = new System.Windows.Forms.CheckBox();
      this.chkQuestionAndAnswerRequired = new System.Windows.Forms.CheckBox();
      this.label6 = new System.Windows.Forms.Label();
      this.txtMinimumPasswordLenght = new System.Windows.Forms.TextBox();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.txtUserName = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.createAdministratorUserCheck = new System.Windows.Forms.CheckBox();
      this.label8 = new System.Windows.Forms.Label();
      this.label7 = new System.Windows.Forms.Label();
      this.txtPwdConfirm = new System.Windows.Forms.TextBox();
      this.txtPwd = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.txtQuestion = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.txtAnswer = new System.Windows.Forms.TextBox();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
      this.groupBox1.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.SuspendLayout();
      // 
      // errorProvider1
      // 
      this.errorProvider1.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
      this.errorProvider1.ContainerControl = this;
      // 
      // chkWriteExceptions
      // 
      this.chkWriteExceptions.AutoSize = true;
      this.chkWriteExceptions.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(242)))));
      this.chkWriteExceptions.Location = new System.Drawing.Point(22, 28);
      this.chkWriteExceptions.Name = "chkWriteExceptions";
      this.chkWriteExceptions.Size = new System.Drawing.Size(176, 19);
      this.chkWriteExceptions.TabIndex = 63;
      this.chkWriteExceptions.Text = "Write Exceptions to Eventlog";
      this.chkWriteExceptions.UseVisualStyleBackColor = false;
      // 
      // chkQuestionAndAnswerRequired
      // 
      this.chkQuestionAndAnswerRequired.AutoSize = true;
      this.chkQuestionAndAnswerRequired.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(242)))));
      this.chkQuestionAndAnswerRequired.Location = new System.Drawing.Point(22, 51);
      this.chkQuestionAndAnswerRequired.Name = "chkQuestionAndAnswerRequired";
      this.chkQuestionAndAnswerRequired.Size = new System.Drawing.Size(182, 19);
      this.chkQuestionAndAnswerRequired.TabIndex = 65;
      this.chkQuestionAndAnswerRequired.Text = "Require Question and Answer";
      this.chkQuestionAndAnswerRequired.UseVisualStyleBackColor = false;
      this.chkQuestionAndAnswerRequired.CheckedChanged += new System.EventHandler(this.chkQuestionAndAnswerRequired_CheckedChanged);
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(242)))));
      this.label6.Location = new System.Drawing.Point(267, 29);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(153, 15);
      this.label6.TabIndex = 37;
      this.label6.Text = "Minimum password length:";
      // 
      // txtMinimumPasswordLenght
      // 
      this.txtMinimumPasswordLenght.Location = new System.Drawing.Point(426, 26);
      this.txtMinimumPasswordLenght.Name = "txtMinimumPasswordLenght";
      this.txtMinimumPasswordLenght.Size = new System.Drawing.Size(50, 23);
      this.txtMinimumPasswordLenght.TabIndex = 64;
      // 
      // groupBox1
      // 
      this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBox1.Controls.Add(this.txtMinimumPasswordLenght);
      this.groupBox1.Controls.Add(this.label6);
      this.groupBox1.Controls.Add(this.chkQuestionAndAnswerRequired);
      this.groupBox1.Controls.Add(this.chkWriteExceptions);
      this.groupBox1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBox1.Location = new System.Drawing.Point(25, 20);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(541, 84);
      this.groupBox1.TabIndex = 50;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "MySQL Membership Provider general options";
      // 
      // txtUserName
      // 
      this.txtUserName.Location = new System.Drawing.Point(139, 29);
      this.txtUserName.Name = "txtUserName";
      this.txtUserName.Size = new System.Drawing.Size(100, 23);
      this.txtUserName.TabIndex = 67;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(19, 32);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(68, 15);
      this.label1.TabIndex = 62;
      this.label1.Text = "User Name:";
      // 
      // createAdministratorUserCheck
      // 
      this.createAdministratorUserCheck.AutoSize = true;
      this.createAdministratorUserCheck.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(242)))));
      this.createAdministratorUserCheck.Location = new System.Drawing.Point(16, 0);
      this.createAdministratorUserCheck.Name = "createAdministratorUserCheck";
      this.createAdministratorUserCheck.Size = new System.Drawing.Size(159, 19);
      this.createAdministratorUserCheck.TabIndex = 66;
      this.createAdministratorUserCheck.Text = "Create administrator user";
      this.createAdministratorUserCheck.UseVisualStyleBackColor = false;
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(242)))));
      this.label8.Location = new System.Drawing.Point(19, 94);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(107, 15);
      this.label8.TabIndex = 60;
      this.label8.Text = "Confirm Password:";
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(242)))));
      this.label7.Location = new System.Drawing.Point(19, 63);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(60, 15);
      this.label7.TabIndex = 59;
      this.label7.Text = "Password:";
      // 
      // txtPwdConfirm
      // 
      this.txtPwdConfirm.Location = new System.Drawing.Point(139, 91);
      this.txtPwdConfirm.Name = "txtPwdConfirm";
      this.txtPwdConfirm.PasswordChar = '*';
      this.txtPwdConfirm.Size = new System.Drawing.Size(100, 23);
      this.txtPwdConfirm.TabIndex = 69;
      this.txtPwdConfirm.UseSystemPasswordChar = true;
      // 
      // txtPwd
      // 
      this.txtPwd.Location = new System.Drawing.Point(139, 60);
      this.txtPwd.Name = "txtPwd";
      this.txtPwd.PasswordChar = '*';
      this.txtPwd.Size = new System.Drawing.Size(100, 23);
      this.txtPwd.TabIndex = 68;
      this.txtPwd.UseSystemPasswordChar = true;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(19, 125);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(58, 15);
      this.label2.TabIndex = 64;
      this.label2.Text = "Question:";
      // 
      // txtQuestion
      // 
      this.txtQuestion.Location = new System.Drawing.Point(139, 122);
      this.txtQuestion.Name = "txtQuestion";
      this.txtQuestion.Size = new System.Drawing.Size(385, 23);
      this.txtQuestion.TabIndex = 70;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(19, 156);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(49, 15);
      this.label3.TabIndex = 66;
      this.label3.Text = "Answer:";
      // 
      // txtAnswer
      // 
      this.txtAnswer.Location = new System.Drawing.Point(139, 153);
      this.txtAnswer.Name = "txtAnswer";
      this.txtAnswer.Size = new System.Drawing.Size(385, 23);
      this.txtAnswer.TabIndex = 71;
      // 
      // groupBox2
      // 
      this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBox2.Controls.Add(this.txtAnswer);
      this.groupBox2.Controls.Add(this.label3);
      this.groupBox2.Controls.Add(this.txtQuestion);
      this.groupBox2.Controls.Add(this.label2);
      this.groupBox2.Controls.Add(this.txtUserName);
      this.groupBox2.Controls.Add(this.label1);
      this.groupBox2.Controls.Add(this.createAdministratorUserCheck);
      this.groupBox2.Controls.Add(this.label8);
      this.groupBox2.Controls.Add(this.label7);
      this.groupBox2.Controls.Add(this.txtPwdConfirm);
      this.groupBox2.Controls.Add(this.txtPwd);
      this.groupBox2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBox2.Location = new System.Drawing.Point(25, 134);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(539, 213);
      this.groupBox2.TabIndex = 68;
      this.groupBox2.TabStop = false;
      // 
      // ProviderConfiguration
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.BackColor = System.Drawing.SystemColors.Control;
      this.Controls.Add(this.groupBox2);
      this.Controls.Add(this.groupBox1);
      this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Name = "ProviderConfiguration";
      this.Size = new System.Drawing.Size(584, 380);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ErrorProvider errorProvider1;
    private System.Windows.Forms.TextBox txtUserName;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.CheckBox createAdministratorUserCheck;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.TextBox txtPwdConfirm;
    private System.Windows.Forms.TextBox txtPwd;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.TextBox txtMinimumPasswordLenght;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.CheckBox chkQuestionAndAnswerRequired;
    private System.Windows.Forms.CheckBox chkWriteExceptions;
    private System.Windows.Forms.TextBox txtQuestion;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox txtAnswer;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.GroupBox groupBox2;
  }
}
