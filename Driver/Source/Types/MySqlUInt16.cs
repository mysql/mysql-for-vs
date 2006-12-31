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
using System.Data;
using MySql.Data.MySqlClient;

namespace MySql.Data.Types
{
	internal struct MySqlUInt16 : IMySqlValue
	{
		private ushort mValue;
		private bool isNull;

		public MySqlUInt16(bool isNull)
		{
			this.isNull = isNull;
			mValue = 0;
		}

		public MySqlUInt16(ushort val)
		{
			this.isNull = false;
			mValue = val;
		}

		#region IMySqlValue Members

		public bool IsNull
		{
			get { return isNull; }
		}

		MySqlDbType IMySqlValue.MySqlDbType
		{
			get { return MySqlDbType.UInt16; }
		}

		DbType IMySqlValue.DbType
		{
			get { return DbType.UInt16; }
		}

		object IMySqlValue.Value
		{
			get { return mValue; }
		}

		public ushort Value
		{
			get { return mValue; }
		}

		Type IMySqlValue.SystemType
		{
			get { return typeof(ushort); }
		}

		string IMySqlValue.MySqlTypeName
		{
			get { return "SMALLINT"; }
		}

		void IMySqlValue.WriteValue(MySqlStream stream, bool binary, object val, int length)
		{
			int v = Convert.ToUInt16(val);
			if (binary)
				stream.Write(BitConverter.GetBytes(v));
			else
				stream.WriteStringNoNull(v.ToString());
		}

		IMySqlValue IMySqlValue.ReadValue(MySqlStream stream, long length, bool nullVal)
		{
			if (nullVal)
				return new MySqlUInt16(true);

			if (length == -1)
				return new MySqlUInt16((ushort)stream.ReadInteger(2));
			else
				return new MySqlUInt16(UInt16.Parse(stream.ReadString(length)));
		}

		void IMySqlValue.SkipValue(MySqlStream stream)
		{
			stream.SkipBytes(2);
		}

		#endregion

		internal static void SetDSInfo(DataTable dsTable)
		{
			// we use name indexing because this method will only be called
			// when GetSchema is called for the DataSourceInformation 
			// collection and then it wil be cached.
			DataRow row = dsTable.NewRow();
			row["TypeName"] = "SMALLINT";
			row["ProviderDbType"] = MySqlDbType.UInt16;
			row["ColumnSize"] = 0;
			row["CreateFormat"] = "SMALLINT UNSIGNED";
			row["CreateParameters"] = null;
			row["DataType"] = "System.UInt16";
			row["IsAutoincrementable"] = true;
			row["IsBestMatch"] = true;
			row["IsCaseSensitive"] = false;
			row["IsFixedLength"] = true;
			row["IsFixedPrecisionScale"] = true;
			row["IsLong"] = false;
			row["IsNullable"] = true;
			row["IsSearchable"] = true;
			row["IsSearchableWithLike"] = false;
			row["IsUnsigned"] = true;
			row["MaximumScale"] = 0;
			row["MinimumScale"] = 0;
			row["IsConcurrencyType"] = DBNull.Value;
			row["IsLiteralsSupported"] = false;
			row["LiteralPrefix"] = null;
			row["LiteralSuffix"] = null;
			row["NativeDataType"] = null;
			dsTable.Rows.Add(row);
		}
	}
}
