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

namespace MySql.Data.MySqlClient
{
	public abstract class DbParameter : MarshalByRefObject, IDbDataParameter, IDataParameter
	{
		protected DbParameter()
		{
		}

		public abstract void ResetDbType();

        #region IDbDataParameter Members

        byte IDbDataParameter.Precision
        {
			get { return 0; }
			set { }
        }

        byte IDbDataParameter.Scale
        {
			get { return 0; }
			set { }
        }

		public abstract int Size { get; set; }

        #endregion

        #region IDataParameter Members

		public abstract DbType DbType { get; set; }
		public abstract ParameterDirection Direction { get; set; }
		public abstract bool IsNullable { get; set; }
		public abstract string ParameterName { get; set; }
		public abstract string SourceColumn { get; set; }
		public abstract bool SourceColumnNullMapping { get; set; }
		public abstract DataRowVersion SourceVersion { get; set; }
		public abstract object Value { get; set; }

        #endregion
	}
}
