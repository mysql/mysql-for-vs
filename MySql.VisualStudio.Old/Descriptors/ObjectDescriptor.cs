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
 * This file contains implementation of the base class for all descriptors.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using MySql.Data.VisualStudio.Utils;
using MySql.Data.VisualStudio.Properties;
using System.Globalization;
using System.Data;
using System.Collections;

namespace MySql.Data.VisualStudio.Descriptors
{
    #region A commonly used enumeration
    /// <summary>
    /// Types of security
    /// </summary>
    public enum SecurityTypes
    {
        DEFINER,
        INVOKER
    }
    #endregion

    #region ObjectDescriptor class
    /// <summary>
    /// This is the base class for all descriptors objects. It fully implement common 
    /// descriptor interface and uses reflection to get proper values from successor 
    /// static fields. For several properties uses custom reflection attributes.
    /// </summary>
    public class ObjectDescriptor : IObjectDescriptor
    {
        #region IObjectDescriptor Members
        /// <summary>
        /// Amount of parts in the object identifier. Root objects have only three 
        /// parts – catalog name, schema name and their own name.
        /// Read from IdLength custom attribute.
        /// </summary>
        public int IdLength
        {
            get
            {
                if (idLengthVal == -1)
                {
                    // Get identifier length
                    IdLengthAttribute idLength = ReflectionHelper.GetIdLengthAttribute(this.GetType());
                    Debug.Assert(idLength != null, "Failed to get identifier length!");
                    if (idLength == null)
                        throw new NotSupportedException(Resources.Error_NotMarkedWithIdLength);
                    this.idLengthVal = idLength.Length;
                }
                return idLengthVal;
            }
        }

        /// <summary>
        /// Type name how it was introduced in DataObject XML file.
        /// Read from ObjectDescriptor custom attribute.
        /// </summary>
        public string TypeName
        {
            get
            {
                if (typeNameVal == null)
                {
                    // Get object type name
                    ObjectDescriptorAttribute objectDescriptor = ReflectionHelper.GetObjectDescriptorAttribute(this.GetType());
                    Debug.Assert(objectDescriptor != null, "Failed to get object type name!");
                    if (objectDescriptor == null)
                        throw new NotSupportedException(Resources.Error_NotMarkedAsDescriptor);
                    typeNameVal = objectDescriptor.TypeName;
                }
                return typeNameVal;
            }
        }

        /// <summary>
        /// Returns array with object attributes names.
        /// </summary>
        public string[] ObjectAttributes
        {
            get
            {
                if (objectAttributesArray == null)
                    LoadAttributes();
                return objectAttributesArray;
            }
        }

        /// <summary>
        /// Returns array with object identifier parts names.
        /// </summary>
        public string[] Identifier
        {
            get
            {
                if (identifierArray == null)
                    LoadAttributes();
                return identifierArray;
            }
        }

        /// <summary>
        /// Returns name of the Schema attribute for this database object.
        /// </summary>
        public string SchemaAttributeName
        {
            get
            {
                if (schemaAttributeVal == null)
                    LoadAttributes();
                return schemaAttributeVal;
            }
        }

        /// <summary>
        /// Returns name of the Name attribute for this database object.
        /// </summary>
        public string NameAttributeName
        {
            get
            {
                if (nameAttributeVal == null)
                    LoadAttributes();
                return nameAttributeVal;
            }
        }

        /// <summary>
        /// Objects could not be droped by default. Returns false.
        /// </summary>
        public virtual bool CanBeDropped
        {
            get { return false; }
        }

        /// <summary>
        /// Objects could not be droped by default. Returns empty string.
        /// </summary>
        /// <param name="identifier">Database object identifier.</param>
        /// <returns>
        /// Objects could not be droped by default. Returns empty string.
        /// </returns>
        public virtual string BuildDropSql(object[] identifier)
        {
            Debug.Fail("Could not drop abstract object!");
            return String.Empty;
        }

        /// <summary>
        /// The lowest version of the MySQL Server where the descriptor's object 
        /// appears
        /// </summary>
        public virtual Version RequiredVersion
        {
            get
            {
                // The default minimal version
                return new Version(4, 0);
            }
        }
        #endregion

        #region Protected properties
        /// <summary>
        /// Returns name of the FieldString attribute for this database object
        /// </summary>
        protected string FieldsStringName
        {
            get
            {
                if (fieldStringNameVal == null)
                    LoadAttributes();

                return fieldStringNameVal;
            }
        }

        /// <summary>
        /// Returns a dictionary with known fields
        /// </summary>
        private Dictionary<string, FieldAttribute> Fields
        {
            get
            {
                if (fieldsDictionary == null)
                    LoadAttributes();

                return fieldsDictionary;
            }
        }
        #endregion

        #region Enumeration SQL
        /// <summary>
        /// Returns enumeration SQL template for a given connection. May use server version to customize result.
        /// </summary>
        /// <param name="connection">The DataConnectionWrapper to be used for enumeration.</param>
        /// <returns>Returns enumeration SQL template for a given connection.</returns>
        protected virtual string GetEnumerateSqlTemplate(DataConnectionWrapper connection)
        {
            return EnumerateSqlTemplate;
        }

        /// <summary>
        /// Template string used to generate enumerate SQL query.
        /// Read from EnumerateSqlTemplate static field of successor.
        /// </summary>
        protected virtual string EnumerateSqlTemplate
        {
            get
            {
                if (String.IsNullOrEmpty(enumerateSqlTemplateVal))
                {
                    // Get enumeration SQL template
                    enumerateSqlTemplateVal = GetFieldValue("EnumerateSqlTemplate") as string;
                    Debug.Assert(!String.IsNullOrEmpty(enumerateSqlTemplateVal), "Failed to get enumerate SQL template");
                    if (String.IsNullOrEmpty(enumerateSqlTemplateVal))
                        throw new NotSupportedException(Resources.Error_NoEnumerateSql);
                }
                return enumerateSqlTemplateVal;
            }
        }

        /// <summary>
        /// Returns default enumerate restrictions for a given connection. May use server version to customize result.
        /// </summary>
        /// <param name="connection">The DataConnectionWrapper to be used for enumeration.</param>
        /// <returns>Returns default enumerate restrictions for a given connection.</returns>
        protected virtual string[] GetDefaultRestrictions(DataConnectionWrapper connection)
        {
            return DefaultRestrictions;
        }

        /// <summary>
        /// String array with default restrictions values for enumeration query.
        /// Read from DefaultRestrictions static field of successor.
        /// </summary>
        protected virtual string[] DefaultRestrictions
        {
            get
            {
                if (defaultRestrictionsArray == null)
                {
                    // Get default restrictions array
                    defaultRestrictionsArray = GetFieldValue("DefaultRestrictions") as string[];
                    Debug.Assert(defaultRestrictionsArray != null, "Failed to get default restrictions");
                    if (defaultRestrictionsArray == null)
                        throw new NotSupportedException(Resources.Error_NoDefaultRestrictions);
                }
                return defaultRestrictionsArray;
            }
        }

        /// <summary>
        /// Default fields to be used in ORDER BY clause for enumeration query.
        /// Read from DefaultSortFields static field of successor.
        /// </summary>
        protected virtual string DefaultSortString
        {
            get
            {
                if (defaultSortStringVal == null)
                {
                    // Get default sort fields array
                    defaultSortStringVal = GetFieldValue("DefaultSortString") as string;
                    Debug.Assert(defaultSortStringVal != null, "Failed to get default sort fields");
                    if (defaultSortStringVal == null)
                        throw new NotSupportedException(Resources.Error_NoDefaultSort);
                }
                return defaultSortStringVal;
            }
        }
        #endregion

        #region Public static methods
        /// <summary>
        /// Returns multipart object identifier length for the given object type.
        /// </summary>
        /// <param name="typeName">Object type name.</param>
        /// <returns>Returns multipart object identifier length for the given object type.</returns>
        public static int GetIdentifierLength(string typeName)
        {
            if (typeName == null)
                throw new ArgumentNullException("typeName");

            IObjectDescriptor descriptor = ObjectDescriptorFactory.Instance.CreateDescriptor(typeName);
            if (descriptor == null)
                throw new NotSupportedException(String.Format(
                                CultureInfo.CurrentCulture,
                                Resources.Error_UnableToGetDescriptor,
                                typeName));

            return descriptor.IdLength;
        }

        /// <summary>
        /// Enumerates database object of given type with given restrictions into 
        /// DataTable.
        /// </summary>
        /// <param name="connection">The DataConnectionWrapper to be used for enumeration.</param>
        /// <param name="typeName">The type name for objects to be enumerated</param>
        /// <param name="restrictions">The restrictions to be putted on the retrieved objects set.</param>
        /// <returns>
        /// Returns DataTable which contains all database objects of given type which 
        /// satisfy given restrictions.
        /// </returns>
        public static DataTable EnumerateObjects(DataConnectionWrapper connection, string typeName, object[] restrictions)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (typeName == null)
                throw new ArgumentNullException("typeName");
            return EnumerateObjects(connection, typeName, restrictions, null);
        }

        /// <summary>
        /// Enumerates database object of given type with given restrictions into 
        /// DataTable.
        /// </summary>
        /// <param name="connection">The DataConnectionWrapper to be used for enumeration.</param>
        /// <param name="typeName">The type name for objects to be enumerated</param>
        /// <param name="restrictions">The restrictions to be putted on the retrieved objects set.</param>
        /// <param name="sort">Sort expresion to append after ORDER BY clause.</param>
        /// <returns>
        /// Returns DataTable which contains all database objects of given type which 
        /// satisfy given restrictions.
        /// </returns>
        public static DataTable EnumerateObjects(DataConnectionWrapper connection, string typeName, object[] restrictions, string sort)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (typeName == null)
                throw new ArgumentNullException("typeName");

            // Get object descriptor for given type
            ObjectDescriptor descriptor = ObjectDescriptorFactory.Instance.CreateDescriptor(typeName) as ObjectDescriptor;
            if (descriptor == null)
            {
                Debug.Fail("Unsupported object type '" + typeName + "'");
                throw new NotSupportedException(String.Format(
                    CultureInfo.CurrentCulture,
                    Resources.Error_ObjectTypeNotSupported,
                    typeName));
            }

            // Read objects
            DataTable result = descriptor.ReadTable(connection, restrictions, sort);
            if (result == null)
            {
                Debug.Fail("Failed to read data!");
                return null;
            }

            // Perform post-processing
            descriptor.PostProcessData(connection, result);
            result.AcceptChanges();

            // Return result
            return result;
        }

        /// <summary>
        /// Completes identifier for new object. Last element of id array considered as the name of new object. This
        /// method sequentially generates new names in form {template}{N} and checks for existing object with same name. 
        /// To check it this method tries to enumerate objects with restriction to whole id.
        /// </summary>
        /// <param name="hierarchy">Server explorer facade object used to check for existen objects.</param>
        /// <param name="typeName">Object type name.</param>
        /// <param name="id">Array with object identifier.</param>
        /// <param name="template">Template for the new object identifier.</param>
        public static void CompleteNewObjectID(ServerExplorerFacade hierarchy, string typeName, ref object[] id, string template)
        {
            if (hierarchy == null)
                throw new ArgumentNullException("hierarchy");
            if (typeName == null)
                throw new ArgumentNullException("typeName");
            if (id == null)
                throw new ArgumentNullException("id");
            if (String.IsNullOrEmpty(template))
                throw new ArgumentException(Resources.Error_EmptyString, "template");

            // Retrieve connection information
            DataConnectionWrapper connection = hierarchy.Connection;
            Debug.Assert(connection != null, "Empty connection object!");
            if (connection == null)
                return;

            // Calculate "name" part of identifier (it is the last part)
            int nameIdPart = id.Length - 1;
            Debug.Assert(nameIdPart >= 0, "Could not complete empty identifier!");
            if (nameIdPart < 0)
                return;

            // Initialize search context
            int objectIndex = 0;
            DataTable objectTable = null;
            bool objectsFounded = false;

            // Generate object name in <typeName><N> style
            do
            {
                // Build exact identifier
                id[nameIdPart] = template + (++objectIndex).ToString(CultureInfo.InvariantCulture);
                objectsFounded = false;

                try
                {
                    // Look for exisiting object with this identifier
                    objectTable = EnumerateObjects(connection, typeName, id, null);
                    objectsFounded = objectTable != null && objectTable.Rows != null && objectTable.Rows.Count > 0;
                }
                finally
                {
                    // Release resources
                    if (objectTable != null)
                        objectTable.Dispose();
                    objectTable = null;
                }

                // Look for registered document (may be second new object)
                objectsFounded = objectsFounded || hierarchy.HasDocument(typeName, id);
            }
            // Trying while objects are exists and objectIndex less when MaxObjectIndex
            while (objectsFounded && objectIndex < MaxObjectIndex);
        } 
        #endregion

        #region Attributes
        /// <summary>
        /// List of known common attributes
        /// </summary>
        public static class Attributes
        {
            public const string Name = "{0}_NAME";
            public const string Comments = "{0}_COMMENT";
        }
        #endregion

        #region Enumeration and Post processing
        /// <summary>
        /// Reads table with Database Objects which satisfy given restriction. Base implementation 
        /// uses direct SQL query to the INFORMATION_SCHEMA.
        /// </summary>
        /// <param name="connection">The DataConnectionWrapper to be used for enumeration.</param>
        /// <param name="restrictions">The restrictions to be putted on the retrieved objects set.</param>
        /// <param name="sort">Sort expresion to append after ORDER BY clause.</param>
        /// <returns>Returns table with Database Objects which satisfy given restriction.</returns>
        protected virtual DataTable ReadTable(DataConnectionWrapper connection, object[] restrictions, string sort)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            if (connection.ServerVersion != null && RequiredVersion > connection.ServerVersion)
                // This object requires a higher version of the MySql Server
                return new DataTable();

            return connection.ExecuteSelectTable(BuildEnumerateSql(connection, restrictions, sort));
        }

        /// <summary>
        /// Builds enumerate SQL query for object of this type with given restrictions.
        /// </summary>
        /// <param name="connection">The DataConnectionWrapper to be used for enumeration.</param>
        /// <param name="restrictions">Restrictions to enumerated objects.</param>
        /// <param name="sort">Sort expression to use.</param>
        /// <returns>Enumerating SQL query string.</returns>
        protected virtual string BuildEnumerateSql(DataConnectionWrapper connection, object[] restrictions, string sort)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            // Extract enumeration SQL information from the descriptor
            string sqlTemplate = GetEnumerateSqlTemplate(connection);
            string[] defaultRestrictions = GetDefaultRestrictions(connection);
            if (String.IsNullOrEmpty(sqlTemplate) || defaultRestrictions == null)
            {
                Debug.Fail("Failed to get enumeration SQL information for object type '" + TypeName + "'");
                throw new NotSupportedException(String.Format(
                    CultureInfo.CurrentCulture,
                    Resources.Error_UnableToGetEnumerationSql,
                    TypeName));
            }

            // Get formated SQL
            string sqlStatement = FormatSqlString(sqlTemplate, restrictions, defaultRestrictions);

            // Check builded statement
            Debug.Assert(!String.IsNullOrEmpty(sqlStatement), "Failed to build enumeration statement!");
            if (String.IsNullOrEmpty(sqlStatement))
                throw new NotSupportedException(String.Format(
                    CultureInfo.CurrentCulture,
                    Resources.Error_UnableToGetEnumerationSql,
                    TypeName));

            // Append ORDER BY if any
            string sortExpression = !String.IsNullOrEmpty(sort) ? sort : DefaultSortString;
            if (!String.IsNullOrEmpty(sortExpression))
                return String.Format("{0} ORDER BY {1}", sqlStatement, sortExpression);

            // Retrun results
            return sqlStatement;
        }

        /// <summary>
        /// Post process enumeration data. Base implementation adds primary key to 
        /// the table and calculate fields
        /// </summary>
        /// <param name="connection">The DataConnectionWrapper to be used for enumeration</param>
        /// <param name="table">A table with data to post process</param>
        protected virtual void PostProcessData(DataConnectionWrapper connection, DataTable table)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (table == null)
                throw new ArgumentNullException("table");

            // Adding table columns
            ExtendData(connection, table);

            // Validate read data
            if (!ValidateAttributesTable(table))
                throw new Exception(Resources.Error_AttributesAreMissing);

            // Add primary key
            AddPrimaryKey(table);

        }

        /// <summary>
        /// Exdents table data with additional information. Base implementation adds
        /// attributes of this database object
        /// </summary>
        /// <param name="connection">The DataConnectionWrapper to be used for enumeration.</param>
        /// <param name="table">Table with data to extend.</param>
        protected virtual void ExtendData(DataConnectionWrapper connection, DataTable table)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (table == null)
                throw new ArgumentNullException("table");

            if (Fields == null || Fields.Count <= 0)
                // No known fields
                return;

            // Add columns for fields
            Debug.Assert(table.Columns != null, "Table has empty column collection!");
            Dictionary<string, FieldAttribute> added = new Dictionary<string, FieldAttribute>();
            foreach (KeyValuePair<string, FieldAttribute> field in Fields)
            {
                // If column is already exists, skip it
                if (table.Columns.Contains(field.Key))
                    continue;
                
                // Add column for field
                AddFieldColumn(table, field.Key, field.Value.FieldType);
                
                // Add entry to added dictionary
                added[field.Key] = field.Value;
            }

            // Calculate field values for each row
            Dictionary<string, string> fieldValues;
            foreach (DataRow row in table.Rows)
            {
                // Parsing field string
                fieldValues = ExtractOptions(connection, row);
                if (fieldValues == null || fieldValues.Count <= 0)
                    continue;

                // Adding a field
                foreach (KeyValuePair<string, FieldAttribute> field in added)
                {
                    // Skip empty option names and columns which was in result before option adding.
                    if (string.IsNullOrEmpty(field.Value.OptionName))
                        continue;

                    AddFieldValue(row, field.Key, field.Value.OptionName, field.Value.FieldType, fieldValues);
                }
            }
        }

        /// <summary>
        /// Extracts field values for given DataRow. Base implementation simply uses Parser
        /// </summary>
        /// <param name="connection">The DataConnectionWrapper to be used for enumeration</param>
        /// <param name="row">DataRow to extract values</param>
        /// <returns>Returns field values for a given DataRow</returns>
        protected virtual Dictionary<string, string> ExtractOptions(DataConnectionWrapper connection, DataRow row)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (row == null)
                throw new ArgumentNullException("row");

            // Check a field string (if empty, returns empty dictionary)
            if (String.IsNullOrEmpty(FieldsStringName))
                return new Dictionary<string, string>();

            return Parser.ExtractFieldsDictionary(row, FieldsStringName);
        }

        /// <summary>
        /// Adds primary key to the table. Base implementation adds all 
        /// known identifier parts.
        /// </summary>
        /// <param name="table">Table to add primary key</param>
        protected virtual void AddPrimaryKey(DataTable table)
        {
            DataColumn[] primaryKey = new DataColumn[Identifier.Length];
            for (int i = 0; i < Identifier.Length; i++)
            {
                primaryKey[i] = table.Columns[Identifier[i]];
                Debug.Assert(primaryKey[i] != null, "Failed to read primaruy key column!");
            }
            table.PrimaryKey = primaryKey;
        }

        /// <summary>
        /// Check table with object attributes for consistency. 
        /// Typical only ensure that all attribute columns are present.
        /// </summary>
        /// <param name="table">DataTable object to check for consistency.</param>
        /// <returns>Returns true if table considered as consistent and false otherwise.</returns>
        protected virtual bool ValidateAttributesTable(DataTable table)
        {
            if (table == null)
                throw new ArgumentNullException("objectTable");

            // Get list of object attributes
            string[] objectAttributes = ObjectAttributes;
            Debug.Assert(objectAttributes != null, "Unable to get attributes list!");
            if (objectAttributes == null)
                return false;

            // Check, if table contains all necessary columns
            foreach (string attribute in objectAttributes)
                if (!table.Columns.Contains(attribute))
                    return false;

            // All attributes are present
            return true;
        }
        #endregion

        #region Protected utility methods
        /// <summary>
        /// Renames column in the DataTable.
        /// </summary>
        /// <param name="oldName">Name of exists column.</param>
        /// <param name="newName">New name for a column.</param>
        /// <param name="table">DataTable to perform operation.</param>
        protected void RenameColumn(string oldName, string newName, DataTable table)
        {
            if (String.IsNullOrEmpty(oldName))
                throw new ArgumentException(Resources.Error_EmptyString, "oldName");
            if (String.IsNullOrEmpty(newName))
                throw new ArgumentException(Resources.Error_EmptyString, "newName");
            if (table == null)
                throw new ArgumentNullException("table");

            // Check table columns
            if (table.Columns == null || !table.Columns.Contains(oldName))
            {
                Debug.Fail("Table columns are empty or there is no column " + oldName);
                return;
            }

            // Extract proper column
            DataColumn column = table.Columns[oldName];
            if (column == null)
            {
                Debug.Fail("Failed to extract column with name " + oldName);
                return;
            }

            // Rename column
            column.ColumnName = newName;
        }
        #endregion

        #region Field processing private methods
        /// <summary>
        /// Creates new column in the DataTable for a field attribute
        /// </summary>
        /// <param name="table">DataTable to add column</param>
        /// <param name="columnName">Name of the column</param>
        /// <param name="attributeType">Type of the column</param>
        private static void AddFieldColumn(DataTable table, string columnName, TypeCode attributeType)
        {
            if (table == null)
                throw new ArgumentNullException("table");

            // Check, if table already has suh column
            if (table.Columns.Contains(columnName))
                return;

            switch (attributeType)
            {
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    table.Columns.Add(columnName, typeof(Int64));
                    break;
                case TypeCode.Boolean:
                case TypeCode.String:
                    table.Columns.Add(columnName, typeof(string));
                    break;
                case TypeCode.DateTime:
                    table.Columns.Add(columnName, typeof(DateTime));
                    break;
                default:
                    Debug.Fail("Unsupported type code is Sused!");
                    table.Columns.Add(columnName, typeof(string));
                    break;
            }
        }

        /// <summary>
        /// Adds a field value to the given data row. Choses field conversion type 
        /// depending on give attribute type
        /// </summary>
        /// <param name="row">DataRow to add the field value</param>
        /// <param name="attributeName">Attribute name for which the value should be added</param>
        /// <param name="fieldName">Field name to look for value in the dictionary</param>
        /// <param name="attributeType">Attribute type to use for conversion</param>
        /// <param name="fieldValues">Dictionary with read field values</param>
        private static void AddFieldValue(DataRow row, string attributeName, string fieldName, TypeCode attributeType, Dictionary<string, string> fieldValues)
        {
            if (row == null)
                throw new ArgumentNullException("row");
            if (String.IsNullOrEmpty(attributeName))
                throw new ArgumentException(Resources.Error_EmptyString, "attributeName");
            if (String.IsNullOrEmpty(fieldName))
                throw new ArgumentException(Resources.Error_EmptyString, "fieldName");

            object value = null;

            // Select proper conversion depending on the attribute type
            switch (attributeType)
            {
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    // If a field has a valid value in the dictionary, copy it to the row
                    value = GetIntField(fieldName, fieldValues);
                    break;
                case TypeCode.Boolean:
                    // If a field has a valid value in the dictionary, copy it to the row
                    value = GetBoolField(fieldName, fieldValues);
                    break;
                case TypeCode.String:
                    // If a field has a valid value in the dictionary, copy it to the row
                    value = GetStringField(fieldName, fieldValues);
                    break;
                default:
                    Debug.Fail("Unsuported type code used!");
                    // If a field has a valid value in the dictionary, copy it to the row
                    value = GetStringField(fieldName, fieldValues);
                    break;
            }

            if (value != null)
                row[attributeName] = value;
        }

        /// <summary>
        /// Returns value of the field, interpreted as integer. If value is in the dictionary, tries 
        /// to convert it to the Int64 and return result. Otherwise returns null.
        /// </summary>
        /// <param name="fieldName">Field name to look for value in the dictionary.</param>        
        /// <param name="fieldValues">Dictionary with read field values</param>
        /// <returns>
        /// Returns value of the field, interpreted as integer. If value is in the dictionary, tries 
        /// to convert it to the Int64 and return result. Otherwise returns null.
        /// </returns>
        private static object GetIntField(string fieldName, Dictionary<string, string> fieldValues)
        {
            if (fieldValues.ContainsKey(fieldName))
            {
                // Extract and validate string
                string stringVal = fieldValues[fieldName];
                if (!String.IsNullOrEmpty(stringVal))
                {
                    // Parse string as integer
                    Int64 intVal;
                    if (Int64.TryParse(stringVal, out intVal))
                        return intVal;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns value of the field, interpreted as Boolean. If any value is in the dictionary, 
        /// returns true string, otherwise returns false string.
        /// </summary>
        /// <param name="fieldName">Field name to look for value in the dictionary.</param>        
        /// <param name="fieldValues">Dictionary with read field values.</param>
        /// <returns>
        /// Returns value of the field, interpreted as Boolean. If any value is in the dictionary, 
        /// returns true string, otherwise returns false string.
        /// </returns>
        private static object GetBoolField(string fieldName, Dictionary<string, string> fieldValues)
        {
            return fieldValues.ContainsKey(fieldName) ? DataInterpreter.True : DataInterpreter.False;
        }

        /// <summary>
        /// Returns value of the field, interpreted as string. If nonempty value is in the dictionary, 
        /// returns this value, otherwise returns null.
        /// </summary>
        /// <param name="fieldName">Field name to look for value in the dictionary.</param>        
        /// <param name="fieldValues">Dictionary with read field values.</param>
        /// <returns>
        /// Returns value of the field, interpreted as string. If nonempty value is in the dictionary, 
        /// returns this value, otherwise returns null.
        /// </returns>
        private static object GetStringField(string fieldName, Dictionary<string, string> fieldValues)
        {
            // If field has a valid value in the dictionary, copy it to the row
            if (fieldValues.ContainsKey(fieldName))
            {
                // Extract and validate string
                string stringVal = fieldValues[fieldName];
                if (!String.IsNullOrEmpty(stringVal))
                    return stringVal;
            }
            return null;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method formats a SQL string by specifying format arguments
        /// based on restrictions. Used to escape special characters (at this 
        /// point only quotes).
        /// </summary>
        /// <param name="sql">SQL query template.</param>
        /// <param name="restrictions">Restrictions to be placed in the template.</param>
        /// <param name="defaultRestrictions">Default values for unspecified restrictions.</param>
        /// <returns>Formatted and escaped SQL query.</returns>
        private static string FormatSqlString(string sql, object[] restrictions, object[] defaultRestrictions)
        {
            Debug.Assert(sql != null);
            Debug.Assert(defaultRestrictions != null);

            object[] formatArgs = new object[defaultRestrictions.Length];
            //formatArgs[0] = (restrictions[0] as string).Replace("]", "]]");
            for (int i = 0; i < defaultRestrictions.Length; i++)
            {
                if (restrictions != null && restrictions.Length > i && restrictions[i] != null)
                {
                    formatArgs[i] = QueryBuilder.EscapeAndQuoteString(restrictions[i].ToString());
                }
                else
                {
                    formatArgs[i] = defaultRestrictions[i];
                }
            }
            return String.Format(CultureInfo.CurrentCulture, sql, formatArgs);
        }

        /// <summary>
        /// Used to get value for given static field of successor via Reflection.
        /// </summary>
        /// <param name="fieldName">Field name.</param>
        /// <returns>Returns value for given static field of successor.</returns>
        private object GetFieldValue(string fieldName)
        {
            Type type = this.GetType();
            return type.InvokeMember(
                            fieldName,
                            BindingFlags.GetField | BindingFlags.GetProperty
                            | BindingFlags.NonPublic | BindingFlags.Static
                            | BindingFlags.Public,
                            null, null, new object[] { });
        }

        /// <summary>
        /// Loads sttributes names array
        /// </summary>
        private void LoadAttributes()
        {
            // Get array of object attributes names

            // Get current object type
            Type thisType = this.GetType();
            Debug.Assert(thisType != null);

            // Get attributes type
            Type attributes = thisType.GetNestedType("Attributes");
            Debug.Assert(attributes != null, "Attributes container class is undefined!");
            if (attributes == null)
                throw new NotSupportedException(Resources.Error_NoAttributesNestedClass);

            // Get fields array
            FieldInfo[] fields = attributes.GetFields();
            Debug.Assert(fields != null, "Unable to get fields list!");
            if (fields == null || fields.Length <= 0)
                throw new NotSupportedException(Resources.Error_EmptyAttributesNestedClass);

            // Parse and extract attributes information
            ParseAttributes(fields);

            // Validate and complete result
            if (String.IsNullOrEmpty(schemaAttributeVal))
                schemaAttributeVal = String.Empty;
            if (String.IsNullOrEmpty(nameAttributeVal))
                nameAttributeVal = String.Empty;
            if (String.IsNullOrEmpty(fieldStringNameVal))
                fieldStringNameVal = String.Empty;
        }

        /// <summary>
        /// Extarcts information about Database Object attributes from the array of FieldInfo.
        /// </summary>
        /// <param name="fields">Array with information about Database Object attributes.</param>
        private void ParseAttributes(FieldInfo[] fields)
        {
            // Initialize temporary data
            List<string> identifier = new List<string>();
            objectAttributesArray = new string[fields.Length];
            IdentifierAttribute identifierFlag;
            fieldsDictionary = new Dictionary<string, FieldAttribute>();
            FieldAttribute fieldMark;

            // Extracts value for each static field
            for (int i = 0; i < fields.Length; i++)
            {
                // Validate field type
                if (!fields[i].IsStatic || !typeof(String).IsAssignableFrom(fields[i].FieldType))
                {
                    Debug.Fail("Unsupported attribute declaration found!");
                    continue;
                }

                // Extract field value    
                objectAttributesArray[i] = fields[i].GetValue(null) as string;
                if (String.IsNullOrEmpty(objectAttributesArray[i]))
                    throw new NotSupportedException(Resources.Error_EmptyAttributeName);

                // Check for Identifier attribute
                identifierFlag = ReflectionHelper.GetIdentifierAttribute(fields[i]);
                if (identifierFlag != null)
                {
                    identifier.Add(objectAttributesArray[i]);

                    // Check if schema attribute
                    if (identifierFlag.IsSchema)
                    {
                        Debug.Assert(String.IsNullOrEmpty(schemaAttributeVal), "Duplicate schema attribute!");
                        schemaAttributeVal = objectAttributesArray[i];
                    }

                    // Check if name attribute
                    if (identifierFlag.IsName)
                    {
                        Debug.Assert(String.IsNullOrEmpty(nameAttributeVal), "Duplicate name attribute!");
                        nameAttributeVal = objectAttributesArray[i];
                    }
                }

                // Check if this field is string with fields
                if (ReflectionHelper.GetFieldStringAttribute(fields[i]) != null)
                {
                    Debug.Assert(String.IsNullOrEmpty(fieldStringNameVal), "Duplicate field string attribute!");
                    fieldStringNameVal = objectAttributesArray[i];
                }

                // Check if this is a field
                fieldMark = ReflectionHelper.GetFieldAttribute(fields[i]);
                if (fieldMark != null)
                {
                    Debug.Assert(!fieldsDictionary.ContainsKey(objectAttributesArray[i]), "This field already exists!");
                    fieldsDictionary[objectAttributesArray[i]] = fieldMark;
                }
            }

            // Copy extracted identifier
            identifierArray = new string[identifier.Count];
            identifier.CopyTo(identifierArray);

            // Initialize default name
            if (String.IsNullOrEmpty(nameAttributeVal))
            {
                nameAttributeVal = String.Format(
                                    Attributes.Name,
                                    TypeName.ToUpperInvariant(),
                                    CultureInfo.InvariantCulture);
            }
        }
        #endregion

        #region Private variables to store properties
        private string enumerateSqlTemplateVal = null;
        private string[] defaultRestrictionsArray = null;
        private string defaultSortStringVal = null;
        private int idLengthVal = -1;
        private string typeNameVal = null;
        private string[] objectAttributesArray = null;
        private string[] identifierArray = null;
        private string nameAttributeVal = null;
        private string schemaAttributeVal = null;
        private string fieldStringNameVal = null;
        private Dictionary<string, FieldAttribute> fieldsDictionary;
        #endregion

        #region Constants
        /// <summary>
        /// Maximum object index for new identifier generator.
        /// </summary>
        private const int MaxObjectIndex = 50;
        #endregion
    }
    #endregion
}
