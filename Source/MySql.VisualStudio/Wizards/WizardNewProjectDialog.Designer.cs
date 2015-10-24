namespace MySql.Data.VisualStudio.Wizards
{
  partial class WizardNewProjectDialog
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
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizardNewProjectDialog));
      this.btnOK = new System.Windows.Forms.Button();
      this.btnCancel = new System.Windows.Forms.Button();
      this.lblWizardName = new System.Windows.Forms.Label();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.lblStepTitle = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.panel1 = new System.Windows.Forms.Panel();
      this.lblProjectName = new System.Windows.Forms.Label();
      this.lblProjectPath = new System.Windows.Forms.Label();
      this.txtProjectName = new System.Windows.Forms.TextBox();
      this.txtProjectPath = new System.Windows.Forms.TextBox();
      this.btnBrowse = new System.Windows.Forms.Button();
      this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
      this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
      this.label1 = new System.Windows.Forms.Label();
      this.solutionNameTextBox = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.solutionOptions = new System.Windows.Forms.ComboBox();
      this.imageList1 = new System.Windows.Forms.ImageList(this.components);
      this.projectTypesList = new System.Windows.Forms.ListView();
      this.lblProjectDescription = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.languageLbl = new System.Windows.Forms.Label();
      this.createDirectoryForSolutionChk = new System.Windows.Forms.CheckBox();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.panel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
      this.SuspendLayout();
      // 
      // btnOK
      // 
      this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.btnOK.Font = new System.Drawing.Font("Segoe UI", 9F);
      this.btnOK.Location = new System.Drawing.Point(786, 475);
      this.btnOK.Name = "btnOK";
      this.btnOK.Size = new System.Drawing.Size(75, 23);
      this.btnOK.TabIndex = 8;
      this.btnOK.Text = "OK";
      this.btnOK.UseVisualStyleBackColor = true;
      this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
      // 
      // btnCancel
      // 
      this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnCancel.CausesValidation = false;
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F);
      this.btnCancel.Location = new System.Drawing.Point(705, 475);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(75, 23);
      this.btnCancel.TabIndex = 9;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      // 
      // lblWizardName
      // 
      this.lblWizardName.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblWizardName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
      this.lblWizardName.Location = new System.Drawing.Point(20, 119);
      this.lblWizardName.Name = "lblWizardName";
      this.lblWizardName.Size = new System.Drawing.Size(213, 116);
      this.lblWizardName.TabIndex = 1;
      this.lblWizardName.Text = "Select the type of MySQL Project you would like to create.";
      // 
      // pictureBox1
      // 
      this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
      this.pictureBox1.Location = new System.Drawing.Point(19, 23);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(214, 88);
      this.pictureBox1.TabIndex = 0;
      this.pictureBox1.TabStop = false;
      // 
      // lblStepTitle
      // 
      this.lblStepTitle.AutoSize = true;
      this.lblStepTitle.BackColor = System.Drawing.Color.White;
      this.lblStepTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblStepTitle.Location = new System.Drawing.Point(275, 26);
      this.lblStepTitle.Name = "lblStepTitle";
      this.lblStepTitle.Size = new System.Drawing.Size(234, 25);
      this.lblStepTitle.TabIndex = 20;
      this.lblStepTitle.Text = "Create new MySQL project";
      // 
      // label2
      // 
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.label2.BackColor = System.Drawing.Color.White;
      this.label2.Location = new System.Drawing.Point(254, 2);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(624, 80);
      this.label2.TabIndex = 19;
      // 
      // panel1
      // 
      this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(41)))), ((int)(((byte)(41)))));
      this.panel1.Controls.Add(this.lblWizardName);
      this.panel1.Controls.Add(this.pictureBox1);
      this.panel1.Location = new System.Drawing.Point(-1, -1);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(255, 542);
      this.panel1.TabIndex = 18;
      // 
      // lblProjectName
      // 
      this.lblProjectName.AutoSize = true;
      this.lblProjectName.Font = new System.Drawing.Font("Segoe UI", 9F);
      this.lblProjectName.Location = new System.Drawing.Point(277, 308);
      this.lblProjectName.Name = "lblProjectName";
      this.lblProjectName.Size = new System.Drawing.Size(42, 15);
      this.lblProjectName.TabIndex = 22;
      this.lblProjectName.Text = "Name:";
      // 
      // lblProjectPath
      // 
      this.lblProjectPath.AutoSize = true;
      this.lblProjectPath.Font = new System.Drawing.Font("Segoe UI", 9F);
      this.lblProjectPath.Location = new System.Drawing.Point(277, 339);
      this.lblProjectPath.Name = "lblProjectPath";
      this.lblProjectPath.Size = new System.Drawing.Size(56, 15);
      this.lblProjectPath.TabIndex = 23;
      this.lblProjectPath.Text = "Location:";
      // 
      // txtProjectName
      // 
      this.txtProjectName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtProjectName.Location = new System.Drawing.Point(383, 305);
      this.txtProjectName.Name = "txtProjectName";
      this.txtProjectName.Size = new System.Drawing.Size(463, 22);
      this.txtProjectName.TabIndex = 2;
      this.txtProjectName.Leave += new System.EventHandler(this.txtProjectName_Leave);
      // 
      // txtProjectPath
      // 
      this.txtProjectPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtProjectPath.Location = new System.Drawing.Point(383, 336);
      this.txtProjectPath.Name = "txtProjectPath";
      this.txtProjectPath.Size = new System.Drawing.Size(382, 22);
      this.txtProjectPath.TabIndex = 3;
      this.txtProjectPath.Validating += new System.ComponentModel.CancelEventHandler(this.txtProjectPath_Validating);
      // 
      // btnBrowse
      // 
      this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnBrowse.Font = new System.Drawing.Font("Segoe UI", 9F);
      this.btnBrowse.Location = new System.Drawing.Point(771, 335);
      this.btnBrowse.Name = "btnBrowse";
      this.btnBrowse.Size = new System.Drawing.Size(75, 23);
      this.btnBrowse.TabIndex = 4;
      this.btnBrowse.Text = "Browse...";
      this.btnBrowse.UseVisualStyleBackColor = true;
      this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
      // 
      // errorProvider1
      // 
      this.errorProvider1.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
      this.errorProvider1.ContainerControl = this;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Segoe UI", 9F);
      this.label1.Location = new System.Drawing.Point(277, 370);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(54, 15);
      this.label1.TabIndex = 33;
      this.label1.Text = "Solution:";
      // 
      // solutionNameTextBox
      // 
      this.solutionNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.solutionNameTextBox.Location = new System.Drawing.Point(383, 397);
      this.solutionNameTextBox.Name = "solutionNameTextBox";
      this.solutionNameTextBox.Size = new System.Drawing.Size(463, 22);
      this.solutionNameTextBox.TabIndex = 6;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Segoe UI", 9F);
      this.label3.Location = new System.Drawing.Point(277, 401);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(89, 15);
      this.label3.TabIndex = 35;
      this.label3.Text = "Solution Name:";
      // 
      // solutionOptions
      // 
      this.solutionOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.solutionOptions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.solutionOptions.FormattingEnabled = true;
      this.solutionOptions.Items.AddRange(new object[] {
            "Create new solution",
            "Add to solution"});
      this.solutionOptions.Location = new System.Drawing.Point(383, 367);
      this.solutionOptions.Name = "solutionOptions";
      this.solutionOptions.Size = new System.Drawing.Size(463, 21);
      this.solutionOptions.TabIndex = 5;
      // 
      // imageList1
      // 
      this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
      this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
      this.imageList1.Images.SetKeyName(0, "MySQL.png");
      // 
      // projectTypesList
      // 
      this.projectTypesList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.projectTypesList.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.projectTypesList.HideSelection = false;
      this.projectTypesList.Location = new System.Drawing.Point(280, 98);
      this.projectTypesList.MultiSelect = false;
      this.projectTypesList.Name = "projectTypesList";
      this.projectTypesList.Scrollable = false;
      this.projectTypesList.Size = new System.Drawing.Size(382, 181);
      this.projectTypesList.TabIndex = 1;
      this.projectTypesList.UseCompatibleStateImageBehavior = false;
      // 
      // lblProjectDescription
      // 
      this.lblProjectDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.lblProjectDescription.BackColor = System.Drawing.Color.Transparent;
      this.lblProjectDescription.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblProjectDescription.ForeColor = System.Drawing.Color.Black;
      this.lblProjectDescription.Location = new System.Drawing.Point(668, 143);
      this.lblProjectDescription.Name = "lblProjectDescription";
      this.lblProjectDescription.Padding = new System.Windows.Forms.Padding(6);
      this.lblProjectDescription.Size = new System.Drawing.Size(178, 136);
      this.lblProjectDescription.TabIndex = 39;
      this.lblProjectDescription.Text = "This wizard will create a full MVC project connected to a MySQL database existing" +
    " or will create a new one with a web site that includes user authentication with" +
    " the ASP.NET MySQL Membership provider.";
      // 
      // label5
      // 
      this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.label5.BackColor = System.Drawing.Color.Transparent;
      this.label5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label5.ForeColor = System.Drawing.Color.Black;
      this.label5.Location = new System.Drawing.Point(668, 115);
      this.label5.Name = "label5";
      this.label5.Padding = new System.Windows.Forms.Padding(6);
      this.label5.Size = new System.Drawing.Size(57, 27);
      this.label5.TabIndex = 40;
      this.label5.Text = "Type: ";
      // 
      // languageLbl
      // 
      this.languageLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.languageLbl.BackColor = System.Drawing.Color.Transparent;
      this.languageLbl.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.languageLbl.ForeColor = System.Drawing.Color.Black;
      this.languageLbl.Location = new System.Drawing.Point(731, 115);
      this.languageLbl.Name = "languageLbl";
      this.languageLbl.Padding = new System.Windows.Forms.Padding(6);
      this.languageLbl.Size = new System.Drawing.Size(105, 27);
      this.languageLbl.TabIndex = 41;
      this.languageLbl.Text = "Visual C#";
      // 
      // createDirectoryForSolutionChk
      // 
      this.createDirectoryForSolutionChk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.createDirectoryForSolutionChk.AutoSize = true;
      this.createDirectoryForSolutionChk.Location = new System.Drawing.Point(675, 425);
      this.createDirectoryForSolutionChk.Name = "createDirectoryForSolutionChk";
      this.createDirectoryForSolutionChk.Size = new System.Drawing.Size(171, 17);
      this.createDirectoryForSolutionChk.TabIndex = 7;
      this.createDirectoryForSolutionChk.Text = "Create directory for solution";
      this.createDirectoryForSolutionChk.UseVisualStyleBackColor = true;
      // 
      // WizardNewProjectDialog
      // 
      this.AcceptButton = this.btnOK;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnCancel;
      this.ClientSize = new System.Drawing.Size(873, 510);
      this.Controls.Add(this.createDirectoryForSolutionChk);
      this.Controls.Add(this.languageLbl);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.lblProjectDescription);
      this.Controls.Add(this.projectTypesList);
      this.Controls.Add(this.solutionOptions);
      this.Controls.Add(this.solutionNameTextBox);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.btnBrowse);
      this.Controls.Add(this.txtProjectPath);
      this.Controls.Add(this.txtProjectName);
      this.Controls.Add(this.lblProjectPath);
      this.Controls.Add(this.lblProjectName);
      this.Controls.Add(this.lblStepTitle);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.panel1);
      this.Controls.Add(this.btnOK);
      this.Controls.Add(this.btnCancel);
      this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "WizardNewProjectDialog";
      this.Text = "New Project";
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.panel1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    internal System.Windows.Forms.Button btnOK;
    protected System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Label lblWizardName;
    private System.Windows.Forms.PictureBox pictureBox1;
    internal System.Windows.Forms.Label lblStepTitle;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Label lblProjectName;
    private System.Windows.Forms.Label lblProjectPath;
    internal System.Windows.Forms.TextBox txtProjectName;
    internal System.Windows.Forms.TextBox txtProjectPath;
    private System.Windows.Forms.Button btnBrowse;
    private System.Windows.Forms.ErrorProvider errorProvider1;
    private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    private System.Windows.Forms.ComboBox solutionOptions;
    private System.Windows.Forms.TextBox solutionNameTextBox;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ImageList imageList1;
    private System.Windows.Forms.ListView projectTypesList;
    internal System.Windows.Forms.Label lblProjectDescription;
    private System.Windows.Forms.Label languageLbl;
    internal System.Windows.Forms.Label label5;
    private System.Windows.Forms.CheckBox createDirectoryForSolutionChk;
  }
}
