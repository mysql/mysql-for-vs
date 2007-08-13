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
 * This file contains Table object descriptor
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using MySql.Data.VisualStudio.Utils;
using System.Diagnostics;
using MySql.Data.VisualStudio.Properties;
using System.Globalization;
using System.Data.Common;

namespace MySql.Data.VisualStudio.Descriptors
{
    /// <summary>
    /// This is the Table object descriptor.
    /// </summary>
    [ObjectDescriptor(TableDescriptor.TypeName, typeof(TableDescriptor))]
    [IdLength(3)]
    public class TableDescriptor: ObjectDescriptor
    {
        /// <summary>
        /// Table object type name.
        /// </summary>
        public new const string TypeName = "Table";

        #region Enumerate SQL
        /// <summary>
        /// Table enumerate SQL template.
        /// </summary>
        protected new const string EnumerateSqlTemplate =
            "SELECT "
            + "t.TABLE_CATALOG, "
            + "t.TABLE_SCHEMA, "
            + "t.TABLE_NAME, "
            + "t.TABLE_TYPE, "
            + "t.`ENGINE`, "
            + "t.VERSION, "
            + "t.`ROW_FORMAT`, "
            + "t.TABLE_ROWS, "
            + "t.AVG_ROW_LENGTH, "
            + "t.DATA_LENGTH, "
            + "t.MAX_DATA_LENGTH, "
            + "t.INDEX_LENGTH, "
            + "t.DATA_FREE, "
            + "t.AUTO_INCREMENT, "
            + "t.CREATE_TIME, "
            + "t.UPDATE_TIME, "
            + "t.CHECK_TIME, "
            + "ca.CHARACTER_SET_NAME AS TABLE_CHARACTER_SET, "
            + "t.TABLE_COLLATION, "
            + "t.`CHECKSUM`, "
            + "t.CREATE_OPTIONS, "
            + "t.TABLE_COMMENT "
            + "FROM information_schema.`TABLES` t "
            + "LEFT JOIN information_schema.COLLATION_CHARACTER_SET_APPLICABILITY ca "
            + "ON t.TABLE_COLLATION = ca.COLLATION_NAME "
            + "WHERE TABLE_SCHEMA = {1} AND TABLE_NAME = {2} AND TABLE_TYPE = {3} "
            + "AND TABLE_TYPE != 'VIEW'";

        /// <summary>
        /// Table enumeration defaults.
        /// </summary>
        protected new static readonly string[] DefaultRestrictions =
		{
			"",
            "TABLE_SCHEMA",
			"TABLE_NAME",
			"TABLE_TYPE"			
		};

        /// <summary>
        /// Table default sort fields.
        /// </summary>
        protected new const string DefaultSortString = "TABLE_SCHEMA, TABLE_NAME"; 
        #endregion

        #region Attributes
        /// <summary>
        /// List of known attributes for Table object
        /// </summary>
        public static new class Attributes
        {
            [Field(FieldType = TypeCode.String)]
            public const string Database = "TABLE_CATALOG";            
            [Field(OptionName = SchemaField, FieldType = TypeCode.String)]
            [Identifier(IsSchema = true)]
            public const string Schema = "TABLE_SCHEMA";
            [Field(FieldType = TypeCode.String)]
            [Identifier(IsName=true)]
            public const string Name = "TABLE_NAME";
            [Field(FieldType = TypeCode.String)]
            public const string Type = "TABLE_TYPE";
            [Field(FieldType = TypeCode.String)]
            public const string Engine = "ENGINE";
            [Field(FieldType = TypeCode.Int64)]
            public const string Version = "VERSION";
            [Field(FieldType = TypeCode.String)]
            public const string RowFormat = "ROW_FORMAT";
            [Field(FieldType = TypeCode.Int64)]
            public const string TableRows = "TABLE_ROWS";
            [Field(FieldType = TypeCode.Int64)]
            public const string AverageRowLength = "AVG_ROW_LENGTH";
            [Field(FieldType = TypeCode.Int64)]
            public const string DataLength = "DATA_LENGTH";
            [Field(FieldType = TypeCode.Int64)]
            public const string MaxDataLength = "MAX_DATA_LENGTH";
            [Field(FieldType = TypeCode.Int64)]
            public const string IndexLength = "INDEX_LENGTH";
            [Field(FieldType = TypeCode.Int64)]
            public const string DataFree = "DATA_FREE";
            [Field(FieldType = TypeCode.Int64)]
            public const string AutoIncrement = "AUTO_INCREMENT";
            [Field(FieldType = TypeCode.DateTime)]
            public const string CreateDateTime = "CREATE_TIME";
            [Field(FieldType = TypeCode.DateTime)]
            public const string UpdateDateTime = "UPDATE_TIME";
            [Field(FieldType = TypeCode.DateTime)]
            public const string CheckDateTime = "CHECK_TIME";
            [Field(OptionName = CharacterSetField, FieldType = TypeCode.String)]
            public const string CharacterSet = "TABLE_CHARACTER_SET";
            [Field(FieldType = TypeCode.String)]
            public const string Collation = "TABLE_COLLATION";
            [Field(FieldType = TypeCode.Int64)]
            public const string Checksum = "CHECKSUM";
            [Field(FieldType = TypeCode.String)]
            [OptionString]
            public const string CreateOptions = "CREATE_OPTIONS";
            [Field(FieldType = TypeCode.String)]
            public const string Comments = "TABLE_COMMENT";

            [Field(OptionName = "avg_row_length", FieldType = TypeCode.Int64)]
            public const string AverageRowLengthField = "AverageRowLengthField";

            [Field(OptionName = "checksum", FieldType = TypeCode.Int64)]
            public const string ChecksumField = "ChecksumField";

            [Field(OptionName = "min_rows", FieldType = TypeCode.Int64)]
            public const string MinRows = "MinRowsField";

            [Field(OptionName = "row_format", FieldType = TypeCode.String)]
            public const string RowFormatField = "RowFormatField";

            [Field(OptionName = "max_rows", FieldType = TypeCode.Int64)]
            public const string MaxRows = "MaxRowsField";

            [Field(OptionName = ConnectionField, FieldType = TypeCode.String)]
            public const string Connection = "ConnectionField";

            [Field(OptionName = DataDirectoryField, FieldType = TypeCode.String)]
            public const string DataDirectory = "DataDirectoryField";

            [Field(OptionName = IndexDirectoryField, FieldType = TypeCode.String)]
            public const string IndexDirectory = "IndexDirectoryField";

            [Field(OptionName = "delay_key_write", FieldType = TypeCode.Int64)]
            public const string DelayKeyWrite = "DelayKeyWriteField";

            [Field(OptionName = "pack_keys", FieldType = TypeCode.String)]
            public const string PackKeys = "PackKeysField";

            [Field(OptionName = "password", FieldType = TypeCode.String)]
            public const string Password = "PasswordField";

            [Field(OptionName = InsertMethodField, FieldType = TypeCode.String)]
            public const string InsertMethod = "InsertMethodField";

            [Field(OptionName = UnionField, FieldType = TypeCode.String)]
            public const string Union = "UnionField";
        }
        #endregion

        #region Enumerate method
        /// <summary>
        /// Enumerates tables with given restrictions into DataTable.
        /// </summary>
        /// <param name="connection">The DataConnectionWrapper to be used for enumeration.</param>
        /// <param name="restrictions">The restrictions to be putted on the retrieved objects set.</param>
        /// <returns>
        /// Returns DataTable which contains all tables which satisfy given restrictions.
        /// </returns>
        public static DataTable Enumerate(DataConnectionWrapper connection, object[] restrictions)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            return ObjectDescriptor.EnumerateObjects(connection, TypeName, restrictions);
        }
        #endregion

        #region Dropping
        /// <summary>
        /// Tables can be dropped. Returns true.
        /// </summary>
        public override bool CanBeDropped
        {
            get { return true; }
        }

        /// <summary>
        /// Returns DROP TABLE statement.
        /// </summary>
        /// <param name="identifier">Database object identifier.</param>
        /// <returns>Returns DROP TABLE statement.</returns>
        public override string BuildDropSql(object[] identifier)
        {
            if (identifier == null)
                throw new ArgumentNullException("identifier");
            if (identifier.Length != 3 || String.IsNullOrEmpty(identifier[1] as string) || String.IsNullOrEmpty(identifier[2] as string))
                throw new ArgumentException(
                    String.Format(
                        CultureInfo.CurrentCulture,
                        Resources.Error_InvlaidIdentifier,
                        identifier.Length,
                        TypeName,
                        3),
                     "id");
            
            // Build query
            StringBuilder query = new StringBuilder("DROP TABLE ");
            QueryBuilder.WriteIdentifier(identifier[1] as string, identifier[2] as string, query);
            return query.ToString();
        }
        #endregion

        #region Aditional constants
        /// <summary>
        /// Un-displayable name MRG_MyISAM
        /// </summary>
        public const string MRG_MyISAM = "MRG_MyISAM";
        /// <summary>
        /// Displayable name MERGE
        /// </summary>
        public const string MERGE = "MERGE";
        /// <summary>
        /// MEMORY engine name
        /// </summary>
        public const string MEMORY = "MEMORY";
        /// <summary>
        /// MyISAM engine name
        /// </summary>
        public const string MyISAM = "MyISAM";
        /// <summary>
        /// InnoDB engine name
        /// </summary>
        public const string InnoDB = "InnoDB";
        /// <summary>
        /// ARCHIVE engine name
        /// </summary>
        public const string ARCHIVE = "ARCHIVE";
        /// <summary>
        /// BDB engine name
        /// </summary>
        public const string BDB = "BDB";
        /// <summary>
        /// NDB engine name
        /// </summary>
        public const string NDB = "NDB";
        /// <summary>
        /// Default type for columns ends with ID
        /// </summary>
        public const string DefaultIntType = "int(10)";
        /// <summary>
        /// Default type for othe columns
        /// </summary>
        public const string DefaultCharType = "varchar(45)";
        #endregion

        #region Private constants
        private const string ConnectionQueryPart = "CONNECTION=";
        private const string ConnectionField = "connection";
        private const string InsertMethodQueryPart = "INSERT_METHOD=";
        private const string InsertMethodField = "insert_method";
        private const string UnionQueryPart = "UNION=";
        private const string UnionField = "union";
        private const string DataDirectoryQueryPart = "DATA DIRECTORY=";
        private const string DataDirectoryField = "data_directory";
        private const string IndexDirectoryQueryPart = "INDEX DIRECTORY=";
        private const string IndexDirectoryField = "index_directory";
        private const string CharacterSetField = "character_set";
        private const string SchemaField = "schema";
        private const string CreateTableColumn = "Create Table";
        #endregion

        #region Legacy MySQL version support
        /// <summary>
        /// Builds enumerate SQL query for object of this type with given restrictions. For legacy version uses
        /// SHOW TABLE STATUS instead INFORMATION_SCHEMA.
        /// </summary>
        /// <param name="connection">The DataConnectionWrapper to be used for enumeration.</param>
        /// <param name="restrictions">Restrictions to enumerated objects.</param>
        /// <param name="sort">Sort expression to use.</param>
        /// <returns>Enumerating SQL query string.</returns>
        protected override string BuildEnumerateSql(DataConnectionWrapper connection, object[] restrictions, string sort)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            // Extract server version
            Version serverVersion = connection.ServerVersion;

            // For latest version just call base
            if (serverVersion == null || serverVersion.Major >= 5)
                return base.BuildEnumerateSql(connection, restrictions, sort);

            // Build SHOW TABLE STATUS
            StringBuilder query = new StringBuilder("SHOW TABLE STATUS");

            // If there is a restriction on schema, apply it
            if (restrictions != null && restrictions.Length >= 2 && !String.IsNullOrEmpty(restrictions[1] as string))
            {
                query.Append(" FROM ");
                QueryBuilder.WriteIdentifier(restrictions[1] as string, query);
            }            

            // If there is a restriction on table, apply it
            if (restrictions != null && restrictions.Length >= 3)
                QueryBuilder.WriteIfNotEmptyString(restrictions[2], " LIKE ", query);

            // Return result
            return query.ToString();
        }

        /// <summary>
        /// Reads table with Database Objects which satisfy given restriction. Base implementation 
        /// uses direct SQL query to the INFORMATION_SCHEMA.
        /// </summary>
        /// <param name="connection">The DataConnectionWrapper to be used for enumeration.</param>
        /// <param name="restrictions">The restrictions to be putted on the retrieved objects set.</param>
        /// <param name="sort">Sort expresion to append after ORDER BY clause.</param>
        /// <returns>Returns table with Database Objects which satisfy given restriction.</returns>
        protected override DataTable ReadTable(DataConnectionWrapper connection, object[] restrictions, string sort)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

           // Extract server version
            Version serverVersion = connection.ServerVersion;

            // For latest version just return base result
            if (serverVersion == null || serverVersion.Major >= 5)
                return base.ReadTable(connection, restrictions, sort);

            // Execute base method
            DataTable result;
            try
            {
                result = base.ReadTable(connection, restrictions, sort);
            }
            catch(DbException)
            {
                // This most probably meanes that table is not exists. Return empty table on this
                return new DataTable();
            }

            // If result is null, exit
            if (result == null)
                return null;


            // For legacy version rename columns
            RenameColumn("Name",            Attributes.Name, result);
            RenameColumn("Version",         Attributes.Version, result);
            RenameColumn("Row_format",      Attributes.RowFormat, result);
            RenameColumn("Rows",            Attributes.TableRows, result);
            RenameColumn("Avg_row_length",  Attributes.AverageRowLength, result);
            RenameColumn("Data_length",     Attributes.DataLength, result);
            RenameColumn("Max_data_length", Attributes.MaxDataLength, result);
            RenameColumn("Index_length",    Attributes.IndexLength, result);
            RenameColumn("Data_free",       Attributes.DataFree, result);
            RenameColumn("Auto_increment",  Attributes.AutoIncrement, result);
            RenameColumn("Create_time",     Attributes.CreateDateTime, result);
            RenameColumn("Update_time",     Attributes.UpdateDateTime, result);
            RenameColumn("Check_time",      Attributes.CheckDateTime, result);
            RenameColumn("Create_options",  Attributes.CreateOptions, result);
            RenameColumn("Comment",         Attributes.Comments, result);

            // Engine was called type before 4.1.2
            if (serverVersion < new Version(4, 1, 2))
                RenameColumn("Type", Attributes.Engine, result);
            else
                RenameColumn("Engine", Attributes.Engine, result);

            // Engine collation and checksum are implemented only in 4.1.1
            if (serverVersion < new Version(4, 1, 1))
            {
                result.Columns.Add(Attributes.Collation, typeof(string));
                result.Columns.Add(Attributes.Checksum, typeof(Int64));
            }
            else
            {
                RenameColumn("Collation", Attributes.Collation, result);
                RenameColumn("Checksum", Attributes.Checksum, result);
            }
            
            // Calculate schema name
            string schema;
            if (restrictions != null && restrictions.Length >= 2 && !String.IsNullOrEmpty(restrictions[1] as string))
                schema = restrictions[1] as string;
            else
                schema = connection.Schema;
            
            // Add catalog, schema and type column
            result.Columns.Add(Attributes.Database, typeof(string));
            result.Columns.Add(Attributes.Schema, typeof(string));
            result.Columns.Add(Attributes.Type, typeof(string));

            // Set schema and type name for each row
            foreach (DataRow table in result.Rows)
            {
                DataInterpreter.SetValueIfChanged(table, Attributes.Schema, schema);
                DataInterpreter.SetValueIfChanged(table, Attributes.Type, "BASE TABLE");
            }

            // Finaly, return result
            return result;
        }
        #endregion

        #region Aditional fields extracting
        /// <summary>
        /// Extracts field values for given DataRow. Base implementation simply uses Parser.
        /// </summary>
        /// <param name="connection">The DataConnectionWrapper to be used for enumeration.</param>
        /// <param name="row">DataRow to extract values.</param>
        /// <returns>Returns field values for given DataRow.</returns>
        protected override Dictionary<string, string> ExtractOptions(DataConnectionWrapper connection, DataRow row)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (row == null)
                throw new ArgumentNullException("row");

            // Call to base 
            Dictionary<string, string> result = base.ExtractOptions(connection, row);
            if (result == null)
                result = new Dictionary<string, string>();

            // Extract CREATE TABLE sql
            string createTableQuery = GetCreateTableQuery(connection, row);
            if (String.IsNullOrEmpty(createTableQuery))
                return result;

            // Extract a connection field
            Parser.ExtractAdvancedFieldUnquoted(result, createTableQuery, ConnectionQueryPart, ConnectionField);

            // Extract an insert method field
            Parser.ExtractAdvancedFieldToken(result, createTableQuery, InsertMethodQueryPart, InsertMethodField);

            // Extract a union field
            Parser.ExtractAdvancedFieldUnbraced(result, createTableQuery, UnionQueryPart, UnionField);

            // Extract the DATA_DIRECTORY field
            Parser.ExtractAdvancedFieldUnquoted(result, createTableQuery, DataDirectoryQueryPart, DataDirectoryField);

            // Extract the INDEX_DIRECTORY field
            Parser.ExtractAdvancedFieldUnquoted(result, createTableQuery, IndexDirectoryQueryPart, IndexDirectoryField);

            // For legacy version calculate character set
            // Extract server version
            Version serverVersion = connection.ServerVersion;

            // For MySQL greater then 4.1.1 we can calculate character set
            if (serverVersion != null && serverVersion.Major < 5
                && serverVersion >= new Version(4, 1, 1))
            {
                // Extract collation
                string collation = DataInterpreter.GetString(row, Attributes.Collation);
                if (!String.IsNullOrEmpty(collation))
                {
                    string characterSet = connection.GetCharacterSetForCollation(collation);
                    if (!String.IsNullOrEmpty(characterSet))
                        result[CharacterSetField] = characterSet;
                }
            }
                

            // Return results
            return result;
        }

        /// <summary>
        /// Returns string with CREATE TABLE sql for this table.
        /// </summary>
        /// <param name="connection">Connection to use to execute query.</param>
        /// <param name="row">DataRow with information about table.</param>
        /// <returns>Returns string with CREATE TABLE sql for this table.</returns>
        private static string GetCreateTableQuery(DataConnectionWrapper connection, DataRow row)
        {
            // Extract schema and table name
            string schemaName = DataInterpreter.GetString(row, Attributes.Schema);
            string tableName = DataInterpreter.GetString(row, Attributes.Name);
            if (String.IsNullOrEmpty(schemaName) || String.IsNullOrEmpty(tableName))
            {
                Debug.Fail("Unable to get table or schema name");
                return String.Empty;
            }

            return GetCreateTableQuery(connection, schemaName, tableName);
        }

        /// <summary>
        /// Returns string with CREATE TABLE sql for table by gicen schema name and table name.
        /// </summary>
        /// <param name="connection">Connection to use to execute query.</param>
        /// <param name="schemaName">Name of table schema.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>Returns string with CREATE TABLE sql for this table.</returns>
        public static string GetCreateTableQuery(DataConnectionWrapper connection, string schemaName, string tableName)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (String.IsNullOrEmpty(schemaName))
                throw new ArgumentException(Resources.Error_EmptyString, "schemaName");
            if (String.IsNullOrEmpty(tableName))
                throw new ArgumentException(Resources.Error_EmptyString, "tableName");

            // Build SHOW CREATE TABLE table
            StringBuilder query = new StringBuilder();
            query.Append("SHOW CREATE TABLE ");
            QueryBuilder.WriteIdentifier(schemaName, tableName, query);

            // Execute query and check table
            IDataReader reader = connection.ExecuteReader(query.ToString(), 
                false, CommandBehavior.Default);
            if (reader == null || !reader.Read())
            {
                Debug.Fail("Failed to read CREATE TABLE query!");
                return String.Empty;
            }

            // Extract result
            string result = reader.GetString(1);
            reader.Close();

            return result;
        }
        #endregion
    }
}
