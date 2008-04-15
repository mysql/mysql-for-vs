namespace MySql.Data.VisualStudio.Dialogs
{
    partial class SqlErrorDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SqlErrorDialog));
            System.Windows.Forms.Label engineStatusesLabel;
            this.errorLabel = new System.Windows.Forms.Label();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.iconLabel = new System.Windows.Forms.Label();
            this.errorText = new System.Windows.Forms.RichTextBox();
            this.statusText = new System.Windows.Forms.RichTextBox();
            this.buttonOK = new System.Windows.Forms.Button();
            engineStatusesLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // errorLabel
            // 
            resources.ApplyResources(this.errorLabel, "errorLabel");
            this.errorLabel.Name = "errorLabel";
            // 
            // imageList
            // 
            this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            resources.ApplyResources(this.imageList, "imageList");
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // iconLabel
            // 
            this.iconLabel.ImageList = this.imageList;
            resources.ApplyResources(this.iconLabel, "iconLabel");
            this.iconLabel.Name = "iconLabel";
            // 
            // errorText
            // 
            resources.ApplyResources(this.errorText, "errorText");
            this.errorText.BackColor = System.Drawing.SystemColors.Control;
            this.errorText.Name = "errorText";
            this.errorText.ReadOnly = true;
            this.errorText.Text = global::MySql.Data.VisualStudio.Properties.Resources.Description_Table_DataFree;
            // 
            // engineStatusesLabel
            // 
            resources.ApplyResources(engineStatusesLabel, "engineStatusesLabel");
            engineStatusesLabel.Name = "engineStatusesLabel";
            // 
            // statusText
            // 
            resources.ApplyResources(this.statusText, "statusText");
            this.statusText.BackColor = System.Drawing.SystemColors.Control;
            this.statusText.Name = "statusText";
            this.statusText.ReadOnly = true;
            this.statusText.Text = global::MySql.Data.VisualStudio.Properties.Resources.Description_Table_DataFree;
            // 
            // buttonOK
            // 
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // SqlErrorDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.statusText);
            this.Controls.Add(engineStatusesLabel);
            this.Controls.Add(this.errorText);
            this.Controls.Add(this.iconLabel);
            this.Controls.Add(this.errorLabel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SqlErrorDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.Label iconLabel;
        private System.Windows.Forms.RichTextBox errorText;
        private System.Windows.Forms.RichTextBox statusText;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Label errorLabel;
    }
}