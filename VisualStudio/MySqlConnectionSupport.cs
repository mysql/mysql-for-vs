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
 * This file contains implementation of custom connection support for MySQL.
 */

using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Reflection;
using Microsoft.VisualStudio.Data;
using Microsoft.VisualStudio.Data.AdoDotNet;
using System.Globalization;
using MySql.Data.VisualStudio.Properties;


namespace MySql.Data.VisualStudio
{
    /// <summary>
    /// Represents a custom data connection support implementation for MySQL.
    /// MySQL Connector/NET 1.0 is used as underlying ADO.NET data provider.
    /// </summary>
    public class MySqlConnectionSupport : AdoDotNetConnectionSupport
    {
        #region Initialization
        /// <summary>
        /// Simple constructor
        /// </summary>
        public MySqlConnectionSupport()
            : base(MySqlConnectionProperties.Names.InvariantProviderName) { } 
        #endregion

        #region Overridden methods
        /// <summary>
        /// Closes the specified connection.
        /// </summary>
        public override void Close()
        {
            IDbConnection providerObjectVal = ProviderObject as IDbConnection;
            Debug.Assert(providerObjectVal != null, "Provider object is not initialized!");

            // check connection, if it's not valid - recreate it
            if (!TryToPingConnection(providerObjectVal))
            {
                // connection is brooken and we should kill this connection
                providerObjectVal.Dispose();
                Initialize(null);
            }
            else
            {
                // connection is OK, just close it normaly
                providerObjectVal.Close();
            }
        }

        /// <summary>
        /// Opens the specified connection.  
        /// </summary>
        /// <param name="doPromptCheck">
        /// Indicates whether the call to the Open method should return false 
        /// for specific errors that relate to missing connection information, 
        /// as opposed to simply throwing an error in all cases of failure. 
        /// Data providers that do not implement a prompt dialog (or have 
        /// their own prompting mechanism) should ignore this parameter, 
        /// and always assume a value of false.
        /// </param>
        /// <returns>
        /// Returns true if the connection was opened successfully and does 
        /// not require a prompt. Returns false if the connection is missing 
        /// required connection information and a prompt should be displayed 
        /// to obtain the missing information from the user. You should return 
        /// this value only when a data provider has also implemented the 
        /// DataConnectionPromptDialog class.
        /// </returns>
        public override bool Open(bool doPromptCheck)
        {
            // Open connection
            try
            {
                // Call base method first
                if (!base.Open(doPromptCheck))
                    return false;
            }
            catch (DbException)
            {
                // If can't prompt user for new authentication data, re-throw exception
                if (!doPromptCheck)
                    throw;

                // Else return false to display prompt dialog
                return false;
            }            

            // Extract provider object (withou lock, because nobody knows about it yet)
            DbConnection providerObjectVal = ProviderObject as DbConnection;            
            if (providerObjectVal == null)
            {
                Debug.Fail("Provider object is not initialized!");
                return false;
            }

            try
            {
                // Validate server version (may throw an exception)
                ValidateVersion(providerObjectVal);
            }
            catch
            {
                // Version is invalid. close the connection
                providerObjectVal.Close();
                // Re-throw exception for upper code
                throw;
            }

            // Rreturn true if everything is ok
            return true;
        }

        /// <summary>
        /// Retrieves a service of the specified type. Following services are 
        /// supported:
        /// DataViewSupport – information about view schema.
        /// DataObjectSupport – information about object model.
        /// </summary>
        /// <param name="serviceType">A service type.</param>
        /// <returns>
        /// Returns the service of the specified type, or returns a null reference 
        /// if no service was found. 
        /// </returns>
        protected override object GetServiceImpl(Type serviceType)
        {
            if (serviceType == typeof(DataViewSupport))
            {
                if (viewSupport == null)
                {
                    viewSupport = new MySqlDataViewSupport();
                }
                return viewSupport;
            }
            if (serviceType == typeof(DataObjectSupport))
            {
                if (objectSupport == null)
                {
                    objectSupport = new MySqlDataObjectSupport();
                }
                return objectSupport;
            }
            if (serviceType == typeof(DataSourceInformation))
            {
                if (sourceInformation == null)
                {
                    sourceInformation = new MySqlDataSourceInformation(Site as DataConnection);
                }
                return sourceInformation;
            }
            object result = base.GetServiceImpl(serviceType);
            return result;
        }
        
        /// <summary>
        /// Returns new instance of identifier converter
        /// </summary>
        /// <returns>Returns new instance of identifier converter</returns>
        protected override DataObjectIdentifierConverter CreateObjectIdentifierConverter()
        {
            return new MySqlDataObjectIdentifierConverter(Site as DataConnection);
        }
        #endregion

        #region Version validation
        /// <summary>
        /// Validates MySQL server version and throws an exception if version is not compatible.
        /// </summary>
        /// <param name="connection">Connection object to check version.</param>
        public static void ValidateVersion(DbConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            // Open connection object
            if (connection.State != ConnectionState.Open)
                connection.Open();

            // Parse version
            Version version = ParseVersionString(connection.ServerVersion);

            // Check server version (currently only 4.1 is supported)
            if (version != null && version < new Version(4,1))
                throw new NotSupportedException(String.Format(
                            CultureInfo.CurrentCulture,
                            Resources.Error_UnsupportedServerVersion,                            
                            connection.ServerVersion));
        }

        /// <summary>
        /// Returns Version object for given MySQL server version string.
        /// </summary>
        /// <param name="versionString">MySQL server version string to parse.</param>
        /// <returns>Returns Version object for given MySQL server version string.</returns>
        public static Version ParseVersionString(string versionString)
        {
            // Check if version is extracted
            if (String.IsNullOrEmpty(versionString))
            {
                Debug.Fail("Failed to read server verson!");
                return null;
            }

            // Extract numeric part
            int pos = versionString.IndexOf('-');
            if (pos < 0)
            {
                Debug.Fail("Failed to extract numeric part for server version!");
                return null;
            }
            string numeric = versionString.Substring(0, pos);

            // Parse numeric part
            try
            {
                return new Version(numeric);
            }
            catch (Exception e)
            {
                // If something wrong, return null
                Debug.Fail("Failed to parse server version numeric part!", e.Message);
                return null;
            }
        }
        #endregion

        #region Connection pinging
        /// <summary>
        /// Method, which tries to Ping specified connection
        /// </summary>
        /// <param name="connection">Inkoming parameter. Connection to Ping</param>
        /// <returns>
        /// If specified connection's type contains method Pind, 
        /// method returns it's result. If not - method returnes false
        /// </returns>
        public static bool TryToPingConnection(IDbConnection connection)
        {
            // Check if we have connection
            if (connection == null)
                throw new ArgumentNullException("connection");

            // We try to get method Ping from connection type using reflection
            Type connectionType = connection.GetType();
            Debug.Assert(connectionType != null, "Failed to get connection type information!");

            // Get method Ping without any parameters
            MethodInfo method = connectionType.GetMethod("Ping", new Type[0]);
            if (method == null)
            {
                Debug.Fail("Connection hasn't Ping method!");
                return false;
            }

            // Invoke method using reflection
            object methodInvokingResult;
            try
            {
                methodInvokingResult = method.Invoke(connection, null);
            }
            catch (Exception e)
            {
                Debug.Fail("Failed to execute Ping method!", e.ToString());
                return false;
            }

            // Check result type and cast it if possible
            if (methodInvokingResult is bool)
                return (bool)methodInvokingResult;

            Debug.Fail("Invalid result type returned by Ping method!");
            // Return false if result type is invalid
            return false;
        }
        #endregion

        #region Support entities
        private DataViewSupport viewSupport;
        private DataObjectSupport objectSupport;
        private DataSourceInformation sourceInformation;
        #endregion
    }
}
