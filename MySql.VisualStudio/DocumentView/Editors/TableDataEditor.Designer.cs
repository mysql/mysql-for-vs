namespace MySql.Data.VisualStudio.DocumentView
{
    partial class TableDataEditor
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
            this.panelToChangeFocus = new System.Windows.Forms.Panel();
            this.dataGridView = new AdvancedDataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // panelToChangeFocus
            // 
            this.panelToChangeFocus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelToChangeFocus.Location = new System.Drawing.Point(0, 150);
            this.panelToChangeFocus.Name = "panelToChangeFocus";
            this.panelToChangeFocus.Size = new System.Drawing.Size(150, 0);
            this.panelToChangeFocus.TabIndex = 0;
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToOrderColumns = true;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(0, 0);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.Size = new System.Drawing.Size(150, 150);
            this.dataGridView.TabIndex = 1;
            // 
            // TableDataEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.panelToChangeFocus);
            this.Name = "TableDataEditor";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelToChangeFocus;
        private AdvancedDataGridView dataGridView;
    }
}
