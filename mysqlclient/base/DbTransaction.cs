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
