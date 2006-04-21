namespace MySql.VSTools
{
    partial class QueryControl
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
            this.inputBox = new System.Windows.Forms.RichTextBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.resultsGrid = new System.Windows.Forms.DataGridView();
            this.resultsText = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.resultsGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // inputBox
            // 
            this.inputBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.inputBox.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.inputBox.Location = new System.Drawing.Point(5, 5);
            this.inputBox.Name = "inputBox";
            this.inputBox.Size = new System.Drawing.Size(356, 96);
            this.inputBox.TabIndex = 0;
            this.inputBox.Text = "";
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(5, 101);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(356, 3);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // resultsGrid
            // 
            this.resultsGrid.AllowUserToOrderColumns = true;
            this.resultsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.resultsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resultsGrid.Location = new System.Drawing.Point(5, 104);
            this.resultsGrid.Name = "resultsGrid";
            this.resultsGrid.Size = new System.Drawing.Size(356, 219);
            this.resultsGrid.TabIndex = 2;
            // 
            // resultsText
            // 
            this.resultsText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resultsText.Location = new System.Drawing.Point(5, 5);
            this.resultsText.Multiline = true;
            this.resultsText.Name = "resultsText";
            this.resultsText.Size = new System.Drawing.Size(356, 318);
            this.resultsText.TabIndex = 3;
            this.resultsText.Visible = false;
            // 
            // QueryControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.resultsGrid);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.inputBox);
            this.Controls.Add(this.resultsText);
            this.Name = "QueryControl";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Size = new System.Drawing.Size(366, 328);
            ((System.ComponentModel.ISupportInitialize)(this.resultsGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox inputBox;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.DataGridView resultsGrid;
        private System.Windows.Forms.TextBox resultsText;
    }
}
