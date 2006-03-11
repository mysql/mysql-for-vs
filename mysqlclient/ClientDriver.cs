// Copyright (C) 2004 MySQL AB
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
using System.IO;
using System.Runtime.InteropServices;
using MySql.Data.Types;

namespace MySql.Data.MySqlClient
{
#if !CF
	/// <summary>
	/// Summary description for ClientDriver.
	/// </summary>
	internal class ClientDriver : Driver 
	{
		private IntPtr	mysql;
		private IntPtr	resultSet;
		private IntPtr	currentRow;
		private uint[]	currentLengths;
		private	int		resultsCount;

		public ClientDriver(MySqlConnectionString settings) : base(settings)
		{
			resultSet = IntPtr.Zero;
			resultsCount = 0;
		}

		#region Properties

		public override bool SupportsBatch
		{
			get	{ return true;	}
		}

//		public override bool HasMoreResults
//		{
//			get	{ return fieldCount > 0; }
//		}


		#endregion

		public override void Open()
		{
			base.Open ();

			mysql = Init(mysql);

			ClientFlags flags = ClientFlags.FOUND_ROWS | ClientFlags.MULTI_RESULTS | 
				ClientFlags.MULTI_STATEMENTS | ClientFlags.LOCAL_FILES;
			if (connectionString.UseCompression)
				flags |= ClientFlags.COMPRESS;
			if (connectionString.UseSSL)
				flags |= ClientFlags.SSL;

			object timeout = connectionString.ConnectionTimeout;
			SetOptions(mysql, ClientAPIOption.MYSQL_OPT_CONNECT_TIMEOUT, 
				ref timeout);

			//TODO: support charset, shared memory, named pipes

			IntPtr result = Connect(mysql, connectionString.Server, connectionString.UserId,
				connectionString.Password, connectionString.Database, connectionString.Port, 
				null, (uint)flags);
			if (result == IntPtr.Zero)
			{
				throw new MySqlException(ErrorMsg(mysql), ErrorNumber(mysql));
			}

			version = MySql.Data.Common.DBVersion.Parse(VersionString(mysql));
			serverCharSet = CharacterSetName(mysql);
		}

		public override void Close()
		{
			Close(mysql);
			mysql = IntPtr.Zero;

			base.Close ();
		}

		public override bool Ping()
		{
			int val = Ping(mysql);
			if (val == 0) return true;
			isOpen = false;
			return false;
		}

		public override void Query(byte[] bytes, int length)
		{
			int result = Query(mysql, bytes, (uint)length);
			if (result != 0)
				throw new MySqlException(ErrorMsg(mysql), ErrorNumber(mysql));

			resultsCount = 0;
		}

		public override void SetDatabase(string dbName)
		{
			int result = SelectDatabase(mysql, dbName);
			if (result == 0) return;

			MySqlException e = new MySqlException(
				ErrorMsg(mysql), ErrorNumber(mysql));
			Logger.LogException(e);
			throw e;
		}

		public override void Reset()
		{

		}


		public override int PrepareStatement(string sql, ref MySqlField[] parameters)
		{
            IntPtr mysql_statement = StatementInit(mysql);
            if (mysql_statement == IntPtr.Zero)
                throw new MySqlException("Error initializing prepared statement");
            int result = StatementPrepare(mysql_statement, sql, (uint)sql.Length);
            if (result != 0)
                throw new MySqlException(StatementError(mysql_statement), result);
            return (int)mysql_statement;
		}

		public override long ReadResult(ref ulong affectedRows, ref long lastInsertId)
		{
			if (resultSet != IntPtr.Zero) 
			{
				FreeResult(resultSet);
				resultSet = IntPtr.Zero;
				if (! version.isAtLeast(4,1,0)) return -1;
			}

			if (version.isAtLeast(4,1,0)) 
			{
				if (resultsCount > 0) 
				{
					int result = NextResult(mysql);
					if (result == -1) return -1;
					if (result > 0)
						throw new MySqlException( ErrorMsg(mysql), ErrorNumber(mysql));
				}
			}
			else 
			{
				if (resultsCount > 0) return 0;
			}

			long numfields = GetFieldCount(mysql);
			if (numfields > 0) 
			{
				// now we use the resultset
				resultSet = UseResult(mysql);
				if (resultSet == IntPtr.Zero)
					throw new MySqlException(ErrorMsg(mysql), ErrorNumber(mysql));
			}

			if (numfields == 0) 
			{
				affectedRows = AffectedRows(mysql);
				lastInsertId = (long)LastInsertId(mysql);
			}
			resultsCount++;
			return numfields;
		}

		public override void SkipColumnValue(MySql.Data.Types.IMySqlValue valObject)
		{
			
		}

		public override IMySqlValue ReadColumnValue(int index, MySqlField field, IMySqlValue value)
		{
			int dataPtr = (int)currentRow;
			for (int i=0; i < index; i++)
				dataPtr += 4;

			IntPtr fieldPtr = Marshal.ReadIntPtr((IntPtr)dataPtr);
			MySqlStreamReader reader = null;
			if (fieldPtr != IntPtr.Zero) 
			{
				byte[] buf = new byte[currentLengths[index]];
				Marshal.Copy(fieldPtr, buf, 0, buf.Length);
				reader = new MySqlStreamReader(new MemoryStream(buf), encoding);
			}
			
			return value.ReadValue(reader, currentLengths[index], fieldPtr == IntPtr.Zero); 
		}

		public override MySqlField[] ReadColumnMetadata(int count)
		{
			MySqlField[] fields = new MySqlField[count];

			for (int i=0; i < count; i++)
			{
				ClientField fieldDef = FetchField(resultSet);
				fields[i] = new MySqlField(this.Version);
				fields[i].CatalogName = fieldDef.catalog;
				fields[i].ColumnName = fieldDef.name;
				fields[i].DatabaseName = fieldDef.db;
				fields[i].OriginalColumnName = fieldDef.org_name;
				fields[i].RealTableName = fieldDef.org_table;
				fields[i].Type = fieldDef.type;
				fields[i].Flags = (ColumnFlags)fieldDef.flags;
				fields[i].ColumnLength = (int)fieldDef.length;
				fields[i].Precision = (byte)fieldDef.decimals;
				if (charSets != null && charSets.Count > 0) 
				{
					string charSetName = (string)charSets[(int)fieldDef.charset];
					fields[i].Encoding = CharSetMap.GetEncoding(version, charSetName);
				}
			}

			currentLengths = new uint[count];
            return fields;
		}

//        public override bool ReadDataRow(int statementId, MySqlField[] fields, bool seq)
  //      {
    //        return false;
      //  }

        public override bool SkipDataRow()
        {
            return false;
        }

        public override bool FetchDataRow(int statementId, int pageSize, int columns)
        {
			currentRow = FetchRow(resultSet);
			if (currentRow == IntPtr.Zero) 
			{
				int err = ErrorNumber(mysql);
				if (err == 0) 
				{
					//FreeResult(resultSet);
					resultSet = IntPtr.Zero;
					return false;
				}
				throw new MySqlException(ErrorMsg(mysql), err);
			}

			IntPtr lengths = FetchLengths(resultSet);
			for (int i=0; i < columns; i++)
			{
				currentLengths[i] = (uint)Marshal.ReadInt32(lengths);
				lengths = (IntPtr)((int)lengths + 4);
			}
			
			return true;
		}

		public override void ExecuteStatement(byte[] bytes)
		{

            //TODO
		}

		#region Interface methods

		protected virtual IntPtr Init(IntPtr mysql) 
		{
			return ClientAPI.Init(mysql);
		}

		protected virtual IntPtr Connect(IntPtr mysql, string host, string user,
			string password, string db, uint port, string unix_socket, uint flag)
		{
			return ClientAPI.Connect(mysql, host, user, password, db, port, unix_socket, flag);
		}

		protected virtual int SetOptions(IntPtr mysql, ClientAPIOption option, ref object optionValue)
		{
			return ClientAPI.SetOptions(mysql, option, ref optionValue);
		}

		protected virtual void Close(IntPtr mysql) 
		{
			ClientAPI.Close(mysql);
		}

		protected virtual int SelectDatabase(IntPtr mysql, string dbName) 
		{
			return ClientAPI.SelectDatabase(mysql, dbName);
		}

		protected virtual int Query(IntPtr mysql, byte[] query, uint len) 
		{
			return ClientAPI.Query(mysql, query, len);
		}

		protected virtual string ErrorMsg(IntPtr mysql)
		{
			return ClientAPI.ErrorMsg(mysql);
		}

		protected virtual int ErrorNumber(IntPtr mysql) 
		{
			return ClientAPI.ErrorNumber(mysql);
		}

		protected virtual IntPtr UseResult(IntPtr mysql)
		{
			return ClientAPI.UseResult(mysql);
		}

		protected virtual bool MoreResults(IntPtr mysql)
		{
			return ClientAPI.MoreResults(mysql);
		}

		protected virtual int NextResult(IntPtr mysql)
		{
			return ClientAPI.NextResult(mysql);
		}

		protected virtual void FreeResult(IntPtr resultSet) 
		{
			ClientAPI.FreeResult(resultSet);
		}

		protected virtual int GetFieldCount(IntPtr resultSet) 
		{
			return ClientAPI.FieldCount(resultSet);
		}

		protected virtual ulong AffectedRows(IntPtr mysql)
		{
			return ClientAPI.AffectedRows(mysql);
		}

		protected virtual ulong LastInsertId(IntPtr mysql)
		{
			return ClientAPI.LastInsertId(mysql);
		}

		protected virtual ClientField FetchField(IntPtr resultSet)
		{
			return ClientAPI.FetchField(resultSet);
		}

		protected virtual IntPtr FetchRow(IntPtr resultSet)
		{
			return ClientAPI.FetchRow(resultSet);
		}
		
		protected virtual IntPtr FetchLengths(IntPtr resultSet)
		{
			return ClientAPI.FetchLengths(resultSet);
		}

		protected virtual int Ping(IntPtr mysql)
		{
			return ClientAPI.Ping(mysql);
		}

        protected virtual IntPtr StatementInit(IntPtr mysql)
        {
            return ClientAPI.StatementInit(mysql);
        }

        protected virtual int StatementPrepare(IntPtr mysql_statement, string query, uint length)
        {
            return ClientAPI.StatementPrepare(mysql_statement, query, length);
        }

        protected virtual bool StatementClose(IntPtr mysql_statement)
        {
            return ClientAPI.StatementClose(mysql_statement);
        }

        protected virtual string StatementError(IntPtr mysql_statement)
        {
            return ClientAPI.StatementError(mysql_statement);
        }

		protected virtual string VersionString(IntPtr mysql)
		{
			return ClientAPI.VersionString(mysql);
		}

		protected virtual string CharacterSetName(IntPtr mysql)
		{
			return ClientAPI.CharacterSetName(mysql);
		}

		#endregion

	}
#endif
}
