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
 * This file contains data object support entity implementation.
 */

using Microsoft.VisualStudio.Data;

namespace MySql.Data.VisualStudio
{
	/// <summary>
	/// Represents an implementation of data object support that returns
	/// the stream of XML containing the data object support elements.
	/// </summary>
	internal class MySqlDataObjectSupport : DataObjectSupport
	{
		/// <summary>
        /// Constructor just passes reference to XML to base constructor.
		/// </summary>
        public MySqlDataObjectSupport()
			: base("MySql.Data.VisualStudio.DDEX.MySqlDataObjectSupport", typeof(MySqlDataObjectSupport).Assembly)
		{
		}
	}
}
