using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Text;

namespace MySql.Data.MySqlClient
{
	public abstract class DbConnection2 : Component, IDbConnection, IDisposable
    {


        #region Abstracts
        public abstract DbTransaction BeginTransaction(IsolationLevel il);
        public abstract void ChangeDatabase(string databaseName);
        public abstract void Open();
        public abstract void Close();
        #endregion

#region Abstract Properties
        public abstract string ConnectionString { get; set; }
        public abstract int ConnectionTimeout { get; }
        public abstract string Database { get; }
        public abstract ConnectionState State { get; }
#endregion

        #region IDbConnection Members

        IDbTransaction IDbConnection.BeginTransaction(IsolationLevel il)
        {
            return BeginTransaction(il);
        }

        IDbTransaction IDbConnection.BeginTransaction()
        {
            return BeginTransaction(IsolationLevel.Unspecified);
        }


        IDbCommand IDbConnection.CreateCommand()
        {
            return CreateCommand();
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
}
}
