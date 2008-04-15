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
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;

namespace MySql.Data.MySqlClient
{
	public abstract class DbConnection : Component, IDbConnection, IDisposable
	{
		public event StateChangeEventHandler StateChange;

		#region Abstracts

		protected abstract DbTransaction BeginDbTransaction(IsolationLevel il);
		protected abstract DbCommand CreateDbCommand();
		public abstract void ChangeDatabase(string databaseName);
		public abstract void Open();
		public abstract void Close();

		#endregion

		#region Abstract Properties

		public abstract string ConnectionString { get; set; }
		public abstract string Database { get; }
		public abstract string DataSource { get; }
		public abstract string ServerVersion { get; }
		public abstract ConnectionState State { get; }

		#endregion

		#region IDbConnection Members

		IDbTransaction IDbConnection.BeginTransaction(IsolationLevel il)
		{
			return BeginDbTransaction(il);
		}

		IDbTransaction IDbConnection.BeginTransaction()
		{
			return BeginDbTransaction(IsolationLevel.Unspecified);
		}

		IDbCommand IDbConnection.CreateCommand()
		{
			return CreateDbCommand();
		}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Releases the resources used by the MySqlConnection.
		/// </summary>
		void System.IDisposable.Dispose()
		{
			Dispose(true);
		}


		#endregion

		public virtual int ConnectionTimeout 
		{ 
			get { return 30; }
		}

		public DbTransaction BeginTransaction()
		{
			return this.BeginDbTransaction(IsolationLevel.Unspecified);
		}

		public DbTransaction BeginTransaction(IsolationLevel level)
		{
			return this.BeginDbTransaction(level);
		}

		public DbCommand CreateCommand()
		{
			return this.CreateDbCommand();
		}

	}
}
