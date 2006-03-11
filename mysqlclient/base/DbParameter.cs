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
