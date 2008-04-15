namespace MySql.Data.VisualStudio.DocumentView
{
    partial class IndexesEdit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IndexesEdit));
            System.Windows.Forms.TableLayoutPanel innerLayoutPanel;
            System.Windows.Forms.Label indexNameLabel;
            System.Windows.Forms.Label indexKindLabel;
            System.Windows.Forms.Label indexTypeLabel;
            System.Windows.Forms.ImageList imageList;
            this.indexSettingsGroup = new System.Windows.Forms.GroupBox();
            this.indexNameText = new MySql.Data.VisualStudio.DocumentView.NameTextBox();
            this.indexKindSelect = new System.Windows.Forms.ComboBox();
            this.indexTypeSelect = new System.Windows.Forms.ComboBox();
            this.indexColumns = new MySql.Data.VisualStudio.DocumentView.AdvancedDataGridView();
            this.sourceColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.columnLengthColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.indexNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.positionInIndexColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.removeButton = new System.Windows.Forms.Button();
            this.addButton = new System.Windows.Forms.Button();
            this.indexesList = new System.Windows.Forms.ListBox();
            outerLayout = new System.Windows.Forms.TableLayoutPanel();
            innerLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            indexNameLabel = new System.Windows.Forms.Label();
            indexKindLabel = new System.Windows.Forms.Label();
            indexTypeLabel = new System.Windows.Forms.Label();
            imageList = new System.Windows.Forms.ImageList(this.components);
            outerLayout.SuspendLayout();
            this.indexSettingsGroup.SuspendLayout();
            innerLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.indexColumns)).BeginInit();
            this.SuspendLayout();
            // 
            // outerLayout
            // 
            resources.ApplyResources(outerLayout, "outerLayout");
            outerLayout.Controls.Add(this.indexSettingsGroup, 2, 0);
            outerLayout.Controls.Add(this.removeButton, 1, 1);
            outerLayout.Controls.Add(this.addButton, 0, 1);
            outerLayout.Controls.Add(this.indexesList, 0, 0);
            outerLayout.Name = "outerLayout";
            // 
            // indexSettingsGroup
            // 
            resources.ApplyResources(this.indexSettingsGroup, "indexSettingsGroup");
            this.indexSettingsGroup.Controls.Add(innerLayoutPanel);
            this.indexSettingsGroup.Name = "indexSettingsGroup";
            outerLayout.SetRowSpan(this.indexSettingsGroup, 2);
            this.indexSettingsGroup.TabStop = false;
            // 
            // innerLayoutPanel
            // 
            resources.ApplyResources(innerLayoutPanel, "innerLayoutPanel");
            innerLayoutPanel.Controls.Add(indexNameLabel, 0, 0);
            innerLayoutPanel.Controls.Add(indexKindLabel, 0, 1);
            innerLayoutPanel.Controls.Add(indexTypeLabel, 0, 2);
            innerLayoutPanel.Controls.Add(this.indexNameText, 1, 0);
            innerLayoutPanel.Controls.Add(this.indexKindSelect, 1, 1);
            innerLayoutPanel.Controls.Add(this.indexTypeSelect, 1, 2);
            innerLayoutPanel.Controls.Add(this.indexColumns, 2, 0);
            innerLayoutPanel.Name = "innerLayoutPanel";
            // 
            // indexNameLabel
            // 
            resources.ApplyResources(indexNameLabel, "indexNameLabel");
            indexNameLabel.Name = "indexNameLabel";
            // 
            // indexKindLabel
            // 
            resources.ApplyResources(indexKindLabel, "indexKindLabel");
            indexKindLabel.Name = "indexKindLabel";
            // 
            // indexTypeLabel
            // 
            resources.ApplyResources(indexTypeLabel, "indexTypeLabel");
            indexTypeLabel.Name = "indexTypeLabel";
            // 
            // indexNameText
            // 
            this.indexNameText.AttributeName = null;
            this.indexNameText.DataSource = null;
            resources.ApplyResources(this.indexNameText, "indexNameText");
            this.indexNameText.Name = "indexNameText";
            this.indexNameText.NameChanging += new System.EventHandler(this.OnIndexNameChanging);
            this.indexNameText.NameChanged += new System.EventHandler(this.OnIndexNameChanged);
            // 
            // indexKindSelect
            // 
            this.indexKindSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.indexKindSelect.FormattingEnabled = true;
            resources.ApplyResources(this.indexKindSelect, "indexKindSelect");
            this.indexKindSelect.Name = "indexKindSelect";
            this.indexKindSelect.SelectedIndexChanged += new System.EventHandler(this.OnKindOrTypeChanged);
            // 
            // indexTypeSelect
            // 
            this.indexTypeSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.indexTypeSelect.FormattingEnabled = true;
            resources.ApplyResources(this.indexTypeSelect, "indexTypeSelect");
            this.indexTypeSelect.Name = "indexTypeSelect";
            this.indexTypeSelect.SelectedIndexChanged += new System.EventHandler(this.OnKindOrTypeChanged);
            // 
            // indexColumns
            // 
            resources.ApplyResources(this.indexColumns, "indexColumns");
            this.indexColumns.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.indexColumns.BackgroundColor = System.Drawing.Color.White;
            this.indexColumns.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.indexColumns.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.sourceColumn,
            this.columnLengthColumn,
            this.indexNameColumn,
            this.positionInIndexColumn});
            innerLayoutPanel.SetColumnSpan(this.indexColumns, 2);
            this.indexColumns.Name = "indexColumns";
            this.indexColumns.OrdinalColumn = null;
            innerLayoutPanel.SetRowSpan(this.indexColumns, 4);
            this.indexColumns.ShowEditingIcon = false;
            this.indexColumns.DefaultValuesNeeded += new System.Windows.Forms.DataGridViewRowEventHandler(this.OnIndexColumnDefaultValuesNeeded);
            this.indexColumns.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.OnIndexColumnsDataError);
            // 
            // sourceColumn
            // 
            this.sourceColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.sourceColumn, "sourceColumn");
            this.sourceColumn.Name = "sourceColumn";
            // 
            // columnLengthColumn
            // 
            this.columnLengthColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.columnLengthColumn, "columnLengthColumn");
            this.columnLengthColumn.Name = "columnLengthColumn";
            this.columnLengthColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // indexNameColumn
            // 
            resources.ApplyResources(this.indexNameColumn, "indexNameColumn");
            this.indexNameColumn.Name = "indexNameColumn";
            this.indexNameColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // positionInIndexColumn
            // 
            resources.ApplyResources(this.positionInIndexColumn, "positionInIndexColumn");
            this.positionInIndexColumn.Name = "positionInIndexColumn";
            // 
            // removeButton
            // 
            resources.ApplyResources(this.removeButton, "removeButton");
            this.removeButton.FlatAppearance.BorderSize = 0;
            this.removeButton.ImageList = imageList;
            this.removeButton.Name = "removeButton";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.OnRemoveIndexClick);
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
            this.addButton.Click += new System.EventHandler(this.OnAddIndexClick);
            // 
            // indexesList
            // 
            resources.ApplyResources(this.indexesList, "indexesList");
            outerLayout.SetColumnSpan(this.indexesList, 2);
            this.indexesList.FormattingEnabled = true;
            this.indexesList.Name = "indexesList";
            this.indexesList.SelectedIndexChanged += new System.EventHandler(this.OnSelectedIndexChanged);
            // 
            // IndexesEdit
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(outerLayout);
            this.Name = "IndexesEdit";
            outerLayout.ResumeLayout(false);
            this.indexSettingsGroup.ResumeLayout(false);
            innerLayoutPanel.ResumeLayout(false);
            innerLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.indexColumns)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox indexesList;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button removeButton;
        private MySql.Data.VisualStudio.DocumentView.NameTextBox indexNameText;
        private System.Windows.Forms.ComboBox indexKindSelect;
        private System.Windows.Forms.ComboBox indexTypeSelect;
        private System.Windows.Forms.GroupBox indexSettingsGroup;
        private AdvancedDataGridView indexColumns;
        private System.Windows.Forms.DataGridViewComboBoxColumn sourceColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnLengthColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn indexNameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn positionInIndexColumn;
    }
}
