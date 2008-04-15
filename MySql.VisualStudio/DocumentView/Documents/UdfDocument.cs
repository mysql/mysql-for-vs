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
using System.Globalization;
using System.Text;

using MySql.Data.VisualStudio.Descriptors;
using Udf = MySql.Data.VisualStudio.Descriptors.UdfDescriptor.Attributes;
using MySql.Data.VisualStudio.Properties;
using MySql.Data.VisualStudio.Utils;

namespace MySql.Data.VisualStudio.DocumentView
{
    #region Enumeration for return types
    /// <summary>
    /// Permittable return types for a user defined function
    /// </summary>
    public enum ReturnTypes : sbyte
    {
        STRING = 0,
        REAL = 1,
        INTEGER = 2,
        DECIMAL = 4
    }
    #endregion

    #region The UDF document
    /// <summary>
    /// Implements a document functionality and represent a user defined function
    /// </summary>
    [DocumentObject(UdfDescriptor.TypeName, typeof(UdfDocument))]
    public class UdfDocument : BaseDocument
    {
        #region Identifying properties
        /// <summary>
        /// ID of the UDF
        /// </summary>
        [Browsable(false)]
        public override object[] ObjectID
        {
            get
            {
                return new object[] { null, Name };
            }
        }

        /// <summary>
        /// The old ID of the UDF (before changes)
        /// </summary>
        [Browsable(false)]
        public override object[] OldObjectID
        {
            get
            {
                return new object[] { null, OldName };
            }
        }
        #endregion

        #region Displayable properties
        /// <summary>
        /// UDFs have no comments
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
        /// Hides schema from list of properties
        /// </summary>
        [Browsable(false)]
        public override string Schema
        {
            get
            {
                return base.Schema;
            }
        }

        /// <summary>
        /// The basename of the file with implementation of the function 
        /// </summary>
        [LocalizableCategory("Category_Base")]
        [LocalizableDescription("Description_UDF_Dll")]
        [LocalizableDisplayName("DisplayName_UDF_Dll")]
        public string Dll
        {
            get
            {
                return GetAttributeAsString(Udf.Dll);
            }

            set
            {
                SetAttribute(Udf.Dll, value);
            }
        }

        /// <summary>
        /// Indicates if the function is aggregate
        /// </summary>
        [LocalizableCategory("Category_Base")]
        [LocalizableDescription("Description_UDF_IsAgregate")]
        [LocalizableDisplayName("DisplayName_UDF_IsAgregate")]
        public bool IsAggregate
        {
            get
            {
                string type = GetAttributeAsString(Udf.Type);
                if (string.IsNullOrEmpty(type))
                    return false;

                return (type.ToUpper() == "AGGREGATE");
            }

            set
            {
                SetAttribute(Udf.Type, value ? "AGGREGATE" : "FUNCTION");
            }
        }

        /// <summary>
        /// Return type of the function
        /// </summary>
        [LocalizableCategory("Category_Base")]
        [LocalizableDescription("Description_Routine_Returns")]
        [LocalizableDisplayName("DisplayName_Routine_Returns")]
        public ReturnTypes Returns
        {
            get
            {
                // Converting a byte index to an enumeration value
                object typeIndex = Attributes[Udf.Returns];
                if (!(typeIndex is sbyte))
                    return ReturnTypes.STRING;

                return (ReturnTypes)typeIndex;
            }

            set
            {
                // Converting an enumeration value to a byte index
                SetAttribute(Udf.Returns, (sbyte)value);
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
        public UdfDocument(ServerExplorerFacade hierarchy, bool isNew, object[] id)
            : base(hierarchy, isNew, id)
        {
        }
        #endregion

        #region Overridings
        /// <summary>
        /// Extracts a schema of the object from the latter's identifier
        /// </summary>
        /// <param name="id">The object identifier</param>
        /// <returns>The schema which the object belongs to</returns>
        protected override string  GetSchemaFromID(object[] id)
        {
            return null;
        }

        /// <summary>
        /// Initializes attributes of a new object
        /// </summary>
        /// <param name="newRow">A data row to write new attributes in</param>
        protected override void FillNewObjectAttributes(DataRow newRow)
        {
            if (newRow == null)
                throw new ArgumentNullException("newRow");

            newRow[Descriptor.NameAttributeName] = Name;
        }
        #endregion

        #region Building of queries
        /// <summary>
        /// Builds a query on a creation of a UDF
        /// </summary>
        /// <returns>A string representation of the query</returns>
        protected override string BuildCreateQuery()
        {
            return CreateQuery();
        }

        /// <summary>
        /// Returns query for UDF pre-dropping.
        /// </summary>
        /// <returns>Returns query for UDF pre-dropping.</returns>
        protected override string BuildPreDropQuery()
        {
            // Drop function to recreate it later.
            return "DROP FUNCTION IF EXISTS " + OldName;
        }

        /// <summary>
        /// Builds a query on a alteration of a UDF
        /// </summary>
        /// <returns>A string representation of the query</returns>
        protected override string BuildAlterQuery()
        {
            // Recreate function
            return CreateQuery();
        }

        /// <summary>
        /// Creates a query on a creation of UDF
        /// </summary>
        /// <returns></returns>
        private string CreateQuery()
        {
            StringBuilder sb = new StringBuilder();

            // Adding a header
            sb.Append("CREATE ");
            if (IsAggregate)
                sb.Append("AGGREGATE ");
            sb.Append("FUNCTION ");

            // Adding a name
            QueryBuilder.WriteIdentifier(Attributes, Udf.Name, sb);

            // Adding a return type
            sb.Append(" RETURNS ");
            QueryBuilder.WriteValue(Returns, sb, false);

            // Adding a file
            sb.Append(" SONAME ");
            QueryBuilder.WriteValue(Attributes, Udf.Dll, sb);            

            return sb.ToString();
        }
        #endregion
    }
    #endregion
}