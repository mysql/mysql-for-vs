namespace MySql.VSTools
{
    partial class EditColumnDialog
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
            this.columnName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.columnType = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.autoincrement = new System.Windows.Forms.CheckBox();
            this.allowNull = new System.Windows.Forms.CheckBox();
            this.zerofill = new System.Windows.Forms.CheckBox();
            this.unsigned = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.charSet = new System.Windows.Forms.ComboBox();
            this.collation = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.defaultValue = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.comment = new System.Windows.Forms.TextBox();
            this.cancelbtn = new System.Windows.Forms.Button();
            this.okbtn = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(49, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name:";
            // 
            // columnName
            // 
            this.columnName.Location = new System.Drawing.Point(93, 12);
            this.columnName.Name = "columnName";
            this.columnName.Size = new System.Drawing.Size(169, 20);
            this.columnName.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(53, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Type:";
            // 
            // columnType
            // 
            this.columnType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.columnType.FormattingEnabled = true;
            this.columnType.Items.AddRange(new object[] {
            "BINARY",
            "BLOB",
            "CHAR",
            "DATE",
            "DATETIME",
            "DECIMAL",
            "DOUBLE",
            "ENUM",
            "FLOAT",
            "INTEGER",
            "DECIMAL",
            "REAL",
            "SET",
            "TEXT",
            "TIME",
            "TIMESTAMP",
            "VARBINARY",
            "VARCHAR",
            "YEAR"});
            this.columnType.Location = new System.Drawing.Point(93, 38);
            this.columnType.Name = "columnType";
            this.columnType.Size = new System.Drawing.Size(169, 21);
            this.columnType.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.autoincrement);
            this.groupBox1.Controls.Add(this.allowNull);
            this.groupBox1.Controls.Add(this.zerofill);
            this.groupBox1.Controls.Add(this.unsigned);
            this.groupBox1.Location = new System.Drawing.Point(280, 15);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 98);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Options";
            // 
            // autoincrement
            // 
            this.autoincrement.AutoSize = true;
            this.autoincrement.Location = new System.Drawing.Point(85, 44);
            this.autoincrement.Name = "autoincrement";
            this.autoincrement.Size = new System.Drawing.Size(94, 17);
            this.autoincrement.TabIndex = 3;
            this.autoincrement.Text = "Autoincrement";
            this.autoincrement.UseVisualStyleBackColor = true;
            // 
            // allowNull
            // 
            this.allowNull.AutoSize = true;
            this.allowNull.Location = new System.Drawing.Point(85, 20);
            this.allowNull.Name = "allowNull";
            this.allowNull.Size = new System.Drawing.Size(72, 17);
            this.allowNull.TabIndex = 2;
            this.allowNull.Text = "Allow Null";
            this.allowNull.UseVisualStyleBackColor = true;
            // 
            // zerofill
            // 
            this.zerofill.AutoSize = true;
            this.zerofill.Location = new System.Drawing.Point(7, 44);
            this.zerofill.Name = "zerofill";
            this.zerofill.Size = new System.Drawing.Size(63, 17);
            this.zerofill.TabIndex = 1;
            this.zerofill.Text = "Zero Fill";
            this.zerofill.UseVisualStyleBackColor = true;
            // 
            // unsigned
            // 
            this.unsigned.AutoSize = true;
            this.unsigned.Location = new System.Drawing.Point(7, 20);
            this.unsigned.Name = "unsigned";
            this.unsigned.Size = new System.Drawing.Size(71, 17);
            this.unsigned.TabIndex = 0;
            this.unsigned.Text = "Unsigned";
            this.unsigned.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Character Set:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(37, 96);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Collation:";
            // 
            // charSet
            // 
            this.charSet.FormattingEnabled = true;
            this.charSet.Location = new System.Drawing.Point(93, 65);
            this.charSet.Name = "charSet";
            this.charSet.Size = new System.Drawing.Size(169, 21);
            this.charSet.TabIndex = 7;
            // 
            // collation
            // 
            this.collation.FormattingEnabled = true;
            this.collation.Location = new System.Drawing.Point(93, 92);
            this.collation.Name = "collation";
            this.collation.Size = new System.Drawing.Size(169, 21);
            this.collation.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 123);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Default Value:";
            // 
            // defaultValue
            // 
            this.defaultValue.Location = new System.Drawing.Point(93, 120);
            this.defaultValue.Name = "defaultValue";
            this.defaultValue.Size = new System.Drawing.Size(387, 20);
            this.defaultValue.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(33, 152);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(54, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Comment:";
            // 
            // comment
            // 
            this.comment.Location = new System.Drawing.Point(93, 147);
            this.comment.Multiline = true;
            this.comment.Name = "comment";
            this.comment.Size = new System.Drawing.Size(387, 74);
            this.comment.TabIndex = 12;
            // 
            // cancelbtn
            // 
            this.cancelbtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelbtn.Location = new System.Drawing.Point(405, 227);
            this.cancelbtn.Name = "cancelbtn";
            this.cancelbtn.Size = new System.Drawing.Size(75, 23);
            this.cancelbtn.TabIndex = 13;
            this.cancelbtn.Text = "Cancel";
            this.cancelbtn.UseVisualStyleBackColor = true;
            // 
            // okbtn
            // 
            this.okbtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okbtn.Location = new System.Drawing.Point(324, 227);
            this.okbtn.Name = "okbtn";
            this.okbtn.Size = new System.Drawing.Size(75, 23);
            this.okbtn.TabIndex = 14;
            this.okbtn.Text = "OK";
            this.okbtn.UseVisualStyleBackColor = true;
            this.okbtn.Click += new System.EventHandler(this.okbtn_Click);
            // 
            // EditColumnDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(492, 262);
            this.Controls.Add(this.okbtn);
            this.Controls.Add(this.cancelbtn);
            this.Controls.Add(this.comment);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.defaultValue);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.collation);
            this.Controls.Add(this.charSet);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.columnType);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.columnName);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditColumnDialog";
            this.Text = "Edit Column";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox columnName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox columnType;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox charSet;
        private System.Windows.Forms.ComboBox collation;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox defaultValue;
        private System.Windows.Forms.CheckBox autoincrement;
        private System.Windows.Forms.CheckBox allowNull;
        private System.Windows.Forms.CheckBox zerofill;
        private System.Windows.Forms.CheckBox unsigned;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox comment;
        private System.Windows.Forms.Button cancelbtn;
        private System.Windows.Forms.Button okbtn;
    }
}