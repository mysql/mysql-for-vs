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
using System.Data;
using System.Globalization;
using MySql.Data.VisualStudio.Properties;
using System.Text;
using MySql.Data.VisualStudio.Utils;

namespace MySql.Data.VisualStudio.Descriptors
{
    /// <summary>
    /// A descriptor of database triggers
    /// </summary>
    [ObjectDescriptor(TriggerDescriptor.TypeName, typeof(TriggerDescriptor))]
    [IdLength(4)]
    public class TriggerDescriptor : ObjectDescriptor
    {
        #region Type name
        /// <summary>Type name of the descriptor</summary>
        public new const string TypeName = "Trigger";
        #endregion

        #region Constants for the SQL request
        /// <summary>Template of the request itself</summary>
        protected new const string EnumerateSqlTemplate =
            "SELECT " +
            "TRIGGER_CATALOG, " +
            "TRIGGER_SCHEMA, " +
            "TRIGGER_NAME, " +
            "EVENT_MANIPULATION, " +
            "EVENT_OBJECT_CATALOG, " +
            "EVENT_OBJECT_SCHEMA, " +
            "EVENT_OBJECT_TABLE, " +
            "ACTION_ORDER, " +
            "ACTION_CONDITION, " +
            "ACTION_STATEMENT, " +
            "ACTION_ORIENTATION, " +
            "ACTION_TIMING, " +
            "ACTION_REFERENCE_OLD_TABLE, " +
            "ACTION_REFERENCE_NEW_TABLE, " +
            "ACTION_REFERENCE_OLD_ROW, " +
            "ACTION_REFERENCE_NEW_ROW, " +
            "CREATED, " +
            "SQL_MODE, " +
            "`DEFINER` " +
            "FROM information_schema.TRIGGERS " +
            "WHERE TRIGGER_SCHEMA = {1} AND EVENT_OBJECT_TABLE = {2} AND TRIGGER_NAME = {3}";

        /// <summary>Default restrictions</summary>
        protected new static readonly string[] DefaultRestrictions = 
		{
			"", "TRIGGER_SCHEMA", "EVENT_OBJECT_TABLE", "TRIGGER_NAME"
		};

        /// <summary>Default sort fields</summary>
        protected new const string DefaultSortString = "";
        #endregion

        #region Attributes
        /// <summary>
        /// Attributes of the View object
        /// </summary>
        public static new class Attributes
        {
            [Field(FieldType = TypeCode.String)]
            public const string Database = "TRIGGER_CATALOG";
            [Field(FieldType = TypeCode.String)]
            [Identifier(IsSchema = true)]
            public const string Schema = "TRIGGER_SCHEMA";
            [Field(FieldType = TypeCode.String)]
            [Identifier(IsName = true)]
            public const string Name = "TRIGGER_NAME";
            [Field(FieldType = TypeCode.String)]
            public const string EventManipulation = "EVENT_MANIPULATION";
            [Field(FieldType = TypeCode.String)]
            public const string EventCatalog = "EVENT_OBJECT_CATALOG";
            [Field(FieldType = TypeCode.String)]
            public const string EventSchema = "EVENT_OBJECT_SCHEMA";
            [Field(FieldType = TypeCode.String)]
            public const string EventTable = "EVENT_OBJECT_TABLE";
            [Field(FieldType = TypeCode.Int64)]
            public const string Order = "ACTION_ORDER";
            [Field(FieldType = TypeCode.String)]
            public const string Condition = "ACTION_CONDITION";
            [Field(FieldType = TypeCode.String)]
            public const string Statement = "ACTION_STATEMENT";
            [Field(FieldType = TypeCode.String)]
            public const string Orientation = "ACTION_ORIENTATION";
            [Field(FieldType = TypeCode.String)]
            public const string Timing = "ACTION_TIMING";
            [Field(FieldType = TypeCode.String)]
            public const string RefToOldTable = "ACTION_REFERENCE_OLD_TABLE";
            [Field(FieldType = TypeCode.String)]
            public const string RefToNewTable = "ACTION_REFERENCE_NEW_TABLE";
            [Field(FieldType = TypeCode.String)]
            public const string RefToOldRow = "ACTION_REFERENCE_OLD_ROW";
            [Field(FieldType = TypeCode.String)]
            public const string RefToNewRow = "ACTION_REFERENCE_NEW_ROW";
            [Field(FieldType = TypeCode.DateTime)]
            public const string CreationTime = "CREATED";
            [Field(FieldType = TypeCode.String)]
            public const string Mode = "SQL_MODE";
            [Field(FieldType = TypeCode.String)]
            public const string Definer = "DEFINER";
        }
        #endregion

        #region Virtual property
        /// <summary>
        /// The lowest version of the MySQL Server where the descriptor's object 
        /// appears
        /// </summary>
        public override Version RequiredVersion
        {
            get
            {
                return new Version(5, 0);
            }
        }
        #endregion

        #region Dropping
        /// <summary>
        /// Triggers can be dropped. Returns true.
        /// </summary>
        public override bool CanBeDropped
        {
            get { return true; }
        }

        /// <summary>
        /// Returns DROP TRIGGER statement.
        /// </summary>
        /// <param name="identifier">Database object identifier.</param>
        /// <returns>Returns DROP TRIGGER statement.</returns>
        public override string BuildDropSql(object[] identifier)
        {
            if (identifier == null)
                throw new ArgumentNullException("identifier");
            if (identifier.Length != 4 || String.IsNullOrEmpty(identifier[1] as string) || String.IsNullOrEmpty(identifier[3] as string))
                throw new ArgumentException(
                    String.Format(
                        CultureInfo.CurrentCulture,
                        Resources.Error_InvlaidIdentifier,
                        identifier.Length,
                        TypeName,
                        4),
                     "id");

            // Build query
            StringBuilder query = new StringBuilder("DROP TRIGGER ");
            QueryBuilder.WriteIdentifier(identifier[1] as string, identifier[3] as string, query);
            return query.ToString();
        }
        #endregion

        #region Enumerate method
        /// <summary>
        /// Enumerates triggers with given restrictions into a DataTable object
        /// </summary>
        /// <param name="connection">A DataConnectionWrapper to be used for enumeration</param>
        /// <param name="restrictions">Restrictions to be put on the retrieved objects' set</param>
        /// <returns>A data table containing all triggers which satisfy the given 
        /// restrictions</returns>
        public static DataTable Enumerate(DataConnectionWrapper connection, object[] restrictions)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            return ObjectDescriptor.EnumerateObjects(connection, TypeName, restrictions);
        }
        #endregion
    }
}