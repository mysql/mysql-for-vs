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
      this.lblText = new System.Windows.Forms.Label();
      this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
      this.txtFilter = new System.Windows.Forms.TextBox();
      this.btnFilter = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.listTables)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
      this.SuspendLayout();
      // 
      // chkSelectAllTables
      // 
      this.chkSelectAllTables.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.chkSelectAllTables.AutoSize = true;
      this.chkSelectAllTables.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.chkSelectAllTables.Location = new System.Drawing.Point(458, 337);
      this.chkSelectAllTables.Name = "chkSelectAllTables";
      this.chkSelectAllTables.Size = new System.Drawing.Size(106, 19);
      this.chkSelectAllTables.TabIndex = 66;
      this.chkSelectAllTables.Text = "Select all tables";
      this.chkSelectAllTables.UseVisualStyleBackColor = true;
      // 
      // listTables
      // 
      this.listTables.AllowUserToAddRows = false;
      this.listTables.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.listTables.BackgroundColor = System.Drawing.Color.White;
      this.listTables.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.listTables.Location = new System.Drawing.Point(18, 72);
      this.listTables.Name = "listTables";
      this.listTables.RowHeadersVisible = false;
      this.listTables.Size = new System.Drawing.Size(546, 259);
      this.listTables.TabIndex = 65;
      // 
      // lblText
      // 
      this.lblText.AutoSize = true;
      this.lblText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(242)))));
      this.lblText.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblText.Location = new System.Drawing.Point(15, 12);
      this.lblText.Name = "lblText";
      this.lblText.Size = new System.Drawing.Size(181, 15);
      this.lblText.TabIndex = 64;
      this.lblText.Text = "Select tables to include in model:";
      // 
      // errorProvider1
      // 
      this.errorProvider1.ContainerControl = this;
      // 
      // txtFilter
      // 
      this.txtFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtFilter.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtFilter.Location = new System.Drawing.Point(66, 39);
      this.txtFilter.Name = "txtFilter";
      this.txtFilter.Size = new System.Drawing.Size(379, 23);
      this.txtFilter.TabIndex = 67;
      this.txtFilter.TextChanged += new System.EventHandler(this.txtFilter_TextChanged);
      // 
      // btnFilter
      // 
      this.btnFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnFilter.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnFilter.Location = new System.Drawing.Point(451, 37);
      this.btnFilter.Name = "btnFilter";
      this.btnFilter.Size = new System.Drawing.Size(113, 24);
      this.btnFilter.TabIndex = 69;
      this.btnFilter.Text = "Filter tables";
      this.btnFilter.UseVisualStyleBackColor = true;
      this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(18, 42);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(42, 15);
      this.label1.TabIndex = 70;
      this.label1.Text = "Name:";
      // 
      // TablesSelection
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.label1);
      this.Controls.Add(this.btnFilter);
      this.Controls.Add(this.txtFilter);
      this.Controls.Add(this.chkSelectAllTables);
      this.Controls.Add(this.listTables);
      this.Controls.Add(this.lblText);
      this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
      this.Name = "TablesSelection";
      this.Size = new System.Drawing.Size(584, 380);
      ((System.ComponentModel.ISupportInitialize)(this.listTables)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label lblText;
    private System.Windows.Forms.ErrorProvider errorProvider1;
    private System.Windows.Forms.Label label1;
    protected System.Windows.Forms.CheckBox chkSelectAllTables;
    protected System.Windows.Forms.DataGridView listTables;
    protected System.Windows.Forms.TextBox txtFilter;
    protected System.Windows.Forms.Button btnFilter;
  }
}
