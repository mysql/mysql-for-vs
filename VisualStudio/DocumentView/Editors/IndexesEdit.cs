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
 * This file contains implementation of the custom user control for indexes editing.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Index = MySql.Data.VisualStudio.Descriptors.IndexDescriptor.Attributes;
using IndexColumn = MySql.Data.VisualStudio.Descriptors.IndexColumnDescriptor.Attributes;
using Table = MySql.Data.VisualStudio.Descriptors.TableDescriptor.Attributes;
using Column = MySql.Data.VisualStudio.Descriptors.ColumnDescriptor.Attributes;
using MySql.Data.VisualStudio.Descriptors;
using MySql.Data.VisualStudio.Utils;
using MySql.Data.VisualStudio.Properties;

namespace MySql.Data.VisualStudio.DocumentView
{
    /// <summary>
    /// This is custom user control for indexes editing.
    /// </summary>
    public partial class IndexesEdit : UserControl
    {
        #region Private variables
        /// <summary>Variables to store properties</summary>
        private TableDocument documentRef;
        private DataTable columnsTable;
        private DataTable indexesTable;
        private DataTable indexColumnsTable;
        private DataConnectionWrapper connectionRef;
        private DataRow selectedIndexRow;

        /// <summary>A manager of key events</summary>
        private KeyEventsManager keyEventsManager;
        #endregion

        #region Initialization
        /// <summary>
        /// Constructor initializes databindings and fills comboboxes with default values.
        /// </summary>
        public IndexesEdit()
        {
            InitializeComponent();

            // Initialize data bindings
            InitBindings();

            // Grid should not generate columns automatically
            indexColumns.AutoGenerateColumns = false;
            indexColumns.OrdinalColumn = positionInIndexColumn;

            // Should not show null image at the index length
            columnLengthColumn.CellTemplate = new DataGridViewNotNullableTextBoxCell();

            // Creating a key events manager
            keyEventsManager = new KeyEventsManager(this);
        }       
        #endregion

        #region Data properties
        /// <summary>
        /// Gets or sets reference to the underlying document object. Supposed 
        /// to be set only once.
        /// </summary>
        [Browsable(false)]
        [ReadOnly(true)]
        [DefaultValue(null)]
        public TableDocument Document
        {
            get
            {
                Debug.Assert(DesignMode || documentRef != null, "Document is not set yet!");
                return documentRef;
            }
            set
            {
                Debug.Assert(documentRef == null, "Document is already set!");

                // Store document and call connect to events if it is not empty
                documentRef = value;                
                if (documentRef != null)
                    Connect();
            }
        }

        /// <summary>
        /// Returns DataTable with colums.
        /// </summary>
        [Browsable(false)]
        protected DataTable Columns
        {
            get
            {
                Debug.Assert(columnsTable != null, "Empty columns table!");
                return columnsTable;
            }
        }

        /// <summary>
        /// Returns DataTable with indexes.
        /// </summary>
        [Browsable(false)]
        protected DataTable Indexes
        {
            get
            {
                Debug.Assert(indexesTable != null, "Empty indexes table!");
                return indexesTable;
            }
        }

        /// <summary>
        /// Returns DataTable with index columns.
        /// </summary>
        [Browsable(false)]
        protected DataTable IndexColumns
        {
            get
            {
                Debug.Assert(indexColumnsTable != null, "Empty index columns table!");
                return indexColumnsTable;
            }
        }

        /// <summary>
        /// Gets or sets currently selected index data row.
        /// </summary>
        [Browsable(false)]
        protected DataRow SelectedIndex
        {
            get
            {
                return selectedIndexRow;
            }
            set
            {
                // Skip if not changed or locked
                if (selectedIndexRow == value || lockUpdate)
                    return;

                // If already have selected index, reset view.
                if (selectedIndexRow != null)
                    ResetSelectedIndexView();

                // Store new value
                selectedIndexRow = value;
                
                // If have new selected index, fill view.
                if (selectedIndexRow != null)
                    FillSelectedIndexView();
                else
                    // Disable index settings
                    indexSettingsGroup.Enabled = false;
            }
        }

        /// <summary>
        /// Returns name of the selected index name.
        /// </summary>
        [Browsable(false)]
        protected string SelectedIndexName
        {
            get
            {
                return indexNameText.Text;
            }
        }

        /// <summary>
        /// Used to lock updates of the selected index.
        /// </summary>
        [Browsable(false)]
        private bool lockUpdate = false;
        #endregion

        #region Document event handlers and data extraction
        /// <summary>
        /// Connects to document events.
        /// </summary>
        private void Connect()
        {
            Document.DataLoaded += new EventHandler(OnDocumentLoaded);
            Document.Saving += new CancelEventHandler(OnDocumentSaving);
            Document.SuccessfullySaved += new EventHandler(OnDocumentSuccessfullySaved);
            Document.DataChanged += new EventHandler(OnDocumentDataChanged);
        }

        /// <summary>
        /// Then document is loaded or saved, extract all necessary data.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Detailed information about event.</param>
        void OnDocumentSuccessfullySaved(object sender, EventArgs e)
        {
            ExtractData();            
        }

        /// <summary>
        /// Then document is loaded or saved, extract all necessary data.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Detailed information about event.</param>
        void OnDocumentLoaded(object sender, EventArgs e)
        {
            ExtractData();            
        }

        /// <summary>
        /// Then document is to be saved, flush all chached changes.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Detailed information about event.</param>
        void OnDocumentSaving(object sender, CancelEventArgs e)
        {
            // Accept changes in the grid
            if (!indexColumns.EndEdit())
                Debug.Fail("Failed to commit edit!");

            // Validate index name
            if (e != null && !e.Cancel)
                e.Cancel = !indexNameText.ValidateAndCopyName();

            // Force columns grid to flush by moving focus away
            if (indexColumns.ContainsFocus || indexColumns.EditingControl != null)
                indexNameText.Focus();
        }

        /// <summary>
        /// Refils comboboxes on data change (if engine changed, need to 
        /// change possible type and kind for indexes).
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Detailed information about event.</param>
        void OnDocumentDataChanged(object sender, EventArgs e)
        {
            FillComboboxes();
        }

        /// <summary>
        /// Extracts data from the document and forces view to refill.
        /// </summary>
        private void ExtractData()
        {
            // Reset current data
            columnsTable = indexesTable = indexColumnsTable = null;
            connectionRef = null;

            // Reset view
            ResetView();

            // If document is empty, return
            if (Document == null)
                return;

            // Extract all necessary data
            columnsTable = Document.Columns;
            indexesTable = Document.Indexes;
            indexColumnsTable = Document.IndexColumns;
            connectionRef = Document.Connection;

            // Validate data
            Debug.Assert(   columnsTable != null && indexesTable != null 
                            && indexColumnsTable != null && connectionRef != null, 
                            "Failed to extract necessary data!");

            // Fill view with new data
            FillView();
        }

        /// <summary>
        /// Handles deletion and creation of the index and forces recalculation of the selected index.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Detailed information on event.</param>
        void OnIndexChangedInDataSource(object sender, DataRowChangeEventArgs e)
        {
            if (e == null)
                return;

            switch (e.Action)
            {
                case DataRowAction.Add:
                case DataRowAction.Delete:
                    // Forces selected index canged
                    OnSelectedIndexChanged(this, EventArgs.Empty);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Initializes databindings settings.
        /// </summary>
        private void InitBindings()
        {
            // Init index name text
            indexNameText.AttributeName = Index.Name;

            // Init list display memeber
            indexesList.DisplayMember = Index.Name;


            // Init source column column data bindings
            sourceColumn.DataPropertyName = IndexColumn.Name;
            sourceColumn.DisplayMember = Column.Name;
            sourceColumn.ValueMember = Column.Name;

            // Init index length column
            columnLengthColumn.DataPropertyName = IndexColumn.IndexLength;

            // Init index name column data bindings
            indexNameColumn.DataPropertyName = IndexColumn.Index;

            // Init position in index column data bindings
            positionInIndexColumn.DataPropertyName = IndexColumn.Ordinal;
        }

        /// <summary>
        /// Fills available values for index comboboxes
        /// </summary>
        private void FillComboboxes()
        {
            // Fill index kind combobox
            indexKindSelect.Tag = Index.IndexKind;
            FillCombobox(indexKindSelect, Document.GetSupportedIndexKinds());
            
            // If indexes are not supported at all, disable add button
            bool enabled = indexKindSelect.Items.Count > 0;
            addButton.Enabled = enabled;            

            // If selected index is primary key, add it to the list of available kinds and select
            if (DataInterpreter.CompareInvariant(SelectedIndexName, IndexDescriptor.PRIMARY))
            {
                indexKindSelect.Items.Add(IndexDescriptor.PRIMARY);
                indexKindSelect.SelectedItem = IndexDescriptor.PRIMARY;
            }


            // Fill index types
            indexTypeSelect.Tag = Index.IndexType;
            FillCombobox(indexTypeSelect, Document.GetSupportedIndexTypes());

            // Disable combobox if only one type is supported
            indexTypeSelect.Enabled = indexTypeSelect.Items.Count > 1;
                
        }

        /// <summary>
        /// Fills given combobox with given values and saves selection.
        /// </summary>
        /// <param name="target">Combobox to fill.</param>
        /// <param name="values">Values to use.</param>
        private void FillCombobox(ComboBox target, string[] values)
        {
            // Store selected item
            object selected = target.SelectedItem;

            target.BeginUpdate();

            // Clear collection
            target.Items.Clear();

            // Add kinds if any
            if (values != null)
                target.Items.AddRange(values);
            
            // If stored selected item is valid, store it
            if (selected != null && target.Items.Contains(selected))
                target.SelectedItem = selected;
            // Otherwize select first item
            else if (target.Items.Count > 0)
                target.SelectedItem = target.Items[0];

            target.EndUpdate();
        } 
        #endregion

        #region View management global
        /// <summary>
        /// Fills view with new extracted data.
        /// </summary>
        private void FillView()
        {
            InitBindings();
    
            // Fill comboboxes with data
            FillComboboxes();

            // Subscribe to events
            Indexes.RowChanged += new DataRowChangeEventHandler(OnIndexChangedInDataSource);
            Indexes.RowDeleted += new DataRowChangeEventHandler(OnIndexChangedInDataSource);

            // Init index list
            DataView indexes = new DataView(Indexes);            
            indexesList.DataSource = indexes;

            // Init index column column
            sourceColumn.DataSource = Columns;
        }

        /// <summary>
        /// Resets view and all underlying data
        /// </summary>
        private void ResetView()
        {
            lockUpdate = true;

            // Unsubscribe from events (pre-check is made to avoid assertion warning)
            if (indexesTable != null && Indexes != null)
            {
                Indexes.RowChanged += new DataRowChangeEventHandler(OnIndexChangedInDataSource);
                Indexes.RowDeleted -= new DataRowChangeEventHandler(OnIndexChangedInDataSource);
            }

            sourceColumn.DataSource = null;
            indexesList.DataSource = null;

            // Need to keep this table connected for a while, or
            // it will produce noisy warnings.
            //indexColumnsTable.DataSource = null;
            
            
            lockUpdate = false;
        }  
        #endregion

        #region View management for selected index
        /// <summary>
        /// Fills details view with data for selected index
        /// </summary>
        private void FillSelectedIndexView()
        {
            // If selected index is empty, reset view and exit
            if (SelectedIndex == null)
            {
                ResetSelectedIndexView();
                return;
            }            
            
            // Lock selected index updates
            lockUpdate = true;

            // Fill comboboxes
            FillComboboxes();

            // Fill index name
            indexNameText.DataSource = SelectedIndex;

            // If index is primary, need to disable combo-boxes and text boxes
            bool enable = !DataInterpreter.CompareInvariant(SelectedIndexName, IndexDescriptor.PRIMARY);
            indexNameText.Enabled = enable;
            indexKindSelect.Enabled = enable;
            indexTypeSelect.Enabled = indexTypeSelect.Enabled && enable;
            
            // If not enabled, there is no value for index kind to select and we should add it
            if (!enable && !indexKindSelect.Items.Contains(IndexDescriptor.PRIMARY))
                indexKindSelect.Items.Add(IndexDescriptor.PRIMARY);
            // In other case PRIMARY is not allowed
            else if (indexKindSelect.Items.Contains(IndexDescriptor.PRIMARY))
                indexKindSelect.Items.Remove(IndexDescriptor.PRIMARY);

            // Enable index settings
            indexSettingsGroup.Enabled = true;

            // Attach grid view to filtered datasource
            AttachIndexColumns();

            // Select index kind and type
            indexKindSelect.SelectedItem = SelectedIndex[Index.IndexKind];
            indexTypeSelect.SelectedItem = SelectedIndex[Index.IndexType];            

            // Unlock selected index updates
            lockUpdate = false;
        }

        /// <summary>
        /// Attaches index columns table to datasource
        /// </summary>
        private void AttachIndexColumns()
        {
            DataView view = indexColumns.DataSource is DataView ? indexColumns.DataSource as DataView : new DataView(IndexColumns);
            
            if (view.Table != IndexColumns)
                view.Table = IndexColumns;
            
            view.RowFilter = DataInterpreter.BuildFilter(IndexColumn.Index, SelectedIndexName);
            view.Sort = IndexColumn.Ordinal;
            
            indexColumns.DataSource = view;
        }

        /// <summary>
        /// Resets details data.
        /// </summary>
        private void ResetSelectedIndexView()
        {
            // Lock selected index updates
            lockUpdate = true;

            // Disconect columns data grid
            // TODO: disconnect causes nule reference exception on save!
            //indexColumns.DataSource = null;

            // Reset values to empty
            indexNameText.DataSource = null;

            // Reset actions
            indexKindSelect.SelectedItem = null;
            indexTypeSelect.SelectedItem = null;

            // Unlock selected index updates
            lockUpdate = false;
        }
        #endregion

        #region GUI event handlers
        /// <summary>
        /// Handles changes in the index selection.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Detailed information about event.</param>
        private void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            // If update is loked, exit
            if (lockUpdate)
                return;

            DataRowView rowView = indexesList.SelectedValue as DataRowView;
            SelectedIndex = rowView != null ? rowView.Row : null;            
        }

        /// <summary>
        /// Handles click on the add index button.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Detailed information about event.</param>
        private void OnAddIndexClick(object sender, EventArgs e)
        {
            if (Indexes != null)
            {
                // Create new index
                DataRow newIndex = Indexes.NewRow();
                if (newIndex == null)
                    return;
                
                // Add new index to table
                Indexes.Rows.Add(newIndex);

                // Select new index in the list
                indexesList.SetSelected(indexesList.Items.Count - 1, true);
            }
        }

        /// <summary>
        /// Handles click on the remove index button.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Detailed information about event.</param>
        private void OnRemoveIndexClick(object sender, EventArgs e)
        {
            // Get selected item
            DataRowView item = indexesList.SelectedValue as DataRowView;
            if (item != null)
            {
                // Delete item
                item.Delete();

                // Force selection changed event
                OnSelectedIndexChanged(indexesList, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Locks updates before name will be changed.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Detailed information about event.</param>
        private void OnIndexNameChanging(object sender, EventArgs e)
        {
            // Lock updates for a while
            lockUpdate = true;
        }

        /// <summary>
        /// Then index name is changed, reataches index columns.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Detailed information about event.</param>
        private void OnIndexNameChanged(object sender, EventArgs e)
        {
            // Reatach columns
            AttachIndexColumns();
            // Unlock updates
            lockUpdate = false;
        }
        
        /// <summary>
        /// Handles notification on type or kind combobox selection changed.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Detailed information about event.</param>
        private void OnKindOrTypeChanged(object sender, EventArgs e)
        {
            // If updates locked or no selected index, ignore event
            if (lockUpdate || SelectedIndex == null)
                return;

            // Get and validate sender combobox
            ComboBox cont = sender as ComboBox;
            if (cont == null | !(cont.Tag is string))
            {
                Debug.Fail("Invalid event sender!");
                return;
            }

            // Set attribute
            DataInterpreter.SetValueIfChanged(SelectedIndex, cont.Tag as string, cont.SelectedItem);
        }


        /// <summary>
        /// Handles request for default values for the new row and sets index name.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Detailed information about event.</param>
        private void OnIndexColumnDefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            // Validate event args
            if (e == null || e.Row == null)
            {
                Debug.Fail("Invalid event args specified!");
                return;
            }

            DataGridViewRow gridRow = e.Row;

            // Change index for new row
            gridRow.Cells[indexNameColumn.Index].Value = SelectedIndexName;
        }

        /// <summary>
        /// Handles data errors and suppress error message from comboboxes columns.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Detailed information about event.</param>
        private void OnIndexColumnsDataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // Only argument exception are considered
            if (e == null || !(e.Exception is ArgumentException))
                return;

            // TODO: May be this is not the best way to suppress exceptions. Beter to find out why they thrown
            // Suppress this exception
            if (e.ColumnIndex == sourceColumn.Index)
                e.ThrowException = false;
        }
        #endregion
    }
}
