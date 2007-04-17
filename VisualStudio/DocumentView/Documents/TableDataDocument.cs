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
 * This file contains implementation of the document for table and view data editing.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Data;
using MySql.Data.VisualStudio.Utils;
using MySql.Data.VisualStudio.Descriptors;
using System.ComponentModel;
using System.Globalization;
using MySql.Data.VisualStudio.Properties;

namespace MySql.Data.VisualStudio.DocumentView
{
    /// <summary>
    /// This class implements document (data object) for table and view data editing.
    /// </summary>
    [DocumentObject(TableDataDescriptor.TypeName, typeof(TableDataDocument))]
    public class TableDataDocument : BaseDocument
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
        public TableDataDocument(ServerExplorerFacade hierarchy, bool isNew, object[] id)
            :base(hierarchy, isNew, id)
        {
            Debug.Assert(!isNew, "Editing data in the new objects are not supported!");
        }
        #endregion        

        #region Database object properties
        /// <summary>
        /// Shows if the view is updatable
        /// </summary>
        [LocalizableCategory("Category_Advanced")]
        [LocalizableDescription("Description_View_IsUpdatable")]
        [LocalizableDisplayName("DisplayName_View_IsUpdatable")]
        [DefaultValue(false)]
        public bool IsUpdatable
        {
            get
            {
                return GetAttributeAsSqlBool(TableDataDescriptor.Attributes.IsUpdatable).IsTrue;
            }
        }

        /// <summary>
        /// Returns DataTable with object data
        /// </summary>
        [Browsable(false)]
        public DataTable Data
        {
            get
            {
                Debug.Assert(dataRef != null, "Data are not read!");
                return dataRef;
            }
        }

        /// <summary>
        /// Comments are not supported
        /// </summary>
        [Browsable(false)]
        public override string Comments
        {
            get
            {
                Debug.Fail("Comments are not supported!");
                return String.Empty;
            }
            set
            {
                Debug.Fail("Comments are not supported!");
            }
        }
        #endregion

        #region Overridings
        /// <summary>
        /// Returns true if underlying object is updateable data were changed.
        /// </summary>
        protected override bool IsDirty
        {
            get
            {
                return IsUpdatable && (Data != null ? DataInterpreter.HasChanged(Data) : false);
            }
        }

        /// <summary>
        /// Name of this database object.
        /// </summary>
        [LocalizableCategory("Category_Identifier")]
        [LocalizableDescription("Description_Object_Name")]
        [LocalizableDisplayName("DisplayName_Object_Name")]
        [ReadOnly(true)]
        public override string Name
        {
            get
            {
                return base.Name;
            }
            set
            {
                Debug.Fail("Set name is not supported!");
            }
        }

        /// <summary>
        /// Loads data from database object.
        /// </summary>
        /// <param name="reloading">
        /// This flag indicates that object is reloading. Should be ignored in most cases.
        /// </param>
        /// <returns>Returns true if load succeeds and false otherwise.</returns>
        protected override bool LoadData(bool reloading)
        {
            // Call base to load attributes including names and IS_UPDATABLE flag
            if (!base.LoadData(reloading))
                return false;

            // Disposes old table if any.
            if (dataRef != null)
                dataRef.Dispose();


            // Load data from object
            dataRef = Connection.ExecuteSelectTable(SelectQuery);

            return dataRef != null;
        }

        /// <summary>
        /// Saves changes into database.
        /// </summary>
        /// <param name="silent">
        /// If this flag set to true, save should be performed without any user interaction.
        /// </param>
        /// <returns>
        /// Returns true if save operation succeed. If user cancels operation or SQL query fails, 
        /// this method returns false.
        /// </returns>
        protected override bool SaveData(bool silent)
        {
            // Validate connection and data
            if (Connection == null || Data == null)
            {
                Debug.Fail("Not initialized in the save method!");
                return false;
            }

            // Update data using connection
            if( !Connection.UpdateTable(Data, SelectQuery))
                return false;


            // Accept all changes to object
            AcceptChanges();

            // Reload object data
            if (!ReloadData())
                return false;

            // Fire successfully saved for vies to update them
            FireSuccessfullySaved();

            // Finaly, return true
            return true;
        }

        /// <summary>
        /// Returns true if table is stil exists.
        /// </summary>
        /// <returns>Returns true if table is stil exists.</returns>
        protected override bool ValidateData()
        {
            if (!base.ValidateData())
                return false;

            // Check if table is still exists
            DataTable tables = TableDataDescriptor.Enumerate(Connection, ObjectID);
            if (tables == null || tables.Rows == null || tables.Rows.Count <= 0)
            {
                UIHelper.ShowError(String.Format(
                    CultureInfo.CurrentCulture,
                    Resources.Error_TableWasDeleted,
                    Name));
                return false;
            }
            return true;
        }
        #endregion

        #region Create and Alter generation stubs
        protected override string BuildCreateQuery()
        {
            return String.Empty;
        }

        protected override string BuildAlterQuery()
        {
            return String.Empty;
        } 
        #endregion

        #region Private properties
        /// <summary>
        /// Returns select query for the table or view.
        /// </summary>
        private string SelectQuery
        {
            get
            {
                StringBuilder query = new StringBuilder();
                query.Append("SELECT * FROM ");
                QueryBuilder.WriteIdentifier(Schema, Name, query);
                return query.ToString();
            }
        }
        #endregion

        #region Private variables to store properties
        DataTable dataRef;
        #endregion
    }
}
