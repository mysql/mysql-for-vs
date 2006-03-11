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
