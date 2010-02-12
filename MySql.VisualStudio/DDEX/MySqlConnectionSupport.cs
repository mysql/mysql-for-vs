// Copyright (c) 2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
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
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

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
        private MySqlDataSourceInformation sourceInformation = null;

        public MySqlConnectionSupport()
            : base(MySqlConnectionProperties.InvariantName) 
		{
		} 

        /// <summary>
        /// Retrieves a service of the specified type. Following services are 
        /// supported:
        /// DataViewSupport � information about view schema.
        /// DataObjectSupport � information about object model.
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

            // TODO: check server version compatibility

            // Rreturn true if everything is ok
            if (sourceInformation != null)
                sourceInformation.Refresh();
            return true;
        }
    }
}
