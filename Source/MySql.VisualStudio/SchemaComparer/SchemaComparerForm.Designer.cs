namespace MySql.Data.VisualStudio.SchemaComparer
{
  partial class SchemaComparerForm
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
      this.dgDiffSummary = new System.Windows.Forms.DataGridView();
      this.txtLeft = new System.Windows.Forms.TextBox();
      this.txtRight = new System.Windows.Forms.TextBox();
      this.btnGetLeftChange = new System.Windows.Forms.Button();
      this.btnGetAllLeftChanges = new System.Windows.Forms.Button();
      this.btnGetRightChange = new System.Windows.Forms.Button();
      this.btnGetAllRightChanges = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.dgDiffSummary)).BeginInit();
      this.SuspendLayout();
      // 
      // dgDiffSummary
      // 
      this.dgDiffSummary.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgDiffSummary.Location = new System.Drawing.Point(241, 45);
      this.dgDiffSummary.Name = "dgDiffSummary";
      this.dgDiffSummary.Size = new System.Drawing.Size(388, 313);
      this.dgDiffSummary.TabIndex = 0;
      // 
      // txtLeft
      // 
      this.txtLeft.Location = new System.Drawing.Point(12, 45);
      this.txtLeft.Multiline = true;
      this.txtLeft.Name = "txtLeft";
      this.txtLeft.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.txtLeft.Size = new System.Drawing.Size(223, 313);
      this.txtLeft.TabIndex = 1;
      // 
      // txtRight
      // 
      this.txtRight.Location = new System.Drawing.Point(635, 45);
      this.txtRight.Multiline = true;
      this.txtRight.Name = "txtRight";
      this.txtRight.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.txtRight.Size = new System.Drawing.Size(223, 313);
      this.txtRight.TabIndex = 2;
      // 
      // btnGetLeftChange
      // 
      this.btnGetLeftChange.Location = new System.Drawing.Point(12, 383);
      this.btnGetLeftChange.Name = "btnGetLeftChange";
      this.btnGetLeftChange.Size = new System.Drawing.Size(123, 23);
      this.btnGetLeftChange.TabIndex = 3;
      this.btnGetLeftChange.Text = "Get Left Script";
      this.btnGetLeftChange.UseVisualStyleBackColor = true;
      this.btnGetLeftChange.Visible = false;
      this.btnGetLeftChange.Click += new System.EventHandler(this.btnGetLeftChange_Click);
      // 
      // btnGetAllLeftChanges
      // 
      this.btnGetAllLeftChanges.Location = new System.Drawing.Point(12, 412);
      this.btnGetAllLeftChanges.Name = "btnGetAllLeftChanges";
      this.btnGetAllLeftChanges.Size = new System.Drawing.Size(123, 23);
      this.btnGetAllLeftChanges.TabIndex = 4;
      this.btnGetAllLeftChanges.Text = "Get All Left Changes";
      this.btnGetAllLeftChanges.UseVisualStyleBackColor = true;
      this.btnGetAllLeftChanges.Click += new System.EventHandler(this.btnGetAllLeftChanges_Click);
      // 
      // btnGetRightChange
      // 
      this.btnGetRightChange.Location = new System.Drawing.Point(735, 383);
      this.btnGetRightChange.Name = "btnGetRightChange";
      this.btnGetRightChange.Size = new System.Drawing.Size(123, 23);
      this.btnGetRightChange.TabIndex = 5;
      this.btnGetRightChange.Text = "Get Right Change";
      this.btnGetRightChange.UseVisualStyleBackColor = true;
      this.btnGetRightChange.Visible = false;
      this.btnGetRightChange.Click += new System.EventHandler(this.btnGetRightChange_Click);
      // 
      // btnGetAllRightChanges
      // 
      this.btnGetAllRightChanges.Location = new System.Drawing.Point(735, 412);
      this.btnGetAllRightChanges.Name = "btnGetAllRightChanges";
      this.btnGetAllRightChanges.Size = new System.Drawing.Size(123, 23);
      this.btnGetAllRightChanges.TabIndex = 6;
      this.btnGetAllRightChanges.Text = "Get All Right Changes";
      this.btnGetAllRightChanges.UseVisualStyleBackColor = true;
      this.btnGetAllRightChanges.Click += new System.EventHandler(this.btnGetAllRightChanges_Click);
      // 
      // SchemaComparerForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(870, 448);
      this.Controls.Add(this.btnGetAllRightChanges);
      this.Controls.Add(this.btnGetRightChange);
      this.Controls.Add(this.btnGetAllLeftChanges);
      this.Controls.Add(this.btnGetLeftChange);
      this.Controls.Add(this.txtRight);
      this.Controls.Add(this.txtLeft);
      this.Controls.Add(this.dgDiffSummary);
      this.Name = "SchemaComparerForm";
      this.Text = "MySql Schema Comparer";
      this.Load += new System.EventHandler(this.SchemaComparerForm_Load);
      ((System.ComponentModel.ISupportInitialize)(this.dgDiffSummary)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.DataGridView dgDiffSummary;
    private System.Windows.Forms.TextBox txtLeft;
    private System.Windows.Forms.TextBox txtRight;
    private System.Windows.Forms.Button btnGetLeftChange;
    private System.Windows.Forms.Button btnGetAllLeftChanges;
    private System.Windows.Forms.Button btnGetRightChange;
    private System.Windows.Forms.Button btnGetAllRightChanges;
  }
}