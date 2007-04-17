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
 * This file contains implementation of the custom user control for foregin 
 * keys editing.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using ForeignKey = MySql.Data.VisualStudio.Descriptors.ForeignKeyDescriptor.Attributes;
using ForeignKeyColumn = MySql.Data.VisualStudio.Descriptors.ForeignKeyColumnDescriptor.Attributes;
using Table = MySql.Data.VisualStudio.Descriptors.TableDescriptor.Attributes;
using Column = MySql.Data.VisualStudio.Descriptors.ColumnDescriptor.Attributes;
using MySql.Data.VisualStudio.Descriptors;
using MySql.Data.VisualStudio.Utils;
using MySql.Data.VisualStudio.Properties;

namespace MySql.Data.VisualStudio.DocumentView
{
    /// <summary>
    /// This is custom user control for foreign keys editing.
    /// </summary>
    public partial class ForeignKeysEdit : UserControl
    {
        #region Private variables
        /// <summary>Variables to store properties</summary>
        private TableDocument documentRef;
        private DataTable columnsTable;
        private DataTable foreignKeysTable;
        private DataTable foreignKeyColumnsTable;
        private DataConnectionWrapper connectionRef;
        private DataRow selectedKeyRow;

        /// <summary>A manager of key events</summary>
        private KeyEventsManager keyEventsManager;
        #endregion

        #region Initialization
        /// <summary>
        /// Constructor initializes databindings and fills comboboxes with default values.
        /// </summary>
        public ForeignKeysEdit()
        {
            InitializeComponent();

            // Initialize data bindings
            InitBindings();

            // Grid should not generate columns automatically
            foreigKeyColumns.AutoGenerateColumns = false;
            foreigKeyColumns.OrdinalColumn = ordinalColumn;

            // Fill action comboboxes
            onDeleteSelect.Tag = ForeignKey.OnDelete;
            onDeleteSelect.Items.Add(new KeyDisplayValuePair(ForeignKeyDescriptor.NOACTION, Resources.Action_NoAction));
            onDeleteSelect.Items.Add(new KeyDisplayValuePair(ForeignKeyDescriptor.CASCADE, Resources.Action_Cascade));
            onDeleteSelect.Items.Add(new KeyDisplayValuePair(ForeignKeyDescriptor.SETNULL, Resources.Action_SetNull));
            onDeleteSelect.Items.Add(new KeyDisplayValuePair(ForeignKeyDescriptor.RESTRICT, Resources.Action_Restrict));

            onUpdateSelect.Tag = ForeignKey.OnUpdate;
            onUpdateSelect.Items.Add(new KeyDisplayValuePair(ForeignKeyDescriptor.NOACTION, Resources.Action_NoAction));
            onUpdateSelect.Items.Add(new KeyDisplayValuePair(ForeignKeyDescriptor.CASCADE, Resources.Action_Cascade));
            onUpdateSelect.Items.Add(new KeyDisplayValuePair(ForeignKeyDescriptor.SETNULL, Resources.Action_SetNull));
            onUpdateSelect.Items.Add(new KeyDisplayValuePair(ForeignKeyDescriptor.RESTRICT, Resources.Action_Restrict));

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
        /// Returns DataTable with foreign keys.
        /// </summary>
        [Browsable(false)]
        protected DataTable ForeignKeys
        {
            get
            {
                Debug.Assert(foreignKeysTable != null, "Empty foreign keys table!");
                return foreignKeysTable;
            }
        }

        /// <summary>
        /// Returns DataTable with foreign keys columns.
        /// </summary>
        [Browsable(false)]
        protected DataTable ForeignKeysColumns
        {
            get
            {
                Debug.Assert(foreignKeyColumnsTable != null, "Empty foreign keys columns table!");
                return foreignKeyColumnsTable;
            }
        }

        /// <summary>
        /// Gets or sets currently selected foreign key data row.
        /// </summary>
        [Browsable(false)]
        protected DataRow SelectedKey
        {
            get
            {                
                return selectedKeyRow;
            }
            set
            {
                // Skip if not changed or locked
                if (selectedKeyRow == value || lockUpdate)
                    return;

                // If already have selected key, reset view.
                if (selectedKeyRow != null)
                    ResetSelectedKeyView();

                // Store new value
                selectedKeyRow = value;
                
                // If have new selected key, fill view.
                if (selectedKeyRow != null)
                    FillSelectedKeyView();
                else
                    // Disable key settings
                    keySettingsGroup.Enabled = false;
            }
        }

        /// <summary>
        /// Returns name of the selected foreign name.
        /// </summary>
        [Browsable(false)]
        protected string SelectedKeyName
        {
            get
            {
                return keyNameText.Text;
            }
        }

        /// <summary>
        /// Used to lock updates of the selected foreign key.
        /// </summary>
        [Browsable(false)]
        private bool lockUpdate = false;

        /// <summary>
        /// Returns name of the referenced table.
        /// </summary>
        [Browsable(false)]
        protected string ReferencedTable
        {
            get
            {
                return refTableSelect.SelectedValue as string;
            }
        }

        /// <summary>
        /// Returns reference to the connection wrapper, which should be used to
        /// read information about referenced tables.
        /// </summary>
        [Browsable(false)]
        protected DataConnectionWrapper Connection
        {
            get
            {
                Debug.Assert(connectionRef != null, "Empty connection reference!");
                return connectionRef;
            }
        }
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
        }

        /// <summary>
        /// Then document is loaded or saved, extract all necessary data.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Detailed information about event.</param>
        void OnDocumentSuccessfullySaved(object sender, EventArgs e)
        {
            SelectedKey = null;
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
            if (!foreigKeyColumns.EndEdit())
                Debug.Fail("Failed to commit edit!");

            // Validate name
            if (e != null && !e.Cancel)
                e.Cancel = !keyNameText.ValidateAndCopyName();

            // Force columns grid to flush by moving focus away
            if (foreigKeyColumns.ContainsFocus || foreigKeyColumns.EditingControl != null)
                keyNameText.Focus();
        }

        /// <summary>
        /// Extracts data from the document and forces view to refill.
        /// </summary>
        private void ExtractData()
        {
            // Reset current data
            columnsTable = foreignKeysTable = foreignKeyColumnsTable = null;
            connectionRef = null;

            // Reset view
            ResetView();

            // If document is empty, return
            if (Document == null)
                return;

            // Extract all necessary data
            columnsTable = Document.Columns;
            foreignKeysTable = Document.ForeignKeys;
            foreignKeyColumnsTable = Document.ForeignKeysColumns;
            connectionRef = Document.Connection;

            // Validate data
            Debug.Assert(   columnsTable != null && foreignKeysTable != null 
                            && foreignKeyColumnsTable != null && connectionRef != null, 
                            "Failed to extract necessary data!");

            // Fill view with new data
            FillView();
        }

        /// <summary>
        /// Initializes databindings settings.
        /// </summary>
        private void InitBindings()
        {
            // Init key name text
            keyNameText.AttributeName = ForeignKey.Name;

            // Init list display memeber
            foreignKeysList.DisplayMember = ForeignKey.Name;

            // Init referenced table select data bindings
            refTableSelect.DisplayMember = Table.Name;
            refTableSelect.ValueMember = Table.Name;

            // Init source column column data bindings
            sourceColumn.DataPropertyName = ForeignKeyColumn.Name;
            sourceColumn.DisplayMember = Column.Name;
            sourceColumn.ValueMember = Column.Name;

            // Init key name column data bindings
            keyNameColumn.DataPropertyName = ForeignKeyColumn.ForeignKeyName;

            // Init ordinal column
            ordinalColumn.DataPropertyName = ForeignKeyColumn.OrdinalPosition;
        }
        #endregion

        #region View management global
        /// <summary>
        /// Fills view with new extracted data.
        /// </summary>
        private void FillView()
        {
            InitBindings();

            // Lock updates for a while
            lockUpdate = true;

            // Read list of all tables
            refTableSelect.DataSource = ReadAllTables();            

            // Init foreign keys list
            foreignKeysList.DataSource = new DataView(ForeignKeys);

            // Init source column column
            sourceColumn.DataSource = Columns;

            // Unlock updates
            lockUpdate = false;

            // Force selected key changed
            OnSelectedForeignKeyChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Resets view and all underlying data
        /// </summary>
        private void ResetView()
        {
            lockUpdate = true;

            refTableSelect.DataSource = null;            
            sourceColumn.DataSource = null;
            referenceColumn.DataSource = null;            
            foreignKeysList.DataSource = null;

            // Need to keep this table connected for a while, or
            // it will produce noisy warnings.
            //foreigKeyColumns.DataSource = null;
            
            lockUpdate = false;
        }  
        #endregion

        #region View management for selected key
        /// <summary>
        /// Fills details view with data for selected foreign key.
        /// </summary>
        private void FillSelectedKeyView()
        {
            // If selected key is empty, reset view and exit
            if (SelectedKey == null)
            {
                ResetSelectedKeyView();
                return;
            }
            
            // Lock selected key updates
            lockUpdate = true;

            // Enable key settings
            keySettingsGroup.Enabled = true;

            // Fill key name
            keyNameText.DataSource = SelectedKey;

            // Select referenced table
            refTableSelect.SelectedValue = DataInterpreter.GetStringNotNull(SelectedKey, ForeignKey.ReferencedTableName);
            
            // Attache referenced column column to the datasource
            AttachReferencedColumnColumn();

            // Attach grid view to filtered datasource
            AttachForeignKeyColumns();

            // Select proper actions
            onDeleteSelect.SelectedItem = SelectedKey[ForeignKey.OnDelete];
            onUpdateSelect.SelectedItem = SelectedKey[ForeignKey.OnUpdate];
            

            // Unlock selected key updates
            lockUpdate = false;
        }

        /// <summary>
        /// Attaches foreign jey columns table to datasource
        /// </summary>
        private void AttachForeignKeyColumns()
        {
            DataView view = foreigKeyColumns.DataSource is DataView
                ? foreigKeyColumns.DataSource as DataView : new DataView(ForeignKeysColumns);

            if (view.Table != ForeignKeysColumns)
                view.Table = ForeignKeysColumns;

            view.RowFilter = DataInterpreter.BuildFilter(ForeignKeyColumn.ForeignKeyName, SelectedKeyName);

            foreigKeyColumns.DataSource = view;
        }

        /// <summary>
        /// Resets details data.
        /// </summary>
        private void ResetSelectedKeyView()
        {
            // Lock selected key updates
            lockUpdate = true;

            // Reset datasource for referenced columns grid
            foreigKeyColumns.DataSource = null;

            // Reset data source for referenced column column
            referenceColumn.DataSource = null;

            // Reset values to empty
            keyNameText.DataSource = null;
            refTableSelect.SelectedValue = String.Empty;

            // Reset actions
            onDeleteSelect.SelectedItem = null;
            onUpdateSelect.SelectedItem = null;

            // Unlock selected key updates
            lockUpdate = false;
        }

        /// <summary>
        /// Attaches referenced column combobox to the proper datasource with posible 
        /// referenced columns.
        /// </summary>
        private void AttachReferencedColumnColumn()
        {
            if (!String.IsNullOrEmpty(ReferencedTable))
            {
                // Reset data soure before rebind
                referenceColumn.DataSource = null;

                // Restore binding settings
                referenceColumn.DataPropertyName = ForeignKeyColumn.ReferencedColumn;
                referenceColumn.DisplayMember = Column.Name;
                referenceColumn.ValueMember = Column.Name;

                // Set data source for referenced column column
                referenceColumn.DataSource = ColumnDescriptor.Enumerate(
                                                                Connection,
                                                                new object[] { 
                                                                    null, 
                                                                    Connection.Schema, 
                                                                    ReferencedTable });

            }
            else
            {
                // Reset data source for referenced column column
                referenceColumn.DataSource = null;
            }
        }

        #endregion

        #region GUI event handlers
        /// <summary>
        /// Handles changes in the foreign key selection.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Detailed information about event.</param>
        private void OnSelectedForeignKeyChanged(object sender, EventArgs e)
        {
            // If update is loked, exit
            if (lockUpdate)
                return;

            DataRowView rowView = foreignKeysList.SelectedValue as DataRowView;
            SelectedKey = rowView != null ? rowView.Row : null;            
        }

        /// <summary>
        /// Handles referenced table changes and sets correct data source for 
        /// the refernced column column
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Detailed information about event.</param>
        private void OnReferencedTableChanged(object sender, EventArgs e)
        {
            // If update is loked, exit
            if (lockUpdate)
                return;

            // Reset foreign key columns
            DataView view = foreigKeyColumns.DataSource as DataView;
            if (view != null)
                for (int i = view.Count - 1; i >= 0; i--)
                    view.Delete(i);

            // Set new referenced table
            if (SelectedKey != null)
                DataInterpreter.SetValueIfChanged(SelectedKey, ForeignKey.ReferencedTableName, ReferencedTable);

            AttachReferencedColumnColumn();
        }

        /// <summary>
        /// Handles click on the add key button.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Detailed information about event.</param>
        private void OnAddKeyClick(object sender, EventArgs e)
        {
            if (ForeignKeys != null)
            {
                // Create new key
                DataRow newForeignKey = ForeignKeys.NewRow();
                if (newForeignKey == null)
                    return;
                
                // Add new key to table
                ForeignKeys.Rows.Add(newForeignKey);

                // Select new key in the list
                foreignKeysList.SetSelected(foreignKeysList.Items.Count - 1, true);
            }
        }

        /// <summary>
        /// Handles click on the remove key button.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Detailed information about event.</param>
        private void OnRemoveKeyClick(object sender, EventArgs e)
        {
            // Get selected item
            DataRowView item = foreignKeysList.SelectedValue as DataRowView;
            if (item != null)
            {
                // Delete item
                item.Delete();

                // Force selection changed event
                OnSelectedForeignKeyChanged(foreignKeysList, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Handles notification on command combobox selection changed.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Detailed information about event.</param>
        private void OnActionChanged(object sender, EventArgs e)
        {
            // If updates locked or no selected key, ignore event
            if (lockUpdate || SelectedKey == null)
                return;

            // Get and validate sender combobox
            ComboBox cont = sender as ComboBox;
            if (cont == null | !(cont.Tag is string))
            {
                Debug.Fail("Invalid event sender!");
                return;
            }

            // Get slected item and extract value
            KeyDisplayValuePair item = cont.SelectedItem as KeyDisplayValuePair;
            object value = item != null ? item.Key : null;

            // Set attribute
            DataInterpreter.SetValueIfChanged(SelectedKey, cont.Tag as string, value);
        }

        /// <summary>
        /// Locks updates before name will be changed.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Detailed information about event.</param>
        private void OnKeyNameChanging(object sender, EventArgs e)
        {
            // Lock updates for a while
            lockUpdate = true;
        }

        /// <summary>
        /// Then key name is changed, reataches foreign key columns.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Detailed information about event.</param>
        private void OnKeyNameChanged(object sender, EventArgs e)
        {
            // Reatach columns
            AttachForeignKeyColumns();
            // Unlock updates
            lockUpdate = false;
        }

        /// <summary>
        /// Handles request for default values for the new row and sets foreign key 
        /// name and the referenced column name.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Detailed information about event.</param>
        private void OnForeigKeyColumnDefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            // Validate event args
            if (e == null || e.Row == null)
            {
                Debug.Fail("Invalid event args specified!");
                return;
            }

            DataGridViewRow gridRow = e.Row;

            // Change foreign key for new row
            gridRow.Cells[keyNameColumn.Index].Value = SelectedKeyName;

            // Set referenced column name to the referenced table first column name, if any.
            if (referenceColumn.Items.Count >= 1)
            {
                DataRowView viewRow = referenceColumn.Items[0] as DataRowView;
                if (viewRow != null && viewRow.Row != null)
                    gridRow.Cells[referenceColumn.Index].Value = DataInterpreter.GetString(viewRow.Row, Column.Name);
            }
        }

        /// <summary>
        /// Handles data errors and suppress error message from comboboxes columns.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Detailed information about event.</param>
        private void OnForeigKeyColumnsDataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // Only argument exception are considered
            if (e == null || !(e.Exception is ArgumentException))
                return;

            // TODO: May be this is not the best way to suppress exceptions. Beter to find out why they thrown
            // Suppress this exception
            if (e.ColumnIndex == sourceColumn.Index || e.ColumnIndex == referenceColumn.Index)
                e.ThrowException = false;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Returns list of all tables for current schema and adds empty table to that list.
        /// </summary>
        /// <returns>
        /// Returns list of all tables for current schema and adds empty table to that list.
        /// </returns>
        private DataTable ReadAllTables()
        {
            // Enumerate all tables for current schema
            DataTable allTables = TableDescriptor.Enumerate(Connection, new object[] { null, Connection.Schema });
            if (allTables == null)
            {
                Debug.Fail("Failed to enumerate all tables");
                return null;
            }

            // Create new row for empty table value
            /*DataRow emptyTable = allTables.NewRow();
            if (emptyTable == null)
            {
                Debug.Fail("Failed to create new row for empty table");
                return allTables;
            }

            // Set properties for empty table
            emptyTable[Table.Schema] = Connection.Schema;
            emptyTable[Table.Name] = String.Empty;

            // Insert empty table at the begining of all tables and accept changes
            allTables.Rows.InsertAt(emptyTable, 0);
            allTables.AcceptChanges();*/

            // Return result
            return allTables;
        }
        #endregion
    }
}
