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
using System.Text;
using System.Windows.Forms;


namespace MySql.Data.VisualStudio
{
    public class MySqlConnectionSupport : AdoDotNetConnectionSupport
    {
        public MySqlConnectionSupport()
            : base(MySqlConnectionProperties.InvariantName) 
		{
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
                return new MySqlDataViewSupport();
            }
            else if (serviceType == typeof(DataObjectSupport))
            {
                return new MySqlDataObjectSupport();
            }
            else if (serviceType == typeof(DataSourceInformation))
            {
                return new MySqlDataSourceInformation(Site as DataConnection);
            }
			else return base.GetServiceImpl(serviceType);
        }
        
        /// <summary>
        /// Returns new instance of identifier converter
        /// </summary>
        /// <returns>Returns new instance of identifier converter</returns>
//        protected override DataObjectIdentifierConverter CreateObjectIdentifierConverter()
  //      {
    //        return new MySqlDataObjectIdentifierConverter(Site as DataConnection);
      //  }

/*        #region Version validation
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
        #endregion*/

        #region Support entities
//        private DataViewSupport viewSupport;
  //      private DataObjectSupport objectSupport;
        private MySqlDataSourceInformation sourceInformation;
        #endregion

		internal static DataTable ConvertAllBinaryColumns(DataTable dt)
		{
			// stupid hack to work around the issue that show engines returns 
			// everything as byte[]
			DataTable newDT = dt.Clone();

			foreach (DataColumn column in newDT.Columns)
				if (column.DataType == typeof(System.Byte[]))
					column.DataType = typeof(System.String);

			Encoding e = Encoding.GetEncoding("latin1");
			foreach (DataRow row in dt.Rows)
			{
				DataRow newRow = newDT.NewRow();
				foreach (DataColumn column in dt.Columns)
				{
					if (column.DataType == typeof(System.Byte[]))
						newRow[column.Ordinal] = e.GetString((byte[])row[column.Ordinal]);
					else
						newRow[column.Ordinal] = row[column.Ordinal];
				}
				newDT.Rows.Add(newRow);
			}
			return newDT;
		}
    }
}
