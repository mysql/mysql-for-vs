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
 * This file contains data view support entity implementation.
 */

using System;
using System.IO;
using System.Globalization;
using Microsoft.VisualStudio.Data;

namespace MySql.Data.VisualStudio
{
	/// <summary>
	/// Represents an implementation of data view support that returns
	/// the stream of XML containing the data view support elements.
	/// </summary>
	internal class MySqlDataViewSupport : DataViewSupport
	{
        /// <summary>
        /// Constructor just passes reference to XML to base constructor.
        /// </summary>
        public MySqlDataViewSupport()
			: base("MySql.Data.VisualStudio.MySqlDataViewSupport", typeof(MySqlDataViewSupport).Assembly)
		{
		}
	}
}
