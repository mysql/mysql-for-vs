using System.Windows.Forms;
namespace MySql.Data.VisualStudio.DocumentView
{
    public partial class TableEditor
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
            System.Windows.Forms.TabPage foreignKeysTabPage;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TableEditor));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.foreignKeysEdit = new MySql.Data.VisualStudio.DocumentView.ForeignKeysEdit();
            this.columnsSplitContainer = new System.Windows.Forms.SplitContainer();
            this.columnsDataGrid = new MySql.Data.VisualStudio.DocumentView.AdvancedDataGridView();
            this.primaryKeyColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.columnNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.datatypeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.notNullColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.autoIncrementColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.flagsColumn = new MySql.Data.VisualStudio.DocumentView.DataGridViewFlagsColumn();
            this.defaultValueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.commentColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ordinalPosition = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnsFooterTabControl = new System.Windows.Forms.TabControl();
            this.indexesTabPage = new System.Windows.Forms.TabPage();
            this.indexesEdit = new MySql.Data.VisualStudio.DocumentView.IndexesEdit();
            this.columnDetailsTabPage = new System.Windows.Forms.TabPage();
            this.columnDetails = new MySql.Data.VisualStudio.DocumentView.ColumnDetails();
            foreignKeysTabPage = new System.Windows.Forms.TabPage();
            foreignKeysTabPage.SuspendLayout();
            this.columnsSplitContainer.Panel1.SuspendLayout();
            this.columnsSplitContainer.Panel2.SuspendLayout();
            this.columnsSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.columnsDataGrid)).BeginInit();
            this.columnsFooterTabControl.SuspendLayout();
            this.indexesTabPage.SuspendLayout();
            this.columnDetailsTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // foreignKeysTabPage
            // 
            foreignKeysTabPage.Controls.Add(this.foreignKeysEdit);
            resources.ApplyResources(foreignKeysTabPage, "foreignKeysTabPage");
            foreignKeysTabPage.Name = "foreignKeysTabPage";
            foreignKeysTabPage.UseVisualStyleBackColor = true;
            // 
            // foreignKeysEdit
            // 
            resources.ApplyResources(this.foreignKeysEdit, "foreignKeysEdit");
            this.foreignKeysEdit.Name = "foreignKeysEdit";
            // 
            // columnsSplitContainer
            // 
            resources.ApplyResources(this.columnsSplitContainer, "columnsSplitContainer");
            this.columnsSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.columnsSplitContainer.Name = "columnsSplitContainer";
            // 
            // columnsSplitContainer.Panel1
            // 
            this.columnsSplitContainer.Panel1.Controls.Add(this.columnsDataGrid);
            // 
            // columnsSplitContainer.Panel2
            // 
            this.columnsSplitContainer.Panel2.Controls.Add(this.columnsFooterTabControl);
            // 
            // columnsDataGrid
            // 
            this.columnsDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.columnsDataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.primaryKeyColumn,
            this.columnNameColumn,
            this.datatypeColumn,
            this.notNullColumn,
            this.autoIncrementColumn,
            this.flagsColumn,
            this.defaultValueColumn,
            this.commentColumn,
            this.ordinalPosition});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.columnsDataGrid.DefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.columnsDataGrid, "columnsDataGrid");
            this.columnsDataGrid.Name = "columnsDataGrid";
            this.columnsDataGrid.OrdinalColumn = null;
            this.columnsDataGrid.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.columnsDataGrid.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.OnColumnsDataGridRowEnter);
            this.columnsDataGrid.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.OnCellValidating);
            this.columnsDataGrid.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.OnEditingControlShowing);
            // 
            // primaryKeyColumn
            // 
            this.primaryKeyColumn.DataPropertyName = "IS_PRIMARYKEY";
            this.primaryKeyColumn.FalseValue = "NO";
            resources.ApplyResources(this.primaryKeyColumn, "primaryKeyColumn");
            this.primaryKeyColumn.Name = "primaryKeyColumn";
            this.primaryKeyColumn.TrueValue = "YES";
            // 
            // columnNameColumn
            // 
            this.columnNameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.columnNameColumn.DataPropertyName = "COLUMN_NAME";
            resources.ApplyResources(this.columnNameColumn, "columnNameColumn");
            this.columnNameColumn.Name = "columnNameColumn";
            this.columnNameColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // datatypeColumn
            // 
            this.datatypeColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.datatypeColumn.DataPropertyName = "COLUMN_TYPE";
            resources.ApplyResources(this.datatypeColumn, "datatypeColumn");
            this.datatypeColumn.Name = "datatypeColumn";
            this.datatypeColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.datatypeColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // notNullColumn
            // 
            this.notNullColumn.DataPropertyName = "IS_NULLABLE";
            this.notNullColumn.FalseValue = "YES";
            resources.ApplyResources(this.notNullColumn, "notNullColumn");
            this.notNullColumn.Name = "notNullColumn";
            this.notNullColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.notNullColumn.TrueValue = "NO";
            // 
            // autoIncrementColumn
            // 
            this.autoIncrementColumn.DataPropertyName = "IS_AUTOINCREMENT";
            this.autoIncrementColumn.FalseValue = "NO";
            resources.ApplyResources(this.autoIncrementColumn, "autoIncrementColumn");
            this.autoIncrementColumn.Name = "autoIncrementColumn";
            this.autoIncrementColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.autoIncrementColumn.TrueValue = "YES";
            // 
            // flagsColumn
            // 
            this.flagsColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.flagsColumn, "flagsColumn");
            this.flagsColumn.Name = "flagsColumn";
            // 
            // defaultValueColumn
            // 
            this.defaultValueColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.defaultValueColumn.DataPropertyName = "COLUMN_DEFAULT";
            resources.ApplyResources(this.defaultValueColumn, "defaultValueColumn");
            this.defaultValueColumn.Name = "defaultValueColumn";
            this.defaultValueColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.defaultValueColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // commentColumn
            // 
            this.commentColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.commentColumn.DataPropertyName = "COLUMN_COMMENT";
            resources.ApplyResources(this.commentColumn, "commentColumn");
            this.commentColumn.Name = "commentColumn";
            this.commentColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ordinalPosition
            // 
            resources.ApplyResources(this.ordinalPosition, "ordinalPosition");
            this.ordinalPosition.Name = "ordinalPosition";
            // 
            // columnsFooterTabControl
            // 
            this.columnsFooterTabControl.Controls.Add(this.indexesTabPage);
            this.columnsFooterTabControl.Controls.Add(foreignKeysTabPage);
            this.columnsFooterTabControl.Controls.Add(this.columnDetailsTabPage);
            resources.ApplyResources(this.columnsFooterTabControl, "columnsFooterTabControl");
            this.columnsFooterTabControl.MinimumSize = new System.Drawing.Size(0, 153);
            this.columnsFooterTabControl.Name = "columnsFooterTabControl";
            this.columnsFooterTabControl.SelectedIndex = 0;
            this.columnsFooterTabControl.Deselecting += new System.Windows.Forms.TabControlCancelEventHandler(this.OnFooterTabDeselecting);
            // 
            // indexesTabPage
            // 
            this.indexesTabPage.Controls.Add(this.indexesEdit);
            resources.ApplyResources(this.indexesTabPage, "indexesTabPage");
            this.indexesTabPage.Name = "indexesTabPage";
            this.indexesTabPage.UseVisualStyleBackColor = true;
            // 
            // indexesEdit
            // 
            resources.ApplyResources(this.indexesEdit, "indexesEdit");
            this.indexesEdit.Name = "indexesEdit";
            // 
            // columnDetailsTabPage
            // 
            this.columnDetailsTabPage.Controls.Add(this.columnDetails);
            resources.ApplyResources(this.columnDetailsTabPage, "columnDetailsTabPage");
            this.columnDetailsTabPage.Name = "columnDetailsTabPage";
            this.columnDetailsTabPage.UseVisualStyleBackColor = true;
            // 
            // columnDetails
            // 
            this.columnDetails.ColumnRow = null;
            this.columnDetails.Connection = null;
            resources.ApplyResources(this.columnDetails, "columnDetails");
            this.columnDetails.Name = "columnDetails";
            // 
            // TableEditor
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.columnsSplitContainer);
            this.DoubleBuffered = true;
            this.Name = "TableEditor";
            foreignKeysTabPage.ResumeLayout(false);
            this.columnsSplitContainer.Panel1.ResumeLayout(false);
            this.columnsSplitContainer.Panel2.ResumeLayout(false);
            this.columnsSplitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.columnsDataGrid)).EndInit();
            this.columnsFooterTabControl.ResumeLayout(false);
            this.indexesTabPage.ResumeLayout(false);
            this.columnDetailsTabPage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private SplitContainer columnsSplitContainer;
        private AdvancedDataGridView columnsDataGrid;
        private TabControl columnsFooterTabControl;
        private TabPage columnDetailsTabPage;
        private MySql.Data.VisualStudio.DocumentView.ColumnDetails columnDetails;
        private MySql.Data.VisualStudio.DocumentView.ForeignKeysEdit foreignKeysEdit;
        private TabPage indexesTabPage;
        private IndexesEdit indexesEdit;
        private DataGridViewCheckBoxColumn primaryKeyColumn;
        private DataGridViewTextBoxColumn columnNameColumn;
        private DataGridViewTextBoxColumn datatypeColumn;
        private DataGridViewCheckBoxColumn notNullColumn;
        private DataGridViewCheckBoxColumn autoIncrementColumn;
        private DataGridViewFlagsColumn flagsColumn;
        private DataGridViewTextBoxColumn defaultValueColumn;
        private DataGridViewTextBoxColumn commentColumn;
        private DataGridViewTextBoxColumn ordinalPosition;















    }
}
