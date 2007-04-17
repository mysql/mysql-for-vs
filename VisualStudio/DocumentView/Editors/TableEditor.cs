// Copyright (C) 2006-2007 MySQL AB
//
// This file is part of MySQL Tools for Visual Studio.
// MySQL Tools for Visual Studio is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public 
// License version 2.1 as published by the Free Software Foundation
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA using System;

/*
 * This file contains implementation of the table editor class.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using MySql.Data.VisualStudio;
using MySql.Data.VisualStudio.Descriptors;
using Column = MySql.Data.VisualStudio.Descriptors.ColumnDescriptor.Attributes;
using MySql.Data.VisualStudio.Properties;
using MySql.Data.VisualStudio.Utils;

namespace MySql.Data.VisualStudio.DocumentView
{
    /// <summary>
    /// This is the most complex editor – table editor. It is intended to edit 
    /// all table properties, including columns, indices, foreign keys and 
    /// table options.
    /// </summary>
    [ViewObject(TableDescriptor.TypeName, typeof(TableEditor))]
    public partial class TableEditor : BaseEditor
    {
        #region Initialization
        /// <summary>
        /// Default constructor for table designer
        /// </summary>
        private TableEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructs and initializes editor.
        /// </summary>        
        /// <param name="document">Reference to document object.</param>
        public TableEditor(IDocument document)
            : base(document)
        {
            if (!(document is TableDocument))
                throw new ArgumentException(
                    Resources.Error_UnsupportedDocument,
                    "document");

            InitializeComponent();

            // Set columns grid databindings
            InitializeColumnsGridDatabindings();

            // Change advanced columns grid properties
            AdjustColumnsGridStyle();

            // Initialize column details tab
            InitColumnDetails();

            // Initialize foreign keys tab
            foreignKeysEdit.Document = Document;

            // Initialize indexes tab
            indexesEdit.Document = Document;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Returns typed document object
        /// </summary>
        protected new TableDocument Document
        {
            get
            {
                return base.Document as TableDocument;
            }
        }
        #endregion

        #region Overridings
        /// <summary>
        /// Atachs columns grid to datasource and initialize header
        /// </summary>
        protected override void RefreshView()
        {
            // Extract collumns
            Debug.Assert(Document.Columns != null, "Document is not loaded!");
            if (!Object.ReferenceEquals(columnsDataGrid.DataSource, Document.Columns))
            {
                // Unsubscribe from previous data source events
                if(columnsDataGrid.DataSource is DataTable)
                    (columnsDataGrid.DataSource as DataTable).RowChanged -= new DataRowChangeEventHandler(OnColumnsRowChanged);

                // Set new data source
                columnsDataGrid.DataSource = Document.Columns;

                // Subscribe to new data source events
                Document.Columns.RowChanged += new DataRowChangeEventHandler(OnColumnsRowChanged);
            }
        }

        /// <summary>
        /// Commits changes for columns grid and calls base method.
        /// </summary>
        /// <returns>Returns false if column name validating fails.</returns>
        protected override bool ValidateAndFlushChanges()
        {
            if (!base.ValidateAndFlushChanges())
                return false;

            // Set current state to valid
            isValid = true;

            // Move focus away from grid to force it to save changes
            if(columnsDataGrid.ContainsFocus || columnsDataGrid.EditingControl != null)
                columnsFooterTabControl.Focus();

            // End edit of the columns grid (for some reason doesn't work without line above)
            columnsDataGrid.EndEdit();

            // Valid state may be changed to false by cell validation event handler
            if (!isValid)
                return false;

            return columnDetails.ValidateAndFlushChanges();
        }        
        #endregion

        #region Private methods
        /// <summary>
        /// Initializes columns grid databindings using descriptor information
        /// </summary>
        private void InitializeColumnsGridDatabindings()
        {
            this.primaryKeyColumn.DataPropertyName = Column.IsPrimaryKey;
            this.columnNameColumn.DataPropertyName = Column.Name;
            this.datatypeColumn.DataPropertyName = Column.MySqlType;
            this.notNullColumn.DataPropertyName = Column.Nullable;
            this.autoIncrementColumn.DataPropertyName = Column.IsAutoIncrement;
            this.defaultValueColumn.DataPropertyName = Column.Default;
            this.commentColumn.DataPropertyName = Column.Comments;
            this.ordinalPosition.DataPropertyName = Column.Ordinal;
        }

        /// <summary>
        /// Changes advanced column grid properties, inaccessible via designer.
        /// </summary>
        private void AdjustColumnsGridStyle()
        {
            // Grid should not generate columns automatically
            columnsDataGrid.AutoGenerateColumns = false;

            // Creat smaller font
            Font sourceFont = columnsDataGrid.Font;
            Font smallerFont = new Font("Arial", sourceFont.Size * 0.7F,
                sourceFont.Style, sourceFont.Unit, sourceFont.GdiCharSet,
                sourceFont.GdiVerticalFont);

            // Adjust booleancolumns header font size
            notNullColumn.HeaderCell.Style.Font = smallerFont;
            autoIncrementColumn.HeaderCell.Style.Font = smallerFont;

            // Mark columns which should not display null image
            columnNameColumn.CellTemplate = new DataGridViewNotNullableTextBoxCell();
            datatypeColumn.CellTemplate = new DataGridViewNotNullableTextBoxCell();
            commentColumn.CellTemplate = new DataGridViewNotNullableTextBoxCell();

            // Set ordinal column
            columnsDataGrid.OrdinalColumn = ordinalPosition;

            // Allow to use empty string as default value.
            defaultValueColumn.DefaultCellStyle.NullValue = null;
        }

        /// <summary>
        /// Initializes column details control
        /// </summary>
        private void InitColumnDetails()
        {
            columnDetails.Connection = Document.Connection;
        }
        #endregion

        #region Settings management
        /// <summary>
        /// Loads configuration settings
        /// </summary>
        protected override void LoadSettings()
        {
            base.LoadSettings();

            Settings settings = new Settings();
            columnsFooterTabControl.SelectedIndex = settings.SelectedTabIndex;
            columnsSplitContainer.SplitterDistance = settings.SplitterDistance;
        }

        /// <summary>
        /// Saves configuration settings
        /// </summary>
        protected override void SaveSettings()
        {
            base.SaveSettings();

            Settings settings = new Settings();
            settings.SelectedTabIndex = columnsFooterTabControl.SelectedIndex;
            settings.SplitterDistance = columnsSplitContainer.SplitterDistance;
            settings.Save();
        }
        #endregion

        #region Event handlers
        /// <summary>
        /// Validates a cell of the columns grid
        /// </summary>
        private void OnCellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            // Checking only column names
            if (e.ColumnIndex != columnNameColumn.Index)
                return;

            // Getting a cell value
            DataGridViewCell cell = columnsDataGrid[e.ColumnIndex, e.RowIndex];
            string cellValue = cell.EditedFormattedValue as string;

            // A column name can't be empty
            if (string.IsNullOrEmpty(cellValue))
            {
                e.Cancel = true;
                string errorMessage = String.Format(Resources.Error_EmptyColumnName, Document.Name);
                UIHelper.ShowError(errorMessage);
                isValid = false;
                return;
            }

            // Checking if this cell name duplicates another column name
            foreach (DataGridViewRow row in columnsDataGrid.Rows)
            {
                // Skip current row
                if (row.Index == e.RowIndex)
                    continue;

                // Extract column name
                string columnName = row.Cells[e.ColumnIndex].Value as string;
                if (string.IsNullOrEmpty(columnName))
                    continue;


                if (DataInterpreter.CompareInvariant(cellValue, columnName))
                {
                    e.Cancel = true;
                    string errorMessage = String.Format(Resources.Error_DuplicateColumnName, cellValue);
                    UIHelper.ShowError(errorMessage);
                    isValid = false;
                    return;
                }
            }
        }

        /// <summary>
        /// Handles changing of the data grid selection
        /// </summary>
        /// <param name="sender">Sender fo the event.</param>
        /// <param name="e">Event information</param>
        private void OnColumnsDataGridRowEnter(object sender, DataGridViewCellEventArgs e)
        {
            // Extract new row
            DataGridViewRow row = columnsDataGrid.Rows[e.RowIndex];
            if (row == null)
                return;

            // Set new row to column details.
            DataRowView viewRow = row.DataBoundItem as DataRowView;
            columnDetails.ColumnRow = viewRow != null ? viewRow.Row : null;
        }

        /// <summary>
        /// Handles column row changes and updates flags column.
        /// </summary>
        /// <param name="sender">Object which raised this event.</param>
        /// <param name="e">Detailed information on the event.</param>
        private void OnColumnsRowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e == null || e.Row == null)
                return;

            // Look for related DataGridView row
            foreach (DataGridViewRow gridRow in columnsDataGrid.Rows)
            {
                if (!(gridRow.DataBoundItem is DataRowView)
                    || (gridRow.DataBoundItem as DataRowView).Row != e.Row)
                    continue;

                // If found, repaint and update
                columnsDataGrid.InvalidateCell(flagsColumn.Index, gridRow.Index);
                columnsDataGrid.AutoResizeColumn(flagsColumn.Index);
                break;
            }
        }

        /// <summary>
        /// Handles tab deselection and forces delayed validation.
        /// </summary>
        /// <param name="sender">Sender fo the event.</param>
        /// <param name="e">Event information</param>
        private void OnFooterTabDeselecting(object sender, TabControlCancelEventArgs e)
        {
            // TODO: Validation in such way causes message box to appear twice.
            // Tries to move focus away to force delayed validation
            columnsDataGrid.Focus();

            // If focus was not moved, it means that validation fails and we need to cancel tab changing
            e.Cancel = columnsFooterTabControl.ContainsFocus;
        }
        #endregion

        #region Private variables
        /// <summary>
        /// This flag is used to determine if chached changes are valid.
        /// </summary>
        private bool isValid = true;
        #endregion

        #region Datatype autocomplete
        /// <summary>
        /// Enables autocomplete functionality for datatype column.
        /// </summary>
        /// <param name="sender">Sender fo the event.</param>
        /// <param name="e">Event information</param>
        private void OnEditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            // Validate event data
            if (e == null && !(e.Control is TextBox))
                return;

            // Extract TextBox
            TextBox editControl = e.Control as TextBox;

            // If current cell belongs to datatype column, enable autocomplete, otherwize disable autocomplete.
            if (columnsDataGrid.CurrentCell != null && columnsDataGrid.CurrentCell.ColumnIndex == datatypeColumn.Index)
            {
                // Attach auto-comlete source
                editControl.AutoCompleteCustomSource = new AutoCompleteStringCollection();
                editControl.AutoCompleteCustomSource.AddRange(dataTypes);

                // Set auto-complete source
                editControl.AutoCompleteSource = AutoCompleteSource.CustomSource;

                // Set auto-complete mode
                editControl.AutoCompleteMode = AutoCompleteMode.Suggest;
            }
            else
            {
                editControl.AutoCompleteMode = AutoCompleteMode.None;
            }

        }

        /// <summary>
        /// List of known datatypes
        /// </summary>
        private static readonly string[] dataTypes = new string[] 
        {
            // Numeric types
            Parser.TINYINT.ToLowerInvariant(),
            Parser.SMALLINT.ToLowerInvariant(),
            Parser.MEDIUMINT.ToLowerInvariant(),
            Parser.INT.ToLowerInvariant(),
            Parser.INTEGER.ToLowerInvariant(),
            Parser.BIGINT.ToLowerInvariant(),
            Parser.REAL.ToLowerInvariant(),
            Parser.FLOAT.ToLowerInvariant(),
            Parser.DECIMAL.ToLowerInvariant(),
            Parser.NUMERIC.ToLowerInvariant(),
            Parser.BIT.ToLowerInvariant(),

            // Character types
            Parser.CHAR.ToLowerInvariant(),
            Parser.VARCHAR.ToLowerInvariant() + "(45)",
            Parser.TINYTEXT.ToLowerInvariant(),
            Parser.TEXT.ToLowerInvariant(),
            Parser.MEDIUMTEXT.ToLowerInvariant(),
            Parser.LONGTEXT.ToLowerInvariant(),

            // Binary types
            Parser.BINARY.ToLowerInvariant(),
            Parser.VARBINARY.ToLowerInvariant() + "(256)",
            Parser.TINYBLOB.ToLowerInvariant(),
            Parser.BLOB.ToLowerInvariant(),
            Parser.MEDIUMBLOB.ToLowerInvariant(),
            Parser.LONGBLOB.ToLowerInvariant(),

            // MySQL types
            Parser.ENUM.ToLowerInvariant(),
            Parser.SET.ToLowerInvariant(),

            // Spatial types
            Parser.GEOMETRY.ToLowerInvariant(),
            Parser.POINT.ToLowerInvariant(),
            Parser.LINESTRING.ToLowerInvariant(),
            Parser.POLYGON.ToLowerInvariant(),

            // Spatial collections
            Parser.GEOMETRYCOLLECTION.ToLowerInvariant(),
            Parser.MULTIPOINT.ToLowerInvariant(),
            Parser.MULTILINESTRING.ToLowerInvariant(),
            Parser.MULTIPOLYGON.ToLowerInvariant(),
            
            // Date and time
            Parser.DATE.ToLowerInvariant(),
            Parser.TIME.ToLowerInvariant(),
            Parser.DATETIME.ToLowerInvariant(),
            Parser.TIMESTAMP.ToLowerInvariant()
        }; 
        #endregion
    }
}
