namespace MySql.Data.VisualStudio.Dialogs
{
    partial class SqlPreviewDialog
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SqlPreviewDialog));
            this.label1 = new System.Windows.Forms.Label();
            this.sqlText = new System.Windows.Forms.RichTextBox();
            this.executeButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.iconLabel = new System.Windows.Forms.Label();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // sqlText
            // 
            resources.ApplyResources(this.sqlText, "sqlText");
            this.sqlText.BackColor = System.Drawing.SystemColors.Control;
            this.sqlText.MinimumSize = new System.Drawing.Size(407, 182);
            this.sqlText.Name = "sqlText";
            this.sqlText.ReadOnly = true;
            this.sqlText.Text = global::MySql.Data.VisualStudio.Properties.Resources.Description_Table_DataFree;
            // 
            // executeButton
            // 
            resources.ApplyResources(this.executeButton, "executeButton");
            this.executeButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.executeButton.Name = "executeButton";
            this.executeButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // iconLabel
            // 
            this.iconLabel.ImageList = this.imageList;
            resources.ApplyResources(this.iconLabel, "iconLabel");
            this.iconLabel.Name = "iconLabel";
            // 
            // imageList
            // 
            this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            resources.ApplyResources(this.imageList, "imageList");
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // SqlPreviewDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.iconLabel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.executeButton);
            this.Controls.Add(this.sqlText);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SqlPreviewDialog";
            this.ShowIcon = false;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox sqlText;
        private System.Windows.Forms.Button executeButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label iconLabel;
        private System.Windows.Forms.ImageList imageList;
    }
}