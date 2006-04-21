namespace MySql.VSTools
{
    partial class TableEditor
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
            this.label1 = new System.Windows.Forms.Label();
            this.tableName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.columnsPage = new System.Windows.Forms.TabPage();
            this.removeButton = new System.Windows.Forms.Button();
            this.addColumn = new System.Windows.Forms.Button();
            this.columnList = new System.Windows.Forms.ListView();
            this.nameColumn = new System.Windows.Forms.ColumnHeader();
            this.typeColumn = new System.Windows.Forms.ColumnHeader();
            this.lengthColumn = new System.Windows.Forms.ColumnHeader();
            this.allowNull = new System.Windows.Forms.ColumnHeader();
            this.binary = new System.Windows.Forms.ColumnHeader();
            this.zero = new System.Windows.Forms.ColumnHeader();
            this.characterSet = new System.Windows.Forms.ColumnHeader();
            this.collation = new System.Windows.Forms.ColumnHeader();
            this.constraintsPage = new System.Windows.Forms.TabPage();
            this.indexesPage = new System.Windows.Forms.TabPage();
            this.optionsPage = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.maxRows = new System.Windows.Forms.TextBox();
            this.avgRowLen = new System.Windows.Forms.TextBox();
            this.minRows = new System.Windows.Forms.TextBox();
            this.useChecksum = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.rowFormat = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.indexDirectory = new System.Windows.Forms.TextBox();
            this.dataDirectory = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tableType = new System.Windows.Forms.ComboBox();
            this.sqlPage = new System.Windows.Forms.TabPage();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.tableSchema = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this.columnsPage.SuspendLayout();
            this.optionsPage.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.sqlPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Table Name:";
            // 
            // tableName
            // 
            this.tableName.Location = new System.Drawing.Point(90, 5);
            this.tableName.Name = "tableName";
            this.tableName.Size = new System.Drawing.Size(247, 20);
            this.tableName.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Table Schema:";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.columnsPage);
            this.tabControl1.Controls.Add(this.constraintsPage);
            this.tabControl1.Controls.Add(this.indexesPage);
            this.tabControl1.Controls.Add(this.optionsPage);
            this.tabControl1.Controls.Add(this.sqlPage);
            this.tabControl1.Location = new System.Drawing.Point(7, 69);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(640, 269);
            this.tabControl1.TabIndex = 3;
            // 
            // columnsPage
            // 
            this.columnsPage.Controls.Add(this.removeButton);
            this.columnsPage.Controls.Add(this.addColumn);
            this.columnsPage.Controls.Add(this.columnList);
            this.columnsPage.Location = new System.Drawing.Point(4, 22);
            this.columnsPage.Name = "columnsPage";
            this.columnsPage.Padding = new System.Windows.Forms.Padding(3);
            this.columnsPage.Size = new System.Drawing.Size(632, 243);
            this.columnsPage.TabIndex = 0;
            this.columnsPage.Text = "Columns";
            this.columnsPage.UseVisualStyleBackColor = true;
            // 
            // removeButton
            // 
            this.removeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.removeButton.Location = new System.Drawing.Point(87, 213);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(75, 23);
            this.removeButton.TabIndex = 5;
            this.removeButton.Text = "Remove";
            this.removeButton.UseVisualStyleBackColor = true;
            // 
            // addColumn
            // 
            this.addColumn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.addColumn.Location = new System.Drawing.Point(6, 214);
            this.addColumn.Name = "addColumn";
            this.addColumn.Size = new System.Drawing.Size(75, 23);
            this.addColumn.TabIndex = 3;
            this.addColumn.Text = "Add...";
            this.addColumn.UseVisualStyleBackColor = true;
            // 
            // columnList
            // 
            this.columnList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.columnList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameColumn,
            this.typeColumn,
            this.lengthColumn,
            this.allowNull,
            this.binary,
            this.zero,
            this.characterSet,
            this.collation});
            this.columnList.FullRowSelect = true;
            this.columnList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.columnList.Location = new System.Drawing.Point(6, 6);
            this.columnList.Name = "columnList";
            this.columnList.Size = new System.Drawing.Size(620, 201);
            this.columnList.TabIndex = 0;
            this.columnList.UseCompatibleStateImageBehavior = false;
            this.columnList.View = System.Windows.Forms.View.Details;
            this.columnList.DoubleClick += new System.EventHandler(this.columnList_DoubleClick);
            this.columnList.SizeChanged += new System.EventHandler(this.columnList_SizeChanged);
            // 
            // nameColumn
            // 
            this.nameColumn.Text = "Name";
            this.nameColumn.Width = 135;
            // 
            // typeColumn
            // 
            this.typeColumn.Text = "Type";
            // 
            // lengthColumn
            // 
            this.lengthColumn.Text = "Length";
            // 
            // allowNull
            // 
            this.allowNull.Text = "Allow Null";
            this.allowNull.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.allowNull.Width = 75;
            // 
            // binary
            // 
            this.binary.Text = "Binary";
            this.binary.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.binary.Width = 50;
            // 
            // zero
            // 
            this.zero.Text = "Zero Fill";
            this.zero.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.zero.Width = 65;
            // 
            // characterSet
            // 
            this.characterSet.Text = "Character Set";
            this.characterSet.Width = 100;
            // 
            // collation
            // 
            this.collation.Text = "Collation";
            this.collation.Width = 80;
            // 
            // constraintsPage
            // 
            this.constraintsPage.Location = new System.Drawing.Point(4, 22);
            this.constraintsPage.Name = "constraintsPage";
            this.constraintsPage.Padding = new System.Windows.Forms.Padding(3);
            this.constraintsPage.Size = new System.Drawing.Size(632, 377);
            this.constraintsPage.TabIndex = 1;
            this.constraintsPage.Text = "Constraints";
            this.constraintsPage.UseVisualStyleBackColor = true;
            // 
            // indexesPage
            // 
            this.indexesPage.Location = new System.Drawing.Point(4, 22);
            this.indexesPage.Name = "indexesPage";
            this.indexesPage.Size = new System.Drawing.Size(632, 377);
            this.indexesPage.TabIndex = 2;
            this.indexesPage.Text = "Indexes";
            this.indexesPage.UseVisualStyleBackColor = true;
            // 
            // optionsPage
            // 
            this.optionsPage.Controls.Add(this.groupBox2);
            this.optionsPage.Controls.Add(this.groupBox1);
            this.optionsPage.Location = new System.Drawing.Point(4, 22);
            this.optionsPage.Name = "optionsPage";
            this.optionsPage.Padding = new System.Windows.Forms.Padding(3);
            this.optionsPage.Size = new System.Drawing.Size(632, 377);
            this.optionsPage.TabIndex = 3;
            this.optionsPage.Text = "Options";
            this.optionsPage.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.maxRows);
            this.groupBox2.Controls.Add(this.avgRowLen);
            this.groupBox2.Controls.Add(this.minRows);
            this.groupBox2.Controls.Add(this.useChecksum);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.rowFormat);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Location = new System.Drawing.Point(6, 134);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(620, 107);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Row Options";
            // 
            // maxRows
            // 
            this.maxRows.Location = new System.Drawing.Point(339, 46);
            this.maxRows.Name = "maxRows";
            this.maxRows.Size = new System.Drawing.Size(120, 20);
            this.maxRows.TabIndex = 11;
            // 
            // avgRowLen
            // 
            this.avgRowLen.Location = new System.Drawing.Point(123, 74);
            this.avgRowLen.Name = "avgRowLen";
            this.avgRowLen.Size = new System.Drawing.Size(120, 20);
            this.avgRowLen.TabIndex = 10;
            // 
            // minRows
            // 
            this.minRows.Location = new System.Drawing.Point(123, 48);
            this.minRows.Name = "minRows";
            this.minRows.Size = new System.Drawing.Size(120, 20);
            this.minRows.TabIndex = 9;
            // 
            // useChecksum
            // 
            this.useChecksum.AutoSize = true;
            this.useChecksum.Location = new System.Drawing.Point(250, 23);
            this.useChecksum.Name = "useChecksum";
            this.useChecksum.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.useChecksum.Size = new System.Drawing.Size(98, 17);
            this.useChecksum.TabIndex = 8;
            this.useChecksum.Text = "Use Checksum";
            this.useChecksum.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 76);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(111, 13);
            this.label9.TabIndex = 7;
            this.label9.Text = "Average Row Length:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(249, 49);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(84, 13);
            this.label8.TabIndex = 3;
            this.label8.Text = "Maximum Rows:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 49);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(81, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "Minimum Rows:";
            // 
            // rowFormat
            // 
            this.rowFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.rowFormat.FormattingEnabled = true;
            this.rowFormat.Items.AddRange(new object[] {
            "Compact",
            "Default",
            "Dynamic",
            "Fixed",
            "Compressed"});
            this.rowFormat.Location = new System.Drawing.Point(122, 20);
            this.rowFormat.Name = "rowFormat";
            this.rowFormat.Size = new System.Drawing.Size(121, 21);
            this.rowFormat.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(22, 23);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Row Format:";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.indexDirectory);
            this.groupBox1.Controls.Add(this.dataDirectory);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.tableType);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(5);
            this.groupBox1.Size = new System.Drawing.Size(620, 122);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Storage";
            // 
            // indexDirectory
            // 
            this.indexDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.indexDirectory.Location = new System.Drawing.Point(95, 77);
            this.indexDirectory.Name = "indexDirectory";
            this.indexDirectory.Size = new System.Drawing.Size(517, 20);
            this.indexDirectory.TabIndex = 5;
            // 
            // dataDirectory
            // 
            this.dataDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataDirectory.Location = new System.Drawing.Point(95, 51);
            this.dataDirectory.Name = "dataDirectory";
            this.dataDirectory.Size = new System.Drawing.Size(517, 20);
            this.dataDirectory.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 80);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(81, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Index Directory:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(78, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Data Directory:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(25, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Table Type:";
            // 
            // tableType
            // 
            this.tableType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tableType.FormattingEnabled = true;
            this.tableType.Items.AddRange(new object[] {
            "MyISAM",
            "InnoDB",
            "Memory",
            "Merge",
            "NDB",
            "BDB",
            "ISAM"});
            this.tableType.Location = new System.Drawing.Point(95, 24);
            this.tableType.Name = "tableType";
            this.tableType.Size = new System.Drawing.Size(176, 21);
            this.tableType.TabIndex = 1;
            // 
            // sqlPage
            // 
            this.sqlPage.Controls.Add(this.richTextBox1);
            this.sqlPage.Location = new System.Drawing.Point(4, 22);
            this.sqlPage.Name = "sqlPage";
            this.sqlPage.Padding = new System.Windows.Forms.Padding(3);
            this.sqlPage.Size = new System.Drawing.Size(632, 377);
            this.sqlPage.TabIndex = 4;
            this.sqlPage.Text = "SQL";
            this.sqlPage.UseVisualStyleBackColor = true;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(3, 3);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(626, 371);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // tableSchema
            // 
            this.tableSchema.Enabled = false;
            this.tableSchema.Location = new System.Drawing.Point(90, 31);
            this.tableSchema.Name = "tableSchema";
            this.tableSchema.Size = new System.Drawing.Size(247, 20);
            this.tableSchema.TabIndex = 4;
            // 
            // TableEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableSchema);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tableName);
            this.Controls.Add(this.label1);
            this.Name = "TableEditor";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Size = new System.Drawing.Size(650, 341);
            this.tabControl1.ResumeLayout(false);
            this.columnsPage.ResumeLayout(false);
            this.optionsPage.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.sqlPage.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tableName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage columnsPage;
        private System.Windows.Forms.TabPage constraintsPage;
        private System.Windows.Forms.TabPage indexesPage;
        private System.Windows.Forms.TabPage optionsPage;
        private System.Windows.Forms.TextBox tableSchema;
        private System.Windows.Forms.ListView columnList;
        private System.Windows.Forms.TabPage sqlPage;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.ColumnHeader nameColumn;
        private System.Windows.Forms.ColumnHeader typeColumn;
        private System.Windows.Forms.Button addColumn;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.ColumnHeader lengthColumn;
        private System.Windows.Forms.ColumnHeader allowNull;
        private System.Windows.Forms.ColumnHeader binary;
        private System.Windows.Forms.ColumnHeader zero;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox indexDirectory;
        private System.Windows.Forms.TextBox dataDirectory;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox tableType;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox rowFormat;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox useChecksum;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox maxRows;
        private System.Windows.Forms.TextBox avgRowLen;
        private System.Windows.Forms.TextBox minRows;
        private System.Windows.Forms.ColumnHeader characterSet;
        private System.Windows.Forms.ColumnHeader collation;
    }
}
