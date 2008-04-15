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
 * This file contains implementation of the collations converter.
 */
using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.VisualStudio.Descriptors;
using System.Diagnostics;

namespace MySql.Data.VisualStudio.DocumentView
{
    /// <summary>
    /// This converter is used to read information about available for document 
    /// character set collations and return it as standard values.
    /// </summary>
    class CollationConverter: BaseValueListConverter
    {
        /// <summary>
        /// This interface must be implemented by document to read its character set.
        /// </summary>
        public interface ICharacterSetProvider
        {
            /// <summary>
            /// Returns character set name.
            /// </summary>
            string CharcterSet
            {
                get;
            }
        }
        
        /// <summary>
        /// Returns string array of supported collations for document character sets.
        /// </summary>
        /// <param name="document">Document object for which we need standard values.</param>
        /// <param name="connection">Connection to use for standard values extraction.</param>
        /// <returns>Returns string array of supported collations for document character sets.</returns>
        protected override string[] ReadStandartValues(IDocument document, DataConnectionWrapper connection)
        {
            if (document == null)
                throw new ArgumentNullException("document");            
            if (connection == null)
                throw new ArgumentNullException("connection");

            // Extract provider, if available
            ICharacterSetProvider provider = (document as ICharacterSetProvider);

            // If provider not availabel return all collations.
            if (provider == null)
                return connection.GetCollations();

            // Return collations for provider cahracter set
            return connection.GetCollationsForCharacterSet(provider.CharcterSet);            
        }
    }
}
