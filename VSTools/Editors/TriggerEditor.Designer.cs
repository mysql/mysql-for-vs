namespace MySql.VSTools
{
    partial class TriggerEditor
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
            this.schema = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.triggername = new System.Windows.Forms.TextBox();
            this.when = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.action = new System.Windows.Forms.ComboBox();
            this.sql = new System.Windows.Forms.RichTextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Schema:";
            // 
            // schema
            // 
            this.schema.Enabled = false;
            this.schema.Location = new System.Drawing.Point(64, 8);
            this.schema.Name = "schema";
            this.schema.Size = new System.Drawing.Size(160, 20);
            this.schema.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Name:";
            // 
            // triggername
            // 
            this.triggername.Location = new System.Drawing.Point(64, 35);
            this.triggername.Name = "triggername";
            this.triggername.Size = new System.Drawing.Size(160, 20);
            this.triggername.TabIndex = 3;
            this.triggername.TextChanged += new System.EventHandler(this.triggername_TextChanged);
            // 
            // when
            // 
            this.when.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.when.FormattingEnabled = true;
            this.when.Items.AddRange(new object[] {
            "BEFORE",
            "AFTER"});
            this.when.Location = new System.Drawing.Point(64, 62);
            this.when.Name = "when";
            this.when.Size = new System.Drawing.Size(121, 21);
            this.when.TabIndex = 4;
            this.when.SelectedIndexChanged += new System.EventHandler(this.when_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "When:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(191, 65);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Action:";
            // 
            // action
            // 
            this.action.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.action.FormattingEnabled = true;
            this.action.Items.AddRange(new object[] {
            "INSERT",
            "UPDATE",
            "DELETE"});
            this.action.Location = new System.Drawing.Point(237, 62);
            this.action.Name = "action";
            this.action.Size = new System.Drawing.Size(121, 21);
            this.action.TabIndex = 7;
            this.action.SelectedIndexChanged += new System.EventHandler(this.action_SelectedIndexChanged);
            // 
            // sql
            // 
            this.sql.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sql.Location = new System.Drawing.Point(12, 109);
            this.sql.Name = "sql";
            this.sql.Size = new System.Drawing.Size(556, 264);
            this.sql.TabIndex = 8;
            this.sql.Text = "";
            this.sql.TextChanged += new System.EventHandler(this.sql_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 93);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "SQL:";
            // 
            // TriggerEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label5);
            this.Controls.Add(this.sql);
            this.Controls.Add(this.action);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.when);
            this.Controls.Add(this.triggername);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.schema);
            this.Controls.Add(this.label1);
            this.Name = "TriggerEditor";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Size = new System.Drawing.Size(576, 381);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox schema;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox triggername;
        private System.Windows.Forms.ComboBox when;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox action;
        private System.Windows.Forms.RichTextBox sql;
        private System.Windows.Forms.Label label5;
    }
}
