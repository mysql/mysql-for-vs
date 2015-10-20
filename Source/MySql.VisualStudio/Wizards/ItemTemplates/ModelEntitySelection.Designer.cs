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
      this.pnlEntityContainer = new System.Windows.Forms.Panel();
      this.comboEntities = new System.Windows.Forms.ComboBox();
      this.lblText = new System.Windows.Forms.Label();
      this.pnlModelContainer = new System.Windows.Forms.Panel();
      this.comboModelsList = new System.Windows.Forms.ComboBox();
      this.label3 = new System.Windows.Forms.Label();
      this.pnlEntityContainer.SuspendLayout();
      this.pnlModelContainer.SuspendLayout();
      this.SuspendLayout();
      // 
      // pnlEntityContainer
      // 
      this.pnlEntityContainer.Controls.Add(this.comboEntities);
      this.pnlEntityContainer.Controls.Add(this.lblText);
      this.pnlEntityContainer.Enabled = false;
      this.pnlEntityContainer.Location = new System.Drawing.Point(3, 66);
      this.pnlEntityContainer.Name = "pnlEntityContainer";
      this.pnlEntityContainer.Size = new System.Drawing.Size(310, 55);
      this.pnlEntityContainer.TabIndex = 94;
      // 
      // comboEntities
      // 
      this.comboEntities.Enabled = false;
      this.comboEntities.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.comboEntities.FormattingEnabled = true;
      this.comboEntities.ItemHeight = 13;
      this.comboEntities.Location = new System.Drawing.Point(6, 26);
      this.comboEntities.Name = "comboEntities";
      this.comboEntities.Size = new System.Drawing.Size(285, 21);
      this.comboEntities.TabIndex = 95;
      // 
      // lblText
      // 
      this.lblText.AutoSize = true;
      this.lblText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(242)))));
      this.lblText.Location = new System.Drawing.Point(3, 10);
      this.lblText.Name = "lblText";
      this.lblText.Size = new System.Drawing.Size(187, 13);
      this.lblText.TabIndex = 94;
      this.lblText.Text = "Select the entity to create the form:";
      // 
      // pnlModelContainer
      // 
      this.pnlModelContainer.Controls.Add(this.comboModelsList);
      this.pnlModelContainer.Controls.Add(this.label3);
      this.pnlModelContainer.Location = new System.Drawing.Point(3, 3);
      this.pnlModelContainer.Name = "pnlModelContainer";
      this.pnlModelContainer.Size = new System.Drawing.Size(480, 57);
      this.pnlModelContainer.TabIndex = 96;
      // 
      // comboModelsList
      // 
      this.comboModelsList.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.comboModelsList.FormattingEnabled = true;
      this.comboModelsList.Location = new System.Drawing.Point(6, 27);
      this.comboModelsList.Name = "comboModelsList";
      this.comboModelsList.Size = new System.Drawing.Size(285, 21);
      this.comboModelsList.TabIndex = 34;
      // 
      // label3
      // 
      this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(242)))));
      this.label3.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.Location = new System.Drawing.Point(3, 9);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(473, 15);
      this.label3.TabIndex = 33;
      this.label3.Text = "From the dropdown list below, select the MySql model you want to use to create th" +
    "e form:\r\n";
      // 
      // ModelEntitySelection
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.pnlModelContainer);
      this.Controls.Add(this.pnlEntityContainer);
      this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
      this.Name = "ModelEntitySelection";
      this.Size = new System.Drawing.Size(489, 128);
      this.pnlEntityContainer.ResumeLayout(false);
      this.pnlEntityContainer.PerformLayout();
      this.pnlModelContainer.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel pnlModelContainer;
    private System.Windows.Forms.Panel pnlEntityContainer;
    private System.Windows.Forms.ComboBox comboEntities;
    private System.Windows.Forms.Label lblText;
    private System.Windows.Forms.ComboBox comboModelsList;
    private System.Windows.Forms.Label label3;




  }
}
