namespace MySql.Data.VisualStudio.DocumentView
{
    partial class SqlSourceEditor
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
            this.sqlEditor = new MySql.Data.VisualStudio.DocumentView.SqlEditor();
            this.SuspendLayout();
            // 
            // sqlEditor
            // 
            this.sqlEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sqlEditor.Location = new System.Drawing.Point(0, 0);
            this.sqlEditor.Name = "sqlEditor";
            this.sqlEditor.Size = new System.Drawing.Size(400, 100);
            this.sqlEditor.SqlSource = null;
            this.sqlEditor.TabIndex = 0;
            // 
            // ViewEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.sqlEditor);
            this.Name = "ViewEditor";
            this.Size = new System.Drawing.Size(400, 100);
            this.ResumeLayout(false);

        }

        #endregion

        private SqlEditor sqlEditor;
    }
}
