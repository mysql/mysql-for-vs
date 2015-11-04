namespace MySql.Data.VisualStudio.Wizards.ItemTemplates
{
  partial class ModelEntitySelection
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
      this.comboModelsList = new System.Windows.Forms.ComboBox();
      this.label3 = new System.Windows.Forms.Label();
      this.comboEntities = new System.Windows.Forms.ComboBox();
      this.lblText = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // comboModelsList
      // 
      this.comboModelsList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.comboModelsList.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.comboModelsList.FormattingEnabled = true;
      this.comboModelsList.Location = new System.Drawing.Point(9, 31);
      this.comboModelsList.Name = "comboModelsList";
      this.comboModelsList.Size = new System.Drawing.Size(297, 23);
      this.comboModelsList.TabIndex = 96;
      // 
      // label3
      // 
      this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(242)))));
      this.label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.Location = new System.Drawing.Point(6, 9);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(485, 15);
      this.label3.TabIndex = 95;
      this.label3.Text = "From the dropdown list below, select the MySql model you want to use to create th" +
    "e form:\r\n";
      // 
      // comboEntities
      // 
      this.comboEntities.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.comboEntities.Enabled = false;
      this.comboEntities.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.comboEntities.FormattingEnabled = true;
      this.comboEntities.ItemHeight = 15;
      this.comboEntities.Location = new System.Drawing.Point(9, 92);
      this.comboEntities.Name = "comboEntities";
      this.comboEntities.Size = new System.Drawing.Size(297, 23);
      this.comboEntities.TabIndex = 98;
      // 
      // lblText
      // 
      this.lblText.AutoSize = true;
      this.lblText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(242)))));
      this.lblText.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblText.Location = new System.Drawing.Point(6, 69);
      this.lblText.Name = "lblText";
      this.lblText.Size = new System.Drawing.Size(192, 15);
      this.lblText.TabIndex = 97;
      this.lblText.Text = "Select the entity to create the form:";
      // 
      // ModelEntitySelection
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.comboEntities);
      this.Controls.Add(this.lblText);
      this.Controls.Add(this.comboModelsList);
      this.Controls.Add(this.label3);
      this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
      this.Name = "ModelEntitySelection";
      this.Size = new System.Drawing.Size(501, 132);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ComboBox comboModelsList;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.ComboBox comboEntities;
    private System.Windows.Forms.Label lblText;





  }
}
