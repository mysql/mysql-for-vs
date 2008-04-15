namespace MySql.Data.VisualStudio.DocumentView
{
    partial class UdfEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UdfEditor));
            this.pnlTable = new System.Windows.Forms.TableLayoutPanel();
            this.lblType = new System.Windows.Forms.Label();
            this.chkType = new System.Windows.Forms.CheckBox();
            this.cboReturns = new System.Windows.Forms.ComboBox();
            this.lblReturns = new System.Windows.Forms.Label();
            this.txtDll = new System.Windows.Forms.TextBox();
            this.lblDll = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.pnlTable.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTable
            // 
            resources.ApplyResources(this.pnlTable, "pnlTable");
            this.pnlTable.Controls.Add(this.lblType, 0, 3);
            this.pnlTable.Controls.Add(this.chkType, 1, 3);
            this.pnlTable.Controls.Add(this.cboReturns, 1, 2);
            this.pnlTable.Controls.Add(this.lblReturns, 0, 2);
            this.pnlTable.Controls.Add(this.txtDll, 1, 1);
            this.pnlTable.Controls.Add(this.lblDll, 0, 1);
            this.pnlTable.Controls.Add(this.lblName, 0, 0);
            this.pnlTable.Controls.Add(this.txtName, 1, 0);
            this.pnlTable.Name = "pnlTable";
            // 
            // lblType
            // 
            resources.ApplyResources(this.lblType, "lblType");
            this.lblType.Name = "lblType";
            // 
            // chkType
            // 
            resources.ApplyResources(this.chkType, "chkType");
            this.chkType.Name = "chkType";
            this.chkType.UseVisualStyleBackColor = true;
            this.chkType.CheckedChanged += new System.EventHandler(this.chkType_CheckedChanged);
            // 
            // cboReturns
            // 
            resources.ApplyResources(this.cboReturns, "cboReturns");
            this.cboReturns.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboReturns.FormattingEnabled = true;
            this.cboReturns.Name = "cboReturns";
            this.cboReturns.SelectedIndexChanged += new System.EventHandler(this.cboReturns_SelectedIndexChanged);
            // 
            // lblReturns
            // 
            resources.ApplyResources(this.lblReturns, "lblReturns");
            this.lblReturns.Name = "lblReturns";
            // 
            // txtDll
            // 
            resources.ApplyResources(this.txtDll, "txtDll");
            this.txtDll.Name = "txtDll";
            this.txtDll.Leave += new System.EventHandler(this.txtDll_Leave);
            // 
            // lblDll
            // 
            resources.ApplyResources(this.lblDll, "lblDll");
            this.lblDll.Name = "lblDll";
            // 
            // lblName
            // 
            resources.ApplyResources(this.lblName, "lblName");
            this.lblName.Name = "lblName";
            // 
            // txtName
            // 
            resources.ApplyResources(this.txtName, "txtName");
            this.txtName.Name = "txtName";
            this.txtName.Leave += new System.EventHandler(this.txtName_Leave);
            // 
            // UdfEditor
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlTable);
            this.Name = "UdfEditor";
            this.pnlTable.ResumeLayout(false);
            this.pnlTable.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel pnlTable;
        private System.Windows.Forms.Label lblDll;
        private System.Windows.Forms.TextBox txtDll;
        private System.Windows.Forms.Label lblReturns;
        private System.Windows.Forms.ComboBox cboReturns;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.CheckBox chkType;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
    }
}
