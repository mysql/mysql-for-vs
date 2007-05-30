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

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using System.Diagnostics;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Data;
using System.Windows.Forms.Design;
using MySql.Data.VisualStudio.Properties;
using System.Windows.Forms;
using MySql.Data.VisualStudio.Utils;
using Microsoft.VisualStudio.Shell;
using System.Globalization;
using Microsoft.VisualStudio.OLE.Interop;
using System.Data;
using System.ComponentModel;
using System.Data.SqlTypes;
using CommonObject = MySql.Data.VisualStudio.Descriptors.ObjectDescriptor.Attributes;
using MySql.Data.VisualStudio.Descriptors;
using System.ComponentModel.Design;
using System.Drawing.Design;
using MySql.Data.VisualStudio.Dialogs;
using System.Data.Common;

namespace MySql.Data.VisualStudio.DocumentView
{
    /// <summary>
    /// This is base class for all documents. Implements IVsPersistDocData interface 
    /// and converts it to set of virtual and abstract methods.
    /// </summary>
    public abstract class BaseDocument : IDocument
    {
        #region Constants
        /// <summary>Constants for converting enumeration values to MySql-compatible 
        /// values</summary>
        private const char Space = ' ';
        private const char Underscore = '_';
        #endregion

        #region Initialization
        /// <summary>
        /// This constructor initialize private variables and reads columns list.
        /// </summary>
        /// <param name="hierarchy">
        /// Server explorer facade object to be used for Server Explorer hierarchy interaction. 
        /// Also used to extract connection.
        /// </param>
        /// <param name="schemaName">Name of schema to which owns this database object.</param>
        /// <param name="objectName">Name of this database object.</param>
        /// <param name="isNew">
        /// Indicates if this instance represents new database object doesn’t fixed in 
        /// database yet.
        /// </param>
        protected BaseDocument(ServerExplorerFacade hierarchy, bool isNew, object[] id)
        {
            // Check input paramaters
            if (hierarchy == null)
                throw new ArgumentNullException("hierarchy");
            if (id == null)
                throw new ArgumentNullException("id");
            if (id.Length != ObjectDescriptor.GetIdentifierLength(TypeName))
                throw new ArgumentException(
                    String.Format(
                        CultureInfo.CurrentCulture,
                        Resources.Error_InvlaidIdentifier,
                        id.Length, 
                        TypeName,
                        ObjectDescriptor.GetIdentifierLength(TypeName)),
                     "id");
            
            // Storing inputs in private variables
            this.hierarchyRef = hierarchy;
            this.connectionRef = Hierarchy.Connection;
            this.schemaVal = GetSchemaFromID(id);
            this.nameVal = GetNameFromID(id);         
            this.isNewVal = isNew;
        }
        #endregion        

        #region Database object properties
        /// <summary>
        /// Name of schema to which owns this database object. This property is read-only.
        /// </summary>
        [LocalizableCategory("Category_Identifier")]
        [LocalizableDescription("Description_Object_Schema")]
        [LocalizableDisplayName("DisplayName_Object_Schema")]
        public virtual string Schema
        {
            get
            {
                return schemaVal;
            }
        }

        /// <summary>
        /// Name of this database object.
        /// </summary>
        [LocalizableCategory("Category_Identifier")]
        [LocalizableDescription("Description_Object_Name")]
        [LocalizableDisplayName("DisplayName_Object_Name")]
        public virtual string Name
        {
            get
            {
                // If object is cloned, return its cloned name
                if (!String.IsNullOrEmpty(clonnedName))
                    return clonnedName;
                
                // It is necessary for all identifier parts to be accessible even 
                // before attributes are loaded.
                if (!IsAttributesLoaded)                    
                    return nameVal;

                return GetAttributeAsString(Descriptor.NameAttributeName);
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                SetAttribute(Descriptor.NameAttributeName, value);
            }
        }

        /// <summary>
        /// Unchanged name of the object which is equal to database name.
        /// </summary>
        [Browsable(false)]
        public string OldName
        {
            get
            {
                return GetOldAttributeAsString(Descriptor.NameAttributeName);
            }
        }

        /// <summary>
        /// Database object comments.
        /// </summary>
        [LocalizableCategory("Category_Base")]
        [LocalizableDescription("Description_Object_Comment")]
        [LocalizableDisplayName("DisplayName_Object_Comment")]
        [Editor(typeof(MultilineStringEditor),typeof(UITypeEditor))]
        public virtual string Comments
        {
            get
            {
                return GetAttributeAsString(CommentAttributeName);
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                SetAttribute(CommentAttributeName, value);
            }
        }

        /// <summary>
        /// Unchanged comments of the object.
        /// </summary>
        [Browsable(false)]
        public string OldComments
        {
            get
            {
                return GetOldAttributeAsString(CommentAttributeName);
            }
        }
        #endregion

        #region Public properties and methods

        /// <summary>
        /// Name of server host which owns this database. This property is read-only.
        /// </summary>
        [LocalizableCategory("Category_Identifier")]
        [LocalizableDescription("Description_Object_Server")]
        [LocalizableDisplayName("DisplayName_Object_Server")]
        public string Server
        {
            get
            {
                return Connection.ServerName;
            }
        }

        /// <summary>
        /// Indicates if this instance represents new database object 
        /// doesn’t fixed in database yet.
        /// </summary>
        [Browsable(false)]
        public bool IsNew
        {
            get
            {
                return isNewVal;
            }
        }

        /// <summary>
        /// Name of the document object type, as it declared in the DataObjectSupport 
        /// XML.
        /// </summary>
        [Browsable(false)]
        public string TypeName
        {
            get
            {
                DocumentObjectAttribute attribute = ReflectionHelper.GetDocumentObjectAttribute(this.GetType());
                if (attribute == null)
                    throw new NotSupportedException(Resources.Error_NotMarkedAsDocument);
                return attribute.DocumentTypeName;
            }
        }

        /// <summary>
        /// ID of this object as array of strings (null, schema name, object name 
        /// by default).
        /// </summary>
        [Browsable(false)]
        public virtual object[] ObjectID
        {
            get
            {
                return new object[] { null, Schema, Name };
            }
        }

        /// <summary>
        /// Pointer to the IDE facade object.
        /// </summary>
        [Browsable(false)]
        public ServerExplorerFacade Hierarchy
        {
            get
            {
                return hierarchyRef;
            }
        }
        
        /// <summary>
        /// Data connection object used to interact with MySQL server. This property 
        /// is read-only.
        /// </summary>
        [Browsable(false)]
        public DataConnectionWrapper Connection
        {
            get
            {
                return connectionRef;
            }
        }

        /// <summary>
        /// ID of this object as array of strings (null, schema name, object name 
        /// by default) before changes.
        /// </summary>
        [Browsable(false)]
        public virtual object[] OldObjectID
        {
            get
            {
                return new object[] { null, Schema, OldName };
            }
        }

        /// <summary>
        /// Changes name of this object and detach it from underlying database object. This object
        /// will be considered as new after this method is called. This method should be used before
        /// data are loaded.
        /// </summary>
        /// <param name="newName">New name to use for object.</param>
        public void CloneToName(string newName)
        {
            // Store new name to aplly it after data would be loaded.
            clonnedName = newName;
        }
        #endregion

        #region Events
        /// <summary>
        /// This event fires after complete load of document data.
        /// </summary>
        [Browsable(false)]
        public event EventHandler DataLoaded;

        /// <summary>
        /// This event fires after any changes in the document data hapens
        /// </summary>
        [Browsable(false)]
        public event EventHandler DataChanged;

        /// <summary>
        /// This event fires before object data are saved.
        /// </summary>
        [Browsable(false)]
        public event CancelEventHandler Saving;

        /// <summary>
        /// This event fires after object data are successfully saved.
        /// </summary>
        [Browsable(false)]
        public event EventHandler SuccessfullySaved;

        /// <summary>
        /// This event fires if attempt to save document fails.
        /// </summary>
        [Browsable(false)]
        public event EventHandler SaveAttemptFailed;
        #endregion

        #region Virtual document support methods and properties
        /// <summary>
        /// Dirty flag used to determine whenever data are changed or not.
        /// </summary>
        [Browsable(false)]
        protected virtual bool IsDirty
        {
            get
            {
                Debug.Assert(Attributes != null, "Attributes are not read!");
                if (Attributes == null)
                    return false;

                // Check, if table contains all necessary columns
                foreach (DataColumn column in Attributes.Table.Columns)
                    if (IsAttributeChanged(column.ColumnName))
                        return true;
                
                // Attributes are not changed
                return false;
            }
        }

        /// <summary>
        /// Build SQL query to create new instance of this object in 
        /// database.
        /// </summary>
        /// <returns>Create SQL query.</returns>
        protected abstract string BuildCreateQuery();

        /// <summary>
        /// Builds query to modify properties of existing object in database.
        /// </summary>
        /// <returns>Alter SQL query.</returns>
        protected abstract string BuildAlterQuery();

        /// <summary>
        /// Builds query to pre-drop object before recreating it. Using for stored procedures,
        /// indexes, UDF's and other dabase objects which can not be altered and should be 
        /// recreated instead.
        /// </summary>
        /// <returns>Returns SQL statement to pre-drop dabase object.</returns>
        protected virtual string BuildPreDropQuery()
        {
            return String.Empty;
        }

        /// <summary>
        /// Validates scalar result of create or alter query. In base implementation always returns true.
        /// </summary>
        /// <param name="result">Scalar result code returned by query execution.</param>
        /// <returns>Returns true if given result indicates succesfull saving.</returns>
        protected virtual bool ValidateSaveResult(object result)
        {
            return true;
        }

        /// <summary>
        /// Validates scalar result of pre-drop query. In base implementation always returns true.
        /// </summary>
        /// <param name="result">Scalar result code returned by query execution.</param>
        /// <returns>Returns true if given result indicates succesfull pre-dropping.</returns>
        protected virtual bool ValidatePreDropResult(object result)
        {
            return true;
        }

        /// <summary>
        /// This method is called when object data are successfully changes. Document 
        /// should perform all necessary operations to remove IsDirty flag.
        /// </summary>
        protected virtual void AcceptChanges()
        {
            // Rename document in the RDT if moniker was changed
            if (!DataInterpreter.CompareInvariant(OldMoniker, Moniker))
            {
                // Get new hierarchy item identifier
                int itemID = HierarchyItemID;
                // Rename document to the new moniker in the RDT
                Hierarchy.Rename(OldMoniker, Moniker, itemID);
                // Set new name for hierarchy node
                Hierarchy.SetName(HierarchyItemID, Name);
            }

            // Object is not new any more
            isNewVal = false;

            // Accept attributes changes
            Attributes.AcceptChanges();
        }

        /// <summary>
        /// Saves database object changes to database.
        /// </summary>
        /// <param name="silent">
        /// If this flag set to true, save should be performed without any user interaction.
        /// </param>
        /// <returns>
        /// Returns true if save operation succeed. If user cancels operation or SQL query fails, 
        /// this method returns false.
        /// </returns>
        protected virtual bool SaveData(bool silent)
        {
            // Build SQL query
            string query;
            string preDropQuery = String.Empty;
            if (IsNew)
                query = BuildCreateQuery(); // For new object builds create query
            else
            {
                // For existing object builds pre-drop and alter query
                preDropQuery = BuildPreDropQuery();
                query = BuildAlterQuery();
            }
            
            // Check query
            if (String.IsNullOrEmpty(query))
                return false;

            // Confirms query execution if not in silent mode
            if (!silent)
            {
                // Build query to show
                string queryToShow = String.IsNullOrEmpty(preDropQuery) ? query : preDropQuery + ";\r\n" + query;
                if (SqlPreviewDialog.Show(queryToShow) != DialogResult.OK)
                    return false;
            }

            // Executes pre-drop query (if any) and validate results
            if (!String.IsNullOrEmpty(preDropQuery)
                && !ValidatePreDropResult(Connection.ExecuteScalar(preDropQuery)))
            {
                Debug.Fail("Failed to pre drop object!");
                return false;
            }

            //Executes query and validates result
            if (ValidateSaveResult(Connection.ExecuteScalar(query)))
            {
                // Need to drop if identifier was changed
                bool needDrop = IsAttributeChanged(Descriptor.NameAttributeName);
                string oldMoniker = OldMoniker;

                // Raises ObjectChangeEvents. We don't know what for, but documentation sais their
                // should be raised.
                if (IsNew)
                {
                    // If object is new, raise added event
                    Connection.ObjectChangeEvents.RaiseObjectAdded(TypeName, ObjectID);
                }
                else
                {
                    // If object was renamed, raise changed with old and new ID, else only with new ID.
                    if (needDrop)
                        Connection.ObjectChangeEvents.RaiseObjectChanged(TypeName, OldObjectID, ObjectID);
                    else
                        Connection.ObjectChangeEvents.RaiseObjectChanged(TypeName, ObjectID);
                }

                // Drop current coresponding node, because it can contains wrong object
                // identifier and cause Server Explorer to display warning message
                if (needDrop)
                    ResetHierarchyItem();                

                // Accept all changes to object
                AcceptChanges();

                // Refreshing server explorer
                if (!silent)
                    Hierarchy.Refresh();    
                
                // Reload object data
                if (ReloadData())
                    return false;

                // Fire successfully saved for vies to update them
                FireSuccessfullySaved();

                // Finaly, return true
                return true;
            }

            // ValidateSaveResult fails
            return false;
        }

        /// <summary>
        /// Validates document data before saving.
        /// </summary>
        /// <returns>
        /// Returns true if document is consistent and can be saved. 
        /// Returns false otherwize.
        /// </returns>
        protected virtual bool ValidateData()
        {
            // Ask listenters what do they think
            if (!FireSaving())
                return false;

            // Name should not be empty
            if (String.IsNullOrEmpty(Name))
            {
                UIHelper.ShowError(Resources.Error_EmptyName);
                return false;
            }

            // Validate object name
            if (!Parser.IsValidIdentifier(Name))
            {
                UIHelper.ShowError(String.Format(
                   Resources.Error_InvalidName,
                   Name));
                return false;
            }

            // If object is new, we need to check for existen objects
            if (IsNew)
            {
                // Check for existent object
                DataTable current = ObjectDescriptor.EnumerateObjects(Connection, TypeName, ObjectID);
                if (current != null && current.Rows != null && current.Rows.Count > 0)
                {
                    UIHelper.ShowError(String.Format(
                       Resources.Error_UnableToCreateObjectExists,
                       Name));
                    return false;
                }

                // Check for opened editors if name was changed
                if (!DataInterpreter.CompareInvariant(OldName, Name) && Hierarchy.HasDistinctDocument(Moniker, DocumentCookie))
                {
                    UIHelper.ShowError(String.Format(
                      Resources.Error_UnableToCreateEditorOpened,
                      Name));
                    return false;
                }
            }

            // If object is to be renamed, we need to check for existen objects
            if (!DataInterpreter.CompareInvariant(OldName, Name))
            {
                // Check for existent object
                DataTable current = ObjectDescriptor.EnumerateObjects(Connection, TypeName, ObjectID);
                if (current != null && current.Rows != null && current.Rows.Count > 0)
                {
                    UIHelper.ShowError(String.Format(
                       Resources.Error_UnableToRenameObjectExists,
                       OldName,
                       Name));
                    return false;
                }

                // Check for opened editors
                if (Hierarchy.HasDistinctDocument(Moniker, DocumentCookie))
                {
                    UIHelper.ShowError(String.Format(
                      Resources.Error_UnableToRenameEditorOpened,
                      OldName,
                      Name));
                    return false;
                }
            }

            // Finaly, return true
            return true;
        }

        /// <summary>
        /// Load database object from database.
        /// </summary>
        /// <param name="reloading">
        /// This flag indicates that object is reloading. Should be ignored in most cases.
        /// </param>
        /// <returns>Returns true if load succeeds and false otherwise.</returns>
        protected virtual bool LoadData(bool reloading)
        {
            // To read object attributes execute enumeration query for
            // object type and with given ID.
            DataTable objectTable = ObjectDescriptor.EnumerateObjects(
                                                        Connection, 
                                                        TypeName,
                                                        ObjectIDForLoad);
            
            // Check results
            if (objectTable == null || (objectTable.Rows.Count == 0 && !IsNew))
            {
                Debug.Fail("Unable to read data for object type '" + TypeName + "'");
                return false;
            }

            // Fill attributes for new object
            if (IsNew)
            {
                Debug.Assert(objectTable.Rows.Count == 0, "Another row exists in the table for new object!");
                // Create new row
                DataRow newRow = objectTable.NewRow();

                // Fill row with data
                FillNewObjectAttributes(newRow);

                // Add row to the table
                objectTable.Rows.Add(newRow);
            }

            // Extract attributes row
            DataRow attributesRow = objectTable.Rows[0];
            Debug.Assert(attributesRow != null, "Failed to extract attributes row!");
            
            // Accept changes
            objectTable.AcceptChanges();

            // Store attributes row
            lock (this)
            {
                // Reseting attributes
                if (reloading)
                    ResetAttributes();  
                attributes = attributesRow;
            }

            // Connect to table notifications 
            objectTable.RowChanged += new DataRowChangeEventHandler(OnAttributesRowChanged);
            
            return true;
        }

        /// <summary>
        /// Reloads document data and doesn't throws an exception in any case.
        /// </summary>
        /// <returns>Returns true if reload is succeeded and false otherwize.</returns>
        protected virtual bool ReloadData()
        {
            // Reload document data using interface method to avoid exception
            return (this as IVsPersistDocData).ReloadDocData(0) != VSConstants.S_OK;
        }

        /// <summary>
        /// Initialize attributes for the new object.
        /// </summary>
        /// <param name="newRow">DataRow object to receive new object attributes.</param>
        protected virtual void FillNewObjectAttributes(DataRow newRow)
        {
            if (newRow == null)
                throw new ArgumentNullException("newRow");

            newRow[Descriptor.SchemaAttributeName] = Schema;
            newRow[Descriptor.NameAttributeName] = Name;
        }

        /// <summary>
        /// Releases all specific resources. In most cases do nothing.
        /// </summary>
        protected virtual void Close()
        {
        }

        /// <summary>
        /// Determines if current object can be reloaded. By default, 
        /// all objects are reloadable.
        /// </summary>
        [Browsable(false)]
        protected virtual bool IsReloadable
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Returns name of the Comments attribute. Inheritors may override it 
        /// if it differs from the standard "{TYPE}_COMMENT"
        /// </summary>
        [Browsable(false)]
        protected virtual string CommentAttributeName
        {
            get
            {
                return String.Format(
                                CommonObject.Comments,
                                TypeName.ToUpperInvariant(),
                                CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// This method is caled then attribtues of this object are changed
        /// </summary>
        protected virtual void HandleAttributesChanges()
        {
        }


        /// <summary>
        /// Extracts Schema part from the object identifier.
        /// </summary>
        /// <param name="id">Object identifier to process.</param>
        /// <returns>Returns Schema part extracted from the object identifier.</returns>
        protected virtual string GetSchemaFromID(object[] id)
        {
            if (id == null)
                throw new ArgumentNullException("id");
            if (id.Length != ObjectDescriptor.GetIdentifierLength(TypeName)
                || String.IsNullOrEmpty(id[1] as string))
                throw new ArgumentException(
                    String.Format(
                        CultureInfo.CurrentCulture,
                        Resources.Error_InvlaidIdentifier,
                        id.Length,
                        TypeName,
                        ObjectDescriptor.GetIdentifierLength(TypeName)),
                     "id");
            return id[1] as string;
        }

        /// <summary>
        /// Extracts Name part from the object identifier.
        /// </summary>
        /// <param name="id">Object identifier to process.</param>
        /// <returns>Returns Name part extracted from the object identifier.</returns>
        protected virtual string GetNameFromID(object[] id)
        {
            if (id == null)
                throw new ArgumentNullException("id");
            if (id.Length != ObjectDescriptor.GetIdentifierLength(TypeName)
                || String.IsNullOrEmpty(id[id.Length - 1] as string))
                throw new ArgumentException(
                    String.Format(
                        CultureInfo.CurrentCulture,
                        Resources.Error_InvlaidIdentifier,
                        id.Length,
                        TypeName,
                        ObjectDescriptor.GetIdentifierLength(TypeName)),
                     "id");

            // A name of an object is always the last identifier part
            return id[id.Length - 1] as string;
        }

        /// <summary>
        /// Resets state of all document data to the New. This method is used for cloning
        /// </summary>
        protected virtual void ResetToNew()
        {
            // Mark document as new
            isNewVal = true;

            // Check attributes
            if (Attributes == null || Attributes.Table == null)
            {
                Debug.Fail("Unable to clone because not loaded yet!");
                return;
            }

            // Dropping the node if it exists in a node tree, and creating the new one
            if (Hierarchy.DropObjectNode(HierarchyItemID))
            {
                hierarchyItemIDVal = Hierarchy.CreateObjectNode();
                Hierarchy.Rename(OldMoniker, Moniker, HierarchyItemID);
            }

            // Extract table
            DataTable table = attributes.Table;

            // Disconnect from events first
            table.RowChanged -= new DataRowChangeEventHandler(OnAttributesRowChanged);

            // Make new row for clone
            DataRow newRow = table.NewRow();
            FillNewObjectAttributes(newRow);

            // Set new clonned name
            if (clonnedName != null)
            {
                // Set name in the attributes
                newRow[Descriptor.NameAttributeName] = clonnedName;
                
                // Rename in the hierarchy
                Hierarchy.SetName(HierarchyItemID, clonnedName);
            }

            // Store attributes
            object[] oldAttributes = Attributes.ItemArray;

            // Delete source attributes row
            Attributes.Delete();

            // Add new row to the table
            table.Rows.Add(newRow);

            // Accept changes to store original version.
            table.AcceptChanges();

            // Fill row with data
            foreach (DataColumn column in table.Columns)
                if (column != null && !DataInterpreter.CompareInvariant(column.ColumnName, Descriptor.NameAttributeName))
                    DataInterpreter.SetValueIfChanged(newRow, column.ColumnName, oldAttributes[column.Ordinal]);

            // Replace attributes with new row
            attributes = newRow;

            // Connect to events again
            table.RowChanged += new DataRowChangeEventHandler(OnAttributesRowChanged);

            // Remove cloned state
            clonnedName = null;
        }


        /// <summary>
        /// Called if save failed. Base implementation fires propper event.
        /// </summary>
        protected virtual void SaveFailed()
        {
            // If object is not new, check for posible pre-drop
            if (!IsNew)
            {
                // Enumerate current object to chek, if it was pre-dropped
                DataTable current = ObjectDescriptor.EnumerateObjects(Connection, TypeName, OldObjectID);
                if (current == null || current.Rows == null || current.Rows.Count <= 0)
                {
                    // Reset hierarchy item, because it is not functional.
                    ResetHierarchyItem();

                    // Warn user
                    UIHelper.ShowWarning(String.Format(
                        CultureInfo.CurrentCulture,
                        Resources.Warning_ObjectPreDropped,
                        OldName));
                    
                    // Clone object to new name and reset its state
                    CloneToName(Name);
                    ResetToNew();

                    // Refresh server explorer hierarchy to be sure
                    Hierarchy.Refresh();
                }
            }

            FireSaveAttemptFailed();
        }
        #endregion

        #region IVsPersistDocData Members
        /// <summary>
        /// This interface method must check if document data changed or not. 
        /// IsDirty virtual property is used to detect changes.
        /// </summary>
        /// <param name="pfDirty">
        /// Out parameter must be set to 1 if there are some changes and to 0 otherwise.
        /// </param>
        /// <returns>
        /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
        /// </returns>
        int IVsPersistDocData.IsDocDataDirty(out int pfDirty)
        {
            try
            {
                // IsDirty virtual property is used to detect changes.
                pfDirty = IsNew ||IsDirty ? 1 : 0;
            }
            catch (Exception e)
            {
                Trace.TraceError("Error during checking for changes:\n{0}", e.ToString());
                // Return unspecified error
                pfDirty = 0;
                return VSConstants.S_FALSE;
            }
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Saves document data. At this point proper SQL query should be prepared and executed 
        /// to fix object changes in database.
        /// </summary>
        /// <param name="dwSave">Flags whose values are taken from the VSSAVEFLAGS enumeration.</param>
        /// <param name="pbstrMkDocumentNew">Pointer to the path to the new document.</param>
        /// <param name="pfSaveCanceled">1 if the document was not saved.</param>
        /// <returns>
        /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
        /// </returns>
        int IVsPersistDocData.SaveDocData(
            Microsoft.VisualStudio.Shell.Interop.VSSAVEFLAGS dwSave,
            out string pbstrMkDocumentNew,
            out int pfSaveCanceled)
        {
            // Initialize output parameters for negative case
            pbstrMkDocumentNew = String.Empty;
            pfSaveCanceled = 1;

            try
            {
                // Validate data and fire saving event for views to fix all changes
                if (!ValidateData())
                {
                    pfSaveCanceled = 1;
                    // Activates editor, which causes validation failure (note that old identifier should be used)
                    if (Hierarchy != null)
                        Hierarchy.FindAndActivateEditor(HierarchyItemID, TypeName, OldObjectID);
                    return VSConstants.S_OK;
                }

                if (IsDirty || IsNew)
                {
                    // If SaveData failed (returns false) set pfSaveCanceled flag to 1
                    pfSaveCanceled = SaveData((dwSave & VSSAVEFLAGS.VSSAVE_SilentSave) != 0) ? 0 : 1;
                }
                else
                {
                    // Nothing to save, but it's OK
                    pfSaveCanceled = 0;
                }
                // Retrives new moniker
                pbstrMkDocumentNew = Moniker.ToLowerInvariant();
            }
            catch (DbException e)
            {
                Trace.TraceError("Error during saving object:\n{0}", e.ToString());
                SqlErrorDialog.ShowError(e, Connection.GetFullStatus());
                // Notify all about failure
                SaveFailed();
                // Return unspecified error
                pfSaveCanceled = 1;
                return VSConstants.S_FALSE;
            }
            catch (Exception e)
            {
                Trace.TraceError("Error during saving object:\n{0}", e.ToString());
                UIHelper.ShowError(e);
                // Notify all about failure
                SaveFailed();
                // Return unspecified error
                pfSaveCanceled = 1;
                return VSConstants.S_FALSE;
            }
            // Return S_OK
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Loads the document data from a given MkDocument.
        /// </summary>
        /// <param name="pszMkDocument">Document moniker.</param>
        /// <returns>
        /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
        /// </returns>
        int IVsPersistDocData.LoadDocData(string pszMkDocument)
        {
            // pszMkDocument must be the same as Moniker
            if (!DataInterpreter.CompareInvariant(pszMkDocument, Moniker))
            {
                Debug.Fail("Trying to load from different moniker!");
                return VSConstants.E_INVALIDARG;
            }
            try
            {
                // Initialy load data
                if (!LoadData(false))
                {
                    // Close document and exit
                    UrgentlyCloseDocument(null);
                    return VSConstants.S_FALSE;
                }

                // Check if we should create cloned object. In that case we should reset state of 
                // all data to new
                if (!String.IsNullOrEmpty(clonnedName))
                    ResetToNew();

                // Fire DataLoaded
                FireDataLoaded();
            }
            catch (Exception e)
            {
                Trace.TraceError("Error during loading object:\n{0}", e.ToString());
                // Close document and exit
                UrgentlyCloseDocument(e);
                // Return unspecified error
                return VSConstants.S_FALSE;
            }
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Returns the unique identifier of the editor factory that created the 
        /// IVsPersistDocData object. Unfortunately, our data object was created 
        /// without factory and this method makes no sense.
        /// </summary>
        /// <param name="pClassID">Pointer to the class identifier of the editor type.</param>
        /// <returns>
        /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
        /// </returns>
        int IVsPersistDocData.GetGuidEditorType(out Guid pClassID)
        {
            // Predefined GUID
            pClassID = GuidList.EditorFactoryCLSID;
            return VSConstants.S_OK;
        }

        /// <summary>
        /// This method should release all interfaces and incapacitate itself.
        /// </summary>
        /// <returns>
        /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
        /// </returns>
        int IVsPersistDocData.Close()
        {
            try
            {
                // Call to virtual method
                Close();
            }
            catch (Exception e)
            {
                Trace.TraceError("Error during releasing resources:\n{0}", e.ToString());
                // Return unspecified error
                return VSConstants.S_FALSE;
            }
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Determines whether the document data can be reloaded.
        /// In fileless documents this method doesn’t make much sense.
        /// </summary>
        /// <param name="pfReloadable">1 if the document data can be reloaded.</param>
        /// <returns>
        /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
        /// </returns>
        int IVsPersistDocData.IsDocDataReloadable(out int pfReloadable)
        {
            try
            {
                // Call to virtual property IsReloadable
                pfReloadable = IsReloadable ? 1 : 0;
            }
            catch (Exception e)
            {
                Trace.TraceError("Error during checking reload ability:\n{0}", e.ToString());
                // Return unspecified error
                pfReloadable = 0;
                return VSConstants.S_FALSE;
            }
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Called by the Running Document Table (RDT) when it registers the 
        /// document data in the RDT. 
        /// </summary>
        /// <param name="docCookie">Abstract handle for the document to be registered.</param>
        /// <param name="pHierNew">Pointer to the IVsHierarchy interface.</param>
        /// <param name="itemidNew">Item identifier of the document to be registered from VSITEM.</param>
        /// <returns>
        /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
        /// </returns>
        int IVsPersistDocData.OnRegisterDocData(
            uint docCookie, 
            IVsHierarchy pHierNew, 
            uint itemidNew)
        {
            // Stores data into local variables
            documentCookieVal = docCookie;
            Debug.Assert(hierarchyRef.Hierarchy == pHierNew, "Registration in wrong hierarchy!");
            hierarchyItemIDVal = (int)itemidNew;
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Sets the initial name (or path) for unsaved, newly created document data.
        /// </summary>
        /// <param name="pszDocDataPath">
        /// String indicating the path of the document. Most editors can ignore this parameter. 
        /// It exists for historical reasons.
        /// </param>
        /// <returns>
        /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
        /// </returns>
        int IVsPersistDocData.SetUntitledDocPath(string pszDocDataPath)
        {
            // Do nothing at this point.
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Renames the document data. We are not going to allow renames in such way. 
        /// </summary>
        /// <param name="grfAttribs">File attribute of the document data to be renamed.</param>
        /// <param name="pHierNew">Pointer to the IVsHierarchy interface of the document being renamed.</param>
        /// <param name="itemidNew">Item identifier of the document being renamed.</param>
        /// <param name="pszMkDocumentNew">Path to the document being renamed.</param>
        /// <returns>
        /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
        /// </returns>
        int IVsPersistDocData.RenameDocData(
            uint grfAttribs,
            IVsHierarchy pHierNew,
            uint itemidNew,
            string pszMkDocumentNew)
        {
            // This method is called then right hierarchy item fouded 
            // (for new saved objects after double-click in the server explorer)

            // pszMkDocument must be the same as OldMoniker or Moniker
            if (!DataInterpreter.CompareInvariant(pszMkDocumentNew, OldMoniker) &&
                !DataInterpreter.CompareInvariant(pszMkDocumentNew, Moniker))
            {
                Debug.Fail("Trying to rename to the different moniker!");
                return VSConstants.E_INVALIDARG;
            }
            Debug.Assert(hierarchyRef.Hierarchy == pHierNew, "Renaming in wrong hierarchy!");
            hierarchyItemIDVal = (int)itemidNew;
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Reloads the document data and in the process determines whether to 
        /// ignore a subsequent data change.
        /// </summary>
        /// <param name="grfFlags">
        /// Flag indicating whether to ignore the next data change when reloading the document data.
        /// At this time flag is ignored.
        /// </param>
        /// <returns>
        /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
        /// </returns>
        int IVsPersistDocData.ReloadDocData(uint grfFlags)
        {
            try
            {
                // Initialy load data
                if (!LoadData(true))
                {
                    // Close document and exit
                    UrgentlyCloseDocument(null);
                    return VSConstants.S_FALSE;
                }

                // Fire DataLoaded
                FireDataLoaded();
            }
            catch (Exception e)
            {
                Trace.TraceError("Error during loading object:\n{0}", e.ToString());
                // Close document and exit
                UrgentlyCloseDocument(e);
                // Return unspecified error
                return VSConstants.S_FALSE;
            }
            return VSConstants.S_OK;
        }
        #endregion

        #region Document attribute row and accessors
        /// <summary>
        /// Attributes row for this object. Describes all object properties.
        /// </summary>
        [Browsable(false)]
        protected DataRow Attributes
        {
            get
            {
                lock (this)
                {
                    return attributes;
                }
            }
        }

        /// <summary>
        /// Determines if attributes for the object are loaded.
        /// </summary>
        [Browsable(false)]
        protected bool IsAttributesLoaded
        {
            get
            {
                return Attributes != null;
            }
        }

        /// <summary>
        /// Checks, if value of given attribute is changed.
        /// </summary>
        /// <param name="attributeName">Attribute name.</param>
        /// <returns>Returns true if value of given attribute was changed and false otherwise.</returns>
        protected bool IsAttributeChanged(string attributeName)
        {
            // Check input
            if (attributeName == null)
                throw new ArgumentNullException("attributeName");

            // Check attribute collection
            Debug.Assert(Attributes != null, "Atributes are not read!");

            return DataInterpreter.HasChanged(Attributes, attributeName);            
        }

        /// <summary>
        /// Returns string value for given attribute.
        /// </summary>
        /// <param name="attributeName">Attribute name.</param>
        /// <returns>Returns string value for given attribute.</returns>
        protected string GetAttributeAsString(string attributeName)
        {
            // Check input
            if (attributeName == null)
                throw new ArgumentNullException("attributeName");

            // Check attribute collection
            Debug.Assert(Attributes != null, "Atributes are not read!");

            return DataInterpreter.GetString(Attributes, attributeName);
        }

        /// <summary>
        /// Returns string value for given attribute. Changes null's to empty strings.
        /// </summary>
        /// <param name="attributeName">Attribute name.</param>
        /// <returns>Returns string value for given attribute.</returns>
        protected string GetAttributeAsStringNotNull(string attributeName)
        {
            // Check input
            if (attributeName == null)
                throw new ArgumentNullException("attributeName");

            // Check attribute collection
            Debug.Assert(Attributes != null, "Atributes are not read!");

            return DataInterpreter.GetStringNotNull(Attributes, attributeName);
        }

        /// <summary>
        /// Returns integer value for given attribute.
        /// </summary>
        /// <param name="attributeName">Attribute name.</param>
        /// <returns>Returns integer value for given attribute.</returns>
        protected Nullable<Int64> GetAttributeAsInt(string attributeName)
        {
            // Check input
            if (attributeName == null)
                throw new ArgumentNullException("attributeName");

            // Check attribute collection
            Debug.Assert(Attributes != null, "Atributes are not read!");

            return DataInterpreter.GetInt(Attributes, attributeName);
        }

        /// <summary>
        /// Returns SqlBoolean value for given attribute.
        /// </summary>
        /// <param name="attributeName">Attribute name.</param>
        /// <returns>Returns SqlBoolean value for given attribute.</returns>
        protected SqlBoolean GetAttributeAsSqlBool(string attributeName)
        {
            // Check input
            if (attributeName == null)
                throw new ArgumentNullException("attributeName");
            
            // Check attribute collection
            Debug.Assert(Attributes != null, "Atributes are not read!");

            return DataInterpreter.GetSqlBool(Attributes, attributeName);
        }

        /// <summary>
        /// Returns value of a given attribute as value of a given enumeration
        /// </summary>
        /// <param name="attrName">Attribute name</param>
        /// <param name="defaultValue">Default value for the attribute</param>
        /// <returns>Value of a given attribute as enumeration value</returns>
        protected object GetAttributeAsEnum(string attrName, object defaultValue)
        {
            string strAttrValue = GetAttributeAsString(attrName);
            return ConvertToEnum(strAttrValue, defaultValue, false);
        }

        /// <summary>
        /// Returns value of a given attribute as value of a given enumeration, when 
        /// string representation of the value can contain spaces
        /// </summary>
        /// <param name="attrName">Attribute name</param>
        /// <param name="defaultValue">Default value for the attribute</param>
        /// <returns>Value of a given attribute as enumeration value</returns>
        protected object GetAttributeAsSpacedEnum(string attrName, object defaultValue)
        {
            string strAttrValue = GetAttributeAsString(attrName);
            return ConvertToEnum(strAttrValue, defaultValue, true);
        }

        /// <summary>
        /// Returns string value for given attribute, using original data row version.
        /// </summary>
        /// <param name="attributeName">Attribute name.</param>
        /// <returns>Returns string value for given attribute, using original data row version.</returns>
        protected string GetOldAttributeAsString(string attributeName)
        {
            // Check input
            if (attributeName == null)
                throw new ArgumentNullException("attributeName");

            // Check attribute collection
            Debug.Assert(Attributes != null, "Atributes are not read!");

            return DataInterpreter.GetString(Attributes, attributeName, DataRowVersion.Original);
        }

        /// <summary>
        /// Returns integer value for given attribute, using original data row version.
        /// </summary>
        /// <param name="attributeName">Attribute name.</param>
        /// <returns>Returns integer value for given attribute, using original data row version.</returns>
        protected Nullable<Int64> GetOldAttributeAsInt(string attributeName)
        {
            // Check input
            if (attributeName == null)
                throw new ArgumentNullException("attributeName");

            // Check attribute collection
            Debug.Assert(Attributes != null, "Atributes are not read!");

            return DataInterpreter.GetInt(Attributes, attributeName, DataRowVersion.Original);
        }

        /// <summary>
        /// Returns SqlBoolean value for given attribute, using original data row version.
        /// </summary>
        /// <param name="attributeName">Attribute name.</param>
        /// <returns>Returns SqlBoolean value for given attribute, using original data row version.</returns>
        protected SqlBoolean GetOldAttributeAsSqlBool(string attributeName)
        {
            // Check input
            if (attributeName == null)
                throw new ArgumentNullException("attributeName");

            // Check attribute collection
            Debug.Assert(Attributes != null, "Atributes are not read!");

            return DataInterpreter.GetSqlBool(Attributes, attributeName, DataRowVersion.Original);
        }

        /// <summary>
        /// Set new value for given attribute.
        /// </summary>
        /// <param name="attributeName">Attribute name.</param>
        /// <param name="value">New value.</param>
        protected void SetAttribute(string attributeName, object value)
        {
            // Check input
            if (attributeName == null)
                throw new ArgumentNullException("attributeName");

            // Check attribute collection
            Debug.Assert(Attributes != null, "Atributes are not read!");

            object dbValue = value != null ? value : DBNull.Value;

            // Change value if not equals to current
            if (!DataInterpreter.CompareObjects(dbValue, Attributes[attributeName]))
                Attributes[attributeName] = dbValue;
        }

        /// <summary>
        /// Set a new value for a given enumeration attribute, if the latter can have 
        /// spaces
        /// </summary>
        /// <param name="attributeName">The attribute's name</param>
        /// <param name="value">The new value</param>
        protected void SetAttributeAsSpacedEnum(string attributeName, object value)
        {
            // Casting an enumeration value to a usual string with spaces
            string strValue =
                (value != null) ? value.ToString().Replace(Underscore, Space) : null;
            SetAttribute(attributeName, strValue);
        }

        /// <summary>
        /// Allows inheritors to change attributes row.
        /// </summary>
        /// <param name="attributesRow">New data row with attributes.</param>
        protected void ChangeAttributesRow(DataRow attributesRow)
        {
            if (attributesRow == null)
                throw new ArgumentNullException("attributesRow");

            // Validate datatable first
            if (attributesRow.Table == null)
            {
                Debug.Fail("Attributes table is is missing!");
                return;
            }

            // Store attributes row
            lock (this)
            {
                // Reseting attributes
                ResetAttributes();
                attributes = attributesRow;
            }

            // Connect to table notifications 
            attributesRow.Table.RowChanged += new DataRowChangeEventHandler(OnAttributesRowChanged);
        }
        #endregion

        #region Protected properties
        /// <summary>
        /// Abstract handle for the document to be registered in RDT.
        /// </summary>
        [Browsable(false)]
        protected uint DocumentCookie
        {
            get
            {
                return documentCookieVal;
            }
        }

        protected int HierarchyItemIDVal
        {
            get { return hierarchyItemIDVal; }
        }

        /// <summary>
        /// Item identifier of the document in the hierarchy.
        /// </summary>
        [Browsable(false)]
        protected int HierarchyItemID
        {
            get
            {
                // Check current hierarchy item id
                string name = String.Empty;
                try
                {
                    // If name can be retrived, it is ok
                    if (hierarchyItemIDVal != int.MinValue)
                        name = Hierarchy.GetName(hierarchyItemIDVal);

                }
                catch
                {
                    name = string.Empty;
                }

                // Compare extracted name with our old name. Type is not checked, because node can be contextless
                // and has no type.
                if (DataInterpreter.CompareInvariant(name, OldName))
                    return hierarchyItemIDVal;

                // If item not found and names are different, create new item and set its name
                hierarchyItemIDVal = Hierarchy.CreateObjectNode();
                Hierarchy.SetName(hierarchyItemIDVal, OldName);

                // Rename document to the new hierarchy item in the RDT
                Hierarchy.Rename(OldMoniker, OldMoniker, hierarchyItemIDVal);

                // If search failed, returns stored item
                return hierarchyItemIDVal;
            }
        }

        /// <summary>
        /// Returns moniker string, used to register this object in RDT.
        /// </summary>
        [Browsable(false)]
        protected String Moniker
        {
            get
            {
                return Hierarchy.BuildMoniker(TypeName, ObjectID);
            }
        }
        /// <summary>
        /// Returns old moniker string (moniker befre changes are applyed), 
        /// used to register this object in RDT.
        /// </summary>
        [Browsable(false)]
        protected String OldMoniker
        {
            get
            {
                return Hierarchy.BuildMoniker(TypeName, OldObjectID);
            }
        }

        /// <summary>
        /// Returns descriptor for this object type
        /// </summary>
        [Browsable(false)]
        protected IObjectDescriptor Descriptor
        {
            get
            {
                if (descriptorRef == null)
                {
                    descriptorRef = ObjectDescriptorFactory.Instance.CreateDescriptor(TypeName);
                    if (descriptorRef == null)
                        throw new NotSupportedException(String.Format(
                            CultureInfo.CurrentCulture,
                            Resources.Error_UnableToGetDescriptor,
                            TypeName));
                }
                return descriptorRef;
            }
        }

        /// <summary>
        /// Returns object identifier which should be used for data loading. This identifier 
        /// is current object identifier for all objects except cloned. For cloned object it 
        /// is the identifier of the source object.
        /// </summary>
        [Browsable(false)]
        public virtual object[] ObjectIDForLoad
        {
            get
            {
                // Extract current id
                object[] id = ObjectID;

                // If object is clonned, replace name in the identifier by the original name
                if (clonnedName != null)
                    id[id.Length - 1] = nameVal;

                // Return result
                return id;
            }
        }
        #endregion

        #region Protected methods
        /// <summary>
        /// Fires DataLoaded event
        /// </summary>
        protected void FireDataLoaded()
        {
            if (DataLoaded != null)
                DataLoaded(this, EventArgs.Empty);
        }
        
        /// <summary>
        /// Fires DataChanged event
        /// </summary>
        protected void FireDataChanged()
        {
            if (DataChanged != null)
                DataChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Fires Saving event.
        /// </summary>
        /// <returns>Returns false if any listener cancels saving.</returns>
        protected bool FireSaving()
        {
            if (Saving != null)
            {
                CancelEventArgs e = new CancelEventArgs(false);
                Saving(this, e);
                return !e.Cancel;
            }
            return true;
        }

        /// <summary>
        /// Fires SuccessfullySaved event
        /// </summary>
        protected void FireSuccessfullySaved()
        {
            if (SuccessfullySaved != null)
                SuccessfullySaved(this, EventArgs.Empty);
        }

        /// <summary>
        /// Fires SaveAttemptFailed event
        /// </summary>
        protected void FireSaveAttemptFailed()
        {
            if (SaveAttemptFailed != null)
                SaveAttemptFailed(this, EventArgs.Empty);
        }
        /// <summary>
        /// Urgently close document is something wrong.
        /// </summary>
        protected void UrgentlyCloseDocument(Exception e)
        {
            // Close document and exit
            Hierarchy.CloseDocument(TypeName, ObjectID);
            
            // If exception is given, alert user
            if (e != null)
                UIHelper.ShowError(e);
            
            // Alert user that document windows is closed
            UIHelper.ShowError(Resources.Error_ReloadFailed);
        }


        /// <summary>
        /// Resets hierarchy item identifier to be recreated later
        /// </summary>
        protected void ResetHierarchyItem()
        {
            // Drop current hierarchy node
            Hierarchy.DropObjectNode(HierarchyItemID);

            // Reset hierarchy item identifier to be recreated later
            hierarchyItemIDVal = int.MinValue;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Resets attributes row before reload and disconnects events.
        /// </summary>
        private void ResetAttributes()
        {
            Debug.Assert(attributes != null, "Attributes are not loaded!");
            attributes.Table.RowChanged -= new DataRowChangeEventHandler(OnAttributesRowChanged);
            attributes.Table.Dispose();
            attributes = null;
        }

        /// <summary>
        /// Handles notification about attributes row changes.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Detailed information on the event.</param>
        private void OnAttributesRowChanged(object sender, DataRowChangeEventArgs e)
        {
            Debug.Assert(e != null && e.Row == Attributes, "Empty event argumets provided or wrong DataRow included!");
            HandleAttributesChanges();
            FireDataChanged();
        }

        /// <summary>
        /// Converts a string to an enumeration
        /// </summary>
        /// <param name="strValue">String representation of a value to convert</param>
        /// <param name="defaultValue">Default value for the enumeration</param>
        /// <param name="hasSpaces">Indicates if the string representation can have 
        /// spaces</param>
        /// <returns>The value converted to enumeration if possible</returns>
        private object ConvertToEnum(string strValue, object defaultValue, bool hasSpaces)
        {
            if (string.IsNullOrEmpty(strValue))
                return defaultValue;

            if (defaultValue == null)
                // The value is not typed; returning it as a string
                return strValue;

            if (hasSpaces)
                // Enumeration value can't have spaces
                strValue = strValue.Replace(Space, Underscore);

            // Casting a string representation of the value to the enumeration type
            Type enumType = defaultValue.GetType();
            try
            {
                return Enum.Parse(enumType, strValue, true);
            }
            catch (ArgumentException)
            {
                return defaultValue;
            }
        }
        #endregion

        #region Private variables to store properties
        private uint documentCookieVal;
        private int hierarchyItemIDVal;
        private ServerExplorerFacade hierarchyRef;
        private DataConnectionWrapper connectionRef;
        private string schemaVal;
        private string nameVal;
        private bool isNewVal;
        private DataRow attributes;
        private IObjectDescriptor descriptorRef;
        private string clonnedName = null;
        #endregion
    }
}
