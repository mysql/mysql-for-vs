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
    public abstract class DbCommand : Component, IDbCommand, IDisposable
    {
        #region Abstract Properties 

		//public abstract UpdateRowSource UpdatedRowSource { get; set; }
		protected abstract DbConnection DbConnection { get; set; }
        protected abstract DbTransaction DbTransaction { get; set; }
		protected abstract DbParameterCollection DbParameterCollection { get; }
		//public abstract UpdateRowSource UpdatedRowSource { get; set; }

		#endregion

        #region Abstract Methods

        protected abstract DbDataReader ExecuteDbDataReader(CommandBehavior behavior);
        protected abstract DbParameter CreateDbParameter();
		public abstract int ExecuteNonQuery();
		public abstract object ExecuteScalar();
		public abstract void Prepare();

        #endregion

		public abstract void Cancel();
		public abstract string CommandText { get; set; }
		public abstract int CommandTimeout { get; set; }
		public abstract CommandType CommandType { get; set; }

		#region IDbCommand Members

        IDbConnection IDbCommand.Connection
        {
            get { return this.DbConnection; }
            set { this.DbConnection = (DbConnection)value; }
        }

		IDbTransaction IDbCommand.Transaction
		{
			get { return this.DbTransaction; }
			set { this.DbTransaction = (DbTransaction)value; }
		}

		IDataParameterCollection IDbCommand.Parameters
		{
			get { return this.DbParameterCollection; }
		}

		IDbDataParameter IDbCommand.CreateParameter()
		{
			return this.CreateParameter();
		}

		IDataReader IDbCommand.ExecuteReader()
		{
			return this.ExecuteDbDataReader(CommandBehavior.Default);
		}

		IDataReader IDbCommand.ExecuteReader(CommandBehavior behavior)
		{
			return this.ExecuteDbDataReader(behavior);
		}

		#endregion

		#region Properties

		[Browsable(false)]
		public DbConnection Connection 
		{
			get { return this.DbConnection; }
			set { this.DbConnection = (DbConnection)value; }
		}

		[Browsable(false)]
		public DbTransaction Transaction 
		{
			get { return this.DbTransaction; }
			set { this.DbTransaction = (DbTransaction)value; }
		}

		[Browsable(false)]
		public DbParameterCollection Parameters 
		{
			get { return this.DbParameterCollection; }
		}

		#endregion

		UpdateRowSource IDbCommand.UpdatedRowSource
		{
			get { return UpdateRowSource.Both; }
			set { }
		}


        public DbDataReader ExecuteReader(CommandBehavior behavior)
        {
            return this.ExecuteDbDataReader(behavior);
        }

		public DbDataReader ExecuteReader()
		{
			return this.ExecuteDbDataReader(CommandBehavior);
		}

	}
}
