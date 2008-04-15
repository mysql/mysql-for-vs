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
using System.Text;

namespace MySql.Data.MySqlClient
{
    public abstract class DbTransaction : MarshalByRefObject, IDbTransaction, IDisposable 
	{
        protected DbTransaction()
        {
        }

        #region IDbTransaction Members


        IDbConnection IDbTransaction.Connection
        {
            get { return this.DbConnection; }
        }

		public DbConnection Connection 
		{
			get { return this.DbConnection; }
		}

        public abstract IsolationLevel IsolationLevel { get; }
        protected abstract DbConnection DbConnection { get; }
        public abstract void Commit();
        public abstract void Rollback();

        #endregion

        #region IDisposable Members

		public void Dispose()
		{
			this.Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
		}

        #endregion

    }
}
