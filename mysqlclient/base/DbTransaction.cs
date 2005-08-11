using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Text;

namespace MySql.Data.MySqlClient
{
    public abstract class DbTransaction : MarshalByRefObject, IDbTransaction, IDisposable 
	{
        protected DbTransaction2()
        {
        }

        #region IDbTransaction Members


        IDbConnection IDbTransaction.Connection
        {
            get { return this.Connection; }
        }

        public abstract IsolationLevel IsolationLevel { get; }
        protected abstract DbConnection DbConnection { get; }

        public abstract void Commit();
        public abstract void Rollback();

        #endregion

        #region IDisposable Members

        public abstract void Dispose();

        #endregion

    }
}
