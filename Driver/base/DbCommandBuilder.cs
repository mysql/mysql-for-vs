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
using System.Data.Common;
using System.ComponentModel;

namespace MySql.Data.MySqlClient
{
	public abstract class DbCommandBuilder : Component
	{
		protected DbCommandBuilder() 
		{
		}

		protected abstract string GetParameterName(int ordinal);
		protected abstract string GetParameterName(string parameterName);
		protected abstract string GetParameterPlaceholder(int ordinal);
		protected abstract void ApplyParameterInfo(DbParameter parameter, DataRow row, 
			StatementType statementType, bool whereClause);
		protected abstract void SetRowUpdatingHandler(DbDataAdapter adapter);
	}
}
