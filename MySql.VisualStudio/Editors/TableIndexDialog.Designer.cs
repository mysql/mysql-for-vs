namespace MySql.Data.VisualStudio.Editors
{
    partial class TableIndexDialog
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
            this.indexList = new System.Windows.Forms.ListBox();
            this.addButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.indexType = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.keyBlockSize = new System.Windows.Forms.NumericUpDown();
            this.storageType = new System.Windows.Forms.ComboBox();
            this.columnGrid = new System.Windows.Forms.DataGridView();
            this.Column = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Length = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Sort = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.closeButton = new System.Windows.Forms.Button();
            this.indexName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.keyBlockSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.columnGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(190, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Selected Primary/Unique Key or Index:";
            // 
            // indexList
            // 
            this.indexList.FormattingEnabled = true;
            this.indexList.Location = new System.Drawing.Point(12, 31);
            this.indexList.Name = "indexList";
            this.indexList.Size = new System.Drawing.Size(176, 264);
            this.indexList.TabIndex = 0;
            this.indexList.SelectedIndexChanged += new System.EventHandler(this.indexList_SelectedIndexChanged);
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(12, 311);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(85, 23);
            this.addButton.TabIndex = 5;
            this.addButton.Text = "Add";
            this.addButton.UseVisualStyleBackColor = true;
            // 
            // deleteButton
            // 
            this.deleteButton.Location = new System.Drawing.Point(103, 311);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(85, 23);
            this.deleteButton.TabIndex = 6;
            this.deleteButton.Text = "Delete";
            this.deleteButton.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(255, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Type:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // indexType
            // 
            this.indexType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.indexType.Enabled = false;
            this.indexType.FormattingEnabled = true;
            this.indexType.Items.AddRange(new object[] {
            "INDEX",
            "UNIQUE",
            "FULLTEXT",
            "SPATIAL",
            "PRIMARY"});
            this.indexType.Location = new System.Drawing.Point(296, 60);
            this.indexType.Name = "indexType";
            this.indexType.Size = new System.Drawing.Size(108, 21);
            this.indexType.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(410, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Storage:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(208, 92);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(81, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Key Block Size:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // keyBlockSize
            // 
            this.keyBlockSize.Enabled = false;
            this.keyBlockSize.Location = new System.Drawing.Point(296, 89);
            this.keyBlockSize.Name = "keyBlockSize";
            this.keyBlockSize.Size = new System.Drawing.Size(108, 20);
            this.keyBlockSize.TabIndex = 3;
            // 
            // storageType
            // 
            this.storageType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.storageType.Enabled = false;
            this.storageType.FormattingEnabled = true;
            this.storageType.Items.AddRange(new object[] {
            "BTREE",
            "HASH",
            "RTREE"});
            this.storageType.Location = new System.Drawing.Point(463, 60);
            this.storageType.Name = "storageType";
            this.storageType.Size = new System.Drawing.Size(75, 21);
            this.storageType.TabIndex = 2;
            // 
            // columnGrid
            // 
            this.columnGrid.AllowUserToResizeRows = false;
            this.columnGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.columnGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.columnGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column,
            this.Length,
            this.Sort});
            this.columnGrid.Location = new System.Drawing.Point(209, 127);
            this.columnGrid.MultiSelect = false;
            this.columnGrid.Name = "columnGrid";
            this.columnGrid.RowHeadersVisible = false;
            this.columnGrid.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.columnGrid.ShowEditingIcon = false;
            this.columnGrid.Size = new System.Drawing.Size(339, 168);
            this.columnGrid.TabIndex = 4;
            this.columnGrid.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.columnGrid_EditingControlShowing);
            // 
            // Column
            // 
            this.Column.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.Column.HeaderText = "Column";
            this.Column.Name = "Column";
            // 
            // Length
            // 
            this.Length.HeaderText = "Length";
            this.Length.Name = "Length";
            // 
            // Sort
            // 
            this.Sort.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Sort.HeaderText = "Sort";
            this.Sort.Name = "Sort";
            // 
            // closeButton
            // 
            this.closeButton.Location = new System.Drawing.Point(463, 311);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(85, 23);
            this.closeButton.TabIndex = 7;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // indexName
            // 
            this.indexName.Enabled = false;
            this.indexName.Location = new System.Drawing.Point(296, 31);
            this.indexName.Name = "indexName";
            this.indexName.Size = new System.Drawing.Size(242, 20);
            this.indexName.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(251, 35);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Name:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBox1
            // 
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox1.Location = new System.Drawing.Point(254, 311);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 10;
            // 
            // TableIndexDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(560, 346);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.indexName);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.columnGrid);
            this.Controls.Add(this.storageType);
            this.Controls.Add(this.keyBlockSize);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.indexType);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.indexList);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TableIndexDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Indexes/Keys";
            ((System.ComponentModel.ISupportInitialize)(this.keyBlockSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.columnGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox indexList;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox indexType;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown keyBlockSize;
        private System.Windows.Forms.ComboBox storageType;
        private System.Windows.Forms.DataGridView columnGrid;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.TextBox indexName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DataGridViewComboBoxColumn Column;
        private System.Windows.Forms.DataGridViewTextBoxColumn Length;
        private System.Windows.Forms.DataGridViewComboBoxColumn Sort;
        private System.Windows.Forms.TextBox textBox1;
    }
}