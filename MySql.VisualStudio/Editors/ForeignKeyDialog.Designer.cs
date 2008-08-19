namespace MySql.Data.VisualStudio.Editors
{
    partial class ForeignKeyDialog
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
            this.fkList = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.addButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.refTable = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.updateAction = new System.Windows.Forms.ComboBox();
            this.deleteAction = new System.Windows.Forms.ComboBox();
            this.columnGrid = new System.Windows.Forms.DataGridView();
            this.column = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.fkColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.closeButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.fkName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.columnGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // fkList
            // 
            this.fkList.FormattingEnabled = true;
            this.fkList.Location = new System.Drawing.Point(12, 30);
            this.fkList.Name = "fkList";
            this.fkList.Size = new System.Drawing.Size(156, 251);
            this.fkList.TabIndex = 0;
            this.fkList.SelectedIndexChanged += new System.EventHandler(this.fkList_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Selected Relationships:";
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(12, 288);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(75, 23);
            this.addButton.TabIndex = 2;
            this.addButton.Text = "Add";
            this.addButton.UseVisualStyleBackColor = true;
            // 
            // deleteButton
            // 
            this.deleteButton.Location = new System.Drawing.Point(93, 288);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(75, 23);
            this.deleteButton.TabIndex = 3;
            this.deleteButton.Text = "Delete";
            this.deleteButton.UseVisualStyleBackColor = true;
            // 
            // refTable
            // 
            this.refTable.Enabled = false;
            this.refTable.FormattingEnabled = true;
            this.refTable.Location = new System.Drawing.Point(293, 56);
            this.refTable.Name = "refTable";
            this.refTable.Size = new System.Drawing.Size(215, 21);
            this.refTable.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(194, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Referenced Table:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(210, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Update:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // updateAction
            // 
            this.updateAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.updateAction.Enabled = false;
            this.updateAction.FormattingEnabled = true;
            this.updateAction.Items.AddRange(new object[] {
            "RESTRICT",
            "CASCADE",
            "SET NULL",
            "NO ACTION"});
            this.updateAction.Location = new System.Drawing.Point(260, 86);
            this.updateAction.Name = "updateAction";
            this.updateAction.Size = new System.Drawing.Size(92, 21);
            this.updateAction.TabIndex = 8;
            // 
            // deleteAction
            // 
            this.deleteAction.Items.AddRange(new object[] {
            "RESTRICT",
            "CASCADE",
            "SET NULL",
            "NO ACTION"});
            this.deleteAction.Location = new System.Drawing.Point(416, 86);
            this.deleteAction.Name = "deleteAction";
            this.deleteAction.Size = new System.Drawing.Size(92, 21);
            this.deleteAction.TabIndex = 9;
            // 
            // columnGrid
            // 
            this.columnGrid.AllowUserToResizeRows = false;
            this.columnGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.columnGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.columnGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.column,
            this.fkColumn});
            this.columnGrid.Enabled = false;
            this.columnGrid.Location = new System.Drawing.Point(187, 116);
            this.columnGrid.MultiSelect = false;
            this.columnGrid.Name = "columnGrid";
            this.columnGrid.RowHeadersVisible = false;
            this.columnGrid.ShowEditingIcon = false;
            this.columnGrid.Size = new System.Drawing.Size(321, 165);
            this.columnGrid.TabIndex = 10;
            // 
            // column
            // 
            this.column.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.column.DisplayStyleForCurrentCellOnly = true;
            this.column.HeaderText = "Column";
            this.column.Name = "column";
            // 
            // fkColumn
            // 
            this.fkColumn.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.fkColumn.DisplayStyleForCurrentCellOnly = true;
            this.fkColumn.HeaderText = "Foreign Column";
            this.fkColumn.Name = "fkColumn";
            // 
            // closeButton
            // 
            this.closeButton.Location = new System.Drawing.Point(433, 288);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 12;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(372, 90);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Delete:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // fkName
            // 
            this.fkName.Location = new System.Drawing.Point(293, 30);
            this.fkName.Name = "fkName";
            this.fkName.Size = new System.Drawing.Size(215, 20);
            this.fkName.TabIndex = 14;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(252, 34);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "Name:";
            // 
            // ForeignKeyDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(522, 323);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.fkName);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.columnGrid);
            this.Controls.Add(this.deleteAction);
            this.Controls.Add(this.updateAction);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.refTable);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.fkList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ForeignKeyDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Foreign Key Relationships";
            ((System.ComponentModel.ISupportInitialize)(this.columnGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox fkList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.ComboBox refTable;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox updateAction;
        private System.Windows.Forms.ComboBox deleteAction;
        private System.Windows.Forms.DataGridView columnGrid;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox fkName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridViewComboBoxColumn column;
        private System.Windows.Forms.DataGridViewComboBoxColumn fkColumn;
    }
}