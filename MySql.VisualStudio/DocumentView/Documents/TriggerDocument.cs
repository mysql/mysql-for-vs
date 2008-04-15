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

using System.ComponentModel;
using System.Data;
using System.Text;

using MySql.Data.VisualStudio.Descriptors;
using Trigger = MySql.Data.VisualStudio.Descriptors.TriggerDescriptor.Attributes;
using MySql.Data.VisualStudio.Utils;
using System;
using MySql.Data.VisualStudio.Properties;
using System.Globalization;

namespace MySql.Data.VisualStudio.DocumentView
{
    /// <summary>
    /// Implements a document functionality and represent a database trigger
    /// </summary>
    [DocumentObject(TriggerDescriptor.TypeName, typeof(TriggerDocument))]
    public class TriggerDocument : BaseDocument, ISqlSource
    {
        #region Private variable
        /// <summary>Event table of the trigger</summary>
        private string tableVal;
        #endregion

        #region Enumerations
        /// <summary>
        /// The trigger action time
        /// </summary>
        public enum ActionTime
        {
            AFTER,
            BEFORE
        }

        /// <summary>
        /// Kinds of statements which activate a trigger
        /// </summary>
        public enum TriggerEvents
        {
            INSERT,
            UPDATE,
            DELETE
        }
        #endregion

        #region Identifying properties
        /// <summary>
        /// Event table of the trigger
        /// </summary>
        [Browsable(false)]
        public string EventTable
        {
            get
            {
                if (!IsAttributesLoaded)
                    return tableVal;

                return GetAttributeAsString(Trigger.EventTable);
            }

            set
            {
                SetAttribute(Trigger.EventTable, value);
            }
        }

        /// <summary>
        /// ID of the trigger
        /// </summary>
        [Browsable(false)]
        public override object[] ObjectID
        {
            get
            {
                return new object[] { null, Schema, EventTable, Name };
            }
        }

        /// <summary>
        /// The old ID of the trigger (before changes)
        /// </summary>
        [Browsable(false)]
        public override object[] OldObjectID
        {
            get
            {
                return new object[] { null, Schema, EventTable, OldName };
            }
        }

        /// <summary>
        /// SQL definition of the procedure
        /// </summary>
        [Browsable(false)]
        public string SqlSource
        {
            get
            {
                return GetAttributeAsString(Trigger.Statement);
            }

            set
            {
                SetAttribute(Trigger.Statement, value);
            }
        }
        #endregion

        #region Displayed properties
        /// <summary>
        /// Triggers have no comments
        /// </summary>
        [Browsable(false)]
        public override string Comments
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Definer of the trigger
        /// </summary>
        [LocalizableCategory("Category_Advanced")]
        [LocalizableDescription("Description_Object_Definer")]
        [LocalizableDisplayName("DisplayName_Object_Definer")]
        [DefaultValue(null)]
        public string Definer
        {
            get
            {
                return GetAttributeAsString(Trigger.Definer);
            }

            set
            {
                SetAttribute(Trigger.Definer, value);
            }
        }

        /// <summary>
        /// An event which activates the trigger
        /// </summary>
        [LocalizableCategory("Category_Base")]
        [LocalizableDescription("Description_Trigger_EventManipulation")]
        [LocalizableDisplayName("DisplayName_Trigger_EventManipulation")]
        [DefaultValue(TriggerEvents.INSERT)]
        public TriggerEvents Event
        {
            get
            {
                return (TriggerEvents)GetAttributeAsEnum(Trigger.EventManipulation, TriggerEvents.INSERT);
            }

            set
            {
                SetAttribute(Trigger.EventManipulation, value.ToString());
            }
        }

        /// <summary>
        /// The trigger action time
        /// </summary>
        [LocalizableCategory("Category_Base")]
        [LocalizableDescription("Description_Trigger_Timing")]
        [LocalizableDisplayName("DisplayName_Trigger_Timing")]
        [DefaultValue(ActionTime.AFTER)]
        public ActionTime Execute
        {
            get
            {
                return (ActionTime)GetAttributeAsEnum(Trigger.Timing, ActionTime.AFTER);
            }

            set
            {
                SetAttribute(Trigger.Timing, value.ToString());
            }
        }

        /// <summary>
        /// An event which activates the trigger
        /// </summary>
        [LocalizableCategory("Category_Identifier")]
        [LocalizableDescription("Description_Trigger_EventTable")]
        [LocalizableDisplayName("DisplayName_Trigger_EventTable")]
        [DefaultValue(null)]
        public string Table
        {
            get
            {
                return GetAttributeAsString(Trigger.EventTable);
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
        public TriggerDocument(ServerExplorerFacade hierarchy, bool isNew, object[] id)
            : base(hierarchy, isNew, id)
        {
            // Type is the 3rd part of an identifier
            tableVal = id[2] as string;
        }
        #endregion

        #region Initialization of a new trigger
        /// <summary>
        /// Initializes attributes for a new trigger
        /// </summary>
        /// <param name="newRow">A data row to write a new object's attributes in</param>
        protected override void FillNewObjectAttributes(DataRow newRow)
        {
            base.FillNewObjectAttributes(newRow);

            newRow[Trigger.EventManipulation] = TriggerEvents.INSERT;
            newRow[Trigger.Timing] = ActionTime.AFTER;
            newRow[Trigger.EventTable] = EventTable;
        }
        #endregion

        #region Building of queries
        /// <summary>
        /// Creates a query on a creation of a trigger
        /// </summary>
        /// <returns>A string representation of the query</returns>
        protected override string BuildCreateQuery()
        {
            return CreateQuery();
        }

        /// <summary>
        /// Returns query needed to pre-drop exists trigger.
        /// </summary>
        /// <returns>Returns query needed to pre-drop exists trigger.</returns>
        protected override string BuildPreDropQuery()
        {
            // Dropping exists trigger
            return "DROP TRIGGER " + OldName;
        }

        /// <summary>
        /// Creates a query on a alteration of a trigger
        /// </summary>
        /// <returns>A string representation of the query</returns>
        protected override string BuildAlterQuery()
        {
            // Creating a new trigger
            return CreateQuery();
        }

        /// <summary>
        /// Builds the "CREATE" query
        /// </summary>
        /// <returns></returns>
        private string CreateQuery()
        {
            StringBuilder sb = new StringBuilder();

            // Creating the action
            sb.Append("CREATE");

            // Adding a definer if not empty
            QueryBuilder.WriteUserNameIfNotEmpty(Definer, " DEFINER = ", sb);            

            // Adding main attributes
            sb.Append(" TRIGGER ");
            QueryBuilder.WriteIdentifier(Attributes, Trigger.Name, sb);
            sb.Append(" ");
            QueryBuilder.WriteValue(Attributes, Trigger.Timing, sb, false);
            sb.Append(" ");
            QueryBuilder.WriteValue(Attributes, Trigger.EventManipulation, sb, false);

            // Adding a statement
            sb.Append(" ON ");
            QueryBuilder.WriteIdentifier(Attributes, Trigger.EventTable, sb);
            sb.AppendLine();
            sb.AppendLine("FOR EACH ROW");
            QueryBuilder.WriteValue(Attributes, Trigger.Statement, sb, false);

            return sb.ToString();
        }
        #endregion

        #region Validation
        /// <summary>
        /// Validates trigger body and options.
        /// </summary>
        /// <returns>
        /// Returns true if trigger is consistent and can be saved. 
        /// Returns false otherwize.
        /// </returns>
        protected override bool ValidateData()
        {
            // Fires saving event by calling base class
            if (!base.ValidateData())
                return false;

            // Trigger body should not be empty
            if (String.IsNullOrEmpty(SqlSource))
            {
                UIHelper.ShowError(String.Format(
                    Resources.Error_EmptyTrigger,
                    Name));
                return false;
            }

            // Finaly, returns true
            return true;
        }
        #endregion
    }
}