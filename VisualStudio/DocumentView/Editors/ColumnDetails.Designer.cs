namespace MySql.Data.VisualStudio.DocumentView
{
    partial class ColumnDetails
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
            System.Windows.Forms.Label defaultValueLabel;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ColumnDetails));
            System.Windows.Forms.Label datatypeLabel;
            System.Windows.Forms.Label nameLabel;
            System.Windows.Forms.Label charsetLabel;
            System.Windows.Forms.Label collationLabel;
            System.Windows.Forms.Label flagsLabel;
            System.Windows.Forms.Label commentLabel;
            this.layoutTable = new System.Windows.Forms.TableLayoutPanel();
            this.defaultValueText = new System.Windows.Forms.TextBox();
            this.datatypeText = new System.Windows.Forms.TextBox();
            this.nameText = new MySql.Data.VisualStudio.DocumentView.NameTextBox();
            this.charsetSelect = new System.Windows.Forms.ComboBox();
            this.collationSelect = new System.Windows.Forms.ComboBox();
            this.flagsList = new System.Windows.Forms.CheckedListBox();
            this.commentText = new System.Windows.Forms.TextBox();
            this.columnOptions = new System.Windows.Forms.GroupBox();
            this.autoIncrement = new System.Windows.Forms.CheckBox();
            this.notNull = new System.Windows.Forms.CheckBox();
            this.primaryKey = new System.Windows.Forms.CheckBox();
            this.buttonNull = new System.Windows.Forms.Button();
            defaultValueLabel = new System.Windows.Forms.Label();
            datatypeLabel = new System.Windows.Forms.Label();
            nameLabel = new System.Windows.Forms.Label();
            charsetLabel = new System.Windows.Forms.Label();
            collationLabel = new System.Windows.Forms.Label();
            flagsLabel = new System.Windows.Forms.Label();
            commentLabel = new System.Windows.Forms.Label();
            this.layoutTable.SuspendLayout();
            this.columnOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // defaultValueLabel
            // 
            resources.ApplyResources(defaultValueLabel, "defaultValueLabel");
            defaultValueLabel.Name = "defaultValueLabel";
            // 
            // datatypeLabel
            // 
            resources.ApplyResources(datatypeLabel, "datatypeLabel");
            datatypeLabel.Name = "datatypeLabel";
            // 
            // nameLabel
            // 
            resources.ApplyResources(nameLabel, "nameLabel");
            nameLabel.Name = "nameLabel";
            // 
            // charsetLabel
            // 
            resources.ApplyResources(charsetLabel, "charsetLabel");
            charsetLabel.Name = "charsetLabel";
            // 
            // collationLabel
            // 
            resources.ApplyResources(collationLabel, "collationLabel");
            collationLabel.Name = "collationLabel";
            // 
            // flagsLabel
            // 
            resources.ApplyResources(flagsLabel, "flagsLabel");
            flagsLabel.Name = "flagsLabel";
            // 
            // commentLabel
            // 
            resources.ApplyResources(commentLabel, "commentLabel");
            commentLabel.Name = "commentLabel";
            // 
            // layoutTable
            // 
            resources.ApplyResources(this.layoutTable, "layoutTable");
            this.layoutTable.Controls.Add(this.defaultValueText, 5, 0);
            this.layoutTable.Controls.Add(defaultValueLabel, 4, 0);
            this.layoutTable.Controls.Add(this.datatypeText, 3, 0);
            this.layoutTable.Controls.Add(datatypeLabel, 2, 0);
            this.layoutTable.Controls.Add(nameLabel, 0, 0);
            this.layoutTable.Controls.Add(this.nameText, 1, 0);
            this.layoutTable.Controls.Add(charsetLabel, 4, 1);
            this.layoutTable.Controls.Add(this.charsetSelect, 5, 1);
            this.layoutTable.Controls.Add(collationLabel, 4, 2);
            this.layoutTable.Controls.Add(this.collationSelect, 5, 2);
            this.layoutTable.Controls.Add(flagsLabel, 2, 1);
            this.layoutTable.Controls.Add(this.flagsList, 3, 1);
            this.layoutTable.Controls.Add(commentLabel, 2, 3);
            this.layoutTable.Controls.Add(this.commentText, 3, 3);
            this.layoutTable.Controls.Add(this.columnOptions, 1, 1);
            this.layoutTable.Controls.Add(this.buttonNull, 6, 0);
            this.layoutTable.Name = "layoutTable";
            // 
            // defaultValueText
            // 
            resources.ApplyResources(this.defaultValueText, "defaultValueText");
            this.defaultValueText.Name = "defaultValueText";
            this.defaultValueText.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // datatypeText
            // 
            resources.ApplyResources(this.datatypeText, "datatypeText");
            this.datatypeText.Name = "datatypeText";
            this.datatypeText.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // nameText
            // 
            this.nameText.AttributeName = null;
            this.nameText.DataSource = null;
            resources.ApplyResources(this.nameText, "nameText");
            this.nameText.Name = "nameText";
            // 
            // charsetSelect
            // 
            resources.ApplyResources(this.charsetSelect, "charsetSelect");
            this.layoutTable.SetColumnSpan(this.charsetSelect, 2);
            this.charsetSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.charsetSelect.FormattingEnabled = true;
            this.charsetSelect.Name = "charsetSelect";
            this.charsetSelect.SelectionChangeCommitted += new System.EventHandler(this.OnCharacterSetChanged);
            // 
            // collationSelect
            // 
            resources.ApplyResources(this.collationSelect, "collationSelect");
            this.layoutTable.SetColumnSpan(this.collationSelect, 2);
            this.collationSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.collationSelect.FormattingEnabled = true;
            this.collationSelect.Name = "collationSelect";
            this.collationSelect.SelectionChangeCommitted += new System.EventHandler(this.OnCollationChanged);
            // 
            // flagsList
            // 
            resources.ApplyResources(this.flagsList, "flagsList");
            this.flagsList.CheckOnClick = true;
            this.flagsList.FormattingEnabled = true;
            this.flagsList.Items.AddRange(new object[] {
            resources.GetString("flagsList.Items"),
            resources.GetString("flagsList.Items1"),
            resources.GetString("flagsList.Items2")});
            this.flagsList.Name = "flagsList";
            this.layoutTable.SetRowSpan(this.flagsList, 2);
            this.flagsList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.OnFlagsListItemCheck);
            // 
            // commentText
            // 
            resources.ApplyResources(this.commentText, "commentText");
            this.layoutTable.SetColumnSpan(this.commentText, 4);
            this.commentText.Name = "commentText";
            this.commentText.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // columnOptions
            // 
            resources.ApplyResources(this.columnOptions, "columnOptions");
            this.columnOptions.Controls.Add(this.autoIncrement);
            this.columnOptions.Controls.Add(this.notNull);
            this.columnOptions.Controls.Add(this.primaryKey);
            this.columnOptions.MaximumSize = new System.Drawing.Size(0, 87);
            this.columnOptions.MinimumSize = new System.Drawing.Size(126, 87);
            this.columnOptions.Name = "columnOptions";
            this.layoutTable.SetRowSpan(this.columnOptions, 3);
            this.columnOptions.TabStop = false;
            // 
            // autoIncrement
            // 
            resources.ApplyResources(this.autoIncrement, "autoIncrement");
            this.autoIncrement.Name = "autoIncrement";
            this.autoIncrement.UseVisualStyleBackColor = true;
            this.autoIncrement.CheckedChanged += new System.EventHandler(this.OnCheckedChanged);
            // 
            // notNull
            // 
            resources.ApplyResources(this.notNull, "notNull");
            this.notNull.Name = "notNull";
            this.notNull.UseVisualStyleBackColor = true;
            this.notNull.CheckedChanged += new System.EventHandler(this.OnCheckedChangedReversed);
            // 
            // primaryKey
            // 
            resources.ApplyResources(this.primaryKey, "primaryKey");
            this.primaryKey.Name = "primaryKey";
            this.primaryKey.UseVisualStyleBackColor = true;
            this.primaryKey.CheckedChanged += new System.EventHandler(this.OnCheckedChanged);
            // 
            // buttonNull
            // 
            resources.ApplyResources(this.buttonNull, "buttonNull");
            this.buttonNull.Name = "buttonNull";
            this.buttonNull.UseVisualStyleBackColor = true;
            this.buttonNull.Click += new System.EventHandler(this.OnButtonNullClick);
            // 
            // ColumnDetails
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutTable);
            this.Name = "ColumnDetails";
            this.layoutTable.ResumeLayout(false);
            this.layoutTable.PerformLayout();
            this.columnOptions.ResumeLayout(false);
            this.columnOptions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel layoutTable;
        private MySql.Data.VisualStudio.DocumentView.NameTextBox nameText;
        private System.Windows.Forms.TextBox datatypeText;
        private System.Windows.Forms.TextBox defaultValueText;
        private System.Windows.Forms.ComboBox charsetSelect;
        private System.Windows.Forms.ComboBox collationSelect;
        private System.Windows.Forms.CheckedListBox flagsList;
        private System.Windows.Forms.TextBox commentText;
        private System.Windows.Forms.GroupBox columnOptions;
        private System.Windows.Forms.CheckBox autoIncrement;
        private System.Windows.Forms.CheckBox notNull;
        private System.Windows.Forms.CheckBox primaryKey;
        private System.Windows.Forms.Button buttonNull;
    }
}
