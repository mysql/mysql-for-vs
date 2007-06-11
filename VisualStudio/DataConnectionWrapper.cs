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
 * This class contains implementation of the DataConnection wrapper.
 */
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Data;
using System.Data;
using MySql.Data.VisualStudio.Properties;
using System.Globalization;
using System.Diagnostics;
using MySql.Data.VisualStudio.Utils;
using MySql.Data.VisualStudio.Descriptors;
using System.Data.SqlTypes;
using System.Data.Common;

namespace MySql.Data.VisualStudio
{
    /// <summary>
    /// This class wraps DataConnection object and exposes set of useful methods. 
    /// It supports queries execution and connection information extraction.
    /// </summary>
    public class DataConnectionWrapper
    {
        #region Initialization
        /// <summary>
        /// Constructor stores given connection into private variable
        /// </summary>
        /// <param name="connection"></param>
        public DataConnectionWrapper(DataConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            connectionRef = connection;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Returns underlying connection object
        /// </summary>
        public DataConnection Connection
        {
            get
            {
                return connectionRef;
            }
        }

        /// <summary>
        /// Returns server name for the underlying connection object.
        /// </summary>
        public string ServerName
        {
            get
            {
                // Retrieving connection
                DbConnection conn = GetConnection();

                try
                {
                    // Ensure the connection is open
                    EnsureConnectionIsOpen();

                    return conn.DataSource;
                }
                finally
                {
                    Connection.UnlockProviderObject();
                }
            }
        }

        /// <summary>
        /// Returns server version typed as Version class.
        /// </summary>
        public Version ServerVersion
        {
            get
            {
                // Retrieving connection
                DbConnection conn = GetConnection();
                
                // Temporary string to receive server version
                string versionString;

                // Extract server version from connection
                try
                {
                    // Ensure the connection is open
                    EnsureConnectionIsOpen();

                    versionString = conn.ServerVersion;
                }
                finally
                {
                    Connection.UnlockProviderObject();
                }

                return MySqlConnectionSupport.ParseVersionString(versionString);
            }
        }

        /// <summary>
        /// Returns default schema for the underlying connection.
        /// </summary>
        public string Schema
        {
            get
            {
                // Retrieving connection
                DbConnection conn = GetConnection();

                try
                {
                    // Ensure the connection is open
                    EnsureConnectionIsOpen();

                    return conn.Database;
                }
                finally
                {
                    Connection.UnlockProviderObject();
                }
            }
        }

        /// <summary>
        /// Returns default character set for underlying schema
        /// </summary>
        public string DefaultEngine
        {
            get
            {
                if (String.IsNullOrEmpty(defaultEngineVal))
                {
                    ReadTableEngines();
                }
                return defaultEngineVal;
            }
        }

        /// <summary>
        /// Returns default character set for underlying schema
        /// </summary>
        public string DefaultCharacterSet
        {
            get
            {
                if (String.IsNullOrEmpty(defaultCharacterSetVal))
                {
                    ReadDefaulCharacterSetAndCollationForSchema();
                }
                return defaultCharacterSetVal;
            }
        }

        /// <summary>
        /// Returns default collation for underlying schema
        /// </summary>
        public string DefaultCollation
        {
            get
            {
                if (String.IsNullOrEmpty(defaultCollationVal))
                {
                    ReadDefaulCharacterSetAndCollationForSchema();
                }
                return defaultCollationVal;
            }
        }

        /// <summary>
        /// Returns name of the current user
        /// </summary>
        public string CurrentUser
        {
            get
            {
                return ExecuteScalar("SELECT CURRENT_USER") as string;
            }
        }

        /// <summary>
        /// Returns connection string used for this connection in encrypted fasion.
        /// </summary>
        public string EncryptedConnectionString
        {
            get
            {
                Debug.Assert(connectionRef != null, "Connection reference is empty!");
                Debug.Assert(!String.IsNullOrEmpty(connectionRef.EncryptedConnectionString), "Connection string is empty!");
                return connectionRef.EncryptedConnectionString;
            }
        }

        /// <summary>
        /// Returns underlying ObjectChangeEvents object which can be used by to notify about
        /// object changes.
        /// </summary>
        public DataObjectChangeEvents ObjectChangeEvents
        {
            get
            {
                Debug.Assert(connectionRef != null, "Connection reference is empty!");
                Debug.Assert(connectionRef.ObjectChangeEvents != null, "ObjectChangeEvents is empty!");
                return connectionRef.ObjectChangeEvents;
            }
        }
        #endregion

        #region Query execution methods

        /// <summary>
        /// Executes single SELECT query and returns DataTable as a result.
        /// </summary>
        /// <param name="selectStatement">SELECT statement to execute.</param>
        /// <returns>DataTable object with query results.</returns>
        public DataTable ExecuteSelectTable(string selectStatement)
        {
            // Validate inputs
            if (String.IsNullOrEmpty(selectStatement))
                throw new ArgumentException(Resources.Error_EmptyString, "selectStatement");

            Debug.Assert(Connection != null, "Connection is not initialized!");

            // Retrieving connection
            DbConnection conn = GetConnection();

            try
            {
                // Ensure the connection is open
                EnsureConnectionIsOpen();

                // Create a command object            
                DbCommand comm = conn.CreateCommand();
                comm.CommandText = selectStatement;

                // Create adapter
                DbDataAdapter adapter = CreateDataAdapter();
                adapter.SelectCommand = comm;

                // Extract data
                DataTable result = new DataTable();
                result.Locale = CultureInfo.InvariantCulture;
                adapter.Fill(result);

                return result;
            }
            catch
            {
                // Try to ping connection after error to force socket recreation
                TryToPingConnection(conn);
                // Throw error for further processing
                throw;
            }
            finally
            {
                Connection.UnlockProviderObject();
            }            
        }

        public IDataReader ExecuteReader(string query, bool isProcedure, CommandBehavior behavior)
        {
            // Validate inputs
            if (String.IsNullOrEmpty(query))
                throw new ArgumentException(Resources.Error_EmptyString, "query");

            Debug.Assert(Connection != null, "Connection is not initialized!");

            // Retrieving connection
            DbConnection conn = GetConnection();

            try
            {
                // Ensure the connection is open
                EnsureConnectionIsOpen();

                // Create a command object
                DbCommand comm = conn.CreateCommand();
                comm.CommandText = query;
                if (isProcedure)
                    comm.CommandType = CommandType.StoredProcedure;

                return comm.ExecuteReader(behavior);
            }
            catch
            {
                // Try to ping connection after error to force socket recreation
                TryToPingConnection(conn);

                throw;
            }
            finally
            {
                Connection.UnlockProviderObject();
            }
        }

        /// <summary>
        /// Executes query which doesn’t return table. Usually this 
        /// query is for modifications.
        /// </summary>
        /// <param name="query">SQL text of the query to execute/</param>
        /// <returns>Scalar result of the query, if any.</returns>
        public object ExecuteScalar(string query)
        {
            // Validate inputs
            if (String.IsNullOrEmpty(query))
                throw new ArgumentException(Resources.Error_EmptyString, "query");

            Debug.Assert(Connection != null, "Connection is not initialized!");

            // Retrieving connection
            DbConnection conn = GetConnection();

            // Transaction to use
            DbTransaction transaction = null;
            try
            {
                // Ensure the connection is open
                EnsureConnectionIsOpen();

                // Start transaction
                transaction = conn.BeginTransaction();

                // Create a command object
                DbCommand comm = conn.CreateCommand();
                comm.CommandText = query;

                // Set transaction to command
                comm.Transaction = transaction;

                // Executes
                object result = comm.ExecuteScalar();

                // Commit transcation
                transaction.Commit();

                // Return resukts
                return result;
            }
            catch
            {
                // Try to ping connection after error to force socket recreation
                TryToPingConnection(conn);

                // Rollback transaction on error
                if (transaction != null)
                    transaction.Rollback();
                throw;
            }
            finally
            {
                Connection.UnlockProviderObject();
            }
        }

        /// <summary>
        /// Saves changes made to table data to the database.
        /// </summary>
        /// <param name="table">DataTable with changes.</param>
        /// <param name="selectQuery">SELECT SQL query which was used to read this table.</param>
        /// <returns>Returns true if save was successful and returns false otherwise.</returns>
        public bool UpdateTable(DataTable table, string selectQuery)
        {
            // Validate inputs
            if (table == null)
                throw new ArgumentNullException("table");
            if (String.IsNullOrEmpty(selectQuery))
                throw new ArgumentException(Resources.Error_EmptyString, "selectQuery");

            Debug.Assert(Connection != null, "Connection is not initialized!");

            // Extract changes from table
            DataTable changes = table.GetChanges();

            // If no changes detected, return true
            if (changes == null)
                return true;

            // Retrieving connection
            DbConnection conn = GetConnection();
            DbTransaction transaction = null;

            try
            {
                // Ensure the connection is open
                EnsureConnectionIsOpen();

                // Start transaction
                transaction = conn.BeginTransaction();

                // Create command object
                DbCommand comm = conn.CreateCommand();
                comm.CommandText = selectQuery;
                comm.Transaction = transaction;

                // Create a data adapter and attach it to command
                DbDataAdapter adapter = CreateDataAdapter();
                adapter.SelectCommand = comm;
                
                // Create a command builder and attach it to adapter
                DbCommandBuilder builder = CreateCommandBuilder(adapter);
                
                // Build update commands                
                // If there are deleted rows, create delete command
                DataRow[] selection = table.Select(String.Empty, String.Empty, DataViewRowState.Deleted);
                if (selection != null && selection.Length > 0)
                    adapter.DeleteCommand = builder.GetDeleteCommand();

                // If there are modified rows, create update command
                selection = table.Select(String.Empty, String.Empty, DataViewRowState.ModifiedCurrent);
                if (selection != null && selection.Length > 0)
                    adapter.UpdateCommand = builder.GetUpdateCommand();

                // If there are new rows, create insert command
                selection = table.Select(String.Empty, String.Empty, DataViewRowState.Added);
                if (selection != null && selection.Length > 0)
                    adapter.InsertCommand = builder.GetInsertCommand();

                // Attach adapter commands to transactions
                if (adapter.UpdateCommand != null)
                    adapter.UpdateCommand.Transaction = transaction;
                if (adapter.InsertCommand != null)
                    adapter.InsertCommand.Transaction = transaction;
                if (adapter.DeleteCommand != null)
                    adapter.DeleteCommand.Transaction = transaction;

                // Saves data                
                adapter.Update(changes);

                // Release resources
                builder.Dispose();
                adapter.Dispose();

                // Commit transaction
                transaction.Commit();

                // Return results
                return true;
            }
            catch
            {
                // Try to ping connection after error to force socket recreation
                TryToPingConnection(conn);

                // On any error rolback transaction if any
                if (transaction != null)
                    transaction.Rollback();
                throw;
            }
            finally
            {
                Connection.UnlockProviderObject();
            }
        }
        #endregion

        #region GetSchema access
        /// <summary>
        /// Reads schema table using conneection GetSchema method.
        /// </summary>
        /// <param name="collectionName">Name of collection to read.</param>
        /// <param name="restrictions">Restrictions to put on.</param>
        /// <returns>Returns DataTable for given collection name.</returns>
        public DataTable GetSchema(string collectionName, object[] restrictions)
        {
            Debug.Assert(Connection != null, "Connection is not initialized!");

            // Retrieving connection
            DbConnection conn = GetConnection();

            try
            {
                // Ensure the connection is open
                EnsureConnectionIsOpen();

                return SwitchNumbersToInt64(conn.GetSchema(collectionName, BuildStringRestrictions(restrictions)));
            }
            finally
            {
                Connection.UnlockProviderObject();
            }
        }

        /// <summary>
        /// Builds string restriction array from object restriction array.
        /// </summary>
        /// <param name="restrictions">Array of restrictions as objects.</param>
        /// <returns>Returns string restriction array from object restriction array.</returns>
        private static string[] BuildStringRestrictions(object[] restrictions)
        {
            string[] stringRestrictions;
            if (restrictions == null)
            {
                stringRestrictions = new string[0];
            }
            else
            {
                stringRestrictions = new string[restrictions.Length];
                for (int i = 0; i < restrictions.Length; i++)
                    if (restrictions[i] != null)
                        stringRestrictions[i] = restrictions[i].ToString();
            }
            return stringRestrictions;
        }

        /// <summary>
        /// Changes types of all numeric columns to Int64. All numeric columns should have 
        /// identical type from point of view of other system components.
        /// </summary>
        /// <param name="table">DataTable with source data.</param>
        /// <returns>
        /// Returns table with same data, but types of all numeric columns set to Int64.
        /// </returns>
        private static DataTable SwitchNumbersToInt64(DataTable table)
        {
            // If source is null, return null.
            if (table == null)
                return null;

            // Clone source schema
            DataTable clone = table.Clone();

            // Change column types to Int64
            foreach (DataColumn column in clone.Columns)
            {
                if (column.DataType == typeof(Int32)
                       || column.DataType == typeof(Int16)
                       || column.DataType == typeof(Byte))
                {
                    column.DataType = typeof(Int64);
                }
            }

            // Copy row data
            foreach (DataRow row in table.Rows)
                clone.Rows.Add(row.ItemArray);

            // Accept changes
            clone.AcceptChanges();

            // Return cloned table
            return clone;
        }
        #endregion

        #region Connection information retrieving methods
        /// <summary>
        /// Returns array with names of supported engines.
        /// </summary>
        /// <returns>Returns array with names of supported engines.</returns>
        public string[] GetEngines()
        {
            Debug.Assert(characterSetsList != null);

            // Read information if not yet availabel
            if (enginesList.Count == 0)
                ReadTableEngines();

            // Build resulting array
            string[] result = new string[enginesList.Count];
            enginesList.CopyTo(result);
            return result;
        }

        /// <summary>
        /// Returns array with names of supported character sets.
        /// </summary>
        /// <returns>Returns array with names of supported character sets.</returns>
        public string[] GetCharacterSets()
        {
            Debug.Assert(characterSetsList != null);

            // Read information if not yet availabel
            if (characterSetsList.Count == 0)
                ReadAvailabelCharacterSetsAndCollations();

            // Build resulting array
            string[] result = new string[characterSetsList.Count];
            characterSetsList.CopyTo(result);
            return result;
        }

        /// <summary>
        /// Returns array with names of supported character sets.
        /// </summary>
        /// <returns>Returns array with names of supported character sets.</returns>
        public string GetCharacterSetForCollation(string collation)
        {
            if (String.IsNullOrEmpty(collation))
                throw new ArgumentException(Resources.Error_EmptyString, "collation");

            Debug.Assert(collationsForCharacterSetDictionary != null);

            // Read information if not yet availabel
            if (collationsForCharacterSetDictionary.Count == 0)
                ReadAvailabelCharacterSetsAndCollations();

            // TODO: May be, better to create dictionary for reverse mapping.
            // Iterates through character set - collation mapping
            foreach (KeyValuePair<string, List<string>> mapping in collationsForCharacterSetDictionary)
            {
                if (String.IsNullOrEmpty(mapping.Key) || mapping.Value == null)
                    continue;

                if (mapping.Value.Contains(collation))
                    return mapping.Key;
            }

            // Failed to find character set
            Debug.Fail("Collation " + collation + "is unknown!");
            return String.Empty;
        }

        /// <summary>
        /// Returns array with names of supported collations.
        /// </summary>
        /// <returns>Returns array with names of supported collations.</returns>
        public string[] GetCollations()
        {
            Debug.Assert(collationsList != null);

            // Read information if not yet availabel
            if (collationsList.Count == 0)
                ReadAvailabelCharacterSetsAndCollations();

            // Build resulting array
            string[] result = new string[collationsList.Count];
            collationsList.CopyTo(result);
            return result;
        }

        /// <summary>
        /// Returns array with names of supported collation for character set.
        /// </summary>
        /// <param name="characterSet">Character set name to look for collations.</param>
        /// <returns>Returns array with names of supported collation for character set.</returns>
        public string[] GetCollationsForCharacterSet(string characterSet)
        {
            if (String.IsNullOrEmpty(characterSet))
                throw new ArgumentException(Resources.Error_EmptyString, "characterSet");

            Debug.Assert(collationsForCharacterSetDictionary != null);

            // Read information if not yet availabel
            if (!collationsForCharacterSetDictionary.ContainsKey(characterSet) || collationsForCharacterSetDictionary[characterSet] == null)
                ReadAvailabelCharacterSetsAndCollations();
            if (!collationsForCharacterSetDictionary.ContainsKey(characterSet) || collationsForCharacterSetDictionary[characterSet] == null)
            {
                Debug.Fail("Unknown character set '" + characterSet + "'!");
                return null;
            }

            // Build resulting array
            string[] result = new string[collationsForCharacterSetDictionary[characterSet].Count];
            collationsForCharacterSetDictionary[characterSet].CopyTo(result);
            return result;
        }

        /// <summary>
        /// Returns default collation name for the character set.
        /// </summary>
        /// <param name="characterSet">Character set name to look for collations.</param>
        /// <returns>Returns default collation name for the character set.</returns>
        public string GetDefaultCollationForCharacterSet(string characterSet)
        {
            if (String.IsNullOrEmpty(characterSet))
                throw new ArgumentException(Resources.Error_EmptyString, "characterSet");

            Debug.Assert(defaultCollationsDictionary != null);

            // Read information if not yet availabel
            if (defaultCollationsDictionary.Count == 0)
                ReadAvailabelCharacterSetsAndCollations();

            if (!defaultCollationsDictionary.ContainsKey(characterSet))
            {
                Debug.Fail("Unknown character set '" + characterSet + "'!");
                return null;
            }

            // Return founded string
            return defaultCollationsDictionary[characterSet];
        }

        /// <summary>
        /// Returns full status, including status for all table engines.
        /// </summary>
        /// <returns>Returns full status, including status for all table engines.</returns>
        public string GetFullStatus()
        {
            // Read engines list
            string[] engines = GetEngines();
            if (engines == null)
                return String.Empty;

            // Start iteration
            StringBuilder result = new StringBuilder();
            foreach (string engine in engines)
            {
                string status;
                // Check if engine supports status
                if (engine == null || !HasStatus(engine))
                    continue;
                try
                {
                    // Read engine status
                    status = ExecuteScalar(
                       String.Format("SHOW ENGINE {0} STATUS", engine)) as string;
                }
                catch
                {
                    // Can be because of insufitient rights
                    continue;
                }

                // Append status to results
                result.AppendFormat(Resources.Status_for_Engine, engine);
                result.AppendLine();
                result.Append(status);
            }

            // Return result
            return result.ToString();
        }
        #endregion

        #region Reading connection information

        #region Short names
        // Short names for fiels
        private const string CollationName = CollationDescriptor.Attributes.Name;
        private const string CharSetName = CollationDescriptor.Attributes.CharacterSetName;
        private const string IsDefault = CollationDescriptor.Attributes.IsDefault;
        #endregion

        /// <summary>
        /// Reads information about supported table engines
        /// </summary>
        private void ReadTableEngines()
        {
            // Read schema aditional information
            DataTable table = EngineDescriptor.Enumerate(this, null);

            // Exctract default character set and collation names
            if (table != null
                && table.Columns.Contains(EngineDescriptor.Attributes.Name)
                && table.Columns.Contains(EngineDescriptor.Attributes.IsSupported))
            {
                FillTableEngines(table);
            }
            else
            {
                Debug.Fail("Unable to read table engines!");
            }
        }



        /// <summary>
        /// Reads information about default character set and collation for schema.
        /// </summary>
        private void ReadDefaulCharacterSetAndCollationForSchema()
        {
            // Read schema aditional information
            DataTable table = RootDescriptor.Enumerate(this, null);

            // Exctract default character set and collation names
            if (table != null && table.Rows.Count > 0
                && table.Columns.Contains(RootDescriptor.Attributes.DefaultCharset)
                && table.Columns.Contains(RootDescriptor.Attributes.DefaultCollation))
            {
                defaultCharacterSetVal = DataInterpreter.GetString(table.Rows[0], RootDescriptor.Attributes.DefaultCharset);
                defaultCollationVal = DataInterpreter.GetString(table.Rows[0], RootDescriptor.Attributes.DefaultCollation);
            }
        }

        /// <summary>
        /// Reads information about all availabel character sets and collations.
        /// </summary>
        protected void ReadAvailabelCharacterSetsAndCollations()
        {
            // Extract data table with collations
            DataTable table = CollationDescriptor.Enumerate(this, null);
            if (table == null)
            {
                Debug.Fail("Failed to read collations!");
                return;
            }

            // Check data table
            if (!table.Columns.Contains(CollationName)
                || !table.Columns.Contains(CharSetName)
                || !table.Columns.Contains(CharSetName))
            {
                Debug.Fail("One of required collumns is missing!");
                return;
            }

            FillCharSetsAndCollations(table);
        }

        /// <summary>
        /// Extracts information about all availabel table engines from the DataTable.
        /// </summary>
        /// <param name="table">DataTable object with data</param>
        private void FillTableEngines(DataTable table)
        {
            // Iterate through all engines
            foreach (DataRow engine in table.Rows)
            {
                // Extract values
                string name = DataInterpreter.GetString(engine, EngineDescriptor.Attributes.Name);
                SqlBoolean isSupported = DataInterpreter.GetSqlBool(engine, EngineDescriptor.Attributes.IsSupported);

                // Validate name
                if (String.IsNullOrEmpty(name))
                {
                    Debug.Fail("Empty engine name!");
                    continue;
                }

                // Check if engine is not supported
                if (isSupported.IsFalse)
                    continue;

                // Replacing MRG_MyISAM by more readable MERGE
                if (DataInterpreter.CompareInvariant(name, TableDescriptor.MRG_MyISAM))
                    name = TableDescriptor.MERGE;

                // Default engine founded (not YES and not NO - DEFAULT)
                if (isSupported.IsNull)
                {
                    Debug.Assert(String.IsNullOrEmpty(defaultEngineVal), "Duplicated default engine!");
                    defaultEngineVal = name;
                }

                // Add engine to collaection
                if (!enginesList.Contains(name))
                    enginesList.Add(name);
            }
        }

        /// <summary>
        /// Extracts information about all availabel character sets and collations from 
        /// the DataTable.
        /// </summary>
        /// <param name="table">DataTable object with data</param>
        private void FillCharSetsAndCollations(DataTable table)
        {
            // Iterate through all collations
            foreach (DataRow row in table.Rows)
            {
                // Extract collation and character set name
                string collation = DataInterpreter.GetString(row, CollationName);
                string charSet = DataInterpreter.GetString(row, CharSetName);

                // Validate names
                if (String.IsNullOrEmpty(collation) || 
                    String.IsNullOrEmpty(charSet))
                {
                    Debug.Fail("Empty collation or character set name!");
                    continue;
                }

                // Add character set to list if not already there
                if (!characterSetsList.Contains(charSet))
                    characterSetsList.Add(charSet);

                // Add collation to list
                Debug.Assert(!collationsList.Contains(collation), "Dublicate collation name founded!");
                if (!collationsList.Contains(collation))
                    collationsList.Add(collation);

                // Create entry in the collationsForCharacterSet dictionary
                if (!collationsForCharacterSetDictionary.ContainsKey(charSet) || collationsForCharacterSetDictionary[charSet] == null)
                    collationsForCharacterSetDictionary[charSet] = new List<string>();
                List<string> collForCharSet = collationsForCharacterSetDictionary[charSet];

                // Add new collation for character set
                Debug.Assert(collForCharSet != null && 
                    !collForCharSet.Contains(collation), 
                    "Empty collation list or dublicate collation!");
                if (!collForCharSet.Contains(collation))
                    collForCharSet.Add(collation);

                // If collation is default, add it to the defaultCollations dictionary
                if (DataInterpreter.GetSqlBool(row, IsDefault))
                {
                    Debug.Assert(!defaultCollationsDictionary.ContainsKey(charSet), "Default collation already defined!");
                    defaultCollationsDictionary[charSet] = collation;
                }
            }
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Method, which tries to Ping specified connection
        /// </summary>
        /// <param name="connection">Inkoming parameter. Connection to Ping</param>
        /// <returns>
        /// If specified connection's type contains method Pind, 
        /// method returns it's result. If not - method returnes false
        /// </returns>
        private bool TryToPingConnection(IDbConnection connection)
        {
            // Check if we have connection
            if (connection == null)
            {
                Debug.Fail("Empty connection reference in ping method!");
                return false;
            }

            // Call connection support to ping
            return MySqlConnectionSupport.TryToPingConnection(connection);
        }

        /// <summary>
        /// Returns reference to MySqlConnection for given DataConnection object.
        /// WARNING: In successful case looks provider object! Call 
        /// UnlockProviderObject later in you code.
        /// </summary>
        /// <returns>Underlying MySqlConnection object.</returns>
        private DbConnection GetConnection()
        {
            Debug.Assert(Connection != null, "Connection is not initialized!");
            DbConnection conn = Connection.GetLockedProviderObject() as DbConnection;
            Debug.Assert(conn != null, "The underlying connection is not the correct type.");

            if (conn == null)
            {
                Connection.UnlockProviderObject();
                throw new ArgumentException(Resources.Error_InvalidConnection, "connection");
            }
            return conn;
        }

        /// <summary>
        /// Checks connection state and opens it, if necessary.
        /// </summary>
        private void EnsureConnectionIsOpen()
        {
            Debug.Assert(Connection != null, "Connection is not initialized!");
            if (Connection.State != DataConnectionState.Open)
                Connection.Open();
        }

        /// <summary>
        /// Returns true if given engine has status.
        /// </summary>
        /// <param name="engine">Engine name to check.</param>
        /// <returns>Returns true if given engine has status.</returns>
        private bool HasStatus(string engine)
        {
            foreach (string candidate in HasStatusList)
                if (DataInterpreter.CompareInvariant(engine, candidate))
                    return true;
            return false;
        }

        /// <summary>
        /// Returns DbProviderFactory used to interact with data provider
        /// </summary>
        private DbProviderFactory Factory
        {
            get
            {
                if (factoryRef != null)
                    return factoryRef;
                try
                {
                    factoryRef = DbProviderFactories.GetFactory(
                        MySqlConnectionProperties.Names.InvariantProviderName);
                    Debug.Assert(factoryRef != null, "Empty DbProviderFactory!");
                    return factoryRef;
                }
                catch
                {
                    Debug.Fail("Failed to create DbProviderFactory!");
                    throw;
                }
            }
        }

        /// <summary>
        /// Returns newly created data adapter.
        /// </summary>
        /// <returns>Returns newly created data adapter.</returns>
        private DbDataAdapter CreateDataAdapter()
        {
            if (Factory == null)
                return null;

            // Create adapter
            DbDataAdapter adapter = Factory.CreateDataAdapter();
            Debug.Assert(adapter != null, "Failed to create data adapter!");
            return adapter;
        }

        /// <summary>
        /// Returns new command builder connected to the given data adapter.
        /// </summary>
        /// <param name="adapter">Data adapter to connect.</param>
        /// <returns>Returns new command builder connected to the given data adapter.</returns>
        private DbCommandBuilder CreateCommandBuilder(DbDataAdapter adapter)
        {
            if (Factory == null)
                return null;

            // Create builder
            DbCommandBuilder builder = Factory.CreateCommandBuilder();
            Debug.Assert(builder != null, "Failed to create command builder!");

            // Connects to data adapter
            builder.DataAdapter = adapter;

            return builder;
        }
        #endregion

        #region Private constants
        /// <summary>
        /// List of engines which have status.
        /// </summary>
        private readonly string[] HasStatusList = new string[] { "InnoDB" };
        #endregion

        #region Private variables to store properties and connection information
        private string defaultCharacterSetVal;
        private string defaultCollationVal;
        private readonly List<string> characterSetsList = new List<string>();
        private readonly List<string> collationsList = new List<string>();
        private readonly Dictionary<string, List<string>> collationsForCharacterSetDictionary = new Dictionary<string, List<string>>();
        private readonly Dictionary<string, string> defaultCollationsDictionary = new Dictionary<string, string>();
        private readonly DataConnection connectionRef;
        private readonly List<string> enginesList = new List<string>();
        private string defaultEngineVal = null;
        private DbProviderFactory factoryRef;
        #endregion
    }
}
