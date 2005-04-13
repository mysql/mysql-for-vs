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
		private int		fieldCount;
		private IntPtr	currentRow;
		private uint[]	currentLengths;

		public ClientDriver(MySqlConnectionString settings) : base(settings)
		{
			fieldCount = -1;
		}

		#region Properties

		public override bool SupportsBatch
		{
			get	{ return true;	}
		}

		public override bool HasMoreResults
		{
			get	{ return fieldCount > 0; }
		}


		#endregion

		public override void Open()
		{
			base.Open ();

			mysql = ClientAPI.Init(mysql);

			ClientFlags flags = ClientFlags.FOUND_ROWS | ClientFlags.MULTI_RESULTS | 
				ClientFlags.MULTI_STATEMENTS | ClientFlags.LOCAL_FILES;
			if (connectionString.UseCompression)
				flags |= ClientFlags.COMPRESS;
			if (connectionString.UseSSL)
				flags |= ClientFlags.SSL;

			object timeout = connectionString.ConnectionTimeout;
			ClientAPI.SetOptions(mysql, ClientAPIOption.MYSQL_OPT_CONNECT_TIMEOUT, 
				ref timeout);

			//TODO: support charset, shared memory, named pipes

			IntPtr result = ClientAPI.Connect(mysql, connectionString.Server, connectionString.UserId,
				connectionString.Password, connectionString.Database, connectionString.Port, 
				null, (uint)flags);
			if (result == IntPtr.Zero)
			{
				throw new MySqlException(ClientAPI.ErrorMsg(mysql), ClientAPI.ErrorNumber(mysql));
			}

			version = MySql.Data.Common.DBVersion.Parse(ClientAPI.VersionString(mysql));
			serverCharSet = ClientAPI.CharacterSetName(mysql);
		}

		public override void Close()
		{
			ClientAPI.Close(mysql);
			mysql = IntPtr.Zero;

			base.Close ();
		}

		public override bool Ping()
		{
			int val = ClientAPI.Ping(mysql);
			if (val == 0) return true;
			isOpen = false;
			return false;
		}

		public override CommandResult SendQuery(byte[] bytes, int length, bool consume)
		{
			string s = encoding.GetString(bytes, 0, bytes.Length);
			int result = ClientAPI.Query(mysql, bytes, (uint)length);
			if (result != 0)
				throw new MySqlException(ClientAPI.ErrorMsg(mysql), ClientAPI.ErrorNumber(mysql));

			fieldCount = ClientAPI.FieldCount(mysql);
			if (fieldCount > 0) 
			{
				// tell the client library we will be processing the results row by row
				resultSet = ClientAPI.UseResult(mysql);
				if (resultSet == IntPtr.Zero)
					throw new MySqlException(ClientAPI.ErrorMsg(mysql), ClientAPI.ErrorNumber(mysql));
			}

			return new CommandResult(this, false);
		}

		public override void SetDatabase(string dbName)
		{
			int result = ClientAPI.SelectDatabase(mysql, dbName);
			if (result == 0) return;

			MySqlException e = new MySqlException(
				ClientAPI.ErrorMsg(mysql), ClientAPI.ErrorNumber(mysql));
			Logger.LogException(e);
			throw e;
		}

		public override void Reset()
		{

		}


		public override PreparedStatement Prepare(string sql, string[] names)
		{
			return null;
		}

		public override bool ReadResult(ref long numfields, ref ulong affectedRows, ref long lastInsertId)
		{
			if (fieldCount == -1)
			{
				if (! version.isAtLeast(4,1,0)) return false;
				int result = ClientAPI.NextResult(mysql);
				if (result == -1) return false;
				if (result > 0)
					throw new MySqlException( ClientAPI.ErrorMsg(mysql), ClientAPI.ErrorNumber(mysql));

				fieldCount = ClientAPI.FieldCount(resultSet);
				if (fieldCount > 0) 
				{
					// now we use the resultset
					resultSet = ClientAPI.UseResult(mysql);
					if (resultSet == IntPtr.Zero)
						throw new MySqlException(ClientAPI.ErrorMsg(mysql), ClientAPI.ErrorNumber(mysql));
				}
			}

			affectedRows = 0;
			if (fieldCount == 0)
				affectedRows = ClientAPI.AffectedRows(mysql);
			lastInsertId = (long)ClientAPI.LastInsertId(mysql);
			numfields = fieldCount;
			fieldCount = -1;
			return true;
		}

		public override void SkipField(MySql.Data.Types.IMySqlValue valObject)
		{
			
		}

		public override MySql.Data.Types.IMySqlValue ReadFieldValue(int index, MySqlField field, MySql.Data.Types.IMySqlValue value)
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

		public override void ReadFieldMetadata(int count, ref MySqlField[] fields)
		{
			fields = new MySqlField[count];

			for (int i=0; i < count; i++)
			{
				ClientField fieldDef = ClientAPI.FetchField(resultSet);
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
			}

			currentLengths = new uint[count];
		}


		public override bool OpenDataRow(int fieldCount, bool isBinary, int statementId)
		{
			currentRow = ClientAPI.FetchRow(resultSet);

			IntPtr lengths = ClientAPI.FetchLengths(resultSet);
			for (int i=0; i < fieldCount; i++)
			{
				currentLengths[i] = (uint)Marshal.ReadInt32(lengths);
				lengths = (IntPtr)((int)lengths + 4);
			}
			
			if (currentRow == IntPtr.Zero)
			{
				int err = ClientAPI.ErrorNumber(mysql);
				if (err == 0) return false;
				throw new MySqlException(ClientAPI.ErrorMsg(mysql), err);
			}
			return true;
		}

		public override CommandResult ExecuteStatement(byte[] bytes, int statementId, int cursorPageSize)
		{
			return null;
		}

	}
#endif
}
