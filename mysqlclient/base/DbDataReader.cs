using System;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace MySql.Data.MySqlClient
{
	public abstract class DbDataReader : MarshalByRefObject, IDataReader, IDisposable, IDataRecord, IEnumerable
	{
		protected DbDataReader()
		{
		}

		#region Abstract Methods

		public abstract void Close();
		public abstract bool GetBoolean(int index);
		public abstract byte GetByte(int i);
		public abstract long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length);
		public abstract char GetChar(int i);
		public abstract long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length);

		public abstract string GetDataTypeName(int i);
		public abstract DateTime GetDateTime(int i);

		public abstract decimal GetDecimal(int i);
		public abstract double GetDouble(int i);

		public abstract Type GetFieldType(int i);
		public abstract float GetFloat(int i);
		public abstract Guid GetGuid(int i);
		public abstract short GetInt16(int i);
		public abstract int GetInt32(int i);
		public abstract long GetInt64(int i);
		public abstract string GetName(int i);
		public abstract int GetOrdinal(string name);
		public abstract DataTable GetSchemaTable();
		public abstract string GetString(int i);
		public abstract object GetValue(int i);
		public abstract int GetValues(object[] values);
		public abstract bool IsDBNull(int i);
		public abstract bool NextResult();
		public abstract bool Read();
		#endregion

		#region Abstract Properties

		public abstract int Depth { get; }
		public abstract int FieldCount { get; }
		public abstract bool HasRows { get; }
		public abstract bool IsClosed { get; }
		public abstract object this[string name] { get; }
		public abstract object this[int i] { get; }
		public abstract int RecordsAffected { get; }
		//TODO: check viability of VisibleFieldCount

		#endregion

        #region IDisposable Members

        public void Dispose()
        {
            throw new Exception("The method or operation is not implemented.");
        }

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
				this.Close();
		}

        #endregion

		IDataReader IDataRecord.GetData(int i)
		{
			throw new NotSupportedException("GetData not supported.");
		}

        public DbDataReader GetData(int i)
        {
			throw new NotSupportedException("GetData not supported.");
		}

        #region IEnumerable Members

        public abstract IEnumerator GetEnumerator();

        #endregion
	}
}
