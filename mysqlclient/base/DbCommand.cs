using System;
using System.Data;
using System.Data.Common;
using System.ComponentModel;

namespace MySql.Data.MySqlClient
{
    public abstract class DbCommand2 : Component, IDbCommand, IDisposable
    {
        #region Abstract Properties
        public abstract DbConnection DbConnection { get; set; }
        public abstract DbTransaction DbTransaction { get; set; }
        #endregion

        #region Abstract Methods
        public abstract DbDataReader ExecuteDbDataReader(CommandBehavior behavior);
        public abstract DbParameter CreateParameter();
        #endregion

        #region IDbCommand Members

        public void Cancel()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string CommandText
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public int CommandTimeout
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public CommandType CommandType
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        IDbConnection IDbCommand.Connection
        {
            get { return DbConnection; }
            set { DbConnection = value; }
        }


        public IDbConnection Connection
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        IDbDataParameter IDbCommand.CreateParameter()
        {
            return this.CreateParameter();
        }


        public int ExecuteNonQuery()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            return this.ExecuteDbDataReader(behavior);
        }

        IDataReader IDbCommand.ExecuteReader()
        {
            return ExecuteReader();
        }

        public IDataReader ExecuteReader()
        {
            return this.ExecuteDbDataReader(CommandBehavior);
        }

        public object ExecuteScalar()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        IDataParameterCollection IDbCommand.Parameters
        {
            get { return DbParameterCollection; }
        }


        public void Prepare()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        IDbTransaction IDbCommand.Transaction
        {
            get { return Transaction; }
            set { Transaction = (MySqlTransaction)value; }
        }


        public IDbTransaction Transaction
        {
            get { return DbTransaction; }
            set { DbTransaction = Value; }
        }

        public UpdateRowSource UpdatedRowSource
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        #endregion
}
}
