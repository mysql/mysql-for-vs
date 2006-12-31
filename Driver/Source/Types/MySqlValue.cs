// Copyright (C) 2004-2006 MySQL AB
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 2 as published by
// the Free Software Foundation
//
// There are special exceptions to the terms and conditions of the GPL 
// as it is applied to this software. View the full text of the 
// exception in file EXCEPTIONS in the directory of this software 
// distribution.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Data;
using System.Globalization;
using MySql.Data.MySqlClient;

namespace MySql.Data.Types
{
	internal interface IMySqlValue
	{
		bool IsNull { get; }
		MySqlDbType MySqlDbType { get; }
		DbType DbType { get; }
		object Value { get; /*set;*/ }
		Type SystemType { get; }
		string MySqlTypeName { get; }

		void WriteValue(MySqlStream stream, bool binary, object value, int length);
		IMySqlValue ReadValue(MySqlStream stream, long length, bool isNull);
		void SkipValue(MySqlStream stream);
	}
}
