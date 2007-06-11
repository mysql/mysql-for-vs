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
 * This file contains implementation of identifier convertor support entity.
 */

using System;
using Microsoft.VisualStudio.Data;
using Microsoft.VisualStudio.Data.AdoDotNet;
using MySql.Data.VisualStudio.Utils;

namespace MySql.Data.VisualStudio
{
	/// <summary>
	/// Identifier converter is responsible mainly for quoting and unquoting identifiers.
	/// </summary>
    public class MySqlDataObjectIdentifierConverter : AdoDotNetObjectIdentifierConverter
    {
        #region Initialization
		/// <summary>
		/// Just calls base constructor.
		/// </summary>
		/// <param name="connection">Connection to the data source object.</param>
        public MySqlDataObjectIdentifierConverter(DataConnection connection)
            : base(connection)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
        }
        #endregion

        #region Overridings
        /// <summary>
        /// Formats a specified identifier part; where the with Quotes parameter is true, 
        /// this method calls the EscapeAndQuoteIdentifier method. 
        /// </summary>
        /// <param name="typeName">The name of the data object type.</param>
        /// <param name="identifierPart">The unformatted value of an identifier part.</param>
        /// <param name="withQuotes">Indicates whether the formatted part should be enclosed in quotation marks, where necessary.</param>
        /// <returns>
        /// Returns the specified identifier part as a formatted string. 
        /// </returns>
        protected override string FormatPart(string typeName, object identifierPart, bool withQuotes)
        {
            if (typeName == null)
                throw new ArgumentNullException("typeName");
            
            // If identifier part is empty (which is true for catalog), return null
            if (String.IsNullOrEmpty(identifierPart as string))
                return null;

			// Add quotes if necessary
            if (withQuotes)
                return QueryBuilder.EscapeAndQuoteIdentifier(identifierPart as string);
            else
                return identifierPart as string;
        }
      

        /// <summary>
        /// Returns the unformatted equivalent of the formatted identifier part. 
        /// </summary>
        /// <param name="typeName">The name of a data object type.</param>
        /// <param name="identifierPart">A formatted identifier part.</param>
        /// <returns>Returns the unformatted equivalent of the formatted identifier part. </returns>
        protected override object UnformatPart(string typeName, string identifierPart)
        {
            // TODO: Implement identifier unquoting
            object result = base.UnformatPart(typeName, identifierPart);
            return result;
        }
        #endregion
    }
}
