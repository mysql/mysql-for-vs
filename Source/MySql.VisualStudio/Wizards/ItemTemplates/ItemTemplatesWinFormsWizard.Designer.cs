using EnvDTE;
namespace MySql.Data.VisualStudio.Wizards.ItemTemplates
{
	partial class ItemTemplatesWinFormsWizard
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ItemTemplatesWinFormsWizard));
      this.btnFinish = new System.Windows.Forms.Button();
      this.btnCancel = new System.Windows.Forms.Button();
      this.lblDescription = new System.Windows.Forms.Label();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.panel1 = new System.Windows.Forms.Panel();
      this.modelEntitySelection_WinForms1 = new MySql.Data.VisualStudio.Wizards.ItemTemplates.ModelEntitySelection_WinForms();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // btnFinish
      // 
      this.btnFinish.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnFinish.Enabled = false;
      this.btnFinish.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnFinish.Location = new System.Drawing.Point(592, 394);
      this.btnFinish.Name = "btnFinish";
      this.btnFinish.Size = new System.Drawing.Size(75, 23);
      this.btnFinish.TabIndex = 11;
      this.btnFinish.Text = "Finish";
      this.btnFinish.UseVisualStyleBackColor = true;
      this.btnFinish.Click += new System.EventHandler(this.btnFinish_Click);
      // 
      // btnCancel
      // 
      this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnCancel.CausesValidation = false;
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnCancel.Location = new System.Drawing.Point(673, 394);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(75, 23);
      this.btnCancel.TabIndex = 10;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      // 
      // lblDescription
      // 
      this.lblDescription.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblDescription.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
      this.lblDescription.Location = new System.Drawing.Point(10, 116);
      this.lblDescription.Name = "lblDescription";
      this.lblDescription.Size = new System.Drawing.Size(219, 69);
      this.lblDescription.TabIndex = 2;
      this.lblDescription.Text = "This form will allow you to create a Windows Forms item connected to an existing " +
    "MySQL database.";
      // 
      // pictureBox1
      // 
      this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
      this.pictureBox1.Location = new System.Drawing.Point(13, 13);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(216, 88);
      this.pictureBox1.TabIndex = 0;
      this.pictureBox1.TabStop = false;
      // 
      // panel1
      // 
      this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(41)))), ((int)(((byte)(41)))));
      this.panel1.Controls.Add(this.lblDescription);
      this.panel1.Controls.Add(this.pictureBox1);
      this.panel1.Location = new System.Drawing.Point(-1, -1);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(245, 438);
      this.panel1.TabIndex = 18;
      // 
      // modelEntitySelection_WinForms1
      // 
      this.modelEntitySelection_WinForms1.Cursor = System.Windows.Forms.Cursors.Default;
      this.modelEntitySelection_WinForms1.Font = new System.Drawing.Font("Segoe UI", 8.25F);
      this.modelEntitySelection_WinForms1.Location = new System.Drawing.Point(250, 12);
      this.modelEntitySelection_WinForms1.Name = "modelEntitySelection_WinForms1";
      this.modelEntitySelection_WinForms1.Size = new System.Drawing.Size(495, 351);
      this.modelEntitySelection_WinForms1.TabIndex = 24;
      // 
      // ItemTemplatesWinFormsWizard
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnCancel;
      this.ClientSize = new System.Drawing.Size(760, 429);
      this.Controls.Add(this.modelEntitySelection_WinForms1);
      this.Controls.Add(this.panel1);
      this.Controls.Add(this.btnFinish);
      this.Controls.Add(this.btnCancel);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ItemTemplatesWinFormsWizard";
      this.Text = "Windows Forms Item Template";
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.panel1.ResumeLayout(false);
      this.ResumeLayout(false);

		}

		#endregion

		internal System.Windows.Forms.Label lblDescription;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Panel panel1;
		private ModelEntitySelection_WinForms modelEntitySelection_WinForms1;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnFinish;
	}
}