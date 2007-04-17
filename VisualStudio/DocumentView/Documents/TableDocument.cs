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
 * This file contains implementation of data object for TABLE representation.
 */
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Data;
using Microsoft.VisualStudio.Data;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using MySql.Data.VisualStudio.Utils;
using System.Windows.Forms.Design;
using System.Data.SqlTypes;
using MySql.Data.VisualStudio.Descriptors;
using Table = MySql.Data.VisualStudio.Descriptors.TableDescriptor.Attributes;
using Column = MySql.Data.VisualStudio.Descriptors.ColumnDescriptor.Attributes;
using ForeignKey = MySql.Data.VisualStudio.Descriptors.ForeignKeyDescriptor.Attributes;
using ForeignKeyColumn = MySql.Data.VisualStudio.Descriptors.ForeignKeyColumnDescriptor.Attributes;
using Index = MySql.Data.VisualStudio.Descriptors.IndexDescriptor.Attributes;
using IndexColumn = MySql.Data.VisualStudio.Descriptors.IndexColumnDescriptor.Attributes;
using System.Globalization;
using MySql.Data.VisualStudio.Properties;
using System.Drawing.Design;
using System.Windows.Forms;

namespace MySql.Data.VisualStudio.DocumentView
{
    /// <summary>
    /// This class implements document (data object) functionality and 
    /// represent database table. It implements IVsPersistDocData interface and 
    /// beaves like filelles data object for Visual Studio enviroment.
    /// </summary>
    [DocumentObject(TableDescriptor.TypeName, typeof(TableDocument))]
    public class TableDocument : BaseDocument, CollationConverter.ICharacterSetProvider
    {
        #region Initialization
        /// <summary>
        /// This constructor initialize private identifier variables.
        /// </summary>
        /// <param name="hierarchy">
        /// Data view hierarchy accessor used to interact with Server Explorer. 
        /// Also used to extract connection.
        /// </param>
        /// <param name="id">
        /// Array with the object identifier.
        /// </param>
        /// <param name="isNew">
        /// Indicates if this instance represents new database object doesn’t fixed in 
        /// database yet.
        /// </param>
        public TableDocument(ServerExplorerFacade hierarchy, bool isNew, object[] id)
            :base(hierarchy, isNew, id)
        {
        }
        #endregion        

        #region BaseDocument overridings
        /// <summary>
        /// Dirty flag used to determine whenever data are changed or not. At this 
        /// point table data considered as changed if Columns data table has changes.
        /// </summary>
        protected override bool IsDirty
        {
            get
            {
                // Asks base class
                if (base.IsDirty)
                    return true;

                Debug.Assert(Columns != null, "Collumns are not read!");
                Debug.Assert(ForeignKeys != null, "Foreign keys are not read!");
                Debug.Assert(ForeignKeysColumns != null, "Foreign keys columns are not read!");
                Debug.Assert(Indexes != null, "Indexes are not read!");
                Debug.Assert(IndexColumns != null, "IndexColumns columns are not read!");

                // If we aren't read return true
                if (Columns == null || ForeignKeys == null || ForeignKeysColumns == null
                    || Indexes == null || IndexColumns == null)
                    return false;

                // Check column list and other tables
                if (DataInterpreter.HasChanged(Columns) 
                    || DataInterpreter.HasChanged(ForeignKeys)
                    || DataInterpreter.HasChanged(ForeignKeysColumns)
                    || DataInterpreter.HasChanged(Indexes)
                    || DataInterpreter.HasChanged(IndexColumns))
                    return true; ;
                return false;
            }
        }

        /// <summary>
        /// Returns query for pre-dropping foreign keys and indexes.
        /// </summary>
        /// <returns>Returns query for pre-dropping foreign keys and indexes.</returns>
        protected override string BuildPreDropQuery()
        {
            // Pre-drop foreign keys and indexes if needed
            if (NeedToDropForeignKeys())
                return BuildPreDropForeignKeys();

            return String.Empty;
        }

        /// <summary>
        /// Builds alter query for table.
        /// </summary>
        /// <returns>Alter query for table.</returns>
        protected override string BuildAlterQuery()
        {
            StringBuilder query = new StringBuilder();

             // Write header
            BuildAlterHeader(query);

            BuildAlterSpecifications(query);
            
            return query.ToString();
        }        

        /// <summary>
        /// Builda create query for table.
        /// </summary>
        /// <returns>Create query for table.</returns>
        protected override string BuildCreateQuery()
        {
            StringBuilder query = new StringBuilder();
            BuildCreateHeader(query);
            query.Append(" (");
            BuildCreateDefinition(query);
            query.Append(" )");
            BuildTableOptions(query, null);
            return query.ToString();
        }

        /// <summary>
        /// Load database object from database.
        /// </summary>
        /// <param name="reloading">
        /// This flag indicates that object is reloading. Should be ignored in most cases.
        /// </param>
        /// <returns>Returns true if load succeeds and false otherwise.</returns>
        protected override bool LoadData(bool reloading)
        {
            // Read table atributes using base method
            if (!base.LoadData(reloading))
                return false;

            // Reset current columns table, if any.
            if (columnsTable != null)
                ResetColumnsTable();

            // Reset foreign keys table, if any
            if (foreignKeysTable != null)
                ResetForeignKeysTable();

            // Reset foreign keys columns table, if any
            if (foreignKeysColumnsTable != null)
                ResetForeignKeysColumnsTable();

            // Reset indexes table, if any
            if (indexesTable != null)
                ResetIndexesTable();

            // Reset indexes columns table, if any
            if (indexesTable != null)
                ResetIndexesColumnsTable();
            
            // Read columns for table
            columnsTable = ColumnDescriptor.Enumerate(Connection, ObjectIDForLoad);
            if (columnsTable == null)
            {
                Debug.Fail("Failed to read columns!");
                return false;
            }
            // Subscribe to new columns table events
            SubscribeToColumnsTableEvents();

            // Read foreign keys for table.
            foreignKeysTable = ForeignKeyDescriptor.Enumerate(Connection, ObjectIDForLoad);
            if (foreignKeysTable == null)
            {
                Debug.Fail("Failed to read foreign keys!");
                return false;
            }
            // Subscrube to events
            SubscribeToForeignKeysTableEvents();            

            // Read foreign keys columns for table.
            foreignKeysColumnsTable = ForeignKeyColumnDescriptor.Enumerate(Connection, ObjectIDForLoad);
            if (foreignKeysColumnsTable == null)
            {
                Debug.Fail("Failed to read foreign keys columns!");
                return false;
            }
            // Subscrube to events
            SubscribeToForeignKeysColumnsTableEvents();

            // Read indexes for table.
            indexesTable = IndexDescriptor.Enumerate(Connection, ObjectIDForLoad);
            if (indexesTable == null)
            {
                Debug.Fail("Failed to read indexes!");
                return false;
            }
            // Subscrube to events
            SubscribeToIndexesTableEvents();

            // Read index columns for table.
            indexColumnsTable = IndexColumnDescriptor.Enumerate(Connection, ObjectIDForLoad);
            if (indexColumnsTable == null)
            {
                Debug.Fail("Failed to read index columns!");
                return false;
            }
            // Subscrube to events
            SubscribeToIndexesColumnsTableEvents();

            return true;
        }

        /// <summary>
        /// Accepts changes in column grid.
        /// </summary>
        protected override void AcceptChanges()
        {
            base.AcceptChanges();
            
            // Accepting columns changes
            columnsTable.AcceptChanges();
        }

        /// <summary>
        /// Fills aditional properties for new table.
        /// </summary>
        /// <param name="newRow">DataRow to fill with properties.</param>
        protected override void FillNewObjectAttributes(DataRow newRow)
        {
            base.FillNewObjectAttributes(newRow);
            newRow[Table.Engine] = Connection.DefaultEngine;
            newRow[Table.CharacterSet] = Connection.DefaultCharacterSet;
            newRow[Table.Collation] = Connection.DefaultCollation;
        }

        /// <summary>
        /// Resets data for the cloned table.
        /// </summary>
        protected override void ResetToNew()
        {
            // Call to base
            base.ResetToNew();

            // Validate columns and other data (must be loaded)
            if (Columns == null || ForeignKeys == null || ForeignKeysColumns == null
                || Indexes == null || IndexColumns == null)
                return;

            //Reset columns
            DataTable temp = MakeCopy(Columns);
            ResetTableName(temp, Column.Table);
            ResetColumnsTable();
            columnsTable = temp;
            SubscribeToColumnsTableEvents();

            //Reset foreign keys
            temp = MakeCopy(ForeignKeys);
            ResetTableName(temp, ForeignKey.Table);
            // For each foreign key we need new name (some times InnoDB falls if keys have same name 
            // even in the different tables)
            foreach (DataRow key in temp.Rows)
                GenerateNewName(key);
            ResetForeignKeysTable();
            foreignKeysTable = temp;
            SubscribeToForeignKeysTableEvents();

            //Reset foreign key columns
            temp = MakeCopy(ForeignKeysColumns);
            ResetTableName(temp, ForeignKeyColumn.Table);
            ResetForeignKeysColumnsTable();
            foreignKeysColumnsTable = temp;
            SubscribeToForeignKeysColumnsTableEvents();

            //Reset indexes
            temp = MakeCopy(Indexes);
            ResetTableName(temp, Index.Table);
            ResetIndexesTable();
            indexesTable = temp;
            SubscribeToIndexesTableEvents();

            //Reset indexes columns
            temp = MakeCopy(IndexColumns);
            ResetTableName(temp, IndexColumn.Table);
            ResetIndexesColumnsTable();
            indexColumnsTable = temp;
            SubscribeToIndexesColumnsTableEvents();
        }
        
        /// <summary>
        /// Processes failures on save. May remove pre-dropped foreign keys and indexes
        /// from tables.
        /// </summary>
        protected override void SaveFailed()
        {
            base.SaveFailed();

            // If query was with two parts and at first part we droped foreign keys
            // we must check all keys and indexes for existens
            if (!NeedToDropForeignKeys())
            {
                // Call to base
                CallBaseSaveFailed();
                return;
            }

            // Flag to indicate that we droped somthing
            bool dropped = false;

            // Enumerate current foreign keys
            DataTable currentKeys = ForeignKeyDescriptor.Enumerate(Connection, OldObjectID);
            Debug.Assert(currentKeys != null && currentKeys.Rows != null, "Failed to re-enumerate foreign keys");
            if (currentKeys != null && currentKeys.Rows != null)
            {
                foreach (DataRow key in ForeignKeys.Select())
                {
                    switch (key.RowState)
                    {
                        // Skip added and deleted keys                    
                        case DataRowState.Added:
                        case DataRowState.Deleted:
                            break;
                        // Check if this key was changed and pre-droped
                        default:
                            if (HasForeignKeyChanged(key))
                            {
                                DataRow currentKey = currentKeys.Rows.Find(new object[] { 
                                                            Schema, 
                                                            OldName, 
                                                            DataInterpreter.GetString(key, ForeignKey.Name) });
                                // If no current key founded, mark it as new
                                if (currentKey == null)
                                {
                                    dropped = true;
                                    key.AcceptChanges();
                                    key.SetAdded();
                                }
                            }
                            break;
                    }
                }
            }

            // Enumerate current indexes
            DataTable currentIndexes = IndexDescriptor.Enumerate(Connection, OldObjectID);
            Debug.Assert(currentIndexes != null && currentIndexes.Rows != null, "Failed to re-enumerate indexes");
            if (currentIndexes != null && currentIndexes.Rows != null)
            {
                foreach (DataRow index in Indexes.Select())
                {
                    switch (index.RowState)
                    {
                        // Skip added and deleted indexes                    
                        case DataRowState.Added:
                        case DataRowState.Deleted:
                            break;
                        // Check if this index was changed and pre-droped
                        default:
                            DataRow currentIndex = currentIndexes.Rows.Find(new object[] { 
                                                        Schema, 
                                                        OldName, 
                                                        DataInterpreter.GetString(index, Index.Name) });
                            // If no current index founded, mark it as new
                            if (currentIndex == null)
                            {
                                dropped = true;
                                index.AcceptChanges();
                                index.SetAdded();
                            }
                            break;

                    }
                }
            }

            // Call to base
            CallBaseSaveFailed();

            // Warn user if key or index was pre-droped
            if (dropped)
                UIHelper.ShowWarning(Resources.Warning_KeyWasPredropped);            
        }
        #endregion

        #region Index changes handling
        /// <summary>
        /// Unsubscribe from old indexes table event
        /// </summary>
        private void ResetIndexesTable()
        {
            indexesTable.TableNewRow -= new DataTableNewRowEventHandler(OnNewIndex);
            indexesTable.RowChanged -= new DataRowChangeEventHandler(OnIndexRowChanged);
            indexesTable.RowDeleted -= new DataRowChangeEventHandler(OnIndexRowDeleted);
            indexesTable.Dispose();
            indexesTable = null;
        }

        /// <summary>
        /// Subscribe to new index table events
        /// </summary>
        private void SubscribeToIndexesTableEvents()
        {
            indexesTable.TableNewRow += new DataTableNewRowEventHandler(OnNewIndex);
            indexesTable.RowChanged += new DataRowChangeEventHandler(OnIndexRowChanged);
            indexesTable.RowDeleted += new DataRowChangeEventHandler(OnIndexRowDeleted);
        }

        /// <summary>
        /// Handles chnages for the index rows.
        /// </summary>
        /// <param name="sender">Event sender, unused.</param>
        /// <param name="e">Detailed information about event</param>>
        void OnIndexRowChanged(object sender, DataRowChangeEventArgs e)
        {
            Debug.Assert(e != null && e.Row != null, "Empty event argumets provided!");

            // Extract changed index
            DataRow index = e.Row;
            if (index == null)
                return;

            // Chose right method for action
            switch (e.Action)
            {
                case DataRowAction.Change:
                    HandleIndexChanges(index);
                    break;
                default: break;
            }
        }

        /// <summary>
        /// Handles adding new index.
        /// </summary>
        /// <param name="sender">Event sender, unused.</param>
        /// <param name="e">Detailed information about event.</param>
        void OnNewIndex(object sender, DataTableNewRowEventArgs e)
        {
            Debug.Assert(e != null && e.Row != null, "Empty event argumets provided!");

            // Extract added index
            DataRow newIndex = e.Row;
            if (newIndex == null)
                return;

            // Initialize new column attributes
            newIndex[Index.Schema] = Schema;
            newIndex[Index.Table] = OldName; // Old name is used to keep table name for all indexes the same.
            newIndex[Index.Name] = BuildNewIndexName();
            newIndex[Index.IndexKind] = IndexDescriptor.INDEX;
            newIndex[Index.IndexType] = IndexDescriptor.BTREE;
        }

        /// <summary>
        /// If index changed, ensures that all index columns will have right index name.
        /// </summary>
        /// <param name="index">DataRow with index data.</param>
        private void HandleIndexChanges(DataRow index)
        {
            string newName = DataInterpreter.GetStringNotNull(index, Index.Name);

            // Iterate through index columns
            string indexName;
            foreach (DataRow column in IndexColumns.Rows)
            {
                // Need to skip deleted columns if any
                if (column.RowState == DataRowState.Deleted)
                    continue;

                // Extract index name
                indexName = DataInterpreter.GetStringNotNull(column, IndexColumn.Index);

                // Check if any index with this name (search for index with given schema, table and name)
                DataRow candidate = FindIndex(indexName);

                // If index was not found, need to change index name for index column.
                if (candidate == null)
                    column[IndexColumn.Index] = newName;
            }
        }

        /// <summary>
        /// Handles indeex deletion and delete all index columns.
        /// </summary>        
        /// <param name="sender">Event sender, unused.</param>
        /// <param name="e">Detailed information about event</param>>
        private void OnIndexRowDeleted(object sender, DataRowChangeEventArgs e)
        {
            string indexName;
            foreach (DataRow column in IndexColumns.Select())
            {
                // Need to skip deleted columns if any
                if (column.RowState == DataRowState.Deleted)
                    continue;

                // Extract index name
                indexName = DataInterpreter.GetStringNotNull(column, IndexColumn.Index);

                // Check if any index with this name (search for index with given schema, table and name)
                DataRow candidate = FindIndex(indexName);

                // If index was not found, need to delete index column.
                if (candidate == null)
                    column.Delete();
            }
        }

        /// <summary>
        /// Builds name for the new new index in format Index_N.
        /// </summary>
        /// <returns>Returns name for the new new index in format Index_N.</returns>
        private string BuildNewIndexName()
        {
            // Initialize search data
            int count = 0; string result; DataRow[] existsIndexes = null;

            // Generate new index name
            do
            {
                result = String.Format(CultureInfo.CurrentCulture, Resources.New_Index_Template, ++count);
                existsIndexes = DataInterpreter.Select(Indexes, Index.Name, result);
            }
            while (existsIndexes == null || existsIndexes.Length > 0);

            // Return results
            return result;
        }
        #endregion

        #region Indexes columns changes handling
        /// <summary>
        /// Unsubscribe from old indexes columns table event
        /// </summary>
        private void ResetIndexesColumnsTable()
        {
            indexColumnsTable.TableNewRow -= new DataTableNewRowEventHandler(OnNewIndexColumn);
            indexColumnsTable.RowChanged -= new DataRowChangeEventHandler(OnIndexColumnChanged);
            indexColumnsTable.RowDeleted -= new DataRowChangeEventHandler(OnIndexColumnChanged);
            indexColumnsTable.Dispose();
            indexColumnsTable = null;
        }

        /// <summary>
        /// Subscribe to new indexes columns table events
        /// </summary>
        private void SubscribeToIndexesColumnsTableEvents()
        {
            indexColumnsTable.TableNewRow += new DataTableNewRowEventHandler(OnNewIndexColumn);
            indexColumnsTable.RowChanged += new DataRowChangeEventHandler(OnIndexColumnChanged);
            indexColumnsTable.RowDeleted += new DataRowChangeEventHandler(OnIndexColumnChanged);
        }

        /// <summary>
        /// Handles adding new indes collumn.
        /// </summary>
        /// <param name="sender">Event sender, unused.</param>
        /// <param name="e">Detailed information about event.</param>
        void OnNewIndexColumn(object sender, DataTableNewRowEventArgs e)
        {
            Debug.Assert(e != null && e.Row != null, "Empty event argumets provided!");

            // Extract added index column
            DataRow newColumn = e.Row;
            if (newColumn == null)
                return;

            // Initialize new column attributes
            newColumn[IndexColumn.Schema] = Schema;
            newColumn[IndexColumn.Table] = OldName; // Old name is used to keep table name for all columns the same.

            // Set index name to empty string
            newColumn[IndexColumn.Index] = String.Empty;

            // Search for any not deleted column and set name for index column
            DataRow dataColumn = DataInterpreter.GetNotDeletedRow(Columns);
            newColumn[IndexColumn.Name] =   dataColumn != null
                                            ? DataInterpreter.GetStringNotNull(dataColumn, Column.Name)
                                            : String.Empty;

        }

        /// <summary>
        /// Handles index column changes and deletions. Adjust table columns primary key status.
        /// </summary>
        /// <param name="sender">Event sender, unused.</param>
        /// <param name="e">Detailed information about event.</param>
        void OnIndexColumnChanged(object sender, DataRowChangeEventArgs e)
        {
            Debug.Assert(e != null && e.Row != null, "Empty event argumets provided!");

            // Extract added column
            DataRow column = e.Row;
            if (column == null)
                return;

            // Chose right method for action
            switch (e.Action)
            {
                case DataRowAction.Add:
                case DataRowAction.Change:
                    HandleIndexColumnChanges(column);
                    break;
                case DataRowAction.Delete:
                    HandleIndexColumnDeleted();
                    break;
                default: break;
            }
        }

        /// <summary>
        /// Then primary key index column is deleted, ensures that table column will be unmarked
        /// as primary key.
        /// </summary>
        private void HandleIndexColumnDeleted()
        {
            // Check each primary key column
            foreach (DataRow column in DataInterpreter.Select(Columns, Column.IsPrimaryKey, DataInterpreter.True))
            {
                // Skip deleted columns
                if (column.RowState == DataRowState.Deleted)
                    continue;
                
                // Ensure that index column stil exists
                DataRow indexColumn = FindIndexColumn(IndexDescriptor.PRIMARY, DataInterpreter.GetStringNotNull(column, Column.Name));
                if(indexColumn == null)
                    DataInterpreter.SetValueIfChanged(column, Column.IsPrimaryKey, DataInterpreter.False);
            }
        }

        /// <summary>
        /// Handles index column changes and mark/unmark primary key columns.
        /// </summary>
        /// <param name="column">DataRow with index column data.</param>
        private void HandleIndexColumnChanges(DataRow column)
        {
            // Extract index name
            string indexName = DataInterpreter.GetStringNotNull(column, IndexColumn.Index);
            // Extract related table column
            DataRow tableColumn = FindColumn(DataInterpreter.GetStringNotNull(column, IndexColumn.Name));
            if (tableColumn == null)
                return;

            // If index is primary key, mark column as primary key
            if (DataInterpreter.CompareInvariant(indexName, IndexDescriptor.PRIMARY))
                DataInterpreter.SetValueIfChanged(tableColumn, Column.IsPrimaryKey, DataInterpreter.True);
        }
        #endregion

        #region Foreign keys changes handling
        /// <summary>
        /// Unsubscribe from old foreign keys columns table event
        /// </summary>
        private void ResetForeignKeysTable()
        {
            foreignKeysTable.TableNewRow -= new DataTableNewRowEventHandler(OnNewForeignKey);
            foreignKeysTable.RowChanged -= new DataRowChangeEventHandler(OnForeignKeyRowChanged);
            foreignKeysTable.RowDeleted -= new DataRowChangeEventHandler(OnForeignKeyRowDeleted);
            foreignKeysTable.Dispose();
            foreignKeysTable = null;
        }

        /// <summary>
        /// Subscribe to new foreign keys columns table events
        /// </summary>
        private void SubscribeToForeignKeysTableEvents()
        {
            foreignKeysTable.TableNewRow += new DataTableNewRowEventHandler(OnNewForeignKey);
            foreignKeysTable.RowChanged += new DataRowChangeEventHandler(OnForeignKeyRowChanged);
            foreignKeysTable.RowDeleted += new DataRowChangeEventHandler(OnForeignKeyRowDeleted);
        }

        /// <summary>
        /// Handles chnages for the column rows.
        /// </summary>
        /// <param name="sender">Event sender, unused.</param>
        /// <param name="e">Detailed information about event</param>>
        void OnForeignKeyRowChanged(object sender, DataRowChangeEventArgs e)
        {
            Debug.Assert(e != null && e.Row != null, "Empty event argumets provided!");

            // Extract changed key
            DataRow key = e.Row;
            if (key == null)
                return;

            // Chose right method for action
            switch (e.Action)
            {
                case DataRowAction.Change:
                    HandleKeyChanges(key);
                    break;
                default: break;
            }
        }

        /// <summary>
        /// Handles adding new key.
        /// </summary>
        /// <param name="sender">Event sender, unused.</param>
        /// <param name="e">Detailed information about event.</param>
        void OnNewForeignKey(object sender, DataTableNewRowEventArgs e)
        {
            Debug.Assert(e != null && e.Row != null, "Empty event argumets provided!");

            // Extract added foreign key column
            DataRow newKey = e.Row;
            if (newKey == null)
                return;

            // Initialize new column attributes
            newKey[ForeignKey.Schema] = Schema;
            newKey[ForeignKey.Table] = OldName; // Old name is used to keep table name for all keys the same.
            newKey[ForeignKey.Name] = BuildNewKeyName();
            newKey[ForeignKey.OnDelete] = ForeignKeyDescriptor.RESTRICT;
            newKey[ForeignKey.OnUpdate] = ForeignKeyDescriptor.RESTRICT;
        }

        /// <summary>
        /// If foreign key changed, ensures that all foreign key columns will have right 
        /// key name.
        /// </summary>
        /// <param name="key">DataRow with foreign key data.</param>
        private void HandleKeyChanges(DataRow key)
        {
            string newName = DataInterpreter.GetStringNotNull(key, ForeignKey.Name);

            // Iterate through foreign keys columns
            string fkName;
            foreach (DataRow column in ForeignKeysColumns.Rows)
            {
                // Need to skip deleted columns if any
                if (column.RowState == DataRowState.Deleted)
                    continue;

                // Extract foreign key name
                fkName = DataInterpreter.GetStringNotNull(column, ForeignKeyColumn.ForeignKeyName);

                // Check if any foreign key with this name (search for key with given schema, table and name)
                DataRow candidate = FindForeignKey(fkName);

                // If key was not found, need to change key name for foreign key column.
                if (candidate == null)
                    column[ForeignKeyColumn.ForeignKeyName] = newName;
            }
        }

        /// <summary>
        /// Handles foreign key deletion and delete all foreign key columns.
        /// </summary>        
        /// <param name="sender">Event sender, unused.</param>
        /// <param name="e">Detailed information about event</param>>
        private void OnForeignKeyRowDeleted(object sender, DataRowChangeEventArgs e)
        {
            string fkName;
            foreach (DataRow column in ForeignKeysColumns.Select())
            {
                // Need to skip deleted columns if any
                if (column.RowState == DataRowState.Deleted)
                    continue;

                // Extract foreign key name
                fkName = DataInterpreter.GetStringNotNull(column, ForeignKeyColumn.ForeignKeyName);

                // Check if any foreign key with this name (search for key with given schema, table and name)
                DataRow candidate = FindForeignKey(fkName);

                // If key was not found, need to delete foreign key column.
                if (candidate == null)
                    column.Delete();
            }
        }

        /// <summary>
        /// Builds name for the new foreign key in format FK_{table name}_N.
        /// </summary>
        /// <returns>Returns name for the new foreign key in format FK_{table name}_N.</returns>
        private string BuildNewKeyName()
        {
            // Initialize searche data
            int count = 0; string result; DataRow[] existsKeys = null;

            // Generate new column name
            do
            {
                result = String.Format(CultureInfo.CurrentCulture, Resources.New_ForeignKey_Template, Name, ++count);
                existsKeys = DataInterpreter.Select(ForeignKeys, ForeignKey.Name, result);
            }
            while (existsKeys == null || existsKeys.Length > 0);

            // Return results
            return result;
        }
        #endregion

        #region Foreign keys column changes handling
        /// <summary>
        /// Unsubscribe from old foreign keys columns table event
        /// </summary>
        private void ResetForeignKeysColumnsTable()
        {
            foreignKeysColumnsTable.TableNewRow -= new DataTableNewRowEventHandler(OnNewForeignKeyColumn);
            foreignKeysColumnsTable.Dispose();
            foreignKeysColumnsTable = null;
        }

        /// <summary>
        /// Subscribe to new foreign keys columns table events
        /// </summary>
        private void SubscribeToForeignKeysColumnsTableEvents()
        {
            foreignKeysColumnsTable.TableNewRow += new DataTableNewRowEventHandler(OnNewForeignKeyColumn);
        }

        /// <summary>
        /// Handles adding new foreign key collumn.
        /// </summary>
        /// <param name="sender">Event sender, unused.</param>
        /// <param name="e">Detailed information about event.</param>
        void OnNewForeignKeyColumn(object sender, DataTableNewRowEventArgs e)
        {
            Debug.Assert(e != null && e.Row != null, "Empty event argumets provided!");

            // Extract added foreign key column
            DataRow newColumn = e.Row;
            if (newColumn == null)
                return;

            // Initialize new column attributes
            newColumn[ForeignKeyColumn.Schema] = Schema;
            newColumn[ForeignKeyColumn.Table] = OldName; // Old name is used to keep table name for all columns the same.

            // Set foreign key name to empty string
            newColumn[ForeignKeyColumn.ForeignKeyName] = String.Empty;
            
            // Search for any not deleted column and set name for foreign key column
            DataRow dataColumn = DataInterpreter.GetNotDeletedRow(Columns);
            newColumn[ForeignKeyColumn.Name] =  dataColumn != null 
                                                ? DataInterpreter.GetStringNotNull(dataColumn, Column.Name)
                                                : String.Empty;
        }
        #endregion

        #region Columns changes handling
        /// <summary>
        /// Unsubscribe from old columns table event
        /// </summary>
        private void ResetColumnsTable()
        {
            columnsTable.TableNewRow -= new DataTableNewRowEventHandler(OnNewColumnRow);
            columnsTable.RowChanged -= new DataRowChangeEventHandler(OnColumnRowChanged);
            columnsTable.RowDeleted -= new DataRowChangeEventHandler(OnColumnDeleted);
            columnsTable.Dispose();
            columnsTable = null;

            // Clear column defaults dictionary
            columnDefaults.Clear();
        }

        /// <summary>
        /// Subscribe to new columns table events
        /// </summary>
        private void SubscribeToColumnsTableEvents()
        {
            columnsTable.TableNewRow += new DataTableNewRowEventHandler(OnNewColumnRow);
            columnsTable.RowChanged += new DataRowChangeEventHandler(OnColumnRowChanged);
            columnsTable.RowDeleted += new DataRowChangeEventHandler(OnColumnDeleted);
           
            // Add default value for each column into defaluts dictionary
            foreach (DataRow column in columnsTable.Rows)
                if (column != null && column.HasVersion(DataRowVersion.Current))
                    columnDefaults[DataInterpreter.GetStringNotNull(column, Column.Name)]
                        = DataInterpreter.GetString(column, Column.Default);
        }

        /// <summary>
        /// Handles chnages for the column rows.
        /// </summary>
        /// <param name="sender">Event sender, unused.</param>
        /// <param name="e">Detailed information about event</param>>
        void OnColumnRowChanged(object sender, DataRowChangeEventArgs e)
        {
            Debug.Assert(e != null && e.Row != null, "Empty event argumets provided!");

            // Extract added column
            DataRow column = e.Row;
            if (column == null)
                return;

            // Chose right method for action
            switch (e.Action)
            {
                case DataRowAction.Add:
                    CompleteNewCollumn(column);
                    HandleColumnChanges(column);
                    break;
                case DataRowAction.Change:
                    HandleColumnChanges(column);
                    break;
                default: break;
            }
        }

        /// <summary>
        /// Handles adding new collumn.
        /// </summary>
        /// <param name="sender">Event sender, unused.</param>
        /// <param name="e">Detailed information about event.</param>
        private void OnNewColumnRow(object sender, DataTableNewRowEventArgs e)
        {
            Debug.Assert(e != null && e.Row != null, "Empty event argumets provided!");
            
            // Extract added column
            DataRow newColumn = e.Row;
            if (newColumn == null)
                return;

            // Initialize new column attributes
            newColumn[Column.Schema] = Schema;
            newColumn[Column.Table] = OldName; // Old name is used to keep table name for all columns the same.
            newColumn[Column.Name] = BuildNewColumnName();
        }

        /// <summary>
        /// Handles chenges in the column attributes.
        /// </summary>
        /// <param name="column">Data row with column attributes.</param>
        private void HandleColumnChanges(DataRow column)
        {            
            string newName = DataInterpreter.GetStringNotNull(column, Column.Name);

            // Iterate through foreign keys columns
            string fkName;
            foreach (DataRow fkColumn in ForeignKeysColumns.Rows)
            {
                // Need to skip deleted columns if any
                if (fkColumn.RowState == DataRowState.Deleted)
                    continue;

                fkName = DataInterpreter.GetStringNotNull(fkColumn, ForeignKeyColumn.Name);

                // Check if any data column with proper name (search for columns with given schema, table and name)
                DataRow candidate = FindColumn(fkName);

                // If column was not found, need to change column name for foreign key column.
                if (candidate == null)
                    fkColumn[ForeignKeyColumn.Name] = newName;

            }

            // Iterate through indexes columns
            string indexName;
            foreach (DataRow indexColumn in IndexColumns.Rows)
            {
                // Need to skip deleted columns if any
                if (indexColumn.RowState == DataRowState.Deleted)
                    continue;

                indexName = DataInterpreter.GetStringNotNull(indexColumn, IndexColumn.Name);

                // Check if any data column with proper name (search for columns with given schema, table and name)
                DataRow candidate = FindColumn(indexName);

                // If column was not found, need to change column name for index column.
                if (candidate == null)
                    indexColumn[IndexColumn.Name] = newName;
            }

            // If column is primary key, should be index column in index PRIMARY
            if (DataInterpreter.GetSqlBool(column, Column.IsPrimaryKey).IsTrue)
                IncludeInPrimaryKey(newName);
            // If column is not primary key, exclude it from primary key index
            if (DataInterpreter.GetSqlBool(column, Column.IsPrimaryKey).IsFalse)
                ExcludeFromPrimaryKey(newName);

            // If default value is null, reset nullable flag (only for not auto increment columns)
            if (!DataInterpreter.IsNotNull(column, Column.Default)
                && DataInterpreter.GetSqlBool(column, Column.IsAutoIncrement).IsFalse)
            {
                object oldDefault = null;
                // If previous default value is stored and it is not null, reset allow nulls flag
                if (columnDefaults.TryGetValue(DataInterpreter.GetStringNotNull(column, Column.Name), out oldDefault)
                    && oldDefault != null)
                    DataInterpreter.SetValueIfChanged(column, Column.Nullable, DataInterpreter.True);
            }
            // If not null flag set, reset default value to empty string (only for not auto increment columns)
            if (DataInterpreter.GetSqlBool(column, Column.Nullable).IsFalse
                && !DataInterpreter.IsNotNull(column, Column.Default)
                && DataInterpreter.GetSqlBool(column, Column.IsAutoIncrement).IsFalse)
                SetDefault(column);

            // Stores old default
            columnDefaults[DataInterpreter.GetStringNotNull(column, Column.Name)] 
                = DataInterpreter.GetString(column, Column.Default);
        }

        /// <summary>
        /// Includes given column into primary key.
        /// </summary>
        /// <param name="newName">Name of column to include in primary key.</param>
        private void IncludeInPrimaryKey(string newName)
        {
            // Create primary key index if not created yet
            if (FindIndex(IndexDescriptor.PRIMARY) == null)
            {
                DataRow primaryKey = Indexes.NewRow();
                primaryKey[Index.Name] = IndexDescriptor.PRIMARY;
                primaryKey[Index.IndexKind] = IndexDescriptor.PRIMARY;
                Indexes.Rows.Add(primaryKey);
            }

            // Extract exists primary key columns
            DataRow[] columns = DataInterpreter.Select(
                                    IndexColumns, 
                                    IndexColumn.Index, 
                                    IndexDescriptor.PRIMARY);

            // Create primary key column if not exists
            if (FindIndexColumn(IndexDescriptor.PRIMARY, newName) == null)
            {
                DataRow pkColumn = IndexColumns.NewRow();
                pkColumn[IndexColumn.Name] = newName;
                pkColumn[IndexColumn.Index] = IndexDescriptor.PRIMARY;
                // Index ordinals are one-based
                pkColumn[IndexColumn.Ordinal] = GetMaximumOrdinal(columns);
                IndexColumns.Rows.Add(pkColumn);
            }
        }

        /// <summary>
        /// Excludes given column from primary key.
        /// </summary>
        /// <param name="newName">Name of column to exclude from primary key.</param>
        private void ExcludeFromPrimaryKey(string newName)
        {
            // Delete related primary key column
            DataRow pkColumn = FindIndexColumn(IndexDescriptor.PRIMARY, newName);
            if (pkColumn != null)
                pkColumn.Delete();

            // If no more primary key columns, delete primary key index
            DataRow[] pkColumns = DataInterpreter.Select(IndexColumns, IndexColumn.Index, IndexDescriptor.PRIMARY);
            if (pkColumns == null || pkColumns.Length <= 0)
            {
                // Delete primary key, if any
                DataRow primaryKey = FindIndex(IndexDescriptor.PRIMARY);
                if (primaryKey != null)
                    primaryKey.Delete();
            }
        }

        /// <summary>
        /// Returns maximum ordinal value for the given index columns.
        /// </summary>
        /// <param name="columns">Array with index columns to process.</param>
        /// <returns>Returns maximum ordinal value for the given index columns.</returns>
        private static Int64 GetMaximumOrdinal(DataRow[] columns)
        {
            if (columns == null)
                return 1;
            Int64 result = columns.Length + 1;
            foreach (DataRow column in columns)
                if (DataInterpreter.GetInt(column, IndexColumn.Ordinal) >= result)
                    result = (Int64)DataInterpreter.GetInt(column, IndexColumn.Ordinal) + 1;
            return result;
        }

        /// <summary>
        /// Completes definition of the command.
        /// </summary>
        /// <param name="newColumn">Command data row to complete.</param>
        private void CompleteNewCollumn(DataRow newColumn)
        {
            // Extract column name, if any
            string columnName = DataInterpreter.GetString(newColumn, Column.Name);

            // If column name is given and type is not, try to find out default options
            if (!String.IsNullOrEmpty(columnName)
                && !DataInterpreter.IsNotEmptyString(newColumn, Column.MySqlType))
            {
                // If column ends with ID, let it be integer, otherwise, let it be varchar
                if (columnName.EndsWith(Resources.ID_Column_Name, StringComparison.CurrentCulture))
                {
                    newColumn[Column.MySqlType] = TableDescriptor.DefaultIntType;
                    newColumn[Column.Unsigned] = DataInterpreter.True;
                }
                else
                    newColumn[Column.MySqlType] = TableDescriptor.DefaultCharType;

                // If column is ID and table have not primary key yet, let column be primary key
                if (columnName.Equals(Resources.ID_Column_Name, StringComparison.CurrentCulture)
                    && !HasPrimaryKey)
                {
                    newColumn[Column.IsAutoIncrement] = DataInterpreter.True;
                    newColumn[Column.IsPrimaryKey] = DataInterpreter.True;                    
                }
                else
                {
                    // Set default value if columns is not marked as autoincrement
                    SetDefault(newColumn);
                }

                // Let column be not nullabale
                newColumn[Column.Nullable] = DataInterpreter.False;
            }

            columnDefaults[DataInterpreter.GetStringNotNull(newColumn, Column.Name)]
                        = DataInterpreter.GetString(newColumn, Column.Default);
        }

        /// <summary>
        /// Handles column deletion and deletes related foreign key columns.
        /// </summary>
        /// <param name="sender">Event sender, unused.</param>
        /// <param name="e">Detailed information about event.</param>
        private void OnColumnDeleted(object sender, DataRowChangeEventArgs e)
        {
            // Iterate through foreign keys columns
            string fkName;
            foreach (DataRow fkColumn in ForeignKeysColumns.Select())
            {
                // Need to skip deleted columns if any
                if (fkColumn.RowState == DataRowState.Deleted)
                    continue;

                fkName = DataInterpreter.GetStringNotNull(fkColumn, ForeignKeyColumn.Name);

                // Check if any data column with proper name (search for columns with given schema, table and name)
                DataRow candidate = FindColumn(fkName);

                // If column was not found, need to delete this foreign key column.
                if (candidate == null)
                    fkColumn.Delete();

            }

            // Iterate through indexes columns
            string indexName;
            foreach (DataRow indexColumn in IndexColumns.Select())
            {
                // Need to skip deleted columns if any
                if (indexColumn.RowState == DataRowState.Deleted)
                    continue;

                indexName = DataInterpreter.GetStringNotNull(indexColumn, IndexColumn.Name);

                // Check if any data column with proper name (search for columns with given schema, table and name)
                DataRow candidate = FindColumn(indexName);

                // If column was not found, need to delete this index column.
                if (candidate == null)
                    indexColumn.Delete();

            }
        }

        /// <summary>
        /// Builds new name for the column.
        /// </summary>
        /// <returns>Returns new name for the column.</returns>
        private string BuildNewColumnName()
        {
            // Initialize searche data
            int count = 0; string result; DataRow[] existsColumns = null;

            // Generate new column name
            do
            {
                result = String.Format(CultureInfo.CurrentCulture, Resources.New_Column_Name_Template, ++count);
                existsColumns = DataInterpreter.Select(Columns, Column.Name, result);
            }
            while (existsColumns == null || existsColumns.Length > 0);

            // Return results
            return result;
        }


        /// <summary>
        /// Set not empty default value for the column.
        /// </summary>
        /// <param name="column">Column, for which default value should be set.</param>
        private void SetDefault(DataRow column)
        {
            if (Parser.IsNumericType(DataInterpreter.GetStringNotNull(column, Column.MySqlType)))
                DataInterpreter.SetValueIfChanged(column, Column.Default, 0);
            else
                DataInterpreter.SetValueIfChanged(column, Column.Default, String.Empty);
        }
        #endregion

        #region Data validation before save
        /// <summary>
        /// Validates table settings in complex.
        /// </summary>
        /// <returns>
        /// Returns true if table is consistent and can be saved. 
        /// Returns false otherwize.
        /// </returns>
        protected override bool ValidateData()
        {
            // Fires saving event by calling base class
            if (!base.ValidateData())
                return false;

            // Validate columns and other data (must be loaded)
            if (Columns == null || ForeignKeys == null || ForeignKeysColumns == null
                || Indexes == null || IndexColumns == null)
                return false;

            // Validate table options
            if (!ValidateOptions())
                return false;

            // Validate column names and types
            if (!ValidateColumns())
                return false;

            // Validates primary keys (should not allow nulls)
            if (!ValidatePrimaryKeys())
                return false;

            // Validates auto increment columns
            if (!ValidateAutoIncrementColumns())
                return false;

            // Validates foreign keys
            if (!ValidateForeignKeys())
                return false;

            // Validates foreign keys
            if (!ValidateIndexes())
                return false;

            return true;
        }

        /// <summary>
        /// Returns false if any table option is invalid.
        /// </summary>
        /// <returns>Returns false if any table option is invalid.</returns>
        private bool ValidateOptions()
        {
            // Once set, UNION can not be reset
            if (DataInterpreter.HasChanged(Attributes, Table.Union)
                && !DataInterpreter.IsNotEmptyString(Attributes, Table.Union))
            {
                UIHelper.ShowError(String.Format(
                    CultureInfo.CurrentCulture,
                    Resources.Error_CantResetUnion,
                    Name));
                return false;
            }

            // TODO: Add other options validation
            return true;
        }

        /// <summary>
        /// Returns false if any column has invalid name or invalid type.
        /// </summary>
        /// <returns>Returns false if any column has invalid name or invalid type.</returns>
        private bool ValidateColumns()
        {
            // Columns should exists
            if (Columns.Rows.Count <= 0)
            {
                UIHelper.ShowError(String.Format(
                    Resources.Error_NoColumns,
                    Name));
                return false;
            }

            // Iterate through columns
            foreach (DataRow column in Columns.Rows)
            {
                if (column == null || column.RowState == DataRowState.Deleted)
                    continue;
                
                // Extract name and check for emptines
                string name = DataInterpreter.GetString(column, Column.Name);
                if (String.IsNullOrEmpty(name))
                {
                    UIHelper.ShowError(String.Format(
                        Resources.Error_EmptyColumnName,
                        Name));
                    return false;
                }

                // Validate indentifier
                if (!Parser.IsValidIdentifier(name))
                {
                    UIHelper.ShowError(String.Format(
                        Resources.Error_InvalidColumnName,
                        name,
                        Name));
                    return false;
                }

                // Validate column type
                string datatype = DataInterpreter.GetStringNotNull(column, Column.MySqlType);
                if (!Parser.IsValidDatatype(datatype))
                {
                    UIHelper.ShowError(String.Format(
                         Resources.Error_InvlaidColumnType,
                         datatype,
                         name,
                         Name));
                    return false;
                }

                // TODO: Check for supported by engine types

                // BLOB and TEXT columns should not have default values
                if ((Parser.IsBlobType(datatype) || Parser.IsTextType(datatype))
                    && DataInterpreter.IsNotEmptyString(column, Column.Default))
                {
                    UIHelper.ShowError(String.Format(
                         Resources.Error_DefaultForBlobOrText,
                         datatype,
                         name,
                         Name));
                    return false;
                }
            }

            // If everything is ok, return true
            return true;
        }

        /// <summary>
        /// Checks if there any nulable primary key and reset it with 
        /// proper warning.
        /// </summary>
        private bool ValidatePrimaryKeys()
        {            
            // Select all primary keys
            DataRow[] pk = DataInterpreter.Select(Columns, Column.IsPrimaryKey, DataInterpreter.True);

            // If primary keys was not founded, return true
            if (pk == null || pk.Length == 0)
                return true;

            // Build list of primary keys which allow nulls
            StringBuilder list = new StringBuilder();
            foreach (DataRow column in pk)
            {
                // Check row
                if(column == null)
                {
                    Debug.Fail("Null column in array!");
                    continue;
                }

                // If not nulable, continue
                if (DataInterpreter.GetSqlBool(column, Column.Nullable).IsFalse)
                    continue;

                // If not first at the list, add comma
                if (list.Length > 0)
                    list.Append(Resources.Comma);

                // Append column name
                list.Append(DataInterpreter.GetStringNotNull(column, Column.Name));
            }

            // If list is empty, returns true
            if(list.Length == 0)
                return true;

            // If user chose to return to editor return false
            if( UIHelper.ShowWarning(String.Format(Resources.Warning_NullablePrimaryKey, list.ToString()), MessageBoxButtons.YesNo) 
                == DialogResult.No )
                return false;

            // User chose to set all columns to not nullable
            foreach (DataRow column in pk)
            {
                // Check row
                if(column == null)
                {
                    Debug.Fail("Null column in array!");
                    continue;
                }

                DataInterpreter.SetValueIfChanged( column, Column.Nullable, DataInterpreter.False);
            }

            // Return true, because all columns are marked
            return true;
        }

        /// <summary>
        /// Returns false if there are several autoincrement columns.
        /// </summary>
        /// <returns>Returns false if there are several autoincrement columns.</returns>
        private bool ValidateAutoIncrementColumns()
        {
            // Select all auto increment columns
            DataRow[] columns = DataInterpreter.Select(Columns, Column.IsAutoIncrement, DataInterpreter.True);

            // Empty array is OK
            if (columns == null || columns.Length == 0)
                return true;

            // Only one auto increment column is allowed
            if (columns.Length > 1)
            {
                UIHelper.ShowError(Resources.Error_MultipleAutoIncrement);
                return false;
            }

            // Extract related primary key column
            DataRow primaryKeyColumn = FindIndexColumn(
                                        IndexDescriptor.PRIMARY, 
                                        DataInterpreter.GetStringNotNull(columns[0], IndexColumn.Name));

            // If this column is not primary key, make it primary key
            if (primaryKeyColumn == null)
            {

                // If user chose to return to editor return false
                if (UIHelper.ShowWarning(
                        String.Format(
                            Resources.Warning_AutoIncrementIsNotPrimaryKey,
                            DataInterpreter.GetStringNotNull(columns[0], Column.Name)),
                        MessageBoxButtons.YesNo)
                    == DialogResult.No)
                    return false;

                // Mark column as primary key and return true
                DataInterpreter.SetValueIfChanged(columns[0], Column.IsPrimaryKey, DataInterpreter.True);
                
                // Extract new primary key column
                primaryKeyColumn = FindIndexColumn(
                                    IndexDescriptor.PRIMARY, 
                                    DataInterpreter.GetStringNotNull(columns[0], IndexColumn.Name));
                if(primaryKeyColumn == null)
                {
                    Debug.Fail("Failed to get primary key column description");
                    // Return true and allow SQL generation. Let SQL parser and executor found this error.
                    // This should not happen if columns changes handling works correctly.
                    return true;
                }

                // Make new primary key column first
                MakeFirstPrimaryKeyColumn(primaryKeyColumn);
            }

            // Check primary key column ordinal
            if (DataInterpreter.GetInt(primaryKeyColumn, IndexColumn.Ordinal) != 1)
            {
                // Display warning and if user chose to return to editor return false
                if (UIHelper.ShowWarning(
                        String.Format(
                            Resources.Warning_AutoIncrementIsNotFirstPrimaryKey,
                            DataInterpreter.GetStringNotNull(columns[0], Column.Name)),
                        MessageBoxButtons.YesNo)
                    == DialogResult.No)
                    return false;

                // Make primary key column first
                MakeFirstPrimaryKeyColumn(primaryKeyColumn);
            }

            // Finaly return true
            return true;
        }

        /// <summary>
        /// Returns false if any foreign key or foreign key column is not valid.
        /// </summary>
        /// <returns>Returns false if any foreign key or foreign key column is not valid.</returns>
        private bool ValidateForeignKeys()
        {
            // Validate foreign keys
            foreach (DataRow key in ForeignKeys.Rows)
                if (key != null && key.RowState != DataRowState.Deleted && !IsValidForeignKey(key))
                    return false;
            return true;
        }
        
        /// <summary>
        /// Returns false if given foreign key is not valid.
        /// </summary>
        /// <param name="key">DataRow with key data.</param>
        /// <returns>Returns false if given foreign key is not valid.</returns>
        private bool IsValidForeignKey(DataRow key)
        {
            // Name should not be empty
            string name = DataInterpreter.GetString(key, ForeignKey.Name);
            if (String.IsNullOrEmpty(name))
            {
                UIHelper.ShowError(Resources.Error_EmptyForeignKeyName);
                return false;
            }

            // Name should be valid identifier
            if (!Parser.IsValidIdentifier(name))
            {
                UIHelper.ShowError(String.Format(
                   Resources.Error_InvalidForeignKeyName,
                   name,
                   Name));
                return false;
            }
            
            // Referenced table name should not be empty
            if (!DataInterpreter.IsNotEmptyString(key, ForeignKey.ReferencedTableName))
            {
                UIHelper.ShowError(String.Format(Resources.Error_EmptyReferencedTable, name));
                return false;
            }

            // Check columns (must be at least one)
            DataRow[] columns = DataInterpreter.Select(
                                    ForeignKeysColumns,
                                    ForeignKeyColumn.ForeignKeyName,
                                    name,
                                    ForeignKeyColumn.OrdinalPosition);
            if (columns == null || columns.Length <= 0)
            {
                UIHelper.ShowError(String.Format(Resources.Error_NoColumnsInForeignKey, name));
                return false;
            }

            // Check if SET NULL action is used
            if (DataInterpreter.CompareInvariant(
                    DataInterpreter.GetStringNotNull(key, ForeignKey.OnDelete), 
                    ForeignKeyDescriptor.SETNULL)
                || DataInterpreter.CompareInvariant(
                    DataInterpreter.GetStringNotNull(key, ForeignKey.OnUpdate),
                    ForeignKeyDescriptor.SETNULL))
            {
                // Check that each foreign key column is nulable
                StringBuilder list = new StringBuilder();
                foreach (DataRow fkColumn in columns)
                {
                    if (fkColumn == null)
                        continue;
                    
                    // Extract source column
                    DataRow column = FindColumn(DataInterpreter.GetStringNotNull(fkColumn, ForeignKeyColumn.Name));
                    
                    // Don't show error on it because it will be showed later
                    if (column == null)
                        continue;

                    // Check that column allow nulls, if not, add to list
                    if (DataInterpreter.GetSqlBool(column, Column.Nullable).IsFalse)
                    {
                        if(list.Length > 0)
                            list.Append(Resources.Comma);
                        list.Append(DataInterpreter.GetStringNotNull(column, Column.Name));
                    }
                }

                // If list is not empty, show error
                if (list.Length > 0)
                {
                    UIHelper.ShowError(
                        String.Format(
                            Resources.Error_InvalidSetNull,
                            name,
                            list.ToString()));
                    return false;
                }
            }

            // Finaly, validate foreign key columns
            return ValidateForeignKeyColumns(key, columns);
        }

        /// <summary>
        /// Returns false if any of given foreign keys columns are not valid.
        /// </summary>
        /// <param name="key">DataRow with key data.</param>
        /// <param name="columns">Arrya with foreign keys columns.</param> 
        /// <returns>Returns false if any of given foreign keys columns are not valid.</returns>
        private bool ValidateForeignKeyColumns(DataRow key, DataRow[] columns)
        {
            // Extract key and referenced table names
            string keyName = DataInterpreter.GetStringNotNull(key, ForeignKey.Name);
            string referencedTable = DataInterpreter.GetStringNotNull(key, ForeignKey.ReferencedTableName);
            Debug.Assert(!String.IsNullOrEmpty(keyName) && !String.IsNullOrEmpty(referencedTable), "Empty key name or referenced table name!");

            // Enumerate referenced table columns
            DataTable referencedColumns = ColumnDescriptor.Enumerate(Connection, new object[] { null, Schema, referencedTable });
            Debug.Assert(referencedColumns != null, "Failed to read referenced table columns!");

            // Enumerate referenced table index columns
            DataTable indexColumns = IndexColumnDescriptor.Enumerate(Connection, new object[] { null, Schema, referencedTable });
            Debug.Assert(indexColumns != null, "Failed to read referenced table index columns!");

            // Iterate through foreign key columns
            for (int i = 0; i < columns.Length; i++ )
            {
                DataRow column = columns[i];
                if (column == null || column.RowState == DataRowState.Deleted)
                    continue;

                // Get source column
                DataRow source = FindColumn(DataInterpreter.GetStringNotNull(column, ForeignKeyColumn.Name));
                if (source == null)
                {
                    UIHelper.ShowError(String.Format(
                        Resources.Error_NoSourceColumnForForeignKeyColumn,
                        DataInterpreter.GetStringNotNull(column, ForeignKeyColumn.Name),
                        keyName));
                    return false;
                }

                // Get referenced column name
                string referencedName = DataInterpreter.GetString(column, ForeignKeyColumn.ReferencedColumn);
                if (String.IsNullOrEmpty(referencedName))
                {
                    UIHelper.ShowError(String.Format(
                        Resources.Error_EmptyReferencedColumn,
                        DataInterpreter.GetStringNotNull(column, ForeignKeyColumn.Name),
                        keyName));
                    return false;
                }

                // If have no referenced table columns, continue (do not want to fall on it)
                if (referencedColumns == null)
                    continue;

                // Get referenced column information
                DataRow reference = referencedColumns.Rows.Find(new object[] { Schema, referencedTable, referencedName });
                if (reference == null)
                {
                    UIHelper.ShowError(String.Format(
                        Resources.Error_NoReferencedColumn,
                        new object[] {
                            DataInterpreter.GetStringNotNull(column, ForeignKeyColumn.Name),
                            keyName,
                            referencedName,
                            referencedTable}));
                    return false;
                }

                // Extract information
                string sourceType = DataInterpreter.GetStringNotNull(source, Column.MySqlType);
                string referenceType = DataInterpreter.GetStringNotNull(reference, Column.MySqlType);

                // Add unsigned option
                if (DataInterpreter.GetSqlBool(source, Column.Unsigned))
                    sourceType += " unsigned";
                if (DataInterpreter.GetSqlBool(reference, Column.Unsigned))
                    referenceType += " unsigned";

                // Compare types
                if (!DataInterpreter.CompareInvariant(sourceType, referenceType))
                {
                    UIHelper.ShowError(String.Format(
                        Resources.Error_IncompatibleTypes,
                        new object[] {
                            DataInterpreter.GetStringNotNull(column, ForeignKeyColumn.Name),
                            keyName,
                            referencedName,
                            referencedTable,
                            referenceType,
                            sourceType}));
                    return false;
                }

                // If have no referenced table index columns, continue (do not want to fall on it)
                if (indexColumns == null)
                    continue;

                // Check, if referenced table has proper index
                DataRow[] index = DataInterpreter.Select(
                                      indexColumns,
                                      IndexColumn.Name,
                                      referencedName,
                                      IndexColumn.Ordinal,
                                      i + 1);
                if (index == null || index.Length <= 0)
                {
                    UIHelper.ShowError(String.Format(
                        Resources.Error_NoIndexForForeignKey,
                        new object[] {
                            DataInterpreter.GetStringNotNull(column, ForeignKeyColumn.Name),
                            keyName,
                            referencedName,
                            referencedTable}));
                    return false;
                }
            }

            // Finaly, it is ok
            return true;
        }

        /// <summary>
        /// Returns false if any index or index column is not valid.
        /// </summary>
        /// <returns>Returns false if any index or index column is not valid.</returns>
        private bool ValidateIndexes()
        {
            // Validate indexes
            foreach (DataRow index in Indexes.Rows)
                if (index != null && index.RowState != DataRowState.Deleted && !IsValidIndex(index))
                    return false;
            return true;
        }

        /// <summary>
        /// Returns false if given index is not valid.
        /// </summary>
        /// <param name="key">DataRow with index data.</param>
        /// <returns>Returns false if given index is not valid.</returns>
        private bool IsValidIndex(DataRow index)
        {
            // Name should not be empty
            string name = DataInterpreter.GetString(index, Index.Name);
            if (String.IsNullOrEmpty(name))
            {
                UIHelper.ShowError(Resources.Error_EmptyIndexName);
                return false;
            }

            // Name should be valid identifier
            if (!Parser.IsValidIdentifier(name))
            {
                UIHelper.ShowError(String.Format(
                   Resources.Error_InvalidIndexName,
                   name,
                   Name));
                return false;
            }

            // Check columns (must be at least one)
            DataRow[] columns = DataInterpreter.Select(IndexColumns, IndexColumn.Index, name);
            if (columns == null || columns.Length <= 0)
            {
                UIHelper.ShowError(String.Format(Resources.Error_NoColumnsInIndex, name));
                return false;
            }

            // Extract index kind
            string kind = DataInterpreter.GetStringNotNull(index, Index.IndexKind);

            // If index is not primary and indexes are not supported, it is a error
            if (!DataInterpreter.CompareInvariant(kind, IndexDescriptor.PRIMARY) && !SupportIndexes)
            {
                UIHelper.ShowError(String.Format(
                    Resources.Error_IndexesUnsupported, 
                    Name,
                    Engine));
                return false;
            }

            // Check if index is spatial and not supported
            if (DataInterpreter.CompareInvariant(kind, IndexDescriptor.SPATIAL) && !SupportSpatialIndexes)
            {
                UIHelper.ShowError(String.Format(
                    Resources.Error_SpatialIndexesUnsupported,
                    name,
                    Name,
                    Engine));
                return false;
            }

            // Check if index is fulltext and not supported
            if (DataInterpreter.CompareInvariant(kind, IndexDescriptor.FULLTEXT) && !SupportFullTextIndexes)
            {
                UIHelper.ShowError(String.Format(
                    Resources.Error_FulltextIndexesUnsupported,
                    name,
                    Name,
                    Engine));
                return false;
            }

            // Finaly, validate index columns
            return ValidateIndexColumns(index, columns);
        }

        /// <summary>
        /// Returns false if any of given index columns are not valid.
        /// </summary>
        /// <param name="index">DataRow with index data.</param>
        /// <param name="columns">Array with index columns.</param> 
        /// <returns>Returns false if any of given index columns are not valid.</returns>
        private bool ValidateIndexColumns(DataRow index, DataRow[] columns)
        {
            // Extract key and referenced table names
            string indexName = DataInterpreter.GetStringNotNull(index, Index.Name);
            string indexKind = DataInterpreter.GetStringNotNull(index, Index.IndexKind);
            Debug.Assert(!String.IsNullOrEmpty(indexName) && !String.IsNullOrEmpty(indexKind), "Empty index name or index kind!");

            // Iterate through foreign key columns
            foreach (DataRow column in columns)
            {
                if (column == null || column.RowState == DataRowState.Deleted)
                    continue;

                // Get name
                string name = DataInterpreter.GetStringNotNull(column, IndexColumn.Name);

                // Get source column
                DataRow source = FindColumn(name);
                if (source == null)
                {
                    UIHelper.ShowError(String.Format(
                        Resources.Error_NoSourceColumnForIndexColumn,
                        name,
                        indexName));
                    return false;
                }

                // Extract type information
                string type = DataInterpreter.GetStringNotNull(source, Column.MySqlType);
                bool nullable = DataInterpreter.GetSqlBool(source, Column.Nullable).IsTrue;

                // Check if BLOB/TEXT supported
                if (!SupportBlobAndTextIndexes && (Parser.IsTextType(type) || Parser.IsBlobType(type)))
                {
                    UIHelper.ShowError(String.Format(
                        Resources.Error_IndexOnBlob,
                        name,
                        indexName,
                        Engine));
                    return false;
                }

                // Check if nullable columns supported
                if (!SupportNullIndexes && nullable)
                {
                    UIHelper.ShowError(String.Format(
                        Resources.Error_IndexOnNull,
                        name,
                        indexName,
                        Engine));
                    return false;
                }

                // Aditional checks for spatial indexes
                if (DataInterpreter.CompareInvariant(indexKind, IndexDescriptor.SPATIAL))
                {
                    // Column should have spatial type
                    if (!Parser.IsSpatialType(type))
                    {
                        UIHelper.ShowError(String.Format(
                            Resources.Error_NonSpatialType,
                            name,
                            indexName,
                            type));
                        return false;
                    }

                    // Column must be not nullable
                    if (nullable)
                    {
                        UIHelper.ShowError(String.Format(
                             Resources.Error_NulableColumnInSpatialIndex,
                             name,
                             indexName));
                        return false;
                    }
                }

                // Aditional check for fulltext indexes (column should have TEXT or character type)
                if (DataInterpreter.CompareInvariant(indexKind, IndexDescriptor.FULLTEXT)
                    && !Parser.IsCharacterType(type))
                {
                    UIHelper.ShowError(String.Format(
                        Resources.Error_NonCharacterType,
                        name,
                        indexName,
                        type));
                    return false;
                }
            }

            // Finaly, it is ok
            return true;
        }
        #endregion

        #region Table Database Object properties

        #region Value enums
        /// <summary>
        /// Enumeration with posible row format option values.
        /// </summary>
        public enum RowFormatValues
        {
            DEFAULT,
            DYNAMIC,
            FIXED,
            COMPRESSED,
            REDUNDANT,
            COMPACT
        }

        /// <summary>
        /// Enumeration with posible pack keys option values.
        /// </summary>
        public enum PackKeysValues
        {
            Default,            
            All,            
            None
        }

        /// <summary>
        /// Enumeration with posible insert method option values.
        /// </summary>
        public enum InsertMethodValues
        {
            NO,
            FIRST,
            LAST
        }
        #endregion

        /// <summary>
        /// Table engine property. Converts underlying string values to the proper enum
        /// and vise versa.
        /// </summary>
        [LocalizableCategory("Category_Base")]
        [LocalizableDescription("Description_Table_Engine")]
        [LocalizableDisplayName("DisplayName_Table_Engine")]
        [TypeConverter(typeof(EngineConverter))]
        public string Engine
        {
            get
            {
                string result = GetAttributeAsString(Table.Engine);
                
                // Replacing MRG_MyISAM by more readable MERGE
                if (DataInterpreter.CompareInvariant(result, TableDescriptor.MRG_MyISAM))
                    result = TableDescriptor.MERGE;

                return result;
            }
            set
            {
                SetAttribute(Table.Engine, value);
            }
        }

        /// <summary>
        /// Defines how the rows should be stored. For MyISAM tables, the option value 
        /// can be FIXED or DYNAMIC for static or variable-length row format. myisampack 
        /// sets the type to COMPRESSED.
        /// </summary>
        [LocalizableCategory("Category_Advanced")]
        [LocalizableDescription("Description_Table_RowFormat_Option")]
        [LocalizableDisplayName("DisplayName_Table_RowFormat_Option")]
        [DefaultValue(RowFormatValues.DEFAULT)]
        public RowFormatValues RowFormat
        {
            get
            {
                // Extract initial 
                string value = GetAttributeAsString(Table.RowFormatField);
                if (value == null)
                    return RowFormatValues.DEFAULT;               

                try
                {
                    string[] names = Enum.GetNames(typeof(RowFormatValues));

                    // Searches for proper value (do not want to cause exception)
                    foreach (string name in names)
                        if (DataInterpreter.CompareInvariant(name, value))
                            return (RowFormatValues)Enum.Parse(typeof(RowFormatValues), value, true);

                    return RowFormatValues.DEFAULT;  
                }
                catch (ArgumentException)
                {
                    return RowFormatValues.DEFAULT;                    
                }
            }
            set
            {
                SetAttribute(Table.RowFormatField, value);
            }
        }

        /// <summary>
        /// The initial AUTO_INCREMENT value for the table. Only for MyISAM.
        /// </summary>
        [LocalizableCategory("Category_Advanced")]
        [LocalizableDescription("Description_Table_AutoIncrement")]
        [LocalizableDisplayName("DisplayName_Table_AutoIncrement")]
        [DefaultValue(1)]
        public Nullable<Int64> AutoIncrement
        {
            get
            {
                return GetAttributeAsInt(Table.AutoIncrement);
            }
            set
            {
                // Once set, auto increment can not be removed
                if (value == null && GetOldAttributeAsInt(Table.AutoIncrement) != null)
                    SetAttribute(Table.AutoIncrement, 1);
                else
                    SetAttribute(Table.AutoIncrement, value);
            }
        }

        /// <summary>
        /// An approximation of the average row length for the table. Set this 
        /// value only for large tables with variable-size records.
        /// </summary>
        [LocalizableCategory("Category_Advanced")]
        [LocalizableDescription("Description_Table_AverageRowLength_Option")]
        [LocalizableDisplayName("DisplayName_Table_AverageRowLength_Option")]
        [DefaultValue(null)]
        public Nullable<Int64> AverageRowLength
        {
            get
            {
                return GetAttributeAsInt(Table.AverageRowLengthField);
            }
            set
            {
                SetAttribute(Table.AverageRowLengthField, value);
            }
        }

        /// <summary>
        /// The minimum number of rows you plan to store in the table.
        /// </summary>
        [LocalizableCategory("Category_Advanced")]
        [LocalizableDescription("Description_Table_MinRows_Option")]
        [LocalizableDisplayName("DisplayName_Table_MinRows_Option")]
        [DefaultValue(null)]
        public Nullable<Int64> MinRows
        {
            get
            {
                return GetAttributeAsInt(Table.MinRows);
            }
            set
            {
                SetAttribute(Table.MinRows, value);
            }
        }

        /// <summary>
        /// The maximum number of rows you plan to store in the table. This is not a
        /// hard limit, but rather a hint to the storage engine that the table must be 
        /// able to store at least this many rows.
        /// </summary>
        [LocalizableCategory("Category_Advanced")]
        [LocalizableDescription("Description_Table_MaxRows_Option")]
        [LocalizableDisplayName("DisplayName_Table_MaxRows_Option")]
        [DefaultValue(null)]
        public Nullable<Int64> MaxRows
        {
            get
            {
                return GetAttributeAsInt(Table.MaxRows);
            }
            set
            {
                SetAttribute(Table.MaxRows, value);
            }
        }

        /// <summary>
        /// Set this option if you want MySQL to maintain a live checksum for all rows 
        /// (that is, a checksum that MySQL updates automatically as the table changes). 
        /// This makes the table a little slower to update, but also makes it easier to 
        /// find corrupted tables. The CHECKSUM TABLE statement reports the checksum. 
        /// (MyISAM only.)
        /// </summary>
        [LocalizableCategory("Category_Advanced")]
        [LocalizableDescription("Description_Table_Checksum_Option")]
        [LocalizableDisplayName("DisplayName_Table_Checksum_Option")]
        [DefaultValue(false)]
        public bool Checksum
        {
            get
            {
                // True only if set 1. RTFM 13.1.5
                return GetAttributeAsInt(Table.ChecksumField) == 1;
            }
            set
            {
                // To set on, set to 1. To set off, set to 0. RTFM 13.1.5
                SetAttribute(Table.ChecksumField, value ? 1 : 0);
            }
        }
        

        /// <summary>
        /// The default character set that is used for the database object.
        /// </summary>
        [LocalizableCategory("Category_Base")]
        [LocalizableDescription("Description_Object_CharacterSet")]
        [LocalizableDisplayName("DisplayName_Object_CharacterSet")]
        [TypeConverter(typeof(CharacterSetConverter))]
        public string CharacterSet
        {
            get
            {                
                return GetAttributeAsString(Table.CharacterSet);
            }
            set
            {
                SetAttribute(Table.CharacterSet, value);
            }
        }

        /// <summary>
        /// The collation that is used to compare text for the database object.
        /// </summary>
        [LocalizableCategory("Category_Base")]
        [LocalizableDescription("Description_Object_Collation")]
        [LocalizableDisplayName("DisplayName_Object_Collation")]
        [TypeConverter(typeof(CollationConverter))]
        public string Collation
        {
            get
            {
                return GetAttributeAsString(Table.Collation);
            }
            set
            {
                SetAttribute(Table.Collation, value);
            }
        }

        /// <summary>
        /// The connection string for a FEDERATED table. This option is available as of 
        /// MySQL 5.0.13; before that, use a COMMENT option for the connection string.
        /// </summary>
        [LocalizableCategory("Category_Advanced")]
        [LocalizableDescription("Description_Table_Connection_Option")]
        [LocalizableDisplayName("DisplayName_Table_Connection_Option")]        
        [DefaultValue(null)]
        public string ConnectionForFederate
        {
            get
            {
                return GetAttributeAsString(Table.Connection);
            }
            set
            {
                SetAttribute(Table.Connection, value);
            }
        }

        /// <summary>
        /// By using this option you can specify where the MyISAM storage engine should put a 
        /// table's data file. The directory must be the full pathname to the directory, not 
        /// a relative path.
        /// </summary>
        [LocalizableCategory("Category_Advanced")]
        [LocalizableDescription("Description_Table_DataDirectory_Option")]
        [LocalizableDisplayName("DisplayName_Table_DataDirectory_Option")]
        [DefaultValue(null)]
        [Editor(typeof(FolderNameEditor),typeof(UITypeEditor))]
        public string DataDirectory
        {
            get
            {
                return GetAttributeAsString(Table.DataDirectory);
            }
            set
            {
                SetAttribute(Table.DataDirectory, value);
            }
        }

        /// <summary>
        /// By using this option you can specify where the MyISAM storage engine should put a 
        /// table's index file. The directory must be the full pathname to the directory, not 
        /// a relative path.
        /// </summary>
        [LocalizableCategory("Category_Advanced")]
        [LocalizableDescription("Description_Table_IndexDirectory_Option")]
        [LocalizableDisplayName("DisplayName_Table_IndexDirectory_Option")]
        [DefaultValue(null)]
        [Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public string IndexDirectory
        {
            get
            {
                return GetAttributeAsString(Table.IndexDirectory);
            }
            set
            {
                SetAttribute(Table.IndexDirectory, value);
            }
        }

        /// <summary>
        /// Set this option if you want MySQL to maintain a live checksum for all rows 
        /// (that is, a checksum that MySQL updates automatically as the table changes). 
        /// This makes the table a little slower to update, but also makes it easier to 
        /// find corrupted tables. The CHECKSUM TABLE statement reports the checksum. 
        /// (MyISAM only.)
        /// </summary>
        [LocalizableCategory("Category_Advanced")]
        [LocalizableDescription("Description_Table_DelayKeyWrite")]
        [LocalizableDisplayName("DisplayName_Table_DelayKeyWrite")]
        [DefaultValue(false)]
        public bool DeleteKeyWrite
        {
            get
            {
                // True only if set 1. RTFM 13.1.5
                return GetAttributeAsInt(Table.DelayKeyWrite) == 1;
            }
            set
            {
                // To set on, set to 1. To set off, set to 0. RTFM 13.1.5
                SetAttribute(Table.DelayKeyWrite, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Use this option to generate smaller indices. This usually makes updates 
        /// slower and reads faster. Setting it to DEFAULT tells the storage engine 
        /// to only pack long CHAR/VARCHAR columns.
        /// </summary>
        [LocalizableCategory("Category_Advanced")]
        [LocalizableDescription("Description_Table_PackKeys")]
        [LocalizableDisplayName("DisplayName_Table_PackKeys")]
        [DefaultValue(TableDocument.PackKeysValues.Default)]
        public PackKeysValues PackKeys
        {
            get
            {
                string stringVal = GetAttributeAsString(Table.PackKeys);
                
                // If nothing, then default
                if (String.IsNullOrEmpty(stringVal))
                    return PackKeysValues.Default;

                // If "1", then all
                if (DataInterpreter.CompareInvariant(stringVal, "1"))
                    return PackKeysValues.All;

                // If "0", then none
                if (DataInterpreter.CompareInvariant(stringVal, "0"))
                    return PackKeysValues.None;

                // In all other cases DEFAULT
                return PackKeysValues.Default;
            }
            set
            {
                switch (value)
                {
                    case PackKeysValues.All: // If All, then "1"
                        SetAttribute(Table.PackKeys, "1");
                        break;
                    case PackKeysValues.None: // If None, thne "0"
                        SetAttribute(Table.PackKeys, "0");
                        break;
                    default: // In all other cases "DEFAULT"
                        SetAttribute(Table.PackKeys, "DEFAULT");
                        break;
                }
            }
        }

        /// <summary>
        /// Use this option to encrypt the .frm file with a password. This option 
        /// does nothing in the standard MySQL version.
        /// </summary>
        [LocalizableCategory("Category_Advanced")]
        [LocalizableDescription("Description_Table_Password")]
        [LocalizableDisplayName("DisplayName_Table_Password")]        
        public string Password
        {
            get
            {
                return GetAttributeAsString(Table.Password);
            }
            set
            {
                SetAttribute(Table.Password, value);
            }
        }

        /// <summary>
        /// If you want to insert data into a MERGE table, you must specify with 
        /// INSERT_METHOD the table into which the row should be inserted. 
        /// INSERT_METHOD is an option useful for MERGE tables only. Use a value 
        /// of FIRST or LAST to have inserts go to the first or last table, or a 
        /// value of NO to prevent inserts.
        /// </summary>
        [LocalizableCategory("Category_Advanced")]
        [LocalizableDescription("Description_Table_InsertMethod")]
        [LocalizableDisplayName("DisplayName_Table_InsertMethod")]
        [DefaultValue(TableDocument.InsertMethodValues.NO)]
        public InsertMethodValues InsertMethod
        {
            get
            {
                string stringVal = GetAttributeAsString(Table.InsertMethod);

                // If nothing, then NO
                if (String.IsNullOrEmpty(stringVal))
                    return InsertMethodValues.NO;

                // If "FIRST", then FIRST
                if (DataInterpreter.CompareInvariant(stringVal, InsertMethodValues.FIRST.ToString()))
                    return InsertMethodValues.FIRST;

                // If "LAST", then LAST
                if (DataInterpreter.CompareInvariant(stringVal, InsertMethodValues.LAST.ToString()))
                    return InsertMethodValues.LAST;

                // In all other cases NO
                return InsertMethodValues.NO;
            }
            set
            {
                SetAttribute(Table.InsertMethod, value);
            }
        }

        /// <summary>
        /// List of MyISAM tables that should be used by the MERGE table.
        /// </summary>
        [LocalizableCategory("Category_Advanced")]
        [LocalizableDescription("Description_Table_Union")]
        [LocalizableDisplayName("DisplayName_Table_Union")]
        public string Union
        {
            get
            {
                return GetAttributeAsString(Table.Union);
            }
            set
            {
                SetAttribute(Table.Union, value);
            }
        }
        #endregion

        #region Public properties
        /// <summary>
        /// DataTable object with lists of all columns for this table. This property 
        /// is read-only, but all changes madden to DataTable affects table object too.
        /// This table are read in LoadData.
        /// </summary>
        [Browsable(false)]
        public DataTable Columns
        {
            get
            {
                Debug.Assert(columnsTable != null, "Columns are not read!");
                return columnsTable;
            }
        }

        /// <summary>
        /// DataTable object with lists of all foreign keys for this table. This property 
        /// is read-only, but all changes madden to DataTable affects table object too.
        /// This table are read in LoadData.
        /// </summary>
        [Browsable(false)]
        public DataTable ForeignKeys
        {
            get
            {
                Debug.Assert(foreignKeysTable != null, "Foreign keys are not read!");
                return foreignKeysTable;
            }
        }

        /// <summary>
        /// DataTable object with lists of all foreign keys columns for this table. This 
        /// property is read-only, but all changes madden to DataTable affects table object 
        /// too. This table are read in LoadData.
        /// </summary>
        [Browsable(false)]
        public DataTable ForeignKeysColumns
        {
            get
            {
                Debug.Assert(foreignKeysColumnsTable != null, "Foreign keys columns are not read!");
                return foreignKeysColumnsTable;
            }
        }

        /// <summary>
        /// DataTable object with lists of all indexes for this table. This property is read-only, 
        /// but all changes madden to DataTable affects table object too. This table are read in 
        /// LoadData.
        /// </summary>
        [Browsable(false)]
        public DataTable Indexes
        {
            get
            {
                Debug.Assert(indexesTable != null, "Indexes are not read!");
                return indexesTable;
            }
        }

        /// <summary>
        /// DataTable object with lists of all index columns  for this table. This property is 
        /// read-only, but all changes madden to DataTable affects table object too. This table 
        /// are read in LoadData.
        /// </summary>
        [Browsable(false)]
        public DataTable IndexColumns
        {
            get
            {
                Debug.Assert(indexColumnsTable != null, "Index columns are not read!");
                return indexColumnsTable;
            }
        }
        #endregion 
        
        #region Indexes information
        /// <summary>
        /// Returns true if indexes are supported. ARCHIVE engine does not support indexes.
        /// </summary>
        [Browsable(false)]
        public bool SupportIndexes
        {
            get
            {
                return !DataInterpreter.CompareInvariant(Engine, TableDescriptor.ARCHIVE);
            }
        }
        /// <summary>
        /// Returns true if FULLTEXT indexes are supported. Only InnoDB engine supports FULLTEXT
        /// indexes.
        /// </summary>
        [Browsable(false)]
        public bool SupportFullTextIndexes
        {
            get
            {
                return DataInterpreter.CompareInvariant(Engine, TableDescriptor.MyISAM);
            }
        }

        /// <summary>
        /// Returns true if SPATIAL indexes are supported. Only MyISAM, InnoDB, NDB and BDB
        /// engines supports SPATIAL indexes (RTFM 16).
        /// </summary>
        [Browsable(false)]
        public bool SupportSpatialIndexes
        {
            get
            {
                return DataInterpreter.CompareInvariant(Engine, TableDescriptor.MyISAM)
                    || DataInterpreter.CompareInvariant(Engine, TableDescriptor.InnoDB)
                    || DataInterpreter.CompareInvariant(Engine, TableDescriptor.BDB)
                    || DataInterpreter.CompareInvariant(Engine, TableDescriptor.NDB);
            }
        }

        /// <summary>
        /// Returns true if hash indexes are supported. Only MEMORY engine supports HASH
        /// indexes.
        /// </summary>
        [Browsable(false)]
        public bool SupportHashIndexes
        {
            get
            {
                return DataInterpreter.CompareInvariant(Engine, TableDescriptor.MEMORY);
            }
        }

        /// <summary>
        /// Returns true if BLOB and TEXT indexes are supported. Only MyISAM, InnoDB and BDB
        /// engines supports BLOB and TEXT indexes (RTFM 13.1.14).
        /// </summary>
        [Browsable(false)]
        public bool SupportBlobAndTextIndexes
        {
            get
            {
                return DataInterpreter.CompareInvariant(Engine, TableDescriptor.MyISAM)
                    || DataInterpreter.CompareInvariant(Engine, TableDescriptor.InnoDB)
                    || DataInterpreter.CompareInvariant(Engine, TableDescriptor.BDB);
            }
        }

        /// <summary>
        /// Returns true if indexes on nullable columns are supported. Only MyISAM, InnoDB, BDB
        /// and MEMORY engines supports indexes on nullable columns (RTFM 13.1.14).
        /// </summary>
        [Browsable(false)]
        public bool SupportNullIndexes
        {
            get
            {
                return DataInterpreter.CompareInvariant(Engine, TableDescriptor.MyISAM)
                    || DataInterpreter.CompareInvariant(Engine, TableDescriptor.InnoDB)
                    || DataInterpreter.CompareInvariant(Engine, TableDescriptor.BDB)
                    || DataInterpreter.CompareInvariant(Engine, TableDescriptor.MEMORY);
            }
        }

        /// <summary>
        /// Returns array with names of supported index kinds.
        /// </summary>
        /// <returns>Returns array with names of supported index kinds.</returns>
        public string[] GetSupportedIndexKinds()
        {
            if (Attributes == null)
                return null;

            // Collect result into temporary list
            List<string> list = new List<string>();

            // If indexes are not supported, return empty list
            if (!SupportIndexes)
                return new string[0];
            
            // Simple index and unique are supported by all engines that supports indexes
            list.Add(IndexDescriptor.INDEX);
            list.Add(IndexDescriptor.UNIQUE);
            
            // Full text indexes supported only by MyISAM
            if (SupportFullTextIndexes)
                list.Add(IndexDescriptor.FULLTEXT);
            
            // Spatial indexes supported only by MyISAM
            if (SupportSpatialIndexes)
                list.Add(IndexDescriptor.SPATIAL);

            // Prepare and return results
            string[] result = new string[list.Count];
            list.CopyTo(result);
            return result;
        }

        /// <summary>
        /// Returns array with names of supported index types.
        /// </summary>
        /// <returns>Returns array with names of supported index types.</returns>
        public string[] GetSupportedIndexTypes()
        {
            if (Attributes == null)
                return null;

            // Collect result into temporary list
            List<string> list = new List<string>();

            // Simple BTREE are supported by all engines
            list.Add(IndexDescriptor.BTREE);

            // HASH indexes supported only by MEMORY
            if (SupportHashIndexes)
                list.Add(IndexDescriptor.HASH);

            // Prepare and return results
            string[] result = new string[list.Count];
            list.CopyTo(result);
            return result;
        }
        #endregion

        #region Table create or alter SQL generation
        /// <summary>
        /// Builds header for CREATE TABLE request.
        /// </summary>
        /// <param name="target">String builder, used to build query.</param>
        private void BuildCreateHeader(StringBuilder target)
        {
            target.Append("CREATE TABLE ");
            QueryBuilder.WriteIdentifier(Name, target);          
        }

        /// <summary>
        /// Builds header for ALTER TABLE request.
        /// </summary>
        /// <param name="target">String builder, used to build query.</param>
        private void BuildAlterHeader(StringBuilder target)
        {
            target.Append("ALTER TABLE ");
            QueryBuilder.WriteIdentifier(OldName, target);  
        }

        /// <summary>
        /// Builds create definition, as described in MySQL manual 13.1.5.
        /// </summary>
        /// <param name="target">String builder, used to build query.</param>
        private void BuildCreateDefinition(StringBuilder target)
        {
            BuildColumnsCreation(target);

            // Write primary key columns
            BuildPrimaryKey(target, null);

            // Write indexes
            BuildIndexes(target, null);

            // Write foreign keys
            BuildForeignKeys(target, null);

            // Remove trailing ","
            if (target[target.Length - 1] == ',')
                target[target.Length - 1] = ' ';
        }

        /// <summary>
        /// Builds alter specifications, as described in MySQL manual 13.1.2.
        /// </summary>
        /// <param name="target">String builder, used to build query.</param>
        private void BuildAlterSpecifications(StringBuilder target)
        {
            // If table was renamed
            QueryBuilder.WriteIdentifierIfChanged(Attributes, Table.Name, "\nRENAME TO ", ",", target);

            // Check if primary key need to be changed
            bool needDropKey, needNewKey;
            DetermineRequiredPrimaryKeyChanges(out needDropKey, out needNewKey);

            // Drop primary key if needed
            if (needDropKey)
                target.Append("\nDROP PRIMARY KEY,");

            // Write detected columns changes
            BuildColumnsChanges(target);

            // Create primary key if needed
            if (needNewKey)
                BuildPrimaryKey(target, "ADD ");

            // Write indexes
            BuildIndexes(target, "ADD ");

            // Write foreign keys
            BuildForeignKeys(target, "ADD ");

            // Write options
            BuildTableOptions(target, ",");

            // Remove trailing ","
            if (target[target.Length - 1] == ',')
                target[target.Length - 1] = ';';
        }

        /// <summary>
        /// Builds table options, as described in MySQL manual 13.1.5.
        /// </summary>
        /// <param name="target">String builder, used to build query.</param>
        private void BuildTableOptions(StringBuilder target, string suffix)
        {
            Debug.Assert(Attributes != null, "Attributes are not loaded!");

            // Write table comments
            QueryBuilder.WriteIfChanged(Attributes, Table.Comments, "\nCOMMENT = ", target, suffix);

            // Write table engine
            QueryBuilder.WriteIfChanged(Attributes, Table.Engine, "\nENGINE = ", target, suffix, false);

            // Write character set option
            QueryBuilder.WriteIfChanged(Attributes, Table.CharacterSet, "\nCHARACTER SET ", target, suffix, false);

            // Write character set option
            QueryBuilder.WriteIfChanged(Attributes, Table.Collation, "\nCOLLATE ", target, suffix, false);

            // Write initial autoincrement value
            QueryBuilder.WriteIfChanged(Attributes, Table.AutoIncrement, "\nAUTO_INCREMENT = ", target, suffix, false);

            // Write average row length value
            QueryBuilder.WriteIfChanged(Attributes, Table.AverageRowLengthField, "\nAVG_ROW_LENGTH = ", target, suffix, false);

            // Write min_rows value
            QueryBuilder.WriteIfChanged(Attributes, Table.MinRows, "\nMIN_ROWS = ", target, suffix, false);
            
            // Write max_rows value
            QueryBuilder.WriteIfChanged(Attributes, Table.MaxRows, "\nMAX_ROWS = ", target, suffix, false);

            // Write row_format value
            QueryBuilder.WriteIfChanged(Attributes, Table.RowFormatField, "\nROW_FORMAT = ", target, suffix, false);

            // Write checksum option
            QueryBuilder.WriteIfChanged(Attributes, Table.ChecksumField, "\nCHECKSUM = ", target, suffix, false);

            // Write connection option
            QueryBuilder.WriteIfChanged(Attributes, Table.Connection, "\nCONNECTION = ", target, suffix);

            // Write data directory option
            QueryBuilder.WriteIfChanged(Attributes, Table.DataDirectory, "\nDATA DIRECTORY = ", target, suffix);

            // Write index directory option
            QueryBuilder.WriteIfChanged(Attributes, Table.IndexDirectory, "\nINDEX DIRECTORY = ", target, suffix);

            // Write delay key write option
            QueryBuilder.WriteIfChanged(Attributes, Table.DelayKeyWrite, "\nDELAY_KEY_WRITE = ", target, suffix, false);

            // Write pack keys option
            QueryBuilder.WriteIfChanged(Attributes, Table.PackKeys, "\nPACK_KEYS = ", target, suffix, false);

            // Write password option
            QueryBuilder.WriteIfChanged(Attributes, Table.Password, "\nPASSWORD = ", target, suffix);

            // Write insert method option
            QueryBuilder.WriteIfChanged(Attributes, Table.InsertMethod, "\nINSERT_METHOD = ", target, suffix, false);

            // Write union option
            QueryBuilder.WriteIfChanged(Attributes, Table.Union, "\nUNION = (", target, ")" + suffix, false);
        }
        #endregion

        #region Columns operations SQL
        /// <summary>
        /// Writes definitionas of table columns.
        /// </summary>
        /// <param name="target">String builder, used to build query.</param>
        private void BuildColumnsCreation(StringBuilder target)
        {
            // Write columns definitions
            foreach (DataRow column in Columns.Select(String.Empty, Column.Ordinal))
            {
                // Start new line
                target.AppendLine();
                BuildColumnDefinition(column, target, null);
            }
        }

        /// <summary>
        /// Writes detected column changes
        /// </summary>
        /// <param name="target">String builder, used to build query.</param>
        private void BuildColumnsChanges(StringBuilder target)
        {
            // Initial after is first
            string after = " FIRST";

            // Iterate through columns
            foreach (DataRow column in Columns.Select(String.Empty, Column.Ordinal, DataViewRowState.Deleted | DataViewRowState.CurrentRows))
            {
                switch (column.RowState)
                {
                    // Create definition for new column
                    case DataRowState.Added:
                        BuildAddColumn(target, column, after);
                        break;
                    // Create drop column command
                    case DataRowState.Deleted:
                        BuildDropColumn(target, column);                        
                        break;
                    // Create alter column comand
                    case DataRowState.Modified:
                        // Not all changes should be handled here, for example,
                        // primary key status handled by BuildAlterPrimaryKey
                        if (IsColumnChanged(column))
                            BuildChangeColumn(target, column, after);
                        break;
                    default:
                        break;
                }

                // If row is deleted, continue
                if (column.RowState == DataRowState.Deleted)
                    continue;

                // Recaculate after                
                after = " AFTER " + QueryBuilder.EscapeAndQuoteIdentifier(
                                                    DataInterpreter.GetStringNotNull(column, Column.Name));
            }
        }

        /// <summary>
        /// Builds column definition, as described in MySQL manual 13.1.15.
        /// </summary>
        /// <param name="column">Data row with column attributes.</param>
        /// <param name="target">String builder, used to build query.</param>
        private void BuildColumnDefinition(DataRow column, StringBuilder target, string after)
        {

            // Write column name
            QueryBuilder.WriteIdentifier(column, Column.Name, target);

            // Write column type
            target.Append(' ');
            BuildDataType(column, target);

            // Write NOT NULL if necessary
            QueryBuilder.WriteIfFalse(column, Column.Nullable, " NOT NULL", target);

            // Write default value if specified (only for not autoincrement columns)
            if (!DataInterpreter.GetSqlBool(column, Column.IsAutoIncrement).IsTrue)
            {
                if (Parser.IsNumericType(DataInterpreter.GetStringNotNull(column, Column.MySqlType)))
                    // For integers we neve use quotes
                    QueryBuilder.WriteIfNotEmptyString(column, Column.Default, " DEFAULT ", target, false);
                else if (Parser.IsDateTimeType(DataInterpreter.GetStringNotNull(column, Column.MySqlType)))
                    // For datetimes we should use quotes only if punctuation character is used
                    QueryBuilder.WriteIfNotEmptyString(column, Column.Default, " DEFAULT ", target,
                        Parser.HasPunctuation(DataInterpreter.GetStringNotNull(column, Column.Default)));
                else
                    // Other types are always quoted and empty string is allowed
                    QueryBuilder.WriteIfNotNull(column, Column.Default, " DEFAULT ", target);
            }

            // Write auto increment flag
            QueryBuilder.WriteIfTrue(column, Column.IsAutoIncrement, " AUTO_INCREMENT", target);

            // Write column comments
            QueryBuilder.WriteIfNotEmptyString(column, Column.Comments, " COMMENT ", target);

            // Write after definition, if any
            QueryBuilder.WriteIfNotEmptyString(after, null, target, false);

            // Write trailing ","
            target.Append(",");
        }

        /// <summary>
        /// Builds CHANGE COLUMN or MODIFY COLUMN definition, as described in 
        /// MySQL manual 13.1.2.
        /// </summary>
        /// <param name="column">Data row with column attributes.</param>
        /// <param name="target">String builder, used to build query.</param>
        private void BuildChangeColumn(StringBuilder target, DataRow column, string after)
        {
            if (DataInterpreter.HasChanged(column, Column.Name))
            {
                target.Append("\nCHANGE COLUMN ");
                QueryBuilder.WriteOldIdentifier(column, Column.Name, target);
                target.Append(" ");
            }
            else
            {
                target.Append("\nMODIFY COLUMN ");
            }
            BuildColumnDefinition(column, target, after);
        }

        /// <summary>
        /// Builds DROP COLUMN definition, as described in MySQL manual 13.1.2.
        /// </summary>
        /// <param name="column">Data row with column attributes.</param>
        /// <param name="target">String builder, used to build query.</param>
        private void BuildDropColumn(StringBuilder target, DataRow column)
        {
            target.Append("\nDROP COLUMN ");
            QueryBuilder.WriteOldIdentifier(column, Column.Name, target);

            // Write trailing ","
            target.Append(",");
        }

        /// <summary>
        /// Builds ADD COLUMN definition, as described in MySQL manual 13.1.2.
        /// </summary>
        /// <param name="column">Data row with column attributes.</param>
        /// <param name="target">String builder, used to build query.</param>
        private void BuildAddColumn(StringBuilder target, DataRow column, string after)
        {
            target.Append("\nADD COLUMN ");
            BuildColumnDefinition(column, target, after);
        }

        /// <summary>
        /// Builds data type definition, as described in MySQL manual 13.1.15.
        /// </summary>
        /// <param name="column">Data row with column attributes.</param>
        /// <param name="target">String builder, used to build query.</param>
        private void BuildDataType(DataRow column, StringBuilder target)
        {
            string dataType = DataInterpreter.GetStringNotNull(column, Column.MySqlType);
                       
            target.Append(dataType);

            // If numeric, check for UNSIGNED and ZEROFILL
            if (Parser.IsNumericType(dataType))
            {
                QueryBuilder.WriteIfTrue(column, Column.Unsigned, " UNSIGNED", target);
                QueryBuilder.WriteIfTrue(column, Column.Zerofill, " ZEROFILL", target);
            }

            // If supports binary, check for binary
            if( Parser.SupportBinary(dataType) )
                QueryBuilder.WriteIfTrue(column, Column.Binary, " BINARY", target);

            // If supports ASCII and UNICODE, check for them
            if (Parser.SupportAsciiAndUnicode(dataType))
            {
                QueryBuilder.WriteIfTrue(column, Column.Ascii, " ASCII", target);
                QueryBuilder.WriteIfTrue(column, Column.Unicode, " UNICODE", target);
            }

            // If supports character set and colltion, check for them
            if (Parser.SupportCharacterSet(dataType))
            {
                // Write character set if differs from default for table or changed
                if (!DataInterpreter.CompareInvariant(
                            CharacterSet,
                            DataInterpreter.GetStringNotNull(column, Column.CharacterSet))
                    || DataInterpreter.HasChanged(column, Column.CharacterSet))
                {
                    QueryBuilder.WriteIfNotEmptyString(column, Column.CharacterSet, " CHARACTER SET ", target, false);
                }
                
                // Write collation if differs from default for table or changed
                if (!DataInterpreter.CompareInvariant(
                            Collation,
                            DataInterpreter.GetStringNotNull(column, Column.Collation))
                    || DataInterpreter.HasChanged(column, Column.Collation))
                {
                    QueryBuilder.WriteIfNotEmptyString(column, Column.Collation, " COLLATE ", target, false);
                }
            }
        }

        /// <summary>
        /// Checks, if column changes should be processed by BuildChangeColumn method. 
        /// Not any change cause MODIFY and CHANGE COLUMN statemets to be inserted. For 
        /// example, if only primary key status of the column was changed, it will be 
        /// processed by BuildAlterPrimaryKey method. 
        /// </summary>
        /// <param name="column">DataRow with column attributes to check.</param>
        /// <returns>
        /// Returns true if changes should be handled by method and false otherwise.
        /// </returns>
        private bool IsColumnChanged(DataRow column)
        {
            // Look for changed columns attributes
            foreach (DataColumn tableColumn in column.Table.Columns)
            {
                if (DataInterpreter.HasChanged(column, tableColumn.ColumnName))
                {
                    // If primary key status was changed, it will be handeled
                    // by BuildAlterPrimaryKey
                    if (DataInterpreter.CompareInvariant(
                                tableColumn.ColumnName,
                                Column.IsPrimaryKey))
                        continue;
                    // All other changes need to be handeled by BuildChangeColumn
                    return true;
                }
            }
            // No changes found for BuildChangeColumn
            return false;
        }
        #endregion

        #region Foreign key SQL generation
        /// <summary>
        /// Builds pre-query to drop foreign keys. This is necessary because of 
        /// bug http://bugs.mysql.com/bug.php?id=8377 .
        /// </summary>
        /// <returns>Returns pre-query to drop foreign keys</returns>
        private string BuildPreDropForeignKeys()
        {
            StringBuilder target = new StringBuilder();
            
            // Write header
            BuildAlterHeader(target);

            // Iterate through foreign keys and drop them
            foreach (DataRow key in ForeignKeys.Rows)
            {
                switch (key.RowState)
                {
                    // Skip added and deleted keys                    
                    case DataRowState.Added:
                    case DataRowState.Deleted:
                        break;
                    // Check if this key was changed and drop it
                    default:
                        if (HasForeignKeyChanged(key))
                            BuildDropForeignKey(target, key);
                        break;

                }
            }

            // Remove trailing ","
            if (target[target.Length - 1] == ',')
                target[target.Length - 1] = ' ';

            // Append new line
            return target.ToString();
        }

        /// <summary>
        /// Returns true if any of the tables foreign keys was changed and need to be
        /// pre-droped. This is necessary because of bug http://bugs.mysql.com/bug.php?id=8377 .
        /// </summary>
        /// <returns>
        /// Returns true if any of the tables foreign keys was changed and need to be
        /// pre-droped.
        /// </returns>
        private bool NeedToDropForeignKeys()
        {
            // Iterate through foreign keys and check them
            foreach (DataRow key in ForeignKeys.Rows)
            {
                switch (key.RowState)
                {
                    // Skip added and deleted keys                    
                    case DataRowState.Added:
                    case DataRowState.Deleted:
                        break;
                    // Check if this key was changed
                    default:
                        if (HasForeignKeyChanged(key))
                            return true;
                        break;
                }
            }
            return false;
        }

        /// <summary>
        /// Build definitions for changes in the foreign keys.
        /// </summary>
        /// <param name="target">String builder, used to build query.</param>
        /// <param name="prefix">
        /// Prefix to add before definition. Could be empty or "ADD "
        /// </param>
        private void BuildForeignKeys(StringBuilder target, string prefix)
        {
            // Iterate through foreign keys
            foreach (DataRow key in ForeignKeys.Rows)
            {
                switch (key.RowState)
                {
                    // Create definition for new foreign key
                    case DataRowState.Added:
                        BuildAddForeignKey(target, key, prefix);
                        break;
                    // Create drop key command
                    case DataRowState.Deleted:
                        BuildDropForeignKey(target, key);
                        break;
                    // Create DROP/ADD key, if needed
                    default:
                        if (HasForeignKeyChanged(key))
                            BuildAddForeignKey(target, key, prefix);
                        break;

                }
            }
        }

        /// <summary>
        /// Builds DROP FOREIGN KEY statement
        /// </summary>
        /// <param name="target">String builder, used to build query.</param>
        /// <param name="key">DataRow with foreign key data.</param>
        private void BuildDropForeignKey(StringBuilder target, DataRow key)
        {
            target.Append("\nDROP FOREIGN KEY ");
            QueryBuilder.WriteOldIdentifier(key, ForeignKey.Name, target);

            // Write trailing ","
            target.Append(",");

            // If we have index with same name, drop it too
            if (key.HasVersion(DataRowVersion.Original))
            {
                DataRow index = FindIndex(DataInterpreter.GetStringNotNull(key, ForeignKey.Name, DataRowVersion.Original));
                if (index != null)
                    BuildDropIndex(target, index);
            }
        }

        /// <summary>
        /// Builds ADD FOREIGN KEY statement
        /// </summary>
        /// <param name="target">String builder, used to build query.</param>
        /// <param name="key">DataRow with foreign key data.</param>
        /// <param name="prefix">
        /// Prefix to add before definition. Could be empty or "ADD "
        /// </param>
        private void BuildAddForeignKey(StringBuilder target, DataRow key, string prefix)
        {
            // Start new line
            target.AppendLine();
            // Append prefix if any
            if (!String.IsNullOrEmpty(prefix))
                target.Append(prefix);
            
            // Start definition
            target.Append("\nCONSTRAINT ");
            QueryBuilder.WriteIdentifier(key, ForeignKey.Name, target);
            target.Append("\n\tFOREIGN KEY ");
            QueryBuilder.WriteIdentifier(key, ForeignKey.Name, target);

            // Extract all foreign keys columns
            DataRow[] columns = DataInterpreter.Select(
                                    ForeignKeysColumns,
                                    ForeignKeyColumn.ForeignKeyName,
                                    DataInterpreter.GetStringNotNull(key, ForeignKey.Name),
                                    ForeignKeyColumn.OrdinalPosition);

            // Validate array
            if (columns == null)
            {
                Debug.Fail("Failed to get foreign key columns!");
                return;
            }

            // Write source columns
            WriteColumnNames(target, columns, ForeignKeyColumn.Name);

            // Write refernces
            target.Append("\n\tREFERENCES ");
            QueryBuilder.WriteIdentifier(key, ForeignKey.ReferencedTableName, target);

            // Write referenced columns
            WriteColumnNames(target, columns, ForeignKeyColumn.ReferencedColumn);

            // Write options
            QueryBuilder.WriteIfNotEmptyString(key, ForeignKey.OnDelete, "\n\tON DELETE ", target, false);
            QueryBuilder.WriteIfNotEmptyString(key, ForeignKey.OnUpdate, "\n\tON UPDATE ", target, false);

            // Write trailing ','
            target.Append(",");
        }

        /// <summary>
        /// Writes list of foreign key columns names into query. Can write both source 
        /// columns and referenced columns names.
        /// </summary>
        /// <param name="target">String builder, used to build query.</param>
        /// <param name="columns">DataRow array with foreign key columns.</param>
        /// <param name="attributeName">Name of the column attribute to write to query.</param>
        private static void WriteColumnNames(StringBuilder target, DataRow[] columns, string attributeName)
        {
            target.Append(" (");
            for (int i = 0; i < columns.Length; i++)
            {
                if (i > 0)
                    target.Append(", ");
                QueryBuilder.WriteIdentifier(columns[i], attributeName, target);
            }
            target.Append(")");
        }

        /// <summary>
        /// Returns true if given foreign key was changed.
        /// </summary>
        /// <param name="key">DataRow with description of the foreign key.</param>
        /// <returns>Returns true if given foreign key was changed.</returns>
        private bool HasForeignKeyChanged(DataRow key)
        {
            // Check key for changes
            if (DataInterpreter.HasChanged(key))
                return true;

            // Get list of all foreign keys columns
            DataRow[] columns = DataInterpreter.Select(
                                    ForeignKeysColumns,
                                    ForeignKeyColumn.ForeignKeyName,
                                    DataInterpreter.GetStringNotNull(key, ForeignKey.Name));

            // Check columns
            if( columns == null )
            {
                Debug.Fail("Failed to get foreign key columns!");
                return false;
            }

            // Check each column
            foreach (DataRow column in columns)
                if (HasForeignKeyColumnChanged(column))
                    return true;

            // Look for deleted columns
            string oldName = DataInterpreter.GetStringNotNull(key, ForeignKey.Name, DataRowVersion.Original);
            foreach (DataRow fkColumn in ForeignKeysColumns.Rows)
            {
                // Only deleted columns are intrusting
                if (fkColumn.RowState != DataRowState.Deleted)
                    continue;

                // Extract original foreign key name
                string fkName = DataInterpreter.GetStringNotNull(fkColumn, ForeignKeyColumn.ForeignKeyName, DataRowVersion.Original);

                // If exact like current foreign key name, then it is changed
                if (DataInterpreter.CompareInvariant(oldName, fkName))
                    return true;
            }

            return false;
        }
        
        /// <summary>
        /// Returns true if given foreign key column was changed.
        /// </summary>
        /// <param name="column">DataRow with the foreign key column description.</param>
        /// <returns>Returns true if given foreign key column was changed.</returns>
        private bool HasForeignKeyColumnChanged(DataRow column)
        {
            // Even if only column name was changed, we need to drop and recreate key.
            // See http://bugs.mysql.com/bug.php?id=8377 for details.
            return DataInterpreter.HasChanged(column);
        }
        #endregion

        #region Primary Key SQL operations
        /// <summary>
        /// Check if primary key should be dropped and/or created.
        /// 
        /// Primary key should be dropped if:
        /// - Column was excluded from primary key, but not from the table.
        /// - Column auto increment status was changed.
        /// - New column is added to the primary key and it already contains columns.
        /// 
        /// Primary key should be created if:
        /// - New column added to the primary key.
        /// - Primary key need to be dropped, but it is not empty.
        /// 
        /// Finaly, primary key should be recreated if index columns were reordered.
        /// </summary>
        /// <param name="needDropKey">Indicates that primary key should be droped.</param>
        /// <param name="needNewKey">Indicates if primary key should be added.</param>
        private void DetermineRequiredPrimaryKeyChanges(out bool needDropKey, out bool needNewKey)
        {
            // Set to true if any of primary key columns should be removed 
            // from the primary key, but not from the table, or primary
            // key should be removed for other reason.
            needDropKey = false;
            // Set to true if new columns should be added to the primary key
            needNewKey = false;
            // Set true if primary key is exists and need to be keeped
            bool needKeepKey = false;

            // Look for primary key option changes
            foreach (DataRow column in Columns.Rows)
            {
                switch (column.RowState)
                {
                    // New column added.
                    case DataRowState.Added:
                        // Should it be added to primary key?
                        needNewKey = needNewKey || DataInterpreter.GetSqlBool(column, Column.IsPrimaryKey).IsTrue;
                        break;
                    // Old column founded
                    case DataRowState.Modified:
                    case DataRowState.Unchanged:
                        // Its primary key status was changed
                        if (DataInterpreter.HasChanged(column, Column.IsPrimaryKey))
                        {
                            // Should we include it into primary key?
                            needNewKey = needNewKey
                                            || DataInterpreter.GetSqlBool(column, Column.IsPrimaryKey).IsTrue;
                            // Should we exclude it from the primary key?
                            needDropKey = needDropKey
                                            || DataInterpreter.GetSqlBool(column, Column.IsPrimaryKey, DataRowVersion.Original).IsTrue;
                        }
                        // Its primary key status wasn't changed
                        else
                        {
                            // Should we keep it in the primary key?
                            needKeepKey = needKeepKey 
                                || DataInterpreter.GetSqlBool(column, Column.IsPrimaryKey).IsTrue
                                || DataInterpreter.GetSqlBool(column, Column.IsAutoIncrement).IsTrue;

                            // If column auto increment flag was changed, we need to recreate key
                            needDropKey = needDropKey || DataInterpreter.HasChanged(column, Column.IsAutoIncrement);
                        }
                        break;
                    default:
                        break;
                }

                // Need new or need to recreate
                needNewKey = needNewKey || (needDropKey && needKeepKey);
                // Need drop or need to extend
                needDropKey = needDropKey || (needNewKey && needKeepKey);

                // Both operations should be performed, quit
                if (needDropKey && needNewKey)
                    return;
            }

            // If no operation supposed, check index for changes
            if (!needDropKey && !needNewKey)
            {
                // Search for index
                DataRow index = FindIndex(IndexDescriptor.PRIMARY);
                // If index founded and it is changed, then it means that index columns were
                // reordered, so we need to drop and recreate primary key.
                if (index != null && HasIndexChanged(index))
                {
                    needDropKey = true;
                    needNewKey = true;
                }
            }
        }

        /// <summary>
        /// Builds table primary key definition.
        /// </summary>
        /// <param name="target">String builder, used to build query.</param>
        /// <param name="prefix">
        /// Prefix to add before definition. Could be empty or "ADD "
        /// </param>
        private void BuildPrimaryKey(StringBuilder target, string prefix)
        {
            // Extract columns, marked as primary key            
            DataRow[] primaryKey = DataInterpreter.Select(
                                            IndexColumns,
                                            // Select columns which are in primary key
                                            IndexColumn.Index, IndexDescriptor.PRIMARY, 
                                            // Ensure that auto increment column is the first one.
                                            IndexColumn.Ordinal );

            // Check if primary key not empty
            if (primaryKey == null || primaryKey.Length <= 0)
                return;

            // Start primary key definition
            target.AppendLine();
            if (!String.IsNullOrEmpty(prefix))
                target.Append(prefix);
            target.Append("PRIMARY KEY (");

            // Write column names
            for (int i = 0; i < primaryKey.Length; i++)
            {
                if (i > 0) target.Append(", ");
                QueryBuilder.WriteIdentifier(primaryKey[i], IndexColumn.Name, target);
            }

            // End primary key definition
            target.Append("),");
        }

        /// <summary>
        /// Returns true if table already have primary key and false otherwise.
        /// </summary>
        private bool HasPrimaryKey
        {
            get
            {
                // Extract columns, marked as primary key            
                DataRow[] primaryKey = DataInterpreter.Select(Columns, Column.IsPrimaryKey, DataInterpreter.True);
                return primaryKey != null && primaryKey.Length > 0;
            }
        }
        #endregion

        #region Indexes SQL generation
        /// <summary>
        /// Build definitions changes in the indexes.
        /// </summary>
        /// <param name="target">String builder, used to build query.</param>
        /// <param name="prefix">
        /// Prefix to add before definition. Could be empty or "ADD "
        /// </param>
        private void BuildIndexes(StringBuilder target, string prefix)
        {
            // Iterate through indexes
            foreach (DataRow index in Indexes.Rows)
            {
                // Check index row state
                switch (index.RowState)
                {
                    // Create definition for new index
                    case DataRowState.Added:
                        BuildAddIndex(target, index, prefix);
                        break;
                    // Create drop index command
                    case DataRowState.Deleted:
                        BuildDropIndex(target, index);
                        break;
                    // Create DROP/ADD index, if needed
                    default:
                        if (HasIndexChanged(index))
                        {
                            BuildDropIndex(target, index);
                            BuildAddIndex(target, index, prefix);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Builds DROP INDEX statement
        /// </summary>
        /// <param name="target">String builder, used to build query.</param>
        /// <param name="index">DataRow with index data.</param>
        private void BuildDropIndex(StringBuilder target, DataRow index)
        {
            // Skip primary key, it will be processed separately
            string kind = DataInterpreter.GetStringNotNull(index, Index.IndexKind, DataRowVersion.Original);
            if (DataInterpreter.CompareInvariant(kind, IndexDescriptor.PRIMARY))
                return;

            target.Append("\nDROP INDEX ");
            QueryBuilder.WriteOldIdentifier(index, Index.Name, target);

            // Write trailing ","
            target.Append(",");
        }

        /// <summary>
        /// Builds ADD INDEX statement
        /// </summary>
        /// <param name="target">String builder, used to build query.</param>
        /// <param name="index">DataRow with index data.</param>
        /// <param name="prefix">
        /// Prefix to add before definition. Could be empty or "ADD "
        /// </param>
        private void BuildAddIndex(StringBuilder target, DataRow index, string prefix)
        {
            // Skip primary key, it will be processed separately
            string kind = DataInterpreter.GetStringNotNull(index, Index.IndexKind);
            if (DataInterpreter.CompareInvariant(kind, IndexDescriptor.PRIMARY))
                return;

            // Write new line
            target.AppendLine();
            
            // Write prefix if any
            if (!String.IsNullOrEmpty(prefix))
                target.Append(prefix);
            
            // Writing index kind
            QueryBuilder.WriteValue(kind, target, false);

            // If index kind is not INDEX, write INDEX
            if (!DataInterpreter.CompareInvariant(kind, IndexDescriptor.INDEX))
            {
                // Append space
                target.Append(' ');
                QueryBuilder.WriteValue(IndexDescriptor.INDEX, target, false);
            }

            // Append space
            target.Append(' ');

            // Write index name and append space
            QueryBuilder.WriteIdentifier(index, Index.Name, target);

            // For UNIQUE and INDEX indexes index type is available
            if (DataInterpreter.CompareInvariant(kind, IndexDescriptor.INDEX)
                || DataInterpreter.CompareInvariant(kind, IndexDescriptor.UNIQUE))
                QueryBuilder.WriteIfNotEmptyString(index, Index.IndexType, " USING ", target, false);

            
            // Extract all index columns
            DataRow[] columns = DataInterpreter.Select(
                                    IndexColumns,
                                    IndexColumn.Index,
                                    DataInterpreter.GetStringNotNull(index, Index.Name),
                                    IndexColumn.Ordinal);

            // Validate array
            if (columns == null)
            {
                Debug.Fail("Failed to get index columns!");
                return;
            }

            // Write source columns
            target.Append(" (");
            for (int i = 0; i < columns.Length; i++)
            {
                // Write comma if not the first column
                if (i > 0)
                    target.Append(", ");
                
                // Write column name
                QueryBuilder.WriteIdentifier(columns[i], IndexColumn.Name, target);
                
                // Get index length and write it if not null
                Nullable<Int64> length = DataInterpreter.GetInt(columns[i], IndexColumn.IndexLength);
                if (length != null)
                    target.AppendFormat(CultureInfo.InvariantCulture, "({0})", length.Value);

            }
            target.Append(")");

            // Write trailing ','
            target.Append(",");
        }

        /// <summary>
        /// Returns true if given index was changed.
        /// </summary>
        /// <param name="index">DataRow with description of the index.</param>
        /// <returns>Returns true if given index was changed.</returns>
        private bool HasIndexChanged(DataRow index)
        {
            // Check index for changes
            if (DataInterpreter.HasChanged(index))
                return true;

            // Get list of all index columns
            DataRow[] columns = DataInterpreter.Select(
                                    IndexColumns,
                                    IndexColumn.Index,
                                    DataInterpreter.GetStringNotNull(index, Index.Name));

            // Check columns
            if (columns == null)
            {
                Debug.Fail("Failed to get index columns!");
                return false;
            }

            // Check each column
            foreach (DataRow column in columns)
                if (HasIndexColumnChanged(column))
                    return true;

            // Look for deleted columns
            string oldName = DataInterpreter.GetStringNotNull(index, Index.Name, DataRowVersion.Original);
            foreach (DataRow indexColumn in IndexColumns.Rows)
            {
                // Only deleted columns are intrusting
                if (indexColumn.RowState != DataRowState.Deleted)
                    continue;

                // Extract original index name
                string indexName = DataInterpreter.GetStringNotNull(indexColumn, IndexColumn.Index, DataRowVersion.Original);

                // If exact like current idex name, then it is changed
                if (DataInterpreter.CompareInvariant(oldName, indexName))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if given index column was changed.
        /// </summary>
        /// <param name="column">DataRow with the index column description.</param>
        /// <returns>Returns true if given index column was changed.</returns>
        private bool HasIndexColumnChanged(DataRow column)
        {
            // If changed something besides source column name, return true.
            if (column.RowState == DataRowState.Deleted
                || column.RowState == DataRowState.Added
                || DataInterpreter.HasChanged(column, IndexColumnName) )
                return true;

            // If source column name is unchanged, return false.
            if (!DataInterpreter.HasChanged(column, IndexColumn.Name))
                return false;

            // Get new column name
            string newColumnName = DataInterpreter.GetStringNotNull(column, IndexColumn.Name);
            string oldColumnName = DataInterpreter.GetStringNotNull(column, IndexColumn.Name, DataRowVersion.Original);
            if (String.IsNullOrEmpty(newColumnName))
            {
                Debug.Fail("Failed to get column names!");
                return false;
            }

            // Get source column description
            DataRow sourceColumn = FindColumn(newColumnName);
            if (sourceColumn == null)
            {
                Debug.Fail("Unable to get source column description!");
                return false;
            }

            // If column is new, then return true
            if (sourceColumn.RowState == DataRowState.Added)
                return true;

            // If column is deleted, when it is error
            if (sourceColumn.RowState == DataRowState.Deleted)
            {
                Debug.Fail("Deleted column attached to index!");
                return false;
            }

            // Get old name in column
            string oldNameInColumn = DataInterpreter.GetStringNotNull(sourceColumn, Column.Name, DataRowVersion.Original);
            if (String.IsNullOrEmpty(oldNameInColumn))
            {
                Debug.Fail("Failed to get old name for source column!");
                return false;
            }

            // Compare names (if they are equals, then index column should be considered as unchanged)
            return !DataInterpreter.CompareInvariant(oldColumnName, oldNameInColumn);
        }
        #endregion

        #region Handling attribute changes
        /// <summary>
        /// Handles table attributes changes.
        /// </summary>
        protected override void HandleAttributesChanges()
        {
            base.HandleAttributesChanges();

            // If caharacter set changed, need to change collation to default
            // for new character set.
            if (IsAttributeChanged(Table.CharacterSet))
                HandleCharacterSetChanged();
        }

        /// <summary>
        /// Reset collation to default if character set was changed.
        /// </summary>
        private void HandleCharacterSetChanged()
        {
            if (!DataInterpreter.CompareInvariant(
                        previousCharacterSet, 
                        CharacterSet))
            {
                previousCharacterSet = CharacterSet;
                Collation = Connection.GetDefaultCollationForCharacterSet(CharacterSet);
            }
        }

        // We don't know what attribute exactly was changed at this moment
        // so, we need to store previously processed values.
        #region Support private variables
        /// <summary>
        /// Used to store previously porcessed character set name. DataRow.Original
        /// is not enough.
        /// </summary>
        private string previousCharacterSet = String.Empty; 
        #endregion

        #endregion

        #region Aditional private methods
        /// <summary>
        /// Returns DataRow for column with given name.
        /// </summary>
        /// <param name="columnName">Name of column to search.</param>
        /// <returns>Returns DataRow for column with given name.</returns>
        private DataRow FindColumn(string columnName)
        {
            return Columns.Rows.Find(new object[] { Schema, OldName, columnName });
        }


        /// <summary>
        /// Returns DataRow for foreign key with given name.
        /// </summary>
        /// <param name="keyName">Name of foreign key to search.</param>
        /// <returns>Returns DataRow for foreign key with given name.</returns>
        private DataRow FindForeignKey(string keyName)
        {
            return ForeignKeys.Rows.Find(new object[] { Schema, OldName, keyName });
        }

        /// <summary>
        /// Returns DataRow for index with given name.
        /// </summary>
        /// <param name="indexName">Name of index to search.</param>
        /// <returns>Returns DataRow for index with given name.</returns>
        private DataRow FindIndex(string indexName)
        {
            return Indexes.Rows.Find(new object[] { Schema, OldName, indexName });
        }

        /// <summary>
        /// Returns DataRow for index column with given name.
        /// </summary>
        /// <param name="indexName">Name of index to search.</param>
        /// <param name="columnName">Name of column to search.</param>
        /// <returns>Returns DataRow for index with given name.</returns>
        private DataRow FindIndexColumn(string indexName, string columnName)
        {
            return IndexColumns.Rows.Find(new object[] { Schema, OldName, indexName, columnName });
        }

        /// <summary>
        /// Returns copy of given table where all data rows have state Added.
        /// </summary>
        /// <param name="table">DataTablw to copy.</param>
        /// <returns>Returns copy of given table where all data rows have state Added.</returns>
        private DataTable MakeCopy(DataTable table)
        {
            // Clone structure
            DataTable result = table.Clone();

            // Copy data
            foreach (DataRow row in table.Rows)
                result.Rows.Add(row.ItemArray);

            return result;
        }

        /// <summary>
        /// Sets correct table name for all rows to support Find methods.
        /// </summary>
        /// <param name="table">DataTablw to process.</param>
        /// <param name="attribute">Name of table name attribute.</param>
        private void ResetTableName(DataTable table, string attribute)
        {            
            foreach (DataRow row in table.Rows)
                DataInterpreter.SetValueIfChanged(row, attribute, OldName);            
        }


        /// <summary>
        /// Generates new name for cloned foreign key to avoid conflicts. Some
        /// times InnoDB fails if there is another key with same name in the other
        /// table.
        /// </summary>
        /// <param name="key">Foreign key row to generate new name for.</param>
        private void GenerateNewName(DataRow key)
        {
            // Extract name
            string name = DataInterpreter.GetStringNotNull(key, ForeignKey.Name);
            
            // Select foreign key columns (from old table because not copied yet)
            DataRow[] columns = DataInterpreter.Select(ForeignKeysColumns, ForeignKeyColumn.ForeignKeyName, name);

            // Generate new name template
            string template = name + "_";

            // Identifier to complete
            object[] id = new object[] { null, Schema, null /*for all tables*/, null };

            // Complete identifier
            ObjectDescriptor.CompleteNewObjectID(Hierarchy, ForeignKeyDescriptor.TypeName, ref id, template);

            // Set new name for key
            DataInterpreter.SetValueIfChanged(key, ForeignKey.Name, id[3]);

            // Cahnge key name for each column
            foreach (DataRow column in columns)
                DataInterpreter.SetValueIfChanged(column, ForeignKeyColumn.ForeignKeyName, id[3]);
        }

        /// <summary>
        /// Makes given column first in the primary key index.
        /// </summary>
        /// <param name="firstColumn">Column to make first in the primary key index.</param>
        private void MakeFirstPrimaryKeyColumn(DataRow firstColumn)
        {
            // Set this index column to be the first
            int ordinal = 1;
            DataInterpreter.SetValueIfChanged(firstColumn, IndexColumn.Ordinal, ordinal++);

            // Select all primary key columns
            DataRow[] columns = DataInterpreter.Select(
                                    IndexColumns,
                                    IndexColumn.Index,
                                    IndexDescriptor.PRIMARY,
                                    IndexColumn.Ordinal);

            // Validate enumerated columns
            if (columns == null)
            {
                Debug.Fail("Failed to enumerate primary key columns!");
                return;
            }

            // Set new ordinal for each column
            foreach (DataRow column in columns)
            {
                // Skip first column
                if (column == firstColumn)
                    continue;

                // Set new ordinal
                DataInterpreter.SetValueIfChanged(column, IndexColumn.Ordinal, ordinal++);
            }
        }

        /// <summary>
        /// Calls base class SaveFailed method. Switch names before and after call to prevent
        /// editor caption changes.
        /// </summary>
        private void CallBaseSaveFailed()
        {
            string newName = Name;
            Name = OldName;
            base.SaveFailed();
            Name = newName;
        }
        #endregion

        #region Private variables to store properties
        private DataTable columnsTable;
        private DataTable foreignKeysTable;
        private DataTable foreignKeysColumnsTable;
        private DataTable indexesTable;
        private DataTable indexColumnsTable;
        private Dictionary<string, object> columnDefaults = new Dictionary<string,object>();
        #endregion

        #region ICharacterSetProvider Members
        /// <summary>
        /// Returns character set to select default collations.
        /// </summary>
        string CollationConverter.ICharacterSetProvider.CharcterSet
        {
            get
            {
                return this.CharacterSet;
            }
        }
        #endregion

        #region Private constants
        private static string[] IndexColumnName = new string[] { IndexColumn.Name };
        #endregion
    }
}
