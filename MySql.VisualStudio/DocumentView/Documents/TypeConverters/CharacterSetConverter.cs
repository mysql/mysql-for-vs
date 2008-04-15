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
 * This class contains implementation of the character set name 
 * standard values extractor.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using MySql.Data.VisualStudio.Properties;
using System.Diagnostics;
using System.Data;
using MySql.Data.VisualStudio.Utils;
using MySql.Data.VisualStudio.Descriptors;

namespace MySql.Data.VisualStudio.DocumentView
{
    /// <summary>
    /// Used to extract standard values for character set. Enumerates all available 
    /// character set and extract their names.
    /// </summary>
    class CharacterSetConverter: BaseValueListConverter
    {
        /// <summary>
        /// Returns string array of supported character sets.
        /// </summary>
        /// <param name="document">Document object for which we need standard values.</param>
        /// <param name="connection">Connection to use for standard values extraction.</param>
        /// <returns>Returns string array of supported character sets.</returns>
        protected override string[] ReadStandartValues(IDocument document, DataConnectionWrapper connection)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            return connection.GetCharacterSets();
        }
    }
}
