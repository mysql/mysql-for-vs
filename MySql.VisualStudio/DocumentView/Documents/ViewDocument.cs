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
using System.ComponentModel;
using System.Data;
using System.Text;

using MySql.Data.VisualStudio.Descriptors;
using View = MySql.Data.VisualStudio.Descriptors.ViewDescriptor.Attributes;
using MySql.Data.VisualStudio.Properties;
using MySql.Data.VisualStudio.Utils;
using System.Globalization;

namespace MySql.Data.VisualStudio.DocumentView
{
    /// <summary>
    /// Implements a document functionality and represent a database view
    /// </summary>
    [DocumentObject(ViewDescriptor.TypeName, typeof(ViewDocument))]
    public class ViewDocument : BaseDocument, ISqlSource
    {
        #region Enumerations
        /// <summary>
        /// Types of processing a view in SQL statements
        /// </summary>
        public enum Algorithms
        {
            UNDEFINED,
            MERGE,
            TEMPTABLE
        }

        /// <summary>
        /// Checking options
        /// </summary>
        public enum CheckOptions
        {
            NONE,
            CASCADED,
            LOCAL
        }
        #endregion

        #region SQL definition of the view
        /// <summary>
        /// SQL definition of the view
        /// </summary>
        [Browsable(false)]
        public string SqlSource
        {
            get
            {
                return Definition;
            }

            set
            {
                Definition = value;
            }
        }
        #endregion

        #region Checking properties
        /// <summary>
        /// Checks if any option besides the view's name is changed
        /// </summary>
        private bool IsAltered
        {
            get
            {
                if (Attributes == null)
                    return false;

                foreach (DataColumn column in Attributes.Table.Columns)
                {
                    if (column.Caption == View.Name)
                        continue;

                    if (IsAttributeChanged(column.ColumnName))
                        return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Checks if a name of the view is changed
        /// </summary>
        private bool IsRenamed
        {
            get
            {
                return IsAttributeChanged(View.Name);
            }
        }
        #endregion

        #region Displayable properties
        /// <summary>
        /// Indicates how to treat the view in SQL statements
        /// </summary>
        [LocalizableCategory("Category_Advanced")]
        [LocalizableDescription("Description_View_Algorithm")]
        [LocalizableDisplayName("DisplayName_View_Algorithm")]
        [DefaultValue(Algorithms.UNDEFINED)]
        public Algorithms Algorithm
        {
            get
            {
               return (Algorithms)GetAttributeAsEnum(View.Algorithm, Algorithms.UNDEFINED);
            }

            set
            {
                SetAttribute(View.Algorithm, value);
            }
        }

        /// <summary>
        /// Checking options for inserts and updates of the view
        /// </summary>
        [LocalizableCategory("Category_Advanced")]
        [LocalizableDescription("Description_View_CheckOption")]
        [LocalizableDisplayName("DisplayName_View_CheckOption")]
        [DefaultValue(CheckOptions.NONE)]
        public CheckOptions CheckOption
        {
            get
            {
                return (CheckOptions)GetAttributeAsEnum(View.CheckOption, CheckOptions.NONE);
            }

            set
            {
                SetAttribute(View.CheckOption, value);
            }
        }

        /// <summary>
        /// Disables view comments in a properties window
        /// </summary>
        [Browsable(false)]
        public override string Comments
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        /// <summary>
        /// Definer of the view
        /// </summary>
        [LocalizableCategory("Category_Advanced")]
        [LocalizableDescription("Description_Object_Definer")]
        [LocalizableDisplayName("DisplayName_Object_Definer")]
        [DefaultValue(null)]
        public string Definer
        {
            get
            {
                return GetAttributeAsString(View.Definer);
            }

            set
            {
                SetAttribute(View.Definer, value);
            }
        }

        /// <summary>
        /// SQL definition of the view
        /// </summary>
        [LocalizableCategory("Category_Base")]
        [LocalizableDescription("Description_View_Definition")]
        [LocalizableDisplayName("DisplayName_View_Definition")]
        [DefaultValue(null)]
        [Browsable(false)]
        public string Definition
        {
            get
            {
                return GetAttributeAsString(View.Definition);
            }

            set
            {
                SetAttribute(View.Definition, value);
            }
        }

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
                string strUpdatable = GetAttributeAsString(View.IsUpdatable);
                return DataInterpreter.CompareInvariant(strUpdatable, "YES");
            }
        }

        /// <summary>
        /// Type of security for actions on the view
        /// </summary>
        [LocalizableCategory("Category_Security")]
        [LocalizableDescription("Description_Object_SecurityType")]
        [LocalizableDisplayName("DisplayName_Object_SecurityType")]
        [DefaultValue(SecurityTypes.DEFINER)]
        public SecurityTypes SecurityType
        {
            get
            {
                return (SecurityTypes)GetAttributeAsEnum(View.SecurityType, SecurityTypes.DEFINER);
            }

            set
            {
                SetAttribute(View.SecurityType, value);
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes private variables
        /// </summary>
        /// <param name="hierarchy">A data view hierarchy accessor used to interact 
        /// with Server Explorer. Also used to extract connection</param>
        /// <param name="isNew">Indicates if the instance of the class represents a 
        /// new database object which hasn't yet been stored in a database</param>
        /// <param name="id">An array containing the object's identifier</param>
        public ViewDocument(ServerExplorerFacade hierarchy, bool isNew, object[] id)
            : base(hierarchy, isNew, id)
        {
        }
        #endregion

        #region Validation
        /// <summary>
        /// Returns true if view data is consistent and ready for saving.
        /// </summary>
        /// <returns>Returns true if view data is consistent and ready for saving.</returns>
        protected override bool ValidateData()
        {
            // Ask base class
            if( !base.ValidateData())
                return false;

            // Check view definition for emptiness.
            if (string.IsNullOrEmpty(Definition))
            {
                UIHelper.ShowError(Resources.Error_ViewDefinitionIsEmpty);
                return false;
            }

            // Return true if everything is OK
            return true;
        }

        /// <summary>
        /// Check if view was renamed by the first part of the alter query. If so, change attributes row
        /// to reflect this fact.
        /// </summary>
        protected override void SaveFailed()
        {
            // If view is new, do nothing
            if (IsNew)
            {
                base.SaveFailed();
                return;
            }

            // Enumerate old view
            DataTable oldView = ViewDescriptor.Enumerate(Connection, OldObjectID);
            // Enumerate new view
            DataTable newView = ViewDescriptor.Enumerate(Connection, ObjectID);

            // Check if old view still exists
            if (oldView == null || oldView.Rows == null || oldView.Rows.Count <= 0)
            {
                // Check if new view is exists
                if (newView == null || newView.Rows == null || newView.Rows.Count <= 0)
                {
                    // New view doesn't exist, view will be considered as pre-dropped
                    CloneToName(Name);
                    ResetToNew();
                    UIHelper.ShowWarning(String.Format(
                        CultureInfo.CurrentCulture,
                        Resources.Warning_ObjectPreDropped,
                        Name));
                }
                else
                {
                    // New view is exisits - view will be considered as renamed
                    // Save all view attributtes
                    object[] currentAttributtes = Attributes.ItemArray;

                    // Reset hierarchy item to be recreated later
                    ResetHierarchyItem();

                    // Rename document
                    Hierarchy.Rename(OldMoniker, Moniker, HierarchyItemID);

                    // Set new name for the hierarchy item
                    Hierarchy.SetName(HierarchyItemID, Name);

                    // Refresh hierarchy to reflect view renaming
                    Hierarchy.Refresh();

                    // Notify user about rename of the view
                    UIHelper.ShowWarning(String.Format(
                        CultureInfo.CurrentCulture,
                        Resources.Warning_ViewWasRenamed,
                        OldName,
                        Name));

                    // Change attributes row for renamed view
                    ChangeAttributesRow(newView.Rows[0]);

                    // Copy view options to the new attributes row
                    for (int i = 0; i < currentAttributtes.Length; i++)
                        DataInterpreter.SetValueIfChanged(Attributes, newView.Columns[i].ColumnName, currentAttributtes[i]);
                }
            }

            // Call to base before exit
            base.SaveFailed();
        }
        #endregion

        #region Building of queries
        /// <summary>
        /// Creates a query on a creation of a view
        /// </summary>
        /// <returns>A string representation of the query</returns>
        protected override string BuildCreateQuery()
        {
            StringBuilder sb = new StringBuilder();
            CreateQuery(sb, "CREATE");

            return sb.ToString();
        }

        /// <summary>
        /// If view was both altered and renamed returns rename statement as the pre-dropoperation.
        /// </summary>
        /// <returns>If view was both altered and renamed returns rename statement as the pre-dropoperation.</returns>
        protected override string BuildPreDropQuery()
        {
            // If view was renamed and alterred in the same query, we need to rename it in the pre-drop operation
            // to avoid syntax errors because of delimeterts.
            if( IsRenamed && IsAltered )
                return CreateRename();
            
            return String.Empty;
        }
        /// <summary>
        /// Creates a query on a alteration of a view
        /// </summary>
        /// <returns>A string representation of the query</returns>
        protected override string BuildAlterQuery()
        {
            // If view was only renamed, return rename query as the main part of saving.
            if (IsRenamed && !IsAltered)
                return CreateRename();

            StringBuilder sb = new StringBuilder();
            if (IsAltered)
                CreateQuery(sb, "ALTER");

            return sb.ToString();
        }

        /// <summary>
        /// Creates a query for creation or alteration of a view
        /// </summary>
        /// <param name="sb">StringBuilder to write the query in</param>
        /// <param name="actionName">The name of the requested action</param>
        private void CreateQuery(StringBuilder sb, string actionName)
        {
            // Adding the name of the action
            sb.Append(actionName);

            // Adding an algorithm
            if (Algorithm != Algorithms.UNDEFINED)
            {
                sb.Append(" ALGORITHM = ");
                sb.Append(Algorithm.ToString());
            }

            // Adding definer if not current user
            string definer = DataInterpreter.GetStringNotNull(Attributes, View.Definer);
            if (!DataInterpreter.CompareInvariant(definer, CurretnUser))
                QueryBuilder.WriteUserNameIfNotEmpty(definer, " DEFINER = ", sb);
            

            // Adding security type, if are not empty
            QueryBuilder.WriteIfNotEmptyString(Attributes, View.SecurityType, " SQL SECURITY ", sb, false);

            // Adding definition of a view
            sb.Append(" VIEW ");
            QueryBuilder.WriteIdentifier(Attributes, View.Name, sb);
            sb.Append(" AS ");
            sb.AppendLine();
            QueryBuilder.WriteValue(Attributes, View.Definition, sb, false);

            // Adding check options
            if (CheckOption != CheckOptions.NONE)
            {
                sb.AppendLine();
                sb.Append(" WITH ");
                sb.Append(CheckOption.ToString());
                sb.Append(" CHECK OPTION");
            }
        }

        /// <summary>
        /// Creates a rename query
        /// </summary>
        /// <param name="sb">StringBuilder to write the query in</param>
        private string CreateRename()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("RENAME TABLE ");
            QueryBuilder.WriteIdentifier(GetOldAttributeAsString(View.Name), sb);
            sb.Append(" TO ");
            QueryBuilder.WriteIdentifier(Name, sb);
            return sb.ToString();
        }
        #endregion

        #region Constants
        /// <summary>Current user function name.</summary>
        public const string CurretnUser = "CURRENT_USER";
        #endregion
    }
}