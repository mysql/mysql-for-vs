namespace MySql.Data.VisualStudio.DocumentView
{
    partial class ForeignKeysEdit
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TableLayoutPanel outerLayout;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ForeignKeysEdit));
            System.Windows.Forms.TableLayoutPanel innerLayoutPanel;
            System.Windows.Forms.Label refTableLabel;
            System.Windows.Forms.Label keyNameLabel;
            System.Windows.Forms.Label onDeleteLabel;
            System.Windows.Forms.Label onUpdateLabel;
            System.Windows.Forms.ImageList imageList;
            this.keySettingsGroup = new System.Windows.Forms.GroupBox();
            this.refTableSelect = new System.Windows.Forms.ComboBox();
            this.keyNameText = new MySql.Data.VisualStudio.DocumentView.NameTextBox();
            this.onDeleteSelect = new System.Windows.Forms.ComboBox();
            this.onUpdateSelect = new System.Windows.Forms.ComboBox();
            this.foreigKeyColumns = new MySql.Data.VisualStudio.DocumentView.AdvancedDataGridView();
            this.sourceColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.referenceColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.keyNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ordinalColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.removeButton = new System.Windows.Forms.Button();
            this.addButton = new System.Windows.Forms.Button();
            this.foreignKeysList = new System.Windows.Forms.ListBox();
            outerLayout = new System.Windows.Forms.TableLayoutPanel();
            innerLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            refTableLabel = new System.Windows.Forms.Label();
            keyNameLabel = new System.Windows.Forms.Label();
            onDeleteLabel = new System.Windows.Forms.Label();
            onUpdateLabel = new System.Windows.Forms.Label();
            imageList = new System.Windows.Forms.ImageList(this.components);
            outerLayout.SuspendLayout();
            this.keySettingsGroup.SuspendLayout();
            innerLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.foreigKeyColumns)).BeginInit();
            this.SuspendLayout();
            // 
            // outerLayout
            // 
            resources.ApplyResources(outerLayout, "outerLayout");
            outerLayout.Controls.Add(this.keySettingsGroup, 2, 0);
            outerLayout.Controls.Add(this.removeButton, 1, 1);
            outerLayout.Controls.Add(this.addButton, 0, 1);
            outerLayout.Controls.Add(this.foreignKeysList, 0, 0);
            outerLayout.Name = "outerLayout";
            // 
            // keySettingsGroup
            // 
            resources.ApplyResources(this.keySettingsGroup, "keySettingsGroup");
            this.keySettingsGroup.Controls.Add(innerLayoutPanel);
            this.keySettingsGroup.Name = "keySettingsGroup";
            outerLayout.SetRowSpan(this.keySettingsGroup, 2);
            this.keySettingsGroup.TabStop = false;
            // 
            // innerLayoutPanel
            // 
            resources.ApplyResources(innerLayoutPanel, "innerLayoutPanel");
            innerLayoutPanel.Controls.Add(this.refTableSelect, 3, 0);
            innerLayoutPanel.Controls.Add(refTableLabel, 2, 0);
            innerLayoutPanel.Controls.Add(keyNameLabel, 0, 0);
            innerLayoutPanel.Controls.Add(onDeleteLabel, 0, 1);
            innerLayoutPanel.Controls.Add(onUpdateLabel, 0, 2);
            innerLayoutPanel.Controls.Add(this.keyNameText, 1, 0);
            innerLayoutPanel.Controls.Add(this.onDeleteSelect, 1, 1);
            innerLayoutPanel.Controls.Add(this.onUpdateSelect, 1, 2);
            innerLayoutPanel.Controls.Add(this.foreigKeyColumns, 2, 1);
            innerLayoutPanel.Name = "innerLayoutPanel";
            // 
            // refTableSelect
            // 
            resources.ApplyResources(this.refTableSelect, "refTableSelect");
            this.refTableSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.refTableSelect.FormattingEnabled = true;
            this.refTableSelect.Name = "refTableSelect";
            this.refTableSelect.SelectedIndexChanged += new System.EventHandler(this.OnReferencedTableChanged);
            // 
            // refTableLabel
            // 
            resources.ApplyResources(refTableLabel, "refTableLabel");
            refTableLabel.Name = "refTableLabel";
            // 
            // keyNameLabel
            // 
            resources.ApplyResources(keyNameLabel, "keyNameLabel");
            keyNameLabel.Name = "keyNameLabel";
            // 
            // onDeleteLabel
            // 
            resources.ApplyResources(onDeleteLabel, "onDeleteLabel");
            onDeleteLabel.Name = "onDeleteLabel";
            // 
            // onUpdateLabel
            // 
            resources.ApplyResources(onUpdateLabel, "onUpdateLabel");
            onUpdateLabel.Name = "onUpdateLabel";
            // 
            // keyNameText
            // 
            this.keyNameText.AttributeName = null;
            this.keyNameText.DataSource = null;
            resources.ApplyResources(this.keyNameText, "keyNameText");
            this.keyNameText.Name = "keyNameText";
            this.keyNameText.NameChanging += new System.EventHandler(this.OnKeyNameChanging);
            this.keyNameText.NameChanged += new System.EventHandler(this.OnKeyNameChanged);
            // 
            // onDeleteSelect
            // 
            this.onDeleteSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.onDeleteSelect.FormattingEnabled = true;
            resources.ApplyResources(this.onDeleteSelect, "onDeleteSelect");
            this.onDeleteSelect.Name = "onDeleteSelect";
            this.onDeleteSelect.SelectedIndexChanged += new System.EventHandler(this.OnActionChanged);
            // 
            // onUpdateSelect
            // 
            this.onUpdateSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.onUpdateSelect.FormattingEnabled = true;
            resources.ApplyResources(this.onUpdateSelect, "onUpdateSelect");
            this.onUpdateSelect.Name = "onUpdateSelect";
            this.onUpdateSelect.SelectedIndexChanged += new System.EventHandler(this.OnActionChanged);
            // 
            // foreigKeyColumns
            // 
            resources.ApplyResources(this.foreigKeyColumns, "foreigKeyColumns");
            this.foreigKeyColumns.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.foreigKeyColumns.BackgroundColor = System.Drawing.Color.White;
            this.foreigKeyColumns.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.foreigKeyColumns.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.sourceColumn,
            this.referenceColumn,
            this.keyNameColumn,
            this.ordinalColumn});
            innerLayoutPanel.SetColumnSpan(this.foreigKeyColumns, 2);
            this.foreigKeyColumns.Name = "foreigKeyColumns";
            this.foreigKeyColumns.OrdinalColumn = null;
            innerLayoutPanel.SetRowSpan(this.foreigKeyColumns, 3);
            this.foreigKeyColumns.ShowEditingIcon = false;
            this.foreigKeyColumns.DefaultValuesNeeded += new System.Windows.Forms.DataGridViewRowEventHandler(this.OnForeigKeyColumnDefaultValuesNeeded);
            this.foreigKeyColumns.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.OnForeigKeyColumnsDataError);
            // 
            // sourceColumn
            // 
            this.sourceColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.sourceColumn, "sourceColumn");
            this.sourceColumn.Name = "sourceColumn";
            // 
            // referenceColumn
            // 
            this.referenceColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.referenceColumn, "referenceColumn");
            this.referenceColumn.Name = "referenceColumn";
            // 
            // keyNameColumn
            // 
            resources.ApplyResources(this.keyNameColumn, "keyNameColumn");
            this.keyNameColumn.Name = "keyNameColumn";
            // 
            // ordinalColumn
            // 
            resources.ApplyResources(this.ordinalColumn, "ordinalColumn");
            this.ordinalColumn.Name = "ordinalColumn";
            // 
            // removeButton
            // 
            resources.ApplyResources(this.removeButton, "removeButton");
            this.removeButton.FlatAppearance.BorderSize = 0;
            this.removeButton.ImageList = imageList;
            this.removeButton.Name = "removeButton";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.OnRemoveKeyClick);
            // 
            // imageList
            // 
            imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            imageList.TransparentColor = System.Drawing.Color.Transparent;
            imageList.Images.SetKeyName(0, "Add_Icon.ico");
            imageList.Images.SetKeyName(1, "Remove_Icon.ico");
            // 
            // addButton
            // 
            resources.ApplyResources(this.addButton, "addButton");
            this.addButton.FlatAppearance.BorderSize = 0;
            this.addButton.ForeColor = System.Drawing.SystemColors.Control;
            this.addButton.ImageList = imageList;
            this.addButton.Name = "addButton";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.OnAddKeyClick);
            // 
            // foreignKeysList
            // 
            resources.ApplyResources(this.foreignKeysList, "foreignKeysList");
            outerLayout.SetColumnSpan(this.foreignKeysList, 2);
            this.foreignKeysList.FormattingEnabled = true;
            this.foreignKeysList.Name = "foreignKeysList";
            this.foreignKeysList.SelectedIndexChanged += new System.EventHandler(this.OnSelectedForeignKeyChanged);
            // 
            // ForeignKeysEdit
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(outerLayout);
            this.Name = "ForeignKeysEdit";
            outerLayout.ResumeLayout(false);
            this.keySettingsGroup.ResumeLayout(false);
            innerLayoutPanel.ResumeLayout(false);
            innerLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.foreigKeyColumns)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox foreignKeysList;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button removeButton;
        private MySql.Data.VisualStudio.DocumentView.NameTextBox keyNameText;
        private System.Windows.Forms.ComboBox onDeleteSelect;
        private System.Windows.Forms.ComboBox refTableSelect;
        private System.Windows.Forms.ComboBox onUpdateSelect;
        private AdvancedDataGridView foreigKeyColumns;
        private System.Windows.Forms.GroupBox keySettingsGroup;
        private System.Windows.Forms.DataGridViewComboBoxColumn sourceColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn referenceColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn keyNameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ordinalColumn;
    }
}
