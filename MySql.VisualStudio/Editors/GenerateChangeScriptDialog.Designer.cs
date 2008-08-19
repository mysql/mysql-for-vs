namespace MySql.Data.VisualStudio.Editors
{
    partial class GenerateChangeScriptDialog
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
            this.label1 = new System.Windows.Forms.Label();
            this.sqlBox = new System.Windows.Forms.TextBox();
            this.noButton = new System.Windows.Forms.Button();
            this.yesButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Current changes";
            // 
            // sqlBox
            // 
            this.sqlBox.Location = new System.Drawing.Point(15, 36);
            this.sqlBox.Multiline = true;
            this.sqlBox.Name = "sqlBox";
            this.sqlBox.ReadOnly = true;
            this.sqlBox.Size = new System.Drawing.Size(495, 254);
            this.sqlBox.TabIndex = 1;
            // 
            // noButton
            // 
            this.noButton.Location = new System.Drawing.Point(435, 308);
            this.noButton.Name = "noButton";
            this.noButton.Size = new System.Drawing.Size(75, 23);
            this.noButton.TabIndex = 2;
            this.noButton.Text = "No";
            this.noButton.UseVisualStyleBackColor = true;
            this.noButton.Click += new System.EventHandler(this.noButton_Click);
            // 
            // yesButton
            // 
            this.yesButton.Location = new System.Drawing.Point(354, 308);
            this.yesButton.Name = "yesButton";
            this.yesButton.Size = new System.Drawing.Size(75, 23);
            this.yesButton.TabIndex = 3;
            this.yesButton.Text = "Yes";
            this.yesButton.UseVisualStyleBackColor = true;
            this.yesButton.Click += new System.EventHandler(this.yesButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(102, 313);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(221, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Do you want to save these changes to a file?";
            // 
            // GenerateChangeScriptDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(522, 343);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.yesButton);
            this.Controls.Add(this.noButton);
            this.Controls.Add(this.sqlBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GenerateChangeScriptDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Save Change Script";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox sqlBox;
        private System.Windows.Forms.Button noButton;
        private System.Windows.Forms.Button yesButton;
        private System.Windows.Forms.Label label2;
    }
}