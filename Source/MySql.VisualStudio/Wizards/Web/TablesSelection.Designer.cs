namespace MySql.Data.VisualStudio.Wizards.Web
{
  partial class TablesSelection
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
      this.chkSelectAllTables = new System.Windows.Forms.CheckBox();
      this.listTables = new System.Windows.Forms.DataGridView();
      this.label5 = new System.Windows.Forms.Label();
      this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.listTables)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
      this.SuspendLayout();
      // 
      // chkSelectAllTables
      // 
      this.chkSelectAllTables.AutoSize = true;
      this.chkSelectAllTables.Location = new System.Drawing.Point(452, 331);
      this.chkSelectAllTables.Name = "chkSelectAllTables";
      this.chkSelectAllTables.Size = new System.Drawing.Size(105, 17);
      this.chkSelectAllTables.TabIndex = 66;
      this.chkSelectAllTables.Text = "Select all tables";
      this.chkSelectAllTables.UseVisualStyleBackColor = true;
      // 
      // listTables
      // 
      this.listTables.AllowUserToAddRows = false;
      this.listTables.BackgroundColor = System.Drawing.Color.White;
      this.listTables.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.listTables.Location = new System.Drawing.Point(17, 51);
      this.listTables.Name = "listTables";
      this.listTables.RowHeadersVisible = false;
      this.listTables.Size = new System.Drawing.Size(540, 254);
      this.listTables.TabIndex = 65;
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(242)))));
      this.label5.Location = new System.Drawing.Point(17, 26);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(177, 13);
      this.label5.TabIndex = 64;
      this.label5.Text = "Select tables to include in model:";
      // 
      // errorProvider1
      // 
      this.errorProvider1.ContainerControl = this;
      // 
      // TablesSelection
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.chkSelectAllTables);
      this.Controls.Add(this.listTables);
      this.Controls.Add(this.label5);
      this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
      this.Name = "TablesSelection";
      this.Size = new System.Drawing.Size(584, 380);
      ((System.ComponentModel.ISupportInitialize)(this.listTables)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.CheckBox chkSelectAllTables;
    private System.Windows.Forms.DataGridView listTables;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.ErrorProvider errorProvider1;
  }
}
