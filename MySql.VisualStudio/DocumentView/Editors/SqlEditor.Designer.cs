namespace MySql.Data.VisualStudio.DocumentView
{
    partial class SqlEditor
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
            this.txtSqlSource = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtSqlSource
            // 
            this.txtSqlSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSqlSource.Location = new System.Drawing.Point(0, 0);
            this.txtSqlSource.Multiline = true;
            this.txtSqlSource.Name = "txtSqlSource";
            this.txtSqlSource.Size = new System.Drawing.Size(150, 100);
            this.txtSqlSource.TabIndex = 0;
            this.txtSqlSource.TextChanged += new System.EventHandler(this.txtSqlSource_TextChanged);
            // 
            // SqlEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtSqlSource);
            this.Name = "SqlEditor";
            this.Size = new System.Drawing.Size(150, 100);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtSqlSource;
    }
}
