namespace MySql.Data.VisualStudio
{
  partial class MySqlScriptDialog
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
      this.txtScript = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // txtScript
      // 
      this.txtScript.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtScript.BackColor = System.Drawing.SystemColors.ControlLightLight;
      this.txtScript.Location = new System.Drawing.Point(4, 5);
      this.txtScript.Multiline = true;
      this.txtScript.Name = "txtScript";
      this.txtScript.ReadOnly = true;
      this.txtScript.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.txtScript.Size = new System.Drawing.Size(437, 295);
      this.txtScript.TabIndex = 0;
      // 
      // MySqlScriptDialog
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(435, 286);
      this.Controls.Add(this.txtScript);
      this.Name = "MySqlScriptDialog";
      this.Text = "Script Generated";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox txtScript;
  }
}