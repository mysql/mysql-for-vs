// Copyright (C) 2004 MySQL AB
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
using System.ComponentModel.Design;
using MySql.Data.MySqlClient;

namespace MySql.Data.Common
{
	/// <summary>
	/// Summary description for DBParametersEditor.
	/// </summary>
	internal class DBParametersEditor : CollectionEditor
	{
		public DBParametersEditor(Type t) : base(t)
		{
		}

		protected override object CreateInstance(Type itemType)
		{
			object[] items = base.GetItems(null);

			int i = 1;
			while (true) 
			{
				bool found = false;
				foreach (object obj in items) 
				{
					MySqlParameter p = (MySqlParameter)obj;
					if (p.ParameterName.Equals( "parameter" + i )) 
					{
						found = true;
						break;
					}
				}
				if (! found) break;
				i ++;
			}

			MySqlParameter parm = new MySqlParameter("parameter"+i, MySqlDbType.VarChar);
			return parm;
		}

	}
}
